IF OBJECT_ID('XINVSetting_T', 'U') IS NOT NULL DROP TABLE dbo.XINVSetting_T

CREATE TABLE [XINVSetting_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FKiApplicationID] INT not NULL,
	[FkiOrgID] INT not NULL,
	[sSettingName] VARCHAR(100) NULL,
	[sSettingValue] VARCHAR(100) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sEnvironment] VARCHAR(50) NULL,
	[bIsEncrypt] bit NOT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[izXDeleted] INT NULL default((0)),
	[sSecurityGUID] VARCHAR(100) NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)