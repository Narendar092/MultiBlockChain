IF OBJECT_ID('refTraceNode_T', 'U') IS NOT NULL DROP TABLE dbo.refTraceNode_T

	
CREATE TABLE [refTraceNode_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[FKiTraceElementID] INT NULL,
	[sDescription] VARCHAR(250) NULL,
	[sParameter1] VARCHAR(50) NULL,
	[sParameter2] VARCHAR(50) NULL,
	[sParameter3] VARCHAR(50) NULL,
	[sParameter4] VARCHAR(50) NULL,
	[sParameter5] VARCHAR(50) NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[sCode] VARCHAR(50) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)