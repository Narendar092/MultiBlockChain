IF OBJECT_ID('BSRClaims_T', 'U') IS NOT NULL DROP TABLE dbo.BSRClaims_T

	
CREATE TABLE [BSRClaims_T]
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
	[FKiClientID] bigint NULL,
	[dDate] date NULL,
	[iClaimType] int NULL,
	[iClaimSettled] int NULL,
	[iClaimStatus] int NULL
)