INSERT INTO [XIBOScript_T] ([ID],
 [FKiBOID],
 [FKiBOAttributeID],
 [sExecutionType],
 [sName],
 [sDescription],
 [sScript],
 [sType],
 [sLanguage],
 [StatusTypeID],
 [CreatedTime],
 [CreatedBy],
 [CreatedBySYSID],
 [UpdatedBy],
 [UpdatedBySYSID],
 [UpdatedTime],
 [FKiApplicationID],
 [sMethodName],
 [sVersion],
 [iOrder],
 [OrganisationID],
 [izXDeleted],
 [zXCrtdBy],
 [zXCrtdWhn],
 [zXUpdtdBy],
 [zXUpdtdWhn],
 [sLevel],
 [sCategory],
 [sClassification],
 [XIGUID],
 [XIDeleted],
 [XICreatedBy],
 [XIUpdatedBy],
 [XICreatedWhen],
 [XIUpdatedWhen],
 [sTag],
 [FKiTagSpaceID],
 [FKiVersionID],
 [FKiBOIDXIGUID],
 [FKiBOAttributeIDXIGUID],
 [FKiApplicationIDXIGUID],
 [FKiTagSpaceIDXIGUID],
 [FKiVersionIDXIGUID]) Values(N'10',
 N'0',
 N'0',
 NULL,
 N'Order Status',
 N'Change the Order Status',
 N'public static XIResults Approve(CXiAPI oXiAPI, List<cNameValuePairs> lParam)
        {   
            ModelDbContext dbContext = new ModelDbContext(oXiAPI.DevDB);   
            DataContext Spdb = new DataContext(oXiAPI.SharedDB); 
            XIResults oResult = new XIResults();
            cBOInstance oBOInstance = new cBOInstance();
            List<cInformationNVPairs> lInfNVPairs = new List<cInformationNVPairs>();
            cInformationNVPairs InfNVPairs = new cInformationNVPairs();
            try
            {
                string sStatusVal = lParam.Where(m => m.sName == "FieldValue").FirstOrDefault().sValue;
                string sName = lParam.Where(m => m.sName == "FieldName").FirstOrDefault().sValue;
                string sUID = lParam.Where(m => m.sName == "sUID").FirstOrDefault().sValue;
                string sSessionID = lParam.Where(m => m.sName == "sSessionID").FirstOrDefault().sValue;
                string sBOGroup = lParam.Where(m => m.sName == "Group").FirstOrDefault().sValue;
                string sActionName = lParam.Where(m => m.sName == "ScriptAction").FirstOrDefault().sValue;
                string BOID = oXiAPI.Get_ParamVal(sSessionID, sUID, null, "{XIP|BODID}");
                string InstanceID = oXiAPI.Get_ParamVal(sSessionID, sUID, null, "{XIP|Order.id}");
                string iUserID = oXiAPI.Get_ParamVal(sSessionID, sUID, null, "{XIP|UserID}");
                string sOrgName = oXiAPI.Get_ParamVal(sSessionID, sUID, null, "{XIP|OrgName}");
                oBOInstance = oXiAPI.Load_BOInstance(Convert.ToInt32(BOID), Convert.ToInt32(InstanceID), sBOGroup, sActionName, Convert.ToInt32(iUserID), sOrgName);
                string sFieldVal = oBOInstance.NVPairs.Where(m => m.sName == sName).FirstOrDefault().sValue;   
                if (sFieldVal == sStatusVal.ToString())
                {
                    oResult.sCode = "200";
                    oResult.sResult = "Warning";
                    oResult.sMessage = "Warning: No update as status is already at:" + sFieldVal;
                    InfNVPairs.sName = "TransactionCount";
                    InfNVPairs.sValue = "10";
                    lInfNVPairs.Add(InfNVPairs);
                    oResult.NVPairs = lInfNVPairs;
                }
                else
                {                    
                    oBOInstance.NVPairs.Where(m => m.sName == sName).FirstOrDefault().sValue = sStatusVal.ToString();
                    oBOInstance.NVPairs.Where(m => m.sName == sName).FirstOrDefault().bDirty = true;
                    oXiAPI.UpdateBO(oBOInstance);
                    oResult.sCode = "0";
                    oResult.sResult = "Success";
                    oResult.sMessage = "Success";
                    InfNVPairs.sName = "TransactionCount";
                    InfNVPairs.sValue = "10";
                    lInfNVPairs.Add(InfNVPairs);
                    oResult.NVPairs = lInfNVPairs;
                }
            }
            catch (Exception ex)
            {
                oResult.sCode = "100";
                oResult.sResult = "Error";
                oResult.sMessage = ex.ToString();
            }
            return oResult;
        }',
 N'Prepersist',
 N'C# Code',
 N'0',
 N'2018-03-02 00:00:00.000',
 N'1',
 NULL,
 N'6',
 N'169.254.80.80',
 N'2018-08-03 15:20:08.000',
 N'15',
 NULL,
 N'null',
 NULL,
 N'5',
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 N'8aa7dbac-ad01-4f9e-a21b-2868c77446b9',
 N'0',
 N'0',
 N'0',
 N'2021-08-18 14:03:31.000',
 N'2021-08-18 14:03:31.000',
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 N'64f97dfc-8306-4e18-868e-92f58bd9ca32',
 NULL,
 NULL)
