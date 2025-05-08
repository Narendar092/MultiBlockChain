IF OBJECT_ID('Riskaddress_T', 'U') IS NOT NULL DROP TABLE dbo.Riskaddress_T

	
CREATE TABLE [Riskaddress_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[FkiQSinstanceID] INT NULL,
	[RiskPostcode] VARCHAR(50) NULL,
	[Markelarea] VARCHAR(256) NULL,
	[Markelfloodreferal] VARCHAR(256) NULL,
	[Markelsubsidencereferal] VARCHAR(256) NULL,
	[Ageastheftarea] VARCHAR(256) NULL,
	[Ageassubsidenceband] VARCHAR(256) NULL,
	[Ageasfloodscore] VARCHAR(256) NULL,
	[PoolREterrorismzone] VARCHAR(256) NULL,
	[NICI] VARCHAR(256) NULL,
	[IPTexempt] VARCHAR(256) NULL,
	[FkiQSinstanceIDXIGUID] UNIQUEIDENTIFIER NULL
)