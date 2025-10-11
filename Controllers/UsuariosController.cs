using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Inmobiliaria.Data;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SQLite;

namespace Inmobiliaria.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly DatabaseConnection _dbConnection;

        public UsuariosController(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // GET: Usuarios
        public IActionResult Index()
        {
            var usuarios = new List<Usuario>();
            try
            {
                using var connection = _dbConnection.GetConnection();
                connection.Open();

                var query = "SELECT Id, Nombre, Apellido, Email, Clave, Rol, AvatarPath FROM Usuarios ORDER BY Id";
                using var command = connection.CreateCommand();
                command.CommandText = query;

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    usuarios.Add(new Usuario
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Email = reader.GetString("Email"),
                        Rol = Enum.Parse<Rol>(reader.GetString("Rol")),
                        ClaveHash = reader.GetString("Clave"),
                        AvatarPath = reader.IsDBNull("AvatarPath") ? null : reader.GetString("AvatarPath")
                    });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar usuarios: {ex.Message}";
            }

            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                using var connection = _dbConnection.GetConnection();
                connection.Open();

                var query = "SELECT Id, Nombre, Apellido, Email, Clave, Rol, AvatarPath FROM Usuarios WHERE Id = @id";
                using var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.Add(CreateParameter(command, "@id", id));

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var usuario = new Usuario
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Email = reader.GetString("Email"),
                        Rol = Enum.Parse<Rol>(reader.GetString("Rol")),
                        ClaveHash = reader.GetString("Clave"),
                        AvatarPath = reader.IsDBNull("AvatarPath") ? null : reader.GetString("AvatarPath")
                    };
                    return View(usuario);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el usuario: {ex.Message}";
            }

            return NotFound();
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario)
        {
            // Debug: Log de los valores recibidos
            Console.WriteLine($"Usuario recibido - Nombre: {usuario.Nombre}, Apellido: {usuario.Apellido}, Email: {usuario.Email}, Clave: {usuario.Clave}, Rol: {usuario.Rol}");
            
            // Debug: Log del estado del modelo
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var errorMessage = $"Errores de validación: {string.Join(", ", errors)}";
                Console.WriteLine(errorMessage);
                
                // Debug: Log de cada error individual
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state?.Errors.Any() == true)
                    {
                        Console.WriteLine($"Error en {key}: {string.Join(", ", state.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
                
                TempData["Error"] = errorMessage;
                return View(usuario);
            }

            try
            {
                using var connection = _dbConnection.GetConnection();
                connection.Open();

                var query = "INSERT INTO Usuarios (Nombre, Apellido, Email, Clave, Rol) VALUES (@nombre, @apellido, @email, @clave, @rol)";
                using var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.Add(CreateParameter(command, "@nombre", usuario.Nombre));
                command.Parameters.Add(CreateParameter(command, "@apellido", usuario.Apellido));
                command.Parameters.Add(CreateParameter(command, "@email", usuario.Email));
                command.Parameters.Add(CreateParameter(command, "@clave", BCrypt.Net.BCrypt.HashPassword(usuario.Clave)));
                command.Parameters.Add(CreateParameter(command, "@rol", usuario.Rol.ToString()));

                Console.WriteLine($"Ejecutando query: {query}");
                Console.WriteLine($"Parámetros: Nombre={usuario.Nombre}, Apellido={usuario.Apellido}, Email={usuario.Email}, Rol={usuario.Rol}");

                var rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"Filas afectadas: {rowsAffected}");
                
                if (rowsAffected > 0)
                {
                    TempData["Success"] = "Usuario creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "No se pudo crear el usuario. No se insertaron filas.";
                    return View(usuario);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear usuario: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = $"Error al crear el usuario: {ex.Message}";
                return View(usuario);
            }
        }

        // GET: Usuarios/Edit/5
        public IActionResult Edit(int id)
        {
            try
            {
                using var connection = _dbConnection.GetConnection();
                connection.Open();

                var query = "SELECT Id, Nombre, Apellido, Email, Clave, Rol, AvatarPath FROM Usuarios WHERE Id = @id";
                using var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.Add(CreateParameter(command, "@id", id));

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var usuario = new Usuario
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Email = reader.GetString("Email"),
                        Rol = Enum.Parse<Rol>(reader.GetString("Rol")),
                        ClaveHash = reader.GetString("Clave"),
                        AvatarPath = reader.IsDBNull("AvatarPath") ? null : reader.GetString("AvatarPath")
                    };
                    return View(usuario);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el usuario: {ex.Message}";
            }

            return NotFound();
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using var connection = _dbConnection.GetConnection();
                    connection.Open();

                    var query = "UPDATE Usuarios SET Nombre = @nombre, Apellido = @apellido, Email = @email, Rol = @rol WHERE Id = @id";
                    using var command = connection.CreateCommand();
                    command.CommandText = query;
                    command.Parameters.Add(CreateParameter(command, "@id", usuario.Id));
                    command.Parameters.Add(CreateParameter(command, "@nombre", usuario.Nombre));
                    command.Parameters.Add(CreateParameter(command, "@apellido", usuario.Apellido));
                    command.Parameters.Add(CreateParameter(command, "@email", usuario.Email));
                    command.Parameters.Add(CreateParameter(command, "@rol", usuario.Rol.ToString()));

                    command.ExecuteNonQuery();

                    TempData["Success"] = "Usuario actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al actualizar el usuario: {ex.Message}";
                }
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public IActionResult Delete(int id)
        {
            try
            {
                using var connection = _dbConnection.GetConnection();
                connection.Open();

                var query = "SELECT Id, Nombre, Apellido, Email, Clave, Rol, AvatarPath FROM Usuarios WHERE Id = @id";
                using var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.Add(CreateParameter(command, "@id", id));

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var usuario = new Usuario
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Email = reader.GetString("Email"),
                        Rol = Enum.Parse<Rol>(reader.GetString("Rol")),
                        ClaveHash = reader.GetString("Clave"),
                        AvatarPath = reader.IsDBNull("AvatarPath") ? null : reader.GetString("AvatarPath")
                    };
                    return View(usuario);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el usuario: {ex.Message}";
            }

            return NotFound();
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                using var connection = _dbConnection.GetConnection();
                connection.Open();

                var query = "DELETE FROM Usuarios WHERE Id = @id";
                using var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.Add(CreateParameter(command, "@id", id));

                command.ExecuteNonQuery();

                TempData["Success"] = "Usuario eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el usuario: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
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