IF OBJECT_ID('BSRComplaints_T', 'U') IS NOT NULL DROP TABLE dbo.BSRComplaints_T

	
CREATE TABLE [BSRComplaints_T]
(
	[ID] INT NULL,
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
	[FKiClientID] bigint NULL,
	[dtReceivedDate] datetime NULL,
	[FKiHandlerID] int NULL,
	[FKiCategoryID] int NULL,
	[sDetails] varchar(256) NULL,
	[dtClosedReport] datetime NULL
)