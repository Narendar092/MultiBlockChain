IF OBJECT_ID('refConviction_T', 'U') IS NOT NULL DROP TABLE dbo.refConviction_T

	
CREATE TABLE [refConviction_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sCode] NVARCHAR(255) NULL,
	[sName] NVARCHAR(255) NULL,
	[FKiDocumentID] INT NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[sAlert] VARCHAR(50) NULL,
	[sLogo] VARCHAR(50) NULL,
	[zPoints] INT NULL,
	[iType] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[sValue] VARCHAR(50) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)