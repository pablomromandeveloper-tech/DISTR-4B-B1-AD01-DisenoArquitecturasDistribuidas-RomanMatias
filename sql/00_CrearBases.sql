-- Ejecutar en SQL Server con una cuenta de desarrollo autorizada.
USE master;
GO
IF DB_ID(N'CatalogoDB') IS NULL CREATE DATABASE CatalogoDB;
GO
IF DB_ID(N'InventarioDB') IS NULL CREATE DATABASE InventarioDB;
GO
IF DB_ID(N'PedidosDB') IS NULL CREATE DATABASE PedidosDB;
GO
