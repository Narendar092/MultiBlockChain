IF OBJECT_ID('commsAppMapping_T', 'U') IS NOT NULL DROP TABLE dbo.commsAppMapping_T

	
CREATE TABLE [commsAppMapping_T]
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
	[sSGMessageID] VARCHAR(256) NULL,
	[FKiCommID] INT NULL,
	[FKiAppID] INT NULL,
	[FKiAppIDXIGUID] UNIQUEIDENTIFIER NULL
)