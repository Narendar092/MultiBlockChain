IF OBJECT_ID('XIQSDefExcelCalcOrg_T', 'U') IS NOT NULL DROP TABLE dbo.XIQSDefExcelCalcOrg_T

	
CREATE TABLE [XIQSDefExcelCalcOrg_T]
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
	[FKiQSDefinitionID] INT NULL,
	[FKiQSDefinitionIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiOrgID] INT not NULL,
	[FKiTemplateIDXIGUID] UNIQUEIDENTIFIER NULL,
	[sExcelCalcPath] NVARCHAR(1024) NULL
)