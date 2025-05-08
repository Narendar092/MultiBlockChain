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
 [FKiVersionIDXIGUID]) Values(N'10368',
 N'296',
 N'0',
 NULL,
 N'Link Legal simple cal',
 N'PolicyCalculation',
 N'        public static CResult PolicyMainCal(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            //XIIQS oQsInstance = new XIIQS();
            CResult oResult = new CResult();
            oResult.sMessage = "Legal simple cal script running";
            CNV oNV = new CNV();
            oNV.sName = "sCode";
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                int iInsatnceID = Convert.ToInt32(lParam.Where(m => m.sName == "iInsatnceID").FirstOrDefault().sValue);
                XIIBO oBII = new XIIBO();
                oBII.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "LegalAggregation", null);
                //oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = "16", bDirty = true };
                //oBII.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = "16", bDirty = true };
                //oBII.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = "32", bDirty = true };
                oBII.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = "130", bDirty = true };
                oBII.Attributes["FKiQSInstanceID"] = new XIIAttribute { sName = "FKiQSInstanceID", sValue = iInsatnceID.ToString(), bDirty = true };
                oBII.Attributes["sName"] = new XIIAttribute { sName = "sName", sValue = "Link Legal", bDirty = true };
                oBII.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = "130", bDirty = true };
                oBII.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = "", bDirty = true };
                var oResponse = oBII.Save(oBII);                                
                if (oResponse.bOK && oResponse.oResult != null)
                {
                    oBII = (XIIBO)oResponse.oResult;
                }

                oResult.oResult = "Success";
                oNV.sValue = "00";
            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.oResult = "Error";
                oNV.sValue = "100";
            }
            oResult.oCollectionResult.Add(oNV);
            return oResult;
        }                                   ',
 N'Prepersist',
 N'C# Code',
 N'0',
 N'2021-05-13 13:14:17.000',
 N'6',
 N'169.254.204.77',
 N'174',
 N'169.254.204.77',
 N'2021-05-13 13:14:17.000',
 N'15',
 N'PolicyMainCal',
 NULL,
 NULL,
 N'5',
 N'0',
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 N'4f6a0537-44b8-4641-bc0e-8e3a4cffc87e',
 N'0',
 N'0',
 N'0',
 N'2021-08-18 14:03:31.000',
 N'2021-08-18 14:03:31.000',
 NULL,
 NULL,
 NULL,
 N'610981b2-35c6-483e-bdee-5a08e5cbcf5f',
 NULL,
 N'64f97dfc-8306-4e18-868e-92f58bd9ca32',
 NULL,
 NULL)
