IF OBJECT_ID('SampleReport_T', 'U') IS NOT NULL DROP TABLE dbo.SampleReport_T

	
CREATE TABLE [SampleReport_T]
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
	[dtStartDate] datetime NULL,
	[dtEndDate] datetime NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL default((0)),
	[iType] INT NULL,
	[FKiCategoryID] INT NULL,
	[iCategory] INT NULL
)