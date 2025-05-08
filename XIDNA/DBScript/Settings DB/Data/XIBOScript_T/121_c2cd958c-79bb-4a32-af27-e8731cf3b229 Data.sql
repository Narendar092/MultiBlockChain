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
 [FKiVersionIDXIGUID]) Values(N'121',
 N'0',
 N'0',
 NULL,
 N'KGMCancellation',
 N'cancellation',
 N'        public static CResult PolicyMainCal(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            PolicyCalculation Pcal = new PolicyCalculation();
            CResult oResult = new CResult();
            oResult.sMessage = "KGM cancellation script running";
            oIB.SaveErrortoDB(oResult);
            CNV oNV = new CNV();
            oNV.sName = "sCode";
            try
            {
                string sSessionID = lParam.Where(m => m.sName == "sSessionID").FirstOrDefault().sValue;
                string sUID = lParam.Where(m => m.sName == "sUID").FirstOrDefault().sValue;
                int iInsatnceID = Convert.ToInt32(lParam.Where(m => m.sName == "iInsatnceID").FirstOrDefault().sValue);
                string dCoverStart = lParam.Where(m => m.sName == "dCoverStart").FirstOrDefault().sValue;
                string rGrossPremium = lParam.Where(m => m.sName == "rGrossPremium").FirstOrDefault().sValue;
                //oResult.sMessage = "In Script: Questionset object Loaded from cache";
                //oIB.SaveErrortoDB(oResult);
                oResult = Pcal.Calculation(iInsatnceID, dCoverStart, rGrossPremium,sSessionID,sUID);
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
            public CResult GetCancellationScales(DateTime PolicyAppliedDate,DateTime dtCancelDate)
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
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "CancellationScales";
                    //DateTime CurrentDate = DateTime.Now;
                    var Date = Convert.ToInt32((dtCancelDate - PolicyAppliedDate).TotalDays);
                    if (Date <= 30)
                    {
                        oNV1.sValue = "+40";
                    }
                    else if (Date > 30 && Date <= 60)
                    {
                        oNV1.sValue = "+60";
                    }
                    else if (Date > 60 && Date <= 90)
                    {
                        oNV1.sValue = "+80";
                    }
                    else if (Date > 90)
                    {
                        oNV1.sValue = "+100";
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
                }
                return oresult;
            }
        }
        public class PolicyCalculation
        {
            public CResult Calculation(int iInsatnceID, string dCoverStart, string rGrossPremium,string sSessionID,string sGUID)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                try
                {
                    double total = 0.0;
                    XIDXI oXIDXI = new XIDXI();
                    PolicyBaseCalc PolicyBaseCalc = new PolicyBaseCalc();
                    XIIBO oBII = new XIIBO();
                    XIInfraCache oCache = new XIInfraCache();
                    var QSInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iQSInstanceID}");
                    int iQSInsatnceID = 0;
                    if (int.TryParse(QSInstanceID, out iQSInsatnceID))
                    { }
                    XIIXI oIXI = new XIIXI();
                    XIIQS oQsInstance = oIXI.GetQSXIValuesByQSIID(iQSInsatnceID);
                    var CancelDate = oQsInstance.XIValues["dDate"].sValue;
                    DateTime dtCancelDate = new DateTime();
                    oResult.sMessage = "Policy CancelDate:" + CancelDate;
                    oIB.SaveErrortoDB(oResult);
                    if (DateTime.TryParse(CancelDate, out dtCancelDate))
                    {
                    }
      XIIXI oXII = new XIIXI();
                    var oCancelI = oXII.BOI("CancelPolicy_T", iInsatnceID.ToString(), "Create");
     if (oCancelI.Attributes.ContainsKey("FKiACPolicyID"))
                    {
                        var oPolicyI = oXII.BOI("ACPolicy_T", oCancelI.Attributes["FKiACPolicyID"].sValue, "Create");
                        if (oPolicyI.Attributes.ContainsKey("dCurrentPolicyOnCover"))
                        {
       dCoverStart=oPolicyI.Attributes["dCurrentPolicyOnCover"].sValue;
                        }
     }
                    //oBII.BOD = (XIDBO)oXIDXI.Get_BODefinition("CancelPolicy_T").oResult;
                    DateTime dtcoverStart = new DateTime();
                    if (DateTime.TryParse(dCoverStart, out dtcoverStart))
                    {
                        var oCancelResult = PolicyBaseCalc.GetCancellationScales(dtcoverStart, dtCancelDate);
                        if (oCancelResult.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess && oCancelResult.oCollectionResult != null)
                        {
                            var CancellationCharges = Convert.ToDouble(oCancelResult.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                            double rPremium = 0.00;
                            if (double.TryParse(rGrossPremium, out rPremium))
                            {
                                total = (rPremium * CancellationCharges * 0.01);
                                total = rPremium - total;
                            }
                        }
                    }
                   
                    if (oCancelI.Attributes.ContainsKey("rCancelRate"))
                    {
                        oCancelI.Attributes["rCancelRate"].sValue = String.Format("{0:0.00}", total);
                    }
                    oCancelI.Attributes.ToList().ForEach(m => m.Value.bDirty = true);
                    var ores = oBII.Save(oCancelI);
                    if (ores.bOK && ores.oResult != null)
                    {
                        oBII = (XIIBO)ores.oResult;
                    }
                    // oResult.sMessage = "KGM Quote inserted Sucessfully with the amount of " + total;
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
 N'2021-03-25 13:57:16.000',
 N'6',
 N'fe80::1584:fa50:8b91:a35d%9',
 N'174',
 N'fe80::1c07:2220:960d:8e3f%17',
 N'2021-03-25 13:57:16.000',
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
 N'c2cd958c-79bb-4a32-af27-e8731cf3b229',
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
