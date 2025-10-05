using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Models;
using Inmobiliaria.Data;
using Microsoft.AspNetCore.Hosting;

namespace Inmobiliaria.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment env;

        public InmueblesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            env = environment;
        }

        private async Task CargarPropietarios()
        {
            ViewBag.Propietarios = await _context.Propietarios.ToListAsync();
        }

        private async Task CargarTiposInmueble()
        {
            ViewBag.TiposInmueble = await _context.TiposInmueble.ToListAsync();
        }

        // GET: Inmuebles
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Inmuebles
                .Include(i => i.Propietario)
                .Include(i => i.TipoInmueble)
                .ToListAsync();
            ViewBag.Id = TempData["Id"];
            ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        // GET: Inmuebles/Disponibles
        public async Task<IActionResult> Disponibles()
        {
            var lista = await _context.Inmuebles
                .Where(i => i.Disponible)
                .Include(i => i.Propietario)
                .Include(i => i.TipoInmueble)
                .ToListAsync();
            return View(lista);
        }

        // GET: Inmuebles/Create
        public async Task<IActionResult> Create()
        {
            await CargarPropietarios();
            await CargarTiposInmueble();
            return View();
        }

        // POST: Inmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inmueble entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (entidad.PortadaFile != null)
                    {
                        string ruta = Path.Combine(env.WebRootPath, "img", entidad.PortadaFile.FileName);
                        using (var stream = new FileStream(ruta, FileMode.Create))
                        {
                            entidad.PortadaFile.CopyTo(stream);
                        }
                        entidad.Portada = entidad.PortadaFile.FileName;
                    }

                    _context.Add(entidad);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Inmueble creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Errores de validación: " + string.Join(", ", errors);
                await CargarPropietarios();
                await CargarTiposInmueble();
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                await CargarPropietarios();
                await CargarTiposInmueble();
                return View(entidad);
            }
        }

        // GET: Inmuebles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var entidad = await _context.Inmuebles.FindAsync(id);
            if (entidad == null)
                return NotFound();

            await CargarPropietarios();
            await CargarTiposInmueble();
            return View(entidad);
        }

        // POST: Inmuebles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inmueble entidad)
        {
            try
            {
                if (id != entidad.Id)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    if (entidad.PortadaFile != null)
                    {
                        string ruta = Path.Combine(env.WebRootPath, "img", entidad.PortadaFile.FileName);
                        using (var stream = new FileStream(ruta, FileMode.Create))
                        {
                            entidad.PortadaFile.CopyTo(stream);
                        }
                        entidad.Portada = entidad.PortadaFile.FileName;
                    }

                    _context.Update(entidad);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Inmueble actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Errores de validación: " + string.Join(", ", errors);
                await CargarPropietarios();
                await CargarTiposInmueble();
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                await CargarPropietarios();
                await CargarTiposInmueble();
                return View(entidad);
            }
        }

        // GET: Inmuebles/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var entidad = await _context.Inmuebles
                .Include(i => i.Propietario)
                .Include(i => i.TipoInmueble)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (entidad == null)
                return NotFound();

            return View(entidad);
        }

        // GET: Inmuebles/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var entidad = await _context.Inmuebles
                .Include(i => i.Propietario)
                .Include(i => i.TipoInmueble)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (entidad == null)
                return NotFound();

            ViewBag.Mensaje = TempData["Mensaje"];
            ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: Inmuebles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var entidad = await _context.Inmuebles.FindAsync(id);
                if (entidad != null)
                {
                    _context.Inmuebles.Remove(entidad);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Inmueble eliminado exitosamente.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                var entidad = await _context.Inmuebles
                    .Include(i => i.Propietario)
                    .Include(i => i.TipoInmueble)
                    .FirstOrDefaultAsync(i => i.Id == id);
                return View(entidad);
            }
        }
    }
}