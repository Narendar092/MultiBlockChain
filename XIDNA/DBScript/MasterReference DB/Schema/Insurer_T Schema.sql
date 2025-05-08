IF OBJECT_ID('Insurer_T', 'U') IS NOT NULL DROP TABLE dbo.Insurer_T

	
CREATE TABLE [Insurer_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[XICreatedBy] VARCHAR(15) NULL,
	[XICreatedWhen] datetime NULL,
	[XIUpdatedBy] VARCHAR(15) NULL,
	[XIUpdatedWhen] datetime NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(32) NULL,
	[iType] INT NULL,
	[iStatus] INT NULL,
	[sAddressLine1] VARCHAR(256) NULL,
	[sAgencynumber] VARCHAR(256) NULL,
	[sPostCode] VARCHAR(256) NULL,
	[sAddressLine2] VARCHAR(256) NULL,
	[sAddressLine3] VARCHAR(256) NULL,
	[sAddressLine4] VARCHAR(256) NULL,
	[sShotName] VARCHAR(256) NULL,
	[bIsFleet] bit NOT NULL DEFAULT((0))
)