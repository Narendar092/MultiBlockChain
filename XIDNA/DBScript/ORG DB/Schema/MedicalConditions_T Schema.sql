IF OBJECT_ID('MedicalConditions_T', 'U') IS NOT NULL DROP TABLE dbo.MedicalConditions_T

	
CREATE TABLE [MedicalConditions_T]
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
	[dDiagnosed] date NULL,
	[sMedicalCondition] INT NULL,
	[bDVLAInformed] bit NULL,
	[bRegistrationDisabled] bit NULL,
	[bDisabledBadgeHolder] bit NULL,
	[FkiDriverID] INT NULL,
	[FkiQSinstanceIDXIGUID] VARCHAR(256) NULL,
	[sDriverName] varchar(256) NULL,
	[iMedicationDetails] int NULL,
	[iAnyDrivingRestrictionsimposed] int NULL,
	[dLicenceExpiryDate] date NULL 
)