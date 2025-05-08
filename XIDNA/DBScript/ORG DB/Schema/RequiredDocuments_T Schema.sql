IF OBJECT_ID('RequiredDocuments_T', 'U') IS NOT NULL DROP TABLE dbo.RequiredDocuments_T

	
CREATE TABLE [RequiredDocuments_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[XICreatedBy] VARCHAR(15) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedBy] VARCHAR(15) NOT NULL default(''),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[XIDeleted] INT NULL default((0)),
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[iOrder] float NULL,
	[FKiProductID] INT NULL,
	[bIsPolicyDocument] bit NOT NULL default((0)),
	[iDueInDays] bigint NULL,
	[FKiOrgID] int NULL
)