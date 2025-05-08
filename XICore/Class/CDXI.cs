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

namespace XICore
{
    public class XIDXI : XIDefinitionBase
    {
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public int iOrgID { get; set; }
        public string sAppName { get; set; }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        #region BOMethods

        public CResult Get_BODefinition(string sBOName = "", int iBODID = 0, string sUID = "")
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { Name = "Stage", Value = "Started Execution" });
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
                XIDBO oBOD = new XIDBO();
                long iID = 0;
                if (sBOName != "" || iBODID > 0)
                {
                    //Load BO Definition
                    oBOD = Load_BO(sBOName, iBODID);
                    var FKAttributes = oBOD.Attributes.Where(m => m.Value.FKiType > 0 && !string.IsNullOrEmpty(m.Value.FKTableName)).ToList();
                    foreach (var item in FKAttributes)
                    {
                        string sBODataSource = string.Empty;
                        var sTableName = item.Value.FKTableName;
                        Dictionary<string, object> Params = new Dictionary<string, object>();
                        Params["TableName"] = sTableName;
                        var FKBO = Connection.Select<XIDBO>("XIBO_T_N", Params).FirstOrDefault();
                        var FKBOD = Load_BO(FKBO.Name, FKBO.BOID);
                        //var BO = AllBOs.Where(m => m.TableName == sTableName).FirstOrDefault();
                        sBODataSource = GetBODataSource(FKBOD.iDataSource);
                        oBOD.Attributes[item.Value.Name].sFKBOSize = FKBOD.sSize;
                        if (FKBOD.sSize == "10")//maximum number of results in dropdown -- To Do
                        {
                            var Con = new XIDBAPI(sBODataSource);
                            if (FKBOD != null)
                            {
                                XIDGroup oGroup;
                                if (FKBOD.Groups.TryGetValue("Label", out oGroup))
                                {
                                    var LabelGroup = oGroup.BOFieldNames;
                                    if (!string.IsNullOrEmpty(LabelGroup))
                                    {
                                        Dictionary<string, string> DDL = Con.SelectDDL(LabelGroup, FKBOD.TableName);
                                        var FKDDL = DDL.Select(m => new XIDropDown { text = m.Key, Expression = m.Value }).ToList();
                                        oBOD.Attributes[item.Value.Name].FieldDDL = FKDDL;
                                    }
                                }
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

                if (oBOD != null)
                    oBOD.oParent = this;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oBOD;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always

            //return oURL;
        }       

        private XIDBO Load_BO(string sBOName, int iBODID)
        {
            XIDBO oBOD = new XIDBO();
            string sBODefinition = "select * from XIBO_T_N BOD " +
                  "inner join XIBOAttribute_T_N Attr on BOD.BOID = Attr.BOID " +
                  "left join XIBOGroup_T_N Grp on BOD.BOID = Grp.BOID " +
                  "left join XIBOScript_T Scr on BOD.BOID = Scr.FKiBOID " +
                  "left join XIBOUIDetails_T BOUI on BOD.BOID = BOUI.FKiBOID " +
                  "left join XIBOStructure_T Strct on BOD.BOID = Strct.BOID " +
                  "left join XIBOOptionList_T_N Opt on Attr.ID = Opt.BOFieldID " +
                    "";

            if (iBODID > 0)
            {
                sBODefinition = sBODefinition + "WHERE BOD.BOID = @iBODID";
            }
            else if (!string.IsNullOrEmpty(sBOName))
            {
                sBODefinition = sBODefinition + "WHERE BOD.Name = @Name";
            }
            var param = new
            {
                Name = sBOName,
                iBODID = iBODID
            };

            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
            Conn.Open();
            //Conn.ChangeDatabase(sDatabase);
            var lookupBOs = new Dictionary<int, XIDBO>();
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
                                oBOD.Attributes[oAttr.Name] = oAttr;
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
                                oBOD.Groups[oGrp.GroupName] = oGrp;
                            }
                        }
                        if (oScript != null)
                        {
                            XIDScript oSrpt;
                            if (!lookupScripts.TryGetValue(oScript.ID, out oSrpt))
                            {
                                lookupScripts.Add(oScript.ID, oSrpt = oScript);
                                oBOD.Scripts[oSrpt.sName] = oSrpt;
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
                                oBOD.Structures[oStructure.sStructureName] = oStrct;
                            }
                        }

                    }
                    return oBOD;
                },
                param
                ).AsQueryable();

            oBOD = lookupBOs.Values.FirstOrDefault();
            return oBOD;
        }

        public string GetBODataSource(int iDataSourceID)
        {
            cConnectionString oConString = new cConnectionString();
            var sBODataSource = string.Empty;
            if (iDataSourceID != 0)
            {
                if (iDataSourceID == -1)
                {
                    sBODataSource = oConString.ConnectionString(sCoreDatabase);
                }
                else if (iDataSourceID == -2)
                {
                    sBODataSource = oConString.ConnectionString(sOrgDatabase);
                }
                else
                {
                    var SrcID = iDataSourceID;
                    XIEncryption oEncrypt = new XIEncryption();
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["id"] = SrcID;
                    var DataSource = Connection.Select<XIDataSource>("XIDataSource_XID_T", Params).FirstOrDefault();// XIContext.XIDataSources.Find(SrcID);
                    sBODataSource = oEncrypt.DecryptData(DataSource.sConnectionString, true, DataSource.ID.ToString());
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
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { Name = "Stage", Value = "Started Execution" });
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
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { Name = "Stage", Value = "Started Execution" });
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
                Dictionary<string, string> DDL = Connection.SelectDDL("name, tablename", "XIBO_T");
                var BOsDDL = DDL.Select(m => new XIDropDown { text = m.Key, Expression = m.Value }).ToList();
                oCResult.oResult = BOsDDL;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
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
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { Name = "Stage", Value = "Started Execution" });
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
                var DataSourceDDL = DDL.Select(m => new XIDropDown { Value = Convert.ToInt32(m.Key), text = m.Value }).ToList();
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
            }
            return oCResult; // always

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


        public CResult Get_OrgDefinition(string sOrgName = "", int iOrgID = 0)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { Name = "Stage", Value = "Started Execution" });
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
                XIDOrganisation oOrg = null;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sOrgDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(sOrgName))
                {
                    Params["Name"] = sOrgName;
                }
                else if (iOrgID > 0)
                {
                    Params["ID"] = iOrgID;
                }                
                oOrg = Connection.Select<XIDOrganisation>("Organizations", Params).FirstOrDefault();
                oCResult.oResult = oOrg;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }

        public CResult Get_LayoutDefinition(string LayoutName = "", int iLayoutID = 0)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { Name = "Stage", Value = "Started Execution" });
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
                CDLayout oLayout = new CDLayout();
                List<CDLayout> oLayoutList = new List<CDLayout>();
                List<CDMasterData> oLayoutThemes = new List<CDMasterData>();
                string sGUID = string.Empty;
                XICoreDbContext dbContext = new XICoreDbContext();
                if (iLayoutID > 0)
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["ID"] = iLayoutID;
                    oLayout = Connection.Select<XIDLayout>("XILayout_T", Params).FirstOrDefault();
                }
                else if (!string.IsNullOrEmpty(LayoutName))
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["LayoutName"] = LayoutName;
                    oLayout = Connection.Select<XIDLayout>("XILayout_T", Params).FirstOrDefault();
                }
                if (oLayout.ID != 0)
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["LayoutID"] = oLayout.ID;
                    oLayout.LayoutDetails = Connection.Select<XIDLayoutDetails>("XILayoutDetail_T", Params).ToList();
                }
                else
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["LayoutType"] = "inline";
                    oLayoutList = Connection.Select<CDLayout>("XILayout_T", Params).ToList();
                    var oLayoutLists = oLayoutList.Select(m => new cDropDown { Expression = m.LayoutName, ID = m.ID }).ToList();
                    oLayout.LayoutsList = oLayoutLists;
                }
                if (oLayout != null)
                {
                if (oLayout.iThemeID > 0)
                {
                    Dictionary<string, object> UserParams = new Dictionary<string, object>();
                    UserParams["ID"] = oLayout.iThemeID;
                    oLayout.sThemeName = Connection.Select<XIDMasterData>("XIMasterData_T", UserParams).FirstOrDefault().FileName;
                }
                    else
                    {
                        Dictionary<string, object> UserParams = new Dictionary<string, object>();
                        UserParams["Name"] = "Themes";
                        oLayoutThemes = Connection.Select<CDMasterData>("XIMasterData_T", UserParams).ToList();
                        var oAllLayoutThemes = oLayoutThemes.Select(m => new cDropDown { Expression = m.Expression, ID = m.ID }).ToList();
                        oLayout.LayoutThemes = oAllLayoutThemes;
                    }
                }

                Dictionary<string, object> MapParams = new Dictionary<string, object>();
                //MapParams["PopupLayoutID"] = oLayout.ID;
                //oLayout.LayoutMappings = Connection.Select<CDLayoutMapping>("XILayoutMapping_T", MapParams).ToList();
                
                //if (oLayout.LayoutType.ToLower() == "inline" || oLayout.LayoutType.ToLower() == "template")
                //{
                //    oLayout.sGUID = Guid.NewGuid().ToString();
                //}
                if (oLayout != null)
                {
                    oCResult.oResult = oLayout;
                }
                else
                {
                    oCResult.oResult = oLayoutList;
                }

                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
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
        
        public CResult Get_ApplicationDefinition(string sApplicationName = "")
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { Name = "Stage", Value = "Started Execution" });
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
                XIDApplication oAPP = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["sApplicationName"] = sApplicationName;
                oAPP = Connection.Select<XIDApplication>("XIApplication_T", Params).FirstOrDefault();
                oCResult.oResult = oAPP;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
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
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { Name = "Stage", Value = "Started Execution" });
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
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Get_XILinkDefinition(int ID = 0)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { Name = "Stage", Value = "Started Execution" });
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
                XILink oXiLink = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["XiLinkID"] = ID;
                oXiLink = Connection.Select<XILink>("XILink_T", Params).FirstOrDefault();
                if (oXiLink != null)
                {                    
                    Dictionary<string, object> NVParams = new Dictionary<string, object>();
                    NVParams["XiLinkID"] = ID;
                    oXiLink.XiLinkNVs = Connection.Select<XiLinkNV>("XILinkNV_T", NVParams).ToList();
                }
                if (oXiLink != null)
                {
                    Dictionary<string, object> ListParams = new Dictionary<string, object>();
                    ListParams["XiLinkID"] = ID;
                    oXiLink.XiLinkLists = Connection.Select<XiLinkList>("XILinkList_T", ListParams).ToList();
                }
                oCResult.oResult = oXiLink;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always

            //return oURL;
        }

        public XID1Click D1Click(string sUID, string sStructureCode = "")
        {
            XID1Click o1Click = new XID1Click();
            long iID = 0;




            if (long.TryParse(sUID, out iID))
            {
            }
            else
            {
            }


            return o1Click;
        }

        #region QuestionSet 
             
        public CResult Get_QSDefinition(string sQSName = "", int iQSID = 0, string sUID = "")
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
                oCResult.oTraceStack.Add(new CNV { Name = "Stage", Value = "Started Execution" });
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
                XIIBO oBOI = null/* TODO Change to default(_) if this is not a reference type */;


                // sLoadSteps - if "" then load all. otherwise for example:
                // Step1,Step3,Step9,Step10
                // load steps keyed by name (which must be unique and if it gets same name again just dont add in)
                // the steps are instances
                // KEY BY LOWER CASE ONLY

                // TO DO - get oQSD from the cache
                if (!string.IsNullOrEmpty(sQSName) || iQSID > 0)
                {
                    oDQS = GetQuestionSetDefinitionByID(sQSName, iQSID);
                }
                if (long.TryParse(sUID, out iID))
                {
                }
                else
                {
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
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always

            //return oURL;
        }

        public XIDQS GetQuestionSetDefinitionByID(string sQSName, int iQSID)
        {
            string sQSDefinitionQry = "select * from XIQSDefinition_T QSDef " +
              "left join XIQSStepDefinition_T QSSDef on QSDef.ID = QSSDef.FKiQSDefintionID " +
              "left join XIFieldDefinition_T XIFD on QSSDef.ID = XIFD.FKiXIStepDefinitionID " +
              "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginID = XIO.ID " +
              "left join XIDataType_T XIDT on XIO.FKiDataType = XIDT.id " +
              "left join XIQSNavigation_T NAV on QSSDef.ID = NAV.FKiStepDefinitionID " +
              "left join XIFieldOptionList_T OPT on XIO.ID = OPT.FKiQSFieldID ";
             

            if (!string.IsNullOrEmpty(sQSName))
            {
                sQSDefinitionQry = sQSDefinitionQry + "WHERE QSDef.sName = @Name;";
            }
            else if (iQSID > 0)
            {
                sQSDefinitionQry = sQSDefinitionQry + "WHERE QSDef.ID = @id;";
            }
            var param = new
            {
                id = iQSID,
                Name = sQSName
            };

            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
            Conn.Open();
            //Conn.ChangeDatabase(sDatabase);
            var lookup = new Dictionary<int, XIDQS>();
            var lookup2 = new Dictionary<int, XIDQSStep>();
            var lookup3 = new Dictionary<int, XIDFieldDefinition>();
            var lookup4 = new Dictionary<int, XIDFieldOrigin>();
            var lookup5 = new Dictionary<int, XIDQSStepNavigations>();
            var lookup6 = new Dictionary<int, XIDFieldOptionList>();
            Conn.Query<XIDQS, XIDQSStep, XIDFieldDefinition, XIDFieldOrigin, XIDDataType, XIDQSStepNavigations, XIDFieldOptionList, XIDQS>(sQSDefinitionQry,
                (QS, Step, FieldDefinition, FieldOrigin, DataType, Navigations, OptionList) =>
                {
                    XIDQS oQSDefinition;
                    if (!lookup.TryGetValue(QS.ID, out oQSDefinition))
                    {
                        lookup.Add(QS.ID, oQSDefinition = QS);
                    }
                    XIDQSStep oStepDefinition;
                    if (Step != null)
                    {
                        if (!lookup2.TryGetValue(Step.ID, out oStepDefinition))
                        {
                            lookup2.Add(Step.ID, oStepDefinition = Step);
                            oQSDefinition.Steps[oStepDefinition.sName] = oStepDefinition;
                        }
                        XIDFieldDefinition oFieldDefintion;
                        if (FieldDefinition != null)
                        {
                            if (!lookup3.TryGetValue(FieldDefinition.ID, out oFieldDefintion))
                            {
                                lookup3.Add(FieldDefinition.ID, oFieldDefintion = FieldDefinition);
                                oStepDefinition.FieldDefs[FieldOrigin.sName] = oFieldDefintion;

                            }
                            XIDFieldOrigin oXIFieldOrigin;
                            if (FieldOrigin != null)
                            {
                                
                                if (!lookup4.TryGetValue(FieldOrigin.ID, out oXIFieldOrigin))
                                {

                                    if (FieldOrigin.iMasterDataID > 0)
                                    {
                                        //FieldOrigin.ddlFieldOptionList = dbContext.Types.Where(m => m.Code == FieldOrigin.iMasterDataID).ToList().Select(m => new cXIFieldOptionList { sOptionName = m.Expression, sOptionValue = m.ID.ToString() }).ToList();
                                    }
                                    else if (FieldOrigin.FK1ClickID > 0)
                                    {
                                        //var o1Click = dbContext.Reports.Where(m => m.ID == FieldOrigin.FK1ClickID).FirstOrDefault();
                                        //var oBO = dbContext.BOs.Find(o1Click.BOID);
                                        //FieldOrigin.bIsLargeBO = true;
                                        //FieldOrigin.ddlFieldOptionList = new List<cXIFieldOptionList>();
                                        //var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                                        //SqlConnection Con = new SqlConnection(sBODataSource);
                                        //Con.Open();
                                        //SqlCommand cmd = new SqlCommand();
                                        //cmd.Connection = Con;
                                        //cmd.CommandText = o1Click.Query;
                                        //SqlDataReader reader = cmd.ExecuteReader();
                                        //while (reader.Read())
                                        //{
                                        //    FieldOrigin.ddlFieldOptionList.Add(new cXIFieldOptionList
                                        //    {
                                        //        sOptionValue = reader.GetInt32(0).ToString(),
                                        //        sOptionName = reader.GetString(1)
                                        //    });
                                        //}
                                        //Con.Close();
                                    }
                                    lookup4.Add(FieldOrigin.ID, oXIFieldOrigin = FieldOrigin);
                                    oXIFieldOrigin = FieldOrigin;
                                }

                                XIDFieldOptionList oOptions;
                                if (OptionList != null)
                                {
                                    oOptions = OptionList;
                                    if (!lookup6.TryGetValue(OptionList.ID, out oOptions))
                                    {
                                        lookup6.Add(OptionList.ID, oOptions = OptionList);
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
                            if (!lookup5.TryGetValue(Navigations.ID, out nNavs))
                            {
                                lookup5.Add(Navigations.ID, nNavs = Navigations);
                                oStepDefinition.Navigations[nNavs.sName] = nNavs;
                            }
                        }
                    }
                    return oQSDefinition;
                },
                param
                ).AsQueryable();

            var oQSDef = lookup.Values.FirstOrDefault();
            var Sections = GetStepSectionDefinitions(oQSDef.ID);
            if (Sections != null)
            {
                foreach (var items in Sections.Steps)
                {
                    oQSDef.Steps.Where(m => m.Value.ID == items.Value.ID).FirstOrDefault().Value.Sections = items.Value.Sections;
                    //oQSInstance.nStepInstances.Where(m => m.FKiQSStepDefinitionID == items.ID).FirstOrDefault().nSections = items.Sections;
                }
            }
            oQSDef.Visualisation = GetQSVisualisations(oQSDef.iVisualisationID);
            oQSDef.QSVisualisations = GetQSFiledVisualisations(iQSID);
            if (oQSDef.Steps != null)
            {
                foreach (var items in oQSDef.Steps)
                {
                    if (items.Value.iLayoutID > 0)
                    {
                        items.Value.Layout = (XIDLayout)Get_LayoutDefinition(null, items.Value.iLayoutID).oResult;
                    }
                }
            }
            return oQSDef;
        }

        public XIDQS GetStepSectionDefinitions(int iQSID)
        {            
            string sQSDefinitionQry = "select * from XIQSDefinition_T QSDef " +
                "inner join XIQSStepDefinition_T QSSDef on QSDef.ID = QSSDef.FKiQSDefintionID " +
                "inner join XIStepSectionDefinition_T Sec on QSSDef.ID = Sec.FKiStepDefinitionID " +
                "left join XIFieldDefinition_T XIFD on Sec.ID = XIFD.FKiStepSectionID " +
                "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginID = XIO.ID " +
                "left join XIDataType_T XIDT on XIO.FKiDataType = XIDT.id " +
                "left join XIFieldOptionList_T OPT on XIO.ID = OPT.FKiQSFieldID " +
                "WHERE QSDef.ID = @id;";
            var param = new
            {
                id = iQSID
            };

            //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
            Conn.Open();
            //Conn.ChangeDatabase(sDatabase);
            var lookup = new Dictionary<int, XIDQS>();
            var lookup2 = new Dictionary<int, XIDQSStep>();
            var lookup3 = new Dictionary<int, XIDFieldDefinition>();
            var lookup4 = new Dictionary<int, XIDFieldOrigin>();
            var lookup6 = new Dictionary<int, XIDFieldOptionList>();
            var lookup7 = new Dictionary<int, XIDQSSection>();
            Conn.Query<XIDQS, XIDQSStep, XIDQSSection, XIDFieldDefinition, XIDFieldOrigin, XIDDataType, XIDFieldOptionList, XIDQS>(sQSDefinitionQry,
                (QS, Step, SectionDef, FieldDefinition, FieldOrigin, DataType, OptionList) =>
                {
                    XIDQS oQSDefinition;
                    if (!lookup.TryGetValue(QS.ID, out oQSDefinition))
                    {
                        lookup.Add(QS.ID, oQSDefinition = QS);
                    }
                    XIDQSStep oStepDefinition;
                    if (Step != null)
                    {
                        if (!lookup2.TryGetValue(Step.ID, out oStepDefinition))
                        {
                            lookup2.Add(Step.ID, oStepDefinition = Step);
                            oQSDefinition.Steps[oStepDefinition.sName] = oStepDefinition;
                        }
                        XIDQSSection oSection;
                        if (SectionDef != null)
                        {
                            if (!lookup7.TryGetValue(SectionDef.ID, out oSection))
                            {
                                lookup7.Add(SectionDef.ID, oSection = SectionDef);
                                oStepDefinition.Sections[SectionDef.ID.ToString()] = SectionDef;
                            }
                            XIDFieldDefinition oFieldDefintion;
                            if (FieldDefinition != null)
                            {
                                XIDFieldOrigin oXIFieldOrigin;
                                if (FieldOrigin != null)
                                {
                                if (!lookup3.TryGetValue(FieldDefinition.ID, out oFieldDefintion))
                                {
                                    lookup3.Add(FieldDefinition.ID, oFieldDefintion = FieldDefinition);
                                        oSection.FieldDefs[FieldOrigin.sName] = oFieldDefintion;
                                }
                                    if (!lookup4.TryGetValue(FieldOrigin.ID, out oXIFieldOrigin))
                                    {

                                        if (FieldOrigin.iMasterDataID > 0)
                                        {
                                            //FieldOrigin.ddlFieldOptionList = dbContext.Types.Where(m => m.Code == FieldOrigin.iMasterDataID).ToList().Select(m => new cXIFieldOptionList { sOptionName = m.Expression, sOptionValue = m.ID.ToString() }).ToList();
                                        }
                                        else if (FieldOrigin.FK1ClickID > 0)
                                        {
                                            //var o1Click = dbContext.Reports.Where(m => m.ID == FieldOrigin.FK1ClickID).FirstOrDefault();
                                            //var oBO = dbContext.BOs.Find(o1Click.BOID);
                                            //FieldOrigin.bIsLargeBO = true;
                                            //FieldOrigin.ddlFieldOptionList = new List<cXIFieldOptionList>();
                                            //var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                                            //SqlConnection Con = new SqlConnection(sBODataSource);
                                            //Con.Open();
                                            //SqlCommand cmd = new SqlCommand();
                                            //cmd.Connection = Con;
                                            //cmd.CommandText = o1Click.Query;
                                            //SqlDataReader reader = cmd.ExecuteReader();
                                            //while (reader.Read())
                                            //{
                                            //    FieldOrigin.ddlFieldOptionList.Add(new cXIFieldOptionList
                                            //    {
                                            //        sOptionValue = reader.GetInt32(0).ToString(),
                                            //        sOptionName = reader.GetString(1)
                                            //    });
                                            //}
                                            //Con.Close();
                                        }
                                        lookup4.Add(FieldOrigin.ID, oXIFieldOrigin = FieldOrigin);
                                        oXIFieldOrigin = FieldOrigin;
                                    }

                                    XIDFieldOptionList oOptions;
                                    if (OptionList != null)
                                    {
                                        oOptions = OptionList;
                                        if (!lookup6.TryGetValue(OptionList.ID, out oOptions))
                                        {
                                            lookup6.Add(OptionList.ID, oOptions = OptionList);
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
                        }
                    }
                    return oQSDefinition;
                },
                param
                ).AsQueryable();

            var Sections = lookup.Values.FirstOrDefault();


            var SectionContent = GetComponentParams(iQSID);
            if (SectionContent != null)
            {
                foreach (var items in SectionContent.Steps)
                {
                    foreach (var item in items.Value.Sections)
                    {
                        if (item.Value.ComponentDefinition != null)
                        {
                            var AllSections = Sections.Steps.Where(m => m.Value.ID == items.Value.ID).FirstOrDefault();
                            AllSections.Value.ComponentDefinition = item.Value.ComponentDefinition;
                        }
                    }
                }
            }
            return Sections;
        }

        private XIDQS GetComponentParams(int iQSID)
        {
            
            string sQSDefinitionQry = "select * from XIQSDefinition_T QSD " +
                "inner join XIQSStepDefinition_T QSSD on QSD.ID = QSSD.FKiQSDefintionID " +
                "inner join XIStepSectionDefinition_T Sec on QSSD.ID = Sec.FKiStepDefinitionID " +
                "inner join XIComponentParams_T NVs on sec.ID = NVs.iStepSectionID " +
                "inner join XIComponents_XC_T XC on NVs.FKiComponentID = XC.ID " +
                "where QSD.ID = @id;";
            var param = new
            {
                id = iQSID
            };

            // SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
            Conn.Open();
            //Conn.ChangeDatabase(sDatabase);
            var lookup = new Dictionary<int, XIDQS>();
            var lookup2 = new Dictionary<int, XIDQSStep>();
            var lookup3 = new Dictionary<int, XIDQSSection>();
            var lookup4 = new Dictionary<int, XIDComponent>();
            Conn.Query<XIDQS, XIDQSStep, XIDQSSection, cDComponentParam, XIDComponent, XIDQS>(sQSDefinitionQry,
                (QS, Step, SectionDef, ComponentParams, Component) =>
                {
                    XIDQS oQSDefinition;
                    if (!lookup.TryGetValue(QS.ID, out oQSDefinition))
                    {
                        lookup.Add(QS.ID, oQSDefinition = QS);
                    }
                    XIDQSStep oStepDefinition;
                    if (Step != null)
                    {
                        if (!lookup2.TryGetValue(Step.ID, out oStepDefinition))
                        {
                            lookup2.Add(Step.ID, oStepDefinition = Step);
                            oQSDefinition.Steps[oStepDefinition.sName] = oStepDefinition;
                        }
                        XIDQSSection oSection;
                        if (SectionDef != null)
                        {
                            if (!lookup3.TryGetValue(SectionDef.ID, out oSection))
                            {
                                lookup3.Add(SectionDef.ID, oSection = SectionDef);
                                oStepDefinition.Sections[SectionDef.ID.ToString()] = SectionDef;
                            }
                            XIDComponent oComponent;
                            if (Component != null)
                            {
                                if (!lookup4.TryGetValue(SectionDef.ID, out oComponent))
                                {
                                    lookup4.Add(SectionDef.ID, oComponent = Component);
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
                                        oSection.ComponentDefinition.Params = new List<cDComponentParam>();
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

            var SectionContent = lookup.Values.FirstOrDefault();
            return SectionContent;
        }

        private XIVisualisation GetQSVisualisations(int iVisualID)
        {
            XIVisualisation oXiVis = null;
            Dictionary<string, object> Params = new Dictionary<string, object>();
            Params["XiVisualID"] = iVisualID;
            oXiVis = Connection.Select<XIVisualisation>("XiVisualisations", Params).FirstOrDefault();
            if (oXiVis != null)
            {
                List<XIVisualisationNV> oXIVisNV = null;
                Dictionary<string, object> NVParams = new Dictionary<string, object>();
                NVParams["XiVisualID"] = iVisualID;
                oXIVisNV = Connection.Select<XIVisualisationNV>("XiVisualisationNVs", NVParams).ToList();
                if(oXIVisNV!=null && oXIVisNV.Count() > 0)
                {
                    oXiVis.XiVisualisationNVs = oXIVisNV;
                }
            }
            return oXiVis;
        }

        private List<XIQSVisualisation> GetQSFiledVisualisations(int iQSID)
        {
            List<XIQSVisualisation> oXiVis = null;
            Dictionary<string, object> Params = new Dictionary<string, object>();
            Params["FKiQSDefinitionID"] = iQSID;
            oXiVis = Connection.Select<XIQSVisualisation>("XIQSVisualisation_T", Params).ToList();
            return oXiVis;
        }

        #endregion QuestionSet
    }
}