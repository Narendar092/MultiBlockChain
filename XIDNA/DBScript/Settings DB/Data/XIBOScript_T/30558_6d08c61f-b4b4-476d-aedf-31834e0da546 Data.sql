Delete XIBOScript_T where XIGUID='6d08c61f-b4b4-476d-aedf-31834e0da546'
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
 [FKiVersionIDXIGUID]) Values(N'30558',
 N'296',
 N'0',
 NULL,
 N'Lifesure',
 N'Lifesure',
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
        int iOrgID = Convert.ToInt32(lParam.Where(m => m.sName == "iOrgID").FirstOrDefault().sValue);
        int ProductID = Convert.ToInt32(lParam.Where(m => m.sName == "ProductID").FirstOrDefault().sValue);
        //oResult = Pcal.Calculation(iInsatnceID, iUserID, iCustomerID, sDataBase, sProductName, sVersion, sProductCode, sSessionID, sUID, iQuoteID);
        oResult = Pcal.Calculation(iInsatnceID, iUserID, iCustomerID, sVersion, sProductCode, iQuoteID, iOrgID, ProductID);
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

}

public class PolicyCalculation
{
    public CResult Calculation(string iInsatnceID, int iUserID, int iCustomerID, string sVersion, string sProductCode, int iQuoteID, int iOrgID, int ProductID)
    {
        List<string> Info = new List<string>();
        Info.Add("[QsInstanceID_" + iInsatnceID + "]");
        Info.Add("Lifesure script running");
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

            XIIBO oBII = new XIIBO();
            XIAPI oXIAPI = new XIAPI();
            bool bProductValidation = false;
            bool ages = false;
            bool UKlicences = false;
            bool startdt = false;
            bool Valueofyourvehicles = false;
            bool vehicleuses = false;
            bool NCBdoyou = false;
            bool postcode = false;
            bool dailyuses = false;
            bool vehicletobeinsureds = false;
            bool L2Muses = false;
            bool conversions = false;
            bool makeandmodels = false;
            //FIrst page 
            XIInfraCache oCache = new XIInfraCache();
            if (ostructureInstance.XIIValues.Count() > 0)
            {
                if (ostructureInstance.XIIValues.ContainsKey("MTAge"))
                {
                    int age = Convert.ToInt32(ostructureInstance.XIIValues["MTAge"].sValue);
                    if (age >= 30 && age <= 79)
                    {
                        ages = true;
                    }
                    else
                    {
                        bProductValidation = false;
                        var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["MTAge"].sDisplayName);
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
                                    bProductValidation = false;
                                }
                                oXiResults.Add(Steps);
                            }
                        }
                    }
                }
                if (ostructureInstance.XIIValues.ContainsKey("sHowlonghaveyouheldaUKlicence"))
                {
                    var licence = ostructureInstance.XIIValues["sHowlonghaveyouheldaUKlicence"].sResolvedValue;
                    int UKlicence = 0;
					if (licence.Contains("+"))
                    {
                      UKlicence = Convert.ToInt32(ostructureInstance.XIIValues["sHowlonghaveyouheldaUKlicence"].sResolvedValue.Replace(" Years+", ""));
                   }
				   else if (licence == "1 Year")
                    {
                       UKlicence = Convert.ToInt32(ostructureInstance.XIIValues["sHowlonghaveyouheldaUKlicence"].sResolvedValue.Replace(" Year", ""));
                    }
                    else
                    {
                        UKlicence = Convert.ToInt32(ostructureInstance.XIIValues["sHowlonghaveyouheldaUKlicence"].sResolvedValue.Replace(" Years", ""));
                    }
                    if (UKlicence >= 5)
                    {
                        UKlicences = true;
                    }
                    else
                    {
                        bProductValidation = false;
                        var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["sHowlonghaveyouheldaUKlicence"].sDisplayName);
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
                                    bProductValidation = false;
                                }
                                oXiResults.Add(Steps);
                            }
                        }
                    }
                }
                if (ostructureInstance.XIIValues.ContainsKey("Noofdays"))
                {
                    int days = Convert.ToInt32(ostructureInstance.XIIValues["Noofdays"].sValue);
                    if (days < 21)
                    {
                        startdt = true;
                    }
                    else
                    {
                        bProductValidation = false;
                        var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["dWhenwouldyoulikethepolicytostart"].sDisplayName);
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
                                    bProductValidation = false;
                                }
                                oXiResults.Add(Steps);
                            }
                        }
                    }
                }
                if (ostructureInstance.XIIValues.ContainsKey("sPleasecanyouconfirmthevalueofyourvehicle"))
                {
                    var Valueofyourvehicle = ostructureInstance.XIIValues["sPleasecanyouconfirmthevalueofyourvehicle"].sResolvedValue;
                    if (Valueofyourvehicle == "Please select" || Valueofyourvehicle == "null") { }
                    else
                    {
                        var value = Valueofyourvehicle.Split(''-'');
                        int min = Convert.ToInt32(value[0].Replace("£", "").Replace("k", ""));
                        int max = Convert.ToInt32(value[1].Replace("£", "").Replace("k", ""));

                        if (min >= 5 && max <= 75)
                        {
                            Valueofyourvehicles = true;
                        }
                        else
                        {
                            bProductValidation = false;
                            var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["sPleasecanyouconfirmthevalueofyourvehicle"].sDisplayName);
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
                                        bProductValidation = false;
                                    }
                                    oXiResults.Add(Steps);
                                }
                            }
                        }
                    }
                }
                if (ostructureInstance.XIIValues.ContainsKey("sCanyouconfirmthetypeofuseforthisvehicle?"))
                {
                    int vehicleuse = Convert.ToInt32(ostructureInstance.XIIValues["sCanyouconfirmthetypeofuseforthisvehicle?"].sValue);
                    if (vehicleuse == 30 || vehicleuse == 20)
                    {
                        vehicleuses = true;
                    }
                    else
                    {
                        bProductValidation = false;
                        var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["sCanyouconfirmthetypeofuseforthisvehicle?"].sDisplayName);
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
                                    bProductValidation = false;
                                }
                                oXiResults.Add(Steps);
                            }
                        }
                    }
                }
                if (ostructureInstance.XIIValues.ContainsKey("sPleasecanyouconfirmhowmanyyearsNCBdoyouhave"))
                {
                    int NCBdoyouhave = 0;
                    var Ncb = ostructureInstance.XIIValues["sPleasecanyouconfirmhowmanyyearsNCBdoyouhave"].sResolvedValue;
                    if (Ncb.Contains("+"))
                    {
                        NCBdoyouhave = Convert.ToInt32(ostructureInstance.XIIValues["sPleasecanyouconfirmhowmanyyearsNCBdoyouhave"].sResolvedValue.Replace(" +Years", ""));
                    }
					else if (Ncb == "1 Year")
                    {
                        NCBdoyouhave = Convert.ToInt32(ostructureInstance.XIIValues["sPleasecanyouconfirmhowmanyyearsNCBdoyouhave"].sResolvedValue.Replace(" Year", ""));
                    }
                    else
                    {
                        NCBdoyouhave = Convert.ToInt32(ostructureInstance.XIIValues["sPleasecanyouconfirmhowmanyyearsNCBdoyouhave"].sResolvedValue.Replace(" Years", ""));
                    }
                    if (NCBdoyouhave >= 5)
                    {
                        NCBdoyou = true;
                    }
                    else
                    {
                        bProductValidation = false;
                        var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["sPleasecanyouconfirmhowmanyyearsNCBdoyouhave"].sDisplayName);
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
                                    bProductValidation = false;
                                }
                                oXiResults.Add(Steps);
                            }
                        }
                    }
                }
                if (ostructureInstance.XIIValues.ContainsKey("MTPostcode"))
                {
                    XID1Click PV1Click = new XID1Click();
                    var PurchaseQuery = "select* From ProductPostCodeexclusion_T where ''" + ostructureInstance.XIIValues["MTPostcode"].sValue + "'' like sPostCode +''%'' and FKiProductID = " + ProductID + " and FKiOrgID=" + iOrgID + "";
                    PV1Click.Query = PurchaseQuery;
                    PV1Click.Name = "ProductPostCodeexclusion_T";
                    var Result = PV1Click.OneClick_Execute();
                    if (Result.Count() == 0)
                    {
                        postcode = true;
                    }
                    else
                    {
                        bProductValidation = false;
                        var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["MTPostcode"].sDisplayName);
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
                                    bProductValidation = false;
                                }
                                oXiResults.Add(Steps);
                            }
                        }
                    }
                }
                if (ostructureInstance.XIIValues.ContainsKey("sIsthemotorhome/campervanyourprimaryvehiclefordailyuse"))
                {
                    var dailyuse = ostructureInstance.XIIValues["sIsthemotorhome/campervanyourprimaryvehiclefordailyuse"].sResolvedValue;
                    if (dailyuse == "No")
                    {
                        dailyuses = true;
                    }
                    else
                    {
                        bProductValidation = false;
                        var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["sIsthemotorhome/campervanyourprimaryvehiclefordailyuse"].sDisplayName);
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
                                    bProductValidation = false;
                                }
                                oXiResults.Add(Steps);
                            }
                        }
                    }
                }
                if (ostructureInstance.XIIValues.ContainsKey("sCanyouconfirmtheyearofmanufactureageofthevehicletobeinsured"))
                {
                    var vehicletobeinsured = ostructureInstance.XIIValues["sCanyouconfirmtheyearofmanufactureageofthevehicletobeinsured"].sResolvedValue;
					int min1 = 0;
                    int max1 = 0;
                if (vehicletobeinsured.Contains("+"))
                {
                  var values = vehicletobeinsured.Replace(" +Years", "");
                  max1 = Convert.ToInt32(values);
                 }
				 else
				 {
                    var value = vehicletobeinsured.Split(''-'');
                    min1 = Convert.ToInt32(value[0]);
                    max1 = Convert.ToInt32(value[1].Replace("Years", ""));
				 }
                    if (min1 >= 0 && max1 <= 35)
                    {
                        vehicletobeinsureds = true;
                    }
                    else
                    {
                        bProductValidation = false;
                        var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["sCanyouconfirmtheyearofmanufactureageofthevehicletobeinsured"].sDisplayName);
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
                                    bProductValidation = false;
                                }
                                oXiResults.Add(Steps);
                            }
                        }
                    }
                }
                if (ostructureInstance.XIIValues.ContainsKey("sIsthemotorhomecampercurrentlybeingconverted"))
                {
                    var beingconverted = ostructureInstance.XIIValues["sIsthemotorhomecampercurrentlybeingconverted"].sResolvedValue;
                    if (beingconverted == "Yes")
                    {
                        if (ostructureInstance.XIIValues.ContainsKey("sWhenwilltheconversionbecompleted"))
                        {
                            var conversionbecompleted = ostructureInstance.XIIValues["sWhenwilltheconversionbecompleted"].sResolvedValue;
                            if (conversionbecompleted == "Less than 6 months")
                            {
                                conversions = true;
                            }
                            else
                            {
                                bProductValidation = false;
                                var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["sWhenwilltheconversionbecompleted"].sDisplayName);
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
                                            bProductValidation = false;
                                        }
                                        oXiResults.Add(Steps);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        bProductValidation = false;
                        var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["sIsthemotorhomecampercurrentlybeingconverted"].sDisplayName);
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
                                    bProductValidation = false;
                                }
                                oXiResults.Add(Steps);
                            }
                        }
                    }
                }
                if (ostructureInstance.XIIValues.ContainsKey("sCanyouconfirmthemakeandmodel?"))
                {
                    var makeandmodel = ostructureInstance.XIIValues["sCanyouconfirmthemakeandmodel?"].sValue;
                    if (makeandmodel != "Lord Munsterland (LMC)".ToLower())
                    {
                        makeandmodels = true;
                    }
                    else
                    {
                        bProductValidation = false;
                        var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["sCanyouconfirmthemakeandmodel?"].sDisplayName);
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
                                    bProductValidation = false;
                                }
                                oXiResults.Add(Steps);
                            }
                        }
                    }
                }
                if (ostructureInstance.XIIValues.ContainsKey("Campaign Number for L2M use only"))
                {
                    int L2Museonly = Convert.ToInt32(ostructureInstance.XIIValues["Campaign Number for L2M use only"].sValue);
                    if (L2Museonly >= 00 && L2Museonly <= 1919)
                    {
                        L2Muses = true;
                    }
                    else
                    {
                        bProductValidation = false;
                        var Steps = BaseCal.PageValidation(ostructureInstance.XIIValues["Campaign Number for L2M use only"].sDisplayName);
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
                                    bProductValidation = false;
                                }
                                oXiResults.Add(Steps);
                            }
                        }
                    }
                }
            }
            if (ages && UKlicences && startdt && Valueofyourvehicles && vehicleuses && NCBdoyou && postcode && dailyuses && vehicletobeinsureds && L2Muses && conversions && makeandmodels)
            {
                bProductValidation = true;
            }
            else
            {
                bProductValidation = false;
            }
            XIDBO oRiskFactorsBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
            XIIBO oBO = new XIIBO();
            //oBO.BOD = oRiskFactorsBOD;
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
            System.Data.DataTable dtbulk = xibulk.MakeBulkSqlTable(oBOIList);
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
            if (bProductValidation == false)
            {
                oBII.Attributes["iQuoteStatus"] = new XIIAttribute
                {
                    sName = "iQuoteStatus",
                    sValue = "60",
                    bDirty = true
                };
            }
            else
            {
                oBII.Attributes["iQuoteStatus"] = new XIIAttribute
                {
                    sName = "iQuoteStatus",
                    sValue = "50",
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
            //oBII.Attributes["sInsurer"] = new XIIAttribute { sName = "sInsurer", sValue = "", bDirty = true };
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
            //oIB.SaveErrortoDB(oresult, iInsatnceID);
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
 N'6d08c61f-b4b4-476d-aedf-31834e0da546',
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
