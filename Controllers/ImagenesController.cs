using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class ImagenesController : Controller
    {
        private readonly RepositorioImagen repositorio;

        public ImagenesController(RepositorioImagen repositorio)
        {
            this.repositorio = repositorio;
        }
        
        [HttpPost]
        public async Task<IActionResult> Alta(int id, List<IFormFile> imagenes, [FromServices] IWebHostEnvironment environment)
        {
            Console.WriteLine($"Alta: id={id}, imagenes count={imagenes?.Count ?? 0}");
            if (imagenes == null || imagenes.Count == 0)
                return BadRequest("No se recibieron archivos.");
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, "Inmuebles");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach (var file in imagenes)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    var rutaArchivo = Path.Combine(path, nombreArchivo);

                    Console.WriteLine($"Guardando archivo: {rutaArchivo}");
                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    Imagen imagen = new Imagen
                    {
                        InmuebleId = id,
                        Url = $"/Uploads/Inmuebles/{id}/{nombreArchivo}",
                    };
                    Console.WriteLine($"Guardando en BD: {imagen.Url}");
                    repositorio.Alta(imagen);
                }
            }
            var resultado = repositorio.BuscarPorInmueble(id);
            Console.WriteLine($"Retornando {resultado.Count} imágenes");
            return Ok(resultado);
        }

        // POST: Imagenes/Eliminar/5
        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad != null)
                {
                    // Eliminar el archivo físico
                    if (!string.IsNullOrEmpty(entidad.Url))
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", entidad.Url.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    
                    repositorio.Baja(id);
                    return Ok(repositorio.BuscarPorInmueble(entidad.InmuebleId));
                }
                return BadRequest("Imagen no encontrada");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
