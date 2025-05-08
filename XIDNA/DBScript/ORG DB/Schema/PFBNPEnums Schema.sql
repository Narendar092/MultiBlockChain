IF OBJECT_ID('PFBNPEnums', 'U') IS NOT NULL DROP TABLE dbo.PFBNPEnums

	
CREATE TABLE [PFBNPEnums]
(
	[Value] NVARCHAR(255) NULL,
	[Display] NVARCHAR(255) NULL,
	[Type] NVARCHAR(255) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)