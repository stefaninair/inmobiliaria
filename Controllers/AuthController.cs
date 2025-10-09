using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Data;
using Inmobiliaria.Models;
using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string clave)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(clave))
                {
                    ViewBag.Error = "Email y clave son requeridos";
                    return View();
                }

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(clave, usuario.ClaveHash))
                {
                    ViewBag.Error = "Credenciales inválidas";
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.RolNombre)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                TempData["Success"] = $"Bienvenido, {usuario.Nombre}";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al iniciar sesión: " + ex.Message;
                return View();
            }
        }

        // GET: Auth/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Sesión cerrada exitosamente";
            return RedirectToAction("Login");
        }

        // GET: Auth/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Auth/Perfil
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");
            var usuario = await _context.Usuarios.FindAsync(userId);
            
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // POST: Auth/CambiarPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(string claveActual, string nuevaClave, string confirmarClave)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");
                var usuario = await _context.Usuarios.FindAsync(userId);
                
                if (usuario == null)
                    return NotFound();

                // Validar clave actual
                if (!BCrypt.Net.BCrypt.Verify(claveActual, usuario.ClaveHash))
                {
                    TempData["Error"] = "La clave actual es incorrecta.";
                    return RedirectToAction("Perfil");
                }

                // Validar nueva clave
                if (string.IsNullOrWhiteSpace(nuevaClave) || nuevaClave.Length < 6)
                {
                    TempData["Error"] = "La nueva clave debe tener al menos 6 caracteres.";
                    return RedirectToAction("Perfil");
                }

                if (nuevaClave != confirmarClave)
                {
                    TempData["Error"] = "Las claves nuevas no coinciden.";
                    return RedirectToAction("Perfil");
                }

                // Actualizar clave
                usuario.ClaveHash = BCrypt.Net.BCrypt.HashPassword(nuevaClave);
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Clave cambiada correctamente.";
                return RedirectToAction("Perfil");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cambiar la clave: " + ex.Message;
                return RedirectToAction("Perfil");
            }
        }

        // POST: Auth/CambiarAvatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarAvatar(IFormFile avatarFile)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");
                var usuario = await _context.Usuarios.FindAsync(userId);
                
                if (usuario == null)
                    return NotFound();

                if (avatarFile != null && avatarFile.Length > 0)
                {
                    // Validar tipo de archivo
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(avatarFile.ContentType))
                    {
                        TempData["Error"] = "Solo se permiten archivos JPG, PNG o GIF.";
                        return RedirectToAction("Perfil");
                    }

                    // Validar tamaño (máximo 2MB)
                    if (avatarFile.Length > 2 * 1024 * 1024)
                    {
                        TempData["Error"] = "El archivo no puede ser mayor a 2MB.";
                        return RedirectToAction("Perfil");
                    }

                    // Crear directorio si no existe
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                    if (!Directory.Exists(uploadsPath))
                        Directory.CreateDirectory(uploadsPath);

                    // Generar nombre único
                    var fileName = $"avatar_{userId}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(avatarFile.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    // Guardar archivo
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }

                    // Eliminar avatar anterior si existe
                    if (!string.IsNullOrEmpty(usuario.AvatarPath))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", usuario.AvatarPath);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    // Actualizar ruta del avatar
                    usuario.AvatarPath = $"uploads/avatars/{fileName}";
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Avatar actualizado correctamente.";
                }
                else
                {
                    TempData["Error"] = "Debe seleccionar un archivo.";
                }

                return RedirectToAction("Perfil");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cambiar el avatar: " + ex.Message;
                return RedirectToAction("Perfil");
            }
        }
    }
}
