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
using System.Web;
using Microsoft.VisualBasic;
using XISystem;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using XIDatabase;
using XIDataBase;
using xiEnumSystem;
using static XIDatabase.XIDBAPI;
using Microsoft.AspNet.SignalR.Messaging;
using System.Data.SqlTypes;

namespace XICore
{
    public class XIIBO : XIInstanceBase
    {
        public XIIBO() //Default constructor
        {
            TransactionIntiation = null;
        }
        public XIIBO(TransactionInitiation oTX)
        {
            this.TransactionIntiation = oTX;
        }
        public string sPrimaryKey { get; set; }
        public string sTableName { get; set; }
        [NonSerialized]
        public TransactionInitiation TransactionIntiation;
        public int iBODID { get; set; }
        public string sBOName { get; set; }
        public int iStructureID { get; set; }
        public int iInstanceID { get; set; }
        private XIIBOUI oMyBOUI;
        public Dictionary<string, XIIBO> ODictionaryList = new Dictionary<string, XIIBO>();
        public string sSubStructName { get; set; }
        public string sCoreDataBase { get; set; }
        public string sOrgDataBase { get; set; }
        public int iOrgId { get; set; }
        public List<XIDropDown> ImagePathDetails { get; set; }
        public CResult oCResult = new CResult();
        public List<XIIBO> oBOIList { get; set; }
        public List<string> DependentFields { get; set; }
        public string sShowID { get; set; }
        public string sErrorMessage { get; set; }
        public List<string> sIDs { get; set; }
        public List<string> sFields { get; set; }
        public bool bIsAudit = true;
        public bool bIsCreatedBy = false;
        public string sHierarchy { get; set; }
        public int iUserID { get; set; }
        public int iRoleID { get; set; }
        public string XIGUID { get; set; }
        public Guid BOIDXIGUID { get; set; }
        public Guid ParentXIGUID { get; set; }
        public Guid Parent1ClickGUID { get; set; }
        [DapperIgnore]
        public List<XIDropDown> oAttributeList { get; set; }
        public bool bIsEncrypted { get; set; }
        //public int iOneClickID { get; set; }
        //public XIIBO iBOI { get; set; }
        public XIIBOUI BOUII
        {
            get
            {
                return oMyBOUI;
            }
            set
            {
                oMyBOUI = value;
            }
        }
        //public SqlConnection SqlConn = null;
        //public SqlTransaction SqlTrans = null;

        public Dictionary<string, List<XIIBO>> oStructureInstance { get; set; }
        public List<CNV> NVPairs = new List<CNV>();
        public Dictionary<string, XIIValue> XIIValues = new Dictionary<string, XIIValue>();

        private Dictionary<string, XIIAttribute> oMyAttributes = new Dictionary<string, XIIAttribute>(StringComparer.CurrentCultureIgnoreCase);
        public iSiganlR oSignalR = null;
        public Dictionary<string, XIIAttribute> Attributes
        {
            get
            {
                return oMyAttributes;
            }
            set
            {
                oMyAttributes = value;
            }
        }
        public List<XIDBOAction> Actions { get; set; }

        public void Exclude(String sItems)
        {
            if (!string.IsNullOrEmpty(sItems))
            {
                // REMOVE ITEMS FROM DICTIONARY
                var lstitems = sItems.Split(',').Where(y => !string.IsNullOrEmpty(y)).Select(f => f.Trim()).ToList();
                lstitems = lstitems ?? new List<string>();
                foreach (var item in lstitems)
                {
                    if (oMyAttributes.ContainsKey(item))
                    {
                        oMyAttributes.Remove(item);
                    }
                }
            }
        }

        public XIIAttribute AttributeI(string sAttributeName)
        {
            XIIAttribute oThisAttrI = new XIIAttribute();/* TODO Change to default(_) if this is not a reference type */;
            //sAttributeName = sAttributeName.ToLower();
            //if(oMyAttributes.TryGetValue(sAttributeName,out oThisAttrI))
            //{
            //}
            if (oMyAttributes.ContainsKey(sAttributeName))
            {
                oThisAttrI = oMyAttributes[sAttributeName];
                oThisAttrI.BOI = this;
            }
            else if (oMyAttributes.ContainsKey(sAttributeName.ToLower()))
            {
                oThisAttrI = oMyAttributes[sAttributeName.ToLower()];
                oThisAttrI.BOI = this;
            }
            else
            {
            }

            return oThisAttrI;
        }

        private Dictionary<string, XIIClassAttr> oMyClassAttributes = new Dictionary<string, XIIClassAttr>();

        public Dictionary<string, XIIClassAttr> ClassAttributes
        {
            get
            {
                return oMyClassAttributes;
            }
            set
            {
                oMyClassAttributes = value;
            }
        }
        public XIIClassAttr ClassAttributeI(string sClassAttributeName)
        {
            XIIClassAttr oThisAttrI = null/* TODO Change to default(_) if this is not a reference type */;


            sClassAttributeName = sClassAttributeName.ToLower();


            if (oMyClassAttributes.ContainsKey(sClassAttributeName))
            {
            }
            else
            {
            }

            return oThisAttrI;
        }



        private Dictionary<string, XIIScript> oMyScripts = new Dictionary<string, XIIScript>();

        public Dictionary<string, XIIScript> Scripts
        {
            get
            {
                return oMyScripts;
            }
            set
            {
                oMyScripts = value;
            }
        }
        public XIIScript ScriptI(string sScriptName)
        {
            XIIScript oThisValueI = null/* TODO Change to default(_) if this is not a reference type */;


            sScriptName = sScriptName.ToLower();


            if (oMyScripts.ContainsKey(sScriptName))
            {
            }
            else
            {
            }

            return oThisValueI;
        }

        private Dictionary<string, XIIGroup> oMyGroups = new Dictionary<string, XIIGroup>();

        public Dictionary<string, XIIGroup> Groups
        {
            get
            {
                return oMyGroups;
            }
            set
            {
                oMyGroups = value;
            }
        }
        public XIIGroup GroupI(string sGroupName)
        {
            XIIGroup oThisAttrI = null/* TODO Change to default(_) if this is not a reference type */;


            sGroupName = sGroupName.ToLower();


            if (oMyGroups.ContainsKey(sGroupName))
            {
            }
            else
            {
            }

            return oThisAttrI;
        }


        private Dictionary<string, XIIStructure> oMyStructNodes = new Dictionary<string, XIIStructure>();

        public Dictionary<string, XIIStructure> StructNodes
        {
            get
            {
                return oMyStructNodes;
            }
            set
            {
                oMyStructNodes = value;
            }
        }
        public Dictionary<string, string> oScriptErrors { get; set; }
        public XIIStructure StructureI(string sValueName)
        {
            XIIStructure oThisValueI = null/* TODO Change to default(_) if this is not a reference type */;



            sValueName = sValueName.ToLower();

            if (oMyStructNodes.ContainsKey(sValueName))
            {

            }
            else
            {

            }

            return oThisValueI;
        }
        private List<XIDScript> oMyXIDScripts = new List<XIDScript>();
        public List<XIDScript> oXIDScripts
        {
            get
            {
                return oMyXIDScripts;
            }
            set
            {
                oMyXIDScripts = value;
            }
        }
        private List<XIDScriptResult> oMyScriptResults = new List<XIDScriptResult>();
        public List<XIDScriptResult> ScriptResults
        {
            get
            {
                return oMyScriptResults;
            }
            set
            {
                oMyScriptResults = value;
            }
        }
        public void LoadBOI(string sGroupName)
        {
            var oBOD = this.BOD;
            if (!string.IsNullOrEmpty(sGroupName))
            {
                List<XIDAttribute> attributes = new List<XIDAttribute>();
                var oGrpFields = sGroupName.ToLower().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var GrpField in oGrpFields)
                {
                    var GroupFields = oBOD.GroupD(GrpField.ToLower()).BOFieldNames;//oXIAPI.GetBOGroupFields(sBOName, sLockGroup, iUserID, sDatabase);
                    if (!string.IsNullOrEmpty(GroupFields))
                    {
                        var oGrpFieldNames = GroupFields.ToLower().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        //attributes = oBOD.Attributes.Values.Where(m => oGrpFields.Any(n => n == m.Name.ToLower())).ToList();

                        foreach (var item in oGrpFieldNames)
                        {
                            if (item.ToLower() == XIConstant.Key_XICrtdBy.ToLower() || item.ToLower() == XIConstant.Key_XICrtdWhn.ToLower() || item.ToLower() == XIConstant.Key_XIUpdtdBy.ToLower() || item.ToLower() == XIConstant.Key_XIUpdtdWhn.ToLower())
                            {
                                XIDAttribute oAttribute = new XIDAttribute();
                                oAttribute.Name = item;
                                attributes.Add(oAttribute);
                            }
                            else
                            {
                                GroupFields = oBOD.GroupD(item.ToLower()).BOFieldNames;
                                if (!string.IsNullOrEmpty(GroupFields))
                                {
                                    var oSubGrpFields = GroupFields.ToLower().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    foreach (var SubItem in oSubGrpFields)
                                    {
                                        attributes.Add(oBOD.Attributes.Values.Where(m => m.Name.ToLower() == SubItem).FirstOrDefault());
                                    }
                                }
                                else
                                {
                                    var AttrI = attributes.Where(m => m.Name.ToLower() == item).FirstOrDefault();
                                    if (AttrI == null)
                                    {
                                        attributes.Add(oBOD.Attributes.Values.Where(m => m.Name.ToLower() == item).FirstOrDefault());
                                    }
                                    else
                                    {

                                    }
                                }
                            }

                        }
                        //var attributes = oBOD.Attributes.Values.Where(m => oGrpFields.Any(n => n == m.Name.ToLower())).ToList();
                    }
                    if (oBOD.Attributes.Values.Where(m => m.Name.ToLower() == GrpField.ToLower()).FirstOrDefault() != null)
                        attributes.Add(oBOD.Attributes.Values.Where(m => m.Name.ToLower() == GrpField.ToLower()).FirstOrDefault());
                }
                var attrs = attributes.ToDictionary(i => i.Name.ToLower(), i => new XIIAttribute { sName = i.Name, Format = i.Format, sDefaultDate = i.sDefaultDate, iOneClickID = i.iOneClickID, sValue = i.DefaultValue, bDirty = true });
                Attributes = attrs;
            }
        }

        public XIDStructure Structure(string sValueName, string UserID = "")
        {
            CResult oCResult = new CResult();
            XIDStructure oXIDStructure = new XIDStructure();
            try
            {
                if (this.oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    XIInfraCache oCache = new XIInfraCache();
                    XIInfraUsers oUser = new XIInfraUsers();
                    if (!string.IsNullOrEmpty(UserID))
                        oUser.UserID = Convert.ToInt32(UserID);
                    if (oUser.UserID == 0 && HttpContext.Current != null)
                    {
                        if (HttpContext.Current.Session != null && HttpContext.Current.Session["UserID"] != null)
                        {
                            UserID = HttpContext.Current.Session["UserID"].ToString();
                            oUser.UserID = Convert.ToInt32(UserID);
                        }
                    }
                    string sCoreDB = System.Configuration.ConfigurationManager.AppSettings["CoreDataBase"];
                    var oInfo = oUser.Get_UserInfo(sCoreDB);
                    var sIDRef = "";
                    if (oInfo != null)
                    {
                        if (oInfo.iRoleID != 55)
                        {
                            sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, oInfo.iApplicationID + "_" + oInfo.iOrganizationID + "_" + oInfo.iRoleID + "_" + XIConstant.IDRef_internal);
                        }
                        else
                        {
                            sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, oInfo.iApplicationID + "_" + oInfo.iOrganizationID + "_" + XIConstant.IDRef_public);
                        }
                    }
                    int BOID = 0;
                    string sBOID = "";
                    if (this.BOD != null)
                    {
                        if (!string.IsNullOrEmpty(sIDRef) && sIDRef.ToLower() == "" && sIDRef == "xiguid" && this.BOD.XIGUID != null && this.BOD.XIGUID != Guid.Empty)
                        {
                            sBOID = this.BOD.XIGUID.ToString();
                            BOID = this.BOD.BOID;
                        }
                        else
                        {
                            sBOID = this.BOD.XIGUID.ToString();
                            BOID = this.BOD.BOID;
                        }
                    }
                    var BOI = this;
                    //XIDStructure oThisValueI = null/* TODO Change to default(_) if this is not a reference type */;
                    List<XIDStructure> oStructureDef = new List<XIDStructure>();
                    //XIInfraCache oCache = new XIInfraCache();
                    var oNodes = oCache.GetObjectFromCache(XIConstant.CacheStructure, sValueName, sBOID);
                    //var oNodes = oXIDStructure.GetXITreeStructure(BOID, sValueName);
                    if (oNodes != null)
                    {
                        oStructureDef = (List<XIDStructure>)oNodes;
                        oXIDStructure.oDefintion = oStructureDef;
                        oXIDStructure.BOID = BOID;
                        oXIDStructure.sBOID = sBOID;
                        oXIDStructure.BOI = BOI;
                        //XIBOInstance oStructureInstance = oXIDStructure.XILoad();
                        //Dictionary<string, List<XIIBO>> oSubStructureInstance = (Dictionary<string, List<XIIBO>>)oStructureInstance.oParent;
                        //oXIDStructure.oParent = oSubStructureInstance;
                        //oXIDStructure.oStructureInstance = oStructureInstance.oStructureInstance;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            return oXIDStructure;
        }

        public bool bHasChilds { get; set; }

        private Dictionary<string, List<XIIBO>> oSubChildI = new Dictionary<string, List<XIIBO>>(StringComparer.CurrentCultureIgnoreCase);

        public Dictionary<string, List<XIIBO>> SubChildI
        {
            get
            {
                return oSubChildI;
            }
            set
            {
                oSubChildI = value;
            }
        }

        private List<XIIBO> oFilteredData = new List<XIIBO>();

        public List<XIIBO> FilteredData
        {
            get
            {
                return oFilteredData;
            }
            set
            {
                oFilteredData = value;
            }
        }
        public Dictionary<string, List<XIIBO>> oBOStructure = new Dictionary<string, List<XIIBO>>();
        public List<XIIBO> oStructureI(string sStructureName)
        {
            List<XIIBO> oXI = null;
            if (oBOStructure.ContainsKey(sStructureName))
            {
                oXI = oBOStructure.Where(m => m.Key == sStructureName).Select(x => x.Value).FirstOrDefault();
            }
            else
            {
                oXI = new List<XIIBO>();
            }
            return oXI;
        }

        public XIIBO Get_BOInstance(string sBOName, string GroupName)
        {
            XIIBO oBOInstance = new XIIBO();
            CResult oResult = new CResult();
            XIDefinitionBase oXID = new XIDefinitionBase();
            try
            {
                if (!string.IsNullOrEmpty(sBOName))
                {
                    List<CNV> lSaveFields = new List<CNV>();
                    XIInfraCache oCache = new XIInfraCache();
                    //var sSessionID = HttpContext.Current.Session.SessionID;
                    var BODefinition = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);
                    //var BODefinition = Get_BODefinition(sBOName, iUserID, sOrgName, sDatabase);
                    if (!string.IsNullOrEmpty(GroupName))
                    {
                        var sFields = BODefinition.Groups.Where(m => m.Value.GroupName.ToLower() == GroupName.ToLower()).Select(m => m.Value.BOFieldNames).FirstOrDefault();
                        //lSaveFields = sFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(m => new CNV { sName = m }).ToList();
                        foreach (var field in sFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList())
                        {
                            oBOInstance.Attributes.Add(field, new XIIAttribute { sName = field, bDirty = true });
                        }
                    }
                    if (BODefinition != null)
                    {
                        oBOInstance.BOD = BODefinition;
                        if (!string.IsNullOrEmpty(GroupName))
                        {
                            //lSaveFields.ToList().ForEach(m => m.bDirty = true);
                            oBOInstance.NVPairs = lSaveFields;
                        }
                        else
                        {
                            foreach (var field in BODefinition.Attributes.Select(m => m.Value.Name).ToList())
                            {
                                oBOInstance.Attributes.Add(field, new XIIAttribute { sName = field, bDirty = true });
                            }
                            //List<CNV> Fields = BODefinition.Attributes.Select(m => new CNV { sName = m.Value.Name }).ToList();
                            //Fields.Where(m => lSaveFields.Any(n => n.sName == m.sName)).ToList().ForEach(c => c.bDirty = true);
                            //oBOInstance.NVPairs = Fields;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oBOInstance;
        }
        public CResult BuildBoObject(string BOName, string GroupName, Dictionary<string, CNV> oQSD, string ID = "", string sCoreDB = null, string sSessionID = null, string sEmail = null, string XIGUID = "")
        {
            CResult oResult = new CResult();
            XIDefinitionBase oXID = new XIDefinitionBase();
            try
            {
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                var oBOInsatnce = Get_BOInstance(BOName, GroupName);
                XIIXI oXIIXI = new XIIXI();
                foreach (var item in oBOInsatnce.Attributes.Values)
                {
                    if (oQSD.ContainsKey(item.sName))
                    {
                        item.sValue = oQSD[item.sName].sValue;
                        item.sPreviousValue = oQSD[item.sName].sPreviousValue;
                    }
                    if (item.sName == "sName")
                    {
                        var sFirstName = "";
                        var sLastName = "";
                        if (oQSD.ContainsKey("sFirstName"))
                        {
                            sFirstName = oQSD["sFirstName"].sValue;
                        }
                        if (oQSD.ContainsKey("sLastName"))
                        {
                            sLastName = oQSD["sLastName"].sValue;
                        }
                        item.sValue = sFirstName + " " + sLastName;
                    }
                    if (item.sName.ToLower() == oBOInsatnce.BOD.sPrimaryKey.ToLower() && !string.IsNullOrEmpty(ID))
                    {
                        item.sValue = ID;
                    }
                    if (item.sName.ToLower() == "xiguid" && !string.IsNullOrEmpty(XIGUID))
                    {
                        item.sValue = XIGUID;
                    }
                }
                //if (BOName.ToLower() == "lead_t")
                //{
                //    var sPK = oBOInsatnce.BOD.sPrimaryKey;
                //    var bType = oBOInsatnce.Attributes.Values.Where(m => m.sName.ToLower() == sPK.ToLower()).FirstOrDefault();
                //    if (bType != null && (bType.sValue == null || bType.sValue == "0"))
                //    {
                //        if (!oBOInsatnce.Attributes.ContainsKey("XICreatedWhen"))
                //        {
                //            oBOInsatnce.Attributes["XICreatedWhen"] = new XIIAttribute() { sName = "XICreatedWhen", bDirty = true };
                //        }
                //        if (!oBOInsatnce.Attributes.ContainsKey("XIUpdatedWhen"))
                //        {
                //            oBOInsatnce.Attributes["XIUpdatedWhen"] = new XIIAttribute() { sName = "XIUpdatedWhen", bDirty = true };
                //        }
                //    }
                //    else
                //    {
                //        if (!oBOInsatnce.Attributes.ContainsKey("XIUpdatedWhen"))
                //        {
                //            oBOInsatnce.Attributes["XIUpdatedWhen"] = new XIIAttribute() { sName = "XIUpdatedWhen", bDirty = true };
                //        }
                //    }
                //}
                var obj = Save(oBOInsatnce, true, sCoreDB, sSessionID, sEmail);
                if (obj.bOK && obj.oResult != null)
                {
                    oResult.oResult = obj;
                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oResult.oResult = obj;
                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }

            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oResult;
        }

        #region BULK_METHODS

        public CResult SaveBulk(DataTable dt, string DataSourceUID, string TableName)
        {
            CResult oResult = new CResult();
            try
            {
                XIDXI oXID = new XIDXI();
                string sConnection = oXID.GetBODataSource(DataSourceUID, 0);
                if (!string.IsNullOrEmpty(sConnection))
                {
                    using (SqlConnection _conn = new SqlConnection(sConnection))
                    {
                        _conn.Open();
                        using (SqlTransaction sqlTransaction = _conn.BeginTransaction())
                        {
                            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(_conn, SqlBulkCopyOptions.Default, sqlTransaction))
                            {
                                //Set the database table name
                                var columnNames = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                                foreach (var dtcolumn in columnNames)
                                {
                                    sqlBulkCopy.ColumnMappings.Add(dtcolumn, dtcolumn);
                                }

                                sqlBulkCopy.DestinationTableName = TableName;
                                //sqlBulkCopy.WriteToServer(dt);

                                try
                                {
                                    sqlBulkCopy.WriteToServer(dt);
                                    sqlTransaction.Commit();
                                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    oResult.oResult = "Success";
                                }
                                catch (Exception ex)
                                {
                                    oResult.sMessage = "ERROR SqlBulkCopy: [" + TableName + " " + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                                    oResult.LogToFile();
                                    SaveErrortoDB(oResult);
                                }
                            }
                        }
                        _conn.Close();
                        //        using (SqlBulkCopy bulk = new SqlBulkCopy(_conn))
                        //{
                        //    var columnNames = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                        //    foreach (var dtcolumn in columnNames)
                        //    {
                        //        bulk.ColumnMappings.Add(dtcolumn, dtcolumn);
                        //    }
                        //    _conn.Open();
                        //    bulk.DestinationTableName = TableName;
                        //    bulk.WriteToServer(dt);
                        //    _conn.Close();
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR SqlBulkCopy: [" + TableName + " " + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oResult;
        }

        private void AddDefaultColumns(DataTable dt)
        {
            XIInfraUsers oUser = new XIInfraUsers();
            CUserInfo oInfo = new CUserInfo();
            oInfo = oUser.Get_UserInfo();
            //oInfo = oInfo.GetUserInfo();
            if (dt.Columns.Contains(XIConstant.Key_XICrtdBy))
            {
                dt.Columns.Remove(XIConstant.Key_XICrtdBy);
            }
            if (dt.Columns.Contains(XIConstant.Key_XIUpdtdBy))
            {
                dt.Columns.Remove(XIConstant.Key_XIUpdtdBy);
            }
            if (dt.Columns.Contains(XIConstant.Key_XICrtdWhn))
            {
                dt.Columns.Remove(XIConstant.Key_XICrtdWhn);
            }
            if (dt.Columns.Contains(XIConstant.Key_XIUpdtdWhn))
            {
                dt.Columns.Remove(XIConstant.Key_XIUpdtdWhn);
            }
            dt.Columns.Add(XIConstant.Key_XICrtdBy, typeof(string)).DefaultValue = oInfo.sShortName;
            dt.Columns.Add(XIConstant.Key_XIUpdtdBy, typeof(string)).DefaultValue = oInfo.sShortName;
            dt.Columns.Add(XIConstant.Key_XICrtdWhn, typeof(DateTime)).DefaultValue = DateTime.Now;
            dt.Columns.Add(XIConstant.Key_XIUpdtdWhn, typeof(DateTime)).DefaultValue = DateTime.Now;
        }

        public DataTable MakeBulkSqlTable(List<XIIBO> xibo)
        {
            CResult oResult = new CResult();
            DataTable rturndt = new DataTable("bulk");
            xibo = xibo ?? new List<XIIBO>();
            try
            {
                if (xibo.Count() > 0)
                {
                    var BoDefination = xibo[0].Attributes.Where(f => f.Value.bDirty == true).Select(t => t.Value.sName).ToList();
                    var xiboDef = xibo[0].BOD;
                    DataColumn clmkey = null;
                    if (BoDefination != null)
                    {
                        // Build DataTable Headers here
                        foreach (var item in BoDefination)
                        {
                            int DataTypeID = xiboDef.AttributeD(item).TypeID;
                            if(item == XIConstant.Key_XICrtdWhn || item == XIConstant.Key_XIUpdtdWhn)
                            {
                                DataTypeID = 150;
                            }

                            if (xiboDef.bIsAutoIncrement == true && xiboDef.sPrimaryKey == item)
                            {
                                clmkey = new DataColumn();
                                clmkey.ColumnName = item;
                                clmkey.AutoIncrement = true;
                                clmkey.DataType = Utility.ConverttoType(DataTypeID);
                                rturndt.Columns.Add(clmkey);
                            }
                            else if (item != "zXCrtdWhn" && item != "zXUpdtdWhn")
                            {
                                DataColumn clm = new DataColumn();
                                clm.ColumnName = item;
                                clm.DataType = Utility.ConverttoType(DataTypeID);
                                clm.AllowDBNull = true;
                                rturndt.Columns.Add(clm);
                            }
                        }
                        // Add Default columns to the table here
                        // AddDefaultColumns(rturndt);
                        // primakey keys
                        if (clmkey != null)
                        {
                            DataColumn[] keys = new DataColumn[1];
                            keys[0] = clmkey;
                            rturndt.PrimaryKey = keys;
                        }
                        // Adding values
                        for (int j = 0; j < xibo.Count(); j++) // outer loop
                        {
                            var rowvalues = xibo[j].Attributes.Where(t => t.Value.bDirty == true);
                            DataRow dtrow = rturndt.NewRow();
                            foreach (var row in rowvalues) //Inner Loop
                            {
                                //rturndt.Rows[0][row.Value.sName] = row.Value.sValue;
                                int DataTypeID = xiboDef.AttributeD(row.Value.sName).TypeID;
                                if (row.Value.sName.ToLower() != "id" && dtrow.Table.Columns.Contains(row.Value.sName.Trim()))
                                {
                                    if (DataTypeID == Convert.ToInt32(BODatatypes.VARCHAR))
                                    {
                                        dtrow[row.Value.sName] = string.IsNullOrEmpty(row.Value.sValue) ? null : row.Value.sValue;
                                    }
                                    else if (DataTypeID == Convert.ToInt32(BODatatypes.DATETIME) && string.IsNullOrEmpty(row.Value.sValue))
                                    {
                                        dtrow[row.Value.sName] = DBNull.Value;
                                    }
                                    else if (DataTypeID != Convert.ToInt32(BODatatypes.UniqueIdentifier))
                                    {
                                        dtrow[row.Value.sName] = Utility.ConvertToExpected(row.Value.sValue, DataTypeID, row.Value.sName);//row.Value.sValue;
                                    }
                                    else if (DataTypeID == Convert.ToInt32(BODatatypes.UniqueIdentifier))
                                    {
                                        dtrow[row.Value.sName] = Utility.ConvertToExpected(row.Value.sValue, DataTypeID, row.Value.sName);
                                    }
                                    //dtrow[row.Value.sName] = row.Value.sValue;// (row.Value.sValue == "") ? null : row.Value.sValue;
                                }
                            }
                            rturndt.Rows.Add(dtrow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR SqlBulkCopy: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return rturndt;
        }

        #endregion
        public CResult Write_Activity(string sActivity, string sCategoryCode, int iCriticality = 0)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            //oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Save activity against Object and instance";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "sActivity", sValue = sActivity });
                if (!string.IsNullOrEmpty(sActivity))
                {
                    XIIXI oXI = new XIIXI();
                    XIIBO oBOI = new XIIBO();
                    int iCatID = 0;
                    var CatID = "";
                    XIInfraCache oCache = new XIInfraCache();
                    List<CNV> WhrParams = new List<CNV>();
                    if (!string.IsNullOrEmpty(sCategoryCode))
                    {
                        WhrParams.Add(new CNV { sName = "sCode", sValue = sCategoryCode });
                        var oCatI = oXI.BOI("refCategory", null, null, WhrParams);
                        if (oCatI != null && oCatI.Attributes.Count() > 0)
                        {
                            CatID = oCatI.AttributeI("ID").sValue;
                            int.TryParse(CatID, out iCatID);
                        }
                        else if (!string.IsNullOrEmpty(sCategoryCode))
                        {
                            XIIBO oCatBOI = new XIIBO();
                            oCatBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "refCategory");
                            oCatBOI.SetAttribute("sCode", sCategoryCode);
                            oCatBOI.SetAttribute("sName", sCategoryCode);
                            oCR = oCatBOI.Save(oCatBOI);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oCatBOI = (XIIBO)oCR.oResult;
                                CatID = oCatBOI.AttributeI("ID").sValue;
                                int.TryParse(CatID, out iCatID);
                            }
                            else
                            {

                            }
                        }
                    }
                    var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIActivity");
                    oBOI.BOD = BOD;
                    oBOI.SetAttribute("sDescription", sActivity);
                    oBOI.SetAttribute("FKiBODIDXIGUID", this.BOD.XIGUID.ToString());
                    var iBOIID = this.AttributeI("id").sValue;
                    oBOI.SetAttribute("FKiBOIID", iBOIID);
                    if (!string.IsNullOrEmpty(sCategoryCode))
                    {
                        oBOI.SetAttribute("FKsCategoryID", sCategoryCode);
                    }
                    if (iCatID > 0)
                    {
                        oBOI.SetAttribute("refCategoryID", iCatID.ToString());
                    }
                    oCR = oBOI.Save(oBOI);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = "Success";
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oTrace.sMessage = "Activity saving failed";
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sActivity is missing";
                }
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Save(XIIBO oBOI, bool bIsvalidate = true, string sCoreDB = null, string sSessionID = null, string sEmail = null, bool bAction = true, string context = "")
        {
            CResult oCResult = new CResult();
            CResult oResult = new CResult();
            string sPrimaryKey = string.Empty;
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                bool bContinue = true;
                //Check global configuration
                var sProtect = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "ObjectProtection");
                if (!string.IsNullOrEmpty(sProtect) && sProtect.ToLower() == "yes")
                {
                    if (oBOI.BOD != null && !string.IsNullOrEmpty(oBOI.BOD.Name))
                    {
                        var BoName = oBOI.BOD.Name;
                        XIDefinitionBase oBase = new XIDefinitionBase();
                        if (!String.IsNullOrEmpty(BoName) && ProtectedObject.isProtectedObject(BoName))
                        {
                            //check if calling method is in list or not.
                            if (!ProtectedObject.isInProtectedList(Assembly.GetCallingAssembly(), BoName))
                            {
                                // Get call stack
                                StackTrace stackTrace = new StackTrace();
                                // Get calling method name
                                var sMethodName = stackTrace.GetFrame(1).GetMethod().Name;
                                var methodInfo = new StackTrace().GetFrame(1).GetMethod();
                                var className = methodInfo.ReflectedType.Name;
                                //write to error log as it is protected object and method is not configured
                                oCResult = new CResult();
                                oCResult.sMessage = "Instance saving failed - Object: " + BoName + " has protection but its called from class: " + className + " and method: " + sMethodName;
                                oCResult.sCategory = "Object Protection";
                                oCResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                                oBase.SaveErrortoDB(oCResult);
                                //Console.WriteLine("not saving");
                                bContinue = false;
                            }
                        }
                        //bContinue = true;
                        //bool bContinue = Check_Protection(oBOI.BOD.Name);
                        if (!bContinue)
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            return oCResult;
                        }
                    }
                }
                bool bIsAllowed = false, bIsSave = true;
                XIInfraUsers oUser = new XIInfraUsers();
                int iOrgID = 0;
                int iAppID = 0;
                int iRoleID = 0;
                string sUserType = "internal";
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                if (oInfo != null && oInfo.iUserID > 0)
                {
                    if (oInfo.bDBAccess)
                    {
                        bIsAllowed = true;
                    }
                    else if (!string.IsNullOrEmpty(oInfo.sRoleName) && (oInfo.sRoleName.ToLower() == xiEnumSystem.EnumRoles.WebUsers.ToString().ToLower() || oInfo.sRoleName.ToLower() == xiEnumSystem.EnumRoles.Admin.ToString().ToLower()))
                    {
                        bIsAllowed = true;
                    }
                    iOrgID = oInfo.iOrganizationID;
                    iAppID = oInfo.iApplicationID;
                    iRoleID = oInfo.iRoleID;
                }
                else
                {
                    bIsAllowed = true;
                }
                bIsAllowed = true;
                //*************************Hierarchy*********************************************
                if (oBOI.BOD != null && !string.IsNullOrEmpty(oBOI.BOD.sPrimaryKey))
                    sPrimaryKey = oBOI.BOD.sPrimaryKey.ToLower();
                else
                    sPrimaryKey = "id";

                if (oBOI.BOD != null && oBOI.BOD.bIsHierarchy && oBOI.BOD.Attributes.Keys.Contains("sHierarchy"))
                {
                    if (oBOI.Attributes.Keys.Contains(sPrimaryKey) && oBOI.Attributes[sPrimaryKey].iValue > 0)
                    {
                        if (!string.IsNullOrEmpty(oUser.sUpdateHierarchy) && oUser.sUpdateHierarchy.Contains("|"))
                        //if (oUser.sUpdateHierarchy.Contains("|"))
                        {
                            foreach (string s in oUser.sUpdateHierarchy.Split('|'))
                            {
                                if (CompareHierarchy(oBOI.sHierarchy, s))
                                {
                                    bIsSave = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            bIsSave = CompareHierarchy(oBOI.sHierarchy, oUser.sUpdateHierarchy);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(oBOI.sHierarchy))
                            oBOI.SetAttribute("sHierarchy", oBOI.sHierarchy);
                        else if (!string.IsNullOrEmpty(oUser.sInsertDefaultCode))
                            oBOI.SetAttribute("sHierarchy", oUser.sInsertDefaultCode);
                        else if (!string.IsNullOrEmpty(oUser.sHierarchy))
                            oBOI.SetAttribute("sHierarchy", oUser.sHierarchy);
                    }
                }

                //check white list                
                var WhiteListCheck = System.Configuration.ConfigurationManager.AppSettings["WhitelistCheck"];
                if (WhiteListCheck == "yes" && oBOI.BOD.BOID != 2459 && oBOI.BOD.BOID != 762)
                {
                    string sOperation = string.Empty;
                    var PKValue = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue;
                    if (!string.IsNullOrEmpty(PKValue) && PKValue != "0")
                    {
                        sOperation = "update";
                    }
                    else
                    {
                        sOperation = "create";
                    }
                    if (oInfo.iUserID == 0)
                    {
                        if (HttpContext.Current != null && HttpContext.Current.Session != null)
                        {
                            var OrgID = HttpContext.Current.Session["OrganizationID"];
                            if (OrgID != null)
                                int.TryParse(OrgID.ToString(), out iOrgID);
                            var AppID = HttpContext.Current.Session["ApplicationID"];
                            if (AppID != null)
                                int.TryParse(AppID.ToString(), out iAppID);
                            var RoleID = HttpContext.Current.Session["iRoleID"];
                            if (RoleID != null)
                                int.TryParse(RoleID.ToString(), out iRoleID);
                            sUserType = "public";
                        }
                    }
                    var oCR = Check_Whitelist(oBOI.BOD.BOID, oInfo.iRoleID, iOrgID, iAppID, sOperation, oBOI.BOD.iLevel, oInfo.iLevel);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var bUNAuth = (bool)oCR.oResult;
                        if (bUNAuth)
                        {
                            bIsAllowed = false;
                        }
                    }
                }

                //*******************************************************************************

                if (bIsSave && bIsAllowed)
                {
                    //Check manadatory attributes with null or empty data, which shouldn't be saved to database
                    //var mandAttrs = oBOI.BOD.Attributes.Values.Where(m => m.bIsMandatory == true).ToList();
                    //if (mandAttrs != null && mandAttrs.Count() > 0)
                    //{
                    //    var Values = mandAttrs.Select(m => m.Name).ToList().Where(k => oBOI.Attributes.ContainsKey(k)).Select(k => oBOI.Attributes[k]).ToList();
                    //    var Empty = Values.Where(m => m.sValue == null || m.sValue == "").ToList();
                    //}
                    string sIDRef = string.Empty;
                    if (oInfo.iUserID == 0)
                    {
                        sIDRef = "id";
                    }
                    else if (sUserType == "internal")
                    {
                        sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, iAppID + "_" + iOrgID + "_" + iRoleID + "_" + XIConstant.IDRef_internal);
                    }
                    else if (sUserType == "public")
                    {
                        sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, iAppID + "_" + iOrgID + "_" + XIConstant.IDRef_public);
                    }
                    string sActionType = "create";
                    int iBOIID = 0;
                    Guid XIGUID = Guid.Empty;
                    iBOIID = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).iValue;
                    var sBOIGUID = oBOI.AttributeI("xiguid").sValue;
                    Guid.TryParse(sBOIGUID, out XIGUID);
                    if (iBOIID > 0 || (XIGUID != null && XIGUID != Guid.Empty))
                    {
                        sActionType = "update";
                    }
                    List<XIDScript> Scrpts = new List<XIDScript>();
                    //throw new NullReferenceException();
                    if (bIsvalidate && oBOI != null && oBOI.BOD != null && oBOI.BOD.Scripts.Values.Where(m => m.sType.ToLower() == "prepersist").Count() > 0)
                    {
                        Scrpts = RunScript(oBOI.BOD.Scripts.Values.Where(m => m.sType.ToLower() == "prepersist").ToList(), oBOI, sCoreDB, sSessionID, sEmail, sIDRef);
                    }
                    //Add new record to respective table  
                    //dbSet.Add(oBOInstance);
                    var MandatoryAttrs = oBOI.BOD.Attributes.Values.ToList().Where(m => m.bIsMandatory == true).ToList();
                    if (MandatoryAttrs != null && MandatoryAttrs.Count() > 0)
                    {
                        foreach (var attr in MandatoryAttrs)
                        {
                            if (oBOI.Attributes.ContainsKey(attr.Name))
                            {
                                var AttriVal = oBOI.Attributes[attr.Name].sValue;
                                if (string.IsNullOrEmpty(AttriVal))
                                {
                                    List<XIDScriptResult> ErrMsg = new List<XIDScriptResult>();
                                    ErrMsg.Add(new XIDScriptResult { sUserError = "Mandatory field " + attr.LabelName + " is not filled" });
                                    Scrpts.Add(new XIDScript { IsSuccess = false, sName = attr.Name, ScriptResults = ErrMsg });
                                }
                            }
                        }
                    }
                    if (Scrpts.Count() == 0 || Scrpts.Where(m => m.IsSuccess == false).Count() == 0)
                    {
                        if (oBOI.BOD != null && oBOI.BOD.Name != null && (oBOI.BOD.Name.ToLower() == "Organisations".ToLower() || oBOI.BOD.Name.ToLower() == "BO WhiteList".ToLower() || oBOI.BOD.Name.ToLower() == "XIAppRoles_AR_T".ToLower()))
                        {
                            int iApplicationID = 0;
                            var AppID = oBOI.AttributeI("FKiApplicationID").sValue;
                            int.TryParse(AppID, out iApplicationID);
                            if (iApplicationID > 0)
                            {
                                var oAPPD = (XIDApplication)oCache.GetObjectFromCache(XIConstant.CacheApplication, null, iApplicationID.ToString());
                                var sCoreDBName = oAPPD.sDatabaseName;
                                var DataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, sCoreDBName);
                                oBOI.BOD.iDataSource = DataSource.ID;
                                oBOI.BOD.iDataSourceXIGUID = DataSource.XIGUID;
                            }
                            else if (iAppID > 0)
                            {
                                var oAPPD = (XIDApplication)oCache.GetObjectFromCache(XIConstant.CacheApplication, null, iAppID.ToString());
                                var sCoreDBName = oAPPD.sDatabaseName;
                                var DataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, sCoreDBName);
                                oBOI.BOD.iDataSource = DataSource.ID;
                                oBOI.BOD.iDataSourceXIGUID = DataSource.XIGUID;
                            }
                        }
                        if (oBOI.BOD != null && (oBOI.BOD.TableName == "RefTraceStage_T" || oBOI.BOD.TableName == "refValidTrace_T" || oBOI.BOD.TableName == "refLeadQuality_T" || oBOI.BOD.TableName == "TraceTransactions_T"))
                        {
                            if (oInfo.sDatabaseName != null)
                            {
                                var DataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, oInfo.sDatabaseName);
                                oBOI.BOD.iDataSource = DataSource.ID;
                                oBOI.BOD.iDataSourceXIGUID = DataSource.XIGUID;
                            }
                        }
                        //Check versioning
                        if (sActionType == "update" && oBOI.BOD.XIVersion == 20)
                        {
                            string sUID = iBOIID > 0 ? iBOIID.ToString() : (XIGUID != Guid.Empty ? XIGUID.ToString() : Guid.Empty.ToString());
                            oCResult = Versionise(oBOI.BOD.XIGUID.ToString(), sUID);
                            if (oCResult.bOK && oCResult.oResult != null)
                            {
                                var iCurrentVersion = (int)oCResult.oResult;
                                var iNextVersion = iCurrentVersion + 1;
                                oBOI.SetAttribute("XIVersion", iNextVersion.ToString());
                            }
                            else
                            {

                            }
                        }
                        else if (sActionType == "create" && oBOI.BOD.XIVersion == 20)
                        {
                            oBOI.SetAttribute("XIVersion", "1");
                        }

                        //Run Pre Actions
                        if (oBOI.BOD != null && oBOI.BOD.Actions != null && oBOI.BOD.Actions.Count() > 0 && bAction)
                        {
                            foreach (var action in oBOI.BOD.Actions.Values.ToList().Where(m => m.iRunType == 10).ToList())
                            {
                                if (action.iType == 30 && (action.FKiAlgorithmID > 0 || (action.FKiAlgorithmIDXIGUID != null && action.FKiAlgorithmIDXIGUID != Guid.Empty)))
                                {
                                    var sGUID = Guid.NewGuid().ToString();
                                    XIDAlgorithm oAlogD = new XIDAlgorithm();
                                    XIInfraUsers oUsers = new XIInfraUsers();
                                    if (action.FKiAlgorithmIDXIGUID != null && action.FKiAlgorithmIDXIGUID != Guid.Empty)
                                    {
                                        oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, action.FKiAlgorithmIDXIGUID.ToString());
                                    }
                                    else if (action.FKiAlgorithmID > 0)
                                    {
                                        oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, action.FKiAlgorithmID.ToString());
                                    }
                                    List<CNV> oNVsList = new List<CNV>();
                                    oNVsList.Add(new CNV { sName = "-sActionType", sValue = sActionType });
                                    foreach (var items in oBOI.Attributes.Values.ToList())
                                    {
                                        oNVsList.Add(new CNV { sName = "-" + items.sName, sValue = items.sValue, sPreviousValue = items.sPreviousValue });
                                    }
                                    oCache.SetXIParams(oNVsList, sGUID, sSessionID);
                                    var oCR = oAlogD.Execute_XIAlgorithm(sSessionID, sGUID);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {

                                    }
                                    else
                                    {
                                        oCResult.sMessage = "ERROR: XIIBO:Save Method - BO Pre action:" + action.sName + " execution failed";
                                        oCResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                                        SaveErrortoDB(oCResult);
                                    }
                                    oCache.Clear_GuidCache(sSessionID, sGUID);
                                }
                            }
                        }

                        oResult = Update_TODB(oBOI, sIDRef);
                        oCResult = oResult;
                        if (oResult.bOK && oResult.oResult != null)
                        {
                            var PKValue = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue;
                            if (!string.IsNullOrEmpty(oBOI.BOD.sSearchType))
                            {
                                var oCR = Save_XIMeta(oBOI, oBOI.BOD.sSearchType);
                            }
                            //Run Post Actions
                            var ListofBoActions = new List<XIDBOAction>();
                            if (oBOI.BOD.Actions != null && oBOI.BOD.Actions.Count() > 0 && bAction)
                            {
                                if (!string.IsNullOrEmpty(context))
                                {
                                    ListofBoActions = oBOI.BOD.Actions.Values.ToList().Where(m => m.iRunType == 20 && m.sCode == context).ToList();
                                }
                                else { ListofBoActions = oBOI.BOD.Actions.Values.ToList().Where(m => m.iRunType == 20).ToList(); }

                                foreach (var action in ListofBoActions)
                                {
                                    if (action.iType == 30 && (action.FKiAlgorithmID > 0 || (action.FKiAlgorithmIDXIGUID != null && action.FKiAlgorithmIDXIGUID != Guid.Empty)))
                                    {
                                        var sGUID = Guid.NewGuid().ToString();
                                        XIDAlgorithm oAlogD = new XIDAlgorithm();
                                        XIInfraUsers oUsers = new XIInfraUsers();
                                        if (action.FKiAlgorithmIDXIGUID != null && action.FKiAlgorithmIDXIGUID != Guid.Empty)
                                        {
                                            oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, action.FKiAlgorithmIDXIGUID.ToString());
                                        }
                                        else if (action.FKiAlgorithmID > 0)
                                        {
                                            oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, action.FKiAlgorithmID.ToString());
                                        }
                                        List<CNV> oNVsList = new List<CNV>();
                                        oNVsList.Add(new CNV { sName = "-iBOIID", sValue = PKValue });
                                        oNVsList.Add(new CNV { sName = "-sBO", sValue = oBOI.BOD.Name });
                                        oNVsList.Add(new CNV { sName = "-sActionType", sValue = sActionType });
                                        foreach (var items in oBOI.Attributes.Values.ToList())
                                        {
                                            oNVsList.Add(new CNV { sName = "-" + items.sName, sValue = items.sValue, sPreviousValue = items.sPreviousValue });
                                        }
                                        oCache.SetXIParams(oNVsList, sGUID, sSessionID);
                                        var oCR = oAlogD.Execute_XIAlgorithm(sSessionID, sGUID);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {

                                        }
                                        else
                                        {
                                            oCResult.sMessage = "ERROR: XIIBO:Save Method - BO Post action:" + action.sName + " execution failed";
                                            oCResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                                            SaveErrortoDB(oCResult);
                                        }
                                        oCache.Clear_GuidCache(sSessionID, sGUID);
                                    }
                                    else if (action.iType == 100 && action.sMethodProject != null && action.sMethodURL != null && action.sMethodName != null)
                                    {
                                        Assembly exceutable;
                                        Type Ltype;
                                        object objclass;
                                        exceutable = Assembly.Load(action.sMethodProject);
                                        Ltype = exceutable.GetType(action.sMethodProject + "." + action.sMethodURL);
                                        objclass = Activator.CreateInstance(Ltype);
                                        MethodInfo method = Ltype.GetMethod(action.sMethodName);


                                        List<CNV> oNVParams = new List<CNV>();
                                        oNVParams.Add(new CNV { sName = "-sRunType", sValue = "Post" });
                                        oNVParams.Add(new CNV { sName = "-iBOIID", sValue = PKValue });
                                        if (oBOI.BOD.Name == "KPI")
                                        {
                                            if (string.IsNullOrEmpty(oBOI.AttributeI("fki1clickidxiguid").sValue))
                                            {
                                                oNVParams.Add(new CNV { sName = "ID", sValue = oBOI.AttributeI("kpilistid").sValue.ToString() });
                                            }
                                        }
                                        object[] parametersArray = new object[] { oNVParams };
                                        var Response = (object)method.Invoke(objclass, parametersArray);

                                        if (((CResult)Response).bOK && ((CResult)Response).oResult != null) { }
                                        else
                                        {
                                            oCResult.sMessage = "ERROR: XIIBO:Save Method - BO Post action:" + action.sName + " execution failed";
                                            oCResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                                            SaveErrortoDB(oCResult);
                                        }
                                    }
                                }
                            }
                            oBOI = (XIIBO)oResult.oResult;
                            oBOI.BOD.sScripts = Scrpts;
                            oBOI.Attributes.Values.ToList().ForEach(m => m.bDirty = false);
                            oCResult.oResult = oBOI;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            if (oBOI.BOD.BOID == 762)
                            {
                                var Type = oBOI.AttributeI("iCriticality").sValue;
                                if (Type == "30")
                                {
                                    XIDefinitionBase oDef = new XIDefinitionBase();
                                    oDef.XIMonitor(new CResult() { iType = 10 }, 76);
                                }
                                //if (string.IsNullOrEmpty(sSessionID))
                                //{
                                //    if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session.SessionID != null)
                                //    {
                                //        sSessionID = HttpContext.Current.Session.SessionID;
                                //    }
                                //}
                                //var sGUID = Guid.NewGuid().ToString();
                                //List<CNV> oNVsList = new List<CNV>();
                                //oNVsList.Add(new CNV { sName = "-iBOIID", sValue = oBOI.Attributes[oBOI.BOD.sPrimaryKey.ToLower()].sValue });
                                //oCache.SetXIParams(oNVsList, sGUID, sSessionID);
                                //XIDAlgorithm oAlogD = new XIDAlgorithm();
                                //oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, "9");
                                //oAlogD.Execute_XIAlgorithm(sSessionID, sGUID);
                            }
                            //Check tag code attributes
                            var TagCodeAttrs = oBOI.BOD.Attributes.Values.ToList().Where(m => m.TypeID == 260).ToList();
                            if (TagCodeAttrs != null && TagCodeAttrs.Count() > 0 && bAction)
                            {
                                foreach (var TagAttr in TagCodeAttrs)
                                {
                                    var TagAttrI = oBOI.BOD.AttributeD(TagAttr.Name);
                                    if (!string.IsNullOrEmpty(TagAttrI.DefaultValue))
                                    {
                                        var Spilts = TagAttrI.DefaultValue.Split('-').ToList();
                                        var sTagCode = string.Empty;
                                        var Values = new List<string>();
                                        bool bTagDone = true;
                                        foreach (var attr in Spilts)
                                        {
                                            var sValue = oBOI.AttributeI(attr).sResolvedValue;
                                            if (string.IsNullOrEmpty(sValue) || !bTagDone)
                                            {
                                                bTagDone = false;
                                            }
                                            Values.Add(sValue);
                                        }
                                        if (bTagDone)
                                        {
                                            sTagCode = string.Join("-", Values);
                                            oBOI.SetAttribute(TagAttr.Name, sTagCode);
                                            oBOI.Save(oBOI, true, null, null, null, false);
                                        }
                                    }
                                }
                            }
                            if (oSignalR != null)
                            {
                                SignalRUpdate(oBOI, iOrgID, oInfo, sActionType);
                            //    List<CNV> oNVs = new List<CNV>();
                            //    if (!string.IsNullOrEmpty(oBOI.BOD.sBroadcastGroup))
                            //    {
                            //        var sigRAttrs = new List<string>();
                            //        var oGroupD = oBOI.BOD.GroupD(oBOI.BOD.sBroadcastGroup);
                            //        if (oGroupD != null && oGroupD.XIGUID != null && oGroupD.XIGUID != Guid.Empty)
                            //        {
                            //            sigRAttrs = oGroupD.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            //        }
                            //        else
                            //        {
                            //            sigRAttrs = oBOI.BOD.sBroadcastGroup.Split(',').ToList();
                            //        }
                            //        if (sigRAttrs != null && sigRAttrs.Count() > 0)
                            //        {
                            //            foreach (var attr in sigRAttrs)
                            //            {
                            //                var PrevValue = oBOI.AttributeI(attr).sPreviousResolvedValue;
                            //                var CurrentValue = oBOI.AttributeI(attr).sResolvedValue;
                            //                if (PrevValue != CurrentValue)
                            //                {
                            //                    oNVs.Add(new CNV { sName = attr, sValue = CurrentValue, sType = "AttrBroadcast" });
                            //                }
                            //            }
                            //        }
                            //        if (iOrgID == 0 && oInfo != null)
                            //        {
                            //            iOrgID = Convert.ToInt32(oBOI.AttributeI("FKiOrgID").sValue);
                            //        }
                            //    }
                            //    var oBOSignalR = (List<XISignalRsettings>)oCache.GetObjectFromCache(XIConstant.CacheBOSignalR, null, oBOI.BOD.XIGUID.ToString());
                            //    if (oBOSignalR != null && oBOSignalR.Count() > 0)
                            //    {
                            //        var oConfigs = oBOSignalR.Where(m => m.iType == 10).ToList();
                            //        foreach (var items in oConfigs)
                            //        {
                            //            if (!string.IsNullOrEmpty(items.sTargetArea))
                            //            {
                            //                var sTarget = items.sTargetArea;
                            //                string sRegex = "\\[(.*?)\\]";
                            //                var matches = Regex.Matches(sTarget, sRegex);
                            //                foreach (Match m in matches)
                            //                {
                            //                    var sAttr = m.ToString().Replace("[", "").Replace("]", "");
                            //                    var sValue = oBOI.AttributeI(sAttr).sValue;
                            //                    sTarget = sTarget.Replace("[" + m.ToString() + "]", sValue);
                            //                }
                            //                if (items.FKiTemplateIDXIGUID != null && items.FKiTemplateIDXIGUID != Guid.Empty)
                            //                {
                            //                    XIIXI oXII = new XIIXI();
                            //                    var oStructObj = new XIBOInstance();
                            //                    XIContentEditors oConentED = new XIContentEditors();
                            //                    var oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, items.FKiTemplateIDXIGUID.ToString());
                            //                    if (!string.IsNullOrEmpty(items.sStructureCode))
                            //                    {
                            //                        oStructObj = oXII.BOI(oBOI.BOD.Name, oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue).Structure(items.sStructureCode).XILoad();
                            //                    }
                            //                    else
                            //                    {
                            //                        List<XIIBO> nBOI = new List<XIIBO>();
                            //                        nBOI.Add(oBOI);
                            //                        oStructObj.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                            //                        oStructObj.oStructureInstance[oBOI.BOD.Name.ToLower()] = nBOI;
                            //                    }
                            //                    var Result = oConentED.MergeContentTemplate(oContentDef.FirstOrDefault(), oStructObj);
                            //                    oNVs.Add(new CNV { sName = sTarget, sValue = (string)Result.oResult, sType = "TargetBroadcast" });
                            //                }
                            //                else
                            //                {
                            //                    var sigRAttrs = new List<string>();
                            //                    var oGroupD = oBOI.BOD.GroupD(items.sSelectedFields);
                            //                    if (oGroupD != null && oGroupD.XIGUID != null && oGroupD.XIGUID != Guid.Empty)
                            //                    {
                            //                        sigRAttrs = oGroupD.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            //                    }
                            //                    else
                            //                    {
                            //                        sigRAttrs = items.sSelectedFields.Split(',').ToList();
                            //                    }
                            //                    if (sigRAttrs != null && sigRAttrs.Count() > 0)
                            //                    {
                            //                        foreach (var attr in sigRAttrs)
                            //                        {
                            //                            var PrevValue = oBOI.AttributeI(attr).sPreviousResolvedValue;
                            //                            var CurrentValue = oBOI.AttributeI(attr).sResolvedValue;
                            //                            if (PrevValue != CurrentValue)
                            //                            {
                            //                                oNVs.Add(new CNV { sName = sTarget, sValue = CurrentValue, sType = "TargetAttrBroadcast" });
                            //                            }
                            //                        }
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //    if (!string.IsNullOrEmpty(sActionType) && sActionType.ToLower() == "create")
                            //    {
                            //        //1Querys in Menus, tabs, kpi: 1Query definitions will have different attributes
                            //        //Get all the 1Queries assigned for UI(How to get them?) and build the new instance object and call signalR method
                            //        if (!string.IsNullOrEmpty(oBOI.BOD.sBroadcastGroup))
                            //        {
                            //            dynamic oSigRResponse = new { BOName = oBOI.BOD.Name, BODID = oBOI.BOD.XIGUID, BOIID = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue };
                            //            oSignalR.ListAddRefresh(oSigRResponse, iOrgID.ToString());
                            //        }
                            //    }else if (oNVs != null && oNVs.Count() > 0) // signal either full row or cell update
                            //    {
                            //        dynamic oSigRResponse = new { BOName = oBOI.BOD.Name, BODID = oBOI.BOD.XIGUID, BOIID = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue, HTML = "Data Inserted", Action = "Insert", BOSerialisation = "", Security = "", oNVs = oNVs };
                            //        oSignalR.ListRefresh(oSigRResponse, iOrgID.ToString());
                            //    }
                            }
                        }
                    }
                    else
                    {
                        oBOI.BOD.sScripts = Scrpts;
                        oCResult.oResult = oBOI;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        return oCResult;
                    }
                    if (bIsvalidate && oBOI != null && oBOI.BOD != null && oBOI.BOD.Scripts.Values.Where(m => m.sType.ToLower() == "postpersist" && m.StatusTypeID == 0).Count() > 0)
                    {
                        //for all attributes
                        //XIIXI oXIIXI = new XIIXI();
                        //var iInstanceID = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue;
                        //var oFullBOI = oXIIXI.BOI(oBOI.BOD.Name, iInstanceID);
                        Scrpts = RunScript(oBOI.BOD.Scripts.Values.Where(m => m.sType.ToLower() == "postpersist").ToList(), oBOI, sCoreDB, sSessionID, sEmail, sIDRef);
                    }
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.oResult = oBOI;
                }
            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                oResult.LogToFile();
                SaveErrortoDB(oResult);
            }
            return oCResult;
        }

        private void SignalRUpdate(XIIBO oBOI, int iOrgID, CUserInfo oInfo, string sActionType = null)
        {
            XIInfraCache oCache = new XIInfraCache();
            List<CNV> oNVs = new List<CNV>();
            if (!string.IsNullOrEmpty(oBOI.BOD.sBroadcastGroup))
            {
                var sigRAttrs = new List<string>();
                var oGroupD = oBOI.BOD.GroupD(oBOI.BOD.sBroadcastGroup);
                if (oGroupD != null && oGroupD.XIGUID != null && oGroupD.XIGUID != Guid.Empty)
                {
                    sigRAttrs = oGroupD.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    sigRAttrs = oBOI.BOD.sBroadcastGroup.Split(',').ToList();
                }
                if (sigRAttrs != null && sigRAttrs.Count() > 0)
                {
                    foreach (var attr in sigRAttrs)
                    {
                        var PrevValue = oBOI.AttributeI(attr).sPreviousResolvedValue;
                        var CurrentValue = oBOI.AttributeI(attr).sResolvedValue;
                        if (PrevValue != CurrentValue)
                        {
                            oNVs.Add(new CNV { sName = attr, sValue = CurrentValue, sType = "AttrBroadcast" });
                        }
                    }
                }
                if (iOrgID == 0 && oInfo != null)
                {
                    iOrgID = Convert.ToInt32(oBOI.AttributeI("FKiOrgID").sValue);
                }
            }
            var oBOSignalR = (List<XISignalRsettings>)oCache.GetObjectFromCache(XIConstant.CacheBOSignalR, null, oBOI.BOD.XIGUID.ToString());
            if (oBOSignalR != null && oBOSignalR.Count() > 0)
            {
                var oConfigs = oBOSignalR.Where(m => m.iType == 10).ToList();
                foreach (var items in oConfigs)
                {
                    if (!string.IsNullOrEmpty(items.sTargetArea))
                    {
                        var sTarget = items.sTargetArea;
                        string sRegex = "\\[(.*?)\\]";
                        var matches = Regex.Matches(sTarget, sRegex);
                        foreach (Match m in matches)
                        {
                            var sAttr = m.ToString().Replace("[", "").Replace("]", "");
                            var sValue = oBOI.AttributeI(sAttr).sValue;
                            sTarget = sTarget.Replace("[" + m.ToString() + "]", sValue);
                        }
                        if (items.FKiTemplateIDXIGUID != null && items.FKiTemplateIDXIGUID != Guid.Empty)
                        {
                            XIIXI oXII = new XIIXI();
                            var oStructObj = new XIBOInstance();
                            XIContentEditors oConentED = new XIContentEditors();
                            var oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, items.FKiTemplateIDXIGUID.ToString());
                            if (!string.IsNullOrEmpty(items.sStructureCode))
                            {
                                                    if (items.sStructureCode.Contains('_'))
                                                    {
                                                        var sFKBO = string.Empty;
                                                        var StrDetails = items.sStructureCode.Split('_').ToList();
                                                        var sFKAttr = StrDetails[0].Replace("[", "").Replace("]", "");
                                                        var FKAttrIVal = oBOI.AttributeI(sFKAttr).sValue;
                                                        var FKAttrD = oBOI.BOD.AttributeD(sFKAttr);
                                                        if (FKAttrD != null)
                                                        {
                                                            sFKBO = FKAttrD.sFKBOName;
                                                        }
                                                        if (!string.IsNullOrEmpty(sFKBO) && !string.IsNullOrEmpty(FKAttrIVal))
                                                        {
                                                            oStructObj = oXII.BOI(sFKBO, FKAttrIVal).Structure(StrDetails[1]).XILoad();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        oStructObj = oXII.BOI(oBOI.BOD.Name, oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue).Structure(items.sStructureCode).XILoad();
                                                    }
                             }
                            else
                            {
                                List<XIIBO> nBOI = new List<XIIBO>();
                                nBOI.Add(oBOI);
                                oStructObj.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                                oStructObj.oStructureInstance[oBOI.BOD.Name.ToLower()] = nBOI;
                            }
                            var Result = oConentED.MergeContentTemplate(oContentDef.FirstOrDefault(), oStructObj);
                            oNVs.Add(new CNV { sName = sTarget, sValue = (string)Result.oResult, sType = "TargetBroadcast" });
                        }
                        else
                        {
                            var sigRAttrs = new List<string>();
                            var oGroupD = oBOI.BOD.GroupD(items.sSelectedFields);
                            if (oGroupD != null && oGroupD.XIGUID != null && oGroupD.XIGUID != Guid.Empty)
                            {
                                sigRAttrs = oGroupD.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            }
                            else
                            {
                                sigRAttrs = items.sSelectedFields.Split(',').ToList();
                            }
                            if (sigRAttrs != null && sigRAttrs.Count() > 0)
                            {
                                foreach (var attr in sigRAttrs)
                                {
                                    var PrevValue = oBOI.AttributeI(attr).sPreviousResolvedValue;
                                    var CurrentValue = oBOI.AttributeI(attr).sResolvedValue;
                                    if (PrevValue != CurrentValue)
                                    {
                                        oNVs.Add(new CNV { sName = sTarget, sValue = CurrentValue, sType = "TargetAttrBroadcast" });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(sActionType) && sActionType.ToLower() == "create")
            {
                //1Querys in Menus, tabs, kpi: 1Query definitions will have different attributes
                //Get all the 1Queries assigned for UI(How to get them?) and build the new instance object and call signalR method
                if (!string.IsNullOrEmpty(oBOI.BOD.sBroadcastGroup))
                {
                    dynamic oSigRResponse = new { BOName = oBOI.BOD.Name, BODID = oBOI.BOD.XIGUID, BOIID = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue };
                    oSignalR.ListAddRefresh(oSigRResponse, iOrgID.ToString());
                }
            }
            else if (oNVs != null && oNVs.Count() > 0) // signal either full row or cell update
            {
                dynamic oSigRResponse = new { BOName = oBOI.BOD.Name, BODID = oBOI.BOD.XIGUID, BOIID = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue, HTML = "Data Inserted", Action = "Insert", BOSerialisation = "", Security = "", oNVs = oNVs };
                oSignalR.ListRefresh(oSigRResponse, iOrgID.ToString());
            }
        }

        public CResult SaveV2(XIIBO oBOI, bool bIsvalidate = true)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            string sPrimaryKey = string.Empty;
            try
            {
                oTrace.sClass = "XIIBO";
                oTrace.sMethod = "Save";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                bool bIsAllowed = false, bIsSave = true;
                XIInfraCache oCache = new XIInfraCache();
                XIInfraUsers oUser = new XIInfraUsers();
                int iOrgID = 0;
                int iAppID = 0;
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                if (oInfo != null && oInfo.iUserID > 0)
                {
                    if (oInfo.bDBAccess)
                    {
                        bIsAllowed = true;
                    }
                    else if (!string.IsNullOrEmpty(oInfo.sRoleName) && (oInfo.sRoleName.ToLower() == xiEnumSystem.EnumRoles.WebUsers.ToString().ToLower() || oInfo.sRoleName.ToLower() == xiEnumSystem.EnumRoles.Admin.ToString().ToLower()))
                    {
                        bIsAllowed = true;
                    }
                    iOrgID = oInfo.iOrganizationID;
                    iAppID = oInfo.iApplicationID;
                }
                else
                {
                    bIsAllowed = true;
                }
                bIsAllowed = true;
                //*************************Hierarchy*********************************************
                if (!string.IsNullOrEmpty(oBOI.BOD.sPrimaryKey))
                    sPrimaryKey = oBOI.BOD.sPrimaryKey.ToLower();
                else
                    sPrimaryKey = "id";

                if (oBOI.BOD.bIsHierarchy && oBOI.BOD.Attributes.Keys.Contains("sHierarchy"))
                {
                    if (oBOI.Attributes.Keys.Contains(sPrimaryKey) && oBOI.Attributes[sPrimaryKey].iValue > 0)
                    {
                        if (!string.IsNullOrEmpty(oUser.sUpdateHierarchy) && oUser.sUpdateHierarchy.Contains("|"))
                        //if (oUser.sUpdateHierarchy.Contains("|"))
                        {
                            foreach (string s in oUser.sUpdateHierarchy.Split('|'))
                            {
                                if (CompareHierarchy(oBOI.sHierarchy, s))
                                {
                                    bIsSave = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            bIsSave = CompareHierarchy(oBOI.sHierarchy, oUser.sUpdateHierarchy);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(oBOI.sHierarchy))
                            oBOI.SetAttribute("sHierarchy", oBOI.sHierarchy);
                        else if (!string.IsNullOrEmpty(oUser.sInsertDefaultCode))
                            oBOI.SetAttribute("sHierarchy", oUser.sInsertDefaultCode);
                        else if (!string.IsNullOrEmpty(oUser.sHierarchy))
                            oBOI.SetAttribute("sHierarchy", oUser.sHierarchy);
                    }
                }
                //check white list

                var WhiteListCheck = System.Configuration.ConfigurationManager.AppSettings["WhitelistCheck"];
                if (WhiteListCheck == "yes")
                {
                    string sOperation = string.Empty;
                    var PKValue = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue;
                    if (!string.IsNullOrEmpty(PKValue) && PKValue != "0")
                    {
                        sOperation = "update";
                    }
                    else
                    {
                        sOperation = "create";
                    }
                    if (oInfo.iUserID == 0)
                    {
                        if (HttpContext.Current != null && HttpContext.Current.Session != null)
                        {
                            var OrgID = HttpContext.Current.Session["OrganizationID"];
                            if (OrgID != null)
                                int.TryParse(OrgID.ToString(), out iOrgID);
                            var AppID = HttpContext.Current.Session["ApplicationID"];
                            if (AppID != null)
                                int.TryParse(AppID.ToString(), out iAppID);
                        }
                    }
                    oCR = Check_Whitelist(oBOI.BOD.BOID, oInfo.iRoleID, iOrgID, iAppID, sOperation, oBOI.BOD.iLevel, oInfo.iLevel);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var bUNAuth = (bool)oCR.oResult;
                        if (bUNAuth)
                        {
                            bIsAllowed = false;
                        }
                    }
                }


                //*******************************************************************************

                if (bIsSave && bIsAllowed)
                {
                    //Check manadatory attributes with null or empty data, which shouldn't be saved to database
                    //var mandAttrs = oBOI.BOD.Attributes.Values.Where(m => m.bIsMandatory == true).ToList();
                    //if (mandAttrs != null && mandAttrs.Count() > 0)
                    //{
                    //    var Values = mandAttrs.Select(m => m.Name).ToList().Where(k => oBOI.Attributes.ContainsKey(k)).Select(k => oBOI.Attributes[k]).ToList();
                    //    var Empty = Values.Where(m => m.sValue == null || m.sValue == "").ToList();
                    //}

                    List<XIDScript> Scrpts = new List<XIDScript>();
                    //throw new NullReferenceException();
                    if (bIsvalidate && oBOI != null && oBOI.BOD.Scripts.Values.Where(m => m.sType.ToLower() == "prepersist").Count() > 0)
                    {
                        Scrpts = RunScript(oBOI.BOD.Scripts.Values.Where(m => m.sType.ToLower() == "prepersist").ToList(), oBOI);
                    }
                    //Add new record to respective table  
                    //dbSet.Add(oBOInstance);

                    if (Scrpts.Count() == 0 || Scrpts.Where(m => m.IsSuccess == false).Count() == 0)
                    {
                        oCR = Update_TODBV2(oBOI);
                        oTrace.oTrace.Add(oCR.oTrace);
                        oCResult = oCR;
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oBOI = (XIIBO)oCR.oResult;
                            oBOI.BOD.sScripts = Scrpts;
                            oBOI.Attributes.Values.ToList().ForEach(m => m.bDirty = false);
                            if (!string.IsNullOrEmpty(oBOI.BOD.sType) && oBOI.BOD.sType.ToLower() == "xisystem")
                            {
                                oCache.Remove_ConfigCache(oBOI);
                            }
                            oCResult.oResult = oBOI;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.oResult = "No Access";
                        }
                    }
                    else
                    {
                        oBOI.BOD.sScripts = Scrpts;
                        oCResult.oResult = oBOI;
                        //oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        return oCResult;
                    }
                    if (bIsvalidate && oBOI != null && oBOI.BOD.Scripts.Values.Where(m => m.sType.ToLower() == "postpersist" && m.StatusTypeID == 0).Count() > 0)
                    {
                        //for all attributes
                        //XIIXI oXIIXI = new XIIXI();
                        //var iInstanceID = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue;
                        //var oFullBOI = oXIIXI.BOI(oBOI.BOD.Name, iInstanceID);
                        Scrpts = RunScript(oBOI.BOD.Scripts.Values.Where(m => m.sType.ToLower() == "postpersist").ToList(), oBOI);
                    }
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.oResult = "No Access";
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oTrace.sMessage = ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        //Compare User and FormComponent Hierarchy for insert and update
        private bool CompareHierarchy(string sHierarchy, string sUserHierarchy)
        {
            if (!string.IsNullOrEmpty(sHierarchy) && !string.IsNullOrEmpty(sUserHierarchy) && sHierarchy.Contains(sUserHierarchy))
            {
                if (sHierarchy != sUserHierarchy)
                {
                    string sSubHierachy = sHierarchy.Substring(sUserHierarchy.Length, 1);
                    if (!string.IsNullOrEmpty(sSubHierachy) && (sSubHierachy != "_"))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private Dictionary<string, XIIAttribute> AssignDefaultValues(XIIBO oBOInstance)
        {
            //Dictionary<string, XIIAttribute> nAttr = new Dictionary<string, XIIAttribute>();
            XIInfraUsers oUser = new XIInfraUsers();
            CUserInfo oInfo = new CUserInfo();
            oInfo = oUser.Get_UserInfo();
            //oInfo = oInfo.GetUserInfo();
            XIIAttribute oAttr = new XIIAttribute();
            oAttr.sName = XIConstant.Key_XICrtdBy;
            oAttr.sValue = oInfo.sName;
            oAttr.bDirty = true;
            //oAttr.bIsAutoSet = true;
            oBOInstance.Attributes.Add(oAttr.sName, oAttr);
            oAttr = new XIIAttribute();
            oAttr.sName = XIConstant.Key_XICrtdWhn;
            oAttr.sValue = DateTime.Now.ToString();
            oAttr.bDirty = true;
            //oAttr.bIsAutoSet = true;            
            oBOInstance.Attributes.Add(oAttr.sName, oAttr);
            oAttr = new XIIAttribute();
            oAttr.sName = XIConstant.Key_XIUpdtdBy;
            oAttr.sValue = oInfo.sName;
            oAttr.bDirty = true;
            //oAttr.bIsAutoSet = true;           
            oBOInstance.Attributes.Add(oAttr.sName, oAttr);
            oAttr = new XIIAttribute();
            oAttr.sName = XIConstant.Key_XIUpdtdWhn;
            oAttr.sValue = DateTime.Now.ToString();
            oAttr.bDirty = true;
            //oAttr.bIsAutoSet = true;
            oBOInstance.Attributes.Add(oAttr.sName, oAttr);
            foreach (var Attr in oBOInstance.Attributes)
            {
                if (string.IsNullOrEmpty(Attr.Value.sValue))
                {
                    var sDefaultValue = oBOInstance.BOD.Attributes.Values.Where(x => x.Name.ToLower() == Attr.Value.sName.ToLower()).Select(x => x.DefaultValue).FirstOrDefault();
                    Attr.Value.sValue = sDefaultValue;
                }
            }
            return oBOInstance.Attributes;
        }
        public CResult Delete(XIIBO oBOI)
        {
            CResult oCResult = new CResult();
            oCResult.oResult = false;
            string sPrimaryKey = string.Empty;
            string sCondition = string.Empty;
            int i = 0;
            string sTableName = oBOI.BOD.TableName;
            XIDXI oXID = new XIDXI();
            string sBODataSource = oXID.GetBODataSource(oBOI.BOD.iDataSourceXIGUID.ToString(), oBOI.BOD.FKiApplicationID);
            XIInfraUsers oUser = new XIInfraUsers();
            XIInfraCache oCache = new XIInfraCache();
            string sIDRef = string.Empty;
            CUserInfo oInfo = new CUserInfo();
            oInfo = oUser.Get_UserInfo();
            if (oInfo != null && oInfo.iUserID > 0)
            {
                iOrgId = oInfo.iOrganizationID;
                var iAppID = oInfo.iApplicationID;
                iRoleID = oInfo.iRoleID;
                if (oInfo.iRoleID != 55)
                {
                    sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, iAppID + "_" + iOrgId + "_" + iRoleID + "_" + XIConstant.IDRef_internal);
                }
                else
                {
                    sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, iAppID + "_" + iOrgId + "_" + XIConstant.IDRef_public);
                }
            }


            if (oBOI.BOD.bUID && (string.IsNullOrEmpty(sIDRef) || (!string.IsNullOrEmpty(sIDRef) && sIDRef.ToLower() == "xiguid")))
            {
                sPrimaryKey = "xiguid";
                if (oBOI.Attributes["xiguid"] != null)
                {
                    sCondition = sPrimaryKey + " = '" + oBOI.Attributes["xiguid"].sValue + "'";
                }
            }
            else
            {
                sPrimaryKey = oBOI.BOD.sPrimaryKey;
                string ID = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == sPrimaryKey.ToLower()).Select(m => m.sValue).FirstOrDefault();
                sCondition = sPrimaryKey + "=" + ID;
            }

            if (!string.IsNullOrEmpty(sTableName) && !string.IsNullOrEmpty(sCondition))
            {
                using (SqlConnection Conn = new SqlConnection(sBODataSource))
                {
                    Conn.Open();
                    string sQuery = string.Empty;
                    //string sQuery = "DELETE " + sTableName + " WHERE " + sCondition;
                    if (oBOI.BOD.Attributes.ContainsKey("iStatus"))
                    {
                        sQuery = "UPDATE " + sTableName + " set " + XIConstant.Key_XIDeleted + " = 1, iStatus=50 WHERE " + sCondition;
                    }
                    else
                    {
                        sQuery = "UPDATE " + sTableName + " set " + XIConstant.Key_XIDeleted + " = 1 WHERE " + sCondition;
                    }

                    SqlCommand cmd = new SqlCommand(sQuery, Conn);
                    i = cmd.ExecuteNonQuery();
                    if (i == 1)
                    {
                        oCResult.oResult = true;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        if (oSignalR != null)
                        {
                            SignalRUpdate(oBOI, iOrgId, oInfo, null);
                        }
                    }
                    else
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                }
            }
            return oCResult;
        }

        public List<XIDScript> RunScript(List<XIDScript> lScripts, XIIBO oBOInstance, string sCoreDB = null, string sSessionID = null, string sEmail = null, string sIDRef = null)
        {
            string sPrimaryKey = sIDRef;
            if (string.IsNullOrEmpty(sPrimaryKey))
            {
                sPrimaryKey = oBOInstance.BOD.sPrimaryKey;
            }
            foreach (var script in lScripts)
            {
                //if (!script.bRun)
                //{
                if (script.sLanguage.ToLower() == "regular expressions")
                {
                    string sRuntimescript = script.sScript;
                    var srtScritMerged = MergeScript(sRuntimescript, oBOInstance);
                    var sMergedScript = srtScritMerged[1];
                    if (oBOInstance.Attributes.ContainsKey(srtScritMerged[0].ToLower()) && oBOInstance.Attributes[srtScritMerged[0].ToLower()].bDirty)
                    {
                        var oAttrD = oBOInstance.BOD.Attributes.Values.Where(m => m.Name.ToLower() == srtScritMerged[0].ToLower()).FirstOrDefault();
                        //Example: [a-zA-Z0-9]{BO.sMob}
                        string sRegex = "";
                        var pattern = @"\<(.*?)\>";
                        var matches = Regex.Matches(sRuntimescript, pattern);
                        foreach (Match m in matches)
                        {
                            sRegex = m.ToString().Replace("<", "").Replace(">", "");
                        }
                        var sValue = sMergedScript.Split('>')[1];
                        if (oAttrD.TypeID == 150)
                        {
                            oAttrD.sMinDate = oAttrD.sMinResolvedValue;
                            oAttrD.sMaxDate = oAttrD.sMaxResolvedValue;
                            if (!string.IsNullOrEmpty(sValue))
                            {
                                string sScript = sMergedScript.Split('>')[0];
                                var Datepattern = @"\[(.*?)\]";
                                var matches1 = Regex.Matches(sScript, Datepattern);
                                foreach (Match m in matches1)
                                {
                                    string sDateScript = m.ToString().Replace("[", "").Replace("]", "");
                                    var sMinDate = oAttrD.sMinDate;
                                    var sMaxDate = oAttrD.sMaxDate;
                                    //string sModelminDateName = "get_" + sDateScript.Split('-')[0];
                                    //var method1 = ((object)oAttrD).GetType().GetMethod(sModelminDateName);
                                    //var sMinDate = (string)method1.Invoke(oAttrD, null);
                                    //string sModelmaxDateName = "get_" + sDateScript.Split('-')[1];
                                    //var method2 = ((object)oAttrD).GetType().GetMethod(sModelmaxDateName);
                                    //var sMaxDate = (string)method2.Invoke(oAttrD, null);
                                    if (string.IsNullOrEmpty(sMinDate) && string.IsNullOrEmpty(sMaxDate))
                                    {
                                        script.IsSuccess = true;
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(sMinDate) && !string.IsNullOrEmpty(sMaxDate))
                                        {
                                            DateTime dStartDate = ConvertToDtTime(sMinDate);
                                            DateTime sEndDate = ConvertToDtTime(sMaxDate);
                                            DateTime dInputDate = ConvertToDtTime(sValue);
                                            if (dInputDate >= dStartDate && dInputDate <= sEndDate)
                                            {
                                                script.IsSuccess = true;
                                            }
                                            else
                                            {
                                                FailedBoFieldValidation(oBOInstance.sBOName, oAttrD.Name, sValue);
                                                script.IsSuccess = false;
                                            }
                                        }
                                        else if (!string.IsNullOrEmpty(sMinDate))
                                        {
                                            DateTime dStartDate = ConvertToDtTime(sMinDate);
                                            DateTime dInputDate = ConvertToDtTime(sValue);
                                            if (dInputDate >= dStartDate)
                                            {
                                                script.IsSuccess = true;
                                            }
                                            else
                                            {
                                                FailedBoFieldValidation(oBOInstance.sBOName, oAttrD.Name, sValue);
                                                script.IsSuccess = false;
                                            }
                                        }
                                        else if (!string.IsNullOrEmpty(sMaxDate))
                                        {
                                            DateTime sEndDate = ConvertToDtTime(sMaxDate);
                                            DateTime dInputDate = ConvertToDtTime(sValue);
                                            if (dInputDate <= sEndDate)
                                            {
                                                script.IsSuccess = true;
                                            }
                                            else
                                            {
                                                FailedBoFieldValidation(oBOInstance.sBOName, oAttrD.Name, sValue);
                                                script.IsSuccess = false;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                FailedBoFieldValidation(oBOInstance.sBOName, oAttrD.Name, sValue);
                                script.IsSuccess = false;
                            }
                        }
                        else if (oAttrD.FKiType > 0 || oAttrD.IsOptionList)
                        {
                            if (!string.IsNullOrEmpty(sValue))
                            {
                                string sScript = sMergedScript.Split('>')[0];
                                sScript = sScript.TrimStart('<');
                                if (!string.IsNullOrEmpty(sScript))
                                {
                                    var oScriptArray = sScript.Split('[');
                                    //sScript = sScript.Replace("[", "").Replace("]","");
                                    if (oScriptArray != null && oScriptArray.Count() > 0)
                                    {
                                        var sConditionValue = oScriptArray[1];
                                        sConditionValue = !string.IsNullOrEmpty(sConditionValue) ? sConditionValue.Replace("]", "") : "";
                                        if (!string.IsNullOrEmpty(sConditionValue))
                                        {
                                            var sScriptCondition = oScriptArray[0];
                                            switch (sScriptCondition)
                                            {
                                                case "ne":
                                                case "<>":
                                                case "!=":
                                                    if (sValue != sConditionValue)
                                                    {
                                                        script.IsSuccess = true;
                                                    }
                                                    else
                                                    {
                                                        FailedBoFieldValidation(oBOInstance.sBOName, oAttrD.Name, sValue);
                                                        script.IsSuccess = false;
                                                    }
                                                    break;
                                                case "eq":
                                                case "==":
                                                    if (sValue == sConditionValue)
                                                    {
                                                        script.IsSuccess = true;
                                                    }
                                                    else
                                                    {
                                                        FailedBoFieldValidation(oBOInstance.sBOName, oAttrD.Name, sValue);
                                                        script.IsSuccess = false;
                                                    }
                                                    break;
                                                case ">":
                                                case "gt":
                                                    if (Convert.ToInt32(sValue) > Convert.ToInt32(sConditionValue))
                                                    {
                                                        script.IsSuccess = true;
                                                    }
                                                    else
                                                    {
                                                        FailedBoFieldValidation(oBOInstance.sBOName, oAttrD.Name, sValue);
                                                        script.IsSuccess = false;
                                                    }
                                                    break;
                                                case "<":
                                                case "lt":
                                                    if (Convert.ToInt32(sValue) < Convert.ToInt32(sConditionValue))
                                                    {
                                                        script.IsSuccess = true;
                                                    }
                                                    else
                                                    {
                                                        FailedBoFieldValidation(oBOInstance.sBOName, oAttrD.Name, sValue);
                                                        script.IsSuccess = false;
                                                    }
                                                    break;
                                                case ">=":
                                                case "gteq":
                                                    if (Convert.ToInt32(sValue) >= Convert.ToInt32(sConditionValue))
                                                    {
                                                        script.IsSuccess = true;
                                                    }
                                                    else
                                                    {
                                                        FailedBoFieldValidation(oBOInstance.sBOName, oAttrD.Name, sValue);
                                                        script.IsSuccess = false;
                                                    }
                                                    break;
                                                case "<=":
                                                case "lteq":
                                                    if (Convert.ToInt32(sValue) <= Convert.ToInt32(sConditionValue))
                                                    {
                                                        script.IsSuccess = true;
                                                    }
                                                    else
                                                    {
                                                        FailedBoFieldValidation(oBOInstance.sBOName, oAttrD.Name, sValue);
                                                        script.IsSuccess = false;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                FailedBoFieldValidation(oBOInstance.sBOName, oAttrD.Name, sValue);
                                script.IsSuccess = false;
                            }
                        }
                        else if (sRegex.ToLower().StartsWith("compare"))
                        {
                            script.IsSuccess = false;
                            if (sRegex.Contains('/'))
                            {
                                var ParentAttr = sRegex.Split('/')[1];
                                var ParentVal = string.Empty;
                                if (oBOInstance.Attributes.ContainsKey(ParentAttr.ToLower()))
                                {
                                    ParentVal = oBOInstance.Attributes[ParentAttr.ToLower()].sValue;
                                    if (!string.IsNullOrEmpty(ParentVal) && !string.IsNullOrEmpty(sValue))
                                    {
                                        List<CNV> oParams = new List<CNV>();
                                        oParams.Add(new CNV { sName = XIConstant.Param_ParentAttrVal, sValue = ParentVal });
                                        oParams.Add(new CNV { sName = XIConstant.Param_ChildAttrVal, sValue = sValue });
                                        var oCR = Validate_Compare(oParams);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            bool bIsEqual = (bool)oCR.oResult;
                                            if (bIsEqual)
                                            {
                                                script.IsSuccess = true; ;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (sRegex.ToLower().StartsWith("unique"))
                        {
                            script.IsSuccess = true;
                            if (sRegex.Contains('/'))
                            {
                                var sAttrName = sRegex.Split('/')[1];
                                var sAttrVal = sValue;
                                if (!string.IsNullOrEmpty(sAttrName) && !string.IsNullOrEmpty(sAttrVal))
                                {
                                    string sPK = string.Empty;
                                    var PKValue = oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == sPrimaryKey.ToLower()).FirstOrDefault();
                                    if (PKValue != null)
                                    {
                                        sPK = PKValue.sValue;
                                    }
                                    //var sPreviousValue = oBOInstance.BOI.Attributes.Values.Where(m => m.sName.ToLower() == srtScritMerged[0].ToLower()).FirstOrDefault().sPreviousValue;
                                    List<CNV> oParams = new List<CNV>();
                                    oParams.Add(new CNV { sName = XIConstant.Param_BODID, sValue = oBOInstance.BOD.BOID.ToString() });
                                    oParams.Add(new CNV { sName = XIConstant.Param_AttrName, sValue = sAttrName });
                                    oParams.Add(new CNV { sName = XIConstant.Param_AttrValue, sValue = sAttrVal });
                                    oParams.Add(new CNV { sName = XIConstant.Param_BOIID, sValue = sPK });
                                    oParams.Add(new CNV { sName = "sIDRef", sValue = sIDRef });
                                    //oParams.Add(new CNV { sName = XIConstant.Param_PreviousValue, sValue = sPreviousValue });
                                    var oCR = Validate_Unique(oParams);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        bool bIsExists = (bool)oCR.oResult;
                                        if (bIsExists)
                                        {
                                            script.IsSuccess = false;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var sScrptRegex = new Regex(sRegex);
                            if (!string.IsNullOrEmpty(sValue.ToString()) && sScrptRegex.IsMatch(sValue.ToString()))
                            {
                                //if (sScrptRegex.IsMatch(sValue.ToString()))
                                //{
                                //Valid
                                script.IsSuccess = true;
                                // }

                            }
                            else
                            {
                                FailedBoFieldValidation(oBOInstance.sBOName, oAttrD.Name, sValue);
                                //Not Valid
                                script.IsSuccess = false;
                            }

                        }
                    }
                    else
                    {
                        script.IsSuccess = true;
                    }
                    script.sFieldName = srtScritMerged[0];
                }
                else if (script.sLanguage.ToLower() == "xiscript")
                {
                    string sScriptresult = string.Empty;
                    XIInfraCache oCache = new XIInfraCache();
                    string sGUID = Guid.NewGuid().ToString();
                    string sScriptString = script.sScript.Substring(2, script.sScript.Length - 4);
                    if (sScriptString.StartsWith("<xi.s"))
                    {
                        if (sScriptString.Contains("}>{"))
                        {
                            CNV oParam = new CNV();
                            List<CNV> oNVList = new List<CNV>();
                            int iScriptColumnLastIndex = sScriptString.LastIndexOf(">");
                            string sScriptColumn = sScriptString.Substring(iScriptColumnLastIndex).Replace(">", "");
                            sScriptColumn = sScriptColumn.Replace("{", "").Replace("}", "");
                            string sScriptBOName = sScriptColumn.Split('.')[0];
                            sScriptColumn = sScriptColumn.Split('.')[1];
                            if (oBOInstance.Attributes.ContainsKey(sScriptColumn.ToLower()) && oBOInstance.Attributes[sScriptColumn.ToLower()].bDirty)
                            {
                                string sInstnaceID = string.Empty;
                                sInstnaceID = oBOInstance.AttributeI(sScriptColumn).sValue;
                                string sPreviousValue = oBOInstance.AttributeI(sScriptColumn).sPreviousValue;
                                if (sInstnaceID != sPreviousValue)
                                {
                                    oNVList.Add(new CNV { sName = "sScript", sValue = "" });
                                    oNVList.Add(new CNV { sName = "-" + sScriptColumn, sValue = sInstnaceID });
                                    //oParam.sName = "sScript";
                                    //oParam.sValue = "";
                                    //oNVList.Add(oParam);
                                    //oParam = new CNV();
                                    //oParam.sName = "-" + sScriptColumn;
                                    //oParam.sValue = sInstnaceID;
                                    //oNVList.Add(oParam);
                                    oCache.SetXIParams(oNVList, sGUID, "");
                                    int iCollectionIndex = sScriptString.IndexOf(">");
                                    int iStartPosi = sScriptString.LastIndexOf("<", iCollectionIndex);
                                    int iStringLength = ">".Length;
                                    string sScript = sScriptString.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength);
                                    sScript = sScript.Replace("<", "").Replace(">", "");
                                    XIDScript oXIScript = new XIDScript();
                                    oXIScript.sScript = sScript.ToString();
                                    var oResult = oXIScript.Execute_Script(sGUID, "");
                                    if (oResult.bOK && oResult.oResult != null)
                                    {
                                        sScriptresult = (string)oResult.oResult;
                                        if (!string.IsNullOrEmpty(sScriptresult) && sInstnaceID != sScriptresult)
                                        {
                                            XIInfraNotifications oNotifications = new XIInfraNotifications();
                                            int iDocID = 0;
                                            if (oBOInstance.Attributes.ContainsKey("fkizxdoc"))
                                            {
                                                if (int.TryParse(oBOInstance.AttributeI("fkizxdoc").sValue, out iDocID))
                                                {
                                                }
                                            }
                                            if (iDocID != 0)
                                            {
                                                XIIXI oIXI = new XIIXI();
                                                List<XIIBO> nBOI = new List<XIIBO>();
                                                XIContentEditors oContent = new XIContentEditors();
                                                var oNotificationContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, sScriptresult, "0");
                                                if (oNotificationContentDef != null && oNotificationContentDef.Count() > 0)
                                                {
                                                    string sUserId = string.Empty; string sInstanceID = string.Empty;
                                                    var oNotificationContent = oNotificationContentDef.FirstOrDefault();
                                                    if (oNotificationContent != null)
                                                    {
                                                        var oDocumentBOI = oIXI.BOI("Documents_T", iDocID.ToString());
                                                        nBOI.Add(oDocumentBOI);
                                                        oNotifications.iStatus = 10;
                                                        XIBOInstance oBOIns = new XIBOInstance();
                                                        oBOIns.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                                                        oBOIns.oStructureInstance[oDocumentBOI.BOD.Name.ToLower()] = nBOI;
                                                        var Result = oContent.MergeContentTemplate(oNotificationContent, oBOIns);
                                                        if (Result.bOK && Result.oResult != null)
                                                        {
                                                            string sOrgID = Convert.ToString(oNotificationContent.OrganizationID);
                                                            string sSubject = oNotificationContent.sSubject;
                                                            string sMessage = (string)Result.oResult;
                                                            if (oBOInstance.Attributes.ContainsKey("fkiuserid"))
                                                            {
                                                                sUserId = oBOInstance.AttributeI("fkiuserid").sValue;
                                                            }
                                                            if (oBOInstance.Attributes.ContainsKey("fkipolicyid"))
                                                            {
                                                                sInstanceID = oBOInstance.AttributeI("fkipolicyid").sValue;
                                                            }

                                                            oNotifications.iStatus = 20;
                                                            oNotifications.Create(sUserId, oNotificationContent.Name, iDocID.ToString(), sSubject, sMessage, sInstanceID, sOrgID);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                script.IsSuccess = true;
                            }
                        }
                        else
                        {
                            XIInfraUsers oUser = new XIInfraUsers();
                            oUser.sUserName = "";
                            CUserInfo oInfo = new CUserInfo();
                            oUser.sUserName = string.IsNullOrEmpty(sEmail) ? "" : sEmail;
                            oInfo = oUser.Get_UserInfo(sCoreDB);
                            if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session.SessionID != null)
                            {
                                sSessionID = HttpContext.Current.Session.SessionID;
                            }
                            List<CNV> oNVsList = new List<CNV>();
                            oNVsList.Add(new CNV { sName = "-sBO", sValue = oBOInstance.BOD.Name });
                            var sPrimarykeychecking = oBOInstance.Attributes.Where(m => m.Key.ToLower() == sPrimaryKey.ToLower()).Select(m => m.Value.sValue).FirstOrDefault();
                            oNVsList.Add(new CNV { sName = "-instanceid", sValue = sPrimarykeychecking /*oBOInstance.Attributes[sPrimaryKey.ToLower()].sValue*/ });
                            oNVsList.Add(new CNV { sName = "sRole", sValue = oInfo.sRoleName });
                            oNVsList.Add(new CNV { sName = "sActionType", sValue = "Save" });
                            if (oBOInstance.BOD.Name.ToLower() == "Organisations".ToLower())
                            {
                                oNVsList.Add(new CNV { sName = "FKiAppID", sValue = oBOInstance.Attributes["fkiapplicationid"].sValue });
                            }
                            oCache.SetXIParams(oNVsList, sGUID, sSessionID);
                            XIDScript oXIScript = new XIDScript();
                            sScriptString = sScriptString.Replace("<", "").Replace(">", "");
                            oXIScript.sScript = sScriptString.ToString();
                            XIInfraMenuComponent oMenu = new XIInfraMenuComponent();
                            string sResultForTrace = string.Empty;
                            CResult oCR = new CResult();
                            if (script.FKiBOAttributeID > 0)
                            {
                                var oAttrD = oBOInstance.BOD.Attributes.Values.Where(m => m.ID == script.FKiBOAttributeID).FirstOrDefault();
                                if (oAttrD != null)
                                {
                                    var oAttrI = oBOInstance.Attributes.Values.Where(m => m.sName == oAttrD.Name).FirstOrDefault();
                                    if (oAttrI != null)
                                    {
                                        oNVsList = new List<CNV>();
                                        oNVsList.Add(new CNV { sName = "-" + oAttrD.Name, sValue = oAttrI.sValue });
                                        oNVsList.Add(new CNV { sName = "-" + oAttrD.Name + "_previous", sValue = oAttrI.sPreviousValue });
                                        oCache.SetXIParams(oNVsList, sGUID, sSessionID);
                                        if (oAttrI != null && oAttrI.sValue != oAttrI.sPreviousValue && !string.IsNullOrEmpty(script.sExecutionType) && script.sExecutionType.ToLower() == "ondifferentvalue")
                                        {
                                            oCR = oMenu.RunScript(sScriptString, sGUID, sSessionID);

                                        }
                                        if (oAttrI != null && !string.IsNullOrEmpty(script.sExecutionType) && script.sExecutionType.ToLower() == "everytime")
                                        {
                                            if (sScriptString.ToLower().Contains("{bo."))
                                            {
                                            repeat:
                                                int iCollectionIndex = sScriptString.IndexOf("{BO.");
                                                int iStartPosi = sScriptString.IndexOf("}", iCollectionIndex);
                                                int iStringLength = "{BO.".Length;
                                                string sScript = sScriptString.Substring(iCollectionIndex + iStringLength, iStartPosi - (iCollectionIndex + iStringLength));
                                                var value = oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == sScript.ToLower()).Select(t => t.sValue).FirstOrDefault();

                                                sScriptString = sScriptString.Replace("BO." + sScript, value);
                                                if (sScriptString.ToLower().Contains("{bo."))
                                                {
                                                    goto repeat;
                                                }
                                            }
                                            if (sScriptString.ToLower().Contains(",bo."))
                                            {
                                            repeat1:
                                                int iCollectionIndex = sScriptString.IndexOf(",BO.");
                                                int iStartPosi = sScriptString.IndexOf("}", iCollectionIndex);
                                                int iStringLength = ",BO.".Length;
                                                string sScript = sScriptString.Substring(iCollectionIndex + iStringLength, iStartPosi - (iCollectionIndex + iStringLength));
                                                var value = oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == sScript.ToLower()).Select(t => t.sValue).FirstOrDefault();

                                                sScriptString = sScriptString.Replace("BO." + sScript, value);
                                                if (sScriptString.ToLower().Contains(",bo."))
                                                {
                                                    goto repeat1;
                                                }
                                            }
                                            oCR = oMenu.RunScript(sScriptString, sGUID, sSessionID);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                oCR = oMenu.RunScript(sScriptString, sGUID, sSessionID);
                            }
                            sResultForTrace = oCR.oResult == null ? "" : oCR.oResult.ToString();
                            //sResultForTarce is used to restrict the changing of status in Lead update if it is invalid based on xiscript
                            if (sResultForTrace.ToLower() == "invalidtrace" || sResultForTrace.ToLower() == "false")
                                script.IsSuccess = false;
                            else
                                script.IsSuccess = true;
                        }
                    }
                }
                else
                {
                    script.IsSuccess = true;
                }

            }
            return lScripts;
        }
        public void FailedBoFieldValidation(string sBo, string sField, string sValue)
        {
            CResult oResult = new CResult();
            oResult.sMessage = "ERROR: [Field Validation failed: [" + "Bo: " + sBo + " - Field: " + sField + " - FieldValue: " + sValue + "]";
            oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
            oResult.LogToFile();
            SaveErrortoDB(oResult);
        }

        public List<string> MergeScript(string sRuntimescript, XIIBO oBOInstance)
        {
            List<string> Field = new List<string>();
            Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(sRuntimescript);
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    if (match.ToString().IndexOf('.') > 0)
                    {
                        var sProperty = match.ToString();
                        var sName = match.ToString().Split('.')[1];
                        Field.Add(sName);
                        var sValue = oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == sName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                        sRuntimescript = sRuntimescript.ToString().Replace("{" + sProperty + "}", sValue);
                        Field.Add(sRuntimescript);
                    }
                }
            }
            return Field;
        }

        string sFinalQuery = string.Empty;

        private string Execute_TransactionEnabledQuery(string resolvedQuery, bool Isinsert)
        {
            string iInstanceID = "0";
            CResult oCResult = new CResult();
            if (TransactionIntiation != null)
            {
                using (SqlCommand command = new SqlCommand(resolvedQuery, TransactionIntiation.TXSqlconn, TransactionIntiation.TXSqltrans))
                {
                    try
                    {
                        if (TransactionIntiation.TXSqlconn.State == ConnectionState.Closed)
                        {
                            TransactionIntiation.TXSqlconn.Open();
                        }
                        if (Isinsert)
                        {
                            iInstanceID = command.ExecuteScalar().ToString();
                        }
                        else
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sCategory = "DB query error";
                        SaveErrortoDB(oCResult);
                        //oCResult.sMessage = "Error Query " + sFinalQuery;
                        //oCResult.sCategory = "DB query error";
                        //SaveErrortoDB(oCResult);
                        oCResult.LogToFile();
                        iInstanceID = "-1";
                        try
                        {
                            TransactionIntiation.TXSqltrans.Rollback();
                        }
                        catch (Exception iex)
                        {
                            oCResult.sMessage = "ERROR: ROLLBACK [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + iex.Message + " - Trace: " + iex.StackTrace + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sCategory = "DB query error";
                            SaveErrortoDB(oCResult);
                            //oCResult.sMessage = "Error Query " + sFinalQuery;
                            //oCResult.sCategory = "DB query error";
                            //SaveErrortoDB(oCResult);
                            oCResult.LogToFile();
                            iInstanceID = "-1";
                        }
                        // TransactionIntiation.TXSqlconn.Close();
                    }
                }

            }
            return iInstanceID;
        }

        #region TRANSACTION_ENABLED QUERY COMMIT AND ROLLBAKC
        private string Execute_TransactionEnabledQuery_OLD(string resolvedQuery, string iDataSource, bool Isinsert)
        {
            string iInstanceID = "0";
            string sBODataSource = string.Empty;
            XIDXI oXID = new XIDXI();
            sBODataSource = oXID.GetBODataSource(iDataSource, 0);
            if (!string.IsNullOrEmpty(sBODataSource))
            {
                using (SqlConnection conn = new SqlConnection(sBODataSource))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        using (SqlCommand command = new SqlCommand(resolvedQuery, conn, transaction))
                        {
                            try
                            {
                                if (Isinsert)
                                {
                                    iInstanceID = command.ExecuteScalar().ToString();
                                    transaction.Commit();
                                }
                                else
                                {
                                    command.ExecuteNonQuery();
                                    transaction.Commit();
                                }
                                conn.Close();
                            }
                            catch (Exception ex)
                            {
                                CResult oCResult = new CResult();
                                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                SaveErrortoDB(oCResult);
                                oCResult.sMessage = "Error Query " + sFinalQuery;
                                SaveErrortoDB(oCResult);
                                oCResult.LogToFile();
                                try
                                {
                                    transaction.Rollback();
                                }
                                catch (Exception iex) // Log Again for Rollback issue
                                {
                                    oCResult.sMessage = "ERROR Occured When doing Rollback, DatasourceID :" + iDataSource + " [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + iex.Message + " - Trace: " + iex.StackTrace + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    SaveErrortoDB(oCResult);
                                    oCResult.sMessage = "Error Query " + sFinalQuery;
                                    SaveErrortoDB(oCResult);
                                }
                            }
                        }
                    }
                }
            }
            else // LOG DTATABASE CONNECTION STRING NOT THERE
            {
                CResult oCResult = new CResult();
                oCResult.sMessage = "Log: Connection String not there for DatasourceID: " + iDataSource;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.sMessage = "Log Query " + sFinalQuery;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
            }
            return iInstanceID;
        }
        #endregion

        #region NORMAL QUERY EXECUTION

        private string Execute_NormalQuery(string resolvedQuery, string iDataSource, bool Isinsert)
        {
            string iInstanceID = "0";
            string sBODataSource = string.Empty;
            XIDXI oXID = new XIDXI();
            sBODataSource = oXID.GetBODataSource(iDataSource, 0);
            if (!string.IsNullOrEmpty(sBODataSource))
            {
                using (SqlConnection conn = new SqlConnection(sBODataSource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(resolvedQuery, conn))
                    {
                        try
                        {
                            if (Isinsert)
                            {
                                iInstanceID = command.ExecuteScalar().ToString();
                            }
                            else
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {

                            CResult oCResult = new CResult();
                            oCResult.sMessage = "Critical ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sCategory = "DB query error";
                            SaveErrortoDB(oCResult);
                            //oCResult.sMessage = "Error Query " + sFinalQuery;
                            //oCResult.sCategory = "DB query error";
                            //SaveErrortoDB(oCResult);
                            oCResult.LogToFile();
                            conn.Close();
                            iInstanceID = "-1";
                        }
                    }
                    conn.Close();
                }
            }
            else // LOG DTATABASE CONNECTION STRING NOT THERE
            {
                CResult oCResult = new CResult();
                oCResult.sMessage = "Log: Connection String not there for DatasourceID: " + iDataSource;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                //oCResult.sMessage = "Log Query " + sFinalQuery;
                //SaveErrortoDB(oCResult);
                oCResult.LogToFile();
                iInstanceID = "-1";
            }
            return iInstanceID;
        }

        private CResult Execute_NormalQueryV2(string resolvedQuery, string iDataSource, bool Isinsert)
        {
            CResult oCResult = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = "XIIBO";
            oTrace.sMethod = "Execute_NormalQuery";
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            string iInstanceID = "0";
            string sBODataSource = string.Empty;
            XIDXI oXID = new XIDXI();
            sBODataSource = oXID.GetBODataSource(iDataSource, 0);
            if (!string.IsNullOrEmpty(sBODataSource))
            {
                using (SqlConnection conn = new SqlConnection(sBODataSource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(resolvedQuery, conn))
                    {
                        try
                        {
                            oTrace.sQuery = resolvedQuery;
                            if (Isinsert)
                            {
                                iInstanceID = command.ExecuteScalar().ToString();
                                oCResult.oResult = iInstanceID;
                            }
                            else
                            {
                                command.ExecuteNonQuery();
                                oCResult.oResult = "Success";
                            }
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        catch (Exception ex)
                        {

                            oCResult = new CResult();
                            oCResult.sMessage = "Critical ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            SaveErrortoDB(oCResult);
                            oCResult.sMessage = "Error Query " + sFinalQuery;
                            SaveErrortoDB(oCResult);
                            oCResult.LogToFile();
                            conn.Close();
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            oTrace.sMessage = ex.ToString();
                        }
                    }
                    conn.Close();
                }
            }
            else // LOG DTATABASE CONNECTION STRING NOT THERE
            {
                oCResult = new CResult();
                oCResult.sMessage = "Log: Connection String not there for DatasourceID: " + iDataSource;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.sMessage = "Log Query " + sFinalQuery;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oTrace.sMessage = oCResult.sMessage;
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        #endregion

        //IF user is non authenticated then if whitelist check()
        //{oBOD==false
        //then write error to Log
        //Exit

        public CResult Update_TODB(XIIBO oBOInstance, string sIDRef)
        {
            CResult oCResult = new CResult();
            string BOName = string.Empty;
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                string sPrimaryKey = string.Empty;
                sPrimaryKey = oBOInstance.BOD.sPrimaryKey;
                //if (!string.IsNullOrEmpty(sIDRef) && (oBOInstance.BOD.bUID || sIDRef.ToLower() == "xiguid"))
                //{
                //    sPrimaryKey = "XIGUID";
                //}
                //else if (string.IsNullOrEmpty(sPrimaryKey))
                //{
                //    sPrimaryKey = "id";
                //}
                //else if (oBOInstance.BOD.bUID)
                //{
                //    sPrimaryKey = "XIGUID";
                //}
                var sPKValue = string.Empty;
                var PKGUID = string.Empty;
                Guid InsGUID = Guid.Empty;
                PKGUID = oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == "xiguid".ToLower()).Select(m => m.sValue).FirstOrDefault();
                Guid.TryParse(PKGUID, out InsGUID);
                if (!string.IsNullOrEmpty(sPrimaryKey))
                {
                    sPKValue = oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == sPrimaryKey.ToLower()).Select(m => m.sValue).FirstOrDefault();
                }
                var sOriginalPK = oBOInstance.BOD.sPrimaryKey;
                var iDatasource = oBOInstance.BOD.iDataSource;
                XIDXI oXID = new XIDXI();
                var oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, oBOInstance.BOD.iDataSourceXIGUID.ToString());
                if (oDataSource.sQueryType.ToLower() == "mongodb")
                {
                    //Dictionary<string, object> Data1 = new Dictionary<string, object>();
                    //XIDBMongoDB oMongoDB = new XIDBMongoDB();
                    //oMongoDB.sServer = oDataSource.sServer;
                    //oMongoDB.sDatabase = oDataSource.sDatabase;
                    //oMongoDB.sTable = oBOInstance.BOD.TableName;
                    //if (string.IsNullOrEmpty(sPKValue) || sPKValue == "0")
                    //{
                    //    foreach (var oAttrI in oBOInstance.Attributes.Values.ToList())
                    //    {
                    //        Data1[oAttrI.sName] = oAttrI.sValue;
                    //    }
                    //    var jsonDoc = Newtonsoft.Json.JsonConvert.SerializeObject(Data1);
                    //    oMongoDB.Insert_Data(jsonDoc);
                    //}
                    //else
                    //{
                    //    oMongoDB.sPrimaryKey = sPrimaryKey;
                    //    oMongoDB.sUID = sPKValue;
                    //    var Attrs = oBOInstance.Attributes.Values.ToList().Where(m => m.bDirty == true).Select(m => new CNV { sName = m.sName, sValue = m.sValue }).ToList();
                    //    oMongoDB.Update_Data(Attrs);
                    //}
                }
                else
                {
                    //oBOInstance.Attributes = AssignDefaultValues(oBOInstance);
                    bool IsValidQuery = true;
                    //if (oBOInstance.Attributes.ContainsKey("XIGUID") && oBOInstance.Attributes["XIGUID"] != null)
                    //{
                    //    if (string.IsNullOrEmpty(oBOInstance.Attributes["XIGUID"].sValue))
                    //    {
                    //        oBOInstance.Attributes["XIGUID"].sValue = Guid.NewGuid().ToString();
                    //    }
                    //}
                    //else
                    //{
                    //    oBOInstance.Attributes["XIGUID"] = new XIIAttribute { sName = "XIGUID", sValue = Guid.NewGuid().ToString() };
                    //}
                    if (oBOInstance.Attributes.ContainsKey("XIGUID"))
                    {
                        oBOInstance.Attributes["XIGUID"].bDirty = false;
                    }
                    var BOIAttrs = oBOInstance.Attributes.Values.Where(m => m.bDirty == true).ToList();
                    if (BOIAttrs != null && BOIAttrs.Count() > 0)
                    {
                        if (string.IsNullOrEmpty(sPKValue) || sPKValue == "0")
                        {
                            oBOInstance = AddDefaultDataForInsert(oBOInstance);
                        }
                        else
                        {
                            oBOInstance = AddDefaultDataForUpdate(oBOInstance);
                        }
                    }
                    var Columns = oBOInstance.Attributes.Values.Where(m => m.bDirty == true).Select(m => m.sName).ToList();
                    var Data = oBOInstance.Attributes.Values.Where(m => m.bDirty == true).Select(m => m.sValue).ToList();
                    string sQuery = "";
                    StringBuilder sbQ = new StringBuilder();
                    StringBuilder sbV = new StringBuilder();
                    string sUpdateQuery = "";
                    string sValues = string.Empty;

                    string sFieldName = "";
                    BOName = oBOInstance.BOD.Name;
                    //if (oBOInstance.BOD.bUID)
                    //{
                    //sPrimaryKey = "XIGUID";

                    //}
                    //else
                    //{                    
                    using (IEnumerator<string> enumerator1 = Columns.GetEnumerator(), enumerator2 = Data.GetEnumerator())
                    {
                        while (enumerator1.MoveNext() && enumerator2.MoveNext())
                        {
                            if (!string.IsNullOrEmpty(enumerator1.Current) && enumerator1.Current.ToLower() != sPrimaryKey.ToLower() && enumerator1.Current.ToLower() != sOriginalPK.ToLower())
                            {
                                sFieldName = enumerator1.Current;
                                string sFieldValue = enumerator2.Current;
                                sFieldValue = sFieldValue == null ? null : sFieldValue.Trim();
                                XIDAttribute oAttrD = new XIDAttribute();
                                oAttrD = oBOInstance.BOD.Attributes.Values.ToList().Where(m => !string.IsNullOrEmpty(m.Name) && m.Name.ToLower() == sFieldName.ToLower()).FirstOrDefault();
                                //sQuery = sQuery + enumerator1.Current + "='" + enumerator2.Current + "', ";
                                if (oAttrD == null)
                                {
                                    if (sFieldName.ToLower() == XIConstant.Key_XIDeleted.ToLower() || sFieldName.ToLower() == XIConstant.Key_XICrtdBy.ToLower() || sFieldName.ToLower() == XIConstant.Key_XICrtdWhn.ToLower() || sFieldName.ToLower() == XIConstant.Key_XIUpdtdBy.ToLower() || sFieldName.ToLower() == XIConstant.Key_XIUpdtdWhn.ToLower())
                                    {
                                        sQuery = sQuery + sFieldName + ',';
                                        sbQ.Append(sFieldName + ",");

                                        if (sFieldName.ToLower() == XIConstant.Key_XICrtdWhn.ToLower() || sFieldName.ToLower() == XIConstant.Key_XIUpdtdWhn.ToLower())
                                        {
                                            string sCurrentValue = sFieldValue;
                                            if (sFieldName.ToLower() == XIConstant.Key_XIUpdtdWhn.ToLower())
                                            {
                                                sCurrentValue = DateTime.Now.ToString();
                                            }
                                            if (string.IsNullOrEmpty(sCurrentValue))
                                            {
                                                sCurrentValue = DateTime.Now.ToString();
                                            }
                                            var dtDate = ConvertToDtTime(sCurrentValue);
                                            sValues = sValues + "'" + dtDate.ToString(XIConstant.SqlDateFormat) + "'" + ',';
                                            sbV.Append("'" + dtDate.ToString(XIConstant.SqlDateFormat) + "'" + ',');
                                            sUpdateQuery = sUpdateQuery + sFieldName + "='" + dtDate.ToString(XIConstant.SqlDateFormat) + "'" + ',';
                                        }
                                        else
                                        {
                                            sValues = sValues + "'" + sFieldValue + "'" + ',';
                                            sbV.Append("'" + sFieldValue + "'" + ',');
                                            sUpdateQuery = sUpdateQuery + sFieldName + "='" + sFieldValue + "'" + ',';
                                        }
                                    }
                                    else
                                    {
                                        if (oBOInstance.BOD.Attributes.ContainsKey(sFieldName.ToLower()))
                                            oAttrD = oBOInstance.BOD.Attributes.Values.Where(x => x.LabelName.ToLower() == sFieldName.ToLower()).FirstOrDefault();
                                    }

                                }
                                if (oAttrD != null && !sQuery.ToLower().Contains(',' + oAttrD.Name.ToLower() + ',') && !sQuery.ToLower().StartsWith(oAttrD.Name.ToLower() + ','))
                                {
                                    //if ((bool)(oAttrD.CheckDatatype(enumerator2.Current, oAttrD.Get_BODatatype(oAttrD.TypeID))))
                                    //{
                                    //    sValues = sValues + "'" + enumerator2.Current + "'" + ',';
                                    //    sUpdateQuery = sUpdateQuery + enumerator1.Current + "='" + enumerator2.Current + "'" + ',';
                                    //}
                                    //else
                                    //{
                                    //    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Mismatching value with the name of '" + enumerator1.Current + "' and the value is '" + enumerator2.Current + "'" });
                                    //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    //    IsValidQuery = false;
                                    //}
                                    if (oAttrD.bTrack)
                                    {
                                        if (!string.IsNullOrEmpty(sPKValue) && sPKValue != "0")
                                        {

                                            if (oBOInstance.Attributes[oAttrD.Name.ToLower()].sPreviousValue == null)
                                            {
                                                XIIXI oXI = new XIIXI();
                                                var oBOI = oXI.BOI(oBOInstance.BOD.Name, sPKValue);
                                                if (oBOI != null && oBOI.Attributes.Count() > 0)
                                                {
                                                    var sPrevValue = oBOI.AttributeI(oAttrD.Name).sValue;
                                                    if (!string.IsNullOrEmpty(sPrevValue))
                                                    {
                                                        var oAtrI = oBOInstance.Attributes.Values.ToList().Where(m => m.sName.ToLower() == oAttrD.Name.ToLower()).FirstOrDefault();
                                                        if (oAtrI != null)
                                                        {

                                                            oBOInstance.Attributes.Values.ToList().Where(m => m.sName.ToLower() == oAttrD.Name.ToLower()).FirstOrDefault().sPreviousValue = sPrevValue;
                                                        }
                                                    }
                                                }
                                            }
                                            if (oBOInstance.Attributes[oAttrD.Name.ToLower()].sValue != oBOInstance.Attributes[oAttrD.Name.ToLower()].sPreviousValue && oBOInstance.Attributes[oAttrD.Name.ToLower()].sPreviousValue != null)
                                            {
                                                //XIInfraCache oCache = new XIInfraCache();
                                                XIIBO oTrackBOI = new XIIBO();
                                                oTrackBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, "XIBOAttributeTrack", null);
                                                oTrackBOI.SetAttribute("FKiBODID", oBOInstance.BOD.BOID.ToString());
                                                oTrackBOI.SetAttribute("FKiBODIDXIGUID", oBOInstance.BOD.XIGUID.ToString());
                                                oTrackBOI.SetAttribute("FKiAttributeDID", oAttrD.ID.ToString());
                                                oTrackBOI.SetAttribute("FKiAttributeDIDXIGUID", oAttrD.XIGUID.ToString());
                                                oTrackBOI.SetAttribute("iBOIID", oBOInstance.Attributes[sPrimaryKey.ToLower()].sValue);
                                                oTrackBOI.SetAttribute("sValue", oBOInstance.Attributes[oAttrD.Name.ToLower()].sPreviousValue);
                                                oTrackBOI.SetAttribute("sValueString", oBOInstance.Attributes[oAttrD.Name.ToLower()].sResolvedValue);
                                                var oCR = Update_TODB(oTrackBOI, sIDRef);
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(sFieldValue))
                                    {
                                        if (!string.IsNullOrEmpty(sFieldValue) && sFieldValue.Contains("\'"))
                                        {
                                            sFieldValue = sFieldValue.Replace("\'", "\''");
                                        }
                                        if (oAttrD.FKiType > 0 && oAttrD.bFKGUID)
                                        {
                                            if (oAttrD.TypeID == 10 || oAttrD.TypeID == 60)
                                            {
                                                int iFieldVal = 0;
                                                if (int.TryParse(sFieldValue, out iFieldVal))
                                                {
                                                    bool bFKGUIDSet = false;
                                                    if (oBOInstance.Attributes.ContainsKey(oAttrD.Name.ToLower() + "XIGUID".ToLower()))
                                                    {
                                                        var FKGUIDAttrVal = oBOInstance.AttributeI(oAttrD.Name.ToLower() + "XIGUID".ToLower()).sValue;
                                                        var FKGUIDValue = Guid.Empty;
                                                        Guid.TryParse(FKGUIDAttrVal, out FKGUIDValue);
                                                        if (FKGUIDValue != null && FKGUIDValue != Guid.Empty)
                                                        {
                                                            bFKGUIDSet = true;
                                                        }
                                                    }
                                                    if (!bFKGUIDSet)
                                                    {
                                                        var oCR = Assign_FKGUIDAttribute(sFieldValue, oAttrD);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            var FKGUID = (string)oCR.oResult;
                                                            if (!string.IsNullOrEmpty(sQuery) && sQuery.ToLower().Contains(oAttrD.Name.ToLower() + "XIGUID,".ToLower()))
                                                            {

                                                            }
                                                            else
                                                            {
                                                                sQuery = sQuery + oAttrD.Name + "XIGUID,";
                                                                sbQ.Append(oAttrD.Name + "XIGUID,");
                                                                sValues = sValues + "'" + FKGUID + "'" + ',';
                                                                sbV.Append("'" + FKGUID + "'" + ',');
                                                                sUpdateQuery = sUpdateQuery + oAttrD.Name + "XIGUID" + "='" + FKGUID + "'" + ',';
                                                            }
                                                        }
                                                    }
                                                    sQuery = sQuery + oAttrD.Name + ',';
                                                    sbQ.Append(oAttrD.Name + ",");
                                                    sValues = sValues + "'" + sFieldValue + "'" + ',';
                                                    sbV.Append("'" + sFieldValue + "'" + ',');
                                                    sUpdateQuery = sUpdateQuery + oAttrD.Name + "='" + sFieldValue + "'" + ',';
                                                }
                                            }
                                            else if(oAttrD.TypeID == 180)
                                            {
                                                sQuery = sQuery + oAttrD.Name + ',';
                                                sbQ.Append(oAttrD.Name + ",");
                                                sValues = sValues + "'" + sFieldValue + "'" + ',';
                                                sbV.Append("'" + sFieldValue + "'" + ',');
                                                sUpdateQuery = sUpdateQuery + oAttrD.Name + "='" + sFieldValue + "'" + ',';
                                            }
                                            else
                                            {
                                                Guid guid = Guid.Empty;
                                                if (Guid.TryParse(sFieldValue, out guid))
                                                {
                                                    var FKAttr = oAttrD.Name.Replace("XIGUID", "");
                                                    bool bFKIDSet = false;
                                                    if (oBOInstance.Attributes.ContainsKey(FKAttr.ToLower()))
                                                    {
                                                        var FKIDAttrVal = oBOInstance.AttributeI(FKAttr.ToLower()).sValue;
                                                        var FKIDValue = 0;
                                                        int.TryParse(FKIDAttrVal, out FKIDValue);
                                                        if (FKIDValue > 0)
                                                        {
                                                            bFKIDSet = true;
                                                        }
                                                    }
                                                    if (!bFKIDSet)
                                                    {
                                                        var oCR = Assign_FKIDAttribute(sFieldValue, oAttrD);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            var FKGUID = (string)oCR.oResult;
                                                            if (!string.IsNullOrEmpty(sQuery) && sQuery.ToLower().Contains(FKAttr.ToLower() + ","))
                                                            {

                                                            }
                                                            else
                                                            {
                                                                sQuery = sQuery + FKAttr + ",";
                                                                sbQ.Append(FKAttr + ",");
                                                                sValues = sValues + "'" + FKGUID + "'" + ',';
                                                                sbV.Append("'" + FKGUID + "'" + ',');
                                                                sUpdateQuery = sUpdateQuery + FKAttr + "='" + FKGUID + "'" + ',';
                                                            }
                                                        }
                                                    }
                                                    sQuery = sQuery + oAttrD.Name + ',';
                                                    sbQ.Append(oAttrD.Name + ",");
                                                    sValues = sValues + "'" + sFieldValue + "'" + ',';
                                                    sbV.Append("'" + sFieldValue + "'" + ',');
                                                    sUpdateQuery = sUpdateQuery + oAttrD.Name + "='" + sFieldValue + "'" + ',';
                                                }
                                            }
                                        }
                                        else if ((oAttrD.TypeID == 150 || oAttrD.TypeID == 110))
                                        {
                                            if ((sFieldValue == "1/1/1900 12:00:00 AM" || sFieldValue == "1/1/0001 12:00:00 AM"))
                                            {
                                                sQuery = sQuery + oAttrD.Name + ',';
                                                sbQ.Append(oAttrD.Name + ",");
                                                sValues = sValues + "'',";
                                                sbV.Append("'',");
                                                sUpdateQuery = sUpdateQuery + oAttrD.Name + "=''" + ',';
                                            }
                                            else
                                            {
                                                var dtDate = ConvertToDtTime(sFieldValue);
                                                if (dtDate != DateTime.MinValue)
                                                {
                                                    sQuery = sQuery + oAttrD.Name + ',';
                                                    sbQ.Append(oAttrD.Name + ",");
                                                    sValues = sValues + "'" + dtDate.ToString(XIConstant.SqlDateFormat) + "'" + ',';
                                                    sbV.Append("'" + dtDate.ToString(XIConstant.SqlDateFormat) + "'" + ',');
                                                    sUpdateQuery = sUpdateQuery + oAttrD.Name + "='" + dtDate.ToString(XIConstant.SqlDateFormat) + "'" + ',';
                                                }
                                                else
                                                {
                                                    //sValues = sValues + "''" + ',';
                                                    //sbV.Append("''" + ',');
                                                    //sUpdateQuery = sUpdateQuery + oAttrD.Name + "=''" + ',';
                                                }
                                            }
                                        }
                                        else if (oAttrD.TypeID == 20)
                                        {
                                            sQuery = sQuery + oAttrD.Name + ',';
                                            sbQ.Append(oAttrD.Name + ",");
                                            string sString = sFieldValue;
                                            if (sFieldValue != null && sFieldValue.ToLower() == "on")
                                            {
                                                sString = "true";
                                            }
                                            sValues = sValues + "'" + sString + "'" + ',';
                                            sbV.Append("'" + sString + "'" + ',');
                                            sUpdateQuery = sUpdateQuery + oAttrD.Name + "='" + sString + "'" + ',';
                                        }
                                        else if (oAttrD.TypeID == 210)
                                        {
                                            sQuery = sQuery + oAttrD.Name + ',';
                                            sbQ.Append(oAttrD.Name + ",");
                                            sValues = sValues + "N'" + sFieldValue + "'" + ',';
                                            sbV.Append("N'" + sFieldValue + "'" + ',');
                                            sUpdateQuery = sUpdateQuery + oAttrD.Name + "=N'" + sFieldValue + "'" + ',';
                                        }
                                        else
                                        {
                                            sQuery = sQuery + oAttrD.Name + ',';
                                            sbQ.Append(oAttrD.Name + ",");
                                            sValues = sValues + "'" + sFieldValue + "'" + ',';
                                            sbV.Append("'" + sFieldValue + "'" + ',');
                                            sUpdateQuery = sUpdateQuery + oAttrD.Name + "='" + sFieldValue + "'" + ',';
                                        }
                                    }
                                    else
                                    {
                                        if (oAttrD.FKiType > 0 && oAttrD.bFKGUID)
                                        {
                                            if (oAttrD.TypeID == 10 || oAttrD.TypeID == 60)
                                            {
                                                sQuery = sQuery + oAttrD.Name + ',';
                                                sbQ.Append(oAttrD.Name + ",");
                                                sValues = sValues + "0,";
                                                sbV.Append("0,");
                                                sUpdateQuery = sUpdateQuery + oAttrD.Name + "=0,";

                                                //Set Guid attribute to null on update
                                                string sGuidAttr = oAttrD.Name + "XIGUID";
                                                var GuidAttrD = oBOInstance.BOD.Attributes.Values.ToList().Where(m => !string.IsNullOrEmpty(m.Name) && m.Name.ToLower() == sGuidAttr.ToLower()).FirstOrDefault();
                                                if (GuidAttrD != null)
                                                {
                                                    sQuery = sQuery + sGuidAttr + ',';
                                                    sbQ.Append(sGuidAttr + ",");
                                                    sValues = sValues + "null,";
                                                    sbV.Append("null,");
                                                    sUpdateQuery = sUpdateQuery + sGuidAttr + "=null,";
                                                }
                                            }
                                            else
                                            {
                                                sQuery = sQuery + oAttrD.Name + ',';
                                                sbQ.Append(oAttrD.Name + ",");
                                                sValues = sValues + "null,";
                                                sbV.Append("null,");
                                                sUpdateQuery = sUpdateQuery + oAttrD.Name + "=null,";

                                                //Set Guid attribute to null on update
                                                string sIDAttr = oAttrD.Name.Replace("XIGUID", "");
                                                var IDAttrD = oBOInstance.BOD.Attributes.Values.ToList().Where(m => !string.IsNullOrEmpty(m.Name) && m.Name.ToLower() == sIDAttr.ToLower()).FirstOrDefault();
                                                if (IDAttrD != null)
                                                {
                                                    sQuery = sQuery + sIDAttr + ',';
                                                    sbQ.Append(sIDAttr + ",");
                                                    sValues = sValues + "0,";
                                                    sbV.Append("0,");
                                                    sUpdateQuery = sUpdateQuery + sIDAttr + "=0,";
                                                }
                                            }
                                        }
                                        else if (oAttrD.TypeID == 150 || oAttrD.TypeID == 110 || oAttrD.TypeID == 90)
                                        {
                                            sQuery = sQuery + oAttrD.Name + ',';
                                            sbQ.Append(oAttrD.Name + ",");
                                            sValues = sValues + "NULL" + ',';
                                            sbV.Append("NULL" + ',');
                                            sUpdateQuery = sUpdateQuery + oAttrD.Name + "=NULL" + ',';
                                        }
                                        else if (oAttrD.TypeID == 10 || oAttrD.TypeID == 40 || oAttrD.TypeID == 60)
                                        {
                                            sQuery = sQuery + oAttrD.Name + ',';
                                            sbQ.Append(oAttrD.Name + ",");
                                            sValues = sValues + "0" + ',';
                                            sbV.Append("0" + ',');
                                            sUpdateQuery = sUpdateQuery + oAttrD.Name + "=0" + ',';
                                        }
                                        else if (oAttrD.TypeID == 250 && oAttrD.Name.ToLower() == "xiguid")
                                        {
                                            string sVal = Guid.NewGuid().ToString();
                                            sQuery = sQuery + oAttrD.Name + ',';
                                            sbQ.Append(oAttrD.Name + ",");
                                            sValues = sValues + "'" + sVal + "'" + ',';
                                            sbV.Append("'" + sVal + "'" + ',');
                                            sUpdateQuery = sUpdateQuery + oAttrD.Name + "='" + sVal + "'" + ',';
                                        }
                                        else if (string.IsNullOrEmpty(sFieldValue))
                                        {
                                            sQuery = sQuery + oAttrD.Name + ',';
                                            sbQ.Append(oAttrD.Name + ",");
                                            sValues = sValues + "null,";
                                            sbV.Append("null,");
                                            sUpdateQuery = sUpdateQuery + oAttrD.Name + "=null,";
                                        }
                                        //Commented to handle attributes value saving in generic way
                                        //else if (sFieldValue != "1/1/0001 12:00:00 AM")
                                        //{
                                        //    sQuery = sQuery + oAttrD.Name + ',';
                                        //    sbQ.Append(oAttrD.Name + ",");
                                        //    string sString = string.Empty;
                                        //    if (!string.IsNullOrEmpty(sFieldValue) && sFieldValue.Contains("\'"))
                                        //    {
                                        //        sString = sFieldValue.Replace("\'", "\''");
                                        //    }
                                        //    else if (oAttrD.TypeID == 90 && string.IsNullOrEmpty(sFieldValue))
                                        //    {
                                        //        sString = null;
                                        //    }
                                        //    else
                                        //    {
                                        //        sString = sFieldValue;
                                        //    }
                                        //    if (oAttrD.TypeID == 40)
                                        //    {
                                        //        if (string.IsNullOrEmpty(sString))
                                        //        {
                                        //            sString = "0";
                                        //        }
                                        //    }
                                        //    else if (oAttrD.TypeID == 60)
                                        //    {
                                        //        if (string.IsNullOrEmpty(sString))
                                        //        {
                                        //            if (!string.IsNullOrEmpty(oAttrD.DefaultValue))
                                        //            {
                                        //                sString = oAttrD.DefaultValue;
                                        //            }
                                        //            else
                                        //            {
                                        //                sString = "0";
                                        //            }
                                        //        }
                                        //    }
                                        //    else if (oAttrD.TypeID == 250)
                                        //    {
                                        //        if (string.IsNullOrEmpty(sString))
                                        //        {
                                        //            if (oAttrD.Name.ToLower() == "xiguid")
                                        //            {
                                        //                sString = Guid.NewGuid().ToString();
                                        //            }
                                        //            else
                                        //            {
                                        //                sString = Guid.Empty.ToString();
                                        //            }
                                        //        }
                                        //    }
                                        //    if (sString == null)
                                        //    {
                                        //        sValues = sValues + "null,";
                                        //        sUpdateQuery = sUpdateQuery + oAttrD.Name + "=null,";
                                        //    }
                                        //    else
                                        //    {
                                        //        sValues = sValues + "'" + sString + "'" + ',';
                                        //        sbV.Append("'" + sString + "'" + ',');
                                        //        sUpdateQuery = sUpdateQuery + oAttrD.Name + "='" + sString + "'" + ',';
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    sValues = sValues + "' ',";
                                        //    sbV.Append("' ',");
                                        //    sUpdateQuery = sUpdateQuery + oAttrD.Name + "=' ',";
                                        //}
                                    }
                                }
                            }
                        }
                    }
                    if (IsValidQuery)
                    {
                        sQuery = sQuery.Substring(0, sQuery.Length - 1);
                        sValues = sValues.Substring(0, sValues.Length - 1);
                        sbV.Remove(sbV.Length - 1, 1);
                        sbQ.Remove(sbQ.Length - 1, 1); //remove Last Character comma
                        sUpdateQuery = sUpdateQuery.Substring(0, sUpdateQuery.Length - 1);
                        //var sBODataSource = string.Empty;
                        //XIDXI oXID = new XIDXI();
                        //sBODataSource = oXID.GetBODataSource(oBOInstance.BOD.iDataSource,oBOInstance.BOD.FKiApplicationID);
                        var TableName = string.Empty;
                        if (oBOInstance.BOD.TableName != null)
                        {
                            TableName = oBOInstance.BOD.TableName;
                        }
                        else
                        {
                            TableName = oBOInstance.BOD.Name;
                        }
                        if ((string.IsNullOrEmpty(sPKValue) || sPKValue == "0") && (InsGUID == null || InsGUID == Guid.Empty))
                        {
                            string cmdText = "";
                            if (oBOInstance.BOD.bIsAutoIncrement)
                            {
                                //sQuery = AddDefaultColumnsForInsert(sQuery);
                                //if (!bIsCreatedBy)
                                //{
                                //    sValues = AddDefaultDataForInsert(sQuery, sValues);
                                //}
                                cmdText = "INSERT INTO " + TableName + "(" + sQuery + ") output INSERTED." + sPrimaryKey + " VALUES(" + sValues + ")";
                            }
                            else
                            {
                                string sPK = sPrimaryKey;

                                sQuery = sPK + "," + sQuery;
                                //string _sbq = sbQ.ToString();
                                //sQuery = sPrimaryKey + "," + sQuery;
                                sbQ.Insert(0, sPK + ",");
                                sValues = "isnull(@max,0)+1," + sValues;
                                sbV.Append("@max+1,");
                                sQuery = AddDefaultColumnsForInsert(sQuery);
                                //if (!bIsCreatedBy)
                                //{
                                //    sValues = AddDefaultDataForInsert(sQuery, sValues);
                                //}
                                //if (oBOInstance.BOD.bUID)
                                //{
                                //    cmdText = "declare @max int; set @max = (SELECT MAX(" + sPK + ") FROM " + TableName + "); INSERT INTO " + TableName + "(" + sQuery + ") output inserted.XIGUID VALUES(" + sValues + ")";
                                //    //cmdText = "DECLARE @Tableobj TABLE (" + sPK + " nvarchar(256)) declare @max int; set @max = (SELECT MAX(" + sPK + ") FROM " + TableName + "); INSERT INTO " + TableName + "(" + sQuery + ") output inserted." + sPK + " INTO @Tableobj VALUES(" + sValues + ") SELECT * FROM @Tableobj";
                                //    // cmdText = "declare @max int; set @max = (SELECT MAX(" + sPK + ") FROM " + TableName + "); INSERT INTO " + TableName + "(" + sQuery + ") output inserted.XIGUID VALUES(" + sValues + ")";
                                //}
                                //else 
                                if (oBOInstance.BOD.bUID || (!string.IsNullOrEmpty(sIDRef) && sIDRef.ToLower() == "xiguid"))
                                {
                                    //cmdText = "declare @max int; set @max = (SELECT MAX(" + sPK + ") FROM " + TableName + "); INSERT INTO " + TableName + "(" + sQuery + ") output inserted.XIGUID VALUES(" + sValues + ")";
                                    cmdText = "DECLARE @Tableobj TABLE (" + sPK + "  nvarchar(256)) declare @max int; set @max = (SELECT MAX(" + sPK + ") FROM " + TableName + "); INSERT INTO " + TableName + "(" + sQuery + ") output inserted.XIGUID INTO @Tableobj VALUES(" + sValues + ") SELECT * FROM @Tableobj";
                                    //cmdText = "declare @max int; set @max = (SELECT MAX(" + sPK + ") FROM " + TableName + "); INSERT INTO " + TableName + "(" + sQuery + ") output inserted.XIGUID VALUES(" + sValues + ")";
                                }
                                else
                                {
                                    //cmdText = "declare @max int; set @max = (SELECT MAX(" + sPK + ") FROM " + TableName + "); INSERT INTO " + TableName + "(" + sQuery + ") output inserted." + sPrimaryKey + " VALUES(" + sValues + ")";
                                    cmdText = "DECLARE @Tableobj TABLE (" + sPK + "  nvarchar(256)) declare @max int; set @max = (SELECT MAX(" + sPK + ") FROM " + TableName + "); INSERT INTO " + TableName + "(" + sQuery + ") output inserted." + sPrimaryKey + " INTO @Tableobj VALUES(" + sValues + ") SELECT * FROM @Tableobj";
                                    //cmdText = "declare @max int; set @max = (SELECT MAX(" + sPK + ") FROM " + TableName + "); INSERT INTO " + TableName + "(" + sQuery + ") output inserted." + sPrimaryKey + " VALUES(" + sValues + ")";
                                }

                            }
                            sFinalQuery = cmdText;
                            string iInstanceID = "0";
                            //if (Convert.ToInt32(EnumTransactionEnabled.Yes) == oBOInstance.BOD.iTransactionEnable)
                            if (TransactionIntiation != null)
                            {
                                iInstanceID = Execute_TransactionEnabledQuery(cmdText, true);
                            }
                            else
                            {
                                iInstanceID = Execute_NormalQuery(cmdText, oBOInstance.BOD.iDataSourceXIGUID.ToString(), true);
                            }
                            int iInsID = 0;
                            var InstanceGUID = Guid.Empty;
                            Guid.TryParse(iInstanceID, out InstanceGUID);
                            int.TryParse(iInstanceID, out iInsID);
                            if ((InstanceGUID != null && InstanceGUID != Guid.Empty) || iInsID > 0)
                            {
                                if (oBOInstance.BOD.bUID)
                                {
                                    sPrimaryKey = "XIGUID";
                                }
                                else if (!string.IsNullOrEmpty(sIDRef) && sIDRef.ToLower() == "xiguid" && InstanceGUID != null && InstanceGUID != Guid.Empty)
                                {
                                    sPrimaryKey = "XIGUID";
                                }
                                else if (string.IsNullOrEmpty(sPrimaryKey))
                                {
                                    sPrimaryKey = oBOInstance.BOD.sPrimaryKey;
                                }
                                var IDPair = oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == sPrimaryKey.ToLower()).FirstOrDefault();
                                if (IDPair != null)
                                {
                                    if (iInstanceID != "0")
                                    {

                                    }
                                    if ((!string.IsNullOrEmpty(sIDRef) && sIDRef.ToLower() == "xiguid" && InstanceGUID != Guid.Empty) || oBOInstance.BOD.bUID)
                                    {
                                        if (oBOInstance.Attributes.Values.Any(m => m.sName.ToLower() == "xiguid".ToLower()))
                                        {
                                            oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == "xiguid".ToLower()).FirstOrDefault().sValue = iInstanceID.ToString();
                                        }
                                        else
                                        {
                                            oBOInstance.SetAttribute("xiguid", iInstanceID.ToString());
                                        }
                                    }
                                    else
                                        oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == sPrimaryKey.ToLower()).FirstOrDefault().sValue = iInstanceID.ToString();
                                }
                                else
                                {
                                    XIIAttribute oAttrI = new XIIAttribute();
                                    oAttrI.sValue = iInstanceID;
                                    if (InstanceGUID != null && InstanceGUID != Guid.Empty)
                                    {
                                        oAttrI.sName = "xiguid";
                                        oBOInstance.Attributes["xiguid"] = oAttrI;
                                    }
                                    else
                                    {
                                        oAttrI.sName = sPrimaryKey.ToLower();
                                        oBOInstance.Attributes[sPrimaryKey.ToLower()] = oAttrI;
                                    }
                                }
                                sPKValue = iInstanceID;
                                if (!string.IsNullOrEmpty(oBOInstance.BOD.sAuditBOName) && bIsAudit && oBOInstance.BOD.sAuditBOName == "Audit_T" && !string.IsNullOrEmpty(oBOInstance.BOD.sAuditBOfield) && oBOInstance.BOD.sAuditBOfield.ToLower() != oBOInstance.BOD.sPrimaryKey.ToLower() && oBOInstance.Attributes.ContainsKey(oBOInstance.BOD.sAuditBOfield.ToLower()))
                                {
                                    //XIInfraCache oCache = new XIInfraCache();
                                    //XIIBO oAuditBOI = new XIIBO();
                                    var oTransaction = oBOInstance.TransactionIntiation;
                                    oBOInstance.TransactionIntiation = null;
                                    XIIBO oBOICopy = oBOInstance.GetCopy();
                                    oBOInstance.TransactionIntiation = oTransaction;
                                    string sAuditBOPrimaryKey = oBOICopy.BOD.sPrimaryKey;
                                    XIIXI oXIIXI = new XIIXI();
                                    var sID = oBOInstance.Attributes[sAuditBOPrimaryKey.ToLower()].sValue;

                                    //merge current boi with previous values
                                    //oAuditBOI = oAuditBOI.MergeAuditBOI(oAuditBOI);

                                    //oAuditBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, oBOInstance.BOD.sAuditBOName, null);
                                    //var oChangedData = oBOInstance.IsHavingChanges(oBOInstance, oBOICopy);
                                    //string sIsHavingChanges = string.Empty;
                                    //if (oChangedData != null && oChangedData.Count() > 0)
                                    //{
                                    //    sIsHavingChanges = oChangedData.Where(x => x.sName.ToLower() == "bIsHavingChanges".ToLower()).Select(x => x.sValue).FirstOrDefault();
                                    //}
                                    int iID = 0;
                                    if (int.TryParse(sID, out iID)) { }
                                    if (iID > 0)
                                    {
                                        //    if (oChangedData!=null && oChangedData.Count()>0)
                                        //{
                                        string sChangedData = "New " + oBOInstance.BOD.LabelName.ToLower() + " added with this id:" + iInstanceID;
                                        //string sPreviousData = oChangedData.Where(x => x.sName.ToLower() == "sPreviousData".ToLower()).Select(x => x.sValue).FirstOrDefault();
                                        XIIBO oAuditBOI = new XIIBO();
                                        if (!string.IsNullOrEmpty(oBOICopy.BOD.Attributes[oBOICopy.BOD.sAuditBOfield.ToLower()].sFKBOName))
                                        {
                                            var oParentBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oBOICopy.BOD.Attributes[oBOICopy.BOD.sAuditBOfield.ToLower()].sFKBOName, null);
                                            XIIXI oXII = new XIIXI();
                                            int iBOIID = 0;
                                            //oBOI = oXII.BOI(sBOName, sInstanceID);
                                            if (oBOICopy != null && oBOICopy.Attributes.Count() > 0 && oBOICopy.Attributes.ContainsKey(oBOInstance.BOD.sAuditBOfield.ToLower()) && !string.IsNullOrEmpty(oBOICopy.Attributes[oBOInstance.BOD.sAuditBOfield.ToLower()].sValue))
                                            {
                                                if (int.TryParse(oBOICopy.Attributes[oBOInstance.BOD.sAuditBOfield.ToLower()].sValue, out iBOIID)) { }
                                            }
                                            if (iBOIID == 0 && oBOInstance.Attributes.ContainsKey(oBOInstance.BOD.sAuditBOfield) && !string.IsNullOrEmpty(oBOInstance.Attributes[oBOInstance.BOD.sAuditBOfield].sValue))
                                            {
                                                int.TryParse(oBOInstance.Attributes[BOD.sAuditBOfield].sValue, out iBOIID);
                                            }
                                            oAuditBOI.SetAttribute("FksParentBOName", oParentBOD.Name);
                                            oAuditBOI.SetAttribute("FKiParentInstanceID", iBOIID.ToString());
                                            oAuditBOI.SetAttribute("FkiParentBOID", oParentBOD.BOID.ToString());
                                        }
                                        oAuditBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, oBOInstance.BOD.sAuditBOName, null);
                                        //oBOICopy.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oBOInstance.BOD.sAuditBOName);
                                        oAuditBOI.SetAttribute("FKiBOID", oBOInstance.BOD.BOID.ToString());
                                        oAuditBOI.SetAttribute("sBOName", oBOInstance.BOD.Name);
                                        oAuditBOI.SetAttribute("sData", sChangedData);
                                        oAuditBOI.SetAttribute("sOldData", "");
                                        //oAuditBOI.SetAttribute(XICreatedBy, "");
                                        //oAuditBOI.SetAttribute("XICreatedWhen", DateTime.Now.ToString());
                                        oAuditBOI.SetAttribute("sType", "New " + oBOInstance.BOD.LabelName);
                                        oAuditBOI.SetAttribute("sActivity", "New " + oBOInstance.BOD.LabelName);
                                        oAuditBOI.SetAttribute("FKiInstanceID", iID.ToString());
                                        var oAuditBOResponse = Update_TODB(oAuditBOI, sIDRef);
                                    }
                                }
                                if (!string.IsNullOrEmpty(oBOInstance.BOD.sTraceAttribute))
                                //if (!string.IsNullOrEmpty(oBOInstance.BOD.sTraceAttribute) && oBOInstance.BOD.AttributeD(oBOInstance.BOD.sTraceAttribute).bIsTrace)
                                {
                                    UpdateTraceTransaction(oBOInstance);
                                }
                            }
                            else
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(oBOInstance.BOD.sAuditBOName) && bIsAudit && oBOInstance.BOD.sAuditBOName == "Audit_T")
                            //if (oBOInstance.BOD.sAudit == "30" && !string.IsNullOrEmpty(oBOInstance.BOD.sAuditBOName) && bIsAudit)
                            {
                                //XIInfraCache oCache = new XIInfraCache();
                                //XIIBO oAuditBOI = new XIIBO();
                                var oTransaction = oBOInstance.TransactionIntiation;
                                oBOInstance.TransactionIntiation = null;
                                var BOD = oBOInstance.BOD;
                                oBOInstance.BOD = null;
                                XIIBO oBOICopy = oBOInstance.GetCopy();
                                oBOInstance.BOD = BOD;
                                oBOInstance.TransactionIntiation = oTransaction;
                                oBOICopy.BOD = BOD;
                                string sAuditBOPrimaryKey = oBOICopy.BOD.sPrimaryKey;
                                XIIXI oXIIXI = new XIIXI();
                                var iInstanceID = oBOInstance.Attributes[sPrimaryKey.ToLower()].sValue;
                                //merge current boi with previous values
                                //oAuditBOI = oAuditBOI.MergeAuditBOI(oAuditBOI);
                                //Load boi for previous data
                                //oBOICopy = oXIIXI.BOI(oBOICopy.BOD.Name, iInstanceID);
                                //oAuditBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, oBOInstance.BOD.sAuditBOName, null);
                                var oChangedData = oBOInstance.IsHavingChanges(oBOInstance, oBOICopy);
                                string sIsHavingChanges = string.Empty;
                                if (oChangedData != null && oChangedData.Count() > 0)
                                {
                                    sIsHavingChanges = oChangedData.Where(x => x.sName.ToLower() == "bIsHavingChanges".ToLower()).Select(x => x.sValue).FirstOrDefault();
                                }
                                if (!string.IsNullOrEmpty(sIsHavingChanges) && sIsHavingChanges.ToLower() == "true")
                                {
                                    //    if (oChangedData!=null && oChangedData.Count()>0)
                                    //{
                                    string sChangedData = oChangedData.Where(x => x.sName.ToLower() == "sChangedData".ToLower()).Select(x => x.sValue).FirstOrDefault();
                                    string sPreviousData = oChangedData.Where(x => x.sName.ToLower() == "sPreviousData".ToLower()).Select(x => x.sValue).FirstOrDefault();
                                    XIIBO oAuditBOI = new XIIBO();
                                    oAuditBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, oBOInstance.BOD.sAuditBOName, null);
                                    if (!string.IsNullOrEmpty(oBOInstance.BOD.sAuditBOfield) && oBOICopy.Attributes.ContainsKey(oBOInstance.BOD.sAuditBOfield.ToLower()))
                                    {
                                        //if()
                                        if (!string.IsNullOrEmpty(oBOICopy.BOD.Attributes[oBOICopy.BOD.sAuditBOfield].sFKBOName))
                                        {
                                            var oParentBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oBOICopy.BOD.Attributes[oBOICopy.BOD.sAuditBOfield].sFKBOName, null);
                                            XIIXI oXII = new XIIXI();
                                            int iBOIID = 0;
                                            //oBOI = oXII.BOI(sBOName, sInstanceID);
                                            if (oBOICopy != null && oBOICopy.Attributes.Count() > 0 && oBOICopy.Attributes.ContainsKey(BOD.sAuditBOfield) && !string.IsNullOrEmpty(oBOICopy.Attributes[BOD.sAuditBOfield].sValue))
                                            {
                                                if (int.TryParse(oBOICopy.Attributes[BOD.sAuditBOfield].sValue, out iBOIID)) { }
                                            }
                                            if (iBOIID == 0 && oBOInstance.Attributes.ContainsKey(BOD.sAuditBOfield) && !string.IsNullOrEmpty(oBOInstance.Attributes[BOD.sAuditBOfield].sValue))
                                            {
                                                int.TryParse(oBOInstance.Attributes[BOD.sAuditBOfield].sValue, out iBOIID);
                                            }
                                            oAuditBOI.SetAttribute("FksParentBOName", oParentBOD.Name);
                                            oAuditBOI.SetAttribute("FKiParentInstanceID", iBOIID.ToString());
                                            oAuditBOI.SetAttribute("FkiParentBOID", oParentBOD.BOID.ToString());
                                        }
                                        //if (!string.IsNullOrEmpty(oBOICopy.Attributes[oBOInstance.BOD.sAuditBOfield.ToLower()].sValue))
                                        //{
                                        //    iID = oBOICopy.Attributes[oBOInstance.BOD.sAuditBOfield.ToLower()].sValue;
                                        //}
                                    }
                                    //oBOICopy.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oBOInstance.BOD.sAuditBOName);
                                    oAuditBOI.SetAttribute("FKiBOID", oBOInstance.BOD.BOID.ToString());
                                    oAuditBOI.SetAttribute("sBOName", oBOInstance.BOD.Name);
                                    oAuditBOI.SetAttribute("sData", sChangedData);
                                    oAuditBOI.SetAttribute("sOldData", sPreviousData);
                                    //oAuditBOI.SetAttribute("XICreatedBy", "");
                                    //oAuditBOI.SetAttribute("XICreatedWhen", DateTime.Now.ToString());
                                    oAuditBOI.SetAttribute("sType", "User Edit");
                                    oAuditBOI.SetAttribute("sActivity", "Edit " + oBOInstance.BOD.LabelName + " By User:");
                                    oAuditBOI.SetAttribute("FKiInstanceID", iInstanceID.ToString());
                                    var oAuditBOResponse = Update_TODB(oAuditBOI, sIDRef);
                                }
                            }
                            else if (!string.IsNullOrEmpty(oBOInstance.BOD.sAuditBOName) && bIsAudit)
                            {
                                //XIInfraCache oCache = new XIInfraCache();
                                //XIIBO oAuditBOI = new XIIBO();
                                XIIBO oAuditBOI = oBOInstance.GetCopy();
                                string sAuditBOPrimaryKey = oAuditBOI.BOD.sPrimaryKey;
                                XIIXI oXIIXI = new XIIXI();
                                var iInstanceID = oBOInstance.Attributes[sPrimaryKey].sValue;
                                //merge current boi with previous values
                                //oAuditBOI = oAuditBOI.MergeAuditBOI(oAuditBOI);
                                //Load boi for previous data
                                oAuditBOI = oXIIXI.BOI(oAuditBOI.BOD.Name, iInstanceID);
                                oAuditBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, oBOInstance.BOD.sAuditBOName, null);
                                var oChangedData = oBOInstance.IsHavingChanges(oBOInstance, oAuditBOI);
                                string sIsHavingChanges = string.Empty;
                                if (oChangedData != null && oChangedData.Count() > 0)
                                {
                                    sIsHavingChanges = oChangedData.Where(x => x.sName.ToLower() == "bIsHavingChanges".ToLower()).Select(x => x.sValue).FirstOrDefault();
                                }
                                if (!string.IsNullOrEmpty(sIsHavingChanges) && sIsHavingChanges.ToLower() == "true")
                                {
                                    oAuditBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oBOInstance.BOD.sAuditBOName);
                                    if (!oAuditBOI.Attributes.ContainsKey("FKiInstanceID"))
                                    {
                                        oAuditBOI.Attributes["FKiInstanceID"] = new XIIAttribute { sName = "FKiInstanceID", sValue = oBOInstance.Attributes[sPrimaryKey].sValue };
                                    }
                                    if (!oAuditBOI.Attributes.ContainsKey("zXAuditCrtdWhn"))
                                    {
                                        oAuditBOI.Attributes["zXAuditCrtdWhn"] = new XIIAttribute { sName = "zXAuditCrtdWhn", sValue = DateTime.Now.ToString() };
                                    }
                                    int sPrimaryKeyValue = Convert.ToInt32(oAuditBOI.Attributes[sAuditBOPrimaryKey].sValue);
                                    if (sPrimaryKeyValue != 0)
                                    {
                                        oAuditBOI.Attributes.Where(n => n.Key.ToLower().Equals(sAuditBOPrimaryKey.ToLower())).Select(m => m.Value).ToList().ForEach(m => { m.sValue = null; });//set primary column to null
                                        oAuditBOI.Attributes.ToList().ForEach(m => m.Value.bDirty = true);
                                    }
                                    var oAuditBOResponse = Update_TODB(oAuditBOI, sIDRef);
                                }
                            }
                            string IDValue = "";
                            if (oBOInstance.sIDs != null && oBOInstance.sFields != null)
                            {
                                IDValue = string.Join(",", oBOInstance.sIDs);
                                foreach (var item in oBOInstance.sFields)
                                {
                                    sUpdateQuery = sUpdateQuery.Replace("'" + item + "'", item);
                                }
                            }
                            //sUpdateQuery = AddDefaultDataForUpdate(sUpdateQuery);
                            string cmdText = string.Empty;
                            if (IDValue.Contains(","))  // ID values contains comma seperated string,then apply WHERE IN
                            {
                                cmdText = "UPDATE " + TableName + " SET " + sUpdateQuery + " WHERE " + sPrimaryKey + " IN( " + IDValue + " )";
                            }
                            else
                            {
                                if (oBOInstance.BOD.bUID)
                                {
                                    cmdText = "UPDATE " + TableName + " SET" + " " + sUpdateQuery + " WHERE" + " xiguid='" + InsGUID + "'";
                                }
                                else if (!string.IsNullOrEmpty(sIDRef) && sIDRef.ToLower() == "xiguid" && (InsGUID != null && InsGUID != Guid.Empty))
                                {
                                    cmdText = "UPDATE " + TableName + " SET" + " " + sUpdateQuery + " WHERE" + " xiguid='" + InsGUID + "'";
                                }
                                else
                                {
                                    cmdText = "UPDATE " + TableName + " SET" + " " + sUpdateQuery + " WHERE" + " " + sPrimaryKey + "='" + sPKValue + "'";
                                }

                            }
                            sFinalQuery = cmdText;
                            //if (Convert.ToInt32(EnumTransactionEnabled.Yes) == oBOInstance.BOD.iTransactionEnable)
                            //    Execute_TransactionEnabledQuery(cmdText, oBOInstance.BOD.iDataSource, false);
                            //else
                            //    Execute_NormalQuery(cmdText, oBOInstance.BOD.iDataSource, false);
                            string uid = string.Empty;
                            if (TransactionIntiation != null)
                                uid = Execute_TransactionEnabledQuery(cmdText, false);
                            else
                                uid = Execute_NormalQuery(cmdText, oBOInstance.BOD.iDataSourceXIGUID.ToString(), false);

                            if (uid == "0")
                            {
                                if (!string.IsNullOrEmpty(oBOInstance.BOD.sTraceAttribute))
                                {
                                    UpdateTraceTransaction(oBOInstance);
                                }
                            }
                            else if (uid == "-1")
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                            //if (!string.IsNullOrEmpty(oBOInstance.BOD.sTraceAttribute) && oBOInstance.BOD.AttributeD(oBOInstance.BOD.sTraceAttribute).bIsTrace)
                            //{
                            //    UpdateTraceTransaction(oBOInstance);
                            //}                            
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                    if (!bIsEncrypted)
                    {
                        var iEncryptCount = 0;
                        var Attrs = oBOInstance.BOD.Attributes.Values.Where(m => m.bIsEncrypt == true || m.bIsUserEncrypt == true).ToList();
                        //var Role = oBOInstance.BOD.Attributes.Where(t => t.Value.Name.ToLower() == "iroleid").Select(y => y.Value.Value).FirstOrDefault();
                        if (Attrs != null && Attrs.Count() > 0)
                        {
                            XIInfraEncryption oEncrypt = new XIInfraEncryption();
                            foreach (var attr in Attrs)
                            {
                                var sValue = oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == attr.Name.ToLower()).Select(m => m.sValue).FirstOrDefault();
                                if (!string.IsNullOrEmpty(sValue))
                                {
                                    var EncryptVal = string.Empty;
                                    if (attr.bIsEncrypt == true)
                                    {
                                        EncryptVal = oEncrypt.EncryptData(sValue, true, sPKValue);
                                    }
                                    else if (attr.bIsUserEncrypt == true)
                                    {
                                        XIIXI oIXI = new XIIXI();
                                        //var UserDetail = oIXI.BOI("XIAPPUsers", iUserID.ToString());
                                        var UserDetails = oIXI.BOI("XIAppRoles_AR_T", iRoleID.ToString());
                                        var XIGUIDValue = UserDetails.Attributes.Values.Where(n => n.sName.ToLower() == "xiguid").Select(i => i.sValue).FirstOrDefault();
                                        EncryptVal = oEncrypt.EncryptData(sValue, true, sPKValue, XIGUIDValue);
                                    }
                                    oBOInstance.SetAttribute(attr.Name.ToLower(), EncryptVal);
                                    iEncryptCount++;
                                }
                            }
                        }
                        if (iEncryptCount > 0)
                        {
                            bIsEncrypted = true;
                            oBOInstance.Update_TODB(oBOInstance, sIDRef);
                        }
                    }
                }
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = oBOInstance;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - Error while inserting into " + BOName + " - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.sCategory = "DB query error";
                oCResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                SaveErrortoDB(oCResult);
                //oCResult.sMessage = "Error Query " + sFinalQuery;
                //oCResult.sCategory = "DB query error";
                //SaveErrortoDB(oCResult);
                oCResult.LogToFile();
            }
            return oCResult;
        }
        public void UpdateTraceTransaction(XIIBO oBOInstance)
        {
            CResult oCResult = new CResult();
            int /*iInstanceID = 0,*/ iBOID = 0, iAttrID = 0;
            //string sBOName = string.Empty;

            try
            {
                string sAttributeName = string.Empty;
                //var sGUID = Params.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                XIInfraCache oCache = new XIInfraCache();
                //int.TryParse(oCache.Get_ParamVal("", sGUID, "", "-iAttrID"), out iAttrID);

                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "TraceTransactions");
                var oCloneBOD = (XIDBO)oBOD.Clone(oBOD);
                oCloneBOD.Attributes = oCloneBOD.Attributes.ToDictionary(dic => dic.Key, dic => dic.Value);
                oCloneBOD.Scripts = new Dictionary<string, XIDScript>();
                //int.TryParse(Params.Where(t => t.sName == "iInstanceID").Select(f => f.sValue).FirstOrDefault(), out iInstanceID);
                //sBOName = Params.Where(t => t.sName == "sBO").Select(f => f.sValue).FirstOrDefault();
                //var oDynamicBO = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);



                //if (iAttrID > 0 && oDynamicBO?.BOID > 0 && oDynamicBO?.Attributes?.Count > 0)
                //{

                //    iBOID = oDynamicBO.BOID;
                //    sAttributeName = oDynamicBO.Attributes.Values.Where(m => m.ID == iAttrID && m.bIsTrace)?.Select(m => m.Name).FirstOrDefault();
                //}
                int iStatus = -1;
                //get status attribute on the table
                XIIXI oXII = new XIIXI();

                //var oResultI = oXII.BOI(sBOName, iInstanceID.ToString());

                if (oBOInstance.BOD != null && (oBOInstance.BOD.iDataSource > 0 || (oBOInstance.BOD.iDataSourceXIGUID != null && oBOInstance.BOD.iDataSourceXIGUID != Guid.Empty)))
                {
                    if (oBOInstance.BOD.iDataSourceXIGUID != null && oBOInstance.BOD.iDataSourceXIGUID != Guid.Empty)
                    {
                        oCloneBOD.iDataSourceXIGUID = oBOInstance.BOD.iDataSourceXIGUID;
                    }
                    else if (oBOInstance.BOD.iDataSource > 0)
                        oCloneBOD.iDataSource = oBOInstance.BOD.iDataSource;
                }

                //Get iStatus from the Inserted table

                //if (iInstanceID > 0 && iBOID > 0 && iStatus > -1)
                //{
                if (oBOInstance.BOD != null && (oBOInstance.BOD.iDataSource > 0 || (oBOInstance.BOD.iDataSourceXIGUID != null && oBOInstance.BOD.iDataSourceXIGUID != Guid.Empty)))
                {
                    if ((oBOInstance.BOD.iDataSourceXIGUID != null && oBOInstance.BOD.iDataSourceXIGUID != Guid.Empty))
                        oXII.SwitchDataSrcIDXIGUID = oBOInstance.BOD.iDataSourceXIGUID;
                    else if (oBOInstance.BOD.iDataSource > 0)
                        oXII.iSwitchDataSrcID = oBOInstance.BOD.iDataSource;
                }
                if (!string.IsNullOrEmpty(oBOInstance.BOD.sTraceAttribute))
                {
                    var TraceAttrs = oBOInstance.BOD.sTraceAttribute.Split(',');
                    foreach (var TraceAttr in TraceAttrs)
                    {
                        if (oBOInstance.BOD.Attributes.ContainsKey(TraceAttr) && oBOInstance.BOD.AttributeD(TraceAttr).bIsTrace)
                        {
                            sAttributeName = TraceAttr;
                            iAttrID = oBOInstance.BOD.AttributeD(TraceAttr).ID;
                            if (!string.IsNullOrEmpty(sAttributeName))
                                iStatus = oBOInstance.AttributeI(sAttributeName).iValue;
                            var oParams = new List<CNV>();
                            oParams.Add(new CNV { sName = "FKiBOID", sValue = oBOInstance.BOD.BOID.ToString() });
                            oParams.Add(new CNV { sName = "iInstanceID", sValue = oBOInstance.AttributeI("id").sValue });
                            oParams.Add(new CNV { sName = "FKiAttrID", sValue = iAttrID.ToString() });
                            var oResult = oXII.BOI("TraceTransactions", "", "", oParams);

                            //refTraceStage_T data based on iStatus
                            oParams = new List<CNV>();
                            oParams.Add(new CNV { sName = "iStatusValue1", sValue = iStatus.ToString() });
                            //oParams.Add(new CNV { sName = "sStatusName1", sValue = "iStatus" });
                            oParams.Add(new CNV { sName = "FKiAttrID", sValue = iAttrID.ToString() });
                            oParams.Add(new CNV { sName = "FKiBOID", sValue = oBOInstance.BOD.BOID.ToString() });
                            var oRefTraceI = oXII.BOI("RefTraceStage", "", "", oParams);
                            int iHighStageID = 0;
                            int iValidTraceID = 0;
                            string sTraceStage = string.Empty;
                            if (oRefTraceI?.Attributes?.Count() > 0)
                            {
                                iHighStageID = oRefTraceI.AttributeI("ID").iValue;
                                sTraceStage = oRefTraceI.AttributeI("sName").sValue;
                                oParams = new List<CNV>();
                                if (oResult != null && oResult.Attributes.Count > 0 && !string.IsNullOrEmpty(oResult.AttributeI("sTraceStage").sValue))
                                {
                                    oParams.Add(new CNV { sName = "sName", sValue = oResult.AttributeI("sTraceStage").sValue + "_" + sTraceStage });
                                }
                                else
                                {
                                    oParams.Add(new CNV { sName = "sName", sValue = sTraceStage });
                                }
                                var oTraceI = oXII.BOI("refValidTrace_T", "", "", oParams);
                                if (oTraceI != null && oTraceI.Attributes.Count() > 0)
                                {
                                    iValidTraceID = oTraceI.AttributeI("ID").iValue;
                                }

                            }
                            if (string.IsNullOrEmpty(sTraceStage))
                            {
                                oParams = new List<CNV>
                            {
                            new CNV { sName = "BOID", sValue = oBOInstance.BOD.XIGUID.ToString() },
                            new CNV { sName = "BOFieldID", sValue = iAttrID.ToString() },
                            new CNV { sName = "sValues", sValue = iStatus.ToString() }
                            };
                                var oAttrOptI = oXII.BOI("XIBOOptionList", "", "", oParams);
                                if (oAttrOptI?.Attributes?.Count > 0)
                                {
                                    sTraceStage = oAttrOptI.Attributes["sOptionName"].sValue;
                                    var oRefBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RefTraceStage");
                                    var oRefBOI = new XIIBO();
                                    oRefBOI.SetAttribute("sName", sTraceStage);
                                    oRefBOI.SetAttribute("sDescription", sTraceStage);
                                    oRefBOI.SetAttribute("iStatus", "10");
                                    oRefBOI.SetAttribute("sStatusName1", oAttrOptI.Attributes["sOptionName"].sName);
                                    oRefBOI.SetAttribute("iStatusValue1", iStatus.ToString());
                                    oRefBOI.SetAttribute("FKiBOID", oBOInstance.BOD.BOID.ToString());
                                    oRefBOI.SetAttribute("FKiAttrID", iAttrID.ToString());
                                    oRefBOI.BOD = oRefBOD;
                                    oRefBOI.Save(oRefBOI);
                                    iHighStageID = oRefBOI.Attributes["id"].iValue;
                                }

                            }
                            if (!string.IsNullOrEmpty(sTraceStage))
                                if (oResult?.Attributes?.Count > 0)
                                {

                                    string sExistingTraceStage = oResult.AttributeI("sTraceStage").sValue;
                                    if (!string.IsNullOrEmpty(sExistingTraceStage))
                                        sTraceStage = sExistingTraceStage + "_" + sTraceStage;
                                    else
                                        sTraceStage = sExistingTraceStage;

                                    int iCount = 0;
                                    //Count for FkiStageID and dtStage update
                                    if (!string.IsNullOrEmpty(sTraceStage))
                                        iCount = sTraceStage.Split('_').Count();

                                    //Update Trace Information
                                    if (iCount > 0 && iCount <= 10)
                                    {
                                        oResult.SetAttribute("FKiStage" + iCount + "ID", iHighStageID.ToString());
                                        oResult.SetAttribute("dtStage" + iCount, DateTime.Now.ToString());
                                    }

                                    oResult.SetAttribute("sTraceStage", sTraceStage);
                                    oResult.SetAttribute("FkiValidTraceID", iValidTraceID.ToString());
                                    //oResult.SetAttribute("FKiLeadQualityID", FKiLeadQualityID.ToString());
                                    //oResult.SetAttribute("FKiOutComesID", FkiOutComesID.ToString());
                                    oResult.SetAttribute("iHighStageID", iHighStageID.ToString());
                                    oResult.BOD = oCloneBOD;
                                    oResult.Save(oResult);
                                    //InsertValidTrace(iBOID, iAttrID, sTraceStage);
                                }
                                else
                                {
                                    //Insert TraceTransactions
                                    oResult = new XIIBO();
                                    oResult.SetAttribute("FKiAttrID", iAttrID.ToString());
                                    oResult.SetAttribute("FKiBOID", oBOInstance.BOD.BOID.ToString());
                                    oResult.SetAttribute("iInstanceID", oBOInstance.AttributeI("id").sValue);
                                    oResult.SetAttribute("sTraceStage", sTraceStage);
                                    oResult.SetAttribute("FKiStage1ID", iHighStageID.ToString());
                                    oResult.SetAttribute("dtStage1", DateTime.Now.ToString());
                                    oResult.SetAttribute("iHighStageID", iHighStageID.ToString());
                                    oResult.SetAttribute("FkiValidTraceID", iValidTraceID.ToString());
                                    oResult.BOD = oCloneBOD;
                                    oResult.Save(oResult);
                                    //InsertValidTrace(iBOID, iAttrID, iStatus.ToString());
                                }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                oCResult.sMessage = "ERROR: [" + sBOName + ":" + iInstanceID + "" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] " + ex.Message.ToString() + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
        }
        public CResult Update_TODBV2(XIIBO oBOInstance)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            string BOName = string.Empty;
            try
            {
                oTrace.sClass = "XIIBO";
                oTrace.sMethod = "Update_TODB";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                //oBOInstance.Attributes = AssignDefaultValues(oBOInstance);
                bool IsValidQuery = true;
                //if (oBOInstance.Attributes.ContainsKey("XIGUID") && oBOInstance.Attributes["XIGUID"] != null)
                //{
                //    if (string.IsNullOrEmpty(oBOInstance.Attributes["XIGUID"].sValue))
                //    {
                //        oBOInstance.Attributes["XIGUID"].sValue = Guid.NewGuid().ToString();
                //    }
                //}
                //else
                //{
                //    oBOInstance.Attributes["XIGUID"] = new XIIAttribute { sName = "XIGUID", sValue = Guid.NewGuid().ToString() };
                //}
                string sPrimaryKey = string.Empty;
                sPrimaryKey = oBOInstance.BOD.sPrimaryKey;
                if (string.IsNullOrEmpty(sPrimaryKey))
                {
                    sPrimaryKey = "id";
                }
                var sPKValue = string.Empty;
                sPKValue = oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == sPrimaryKey.ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (oBOInstance.Attributes.ContainsKey("XIGUID"))
                {
                    oBOInstance.Attributes.Remove("XIGUID");
                }
                var BOIAttrs = oBOInstance.Attributes.Values.Where(m => m.bDirty == true).ToList();
                if (BOIAttrs != null && BOIAttrs.Count() > 0)
                {
                    if (string.IsNullOrEmpty(sPKValue) || sPKValue == "0")
                    {
                        oBOInstance = AddDefaultDataForInsert(oBOInstance);
                    }
                    else
                    {
                        oBOInstance = AddDefaultDataForUpdate(oBOInstance);
                    }
                }
                var Columns = oBOInstance.Attributes.Values.Where(m => m.bDirty == true).Select(m => m.sName).ToList();
                var Data = oBOInstance.Attributes.Values.Where(m => m.bDirty == true).Select(m => m.sValue).ToList();
                string sQuery = "";
                StringBuilder sbQ = new StringBuilder();
                StringBuilder sbV = new StringBuilder();
                string sUpdateQuery = "";
                string sValues = string.Empty;

                string sFieldName = "";
                BOName = oBOInstance.BOD.Name;
                //if (oBOInstance.BOD.bUID)
                //{
                //sPrimaryKey = "XIGUID";

                //}
                //else
                //{

                //}

                using (IEnumerator<string> enumerator1 = Columns.GetEnumerator(), enumerator2 = Data.GetEnumerator())
                {
                    while (enumerator1.MoveNext() && enumerator2.MoveNext())
                    {
                        if (!string.IsNullOrEmpty(enumerator1.Current) && enumerator1.Current.ToLower() != sPrimaryKey.ToLower())
                        {
                            sFieldName = enumerator1.Current;
                            string sFieldValue = enumerator2.Current;
                            sFieldValue = sFieldValue == null ? null : sFieldValue.Trim();
                            XIDAttribute oAttrD = new XIDAttribute();
                            oAttrD = oBOInstance.BOD.Attributes.Values.ToList().Where(m => !string.IsNullOrEmpty(m.Name) && m.Name.ToLower() == sFieldName.ToLower()).FirstOrDefault();
                            //sQuery = sQuery + enumerator1.Current + "='" + enumerator2.Current + "', ";
                            if (oAttrD == null)
                            {
                                if (sFieldName.ToLower() == XIConstant.Key_XIDeleted.ToLower() || sFieldName.ToLower() == XIConstant.Key_XICrtdBy.ToLower() || sFieldName.ToLower() == XIConstant.Key_XICrtdWhn.ToLower() || sFieldName.ToLower() == XIConstant.Key_XIUpdtdBy.ToLower() || sFieldName.ToLower() == XIConstant.Key_XIUpdtdWhn.ToLower())
                                {
                                    sQuery = sQuery + sFieldName + ',';
                                    sbQ.Append(sFieldName + ",");

                                    if (sFieldName.ToLower() == XIConstant.Key_XICrtdWhn.ToLower() || sFieldName.ToLower() == XIConstant.Key_XIUpdtdWhn.ToLower())
                                    {
                                        string sCurrentValue = sFieldValue;
                                        if (sFieldName.ToLower() == XIConstant.Key_XIUpdtdWhn.ToLower())
                                        {
                                            sCurrentValue = DateTime.Now.ToString();
                                        }
                                        if (string.IsNullOrEmpty(sCurrentValue))
                                        {
                                            sCurrentValue = DateTime.Now.ToString();
                                        }
                                        var dtDate = ConvertToDtTime(sCurrentValue);
                                        sValues = sValues + "'" + dtDate.ToString(XIConstant.SqlDateFormat) + "'" + ',';
                                        sbV.Append("'" + dtDate.ToString(XIConstant.SqlDateFormat) + "'" + ',');
                                        sUpdateQuery = sUpdateQuery + sFieldName + "='" + dtDate.ToString(XIConstant.SqlDateFormat) + "'" + ',';
                                    }
                                    else
                                    {
                                        sValues = sValues + "'" + sFieldValue + "'" + ',';
                                        sbV.Append("'" + sFieldValue + "'" + ',');
                                        sUpdateQuery = sUpdateQuery + sFieldName + "='" + sFieldValue + "'" + ',';
                                    }
                                }
                                else
                                {
                                    oAttrD = oBOInstance.BOD.Attributes.Values.Where(x => x.LabelName.ToLower() == sFieldName.ToLower()).FirstOrDefault();
                                }

                            }
                            if (oAttrD != null)
                            {
                                //if ((bool)(oAttrD.CheckDatatype(enumerator2.Current, oAttrD.Get_BODatatype(oAttrD.TypeID))))
                                //{
                                //    sValues = sValues + "'" + enumerator2.Current + "'" + ',';
                                //    sUpdateQuery = sUpdateQuery + enumerator1.Current + "='" + enumerator2.Current + "'" + ',';
                                //}
                                //else
                                //{
                                //    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Mismatching value with the name of '" + enumerator1.Current + "' and the value is '" + enumerator2.Current + "'" });
                                //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                //    IsValidQuery = false;
                                //}
                                if (oAttrD.bTrack)
                                {
                                    if (!string.IsNullOrEmpty(sPKValue) && sPKValue != "0")
                                    {
                                        if (oBOInstance.Attributes[oAttrD.Name.ToLower()].sPreviousValue == null)
                                        {
                                            XIIXI oXI = new XIIXI();
                                            var oBOI = oXI.BOI(oBOInstance.BOD.Name, sPKValue);
                                            if (oBOI != null && oBOI.Attributes.Count() > 0)
                                            {
                                                var sPrevValue = oBOI.AttributeI(oAttrD.Name).sValue;
                                                if (!string.IsNullOrEmpty(sPrevValue))
                                                {
                                                    var oAtrI = oBOInstance.Attributes.Values.ToList().Where(m => m.sName.ToLower() == oAttrD.Name.ToLower()).FirstOrDefault();
                                                    if (oAtrI != null)
                                                    {
                                                        oBOInstance.Attributes.Values.ToList().Where(m => m.sName.ToLower() == oAttrD.Name.ToLower()).FirstOrDefault().sPreviousValue = sPrevValue;
                                                    }
                                                }
                                            }
                                        }
                                        if (oBOInstance.Attributes[oAttrD.Name.ToLower()].sValue != oBOInstance.Attributes[oAttrD.Name.ToLower()].sPreviousValue && oBOInstance.Attributes[oAttrD.Name.ToLower()].sPreviousValue != null)
                                        {
                                            XIInfraCache oCache = new XIInfraCache();
                                            XIIBO oTrackBOI = new XIIBO();
                                            oTrackBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, "XIBOAttributeTrack", null);
                                            oTrackBOI.SetAttribute("FKiBODID", oBOInstance.BOD.BOID.ToString());
                                            oTrackBOI.SetAttribute("FKiBODID", oBOInstance.BOD.XIGUID.ToString());
                                            oTrackBOI.SetAttribute("FKiAttributeDID", oAttrD.ID.ToString());
                                            oTrackBOI.SetAttribute("FKiAttributeDIDXIGUID", oAttrD.XIGUID.ToString());
                                            oTrackBOI.SetAttribute("iBOIID", oBOInstance.Attributes[sPrimaryKey.ToLower()].sValue);
                                            oTrackBOI.SetAttribute("sValue", oBOInstance.Attributes[oAttrD.Name.ToLower()].sPreviousValue);
                                            oTrackBOI.SetAttribute("sValueString", oBOInstance.Attributes[oAttrD.Name.ToLower()].sResolvedValue);
                                            oCR = Update_TODBV2(oTrackBOI);
                                        }

                                    }
                                }
                                if (oAttrD.FKiType > 0 && oAttrD.bFKGUID)
                                {
                                    if (!string.IsNullOrEmpty(sFieldValue))
                                    {
                                        int iFieldVal = 0;
                                        if (int.TryParse(sFieldValue, out iFieldVal))
                                        {
                                            oCR = Assign_FKGUIDAttribute(sFieldValue, oAttrD);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                var FKGUID = (string)oCR.oResult;
                                                sQuery = sQuery + oAttrD.Name + "XIGUID,";
                                                sbQ.Append(oAttrD.Name + "XIGUID,");
                                                sValues = sValues + "'" + FKGUID + "'" + ',';
                                                sbV.Append("'" + FKGUID + "'" + ',');
                                                sUpdateQuery = sUpdateQuery + oAttrD.Name + "XIGUID" + "='" + FKGUID + "'" + ',';
                                            }
                                        }
                                        else
                                        {
                                            oCR = Assign_FKIDAttribute(sFieldValue, oAttrD);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                var FKGUID = (string)oCR.oResult;
                                                var FKAttr = oAttrD.Name.Replace("XIGUID", "");
                                                sQuery = sQuery + FKAttr + ",";
                                                sbQ.Append(FKAttr + ",");
                                                sValues = sValues + "'" + FKGUID + "'" + ',';
                                                sbV.Append("'" + FKGUID + "'" + ',');
                                                sUpdateQuery = sUpdateQuery + FKAttr + "='" + FKGUID + "'" + ',';
                                            }
                                        }
                                    }
                                }
                                if (oAttrD.TypeID == 150 && !string.IsNullOrEmpty(sFieldValue))
                                {
                                    if (!string.IsNullOrEmpty(sFieldValue))
                                    {
                                        if ((sFieldValue == "1/1/1900 12:00:00 AM" || sFieldValue == "1/1/0001 12:00:00 AM"))
                                        {
                                            //sQuery = sQuery + oAttrD.Name + ',';
                                            //sbQ.Append(oAttrD.Name + ",");
                                            //sValues = sValues + "'',";
                                            //sbV.Append("'',");
                                            //sUpdateQuery = sUpdateQuery + oAttrD.Name + "=''" + ',';
                                        }
                                        else
                                        {
                                            var dtDate = ConvertToDtTime(sFieldValue);
                                            if (dtDate != DateTime.MinValue)
                                            {
                                                sQuery = sQuery + oAttrD.Name + ',';
                                                sbQ.Append(oAttrD.Name + ",");
                                                sValues = sValues + "'" + dtDate.ToString(XIConstant.SqlDateFormat) + "'" + ',';
                                                sbV.Append("'" + dtDate.ToString(XIConstant.SqlDateFormat) + "'" + ',');
                                                sUpdateQuery = sUpdateQuery + oAttrD.Name + "='" + dtDate.ToString(XIConstant.SqlDateFormat) + "'" + ',';
                                            }
                                            else
                                            {
                                                //sValues = sValues + "''" + ',';
                                                //sbV.Append("''" + ',');
                                                //sUpdateQuery = sUpdateQuery + oAttrD.Name + "=''" + ',';
                                            }
                                        }
                                    }
                                    else
                                    {
                                        sValues = sValues + "'" + sFieldValue + "'" + ',';
                                        sbV.Append("'" + sFieldValue + "'" + ',');
                                        sUpdateQuery = sUpdateQuery + oAttrD.Name + "='" + sFieldValue + "'" + ',';
                                    }
                                }
                                else if (oAttrD.TypeID == 20 && !string.IsNullOrEmpty(sFieldValue))
                                {
                                    sQuery = sQuery + oAttrD.Name + ',';
                                    sbQ.Append(oAttrD.Name + ",");
                                    string sString = sFieldValue;
                                    if (sFieldValue != null && sFieldValue.ToLower() == "on")
                                    {
                                        sString = "true";
                                    }
                                    sValues = sValues + "'" + sString + "'" + ',';
                                    sbV.Append("'" + sString + "'" + ',');
                                    sUpdateQuery = sUpdateQuery + oAttrD.Name + "='" + sString + "'" + ',';
                                }
                                else
                                {
                                    if (oAttrD.TypeID == 150 && string.IsNullOrEmpty(sFieldValue))
                                    {
                                    }
                                    else if (sFieldValue != "1/1/0001 12:00:00 AM")
                                    {
                                        sQuery = sQuery + oAttrD.Name + ',';
                                        sbQ.Append(oAttrD.Name + ",");
                                        string sString = string.Empty;
                                        if (!string.IsNullOrEmpty(sFieldValue) && sFieldValue.Contains("\'"))
                                        {
                                            sString = sFieldValue.Replace("\'", "\''");
                                        }
                                        else
                                        {
                                            sString = sFieldValue;
                                        }
                                        if (oAttrD.TypeID == 40)
                                        {
                                            if (string.IsNullOrEmpty(sString))
                                            {
                                                sString = "0";
                                            }
                                        }
                                        else if (oAttrD.TypeID == 250)
                                        {
                                            if (string.IsNullOrEmpty(sString))
                                            {
                                                sString = Guid.Empty.ToString();
                                            }
                                        }
                                        sValues = sValues + "'" + sString + "'" + ',';
                                        sbV.Append("'" + sString + "'" + ',');
                                        sUpdateQuery = sUpdateQuery + oAttrD.Name + "='" + sString + "'" + ',';
                                    }
                                    else
                                    {
                                        sValues = sValues + "' ',";
                                        sbV.Append("' ',");
                                        sUpdateQuery = sUpdateQuery + oAttrD.Name + "=' ',";
                                    }
                                }
                            }
                        }
                    }
                }
                if (IsValidQuery)
                {
                    sQuery = sQuery.Substring(0, sQuery.Length - 1);
                    sValues = sValues.Substring(0, sValues.Length - 1);
                    sbV.Remove(sbV.Length - 1, 1);
                    sbQ.Remove(sbQ.Length - 1, 1); //remove Last Character comma
                    sUpdateQuery = sUpdateQuery.Substring(0, sUpdateQuery.Length - 1);
                    var sBODataSource = string.Empty;
                    XIDXI oXID = new XIDXI();
                    //sBODataSource = oXID.GetBODataSource(oBOInstance.BOD.iDataSource,oBOInstance.BOD.FKiApplicationID);
                    var TableName = string.Empty;
                    if (oBOInstance.BOD.TableName != null)
                    {
                        TableName = oBOInstance.BOD.TableName;
                    }
                    else
                    {
                        TableName = oBOInstance.BOD.Name;
                    }
                    if (string.IsNullOrEmpty(sPKValue) || sPKValue == "0")
                    {
                        string cmdText = "";
                        if (oBOInstance.BOD.bIsAutoIncrement)
                        {
                            //sQuery = AddDefaultColumnsForInsert(sQuery);
                            //if (!bIsCreatedBy)
                            //{
                            //    sValues = AddDefaultDataForInsert(sQuery, sValues);
                            //}
                            cmdText = "INSERT INTO " + TableName + "(" + sQuery + ") output INSERTED." + sPrimaryKey + " VALUES(" + sValues + ")";
                        }
                        else
                        {
                            //string _sbq = sbQ.ToString();
                            if (sPrimaryKey.ToLower() != "id")
                            {
                                sQuery = sPrimaryKey + "," + sQuery;
                                sbQ.Insert(0, sPrimaryKey + ",");
                                cmdText = "DECLARE @UNIQUEX UNIQUEIDENTIFIER; SET @UNIQUEX = NEWID(); INSERT INTO " + TableName + "(" + sQuery + ") output inserted." + sPrimaryKey + " VALUES(@UNIQUEX," + sValues + ")";
                            }
                            else
                            {
                                sQuery = sPrimaryKey + "," + sQuery;
                                sbQ.Insert(0, sPrimaryKey + ",");
                                sValues = "isnull(@max,0)+1," + sValues;
                                sbV.Append("@max+1,");
                                //sQuery = AddDefaultColumnsForInsert(sQuery);
                                //if (!bIsCreatedBy)
                                //{
                                //    sValues = AddDefaultDataForInsert(sQuery, sValues);
                                //}
                                //cmdText = "declare @max int; set @max = (SELECT MAX(" + sPrimaryKey + ") FROM " + TableName + "); INSERT INTO " + TableName + "(" + sQuery + ") output inserted." + sPrimaryKey + " VALUES(" + sValues + ")";
                                cmdText = "DECLARE @Tableobj TABLE (" + sPrimaryKey + " INT) declare @max int; set @max = (SELECT MAX(" + sPrimaryKey + ") FROM " + TableName + "); INSERT INTO " + TableName + "(" + sQuery + ") output inserted." + sPrimaryKey + " INTO @Tableobj VALUES(" + sValues + ") SELECT * FROM @Tableobj";
                            }
                        }
                        sFinalQuery = cmdText;
                        oTrace.sQuery = sFinalQuery;
                        string iInstanceID = "0";
                        //if (Convert.ToInt32(EnumTransactionEnabled.Yes) == oBOInstance.BOD.iTransactionEnable)
                        if (TransactionIntiation != null)
                        {
                            iInstanceID = Execute_TransactionEnabledQuery(cmdText, true);
                        }
                        else
                        {
                            oCR = Execute_NormalQueryV2(cmdText, oBOInstance.BOD.iDataSourceXIGUID.ToString(), true);
                            oTrace.oTrace.Add(oCR.oTrace);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                iInstanceID = (string)oCR.oResult;
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        if (iInstanceID == "0")
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;

                        var IDPair = oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == sPrimaryKey.ToLower()).FirstOrDefault();
                        if (IDPair != null)
                        {
                            if (iInstanceID != "0")
                            {

                            }
                            oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == sPrimaryKey.ToLower()).FirstOrDefault().sValue = iInstanceID.ToString();
                        }
                        else
                        {
                            XIIAttribute oAttrI = new XIIAttribute();
                            oAttrI.sName = sPrimaryKey.ToLower();
                            oAttrI.sValue = iInstanceID;
                            oBOInstance.Attributes[sPrimaryKey.ToLower()] = oAttrI;
                        }
                        sPKValue = iInstanceID;
                        if (!string.IsNullOrEmpty(oBOInstance.BOD.sAuditBOName) && bIsAudit && oBOInstance.BOD.sAuditBOName == "Audit_T" && !string.IsNullOrEmpty(oBOInstance.BOD.sAuditBOfield) && oBOInstance.BOD.sAuditBOfield.ToLower() != oBOInstance.BOD.sPrimaryKey.ToLower() && oBOInstance.Attributes.ContainsKey(oBOInstance.BOD.sAuditBOfield.ToLower()))
                        {
                            XIInfraCache oCache = new XIInfraCache();
                            //XIIBO oAuditBOI = new XIIBO();
                            XIIBO oBOICopy = oBOInstance.GetCopy();
                            string sAuditBOPrimaryKey = oBOICopy.BOD.sPrimaryKey;
                            XIIXI oXIIXI = new XIIXI();
                            var sID = oBOInstance.Attributes[sAuditBOPrimaryKey.ToLower()].sValue;

                            //merge current boi with previous values
                            //oAuditBOI = oAuditBOI.MergeAuditBOI(oAuditBOI);

                            //oAuditBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, oBOInstance.BOD.sAuditBOName, null);
                            //var oChangedData = oBOInstance.IsHavingChanges(oBOInstance, oBOICopy);
                            //string sIsHavingChanges = string.Empty;
                            //if (oChangedData != null && oChangedData.Count() > 0)
                            //{
                            //    sIsHavingChanges = oChangedData.Where(x => x.sName.ToLower() == "bIsHavingChanges".ToLower()).Select(x => x.sValue).FirstOrDefault();
                            //}
                            int iID = 0;
                            if (int.TryParse(sID, out iID)) { }
                            if (iID > 0)
                            {
                                //    if (oChangedData!=null && oChangedData.Count()>0)
                                //{
                                string sChangedData = "New " + oBOInstance.BOD.LabelName.ToLower() + " added with this id:" + iInstanceID;
                                //string sPreviousData = oChangedData.Where(x => x.sName.ToLower() == "sPreviousData".ToLower()).Select(x => x.sValue).FirstOrDefault();
                                XIIBO oAuditBOI = new XIIBO();
                                if (!string.IsNullOrEmpty(oBOICopy.BOD.Attributes[oBOICopy.BOD.sAuditBOfield].sFKBOName))
                                {
                                    var oParentBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oBOICopy.BOD.Attributes[oBOICopy.BOD.sAuditBOfield].sFKBOName, null);
                                    XIIXI oXII = new XIIXI();
                                    int iBOIID = 0;
                                    //oBOI = oXII.BOI(sBOName, sInstanceID);
                                    if (oBOICopy != null && oBOICopy.Attributes.Count() > 0 && oBOICopy.Attributes.ContainsKey(BOD.sAuditBOfield.ToLower()) && !string.IsNullOrEmpty(oBOICopy.Attributes[BOD.sAuditBOfield.ToLower()].sValue))
                                    {
                                        if (int.TryParse(oBOICopy.Attributes[BOD.sAuditBOfield.ToLower()].sValue, out iBOIID)) { }
                                    }
                                    if (iBOIID == 0 && oBOInstance.Attributes.ContainsKey(BOD.sAuditBOfield) && !string.IsNullOrEmpty(oBOInstance.Attributes[BOD.sAuditBOfield].sValue))
                                    {
                                        int.TryParse(oBOInstance.Attributes[BOD.sAuditBOfield].sValue, out iBOIID);
                                    }
                                    oAuditBOI.SetAttribute("FksParentBOName", oParentBOD.Name);
                                    oAuditBOI.SetAttribute("FKiParentInstanceID", iBOIID.ToString());
                                    oAuditBOI.SetAttribute("FkiParentBOID", oParentBOD.BOID.ToString());
                                }
                                oAuditBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, oBOInstance.BOD.sAuditBOName, null);
                                //oBOICopy.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oBOInstance.BOD.sAuditBOName);
                                oAuditBOI.SetAttribute("FKiBOID", oBOInstance.BOD.BOID.ToString());
                                oAuditBOI.SetAttribute("sBOName", oBOInstance.BOD.Name);
                                oAuditBOI.SetAttribute("sData", sChangedData);
                                oAuditBOI.SetAttribute("sOldData", "");
                                //oAuditBOI.SetAttribute("XICreatedBy", "");
                                //oAuditBOI.SetAttribute("XICreatedWhen", DateTime.Now.ToString());
                                oAuditBOI.SetAttribute("sType", "New " + oBOInstance.BOD.LabelName);
                                oAuditBOI.SetAttribute("sActivity", "New " + oBOInstance.BOD.LabelName);
                                oAuditBOI.SetAttribute("FKiInstanceID", iID.ToString());
                                var oAuditBOResponse = Update_TODBV2(oAuditBOI);
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(oBOInstance.BOD.sAuditBOName) && bIsAudit && oBOInstance.BOD.sAuditBOName == "Audit_T")
                        //if (oBOInstance.BOD.sAudit == "30" && !string.IsNullOrEmpty(oBOInstance.BOD.sAuditBOName) && bIsAudit)
                        {
                            XIInfraCache oCache = new XIInfraCache();
                            //XIIBO oAuditBOI = new XIIBO();
                            XIIBO oBOICopy = oBOInstance.GetCopy();
                            string sAuditBOPrimaryKey = oBOICopy.BOD.sPrimaryKey;
                            XIIXI oXIIXI = new XIIXI();
                            var iInstanceID = oBOInstance.Attributes[sPrimaryKey.ToLower()].sValue;
                            //merge current boi with previous values
                            //oAuditBOI = oAuditBOI.MergeAuditBOI(oAuditBOI);
                            //Load boi for previous data
                            oBOICopy = oXIIXI.BOI(oBOICopy.BOD.Name, iInstanceID);
                            //oAuditBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, oBOInstance.BOD.sAuditBOName, null);
                            var oChangedData = oBOInstance.IsHavingChanges(oBOInstance, oBOICopy);
                            string sIsHavingChanges = string.Empty;
                            if (oChangedData != null && oChangedData.Count() > 0)
                            {
                                sIsHavingChanges = oChangedData.Where(x => x.sName.ToLower() == "bIsHavingChanges".ToLower()).Select(x => x.sValue).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(sIsHavingChanges) && sIsHavingChanges.ToLower() == "true")
                            {
                                //    if (oChangedData!=null && oChangedData.Count()>0)
                                //{
                                string sChangedData = oChangedData.Where(x => x.sName.ToLower() == "sChangedData".ToLower()).Select(x => x.sValue).FirstOrDefault();
                                string sPreviousData = oChangedData.Where(x => x.sName.ToLower() == "sPreviousData".ToLower()).Select(x => x.sValue).FirstOrDefault();
                                XIIBO oAuditBOI = new XIIBO();
                                oAuditBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, oBOInstance.BOD.sAuditBOName, null);
                                if (!string.IsNullOrEmpty(oBOInstance.BOD.sAuditBOfield) && oBOICopy.Attributes.ContainsKey(oBOInstance.BOD.sAuditBOfield.ToLower()))
                                {
                                    if (!string.IsNullOrEmpty(oBOICopy.BOD.Attributes[oBOICopy.BOD.sAuditBOfield].sFKBOName))
                                    {
                                        var oParentBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oBOICopy.BOD.Attributes[oBOICopy.BOD.sAuditBOfield].sFKBOName, null);
                                        XIIXI oXII = new XIIXI();
                                        int iBOIID = 0;
                                        //oBOI = oXII.BOI(sBOName, sInstanceID);
                                        if (oBOICopy != null && oBOICopy.Attributes.Count() > 0 && oBOICopy.Attributes.ContainsKey(BOD.sAuditBOfield) && !string.IsNullOrEmpty(oBOICopy.Attributes[BOD.sAuditBOfield].sValue))
                                        {
                                            if (int.TryParse(oBOICopy.Attributes[BOD.sAuditBOfield].sValue, out iBOIID)) { }
                                        }
                                        if (iBOIID == 0 && oBOInstance.Attributes.ContainsKey(BOD.sAuditBOfield) && !string.IsNullOrEmpty(oBOInstance.Attributes[BOD.sAuditBOfield].sValue))
                                        {
                                            int.TryParse(oBOInstance.Attributes[BOD.sAuditBOfield].sValue, out iBOIID);
                                        }
                                        oAuditBOI.SetAttribute("FksParentBOName", oParentBOD.Name);
                                        oAuditBOI.SetAttribute("FKiParentInstanceID", iBOIID.ToString());
                                        oAuditBOI.SetAttribute("FkiParentBOID", oParentBOD.BOID.ToString());
                                    }
                                    //if (!string.IsNullOrEmpty(oBOICopy.Attributes[oBOInstance.BOD.sAuditBOfield.ToLower()].sValue))
                                    //{
                                    //    iID = oBOICopy.Attributes[oBOInstance.BOD.sAuditBOfield.ToLower()].sValue;
                                    //}
                                }
                                //oBOICopy.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oBOInstance.BOD.sAuditBOName);
                                oAuditBOI.SetAttribute("FKiBOID", oBOInstance.BOD.BOID.ToString());
                                oAuditBOI.SetAttribute("sBOName", oBOInstance.BOD.Name);
                                oAuditBOI.SetAttribute("sData", sChangedData);
                                oAuditBOI.SetAttribute("sOldData", sPreviousData);
                                //oAuditBOI.SetAttribute("XICreatedBy", "");
                                //oAuditBOI.SetAttribute("XICreatedWhen", DateTime.Now.ToString());
                                oAuditBOI.SetAttribute("sType", "User Edit");
                                oAuditBOI.SetAttribute("sActivity", "Edit " + oBOInstance.BOD.LabelName + " By User:");
                                oAuditBOI.SetAttribute("FKiInstanceID", iInstanceID.ToString());
                                var oAuditBOResponse = Update_TODBV2(oAuditBOI);
                            }
                        }
                        else if (!string.IsNullOrEmpty(oBOInstance.BOD.sAuditBOName) && bIsAudit)
                        {
                            XIInfraCache oCache = new XIInfraCache();
                            //XIIBO oAuditBOI = new XIIBO();
                            XIIBO oAuditBOI = oBOInstance.GetCopy();
                            string sAuditBOPrimaryKey = oAuditBOI.BOD.sPrimaryKey;
                            XIIXI oXIIXI = new XIIXI();
                            var iInstanceID = oBOInstance.Attributes[sPrimaryKey].sValue;
                            //merge current boi with previous values
                            //oAuditBOI = oAuditBOI.MergeAuditBOI(oAuditBOI);
                            //Load boi for previous data
                            oAuditBOI = oXIIXI.BOI(oAuditBOI.BOD.Name, iInstanceID);
                            oAuditBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, oBOInstance.BOD.sAuditBOName, null);
                            var oChangedData = oBOInstance.IsHavingChanges(oBOInstance, oAuditBOI);
                            string sIsHavingChanges = string.Empty;
                            if (oChangedData != null && oChangedData.Count() > 0)
                            {
                                sIsHavingChanges = oChangedData.Where(x => x.sName.ToLower() == "bIsHavingChanges".ToLower()).Select(x => x.sValue).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(sIsHavingChanges) && sIsHavingChanges.ToLower() == "true")
                            {
                                oAuditBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oBOInstance.BOD.sAuditBOName);
                                if (!oAuditBOI.Attributes.ContainsKey("FKiInstanceID"))
                                {
                                    oAuditBOI.Attributes["FKiInstanceID"] = new XIIAttribute { sName = "FKiInstanceID", sValue = oBOInstance.Attributes[sPrimaryKey].sValue };
                                }
                                if (!oAuditBOI.Attributes.ContainsKey("zXAuditCrtdWhn"))
                                {
                                    oAuditBOI.Attributes["zXAuditCrtdWhn"] = new XIIAttribute { sName = "zXAuditCrtdWhn", sValue = DateTime.Now.ToString() };
                                }
                                int sPrimaryKeyValue = Convert.ToInt32(oAuditBOI.Attributes[sAuditBOPrimaryKey].sValue);
                                if (sPrimaryKeyValue != 0)
                                {
                                    oAuditBOI.Attributes.Where(n => n.Key.ToLower().Equals(sAuditBOPrimaryKey.ToLower())).Select(m => m.Value).ToList().ForEach(m => { m.sValue = null; });//set primary column to null
                                    oAuditBOI.Attributes.ToList().ForEach(m => m.Value.bDirty = true);
                                }
                                var oAuditBOResponse = Update_TODBV2(oAuditBOI);
                            }
                        }
                        string IDValue = "";
                        if (oBOInstance.sIDs != null && oBOInstance.sFields != null)
                        {
                            IDValue = string.Join(",", oBOInstance.sIDs);
                            foreach (var item in oBOInstance.sFields)
                            {
                                sUpdateQuery = sUpdateQuery.Replace("'" + item + "'", item);
                            }
                        }
                        //sUpdateQuery = AddDefaultDataForUpdate(sUpdateQuery);
                        string cmdText = string.Empty;
                        if (IDValue.Contains(","))  // ID values contains comma seperated string,then apply WHERE IN
                        {
                            cmdText = "UPDATE " + TableName + " SET " + sUpdateQuery + " WHERE " + sPrimaryKey + " IN( " + IDValue + " )";
                        }
                        else
                        {
                            cmdText = "UPDATE " + TableName + " SET" + " " + sUpdateQuery + " WHERE" + " " + sPrimaryKey + "='" + sPKValue + "'";
                        }
                        sFinalQuery = cmdText;
                        oTrace.sQuery = sFinalQuery;
                        //if (Convert.ToInt32(EnumTransactionEnabled.Yes) == oBOInstance.BOD.iTransactionEnable)
                        //    Execute_TransactionEnabledQuery(cmdText, oBOInstance.BOD.iDataSource, false);
                        //else
                        //    Execute_NormalQuery(cmdText, oBOInstance.BOD.iDataSource, false);
                        if (TransactionIntiation != null)
                            Execute_TransactionEnabledQuery(cmdText, false);
                        else
                            oCR = Execute_NormalQueryV2(cmdText, oBOInstance.BOD.iDataSourceXIGUID.ToString(), false);
                        oTrace.oTrace.Add(oCR.oTrace);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                }
                else
                {
                    throw new Exception();
                }
                if (!bIsEncrypted)
                {
                    var iEncryptCount = 0;
                    var Attrs = oBOInstance.BOD.Attributes.Values.Where(m => m.bIsEncrypt == true).ToList();
                    if (Attrs != null && Attrs.Count() > 0)
                    {
                        XIInfraEncryption oEncrypt = new XIInfraEncryption();
                        foreach (var attr in Attrs)
                        {
                            var sValue = oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == attr.Name.ToLower()).Select(m => m.sValue).FirstOrDefault();
                            if (!string.IsNullOrEmpty(sValue))
                            {
                                var EncryptVal = oEncrypt.EncryptData(sValue, true, sPKValue);
                                oBOInstance.SetAttribute(attr.Name.ToLower(), EncryptVal);
                                iEncryptCount++;
                            }
                        }
                    }
                    if (iEncryptCount > 0)
                    {
                        bIsEncrypted = true;
                        oBOInstance.Update_TODBV2(oBOInstance);
                    }
                }

                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oBOInstance;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - Error while inserting into " + BOName + " - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.sMessage = "Error Query " + sFinalQuery;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
                oTrace.sMessage = ex.ToString();
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        private string AddDefaultColumnsForInsert(string sQuery)
        {
            if (sQuery.IndexOf(XIConstant.Key_XICrtdBy, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                bIsCreatedBy = true;
            }
            else
            {
                if (sQuery.IndexOf(XIConstant.Key_XIDeleted, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    sQuery = sQuery + "," + XIConstant.Key_XICrtdBy + "," + XIConstant.Key_XICrtdWhn + "," + XIConstant.Key_XIUpdtdBy + "," + XIConstant.Key_XIUpdtdWhn;
                }
                else
                {
                    sQuery = sQuery + "," + XIConstant.Key_XIDeleted + "," + XIConstant.Key_XICrtdBy + "," + XIConstant.Key_XICrtdWhn + "," + XIConstant.Key_XIUpdtdBy + "," + XIConstant.Key_XIUpdtdWhn;
                }
            }

            return sQuery;
        }

        private XIIBO AddDefaultDataForInsert(XIIBO oBOI)
        {
            XIInfraUsers oUser = new XIInfraUsers();
            CUserInfo oInfo = new CUserInfo();
            oInfo = oUser.Get_UserInfo();
            //oInfo = oInfo.GetUserInfo();
            string sUserName = oInfo.sShortName;//oInfo.sName == null ? null : (oInfo.sName.Length >= 15 ? oInfo.sName.Substring(0, 14) : oInfo.sName);
            if (oInfo.sShortName== null) 
            { sUserName = "Pc lines"; } //hardcoded for signalr(Createdby and updatedby columns some of the tables are having not to allow null in design so below code is failing while saving the instance)
            if (!oBOI.Attributes.ContainsKey(XIConstant.Key_XIDeleted) && !oBOI.Attributes.ContainsKey(XIConstant.Key_XIDeleted.ToLower()))
            {
                oBOI.SetAttribute(XIConstant.Key_XIDeleted, "0");
            }
            if (!oBOI.Attributes.ContainsKey(XIConstant.Key_XICrtdBy) && !oBOI.Attributes.ContainsKey(XIConstant.Key_XICrtdBy.ToLower()))
            {
                oBOI.SetAttribute(XIConstant.Key_XICrtdBy, sUserName);
            }
            if (!oBOI.Attributes.ContainsKey(XIConstant.Key_XICrtdWhn) && !oBOI.Attributes.ContainsKey(XIConstant.Key_XICrtdWhn.ToLower()))
            {
                oBOI.SetAttribute(XIConstant.Key_XICrtdWhn, DateTime.Now.ToString());
            }
            if (!oBOI.Attributes.ContainsKey(XIConstant.Key_XIUpdtdBy) && !oBOI.Attributes.ContainsKey(XIConstant.Key_XIUpdtdBy.ToLower()))
            {
                oBOI.SetAttribute(XIConstant.Key_XIUpdtdBy, sUserName);
            }
            if (!oBOI.Attributes.ContainsKey(XIConstant.Key_XIUpdtdWhn) && !oBOI.Attributes.ContainsKey(XIConstant.Key_XIUpdtdWhn.ToLower()))
            {
                oBOI.SetAttribute(XIConstant.Key_XIUpdtdWhn, DateTime.Now.ToString());
            }
            if ((!oBOI.Attributes.ContainsKey("FKiOrgID") && !oBOI.Attributes.ContainsKey("fkiorgid")) && (oBOI.BOD.Attributes.ContainsKey("FKiOrgID".ToLower()) || oBOI.BOD.Attributes.ContainsKey("FKiOrgID")))
            {
                oBOI.SetAttribute("FKiOrgID", oInfo.iOrganizationID.ToString());
            }
            if ((!oBOI.Attributes.ContainsKey("FKiAppID") && !oBOI.Attributes.ContainsKey("fkiappid")) && (oBOI.BOD.Attributes.ContainsKey("FKiAppID".ToLower()) || oBOI.BOD.Attributes.ContainsKey("FKiAppID")))
            {
                oBOI.SetAttribute("FKiAppID", oInfo.iApplicationID.ToString());
            }
            if ((!oBOI.Attributes.ContainsKey("FKiApplicationID") && !oBOI.Attributes.ContainsKey("fkiapplicationid")) && (oBOI.BOD.Attributes.ContainsKey("FKiApplicationID".ToLower()) || oBOI.BOD.Attributes.ContainsKey("FKiApplicationID")))
            {
                oBOI.SetAttribute("FKiApplicationID", oInfo.iApplicationID.ToString());
            }
            return oBOI;
        }

        private XIIBO AddDefaultDataForUpdate(XIIBO oBOI)
        {
            XIInfraUsers oUser = new XIInfraUsers();
            CUserInfo oInfo = new CUserInfo();
            oInfo = oUser.Get_UserInfo();
            string sUserName = oInfo.sShortName;//oInfo.sName == null ? null : (oInfo.sName.Length >= 15 ? oInfo.sName.Substring(0, 14) : oInfo.sName);
            //oInfo = oInfo.GetUserInfo();
            if (oInfo.sShortName == null)
            { sUserName = "Pc lines"; } //hardcoded for signalr(updatedby column some of the tables are having not to allow null in design so below code is failing while saving the instance)
            if (!oBOI.Attributes.ContainsKey(XIConstant.Key_XIUpdtdBy))
            {
                oBOI.SetAttribute(XIConstant.Key_XIUpdtdBy, sUserName);
            }
            else
            {
                oBOI.SetAttribute(XIConstant.Key_XIUpdtdBy, sUserName);
            }
            if (!oBOI.Attributes.ContainsKey(XIConstant.Key_XIUpdtdWhn))
            {
                oBOI.SetAttribute(XIConstant.Key_XIUpdtdWhn, DateTime.Now.ToString());
            }
            else
            {
                oBOI.SetAttribute(XIConstant.Key_XIUpdtdWhn, DateTime.Now.ToString());
            }

            return oBOI;
        }

        public string ConvertToDateTime(string InputString)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                string[] formats = {
                  "yyyy-MM-dd", "yyyy-MMM-dd", "yyyy.MM.dd","yyyy/MM/dd","yyyy/MMM/dd","yyyy.MMM.dd",
                  "dd-MM-yyyy","dd.MM.yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "dd.MMM.yyyy",
                  "MMM-dd-yyyy","MM-dd-yyyy", "MM.dd.yyyy", "MMM.dd.yyyy", "MM/dd/yyyy", "mm/dd/yyyy hh:mm:ss tt tt","d/m/yyyy","m/d/yyyy"
              };
                if (InputString.IndexOf(" ") > 0)
                {
                    InputString = InputString.Split(' ')[0];
                    if (InputString.Contains('/'))
                    {
                        var datearray = InputString.Split('/');
                        if (datearray[2] == "1900" || datearray[2] == "0001")
                        {
                            datearray[2] = "1920";
                        }
                        InputString = datearray[1] + "/" + datearray[0] + "/" + datearray[2];
                    }
                    else if (InputString.Contains('-'))
                    {
                        var datearray = InputString.Split('-');
                        int month = 0;
                        if (int.TryParse(datearray[1], out month))
                        {
                            // It was assigned.
                        }
                        if (datearray[2] == "1900" || datearray[2] == "0001")
                        {
                            datearray[2] = "1920";
                        }
                        if (month > 0)
                        {
                            InputString = datearray[1] + "/" + datearray[0] + "/" + datearray[2];
                        }
                        else
                        {
                            InputString = datearray[0] + "/" + datearray[1] + "/" + datearray[2];
                        }
                    }
                }
                else
                {
                    if (InputString.Contains('/'))
                    {
                        var datearray = InputString.Split('/');
                        if (datearray[2] == "1900" || datearray[2] == "0001")
                        {
                            datearray[2] = "1920";
                        }
                        InputString = datearray[1] + "/" + datearray[0] + "/" + datearray[2];
                    }
                    else if (InputString.Contains('-'))
                    {
                        var datearray = InputString.Split('-');
                        int month = 0;
                        if (int.TryParse(datearray[1], out month))
                        {
                            // It was assigned.
                        }
                        if (datearray[2] == "1900" || datearray[2] == "0001")
                        {
                            datearray[2] = "1920";
                        }
                        if (month > 0)
                        {
                            InputString = datearray[1] + "/" + datearray[0] + "/" + datearray[2];
                        }
                        else
                        {
                            InputString = datearray[0] + "/" + datearray[1] + "/" + datearray[2];
                        }
                    }
                }
                DateTime dateValue;
                // var dt = "26.May.1975";
                bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);
                if (!IsValidDate)
                {
                    dateValue = DateTime.MinValue;
                }
                else
                {
                    InputString = dateValue.ToString("dd-MMM-yy");
                }
                return InputString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTime ConvertToDtTime(string InputString)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                string[] formats = {
"yyyy-MM-dd", "yyyy-MMM-dd", "yyyy.MM.dd","yyyy/MM/dd","yyyy/MMM/dd","yyyy.MMM.dd",
"dd-MM-yyyy","dd.MM.yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "dd.MMM.yyyy",
"MMM-dd-yyyy","MM-dd-yyyy", "MM.dd.yyyy", "MMM.dd.yyyy", "MM/dd/yyyy","d-MMM-yyyy", "yyyy-MMMM-dd","MMMM-dd-yyyy", "dd-MMMM-yyyy",

"dd/MM/yyyy HH:mm:ss",

"MM/dd/yyyy hh:mm:ss tt","MM/dd/yyyy hh:m:ss tt","MM/dd/yyyy hh:mm:s tt","MM/dd/yyyy hh:m:s tt","MM/dd/yyyy h:mm:s tt",
"MM/dd/yyyy h:mm:ss tt","MM/dd/yyyy h:m:ss tt","MM/dd/yyyy h:m:s tt","MM/dd/yyyy hh:mm:ss tt","M/dd/yyyy hh:m:ss tt",
"M/dd/yyyy hh:mm:s tt","M/dd/yyyy hh:m:s tt","M/dd/yyyy h:mm:s tt","M/dd/yyyy h:mm:ss tt","M/dd/yyyy h:m:ss tt","M/dd/yyyy h:m:s tt",

"MM/d/yyyy hh:mm:ss tt","MM/d/yyyy hh:m:ss tt","MM/d/yyyy hh:mm:s tt","MM/d/yyyy hh:m:s tt","MM/d/yyyy h:mm:s tt","MM/d/yyyy h:mm:ss tt",
"MM/d/yyyy h:m:ss tt","MM/d/yyyy h:m:s tt","MM/d/yyyy hh:mm:ss tt","M/d/yyyy hh:m:ss tt","M/d/yyyy hh:mm:s tt","M/d/yyyy hh:m:s tt",
"M/d/yyyy h:mm:s tt","M/d/yyyy h:mm:ss tt","M/d/yyyy h:m:ss tt","M/d/yyyy h:m:s tt",


"MM/dd/yyyy hh:mm:ss tt", "M/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "yyyy-MM-dd hh:mm:ss tt", "yyyy-MM-dd h:mm:ss tt", "yyyy-MMM-dd hh:mm:ss tt", "yyyy.MM.dd hh:mm:ss tt","yyyy/MM/dd hh:mm:ss tt","yyyy/MMM/dd hh:mm:ss tt","yyyy.MMM.dd hh:mm:ss tt",
"dd-MM-yyyy h:m:s tt","dd-MM-yyyy hh:mm:ss tt","dd.MM.yyyy hh:mm:ss tt", "dd/MM/yyyy hh:mm:ss tt", "dd-MMM-yyyy hh:mm:ss tt", "dd.MMM.yyyy hh:mm:ss tt",
"MMM-dd-yyyy hh:mm:ss tt","MM-dd-yyyy hh:mm:ss tt", "MM.dd.yyyy hh:mm:ss tt", "MMM.dd.yyyy hh:mm:ss tt",

"dd/MM/yyyy HH:mm:ss",

"MM/dd/yyyy HH:mm:ss","MM/dd/yyyy HH:m:ss","MM/dd/yyyy HH:mm:s","MM/dd/yyyy HH:m:s","MM/dd/yyyy H:mm:s",
"MM/dd/yyyy H:mm:ss","MM/dd/yyyy H:m:ss","MM/dd/yyyy H:m:s","MM/dd/yyyy HH:mm:ss","M/dd/yyyy HH:m:ss",
"M/dd/yyyy HH:mm:s","M/dd/yyyy HH:m:s","M/dd/yyyy H:mm:s","M/dd/yyyy H:mm:ss","M/dd/yyyy H:m:ss","M/dd/yyyy H:m:s",

"MM/d/yyyy HH:mm:ss","MM/d/yyyy HH:m:ss","MM/d/yyyy HH:mm:s","MM/d/yyyy HH:m:s","MM/d/yyyy H:mm:s","MM/d/yyyy H:mm:ss",
"MM/d/yyyy H:m:ss","MM/d/yyyy H:m:s","MM/d/yyyy HH:mm:ss","M/d/yyyy HH:m:ss","M/d/yyyy HH:mm:s","M/d/yyyy HH:m:s",
"M/d/yyyy H:mm:s","M/d/yyyy H:mm:ss","M/d/yyyy H:m:ss","M/d/yyyy H:m:s",


"MM/dd/yyyy HH:mm:ss", "M/dd/yyyy HH:mm:ss", "MM/dd/yyyy H:mm:ss", "M/dd/yyyy H:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyy-MMM-dd HH:mm:ss", "yyyy.MM.dd HH:mm:ss","yyyy/MM/dd HH:mm:ss","yyyy/MMM/dd HH:mm:ss","yyyy.MMM.dd HH:mm:ss",
"dd-MM-yyyy HH:mm:ss","dd.MM.yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "dd-MMM-yyyy HH:mm:ss", "dd.MMM.yyyy HH:mm:ss",
"MMM-dd-yyyy HH:mm:ss","MM-dd-yyyy HH:mm:ss", "MM.dd.yyyy HH:mm:ss", "MMM.dd.yyyy HH:mm:ss", "dd-MMM-yyyy HH:mm"
};
                DateTime dateValue;
                // var dt = "26.May.1975";
                bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);
                if (!IsValidDate)
                {
                    oCResult.sMessage = "Invalid string datetime: " + InputString;
                    SaveErrortoDB(oCResult);
                    dateValue = new DateTime();
                }
                //string finaldt = dateValue.ToString(XIConstant.SqlDateFormat);
                return dateValue; //Converting to Sql datetime format
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "Error In ConvertToDtTime() - Can't convert string to datetime";
                SaveErrortoDB(oCResult);
                throw ex;
            }
        }

        public bool IsValidDate(string InputString)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                string[] formats = {
"yyyy-MM-dd", "yyyy-MMM-dd", "yyyy.MM.dd","yyyy/MM/dd","yyyy/MMM/dd","yyyy.MMM.dd",
"dd-MM-yyyy","dd.MM.yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "dd.MMM.yyyy",
"MMM-dd-yyyy","MM-dd-yyyy", "MM.dd.yyyy", "MMM.dd.yyyy", "MM/dd/yyyy",

"dd/MM/yyyy hh:mm:ss",
//INFO: Save_ScheduleInstance - dtCurrentDate: 3/22/2024 2:02:57 PM dRunDate:3/22/2024 12:00:00 AM

"MM/dd/yyyy hh:mm:ss tt","MM/dd/yyyy hh:m:ss tt","MM/dd/yyyy hh:mm:s tt","MM/dd/yyyy hh:m:s tt","MM/dd/yyyy h:mm:s tt",
"MM/dd/yyyy h:mm:ss tt","MM/dd/yyyy h:m:ss tt","MM/dd/yyyy h:m:s tt","MM/dd/yyyy hh:mm:ss tt","M/dd/yyyy hh:m:ss tt",
"M/dd/yyyy hh:mm:s tt","M/dd/yyyy hh:m:s tt","M/dd/yyyy h:mm:s tt","M/dd/yyyy h:mm:ss tt","M/dd/yyyy h:m:ss tt","M/dd/yyyy h:m:s tt",

"MM/d/yyyy hh:mm:ss tt","MM/d/yyyy hh:m:ss tt","MM/d/yyyy hh:mm:s tt","MM/d/yyyy hh:m:s tt","MM/d/yyyy h:mm:s tt","MM/d/yyyy h:mm:ss tt",
"MM/d/yyyy h:m:ss tt","MM/d/yyyy h:m:s tt","MM/d/yyyy hh:mm:ss tt","M/d/yyyy hh:m:ss tt","M/d/yyyy hh:mm:s tt","M/d/yyyy hh:m:s tt",
"M/d/yyyy h:mm:s tt","M/d/yyyy h:mm:ss tt","M/d/yyyy h:m:ss tt","M/d/yyyy h:m:s tt",


"MM/dd/yyyy hh:mm:ss tt", "M/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "yyyy-MM-dd hh:mm:ss tt", "yyyy-MM-dd h:mm:ss tt", "yyyy-MMM-dd hh:mm:ss tt", "yyyy.MM.dd hh:mm:ss tt","yyyy/MM/dd hh:mm:ss tt","yyyy/MMM/dd hh:mm:ss tt","yyyy.MMM.dd hh:mm:ss tt",
"dd-MM-yyyy h:m:s tt","dd-MM-yyyy hh:mm:ss tt","dd.MM.yyyy hh:mm:ss tt", "dd/MM/yyyy hh:mm:ss tt", "dd-MMM-yyyy hh:mm:ss tt", "dd.MMM.yyyy hh:mm:ss tt",
"MMM-dd-yyyy hh:mm:ss tt","MM-dd-yyyy hh:mm:ss tt", "MM.dd.yyyy hh:mm:ss tt", "MMM.dd.yyyy hh:mm:ss tt",

"dd/MM/yyyy HH:mm:ss",

"MM/dd/yyyy HH:mm:ss","MM/dd/yyyy HH:m:ss","MM/dd/yyyy HH:mm:s","MM/dd/yyyy HH:m:s","MM/dd/yyyy H:mm:s",
"MM/dd/yyyy H:mm:ss","MM/dd/yyyy H:m:ss","MM/dd/yyyy H:m:s","MM/dd/yyyy HH:mm:ss","M/dd/yyyy HH:m:ss",
"M/dd/yyyy HH:mm:s","M/dd/yyyy HH:m:s","M/dd/yyyy H:mm:s","M/dd/yyyy H:mm:ss","M/dd/yyyy H:m:ss","M/dd/yyyy H:m:s",

"MM/d/yyyy HH:mm:ss","MM/d/yyyy HH:m:ss","MM/d/yyyy HH:mm:s","MM/d/yyyy HH:m:s","MM/d/yyyy H:mm:s","MM/d/yyyy H:mm:ss",
"MM/d/yyyy H:m:ss","MM/d/yyyy H:m:s","MM/d/yyyy HH:mm:ss","M/d/yyyy HH:m:ss","M/d/yyyy HH:mm:s","M/d/yyyy HH:m:s",
"M/d/yyyy H:mm:s","M/d/yyyy H:mm:ss","M/d/yyyy H:m:ss","M/d/yyyy H:m:s",


"MM/dd/yyyy HH:mm:ss", "M/dd/yyyy HH:mm:ss", "MM/dd/yyyy H:mm:ss", "M/dd/yyyy H:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyy-MMM-dd HH:mm:ss", "yyyy.MM.dd HH:mm:ss","yyyy/MM/dd HH:mm:ss","yyyy/MMM/dd HH:mm:ss","yyyy.MMM.dd HH:mm:ss",
"dd-MM-yyyy HH:mm:ss","dd.MM.yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "dd-MMM-yyyy HH:mm:ss", "dd.MMM.yyyy HH:mm:ss",
"MMM-dd-yyyy HH:mm:ss","MM-dd-yyyy HH:mm:ss", "MM.dd.yyyy HH:mm:ss", "MMM.dd.yyyy HH:mm:ss", "dd-MMM-yyyy HH:mm"
};
                DateTime dateValue;
                // var dt = "26.May.1975";
                bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);
                if (!IsValidDate)
                {
                    oCResult.sMessage = "Invalid string datetime: " + InputString;
                    SaveErrortoDB(oCResult);
                    dateValue = new DateTime();
                }
                //string finaldt = dateValue.ToString(XIConstant.SqlDateFormat);
                return IsValidDate; //Converting to Sql datetime format
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "Error In ConvertToDtTime() - Can't convert string to datetime";
                SaveErrortoDB(oCResult);
                throw ex;
            }
        }
        public XIIValue XIIValue(string sKey)
        {
            XIIValue oXIValue = new XIIValue();

            var oXIIValues = this.XIIValues;

            if (oXIIValues != null && oXIIValues.Count() > 0 && oXIIValues.ContainsKey(sKey))
            {
                oXIValue = oXIIValues[sKey];
            }
            return oXIValue;
        }
        public XIIBO Item(int iIndex)
        {
            var oBOIList = this.oBOIList; var oBOI = new XIIBO();
            if (oBOIList != null && oBOIList.Count() > iIndex)
            {
                oBOI = oBOIList[iIndex];
            }
            return oBOI;
        }

        public XIIBO ChildI(string sName, string Index)
        {
            if (this.SubChildI.ContainsKey(sName))
            {
                var oBOI = this.SubChildI[sName];
                if (oBOI != null && oBOI.Count() > 0)
                {
                    var iIndex = Convert.ToInt32(Index);
                    if (oBOI.Count() > iIndex)
                    {
                        return oBOI[iIndex];
                    }
                }
            }
            return new XIIBO();
        }
        public List<XIIBO> Childs(string sName)
        {
            if (this.SubChildI.ContainsKey(sName))
            {
                var oBOI = this.SubChildI[sName];
                return oBOI;
            }
            return new List<XIIBO>();
        }

        public XIIBO GetBOI(string sWhereParams)
        {
            var oBOIList = this.oBOIList; var oBOI = new XIIBO();
            if (oBOIList != null)
            {
                List<CNV> oParams = new List<CNV>();
                if (!string.IsNullOrEmpty(sWhereParams))
                {
                    var sWhereParamsArray = sWhereParams.Split('_');
                    if (sWhereParamsArray != null && sWhereParamsArray.Count() > 0)
                    {
                        foreach (var boi in oBOIList)
                        {
                            foreach (var scondition in sWhereParamsArray)
                            {
                                var sConditionArr = scondition.Split(':');
                                //CNV oCNV = new CNV();
                                //oCNV.sName = sConditionArr[0];
                                //oCNV.sValue = sConditionArr[1];
                                //oParams.Add(oCNV);
                                if (boi.Attributes.ContainsKey(sConditionArr[0]))
                                {
                                    var sValue = boi.AttributeI(sConditionArr[0]).sValue;
                                    if (!string.IsNullOrEmpty(sValue) && sValue.ToLower() == sConditionArr[1].ToLower())
                                    {
                                        oBOI = boi;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return oBOI;
        }
        public XIIBO Get_FKHover(int iInstanceID, int BOID, string ColumnName, int ViewGroup)
        {
            XIInfraCache oCache = new XIInfraCache();
            var oBO = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOID.ToString());
            //var oBO = dbContext.BOs.Find(BOID);
            var sBOName = string.Empty;
            sBOName = oBO.Name;
            XIIBO oBOInstance = Get_BOInstance(sBOName, null);
            oBOInstance.BOD = oBO;
            var sGroupResult = new List<CNV>();
            XIDBO FKBOD = new XIDBO();
            if (ColumnName != null && ColumnName.ToLower() == "id")
            {
                sGroupResult = ResolveGroupFieldsWithNameValuePairs("Summary".ToLower(), iInstanceID, oBOInstance);
            }
            else
            {
                var FKBO = oBO.Attributes.Values.Where(m => m.LabelName == ColumnName).Select(m => m.sFKBOName).FirstOrDefault();
                XIIXI oXI = new XIIXI();
                List<CNV> oParams = new List<CNV>();
                oParams.Add(new CNV { sName = "name", sValue = FKBO });
                var BOI = oXI.BOI("XIBO", null, null, oParams);
                if (BOI != null && BOI.Attributes.Count() > 0)
                {
                    var sFKBO = BOI.AttributeI("name").sValue;
                    if (!string.IsNullOrEmpty(sFKBO))
                    {
                        FKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sFKBO);
                        oBOInstance = Get_BOInstance(sFKBO, null);
                    }
                    if (ViewGroup > 0)
                    {
                        string sViewGroup = FKBOD.Groups.Where(x => x.Value.ID == ViewGroup).Select(t => t.Key).FirstOrDefault();
                        sGroupResult = ResolveGroupFieldsWithNameValuePairs(sViewGroup.ToLower(), iInstanceID, oBOInstance);
                    }
                    else
                    {
                        XIDGroup SummaryGrpD = FKBOD.GroupD("summary");
                        if (SummaryGrpD != null && SummaryGrpD.ID > 0)
                        {
                            sGroupResult = ResolveGroupFieldsWithNameValuePairs("summary".ToLower(), iInstanceID, oBOInstance);
                        }
                        else
                            sGroupResult = ResolveGroupFieldsWithNameValuePairs("Label".ToLower(), iInstanceID, oBOInstance);
                    }

                }

            }

            foreach (var items in sGroupResult)
            {
                var Def = FKBOD.Attributes.Values.Where(m => m.Name.ToLower() == items.sName.ToLower()).FirstOrDefault();
                if (Def.Format != null)
                {
                    var sFormattedValue = FormatValue(items.sName, items.sValue, Def.Format, Def.TypeID);
                    if (!string.IsNullOrEmpty(sFormattedValue))
                    {
                        items.sValue = sFormattedValue;
                    }
                }
                else if (Def.FKiFileTypeID > 0)
                {
                    var ItemFields = items.sValue.Split(',').ToList();
                    var FieldValue = "";
                    for (int i = 0; i < ItemFields.Count(); i++)
                    {
                        FieldValue = ItemFields[i];
                        if (!string.IsNullOrEmpty(FieldValue) && FieldValue.Length > 0 && Convert.ToInt32(FieldValue) > 0)
                        {
                            var DocID = Convert.ToInt32(FieldValue);
                            if (DocID > 0)
                            {

                                //var XIDoc = dbContext.XIDocs.Find(DocID);
                                //if (XIDoc != null)
                                //{
                                //    var FileFolder = dbContext.XIDocTypes.Where(m => m.ID == XIDoc.FKiDocType).Select(m => m.Type).FirstOrDefault();
                                //    var Path = ConfigurationManager.AppSettings["XIDocsPath"] + "//" + FileFolder + "//" + XIDoc.SubDirectoryPath + "//" + XIDoc.FileName;
                                //    items.sValue = Path;
                                //}
                                //else
                                //{
                                //    items.sValue = ConfigurationManager.AppSettings["NoImagePath"];
                                //}
                            }
                        }
                    }
                }
            }
            oBOInstance.NVPairs = sGroupResult;
            oBOInstance.NVPairs.ToList().ForEach(m => m.sName = oBOInstance.BOD.Attributes.Values.Where(n => n.Name.ToLower() == m.sName.ToLower()).FirstOrDefault().LabelName);
            return oBOInstance;
        }

        private List<CNV> ResolveGroupFieldsWithNameValuePairs(string sGroup, int iInstanceID, XIIBO oBOInstance)
        {
            List<CNV> NVPairs = new List<CNV>();
            var Group = oBOInstance.BOD.Groups.Values.Where(m => m.GroupName.ToLower() == sGroup).FirstOrDefault();
            if (Group == null)
            {
                Group = new XIDGroup();
                Group.BOFieldNames = "id";
            }
            if (Group != null)
            {
                string sTableName = string.Empty;
                List<string[]> Rows = new List<string[]>();
                if (string.IsNullOrEmpty(oBOInstance.BOD.TableName))
                {
                    sTableName = oBOInstance.BOD.Name;
                }
                else
                {
                    sTableName = oBOInstance.BOD.TableName;
                }
                XIDXI oXID = new XIDXI();
                var sBODataSource = oXID.GetBODataSource(oBOInstance.BOD.iDataSourceXIGUID.ToString(), oBOInstance.BOD.FKiApplicationID);
                using (SqlConnection Con = new SqlConnection(sBODataSource))
                {
                    SqlCommand cmd = new SqlCommand();
                    {
                        cmd.Connection = Con;
                        cmd.CommandText = "Select " + Group.BOFieldNames + " from " + sTableName + " Where ID=" + iInstanceID;
                        Con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        DataTable data = new DataTable();
                        data.Load(reader);
                        Rows = data.Rows.Cast<DataRow>()
          .Select(row => data.Columns.Cast<DataColumn>()
             .Select(col => Convert.ToString(row[col]))
          .ToArray())
        .ToList();
                        Con.Dispose();
                        var GroupFields = Group.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (Rows.Count() > 0)
                        {
                            for (int i = 0; i < GroupFields.Count(); i++)
                            {
                                CNV oNV = new CNV();
                                oNV.sName = GroupFields[i];
                                oNV.sValue = Rows[0][i];
                                NVPairs.Add(oNV);
                            }
                        }
                    }
                }
            }
            return NVPairs;
        }

        public string FormatValue(string sField, string sValue, string Format, int iDataType)
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
                else if (FirstLetter == 'd' || iDataType == 150)
                {
                    sFormattedValue = String.Format("{0:" + Format + "}", Convert.ToDateTime(sValue));
                }
            }
            else if (Position == 2)
            {
                var Prefix = sField.Substring(0, 2);
                if (Prefix == "dt" || iDataType == 150)
                {
                    sFormattedValue = String.Format("{0:" + Format + "}", Convert.ToDateTime(sValue));
                }
            }
            return sFormattedValue;
        }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public CResult ResloveFKFields()
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            try
            {
                XIDXI oXID = new XIDXI();
                if (Attributes != null && Attributes.Count > 0)
                {
                    Attributes.ToList().ForEach(s =>
                    {
                        var m = BOD.Attributes.ToList().Where(boAtt => boAtt.Key.ToLower() == s.Key.ToLower() && !string.IsNullOrEmpty(boAtt.Value.sFKBOName)).FirstOrDefault();
                        if (m.Value != null)
                        {
                            string sBOName = m.Value.sFKBOName; //Connection.Select<string>("XIBO_T_N", Params, "Name").FirstOrDefault();
                            if (!string.IsNullOrEmpty(sBOName))
                            {
                                XIInfraCache oCache = new XIInfraCache();
                                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);
                                var GroupD = new XIDGroup();
                                if (oBOD.Groups.TryGetValue("label", out GroupD))
                                {
                                    XIDBAPI Myconntection = new XIDBAPI(oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID));
                                    if (oBOD != null && oBOD.Groups.Any(group => group.Key.ToLower() == "label") && (oBOD.sSize == "30" || oBOD.sSize == "20") && !string.IsNullOrEmpty(s.Value.sValue) && s.Value.sValue != "0")
                                    {
                                        var BODParams = new Dictionary<string, object>();
                                        BODParams[oBOD.sPrimaryKey] = s.Value.sValue;
                                        string FinalString = oBOD.GroupD("label").ConcatanateGroupFields(" ");//concatenate the string with join String 
                                        if (!string.IsNullOrEmpty(FinalString))
                                        {
                                            var Result = Myconntection.Select<string>(oBOD.TableName, BODParams, FinalString + " As Result ").FirstOrDefault();
                                            s.Value.sResolvedValue = Result;
                                        }
                                    }

                                }
                            }
                        }
                    });
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while merging xiparameter params into cache" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                //SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult ResolveOptionList()
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            try
            {
                XIDXI oXID = new XIDXI();
                if (Attributes != null && Attributes.Count > 0)
                {
                    Attributes.ToList().ForEach(s =>
                    {
                        var m = BOD.Attributes.ToList().Where(boAtt => boAtt.Key.ToLower() == s.Key.ToLower() && boAtt.Value.IsOptionList).FirstOrDefault();
                        if (m.Value != null)
                        {
                            var sOptionValue = m.Value.OptionList.Where(k => k.sValues == s.Value.sValue).Select(k => k.sOptionName).FirstOrDefault();
                            s.Value.sValue = sOptionValue;
                        }
                    });
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while merging xiparameter params into cache" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                //SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult FormatAttrs()
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIDXI oXID = new XIDXI();
                BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID.ToString());
                if (Attributes != null && Attributes.Count > 0)
                {
                    Attributes.Values.Where(m => !string.IsNullOrEmpty(m.sValue)).ToList().ForEach(s =>
                    {
                        if (!string.IsNullOrEmpty(s.sValue))
                        {
                            XIDAttribute oAttrD = null;
                            if (BOD.Attributes.ContainsKey(s.sName.ToLower()))
                            {
                                oAttrD = BOD.Attributes[s.sName.ToLower()];//.ToList().Where(boAtt => boAtt.Key.ToLower() == s.ToLower()).Select(m => m.Value).FirstOrDefault();
                            }
                            else
                            {
                                oAttrD = BOD.Attributes.Values.Where(m => m.LabelName.ToLower() == s.sName.ToLower()).FirstOrDefault();
                            }
                            if (oAttrD != null && oAttrD.Format != null)
                            {
                                if (s.sValue == XIConstant.SqlDefaultDate)
                                {
                                    s.sValue = "";
                                }
                                else
                                {
                                    var sFomattedVal = FormatValue(oAttrD.Name, s.sValue, oAttrD.Format, oAttrD.TypeID);
                                    Attributes[s.sName.ToLower()].sValue = sFomattedVal;
                                }
                            }
                            else if (oAttrD.IsOptionList)
                            {
                                var sOptionValue = oAttrD.OptionList.Where(m => m.sValues == s.sValue).Select(m => m.sOptionName).FirstOrDefault();
                                if (!string.IsNullOrEmpty(sOptionValue))
                                {
                                    Attributes[s.sName.ToLower()].sValue = sOptionValue;
                                }
                            }
                            else if (oAttrD.FKiFileTypeID > 0)
                            {
                                //Get Image Details and assign to Attribute
                            }
                        }
                    });
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while merging xiparameter params into cache" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                //SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public CResult AddButtons(List<XID1ClickLink> oMyLinks)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIDXI oXID = new XIDXI();

                if (Attributes != null && Attributes.Count > 0)
                {
                    foreach (var Link in oMyLinks)
                    {
                        if (Link.FKiXILinkID > 0)
                        {
                            XILink oXILinkD = new XILink();
                            oXILinkD = (XILink)oCache.GetObjectFromCache(XIConstant.CacheXILink, null, Link.FKiXILinkID.ToString());
                            XIIAttribute oAttrI = new XIIAttribute();
                            oAttrI.sValue = "<input type='button' class='btn btn-theme' onclick='XIRun(null,'" + oXILinkD.XiLinkID + "')' />";
                        }
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while merging xiparameter params into cache" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                //SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public Dictionary<string, XIIAttribute> SetAttribute(string sName, string sValue)
        {
            Attributes[sName] = new XIIAttribute { sName = sName, sValue = sValue, bDirty = true };
            return Attributes;
        }
        public CResult BOICopy(string sInstanceID, string sBOName)
        {
            CResult oCResult = new CResult();
            try
            {
                XIIXI oXIIXI = new XIIXI();
                XIIBO oBOI = oXIIXI.BOI(sBOName, sInstanceID);
                int sPrimaryKeyValue = Convert.ToInt32(oBOI.Attributes.Where(x => x.Key.ToLower() == oBOI.BOD.sPrimaryKey.ToLower()).Select(x => x.Value.sValue).FirstOrDefault());
                if (sPrimaryKeyValue != 0)
                {
                    oBOI.Attributes.Where(n => n.Key.ToLower().Equals(oBOI.BOD.sPrimaryKey.ToLower())).Select(m => m.Value).ToList().ForEach(m => { m.sValue = null; });
                    if (oBOI.Attributes.ContainsKey("XIGUID"))
                    {
                        oBOI.Attributes.Where(n => n.Key.ToLower().Equals("XIGUID".ToLower())).Select(m => m.Value).ToList().ForEach(m => { m.sValue = null; });
                    }
                    oBOI.Attributes.ToList().ForEach(m => m.Value.bDirty = true);
                }
                //XIIBO oCopyBOI = oBOI.Save(oBOI);
                XIIBO oCopyBOI = new XIIBO();
                var oResponse = oBOI.Save(oBOI);
                if (oResponse.bOK && oResponse.oResult != null)
                {
                    oCopyBOI = (XIIBO)oResponse.oResult;
                }

                string iInsertedID = oCopyBOI.Attributes.Where(s => s.Key.ToLower().Equals("XIGUID".ToLower())).Select(s => s.Value).FirstOrDefault().sValue;
                oCResult.oResult = iInsertedID;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while Resolving FK Expression" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }
        public CResult GetScriptResult(List<XID1ClickScripts> oScripts)
        {
            XIInfraCache oCache = new XIInfraCache();
            CResult oCResult = new CResult();
            string sSessionID = HttpContext.Current.Session.SessionID;
            try
            {

                //this.AttributeI(\"iStatus\").sValue
                string sGUID = Guid.NewGuid().ToString();
                //XIDScript oScripts = new XIDScript();
                //oScripts.sScript = "xi.s|{if|{eq|"+this.AttributeI("iStatus").sValue + ",'Requested'},'trred','trgreen'}||{if|{lt|" + this.AttributeI("iDueInDays").sValue +",'3'},'trred','trgreen'}";
                //oScripts.sScript = "xi.s|{if|{or|"+this.AttributeI("iStatus").sValue +",'Requested','Cancelled'},'trred','trgreen'}";
                //oScripts.sScript = "xi.s|{translate|" + this.AttributeI("iStatus").sValue + ",'Cancelled','trred','Approved','trgreen','Requested','trorange'}";
                //oScripts.sScript = ; //"xi.s|{translate|" + this.AttributeI("iStatus").sValue + ",'Not Approved','trred','Approved','trgreen','OutStanding',xi.s|{if|{lt|" + this.AttributeI("iDueInDays").sValue + ",'3'},'trred',''},'Additional Information Required',xi.s|{if|{lt|" + this.AttributeI("iDueInDays").sValue + ",'3'},'trred',''}}";
                //oScripts.sType = "RowColour";
                //this.oXIDScripts.Add(oScripts);
                //oScripts = new XIDScript();
                //oScripts.sScript = "xi.s|{translate|" + this.AttributeI("iStatus").sValue + ",'OutStanding','true','Not Approved','true','Additional Information Required','true'}";
                //oScripts.sType = "UploadStatus";
                //this.oXIDScripts.Add(oScripts);
                //if (this.oXIDScripts != null && this.oXIDScripts.Count() > 0)
                //{
                string sScriptColumn = string.Empty;
                string sInstnaceID = string.Empty;
                List<CNV> oNVList = new List<CNV>();
                CNV oParam = new CNV();
                foreach (var oScript in oScripts)
                {
                    switch (oScript.sName)
                    {
                        case "RowColour":
                        case "UploadStatus":
                            string sScript = oScript.sFunction;
                            foreach (var item in this.Attributes)
                            {
                                sScript = sScript.Replace("'" + item.Key + "'", item.Value.sValue);
                                //oScript.sFunction.Replace(item.Key, item.Value.sValue);
                            }
                            CResult oResult = new CResult();
                            XIDScript oXIScript = new XIDScript();
                            oXIScript.sScript = sScript.ToString();
                            oResult = oXIScript.Execute_Script(sGUID, sSessionID);
                            string sValue = string.Empty;
                            if (oResult.bOK && oResult.oResult != null)
                            {
                                sValue = (string)oResult.oResult;
                                if (sValue.Contains("{"))
                                {
                                    CResult oCRes = new CResult();
                                    string sSubsScript = sValue;
                                    XIDScript oXIDScripts = new XIDScript();
                                    oXIDScripts.sScript = sValue.ToString();
                                    oCRes = oXIDScripts.Execute_Script(sGUID, sSessionID);
                                    if (oCRes.bOK && oCRes.oResult != null)
                                    {
                                        sValue = (string)oCRes.oResult;
                                    }
                                }
                            }
                            XIDScriptResult oScriptResult = new XIDScriptResult();
                            oScriptResult.sType = oScript.sName;
                            oScriptResult.sScriptResult = sValue;
                            this.ScriptResults.Add(oScriptResult);
                            break;
                            //case "UploadStatus":
                            //    sScript = oScript.sScript;
                            //    CResult oResponse = new CResult();
                            //    XIDScript oXIDScript = new XIDScript();
                            //    oXIDScript.sScript = sScript.ToString();
                            //    oResponse = oXIDScript.Execute_Script(sGUID);
                            //    string sResponseValue = string.Empty;
                            //    if (oResponse.bOK && oResponse.oResult != null)
                            //    {
                            //        sResponseValue = (string)oResponse.oResult;
                            //    }
                            //    XIDScriptResult oXIDScriptResult = new XIDScriptResult();
                            //    oXIDScriptResult.sType = oScript.sType;
                            //    oXIDScriptResult.sScriptResult = sResponseValue;
                            //    this.ScriptResults.Add(oXIDScriptResult);
                            //    break;
                    }
                }
                //}
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while Resolving BOI ScriptResults" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }
        public XIIBO MergeAuditBOI(XIIBO oAuditBOI)
        {
            oAuditBOI.Attributes.ToList().ForEach(m => m.Value.sValue = m.Value.sPreviousValue);
            return oAuditBOI;
        }
        public List<CNV> IsHavingChanges(XIIBO oCurrentBOI, XIIBO oPreviousBOI)
        {
            bool bISHavingChange = false; string sChangedData = string.Empty; string sPreviousData = string.Empty;
            List<CNV> oParams = new List<CNV>();
            CNV oNVs = new CNV();
            try
            {
                foreach (var oCurrentAttr in oCurrentBOI.Attributes)
                {
                    if (oPreviousBOI.Attributes.ContainsKey(oCurrentAttr.Value.sName))
                    {
                        if (string.IsNullOrEmpty(oCurrentAttr.Value.sValue) && string.IsNullOrEmpty(oPreviousBOI.Attributes[oCurrentAttr.Value.sName].sValue))
                        {
                            bISHavingChange = false;
                        }
                        else if (oCurrentAttr.Value.sValue != oPreviousBOI.Attributes[oCurrentAttr.Value.sName].sValue)
                        {
                            bISHavingChange = true;
                            //if(!string.IsNullOrEmpty(sData))
                            //{
                            //    sData += oCurrentAttr.Value.sName + ":" + oCurrentAttr.Value.sValue +"<br/>";
                            //}
                            //else
                            //{
                            sChangedData += oCurrentAttr.Value.sName + ":" + oCurrentAttr.Value.sValue + "<br/>";
                            sPreviousData += oCurrentAttr.Value.sName + ":" + oPreviousBOI.Attributes[oCurrentAttr.Value.sName].sValue + "<br/>";
                            //}

                        }
                    }
                }
                oParams.Add(new CNV { sName = "sChangedData", sValue = sChangedData });
                oParams.Add(new CNV { sName = "sPreviousData", sValue = sPreviousData });
                oParams.Add(new CNV { sName = "bIsHavingChanges", sValue = bISHavingChange.ToString() });
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while Executing IsHavingChanges" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oParams;
        }
        public string GetConcatStringFromList(string sValues, int iCount = 0, bool bIsABICode = false, bool bIsDisplayLable = false, string sReplaceContent = "")
        {
            string sResult = string.Empty;
            List<string> sReturnList = new List<string>();
            try
            {
                XIIValue oXIValue = new XIIValue();
                var oXIIValues = this.XIIValues;
                if (!string.IsNullOrEmpty(sValues))
                {
                    var sInputFields = sValues.Split(',');
                    List<string> sInputFieldsList = sInputFields.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    foreach (var sField in sInputFieldsList)
                    {
                        string sDisplayName = sField;
                        if (oXIIValues != null && oXIIValues.Count() > 0 && oXIIValues.ContainsKey(sField))
                        {
                            if (oXIIValues.ContainsKey(sField) && !string.IsNullOrEmpty(oXIIValues[sField].sValue) && (oXIIValues[sField].sValue.ToLower() == "on" || oXIIValues[sField].sValue.ToLower() == "true"))
                            {
                                if (bIsABICode)
                                {
                                    XIIXI oIXI = new XIIXI();
                                    List<CNV> oWhereparams = new List<CNV>();
                                    oWhereparams.Add(new CNV { sName = "FKiFieldID", sValue = oXIIValues[sField].FKiFieldOriginID.ToString() });
                                    var oEDIBOI = oIXI.BOI("EDICodes_T", null, null, oWhereparams);
                                    var sEDICode = string.Empty;
                                    if (oEDIBOI != null && oEDIBOI.Attributes != null && oEDIBOI.Attributes.Count() > 0)
                                    {
                                        if (oEDIBOI.Attributes.ContainsKey("sCode"))
                                        {
                                            sEDICode = oEDIBOI.AttributeI("sCode").sValue;
                                        }
                                    }
                                    sResult += sEDICode + ", ";
                                }
                                else
                                {
                                    if (bIsDisplayLable)
                                    {
                                        sDisplayName = oXIIValues[sField].sDisplayName;
                                    }
                                    sResult += sDisplayName + ", ";
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(sResult))
                    {
                        sResult = sResult.Remove(sResult.LastIndexOf(","));
                    }
                }

                if (iCount != 0)
                {
                    if (!string.IsNullOrEmpty(sResult))
                    {
                        sReturnList = sResult.Split(',').ToList();
                        if (sReturnList.Count() > iCount)
                        {
                            var sRequiredList = sReturnList.Take(3);
                            sResult = string.Join(",", sRequiredList);
                        }
                        else if (sReturnList.Count() == iCount)
                        {
                            sResult = string.Join(",", sReturnList);
                        }
                        else
                        {
                            //int iReturStringCount = sReturnList.Count();
                            int iRequiredCount = iCount - sReturnList.Count();
                            for (int len = 1; len <= iRequiredCount; len++)
                            {
                                sReturnList.Add("");
                            }
                            sResult = string.Join(",", sReturnList);
                        }
                    }
                    else
                    {
                        for (int len = 1; len <= iCount; len++)
                        {
                            sReturnList.Add("");
                        }
                        sResult = string.Join(",", sReturnList);
                    }
                }
                if (string.IsNullOrEmpty(sResult) && !string.IsNullOrEmpty(sReplaceContent))
                {
                    sResult = sReplaceContent;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while Resolving Comma String from List" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return sResult;
        }

        public CResult Validate_Unique(List<CNV> Params)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            try
            {
                bool bIsExist = false;
                int iBODID = 0;
                var sBOID = Params.Where(m => m.sName.ToLower() == XIConstant.Param_BODID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sBOID, out iBODID);
                var sAttrName = Params.Where(m => m.sName.ToLower() == XIConstant.Param_AttrName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sAttrValue = Params.Where(m => m.sName.ToLower() == XIConstant.Param_AttrValue.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var iBOIID = Params.Where(m => m.sName.ToLower() == XIConstant.Param_BOIID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sPreviousValue = Params.Where(m => m.sName.ToLower() == XIConstant.Param_PreviousValue.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sIDRef = Params.Where(m => m.sName.ToLower() == "sidref").Select(m => m.sValue).FirstOrDefault();
                if (iBODID > 0)
                {
                    if (string.IsNullOrEmpty(iBOIID) || iBOIID == "0")
                    {
                        bIsExist = false;
                    }
                    else
                    {
                        XIInfraCache oCache = new XIInfraCache();
                        var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID.ToString());
                        List<CNV> whrParams = new List<CNV>();
                        whrParams.Add(new CNV { sName = sAttrName, sValue = sAttrValue });
                        XIIXI oXI = new XIIXI();
                        var oBOI = oXI.BOI(oBOD.Name, null, null, whrParams);
                        if (oBOI != null && oBOI.Attributes.Values.Count() > 0)
                        {
                            long iID = 0;
                            var sID = oBOI.Attributes[sIDRef].sValue;
                            long.TryParse(sID, out iID);
                            if (iID > 0)
                            {
                                if (sID == iBOIID)
                                {
                                    bIsExist = false;
                                }
                                else
                                {
                                    bIsExist = true;
                                }
                            }
                        }
                    }
                }
                oCResult.oResult = bIsExist;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while checking unique value" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                //SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult Validate_Compare(List<CNV> Params)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            try
            {
                bool bIsEqual = false;
                var sParentAttrValue = Params.Where(m => m.sName.ToLower() == XIConstant.Param_ParentAttrVal.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sChildAttrValue = Params.Where(m => m.sName.ToLower() == XIConstant.Param_ChildAttrVal.ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sParentAttrValue) && !string.IsNullOrEmpty(sChildAttrValue))
                {
                    if (sParentAttrValue == sChildAttrValue)
                    {
                        bIsEqual = true;
                    }
                }
                oCResult.oResult = bIsEqual;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while checking unique value" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                //SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult Get_BODialogLabel(string iBODID, string UID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            try
            {
                oTrace.sClass = "XIIBO";
                oTrace.sMethod = "Get_BODialogLabel";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                Guid BOIDXIGUID = Guid.Empty;
                string sGroupLabel = string.Empty;
                XIInfraCache oCache = new XIInfraCache();
                var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID.ToString());
                var sGroupFields = string.Empty;
                XIDGroup oGroupD = new XIDGroup();
                oGroupD = BOD.GroupD("popup");
                if (oGroupD.ID == 0 && (oGroupD.XIGUID == null || oGroupD.XIGUID == Guid.Empty))
                {
                    oGroupD = BOD.GroupD("label");
                }
                if (oGroupD.ID > 0 || (oGroupD.XIGUID != null && oGroupD.XIGUID != Guid.Empty))
                {
                    sGroupFields = oGroupD.ConcatanateGroupFields(" ");
                }
                else
                {
                    sGroupFields = BOD.sPrimaryKey;
                }
                if (!string.IsNullOrEmpty(sGroupFields))
                {

                    XIDXI oXID = new XIDXI();
                    string sBODataSource = oXID.GetBODataSource(BOD.iDataSourceXIGUID.ToString(), BOD.FKiApplicationID);
                    var Con = new XIDBAPI(sBODataSource);
                    Dictionary<string, object> UserParams = new Dictionary<string, object>();
                    string sIDRef = "";
                    XIInfraUsers oUser = new XIInfraUsers();
                    CUserInfo oInfo = new CUserInfo();
                    oInfo = oUser.Get_UserInfo();
                    if (oInfo != null && oInfo.iUserID > 0)
                    {
                        iOrgId = oInfo.iOrganizationID;
                        var iAppID = oInfo.iApplicationID;
                        iRoleID = oInfo.iRoleID;
                        if (oInfo.iRoleID != 55)
                        {
                            sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, iAppID + "_" + iOrgId + "_" + iRoleID + "_" + XIConstant.IDRef_internal);
                        }
                        else
                        {
                            sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, iAppID + "_" + iOrgId + "_" + XIConstant.IDRef_public);
                        }
                    }

                    Guid XIGUID = Guid.Empty;
                    Guid.TryParse(UID, out XIGUID);
                    if (BOD.bUID || (!string.IsNullOrEmpty(sIDRef) && sIDRef.ToLower() == "xiguid") || (XIGUID != null && XIGUID != Guid.Empty))
                    {
                        UserParams["XIGUID"] = UID;
                    }
                    else
                    {
                        UserParams[BOD.sPrimaryKey] = UID;
                    }
                    sGroupLabel = Con.SelectString(sGroupFields, BOD.TableName, UserParams).ToString();
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sGroupLabel;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oTrace.sMessage = ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Get_GroupLabel(string iBODID, string UID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            try
            {
                oTrace.sClass = "XIIBO";
                oTrace.sMethod = "Get_GroupLabel";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                Guid BOIDXIGUID = Guid.Empty;
                string sGroupLabel = string.Empty;
                XIInfraCache oCache = new XIInfraCache();
                var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID.ToString());
                var Scripts = BOD.Scripts.Values.ToList().Where(m => m.sType.ToLower() == "taskbar").FirstOrDefault();
                if (Scripts != null)
                {
                    string sFunction = Scripts.sScript;
                    //string sFunction = "xi.s|{xi.function|'Format','{xi.p|-iParam}','dd/mm/yyyy'}";
                    List<CNV> oNVList = new List<CNV>();
                    string sNewGUID = Guid.NewGuid().ToString();
                    string sSessionID = Guid.NewGuid().ToString();
                    CNV oParam = new CNV();
                    oParam.sName = "sFunction";
                    oParam.sValue = sFunction;
                    oNVList.Add(oParam);
                    oParam = new CNV();
                    oParam.sName = "-iBOIID";
                    oParam.sValue = UID;
                    oNVList.Add(oParam);
                    oCache.SetXIParams(oNVList, sNewGUID, sSessionID);
                    XIDScript oXIScript = new XIDScript();
                    oXIScript.sScript = sFunction.ToString();
                    oCR = oXIScript.Execute_Script(sNewGUID, sSessionID);
                    string sValue = string.Empty;
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        sGroupLabel = (string)oCR.oResult;
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sGroupLabel;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oTrace.sMessage = ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Assign_FKGUIDAttribute(string sValue, XIDAttribute oAttrD)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["Name"] = oAttrD.sFKBOName;
                string sSelectFields = string.Empty;
                sSelectFields = "Name,BOID,iDataSource,sSize,TableName,sPrimaryKey,sType";
                //var FKBOD = Connection.Select<XIDBO>("XIBO_T_N", Params, sSelectFields).FirstOrDefault();
                XIInfraCache oCache = new XIInfraCache();
                var FKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oAttrD.sFKBOName);

                XIIXI oXI = new XIIXI();
                var oBOI = oXI.BOI(FKBOD.Name, sValue, "", null, false, true);
                if (oBOI != null && oBOI.Attributes.Count() > 0)
                {
                    var GUID = oBOI.AttributeI("xiguid").sValue;
                    oCResult.oResult = GUID;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString(); oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public CResult Assign_FKIDAttribute(string sValue, XIDAttribute oAttrD)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["Name"] = oAttrD.sFKBOName;
                string sSelectFields = string.Empty;
                sSelectFields = "Name,BOID,iDataSource,sSize,TableName,sPrimaryKey,sType";
                //var FKBOD = Connection.Select<XIDBO>("XIBO_T_N", Params, sSelectFields).FirstOrDefault();
                XIInfraCache oCache = new XIInfraCache();
                var FKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oAttrD.sFKBOName);

                XIIXI oXI = new XIIXI();
                var oBOI = oXI.BOI(FKBOD.Name, sValue, "", null, false);
                if (oBOI != null && oBOI.Attributes.Count() > 0)
                {
                    var ID = oBOI.AttributeI(FKBOD.sPrimaryKey).sValue;
                    oCResult.oResult = ID;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString(); oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Resolve_Notation(string sNotation)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "Notation", sValue = sNotation });
                if (!string.IsNullOrEmpty(sNotation))//check mandatory params are passed or not
                {
                    var Splits = sNotation.Split('.').ToList();
                    if (Splits.Count() == 4)
                    {
                        if (Splits[0].ToLower() == "me" && Splits[1].ToLower() == "attributes")
                        {
                            if (Splits[3].ToLower() == "oldvalue")
                            {
                                oCResult.oResult = AttributeI(Splits[2]).sPreviousValue;
                            }
                            else if (Splits[3].ToLower() == "newvalue" || Splits[3].ToLower() == "svalue")
                            {
                                oCResult.oResult = AttributeI(Splits[2]).sValue;
                            }
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sNotation is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString(); oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Check_Whitelist(int iBODID, int iRoleID, int iOrgID, int iAppID, string sOperation, int iBOLevel, int iUserLevel)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "check white list access for the BO operation against the role";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "iBODID", sValue = iBODID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "iOrgID", sValue = iOrgID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "iAppID", sValue = iAppID.ToString() });
                if (iBODID > 0 && iOrgID > 0 && iAppID > 0 && !string.IsNullOrEmpty(sOperation))//check mandatory params are passed or not
                {
                    string sKey = iBODID + "_" + iRoleID + "_" + iOrgID + "_" + iAppID;
                    XIIBO oWhiteI = new XIIBO();
                    XIIXI oXI = new XIIXI();
                    bool bUNAuth = false;
                    XIInfraCache oCache = new XIInfraCache();
                    var oWhiteList = (Dictionary<string, object>)oCache.GetObjectFromCache(XIConstant.CacheWhiteList, "CacheWhiteList");
                    if (oWhiteList.ContainsKey(sKey))
                    {
                        oWhiteI = (XIIBO)oWhiteList[sKey];
                    }
                    else
                    {
                        List<CNV> WhrPrms = new List<CNV>();
                        WhrPrms.Add(new CNV { sName = "FKiBODID", sValue = iBODID.ToString() });
                        WhrPrms.Add(new CNV { sName = "FKiRoleID", sValue = iRoleID.ToString() });
                        WhrPrms.Add(new CNV { sName = "FKiOrgID", sValue = iOrgID.ToString() });
                        WhrPrms.Add(new CNV { sName = "FKiAppID", sValue = iAppID.ToString() });
                        oWhiteI = oXI.BOI("BO WhiteList", null, null, WhrPrms);
                        if (oWhiteI != null && oWhiteI.Attributes.Count() > 0)
                        {
                            oWhiteList[sKey] = oWhiteI;
                            oCache.InsertIntoCache(oWhiteList, "CacheWhiteList");
                        }
                        else
                        {
                            //Check system config setting for auto white list adding
                            var sValue = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, iAppID + "_" + iOrgID + "_AutoAddWhiteList");
                            if (!string.IsNullOrEmpty(sValue) && sValue.ToLower() == "yes")
                            {
                                //Add White list to DB
                                oCR = Add_Whitelist(iBODID, iRoleID, iOrgID, iAppID, iBOLevel, iUserLevel);
                                //Add to Cache
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oWhiteI = (XIIBO)oCR.oResult;
                                    if (oWhiteI != null && oWhiteI.Attributes.Count() > 0)
                                    {
                                        oWhiteList[sKey] = oWhiteI;
                                        oCache.InsertIntoCache(oWhiteList, "CacheWhiteList");
                                    }
                                }
                            }
                        }
                    }
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID.ToString());
                    if (oWhiteI != null && oWhiteI.Attributes.Count() > 0)
                    {
                        var UNAuth = oWhiteI.AttributeI("bUNAuthorize").sValue;
                        bool.TryParse(UNAuth, out bUNAuth);
                        if (bUNAuth)
                        {
                            oCResult.oResult = true;
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.sCategory = "Unauthorize access";
                            oCResult.sMessage = "Unauthorize access from BO " + oBOD.LabelName + " and key:" + sKey;
                            SaveErrortoDB(oCResult);
                        }
                        else
                        {
                            string sAccess = string.Empty;
                            if (sOperation == "create")
                            {
                                sAccess = oWhiteI.AttributeI("bCreate").sValue;
                            }
                            else if (sOperation == "read")
                            {
                                sAccess = oWhiteI.AttributeI("bRead").sValue;
                            }
                            else if (sOperation == "update")
                            {
                                sAccess = oWhiteI.AttributeI("bUpdate").sValue;
                            }
                            else if (sOperation == "delete")
                            {
                                sAccess = oWhiteI.AttributeI("bDelete").sValue;
                            }
                            else if (sOperation == "action")
                            {
                                sAccess = oWhiteI.AttributeI("bAction").sValue;
                            }
                            else if (sOperation == "query")
                            {
                                sAccess = oWhiteI.AttributeI("b1Query").sValue;
                            }
                            if (sAccess == "1")
                            {
                                bUNAuth = true;
                            }
                            else if (sAccess == "0")
                            {
                                bUNAuth = false;
                            }
                            else
                            {
                                bool.TryParse(sAccess, out bUNAuth);
                            }
                            if (bUNAuth)
                            {
                                oCResult.oResult = false;
                            }
                            else
                            {
                                oCResult.oResult = true;
                            }
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - white list not found for key " + sKey;
                        oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                        SaveErrortoDB(oCResult);
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: iBODID or iOrgID or iAppID or sOperation is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString(); oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Add_Whitelist(int iBODID, int iRoleID, int iOrgID, int iAppID, int iBOLevel, int iUserLevel)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "IF BO is missing in white list then add to white list against role";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "iBODID", sValue = iBODID.ToString() });
                if (iBODID > 0)//check mandatory params are passed or not
                {
                    XIIBO oBOI = new XIIBO();
                    XIInfraCache oCache = new XIInfraCache();
                    var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "BO Whitelist");
                    oBOI = new XIIBO();
                    oBOI.BOD = BOD;
                    oBOI.SetAttribute("FKiBODID", iBODID.ToString());
                    oBOI.SetAttribute("FKiRoleID", iRoleID.ToString());
                    oBOI.SetAttribute("FKiOrgID", iOrgID.ToString());
                    oBOI.SetAttribute("FKiAppID", iAppID.ToString());
                    oBOI.SetAttribute("bUNAuthorize", "0");
                    string sAccess = string.Empty;
                    //Application level BO and Application Configurar
                    if (iBOLevel == 0 && iUserLevel == 0)
                    {
                        sAccess = "1";
                    }
                    //organisation level BO and Application Configurar
                    if (iBOLevel == 10 && iUserLevel == 0)
                    {
                        sAccess = "1";
                    }
                    //Application level BO and organisation Configurar
                    else if (iBOLevel == 0 && (iUserLevel == 10 || iUserLevel == 20))
                    {
                        sAccess = "0";
                    }
                    //organisation level BO and organisation Configurar
                    else if (iBOLevel == 10 && (iUserLevel == 10 || iUserLevel == 20))
                    {
                        sAccess = "1";
                    }
                    oBOI.SetAttribute("bCreate", sAccess);
                    oBOI.SetAttribute("bRead", sAccess);
                    oBOI.SetAttribute("bUpdate", sAccess);
                    oBOI.SetAttribute("bDelete", sAccess);
                    oBOI.SetAttribute("bAction", sAccess);
                    oBOI.SetAttribute("b1Query", sAccess);
                    oCR = oBOI.Save(oBOI);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oBOI = (XIIBO)oCR.oResult;
                        oCResult.oResult = oBOI;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: iBODID is missing";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString(); oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Save_BOI(List<CNV> oNVs, string sBOUID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Save object instance with NV pairs which is generic to all objects";//expalin about this method logic
            try
            {
                int BODID = 0;
                int.TryParse(sBOUID, out BODID);
                Guid BOGUID = Guid.Empty;
                Guid.TryParse(sBOUID, out BOGUID);
                //oTrace.oParams.Add(new CNV { sName = "XIGUID", sValue = XIGUID.ToString() });
                if (oNVs != null && oNVs.Count() > 0)//check mandatory params are passed or not
                {
                    XIInfraCache oCache = new XIInfraCache();
                    XIIXI oXI = new XIIXI();
                    XIIBO oBOI = new XIIBO();
                    var oBOD = new XIDBO();
                    if (BOGUID != null && BOGUID != Guid.Empty)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOGUID.ToString());
                    }
                    else if (BODID > 0)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BODID.ToString());
                    }
                    else
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOUID);
                    }
                    oBOI.BOD = oBOD;
                    foreach (var NV in oNVs)
                    {
                        oBOI.SetAttribute(NV.sName, NV.sValue);
                    }
                    oCR = oBOI.Save(oBOI);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oBOI = (XIIBO)oCR.oResult;
                        var sInstanceID = oBOI.AttributeI("id").sValue;
                        oCResult.oResult = sInstanceID;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: iStatus is missing";
                }
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Save_XIMeta(XIIBO oBOI, string sSearchType)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Save ximeta for search";//expalin about this method logic
            try
            {
                if (oBOI != null && oBOI.Attributes.Count() > 0 && !string.IsNullOrEmpty(sSearchType))//check mandatory params are passed or not
                {
                    XIIXI oXI = new XIIXI();
                    var iBOIID = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue;
                    var XIGUID = oBOI.AttributeI("xiguid").sValue;
                    if (string.IsNullOrEmpty(XIGUID))
                    {
                        oBOI = oXI.BOI(oBOI.BOD.Name, iBOIID);
                        XIGUID = oBOI.AttributeI("xiguid").sValue;
                    }
                    var sName = oBOI.AttributeI("sname").sValue;
                    List<string> srchValues = new List<string>();
                    var sSrchString = string.Empty;
                    var oGroupD = oBOI.BOD.GroupD("meta");
                    if (oGroupD != null && !string.IsNullOrEmpty(oGroupD.BOFieldIDs)) { }
                    else
                    {
                        oGroupD = oBOI.BOD.GroupD("summary");
                    }
                    if (oGroupD != null && !string.IsNullOrEmpty(oGroupD.BOFieldIDs))
                    {
                        var Attrs = oGroupD.BOFieldIDs.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var attr in Attrs)
                        {
                            var oAttrD = oBOI.BOD.Attributes.Values.ToList().Where(m => m.XIGUID.ToString().ToLower() == attr.ToLower()).FirstOrDefault();
                            if (oAttrD != null)
                            {
                                var sValue = oBOI.AttributeI(oAttrD.Name).sResolvedValue;
                                if (!string.IsNullOrEmpty(sValue))
                                {
                                    srchValues.Add(oAttrD.LabelName + "=" + sValue);
                                }
                            }
                        }
                        if (srchValues.Count() > 0)
                        {
                            sSrchString = string.Join("|", srchValues);
                        }
                    }
                    var oMetaDGroupD = oBOI.BOD.GroupD("meta display");
                    string sSummaryHTML = string.Empty;
                    srchValues = new List<string>();
                    if (oMetaDGroupD != null && !string.IsNullOrEmpty(oMetaDGroupD.BOFieldIDs))
                    {
                        var DispAttrs = oMetaDGroupD.BOFieldIDs.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var attr in DispAttrs)
                        {
                            var oAttrD = oBOI.BOD.Attributes.Values.ToList().Where(m => m.XIGUID.ToString().ToLower() == attr.ToLower()).FirstOrDefault();
                            if (oAttrD != null)
                            {
                                var sValue = oBOI.AttributeI(oAttrD.Name).sResolvedValue;
                                if (!string.IsNullOrEmpty(sValue))
                                {
                                    srchValues.Add(oAttrD.LabelName + "=" + sValue);
                                }
                            }
                        }
                        if (srchValues.Count() > 0)
                        {
                            var sDispSrchString = string.Join("|", srchValues);
                            sSummaryHTML = sDispSrchString;
                        }
                    }
                    else
                    {
                        sSummaryHTML = sSrchString;
                    }

                    if (!string.IsNullOrEmpty(sSummaryHTML) || !string.IsNullOrEmpty(sSrchString))
                    {
                        XIInfraCache oCache = new XIInfraCache();
                        XIIBO oMetaI = new XIIBO();
                        List<CNV> oWhrParams = new List<CNV>();
                        oWhrParams.Add(new CNV() { sName = "FKiBODIDXIGUID", sValue = oBOI.BOD.XIGUID.ToString() });
                        oWhrParams.Add(new CNV() { sName = "FKiBOIID", sValue = iBOIID });
                        oWhrParams.Add(new CNV() { sName = "iStatus", sValue = "20" });
                        oMetaI = oXI.BOI("ximeta", null, null, oWhrParams);
                        int iVersion = 0;
                        if (oMetaI != null && oMetaI.Attributes.Values.Count() > 0)
                        {
                            iVersion = oMetaI.AttributeI("iVersion").iValue;
                            oMetaI.SetAttribute("iStatus", "10");
                            oMetaI.Save(oMetaI);
                        }
                        iVersion++;
                        oMetaI = new XIIBO();
                        var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "ximeta");
                        oMetaI.BOD = oBOD;
                        oMetaI.SetAttribute("sName", sName);
                        oMetaI.SetAttribute("FKiBODID", oBOI.BOD.BOID.ToString());
                        oMetaI.SetAttribute("FKiBODIDXIGUID", oBOI.BOD.XIGUID.ToString());
                        oMetaI.SetAttribute("FKiBOIID", iBOIID);
                        oMetaI.SetAttribute("objectGUID", XIGUID);
                        oMetaI.SetAttribute("objectID", iBOIID);
                        oMetaI.SetAttribute("sSummary", sSrchString);
                        oMetaI.SetAttribute("sSummaryHTML", sSummaryHTML);
                        oMetaI.SetAttribute("iType", sSearchType);
                        oMetaI.SetAttribute("iStatus", "20");
                        oMetaI.SetAttribute("iVersion", iVersion.ToString());
                        oCR = oMetaI.Save(oMetaI);


                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: oBOI or sSearchType are missing";
                }
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Calculate_ScheduleDateTime_V1(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Calculate schedule instances run date time based on throttle settings";//expalin about this method logic
            try
            {
                var sBO = oParams.Where(m => m.sName.ToLower() == "bo").Select(m => m.sValue).First();
                var iBOIID = oParams.Where(m => m.sName.ToLower() == "iInstanceID".ToLower()).Select(m => m.sValue).First();
                oTrace.oParams.Add(new CNV { sName = "sBO", sValue = sBO });
                oTrace.oParams.Add(new CNV { sName = "iBOIID", sValue = iBOIID });
                if (!string.IsNullOrEmpty(sBO) && !string.IsNullOrEmpty(iBOIID))//check mandatory params are passed or not
                {
                    XIIXI oXI = new XIIXI();
                    XIInfraCache oCache = new XIInfraCache();
                    var oBOI = oXI.BOI(sBO, iBOIID);
                    if (oBOI != null && oBOI.Attributes.Count() > 0)
                    {
                        var iThrottleType = oBOI.AttributeI("iThrottleType").iValue;
                        var iCount = oBOI.AttributeI("iCount").iValue;
                        var iThrottle = oBOI.AttributeI("iThrottle").iValue;
                        var iTimeInterval = oBOI.AttributeI("iTimeInterval").iValue;
                        var FKiLeadImportID = oBOI.AttributeI("FKiLeadImportID").iValue;
                        var FKiPilotID = oBOI.AttributeI("FKiPilotID").iValue;
                        var sEventDt = oBOI.AttributeI("dtEvent").sValue;
                        var FKiSubscriptionID = oBOI.AttributeI("FKiSubscriptionID").iValue;
                        DateTime dtEvent = DateTime.MinValue;
                        DateTime dtEventDate = DateTime.MinValue;
                        bool bValidDate = oBOI.IsValidDate(sEventDt);
                        if (bValidDate)
                        {
                            dtEvent = oBOI.ConvertToDtTime(sEventDt);
                        }
                        if (dtEvent != null && dtEvent != DateTime.MinValue && dtEvent.Year.ToString() != "1900") { dtEventDate = dtEvent; }
                        else
                        {
                            dtEventDate = DateTime.Now;
                        }
                        //Load Pilot
                        Guid QSDXIGUID = Guid.Empty;
                        var oPilotI = oXI.BOI("BSPPilot", FKiPilotID.ToString());
                        if (oPilotI != null && oPilotI.Attributes.Count() > 0)
                        {
                            var QSDGUID = oPilotI.AttributeI("FKiQSDIDXIGUID").sValue;
                            Guid.TryParse(QSDGUID, out QSDXIGUID);
                        }
                        XID1Click o1ClickD = new XID1Click();
                        o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "CampaignTargetsonSubscription");
                        XID1Click o1ClickC = new XID1Click();
                        o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                        List<CNV> nParms = new List<CNV>();
                        nParms.Add(new CNV { sName = "{XIP|FKiImportID}", sValue = FKiLeadImportID.ToString() });
                        if (FKiSubscriptionID > 0)
                            o1ClickC.Query = o1ClickC.AddSearchParameters(o1ClickC.Query, "FKiSubscriptionID=" + FKiSubscriptionID);
                        o1ClickC.ReplaceFKExpressions(nParms);
                        //o1ClickC.Query = o1ClickC.AddSearchParameters(o1ClickC.Query, "FKiImportID=" + FKiLeadImportID);
                        var Results = o1ClickC.OneClick_Run();
                        //oCResult.sMessage = "INFO: Save_ScheduleInstance - dtCurrentDate: " + dtCurrentDate + " dRunDate:" + dRunDate;
                        //oCResult.sCategory = "Whatsapp API";
                        //SaveErrortoDB(oCResult);
                        int i = 0;
                        foreach (var ins in Results.Values.ToList())
                        {
                            var insID = ins.AttributeI("ID").sValue;
                            var sMobileNo = ins.AttributeI("sMob").sValue;
                            if (!string.IsNullOrEmpty(sMobileNo))
                            {
                                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIScheduleInstance");
                                oBOI = new XIIBO();
                                oBOI.BOD = oBOD;
                                oBOI.SetAttribute("sMobileNo", sMobileNo);
                                oBOI.SetAttribute("FKiCampaignID", iBOIID);
                                oBOI.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                if (iThrottleType == 10)
                                {
                                    if (iTimeInterval == 10)
                                    {
                                        //Minutes: Ex: Per Minute 100 :- 60/100 = 0.6 -> For every 0.6 seconds 1 schedule instance will be created
                                        double Interval = ((double)60 / iCount) * i;
                                        dtEvent = dtEventDate.AddSeconds(Interval);
                                    }
                                    else if (iTimeInterval == 20)
                                    {
                                        //Hours: Ex: Per hour 100 :- 60/100 = 0.6 -> For every 0.6 minutes 1 schedule instance will be created
                                        double Interval = ((double)60 / iCount) * i;
                                        dtEvent = dtEventDate.AddMinutes(Interval);
                                    }
                                    else if (iTimeInterval == 30)
                                    {
                                        //Days: Ex: Per day 100 :- (60*24)/100 = 1440/100=14.4 -> For every 14.4 minutes 1 schedule instance will be created
                                        double Interval = ((double)(60 * 24) / iCount) * i;
                                        dtEvent = dtEventDate.AddMinutes(Interval);
                                    }
                                    else if (iTimeInterval == 40)
                                    {
                                        //Months: Ex: Per month 100 :- (60*24*30)/100 = 43200/100=432 -> For every 432 minutes 1 schedule instance will be created
                                        int year = DateTime.Now.Year;
                                        int month = DateTime.Now.Month;
                                        int iMonthDays = DateTime.DaysInMonth(year, month);
                                        double Interval = ((double)(60 * 24 * iMonthDays) / iCount) * i;
                                        dtEvent = dtEventDate.AddMinutes(Interval);
                                    }
                                    else if (iTimeInterval == 50)
                                    {
                                        //Years
                                    }
                                }
                                else if (iThrottleType == 20)
                                {
                                    var insCount = Results.Count;
                                    if (iTimeInterval == 10)
                                    {
                                        //Minutes: Ex: 100 instances over next 30 minutes :- 30*60 = 1800 seconds => 100/1800 = 0.055 -> For every 0.055 seconds 1 schedule instance will be created
                                        //Minutes: Ex: 100 instances over next 60 minutes :- 60*60 = 3600 seconds => 100/3600 = 0.027 -> For every 0.027 seconds 1 schedule instance will be created
                                        double Interval = ((double)insCount / (iThrottle * 60)) * i;
                                        double Interval1 = ((double)(iThrottle * 60) / insCount) * i;
                                        dtEvent = dtEventDate.AddSeconds(Interval1);
                                    }
                                    else if (iTimeInterval == 20)
                                    {
                                        //Hours: Ex: 100 instances over next 2 hours :- 2*60 = 120 minutes => 120/100 = 1.2 -> For every 1.2 minutes 1 schedule instance will be created
                                        double Interval = ((double)(iThrottle * 60) / insCount) * i;
                                        dtEvent = dtEventDate.AddMinutes(Interval);
                                    }
                                    else if (iTimeInterval == 30)
                                    {
                                        //Days: Ex: 100 instances over next 2 day :- (2 * 60 * 24)/100 = 2880/100=28.8 -> For every 28.8 minutes 1 schedule instance will be created
                                        double Interval = ((double)(iThrottle * 60 * 24) / insCount) * i;
                                        dtEvent = dtEventDate.AddMinutes(Interval);
                                    }
                                    else if (iTimeInterval == 40)
                                    {
                                        //Months: Ex: 100 instances over next 2 months :- (2*60*24*30)/100 = 86400/100=864 -> For every 864 minutes 1 schedule instance will be created
                                        int year = DateTime.Now.Year;
                                        int month = DateTime.Now.Month;
                                        int iMonthDays = DateTime.DaysInMonth(year, month);
                                        double Interval = ((double)(iThrottle * 60 * 24 * iMonthDays) / insCount) * i;
                                        dtEvent = dtEventDate.AddMinutes(Interval);
                                    }
                                    else if (iTimeInterval == 50)
                                    {
                                        //Years
                                    }
                                }
                                else
                                {
                                    dtEvent = dtEventDate;
                                }
                                oBOI.SetAttribute("dRunDate", dtEvent.ToString());
                                oBOI.SetAttribute("iStatus", "30");
                                oCR = oBOI.Save(oBOI);
                                i++;
                            }
                            else
                            {
                                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - Mobile number missing for target instanceid:" + insID;
                            }
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param:  is missing";
                }
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Calculate_ScheduleDateTime(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Calculate schedule instances run date time based on throttle settings";//expalin about this method logic
            try
            {
                var sBO = oParams.Where(m => m.sName.ToLower() == "bo").Select(m => m.sValue).First();
                var iBOIID = oParams.Where(m => m.sName.ToLower() == "iInstanceID".ToLower()).Select(m => m.sValue).First();
                oTrace.oParams.Add(new CNV { sName = "sBO", sValue = sBO });
                oTrace.oParams.Add(new CNV { sName = "iBOIID", sValue = iBOIID });
                if (!string.IsNullOrEmpty(sBO) && !string.IsNullOrEmpty(iBOIID))//check mandatory params are passed or not
                {
                    XIInfraUsers oUser = new XIInfraUsers();
                    CUserInfo oInfo = oUser.Get_UserInfo();
                    XIIXI oXI = new XIIXI();
                    XIInfraCache oCache = new XIInfraCache();
                    var oBOI = oXI.BOI(sBO, iBOIID);
                    if (oBOI != null && oBOI.Attributes.Count() > 0)
                    {
                        var iThrottleType = oBOI.AttributeI("iThrottleType").iValue;
                        var iCount = oBOI.AttributeI("iCount").iValue;
                        var iThrottle = oBOI.AttributeI("iThrottle").iValue;
                        var iTimeInterval = oBOI.AttributeI("iTimeInterval").iValue;
                        var FKiLeadImportID = oBOI.AttributeI("FKiLeadImportID").iValue;
                        var FKiPilotID = oBOI.AttributeI("FKiPilotID").iValue;
                        var sEventDt = oBOI.AttributeI("dtEvent").sValue;
                        var FKiSubscriptionID = oBOI.AttributeI("FKiSubscriptionID").iValue;
                        var FKi1QueryXIGUID = oBOI.AttributeI("FKi1QueryXIGUID").sValue;
                        var FKiBODIDXIGUID = oBOI.AttributeI("FKiBODIDXIGUID").sValue;
                        var iType = oBOI.AttributeI("iType").sValue;
                        DateTime dtEvent = DateTime.MinValue;
                        DateTime dtEventDate = DateTime.MinValue;
                        bool bValidDate = oBOI.IsValidDate(sEventDt);
                        if (bValidDate)
                        {
                            dtEvent = oBOI.ConvertToDtTime(sEventDt);
                        }
                        if (dtEvent != null && dtEvent != DateTime.MinValue && dtEvent.Year.ToString() != "1900") { dtEventDate = dtEvent; }
                        else
                        {
                            dtEventDate = DateTime.Now;
                        }
                        //Load Pilot
                        Guid QSDXIGUID = Guid.Empty;
                        var oPilotI = oXI.BOI("BSPPilot", FKiPilotID.ToString());
                        if (oPilotI != null && oPilotI.Attributes.Count() > 0)
                        {
                            var QSDGUID = oPilotI.AttributeI("FKiQSDIDXIGUID").sValue;
                            Guid.TryParse(QSDGUID, out QSDXIGUID);
                        }
                        XID1Click o1ClickD = new XID1Click();
                        o1ClickD.BOIDXIGUID = new Guid("BF0A6CFA-BAEB-456D-B6CA-8FB614BCC455");
                        if (FKiLeadImportID > 0)
                        {
                            o1ClickD.Query = "select st.ID, it.FKiBOIID from XSubscriptionTargetMap_T st inner join BSPImportTargetMap_T it on it.ID=st.FKiTargetID and it.FKiBODIDXIGUID='" + FKiBODIDXIGUID + "' and FKiImportID='" + FKiLeadImportID + "' where st.FKiSubscriptionID='" + FKiSubscriptionID + "' and st.XIDeleted=0 and st.FKiOrgID=" + oInfo.iOrganizationID;
                        }
                        else
                        {
                            o1ClickD.Query = "select st.ID, it.FKiBOIID from XSubscriptionTargetMap_T st inner join BSPImportTargetMap_T it on it.ID=st.FKiTargetID and it.FKiBODIDXIGUID='" + FKiBODIDXIGUID + "' where st.FKiSubscriptionID='" + FKiSubscriptionID + "' and st.XIDeleted=0 and st.FKiOrgID=" + oInfo.iOrganizationID;
                        }

                        var Subs = o1ClickD.OneClick_Run();
                        if (Subs != null && Subs.Count() > 0)
                        {
                            List<string> TIDs = new List<string>();
                            foreach (var subtarmap in Subs.Values.ToList())
                            {
                                TIDs.Add(subtarmap.AttributeI("FKiBOIID").sValue);
                            }
                            var FilteredIDs = string.Join(",", TIDs);
                            XID1Click o1ClickDTar = new XID1Click();
                            o1ClickDTar = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, FKi1QueryXIGUID);
                            XID1Click o1ClickCTar = new XID1Click();
                            o1ClickCTar = (XID1Click)o1ClickDTar.Clone(o1ClickDTar);
                            o1ClickCTar.Query = o1ClickCTar.AddSearchParameters(o1ClickCTar.Query, "ID in (" + FilteredIDs + ")");
                            var Targets = o1ClickCTar.OneClick_Run();
                            if (Targets != null && Targets.Count() > 0)
                            {
                                foreach (var ins in Targets.Values.ToList())
                                {
                                    var TID = ins.AttributeI("id").sValue;
                                    if (TIDs.Contains(TID))
                                    {
                                        int i = 0;
                                        var insID = ins.AttributeI("ID").sValue;
                                        var sMobileNo = ins.AttributeI("sMob").sValue;
                                        if (!string.IsNullOrEmpty(sMobileNo))
                                        {
                                            var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIScheduleInstance");
                                            oBOI = new XIIBO();
                                            oBOI.BOD = oBOD;
                                            oBOI.SetAttribute("sMobileNo", sMobileNo);
                                            oBOI.SetAttribute("FKiCampaignID", iBOIID);
                                            oBOI.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                            oBOI.SetAttribute("FKiBODIDXIGUID", o1ClickCTar.BOIDXIGUID.ToString());
                                            oBOI.SetAttribute("FKiBOIID", insID);
                                            oBOI.SetAttribute("iType", iType);
                                            if (iThrottleType == 10)
                                            {
                                                if (iTimeInterval == 10)
                                                {
                                                    //Minutes: Ex: Per Minute 100 :- 60/100 = 0.6 -> For every 0.6 seconds 1 schedule instance will be created
                                                    double Interval = ((double)60 / iCount) * i;
                                                    dtEvent = dtEventDate.AddSeconds(Interval);
                                                }
                                                else if (iTimeInterval == 20)
                                                {
                                                    //Hours: Ex: Per hour 100 :- 60/100 = 0.6 -> For every 0.6 minutes 1 schedule instance will be created
                                                    double Interval = ((double)60 / iCount) * i;
                                                    dtEvent = dtEventDate.AddMinutes(Interval);
                                                }
                                                else if (iTimeInterval == 30)
                                                {
                                                    //Days: Ex: Per day 100 :- (60*24)/100 = 1440/100=14.4 -> For every 14.4 minutes 1 schedule instance will be created
                                                    double Interval = ((double)(60 * 24) / iCount) * i;
                                                    dtEvent = dtEventDate.AddMinutes(Interval);
                                                }
                                                else if (iTimeInterval == 40)
                                                {
                                                    //Months: Ex: Per month 100 :- (60*24*30)/100 = 43200/100=432 -> For every 432 minutes 1 schedule instance will be created
                                                    int year = DateTime.Now.Year;
                                                    int month = DateTime.Now.Month;
                                                    int iMonthDays = DateTime.DaysInMonth(year, month);
                                                    double Interval = ((double)(60 * 24 * iMonthDays) / iCount) * i;
                                                    dtEvent = dtEventDate.AddMinutes(Interval);
                                                }
                                                else if (iTimeInterval == 50)
                                                {
                                                    //Years
                                                }
                                            }
                                            else if (iThrottleType == 20)
                                            {
                                                var insCount = Targets.Count;
                                                if (iTimeInterval == 10)
                                                {
                                                    //Minutes: Ex: 100 instances over next 30 minutes :- 30*60 = 1800 seconds => 100/1800 = 0.055 -> For every 0.055 seconds 1 schedule instance will be created
                                                    //Minutes: Ex: 100 instances over next 60 minutes :- 60*60 = 3600 seconds => 100/3600 = 0.027 -> For every 0.027 seconds 1 schedule instance will be created
                                                    double Interval = ((double)insCount / (iThrottle * 60)) * i;
                                                    double Interval1 = ((double)(iThrottle * 60) / insCount) * i;
                                                    dtEvent = dtEventDate.AddSeconds(Interval1);
                                                }
                                                else if (iTimeInterval == 20)
                                                {
                                                    //Hours: Ex: 100 instances over next 2 hours :- 2*60 = 120 minutes => 120/100 = 1.2 -> For every 1.2 minutes 1 schedule instance will be created
                                                    double Interval = ((double)(iThrottle * 60) / insCount) * i;
                                                    dtEvent = dtEventDate.AddMinutes(Interval);
                                                }
                                                else if (iTimeInterval == 30)
                                                {
                                                    //Days: Ex: 100 instances over next 2 day :- (2 * 60 * 24)/100 = 2880/100=28.8 -> For every 28.8 minutes 1 schedule instance will be created
                                                    double Interval = ((double)(iThrottle * 60 * 24) / insCount) * i;
                                                    dtEvent = dtEventDate.AddMinutes(Interval);
                                                }
                                                else if (iTimeInterval == 40)
                                                {
                                                    //Months: Ex: 100 instances over next 2 months :- (2*60*24*30)/100 = 86400/100=864 -> For every 864 minutes 1 schedule instance will be created
                                                    int year = DateTime.Now.Year;
                                                    int month = DateTime.Now.Month;
                                                    int iMonthDays = DateTime.DaysInMonth(year, month);
                                                    double Interval = ((double)(iThrottle * 60 * 24 * iMonthDays) / insCount) * i;
                                                    dtEvent = dtEventDate.AddMinutes(Interval);
                                                }
                                                else if (iTimeInterval == 50)
                                                {
                                                    //Years
                                                }
                                            }
                                            else
                                            {
                                                dtEvent = dtEventDate;
                                            }
                                            oBOI.SetAttribute("dRunDate", dtEvent.ToString());
                                            oBOI.SetAttribute("iStatus", "30");
                                            oCR = oBOI.Save(oBOI);
                                            i++;
                                        }
                                        else
                                        {
                                            oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - Mobile number missing for target instanceid:" + insID;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sBO or iBOIID is missing";
                }
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Versionise_Instance(string BODGuid, string UID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Check version is configured on object definition and call Versionise method";//expalin about this method logic
            try
            {
                //oTrace.oParams.Add(new CNV { sName = "", sValue = "" });
                if (!string.IsNullOrEmpty(BODGuid) && !string.IsNullOrEmpty(UID))//check mandatory params are passed or not
                {
                    Guid BODXIGuid = Guid.Empty;
                    if (Guid.TryParse(BODGuid, out BODXIGuid))
                    {
                        XIInfraCache oCache = new XIInfraCache();
                        var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BODXIGuid.ToString());
                        if (oBOD.XIVersion == 10)
                        {
                            oCR = Versionise(BODGuid, UID);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                oCResult.oResult = "Success";
                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: BODGuid: " + BODGuid + " or UID: " + UID + " is missing";
                }
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public CResult Versionise(string BODGuid, string UID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "create a version copy for object instance";//expalin about this method logic
            try
            {
                //oTrace.oParams.Add(new CNV { sName = "", sValue = "" });
                if (!string.IsNullOrEmpty(BODGuid) && !string.IsNullOrEmpty(UID))//check mandatory params are passed or not
                {
                    Guid BODXIGuid = Guid.Empty;
                    if (Guid.TryParse(BODGuid, out BODXIGuid))
                    {
                        XIInfraCache oCache = new XIInfraCache();
                        var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BODXIGuid.ToString());
                        XIIXI oXII = new XIIXI();
                        int iBOIID = 0;
                        int.TryParse(UID, out iBOIID);
                        Guid BOIIDGuid = Guid.Empty;
                        Guid.TryParse(UID, out BOIIDGuid);
                        XIIBO oBOI = new XIIBO();
                        if (BOIIDGuid != null && BOIIDGuid != Guid.Empty)
                        {
                            oBOI = oXII.BOI(oBOD.Name, BOIIDGuid.ToString());
                        }
                        else if (iBOIID > 0)
                        {
                            oBOI = oXII.BOI(oBOD.Name, iBOIID.ToString());
                        }
                        if (oBOI != null && oBOI.Attributes.Count() > 0)
                        {
                            oBOI.Attributes.Values.ToList().ForEach(m => m.bDirty = true);
                            var iCurrentVersion = oBOI.AttributeI("XIVersion").iValue;
                            oBOI.SetAttribute(oBOD.sPrimaryKey, "");
                            oBOI.SetAttribute("xiguid", "");
                            oBOI.SetAttribute("XIDeleted", "20");
                            oBOI.SetAttribute("FKiCurrentVersionID", iBOIID.ToString());
                            oBOI.SetAttribute("FKiCurrentVersionIDXIGUID", BOIIDGuid.ToString());
                            oCR = oBOI.Save(oBOI);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                oCResult.oResult = iCurrentVersion;
                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: BODGuid: " + BODGuid + " or UID: " + UID + " is missing";
                }
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

    }

    public class XIIBOList
    {
        public Dictionary<string, XIIBO> oDictionaryBOI;
        public string sSubStructName { get; set; }
        public XIIBO Item(int iIndex)
        {
            XIIBO oBOI = new XIIBO();
            if (oDictionaryBOI.Count() > iIndex)
            {
                oBOI = oDictionaryBOI[iIndex.ToString()];
                oBOI.sSubStructName = sSubStructName;
            }
            //else
            //{

            //}
            return oBOI;
        }
    }


}