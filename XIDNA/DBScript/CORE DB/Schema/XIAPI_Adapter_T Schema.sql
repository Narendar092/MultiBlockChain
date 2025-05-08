IF OBJECT_ID('XIAPI_Adapter_T', 'U') IS NOT NULL DROP TABLE dbo.XIAPI_Adapter_T

	
CREATE TABLE [XIAPI_Adapter_T]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[sName] VARCHAR(256) NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sAuthenticationType] VARCHAR(256) NULL,
	[sURL] VARCHAR(256) NULL,
	[sAction] VARCHAR(256) NULL,
	[sType] VARCHAR(256) NULL,
	[sFormat] VARCHAR(256) NULL,
	[sValue] VARCHAR(256) NULL,
	[sParams] VARCHAR(256) NULL,
	[sHeaders] VARCHAR(256) NULL,
	[sKey] VARCHAR(256) NULL
)