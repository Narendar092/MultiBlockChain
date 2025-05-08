IF OBJECT_ID('XINVSetting_T', 'U') IS NOT NULL DROP TABLE dbo.XINVSetting_T

	
CREATE TABLE [XINVSetting_T]
(
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[izXDeleted] INT NULL default((0)),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[FkiOrgID] INT not NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sEnvironment] VARCHAR(50) NULL,
	[sSettingName] VARCHAR(100) NULL,
	[FKiApplicationID] INT not NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[sSettingValue] VARCHAR(100) NULL,
	[bIsEncrypt] bit NOT NULL
)