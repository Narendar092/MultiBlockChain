using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace ZeeInsurance
{
    public class TowerGate
    {
        public static CResult CaravanCalculation(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            TouringCaravan oTCaravan = new TouringCaravan();
            //XIIQS oQsInstance = new XIIQS();
            CResult oResult = new CResult();
            oResult.sMessage = "Towergate Version1 script running";
            oIB.SaveErrortoDB(oResult);
            CNV oNV = new CNV();
            oNV.sName = "sCode";
            try
            {
                string sGUID = lParam.Where(m => m.sName == "sUID").FirstOrDefault().sValue;
                int iInstanceID = Convert.ToInt32(lParam.Where(m => m.sName == "iInsatnceID").FirstOrDefault().sValue);
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
                oResult = oTCaravan.GetCaravanFinalPremium(sGUID, iInstanceID, iUserID, sProductName, sVersion, sSessionID, iCustomerID, iQuoteID);
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
        public class TouringCaravan
        {
            //public static double CaravanSI = 12000;
            //public static double ContentsSI = 1000;
            //public static double EquipmentSI = 1000;
            //public static double AwningSI = 250;
            public static double CaravanSI = 0;
            public static double ContentsSI = 0;
            public static double EquipmentSI = 0;
            public static double AwningSI = 0;
            public static double CorePremium = 0;
            //public static string Touring = "Basic";
            public static string Touring = string.Empty;
            public static string PrimaryStoragePostCode = "";
            public static double IPTRate = 12;
            public static string PolicyStartDate = "";
            public List<CResult> oXiResults = new List<CResult>();
            List<CResult> oGeneralRefers = new List<CResult>();
            //Touring Caravan
            public CResult GetTouringCaravanPremium(List<CNV> oParams)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    //string sDriverType = "Any Driver";
                    //string sGeographicalLimit = "UK";
                    //string sMileage = "Limited";
                    //string sRegularService = "Y";
                    //string CaravanAge = "2";
                    //string sType = Touring;
                    //string sResult = string.Empty;
                    //int iMileage = 0;
                    //if (int.TryParse(sMileage, out iMileage)) { }
                    string sDriverType = oParams.Where(x => x.sName == "sDriverType").Select(x => x.sValue).FirstOrDefault();
                    string sGeographicalLimit = oParams.Where(x => x.sName == "sGeographicalLimit").Select(x => x.sValue).FirstOrDefault();
                    string sMileage = oParams.Where(x => x.sName == "sMileage").Select(x => x.sValue).FirstOrDefault();
                    string sRegularService = oParams.Where(x => x.sName == "sRegularService").Select(x => x.sValue).FirstOrDefault();
                    string CaravanAge = oParams.Where(x => x.sName == "CaravanAge").Select(x => x.sValue).FirstOrDefault();
                    string sRateTouring = string.Empty; string sMileageDiscount = string.Empty; string sServiceDiscount = string.Empty;
                    double dMileageDiscount = 0; double dServiceDiscount = 0; double dRateTouring = 0;

                    var oRateTouring = GetRateTouring(Touring, sGeographicalLimit, sDriverType);
                    if (oRateTouring.bOK && oRateTouring.oResult != null)
                    {
                        sRateTouring = (string)oRateTouring.oResult;
                        if (oRateTouring.oCollectionResult != null && oRateTouring.oCollectionResult.Count() > 0)
                        {
                            oXiResults.Add(oRateTouring);
                        }
                    }
                    var oMileageDiscount = GetMileageDiscount(sMileage);
                    if (oMileageDiscount.bOK && oMileageDiscount.oResult != null)
                    {
                        sMileageDiscount = (string)oMileageDiscount.oResult;
                        if (oMileageDiscount.oCollectionResult != null && oMileageDiscount.oCollectionResult.Count() > 0)
                        {
                            oXiResults.Add(oMileageDiscount);
                        }
                    }
                    CResult oServiceDiscount = new CResult();
                    if (Convert.ToInt32(CaravanAge) <= 2 || (!string.IsNullOrEmpty(sRegularService) && sRegularService.ToLower() == "yes"))
                    {
                        oServiceDiscount = GetServicedDiscount(sRegularService);
                        if (oServiceDiscount.bOK && oServiceDiscount.oResult != null)
                        {
                            sServiceDiscount = (string)oServiceDiscount.oResult;
                            if (oServiceDiscount.oCollectionResult != null && oServiceDiscount.oCollectionResult.Count() > 0)
                            {
                                oXiResults.Add(oServiceDiscount);
                            }
                        }
                    }
                    if (double.TryParse(sMileageDiscount, out dMileageDiscount)) { }
                    if (double.TryParse(sServiceDiscount, out dServiceDiscount)) { }
                    if (double.TryParse(sRateTouring, out dRateTouring)) { }
                    var sDiscountTouringTotal = dMileageDiscount + dServiceDiscount;
                    var sPremiumTouringCover = CorePremium * (dRateTouring / 1000);
                    var sPremiumDiscountedTouring = (sPremiumTouringCover / 100) * (100 - sDiscountTouringTotal);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sPremiumDiscountedTouring;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }

            //Material Damage
            public CResult GetAccidentalDamagePremium(List<CNV> oParams)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sAccidentalDamageCover = oParams.Where(x => x.sName == "sAccidentalDamageCover").Select(x => x.sValue).FirstOrDefault();
                    // var sAccidentalDamageCover = "Yes";
                    double iPremiumAccidentalDamage = 0;
                    string sRateAccidentalDamage = string.Empty; double dRateAccidentalDamage = 0;

                    var oAccidentalDamageCover = GetRateAccidentalDamage(sAccidentalDamageCover, Touring);
                    if (oAccidentalDamageCover.bOK && oAccidentalDamageCover.oResult != null)
                    {
                        sRateAccidentalDamage = (string)oAccidentalDamageCover.oResult;
                        if (oAccidentalDamageCover.oCollectionResult != null && oAccidentalDamageCover.oCollectionResult.Count() > 0)
                        {
                            oXiResults.Add(oAccidentalDamageCover);
                        }
                    }
                    if (double.TryParse(sRateAccidentalDamage, out dRateAccidentalDamage)) { }
                    iPremiumAccidentalDamage = CorePremium * (dRateAccidentalDamage / 1000);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = iPremiumAccidentalDamage;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetTouringCaravanTotalPremium(List<CNV> oParams)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sSecurityLevel = oParams.Where(x => x.sName == "iSecurityLevel").Select(x => x.sValue).FirstOrDefault();
                    //string sResult = string.Empty;
                    //string sSecurityvan = string.Empty;
                    //string sSecurityStorage = string.Empty;
                    string sCaravanCoreRate = string.Empty; string sCaravanTheftRate = string.Empty;
                    //int iSecurityvan = 0; int iSecurityStorage = 0;
                    int iSecurityLevel = 0; double dCaravanCoreRate = 0; double dCaravanTheftRate = 0;
                    if (int.TryParse(sSecurityLevel, out iSecurityLevel)) { }
                    //if (int.TryParse(sSecurityvan, out iSecurityvan)) { }
                    //if (int.TryParse(sSecurityStorage, out iSecurityStorage)) { }
                    //var sSecurity = iSecurityvan + iSecurityStorage;
                    //if (sSecurity < 2)
                    //{
                    //    iSecurityLevel = 0;
                    //}
                    //else if (sSecurity == 2)
                    //{
                    //    iSecurityLevel = 1;
                    //}
                    //else
                    //{
                    //    iSecurityLevel = 2;
                    //}
                    var oCaravanCoreRate = GetCaravanCoreRate(Touring);
                    if (oCaravanCoreRate.bOK && oCaravanCoreRate.oResult != null)
                    {
                        sCaravanCoreRate = (string)oCaravanCoreRate.oResult;
                        if (oCaravanCoreRate.oCollectionResult != null && oCaravanCoreRate.oCollectionResult.Count() > 0)
                        {
                            oXiResults.Add(oCaravanCoreRate);
                        }
                    }
                    var oCaravanTheftRate = GetCaravanTheftRate(Touring, iSecurityLevel);
                    if (oCaravanTheftRate.bOK && oCaravanTheftRate.oResult != null)
                    {
                        sCaravanTheftRate = (string)oCaravanTheftRate.oResult;
                        if (oCaravanTheftRate.oCollectionResult != null && oCaravanTheftRate.oCollectionResult.Count() > 0)
                        {
                            oXiResults.Add(oCaravanTheftRate);
                        }
                    }
                    if (double.TryParse(sCaravanCoreRate, out dCaravanCoreRate)) { }
                    if (double.TryParse(sCaravanTheftRate, out dCaravanTheftRate)) { }
                    var iPremiumCaravanCore = (CorePremium * dCaravanCoreRate) / 1000;
                    var iPremiumCaravanTheft = (CorePremium * dCaravanTheftRate) / 1000;
                    var iPremiumTotalCaravan = iPremiumCaravanCore + iPremiumCaravanTheft;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = iPremiumTotalCaravan;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetContentsTotalPremium(List<CNV> oParams)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    int iSecurityLevel = 0;
                    var sSecurityLevel = oParams.Where(x => x.sName == "iSecurityLevel").Select(x => x.sValue).FirstOrDefault();
                    if (int.TryParse(sSecurityLevel, out iSecurityLevel)) { }
                    double dContentRate = 0;// decimal rContentRate = 0;
                    string sResult = string.Empty; string sContentRate = string.Empty;
                    var oContentsRate = GetContentsRate(iSecurityLevel, Touring);
                    if (oContentsRate.bOK && oContentsRate.oResult != null)
                    {
                        sContentRate = (string)oContentsRate.oResult;
                        if (oContentsRate.oCollectionResult != null && oContentsRate.oCollectionResult.Count() > 0)
                        {
                            oXiResults.Add(oContentsRate);
                        }

                    }
                    if (double.TryParse(sContentRate, out dContentRate)) { }
                    //if (decimal.TryParse(sContentRate, out rContentRate)) { }
                    var iPremiumEquipment = EquipmentSI * dContentRate / 1000;
                    var iPremiumContentsPart = ((ContentsSI - 1000) * dContentRate) / 1000;
                    var iPremiumTotalContents = iPremiumEquipment + iPremiumContentsPart;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = iPremiumTotalContents;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetAwningTotalPremium(List<CNV> oParams)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    int iProposerAge = 0;
                    var sProposerAge = oParams.Where(x => x.sName == "iProposerAge").Select(x => x.sValue).FirstOrDefault();
                    if (int.TryParse(sProposerAge, out iProposerAge)) { }
                    double dAwningRate = 0;
                    string sResult = string.Empty; string sAwningRate = string.Empty;
                    var oAwningRate = GetAwningRate(iProposerAge, Touring);
                    if (oAwningRate.bOK && oAwningRate.oResult != null)
                    {
                        sAwningRate = (string)oAwningRate.oResult;
                        if (oAwningRate.oCollectionResult != null && oAwningRate.oCollectionResult.Count() > 0)
                        {
                            oXiResults.Add(oAwningRate);
                        }
                    }
                    if (double.TryParse(sAwningRate, out dAwningRate)) { }
                    double dAwningTotalPremium = dAwningRate * (AwningSI / 1000);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = dAwningTotalPremium;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetMaterialDamageTotalPremium(List<CNV> oParams)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    double dAccidentalDamagePremium = 0; double dCaravanTotalPremium = 0; double dContentsTotalPremium = 0; double dAwningTotalPremium = 0;
                    double dDiscountSize = 0;
                    var oAccidentalDamagePremium = GetAccidentalDamagePremium(oParams);
                    if (oAccidentalDamagePremium.bOK && oAccidentalDamagePremium.oResult != null)
                    {
                        dAccidentalDamagePremium = (double)oAccidentalDamagePremium.oResult;
                        //if (double.TryParse(sAccidentalDamagePremium, out dAccidentalDamagePremium)) { }
                    }
                    var oCaravanTotalPremium = GetTouringCaravanTotalPremium(oParams);
                    if (oCaravanTotalPremium.bOK && oCaravanTotalPremium.oResult != null)
                    {
                        dCaravanTotalPremium = (double)oCaravanTotalPremium.oResult;
                        //if (double.TryParse(sCaravanTotalPremium, out dCaravanTotalPremium)) { }
                    }
                    var oContentsTotalPremium = GetContentsTotalPremium(oParams);
                    if (oContentsTotalPremium.bOK && oContentsTotalPremium.oResult != null)
                    {
                        dContentsTotalPremium = (double)oContentsTotalPremium.oResult;
                        //if (double.TryParse(sContentsTotalPremium, out dContentsTotalPremium)) { }
                    }
                    var oAwningTotalPremium = GetAwningTotalPremium(oParams);
                    if (oAwningTotalPremium.bOK && oAwningTotalPremium.oResult != null)
                    {
                        dAwningTotalPremium = (double)oAwningTotalPremium.oResult;
                        //if (double.TryParse(sAwningTotalPremium, out dAwningTotalPremium)) { }
                    }
                    var iMaterialDamagePremium = dAccidentalDamagePremium + dCaravanTotalPremium + dContentsTotalPremium + dAwningTotalPremium;
                    var iTotalSI = CaravanSI + ContentsSI + EquipmentSI + AwningSI;
                    var oDiscountSize = GetDiscountSize(iTotalSI);
                    if (oDiscountSize.bOK && oDiscountSize.oResult != null)
                    {
                        var sDiscountSize = (string)oDiscountSize.oResult;
                        if (oDiscountSize.oCollectionResult != null && oDiscountSize.oCollectionResult.Count() > 0)
                        {
                            oXiResults.Add(oDiscountSize);
                        }

                        if (double.TryParse(sDiscountSize, out dDiscountSize)) { }
                    }
                    var iMaterialDamageTotalPremium = (iMaterialDamagePremium / 100) * (100 - dDiscountSize);
                    var dMaterialDamageTotalPremium = Convert.ToDouble(String.Format("{0:0.00}", iMaterialDamageTotalPremium));
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = dMaterialDamageTotalPremium;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }

            //Rate Discounts(pending)
            public CResult GetRateDiscount(List<CNV> oParams)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sProtectedNCD = oParams.Where(x => x.sName == "sProtectedNCD").Select(x => x.sValue).FirstOrDefault();
                    var sNoClaimYears = oParams.Where(x => x.sName == "iNoClaimYears").Select(x => x.sValue).FirstOrDefault();
                    //var sProtectedNCD = "Y";
                    int iProtectedNCD = 0; int iNoClaimYears = 0;
                    if (int.TryParse(sNoClaimYears, out iNoClaimYears)) { }
                    double iCaravanDiscountedPremium = 0; double iMaterialDamagePremium = 0; double dPremiumSubTotal = 0;
                    double dNoClaimDiscount = 0;
                    var oCaravanDiscountedPremium = GetTouringCaravanPremium(oParams);
                    if (oCaravanDiscountedPremium.bOK && oCaravanDiscountedPremium.oResult != null)
                    {
                        iCaravanDiscountedPremium = (double)oCaravanDiscountedPremium.oResult;
                    }
                    var oMaterialDamageTotalPremium = GetMaterialDamageTotalPremium(oParams);
                    if (oMaterialDamageTotalPremium.bOK && oMaterialDamageTotalPremium.oResult != null)
                    {
                        iMaterialDamagePremium = (double)oMaterialDamageTotalPremium.oResult;
                    }
                    dPremiumSubTotal = iCaravanDiscountedPremium + iMaterialDamagePremium;
                    dPremiumSubTotal = Convert.ToDouble(String.Format("{0:0.00}", dPremiumSubTotal));
                    var oNoClaimDiscount = GetNoClaimDiscount(iNoClaimYears);
                    if (oNoClaimDiscount.bOK && oNoClaimDiscount.oResult != null)
                    {
                        var sNoClaimDiscount = (string)oNoClaimDiscount.oResult;
                        if (double.TryParse(sNoClaimDiscount, out dNoClaimDiscount)) { }
                    }
                    double dNoClaimDiscountPremium = (dPremiumSubTotal / 100) * (100 - dNoClaimDiscount);
                    if (!string.IsNullOrEmpty(sProtectedNCD) && sProtectedNCD.ToLower() == "yes")
                    {
                        var oProtectedNCDLoad = GetProtectedNCDLoad();
                        if (oProtectedNCDLoad.bOK && oProtectedNCDLoad.oResult != null)
                        {
                            sProtectedNCD = (string)oProtectedNCDLoad.oResult;
                        }
                    }
                    else
                    {
                        sProtectedNCD = "0";
                    }
                    if (int.TryParse(sProtectedNCD, out iProtectedNCD)) { }
                    double dProtectedNCDPremium = (dNoClaimDiscountPremium / 100) * (100 + iProtectedNCD);
                    int iDiscountPromoTotal = 1;
                    //Promo
                    double dTotalDiscountedPremium = dProtectedNCDPremium * iDiscountPromoTotal;
                    dTotalDiscountedPremium = Convert.ToDouble(String.Format("{0:0.00}", dTotalDiscountedPremium));
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = dTotalDiscountedPremium;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            //TCCM Discount / Loads
            public CResult GetTCCMDiscount(List<CNV> oParams)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var iDiscretionaryDiscount = oParams.Where(x => x.sName == "iDiscretionaryDiscount").Select(x => x.sValue).FirstOrDefault();
                    var iUnderwritingLoad = oParams.Where(x => x.sName == "iUnderwritingLoad").Select(x => x.sValue).FirstOrDefault();
                    var CommissionOverridePercentage = oParams.Where(x => x.sName == "CommissionOverridePercentage").Select(x => x.sValue).FirstOrDefault();
                    var OriginalInceptionDate = oParams.Where(x => x.sName == "OriginalInceptionDate").Select(x => x.sValue).FirstOrDefault();
                    var PolBandInceptionDate = oParams.Where(x => x.sName == "PolBandInceptionDate").Select(x => x.sValue).FirstOrDefault();
                    var RTPApplied = oParams.Where(x => x.sName == "RTPApplied").Select(x => x.sValue).FirstOrDefault();
                    var CommissionRTPPercentage = oParams.Where(x => x.sName == "CommissionRTPPercentage").Select(x => x.sValue).FirstOrDefault();
                    //var iDiscretionaryDiscount = "12"; var iUnderwritingLoad = "10"; var CommissionOverridePercentage = "15";
                    //var OriginalInceptionDate = ""; var PolBandInceptionDate = ""; var RTPApplied = "True"; var CommissionRTPPercentage = "";
                    double rMinPremium = 0;
                    double rPremiumGrossCore = 0; double rTotalDiscountedPremium = 0; double rCommissionAllowed = 0;
                    var oTotalDiscountedPremium = GetRateDiscount(oParams);
                    if (oTotalDiscountedPremium.bOK && oTotalDiscountedPremium.oResult != null)
                    {
                        rTotalDiscountedPremium = (double)oTotalDiscountedPremium.oResult;
                    }
                    double rDiscretionaryDiscount = 0; double rUnderwritingLoad = 0; double rCommissionOverridePercentage = 0; double rCommissionRTPPercentage = 0;
                    if (double.TryParse(iDiscretionaryDiscount, out rDiscretionaryDiscount)) { }
                    if (double.TryParse(iUnderwritingLoad, out rUnderwritingLoad)) { }
                    if (double.TryParse(CommissionOverridePercentage, out rCommissionOverridePercentage)) { }
                    if (double.TryParse(CommissionRTPPercentage, out rCommissionRTPPercentage)) { }
                    double rTotalDiscountLoad = rDiscretionaryDiscount - rUnderwritingLoad;
                    double rAppliedPremiumDiscountLoad = (rTotalDiscountedPremium / 100) * (100 - rTotalDiscountLoad);
                    var oMinPremium = GetMinPremium();
                    if (oMinPremium.bOK && oMinPremium.oResult != null)
                    {
                        var sMinPremium = (string)oMinPremium.oResult;
                        if (oMinPremium.oCollectionResult != null && oMinPremium.oCollectionResult.Count() > 0)
                        {
                            oXiResults.Add(oMinPremium);
                        }
                        if (double.TryParse(sMinPremium, out rMinPremium)) { }
                    }
                    if (rAppliedPremiumDiscountLoad < rMinPremium)
                    {
                        rAppliedPremiumDiscountLoad = rMinPremium;
                    }
                    double rCommissionDefaultPercentage = 15;
                    //var oCaravanCommission = GetCaravanCommission(OriginalInceptionDate);
                    //if(oCaravanCommission.bOK&& oCaravanCommission.oResult!=null)
                    //{
                    //    rCommissionDefaultPercentage = (double)oCaravanCommission.oResult;
                    //}
                    double rCommissionCalPart = (rAppliedPremiumDiscountLoad / 100) * (100 - rCommissionDefaultPercentage);
                    if (!string.IsNullOrEmpty(RTPApplied) && RTPApplied.ToLower() == "true" && rCommissionOverridePercentage == rCommissionRTPPercentage)
                    {
                        rCommissionAllowed = rCommissionDefaultPercentage;
                    }
                    else
                    {
                        if (rCommissionOverridePercentage != 0)
                        {
                            rCommissionAllowed = rCommissionOverridePercentage;
                        }
                        else
                        {
                            rCommissionAllowed = rCommissionDefaultPercentage;
                        }
                    }
                    rPremiumGrossCore = rCommissionCalPart / (100 - rCommissionAllowed) * 100;
                    rPremiumGrossCore = Convert.ToDouble(String.Format("{0:0.00}", rPremiumGrossCore));
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = rPremiumGrossCore;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            //Additional Covers
            public CResult GetPolicyGrosspremium(List<CNV> oParams)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    bool bExcessProtection = false; bool bBreakDown = false; bool bLegalExpenses = false; bool bKeyProtection = false; var sBreakDownCoverLevel = string.Empty;
                    double rExcessProtectionGrossPremium = 0; double rBreakDownGrossPremium = 0; double rLegalExpensesGrossPremium = 0; double rKeyProtectionGrossPremium = 0;
                    double rExcessProtectionCommRate = 0; double rBreakDownCommRate = 0; double rLegalExpensesCommRate = 0; double rKeyProtectionCommRate = 0;
                    double dGrossCorePremium = 0; double dAdditionalPremium = 0; string sCover = string.Empty;
                    var oGrossCorePremium = GetTCCMDiscount(oParams);
                    if (oGrossCorePremium.bOK && oGrossCorePremium.oResult != null)
                    {
                        dGrossCorePremium = (double)oGrossCorePremium.oResult;
                    }
                    if (bExcessProtection)
                    {
                        sCover = "ExcessProtection";
                        var oExcessProtectionPremium = GetAdditionalCoverPremium(sCover, "");
                        if (oExcessProtectionPremium.bOK && oExcessProtectionPremium.oResult != null)
                        {
                            if (oExcessProtectionPremium.oCollectionResult != null && oExcessProtectionPremium.oCollectionResult.Count() > 0)
                            {
                                if (double.TryParse(oExcessProtectionPremium.oCollectionResult.Where(m => m.sName == "sGrossPremium").Select(m => m.sValue).FirstOrDefault(), out rExcessProtectionGrossPremium)) { }
                                if (double.TryParse(oExcessProtectionPremium.oCollectionResult.Where(m => m.sName == "sCommRate").Select(m => m.sValue).FirstOrDefault(), out rExcessProtectionCommRate)) { }
                                rExcessProtectionGrossPremium = Convert.ToDouble(String.Format("{0:0.00}", rExcessProtectionGrossPremium));
                            }
                        }
                    }
                    if (bBreakDown)
                    {
                        sCover = "BreakDown";
                        var oBreakDownPremium = GetAdditionalCoverPremium(sCover, sBreakDownCoverLevel);
                        if (oBreakDownPremium.bOK && oBreakDownPremium.oResult != null)
                        {
                            if (oBreakDownPremium.oCollectionResult != null && oBreakDownPremium.oCollectionResult.Count() > 0)
                            {
                                if (double.TryParse(oBreakDownPremium.oCollectionResult.Where(m => m.sName == "sGrossPremium").Select(m => m.sValue).FirstOrDefault(), out rBreakDownGrossPremium)) { }
                                if (double.TryParse(oBreakDownPremium.oCollectionResult.Where(m => m.sName == "sCommRate").Select(m => m.sValue).FirstOrDefault(), out rBreakDownCommRate)) { }
                                rBreakDownGrossPremium = Convert.ToDouble(String.Format("{0:0.00}", rBreakDownGrossPremium));
                            }
                        }
                    }
                    if (bLegalExpenses)
                    {
                        sCover = "LegalExpenses";
                        var oLegalExpensesPremium = GetAdditionalCoverPremium(sCover, "");
                        if (oLegalExpensesPremium.bOK && oLegalExpensesPremium.oResult != null)
                        {
                            if (oLegalExpensesPremium.oCollectionResult != null && oLegalExpensesPremium.oCollectionResult.Count() > 0)
                            {
                                if (double.TryParse(oLegalExpensesPremium.oCollectionResult.Where(m => m.sName == "sGrossPremium").Select(m => m.sValue).FirstOrDefault(), out rLegalExpensesGrossPremium)) { }
                                if (double.TryParse(oLegalExpensesPremium.oCollectionResult.Where(m => m.sName == "sCommRate").Select(m => m.sValue).FirstOrDefault(), out rLegalExpensesCommRate)) { }
                                rLegalExpensesGrossPremium = Convert.ToDouble(String.Format("{0:0.00}", rLegalExpensesGrossPremium));
                            }
                        }
                    }
                    if (bKeyProtection)
                    {
                        sCover = "KeyProtection";
                        var oKeyProtectionPremium = GetAdditionalCoverPremium(sCover, "");
                        if (oKeyProtectionPremium.bOK && oKeyProtectionPremium.oResult != null)
                        {
                            if (oKeyProtectionPremium.oCollectionResult != null && oKeyProtectionPremium.oCollectionResult.Count() > 0)
                            {
                                if (double.TryParse(oKeyProtectionPremium.oCollectionResult.Where(m => m.sName == "sGrossPremium").Select(m => m.sValue).FirstOrDefault(), out rKeyProtectionGrossPremium)) { }
                                if (double.TryParse(oKeyProtectionPremium.oCollectionResult.Where(m => m.sName == "sCommRate").Select(m => m.sValue).FirstOrDefault(), out rKeyProtectionCommRate)) { }
                                rKeyProtectionGrossPremium = Convert.ToDouble(String.Format("{0:0.00}", rKeyProtectionGrossPremium));
                            }
                        }
                    }
                    dAdditionalPremium = rExcessProtectionGrossPremium + rBreakDownGrossPremium + rLegalExpensesGrossPremium + rKeyProtectionGrossPremium;
                    double dPolicyGrossPremium = dGrossCorePremium + dAdditionalPremium;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = dPolicyGrossPremium;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }

            public CResult GetFinalPremium(List<CNV> oParams)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    CNV oNV = new CNV();
                    oNV.sName = "rInterestAmount";
                    double dPolicyGrossPremium = 0; double dIPTAmount = 0;
                    var oPolicyGrossPremium = GetPolicyGrosspremium(oParams);
                    if (oPolicyGrossPremium.bOK && oPolicyGrossPremium.oResult != null)
                    {
                        dPolicyGrossPremium = (double)oPolicyGrossPremium.oResult;
                    }
                    dIPTAmount = dPolicyGrossPremium * (IPTRate / 100);
                    oNV.sValue = dIPTAmount.ToString();
                    double dFinalPremium = dPolicyGrossPremium + dIPTAmount;
                    dFinalPremium = Convert.ToDouble(String.Format("{0:0.00}", dFinalPremium));
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oResult = dFinalPremium;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }

            public CResult GetRateTouring(string sType, string sGeographicalLimit, string sDriverType)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    string sResult = string.Empty;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "RateTouring";
                    if (!string.IsNullOrEmpty(sType) && !string.IsNullOrEmpty(sGeographicalLimit) && !string.IsNullOrEmpty(sDriverType))
                    {
                        switch (sType)
                        {
                            case "Basic":
                                {
                                    if (sDriverType == "Immediate Family only")
                                    {
                                        if (sGeographicalLimit == "UK")
                                        {
                                            oNV1.sValue = "3.60";
                                            sResult = "3.60";
                                        }
                                        else if (sGeographicalLimit == "Europe")
                                        {
                                            oNV1.sValue = "4.75";
                                            sResult = "4.75";
                                        }
                                    }
                                    else if (sDriverType == "Any Driver")
                                    {
                                        if (sGeographicalLimit == "UK")
                                        {
                                            oNV1.sValue = "4.30";
                                            sResult = "4.30";
                                        }
                                        else if (sGeographicalLimit == "Europe")
                                        {
                                            oNV1.sValue = "6.20";
                                            sResult = "6.20";
                                        }
                                    }
                                    break;
                                }
                            case "Complex":
                                {
                                    if (sDriverType == "Immediate Family only")
                                    {
                                        if (sGeographicalLimit == "UK")
                                        {
                                            oNV1.sValue = "3.40";
                                            sResult = "3.40";
                                        }
                                        else if (sGeographicalLimit == "Europe")
                                        {
                                            oNV1.sValue = "4.60";
                                            sResult = "4.60";
                                        }
                                    }
                                    else if (sDriverType == "Any Driver")
                                    {
                                        if (sGeographicalLimit == "UK")
                                        {
                                            oNV1.sValue = "3.80";
                                            sResult = "3.80";
                                        }
                                        else if (sGeographicalLimit == "Europe")
                                        {
                                            oNV1.sValue = "5.70";
                                            sResult = "5.70";
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetMileageDiscount(string sMileage)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Mileage Discount";
                    var sResult = string.Empty;
                    if (sMileage == "Limited")
                    {
                        oNV1.sValue = "75.00";
                        sResult = "75.00";
                    }
                    else if (sMileage == "Seasonal")
                    {
                        oNV1.sValue = "15.00";
                        sResult = "15.00";
                    }
                    else if (sMileage == "Frequent")
                    {
                        oNV1.sValue = "0.00";
                        sResult = "0.00";
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetServicedDiscount(string sServiceDiscount)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Serviced Discount";
                    if (!string.IsNullOrEmpty(sServiceDiscount))
                    {
                        switch (sServiceDiscount.ToLower())
                        {
                            case "yes":
                                {
                                    oNV1.sValue = "10.00";
                                    sResult = "10.00";
                                    break;
                                }
                            case "no":
                                {
                                    oNV1.sValue = "0.00";
                                    sResult = "0.00";
                                    break;
                                }
                        }
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetRateAccidentalDamage(string sAccidentalDamageCover, string sTouring)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "AccidentalDamage Rate";
                    switch (sTouring)
                    {
                        case "Basic":
                            {
                                if (!string.IsNullOrEmpty(sAccidentalDamageCover) && sAccidentalDamageCover.ToLower() == "yes")
                                {
                                    oNV1.sValue = "4.00";
                                    sResult = "4.00";
                                }
                                else if (!string.IsNullOrEmpty(sAccidentalDamageCover) && sAccidentalDamageCover.ToLower() == "no")
                                {
                                    oNV1.sValue = "0.00";
                                    sResult = "0.00";
                                }
                                break;
                            }
                        case "Complex":
                            {
                                if (!string.IsNullOrEmpty(sAccidentalDamageCover) && sAccidentalDamageCover.ToLower() == "yes")
                                {
                                    oNV1.sValue = "3.00";
                                    sResult = "3.00";
                                }
                                else if (!string.IsNullOrEmpty(sAccidentalDamageCover) && sAccidentalDamageCover.ToLower() == "no")
                                {
                                    oNV1.sValue = "0.00";
                                    sResult = "0.00";
                                }
                                break;
                            }
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetCaravanCoreRate(string sTouring)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "CaravanCore Rate";
                    switch (sTouring)
                    {
                        case "Basic":
                            {
                                oNV1.sValue = "7.50";
                                sResult = "7.50";
                                break;
                            }
                        case "Complex":
                            {
                                oNV1.sValue = "7.50";
                                sResult = "9.00";
                                break;
                            }
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetCaravanTheftRate(string sTouring, int iSecurityLevel)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "CaravanTheft Rate";
                    switch (sTouring)
                    {
                        case "Basic":
                            {
                                if (iSecurityLevel == 0)
                                {
                                    oNV1.sValue = "1.75";
                                    sResult = "1.75";
                                }
                                else if (iSecurityLevel == 1)
                                {
                                    oNV1.sValue = "1.10";
                                    sResult = "1.10";
                                }
                                else if (iSecurityLevel == 2)
                                {
                                    oNV1.sValue = "0.90";
                                    sResult = "0.90";
                                }
                                break;
                            }
                        case "Complex":
                            {
                                if (iSecurityLevel == 0)
                                {
                                    oNV1.sValue = "1.50";
                                    sResult = "1.50";
                                }
                                else if (iSecurityLevel == 1)
                                {
                                    oNV1.sValue = "1.25";
                                    sResult = "1.25";
                                }
                                else if (iSecurityLevel == 2)
                                {
                                    oNV1.sValue = "0.85";
                                    sResult = "0.85";
                                }
                                break;
                            }
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetContentsRate(int iSecurityLevel, string sTouring)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Contents Rate";
                    switch (sTouring)
                    {
                        case "Basic":
                            {
                                if (iSecurityLevel == 0)
                                {
                                    oNV1.sValue = "7.00";
                                    sResult = "7.00";
                                }
                                else if (iSecurityLevel == 1)
                                {
                                    oNV1.sValue = "4.20";
                                    sResult = "4.20";
                                }
                                else if (iSecurityLevel == 2)
                                {
                                    oNV1.sValue = "2.52";
                                    sResult = "2.52";
                                }
                                break;
                            }
                        case "Complex":
                            {
                                if (iSecurityLevel == 0)
                                {
                                    oNV1.sValue = "6.00";
                                    sResult = "6.00";
                                }
                                else if (iSecurityLevel == 1)
                                {
                                    oNV1.sValue = "3.60";
                                    sResult = "3.60";
                                }
                                else if (iSecurityLevel == 2)
                                {
                                    oNV1.sValue = "2.16";
                                    sResult = "2.16";
                                }
                                break;
                            }
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetAwningRate(int iProposerAge, string sTouring)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Awning Rate";
                    switch (sTouring)
                    {
                        case "Basic":
                            {
                                if (iProposerAge < 60)
                                {
                                    oNV1.sValue = "17.50";
                                    sResult = "17.50";
                                }
                                else if (iProposerAge >= 60)
                                {
                                    oNV1.sValue = "10.50";
                                    sResult = "10.50";
                                }

                                break;
                            }
                        case "Complex":
                            {
                                if (iProposerAge < 60)
                                {
                                    oNV1.sValue = "27.50";
                                    sResult = "27.50";
                                }
                                else if (iProposerAge >= 60)
                                {
                                    oNV1.sValue = "16.50";
                                    sResult = "16.50";
                                }
                                break;
                            }
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetDiscountSize(double iTotalSI)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Discount Size";
                    if (iTotalSI > 18000)
                    {
                        oNV1.sValue = "22.5";
                        sResult = "22.5";
                    }
                    else if (iTotalSI > 13000)
                    {
                        oNV1.sValue = "17.5";
                        sResult = "17.5";
                    }
                    else if (iTotalSI > 8000)
                    {
                        oNV1.sValue = "12.5";
                        sResult = "12.5";
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetNoClaimDiscount(int iNoClaims)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    if (iNoClaims == 1)
                    {
                        sResult = "7.5";
                    }
                    else if (iNoClaims == 2)
                    {
                        sResult = "10.0";
                    }
                    else if (iNoClaims == 3)
                    {
                        sResult = "15.0";
                    }
                    else if (iNoClaims == 4)
                    {
                        sResult = "17.5";
                    }
                    else if (iNoClaims == 5)
                    {
                        sResult = "20.0";
                    }
                    else if (iNoClaims >= 6)
                    {
                        sResult = "25.0";
                    }
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetProtectedNCDLoad()
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    sResult = "5";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetMinPremium()
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Minimum Premium";
                    sResult = "50";
                    oNV1.sValue = "50";
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetAdditionalCoverPremium(string sCover, string sCoverLevel)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    sResult = "50";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetCaravanCommission(string sOriginalInceptionDate)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    //sResult = "50";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
            public CResult GetAgeFromDOB(string sDOB, string sCoverStartDate)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    oresult.sMessage = "dDOB_" + sDOB;
                    oIB.SaveErrortoDB(oresult);
                    int iAge = DateTime.Now.Year - Convert.ToDateTime(sDOB).Year;
                    if (DateTime.Now < Convert.ToDateTime(sDOB).AddYears(iAge)) iAge--;
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

            public CResult GetTouringType(string sType)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    switch (sType)
                    {
                        case "Single Axle":
                            {
                                sResult = "Basic";
                                break;
                            }
                        case "Twin Axle":
                            {
                                sResult = "Complex";
                                break;
                            }
                        case "Fifth Wheeler":
                            {
                                sResult = "Complex";
                                break;
                            }
                        case "Folding Caravan/Popup Tent/Teardrop/T@B/Micro/Trailer Tent":
                            {
                                sResult = "Basic";
                                break;
                            }
                        case "Touring Caravan":
                            {
                                sResult = "Basic";
                                break;
                            }
                        default:
                            {
                                sResult = "Basic";
                                break;
                            }
                    }
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }

            public CResult GetDriverType(string sType)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    var sResult = string.Empty;
                    switch (sType)
                    {
                        case "You and your family":
                        case "You":
                            {
                                sResult = "Immediate Family only";
                                break;
                            }
                        case "your family and friends":
                            {
                                sResult = "Any Driver";
                                break;
                            }
                    }
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sResult;
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetOccupationsLoadFactor(string sOccupation)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "0.0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Occupation/Secondary Occupation";

                    string[] ReferArray = new string[] { "circus proprietor", "circus worker", "coin dealer", "croupier", "dancer", "dealer", "dealer - general", "dealer - scrap/waste", "diamond dealer", "disc jockey", "entertainer", "exotic dancer", "fairground worker", "floor manager", "footballer", "footballer - semi professional", "furniture dealer", "furniture restorer", "gambler", "gaming club manager", "gaming club proprietor", "gaming club staff - licensed premises", "gaming club staff - unlicensed premises", "golf caddy", "golf club professional", "golf coach", "golfer", "hawker", "horse breeder", "horse dealer", "horse dealer (non sport)", "horse dealer (sport)", "horse trader", "horse trainer", "interviewer", "jeweller", "jockey", "journalist", "journalist - freelance", "journalistic agent", "kissagram person", "landscape gardener", "landworker", "licensee", "magician", "manager - ring sports", "manager - sports", "market trader", "medal dealer", "model", "money broker", "money dealer", "moneylender", "motor dealer", "motor racing driver", "motor racing organiser", "motor trader", "music producer", "musician", "musician - amateur", "musician - classical", "musician - dance band", "musician - pop group", "night club staff", "non professional footballer", "non professional sports coach", "opera singer", "orchestra leader", "orchestral violinist", "playwright", "private detective", "private investigator", "professional apprentice footballer", "professional boxer", "professional cricketer", "professional cyclist", "professional footballer", "professional racing driver", "professional racing motorcyclist", "professional sports coach", "professional sportsperson", "professional wrestler", "promoter", "promoter - entertainments", "promoter - racing", "promoter - ring sports", "promoter - sports", "publican", "publicity manager", "publisher", "publishing manager", "racehorse groom", "racing motorcyclist", "racing organiser", "radio director", "radio presenter", "radio producer", "rally driver", "scrap dealer", "second hand dealer", "semi-professional sportsperson", "show jumper", "showman", "snooker player", "song writer", "sports administrator - other sports", "sports administrator - ring sports", "sports agent", "sports centre attendant", "sports coach", "sports commentator", "sports scout", "sportsman", "sportswoman", "store detective", "street entertainer", "street trader", "student", "student - foreign", "student - living away", "student - living at home", "student nurse", "student nurse - living at home", "student nurse - living away", "student teacher", "student teacher - living at home", "student teacher - living away", "tv announcer", "tv broadcasting technician", "tv editor", "tarot reader/palmistry expert", "television director", "television presenter", "television producer", "trainer - animal", "trainer - greyhound", "trainer - race horse", "travelling showman", "turf accountant", "undergraduate student - living at home", "undergraduate student - living away from home", "unemployed", "ventriloquist", "waste dealer", "acrobat", "actor", "actor/actress", "actress", "amusement arcade worker", "antique dealer", "antique renovator", "art buyer", "art dealer", "asphalter/roadworker", "bodyguard", "bookmaker", "bricklayer", "broadcaster", "broadcaster - tv/radio" };
                    sOccupation = sOccupation.ToLower();
                    int pos = Array.IndexOf(ReferArray, sOccupation);
                    if (pos > -1)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);

                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }

            //mainCalculation
            public CResult GetCaravanFinalPremium(string sGUID, int iInstanceID, int iUserID, string sProductName, string sVersion, string sSessionID, int iCustomerID, int iQuoteID)
            {
                List<string> Info = new List<string>();
                Info.Add("QsInstanceID_" + iInstanceID);
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oIXI = new XIIXI();

                try
                {
                    double rFinalPremium = 0; int iProposerAge = 0; string sContentsSI = string.Empty; string sEquipmentSI = string.Empty; string sAwningSI = string.Empty;
                    double rCompulsaryExcess = 0; double rVoluntaryExcess = 0;string sInterestAmount = string.Empty;
                    string sCaravanSI = string.Empty;
                    //string sSecurityvan = string.Empty;
                    //string sSecurityStorage = string.Empty;
                    int iSecurityVan = 0; int iSecurityStorage = 0; int iSecurityLevel = 0; int iNoClaimYears = 0;
                    var oQSI = oCache.Get_QsStructureObj(sSessionID, sGUID, "QSInstance_" + iInstanceID + "NotationStructure");
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    //string sDOB = "26-Aug-1989"; int iNoClaimYears = 5;
                    string sCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;
                    var oLIst = oQSI.oSubStructureI("Driver_T");
                    var oDriversI = (List<XIIBO>)oLIst.oBOIList;
                    foreach (var item in oDriversI)
                    {
                        var sIsMainDrive = string.Empty;
                        XIIBO oBOI2 = new XIIBO();
                        oBOI2.iBODID = item.iBODID;
                        item.Attributes["enumOccupatation"].BOI = oBOI2;
                        if (item.Attributes.ContainsKey("bMainDriver"))
                        {
                            sIsMainDrive = item.Attributes["bMainDriver"].sValue;
                        }
                        if (!string.IsNullOrEmpty(sIsMainDrive) && sIsMainDrive.ToLower() == "true")
                        {
                            string sDOB = item.AttributeI("dDOB").sResolvedValue;
                            var oProposerAge = GetAgeFromDOB(sDOB, sCoverStartDate);
                            if (oProposerAge.bOK && oProposerAge.oResult != null)
                            {
                                iProposerAge = (int)oProposerAge.oResult;
                            }
                        }
                        string sOccupation = item.AttributeI("enumOccupatation").sResolvedValue;
                        if (!string.IsNullOrEmpty(sOccupation))
                        {
                            Info.Add("Occupation " + sOccupation);
                            var result = GetOccupationsLoadFactor(sOccupation);
                            if (result.bOK && result.oCollectionResult.Count > 0)
                            {
                                oGeneralRefers.Add(result);
                            }
                        }
                        string sSecondaryOccupation = item.AttributeI("sSecondaryOccupation").sResolvedValue;
                        if (!string.IsNullOrEmpty(sSecondaryOccupation))
                        {
                            Info.Add("Secondary Occupation " + sSecondaryOccupation);
                            var result = GetOccupationsLoadFactor(sSecondaryOccupation);
                            if (result.bOK && result.oCollectionResult.Count > 0)
                            {
                                oGeneralRefers.Add(result);
                            }
                        }
                    }
                    string sCaravanLocation = ostructureInstance.XIIValue("CaravanstoreTC").sResolvedValue;
                    string sCaravanMake = ostructureInstance.XIIValue("CaravanMakeTC").sValue;
                    if (!string.IsNullOrEmpty(sCaravanMake) && sCaravanMake.TrimEnd().ToLower() == "other")
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Caravan Make", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Caravan Make", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }
                    var sDriverType = ostructureInstance.XIIValue("towingyourcaravanTC").sResolvedValue;
                    var oDriverType = GetDriverType(sDriverType);
                    if (oDriverType.bOK && oDriverType.oResult != null)
                    {
                        sDriverType = (string)oDriverType.oResult;
                    }
                    var sTypeofAxle = ostructureInstance.XIIValue("TypeofAxleTC").sResolvedValue;
                    var sTypeofCaravan = ostructureInstance.XIIValue("TypeofcaravanTC").sResolvedValue;
                    var oAxleType = GetTouringType(sTypeofAxle); string sAxleType = string.Empty;
                    if (oAxleType.bOK && oAxleType.oResult != null)
                    {
                        sAxleType = (string)oAxleType.oResult;
                    }
                    var oCaravanType = GetTouringType(sTypeofCaravan); string sCaravanType = string.Empty;
                    if (oCaravanType.bOK && oCaravanType.oResult != null)
                    {
                        sCaravanType = (string)oCaravanType.oResult;
                    }
                    if (sAxleType == "Complex")
                    {
                        Touring = "Complex";
                    }
                    else if (sAxleType == "Basic")
                    {
                        if (sCaravanType == "Basic")
                        {
                            Touring = "Basic";
                        }
                        else if (sCaravanType == "Complex")
                        {
                            Touring = "Complex";
                        }
                    }
                    sContentsSI = ostructureInstance.XIIValue("AdditionalcontentstoinsureTC").sValue;
                    //string sContentsInsured = ostructureInstance.XIIValue("StandardContentsinsuredTC").sResolvedValue;
                    //if (!string.IsNullOrEmpty(sContentsInsured) && sContentsInsured.ToLower() == "yes")
                    //{
                    if (double.TryParse(sContentsSI, out ContentsSI)) { }
                    if (ContentsSI > 3000)
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Contents Value", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Contents Value", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }
                    //}

                    sEquipmentSI = ostructureInstance.XIIValue("currentmarketvalueofequipmentTC").sValue;
                    string sEquipmentInsured = ostructureInstance.XIIValue("insureanyequipmentTC").sResolvedValue;
                    if (!string.IsNullOrEmpty(sEquipmentInsured) && sEquipmentInsured.ToLower() == "yes")
                    {
                        if (double.TryParse(sEquipmentSI, out EquipmentSI)) { }
                        if (EquipmentSI > 3000)
                        {
                            oGeneralRefers.Add(BuildCResultObject("0.0", "Equipment Value", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                        }
                        else
                        {
                            oGeneralRefers.Add(BuildCResultObject("0.0", "Equipment Value", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                        }
                    }

                    sAwningSI = ostructureInstance.XIIValue("currentmarketvalueofawningTC").sValue;
                    string sAwningInsured = ostructureInstance.XIIValue("insureanawningTC").sResolvedValue;
                    if (!string.IsNullOrEmpty(sAwningInsured) && sAwningInsured.ToLower() == "yes")
                    {
                        if (double.TryParse(sAwningSI, out AwningSI)) { }
                        if (AwningSI > 3000)
                        {
                            oGeneralRefers.Add(BuildCResultObject("0.0", "Awning Value", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                        }
                        else
                        {
                            oGeneralRefers.Add(BuildCResultObject("0.0", "Awning Value", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                        }
                    }

                    sCaravanSI = ostructureInstance.XIIValue("CurrentcaravanvalueTC").sValue;
                    if (double.TryParse(sCaravanSI, out CaravanSI)) { }
                    if (CaravanSI > 65000)
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Caravan Value", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Caravan Value", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }
                    CorePremium = CaravanSI + (ContentsSI - 1000) + EquipmentSI;
                    string sVoluntaryExcess = ostructureInstance.XIIValue("VoluntaryExcess").sValue;
                    if (sVoluntaryExcess == "10")
                    {
                        sVoluntaryExcess = "0";
                    }
                    string sIsInGoodState = ostructureInstance.XIIValue("caravanrepairTC").sValue;
                    if (!string.IsNullOrEmpty(sIsInGoodState) && sIsInGoodState == "20")
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Good State of Repair", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Good State of Repair", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }
                    if (double.TryParse(sVoluntaryExcess, out rVoluntaryExcess)) { }
                    string sRegularService = ostructureInstance.XIIValue("caravanservicedTC").sResolvedValue;
                    string sYearOfManufacture = ostructureInstance.XIIValue("YearOfManufactureTC").sValue;
                    var sAccidentalDamageCover = ostructureInstance.XIIValue("accidentaldamagecoverTC").sResolvedValue;
                    int iCaravanAge = 0;
                    int iYearOfManufacture = 0; int iCurrentYear = DateTime.Now.Year;
                    if (int.TryParse(sYearOfManufacture, out iYearOfManufacture))
                    {
                        if (iYearOfManufacture < iCurrentYear)
                        {
                            iCaravanAge = iCurrentYear - iYearOfManufacture;
                        }
                    }

                    string sNoClaimYears = ostructureInstance.XIIValue("iNoClaimsYears").sValue;
                    if (int.TryParse(sNoClaimYears, out iNoClaimYears)) { }
                    string sProtectedNCD = ostructureInstance.XIIValue("ProtectYourNoClaimsDiscountTC").sResolvedValue;

                    string sMileage = ostructureInstance.XIIValue("useyourcaravanTC").sResolvedValue;
                    List<string> oSecurityVanList = new List<string>();
                    oSecurityVanList.Add(ostructureInstance.XIIValue("ChassisSecureWheelLockTC").sValue);
                    oSecurityVanList.Add(ostructureInstance.XIIValue("MonitoredTrackerTC").sValue);
                    oSecurityVanList.Add(ostructureInstance.XIIValue("UnmonitoredTrackerTC").sValue);
                    oSecurityVanList.Add(ostructureInstance.XIIValue("AlarmTC").sValue);
                    oSecurityVanList.Add(ostructureInstance.XIIValue("TaggingSystemTC").sValue);
                    oSecurityVanList.Add(ostructureInstance.XIIValue("LockingPinTC").sValue);
                    oSecurityVanList.Add(ostructureInstance.XIIValue("WheelRemovedTC").sValue);
                    oSecurityVanList.Add(ostructureInstance.XIIValue("WinterWheelsTC").sValue);
                    oSecurityVanList = oSecurityVanList.Where(m => !string.IsNullOrEmpty(m)).ToList();
                    //string[] oSecurityReferArray = new string[] { "WheelRemovedTC", "WheelClampTC", "ChassisSecureWheelLockTC", "HitchlockTC"};
                    //List<CResult> oSecurityRefers = new List<CResult>();
                    //foreach (var sSecurityitem in oSecurityReferArray)
                    //{
                    //    var sSecurityValue = ostructureInstance.XIIValue(sSecurityitem).sValue;
                    //    //var sLoadFactorName = ostructureInstance.XIIValue(sSecurityitem).sDisplayName;
                    //    if (string.IsNullOrEmpty(sSecurityValue) && sSecurityValue.ToLower() == "false")
                    //    {
                    //        oGeneralRefers.Add(BuildCResultObject("0.0", "security", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    //    }
                    //}
                    string sNoneTCSecurity = ostructureInstance.XIIValue("NoneTC").sValue;
                    if (!string.IsNullOrEmpty(sNoneTCSecurity) && sNoneTCSecurity.ToLower() == "on")
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "security", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    string sWheelsRemoveSecurity = ostructureInstance.XIIValue("WheelRemovedTC").sValue;
                    string sWheelClampSecurity = ostructureInstance.XIIValue("WheelClampTC").sValue;
                    string sChassisSecureWheelLockTC = ostructureInstance.XIIValue("ChassisSecureWheelLockTC").sValue;
                    string sHitchLockSecurity = ostructureInstance.XIIValue("HitchlockTC").sValue;
                    if (string.IsNullOrEmpty(sWheelsRemoveSecurity) || sWheelsRemoveSecurity == "false")
                    {
                        if (string.IsNullOrEmpty(sWheelClampSecurity) || sWheelClampSecurity == "false")
                        {
                            if (string.IsNullOrEmpty(sChassisSecureWheelLockTC) || sChassisSecureWheelLockTC == "false")
                            {
                                if (string.IsNullOrEmpty(sHitchLockSecurity) || sHitchLockSecurity == "false")
                                {
                                    oGeneralRefers.Add(BuildCResultObject("0.0", "Security", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                                }
                                //else
                                //{
                                //    oGeneralRefers.Add(BuildCResultObject("0.0", "Security", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                                //}
                            }
                        }

                    }

                    //string sUnmonitoredTrackerTCSecurity = ostructureInstance.XIIValue("UnmonitoredTrackerTC").sValue;
                    //string sAlarmTCSecurity = ostructureInstance.XIIValue("AlarmTC").sValue;
                    //string sTaggingSystemTCSecurity = ostructureInstance.XIIValue("TaggingSystemTC").sValue;
                    //string sWinterWheelsTCSecurity = ostructureInstance.XIIValue("WinterWheelsTC").sValue;
                    string sMonitoredTrackerTCSecurity = ostructureInstance.XIIValue("MonitoredTrackerTC").sValue;
                    if (string.IsNullOrEmpty(sMonitoredTrackerTCSecurity) || sMonitoredTrackerTCSecurity == "false")
                    {
                        if (!string.IsNullOrEmpty(sCaravanMake) && sCaravanMake.ToLower() == "5th WHEEL/RV".ToLower())
                        {
                            oGeneralRefers.Add(BuildCResultObject("0.0", "Monitored Tracker", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                        }
                    }
                    string sLockingPinTCSecurity = ostructureInstance.XIIValue("LockingPinTC").sValue;
                    if (string.IsNullOrEmpty(sLockingPinTCSecurity) || sLockingPinTCSecurity == "false")
                    {
                        if (!string.IsNullOrEmpty(sCaravanMake) && sCaravanMake.ToLower() == "5th WHEEL/RV".ToLower())
                        {
                            oGeneralRefers.Add(BuildCResultObject("0.0", "Locking Pin", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                        }
                    }
                    string[] oStorageTypeReferArray = new string[] { "Home address", "Business premises in the UK", "Caravan storage facility", "Working farm in the UK", "Non working farm in the UK", "Other" };
                    int iPos = Array.IndexOf(oStorageTypeReferArray, sCaravanLocation);
                    if (iPos > -1)
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Caravan Location", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Caravan Location", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }
                    if (oSecurityVanList != null && oSecurityVanList.Count() > 0)
                    {
                        iSecurityVan = oSecurityVanList.Where(x => x.ToLower() == "on").Count();
                    }
                    string sLockableGatesTC1 = ostructureInstance.XIIValue("LockableGatesTC1").sValue;
                    if (string.IsNullOrEmpty(sLockableGatesTC1) || sLockableGatesTC1 == "false")
                    {
                        if (!string.IsNullOrEmpty(sCaravanLocation) && sCaravanLocation.ToLower() == "other residential address")
                        {
                            oGeneralRefers.Add(BuildCResultObject("0.0", "Lockable Gates", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                        }
                    }
                    string sStorageLocation = ostructureInstance.XIIValue("None2TC").sValue;
                    if (!string.IsNullOrEmpty(sStorageLocation) && sStorageLocation.ToLower() == "on")
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Storage Location", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Storage Location", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }
                    string sStorageLocationCaravansCount = ostructureInstance.XIIValue("caravansstoredlocationTC").sResolvedValue;
                    if (!string.IsNullOrEmpty(sCaravanLocation) && sCaravanLocation.ToLower() == "other" && !string.IsNullOrEmpty(sStorageLocationCaravansCount) && sStorageLocationCaravansCount.ToLower() == "less than 10")
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Caravan Count", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Caravan Count", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }
                    List<string> oSecurityStorageList = new List<string>();
                    oSecurityStorageList.Add(ostructureInstance.XIIValue("CCTVinOperationTC1").sValue);
                    oSecurityStorageList.Add(ostructureInstance.XIIValue("FullTimeWardenTC1").sValue);
                    oSecurityStorageList.Add(ostructureInstance.XIIValue("FullyAlarmedTC1").sValue);
                    oSecurityStorageList.Add(ostructureInstance.XIIValue("LockableGatesTC1").sValue);
                    oSecurityStorageList.Add(ostructureInstance.XIIValue("SecurityLightingTC1").sValue);
                    oSecurityStorageList = oSecurityStorageList.Where(m => !string.IsNullOrEmpty(m)).ToList();
                    if (oSecurityStorageList != null && oSecurityStorageList.Count() > 0)
                    {
                        iSecurityStorage = oSecurityStorageList.Where(x => x.ToLower() == "on").Count();
                    }
                    //if (int.TryParse(sSecurityvan, out iSecurityvan)) { }
                    //if (int.TryParse(sSecurityStorage, out iSecurityStorage)) { }
                    var sSecurity = iSecurityVan + iSecurityStorage;
                    if (sSecurity < 2)
                    {
                        iSecurityLevel = 0;
                    }
                    else if (sSecurity == 2)
                    {
                        iSecurityLevel = 1;
                    }
                    else
                    {
                        iSecurityLevel = 2;
                    }

                    if (iProposerAge < 25)
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Age", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    string sPreviousMotorOrCaravanInsuranceTC = ostructureInstance.XIIValue("PreviousMotorOrCaravanInsuranceTC").sResolvedValue;
                    if (!string.IsNullOrEmpty(sPreviousMotorOrCaravanInsuranceTC) && sPreviousMotorOrCaravanInsuranceTC.ToLower() == "no")
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "PreviousMotor Or CaravanInsurance", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "PreviousMotor Or CaravanInsurance", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }
                    string sMotorOffence = ostructureInstance.XIIValue("MotorOffenceTC").sResolvedValue;
                    if (!string.IsNullOrEmpty(sMotorOffence) && sMotorOffence.ToLower() == "no")
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Motor Offence", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Motor Offence", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }
                    string sFinancialInterest = ostructureInstance.XIIValue("financialinterestincaravanTC").sResolvedValue;
                    if (!string.IsNullOrEmpty(sFinancialInterest) && sFinancialInterest.ToLower() == "no")
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Financialinterest in Caravan", xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()));
                    }
                    else
                    {
                        oGeneralRefers.Add(BuildCResultObject("0.0", "Financialinterest in Caravan", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                    }
                    List<CNV> oParams = new List<CNV>();
                    oParams.Add(new CNV { sName = "iSecurityLevel", sValue = iSecurityLevel.ToString() });
                    oParams.Add(new CNV { sName = "Touring", sValue = Touring });
                    oParams.Add(new CNV { sName = "iNoClaimYears", sValue = sNoClaimYears });
                    oParams.Add(new CNV { sName = "iProposerAge", sValue = iProposerAge.ToString() });
                    oParams.Add(new CNV { sName = "bExcessProtection", sValue = "False" });
                    oParams.Add(new CNV { sName = "bBreakDown", sValue = "False" });
                    oParams.Add(new CNV { sName = "bLegalExpenses", sValue = "False" });
                    oParams.Add(new CNV { sName = "bKeyProtection", sValue = "False" });
                    oParams.Add(new CNV { sName = "sBreakDownCoverLevel", sValue = "" });

                    oParams.Add(new CNV { sName = "iDiscretionaryDiscount", sValue = "12" });
                    oParams.Add(new CNV { sName = "iUnderwritingLoad", sValue = "10" });
                    oParams.Add(new CNV { sName = "CommissionOverridePercentage", sValue = "15" });
                    oParams.Add(new CNV { sName = "OriginalInceptionDate", sValue = sCoverStartDate });
                    oParams.Add(new CNV { sName = "PolBandInceptionDate", sValue = "" });
                    oParams.Add(new CNV { sName = "RTPApplied", sValue = "true" });
                    oParams.Add(new CNV { sName = "CommissionRTPPercentage", sValue = "" });

                    oParams.Add(new CNV { sName = "sProtectedNCD", sValue = sProtectedNCD });

                    oParams.Add(new CNV { sName = "sDriverType", sValue = sDriverType });
                    oParams.Add(new CNV { sName = "sGeographicalLimit", sValue = "UK" });
                    oParams.Add(new CNV { sName = "sMileage", sValue = sMileage });
                    oParams.Add(new CNV { sName = "sRegularService", sValue = sRegularService });
                    oParams.Add(new CNV { sName = "CaravanAge", sValue = iCaravanAge.ToString() });

                    oParams.Add(new CNV { sName = "sAccidentalDamageCover", sValue = sAccidentalDamageCover });

                    //var oFinalPremium = GetFinalPremium(iSecurityLevel, Touring, iNoClaimYears, iProposerAge);
                    var oFinalPremium = GetFinalPremium(oParams);
                    if (oFinalPremium.bOK && oFinalPremium.oResult != null)
                    {
                        rFinalPremium = (double)oFinalPremium.oResult;
                    }
                    if (oFinalPremium.oCollectionResult!=null && oFinalPremium.oCollectionResult.Count()>0)
                    {
                        sInterestAmount = oFinalPremium.oCollectionResult.Where(m => m.sName == "rInterestAmount").Select(m => m.sValue).FirstOrDefault();
                    }
                    var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
                    int iProductID = 0;
                    string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                    if (int.TryParse(ProductID, out iProductID)) { }
                    var oProductI = oIXI.BOI("Product", ProductID);
                    oProductI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Product");
                    XIIBO oBOI = new XIIBO();
                    XIAPI oXIAPI = new XIAPI();
                    oBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations");
                    Info.Add("RiskFactorsCount:" + oGeneralRefers.Count);
                    foreach (var item in oGeneralRefers)
                    {
                        string sMessage = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        Info.Add(sMessage);
                    }
                    if (oGeneralRefers != null && oGeneralRefers.Count() > 0 && oGeneralRefers.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())))
                    {
                        Info.Add("All Load Factors are normal");
                        double rFinalQuote = 0;
                        double rPFAmount = 0;
                        double rMonthlyTotal = 0;
                        double rPaymentCharge = 0; double rInsurerCharge = 0; double rAdmin = 0;
                        if (double.TryParse(oProductI.Attributes["rPaymentCharge"].sValue, out rPaymentCharge)) { }
                        if (double.TryParse(oProductI.Attributes["rInsurerCharge"].sValue, out rInsurerCharge)) { }
                        if (double.TryParse(oProductI.Attributes["zDefaultAdmin"].sValue, out rAdmin)) { }
                        rFinalQuote = rFinalPremium + rPaymentCharge + rInsurerCharge + rAdmin;
                        double rInterestAmount = 0;
                        if (double.TryParse(sInterestAmount,out rInterestAmount)) { }
                        oBOI.Attributes["rInterestAmount"] = new XIIAttribute { sName = "rInterestAmount", sValue = String.Format("{0:0.00}", rInterestAmount), bDirty = true };
                        oBOI.Attributes["rInterestRate"] = new XIIAttribute { sName = "rInterestRate", sValue = String.Format("{0:0.00}", IPTRate), bDirty = true };
                        oBOI.Attributes["rPaymentCharge"] = new XIIAttribute { sName = "rPaymentCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rPaymentCharge"].sValue), bDirty = true };
                        oBOI.Attributes["rInsurerCharge"] = new XIIAttribute { sName = "rInsurerCharge", sValue = String.Format("{0:0.00}", oProductI.Attributes["rInsurerCharge"].sValue), bDirty = true };
                        var MinimumDeposit = oXIAPI.GetMinimumDepostAmount(rPaymentCharge, rInsurerCharge, rFinalQuote, rAdmin, sGUID, iInstanceID, iProductID, 0, 0);
                        double rMinDeposit = 0;
                        if (double.TryParse(MinimumDeposit, out rMinDeposit)) { }
                        oBOI.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = String.Format("{0:0.00}", rFinalPremium), bDirty = true };
                        oBOI.Attributes["rQuotePremium"] = new XIIAttribute { sName = "rQuotePremium", sValue = String.Format("{0:0.00}", rFinalPremium), bDirty = true };
                        oBOI.Attributes["rGrossPremium"] = new XIIAttribute { sName = "rGrossPremium", sValue = String.Format("{0:0.00}", rFinalPremium), bDirty = true };
                        oBOI.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = String.Format("{0:0.00}", rFinalQuote), bDirty = true };
                        oBOI.Attributes["zDefaultDeposit"] = new XIIAttribute { sName = "zDefaultDeposit", sValue = String.Format("{0:0.00}", MinimumDeposit), bDirty = true };
                        oBOI.Attributes["zDefaultAdmin"] = new XIIAttribute { sName = "zDefaultAdmin", sValue = String.Format("{0:0.00}", oProductI.Attributes["zDefaultAdmin"].sValue), bDirty = true };
                        var MonthlyAmount = oXIAPI.GetMonthlyPremiumAmount(rFinalQuote, rMinDeposit, iProductID, 0, 0);
                        Info.Add("Monthly Amount:" + MonthlyAmount);
                        rMonthlyTotal = (MonthlyAmount * 10) + rMinDeposit;
                        oBOI.Attributes["rMonthlyPrice"] = new XIIAttribute { sName = "rMonthlyPrice", sValue = String.Format("{0:0.00}", MonthlyAmount), bDirty = true };
                        oBOI.Attributes["rMonthlyTotal"] = new XIIAttribute { sName = "rMonthlyTotal", sValue = String.Format("{0:0.00}", rMonthlyTotal), bDirty = true };
                        rPFAmount = rMonthlyTotal - rMinDeposit;
                        oBOI.Attributes["rPremiumFinanceAmount"] = new XIIAttribute { sName = "rPremiumFinanceAmount", sValue = String.Format("{0:0.00}", rPFAmount), bDirty = true };
                        oBOI.Attributes["bIsCoverAbroad"] = new XIIAttribute { sName = "bIsCoverAbroad", sValue = oProductI.Attributes["bIsCoverAbroad"].sValue, bDirty = true };
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
                        oBOI.Attributes["sExcess"] = new XIIAttribute { sName = "sExcess", sValue = ExcessContent, bDirty = true };
                        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "0", bDirty = true };
                    }
                    else if (oGeneralRefers != null && oGeneralRefers.Count() > 0 && oGeneralRefers.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())))
                    {
                        Info.Add("Some Load Factors are Refered");
                        oBOI.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "10", bDirty = true };
                        oBOI.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = "0.00", bDirty = true };
                        oBOI.Attributes["rFinalQuote"] = new XIIAttribute { sName = "rFinalQuote", sValue = "0.00", bDirty = true };
                        oBOI.Attributes["zDefaultDeposit"] = new XIIAttribute { sName = "zDefaultDeposit", sValue = "0.00", bDirty = true };
                        oBOI.Attributes["zDefaultAdmin"] = new XIIAttribute { sName = "zDefaultAdmin", sValue = "0.00", bDirty = true };
                        oBOI.Attributes["rMonthlyPrice"] = new XIIAttribute { sName = "rMonthlyPrice", sValue = "0.00", bDirty = true };
                    }
                    var sQuoteGUID = Guid.NewGuid().ToString("N").Substring(0, 10);
                    var oSource = oIXI.BOI("XISource_T", ostructureInstance.Attributes["fkisourceid"].sValue);
                    string sPrefix = string.Empty;
                    if (oSource.Attributes != null && oSource.Attributes.ContainsKey("sprefixcode"))
                    {
                        sPrefix = oSource.Attributes["sprefixcode"].sValue;
                    }
                    var iBatchID = iCustomerID.ToString() + iInstanceID.ToString();
                    oBOI.Attributes["sRegNo"] = new XIIAttribute { sName = "sRegNo", sValue = ostructureInstance.XIIValue("sRegNo").sValue, bDirty = true };
                    oBOI.Attributes["dCoverStart"] = new XIIAttribute { sName = "dCoverStart", sValue = sCoverStartDate, bDirty = true };
                    oBOI.Attributes["sCaravanMake"] = new XIIAttribute { sName = "sCaravanMake", sValue = ostructureInstance.XIIValue("CaravanMakeTC").sDerivedValue, bDirty = true };
                    oBOI.Attributes["sCaravanModel"] = new XIIAttribute { sName = "sCaravanModel", sValue = ostructureInstance.XIIValue("ModelofCaravanTC").sValue, bDirty = true };


                    oBOI.Attributes["FKiQSInstanceID"] = new XIIAttribute { sName = "FKiQSInstanceID", sValue = iInstanceID.ToString(), bDirty = true };
                    oBOI.Attributes["sInsurer"] = new XIIAttribute { sName = "sInsurer", sValue = oProductI.Attributes["sName"].sValue, bDirty = true };
                    oBOI.Attributes["FKiCustomerID"] = new XIIAttribute { sName = "FKiCustomerID", sValue = iCustomerID.ToString(), bDirty = true };
                    oBOI.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                    oBOI.Attributes["dtqsupdateddate"] = new XIIAttribute { sName = "dtqsupdateddate", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBOI.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBOI.Attributes["FKiProductVersionID"] = new XIIAttribute { sName = "FKiProductVersionID", sValue = sVersion, bDirty = true };
                    oBOI.Attributes["sGUID"] = new XIIAttribute { sName = "sGUID", sValue = sQuoteGUID, bDirty = true };
                    Random generator = new Random();
                    string sRef = generator.Next(1, 10000000).ToString(new String('0', 7));
                    oBOI.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + sRef, bDirty = true };
                    //oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + Guid.NewGuid().ToString("N").Substring(0, 6), bDirty = true };
                    //oBOI.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = Guid.NewGuid().ToString("N").Substring(0, 6), bDirty = true };
                    oBOI.Attributes["BatchID"] = new XIIAttribute { sName = "BatchID", sValue = iBatchID, bDirty = true };
                    oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = iQuoteID.ToString(), bDirty = true };
                    oBOI.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = String.Format("{0:0.00}", rCompulsaryExcess), bDirty = true };
                    oBOI.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = String.Format("{0:0.00}", rVoluntaryExcess), bDirty = true };
                    double rTotalExcess = rCompulsaryExcess + rVoluntaryExcess;
                    oBOI.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = String.Format("{0:0.00}", rTotalExcess), bDirty = true };
                    //Info.Add("QuoteRefID_" + oBOI.Attributes["sRefID"].sValue);
                    var oRes = oBOI.Save(oBOI);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        oBOI = (XIIBO)oRes.oResult;
                    }
                    Info.Add(oProductI.Attributes["sName"].sValue + "Quote inserted Sucessfully with the amount of " + rFinalPremium);
                    XIDBO oRiskFactorsBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
                    XIIBO oBO = new XIIBO();
                    oBO.BOD = oRiskFactorsBOD;
                    List<XIIBO> oBOIList = new List<XIIBO>();
                    foreach (var item in oGeneralRefers)
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
                    oCResult.oResult = rFinalPremium;
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
        }
    }
}