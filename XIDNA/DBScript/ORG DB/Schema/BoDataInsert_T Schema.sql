IF OBJECT_ID('BoDataInsert_T', 'U') IS NOT NULL DROP TABLE dbo.BoDataInsert_T

	
CREATE TABLE [BoDataInsert_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FKiBOID] INT NULL,
	[sTextArea] VARCHAR(MAX) NULL,
	[sInsertionFields] VARCHAR(MAX) NULL,
	[izXDeleted] INT NULL,
	[zXCrtdBy] VARCHAR(16) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(16) NULL,
	[zXUpdtdWhn] datetime NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)