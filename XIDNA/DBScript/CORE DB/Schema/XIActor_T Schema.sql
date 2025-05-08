IF OBJECT_ID('XIActor_T', 'U') IS NOT NULL DROP TABLE dbo.XIActor_T

	
CREATE TABLE [XIActor_T]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[zXCrtdBy] VARCHAR(32) NULL,
	[sName] VARCHAR(50) NULL,
	[zXUpdtdWhn] datetime NULL,
	[sWhereField] VARCHAR(50) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(32) NULL,
	[izXDeleted] INT NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1)
)