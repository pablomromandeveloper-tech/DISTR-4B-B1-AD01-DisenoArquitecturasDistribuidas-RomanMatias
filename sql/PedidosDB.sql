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
    CREATE TABLE [Cliente] (
        [IdCliente] int NOT NULL IDENTITY,
        [Nombres] nvarchar(150) NOT NULL,
        [Cedula] nvarchar(10) NOT NULL,
        [Correo] nvarchar(150) NOT NULL,
        CONSTRAINT [PK_Cliente] PRIMARY KEY ([IdCliente])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE TABLE [Pedido] (
        [IdPedido] int NOT NULL IDENTITY,
        [IdCliente] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Total] decimal(18,2) NOT NULL,
        [Estado] nvarchar(15) NOT NULL,
        CONSTRAINT [PK_Pedido] PRIMARY KEY ([IdPedido]),
        CONSTRAINT [CK_Pedido_Estado] CHECK ([Estado] IN ('Pendiente','Confirmado','Cancelado')),
        CONSTRAINT [CK_Pedido_Total] CHECK ([Total] > 0),
        CONSTRAINT [FK_Pedido_Cliente_IdCliente] FOREIGN KEY ([IdCliente]) REFERENCES [Cliente] ([IdCliente]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE TABLE [DetallePedido] (
        [IdDetalle] int NOT NULL IDENTITY,
        [IdPedido] int NOT NULL,
        [IdProducto] int NOT NULL,
        [Cantidad] int NOT NULL,
        [PrecioUnitario] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_DetallePedido] PRIMARY KEY ([IdDetalle]),
        CONSTRAINT [CK_Detalle_Cantidad] CHECK ([Cantidad] > 0),
        CONSTRAINT [CK_Detalle_Precio] CHECK ([PrecioUnitario] > 0),
        CONSTRAINT [FK_DetallePedido_Pedido_IdPedido] FOREIGN KEY ([IdPedido]) REFERENCES [Pedido] ([IdPedido]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cliente_Cedula] ON [Cliente] ([Cedula]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cliente_Correo] ON [Cliente] ([Correo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DetallePedido_IdPedido_IdProducto] ON [DetallePedido] ([IdPedido], [IdProducto]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111141359_Inicial'
)
BEGIN
    CREATE INDEX [IX_Pedido_IdCliente] ON [Pedido] ([IdCliente]);
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
