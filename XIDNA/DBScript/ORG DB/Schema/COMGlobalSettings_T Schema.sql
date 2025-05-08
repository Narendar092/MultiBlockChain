IF OBJECT_ID('COMGlobalSettings_T', 'U') IS NOT NULL DROP TABLE dbo.COMGlobalSettings_T

	
CREATE TABLE [COMGlobalSettings_T]
(
	[id] INT NOT NULL,
	[sCode] NVARCHAR(50) NULL,
	[sName] NVARCHAR(50) NULL,
	[sValue] NVARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] NVARCHAR(250) NULL,
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