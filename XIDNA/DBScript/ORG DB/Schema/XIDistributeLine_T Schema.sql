IF OBJECT_ID('XIDistributeLine_T', 'U') IS NOT NULL DROP TABLE dbo.XIDistributeLine_T

	
CREATE TABLE [XIDistributeLine_T]
(
	[ID] INT NOT NULL,
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
	[FKiDistributeID] INT NULL,
	[FKi1ClickID] INT NULL,
	[FKiProcessControllerID] INT NULL,
	[iOverrideStatus] INT NULL,
	[XIfOrder] float NULL,
	[FKiUserID] INT NULL default((0)),
	[FKiTeamID] INT NULL,
	[FKi1ClickIDXIGUID] UNIQUEIDENTIFIER null,
	[FKiProcessControllerIDXIGUID] UNIQUEIDENTIFIER null,
	[FKiDistributeIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiParameterID] INT NULL,
	[FKiParameterIDXIGUID] UNIQUEIDENTIFIER NULL
)
