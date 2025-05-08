IF OBJECT_ID('OrganizationClasses', 'U') IS NOT NULL DROP TABLE dbo.OrganizationClasses

	
CREATE TABLE [OrganizationClasses]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[OrganizationID] INT NULL,
	[ClassID] INT NULL,
	[Class] VARCHAR(50) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)