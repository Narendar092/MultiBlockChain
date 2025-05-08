using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using XICore;
using XISystem;

namespace ZeeInsurance.XIScriptCalculation._183
{
    public class ZENITH
    {
        public static CResult PolicyMainCal(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            PolicyCalculation Pcal = new PolicyCalculation();
            //XIIQS oQsInstance = new XIIQS();
            CResult oResult = new CResult();
            oResult.sMessage = "Zenith Marque script running";
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
            public CResult CompulsaryExcess(int DriverAge, int lienceHeldInYrs)
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
                    oNV2.sValue = "Compulsory Excess";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Value";

                    if (lienceHeldInYrs >= 1)
                    {
                        oNV1.sValue = "+100";
                    }
                    else
                    {
                        oNV1.sValue = "+250";
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetVechileValue(double iCurrentCaravanValue, string sTypeOfMotorhome)
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
                    oNV2.sValue = "Vehicle Value";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Value";

                    if (!string.IsNullOrEmpty(sTypeOfMotorhome))
                    {
                        sTypeOfMotorhome = sTypeOfMotorhome.ToLower();
                    }
                    if (sTypeOfMotorhome == "tpft" && iCurrentCaravanValue > 5000)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    else if ((sTypeOfMotorhome == "comp" || sTypeOfMotorhome == "tpo") && iCurrentCaravanValue > 100000)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    else if (sTypeOfMotorhome == "tpft" || sTypeOfMotorhome == "comp")
                    {
                        if (iCurrentCaravanValue >= 0 && iCurrentCaravanValue <= 12000)
                        {
                            oNV1.sValue = "+0";
                        }
                        else if (iCurrentCaravanValue >= 12001 && iCurrentCaravanValue <= 25000)
                        {
                            oNV1.sValue = "+100";
                        }
                        else if (iCurrentCaravanValue >= 25001 && iCurrentCaravanValue <= 40000)
                        {
                            oNV1.sValue = "+150";
                        }
                        else if (iCurrentCaravanValue >= 40001 && iCurrentCaravanValue <= 50000)
                        {
                            oNV1.sValue = "+200";
                        }
                        else if (iCurrentCaravanValue >= 50001 && iCurrentCaravanValue <= 75000)
                        {
                            oNV1.sValue = "+250";
                        }
                        else if (iCurrentCaravanValue >= 75001 && iCurrentCaravanValue <= 100000)
                        {
                            oNV1.sValue = "+400";
                        }
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetBaseRate(string sTypeOfMotorhome)
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
                    oNV2.sValue = "Base Rate";
                    if (!string.IsNullOrEmpty(sTypeOfMotorhome))
                    {
                        sTypeOfMotorhome = sTypeOfMotorhome.ToLower();
                    }
                    if (sTypeOfMotorhome == "comp")
                    {
                        oNV1.sValue = "+1260";
                    }
                    else if (sTypeOfMotorhome == "tpft")
                    {
                        oNV1.sValue = "+854";
                    }
                    else if (sTypeOfMotorhome == "tpo")
                    {
                        oNV1.sValue = "+779";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    //oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetVehicleGroup(int EngineSize)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    if (EngineSize <= 1300)
                    {
                        oresult.oResult = "4";
                    }
                    else if (EngineSize >= 1301 && EngineSize <= 1500)
                    {
                        oresult.oResult = "5";
                    }
                    else if (EngineSize >= 1501 && EngineSize <= 1700)
                    {
                        oresult.oResult = "6";
                    }
                    else if (EngineSize >= 1701 && EngineSize <= 1900)
                    {
                        oresult.oResult = "7";
                    }
                    else if (EngineSize >= 1901 && EngineSize <= 2000)
                    {
                        oresult.oResult = "8";
                    }
                    else if (EngineSize >= 2001 && EngineSize <= 2300)
                    {
                        oresult.oResult = "9";
                    }
                    else if (EngineSize >= 2301 && EngineSize <= 2500)
                    {
                        oresult.oResult = "10";
                    }
                    else if (EngineSize >= 2501 && EngineSize <= 2800)
                    {
                        oresult.oResult = "12";
                    }
                    else if (EngineSize >= 2801 && EngineSize <= 4000)
                    {
                        oresult.oResult = "14";
                    }
                    else if (EngineSize >= 4001 && EngineSize <= 5000)
                    {
                        oresult.oResult = "16";
                    }
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetVehicleGroupLoad(int Group, string sTypeOfMotorhome)
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
                    oNV2.sValue = "Vehicle Group";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    switch (Group)
                    {
                        case 1:
                        case 2:
                        case 3:
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                            break;
                        case 4:
                            oNV1.sValue = "+3.30";
                            break;
                        case 5:
                            oNV1.sValue = "+5.80";
                            break;
                        case 6:
                            oNV1.sValue = "+10.80";
                            break;
                        case 7:
                            oNV1.sValue = "+15.90";
                            break;
                        case 8:
                            oNV1.sValue = "+23.32";
                            break;
                        case 9:
                            oNV1.sValue = "+29.68";
                            break;
                        case 10:
                            oNV1.sValue = "+42.80";
                            break;
                        case 11:
                            oNV1.sValue = "+47.90";
                            break;
                        case 12:
                            oNV1.sValue = "+53.75";
                            break;
                        case 13:
                            oNV1.sValue = "+58.88";
                            break;
                        case 14:
                            oNV1.sValue = "+64.00";
                            break;
                        case 15:
                            oNV1.sValue = "+67.48";
                            break;
                        case 16:
                            oNV1.sValue = "+71.70";
                            break;
                        case 17:
                            oNV1.sValue = "+75.90";
                            break;
                        case 18:
                            oNV1.sValue = "+79.40";
                            break;
                        case 19:
                            oNV1.sValue = "+81.20";
                            break;
                        case 20:
                            oNV1.sValue = "+83.00";
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetMileageLoadFactor(int miles, string sVehicleUse)
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
                    oNV2.sValue = "Annual Mileage";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (!string.IsNullOrEmpty(sVehicleUse) && sVehicleUse.ToLower() == "sdp")
                    {
                        if (miles >= 1 && miles <= 3000)
                        {
                            oNV1.sValue = "-25";
                        }
                        else if (miles >= 3001 && miles <= 5000)
                        {
                            oNV1.sValue = "-15";
                        }
                        else if (miles >= 5001 && miles <= 7500)
                        {
                            oNV1.sValue = "-5";
                        }
                        else if (miles >= 7501 && miles <= 9000)
                        {
                            oNV1.sValue = "-0";
                        }
                        else if (miles >= 9001 && miles <= 11000)
                        {
                            oNV1.sValue = "+20";
                        }
                        else
                        {
                            oNV1.sValue = "+30";
                        }
                    }
                    else if (!string.IsNullOrEmpty(sVehicleUse) && sVehicleUse.ToLower() == "sdpc")
                    {
                        if (miles >= 1 && miles <= 3000)
                        {
                            oNV1.sValue = "-21";
                        }
                        else if (miles >= 3001 && miles <= 5000)
                        {
                            oNV1.sValue = "-11";
                        }
                        else if (miles >= 5001 && miles <= 7500)
                        {
                            oNV1.sValue = "+10";
                        }
                        else if (miles >= 7501 && miles <= 9000)
                        {
                            oNV1.sValue = "+5";
                        }
                        else if (miles >= 9001 && miles <= 11000)
                        {
                            oNV1.sValue = "+26";
                        }
                        else
                        {
                            oNV1.sValue = "+37";
                        }
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }

            public CResult GetDisrictLoad(string sArea, string sTypeOfCover)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                XIAPI oXIAPI = new XIAPI();
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
                    oNV2.sValue = "District";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    string sType = string.Empty;
                    if (!string.IsNullOrEmpty(sTypeOfCover) && sTypeOfCover.ToLower() == "comp")
                    {
                        sType = "Comp";
                    }
                    else
                    {
                        sType = "NonComp";
                    }
                    List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                    oWHParams.Add(new XIWhereParams { sField = "Area", sOperator = "=", sValue = sArea });
                    var oAreaLoad = oXIAPI.GetValue("DistrictLoad_T", sType, oWHParams);
                    if (!string.IsNullOrEmpty(oAreaLoad))
                    {
                        oNV1.sValue = oAreaLoad;
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public string GetArea(string sPostCode, string sTypeOfCover)
            {
                string sArea = string.Empty;
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                XIAPI oXIAPI = new XIAPI();
                try
                {
                    sPostCode = sPostCode.Replace(" ", "").ToUpper();
                    sPostCode = sPostCode.Insert(sPostCode.Length - 3, " ");
                    string sMPostCodetest = sPostCode;
                    for (int i = 0; i < sPostCode.Length; i++)
                    {

                        List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                        oWHParams.Add(new XIWhereParams { sField = "PostCode", sOperator = "=", sValue = sMPostCodetest });
                        var oArea = oXIAPI.GetValue("ZenithFullPostCodelookup_T", sTypeOfCover, oWHParams);
                        if (!string.IsNullOrEmpty(oArea))
                        {
                            sArea = oArea;
                            break;
                        }
                        sMPostCodetest = sMPostCodetest.Remove(sMPostCodetest.Length - (i + 1));
                    }
                    if (string.IsNullOrEmpty(sArea))
                    {
                        for (int i = 0; i < sPostCode.Length; i++)
                        {
                            string sMPostCode = sPostCode.Remove(i);
                            List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                            oWHParams.Add(new XIWhereParams { sField = "sPostCode", sOperator = "=", sValue = sMPostCode });
                            var oArea = oXIAPI.GetValue("AreaLookUp_T", "Area", oWHParams);
                            if (!string.IsNullOrEmpty(oArea))
                            {
                                sArea = oArea;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return sArea;
            }
            public CResult GetAreaLoad(string Area, string sTypeOfCover)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                XIAPI oXIAPI = new XIAPI();
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
                    oNV2.sValue = "Area";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                    oWHParams.Add(new XIWhereParams { sField = "Area", sOperator = "=", sValue = Area });
                    var oAreaLoad = oXIAPI.GetValue("AreaLoad_T", sTypeOfCover, oWHParams);
                    if (!string.IsNullOrEmpty(oAreaLoad))
                    {
                        if (oAreaLoad.ToLower() == "decline")
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                        else
                        {
                            oNV1.sValue = oAreaLoad;
                        }
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetGaragingLoad(string Area, string sTypeOfCover, string sGaraging)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                XIAPI oXIAPI = new XIAPI();
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
                    oNV2.sValue = "Garaging";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    string sField = string.Empty;
                    if ((sGaraging == "Garaged" || sGaraging == "Locked Building" || sGaraging == "Locked Compound") && (sTypeOfCover == "Comp" || sTypeOfCover == "TPFT"))
                    {
                        sField = "COMPOrTPFTGarageandLocked";
                    }
                    else if ((sGaraging == "Carport" || sGaraging == "Unlocked Building" || sGaraging == "Unlocked Compound" || sGaraging == "Parked on Drive") && (sTypeOfCover == "Comp" || sTypeOfCover == "TPFT"))
                    {
                        sField = "COMPOrTPFTUnLocked";
                    }
                    else if ((sGaraging == "Kept on Public Road" || sGaraging == "Car Park") && (sTypeOfCover == "Comp" || sTypeOfCover == "TPFT"))
                    {
                        sField = "COMPOrTPFTUnLocked";
                    }
                    else if ((sGaraging == "Garaged" || sGaraging == "Locked Building" || sGaraging == "Locked Compound") && sTypeOfCover == "TPO")
                    {
                        sField = "TPOGarageandLocked";
                    }
                    else if ((sGaraging == "Carport" || sGaraging == "Unlocked Building" || sGaraging == "Unlocked Compound" || sGaraging == "Parked on Drive") && sTypeOfCover == "TPO")
                    {
                        sField = "TPOUnLocked";
                    }
                    else if ((sGaraging == "Kept on Public Road" || sGaraging == "Car Park") && sTypeOfCover == "TPO")
                    {
                        sField = "TPORoadandCarPark";
                    }
                    List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                    oWHParams.Add(new XIWhereParams { sField = "Area", sOperator = "=", sValue = Area });
                    var oAreaLoad = oXIAPI.GetValue("GaragingLoad_T", sField, oWHParams);
                    if (!string.IsNullOrEmpty(oAreaLoad))
                    {
                        if (oAreaLoad.ToLower() == "decline")
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                        else
                        {
                            oNV1.sValue = oAreaLoad;
                        }
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetAgeOfVechicleDiscount(int YearofManufacture, string sTypeOfMotorhome)
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
                    oNV2.sValue = "Vehicle Age";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    var CurrentDate = DateTime.Now.Year;
                    var YearDiff = CurrentDate - YearofManufacture;
                    if (!string.IsNullOrEmpty(sTypeOfMotorhome))
                    {
                        sTypeOfMotorhome = sTypeOfMotorhome.ToLower();
                    }
                    if (sTypeOfMotorhome == "tpo" && YearDiff >= 16)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    }
                    else if (sTypeOfMotorhome == "tpo")
                    {
                        oNV1.sValue = "+0";
                    }
                    else
                    {
                        switch (YearDiff)
                        {
                            case 0:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-1.67" : "+15";
                                break;
                            case 1:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-3.81" : "+12.50";
                                break;
                            case 2:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "+1.75" : "+10.00";
                                break;
                            case 3:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "+10.75" : "+7.50";
                                break;
                            case 4:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "+7.10" : "+5.00";
                                break;
                            case 5:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "+4.54" : "+3.50";
                                break;
                            case 6:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "+4.05" : "+2.00";
                                break;
                            case 7:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "+2.51" : "+0.50";
                                break;
                            case 8:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "+0.94" : "-2.98";
                                break;
                            case 9:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-1.20" : "-6.42";
                                break;
                            case 10:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-4.84" : "-10.33";
                                break;
                            case 11:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-8.42" : "-15.76";
                                break;
                            case 12:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-12.22" : "-18.91";
                                break;
                            case 13:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-15.12" : "-21.99";
                                break;
                            case 14:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-17.97" : "-25.00";
                                break;
                            case 15:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-20.79" : "-27.95";
                                break;
                            case 16:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-23.56" : "-30.84";
                                break;
                            case 17:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-25.94" : "-33.35";
                                break;
                            case 18:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-27.35" : "-34.96";
                                break;
                            case 19:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-28.82" : "-36.62";
                                break;
                            case 20:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-29.77" : "-38.00";
                                break;
                            case 21:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-30.11" : "-38.72";
                                break;
                            case 22:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-30.44" : "-39.05";
                                break;
                            case 23:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-30.60" : "-39.38";
                                break;
                            case 24:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-30.77" : "-39.71";
                                break;
                            case 25:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-30.93" : "-40.04";
                                break;
                            case 26:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-31.10" : "-40.25";
                                break;
                            case 27:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-31.27" : "-40.37";
                                break;
                            case 28:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-31.43" : "-40.37";
                                break;
                            case 29:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-31.60" : "-40.37";
                                break;
                            case 30:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-31.76" : "-40.37";
                                break;
                            case 31:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-31.93" : "-40.37";
                                break;
                            case 32:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-32.09" : "-40.37";
                                break;
                            case 33:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-32.26" : "-40.37";
                                break;
                            case 34:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-32.42" : "-40.37";
                                break;
                            case 35:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-32.59" : "-40.37";
                                break;
                            case 36:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-32.76" : "-40.37";
                                break;
                            case 37:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-32.92" : "-40.37";
                                break;
                            case 38:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-33.09" : "-40.37";
                                break;
                            case 39:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-33.25" : "-40.37";
                                break;
                            case 40:
                                oNV1.sValue = sTypeOfMotorhome == "comp" ? "-33.42" : "-40.37";
                                break;
                            default:
                                oNV1.sValue = "+0.00";
                                oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                break;
                        }
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetDrivingRestrictionsLoadFactor(List<string> type)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Driving Restriction";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    int icount = type.Count;
                    oresult.sMessage = "TypeOfDrivingCount:" + icount;
                    oIB.SaveErrortoDB(oresult);
                    if (icount == 1)
                    {
                        oNV1.sValue = "-2.5";
                    }
                    else if (icount == 2)
                    {
                        if (type.Contains("spouse") || type.Contains("common law partner"))
                        {
                            oNV1.sValue = "-15";
                        }
                        else
                        {
                            oNV1.sValue = "-0";
                        }
                    }
                    else if (icount == 3)
                    {
                        oNV1.sValue = "-0";
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetMaritalStatusLoadFactor(string sMaritalStatus)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Marital Status";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (sMaritalStatus == "Divorced" || sMaritalStatus == "Seperated" || sMaritalStatus == "Single")
                    {
                        oNV1.sValue = "+7.5";
                    }
                    else if (sMaritalStatus == "Married - Common Law" || sMaritalStatus == "Married" || sMaritalStatus == "Partnered")
                    {
                        oNV1.sValue = "-5";
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetOccupationLoad(string sOccupation, string sTypeOfMotorhome, string sVehicleUse, int iClassID)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                XIAPI oXIAPI = new XIAPI();
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
                    oNV2.sValue = "Occupation";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                    oWHParams.Add(new XIWhereParams { sField = "sName", sOperator = "=", sValue = sOccupation });
                    oWHParams.Add(new XIWhereParams { sField = "FkiClassID", sOperator = "=", sValue = iClassID.ToString() });
                    var oOccupationTerm = oXIAPI.GetValue("enumMasterOccupation_T", "sTerm", oWHParams);
                    if (!string.IsNullOrEmpty(sVehicleUse))
                    {
                        sVehicleUse = sVehicleUse.ToLower();
                    }
                    oresult.sMessage = "Ocupation :" + sOccupation + " _typeOfMotorHome :" + sTypeOfMotorhome + " _VehicleUse" + sVehicleUse + " _OccupationTerm" + oOccupationTerm;
                    oIB.SaveErrortoDB(oresult);
                    if (!string.IsNullOrEmpty(oOccupationTerm) && !string.IsNullOrEmpty(sTypeOfMotorhome))
                    {
                        oOccupationTerm = oOccupationTerm.ToLower();
                        sTypeOfMotorhome = sTypeOfMotorhome.ToLower();
                        if (oOccupationTerm.Contains("x") && sVehicleUse != "sdp")
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                        else if (oOccupationTerm == "h")
                        {
                            oNV1.sValue = "-5";
                        }
                        else if (oOccupationTerm == "q")
                        {
                            oNV1.sValue = "+0";
                        }
                        else if (oOccupationTerm == "s")
                        {
                            oNV1.sValue = "+10";
                        }
                        else if (oOccupationTerm == "l")
                        {
                            oNV1.sValue = "+25";
                        }
                        else if (oOccupationTerm == "r")
                        {
                            oNV1.sValue = "+5";
                        }
                        else if (oOccupationTerm == "u")
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetClaimLoadFactor(int NoOfClaims, List<XIIBO> ClaimI, string CoverStartDate, int iNoOfClaims, int iNoOfConvinctions)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "sNoOfClaims";
                    int demeritPoints = 0;
                    List<string> sClaimNamesExWindScreenAndGlass = new List<string>();
                    foreach (var Claim in ClaimI)
                    {
                        var sClaimName = Claim.AttributeI("sName").ResolveFK("display");
                        if (sClaimName.ToLower() != "windscreen" && sClaimName.ToLower() != "glass")
                        {
                            sClaimNamesExWindScreenAndGlass.Add(sClaimName);
                        }
                        var sFault = Claim.AttributeI("iWhoseFault").sResolvedValue;
                        var claimCost = Claim.AttributeI("rTotalClaimCost").sValue;
                        var dt3Years = DateTime.Now.AddYears(-3).Date;
                        var dtClaimDate = Convert.ToDateTime(Claim.AttributeI("dDate").sValue);
                        double rCost = 0;
                        if (double.TryParse(claimCost, out rCost))
                        { }
                        int dPoints = 0;
                        oresult.sMessage = "sClaimName" + sClaimName;
                        oIB.SaveErrortoDB(oresult);
                        if ((sFault.ToLower() == "policy holder fault" || sClaimName.ToLower() == "theft") && dtClaimDate > DateTime.Now.AddYears(-3).Date)
                        {
                            iNoOfClaims++;
                        }
                        if (sClaimName.ToLower() == "windscreen")
                        {
                            dPoints = 0;
                        }
                        else if (sFault.ToLower() == "non fault" && dtClaimDate > dt3Years)
                        {
                            dPoints = 1;
                        }
                        else if ((sClaimName.ToLower() == "fire" || sFault.ToLower() == "policy holder fault") && dtClaimDate > DateTime.Now.AddYears(-1).Date)
                        {
                            dPoints = 5;
                        }
                        else if ((sClaimName.ToLower() == "fire" || sFault.ToLower() == "policy holder fault") && dtClaimDate > DateTime.Now.AddYears(-3).Date && dtClaimDate <= DateTime.Now.AddYears(-1).Date)
                        {
                            dPoints = 4;
                        }
                        else if ((sClaimName.ToLower() == "theft" || sClaimName.ToLower() == "vandalism") && dtClaimDate > DateTime.Now.AddYears(-1).Date)
                        {
                            dPoints = 5;
                        }
                        else if ((sClaimName.ToLower() == "theft" || sClaimName.ToLower() == "vandalism") && dtClaimDate > DateTime.Now.AddYears(-3).Date && dtClaimDate <= DateTime.Now.AddYears(-1).Date)
                        {
                            dPoints = 4;
                        }
                        else if ((sFault.ToLower() == "non fault" || sFault.ToLower() == "policy holder fault" || sClaimName.ToLower() == "fire" || sClaimName.ToLower() == "theft" || sClaimName.ToLower() == "vandalism") && dtClaimDate <= DateTime.Now.AddYears(-5).Date)
                        {
                            dPoints = 0;
                        }
                        demeritPoints += dPoints;
                    }
                    if (sClaimNamesExWindScreenAndGlass != null && sClaimNamesExWindScreenAndGlass.Count >= 3 && iNoOfConvinctions > 2)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
                    }
                    oNV1.sValue = iNoOfClaims.ToString();
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oResult = demeritPoints;
                    //oresult = DemeritPoints(demeritPoints, "ClaimLoadFactor");
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

            public CResult GetConvictionsDemeritPoints(int NoOfConvictions, List<XIIBO> ConvictionI, string PolicyStartDate, int iProductID)
            {
                CResult oCResult = new CResult();
                XIIXI oIXI = new XIIXI();
                XIInstanceBase oIB = new XIInstanceBase();
                try
                {
                    string[] DeclineCodesWithinLastThreeeYrs = new string[] { "DD90", "DG40", "E012", "E018", "MR09", "MR19", "MR29", "MR39", "MR49", "MR59", "N0042", "N0043", "N0044", "N0045", "N0046", "NA01", "NA02", "NA03", "NE", "NI09", "NR09", "QI09", "SC", "SE", "TT", "UT", "XX" };
                    int demeritPoints = 0;
                    int convictionYear = 0; //int demeritPoints = 0;
                    foreach (var Conviction in ConvictionI)
                    {
                        CNV oNV = new CNV();
                        oNV.sName = "sMessage";
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                        var sDemeritpoints = "0";
                        var oConvictionI = oIXI.BOI("refConviction_T", Conviction.AttributeI("refconviction").sValue);
                        int iBan = 0;
                        if (int.TryParse(Conviction.AttributeI("sLengthOfSuspension").sValue, out iBan))
                        {
                        }
                        oCResult.sMessage = "BanMonths : " + iBan;
                        oIB.SaveErrortoDB(oCResult);
                        if (Conviction.AttributeI("dDate").sValue != null)
                        {
                            var result = GetAgeFromDOB(Conviction.AttributeI("ddate").sValue, PolicyStartDate);
                            if (result.xiStatus == 00 && result.oResult != null)
                            {
                                convictionYear = (int)result.oResult;
                            }
                        }
                        DateTime dDAte = Convert.ToDateTime(Conviction.AttributeI("dDate").sValue);
                        DateTime sPolicyStartDAte = Convert.ToDateTime(PolicyStartDate);
                        //double BanDays = 0;
                        DateTime ConvictionDate = DateTime.MinValue;
                        var ConvictionCode = oConvictionI.Attributes["scode"].sValue;
                        if (!string.IsNullOrEmpty(ConvictionCode))
                        {
                            ConvictionCode = ConvictionCode.TrimEnd();
                        }
                        string Prefix = ""; int numberPart = 0;
                        Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                        Match Convictionresult = re.Match(ConvictionCode);
                        if (Convictionresult.Success == true)
                        {
                            Prefix = Convictionresult.Groups[1].Value.ToUpper();
                            numberPart = Convert.ToInt32(Convictionresult.Groups[2].Value);
                        }
                        else
                        {
                            Prefix = ConvictionCode.ToUpper();
                        }
                        //
                        if (iBan > 0 && (Prefix != "DR" && ConvictionCode != "E004" && ConvictionCode != "E005"))
                        {
                            sDemeritpoints = "30";
                        }
                        else if (Array.IndexOf(DeclineCodesWithinLastThreeeYrs, Prefix) > -1 && dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte)
                        {
                            sDemeritpoints = "30";
                        }
                        else if (Prefix == "BA" && numberPart >= 40 && numberPart <= 66 && (dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte))
                        {
                            sDemeritpoints = "30";
                        }
                        else if (Prefix == "CD" && numberPart >= 40 && numberPart <= 90 && (dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte))
                        {
                            sDemeritpoints = "30";
                        }
                        else if (((Prefix == "DD" && numberPart >= 50 && numberPart <= 80) || (Prefix == "Z" && numberPart >= 1 && numberPart <= 11 && numberPart != 9)) && (dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte))
                        {
                            sDemeritpoints = "30";
                        }
                        else if (Prefix == "DG" && numberPart != 40)
                        {
                            sDemeritpoints = "30";
                        }
                        else if (Prefix == "DR" && (dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte))
                        {
                            sDemeritpoints = "10";
                        }
                        else if (Prefix == "DR" && (dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte))
                        {
                            sDemeritpoints = "10";
                        }
                        else if (Prefix == "DR" && (dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte) && numberPart != 40 && numberPart != 50 && numberPart != 60 && numberPart != 70 && numberPart != 90)
                        {
                            sDemeritpoints = "8";
                        }
                        else if (Prefix == "DR" && (dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte) && (numberPart == 40 || numberPart == 50 || numberPart == 60 || numberPart == 70 || numberPart == 90))
                        {
                            sDemeritpoints = "0";
                        }
                        else if (ConvictionCode == "NEND" && (dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte))
                        {
                            sDemeritpoints = "0";
                        }
                        else if (convictionYear <= 3)
                        {
                            var sKey = "";
                            //var ConvictionCode = oConvictionI.Attributes["scode"].sValue;
                            if (convictionYear >= 0 && convictionYear < 1)
                            {
                                sKey = ConvictionCode + " 0-1";
                            }
                            if (convictionYear >= 1 && convictionYear < 2)
                            {
                                sKey = ConvictionCode + " 1-2";
                            }
                            if (convictionYear >= 2 && convictionYear <= 3)
                            {
                                sKey = ConvictionCode + " 2-3";
                            }
                            oCResult.sMessage = "sConvictionKey_" + sKey;
                            oIB.SaveErrortoDB(oCResult);
                            XIAPI oXIAPI = new XIAPI();
                            List<XIWhereParams> oWhereParams = new List<XIWhereParams>();
                            oWhereParams.Add(new XIWhereParams { sField = "sName", sValue = sKey, sOperator = "=" });
                            oWhereParams.Add(new XIWhereParams { sField = "FkiProductID", sValue = iProductID.ToString(), sOperator = "=" });
                            //sDemeritpoints = oXIAPI.GetValue("refConvictionsDemerit_T", sKey, "sValue", "sName");
                            sDemeritpoints = oXIAPI.GetValue("refConvictionsDemerit_T", "sValue", oWhereParams);
                        }
                        else if ((ConvictionCode == "DR31" || ConvictionCode == "DR61") && dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte)
                        {
                            sDemeritpoints = "7";
                        }
                        else if (dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte && dDAte <= sPolicyStartDAte.AddYears(-3))
                        {
                            sDemeritpoints = "0";
                        }
                        else
                        {
                            sDemeritpoints = "30";
                        }
                        int dPoints = 0;
                        if (int.TryParse(sDemeritpoints, out dPoints))
                        {
                            demeritPoints += dPoints;
                        }
                        oCResult.oCollectionResult.Add(oNV);
                    }
                    oCResult.oResult = demeritPoints;
                    //oCResult = DemeritPoints(demeritPoints, "Convictions Load");
                }
                catch (Exception ex)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }

            public CResult DemeritPoints(int idemeritPoins, string sLoadName, List<CResult> oClaimConvictionList)
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
                    oNV2.sValue = sLoadName;
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    CNV oNV4 = new CNV();
                    oNV4.sName = "AdditionalExcess";
                    oNV4.sValue = "+0";
                    switch (idemeritPoins)
                    {
                        case 0:
                        case 1:
                        case 2:
                            oNV1.sValue = "+0.00";
                            oNV4.sValue = "+0";
                            break;
                        case 3:
                        case 4:
                            oNV1.sValue = "+5.00";
                            oNV4.sValue = "+0";
                            break;
                        case 5:
                            oNV1.sValue = "+7.50";
                            oNV4.sValue = "+0";
                            break;
                        case 6:
                            oNV1.sValue = "+10.00";
                            oNV4.sValue = "+0";
                            break;
                        case 7:
                            oNV1.sValue = "+15.00";
                            oNV4.sValue = "+100";
                            break;
                        case 8:
                            oNV1.sValue = "+20.00";
                            oNV4.sValue = "+100";
                            break;
                        case 9:
                            oNV1.sValue = "+25.00";
                            oNV4.sValue = "+100";
                            break;
                        case 10:
                            oNV1.sValue = "+30.00";
                            oNV4.sValue = "+100";
                            break;
                        case 11:
                            oNV1.sValue = "+35.00";
                            oNV4.sValue = "+100";
                            break;
                        case 12:
                            oNV1.sValue = "+40.00";
                            oNV4.sValue = "+100";
                            break;
                        case 13:
                            oNV1.sValue = "+45.00";
                            oNV4.sValue = "+200";
                            break;
                        case 14:
                            oNV1.sValue = "+50.00";
                            oNV4.sValue = "+200";
                            break;
                        case 15:
                            oNV1.sValue = "+55.00";
                            oNV4.sValue = "+200";
                            break;
                        case 16:
                            oNV1.sValue = "+60.00";
                            oNV4.sValue = "+200";
                            break;
                        case 17:
                            oNV1.sValue = "+65.00";
                            oNV4.sValue = "+200";
                            break;
                        case 18:
                            oNV1.sValue = "+70.00";
                            oNV4.sValue = "+200";
                            break;
                        case 19:
                            oNV1.sValue = "+75.00";
                            oNV4.sValue = "+200";
                            break;
                        case 20:
                            oNV1.sValue = "+80.00";
                            oNV4.sValue = "+200";
                            break;
                        case 21:
                            oNV1.sValue = "+90.00";
                            oNV4.sValue = "+200";
                            break;
                        case 22:
                            oNV1.sValue = "+100.00";
                            oNV4.sValue = "+200";
                            break;
                        case 23:
                            oNV1.sValue = "+110.00";
                            oNV4.sValue = "+200";
                            break;
                        case 24:
                            oNV1.sValue = "+120.00";
                            oNV4.sValue = "+200";
                            break;
                        case 25:
                            oNV1.sValue = "+230.00";
                            oNV4.sValue = "+200";
                            break;
                        case 26:
                            oNV1.sValue = "+240.00";
                            oNV4.sValue = "+200";
                            break;
                        default:
                            oNV1.sValue = "+0";
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                            break;
                    }
                    if (oNV.sValue != xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString() && oClaimConvictionList.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())))
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                    oresult.oCollectionResult.Add(oNV4);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    //oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }


            public CResult GetLeftHandOrRightHandLoas(string IsLeftHandDrive, string sEuropeanUseInDays)
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
                    oNV2.sValue = "LHD / RHD";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (IsLeftHandDrive == "10" && sEuropeanUseInDays == "10")
                    {
                        oNV1.sValue = "+5";
                    }
                    else if (IsLeftHandDrive == "10" && sEuropeanUseInDays == "20")
                    {
                        oNV1.sValue = "+16";
                    }
                    else if (IsLeftHandDrive == "10" && sEuropeanUseInDays == "30")
                    {
                        oNV1.sValue = "+38";
                    }
                    else if (IsLeftHandDrive == "10" && sEuropeanUseInDays == "40")
                    {
                        oNV1.sValue = "+58";
                    }
                    else if (IsLeftHandDrive == "20" && sEuropeanUseInDays == "10")
                    {
                        oNV1.sValue = "-5";
                    }
                    else if (IsLeftHandDrive == "20" && sEuropeanUseInDays == "20")
                    {
                        oNV1.sValue = "+5";
                    }
                    else if (IsLeftHandDrive == "20" && sEuropeanUseInDays == "30")
                    {
                        oNV1.sValue = "+25";
                    }
                    else if (IsLeftHandDrive == "20" && sEuropeanUseInDays == "40")
                    {
                        oNV1.sValue = "+50";
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }

            public CResult GetLicenceTypeLoad(string sTypeOfLicence, int LicenceHeldYears)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Licence Type";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (sTypeOfLicence.ToLower() != "eu" && sTypeOfLicence.ToLower() != "full uk")
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    else
                    {
                        if (LicenceHeldYears < 1)
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                        else if (LicenceHeldYears >= 1 && LicenceHeldYears < 2)
                        {
                            oNV1.sValue = "+50";
                        }
                        else if (LicenceHeldYears >= 2 && LicenceHeldYears < 3)
                        {
                            oNV1.sValue = "+30";
                        }
                        else if (LicenceHeldYears >= 3 && LicenceHeldYears < 4)
                        {
                            oNV1.sValue = "+10";
                        }
                        else
                        {
                            oNV1.sValue = "+0";
                        }
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }

            public CResult GetExcessValueLoad(double rCaravanValue, string sTypeOfCover)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                XIAPI oXIAPI = new XIAPI();
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
                    oNV2.sValue = "Excess Value";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    string sCaravanValue = string.Empty;

                    if (rCaravanValue > 0)
                    {
                        var rCaravan = (rCaravanValue / 1000);
                        string sCaravan = rCaravan.ToString();
                        int iCaravan = 0;
                        iCaravan = Convert.ToInt32(rCaravan);
                        double rCompCaravanVal = 0;
                        if (double.TryParse(iCaravan.ToString(), out rCompCaravanVal))
                        { }
                        if (rCaravan == rCompCaravanVal)
                        {
                            sCaravanValue = ((iCaravan - 1) * 1000) + 1 + " - " + (iCaravan) * 1000;
                        }
                        else
                        {
                            //var iCaravan = Convert.ToInt32(Math.Ceiling((rCaravanValue / 1000)));
                            sCaravanValue = (iCaravan * 1000) + 1 + " - " + (iCaravan + 1) * 1000;
                        }
                    }

                    List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                    oWHParams.Add(new XIWhereParams { sField = "sVehicleValue", sOperator = "=", sValue = sCaravanValue });
                    var oExcessLoad = oXIAPI.GetValue("ExcessLoad_T", sTypeOfCover, oWHParams);
                    if (!string.IsNullOrEmpty(oExcessLoad))
                    {
                        if (oExcessLoad.ToLower() == "decline")
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                        else
                        {
                            oNV1.sValue = oExcessLoad;
                        }
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }

            public CResult GetOwnerShipLoad(int iLengthOfOwnerShip, int iGroup, string sTypeOfMotorhome)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Vehicle Ownership";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";

                    if (iGroup >= 1 && iGroup <= 9)
                    {
                        if (iLengthOfOwnerShip < 24)
                        {
                            oNV1.sValue = "-0";
                        }
                        else if (iLengthOfOwnerShip >= 24 && iLengthOfOwnerShip < 36)
                        {
                            if (sTypeOfMotorhome == "COMP")
                            {
                                oNV1.sValue = "-3.25";
                            }
                            else
                            {
                                oNV1.sValue = "-5.50";
                            }
                        }
                        else if (iLengthOfOwnerShip >= 36 && iLengthOfOwnerShip < 48)
                        {
                            oNV1.sValue = "-6.50";
                        }
                        else if (iLengthOfOwnerShip >= 48 && iLengthOfOwnerShip < 61)
                        {
                            if (sTypeOfMotorhome == "COMP")
                            {
                                oNV1.sValue = "-8.62";
                            }
                            else
                            {
                                oNV1.sValue = "-6.50";
                            }
                        }
                        else if (iLengthOfOwnerShip >= 61)
                        {
                            oNV1.sValue = "-23";
                        }
                    }
                    else if (iGroup >= 10 && iGroup <= 14)
                    {
                        if (iLengthOfOwnerShip < 12)
                        {
                            oNV1.sValue = "+10.00";
                        }
                        else if (iLengthOfOwnerShip >= 12 && iLengthOfOwnerShip < 24)
                        {
                            oNV1.sValue = "+5.00";
                        }
                        else if (iLengthOfOwnerShip >= 24 && iLengthOfOwnerShip < 36)
                        {
                            oNV1.sValue = "+0.00";
                        }
                        else if (iLengthOfOwnerShip >= 36 && iLengthOfOwnerShip < 48)
                        {
                            oNV1.sValue = "-5.00";
                        }
                        else if (iLengthOfOwnerShip >= 48 && iLengthOfOwnerShip < 61)
                        {
                            oNV1.sValue = "-10.00";
                        }
                        else if (iLengthOfOwnerShip >= 61)
                        {
                            oNV1.sValue = "-23";
                        }
                    }
                    else if (iGroup >= 15 && iGroup <= 20)
                    {
                        if (iLengthOfOwnerShip < 12)
                        {
                            oNV1.sValue = "+13.00";
                        }
                        else if (iLengthOfOwnerShip >= 12 && iLengthOfOwnerShip < 24)
                        {
                            oNV1.sValue = "+10.00";
                        }
                        else if (iLengthOfOwnerShip >= 24 && iLengthOfOwnerShip < 36)
                        {
                            oNV1.sValue = "+0.00";
                        }
                        else if (iLengthOfOwnerShip >= 36 && iLengthOfOwnerShip < 48)
                        {
                            oNV1.sValue = "-5.00";
                        }
                        else if (iLengthOfOwnerShip >= 48 && iLengthOfOwnerShip < 61)
                        {
                            oNV1.sValue = "-10.00";
                        }
                        else if (iLengthOfOwnerShip >= 61)
                        {
                            oNV1.sValue = "-23";
                        }
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetSecurityLoad(string sSecurityDevice, string sTypeOfMotorhome)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Security";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (!string.IsNullOrEmpty(sTypeOfMotorhome))
                    {
                        sTypeOfMotorhome = sTypeOfMotorhome.ToLower();
                    }
                    if (sSecurityDevice == "T1 Device" && (sTypeOfMotorhome == "comp" || sTypeOfMotorhome == "tpft"))
                    {
                        oNV1.sValue = "-5.00";
                    }
                    else if (sSecurityDevice == "T2 Device" && (sTypeOfMotorhome == "comp" || sTypeOfMotorhome == "tpft"))
                    {
                        oNV1.sValue = "-2.50";
                    }
                    else if (sSecurityDevice == "T2 - 1 Device" && (sTypeOfMotorhome == "comp" || sTypeOfMotorhome == "tpft"))
                    {
                        oNV1.sValue = "-5.00";
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetNoClaimsDiscount(int iMotorHomeNoclaimYear, int iPrivateClaimNoclaimYear)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "No Claims Discount";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    int iNoClaimYear = 0;
                    if (iMotorHomeNoclaimYear == 1)
                    {
                        iNoClaimYear = iPrivateClaimNoclaimYear;
                    }
                    else
                    {
                        iNoClaimYear = iMotorHomeNoclaimYear;
                    }
                    switch (iNoClaimYear)
                    {
                        case 1:
                            oNV1.sValue = "+0";
                            break;
                        case 2:
                            oNV1.sValue = "-30";
                            break;
                        case 3:
                            oNV1.sValue = "-40";
                            break;
                        case 4:
                            oNV1.sValue = "-50";
                            break;
                        case 5:
                            oNV1.sValue = "-55";
                            break;
                        case 6:
                            oNV1.sValue = "-60";
                            break;
                        case 7:
                            oNV1.sValue = "-62";
                            break;
                        case 8:
                            oNV1.sValue = "-64";
                            break;
                        default:
                            oNV1.sValue = "-66";
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;

            }

            public CResult GetProtectedNoClaimBonus(string IsProtectedNoClaimBonus, int iNoClaimYears, int iNoOfClaims)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Protected No Claims Discount";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (!string.IsNullOrEmpty(IsProtectedNoClaimBonus) && IsProtectedNoClaimBonus.ToLower() == "yes")
                    {
                        if (iNoClaimYears >= 5 && iNoOfClaims <= 1)
                        {
                            oNV1.sValue = "+10";
                        }
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;

            }

            public CResult GetClubMemberDiscount(string IsClubMember)
            {
                CResult oresult = new CResult();
                XIInstanceBase oIB = new XIInstanceBase();
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
                    oNV2.sValue = "Club Membership";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (IsClubMember.ToLower() == "10")
                    {
                        oNV1.sValue = "-10";
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }



            public CResult GetVoluntaryExcessLoad(int VoluntaryExcess, string sTypeOfMotorhome)
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
                    oNV2.sValue = "Voluntary Excess";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    CNV oNV4 = new CNV();
                    oNV4.sName = "MaxDiscount";
                    oNV4.sValue = "+0";
                    if (!string.IsNullOrEmpty(sTypeOfMotorhome))
                    {
                        sTypeOfMotorhome = sTypeOfMotorhome.ToLower();
                    }
                    if (sTypeOfMotorhome == "comp")
                    {
                        switch (VoluntaryExcess)
                        {
                            case 100:
                                oNV1.sValue = "-7.5";
                                oNV4.sValue = "-50";
                                break;
                            case 150:
                                oNV1.sValue = "-10";
                                oNV4.sValue = "-75";
                                break;
                            case 200:
                                oNV1.sValue = "-15";
                                oNV4.sValue = "-100";
                                break;
                            case 250:
                                oNV1.sValue = "-20";
                                oNV4.sValue = "-100";
                                break;
                            default:
                                oNV1.sValue = "+0";
                                oNV4.sValue = "-0";
                                break;
                        }
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                    oresult.oCollectionResult.Add(oNV4);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    //oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetMinimumPremium(string sTypeOfMotorhome, double iCurrentCaravanValue, int iGroup)
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
                    oNV2.sValue = "Minimum Premium";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Value";

                    if (sTypeOfMotorhome == "TPO" || sTypeOfMotorhome == "TPFT")
                    {
                        if (iCurrentCaravanValue <= 50000)
                        {
                            oNV1.sValue = "+75";
                        }
                        else
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                    }
                    else
                    {
                        if (iGroup >= 18 && iGroup <= 20)
                        {
                            if (iCurrentCaravanValue <= 75000)
                            {
                                oNV1.sValue = "+300";
                            }
                            else
                            {
                                oNV1.sValue = "+375";
                            }
                        }
                        else if (iGroup == 17)
                        {
                            if (iCurrentCaravanValue <= 75000)
                            {
                                oNV1.sValue = "+250";
                            }
                            else
                            {
                                oNV1.sValue = "+325";
                            }
                        }
                        else if (iGroup == 16)
                        {
                            if (iCurrentCaravanValue <= 75000)
                            {
                                oNV1.sValue = "+225";
                            }
                            else
                            {
                                oNV1.sValue = "+300";
                            }
                        }
                        else if (iGroup == 15)
                        {
                            if (iCurrentCaravanValue <= 70000)
                            {
                                oNV1.sValue = "+200";
                            }
                            else if (iCurrentCaravanValue > 70000 && iCurrentCaravanValue <= 75000)
                            {
                                oNV1.sValue = "+225";
                            }
                            else
                            {
                                oNV1.sValue = "+300";
                            }
                        }
                        else if (iGroup == 14)
                        {
                            if (iCurrentCaravanValue <= 60000)
                            {
                                oNV1.sValue = "+175";
                            }
                            else if (iCurrentCaravanValue > 60000 && iCurrentCaravanValue <= 70000)
                            {
                                oNV1.sValue = "+200";
                            }
                            else if (iCurrentCaravanValue > 70000 && iCurrentCaravanValue <= 75000)
                            {
                                oNV1.sValue = "+225";
                            }
                            else
                            {
                                oNV1.sValue = "+300";
                            }
                        }
                        else if (iGroup >= 10 && iGroup <= 13)
                        {
                            if (iCurrentCaravanValue <= 50000)
                            {
                                oNV1.sValue = "+150";
                            }
                            else if (iCurrentCaravanValue > 50000 && iCurrentCaravanValue <= 60000)
                            {
                                oNV1.sValue = "+175";
                            }
                            else if (iCurrentCaravanValue > 60000 && iCurrentCaravanValue <= 70000)
                            {
                                oNV1.sValue = "+200";
                            }
                            else if (iCurrentCaravanValue > 70000 && iCurrentCaravanValue <= 75000)
                            {
                                oNV1.sValue = "+225";
                            }
                            else
                            {
                                oNV1.sValue = "+300";
                            }
                        }
                        else if (iGroup >= 1 && iGroup <= 9)
                        {
                            if (iCurrentCaravanValue <= 50000)
                            {
                                oNV1.sValue = "+100";
                            }
                            else if (iCurrentCaravanValue > 50000 && iCurrentCaravanValue <= 60000)
                            {
                                oNV1.sValue = "+175";
                            }
                            else if (iCurrentCaravanValue > 60000 && iCurrentCaravanValue <= 70000)
                            {
                                oNV1.sValue = "+200";
                            }
                            else if (iCurrentCaravanValue > 70000 && iCurrentCaravanValue <= 75000)
                            {
                                oNV1.sValue = "+225";
                            }
                            else
                            {
                                oNV1.sValue = "+300";
                            }
                        }
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }

            public CResult GetMaxLoadFactor(List<CResult> oResult)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult result = new CResult();
                try
                {
                    result = oResult.FirstOrDefault();

                    if (oResult.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())))
                    {
                        result = oResult.Where(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Select(x => x.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()).FirstOrDefault();

                    }
                    else if (oResult.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Select(x => x.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()))
                    {
                        result = oResult.Where(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Select(x => x.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()).FirstOrDefault();
                    }
                    else
                    {
                        foreach (var item in oResult)
                        {
                            if (Convert.ToDouble(result.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) < Convert.ToDouble(item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()))
                                result = item;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.sMessage = "ERROR: [" + result.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    result.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    result.LogToFile();
                    oIB.SaveErrortoDB(result);
                }
                return result;
            }

            public CResult GetAgeFromDOB(string dDOB, string PresentDate)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    oresult.sMessage = "dDOB_" + dDOB;
                    oIB.SaveErrortoDB(oresult);
                    int iAge = DateTime.Now.Year - Convert.ToDateTime(dDOB).Year;
                    if (DateTime.Now < Convert.ToDateTime(dDOB).AddYears(iAge)) iAge--;
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

            public CResult GetMedicalConditionLoad(string sMedicalCondition)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                XIAPI oXIAPI = new XIAPI();
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
                    oNV2.sValue = "Medical Condition";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                    oWHParams.Add(new XIWhereParams { sField = "sTypeOfMedicalCondition", sOperator = "=", sValue = sMedicalCondition });
                    var oMedicalConditionLoad = oXIAPI.GetValue("MedicalConditionLoad_T", "bIsDecline", oWHParams);
                    oresult.sMessage = "MedicalCondition_" + sMedicalCondition + " _Result:" + oMedicalConditionLoad;
                    oIB.SaveErrortoDB(oresult);
                    if (!string.IsNullOrEmpty(oMedicalConditionLoad) && oMedicalConditionLoad.ToLower() == "true")
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetEmploymentTypeLoad(string sEmploymentType)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Employment Type";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (!string.IsNullOrEmpty(sEmploymentType))
                    {
                        sEmploymentType = sEmploymentType.ToLower();
                    }
                    if (sEmploymentType == "unemployed")
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetModificationLoad(List<string> Modificationlist, string sTypeOfMotorhome, int iProductID)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                XIAPI oXIAPI = new XIAPI();
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
                    oNV2.sValue = "Modification";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    CNV oNV4 = new CNV();
                    oNV4.sName = "AdditionalExcess";
                    oNV4.sValue = "+0";
                    string sField = string.Empty;
                    if (!string.IsNullOrEmpty(sTypeOfMotorhome))
                    {
                        sTypeOfMotorhome = sTypeOfMotorhome.ToLower();
                    }
                    if (sTypeOfMotorhome == "comp")
                    {
                        sField = "COMP";
                    }
                    else
                    {
                        sField = "NonCOMP";
                    }
                    int DemeritPoints = 0;
                    foreach (var sModification in Modificationlist)
                    {
                        int iPoints = 0;
                        List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                        oWHParams.Add(new XIWhereParams { sField = "FkiModificationID", sOperator = "=", sValue = sModification });
                        oWHParams.Add(new XIWhereParams { sField = "FKiProductID", sOperator = "=", sValue = iProductID.ToString() });
                        var oMedicalConditionLoad = oXIAPI.GetValue("ModificationsLoad_T", sField, oWHParams);
                        oresult.sMessage = "Modification :" + sModification + " _Result" + oMedicalConditionLoad;
                        oIB.SaveErrortoDB(oresult);
                        if (!string.IsNullOrEmpty(oMedicalConditionLoad))
                        {
                            if (oMedicalConditionLoad.ToLower() == "decline")
                            {
                                oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                            }
                            else
                            {
                                if (int.TryParse(oMedicalConditionLoad, out iPoints))
                                { }
                            }
                        }
                        DemeritPoints += iPoints;
                    }
                    if (oNV.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())
                    {
                        if (DemeritPoints <= 5)
                        {
                            oNV1.sValue = "+0";
                            oNV4.sValue = "+0";
                        }
                        else if (DemeritPoints >= 6 && DemeritPoints <= 9)
                        {
                            oNV1.sValue = "+25";
                            oNV4.sValue = "+50";
                        }
                        else if (DemeritPoints >= 10 && DemeritPoints <= 14)
                        {
                            oNV1.sValue = "+50";
                            oNV4.sValue = "+50";
                        }
                        else if (DemeritPoints >= 15)
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV3);
                    oresult.oCollectionResult.Add(oNV4);
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
        }
        public class PolicyCalculation
        {
            public CResult Calculation(int iInsatnceID, int iUserID, int iCustomerID, string sDataBase, string sProductName, string sVersion, string sProductCode, string sSessionID, string sUID, int iQuoteID)
            {
                List<string> Info = new List<string>();
                Info.Add("QsInstanceID_" + iInsatnceID);
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                try
                {
                    List<CResult> oGeneralDeclines = new List<CResult>();
                    XIDXI oXIDXI = new XIDXI();
                    PolicyBaseCalc PolicyCal = new PolicyBaseCalc();
                    List<CResult> oXiResults = new List<CResult>();
                    XIIXI oIXI = new XIIXI();
                    XIIBO oBII = new XIIBO();
                    List<CResult> CompulsaryExcessResult = new List<CResult>();
                    List<CResult> MaritalStatusResult = new List<CResult>();
                    List<CResult> AgesResult = new List<CResult>();
                    List<CResult> OccupationResult = new List<CResult>();
                    List<CResult> oDrivingQualificationResult = new List<CResult>();
                    List<string> oRelation = new List<string>();
                    List<CResult> MedicalConditionResult = new List<CResult>();
                    List<CResult> EmploymentTypeLoadResult = new List<CResult>();
                    string DOB = "";
                    string sOccupation = "";
                    string sSecondaryOccupation = "";
                    XIInfraCache oCache = new XIInfraCache();
                    int NoOfClaims = 0;
                    oBII.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations", null);
                    var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
                    int iProductID = 0;
                    string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                    if (int.TryParse(ProductID, out iProductID)) { }
                    var oProductI = oIXI.BOI("Product", ProductID);
                    int iClassID = 0;
                    if (oProductI != null && oProductI.Attributes.ContainsKey("FKiClassID"))
                    {
                        string sClassID = oProductI.Attributes["FKiClassID"].sValue;
                        if (int.TryParse(sClassID, out iClassID))
                        { }
                    }
                    CResult ClaimResult = new CResult();
                    var result = new CResult();
                    CResult ConvictionResult = new CResult();
                    List<CResult> ClaimConvictionResultList = new List<CResult>();
                    CResult ClaimConvictionResult = new CResult();
                    var oQSI = oCache.Get_QsStructureObj(sSessionID, sUID, "QSInstance_" + iInsatnceID + "NotationStructure");
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    var oLIst = oQSI.oSubStructureI("Driver_T");
                    var oDriversI = (List<XIIBO>)oLIst.oBOIList;
                    string dtInsuranceCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;
                    int iNoOfClaims = 0;
                    int iDemeritPoints = 0;
                    int NoOfConvinctions = 0;
                    double AdditionalExcess = 0;
                    string TypeOfCover = ostructureInstance.XIIValue("sTypeofCoverUpdated").sValue;
                    string sUseOfVechile = ostructureInstance.XIIValue("sTypeOfDrivingupdated").sValue;
                    List<CResult> OccupationsResult = new List<CResult>();
                    foreach (var item in oDriversI)
                    {
                        int Age = 0;
                        Info.Add("DriverID_" + item.Attributes["id"].sValue + "_QSInstanceID_" + iInsatnceID);
                        item.oBOStructure = item.SubChildI;
                        DOB = item.AttributeI("dDOB").sValue;
                        var dDOB = "";
                        DateTime dtDOB = DateTime.MinValue;
                        if (DateTime.TryParse(DOB, out dtDOB))
                        {
                            dDOB = dtDOB.ToString("dd/MM/yyyy");
                        }
                        QueryEngine oQE = new QueryEngine();
                        List<XIWhereParams> oWParams = new List<XIWhereParams>();
                        XIWhereParams oWP = new XIWhereParams();
                        oWP.sField = "Name_6";
                        oWP.sOperator = "=";
                        oWP.sValue = item.AttributeI("sName").sValue;
                        oWParams.Add(oWP);
                        oWP = new XIWhereParams();
                        oWP.sField = "Name_1";
                        oWP.sOperator = "=";
                        oWP.sValue = item.AttributeI("sForeName").sValue;
                        oWParams.Add(oWP);
                        oWP = new XIWhereParams();
                        oWP.sField = "DOB";
                        oWP.sOperator = "=";
                        oWP.sValue = dDOB;
                        oWParams.Add(oWP);
                        oWP = new XIWhereParams();
                        oWP.sField = "izXDeleted";
                        oWP.sOperator = "=";
                        oWP.sValue = "0";
                        oWParams.Add(oWP);
                        oQE.AddBO("sanctionsconlist", null, oWParams);
                        CResult oCresult = oQE.BuildQuery();
                        if (oCresult.bOK && oCresult.oResult != null)
                        {
                            var sSql = (string)oCresult.oResult;
                            ExecutionEngine oEE = new ExecutionEngine();
                            oEE.XIDataSource = oQE.XIDataSource;
                            oEE.sSQL = sSql;
                            var oQResult = oEE.Execute();
                            if (oQResult.bOK && oQResult.oResult != null)
                            {
                                var oSanctionList = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                                if (oSanctionList.Count > 0)
                                {
                                    oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "H M Sanctions", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                                }
                            }
                        }

                        //Compulsary Excess
                        int dpassedyr = 0;
                        var dPassed = item.AttributeI("dDateTestPassed").sResolvedValue;
                        Info.Add("dDateTestPassed" + dPassed);
                        var ores = PolicyCal.GetAgeFromDOB(dPassed, DateTime.Now.ToString());
                        if (ores.xiStatus == 0 && ores.oResult != null)
                        {
                            dpassedyr = (int)ores.oResult;
                            Info.Add("dDateTestPassedYears_" + dpassedyr);
                        }
                        result = PolicyCal.GetAgeFromDOB(DOB, DateTime.Now.ToString());
                        if (result.xiStatus == 00 && result.oResult != null)
                        {
                            Age = (int)result.oResult;
                            result = PolicyCal.CompulsaryExcess(Age, dpassedyr);
                            if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                            {
                                CompulsaryExcessResult.Add(result);
                            }
                        }
                        XIIBO oBOI1 = new XIIBO();
                        oBOI1.iBODID = item.iBODID;
                        item.Attributes["enumRelationship"].BOI = oBOI1;
                        string srelation = item.AttributeI("enumRelationship").ResolveFK("display1");
                        if (!string.IsNullOrEmpty(srelation))
                        {
                            oRelation.Add(srelation.ToLower());
                        }
                        string sMaritalStatus = item.AttributeI("sMaritalStatus").ResolveFK("display1");
                        var oMaritalStatusLoad = PolicyCal.GetMaritalStatusLoadFactor(sMaritalStatus);
                        if (oMaritalStatusLoad.bOK && oMaritalStatusLoad.oCollectionResult.Count > 0)
                        {
                            MaritalStatusResult.Add(oMaritalStatusLoad);
                        }
                        var oDrivingQualifiication = oIXI.BOI("enumLicenceType_T", item.AttributeI("sDrivingQualification").sValue);
                        string sDrivingQualifiication = oDrivingQualifiication.AttributeI("sName").sValue;
                        oResult.sMessage = "sDrivingQualifiication" + sDrivingQualifiication;
                        oIB.SaveErrortoDB(oResult);
                        var oTypeOfDrivingLoad = PolicyCal.GetLicenceTypeLoad(sDrivingQualifiication, dpassedyr);
                        if (oTypeOfDrivingLoad.bOK && oTypeOfDrivingLoad.oCollectionResult.Count > 0)
                        {
                            oDrivingQualificationResult.Add(oTypeOfDrivingLoad);
                        }


                        var ConvinctionsList = item.oStructureI("Conviction_T");
                        NoOfConvinctions = ConvinctionsList.Count();
                        if (NoOfConvinctions > 0)
                        {
                            result = PolicyCal.GetConvictionsDemeritPoints(NoOfConvinctions, ConvinctionsList, dtInsuranceCoverStartDate, iProductID);
                            if (result.xiStatus == 0)
                            {
                                int ipoints = 0;
                                int.TryParse(result.oResult.ToString(), out ipoints);
                                iDemeritPoints = iDemeritPoints + ipoints;
                                //ConvictionResult = result;
                            }
                            ClaimConvictionResultList.Add(result);
                        }
                        var sSubList = item.oStructureI("Claim_T");
                        NoOfClaims = sSubList.Count();
                        if (NoOfClaims > 0)
                        {
                            result = PolicyCal.GetClaimLoadFactor(NoOfClaims, sSubList, dtInsuranceCoverStartDate, iNoOfClaims, NoOfConvinctions);
                            if (result.xiStatus == 0)
                            {
                                string Count = result.oCollectionResult.Where(x => x.sName == "sNoOfClaims").Select(m => m.sValue).FirstOrDefault();
                                if (int.TryParse(Count, out iNoOfClaims))
                                { }
                                int ipoints = 0;
                                int.TryParse(result.oResult.ToString(), out ipoints);
                                iDemeritPoints = iDemeritPoints + ipoints;
                                //ClaimResult = result;
                            }
                        }

                        var MedicalConditionList = item.oStructureI("MedicalCondition_T");
                        var NoOfMedicalConditions = MedicalConditionList.Count();
                        if (NoOfMedicalConditions > 0)
                        {
                            foreach (var oMedicalCondition in MedicalConditionList)
                            {
                                //Info.Add("sMedicalCondition_" + oMedicalCondition.AttributeI("refMedicalCondition").ResolveFK("Display") + " Medical Resolve_" +oMedicalCondition.AttributeI("refMedicalCondition").sValue);
                                var oMedicalConditionResult = PolicyCal.GetMedicalConditionLoad(oMedicalCondition.AttributeI("refMedicalCondition").ResolveFK("display"));
                                //var oMedicalConditionResult = PolicyCal.GetMedicalConditionLoad(oMedicalCondition.Attributes["refMedicalCondition"].sResolvedValue);
                                if (oMedicalConditionResult.bOK && oMedicalConditionResult.oCollectionResult.Count > 0)
                                {
                                    MedicalConditionResult.Add(oMedicalConditionResult);
                                }
                            }
                        }
                        string sEmployementType = item.AttributeI("sEmploymentType").ResolveFK("display1");
                        if (!string.IsNullOrEmpty(sEmployementType))
                        {
                            var EmploymentTypeLoad = PolicyCal.GetEmploymentTypeLoad(sEmployementType);
                            if (EmploymentTypeLoad.bOK && EmploymentTypeLoad.oCollectionResult.Count > 0)
                            {
                                EmploymentTypeLoadResult.Add(EmploymentTypeLoad);
                            }
                        }
                        //Secondary
                        string sSecEmploymentType = item.AttributeI("sSecondaryEmploymentType").ResolveFK("display1");
                        if (!string.IsNullOrEmpty(sSecEmploymentType))
                        {
                            var SecEmploymentTypeLoad = PolicyCal.GetEmploymentTypeLoad(sSecEmploymentType);
                            if (SecEmploymentTypeLoad.bOK && SecEmploymentTypeLoad.oCollectionResult.Count > 0)
                            {
                                EmploymentTypeLoadResult.Add(SecEmploymentTypeLoad);
                            }
                        }


                        sOccupation = item.AttributeI("enumOccupatation").sResolvedValue;
                        //oResult.sMessage = "sOccupation_" + sOccupation;
                        //oIB.SaveErrortoDB(oResult);
                        Info.Add("sOccupation_" + sOccupation);
                        result = PolicyCal.GetOccupationLoad(sOccupation, TypeOfCover, sUseOfVechile, iClassID);
                        if (result.xiStatus == 00 && result.oCollectionResult.Count > 0)
                        {
                            OccupationResult.Add(result);
                        }
                        sSecondaryOccupation = item.AttributeI("sSecondaryOccupation").sResolvedValue;
                        if (!string.IsNullOrEmpty(sSecondaryOccupation))
                        {
                            Info.Add("Secondary Occupation_" + sSecondaryOccupation);
                            result = PolicyCal.GetOccupationLoad(sSecondaryOccupation, TypeOfCover, sUseOfVechile, iClassID);
                            if (result.xiStatus == 00 && result.oCollectionResult.Count > 0)
                            {
                                OccupationResult.Add(result);
                            }
                        }


                        int dUkResyr = 0;
                        var bSinceBirth = item.AttributeI("bSinceBirth").sValue;
                        //oResult.sMessage = "bSinceBirth" + bSinceBirth;
                        //oIB.SaveErrortoDB(oResult);
                        Info.Add("bSinceBirth_" + bSinceBirth);
                        if (!string.IsNullOrEmpty(bSinceBirth) && bSinceBirth.ToLower() == "true")
                        {
                            dUkResyr = Age;
                            //oResult.sMessage = "bSinceBirth_UkresYear" + dUkResyr;
                            //oIB.SaveErrortoDB(oResult);
                            Info.Add("bSinceBirth_UkresYear" + dUkResyr);
                        }
                        else
                        {
                            var UKResidentYear = item.AttributeI("iUKResidentYear").sValue;
                            //oResult.sMessage = "iUKResidentYear" + UKResidentYear;
                            //oIB.SaveErrortoDB(oResult);
                            Info.Add("iUKResidentYear_" + UKResidentYear);
                            var UKResidentMonth = item.AttributeI("iUKResidentMonth").sValue;
                            //oResult.sMessage = "iUKResidentMonth" + UKResidentMonth;
                            //oIB.SaveErrortoDB(oResult);
                            Info.Add("iUKResidentMonth_" + UKResidentMonth);
                            int iUKResidentYear = 0; int iUKResidentMonth = 0;
                            if (int.TryParse(UKResidentYear, out iUKResidentYear))
                            {
                                if (int.TryParse(UKResidentMonth, out iUKResidentMonth))
                                {
                                }
                                dUkResyr = (DateTime.Now.Year - iUKResidentYear);
                                if (DateTime.Now.Month < iUKResidentMonth) dUkResyr--;
                                //oResult.sMessage = "UkresMonth&Year_UkresYear" + dUkResyr;
                                //oIB.SaveErrortoDB(oResult);
                                Info.Add("UkresMonth&Year_UkresYear_" + dUkResyr);
                            }
                        }
                        //var sUKResidency = item.AttributeI("sUKResidency").sResolvedValue;
                        //var oukresidencyres = PolicyCal.GetAgeFromDOB(sUKResidency, DateTime.Now.ToString());
                        //if (oukresidencyres.xiStatus == 0 && oukresidencyres.oResult != null)
                        //{
                        //    dUkResyr = (int)oukresidencyres.oResult;
                        if (dUkResyr < 3)
                        {
                            oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "UK Residency", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                        }

                    }
                    ClaimConvictionResult = PolicyCal.DemeritPoints(iDemeritPoints, "Claim/Conviction Demerit Points LoadFactor", ClaimConvictionResultList);
                    string sCaravanValue = ostructureInstance.XIIValue("iCurrentCaravanValue").sValue;
                    double rCaravanVal = 0;
                    if (double.TryParse(sCaravanValue, out rCaravanVal))
                    { }

                    string sEngineSize = ostructureInstance.XIIValue("sEngineSize").sValue;
                    string sMileage = ostructureInstance.XIIValue("iMileage").sValue;

                    string sYearOfManufacture = ostructureInstance.XIIValue("iYearOfManufactureUpdated").sValue;
                    string bISLeftHandDrive = ostructureInstance.XIIValue("bISLeftHandDrive").sValue;
                    string bIsClubMember = ostructureInstance.XIIValue("iIsClubMember").sValue;
                    string sTypeofdevice = ostructureInstance.XIIValue("iTypeofdevice").sDerivedValue;
                    string sVoluntaryExcess = ostructureInstance.XIIValue("VoluntaryExcess").sValue;
                    string sNoClaimsYears = ostructureInstance.XIIValue("iNoClaimsYears").sValue;
                    string sPostCode = ostructureInstance.XIIValue("sPostCode").sValue;
                    string sParkingPostCode = ostructureInstance.XIIValue("sParkingPostCode").sValue;
                    string sGaraging = ostructureInstance.XIIValue("sWheredoyoustoreUpdated").sDerivedValue;
                    string sLiketoProtectYourNoClaimsDiscountUpdated = ostructureInstance.XIIValue("sLiketoProtectYourNoClaimsDiscountUpdated").sResolvedValue;
                    string sNoSeats = ostructureInstance.XIIValue("iNoofSeats").sValue;
                    int iNoClaimYears = 0;
                    if (int.TryParse(sNoClaimsYears, out iNoClaimYears))
                    { }
                    string sPrivatenocclaim = ostructureInstance.XIIValue("Privatenocclaim").sValue;
                    int iPrivateClaimYears = 0;
                    if (int.TryParse(sPrivatenocclaim, out iPrivateClaimYears))
                    { }
                    string sVehicleOwner = ostructureInstance.XIIValue("sVehicleOwner").sDerivedValue;
                    string sVehicleKeeper = ostructureInstance.XIIValue("sVehicleKeeper").sDerivedValue;
                    oResult.sMessage = "sVehicleOwner_" + sVehicleOwner;
                    oIB.SaveErrortoDB(oResult);
                    oResult.sMessage = "sVehicleKeeper_" + sVehicleKeeper;
                    oIB.SaveErrortoDB(oResult);
                    string sEuropeanUseInDays = ostructureInstance.XIIValue("Europeperyear").sValue;
                    Info.Add("bISLeftHandDrive:" + bISLeftHandDrive);
                    if (string.IsNullOrEmpty(bISLeftHandDrive))
                    {
                        string bISRightHandDrive = ostructureInstance.XIIValue("bISRightHandDrive").sValue;
                        Info.Add("bISRightHandDrive:" + bISRightHandDrive);
                        if (!string.IsNullOrEmpty(bISRightHandDrive))
                        {
                            if (bISRightHandDrive == "10")
                            {
                                bISLeftHandDrive = "20";
                            }
                            else
                            {
                                bISLeftHandDrive = "10";
                            }
                        }
                        Info.Add("bISLeftHandDrive:" + bISLeftHandDrive);
                    }
                    int iEngine = 0;
                    if (int.TryParse(sEngineSize, out iEngine))
                    { }
                    string sDateofpurchase = "";
                    if (ostructureInstance.XIIValues.ContainsKey("iMotorhomebuyMonth") && ostructureInstance.XIIValues.ContainsKey("iMotorhomebuyYear"))
                    {
                        int iMonth = 0; int iYear = 0;
                        int.TryParse(ostructureInstance.XIIValue("iMotorhomebuyMonth").sValue, out iMonth);
                        int.TryParse(ostructureInstance.XIIValue("iMotorhomebuyYear").sValue, out iYear);
                        var dtDateofPurchase = new DateTime(iYear, iMonth, 1);
                        sDateofpurchase = dtDateofPurchase.ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        sDateofpurchase = ostructureInstance.XIIValue("dDateofpurchase").sValue;
                    }
                    //Compulsary Excess
                    var oCompulsaryExcess = PolicyCal.GetMaxLoadFactor(CompulsaryExcessResult);
                    if (oCompulsaryExcess.bOK && oCompulsaryExcess.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oCompulsaryExcess);
                    }
                    //Vehicle Value
                    var oVehicle = PolicyCal.GetVechileValue(rCaravanVal, TypeOfCover);
                    if (oVehicle.bOK && oVehicle.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oVehicle);
                    }
                    //BaseRate
                    var oBaseRate = PolicyCal.GetBaseRate(TypeOfCover);
                    if (oBaseRate.bOK && oBaseRate.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oBaseRate);
                    }
                    int iVehicleGroup = 0;
                    var oVehicleGroup = PolicyCal.GetVehicleGroup(iEngine);
                    if (oVehicleGroup.bOK && oVehicleGroup.oResult != null)
                    {
                        string sVehicleGroup = (string)oVehicleGroup.oResult;
                        if (int.TryParse(sVehicleGroup, out iVehicleGroup))
                        { }
                    }
                    //Vehicle Group
                    var oVehicleGroupLoad = PolicyCal.GetVehicleGroupLoad(iVehicleGroup, TypeOfCover);
                    if (oVehicleGroupLoad.bOK && oVehicleGroupLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oVehicleGroupLoad);
                    }
                    //Mileage
                    int iMileage = 0;
                    if (int.TryParse(sMileage, out iMileage))
                    { }
                    var oMileage = PolicyCal.GetMileageLoadFactor(iMileage, sUseOfVechile);
                    if (oMileage.bOK && oMileage.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oMileage);
                    }
                    //District
                    string sHomePostCodeArea = string.Empty;
                    var oArea = PolicyCal.GetArea(sPostCode, TypeOfCover);
                    if (!string.IsNullOrEmpty(oArea))
                    {
                        sHomePostCodeArea = oArea;
                    }
                    string sParkingPostCodeArea = string.Empty;
                    var oParkingArea = PolicyCal.GetArea(sPostCode, TypeOfCover);
                    if (!string.IsNullOrEmpty(oParkingArea))
                    {
                        sParkingPostCodeArea = oParkingArea;
                    }
                    List<CResult> DistrictLoadList = new List<CResult>();
                    var oHomeDistrict = PolicyCal.GetDisrictLoad(sHomePostCodeArea, TypeOfCover);
                    if (oHomeDistrict.bOK && oHomeDistrict.oCollectionResult.Count > 0)
                    {
                        DistrictLoadList.Add(oHomeDistrict);
                    }
                    var oParkingDistrict = PolicyCal.GetDisrictLoad(sParkingPostCodeArea, TypeOfCover);
                    if (oParkingDistrict.bOK && oParkingDistrict.oCollectionResult.Count > 0)
                    {
                        DistrictLoadList.Add(oParkingDistrict);
                    }
                    if (DistrictLoadList.Count > 0)
                    {
                        var oDistrictLoad = PolicyCal.GetMaxLoadFactor(DistrictLoadList);
                        if (oDistrictLoad.bOK && oDistrictLoad.oCollectionResult.Count > 0)
                        {
                            oXiResults.Add(oDistrictLoad);
                        }
                    }
                    //Area
                    List<CResult> AreaLoadList = new List<CResult>();
                    var HomeAreaLoad = PolicyCal.GetAreaLoad(sHomePostCodeArea, TypeOfCover);
                    if (HomeAreaLoad.bOK && HomeAreaLoad.oCollectionResult.Count > 0)
                    {
                        AreaLoadList.Add(HomeAreaLoad);
                    }
                    var ParkingAreaLoad = PolicyCal.GetAreaLoad(sParkingPostCodeArea, TypeOfCover);
                    if (ParkingAreaLoad.bOK && ParkingAreaLoad.oCollectionResult.Count > 0)
                    {
                        AreaLoadList.Add(ParkingAreaLoad);
                    }
                    if (AreaLoadList.Count > 0)
                    {
                        var oAreaLoad = PolicyCal.GetMaxLoadFactor(AreaLoadList);
                        if (oAreaLoad.bOK && oAreaLoad.oCollectionResult.Count > 0)
                        {
                            oXiResults.Add(oAreaLoad);
                        }
                    }
                    //Garaging
                    var oGaragingLoad = PolicyCal.GetGaragingLoad(sHomePostCodeArea, TypeOfCover, sGaraging);
                    if (oGaragingLoad.bOK && oGaragingLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oGaragingLoad);
                    }
                    //Vehicle Age
                    int iYearOfManufacture = 0;
                    if (int.TryParse(sYearOfManufacture, out iYearOfManufacture))
                    { }
                    var oAgeOfVehicleLoad = PolicyCal.GetAgeOfVechicleDiscount(iYearOfManufacture, TypeOfCover);
                    if (oAgeOfVehicleLoad.bOK && oAgeOfVehicleLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oAgeOfVehicleLoad);
                    }
                    //Driving Restriction
                    var oDrivingrestriction = PolicyCal.GetDrivingRestrictionsLoadFactor(oRelation);
                    if (oDrivingrestriction.bOK && oDrivingrestriction.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oDrivingrestriction);
                    }
                    //Marital Status
                    var oMaritalStatus = PolicyCal.GetMaxLoadFactor(MaritalStatusResult);
                    if (oMaritalStatus.bOK && oMaritalStatus.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oMaritalStatus);
                    }
                    //Occupation
                    var oOccupationsLoad = PolicyCal.GetMaxLoadFactor(OccupationResult);
                    if (oOccupationsLoad.bOK && oOccupationsLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oOccupationsLoad);
                    }
                    //Claims & Convictions (Demerit Points)
                    if (ClaimConvictionResult.xiStatus == 0 && ClaimConvictionResult.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(ClaimConvictionResult);
                        var AdditionalExcessLoad = ClaimConvictionResult.oCollectionResult.Where(m => m.sName == "AdditionalExcess").Select(m => m.sValue).FirstOrDefault();
                        if (double.TryParse(AdditionalExcessLoad, out AdditionalExcess))
                        { }
                    }
                    //LHD / RHD
                    var oLeftOrRightHandDriveload = PolicyCal.GetLeftHandOrRightHandLoas(bISLeftHandDrive, sEuropeanUseInDays);
                    if (oLeftOrRightHandDriveload.bOK && oLeftOrRightHandDriveload.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oLeftOrRightHandDriveload);
                    }
                    //Licence Type
                    var oLicenceTypeLoad = PolicyCal.GetMaxLoadFactor(oDrivingQualificationResult);
                    if (oLicenceTypeLoad.bOK && oLicenceTypeLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oLicenceTypeLoad);
                    }
                    //Excess Value
                    var oExcessLoad = PolicyCal.GetExcessValueLoad(rCaravanVal, TypeOfCover);
                    if (oExcessLoad.bOK && oExcessLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oExcessLoad);
                    }
                    //Vehicle Ownership
                    int iYearOfOwnership = 0;
                    int iLengthOfOwnershipInMonths = 0;
                    var oDateOfPurchase = PolicyCal.GetAgeFromDOB(sDateofpurchase, DateTime.Now.ToString());
                    if (oDateOfPurchase.xiStatus == 00 && oDateOfPurchase.oResult != null)
                    {
                        iYearOfOwnership = (int)oDateOfPurchase.oResult;
                        DateTime dDAte = Convert.ToDateTime(sDateofpurchase);
                        iLengthOfOwnershipInMonths = 12 * (DateTime.Now.Year - dDAte.Year) + DateTime.Now.Month - dDAte.Month;
                        if ((DateTime.Now < Convert.ToDateTime(sDateofpurchase).AddMonths(iLengthOfOwnershipInMonths))) iLengthOfOwnershipInMonths--;
                    }
                    var oOwnershipLoad = PolicyCal.GetOwnerShipLoad(iLengthOfOwnershipInMonths, iVehicleGroup, TypeOfCover);
                    if (oOwnershipLoad.bOK && oOwnershipLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oOwnershipLoad);
                    }
                    //Security
                    var oSecurityLoad = PolicyCal.GetSecurityLoad(sTypeofdevice, TypeOfCover);
                    if (oSecurityLoad.bOK && oSecurityLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oSecurityLoad);
                    }
                    //No Claims Discount
                    var oNoClaimBonus = PolicyCal.GetNoClaimsDiscount(iNoClaimYears, iPrivateClaimYears);
                    if (oNoClaimBonus.bOK && oNoClaimBonus.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oNoClaimBonus);
                    }
                    //Protected No Claims Discount
                    var oProtectedNoClaimBonus = PolicyCal.GetProtectedNoClaimBonus(sLiketoProtectYourNoClaimsDiscountUpdated, iNoClaimYears, iNoOfClaims);
                    if (oProtectedNoClaimBonus.bOK && oProtectedNoClaimBonus.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oProtectedNoClaimBonus);
                    }
                    //Club Membership
                    var oClubMemberShip = PolicyCal.GetClubMemberDiscount(bIsClubMember);
                    if (oClubMemberShip.bOK && oClubMemberShip.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oClubMemberShip);
                    }
                    //Modifications
                    var sModification = ostructureInstance.XIIValue("sModificationList").sValue;
                    List<string> ModificationsList = new List<string>();
                    if (!string.IsNullOrEmpty(sModification))
                    {
                        var Modifications = sModification.Split(',');
                        foreach (var Modification in Modifications)
                        {
                            ModificationsList.Add(Modification);
                        }
                    }
                    var oModificationLoad = PolicyCal.GetModificationLoad(ModificationsList, TypeOfCover, iProductID);
                    if (oModificationLoad.bOK && oModificationLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oModificationLoad);
                        var AdditionalExcessLoad = oModificationLoad.oCollectionResult.Where(m => m.sName == "AdditionalExcess").Select(m => m.sValue).FirstOrDefault();
                        double rModificationExcess = 0.0;
                        if (double.TryParse(AdditionalExcessLoad, out rModificationExcess))
                        { }
                        AdditionalExcess += rModificationExcess;
                    }
                    //Voluntary Excess
                    int iVoluntaryExcess = 0;
                    if (int.TryParse(sVoluntaryExcess, out iVoluntaryExcess))
                    { }
                    var oVoluntaryExcessLoad = PolicyCal.GetVoluntaryExcessLoad(iVoluntaryExcess, TypeOfCover);
                    if (oVoluntaryExcessLoad.bOK && oVoluntaryExcessLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oVoluntaryExcessLoad);
                    }

                    //MinimumPremium
                    var oMinimumPremium = new CResult();
                    var MinimumPremium = PolicyCal.GetMinimumPremium(TypeOfCover, rCaravanVal, iVehicleGroup);
                    if (MinimumPremium.xiStatus == 0 && MinimumPremium.oCollectionResult != null)
                    {
                        oMinimumPremium = MinimumPremium;
                    }
                    //IPT
                    var IPTLoadFactor = new CResult();
                    var oIPTLoadFactor = PolicyCal.GetIPTLoadFactor();
                    if (oIPTLoadFactor.xiStatus == 0 && oIPTLoadFactor.oCollectionResult != null)
                    {
                        IPTLoadFactor = oIPTLoadFactor;
                    }

                    //Medical Condition
                    var oMedicalConditionLoad = new CResult();
                    if (MedicalConditionResult != null && MedicalConditionResult.Count > 0)
                    {
                        oMedicalConditionLoad = PolicyCal.GetMaxLoadFactor(MedicalConditionResult);
                    }
                    //EmploymentType
                    var oEmploymentType = new CResult();
                    if (EmploymentTypeLoadResult != null && EmploymentTypeLoadResult.Count > 0)
                    {
                        oEmploymentType = PolicyCal.GetMaxLoadFactor(EmploymentTypeLoadResult);
                    }

                    //General Declines
                    int iNoOfSeats = 0;
                    if (int.TryParse(sNoSeats, out iNoOfSeats))
                    {
                    }
                    if (iNoOfSeats > 8)
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "No of seats", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    string[] DeclineOwnerList = new string[] { "company other than proposer", "vehicle leasing company", "parent", "common law partner", "son or daughter", "other", "employer", "employee", "proposer's business partner", "contract hire", "garage", "other family member" };
                    if (!string.IsNullOrEmpty(sVehicleOwner) && DeclineOwnerList.Contains(sVehicleOwner.ToLower()))
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Vehicle Owner", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    if (!string.IsNullOrEmpty(sVehicleKeeper) && DeclineOwnerList.Contains(sVehicleKeeper.ToLower()))
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Vehicle Keeper", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    string sInsurancePolicyDetails = ostructureInstance.XIIValue("sPreviousMotorOrCaravanInsurancePolicyDetails").sResolvedValue;
                    if (sInsurancePolicyDetails == "Yes")
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Refused or canceled insurance", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    string sMotorOffence = ostructureInstance.XIIValue("sMotorOffence").sResolvedValue;
                    if (sMotorOffence == "Yes")
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "non-motoring conviction", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    double total = 0;
                    double BaseLoad = 0;

                    if (oXiResults != null && oXiResults.Count() > 0)
                    {
                        int i = 0;
                        foreach (var item in oXiResults)
                        {
                            if (item.oCollectionResult != null)
                            {
                                Info.Add("AdditionExcess:" + AdditionalExcess);
                                if (AdditionalExcess > 0)
                                {
                                    var Excess = item.oCollectionResult.Where(m => m.sValue == "Compulsory Excess").Select(m => m.sValue).FirstOrDefault();
                                    Info.Add("Excess:" + Excess);
                                    if (!string.IsNullOrEmpty(Excess))
                                    {
                                        var ExcessAmount = Convert.ToDouble(item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                                        ExcessAmount += AdditionalExcess;
                                        Info.Add("Total Excess:" + ExcessAmount);
                                        item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue = ExcessAmount.ToString()).ToList();
                                    }
                                }
                                if (item.oCollectionResult.Where(m => m.sName == "Type").Select(m => m.sValue).FirstOrDefault() == "Percent")
                                {
                                    var LoadAmount = ((Convert.ToDouble(item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * BaseLoad);
                                    var MaxAmount = item.oCollectionResult.Where(m => m.sName == "MaxDiscount").Select(m => m.sValue).FirstOrDefault();
                                    double rMaxAmount = 0;
                                    if (!string.IsNullOrEmpty(MaxAmount))
                                    {
                                        if (double.TryParse(MaxAmount, out rMaxAmount))
                                        { }
                                    }
                                    if (rMaxAmount != 0 && rMaxAmount > LoadAmount)
                                    {
                                        LoadAmount = rMaxAmount;
                                    }
                                    BaseLoad += LoadAmount;
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
                    total = BaseLoad;
                    result = PolicyCal.BuildCResultObject(NetPremium.ToString(), "Net Premium", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString());
                    oXiResults.Add(result);
                    oProductI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Product");
                    oProductI.Attributes["iDefaultAmountType"].BOD = oProductI.BOD;
                    double rMinPremium = 0.0;
                    string sMinPremium = oMinimumPremium.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                    if (double.TryParse(sMinPremium, out rMinPremium))
                    {
                        if (rMinPremium > total)
                        {
                            total = rMinPremium;
                        }
                    }
                    oXiResults.Add(oMinimumPremium);
                    if (total > 1500)
                    {
                        result = PolicyCal.BuildCResultObject("0", "Premium Exceded", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                        oGeneralDeclines.Add(result);
                    }
                    double rAdditionalLoad = 0;
                    if (oProductVersionI.Attributes.ContainsKey("rAdditionalLoad"))
                    {
                        var AdditionLoad = oProductVersionI.Attributes["rAdditionalLoad"].sValue;
                        if (double.TryParse(AdditionLoad, out rAdditionalLoad))
                        {
                        }
                    }
                    if (oMedicalConditionLoad != null && oMedicalConditionLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oMedicalConditionLoad);
                    }
                    if (oEmploymentType != null && oEmploymentType.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oEmploymentType);
                    }
                    if (oXiResults.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())) && oMinimumPremium.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && oGeneralDeclines.Count <= 0)
                    {
                        Info.Add("All Load Factors are normal");
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "0", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "false", bDirty = true };
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())) || oMinimumPremium.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString() && oGeneralDeclines.Count <= 0)
                    {
                        Info.Add("Some Load Factors are Refered");
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "10", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "true", bDirty = true };
                        oXiResults.Add(PolicyCal.BuildCResultObject(rAdditionalLoad.ToString(), "Additional Load", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                        total += (total * 0.01) * rAdditionalLoad;
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())) || oMinimumPremium.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString() || oGeneralDeclines.Count > 0)
                    {
                        oResult.sMessage = "Some Load Factors are Declined";
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "20", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "true", bDirty = true };
                        oXiResults.Add(PolicyCal.BuildCResultObject(rAdditionalLoad.ToString(), "Additional Load", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                        total += (total * 0.01) * rAdditionalLoad;
                    }
                    if (oGeneralDeclines.Count > 0)
                    {
                        //oResult.sMessage = "oGeneralDeclines: Declined" + oGeneralDeclines.Count;
                        //oIB.SaveErrortoDB(oResult);
                        Info.Add("oGeneralDeclines: Declined" + oGeneralDeclines.Count);
                        foreach (var declineCase in oGeneralDeclines)
                        {
                            oXiResults.Add(declineCase);
                        }
                    }
                    double IPT = 0;
                    double rInterestRate = 0;
                    if (IPTLoadFactor.oCollectionResult != null)
                    {
                        rInterestRate = Convert.ToDouble(IPTLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                        IPT = ((Convert.ToDouble(IPTLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * total);
                        //IPT = Math.Round(IPT, 2);
                        result = PolicyCal.BuildCResultObject(String.Format("{0:0.00}", IPT), "Net IPT", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString());
                        oXiResults.Add(result);
                        total += IPT;
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
                    oXiResults.Add(IPTLoadFactor);
                    var sQuoteGUID = Guid.NewGuid().ToString("N").Substring(0, 10);
                    var iBatchID = iCustomerID.ToString() + iInsatnceID.ToString();
                    oBII.Attributes["sRegNo"] = new XIIAttribute { sName = "sRegNo", sValue = ostructureInstance.XIIValue("sRegNo").sValue, bDirty = true };
                    oBII.Attributes["dCoverStart"] = new XIIAttribute { sName = "dCoverStart", sValue = dtInsuranceCoverStartDate, bDirty = true };
                    oBII.Attributes["sCaravanMake"] = new XIIAttribute { sName = "sCaravanMake", sValue = ostructureInstance.XIIValue("sCaravanMakeUpdated").sDerivedValue, bDirty = true };
                    oBII.Attributes["sCaravanModel"] = new XIIAttribute { sName = "sCaravanModel", sValue = ostructureInstance.XIIValue("sModelofCaravanUpdated").sValue, bDirty = true };

                    oBII.Attributes["FKiQSInstanceID"] = new XIIAttribute { sName = "FKiQSInstanceID", sValue = iInsatnceID.ToString(), bDirty = true };
                    oBII.Attributes["sInsurer"] = new XIIAttribute { sName = "sInsurer", sValue = "Zenith Marque", bDirty = true };
                    oBII.Attributes["FKiCustomerID"] = new XIIAttribute { sName = "FKiCustomerID", sValue = iCustomerID.ToString(), bDirty = true };
                    oBII.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                    oBII.Attributes["dtqsupdateddate"] = new XIIAttribute { sName = "dtqsupdateddate", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBII.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBII.Attributes["FKiProductVersionID"] = new XIIAttribute { sName = "FKiProductVersionID", sValue = sVersion, bDirty = true };
                    oBII.Attributes["BatchID"] = new XIIAttribute { sName = "BatchID", sValue = iBatchID, bDirty = true };
                    oBII.Attributes["sGUID"] = new XIIAttribute { sName = "sGUID", sValue = sQuoteGUID, bDirty = true };
                    //oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = String.Format("{0:0.00}", rCompulsaryExcess), bDirty = true };
                    //oBII.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = String.Format("{0:0.00}", rVoluntaryExcess), bDirty = true };
                    //oBII.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = String.Format("{0:0.00}", rTotalExcess), bDirty = true };
                    oBII.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = iQuoteID.ToString(), bDirty = true };
                    var oRes = oBII.Save(oBII);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        oBII = (XIIBO)oRes.oResult;
                    }
                    Info.Add("Zenith Marque Quote inserted Sucessfully with the amount of " + total);
                    XIDBO oRiskFactorsBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
                    XIIBO oBO = new XIIBO();
                    //oBO.BOD = oRiskFactorsBOD;
                    List<XIIBO> oBOIList = new List<XIIBO>();
                    //oBO.Attributes["FKiQuoteID"] = new XIIAttribute { sName = "FKiQuoteID", sValue = oBII.Attributes["ID"].sValue, bDirty = true };
                    //oBO.Attributes["sFactorName"] = new XIIAttribute { sName = "sFactorName", sValue = "Base Rate", bDirty = true };
                    //oBO.Attributes["sValue"] = new XIIAttribute { sName = "sValue", sValue = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    //oBO.Attributes["sMessage"] = new XIIAttribute { sName = "sMessage", sValue = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    //oBO.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    //oBO.Attributes["FKsQuoteID"] = new XIIAttribute { sName = "FKsQuoteID", sValue = sQuoteGUID, bDirty = true };
                    //oBO.Attributes["ID"] = new XIIAttribute { sName = "ID", bDirty = true };
                    //oBOIList.Add(oBO);
                    oBO = new XIIBO();
                    oBO.BOD = oRiskFactorsBOD;
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
        }
    }
}