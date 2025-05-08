IF OBJECT_ID('Complaints_T', 'U') IS NOT NULL DROP TABLE dbo.Complaints_T

	
CREATE TABLE [Complaints_T]
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
	[FKiACPolicyID] INT NULL,
	[dtReceivedDate] datetime NULL,
	[FKiHandlerID] INT NULL,
	[FKiHandlerIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiCategoryID] INT NULL,
	[sDetails] VARCHAR(512) NULL,
	[FKiClientID] INT NULL,
	[rRedressPaid] decimal(18,6) NULL,
	[dtClosedReport] datetime NULL
)