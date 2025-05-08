IF OBJECT_ID('Lock_T', 'U') IS NOT NULL DROP TABLE dbo.Lock_T

	
CREATE TABLE [Lock_T]
(
	[ID] INT NOT NULL,
	[FKiBOID] INT NULL,
	[FKsBOName] VARCHAR(50) NULL,
	[zXCrtdBy] NVARCHAR(30) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] NVARCHAR(30) NULL,
	[zXUpdtdWhn] datetime NULL,
	[izXDeleted] INT NULL default((0)),
	[FKiInstanceID] INT NULL,
	[FKiUserID] INT NULL,
	[bIsLock] bit NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)