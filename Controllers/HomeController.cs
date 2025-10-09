using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliaria.Models;

namespace inmobiliaria.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Ruta(string valor)
    {
        ViewBag.Valor = valor;
        return View();
    }

    public IActionResult Fecha(int anio, int mes, int dia)
    {
        try
        {
            var fecha = new DateTime(anio, mes, dia);
            ViewBag.Fecha = fecha;
        }
        catch
        {
            ViewBag.Fecha = DateTime.Now;
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
