IF OBJECT_ID('XIUserNotifications_T', 'U') IS NOT NULL DROP TABLE dbo.XIUserNotifications_T

	
CREATE TABLE [XIUserNotifications_T]
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
	[bIsToastSent] bit NULL,
	[bIsRead] bit NULL,
	[sNotes] NVARCHAR(1024) NULL,
	[FKiNotificationID] INT NULL,
	[FKiTaskID] INT NULL,
	[iUserID] INT NULL
)