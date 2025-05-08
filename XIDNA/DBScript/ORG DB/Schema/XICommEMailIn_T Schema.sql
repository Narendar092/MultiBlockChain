IF OBJECT_ID('XICommEMailIn_T', 'U') IS NULL --DROP TABLE dbo.XICommEMailIn_T

	
CREATE TABLE [XICommEMailIn_T]
(
	[ID] INT NOT NULL,
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER NULL,
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[iComType] INT NULL,
	[iDirection] INT NULL,
	[sOrigin] VARCHAR(32) NULL,
	[sFrom] VARCHAR(128) NULL,
	[sTo] VARCHAR(128) NULL,
	[sHeader] VARCHAR(512) NULL,
	[sContent] VARCHAR(MAX) NULL,
	[sCC] VARCHAR(1024) NULL,
	[sBCC] VARCHAR(1024) NULL,
	[FKizXDoc] VARCHAR(512) NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[sExtUID] VARCHAR(256) NULL,
	[sAccountOrigin] VARCHAR(256) NULL,
	[sContentPlain] VARCHAR(MAX) NULL,
	[FKiOrgID] int NULL,
	[iImportType] int NOT NULL DEFAULT((0))
)