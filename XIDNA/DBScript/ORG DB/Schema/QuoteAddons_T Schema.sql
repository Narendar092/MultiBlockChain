IF OBJECT_ID('QuoteAddons_T', 'U') IS NOT NULL DROP TABLE dbo.QuoteAddons_T

	
CREATE TABLE [QuoteAddons_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(255) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(64) NULL,
	[sHierarchy] VARCHAR(64) NULL,
	[rSalePrice] float NULL,
	[rSalePriceExcludeIPT] float NULL,
	[rCostPrice] float NULL,
	[rMargin] float NULL,
	[rIPT] float NULL,
	[FKiQuoteID] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[FKiProductAddonID] INT NULL,
	[refAddon] INT NULL,
	[FKiQSInstanceID] bigint NULL,
	[FKiProductVersionID] INT NULL,
	[bIsIPT] bit NULL,
	[FKiQSInstanceIDXIGUID] UNIQUEIDENTIFIER NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid())
)