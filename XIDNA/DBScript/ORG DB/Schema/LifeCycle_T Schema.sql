IF OBJECT_ID('LifeCycle_T', 'U') IS NOT NULL DROP TABLE dbo.LifeCycle_T

	
CREATE TABLE [LifeCycle_T]
(
	[ID] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[FKiLeadID] INT NULL,
	[FKiQSIID] INT NULL,
	[iFromID] INT NULL,
	[iToID] INT NULL,
	[sFrom] VARCHAR(50) NULL,
	[sTo] VARCHAR(50) NULL,
	[refLifeCycleID] INT NULL,
	[dtFrom] datetime NULL,
	[dtTo] datetime NULL,
	[sCode] VARCHAR(50) NULL,
	[izXDeleted] INT NULL,
	[zXCrtdBy] VARCHAR(120) NULL,
	[zXCrtdWhn] datetime NOT NULL default(getdate()),
	[zXUpdtdBy] VARCHAR(120) NULL,
	[zXUpdtdWhn] datetime NOT NULL default(getdate()),
	[FKiQSDefinitionID] INT NULL default((0)),
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[FKiQSIIDXIGUID] UNIQUEIDENTIFIER NULL,
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[FKiOrgID] INT NULL
)