IF OBJECT_ID('PFInsurerCodes', 'U') IS NOT NULL DROP TABLE dbo.PFInsurerCodes

	
CREATE TABLE [PFInsurerCodes]
(
	[sSupplier] NVARCHAR(255) NULL,
	[sCode] float NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)