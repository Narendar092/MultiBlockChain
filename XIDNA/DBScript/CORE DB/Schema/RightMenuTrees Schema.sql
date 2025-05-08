IF OBJECT_ID('RightMenuTrees', 'U') IS NOT NULL DROP TABLE dbo.RightMenuTrees

	
CREATE TABLE [RightMenuTrees]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[CreatedBy] INT NOT NULL default((0)),
	[CreatedTime] datetime NOT NULL default(getdate()),
	[UpdatedBy] INT NOT NULL default((0)),
	[UpdatedTime] datetime NOT NULL default(getdate()),
	[RootName] VARCHAR(50) NULL,
	[MenuAction] VARCHAR(64) NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[OrgID] INT NULL,
	[ActionType] INT NULL,
	[StatusTypeID] INT NOT NULL,
	[XiLinkID] INT NULL,
	[CreatedBySYSID] VARCHAR(512) NULL,
	[MenuID] VARCHAR(64) NULL,
	[UpdatedBySYSID] VARCHAR(512) NULL,
	[MenuController] VARCHAR(64) NULL,
	[RoleID] INT NULL,
	[Priority] INT NULL,
	[Name] VARCHAR(64) NULL,
	[ParentID] VARCHAR(32) NULL
)