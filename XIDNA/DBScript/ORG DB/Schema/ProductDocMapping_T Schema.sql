IF OBJECT_ID('ProductDocMapping_T', 'U') IS NOT NULL DROP TABLE dbo.ProductDocMapping_T

	
CREATE TABLE [ProductDocMapping_T]
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
	[XIiVersion] INT NOT NULL default((0)),
	[FKiQSInstanceIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiProductID] INT NULL,
	[FKiReqDocID] INT NULL,
	[bIsSelection] bit NOT NULL default((0)),
	[bIsPolicyDocument] bit NOT NULL default((0)),
	[FKiERPTaskID] bigint NULL,
	[FKiOrgID] INT NULL
)