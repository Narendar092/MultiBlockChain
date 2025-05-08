IF OBJECT_ID('refExtSource_T', 'U') IS NOT NULL DROP TABLE dbo.refExtSource_T

	
CREATE TABLE [refExtSource_T]
(
	[id] INT NOT NULL,
	[sName] NVARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] NVARCHAR(250) NULL,
	[src_num] INT NOT NULL,
	[src_code] NVARCHAR(3) NULL,
	[src_text] NVARCHAR(50) NULL,
	[src_sts] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)