IF OBJECT_ID('FleetBusinessPartners_T', 'U') IS NOT NULL DROP TABLE dbo.FleetBusinessPartners_T
	
CREATE TABLE [FleetBusinessPartners_T]
(
	[ID] INT NULL,
	[XICreatedBy] VARCHAR(15) NOT NULL default(''),
	[XICreatedWhen] DATETIME NOT NULL default(GETDATE()),
	[XIUpdatedBy] VARCHAR(15) NOT NULL default(''),
	[XIUpdatedWhen] DATETIME NOT NULL default(GETDATE()),
	[XIDeleted] INT NULL default(0),
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] uniqueidentifier NOT NULL default(NEWID()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[sBPFirstname] VARCHAR(60) NULL,
	[sBPSurname] VARCHAR(256) NULL,
	[iBPTitle] INT NULL,
	[FKiQSInstanceIDXIGUID] uniqueidentifier NULL
)