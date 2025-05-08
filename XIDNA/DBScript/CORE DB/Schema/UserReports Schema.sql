IF OBJECT_ID('UserReports', 'U') IS NOT NULL DROP TABLE dbo.UserReports

	
CREATE TABLE [UserReports]
(
	[ReportID] INT NOT NULL default((0)),
	[FKiApplicationID] INT NOT NULL default((0)),
	[OrganizationID] INT NOT NULL default((0)),
	[RoleID] INT NOT NULL default((0)),
	[Rank] INT NOT NULL default((0)),
	[XIDeleted] INT NOT NULL default((0)),
	[Icon] VARCHAR(256) NULL default(''),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[Target] INT NOT NULL default((0)),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[Location] INT NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[DisplayAs] INT NOT NULL default(''),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[StatusTypeID] tinyint NOT NULL default((0)),
	[CreatedByID] INT NOT NULL default((0)),
	[CreatedByName] VARCHAR(32) NOT NULL default(''),
	[CreatedBySYSID] VARCHAR(32) NOT NULL default(''),
	[CreatedTime] datetime NOT NULL default('1900-01-01'),
	[ModifiedByID] INT NOT NULL default((0)),
	[ModifiedByName] VARCHAR(32) NOT NULL default(''),
	[ModifiedTime] datetime NOT NULL default('1900-01-01'),
	[TypeID] INT NULL,
	[TargetTemplateID] INT NULL,
	[BO] INT NULL,
	[ClassID] INT NULL,
	[MenuID] INT NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[TargetResultType] VARCHAR(32) NULL
)