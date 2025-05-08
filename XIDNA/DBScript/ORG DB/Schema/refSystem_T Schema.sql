IF OBJECT_ID('refSystem_T', 'U') IS NOT NULL DROP TABLE dbo.refSystem_T

	
CREATE TABLE [refSystem_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(50) Not NULL,
	[iStatus] bigint NOT NULL,
	[izXDeleted] bigint NOT NULL,
	[sNotes] VARCHAR(250) Not NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)