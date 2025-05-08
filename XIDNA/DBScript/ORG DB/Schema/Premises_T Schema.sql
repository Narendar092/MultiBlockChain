IF OBJECT_ID('Premises_T', 'U') IS NOT NULL DROP TABLE dbo.Premises_T

	
CREATE TABLE [Premises_T]
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
	[sBuildings] VARCHAR(50) NULL,
	[sContents] VARCHAR(50) NULL,
	[FKiQuoteID] bigint NULL,
	[FkiQSinstanceID] INT NULL,
	[RiskPostcode] NVARCHAR(10) NULL,
	[OwnTheBuilding] INT NULL,
	[RiskAddress1] VARCHAR(256) NULL,
	[RiskAddress2] VARCHAR(256) NULL,
	[RiskAddress3] VARCHAR(256) NULL,
	[NSC] INT NULL,
	[NSCPercent] INT NULL,
	[FkiQSinstanceIDXIGUID] UNIQUEIDENTIFIER NULL
)