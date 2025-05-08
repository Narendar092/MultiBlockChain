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
 [FKiVersionIDXIGUID]) Values(N'227',
 N'296',
 N'0',
 NULL,
 N'SCTowerGate Calculation_24-08-19',
 N'PolicyCalculation',
 N'  public static CResult SCTowerGateCalculation(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            //XIIQS oQsInstance = new XIIQS();
            SCTowergate Pcal = new SCTowergate();
            CResult oResult = new CResult();
            oResult.sMessage = "SCTowerGate script running";
            oIB.SaveErrortoDB(oResult);
            CNV oNV = new CNV();
            oNV.sName = "sCode";
            try
            {
                string sGUID = lParam.Where(m => m.sName == "sUID").FirstOrDefault().sValue;
                int iInstanceID = Convert.ToInt32(lParam.Where(m => m.sName == "iInsatnceID").FirstOrDefault().sValue);
                int iUserID = Convert.ToInt32(lParam.Where(m => m.sName == "iUserID").FirstOrDefault().sValue);
                int iCustomerID = Convert.ToInt32(lParam.Where(m => m.sName == "iCustomerID").FirstOrDefault().sValue);
                string sDataBase = lParam.Where(m => m.sName == "sDataBase").FirstOrDefault().sValue;
                string sProductName = lParam.Where(m => m.sName == "ProductName").FirstOrDefault().sValue;
                string sVersion = lParam.Where(m => m.sName == "Version").FirstOrDefault().sValue;
                string sSessionID = lParam.Where(m => m.sName == "sSessionID").FirstOrDefault().sValue;
                string sProductCode = lParam.Where(m => m.sName == "ProductCode").FirstOrDefault().sValue;
                int iQuoteID = Convert.ToInt32(lParam.Where(m => m.sName == "iQuoteID").FirstOrDefault().sValue);
                //XIInfraCache XIInfraCache = new XIInfraCache();
                //oQsInstance = XIInfraCache.Get_QuestionSetCache("QuestionSetCache", sUID, iInsatnceID);
                //oResult.sMessage = "In Script: Questionset object Loaded from cache";
                //oIB.SaveErrortoDB(oResult);
                oResult = Pcal.GetCaravanFinalPremium(sGUID, iInstanceID, iUserID, sProductName, sVersion, sSessionID, iCustomerID, iQuoteID);
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
        public class SCTowergate
        {
            public CResult GetAgeFromDOB(string dDOB, string PresentDate)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    oresult.sMessage = "dDOB_" + dDOB;
                    oIB.SaveErrortoDB(oresult);
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult BuildCResultObject(string Value, string Name, string Status)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = Status;
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = Value;
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = Name;
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetBaseRate(string sSumInsured)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                int iSumInsured = 0;
                if (int.TryParse(sSumInsured,out iSumInsured)) { }
                try
                {
                    oresult.sMessage = "SumInsured-" + sSumInsured;
                    oIB.SaveErrortoDB(oresult);
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "+0";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Base Rate";
                    if(iSumInsured<= 29999)
                    {
                        oNV1.sValue = "200";
                    }
                    else if(iSumInsured <= 60000)
                    {
                        oNV1.sValue = "230";
                    }
                    else if (iSumInsured <= 999999)
                    {
                        oNV1.sValue = "250";
                    }
                    oresult.sMessage = "Base Rate-" + oNV1.sValue + "_" + oNV.sValue;
                    oIB.SaveErrortoDB(oresult);
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetIPTLoadFactor()
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "+12";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "IPT";
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public static string GetPostCodeLookUp(string PostCode, int EngineSize, string sDataBase, string sProductName, string sVersion, string sProductCode)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                string res = "";
                try
                {
                    XIPostCodeLookUp oPostCodeLookUp = new XIPostCodeLookUp();
                    oPostCodeLookUp.sMPostCode = PostCode;
                    oPostCodeLookUp.sProductName = sProductName;
                    oPostCodeLookUp.sVersion = sVersion;
                    oPostCodeLookUp.sProductCode = sProductCode;
                    var PostCodeGroup = oPostCodeLookUp.Get_PostCode();
                    var Area = "";
                    if (PostCodeGroup.xiStatus == 0 && PostCodeGroup.oResult != null)
                    {
                        Area = PostCodeGroup.oResult.ToString();
                        oResult.sMessage = "PostCode:" + PostCode + "Area:" + Area;
                        oIB.SaveErrortoDB(oResult);
                        if (Area.Contains("Refer") || Area.Contains("Decline"))
                        {
                            res = Area;
                        }
                        else
                        {
                            //var Group = EngineLookUp(EngineSize);
                            //res = Area + Group;
                        }
                    }
                }
                catch (Exception ex)
                {
                    oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oResult.oResult = "Error";
                    oIB.SaveErrortoDB(oResult);
                }
                return res;
            }
            public CResult GetCoverLoad(string sTypeofCover)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Cover";
                    if(!string.IsNullOrEmpty(sTypeofCover)&& sTypeofCover== "Market value")
                    {
                        oNV1.sValue = "30";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetClaimLoadFactor(int iNoOfClaims, List<XIIBO> ClaimI, string CoverStartDate)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "ClaimLoadFactor";
                    if (iNoOfClaims<=2)
                    {
                        if(iNoOfClaims==1)
                        {
                            foreach (var Claim in ClaimI)
                            {
                                if (oNV.sValue != xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())
                                {
                                    var sClaimName = Claim.AttributeI("sName").sResolvedValue;
                                    var sFault = Claim.AttributeI("iWhoseFault").sResolvedValue;
                                    var claimCost = Claim.AttributeI("rTotalClaimCost").sValue;
                                    var InjuredCost = Claim.AttributeI("rCostInjured").sValue;
                                    int ClaimYear = 0;
                                    var result = GetAgeFromDOB(Claim.AttributeI("dDate").sValue, CoverStartDate);
                                    if (result.xiStatus == 00 && result.oResult != null)
                                    {
                                        ClaimYear = (int)result.oResult;
                                    }
                                    if(ClaimYear<1)
                                    {
                                        double rCost = 0;
                                        if (double.TryParse(claimCost, out rCost)) { }
                                        if(rCost<1500)
                                        {
                                            oNV1.sValue = "0";
                                        }
                                        else if(rCost >= 1500 && rCost<=2499)
                                        {
                                            oNV1.sValue = "10";
                                        }
                                        else if (rCost >= 2500 && rCost <= 4999)
                                        {
                                            oNV1.sValue = "20";
                                        }
                                        else if (rCost >= 5000 && rCost <= 10000)
                                        {
                                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                        }
                                        else if (rCost > 10000)
                                        {
                                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                        }

                                    }
                                    else if(ClaimYear>=1 && ClaimYear<=3)
                                    {
                                        double rCost = 0;
                                        if (double.TryParse(claimCost, out rCost)) { }
                                        if (rCost < 1500)
                                        {
                                            oNV1.sValue = "0";
                                        }
                                        else if (rCost >= 1500 && rCost <= 2499)
                                        {
                                            oNV1.sValue = "5";
                                        }
                                        else if (rCost >= 2500 && rCost <= 4999)
                                        {
                                            oNV1.sValue = "10";
                                        }
                                        else if (rCost >= 5000 && rCost <= 10000)
                                        {
                                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                        }
                                        else if (rCost > 10000)
                                        {
                                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                        }
                                    }
                                    else if(ClaimYear >= 4 && ClaimYear <= 5)
                                    {
                                        double rCost = 0;
                                        if (double.TryParse(claimCost, out rCost)) { }
                                        if (rCost < 1500)
                                        {
                                            oNV1.sValue = "0";
                                        }
                                        else if (rCost >= 1500 && rCost <= 2499)
                                        {
                                            oNV1.sValue = "0";
                                        }
                                        else if (rCost >= 2500 && rCost <= 4999)
                                        {
                                            oNV1.sValue = "0";
                                        }
                                        else if (rCost >= 5000 && rCost <= 10000)
                                        {
                                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                        }
                                        else if (rCost > 10000)
                                        {
                                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                        }
                                    }
                                   
                                }
                            }

                        }
                        else if(iNoOfClaims==2)
                        {
                            string sFirstClaimType = string.Empty;
                            string sSecondClaimType = string.Empty;int i = 0;
                            int iFirstClaimYear = 0;int iSecondClaimYear = 0;
                            double rFirstClaimCost = 0;double rsecondClaimCost = 0;
                            foreach (var Claim in ClaimI)
                            {
                                if(i==0)
                                {
                                    sFirstClaimType = Claim.AttributeI("sName").sResolvedValue;
                                    var result = GetAgeFromDOB(Claim.AttributeI("dDate").sValue, CoverStartDate);
                                    if (result.xiStatus == 00 && result.oResult != null)
                                    {
                                        iFirstClaimYear = (int)result.oResult;
                                    }
                                    var claimCost = Claim.AttributeI("rTotalClaimCost").sValue;
                                    if (double.TryParse(claimCost, out rFirstClaimCost)) { }
                                }
                                else
                                {
                                    sSecondClaimType = Claim.AttributeI("sName").sResolvedValue;
                                    var result = GetAgeFromDOB(Claim.AttributeI("dDate").sValue, CoverStartDate);
                                    if (result.xiStatus == 00 && result.oResult != null)
                                    {
                                        iSecondClaimYear = (int)result.oResult;
                                    }
                                    var claimCost = Claim.AttributeI("rTotalClaimCost").sValue;
                                    if (double.TryParse(claimCost, out rsecondClaimCost)) { }
                                }
                                i ++;
                            }
                            if(sFirstClaimType.ToLower()==sSecondClaimType.ToLower())
                            {
                                if(iFirstClaimYear<=1 && iSecondClaimYear<=1)
                                {
                                    double rCost = 0;
                                    if(rFirstClaimCost>rsecondClaimCost)
                                    {
                                        rCost = rFirstClaimCost;
                                    }
                                    else
                                    {
                                        rCost = rsecondClaimCost;
                                    }
                                    if (rCost < 1500)
                                    {
                                        oNV1.sValue = "0";
                                    }
                                    else if (rCost >= 1500 && rCost <= 2499)
                                    {
                                        oNV1.sValue = "10";
                                    }
                                    else if (rCost >= 2500 && rCost <= 4999)
                                    {
                                        oNV1.sValue = "20";
                                    }
                                    else if (rCost >= 5000 && rCost <= 10000)
                                    {
                                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    }
                                    else if (rCost > 10000)
                                    {
                                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    }
                                }
                                else if (iFirstClaimYear <= 1 || iSecondClaimYear <= 1)
                                {
                                    double rCost = 0;
                                    if (rFirstClaimCost > rsecondClaimCost)
                                    {
                                        rCost = rFirstClaimCost;
                                    }
                                    else
                                    {
                                        rCost = rsecondClaimCost;
                                    }
                                    if (rCost < 1500)
                                    {
                                        oNV1.sValue = "0";
                                    }
                                    else if (rCost >= 1500 && rCost <= 2499)
                                    {
                                        oNV1.sValue = "5";
                                    }
                                    else if (rCost >= 2500 && rCost <= 4999)
                                    {
                                        oNV1.sValue = "10";
                                    }
                                    else if (rCost >= 5000 && rCost <= 10000)
                                    {
                                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    }
                                    else if (rCost > 10000)
                                    {
                                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    }
                                }
                                else if (iFirstClaimYear <= 3 || iSecondClaimYear <= 3)
                                {
                                    double rCost = 0;
                                    if (rFirstClaimCost > rsecondClaimCost)
                                    {
                                        rCost = rFirstClaimCost;
                                    }
                                    else
                                    {
                                        rCost = rsecondClaimCost;
                                    }
                                    if (rCost < 1500)
                                    {
                                        oNV1.sValue = "0";
                                    }
                                    else if (rCost >= 1500 && rCost <= 2499)
                                    {
                                        oNV1.sValue = "0";
                                    }
                                    else if (rCost >= 2500 && rCost <= 4999)
                                    {
                                        oNV1.sValue = "5";
                                    }
                                    else if (rCost >= 5000 && rCost <= 10000)
                                    {
                                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    }
                                    else if (rCost > 10000)
                                    {
                                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    }
                                }
                            }
                            else
                            {
                                if (iFirstClaimYear <= 1 && iSecondClaimYear <= 1)
                                {
                                    double rCost = 0;
                                    if (rFirstClaimCost > rsecondClaimCost)
                                    {
                                        rCost = rFirstClaimCost;
                                    }
                                    else
                                    {
                                        rCost = rsecondClaimCost;
                                    }
                                    if (rCost < 1500)
                                    {
                                        oNV1.sValue = "0";
                                    }
                                    else if (rCost >= 1500 && rCost <= 2499)
                                    {
                                        oNV1.sValue = "10";
                                    }
                                    else if (rCost >= 2500 && rCost <= 4999)
                                    {
                                        oNV1.sValue = "20";
                                    }
                                    else if (rCost >= 5000 && rCost <= 10000)
                                    {
                                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    }
                                    else if (rCost > 10000)
                                    {
                                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    }
                                }
                                else if (iFirstClaimYear <= 1 || iSecondClaimYear <= 1)
                                {
                                    double rCost = 0;
                                    if (rFirstClaimCost > rsecondClaimCost)
                                    {
                                        rCost = rFirstClaimCost;
                                    }
                                    else
                                    {
                                        rCost = rsecondClaimCost;
                                    }
                                    if (rCost < 1500)
                                    {
                                        oNV1.sValue = "0";
                                    }
                                    else if (rCost >= 1500 && rCost <= 2499)
                                    {
                                        oNV1.sValue = "5";
                                    }
                                    else if (rCost >= 2500 && rCost <= 4999)
                                    {
                                        oNV1.sValue = "30";
                                    }
                                    else if (rCost >= 5000 && rCost <= 10000)
                                    {
                                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    }
                                    else if (rCost > 10000)
                                    {
                                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    }
                                }
                                else if (iFirstClaimYear <= 3 || iSecondClaimYear <= 3)
                                {
                                    double rCost = 0;
                                    if (rFirstClaimCost > rsecondClaimCost)
                                    {
                                        rCost = rFirstClaimCost;
                                    }
                                    else
                                    {
                                        rCost = rsecondClaimCost;
                                    }
                                    if (rCost < 1500)
                                    {
                                        oNV1.sValue = "0";
                                    }
                                    else if (rCost >= 1500 && rCost <= 2499)
                                    {
                                        oNV1.sValue = "0";
                                    }
                                    else if (rCost >= 2500 && rCost <= 4999)
                                    {
                                        oNV1.sValue = "20";
                                    }
                                    else if (rCost >= 5000 && rCost <= 10000)
                                    {
                                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    }
                                    else if (rCost > 10000)
                                    {
                                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetOccupationsLoadFactor(string sOccupation)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0.0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Occupation/Secondary Occupation";

                    string[] DeclineArray = new string[] { "acrobat", "actor", "actor/actress", "actress", "amusement arcade worker", "antique dealer", "antique renovator", "art buyer", "art dealer", "asphalter/roadworker", "bodyguard", "bookmaker", "bricklayer", "broadcaster", "broadcaster - tv/radio", "circus proprietor", "circus worker", "coin dealer", "croupier", "dancer", "dealer", "dealer - general", "dealer - scrap/waste", "diamond dealer", "disc jockey", "entertainer", "exotic dancer", "fairground worker", "floor manager", "footballer", "footballer - semi professional", "furniture dealer", "furniture restorer", "gambler", "gaming club manager", "gaming club proprietor", "gaming club staff - licensed premises", "gaming club staff - unlicensed premises", "golf caddy", "golf club professional", "golf coach", "golfer", "hawker", "horse breeder", "horse dealer", "horse dealer (non sport)", "horse dealer (sport)", "horse trader", "horse trainer", "interviewer", "jeweller", "jockey", "journalist", "journalist - freelance", "journalistic agent", "kissagram person", "landscape gardener", "landworker", "licensee", "magician", "manager - ring sports", "manager - sports", "market trader", "medal dealer", "model", "money broker", "money dealer", "moneylender", "motor dealer", "motor racing driver", "motor racing organiser", "motor trader", "music producer", "musician", "musician - amateur", "musician - classical", "musician - dance band", "musician - pop group", "night club staff", "non professional footballer", "non professional sports coach", "opera singer", "orchestra leader", "orchestral violinist", "playwright", "private detective", "private investigator", "professional apprentice footballer", "professional boxer", "professional cricketer", "professional cyclist", "professional footballer", "professional racing driver", "professional racing motorcyclist", "professional sports coach", "professional sportsperson", "professional wrestler", "promoter", "promoter - entertainments", "promoter - racing", "promoter - ring sports", "promoter - sports", "publican", "publicity manager", "publisher", "publishing manager", "racehorse groom", "racing motorcyclist", "racing organiser", "radio director", "radio presenter", "radio producer", "rally driver", "scrap dealer", "second hand dealer", "semi-professional sportsperson", "show jumper", "showman", "snooker player", "song writer", "sports administrator - other sports", "sports administrator - ring sports", "sports agent", "sports centre attendant", "sports coach", "sports commentator", "sports scout", "sportsman", "sportswoman", "store detective", "street entertainer", "street trader", "student", "student - foreign", "student - living away", "student - living at home", "student nurse", "student nurse - living at home", "student nurse - living away", "student teacher", "student teacher - living at home", "student teacher - living away", "tv announcer", "tv broadcasting technician", "tv editor", "tarot reader/palmistry expert", "television director", "television presenter", "television producer", "trainer - animal", "trainer - greyhound", "trainer - race horse", "travelling showman", "turf accountant", "undergraduate student - living at home", "undergraduate student - living away from home", "unemployed", "ventriloquist", "waste dealer" };
                    sOccupation = sOccupation.ToLower();
                    int pos = Array.IndexOf(DeclineArray, sOccupation);
                    if (pos > -1)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);

                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }

            //main calculation
            public CResult GetCaravanFinalPremium(string sGUID, int iInstanceID, int iUserID, string sProductName, string sVersion, string sSessionID, int iCustomerID, int iQuoteID)
            {

                List<CResult> oGeneralDeclines = new List<CResult>();
                List<string> Info = new List<string>();
                Info.Add("QsInstanceID_" + iInstanceID);
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oIXI = new XIIXI();
                List<CResult> oXiResults = new List<CResult>();
                try
                {
                    double rFinalPremium = 0; int iProposerAge = 0; string sContentsSI = string.Empty; string sEquipmentSI = string.Empty; string sAwningSI = string.Empty;
                    double rCompulsaryExcess = 0; double rVoluntaryExcess = 0;
                    double rBaseLoad = 0;
                    var oQSI = oCache.Get_QsStructureObj(sSessionID, sGUID, "QSInstance_" + iInstanceID + "Caravan NotationStructure");
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    //string sDOB = "26-Aug-1989"; int iNoClaimYears = 5;
                    string sCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;
                    string sSumInsured = ostructureInstance.XIIValue("insureleisurehomeSC").sValue;
                    string sCoverType = ostructureInstance.XIIValue("sTypeofCoverUpdated").sValue;
                    string sYearofManufacture = ostructureInstance.XIIValue("YearOfManufacturePH").sValue;
                    string dtInsuranceCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;
                    string sOccupation = ostructureInstance.XIIValue("OccupationSC").sValue;
                    string sSecondaryOccupation = ostructureInstance.XIIValue("SecondaryOccupationSC").sValue;
                    //List<CNV> oParams = new List<CNV>();
                    //oParams.Add(new CNV { sName = "sUnit", sValue = sUnitType });
                    var sSubList = ostructureInstance.oStructureI("Claim_T");
                   int iNoOfClaims = sSubList.Count();
                    CResult oBaseLoadFactor = new CResult();
                    CResult oCoverLoadFactor = new CResult();
                    var oResult = GetBaseRate(sSumInsured);
                    if (oResult.xiStatus == 0 && oResult.oCollectionResult.Count > 0)
                    {
                        oBaseLoadFactor = oResult;
                    }
                    if (oBaseLoadFactor.oCollectionResult != null)
                    {
                        rBaseLoad = Convert.ToDouble(oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                    }
                    //get postcode loadfactor
                    var oCoverLoadResult = GetCoverLoad(sCoverType);
                    if (oCoverLoadResult.xiStatus == 0 && oCoverLoadResult.oCollectionResult.Count > 0)
                    {
                        oCoverLoadFactor = oCoverLoadResult;
                    }
                    var sPersonalEffects = "0.00"; double dPersonalEffects = 0.00;
                    var sWindSurf = "0.00"; double dWindSurf = 0.00;
                    //var sRoof = "0.00"; double dRoof = 0.00;
                    double rTotal = 0.00;
                    if (double.TryParse(sPersonalEffects, out dPersonalEffects)) { }
                    if (double.TryParse(sWindSurf, out dWindSurf)) { }
                    //if (double.TryParse(sRoof, out dRoof)) { }
                    //total = rBaseLoad + dPersonalPossessions + dPolicyHolder + dRoof;
                    //get claims load
                    CResult ClaimResult = new CResult();
                   var oClaimResult = GetClaimLoadFactor(iNoOfClaims, sSubList, dtInsuranceCoverStartDate);
                    if (oClaimResult.xiStatus == 0 && oClaimResult.oCollectionResult.Count > 0)
                    {
                        ClaimResult = oClaimResult;
                    }
                    if (ClaimResult.xiStatus == 0 && ClaimResult.oCollectionResult != null)
                    {
                        oXiResults.Add(ClaimResult);
                    }
                    //occupations load
                    CResult OccupationResult = new CResult();
                    if (!string.IsNullOrEmpty(sOccupation))
                    {
                        Info.Add("Occupation " + sOccupation);
                        var result = GetOccupationsLoadFactor(sOccupation);
                        if (result.bOK && result.oCollectionResult.Count > 0)
                        {
                            oXiResults.Add(result);
                        }
                    }
                    if (!string.IsNullOrEmpty(sSecondaryOccupation))
                    {
                        Info.Add("Secondary Occupation " + sSecondaryOccupation);
                        var result = GetOccupationsLoadFactor(sSecondaryOccupation);
                        if (result.bOK && result.oCollectionResult.Count > 0)
                        {
                            oXiResults.Add(result);
                        }
                    }
                    if(rTotal<50)
                    {
                        rTotal = 50;
                    }
                    var IPTLoadFactor = new CResult();
                    var oIPTLoadFactor = GetIPTLoadFactor();
                    if (oIPTLoadFactor.xiStatus == 0 && oIPTLoadFactor.oCollectionResult != null)
                    {
                        IPTLoadFactor = oIPTLoadFactor;
                    }
                    double rInterestRate = 0; double IPT = 0;
                    if (IPTLoadFactor.oCollectionResult != null)
                    {
                        rInterestRate = Convert.ToDouble(IPTLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                        IPT = ((Convert.ToDouble(IPTLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * rTotal);

                        var result = BuildCResultObject(IPT.ToString(), "Net IPT", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString());
                        oXiResults.Add(result);
                        rTotal += IPT;
                    }

                    var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
                    int iProductID = 0;
                    string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                    if (int.TryParse(ProductID, out iProductID)) { }
                    var oProductI = oIXI.BOI("Product", ProductID);
                    oProductI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Product");
                    XIIBO oBOI = new XIIBO();
                    XIAPI oXIAPI = new XIAPI();
                    oBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations");
                    Info.Add("RiskFactorsCount:" + oGeneralDeclines.Count);
                    foreach (var item in oGeneralDeclines)
                    {
                        string sMessage = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        Info.Add(sMessage);
                    }
                    if (oXiResults.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())) && oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())
                    {
                        Info.Add("All Load Factors are normal");
                        double rFinalQuote = 0;
                        double rPFAmount = 0;
                        double rMonthlyTotal = 0;
                        double rPaymentCharge = 0; double rInsurerCharge = 0; double rAdmin = 0;
                        if (double.TryParse(oProductI.Attributes["rPaymentCharge"].sValue, out rPaymentCharge)) { }
                        if (double.TryParse(oProductI.Attributes["rInsurerCharge"].sValue, out rInsurerCharge)) { }
                        if (double.TryParse(oProductI.Attributes["zDefaultAdmin"].sValue, out rAdmin)) { }
                        rFinalQuote = rTotal + rPaymentCharge + rInsurerCharge + rAdmin;
                        oBOI.Attributes["rInterestAmount"] = new XIIAttribute { sName = "rInterestAmount", sValue = String.Format("{0:0.00}", IPT), bDirty = true };
                        oBOI.Attributes["rInterestRate"] = new XIIAttribute { sName = "rInterestRate", sValue = String.Format("{0:0.00}", rInterestRate), bDirty = true };
                        oBOI.Attributes["rPaymentCharge"] = new XIIAttribute { sName = "rPaymentCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rPaymentCharge"].sValue), bDirty = true };
                        oBOI.Attributes["rInsurerCharge"] = new XIIAttribute { sName = "rInsurerCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rInsurerCharge"].sValue), bDirty = true };
                        var MinimumDeposit = oXIAPI.GetMinimumDepostAmount(rPaymentCharge, rInsurerCharge, rFinalQuote, rAdmin, sGUID, iInstanceID, iProductID, 0, 0);
                        double rMinDeposit = 0;
                        if (double.TryParse(MinimumDeposit, out rMinDeposit)) { }
                        oBOI.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = String.Format("{0:0.00}", rTotal), bDirty = true };
                        oBOI.Attributes["rQuotePremium"] = new XIIAttribute { sName = "rQuotePremium", sValue = String.Format("{0:0.00}", rTotal), bDirty = true };
                        oBOI.Attributes["rGrossPremium"] = new XIIAttribute { sName = "rGrossPremium", sValue = String.Format("{0:0.00}", rTotal), bDirty = true };
                        oBOI.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = String.Format("{0:0.00}", rFinalQuote), bDirty = true };
                        oBOI.Attributes["zDefaultDeposit"] = new XIIAttribute { sName = "zDefaultDeposit", sValue = String.Format("{0:0.00}", MinimumDeposit), bDirty = true };
                        oBOI.Attributes["zDefaultAdmin"] = new XIIAttribute { sName = "zDefaultAdmin", sValue = String.Format("{0:0.00}", oProductI.Attributes["zDefaultAdmin"].sValue), bDirty = true };
                        var MonthlyAmount = oXIAPI.GetMonthlyPremiumAmount(rFinalQuote, rMinDeposit, iProductID, 0, 0);
                        Info.Add("Monthly Amount:" + MonthlyAmount);
                        rMonthlyTotal = (MonthlyAmount * 10) + rMinDeposit;
                        oBOI.Attributes["rMonthlyPrice"] = new XIIAttribute { sName = "rMonthlyPrice", sValue = String.Format("{0:0.00}", MonthlyAmount), bDirty = true };
                        oBOI.Attributes["rMonthlyTotal"] = new XIIAttribute { sName = "rMonthlyTotal", sValue = String.Format("{0:0.00}", rMonthlyTotal), bDirty = true };
                        rPFAmount = rMonthlyTotal - rMinDeposit;
                        oBOI.Attributes["rPremiumFinanceAmount"] = new XIIAttribute { sName = "rPremiumFinanceAmount", sValue = String.Format("{0:0.00}", rPFAmount), bDirty = true };
                        oBOI.Attributes["bIsCoverAbroad"] = new XIIAttribute { sName = "bIsCoverAbroad", sValue = oProductI.Attributes["bIsCoverAbroad"].sValue, bDirty = true };
                        string ExcessContent = "<table class=\"table\">";
                        ExcessContent += "<tr>";
                        ExcessContent += "<td class=\"text-left\">Compulsory</td>";
                        ExcessContent += "<td class=\"text-right\">£" + rCompulsaryExcess + "";
                        ExcessContent += "</td>";
                        ExcessContent += "</tr>";
                        ExcessContent += "<tr>";
                        ExcessContent += "<td class=\"text-left\">Total</td>";
                        ExcessContent += "<td class=\"text-right\">£" + rCompulsaryExcess + "</td>";
                        ExcessContent += "</tr>";
                        ExcessContent += "</table>";
                        oBOI.Attributes["sExcess"] = new XIIAttribute { sName = "sExcess", sValue = ExcessContent, bDirty = true };
                        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "0", bDirty = true };
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())) && oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())
                    {
                        Info.Add("Some Load Factors are Declined");
                        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "20", bDirty = true };
                        oBOI.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = "0.00", bDirty = true };
                        oBOI.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = "0.00", bDirty = true };
                        oBOI.Attributes["zDefaultDeposit"] = new XIIAttribute { sName = "zDefaultDeposit", sValue = "0.00", bDirty = true };
                        oBOI.Attributes["zDefaultAdmin"] = new XIIAttribute { sName = "zDefaultAdmin", sValue = "0.00", bDirty = true };
                        oBOI.Attributes["rMonthlyPrice"] = new XIIAttribute { sName = "rMonthlyPrice", sValue = "0.00", bDirty = true };
                    }
                    var sQuoteGUID = Guid.NewGuid().ToString("N").Substring(0, 10);
                    var oSource = oIXI.BOI("XISource_T", ostructureInstance.Attributes["fkisourceid"].sValue);
                    string sPrefix = string.Empty;
                    if (oSource.Attributes != null && oSource.Attributes.ContainsKey("sprefixcode"))
                    {
                        sPrefix = oSource.Attributes["sprefixcode"].sValue;
                    }
                    var iBatchID = iCustomerID.ToString() + iInstanceID.ToString();
                    oBOI.Attributes["sRegNo"] = new XIIAttribute { sName = "sRegNo", sValue = ostructureInstance.XIIValue("sRegNo").sValue, bDirty = true };
                    oBOI.Attributes["dCoverStart"] = new XIIAttribute { sName = "dCoverStart", sValue = sCoverStartDate, bDirty = true };
                    oBOI.Attributes["sCaravanMake"] = new XIIAttribute { sName = "sCaravanMake", sValue = ostructureInstance.XIIValue("sCaravanMakeUpdated").sDerivedValue, bDirty = true };
                    oBOI.Attributes["sCaravanModel"] = new XIIAttribute { sName = "sCaravanModel", sValue = ostructureInstance.XIIValue("sModelofCaravanUpdated").sValue, bDirty = true };


                    oBOI.Attributes["FKiQSInstanceID"] = new XIIAttribute { sName = "FKiQSInstanceID", sValue = iInstanceID.ToString(), bDirty = true };
                    oBOI.Attributes["sInsurer"] = new XIIAttribute { sName = "sInsurer", sValue = oProductI.Attributes["sName"].sValue, bDirty = true };
                    oBOI.Attributes["FKiCustomerID"] = new XIIAttribute { sName = "FKiCustomerID", sValue = iCustomerID.ToString(), bDirty = true };
                    oBOI.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                    oBOI.Attributes["dtqsupdateddate"] = new XIIAttribute { sName = "dtqsupdateddate", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBOI.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBOI.Attributes["FKiProductVersionID"] = new XIIAttribute { sName = "FKiProductVersionID", sValue = sVersion, bDirty = true };
                    oBOI.Attributes["sGUID"] = new XIIAttribute { sName = "sGUID", sValue = sQuoteGUID, bDirty = true };
                    Random generator = new Random();
                    string sRef = generator.Next(1, 10000000).ToString(new String(''0'', 7));
                    oBOI.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + sRef, bDirty = true };
                    //oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + Guid.NewGuid().ToString("N").Substring(0, 6), bDirty = true };
                    //oBOI.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = Guid.NewGuid().ToString("N").Substring(0, 6), bDirty = true };
                    oBOI.Attributes["BatchID"] = new XIIAttribute { sName = "BatchID", sValue = iBatchID, bDirty = true };
                    oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = iQuoteID.ToString(), bDirty = true };
                    oBOI.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = String.Format("{0:0.00}", rCompulsaryExcess), bDirty = true };
                    oBOI.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = String.Format("{0:0.00}", rVoluntaryExcess), bDirty = true };
                    double rTotalExcess = rCompulsaryExcess + rVoluntaryExcess;
                    oBOI.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = String.Format("{0:0.00}", rTotalExcess), bDirty = true };
                    //Info.Add("QuoteRefID_" + oBOI.Attributes["sRefID"].sValue);
                    var oRes = oBOI.Save(oBOI);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        oBOI = (XIIBO)oRes.oResult;
                    }
                    Info.Add(oProductI.Attributes["sName"].sValue + "Quote inserted Sucessfully with the amount of " + rFinalPremium);
                    XIDBO oRiskFactorsBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
                    XIIBO oBO = new XIIBO();
                    oBO.BOD = oRiskFactorsBOD;
                    List<XIIBO> oBOIList = new List<XIIBO>();
                    foreach (var item in oXiResults)
                    {
                        oBO = new XIIBO();
                        oBO.BOD = oRiskFactorsBOD;
                        oBO.Attributes["FKiQuoteID"] = new XIIAttribute { sName = "FKiQuoteID", sValue = oBOI.Attributes["ID"].sValue, bDirty = true };
                        oBO.Attributes["sFactorName"] = new XIIAttribute { sName = "sFactorName", sValue = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                        oBO.Attributes["sValue"] = new XIIAttribute { sName = "sValue", sValue = item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                        oBO.Attributes["sMessage"] = new XIIAttribute { sName = "sMessage", sValue = item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                        oBO.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                        oBO.Attributes["FKsQuoteID"] = new XIIAttribute { sName = "FKsQuoteID", sValue = sQuoteGUID, bDirty = true };
                        oBO.Attributes["ID"] = new XIIAttribute { sName = "ID", bDirty = true };
                        oBOIList.Add(oBO);
                    }
                    XIIBO xibulk = new XIIBO();
                    DataTable dtbulk = xibulk.MakeBulkSqlTable(oBOIList);
                    xibulk.SaveBulk(dtbulk, oBOIList[0].BOD.iDataSource, oBOIList[0].BOD.TableName);
                    oCResult.oCollectionResult.Add(new CNV { sName = "QuoteID", sValue = oBOI.Attributes["ID"].sValue });
                    oCResult.sMessage = "Success";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = "Success";
                    string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                    oCResult.sMessage = sInfo;
                    oIB.SaveErrortoDB(oCResult);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = rFinalPremium;
                }
                catch (Exception ex)
                {
                    string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                    oCResult.sMessage = sInfo;
                    oIB.SaveErrortoDB(oCResult);
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.oResult = "Error";
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
        }',
 N'Prepersist',
 N'C# Code',
 N'0',
 N'2019-08-24 15:53:54.000',
 N'6',
 N'169.254.204.77',
 N'174',
 N'169.254.204.77',
 N'2019-08-24 15:53:54.000',
 N'15',
 N'SCTowerGateCalculation',
 NULL,
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
 N'0da55dbf-008b-4124-a9f8-40c3b34186fd',
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
