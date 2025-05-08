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
 [FKiVersionIDXIGUID]) Values(N'30553',
 N'296',
 N'0',
 NULL,
 N'TradexCalculation',
 N'TradexPolicyCalculation',
 N' public static CResult PolicyMainCal(List<CNV> lParam)
        {
            XIDefinitionBase oXID = new XIDefinitionBase();
            XIInstanceBase oIB = new XIInstanceBase();
            PolicyCalculation Pcal = new PolicyCalculation();
            CResult oResult = new CResult();
            CNV oNV = new CNV();
            oNV.sName = "sCode";
            CResult ocr1 = new CResult();
            ocr1.sMessage = "Entered into PolicyMainCal method for self calculation";
            oXID.SaveErrortoDB(ocr1);
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
            public string iInstanceID { get; set; }
            public int iAge { get; set; }
            string DriverRelationLoad = "1";
            double DemeritLoad = 0;
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetAgeFromDate(string Date, string PresentDate)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    var GetDate = Convert.ToDateTime(Date);
                    var Now = Convert.ToDateTime(PresentDate);

                    int Years = new DateTime(DateTime.Now.Subtract(GetDate).Ticks).Year - 1;
                    DateTime PastYearDate = GetDate.AddYears(Years);
                    int Months = 0;
                    for (int i = 1; i <= 12; i++)
                    {
                        if (PastYearDate.AddMonths(i) == Now)
                        {
                            Months = i;
                            break;
                        }
                        else if (PastYearDate.AddMonths(i) >= Now)
                        {
                            Months = i - 1;
                            break;
                        }
                    }
                    var sAge = Years + "." + Months;

                    oresult.oResult = Convert.ToDouble(sAge);
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

            public CResult GetArea(string PostCode, string sProductName, string sProductCode, string sVersion, string sCoreDataBase, int iAge)
            {
                string Price = string.Empty;
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                var watch = System.Diagnostics.Stopwatch.StartNew();
                CTraceStack oTrace = new CTraceStack();
                oTrace.sClass = this.GetType().Name;
                oTrace.sMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                List<CNV> oTraceInfo = new List<CNV>();
                oTrace.sTask = "Get Area event, postCode poductid and driver age";//expalin about this method logic            

                XIAPI oXIAPI = new XIAPI();
                try
                {
                    oTraceInfo.Add(new CNV { sValue = "Class is XIBOScript_T table script ID and Method is GetArea" });

                    if (iAge >= 21 && iAge <= 69)
                    {
                        if (!string.IsNullOrEmpty(PostCode))
                        {
                            PostCode = PostCode.Replace(" ", "").ToUpper();

                            List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                            oWHParams.Add(new XIWhereParams { sField = "sPostCode", sOperator = "=", sValue = PostCode });
                            oWHParams.Add(new XIWhereParams { sField = "FKiProductID", sOperator = "=", sValue = sVersion });
                            oTraceInfo.Add(new CNV { sValue = "PostCode is: " + PostCode });
                            var oAreaType = oXIAPI.GetValue("TradexPostCode_T", "sCappedNG", oWHParams, "PostalCode");

                            if (!string.IsNullOrEmpty(oAreaType))
                            {
                                oTraceInfo.Add(new CNV { sValue = "Area Group Code is: " + oAreaType + " Age: " + iAge });
                                oWHParams = new List<XIWhereParams>();
                                oTraceInfo.Add(new CNV { sValue = "Driver Age is: " + iAge });
                                oWHParams.Add(new XIWhereParams { sField = "iAge", sOperator = "=", sValue = iAge.ToString() });
                                //oWHParams.Add(new XIWhereParams { sField = "sProductType", sOperator = "=", sValue = PolicyType });
                                oTraceInfo.Add(new CNV { sValue = "Product ID is: " + sVersion });
                                oWHParams.Add(new XIWhereParams { sField = "iProductID", sOperator = "=", sValue = sVersion });
                                oTraceInfo.Add(new CNV { sValue = "Area Grop Code is: " + oAreaType });
                                oWHParams.Add(new XIWhereParams { sField = "sCode", sOperator = "=", sValue = oAreaType });
                                Price = oXIAPI.GetValue("TradexCalculation_T", "rPrice", oWHParams, "TradexCalculations");
                                if (!string.IsNullOrEmpty(Price) && Price != "0")
                                {
                                    oCResult.oResult = Price;
                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.sMessage = "Check entered Postcode is null" + PostCode;
                                    //oCResult.sMessage = "Error: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + " Check Your Area Post Code";
                                    ////oCResult.sCode = "Config Error";
                                    //oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    //oCResult.LogToFile();
                                    //oIB.SaveErrortoDB(oCResult);
                                }
                                return oCResult;
                            }
                            else
                            {
                                PostCode = PostCode.Remove(PostCode.Length - 1);
                                oTraceInfo.Add(new CNV { sValue = "PostCode is: " + PostCode });
                                oCResult = GetArea(PostCode, sProductName, sProductCode, sVersion, sCoreDataBase, iAge);
                                if (oCResult.bOK && oCResult.oResult != null)
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    oCResult.sMessage = "Checking Post Codes:" + PostCode;
                                    oCResult.oResult = "Success";
                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.sMessage = "Check entered Postcode: " + PostCode;
                                }
                                //return oCResult;
                            }
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sMessage = "Check entered Postcode is null" + PostCode;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "Driver Age Should be maintain between 21 and 69 " + iAge;
                    }
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult);
                }
                oCResult.oTraceStack = oTraceInfo;
                oIB.SaveErrortoDB(oCResult);
                return oCResult;
            }

            public CResult GetAreaPrice(string PostCodeArea, string sVersion, double iAge)
            {
                string Price = string.Empty;
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                var watch = System.Diagnostics.Stopwatch.StartNew();
                CTraceStack oTrace = new CTraceStack();
                oTrace.sClass = this.GetType().Name;
                oTrace.sMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                List<CNV> oTraceInfo = new List<CNV>();
                oTrace.sTask = "Get Area event, postCode poductid and driver age";//expalin about this method logic            

                XIAPI oXIAPI = new XIAPI();
                try
                {
                    oTraceInfo.Add(new CNV { sValue = "Class is XIBOScript_T table script ID and Method is GetArea" });

                    if (iAge >= 21 && iAge <= 84)
                    {
                        if (!string.IsNullOrEmpty(PostCodeArea))
                        {
                            List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                            oTraceInfo.Add(new CNV { sValue = "Area Group Code is: " + PostCodeArea + " Age: " + iAge });
                            oTraceInfo.Add(new CNV { sValue = "Driver Age is: " + iAge });
                            oWHParams.Add(new XIWhereParams { sField = "iAge", sOperator = "=", sValue = iAge.ToString() });
                            //oWHParams.Add(new XIWhereParams { sField = "sProductType", sOperator = "=", sValue = PolicyType });
                            oTraceInfo.Add(new CNV { sValue = "Product ID is: " + sVersion });
                            oWHParams.Add(new XIWhereParams { sField = "iProductID", sOperator = "=", sValue = sVersion });
                            oTraceInfo.Add(new CNV { sValue = "Area Grop Code is: " + PostCodeArea });
                            oWHParams.Add(new XIWhereParams { sField = "sCode", sOperator = "=", sValue = PostCodeArea });
                            Price = oXIAPI.GetValue("TradexCalculation_T", "rPrice", oWHParams, "TradexCalculations");
                            if (!string.IsNullOrEmpty(Price) && Price != "0")
                            {
                                oCResult.oResult = Price;
                            }
                            return oCResult;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "Check Driver Age" + iAge;
                    }
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult);
                }
                oCResult.oTraceStack = oTraceInfo;
                oIB.SaveErrortoDB(oCResult);
                return oCResult;
            }


            public CResult GetAreafromPostCodes(string PostCode, string sVersion, int iAge)
            {
                string Price = string.Empty;
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                var watch = System.Diagnostics.Stopwatch.StartNew();
                CTraceStack oTrace = new CTraceStack();
                oTrace.sClass = this.GetType().Name;
                oTrace.sMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                List<CNV> oTraceInfo = new List<CNV>();
                oTrace.sTask = "Get Area event, postCode poductid and driver age";//expalin about this method logic            

                XIAPI oXIAPI = new XIAPI();
                try
                {
                    //oTraceInfo.Add(new CNV { sValue = "Class is XIBOScript_T table script ID and Method is GetArea" });

                    if (iAge >= 21 && iAge <= 84)
                    {
                        if (!string.IsNullOrEmpty(PostCode))
                        {
                            PostCode = PostCode.Replace(" ", "").ToUpper();

                            List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                            oWHParams.Add(new XIWhereParams { sField = "sPostCode", sOperator = "=", sValue = PostCode });
                            oWHParams.Add(new XIWhereParams { sField = "FKiProductID", sOperator = "=", sValue = sVersion });
                            oTraceInfo.Add(new CNV { sValue = "PostCode is: " + PostCode });
                            var oAreaType = oXIAPI.GetValue("TradexPostCode_T", "sCappedNG", oWHParams, "PostalCode");

                            if (!string.IsNullOrEmpty(oAreaType))
                            {
                                //oTraceInfo.Add(new CNV { sValue = "Area Group Code is: " + oAreaType + " Age: " + iAge });
                                //oWHParams = new List<XIWhereParams>();
                                //oTraceInfo.Add(new CNV { sValue = "Driver Age is: " + iAge });
                                //oWHParams.Add(new XIWhereParams { sField = "iAge", sOperator = "=", sValue = iAge.ToString() });
                                ////oWHParams.Add(new XIWhereParams { sField = "sProductType", sOperator = "=", sValue = PolicyType });
                                //oTraceInfo.Add(new CNV { sValue = "Product ID is: " + sVersion });
                                //oWHParams.Add(new XIWhereParams { sField = "iProductID", sOperator = "=", sValue = sVersion });
                                //oTraceInfo.Add(new CNV { sValue = "Area Grop Code is: " + oAreaType });
                                //oWHParams.Add(new XIWhereParams { sField = "sCode", sOperator = "=", sValue = oAreaType });
                                //Price = oXIAPI.GetValue("TradexCalculation_T", "rPrice", oWHParams, "TradexCalculations");
                                //if (!string.IsNullOrEmpty(Price) && Price != "0")
                                //{
                                oCResult.oResult = oAreaType;
                                //}
                                //else
                                //{
                                //    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                //    oCResult.sMessage = "Check entered Postcode is not in DB: " + PostCode;
                                //    //oCResult.sMessage = "Error: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + " Check Your Area Post Code";
                                //    ////oCResult.sCode = "Config Error";
                                //    //oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                //    //oCResult.LogToFile();
                                //    //oIB.SaveErrortoDB(oCResult);
                                //}
                                return oCResult;
                            }
                            else
                            {
                                PostCode = PostCode.Remove(PostCode.Length - 1);
                                oTraceInfo.Add(new CNV { sValue = "PostCode is: " + PostCode });
                                oCResult = GetAreafromPostCodes(PostCode, sVersion, iAge);
                                //if (oCResult.bOK && oCResult.oResult != null)
                                //{
                                //    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                //    oCResult.sMessage = "Checking Post Codes:" + PostCode;
                                //    oCResult.oResult = "Success";
                                //}
                                //else
                                //{
                                //    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                //    oCResult.sMessage = "Check entered Postcode: " + PostCode;
                                //}
                                //return oCResult;
                            }
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sMessage = "Check entered Postcode is null" + PostCode;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "Driver Age Should be maintain between 21 and 84 " + iAge;
                    }
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult);
                }
                oCResult.oTraceStack = oTraceInfo;
                oIB.SaveErrortoDB(oCResult);
                return oCResult;
            }

            public CResult GetValueofPostCodes(string CharType)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                var watch = System.Diagnostics.Stopwatch.StartNew();
                CTraceStack oTrace = new CTraceStack();
                oTrace.sClass = this.GetType().Name;
                oTrace.sMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                List<CNV> oTraceInfo = new List<CNV>();
                oTrace.sTask = "Get value of area Postcode";//expalin about this method logic            
                int Char = 0;
                XIAPI oXIAPI = new XIAPI();
                try
                {
                    //oTraceInfo.Add(new CNV { sValue = "Class is XIBOScript_T table script ID and Method is GetArea" });
                    if (!string.IsNullOrEmpty(CharType))
                    {
                        if (CharType == "A") { Char = 1; }
                        else if (CharType == "B") { Char = 2; }
                        else if (CharType == "C") { Char = 3; }
                        else if (CharType == "D") { Char = 4; }
                        else if (CharType == "E") { Char = 5; }
                        else if (CharType == "F") { Char = 6; }
                        else if (CharType == "G") { Char = 7; }
                        else if (CharType == "H") { Char = 8; }
                        else if (CharType == "I") { Char = 9; }
                        else if (CharType == "J") { Char = 10; }
                        else if (CharType == "K") { Char = 11; }
                        oCResult.oResult = Char;

                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "Invalid Postcode Area " + CharType;
                    }

                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult);
                }
                oCResult.oTraceStack = oTraceInfo;
                oIB.SaveErrortoDB(oCResult);
                return oCResult;
            }


            public CResult DriverPolicyCover(string PolicyCoverType, string DriverAmount, string DriverName, double Age, string PostCode)
            {
                CResult oCresult = new CResult();
                XIInstanceBase oIB = new XIInstanceBase();
                try
                {
                    oCresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = DriverName + " " + Age + PostCode;
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    double rDriverAmount = 0;
                    double.TryParse(DriverAmount, out rDriverAmount);
                    // int iDriverAge = 0;
                    //20 Comprehensive
                    if (PolicyCoverType == "20") { rDriverAmount = rDriverAmount * 1; }
                    //30 Third Party Fire and Theft
                    else if (PolicyCoverType == "30") { rDriverAmount = rDriverAmount * 0.8; }
                    //40 Third Party Only
                    else if (PolicyCoverType == "40") { rDriverAmount = rDriverAmount * 0.75; }
                    else { }
                    oCresult.oResult = rDriverAmount;
                    oNV1.sValue = rDriverAmount.ToString();
                    oCresult.oCollectionResult.Add(oNV);
                    oCresult.oCollectionResult.Add(oNV1);
                    oCresult.oCollectionResult.Add(oNV2);
                    oCresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oCresult.sMessage = "ERROR: [" + oCresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCresult);
                }
                return oCresult;
            }
            public CResult GetClaimLoadFactor(string ClaimStatus, double iClaimAge, string ClaimType)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    double rClaims = 0;
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Claim Load ClaimStatus(" + ClaimType + ") ClaimAge:" + iClaimAge;
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    //Fault Claim 20  //Pending Claim 40 // Non-Fault Claim 30 set as all 0s
                    if (ClaimStatus == "20" || ClaimStatus == "40")
                    {
                        if (iClaimAge <= 1) { rClaims = 0.15; }
                        else if (iClaimAge <= 2) { rClaims = 0.1; }
                        else if (iClaimAge <= 3) { rClaims = 0.05; }
                        else if (iClaimAge <= 4) { rClaims = 0; }
                        else if (iClaimAge <= 5) { rClaims = 0; }
                    }
                    //Fire Claim - NCB Affected 50 //Theft Claim - NCB Affected=60 //Vandalism Claim - NCB Affected 70
                    else if (ClaimStatus == "50" || ClaimStatus == "60" || ClaimStatus == "70")
                    {
                        if (iClaimAge <= 1) { rClaims = 0.1; }
                        else if (iClaimAge <= 2) { rClaims = 0.05; }
                        else if (iClaimAge <= 3) { rClaims = 0; }
                        else if (iClaimAge <= 4) { rClaims = 0; }
                        else if (iClaimAge <= 5) { rClaims = 0; }
                    }
                    oNV1.sValue = rClaims.ToString();
                    oresult.oResult = rClaims;
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetConvictionsLoadFactor(int Banofmonths, string ConvCode, string DriverBan, DateTime dtConvictionDate, double rConAge)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    double rConvictions = 0;
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Convictions Load " + rConAge + "Months";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    string[] row1 = { "IN10", "IN12", "IN14", "IN16" };
                    string[] row2 = { "BA10", "BA30", "DD40", "DD60", "DD80", "DR10", "DR20", "DR30", "DR40", "DR50", "DR60", "DR70", "DR80", "DR90", "TT99", "UT50", "BA40", "DD10", "DD90", "DG10 ", "DG40", "DG60", "DR31", "DR61" };
                    string[] row3 = { "AC10", "AC20", "AC30", "CD10", "CD20", "CD30", "CD40", "CD50", "CD60", "CD70", "CD80", "CD90", "CU80" };
                    string[] row4 = { "CU10", "CU20", "CU30", "CU40", "CU50", "LC20", "LC30", "LC40", "LC50", "MS10", "MS20", "MS30", "MS50", "MS60", "MS70", "MS80", "MS90", "MW10", "PC10", "PC20", "PC30", "SP10", "SP20", "SP30", "SP40", "SP50", "TS10", "TS20", "TS30", "TS40", "TS50", "TS60", "TS70" };
                    string[] row5 = { "TT99" };
                    //yes
                    //if (DriverBan == "20")
                    //{
                    //    string[] row7 = { "CU10", "CU20", "CU30", "CU40", "CU50", "LC20", "LC30", "LC40", "LC50", "MS10", "MS20", "MS30", "MS50", "MS60", "MS70", "MS80", "MS90" };
                    //    //if (ConvCode == "141" || ConvCode == "145" || ConvCode == "149" || ConvCode == "153" || ConvCode == "157" || ConvCode == "161" || ConvCode == "165" || ConvCode == "177" || ConvCode == "181" || ConvCode == "185" || ConvCode == "189" || ConvCode == "173" || ConvCode == "61" || ConvCode == "65" || ConvCode == "69" || ConvCode == "73" || ConvCode == "77")
                    //    if (row7.Any(ConvCode.Contains))
                    //    {
                    //        if (Banofmonths <= 12)
                    //        {
                    //            rConvictions = rConvictions + 0.25;
                    //        }
                    //        else if (Banofmonths >= 12 && Banofmonths <= 24)
                    //        {
                    //            rConvictions = rConvictions + 0.5;
                    //        }
                    //        else if (Banofmonths >= 24 && Banofmonths <= 36)
                    //        {
                    //            rConvictions = rConvictions + 0.75;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (Banofmonths <= 12)
                    //        {
                    //            rConvictions = rConvictions + 0.5;
                    //        }
                    //        else if (Banofmonths >= 12 && Banofmonths <= 24)
                    //        {
                    //            rConvictions = rConvictions + 0.75;
                    //        }
                    //        else if (Banofmonths >= 24 && Banofmonths <= 36)
                    //        {
                    //            rConvictions = rConvictions + 1;
                    //        }
                    //    }
                    //}
                    ////no
                    //else if (DriverBan == "30")
                    //{
                    //if (DateTime.Now <= dtConvictionDate.AddYears(1))
                    if (rConAge <= 1)
                    {
                        if (row1.Any(ConvCode.Contains)) { rConvictions = 0.1; }
                        if (row2.Any(ConvCode.Contains)) { rConvictions = 0.75; }
                        if (row3.Any(ConvCode.Contains)) { rConvictions = 0.25; }
                        if (row4.Any(ConvCode.Contains)) { rConvictions = 0.05; }

                        else { }
                    }
                    else if (rConAge <= 2)//(DateTime.Now >= dtConvictionDate.AddYears(1) && DateTime.Now <= dtConvictionDate.AddYears(2))
                    {
                        if (row1.Any(ConvCode.Contains)) { rConvictions = 0.1; }
                        if (row2.Any(ConvCode.Contains)) { rConvictions = 0.75; }
                        if (row3.Any(ConvCode.Contains)) { rConvictions = 0.25; }
                        if (row4.Any(ConvCode.Contains)) { rConvictions = 0.05; }
                        if (row5.Any(ConvCode.Contains)) { rConvictions = 0.65; }


                    }
                    else if (rConAge <= 3)//(DateTime.Now >= dtConvictionDate.AddYears(2) && DateTime.Now <= dtConvictionDate.AddYears(3))
                    {
                        if (row1.Any(ConvCode.Contains)) { rConvictions = 0.1; }
                        if (row2.Any(ConvCode.Contains)) { rConvictions = 0.65; }
                        if (row3.Any(ConvCode.Contains)) { rConvictions = 0.25; }
                        if (row4.Any(ConvCode.Contains)) { rConvictions = 0.05; }
                        if (row5.Any(ConvCode.Contains)) { rConvictions = 0.55; }

                    }
                    else if (rConAge <= 4)//(DateTime.Now >= dtConvictionDate.AddYears(3) && DateTime.Now <= dtConvictionDate.AddYears(4))
                    {
                        if (row1.Any(ConvCode.Contains)) { rConvictions = 0.05; }
                        if (row2.Any(ConvCode.Contains)) { rConvictions = 0.4; }
                        if (row3.Any(ConvCode.Contains)) { rConvictions = 0.1; }
                        if (row4.Any(ConvCode.Contains)) { rConvictions = 0; }

                    }
                    else if (rConAge <= 5)// (DateTime.Now >= dtConvictionDate.AddYears(4) && DateTime.Now <= dtConvictionDate.AddYears(5))
                    {
                        if (row1.Any(ConvCode.Contains)) { rConvictions = 0.05; }
                        if (row2.Any(ConvCode.Contains)) { rConvictions = 0.01; }
                        if (row3.Any(ConvCode.Contains)) { rConvictions = 0.01; }
                        if (row4.Any(ConvCode.Contains)) { rConvictions = 0; }

                    }
                    //}
                    oNV1.sValue = rConvictions.ToString();

                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }

            public CResult DemeritLoadFactor(double ClaimnConv)
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
                    oNV2.sValue = "Demerit Load";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    //if (Totaldriver > 0) {
                    DemeritLoad = 1 + ClaimnConv;
                    //}

                    oNV1.sValue = DemeritLoad.ToString();

                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetRelationFactor(string DriverType, string DriverRelation, double iDriverAge)
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
                    oNV2.sValue = "Driver Relation";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    //double DriverRelationLoad = 0;
                    //30 Spouse// 40 Common Law
                    if (iDriverAge >= 21 && iDriverAge <= 69 && DriverRelation != "20")
                    {
                        //20 self

                        if (DriverRelation == "30" || DriverRelation == "40")
                        {
                            if (iDriverAge >= 21 && iDriverAge <= 69)
                            {
                                DriverRelationLoad = "0.15";
                            }
                        }
                        //Employee
                        else if (DriverRelation == "50")
                        {
                            if (iDriverAge >= 21 && iDriverAge <= 24) { DriverRelationLoad = "0.45"; }
                            else if (iDriverAge >= 25 && iDriverAge <= 60) { DriverRelationLoad = "0.35"; }
                            else if (iDriverAge >= 61 && iDriverAge <= 69) { DriverRelationLoad = "0.15"; }
                        }
                        //Business Partner
                        else if (DriverRelation == "60")
                        {
                            if (iDriverAge >= 21 && iDriverAge <= 69) { DriverRelationLoad = "0.5"; }
                        }

                    }
                    else if (DriverRelation == "20")
                    {
                        DriverRelationLoad = "1";
                    }
                    else
                    {

                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    oNV1.sValue = DriverRelationLoad.ToString();
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetIndemnityLimit(int iIdenminityLimitSV, int iIdenminityLimitCV)
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
                    oNV2.sValue = "Indemnity Limit";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    double rIndemnity = 1;
                    if (iIdenminityLimitSV == 2500)
                    {
                        if (iIdenminityLimitCV >= 2500 && iIdenminityLimitCV <= 15000)
                        {
                            rIndemnity = 0.775;
                        }
                        else if (iIdenminityLimitCV >= 20000 && iIdenminityLimitCV <= 30000)
                        {
                            rIndemnity = 0.825;
                        }
                        else if (iIdenminityLimitCV >= 35000 && iIdenminityLimitCV <= 50000)
                        {
                            rIndemnity = 1.125;
                        }
                        else
                        {
                            rIndemnity = 0.85;
                        }
                    }
                    else if (iIdenminityLimitSV == 5000)
                    {
                        if (iIdenminityLimitCV >= 5000 && iIdenminityLimitCV <= 15000)
                        {
                            rIndemnity = 0.8;
                        }
                        else if (iIdenminityLimitCV >= 20000 && iIdenminityLimitCV <= 30000)
                        {
                            rIndemnity = 0.85;
                        }
                        else if (iIdenminityLimitCV >= 35000 && iIdenminityLimitCV <= 50000)
                        {
                            rIndemnity = 1.15;
                        }
                        else
                        {
                            rIndemnity = 0.875;
                        }
                    }
                    else if (iIdenminityLimitSV == 7500)
                    {
                        if (iIdenminityLimitCV >= 7500 && iIdenminityLimitCV <= 15000)
                        {
                            rIndemnity = 0.825;
                        }
                        else if (iIdenminityLimitCV >= 20000 && iIdenminityLimitCV <= 30000)
                        {
                            rIndemnity = 0.875;
                        }
                        else if (iIdenminityLimitCV >= 35000 && iIdenminityLimitCV <= 50000)
                        {
                            rIndemnity = 1.175;
                        }
                        else
                        {
                            rIndemnity = 0.9;
                        }
                    }
                    else if (iIdenminityLimitSV == 10000)
                    {
                        if (iIdenminityLimitCV >= 10000 && iIdenminityLimitCV <= 15000)
                        {
                            rIndemnity = 0.85;
                        }
                        else if (iIdenminityLimitCV >= 20000 && iIdenminityLimitCV <= 30000)
                        {
                            rIndemnity = 0.9;
                        }
                        else if (iIdenminityLimitCV >= 35000 && iIdenminityLimitCV <= 50000)
                        {
                            rIndemnity = 1.2;
                        }
                        else
                        {
                            rIndemnity = 0.925;
                        }
                    }
                    else if (iIdenminityLimitSV == 12500)
                    {
                        if (iIdenminityLimitCV >= 12500 && iIdenminityLimitCV <= 15000)
                        {
                            rIndemnity = 0.875;
                        }
                        else if (iIdenminityLimitCV >= 20000 && iIdenminityLimitCV <= 30000)
                        {
                            rIndemnity = 0.925;
                        }
                        else if (iIdenminityLimitCV >= 35000 && iIdenminityLimitCV <= 50000)
                        {
                            rIndemnity = 1.125;
                        }
                        else
                        {
                            rIndemnity = 0.95;
                        }
                    }
                    else if (iIdenminityLimitSV == 15000)
                    {
                        if (iIdenminityLimitCV == 15000)
                        {
                            rIndemnity = 0.9;
                        }
                        else if (iIdenminityLimitCV >= 20000 && iIdenminityLimitCV <= 30000)
                        {
                            rIndemnity = 1;
                        }
                        else if (iIdenminityLimitCV >= 35000 && iIdenminityLimitCV <= 50000)
                        {
                            rIndemnity = 1.25;
                        }
                        else
                        {
                            rIndemnity = 1;
                        }
                    }
                    else if (iIdenminityLimitSV == 20000)
                    {
                        if (iIdenminityLimitCV >= 20000 && iIdenminityLimitCV == 30000)
                        {
                            rIndemnity = 1.25;
                        }
                        else if (iIdenminityLimitCV >= 35000 && iIdenminityLimitCV <= 50000)
                        {
                            rIndemnity = 1.3;
                        }
                        else
                        {
                            rIndemnity = 1.2;
                        }
                    }
                    else if (iIdenminityLimitSV == 25000)
                    {
                        if (iIdenminityLimitCV >= 25000 && iIdenminityLimitCV <= 30000)
                        {
                            rIndemnity = 1.275;
                        }
                        else if (iIdenminityLimitCV >= 35000 && iIdenminityLimitCV <= 50000)
                        {
                            rIndemnity = 1.325;
                        }
                        else
                        {
                            rIndemnity = 1.4;
                        }
                    }
                    else if (iIdenminityLimitSV == 30000)
                    {
                        if (iIdenminityLimitCV == 30000)
                        {
                            rIndemnity = 1.3;
                        }
                        else if (iIdenminityLimitCV >= 35000 && iIdenminityLimitCV <= 50000)
                        {
                            rIndemnity = 1.35;
                        }
                        else
                        {
                            rIndemnity = 1.6;
                        }
                    }
                    else if (iIdenminityLimitSV == 35000)
                    {
                        if (iIdenminityLimitCV >= 35000 && iIdenminityLimitCV <= 50000)
                        {
                            rIndemnity = 1.35;
                        }
                        else
                        {
                            rIndemnity = 1.6;
                        }
                    }
                    else if (iIdenminityLimitSV == 40000)
                    {
                        if (iIdenminityLimitCV >= 40000 && iIdenminityLimitCV <= 50000)
                        {
                            rIndemnity = 1.4;
                        }
                        else
                        {
                            rIndemnity = 1.8;
                        }
                    }
                    else if (iIdenminityLimitSV == 45000)
                    {
                        if (iIdenminityLimitCV >= 45000 && iIdenminityLimitCV <= 50000)
                        {
                            rIndemnity = 1.5;
                        }
                        else
                        {
                            rIndemnity = 2;
                        }
                    }
                    else if (iIdenminityLimitSV == 50000)
                    {
                        rIndemnity = 2;
                    }



                    oNV1.sValue = rIndemnity.ToString();

                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetNCBFactor(string NCBPreviousHistory, string NCBYears, string NCBBounsType)
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
                    oNV2.sValue = "NCB";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    double rNCB = 0;
                    if (!string.IsNullOrEmpty(NCBPreviousHistory))
                    {
                        if (NCBPreviousHistory.ToLower() == "NO".ToLower())
                        {
                            rNCB = 1;
                        }
                        if (NCBBounsType.ToLower() == "car" || NCBBounsType.ToLower() == "van")
                        {
                            if (NCBYears != null)
                            {
                                if (NCBYears.ToLower() == "1 year") { rNCB = 1; }
                                else if (NCBYears.ToLower() == "2 years") { rNCB = 0.8; }
                                else if (NCBYears.ToLower() == "3 years" || NCBYears.ToLower() == "4 years") { rNCB = 0.7; }
                                else if (NCBYears.ToLower() == "5 years" || NCBYears.ToLower() == "6 years" || NCBYears.ToLower() == "7 years" || NCBYears.ToLower() == "8 years" || NCBYears.ToLower() == "9 years" || NCBYears.ToLower() == "10+ years") { rNCB = 0.6; }
                            }
                        }
                        else if (NCBBounsType.ToLower() == "Motor Trade".ToLower())
                        {
                            if (NCBYears != null)
                            {
                                if (NCBYears.ToLower() == "1 year") { rNCB = 0.6; }
                                else if (NCBYears.ToLower() == "2 years") { rNCB = 0.5; }
                                else if (NCBYears.ToLower() == "3 years") { rNCB = 0.45; }
                                else if (NCBYears.ToLower() == "4 years") { rNCB = 0.4; }
                                else if (NCBYears.ToLower() == "5 years") { rNCB = 0.35; }
                                else if (NCBYears.ToLower() == "6 years") { rNCB = 0.325; }
                                else if (NCBYears.ToLower() == "7 years" || NCBYears.ToLower() == "8 years" || NCBYears.ToLower() == "9 years" || NCBYears.ToLower() == "10+ years") { rNCB = 0.3; }

                            }
                        }
                        else
                        {
                            if (NCBYears != null)
                            {
                                if (NCBYears.ToLower() == "1 year") { rNCB = 1; }
                                else if (NCBYears.ToLower() == "2 years") { rNCB = 0.8; }
                                else if (NCBYears.ToLower() == "3 years" || NCBYears.ToLower() == "4 years") { rNCB = 0.7; }
                                else if (NCBYears.ToLower() == "5 years" || NCBYears.ToLower() == "6 years" || NCBYears.ToLower() == "7 years" || NCBYears.ToLower() == "8 years" || NCBYears.ToLower() == "9 years" || NCBYears.ToLower() == "10+ years") { rNCB = 0.6; }
                            }
                        }
                    }
                    oNV1.sValue = rNCB.ToString();

                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetTradeOccupation(string VachicleSale, string VachicleRepair, string VachicleValeting, string VachicleBR, string VachicleCD, string VachicleBodyRepair, string OtherActivity)
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
                    oNV2.sValue = "Trade Occupation";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    double rTradeOcc = 1;
                    // if (VachicleSale.ToLower() == "yes") { rTradeOcc = 1; }
                    // if (VachicleRepair.ToLower() == "yes") { rTradeOcc = 1; }
                    // if (VachicleValeting.ToLower() == "yes") { rTradeOcc = 1; }
                    if (!string.IsNullOrEmpty(VachicleBR) && VachicleBR.ToLower() == "yes") { rTradeOcc = 2; }
                    if (!string.IsNullOrEmpty(VachicleCD) && VachicleCD.ToLower() == "yes") { rTradeOcc = 2; }
                    // if (OtherActivity.ToLower() == "yes") { rTradeOcc = 1; }
                    // if (VachicleSale.ToLower() == "yes" && VachicleRepair.ToLower() == "yes") { rTradeOcc = 1; }
                    // if (VachicleSale.ToLower() == "yes" && VachicleValeting.ToLower() == "yes") { rTradeOcc = 1; }
                    // if (VachicleRepair.ToLower() == "yes" && VachicleValeting.ToLower() == "yes") { rTradeOcc = 1; }
                    oNV1.sValue = rTradeOcc.ToString();
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetDemonstrationCover(string DemoCover)
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
                    oNV2.sValue = "Demonstration Cover";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    double rDemoCover = 0;
                    if (!string.IsNullOrEmpty(DemoCover)) { rDemoCover = 1; }


                    oNV1.sValue = rDemoCover.ToString();

                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetPolicyExcess(string ExcessType)
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
                    double rExcess = 0;
                    if (!string.IsNullOrEmpty(ExcessType)) { rExcess = 1; }
                    oNV1.sValue = rExcess.ToString();
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetProtectedNCB(string ProtectedNCB)
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
                    oNV2.sValue = "Protected NCB";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    double rPrNCB = 0;
                    if (!string.IsNullOrEmpty(ProtectedNCB))
                    {
                        if (ProtectedNCB.ToLower() == "yes")
                        {
                            rPrNCB = 1.15;
                        }
                        else if (ProtectedNCB.ToLower() == "no")
                        {
                            rPrNCB = 1;
                        }
                    }
                    oNV1.sValue = rPrNCB.ToString();
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetABURequired(string ABURequired)
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
                    oNV2.sValue = "ABU Required";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    double rRABU = 1;

                    if (ABURequired != null)
                    {
                        if (ABURequired.ToLower() == "true")
                        {
                            rRABU = 1.2;
                        }
                        else
                        {
                            rRABU = 1;
                        }
                    }
                    oNV1.sValue = rRABU.ToString();
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetDisscount(List<double> DriversAge, double TotalAmount)
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
                    oNV2.sValue = "Disscount";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    double rDisscount = 0;
                    var Drivers = DriversAge.Where(i => i >= 21 && i <= 69).ToList();
                    if (Drivers.Count == 3)
                    {
                        //TotalAmount = TotalAmount * 0.25;
                        rDisscount = Math.Round(TotalAmount * 0.25, 2);
                    }
                    oNV1.sValue = rDisscount.ToString();
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetMinAgeGroup(double iAge, double rTotalAmount, string Covertype)
            {
                string Price = string.Empty;
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                var watch = System.Diagnostics.Stopwatch.StartNew();
                CTraceStack oTrace = new CTraceStack();
                oTrace.sClass = this.GetType().Name;
                oTrace.sMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                List<CNV> oTraceInfo = new List<CNV>();
                string sGroupvalue = "";
                oTrace.sTask = "Get min driver age Groups from all drivers";//expalin about this method logic            

                XIAPI oXIAPI = new XIAPI();
                try
                {
                    oTraceInfo.Add(new CNV { sValue = "Driver age" });

                    if (iAge >= 21 && iAge <= 69)
                    {
                        //if (!string.IsNullOrEmpty(iage))
                        //{
                        oTraceInfo.Add(new CNV { sValue = "driver min Age is: " + iAge });
                        List<CNV> oCNV = new List<CNV>();
                        CNV cnv = new CNV();
                        cnv.sName = "iAge";
                        cnv.sValue = iAge.ToString();
                        oCNV.Add(cnv);
                        XIIXI oIXI = new XIIXI();
                        var TradexGroupValue = oIXI.BOI("TradexGroup", "", "", oCNV);
                        double Group28 = 0;
                        double Group35 = 0;
                        double Group40 = 0;
                        double Group50 = 0;
                        double rGroup28 = 0;
                        double rGroup35 = 0;
                        double rGroup40 = 0;
                        double rGroup50 = 0;

                        //20 Comprehensive
                        if (Covertype == "20")
                        {
                            Group28 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGroup28".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                            rGroup28 = Group28 * rTotalAmount * 1.05;
                            if (rGroup28 < 882)
                            {
                                rGroup28 = 882;
                            }
                            Group35 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGrroup35".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                            rGroup35 = Group35 * rTotalAmount * 1.05;
                            if (rGroup35 < 882)
                            {
                                rGroup35 = 882;
                            }
                            Group40 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGroup40".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                            rGroup40 = Group40 * rTotalAmount * 1.05;
                            if (rGroup40 < 882)
                            {
                                rGroup40 = 882;
                            }
                            Group50 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGroup50".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                            rGroup50 = Group50 * rTotalAmount * 1.05;
                            if (rGroup50 < 882)
                            {
                                rGroup50 = 882;
                            }

                        }
                        //30 Third Party Fire and Theft
                        else if (Covertype == "30")
                        {

                            Group28 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGroup28".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                            rGroup28 = Group28 * rTotalAmount * 1.05;
                            if (rGroup28 < 705.6)
                            {
                                rGroup28 = 705.6;
                            }
                            Group35 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGrroup35".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                            rGroup35 = Group35 * rTotalAmount * 1.05;
                            if (rGroup35 < 705.6)
                            {
                                rGroup35 = 705.6;
                            }
                            Group40 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGroup40".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                            rGroup40 = Group40 * rTotalAmount * 1.05;
                            if (rGroup40 < 705.6)
                            {
                                rGroup40 = 705.6;
                            }
                            Group50 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGroup50".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                            rGroup50 = Group50 * rTotalAmount * 1.05;
                            if (rGroup50 < 705.6)
                            {
                                rGroup50 = 705.6;
                            }
                        }
                        //40 Third Party Only
                        else if (Covertype == "40")
                        {
                            Group28 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGroup28".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                            rGroup28 = Group28 * rTotalAmount * 1.05;
                            if (rGroup28 < 588)
                            {
                                rGroup28 = 588;
                            }
                            Group35 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGrroup35".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                            rGroup35 = Group35 * rTotalAmount * 1.05;
                            if (rGroup35 < 588)
                            {
                                rGroup35 = 588;
                            }
                            Group40 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGroup40".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                            rGroup40 = Group40 * rTotalAmount * 1.05;
                            if (rGroup40 < 588)
                            {
                                rGroup40 = 588;
                            }
                            Group50 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGroup50".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                            rGroup50 = Group50 * rTotalAmount * 1.05;
                            if (rGroup50 < 588)
                            {
                                rGroup50 = 588;
                            }
                        }

                        //var Group28 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k=>k.Key.ToLower()=="rGroup28".ToLower()).Select(i=>i.Value.sValue).FirstOrDefault());
                        //var Group35 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGrroup35".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                        //var Group40 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGroup40".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                        //var Group50 = Convert.ToDouble(TradexGroupValue.Attributes.Where(k => k.Key.ToLower() == "rGroup50".ToLower()).Select(i => i.Value.sValue).FirstOrDefault());
                        sGroupvalue = "Group28 : " + (Math.Round(rGroup28, 3)) + "\n Group35 : " + (Math.Round(rGroup35, 3)) + "\n Group40 : " + (Math.Round(rGroup40, 3)) + "\n Group50 : " + (Math.Round(rGroup50, 3));
                        oCResult.oResult = sGroupvalue;
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "Driver minimum Age Should be maintain in between 21 and 69 " + iAge;
                    }
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult);
                }
                oCResult.oTraceStack = oTraceInfo;
                oIB.SaveErrortoDB(oCResult);
                return oCResult;
            }
        }
        public class PolicyCalculation
        {
            public CResult Calculation(string InsatnceID, int iUserID, int iCustomerID, string sVersion, string sProductCode, int iQuoteID)
            {
                XIDefinitionBase oXID = new XIDefinitionBase();
                List<string> Info = new List<string>();
                Info.Add("[QsInstanceID_" + InsatnceID + "]");
                Info.Add("Granite script running");
                double rTotalAmount = 0;
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                CResult ocr2 = new CResult();
                ocr2.sMessage = "Entered into Calculation method for self calculation";
                oXID.SaveErrortoDB(ocr2);
                try
                {
                    List<CResult> oXiResults = new List<CResult>();
                    PolicyBaseCalc PolicyCal = new PolicyBaseCalc();
                    List<CResult> ClaimResultList = new List<CResult>();
                    CResult DriverResult = new CResult();
                    CResult DriverAgenRelation = new CResult();
                    CResult oResult2 = new CResult();
                    //CResult IndemnityResult = new CResult();
                    //CResult NCBResult = new CResult();
                    //CResult TradeOccupation = new CResult();
                    //CResult DemonstrationCover = new CResult();
                    //CResult PolicyProtectedNCB = new CResult();
                    //CResult RequiredABU = new CResult();
                    //CResult oResult3 = new CResult();
                    //CResult oResult4 = new CResult();
                    //CResult oResult = new CResult();

                    PolicyCal.iInstanceID = InsatnceID;
                    //oresult.xiStatus = 00;//xiEnumSystem.xiFuncResult.xiSuccess;
                    XIIXI oIXI = new XIIXI();
                    var sStructureName = "MT QS";
                    var oQSSI = oIXI.BOI("QS Instance", InsatnceID.ToString()).Structure(sStructureName).XILoad();

                    ocr2 = new CResult();
                    ocr2.sMessage = "Structure Loading MT QS";
                    oXID.SaveErrortoDB(ocr2);
                    //oCache.Set_QsStructureObj(sSessionID, sGUID, "QSInstance_" + iInstanceID + "" + sStructureName + "", oQSSI);

                    var ostructureInstance = oQSSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    var oBaseLoadFactor = new CResult();

                    //Driver
                    var oDriver = oQSSI.oSubStructureI("MTDriver");
                    var oDriverI = (List<XIIBO>)oDriver.oBOIList;
                    // double dirverAmount = 0;

                    List<double> LdriverAge = new List<double>();


                    var HolderAge = PolicyCal.GetAgeFromDOB(ostructureInstance.XIIValue("dMTDateOfBirth").sValue, DateTime.Now.ToString());

                    int iPHAge = Convert.ToInt32(HolderAge.oResult.ToString());
                    CResult HPostCodeArea = new CResult();
                    CResult BPostCodeArea = new CResult();
                    var HolderHomePostCOde = ostructureInstance.XIIValue("sMTHomeHouseNameNumber").sValue;
                    int iHPC = 0; int iBPC = 0;
                    if (!string.IsNullOrEmpty(HolderHomePostCOde))
                    {
                        ocr2 = new CResult();
                        ocr2.sMessage = "Self calculation Home post code: " + HolderHomePostCOde;
                        oXID.SaveErrortoDB(ocr2);
                        HPostCodeArea = PolicyCal.GetAreafromPostCodes(HolderHomePostCOde, sVersion, iPHAge);
                        //iHPC = Convert.ToInt32(PolicyCal.GetValueofPostCodes(HPostCodeArea.oResult.ToString()));
                        if (HPostCodeArea.oResult != null)
                        {
                            var hpc = PolicyCal.GetValueofPostCodes(HPostCodeArea.oResult.ToString());
                            int.TryParse(hpc.oResult.ToString(), out iHPC);
                        }

                    }
                    var BusinessPostCode = ostructureInstance.XIIValue("MTPremisesPostcode").sValue;

                    if (!string.IsNullOrEmpty(BusinessPostCode))
                    {
                        ocr2 = new CResult();
                        ocr2.sMessage = "Self calculation Business post code: " + BusinessPostCode;
                        oXID.SaveErrortoDB(ocr2);
                        BPostCodeArea = PolicyCal.GetAreafromPostCodes(BusinessPostCode, sVersion, iPHAge);
                        //iBPC = Convert.ToInt32(PolicyCal.GetValueofPostCodes(BPostCodeArea.oResult.ToString()));
                        if (BPostCodeArea.oResult != null)
                        {
                            var hpc = PolicyCal.GetValueofPostCodes(BPostCodeArea.oResult.ToString());
                            int.TryParse(hpc.oResult.ToString(), out iBPC);
                        }
                    }
                    var HighestPC = "";
                    if (iBPC == 0 && iHPC == 0)
                    {
                        CResult oResult = new CResult();
                        CNV oNV = new CNV();
                        oNV.sName = "sMessage";
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        CNV oNV1 = new CNV();
                        oNV1.sName = "oResult";
                        CNV oNV2 = new CNV();
                        oNV2.sName = "LoadFactorName";
                        oNV1.sValue = "0.00";
                        oNV2.sValue = "PolicyHolder Age & Area";

                        oResult.oCollectionResult.Add(oNV);
                        oResult.oCollectionResult.Add(oNV1);
                        oResult.oCollectionResult.Add(oNV2);
                        oXiResults.Add(oResult);
                    }
                    else if (iBPC > iHPC)
                    {
                        HighestPC = BPostCodeArea.oResult.ToString();
                    }
                    else { HighestPC = HPostCodeArea.oResult.ToString(); }
                    string DOB = "";
                    if (oDriverI != null)
                    {
                        ocr2 = new CResult();
                        ocr2.sMessage = "Self calculation DriverCount post code: " + oDriverI.Count();
                        oXID.SaveErrortoDB(ocr2);

                        // double rTotalDriverAmount = 0;
                        foreach (var oDrvr in oDriverI)
                        {
                            double rTotalDriverAmount = 0;
                            double SumofClaimsandConv = 0;
                            CResult oCresult = new CResult();
                            double rDriverAmount = 0;
                            double rClaims = 0; double rConvictions = 0;
                            double rRelation = 0;
                            //Area
                            string sPostCode = oDrvr.AttributeI("sPostCode").sValue;//ostructureInstance.XIIValue("sMTHomePostcode").sResolvedValue;

                            string DriverName = oDrvr.AttributeI("sFirstName").sValue;
                            //string Age = oDrvr.AttributeI("ddateofbirth").sValue;// ostructureInstance.XIIValue("MTAge").sValue;

                            var AgeResult = PolicyCal.GetAgeFromDOB(oDrvr.AttributeI("ddateofbirth").sValue, DateTime.Now.ToString());
                            double iAge = 0;
                            double.TryParse(AgeResult.oResult.ToString(), out iAge);
                            LdriverAge.Add(iAge);
                            string PolicyCoverType = ostructureInstance.XIIValue("iMTPolicyCover").sValue;
                            //20 Comprehensive//30 Third Party Fire and Theft//40 Third Party Only
                            //oCresult = PolicyCal.GetArea(sPostCode, null, sProductCode, sVersion, "", iAge);
                            oCresult = PolicyCal.GetAreaPrice(HighestPC, sVersion, iAge);
                            if (oCresult.oResult != null)
                            {
                                DriverResult = PolicyCal.DriverPolicyCover(PolicyCoverType, oCresult.oResult.ToString(), DriverName, iAge, HighestPC);
                                //rDriverAmount = Convert.ToDouble(DriverResult.oResult);
                                if (DriverResult.xiStatus == 0 && DriverResult.oCollectionResult.Count > 0)
                                {
                                    oBaseLoadFactor = DriverResult;
                                    string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                                    double.TryParse(sBase, out rDriverAmount);
                                    oXiResults.Add(DriverResult);
                                }
                            }
                            Info.Add("DriverID_" + oDrvr.Attributes["id"].sValue + "_QSInstanceID_" + InsatnceID);
                            double iDriverAge = 0;
                            //oDrvr.oBOStructure = oDrvr.SubChildI;
                            DOB = oDrvr.AttributeI("ddateofbirth").sValue;
                            var dDOB = "";
                            DateTime dtDOB = DateTime.MinValue;
                            if (DateTime.TryParse(DOB, out dtDOB))
                            {
                                dDOB = dtDOB.ToString("dd/MM/yyyy");
                            }

                            var driverAge = PolicyCal.GetAgeFromDate(dDOB, DateTime.Now.ToString());
                            double.TryParse(driverAge.oResult.ToString(), out iDriverAge);
                            var DriverType = ostructureInstance.XIIValue("iMTDrivingRestriction").sValue;
                            //20 Insuredonly
                            //30 Insured & Spouse//Common Law
                            //40 Insured & Drivers
                            string DriverRelation = oDrvr.AttributeI("iRelationship").sValue;

                            DriverAgenRelation = PolicyCal.GetRelationFactor(DriverType, DriverRelation, iDriverAge);
                            if (DriverAgenRelation.xiStatus == 0 && DriverAgenRelation.oCollectionResult.Count > 0)
                            {
                                oBaseLoadFactor = DriverAgenRelation;
                                string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                                double.TryParse(sBase, out rRelation);
                                oXiResults.Add(DriverAgenRelation);
                            }

                            if (oDrvr.SubChildI.Count > 0)
                            {
                                //MT Claims
                                var MTCla = oDrvr.SubChildI.Where(k => k.Key.ToLower() == "MTClaims".ToLower()).ToList();
                                if (MTCla != null && MTCla.Count > 0)
                                {
                                    var oClaimLIst = oDrvr.SubChildI["MTClaims"]; //oQSSI.oSubStructureI("MTClaims_T");
                                                                                  //var oClaimI = (List<XIIBO>)oClaimLIst.oBOIList;

                                    if (oClaimLIst != null)
                                    {
                                        foreach (var claim in oClaimLIst)
                                        {
                                            double ClaimLi = 0;
                                            var dt12months = DateTime.Now.AddMonths(-12).Date;
                                            //var dtClaimDate = Convert.ToDateTime(claim.AttributeI("dDate").sValue);
                                            var oClaimAge = PolicyCal.GetAgeFromDate(claim.AttributeI("dDate").sValue, DateTime.Now.ToString());
                                            var claimStatus = claim.AttributeI("iClaimStatus").sValue;//20 Fault//30 Non Fault //40 Pending
                                            //var claimType = claim.AttributeI("iClaimType").sValue;//40 Fire//90 Theft //100 Vandalism
                                            var claimCost = claim.AttributeI("rCostOwn").sValue;
                                            double iClaimAge = 0;
                                            double.TryParse(oClaimAge.oResult.ToString(), out iClaimAge);
                                            var ClaimmResult = PolicyCal.GetClaimLoadFactor(claimStatus, iClaimAge, claim.AttributeI("iClaimStatus").sResolvedValue);
                                            // double.TryParse(ClaimmResult.oResult.ToString(), out rClaims);
                                            if (ClaimmResult.xiStatus == 0 && ClaimmResult.oCollectionResult.Count > 0)
                                            {
                                                oBaseLoadFactor = ClaimmResult;
                                                string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                                                double.TryParse(sBase, out ClaimLi);
                                                rClaims += ClaimLi;
                                                oXiResults.Add(ClaimmResult);
                                            }
                                        }
                                    }
                                }
                                //MT Convictions
                                var MTConv = oDrvr.SubChildI.Where(k => k.Key.ToLower() == "MTConvictions".ToLower()).ToList();
                                if (MTConv != null && MTConv.Count > 0)
                                {
                                    var oConvictions = oDrvr.SubChildI["MTConvictions"];//oQSSI.oSubStructureI("MTConvictions");
                                                                                        //var oConvictionsI = (List<XIIBO>)oConvictions.oBOIList;

                                    if (oConvictions != null)
                                    {
                                        foreach (var Conv in oConvictions)
                                        {
                                            double rConvAmount = 0;
                                            var dtConvictionDate = Convert.ToDateTime(Conv.AttributeI("dConvictionDate").sValue);
                                            var Condate = PolicyCal.GetAgeFromDate(Conv.AttributeI("dConvictionDate").sValue, DateTime.Now.ToString());
                                            double rConCode = 0;
                                            double.TryParse(Condate.oResult.ToString(), out rConCode);
                                            var DriverBan = Conv.AttributeI("iDrivingBan").sValue;// 20 Yes, 30 NO
                                                                                                  //var ConvCode = Conv.AttributeI("sConvictionCode").sValue;// IN10 133,LC10 137, LC20 141,
                                            var ConvCode = Conv.AttributeI("sConvictionCode").sResolvedValue;
                                            //var BanforMonths = Convert.ToDateTime(Conv.AttributeI("iMonthsDisqualified").sValue);
                                            var BanforMonths = Conv.AttributeI("iMonthsDisqualified").sValue;
                                            int Banofmonths = 0;
                                            int.TryParse(BanforMonths, out Banofmonths);
                                            //var ConvictionCodes = new[] { "SP30", "SP50","CU80","MS90","IN10","DR10","LC20","AC10","AC20","AC30",
                                            //    "BA10","BA30","BA40","BA60","CD10", "CD20", "CD30", "CD40", "CD50", "CD60", "CD70", "CD80", "CD90", "CU10", "CU20", "CU30",
                                            //    "CU40", "CU50","DD10", "DD40", "DD60", "DD80", "DD90", "DR20", "DR30", "DR31", "DR40", "DR50", "DR60", "DR61", "DR70", "DR80", "DR90",
                                            //    "DG10","DG40","DG60","IN12","IN14","IN16","LC30","LC40","LC50","MS10","MS20","MS30","MS50","MS60","MS70","MS80","MW10","PC10","PC20","PC30","SP10","SP20","SP40","TS10","TS20","TS30","TS40","TS50","TS60","TS70",
                                            //"TT99","UT50"};

                                            var ConviResult = PolicyCal.GetConvictionsLoadFactor(Banofmonths, ConvCode, DriverBan, dtConvictionDate, rConCode);
                                            //var ConviResult = PolicyCal.GetConvictionsLoadFactor(Banofmonths, ConvCode, DriverBan, dtConvictionDate);
                                            //double.TryParse(ConviResult.oResult.ToString(),out rConvictions);
                                            if (ConviResult.xiStatus == 0 && ConviResult.oCollectionResult.Count > 0)
                                            {
                                                oBaseLoadFactor = ConviResult;
                                                string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                                                double.TryParse(sBase, out rConvAmount);
                                                rConvictions += rConvAmount;
                                                oXiResults.Add(ConviResult);
                                            }

                                        }
                                    }
                                }
                            }

                            SumofClaimsandConv = rClaims + rConvictions;
                            if (rDriverAmount > 0)
                            {
                                var DemeritLoad = PolicyCal.DemeritLoadFactor(SumofClaimsandConv);
                                double rDemeritLoad = 0;
                                if (DemeritLoad.xiStatus == 0 && DemeritLoad.oCollectionResult.Count > 0)
                                {
                                    oBaseLoadFactor = DemeritLoad;
                                    string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                                    double.TryParse(sBase, out rDemeritLoad);
                                    rTotalDriverAmount = rDriverAmount * rDemeritLoad;
                                    oXiResults.Add(DemeritLoad);
                                }
                            }

                            if (rRelation > 0)
                            {
                                rTotalDriverAmount = rTotalDriverAmount * rRelation;
                            }
                            rTotalAmount += rTotalDriverAmount;
                            CResult oResult = new CResult();
                            CNV oNV = new CNV();
                            oNV.sName = "sMessage";
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                            CNV oNV1 = new CNV();
                            oNV1.sName = "oResult";
                            CNV oNV2 = new CNV();
                            oNV2.sName = "LoadFactorName";
                            oNV1.sValue = rTotalDriverAmount.ToString();
                            oNV2.sValue = "Driver Base";

                            oResult.oCollectionResult.Add(oNV);
                            oResult.oCollectionResult.Add(oNV1);
                            oResult.oCollectionResult.Add(oNV2);
                            if (oResult.xiStatus == 0 && oResult.oCollectionResult.Count > 0)
                            {
                                oBaseLoadFactor = oResult;
                                oXiResults.Add(oResult);
                            }

                        }
                        oResult2 = new CResult();
                        CNV oNV3 = new CNV();
                        oNV3.sName = "sMessage";
                        oNV3.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                        CNV oNV4 = new CNV();
                        oNV4.sName = "oResult";
                        CNV oNV5 = new CNV();
                        oNV5.sName = "LoadFactorName";

                        oNV4.sValue = rTotalAmount.ToString();
                        oNV5.sValue = "Base Rate";

                        oResult2.oCollectionResult.Add(oNV3);
                        oResult2.oCollectionResult.Add(oNV4);
                        oResult2.oCollectionResult.Add(oNV5);
                        if (oResult2.xiStatus == 0 && oResult2.oCollectionResult.Count > 0)
                        {
                            oBaseLoadFactor = oResult2;
                            oXiResults.Add(oResult2);
                        }
                    }

                    // var ostructureInstance = oQSSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();

                    string IdenminityLimitSV = ostructureInstance.XIIValue("rMTLimitofIndemnityPersonalandStockVehicles").sResolvedValue;
                    int iIdenminityLimitSV = 0;
                    int.TryParse(IdenminityLimitSV, out iIdenminityLimitSV);
                    string IdenminityLimitCV = ostructureInstance.XIIValue("rLimitOfIndemnity").sResolvedValue;
                    int iIdenminityLimitCV = 0;
                    int.TryParse(IdenminityLimitCV, out iIdenminityLimitCV);
                    var IndemnityResult = PolicyCal.GetIndemnityLimit(iIdenminityLimitSV, iIdenminityLimitCV);
                    if (IndemnityResult.xiStatus == 0 && IndemnityResult.oCollectionResult.Count > 0)
                    {
                        double rIndemnityLoad = 0;
                        oBaseLoadFactor = IndemnityResult;
                        string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        double.TryParse(sBase, out rIndemnityLoad);
                        oXiResults.Add(IndemnityResult);
                        rTotalAmount = rTotalAmount * rIndemnityLoad;
                    }
                    //NCB
                    string NCBPreviousBonusHistory = ostructureInstance.XIIValue("iMTNoClaimsBonusType").sResolvedValue;
                    string NCBYears = ostructureInstance.XIIValue("iMTNumberofYearsNCB").sResolvedValue;
                    string NCBBounsType = ostructureInstance.XIIValue("iMTNoClaimsBonusType").sResolvedValue;

                    var NCBResult = PolicyCal.GetNCBFactor(NCBPreviousBonusHistory, NCBYears, NCBBounsType);
                    if (NCBResult.xiStatus == 0 && NCBResult.oCollectionResult.Count > 0)
                    {
                        double rNCB = 0;
                        oBaseLoadFactor = NCBResult;
                        string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        double.TryParse(sBase, out rNCB);
                        oXiResults.Add(NCBResult);
                        if (rNCB > 0)
                            rTotalAmount = rTotalAmount * rNCB;
                    }
                    //Trade Occupation
                    string VachicleSale = ostructureInstance.XIIValue("iMTVehicleSales1").sResolvedValue;
                    string VachicleRepair = ostructureInstance.XIIValue("iMTVehicleRepairsServicing1").sResolvedValue;
                    //string VachicleServicing = ostructureInstance.XIIValue("iMTVehicleServicing3").sResolvedValue;
                    string VachicleValeting = ostructureInstance.XIIValue("iMTVehicleValeting1").sResolvedValue;
                    string VachicleBR = ostructureInstance.XIIValue("iMTBreakdownRecovery1").sResolvedValue;
                    string VachicleCD = ostructureInstance.XIIValue("iMTCollectionandDelivery1").sResolvedValue;
                    string VachicleBodyRepair = ostructureInstance.XIIValue("iMTVehicleBodyRepairs1").sResolvedValue;
                    //Vehicle Sales & Repairs
                    //Vehicle Sales & Valeting
                    //Vehicle Repairs & Valeting
                    string OtherActivity = ostructureInstance.XIIValue("iMTOtherActivity1").sResolvedValue;

                    var TradeOccupation = PolicyCal.GetTradeOccupation(VachicleSale, VachicleRepair, VachicleValeting, VachicleBR, VachicleCD, VachicleBodyRepair, OtherActivity);
                    if (TradeOccupation.xiStatus == 0 && TradeOccupation.oCollectionResult.Count > 0)
                    {
                        double rTradeOC = 0;
                        oBaseLoadFactor = TradeOccupation;
                        string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        double.TryParse(sBase, out rTradeOC);
                        oXiResults.Add(TradeOccupation);
                        if (rTradeOC > 0)
                            rTotalAmount = rTotalAmount * rTradeOC;
                    }

                    //Demonstration Cover Required
                    string DemoCover = ostructureInstance.XIIValue("bMTDoyourequireaccompanieddemonstrationcover").sResolvedValue;
                    var DemonstrationCover = PolicyCal.GetDemonstrationCover(DemoCover);
                    if (DemonstrationCover.xiStatus == 0 && DemonstrationCover.oCollectionResult.Count > 0)
                    {
                        double rDemoCover = 0;
                        oBaseLoadFactor = DemonstrationCover;
                        string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        double.TryParse(sBase, out rDemoCover);
                        oXiResults.Add(DemonstrationCover);
                        if (rDemoCover > 0)
                            rTotalAmount = rTotalAmount * rDemoCover;
                    }
                    //policy Excess
                    string Excess = ostructureInstance.XIIValue("rMTPolicyExcess").sResolvedValue;
                    //var PolicyExcess = PolicyCal.GetPolicyExcess(Excess);
                    //if (PolicyExcess.xiStatus == 0 && PolicyExcess.oCollectionResult.Count > 0)
                    //{
                    //    double rExcess = 0;
                    //    oBaseLoadFactor = PolicyExcess;
                    //    string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                    //    double.TryParse(sBase, out rExcess);
                    //    oXiResults.Add(PolicyExcess);
                    //    if (rExcess > 0)
                    //        rTotalAmount = rTotalAmount * rExcess;
                    //}

                    //Protected NCB
                    string ProtectedNCB = ostructureInstance.XIIValue("iMTProtectedNoClaimsBonus").sResolvedValue;
                    var PolicyProtectedNCB = PolicyCal.GetProtectedNCB(ProtectedNCB);
                    if (PolicyProtectedNCB.xiStatus == 0 && PolicyProtectedNCB.oCollectionResult.Count > 0)
                    {
                        double rPNCB = 0;
                        oBaseLoadFactor = PolicyProtectedNCB;
                        string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        double.TryParse(sBase, out rPNCB);
                        oXiResults.Add(PolicyProtectedNCB);
                        if (rPNCB > 0)
                            rTotalAmount = rTotalAmount * rPNCB;
                    }

                    //5+ years established
                    var BusinessEstablished = Convert.ToDateTime(ostructureInstance.XIIValue("dMTBusinessEstablished").sValue);
                    if (DateTime.Now.AddYears(5) <= BusinessEstablished)
                    {
                        rTotalAmount = rTotalAmount * 1;
                    }
                    //Motorcycles
                    string MotorCycle = ostructureInstance.XIIValue("iMTMotorbikes1").sResolvedValue;
                    if (!string.IsNullOrEmpty(MotorCycle))
                    {
                        rTotalAmount = rTotalAmount * 1;
                    }
                    //Recovery Truck
                    string RecoveryTruck = ostructureInstance.XIIValue("iMTMotorbikes1").sResolvedValue;
                    if (!string.IsNullOrEmpty(MotorCycle))
                    {
                        rTotalAmount = rTotalAmount * 1;
                    }
                    //Vehicle over 3.5T Load
                    string VechicleOverLoad = ostructureInstance.XIIValue("iMTCommercialVehicleuptoGVW1").sResolvedValue;
                    if (!string.IsNullOrEmpty(VechicleOverLoad) && VechicleOverLoad.ToLower() == "yes")
                    {
                        rTotalAmount = rTotalAmount * 1;
                    }
                    //ABU Required
                    string ABURequired = ostructureInstance.XIIValue("bMTAdditionalBusinessUse").sResolvedValue;
                    var RequiredABU = PolicyCal.GetABURequired(ABURequired);
                    if (RequiredABU.xiStatus == 0 && RequiredABU.oCollectionResult.Count > 0)
                    {
                        double rRabu = 0;
                        oBaseLoadFactor = RequiredABU;
                        string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        double.TryParse(sBase, out rRabu);
                        oXiResults.Add(RequiredABU);
                        if (rRabu > 0)
                            rTotalAmount = rTotalAmount * rRabu;
                    }
                    //IPT Tax
                    double IPT = 1.12;
                    rTotalAmount = rTotalAmount * 1.12;
                    CResult oResult3 = new CResult();
                    CNV oNVIPT = new CNV();
                    oNVIPT.sName = "sMessage";
                    oNVIPT.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNVIPT1 = new CNV();
                    oNVIPT1.sName = "oResult";
                    CNV oNVIPT2 = new CNV();
                    oNVIPT2.sName = "LoadFactorName";

                    oNVIPT1.sValue = IPT.ToString();
                    oNVIPT2.sValue = "IPT";

                    oResult3.oCollectionResult.Add(oNVIPT);
                    oResult3.oCollectionResult.Add(oNVIPT1);
                    oResult3.oCollectionResult.Add(oNVIPT2);
                    if (oResult3.xiStatus == 0 && oResult3.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oResult3);
                    }
                    CResult cRDisscount = PolicyCal.GetDisscount(LdriverAge, rTotalAmount);
                    if (cRDisscount.xiStatus == 0 && cRDisscount.oCollectionResult.Count > 0)
                    {
                        double rDisscount = 0;
                        oBaseLoadFactor = cRDisscount;
                        string sBase = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        double.TryParse(sBase, out rDisscount);
                        oXiResults.Add(cRDisscount);
                        if (rDisscount > 0)
                            rTotalAmount = rTotalAmount - rDisscount;
                    }
                    CResult oResult4 = new CResult();
                    CNV oNVTotal = new CNV();
                    oNVTotal.sName = "sMessage";
                    oNVTotal.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNVTotal1 = new CNV();
                    oNVTotal1.sName = "oResult";
                    CNV oNVTotal2 = new CNV();
                    oNVTotal2.sName = "LoadFactorName";
                    oNVTotal1.sValue = Math.Round(rTotalAmount, 2).ToString(); //rFinalPrice;
                    oNVTotal2.sValue = "Quote value";

                    oResult4.oCollectionResult.Add(oNVTotal);
                    oResult4.oCollectionResult.Add(oNVTotal1);
                    oResult4.oCollectionResult.Add(oNVTotal2);
                    if (oResult4.xiStatus == 0 && oResult4.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oResult4);
                    }
                    string MinDAgeGroupvalue = "";
                    if (LdriverAge.Count > 0)
                    {
                        double minAge = LdriverAge.Min();
                        if (minAge > 21 && minAge < 69)
                        {
                            var mingroup = PolicyCal.GetMinAgeGroup(minAge, rTotalAmount, ostructureInstance.XIIValue("iMTPolicyCover").sValue);
                            if (mingroup.oResult != null)
                            {
                                MinDAgeGroupvalue = mingroup.oResult.ToString();
                            }

                        }
                    }

                    ocr2 = new CResult();
                    ocr2.sMessage = "Self calculation Total Amount: " + rTotalAmount;

                    oXID.SaveErrortoDB(ocr2);

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
                    double rMinPremium = 0.0;
                    oBII.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations");
                    if (oXiResults.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue != xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())))
                    {
                        oBII.Attributes["rPaymentCharge"] = new XIIAttribute { sName = "rPaymentCharge", sValue = oProductI.Attributes["rPaymentCharge"].sValue, bDirty = true };
                        oBII.Attributes["rInsurerCharge"] = new XIIAttribute { sName = "rInsurerCharge", sValue = oProductI.Attributes["rInsurerCharge"].sValue, bDirty = true };
                        oBII.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = Math.Round(rTotalAmount, 2).ToString(), bDirty = true };
                        oBII.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = Math.Round(rTotalAmount, 2).ToString(), bDirty = true };
                        oBII.Attributes["zDefaultAdmin"] = new XIIAttribute { sName = "zDefaultAdmin", sValue = oProductI.Attributes["zDefaultAdmin"].sValue, bDirty = true };
                        oBII.Attributes["bIsCoverAbroad"] = new XIIAttribute { sName = "bIsCoverAbroad", sValue = oProductI.Attributes["bIsCoverAbroad"].sValue, bDirty = true };
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "50", bDirty = true };
                        oBII.Attributes["fPercentComplete"] = new XIIAttribute { sName = "fPercentComplete", sValue = "100", bDirty = true };
                    }
                    else
                    {
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "60", bDirty = true };
                    }
                    var sQuoteGUID = Guid.NewGuid().ToString("N").Substring(0, 10);

                    var oSource = oIXI.BOI("XISource_T", ostructureInstance.Attributes["fkisourceid"].sValue);
                    string sPrefix = string.Empty;

                    if (oSource != null && oSource.Attributes != null && oSource.Attributes.ContainsKey("sprefixcode"))
                    {
                        sPrefix = oSource.Attributes["sprefixcode"].sValue;
                    }
                    var iBatchID = iCustomerID.ToString() + InsatnceID.ToString();
                    oBII.Attributes["FKiQSInstanceIDXIGUID"] = new XIIAttribute { sName = "FKiQSInstanceIDXIGUID", sValue = InsatnceID.ToString(), bDirty = true };
                    //oBII.Attributes["sInsurer"] = new XIIAttribute { sName = "sInsurer", sValue = "", bDirty = true };
                    oBII.Attributes["FKiCustomerID"] = new XIIAttribute { sName = "FKiCustomerID", sValue = iCustomerID.ToString(), bDirty = true };
                    oBII.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                    oBII.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString("dd-MMM-yyyy"), bDirty = true };
                    oBII.Attributes["FKiProductVersionID"] = new XIIAttribute { sName = "FKiProductVersionID", sValue = sVersion, bDirty = true };
                    oBII.Attributes["sGUID"] = new XIIAttribute { sName = "sGUID", sValue = sQuoteGUID, bDirty = true };
                    oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + Guid.NewGuid().ToString("N").Substring(0, 6), bDirty = true };
                    oBII.Attributes["BatchID"] = new XIIAttribute { sName = "BatchID", sValue = iBatchID, bDirty = true };
                    oBII.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = iQuoteID.ToString(), bDirty = true };
                    oBII.Attributes["sGroupQuote"] = new XIIAttribute { sName = "sGroupQuote", sValue = MinDAgeGroupvalue, bDirty = true };
                    //oBII.Attributes["XIUpdatedWhen"] = new XIIAttribute { sName = "XIUpdatedWhen", sValue = DateTime.Now.ToString(XIConstant.SqlDateFormat, CultureInfo.InvariantCulture), bDirty = true };
                    oBII.Attributes["fPercentComplete"] = new XIIAttribute { sName = "fPercentComplete", sValue = "100", bDirty = true };


                    Info.Add("Granite Quote inserted Sucessfully with the amount of " + rTotalAmount);
                    XIDBO oRiskFactorsBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
                    XIIBO oBO = new XIIBO();
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
                    xibulk.SaveBulk(dtbulk, oBOIList[0].BOD.iDataSourceXIGUID.ToString(), oBOIList[0].BOD.TableName);
                    string sInfo = "INFO: " + string.Join(",\r\n ", Info);

                    var oRes = oBII.Save(oBII);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        oBII = (XIIBO)oRes.oResult;
                    }
                    oresult.oCollectionResult.Add(new CNV { sName = "QuoteID", sValue = oBII.Attributes["ID"].sValue });
                    oresult.sMessage = "Success";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oresult.oResult = "Success";
                    oresult.sMessage = sInfo;
                    oIB.SaveErrortoDB(oresult);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }

                return oresult;

            }
        }',
 N'Prepersist',
 N'C# Code',
 N'0',
 N'2023-01-10 05:45:44.000',
 N'0',
 N'192.168.0.100',
 N'2035',
 N'172.16.1.5',
 N'2023-01-10 05:45:44.000',
 N'1',
 N'PolicyMainCal',
 N'0',
 NULL,
 N'1',
 N'0',
 N'0',
 N'2021-08-18 14:03:31.000',
 N'0',
 N'2021-08-18 14:03:31.000',
 NULL,
 NULL,
 NULL,
 N'78a49c98-7ebb-4e71-8a40-d964745498f8',
 N'0',
 N'0',
 N'config configu',
 N'2021-08-18 14:03:31.000',
 N'2022-11-15 10:32:50.000',
 NULL,
 NULL,
 NULL,
 N'610981b2-35c6-483e-bdee-5a08e5cbcf5f',
 NULL,
 N'2b4445fc-c977-40d4-9590-5470df8c7e02',
 NULL,
 NULL)
