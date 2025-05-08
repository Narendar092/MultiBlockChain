IF OBJECT_ID('VariantQuotation_T', 'U') IS NOT NULL DROP TABLE dbo.VariantQuotation_T

	
CREATE TABLE [VariantQuotation_T]
(
	[ID] int NOT NULL PRIMARY KEY,
	[XICreatedBy] VARCHAR(15) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedBy] VARCHAR(15) NOT NULL default(''),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[XIDeleted] int NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] uniqueidentifier NOT NULL default (NEWID()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] int NULL,
	[iType] int NULL,
	[XIiVersion] int NULL,
	[iQSCount] int NULL,
	[iQuoteCount] int NULL,
	[iReferCount] int NULL,
	[iFailedCount] int NULL,
	[dFirstQuoted] datetime NULL,
	[dLastQuoted] datetime NULL,
	[sClass] VARCHAR(50) NULL,
	[dDOB] [date] NULL,
	[sPostCode] VARCHAR(50) NULL,
	[FKiOrgID] INT NULL
)