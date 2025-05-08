using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using XISystem;
using XIDatabase;
using System.Configuration;

namespace XICore
{
    public class XIIAttribute : XIInstanceBase, IEquatable<XIIAttribute>
    {
        public string sName { get; set; }
        public string sDisplayName { get; set; }
        private string sMyValue;
        public string sValue
        {
            get
            {
                return sMyValue;
            }
            set
            {
                sMyValue = value;
            }
        }
        private string sMyFormat;
        public string Format
        {
            get
            {
                return sMyFormat;
            }
            set
            {
                sMyFormat = value;
            }
        }
        private string sMyDefaultDate;
        public string sDefaultDate
        {
            get
            {
                return sMyDefaultDate;
            }
            set
            {
                sMyDefaultDate = value;
            }
        }
        private int iMyOneClickID;
        public int iOneClickID
        {
            get
            {
                return iMyOneClickID;
            }
            set
            {
                iMyOneClickID = value;
            }
        }
        private string sMyResolvedValue;
        public string sResolvedValue
        {
            get
            {
                if (!string.IsNullOrEmpty(sValue))
                {
                    if (sValue.StartsWith("£"))
                    {
                        sValue = sValue.Substring(1, sValue.Length - 1);
                        if (sValue.Contains(','))
                        {
                            sValue = sValue.Replace(",", "");
                        }
                    }
                    sMyResolvedValue = GetResolvedValue();
                }
                return sMyResolvedValue;
            }
            set
            {
                sMyResolvedValue = value;
            }
        }
        private string sMyResolvedPreviousValue;
        public string sPreviousResolvedValue
        {
            get
            {
                if (!string.IsNullOrEmpty(sPreviousValue))
                {
                    if (sPreviousValue.StartsWith("£"))
                    {
                        sPreviousValue = sPreviousValue.Substring(1, sPreviousValue.Length - 1);
                        if (sPreviousValue.Contains(','))
                        {
                            sPreviousValue = sPreviousValue.Replace(",", "");
                        }
                    }
                    sMyResolvedPreviousValue = GetResolvedPreviousValue();
                }
                return sMyResolvedPreviousValue;
            }
            set
            {
                sMyResolvedPreviousValue = value;
            }
        }
        private string sMyPreviousValue;
        public string sPreviousValue
        {
            get
            {
                return sMyPreviousValue;
            }
            set
            {
                sMyPreviousValue = value;
            }
        }
        private DateTime dMyValue;
        public DateTime dValue
        {
            get
            {
                if (!string.IsNullOrEmpty(sValue))
                {
                    //XIIBO oBOI = new XIIBO();
                    //dMyValue = oBOI.ConvertToDtTime(sValue);
                }
                return dMyValue;
            }
            set
            {
                dMyValue = value;
            }
        }


        private int iMyValue;
        public int iValue
        {
            get
            {
                if (!string.IsNullOrEmpty(sValue))
                {
                    int.TryParse(sValue, out iMyValue);
                }
                return iMyValue;
            }
            set
            {
                iMyValue = value;
            }
        }

        private double doMyValue;
        public double doValue
        {
            get
            {
                if (!string.IsNullOrEmpty(sValue))
                {
                    double.TryParse(sValue, out doMyValue);
                }
                return doMyValue;
            }
            set
            {
                doMyValue = value;
            }
        }
        public double sTotalValue { get; set; }
        public bool bMyTotalValue { get; set; }

        private long lMyValue;
        public long lValue
        {
            get
            {
                if (!string.IsNullOrEmpty(sValue))
                {
                    long.TryParse(sValue, out lMyValue);
                }
                return lMyValue;
            }
            set
            {
                lMyValue = value;
            }
        }

        private bool bMyValue;
        public bool bValue
        {
            get
            {
                if (!string.IsNullOrEmpty(sValue))
                {
                    bool.TryParse(sValue, out bMyValue);
                }
                return bMyValue;
            }
            set
            {
                bMyValue = value;
            }
        }

        private decimal rMyValue;
        public decimal rValue
        {
            get
            {
                if (!string.IsNullOrEmpty(sValue))
                {
                    decimal.TryParse(sValue, out rMyValue);
                }
                return rMyValue;
            }
            set
            {
                rMyValue = value;
            }
        }

        private List<XIDropDown> oMyImagePathDetails = new List<XIDropDown>();
        public List<XIDropDown> ImagePathDetails
        {
            get
            {
                return oMyImagePathDetails;
            }
            set
            {
                oMyImagePathDetails = value;
            }
        }

        private List<XIDropDown> oMyFieldDDL = new List<XIDropDown>();
        public List<XIDropDown> FieldDDL
        {
            get
            {
                return oMyFieldDDL;
            }
            set
            {
                oMyFieldDDL = value;
            }
        }
        private List<string> oMyFieldDDL1 = new List<string>();
        public List<string> FieldDDL1
        {
            get
            {
                return oMyFieldDDL1;
            }
            set
            {
                oMyFieldDDL1 = value;
            }
        }
        public bool bDirty { get; set; }
        public bool bLock { get; set; }
        public bool bIsHidden { get; set; }
        public bool bIsScript { get; set; }
        public bool bISConfirm { get; set; }
        public string sAllowedValue { get; set; }
        public string sValueTypeIcon { get; set; }
        public DateTime ConvertToDtTime(string InputString)
        {
            try
            {

                CultureInfo provider = CultureInfo.InvariantCulture;
                string[] formats = {
"yyyy-MM-dd", "yyyy-MMM-dd", "yyyy.MM.dd","yyyy/MM/dd","yyyy/MMM/dd","yyyy.MMM.dd",
"dd-MM-yyyy","dd.MM.yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "dd.MMM.yyyy",
"MMM-dd-yyyy","MM-dd-yyyy", "MM.dd.yyyy", "MMM.dd.yyyy", "MM/dd/yyyy",
"dd/MM/yyyy hh:mm:ss",
"MM/dd/yyyy hh:mm:ss tt",
"MM/dd/yyyy hh:m:ss tt",
"MM/dd/yyyy hh:mm:s tt",
"MM/dd/yyyy hh:m:s tt",
"MM/dd/yyyy h:mm:s tt",
"MM/dd/yyyy h:mm:ss tt",
"MM/dd/yyyy h:m:ss tt",
"MM/dd/yyyy h:m:s tt",
"MM/dd/yyyy hh:mm:ss tt",
"M/dd/yyyy hh:m:ss tt",
"M/dd/yyyy hh:mm:s tt",
"M/dd/yyyy hh:m:s tt",
"M/dd/yyyy h:mm:s tt",
"M/dd/yyyy h:mm:ss tt",
"M/dd/yyyy h:m:ss tt",
"M/dd/yyyy h:m:s tt",


"MM/d/yyyy hh:mm:ss tt",
"MM/d/yyyy hh:m:ss tt",
"MM/d/yyyy hh:mm:s tt",
"MM/d/yyyy hh:m:s tt",
"MM/d/yyyy h:mm:s tt",
"MM/d/yyyy h:mm:ss tt",
"MM/d/yyyy h:m:ss tt",
"MM/d/yyyy h:m:s tt",
"MM/d/yyyy hh:mm:ss tt",
"M/d/yyyy hh:m:ss tt",
"M/d/yyyy hh:mm:s tt",
"M/d/yyyy hh:m:s tt",
"M/d/yyyy h:mm:s tt",
"M/d/yyyy h:mm:ss tt",
"M/d/yyyy h:m:ss tt",
"M/d/yyyy h:m:s tt",

"MM/dd/yyyy hh:mm:ss tt", "M/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "yyyy-MM-dd hh:mm:ss tt", "yyyy-MMM-dd hh:mm:ss tt", "yyyy.MM.dd hh:mm:ss tt","yyyy/MM/dd hh:mm:ss tt","yyyy/MMM/dd hh:mm:ss tt","yyyy.MMM.dd hh:mm:ss tt",
"dd-MM-yyyy hh:mm:ss tt","dd.MM.yyyy hh:mm:ss tt", "dd/MM/yyyy hh:mm:ss tt", "dd-MMM-yyyy hh:mm:ss tt", "dd.MMM.yyyy hh:mm:ss tt",
"MMM-dd-yyyy hh:mm:ss tt","MM-dd-yyyy hh:mm:ss tt", "MM.dd.yyyy hh:mm:ss tt", "MMM.dd.yyyy hh:mm:ss tt", "dd-MM-yyyy hh:mm:ss"
};
                DateTime dateValue;
                // var dt = "26.May.1975";
                bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);
                if (!IsValidDate)
                {
                    dateValue = new DateTime();
                }
                return dateValue;
            }
            catch (Exception ex)
            {
                CResult oCResult = new CResult();
                oCResult.sMessage = "Error In ConvertToDtTime() - Can't convert string to datetime, Data String:" + sValue;
                SaveErrortoDB(oCResult);
                throw ex;
            }
        }

        private string GetResolvedValue(string sGroup = "", bool bIsCode = false)
        {
            string sRValue = sValue;
            string sGroupName = "fklabel";
            if (!string.IsNullOrEmpty(sGroup))
            {
                sGroupName = sGroup;
            }
            if (BOD == null)
            {
                XIInfraCache oCache = new XIInfraCache();
                if (BOI != null && BOI.BOD != null && BOI.BOD.XIGUID != null && BOI.BOD.XIGUID != Guid.Empty)
                {
                    BOD = BOI.BOD;
                }
                else if (BOI != null && ((BOI.iBODID > 0) || (BOI.BOIDXIGUID != null && BOI.BOIDXIGUID != Guid.Empty)))
                {
                    if (BOI.BOIDXIGUID != null && BOI.BOIDXIGUID != Guid.Empty)
                        BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOI.BOIDXIGUID.ToString());
                    else
                        BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOI.iBODID.ToString());
                }
            }
            if (BOD != null && BOD.Attributes != null)
            {
                XIDAttribute oAttrD = null;
                if (BOD.Attributes.ContainsKey(sName.ToLower()))
                {
                    oAttrD = BOD.Attributes[sName.ToLower()];
                }
                else
                {
                    oAttrD = BOD.Attributes.Values.Where(m => m.LabelName.ToLower() == sName.ToLower()).FirstOrDefault();
                }
                if (oAttrD != null)
                {
                    if (!string.IsNullOrEmpty(oAttrD.sFKBOName))
                    {
                        //if (oAttrD.Name.StartsWith("s"))
                        //{
                        //    sRValue = sValue;
                        //}
                        //else
                        //{
                        XIDXI oXID = new XIDXI();
                        XIInfraCache oCache = new XIInfraCache();
                        var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oAttrD.sFKBOName);
                        var GroupD = new XIDGroup();
                        if (!string.IsNullOrEmpty(sGroupName) && sGroupName.ToLower() == "fklabel")
                        {
                            GroupD = oBOD.GroupD(sGroupName);
                            if (GroupD != null && GroupD.XIGUID != null && GroupD.XIGUID != Guid.Empty)
                            {

                            }
                            else
                            {
                                sGroupName = "label";
                                GroupD = oBOD.GroupD("label");
                            }
                        }
                        else
                        {
                            GroupD = oBOD.GroupD(sGroupName);
                        }
                        if (GroupD != null && GroupD.XIGUID != null && GroupD.XIGUID != Guid.Empty)
                        {
                            XIDBAPI Myconntection = new XIDBAPI(oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID));
                            if (oBOD != null && oBOD.Groups.Any(group => group.Key.ToLower() == sGroupName.ToLower()) && !string.IsNullOrEmpty(sValue) && sValue != "0")
                            {
                                //var BODParams = new Dictionary<string, object>();
                                //BODParams[oBOD.sPrimaryKey] = sValue;
                                //string FinalString = oBOD.GroupD(sGroupName).ConcatanateGroupFields(" ");//concatenate the string with join String 
                                //if (!string.IsNullOrEmpty(FinalString))
                                //{
                                //    var Result = Myconntection.Select<string>(oAttrD.FKTableName, BODParams, FinalString + " As Result ").FirstOrDefault();
                                //    sRValue = Result;
                                //}
                                var BODParams = new Dictionary<string, object>();
                                var XIGUID = Guid.Empty;
                                if (Guid.TryParse(sValue, out XIGUID))
                                {
                                    BODParams["XIGUID"] = sValue;
                                }
                                else
                                {
                                    BODParams[oBOD.sPrimaryKey] = sValue;
                                }
                                string Result = "";
                                XIIXI oXII = new XIIXI();
                                var oBOI = oXII.BOI(oBOD.Name, sValue, "", null, false);
                                if (!string.IsNullOrEmpty(sGroupName) && oBOI != null)
                                {
                                    var BOFields = oBOD.GroupD(sGroupName.ToLower()).BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    foreach (var item in BOFields)
                                    {
                                        var AttrD = new XIDAttribute();
                                        if (oBOD.Attributes.ContainsKey(item))
                                        {
                                            AttrD = oBOD.Attributes[item];
                                        }
                                        else if (oBOD.Attributes.ContainsKey(item.ToLower()))
                                        {
                                            AttrD = oBOD.Attributes[item.ToLower()];
                                        }

                                        if (AttrD.IsOptionList)
                                        {
                                            var value = oBOI.Attributes[item].sValue;
                                            Result = Result + " " + AttrD.OptionList.Where(m => m.sValues == value).Select(m => m.sOptionName).FirstOrDefault();
                                        }
                                        else
                                        {
                                            if (oBOI.Attributes.Count() > 0 && oBOI.Attributes.ContainsKey(item))
                                            {
                                                Result = Result + " " + oBOI.Attributes[item].sValue;
                                            }
                                        }
                                    }
                                    sRValue = Result.Trim();
                                }
                                else if (oBOI == null)
                                {
                                    string FinalString = oBOD.GroupD(sGroupName).ConcatanateGroupFields(" ");//concatenate the string with join String 
                                    if (!string.IsNullOrEmpty(FinalString))
                                    {
                                        Result = Myconntection.Select<string>(oBOD.TableName, BODParams, FinalString + " As Result ").FirstOrDefault();
                                        sRValue = Result;
                                    }
                                }
                            }
                        }
                        //}
                    }
                    else if (oAttrD.TypeID == 150 || oAttrD.TypeID == 110)
                    {
                        if (string.IsNullOrEmpty(sValue) && !string.IsNullOrEmpty(oAttrD.DefaultValue))
                        {
                            sRValue = Utility.GetDefaultDateResolvedValue(oAttrD.DefaultValue, oAttrD.Format);
                        }
                    }
                    else if (!string.IsNullOrEmpty(oAttrD.Format))
                    {
                        var sFomattedVal = sValue.Contains('£') ? sValue : FormatValue(oAttrD.Name, sValue, oAttrD.Format);
                        if (!string.IsNullOrEmpty(sFomattedVal))
                        {
                            sRValue = sFomattedVal;
                        }
                    }
                    else if (oAttrD.IsOptionList)
                    {
                        if (bIsCode == true)
                        {
                            var sOptionCode = oAttrD.OptionList.Where(m => m.sValues == sValue).Select(m => m.sOptionCode).FirstOrDefault();
                            if (!string.IsNullOrEmpty(sOptionCode))
                            {
                                sRValue = sOptionCode;
                            }
                        }
                        else
                        {
                            if (oAttrD.OptionList != null)
                            {
                                var sOptionValue = oAttrD.OptionList.Where(m => m.sValues == sValue).Select(m => m.sOptionName).FirstOrDefault();
                                var sUistyle = oAttrD.OptionList.Where(m => m.sValues == sValue).Select(m => m.sUIStyle).FirstOrDefault();
                                if (!string.IsNullOrEmpty(sOptionValue))
                                {
                                    if (!string.IsNullOrEmpty(sUistyle))
                                    {
                                        sRValue = CheckOptionList(oAttrD.Name, sValue, BOD);
                                    }
                                    else
                                    {
                                    sRValue = sOptionValue;
                                    }
                                }
                            }
                            else
                            {
                                sRValue = sValue;
                            }
                        }

                    }
                    else if (oAttrD.FKiFileTypeID > 0)
                    {
                        //Get Image Details and assign to Attribute
                        List<XIDropDown> sPDFPathDetails = new List<XIDropDown>();
                        var NewFileID = sValue.Split(',').ToList();
                        foreach (var item in NewFileID)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                //XIInfraDocs oDocs = new XIInfraDocs();
                                if (oAttrD.FKiFileTypeID == 2)
                                {
                                    sPDFPathDetails.Add(new XIDropDown { Expression = item });
                                    ImagePathDetails.AddRange(sPDFPathDetails);
                                }
                                else
                                {
                                    XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
                                    Dictionary<string, object> oPerams = new Dictionary<string, object>();
                                    oPerams["Name"] = "XIDocs";
                                    string SelectdFields = "Name, TableName, sSize, sPrimaryKey, iDataSource";
                                    var oBODetils = Connection.Select<XIDBO>("XIBO_T_N", oPerams, SelectdFields).FirstOrDefault();
                                    if (oBODetils != null)
                                    {

                                        //string sConString = oConString.ConnectionString(oBODetils.iDataSource);
                                        // Connection = new XIDBAPI(sConString);
                                        //Dictionary<string, object> Params = new Dictionary<string, object>();
                                        //Params["ID"] = ID;
                                        //oXIDocs = Connection.Select<XIInfraDocs>("XIDocs", Params).ToList();
                                    }
                                    //oDocs.ID = Convert.ToInt32(item);
                                    //var sImagePathDetails = (List<XIDropDown>)oDocs.Get_FilePathDetails(BOI.sCoreDataBase).oResult;
                                    //if (sImagePathDetails != null)
                                    //{
                                    //    ImagePathDetails.AddRange(sImagePathDetails);
                                    //}

                                }
                            }
                        }
                    }
                }
                else if (sName.ToLower() == "XICreatedWhen".ToLower() || sName.ToLower() == "XIUpdatedWhen".ToLower())
                {
                    DateTime dtTime = Convert.ToDateTime(sValue);
                    var Time = dtTime.ToString("dd-MMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    sRValue = Time;
                }
            }
            BOD = null;
            return sRValue;
        }

        private string GetResolvedPreviousValue(string sGroup = "", bool bIsCode = false)
        {
            string sRValue = sPreviousValue;
            string sGroupName = "label";
            if (!string.IsNullOrEmpty(sGroup))
            {
                sGroupName = sGroup;
            }
            if (BOD == null)
            {
                XIInfraCache oCache = new XIInfraCache();
                if (BOI != null && BOI.BOD != null && BOI.BOD.XIGUID != null && BOI.BOD.XIGUID != Guid.Empty)
                {
                    BOD = BOI.BOD;
                }
                else if (BOI != null && ((BOI.iBODID > 0) || (BOI.BOIDXIGUID != null && BOI.BOIDXIGUID != Guid.Empty)))
                {
                    if (BOI.BOIDXIGUID != null && BOI.BOIDXIGUID != Guid.Empty)
                        BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOI.BOIDXIGUID.ToString());
                    else
                        BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOI.iBODID.ToString());
                }
            }
            if (BOD != null && BOD.Attributes != null)
            {
                XIDAttribute oAttrD = null;
                if (BOD.Attributes.ContainsKey(sName.ToLower()))
                {
                    oAttrD = BOD.Attributes[sName.ToLower()];
                }
                else
                {
                    oAttrD = BOD.Attributes.Values.Where(m => m.LabelName.ToLower() == sName.ToLower()).FirstOrDefault();
                }
                if (oAttrD != null)
                {
                    if (!string.IsNullOrEmpty(oAttrD.sFKBOName))
                    {
                        //if (oAttrD.Name.StartsWith("s"))
                        //{
                        //    sRValue = sValue;
                        //}
                        //else
                        //{
                        XIDXI oXID = new XIDXI();
                        XIInfraCache oCache = new XIInfraCache();
                        var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oAttrD.sFKBOName);
                        var GroupD = new XIDGroup();
                        if (oBOD.Groups.TryGetValue(sGroupName.ToLower(), out GroupD))
                        {
                            XIDBAPI Myconntection = new XIDBAPI(oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID));
                            if (oBOD != null && oBOD.Groups.Any(group => group.Key.ToLower() == sGroupName.ToLower()) && !string.IsNullOrEmpty(sValue) && sValue != "0")
                            {
                                //var BODParams = new Dictionary<string, object>();
                                //BODParams[oBOD.sPrimaryKey] = sValue;
                                //string FinalString = oBOD.GroupD(sGroupName).ConcatanateGroupFields(" ");//concatenate the string with join String 
                                //if (!string.IsNullOrEmpty(FinalString))
                                //{
                                //    var Result = Myconntection.Select<string>(oAttrD.FKTableName, BODParams, FinalString + " As Result ").FirstOrDefault();
                                //    sRValue = Result;
                                //}
                                var BODParams = new Dictionary<string, object>();
                                var XIGUID = Guid.Empty;
                                if (Guid.TryParse(sPreviousValue, out XIGUID))
                                {
                                    BODParams["XIGUID"] = sPreviousValue;
                                }
                                else
                                {
                                    BODParams[oBOD.sPrimaryKey] = sPreviousValue;
                                }
                                string Result = "";
                                XIIXI oXII = new XIIXI();
                                var oBOI = oXII.BOI(oBOD.Name, sPreviousValue, "", null, false);
                                if (!string.IsNullOrEmpty(sGroupName) && oBOI != null)
                                {
                                    var BOFields = oBOD.GroupD(sGroupName.ToLower()).BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    foreach (var item in BOFields)
                                    {
                                        var AttrD = new XIDAttribute();
                                        if (oBOD.Attributes.ContainsKey(item))
                                        {
                                            AttrD = oBOD.Attributes[item];
                                        }
                                        else if (oBOD.Attributes.ContainsKey(item.ToLower()))
                                        {
                                            AttrD = oBOD.Attributes[item];
                                        }

                                        if (AttrD.IsOptionList)
                                        {
                                            var value = oBOI.Attributes[item].sPreviousValue;
                                            Result = Result + " " + AttrD.OptionList.Where(m => m.sValues == value).Select(m => m.sOptionName).FirstOrDefault();
                                        }
                                        else
                                        {
                                            if (oBOI.Attributes.Count() > 0 && oBOI.Attributes.ContainsKey(item))
                                            {
                                                Result = Result + " " + oBOI.Attributes[item].sPreviousValue;
                                            }
                                        }
                                    }
                                    sRValue = Result.Trim();
                                }
                                else if (oBOI == null)
                                {
                                    string FinalString = oBOD.GroupD(sGroupName).ConcatanateGroupFields(" ");//concatenate the string with join String 
                                    if (!string.IsNullOrEmpty(FinalString))
                                    {
                                        Result = Myconntection.Select<string>(oBOD.TableName, BODParams, FinalString + " As Result ").FirstOrDefault();
                                        sRValue = Result;
                                    }
                                }
                            }
                        }
                        //}
                    }
                    else if (!string.IsNullOrEmpty(oAttrD.Format))
                    {
                        var sFomattedVal = sPreviousValue.Contains('£') ? sPreviousValue : FormatValue(oAttrD.Name, sPreviousValue, oAttrD.Format);
                        if (!string.IsNullOrEmpty(sFomattedVal))
                        {
                            sRValue = sFomattedVal;
                        }
                    }
                    else if (oAttrD.IsOptionList)
                    {
                        if (bIsCode == true)
                        {
                            var sOptionCode = oAttrD.OptionList.Where(m => m.sValues == sPreviousValue).Select(m => m.sOptionCode).FirstOrDefault();
                            if (!string.IsNullOrEmpty(sOptionCode))
                            {
                                sRValue = sOptionCode;
                            }
                        }
                        else
                        {
                            var sOptionValue = oAttrD.OptionList.Where(m => m.sValues == sPreviousValue).Select(m => m.sOptionName).FirstOrDefault();
                            var sUIstyle = oAttrD.OptionList.Where(m => m.sValues == sPreviousValue).Select(m => m.sUIStyle).FirstOrDefault();
                            if (!string.IsNullOrEmpty(sOptionValue))
                            {
                                if (!string.IsNullOrEmpty(sUIstyle)) 
                                {
                                    sRValue = CheckOptionList(oAttrD.Name, sRValue, BOD);
                                }
                                else
                                {
                                sRValue = sOptionValue;
                                }
                            }
                        }

                    }
                    else if (oAttrD.FKiFileTypeID > 0)
                    {
                        //Get Image Details and assign to Attribute
                        List<XIDropDown> sPDFPathDetails = new List<XIDropDown>();
                        var NewFileID = sPreviousValue.Split(',').ToList();
                        foreach (var item in NewFileID)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                //XIInfraDocs oDocs = new XIInfraDocs();
                                if (oAttrD.FKiFileTypeID == 2)
                                {
                                    sPDFPathDetails.Add(new XIDropDown { Expression = item });
                                    ImagePathDetails.AddRange(sPDFPathDetails);
                                }
                                else
                                {
                                    XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
                                    Dictionary<string, object> oPerams = new Dictionary<string, object>();
                                    oPerams["Name"] = "XIDocs";
                                    string SelectdFields = "Name, TableName, sSize, sPrimaryKey, iDataSource";
                                    var oBODetils = Connection.Select<XIDBO>("XIBO_T_N", oPerams, SelectdFields).FirstOrDefault();
                                    if (oBODetils != null)
                                    {

                                        //string sConString = oConString.ConnectionString(oBODetils.iDataSource);
                                        // Connection = new XIDBAPI(sConString);
                                        //Dictionary<string, object> Params = new Dictionary<string, object>();
                                        //Params["ID"] = ID;
                                        //oXIDocs = Connection.Select<XIInfraDocs>("XIDocs", Params).ToList();
                                    }
                                    //oDocs.ID = Convert.ToInt32(item);
                                    //var sImagePathDetails = (List<XIDropDown>)oDocs.Get_FilePathDetails(BOI.sCoreDataBase).oResult;
                                    //if (sImagePathDetails != null)
                                    //{
                                    //    ImagePathDetails.AddRange(sImagePathDetails);
                                    //}

                                }
                            }
                        }

                    }
                }
                else if (sName.ToLower() == "XICreatedWhen".ToLower() || sName.ToLower() == "XIUpdatedWhen".ToLower())
                {
                    DateTime dtTime = Convert.ToDateTime(sPreviousValue);
                    var Time = dtTime.ToString("dd-MMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    sRValue = Time;
                }
            }
            BOD = null;
            return sRValue;
        }
        public string GetDateDifference()
        {
            int iDiff = 0;
            if (!string.IsNullOrEmpty(sValue))
            {
                XIIBO oBOI = new XIIBO();
                var dateval = oBOI.ConvertToDtTime(sValue);
                DateTime dDate = dateval;// Convert.ToDateTime(sValue);
                DateTime now = DateTime.Today;
                iDiff = now.Year - dDate.Year;
                if (dDate > now.AddYears(-iDiff)) iDiff--;
            }
            string sDiff = Convert.ToString(iDiff);
            return sDiff;
        }
        public string ResolveFK(string sGroup = "")
        {
            if (!string.IsNullOrEmpty(sValue))
            {
                return GetResolvedValue(sGroup);
            }
            return null;
        }
        public string ResolveCode()
        {
            return GetResolvedValue("", true);
        }

        //private string GetResolvedValue()
        //{
        //    string sRValue = sValue;
        //    XIInfraCache oCache = new XIInfraCache();
        //    if (BOI != null && BOI.iBODID > 0)
        //    {
        //        var oBODefinition = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOI.iBODID.ToString());
        //        if (oBODefinition != null && oBODefinition.Attributes != null)
        //        {
        //            XIDAttribute oAttrD = null;
        //            if (oBODefinition.Attributes.ContainsKey(sName.ToLower()))
        //            {
        //                oAttrD = oBODefinition.Attributes[sName.ToLower()];
        //            }
        //            else
        //            {
        //                oAttrD = oBODefinition.Attributes.Values.Where(m => m.LabelName.ToLower() == sName.ToLower()).FirstOrDefault();
        //            }
        //            if (oAttrD != null)
        //            {
        //                if (!string.IsNullOrEmpty(oAttrD.sFKBOName))
        //                {
        //                    XIDXI oXID = new XIDXI();
        //                    XIInfraCache oCahce = new XIInfraCache();
        //                    var oBOD = (XIDBO)oCahce.GetObjectFromCache(XIConstant.CacheBO, oAttrD.sFKBOName);
        //                    var GroupD = new XIDGroup();
        //                    if (oBOD.Groups.TryGetValue("label", out GroupD))
        //                    {
        //                        XIDBAPI Myconntection = new XIDBAPI(oXID.GetBODataSource(oBOD.iDataSource));
        //                        if (oBOD != null && oBOD.Groups.Any(group => group.Key.ToLower() == "label") && (oBOD.sSize == "30" || oBOD.sSize == "20") && !string.IsNullOrEmpty(sValue) && sValue != "0")
        //                        {
        //                            var BODParams = new Dictionary<string, object>();
        //                            BODParams[oBOD.sPrimaryKey] = sValue;
        //                            string FinalString = oBOD.GroupD("label").ConcatanateGroupFields(" ");//concatenate the string with join String 
        //                            if (!string.IsNullOrEmpty(FinalString))
        //                            {
        //                                var Result = Myconntection.Select<string>(oAttrD.FKTableName, BODParams, FinalString + " As Result ").FirstOrDefault();
        //                                sRValue = Result;
        //                            }
        //                        }
        //                    }
        //                }
        //                else if (oAttrD.Format != null)
        //                {
        //                    var sFomattedVal = FormatValue(oAttrD.Name, sValue, oAttrD.Format);
        //                    if (!string.IsNullOrEmpty(sFomattedVal))
        //                    {
        //                        sRValue = sFomattedVal;
        //                    }
        //                }
        //                else if (oAttrD.IsOptionList)
        //                {
        //                    var sOptionValue = oAttrD.OptionList.Where(m => m.sValues == sValue).Select(m => m.sOptionName).FirstOrDefault();
        //                    if (sOptionValue != null)
        //                    {
        //                        sRValue = sOptionValue;
        //                    }
        //                }
        //                else if (oAttrD.FKiFileTypeID > 0)
        //                {
        //                    //Get Image Details and assign to Attribute
        //                    List<XIDropDown> sPDFPathDetails = new List<XIDropDown>();
        //                    var NewFileID = sValue.Split(',').ToList();
        //                    foreach (var item in NewFileID)
        //                    {
        //                        if (!string.IsNullOrEmpty(item))
        //                        {
        //                            //XIInfraDocs oDocs = new XIInfraDocs();
        //                            if (oAttrD.FKiFileTypeID == 2)
        //                            {
        //                                sPDFPathDetails.Add(new XIDropDown { Expression = item });
        //                                ImagePathDetails.AddRange(sPDFPathDetails);
        //                            }
        //                            else
        //                            {
        //                                XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        //                                Dictionary<string, object> oPerams = new Dictionary<string, object>();
        //                                oPerams["Name"] = "XIDocs";
        //                                string SelectdFields = "Name, TableName, sSize, sPrimaryKey, iDataSource";
        //                                var oBODetils = Connection.Select<XIDBO>("XIBO_T_N", oPerams, SelectdFields).FirstOrDefault();
        //                                if (oBODetils != null)
        //                                {

        //                                    //string sConString = oConString.ConnectionString(oBODetils.iDataSource);
        //                                    // Connection = new XIDBAPI(sConString);
        //                                    //Dictionary<string, object> Params = new Dictionary<string, object>();
        //                                    //Params["ID"] = ID;
        //                                    //oXIDocs = Connection.Select<XIInfraDocs>("XIDocs", Params).ToList();
        //                                }
        //                                //oDocs.ID = Convert.ToInt32(item);
        //                                //var sImagePathDetails = (List<XIDropDown>)oDocs.Get_FilePathDetails(BOI.sCoreDataBase).oResult;
        //                                //if (sImagePathDetails != null)
        //                                //{
        //                                //    ImagePathDetails.AddRange(sImagePathDetails);
        //                                //}

        //                            }
        //                        }
        //                    }

        //                }
        //            }
        //        }
        //    }
        //    return sRValue;
        //}
        private string FormatValue(string sField, string sValue, string Format)
        {
            string sFormattedValue = string.Empty;
            var Char = sField.Select(c => char.IsUpper(c)).ToList();
            var Position = Char.IndexOf(true);
            if (Position == 1)
            {
                char FirstLetter = sField[0];
                if (FirstLetter == 'r')
                {
                    CultureInfo rgi = new CultureInfo(Format);
                    sFormattedValue = string.Format(rgi, "{0:c}", Convert.ToDecimal(sValue)).ToString();
                }
                else if (FirstLetter == 'd')
                {
                    if (sValue != "#date")
                    {
                        sFormattedValue = String.Format("{0:" + Format + "}", Convert.ToDateTime(sValue));
                    }
                }
                else if (FirstLetter == 'z')
                {
                    if (sField.ToLower() == "zXAuditCrtdWhn".ToLower())
                    {
                        sFormattedValue = String.Format("{0:" + Format + "}", Convert.ToDateTime(sValue));
                    }
                    else if (sField.ToLower() != XIConstant.Key_XICrtdWhn.ToLower() && sField.ToLower() != XIConstant.Key_XICrtdBy.ToLower() && sField.ToLower() != XIConstant.Key_XIUpdtdBy.ToLower() && sField.ToLower() != XIConstant.Key_XIUpdtdWhn.ToLower())
                    {
                        CultureInfo rgi = new CultureInfo(Format);
                        sFormattedValue = string.Format(rgi, "{0:c}", Convert.ToDecimal(sValue)).ToString();
                    }
                }
            }
            else if (Position == 2)
            {
                var Prefix = sField.Substring(0, 2);
                if (Prefix == "dt")
                {
                    sFormattedValue = String.Format("{0:" + Format + "}", Convert.ToDateTime(sValue));
                }
            }
            return sFormattedValue;
        }

        #region Compare_List_of_Two_XIIAttributes
        /*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
          :: Check two XIIAttributes with sName, sValue Properties are equal or not and retrun true or false ::
          :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::*/
        public bool Equals(XIIAttribute obj)
        {
            try
            {
                if (ReferenceEquals(this, obj)) return true;
                if ((this == null) || (obj == null)) return false;
                if (this.GetType() != obj.GetType()) return false;

                if (!this.GetType().IsClass) return this.Equals(obj);

                foreach (var property in this.GetType().GetProperties().Where(Pro => (Pro.Name == "sName" || Pro.Name == "sValue")))
                {
                    var objValue = property.GetValue(this);
                    var anotherValue = property.GetValue(obj);
                    if (objValue != null && !objValue.Equals(anotherValue)) return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Compare_List_of_Two_XIIAttributes

        public CResult Check_ValueTypeForMetaIcon(string sValue)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Check sValue and decide to show meta resolve icon or not";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "sValue", sValue = sValue });
                if (!string.IsNullOrEmpty(sValue))//check mandatory params are passed or not
                {
                    string sShowIcon = string.Empty;
                    Guid XIGUID = Guid.Empty;
                    if (Guid.TryParse(sValue, out XIGUID))
                    {
                        sShowIcon = true + "_GUID";
                    }
                    else if (sValue.ToLower().StartsWith("http"))
                    {
                        sShowIcon = true + "_URL";
                    }
                    else if (sValue.StartsWith("@"))
                    {
                        sShowIcon = true + "_XIPerson";
                    }
                    else if (sValue.StartsWith("#"))
                    {
                        sShowIcon = true + "_XIKeyword";
                    }
                    else
                    {
                        sShowIcon = "false";
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sShowIcon;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sValue is missing";
                }
                //@Ravi-XIPerson
                //#Keyword-XIKeyword
                //XIURL
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public string CheckOptionList(string columnName, string sValue, XIDBO oBOD)
        {
            var oAttrD = oBOD.Attributes[columnName.ToLower()];
            var oBOOption = oAttrD.OptionList.Where(m => m.sValues == sValue).FirstOrDefault();
            if (oBOOption != null)
            {
                var sOptionValue = oBOOption.sOptionName;
                string sUIStyle = oBOOption.sUIStyle;
                if (!string.IsNullOrEmpty(sOptionValue))
                {
                    if (!string.IsNullOrEmpty(sUIStyle))
                    {
                        if (sUIStyle.ToLower().StartsWith("class:"))
                        {
                            sUIStyle = sUIStyle.Replace("class:", "");
                            sValue = "<span class=" + sUIStyle + ">" + sOptionValue + "</span>";
                        }
                        else if (sUIStyle.ToLower().StartsWith("html:"))
                        {
                            sUIStyle = sUIStyle.Replace("html:", "");
                            sValue = sUIStyle.Replace("{}", sOptionValue);
                        }
                        else if (sUIStyle.ToLower().StartsWith("img:"))
                        {
                            sUIStyle = sUIStyle.Replace("img:", "");
                            if (sUIStyle.Contains("{}"))
                            {
                                sValue = sUIStyle.Replace("{}", sOptionValue);
                            }
                            else
                            {
                                sValue = sUIStyle;
                            }
                        }
                        else
                        {
                            sValue = "<span class=" + sUIStyle + ">" + sOptionValue + "</span>";
                        }
                    }
                    else
                        sValue = sOptionValue;
                }
                else if (sValue == "0")
                {
                    sValue = "";
                }
            }
            return sValue;
        }
    }
}