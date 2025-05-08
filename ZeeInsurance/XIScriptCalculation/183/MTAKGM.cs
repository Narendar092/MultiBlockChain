using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using XICore;
using XISystem;

namespace ZeeInsurance.XIScriptCalculation._183
{
    public class MTAKGM
    {
        public static CResult PolicyMainCal(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            PolicyCalculation Pcal = new PolicyCalculation();
            CResult oResult = new CResult();
            oResult.sMessage = "MTA KGM script running";
            oIB.SaveErrortoDB(oResult);
            CNV oNV = new CNV();
            oNV.sName = "sCode";
            try
            {
                string QuoteID = lParam.Where(m => m.sName == "iQuoteID").FirstOrDefault().sValue;
                oResult.sMessage = "iQuoteID:" + QuoteID;
                oIB.SaveErrortoDB(oResult);
                int iInsatnceID = Convert.ToInt32(lParam.Where(m => m.sName == "iInsatnceID").FirstOrDefault().sValue);
                oResult.sMessage = "iInsatnceID:" + iInsatnceID;
                oIB.SaveErrortoDB(oResult);
                string dCoverStart = lParam.Where(m => m.sName == "dCoverStart").FirstOrDefault().sValue;
                oResult.sMessage = "dCoverStart:" + dCoverStart;
                oIB.SaveErrortoDB(oResult);
                string rGrossPremium = lParam.Where(m => m.sName == "rGrossPremium").FirstOrDefault().sValue;
                oResult.sMessage = "rGrossPremium:" + rGrossPremium;
                oIB.SaveErrortoDB(oResult);
                oResult.sMessage = "In Script: Questionset object Loaded from cache";
                oIB.SaveErrortoDB(oResult);
                string sSessionID = lParam.Where(m => m.sName == "sSessionID").FirstOrDefault().sValue;
                string sUID = lParam.Where(m => m.sName == "sUID").FirstOrDefault().sValue;
                string sVersion = lParam.Where(m => m.sName == "Version").FirstOrDefault().sValue;
                oResult = Pcal.Calculation(iInsatnceID, dCoverStart, rGrossPremium, QuoteID, sSessionID, sUID, sVersion);
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
            public CResult Calculation(int iInsatnceID, string dCoverStart, string rGrossPremium, string QuoteID, string sSessionID, string sGUID, string sVersion)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                try
                {
                    XIInfraCache oCache = new XIInfraCache();
                    XIDXI oXIDXI = new XIDXI();
                    PolicyBaseCalc PolicyBaseCalc = new PolicyBaseCalc();
                    XIIXI oIXI = new XIIXI();
                    XIIBO oBII = new XIIBO();
                    oBII.BOD = (XIDBO)oXIDXI.Get_BODefinition("Aggregations").oResult;
                    var result = new CResult();
                    List<CResult> ConvictionResult = new List<CResult>();
                    var oQSI = oCache.Get_QsStructureObj(sSessionID, sGUID, "QSInstance_" + iInsatnceID + "NotationStructure");
                    var AddonPrice = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|rAddon}");
                    var AddonAdminPrice = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|rAddonAdmin}");
                    double rAddon = 0;
                    double rAddonAdmin = 0;
                    if (double.TryParse(AddonPrice, out rAddon)) { }
                    oResult.sMessage = "Total Addon Price" + AddonPrice;
                    oIB.SaveErrortoDB(oResult);
                    if (double.TryParse(AddonAdminPrice, out rAddonAdmin)) { }
                    oResult.sMessage = "Total Addon Admin Price" + AddonAdminPrice;
                    oIB.SaveErrortoDB(oResult);
                    //var oQSI = oIXI.BOI("QS Instance", iInsatnceID.ToString()).Structure("NotationStructure").XILoad();
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    //var oLIst = oQSI.oSubStructureI("Driver_T");
                    string dtInsuranceCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;
                    oResult.sMessage = "dCover Start:" + dtInsuranceCoverStartDate;
                    oIB.SaveErrortoDB(oResult);
                    oResult.sMessage = "Policy date:" + dCoverStart;
                    oIB.SaveErrortoDB(oResult);
                    var days = Convert.ToDateTime(dtInsuranceCoverStartDate) - Convert.ToDateTime(dCoverStart);
                    oResult.sMessage = "Diff Days:" + days;
                    oIB.SaveErrortoDB(oResult);
                    var leftoverdays = 365 - Convert.ToInt32(days.TotalDays);
                    var amount = (Convert.ToDouble(rGrossPremium) / 365) * days.TotalDays;
                    var BalAmount = Convert.ToDouble(rGrossPremium) - amount;
                    double total = 0.0;
                    var oQuoteI = oIXI.BOI("Aggregations", QuoteID);
                    XIIBO oBOI2 = new XIIBO();
                    oBOI2.iBODID = oQuoteI.iBODID;
                    oQuoteI.Attributes["iQuoteStatus"].BOI = oBOI2;
                    var QuoteStatus = oQuoteI.AttributeI("iQuoteStatus").sResolvedValue;
                    oResult.sMessage = "Quote Status" + QuoteStatus;
                    oIB.SaveErrortoDB(oResult);
                    if (QuoteStatus == "Normal")
                    {
                        var QuotePrice = oQuoteI.Attributes["rFinalQuote"].sValue;
                        oResult.sMessage = "KGM QuotePrice" + QuotePrice;
                        oIB.SaveErrortoDB(oResult);
                        if (double.TryParse(QuotePrice, out total)) { }
                        total += rAddon + rAddonAdmin;
                        oResult.sMessage = "KGM Quote total" + total;
                        oIB.SaveErrortoDB(oResult);
                        oResult.sMessage = "leftoverdays:" + leftoverdays;
                        oIB.SaveErrortoDB(oResult);
                        oResult.sMessage = "BalAmount:" + BalAmount;
                        oIB.SaveErrortoDB(oResult);
                        if (total > 0)
                        {
                            total = total / 365 * leftoverdays - BalAmount;
                            oResult.sMessage = "MTA Premium:" + total;
                            oIB.SaveErrortoDB(oResult);
                            var oAdditionalCharges = PolicyBaseCalc.GetAdditionalCharges();
                            if (oAdditionalCharges.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess && oAdditionalCharges.oCollectionResult != null)
                            {
                                var AdditionalCharges = Convert.ToDouble(oAdditionalCharges.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                                total = AdditionalCharges + total;
                            }
                            oResult.sMessage = "KGM MTA Cost " + total;
                            oIB.SaveErrortoDB(oResult);
                            if (total >= -25 && total <= 0)
                            {
                                total = 0.0;
                            }
                        }
                        var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
                        int iProductID = 0;
                        string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                        if (int.TryParse(ProductID, out iProductID)) { }
                        XIAPI oXIAPI = new XIAPI();
                        double rPaymentCharge = 0; double rInsurerCharge = 0; double rAdmin = 0;
                        if (double.TryParse(oQuoteI.Attributes["rPaymentCharge"].sValue, out rPaymentCharge)) { }
                        if (double.TryParse(oQuoteI.Attributes["rInsurerCharge"].sValue, out rInsurerCharge)) { }
                        oBII.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = total.ToString(), bDirty = true };
                        oBII.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = total.ToString(), bDirty = true };
                        var MinimumDeposit = oXIAPI.GetMinimumDepostAmount(rPaymentCharge, rInsurerCharge, total, rAdmin, sGUID, iInsatnceID, iProductID, 0, 0);
                        double rMinDeposit = 0;
                        if (double.TryParse(MinimumDeposit, out rMinDeposit)) { }
                        var MonthlyAmount = oXIAPI.GetMonthlyPremiumAmount(total, rMinDeposit, iProductID, 0, 0);
                        double rMonthlyTotal = 0; double rPFAmount = 0;
                        rMonthlyTotal = (MonthlyAmount * 10) + rMinDeposit;
                        oBII.Attributes["zDefaultDeposit"] = new XIIAttribute { sName = "zDefaultDeposit", sValue = MinimumDeposit, bDirty = true };
                        oBII.Attributes["rMonthlyPrice"] = new XIIAttribute { sName = "rMonthlyPrice", sValue = MonthlyAmount.ToString(), bDirty = true };
                        oBII.Attributes["rMonthlyTotal"] = new XIIAttribute { sName = "rMonthlyTotal", sValue = rMonthlyTotal.ToString("F"), bDirty = true };
                        rPFAmount = rMonthlyTotal - rMinDeposit;
                        oBII.Attributes["rPremiumFinanceAmount"] = new XIIAttribute { sName = "rPremiumFinanceAmount", sValue = rPFAmount.ToString("F"), bDirty = true };
                        //oBII.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString("dd-MMM-yyyy"), bDirty = true };
                        oBII.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = QuoteID, bDirty = true };
                        var oRes = oBII.Save(oBII);
                        oResult.sMessage = "KGM Quote inserted Sucessfully with the amount of " + total;
                        oIB.SaveErrortoDB(oResult);
                        oResult.sMessage = "Success";
                        oResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        oResult.oResult = "Success";
                    }
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