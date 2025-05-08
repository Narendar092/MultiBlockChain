IF OBJECT_ID('XIUserSetting_T', 'U') IS NOT NULL DROP TABLE dbo.XIUserSetting_T

	
CREATE TABLE [XIUserSetting_T]
(
	[CreatedBy] INT NOT NULL default((0)),
	[CreatedTime] datetime NOT NULL default(getdate()),
	[UpdatedBy] INT NOT NULL default((0)),
	[UpdatedTime] datetime NOT NULL default(getdate()),
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[iRoleID] INT NULL,
	[FColour] VARCHAR(128) NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FSize] VARCHAR(128) NULL,
	[CreatedBySYSID] VARCHAR(512) NULL,
	[FKiUserID] INT NULL,
	[zXCrtdWhn] datetime NULL,
	[UpdatedBySYSID] VARCHAR(512) NULL,
	[BColour] VARCHAR(128) NULL,
	[StatusTypeID] INT NOT NULL,
	[izXDeleted] INT NULL,
	[zXUpdtdBy] VARCHAR(15) NULL
)