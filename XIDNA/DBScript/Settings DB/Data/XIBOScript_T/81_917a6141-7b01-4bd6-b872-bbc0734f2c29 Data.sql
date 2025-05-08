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
 [FKiVersionIDXIGUID]) Values(N'81',
 N'296',
 N'0',
 NULL,
 N'HDFC MTA Copy',
 N'HDFC MTA',
 N'        public static CResult PolicyMainCal(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            PolicyCalculation Pcal = new PolicyCalculation();
            CResult oResult = new CResult();
            oResult.sMessage = "HDFC MTA script running";
            oIB.SaveErrortoDB(oResult);
            CNV oNV = new CNV();
            oNV.sName = "sCode";
            try
            {
                string QuoteID = lParam.Where(m => m.sName == "iQuoteID").FirstOrDefault().sValue;
   oResult.sMessage = "iQuoteID:"+QuoteID;
                oIB.SaveErrortoDB(oResult);
                int iInsatnceID = Convert.ToInt32(lParam.Where(m => m.sName == "iInsatnceID").FirstOrDefault().sValue);
  oResult.sMessage = "iInsatnceID:"+iInsatnceID;
                oIB.SaveErrortoDB(oResult);
                string dCoverStart = lParam.Where(m => m.sName == "dCoverStart").FirstOrDefault().sValue;
  oResult.sMessage = "dCoverStart:"+dCoverStart;
                oIB.SaveErrortoDB(oResult);
                string rGrossPremium = lParam.Where(m => m.sName == "rGrossPremium").FirstOrDefault().sValue;
  oResult.sMessage = "rGrossPremium:"+rGrossPremium;
                oIB.SaveErrortoDB(oResult);
                oResult.sMessage = "In Script: Questionset object Loaded from cache";
                oIB.SaveErrortoDB(oResult);
                oResult = Pcal.Calculation(iInsatnceID, dCoverStart, rGrossPremium, QuoteID);
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
            public CResult GetAdditionalCharges()
            {
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
                    oNV2.sValue = "AdditionalCharges";
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oresult.LogToFile();
                }
                return oresult;
            }
        }
        public class PolicyCalculation
        {
            public CResult Calculation(int iInsatnceID, string dCoverStart, string rGrossPremium, string QuoteID)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                try
                {
                    XIDXI oXIDXI = new XIDXI();
                    PolicyBaseCalc PolicyBaseCalc = new PolicyBaseCalc();
                    XIIXI oIXI = new XIIXI();
                    XIIBO oBII = new XIIBO();
                    oBII.BOD = (XIDBO)oXIDXI.Get_BODefinition("Aggregations").oResult;
                    var result = new CResult();
                    List<CResult> ConvictionResult = new List<CResult>();
                    var oQSI = oIXI.BOI("QS Instance", iInsatnceID.ToString()).Structure("NotationStructure").XILoad();
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    //var oLIst = oQSI.oSubStructureI("Driver_T");
                    string dtInsuranceCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;
                    var days = Convert.ToDateTime(dtInsuranceCoverStartDate) - Convert.ToDateTime(dCoverStart);
                    var leftoverdays = 365 - Convert.ToInt32(days.TotalDays);
                    var amount = (Convert.ToDouble(rGrossPremium) / 365) * days.TotalDays;
                    var BalAmount = Convert.ToDouble(rGrossPremium) - amount;
                    double total = 0.0;
                    var oQuoteI = oIXI.BOI("Aggregations", QuoteID);
                    var QuotePrice= oQuoteI.Attributes["rPrice"].sValue;
                    if(double.TryParse(QuotePrice ,out total))
                    { }
                    total = total / 365 * leftoverdays - BalAmount;
                    var oAdditionalCharges = PolicyBaseCalc.GetAdditionalCharges();
                    if (oAdditionalCharges.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess && oAdditionalCharges.oCollectionResult != null)
                    {
                        var AdditionalCharges = Convert.ToDouble(oAdditionalCharges.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                        total = AdditionalCharges + total;
                    }
                    if (total < -25)
                    {
                        total = 0.0;
                    }
                oBII.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = total.ToString(), bDirty = true };
                    //oBII.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString("dd-MMM-yyyy"), bDirty = true };
                    oBII.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = QuoteID, bDirty = true };
                    oBII = oBII.Save(oBII);
                    oResult.sMessage = "HDFC MTA Quote inserted Sucessfully with the amount of " + total;
                    oIB.SaveErrortoDB(oResult);
                    oResult.sMessage = "Success";
                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oResult.oResult = "Success";
                }
                catch (Exception ex)
                {
                    oResult.sMessage = ex.ToString();
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
 N'2018-12-14 15:50:45.000',
 N'6',
 N'192.168.7.6',
 N'6',
 N'192.168.7.6',
 N'2018-12-14 15:50:45.000',
 N'15',
 N'HDFC MTA',
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
 N'917a6141-7b01-4bd6-b872-bbc0734f2c29',
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
