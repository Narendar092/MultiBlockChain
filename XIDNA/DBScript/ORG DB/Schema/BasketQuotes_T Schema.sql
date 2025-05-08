IF OBJECT_ID('BasketQuotes_T', 'U') IS NOT NULL DROP TABLE dbo.BasketQuotes_T

	
CREATE TABLE [BasketQuotes_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[rAmount] float NULL,
	[FKiBasketID] INT NULL,
	[FKiQuoteID] INT NULL,
	[bIsDocumentsGenerated] bit NOT NULL DEFAULT((0)),
	[sResonForRecommend] VARCHAR(MAX) NULL,
	[bISSend] bit NOT NULL default((0))
)