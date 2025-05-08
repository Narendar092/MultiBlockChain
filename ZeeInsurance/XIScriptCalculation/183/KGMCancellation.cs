using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace ZeeInsurance.XIScriptCalculation._183
{
    public class KGMCancellation
    {
        public static CResult PolicyMainCal(List<CNV> lParam)
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
                int iInsatnceID = Convert.ToInt32(lParam.Where(m => m.sName == "iInsatnceID").FirstOrDefault().sValue);
                string dCoverStart = lParam.Where(m => m.sName == "dCoverStart").FirstOrDefault().sValue;
                string rGrossPremium = lParam.Where(m => m.sName == "rGrossPremium").FirstOrDefault().sValue;
                //oResult.sMessage = "In Script: Questionset object Loaded from cache";
                //oIB.SaveErrortoDB(oResult);
                oResult = Pcal.Calculation(iInsatnceID, dCoverStart, rGrossPremium);
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
            public CResult GetCancellationScales(DateTime PolicyAppliedDate)
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
                    DateTime CurrentDate = DateTime.Now;
                    
                    var Date = Convert.ToInt32((CurrentDate - PolicyAppliedDate).TotalDays);
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
            public CResult Calculation(int iInsatnceID, string dCoverStart, string rGrossPremium)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                try
                {
                    double total = 0.0;
                    XIDXI oXIDXI = new XIDXI();
                    PolicyBaseCalc PolicyBaseCalc = new PolicyBaseCalc();
                    XIIBO oBII = new XIIBO();
                    //oBII.BOD = (XIDBO)oXIDXI.Get_BODefinition("CancelPolicy_T").oResult;
                    DateTime dtcoverStart = new DateTime();
                    if (DateTime.TryParse(dCoverStart, out dtcoverStart))
                    {
                        var oCancelResult = PolicyBaseCalc.GetCancellationScales(dtcoverStart);
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
                    XIIXI oXII = new XIIXI();
                    var oCancelI = oXII.BOI("CancelPolicy_T", iInsatnceID.ToString(), "Create");
                    if (oCancelI.Attributes.ContainsKey("rCancelRate"))
                    {
                        oCancelI.Attributes["rCancelRate"].sValue = total.ToString();
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
        }
    }
}