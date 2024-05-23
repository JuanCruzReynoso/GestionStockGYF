CREATE DATABASE GestionStockDB;

USE GestionStockDB;

CREATE TABLE Categorias (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(50) NOT NULL
);

INSERT INTO Categorias (Nombre) VALUES ('Computación'), ('Telefonía');

CREATE TABLE Productos (
    Id INT PRIMARY KEY IDENTITY,
    Precio DECIMAL(18, 2) NOT NULL,
    FechaCarga DATETIME NOT NULL,
    IdCategoria INT NOT NULL,
    FOREIGN KEY (IdCategoria) REFERENCES Categorias(Id)
);

INSERT INTO Productos (Precio, FechaCarga, IdCategoria) 
VALUES (10, DATEADD(DAY, -3, GETDATE()), 2),
       (60, DATEADD(HOUR, -12, GETDATE()), 1),
       (0.5, DATEADD(DAY, -5, GETDATE()), 2),
       (0.5, DATEADD(DAY, -2, GETDATE()), 1),
       (15, DATEADD(MINUTE, -30, GETDATE()), 2),

CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(50) NOT NULL,
    Contraseña NVARCHAR(100) NOT NULL
);
 
