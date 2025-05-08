INSERT INTO [XICodeLog_T] ([ID],
 [sName],
 [sParentID],
 [sProcessID],
 [sClass],
 [sMethod],
 [sCode],
 [iStatus],
 [sMessage],
 [sQuery],
 [sQueryParams],
 [iLapsedTime],
 [iAlert],
 [sParams],
 [XIDeleted],
 [XICreatedBy],
 [XIUpdatedBy],
 [XICreatedWhen],
 [XIUpdatedWhen],
 [XIGUID]) Values(N'131',
 NULL,
 N'24130732-9e',
 N'3a5f1ee1-5a',
 N'XIIBO',
 N'Update_TODB',
 NULL,
 N'0',
 NULL,
 N'INSERT INTO XILayout_T(OrganizationID,FKiApplicationID,LayoutName,LayoutType,LayoutCode,XiParameterID,LayoutLevel,Authentication,iThemeID,bUseParentGUID,StatusTypeID,CreatedBy,CreatedTime,CreatedBySYSID,UpdatedBy,UpdatedTime,UpdatedBySYSID,sSiloAccess,bIsTaskBar,sTaskBarPosition,bAddToParentTaskbar,XIDeleted,XICreatedBy,XICreatedWhen,XIUpdatedBy,XIUpdatedWhen) output INSERTED.ID VALUES(''0'',''1'',''ObjectTabLayoutTemplate'',''Template'',''<div class="row">
<div class="col-md-3" id="1"></div>
<div class="col-md-9">
 <div class="row">
           <div class="col-md-12" id="2"></div>
<div class="col-md-12" id="3"></div>
       </div>
</div>
</div>'',''1'',''OrganisationLevel'',''Authenticated'',''0'',''True'',''10'',''0'',''2022-03-21 20:22:50.000'',''fe80::cc5d:9386:d830:15ca%17'',''0'',''2022-03-21 20:22:50.000'',''fe80::cc5d:9386:d830:15ca%17'','''',''False'','''',''False'',''0'',''config configu'',''2022-03-21 20:22:50.000'',''config configu'',''2022-03-21 20:22:50.000'')',
 NULL,
 N'0.015000',
 N'0',
 NULL,
 N'0',
 N'',
 N'',
 N'2022-03-21 20:22:57.000',
 N'2022-03-21 20:22:57.000',
 N'925d152b-a964-4988-a9da-5849d47e13fc')
