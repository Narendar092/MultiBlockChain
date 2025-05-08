IF OBJECT_ID('EnumReconciliations_T', 'U') IS NOT NULL DROP TABLE dbo.EnumReconciliations_T

	
CREATE TABLE [EnumReconciliations_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)