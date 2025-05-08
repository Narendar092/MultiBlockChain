IF OBJECT_ID('MailConfig_T', 'U') IS NOT NULL DROP TABLE dbo.MailConfig_T

	
CREATE TABLE [MailConfig_T]
(
	[sCode] VARCHAR(64) NULL,
	[iStatus] INT NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sSuccessMessage] VARCHAR(MAX) NULL,
	[sFailedMessage] VARCHAR(MAX) NULL,
	[bIsOnActivity] bit NOT NULL default((0)),
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)