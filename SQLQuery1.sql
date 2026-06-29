-- Nos aseguramos de que los datos entren en la base de datos de KeyVerse
USE KeyVerseDB;

-- 1. tabla de Categorías
CREATE TABLE Categorias (
    IdCategoria INT PRIMARY KEY IDENTITY(1,1),
    NombreCategoria VARCHAR(100) NOT NULL
);

-- 2. tabla de Juegos (Catálogo completo)
CREATE TABLE Juegos (
    IdJuego INT PRIMARY KEY IDENTITY(1,1),
    IdCategoria INT FOREIGN KEY REFERENCES Categorias(IdCategoria),
    Nombre VARCHAR(150) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT DEFAULT 0,
    ImagenUrl VARCHAR(250) NULL
);

-- 3. Categorías oficiales de la tienda
INSERT INTO Categorias (NombreCategoria) VALUES 
('Deportes'), 
('Acción y Aventura'), 
('Servidores VIP'), 
('Software de Redes y OSINT');

-- 4. Poblamos el catálogo con los productos reales de KeyVerse
INSERT INTO Juegos (IdCategoria, Nombre, Precio, Stock, ImagenUrl) VALUES 
(1, 'EA Sports FC 26 - Edición FC Barcelona (Incluye bono vs PSG)', 69.99, 50, '1'),
(3, 'Rango VIP - minecraft más pajitas', 15.00, 999, '2'),
(3, 'Pack de Mods Técnicos y Optimización', 5.99, 100, '3'),
(4, 'Maltego Pro - Licencia Anual de Investigación OSINT', 120.00, 10, '4'),
(4, 'Packet Tracer Labs - Ejercicios de Redes 1', 19.99, 200, '5'),
(2, 'Cyberpunk 2077 - Edición Definitiva', 39.99, 30, '6');