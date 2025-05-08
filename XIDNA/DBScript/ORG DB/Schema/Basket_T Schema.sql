IF OBJECT_ID('Basket_T', 'U') IS NOT NULL DROP TABLE dbo.Basket_T

	
CREATE TABLE [Basket_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(128) NULL,
	[sDescription] VARCHAR(128) NULL,
	[sCode] VARCHAR(128) NULL,
	[iStatus] INT NOT NULL default((0)),
	[iType] INT NOT NULL default((0)),
	[FKiCustomerID] INT NULL,
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[iBasketID] INT NULL,
	[sDemands] VARCHAR(MAX) NULL,
	[bISSend] bit NOT NULL default((0))
)