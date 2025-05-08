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
 [FKiVersionIDXIGUID]) Values(N'272',
 N'296',
 N'0',
 NULL,
 N'GITCalculation',
 N'PolicyCalculation',
 N'public static CResult PolicyMainCal(List<CNV> lParam)
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
            public CResult GetLevelOfCoverLoadFactor(string sCover, int iNoOfvehicles)
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
                    oNV2.sValue = "Level of Cover";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Value";
                    double rAmount = 0;
                    if (sCover == "10")
                    {
                        rAmount = 55;
                    }
                    else if (sCover == "20")
                    {
                        rAmount = 65;
                    }
                    else if (sCover == "30")
                    {
                        rAmount = 70;
                    }
                    else if (sCover == "40")
                    {
                        rAmount = 75;
                    }
                    else if (sCover == "50" || sCover == "60")
                    {
                        rAmount = 80;
                    }
                    else if (sCover == "70")
                    {
                        rAmount = 85;
                    }
                    if (iNoOfvehicles > 1)
                    {
                        rAmount = rAmount + (iNoOfvehicles - 1) * (rAmount / 2);
                    }
                    oNV1.sValue = rAmount.ToString();
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
            public CResult GetPublicLiability(string sLevelOfPublicCover, int iNoOfVehicles)
            {
                CResult oCResult = new CResult();
                XIInstanceBase oIB = new XIInstanceBase();
                XIAPI oXIAPI = new XIAPI();
                try
                {
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Public Liability";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Value";
                    double rAmount = 0;
                    if (sLevelOfPublicCover == "10")
                    {
                        rAmount = 50;
                    }
                    else if (sLevelOfPublicCover == "20")
                    {
                        rAmount = 65;
                    }
                    else if (sLevelOfPublicCover == "10")
                    {
                        rAmount = 75;
                    }
                    oNV1.sValue = (rAmount * iNoOfVehicles).ToString();

                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult, iInstanceID);
                }
                return oCResult;
            }
            public CResult GetEmployersLiability(string sEmpLiability, int iNoOfVehicles)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                XIAPI oXIAPI = new XIAPI();
                try
                {
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Employers Liability";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Value";
                    double rAmount = 0;
                    if (sEmpLiability == "10")
                    {
                        rAmount = 70;
                    }
                    oNV1.sValue = (rAmount * iNoOfVehicles).ToString();
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.oCollectionResult.Add(oNV3);
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult, iInstanceID);
                }
                return oCResult;
            }
            public CResult GetVehicleDiscount(int iNoOfVehicles)
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
                    oNV2.sValue = "Vehicle Discount";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";

                    switch (iNoOfVehicles)
                    {
                        case 1:
                            oNV1.sValue = "-0";
                            break;
                        case 2:
                            oNV1.sValue = "-5";
                            break;
                        case 3:
                            oNV1.sValue = "-8";
                            break;
                        case 4:
                            oNV1.sValue = "-10";
                            break;
                        case 5:
                            oNV1.sValue = "-12";
                            break;
                        case 6:
                            oNV1.sValue = "-14";
                            break;
                        case 7:
                            oNV1.sValue = "-16";
                            break;
                        case 8:
                            oNV1.sValue = "-18";
                            break;
                        case 9:
                            oNV1.sValue = "-20";
                            break;
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                            oNV1.sValue = "-22";
                            break;
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

                    //oresult.LogToFile();
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
            public CResult GetLegalCharge()
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
                    oNV2.sValue = "Legal Charge";
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
                Info.Add("GIT script running");
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                try
                {
                    PolicyBaseCalc PolicyCal = new PolicyBaseCalc();
                    PolicyCal.iInstanceID = iInsatnceID;
                    List<CResult> oXiResults = new List<CResult>();
                    XIIXI oIXI = new XIIXI();
                    XIIBO oBII = new XIIBO();
                    XIInfraCache oCache = new XIInfraCache();
                    oBII.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations", null);
                    var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
                    int iProductID = 0;
                    string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                    if (int.TryParse(ProductID, out iProductID)) { }
                    var oProductI = oIXI.BOI("Product", ProductID);
                    var result = new CResult();
                    XIBOInstance oQSI = new XIBOInstance();
                    if (sSessionID == null)
                    {
                        oQSI = oIXI.BOI("QS Instance", iInsatnceID.ToString()).Structure("NotationStructure").XILoad();
                    }
                    else
                    {
                        oQSI = oCache.Get_QsStructureObj(sSessionID, sUID, "QSInstance_" + iInsatnceID + "NotationStructure");
                    }
                    //var oQSI = oIXI.BOI("QS Instance", iInsatnceID.ToString()).Structure("NotationStructure").XILoad();
                    //var oQSI = oCache.Get_QsStructureObj(sSessionID, sUID, "QSInstance_" + iInsatnceID + "NotationStructure");
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    string dtInsuranceCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;
                    // Fields For Calculation
                    string sLevelOfCover = ostructureInstance.XIIValue("LevelofCover").sValue;
                    string sNoOfVehicles = ostructureInstance.XIIValue("NumberofVehicles").sDerivedValue;
                    int iNoOfVehicles = 0;
                    if (int.TryParse(sNoOfVehicles, out iNoOfVehicles))
                    { }
                    string sPublicLiability = ostructureInstance.XIIValue("LevelofPublicLiabilityCover").sValue;
                    string sEmployerLiability = ostructureInstance.XIIValue("AddEmployersLiability").sValue;

                    //Cover
                    var oCoverLoad = PolicyCal.GetLevelOfCoverLoadFactor(sLevelOfCover, iNoOfVehicles);
                    if (oCoverLoad.xiStatus == 0 && oCoverLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oCoverLoad);
                    }
                    //Public Liability
                    var oPublicLiabilityLoad = PolicyCal.GetPublicLiability(sPublicLiability, iNoOfVehicles);
                    if (oPublicLiabilityLoad.xiStatus == 0 && oPublicLiabilityLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oPublicLiabilityLoad);
                    }
                    //Employers Liability
                    var oEmployerLiability = PolicyCal.GetEmployersLiability(sEmployerLiability, iNoOfVehicles);
                    if (oEmployerLiability.xiStatus == 0 && oEmployerLiability.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oEmployerLiability);
                    }
                    //Vehicle Discount
                    var oVehicleDiscount = PolicyCal.GetVehicleDiscount(iNoOfVehicles);
                    if (oVehicleDiscount.xiStatus == 0 && oVehicleDiscount.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oVehicleDiscount);
                    }
                    //IPT
                    var IPTLoadFactor = new CResult();
                    var oIPTLoadFactor = PolicyCal.GetIPTLoadFactor();
                    if (oIPTLoadFactor.xiStatus == 0 && oIPTLoadFactor.oCollectionResult != null)
                    {
                        IPTLoadFactor = oIPTLoadFactor;
                    }
                    //Legal Charge
                    var LegalChargeLoad = new CResult();
                    var oLegalCharge = PolicyCal.GetLegalCharge();
                    if (oLegalCharge.xiStatus == 0 && oLegalCharge.oCollectionResult != null)
                    {
                        LegalChargeLoad = oLegalCharge;
                    }
                    double total = 0.00;
                    double BaseLoad = 0.00;
                    double rTotalExcess = 0.00;
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
                    //BaseLoad = Math.Round(BaseLoad, 2);
                    total = BaseLoad;
                    result = PolicyCal.BuildCResultObject(NetPremium.ToString(), "Net Premium", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString());
                    oXiResults.Add(result);

                    double rAdditionalLoad = 0;
                    if (oProductVersionI.Attributes.ContainsKey("rAdditionalLoad"))
                    {
                        var AdditionLoad = oProductVersionI.Attributes["rAdditionalLoad"].sValue;
                        if (double.TryParse(AdditionLoad, out rAdditionalLoad))
                        {
                        }
                    }
                    //oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = rCompulsaryExcess.ToString(), bDirty = true };
                    if (oXiResults.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())) && IPTLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && LegalChargeLoad.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())
                    {
                        Info.Add("All Load Factors are normal");
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "0", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "false", bDirty = true };
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())) || LegalChargeLoad.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())
                    {
                        Info.Add("Some Load Factors are Refered");
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "10", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "true", bDirty = true };
                        oXiResults.Add(PolicyCal.BuildCResultObject(rAdditionalLoad.ToString(), "Additional Load", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                        total += (total * 0.01) * rAdditionalLoad;
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())) || LegalChargeLoad.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())
                    {
                        oResult.sMessage = "Some Load Factors are Declined";
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "20", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "true", bDirty = true };
                        oXiResults.Add(PolicyCal.BuildCResultObject(rAdditionalLoad.ToString(), "Additional Load", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                        total += (total * 0.01) * rAdditionalLoad;
                    }
                    double IPT = 0;
                    double rInterestRate = 0;
                    if (IPTLoadFactor.oCollectionResult != null)
                    {
                        rInterestRate = Convert.ToDouble(IPTLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                        IPT = ((Convert.ToDouble(IPTLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * total);
                        //IPT = Math.Round(IPT, 2);
                        result = PolicyCal.BuildCResultObject(String.Format("{0:0.00}", IPT), "Net IPT", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString());
                        oXiResults.Add(IPTLoadFactor);
                        oXiResults.Add(result);
                        total += IPT;
                    }
                    double LegalCharge = 0;
                    if (LegalChargeLoad.oCollectionResult != null)
                    {
                        LegalCharge = Convert.ToDouble(LegalChargeLoad.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                        oXiResults.Add(LegalChargeLoad);
                        total += LegalCharge;
                    }
                    total = Math.Round(total, 2);
                    XIAPI oXIAPI = new XIAPI();
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

                    var sQuoteGUID = Guid.NewGuid().ToString("N").Substring(0, 10);
                    var iBatchID = iCustomerID.ToString() + iInsatnceID.ToString();
                    oBII.Attributes["sRegNo"] = new XIIAttribute { sName = "sRegNo", sValue = ostructureInstance.XIIValue("sRegNo").sValue, bDirty = true };
                    oBII.Attributes["dCoverStart"] = new XIIAttribute { sName = "dCoverStart", sValue = dtInsuranceCoverStartDate, bDirty = true };
                    oBII.Attributes["sCaravanMake"] = new XIIAttribute { sName = "sCaravanMake", sValue = ostructureInstance.XIIValue("sCaravanMakeUpdated").sDerivedValue, bDirty = true };
                    oBII.Attributes["sCaravanModel"] = new XIIAttribute { sName = "sCaravanModel", sValue = ostructureInstance.XIIValue("sModelofCaravanUpdated").sValue, bDirty = true };

                    oBII.Attributes["FKiQSInstanceID"] = new XIIAttribute { sName = "FKiQSInstanceID", sValue = iInsatnceID.ToString(), bDirty = true };
                    oBII.Attributes["sInsurer"] = new XIIAttribute { sName = "sInsurer", sValue = "ClearBroking", bDirty = true };
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

                    oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = String.Format("{0:0.00}", "0.00"), bDirty = true };
                    oBII.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = String.Format("{0:0.00}", "0.00"), bDirty = true };
                    oBII.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = String.Format("{0:0.00}", rTotalExcess), bDirty = true };
                    oBII.Attributes["bIsVisibleToUser"] = new XIIAttribute { sName = "bIsVisibleToUser", sValue = "false", bDirty = true };
                    //oBII.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = rVoluntaryExcess.ToString(), bDirty = true };
                    //oBII.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = (rCompulsaryExcess + rVoluntaryExcess).ToString(), bDirty = true };

                    oBII.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = iQuoteID.ToString(), bDirty = true };
                    //Info.Add("QuoteRefID_" + oBII.Attributes["sRefID"].sValue);
                    var oRes = oBII.Save(oBII);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        oBII = (XIIBO)oRes.oResult;
                    }
                    Info.Add("GIT Quote inserted Sucessfully with the amount of " + total);
                    XIDBO oRiskFactorsBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
                    XIIBO oBO = new XIIBO();
                    oBO.BOD = oRiskFactorsBOD;
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
 N'2020-03-03 14:22:34.000',
 N'6',
 N'169.254.204.77',
 N'174',
 N'fe80::e9b2:6f94:1f17:5fbb%18',
 N'2020-03-03 14:22:34.000',
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
 N'87e7000f-48df-4b43-9611-05f53dba27de',
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
