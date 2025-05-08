IF OBJECT_ID('Introducer_T', 'U') IS NOT NULL DROP TABLE dbo.Introducer_T

	
CREATE TABLE [Introducer_T]
(
	[ID] INT NULL,
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
	[fRenewalPercent] decimal(18,6) NULL,
	[fNewBusinessPercent] decimal(18,6) NULL,
	[fRenewalFixed] decimal(18,6) NULL,
	[fNewBusinessFixed] decimal(18,6) NULL
)