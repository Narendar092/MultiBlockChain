IF OBJECT_ID('XIActorMapping_T', 'U') IS NOT NULL DROP TABLE dbo.XIActorMapping_T

	
CREATE TABLE [XIActorMapping_T]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[iInstanceID] INT NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[zXCrtdWhn] datetime NULL,
	[FKiUserID] INT NULL,
	[izXDeleted] INT NULL,
	[zXUpdtdBy] VARCHAR(32) NULL,
	[zXCrtdBy] VARCHAR(32) NULL,
	[FKiActorID] INT NULL,
	[zXUpdtdWhn] datetime NULL
)