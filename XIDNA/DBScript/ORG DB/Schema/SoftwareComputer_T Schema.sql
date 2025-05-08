IF OBJECT_ID('SoftwareComputer_T', 'U') IS NOT NULL DROP TABLE dbo.SoftwareComputer_T

	
CREATE TABLE [SoftwareComputer_T]
(
	[sName] VARCHAR(256) NULL,
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[ID] INT NULL
)