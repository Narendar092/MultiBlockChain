IF OBJECT_ID('refPolicyRules_T', 'U') IS NOT NULL DROP TABLE dbo.refPolicyRules_T

	
CREATE TABLE [refPolicyRules_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[FKiProductID] INT NULL,
	[FKiRuleNoID] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[sCode] VARCHAR(50) NULL,
	[sMergeRef] VARCHAR(10) NULL,
	[sFunction] VARCHAR(4000) NULL,
	[refTypeID] INT NULL,
	[iLoadStage] INT NULL,
	[iSeq] INT NULL,
	[sHTML] VARCHAR(4000) NULL,
	[iVersion] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)