IF OBJECT_ID('TransferTransaction', 'U') IS NOT NULL DROP TABLE dbo.TransferTransaction

	
CREATE TABLE [TransferTransaction]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[iStatus] INT NULL,
	[iType] INT NULL,
	[izXDeleted] INT NULL,
	[FKiLeadID] INT NULL,
	[FKi1ClickID] INT NULL,
	[zXCrtdBy] VARCHAR(250) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(250) NULL,
	[zXUpdtdWhn] datetime NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sContent] NVARCHAR(MAX) NULL,
	[sReceiver] VARCHAR(50) NULL,
	[sEmail] NVARCHAR(100) NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)