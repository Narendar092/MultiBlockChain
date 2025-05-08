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
using System.Data.SqlClient;
using Dapper;
using System.Configuration;
using System.Web;
using XIDatabase;
using XISystem;
using System.Data;
using System.Text.RegularExpressions;
using xiEnumSystem;

namespace XICore
{
    public class XIDXI : XIDefinitionBase
    {
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public int iOrgID { get; set; }
        public string sAppName { get; set; }
        public string sUserID { get; set; }
        public string sTypeCyption { get; set; }
        public int FKiApplicationID { get; set; }
        Dictionary<Guid, string> DataSrcs = new Dictionary<Guid, string>();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        XIDBAPI XIEnvConnection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIEnvironmentDbContext"].ConnectionString);
        #region BOMethods

        public CResult Get_BODefinition(string sBOName = "", string sUID = "")
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
                XIUtility.IDInsteadOfGUID(sUID, "BO");
                XIDBO oBOD = new XIDBO();
                long iID = 0;
                if (sBOName != "" || !string.IsNullOrEmpty(sUID))
                {
                    //Load BO Definition
                    oBOD = Load_BO(sBOName, sUID);
                }
                else if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }

                if (oBOD != null)
                    oBOD.oParent = this;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oBOD;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading BO definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_BODefinitionALL(string sBOName = "", string sUID = "")
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
                XIUtility.IDInsteadOfGUID(sUID, "BO");
                XIDBO oBOD = new XIDBO();
                long iID = 0; bool isValidGUID = false;
                Guid guidOutput;
                if (!string.IsNullOrEmpty(sUID))
                {
                    if (long.TryParse(sUID, out iID))
                    {

                    }
                    else
                    {
                        isValidGUID = Guid.TryParse(sUID, out guidOutput);
                    }
                }
                if (!string.IsNullOrEmpty(sBOName) || !string.IsNullOrEmpty(sUID) && (iID > 0 || isValidGUID))
                {
                    XIInfraCache oCache = new XIInfraCache();
                    //Load BO Definition
                    //oBOD = Load_BO(sBOName, sUID);
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, sUID);
                    var FKAttributes = oBOD.Attributes.Where(m => m.Value.FKiType > 0 && !string.IsNullOrEmpty(m.Value.sFKBOName) && m.Value.sFKBOName != oBOD.Name).ToList();
                    foreach (var item in FKAttributes)
                    {
                        var oFKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, item.Value.sFKBOName);
                        string sBODataSource = string.Empty;
                        var sFKBOName = item.Value.sFKBOName;
                        Dictionary<string, object> Params = new Dictionary<string, object>();
                        Params["Name"] = sFKBOName;
                        if (oFKBOD != null && oFKBOD.FKiApplicationID > 0 && oFKBOD.TableName != "Organizations" && oFKBOD.TableName != "XIApplication_T" && oFKBOD.TableName != "XIAppRoles_AR_T" && oFKBOD.TableName != "XIWidget_T")
                        {
                            Params["FKiApplicationID"] = oFKBOD.FKiApplicationID.ToString();
                        }
                        string sSelectFields = string.Empty;
                        sSelectFields = "Name,BOID,iDataSource,sSize,TableName,sPrimaryKey,sType,iDataSourceXIGUID,XIGUID,iOrgObject";
                        var FKBOD = Connection.Select<XIDBO>("XIBO_T_N", Params, sSelectFields).FirstOrDefault();
                        //var FKBOD = Load_BO(FKBO.Name, FKBO.BOID);
                        //var BO = AllBOs.Where(m => m.TableName == sTableName).FirstOrDefault();
                        if (FKBOD != null)
                        {
                            CUserInfo oInfo = new CUserInfo();
                            XIInfraUsers oUser = new XIInfraUsers();
                            oInfo = oUser.Get_UserInfo();
                            //if (SessionItems == null)
                            //{

                            //}
                            if (oFKBOD.TableName == "XIAPPUsers_AU_T" || oFKBOD.TableName == "XIAppRoles_AR_T" || oFKBOD.TableName == "Organizations")
                            {
                                if (oInfo.sCoreDataBase != null)
                                {
                                    var DataSource = Get_DataSourceDefinition(oInfo.sCoreDataBase);
                                    var BODS = ((XIDataSource)DataSource.oResult);
                                    sBODataSource = GetBODataSource(BODS.XIGUID.ToString(), oBOD.FKiApplicationID);
                                }
                                else
                                {
                                    sBODataSource = GetBODataSource(FKBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                                    DataSrcs[FKBOD.iDataSourceXIGUID] = sBODataSource;
                                }
                            }
                            if (oFKBOD.iOrgObject > 0 || oFKBOD.TableName == "RefTraceStage_T" || oFKBOD.TableName == "refValidTrace_T" || oFKBOD.TableName == "refLeadQuality_T" || oFKBOD.TableName == "TraceTransactions_T")
                            {
                                //if (!string.IsNullOrEmpty(oInfo.sDatabaseName))
                                //{
                                //    var DataSource = Get_DataSourceDefinition(oInfo.sDatabaseName);
                                //    var BODS = ((XIDataSource)DataSource.oResult);
                                //    sBODataSource = GetBODataSource(BODS.XIGUID.ToString(), oBOD.FKiApplicationID);
                                //}
                                //else
                                {
                                    sBODataSource = GetBODataSource(FKBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                                    DataSrcs[FKBOD.iDataSourceXIGUID] = sBODataSource;
                                }
                            }
                            else if (DataSrcs.ContainsKey(FKBOD.iDataSourceXIGUID))
                            {
                                sBODataSource = DataSrcs[FKBOD.iDataSourceXIGUID];
                            }
                            else
                            {
                                sBODataSource = GetBODataSource(FKBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                                DataSrcs[FKBOD.iDataSourceXIGUID] = sBODataSource;
                            }

                            oBOD.Attributes[item.Value.Name.ToLower()].sFKBOSize = FKBOD.sSize;
                            oBOD.Attributes[item.Value.Name.ToLower()].sFKBOName = FKBOD.Name;
                            //Get BO Default Popup
                            Dictionary<string, object> DefParams = new Dictionary<string, object>();
                            DefParams["FKiBOIDXIGUID"] = FKBOD.XIGUID;
                            DefParams["XIDeleted"] = "0";
                            var BODefaults = Connection.Select<XIDBODefault>("XIBOUIDefault_T", DefParams).FirstOrDefault();
                            if (BODefaults != null)
                            {
                                if (BODefaults.iPopupIDXIGUID != null && BODefaults.iPopupIDXIGUID != Guid.Empty)
                                {
                                    oBOD.Attributes[item.Value.Name.ToLower()].iDefaultPopupIDXIGUID = BODefaults.iPopupIDXIGUID;
                                }
                                else if (BODefaults.iPopupID > 0)
                                {
                                    oBOD.Attributes[item.Value.Name.ToLower()].iDefaultPopupID = BODefaults.iPopupID;
                                }
                            }
                            if (FKBOD.sSize == "10")//maximum number of results in dropdown -- To Do
                            {
                                var Con = new XIDBAPI(sBODataSource);
                                if (item.Value.FKiType == 40 && (item.Value.iOneClickID > 0 || (item.Value.iOneClickIDXIGUID != null && item.Value.iOneClickIDXIGUID != Guid.Empty)))
                                {
                                    string suid = "1click_" + Convert.ToString(item.Value.iOneClickIDXIGUID);
                                    List<CNV> oParams = new List<CNV>();
                                    oParams.Add(new CNV { sName = "{XIP|iAPPID}", sValue = oBOD.FKiApplicationID.ToString() });
                                    oParams.Add(new CNV { sName = "{XIP|iOrgID}", sValue = oInfo.iOrganizationID.ToString() });
                                    oParams.Add(new CNV { sName = "{XIP|iUserOrgID}", sValue = oInfo.iOrganizationID.ToString() });
                                    var oResult = Get_AutoCompleteList(suid, sBOName, oParams);
                                    List<XIDropDown> FKDDL = new List<XIDropDown>();
                                    if (oResult.bOK && oResult.oResult != null)
                                    {
                                        if (FKBOD.sSize == "10")
                                        {
                                            var DDL = (List<XIDFieldOptionList>)oResult.oResult;
                                            FKDDL = DDL.Select(m => new XIDropDown { text = m.sOptionValue, Expression = m.sOptionName }).ToList();
                                        }
                                        else
                                        {
                                            var DDL = (Dictionary<string, string>)oResult.oResult;
                                            FKDDL = DDL.Select(m => new XIDropDown { text = m.Key, Expression = m.Value }).ToList();
                                        }

                                    }
                                    oBOD.Attributes[item.Value.Name.ToLower()].FieldDDL = FKDDL;
                                }
                                else if (FKBOD != null)
                                {
                                    Dictionary<string, object> GrpParams = new Dictionary<string, object>();
                                    GrpParams["BOIDXIGUID"] = FKBOD.XIGUID.ToString();
                                    GrpParams["GroupName"] = "Label";
                                    var FKBOLabelG = Connection.Select<XIDGroup>("XIBOGroup_T_N", GrpParams).FirstOrDefault();
                                    var LabelGroup = FKBOLabelG.BOFieldNames;
                                    if (!string.IsNullOrEmpty(LabelGroup))
                                    {
                                        string sOrgWhere = string.Empty;
                                        if (FKBOD.iOrgObject == 1)
                                        {
                                            sOrgWhere = "FKiOrgID=" + oInfo.iOrganizationID;
                                        }
                                        else if (FKBOD.iOrgObject == 2)
                                        {
                                            sOrgWhere = "(FKiOrgID=" + oInfo.iOrganizationID + " or FKiOrgID is null)";
                                        }
                                        Dictionary<string, string> DDL = Con.SelectDDL(LabelGroup, FKBOD.TableName, sOrgWhere);
                                        var FKDDL = DDL.Select(m => new XIDropDown { text = m.Key, Expression = m.Value }).ToList();
                                        oBOD.Attributes[item.Value.Name.ToLower()].FieldDDL = FKDDL;
                                    }
                                }
                            }
                            else if (FKBOD.sSize == "20")
                            {
                                //var Con = new XIDBAPI(sBODataSource);
                                //if (FKBOD != null)
                                //{
                                //    Dictionary<string, object> GrpParams = new Dictionary<string, object>();
                                //    GrpParams["BOID"] = FKBOD.BOID;
                                //    GrpParams["GroupName"] = "Label";
                                //    var FKBOLabelG = Connection.Select<XIDGroup>("XIBOGroup_T_N", GrpParams).FirstOrDefault();
                                //    var LabelGroup = FKBOLabelG.BOFieldNames;
                                //    if (!string.IsNullOrEmpty(LabelGroup))
                                //    {

                                //        string FinalString = FKBOLabelG.ConcatanateFields(LabelGroup, " ");
                                //        FinalString = FKBOD.sPrimaryKey + "," + FinalString;
                                //        Dictionary<string, string> DDL = Con.SelectDDL(FinalString, FKBOD.TableName);
                                //        var FKDDL = DDL.Select(m => new XIDropDown { text = m.Key, Expression = m.Value }).ToList();
                                //        oBOD.Attributes[item.Value.Name.ToLower()].FieldDDL = FKDDL;
                                //    }
                                //}
                            }
                        }
                    }
                    FKAttributes = oBOD.Attributes.Where(m => m.Value.bIsScript == true).ToList();
                    foreach (var item in FKAttributes)
                    {
                        Dictionary<string, object> DefParams = new Dictionary<string, object>();
                        DefParams["FKiBOID"] = item.Value.BOID;
                        var BODefaults = Connection.Select<XIDBODefault>("XIBOUIDefault_T", DefParams).FirstOrDefault();
                        if (BODefaults != null)
                        {
                            if (BODefaults.iPopupID > 0)
                            {
                                oBOD.Attributes[item.Value.Name.ToLower()].iDefaultPopupID = BODefaults.iPopupID;
                            }
                        }
                    }
                }
                else if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                //if (!string.IsNullOrEmpty(sUID) && iID > 0)
                //{
                //    oBOD.FKiApplicationID = oBOD.FKiApplicationID;
                //}
                //else
                //{
                //    oBOD.FKiApplicationID = FKiApplicationID;
                //}
                if (oBOD != null)
                    oBOD.oParent = this;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oBOD;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading BO definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }
        public List<XIDropDown> Get_ddlBOFieldAttributes(string iBOID = "")
        {
            List<XIDropDown> oXIDAttr = new List<XIDropDown>();
            Dictionary<string, object> Params = new Dictionary<string, object>();
            if (iBOID != "0")
            {
                Params["BOID"] = iBOID;
                var AllAttrs = Connection.Select<XIDAttribute>("XIBOAttribute_T_N", Params).OrderBy(x => x.ID).ToList();
                oXIDAttr = AllAttrs.Select(m => new XIDropDown { text = m.Name, Value = m.ID }).ToList();
            }
            return oXIDAttr;
        }
        public List<XIDropDown> Get_BOAttributes(string iBOID = "")
        {
            int iBODID = 0;
            Guid BOIDXIGUID = Guid.Empty;
            int.TryParse(iBOID, out iBODID);
            Guid.TryParse(iBOID, out BOIDXIGUID);
            List<XIDropDown> oXIDAttr = new List<XIDropDown>();
            Dictionary<string, object> Params = new Dictionary<string, object>();
            if ((BOIDXIGUID != null && BOIDXIGUID != Guid.Empty) || iBODID > 0)
            {
                if (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)
                {
                    Params["BOIDXIGUID"] = BOIDXIGUID;
                }
                else if (iBODID > 0)
                {
                    Params["BOID"] = iBOID;
                }
                var AllAttrs = Connection.Select<XIDAttribute>("XIBOAttribute_T_N", Params).OrderBy(x => x.ID).ToList();
                oXIDAttr = AllAttrs.Select(m => new XIDropDown { text = m.Name, Value = m.ID, sGUID = m.XIGUID.ToString() }).ToList();
            }
            return oXIDAttr;
        }

        public XIDGroup Get_GroupDefinition(string iBOID, string sGroupName)
        {
            Dictionary<string, object> GrpParams = new Dictionary<string, object>();
            GrpParams["BOID"] = iBOID;
            GrpParams["GroupName"] = sGroupName;
            var oGroupD = Connection.Select<XIDGroup>("XIBOGroup_T_N", GrpParams).FirstOrDefault();
            return oGroupD;
        }

        public List<XIDropDown> Get_ddlBODataSourceDefinition(string iBOID = "")
        {
            List<XIDropDown> AllDataSources = new List<XIDropDown>();
            Dictionary<string, object> Params = new Dictionary<string, object>();
            List<XIDataSource> DataSourceDetails = new List<XIDataSource>();
            string sApplication = ConfigurationManager.AppSettings["AppName"];
            if (!string.IsNullOrEmpty(sApplication) && sApplication.ToLower() == "motorhome")
            {
                DataSourceDetails = Connection.Select<XIDataSource>("XIDataSource_XID_T", Params).OrderBy(x => x.ID).ToList();
            }
            else
            {
                DataSourceDetails = XIEnvConnection.Select<XIDataSource>("XIDataSource_XID_T", Params).OrderBy(x => x.ID).ToList();
            }
            //var DataSourceDetails = XIEnvConnection.Select<XIDataSource>("XIDataSource_XID_T", Params).ToList();
            foreach (var item in DataSourceDetails)
            {
                AllDataSources.Add(new XIDropDown
                {
                    text = item.sName,
                    Value = item.ID
                });
            }
            AllDataSources.Insert(0, new XIDropDown
            {
                text = "Organisation DB",
                Value = -2
            });
            AllDataSources.Insert(0, new XIDropDown
            {
                text = "Application DB",
                Value = -1
            });
            return AllDataSources;
        }

        private XIDBO Load_BO(string sBOName, string sUID)
        {
            try
            {
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                XIDBO oBOD = new XIDBO();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (iID > 0)
                {
                    Params["BOID"] = iID;
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    Params["xiguid"] = sUID;
                }
                else if (!string.IsNullOrEmpty(sBOName))
                {
                    Params["Name"] = sBOName;
                }

                oBOD = Connection.Select<XIDBO>("XIBO_T_N ", Params).FirstOrDefault();
                if (oBOD == null)
                {
                    Params = new Dictionary<string, object>();
                    if (iID > 0)
                    {
                        Params["BOID"] = iID;
                    }
                    else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                    {
                        Params["xiguid"] = sUID;
                    }
                    else if (!string.IsNullOrEmpty(sBOName))
                    {
                        Params["LabelName"] = sBOName;
                    }
                    oBOD = Connection.Select<XIDBO>("XIBO_T_N ", Params).FirstOrDefault();
                }
                if (oBOD == null)
                {
                    Params = new Dictionary<string, object>();
                    if (iID > 0)
                    {
                        Params["BOID"] = iID;
                    }
                    else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                    {
                        Params["xiguid"] = sUID;
                    }
                    else if (!string.IsNullOrEmpty(sBOName))
                    {
                        Params["TableName"] = sBOName;
                    }
                    oBOD = Connection.Select<XIDBO>("XIBO_T_N ", Params).FirstOrDefault();
                }
                if (oBOD != null)
                {
                    Dictionary<string, object> NVParams = new Dictionary<string, object>();
                    NVParams["BOIDXIGUID"] = oBOD.XIGUID.ToString();
                    var AttrButes = Connection.Select<XIDAttribute>("XIBOAttribute_T_N", NVParams).OrderBy(x => x.ID).ToList();
                    var Groups = Connection.Select<XIDGroup>("XIBOGroup_T_N", NVParams).OrderBy(x => x.ID).ToList();
                    Dictionary<string, object> ValuePirs = new Dictionary<string, object>();
                    ValuePirs["FKiBOIDXIGUID"] = oBOD.XIGUID.ToString();
                    ValuePirs["XIDeleted"] = "0";
                    var Scripts = Connection.Select<XIDScript>("XIBOScript_T", ValuePirs).OrderBy(x => x.ID).ToList();
                    var BoUiDetails = Connection.Select<XIDBOUI>("XIBOUIDetails_T", ValuePirs).OrderBy(x => x.ID).ToList();

                    Dictionary<string, object> StrNVParams = new Dictionary<string, object>();
                    StrNVParams["BOIDXIGUID"] = oBOD.XIGUID.ToString();
                    StrNVParams["FKiParentID"] = "#";
                    var BoStructures = Connection.Select<XIDStructure>("XIBOStructure_T", StrNVParams).OrderBy(x => x.ID).ToList();
                    Dictionary<string, object> ContentPairs = new Dictionary<string, object>();
                    ContentPairs["BOXIGUID"] = oBOD.XIGUID.ToString();
                    var BOTemplets = Connection.Select<XIContentEditors>("XITemplate_T", ContentPairs).OrderBy(x => x.ID).ToList();
                    Dictionary<string, object> BOActionNV = new Dictionary<string, object>();
                    BOActionNV["FKiBOIDXIGUID"] = oBOD.XIGUID.ToString();
                    BOActionNV["iStatus"] = "10";
                    var BOActions = Connection.Select<XIDBOAction>("XIBOAction_T", BOActionNV).OrderBy(x => x.ID).ToList();

                    Dictionary<string, object> BODefaultNV = new Dictionary<string, object>();
                    BODefaultNV["FKiBOIDXIGUID"] = oBOD.XIGUID.ToString();
                    BODefaultNV["StatusTypeID"] = "10";
                    var BODefault = Connection.Select<XIDBODefault>("XIBOUIDefault_T", BODefaultNV).OrderBy(x => x.ID).ToList();
                    if (Groups.Count > 0)
                    {
                        Groups.ForEach(m =>
                        {
                            oBOD.Groups[m.GroupName.ToLower()] = m;
                        });
                    }
                    if (Scripts.Count > 0)
                    {
                        Scripts.ForEach(m =>
                        {
                            oBOD.Scripts[m.sName.ToLower()] = m;
                        });
                    }
                    if (BoStructures.Count > 0)
                    {
                        BoStructures.ForEach(m =>
                        {
                            oBOD.Structures[m.sStructureName.ToLower()] = m;
                        });
                    }
                    if (BOTemplets.Count > 0)
                    {
                        BOTemplets.ForEach(m =>
                        {
                            oBOD.Templates[m.Name.ToLower()] = m;
                        });
                    }
                    if (BOActions.Count > 0)
                    {
                        BOActions.ForEach(m =>
                        {
                            oBOD.Actions[m.sName.ToLower()] = m;
                        });
                    }

                    if (AttrButes.Count > 0)
                    {
                        AttrButes.ForEach(m =>
                        {
                            if (m.IsOptionList)
                            {
                                Dictionary<string, object> AttrNvs = new Dictionary<string, object>();
                                AttrNvs["BOFieldIDXIGUID"] = m.XIGUID.ToString();
                                AttrNvs[XIConstant.Key_XIDeleted] = "0";
                                //AttrNvs["StatusTypeID"] = "10";
                                List<int> optIDs = new List<int>();
                                List<XIDOptionList> OrderOptList = new List<XIDOptionList>();
                                var optionList = Connection.Select<XIDOptionList>("XIBOOptionList_T_N ", AttrNvs).OrderBy(x => x.ID).ToList();
                                var optValues = optionList.Where(b => !string.IsNullOrEmpty(b.sValues)).Select(b => b.sValues).ToList();
                                foreach (var opt in optValues)
                                {
                                    var iOptVal = 0;
                                    bool bNumeric = int.TryParse(opt, out iOptVal);
                                    if (bNumeric)
                                    {
                                        optIDs.Add(iOptVal);
                                    }
                                }
                                if (optIDs != null && optIDs.Count() > 0 && optionList.Count() == optIDs.Count())
                                {
                                    optIDs = optIDs.OrderBy(b => b).ToList();
                                    foreach (var opt in optIDs)
                                    {
                                        OrderOptList.Add(optionList.Where(b => b.sValues == opt.ToString()).FirstOrDefault());
                                    }
                                    m.OptionList = OrderOptList;
                                }
                                else
                                {
                                    m.OptionList = optionList;
                                }
                                oBOD.Attributes[m.Name.ToLower()] = m;
                            }
                            else if (m.iMasterDataID > 0)
                            {
                                XIInfraCache oCache = new XIInfraCache();
                                var FKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Master Data", null);
                                var sBODataSource = GetBODataSource(FKBOD.iDataSourceXIGUID.ToString(), FKBOD.FKiApplicationID);
                                var Con = new XIDBAPI(sBODataSource);
                                Dictionary<string, object> GrpParams = new Dictionary<string, object>();
                                GrpParams["BOIDXIGUID"] = FKBOD.XIGUID.ToString();
                                GrpParams["GroupName"] = "Label";
                                var FKBOLabelG = Connection.Select<XIDGroup>("XIBOGroup_T_N", GrpParams).FirstOrDefault();
                                var LabelGroup = FKBOLabelG.BOFieldNames;
                                if (!string.IsNullOrEmpty(LabelGroup))
                                {
                                    string sWhrcondition = "code=" + m.iMasterDataID;
                                    Dictionary<string, string> DDL = Con.SelectDDL(LabelGroup, FKBOD.TableName, sWhrcondition);
                                    var FKDDL = DDL.Select(n => new XIDOptionList { sOptionName = n.Value, sValues = n.Key }).ToList();
                                    m.OptionList = FKDDL;
                                    m.IsOptionList = true;
                                    oBOD.Attributes[m.Name.ToLower()] = m;
                                }
                            }
                            else
                            {
                                oBOD.Attributes[m.Name.ToLower()] = m;
                            }
                            Dictionary<string, object> LableNvs = new Dictionary<string, object>();
                            LableNvs["FKiAttributeIDXIGUID"] = m.XIGUID.ToString();
                            LableNvs[XIConstant.Key_XIDeleted] = "0";
                            //AttrNvs["StatusTypeID"] = "10";
                            var AttrLables = Connection.Select<XILabel>("XILabel_T ", LableNvs).ToList();
                            if (AttrLables != null && AttrLables.Count() > 0)
                            {
                                m.Label = AttrLables;
                                oBOD.Attributes[m.Name.ToLower()] = m;
                            }
                        });
                        //foreach (var oAttr in AttrButes)
                        //{
                        //    if (oAttr.TypeID == 150)
                        //    {
                        //        var oResult = SetDefaultDateRange(oAttr.sMinDate, oAttr.sMaxDate);
                        //        if (oResult != null && oResult.Count() > 0)
                        //        {
                        //            if (oResult.ContainsKey("sMinDate"))
                        //            {
                        //                oAttr.sMinDate = oResult["sMinDate"];
                        //            }
                        //            if (oResult.ContainsKey("sMaxDate"))
                        //            {
                        //                oAttr.sMaxDate = oResult["sMaxDate"];
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    if (BoUiDetails != null)
                    {
                        oBOD.BOUID = BoUiDetails.FirstOrDefault();
                    }
                    if (BODefault != null)
                    {
                        oBOD.Default = BODefault.FirstOrDefault();
                    }
                    if (oBOD.Scripts.Count() > 0)
                    {
                        Dictionary<string, object> ListAttributeScriptResults = new Dictionary<string, object>();
                        foreach (var item in oBOD.Scripts)
                        {
                            var ScriptResult = new List<XIDScriptResult>();
                            ListAttributeScriptResults["FKiScriptIDXIGUID"] = item.Value.XIGUID;
                            //var resultforScrResult = Connection.Select<XIDScriptResult>("XIBOScriptResult_T", ListAttributeScriptResults).ToList();
                            ScriptResult.AddRange(Connection.Select<XIDScriptResult>("XIBOScriptResult_T", ListAttributeScriptResults).OrderBy(x => x.ID).ToList());
                            if (ScriptResult != null && ScriptResult.Count() > 0)
                                oBOD.Scripts[item.Key.ToLower()].ScriptResults.AddRange(ScriptResult);
                        }
                    }
                }

                return oBOD;
            }
            catch (Exception ex)
            {
                //throw ex;
                return null;
            }
        }


        private XIDBO Load_BO_OLD(string sBOName, string sUID)
        {
            try
            {
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                XIDBO oBOD = new XIDBO();
                string sBODefinition = "select * from XIBO_T_N BOD " +
              "inner join XIBOAttribute_T_N Attr on BOD.BOID = Attr.BOID " +
              "left join XIBOGroup_T_N Grp on BOD.BOID = Grp.BOID " +
              "left join XIBOScript_T Scr on BOD.BOID = Scr.FKiBOID " +
              "left join XIBOUIDetails_T BOUI on BOD.BOID = BOUI.FKiBOID " +
              "left join XIBOStructure_T Strct on BOD.BOID = Strct.BOID " +
              "left join XIBOOptionList_T_N Opt on Attr.ID = Opt.BOFieldID " +
                "";

                if (iID > 0)
                {
                    sBODefinition = sBODefinition + "WHERE BOD.BOID = @iBODID";
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    sBODefinition = sBODefinition + "WHERE BOD.xiguid = @xiguid";
                }
                else if (!string.IsNullOrEmpty(sBOName))
                {
                    sBODefinition = sBODefinition + "WHERE BOD.Name = @Name";
                }
                var param = new
                {
                    Name = sBOName,
                    iBODID = iID,
                    xiguid = sUID
                };
                var lookupBOs = new Dictionary<int, XIDBO>();
                using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
                {
                    Conn.Open();
                    var lookupAttrs = new Dictionary<int, XIDAttribute>();
                    var lookupGroups = new Dictionary<int, XIDGroup>();
                    var lookupScripts = new Dictionary<long, XIDScript>();
                    var lookupStructs = new Dictionary<long, XIDStructure>();
                    var lookupOpts = new Dictionary<int, XIDOptionList>();
                    Conn.Query<XIDBO, XIDAttribute, XIDGroup, XIDScript, XIDBOUI, XIDStructure, XIDOptionList, XIDBO>(sBODefinition,
                                        (oBO, oAttribure, oGroup, oScript, oBOUI, oStructure, oOptionList) =>
                                        {
                                            oBOD = new XIDBO();
                                            if (oBO != null)
                                            {
                                                if (!lookupBOs.TryGetValue(oBO.BOID, out oBOD))
                                                {
                                                    lookupBOs.Add(oBO.BOID, oBOD = oBO);
                                                }
                                                XIDAttribute oAttr;
                                                if (oAttribure != null)
                                                {
                                                    if (!lookupAttrs.TryGetValue(oAttribure.ID, out oAttr))
                                                    {
                                                        lookupAttrs.Add(oAttribure.ID, oAttr = oAttribure);
                                                        oBOD.Attributes[oAttr.Name.ToLower()] = oAttr;
                                                    }
                                                    XIDOptionList oOpt;
                                                    if (oOptionList != null)
                                                    {
                                                        if (!lookupOpts.TryGetValue(oOptionList.ID, out oOpt))
                                                        {
                                                            lookupOpts.Add(oOptionList.ID, oOpt = oOptionList);
                                                            if (oAttr.OptionList == null)
                                                            {
                                                                oAttr.OptionList = new List<XIDOptionList>();
                                                                oAttr.OptionList.Add(oOpt);
                                                            }
                                                            else
                                                            {
                                                                oAttr.OptionList.Add(oOpt);
                                                            }
                                                        }
                                                    }
                                                }
                                                if (oGroup != null)
                                                {
                                                    XIDGroup oGrp;
                                                    if (!lookupGroups.TryGetValue(oGroup.ID, out oGrp))
                                                    {
                                                        lookupGroups.Add(oGroup.ID, oGrp = oGroup);
                                                        oBOD.Groups[oGrp.GroupName.ToLower()] = oGrp;
                                                    }
                                                }
                                                if (oScript != null)
                                                {
                                                    XIDScript oSrpt;
                                                    if (!lookupScripts.TryGetValue(oScript.ID, out oSrpt))
                                                    {
                                                        lookupScripts.Add(oScript.ID, oSrpt = oScript);
                                                        oBOD.Scripts[oSrpt.sName.ToLower()] = oSrpt;
                                                    }
                                                }
                                                if (oBOUI != null)
                                                {
                                                    oBOD.BOUID = oBOUI;
                                                }
                                                if (oStructure != null)
                                                {
                                                    XIDStructure oStrct;
                                                    if (!lookupStructs.TryGetValue(oStructure.ID, out oStrct))
                                                    {
                                                        lookupStructs.Add(oStructure.ID, oStrct = oStructure);
                                                        oBOD.Structures[oStructure.sStructureName.ToLower()] = oStrct;
                                                    }
                                                }

                                            }
                                            return oBOD;
                                        },
                                        param
                                        ).AsQueryable();
                }
                oBOD = lookupBOs.Values.FirstOrDefault();
                if (oBOD.Scripts.Count() > 0)
                {
                    Dictionary<string, object> ListAttributeScriptResults = new Dictionary<string, object>();
                    foreach (var item in oBOD.Scripts)
                    {
                        var ScriptResult = new List<XIDScriptResult>();
                        ListAttributeScriptResults["FKiScriptID"] = item.Value.ID;
                        //var resultforScrResult = Connection.Select<XIDScriptResult>("XIBOScriptResult_T", ListAttributeScriptResults).ToList();
                        ScriptResult.AddRange(Connection.Select<XIDScriptResult>("XIBOScriptResult_T", ListAttributeScriptResults).OrderBy(x => x.ID).ToList());
                        if (ScriptResult != null && ScriptResult.Count() > 0)
                            oBOD.Scripts[item.Key].ScriptResults.AddRange(ScriptResult);
                    }
                }
                return oBOD;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string GetBODataSource(string iDataSourceID, int iApplicationID, string sType = "")
        {
            cConnectionString oConString = new cConnectionString();
            int DataSourceID = 0;
            Guid DataSourceXIGUID = Guid.Empty;
            int.TryParse(iDataSourceID, out DataSourceID);
            Guid.TryParse(iDataSourceID, out DataSourceXIGUID);
            var sBODataSource = string.Empty;
            if (DataSourceID != 0 || (DataSourceXIGUID != null && DataSourceXIGUID != Guid.Empty))
            {
                if (DataSourceID == -1)
                {
                    sBODataSource = oConString.ConnectionString(sCoreDatabase);
                }
                else if (DataSourceID == -2)
                {
                    sBODataSource = oConString.ConnectionString(sOrgDatabase);
                }
                else
                {
                    XIInfraCache oCache = new XIInfraCache();
                    var SrcID = iDataSourceID.ToString();
                    XIInfraEncryption oEncrypt = new XIInfraEncryption();
                    if (!string.IsNullOrEmpty(sType))
                    {
                        SrcID = sType + "-" + SrcID;
                    }
                    var DataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, SrcID.ToString(), null, null, iApplicationID); //Connection.Select<XIDataSource>("XIDataSource_XID_T", Params).FirstOrDefault();// XIContext.XIDataSources.Find(SrcID);
                    sBODataSource = oEncrypt.DecryptData(DataSource.sConnectionString, true, DataSource.XIGUID.ToString());
                    //sBODataSource = DataSource.sConnectionString;
                }
            }
            else
            {
                if (iOrgID > 0)
                {
                    sBODataSource = oConString.ConnectionString(sOrgDatabase);
                }
                else
                {
                    sBODataSource = oConString.ConnectionString(sCoreDatabase);
                }
            }
            return sBODataSource;
        }

        public CResult RunBOGUID(XIDBOGUID xiBOGUID)
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
                bool bIsSuccess = Connection.ExecuteSP(xiBOGUID.sTableName, xiBOGUID.bIsChangeFK);
                oCResult.oResult = bIsSuccess;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

        }


        public CResult Get_BOsDDL()
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
                //Dictionary<string, string> DDL = Connection.SelectDDL("name, tablename", "XIBO_T_N");
                //var BOsDDL = DDL.Select(m => new XIDropDown { text = m.Key, Expression = m.Value }).ToList();

                Dictionary<string, string> DDL = Connection.SelectDDL("ID, Name", "XIBO_T_N");
                var BOsDDL = DDL.Select(m => new XIDropDown { text = m.Value, Value = Convert.ToInt32(m.Key) }).OrderBy(x => x.ID).ToList();

                oCResult.oResult = BOsDDL;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

        }

        public CResult Get_DataSourcesDDL()
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
                Dictionary<string, string> DDL = Connection.SelectDDL("ID, sName", "XIBO_T");
                var DataSourceDDL = DDL.Select(m => new XIDropDown { Value = Convert.ToInt32(m.Key), text = m.Value }).OrderBy(x => x.ID).ToList();
                DataSourceDDL.Insert(0, new XIDropDown
                {
                    text = "Organisation DB",
                    Value = -2
                });
                DataSourceDDL.Insert(0, new XIDropDown
                {
                    text = "Application DB",
                    Value = -1
                });
                oCResult.oResult = DataSourceDDL;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

        }

        //Get the Attributes name related to BO
        public XIDBO CopyBOByID(int ID, string sDatabase)
        {
            XIDBO oBOD = new XIDBO();
            XIInfraCache oCache = new XIInfraCache();
            string sCol_Name = "";
            string Data_Type = "";
            string sMax_Length = "";
            string sIS_NULL = "";
            string Col_Details = "";
            var lCmptCol = new List<string>();
            var sColDetails = new List<string>();
            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, ID.ToString());
            var sBODataSource = GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
            var sNewCol_Details = "";
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                Con.Open();
                SqlCommand SqlCmd = new SqlCommand();
                SqlCmd.Connection = Con;
                SqlCmd.CommandText = "SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME= '" + oBOD.TableName + "'";
                SqlDataReader DReader = SqlCmd.ExecuteReader();
                while (DReader.Read())
                {
                    sCol_Name = DReader.IsDBNull(0) ? null : DReader.GetValue(0).ToString();
                    Data_Type = DReader.IsDBNull(1) ? null : DReader.GetValue(1).ToString();
                    sMax_Length = DReader.IsDBNull(2) ? null : DReader.GetValue(2).ToString();
                    sIS_NULL = DReader.IsDBNull(3) ? null : DReader.GetValue(3).ToString();

                    string sTwoLtrOfColumn = sCol_Name.Substring(0, 2);
                    string sNewDatatype = "";
                    string sNewColDetails = "";
                    if (sTwoLtrOfColumn == "CM")
                    {
                        string sGetCmptdColDef = "SELECT definition FROM sys.computed_columns WHERE [name] = '" + sCol_Name + "' AND [object_id] = OBJECT_ID('" + oBOD.TableName + "')";
                        //Conn.Open();
                        SqlCommand SqlcmdColDef = new SqlCommand(sGetCmptdColDef, Con);
                        SqlDataReader DReaderChckDtype = SqlcmdColDef.ExecuteReader();
                        var sDefinition = "";
                        while (DReaderChckDtype.Read())
                        {
                            sDefinition = DReaderChckDtype.IsDBNull(0) ? null : DReaderChckDtype.GetValue(0).ToString();
                        }
                        //Conn.Close();
                        int iDefnLength = sDefinition.Length;
                        var sNewDefinition = sDefinition.Substring(1, iDefnLength - 2);
                        string sGetColumnUsed = String.Join(",", Regex.Matches(sNewDefinition, @"\[(.+?)\]").Cast<Match>().Select(m => m.Groups[1].Value));
                        var lColumnList = new List<string>();
                        var lColumnExists = new List<string>();
                        lColumnList = sGetColumnUsed.Split(',').ToList();
                        string sColumns = "";
                        foreach (var ColNmes in lColumnList)
                        {
                            string sCheckColmn = @"IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG='" + sDatabase + "' AND TABLE_NAME='" + oBOD.TableName + "' AND COLUMN_NAME ='" + ColNmes + "') SELECT 0 ELSE SELECT 1";
                            // Conn.Open();
                            SqlCommand Sqlcmd = new SqlCommand(sCheckColmn, Con);
                            int iColExist = Convert.ToInt32(Sqlcmd.ExecuteScalar());
                            // Conn.Close();
                            if (iColExist == 1)
                            {
                                lColumnExists.Add(ColNmes);
                            }
                            sColumns = string.Join(",", lColumnExists.ToArray());
                        }
                        //Col_Details =Col_Details+ sCol_Name+",{"+ sColumns + "} AS " + sNewDefinition + "\n";
                        //take as a sperate string.
                        lCmptCol.Add(sCol_Name + ",{" + sColumns + "} AS " + sNewDefinition + "\n");
                    }
                    else if (sTwoLtrOfColumn == "FK")
                    {
                        var PK_Table = "";
                        var PK_ColName = "";
                        //get the details on foreign key here add extra PKcolumn name and table name 
                        //SqlConnection TempConn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);

                        SqlCommand SqlCmdTemp = new SqlCommand();
                        SqlCmdTemp.Connection = Con;
                        SqlCmdTemp.CommandText = "SELECT FK_Table = FK.TABLE_NAME,FK_Column = CU.COLUMN_NAME, PK_Table = PK.TABLE_NAME, PK_Column = PT.COLUMN_NAME, Constraint_Name = C.CONSTRAINT_NAME FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME INNER JOIN (SELECT i1.TABLE_NAME, i2.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1 INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY') PT ON PT.TABLE_NAME = PK.TABLE_NAME WHERE FK.TABLE_NAME= '" + oBOD.TableName + "' AND CU.COLUMN_Name = '" + sCol_Name + "'";
                        SqlDataReader DReaderTemp = SqlCmdTemp.ExecuteReader();
                        while (DReaderTemp.Read())
                        {
                            PK_Table = DReaderTemp.IsDBNull(2) ? null : DReaderTemp.GetValue(2).ToString();
                            PK_ColName = DReaderTemp.IsDBNull(3) ? null : DReaderTemp.GetValue(3).ToString();
                        }
                        string NewNullVal = "NOT NULL";
                        if (sIS_NULL == "NO" || sIS_NULL == "no")
                        {
                            NewNullVal = "NOT NULL";
                        }
                        else
                        {
                            NewNullVal = "NULL";
                        }
                        if (PK_Table == "" && PK_ColName == "")
                        {
                            if (sMax_Length == "NULL")
                            {
                                sNewColDetails = sCol_Name + "," + Data_Type + "," + NewNullVal;
                            }
                            else
                            {
                                sNewColDetails = sCol_Name + "," + Data_Type + "(" + sMax_Length + ")," + NewNullVal;
                            }
                        }
                        else
                        {
                            if (sMax_Length == "NULL")
                            {
                                // sNewColDetails = sCol_Name + "," + Data_Type + "," + NewNullVal + "," + PK_Table + "," + PK_ColName;
                                sNewColDetails = sCol_Name + "," + PK_Table + "," + PK_ColName;
                            }
                            else
                            {
                                //sNewColDetails = sCol_Name + "," + Data_Type + "(" + sMax_Length + ")," + NewNullVal + "," + PK_Table + "," + PK_ColName;
                                sNewColDetails = sCol_Name + "," + PK_Table + "," + PK_ColName;
                            }
                        }
                        sColDetails.Add(sNewColDetails);
                        if (Col_Details == "")
                        {
                            Col_Details = sNewColDetails + "\n";
                        }
                        else
                        {
                            Col_Details = Col_Details + sNewColDetails + "\n";
                        }
                    }
                    else
                    {
                        if (Data_Type == "int")
                        {
                            sNewDatatype = "i";

                        }
                        else if (Data_Type == "varchar")
                        {
                            sNewDatatype = "s";
                        }
                        else if (Data_Type == "nvarchar")
                        {
                            sNewDatatype = "n";
                        }
                        else if (Data_Type == "datetime")
                        {
                            sNewDatatype = "d";
                        }
                        else if (Data_Type == "float")
                        {
                            sNewDatatype = "r";
                        }

                        //string NewMaxLen = "0";
                        //if(sMax_Length=="NULL"&&sMax_Length=="null")
                        //{
                        //    NewMaxLen = "0";
                        //}
                        //else
                        //{
                        //    NewMaxLen = sMax_Length;
                        //}

                        //Is Null
                        string NewNullVal = "NOT NULL";
                        if (sIS_NULL == "NO" || sIS_NULL == "no")
                        {
                            NewNullVal = "NOT NULL";
                        }
                        else
                        {
                            NewNullVal = "NULL";
                        }
                        string sStartCharacter = sCol_Name.Substring(0, 1);
                        if (sCol_Name == "ID")
                        {
                            //sNewColDetails = sCol_Name + "," + Data_Type + "," + NewNullVal;
                            sNewColDetails = sCol_Name;
                        }
                        else if (sStartCharacter == sNewDatatype)
                        {
                            if (sMax_Length == "NULL")
                            {
                                //sNewColDetails = sCol_Name + "," + Data_Type + "," + NewNullVal;
                                sNewColDetails = sCol_Name;
                            }
                            else
                            {
                                //sNewColDetails = sCol_Name + "," + Data_Type + "(" + sMax_Length + ")," + NewNullVal;
                                sNewColDetails = sCol_Name;
                            }
                        }
                        else
                        {
                            //consider the column name directly
                            if (sMax_Length == "NULL")
                            {
                                sNewColDetails = sCol_Name + "," + Data_Type + "," + NewNullVal;
                            }
                            else
                            {
                                sNewColDetails = sCol_Name + "," + Data_Type + "(" + sMax_Length + ")," + NewNullVal;
                            }
                            //sNewColDetails = sNewDatatype + sCol_Name + "_" + Data_Type + "_" + NewMaxLen + "_" + NewNullVal;
                        }
                        //  sNewColDetails = sNewDatatype + sCol_Name + "_" + Data_Type + "_" + sIS_NULL;
                        sColDetails.Add(sNewColDetails);
                        if (Col_Details == "")
                        {
                            Col_Details = sNewColDetails + "\n";
                        }
                        else
                        {
                            Col_Details = Col_Details + sNewColDetails + "\n";
                        }
                    }
                }
                var sRemoveNewLineAtEnd = Col_Details.TrimEnd('\n');
                var sRemoveEmptyParenthesis = sRemoveNewLineAtEnd.Replace("()", "");

                string sNewCmpCol = string.Join(",", lCmptCol.ToArray());
                string sCmptdCols = sNewCmpCol.Replace("\n,", "\n");
                string sFnlCmptdCol = sCmptdCols.TrimEnd('\n');
                if (sFnlCmptdCol != "")
                {
                    sNewCol_Details = sRemoveEmptyParenthesis + "\n" + sFnlCmptdCol;
                }
                else
                {
                    sNewCol_Details = sRemoveEmptyParenthesis;
                }

                Con.Close();
            }
            var NewBO = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, ID.ToString());
            NewBO.sColumns = sNewCol_Details;
            return NewBO;
        }

        #endregion BOMethods
        // Applications
        // Master

        // XI
        // - Applications
        // - XYZ application
        // - BOs
        // - XYZ BO   
        // - Attributes   
        // - XYZ Attribute
        // - Groups   
        // - Scripts 
        // - Class Attributes
        // - Structure
        // - Actions
        // - Security
        // - BO UI [1:1]
        // - Links
        // - XYZ XILink
        // - LinkGroups   
        // - Params
        // - QuestionSets
        // - Step
        // - Section
        // - Script
        // - Outcome
        // - Navigate
        // - Menus
        // - Visualisers
        // - Components
        // - 1-Clicks (application)
        // - Layouts
        // - BOs
        // - Library
        // - BO 
        // - Attr
        // - Script
        // - QS
        // - Steps
        // - Organisations
        // - 1-Click Searches (Org)
        // - Datasources
        // - 1-Click Searches (Application)


        public CResult Get_OrgDefinition(string sOrgName = "", string sUID = "")
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
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }

                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sOrgName))
                {
                    PKColumn = "Name";
                    PKValue = sOrgName;
                }
                XIDOrganisation oOrg = null;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sOrgDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oOrg = Connection.Select<XIDOrganisation>("Organizations", Params).FirstOrDefault();
                oCResult.oResult = oOrg;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Organisation definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public CResult Get_LayoutDefinition(string LayoutName = "", string sUID = "")
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


            try
            {
                XIUtility.IDInsteadOfGUID(sUID, "Layout");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0" && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(LayoutName))
                {
                    PKColumn = "LayoutName";
                    PKValue = LayoutName;
                }
                XIDLayout oLayout = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oLayout = Connection.Select<XIDLayout>("XILayout_T", Params).FirstOrDefault();
                if (oLayout != null)
                {
                    Dictionary<string, object> DParams = new Dictionary<string, object>();
                    if (oLayout.XIGUID != null && oLayout.XIGUID != Guid.Empty)
                    {
                        DParams["LayoutIDXIGUID"] = oLayout.XIGUID;
                    }
                    else if (oLayout.ID > 0)
                    {
                        DParams["LayoutID"] = oLayout.ID;
                    }
                    oLayout.LayoutDetails = Connection.Select<XIDLayoutDetails>("XILayoutDetail_T", DParams).OrderBy(x => x.PlaceHolderID).ToList();
                    if (oLayout.LayoutDetails != null && oLayout.LayoutDetails.Count() > 0)
                    {
                        oLayout.LayoutMappings = new List<XIDLayoutMapping>();
                        foreach (var detail in oLayout.LayoutDetails)
                        {
                            Dictionary<string, object> MapParams = new Dictionary<string, object>();
                            if (oLayout.XIGUID != null && oLayout.XIGUID != Guid.Empty)
                            {
                                MapParams["PopupLayoutIDXIGUID"] = oLayout.XIGUID;
                                MapParams["PlaceHolderIDXIGUID"] = detail.XIGUID;
                            }
                            else if (oLayout.ID > 0)
                            {
                                MapParams["PopupLayoutID"] = oLayout.ID;
                                MapParams["PlaceHolderID"] = detail.PlaceHolderID;
                            }
                            MapParams["StatusTypeID"] = "10";
                            var Mapping = Connection.Select<XIDLayoutMapping>("XILayoutMapping_T", MapParams).FirstOrDefault();
                            if (Mapping != null && (Mapping.ID > 0 || (Mapping.XIGUID != null && Mapping.XIGUID != Guid.Empty)))
                            {
                                oLayout.LayoutMappings.Add(Mapping);
                            }
                        }
                    }
                }
                if (oLayout != null)
                {
                    //Dictionary<string, object> UserParams = new Dictionary<string, object>();
                    if (oLayout.iThemeIDXIGUID != null && oLayout.iThemeIDXIGUID != Guid.Empty)
                    {
                        Dictionary<string, object> UserParams = new Dictionary<string, object>();
                        if (oLayout.iThemeIDXIGUID != null && oLayout.iThemeIDXIGUID != Guid.Empty)
                        {
                            UserParams["XIGUID"] = oLayout.iThemeIDXIGUID;
                        }
                        else if (oLayout.iThemeID > 0)
                        {
                            UserParams["ID"] = oLayout.iThemeID;
                        }
                        var Theme = Connection.Select<XIDMasterData>("XIMasterData_T", UserParams).FirstOrDefault();
                        if (Theme != null)
                        {
                            oLayout.sThemeName = Theme.FileName;
                        }
                    }
                    //Dictionary<string, object> MapParams = new Dictionary<string, object>();
                    //MapParams["PopupLayoutID"] = oLayout.ID;
                    //MapParams["StatusTypeID"] = "10";
                    //oLayout.LayoutMappings = Connection.Select<XIDLayoutMapping>("XILayoutMapping_T", MapParams).ToList();

                    if (oLayout != null && oLayout.LayoutType.ToLower() == "inline" || oLayout.LayoutType.ToLower() == "template")
                    {
                        oLayout.sGUID = Guid.NewGuid().ToString();
                    }
                    oCResult.oResult = oLayout;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oCResult.sMessage = "Layout definition not loaded for " + PKColumn + ":" + PKValue;
                    SaveErrortoDB(oCResult);
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Layout definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        //private string AddXiParametersToCache(int LayoutID, int XiParameterID, int ID, int BOID, string sNewGUID)
        //{
        //    XIParameter Parameter = new XIParameter();
        //    Dictionary<string, object> Params = new Dictionary<string, object>();
        //    Params["XiParameterID"] = XiParameterID;
        //    Parameter = Connection.Select<XIParameter>("XILayoutDetail_T", Params).FirstOrDefault();
        //    var sSessionID = HttpContext.Current.Session.SessionID;
        //    string sGUID = sNewGUID;
        //    if (!string.IsNullOrEmpty(sGUID))
        //    {
        //        //XICache oCache = new XICache();
        //        var sParentGUID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ParentGUID}");
        //        if (!string.IsNullOrEmpty(sParentGUID))
        //        {
        //            sGUID = sParentGUID;
        //        }
        //        else
        //        {
        //            sParentGUID = oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|ParentGUID}", sGUID.ToString(), null, null);
        //        }
        //    }

        //    //if (ID > 0)
        //    //{
        //    string sBOName = string.Empty;
        //    var oBO = Load_BO("", BOID);
        //    if (oBO != null)
        //    {
        //        sBOName = oBO.Name;
        //    }
        //    //CInstance oCache = Cacheobj.Get_XICache();

        //    var BOFields = oBO.Attributes.Values.Where(m => m.FKTableName == sBOName).ToList();
        //    if (BOFields != null && BOFields.Count() > 0)
        //    {
        //        var ActiveFK = BOFields.FirstOrDefault().Name;
        //        oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveFK}", ActiveFK.ToString(), null, null);
        //    }
        //    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}", sBOName.ToString(), null, null);
        //    if (ID > 0)
        //    {
        //        oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|" + sBOName + ".id}", ID.ToString(), null, null);
        //    }

        //    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|BODID}", BOID.ToString(), null, null);

        //    return sGUID;
        //}

        public CResult Get_ApplicationDefinition(string sApplicationName = "", string sUID = "")
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
                XIUtility.IDInsteadOfGUID(sUID, "Application");
                Guid APPXIGUID = Guid.Empty;
                Guid.TryParse(sUID, out APPXIGUID);
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                XIDApplication oAPP = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (APPXIGUID != null && APPXIGUID != Guid.Empty)
                {
                    Params["xiguid"] = APPXIGUID.ToString();
                }
                else if (iID > 0)
                {
                    Params["ID"] = iID;
                }
                else if (!string.IsNullOrEmpty(sApplicationName))
                {
                    Params["sApplicationName"] = sApplicationName;
                }
                if (iID > 0 || !string.IsNullOrEmpty(sApplicationName) || (APPXIGUID != null && APPXIGUID != Guid.Empty))
                {
                    oAPP = Connection.Select<XIDApplication>("XIApplication_T", Params).FirstOrDefault();
                    oCResult.oResult = oAPP;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                }

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Application definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_ApplicationOTPSettingDefinition(string sApplicationName = "", string sUID = "")
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
                XIUtility.IDInsteadOfGUID(sUID, "ApplicationOTPSetting");
                Guid APPXIGUID = Guid.Empty;
                Guid.TryParse(sUID, out APPXIGUID);
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                XIDApplicationOTPSetting oAPPSetting = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (APPXIGUID != null && APPXIGUID != Guid.Empty)
                {
                    Params["FKiApplicationIDXIGUID"] = APPXIGUID.ToString();
                }
                else if (iID > 0)
                {
                    Params["FKiApplicationID"] = iID;
                }
                else if (!string.IsNullOrEmpty(sApplicationName))
                {
                    Params["sApplicationName"] = sApplicationName;
                }
                if (iID > 0 || !string.IsNullOrEmpty(sApplicationName) || (APPXIGUID != null && APPXIGUID != Guid.Empty))
                {
                    oAPPSetting = Connection.Select<XIDApplicationOTPSetting>("XIApplicationSetting_T", Params).FirstOrDefault();
                    oCResult.oResult = oAPPSetting;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                }

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Application otp setting definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_URLDefinition(string sUrlName = "")
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
                XIURLMappings oURL = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["sUrlName"] = sUrlName;
                oURL = Connection.Select<XIURLMappings>("XIUrlMappings_T", Params).FirstOrDefault();
                oCResult.oResult = oURL;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading URL definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_XILinkDefinition(string sUID = "", string sName = "")
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
                XIUtility.IDInsteadOfGUID(sUID, "XILink");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "XiLinkID";
                    PKValue = iID.ToString();
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "Name";
                    PKValue = sName;
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                XILink oXiLink = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oXiLink = Connection.Select<XILink>("XILink_T", Params).FirstOrDefault();
                if (oXiLink != null)
                {
                    Dictionary<string, object> NVParams = new Dictionary<string, object>();
                    NVParams["XiLinkIDXIGUID"] = oXiLink.XIGUID;
                    NVParams["XIDeleted"] = "0";
                    oXiLink.XiLinkNVs = Connection.Select<XiLinkNV>("XILinkNV_T", NVParams).OrderBy(x => x.ID).ToList();
                    Dictionary<string, object> ListParams = new Dictionary<string, object>();
                    ListParams["XiLinkIDXIGUID"] = oXiLink.XIGUID;
                    oXiLink.XiLinkLists = Connection.Select<XiLinkList>("XILinkList_T", ListParams).OrderBy(x => x.XiLinkListID).ToList();
                }
                oCResult.oResult = oXiLink;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading XILink definition ID: " + sUID + " Name: " + sName });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_1ClickDefinition(string sName = "", string sUID = "", string sStructureCode = "")
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
                XIUtility.IDInsteadOfGUID(sUID, "1Click");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (!String.IsNullOrEmpty(sName))
                {
                    PKColumn = "Name";
                    PKValue = sName;
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                XID1Click o1Click = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                o1Click = Connection.Select<XID1Click>("XI1Click_T", Params).FirstOrDefault();
                if (o1Click != null)
                {
                    if (o1Click.ID > 0 || (o1Click.XIGUID != null && o1Click.XIGUID != Guid.Empty))
                    {
                        List<XID1ClickLink> oLinks = null;
                        Dictionary<string, object> LinkParams = new Dictionary<string, object>();
                        if (o1Click.XIGUID != null && o1Click.XIGUID != Guid.Empty)
                        {
                            LinkParams["fki1clickidxiguid"] = o1Click.XIGUID;
                        }
                        else
                        {
                            LinkParams["fki1clickid"] = o1Click.ID;
                        }
                        LinkParams["iStatus"] = 10;
                        oLinks = Connection.Select<XID1ClickLink>("XI1ClickLink_T", LinkParams).ToList();
                        o1Click.MyLinks = oLinks;
                        List<XID1ClickParameter> oParameter = null;
                        Dictionary<string, object> oClickParameters = new Dictionary<string, object>();
                        if (o1Click.XIGUID != null && o1Click.XIGUID != Guid.Empty)
                        {
                            oClickParameters["fki1clickidxiguid"] = o1Click.XIGUID;
                        }
                        else
                        {
                            oClickParameters["fki1clickid"] = o1Click.ID;
                        }
                        oParameter = Connection.Select<XID1ClickParameter>("XI1ClickParameter_T", oClickParameters).ToList();
                        o1Click.oOneClickParameters = oParameter;
                        if (o1Click.FKiComponentID > 0 || (o1Click.FKiComponentIDXIGUID != null && o1Click.FKiComponentIDXIGUID != Guid.Empty))
                        {
                            XIInfraCache oCache = new XIInfraCache();
                            XIDComponent oXICompD = new XIDComponent();
                            oXICompD = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, null, o1Click.FKiComponentIDXIGUID.ToString());
                            var oXICompC = (XIDComponent)oXICompD.Clone(oXICompD);
                            oXICompC.GetParamsByContext(oXICompC, "OneClick", o1Click.XIGUID.ToString());
                            o1Click.XIComponent = oXICompC;
                        }
                        //List<XI1ClickSummary> oSummary = null;
                        //Dictionary<string, object> oClickSummary = new Dictionary<string, object>();
                        //oClickSummary["FKi1ClickID"] = o1Click.ID;
                        //oSummary = Connection.Select<XI1ClickSummary>("XI1ClickSummary_T", oClickSummary).ToList();
                        //o1Click.XI1ClickSummary = oSummary;
                        List<XID1ClickPermission> oPermission = null;
                        Dictionary<string, object> o1ClickPermission = new Dictionary<string, object>();
                        if (o1Click.XIGUID != null && o1Click.XIGUID != Guid.Empty)
                        {
                            o1ClickPermission["FKi1ClickIDXIGUID"] = o1Click.XIGUID;
                        }
                        else
                        {
                            o1ClickPermission["fki1clickid"] = o1Click.ID;
                        }
                        oPermission = Connection.Select<XID1ClickPermission>("XI1ClickPermission_T", o1ClickPermission).ToList();
                        o1Click.RoleIDs = oPermission.Select(x => x.FKiRoleID).ToList();
                        if (o1Click.BOID > 0 || (o1Click.BOIDXIGUID != null && o1Click.BOIDXIGUID != Guid.Empty))
                        {
                            string sSelectFields = string.Empty;
                            sSelectFields = "Name,TableName";
                            Dictionary<string, object> BOParams = new Dictionary<string, object>();
                            BOParams["XIGUID"] = o1Click.BOIDXIGUID;
                            var FKBOD = Connection.Select<XIDBO>("XIBO_T_N", BOParams, sSelectFields).FirstOrDefault();
                            if (FKBOD != null)
                            {
                                o1Click.sBOName = FKBOD.Name;
                            }
                        }
                        List<XID1Click> sub1Clicks = null;
                        Dictionary<string, object> oSubParams = new Dictionary<string, object>();
                        oSubParams["ParentIDXIGUID"] = o1Click.XIGUID;
                        sub1Clicks = Connection.Select<XID1Click>("XI1Click_T", oSubParams).ToList();
                        o1Click.Sub1Clicks = sub1Clicks;
                        if (!string.IsNullOrEmpty(o1Click.FKi1ClickScriptID))
                        {
                            List<XID1ClickScripts> oScripts = null;
                            Dictionary<string, object> oScriptParams = new Dictionary<string, object>();
                            if (o1Click.XIGUID != null && o1Click.XIGUID != Guid.Empty)
                            {
                                oScriptParams["FKiOneClickIDXIGUID"] = o1Click.XIGUID;
                            }
                            else
                            {
                                oScriptParams["FKiOneClickID"] = o1Click.ID;
                            }
                            oScripts = Connection.Select<XID1ClickScripts>("XI1ClickScripts_T", oScriptParams).ToList();
                            o1Click.oScripts = oScripts;
                        }
                        List<XID1ClickAction> oAction = null;
                        oClickParameters = new Dictionary<string, object>();
                        if (o1Click.XIGUID != null && o1Click.XIGUID != Guid.Empty)
                        {
                            oClickParameters["fki1clickidxiguid"] = o1Click.XIGUID;
                        }
                        else
                        {
                            oClickParameters["fki1clickid"] = o1Click.ID;
                        }
                        oClickParameters[XIConstant.Key_XIDeleted] = "0";
                        oAction = Connection.Select<XID1ClickAction>("XI1ClickAction_T", oClickParameters).ToList();
                        o1Click.Actions = oAction;
                        //Formats
                        List<XIDFormatMapping> oFormats = null;
                        oClickParameters = new Dictionary<string, object>();
                        if (o1Click.XIGUID != null && o1Click.XIGUID != Guid.Empty)
                        {
                            oClickParameters["fki1clickidxiguid"] = o1Click.XIGUID;
                        }
                        else
                        {
                            oClickParameters["fki1clickid"] = o1Click.ID;
                        }
                        oClickParameters[XIConstant.Key_XIDeleted] = "0";
                        oFormats = Connection.Select<XIDFormatMapping>("XIFormatMapping_T", oClickParameters).ToList();
                        o1Click.Formats = oFormats;
                        //Attribute Details Over ride
                        List<XI1QueryAttributeDetails> Override = null;
                        oClickParameters = new Dictionary<string, object>();
                        if (o1Click.XIGUID != null && o1Click.XIGUID != Guid.Empty)
                        {
                            oClickParameters["FKiOneClickIDXIGUID"] = o1Click.XIGUID;
                        }
                        oClickParameters[XIConstant.Key_XIDeleted] = "0";
                        Override = Connection.Select<XI1QueryAttributeDetails>("XI1QueryAttributeDetails_T", oClickParameters).ToList();
                        o1Click.OverrideAttributeDetails = Override;
                    }
                }
                oCResult.oResult = o1Click;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading 1-Click definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_DialogDefinition(string sUID = "")
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
                XIUtility.IDInsteadOfGUID(sUID, "Dialog");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID))
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                XIDDialog oDialog = new XIDDialog();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                if (PKValue != "0")
                {
                    oDialog = Connection.Select<XIDDialog>("XIDialog_T", Params).FirstOrDefault();
                }
                //List<XIDLayout> oXIDLayout = null;
                //Dictionary<string, object> Params1 = new Dictionary<string, object>();
                ////Params1["LayoutType"] = "Dialog";
                //oXIDLayout = Connection.Select<XIDLayout>("XILayout_T", Params1).ToList();
                //var oXIDLayoutDDL = oXIDLayout.Select(m => new XIDropDown { Value = Convert.ToInt32(m.ID), text = m.LayoutName }).ToList();
                //oXIDLayoutDDL.Insert(0, new XIDropDown
                //{
                //    text = "--Select--",
                //    Value = 0
                //});
                //oDialog.Layouts = oXIDLayoutDDL;
                if (iID == 0)
                {
                    oDialog.FKiApplicationID = FKiApplicationID;
                }
                oCResult.oResult = oDialog;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Dialog definition : " + sUID });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_PopupDefinition(string sUID = "")
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
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID))
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                XIDPopup oPopup = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oPopup = Connection.Select<XIDPopup>("XIPopup_T", Params).FirstOrDefault();
                oCResult.oResult = oPopup;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Dialog definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_DataSourceDefinition(string sName, string sUID = "", int iApplicationID = 0)
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
                XIUtility.IDInsteadOfGUID(sUID, "Data Source");
                string sType = string.Empty;
                if (sUID.Contains('_'))
                {
                    var Split = sUID.Split('_').ToList();
                    sUID = Split[1];
                    sType = Split[0];
                }
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "sName";
                    PKValue = sName;
                }
                XIDataSource oDataSourceD = new XIDataSource();
                if (!string.IsNullOrEmpty(PKColumn) && !string.IsNullOrEmpty(PKValue))
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params[PKColumn] = PKValue;
                    //string sApplication = ConfigurationManager.AppSettings["AppName"];

                    if (iApplicationID > 0 && iApplicationID == 15 && string.IsNullOrEmpty(sType))
                    {
                        oDataSourceD = Connection.Select<XIDataSource>("XIDataSource_XID_T", Params).FirstOrDefault();
                    }
                    else
                    {
                        oDataSourceD = XIEnvConnection.Select<XIDataSource>("XIDataSource_XID_T", Params).FirstOrDefault();
                    }
                    if (sTypeCyption == "Decrypt")
                    {
                        XIEncryption oXIAPI = new XIEncryption();
                        oDataSourceD.sConnectionString = oXIAPI.DecryptData(oDataSourceD.sConnectionString, true, oDataSourceD.XIGUID.ToString());
                    }
                }
                if (string.IsNullOrEmpty(PKColumn) && string.IsNullOrEmpty(PKValue))
                {
                    oDataSourceD.FKiApplicationID = FKiApplicationID;
                }
                oCResult.oResult = oDataSourceD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Data Source definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_XIParameterDefinition(string sName, string sUID = "")
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
                XIUtility.IDInsteadOfGUID(sUID, "Parameter");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "XiParameterID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "name";
                    PKValue = sName;
                }
                XIParameter oXiParam = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oXiParam = Connection.Select<XIParameter>("XIParameters", Params).FirstOrDefault();
                if (oXiParam != null)
                {
                    Dictionary<string, object> NVParams = new Dictionary<string, object>();
                    if (PKColumn == "xiguid")
                    {
                        NVParams["XiParameterIDXIGUID"] = oXiParam.XIGUID;
                    }
                    else
                    {
                        NVParams["XiParameterID"] = oXiParam.XiParameterID;
                    }
                    NVParams["xideleted"] = "0";
                    oXiParam.XiParameterNVs = Connection.Select<XiParameterNVs>("XIParameterNVs", NVParams).OrderBy(x => x.ID).ToList();
                }
                if (oXiParam != null)
                {
                    Dictionary<string, object> ListParams = new Dictionary<string, object>();
                    if (PKColumn == "xiguid")
                    {
                        ListParams["XiParameterIDXIGUID"] = oXiParam.XIGUID;
                    }
                    else
                    {
                        ListParams["XiParameterID"] = oXiParam.XiParameterID;
                    }
                    oXiParam.XiParameterLists = Connection.Select<XiParameterLists>("XiParameterLists", ListParams).OrderBy(x => x.XiParameterListID).ToList();
                }
                if (oXiParam != null && oXiParam.XiParameterID == 0)
                {
                    oXiParam.FKiApplicationID = FKiApplicationID;
                }
                oCResult.oResult = oXiParam;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading XILink definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_VisualisationDefinition(string sUID = "", string sName = "")
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
                XIUtility.IDInsteadOfGUID(sUID, "Visualisation");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                Guid VisualisationGUID;
                if (Guid.TryParse(sUID, out VisualisationGUID))
                {
                }
                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "XiVisualID";
                    PKValue = iID.ToString();
                }
                else if (!String.IsNullOrEmpty(sName))
                {
                    PKColumn = "Name";
                    PKValue = sName;
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && VisualisationGUID != Guid.Empty)
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                XIVisualisation oXivisualisation = null;
                if (!string.IsNullOrEmpty(PKColumn) && !string.IsNullOrEmpty(PKValue))
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params[PKColumn] = PKValue;
                    oXivisualisation = Connection.Select<XIVisualisation>("XiVisualisations", Params).FirstOrDefault();
                    if (oXivisualisation != null)
                    {
                        Dictionary<string, object> NVParams = new Dictionary<string, object>();
                        //NVParams["XiVisualID"] = oXivisualisation.XiVisualID;
                        NVParams["XiVisualIDXIGUID"] = oXivisualisation.XIGUID;
                        oXivisualisation.XiVisualisationNVs = Connection.Select<XIVisualisationNV>("XiVisualisationNVs", NVParams).OrderBy(x => x.ID).ToList();
                    }
                    if (oXivisualisation != null)
                    {
                        Dictionary<string, object> ListParams = new Dictionary<string, object>();
                        //ListParams["XiVisualID"] = oXivisualisation.XiVisualID;
                        ListParams["XiVisualIDXIGUID"] = oXivisualisation.XIGUID;
                        oXivisualisation.XiVisualisationLists = Connection.Select<XIVisualisationList>("XiVisualisationLists", ListParams).OrderBy(x => x.XiVisualListID).ToList();
                    }
                }
                oCResult.oResult = oXivisualisation;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading XILink definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_StructureDefinition(string sUID = "")
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
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started ContentDefinition Loading" });
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
                XIUtility.IDInsteadOfGUID(sUID, "Structure");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                Guid XIGUID = Guid.Empty;
                Guid.TryParse(sUID, out XIGUID);
                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (XIGUID != null && XIGUID != Guid.Empty)
                {
                    PKColumn = "XIGUID";
                    PKValue = XIGUID.ToString();
                }
                else if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                XIDStructure oStructD = new XIDStructure();
                if (PKValue != "0" && PKValue != "")
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params[PKColumn] = PKValue;
                    oStructD = Connection.Select<XIDStructure>("XIBOStructure_T", Params).FirstOrDefault();
                }
                //string PKoColumn = string.Empty;
                //string PKoValue = string.Empty;
                //if (iID > 0)
                //{
                //    PKoColumn = "FKiStructureID";
                //    PKoValue = iID.ToString();
                //}
                //if (PKoValue != "0" && PKoValue != "")
                //{
                //    Dictionary<string, object> oParams = new Dictionary<string, object>();
                //    oParams[PKoColumn] = PKoValue;
                //    oStructD.oStructureDetails = Connection.Select<XIDStructureDetail>("XIBOStructureDetail_T", oParams).FirstOrDefault();
                //}
                //All BO DropDowns
                List<XIDropDown> oBOD = new List<XIDropDown>();
                Dictionary<string, object> oQSParam = new Dictionary<string, object>();
                //oQSParam["FKiApplicationID"] = FKiAppID;
                //oQSParam["FKiOrgID"] = iOrgID;
                var oQSDef = Connection.Select<XIDBO>("XIBO_T_N", oQSParam).ToList();
                oBOD = oQSDef.Select(m => new XIDropDown { Value = m.BOID, text = m.Name }).OrderBy(x => x.ID).ToList();
                oStructD.BOList = oBOD;

                Dictionary<string, object> Params5 = new Dictionary<string, object>();
                Dictionary<string, string> QSSteps = new Dictionary<string, string>();
                var oAllBOs = Connection.Select<XIDQSStep>("XIQSStepDefinition_T", Params5).OrderBy(x => x.ID).ToList();
                foreach (var items in oAllBOs)
                {
                    if (items.sName != null)
                        QSSteps[items.sName] = items.ID.ToString();
                }
                oStructD.AllQSSteps = QSSteps;
                oCResult.oResult = oStructD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }
        public CResult Get_StructureNodes(string sName = "")
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
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started ContentDefinition Loading" });
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
                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "scode";
                    PKValue = sName;
                }
                List<XIDStructure> oStructD = new List<XIDStructure>();
                if (PKValue != "0" && PKValue != "")
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params[PKColumn] = PKValue;
                    oStructD = Connection.Select<XIDStructure>("XIBOStructure_T", Params).OrderBy(x => x.ID).ToList();
                }
                oCResult.oResult = oStructD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }
        public CResult Get_InboxDefinition(string sUID = "", string iInboxID = "")
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
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started ContentDefinition Loading" });
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
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "RoleID";
                    PKValue = iID.ToString();
                }
                else if (!string.IsNullOrEmpty(iInboxID))
                {
                    PKColumn = "id";
                    PKValue = iInboxID;
                }
                List<XIDInbox> oInboxD = new List<XIDInbox>();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oInboxD = Connection.Select<XIDInbox>("XIInbox_T", Params).OrderBy(x => x.ID).ToList();
                oCResult.oResult = oInboxD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public CResult Get_MenuNodeDefinition(string sUID = "", string sRootName = "")
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
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started ContentDefinition Loading" });
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
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                Guid XIGUID = Guid.Empty;
                Guid.TryParse(sUID, out XIGUID);
                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (XIGUID != null && XIGUID != Guid.Empty)
                {
                    PKColumn = "XIGUID";
                    PKValue = XIGUID.ToString();
                }
                else if (!string.IsNullOrEmpty(sRootName))
                {
                    PKColumn = "rootname";
                    PKValue = sRootName;
                }
                List<XIMenu> oMenuD = new List<XIMenu>();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oMenuD = Connection.Select<XIMenu>("XIMenu_T", Params).OrderBy(x => x.ID).ToList();
                oCResult.oResult = oMenuD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        #region QuestionSet 

        public CResult Get_QuestionSetDefinition(string sUID = "")
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
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started ContentDefinition Loading" });
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
                XIUtility.IDInsteadOfGUID(sUID, "QuestionSet");
                long iID = 0;
                Guid QSGUID = Guid.Empty;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                Guid.TryParse(sUID, out QSGUID);
                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (QSGUID != null && QSGUID != Guid.Empty)
                {
                    PKColumn = "xiguid";
                    PKValue = QSGUID.ToString();
                }
                XIDQS oQSD = new XIDQS();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oQSD = Connection.Select<XIDQS>("XIQSDefinition_T", Params).FirstOrDefault();
                oCResult.oResult = oQSD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public CResult Get_QSDefinition(string sQSName = "", string sUID = "", string sSessionID = null, string sGUID = null, int iOrgID = 0)
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
                XIDQS oDQS = new XIDQS();
                long iID;
                long.TryParse(sUID, out iID);
                Guid QSDXIGuid = Guid.Empty;
                Guid.TryParse(sUID, out QSDXIGuid);
                XIIBO oBOI = null/* TODO Change to default(_) if this is not a reference type */;

                XIUtility.IDInsteadOfGUID(sUID, "QuestionSet");
                // sLoadSteps - if "" then load all. otherwise for example:
                // Step1,Step3,Step9,Step10
                // load steps keyed by name (which must be unique and if it gets same name again just dont add in)
                // the steps are instances
                // KEY BY LOWER CASE ONLY

                // TO DO - get oQSD from the cache
                if (!string.IsNullOrEmpty(sQSName) || iID > 0 || (QSDXIGuid != null && QSDXIGuid != Guid.Empty))
                {
                    oDQS = GetQuestionSetDefinitionByID(sQSName, sUID, sSessionID, sGUID, iOrgID);
                    if (oDQS != null)
                    {
                        var oPartialQSD = GetPartialQS(oDQS.ID);
                        if (oPartialQSD != null && oPartialQSD.PartialQS != null && oPartialQSD.PartialQS.Count > 0)
                        {
                            foreach (var oPartialQs in oPartialQSD.PartialQS)
                            {
                                var oQSD = oPartialQs.Value.oQSD.FirstOrDefault().Value;
                                var CurrentStepOrder = oDQS.Steps.Where(m => m.Value.ID == oPartialQs.Value.FkiStepID).Select(m => m.Value.iOrder).FirstOrDefault();
                                if (oQSD.Steps != null && oQSD.Steps.Count > 0)
                                {
                                    foreach (var oPartialQSstep in oQSD.Steps)
                                    {
                                        CurrentStepOrder += oPartialQs.Value.iOrderGap;
                                        oPartialQSstep.Value.iOrder = CurrentStepOrder;
                                        oDQS.Steps[oPartialQSstep.Key] = oPartialQSstep.Value;
                                    }
                                }
                            }
                        }
                    }
                }

                if (oDQS != null)
                {
                    oDQS.BOI = oBOI;
                    oDQS.oParent = this;
                }
                oCResult.oResult = oDQS;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Question Set definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public XIDQS GetQuestionSetDefinitionByID(string sQSName, string sUID, string sSessionID = null, string sGUID = null, int iOrgID = 0)
        {
            XIInfraCache oCache = new XIInfraCache();
            string sQSDefinitionQry = "select * from XIQSDefinition_T QSDef " +
              "left join XIQSStepDefinition_T QSSDef on QSDef.ID = QSSDef.FKiQSDefintionID " +
              "left join XIFieldDefinition_T XIFD on QSSDef.ID = XIFD.FKiXIStepDefinitionID " +
              "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginID = XIO.ID " +
              "left join XIDataType_T XIDT on XIO.FKiDataType = XIDT.id " +
              "left join XIQSNavigation_T NAV on QSSDef.ID = NAV.FKiStepDefinitionID " +
              "left join XIFieldOptionList_T OPT on XIO.ID = OPT.FKiQSFieldID ";

            long iID = 0;
            if (long.TryParse(sUID, out iID))
            {
            }
            else
            {

            }

            if (!string.IsNullOrEmpty(sQSName))
            {
                sQSDefinitionQry = sQSDefinitionQry + "WHERE QSDef.sName = @Name";
            }
            else if (iID > 0)
            {
                sQSDefinitionQry = sQSDefinitionQry + "WHERE QSDef.ID = @id";
            }
            else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
            {
                sQSDefinitionQry = "select * from XIQSDefinition_T QSDef " +
             "left join XIQSStepDefinition_T QSSDef on QSDef.XIGUID = QSSDef.FKiQSDefintionIDXIGUID " +
             "left join XIFieldDefinition_T XIFD on QSSDef.XIGUID = XIFD.FKiXIStepDefinitionIDXIGUID " +
             "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginIDXIGUID = XIO.XIGUID " +
             "left join XIDataType_T XIDT on XIO.FKiDataType = XIDT.id " +
             "left join XIQSNavigation_T NAV on QSSDef.ID = NAV.FKiStepDefinitionID " +
             "left join XIFieldOptionList_T OPT on XIO.XIGUID = OPT.FKiQSFieldIDXIGUID "
               + "WHERE QSDef.xiguid = @xiguid";
            }
            if (iOrgID > 0)
            {
                //sQSDefinitionQry = sQSDefinitionQry + " and QSDef.FKiOrgID= @FKiOrgID";
            }
            sQSDefinitionQry = sQSDefinitionQry + ";";
            var param = new
            {
                id = iID,
                Name = sQSName,
                xiguid = sUID,
                //FKiOrgID = iOrgID
            };
            XIIXI oXI = new XIIXI();
            var lookup = new Dictionary<Guid, XIDQS>();
            List<Guid> OrgFieldOrigins = new List<Guid>();
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                Conn.Open();
                var lookup2 = new Dictionary<Guid, XIDQSStep>();
                var lookup3 = new Dictionary<Guid, XIDFieldDefinition>();
                var lookup4 = new Dictionary<Guid, XIDFieldOrigin>();
                var lookup5 = new Dictionary<Guid, XIDQSStepNavigations>();
                var lookup6 = new Dictionary<Guid, XIDFieldOptionList>();
                Conn.Query<XIDQS, XIDQSStep, XIDFieldDefinition, XIDFieldOrigin, XIDDataType, XIDQSStepNavigations, XIDFieldOptionList, XIDQS>(sQSDefinitionQry,
                    (QS, Step, FieldDefinition, FieldOrigin, DataType, Navigations, OptionList) =>
                    {
                        XIDQS oQSDefinition;
                        if (!lookup.TryGetValue(QS.XIGUID, out oQSDefinition))
                        {
                            lookup.Add(QS.XIGUID, oQSDefinition = QS);
                        }
                        XIDQSStep oStepDefinition;
                        if (Step != null && Step.StatusTypeID == (int)xiEnumSystem.xistatus.xiactive)
                        {
                            if (!lookup2.TryGetValue(Step.XIGUID, out oStepDefinition))
                            {
                                if (Step.FkiRuleID > 0 || (Step.FkiRuleIDXIGUID != null && Step.FkiRuleIDXIGUID != Guid.Empty))
                                {
                                    var oRuleI = new XIIBO();
                                    if (Step.FkiRuleIDXIGUID != null && Step.FkiRuleIDXIGUID != Guid.Empty)
                                    {
                                        oRuleI = oXI.BOI("XI Rules", Step.FkiRuleIDXIGUID.ToString());
                                    }
                                    else if (Step.FkiRuleID > 0)
                                    {
                                        oRuleI = oXI.BOI("XI Rules", Step.FkiRuleID.ToString());
                                    }
                                    string sBODataSource = string.Empty;
                                    if (oRuleI != null && oRuleI.Attributes.Count > 0 && oRuleI.Attributes.ContainsKey("sCondition"))
                                    {
                                        Step.sRuleCondition = oRuleI.Attributes["sCondition"].sValue;
                                    }
                                }
                                lookup2.Add(Step.XIGUID, oStepDefinition = Step);
                                oQSDefinition.Steps[oStepDefinition.sName] = oStepDefinition;
                            }
                            XIDFieldDefinition oFieldDefintion;
                            if (FieldDefinition != null && FieldDefinition.XIDeleted != 1 && FieldOrigin != null)
                            {
                                if (!lookup3.TryGetValue(FieldDefinition.XIGUID, out oFieldDefintion))
                                {
                                    lookup3.Add(FieldDefinition.XIGUID, oFieldDefintion = FieldDefinition);
                                    oStepDefinition.FieldDefs[FieldOrigin.sName] = oFieldDefintion;

                                }
                                XIDFieldOrigin oXIFieldOrigin;
                                if (FieldOrigin != null)
                                {

                                    if (!lookup4.TryGetValue(FieldOrigin.XIGUID, out oXIFieldOrigin))
                                    {
                                        if (FieldOrigin.bOrgFK)
                                        {
                                            OrgFieldOrigins.Add(FieldOrigin.XIGUID);
                                        }
                                        if (FieldOrigin.iMasterDataID > 0)
                                        {
                                            //FieldOrigin.ddlFieldOptionList = dbContext.Types.Where(m => m.Code == FieldOrigin.iMasterDataID).ToList().Select(m => new XIDFieldOptionList { sOptionName = m.Expression, sOptionValue = m.ID.ToString() }).ToList();
                                        }
                                        else if (FieldOrigin.FK1ClickID > 0 || (FieldOrigin.FK1ClickIDXIGUID != null && FieldOrigin.FK1ClickIDXIGUID != Guid.Empty))
                                        {
                                            var o1ClickD = new XID1Click();
                                            if (FieldOrigin.FK1ClickIDXIGUID != null && FieldOrigin.FK1ClickIDXIGUID != Guid.Empty)
                                            {
                                                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, FieldOrigin.FK1ClickIDXIGUID.ToString());
                                            }
                                            else if (FieldOrigin.FK1ClickID > 0)
                                            {
                                                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, FieldOrigin.FK1ClickID.ToString());
                                            }
                                            XIDBO oBOD = new XIDBO();
                                            if (o1ClickD.BOIDXIGUID != null && o1ClickD.BOIDXIGUID != Guid.Empty)
                                            {
                                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOIDXIGUID.ToString());
                                            }
                                            else if (o1ClickD.BOID > 0)
                                            {
                                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                                            }
                                            if (oBOD != null)
                                            {
                                                FieldOrigin.sBOSize = oBOD.sSize;
                                            }
                                        }
                                        else if (FieldOrigin.FKiBOID > 0 || (FieldOrigin.FKiBOIDXIGUID != null && FieldOrigin.FKiBOIDXIGUID != Guid.Empty))
                                        {
                                            var oBOD = new XIDBO();
                                            if (FieldOrigin.FKiBOIDXIGUID != null && FieldOrigin.FKiBOIDXIGUID != Guid.Empty)
                                            {
                                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, FieldOrigin.FKiBOIDXIGUID.ToString());
                                            }
                                            else if (FieldOrigin.FKiBOID > 0)
                                            {
                                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, FieldOrigin.FKiBOID.ToString());
                                            }
                                            if (oBOD != null)
                                            {
                                                FieldOrigin.sBOSize = oBOD.sSize;
                                                if (oBOD.sSize == "10")
                                                {
                                                    string sBODataSource = GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                                                    if (oBOD.Groups.ContainsKey("label"))
                                                    {
                                                        var oGroupD = oBOD.Groups["label"];
                                                        var Con = new XIDBAPI(sBODataSource);
                                                        var LabelGroup = oGroupD.BOFieldNames;
                                                        if (!string.IsNullOrEmpty(LabelGroup))
                                                        {
                                                            FieldOrigin.sBOSize = oBOD.sSize;
                                                            //string FinalString = oGroupD.ConcatanateFields(LabelGroup, " ");
                                                            //FinalString = oBOD.sPrimaryKey + "," + FinalString;
                                                            string FinalString = LabelGroup;
                                                            string sWhrClause = "";

                                                            if (oBOD.iOrgObject == 1)
                                                            {
                                                                sWhrClause = "[" + oBOD.TableName + "].FKiOrgID=" + iOrgID;
                                                                //AddSqlParameters("@FKiOrgID", oInfo.iOrganizationID);
                                                                //Query = AddSearchParameters(Query, SearchText);
                                                            }
                                                            Dictionary<string, string> DDL = Con.SelectDDL(FinalString, oBOD.TableName, sWhrClause);
                                                            var FKDDL = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                                                            FieldOrigin.FieldDynamicOptionList = new List<XIDFieldOptionList>();
                                                            FieldOrigin.FieldDynamicOptionList = FKDDL;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        lookup4.Add(FieldOrigin.XIGUID, oXIFieldOrigin = FieldOrigin);
                                        oXIFieldOrigin = FieldOrigin;
                                    }

                                    XIDFieldOptionList oOptions;
                                    if (OptionList != null)
                                    {
                                        oOptions = OptionList;
                                        if (!lookup6.TryGetValue(OptionList.XIGUID, out oOptions))
                                        {
                                            lookup6.Add(OptionList.XIGUID, oOptions = OptionList);
                                            if (oXIFieldOrigin.FieldOptionList != null && oXIFieldOrigin.FieldOptionList.Count() > 0)
                                            {
                                                if (oOptions.XIDeleted == 0)
                                                {
                                                    oXIFieldOrigin.FieldOptionList.Add(oOptions);
                                                }
                                            }
                                            else
                                            {
                                                oXIFieldOrigin.FieldOptionList = new List<XIDFieldOptionList>();
                                                oXIFieldOrigin.FieldOptionList.Add(oOptions);
                                            }
                                        }
                                    }
                                    oFieldDefintion.FieldOrigin = oXIFieldOrigin;
                                    FieldOrigin.DataType = DataType;
                                    //if (!oQSDefinition.XIDFieldOrigin.ContainsKey(oXIFieldOrigin.sName))
                                    //{
                                    //    if (FieldOrigin.DataType.sName.ToLower() == "date")
                                    //    {
                                    //        var oResult = SetDefaultDateRange(FieldOrigin.sMinDate, FieldOrigin.sMaxDate);
                                    //        if (oResult != null && oResult.Count() > 0)
                                    //        {
                                    //            if (oResult.ContainsKey("sMinDate"))
                                    //            {
                                    //                FieldOrigin.sMinDate = oResult["sMinDate"];
                                    //            }
                                    //            if (oResult.ContainsKey("sMaxDate"))
                                    //            {
                                    //                FieldOrigin.sMaxDate = oResult["sMaxDate"];
                                    //            }
                                    //        }
                                    //    }
                                    //    oQSDefinition.XIDFieldOrigin[oXIFieldOrigin.sName] = oXIFieldOrigin;
                                    //}
                                }


                                //if (oFieldInstance.FieldDefinitions != null && oFieldInstance.FieldDefinitions.Count() > 0)
                                //{
                                //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                                //}
                                //else
                                //{
                                //    oFieldInstance.FieldDefinitions = new List<cXIFieldDefinition>();
                                //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                                //}
                            }
                            if (Navigations != null)
                            {
                                XIDQSStepNavigations nNavs;
                                if (!lookup5.TryGetValue(Navigations.XIGUID, out nNavs))
                                {
                                    lookup5.Add(Navigations.XIGUID, nNavs = Navigations);
                                    if (!string.IsNullOrEmpty(nNavs.sName))
                                        oStepDefinition.Navigations[nNavs.sName] = nNavs;
                                }
                            }
                        }
                        return oQSDefinition;
                    },
                    param
                    ).AsQueryable();
            }
            var oQSDef = lookup.Values.FirstOrDefault();
            if (oQSDef != null)
            {
                var FKSteps = oQSDef.Steps.Values.ToList().Where(m => m.FKiParentStepID > 0).ToList();
                if (FKSteps != null && FKSteps.Count() > 0)
                {
                    foreach (var Step in FKSteps)
                    {
                        var oFKStep = GetStepDefinitionByID(Step.FKiParentStepID.ToString());
                        if (oFKStep != null)
                        {
                            var StepID = Step.ID;
                            oFKStep.ID = StepID;
                            oQSDef.Steps[Step.sName] = oFKStep;
                        }
                    }
                }


                var Sections = GetStepSectionDefinitions(oQSDef.XIGUID.ToString(), sSessionID, sGUID, iOrgID);
                if (Sections != null)
                {
                    foreach (var items in Sections.Steps)
                    {
                        var Secs = items.Value.Sections.OrderBy(m => m.Value.iOrder).ToDictionary(t => t.Key, t => t.Value);
                        if (oQSDef.Steps.Where(m => m.Value.XIGUID == items.Value.XIGUID).FirstOrDefault().Value != null)
                        {
                            oQSDef.Steps.Where(m => m.Value.XIGUID == items.Value.XIGUID).FirstOrDefault().Value.Sections = Secs;
                        }
                        //oQSInstance.nStepInstances.Where(m => m.FKiQSStepDefinitionID == items.ID).FirstOrDefault().nSections = items.Sections;
                    }
                    foreach (var sec in Sections.XIDFieldOrigin)
                    {
                        if (!oQSDef.XIDFieldOrigin.ContainsKey(sec.Key))
                        {
                            oQSDef.XIDFieldOrigin[sec.Key] = sec.Value;
                        }
                    }
                    if (Sections.XIDFieldOrigin.Values != null && Sections.XIDFieldOrigin.Values.Count() > 0)
                    {
                        oQSDef.XIDFieldOrigin = Sections.XIDFieldOrigin;
                    }
                    if (Sections.DependentFields != null && Sections.DependentFields.Count() > 0)
                    {
                        oQSDef.DependentFields = Sections.DependentFields;
                    }
                    if (Sections.OrgFieldOrigins != null && Sections.OrgFieldOrigins.Count() > 0)
                    {
                        oQSDef.OrgFieldOrigins = Sections.OrgFieldOrigins;
                    }
                }
                var QSLinks = GetStepQSLinks(oQSDef.XIGUID.ToString());

                if (QSLinks != null)
                {
                    foreach (var items in QSLinks.Steps)
                    {
                        oQSDef.Steps.Values.Where(m => m.XIGUID == items.Value.XIGUID).FirstOrDefault().QSLinks = items.Value.QSLinks;
                    }
                }
                if (oQSDef.iVisualisationIDXIGUID != null && oQSDef.iVisualisationIDXIGUID != Guid.Empty)
                {
                    oQSDef.Visualisation = GetQSVisualisations(oQSDef.iVisualisationIDXIGUID.ToString());
                }
                else if (oQSDef.iVisualisationID > 0)
                {
                    oQSDef.Visualisation = GetQSVisualisations(oQSDef.iVisualisationID.ToString());
                }
                oQSDef.QSVisualisations = GetQSFiledVisualisations(oQSDef.XIGUID.ToString());
                if (oQSDef.Steps != null)
                {
                    foreach (var items in oQSDef.Steps)
                    {
                        if (items.Value.iLayoutID > 0 || (items.Value.iLayoutIDXIGUID != null && items.Value.iLayoutIDXIGUID != Guid.Empty))
                        {
                            if (items.Value.iLayoutIDXIGUID != null && items.Value.iLayoutIDXIGUID != Guid.Empty)
                            {
                                items.Value.Layout = (XIDLayout)Get_LayoutDefinition(null, items.Value.iLayoutIDXIGUID.ToString()).oResult;
                            }
                            else if (items.Value.iLayoutID > 0)
                            {
                                items.Value.Layout = (XIDLayout)Get_LayoutDefinition(null, items.Value.iLayoutID.ToString()).oResult;
                            }
                        }
                        //Get Scripts Linked to Step
                        List<XIQSScript> oStepScripts = null;
                        Dictionary<string, object> Params = new Dictionary<string, object>();
                        Params["FKiStepDefinitionIDXIGUID"] = items.Value.XIGUID;
                        oStepScripts = Connection.Select<XIQSScript>("XIQSScript_T", Params).OrderBy(x => x.ID).ToList();
                        if (oStepScripts != null && oStepScripts.Count() > 0)
                        {
                            foreach (var scr in oStepScripts)
                            {
                                CResult oScr = new CResult();
                                if (scr.FKiScriptIDXIGUID != null && scr.FKiScriptIDXIGUID != Guid.Empty)
                                {
                                    oScr = Get_ScriptDefinition(null, scr.FKiScriptIDXIGUID.ToString());
                                }
                                else if (scr.FKiScriptID > 0)
                                {
                                    oScr = Get_ScriptDefinition(null, scr.FKiScriptID.ToString());
                                }
                                if (oScr.bOK && oScr.oResult != null)
                                {
                                    XIDScript oScriptID = (XIDScript)oScr.oResult;
                                    oQSDef.Steps[items.Value.sName].Scripts[oScriptID.sName] = oScriptID;
                                }
                            }
                        }
                    }
                }
                if (OrgFieldOrigins != null && OrgFieldOrigins.Count > 0)
                {
                    oQSDef.OrgFieldOrigins = OrgFieldOrigins;
                }
            }
            return oQSDef;
        }

        public CResult Get_ScriptDefinition(string sScriptName = "", string sUID = "")
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


            try
            {
                XIUtility.IDInsteadOfGUID(sUID, "Script");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0" && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sScriptName))
                {
                    PKColumn = "sName";
                    PKValue = sScriptName;
                }
                XIDScript oScript = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oScript = Connection.Select<XIDScript>("XIBOScript_T", Params).FirstOrDefault();
                Params = new Dictionary<string, object>();
                Params["FKiScriptIDXIGUID"] = PKValue;
                List<XIDScriptResult> oScriptResult = Connection.Select<XIDScriptResult>("XIBOScriptResult_T", Params).OrderBy(x => x.ID).ToList();
                oScript.ScriptResults = oScriptResult;
                oCResult.oResult = oScript;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Layout definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public XIDQS GetStepSectionDefinitions(string iQSID, string sSessionID = null, string sGUID = null, int iOrgID = 0)
        {
            XIInfraCache oCache = new XIInfraCache();
            int QSID = 0;
            Guid QSGUID = Guid.Empty;
            int.TryParse(iQSID, out QSID);
            Guid.TryParse(iQSID, out QSGUID);
            string sQSDefinitionQry = string.Empty;
            if (QSID > 0)
            {
                sQSDefinitionQry = "select * from XIQSDefinition_T QSDef " +
                "inner join XIQSStepDefinition_T QSSDef on QSDef.ID = QSSDef.FKiQSDefintionID " +
                "inner join XIStepSectionDefinition_T Sec on QSSDef.ID = Sec.FKiStepDefinitionID " +
                "left join XIFieldDefinition_T XIFD on Sec.ID = XIFD.FKiStepSectionID " +
                "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginID = XIO.ID " +
                "left join XIDataType_T XIDT on XIO.FKiDataType = XIDT.id " +
                "left join XIFieldOptionList_T OPT on XIO.ID = OPT.FKiQSFieldID " +
                "WHERE QSDef.ID = @id";
            }
            else if (QSGUID != null && QSGUID != Guid.Empty)
            {
                sQSDefinitionQry = "select * from XIQSDefinition_T QSDef " +
                "inner join XIQSStepDefinition_T QSSDef on QSDef.XIGUID = QSSDef.FKiQSDefintionIDXIGUID " +
                "inner join XIStepSectionDefinition_T Sec on QSSDef.XIGUID = Sec.FKiStepDefinitionIDXIGUID " +
                "left join XIFieldDefinition_T XIFD on Sec.XIGUID = XIFD.FKiStepSectionIDXIGUID " +
                "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginIDXIGUID = XIO.XIGUID " +
                "left join XIDataType_T XIDT on XIO.FKiDataTypeXIGUID = XIDT.XIGUid " +
                "left join XIFieldOptionList_T OPT on XIO.XIGUID = OPT.FKiQSFieldIDXIGUID " +
                "WHERE QSDef.XIGUID = @xiguid;";
            }
            var param = new
            {
                id = QSID,
                xiguid = QSGUID
            };
            XIIXI oXI = new XIIXI();
            List<string> DependentFields = new List<string>();
            var lookup = new Dictionary<Guid, XIDQS>();
            List<Guid> OrgFieldOrigins = new List<Guid>();
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                Conn.Open();
                var lookup2 = new Dictionary<Guid, XIDQSStep>();
                var lookup3 = new Dictionary<Guid, XIDFieldDefinition>();
                var lookup4 = new Dictionary<Guid, XIDFieldOrigin>();
                var lookup6 = new Dictionary<Guid, XIDFieldOptionList>();
                var lookup7 = new Dictionary<Guid, XIDQSSection>();
                Conn.Query<XIDQS, XIDQSStep, XIDQSSection, XIDFieldDefinition, XIDFieldOrigin, XIDDataType, XIDFieldOptionList, XIDQS>(sQSDefinitionQry,
                    (QS, Step, SectionDef, FieldDefinition, FieldOrigin, DataType, OptionList) =>
                    {
                        XIDQS oQSDefinition;
                        if (!lookup.TryGetValue(QS.XIGUID, out oQSDefinition))
                        {
                            lookup.Add(QS.XIGUID, oQSDefinition = QS);
                        }
                        XIDQSStep oStepDefinition;
                        if (Step != null && Step.StatusTypeID == (int)xiEnumSystem.xistatus.xiactive)
                        {
                            if (!lookup2.TryGetValue(Step.XIGUID, out oStepDefinition))
                            {
                                lookup2.Add(Step.XIGUID, oStepDefinition = Step);
                                oQSDefinition.Steps[oStepDefinition.sName] = oStepDefinition;
                            }
                            XIDQSSection oSection;
                            if (SectionDef != null && SectionDef.XIDeleted != 1)
                            {
                                if (!lookup7.TryGetValue(SectionDef.XIGUID, out oSection))
                                {
                                    if (SectionDef.FkiRuleID > 0 || (SectionDef.FkiRuleIDXIGUID != null && SectionDef.FkiRuleIDXIGUID != Guid.Empty))
                                    {
                                        var oRuleI = new XIIBO();
                                        if (SectionDef.FkiRuleIDXIGUID != null && SectionDef.FkiRuleIDXIGUID != Guid.Empty)
                                        {
                                            oRuleI = oXI.BOI("XI Rules", SectionDef.FkiRuleIDXIGUID.ToString());
                                        }
                                        else if (SectionDef.FkiRuleID > 0)
                                        {
                                            oRuleI = oXI.BOI("XI Rules", SectionDef.FkiRuleID.ToString());
                                        }
                                        string sBODataSource = string.Empty;
                                        if (oRuleI != null && oRuleI.Attributes.Count > 0 && oRuleI.Attributes.ContainsKey("sCondition"))
                                        {
                                            SectionDef.sRuleCondition = oRuleI.Attributes["sCondition"].sValue;
                                        }
                                    }
                                    lookup7.Add(SectionDef.XIGUID, oSection = SectionDef);
                                    // oStepDefinition.Sections[SectionDef.ID.ToString() + "_Sec"] = SectionDef;
                                    oStepDefinition.Sections[SectionDef.XIGUID.ToString() + "_Sec"] = SectionDef;
                                }
                                XIDFieldDefinition oFieldDefintion;
                                if (FieldDefinition != null && FieldDefinition.XIDeleted != 1)
                                {
                                    XIDFieldOrigin oXIFieldOrigin;
                                    if (FieldOrigin != null)
                                    {
                                        if (!lookup3.TryGetValue(FieldDefinition.XIGUID, out oFieldDefintion))
                                        {
                                            lookup3.Add(FieldDefinition.XIGUID, oFieldDefintion = FieldDefinition);
                                            oSection.FieldDefs[FieldOrigin.sName] = oFieldDefintion;
                                        }
                                        if (!lookup4.TryGetValue(FieldOrigin.XIGUID, out oXIFieldOrigin))
                                        {
                                            if (FieldOrigin.bOrgFK)
                                            {
                                                OrgFieldOrigins.Add(FieldOrigin.XIGUID);
                                            }
                                            if (FieldOrigin.iMasterDataID > 0)
                                            {
                                                //FieldOrigin.ddlFieldOptionList = dbContext.Types.Where(m => m.Code == FieldOrigin.iMasterDataID).ToList().Select(m => new XIDFieldOptionList { sOptionName = m.Expression, sOptionValue = m.ID.ToString() }).ToList();
                                            }
                                            else if ((FieldOrigin.FK1ClickID > 0 || (FieldOrigin.FK1ClickIDXIGUID != null && FieldOrigin.FK1ClickIDXIGUID != Guid.Empty)) && !FieldOrigin.bOrgFK)
                                            {
                                                var o1ClickD = new XID1Click();
                                                if (FieldOrigin.FK1ClickIDXIGUID != null && FieldOrigin.FK1ClickIDXIGUID != Guid.Empty)
                                                {
                                                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, FieldOrigin.FK1ClickIDXIGUID.ToString());
                                                }
                                                else if (FieldOrigin.FK1ClickID > 0)
                                                {
                                                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, FieldOrigin.FK1ClickID.ToString());
                                                }
                                                XIDBO oBOD = new XIDBO();
                                                if (o1ClickD.BOIDXIGUID != null && o1ClickD.BOIDXIGUID != Guid.Empty)
                                                {
                                                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOIDXIGUID.ToString());
                                                }
                                                else if (o1ClickD.BOID > 0)
                                                {
                                                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                                                }
                                                int iDataSource = oBOD.iDataSource;
                                                Guid iDataSourceXIGUID = oBOD.iDataSourceXIGUID;
                                                XIDXI oXID = new XIDXI();
                                                string sConntection = oXID.GetBODataSource(iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                                                var Connect = new XIDBAPI(sConntection);
                                                o1ClickD.BOD = oBOD;
                                                o1ClickD.Get_1ClickHeadings();
                                                var PrimaryKey = "";
                                                if (o1ClickD.bIsMultiBO == true)
                                                {
                                                    var fields = "[" + o1ClickD.FromBos + "].";
                                                    fields = fields + string.Join(" [" + o1ClickD.FromBos + "].", o1ClickD.TableColumns.ToList());
                                                    var TableColumns = fields.Split(' ');
                                                    o1ClickD.TableColumns = TableColumns.ToList();
                                                    PrimaryKey = "[" + o1ClickD.FromBos + "]." + oBOD.sPrimaryKey;
                                                }
                                                o1ClickD.TableColumns.Remove("HiddenData");
                                                o1ClickD.TableColumns.Remove("[" + oBOD.TableName + "].HiddenData");


                                                var SelFields = string.Join(", ", o1ClickD.TableColumns.ToList());
                                                XIDGroup oGroupD = new XIDGroup();
                                                string FinalString = SelFields;
                                                //option list showing id removed in Drop down list for the QS Fields
                                                // string FinalString = oGroupD.ConcatanateFields(SelFields, " ");
                                                //FinalString = (o1ClickD.bIsMultiBO == true ? PrimaryKey : oBOD.sPrimaryKey) + "," + FinalString;
                                                var FinalQuery = o1ClickD.AddSelectPart(o1ClickD.Query, FinalString);
                                                if (!FinalQuery.Contains("{XIP|"))
                                                {
                                                    Dictionary<string, string> DDL = Connect.GetDDLItems(CommandType.Text, FinalQuery, null);
                                                    var FKDDL = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                                                    var DDList = FKDDL.Where(m => !string.IsNullOrEmpty(m.sOptionName)).GroupBy(m => m.sOptionName).Select(m => m.FirstOrDefault()).ToList().Select(p => new { key = p.sOptionName, value = p.sOptionValue })
                                                        .ToDictionary(x => x.key, x => x.value);
                                                    FieldOrigin.FieldDynamicOptionList = FKDDL;
                                                    FieldOrigin.sBOSize = oBOD.sSize;
                                                }
                                                else if (FinalQuery.Contains("{XIP|-") || FinalQuery.Contains("{XIP|"))
                                                {
                                                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                                                    List<CNV> nParams = new List<CNV>();
                                                    nParams = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                                                    XID1Click oC1ClickD = (XID1Click)o1ClickD.Clone(o1ClickD);
                                                    oC1ClickD.Query = FinalQuery;
                                                    oC1ClickD.ReplaceFKExpressions(nParams);
                                                    FinalQuery = oC1ClickD.Query;
                                                    Dictionary<string, string> DDL = Connect.GetDDLItems(CommandType.Text, FinalQuery, null);
                                                    if (FieldOrigin.FK1ClickID == 8462)
                                                    {
                                                        var iUserOrg = nParams.Where(m => m.sName == "{XIP|iUserOrgID}").Select(m => m.sValue).FirstOrDefault();
                                                        var iNannoGroupID = nParams.Where(m => m.sName == "{XIP|iNannoGroupID}").Select(m => m.sValue).FirstOrDefault();
                                                        var iNannoAppInstID = nParams.Where(m => m.sName == "{XIP|-iNannoAppInstID}").Select(m => m.sValue).FirstOrDefault();
                                                        var iRoleID = nParams.Where(m => m.sName == "iRoleID").Select(m => m.sValue).FirstOrDefault();
                                                        foreach (var obj in DDL)
                                                        {
                                                            var iOrgObjectTypeID = obj.Key.ToString();
                                                            var WhrPrms = new List<CNV>();
                                                            WhrPrms.Add(new CNV { sName = "FKiOrgID", sValue = iUserOrg });
                                                            WhrPrms.Add(new CNV { sName = "FKiNannoGroupID", sValue = iNannoGroupID });
                                                            var shareRole = oXI.BOI("XOrgGroup", null, null, WhrPrms);
                                                            if (shareRole != null && shareRole.Attributes.Count() > 0)
                                                            {
                                                                var ShareRoleID = shareRole.AttributeI("FKiShareRoleID").sValue;
                                                                var Perm1Click = new XID1Click();
                                                                Perm1Click.BOID = 1391;
                                                                if (ShareRoleID == "0" || string.IsNullOrEmpty(ShareRoleID))
                                                                {
                                                                    Perm1Click.Query = "select * from XINannoPermission_T where FKiRoleID =" + iRoleID + " and FKiOrgObjectTypeID=" + iOrgObjectTypeID + " and FKiNannoAppInstID=" + iNannoAppInstID;
                                                                }
                                                                else
                                                                {
                                                                    Perm1Click.Query = "select * from XINannoPermission_T where FKiShareRoleID =" + ShareRoleID + " and FKiOrgObjectTypeID=" + iOrgObjectTypeID + " and FKiNannoAppInstID=" + iNannoAppInstID;
                                                                }

                                                                var Res = Perm1Click.OneClick_Run();
                                                                if (Res != null && Res.Count() > 0)
                                                                {
                                                                    var FKDDL = new List<XIDFieldOptionList>();
                                                                    Dictionary<string, string> Perm = new Dictionary<string, string>();
                                                                    foreach (var perm in Res.Values.ToList())
                                                                    {
                                                                        if (perm.AttributeI("iType").sValue == "30" && perm.AttributeI("iPermission").sValue == "10")
                                                                        {
                                                                            FKDDL.Add(new XIDFieldOptionList { sOptionName = obj.Value, sOptionValue = obj.Key });
                                                                        }
                                                                        //Perm[perm.AttributeI("iType").sValue] = perm.AttributeI("iPermission").sValue;
                                                                    }
                                                                    //Data["Permission"] = Perm;
                                                                    FieldOrigin.FieldDynamicOptionList = FKDDL;
                                                                    FieldOrigin.sBOSize = oBOD.sSize;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var FKDDL = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                                                        var DDList = FKDDL.Where(m => !string.IsNullOrEmpty(m.sOptionName)).GroupBy(m => m.sOptionName).Select(m => m.FirstOrDefault()).ToList().Select(p => new { key = p.sOptionName, value = p.sOptionValue })
                                                            .ToDictionary(x => x.key, x => x.value);
                                                        FieldOrigin.FieldDynamicOptionList = FKDDL;
                                                        FieldOrigin.sBOSize = oBOD.sSize;
                                                    }

                                                }
                                            }
                                            else if ((FieldOrigin.FKiBOID > 0 || (FieldOrigin.FKiBOIDXIGUID != null && FieldOrigin.FKiBOIDXIGUID != Guid.Empty)) && !FieldOrigin.bOrgFK)
                                            {
                                                var oBOD = new XIDBO();
                                                if (FieldOrigin.FKiBOIDXIGUID != null && FieldOrigin.FKiBOIDXIGUID != Guid.Empty)
                                                {
                                                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, FieldOrigin.FKiBOIDXIGUID.ToString());
                                                }
                                                else if (FieldOrigin.FKiBOID > 0)
                                                {
                                                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, FieldOrigin.FKiBOID.ToString());
                                                }
                                                string sBODataSource = string.Empty;
                                                if (oBOD != null)
                                                {
                                                    FieldOrigin.sBOSize = oBOD.sSize;
                                                    if (oBOD.sSize == "10")
                                                    {
                                                        if (DataSrcs.ContainsKey(oBOD.iDataSourceXIGUID))
                                                        {
                                                            sBODataSource = DataSrcs[oBOD.iDataSourceXIGUID];
                                                        }
                                                        else
                                                        {
                                                            sBODataSource = GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                                                            DataSrcs[oBOD.iDataSourceXIGUID] = sBODataSource;
                                                        }
                                                        if (oBOD.Groups.ContainsKey("label"))
                                                        {
                                                            var oGroupD = oBOD.Groups["label"];
                                                            var Con = new XIDBAPI(sBODataSource);
                                                            var LabelGroup = oGroupD.BOFieldNames;
                                                            if (!string.IsNullOrEmpty(LabelGroup))
                                                            {
                                                                FieldOrigin.sBOSize = oBOD.sSize;
                                                                //string FinalString = oGroupD.ConcatanateFields(LabelGroup, " ");
                                                                //FinalString = oBOD.sPrimaryKey + "," + FinalString;
                                                                string sWhrClause = "";

                                                                if (oBOD.iOrgObject == 1)
                                                                {
                                                                    sWhrClause = "[" + oBOD.TableName + "].FKiOrgID=" + iOrgID;
                                                                }
                                                                string FinalString = LabelGroup;
                                                                Dictionary<string, string> DDL = Con.SelectDDL(FinalString, oBOD.TableName, sWhrClause);
                                                                var FKDDL = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                                                                FieldOrigin.FieldDynamicOptionList = new List<XIDFieldOptionList>();
                                                                FieldOrigin.FieldDynamicOptionList = FKDDL;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (FieldOrigin.FkiRuleID > 0 || (FieldOrigin.FkiRuleIDXIGUID != null && FieldOrigin.FkiRuleIDXIGUID != Guid.Empty))
                                            {
                                                var oRuleI = new XIIBO();
                                                if (FieldOrigin.FkiRuleIDXIGUID != null && FieldOrigin.FkiRuleIDXIGUID != Guid.Empty)
                                                {
                                                    oRuleI = oXI.BOI("XI Rules", FieldOrigin.FkiRuleIDXIGUID.ToString());
                                                }
                                                else if (FieldOrigin.FkiRuleID > 0)
                                                {
                                                    oRuleI = oXI.BOI("XI Rules", FieldOrigin.FkiRuleID.ToString());
                                                }
                                                string sBODataSource = string.Empty;
                                                if (oRuleI != null && oRuleI.Attributes.Count > 0 && oRuleI.Attributes.ContainsKey("sCondition"))
                                                {
                                                    FieldOrigin.sRuleCondition = oRuleI.Attributes["sCondition"].sValue;
                                                }
                                            }
                                            if (FieldOrigin.bIsFieldDependent)
                                            {
                                                if (FieldOrigin.sDependentFieldID.Contains(','))
                                                {
                                                    var allflds = FieldOrigin.sDependentFieldID.Split(',').ToList();
                                                    DependentFields.AddRange(allflds);
                                                }
                                                else
                                                {
                                                    DependentFields.Add(FieldOrigin.sDependentFieldID);
                                                }
                                            }
                                            if (FieldOrigin.bIsFieldMerge)
                                            {
                                                DependentFields.Add(FieldOrigin.iMergeFieldID.ToString());
                                            }
                                            lookup4.Add(FieldOrigin.XIGUID, oXIFieldOrigin = FieldOrigin);
                                            oXIFieldOrigin = FieldOrigin;
                                        }

                                        XIDFieldOptionList oOptions;
                                        if (OptionList != null)
                                        {
                                            oOptions = OptionList;
                                            if (!lookup6.TryGetValue(OptionList.XIGUID, out oOptions))
                                            {
                                                lookup6.Add(OptionList.XIGUID, oOptions = OptionList);
                                                if (oXIFieldOrigin.FieldOptionList != null && oXIFieldOrigin.FieldOptionList.Count() > 0)
                                                {
                                                    if (oOptions.XIDeleted == 0)
                                                    {
                                                        oXIFieldOrigin.FieldOptionList.Add(oOptions);
                                                    }
                                                }
                                                else
                                                {
                                                    oXIFieldOrigin.FieldOptionList = new List<XIDFieldOptionList>();
                                                    oXIFieldOrigin.FieldOptionList.Add(oOptions);
                                                }
                                            }
                                        }
                                        oFieldDefintion.FieldOrigin = oXIFieldOrigin;
                                        oQSDefinition.XIDFieldOrigin[oXIFieldOrigin.sName.ToLower()] = oXIFieldOrigin;
                                        FieldOrigin.DataType = DataType;
                                        //if (!oQSDefinition.XIDFieldOrigin.ContainsKey(oXIFieldOrigin.sName))
                                        //{
                                        //    if (FieldOrigin.DataType.sName.ToLower() == "date")
                                        //    {
                                        //        //mindate setting
                                        //        var oResult = SetDefaultDateRange(FieldOrigin.sMinDate, FieldOrigin.sMaxDate);
                                        //        if (oResult != null && oResult.Count() > 0)
                                        //        {
                                        //            if (oResult.ContainsKey("sMinDate"))
                                        //            {
                                        //                FieldOrigin.sMinDate = oResult["sMinDate"];
                                        //            }
                                        //            if (oResult.ContainsKey("sMaxDate"))
                                        //            {
                                        //                FieldOrigin.sMaxDate = oResult["sMaxDate"];
                                        //            }
                                        //        }
                                        //    }
                                        //    oQSDefinition.XIDFieldOrigin[oXIFieldOrigin.sName] = oXIFieldOrigin;
                                        //}
                                    }
                                    //if (oFieldInstance.FieldDefinitions != null && oFieldInstance.FieldDefinitions.Count() > 0)
                                    //{
                                    //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                                    //}
                                    //else
                                    //{
                                    //    oFieldInstance.FieldDefinitions = new List<cXIFieldDefinition>();
                                    //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                                    //}
                                }
                            }
                        }
                        oQSDefinition.DependentFields = DependentFields;
                        return oQSDefinition;
                    },
                    param
                    ).AsQueryable();
            }
            var Sections = lookup.Values.FirstOrDefault();

            var QSLinks = GetSectionQSLinks(iQSID);

            if (QSLinks != null)
            {
                foreach (var items in QSLinks.Steps)
                {
                    foreach (var item in items.Value.Sections)
                    {
                        if (item.Value.QSLinks != null)
                        {
                            Sections.Steps.Where(m => m.Value.XIGUID == items.Value.XIGUID).FirstOrDefault().Value.Sections.Values.Where(m => m.ID == item.Value.ID).FirstOrDefault().QSLinks = item.Value.QSLinks;
                        }
                    }
                }
            }


            var SectionContent = GetComponentParams(iQSID);
            if (SectionContent != null)
            {
                foreach (var items in SectionContent.Steps)
                {
                    foreach (var item in items.Value.Sections)
                    {
                        if (item.Value.ComponentDefinition != null)
                        {
                            var AllSections = Sections.Steps.Where(m => m.Value.XIGUID == items.Value.XIGUID).FirstOrDefault();
                            if (AllSections.Key != null)
                            {
                                AllSections.Value.ComponentDefinition = item.Value.ComponentDefinition;
                            }
                            else
                            {

                            }
                        }
                        //Get Scripts Linked to Section
                        List<XIQSScript> oStepScripts = null;
                        Dictionary<string, object> Params = new Dictionary<string, object>();
                        Params["FKiSectionDefinitionIDXIGUID"] = items.Value.XIGUID;
                        oStepScripts = Connection.Select<XIQSScript>("XIQSScript_T", Params).OrderBy(x => x.ID).ToList();
                        if (oStepScripts != null && oStepScripts.Count() > 0)
                        {
                            foreach (var scr in oStepScripts)
                            {
                                CResult oScr = Get_ScriptDefinition(null, scr.FKiScriptIDXIGUID.ToString());
                                if (oScr.bOK && oScr.oResult != null)
                                {
                                    XIDScript oScriptID = (XIDScript)oScr.oResult;
                                    Sections.Steps.Where(m => m.Value.XIGUID == items.Value.XIGUID).FirstOrDefault().Value.Sections.Values.Where(m => m.ID == item.Value.ID).FirstOrDefault().Scripts[oScriptID.sName] = oScriptID;
                                    //items.Value.Scripts[oScriptID.sName] = oScriptID;
                                }
                            }
                        }
                    }
                }
            }
            if(OrgFieldOrigins != null && OrgFieldOrigins.Count > 0)
            {
                Sections.OrgFieldOrigins = OrgFieldOrigins;
            }
            return Sections;
        }

        public XIDQS GetPartialQS(int iQSID)
        {
            XIInfraCache oCache = new XIInfraCache();
            string sQSDefinitionQry = "select * from XIQSDefinition_T QSDef " +
                "inner join XIDPartialQS_T PartialQS on QSDef.ID = PartialQS.FkiMainQSID " +
                "WHERE QSDef.ID = @id;";
            var param = new
            {
                id = iQSID
            };
            List<string> DependentFields = new List<string>();
            var lookup = new Dictionary<Guid, XIDQS>();
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                Conn.Open();
                var lookup2 = new Dictionary<Guid, XIDPartialQS>();
                var lookup3 = new Dictionary<Guid, XIDFieldDefinition>();
                var lookup4 = new Dictionary<Guid, XIDFieldOrigin>();
                var lookup6 = new Dictionary<Guid, XIDFieldOptionList>();
                var lookup7 = new Dictionary<Guid, XIDQSSection>();
                Conn.Query<XIDQS, XIDPartialQS, XIDQS>(sQSDefinitionQry,
                    (QS, PartialQS) =>
                    {
                        XIDQS oQSDefinition;
                        if (!lookup.TryGetValue(QS.XIGUID, out oQSDefinition))
                        {
                            lookup.Add(QS.XIGUID, oQSDefinition = QS);
                        }
                        XIDPartialQS oPartialQSDefinition;
                        if (PartialQS != null)
                        {
                            if (!lookup2.TryGetValue(PartialQS.XIGUID, out oPartialQSDefinition))
                            {
                                var oPartialQSD = GetQuestionSetDefinitionByID("", PartialQS.FKiPartialQSID.ToString());
                                PartialQS.oQSD[oPartialQSD.sName] = oPartialQSD;
                                lookup2.Add(PartialQS.XIGUID, oPartialQSDefinition = PartialQS);
                                oQSDefinition.PartialQS[oPartialQSD.sName] = oPartialQSDefinition;
                            }
                        }
                        return oQSDefinition;
                    },
                    param
                    ).AsQueryable();
            }
            var oQSDef = lookup.Values.FirstOrDefault();
            return oQSDef;
        }

        private XIDQS GetStepQSLinks(string iQSID)
        {
            int QSID = 0;
            Guid QSGUID = Guid.Empty;
            int.TryParse(iQSID, out QSID);
            Guid.TryParse(iQSID, out QSGUID);
            string sQSDefinitionQry = string.Empty;
            if (QSID > 0)
            {
                sQSDefinitionQry = "select * from XIQSDefinition_T QSD " +
                "inner join XIQSStepDefinition_T QSSD on QSD.ID = QSSD.FKiQSDefintionID " +
                "left join XIQSLink_T QSlink on QSSD.ID = QSlink.FKiStepDefinitionID " +
                "left join XIQSLinkDefinition_T link on QSlink.sCode = link.sCode " +
                "left join XILink_T Xilink on link.FKiXILinkID = Xilink.XiLinkID " +
                "left join XILinkNV_T Xilinknv on Xilink.XILinkID = Xilinknv.XiLinkID " +
                "where QSD.ID = @id and QSSD.StatusTypeID = 10 and QSlink.iStatus=10;";
            }
            else if (QSGUID != null && QSGUID != Guid.Empty)
            {
                sQSDefinitionQry = "select * from XIQSDefinition_T QSD " +
                "inner join XIQSStepDefinition_T QSSD on QSD.XIGUID = QSSD.FKiQSDefintionIDXIGUID " +
                "left join XIQSLink_T QSlink on QSSD.XIGUID = QSlink.FKiStepDefinitionIDXIGUID " +
                "left join XIQSLinkDefinition_T link on QSlink.sCode = link.sCode " +
                "left join XILink_T Xilink on link.FKiXILinkID = Xilink.XiLinkID " +
                "left join XILinkNV_T Xilinknv on Xilink.XILinkID = Xilinknv.XiLinkID " +
                "where QSD.XIGUID = @xiguid and QSSD.StatusTypeID = 10 and QSlink.iStatus=10;";
            }
            var param = new
            {
                id = QSID,
                xiguid = QSGUID
            };
            var lookup = new Dictionary<Guid, XIDQS>();
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                Conn.Open();
                var lookup2 = new Dictionary<Guid, XIDQSStep>();
                var lookup4 = new Dictionary<Guid, XIQSLink>();
                var lookup5 = new Dictionary<Guid, XIQSLinkDefintion>();
                var lookup6 = new Dictionary<Guid, XILink>();
                Conn.Query<XIDQS, XIDQSStep, XIQSLink, XIQSLinkDefintion, XILink, XiLinkNV, XIDQS>(sQSDefinitionQry,
                    (QS, Step, QSLink, Link, XILink, XILinkNV) =>
                    {
                        XIDQS oQSDefinition;
                        if (!lookup.TryGetValue(QS.XIGUID, out oQSDefinition))
                        {
                            lookup.Add(QS.XIGUID, oQSDefinition = QS);
                        }
                        XIDQSStep oStepDefinition;
                        if (Step != null && Step.StatusTypeID == (int)xiEnumSystem.xistatus.xiactive)
                        {
                            if (!lookup2.TryGetValue(Step.XIGUID, out oStepDefinition))
                            {
                                lookup2.Add(Step.XIGUID, oStepDefinition = Step);
                                oQSDefinition.Steps[oStepDefinition.sName] = oStepDefinition;
                            }
                            XIQSLink oSecQSLink;
                            if (QSLink != null)
                            {
                                XIQSLinkDefintion oXIDLink;
                                if (Link != null)
                                {
                                    if (!lookup4.TryGetValue(QSLink.XIGUID, out oSecQSLink))
                                    {
                                        lookup4.Add(QSLink.XIGUID, oSecQSLink = QSLink);
                                        if (XILink != null)
                                        {
                                            oStepDefinition.QSLinks[XILink.Name] = QSLink;
                                        }
                                        else
                                        {
                                            oStepDefinition.QSLinks[QSLink.ID.ToString()] = QSLink;
                                        }
                                    }
                                    oXIDLink = Link;
                                    oSecQSLink.XiLink[Link.sName] = Link;
                                    XILink oXILink;
                                    if (XILink != null)
                                    {
                                        if (!lookup5.TryGetValue(Link.XIGUID, out oXIDLink))
                                        {
                                            lookup5.Add(Link.XIGUID, oXIDLink = Link);

                                            oXILink = XILink;
                                            oXIDLink.XiLink[XILink.Name] = XILink;
                                            if (XILinkNV != null)
                                            {
                                                if (oXILink.XiLinkNVs != null && oXILink.XiLinkNVs.Count() > 0)
                                                {
                                                    oXILink.XiLinkNVs.Add(XILinkNV);
                                                }
                                                else
                                                {
                                                    oXILink.XiLinkNVs = new List<XiLinkNV>();
                                                    oXILink.XiLinkNVs.Add(XILinkNV);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return oQSDefinition;
                    },
                    param,
                     splitOn: "id,id,id,id,xilinkid,id"
                    ).AsQueryable();
            }
            var SectionContent = lookup.Values.FirstOrDefault();
            return SectionContent;
        }

        private XIDQS GetSectionQSLinks(string iQSID)
        {
            int QSID = 0;
            Guid QSGUID = Guid.Empty;
            int.TryParse(iQSID, out QSID);
            Guid.TryParse(iQSID, out QSGUID);
            string sQSDefinitionQry = string.Empty;
            if (QSID > 0)
            {
                sQSDefinitionQry = "select * from XIQSDefinition_T QSD " +
                "inner join XIQSStepDefinition_T QSSD on QSD.ID = QSSD.FKiQSDefintionID " +
                "inner join XIStepSectionDefinition_T Sec on QSSD.ID = Sec.FKiStepDefinitionID " +
                "left join XIQSLink_T QSlink on sec.ID = QSlink.FKiSectionDefinitionID " +
                "left join XIQSLinkDefinition_T link on QSlink.sCode = link.sCode " +
                "left join XILink_T Xilink on link.FKiXILinkID = Xilink.XiLinkID " +
                "left join XILinkNV_T Xilinknv on Xilink.XILinkID = Xilinknv.XiLinkID " +
                "where QSD.ID = @id and QSlink.iStatus=10;";
            }
            else if (QSGUID != null && QSGUID != Guid.Empty)
            {
                sQSDefinitionQry = "select * from XIQSDefinition_T QSD " +
                "inner join XIQSStepDefinition_T QSSD on QSD.XIGUID = QSSD.FKiQSDefintionIDXIGUID " +
                "inner join XIStepSectionDefinition_T Sec on QSSD.XIGUID = Sec.FKiStepDefinitionIDXIGUID " +
                "left join XIQSLink_T QSlink on sec.XIGUID = QSlink.FKiSectionDefinitionIDXIGUID " +
                "left join XIQSLinkDefinition_T link on QSlink.sCode = link.sCode " +
                "left join XILink_T Xilink on link.FKiXILinkID = Xilink.XiLinkID " +
                "left join XILinkNV_T Xilinknv on Xilink.XILinkID = Xilinknv.XiLinkID " +
                "where QSD.xiguid = @xiguid and QSlink.iStatus=10;";
            }

            var param = new
            {
                id = QSID,
                xiguid = QSGUID
            };
            var lookup = new Dictionary<Guid, XIDQS>();
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                Conn.Open();
                var lookup2 = new Dictionary<Guid, XIDQSStep>();
                var lookup3 = new Dictionary<Guid, XIDQSSection>();
                var lookup4 = new Dictionary<Guid, XIQSLink>();
                var lookup5 = new Dictionary<Guid, XIQSLinkDefintion>();
                var lookup6 = new Dictionary<Guid, XILink>();
                Conn.Query<XIDQS, XIDQSStep, XIDQSSection, XIQSLink, XIQSLinkDefintion, XILink, XiLinkNV, XIDQS>(sQSDefinitionQry,
                    (QS, Step, SectionDef, QSLink, Link, XILink, XILinkNV) =>
                    {
                        XIDQS oQSDefinition;
                        if (!lookup.TryGetValue(QS.XIGUID, out oQSDefinition))
                        {
                            lookup.Add(QS.XIGUID, oQSDefinition = QS);
                        }
                        XIDQSStep oStepDefinition;
                        if (Step != null && Step.StatusTypeID == (int)xiEnumSystem.xistatus.xiactive)
                        {
                            if (!lookup2.TryGetValue(Step.XIGUID, out oStepDefinition))
                            {
                                lookup2.Add(Step.XIGUID, oStepDefinition = Step);
                                oQSDefinition.Steps[oStepDefinition.sName] = oStepDefinition;
                            }
                            XIDQSSection oSection;
                            if (SectionDef != null && SectionDef.XIDeleted != 1)
                            {
                                if (!lookup3.TryGetValue(SectionDef.XIGUID, out oSection))
                                {
                                    lookup3.Add(SectionDef.XIGUID, oSection = SectionDef);
                                    //oStepDefinition.Sections[SectionDef.ID.ToString() + "_Sec"] = SectionDef;
                                    oStepDefinition.Sections[SectionDef.XIGUID.ToString() + "_Sec"] = SectionDef;
                                }
                                XIQSLink oSecQSLink;
                                if (QSLink != null)
                                {
                                    XIQSLinkDefintion oXIDLink;
                                    if (Link != null)
                                    {
                                        if (!lookup4.TryGetValue(QSLink.XIGUID, out oSecQSLink))
                                        {
                                            lookup4.Add(QSLink.XIGUID, oSecQSLink = QSLink);
                                            oSection.QSLinks[XILink.Name] = QSLink;
                                        }
                                        oXIDLink = Link;
                                        oSecQSLink.XiLink[Link.sName] = Link;
                                        if (XILink != null)
                                        {
                                            XILink oXILink;
                                            oXILink = XILink;
                                            oXIDLink.XiLink[XILink.Name] = XILink;
                                            if (XILinkNV != null)
                                            {
                                                if (oXILink.XiLinkNVs != null && oXILink.XiLinkNVs.Count() > 0)
                                                {
                                                    oXILink.XiLinkNVs.Add(XILinkNV);
                                                }
                                                else
                                                {
                                                    oXILink.XiLinkNVs = new List<XiLinkNV>();
                                                    oXILink.XiLinkNVs.Add(XILinkNV);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return oQSDefinition;
                    },
                    param,
                     splitOn: "id,id,id,id,xilinkid,id"
                    ).AsQueryable();
            }
            var SectionContent = lookup.Values.FirstOrDefault();
            return SectionContent;
        }

        private XIDQS GetComponentParams(string iQSID)
        {
            int QSID = 0;
            Guid QSGUID = Guid.Empty;
            int.TryParse(iQSID, out QSID);
            Guid.TryParse(iQSID, out QSGUID);
            string sQSDefinitionQry = string.Empty;
            if (QSID > 0)
            {
                sQSDefinitionQry = "select * from XIQSDefinition_T QSD " +
                "inner join XIQSStepDefinition_T QSSD on QSD.ID = QSSD.FKiQSDefintionID " +
                "inner join XIStepSectionDefinition_T Sec on QSSD.ID = Sec.FKiStepDefinitionID " +
                "inner join XIComponentParams_T NVs on sec.ID = NVs.iStepSectionID " +
                "inner join XIComponents_XC_T XC on NVs.FKiComponentID = XC.ID " +
                 "where QSD.ID = @id";
            }
            else if (QSGUID != null && QSGUID != Guid.Empty)
            {
                sQSDefinitionQry = "select * from XIQSDefinition_T QSD " +
                "inner join XIQSStepDefinition_T QSSD on QSD.XIGUID = QSSD.FKiQSDefintionIDXIGUID " +
                "inner join XIStepSectionDefinition_T Sec on QSSD.XIGUID = Sec.FKiStepDefinitionIDXIGUID " +
                "inner join XIComponentParams_T NVs on sec.XIGUID = NVs.iStepSectionIDXIGUID " +
                "inner join XIComponents_XC_T XC on NVs.FKiComponentIDXIGUID = XC.XIGUID " +
                "where QSD.xiguid = @xiguid";
            }
            var param = new
            {
                id = QSID,
                xiguid = QSGUID
            };
            var lookup = new Dictionary<Guid, XIDQS>();
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                Conn.Open();
                var lookup2 = new Dictionary<Guid, XIDQSStep>();
                var lookup3 = new Dictionary<Guid, XIDQSSection>();
                var lookup4 = new Dictionary<Guid, XIDComponent>();
                Conn.Query<XIDQS, XIDQSStep, XIDQSSection, XIDComponentParam, XIDComponent, XIDQS>(sQSDefinitionQry,
                    (QS, Step, SectionDef, ComponentParams, Component) =>
                    {
                        XIDQS oQSDefinition;
                        if (!lookup.TryGetValue(QS.XIGUID, out oQSDefinition))
                        {
                            lookup.Add(QS.XIGUID, oQSDefinition = QS);
                        }
                        XIDQSStep oStepDefinition;
                        if (Step != null && Step.StatusTypeID == (int)xiEnumSystem.xistatus.xiactive)
                        {
                            if (!lookup2.TryGetValue(Step.XIGUID, out oStepDefinition))
                            {
                                lookup2.Add(Step.XIGUID, oStepDefinition = Step);
                                oQSDefinition.Steps[oStepDefinition.sName] = oStepDefinition;
                            }
                            XIDQSSection oSection;
                            if (SectionDef != null && SectionDef.XIDeleted != 1)
                            {
                                if (!lookup3.TryGetValue(SectionDef.XIGUID, out oSection))
                                {
                                    lookup3.Add(SectionDef.XIGUID, oSection = SectionDef);
                                    //oStepDefinition.Sections[SectionDef.ID.ToString() + "_Sec"] = SectionDef;
                                    oStepDefinition.Sections[SectionDef.XIGUID.ToString() + "_Sec"] = SectionDef;
                                }
                                XIDComponent oComponent;
                                if (Component != null)
                                {
                                    if (!lookup4.TryGetValue(SectionDef.XIGUID, out oComponent))
                                    {
                                        lookup4.Add(SectionDef.XIGUID, oComponent = Component);
                                        oSection.ComponentDefinition = Component;
                                    }
                                    if (ComponentParams != null)
                                    {
                                        if (oSection.ComponentDefinition.Params != null && oSection.ComponentDefinition.Params.Count() > 0)
                                        {
                                            oSection.ComponentDefinition.Params.Add(ComponentParams);
                                        }
                                        else
                                        {
                                            oSection.ComponentDefinition.Params = new List<XIDComponentParam>();
                                            oSection.ComponentDefinition.Params.Add(ComponentParams);
                                        }
                                    }
                                }
                            }
                        }
                        return oQSDefinition;
                    },
                    param
                    ).AsQueryable();
            }
            var SectionContent = lookup.Values.FirstOrDefault();
            return SectionContent;
        }

        private XIVisualisation GetQSVisualisations(string VisualID)
        {
            XIVisualisation oXiVis = null;
            int iVisualID = 0;
            int.TryParse(VisualID, out iVisualID);
            Guid VisualGUID = Guid.Empty;
            Guid.TryParse(VisualID, out VisualGUID);
            Dictionary<string, object> Params = new Dictionary<string, object>();
            if (VisualGUID != null && VisualGUID != Guid.Empty)
            {
                Params["XIGUID"] = VisualGUID;
            }
            if (iVisualID > 0)
            {
                Params["XiVisualID"] = iVisualID;
            }
            oXiVis = Connection.Select<XIVisualisation>("XiVisualisations", Params).FirstOrDefault();
            if (oXiVis != null)
            {
                List<XIVisualisationNV> oXIVisNV = null;
                Dictionary<string, object> NVParams = new Dictionary<string, object>();
                if (VisualGUID != null && VisualGUID != Guid.Empty)
                {
                    NVParams["XiVisualIDXIGUID"] = VisualGUID;
                }
                if (iVisualID > 0)
                {
                    NVParams["XiVisualID"] = iVisualID;
                }
                oXIVisNV = Connection.Select<XIVisualisationNV>("XiVisualisationNVs", NVParams).OrderBy(x => x.ID).ToList();
                if (oXIVisNV != null && oXIVisNV.Count() > 0)
                {
                    oXiVis.XiVisualisationNVs = oXIVisNV;
                }
            }
            return oXiVis;
        }

        private List<XIQSVisualisation> GetQSFiledVisualisations(string iQSID)
        {
            int QSID = 0;
            Guid QSGUID = Guid.Empty;
            int.TryParse(iQSID, out QSID);
            Guid.TryParse(iQSID, out QSGUID);
            List<XIQSVisualisation> oXiVis = null;
            Dictionary<string, object> Params = new Dictionary<string, object>();
            if (QSID > 0)
            {
                Params["FKiQSDefinitionID"] = QSID;
            }
            else if (QSGUID != null && QSGUID != Guid.Empty)
            {
                Params["FKiQSDefinitionIDXIGUID"] = QSGUID;
            }
            oXiVis = Connection.Select<XIQSVisualisation>("XIQSVisualisation_T", Params).OrderBy(x => x.ID).ToList();
            return oXiVis;
        }

        public CResult Get_StepDefinition(string sStepName = "", string sUID = "")
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
                XIDQSStep oDQS = new XIDQSStep();
                long iID;
                XIIBO oBOI = null/* TODO Change to default(_) if this is not a reference type */;
                XIUtility.IDInsteadOfGUID(sUID, "Step");

                // sLoadSteps - if "" then load all. otherwise for example:
                // Step1,Step3,Step9,Step10
                // load steps keyed by name (which must be unique and if it gets same name again just dont add in)
                // the steps are instances
                // KEY BY LOWER CASE ONLY

                // TO DO - get oQSD from the cache
                if (!string.IsNullOrEmpty(sStepName) || !string.IsNullOrEmpty(sUID))
                {
                    oDQS = GetStepDefinitionByID(sUID, sStepName);
                }

                if (oDQS != null)
                {
                    oDQS.BOI = oBOI;
                    oDQS.oParent = this;
                }
                oCResult.oResult = oDQS;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Question Set definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }


        public XIDQSStep GetStepDefinitionByID(string sUID = "", string sStepName = "")
        {
            string sQSDefinitionQry = "select * from XIQSStepDefinition_T QSSDef " +
                "left join XIFieldDefinition_T XIFD on QSSDef.ID = XIFD.FKiXIStepDefinitionID " +
                "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginID = XIO.ID " +
                "left join XIDataType_T XIDT on XIO.FKiDataType = XIDT.id " +
                "left join XIQSNavigation_T NAV on QSSDef.ID = NAV.FKiStepDefinitionID " +
                "left join XIFieldOptionList_T OPT on XIO.ID = OPT.FKiQSFieldID ";

            long iID = 0;
            if (long.TryParse(sUID, out iID))
            {
            }
            else
            {
            }

            if (!string.IsNullOrEmpty(sStepName))
            {
                sQSDefinitionQry = sQSDefinitionQry + "WHERE QSSDef.sName = @StepNam;";
            }
            else if (iID > 0)
            {
                sQSDefinitionQry = sQSDefinitionQry + "WHERE QSSDef.ID = @id;";
            }
            else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
            {
                sQSDefinitionQry = "select * from XIQSStepDefinition_T QSSDef " +
               "left join XIFieldDefinition_T XIFD on QSSDef.XIGUID = XIFD.FKiXIStepDefinitionIDXIGUID " +
               "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginIDXIGUID = XIO.XIGUID " +
               "left join XIDataType_T XIDT on XIO.FKiDataTypeXIGUID = XIDT.XIGUID " +
               "left join XIQSNavigation_T NAV on QSSDef.XIGUID = NAV.FKiStepDefinitionIDXIGUID " +
               "left join XIFieldOptionList_T OPT on XIO.XIGUID = OPT.FKiQSFieldIDXIGUID " +
               "WHERE QSSDef.xiguid = @xiguid";
            }

            var param = new
            {
                id = iID,
                StepName = sStepName,
                xiguid = sUID
            };
            XIInfraCache oCache = new XIInfraCache();
            //if (iStepID > 0)
            //{
            //    sQSDefinitionQry = sQSDefinitionQry + "WHERE QSSDef.ID = @id;";
            //}
            //if (!string.IsNullOrEmpty(sStepName))
            //{
            //    sQSDefinitionQry = sQSDefinitionQry + "WHERE QSSDef.sName = @StepName;";
            //}
            var lookup2 = new Dictionary<Guid, XIDQSStep>();
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                Conn.Open();
                var lookup = new Dictionary<Guid, XIDQS>();
                var lookup3 = new Dictionary<Guid, XIDFieldDefinition>();
                var lookup4 = new Dictionary<Guid, XIDFieldOrigin>();
                var lookup5 = new Dictionary<Guid, XIDQSStepNavigations>();
                var lookup6 = new Dictionary<Guid, XIDFieldOptionList>();
                Conn.Query<XIDQSStep, XIDFieldDefinition, XIDFieldOrigin, XIDDataType, XIDQSStepNavigations, XIDFieldOptionList, XIDQSStep>(sQSDefinitionQry,
                    (Step, FieldDefinition, FieldOrigin, DataType, Navigations, OptionList) =>
                    {
                        //XIDQS oQSDefinition;
                        //if (!lookup.TryGetValue(QS.ID, out oQSDefinition))
                        //{
                        //    lookup.Add(QS.ID, oQSDefinition = QS);
                        //}
                        XIDQSStep oStepDefinition;
                        //if (Step != null)
                        //{
                        if (!lookup2.TryGetValue(Step.XIGUID, out oStepDefinition))
                        {
                            lookup2.Add(Step.XIGUID, oStepDefinition = Step);
                        }
                        //if (!lookup2.TryGetValue(Step.ID, out oStepDefinition))
                        //{
                        //    lookup2.Add(Step.ID, oStepDefinition = Step);
                        //    if (oQSDefinition.QSSteps != null && oQSDefinition.QSSteps.Count() > 0)
                        //    {
                        //        oQSDefinition.QSSteps.Add(oStepDefinition);
                        //    }
                        //    else
                        //    {
                        //        oQSDefinition.QSSteps = new List<XIDQSStep>();
                        //        oQSDefinition.QSSteps.Add(oStepDefinition);
                        //    }
                        //}
                        XIDFieldDefinition oFieldDefintion;
                        if (FieldDefinition != null && FieldDefinition.XIDeleted != 1)
                        {

                            XIDFieldOrigin oXIFieldOrigin;
                            if (FieldOrigin != null)
                            {
                                if (!lookup3.TryGetValue(FieldDefinition.XIGUID, out oFieldDefintion))
                                {
                                    lookup3.Add(FieldDefinition.XIGUID, oFieldDefintion = FieldDefinition);
                                    oStepDefinition.FieldDefs[FieldOrigin.sName] = oFieldDefintion;
                                }
                                if (!lookup4.TryGetValue(FieldOrigin.XIGUID, out oXIFieldOrigin))
                                {

                                    if (FieldOrigin.iMasterDataID > 0)
                                    {
                                        //FieldOrigin.ddlFieldOptionList = dbContext.Types.Where(m => m.Code == FieldOrigin.iMasterDataID).ToList().Select(m => new XIDFieldOptionList { sOptionName = m.Expression, sOptionValue = m.ID.ToString() }).ToList();
                                    }
                                    else if (FieldOrigin.FK1ClickID > 0 || (FieldOrigin.FK1ClickIDXIGUID != null && FieldOrigin.FK1ClickIDXIGUID != Guid.Empty))
                                    {
                                        var o1ClickD = new XID1Click();
                                        if (FieldOrigin.FK1ClickIDXIGUID != null && FieldOrigin.FK1ClickIDXIGUID != Guid.Empty)
                                        {
                                            o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, FieldOrigin.FK1ClickIDXIGUID.ToString());
                                        }
                                        else if (FieldOrigin.FK1ClickID > 0)
                                        {
                                            o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, FieldOrigin.FK1ClickID.ToString());
                                        }
                                        XIDBO oBOD = new XIDBO();
                                        if (o1ClickD.BOIDXIGUID != null && o1ClickD.BOIDXIGUID != Guid.Empty)
                                        {
                                            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOIDXIGUID.ToString());
                                        }
                                        else if (o1ClickD.BOID > 0)
                                        {
                                            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                                        }
                                        int iDataSource = oBOD.iDataSource;
                                        Guid iDataSourceXIGUID = oBOD.iDataSourceXIGUID;
                                        XIDXI oXID = new XIDXI();
                                        string sConntection = oXID.GetBODataSource(iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                                        var Connect = new XIDBAPI(sConntection);
                                        o1ClickD.BOD = oBOD;
                                        o1ClickD.Get_1ClickHeadings();
                                        //var bIsPKExists = o1ClickD.TableColumns.ConvertAll(m => m.ToLower()).Contains(oBOD.sPrimaryKey.ToLower());
                                        //if (!bIsPKExists)
                                        //{

                                        //}                                        
                                        var SelFields = string.Join(", ", o1ClickD.TableColumns.ToList());
                                        XIDGroup oGroupD = new XIDGroup();
                                        string FinalString = oGroupD.ConcatanateFields(SelFields, " ");
                                        FinalString = oBOD.sPrimaryKey + "," + FinalString;
                                        var FinalQuery = o1ClickD.AddSelectPart(o1ClickD.Query, FinalString);
                                        //var oBOIns = (DataTable)Connection.ExecuteQuery(o1ClickD.Query);
                                        Dictionary<string, string> DDL = Connect.GetDDLItems(CommandType.Text, FinalQuery, null);
                                        var FKDDL = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                                        //List<Person> List1 = new List<Person>();
                                        var DDList = FKDDL.Where(m => !string.IsNullOrEmpty(m.sOptionName)).GroupBy(m => m.sOptionName).Select(m => m.FirstOrDefault()).ToList().Select(p => new { key = p.sOptionName, value = p.sOptionValue })
                                                .ToDictionary(x => x.key, x => x.value);
                                        FieldOrigin.sBOSize = oBOD.sSize;
                                    }
                                    lookup4.Add(FieldOrigin.XIGUID, oXIFieldOrigin = FieldOrigin);
                                    oXIFieldOrigin = FieldOrigin;
                                }

                                XIDFieldOptionList oOptions;
                                if (OptionList != null)
                                {
                                    oOptions = OptionList;
                                    if (!lookup6.TryGetValue(OptionList.XIGUID, out oOptions))
                                    {
                                        lookup6.Add(OptionList.XIGUID, oOptions = OptionList);
                                        if (oXIFieldOrigin.FieldOptionList != null && oXIFieldOrigin.FieldOptionList.Count() > 0)
                                        {
                                            oXIFieldOrigin.FieldOptionList.Add(oOptions);
                                        }
                                        else
                                        {
                                            oXIFieldOrigin.FieldOptionList = new List<XIDFieldOptionList>();
                                            oXIFieldOrigin.FieldOptionList.Add(oOptions);
                                        }
                                    }
                                }
                                oFieldDefintion.FieldOrigin = oXIFieldOrigin;
                            }

                            FieldOrigin.DataType = DataType;

                            //if (oFieldInstance.FieldDefinitions != null && oFieldInstance.FieldDefinitions.Count() > 0)
                            //{
                            //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                            //}
                            //else
                            //{
                            //    oFieldInstance.FieldDefinitions = new List<cXIFieldDefinition>();
                            //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                            //}
                        }
                        if (Navigations != null)
                        {
                            XIDQSStepNavigations nNavs;
                            if (!lookup5.TryGetValue(Navigations.XIGUID, out nNavs))
                            {
                                if (!string.IsNullOrEmpty(Navigations.sName))
                                    oStepDefinition.Navigations[Navigations.sName] = nNavs;
                            }
                        }
                        //}
                        return oStepDefinition;
                    },
                    param
                    ).AsQueryable();
            }
            var oQSStepDef = lookup2.Values.FirstOrDefault();
            var QSLinks = GetStepQSLinks(oQSStepDef.FKiQSDefintionIDXIGUID.ToString());

            if (QSLinks != null)
            {
                foreach (var items in QSLinks.Steps)
                {
                    oQSStepDef.QSLinks = items.Value.QSLinks;
                }
            }
            var oStepSections = GetStepSectionDefinitionsByStep(oQSStepDef.XIGUID.ToString());
            if (oStepSections != null)
            {
                oQSStepDef.Sections = oStepSections.Sections;
            }
            if (oQSStepDef.iLayoutID > 0)
            {
                oQSStepDef.Layout = (XIDLayout)Get_LayoutDefinition(null, oQSStepDef.iLayoutID.ToString()).oResult; //Common.GetLayoutDetails(oQSStepDef.iLayoutID, 0, 0, 0, null, iUserID, sOrgName, sDatabase);
            }
            return oQSStepDef;
        }

        public XIDQSStep GetStepSectionDefinitionsByStep(string StepUID)
        {
            int iStepID = 0;
            Guid StepGUID = Guid.Empty;
            int.TryParse(StepUID, out iStepID);
            Guid.TryParse(StepUID, out StepGUID);
            XIInfraCache oCache = new XIInfraCache();
            string sQSDefinitionQry = string.Empty;
            if (iStepID > 0)
            {
                sQSDefinitionQry = "select * from XIQSStepDefinition_T QSSDef " +
                "left join XIStepSectionDefinition_T Sec on QSSDef.ID = Sec.FKiStepDefinitionID " +
                "left join XIFieldDefinition_T XIFD on Sec.ID = XIFD.FKiStepSectionID " +
                "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginID = XIO.ID " +
                "left join XIDataType_T XIDT on XIO.FKiDataType = XIDT.id " +
                "left join XIFieldOptionList_T OPT on XIO.ID = OPT.FKiQSFieldID " +
                "WHERE QSSDef.ID = @id;";
            }
            else if (StepGUID != null && StepGUID != Guid.Empty)
            {
                sQSDefinitionQry = "select * from XIQSStepDefinition_T QSSDef " +
                "left join XIStepSectionDefinition_T Sec on QSSDef.XIGUID = Sec.FKiStepDefinitionIDXIGUID " +
                "left join XIFieldDefinition_T XIFD on Sec.XIGUID = XIFD.FKiStepSectionIDXIGUID " +
                "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginIDXIGUID = XIO.XIGUID " +
                "left join XIDataType_T XIDT on XIO.FKiDataTypeXIGUID = XIDT.XIGUID " +
                "left join XIFieldOptionList_T OPT on XIO.XIGUID = OPT.FKiQSFieldIDXIGUID " +
                "WHERE QSSDef.XIGUID = @xiguid;";
            }

            var param = new
            {
                id = iStepID,
                xiguid = StepGUID
            };
            var lookup = new Dictionary<Guid, XIDQSStep>();
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                Conn.Open();
                var lookup2 = new Dictionary<Guid, XIDQSSection>();
                var lookup3 = new Dictionary<Guid, XIDFieldDefinition>();
                var lookup4 = new Dictionary<Guid, XIDFieldOrigin>();
                var lookup5 = new Dictionary<Guid, XIDQSStepNavigations>();
                var lookup6 = new Dictionary<Guid, XIDFieldOptionList>();
                Conn.Query<XIDQSStep, XIDQSSection, XIDFieldDefinition, XIDFieldOrigin, XIDDataType, XIDFieldOptionList, XIDQSStep>(sQSDefinitionQry,
                    (Step, SectionDef, FieldDefinition, FieldOrigin, DataType, OptionList) =>
                    {
                        XIDQSStep oStepDefinition;
                        if (!lookup.TryGetValue(Step.XIGUID, out oStepDefinition))
                        {
                            lookup.Add(Step.XIGUID, oStepDefinition = Step);
                        }
                        XIDQSSection oSection;
                        if (SectionDef != null && SectionDef.XIDeleted != 1)
                        {
                            if (!lookup2.TryGetValue(SectionDef.XIGUID, out oSection))
                            {
                                lookup2.Add(SectionDef.XIGUID, oSection = SectionDef);
                                if (SectionDef.sName != null)
                                {
                                    oStepDefinition.Sections[SectionDef.sName] = SectionDef;
                                }
                                else
                                {
                                    //oStepDefinition.Sections[SectionDef.ID.ToString() + "_Sec"] = SectionDef;
                                    oStepDefinition.Sections[SectionDef.XIGUID.ToString() + "_Sec"] = SectionDef;
                                }
                            }
                            XIDFieldDefinition oFieldDefintion;
                            if (FieldDefinition != null && FieldDefinition.XIDeleted != 1)
                            {
                                XIDFieldOrigin oXIFieldOrigin;
                                if (FieldOrigin != null)
                                {
                                    if (!lookup3.TryGetValue(FieldDefinition.XIGUID, out oFieldDefintion))
                                    {
                                        lookup3.Add(FieldDefinition.XIGUID, oFieldDefintion = FieldDefinition);
                                        oSection.FieldDefs[FieldOrigin.sName] = oFieldDefintion;
                                    }

                                    if (!lookup4.TryGetValue(FieldOrigin.XIGUID, out oXIFieldOrigin))
                                    {

                                        if (FieldOrigin.iMasterDataID > 0)
                                        {
                                            //FieldOrigin.ddlFieldOptionList = dbContext.Types.Where(m => m.Code == FieldOrigin.iMasterDataID).ToList().Select(m => new XIDFieldOptionList { sOptionName = m.Expression, sOptionValue = m.ID.ToString() }).ToList();
                                        }
                                        else if (FieldOrigin.FK1ClickID > 0 || (FieldOrigin.FK1ClickIDXIGUID != null && FieldOrigin.FK1ClickIDXIGUID != Guid.Empty))
                                        {
                                            var o1ClickD = new XID1Click();
                                            if (FieldOrigin.FK1ClickIDXIGUID != null && FieldOrigin.FK1ClickIDXIGUID != Guid.Empty)
                                            {
                                                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, FieldOrigin.FK1ClickIDXIGUID.ToString());
                                            }
                                            else if (FieldOrigin.FK1ClickID > 0)
                                            {
                                                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, FieldOrigin.FK1ClickID.ToString());
                                            }
                                            XIDBO oBOD = new XIDBO();
                                            if (o1ClickD.BOIDXIGUID != null && o1ClickD.BOIDXIGUID != Guid.Empty)
                                            {
                                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOIDXIGUID.ToString());
                                            }
                                            else if (o1ClickD.BOID > 0)
                                            {
                                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                                            }
                                            int iDataSource = oBOD.iDataSource;
                                            Guid DataSourceXIGUID = oBOD.iDataSourceXIGUID;
                                            XIDXI oXID = new XIDXI();
                                            string sConntection = oXID.GetBODataSource(DataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                                            var Connect = new XIDBAPI(sConntection);
                                            o1ClickD.BOD = oBOD;
                                            o1ClickD.Get_1ClickHeadings();
                                            //var bIsPKExists = o1ClickD.TableColumns.ConvertAll(m => m.ToLower()).Contains(oBOD.sPrimaryKey.ToLower());
                                            //if (!bIsPKExists)
                                            //{

                                            //}                                        
                                            var SelFields = string.Join(", ", o1ClickD.TableColumns.ToList());
                                            XIDGroup oGroupD = new XIDGroup();
                                            string FinalString = oGroupD.ConcatanateFields(SelFields, " ");
                                            FinalString = oBOD.sPrimaryKey + "," + FinalString;
                                            var FinalQuery = o1ClickD.AddSelectPart(o1ClickD.Query, FinalString);
                                            //var oBOIns = (DataTable)Connection.ExecuteQuery(o1ClickD.Query);
                                            Dictionary<string, string> DDL = Connect.GetDDLItems(CommandType.Text, FinalQuery, null);
                                            var FKDDL = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                                            //List<Person> List1 = new List<Person>();
                                            var DDList = FKDDL.Where(m => !string.IsNullOrEmpty(m.sOptionName)).GroupBy(m => m.sOptionName).Select(m => m.FirstOrDefault()).ToList().Select(p => new { key = p.sOptionName, value = p.sOptionValue })
                                                    .ToDictionary(x => x.key, x => x.value);
                                            FieldOrigin.sBOSize = oBOD.sSize;
                                        }
                                        else if (FieldOrigin.FKiBOID > 0 || (FieldOrigin.FKiBOIDXIGUID != null && FieldOrigin.FKiBOIDXIGUID != Guid.Empty))
                                        {
                                            var oBOD = new XIDBO();
                                            if (FieldOrigin.FKiBOIDXIGUID != null && FieldOrigin.FKiBOIDXIGUID != Guid.Empty)
                                            {
                                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, FieldOrigin.FKiBOIDXIGUID.ToString());
                                            }
                                            else if (FieldOrigin.FKiBOID > 0)
                                            {
                                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, FieldOrigin.FKiBOID.ToString());
                                            }
                                            string sBODataSource = string.Empty;
                                            if (oBOD != null)
                                            {
                                                FieldOrigin.sBOSize = oBOD.sSize;
                                                //sBODataSource = GetBODataSource(oBOD.iDataSource);
                                                //if (oBOD.Groups.ContainsKey("label"))
                                                //{
                                                //    var oGroupD = oBOD.Groups["label"];
                                                //    var Con = new XIDBAPI(sBODataSource);
                                                //    var LabelGroup = oGroupD.BOFieldNames;
                                                //    if (!string.IsNullOrEmpty(LabelGroup) && oBOD.sSize == "20")
                                                //    {
                                                //        FieldOrigin.sBOSize = oBOD.sSize;
                                                //        string FinalString = oGroupD.ConcatanateFields(LabelGroup, " ");
                                                //        FinalString = oBOD.sPrimaryKey + "," + FinalString;
                                                //        Dictionary<string, string> DDL = Con.SelectDDL(FinalString, oBOD.TableName);
                                                //        var FKDDL = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                                                //        FieldOrigin.FieldDynamicOptionList = new List<XIDFieldOptionList>();
                                                //        FieldOrigin.FieldDynamicOptionList = FKDDL;
                                                //    }
                                                //}
                                            }
                                        }
                                        lookup4.Add(FieldOrigin.XIGUID, oXIFieldOrigin = FieldOrigin);
                                        oXIFieldOrigin = FieldOrigin;
                                    }

                                    XIDFieldOptionList oOptions;
                                    if (OptionList != null)
                                    {
                                        oOptions = OptionList;
                                        if (!lookup6.TryGetValue(OptionList.XIGUID, out oOptions))
                                        {
                                            lookup6.Add(OptionList.XIGUID, oOptions = OptionList);
                                            if (oXIFieldOrigin.FieldOptionList != null && oXIFieldOrigin.FieldOptionList.Count() > 0)
                                            {
                                                oXIFieldOrigin.FieldOptionList.Add(oOptions);
                                            }
                                            else
                                            {
                                                oXIFieldOrigin.FieldOptionList = new List<XIDFieldOptionList>();
                                                oXIFieldOrigin.FieldOptionList.Add(oOptions);
                                            }
                                        }
                                    }
                                    oFieldDefintion.FieldOrigin = oXIFieldOrigin;
                                    FieldOrigin.DataType = DataType;
                                }
                            }
                        }
                        return oStepDefinition;
                    },
                    param
                    ).AsQueryable();
            }
            var StepDef = lookup.Values.FirstOrDefault();
            var QSLinks = GetSectionQSLinks(StepDef.FKiQSDefintionIDXIGUID.ToString());
            if (QSLinks != null)
            {
                if (iStepID > 0)
                {
                    foreach (var item in QSLinks.Steps.Where(m => m.Value.ID == iStepID).ToList())
                    {
                        if (item.Value != null && item.Value.Sections != null && item.Value.Sections.Count() > 0)
                        {
                            foreach (var osec in item.Value.Sections)
                            {
                                if (osec.Value.QSLinks != null && osec.Value.QSLinks.Count() > 0)
                                {
                                    StepDef.Sections.Where(m => m.Value.ID == osec.Value.ID).FirstOrDefault().Value.QSLinks = osec.Value.QSLinks;
                                }
                            }
                        }
                    }
                }
                else if (StepGUID != null && StepGUID != Guid.Empty)
                {
                    foreach (var item in QSLinks.Steps.Where(m => m.Value.XIGUID == StepGUID).ToList())
                    {
                        if (item.Value != null && item.Value.Sections != null && item.Value.Sections.Count() > 0)
                        {
                            foreach (var osec in item.Value.Sections)
                            {
                                if (osec.Value.QSLinks != null && osec.Value.QSLinks.Count() > 0)
                                {
                                    StepDef.Sections.Where(m => m.Value.XIGUID == osec.Value.XIGUID).FirstOrDefault().Value.QSLinks = osec.Value.QSLinks;
                                }
                            }
                        }
                    }
                }

            }

            var SectionContent = GetComponentParamsByStep(StepUID);
            if (SectionContent != null)
            {
                foreach (var item in StepDef.Sections)
                {
                    if (item.Value.ComponentDefinition != null)
                    {
                        StepDef.Sections.Values.Where(m => m.XIGUID == item.Value.XIGUID).FirstOrDefault().ComponentDefinition = item.Value.ComponentDefinition;
                    }
                }
            }
            return StepDef;
        }

        private XIDQSStep GetComponentParamsByStep(string StepUID)
        {
            int iStepID = 0;
            Guid StepGUID = Guid.Empty;
            int.TryParse(StepUID, out iStepID);
            Guid.TryParse(StepUID, out StepGUID);
            string sQSDefinitionQry = string.Empty;
            if (iStepID > 0)
            {
                sQSDefinitionQry = "select * from XIQSStepDefinition_T QSSD " +
                "inner join XIStepSectionDefinition_T Sec on QSSD.ID = Sec.FKiStepDefinitionID " +
                "inner join XIComponentParams_T NVs on sec.ID = NVs.iStepSectionID " +
                "inner join XIComponents_XC_T XC on NVs.FKiComponentID = XC.ID " +
                "where QSSD.ID = @id;";
            }
            else if (StepGUID != null && StepGUID != Guid.Empty)
            {
                sQSDefinitionQry = "select * from XIQSStepDefinition_T QSSD " +
                "inner join XIStepSectionDefinition_T Sec on QSSD.XIGUID = Sec.FKiStepDefinitionIDXIGUID " +
                "inner join XIComponentParams_T NVs on sec.XIGUID = NVs.iStepSectionIDXIGUID " +
                "inner join XIComponents_XC_T XC on NVs.FKiComponentIDXIGUID = XC.XIGUID " +
                "where QSSD.XIGUID = @xiguid;";
            }
            var param = new
            {
                id = iStepID,
                xiguid = StepGUID
            };
            var lookup2 = new Dictionary<Guid, XIDQSStep>();
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                Conn.Open();
                var lookup = new Dictionary<Guid, XIDQS>();
                var lookup3 = new Dictionary<Guid, XIDQSSection>();
                var lookup4 = new Dictionary<Guid, XIDComponent>();
                Conn.Query<XIDQSStep, XIDQSSection, XIDComponentParam, XIDComponent, XIDQSStep>(sQSDefinitionQry,
                    (Step, SectionDef, ComponentParams, Component) =>
                    {
                        XIDQSStep oStepDefinition;
                        if (!lookup2.TryGetValue(Step.XIGUID, out oStepDefinition))
                        {
                            lookup2.Add(Step.XIGUID, oStepDefinition = Step);
                        }
                        XIDQSSection oSection;
                        if (SectionDef != null && SectionDef.XIDeleted != 1)
                        {
                            if (!lookup3.TryGetValue(SectionDef.XIGUID, out oSection))
                            {
                                lookup3.Add(SectionDef.XIGUID, oSection = SectionDef);
                                if (SectionDef.sName != null)
                                {
                                    oStepDefinition.Sections[SectionDef.sName] = SectionDef;
                                }
                                else
                                {
                                    //oStepDefinition.Sections[SectionDef.ID.ToString() + "_Sec"] = SectionDef;
                                    oStepDefinition.Sections[SectionDef.XIGUID.ToString() + "_Sec"] = SectionDef;
                                }

                            }
                            XIDComponent oComponent;
                            if (Component != null)
                            {
                                if (!lookup4.TryGetValue(SectionDef.XIGUID, out oComponent))
                                {
                                    lookup4.Add(SectionDef.XIGUID, oComponent = Component);
                                    oSection.ComponentDefinition = Component;
                                }
                                if (ComponentParams != null)
                                {
                                    if (oSection.ComponentDefinition.Params != null && oSection.ComponentDefinition.Params.Count() > 0)
                                    {
                                        oSection.ComponentDefinition.Params.Add(ComponentParams);
                                    }
                                    else
                                    {
                                        oSection.ComponentDefinition.Params = new List<XIDComponentParam>();
                                        oSection.ComponentDefinition.Params.Add(ComponentParams);
                                    }
                                }
                            }
                        }
                        return oStepDefinition;
                    },
                    param
                    ).AsQueryable();
            }
            var SectionContent = lookup2.Values.FirstOrDefault();
            return SectionContent;
        }

        private XIDQSStep GetQSLinksByStep(int iStepID)
        {
            string sQSDefinitionQry = "select * from XIQSStepDefinition_T QSSD " +
                "inner join XIStepSectionDefinition_T Sec on QSSD.ID = Sec.FKiStepDefinitionID " +
                "left join XIQSLink_T QSlink on sec.ID = QSlink.FKiSectionDefinitionID " +
                "left join XIQSLinkDefinition_T link on QSlink.sCode = link.sCode " +
                "left join XILink_T Xilink on link.FKiXILinkID = Xilink.XiLinkID " +
                "left join XILinkNV_T Xilinknv on Xilink.XILinkID = Xilinknv.XiLinkID " +
                "where QSSD.ID = @id;";
            var param = new
            {
                id = iStepID
            };
            var lookup2 = new Dictionary<Guid, XIDQSStep>();
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                Conn.Open();
                var lookup = new Dictionary<Guid, XIDQS>();
                var lookup3 = new Dictionary<Guid, XIDQSSection>();
                var lookup4 = new Dictionary<Guid, XIQSLink>();
                var lookup5 = new Dictionary<Guid, XIQSLinkDefintion>();
                var lookup6 = new Dictionary<Guid, XILink>();
                Conn.Query<XIDQSStep, XIDQSSection, XIQSLink, XIQSLinkDefintion, XILink, XiLinkNV, XIDQSStep>(sQSDefinitionQry,
                    (Step, SectionDef, QSLink, Link, XILink, XILinkNV) =>
                    {
                        XIDQSStep oStepDefinition = new XIDQSStep();
                        if (Step != null && Step.StatusTypeID == (int)xiEnumSystem.xistatus.xiactive)
                        {
                            if (!lookup2.TryGetValue(Step.XIGUID, out oStepDefinition))
                            {
                                lookup2.Add(Step.XIGUID, oStepDefinition = Step);
                                //oQSDefinition.Steps[oStepDefinition.sName] = oStepDefinition;
                            }
                            XIDQSSection oSection;
                            if (SectionDef != null && SectionDef.XIDeleted != 1)
                            {
                                if (!lookup3.TryGetValue(SectionDef.XIGUID, out oSection))
                                {
                                    lookup3.Add(SectionDef.XIGUID, oSection = SectionDef);
                                    //oStepDefinition.Sections[SectionDef.ID.ToString() + "_Sec"] = SectionDef;
                                    oStepDefinition.Sections[SectionDef.XIGUID.ToString() + "_Sec"] = SectionDef;
                                }
                                XIQSLink oSecQSLink;
                                if (QSLink != null)
                                {
                                    XIQSLinkDefintion oXIDLink;
                                    if (Link != null)
                                    {
                                        if (!lookup4.TryGetValue(QSLink.XIGUID, out oSecQSLink))
                                        {
                                            lookup4.Add(QSLink.XIGUID, oSecQSLink = QSLink);
                                            oStepDefinition.QSLinks[XILink.Name] = QSLink;
                                        }
                                        oXIDLink = Link;
                                        oSecQSLink.XiLink[Link.sName] = Link;
                                        if (XILink != null)
                                        {
                                            XILink oXILink;
                                            oXILink = XILink;
                                            oXIDLink.XiLink[XILink.Name] = XILink;
                                            if (XILinkNV != null)
                                            {
                                                if (oXILink.XiLinkNVs != null && oXILink.XiLinkNVs.Count() > 0)
                                                {
                                                    oXILink.XiLinkNVs.Add(XILinkNV);
                                                }
                                                else
                                                {
                                                    oXILink.XiLinkNVs = new List<XiLinkNV>();
                                                    oXILink.XiLinkNVs.Add(XILinkNV);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return oStepDefinition;
                    },
                    param,
                     splitOn: "id,id,id,xilinkid,id"
                    ).AsQueryable();
            }
            var SectionContent = lookup2.Values.FirstOrDefault();
            return SectionContent;
        }

        #endregion QuestionSet

        #region Component

        public CResult Get_ComponentDefinition(string sName = "", string sUID = "")
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
                XIUtility.IDInsteadOfGUID(sUID, "Component");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                Guid sGUID = Guid.Empty;
                Guid.TryParse(sUID, out sGUID);

                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "sName";
                    PKValue = sName;
                }

                XIDComponent oComponent = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oComponent = Connection.Select<XIDComponent>("XIComponents_XC_T", Params).FirstOrDefault();
                if (oComponent != null)
                {
                    Dictionary<string, object> NVParams = new Dictionary<string, object>();
                    NVParams["FKiComponentIDXIGUID"] = oComponent.XIGUID;
                    oComponent.NVs = Connection.Select<XIDComponentsNV>("XIComponentNVs_T", NVParams).OrderBy(x => x.ID).ToList();
                }
                if (oComponent != null)
                {
                    Dictionary<string, object> ListParams = new Dictionary<string, object>();
                    ListParams["FKiComponentIDXIGUID"] = oComponent.XIGUID;
                    oComponent.Params = Connection.Select<XIDComponentParam>("XIComponentParams_T", ListParams).OrderBy(x => x.ID).ToList();
                }
                if (oComponent != null)
                {
                    Dictionary<string, object> ListParams = new Dictionary<string, object>();
                    ListParams["FKiComponentIDXIGUID"] = oComponent.XIGUID;
                    oComponent.Triggers = Connection.Select<XIDComponentTrigger>("XIComponentTriggers_XCT_T", ListParams).OrderBy(x => x.ID).ToList();
                }
                if (iID == 0 && (!string.IsNullOrEmpty(sUID) && sUID != "0") || (sGUID != null && sGUID != Guid.Empty))
                {
                    oComponent.FKiApplicationID = FKiApplicationID;
                }
                oCResult.oResult = oComponent;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Component definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_ComponentParamsByStep(int iStepID = 0)
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
                int iContextID = 0;
                //Get Section Details
                XIDQSSection oSectionD = null;
                Dictionary<string, object> SecParams = new Dictionary<string, object>();
                SecParams["ID"] = iStepID;//FKiStepDefinitionID
                oSectionD = Connection.Select<XIDQSSection>("XIStepSectionDefinition_T", SecParams).FirstOrDefault();
                Dictionary<string, object> ListParams = new Dictionary<string, object>();
                if (oSectionD != null)
                {
                    iContextID = oSectionD.ID;
                    ListParams["iStepSectionID"] = iContextID;
                }
                else
                {
                    iContextID = iStepID;
                    ListParams["iStepDefinitionID"] = iContextID;
                }
                List<XIDComponentParam> oParams = new List<XIDComponentParam>();
                oParams = Connection.Select<XIDComponentParam>("XIComponentParams_T", ListParams).OrderBy(x => x.ID).ToList();
                oCResult.oResult = oParams;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Component Params By Step" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_ComponentParamsByContext(string sContext, string UID, string ComponentUID)
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
                int iID = 0;
                Guid XIGUID = Guid.Empty;
                int.TryParse(UID, out iID);
                Guid.TryParse(UID, out XIGUID);
                int iComponentID = 0;
                Guid ComponentXIGUID = Guid.Empty;
                int.TryParse(ComponentUID, out iComponentID);
                Guid.TryParse(ComponentUID, out ComponentXIGUID);
                //Get Section Details
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(sContext) && iID > 0)
                {
                    if (sContext.ToLower() == "step" || sContext.ToLower() == "QSStep".ToLower())
                    {
                        Params["iStepDefinitionID"] = iID;
                    }
                    else if (sContext.ToLower() == "section" || sContext.ToLower() == "QSStepSection".ToLower())
                    {
                        Params["iStepSectionID"] = iID;
                    }
                    else if (sContext.ToLower() == "layout")
                    {
                        Params["iLayoutMappingID"] = iID;
                    }
                    else if (sContext.ToLower() == "xilink")
                    {
                        Params["iXiLinkID"] = iID;
                    }
                    else if (sContext.ToLower() == "query")
                    {
                        Params["iQueryID"] = iID;
                    }
                    if (ComponentXIGUID != null && ComponentXIGUID != Guid.Empty)
                    {
                        Params["FKiComponentIDXIGUID"] = ComponentXIGUID;
                    }
                    else if (iComponentID > 0)
                    {
                        Params["FKiComponentID"] = iComponentID;
                    }
                    List<XIDComponentParam> oParams = new List<XIDComponentParam>();
                    oParams = Connection.Select<XIDComponentParam>("XIComponentParams_T", Params).OrderBy(x => x.ID).ToList();
                    oCResult.oResult = oParams;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else if (!string.IsNullOrEmpty(sContext) && XIGUID != null && XIGUID != Guid.Empty)
                {
                    if (sContext.ToLower() == "step" || sContext.ToLower() == "QSStep".ToLower())
                    {
                        Params["iStepDefinitionIDXIGUID"] = XIGUID.ToString();
                    }
                    else if (sContext.ToLower() == "section" || sContext.ToLower() == "QSStepSection".ToLower())
                    {
                        Params["iStepSectionIDXIGUID"] = XIGUID.ToString();
                    }
                    else if (sContext.ToLower() == "layout")
                    {
                        Params["iLayoutMappingIDXIGUID"] = XIGUID.ToString();
                    }
                    else if (sContext.ToLower() == "xilink")
                    {
                        Params["iXiLinkIDXIGUID"] = XIGUID.ToString();
                    }
                    else if (sContext.ToLower() == "query")
                    {
                        Params["iQueryIDXIGUID"] = XIGUID.ToString();
                    }
                    if (ComponentXIGUID != null && ComponentXIGUID != Guid.Empty)
                    {
                        Params["FKiComponentIDXIGUID"] = ComponentXIGUID;
                    }
                    else if (iComponentID > 0)
                    {
                        Params["FKiComponentID"] = iComponentID;
                    }
                    XIIAttribute oAttrI = new XIIAttribute();
                    List<XIDComponentParam> oParams = new List<XIDComponentParam>();
                    oParams = Connection.Select<XIDComponentParam>("XIComponentParams_T", Params).OrderBy(x => x.ID).ToList();
                    foreach (var prm in oParams)
                    {
                        if (!string.IsNullOrEmpty(prm.sValue))
                        {
                            oCR = oAttrI.Check_ValueTypeForMetaIcon(prm.sValue);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                prm.sValueTypeIcon = (string)oCR.oResult;
                            }
                        }
                    }
                    oCResult.oResult = oParams;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }


            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Component Params By Step" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_DataTypeDefinition(string sUID = "")
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
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                XIDDataType oDataTypeD = new XIDDataType();
                Guid XIGUID = Guid.Empty;
                Guid.TryParse(sUID, out XIGUID);
                if (XIGUID != null && XIGUID != Guid.Empty)
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["XIGUID"] = XIGUID;
                    oDataTypeD = Connection.Select<XIDDataType>("XIDataType_T", Params).FirstOrDefault();
                }
                else if (iID > 0)
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["ID"] = iID;
                    oDataTypeD = Connection.Select<XIDDataType>("XIDataType_T", Params).FirstOrDefault();
                }
                if (iID == 0)
                {
                    oDataTypeD.FKiApplicationID = FKiApplicationID;
                }
                oCResult.oResult = oDataTypeD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Component definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        #endregion Component
        #region Document
        public CResult Get_ContentDefinition(string sTemplateID = "", string sTemplateName = "", int iBOID = 0, int iContentType = 0)
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
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started ContentDefinition Loading" });
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
                int iID = 0;
                Guid XIGUID = Guid.Empty;
                XIContentEditors oDocumentContent = null;
                List<XIContentEditors> oDocumentContentDef = new List<XIContentEditors>();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                int.TryParse(sTemplateID, out iID);
                Guid.TryParse(sTemplateID, out XIGUID);
                if (iID > 0)
                {
                    Params["ID"] = iID.ToString();
                }
                else if (XIGUID != null && XIGUID != Guid.Empty)
                {
                    Params["XIGUID"] = XIGUID;
                }
                if (!string.IsNullOrEmpty(sTemplateName))
                {
                    Params["Name"] = sTemplateName;
                }
                if (iBOID != 0)
                {
                    Params["BO"] = iBOID;
                }
                if (iContentType != 0)
                {
                    Params["Type"] = iContentType;
                }
                //Params["OrganizationID"] = iOrgID;
                //Params["ID"] = iContentID;
                oDocumentContent = Connection.Select<XIContentEditors>("XITemplate_T", Params).FirstOrDefault();
                Params = new Dictionary<string, object>();
                if (oDocumentContent != null && oDocumentContent.ID != 0 && iID == 0)
                {
                    iID = oDocumentContent.ID;
                }
                Params["iParentID"] = iID;
                oDocumentContentDef = Connection.Select<XIContentEditors>("XITemplate_T", Params).OrderBy(x => x.ID).ToList();
                List<XIContentEditors> oContentDef = new List<XIContentEditors>();
                if (oDocumentContentDef != null && oDocumentContentDef.Count() > 0)
                {
                    oDocumentContentDef.Insert(0, oDocumentContent);
                }
                else
                {
                    oDocumentContentDef = new List<XIContentEditors>();
                    oDocumentContentDef.Add(oDocumentContent);
                }
                oCResult.oResult = oDocumentContentDef;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }
        #endregion

        #region IOServerDetails

        public CResult Get_IOSServerDetails(int iOrgID, int iServerID = 0)
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
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            try
            {
                XIIOServerDetails oIOServerDetails = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (iServerID > 0)
                {
                    Params["ID"] = iServerID;
                }
                else
                {
                    Params["OrganizationID"] = iOrgID;
                }
                oIOServerDetails = Connection.Select<XIIOServerDetails>("XIIOServerDetails_T", Params).FirstOrDefault();
                oCResult.oResult = oIOServerDetails;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        #endregion IOServerDetails
        #region RightMenuDetails
        public CResult Get_RightMenuDefinition(string MenuName = "")
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
            try
            {
                List<XIMenu> oRightMenus = new List<XIMenu>();

                if (!string.IsNullOrEmpty(MenuName))
                {
                    string sRoleID = string.Empty;
                    if (!string.IsNullOrEmpty(sUserID))
                    {
                        Dictionary<string, object> UserParams = new Dictionary<string, object>();
                        UserParams["UserID"] = sUserID;
                        cConnectionString oConString = new cConnectionString();
                        string sConString = oConString.ConnectionString(sCoreDatabase);
                        XIDBAPI sConnection = new XIDBAPI(sConString);
                        sRoleID = sConnection.SelectString("RoleID", "XIAppUserRoles_AUR_T", UserParams).ToString();
                    }

                    XIMenu oRightMenuTrees = null;
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["RootName"] = MenuName;
                    Params["ParentID"] = "#";
                    if (!string.IsNullOrEmpty(sRoleID))
                    {
                        Params["RoleID"] = sRoleID;
                    }
                    oRightMenuTrees = Connection.Select<XIMenu>("XIMenu_T", Params).FirstOrDefault();
                    string MainID = oRightMenuTrees.XIGUID.ToString();
                    Params = new Dictionary<string, object>();
                    Params["ParentIDXIGUID"] = MainID;
                    Params["StatusTypeID"] = "10";
                    Params["RootName"] = MenuName;
                    if (!string.IsNullOrEmpty(sRoleID))
                    {
                        Params["OrgID"] = iOrgID.ToString();
                        Params["RoleID"] = sRoleID;
                    }
                    oRightMenus = Connection.Select<XIMenu>("XIMenu_T", Params).OrderBy(x => x.ID).ToList().Where(m => (m.StatusTypeID == 10 || m.StatusTypeID == 0) && m.ParentIDXIGUID != null && m.ParentIDXIGUID != Guid.Empty).ToList(); ;
                    oRightMenus = Countdata(oRightMenus, sCoreDatabase, MenuName);
                }

                oCResult.oResult = oRightMenus;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Menu definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public List<XIMenu> CreateHierarchy(List<XIMenu> Menus, string sParentGUID)
        {
            try
            {
                XIMenu oMenuItem = Menus.Where(m => m.XIGUID.ToString() == sParentGUID).SingleOrDefault();
                List<XIMenu> subGroups = Menus.Where(m => m.ParentID == sParentGUID).ToList();
                if (subGroups.Count > 0)
                {
                    oMenuItem.SubGroups = subGroups;
                    subGroups.ForEach(sg => sg.SubGroups = CreateHierarchy(Menus, sg.XIGUID.ToString()));
                    return subGroups;
                }
                return new List<XIMenu>();
            }
            catch (Exception ex)
            {
            }
            return Menus;
        }
        public List<XIMenu> Countdata(List<XIMenu> Menus, string sCoreDatabase, string sMenuName)
        {
            foreach (var items in Menus)
            {
                var ID = items.XIGUID.ToString();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["ParentIDXIGUID"] = ID;
                Params["StatusTypeID"] = "10";
                if (!string.IsNullOrEmpty(sMenuName))
                {
                    Params["RootName"] = sMenuName;
                }
                items.SubGroups = Connection.Select<XIMenu>("XIMenu_T", Params).OrderBy(x => x.ID).ToList();
                if (items.SubGroups.Count() > 0)
                {
                    Countdata(items.SubGroups, sCoreDatabase, sMenuName);
                }
            }
            return Menus;
        }
        public List<XIDInbox> InboxCountdata(List<XIDInbox> Menus, string sCoreDatabase)
        {
            foreach (var items in Menus)
            {
                var ID = items.ID;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["ParentIDXIGUID"] = items.XIGUID.ToString();
                Params["StatusTypeID"] = "10";
                items.SubGroups = Connection.Select<XIDInbox>("XIInbox_T", Params).OrderBy(x => x.ID).ToList();
                if (items.SubGroups.Count() > 0)
                {
                    InboxCountdata(items.SubGroups, sCoreDatabase);
                }
            }
            return Menus;
        }
        #endregion

        public CResult Get_AutoCompleteList(string sUID, string sBOName, List<CNV> oParams = null, int iBOSize = 0, string sReference = "", int iOrgID = 0)
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
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIInfraUsers oUser = new XIInfraUsers();
                var oInfo = oUser.Get_UserInfo();
                var oBOD = new XIDBO();
                string sBODID = string.Empty;
                string s1ClickID = string.Empty;
                string sBO = string.Empty;
                var sType = sUID.Split('_');
                if (sType[0] == "bo")
                {
                    sBODID = sType[1];
                }
                else if (sType[0] == "1click")
                {
                    s1ClickID = sType[1];
                }
                int iBODID;
                int.TryParse(sBODID, out iBODID);
                Guid BOGUID;
                Guid.TryParse(sBODID, out BOGUID);
                if (!string.IsNullOrEmpty(sBOName) && sBOName.Split('_').Length > 1)
                {
                    sBO = sBOName.Split('_')[1];
                }
                int i1ClickID;
                int.TryParse(s1ClickID, out i1ClickID);
                Guid oneclickGUID;
                Guid.TryParse(s1ClickID, out oneclickGUID);
                XID1Click o1ClickD = new XID1Click();
                if (iBODID > 0 || BOGUID != Guid.Empty)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, sBODID, null, null, 0, iOrgID);
                }
                else if (!string.IsNullOrEmpty(sBO) && sType[0] == "bo")
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBO, "0", null, null, 0, iOrgID);
                }
                else if (i1ClickID > 0 || oneclickGUID != Guid.Empty)
                {
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, s1ClickID);
                    if (o1ClickD.BOIDXIGUID != null && o1ClickD.BOIDXIGUID != Guid.Empty)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOIDXIGUID.ToString(), null, null, 0, iOrgID);
                    }
                    else if (o1ClickD.BOID > 0)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString(), null, null, 0, iOrgID);
                    }
                }
                string sBODataSource = string.Empty;
                if (oBOD != null && (iBODID > 0 || !string.IsNullOrEmpty(sBO)) && (!string.IsNullOrEmpty(sType[0]) && sType[0].ToLower() == "bo"))
                {
                    sBODataSource = GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                    if (oBOD.Groups.ContainsKey("label"))
                    {
                        List<XIDFieldOptionList> FKDDL = new List<XIDFieldOptionList>();
                        var oGroupD = oBOD.Groups["label"];
                        var Con = new XIDBAPI(sBODataSource);
                        var LabelGroup = oGroupD.BOFieldNames;
                        if (!string.IsNullOrEmpty(LabelGroup))
                        {
                            string FinalString = oGroupD.ConcatanateFields(LabelGroup, " ");
                            if (oBOD.bUID || sReference == "xiguid")
                            {
                                FinalString = "XIGUID," + FinalString;
                            }
                            else
                            {
                                FinalString = oBOD.sPrimaryKey + "," + FinalString;
                            }
                            string sWhere = string.Empty;
                            List<string> Whrs = new List<string>();
                            if (oParams != null && oParams.Count() > 0 && oParams.Where(m => m.sType == "WhareClause").ToList().Count() > 0)
                            {
                                var WhrParams = oParams.Where(m => m.sType == "WhereClause").ToList();
                                foreach (var whr in WhrParams)
                                {
                                    Whrs.Add(whr.sName + "=" + whr.sValue);
                                }
                            }
                            if (oBOD.iOrgObject == 1)
                            {
                                Whrs.Add("FKiOrgID=" + oInfo.iOrganizationID);
                            }
                            else if (oBOD.iOrgObject == 2)
                            {
                                Whrs.Add("(FKiOrgID=" + oInfo.iOrganizationID + " or FKiOrgID is null)");
                            }
                            if (Whrs != null && Whrs.Count() > 0)
                            {
                                sWhere = string.Join(" and ", Whrs);
                            }
                            Dictionary<string, string> DDL = Con.SelectDDL(FinalString, oBOD.TableName, sWhere);
                            FKDDL = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                        }
                        if (oBOD.sSize == "10")
                        {
                            oCResult.oResult = FKDDL;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else if (oBOD.sSize == "20")
                        {
                            var DDList = FKDDL.Where(m => !string.IsNullOrEmpty(m.sOptionName)).GroupBy(m => m.sOptionName).Select(m => m.FirstOrDefault()).ToList().Select(p => new { key = p.sOptionName, value = p.sOptionValue })
                                .ToDictionary(x => x.key, x => x.value);
                            oCResult.oResult = DDList;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                    }
                }
                else
                {
                    int iDataSource = oBOD.iDataSource;
                    Guid DataSourceXIGUID = oBOD.iDataSourceXIGUID;
                    XIDXI oXID = new XIDXI();
                    string sConntection = oXID.GetBODataSource(DataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                    var Connect = new XIDBAPI(sConntection);
                    o1ClickD.BOD = oBOD;
                    o1ClickD.Get_1ClickHeadings();
                    //var bIsPKExists = o1ClickD.TableColumns.ConvertAll(m => m.ToLower()).Contains(oBOD.sPrimaryKey.ToLower());
                    //if (!bIsPKExists)
                    //{

                    //}    
                    var PrimaryKey = "";
                    if (o1ClickD.bIsMultiBO == true)
                    {
                        if (string.IsNullOrEmpty(o1ClickD.FromBos))
                        {
                            o1ClickD.FromBos = oBOD.TableName;
                        }
                        var fields = "";//"[" + o1ClickD.FromBos + "].";
                        if (o1ClickD != null && o1ClickD.TableColumns != null && o1ClickD.TableColumns.Count > 1)
                        {
                            foreach (var tbcolumns in o1ClickD.TableColumns)
                            {
                                if (tbcolumns.Contains(oBOD.TableName))
                                {
                                    fields += tbcolumns + " ";
                                }
                                else
                                {
                                    fields += "[" + o1ClickD.FromBos + "]." + tbcolumns + " ";
                                    //fields = string.Join(" [" + o1ClickD.FromBos + "].", o1ClickD.TableColumns.ToList());
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(fields))
                        {
                            fields = fields.TrimEnd(' ');
                        }
                        var TableColumns = fields.Split(' ');
                        o1ClickD.TableColumns = TableColumns.ToList();
                        PrimaryKey = "[" + o1ClickD.FromBos + "]." + oBOD.sPrimaryKey;
                    }
                    o1ClickD.TableColumns.Remove("HiddenData");
                    o1ClickD.TableColumns.Remove("[" + oBOD.TableName + "].HiddenData");
                    var SelFields = string.Join(", ", o1ClickD.TableColumns.ToList());
                    XIDGroup oGroupD = new XIDGroup();
                    string FinalString = String.Empty;
                    if ((oBOD.bUID || sReference == "xiguid") && o1ClickD.TableColumns.Contains("XIGUID"))
                    {
                        FinalString = oGroupD.ConcatanateFields(SelFields, " ");
                        FinalString = (o1ClickD.bIsMultiBO == true ? PrimaryKey : "[" + oBOD.TableName + "].XIGUID") + "," + FinalString;
                    }
                    else
                    {
                        FinalString = SelFields;
                    }
                    var FinalQuery = o1ClickD.AddSelectPart(o1ClickD.Query, FinalString);
                    XIDStructure oXIDStructure = new XIDStructure();
                    if (oParams != null && oParams.Count() > 0)
                    {
                        FinalQuery = oXIDStructure.ReplaceExpressionWithCacheValue(FinalQuery, oParams);
                    }
                    if (oBOD.iOrgObject == 1)
                    {
                        FinalQuery = o1ClickD.AddSearchParameters(o1ClickD.Query, "FKiOrgID=" + oInfo.iOrganizationID);
                    }
                    else if (oBOD.iOrgObject == 2)
                    {
                        FinalQuery = o1ClickD.AddSearchParameters(o1ClickD.Query, "(FKiOrgID=" + oInfo.iOrganizationID + " or FKiOrgID is null)");
                    }
                    //var oBOIns = (DataTable)Connection.ExecuteQuery(o1ClickD.Query);
                    Dictionary<string, string> DDL = Connect.GetDDLItems(CommandType.Text, FinalQuery, null);
                    List<XIDFieldOptionList> FKDDL = new List<XIDFieldOptionList>();
                    FKDDL = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                    if (iBOSize != 0)
                    {
                        oBOD.sSize = Convert.ToString(iBOSize);
                    }
                    if (oBOD.sSize == "10")
                    {
                        oCResult.oResult = FKDDL;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                    else if (oBOD.sSize == "20")
                    {
                        var DDList = FKDDL.Where(m => !string.IsNullOrEmpty(m.sOptionName)).GroupBy(m => m.sOptionName).Select(m => m.FirstOrDefault()).ToList().Select(p => new { key = p.sOptionName, value = p.sOptionValue })
                        .ToDictionary(x => x.key, x => x.value);
                        oCResult.oResult = DDList;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }
        public CResult Get_DependencyAutoCompleteSearchList(string sAttribute, string sUID, string sBOName, string sValue, string sParentBO, int iBOSize = 0)
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
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var oBOD = new XIDBO();
                string sBODID = string.Empty;
                string s1ClickID = string.Empty;
                //string sBO = string.Empty;
                var sType = sUID.Split('-');
                sBODID = sType[1];
                int iBODID;
                int.TryParse(sBODID, out iBODID);
                //if (!string.IsNullOrEmpty(sBOName) && sBOName.Split('-').Length > 1)
                //{
                //    sBO = sBOName.Split('-')[1];
                //}
                XID1Click o1ClickD = new XID1Click();
                if (iBODID > 0)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID.ToString());
                }
                else if (!string.IsNullOrEmpty(sBOName))
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, "0");
                }
                string sBODataSource = string.Empty;
                if (oBOD != null)
                {
                    var oAttr = oBOD.Attributes.Values.Where(m => m.sFKBOName == sParentBO).FirstOrDefault();
                    if (oAttr != null)
                    {
                        string sWhrCl = string.Empty;
                        if (!string.IsNullOrEmpty(sValue) && sValue != "-1")
                        {
                            sWhrCl = oAttr.Name + " = " + sValue;
                        }
                        sBODataSource = GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                        if (oBOD.Groups.ContainsKey("label"))
                        {
                            List<XIDFieldOptionList> FKDDL = new List<XIDFieldOptionList>();
                            var oGroupD = oBOD.Groups["label"];
                            var Con = new XIDBAPI(sBODataSource);
                            var LabelGroup = oGroupD.BOFieldNames;
                            if (!string.IsNullOrEmpty(LabelGroup))
                            {
                                string FinalString = oGroupD.ConcatanateFields(LabelGroup, " ");
                                FinalString = oBOD.sPrimaryKey + "," + FinalString;
                                Dictionary<string, string> DDL = Con.SelectDDL(FinalString, oBOD.TableName, sWhrCl);
                                FKDDL = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                            }
                            if (oBOD.sSize == "10")
                            {
                                oCResult.oResult = FKDDL;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                            else if (oBOD.sSize == "20")
                            {
                                var DDList = FKDDL.Where(m => !string.IsNullOrEmpty(m.sOptionName)).GroupBy(m => m.sOptionName).Select(m => m.FirstOrDefault()).ToList().Select(p => new { key = p.sOptionName, value = p.sOptionValue })
                                    .ToDictionary(x => x.key, x => x.value);
                                oCResult.oResult = DDList;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public CResult Get_SourceDefinition(string sUID = "")
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
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                XIDSource oSourceD = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oSourceD = Connection.Select<XIDSource>("XISource_T", Params).FirstOrDefault();
                oCResult.oResult = oSourceD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Source definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_ClassDefinition(string sUID = "")
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
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                XIDClass oSourceD = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oSourceD = Connection.Select<XIDClass>("XIClass_T", Params).FirstOrDefault();
                oCResult.oResult = oSourceD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Class definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        #region BOStructureDefination

        public CResult Get_XIBOStructureDefinition(string sBOID, string sStructureID, string sType)
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
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started ContentDefinition Loading" });
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
                List<XIDStructure> XIStrTrees = new List<XIDStructure>();
                List<XIDStructure> Tree = new List<XIDStructure>();

                //QuestionSet Steps DropDown
                Dictionary<string, object> Params5 = new Dictionary<string, object>();
                Dictionary<string, string> QSSteps = new Dictionary<string, string>();
                var oStepDef = Connection.Select<XIDQSStep>("XIQSStepDefinition_T", Params5).OrderBy(x => x.ID).ToList();

                //All BO DropDowns
                List<XIDropDown> oBOD = new List<XIDropDown>();
                Dictionary<string, object> oBOParam = new Dictionary<string, object>();
                var oBODef = Connection.Select<XIDBO>("XIBO_T_N", oBOParam).OrderBy(x => x.BOID).ToList();

                Dictionary<string, object> oAttrParam = new Dictionary<string, object>();
                var oAttrDef = Connection.Select<XIDAttribute>("XIBOAttribute_T_N", oAttrParam).OrderBy(x => x.ID).ToList();
                int iBOID = 0;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (sType != "Create")
                {
                    Params["FKiParentID"] = '#';
                    if (!string.IsNullOrEmpty(sStructureID))
                    {
                        int iStrctID = 0;
                        if (int.TryParse(sStructureID, out iStrctID))
                        {
                            Params["ID"] = iStrctID.ToString();
                        }
                        else
                        {
                            Params["XIGUID"] = sStructureID;
                        }
                        XIStrTrees = Connection.Select<XIDStructure>("XIBOStructure_T", Params).OrderBy(x => x.ID).ToList();
                    }
                    string sPK = "BOID";
                    if (!string.IsNullOrEmpty(sBOID))
                    {
                        if (int.TryParse(sBOID, out iBOID))
                        {
                            Params["BOID"] = iBOID;
                        }
                        else
                        {
                            sPK = "XIGUID";
                            Params["BOIDXIGUID"] = sBOID;
                        }
                        XIStrTrees = Connection.Select<XIDStructure>("XIBOStructure_T", Params).OrderBy(x => x.ID).ToList();
                    }

                    if (Tree.Count() == 0)
                    {
                        Tree.Add(new XIDStructure());
                    }

                    foreach (var item in XIStrTrees)
                    {
                        item.FKiStepDefinitionName = oStepDef.Where(m => m.ID == Convert.ToInt32(item.FKiStepDefinitionID)).Select(m => m.sName).FirstOrDefault();
                    }

                    if (XIStrTrees != null && XIStrTrees.Count() > 0)
                    {
                        Tree = XITree(XIStrTrees, new List<XIDStructure>());
                    }
                }

                if (sType == "Create")
                {
                    List<XIDAttribute> BOFldList = new List<XIDAttribute>();
                    var oBO = new XIDBO();
                    if (int.TryParse(sBOID, out iBOID))
                    {
                        oBO = oBODef.Where(m => m.BOID == iBOID).FirstOrDefault();
                    }
                    else
                    {
                        Guid sGUID = new Guid();
                        if (Guid.TryParse(sBOID, out sGUID))
                        {
                            oBO = oBODef.Where(m => m.XIGUID == sGUID).FirstOrDefault();
                        }
                    }
                    BOFldList = oAttrDef.Where(m => m.FKTableName == oBO.TableName).ToList();
                    Tree.Add(new XIDStructure { ID = oBO.BOID, XIGUID = oBO.XIGUID, FKiParentID = "#", sName = oBO.LabelName, sBO = oBO.Name, BOID = oBO.BOID, BOIDXIGUID = oBO.XIGUID });
                    foreach (var items in BOFldList)
                    {
                        var BO = oBODef.Where(m => m.BOID == items.BOID).FirstOrDefault();
                        if (BO != null)
                        {
                            if (Tree.Where(m => m.ID == items.BOID).FirstOrDefault() == null)
                            {
                                if (BO.sType != xiBOTypes.Reference.ToString() && BO.sType != xiBOTypes.Enum.ToString() && BO.sType != xiBOTypes.Technical.ToString())
                                {
                                    XIDStructure oStru = new XIDStructure();
                                    oStru.ID = items.BOID;
                                    oStru.XIGUID = items.XIGUID;
                                    oStru.sBO = BO.Name;
                                    oStru.sName = BO.LabelName;
                                    oStru.FKiParentID = oBO.BOID.ToString();
                                    oStru.FKiParentIDXIGUID = oBO.XIGUID;
                                    oStru.BOID = BO.BOID;
                                    Tree.Add(oStru);
                                }
                            }
                        }
                    }
                }

                //Adding All Steps's to Tree
                foreach (var items in oStepDef)
                {
                    if (!string.IsNullOrEmpty(items.sName))
                    {
                        QSSteps[items.sName] = items.XIGUID.ToString();
                    }
                }
                Tree.FirstOrDefault().AllQSSteps = QSSteps;
                //Adding All BO's to Tree
                oBOD = oBODef.Select(m => new XIDropDown { Value = m.BOID, text = m.LabelName, sGUID = m.XIGUID.ToString() }).ToList();
                Tree.FirstOrDefault().BOList = oBOD;

                oCResult.oResult = Tree;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public List<XIDStructure> XITree(List<XIDStructure> XIStrTrees, List<XIDStructure> Tree)
        {
            foreach (var items in XIStrTrees)
            {
                Tree.Add(items);
                var ID = items.ID;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["FKiParentIDXIGUID"] = items.XIGUID.ToString();
                var oStruDef = Connection.Select<XIDStructure>("XIBOStructure_T", Params).OrderBy(x => x.ID).ToList();
                var SubXITreeNodes = oStruDef.OrderBy(m => m.iOrder).ToList();
                if (SubXITreeNodes.Count() > 0)
                {
                    XITree(SubXITreeNodes, Tree);
                }
            }
            return Tree;
        }

        #endregion BOStructureDefination

        public CResult Get_FieldOriginDefinition(string sName, string sUID = "")
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
                XIUtility.IDInsteadOfGUID(sUID, "FieldOrigin");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID))
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "sName";
                    PKValue = sName;
                }
                XIDFieldOrigin oFOrgin = new XIDFieldOrigin();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                if (PKValue != "0")
                {
                    oFOrgin = Connection.Select<XIDFieldOrigin>("XIFieldOrigin_T", Params).FirstOrDefault();
                }
                if (oFOrgin != null && oFOrgin.bIsOptionList)
                {
                    Dictionary<string, object> OptParam = new Dictionary<string, object>();
                    OptParam["FKiQSFieldIDXIGUID"] = oFOrgin.XIGUID.ToString();
                    oFOrgin.FieldOptionList = Connection.Select<XIDFieldOptionList>("XIFieldOptionList_T", OptParam).OrderBy(x => x.ID).ToList();
                }
                oCResult.oResult = oFOrgin;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading field origin definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        #region Section Definition

        public CResult Get_QSSectionDefinition(string sUID = "")
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
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID))
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                XIDQSSection oSecD = new XIDQSSection();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                if (PKValue != "0")
                {
                    oSecD = Connection.Select<XIDQSSection>("XIStepSectionDefinition_T", Params).FirstOrDefault();
                }
                oCResult.oResult = oSecD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading field origin definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_QSSectionsAll()
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
                List<XIDQSSection> oSecD = new List<XIDQSSection>();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[XIConstant.Key_XIDeleted] = "0";
                oSecD = Connection.Select<XIDQSSection>("XIStepSectionDefinition_T", Params).OrderBy(x => x.ID).ToList();
                oCResult.oResult = oSecD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading field origin definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_QSSectionConfiguration(string sUID = "")
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
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID))
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                XIDQSSection oSecD = new XIDQSSection();
                if (iID > 0 || !string.IsNullOrEmpty(sUID))
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params[PKColumn] = PKValue;
                    oSecD = Connection.Select<XIDQSSection>("XIStepSectionDefinition_T", Params).FirstOrDefault();
                    if (oSecD != null)
                    {
                        Params = new Dictionary<string, object>();
                        Params["FKiStepSectionIDXIGUID"] = oSecD.XIGUID.ToString();
                        Params[XIConstant.Key_XIDeleted] = "0";
                        var FieldDefs = Connection.Select<XIDFieldDefinition>("XIFieldDefinition_T", Params).ToList();
                        if (FieldDefs != null && FieldDefs.Count() > 0)
                        {
                            foreach (var def in FieldDefs)
                            {
                                Params = new Dictionary<string, object>();
                                Params["XIGUID"] = def.FKiXIFieldOriginIDXIGUID.ToString();
                                var FieldOrigin = Connection.Select<XIDFieldOrigin>("XIFieldOrigin_T", Params).FirstOrDefault();
                                def.FieldOrigin = FieldOrigin;
                            }
                        }
                        oSecD.FieldDefinitions = FieldDefs;
                    }
                }
                oCResult.oResult = oSecD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading field origin definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        #endregion Section Definition

        #region BOAction

        public CResult Get_BOActionDefinition(string sName, string sUID = "")
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
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID))
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "sName";
                    PKValue = sName;
                }
                XIDBOAction oBOAction = new XIDBOAction();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                if (PKValue != "0")
                {
                    oBOAction = Connection.Select<XIDBOAction>("XIBOAction_T", Params).FirstOrDefault();
                    if (oBOAction != null)
                    {
                        Params = new Dictionary<string, object>();
                        Params["FKiBOActionIDXIGUID"] = oBOAction.XIGUID;
                        var oBOActionNV = Connection.Select<XIDBOActionNV>("XIBOActionNV_T", Params).OrderBy(x => x.ID).ToList();
                        if (oBOActionNV != null && oBOActionNV.Count() > 0)
                        {
                            oBOAction.ActionNV = oBOActionNV;
                        }
                    }
                }
                oCResult.oResult = oBOAction;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading field origin definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        #endregion BOAction

        #region BODefault

        public CResult Get_BODefault(string sName, string sUID = "")
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
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "FKiBOID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID))
                {
                    PKColumn = "FKiBOIDXIGUID";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "sName";
                    PKValue = sName;
                }
                XIDBODefault BODefault = new XIDBODefault();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                if (PKValue != "0")
                {
                    BODefault = Connection.Select<XIDBODefault>("XIBOUIDefault_T", Params).FirstOrDefault();
                }
                oCResult.oResult = BODefault;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading field origin definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        #endregion BODefault

        #region XIWidget

        public CResult Get_WidgetDefinition(string sName, string sUID = "")
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
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID))
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "sName";
                    PKValue = sName;
                }
                XIDWidget oWidgetD = new XIDWidget();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                if (PKValue != "0")
                {
                    oWidgetD = Connection.Select<XIDWidget>("XIWidget_T", Params).FirstOrDefault();
                }
                oCResult.oResult = oWidgetD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading field origin definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        #endregion XIWidget

        #region Algorithm

        public CResult Get_XIAlgorithmDefinition(string sName, string sUID = "")
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
                XIUtility.IDInsteadOfGUID(sUID, "Algorithm");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                Guid guidOutput;
                bool isValidGUID = Guid.TryParse(sUID, out guidOutput);
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (isValidGUID)
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "sName";
                    PKValue = sName;
                }
                XIDAlgorithm oAlgoD = new XIDAlgorithm();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                if (PKValue != "0")
                {
                    oAlgoD = Connection.Select<XIDAlgorithm>("XIAlgorithm_T", Params).FirstOrDefault();
                    if (oAlgoD.ID > 0)
                    {
                        Params = new Dictionary<string, object>();
                        XIInfraCache oCache = new XIInfraCache();
                        XIInfraUsers oUser = new XIInfraUsers();
                        var oInfo = oUser.Get_UserInfo();
                        var sIDRef = "";
                        if (oInfo != null)
                        {
                            //if (oInfo.iRoleID != 55)
                            //{
                            //    sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, oInfo.iApplicationID + "_" + oInfo.iOrganizationID + "_" + oInfo.iRoleID + "_" + XIConstant.IDRef_internal);
                            //}
                            //else
                            //{
                            //    sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, oInfo.iApplicationID + "_" + oInfo.iOrganizationID + "_" + XIConstant.IDRef_public);
                            //}
                        }

                        //if (!string.IsNullOrEmpty(sIDRef) && sIDRef.ToLower() == "xiguid")
                        if (isValidGUID)
                        {
                            Params["FKiAlgorithmIDXIGUID"] = oAlgoD.XIGUID.ToString();
                        }
                        else
                        {
                            Params["FKiAlgorithmID"] = oAlgoD.ID;
                        }
                        Params[XIConstant.Key_XIDeleted] = 0;
                        var Lines = Connection.Select<XIDAlgorithmLine>("XIAlgorithmLines_T", Params).OrderBy(x => x.ID).ToList();
                        if (Lines != null && Lines.Count() > 0)
                        {
                            oAlgoD.Lines = new List<XIDAlgorithmLine>();
                            oAlgoD.Lines = Lines;
                        }
                    }
                }
                oCResult.oResult = oAlgoD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading xialgorithm definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        #endregion Algorithm

        #region Autocomplete

        public CResult Get_XILinks(int iApplicationID)
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
                oTrace.oParams.Add(new CNV { sName = "iApplicationID", sValue = iApplicationID.ToString() });
                if (iApplicationID > 0)//check mandatory params are passed or not
                {
                    Dictionary<string, object> oLinks = new Dictionary<string, object>();
                    oLinks["FKiApplicationID"] = iApplicationID;
                    Dictionary<string, string> XiLinks = new Dictionary<string, string>();
                    var oXiLinkDef = Connection.Select<XILink>("XILink_T", oLinks).OrderBy(x => x.XiLinkID).ToList();
                    foreach (var items in oXiLinkDef)
                    {
                        XiLinks[items.Name] = items.Name;
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = XiLinks;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: ApplicationID is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Get_QuestionSets(int iApplicationID)
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
                oTrace.oParams.Add(new CNV { sName = "iApplicationID", sValue = iApplicationID.ToString() });
                if (iApplicationID > 0)//check mandatory params are passed or not
                {
                    List<XIDropDown> DDL = new List<XIDropDown>();
                    Dictionary<string, object> oLinks = new Dictionary<string, object>();
                    oLinks["FKiApplicationID"] = iApplicationID;
                    var oXiLinkDef = Connection.Select<XIDQS>("XIQSDefinition_T", oLinks).OrderBy(x => x.ID).ToList();
                    foreach (var items in oXiLinkDef)
                    {
                        DDL.Add(new XIDropDown { text = items.sName, Value = items.ID });
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = DDL;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: ApplicationID is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Get_QSSteps(int iQSDID)
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
                oTrace.oParams.Add(new CNV { sName = "iQSDID", sValue = iQSDID.ToString() });
                if (iQSDID > 0)//check mandatory params are passed or not
                {
                    List<XIDropDown> DDL = new List<XIDropDown>();
                    Dictionary<string, object> oLinks = new Dictionary<string, object>();
                    oLinks["FKiQSDefintionID"] = iQSDID;
                    var oXiLinkDef = Connection.Select<XIDQSStep>("XIQSStepDefinition_T", oLinks).OrderBy(x => x.ID).ToList();
                    foreach (var items in oXiLinkDef)
                    {
                        DDL.Add(new XIDropDown { text = items.sName, Value = items.ID });
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = DDL;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: QSDID is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Get_QSSections(int iStepID)
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
                oTrace.oParams.Add(new CNV { sName = "iStepID", sValue = iStepID.ToString() });
                if (iStepID > 0)//check mandatory params are passed or not
                {
                    List<XIDropDown> DDL = new List<XIDropDown>();
                    Dictionary<string, object> oLinks = new Dictionary<string, object>();
                    oLinks["FKiStepDefinitionID"] = iStepID;
                    var oXiLinkDef = Connection.Select<XIDQSSection>("XIStepSectionDefinition_T", oLinks).OrderBy(x => x.ID).ToList();
                    foreach (var items in oXiLinkDef)
                    {
                        if (!string.IsNullOrEmpty(items.sName))
                        {
                            DDL.Add(new XIDropDown { text = items.sName, Value = items.ID });
                        }
                        else
                        {
                            DDL.Add(new XIDropDown { text = items.ID.ToString(), Value = items.ID });
                        }
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = DDL;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: iStepID is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Get_QSLinks(int iApplicationID)
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
                oTrace.oParams.Add(new CNV { sName = "iApplicationID", sValue = iApplicationID.ToString() });
                if (iApplicationID > 0)//check mandatory params are passed or not
                {
                    List<XIDropDown> DDL = new List<XIDropDown>();
                    Dictionary<string, object> oLinks = new Dictionary<string, object>();
                    oLinks["FKiApplicationID"] = iApplicationID;
                    oLinks[XIConstant.Key_XIDeleted] = "0";
                    var oXiLinkDef = Connection.Select<XIQSLinkDefintion>("XIQSLinkDefinition_T", oLinks).OrderBy(x => x.ID).ToList();
                    foreach (var items in oXiLinkDef)
                    {
                        DDL.Add(new XIDropDown { text = items.sName, Value = Convert.ToInt32(items.ID) });
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = DDL;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: ApplicationID is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        #endregion Autocomplete

        #region WhiteList

        public CResult Get_WhiteList()
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
                XIInfraCache oCache = new XIInfraCache();
                Dictionary<string, object> Data = new Dictionary<string, object>();
                var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "BO WhiteList Cache");
                var o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                var Result = o1ClickC.OneClick_Run();
                if (Result != null && Result.Count() > 0)
                {
                    foreach (var items in Result.Values.ToList())
                    {
                        var iBODID = items.AttributeI("FKiBODID").sValue;
                        var iRoleID = items.AttributeI("FKiRoleID").sValue;
                        var iOrgID = items.AttributeI("FKiOrgID").sValue;
                        var iAppID = items.AttributeI("FKiAppID").sValue;
                        if (!string.IsNullOrEmpty(iBODID) && !string.IsNullOrEmpty(iRoleID) && !string.IsNullOrEmpty(iOrgID) && !string.IsNullOrEmpty(iAppID))
                        {
                            Data[iBODID + "_" + iRoleID + "_" + iOrgID + "_" + iAppID] = items;
                        }
                    }
                }
                oCResult.oResult = Data;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
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

        #endregion WhiteList

        #region Config Setting

        public CResult Get_ConfigSetting(string sUID)
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
                int iAppID = 0;
                int iOrgID = 0;
                int iRoleID = 0;
                string sKey = string.Empty;
                var Splits = sUID.Split('_');
                if (Splits.Count() == 2)
                {
                    var AppID = Splits[0];
                    sKey = Splits[1];
                    int.TryParse(AppID, out iAppID);
                }
                else if (Splits.Count() == 3)
                {
                    var AppID = Splits[0];
                    var OrgID = Splits[1];
                    sKey = Splits[2];
                    int.TryParse(AppID, out iAppID);
                    int.TryParse(OrgID, out iOrgID);
                }
                else if (Splits.Count() == 4)
                {
                    var AppID = Splits[0];
                    var OrgID = Splits[1];
                    var RoleID = Splits[2];
                    sKey = Splits[3];
                    int.TryParse(AppID, out iAppID);
                    int.TryParse(OrgID, out iOrgID);
                    int.TryParse(RoleID, out iRoleID);
                }
                else
                {
                    sKey = sUID;
                }
                if (!string.IsNullOrEmpty(sKey))
                {
                    XIIXI oXII = new XIIXI();
                    //List<CNV> oWhrPrms = new List<CNV>();
                    //oWhrPrms.Add(new CNV { sName = "FKiAppID", sValue = iAppID.ToString() });
                    //oWhrPrms.Add(new CNV { sName = "FKiOrgID", sValue = iOrgID.ToString() });
                    //oWhrPrms.Add(new CNV { sName = "sKey", sValue = sKey });
                    Dictionary<string, object> oWhrPrms = new Dictionary<string, object>();
                    if (iAppID > 0)
                        oWhrPrms["FKiAppID"] = iAppID;

                    if (iOrgID > 0)
                        oWhrPrms["FKiOrgID"] = iOrgID;
                    if (iRoleID > 0)
                    {
                        oWhrPrms["FKiRoleID"] = iRoleID;
                    }
                    oWhrPrms["sKey"] = sKey;
                    var sValue = Connection.SelectString("svalue", "XIConfig_T", oWhrPrms);
                    //var oBOI = oXII.BOI("XIConfig_T", null, null, oWhrPrms);
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        oCResult.oResult = sValue;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    }
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

        #endregion Config Setting

        #region 1Link Access

        public CResult Get_1LinkAccess()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Get list of Link access data to add into cache";//expalin about this method logic
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                Dictionary<string, object> Data = new Dictionary<string, object>();
                var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "1Link Access Cache");
                var o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                var Result = o1ClickC.OneClick_Run();
                if (Result != null && Result.Count() > 0)
                {
                    foreach (var items in Result.Values.ToList())
                    {
                        var iXILinkID = items.AttributeI("FKiXILinkID").sValue;
                        var iRoleID = items.AttributeI("FKiRoleID").sValue;
                        var iOrgID = items.AttributeI("FKiOrgID").sValue;
                        var iAppID = items.AttributeI("FKiAppID").sValue;
                        if (!string.IsNullOrEmpty(iXILinkID) && !string.IsNullOrEmpty(iRoleID) && !string.IsNullOrEmpty(iOrgID) && !string.IsNullOrEmpty(iAppID))
                        {
                            Data[iXILinkID + "_" + iRoleID + "_" + iOrgID + "_" + iAppID] = items;
                        }
                    }
                }
                oCResult.oResult = Data;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
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

        #endregion 1Link Access

        #region 1Query Access

        public CResult Get_1QueryAccess()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Get list of 1Query access data to add into cache";//expalin about this method logic
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                Dictionary<string, object> Data = new Dictionary<string, object>();
                var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "1Query Permission Cache");
                var o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                var Result = o1ClickC.OneClick_Run();
                if (Result != null && Result.Count() > 0)
                {
                    foreach (var items in Result.Values.ToList())
                    {
                        var i1ClickID = items.AttributeI("FKi1ClickID").sValue;
                        var iRoleID = items.AttributeI("FKiRoleID").sValue;
                        var iOrgID = items.AttributeI("FKiOrgID").sValue;
                        var iAppID = items.AttributeI("FKiAppID").sValue;
                        if (!string.IsNullOrEmpty(i1ClickID) && !string.IsNullOrEmpty(iRoleID) && !string.IsNullOrEmpty(iOrgID) && !string.IsNullOrEmpty(iAppID))
                        {
                            Data[i1ClickID + "_" + iRoleID + "_" + iOrgID + "_" + iAppID] = items;
                        }
                    }
                }
                oCResult.oResult = Data;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
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

        #endregion 1Query Access

        #region SendGrid
        public CResult Get_SendGridAccountDetails(string sSendGridAccountID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            long iTraceLevel = 10;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";
            try
            {
                if (!string.IsNullOrEmpty(sSendGridAccountID))
                {
                    Dictionary<string, object> oParams = new Dictionary<string, object>();
                    oParams["iSGADID"] = sSendGridAccountID;
                    XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
                    var oResult = Connection.Select<XIDSendGridAccountDetails>("SendGridAccountDetails_T", oParams);
                    if (oResult.Count() > 0)
                    {
                        oCResult.oResult = oResult.FirstOrDefault();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

                    }
                    else
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;

                }

            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult Get_SendGridTemplate(string sTemplateName, string UID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            long iTraceLevel = 10;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";
            try
            {
                var oParams = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(sTemplateName))
                {
                    oParams["sTemplateName"] = sTemplateName;
                }
                else if (!string.IsNullOrEmpty(UID))
                {
                    int iID = 0;
                    Guid TempGUID = Guid.Empty;
                    if (Guid.TryParse(UID, out TempGUID))
                    {
                        oParams["XIGUID"] = TempGUID;
                    }
                    else if (int.TryParse(UID, out iID))
                    {
                        oParams["iSGTID"] = iID.ToString();
                    }
                }
                if (oParams.Count() > 0)
                {
                    var oTemplateResult = Connection.Select<XIDSendGridTemplate>("SendGridTemplate_T", oParams);
                    if (oTemplateResult.Count() > 0)
                    {
                        oCResult.oResult = oTemplateResult.FirstOrDefault();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                }
                else
                {
                    oCResult.oResult = new XIDSendGridTemplate();
                }
                //oParams["FKiSGADID"] = SendGridAccountID;
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        #endregion SendGrid

        #region FK Auto Complete

        public CResult Get_StepsForAutoComplete(string sSearchText, string sStepID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Get all steps list";//expalin about this method logic
            try
            {
                Dictionary<string, object> Params = new Dictionary<string, object>();
                int iStepID = 0; Guid guidOutput;
                bool isValid = Guid.TryParse(sStepID, out guidOutput);
                int.TryParse(sStepID, out iStepID);
                if (iStepID > 0)
                {
                    Params["ID"] = iStepID;
                }
                else if (isValid && guidOutput != Guid.Empty)
                {
                    Params["XIGUID"] = sStepID;
                }
                var oStepDef = Connection.Select<XIDQSStep>("XIQSStepDefinition_T", Params).OrderBy(x => x.ID).ToList();
                List<CNV> QSSteps = new List<CNV>();
                if (!string.IsNullOrEmpty(sSearchText))
                {
                    var FilteredSteps = oStepDef.Where(m => m.sName != null && m.sName.ToLower().Contains(sSearchText.ToLower())).ToList();
                    foreach (var items in FilteredSteps)
                    {
                        QSSteps.Add(new CNV() { sName = items.ID + " " + items.sName, sValue = items.XIGUID.ToString() });
                    }
                }
                else if (iStepID > 0)
                {
                    var FilteredSteps = oStepDef.Where(m => m.ID == iStepID).ToList();
                    foreach (var items in FilteredSteps)
                    {
                        QSSteps.Add(new CNV() { sName = items.ID + " " + items.sName, sValue = items.XIGUID.ToString() });
                    }
                }
                else if (isValid && guidOutput != Guid.Empty)
                {
                    var FilteredSteps = oStepDef.Where(m => m.XIGUID == guidOutput).ToList();
                    foreach (var items in FilteredSteps)
                    {
                        QSSteps.Add(new CNV() { sName = items.ID + " " + items.sName, sValue = items.XIGUID.ToString() });
                    }
                }
                else
                {
                    foreach (var items in oStepDef)
                    {
                        QSSteps.Add(new CNV() { sName = items.ID + " " + items.sName, sValue = items.XIGUID.ToString() });
                    }
                }
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = QSSteps;
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
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

        public CResult Get_XILinksForAutoComplete(string AppUID, string sSearchText, string sXILinkID)
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
                int iApplicationID = 0;
                Guid AppGUID = Guid.Empty;
                int.TryParse(AppUID, out iApplicationID);
                Guid.TryParse(AppUID, out AppGUID);
                oTrace.oParams.Add(new CNV { sName = "iApplicationID", sValue = iApplicationID.ToString() });
                if (iApplicationID > 0 || (AppGUID != null && AppGUID != Guid.Empty))//check mandatory params are passed or not
                {
                    Dictionary<string, object> oLinks = new Dictionary<string, object>();
                    if (iApplicationID > 0)
                        oLinks["FKiApplicationID"] = iApplicationID;
                    else if (AppGUID != null && AppGUID != Guid.Empty)
                        oLinks["FKiApplicationIDXIGUID"] = AppGUID;
                    int iXILinkID = 0; Guid guidOutput;
                    bool isValid = Guid.TryParse(sXILinkID, out guidOutput);
                    if (int.TryParse(sXILinkID, out iXILinkID) && iXILinkID > 0)
                    {
                        oLinks["XiLinkID"] = iXILinkID;
                    }
                    else if (isValid && guidOutput != Guid.Empty)
                    {
                        oLinks["XIGUID"] = sXILinkID;
                    }
                    Dictionary<string, string> XiLinks = new Dictionary<string, string>();
                    var oXiLinkDef = Connection.Select<XILink>("XILink_T", oLinks).OrderBy(x => x.XiLinkID).ToList();
                    if (!string.IsNullOrEmpty(sSearchText))
                    {
                        var FilterItems = oXiLinkDef.Where(m => m.Name.ToLower().Contains(sSearchText.ToLower())).ToList();
                        foreach (var items in FilterItems)
                        {
                            XiLinks[items.XiLinkID.ToString() + " " + items.Name] = items.XIGUID.ToString();
                        }
                    }
                    else if (iXILinkID > 0)
                    {
                        var FilterItems = oXiLinkDef.Where(m => m.XiLinkID == iXILinkID).ToList();
                        foreach (var items in FilterItems)
                        {
                            XiLinks[items.XiLinkID.ToString() + " " + items.Name] = items.XIGUID.ToString();
                        }
                    }
                    else if (isValid && guidOutput != Guid.Empty)
                    {
                        var FilterItems = oXiLinkDef.Where(m => m.XIGUID == guidOutput).ToList();
                        foreach (var items in FilterItems)
                        {
                            XiLinks[items.XiLinkID.ToString() + " " + items.Name] = items.XIGUID.ToString();
                        }
                    }
                    else
                    {
                        foreach (var items in oXiLinkDef)
                        {
                            XiLinks[items.XiLinkID.ToString() + " " + items.Name] = items.XIGUID.ToString();
                        }
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = XiLinks;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: ApplicationID is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Get_LayoutsForAutoComplete(string sSearchText, string sLayoutID, string AppUID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Get all steps list";//expalin about this method logic
            try
            {
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Guid guidOutput; int iLayoutID = 0;
                bool isValid = Guid.TryParse(sLayoutID, out guidOutput);
                if (int.TryParse(sLayoutID, out iLayoutID) && iLayoutID > 0)
                {
                    Params["ID"] = iLayoutID;
                }
                else if (isValid && guidOutput != Guid.Empty)
                {
                    Params["XIGUID"] = sLayoutID;
                }
                Params["FKiApplicationIDXIGUID"] = AppUID;
                var oLayoutDef = Connection.Select<XIDLayout>("XILayout_T", Params).OrderBy(x => x.ID).ToList();
                List<CNV> Layouts = new List<CNV>();
                if (!string.IsNullOrEmpty(sSearchText))
                {
                    var FilteredLayouts = oLayoutDef.Where(m => m.LayoutName.ToLower().Contains(sSearchText.ToLower())).ToList();
                    foreach (var items in FilteredLayouts)
                    {
                        Layouts.Add(new CNV() { sName = items.ID + " " + items.LayoutName, sValue = items.XIGUID.ToString() });
                    }
                }
                else if (iLayoutID > 0)
                {
                    var FilteredLayouts = oLayoutDef.Where(m => m.ID == iLayoutID).ToList();
                    foreach (var items in FilteredLayouts)
                    {
                        Layouts.Add(new CNV() { sName = items.ID + " " + items.LayoutName, sValue = items.XIGUID.ToString() });
                    }
                }
                else if (isValid && guidOutput != Guid.Empty)
                {
                    var FilteredLayouts = oLayoutDef.Where(m => m.XIGUID == guidOutput).ToList();
                    foreach (var items in FilteredLayouts)
                    {
                        Layouts.Add(new CNV() { sName = items.ID + " " + items.LayoutName, sValue = items.XIGUID.ToString() });
                    }
                }
                else
                {
                    foreach (var items in oLayoutDef)
                    {
                        Layouts.Add(new CNV() { sName = items.ID + " " + items.LayoutName, sValue = items.XIGUID.ToString() });
                    }
                }
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = Layouts;
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
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

        public CResult Get_BOsForAutoComplete(string sSearchText, string sBO, string AppUID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Get all BOs list";//expalin about this method logic
            try
            {
                Dictionary<string, object> Params = new Dictionary<string, object>();
                int iBODID = 0; Guid guidOutput;
                bool isValid = Guid.TryParse(sBO, out guidOutput);
                int.TryParse(sBO, out iBODID);
                if (iBODID > 0)
                {
                    Params["ID"] = iBODID;
                }
                else if (isValid && guidOutput != Guid.Empty)
                {
                    Params["XIGUID"] = sBO;
                }
                Params["FKiApplicationIDXIGUID"] = AppUID;
                var oBODef = Connection.Select<XIDBO>("XIBO_T_N", Params).OrderBy(x => x.BOID).ToList();
                List<CNV> BOs = new List<CNV>();
                if (!string.IsNullOrEmpty(sSearchText))
                {
                    var FilteredSteps = oBODef.Where(m => m.Name.ToLower().Contains(sSearchText.ToLower())).ToList();
                    foreach (var items in FilteredSteps)
                    {
                        BOs.Add(new CNV() { sName = items.BOID + " " + items.Name, sValue = items.XIGUID.ToString() });
                    }
                }
                else if (iBODID > 0)
                {
                    var FilteredSteps = oBODef.Where(m => m.BOID == iBODID).ToList();
                    foreach (var items in FilteredSteps)
                    {
                        BOs.Add(new CNV() { sName = items.BOID + " " + items.Name, sValue = items.XIGUID.ToString() });
                    }
                }
                else if (isValid && guidOutput != Guid.Empty)
                {
                    var FilteredSteps = oBODef.Where(m => m.XIGUID == guidOutput).ToList();
                    foreach (var items in FilteredSteps)
                    {
                        BOs.Add(new CNV() { sName = items.BOID + " " + items.Name, sValue = items.XIGUID.ToString() });
                    }
                }
                else
                {
                    foreach (var items in oBODef)
                    {
                        BOs.Add(new CNV() { sName = items.BOID + " " + items.Name, sValue = items.XIGUID.ToString() });
                    }
                }
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = BOs;
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
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

        #endregion FK Auto Complete

        #region Distribution

        public CResult Get_DistributionDefinition(string sName = "", string sUID = "")
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


            try
            {
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "sName";
                    PKValue = sName;
                }
                XIDDistribute oDistribute = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                cConnectionString oConString = new cConnectionString();
                string sConnection = System.Configuration.ConfigurationManager.AppSettings["SharedDataBase"];
                string sConString = oConString.ConnectionString(sConnection);
                //string sConString = ConfigurationManager.ConnectionStrings["ClientDataBase"].ConnectionString;
                XIDBAPI Connection = new XIDBAPI(sConString);
                oDistribute = Connection.Select<XIDDistribute>("XIDistribute_T", Params).FirstOrDefault();
                if (oDistribute != null && oDistribute.ID > 0)
                {
                    Dictionary<string, object> DParams = new Dictionary<string, object>();
                    DParams["FKiDistributeID"] = oDistribute.ID;
                    DParams["XIDeleted"] = 0;
                    oDistribute.DistributeLines = Connection.Select<XIDDistributeLine>("XIDistributeLine_t", DParams).OrderBy(x => x.ID).ToList();
                }

                oCResult.oResult = oDistribute;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Distribution definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        #endregion Distribution

        #region XIAccount

        public CResult Get_XIAccountDefinition(string sName = "", string sUID = "")
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
            try
            {
                XIUtility.IDInsteadOfGUID(sUID, "Account");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }

                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "sName";
                    PKValue = sName;
                }
                XIDAccount oAccount = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oAccount = XIEnvConnection.Select<XIDAccount>("XIAccount_T", Params).FirstOrDefault();
                oCResult.oResult = oAccount;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading XIAccount definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        #endregion XIAccount

        #region Object Definition

        public CResult Get_XIObjectDefinition(string sName = "", string sUID = "")
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
            try
            {
                XIIXI oXI = new XIIXI();
                var oBOI = oXI.BOI(sName, sUID);
                if (oBOI != null && oBOI.Attributes.Count() > 0)
                {
                    oCResult.oResult = oBOI;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oCResult.oResult = new XIIBO();
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading XIremComType definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        #endregion Object Definition

        #region DropDown

        public CResult Get_ComponentsDDL()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Get all components to populate in a dropdown";//expalin about this method logic
            try
            {
                List<XIDropDown> DDL = new List<XIDropDown>();
                Dictionary<string, object> oLinks = new Dictionary<string, object>();
                oLinks[XIConstant.Key_XIDeleted] = "0";
                var oComponents = Connection.Select<XIDComponent>("XIComponents_XC_T", oLinks).ToList();
                foreach (var items in oComponents)
                {
                    DDL.Add(new XIDropDown { text = items.sName, Value = items.ID, sGUID = items.XIGUID.ToString() });
                }
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = DDL;
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        #endregion DropDown

        #region Rule

        public CResult Get_RuleDefinition(string sName = "", string sUID = "")
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


            try
            {
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }


                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "sName";
                    PKValue = sName;
                }
                XIDRule oRuleD = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oRuleD = Connection.Select<XIDRule>("XIRules_T", Params).FirstOrDefault();
                if (oRuleD != null && (oRuleD.ID > 0 && (oRuleD.XIGUID != null && oRuleD.XIGUID != Guid.Empty)))
                {
                    Dictionary<string, object> DParams = new Dictionary<string, object>();
                    DParams["FKiRuleIDXIGUID"] = oRuleD.XIGUID;
                    oRuleD.RuleGroups = Connection.Select<XIDRuleGroup>("XIRuleGroups_T", DParams).OrderBy(x => x.ID).ToList();
                }
                oCResult.oResult = oRuleD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Distribution definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public CResult Get_AllRules()
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


            try
            {
                List<XIDRule> oRuleD = new List<XIDRule>();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[XIConstant.Key_XIDeleted] = "0";
                oRuleD = Connection.Select<XIDRule>("XIRules_T", Params).ToList();
                oCResult.oResult = oRuleD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while Rules definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        #endregion Rule

        #region XIFormat

        public CResult Get_XIFormatDefinition(string sName = "", string sUID = "")
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
            try
            {
                XIUtility.IDInsteadOfGUID(sUID, "Account");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }

                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "ID";
                    PKValue = iID.ToString();
                }
                else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                {
                    PKColumn = "xiguid";
                    PKValue = sUID;
                }
                else if (!string.IsNullOrEmpty(sName))
                {
                    PKColumn = "sName";
                    PKValue = sName;
                }
                XIDFormat oFormat = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                oFormat = Connection.Select<XIDFormat>("XIFormat_T", Params).FirstOrDefault();
                oCResult.oResult = oFormat;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Format definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        #endregion XIFormat
        //Override Attributr List
        public CResult OptionListOverride(string sOneClickID = "", string sGUID = "", bool bIsOverrideOneclickGUID = false)
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
            try
            {
                List<XIDropDown> FKDDL = new List<XIDropDown>();
                XIInfraCache oCache = new XIInfraCache();
                var o1Def = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, "731E97DF-1495-468E-909A-0C91E75BCD8B");
                var oCopy = (XID1Click)o1Def.Clone(o1Def);

                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, /*"Range Details"*/"", oCopy.BOIDXIGUID.ToString());
                XIDBO oBODCopy = (XIDBO)oBOD.Clone(oBOD);
                string sBODataSource = string.Empty;
                var sBOName = oBODCopy.Name;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["Name"] = sBOName;
                string sSelectFields = string.Empty;
                sSelectFields = "Name,BOID,iDataSource,sSize,TableName,sPrimaryKey,sType,iDataSourceXIGUID,XIGUID";
                var FKBOD = Connection.Select<XIDBO>("XIBO_T_N", Params, sSelectFields).FirstOrDefault();
                if (FKBOD != null)
                {
                    sBODataSource = GetBODataSource(FKBOD.iDataSourceXIGUID.ToString(), oBODCopy.FKiApplicationID);
                    if (FKBOD.sSize == "10")//maximum number of results in dropdown -- To Do
                    {
                        var Con = new XIDBAPI(sBODataSource);
                        string suid = "1click_" + "731E97DF-1495-468E-909A-0C91E75BCD8B";
                        var sSessionID = HttpContext.Current.Session.SessionID;
                        XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                        List<CNV> nParms = new List<CNV>();
                        nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                        CNV param = new CNV();
                        param.sName = "{XIP|RangeID}";
                        param.sValue = sOneClickID;
                        nParms.Add(param);
                        var oResult = Get_AutoCompleteList(suid, "", nParms);

                        if (oResult.bOK && oResult.oResult != null)
                        {
                            var DDL = (List<XIDFieldOptionList>)oResult.oResult;
                            FKDDL = DDL.Select(m => new XIDropDown { text = m.sOptionValue, Expression = m.sOptionName }).ToList();
                            oCResult.oResult = FKDDL;
                        }
                        else
                        {
                            //XID1Click o1clic = new XID1Click();
                            //o1clic.Query = o1Def.Query;
                            //o1clic.BOIDXIGUID = o1Def.BOIDXIGUID;
                            //var result = o1clic.GetList();//o1clic.OneClick_Execute(null,o1Def);

                            //Dictionary<string, XIIBO> nBOIns = new Dictionary<string, XIIBO>();
                            //nBOIns =(Dictionary<string, XIIBO>)result.oResult;
                            //foreach (var item in nBOIns)
                            //{
                            //    FKOptDDL = item.Value.Attributes.Select(n => new XIDOptionList { sOptionName = n.Value.sName, sValues = n.Value.sValue }).ToList();
                            //}
                            //oCResult.oResult = FKOptDDL;
                        }
                    }
                }
                if (bIsOverrideOneclickGUID == true)
                {
                    string suid = "1click_" + sOneClickID;
                    var sSessionID = HttpContext.Current.Session.SessionID;
                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                    List<CNV> nParms = new List<CNV>();
                    nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                    CNV param = new CNV();
                    param.sName = "{XIP|RangeID}";
                    param.sValue = sOneClickID;
                    nParms.Add(param);
                    var oResult = Get_AutoCompleteList(suid, "", nParms);

                    if (oResult.bOK && oResult.oResult != null)
                    {
                        var DDL = (List<XIDFieldOptionList>)oResult.oResult;
                        FKDDL = DDL.Select(m => new XIDropDown { text = m.sOptionValue, Expression = m.sOptionName }).ToList();
                        oCResult.oResult = FKDDL;
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Format definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        #region QSParams

        public CResult Get_XIRequiredParamDefinition(string sType = "", string sUID = "")
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
            try
            {
                XIUtility.IDInsteadOfGUID(sUID, "Required Params");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }

                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                int iType = 0;
                if (!string.IsNullOrEmpty(sType))
                {
                    if (sType.ToLower() == "qs")
                    {
                        iType = 10;
                        if (iID > 0)
                        {
                            PKColumn = "FKiQSDID";
                            PKValue = iID.ToString();
                        }
                        else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                        {
                            PKColumn = "FKiQSDIDXIGUID";
                            PKValue = sUID;
                        }
                    }
                    else if (sType.ToLower() == "step")
                    {
                        iType = 20;
                        if (iID > 0)
                        {
                            PKColumn = "FKiStepDID";
                            PKValue = iID.ToString();
                        }
                        else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                        {
                            PKColumn = "FKiStepDIDXIGUID";
                            PKValue = sUID;
                        }
                    }
                    else if (sType.ToLower() == "qsi")
                    {
                        iType = 30;
                        if (iID > 0)
                        {
                            PKColumn = "FKiQSIID";
                            PKValue = iID.ToString();
                        }
                        else if (iID == 0 && !string.IsNullOrEmpty(sUID) && sUID != "0")
                        {
                            PKColumn = "FKiQSIIDXIGUID";
                            PKValue = sUID;
                        }
                    }
                    else if (sType.ToLower() == "stepi")
                    {
                        iType = 40;
                    }
                }
                List<XIRequiredParamDef> oQSParams = new List<XIRequiredParamDef>();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                //Params["iType"] = iType;
                oQSParams = Connection.Select<XIRequiredParamDef>("XIRequiredParamDef_T", Params).ToList();
                oCResult.oResult = oQSParams;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Format definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        #endregion QSParams

        #region BOSignalR

        public CResult Get_BOSignalRConfig(string sUID = "")
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
            try
            {
                XIUtility.IDInsteadOfGUID(sUID, "Required Params");
                long iID = 0;
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
                }
                Guid XIGUID = Guid.Empty;
                Guid.TryParse(sUID, out XIGUID);
                if (XIGUID != null && XIGUID != Guid.Empty)
                {
                    string PKColumn = "FKiBOIDXIGUID";
                    string PKValue = XIGUID.ToString();

                    List<XISignalRsettings> oBOSignalR = new List<XISignalRsettings>();
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params[PKColumn] = PKValue;
                    oBOSignalR = Connection.Select<XISignalRsettings>("XISignalRUsersSettings_AU_T", Params).ToList();
                    oCResult.oResult = oBOSignalR;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Format definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        #endregion BOSignalR
        public CResult Get_UserPreference(string FKiUserID = "", string sCoreDatabase = "")
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;
            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started ContentDefinition Loading" });
            }
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
            }
            oCResult.sMessage = "someone tried to do something they shouldnt";
            try
            {
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                long iID = 0;
                if (long.TryParse(FKiUserID, out iID))
                {
                }
                else
                {
                }
                Guid XIGUID = Guid.Empty;
                Guid.TryParse(FKiUserID, out XIGUID);
                string PKColumn = string.Empty;
                string PKValue = string.Empty;
                if (iID > 0)
                {
                    PKColumn = "FKiUserID";
                    PKValue = iID.ToString();
                }
                else if (XIGUID != null && XIGUID != Guid.Empty)
                {
                    PKColumn = "XIGUID";
                    PKValue = XIGUID.ToString();
                }
                XIInfraUserPreference UserPreference = new XIInfraUserPreference();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params[PKColumn] = PKValue;
                UserPreference = Connection.Select<XIInfraUserPreference>("XIUserPreference_T", Params).OrderBy(x => x.FKiUserID).FirstOrDefault();
                oCResult.oResult = UserPreference;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

    }
}
