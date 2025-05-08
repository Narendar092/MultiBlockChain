IF OBJECT_ID('enumSecurityDevice_T', 'U') IS NOT NULL DROP TABLE dbo.enumSecurityDevice_T

	
CREATE TABLE [enumSecurityDevice_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(100) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[sValue] VARCHAR(50) NULL,
	[FKiImportID] INT NULL,
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