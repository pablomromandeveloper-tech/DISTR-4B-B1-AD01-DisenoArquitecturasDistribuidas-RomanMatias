IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141328_Inicial'
)
BEGIN
    CREATE TABLE [Categoria] (
        [IdCategoria] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(300) NOT NULL,
        CONSTRAINT [PK_Categoria] PRIMARY KEY ([IdCategoria])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141328_Inicial'
)
BEGIN
    CREATE TABLE [Marca] (
        [IdMarca] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [PaisOrigen] nvarchar(80) NOT NULL,
        CONSTRAINT [PK_Marca] PRIMARY KEY ([IdMarca])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141328_Inicial'
)
BEGIN
    CREATE TABLE [Producto] (
        [IdProducto] int NOT NULL IDENTITY,
        [IdCategoria] int NOT NULL,
        [IdMarca] int NOT NULL,
        [Nombre] nvarchar(150) NOT NULL,
        [SKU] nvarchar(40) NOT NULL,
        [Precio] decimal(10,2) NOT NULL,
        [Estado] bit NOT NULL,
        CONSTRAINT [PK_Producto] PRIMARY KEY ([IdProducto]),
        CONSTRAINT [CK_Producto_Precio] CHECK ([Precio] > 0),
        CONSTRAINT [FK_Producto_Categoria_IdCategoria] FOREIGN KEY ([IdCategoria]) REFERENCES [Categoria] ([IdCategoria]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Producto_Marca_IdMarca] FOREIGN KEY ([IdMarca]) REFERENCES [Marca] ([IdMarca]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141328_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Categoria_Nombre] ON [Categoria] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141328_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Marca_Nombre] ON [Marca] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141328_Inicial'
)
BEGIN
    CREATE INDEX [IX_Producto_IdCategoria] ON [Producto] ([IdCategoria]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141328_Inicial'
)
BEGIN
    CREATE INDEX [IX_Producto_IdMarca] ON [Producto] ([IdMarca]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141328_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Producto_SKU] ON [Producto] ([SKU]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141328_Inicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261111141328_Inicial', N'10.0.15');
END;

COMMIT;
GO
