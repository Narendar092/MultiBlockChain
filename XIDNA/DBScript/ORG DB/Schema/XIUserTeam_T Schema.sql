IF OBJECT_ID('XIUserTeam_T', 'U') IS NOT NULL DROP TABLE dbo.XIUserTeam_T

	
CREATE TABLE [XIUserTeam_T]
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
	[iUserID] INT NULL,
	[FKiTeamID] INT NULL,
	[FKiUserIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiTeamIDXIGUID] UNIQUEIDENTIFIER NULL
)