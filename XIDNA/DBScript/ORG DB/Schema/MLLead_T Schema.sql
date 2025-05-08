IF OBJECT_ID('MLLead_T', 'U') IS NOT NULL DROP TABLE dbo.MLLead_T

	
CREATE TABLE [MLLead_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[iStatus] INT NULL,
	[FKiSourceID] INT NULL,
	[FKiClassID] INT NULL,
	[iOrigin] INT NULL,
	[dDOB] date NULL,
	[zXCrtdWhn] date NULL,
	[rBestQuote] float NULL,
	[percentage] INT NULL,
	[Score] float NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)