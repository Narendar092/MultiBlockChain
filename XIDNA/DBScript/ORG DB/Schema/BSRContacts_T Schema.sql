IF OBJECT_ID('BSRContacts_T', 'U') IS NOT NULL DROP TABLE dbo.BSRContacts_T

	
CREATE TABLE [BSRContacts_T]
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
	[FKiClientID] INT NULL,
	[sSurName] VARCHAR(256) NULL,
	[sEmail] VARCHAR(256) NULL,
	[sContactNo] VARCHAR(256) NULL,
	[sTitle] INT NULL,
	[iPreferred] INT NULL,
	[bIsDefault] bit NULL,
	[bIsSMS] bit NULL,
	[bIsEmail] bit NULL,
	[bIsPhone] bit NULL,
	[iTitle] INT NULL,
	[FKiOrgID] INT NULL
)