IF OBJECT_ID('APIQuotes_T', 'U') IS NOT NULL DROP TABLE dbo.APIQuotes_T

	
CREATE TABLE [APIQuotes_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FKiProductVersionID] INT NULL,
	[sRequestObject] VARCHAR(MAX) NULL,
	[sResponseObject] VARCHAR(MAX) NULL,
	[FKiSupplierID] INT NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[izXDeleted] INT NULL,
	[FKiQuoteID] bigint NULL,
	[sAPI] VARCHAR(64) NULL,
	[BatchID] INT NULL,
	[iOverallStatus] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[iPFSchemeID] INT NOT NULL default((0)),
	[FKQSInstanceID] INT NOT NULL default((0)),
	[sConvertQuoteRequest] NVARCHAR(MAX) NULL,
	[sConvertQuoteResponse] NVARCHAR(MAX) NULL,
	[sProduceRenewalRequest] NVARCHAR(MAX) NULL,
	[sProduceRenewalResponse] NVARCHAR(MAX) NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[FKQSInstanceIDXIGUID] UNIQUEIDENTIFIER NULL,
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[FKiCurrentVersionID] INT NULL,
	[FKiCurrentVersionIDXIGUID] UNIQUEIDENTIFIER NULL,
	[XiVersion] INT NULL
)