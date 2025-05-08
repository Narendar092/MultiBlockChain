IF OBJECT_ID('FleetClaims_T', 'U') IS NOT NULL DROP TABLE dbo.FleetClaims_T

	
CREATE TABLE [FleetClaims_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[XICreatedBy] VARCHAR(15) NOT NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) NOT NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] uniqueidentifier NOT NULL default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[dDateofLoss] datetime NULL,
	[iTotalAmountPaid] INT NULL,
	[iTotalAmountReserved] INT NULL,
	[FKiQSInstanceIDXIGUID] uniqueidentifier NULL,
	[iClaimCause] INT NULL
)