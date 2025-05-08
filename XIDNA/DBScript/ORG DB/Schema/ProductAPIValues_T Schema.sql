IF OBJECT_ID('ProductAPIValues_T', 'U') IS NOT NULL DROP TABLE dbo.ProductAPIValues_T

	
CREATE TABLE [ProductAPIValues_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(256) NULL,
	[sType] VARCHAR(256) NULL,
	[FKiTemplateID] INT NULL,
	[sCode] VARCHAR(50) NULL,
	[FKiProductID] INT NULL,
	[izXDeleted] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[XIGUID] UNIQUEIDENTIFIER NULL
)