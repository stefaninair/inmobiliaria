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

                var query = "SELECT Id, Nombre, Email, Clave, Rol FROM Usuarios ORDER BY Id";
                using var command = connection.CreateCommand();
                command.CommandText = query;

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    usuarios.Add(new Usuario
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Email = reader.GetString("Email"),
                        Rol = Enum.Parse<Rol>(reader.GetString("Rol")),
                        ClaveHash = reader.GetString("Clave")
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

                var query = "SELECT Id, Nombre, Email, Clave, Rol FROM Usuarios WHERE Id = @id";
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
                        Email = reader.GetString("Email"),
                        Rol = Enum.Parse<Rol>(reader.GetString("Rol")),
                        ClaveHash = reader.GetString("Clave")
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
            if (ModelState.IsValid)
            {
                try
                {
                    using var connection = _dbConnection.GetConnection();
                    connection.Open();

                    var query = "INSERT INTO Usuarios (Nombre, Email, Clave, Rol) VALUES (@nombre, @email, @clave, @rol)";
                    using var command = connection.CreateCommand();
                    command.CommandText = query;
                    command.Parameters.Add(CreateParameter(command, "@nombre", usuario.Nombre));
                    command.Parameters.Add(CreateParameter(command, "@email", usuario.Email));
                    command.Parameters.Add(CreateParameter(command, "@clave", BCrypt.Net.BCrypt.HashPassword(usuario.ClaveHash)));
                    command.Parameters.Add(CreateParameter(command, "@rol", usuario.Rol.ToString()));

                    command.ExecuteNonQuery();

                    TempData["Success"] = "Usuario creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al crear el usuario: {ex.Message}";
                }
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public IActionResult Edit(int id)
        {
            try
            {
                using var connection = _dbConnection.GetConnection();
                connection.Open();

                var query = "SELECT Id, Nombre, Email, Clave, Rol FROM Usuarios WHERE Id = @id";
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
                        Email = reader.GetString("Email"),
                        Rol = Enum.Parse<Rol>(reader.GetString("Rol")),
                        ClaveHash = reader.GetString("Clave")
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

                    var query = "UPDATE Usuarios SET Nombre = @nombre, Email = @email, Rol = @rol WHERE Id = @id";
                    using var command = connection.CreateCommand();
                    command.CommandText = query;
                    command.Parameters.Add(CreateParameter(command, "@id", usuario.Id));
                    command.Parameters.Add(CreateParameter(command, "@nombre", usuario.Nombre));
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

                var query = "SELECT Id, Nombre, Email, Clave, Rol FROM Usuarios WHERE Id = @id";
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
                        Email = reader.GetString("Email"),
                        Rol = Enum.Parse<Rol>(reader.GetString("Rol")),
                        ClaveHash = reader.GetString("Clave")
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