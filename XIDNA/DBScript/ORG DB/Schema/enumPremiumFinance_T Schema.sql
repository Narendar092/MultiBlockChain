IF OBJECT_ID('enumPremiumFinance_T', 'U') IS NOT NULL DROP TABLE dbo.enumPremiumFinance_T

	
CREATE TABLE [enumPremiumFinance_T]
(
	[id] NVARCHAR(255) NULL,
	[sName] NVARCHAR(255) NULL,
	[izxDeleted] INT NULL default((0)),
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)