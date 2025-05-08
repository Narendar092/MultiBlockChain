IF OBJECT_ID('EDI_Transaction', 'U') IS NOT NULL DROP TABLE dbo.EDI_Transaction

	
CREATE TABLE [EDI_Transaction]
(
	[ID] INT NOT NULL,
	[sMergedText] VARCHAR(MAX) Not NULL,
	[iInstanceID] INT NOT NULL,
	[iStatus] INT NOT NULL default((0)),
	[FkiBOID] INT not NULL,
	[FkiTemplateID] INT not NULL,
	[FKiUserID] INT NOT NULL default((0)),
	[zXCrtdBy] VARCHAR(120) NULL,
	[zXCrtdWhn] datetime NOT NULL default(getdate()),
	[zXUpdtdBy] VARCHAR(120) NULL,
	[zXUpdtdWhn] datetime NOT NULL default(getdate()),
	[iType] INT NULL,
	[FKiProductID] INT NULL,
	[sMergedTextHeader] VARCHAR(MAX) NULL,
	[FKiAddonID] INT NULL,
	[iGeneratedStatus] INT NULL,
	[FKsPolicyNo] VARCHAR(120) NULL,
	[iTransactionType] INT NULL,
	[sDocumentName] VARCHAR(250) NULL,
	[sPolicyHolderName] VARCHAR(250) NULL,
	[sWhenGenerate] VARCHAR(250) NULL,
	[izXDeleted] INT NULL,
	[FKsClientReferenceNumber] VARCHAR(120) NULL,
	[FKiACPolicyVersionID] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)