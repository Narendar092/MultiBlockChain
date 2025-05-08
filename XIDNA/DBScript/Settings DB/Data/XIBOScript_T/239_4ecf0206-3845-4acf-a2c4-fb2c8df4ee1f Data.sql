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
 [FKiVersionIDXIGUID]) Values(N'239',
 N'296',
 N'0',
 NULL,
 N'SC Jackson Lee',
 N'Policy calculation',
 N'public static CResult CaravanCalculation(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            StaticCaravan oTCaravan = new StaticCaravan();
            //XIIQS oQsInstance = new XIIQS();
            CResult oResult = new CResult();
            oResult.sMessage = "Towergate Version1 script running";
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
                oResult = oTCaravan.GetCaravanFinalPremium(sGUID, iInstanceID, iUserID, sProductName, sVersion, sSessionID, iCustomerID, iQuoteID);
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
        public class StaticCaravan
        {
            public static double CaravanSI = 0;
            public static double ContentsSI = 0;
            public static double IPTRate = 12;
            List<CResult> oGeneralRefers = new List<CResult>();
            //Cover load
            public CResult GetCover(string CoverType, double AmountCovered) //Should be %
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Type of Cover";
                    switch (CoverType.ToLower())
                    {
                        case "market value":
                            {
                                if (AmountCovered < 20000)
                                {
                                    oNV1.sValue = "0.65";
                                    sResult = "0.65";
                                }
                                else if (AmountCovered >= 20000 && AmountCovered < 30000)
                                {
                                    oNV1.sValue = "0.50";
                                    sResult = "0.50";
                                }
                                else
                                {
                                    oNV1.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    sResult = "0.50";
                                }

                                break;
                            }
                        case "new for old":
                            {
                                if (AmountCovered >= 30000 && AmountCovered < 60000)
                                {
                                    oNV1.sValue = "0.47";
                                    sResult = "0.47";
                                }
                                else if (AmountCovered >= 60000 && AmountCovered < 100000)
                                {
                                    oNV1.sValue = "0.38";
                                    sResult = "0.38";
                                }
                                else
                                {
                                    oNV1.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    sResult = "0.47";
                                }
                                break;
                            }
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            //Claims load
            public CResult GetNoClaimDiscount(int iNoClaims) //Shoud be %
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Claims";
                    switch (iNoClaims.ToString())
                    {
                        case "0":
                            oNV1.sValue = "0";
                            sResult = "0";
                            break;
                        case "1":
                            oNV1.sValue = "10";
                            sResult = "10";
                            break;
                        case "2":
                            oNV1.sValue = "25";
                            sResult = "25";
                            break;
                        default:
                            oNV1.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                            sResult = "0";
                            break;
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
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
            public CResult GetFinalPremium(List<CNV> oParams)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    CNV oNV = new CNV();
                    oNV.sName = "rInterestAmount";
                    double dPolicyGrossPremium = 0; double dIPTAmount = 0;
                    var CoverLoad = oParams.Where(x => x.sName == "iCover").Select(x => x.sValue).FirstOrDefault();
                    var ClaimLoad = oParams.Where(x => x.sName == "iClaimLoad").Select(x => x.sValue).FirstOrDefault();
                    var BasePrice = (CaravanSI + ContentsSI) * (Convert.ToDouble(CoverLoad) / 100);
                    var Total = BasePrice + 10;
                    dPolicyGrossPremium = Total + (Total * ((ClaimLoad == "0" || ClaimLoad == "" || ClaimLoad.ToLower() == "decline") ? 0 : Convert.ToInt32(ClaimLoad)) / 100); //Claims load Not Applied

                    dIPTAmount = dPolicyGrossPremium * (IPTRate / 100);
                    oNV.sValue = dIPTAmount.ToString();
                    double dFinalPremium = dPolicyGrossPremium + dIPTAmount;
                    dFinalPremium = Convert.ToDouble(String.Format("{0:0.00}", dFinalPremium));
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oResult = dFinalPremium;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            //mainCalculation
            public CResult GetCaravanFinalPremium(string sGUID, int iInstanceID, int iUserID, string sProductName, string sVersion, string sSessionID, int iCustomerID, int iQuoteID)
            {
                List<string> Info = new List<string>();
                Info.Add("QsInstanceID_" + iInstanceID);
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oIXI = new XIIXI();

                try
                {
                    double rFinalPremium = 0; string sContentsSI = string.Empty;
                    double rCompulsaryExcess = 0; double rVoluntaryExcess = 0; string sInterestAmount = string.Empty;
                    string sCaravanSI = string.Empty;
                    double TotalInsured = 0;
                    var oQSI = oCache.Get_QsStructureObj(sSessionID, sGUID, "QSInstance_" + iInstanceID + "StaticCaravan Quotes Structure");
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    //string sDOB = "26-Aug-1989"; int iNoClaimYears = 5;

                    int NoOfClaims = 0;
                    var result = new CResult();
                    string sCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;

                    sCaravanSI = ostructureInstance.XIIValue("staticcoveredSC").sValue;
                    if (double.TryParse(sCaravanSI, out CaravanSI)) { }
                    if (CaravanSI > 65000)
                    {
                        oGeneralRefers.Add(BuildCResultObject(CaravanSI.ToString(), "Caravan Value", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject(CaravanSI.ToString(), "Caravan Value", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }
                    sContentsSI = ostructureInstance.XIIValue("insureleisurehomeSC").sValue;
                    if (double.TryParse(sContentsSI, out ContentsSI)) { }
                    if (ContentsSI > 3000)
                    {
                        oGeneralRefers.Add(BuildCResultObject(ContentsSI.ToString(), "Contents Value", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject(ContentsSI.ToString(), "Contents Value", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }

                    TotalInsured = CaravanSI + ContentsSI;

                    //cover
                    var CoverLoad = ""; string TypeOfCover = "";
                    TypeOfCover = ostructureInstance.XIIValue("sTypeofCoverUpdated").sResolvedValue;
                    result = GetCover(TypeOfCover, TotalInsured);
                    CoverLoad = (string)result.oResult;

                    if (result.oCollectionResult.Where(x => x.sName == "oResult").Select(t => t.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())
                    {
                        oGeneralRefers.Add(BuildCResultObject(CoverLoad.ToString(), "Basis of Cover", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject(CoverLoad.ToString(), "Basis of Cover", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }

                    //claims
                    var ClaimLoad = "";
                    var oClaimLIst = oQSI.oSubStructureI("Claim_T");
                    if (oClaimLIst.oBOIList != null)
                    {
                        var oClaimI = (List<XIIBO>)oClaimLIst.oBOIList.Where(x => Convert.ToInt32(x.AttributeI("ddate").GetDateDifference()) < 5).ToList();
                        NoOfClaims = oClaimI == null ? 0 : oClaimI.Count();
                    }
                    if (NoOfClaims > 0)
                    {
                        result = GetNoClaimDiscount(NoOfClaims);
                        if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                        {
                            ClaimLoad = (string)result.oResult;
                            if (result.oCollectionResult.Where(x => x.sName == "oResult").Select(t => t.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())
                            {
                                oGeneralRefers.Add(BuildCResultObject(ClaimLoad.ToString(), "Claims", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                                oGeneralRefers.Add(BuildCResultObject(NoOfClaims.ToString(), "Claims count", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                            }
                            else
                            {
                                oGeneralRefers.Add(BuildCResultObject(ClaimLoad.ToString(), "Claims", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                            }
                        }
                    }
                    //General Declines
                    string sPreviousMotorOrCaravanInsurancePolicyDetails = ostructureInstance.XIIValue("PreviousMotorOrCaravanInsurancePolicyDetailsSC").sResolvedValue;
                    string sbankruptcy = ostructureInstance.XIIValue("bankruptcySC").sResolvedValue;
                    string ssMotorOffence = ostructureInstance.XIIValue("sMotorOffenceSC").sResolvedValue;
                    if (!string.IsNullOrEmpty(sPreviousMotorOrCaravanInsurancePolicyDetails) && sPreviousMotorOrCaravanInsurancePolicyDetails.ToLower() == "yes")
                    {
                        oGeneralRefers.Add(BuildCResultObject(sPreviousMotorOrCaravanInsurancePolicyDetails, "Refused or canceled insurance", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    if (!string.IsNullOrEmpty(sbankruptcy) && sbankruptcy.ToLower() == "yes")
                    {
                        oGeneralRefers.Add(BuildCResultObject(sbankruptcy, "Bankruptcy", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    if (!string.IsNullOrEmpty(ssMotorOffence) && ssMotorOffence.ToLower() == "yes")
                    {
                        oGeneralRefers.Add(BuildCResultObject(ssMotorOffence, "Non-motoring offence", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    string sVoluntaryExcess = ostructureInstance.XIIValue("VoluntaryExcessTC").sValue;
                    if (sVoluntaryExcess == "10")
                    {
                        sVoluntaryExcess = "0";
                    }
                    if (double.TryParse(sVoluntaryExcess, out rVoluntaryExcess)) { }
                    List<CNV> oParams = new List<CNV>();
                    oParams.Add(new CNV { sName = "iClaimLoad", sValue = ClaimLoad });
                    oParams.Add(new CNV { sName = "iCover", sValue = CoverLoad });

                    var oFinalPremium = GetFinalPremium(oParams);
                    if (oFinalPremium.bOK && oFinalPremium.oResult != null)
                    {
                        rFinalPremium = (double)oFinalPremium.oResult;
                    }
                    if (oFinalPremium.oCollectionResult != null && oFinalPremium.oCollectionResult.Count() > 0)
                    {
                        sInterestAmount = oFinalPremium.oCollectionResult.Where(m => m.sName == "rInterestAmount").Select(m => m.sValue).FirstOrDefault();
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
                    Info.Add("RiskFactorsCount:" + oGeneralRefers.Count);
                    foreach (var item in oGeneralRefers)
                    {
                        string sMessage = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        Info.Add(sMessage);
                    }
                    //if (oGeneralRefers != null && oGeneralRefers.Count() > 0 && oGeneralRefers.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())))
                    //{
                    //Info.Add("All Load Factors are normal");
                    double rFinalQuote = 0;
                    double rPFAmount = 0;
                    double rMonthlyTotal = 0;
                    double rPaymentCharge = 0; double rInsurerCharge = 0; double rAdmin = 0;
                    if (double.TryParse(oProductI.Attributes["rPaymentCharge"].sValue, out rPaymentCharge)) { }
                    if (double.TryParse(oProductI.Attributes["rInsurerCharge"].sValue, out rInsurerCharge)) { }
                    if (double.TryParse(oProductI.Attributes["zDefaultAdmin"].sValue, out rAdmin)) { }
                    rFinalQuote = rFinalPremium + rPaymentCharge + rInsurerCharge + rAdmin;
                    double rInterestAmount = 0;
                    if (double.TryParse(sInterestAmount, out rInterestAmount)) { }
                    oBOI.Attributes["rInterestAmount"] = new XIIAttribute { sName = "rInterestAmount", sValue = String.Format("{0:0.00}", rInterestAmount), bDirty = true };
                    oBOI.Attributes["rInterestRate"] = new XIIAttribute { sName = "rInterestRate", sValue = String.Format("{0:0.00}", IPTRate), bDirty = true };
                    oBOI.Attributes["rPaymentCharge"] = new XIIAttribute { sName = "rPaymentCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rPaymentCharge"].sValue), bDirty = true };
                    oBOI.Attributes["rInsurerCharge"] = new XIIAttribute { sName = "rInsurerCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rInsurerCharge"].sValue), bDirty = true };
                    var MinimumDeposit = oXIAPI.GetMinimumDepostAmount(rPaymentCharge, rInsurerCharge, rFinalQuote, rAdmin, sGUID, iInstanceID, iProductID, 0, 0);
                    double rMinDeposit = 0;
                    if (double.TryParse(MinimumDeposit, out rMinDeposit)) { }
                    oBOI.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = String.Format("{0:0.00}", rFinalPremium), bDirty = true };
                    oBOI.Attributes["rDefaultQuotePrice"] = new XIIAttribute { sName = "rDefaultQuotePrice", sValue = String.Format("{0:0.00}", rFinalPremium), bDirty = true };
                    oBOI.Attributes["rQuotePremium"] = new XIIAttribute { sName = "rQuotePremium", sValue = String.Format("{0:0.00}", rFinalPremium), bDirty = true };
                    oBOI.Attributes["rGrossPremium"] = new XIIAttribute { sName = "rGrossPremium", sValue = String.Format("{0:0.00}", rFinalPremium), bDirty = true };
                    oBOI.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = String.Format("{0:0.00}", rFinalQuote), bDirty = true };
                    oBOI.Attributes["zDefaultDeposit"] = new XIIAttribute { sName = "zDefaultDeposit", sValue = String.Format("{0:0.00}", MinimumDeposit), bDirty = true };
                    oBOI.Attributes["zDefaultAdmin"] = new XIIAttribute { sName = "zDefaultAdmin", sValue = String.Format("{0:0.00}", oProductI.Attributes["zDefaultAdmin"].sValue), bDirty = true };
                    var PFSchemeID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iPFSchemeID}");
                    int iPFSchemeID = 0;
                    if (int.TryParse(PFSchemeID, out iPFSchemeID))
                    { }
                    var MonthlyAmount = oXIAPI.GetMonthlyPremiumAmount(rFinalQuote, rMinDeposit, iProductID, 0, 0, iPFSchemeID);
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
                    if (oGeneralRefers != null && oGeneralRefers.Count() > 0 && oGeneralRefers.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())))
                    {
                        Info.Add("All Load Factors are normal");
                        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "0", bDirty = true };
                        oBOI.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "false", bDirty = true };
                    }
                    else if (oGeneralRefers != null && oGeneralRefers.Count() > 0 && oGeneralRefers.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())))
                    {
                        Info.Add("Some Load Factors are Refered");
                        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "10", bDirty = true };
                        oBOI.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "true", bDirty = true };
                    }
                    else if (oGeneralRefers != null && oGeneralRefers.Count() > 0 && oGeneralRefers.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())))
                    {
                        Info.Add("some Load Factors are Declined");
                        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "20", bDirty = true };
                        oBOI.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "true", bDirty = true };
                    }
                    //{
                    //    Info.Add("Some Load Factors are Refered");
                    //    oBOI.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "10", bDirty = true };
                    //    oBOI.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = "0.00", bDirty = true };
                    //    oBOI.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = "0.00", bDirty = true };
                    //    oBOI.Attributes["zDefaultDeposit"] = new XIIAttribute { sName = "zDefaultDeposit", sValue = "0.00", bDirty = true };
                    //    oBOI.Attributes["zDefaultAdmin"] = new XIIAttribute { sName = "zDefaultAdmin", sValue = "0.00", bDirty = true };
                    //    oBOI.Attributes["rMonthlyPrice"] = new XIIAttribute { sName = "rMonthlyPrice", sValue = "0.00", bDirty = true };
                    //}
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
                    oBOI.Attributes["sCaravanMake"] = new XIIAttribute { sName = "sCaravanMake", sValue = ostructureInstance.XIIValue("CaravanMakeTC").sDerivedValue, bDirty = true };
                    oBOI.Attributes["sCaravanModel"] = new XIIAttribute { sName = "sCaravanModel", sValue = ostructureInstance.XIIValue("ModelofCaravanTC").sValue, bDirty = true };


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
                    oBOI.Attributes["bIsVisibleToUser"] = new XIIAttribute { sName = "bIsVisibleToUser", sValue = "false", bDirty = true };
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
                    foreach (var item in oGeneralRefers)
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
 N'2020-07-30 11:15:36.000',
 N'183',
 N'169.254.204.77',
 N'183',
 N'169.254.204.77',
 N'2020-07-30 11:15:36.000',
 N'15',
 N'CaravanCalculation',
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
 N'4ecf0206-3845-4acf-a2c4-fb2c8df4ee1f',
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
