-- script.sql
CREATE DATABASE PRAC2_DB;
GO

USE PRAC2_DB;
GO

CREATE TABLE Users (
    Email NVARCHAR(255) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    AccountId INT IDENTITY(1,1) NOT NULL
);
GO

INSERT INTO Users (Email, Name, Password)
VALUES 
    ('user1@example.com', 'user1', 'password123'),
    ('user2@example.com', 'user2', 'password456'),
    ('user3@example.com', 'user3', 'password789');
GO

CREATE TABLE News (
    id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Body NVARCHAR(MAX) NOT NULL,
    Datetime DATETIME DEFAULT GETDATE()
);
GO

INSERT INTO News (Title, Body)
VALUES 
    ('Primer titular', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.'),
    ('Segundo titular', 'Pellentesque vitae velit ex. Mauris venenatis ut elit quis tempus.'),
    ('Tercer titular', 'Aliquam erat volutpat. Curabitur convallis fringilla diam.');
GO

-- Cambiar al contexto de la base de datos
USE PRAC2_DB;
GO

-- Crear un usuario de inicio de sesión
CREATE LOGIN servicioWEB 
WITH PASSWORD = 'StrongPassword@123'; 
GO

-- Crear un usuario para la base de datos
USE PRAC2_DB;
GO

CREATE USER servicioWEB FOR LOGIN servicioWEB;
GO

-- Asignar permisos básicos de conexión y vista de base de datos
GRANT CONNECT TO servicioWEB;
GRANT VIEW ANY DATABASE TO servicioWEB;
GRANT CONNECT TO DATABASE::PRAC2_DB TO servicioWEB;
GO

-- Asignar permisos de lectura sobre las tablas
GRANT SELECT ON Users TO servicioWEB;
GRANT SELECT ON News TO servicioWEB;
GO

-- Denegar permisos de modificación explícitamente
DENY INSERT, DELETE, UPDATE ON Users TO servicioWEB;
DENY INSERT, DELETE, UPDATE ON News TO servicioWEB;
GO

-- Establecer PRAC2_DB como la base de datos predeterminada para el usuario
ALTER LOGIN servicioWEB WITH DEFAULT_DATABASE = PRAC2_DB;
GO