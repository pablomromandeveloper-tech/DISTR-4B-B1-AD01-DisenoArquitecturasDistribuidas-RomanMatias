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
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE TABLE [Bodega] (
        [IdBodega] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Ciudad] nvarchar(80) NOT NULL,
        [Direccion] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_Bodega] PRIMARY KEY ([IdBodega])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE TABLE [Existencia] (
        [IdExistencia] int NOT NULL IDENTITY,
        [IdProducto] int NOT NULL,
        [IdBodega] int NOT NULL,
        [Stock] int NOT NULL,
        CONSTRAINT [PK_Existencia] PRIMARY KEY ([IdExistencia]),
        CONSTRAINT [CK_Existencia_Stock] CHECK ([Stock] >= 0),
        CONSTRAINT [FK_Existencia_Bodega_IdBodega] FOREIGN KEY ([IdBodega]) REFERENCES [Bodega] ([IdBodega]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE TABLE [MovimientoInventario] (
        [IdMovimiento] int NOT NULL IDENTITY,
        [IdProducto] int NOT NULL,
        [IdBodega] int NOT NULL,
        [Tipo] nvarchar(10) NOT NULL,
        [Cantidad] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        CONSTRAINT [PK_MovimientoInventario] PRIMARY KEY ([IdMovimiento]),
        CONSTRAINT [CK_Movimiento_Cantidad] CHECK ([Cantidad] > 0),
        CONSTRAINT [CK_Movimiento_Tipo] CHECK ([Tipo] IN ('Entrada','Salida')),
        CONSTRAINT [FK_MovimientoInventario_Bodega_IdBodega] FOREIGN KEY ([IdBodega]) REFERENCES [Bodega] ([IdBodega]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE TABLE [ReservaStock] (
        [IdReserva] int NOT NULL IDENTITY,
        [IdProducto] int NOT NULL,
        [IdBodega] int NOT NULL,
        [Cantidad] int NOT NULL,
        [Estado] nvarchar(15) NOT NULL,
        CONSTRAINT [PK_ReservaStock] PRIMARY KEY ([IdReserva]),
        CONSTRAINT [CK_Reserva_Cantidad] CHECK ([Cantidad] > 0),
        CONSTRAINT [CK_Reserva_Estado] CHECK ([Estado] IN ('Activa','Cancelada','Consumida')),
        CONSTRAINT [FK_ReservaStock_Bodega_IdBodega] FOREIGN KEY ([IdBodega]) REFERENCES [Bodega] ([IdBodega]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Bodega_Nombre] ON [Bodega] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE INDEX [IX_Existencia_IdBodega] ON [Existencia] ([IdBodega]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Existencia_IdProducto_IdBodega] ON [Existencia] ([IdProducto], [IdBodega]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE INDEX [IX_MovimientoInventario_IdBodega] ON [MovimientoInventario] ([IdBodega]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE INDEX [IX_ReservaStock_IdBodega] ON [ReservaStock] ([IdBodega]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE INDEX [IX_ReservaStock_IdProducto_IdBodega_Estado] ON [ReservaStock] ([IdProducto], [IdBodega], [Estado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261111141359_Inicial', N'10.0.15');
END;

COMMIT;
GO
