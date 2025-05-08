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
 [FKiVersionIDXIGUID]) Values(N'30554',
 N'296',
 N'0',
 NULL,
 N'Granite Calculation',
 N'Granite Calculation',
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
                //oResult = Pcal.Calculation(iInsatnceID, iUserID, iCustomerID, sDataBase, sProductName, sVersion, sProductCode, sSessionID, sUID, iQuoteID);
                oResult = Pcal.Calculation(iInsatnceID, iUserID, iCustomerID, sVersion, sProductCode, iQuoteID);
                // oResult.xiStatus = 00;
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
            public int iInstanceID { get; set; }
            public CResult GetAgeFromDOB(string dDOB, string PresentDate)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    int iAge = DateTime.Now.Year - Convert.ToDateTime(dDOB).Year;
                    if (DateTime.Now < Convert.ToDateTime(dDOB).AddYears(iAge)) iAge--;
                    oresult.oResult = iAge;
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult, iInstanceID);
                }
                return oresult;
            }
            public CResult GetNCBandYearValue(string MTNCBType)
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
                    bool NCBType = true;
                    if (MTNCBType.ToLower() == "car" || MTNCBType.ToLower() == "van" || MTNCBType.ToLower() == "taxi")
                    {
                        NCBType = false;
                        oNV.sName = "sMessage";
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                        oNV1.sName = "oResult";
                        oNV1.sValue = "0";
                        oNV2.sName = "LoadFactorName";
                        oNV2.sValue = "Unacceptable NCB Type";
                        oNV3.sName = "Type";
                        oNV3.sValue = "Percent";
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        oNV1.sValue = NCBType.ToString();
                        oCresult.oCollectionResult.Add(oNV);
                        oCresult.oCollectionResult.Add(oNV1);
                        oCresult.oCollectionResult.Add(oNV2);
                        oCresult.oCollectionResult.Add(oNV3);
                    }
                    oCresult.oResult = NCBType;
                }
                catch (Exception ex)
                {
                    oCresult.sMessage = "ERROR: [" + oCresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCresult, iInstanceID);
                }

                return oCresult;
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
                    List<CResult> oXiResults = new List<CResult>();
                    //PolicyBaseCalc PolicyCal = new PolicyBaseCalc();
                    List<CResult> ClaimResultList = new List<CResult>();
                    //PolicyCal.iInstanceID = iInsatnceID;
                    //oresult.xiStatus = 00;//xiEnumSystem.xiFuncResult.xiSuccess;
                    XIIXI oIXI = new XIIXI();
                    var sStructureName = "MT QS";
                    var oQSSI = oIXI.BOI("QS Instance", iInsatnceID.ToString()).Structure(sStructureName).XILoad();
                    var ostructureInstance = oQSSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();


                    //NCB
                    string NCBPreviousHistory = ostructureInstance.XIIValue("bMTPreviousNCBTradeNCBnotconverted").sResolvedValue;
                    string NCBYears = ostructureInstance.XIIValue("iMTNumberofYearsNCB").sResolvedValue;
                    string NCBBounsType = ostructureInstance.XIIValue("iMTNoClaimsBonusType").sResolvedValue;


                    XIIBO oBII = new XIIBO();
                    XIAPI oXIAPI = new XIAPI();

                    XIInfraCache oCache = new XIInfraCache();
                    var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
                    int iProductID = 0;
                    string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                    if (int.TryParse(ProductID, out iProductID)) { }
                    var oProductI = oIXI.BOI("Product", ProductID);
                    oProductI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Product");
                    oProductI.Attributes["iDefaultAmountType"].BOD = oProductI.BOD;
                    string sMinPremium = oProductI.Attributes["rMinPremium"].sValue;
                    //double rMinPremium = 0.0;
                    oBII.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations");
                    if (oXiResults.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())))                        //&& oExcessVal.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && LeftHandLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && IPTLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && oGeneralDeclines.Count <= 0)
                    {
                        oBII.Attributes["rPaymentCharge"] = new XIIAttribute { sName = "rPaymentCharge", sValue = oProductI.Attributes["rPaymentCharge"].sValue, bDirty = true };
                        oBII.Attributes["rInsurerCharge"] = new XIIAttribute { sName = "rInsurerCharge", sValue = oProductI.Attributes["rInsurerCharge"].sValue, bDirty = true };
                        //if (double.TryParse(MinimumDeposit, out rMinDeposit)) { }
                        oBII.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = Math.Round(rTotalAmount, 2).ToString(), bDirty = true };
                        //oBII.Attributes["sGroupQuote"] = new XIIAttribute { sName = "sGroupQuote", sValue = MinDAgeGroupvalue, bDirty = true };
                        oBII.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = Math.Round(rTotalAmount, 2).ToString(), bDirty = true };
                        //oBII.Attributes["zDefaultDeposit"] = new XIIAttribute { sName = "zDefaultDeposit", sValue = MinimumDeposit, bDirty = true };
                        oBII.Attributes["zDefaultAdmin"] = new XIIAttribute { sName = "zDefaultAdmin", sValue = oProductI.Attributes["zDefaultAdmin"].sValue, bDirty = true };
                        oBII.Attributes["bIsCoverAbroad"] = new XIIAttribute { sName = "bIsCoverAbroad", sValue = oProductI.Attributes["bIsCoverAbroad"].sValue, bDirty = true };
                        //oBII.Attributes["sExcess"] = new XIIAttribute { sName = "sExcess", sValue = ExcessContent, bDirty = true };
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "0", bDirty = true };
                        //oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = rCompulsaryExcess.ToString(), bDirty = true };
                    }
                    var sQuoteGUID = Guid.NewGuid().ToString("N").Substring(0, 10);

                    var oSource = oIXI.BOI("XISource_T", ostructureInstance.Attributes["fkisourceid"].sValue);
                    string sPrefix = string.Empty;

                    if (oSource != null && oSource.Attributes != null && oSource.Attributes.ContainsKey("sprefixcode"))
                    {
                        sPrefix = oSource.Attributes["sprefixcode"].sValue;
                    }
                    bool NcbType = true;
                    //if (NCBBounsType.ToLower() == "car" || NCBBounsType.ToLower() == "van")
                    //{
                    PolicyBaseCalc poli = new PolicyBaseCalc();
                    var NCBResult = poli.GetNCBandYearValue(NCBBounsType);
                    if (NCBResult.xiStatus == 0)
                    {
                        string sBase = "";
                        if (NCBResult.oCollectionResult.Count > 0)
                        {
                            oBaseLoadFactor = NCBResult;
                            sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                            bool.TryParse(sBase, out NcbType);
                            oXiResults.Add(NCBResult);
                        }
                    }
                    if (NcbType == false)
                    {
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "90", bDirty = true };
                        //oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        //oresult.oResult = null;
                    }
                    else
                    {
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "40", bDirty = true };
                        //oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        //oresult.oResult = "Success";
                    }
                    var iBatchID = iCustomerID.ToString() + iInsatnceID.ToString();
                    oBII.Attributes["FKiQSInstanceIDXIGUID"] = new XIIAttribute { sName = "FKiQSInstanceIDXIGUID", sValue = iInsatnceID.ToString(), bDirty = true };
                    //oBII.Attributes["sInsurer"] = new XIIAttribute { sName = "sInsurer", sValue = "", bDirty = true };
                    oBII.Attributes["FKiCustomerID"] = new XIIAttribute { sName = "FKiCustomerID", sValue = iCustomerID.ToString(), bDirty = true };
                    oBII.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                    oBII.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString("dd-MMM-yyyy"), bDirty = true };
                    oBII.Attributes["FKiProductVersionID"] = new XIIAttribute { sName = "FKiProductVersionID", sValue = sVersion, bDirty = true };
                    oBII.Attributes["sGUID"] = new XIIAttribute { sName = "sGUID", sValue = sQuoteGUID, bDirty = true };
                    oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + Guid.NewGuid().ToString("N").Substring(0, 6), bDirty = true };
                    oBII.Attributes["BatchID"] = new XIIAttribute { sName = "BatchID", sValue = iBatchID, bDirty = true };
                    oBII.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = iQuoteID.ToString(), bDirty = true };
                    //oBII.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = rTotalAmount.ToString(), bDirty = true };
                    //oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = rCompulsaryExcess.ToString(), bDirty = true };
                    //oBII.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = rVoluntaryExcess.ToString(), bDirty = true };
                    //oBII.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = rTotalExcess.ToString(), bDirty = true };
                    //Info.Add("QuoteRefID_" + oBII.Attributes["sRefID"].sValue);
                    var oRes = oBII.Save(oBII);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        oBII = (XIIBO)oRes.oResult;
                    }
                    Info.Add("Granite Quote inserted Sucessfully with the amount of " + rTotalAmount);
                    XIDBO oRiskFactorsBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
                    XIIBO oBO = new XIIBO();
                    //oBO.BOD = oRiskFactorsBOD;
                    List<XIIBO> oBOIList = new List<XIIBO>();
                    foreach (var item in oXiResults)
                    {
                        oBO = new XIIBO();
                        oBO.BOD = oRiskFactorsBOD;
                        oBO.Attributes["FKiQuoteID"] = new XIIAttribute { sName = "FKiQuoteID", sValue = oBII.Attributes["ID"].sValue, bDirty = true };
                        oBO.Attributes["sFactorName"] = new XIIAttribute { sName = "sFactorName", sValue = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                        oBO.Attributes["sValue"] = new XIIAttribute { sName = "sValue", sValue = item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                        oBO.Attributes["sMessage"] = new XIIAttribute { sName = "sMessage", sValue = item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                        oBO.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                        oBO.Attributes["FKsQuoteID"] = new XIIAttribute { sName = "FKsQuoteID", sValue = sQuoteGUID, bDirty = true };
                        oBO.Attributes["ID"] = new XIIAttribute { sName = "ID", bDirty = true };
                        oBOIList.Add(oBO);
                    }
                    QueryEngine oQEE = new QueryEngine();
                    List<XIWhereParams> oWhereParams = new List<XIWhereParams>();
                    List<SqlParameter> SqlParams = new List<SqlParameter>();
                    oWhereParams.Add(new XIWhereParams { sField = "FKiQuoteID", sOperator = "=", sValue = oBII.Attributes["ID"].sValue });
                    SqlParams.Add(new SqlParameter { ParameterName = "@FKiQuoteID", Value = oBII.Attributes["ID"].sValue });

                    oQEE.AddBO("RiskFactor_T", "", oWhereParams);
                    CResult oresult1 = oQEE.BuildQuery();
                    //oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "BO Instance query build successfully" });
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
                    string sInfo = "INFO: " + string.Join(",\r\n ", Info);

                    oresult.oCollectionResult.Add(new CNV { sName = "QuoteID", sValue = oBII.Attributes["ID"].sValue });
                    oresult.sMessage = "Success";

                    oresult.sMessage = sInfo;
                    oIB.SaveErrortoDB(oresult);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                //oresult.oResult = rTotalAmount;

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
 N'183fa1ba-656b-42f4-934c-66d9e6972f0d',
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
