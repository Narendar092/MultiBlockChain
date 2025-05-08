IF OBJECT_ID('PolicyFinancial_T', 'U') IS NOT NULL DROP TABLE dbo.PolicyFinancial_T

	
CREATE TABLE [PolicyFinancial_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[iRefFinancialTypeID] INT NULL,
	[FKiPolicyID] INT NULL,
	[sRef] VARCHAR(16) NULL,
	[rSignedAmount] decimal(18,2) NULL,
	[rIPT] decimal(18,2) NULL,
	[iRefFinancialTypeIDXIGUID] UNIQUEIDENTIFIER NULL,
	[dDateoftransaction] date NULL,
	[iWho] int NULL
)