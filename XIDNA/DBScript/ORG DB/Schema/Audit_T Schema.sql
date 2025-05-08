IF OBJECT_ID('Audit_T', 'U') IS NOT NULL DROP TABLE dbo.Audit_T

	
CREATE TABLE [Audit_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FKiBOID] INT NULL,
	[sBOName] VARCHAR(50) NULL,
	[sData] NVARCHAR(MAX) NULL,
	[sOldData] NVARCHAR(MAX) NULL,
	[zXCrtdBy] NVARCHAR(30) NULL,
	[zXCrtdWhn] datetime NULL,
	[sType] VARCHAR(30) NULL,
	[sActivity] VARCHAR(512) NULL,
	[FKiInstanceID] INT NULL,
	[izXDeleted] INT NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[FKiPolicyVersionID] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[FkiParentBOID] INT NULL,
	[FksParentBOName] VARCHAR(50) NULL,
	[FKiParentInstanceID] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[FkiLeadID] int NULL,
	[iHandler] int NULL
)