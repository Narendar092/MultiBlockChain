IF OBJECT_ID('EndorsementDocuments_T', 'U') IS NOT NULL DROP TABLE dbo.EndorsementDocuments_T

	
CREATE TABLE [EndorsementDocuments_T]
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
	[sFileName] VARCHAR(264) NULL,
	[sFullPath] VARCHAR(264) NULL,
	[FKiQuoteID] INT NULL,
	[FKiTermID] bigint NULL,
	[FKiQSInstanceID] int NULL,
	[FKiQSInstanceIDXIGUID] uniqueidentifier NULL
)