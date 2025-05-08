IF OBJECT_ID('EmployeeTypes_T', 'U') IS NOT NULL DROP TABLE dbo.EmployeeTypes_T

	
CREATE TABLE [EmployeeTypes_T]
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
	[EmployeeType] INT NULL,
	[Wageroll] VARCHAR(50) NULL,
	[FkiQSinstanceID] INT NULL,
	[NumberofEmployees] INT NULL,
	[FkiQSinstanceIDXIGUID] UNIQUEIDENTIFIER NULL
)