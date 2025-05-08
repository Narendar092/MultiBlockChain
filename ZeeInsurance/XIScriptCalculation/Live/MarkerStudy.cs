using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using XICore;
using XISystem;

namespace ZeeInsurance.XIScriptCalculation.Live
{
    public class MarkerStudy
    {
        public static CResult PolicyMainCal(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            PolicyCalculation Pcal = new PolicyCalculation();
            //XIIQS oQsInstance = new XIIQS();
            CResult oResult = new CResult();
            oResult.sMessage = "MARKERSTUDY script running";
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
            public CResult GetBaseRate()
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
                    oNV1.sValue = "+219";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Base Rate";
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

            public CResult GetCoverLoadFactor(string TypeOfCover)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Cover";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (TypeOfCover.ToLower() == "comp")
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
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
            public CResult GetArea(string PostCode, int iProductID)
            {
                CResult oCResult = new CResult();
                XIInstanceBase oIB = new XIInstanceBase();
                XIAPI oXIAPI = new XIAPI();
                try
                {
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
                    PostCode = PostCode.Replace(" ", "").ToUpper();
                    PostCode = PostCode.Remove(PostCode.Length - 3);
                    List<XIWhereParams> oWHParams = new List<XIWhereParams>();
                    oWHParams.Add(new XIWhereParams { sField = "sPostCode", sOperator = "=", sValue = PostCode });
                    oWHParams.Add(new XIWhereParams { sField = "FkiProductID", sOperator = "=", sValue = iProductID.ToString() });
                    var oArea = oXIAPI.GetValue("AreaLookUp_T", "Area", oWHParams);
                    //var oArea = oXIAPI.GetValue("AreaLookUp_T", PostCode, "Area", "sPostCode");
                    if (!string.IsNullOrEmpty(oArea))
                    {
                        switch (oArea.ToLower())
                        {
                            case "area 1":
                                oNV1.sValue = "-22";
                                break;
                            case "area 2":
                                oNV1.sValue = "-12";
                                break;
                            case "area 3":
                                oNV1.sValue = "-0";
                                break;
                            case "area 4":
                                oNV1.sValue = "+20";
                                break;
                            case "area 5":
                                oNV1.sValue = "+50";
                                break;
                            case "area 6":
                                oNV1.sValue = "+75";
                                break;
                        }
                    }
                    else
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.oCollectionResult.Add(oNV3);
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
            public CResult GetDistrict(string PostCode, string sProductName, string sProductCode, string sVersion, string sCoreDataBase)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                XIAPI oXIAPI = new XIAPI();
                try
                {
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
                    //PostCode = PostCode.Replace(" ", "").ToUpper();
                    //PostCode = PostCode.Remove(PostCode.Length - 2);
                    //PostCode = PostCode.Insert(PostCode.Length - 1, " ");
                    //var oArea = oXIAPI.GetValue("PostCodeLookUps_T", PostCode, "[Group]", "sMPostCode");
                    XIPostCodeLookUp oPostCodeLookUp = new XIPostCodeLookUp();
                    oPostCodeLookUp.sMPostCode = PostCode;
                    oPostCodeLookUp.sProductCode = sProductCode;
                    oPostCodeLookUp.sProductName = sProductName;
                    oPostCodeLookUp.sVersion = sVersion;
                    oCResult.sMessage = "PostCode_" + PostCode + "_sProductCode_" + sProductCode + "_sProductName" + sProductName;
                    oIB.SaveErrortoDB(oCResult);
                    var oArea = oPostCodeLookUp.Get_PostCode();
                    oCResult.sMessage = "oDistrictLookUpResult_" + oArea.xiStatus + "_" + oArea.oResult;
                    oIB.SaveErrortoDB(oCResult);
                    if (oArea.xiStatus == 0 && oArea.oResult != null)
                    {
                        var Area = oArea.oResult.ToString();
                        oCResult.sMessage = "oDistrict" + Area;
                        oIB.SaveErrortoDB(oCResult);
                        if (!string.IsNullOrEmpty(Area))
                        {
                            switch (Area.ToLower())
                            {
                                case "district 6":
                                    oNV1.sValue = "+5";
                                    break;
                                case "district 7":
                                    oNV1.sValue = "+7";
                                    break;
                                case "district 8":
                                    oNV1.sValue = "+10";
                                    break;
                                case "district 9":
                                    oNV1.sValue = "+150";
                                    break;
                                case "district 10":
                                    oNV1.sValue = "+0";
                                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                    break;
                                case "district 100":
                                case "district 101":
                                    oNV1.sValue = "+8";
                                    break;
                                default:
                                    oNV1.sValue = "-0";
                                    break;
                            }
                        }
                        else
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                    }
                    else
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    oCResult.oCollectionResult.Add(oNV);
                    oCResult.oCollectionResult.Add(oNV1);
                    oCResult.oCollectionResult.Add(oNV2);
                    oCResult.oCollectionResult.Add(oNV3);
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
            public CResult GetVehicleMakeLoadFactor(string TypeOfMake)
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
                    oNV2.sValue = "Vehicle Make";
                    TypeOfMake = TypeOfMake.ToLower();
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (TypeOfMake == "ford")
                    {
                        oNV1.sValue = "+25";
                    }
                    else if (TypeOfMake == "volkswagen" || TypeOfMake == "fiat" || TypeOfMake == "peugeot")
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
                    //oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetVechileValue(double iCurrentCaravanValue)
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
                    double f1 = 0, f2 = 0, f3 = 0;
                    double ExcessValue;
                    if (iCurrentCaravanValue >= 1500 && iCurrentCaravanValue <= 50000)
                    {
                        if (iCurrentCaravanValue <= 25000)
                        {
                            f1 = iCurrentCaravanValue * 0.005;
                        }
                        else if (iCurrentCaravanValue > 25000 && iCurrentCaravanValue <= 40000)
                        {
                            f1 = 25000 * 0.005;
                            f2 = (iCurrentCaravanValue - 25000) * 0.005;
                        }
                        else if (iCurrentCaravanValue > 40000)
                        {
                            f1 = 25000 * 0.005;
                            f2 = (40000 - 25000) * 0.005;
                            f3 = (iCurrentCaravanValue - 40000) * 0.008;
                        }
                    }
                    else
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    ExcessValue = f1 + f2 + f3;
                    oNV1.sValue = "+" + ExcessValue.ToString();
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
            public CResult GetAgeLoadFactor(int Age)
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
                    oNV2.sValue = "Age";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    switch (Age)
                    {
                        case 25:
                            oNV1.sValue = "+22";
                            break;
                        case 26:
                            oNV1.sValue = "+21";
                            break;
                        case 27:
                            oNV1.sValue = "+20";
                            break;
                        case 28:
                            oNV1.sValue = "+19";
                            break;
                        case 29:
                            oNV1.sValue = "+18";
                            break;
                        case 30:
                            oNV1.sValue = "+14";
                            break;
                        case 31:
                            oNV1.sValue = "+13";
                            break;
                        case 32:
                            oNV1.sValue = "+12";
                            break;
                        case 33:
                            oNV1.sValue = "+10";
                            break;
                        case 34:
                            oNV1.sValue = "+8";
                            break;
                        case 35:
                            oNV1.sValue = "+3";
                            break;
                        case 36:
                            oNV1.sValue = "+2";
                            break;
                        case 37:
                            oNV1.sValue = "+1";
                            break;
                        case 38:
                            oNV1.sValue = "-0";
                            break;
                        case 39:
                            oNV1.sValue = "-2";
                            break;
                        case 40:
                            oNV1.sValue = "-6";
                            break;
                        case 41:
                            oNV1.sValue = "-7";
                            break;
                        case 42:
                        case 43:
                        case 44:
                        case 45:
                        case 46:
                            oNV1.sValue = "-8";
                            break;
                        case 47:
                            oNV1.sValue = "-10";
                            break;
                        case 48:
                            oNV1.sValue = "-12";
                            break;
                        case 49:
                            oNV1.sValue = "-13";
                            break;
                        case 50:
                        case 51:
                            oNV1.sValue = "-20";
                            break;
                        case 52:
                        case 53:
                            oNV1.sValue = "-21";
                            break;
                        case 54:
                        case 55:
                        case 56:
                            oNV1.sValue = "-20";
                            break;
                        case 57:
                        case 58:
                        case 59:
                            oNV1.sValue = "-19";
                            break;
                        case 60:
                            oNV1.sValue = "-18";
                            break;
                        case 61:
                            oNV1.sValue = "-17";
                            break;
                        case 62:
                            oNV1.sValue = "-16";
                            break;
                        case 63:
                            oNV1.sValue = "-15";
                            break;
                        case 64:
                            oNV1.sValue = "-14";
                            break;
                        case 65:
                            oNV1.sValue = "-13";
                            break;
                        case 66:
                        case 67:
                        case 68:
                        case 69:
                            oNV1.sValue = "-8";
                            break;
                        case 70:
                            oNV1.sValue = "+5";
                            break;
                        case 71:
                        case 72:
                        case 73:
                            oNV1.sValue = "+10";
                            break;
                        default:
                            if (Age < 25 || Age > 73)
                            {
                                oNV1.sValue = "+0";
                                oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                            }
                            else
                            {
                                oNV1.sValue = "+0";
                                oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                            }
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
                    //oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetAgeOfVechicleDiscount(int YearofManufacture)
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
                    oNV2.sValue = "AgeOfVechicle";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    var CurrentDate = DateTime.Now.Year;
                    var YearDiff = CurrentDate - YearofManufacture;
                    if (YearDiff <= 1 && YearDiff < 2)
                    {
                        oNV1.sValue = "+25";
                    }
                    else if (YearDiff >= 2 && YearDiff < 3)
                    {
                        oNV1.sValue = "+20";
                    }
                    else if (YearDiff >= 3 && YearDiff < 4)
                    {
                        oNV1.sValue = "+15";
                    }
                    else if (YearDiff >= 4 && YearDiff < 5)
                    {
                        oNV1.sValue = "+12";
                    }
                    else if (YearDiff >= 5 && YearDiff < 6)
                    {
                        oNV1.sValue = "+10";
                    }
                    else if (YearDiff >= 6 && YearDiff < 7)
                    {
                        oNV1.sValue = "+8";
                    }
                    else if (YearDiff >= 7 && YearDiff < 8)
                    {
                        oNV1.sValue = "+6";
                    }
                    else if (YearDiff >= 8 && YearDiff < 9)
                    {
                        oNV1.sValue = "+3";
                    }
                    else if (YearDiff >= 9 && YearDiff < 10)
                    {
                        oNV1.sValue = "+0";
                    }
                    else if (YearDiff >= 10 && YearDiff <= 11)
                    {
                        oNV1.sValue = "-5";
                    }
                    else if (YearDiff >= 12 && YearDiff <= 15)
                    {
                        oNV1.sValue = "-10";
                    }
                    else if (YearDiff >= 16 && YearDiff <= 20)
                    {
                        oNV1.sValue = "-15";
                    }
                    else if (YearDiff >= 21 && YearDiff <= 26)
                    {
                        oNV1.sValue = "-20";
                    }
                    else if (YearDiff >= 27 && YearDiff <= 29)
                    {
                        oNV1.sValue = "-25";
                    }
                    else if (YearDiff >= 30)
                    {
                        oNV1.sValue = "-30";
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
            public CResult GetOwnershipDiscount(int YearOfOwnerShip)
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
                    oNV2.sValue = "OwnershipDiscount";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (YearOfOwnerShip <= 11)
                    {
                        oNV1.sValue = "+10";
                    }
                    else if (YearOfOwnerShip > 11 && YearOfOwnerShip <= 23)
                    {
                        oNV1.sValue = "-10";
                    }
                    else if (YearOfOwnerShip >= 24)
                    {
                        oNV1.sValue = "-25";
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
                    oNV2.sValue = "ClubMember";
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

            public CResult GetMileageLoadFactor(int miles)
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
                    oNV2.sValue = "Mileage";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (miles >= 0 && miles <= 3000)
                    {
                        oNV1.sValue = "+0";
                    }
                    else if (miles >= 3001 && miles <= 4000)
                    {
                        oNV1.sValue = "+5";
                    }
                    else if (miles >= 4001 && miles <= 5000)
                    {
                        oNV1.sValue = "+10";
                    }
                    else if (miles >= 5001 && miles <= 7000)
                    {
                        oNV1.sValue = "+25";
                    }
                    else if (miles >= 7001 && miles <= 9000)
                    {
                        oNV1.sValue = "+50";
                    }
                    else
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetEngineSizeLoadFactor(int EngineSize)
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
                    oNV2.sValue = "EngineSize";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (EngineSize <= 1600)
                    {
                        oNV1.sValue = "+0";
                    }
                    else if (EngineSize >= 1601 && EngineSize <= 2000)
                    {
                        oNV1.sValue = "+5";
                    }
                    else if (EngineSize >= 2001 && EngineSize <= 2300)
                    {
                        oNV1.sValue = "+10";
                    }
                    else if (EngineSize >= 2301 && EngineSize <= 2500)
                    {
                        oNV1.sValue = "+20";
                    }
                    else if (EngineSize >= 2501 && EngineSize <= 3000)
                    {
                        oNV1.sValue = "+25";
                    }
                    else if (EngineSize >= 3001 && EngineSize <= 4000)
                    {
                        oNV1.sValue = "+50";
                    }
                    else
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
                    oNV2.sValue = "Driving Restrictions";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    int icount = type.Count;
                    oresult.sMessage = "TypeOfDrivingCount:" + icount;
                    oIB.SaveErrortoDB(oresult);
                    if (icount == 1)
                    {
                        oNV1.sValue = "-17.5";
                    }
                    else if (icount == 2)
                    {
                        if (type.Contains("spouse"))
                        {
                            oNV1.sValue = "-20";
                        }
                        else
                        {
                            oNV1.sValue = "-15";
                        }
                    }
                    else if (icount == 3)
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
            public CResult GetLeftHandDriveLoadFactor(string IsLeftHandDrive, int iYearOfOwnership)
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
                    oNV2.sValue = "LeftHandDrive";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (IsLeftHandDrive.ToLower() == "10" && iYearOfOwnership >= 12)
                    {
                        oNV1.sValue = "+25";
                    }
                    else if (iYearOfOwnership < 12 && IsLeftHandDrive.ToLower() == "10")
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
            public CResult GetClaimLoadFactor(int NoOfClaims, List<XIIBO> ClaimI, string CoverStartDate, int iNoOfClaims)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    CNV oNV = new CNV();
                    oNV.sName = "NoOfClaims";
                    int demeritPoints = 0;
                    int iCountOfClaimsWithinLast3Yrs = 0;
                    foreach (var Claim in ClaimI)
                    {
                        var sClaimName = Claim.AttributeI("sName").ResolveFK("display");
                        var sFault = Claim.AttributeI("iWhoseFault").sResolvedValue;
                        var claimCost = Claim.AttributeI("rTotalClaimCost").sValue;
                        //var ConvictionDate = Claim.AttributeI("dDate").sValue;
                        //int ClaimYear = 0;
                        //var result = GetAgeFromDOB(Claim.AttributeI("dDate").sValue, CoverStartDate);
                        //if (result.xiStatus == 00 && result.oResult != null)
                        //{
                        //    ClaimYear = (int)result.oResult;
                        //}
                        var dt3Years = DateTime.Now.AddYears(-3).Date;
                        var dtClaimDate = Convert.ToDateTime(Claim.AttributeI("dDate").sValue);
                        double rCost = 0;
                        if (double.TryParse(claimCost, out rCost))
                        { }
                        int dPoints = 0;
                        oresult.sMessage = "sClaimName" + sClaimName;
                        oIB.SaveErrortoDB(oresult);
                        if (dtClaimDate >= dt3Years)
                        {
                            iCountOfClaimsWithinLast3Yrs++;
                            sClaimName = sClaimName.ToLower();
                            if (sClaimName == "vandalism" || sClaimName == "malicious damage (theft)" || sClaimName == "theft" || sClaimName == "fire" || sClaimName == "riot")
                            {
                                dPoints = 20;
                            }
                            else if (sFault.ToLower() == "fire + theft" && (sClaimName == "accident" || sClaimName == "flood" || sClaimName == "loss of keys" || sClaimName == "other" || sClaimName == "storm" || sClaimName == "riot" || sClaimName == "windscreen") && rCost <= 10000)
                            {
                                dPoints = 20;
                            }
                            else if (rCost > 10000)
                            {
                                dPoints = 99;
                            }
                            else if (sFault.ToLower() == "non fault")
                            {
                                dPoints = 1;
                            }
                            //else if ((sFault.ToLower() == "fire + theft" || sFault.ToLower() == "policy holder fault") && rCost <= 2500)
                            //{
                            //    dPoints = 4;
                            //}
                            else if ((sFault.ToLower() == "fire + theft" || sFault.ToLower() == "policy holder fault") && rCost <= 10000)
                            {
                                dPoints = 4;
                            }
                            //else if ((sFault.ToLower() == "fire + theft" || sFault.ToLower() == "policy holder fault") && rCost > 10000)
                            //{
                            //    dPoints = 99;
                            //}
                        }
                        else
                        {
                            dPoints = 0;
                        }
                        if (iCountOfClaimsWithinLast3Yrs >= 2)
                        {
                            dPoints = 20;
                        }
                        if ((iNoOfClaims + iCountOfClaimsWithinLast3Yrs) >= 2)
                        {
                            dPoints = 20;
                        }
                        demeritPoints += dPoints;
                    }
                    iNoOfClaims += iCountOfClaimsWithinLast3Yrs;
                    oNV.sValue = iNoOfClaims.ToString();
                    oresult.oCollectionResult.Add(oNV);
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
                    string[] DeclineCodes = new string[] { "DD", "DR", "DG", "E", "MR", "MT", "N", "NA", "NE", "NEND", "NI", "NR", "QI", "S", "SC", "TT", "UT", "XX", "Z" };
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
                        //if (DateTime.TryParse(Conviction.AttributeI("dDate").sValue, out ConvictionDate))
                        //{

                        //   DateTime BanDateUpto = ConvictionDate.AddMonths(iBan);
                        // BanDays = (BanDateUpto - ConvictionDate).TotalDays;
                        //  oCResult.sMessage = "BanDays : " + BanDays;
                        //    oIB.SaveErrortoDB(oCResult);
                        // }
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
                        if (iBan > 0 && iBan <= 6 && dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte)
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
                            sDemeritpoints = "0";
                        }
                        if (iBan > 6 && dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte)
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
                            sDemeritpoints = "0";
                        }
                        else if ((Array.IndexOf(DeclineCodes, Prefix) > -1 || ConvictionCode.ToUpper() == "CU70") && dDAte >= sPolicyStartDAte.AddYears(-12) && dDAte <= sPolicyStartDAte)
                        {
                            sDemeritpoints = "20";
                        }
                        else if (Prefix == "CD" && numberPart >= 40 && numberPart <= 96 && (dDAte >= sPolicyStartDAte.AddYears(-12) && dDAte <= sPolicyStartDAte))
                        {
                            sDemeritpoints = "20";
                        }
                        else if (convictionYear <= 5)
                        {
                            //var ConvictionCode = oConvictionI.Attributes["scode"].sValue;
                            var sKey = "";
                            if (!string.IsNullOrEmpty(ConvictionCode))
                            {
                                if (convictionYear >= 0 && convictionYear < 2)
                                {
                                    sKey = ConvictionCode + " 0-2";
                                }
                                if (convictionYear >= 2 && convictionYear < 4)
                                {
                                    sKey = ConvictionCode + " 2-4";
                                }
                                if (convictionYear >= 4 && convictionYear <= 5)
                                {
                                    sKey = ConvictionCode + " 4-5";
                                }
                            }
                            oCResult.sMessage = "sConvictionKey_" + sKey;
                            oIB.SaveErrortoDB(oCResult);
                            XIAPI oXIAPI = new XIAPI();
                            List<XIWhereParams> oWhereParams = new List<XIWhereParams>();
                            oWhereParams.Add(new XIWhereParams { sField = "sName", sValue = sKey, sOperator = "=" });
                            oWhereParams.Add(new XIWhereParams { sField = "FKiProductID", sValue = iProductID.ToString(), sOperator = "=" });
                            //sDemeritpoints = oXIAPI.GetValue("refConvictionsDemerit_T", sKey, "sValue", "sName");
                            sDemeritpoints = oXIAPI.GetValue("refConvictionsDemerit_T", "sValue", oWhereParams);
                        }
                        else
                        {
                            sDemeritpoints = "0";
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
            public CResult GetVechileUseLoadFactor(string sUse)
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
                    oNV2.sValue = "Usage";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (sUse == "SDPC")
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
            public CResult GetOccupationsLoadFactor(string sOccupation)
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
                    oNV2.sValue = "Occupation/Secondary Occupation";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";

                    sOccupation = sOccupation.ToLower();
                    //string[] ReferArray = new string[] { "artist", "unemployed", "licensed trade", "licensee", "publican", "employee", "newspaper journalists", "photographers", "landscape gardener", "unemployed", "roofer", "scaffolder", "road worker", "ashphalter", };
                    //string[] DeclineArray = new string[] { "Actors/Actresses", "Amusement Caterers", "Gaming/Casino", "Demolition Contractors", "Enquiry Agents", "Entertainment Industry", "Fairground workers", "Food Delivery", "Music Industry", "General/Scrap Dealers", "Market/Street Traders", "Gas/Oil Rig workers", "Personalities", "Professional Sportsperson", "Unemployed", "Disc Jockey", "Club Proprietors/employees", "Student", "Journalists", "Diplomats/Embassy Staff ", "Security", "House Parent", "Teacher", "Courier", };
                    string[] DeclineArray = new string[] { "actor or actress", "actor/actress", "actress", "amusement arcade worker", "assistant teacher", "broadcaster - tv/radio", "classical musician", "classical musician", "courier", "courier - driver", "courier - motorcycle", "courier - parcel delivery", "croupier", "dance teacher", "dealer", "dealer - general", "dealer - scrap/waste", "delivery courier", "demolition contractor", "deputy head teacher", "despatch driver", "despatch rider", "despatch worker", "diplomat", "diplomatic staff - british", "diplomatic staff - foreign", "diplomatic staff - republic of ireland", "disc jockey", "disco staff", "driver - hot food delivery", "embassy staff", "embassy staff - british", "embassy staff - foreign", "embassy staff - republic of ireland", "enquiry agent", "extra", "fairground worker", "fast food delivery driver", "funfair employee", "gambler", "gaming board inspector", "gaming club manager", "gaming club proprietor", "gaming club staff - licensed premises", "gaming club staff - unlicensed premises", "headteacher", "house parent", "househusband", "housekeeper", "houseman or woman", "housewife or househusband", "journalist", "journalist - freelance", "journalistic agent", "market trader", "mature student", "mature student - living at home", "mature student - living away", "medical student", "medical student - living at home", "medical student - living away", "mobile disc jockey", "mobile disco owner", "music producer", "music teacher", "music therapist", "music wholesaler", "musical arranger", "musician", "musician - amateur", "musician - classical", "musician - dance band", "musician - pop group", "night club staff", "not in employment", "oil rig crew", "piano teacher", "post graduate student living at home", "post graduate student living away from home", "premises security installers", "professional boxer", "professional cricketer", "professional cyclist", "professional footballer", "professional racing driver", "professional racing motorcyclist", "professional sports coach", "professional sportsperson", "professional wrestler", "racing motorcyclist", "rally driver", "rig worker", "rig worker - off shore", "rugby player", "rugby player - amateur", "rugby player - professional", "school student", "security consultant", "security controller", "security guard", "security officer", "semi-professional sportsperson", "sportsman or woman", "sportswoman", "street trader", "student", "student - foreign", "student - living at home", "student - living away", "student counseller", "student nurse", "student nurse - living at home", "student nurse - living away", "student teacher", "student teacher - living at home", "student teacher - living away", "supply teacher", "teacher", "teachers assistant", "television presenter", "travel courier", "tv announcer", "undergraduate student - living at home", "undergraduate student - living away from home", "unemployed", "yoga teacher", "scrap dealer" };
                    int pos = Array.IndexOf(DeclineArray, sOccupation);
                    if (pos > -1)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    //else
                    //{
                    //    pos = Array.IndexOf(ReferArray, sOccupation);
                    //    if (pos > -1)
                    //    {
                    //        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
                    //    }
                    //}
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

            public CResult GetGaragingLoadFactor(string sWheredoyoustore, double rCaravanValue, string sPostCode, string sLocationOfMotorhomeOvernight, int iProductID)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    XIAPI oXIAPI = new XIAPI();
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
                    sWheredoyoustore = sWheredoyoustore.ToLower();
                    if (!string.IsNullOrEmpty(sLocationOfMotorhomeOvernight) && (sLocationOfMotorhomeOvernight.ToLower() == "more than 100 metres of home address" || sLocationOfMotorhomeOvernight.ToLower() == "within 100 metres of home address"))
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    else
                    {
                        sPostCode = sPostCode.Replace(" ", "").ToUpper();
                        sPostCode = sPostCode.Remove(sPostCode.Length - 3);
                        List<XIWhereParams> oWhereParams = new List<XIWhereParams>();
                        oWhereParams.Add(new XIWhereParams { sField = "sPostCode", sValue = sPostCode, sOperator = "=" });
                        oWhereParams.Add(new XIWhereParams { sField = "FKiProductID", sValue = iProductID.ToString(), sOperator = "=" });
                        //var oArea = oXIAPI.GetValue("AreaLookUp_T", sPostCode, "Area", "sPostCode");
                        var oArea = oXIAPI.GetValue("AreaLookUp_T", "Area", oWhereParams);
                        oArea = oArea.ToLower();
                        if ((sWheredoyoustore == "kept on public road" || sWheredoyoustore == "car park") && (oArea == "area 5" || oArea == "area 6"))
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                        else if ((sWheredoyoustore == "kept on public road" && rCaravanValue >= 15000) || (sWheredoyoustore == "car park" && rCaravanValue >= 7500))
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                        else if (sWheredoyoustore == "kept on public road" && rCaravanValue < 15000)
                        {
                            oNV1.sValue = "+10";
                        }
                        else if (sWheredoyoustore == "car park" && rCaravanValue < 7500)
                        {
                            oNV1.sValue = "+15";
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
                    //oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetVoluntaryExcessLoadFactor(int VoluntaryExcess)
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
                    switch (VoluntaryExcess)
                    {
                        case 100:
                            oNV1.sValue = "-1";
                            break;
                        case 150:
                            oNV1.sValue = "-2";
                            break;
                        case 200:
                            oNV1.sValue = "-3";
                            break;
                        case 250:
                            oNV1.sValue = "-4";
                            break;
                        case 300:
                            oNV1.sValue = "-5";
                            break;
                        default:
                            oNV1.sValue = "+0";
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
                    //oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }

            public CResult GetMinimumPremium(int YearofManufacture)
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
                    var CurrentDate = DateTime.Now.Year;
                    var YearDiff = CurrentDate - YearofManufacture;
                    if (YearDiff <= 30)
                    {
                        oNV1.sValue = "+150";
                    }
                    else if (YearDiff > 30)
                    {
                        oNV1.sValue = "+125";
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
                    switch (idemeritPoins)
                    {
                        case 0:
                        case 1:
                            oNV1.sValue = "+0";
                            break;
                        case 2:
                            oNV1.sValue = "+5";
                            break;
                        case 3:
                            oNV1.sValue = "+10";
                            break;
                        case 4:
                            oNV1.sValue = "+15";
                            break;
                        case 5:
                            oNV1.sValue = "+25";
                            break;
                        case 6:
                            oNV1.sValue = "+35";
                            break;
                        case 7:
                            oNV1.sValue = "+50";
                            break;
                        case 8:
                            oNV1.sValue = "+70";
                            break;
                        case 9:
                            oNV1.sValue = "+80";
                            break;
                        case 10:
                            oNV1.sValue = "+110";
                            break;
                        case 11:
                            oNV1.sValue = "+125";
                            break;
                        case 12:
                            oNV1.sValue = "+150";
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

            public CResult CompulsaryExcess(double iCurrentCaravanValue)
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
                    oNV2.sValue = "Compulsary Excess";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Value";

                    if (iCurrentCaravanValue <= 10000)
                    {
                        oNV1.sValue = "+100";
                    }
                    else if (iCurrentCaravanValue <= 20000)
                    {
                        oNV1.sValue = "+150";
                    }
                    else if (iCurrentCaravanValue <= 30000)
                    {
                        oNV1.sValue = "+200";
                    }
                    else if (iCurrentCaravanValue <= 40000)
                    {
                        oNV1.sValue = "+250";
                    }
                    else if (iCurrentCaravanValue <= 50000)
                    {
                        oNV1.sValue = "+400";
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

            public CResult Modifications(string sModification)
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
                    oNV2.sValue = "Modifications";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";
                    if (!string.IsNullOrEmpty(sModification) && (sModification.ToLower() == "on" || sModification.ToLower() == "true"))
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
            public CResult GetProtectedNoClaimsDiscount(string IsProtectedNoClaimBonus, int iNoClaimYears, int iPrivateNoclaims)
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
                    oNV2.sValue = "ProtectedNoClaimsDiscount";
                    CNV oNV3 = new CNV();
                    oNV3.sName = "Type";
                    oNV3.sValue = "Percent";

                    if (!string.IsNullOrEmpty(IsProtectedNoClaimBonus) && IsProtectedNoClaimBonus.ToLower() == "yes")
                    {
                        if (iNoClaimYears == 1 && iPrivateNoclaims == 1)
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                        else
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
                    List<CResult> AgesResult = new List<CResult>();
                    List<CResult> OccupationResult = new List<CResult>();
                    List<string> oRelation = new List<string>();
                    string DOB = "";
                    string sOccupation = "";
                    string sSecondaryOccupation = "";
                    XIInfraCache oCache = new XIInfraCache();
                    oBII.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations", null);
                    var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
                    int iProductID = 0;
                    string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                    if (int.TryParse(ProductID, out iProductID)) { }
                    var oProductI = oIXI.BOI("Product", ProductID);
                    CResult ClaimResult = new CResult();
                    //List<CResult> DrivingRestrictions = new List<CResult>();
                    int NoOfClaims = 0;
                    int NoOfConvinctions = 0;
                    var result = new CResult();
                    CResult ConvictionResult = new CResult();
                    List<CResult> ClaimConvictionResultList = new List<CResult>();
                    CResult ClaimConvictionResult = new CResult();
                    //var oQSI = oIXI.BOI("QS Instance", iInsatnceID.ToString()).Structure("NotationStructure").XILoad();
                    var oQSI = oCache.Get_QsStructureObj(sSessionID, sUID, "QSInstance_" + iInsatnceID + "NotationStructure");
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    var oLIst = oQSI.oSubStructureI("Driver_T");
                    var oDriversI = (List<XIIBO>)oLIst.oBOIList;
                    string dtInsuranceCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;
                    int j = 0;
                    int iDemeritPoints = 0;
                    int iNoOfClaims = 0;
                    foreach (var item in oDriversI)
                    {
                        int Age = 0;
                        //oResult.sMessage = "DriverID_" + item.Attributes["id"].sValue + "_QSInstanceID_" + iInsatnceID;
                        //oIB.SaveErrortoDB(oResult);
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
                        //if (!ostructureInstance.XIIValues.ContainsKey("vehiclesareavailableinthehousehold"))
                        //{
                        var sOtherVehicles = item.AttributeI("sothervehiclesowned").sValue;
                        int iOtherVehicles = 0;
                        int.TryParse(sOtherVehicles, out iOtherVehicles);
                        if (iOtherVehicles == 0)
                        {
                            oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Other vehicles are 0", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                        }
                        //}
                        result = PolicyCal.GetAgeFromDOB(DOB, DateTime.Now.ToString());
                        if (result.xiStatus == 00 && result.oResult != null)
                        {
                            Age = (int)result.oResult;
                            result = PolicyCal.GetAgeLoadFactor(Age);
                            if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                            {
                                AgesResult.Add(result);
                            }
                        }
                        sOccupation = item.AttributeI("enumOccupatation").sResolvedValue;
                        //oResult.sMessage = "sOccupation_" + sOccupation;
                        //oIB.SaveErrortoDB(oResult);
                        Info.Add("sOccupation_" + sOccupation);
                        result = PolicyCal.GetOccupationsLoadFactor(sOccupation);
                        if (result.xiStatus == 00 && result.oCollectionResult.Count > 0)
                        {
                            OccupationResult.Add(result);
                        }
                        sSecondaryOccupation = item.AttributeI("sSecondaryOccupation").sResolvedValue;
                        if (!string.IsNullOrEmpty(sSecondaryOccupation))
                        {
                            Info.Add("Secondary Occupation_" + sSecondaryOccupation);
                            result = PolicyCal.GetOccupationsLoadFactor(sSecondaryOccupation);
                            if (result.xiStatus == 00 && result.oCollectionResult.Count > 0)
                            {
                                OccupationResult.Add(result);
                            }
                        }

                        //string srelation = item.AttributeI("enumRelationship").sResolvedValue;
                        XIIBO oBOI1 = new XIIBO();
                        oBOI1.iBODID = item.iBODID;
                        item.Attributes["enumRelationship"].BOI = oBOI1;
                        string srelation = item.AttributeI("enumRelationship").ResolveFK("display1");
                        if (!string.IsNullOrEmpty(srelation))
                        {
                            oRelation.Add(srelation.ToLower());
                        }
                        var sSubList = item.oStructureI("Claim_T");
                        NoOfClaims = sSubList.Count();
                        if (NoOfClaims > 0)
                        {
                            result = PolicyCal.GetClaimLoadFactor(NoOfClaims, sSubList, dtInsuranceCoverStartDate, iNoOfClaims);
                            if (result.xiStatus == 0)
                            {
                                string Count = result.oCollectionResult.Where(x => x.sName == "NoOfClaims").Select(m => m.sValue).FirstOrDefault();
                                if (int.TryParse(Count, out iNoOfClaims))
                                { }
                                int ipoints = 0;
                                int.TryParse(result.oResult.ToString(), out ipoints);
                                iDemeritPoints = iDemeritPoints + ipoints;
                                //ClaimResult = result;
                            }
                        }
                        var ConvinctionsList = item.oStructureI("Conviction_T");
                        NoOfConvinctions = ConvinctionsList.Count();
                        if (NoOfConvinctions > 0)
                        {
                            ClaimConvictionResult = PolicyCal.GetConvictionsDemeritPoints(NoOfConvinctions, ConvinctionsList, dtInsuranceCoverStartDate, iProductID);
                            if (ClaimConvictionResult.xiStatus == 0)
                            {
                                int ipoints = 0;
                                int.TryParse(ClaimConvictionResult.oResult.ToString(), out ipoints);
                                iDemeritPoints = iDemeritPoints + ipoints;
                                //ConvictionResult = result;
                            }
                            ClaimConvictionResultList.Add(ClaimConvictionResult);
                        }

                        //ClaimConvictionResult = PolicyCal.DemeritPoints(iDemeritPoints, "Claim/Conviction Demerit Points LoadFactor");
                        //ClaimConvictionResultList.Add(ClaimConvictionResult);
                        var dPassed = item.AttributeI("dDateTestPassed").sResolvedValue;
                        //oResult.sMessage = "dDateTestPassed" + dPassed;
                        //oIB.SaveErrortoDB(oResult);
                        Info.Add("dDateTestPassed" + dPassed);
                        //var sDrivingQualifiication = item.AttributeI("sDrivingQualification").sResolvedValue;
                        var oDrivingQualifiication = oIXI.BOI("enumLicenceType_T", item.AttributeI("sDrivingQualification").sValue);
                        string sDrivingQualifiication = oDrivingQualifiication.AttributeI("sName").sValue;
                        oResult.sMessage = "sDrivingQualifiication" + sDrivingQualifiication;
                        oIB.SaveErrortoDB(oResult);
                        var ores = PolicyCal.GetAgeFromDOB(dPassed, DateTime.Now.ToString());
                        if (ores.xiStatus == 0 && ores.oResult != null)
                        {
                            int dpassedyr = (int)ores.oResult;
                            //oResult.sMessage = "dDateTestPassedYears_" + dpassedyr;
                            //oIB.SaveErrortoDB(oResult);
                            Info.Add("dDateTestPassedYears_" + dpassedyr);
                            if (dpassedyr < 4 && (sDrivingQualifiication == "EU" || sDrivingQualifiication == "Full UK"))
                            {
                                oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Driving licence held years", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                            }
                        }
                        if (sDrivingQualifiication.TrimEnd() == "EU" || sDrivingQualifiication.TrimEnd() == "Full UK")
                        {

                        }
                        else
                        {
                            oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Driving licence", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
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
                        if (dUkResyr < 5)
                        {
                            oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "UK Residency", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                        }
                        //}
                        j++;
                    }
                    ClaimConvictionResult = PolicyCal.DemeritPoints(iDemeritPoints, "Claim/Conviction Demerit Points LoadFactor", ClaimConvictionResultList);
                    string TypeOfCover = ostructureInstance.XIIValue("sTypeofCoverUpdated").sValue;
                    string sPostCode = ostructureInstance.XIIValue("sPostCode").sValue;
                    string sParkingPostCode = ostructureInstance.XIIValue("sParkingPostCode").sValue;
                    string sCaravanMake = ostructureInstance.XIIValue("sCaravanMakeUpdated").sResolvedValue;
                    string sCaravanValue = ostructureInstance.XIIValue("iCurrentCaravanValue").sValue;
                    string sYearOfManufacture = ostructureInstance.XIIValue("iYearOfManufactureUpdated").sValue;
                    //string sYearOfOwnership = ostructureInstance.XIIValue("iYearOfOwnershipUpdated").sValue;
                    string bIsClubMember = ostructureInstance.XIIValue("iIsClubMember").sValue;
                    string sMileage = ostructureInstance.XIIValue("iMileage").sValue;
                    string sEngineSize = ostructureInstance.XIIValue("sEngineSize").sValue;
                    string bISLeftHandDrive = ostructureInstance.XIIValue("bISLeftHandDrive").sValue;

                    string sVehicleOwner = ostructureInstance.XIIValue("sVehicleOwner").sDerivedValue;
                    string sVehicleKeeper = ostructureInstance.XIIValue("sVehicleKeeper").sDerivedValue;
                    oResult.sMessage = "sVehicleOwner_" + sVehicleOwner;
                    oIB.SaveErrortoDB(oResult);
                    oResult.sMessage = "sVehicleKeeper_" + sVehicleKeeper;
                    oIB.SaveErrortoDB(oResult);

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

                    string sUseOfVechile = ostructureInstance.XIIValue("sTypeOfDrivingupdated").sValue;
                    string sGaraging = ostructureInstance.XIIValue("sWheredoyoustoreUpdated").sDerivedValue;
                    string sLocationOfMotorhomeOvernight = ostructureInstance.XIIValue("iWhatisthelocationofyourmotorhomeovernight").sResolvedValue;
                    string sVoluntaryExcess = ostructureInstance.XIIValue("VoluntaryExcess").sValue;
                    //string svehicleModifications = ostructureInstance.XIIValue("sVehicleModificationsDescription").sResolvedValue;
                    string svehicleModifications = ostructureInstance.XIIValue("Other").sValue;
                    string sLiketoProtectYourNoClaimsDiscountUpdated = ostructureInstance.XIIValue("sLiketoProtectYourNoClaimsDiscountUpdated").sResolvedValue;
                    string sNoClaimsYears = ostructureInstance.XIIValue("iNoClaimsYears").sValue;
                    string sPrivatenocclaim = ostructureInstance.XIIValue("Privatenocclaim").sValue;
                    //string sDateofpurchase = ostructureInstance.XIIValue("dDateofpurchase").sValue;
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
                    // Fields For General Declines
                    string sNoSeats = ostructureInstance.XIIValue("iNoofSeats").sValue;
                    string sVehicleSecurity = ostructureInstance.XIIValue("iVehicleSecurity").sResolvedValue;
                    string sTypeofdevice = ostructureInstance.XIIValue("iTypeofdevice").sResolvedValue;
                    string sInsurancePolicyDetails = ostructureInstance.XIIValue("sPreviousMotorOrCaravanInsurancePolicyDetails").sResolvedValue;
                    string sMotorOffence = ostructureInstance.XIIValue("sMotorOffence").sResolvedValue;
                    string sTypeOfCaravan = ostructureInstance.XIIValue("sTypeOfCaravanUpdated").sResolvedValue;
                    //string sHowManyOtherVehicles = string.Empty;
                    //if (ostructureInstance.XIIValues.ContainsKey("vehiclesareavailableinthehousehold"))
                    //{
                    //    sHowManyOtherVehicles = ostructureInstance.XIIValue("vehiclesareavailableinthehousehold").sResolvedValue;
                    //}
                    //BaseRate
                    var oBaseLoadFactor = new CResult();
                    result = PolicyCal.GetBaseRate();
                    if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                    {
                        oBaseLoadFactor = result;
                    }
                    //Cover
                    var oCover = new CResult();
                    result = PolicyCal.GetCoverLoadFactor(TypeOfCover);
                    if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                    {
                        oCover = result;
                        oXiResults.Add(oCover);
                    }
                    //Area
                    List<CResult> oAreaList = new List<CResult>();
                    var oArea = PolicyCal.GetArea(sPostCode, iProductID);
                    if (oArea.xiStatus == 0 && oArea.oCollectionResult.Count > 0)
                    {
                        oAreaList.Add(oArea);
                    }
                    var oParkingArea = PolicyCal.GetArea(sParkingPostCode, iProductID);
                    if (oParkingArea.xiStatus == 0 && oParkingArea.oCollectionResult.Count > 0)
                    {
                        oAreaList.Add(oParkingArea);
                    }
                    if (oAreaList.Count > 0)
                    {
                        var oMaxAreaLoad = PolicyCal.GetMaxLoadFactor(oAreaList);
                        if (oMaxAreaLoad.xiStatus == 0 && oMaxAreaLoad.oCollectionResult.Count > 0)
                        {
                            oXiResults.Add(oMaxAreaLoad);
                        }
                    }
                    //District
                    List<CResult> oDiscrictList = new List<CResult>();
                    var oDistrict = PolicyCal.GetDistrict(sPostCode, sProductName, sProductCode, sVersion, sDataBase);
                    if (oDistrict.xiStatus == 0 && oDistrict.oCollectionResult.Count > 0)
                    {
                        oDiscrictList.Add(oDistrict);
                    }
                    var oParkingDistrict = PolicyCal.GetDistrict(sParkingPostCode, sProductName, sProductCode, sVersion, sDataBase);
                    if (oParkingDistrict.xiStatus == 0 && oParkingDistrict.oCollectionResult.Count > 0)
                    {
                        oDiscrictList.Add(oParkingDistrict);
                    }
                    if (oDiscrictList.Count > 0)
                    {
                        var oMaxDistrictLoad = PolicyCal.GetMaxLoadFactor(oDiscrictList);
                        if (oMaxDistrictLoad.xiStatus == 0 && oMaxDistrictLoad.oCollectionResult.Count > 0)
                        {
                            oXiResults.Add(oMaxDistrictLoad);
                        }
                    }
                    //Vehicle Make
                    var oVehicleMake = PolicyCal.GetVehicleMakeLoadFactor(sCaravanMake);
                    if (oVehicleMake.xiStatus == 0 && oVehicleMake.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oVehicleMake);
                    }
                    //Vehicle Value
                    double rCaravanVal = 0;
                    if (double.TryParse(sCaravanValue, out rCaravanVal))
                    { }
                    var oVehicleValue = PolicyCal.GetVechileValue(rCaravanVal);
                    if (oVehicleValue.xiStatus == 0 && oVehicleValue.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oVehicleValue);
                    }
                    //Driver Age
                    if (AgesResult.Count > 0)
                    {
                        var oAgeLoadFactor = (PolicyCal.GetMaxLoadFactor(AgesResult));
                        if (oAgeLoadFactor.xiStatus == 0 && oAgeLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oAgeLoadFactor);
                        }
                    }
                    //Vehicle age
                    int iYearOfManufacture = 0;
                    if (int.TryParse(sYearOfManufacture, out iYearOfManufacture))
                    { }
                    var oVehicleAgeLoad = PolicyCal.GetAgeOfVechicleDiscount(iYearOfManufacture);
                    if (oVehicleAgeLoad.xiStatus == 0 && oVehicleAgeLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oVehicleAgeLoad);
                    }
                    //OwnerShip
                    int iYearOfOwnership = 0;
                    int iYearOfOwnershipInMonths = 0;
                    var oDateOfPurchase = PolicyCal.GetAgeFromDOB(sDateofpurchase, DateTime.Now.ToString());
                    if (oDateOfPurchase.xiStatus == 00 && oDateOfPurchase.oResult != null)
                    {
                        iYearOfOwnership = (int)oDateOfPurchase.oResult;
                        DateTime dDAte = Convert.ToDateTime(sDateofpurchase);
                        iYearOfOwnershipInMonths = 12 * (DateTime.Now.Year - dDAte.Year) + DateTime.Now.Month - dDAte.Month;
                        if ((DateTime.Now < Convert.ToDateTime(sDateofpurchase).AddMonths(iYearOfOwnershipInMonths))) iYearOfOwnershipInMonths--;
                    }
                    var oOwnerShipLoad = PolicyCal.GetOwnershipDiscount(iYearOfOwnershipInMonths);
                    if (oOwnerShipLoad.xiStatus == 0 && oOwnerShipLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oOwnerShipLoad);
                    }
                    //Club Member
                    var oClubMemberLoad = PolicyCal.GetClubMemberDiscount(bIsClubMember);
                    if (oClubMemberLoad.xiStatus == 0 && oClubMemberLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oClubMemberLoad);
                    }
                    //Mileage
                    int iMileage = 0;
                    if (int.TryParse(sMileage, out iMileage))
                    { }
                    var oMileage = PolicyCal.GetMileageLoadFactor(iMileage);
                    if (oMileage.xiStatus == 0 && oMileage.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oMileage);
                    }
                    //Engine CC
                    int iEngine = 0;
                    if (int.TryParse(sEngineSize, out iEngine))
                    { }
                    var oEngineLoad = PolicyCal.GetEngineSizeLoadFactor(iEngine);
                    if (oEngineLoad.xiStatus == 0 && oEngineLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oEngineLoad);
                    }
                    //Driving Restriction
                    var oDrivingRestrictionsLoad = PolicyCal.GetDrivingRestrictionsLoadFactor(oRelation);
                    if (oDrivingRestrictionsLoad.xiStatus == 00 && oDrivingRestrictionsLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oDrivingRestrictionsLoad);
                    }
                    //if (DrivingRestrictions.Count > 0)
                    //{
                    //    var oDrivingRestrictionsLoad = PolicyCal.GetMaxLoadFactor(DrivingRestrictions);
                    //    if (oDrivingRestrictionsLoad.xiStatus == 0 && oDrivingRestrictionsLoad.oCollectionResult.Count > 0)
                    //    {
                    //        oXiResults.Add(oDrivingRestrictionsLoad);
                    //    }
                    //}
                    //LHD
                    //CResult LeftHandLoad = new CResult();
                    var oLeftHandLoad = PolicyCal.GetLeftHandDriveLoadFactor(bISLeftHandDrive, iYearOfOwnershipInMonths);
                    if (oLeftHandLoad.xiStatus == 0 && oLeftHandLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oLeftHandLoad);
                    }
                    //Claims
                    //if (ClaimResult != null && ClaimResult.xiStatus == 0 && ClaimResult.oCollectionResult.Count > 0)
                    //{
                    //    oXiResults.Add(ClaimResult);
                    //}
                    //Convictions
                    //if (ConvictionResult != null && ConvictionResult.xiStatus == 0 && ConvictionResult.oCollectionResult.Count > 0)
                    //{
                    //    oXiResults.Add(ConvictionResult);
                    //}
                    //Claim or Conviction Demerit points
                    //if (ClaimConvictionResultList.Count > 0)
                    //{
                    //   var oClaimConvictionLoad = PolicyCal.GetMaxLoadFactor(ClaimConvictionResultList);
                    if (ClaimConvictionResult.xiStatus == 0 && ClaimConvictionResult.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(ClaimConvictionResult);
                    }
                    // }
                    //if (ClaimConvictionResult != null && ClaimConvictionResult.xiStatus == 0 && ClaimConvictionResult.oCollectionResult.Count > 0)
                    //{
                    //    oXiResults.Add(ClaimConvictionResult);
                    // }
                    //Vechile Use
                    var oVehicleUse = PolicyCal.GetVechileUseLoadFactor(sUseOfVechile);
                    if (oVehicleUse.xiStatus == 0 && oVehicleUse.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oVehicleUse);
                    }
                    //Garaging
                    var oGaraging = PolicyCal.GetGaragingLoadFactor(sGaraging, rCaravanVal, sPostCode, sLocationOfMotorhomeOvernight, iProductID);
                    if (oGaraging.xiStatus == 0 && oGaraging.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oGaraging);
                    }
                    //Voluntary Excess
                    double rVoluntaryExcess = 0.00;
                    int iVoluntaryExcess = 0;
                    if (int.TryParse(sVoluntaryExcess, out iVoluntaryExcess))
                    { }
                    //oResult.sMessage = "sVoluntaryExcess_" + sVoluntaryExcess;
                    //oIB.SaveErrortoDB(oResult);
                    Info.Add("sVoluntaryExcess_" + sVoluntaryExcess);
                    if (iVoluntaryExcess == 10)
                    {
                        sVoluntaryExcess = "0";
                    }
                    //oResult.sMessage = "sVoluntaryExcess_sval_" + sVoluntaryExcess;
                    //oIB.SaveErrortoDB(oResult);
                    Info.Add("sVoluntaryExcess_sval_" + sVoluntaryExcess);
                    var oVoluntaryExcess = PolicyCal.GetVoluntaryExcessLoadFactor(iVoluntaryExcess);
                    if (oVoluntaryExcess.xiStatus == 0 && oVoluntaryExcess.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oVoluntaryExcess);
                    }
                    if (double.TryParse(sVoluntaryExcess.ToString(), out rVoluntaryExcess)) { }
                    //oResult.sMessage = "rVoluntaryExcess_" + rVoluntaryExcess;
                    //oIB.SaveErrortoDB(oResult);
                    Info.Add("rVoluntaryExcess_" + rVoluntaryExcess);
                    //Protected No Claim Bonus
                    if (sLiketoProtectYourNoClaimsDiscountUpdated != null)
                    {
                        int iNoClaimYears = 0;
                        if (int.TryParse(sNoClaimsYears, out iNoClaimYears))
                        { }
                        Info.Add("sNoClaimsYears_" + sNoClaimsYears + " _iNoClaimYears_" + iNoClaimYears);
                        int iPrivateClaimYears = 0;
                        if (int.TryParse(sPrivatenocclaim, out iPrivateClaimYears))
                        { }
                        Info.Add("sPrivatenocclaim_" + sPrivatenocclaim + " _iPrivateClaimYears_" + iPrivateClaimYears);
                        Info.Add("sLiketoProtectYourNoClaims_" + sLiketoProtectYourNoClaimsDiscountUpdated);
                        var oProtectedNoClaimDiscountLoadFactor = PolicyCal.GetProtectedNoClaimsDiscount(sLiketoProtectYourNoClaimsDiscountUpdated, iNoClaimYears, iPrivateClaimYears);
                        if (oProtectedNoClaimDiscountLoadFactor.xiStatus == 0 && oProtectedNoClaimDiscountLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oProtectedNoClaimDiscountLoadFactor);
                        }
                    }
                    //Minimum Premium
                    var oMinimumPremium = new CResult();
                    var MinimumPremium = PolicyCal.GetMinimumPremium(iYearOfManufacture);
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
                    //Modificatons
                    var ModificationLoad = new CResult();
                    //oResult.sMessage = "sVechileModification_ResolvedVal_" + svehicleModifications;
                    //oIB.SaveErrortoDB(oResult);
                    Info.Add("sVechileModification_ResolvedVal_" + svehicleModifications);
                    var oModifications = PolicyCal.Modifications(svehicleModifications);
                    if (oModifications.xiStatus == 0 && oModifications.oCollectionResult != null)
                    {
                        ModificationLoad = oModifications;
                        var sModificationstatus = ModificationLoad.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault();
                        if (!string.IsNullOrEmpty(sModificationstatus) && sModificationstatus == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())
                        {
                            oGeneralDeclines.Add(ModificationLoad);
                        }
                    }
                    //Occupation 
                    CResult oOccupationsLoad = new CResult();
                    if (OccupationResult.Count > 0)
                    {
                        var oOccupations = PolicyCal.GetMaxLoadFactor(OccupationResult);
                        if (oOccupations.xiStatus == 0 && oOccupations.oCollectionResult.Count > 0)
                        {
                            oOccupationsLoad = oOccupations;
                        }
                    }
                    //Compulsary Excess
                    double rCompulsaryExcess = 0.00;
                    CResult oCompulsaryExcessLoad = new CResult();
                    var ocompulsaryExcess = PolicyCal.CompulsaryExcess(rCaravanVal);
                    if (ocompulsaryExcess.xiStatus == 0 && ocompulsaryExcess.oCollectionResult.Count > 0)
                    {
                        oCompulsaryExcessLoad = ocompulsaryExcess;
                        if (double.TryParse(oCompulsaryExcessLoad.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), out rCompulsaryExcess)) { }
                    }

                    //General Declines                    
                    //if (!string.IsNullOrEmpty(sHowManyOtherVehicles))
                    //{
                    //    int iOtherVehicles = 0;
                    //    int.TryParse(sHowManyOtherVehicles, out iOtherVehicles);
                    //    if (iOtherVehicles == 0)
                    //    {
                    //        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Other vehicles are 0", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    //    }
                    //}

                    int iNoOfSeats = 0;
                    if (int.TryParse(sNoSeats, out iNoOfSeats))
                    {
                    }
                    if (iNoOfSeats > 5)
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "No of seats", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    if (sInsurancePolicyDetails == "Yes")
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Refused or canceled insurance", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    if (sMotorOffence == "Yes")
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Criminal Conviction", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    if (!string.IsNullOrEmpty(sTypeOfCaravan) && (sTypeOfCaravan.ToLower() == "other" || sTypeOfCaravan.ToLower() == "campervan"))
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Type of Motorhome", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    //if (rCaravanVal > 25000 && sGaraging.ToLower() == "kept on public road")
                    // {
                    //    oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Garaging General Decline", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    // }
                    string[] DeclineOwnerList = new string[] { "company other than proposer", "vehicle leasing company", "parent", "common law partner", "son or daughter", "other", "employer", "employee", "proposer's business partner", "contract hire" };
                    if (!string.IsNullOrEmpty(sVehicleOwner) && DeclineOwnerList.Contains(sVehicleOwner.ToLower()))
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Vehicle Owner", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    if (!string.IsNullOrEmpty(sVehicleKeeper) && (sVehicleKeeper.ToLower() != "proposer/policyholder" && sVehicleKeeper.ToLower() != "spouse"))
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Vehicle Keeper", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }


                    //oResult.sMessage = "RiskFactorsCount:" + oXiResults.Count();
                    //oIB.SaveErrortoDB(oResult);
                    Info.Add("RiskFactorsCount:" + oXiResults.Count());
                    foreach (var item in oXiResults)
                    {
                        string sMessage = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        //oResult.sMessage = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        //oIB.SaveErrortoDB(oResult);
                        Info.Add(sMessage);
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
                    double total = 0.00;
                    double BaseLoad = 0.00;
                    double rTotalExcess = rCompulsaryExcess + rVoluntaryExcess;

                    string ExcessContent = "<table class=\"table\">";
                    ExcessContent += "<tr>";
                    ExcessContent += "<td class=\"text-left\">Compulsory</td>";
                    ExcessContent += "<td class=\"text-right\">£" + rCompulsaryExcess + "";
                    ExcessContent += "</td>";
                    ExcessContent += "</tr>";
                    ExcessContent += "<tr>";
                    ExcessContent += "<td class=\"text-left\">Voluntary</td>";
                    ExcessContent += "<td class=\"text-right\">£" + rVoluntaryExcess + "";
                    ExcessContent += "</td>";
                    ExcessContent += "</tr>";
                    ExcessContent += "<tr>";
                    ExcessContent += "<td class=\"text-left\">Total</td>";
                    ExcessContent += "<td class=\"text-right\">£" + rTotalExcess + "</td>";
                    ExcessContent += "</tr>";
                    ExcessContent += "</table>";

                    if (oBaseLoadFactor.oCollectionResult != null)
                    {
                        BaseLoad = Convert.ToDouble(oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                    }
                    //if (oCompulsaryExcessLoad.oCollectionResult != null)
                    // {
                    //    total = BaseLoad + Convert.ToDouble(oCompulsaryExcessLoad.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                    // }
                    total = BaseLoad;
                    if (oXiResults != null && oXiResults.Count() > 0)
                    {
                        int i = 0;
                        foreach (var item in oXiResults)
                        {
                            if (item.oCollectionResult != null)
                            {
                                if (item.oCollectionResult.Where(m => m.sName == "Type").Select(m => m.sValue).FirstOrDefault() == "Percent")
                                {
                                    BaseLoad += ((Convert.ToDouble(item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * BaseLoad);
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
                    //BaseLoad = Math.Round(BaseLoad, 2);
                    total = BaseLoad;
                    result = PolicyCal.BuildCResultObject(NetPremium.ToString(), "Net Premium", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString());
                    oXiResults.Add(result);
                    oProductI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Product");
                    oProductI.Attributes["iDefaultAmountType"].BOD = oProductI.BOD;
                    //string sMinPremium = oProductI.Attributes["rMinPremium"].sValue;
                    double rMinPremium = 0.0;
                    string sMinPremium = oMinimumPremium.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                    if (double.TryParse(sMinPremium, out rMinPremium))
                    {
                        if (rMinPremium > total)
                        {
                            total = rMinPremium;
                        }
                    }
                    //result = PolicyCal.BuildCResultObject(rMinPremium.ToString(), "Net minimum Premium", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString());
                    oXiResults.Add(oMinimumPremium);
                    //oXiResults.Add(LeftHandLoad);
                    //if (LeftHandLoad.oCollectionResult != null)
                    //{
                    //    total += ((Convert.ToDouble(LeftHandLoad.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * BaseLoad);
                    //}


                    double rAdditionalLoad = 0;
                    if (oProductVersionI.Attributes.ContainsKey("rAdditionalLoad"))
                    {
                        var AdditionLoad = oProductVersionI.Attributes["rAdditionalLoad"].sValue;
                        if (double.TryParse(AdditionLoad, out rAdditionalLoad))
                        {
                        }
                    }
                    oXiResults.Add(oOccupationsLoad);
                    //oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = rCompulsaryExcess.ToString(), bDirty = true };
                    if (oXiResults.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())) && oCompulsaryExcessLoad.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && IPTLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && oMinimumPremium.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && oOccupationsLoad.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && oGeneralDeclines.Count <= 0)
                    {
                        //oResult.sMessage = "All Load Factors are normal";
                        //oIB.SaveErrortoDB(oResult);
                        Info.Add("All Load Factors are normal");
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "0", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "false", bDirty = true };
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())) || oOccupationsLoad.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString() || oCompulsaryExcessLoad.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString() || oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString() || oMinimumPremium.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString() && oGeneralDeclines.Count <= 0)
                    {
                        //oResult.sMessage = "Some Load Factors are Refered";
                        //oIB.SaveErrortoDB(oResult);
                        Info.Add("Some Load Factors are Refered");
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "10", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "true", bDirty = true };
                        oXiResults.Add(PolicyCal.BuildCResultObject(rAdditionalLoad.ToString(), "Additional Load", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                        total += (total * 0.01) * rAdditionalLoad;
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())) || oOccupationsLoad.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString() || oCompulsaryExcessLoad.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString() || oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString() && oMinimumPremium.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString() || oGeneralDeclines.Count > 0)
                    {
                        oResult.sMessage = "Some Load Factors are Declined";
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "20", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "true", bDirty = true };
                        oXiResults.Add(PolicyCal.BuildCResultObject(rAdditionalLoad.ToString(), "Additional Load", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                        total += (total * 0.01) * rAdditionalLoad;
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
                    //string iSourceID = "";
                    //if(ostructureInstance.Attributes != null && ostructureInstance.Attributes.ContainsKey("fkisourceid"))
                    // {
                    //    iSourceID = ostructureInstance.Attributes["fkisourceid"].sValue;
                    // }
                    //oResult.sMessage = "SourceID :" + iSourceID;
                    //oIB.SaveErrortoDB(oResult);
                    //var oSource = oIXI.BOI("XISource_T", ostructureInstance.Attributes["fkisourceid"].sValue);
                    //string sPrefix = string.Empty;
                    //if (oSource.Attributes != null && oSource.Attributes.ContainsKey("sprefixcode"))
                    //{
                    //    sPrefix = oSource.Attributes["sprefixcode"].sValue;
                    //}
                    var iBatchID = iCustomerID.ToString() + iInsatnceID.ToString();
                    oBII.Attributes["sRegNo"] = new XIIAttribute { sName = "sRegNo", sValue = ostructureInstance.XIIValue("sRegNo").sValue, bDirty = true };
                    oBII.Attributes["dCoverStart"] = new XIIAttribute { sName = "dCoverStart", sValue = dtInsuranceCoverStartDate, bDirty = true };
                    oBII.Attributes["sCaravanMake"] = new XIIAttribute { sName = "sCaravanMake", sValue = ostructureInstance.XIIValue("sCaravanMakeUpdated").sDerivedValue, bDirty = true };
                    oBII.Attributes["sCaravanModel"] = new XIIAttribute { sName = "sCaravanModel", sValue = ostructureInstance.XIIValue("sModelofCaravanUpdated").sValue, bDirty = true };

                    oBII.Attributes["FKiQSInstanceID"] = new XIIAttribute { sName = "FKiQSInstanceID", sValue = iInsatnceID.ToString(), bDirty = true };
                    oBII.Attributes["sInsurer"] = new XIIAttribute { sName = "sInsurer", sValue = "MARKERSTUDY", bDirty = true };
                    oBII.Attributes["FKiCustomerID"] = new XIIAttribute { sName = "FKiCustomerID", sValue = iCustomerID.ToString(), bDirty = true };
                    oBII.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                    oBII.Attributes["dtqsupdateddate"] = new XIIAttribute { sName = "dtqsupdateddate", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBII.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBII.Attributes["FKiProductVersionID"] = new XIIAttribute { sName = "FKiProductVersionID", sValue = sVersion, bDirty = true };
                    oBII.Attributes["BatchID"] = new XIIAttribute { sName = "BatchID", sValue = iBatchID, bDirty = true };
                    oBII.Attributes["sGUID"] = new XIIAttribute { sName = "sGUID", sValue = sQuoteGUID, bDirty = true };

                    //Random generator = new Random();
                    //string sRef = generator.Next(1, 10000000).ToString(new String('0', 7));
                    //oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + sRef, bDirty = true };
                    //oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + Guid.NewGuid().ToString("N").Substring(0, 6), bDirty = true };
                    oBII.Attributes["sExcess"] = new XIIAttribute { sName = "sExcess", sValue = ExcessContent, bDirty = true };
                    oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = String.Format("{0:0.00}", rCompulsaryExcess), bDirty = true };
                    oBII.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = String.Format("{0:0.00}", rVoluntaryExcess), bDirty = true };
                    oBII.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = String.Format("{0:0.00}", rTotalExcess), bDirty = true };
                    //oBII.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = rVoluntaryExcess.ToString(), bDirty = true };
                    //oBII.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = (rCompulsaryExcess + rVoluntaryExcess).ToString(), bDirty = true };

                    oBII.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = iQuoteID.ToString(), bDirty = true };
                    //Info.Add("QuoteRefID_" + oBII.Attributes["sRefID"].sValue);
                    var oRes = oBII.Save(oBII);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        oBII = (XIIBO)oRes.oResult;
                    }
                    //oResult.sMessage = "MARKERSTUDY Quote inserted Sucessfully with the amount of " + total;
                    //oIB.SaveErrortoDB(oResult);
                    Info.Add("MARKERSTUDY Quote inserted Sucessfully with the amount of " + total);
                    XIDBO oRiskFactorsBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RiskFactor_T");
                    XIIBO oBO = new XIIBO();
                    oBO.BOD = oRiskFactorsBOD;

                    List<XIIBO> oBOIList = new List<XIIBO>();
                    oBO.Attributes["FKiQuoteID"] = new XIIAttribute { sName = "FKiQuoteID", sValue = oBII.Attributes["ID"].sValue, bDirty = true };
                    oBO.Attributes["sFactorName"] = new XIIAttribute { sName = "sFactorName", sValue = "Base Rate", bDirty = true };
                    oBO.Attributes["sValue"] = new XIIAttribute { sName = "sValue", sValue = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    oBO.Attributes["sMessage"] = new XIIAttribute { sName = "sMessage", sValue = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    oBO.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBO.Attributes["FKsQuoteID"] = new XIIAttribute { sName = "FKsQuoteID", sValue = sQuoteGUID, bDirty = true };
                    oBO.Attributes["ID"] = new XIIAttribute { sName = "ID", bDirty = true };
                    oBOIList.Add(oBO);
                    // oBO = new XIIBO();
                    // oBO.BOD = oRiskFactorsBOD;
                    // oBO.Attributes["FKiQuoteID"] = new XIIAttribute { sName = "FKiQuoteID", sValue = oBII.Attributes["ID"].sValue, bDirty = true };
                    // oBO.Attributes["sFactorName"] = new XIIAttribute { sName = "sFactorName", sValue = oCompulsaryExcessLoad.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    // oBO.Attributes["sValue"] = new XIIAttribute { sName = "sValue", sValue = oCompulsaryExcessLoad.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    //  oBO.Attributes["sMessage"] = new XIIAttribute { sName = "sMessage", sValue = oCompulsaryExcessLoad.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    // oBO.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    // oBO.Attributes["FKsQuoteID"] = new XIIAttribute { sName = "FKsQuoteID", sValue = sQuoteGUID, bDirty = true };
                    //oBO.Attributes["ID"] = new XIIAttribute { sName = "ID", bDirty = true };
                    //oBOIList.Add(oBO);
                    oBO = new XIIBO();
                    oBO.BOD = oRiskFactorsBOD;
                    //oXiResults.Add(IPTLoadFactor);
                    foreach (var item in oXiResults)
                    {
                        oBO = new XIIBO();
                        oBO.BOD = oRiskFactorsBOD;
                        //oResult.sMessage = "RiskFacor " + item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault() + " _ " + item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() + " _ " + item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault() + "_QuoteID" + oBII.Attributes["ID"].sValue;
                        //oIB.SaveErrortoDB(oResult);
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