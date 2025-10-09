using Inmobiliaria.Models;
using Inmobiliaria.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar ADO.NET
builder.Services.AddSingleton<DatabaseConnection>();

// Configurar Data Protection
builder.Services.AddDataProtection();

// Configurar Autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "InmobiliariaAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// Configurar Autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloAdmin", policy =>
        policy.RequireRole("Administrador"));
    
    // No usar política global para evitar bucles de redirección
    // La autenticación se manejará a nivel de controlador
});

// Configurar HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Registrar servicios
builder.Services.AddScoped<Inmobiliaria.Services.PagoService>();

// Inyección de dependencias para los repositorios
builder.Services.AddScoped<RepositorioPropietario>();
builder.Services.AddScoped<RepositorioInquilino>();
builder.Services.AddScoped<RepositorioInmueble>();
builder.Services.AddScoped<RepositorioContrato>();
builder.Services.AddScoped<RepositorioTipoInmueble>();


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

app.UseAuthentication();
app.UseAuthorization();

// Rutas personalizadas
app.MapControllerRoute("login", "entrar/{**accion}", new { controller = "Auth", action = "Login" });
app.MapControllerRoute("rutaFija", "ruteo/{valor}", new { controller = "Home", action = "Ruta", valor = "defecto" });
app.MapControllerRoute("fechas", "{controller=Home}/{action=Fecha}/{anio}/{mes}/{dia}");

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Inicializar datos de prueba en desarrollo
if (app.Environment.IsDevelopment())
{
    var dbConnection = app.Services.GetRequiredService<DatabaseConnection>();
    
    // Corregir la base de datos si es necesario
    FixDatabase(dbConnection);
    
    DbInitializer.Initialize(dbConnection);
}

// Método para corregir la base de datos
static void FixDatabase(DatabaseConnection dbConnection)
{
    try
    {
        using var connection = dbConnection.GetConnection();
        connection.Open();

        // Verificar si la columna AvatarPath existe
        var checkColumnQuery = @"
            SELECT COUNT(*) 
            FROM pragma_table_info('Usuarios') 
            WHERE name = 'AvatarPath'";

        using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = checkColumnQuery;
        var columnExists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;

        if (!columnExists)
        {
            // Agregar la columna AvatarPath
            var addColumnQuery = "ALTER TABLE Usuarios ADD COLUMN AvatarPath TEXT";
            using var addCommand = connection.CreateCommand();
            addCommand.CommandText = addColumnQuery;
            addCommand.ExecuteNonQuery();
            Console.WriteLine("Columna AvatarPath agregada exitosamente.");
        }
        else
        {
            Console.WriteLine("La columna AvatarPath ya existe.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al corregir la base de datos: {ex.Message}");
    }
}

app.Run();