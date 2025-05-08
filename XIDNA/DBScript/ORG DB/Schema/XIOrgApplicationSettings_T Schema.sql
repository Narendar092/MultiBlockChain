IF OBJECT_ID('XIOrgApplicationSettings_T', 'U') IS NOT NULL DROP TABLE dbo.XIOrgApplicationSettings_T

	
CREATE TABLE [XIOrgApplicationSettings_T]
(
	[sValue] VARCHAR(256) NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[izXDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] VARCHAR(128) NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)