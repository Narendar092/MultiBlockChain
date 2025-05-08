IF OBJECT_ID('enumClaimType_T', 'U') IS NOT NULL DROP TABLE dbo.enumClaimType_T

	
CREATE TABLE [enumClaimType_T]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(250) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL default((0)),
	[sNotes] VARCHAR(250) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[sValue] VARCHAR(50) NULL,
	[sCode] VARCHAR(20) NULL,
	[FKiClassID] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)