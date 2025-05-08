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
 [FKiVersionIDXIGUID]) Values(N'293',
 N'0',
 N'0',
 NULL,
 N'GIT API',
 N'GIT API',
 N'public static CResult PolicyMainCal(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            PolicyCalculation Pcal = new PolicyCalculation();
            //XIIQS oQsInstance = new XIIQS();         
            CResult oResult = new CResult();
            oResult.sMessage = "Policy Plan API script running";
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
                oResult = Pcal.Calculation(sUID, iInsatnceID, iUserID, iCustomerID, sDataBase, sProductName, sVersion, sProductCode, iQuoteID, sSessionID);
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
            public CResult Calculation(string sUID, int iInstanceID, int iUserID, int iCustomerID, string sDataBase, string sProductName, string sVersion, string sProductCode, int iQuoteID, string sSessionID = null)
            {
                List<string> Info = new List<string>();
                Info.Add("QsInstanceID_" + iInstanceID);
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    List<CResult> oGeneralDeclines = new List<CResult>();
                    XIDXI oXIDXI = new XIDXI();
                    XIIXI oIXI = new XIIXI();
                    XIIBO oBII = new XIIBO();
                    List<CResult> AgesResult = new List<CResult>();
                    List<CResult> OccupationResult = new List<CResult>();
                    List<string> oRelation = new List<string>();
                    XIInfraCache oCache = new XIInfraCache();
                    oBII.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations", null);
                    var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
                    int iProductID = 0;
                    string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                    if (int.TryParse(ProductID, out iProductID)) { }
                    var oProductI = oIXI.BOI("Product", ProductID);
                    CResult ClaimResult = new CResult();
                    //List<CResult> DrivingRestrictions = new List<CResult>();
                    var result = new CResult();
                    CResult ConvictionResult = new CResult();
                    List<CResult> ClaimConvictionResultList = new List<CResult>();
                    CResult ClaimConvictionResult = new CResult();
                    //var oQSI = oIXI.BOI("QS Instance", iInsatnceID.ToString()).Structure("NotationStructure").XILoad();
                    XIBOInstance oQSI = new XIBOInstance();
                    if (sSessionID == null)
                    {
                        oQSI = oIXI.BOI("QS Instance", iInstanceID.ToString()).Structure("NotationStructure").XILoad();
                    }
                    else
                    {
                        oQSI = oCache.Get_QsStructureObj(sSessionID, sUID, "QSInstance_" + iInstanceID + "NotationStructure");
                    }
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    XIContentEditors oXIContent = new XIContentEditors();
                    string sTypeofCover = string.Empty;
                    var oXIIContent = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, "GITXMLRequest", "0");
                    var oXIContentC = oXIIContent.GetCopy();
                    var oXIContentCDef = oXIContentC.FirstOrDefault();
                    if (oXIContentCDef != null)
                    {
                        sTypeofCover = (string)oXIContent.MergeContentTemplate(oXIContentCDef, oQSI).oResult;
                        oCResult.oResult = sTypeofCover;

                        // Consume API
                        string url = "";
                        string sFunction = "xi.s|{xi.a|''XIConfig_T'',''PolicyPlanTestURL'',''sValue'','''',''sName''}";
                        CResult oCR = new CResult();
                        XIDScript oXIScript = new XIDScript();
                        oXIScript.sScript = sFunction.ToString();
                        oCR = oXIScript.Execute_Script("", "");
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            url = "http://localhost:61218/api/XMLHandler/ReadGITXMLAPI/";
                        }
                        string xml = sTypeofCover;
                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                        byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(xml);
                        req.Method = "post";
                        req.ContentType = "text/xml;charset=utf-8";
                        req.ContentLength = requestBytes.Length;
                        Stream requestStream = req.GetRequestStream();
                        requestStream.Write(requestBytes, 0, requestBytes.Length);
                        requestStream.Close();

                        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                        StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.Default);
                        oXIContentCDef.Content = sr.ReadToEnd();
                        XIIBO oboi = new XIIBO();
                        XIDXI oxid = new XIDXI();
                        XIDBO obod = (XIDBO)oxid.Get_BODefinition("APIQuotes_T").oResult;
                        oboi.BOD = obod;
                        oboi.SetAttribute("FKiProductVersionID", sVersion);
                        oboi.SetAttribute("sRequestObject", sTypeofCover);
                        oboi.SetAttribute("sResponseObject", oXIContentCDef.Content);
                        oboi.SetAttribute("FKiQuoteID", iQuoteID.ToString());
                        oboi.SetAttribute("BatchID", iCustomerID.ToString() + iInstanceID.ToString());
                        oboi.SetAttribute("sAPI", "Third Party");
                        var Result = oboi.Save(oboi);
                        sr.Close();
                        res.Close();

                    }
                    //oXIIContent = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, "PolicyPlan API response", "0");
                    //oXIContentC = oXIIContent.GetCopy();
                    //oXIContentCDef = oXIContentC.FirstOrDefault();
                    double total = 0.00;
                    double rCompulsaryExcess = 0;
                    double rVoluntaryExcess = Convert.ToDouble(ostructureInstance.XIIValue("VoluntaryExcess").rValue);
                    if (oXIContentCDef != null)
                    {
                        XmlDocument doc = new XmlDocument();
                        var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, IgnoreWhitespace = true, IgnoreComments = true };

                        var xmlReader = XmlReader.Create(new StringReader(oXIContentCDef.Content), settings);
                        XmlNode node = doc.ReadNode(xmlReader);
                        foreach (XmlNode chldNode in node.ChildNodes)
                        {
                            //Read the attribute Name
                            if (chldNode.Attributes != null && chldNode.Attributes.Count > 0)
                            {
                                total = Convert.ToDouble(chldNode.Attributes["value"].Value);
                                rCompulsaryExcess = Convert.ToDouble(chldNode.Attributes["excess"].Value);
                            }
                            if (chldNode.Name == "noquote")
                            {
                                oGeneralDeclines.Add(BuildCResultObject(chldNode.InnerText, chldNode.Name, xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                            }
                            if (chldNode.Name == "#text")
                            {
                                oGeneralDeclines.Add(BuildCResultObject(chldNode.InnerText, chldNode.Name, xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                            }
                        }
                    }


                    string sCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;
                    XIIBO oBOI = new XIIBO();
                    XICore.XIAPI oXIAPI = new XICore.XIAPI();
                    oBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations");
                    double rFinalQuote = 0;
                    double rPFAmount = 0;
                    double rMonthlyTotal = 0;
                    double IPT = 12;
                    double rInterestRate = 12;
                    double rPaymentCharge = 0; double rInsurerCharge = 0; double rAdmin = 0;

                    if (double.TryParse(oProductI.Attributes["rPaymentCharge"].sValue, out rPaymentCharge)) { }
                    if (double.TryParse(oProductI.Attributes["rInsurerCharge"].sValue, out rInsurerCharge)) { }
                    rInsurerCharge += ((rInterestRate * 0.01) * rInsurerCharge);
                    if (double.TryParse(oProductI.Attributes["zDefaultAdmin"].sValue, out rAdmin)) { }
                    rFinalQuote = total + rPaymentCharge + rInsurerCharge + rAdmin;
                    oBOI.Attributes["rInterestAmount"] = new XIIAttribute { sName = "rInterestAmount", sValue = String.Format("{0:0.00}", IPT), bDirty = true };
                    oBOI.Attributes["rInterestRate"] = new XIIAttribute { sName = "rInterestRate", sValue = String.Format("{0:0.00}", rInterestRate), bDirty = true };
                    oBOI.Attributes["rPaymentCharge"] = new XIIAttribute { sName = "rPaymentCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rPaymentCharge"].sValue), bDirty = true };
                    oBOI.Attributes["rInsurerCharge"] = new XIIAttribute { sName = "rInsurerCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rInsurerCharge"].sValue), bDirty = true };
                    var MinimumDeposit = oXIAPI.GetMinimumDepostAmount(rPaymentCharge, rInsurerCharge, rFinalQuote, rAdmin, sUID, iInstanceID, iProductID, 0, 0);
                    double rMinDeposit = 0;
                    if (double.TryParse(MinimumDeposit, out rMinDeposit)) { }
                    oBOI.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = String.Format("{0:0.00}", total), bDirty = true };
                    oBOI.Attributes["rQuotePremium"] = new XIIAttribute { sName = "rQuotePremium", sValue = String.Format("{0:0.00}", total), bDirty = true };
                    oBOI.Attributes["rGrossPremium"] = new XIIAttribute { sName = "rGrossPremium", sValue = String.Format("{0:0.00}", total), bDirty = true };
                    oBOI.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = String.Format("{0:0.00}", rFinalQuote), bDirty = true };
                    oBOI.Attributes["zDefaultDeposit"] = new XIIAttribute { sName = "zDefaultDeposit", sValue = String.Format("{0:0.00}", MinimumDeposit), bDirty = true };
                    oBOI.Attributes["zDefaultAdmin"] = new XIIAttribute { sName = "zDefaultAdmin", sValue = String.Format("{0:0.00}", oProductI.Attributes["zDefaultAdmin"].sValue), bDirty = true };
                    var PFSchemeID = oCache.Get_ParamVal(sSessionID, sUID, null, "{XIP|iPFSchemeID}");
                    int iPFSchemeID = 0;
                    if (int.TryParse(PFSchemeID, out iPFSchemeID))
                    { }
                    var MonthlyAmount = oXIAPI.GetMonthlyPremiumAmount(rFinalQuote, rMinDeposit, iProductID, 0, 0, iPFSchemeID);
                    Info.Add("Monthly Amount:" + MonthlyAmount + "rPrice:" + total);
                    rMonthlyTotal = (MonthlyAmount * 10) + rMinDeposit;
                    oBOI.Attributes["rMonthlyPrice"] = new XIIAttribute { sName = "rMonthlyPrice", sValue = String.Format("{0:0.00}", MonthlyAmount), bDirty = true };
                    oBOI.Attributes["rMonthlyTotal"] = new XIIAttribute { sName = "rMonthlyTotal", sValue = String.Format("{0:0.00}", rMonthlyTotal), bDirty = true };
                    rPFAmount = rMonthlyTotal - rMinDeposit;
                    oBOI.Attributes["rPremiumFinanceAmount"] = new XIIAttribute { sName = "rPremiumFinanceAmount", sValue = String.Format("{0:0.00}", rPFAmount), bDirty = true };
                    oBOI.Attributes["bIsCoverAbroad"] = new XIIAttribute { sName = "bIsCoverAbroad", sValue = oProductI.Attributes["bIsCoverAbroad"].sValue, bDirty = true };
                    if ((oGeneralDeclines.Count() == 0) || (oGeneralDeclines != null && oGeneralDeclines.Count() > 0 && oGeneralDeclines.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()))))
                    {
                        Info.Add("All Load Factors are Normal");
                        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "0", bDirty = true };
                        oBOI.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "false", bDirty = true };
                    }
                    else if (oGeneralDeclines != null && oGeneralDeclines.Count() > 0 && oGeneralDeclines.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())))
                    {
                        Info.Add("some load factors are declined");
                        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iquotestatus", sValue = "50", bDirty = true };
                        oBOI.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bisoverride", sValue = "true", bDirty = true };
                    }
                    else if (oGeneralDeclines != null && oGeneralDeclines.Count() > 0 && oGeneralDeclines.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())))
                    {
                        Info.Add("some load factors are refered");
                        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iquotestatus", sValue = "50", bDirty = true };
                        oBOI.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bisoverride", sValue = "true", bDirty = true };
                    }
                    var sQuoteGUID = Guid.NewGuid().ToString("N").Substring(0, 10);
                    var oSource = oIXI.BOI("XISource_T", ostructureInstance.Attributes["fkisourceid"].sValue);
                    string sPrefix = string.Empty;
                    if (oSource != null && oSource.Attributes != null && oSource.Attributes.ContainsKey("sprefixcode"))
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
                    Info.Add(oProductI.Attributes["sName"].sValue + "Quote insertion started with the amount of " + total);
                    //Info.Add("QuoteRefID_" + oBOI.Attributes["sRefID"].sValue);
                    var oRes = oBOI.Save(oBOI);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        oBOI = (XIIBO)oRes.oResult;
                    }
                    Info.Add(oProductI.Attributes["sName"].sValue + "Quote inserted Sucessfully with the amount of " + total);
                    XIDBO oRiskFactorsBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
                    XIIBO oBO = new XIIBO();
                    oBO.BOD = oRiskFactorsBOD;
                    List<XIIBO> oBOIList = new List<XIIBO>();
                    foreach (var item in oGeneralDeclines)
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
                    oCResult.oResult = total;
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
 N'2019-12-26 13:59:00.000',
 N'183',
 N'169.254.204.77',
 N'183',
 N'169.254.204.77',
 N'2019-12-26 13:59:00.000',
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
 N'b1818f22-b105-4220-8e45-6a9000ffadff',
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
