IF OBJECT_ID('XINotificationMasterOrg', 'U') IS NOT NULL DROP TABLE dbo.XINotificationMasterOrg

	
CREATE TABLE [XINotificationMasterOrg]
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
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[iCategory] INT NULL,
	[sTheme] NVARCHAR(20) NULL,
	[bIsImportant] bit NULL,
	[FKiLayoutIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiOrgID] INT NULL,
	[FKiNotificationMasterIDXIGUID] UNIQUEIDENTIFIER NULL,
	[iLeft] INT NULL,
	[iTop] INT NULL,
	[iWidth] INT NULL,
	[iHeight] INT NULL
)