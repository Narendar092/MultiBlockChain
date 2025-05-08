IF OBJECT_ID('refBankDetails_T', 'U') IS NOT NULL DROP TABLE dbo.refBankDetails_T

	
CREATE TABLE [refBankDetails_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[sAlert] VARCHAR(50) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[sBankName] VARCHAR(100) NULL,
	[sBankBranch] VARCHAR(100) NULL,
	[sBankAdd1] VARCHAR(80) NULL,
	[sBankAdd2] VARCHAR(80) NULL,
	[sBankAdd3] VARCHAR(80) NULL,
	[sBankAdd4] VARCHAR(80) NULL,
	[sBankAdd5] VARCHAR(80) NULL,
	[sBankPostCode] VARCHAR(50) NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[iSortCode1] VARCHAR(9) NULL,
	[iSortCode2] VARCHAR(9) NULL,
	[iSortCode3] VARCHAR(9) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)