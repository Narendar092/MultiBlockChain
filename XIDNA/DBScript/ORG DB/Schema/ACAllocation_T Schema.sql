IF OBJECT_ID('ACAllocation_T', 'U') IS NOT NULL DROP TABLE dbo.ACAllocation_T

	
CREATE TABLE [ACAllocation_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[sAlert] VARCHAR(50) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[FKiACTransactionFromID] INT NULL,
	[FKiACTransactionToID] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[rValue] float NULL,
	[FKiEnterpriseID] INT NULL,
	[FKiJEID] INT NULL,
	[refAccountCategory] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)