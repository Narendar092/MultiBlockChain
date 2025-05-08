IF OBJECT_ID('XIDocumentTree_T', 'U') IS NOT NULL DROP TABLE dbo.XIDocumentTree_T

	
CREATE TABLE [XIDocumentTree_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FKiProjectID] INT NULL,
	[FKiBuildingID] INT NULL,
	[FKiSubPortalID] INT NULL,
	[FKiSectionID] INT NULL,
	[sName] NVARCHAR(MAX) NULL,
	[sCode] VARCHAR(32) NULL,
	[sPath] VARCHAR(1024) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[sTags] NVARCHAR(MAX) NULL,
	[sDocID] VARCHAR(64) NULL,
	[sPageNo] INT NULL,
	[sParentID] VARCHAR(512) NULL,
	[sType] VARCHAR(128) NULL,
	[iBuildingID] INT NULL,
	[rOrder] float NULL,
	[sVersion] VARCHAR(32) NULL,
	[sFolderName] VARCHAR(128) NULL,
	[iVersionBatchID] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)