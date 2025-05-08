IF OBJECT_ID('PaymentGateWay_T', 'U') IS NOT NULL DROP TABLE dbo.PaymentGateWay_T

	
CREATE TABLE [PaymentGateWay_T]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[StatusTypeID] INT NULL,
	[sApplicationName] VARCHAR(128) NULL,
	[ResponseUrl] VARCHAR(MAX) NULL,
	[sHPPVerion] VARCHAR(32) NULL,
	[sServerKey] VARCHAR(128) NULL,
	[Mode] VARCHAR(128) NULL,
	[OrganizationID] INT NULL,
	[sSecret] VARCHAR(128) NULL,
	[sName] VARCHAR(128) Not NULL,
	[ApplicationID] INT NULL,
	[sOrganizationName] VARCHAR(128) NULL,
	[sMerchantID] VARCHAR(128) NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[ReturnUrl] VARCHAR(MAX) NULL,
	[sAccount] VARCHAR(128) NULL
)