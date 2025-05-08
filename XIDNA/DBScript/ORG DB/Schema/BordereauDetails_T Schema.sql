IF OBJECT_ID('BordereauDetails_T', 'U') IS NOT NULL DROP TABLE dbo.BordereauDetails_T

	
CREATE TABLE [BordereauDetails_T]
(
	[ID] INT NOT NULL,
	[FKiBordereauID] INT NULL,
	[sInstanceIDs] VARCHAR(1024) NULL,
	[zXCrtdBy] VARCHAR(120) NULL,
	[zXCrtdWhn] datetime NOT NULL default(getdate()),
	[zXUpdtdBy] VARCHAR(120) NULL,
	[zXUpdtdWhn] datetime NOT NULL default(getdate()),
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)