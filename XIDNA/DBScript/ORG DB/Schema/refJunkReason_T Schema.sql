IF OBJECT_ID('refJunkReason_T', 'U') IS NOT NULL DROP TABLE dbo.refJunkReason_T

	
CREATE TABLE [refJunkReason_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[iType] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)