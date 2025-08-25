Sistema de Inmobiliaria

Tecnologías
Basado en .Net Core 8

Herramientas
Framework .Net Core. Indispensable.
Visual Studio Code. Para el trabajo día a día.
Github Desktop. Opcional, se puede integrar Github en VS Code.
Postman. Para hacer peticiones a la Web API.
Diagramas. Para modelar diagramas.

Extensiones
C# para VS Code. Extensión de C# para VS Code.

Comandos de creación y ejecución:
dotnet new mvc -n inmobiliaria
dotnet run

Creación de la carpeta .gitignore :
[Bb]in/
[Oo]bj/

Creacion de la carpeta Data donde se encuentra la BD y el DER
Creacion de MVC y ABM de Propietario.
Creacion de MVC y ABM de Inquilino.

Configuración de la Base de Datos: Se creó una base de datos llamada inmobiliaria en phpMyAdmin (MySQL). 

Comandos ejecutados para su correco funcionamiento:
using System.ComponentModel.DataAnnotations;
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Pomelo.EntityFrameworkCore.MySql
using Microsoft.EntityFrameworkCore;



