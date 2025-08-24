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

Comandos
Crear: dotnet new mvc -n inmobiliaria
Para Correrlo: dotnet run

Cree la carpeta .gitignore y añadi :
[Bb]in/
[Oo]bj/

Creacion de ABM de Propietario.

Configuración de la Base de Datos: Se creó una base de datos llamada inmobiliaria en phpMyAdmin (MySQL). 

Comandos ejecutados:
using System.ComponentModel.DataAnnotations;
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Pomelo.EntityFrameworkCore.MySql
