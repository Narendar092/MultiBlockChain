IF OBJECT_ID('Convictions_T', 'U') IS NOT NULL DROP TABLE dbo.Convictions_T

	
CREATE TABLE [Convictions_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL default((0)),
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[dConvictionDate] date NULL,
	[iDriver] INT NULL,
	[iPenaltyPoints] INT NULL,
	[rFine] float NULL,
	[FkiQSinstanceID] INT NULL,
	[sConvictionCode] VARCHAR(256) NULL,
	[FkiDriverID] INT NULL,
	[iDrivingBan] INT NULL,
	[FkiQSinstanceIDXIGUID] VARCHAR(256) NULL,
	[sMonthsDisqualified] VARCHAR(50) NULL,
	[iDisqualifiedfromdriving] int NULL,
	[iDisqualificationPeriod] int NULL,
	[sDisqualificationdays] varchar(256) NULL,
	[sDriverName] varchar(256) NULL,
	[iLengthofBan] int NULL
)