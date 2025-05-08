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
 [FKiVersionIDXIGUID]) Values(N'15',
 N'0',
 N'0',
 NULL,
 N'Order Date',
 N'Change the Order Date',
 N'public static XIResults Approve1(CXiAPI oXiAPI, List<cNameValuePairs> lParam)
        {
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
                string BOID = oXiAPI.Get_ParamVal(sSessionID, sUID, "|XI.boid");
                string InstanceID = oXiAPI.Get_ParamVal(sSessionID, sUID, "|XI.id");
                oBOInstance = oXiAPI.Load_BOInstance(Convert.ToInt32(BOID), Convert.ToInt32(InstanceID), sBOGroup, sActionName);
                string sFieldVal = oBOInstance.NVPairs.Where(m => m.sName == sName).FirstOrDefault().sValue;
                try { Thread.Sleep(5 * 1000); } catch (Exception e) { e.ToString(); }
                if (sFieldVal == sStatusVal.ToString())
                {
                    oResult.sCode = "200";
                    oResult.sResult = "Warning";
                    oResult.sMessage = "Warning: No update as Date is already at:" + sFieldVal;
                    InfNVPairs.sName = "dtOrderedOn";
                    InfNVPairs.sValue = "10-Mar-2018";
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
                    oResult.sMessage = "Order has been approved now.";
                    InfNVPairs.sName = "dtOrderedOn";
                    InfNVPairs.sValue = "10-Mar-2018";
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
 N'2018-06-11 00:00:00.000',
 N'1',
 NULL,
 N'6',
 N'192.168.7.7',
 N'2018-08-02 17:49:22.000',
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
 N'432b2a18-a675-4eb6-8749-10ce48fc439a',
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
