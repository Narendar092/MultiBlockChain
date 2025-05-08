IF OBJECT_ID('Letters_T', 'U') IS NOT NULL DROP TABLE dbo.Letters_T

	
CREATE TABLE [Letters_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sMergedBodyContent] VARCHAR(MAX) NULL,
	[sMergedAttachmentContent] VARCHAR(MAX) NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[FKiPolicyID] INT NULL,
	[FKiProductID] INT NULL,
	[iSentStatus] INT NULL,
	[izXDeleted] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[FKiDocID] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)