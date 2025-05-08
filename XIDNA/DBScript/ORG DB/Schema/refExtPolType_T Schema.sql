IF OBJECT_ID('refExtPolType_T', 'U') IS NOT NULL DROP TABLE dbo.refExtPolType_T

	
CREATE TABLE [refExtPolType_T]
(
	[id] INT NOT NULL,
	[sName] NVARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] NVARCHAR(250) NULL,
	[ptype] NVARCHAR(3) NOT NULL,
	[ptext] NVARCHAR(30) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)