IF OBJECT_ID('XITeam_T', 'U') IS NOT NULL DROP TABLE dbo.XITeam_T

	
CREATE TABLE [XITeam_T]
(
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIDeleted] bit NULL,
	[XICreatedBy] VARCHAR(256) NULL,
	[XIUpdatedBy] VARCHAR(256) NULL,
	[XICreatedWhen] datetime NULL,
	[XIUpdatedWhen] datetime NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1)
)