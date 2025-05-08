IF OBJECT_ID('ACPFCancel_T', 'U') IS NOT NULL DROP TABLE dbo.ACPFCancel_T

	
CREATE TABLE [ACPFCancel_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[sAlert] VARCHAR(50) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[rFinanceBalance] float NULL,
	[rCancelAmount] float NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[FKiPFSchemeID] INT NULL,
	[iEDIAction] INT NULL,
	[FKiACPolicyID] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)