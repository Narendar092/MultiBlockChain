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
 [FKiVersionIDXIGUID]) Values(N'310',
 N'296',
 N'0',
 NULL,
 N'SCKGMCalculationMarch2020',
 N'PolicyCalculation',
 N'        public static CResult PolicyMainCal(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            PolicyCalculation Pcal = new PolicyCalculation();
            //XIIQS oQsInstance = new XIIQS();
            CResult oResult = new CResult();
            CNV oNV = new CNV();
            oNV.sName = "sCode";
            try
            {
                string sUID = lParam.Where(m => m.sName == "sUID").FirstOrDefault().sValue;
                int iInsatnceID = Convert.ToInt32(lParam.Where(m => m.sName == "iInsatnceID").FirstOrDefault().sValue);
                int iUserID = Convert.ToInt32(lParam.Where(m => m.sName == "iUserID").FirstOrDefault().sValue);
                int iCustomerID = Convert.ToInt32(lParam.Where(m => m.sName == "iCustomerID").FirstOrDefault().sValue);
                string sDataBase = lParam.Where(m => m.sName == "sDataBase").FirstOrDefault().sValue;
                string sProductName = lParam.Where(m => m.sName == "ProductName").FirstOrDefault().sValue;
                string sVersion = lParam.Where(m => m.sName == "Version").FirstOrDefault().sValue;
                string sSessionID = lParam.Where(m => m.sName == "sSessionID").FirstOrDefault().sValue;
                string sProductCode = lParam.Where(m => m.sName == "ProductCode").FirstOrDefault().sValue;
                int iQuoteID = Convert.ToInt32(lParam.Where(m => m.sName == "iQuoteID").FirstOrDefault().sValue);
                oResult = Pcal.Calculation(iInsatnceID, iUserID, iCustomerID, sDataBase, sProductName, sVersion, sProductCode, sSessionID, sUID, iQuoteID);
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
            public CResult GetBaseRate(double rSumInsuerd, double rTotalSumInsured)
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
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Base Rate";
                    double rValue = 0;
                    if (rSumInsuerd <= 12500)
                    {
                        rValue = (1.15 * 0.01) * (rTotalSumInsured);
                    }
                    else if (rSumInsuerd > 12500 && rSumInsuerd < 20000)
                    {
                        rValue = (0.85 * 0.01) * (rTotalSumInsured);
                    }
                    else if (rSumInsuerd >= 20000 && rSumInsuerd < 30000)
                    {
                        rValue = (0.54 * 0.01) * (rTotalSumInsured);
                    }
                    else if (rSumInsuerd >= 30000 && rSumInsuerd < 40000)
                    {
                        rValue = (0.50 * 0.01) * (rTotalSumInsured);
                    }
                    else if (rSumInsuerd >= 40000 && rSumInsuerd < 50000)
                    {
                        rValue = (0.46 * 0.01) * (rTotalSumInsured);
                    }
                    else if (rSumInsuerd >= 50000 && rSumInsuerd < 60000)
                    {
                        rValue = (0.41 * 0.01) * (rTotalSumInsured);
                    }
                    else if (rSumInsuerd >= 60000 && rSumInsuerd <= 100000)
                    {
                        rValue = (0.31 * 0.01) * (rTotalSumInsured);
                    }
                    else if (rSumInsuerd >= 100001 && rSumInsuerd <= 120000)
                    {
                        rValue = (0.252 * 0.01) * (rTotalSumInsured);
                    }
                    else
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
                    }
                    oNV1.sValue = rValue.ToString();
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    //oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult, iInstanceID);
                }
                return oresult;
            }
            public CResult GetFloodLoadFactor(string sPostCode, int iProductID)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                XIAPI oXIAPI = new XIAPI();
                try
                {
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Flood Rate ";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";

                    List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                    oWHParams.Add(new XIWhereParams { sField = "sMPostCode", sOperator = "=", sValue = sPostCode });
                    oWHParams.Add(new XIWhereParams { sField = "FkiProductID", sOperator = "=", sValue = iProductID.ToString() });
                    var oArea = oXIAPI.GetValue("PostCodeLookUps_T", "Group", oWHParams);
                    if (!string.IsNullOrEmpty(oArea))
                    {
                        oNV1.sValue = "+75";
                    }

                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult, iInstanceID);
                }
                return oresult;
            }
            public CResult GetClaimDiscount(int iNoOfClaimWithinLastThreeYrs, double rBaseRate)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Claim Discount";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (iNoOfClaimWithinLastThreeYrs == 0 && rBaseRate >= 117)
                    {
                        oNV1.sValue = "-10";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult, iInstanceID);
                }
                return oresult;
            }
            public CResult GetYearOfManufactureDiscount(int iVehicleAge)
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
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Year of Manufacture Discount";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (iVehicleAge <= 10)
                    {
                        oNV1.sValue = "-7.5";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult, iInstanceID);
                }
                return oresult;
            }
            public CResult GetPolicyExcessLoad(string iExcess)
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
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Policy Excess";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (iExcess == "150")
                    {
                        oNV1.sValue = "-5";
                    }
                    else if (iExcess == "250")
                    {
                        oNV1.sValue = "-10";
                    }
                    else if (iExcess == "500")
                    {
                        oNV1.sValue = "-15";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult, iInstanceID);
                }
                return oresult;
            }
            public CResult GetClaimLoadFactor(int iClaimAge)
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
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Claim Load";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (iClaimAge < 1)
                    {
                        oNV1.sValue = "+25";
                    }
                    else if (iClaimAge >= 1 && iClaimAge < 2)
                    {
                        oNV1.sValue = "+20";
                    }
                    else if (iClaimAge >= 2 && iClaimAge < 3)
                    {
                        oNV1.sValue = "+15";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult, iInstanceID);
                }
                return oresult;
            }
            public CResult GetHireLoadFactor(string sHire)
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
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Hire Load";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Value";
                    if (!string.IsNullOrEmpty(sHire) && sHire.ToLower() == "yes")
                    {
                        oNV1.sValue = "+12";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult, iInstanceID);
                }
                return oresult;
            }




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
            public CResult GetMaxLoadFactor(List<CResult> oResult)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult result = new CResult();
                try
                {
                    result = oResult.FirstOrDefault();

                    if (oResult.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())))
                    {
                        result = oResult.Where(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Select(x => x.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()).FirstOrDefault();

                    }
                    else if (oResult.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Select(x => x.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()))
                    {
                        result = oResult.Where(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Select(x => x.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()).FirstOrDefault();
                    }
                    else
                    {
                        foreach (var item in oResult)
                        {
                            if (Convert.ToDouble(result.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) < Convert.ToDouble(item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()))
                                result = item;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.sMessage = "ERROR: [" + result.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    result.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    result.LogToFile();
                    oIB.SaveErrortoDB(result, iInstanceID);
                }
                return result;
            }
            public CResult GetOccupationsLoadFactor(string sOccupation)
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
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Occupation/Secondary Occupation";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";

                    sOccupation = sOccupation.ToLower();

                    string[] ReferArray = new string[] { "horse trader", "landlady or landlord", "landworker", "mobile caterer", "public house manager", "publican", "scrap dealer", "street entertainer", "street trader", "tattooist" };
                    string[] DeclineArray = new string[] { "circus proprietor", "circus worker", "fairground worker", "hawker", "tarot reader/palmistry expert" };
                    int pos = Array.IndexOf(DeclineArray, sOccupation);
                    if (pos > -1)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    else
                    {
                        pos = Array.IndexOf(ReferArray, sOccupation);
                        if (pos > -1)
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
                        }
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
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


            public CResult GetMinimumPremium()
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
                    oNV1.sValue = "+75";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Minimum Premium";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Value";
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
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
            public CResult GetLegalExpense()
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
                    oNV1.sValue = "+10";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Legal Expense";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Value";
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
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
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
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
                    oIB.SaveErrortoDB(oresult, iInstanceID);
                }
                return oresult;
            }

        }
        public class PolicyCalculation
        {
            public CResult Calculation(int iInsatnceID, int iUserID, int iCustomerID, string sDataBase, string sProductName, string sVersion, string sProductCode, string sSessionID, string sUID, int iQuoteID)
            {
                List<string> Info = new List<string>();
                Info.Add("[QsInstanceID_" + iInsatnceID + "]");
                Info.Add("KGM SCCaravan script running");
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                try
                {
                    List<CResult> oGeneralDeclines = new List<CResult>();
                    List<CResult> oGeneralRefers = new List<CResult>();
                    PolicyBaseCalc PolicyCal = new PolicyBaseCalc();
                    PolicyCal.iInstanceID = iInsatnceID;
                    List<CResult> oXiResults = new List<CResult>();
                    XIIXI oIXI = new XIIXI();
                    XIIBO oBII = new XIIBO();
                    List<int> oAgesL = new List<int>();
                    List<CResult> OccupationResult = new List<CResult>();
                    string DOB = "";
                    string sOccupation = "";
                    string sSecondaryOccupation = "";
                    XIInfraCache oCache = new XIInfraCache();
                    oBII.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations", null);
                    var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
                    int iProductID = 0;
                    string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                    if (int.TryParse(ProductID, out iProductID)) { }
                    var oProductI = oIXI.BOI("Product", ProductID);
                    List<CResult> ClaimResultList = new List<CResult>();
                    int NoOfClaims = 0;
                    int NoOfClaimsWithinLastThreeYrs = 0;
                    var result = new CResult();
                    var oQSI = oCache.Get_QsStructureObj(sSessionID, sUID, "QSInstance_" + iInsatnceID + "StaticCaravan Quotes Structure");
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    var oLIst = oQSI.oSubStructureI("Driver_T");
                    var oDriversI = (List<XIIBO>)oLIst.oBOIList;
                    int j = 0;
                    double rCompulsaryExcess = 0;
                    double rTotalClaimCost = 0;
                    foreach (var item in oDriversI)
                    {
                        int Age = 0;
                        Info.Add("DriverID_" + item.Attributes["id"].sValue + "_QSInstanceID_" + iInsatnceID);
                        item.oBOStructure = item.SubChildI;
                        DOB = item.AttributeI("dDOB").sValue;
                        var dDOB = "";
                        DateTime dtDOB = DateTime.MinValue;
                        if (DateTime.TryParse(DOB, out dtDOB))
                        {
                            dDOB = dtDOB.ToString("dd/MM/yyyy");
                        }
                        QueryEngine oQE = new QueryEngine();
                        List<XIWhereParams> oWParams = new List<XIWhereParams>();
                        XIWhereParams oWP = new XIWhereParams();
                        oWP.sField = "Name_6";
                        oWP.sOperator = "=";
                        oWP.sValue = item.AttributeI("sName").sValue;
                        oWParams.Add(oWP);
                        oWP = new XIWhereParams();
                        oWP.sField = "Name_1";
                        oWP.sOperator = "=";
                        oWP.sValue = item.AttributeI("sForeName").sValue;
                        oWParams.Add(oWP);
                        oWP = new XIWhereParams();
                        oWP.sField = "DOB";
                        oWP.sOperator = "=";
                        oWP.sValue = dDOB;
                        oWParams.Add(oWP);
                        oWP = new XIWhereParams();
                        oWP.sField = "izXDeleted";
                        oWP.sOperator = "=";
                        oWP.sValue = "0";
                        oWParams.Add(oWP);
                        oQE.AddBO("sanctionsconlist", null, oWParams);
                        CResult oCresult = oQE.BuildQuery();
                        if (oCresult.bOK && oCresult.oResult != null)
                        {
                            var sSql = (string)oCresult.oResult;
                            Info.Add("HMSanction Query_" + sSql);
                            ExecutionEngine oEE = new ExecutionEngine();
                            oEE.XIDataSource = oQE.XIDataSource;
                            oEE.sSQL = sSql;
                            var oQResult = oEE.Execute();
                            if (oQResult.bOK && oQResult.oResult != null)
                            {
                                var oSanctionList = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                                if (oSanctionList.Count > 0)
                                {
                                    oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "H M Sanctions", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                                }
                            }
                        }
                        result = PolicyCal.GetAgeFromDOB(DOB, DateTime.Now.ToString());
                        if (result.xiStatus == 00 && result.oResult != null)
                        {
                            Age = (int)result.oResult;
                            oAgesL.Add(Age);
                        }
                        sOccupation = item.AttributeI("enumOccupatation").sResolvedValue;
                        Info.Add("sOccupation_" + sOccupation);
                        result = PolicyCal.GetOccupationsLoadFactor(sOccupation);
                        if (result.xiStatus == 00 && result.oCollectionResult.Count > 0)
                        {
                            OccupationResult.Add(result);
                        }
                        sSecondaryOccupation = item.AttributeI("sSecondaryOccupation").sResolvedValue;
                        if (!string.IsNullOrEmpty(sSecondaryOccupation))
                        {
                            Info.Add("Secondary Occupation_" + sSecondaryOccupation);
                            result = PolicyCal.GetOccupationsLoadFactor(sSecondaryOccupation);
                            if (result.xiStatus == 00 && result.oCollectionResult.Count > 0)
                            {
                                OccupationResult.Add(result);
                            }
                        }

                        j++;
                    }

                    var oClaimLIst = oQSI.oSubStructureI("Claim_T");
                    var oClaimI = (List<XIIBO>)oClaimLIst.oBOIList;
                    //if (NoOfClaims > 0)
                    //{
                    if (oClaimI != null)
                    {
                        foreach (var claim in oClaimI)
                        {
                            var dt3Years = DateTime.Now.AddYears(-3).Date;
                            var dtClaimDate = Convert.ToDateTime(claim.AttributeI("dDate").sValue);
                            if (dtClaimDate >= dt3Years)
                            {
                                NoOfClaimsWithinLastThreeYrs++;
                            }
                            var claimCost = claim.AttributeI("rTotalClaimCost").sValue;
                            double rClaimCost = 0;
                            double.TryParse(claimCost, out rClaimCost);
                            rTotalClaimCost += rClaimCost;
                            var oClaimAge = PolicyCal.GetAgeFromDOB(claim.AttributeI("dDate").sValue, DateTime.Now.ToString());
                            if (oClaimAge.xiStatus == 00 && oClaimAge.oResult != null)
                            {
                                var iClaimAge = (int)oClaimAge.oResult;
                                var oClaimResult = PolicyCal.GetClaimLoadFactor(iClaimAge);
                                if (oClaimResult.xiStatus == 0 && oClaimResult.oCollectionResult.Count > 0)
                                {
                                    ClaimResultList.Add(oClaimResult);
                                }
                            }
                        }
                    }
                    //}
                    if (oAgesL.Count > 0)
                    {
                        if (oAgesL.Any(m => m < 18))
                        {
                            oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Age of proposer", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                        }
                    }
                    string sSumInsured = ostructureInstance.XIIValue("insureyourSC").sValue;
                    string sSumInsuredContent = ostructureInstance.XIIValue("insureleisurehomeSC").sValue;
                    string sYearOfManufacture = ostructureInstance.XIIValue("leisurehomemanufacturedSC").sDerivedValue;
                    string sPolicyExcess = ostructureInstance.XIIValue("levelofpolicyexcessdoyourequire").sValue;
                    string sHire = ostructureInstance.XIIValue("LetouthireSC").sDerivedValue;
                    string sMake = ostructureInstance.XIIValue("leisuremakeSC").sDerivedValue;
                    string sNatureOfSite = ostructureInstance.XIIValue("NatureofSiteSC").sDerivedValue;
                    string sStaticLoss = ostructureInstance.XIIValue("staticlossSC").sDerivedValue;
                    string sAdditionalProposer = ostructureInstance.XIIValue("AdditionalproposerSC").sDerivedValue;
                    string sMotorOffence = ostructureInstance.XIIValue("sMotorOffenceSC").sResolvedValue;
                    string sBankruptcy = ostructureInstance.XIIValue("bankruptcySC").sResolvedValue;
                    string sPersonaleffects = ostructureInstance.XIIValue("PersonaleffectsSC").sDerivedValue;
                    string sPostCode = ostructureInstance.XIIValue("sPostCode").sResolvedValue;
                    string sInsuredPolicyDetails = ostructureInstance.XIIValue("PreviousMotorOrCaravanInsurancePolicyDetailsSC").sResolvedValue;
                    string dtInsuranceCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;
                    string sTypeOfCovered = ostructureInstance.XIIValue("sTypeofCoverUpdated").sDerivedValue;
                    string spermanentresidentUK = ostructureInstance.XIIValue("permanentresidentUKSC").sValue;
                    double rSumInsuerd = 0;
                    double rTotalSumInsured = 0;
                    double.TryParse(sSumInsured, out rSumInsuerd);
                    double rSumInsuerdContent = 0;
                    double.TryParse(sSumInsuredContent, out rSumInsuerdContent);
                    Info.Add("Make_" + sMake);
                    rTotalSumInsured = rSumInsuerd + rSumInsuerdContent;
                    //BaseRate
                    double rBaseRate = 0;
                    var oBaseLoadFactor = new CResult();
                    result = PolicyCal.GetBaseRate(rSumInsuerd, rTotalSumInsured);
                    if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                    {
                        oBaseLoadFactor = result;
                        string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        double.TryParse(sBase, out rBaseRate);
                    }
                    ////Flood
                    //var oFlood = PolicyCal.GetFloodLoadFactor(sPostCode,iProductID);
                    //if (oFlood.xiStatus == 0 && oFlood.oCollectionResult.Count > 0)
                    //{
                    //    oXiResults.Add(oFlood);
                    //}
                    //Claim Discount                   
                    var oClaimDiscount = PolicyCal.GetClaimDiscount(NoOfClaimsWithinLastThreeYrs, rBaseRate);
                    if (oClaimDiscount.xiStatus == 0 && oClaimDiscount.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oClaimDiscount);
                    }
                    //Year of manufature discount
                    int iVehicleAge = 0;
                    int iYearOfManufacture = 0;
                    if (int.TryParse(sYearOfManufacture, out iYearOfManufacture))
                    {
                        int currentYear = DateTime.Now.Year;
                        iVehicleAge = currentYear - iYearOfManufacture;
                        var oYearOfManufactureDiscount = PolicyCal.GetYearOfManufactureDiscount(iVehicleAge);
                        if (oYearOfManufactureDiscount.xiStatus == 0 && oYearOfManufactureDiscount.oCollectionResult.Count > 0)
                        {
                            oXiResults.Add(oYearOfManufactureDiscount);
                        }
                    }
                    //Policy Excess
                    var oExcess = PolicyCal.GetPolicyExcessLoad(sPolicyExcess);
                    if (oExcess.xiStatus == 0 && oExcess.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oExcess);
                        if (double.TryParse(sPolicyExcess, out rCompulsaryExcess)) { }
                    }
                    //Claim Load
                    if (ClaimResultList.Count > 0)
                    {
                        var oClaimResult = PolicyCal.GetMaxLoadFactor(ClaimResultList);
                        if (oClaimResult.xiStatus == 0 && oClaimResult.oCollectionResult.Count > 0)
                        {
                            oXiResults.Add(oClaimResult);
                        }
                    }
                    //Hire Load
                    var oHireLoadFactor = PolicyCal.GetHireLoadFactor(sHire);
                    if (oHireLoadFactor.xiStatus == 0 && oHireLoadFactor.oCollectionResult != null)
                    {
                        oXiResults.Add(oHireLoadFactor);
                    }
                    //Minimum Premium
                    var oMinimumPremium = new CResult();
                    var MinimumPremium = PolicyCal.GetMinimumPremium();
                    if (MinimumPremium.xiStatus == 0 && MinimumPremium.oCollectionResult != null)
                    {
                        oMinimumPremium = MinimumPremium;
                    }
                    //Legal expenses
                    var oLegalExpenses = new CResult();
                    var LegalExpensesLoad = PolicyCal.GetLegalExpense();
                    if (LegalExpensesLoad.xiStatus == 0 && LegalExpensesLoad.oCollectionResult != null)
                    {
                        oLegalExpenses = LegalExpensesLoad;
                    }
                    //IPT
                    var IPTLoadFactor = new CResult();
                    var oIPTLoadFactor = PolicyCal.GetIPTLoadFactor();
                    if (oIPTLoadFactor.xiStatus == 0 && oIPTLoadFactor.oCollectionResult != null)
                    {
                        IPTLoadFactor = oIPTLoadFactor;
                    }
                    //Occupation
                    var oOccupationResult = PolicyCal.GetMaxLoadFactor(OccupationResult);
                    if (oOccupationResult.xiStatus == 0 && oOccupationResult.oCollectionResult.Count > 0)
                    {
                        if (oOccupationResult.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())
                        {
                            oGeneralRefers.Add(oOccupationResult);
                        }
                        else if (oOccupationResult.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())
                        {
                            oGeneralDeclines.Add(oOccupationResult);
                        }
                    }
                    //General Refers
                    if (!string.IsNullOrEmpty(sMake) && (sMake.ToLower() == "swift" || sMake.ToLower() == "other, not listed"))
                    {
                        oGeneralRefers.Add(PolicyCal.BuildCResultObject("0.0", "Caravan Make", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    if (iVehicleAge >= 25)
                    {
                        oGeneralRefers.Add(PolicyCal.BuildCResultObject("0.0", "Manufacture Year", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    Info.Add("NatureofSiteSC :" + sNatureOfSite);
                    if (!string.IsNullOrEmpty(sNatureOfSite) && (sNatureOfSite.ToLower() == "building/construction site" || sNatureOfSite.ToLower() == "caravan park - unregistered" || sNatureOfSite.ToLower() == "commercial premises" || sNatureOfSite.ToLower() == "farm" || sNatureOfSite.ToLower() == "private residence" || sNatureOfSite.ToLower() == "other location"))
                    {
                        oGeneralRefers.Add(PolicyCal.BuildCResultObject("0.0", "Nature of site", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    Info.Add("static Loss :" + sStaticLoss);
                    if (!string.IsNullOrEmpty(sStaticLoss) && sStaticLoss.ToLower() == "yes")
                    {
                        oGeneralRefers.Add(PolicyCal.BuildCResultObject("0.0", "Loss or damage", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    if (rSumInsuerdContent > 10000)
                    {
                        oGeneralRefers.Add(PolicyCal.BuildCResultObject("0.0", "Contents cover limit", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    if (rTotalClaimCost > 2500)
                    {
                        oGeneralRefers.Add(PolicyCal.BuildCResultObject("0.0", "Claim cost", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    if (!string.IsNullOrEmpty(sAdditionalProposer) && sAdditionalProposer.ToLower() == "yes")
                    {
                        oGeneralRefers.Add(PolicyCal.BuildCResultObject("0.0", "Financial interest", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    if (!string.IsNullOrEmpty(sMotorOffence) && sMotorOffence.ToLower() == "yes")
                    {
                        oGeneralRefers.Add(PolicyCal.BuildCResultObject("0.0", "Motor offence", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    if (!string.IsNullOrEmpty(sBankruptcy) && sBankruptcy.ToLower() == "yes")
                    {
                        oGeneralRefers.Add(PolicyCal.BuildCResultObject("0.0", "Bankruptcy", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    if (!string.IsNullOrEmpty(spermanentresidentUK) && spermanentresidentUK == "10")
                    {
                        oGeneralRefers.Add(PolicyCal.BuildCResultObject("0.0", "Permanent resident of the UK", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    //General Declines
                    if (!string.IsNullOrEmpty(sPersonaleffects) && sPersonaleffects.ToLower() == "yes")
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Personal effects", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    if (!string.IsNullOrEmpty(sInsuredPolicyDetails) && sInsuredPolicyDetails.ToLower() == "yes")
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Previous Insurance policy declined", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    Info.Add("TypeOfCover_" + sTypeOfCovered + " _vehicleAge :" + iVehicleAge);
                    if (!string.IsNullOrEmpty(sTypeOfCovered) && sTypeOfCovered.ToLower() == "new for old" && iVehicleAge > 20)
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Manufacture Year based on type of cover", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    XIAPI oXIAPI = new XIAPI();
                    List<CNV> oParams = new List<CNV>();
                    Info.Add("sMPostCode_" + sPostCode + " _sProductName :" + sProductName + " _ sProductCode :" + sProductCode);
                    oParams.Add(new CNV { sName = "sMPostCode", sValue = sPostCode });
                    oParams.Add(new CNV { sName = "sProductName", sValue = sProductName });
                    oParams.Add(new CNV { sName = "sProductCode", sValue = sProductCode });
                    var oPostCodeI = oIXI.BOI("PostCodeLookUps_T", "", "groupstatus", oParams);
                    if (oPostCodeI != null && oPostCodeI.Attributes.ContainsKey("group"))
                    {
                        //var oArea = oXIAPI.GetValue("PostCodeLookUps_T", "Group", oWHParams);
                        var oArea = oPostCodeI.Attributes["group"].sValue;
                        Info.Add("PostCode Area_" + oArea);
                        if (!string.IsNullOrEmpty(oArea))
                        {
                            oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "PostCode", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                        }
                    }

                    if (oGeneralDeclines.Count > 0)
                    {
                        Info.Add("oGeneralDeclines: Declined" + oGeneralDeclines.Count);
                        foreach (var declineCase in oGeneralDeclines)
                        {
                            oXiResults.Add(declineCase);
                        }
                    }
                    if (oGeneralRefers.Count > 0)
                    {
                        Info.Add("oGeneralRefers: Declined" + oGeneralRefers.Count);
                        foreach (var referCase in oGeneralRefers)
                        {
                            oXiResults.Add(referCase);
                        }
                    }
                    double total = 0.00;
                    double BaseLoad = 0.00;
                    double rTotalExcess = 0.00;
                    double rExcLegalNIPT = 0.00;
                    if (oBaseLoadFactor.oCollectionResult != null)
                    {
                        BaseLoad = Convert.ToDouble(oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                    }
                    total = BaseLoad;
                    if (oXiResults != null && oXiResults.Count() > 0)
                    {
                        int i = 0;
                        foreach (var item in oXiResults)
                        {
                            if (item.oCollectionResult != null)
                            {
                                if (item.oCollectionResult.Where(m => m.sName == "Type").Select(m => m.sValue).FirstOrDefault() == "Percent")
                                {
                                    BaseLoad += ((Convert.ToDouble(item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * BaseLoad);
                                }
                                else
                                {
                                    BaseLoad += Convert.ToDouble(item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                                }
                            }
                            i++;
                        }
                    }
                    double NetPremium = Math.Round(BaseLoad, 2);
                    total = BaseLoad;
                    result = PolicyCal.BuildCResultObject(NetPremium.ToString(), "Net Premium", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString());
                    oXiResults.Add(result);
                    oProductI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Product");
                    oProductI.Attributes["iDefaultAmountType"].BOD = oProductI.BOD;
                    double rMinPremium = 0.0;
                    string sMinPremium = oMinimumPremium.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                    if (double.TryParse(sMinPremium, out rMinPremium))
                    {
                        if (rMinPremium > total)
                        {
                            total = rMinPremium;
                            oXiResults.Add(oMinimumPremium);
                        }
                    }
                    rExcLegalNIPT = total;
                    //oXiResults.Add(oMinimumPremium);
                    double rLegalExpenses = 0.0;
                    string sLegalExpenses = oLegalExpenses.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                    if (double.TryParse(sLegalExpenses, out rLegalExpenses))
                    {

                        total += rLegalExpenses;
                    }
                    oXiResults.Add(oLegalExpenses);
                    double rAdditionalLoad = 0;
                    if (oProductVersionI.Attributes.ContainsKey("rAdditionalLoad"))
                    {
                        var AdditionLoad = oProductVersionI.Attributes["rAdditionalLoad"].sValue;
                        if (double.TryParse(AdditionLoad, out rAdditionalLoad))
                        {
                        }
                    }
                    if (oXiResults.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())) && oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && IPTLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && oMinimumPremium.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && oGeneralDeclines.Count <= 0 && oGeneralRefers.Count <= 0)
                    {
                        Info.Add("All Load Factors are normal");
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "0", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "false", bDirty = true };
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())) || oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString() || oMinimumPremium.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString() || oGeneralRefers.Count > 0 && oGeneralDeclines.Count <= 0)
                    {
                        Info.Add("Some Load Factors are Refered");
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "10", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "true", bDirty = true };
                        //oXiResults.Add(PolicyCal.BuildCResultObject(rAdditionalLoad.ToString(), "Additional Load", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                        //total += (total * 0.01) * rAdditionalLoad;
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())) || oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString() && oMinimumPremium.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString() || oGeneralDeclines.Count > 0)
                    {
                        oResult.sMessage = "Some Load Factors are Declined";
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "20", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "true", bDirty = true };
                        //oXiResults.Add(PolicyCal.BuildCResultObject(rAdditionalLoad.ToString(), "Additional Load", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                        //total += (total * 0.01) * rAdditionalLoad;
                    }
                    double IPT = 0;
                    double rInterestRate = 0;
                    if (IPTLoadFactor.oCollectionResult != null)
                    {
                        rInterestRate = Convert.ToDouble(IPTLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                        IPT = ((Convert.ToDouble(IPTLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * total);
                        //IPT = Math.Round(IPT, 2);
                        result = PolicyCal.BuildCResultObject(String.Format("{0:0.00}", IPT), "Net IPT", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString());
                        oXiResults.Add(result);
                        total += IPT;
                        rExcLegalNIPT += IPT;
                    }
                    total = Math.Round(total, 2);
                    double rFinalQuote = 0;
                    double rPFAmount = 0;
                    double rMonthlyTotal = 0;
                    double rPaymentCharge = 0; double rInsurerCharge = 0; double rAdmin = 0;
                    if (double.TryParse(oProductI.Attributes["rPaymentCharge"].sValue, out rPaymentCharge)) { }
                    if (double.TryParse(oProductI.Attributes["rInsurerCharge"].sValue, out rInsurerCharge)) { }
                    rInsurerCharge += ((rInterestRate * 0.01) * rInsurerCharge);
                    if (double.TryParse(oProductI.Attributes["zDefaultAdmin"].sValue, out rAdmin)) { }
                    rFinalQuote = total + rPaymentCharge + rInsurerCharge + rAdmin;
                    oBII.Attributes["rInterestAmount"] = new XIIAttribute { sName = "rInterestAmount", sValue = String.Format("{0:0.00}", IPT), bDirty = true };
                    oBII.Attributes["rInterestRate"] = new XIIAttribute { sName = "rInterestRate", sValue = String.Format("{0:0.00}", rInterestRate), bDirty = true };
                    oBII.Attributes["rPaymentCharge"] = new XIIAttribute { sName = "rPaymentCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rPaymentCharge"].sValue), bDirty = true };
                    oBII.Attributes["rInsurerCharge"] = new XIIAttribute { sName = "rInsurerCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rInsurerCharge"].sValue), bDirty = true };
                    var MinimumDeposit = oXIAPI.GetMinimumDepostAmount(rPaymentCharge, rInsurerCharge, rFinalQuote, rAdmin, sUID, iInsatnceID, iProductID, 0, 0);
                    double rMinDeposit = 0;
                    if (double.TryParse(MinimumDeposit, out rMinDeposit)) { }
                    oBII.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = String.Format("{0:0.00}", total), bDirty = true };
                    oBII.Attributes["rDefaultQuotePrice"] = new XIIAttribute { sName = "rDefaultQuotePrice", sValue = String.Format("{0:0.00}", total), bDirty = true };
                    oBII.Attributes["rQuotePremium"] = new XIIAttribute { sName = "rQuotePremium", sValue = String.Format("{0:0.00}", total), bDirty = true };
                    oBII.Attributes["rGrossPremium"] = new XIIAttribute { sName = "rGrossPremium", sValue = String.Format("{0:0.00}", total), bDirty = true };
                    oBII.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = String.Format("{0:0.00}", rFinalQuote), bDirty = true };
                    oBII.Attributes["zDefaultDeposit"] = new XIIAttribute { sName = "zDefaultDeposit", sValue = String.Format("{0:0.00}", MinimumDeposit), bDirty = true };
                    oBII.Attributes["zDefaultAdmin"] = new XIIAttribute { sName = "zDefaultAdmin", sValue = String.Format("{0:0.00}", oProductI.Attributes["zDefaultAdmin"].sValue), bDirty = true };
                    var PFSchemeID = oCache.Get_ParamVal(sSessionID, sUID, null, "{XIP|iPFSchemeID}");
                    int iPFSchemeID = 0;
                    if (int.TryParse(PFSchemeID, out iPFSchemeID))
                    { }
                    var MonthlyAmount = oXIAPI.GetMonthlyPremiumAmount(rFinalQuote, rMinDeposit, iProductID, 0, 0, iPFSchemeID);
                    Info.Add("Monthly Amount:" + MonthlyAmount);
                    rMonthlyTotal = (MonthlyAmount * 10) + rMinDeposit;
                    oBII.Attributes["rMonthlyPrice"] = new XIIAttribute { sName = "rMonthlyPrice", sValue = String.Format("{0:0.00}", MonthlyAmount), bDirty = true };
                    oBII.Attributes["rMonthlyTotal"] = new XIIAttribute { sName = "rMonthlyTotal", sValue = String.Format("{0:0.00}", rMonthlyTotal), bDirty = true };
                    rPFAmount = rMonthlyTotal - rMinDeposit;
                    oBII.Attributes["rPremiumFinanceAmount"] = new XIIAttribute { sName = "rPremiumFinanceAmount", sValue = String.Format("{0:0.00}", rPFAmount), bDirty = true };
                    oBII.Attributes["bIsCoverAbroad"] = new XIIAttribute { sName = "bIsCoverAbroad", sValue = oProductI.Attributes["bIsCoverAbroad"].sValue, bDirty = true };
                    oXiResults.Add(IPTLoadFactor);
                    var sQuoteGUID = Guid.NewGuid().ToString("N").Substring(0, 10);
                    var iBatchID = iCustomerID.ToString() + iInsatnceID.ToString();
                    oBII.Attributes["sRegNo"] = new XIIAttribute { sName = "sRegNo", sValue = ostructureInstance.XIIValue("sRegNo").sValue, bDirty = true };
                    oBII.Attributes["dCoverStart"] = new XIIAttribute { sName = "dCoverStart", sValue = dtInsuranceCoverStartDate, bDirty = true };
                    oBII.Attributes["sCaravanMake"] = new XIIAttribute { sName = "sCaravanMake", sValue = ostructureInstance.XIIValue("sCaravanMakeUpdated").sDerivedValue, bDirty = true };
                    oBII.Attributes["sCaravanModel"] = new XIIAttribute { sName = "sCaravanModel", sValue = ostructureInstance.XIIValue("sModelofCaravanUpdated").sValue, bDirty = true };

                    oBII.Attributes["FKiQSInstanceID"] = new XIIAttribute { sName = "FKiQSInstanceID", sValue = iInsatnceID.ToString(), bDirty = true };
                    oBII.Attributes["sInsurer"] = new XIIAttribute { sName = "sInsurer", sValue = "KGM", bDirty = true };
                    oBII.Attributes["FKiCustomerID"] = new XIIAttribute { sName = "FKiCustomerID", sValue = iCustomerID.ToString(), bDirty = true };
                    oBII.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                    oBII.Attributes["dtqsupdateddate"] = new XIIAttribute { sName = "dtqsupdateddate", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBII.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBII.Attributes["FKiProductVersionID"] = new XIIAttribute { sName = "FKiProductVersionID", sValue = sVersion, bDirty = true };
                    oBII.Attributes["BatchID"] = new XIIAttribute { sName = "BatchID", sValue = iBatchID, bDirty = true };
                    oBII.Attributes["sGUID"] = new XIIAttribute { sName = "sGUID", sValue = sQuoteGUID, bDirty = true };

                    //Random generator = new Random();
                    //string sRef = generator.Next(1, 10000000).ToString(new String(''0'', 7));
                    //oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + sRef, bDirty = true };
                    //oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + Guid.NewGuid().ToString("N").Substring(0, 6), bDirty = true };

                    oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = rCompulsaryExcess.ToString(), bDirty = true };
                    oBII.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = "0.00", bDirty = true };
                    oBII.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = rCompulsaryExcess.ToString(), bDirty = true };
                    //oBII.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = rVoluntaryExcess.ToString(), bDirty = true };
                    //oBII.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = (rCompulsaryExcess + rVoluntaryExcess).ToString(), bDirty = true };
                    oBII.Attributes["rExcLegalNIPT"] = new XIIAttribute { sName = "rExcLegalNIPT", sValue = String.Format("{0:0.00}", rExcLegalNIPT), bDirty = true };
                    oBII.Attributes["rLegal"] = new XIIAttribute { sName = "rLegal", sValue = String.Format("{0:0.00}", rLegalExpenses), bDirty = true };

                    oBII.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = iQuoteID.ToString(), bDirty = true };
                    //Info.Add("QuoteRefID_" + oBII.Attributes["sRefID"].sValue);
                    var oRes = oBII.Save(oBII);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        oBII = (XIIBO)oRes.oResult;
                    }
                    Info.Add("KGM SC caravan Quote inserted Sucessfully with the amount of " + total);
                    XIDBO oRiskFactorsBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
                    XIIBO oBO = new XIIBO();
                    oBO.BOD = oRiskFactorsBOD;

                    List<XIIBO> oBOIList = new List<XIIBO>();
                    oBO.Attributes["FKiQuoteID"] = new XIIAttribute { sName = "FKiQuoteID", sValue = oBII.Attributes["ID"].sValue, bDirty = true };
                    oBO.Attributes["sFactorName"] = new XIIAttribute { sName = "sFactorName", sValue = "Base Rate", bDirty = true };
                    oBO.Attributes["sValue"] = new XIIAttribute { sName = "sValue", sValue = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    oBO.Attributes["sMessage"] = new XIIAttribute { sName = "sMessage", sValue = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    oBO.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBO.Attributes["FKsQuoteID"] = new XIIAttribute { sName = "FKsQuoteID", sValue = sQuoteGUID, bDirty = true };
                    oBO.Attributes["ID"] = new XIIAttribute { sName = "ID", bDirty = true };
                    oBOIList.Add(oBO);
                    oBO = new XIIBO();
                    oBO.BOD = oRiskFactorsBOD;
                    //oXiResults.Add(IPTLoadFactor);
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
                    XIIBO xibulk = new XIIBO();
                    DataTable dtbulk = xibulk.MakeBulkSqlTable(oBOIList);
                    xibulk.SaveBulk(dtbulk, oBOIList[0].BOD.iDataSource, oBOIList[0].BOD.TableName);
                    oResult.oCollectionResult.Add(new CNV { sName = "QuoteID", sValue = oBII.Attributes["ID"].sValue });
                    oResult.sMessage = "Success";
                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oResult.oResult = "Success";
                    oResult.sCode = "Info";
                    string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                    oResult.sMessage = sInfo;
                    oIB.SaveErrortoDB(oResult, iInsatnceID);
                }
                catch (Exception ex)
                {
                    string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                    oResult.sMessage = sInfo;
                    oResult.sCode = "Error";
                    oIB.SaveErrortoDB(oResult);
                    oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oResult.oResult = "Error";
                    oIB.SaveErrortoDB(oResult);
                }
                return oResult;
            }
        }',
 N'Prepersist',
 N'C# Code',
 N'0',
 N'2020-03-12 14:25:09.000',
 N'6',
 N'169.254.204.77',
 N'180',
 N'169.254.204.77',
 N'2020-03-12 14:25:09.000',
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
 N'315a88c6-0bcb-4318-ab71-75982b324e0f',
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
