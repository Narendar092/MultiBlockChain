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
 [FKiVersionIDXIGUID]) Values(N'20535',
 N'296',
 N'0',
 NULL,
 N'Charity Product2',
 N'Charity Product2',
 N'public static CResult PolicyMainCal(List < CNV > lParam) {
  XIInstanceBase oIB = new XIInstanceBase();
  PolicyCalculation Pcal = new PolicyCalculation(); /*XIIQS oQsInstance = new XIIQS();   */
  CResult oResult = new CResult();
  oResult.sMessage = "TouringCaravan API script running";
  oIB.SaveErrortoDB(oResult);
  CNV oNV = new CNV();
  oNV.sName = "sCode";
  try {
    string sUID = lParam.Where(m => m.sName == "sUID").FirstOrDefault().sValue;
    string iInsatnceID = lParam.Where(m => m.sName == "iInsatnceID").FirstOrDefault().sValue;
    int iUserID = Convert.ToInt32(lParam.Where(m => m.sName == "iUserID").FirstOrDefault().sValue);
    int iCustomerID = Convert.ToInt32(lParam.Where(m => m.sName == "iCustomerID").FirstOrDefault().sValue);
    string sDataBase = lParam.Where(m => m.sName == "sDataBase").FirstOrDefault().sValue;
    string sProductName = lParam.Where(m => m.sName == "ProductName").FirstOrDefault().sValue;
    string sVersion = lParam.Where(m => m.sName == "Version").FirstOrDefault().sValue;
    string sSessionID = lParam.Where(m => m.sName == "sSessionID").FirstOrDefault().sValue;
    string sProductCode = null;
    int iQuoteID = Convert.ToInt32(lParam.Where(m => m.sName == "iQuoteID").FirstOrDefault().sValue);
    oResult = Pcal.Calculation(sUID, iInsatnceID, iUserID, iCustomerID, sDataBase, sProductName, sVersion, sProductCode, iQuoteID, sSessionID);
    oNV.sValue = "00";
  } catch (Exception ex) {
    oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
    oResult.oResult = "Error";
    oNV.sValue = "100";
    oIB.SaveErrortoDB(oResult);
  }
  oResult.oCollectionResult.Add(oNV);
  return oResult;
}
public class PolicyCalculation {
  XIIXI oIXI = new XIIXI();
  public CResult MergeXML(List < CNV > oParams) {
    CResult oCResult = new CResult();
    try {
      string sQSInstanceID = oParams.Where(x => x.sName.ToLower() == "iQSInstanceID".ToLower()).Select(t => t.sValue).FirstOrDefault();
      XIInfraCache oCache = new XIInfraCache();
      var oContentDef = (List < XIContentEditors > ) oCache.GetObjectFromCache(XIConstant.CacheTemplate, "Charity XML Request object", "0");
      XIContentEditors oDocumentContent = new XIContentEditors();
      oDocumentContent = oContentDef.FirstOrDefault();
      XIContentEditors oConent = new XIContentEditors();
      var oLIst = oIXI.BOI("QS Instance", sQSInstanceID);
      var oInstance = oLIst.Structure("charity qs").XILoad();
      var oRes = oConent.MergeContentTemplate(oDocumentContent, oInstance);
    } catch (Exception ex) {}
    return oCResult;
  }
  public double XMLResponseParsing(string oConent, string sQSInstanceID, string sQuoteID) {
    var Amount = 0.0;
    var Premium = 0.0;
    try {
      CResult oCResult = new CResult();
      XIInfraCache oCache = new XIInfraCache();
      var client = new RestClient("http://20.49.145.1/XRTEService/XRTEService.svc");
      client.Timeout = -1;
      var request = new RestRequest(Method.POST);
      request.AddHeader("Content-Type", "text/xml; charset=utf-8");
      request.AddHeader("SOAPAction", "http://www.polaris.co.uk/XRTEService/2009/03/IXRTEService/ProcessTranXMLDD");
      request.AddHeader("Authorization", "Basic Y2VkMTY4MDYtMDdlZS00NmVjLThhNzEtYzhhYzk3NzAyNTgzOkk2Q3ZrQmdtV0xDWjdKSHJkMmV4VEUzQw==");
      request.AddParameter("text/xml; charset=utf-8", oConent, ParameterType.RequestBody);
      IRestResponse ResRresponse = client.Execute(request);
      XDocument doc1 = new XDocument();
      XNamespace ns = "http://schemas.datacontract.org/2004/07/XRTEService";
      var document = XDocument.Parse(ResRresponse.Content);
      var soapMessage = document.Descendants().Where(p => p.Name.LocalName.Equals("ResponseXML")).Select(t => t.Value).FirstOrDefault();
      doc1 = XDocument.Parse(soapMessage); /*var oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, "Charity XML Response object", "0");                      //XIContentEditors oDocumentContent = new XIContentEditors();                      //oDocumentContent = oContentDef.FirstOrDefault();                      //var document = XDocument.Parse(oDocumentContent.Content.ToString());                      //var soapMessage = document.Descendants().Where(p => p.Name.LocalName.Equals("ResponseXML")).Select(t => t.Value).FirstOrDefault();                      //doc1 = XDocument.Parse(soapMessage);                      //XNamespace ns = "http://www.polaris-uk.co.uk/Schema/IMISRs";      */
      string Content = "";
      if (doc1.Descendants("RiskResponse").Count() > 0) {
        if (doc1.Descendants("Description").FirstOrDefault().Value.ToLower() == "decline") {
          var sDecline = doc1.Descendants("Code").FirstOrDefault().Value + "-" + doc1.Descendants("Description").FirstOrDefault().Value;
        } else {
          Premium = Convert.ToDouble(doc1.Descendants("Premium").FirstOrDefault().Value);
        }
      }
      IEnumerable < XElement > Amounts = doc1.Descendants("Amount");
      foreach(XElement AmountDetails in Amounts) {
        Amount += double.Parse(AmountDetails.Value.ToString());
      }
      XIIBO oBOI = oIXI.BOI("Aggregations", sQuoteID);
      var BOD = (XIDBO) oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations");
      oBOI.BOD = BOD;
      oBOI.SetAttribute("rPrice", Amount.ToString());
      oBOI.SetAttribute("FKiQSInstanceIDXIGUID", sQSInstanceID);
      oCResult = oBOI.Save(oBOI);
      oBOI = new XIIBO();
      BOD = (XIDBO) oCache.GetObjectFromCache(XIConstant.CacheBO, "APIQuotes_T");
      oBOI.BOD = BOD;
      oBOI.SetAttribute("sRequestObject", oConent);
      oBOI.SetAttribute("sResponseObject", ResRresponse.Content.ToString());
      oBOI.SetAttribute("FKiQuoteID", sQuoteID);
      oBOI.SetAttribute("FKiQSInstanceIDXIGUID", sQSInstanceID);
      oCResult = oBOI.Save(oBOI);
      IEnumerable < XElement > responses = doc1.Descendants("Refer");
      oBOI = new XIIBO();
      BOD = (XIDBO) oCache.GetObjectFromCache(XIConstant.CacheBO, "Referrals");
      foreach(XElement response in responses) {
        if (response.HasElements == true) {
          oBOI = new XIIBO();
          oBOI.BOD = BOD;
          oBOI.SetAttribute("sCode", response.Descendants("Code").FirstOrDefault().Value.ToString());
          oBOI.SetAttribute("sLevel", response.Descendants("Level").FirstOrDefault().Value.ToString()); /*oBOI.SetAttribute("sPremises", response.Descendants("Premises").First().Value.ToString());  */
          oBOI.SetAttribute("FkiQuoteID", sQuoteID);
          oCResult = oBOI.Save(oBOI);
        }
      }
      responses = doc1.Descendants("Endorsement");
      BOD = (XIDBO) oCache.GetObjectFromCache(XIConstant.CacheBO, "Term_T");
      foreach(XElement response in responses) {
        if (response.HasElements == true) {
          oBOI = new XIIBO();
          oBOI.BOD = BOD;
          oBOI.SetAttribute("sCode", response.Descendants("Code").FirstOrDefault().Value.ToString()); /*oBOI.SetAttribute("sApplicableTo", response.Descendants("ApplicableTo").First().Value.ToString());  */
          oBOI.SetAttribute("FkiQuoteID", sQuoteID);
          oCResult = oBOI.Save(oBOI);
        }
      }
      responses = doc1.Descendants("PQB").FirstOrDefault().Elements();
      var QuotePriceBOD = (XIDBO) oCache.GetObjectFromCache(XIConstant.CacheBO, "Quote Price");
      var PrimisesBOD = (XIDBO) oCache.GetObjectFromCache(XIConstant.CacheBO, "Premises_T");
      foreach(XElement response in responses) {
        if (response.HasElements == true) {
          if (response.Name.LocalName == "Section") {
            oBOI = new XIIBO();
            oBOI.BOD = QuotePriceBOD;
            oBOI.SetAttribute("sName", response.Descendants("Name").FirstOrDefault().Value.ToString());
            oBOI.SetAttribute("rAmount", response.Descendants("Amount").FirstOrDefault().Value.ToString());
            oBOI.SetAttribute("FkiQuoteID", sQuoteID);
            oCResult = oBOI.Save(oBOI);
          } else {
            oBOI = new XIIBO();
            oBOI.BOD = PrimisesBOD;
            oBOI.SetAttribute("sName", response.Descendants("PremiseIdentifier").FirstOrDefault().Value.ToString());
            var Childs = response.Descendants("Section");
            foreach(var item in Childs) {
              if (item.Descendants("Name").FirstOrDefault().Value.ToString() == "Buildings") {
                oBOI.SetAttribute("sBuildings", item.Descendants("Amount").FirstOrDefault().Value.ToString());
              }
              if (item.Descendants("Name").FirstOrDefault().Value.ToString() == "Contents") {
                oBOI.SetAttribute("sContents", item.Descendants("Amount").FirstOrDefault().Value.ToString());
              }
            }
            oBOI.SetAttribute("FkiQuoteID", sQuoteID);
            oCResult = oBOI.Save(oBOI);
          }
        }
      }
    } catch (Exception ex) {}
    return Convert.ToDouble(Premium);
  }
  public CResult Calculation(string sGUID, string iInstanceID, int iUserID, int iCustomerID, string sDataBase, string sProductName, string sVersion, string sProductCode, int iQuoteID, string sSessionID = null) {
    XIDXI oXID = new XIDXI();
    List < string > Info = new List < string > ();
    Info.Add("QsInstanceID_" + iInstanceID);
    XIInstanceBase oIB = new XIInstanceBase();
    CResult oCResult = new CResult();
    try {
      List < CResult > oGeneralDeclines = new List < CResult > ();
      XIDXI oXIDXI = new XIDXI();
      XIIXI oIXI = new XIIXI();
      XIIBO oBII = new XIIBO();
      XIInfraCache oCache = new XIInfraCache();
      oBII.BOD = (XIDBO) oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations", null);
      var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
      int iProductID = 0;
      string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
      if (int.TryParse(ProductID, out iProductID)) {}
      var oProductI = oIXI.BOI("Product", ProductID);
      string supplier = oProductI.Attributes["FKiSupplierID"].sValue;
      var supplierloading = oIXI.BOI("Supplier_T", supplier);
      CResult ClaimResult = new CResult(); /*List<CResult> DrivingRestrictions = new List<CResult>();   */
      var result = new CResult();
      XIBOInstance oQSI = new XIBOInstance();
      if (sSessionID == null) {
        oQSI = oIXI.BOI("QS Instance", iInstanceID.ToString()).Structure("StaticCaravan Quotes Structure").XILoad();
      } else {
        var oLIst = oIXI.BOI("Aggregations", iQuoteID.ToString());
        oQSI = oLIst.Structure("Quotes XML").XILoad();
      }
      var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault().SubChildI.Where(x => x.Key == "QS Instance1").Select(x => x.Value).FirstOrDefault().FirstOrDefault();
      XIIBO oboi = new XIIBO();
      string APIKey = string.Empty, APIPWD = string.Empty, Reference = string.Empty;
      List < XIIBO > APIValues = new List < XIIBO > ();
      XIContentEditors oXIContent = new XIContentEditors();
      var oContentDef = (List < XIContentEditors > ) oCache.GetObjectFromCache(XIConstant.CacheTemplate, "Charity XML Request object", "0");
      XIContentEditors oDocumentContent = new XIContentEditors();
      oDocumentContent = oContentDef.FirstOrDefault();
      XIContentEditors oConent = new XIContentEditors();
      oCResult = oConent.MergeContentTemplate(oDocumentContent, oQSI);
      var value = XMLResponseParsing(oCResult.oResult.ToString(), iInstanceID.ToString(), iQuoteID.ToString());
      double total = 0.00;
      double rCompulsaryExcess = 0;
      double rVoluntaryExcess = 0;
      string sCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;
      XIIBO oBOI = new XIIBO();
      XICore.XIAPI oXIAPI = new XICore.XIAPI();
      oBOI.BOD = (XIDBO) oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations");
      double rFinalQuote = 0;
      double rPFAmount = 0;
      double rMonthlyTotal = 0;
      double IPT = 12;
      double rInterestRate = 0;
      double rPaymentCharge = 0;
      double rInsurerCharge = 0;
      double rAdmin = 0;
      total = value;
      if (double.TryParse(oProductI.Attributes["rPaymentCharge"].sValue, out rPaymentCharge)) {}
      if (double.TryParse(oProductI.Attributes["rInsurerCharge"].sValue, out rInsurerCharge)) {}
      rInsurerCharge += ((rInterestRate * 0.01) * rInsurerCharge);
      if (double.TryParse(oProductI.Attributes["zDefaultAdmin"].sValue, out rAdmin)) {}
      rFinalQuote = total + rPaymentCharge + rInsurerCharge + rAdmin;
      XIIBO oCurrentQuoteI = oIXI.BOI("Aggregations", iQuoteID.ToString());
      oBOI.Attributes["XIGUID"] = new XIIAttribute {
        sName = "XIGUID", sValue = oCurrentQuoteI.Attributes["XIGUID"].sValue, bDirty = true
      };
      oBOI.Attributes["rInterestAmount"] = new XIIAttribute {
        sName = "rInterestAmount", sValue = String.Format("{0:0.00}", IPT), bDirty = true
      };
      oBOI.Attributes["rInterestRate"] = new XIIAttribute {
        sName = "rInterestRate", sValue = String.Format("{0:0.00}", rInterestRate), bDirty = true
      };
      oBOI.Attributes["rPaymentCharge"] = new XIIAttribute {
        sName = "rPaymentCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rPaymentCharge"].sValue), bDirty = true
      };
      oBOI.Attributes["rInsurerCharge"] = new XIIAttribute {
        sName = "rInsurerCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rInsurerCharge"].sValue), bDirty = true
      };
      var MinimumDeposit = oXIAPI.GetMinimumDepostAmount(rPaymentCharge, rInsurerCharge, rFinalQuote, rAdmin, sGUID, iInstanceID, iProductID, 0, 0);
      double rMinDeposit = 0;
      if (double.TryParse(MinimumDeposit, out rMinDeposit)) {}
      oBOI.Attributes["rPrice"] = new XIIAttribute {
        sName = "rPrice", sValue = String.Format("{0:0.00}", total), bDirty = true
      };
      oBOI.Attributes["rCost"] = new XIIAttribute {
        sName = "rCost", sValue = String.Format("{0:0.00}", total), bDirty = true
      };
      oBOI.Attributes["rDefaultQuotePrice"] = new XIIAttribute {
        sName = "rDefaultQuotePrice", sValue = String.Format("{0:0.00}", total), bDirty = true
      };
      oBOI.Attributes["rQuotePremium"] = new XIIAttribute {
        sName = "rQuotePremium", sValue = String.Format("{0:0.00}", total), bDirty = true
      };
      oBOI.Attributes["rGrossPremium"] = new XIIAttribute {
        sName = "rGrossPremium", sValue = String.Format("{0:0.00}", total), bDirty = true
      };
      oBOI.Attributes["rFinalQuote"] = new XIIAttribute {
        sName = "rFinalQuote", sValue = String.Format("{0:0.00}", rFinalQuote), bDirty = true
      };
      oBOI.Attributes["zDefaultDeposit"] = new XIIAttribute {
        sName = "zDefaultDeposit", sValue = String.Format("{0:0.00}", MinimumDeposit), bDirty = true
      };
      oBOI.Attributes["zDefaultAdmin"] = new XIIAttribute {
        sName = "zDefaultAdmin", sValue = String.Format("{0:0.00}", oProductI.Attributes["zDefaultAdmin"].sValue), bDirty = true
      };
      var  ExlIPT = rFinalQuote * 100 / (100 + IPT);
      oBOI.Attributes["rCommission"] = new XIIAttribute {
        sName = "rCommission", sValue = String.Format("{0:0.00}", (ExlIPT * Convert.ToDouble(oProductI.Attributes["zCommission"].sValue)) / 100), bDirty = true
      };
      oBOI.Attributes["rAdjustedCommission"] = new XIIAttribute {
        sName = "rAdjustedCommission", sValue = String.Format("{0:0.00}", (ExlIPT * Convert.ToDouble(oProductI.Attributes["zCommission"].sValue)) / 100), bDirty = true
      };
      oBOI.Attributes["rFinalCommission"] = new XIIAttribute {
        sName = "rFinalCommission", sValue = String.Format("{0:0.00}", (ExlIPT * Convert.ToDouble(oProductI.Attributes["zCommission"].sValue)) / 100), bDirty = true
      };
      oBOI.Attributes["rAdmin"] = new XIIAttribute {
        sName = "rAdmin", sValue = String.Format("{0:0.00}", (oProductI.Attributes["iAdminType"].sValue == "10") ? (rFinalQuote * Convert.ToDouble(oProductI.Attributes["zDefaultAdmin"].sValue) / 100) : Convert.ToDouble(oProductI.Attributes["zDefaultAdmin"].sValue)), bDirty = true
      };
      oBOI.Attributes["rPaymentToInsurer"] = new XIIAttribute {
        sName = "rPaymentToInsurer", sValue = String.Format("{0:0.00}", rFinalQuote - (Convert.ToDouble(oBOI.Attributes["rCommission"].sValue) + Convert.ToDouble(oBOI.Attributes["rAdmin"].sValue))), bDirty = true
      };
      oBOI.Attributes["zCommissionOverride"] = new XIIAttribute {
        sName = "zCommissionOverride", sValue = String.Format("{0:0.00}", oProductI.Attributes["zCommission"].sValue), bDirty = true
      };
      oBOI.Attributes["rOverrideAdmin"] = new XIIAttribute {
        sName = "rOverrideAdmin", sValue = String.Format("{0:0.00}", oProductI.Attributes["zDefaultAdmin"].sValue), bDirty = true
      };
      var PFSchemeID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iPFSchemeID}");
      int iPFSchemeID = 0;
      if (int.TryParse(PFSchemeID, out iPFSchemeID)) {}
      var MonthlyAmount = oXIAPI.GetMonthlyPremiumAmount(rFinalQuote, rMinDeposit, iProductID, 0, 0, iPFSchemeID);
      Info.Add("Monthly Amount:" + MonthlyAmount + "rPrice:" + total);
      rMonthlyTotal = (MonthlyAmount * 10) + rMinDeposit;
      oBOI.Attributes["rMonthlyPrice"] = new XIIAttribute {
        sName = "rMonthlyPrice", sValue = String.Format("{0:0.00}", MonthlyAmount), bDirty = true
      };
      oBOI.Attributes["rMonthlyTotal"] = new XIIAttribute {
        sName = "rMonthlyTotal", sValue = String.Format("{0:0.00}", rMonthlyTotal), bDirty = true
      };
      rPFAmount = rMonthlyTotal - rMinDeposit;
      oBOI.Attributes["rPremiumFinanceAmount"] = new XIIAttribute {
        sName = "rPremiumFinanceAmount", sValue = String.Format("{0:0.00}", rPFAmount), bDirty = true
      };
      oBOI.Attributes["bIsCoverAbroad"] = new XIIAttribute {
        sName = "bIsCoverAbroad", sValue = oProductI.Attributes["bIsCoverAbroad"].sValue, bDirty = true
      };
      if ((oGeneralDeclines.Count() == 0) || (oGeneralDeclines != null && oGeneralDeclines.Count() > 0 && oGeneralDeclines.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()))) && total > 0) {
        Info.Add("All Load Factors are Normal");
        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute {
          sName = "iQuoteStatus", sValue = "0", bDirty = true
        };
        oBOI.Attributes["bIsOverRide"] = new XIIAttribute {
          sName = "bIsOverRide", sValue = "false", bDirty = true
        };
      } else if (oGeneralDeclines != null && oGeneralDeclines.Count() > 0 && oGeneralDeclines.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()))) {
        Info.Add("some load factors are declined");
        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute {
          sName = "iquotestatus", sValue = "20", bDirty = true
        };
        oBOI.Attributes["bIsOverRide"] = new XIIAttribute {
          sName = "bisoverride", sValue = "true", bDirty = true
        };
      } else if (oGeneralDeclines != null && oGeneralDeclines.Count() > 0 && oGeneralDeclines.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()))) {
        Info.Add("some load factors are refered");
        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute {
          sName = "iquotestatus", sValue = "10", bDirty = true
        };
        oBOI.Attributes["bIsOverRide"] = new XIIAttribute {
          sName = "bisoverride", sValue = "true", bDirty = true
        };
      }
      var sQuoteGUID = Guid.NewGuid().ToString("N").Substring(0, 10);
      var oSource = oIXI.BOI("XISource_T", ostructureInstance.Attributes["fkisourceid"].sValue);
      string sPrefix = string.Empty;
      if (oSource != null && oSource.Attributes != null && oSource.Attributes.ContainsKey("sprefixcode")) {
        sPrefix = oSource.Attributes["sprefixcode"].sValue;
      }
      var iBatchID = iCustomerID.ToString() + iInstanceID.ToString();
      oBOI.Attributes["sRegNo"] = new XIIAttribute {
        sName = "sRegNo", sValue = ostructureInstance.XIIValue("sRegNo").sValue, bDirty = true
      };
      oBOI.Attributes["dCoverStart"] = new XIIAttribute {
        sName = "dCoverStart", sValue = sCoverStartDate, bDirty = true
      };
      oBOI.Attributes["sCaravanMake"] = new XIIAttribute {
        sName = "sCaravanMake", sValue = ostructureInstance.XIIValue("sCaravanMakeUpdated").sDerivedValue, bDirty = true
      };
      oBOI.Attributes["sCaravanModel"] = new XIIAttribute {
        sName = "sCaravanModel", sValue = ostructureInstance.XIIValue("sModelofCaravanUpdated").sValue, bDirty = true
      };
      oBOI.Attributes["FKiQSInstanceIDXIGUID"] = new XIIAttribute {
        sName = "FKiQSInstanceIDXIGUID", sValue = iInstanceID.ToString(), bDirty = true
      }; /*oBOI.Attributes["sInsurer"] = new XIIAttribute                      {                          sName = "sInsurer",                          sValue = supplierloading.Attributes["sName"].sValue,                          bDirty = true                      };*/
      oBOI.Attributes["FKiCustomerID"] = new XIIAttribute {
        sName = "FKiCustomerID", sValue = iCustomerID.ToString(), bDirty = true
      };
      oBOI.Attributes["FKiUserID"] = new XIIAttribute {
        sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true
      };
      oBOI.Attributes["dtqsupdateddate"] = new XIIAttribute {
        sName = "dtqsupdateddate", sValue = DateTime.Now.ToString(), bDirty = true
      };
      oBOI.Attributes["CreatedTime"] = new XIIAttribute {
        sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true
      };
      oBOI.Attributes["FKiProductVersionID"] = new XIIAttribute {
        sName = "FKiProductVersionID", sValue = sVersion, bDirty = true
      };
      oBOI.Attributes["sGUID"] = new XIIAttribute {
        sName = "sGUID", sValue = sQuoteGUID, bDirty = true
      };
      Random generator = new Random();
      string sRef = generator.Next(1, 10000000).ToString(new String(''0'', 7));
      oBOI.Attributes["sRefID"] = new XIIAttribute {
        sName = "sRefID", sValue = sPrefix + sRef, bDirty = true
      }; /*oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + Guid.NewGuid().ToString("N").Substring(0, 6), bDirty = true };                      oBOI.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = Guid.NewGuid().ToString("N").Substring(0, 6), bDirty = true };       */
      oBOI.Attributes["BatchID"] = new XIIAttribute {
        sName = "BatchID", sValue = iBatchID, bDirty = true
      };
      oBOI.Attributes["ID"] = new XIIAttribute {
        sName = "ID", sValue = iQuoteID.ToString(), bDirty = true
      };
      oBOI.Attributes["rCompulsoryExcess"] = new XIIAttribute {
        sName = "rCompulsoryExcess", sValue = String.Format("{0:0.00}", rCompulsaryExcess), bDirty = true
      };
      oBOI.Attributes["rVoluntaryExcess"] = new XIIAttribute {
        sName = "rVoluntaryExcess", sValue = String.Format("{0:0.00}", rVoluntaryExcess), bDirty = true
      };
      double rTotalExcess = rCompulsaryExcess + rVoluntaryExcess;
      oBOI.Attributes["rTotalExcess"] = new XIIAttribute {
        sName = "rTotalExcess", sValue = String.Format("{0:0.00}", rTotalExcess), bDirty = true
      };
      Info.Add(oProductI.Attributes["sName"].sValue + "Quote insertion started with the amount of " + total); /*Info.Add("QuoteRefID_" + oBOI.Attributes["sRefID"].sValue);    */
      var oRes = oBOI.Save(oBOI);
      if (oRes.bOK && oRes.oResult != null) {
        oBOI = (XIIBO) oRes.oResult;
      }
      Info.Add(oProductI.Attributes["sName"].sValue + "Quote inserted Sucessfully with the amount of " + total);
      XIDBO oRiskFactorsBOD = (XIDBO) oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
      XIIBO oBO = new XIIBO();
      oBO.BOD = oRiskFactorsBOD;
      List < XIIBO > oBOIList = new List < XIIBO > ();
      foreach(var item in oGeneralDeclines) {
        oBO = new XIIBO();
        oBO.BOD = oRiskFactorsBOD;
        oBO.Attributes["FKiQuoteID"] = new XIIAttribute {
          sName = "FKiQuoteID", sValue = oBOI.Attributes["ID"].sValue, bDirty = true
        };
        oBO.Attributes["sFactorName"] = new XIIAttribute {
          sName = "sFactorName", sValue = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault(), bDirty = true
        };
        oBO.Attributes["sValue"] = new XIIAttribute {
          sName = "sValue", sValue = item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), bDirty = true
        };
        oBO.Attributes["sMessage"] = new XIIAttribute {
          sName = "sMessage", sValue = item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault(), bDirty = true
        };
        oBO.Attributes["CreatedTime"] = new XIIAttribute {
          sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true
        };
        oBO.Attributes["FKsQuoteID"] = new XIIAttribute {
          sName = "FKsQuoteID", sValue = sQuoteGUID, bDirty = true
        };
        oBO.Attributes["ID"] = new XIIAttribute {
          sName = "ID", bDirty = true
        };
        oBOIList.Add(oBO);
      }
      if (oBOIList.Count > 0) {
        XIIBO xibulk = new XIIBO();
        DataTable dtbulk = xibulk.MakeBulkSqlTable(oBOIList);
        xibulk.SaveBulk(dtbulk, oBOIList[0].BOD.iDataSourceXIGUID.ToString(), oBOIList[0].BOD.TableName);
      }
      oCResult.oCollectionResult.Add(new CNV {
        sName = "QuoteID", sValue = oBOI.Attributes["ID"].sValue
      });
      oCResult.sMessage = "Success";
      oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
      oCResult.oResult = "Success";
      string sInfo = "INFO: " + string.Join(",\r\n ", Info);
      oCResult.sMessage = sInfo;
      oIB.SaveErrortoDB(oCResult);
      oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
      oCResult.oResult = total;
    } catch (Exception ex) {
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
 N'2021-12-06 10:43:44.000',
 N'174',
 NULL,
 N'174',
 NULL,
 N'2021-12-06 10:43:44.000',
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
 N'87d3838c-af98-4630-8bfe-b79ac1ba744f',
 N'0',
 N'',
 N'',
 N'2021-12-06 10:43:44.000',
 N'2021-12-06 10:43:44.000',
 NULL,
 NULL,
 NULL,
 N'610981b2-35c6-483e-bdee-5a08e5cbcf5f',
 NULL,
 N'2b4445fc-c977-40d4-9590-5470df8c7e02',
 NULL,
 NULL)
