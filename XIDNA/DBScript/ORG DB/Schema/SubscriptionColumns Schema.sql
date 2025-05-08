IF OBJECT_ID('SubscriptionColumns', 'U') IS NOT NULL DROP TABLE dbo.SubscriptionColumns

	
CREATE TABLE [SubscriptionColumns]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[OrganizationID] INT NULL,
	[SubscriptionID] VARCHAR(32) NULL,
	[FieldName] VARCHAR(32) NULL,
	[FieldValue] VARCHAR(128) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid())
)