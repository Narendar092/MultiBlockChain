IF OBJECT_ID('MergeEmailContent_T', 'U') IS NOT NULL DROP TABLE dbo.MergeEmailContent_T

	
CREATE TABLE [MergeEmailContent_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sContent] VARCHAR(MAX) NULL,
	[sEmail] VARCHAR(64) NULL,
	[sSubject] VARCHAR(128) NULL,
	[FKiOrgID] INT NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[iParentID] INT NULL,
	[bIsHavingAttachments] bit NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[sNotes] VARCHAR(MAX) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[FKizXDoc] VARCHAR(250) NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)