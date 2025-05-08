IF OBJECT_ID('IODemo_T', 'U') IS NOT NULL DROP TABLE dbo.IODemo_T

	
CREATE TABLE [IODemo_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[zXCrtdBy] VARCHAR(15) Not NULL,
	[zXCrtdWhn] datetime NOT NULL,
	[zXUpdtdBy] VARCHAR(15) Not NULL,
	[zXUpdtdWhn] datetime NOT NULL,
	[izXDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)