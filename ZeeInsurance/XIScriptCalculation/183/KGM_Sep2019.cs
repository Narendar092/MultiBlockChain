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
    public class KGM_Sep2019
    {
        public static CResult PolicyMainCal(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            PolicyCalculation Pcal = new PolicyCalculation();
            //XIIQS oQsInstance = new XIIQS();
            CResult oResult = new CResult();
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
                //XIInfraCache XIInfraCache = new XIInfraCache();
                //oQsInstance = XIInfraCache.Get_QuestionSetCache("QuestionSetCache", sUID, iInsatnceID);
                //oResult.sMessage = "In Script: Questionset object Loaded from cache";
                //oIB.SaveErrortoDB(oResult);
                oResult = Pcal.Calculation(iInsatnceID, iUserID, iCustomerID, sDataBase, sProductName, sProductCode, sVersion, sSessionID, sUID, iQuoteID);
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
        public abstract class PolicyBaseCalc
        {
            public static int EngineLookUp(int EngineSize)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                int res = 0;
                try
                {
                    if (EngineSize <= 1300)
                    {
                        res = 1;
                    }
                    else if (EngineSize > 1300 && EngineSize <= 2000)
                    {
                        res = 2;
                    }
                    else if (EngineSize > 2000 && EngineSize <= 2500)
                    {
                        res = 3;
                    }
                    else if (EngineSize > 2500 && EngineSize <= 3000)
                    {
                        res = 4;
                    }
                }
                catch (Exception ex)
                {
                    oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oResult);
                }
                return res;
            }
            public static string GetPostCodeLookUp(string PostCode, int EngineSize, string sDataBase, string sProductName, string sVersion, string sProductCode)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                string res = "";
                try
                {
                    XIPostCodeLookUp oPostCodeLookUp = new XIPostCodeLookUp();
                    oPostCodeLookUp.sMPostCode = PostCode;
                    oPostCodeLookUp.sProductName = sProductName;
                    oPostCodeLookUp.sVersion = sVersion;
                    oPostCodeLookUp.sProductCode = sProductCode;
                    var PostCodeGroup = oPostCodeLookUp.Get_PostCode();
                    var Area = "";
                    if (PostCodeGroup.xiStatus == 0 && PostCodeGroup.oResult != null)
                    {
                        Area = PostCodeGroup.oResult.ToString();
                        if (Area.Contains("Refer") || Area.Contains("Decline"))
                        {
                            res = Area;
                        }
                        else
                        {
                            var Group = EngineLookUp(EngineSize);
                            res = Area + Group;
                        }
                    }
                }
                catch (Exception ex)
                {
                    oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oResult.oResult = "Error";
                    oIB.SaveErrortoDB(oResult);
                }
                return res;
            }
            public CResult GetYoungAgeLoadFactor(int Age, string bIsMainDriver)
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
                    oNV2.sValue = "Young Additional Driver Load";
                    if (Age >= 25 && Age <= 29 && bIsMainDriver.ToLower() == "false")//AdditionalDriver only
                    {
                        oNV1.sValue = "+30";
                    }
                    else if (Age >= 25 && Age <= 29 && bIsMainDriver.ToLower() == "true")
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
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
                    oNV2.sValue = "Age of Driver Load/Discount";

                    if (Age > 0)
                    {
                        if (Age >= 30 && Age <= 34)
                        {
                            oNV1.sValue = "+10";
                        }
                        else if (Age >= 35 && Age <= 39)
                        {
                            oNV1.sValue = "0";
                        }
                        else if (Age >= 40 && Age <= 69)
                        {
                            oNV1.sValue = "-15";
                        }
                        else if (Age >= 70 && Age <= 74)
                        {
                            oNV1.sValue = "-10";
                        }
                        else if (Age < 25 || Age > 74)
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public abstract double GetNetAmount(string TypeOfCover, double Amount);
            public abstract CResult GetBaseRate(string AreaGroup, string comp);
            public CResult GetUseLoadFactor(string sUse)
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
                    oNV2.sValue = "Use Load";
                    if (sUse == "SDPC")
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
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
                    oNV2.sValue = "Limited Mileage Discount";
                    if (miles <= 1500)
                    {
                        oNV1.sValue = "-25";
                    }
                    else if (miles > 1500 && miles <= 3000)
                    {
                        oNV1.sValue = "-17.5";
                    }
                    else if (miles > 3000 && miles <= 5000)
                    {
                        oNV1.sValue = "-10";
                    }
                    else if (miles > 5000 && miles <= 8000)
                    {
                        oNV1.sValue = "+10";
                    }
                    else if (miles > 8000 && miles <= 10000)
                    {
                        oNV1.sValue = "+25";
                    }
                    else
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
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
                    oNV2.sValue = "Club Member Discount";
                    //oNVsList.Add(oNV2);
                    if (IsClubMember.ToLower() == "10")
                    {
                        oNV1.sValue = "-10";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
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
                    oNV2.sValue = "Age of Vehicle Discount";
                    if (YearofManufacture <= 1994)
                    {
                        oNV1.sValue = "-15";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
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
                    oNV2.sValue = "Ownership Discount";

                    if (YearOfOwnerShip < 12)
                    {
                        oNV1.sValue = "+10";
                    }
                    else if (YearOfOwnerShip >= 12 && YearOfOwnerShip < 24)
                    {
                        oNV1.sValue = "-5";
                    }
                    else if (YearOfOwnerShip >= 24 && YearOfOwnerShip < 32)
                    {
                        oNV1.sValue = "-10";
                    }
                    else if (YearOfOwnerShip >= 32 && YearOfOwnerShip < 48)
                    {
                        oNV1.sValue = "-15";
                    }
                    else if (YearOfOwnerShip >= 48 && YearOfOwnerShip < 60)
                    {
                        oNV1.sValue = "-20";
                    }
                    else if (YearOfOwnerShip >= 60)
                    {
                        oNV1.sValue = "-25";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetLeftHandDriveLoadFactor(string IsLeftHandDrive, int iYearOfOwnership)
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
                    oNV2.sValue = "Left hand Driver";
                    if (IsLeftHandDrive.ToLower() == "10" && iYearOfOwnership >= 1)
                    {
                        oNV1.sValue = "+10";
                    }
                    else if (IsLeftHandDrive.ToLower() == "10" && iYearOfOwnership < 1)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetNoClaimsDiscount(int Year)
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
                    if (Year == 1)
                    {
                        oNV1.sValue = "+10";
                    }
                    else if (Year == 2)
                    {
                        oNV1.sValue = "+5";
                    }
                    else if (Year == 3)
                    {
                        oNV1.sValue = "+0";
                    }
                    else if (Year == 4)
                    {
                        oNV1.sValue = "-2.5";
                    }
                    else if (Year >= 5)
                    {
                        oNV1.sValue = "-5";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;

            }
            public CResult GetProtectedNoClaimsDiscount(string IsProtectedNoClaimBonus, int iCaravanNoClaims)
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
                    if (!string.IsNullOrEmpty(IsProtectedNoClaimBonus) && IsProtectedNoClaimBonus.ToLower() == "yes")
                    {
                        if (iCaravanNoClaims >= 5)
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                            oNV1.sValue = "+2.5";
                        }
                        else
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetDrivingDiscount(List<string> type, int youngdrivercount)
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
                    oNV2.sValue = "Driving Discounts";
                    int icount = type.Count;
                    if (icount == 1)
                    {
                        oNV1.sValue = "-15";
                    }
                    else if (icount == 2 && type.Contains("spouse"))
                    {
                        oNV1.sValue = "-25";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetClaimLoadFactor(int NoOfClaims, List<XIIBO> ClaimI, string CoverStartDate)
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
                    oNV2.sValue = "Claims Load";
                    int NoOfFaultClaims = 0;
                    int NoOfFireTheftClaims = 0;
                    List<int> FaultClaimYears = new List<int>();
                    List<int> NonFaultClaimYears = new List<int>();
                    int NoOfNonfaultClaims = 0;
                    double rTotalCost = 0;
                    foreach (var Claim in ClaimI)
                    {
                        if (oNV.sValue != xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())
                        {
                            var sClaimName = Claim.AttributeI("sName").ResolveFK("display");
                            var sFault = Claim.AttributeI("iWhoseFault").sResolvedValue;
                            var claimCost = Claim.AttributeI("rTotalClaimCost").sValue;
                            var InjuredCost = Claim.AttributeI("rCostInjured").sValue;
                            int ClaimYear = 0;
                            var result = GetAgeFromDOB(Claim.AttributeI("dDate").sValue, CoverStartDate);
                            if (result.xiStatus == 00 && result.oResult != null)
                            {
                                ClaimYear = (int)result.oResult;
                            }
                            double rInjuredCose = 0;
                            if (double.TryParse(InjuredCost, out rInjuredCose))
                            { }
                            double rCost = 0;
                            if (ClaimYear < 3)
                            {
                                if (sFault.ToLower() == "policy holder fault")
                                {
                                    NoOfFaultClaims++;
                                    FaultClaimYears.Add(ClaimYear);
                                }
                                else if (sFault.ToLower() == "non fault")
                                {
                                    NoOfNonfaultClaims++;
                                    NonFaultClaimYears.Add(ClaimYear);
                                }
                                else if (sFault.ToLower() == "fire + theft")
                                {
                                    NoOfFireTheftClaims++;
                                }
                                if (double.TryParse(claimCost, out rCost))
                                { rTotalCost += rCost; }
                                sClaimName = sClaimName.ToLower();
                                if (sClaimName == "vandalism" || sClaimName == "malicious damage (theft)" || sClaimName == "riot")
                                {
                                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                }
                                else if (rInjuredCose > 0)
                                {
                                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                                }
                            }
                            else if (ClaimYear > 3)
                            {
                                oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                            }
                            if (NoOfFaultClaims >= 2 || NoOfNonfaultClaims >= 3 || NoOfFireTheftClaims >= 2 || rTotalCost > 10000)
                            {
                                oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                            }
                        }
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetConvictionsLoadFactor(int NoOfConvictions, List<XIIBO> ConvictionI, string PolicyStartDate)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                XIIXI oIXI = new XIIXI();
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
                    oNV2.sValue = "Convictions Load";
                    string[] DeclineCodesB = new string[] { "AC", "BA", "UT" };
                    string[] DeclineCodesNB = new string[] { "DG", "E", "MR", "MT", "N", "NA", "NE", "NI", "NR", "NEND", "QI", "SC", "TT", "XX", "Z" };
                    string[] RefCodes = new string[] { "CU", "MW", "PC", "PL", "SP", "TS" };
                    string Prefix = ""; int numberPart = 0;
                    var DeclinedReferlist = new List<string>(); var NormalList = new List<int>();
                    string stringArra = string.Empty; int convictionYear = 0;
                    int iBan = 0;
                    foreach (var Conviction in ConvictionI)
                    {
                        var oConvictionI = oIXI.BOI("refConviction_T", Conviction.AttributeI("refconviction").sValue);

                        if (Conviction.AttributeI("dDate").sValue != null)
                        {
                            var result = GetAgeFromDOB(Conviction.AttributeI("dDate").sValue, PolicyStartDate);
                            if (result.xiStatus == 00 && result.oResult != null)
                            {
                                convictionYear = (int)result.oResult;
                            }
                        }
                        if (int.TryParse(Conviction.AttributeI("sLengthOfSuspension").sValue, out iBan))
                        {
                        }

                        var ConvictionCode = oConvictionI.Attributes["scode"].sValue;
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
                        DateTime dDAte = Convert.ToDateTime(Conviction.AttributeI("dDate").sValue);
                        DateTime sPolicyStartDAte = Convert.ToDateTime(PolicyStartDate);
                        //if (iBan > 0 && dDAte.AddYears(+3) <= sPolicyStartDAte)
                        if (iBan > 0 && dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte)
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                        }
                        // else if (iBan > 12 && dDAte.AddYears(+5) <= sPolicyStartDAte)
                        else if (iBan > 12 && dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte)
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                        }
                        else if ((Array.IndexOf(DeclineCodesB, Prefix) > -1 || ConvictionCode.ToUpper() == "S19") && (dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte))
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if ((Array.IndexOf(DeclineCodesNB, Prefix) > -1 || ConvictionCode.ToUpper() == "S35") && (dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte))
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if (Prefix == "DD" && ((numberPart >= 20 && numberPart <= 46) || (numberPart >= 60 && numberPart <= 86)) && (dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte))
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if (Prefix == "MS" && numberPart >= 40 && numberPart <= 96 && (dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte))
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;

                        }
                        else if (Prefix == "CD" && numberPart >= 40 && numberPart <= 96 && (dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte))
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if (Prefix == "LC" && numberPart >= 10 && numberPart <= 16 && (dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte))
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if (Prefix == "DD" && ((numberPart == 50) || (numberPart == 90) || numberPart == 10) && (dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte))
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if (Prefix == "LC" && numberPart >= 30 && numberPart <= 56 && (dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte))
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if (Prefix == "DR" && numberPart >= 80 && numberPart <= 96 && (dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte))
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if (Prefix == "DR" && numberPart >= 10 && numberPart <= 70 && (dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte))
                        {
                            if (NoOfConvictions == 1)
                            {
                                NormalList.Add(75);
                            }
                            else if (NoOfConvictions >= 2)
                            {
                                DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString());
                            }
                        }
                        else if (Prefix == "CD" && numberPart >= 10 && numberPart <= 30 && (dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte))
                        {
                            if (NoOfConvictions == 1)
                            {
                                NormalList.Add(25);
                            }
                            else if (NoOfConvictions >= 2)
                            {
                                DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString());
                            }
                        }
                        else if (ConvictionCode == "IN10" && (dDAte >= sPolicyStartDAte.AddYears(-5) && dDAte <= sPolicyStartDAte))
                        {
                            if (NoOfConvictions == 1)
                            {
                                NormalList.Add(50);

                            }
                            else if (NoOfConvictions >= 2)
                            {
                                DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString());
                            }
                        }
                        //CU, LC20, MS10-30, MW, PC, PL, SP, TS//CU, LC20, MS10-30, MW, PC, PL, SP, TS
                        else if ((Array.IndexOf(RefCodes, Prefix) > -1 || ConvictionCode.ToUpper() == "LC20") && (dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte))
                        {
                            if (NoOfConvictions <= 2)
                            {
                                NormalList.Add(0);
                            }
                            else
                            {
                                DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString());
                            }
                        }
                        else if (Prefix == "MS" && numberPart >= 10 && numberPart <= 30 && (dDAte >= sPolicyStartDAte.AddYears(-3) && dDAte <= sPolicyStartDAte))
                        {
                            if (NoOfConvictions <= 2)
                            {
                                NormalList.Add(0);
                            }
                            else
                            {
                                DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString());
                            }
                        }
                        else
                        {
                            NormalList.Add(0);
                        }
                    }
                    var DeclinedReferArray = DeclinedReferlist.ToArray(); var NormalArray = NormalList.ToArray();
                    if (Array.IndexOf(DeclinedReferArray, xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()) > -1)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    else if (Array.IndexOf(DeclinedReferArray, xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString()) > -1)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
                    }
                    else
                    {
                        oNV1.sValue = "+" + NormalArray.Max().ToString();
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetOccupationsLoadFactor(string sOccupation, int iAge)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    string sOccupationName = string.Empty;
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Occupation Load/Discount";

                    XIIXI oIXI = new XIIXI();
                    var oOccupationI = oIXI.BOI("enumMasterOccupation_T", sOccupation);
                    string IsDecline = "";
                    string PremiumLoad = "";
                    if (oOccupationI != null && oOccupationI.Attributes.Count > 0)
                    {
                        if (oOccupationI.Attributes.ContainsKey("bIsDecline"))
                        {
                            IsDecline = oOccupationI.Attributes["bIsDecline"].sValue;
                        }
                        if (oOccupationI.Attributes.ContainsKey("iPremiumLoad"))
                        {
                            PremiumLoad = oOccupationI.Attributes["iPremiumLoad"].sValue;
                        }
                        if (oOccupationI.Attributes.ContainsKey("sName"))
                        {
                            sOccupationName = oOccupationI.Attributes["sName"].sValue;
                        }
                    }
                    if (!string.IsNullOrEmpty(IsDecline) && IsDecline.ToLower() == "true")
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    else
                    {
                        int iPremiumLoad = 0;
                        if (int.TryParse(PremiumLoad, out iPremiumLoad))
                        {
                        }
                        oNV1.sValue = iPremiumLoad.ToString();
                    }
                    if (sOccupationName == "Retired" && iAge < 70)
                    {
                        oNV1.sValue = "+0";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetCancellationScales(DateTime PolicyAppliedDate)
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetOnRoadLoadLoadFactor(string ParkingPostCode, string sDataBase, string sProductName, string sVersion, string sProductCode, double rExcessVal, string sGaraging, string sLocationOfMotorhomeOvernight)
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
                    oNV2.sValue = "On Road Load";
                    var Area = "";
                    //if (sGaraging.ToLower() == "Kept on Third Party Premises".ToLower())

                    if (!string.IsNullOrEmpty(sGaraging))
                    {
                        sGaraging = sGaraging.ToLower();
                    }
                    if (sGaraging == "cassoa gold site")
                    {
                        oNV1.sValue = "+0";
                    }
                    else if ((!string.IsNullOrEmpty(sLocationOfMotorhomeOvernight) && (sLocationOfMotorhomeOvernight.ToLower() == "more than 100 metres of home address")) || sGaraging == "cassoa bronze site" || sGaraging == "cassoa silver site" || sGaraging == "car park")
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    else if (rExcessVal > 20000)
                    {
                        if (!string.IsNullOrEmpty(sLocationOfMotorhomeOvernight) && (sLocationOfMotorhomeOvernight.ToLower() == "within 100 metres of home address"))
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                        else if (!string.IsNullOrEmpty(sLocationOfMotorhomeOvernight) && sLocationOfMotorhomeOvernight.ToLower() == "home address" && !string.IsNullOrEmpty(sGaraging) && sGaraging.ToLower() == "kept on public road")
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                        }
                    }
                    else
                    {
                        XIPostCodeLookUp oPostCodeLookUp = new XIPostCodeLookUp();
                        oPostCodeLookUp.sMPostCode = ParkingPostCode;
                        oPostCodeLookUp.sProductName = sProductName;
                        oPostCodeLookUp.sVersion = sVersion;
                        oPostCodeLookUp.sProductCode = sProductCode;
                        var postcodegroup = oPostCodeLookUp.Get_PostCode();
                        if (postcodegroup.xiStatus == 0 && postcodegroup.oResult != null)
                        {
                            Area = postcodegroup.oResult.ToString();
                        }
                        Area = Area.ToUpper();
                        if (Area == "A" || Area == "B")
                        {
                            if (!string.IsNullOrEmpty(sLocationOfMotorhomeOvernight) && (sLocationOfMotorhomeOvernight.ToLower() == "within 100 metres of home address") && !string.IsNullOrEmpty(sGaraging) && sGaraging.ToLower() == "kept on public road")
                            {
                                oNV1.sValue = "+30";
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            if (sGaraging == "kept on public road")
                            {
                                oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                            }
                        }
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetExcessValue(double iCurrentCaravanValue, string sTypeOfCover, string sTypeofDevice)
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
                    oNV2.sValue = "ExcessValue";
                    int f1 = 0, f2 = 0;
                    double ExcessValue;
                    if (iCurrentCaravanValue >= 40000 && !string.IsNullOrEmpty(sTypeOfCover) && sTypeOfCover.ToLower() == "comp" && (string.IsNullOrEmpty(sTypeofDevice) || sTypeofDevice.ToLower() != "tracking device"))
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    else if (iCurrentCaravanValue > 60000 && !string.IsNullOrEmpty(sTypeOfCover) && sTypeOfCover.ToLower() == "comp" && (string.IsNullOrEmpty(sTypeofDevice) || sTypeofDevice.ToLower() == "tracking device"))
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    else if (iCurrentCaravanValue > 5000 && !string.IsNullOrEmpty(sTypeOfCover) && sTypeOfCover.ToLower() == "tpft")
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    else if (!string.IsNullOrEmpty(sTypeOfCover) && sTypeOfCover.ToLower() == "tpo")
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }

                    if (oNV.sValue != xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())
                    {
                        if (iCurrentCaravanValue > 15000 && iCurrentCaravanValue <= 40000)
                        {
                            f1 = Convert.ToInt32(Math.Ceiling((iCurrentCaravanValue - 15000) / 1000));
                        }
                        else if (iCurrentCaravanValue > 40000 && iCurrentCaravanValue <= 60000)
                        {
                            f1 = Convert.ToInt32(((40000 - 15000) / 1000));
                            f2 = Convert.ToInt32(Math.Ceiling((iCurrentCaravanValue - 40000) / 1000));
                        }
                    }

                    ExcessValue = f1 * 10 + f2 * 3;
                    //ExcessValue = 250;
                    oNV1.sValue = "+" + ExcessValue.ToString();
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
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
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetMaxVechileValuefactor(string type, int MaxValue)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "MaxVechileValuefactor";
                    type = type.ToLower();
                    if (type == "Comprehensive")
                    {
                        if (MaxValue <= 60000)
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                        }
                    }
                    else
                    {
                        if (MaxValue <= 5000)
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                        }
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetParkingLoadFactor(string Parkingtype)
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
                    oNV2.sValue = "ParkingLoadFactor";
                    Parkingtype = Parkingtype.ToLower();
                    if (Parkingtype == "road")
                    {
                        oNV1.sValue = "+30";
                    }
                    else if (Parkingtype == "other")
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
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
                            if (Convert.ToInt32(result.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) < Convert.ToInt32(item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()))
                                result = item;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.sMessage = "ERROR: [" + result.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    result.xiStatus = xiEnumSystem.xiFuncResult.xiError;
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
            public CResult GetMinimumPremium(string TypeOfCover, int iYearOfManufacture)
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
                    TypeOfCover = TypeOfCover.ToLower();
                    if (iYearOfManufacture <= 1994)
                    {
                        oNV1.sValue = "+149";
                    }
                    else if (TypeOfCover == "comp")
                    {
                        oNV1.sValue = "+159";
                    }
                    else if (TypeOfCover == "tpft")
                    {
                        oNV1.sValue = "+159";
                    }
                    //oNVsList.Add(oNV2);
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;

            }
            public CResult Modifications(List<string> sModification)
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
                    if (sModification.Any(m => m.ToLower() == "on") || sModification.Any(m => m.ToLower() == "true"))
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetVehicleValueLoadFactor(double iCurrentCaravanValue)
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
                    oNV2.sValue = "Vehicle Value Discount";
                    if (iCurrentCaravanValue >= 3000 && iCurrentCaravanValue <= 12000)
                    {
                        oNV1.sValue = "-15";
                    }
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
        }
        public class PolicyCalculation
        {
            public CResult Calculation(int iInsatnceID, int iUserID, int iCustomerID, string sDataBase, string sProductName, string sProductCode, string sVersion, string sSessionID, string sUID, int iQuoteID)
            {

                List<string> Info = new List<string>();
                Info.Add("[QsInstanceID_" + iInsatnceID + "]");
                Info.Add("KGM script running");
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                try
                {
                    List<CResult> oGeneralDeclines = new List<CResult>();
                    XIDXI oXIDXI = new XIDXI();
                    PolicyCal PolicyCal = new PolicyCal();
                    List<CResult> oXiResults = new List<CResult>();
                    XIIXI oIXI = new XIIXI();
                    XIIBO oBII = new XIIBO();
                    List<CResult> AgesResult = new List<CResult>();
                    List<CResult> YAgesResult = new List<CResult>();
                    List<int> claimYearList = new List<int>();
                    XIInfraCache oCache = new XIInfraCache();
                    List<CResult> DrivingRestrictions = new List<CResult>();
                    List<CResult> OccupationResult = new List<CResult>();
                    List<string> oRelation = new List<string>();
                    int Age = 0;
                    int claimYear = 0;
                    string DOB = "";
                    string sOccupation = "";
                    string sSecondaryOccupation = "";
                    oBII.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Aggregations");
                    List<CResult> ClaimResult = new List<CResult>();
                    List<CResult> NonFaultClaimResult = new List<CResult>();
                    int NoOfClaims = 0;
                    int NoOfConvinctions = 0;
                    var result = new CResult();
                    List<CResult> ConvictionResult = new List<CResult>();
                    var oQSI = oCache.Get_QsStructureObj(sSessionID, sUID, "QSInstance_" + iInsatnceID + "NotationStructure");
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    var oLIst = oQSI.oSubStructureI("Driver_T");
                    var oDriversI = (List<XIIBO>)oLIst.oBOIList;
                    string dtInsuranceCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;
                    double rClaimCostInjured = 0;
                    string bIsMainDriver = "";
                    int iNoOfDrivers = 0;
                    if (oDriversI != null)
                    {
                        iNoOfDrivers = oDriversI.Count;
                    }
                    foreach (var item in oDriversI)
                    {

                        List<object> ConvictionCodes = new List<object>();
                        List<int> ConvictionYears = new List<int>();
                        long rTotalCost = 0;
                        item.oBOStructure = item.SubChildI;
                        DOB = item.AttributeI("dDOB").sValue;
                        bIsMainDriver = item.AttributeI("bMainDriver").sValue;
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
                            Info.Add("Age_" + Age);
                            if (Age > 29)
                            {
                                result = PolicyCal.GetAgeLoadFactor(Age);
                                if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                                {
                                    AgesResult.Add(result);
                                }
                            }
                            else if (Age < 25)
                            {
                                oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Age is less than 24 Years", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                            }
                            else
                            {
                                result = PolicyCal.GetYoungAgeLoadFactor(Age, bIsMainDriver);
                                if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                                {
                                    YAgesResult.Add(result);
                                }
                            }
                        }
                        sOccupation = item.AttributeI("enumOccupatation").sValue;
                        Info.Add("sOccupation_" + sOccupation);
                        result = PolicyCal.GetOccupationsLoadFactor(sOccupation, Age);
                        if (result.xiStatus == 00 && result.oCollectionResult.Count > 0)
                        {
                            OccupationResult.Add(result);
                        }
                        sSecondaryOccupation = item.AttributeI("sSecondaryOccupation").sValue;
                        if (!string.IsNullOrEmpty(sSecondaryOccupation))
                        {
                            Info.Add("Secondary Occupation " + sSecondaryOccupation);
                            result = PolicyCal.GetOccupationsLoadFactor(sSecondaryOccupation, Age);
                            if (result.xiStatus == 00 && result.oCollectionResult.Count > 0)
                            {
                                OccupationResult.Add(result);
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
                        var sSubList = item.oStructureI("Claim_T");
                        NoOfClaims = sSubList.Count();
                        result = PolicyCal.GetClaimLoadFactor(NoOfClaims, sSubList, dtInsuranceCoverStartDate);
                        if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                        {
                            ClaimResult.Add(result);
                        }
                        var ConvinctionsList = item.oStructureI("Conviction_T");
                        NoOfConvinctions = ConvinctionsList.Count();
                        if (NoOfConvinctions > 0)
                        {
                            result = PolicyCal.GetConvictionsLoadFactor(NoOfConvinctions, ConvinctionsList, dtInsuranceCoverStartDate);
                            if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                            {
                                ConvictionResult.Add(result);
                            }
                        }
                        var MedicalConditionList = item.oStructureI("MedicalCondition_T");
                        var NoOfMedicalConditions = MedicalConditionList.Count();
                        if (NoOfMedicalConditions > 0)
                        {
                            foreach (var oMedicalCondition in MedicalConditionList)
                            {
                                if (oMedicalCondition.Attributes.ContainsKey("dDiagnosed") && !string.IsNullOrEmpty(oMedicalCondition.Attributes["dDiagnosed"].sValue))
                                {
                                    DateTime dDAte = Convert.ToDateTime(oMedicalCondition.Attributes["dDiagnosed"].sValue);
                                    var dtSixMonths = DateTime.Now.AddMonths(-6).Date;
                                    if (dDAte > dtSixMonths)// 11/27/2018 > 10/28/2018
                                    {
                                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Medical Condition", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                                    }
                                }
                            }
                        }
                        var dPassed = item.AttributeI("dDateTestPassed").sResolvedValue;
                        var ores = PolicyCal.GetAgeFromDOB(dPassed, DateTime.Now.ToString());
                        if (ores.xiStatus == 0 && ores.oResult != null)
                        {
                            int dpassedyr = (int)ores.oResult;
                            if (dpassedyr < 3)
                            {
                                oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Driving licence held years", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                            }
                        }
                        XIIBO oBOI = new XIIBO();
                        oBOI.iBODID = item.iBODID;
                        item.Attributes["sDrivingQualification"].BOI = oBOI;
                        string sDrivingQualifiication = item.AttributeI("sDrivingQualification").ResolveFK("display1");
                        Info.Add("sDrivingQualifiication" + sDrivingQualifiication);
                        if (sDrivingQualifiication.TrimEnd().ToLower() != "full uk")
                        {
                            oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Driving licence", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                        }
                        int dUkResyr = 0;
                        var bSinceBirth = item.AttributeI("bSinceBirth").sValue;
                        Info.Add("bSinceBirth" + bSinceBirth);
                        if (!string.IsNullOrEmpty(bSinceBirth) && bSinceBirth.ToLower() == "true")
                        {
                            dUkResyr = Age;
                            Info.Add("bSinceBirth_UkresYear" + dUkResyr);
                        }
                        else
                        {
                            var UKResidentYear = item.AttributeI("iUKResidentYear").sValue;
                            Info.Add("iUKResidentYear" + UKResidentYear);
                            var UKResidentMonth = item.AttributeI("iUKResidentMonth").sValue;
                            Info.Add("iUKResidentMonth" + UKResidentMonth);
                            int iUKResidentYear = 0; int iUKResidentMonth = 0;
                            if (int.TryParse(UKResidentYear, out iUKResidentYear))
                            {
                                if (int.TryParse(UKResidentMonth, out iUKResidentMonth))
                                {
                                }
                                dUkResyr = (DateTime.Now.Year - iUKResidentYear);
                                if (DateTime.Now.Month < iUKResidentMonth) dUkResyr--;
                                Info.Add("UkresMonth&Year_UkresYear" + dUkResyr);
                            }
                        }
                        if (dUkResyr < 3)
                        {
                            oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "UK Residency", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                        }
                    }
                    if (iNoOfDrivers > 4)
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Exceeded maximum no of drivers", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }

                    string sPostCode = ostructureInstance.XIIValue("sPostCode").sValue;
                    string sParkingPostCode = ostructureInstance.XIIValue("sParkingPostCode").sValue;
                    string sEnigine = ostructureInstance.XIIValue("sEngineSize").sValue;
                    var oBaseLoadFactor = new CResult();
                    string TypeOfCover = ostructureInstance.XIIValue("sTypeofCoverUpdated").sValue;
                    Info.Add("sEngineSize:" + sEnigine);
                    Info.Add("sPostCode:" + sPostCode);
                    Info.Add("sParkingPostCode:" + sParkingPostCode);
                    Info.Add("sDataBase:" + sDataBase);
                    if (sEnigine != null)
                    {
                        string oPostCodLoadFactor = PolicyBaseCalc.GetPostCodeLookUp(sPostCode, Convert.ToInt32(sEnigine), sDataBase, sProductName, sVersion, sProductCode);

                        var sPCLookup = oPostCodLoadFactor.Length;
                        if (sPCLookup == 2)
                        {
                            char sCC = oPostCodLoadFactor[1];
                            if (sCC == '0')
                            {
                                oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Engine CC is more than 3000", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                            }
                        }

                        result = PolicyCal.GetBaseRate(oPostCodLoadFactor, TypeOfCover);
                        if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                        {
                            oBaseLoadFactor = result;
                        }
                    }
                    string sYearOfManufacture = ostructureInstance.XIIValue("iYearOfManufactureUpdated").sValue;
                    int iYearOfManufacture = 0;
                    if (int.TryParse(sYearOfManufacture, out iYearOfManufacture))
                    { }
                    string bISLeftHandDrive = ostructureInstance.XIIValue("bISLeftHandDrive").sValue;

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

                    string sTypeOfDriving = ostructureInstance.XIIValue("sTypeOfDrivingupdated").sValue;
                    string iMileage = ostructureInstance.XIIValue("iMileage").sValue;
                    string bIsClubMember = ostructureInstance.XIIValue("iIsClubMember").sValue;
                    string sCaravanNoClaimsDiscount = ostructureInstance.XIIValue("iNoClaimsYears").sValue;
                    string sLiketoProtectYourNoClaimsDiscountUpdated = ostructureInstance.XIIValue("sLiketoProtectYourNoClaimsDiscountUpdated").sResolvedValue;
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
                    string sGaraging = ostructureInstance.XIIValue("sWheredoyoustoreUpdated").sDerivedValue;
                    var oExcessVal = new CResult();
                    double rCompulsaryExcess = 250.00;
                    double rExcessVal = 0;
                    string sExcessVal = ostructureInstance.XIIValue("iCurrentCaravanValue").sValue;
                    string sTypeofDevice = ostructureInstance.XIIValue("iTypeofdevice").ResolveFK("display");
                    if (double.TryParse(sExcessVal, out rExcessVal))
                    {
                    }
                    result = PolicyCal.GetExcessValue(rExcessVal, TypeOfCover, sTypeofDevice);
                    if (result.xiStatus == 0 && result.oCollectionResult != null)
                    {
                        oExcessVal = result;
                        //if (double.TryParse(oExcessVal.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), out rCompulsaryExcess)) { }
                    }
                    //General Declines
                    //string sHowManyOtherVehicles = string.Empty;
                    //if (ostructureInstance.XIIValues.ContainsKey("vehiclesareavailableinthehousehold"))
                    //{
                    //    sHowManyOtherVehicles = ostructureInstance.XIIValue("vehiclesareavailableinthehousehold").sResolvedValue;
                    // }
                    // if (!string.IsNullOrEmpty(sHowManyOtherVehicles))
                    // {
                    //    int iOtherVehicles = 0;
                    //    int.TryParse(sHowManyOtherVehicles, out iOtherVehicles);
                    //   if (iOtherVehicles == 0)
                    //   {
                    //        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Other vehicles are 0", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    //   }
                    // }
                    string sLocationOfMotorhomeOvernight = ostructureInstance.XIIValue("iWhatisthelocationofyourmotorhomeovernight").sResolvedValue;

                    //if (!string.IsNullOrEmpty(sLocationOfMotorhomeOvernight) && sLocationOfMotorhomeOvernight.ToLower() != "home address" && rExcessVal > 20000)
                    //{
                    //    oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "location of your motorhome overnight", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    //}
                    string sMotorOffence = ostructureInstance.XIIValue("sMotorOffence").sResolvedValue;
                    string sInsurancePolicyDetails = ostructureInstance.XIIValue("sPreviousMotorOrCaravanInsurancePolicyDetails").sResolvedValue;
                    if (sInsurancePolicyDetails == "Yes")
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Refused or canceled insurance", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    if (sMotorOffence == "Yes")
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "non-motoring conviction", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    var oVehicleValueLoadFactor = PolicyCal.GetVehicleValueLoadFactor(rExcessVal);
                    if (oVehicleValueLoadFactor.xiStatus == 0 && oVehicleValueLoadFactor.oCollectionResult != null)
                    {
                        oXiResults.Add(oVehicleValueLoadFactor);
                    }
                    string sTypeOfCaravan = ostructureInstance.XIIValue("sTypeOfCaravanUpdated").sResolvedValue;
                    var oUseageLoadFactor = PolicyCal.GetUseLoadFactor(sTypeOfDriving);
                    if (oUseageLoadFactor.xiStatus == 0 && oUseageLoadFactor.oCollectionResult != null)
                    {
                        oXiResults.Add(oUseageLoadFactor);
                    }
                    if (ConvictionResult.Count > 0)
                    {
                        var oConvictionMaxLoadFactor = PolicyCal.GetMaxLoadFactor(ConvictionResult);
                        if (oConvictionMaxLoadFactor.xiStatus == 0 && oConvictionMaxLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oConvictionMaxLoadFactor);
                        }
                    }
                    //if (ClaimResult.Count > 0)
                    //{
                    //    var oClaimsMaxLoadFactor = PolicyCal.GetMaxLoadFactor(ClaimResult);

                    if (ClaimResult.Count > 0)
                    {
                        var oClaimMaxLoadFactor = PolicyCal.GetMaxLoadFactor(ClaimResult);
                        if (oClaimMaxLoadFactor.xiStatus == 0 && oClaimMaxLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oClaimMaxLoadFactor);
                        }
                    }
                    var oDrivingRestrictionsLoad = PolicyCal.GetDrivingDiscount(oRelation, YAgesResult.Count());
                    if (oDrivingRestrictionsLoad.xiStatus == 00 && oDrivingRestrictionsLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oDrivingRestrictionsLoad);
                    }
                    if (AgesResult.Count > 0)
                    {
                        var oAgeLoadFactor = (PolicyCal.GetMaxLoadFactor(AgesResult));
                        if (oAgeLoadFactor.xiStatus == 0 && oAgeLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oAgeLoadFactor);
                        }
                    }
                    if (YAgesResult != null && YAgesResult.Count() > 0)
                    {
                        var oYAgeLoadFactor = (PolicyCal.GetMaxLoadFactor(YAgesResult));
                        if (oYAgeLoadFactor.xiStatus == 0 && oYAgeLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oYAgeLoadFactor);
                        }
                    }

                    if (iMileage != null)
                    {
                        var oMileageLoadFactor = PolicyCal.GetMileageLoadFactor(Convert.ToInt32(iMileage));
                        if (oMileageLoadFactor.xiStatus == 0 && oMileageLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oMileageLoadFactor);
                        }
                    }
                    var oClubMemberLoadFactor = PolicyCal.GetClubMemberDiscount(bIsClubMember);
                    if (oClubMemberLoadFactor.xiStatus == 0 && oClubMemberLoadFactor.oCollectionResult != null)
                    {
                        oXiResults.Add(oClubMemberLoadFactor);
                    }

                    if (sYearOfManufacture != null)
                    {
                        var oAgeOfVechileDiscountLoadFactor = PolicyCal.GetAgeOfVechicleDiscount(iYearOfManufacture);
                        if (oAgeOfVechileDiscountLoadFactor.xiStatus == 0 && oAgeOfVechileDiscountLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oAgeOfVechileDiscountLoadFactor);
                        }
                    }
                    //OnRoadLoad LoadFactor is missing
                    var oOnRoadLoadLoadFactor = PolicyCal.GetOnRoadLoadLoadFactor(sParkingPostCode, sDataBase, sProductName, sVersion, sProductCode, rExcessVal, sGaraging, sLocationOfMotorhomeOvernight);
                    if (oOnRoadLoadLoadFactor.xiStatus == 0 && oOnRoadLoadLoadFactor.oCollectionResult != null)
                    {
                        oXiResults.Add(oOnRoadLoadLoadFactor);
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
                        if ((DateTime.Now < Convert.ToDateTime(sDateofpurchase).AddMonths(iYearOfOwnershipInMonths)))
                            iYearOfOwnershipInMonths--;
                    }
                    Info.Add("iYearOfOwnership:" + iYearOfOwnership);
                    Info.Add("iYearOfOwnershipInMonths:" + iYearOfOwnershipInMonths);
                    var oOwnerShipLoad = PolicyCal.GetOwnershipDiscount(iYearOfOwnershipInMonths);
                    if (oOwnerShipLoad.xiStatus == 0 && oOwnerShipLoad.oCollectionResult.Count > 0)
                    {
                        oXiResults.Add(oOwnerShipLoad);
                    }
                    CResult oOccupationsLoad = new CResult();
                    if (OccupationResult.Count > 0)
                    {
                        var oOccupations = PolicyCal.GetMaxLoadFactor(OccupationResult);
                        if (oOccupations.xiStatus == 0 && oOccupations.oCollectionResult.Count > 0)
                        {
                            oXiResults.Add(oOccupations);
                        }
                    }
                    int iCaravanNoClaimsDiscount = 0;
                    if (sCaravanNoClaimsDiscount != null)
                    {

                        if (int.TryParse(sCaravanNoClaimsDiscount, out iCaravanNoClaimsDiscount))
                        {
                        }
                        var oNoClaim = PolicyCal.GetNoClaimsDiscount(iCaravanNoClaimsDiscount);
                        if (oNoClaim.xiStatus == 0 && oNoClaim.oCollectionResult != null)
                        {
                            oXiResults.Add(oNoClaim);
                        }
                    }
                    if (sLiketoProtectYourNoClaimsDiscountUpdated != null)
                    {
                        var oProtectedNoClaimDiscountLoadFactor = PolicyCal.GetProtectedNoClaimsDiscount(sLiketoProtectYourNoClaimsDiscountUpdated, iCaravanNoClaimsDiscount);
                        if (oProtectedNoClaimDiscountLoadFactor.xiStatus == 0 && oProtectedNoClaimDiscountLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oProtectedNoClaimDiscountLoadFactor);
                        }
                    }
                    //var LeftHandLoadFactor = new CResult();
                    var oLeftHandDriveLoadFactor = PolicyCal.GetLeftHandDriveLoadFactor(bISLeftHandDrive, iYearOfOwnership);
                    if (oLeftHandDriveLoadFactor.xiStatus == 0 && oLeftHandDriveLoadFactor.oCollectionResult != null)
                    {
                        oXiResults.Add(oLeftHandDriveLoadFactor);
                    }
                    if (!string.IsNullOrEmpty(sTypeOfCaravan) && (sTypeOfCaravan.ToLower() == "other"))// || sTypeOfCaravan.ToLower() == "campervan")
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Type of Motorhome", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }
                    List<string> ModificationsList = new List<string>();
                    if (!string.IsNullOrEmpty(ostructureInstance.XIIValue("StandardEnginesReplacement").sValue))
                    {
                        ModificationsList.Add(ostructureInstance.XIIValue("StandardEnginesReplacement").sValue);
                    }
                    if (!string.IsNullOrEmpty(ostructureInstance.XIIValue("Exhaust").sValue))
                    {
                        ModificationsList.Add(ostructureInstance.XIIValue("Exhaust").sValue);
                    }
                    if (!string.IsNullOrEmpty(ostructureInstance.XIIValue("SuspensionUpgraded").sValue))
                    {
                        ModificationsList.Add(ostructureInstance.XIIValue("SuspensionUpgraded").sValue);
                    }
                    if (!string.IsNullOrEmpty(ostructureInstance.XIIValue("LPGConversion").sValue))
                    {
                        ModificationsList.Add(ostructureInstance.XIIValue("LPGConversion").sValue);
                    }
                    if (!string.IsNullOrEmpty(ostructureInstance.XIIValue("Other").sValue))
                    {
                        ModificationsList.Add(ostructureInstance.XIIValue("Other").sValue);
                    }
                    var ModificationLoad = new CResult();
                    var oModifications = PolicyCal.Modifications(ModificationsList);
                    if (oModifications.xiStatus == 0 && oModifications.oCollectionResult != null)
                    {
                        ModificationLoad = oModifications;
                        var sModificationstatus = ModificationLoad.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault();
                        if (!string.IsNullOrEmpty(sModificationstatus) && sModificationstatus == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())
                        {
                            oGeneralDeclines.Add(ModificationLoad);
                        }
                    }

                    //Garaging
                    if (!string.IsNullOrEmpty(sGaraging) && sGaraging.ToLower() == "not available")
                    {
                        oGeneralDeclines.Add(PolicyCal.BuildCResultObject("0.0", "Garaging", xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString()));
                    }



                    if (oGeneralDeclines.Count > 0)
                    {
                        Info.Add("oGeneralDeclines: Declined" + oGeneralDeclines.Count);
                        foreach (var declineCase in oGeneralDeclines)
                        {
                            oXiResults.Add(declineCase);
                        }
                    }
                    var IPTLoadFactor = new CResult();
                    var oIPTLoadFactor = PolicyCal.GetIPTLoadFactor();
                    if (oIPTLoadFactor.xiStatus == 0 && oIPTLoadFactor.oCollectionResult != null)
                    {
                        IPTLoadFactor = oIPTLoadFactor;
                    }
                    Info.Add("RiskFactorsCount:" + oXiResults.Count);
                    foreach (var item in oXiResults)
                    {
                        string sMessage = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        Info.Add(sMessage);
                    }
                    double total = 0.00;
                    double BaseLoad = 0.00;
                    double rVoluntaryExcess = 0.00;
                    double rTotalExcess = rCompulsaryExcess + rVoluntaryExcess;

                    if (oBaseLoadFactor.oCollectionResult != null)
                    {
                        BaseLoad = Convert.ToDouble(oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                    }
                    if (oExcessVal.oCollectionResult != null)
                    {
                        //if (double.TryParse((oExcessVal.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()), out rCompulsaryExcess)) { }
                        BaseLoad = BaseLoad + Convert.ToDouble(oExcessVal.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                    }
                    if (oXiResults != null && oXiResults.Count() > 0)
                    {
                        int i = 0;
                        foreach (var item in oXiResults)
                        {
                            if (item.oCollectionResult != null)
                            {
                                BaseLoad += ((Convert.ToDouble(item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * BaseLoad);
                            }
                            i++;
                        }
                    }
                    BaseLoad = Math.Round(BaseLoad, 2);
                    total = BaseLoad;
                    result = PolicyCal.BuildCResultObject(total.ToString(), "Net Premium", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString());
                    oXiResults.Add(result);
                    var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
                    int iProductID = 0;
                    string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                    if (int.TryParse(ProductID, out iProductID)) { }
                    var oProductI = oIXI.BOI("Product", ProductID);
                    oProductI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Product");
                    oProductI.Attributes["iDefaultAmountType"].BOD = oProductI.BOD;
                    var oMinimumPremium = PolicyCal.GetMinimumPremium(TypeOfCover, iYearOfManufacture);
                    if (oMinimumPremium.bOK && oMinimumPremium.oCollectionResult != null)
                    {
                        string sMinPremium = oMinimumPremium.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        double rMinPremium = 0.0;
                        if (double.TryParse(sMinPremium, out rMinPremium))
                        {
                            if (rMinPremium > total)
                            {
                                total = rMinPremium;
                                oXiResults.Add(oMinimumPremium);
                            }
                        }
                        //oXiResults.Add(oMinimumPremium);
                    }
                    double rAdditionalLoad = 0;
                    if (oProductVersionI.Attributes.ContainsKey("rAdditionalLoad"))
                    {
                        var AdditionLoad = oProductVersionI.Attributes["rAdditionalLoad"].sValue;
                        if (double.TryParse(AdditionLoad, out rAdditionalLoad))
                        {
                        }
                    }

                    //oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = rCompulsaryExcess.ToString(), bDirty = true };
                    if (oXiResults.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())) && oExcessVal.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && IPTLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && oGeneralDeclines.Count <= 0)
                    {
                        Info.Add("All Load Factors are normal");
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "0", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "false", bDirty = true };
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())) || oExcessVal.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString() || IPTLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString() || oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString() || oGeneralDeclines.Count > 0)
                    {
                        Info.Add("Some Load Factors are Declined");
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "20", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "true", bDirty = true };
                        oXiResults.Add(PolicyCal.BuildCResultObject(rAdditionalLoad.ToString(), "Additional Load", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                        total += (total * 0.01) * rAdditionalLoad;
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())) || oExcessVal.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString() || IPTLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString() || oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString() || oGeneralDeclines.Count <= 0)
                    {
                        Info.Add("Some Load Factors are Refered");
                        oBII.Attributes["iQuoteStatus"] = new XIIAttribute { sName = "iQuoteStatus", sValue = "10", bDirty = true };
                        oBII.Attributes["bIsOverRide"] = new XIIAttribute { sName = "bIsOverRide", sValue = "true", bDirty = true };
                        oXiResults.Add(PolicyCal.BuildCResultObject(rAdditionalLoad.ToString(), "Additional Load", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString()));
                        total += (total * 0.01) * rAdditionalLoad;
                    }

                    double IPT = 0;
                    double rInterestRate = 0;
                    //result = PolicyCal.BuildCResultObject(rMinPremium.ToString(), "Net minimum Premium", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString());
                    if (IPTLoadFactor.oCollectionResult != null)
                    {
                        rInterestRate = Convert.ToDouble(IPTLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                        IPT = ((Convert.ToDouble(IPTLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * total);
                        IPT = Math.Round(IPT, 2);
                        result = PolicyCal.BuildCResultObject(IPT.ToString(), "Net IPT", xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString());
                        oXiResults.Add(result);
                        total += IPT;
                    }
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
                    oBII.Attributes["sExcess"] = new XIIAttribute { sName = "sExcess", sValue = ExcessContent, bDirty = true };
                    var sQuoteGUID = Guid.NewGuid().ToString("N").Substring(0, 10);
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
                    oBII.Attributes["sInsurer"] = new XIIAttribute { sName = "sInsurer", sValue = "KGM", bDirty = true };
                    oBII.Attributes["FKiCustomerID"] = new XIIAttribute { sName = "FKiCustomerID", sValue = iCustomerID.ToString(), bDirty = true };
                    oBII.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                    oBII.Attributes["dtqsupdateddate"] = new XIIAttribute { sName = "dtqsupdateddate", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBII.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBII.Attributes["FKiProductVersionID"] = new XIIAttribute { sName = "FKiProductVersionID", sValue = sVersion, bDirty = true };
                    oBII.Attributes["sGUID"] = new XIIAttribute { sName = "sGUID", sValue = sQuoteGUID, bDirty = true };

                    //Random generator = new Random();
                    //string sRef = generator.Next(1, 10000000).ToString(new String('0', 7));
                    //oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + sRef, bDirty = true };
                    //oBII.Attributes["sRefID"] = new XIIAttribute { sName = "sRefID", sValue = sPrefix + Guid.NewGuid().ToString("N").Substring(0, 6), bDirty = true };
                    oBII.Attributes["BatchID"] = new XIIAttribute { sName = "BatchID", sValue = iBatchID, bDirty = true };
                    oBII.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = iQuoteID.ToString(), bDirty = true };
                    oBII.Attributes["rCompulsoryExcess"] = new XIIAttribute { sName = "rCompulsoryExcess", sValue = String.Format("{0:0.00}", rCompulsaryExcess), bDirty = true };
                    oBII.Attributes["rVoluntaryExcess"] = new XIIAttribute { sName = "rVoluntaryExcess", sValue = String.Format("{0:0.00}", rVoluntaryExcess), bDirty = true };
                    oBII.Attributes["rTotalExcess"] = new XIIAttribute { sName = "rTotalExcess", sValue = String.Format("{0:0.00}", rTotalExcess), bDirty = true };

                    //Info.Add("QuoteRefID_" + oBII.Attributes["sRefID"].sValue);
                    var oRes = oBII.Save(oBII);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        oBII = (XIIBO)oRes.oResult;
                    }
                    Info.Add("KGM FEB Quote inserted Sucessfully with the amount of " + total);
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
                    oBO = new XIIBO();
                    oBO.BOD = oRiskFactorsBOD;
                    oBO.Attributes["FKiQuoteID"] = new XIIAttribute { sName = "FKiQuoteID", sValue = oBII.Attributes["ID"].sValue, bDirty = true };
                    oBO.Attributes["sFactorName"] = new XIIAttribute { sName = "sFactorName", sValue = oExcessVal.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    oBO.Attributes["sValue"] = new XIIAttribute { sName = "sValue", sValue = oExcessVal.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    oBO.Attributes["sMessage"] = new XIIAttribute { sName = "sMessage", sValue = oExcessVal.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    oBO.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBO.Attributes["FKsQuoteID"] = new XIIAttribute { sName = "FKsQuoteID", sValue = sQuoteGUID, bDirty = true };
                    oBO.Attributes["ID"] = new XIIAttribute { sName = "ID", bDirty = true };
                    oBOIList.Add(oBO);
                    oBO = new XIIBO();
                    oBO.BOD = oRiskFactorsBOD;
                    oXiResults.Add(IPTLoadFactor);
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
                    oResult.sCode = "Info";
                    string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                    oResult.sMessage = sInfo;
                    oIB.SaveErrortoDB(oResult);
                }
                catch (Exception ex)
                {
                    oResult.sCode = "Error";
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
        public class PolicyCal : PolicyBaseCalc
        {
            public override double GetNetAmount(string TypeOfCover, double Amount)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                double NetAmount = Amount;
                try
                {
                    //oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    //List<CNV> oNVsList = new List<CNV>();
                    //oresult.oResult = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    //CNV oNV = new CNV();
                    //oNV.sName = "sMessage";
                    //oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    //CNV oNV1 = new CNV();
                    //oNV1.sName = "oResult";
                    //oNV1.sValue = "+0";
                    if (TypeOfCover.ToLower() == "comp" && Amount < 170)
                    {
                        NetAmount = 170;
                    }
                    else if (TypeOfCover.ToLower() == "TPFT" && Amount < 165)
                    {
                        NetAmount = 165;
                    }
                    //oNVsList.Add(oNV);
                    //oNVsList.Add(oNV1);
                    //oresult.oCollectionResult = oNVsList;
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return NetAmount;
            }
            public override CResult GetBaseRate(string AreaGroup, string comp)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "+0";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "Base Rate";
                    switch (AreaGroup)
                    {
                        case "A1":
                            oNV1.sValue = comp.ToLower() == "comp" ? "362" : "294";
                            break;
                        case "A2":
                            oNV1.sValue = comp.ToLower() == "comp" ? "407" : "329";
                            break;
                        case "A3":
                            oNV1.sValue = comp.ToLower() == "comp" ? "455" : "369";
                            break;
                        case "A4":
                            oNV1.sValue = comp.ToLower() == "comp" ? "478" : "414";
                            break;
                        case "B1":
                            oNV1.sValue = comp.ToLower() == "comp" ? "413" : "335";
                            break;
                        case "B2":
                            oNV1.sValue = comp.ToLower() == "comp" ? "465" : "376";
                            break;
                        case "B3":
                            oNV1.sValue = comp.ToLower() == "comp" ? "519" : "420";
                            break;
                        case "B4":
                            oNV1.sValue = comp.ToLower() == "comp" ? "545" : "471";
                            break;
                        case "C1":
                            oNV1.sValue = comp.ToLower() == "comp" ? "471" : "382";
                            break;
                        case "C2":
                            oNV1.sValue = comp.ToLower() == "comp" ? "528" : "427";
                            break;
                        case "C3":
                            oNV1.sValue = comp.ToLower() == "comp" ? "592" : "478";
                            break;
                        case "C4":
                            oNV1.sValue = comp.ToLower() == "comp" ? "662" : "535";
                            break;
                        case "Decline":
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                            break;
                        default:
                            oNV1.sValue = "+0.00";
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                            break;
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
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
        }
    }
}