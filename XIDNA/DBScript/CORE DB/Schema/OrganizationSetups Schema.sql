IF OBJECT_ID('OrganizationSetups', 'U') IS NOT NULL DROP TABLE dbo.OrganizationSetups

	
CREATE TABLE [OrganizationSetups]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[Columns] VARCHAR(MAX) NULL,
	[Name] VARCHAR(50) NULL,
	[StatusTypeID] INT NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1)
)