IF OBJECT_ID('Notifications', 'U') IS NOT NULL DROP TABLE dbo.Notifications

	
CREATE TABLE [Notifications]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[iParentID] INT NULL,
	[sClientID] VARCHAR(256) NULL,
	[iOrganizationID] INT NULL,
	[iType] INT NULL,
	[sSubject] VARCHAR(512) NULL,
	[sMessage] VARCHAR(MAX) NULL,
	[sIcon] VARCHAR(64) NULL,
	[iImportance] INT NULL,
	[dReceivedOn] datetime NULL,
	[sOrganizationName] VARCHAR(64) NULL,
	[bIsRead] bit NULL,
	[sAttachments] VARCHAR(MAX) NULL,
	[sMailType] VARCHAR(256) NULL,
	[FKiUserID] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[iInstanceID] INT NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[FKiBODID] INT NULL,
	[FKiBOIID] INT NULL,
	[FKiQSInstanceID] INT NULL,
	[iInstanceIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiQSInstanceIDXIGUID] UNIQUEIDENTIFIER NULL
)