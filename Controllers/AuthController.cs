using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Inmobiliaria.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SQLite;

namespace Inmobiliaria.Controllers
{
    public class AuthController : Controller
    {
        private readonly DatabaseConnection _dbConnection;

        public AuthController(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string clave)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(clave))
                {
                    TempData["Error"] = "Email y contraseña son requeridos.";
                    return View();
                }

                using var connection = _dbConnection.GetConnection();
                connection.Open();

                var query = "SELECT Id, Nombre, Apellido, Email, Clave, Rol FROM Usuarios WHERE Email = @email";
                using var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.Add(CreateParameter(command, "@email", email));

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var hashedPassword = reader.GetString("Clave");
                    if (BCrypt.Net.BCrypt.Verify(clave, hashedPassword))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, reader.GetString("Nombre")),
                            new Claim(ClaimTypes.Surname, reader.GetString("Apellido")),
                            new Claim(ClaimTypes.Email, reader.GetString("Email")),
                            new Claim(ClaimTypes.Role, reader.GetString("Rol")),
                            new Claim("UserId", reader.GetInt32("Id").ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                        TempData["Success"] = "Inicio de sesión exitoso.";
                        return RedirectToAction("Index", "Home");
                    }
                }

                TempData["Error"] = "Credenciales inválidas.";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al iniciar sesión: {ex.Message}";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Perfil()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                using var connection = _dbConnection.GetConnection();
                connection.Open();

                var query = "SELECT Id, Nombre, Email, Rol, AvatarPath FROM Usuarios WHERE Id = @id";
                using var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.Add(CreateParameter(command, "@id", int.Parse(userIdClaim)));

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var usuario = new Usuario
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Email = reader.GetString("Email"),
                        Rol = Enum.Parse<Rol>(reader.GetString("Rol")),
                        AvatarPath = reader.IsDBNull("AvatarPath") ? null : reader.GetString("AvatarPath")
                    };
                    return View(usuario);
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el perfil: {ex.Message}";
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CambiarAvatar(IFormFile avatarFile)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                if (avatarFile == null || avatarFile.Length == 0)
                {
                    TempData["Error"] = "Por favor selecciona un archivo.";
                    return RedirectToAction("Perfil");
                }

                // Validar tipo de archivo
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(avatarFile.ContentType.ToLower()))
                {
                    TempData["Error"] = "Solo se permiten archivos JPG, PNG y GIF.";
                    return RedirectToAction("Perfil");
                }

                // Validar tamaño (2MB máximo)
                if (avatarFile.Length > 2 * 1024 * 1024)
                {
                    TempData["Error"] = "El archivo no puede ser mayor a 2MB.";
                    return RedirectToAction("Perfil");
                }

                // Crear directorio de avatares si no existe
                var avatarsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                if (!Directory.Exists(avatarsDir))
                {
                    Directory.CreateDirectory(avatarsDir);
                }

                // Generar nombre único para el archivo
                var fileExtension = Path.GetExtension(avatarFile.FileName);
                var fileName = $"avatar_{userIdClaim}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(avatarsDir, fileName);
                var relativePath = Path.Combine("uploads", "avatars", fileName).Replace("\\", "/");

                // Guardar archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                // Actualizar base de datos
                using var connection = _dbConnection.GetConnection();
                connection.Open();

                var query = "UPDATE Usuarios SET AvatarPath = @avatarPath WHERE Id = @id";
                using var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.Add(CreateParameter(command, "@avatarPath", relativePath));
                command.Parameters.Add(CreateParameter(command, "@id", int.Parse(userIdClaim)));

                command.ExecuteNonQuery();

                TempData["Success"] = "Avatar actualizado exitosamente.";
                return RedirectToAction("Perfil");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cambiar avatar: {ex.Message}";
                return RedirectToAction("Perfil");
            }
        }

        [HttpPost]
        public IActionResult CambiarPassword(string claveActual, string nuevaClave, string confirmarClave)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                if (string.IsNullOrEmpty(claveActual) || string.IsNullOrEmpty(nuevaClave) || string.IsNullOrEmpty(confirmarClave))
                {
                    TempData["Error"] = "Todos los campos son requeridos.";
                    return RedirectToAction("Perfil");
                }

                if (nuevaClave != confirmarClave)
                {
                    TempData["Error"] = "Las contraseñas no coinciden.";
                    return RedirectToAction("Perfil");
                }

                if (nuevaClave.Length < 6)
                {
                    TempData["Error"] = "La nueva contraseña debe tener al menos 6 caracteres.";
                    return RedirectToAction("Perfil");
                }

                using var connection = _dbConnection.GetConnection();
                connection.Open();

                // Verificar contraseña actual
                var verifyQuery = "SELECT Clave FROM Usuarios WHERE Id = @id";
                using var verifyCommand = connection.CreateCommand();
                verifyCommand.CommandText = verifyQuery;
                verifyCommand.Parameters.Add(CreateParameter(verifyCommand, "@id", int.Parse(userIdClaim)));

                using var reader = verifyCommand.ExecuteReader();
                if (reader.Read())
                {
                    var hashedPassword = reader.GetString("Clave");
                    if (!BCrypt.Net.BCrypt.Verify(claveActual, hashedPassword))
                    {
                        TempData["Error"] = "La contraseña actual es incorrecta.";
                        return RedirectToAction("Perfil");
                    }
                }
                else
                {
                    TempData["Error"] = "Usuario no encontrado.";
                    return RedirectToAction("Perfil");
                }

                // Actualizar contraseña
                var updateQuery = "UPDATE Usuarios SET Clave = @nuevaClave WHERE Id = @id";
                using var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = updateQuery;
                updateCommand.Parameters.Add(CreateParameter(updateCommand, "@nuevaClave", BCrypt.Net.BCrypt.HashPassword(nuevaClave)));
                updateCommand.Parameters.Add(CreateParameter(updateCommand, "@id", int.Parse(userIdClaim)));

                updateCommand.ExecuteNonQuery();

                TempData["Success"] = "Contraseña actualizada exitosamente.";
                return RedirectToAction("Perfil");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cambiar contraseña: {ex.Message}";
                return RedirectToAction("Perfil");
            }
        }

        private IDbDataParameter CreateParameter(IDbCommand command, string parameterName, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }
    }
}