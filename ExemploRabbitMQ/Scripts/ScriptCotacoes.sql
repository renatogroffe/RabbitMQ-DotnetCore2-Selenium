CREATE DATABASE TestesRabbitMQ
GO

USE TestesRabbitMQ
GO

CREATE TABLE [dbo].[Cotacoes](
	[NomeMoeda] [varchar](30) NOT NULL,
	[DtUltimaCarga] [datetime] NOT NULL,
	[ValorCompra] [numeric](18, 4) NOT NULL,
	[ValorVenda] [numeric](18, 4) NULL,
	[Variacao] [varchar](10) NOT NULL,
    CONSTRAINT PK_Cotacoes PRIMARY KEY([NomeMoeda])
) ON [PRIMARY]
GO
