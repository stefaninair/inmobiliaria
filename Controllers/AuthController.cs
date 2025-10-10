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

        [HttpPost]
        public async Task<IActionResult> Logout()
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

                var query = "SELECT Id, Nombre, Email, Rol FROM Usuarios WHERE Id = @id";
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
                        Rol = Enum.Parse<Rol>(reader.GetString("Rol"))
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

        private IDbDataParameter CreateParameter(IDbCommand command, string parameterName, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }
    }
}