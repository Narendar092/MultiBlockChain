IF OBJECT_ID('DistributionTeams_T', 'U') IS NOT NULL DROP TABLE dbo.DistributionTeams_T

	
CREATE TABLE [DistributionTeams_T]
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
	[iStatus] INT NULL,
	[iTeamID] INT NULL,
	[iUserID] INT NULL,
	[FKiSourceID] INT NULL,
	[rAmount] decimal(18,6) NULL
)