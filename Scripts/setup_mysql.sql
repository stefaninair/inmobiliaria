-- Script para configurar MySQL para la aplicación Inmobiliaria
-- Ejecutar este script como administrador de MySQL

-- Crear la base de datos
CREATE DATABASE IF NOT EXISTS inmobiliaria CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Crear usuario específico para la aplicación (opcional)
CREATE USER IF NOT EXISTS 'inmobiliaria_user'@'localhost' IDENTIFIED BY 'inmobiliaria123';
GRANT ALL PRIVILEGES ON inmobiliaria.* TO 'inmobiliaria_user'@'localhost';
FLUSH PRIVILEGES;

-- Configurar usuario root para permitir conexión sin contraseña (solo para desarrollo)
-- ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '';
-- FLUSH PRIVILEGES;

-- Mostrar información de conexión
SELECT 'Base de datos inmobiliaria creada exitosamente' as Status;
SELECT 'Usuario: inmobiliaria_user' as Usuario;
SELECT 'Contraseña: inmobiliaria123' as Password;
SELECT 'Base de datos: inmobiliaria' as Database;

