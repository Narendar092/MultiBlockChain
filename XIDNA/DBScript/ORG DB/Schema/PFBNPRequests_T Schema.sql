IF OBJECT_ID('PFBNPRequests_T', 'U') IS NOT NULL DROP TABLE dbo.PFBNPRequests_T

	
CREATE TABLE [PFBNPRequests_T]
(
	[id] INT NOT NULL,
	[sRequestObject] VARCHAR(MAX) NULL,
	[sResponseObject] VARCHAR(MAX) NULL,
	[sType] VARCHAR(256) NULL,
	[zXCrtdBy] VARCHAR(128) NULL,
	[zXUpdtdBy] VARCHAR(128) NULL,
	[zXCrtdWhn] datetime NULL default(getdate()),
	[zXUpdtdWhn] datetime NULL,
	[sRequestedService] VARCHAR(128) NULL,
	[sConversation] VARCHAR(128) NULL,
	[sPdffilepath] VARCHAR(256) NULL,
	[sPartnerTransactionToken] VARCHAR(128) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)