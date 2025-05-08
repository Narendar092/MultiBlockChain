using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using XICore;
using XISystem;
using XIInfrastructure;

namespace ZeeInsurance
{
    public class KGM
    {
        public static CResult PolicyMainCal(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            PolicyCalculation Pcal = new PolicyCalculation();
            //XIIQS oQsInstance = new XIIQS();
            CResult oResult = new CResult();
            oResult.sMessage = "KGM script running";
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
                //XIInfraCache XIInfraCache = new XIInfraCache();
                //oQsInstance = XIInfraCache.Get_QuestionSetCache("QuestionSetCache", sUID, iInsatnceID);
                //oResult.sMessage = "In Script: Questionset object Loaded from cache";
                //oIB.SaveErrortoDB(oResult);
                oResult = Pcal.Calculation(iInsatnceID, iUserID, iCustomerID, sDataBase, sProductName, sVersion, sSessionID, sUID);
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
                int res = 0;
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

                return res;
            }
            public static string GetPostCodeLookUp(string PostCode, int EngineSize, string sDataBase, string sProductName, string sVersion)
            {
                string res = "";
                XIPostCodeLookUp oPostCodeLookUp = new XIPostCodeLookUp();
                oPostCodeLookUp.sMPostCode = PostCode;
                oPostCodeLookUp.sProductName = sProductName;
                oPostCodeLookUp.sVersion = sVersion;
                var PostCodeGroup = oPostCodeLookUp.Get_PostCode(sDataBase);
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
                return res;
            }
            public CResult GetYoungAgeLoadFactor(int Age)
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
                    oNV2.sValue = "Young Additional Driver Load";
                    if (Age >= 25 && Age <= 29)//AdditionalDriver only
                    {
                        oNV1.sValue = "+30";
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

            public CResult GetAgeLoadFactor(int Age)
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
                    oNV2.sValue = "Age";

                    if (Age > 0)
                    {
                        //if (Age >= 25 && Age <= 29)//AdditionalDriver only
                        //{
                        //    oNV1.sValue = "+30";
                        //}
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
                        else if (Age >= 70)
                        {
                            oNV1.sValue = "-10";
                        }
                        else if (Age < 25)
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
                    oresult.LogToFile();
                }
                return oresult;
            }
            public abstract double GetNetAmount(string TypeOfCover, double Amount);
            public abstract CResult GetBaseRate(string AreaGroup, string comp);
            public CResult GetUseLoadFactor(string sUse)
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
                    oNV2.sValue = "Usage";
                    if (sUse == "SDPC")
                    {
                        oNV1.sValue = "+20";
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
            public CResult GetMileageLoadFactor(int miles)
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
                    oNV2.sValue = "Mileage";
                    //oNVsList.Add(oNV2);
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
                    oresult.LogToFile();
                }
                return oresult;

            }
            public CResult GetClubMemberDiscount(string IsClubMember)
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
                    oNV2.sValue = "ClubMember";
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
                    oresult.LogToFile();
                }
                return oresult;
            }
            public CResult GetAgeOfVechicleDiscount(int YearofManufacture)
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
                    oNV2.sValue = "AgeOfVechicle";
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
                    oresult.LogToFile();
                }
                return oresult;
            }
            public CResult GetOwnershipDiscount(int YearOfOwnerShip)
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
                    oNV2.sValue = "OwnershipDiscount";

                    if (YearOfOwnerShip == 1)
                    {
                        oNV1.sValue = "-5";
                    }
                    else if (YearOfOwnerShip == 2)
                    {
                        oNV1.sValue = "-10";
                    }
                    else if (YearOfOwnerShip == 3)
                    {
                        oNV1.sValue = "-15";
                    }
                    else if (YearOfOwnerShip == 4)
                    {
                        oNV1.sValue = "-20";
                    }
                    else if (YearOfOwnerShip >= 5)
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
                    oresult.LogToFile();
                }
                return oresult;
            }
            public CResult GetLeftHandDriveLoadFactor(string IsLeftHandDrive)
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
                    oNV2.sValue = "LeftHandDrive";
                    if (IsLeftHandDrive.ToLower() == "10")
                    {
                        oNV1.sValue = "+10";
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
            public CResult GetNoClaimsDiscount(int Year)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "NoClaimsDiscount";
                    if (Year >= 4)
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
                    oresult.LogToFile();
                }
                return oresult;

            }
            public CResult GetProtectedNoClaimsDiscount(int Year)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "ProtectedNoClaimsDiscount";
                    if (Year == 1)
                    {
                        oNV1.sValue = "+2.5";
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
            public CResult GetDrivingDiscount(string type)
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
                    oNV1.sValue = "-0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "DrivingDiscount";
                    type = type.ToLower();
                    if (type == "insured only")
                    {
                        oNV1.sValue = "-15";
                    }
                    else if (type == "insured and spouse")
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
                    oresult.LogToFile();
                }
                return oresult;
            }

            public CResult GetClaimLoadFactor(int NoOfClaims, string ClaimCost)
            {
                CResult oresult = new CResult();
                try
                {
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = "+0";
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = "ClaimLoadFactor";
                    if (NoOfClaims == 1 && Convert.ToInt32(ClaimCost) <= 10000)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
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
            public CResult GetConvictionsLoadFactor(int NoOfConvictions, List<string> ConvictionCodes)
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
                    oNV2.sValue = "ConvictionsLoadFactor";
                    string[] DeclineCodes = new string[] { "AC", "BA", "TT", "XX" };
                    string[] RefCodes = new string[] { "CU", "MW", "PC", "PL", "SP", "TS" };
                    string Prefix = ""; int numberPart = 0;
                    var DeclinedReferlist = new List<string>(); var NormalList = new List<int>();
                    string stringArra = string.Empty;
                    Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                    foreach (var ConvictionCode in ConvictionCodes)
                    {
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
                        if (Array.IndexOf(DeclineCodes, Prefix) > -1 || ConvictionCode.ToUpper() == "S19")
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if (Prefix == "DD" && ((numberPart >= 20 && numberPart <= 40) || (numberPart >= 60 && numberPart <= 80)))
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if (Prefix == "MS" && numberPart >= 40 && numberPart <= 90)
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;

                        }
                        else if (Prefix == "DR" && numberPart >= 10 && numberPart <= 70)
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
                        else if (Prefix == "DR" && numberPart >= 80 && numberPart <= 90)
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if (Prefix == "LC" && numberPart >= 30 && numberPart <= 50)
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if (Prefix == "UT" && numberPart >= 10 && numberPart <= 50)
                        {
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString());
                            break;
                        }
                        else if (Prefix == "CD" && numberPart >= 10 && numberPart <= 30)
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
                        else if (ConvictionCode == "IN10")
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
                        else if ((Array.IndexOf(RefCodes, Prefix) > -1 || ConvictionCode.ToUpper() == "LC20"))
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
                        else if (Prefix == "MS" && numberPart >= 10 && numberPart <= 30)
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
                            DeclinedReferlist.Add(xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString());
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
                    oresult.LogToFile();
                }
                return oresult;
            }
            public CResult GetOccupationsLoadFactor(string sOccupation)
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
                    oNV2.sValue = "Occupation";

                    string[] ReferArray = new string[] { "artist", "unemployed", "licensed trade", "licensee", "publican", "employee", "newspaper journalists", "photographers", "landscape gardener", "unemployed", "roofer", "scaffolder", "road worker", "ashphalter", };
                    string[] DeclineArray = new string[] { "amusement arcade owner", "employee", "models", "caravan dweller", "musician", "casino employees", "doorman", "bouncer", "night security", "entertainment industry ", "fairground", "circus employee", "horse jockey", "trainer or the like", "market trader", "nightclub owner", "dancer", "professional sports person", "scrap", "general" };

                    int pos = Array.IndexOf(DeclineArray, sOccupation);
                    if (pos > -1)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }
                    else
                    {
                        pos = Array.IndexOf(ReferArray, sOccupation);
                        if (pos > -1)
                        {
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
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
                }
                return oresult;
            }
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
            public CResult GetOnRoadLoadLoadFactor(string ParkingPostCode, string sDataBase, string sProductName, string sVersion)
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
                    oNV2.sValue = "OnRoadLoad";
                    XIPostCodeLookUp oPostCodeLookUp = new XIPostCodeLookUp();
                    oPostCodeLookUp.sMPostCode = ParkingPostCode;
                    oPostCodeLookUp.sProductName = sProductName;
                    oPostCodeLookUp.sVersion = sVersion;
                    var PostCodeGroup = oPostCodeLookUp.Get_PostCode(sDataBase);
                    var Area = "";
                    if (PostCodeGroup.xiStatus == 0 && PostCodeGroup.oResult != null)
                    {
                        Area = PostCodeGroup.oResult.ToString();
                    }
                    //var Area = GetArea(ParkingPostCode);
                    Area = Area.ToUpper();
                    oresult.sMessage = "OnRoadLoad_Area:" + Area;
                    oIB.SaveErrortoDB(oresult);
                    if (Area == "A" || Area == "B")
                    {
                        oNV1.sValue = "+30";
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
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public CResult GetExcessValue(double iCurrentCaravanValue)
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
                    oNV2.sValue = "ExcessValue";
                    int f1 = 0, f2 = 0;
                    double ExcessValue;
                    if (iCurrentCaravanValue > 8000 && iCurrentCaravanValue <= 25000)
                    {
                        f1 = Convert.ToInt32((Math.Ceiling(iCurrentCaravanValue - 8000) / 1000));
                    }
                    else if (iCurrentCaravanValue > 25000 && iCurrentCaravanValue <= 60000)
                    {
                        f1 = Convert.ToInt32(((25000 - 8000) / 1000));
                        f2 = Convert.ToInt32((Math.Ceiling((iCurrentCaravanValue - 25000) / 1000)));
                    }
                    else if (iCurrentCaravanValue > 60000)
                    {
                        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString();
                    }

                    ExcessValue = f1 * 5 + f2 * 3;
                    oNV1.sValue = "+" + ExcessValue.ToString();
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
            //public CResult GetBaseRateBasedOnVechileAge()
            //{
            //    CResult oresult = new CResult();
            //    try
            //    {
            //        oresult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            //        List<CNV> oNVsList = new List<CNV>();
            //        CNV oNV = new CNV();
            //        oNV.sName = "sMessage";
            //        oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
            //        CNV oNV1 = new CNV();
            //        oNV1.sName = "oResult";
            //        oNV1.sValue = "+0";
            //        CNV oNV2 = new CNV();
            //        oNV2.sName = "LoadFactorName";
            //        oNV2.sValue = "BaseRate";

            //        result.sQuery = "BaseRate";
            //        result.sMessage = xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString();
            //        result.oResult = "+149";
            //    }
            //    catch (Exception ex)
            //    {
            //        oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
            //        oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
            //        oresult.LogToFile();
            //    }
            //    return oresult;
            //}
            public CResult GetIPTLoadFactor()
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
                    oresult.LogToFile();
                }
                return oresult;
            }
            public CResult GetMaxVechileValuefactor(string type, int MaxValue)
            {

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
                    oresult.LogToFile();
                }
                return oresult;
            }
            public CResult GetParkingLoadFactor(string Parkingtype)
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
                    oresult.LogToFile();
                }
                return oresult;
            }
            public CResult GetMaxLoadFactor(List<CResult> oResult)
            {
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
                    result.LogToFile();
                }
                return result;
            }
            public CResult GetAgeFromDOB(string dDOB)
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
            public CResult BuildCResultObject(string Value, string Name)
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
            //public static double GetMinimumPolicyAmount()
            //{
            //    return 400.00;
            //}
        }
        public class PolicyCalculation
        {
            public CResult Calculation(int iInsatnceID, int iUserID, int iCustomerID, string sDataBase, string sProductName, string sVersion, string sSessionID, string sUID)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oResult = new CResult();
                try
                {
                    XIDXI oXIDXI = new XIDXI();
                    PolicyCal PolicyCal = new PolicyCal();
                    List<CResult> oXiResults = new List<CResult>();
                    XIIXI oIXI = new XIIXI();
                    XIIBO oBII = new XIIBO();
                    List<CResult> AgesResult = new List<CResult>();
                    List<CResult> YAgesResult = new List<CResult>();
                    int Age = 0;
                    string DOB = "";
                    oBII.BOD = (XIDBO)oXIDXI.Get_BODefinition("Aggregations").oResult;
                    List<CResult> ClaimResult = new List<CResult>();
                    int NoOfClaims = 0;
                    int NoOfConvinctions = 0;
                    var result = new CResult();
                    List<CResult> ConvictionResult = new List<CResult>();
                    //XIIQS oXIIQS = new XIIQS();
                    //var oQSI = oIXI.BOI("QS Instance", iInsatnceID.ToString()).Structure("NotationStructure").XILoad();
                    XIInfraCache oCache = new XIInfraCache();
                    var oQSI = oCache.Get_QsStructureObj(sSessionID, sUID, "QSInstance_" + iInsatnceID + "NotationStructure");
                    var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                    var oLIst = oQSI.oSubStructureI("Driver_T");
                    //var oQSII = oXIIQS.QSI(oQSI);
                    //var oLIst = oIXI.BOI("QS Instance", QSInstance.ID.ToString()).Structure("NotationStructure").oSubStruct("Driver_T");
                    var oDriversI = (List<XIIBO>)oLIst.oBOIList;
                    foreach (var item in oDriversI)
                    {
                        List<string> ConvictionCodes = new List<string>();
                        long rTotalCost = 0;
                        item.oBOStructure = item.SubChildI;
                        DOB = item.AttributeI("dDOB").sValue;
                        result = PolicyCal.GetAgeFromDOB(DOB);
                        if (result.xiStatus == 00 && result.oResult != null)
                        {
                            Age = (int)result.oResult;
                            result = PolicyCal.GetAgeLoadFactor(Age);
                            if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                            {
                                AgesResult.Add(result);
                            }
                            result = PolicyCal.GetYoungAgeLoadFactor(Age);
                            if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                            {
                                YAgesResult.Add(result);
                            }
                        }
                        var sSubList = item.oStructureI("Claim_T");
                        NoOfClaims = sSubList.Count();
                        foreach (var oClaim in sSubList)
                        {
                            if (oClaim.AttributeI("rTotalClaimCost").sValue != null)
                            {
                                long ClaimCostI = 0;
                                if (long.TryParse(oClaim.AttributeI("rTotalClaimCost").sValue, out ClaimCostI))
                                {
                                    rTotalCost = rTotalCost + ClaimCostI;
                                }
                            }
                        }
                        if (NoOfClaims > 0)
                        {
                            result = PolicyCal.GetClaimLoadFactor(NoOfClaims, rTotalCost.ToString());
                            if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                            {
                                ClaimResult.Add(result);
                            }
                        }
                        var ConvinctionsList = item.oStructureI("Conviction_T");
                        NoOfConvinctions = ConvinctionsList.Count();
                        foreach (var oConvinction in ConvinctionsList)
                        {
                            var oConvictionI = oIXI.BOI("refConviction_T", oConvinction.AttributeI("refconviction").sValue);
                            oResult.sMessage = "ConvictionCode :" + oConvictionI.Attributes["scode"].sValue;
                            oIB.SaveErrortoDB(oResult);
                            ConvictionCodes.Add(oConvictionI.Attributes["scode"].sValue);
                            //ConvictionCodes.Add(oConvinction.AttributeI("Conviction Code").sValue);
                            // ConvictionCodes.Add(oConvinction.AttributeI("sCode").sValue);
                        }
                        if (NoOfConvinctions > 0)
                        {
                            result = PolicyCal.GetConvictionsLoadFactor(NoOfConvinctions, ConvictionCodes);
                            if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                            {
                                ConvictionResult.Add(result);
                            }
                        }
                    }
                    DOB = ostructureInstance.XIIValue("dDOB").sValue;
                    //DOB = ostructureInstance.XIIValue("dtDateofbirth").sValue;
                    result = PolicyCal.GetAgeFromDOB(DOB);
                    if (result.xiStatus == 0 && result.oResult != null)
                    {
                        Age = (int)result.oResult;
                        result = PolicyCal.GetAgeLoadFactor(Age);
                        if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                        {
                            AgesResult.Add(result);
                        }
                        result = PolicyCal.GetYoungAgeLoadFactor(Age);
                        if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                        {
                            YAgesResult.Add(result);
                        }
                    }
                    string sPostCode = ostructureInstance.XIIValue("sPostCode").sValue;
                    string sParkingPostCode = ostructureInstance.XIIValue("sParkingPostCode").sValue;
                    string sEnigine = ostructureInstance.XIIValue("sEngineSize").sValue;
                    var oBaseLoadFactor = new CResult();
                    string TypeOfCover = ostructureInstance.XIIValue("sTypeofCoverUpdated").sValue;
                    oResult.sMessage = "sEngineSize:" + sEnigine;
                    oIB.SaveErrortoDB(oResult);
                    oResult.sMessage = "sPostCode:" + sPostCode;
                    oIB.SaveErrortoDB(oResult);
                    oResult.sMessage = "sParkingPostCode:" + sParkingPostCode;
                    oIB.SaveErrortoDB(oResult);
                    oResult.sMessage = "sDataBase:" + sDataBase;
                    oIB.SaveErrortoDB(oResult);
                    if (sEnigine != null)
                    {
                        string oPostCodLoadFactor = PolicyBaseCalc.GetPostCodeLookUp(sPostCode, Convert.ToInt32(sEnigine), sDataBase, sProductName, sVersion);
                        result = PolicyCal.GetBaseRate(oPostCodLoadFactor, TypeOfCover);
                        if (result.xiStatus == 0 && result.oCollectionResult.Count > 0)
                        {
                            oBaseLoadFactor = result;
                        }
                    }
                    string iYearOfManufacture = ostructureInstance.XIIValue("iYearOfManufactureUpdated").sValue;
                    string sOccupation = ostructureInstance.XIIValue("iOccupation").sResolvedValue;
                    string bISLeftHandDrive = ostructureInstance.XIIValue("bISLeftHandDrive").sValue;
                    string sYearOfOwnership = ostructureInstance.XIIValue("iYearOfOwnershipUpdated").sValue;
                    string sTypeOfDriving = ostructureInstance.XIIValue("sTypeOfDriving updated").sValue;
                    string iMileage = ostructureInstance.XIIValue("iMileage").sValue;
                    string bIsClubMember = ostructureInstance.XIIValue("iIsClubMember").sValue;
                    string sWhoTowing = ostructureInstance.XIIValue("sWhoTowingUpdated").sValue;
                    string sCaravanNoClaimsDiscount = ostructureInstance.XIIValue("iNoClaimsYears").sValue;
                    string sLiketoProtectYourNoClaimsDiscountUpdated = ostructureInstance.XIIValue("sLiketoProtectYourNoClaimsDiscountUpdated").sValue;
                    string dtInsuranceCoverStartDate = ostructureInstance.XIIValue("dCoverStart").sValue;

                    var oExcessVal = new CResult();
                    double rExcessVal = 0;
                    string sExcessVal = ostructureInstance.XIIValue("iCurrentCaravanValue").sValue;
                    if (double.TryParse(sExcessVal, out rExcessVal))
                    {
                    }
                    result = PolicyCal.GetExcessValue(rExcessVal);
                    if (result.xiStatus == 0 && result.oCollectionResult != null)
                    {
                        oExcessVal = result;
                    }
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
                    if (ClaimResult.Count > 0)
                    {
                        var oClaimsMaxLoadFactor = PolicyCal.GetMaxLoadFactor(ClaimResult);

                        if (oClaimsMaxLoadFactor.xiStatus == 0 && oClaimsMaxLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oClaimsMaxLoadFactor);
                        }
                    }
                    var oDrivingDiscountLoadFactor = PolicyCal.GetDrivingDiscount(sWhoTowing);
                    if (oDrivingDiscountLoadFactor.xiStatus == 0 && oDrivingDiscountLoadFactor.oCollectionResult != null)
                    {
                        oXiResults.Add(oDrivingDiscountLoadFactor);
                    }
                    var oAgeLoadFactor = (PolicyCal.GetMaxLoadFactor(AgesResult));
                    if (oAgeLoadFactor.xiStatus == 0 && oAgeLoadFactor.oCollectionResult != null)
                    {
                        oXiResults.Add(oAgeLoadFactor);
                    }
                    var oYAgeLoadFactor = (PolicyCal.GetMaxLoadFactor(YAgesResult));
                    if (oYAgeLoadFactor.xiStatus == 0 && oYAgeLoadFactor.oCollectionResult != null)
                    {
                        oXiResults.Add(oYAgeLoadFactor);
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
                    if (iYearOfManufacture != null)
                    {
                        var oAgeOfVechileDiscountLoadFactor = PolicyCal.GetAgeOfVechicleDiscount(Convert.ToInt32(iYearOfManufacture));
                        if (oAgeOfVechileDiscountLoadFactor.xiStatus == 0 && oAgeOfVechileDiscountLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oAgeOfVechileDiscountLoadFactor);
                        }
                    }
                    //OnRoadLoad LoadFactor is missing
                    var oOnRoadLoadLoadFactor = PolicyCal.GetOnRoadLoadLoadFactor(sParkingPostCode, sDataBase, sProductName, sVersion);
                    if (oOnRoadLoadLoadFactor.xiStatus == 0 && oOnRoadLoadLoadFactor.oCollectionResult != null)
                    {
                        oXiResults.Add(oOnRoadLoadLoadFactor);
                    }

                    if (sYearOfOwnership != null)
                    {
                        int iYearOfOwnership = 0;
                        if (int.TryParse(sYearOfOwnership, out iYearOfOwnership))
                        {
                        }
                        var oOwnerShipDiscountLoadFactor = PolicyCal.GetOwnershipDiscount(iYearOfOwnership);
                        if (oOwnerShipDiscountLoadFactor.xiStatus == 0 && oOwnerShipDiscountLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oOwnerShipDiscountLoadFactor);
                        }
                    }
                    var oOccupationLoadFactor = PolicyCal.GetOccupationsLoadFactor(sOccupation);
                    if (oOccupationLoadFactor.xiStatus == 0 && oOccupationLoadFactor.oCollectionResult != null)
                    {
                        oXiResults.Add(oOccupationLoadFactor);
                    }
                    if (sCaravanNoClaimsDiscount != null)
                    {
                        int iCaravanNoClaimsDiscount = 0;
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
                        int iCaravanProtectYourNoClaimsDiscountt = 0;
                        if (int.TryParse(sLiketoProtectYourNoClaimsDiscountUpdated, out iCaravanProtectYourNoClaimsDiscountt))
                        {
                        }
                        var oProtectedNoClaimDiscountLoadFactor = PolicyCal.GetProtectedNoClaimsDiscount(iCaravanProtectYourNoClaimsDiscountt);
                        if (oProtectedNoClaimDiscountLoadFactor.xiStatus == 0 && oProtectedNoClaimDiscountLoadFactor.oCollectionResult != null)
                        {
                            oXiResults.Add(oProtectedNoClaimDiscountLoadFactor);
                        }
                    }
                    var LeftHandLoadFactor = new CResult();
                    var oLeftHandDriveLoadFactor = PolicyCal.GetLeftHandDriveLoadFactor(bISLeftHandDrive);
                    if (oLeftHandDriveLoadFactor.xiStatus == 0 && oLeftHandDriveLoadFactor.oCollectionResult != null)
                    {
                        LeftHandLoadFactor = oLeftHandDriveLoadFactor;
                        // oXiResults.Add(oLeftHandDriveLoadFactor);
                    }
                    //oResult.sMessage = "dtInsuranceCoverStartDate_" + dtInsuranceCoverStartDate;
                    //oIB.SaveErrortoDB(oResult);
                    //if (dtInsuranceCoverStartDate.Contains('/'))
                    //{
                    //    var datearray = dtInsuranceCoverStartDate.Split('/');
                    //    dtInsuranceCoverStartDate = datearray[1] + "/" + datearray[0] + "/" + datearray[2];
                    //}
                    //var oCancellationScales = PolicyCal.GetCancellationScales(Convert.ToDateTime(dtInsuranceCoverStartDate));
                    //if (oCancellationScales.xiStatus == 0 && oCancellationScales.oCollectionResult != null)
                    //{
                    //    oXiResults.Add(oCancellationScales);
                    //}
                    var IPTLoadFactor = new CResult();
                    var oIPTLoadFactor = PolicyCal.GetIPTLoadFactor();
                    if (oIPTLoadFactor.xiStatus == 0 && oIPTLoadFactor.oCollectionResult != null)
                    {
                        IPTLoadFactor = oIPTLoadFactor;
                        //oXiResults.Add(oIPTLoadFactor);
                    }
                    oResult.sMessage = "RiskFactorsCount:" + oXiResults.Count();
                    oIB.SaveErrortoDB(oResult);
                    XIIBO oBOI = new XIIBO();
                    oBOI.BOD = (XIDBO)oXIDXI.Get_BODefinition("RiskFactor_T").oResult;
                    foreach (var item in oXiResults)
                    {
                        oResult.sMessage = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() + "_" + item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault();
                        oIB.SaveErrortoDB(oResult);
                    }
                    double total = 0.00;
                    double BaseLoad = 0.00;
                    if (oXiResults.All(m => m.oCollectionResult.Where(x => x.sName == "sMessage").All(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())) && oExcessVal.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString() && oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Normal.ToString())
                    {
                        oResult.sMessage = "All Load Factors are normal";
                        oIB.SaveErrortoDB(oResult);
                        if (oBaseLoadFactor.oCollectionResult != null)
                        {
                            BaseLoad = Convert.ToDouble(oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                        }
                        if (oExcessVal.oCollectionResult != null)
                        {
                            total = BaseLoad + Convert.ToDouble(oExcessVal.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault());
                        }
                        if (oXiResults != null && oXiResults.Count() > 0)
                        {
                            int i = 0;
                            foreach (var item in oXiResults)
                            {
                                if (item.oCollectionResult != null)
                                {
                                    total += ((Convert.ToDouble(item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * BaseLoad);
                                }
                                i++;
                            }
                        }
                        result = PolicyCal.BuildCResultObject(total.ToString(), "Net Premium");
                        oXiResults.Add(result);
                        var oProductVersionI = oIXI.BOI("ProductVersion_T", sVersion);
                        string ProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                        var oProductI = oIXI.BOI("Product", ProductID);
                        string sMinPremium = oProductI.Attributes["rMinPremium"].sValue;
                        double rMinPremium = 0.0;
                        if (double.TryParse(sMinPremium, out rMinPremium))
                        {
                            if (rMinPremium > total)
                            {
                                total = rMinPremium;
                            }
                        }
                        result = PolicyCal.BuildCResultObject(rMinPremium.ToString(), "Net minimum Premium");
                        oXiResults.Add(result);
                        oXiResults.Add(LeftHandLoadFactor);
                        //total = PolicyCal.GetPremium(TypeOfCover, total);
                        if (LeftHandLoadFactor.oCollectionResult != null)
                        {
                            total += ((Convert.ToDouble(LeftHandLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * BaseLoad);
                        }
                        if (IPTLoadFactor.oCollectionResult != null)
                        {
                            double IPT = ((Convert.ToDouble(IPTLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault()) * 0.01) * total);

                            result = PolicyCal.BuildCResultObject(IPT.ToString(), "Net IPT");
                            oXiResults.Add(result);

                            //string sProductMinIPT = oProductI.Attributes["rMinIPT"].sValue;
                            //double rProductMinIPT = 0.0;

                            //if (double.TryParse(sProductMinIPT, out rProductMinIPT))
                            //{
                            //    if (rProductMinIPT > IPT)
                            //    {
                            //        IPT = rProductMinIPT;
                            //    }
                            //}
                            //result = PolicyCal.BuildCResultObject(rProductMinIPT.ToString(), "minimum IPT");
                            //oXiResults.Add(result);
                            total += IPT;
                        }
                        oBII.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = total.ToString(), bDirty = true };
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())) || oExcessVal.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString())
                    {
                        oResult.sMessage = "Some Load Factors are Refered";
                        oIB.SaveErrortoDB(oResult);
                        oBII.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = "0.00", bDirty = true };
                        oXiResults.Add(LeftHandLoadFactor);
                    }
                    else if (oXiResults.Any(m => m.oCollectionResult.Where(x => x.sName == "sMessage").Any(x => x.sValue == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())) || oExcessVal.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() == xiEnumSystem.xiEnumPolicyLookupResponses.Decline.ToString())
                    {
                        oResult.sMessage = "Some Load Factors are Declined";
                        oIB.SaveErrortoDB(oResult);
                        oBII.Attributes["rPrice"] = new XIIAttribute { sName = "rPrice", sValue = "0.00", bDirty = true };
                        oXiResults.Add(LeftHandLoadFactor);
                    }
                    //var iBatchID = String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);
                    var iBatchID = iCustomerID.ToString() + iInsatnceID.ToString();
                    oBII.Attributes["FKiQSInstanceID"] = new XIIAttribute { sName = "FKiQSInstanceID", sValue = iInsatnceID.ToString(), bDirty = true };
                    oBII.Attributes["sInsurer"] = new XIIAttribute { sName = "sInsurer", sValue = "KGM", bDirty = true };
                    oBII.Attributes["FKiCustomerID"] = new XIIAttribute { sName = "FKiCustomerID", sValue = iCustomerID.ToString(), bDirty = true };
                    oBII.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                    oBII.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString("dd-MMM-yyyy"), bDirty = true };
                    oBII.Attributes["FKiProductVersionID"] = new XIIAttribute { sName = "FKiProductVersionID", sValue = sVersion, bDirty = true };
                    oBII.Attributes["BatchID"] = new XIIAttribute { sName = "BatchID", sValue = iBatchID, bDirty = true };
                    oBII.Attributes["ID"] = new XIIAttribute { sName = "ID", bDirty = true };
                    var oRes = oBII.Save(oBII);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        oBII = (XIIBO)oRes.oResult;
                    }
                    oResult.sMessage = "KGM Quote inserted Sucessfully with the amount of " + total;
                    oIB.SaveErrortoDB(oResult);
                    XIIBO oBO = new XIIBO();
                    oBO.BOD = (XIDBO)oXIDXI.Get_BODefinition("RiskFactor_T").oResult;
                    oBO.Attributes["FKiQuoteID"] = new XIIAttribute { sName = "FKiQuoteID", sValue = oBII.Attributes["ID"].sValue, bDirty = true };
                    oBO.Attributes["sFactorName"] = new XIIAttribute { sName = "sFactorName", sValue = "Base Rate", bDirty = true };
                    oBO.Attributes["sValue"] = new XIIAttribute { sName = "sValue", sValue = oBaseLoadFactor.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    oBO.Attributes["sMessage"] = new XIIAttribute { sName = "sMessage", sValue = "Base Rate", bDirty = true };
                    oBO.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString("dd-MMM-yyyy"), bDirty = true };
                    oBO.Attributes["ID"] = new XIIAttribute { sName = "ID", bDirty = true };
                    var oRes1 = oBO.Save(oBO);
                    if (oRes1.bOK && oRes1.oResult != null)
                    {
                        oBO = (XIIBO)oRes1.oResult;
                    }
                    oBO.Attributes["FKiQuoteID"] = new XIIAttribute { sName = "FKiQuoteID", sValue = oBII.Attributes["ID"].sValue, bDirty = true };
                    oBO.Attributes["sFactorName"] = new XIIAttribute { sName = "sFactorName", sValue = oExcessVal.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    oBO.Attributes["sValue"] = new XIIAttribute { sName = "sValue", sValue = oExcessVal.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    oBO.Attributes["sMessage"] = new XIIAttribute { sName = "sMessage", sValue = oExcessVal.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                    oBO.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString("dd-MMM-yyyy"), bDirty = true };
                    oBO.Attributes["ID"] = new XIIAttribute { sName = "ID", bDirty = true };
                    var oRes2 = oBO.Save(oBO);
                    if (oRes2.bOK && oRes2.oResult != null)
                    {
                        oBO = (XIIBO)oRes2.oResult;
                    }
                    oXiResults.Add(IPTLoadFactor);
                    foreach (var item in oXiResults)
                    {
                        //oResult.sMessage = "RiskFacor " + item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault() + " _ " + item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault() + " _ " + item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault() + "_QuoteID" + oBII.Attributes["ID"].sValue;
                        //oIB.SaveErrortoDB(oResult);
                        oBO.Attributes["FKiQuoteID"] = new XIIAttribute { sName = "FKiQuoteID", sValue = oBII.Attributes["ID"].sValue, bDirty = true };
                        oBO.Attributes["sFactorName"] = new XIIAttribute { sName = "sFactorName", sValue = item.oCollectionResult.Where(m => m.sName == "LoadFactorName").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                        oBO.Attributes["sValue"] = new XIIAttribute { sName = "sValue", sValue = item.oCollectionResult.Where(m => m.sName == "oResult").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                        oBO.Attributes["sMessage"] = new XIIAttribute { sName = "sMessage", sValue = item.oCollectionResult.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault(), bDirty = true };
                        oBO.Attributes["CreatedTime"] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString("dd-MMM-yyyy"), bDirty = true };
                        oBO.Attributes["ID"] = new XIIAttribute { sName = "ID", bDirty = true };
                        var oRes3 = oBO.Save(oBO);
                        if (oRes3.bOK && oRes3.oResult != null)
                        {
                            oBO = (XIIBO)oRes3.oResult;
                        }
                    }
                    oResult.oCollectionResult.Add(new CNV { sName = "QuoteID", sValue = oBII.Attributes["ID"].sValue });
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
                    if (TypeOfCover.ToLower() == "comp" && Amount < 180)
                    {
                        NetAmount = 180;
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
                    oresult.sMessage = "AreaGroup-" + AreaGroup + "Type of cover" + comp;
                    oIB.SaveErrortoDB(oresult);
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
                            oNV1.sValue = comp.ToLower() == "comp" ? "509" : "414";
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
                            oNV1.sValue = comp.ToLower() == "comp" ? "579" : "471";
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
                            oNV1.sValue = "+700.00";
                            oNV.sValue = xiEnumSystem.xiEnumPolicyLookupResponses.Refer.ToString();
                            break;
                    }
                    oresult.sMessage = "Base Rate-" + oNV1.sValue + "_" + oNV.sValue;
                    oIB.SaveErrortoDB(oresult);
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