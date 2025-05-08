IF OBJECT_ID('IOPaymentTypes_t', 'U') IS NOT NULL DROP TABLE dbo.IOPaymentTypes_t

	
CREATE TABLE [IOPaymentTypes_t]
(
	[ID] INT NOT NULL,
	[sName] VARCHAR(256) NULL,
	[sValue] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XICreatedBy] VARCHAR(15) NULL,
	[XICreatedWhen] datetime NULL,
	[XIUpdatedBy] VARCHAR(15) NULL,
	[XIUpdatedWhen] datetime NULL,
	[XIDeleted] INT NULL,
	[sHierarchy1] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid())
)