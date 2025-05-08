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
 [FKiVersionIDXIGUID]) Values(N'167',
 N'296',
 N'0',
 NULL,
 N'TowerGateCalculation',
 N'PolicyCalculation',
 N'        public static CResult PolicyMainCal(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            PolicyCalculation Pcal = new PolicyCalculation();
            //XIIQS oQsInstance = new XIIQS();
            CResult oResult = new CResult();
            oResult.sMessage = "TowerGate script running";
            oIB.SaveErrortoDB(oResult);
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
                //XIInfraCache XIInfraCache = new XIInfraCache();
                //oQsInstance = XIInfraCache.Get_QuestionSetCache("QuestionSetCache", sUID, iInsatnceID);
                //oResult.sMessage = "In Script: Questionset object Loaded from cache";
                //oIB.SaveErrortoDB(oResult);
                oResult = Pcal.Calculation(iInsatnceID, iUserID, iCustomerID, sDataBase, sProductName, sProductCode, sVersion, sSessionID, sUID, iQuoteID);
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

        public class PolicyCalculation
        {
            public CResult Calculation(int iInsatnceID, int iUserID, int iCustomerID, string sDataBase, string sProductName, string sProductCode, string sVersion, string sSessionID, string sUID, int iQuoteID)
            {
                List<string> Info = new List<string>();
                Info.Add("QsInstanceID_" + iInsatnceID);
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                try
                {
                    List<CResult> oGeneralDeclines = new List<CResult>();
                    XIDXI oXIDXI = new XIDXI();
                    List<CResult> oXiResults = new List<CResult>();
                    XIIXI oIXI = new XIIXI();
                    XIIBO oBII = new XIIBO();
                    List<CResult> AgesResult = new List<CResult>();
                    List<CResult> YAgesResult = new List<CResult>();
                    List<int> claimYearList = new List<int>();
                    XIInfraCache oCache = new XIInfraCache();
                    List<CResult> DrivingRestrictions = new List<CResult>();
                    List<CResult> OccupationResult = new List<CResult>();
                    List<string> oRelation = new List<string>();
                    int Age = 0;
                    int claimYear = 0;
                    string DOB = "";
                    string sOccupation = "";
                    oBII.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations");
                    List<CResult> ClaimResult = new List<CResult>();
                    List<CResult> NonFaultClaimResult = new List<CResult>();
                    int NoOfClaims = 0;
                    int NoOfConvinctions = 0;
                    var result = new CResult();
                    List<CResult> ConvictionResult = new List<CResult>();
                    var oQSI = oCache.Get_QsStructureObj(sSessionID, sUID, "QSInstance_" + iInsatnceID + "NotationStructure");
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    var oLIst = oQSI.oSubStructureI("Driver_T");
                    var oDriversI = (List<XIIBO>)oLIst.oBOIList;
                    string dtInsuranceCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;

                    var oExcessVal = new CResult();
                    double rCompulsaryExcess = 250.00;
                    double rExcessVal = 0;
                    string sExcessVal = ostructureInstance.XIIValue("iCurrentCaravanValue").sValue;
                    if (double.TryParse(sExcessVal, out rExcessVal))
                    {
                    }
                    //General Declines

                    Info.Add("RiskFactorsCount:" + oXiResults.Count);
                    foreach (var item in oXiResults)
                    {
                        string sMessage = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        Info.Add(sMessage);
                    }
                    double total = 500.00;
                    double rVoluntaryExcess = 0.00;
                    double rTotalExcess = rCompulsaryExcess + rVoluntaryExcess;
                    var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
                    int iProductID = 0;
                    string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                    if (int.TryParse(ProductID, out iProductID)) { }
                    var oProductI = oIXI.BOI("Product", ProductID);
                    oProductI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Product");
                    oProductI.Attributes["iDefaultAmountType"].BOD = oProductI.BOD;

                    XIAPI oXIAPI = new XIAPI();
                    double rFinalQuote = 0;
                    double rPFAmount = 0;
                    double rMonthlyTotal = 0;
                    double rPaymentCharge = 0; double rInsurerCharge = 0; double rAdmin = 0;
                    if (double.TryParse(oProductI.Attributes["rPaymentCharge"].sValue, out rPaymentCharge)) { }
                    if (double.TryParse(oProductI.Attributes["rInsurerCharge"].sValue, out rInsurerCharge)) { }
                    if (double.TryParse(oProductI.Attributes["zDefaultAdmin"].sValue, out rAdmin)) { }
                    rFinalQuote = total + rPaymentCharge + rInsurerCharge + rAdmin;
                    oBII.Attributes["rPaymentCharge"] = new XIIAttribute { sName = "rPaymentCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rPaymentCharge"].sValue), bDirty = true };
                    oBII.Attributes["rInsurerCharge"] = new XIIAttribute { sName = "rInsurerCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rInsurerCharge"].sValue), bDirty = true };
                    var MinimumDeposit = oXIAPI.GetMinimumDepostAmount(rPaymentCharge, rInsurerCharge, rFinalQuote, rAdmin, sUID, iInsatnceID, iProductID, 0, 0);
                    double rMinDeposit = 0;
                    if (double.TryParse(MinimumDeposit, out rMinDeposit)) { }
                    oBII.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = String.Format("{0:0.00}", total), bDirty = true };
                    oBII.Attributes["rGrossPremium"] = new XIIAttribute { sName = "rGrossPremium", sValue = String.Format("{0:0.00}", total), bDirty = true };
                    oBII.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = String.Format("{0:0.00}", rFinalQuote), bDirty = true };
                    oBII.Attributes["zDefaultDeposit"] = new XIIAttribute { sName = "zDefaultDeposit", sValue = String.Format("{0:0.00}", MinimumDeposit), bDirty = true };
                    oBII.Attributes["zDefaultAdmin"] = new XIIAttribute { sName = "zDefaultAdmin", sValue = String.Format("{0:0.00}", oProductI.Attributes["zDefaultAdmin"].sValue), bDirty = true };
                    var MonthlyAmount = oXIAPI.GetMonthlyPremiumAmount(rFinalQuote, rMinDeposit, iProductID, 0, 0);
                    Info.Add("Monthly Amount:" + MonthlyAmount);
                    rMonthlyTotal = (MonthlyAmount * 10) + rMinDeposit;
                    oBII.Attributes["rMonthlyPrice"] = new XIIAttribute { sName = "rMonthlyPrice", sValue = String.Format("{0:0.00}", MonthlyAmount), bDirty = true };
                    oBII.Attributes["rMonthlyTotal"] = new XIIAttribute { sName = "rMonthlyTotal", sValue = String.Format("{0:0.00}", rMonthlyTotal), bDirty = true };
                    rPFAmount = rMonthlyTotal - rMinDeposit;
                    oBII.Attributes["rPremiumFinanceAmount"] = new XIIAttribute { sName = "rPremiumFinanceAmount", sValue = String.Format("{0:0.00}", rPFAmount), bDirty = true };
                    oBII.Attributes["bIsCoverAbroad"] = new XIIAttribute { sName = "bIsCoverAbroad", sValue = oProductI.Attributes["bIsCoverAbroad"].sValue, bDirty = true };
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
                    oBII.Attributes["sExcess"] = new XIIAttribute { sName = "sExcess", sValue = ExcessContent, bDirty = true };
                    oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "0", bDirty = true };
                    //oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = rCompulsaryExcess.ToString(), bDirty = true };

                    var sQuoteGUID = Guid.NewGuid().ToString("N").Substring(0, 10);
                    var oSource = oIXI.BOI("XISource_T", ostructureInstance.Attributes["fkisourceid"].sValue);
                    string sPrefix = string.Empty;
                    if (oSource.Attributes != null && oSource.Attributes.ContainsKey("sprefixcode"))
                    {
                        sPrefix = oSource.Attributes["sprefixcode"].sValue;
                    }
                    var iBatchID = iCustomerID.ToString() + iInsatnceID.ToString();
                    oBII.Attributes["sRegNo"] = new XIIAttribute { sName = "sRegNo", sValue = ostructureInstance.XIIValue("sRegNo").sValue, bDirty = true };
                    oBII.Attributes["dCoverStart"] = new XIIAttribute { sName = "dCoverStart", sValue = dtInsuranceCoverStartDate, bDirty = true };
                    oBII.Attributes["sCaravanMake"] = new XIIAttribute { sName = "sCaravanMake", sValue = ostructureInstance.XIIValue("sCaravanMakeUpdated").sDerivedValue, bDirty = true };
                    oBII.Attributes["sCaravanModel"] = new XIIAttribute { sName = "sCaravanModel", sValue = ostructureInstance.XIIValue("sModelofCaravanUpdated").sValue, bDirty = true };

                    oBII.Attributes["FKiQSInstanceID"] = new XIIAttribute { sName = "FKiQSInstanceID", sValue = iInsatnceID.ToString(), bDirty = true };
                    oBII.Attributes["sInsurer"] = new XIIAttribute { sName = "sInsurer", sValue = "Towergate", bDirty = true };
                    oBII.Attributes["FKiCustomerID"] = new XIIAttribute { sName = "FKiCustomerID", sValue = iCustomerID.ToString(), bDirty = true };
                    oBII.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                    oBII.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString("dd-MMM-yyyy"), bDirty = true };
                    oBII.Attributes["FKiProductVersionID"] = new XIIAttribute { sName = "FKiProductVersionID", sValue = sVersion, bDirty = true };
                    oBII.Attributes["sGUID"] = new XIIAttribute { sName = "sGUID", sValue = sQuoteGUID, bDirty = true };
                    
                    Random generator = new Random();
        string sRef = generator.Next(1, 10000000).ToString(new String(''0'', 7));
                    oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + sRef, bDirty = true };
                    //oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + Guid.NewGuid().ToString("N").Substring(0, 6), bDirty = true };
                    oBII.Attributes["BatchID"] = new XIIAttribute { sName = "BatchID", sValue = iBatchID, bDirty = true };
                    oBII.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = iQuoteID.ToString(), bDirty = true };
                    oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = String.Format("{0:0.00}", rCompulsaryExcess), bDirty = true };
                    oBII.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = String.Format("{0:0.00}", rVoluntaryExcess), bDirty = true };
                    oBII.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = String.Format("{0:0.00}", rTotalExcess), bDirty = true };
                    Info.Add("QuoteRefID_" + oBII.Attributes["sRefID"].sValue);
                    var oRes = oBII.Save(oBII);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        oBII = (XIIBO)oRes.oResult;
                    }
                    Info.Add("TowerGate Quote inserted Sucessfully with the amount of " + total);
                    XIDBO oRiskFactorsBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
                    XIIBO oBO = new XIIBO();
                    oBO.BOD = oRiskFactorsBOD;
                    List<XIIBO> oBOIList = new List<XIIBO>();
                    oBO.Attributes["FKiQuoteID"] = new XIIAttribute { sName = "FKiQuoteID", sValue = oBII.Attributes["ID"].sValue, bDirty = true };
                    oBO.Attributes["sFactorName"] = new XIIAttribute { sName = "sFactorName", sValue = "Base Rate", bDirty = true };
                    oBO.Attributes["sValue"] = new XIIAttribute { sName = "sValue", sValue = "500.00", bDirty = true };
                    oBO.Attributes["sMessage"] = new XIIAttribute { sName = "sMessage", sValue = "Final Value", bDirty = true };
                    oBO.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBO.Attributes["FKsQuoteID"] = new XIIAttribute { sName = "FKsQuoteID", sValue = sQuoteGUID, bDirty = true };
                    oBO.Attributes["ID"] = new XIIAttribute { sName = "ID", bDirty = true };
                    oBOIList.Add(oBO);
                    XIIBO xibulk = new XIIBO();
                    DataTable dtbulk = xibulk.MakeBulkSqlTable(oBOIList);
                    xibulk.SaveBulk(dtbulk, oBOIList[0].BOD.iDataSource, oBOIList[0].BOD.TableName);
                    oResult.oCollectionResult.Add(new CNV { sName = "QuoteID", sValue = oBII.Attributes["ID"].sValue });
                    oResult.sMessage = "Success";
                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oResult.oResult = "Success";
                    string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                    oResult.sMessage = sInfo;
                    oIB.SaveErrortoDB(oResult);
                }
                catch (Exception ex)
                {
                    string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                    oResult.sMessage = sInfo;
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
 N'2019-06-12 14:58:51.000',
 N'6',
 N'fe80::1584:fa50:8b91:a35d%9',
 N'174',
 N'192.168.7.6',
 N'2019-06-12 14:58:51.000',
 N'15',
 N'PolicyMainCal',
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
 N'aee1460c-ebab-4296-aaf5-799bae4d8d65',
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
