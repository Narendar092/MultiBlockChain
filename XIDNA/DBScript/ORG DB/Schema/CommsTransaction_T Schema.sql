IF OBJECT_ID('CommsTransaction_T', 'U') IS  NULL --DROP TABLE dbo.CommsTransaction_T

	
CREATE TABLE [CommsTransaction_T]
(
	[ID] INT NOT NULL,
	[FKiCommInstanceID] INT NULL default((0)),
	[iType] INT NOT NULL,
	[CreateDate] datetime NULL default(getdate()),
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
	[sClickedUrl] VARCHAR(1024) NULL,
	[FKiCampaignID] INT NULL,
	[sSendGridReference] VARCHAR(256) NULL,
	[dtEvent] datetime NULL,
	[sEvent] VARCHAR(32) NULL,
	[sEmail] VARCHAR(512) NULL,
	[sCategory] VARCHAR(512) NULL,
	[sResponse] VARCHAR(512) NULL,
	[sAttempt] VARCHAR(64) NULL,
	[sTimeStamp] VARCHAR(64) NULL,
	[sStatus] VARCHAR(64) NULL,
	[sReason] VARCHAR(1024) NULL,
	[sType] VARCHAR(256) NULL,
	[sUserAgent] VARCHAR(1024) NULL,
	[sIP] VARCHAR(256) NULL,
	[sg_event_id] VARCHAR(256) NULL,
	[sg_message_id] VARCHAR(1024) NULL
)