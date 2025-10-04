using Inmobiliaria.Models;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Inyección de dependencias para el repositorio de Propietarios y Inquilinos
builder.Services.AddSingleton<RepositorioPropietario>();
builder.Services.AddSingleton<RepositorioInquilino>();
builder.Services.AddSingleton<RepositorioInmueble>();
builder.Services.AddTransient<RepositorioContrato>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();