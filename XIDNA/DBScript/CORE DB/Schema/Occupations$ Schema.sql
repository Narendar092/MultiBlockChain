IF OBJECT_ID('Occupations$', 'U') IS NOT NULL DROP TABLE dbo.Occupations$

	
CREATE TABLE [Occupations$]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[Occupation] NVARCHAR(255) NULL,
	[Acceptable/Decline] NVARCHAR(255) NULL
)