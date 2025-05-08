Delete from XIBOScript_T where xiguid='2ed1dca3-a66b-40a8-b459-6f34d09449fc';
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
 [FKiVersionIDXIGUID]) Values(N'30552',
 N'296',
 N'0',
 NULL,
 N'Step Validation',
 N'Page Validation',
 N'public static CResult PolicyMainCal(List<CNV> lParam)
{
    XIInstanceBase oIB = new XIInstanceBase();
    PolicyCalculation Pcal = new PolicyCalculation();
    CResult oResult = new CResult();
    CNV oNV = new CNV();
    oNV.sName = "sCode";
    try
    {
        string sUID = lParam.Where(m => m.sName == "sUID").FirstOrDefault().sValue;
        string iInsatnceID = lParam.Where(m => m.sName == "iInsatnceID").FirstOrDefault().sValue;
        int iUserID = Convert.ToInt32(lParam.Where(m => m.sName == "iUserID").FirstOrDefault().sValue);
        int iCustomerID = Convert.ToInt32(lParam.Where(m => m.sName == "iCustomerID").FirstOrDefault().sValue);
        string sDataBase = lParam.Where(m => m.sName == "sDataBase").FirstOrDefault().sValue;
        string sProductName = lParam.Where(m => m.sName == "ProductName").FirstOrDefault().sValue;
        string sVersion = lParam.Where(m => m.sName == "Version").FirstOrDefault().sValue;
        string sSessionID = lParam.Where(m => m.sName == "sSessionID").FirstOrDefault().sValue;
        string sProductCode = lParam.Where(m => m.sName == "ProductCode").FirstOrDefault().sValue;
        int iQuoteID = Convert.ToInt32(lParam.Where(m => m.sName == "iQuoteID").FirstOrDefault().sValue);
        oResult = Pcal.Calculation(iInsatnceID, iUserID, iCustomerID, sVersion, sProductCode, iQuoteID);
        oNV.sValue = "00";
    }
    catch (Exception ex)
    {
        oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
        oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
        oResult.oResult = "Error";
        oNV.sValue = "100";
        oIB.SaveErrortoDB(oResult);
    }
    oResult.oCollectionResult.Add(oNV);
    return oResult;
}
public class PolicyBaseCalc
{
    public int iInstanceID
    {
        get;
        set;
    }
    public CResult PageValidation(string sDisplayName /*KeyValuePair<string,XIIValue> oParams*/ )
    {
        CResult oCresult = new CResult();
        XIInstanceBase oIB = new XIInstanceBase();
        try
        {
            oCresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            CNV oNV = new CNV();
            CNV oNV1 = new CNV();
            CNV oNV2 = new CNV();
            CNV oNV3 = new CNV();
            bool Validation = false;
            oNV.sName = "sMessage";
            oNV1.sName = "oResult";
            oNV1.sValue = "Validation";
            oNV2.sName = "LoadFactorName";
            oNV2.sValue = "Please Check " + sDisplayName;
            oNV3.sName = "Type";
            oNV3.sValue = "Percent";
            oNV.sValue = "Failed";
            oCresult.oCollectionResult.Add(oNV);
            oCresult.oCollectionResult.Add(oNV1);
            oCresult.oCollectionResult.Add(oNV2);
            oCresult.oCollectionResult.Add(oNV3);
            oCresult.oResult = Validation;
        }
        catch (Exception ex)
        {
            oCresult.sMessage = "ERROR: [" + oCresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
            oCresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
            oIB.SaveErrortoDB(oCresult, iInstanceID);
        }
        return oCresult;
    }

    public List<CResult> PreValidateIndimity(List<XIDQSStep> CoverReq , Dictionary<string, XIIValue> XIIValuesforAll)
    {
        List<string> Info = new List<string>();
        XIInstanceBase oIB = new XIInstanceBase();
        CResult oresult = new CResult();
        CResult oBaseLoadFactor = new CResult();
        List<CResult> oXiResults = new List<CResult>();
        try
        {
            PolicyBaseCalc BaseCal = new PolicyBaseCalc();
            XIInfraCache oCache = new XIInfraCache();
            bool bStepValidation = true;
            CResult IndeminityStep = new CResult();
            if (CoverReq!=null)
            {
                var oStepslistt = CoverReq;
                //var oStepNamee = oStepslistt[7].sName;
                foreach (var ostepitems in CoverReq)
                {
                    var policycover = XIIValuesforAll.Where(k=>k.Key.ToLower()=="imtpolicycover").Select(l=>l.Value.sValue).FirstOrDefault();
                    var DisplayName = ostepitems.FieldDefs.Where(k => k.Key.ToLower() == "imtpolicycover").Select(l => l.Value.FieldOrigin.sDisplayName).FirstOrDefault();
                    if (string.IsNullOrEmpty(policycover))
                    {
                        PageValidation(DisplayName);
                    }
                    if (policycover == "20" || policycover == "30")
                    {
                        var oValue1 = XIIValuesforAll.Where(k => k.Key.ToLower() == "rlimitofindemnity").Select(l => l.Value.sValue).FirstOrDefault();
                        if (string.IsNullOrEmpty(oValue1))
                        {
                            DisplayName = ostepitems.FieldDefs.Where(k => k.Key.ToLower() == "rlimitofindemnity").Select(l => l.Value.FieldOrigin.sDisplayName).FirstOrDefault();
                           IndeminityStep= PageValidation(DisplayName);
                            if (IndeminityStep.xiStatus == 0)
                            {
                                string sBase = "";
                                if (IndeminityStep.oCollectionResult.Count > 0)
                                {
                                    bool bCurrentStep1 = true;
                                    oBaseLoadFactor = IndeminityStep;
                                    sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                                    bool.TryParse(sBase, out bCurrentStep1);
                                    if (bCurrentStep1 == false)
                                    {
                                        bStepValidation = false;
                                    }
                                    oXiResults.Add(IndeminityStep);
                                }
                            }
                        }
                        var oVal2 = XIIValuesforAll.Where(k => k.Key.ToLower() == "rmtlimitofindemnitypersonalandstockvehicles").Select(l => l.Value.sValue).FirstOrDefault();
                        if (string.IsNullOrEmpty(oVal2))
                        {
                            DisplayName = ostepitems.FieldDefs.Where(k => k.Key.ToLower() == "rmtlimitofindemnitypersonalandstockvehicles").Select(l => l.Value.FieldOrigin.sDisplayName).FirstOrDefault();
                            IndeminityStep = PageValidation(DisplayName);
                            if (IndeminityStep.xiStatus == 0)
                            {
                                string sBase = "";
                                if (IndeminityStep.oCollectionResult.Count > 0)
                                {
                                    bool bCurrentStep1 = true;
                                    oBaseLoadFactor = IndeminityStep;
                                    sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                                    bool.TryParse(sBase, out bCurrentStep1);
                                    if (bCurrentStep1 == false)
                                    {
                                        bStepValidation = false;
                                    }
                                    oXiResults.Add(IndeminityStep);
                                }
                            }
                        }
                    }
                    else if (policycover == "40")
                    {
                        var oValue1 = XIIValuesforAll.Where(k => k.Key.ToLower() == "rlimitofindemnity2").Select(l => l.Value.sValue).FirstOrDefault();
                        if (string.IsNullOrEmpty(oValue1))
                        {
                            DisplayName = ostepitems.FieldDefs.Where(k => k.Key.ToLower() == "rlimitofindemnity2").Select(l => l.Value.FieldOrigin.sDisplayName).FirstOrDefault();
                            IndeminityStep = PageValidation(DisplayName); if (IndeminityStep.xiStatus == 0)
                            {
                                string sBase = "";
                                if (IndeminityStep.oCollectionResult.Count > 0)
                                {
                                    bool bCurrentStep1 = true;
                                    oBaseLoadFactor = IndeminityStep;
                                    sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                                    bool.TryParse(sBase, out bCurrentStep1);
                                    if (bCurrentStep1 == false)
                                    {
                                        bStepValidation = false;
                                    }
                                    oXiResults.Add(IndeminityStep);
                                }
                            }
                        }
                        var oVal2 = XIIValuesforAll.Where(k => k.Key.ToLower() == "rmtlimitofindemnitypersonalandstockvehicles2").Select(l => l.Value.sValue).FirstOrDefault();
                        if (string.IsNullOrEmpty(oVal2))
                        {
                            DisplayName = ostepitems.FieldDefs.Where(k => k.Key.ToLower() == "rmtlimitofindemnitypersonalandstockvehicles2").Select(l => l.Value.FieldOrigin.sDisplayName).FirstOrDefault();
                            IndeminityStep = PageValidation(DisplayName); if (IndeminityStep.xiStatus == 0)
                            {
                                string sBase = "";
                                if (IndeminityStep.oCollectionResult.Count > 0)
                                {
                                    bool bCurrentStep1 = true;
                                    oBaseLoadFactor = IndeminityStep;
                                    sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                                    bool.TryParse(sBase, out bCurrentStep1);
                                    if (bCurrentStep1 == false)
                                    {
                                        bStepValidation = false;
                                    }
                                    oXiResults.Add(IndeminityStep);
                                }
                            }
                        }
                    }

                }
            }
        }
        catch (Exception ex)
        {
            //oXiResults.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
            //oXiResults.xiStatus = xiEnumSystem.xiFuncResult.xiError;
        }

        return oXiResults;
    }
}
public class PolicyCalculation
{
    public CResult Calculation(string iInsatnceID, int iUserID, int iCustomerID, string sVersion, string sProductCode, int iQuoteID)
    {
        List<string> Info = new List<string>();
        Info.Add("[QsInstanceID_" + iInsatnceID + "]");
        Info.Add("Granite script running");
        double rTotalAmount = 0;
        XIInstanceBase oIB = new XIInstanceBase();
        CResult oresult = new CResult();
        CResult oBaseLoadFactor = new CResult();
        try
        {
            PolicyBaseCalc BaseCal = new PolicyBaseCalc();
            List<CResult> oXiResults = new List<CResult>();
            List<CResult> ClaimResultList = new List<CResult>();
            XIIXI oIXI = new XIIXI();
            var sStructureName = "MT QS";
            var oQSSI = oIXI.BOI("QS Instance", iInsatnceID).Structure(sStructureName).XILoad(null, true);
            var ostructureInstance = oQSSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
            var QSUID = ostructureInstance.AttributeI("FKiQSDefinitionIDXIGUID").sValue;
            XIIBO oBII = new XIIBO();
            XIAPI oXIAPI = new XIAPI();
            bool bStepValidation = true;
            XIInfraCache oCache = new XIInfraCache();
            XIDQS oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, QSUID.ToString());
            if (oQSD.Steps.Count() > 0)
            {
                foreach (var ostepitems in oQSD.Steps)
                {
                    SqlConnection ConnA = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
                    ConnA.Open();
                    //var sTableName = "XIFieldDefinition_T";
                    var oFieldDef = new SqlCommand(@"SELECT * FROM [dbo].[XIFieldDefinition_T] Where XIDeleted =0 and FKiXIStepDefinitionIDXIGUID =''" + ostepitems.Value.XIGUID.ToString() + "''", ConnA);
                    DataTable dt = new DataTable();
                    var reader = oFieldDef.ExecuteReader();
                    dt.Load(reader);
                    var leadcount = dt.Rows.Count;
                    foreach (DataRow items in dt.Rows)
                    {
                        if (items.ItemArray.Count() > 0)
                        {
                            var Fielddef = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, "", items.ItemArray[20].ToString());
                            var oXIValue = ostructureInstance.XIIValues.ToList().Where(m => m.Value.FKiFieldOriginIDXIGUID.ToString().ToLower() == items.ItemArray[20].ToString().ToLower()).Select(m => m.Value.sValue).FirstOrDefault();
                            if (Fielddef.bIsPrescriptvalidation == true && string.IsNullOrEmpty(oXIValue))
                            {
                                var Steps = BaseCal.PageValidation(Fielddef.sDisplayName);
                                bool bCurrentStep = true;
                                if (Steps.xiStatus == 0)
                                 {
                                    string sBase = "";
                                    if (Steps.oCollectionResult.Count > 0)
                                    {
                                        oBaseLoadFactor = Steps;
                                        sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                                        bool.TryParse(sBase, out bCurrentStep);
                                        if (bCurrentStep == false)
                                        {
                                            bStepValidation = false;
                                        }
                                        oXiResults.Add(Steps);
                                    }
                                }
                            }
                        }
                    }
                    ConnA.Close();
                }
            }
            var CoverReq = oQSD.Steps.Where(k=>k.Key.ToLower()=="COVER REQUIRED".ToLower()).Select(l=>l.Value).ToList();
            var IndeminityStep = BaseCal.PreValidateIndimity(CoverReq, ostructureInstance.XIIValues);
            foreach(var item in IndeminityStep)
            {
                oXiResults.Add(item);
            }
            var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
            int iProductID = 0;
            string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
            if (int.TryParse(ProductID, out iProductID)) { }
            var oProductI = oIXI.BOI("Product", ProductID);
            oProductI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Product");
            oProductI.Attributes["iDefaultAmountType"].BOD = oProductI.BOD;
            string sMinPremium = oProductI.Attributes["rMinPremium"].sValue;
            Info.Add("Granite Quote inserted Sucessfully with the amount of " + rTotalAmount);
            XIDBO oRiskFactorsBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
            XIIBO oBO = new XIIBO();
            List<XIIBO> oBOIList = new List<XIIBO>();
            var sQuoteGUID = Guid.NewGuid().ToString("N").Substring(0, 10);
            foreach (var item in oXiResults)
            {
                oBO = new XIIBO();
                oBO.BOD = oRiskFactorsBOD;
                oBO.Attributes["FKiQuoteID"] = new XIIAttribute
                {
                    sName = "FKiQuoteID",
                    sValue = iQuoteID.ToString(),
                    bDirty = true
                };
                oBO.Attributes["sFactorName"] = new XIIAttribute
                {
                    sName = "sFactorName",
                    sValue = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault(),
                    bDirty = true
                };
                oBO.Attributes["sValue"] = new XIIAttribute
                {
                    sName = "sValue",
                    sValue = item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(),
                    bDirty = true
                };
                oBO.Attributes["sMessage"] = new XIIAttribute
                {
                    sName = "sMessage",
                    sValue = item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault(),
                    bDirty = true
                };
                oBO.Attributes["CreatedTime"] = new XIIAttribute
                {
                    sName = "CreatedTime",
                    sValue = DateTime.Now.ToString(),
                    bDirty = true
                };
                oBO.Attributes["FKsQuoteID"] = new XIIAttribute
                {
                    sName = "FKsQuoteID",
                    sValue = sQuoteGUID,
                    bDirty = true
                };
                oBO.Attributes["ID"] = new XIIAttribute
                {
                    sName = "ID",
                    bDirty = true
                };
                oBOIList.Add(oBO);
            }
            QueryEngine oQEE = new QueryEngine();
            List<XIWhereParams> oWhereParams = new List<XIWhereParams>();
            List<SqlParameter> SqlParams = new List<SqlParameter>();
            oWhereParams.Add(new XIWhereParams
            {
                sField = "FKiQuoteID",
                sOperator = "=",
                sValue = iQuoteID.ToString()
            });
            SqlParams.Add(new SqlParameter
            {
                ParameterName = "@FKiQuoteID",
                Value = iQuoteID.ToString()
            });
            oQEE.AddBO("RiskFactor_T", "", oWhereParams);
            CResult oresult1 = oQEE.BuildQuery();
            if (oresult1.bOK && oresult1.oResult != null)
            {
                var sSql = (string)oresult1.oResult;
                ExecutionEngine oEE = new ExecutionEngine();
                oEE.XIDataSource = oQEE.XIDataSource;
                oEE.sSQL = sSql;
                oEE.SqlParams = SqlParams;
                var oQResult = oEE.Execute();
                if (oQResult.bOK && oQResult.oResult != null)
                {
                    var oBOIListData = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                    var oBOD = oQEE.QParams.FirstOrDefault().BOD;
                    oBOIListData.ForEach(x => x.BOD = oBOD);
                    if (oBOIListData.Count() > 0)
                    {
                        foreach (var oBOInstance in oBOIListData)
                        {
                            oBOInstance.Delete(oBOInstance);
                        }
                    }
                }
            }
            XIIBO xibulk = new XIIBO();
            DataTable dtbulk = xibulk.MakeBulkSqlTable(oBOIList);
            if (oBOIList.Count > 0)
            {
                xibulk.SaveBulk(dtbulk, oBOIList[0].BOD.iDataSourceXIGUID.ToString(), oBOIList[0].BOD.TableName);
            }
            oBII = new XIIBO();
            oBII.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations");
            if (oXiResults.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()))) { }
            var oSource = oIXI.BOI("XISource_T", ostructureInstance.Attributes["fkisourceid"].sValue);
            string sPrefix = string.Empty;
            if (oSource != null && oSource.Attributes != null && oSource.Attributes.ContainsKey("sprefixcode"))
            {
                sPrefix = oSource.Attributes["sprefixcode"].sValue;
            }
            if (bStepValidation == false)
            {
                oBII.Attributes["iQuoteStatus"] = new XIIAttribute
                {
                    sName = "iQuoteStatus",
                    sValue = "90",
                    bDirty = true
                };
            }
            else
            {
                oBII.Attributes["iQuoteStatus"] = new XIIAttribute
                {
                    sName = "iQuoteStatus",
                    sValue = "40",
                    bDirty = true
                };
            }
            var iBatchID = iCustomerID.ToString() + iInsatnceID;
            oBII.Attributes["FKiQSInstanceIDXIGUID"] = new XIIAttribute
            {
                sName = "FKiQSInstanceIDXIGUID",
                sValue = iInsatnceID,
                bDirty = true
            };
            oBII.Attributes["FKiCustomerID"] = new XIIAttribute
            {
                sName = "FKiCustomerID",
                sValue = iCustomerID.ToString(),
                bDirty = true
            };
            oBII.Attributes["FKiUserID"] = new XIIAttribute
            {
                sName = "FKiUserID",
                sValue = iUserID.ToString(),
                bDirty = true
            };
            oBII.Attributes["CreatedTime"] = new XIIAttribute
            {
                sName = "CreatedTime",
                sValue = DateTime.Now.ToString("dd-MMM-yyyy"),
                bDirty = true
            };
            oBII.Attributes["FKiProductVersionID"] = new XIIAttribute
            {
                sName = "FKiProductVersionID",
                sValue = sVersion,
                bDirty = true
            };
            oBII.Attributes["sGUID"] = new XIIAttribute
            {
                sName = "sGUID",
                sValue = sQuoteGUID,
                bDirty = true
            };
            oBII.Attributes["sRefID"] = new XIIAttribute
            {
                sName = "sRefID",
                sValue = sPrefix + Guid.NewGuid().ToString("N").Substring(0, 6),
                bDirty = true
            };
            oBII.Attributes["BatchID"] = new XIIAttribute
            {
                sName = "BatchID",
                sValue = iBatchID,
                bDirty = true
            };
            oBII.Attributes["ID"] = new XIIAttribute
            {
                sName = "ID",
                sValue = iQuoteID.ToString(),
                bDirty = true
            };
            var oRes = oBII.Save(oBII);
            if (oRes.bOK && oRes.oResult != null)
            {
                oBII = (XIIBO)oRes.oResult;
            }
            string sInfo = "INFO: " + string.Join(",\r\n ", Info);
            oresult.oCollectionResult.Add(new CNV
            {
                sName = "QuoteID",
                sValue = oBII.Attributes["ID"].sValue
            });
            oresult.sMessage = "Success";
            oresult.sMessage = sInfo;
            oIB.SaveErrortoDB(oresult);
        }
        catch (Exception ex)
        {
            oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
            oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
        }
        return oresult;
    }
}',
 N'Prepersist',
 N'C# Code',
 N'0',
 N'2022-11-29 08:03:05.000',
 N'0',
 NULL,
 N'2035',
 N'172.16.1.5',
 N'2022-11-29 08:03:05.000',
 N'1',
 N'PolicyMainCal',
 N'0',
 NULL,
 N'1',
 N'0',
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 N'2ed1dca3-a66b-40a8-b459-6f34d09449fc',
 N'0',
 N'',
 N'',
 N'2022-06-23 11:04:44.000',
 N'2022-06-23 11:04:44.000',
 NULL,
 NULL,
 NULL,
 N'610981b2-35c6-483e-bdee-5a08e5cbcf5f',
 NULL,
 N'2b4445fc-c977-40d4-9590-5470df8c7e02',
 NULL,
 NULL)
