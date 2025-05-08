IF OBJECT_ID('PaymentRequest_T', 'U') IS NOT NULL DROP TABLE dbo.PaymentRequest_T

	
CREATE TABLE [PaymentRequest_T]
(
	[ID] INT NOT NULL,
	[FKsQuoteID] VARCHAR(128) NULL,
	[sSha1hash] VARCHAR(MAX) NULL,
	[rAmount] float NULL,
	[FKiQuoteID] INT NULL,
	[sOrderID] VARCHAR(MAX) NULL,
	[sGUID] VARCHAR(MAX) NULL,
	[izXDeleted] INT NULL default((0)),
	[zXCrtdBy] NVARCHAR(128) NULL,
	[zXCrtdWhn] smalldatetime NULL,
	[zXUpdtdBy] NVARCHAR(128) NULL,
	[zXUpdtdWhn] smalldatetime NULL,
	[iStatus] INT NULL,
	[sResponseURL] VARCHAR(MAX) NULL,
	[sRequestContent] VARCHAR(MAX) NULL,
	[sSecret] VARCHAR(128) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)