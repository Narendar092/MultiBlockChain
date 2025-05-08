IF OBJECT_ID('Distributions_T', 'U') IS NOT NULL DROP TABLE dbo.Distributions_T

	
CREATE TABLE [Distributions_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[FKiLeadID] INT NULL,
	[iDistributionTeamID] INT NULL,
	[iStatus] INT NULL
)