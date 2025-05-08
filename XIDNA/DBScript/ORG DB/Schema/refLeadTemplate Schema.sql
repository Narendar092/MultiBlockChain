IF OBJECT_ID('refLeadTemplate', 'U') IS NOT NULL DROP TABLE dbo.refLeadTemplate

	
CREATE TABLE [refLeadTemplate]
(
	[id] INT NOT NULL,
	[sName] NVARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] NVARCHAR(250) NULL,
	[zXCrtdBy] NVARCHAR(15) NULL,
	[zXCrtdWhn] smalldatetime NULL,
	[zXUpdtdBy] NVARCHAR(15) NULL,
	[zXUpdtdWhn] smalldatetime NULL,
	[iType] INT NULL,
	[FKiLeadTemplateID] INT NULL,
	[refTypeID] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)