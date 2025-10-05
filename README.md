# 🏢 Sistema Inmobiliaria

Sistema web completo para gestión de inmobiliarias desarrollado en ASP.NET Core MVC con Entity Framework Core y MySQL.

## 📋 Características Principales

### 🔐 Autenticación y Autorización
- Sistema de login con roles (Administrador/Empleado)
- Contraseñas encriptadas con BCrypt
- Protección de rutas por roles
- Perfil de usuario con cambio de contraseña y avatar

### 🏠 Gestión de Inmuebles
- CRUD completo de inmuebles
- Categorización por tipos (Casa, Departamento, Local, Oficina)
- Estados de disponibilidad
- Reportes por propietario y estado
- Estadísticas generales

### 👥 Gestión de Personas
- **Propietarios**: Dueños de inmuebles
- **Inquilinos**: Arrendatarios
- Información completa de contacto
- Validaciones de DNI únicos

### 📄 Gestión de Contratos
- Creación de contratos con validación de solapamiento
- Renovación de contratos
- Terminación anticipada con multas
- Auditoría completa (quién creó/terminó)
- Reportes de contratos vigentes, vencidos y por inmueble

### 💰 Gestión de Pagos
- Registro de pagos por período
- Sistema de anulación con motivo
- Soft-delete para recuperación
- Auditoría completa de operaciones
- Reportes de pagos por contrato

### 📊 Reportes y Estadísticas
- Contratos vigentes, vencidos y por inmueble
- Inmuebles libres, ocupados y por propietario
- Estadísticas generales con gráficos
- Ingresos mensuales estimados

## 🚀 Tecnologías Utilizadas

- **Backend**: ASP.NET Core 8.0 MVC
- **Base de Datos**: MySQL 8.0
- **ORM**: Entity Framework Core 8.0
- **Autenticación**: Cookie Authentication
- **Encriptación**: BCrypt.Net-Next
- **Frontend**: Bootstrap 5.3, Font Awesome 6.5
- **Validación**: Data Annotations
- **Migraciones**: EF Core Migrations

## 📦 Instalación y Configuración

### Prerrequisitos
- .NET 8.0 SDK
- MySQL 8.0
- Visual Studio 2022 o VS Code

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/tu-usuario/inmobiliaria.git
   cd inmobiliaria
   ```

2. **Configurar la base de datos**
   - Crear base de datos MySQL: `inmobiliaria`
   - Actualizar connection string en `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=inmobiliaria;Uid=root;Pwd=tu_password;SslMode=none;"
     }
   }
   ```

3. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

4. **Ejecutar migraciones**
   ```bash
   dotnet ef database update
   ```

5. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

6. **Acceder a la aplicación**
   - URL: `https://localhost:7000` o `http://localhost:5000`

## 👤 Credenciales de Demo

### Administrador
- **Email**: `admin@demo.com`
- **Contraseña**: `Admin123!`
- **Permisos**: Acceso completo a todas las funcionalidades

### Empleado
- **Email**: `empleado@demo.com`
- **Contraseña**: `Empleado123!`
- **Permisos**: Acceso limitado (no puede eliminar registros)

## 🗄️ Estructura de la Base de Datos

### Entidades Principales
- **Usuarios**: Administradores y empleados del sistema
- **Propietarios**: Dueños de inmuebles
- **Inquilinos**: Arrendatarios
- **TiposInmueble**: Categorías de inmuebles
- **Inmuebles**: Propiedades disponibles para alquiler
- **Contratos**: Acuerdos de alquiler
- **Pagos**: Registro de pagos de alquiler

### Características del Diseño
- **Auditoría completa**: Rastro de quién creó/modificó cada registro
- **Soft-delete**: Eliminación lógica para recuperación de datos
- **Integridad referencial**: Foreign keys con restricciones
- **Índices optimizados**: Para mejorar rendimiento

## 📁 Estructura del Proyecto

```
inmobiliaria/
├── Controllers/          # Controladores MVC
├── Models/              # Modelos de datos
├── Views/               # Vistas Razor
├── Data/                # Contexto de EF y inicializador
├── Services/            # Servicios de negocio
├── Migrations/          # Migraciones de EF
├── Scripts/             # Scripts SQL
├── Documentation/       # Documentación
└── wwwroot/            # Archivos estáticos
```

## 🔧 Funcionalidades por Módulo

### 🏠 Inmuebles
- ✅ CRUD completo
- ✅ Filtros por disponibilidad
- ✅ Reportes por propietario
- ✅ Estadísticas generales
- ✅ Categorización por tipos

### 👥 Propietarios e Inquilinos
- ✅ CRUD completo
- ✅ Validaciones de DNI únicos
- ✅ Información de contacto
- ✅ Búsqueda y filtrado

### 📄 Contratos
- ✅ Creación con validación de solapamiento
- ✅ Renovación automática
- ✅ Terminación anticipada
- ✅ Auditoría completa
- ✅ Reportes por estado

### 💰 Pagos
- ✅ Registro por período
- ✅ Anulación con motivo
- ✅ Soft-delete
- ✅ Auditoría completa
- ✅ Reportes por contrato

### 📊 Reportes
- ✅ Contratos vigentes/vencidos
- ✅ Inmuebles libres/ocupados
- ✅ Estadísticas generales
- ✅ Ingresos mensuales

## 🛡️ Seguridad

### Autenticación
- Login con email y contraseña
- Contraseñas encriptadas con BCrypt
- Sesiones con cookies seguras
- Timeout automático de sesión

### Autorización
- Roles: Administrador y Empleado
- Protección de rutas por roles
- Restricciones de eliminación para empleados
- Auditoría de operaciones

### Validaciones
- Validación en cliente y servidor
- Sanitización de datos
- Protección contra inyección SQL
- Validación de archivos subidos

## 📈 Rendimiento

### Optimizaciones
- Índices en foreign keys
- Consultas optimizadas con Include
- Paginación en listados
- Caché de datos estáticos

### Monitoreo
- Logs de errores
- Auditoría de operaciones
- Métricas de rendimiento
- Alertas de sistema

## 🧪 Testing

### Datos de Prueba
- Usuarios demo preconfigurados
- Propietarios e inquilinos de ejemplo
- Inmuebles con diferentes tipos
- Contratos y pagos históricos

### Casos de Prueba
- Flujo completo de alquiler
- Validaciones de negocio
- Reportes y estadísticas
- Manejo de errores

## 📚 Documentación

### Archivos de Documentación
- `README.md`: Este archivo
- `Documentation/diagrama_er.md`: Diagrama de base de datos
- `Scripts/backup_database.sql`: Script de respaldo
- Comentarios en código

### Diagramas
- Diagrama ER completo
- Flujo de procesos
- Arquitectura del sistema

## 🚀 Despliegue

### Requisitos de Producción
- Servidor con .NET 8.0 Runtime
- MySQL 8.0
- IIS o Nginx
- Certificado SSL

### Variables de Entorno
```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=prod-server;Database=inmobiliaria;Uid=user;Pwd=password;
```

## 🤝 Contribución

### Cómo Contribuir
1. Fork del repositorio
2. Crear rama feature: `git checkout -b feature/nueva-funcionalidad`
3. Commit cambios: `git commit -m 'Agregar nueva funcionalidad'`
4. Push a la rama: `git push origin feature/nueva-funcionalidad`
5. Crear Pull Request

### Estándares de Código
- Convenciones de C# (.NET)
- Comentarios en español
- Commits descriptivos
- Tests unitarios

## 📞 Soporte

### Contacto
- **Email**: soporte@inmobiliaria.com
- **Documentación**: [Wiki del proyecto](https://github.com/tu-usuario/inmobiliaria/wiki)
- **Issues**: [GitHub Issues](https://github.com/tu-usuario/inmobiliaria/issues)

### FAQ
- **¿Cómo cambiar la contraseña?**: Ve a Perfil > Cambiar Contraseña
- **¿Cómo restaurar datos eliminados?**: Solo administradores pueden restaurar
- **¿Cómo generar reportes?**: Usa el menú de Reportes en cada módulo

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

## 🎯 Roadmap

### Próximas Funcionalidades
- [ ] Notificaciones por email
- [ ] API REST para móviles
- [ ] Dashboard con gráficos avanzados
- [ ] Integración con sistemas de pago
- [ ] Documentos PDF automáticos
- [ ] Backup automático

### Mejoras Planificadas
- [ ] Optimización de consultas
- [ ] Caché Redis
- [ ] Logs estructurados
- [ ] Monitoreo de rendimiento
- [ ] Tests automatizados

---

**Desarrollado con ❤️ para la gestión inmobiliaria moderna**