IF OBJECT_ID('Subsidiaries_T', 'U') IS NOT NULL DROP TABLE dbo.Subsidiaries_T

	
CREATE TABLE [Subsidiaries_T]
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
	[SubsidiaryName] VARCHAR(256) NULL,
	[SubsidiaryERN] VARCHAR(256) NULL,
	[SubsidiaryAddress] VARCHAR(256) NULL,
	[FKiQSInstanceID] INT NULL,
	[FKiQSInstanceIDXIGUID] UNIQUEIDENTIFIER NULL
)