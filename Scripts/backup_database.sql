-- Script de Backup para Base de Datos Inmobiliaria
-- Fecha: 2025-01-10
-- Descripción: Script SQL para crear la estructura de la base de datos

-- Crear base de datos
CREATE DATABASE IF NOT EXISTS inmobiliaria CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE inmobiliaria;

-- Tabla Usuarios
CREATE TABLE IF NOT EXISTS Usuarios (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Email VARCHAR(100) NOT NULL UNIQUE,
    ClaveHash TEXT NOT NULL,
    Rol INT NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    AvatarPath TEXT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla Propietarios
CREATE TABLE IF NOT EXISTS Propietarios (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Dni VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(20) NULL,
    Email VARCHAR(100) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla Inquilinos
CREATE TABLE IF NOT EXISTS Inquilinos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Dni VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(20) NULL,
    Email VARCHAR(100) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla TiposInmueble
CREATE TABLE IF NOT EXISTS TiposInmueble (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla Inmuebles
CREATE TABLE IF NOT EXISTS Inmuebles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Direccion VARCHAR(200) NOT NULL,
    Uso VARCHAR(50) NOT NULL,
    Ambientes INT NOT NULL,
    Superficie DECIMAL(10,2) NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Disponible BOOLEAN NOT NULL DEFAULT TRUE,
    PropietarioId INT NOT NULL,
    TipoInmuebleId INT NOT NULL,
    Observaciones VARCHAR(500) NULL,
    Latitud DECIMAL(10,8) NOT NULL DEFAULT 0,
    Longitud DECIMAL(11,8) NOT NULL DEFAULT 0,
    Portada TEXT NULL,
    Habilitado BOOLEAN NOT NULL DEFAULT TRUE,
    FOREIGN KEY (PropietarioId) REFERENCES Propietarios(Id) ON DELETE RESTRICT,
    FOREIGN KEY (TipoInmuebleId) REFERENCES TiposInmueble(Id) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla Contratos
CREATE TABLE IF NOT EXISTS Contratos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    InquilinoId INT NOT NULL,
    InmuebleId INT NOT NULL,
    MontoMensual DECIMAL(10,2) NOT NULL,
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,
    FechaTerminacionAnticipada DATE NULL,
    Multa DECIMAL(10,2) NULL,
    CreadoPorUserId INT NOT NULL,
    CreadoEn DATETIME NOT NULL,
    TerminadoPorUserId INT NULL,
    TerminadoEn DATETIME NULL,
    FOREIGN KEY (InquilinoId) REFERENCES Inquilinos(Id) ON DELETE RESTRICT,
    FOREIGN KEY (InmuebleId) REFERENCES Inmuebles(Id) ON DELETE RESTRICT,
    FOREIGN KEY (CreadoPorUserId) REFERENCES Usuarios(Id) ON DELETE RESTRICT,
    FOREIGN KEY (TerminadoPorUserId) REFERENCES Usuarios(Id) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla Pagos
CREATE TABLE IF NOT EXISTS Pagos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ContratoId INT NOT NULL,
    Monto DECIMAL(10,2) NOT NULL,
    FechaPago DATE NOT NULL,
    Periodo VARCHAR(20) NOT NULL,
    Observaciones VARCHAR(500) NULL,
    CreadoPorUserId INT NOT NULL,
    CreadoEn DATETIME NOT NULL,
    AnuladoPorUserId INT NULL,
    AnuladoEn DATETIME NULL,
    MotivoAnulacion VARCHAR(200) NULL,
    Eliminado BOOLEAN NOT NULL DEFAULT FALSE,
    EliminadoPorUserId INT NULL,
    EliminadoEn DATETIME NULL,
    FOREIGN KEY (ContratoId) REFERENCES Contratos(Id) ON DELETE RESTRICT,
    FOREIGN KEY (CreadoPorUserId) REFERENCES Usuarios(Id) ON DELETE RESTRICT,
    FOREIGN KEY (AnuladoPorUserId) REFERENCES Usuarios(Id) ON DELETE RESTRICT,
    FOREIGN KEY (EliminadoPorUserId) REFERENCES Usuarios(Id) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Índices para mejorar rendimiento
CREATE INDEX IX_Inmuebles_PropietarioId ON Inmuebles(PropietarioId);
CREATE INDEX IX_Inmuebles_TipoInmuebleId ON Inmuebles(TipoInmuebleId);
CREATE INDEX IX_Contratos_InquilinoId ON Contratos(InquilinoId);
CREATE INDEX IX_Contratos_InmuebleId ON Contratos(InmuebleId);
CREATE INDEX IX_Contratos_CreadoPorUserId ON Contratos(CreadoPorUserId);
CREATE INDEX IX_Contratos_TerminadoPorUserId ON Contratos(TerminadoPorUserId);
CREATE INDEX IX_Pagos_ContratoId ON Pagos(ContratoId);
CREATE INDEX IX_Pagos_CreadoPorUserId ON Pagos(CreadoPorUserId);
CREATE INDEX IX_Pagos_AnuladoPorUserId ON Pagos(AnuladoPorUserId);
CREATE INDEX IX_Pagos_EliminadoPorUserId ON Pagos(EliminadoPorUserId);

-- Datos de prueba
INSERT INTO Usuarios (Email, ClaveHash, Rol, Nombre, AvatarPath) VALUES
('admin@demo.com', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 1, 'Administrador Demo', NULL),
('empleado@demo.com', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 2, 'Empleado Demo', NULL);

INSERT INTO Propietarios (Nombre, Apellido, Dni, Telefono, Email) VALUES
('Juan', 'Pérez', '12345678', '1234567890', 'juan.perez@email.com'),
('María', 'González', '87654321', '0987654321', 'maria.gonzalez@email.com'),
('Carlos', 'López', '11223344', '1122334455', 'carlos.lopez@email.com'),
('Ana', 'Martín', '44332211', '4433221100', 'ana.martin@email.com');

INSERT INTO Inquilinos (Nombre, Apellido, Dni, Telefono, Email) VALUES
('Pedro', 'Rodríguez', '55667788', '5566778899', 'pedro.rodriguez@email.com'),
('Laura', 'Fernández', '99887766', '9988776655', 'laura.fernandez@email.com'),
('Miguel', 'Sánchez', '33445566', '3344556677', 'miguel.sanchez@email.com'),
('Sofía', 'Torres', '66778899', '6677889900', 'sofia.torres@email.com');

INSERT INTO TiposInmueble (Nombre) VALUES
('Casa'),
('Departamento'),
('Local Comercial'),
('Oficina');

-- Nota: Los datos de inmuebles, contratos y pagos se insertan automáticamente
-- por la aplicación al inicializar la base de datos

