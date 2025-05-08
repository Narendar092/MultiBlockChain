IF OBJECT_ID('IOClaim_T', 'U') IS NOT NULL DROP TABLE dbo.IOClaim_T

	
CREATE TABLE [IOClaim_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[izXDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[FKiACPolicyID] bigint NULL,
	[FKiClientID] bigint NULL,
	[sClaimReference] VARCHAR(64) NULL,
	[dCoverDate] datetime NULL,
	[dClaimDate] datetime NULL,
	[iHandler] INT NULL,
	[rEstimatedAmount] float NULL,
	[sCover] INT NULL,
	[sDetails] VARCHAR(64) NULL,
	[dNotifiedDate] datetime NULL,
	[dClosedDate] datetime NULL,
	[FKiACPolicyVersionID] int NULL
)