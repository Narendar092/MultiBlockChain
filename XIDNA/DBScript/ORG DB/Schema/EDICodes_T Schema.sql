IF OBJECT_ID('EDICodes_T', 'U') IS NOT NULL DROP TABLE dbo.EDICodes_T

	
CREATE TABLE [EDICodes_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FKiFieldID] INT NULL,
	[FKiProductID] INT NULL,
	[FKiSupplierID] INT NULL,
	[sValue] VARCHAR(250) NULL,
	[sCode] VARCHAR(250) NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)