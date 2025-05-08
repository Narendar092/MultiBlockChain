IF OBJECT_ID('Reports_t', 'U') IS NOT NULL DROP TABLE dbo.Reports_t

	
CREATE TABLE [Reports_t]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(MAX) NULL,
	[zXCrtdBy] NVARCHAR(128) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] NVARCHAR(128) NULL,
	[zXUpdtdWhn] datetime NULL,
	[sDecription] VARCHAR(512) NULL,
	[sHeader] VARCHAR(512) NULL,
	[FKiReportConfigID] INT NULL,
	[IsSaveToDB] bit NULL,
	[sQuery] VARCHAR(MAX) NULL,
	[sTableName] VARCHAR(100) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sColour] VARCHAR(16) NULL,
	[RowColour] VARCHAR(128) NULL,
	[XiLinkID] INT NULL,
	[bCollapse] bit NULL,
	[izXDeleted] INT NULL default((0)),
	[bIsSubTotal] bit NULL default((0)),
	[sSubTotalHeaders] VARCHAR(512) NULL,
	[iSplitFields] INT NOT NULL default((0)),
	[bAction] bit NOT NULL default((0)),
	[XilinkIDXIGUID] UNIQUEIDENTIFIER NULL,
	[bIsRowClick] bit NOT NULL default((0)),
	[sCellClickList] VARCHAR(256) NULL,
	[CellXilinkIDXIGUID] UNIQUEIDENTIFIER NULL,
	[sRowClickList] VARCHAR(256) NULL,
	[RowXilinkIDXIGUID] UNIQUEIDENTIFIER NULL,
	[bIsExhaustTotal] bit NOT NULL default((0))
)