IF OBJECT_ID('CustomerContact_T', 'U') IS NOT NULL DROP TABLE dbo.CustomerContact_T

	
CREATE TABLE [CustomerContact_T]
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
	[sTitle] INT NULL,
	[sFirstName] VARCHAR(256) NULL,
	[sSurName] VARCHAR(256) NULL,
	[sTelephone] VARCHAR(50) NULL,
	[sMobile] VARCHAR(32) NULL,
	[sEmail] VARCHAR(64) NULL,
	[FKiCustomerID] INT NULL,
	[sSalutation] INT NULL,
	[sOtherPhone] VARCHAR(50) NULL,
	[bIsEmail] bit NULL,
	[bIsPhone] bit NULL,
	[bIsSMS] bit NULL,
	[bIsDefault] bit NULL,
	[dDOB] datetime NULL,
	[iPreferredComms] INT NULL
)