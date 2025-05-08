IF OBJECT_ID('ReportConfig_t', 'U') IS NOT NULL DROP TABLE dbo.ReportConfig_t

	
CREATE TABLE [ReportConfig_t]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(128) NULL,
	[FKiOneclickID] INT NULL,
	[iParentID] INT NULL,
	[iPriority] INT NULL,
	[iAppendType] INT NULL,
	[iNodeType] INT NULL,
	[zXCrtdBy] NVARCHAR(128) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] NVARCHAR(128) NULL,
	[zXUpdtdWhn] datetime NULL,
	[iPivot] INT NULL,
	[sData] VARCHAR(1024) NULL,
	[iDataFormat] INT NULL,
	[sQuery] VARCHAR(1024) NULL,
	[bIsRowTotal] bit NULL,
	[bIsCalculate] bit NULL,
	[bIsHeader] bit NULL,
	[bIsColumnTotal] bit NULL,
	[bIsTotalSum] bit NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[bIsRowColour] bit NULL,
	[Scripts] VARCHAR(256) NULL,
	[sDefault] VARCHAR(50) NULL,
	[bCellValue] bit NULL,
	[bColumnValue] bit NULL,
	[TargetValue] INT NULL,
	[coColour] VARCHAR(50) NULL,
	[izXDeleted] INT NULL default((0)),
	[FKiOneclickIDXIGUID] UNIQUEIDENTIFIER NULL
)