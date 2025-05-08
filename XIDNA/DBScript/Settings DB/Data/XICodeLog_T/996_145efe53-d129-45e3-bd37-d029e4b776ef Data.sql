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
 [XIGUID]) Values(N'996',
 NULL,
 N'0991d92c-f8',
 N'7b152d12-8f',
 N'XIIBO',
 N'Update_TODB',
 NULL,
 N'0',
 NULL,
 N'INSERT INTO XILayout_T(OrganizationID,FKiApplicationID,LayoutName,LayoutType,LayoutCode,XiParameterID,LayoutLevel,Authentication,iThemeID,bUseParentGUID,StatusTypeID,CreatedBy,CreatedTime,CreatedBySYSID,UpdatedBy,UpdatedTime,UpdatedBySYSID,sSiloAccess,bIsTaskBar,sTaskBarPosition,bAddToParentTaskbar,XIDeleted,XICreatedBy,XICreatedWhen,XIUpdatedBy,XIUpdatedWhen) output INSERTED.ID VALUES(''0'',''1'',''testGUID6TabLayoutTemplate'',''Template'',''<div class="row">
<div class="col-md-3" id="1"></div>
<div class="col-md-9">
 <div class="row">
           <div class="col-md-12" id="2"></div>
<div class="col-md-12" id="3"></div>
       </div>
</div>
</div>'',''1'',''OrganisationLevel'',''Authenticated'',''0'',''True'',''10'',''0'',''2022-03-22 17:40:33.000'',''fe80::503e:34b5:69df:3cf3%17'',''0'',''2022-03-22 17:40:33.000'',''fe80::503e:34b5:69df:3cf3%17'','''',''False'','''',''False'',''0'',''config configu'',''2022-03-22 17:40:33.000'',''config configu'',''2022-03-22 17:40:33.000'')',
 NULL,
 N'0.013000',
 N'0',
 NULL,
 N'0',
 N'',
 N'',
 N'2022-03-22 17:40:41.000',
 N'2022-03-22 17:40:41.000',
 N'145efe53-d129-45e3-bd37-d029e4b776ef')
