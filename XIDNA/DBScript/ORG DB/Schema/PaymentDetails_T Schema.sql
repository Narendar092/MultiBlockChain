IF OBJECT_ID('PaymentDetails_T', 'U') IS NOT NULL DROP TABLE dbo.PaymentDetails_T

	
CREATE TABLE [PaymentDetails_T]
(
	[ID] INT NOT NULL,
	[sAuthCode] VARCHAR(20) NULL,
	[sPasRef] VARCHAR(25) NULL,
	[sSha1hash] VARCHAR(MAX) NULL,
	[sGUID] VARCHAR(MAX) NULL,
	[FKiQuoteID] INT NULL,
	[bPolicyStatus] bit NULL,
	[rAmount] float NULL,
	[FKsQuoteID] VARCHAR(128) NULL,
	[iNoOfAttempts] INT NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL default((0)),
	[zXCrtdBy] NVARCHAR(128) NULL,
	[zXCrtdWhn] smalldatetime NULL,
	[zXUpdtdBy] NVARCHAR(128) NULL,
	[zXUpdtdWhn] smalldatetime NULL,
	[FKiPaymentRequestID] INT NULL,
	[sSRD] VARCHAR(128) NULL,
	[sResponseContent] VARCHAR(MAX) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)