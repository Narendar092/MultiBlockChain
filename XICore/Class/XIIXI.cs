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
using System.Configuration;
using Dapper;
using XISystem;
using XIDatabase;
using System.Data;
using System.Web;
using XIDataBase;
using MongoDB.Bson;
using xiEnumSystem;

namespace XICore
{
    public class XIIXI : XIInstanceBase
    {
        public int iSwitchDataSrcID = 0;
        public Guid SwitchDataSrcIDXIGUID = Guid.Empty;
        public CResult oCResult = new CResult();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public XIIBO BOI(string sBODName = "", string sUID = "", string sGroupName = "", List<CNV> oWhrParams = null, bool bIsCopy = false, bool bIsPKID = false)
        {
            CResult oCR = new CResult();
            XIIBO oBOI = null;//new XIIBO();
            try
            {
                long iID = 0;
                int iOrgID = 0;
                int iAppID = 0;
                int iRoleID = 0;
                string sPKValue = string.Empty;
                Guid OutputGUID;
                bool ISGUID = Guid.TryParse(sUID, out OutputGUID);
                long.TryParse(sUID, out iID);
                if (iID > 0 || (OutputGUID != null && OutputGUID != Guid.Empty) || (oWhrParams != null && oWhrParams.Count() > 0))
                {

                }
                else
                {
                    return oBOI;
                }
                CUserInfo oInfo = new CUserInfo();
                XIInfraUsers oUser = new XIInfraUsers();
                oInfo = oUser.Get_UserInfo();
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
                    }
                }
                else
                {
                    iOrgID = oInfo.iOrganizationID;
                    iAppID = oInfo.iApplicationID;
                    iRoleID = oInfo.iRoleID;
                }
                List<SqlParameter> SqlParams = new List<SqlParameter>();
                XIInfraCache oCache = new XIInfraCache();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBODName);
                var WhiteListCheck = System.Configuration.ConfigurationManager.AppSettings["WhitelistCheck"];
                if (WhiteListCheck == "yes")
                {
                    if (oBOD != null && oBOD.Name.ToLower() != "BO WhiteList".ToLower() && oBOD.BOID != 2459)
                    {
                        oCR = oBOI.Check_Whitelist(oBOD.BOID, oInfo.iRoleID, iOrgID, iAppID, "read", oBOD.iLevel, oInfo.iLevel);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            var bUNAuth = (bool)oCR.oResult;
                            if (bUNAuth)
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                return new XIIBO();
                            }
                        }
                    }
                }
                string sIDRef = string.Empty;
                if (iRoleID != 55)
                {
                    sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, iAppID + "_" + iOrgID + "_" + iRoleID + "_" + XIConstant.IDRef_internal);
                }
                else
                {
                    sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, iAppID + "_" + iOrgID + "_" + XIConstant.IDRef_public);
                }
                var iDatasource = oBOD.iDataSource;
                Guid iDataSoruceXIGUID = oBOD.iDataSourceXIGUID;
                XIDXI oXID = new XIDXI();
                var oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, oBOD.iDataSourceXIGUID.ToString());
                if (!string.IsNullOrEmpty(oDataSource.sQueryType))
                {
                    if (oDataSource.sQueryType.ToLower() == "mssql")
                    {
                        oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started BO Instance Loading" });
                        QueryEngine oQE = new QueryEngine();
                        List<XIWhereParams> oWParams = new List<XIWhereParams>();
                        if (!string.IsNullOrEmpty(sUID))
                        {
                            SqlParameter osqlparam = new SqlParameter();

                            XIWhereParams oWP = new XIWhereParams();
                            if (bIsPKID)
                            {
                                oWP.sField = oBOD.sPrimaryKey;
                            }
                            else if (oBOD.bUID && !bIsPKID && ISGUID)
                            {
                                oWP.sField = "xiguid";
                            }
                            else if (ISGUID)
                            {
                                oWP.sField = "xiguid";
                            }
                            else
                            {
                                oWP.sField = "pk";
                            }
                            oWP.sOperator = "=";
                            oWP.sValue = sUID;
                            oWParams.Add(oWP);

                            if (oBOD.bUID && !bIsPKID && ISGUID)
                            {
                                osqlparam.ParameterName = "@XIGUID";
                            }
                            else if (ISGUID)
                            {
                                osqlparam.ParameterName = "@XIGUID";
                            }
                            else
                            {
                                osqlparam.ParameterName = "@" + oBOD.sPrimaryKey;
                            }
                            osqlparam.Value = sUID;
                            SqlParams.Add(osqlparam);
                        }
                        if (oWhrParams != null && oWhrParams.Count() > 0)
                        {
                            oWParams.AddRange(oWhrParams.Select(m => new XIWhereParams { sField = m.sName, sValue = m.sValue, sOperator = "=" }));
                            foreach (var prm in oWhrParams)
                            {
                                SqlParameter osqlparam = new SqlParameter();
                                osqlparam.ParameterName = "@" + prm.sName;
                                osqlparam.Value = prm.sValue;
                                SqlParams.Add(osqlparam);
                            }
                        }

                        oQE.AddBO(sBODName, sGroupName, oWParams);
                        oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "BO definition added successfully to the QueryEngine" });
                        CResult oCresult = oQE.BuildQuery();
                        oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "BO Instance query build successfully" });
                        if (oCresult.bOK && oCresult.oResult != null)
                        {
                            var sSql = (string)oCresult.oResult;
                            oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "SQL:" + sSql });
                            ExecutionEngine oEE = new ExecutionEngine();
                            oInfo = oUser.Get_UserInfo();
                            if (oBOD.TableName.ToLower() == "Organizations".ToLower() || oBOD.TableName.ToLower() == "XIBOWhiteList_T".ToLower() || oBOD.TableName.ToLower() == "XIAppRoles_AR_T".ToLower())
                            {
                                if (oInfo.sCoreDataBase != null)
                                {
                                    var DataSource = oXID.Get_DataSourceDefinition(oInfo.sCoreDataBase);
                                    var BODS = ((XIDataSource)DataSource.oResult);
                                    oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, BODS.XIGUID.ToString());
                                    oEE.XIDataSource = oDataSource;
                                }
                                else
                                {
                                    oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, iDataSoruceXIGUID.ToString());
                                    oEE.XIDataSource = oDataSource;
                                }
                            }
                            else if(oBOD.TableName.ToLower() == "XIRoleMenus_T".ToLower() || oBOD.TableName.ToLower() == "XIMenu_T".ToLower())
                            {
                                oEE.XIDataSource = oQE.XIDataSource;
                            }
                            else if (oBOD.iOrgObject > 0 || oBOD.TableName == "RefTraceStage_T" || oBOD.TableName == "refValidTrace_T" || oBOD.TableName == "refLeadQuality_T" || oBOD.TableName == "TraceTransactions_T" || oBOD.TableName == "XIOrgApplicationSettings_T")
                            {
                                //if (!string.IsNullOrEmpty(oInfo.sDatabaseName))
                                //{
                                //    var DataSource = oXID.Get_DataSourceDefinition(oInfo.sDatabaseName);
                                //    var BODS = ((XIDataSource)DataSource.oResult);
                                //    oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, BODS.XIGUID.ToString());
                                //    oEE.XIDataSource = oDataSource;
                                //}
                                //else
                                {
                                    oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, iDataSoruceXIGUID.ToString());
                                    oEE.XIDataSource = oDataSource;
                                }
                            }
                            else if (SwitchDataSrcIDXIGUID != null && SwitchDataSrcIDXIGUID != Guid.Empty)
                            {
                                oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, SwitchDataSrcIDXIGUID.ToString());
                                oEE.XIDataSource = oDataSource;
                            }
                            else if (iSwitchDataSrcID > 0)
                            {
                                oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, iSwitchDataSrcID.ToString());
                                oEE.XIDataSource = oDataSource;
                            }
                            else
                            {
                                oEE.XIDataSource = oQE.XIDataSource;
                            }
                            oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "DataSource:" + oEE.XIDataSource.sDatabase + "-" + oEE.XIDataSource.ID });
                            oEE.sSQL = sSql;
                            oEE.SqlParams = SqlParams;
                            var oQResult = oEE.Execute();
                            if (oQResult.bOK && oQResult.oResult != null)
                            {
                                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "BO Instance query executed successfully" });
                                oBOI = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.FirstOrDefault();
                                if (oBOI != null)
                                {
                                    oBOI.BOD = oQE.QParams.FirstOrDefault().BOD;
                                    //foreach (var oAttr in oBOI.Attributes)
                                    //{
                                    //    oAttr.Value.sDisplayName = oBOI.BOD.Attributes.Where(x => x.Key.ToLower() == oAttr.Key.ToLower()).Select(t => t.Value.LabelName).FirstOrDefault();
                                    //}
                                }
                            }
                            else if(oQResult.oResult == null && oQResult.sMessage != null)
                            {
                                CResult oCrException= new CResult();
                                oCrException.sMessage = "ERROR: - BOName: " + sBODName + " - ID: " + sUID + " - Group Name: " + sGroupName +" "+ oQResult.sMessage;
                                oCrException.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCrException.iLogLevel = (int)EnumXIErrorPriority.Critical;
                                oCrException.iCriticality= (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                                SaveErrortoDB(oCrException);
                            }
                        }
                    }
                    else if (oDataSource.sQueryType.ToLower() == "mongodb")
                    {
                        //XIDBMongoDB oMongoDB = new XIDBMongoDB();
                        //oMongoDB.sTable = oBOD.TableName;
                        //oMongoDB.sServer = oDataSource.sServer;
                        //oMongoDB.sDatabase = oDataSource.sDatabase;
                        //oMongoDB.sUID = sUID;
                        //oMongoDB.oWhrParams = oWhrParams;
                        //oMongoDB.sPrimaryKey = oBOD.sPrimaryKey;
                        //oCR = oMongoDB.Get_Data();
                        //if (oCR.bOK && oCR.oResult != null)
                        //{
                        //    var Data = ((List<BsonDocument>)oCR.oResult).FirstOrDefault();
                        //    var Attrs = Data.Names.ToList();
                        //    var Values = Data.Values.ToList();
                        //    oBOI = new XIIBO();
                        //    Dictionary<string, XIIAttribute> dictionary = new Dictionary<string, XIIAttribute>();

                        //    for (var i = 0; i < Attrs.Count(); i++)
                        //    {
                        //        dictionary[Attrs[i]] = new XIIAttribute() { sName = Attrs[i], sValue = Values[i].RawValue.ToString() };
                        //    }
                        //    oBOI.Attributes = dictionary;
                        //    oBOI.BOD = oBOD;
                        //}
                    }
                }
                //var BODD = oXID.Get_BODefinition("acpolicy_t");

                if (oBOI != null && sBODName == "QS Instance")
                {
                    string iInsatnceID = string.Empty;
                    if (oBOI.Attributes.ContainsKey("xiguid"))
                    {
                        iInsatnceID = oBOI.Attributes["xiguid"].sValue;
                    }
                    else
                    {
                        iInsatnceID = oBOI.Attributes["id"].sValue;
                    }
                    XIIQS oQsInstance = new XIIQS();
                    if (!string.IsNullOrEmpty(iInsatnceID))
                    {
                        XIIXI oXII = new XIIXI();
                        XIDStructure oStructure = new XIDStructure();
                        //oQsInstance = oXII.GetQuestionSetInstanceByID(0, Convert.ToInt32(iInsatnceID));
                        oQsInstance = oXII.GetQSXIValuesByQSIID(iInsatnceID, iOrgID);
                        if (oQsInstance != null)
                        {
                            oBOI.XIIValues = oQsInstance.XIValues;
                            foreach (var item in oBOI.XIIValues)
                            {
                                oBOI.BOD.Attributes[item.Key.ToLower()] = new XIDAttribute { IsOptionList = item.Value.IsOptionList, OptionList = item.Value.FieldOptionList, iEnumBOD = item.Value.iEnumBOD };
                                if (!bIsCopy)
                                {
                                    oBOI.Attributes[item.Key.ToLower()] = new XIIAttribute { sName = item.Key.ToLower(), sValue = item.Value.sValue };
                                }
                            }
                        }
                    }
                }
                if (oBOI != null)
                {
                    if (oBOI.iBODID == 0 && oBOI.BOD != null)
                    {
                        oBOI.iBODID = oBOI.BOD.BOID;
                        oBOI.BOIDXIGUID = oBOI.BOD.XIGUID;
                    }
                    oBOI.oParent = oBOI;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - BOName: " + sBODName + " - ID: " + sUID + " - Group Name: " + sGroupName;
                SaveErrortoDB(oCResult);
            }
            return oBOI;
        }

        public XIIQS QSI(string sUID, string sLoadSteps = "")
        {
            XIIQS oQSI = new XIIQS();
            long iID = 0;
            XIDQS oQSD = new XIDQS();

            // sLoadSteps - if "" then load all. otherwise for example:
            // Step1,Step3,Step9,Step10
            // load steps keyed by name (which must be unique and if it gets same name again just dont add in)
            // the steps are instances
            // KEY BY LOWER CASE ONLY

            // TO DO - get oQSD from the cache

            if (long.TryParse(sUID, out iID))
            {
            }
            else
            {
            }

            if (oQSI != null)
            {
                oQSI.oDefintion = oQSD;  // RAVI: if we reference it here, how will it load it again when it is in the cache? 

                oQSI.oParent = this;
            }

            return oQSI;
        }

        public CResult CreateQSI(string sQSName, string iQSID = "", string sSteps = "", string sMode = "", int iBODID = 0, int iBOIID = 0, string sCurrentUserGUID = "", int FKiQSSourceID = 0, string sExternalRefID = "", int iOriginID = 0, string sGUID = null, string ParentQSIID = null, int iOrgID = 0)
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
                XIIQS oQSI = null;
                long iID;
                int QSID = 0;
                Guid QSGUID = Guid.Empty;
                int.TryParse(iQSID, out QSID);
                Guid.TryParse(iQSID, out QSGUID);
                // TO DO - if no steps specified it assumes all steps in the QS defintion

                // TO DO - insert this QS and return its UID 
                // TO DO - instance the specified steps and FK them to this instance
                if (!string.IsNullOrEmpty(sQSName) || (QSID > 0 || (QSGUID != null && QSGUID != Guid.Empty)))
                {
                    var sSessionID = string.Empty;
                    if (HttpContext.Current == null)
                    {

                    }
                    else
                    {
                        //sSessionID = HttpContext.Current.Session.SessionID;
                    }
                    XIInfraCache oCache = new XIInfraCache();
                    var oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, iQSID.ToString(), sSessionID, sGUID);

                    XIIQS oQSIN = null;
                    //Dictionary<string, object> Params = new Dictionary<string, object>();
                    //Params["FKiQSDefinitionID"] = iQSID;
                    //Params["FKiUserCookieID"] = sCurrentUserGUID;
                    //oQSIN = Connection.Select<XIIQS>("XIQSInstance_T", Params).FirstOrDefault();

                    XIIBO oBOI = new XIIBO();
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "QS Instance");
                    oBOI.BOD = oBOD;
                    if (oQSIN == null && !oQSD.bInMemoryOnly)
                    {
                        oQSIN = new XIIQS();
                        oBOI.SetAttribute("FKiQSDefinitionID", QSID.ToString());
                        oBOI.SetAttribute("FKiSourceID", FKiQSSourceID.ToString());
                        oBOI.SetAttribute("sExternalRefID", sExternalRefID);
                        oBOI.SetAttribute("FKiUserCookieID", sCurrentUserGUID);
                        oBOI.SetAttribute("sQSName", oQSD.sName);
                        oBOI.SetAttribute("iCurrentStepIDXIGUID", Guid.Empty.ToString());
                        oBOI.SetAttribute("CreatedTime", DateTime.Now.ToString());
                        oBOI.SetAttribute("FKiClassID", oQSD.FKiClassID.ToString());
                        oBOI.SetAttribute("XIDeleted", "0");
                        oBOI.SetAttribute("FKiOriginID", iOriginID.ToString());
                        if (iOrgID > 0)
                        {
                            oBOI.SetAttribute("FKiOrgID", iOrgID.ToString());
                        }
                        int iParentQSIID = 0;
                        Guid ParentQSGUID = Guid.Empty;
                        int.TryParse(ParentQSIID, out iParentQSIID);
                        Guid.TryParse(ParentQSIID, out ParentQSGUID);
                        if (iParentQSIID > 0)
                        {
                            oBOI.SetAttribute("iParentQSIID", iParentQSIID.ToString());
                        }
                        else if (ParentQSGUID != null && ParentQSGUID != Guid.Empty)
                        {
                            oBOI.SetAttribute("iParentQSIIDXIGUID", ParentQSGUID.ToString());
                        }
                        oBOI.SetAttribute("FKiQSDefinitionIDXIGUID", oQSD.XIGUID.ToString());
                        oCR = oBOI.Save(oBOI);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oBOI = (XIIBO)oCR.oResult;
                            Dictionary<string, object> oLinks = new Dictionary<string, object>();
                            if (oBOI.Attributes.ContainsKey("xiguid"))
                            {
                                var XIGUID = oBOI.AttributeI("xiguid").sValue;
                                oLinks["XIGUID"] = XIGUID;
                            }
                            else
                            {
                                var XIGUID = oBOI.AttributeI("id").sValue;
                                oLinks["id"] = XIGUID;
                            }
                            oQSIN = Connection.Select<XIIQS>("XIQSInstance_T", oLinks).FirstOrDefault();
                            oQSI = oQSIN;
                            oCResult.oResult = oQSI;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                        //cConnectionString oConString = new cConnectionString();
                        //string sConString = oConString.ConnectionString(sCoreDatabase);
                        //XIDBAPI Connection = new XIDBAPI(sConString);
                        //oQSIN = Connection.Insert<XIIQS>(oQSIN, "XIQSInstance_T", "ID");
                        //oQSI = oQSIN;
                    }
                    else
                    {
                        oQSIN = new XIIQS();
                        oBOI.SetAttribute("FKiQSDefinitionID", QSID.ToString());
                        oBOI.SetAttribute("FKiQSDefinitionIDXIGUID", oQSD.XIGUID.ToString());
                        oBOI.SetAttribute("FKiUserCookieID", sCurrentUserGUID);
                        oBOI.SetAttribute("sQSName", oQSD.sName);
                        oBOI.SetAttribute("iCurrentStepIDXIGUID", Guid.Empty.ToString());
                        if (iOrgID > 0)
                        {
                            oBOI.SetAttribute("FKiOrgID", iOrgID.ToString());
                        }
                        oCR = oBOI.Save(oBOI);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oBOI = (XIIBO)oCR.oResult;
                            var XIGUID = oBOI.AttributeI("xiguid").sValue;
                            Dictionary<string, object> oLinks = new Dictionary<string, object>();
                            oLinks["XIGUID"] = XIGUID;
                            oQSIN = Connection.Select<XIIQS>("XIQSInstance_T", oLinks).FirstOrDefault();
                            oQSI = oQSIN;
                            oCResult.oResult = oQSI;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                        //oQSI = oQSIN;
                        //oCResult.oResult = oQSI;
                        //oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        //SaveQSInstance();
                        //oQSI = GetQuestionSetInstanceByID(iQSID, oQSIN.ID, sMode, iBODID, iBOIID, sCurrentUserGUID);
                    }
                }

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

        public XIIQS GetQSInstanceByID(string iQSIID)
        {
            int QSIID = 0;
            Guid QSXIGUID = Guid.Empty;
            int.TryParse(iQSIID, out QSIID);
            Guid.TryParse(iQSIID, out QSXIGUID);
            XIIQS oQSIns = new XIIQS();
            if ((QSXIGUID != Guid.Empty && QSXIGUID != null) || QSIID > 0)
            {
                Dictionary<string, object> StepParams = new Dictionary<string, object>();
                if (QSXIGUID != Guid.Empty && QSXIGUID != null)
                {
                    StepParams["XIGUID"] = QSXIGUID;
                }
                else if (QSIID > 0)
                {
                    StepParams["ID"] = iQSIID;
                }
                oQSIns = Connection.Select<XIIQS>("XIQSInstance_T", StepParams).FirstOrDefault();
                //oQSIns = dbContext.QSInstance.Find(iQSIID);
            }
            return oQSIns;
        }

        public XIIQS GetQuestionSetInstanceByID_old(int iQSID, int iQSIID, string sMode, int iBODID, int iInstanceID, string sCurrentUserGUID)
        {
            XIIQS oQSInstance = new XIIQS();
            oQSInstance.FKiQSDefinitionID = iQSID;
            XIDXI oDXI = new XIDXI();
            //var oQSDefinition = oDXI.GetQuestionSetDefinitionByID(null, iQSID.ToString());
            //oQSInstance.QSDefinition = oQSDefinition;

            //Load QS Instance
            string sQSInstanceQry = "select * from XIQSInstance_T QSI " +
              "left join XIQSStepInstance_T QSSI on QSI.ID = QSSI.FKiQSInstanceID " +
              "left join XIQSStepDefinition_T QSSD on QSSI.FKiQSStepDefinitionID = QSSD.ID " +
              "left join XIFieldInstance_T FI on QSSI.FKiQSStepDefinitionID = FI.FKiQSStepDefinitionID " +
              "left join XIFieldDefinition_T FD on FD.ID = FI.FKiFieldDefinitionID " +
              "left join XIFieldOrigin_T FO on FO.ID = FD.FKiXIFieldOriginID " +
                "WHERE QSI.FKiQSDefinitionID = @id";

            var param = new
            {
                id = iQSID,
                CurrentGuestUser = sCurrentUserGUID,
                BODID = iBODID,
                BOIID = iInstanceID,
                QSIID = iQSIID
            };
            if (iQSIID > 0)
            {
                sQSInstanceQry = sQSInstanceQry + " and QSI.ID = @QSIID and FI.FKiQSInstanceID=@QSIID";
            }
            if (!string.IsNullOrEmpty(sCurrentUserGUID))
            {
                sQSInstanceQry = sQSInstanceQry + " and QSI.FKiUserCookieID = @CurrentGuestUser";
            }
            if (!string.IsNullOrEmpty(sMode) && iBODID > 0 && iInstanceID > 0)
            {
                if (sMode.ToLower() == "Popup".ToLower())
                {
                    sQSInstanceQry = sQSInstanceQry + " and QSI.FKiBODID = @BODID and QSI.iBOIID = @BOIID";
                }
            }
            var lookupQSIns = new Dictionary<int, XIIQS>();
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                Conn.Open();

                var lookupStepIns = new Dictionary<int, XIIQSStep>();
                var lookupFieldIns = new Dictionary<long, XIIValue>();
                Conn.Query<XIIQS, XIIQSStep, XIDQSStep, XIIValue, XIDFieldDefinition, XIDFieldOrigin, XIIQS>(sQSInstanceQry,
                    (QS, StepIns, StepD, FieldInstance, FieldD, FieldOrigin) =>
                    {
                        XIIQS oQSIns = new XIIQS();
                        if (QS != null)
                        {
                            if (!lookupQSIns.TryGetValue(QS.ID, out oQSIns))
                            {
                                lookupQSIns.Add(QS.ID, oQSIns = QS);
                            }
                            XIIQSStep oQSSIns;
                            if (StepIns != null)
                            {
                                if (!lookupStepIns.TryGetValue(StepIns.ID, out oQSSIns))
                                {
                                    lookupStepIns.Add(StepIns.ID, oQSSIns = StepIns);
                                    if (oQSIns.HistoryXIGUID == null)
                                    {
                                        oQSIns.HistoryXIGUID = new List<Guid>();
                                    }
                                    oQSIns.HistoryXIGUID.Add(StepD.XIGUID);
                                    oQSIns.Steps[StepD.sName] = oQSSIns;
                                }
                                if (FieldInstance != null)
                                {
                                    if (FieldOrigin != null)
                                    {
                                        XIIValue oFVInstance;
                                        if (!lookupFieldIns.TryGetValue(FieldInstance.ID, out oFVInstance))
                                        {
                                            lookupFieldIns.Add(FieldInstance.ID, oFVInstance = FieldInstance);
                                            if (!oQSSIns.XIValues.ContainsKey(FieldOrigin.sName))
                                                oQSSIns.XIValues[FieldOrigin.sName] = FieldInstance;
                                        }
                                    }
                                }
                            }
                        }
                        return oQSIns;
                    },
                    param
                    ).AsQueryable();
            }
            var oQSInsFinal = lookupQSIns.Values.FirstOrDefault();
            if (oQSInsFinal != null && oQSInsFinal.Steps != null && oQSInsFinal.Steps.Count() > 0)
            {
                oQSInstance.iCurrentStepIDXIGUID = oQSInsFinal.iCurrentStepIDXIGUID;
                oQSInstance.Steps = oQSInsFinal.Steps;
                var StepIns = GetSectionInstances(iQSID, iQSIID, sMode, iBODID, iInstanceID, sCurrentUserGUID).Steps;

                foreach (var items in StepIns)
                {
                    var oQSST = oQSInsFinal.Steps.Where(m => m.Value.FKiQSStepDefinitionID == items.Value.FKiQSStepDefinitionID).FirstOrDefault();
                    if (oQSST.Value != null)
                    {
                        oQSInsFinal.Steps.Where(m => m.Value.FKiQSStepDefinitionID == items.Value.FKiQSStepDefinitionID).FirstOrDefault().Value.Sections = items.Value.Sections;
                    }

                }
                oQSInsFinal.QSDefinition = oDXI.GetQuestionSetDefinitionByID(null, iQSID.ToString());
            }
            else
            {
                oQSInsFinal = new XIIQS();
                oQSInsFinal.QSDefinition = oDXI.GetQuestionSetDefinitionByID(null, iQSID.ToString());
                oQSInsFinal.FKiQSDefinitionID = oQSInsFinal.QSDefinition.ID;
                oQSInsFinal.FKiQSDefinitionIDXIGUID = oQSInsFinal.QSDefinition.XIGUID;
                oQSInsFinal = oQSInsFinal.Save(oQSInsFinal, sCurrentUserGUID);
            }

            return oQSInsFinal;
        }

        private XIIQS GetSectionInstances(int iQSID, int iQSIID, string sMode, int iBODID, int iInstanceID, string sCurrentUserGUID)
        {

            XIIQS oQSInstance = new XIIQS();
            oQSInstance.FKiQSDefinitionID = iQSID;

            //Load QS Instance
            string sQSInstanceQry = "select * from XIQSInstance_T QSI " +
                "inner join XIQSStepInstance_T QSSI on QSI.ID = QSSI.FKiQSInstanceID " +
                "left join XIStepSectionInstance_T SecI on QSSI.ID = SecI.FKiStepInstanceID " +
                "left join XIFieldInstance_T SFI on SecI.FKiStepSectionDefinitionID = SFI.FKiQSSectionDefinitionID " +
                "left join XIFieldDefinition_T XIFD on SFI.FKiFieldDefinitionID = XIFD.ID " +
                "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginID = XIO.ID " +
                //"left join XIFullAddress_T FA on SecI.ID = FA.FKiSectionInstanceID " +
                "WHERE (QSI.ID = SFI.FKiQSInstanceID or SFI.FKiQSInstanceID is null) and QSI.FKiQSDefinitionID = @id";

            var param = new
            {
                id = iQSID,
                CurrentGuestUser = sCurrentUserGUID,
                BODID = iBODID,
                BOIID = iInstanceID,
                QSIID = iQSIID
            };
            if (iQSIID > 0)
            {
                sQSInstanceQry = sQSInstanceQry + " and QSI.ID = @QSIID";
            }
            if (!string.IsNullOrEmpty(sCurrentUserGUID))
            {
                sQSInstanceQry = sQSInstanceQry + " and QSI.FKiUserCookieID = @CurrentGuestUser";
            }
            if (!string.IsNullOrEmpty(sMode) && iBODID > 0 && iInstanceID > 0)
            {
                if (sMode.ToLower() == "Popup".ToLower())
                {
                    sQSInstanceQry = sQSInstanceQry + " and QSI.FKiBODID = @BODID and QSI.iBOIID = @BOIID";
                }
            }
            var lookupQSIns = new Dictionary<int, XIIQS>();
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                Conn.Open();
                var lookupStepIns = new Dictionary<int, XIIQSStep>();
                var lookupSectionIns = new Dictionary<int, XIIQSSection>();
                var lookupFieldIns = new Dictionary<long, XIIValue>();
                Conn.Query<XIIQS, XIIQSStep, XIIQSSection, XIIValue, XIDFieldDefinition, XIDFieldOrigin, XIIQS>(sQSInstanceQry,
                    (QS, StepIns, SectionInstance, FieldInstance, FieldDefinition, FieldOrigin) =>
                    {
                        XIIQS oQSIns = new XIIQS();
                        if (QS != null)
                        {
                            if (!lookupQSIns.TryGetValue(QS.ID, out oQSIns))
                            {
                                lookupQSIns.Add(QS.ID, oQSIns = QS);
                            }
                            XIIQSStep oQSSIns;
                            if (!lookupStepIns.TryGetValue(StepIns.ID, out oQSSIns))
                            {
                                lookupStepIns.Add(StepIns.ID, oQSSIns = StepIns);
                                oQSIns.Steps[oQSSIns.ID.ToString()] = oQSSIns;
                            }

                            XIIQSSection oStepSectionIns;
                            if (SectionInstance != null)
                            {
                                if (!lookupSectionIns.TryGetValue(SectionInstance.ID, out oStepSectionIns))
                                {
                                    lookupSectionIns.Add(SectionInstance.ID, oStepSectionIns = SectionInstance);
                                    //oQSSIns.Sections[oStepSectionIns.FKiStepSectionDefinitionID.ToString() + "_Sec"] = oStepSectionIns;
                                    oQSSIns.Sections[oStepSectionIns.FKiStepSectionDefinitionIDXIGUID.ToString() + "_Sec"] = oStepSectionIns;
                                }

                                XIIValue oFVInstance;
                                if (FieldInstance != null)
                                {
                                    if (!lookupFieldIns.TryGetValue(FieldInstance.ID, out oFVInstance))
                                    {
                                        if (FieldOrigin != null)
                                        {
                                            lookupFieldIns.Add(FieldInstance.ID, oFVInstance = FieldInstance);
                                            if (!oStepSectionIns.XIValues.ContainsKey(FieldOrigin.sName))
                                                oStepSectionIns.XIValues[FieldOrigin.sName] = FieldInstance;
                                        }
                                    }
                                }
                            }
                        }
                        return oQSIns;
                    },
                    param
                    ).AsQueryable();
            }
            var oQSInsFinal = lookupQSIns.Values.FirstOrDefault();

            return oQSInsFinal;
        }

        public string ConvertToDateTime(string InputString, string sFormatType)
        {
            try
            {
                //return String.Format("{0:" + InputString + "}", InputValue);
                CultureInfo provider = CultureInfo.InvariantCulture;
                string MyDate = "";
                // DateTime dt = DateTime.ParseExact(InputString, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                // string MyDate = dt.ToString("MM-dd-yyyy");
                string[] formats = {
                  "yyyy-MM-dd", "yyyy-MMM-dd", "yyyy.MM.dd","yyyy/MM/dd","yyyy/MMM/dd","yyyy.MMM.dd",
                  "dd-MM-yyyy","dd.MM.yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "dd.MMM.yyyy",
                  "MMM-dd-yyyy","MM-dd-yyyy", "MM.dd.yyyy", "MMM.dd.yyyy", "MM/dd/yyyy"
              };
                DateTime dateValue;
                // var dt = "26.May.1975";
                bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);
                if (!IsValidDate)
                {
                    dateValue = DateTime.MinValue;
                    MyDate = null;
                }
                else
                {
                    if (!string.IsNullOrEmpty(sFormatType))
                    {
                        MyDate = dateValue.ToString(sFormatType);
                    }
                    else
                    {
                        MyDate = dateValue.Date.ToShortDateString();
                    }

                }
                return MyDate;
            }
            catch (Exception ex)
            {
                // ServiceUtil.LogError(ex);
                throw ex;
            }
        }
        public bool IsValidDate(string InputString)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            string[] formats = {
                  "yyyy-MM-dd", "yyyy-MMM-dd", "yyyy.MM.dd","yyyy/MM/dd","yyyy/MMM/dd","yyyy.MMM.dd",
                  "dd-MM-yyyy","dd.MM.yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "dd.MMM.yyyy",
                  "MMM-dd-yyyy","MM-dd-yyyy", "MM.dd.yyyy", "MMM.dd.yyyy", "MM/dd/yyyy","M/dd/yyyy","MM/d/yyyy","M/dd/yyyy hh:mm:ss a"
              };
            DateTime dateValue;
            // var dt = "26.May.1975";
            bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);
            return IsValidDate;
        }
        public static string ConvertToString(string InputString, string InputValue)
        {
            try
            {
                //DateTime dt = DateTime.ParseExact(InputValue, "dd-mm-yyyy hh:mm:ss", CultureInfo.InvariantCulture);
                DateTime dt = Convert.ToDateTime(InputValue);
                return dt.ToString("dd-MMM-yyyy");
            }
            catch (Exception ex)
            {
                // ServiceUtil.LogError(ex);
                throw ex;
            }
        }
        public XIIQS GetQuestionSetInstanceByID(string iQSDID = "", string iQSIID = "", string sMode = "", int iBODID = 0, int iBOIID = 0, string sCurrentUserGUID = "", int iParentQSIID = 0)
        {
            int QSDID = 0;
            Guid QSDXIGUID = Guid.Empty;
            int.TryParse(iQSDID, out QSDID);
            Guid.TryParse(iQSDID, out QSDXIGUID);
            int QSIID = 0;
            Guid QSIXIGUID = Guid.Empty;
            int.TryParse(iQSIID, out QSIID);
            Guid.TryParse(iQSIID, out QSIXIGUID);
            XIInfraCache oCache = new XIInfraCache();
            XIIQS oQSInstance = new XIIQS();
            oQSInstance.FKiQSDefinitionID = QSDID;
            oQSInstance.FKiQSDefinitionIDXIGUID = QSDXIGUID;
            XIDXI oDXI = new XIDXI();
            XIDQS oQSD = new XIDQS();
            XIInfraEncryption oEncrypt = new XIInfraEncryption();
            if (QSDID == 0 && (QSIID > 0 || (QSIXIGUID != null && QSIXIGUID != Guid.Empty)))
            {
                var oQSI = new XIIQS();
                if (QSIXIGUID != null && QSIXIGUID != Guid.Empty)
                    oQSI = GetQSInstanceByID(QSIXIGUID.ToString());
                else if (QSIID > 0)
                    oQSI = GetQSInstanceByID(QSIID.ToString());
                QSDID = oQSI.FKiQSDefinitionID;
                QSIXIGUID = oQSI.XIGUID;
                iQSDID = QSDID.ToString();
                QSDXIGUID = oQSI.FKiQSDefinitionIDXIGUID;
            }
            var oQSDefinition = new XIDQS();
            if (QSDXIGUID != null && QSDXIGUID != Guid.Empty)
            {
                oQSDefinition = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, QSDXIGUID.ToString());//oDXI.GetQuestionSetDefinitionByID(null, iQSID.ToString());
            }
            else
                oQSDefinition = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, iQSDID.ToString());//oDXI.GetQuestionSetDefinitionByID(null, iQSID.ToString());
            var oQSDefinitionC = (XIDQS)oQSDefinition.Clone(oQSDefinition);
            oQSInstance.QSDefinition = oQSDefinitionC;
            try
            {
                //var param = new
                //{
                //    id = iQSDID,
                //    CurrentGuestUser = sCurrentUserGUID,
                //    BODID = iBODID,
                //    BOIID = iBOIID,
                //    QSIID = iQSIID
                //};
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (QSDXIGUID != null && QSDXIGUID != Guid.Empty)
                {
                    Params["FKiQSDefinitionIDXIGUID"] = QSDXIGUID;
                }
                else if (QSDID > 0)
                {
                    Params["FKiQSDefinitionID"] = QSDID;
                }
                if (QSIXIGUID != null && QSIXIGUID != Guid.Empty)
                {
                    Params["XIGUID"] = QSIXIGUID;
                }
                else if (QSIID > 0)
                {
                    Params["ID"] = QSIID;
                }
                if (!string.IsNullOrEmpty(sCurrentUserGUID))
                {
                    Params["FKiUserCookieID"] = sCurrentUserGUID;
                }
                if (!string.IsNullOrEmpty(sMode) && iBODID > 0 && iBOIID > 0)
                {
                    if (sMode.ToLower() == "Popup".ToLower())
                    {
                        Params["FKiBODID"] = iBODID;
                    }
                }
                XIDXI oXID = new XIDXI();
                oQSInstance = Connection.Select<XIIQS>("XIQSInstance_T ", Params).FirstOrDefault();
                if (oQSInstance != null)
                {
                    Dictionary<string, object> QSInsParams = new Dictionary<string, object>();
                    var oStepD = new XIDQSStep();
                    //var QSFieldInstan = new List<XIIValue>();
                    //var QSFieldOrigin = new XIDFieldOrigin();
                    var QSFieldInstan = new List<XIIValue>();
                    Dictionary<string, object> FieldInsParams = new Dictionary<string, object>();
                    FieldInsParams["FKiQSInstanceIDXIGUID"] = oQSInstance.XIGUID;
                    QSFieldInstan = Connection.Select<XIIValue>("XIFieldInstance_T", FieldInsParams).ToList();
                    if (QSFieldInstan != null && QSFieldInstan.Count() > 0)
                    {
                        //oCResult.sMessage = "GetQuestionSetInstanceByID - XIValues Count:" + QSFieldInstan.Count();
                        //SaveErrortoDB(oCResult);
                        foreach (var oFieldD in QSFieldInstan)
                        {
                            //oCResult.sMessage = "GetQuestionSetInstanceByID - XIValues for Field:" + oFieldD.FKiFieldOriginIDXIGUID;
                            //SaveErrortoDB(oCResult);
                            if (oFieldD.FKiFieldOriginIDXIGUID != null && oFieldD.FKiFieldOriginIDXIGUID != Guid.Empty)
                            {
                                Dictionary<string, object> FieldOrgParams = new Dictionary<string, object>();
                                FieldOrgParams["XIGUID"] = oFieldD.FKiFieldOriginIDXIGUID;
                                var QSFieldOriginD = Connection.Select<XIDFieldOrigin>("XIFieldOrigin_T", FieldOrgParams).FirstOrDefault();
                                //oCResult.sMessage = "GetQuestionSetInstanceByID - XIValues for Field:" + QSFieldOriginD.sName + " & Value: " + oFieldD.sValue;
                                //SaveErrortoDB(oCResult);
                                oQSInstance.XIValues[QSFieldOriginD.sName] = oFieldD;
                            }
                            else
                            {
                                //oCResult.sMessage = "GetQuestionSetInstanceByID - XIValues for Field origin is empty for:" + oFieldD.ID;
                                //SaveErrortoDB(oCResult);
                            }
                        }
                    }
                    else
                    {
                        oCResult.sMessage = "GetQuestionSetInstanceByID - XIValues Count:0";
                        SaveErrortoDB(oCResult);

                    }
                    var QSFieldOrigin = new XIDFieldOrigin();

                    if (iParentQSIID > 0) // Mostly It will be useful in Run-MTA Case
                    {
                        QSInsParams["FKiQSInstanceID"] = iParentQSIID;
                    }
                    else
                    {
                        QSInsParams["FKiQSInstanceID"] = oQSInstance.ID;
                    }

                    var QSStepInstan = Connection.Select<XIIQSStep>("XIQSStepInstance_T", QSInsParams).ToList();
                    foreach (var oStepI in QSStepInstan)
                    {
                        oStepD = oQSDefinition.Steps.Values.Where(m => m.XIGUID == oStepI.FKiQSStepDefinitionIDXIGUID).FirstOrDefault();
                        Dictionary<string, object> StepSectionParams = new Dictionary<string, object>();
                        StepSectionParams["FKiStepInstanceIDXIGUID"] = oStepI.XIGUID;
                        var QSSections = Connection.Select<XIIQSSection>("XIStepSectionInstance_T", StepSectionParams).ToList();
                        foreach (var oSectionI in QSSections)
                        {
                            FieldInsParams = new Dictionary<string, object>();
                            FieldInsParams["FKiSectionInstanceIDXIGUID"] = oSectionI.XIGUID;
                            if (iParentQSIID > 0)
                            {
                                FieldInsParams["FKiQSInstanceID"] = iParentQSIID;
                            }
                            QSFieldInstan = Connection.Select<XIIValue>("XIFieldInstance_T", FieldInsParams).ToList();
                            foreach (var oFieldI in QSFieldInstan)
                            {
                                Dictionary<string, object> FieldDefParams = new Dictionary<string, object>();
                                FieldDefParams["XIGUID"] = oFieldI.FKiFieldDefinitionIDXIGUID;
                                var oFieldD = Connection.Select<XIDFieldDefinition>("XIFieldDefinition_T", FieldDefParams).FirstOrDefault();
                                if (oFieldD != null)
                                {
                                    Dictionary<string, object> FieldOrgParams = new Dictionary<string, object>();
                                    FieldOrgParams["XIGUID"] = oFieldD.FKiXIFieldOriginIDXIGUID;
                                    QSFieldOrigin = Connection.Select<XIDFieldOrigin>("XIFieldOrigin_T", FieldOrgParams).FirstOrDefault();
                                    if (QSFieldOrigin.bIsEncrypt && !string.IsNullOrEmpty(oFieldI.sValue))
                                    {
                                        oFieldI.sValue = oEncrypt.DecryptData(oFieldI.sValue, true, oFieldI.ID.ToString());
                                        oFieldI.sDerivedValue = oFieldI.sValue;
                                    }
                                    oFieldI.sResolvedValue = oFieldI.sValue;
                                    // oSectionI.XIValues[QSFieldOrigin.sName] = oFieldI;
                                    if (QSFieldOrigin.bIsOptionList || (QSFieldOrigin.FK1ClickID > 0 || (QSFieldOrigin.FK1ClickIDXIGUID != null && QSFieldOrigin.FK1ClickIDXIGUID != Guid.Empty)))
                                    {
                                        oFieldI.IsOptionList = QSFieldOrigin.bIsOptionList;
                                        QSFieldOrigin.FieldOptionList = oQSDefinition.XIDFieldOrigin.Where(x => x.Key.ToLower() == QSFieldOrigin.sName.ToLower()).Select(x => x.Value.FieldOptionList).FirstOrDefault();
                                        if (QSFieldOrigin.FieldOptionList != null)
                                        {
                                            oFieldI.sResolvedValue = QSFieldOrigin.FieldOptionList.Where(m => m.sOptionValue == oFieldI.sValue).Select(m => m.sOptionName).FirstOrDefault();
                                            oFieldI.sResolvedValueCode = QSFieldOrigin.FieldOptionList.Where(m => m.sOptionValue == oFieldI.sValue).Select(m => m.sOptionCode).FirstOrDefault();
                                        }
                                        //oFieldI.FieldOptionList = QSFieldOrigin.FieldOptionList.Select(m => new XIDOptionList
                                        //{
                                        //    ID = m.ID,
                                        //    sOptionName = m.sOptionName,
                                        //    sValues = m.sOptionValue
                                        //}).ToList();
                                        //oFieldI.iEnumBOD = QSFieldOrigin.FK1ClickID;
                                        //XIValues.sValue = FieldDefinition.Value.FieldOrigin.FieldOptionList.Where(m => m.sOptionValue == FieldInstance.Value.sValue).Select(m => m.sOptionName).FirstOrDefault();
                                    }
                                    else if (QSFieldOrigin.FKiBOID > 0 || (QSFieldOrigin.FKiBOIDXIGUID != null && QSFieldOrigin.FKiBOIDXIGUID != Guid.Empty))
                                    {
                                        var oBOD = new XIDBO();
                                        if (QSFieldOrigin.FKiBOIDXIGUID != null && QSFieldOrigin.FKiBOIDXIGUID != Guid.Empty)
                                        {
                                            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, QSFieldOrigin.FKiBOIDXIGUID.ToString());
                                        }
                                        else if (QSFieldOrigin.FKiBOID > 0)
                                        {
                                            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, QSFieldOrigin.FKiBOID.ToString());
                                        }
                                        var GroupD = new XIDGroup();
                                        if (oBOD.Groups.TryGetValue("label", out GroupD))
                                        {
                                            XIDBAPI Myconntection = new XIDBAPI(oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID));
                                            if (oBOD != null && oBOD.Groups.Any(group => group.Key.ToLower() == "label") && (oBOD.sSize == "30" || oBOD.sSize == "20") && !string.IsNullOrEmpty(oFieldI.sValue) && oFieldI.sValue != "0")
                                            {
                                                var BODParams = new Dictionary<string, object>();
                                                BODParams[oBOD.sPrimaryKey] = oFieldI.sValue;
                                                string FinalString = oBOD.GroupD("label").ConcatanateGroupFields(" ");//concatenate the string with join String 
                                                if (!string.IsNullOrEmpty(FinalString))
                                                {
                                                    var Result = Myconntection.Select<string>(oBOD.TableName, BODParams, FinalString + " As Result ").FirstOrDefault();
                                                    oFieldI.sResolvedValue = Result;
                                                }
                                            }
                                        }
                                    }
                                    oFieldI.sDisplayName = QSFieldOrigin.sDisplayName;

                                    if (!oQSInstance.XIValues.ContainsKey(QSFieldOrigin.sName))
                                    {
                                        oQSInstance.XIValues[QSFieldOrigin.sName] = oFieldI;
                                    }
                                    oSectionI.XIValues[QSFieldOrigin.sName] = oFieldI;
                                }
                            }
                            oStepI.Sections[oSectionI.FKiStepSectionDefinitionIDXIGUID.ToString() + "_Sec"] = oSectionI;
                        }
                        if (oQSInstance.HistoryXIGUID == null)
                        {
                            oQSInstance.HistoryXIGUID = new List<Guid>();
                        }
                        if (oStepD != null)
                        {
                            oQSInstance.HistoryXIGUID.Add(oStepD.XIGUID);
                            oQSInstance.Steps[oStepD.sName] = oStepI;
                        }
                    }
                }
                else
                {
                    oQSInstance = new XIIQS();
                    oQSInstance.QSDefinition = oQSDefinition;
                    oQSInstance.FKiQSDefinitionID = oQSInstance.QSDefinition.ID;
                    oQSInstance.FKiQSDefinitionIDXIGUID = oQSInstance.QSDefinition.XIGUID;
                    oQSInstance = oQSInstance.Save(oQSInstance, sCurrentUserGUID);
                }
                oQSInstance.QSDefinition = oQSDefinition;
                return oQSInstance;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(st.FrameCount - 1);
                var line = frame.GetFileLineNumber();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - Line Number:" + line + " Message: " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
                throw ex;
            }
        }

        public XIIQS GetXIValuesByQSIID(int iQSIID = 0)
        {
            XIIQS oQSInstance = new XIIQS();
            try
            {
                if (iQSIID > 0)
                {
                    var QSFieldInstan = new List<XIIValue>();
                    var QSFieldOrigin = new XIDFieldOrigin();
                    Dictionary<string, object> FieldInsParams = new Dictionary<string, object>();
                    FieldInsParams["FKiQSInstanceID"] = iQSIID;
                    QSFieldInstan = Connection.Select<XIIValue>("XIFieldInstance_T", FieldInsParams).ToList();
                    if (QSFieldInstan != null && QSFieldInstan.Count() > 0)
                    {
                        var XIValues = QSFieldInstan.ToDictionary(x => x.FKiFieldOriginID.ToString(), x => x);
                        oQSInstance.XIValues = XIValues;
                    }
                }
                return oQSInstance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public XIIQS GetQSXIValuesByQSIID(string iQSIID,int iOrgID=0)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            XIIQS oQSInstance = new XIIQS();
            try
            {
                int QSIID = 0;
                Guid QSXIGUID = Guid.Empty;
                int.TryParse(iQSIID, out QSIID);
                Guid.TryParse(iQSIID, out QSXIGUID);
                Dictionary<int, XIDBAPI> oDataSource = new Dictionary<int, XIDBAPI>();
                Dictionary<int, XIDBO> oBODList = new Dictionary<int, XIDBO>();
                XIInfraCache oCache = new XIInfraCache();
                XIDXI oXID = new XIDXI();

                //Load QS Instance
                string sQSInstanceQry = String.Empty;
                if (QSXIGUID != null && QSXIGUID != Guid.Empty)
                {
                    sQSInstanceQry = "select * from XIQSInstance_T QSI " +
                    "left join XIFieldInstance_T FI on QSI.XIGUID = FI.FKiQSInstanceIDXIGUID " +
                    "left join XIFieldOrigin_T XIO on FI.FKiFieldOriginIDXIGUID = XIO.XIGUID and FI." + XIConstant.Key_XIDeleted + " = 0";
                }
                else if (QSIID > 0)
                {
                    sQSInstanceQry = "select * from XIQSInstance_T QSI " +
                    "left join XIFieldInstance_T FI on QSI.ID = FI.FKiQSInstanceID " +
                    "left join XIFieldOrigin_T XIO on FI.FKiFieldOriginID = XIO.ID and FI." + XIConstant.Key_XIDeleted + " = 0";
                }


                var param = new
                {
                    QSIID = iQSIID,
                    QSXIGUID = QSXIGUID
                };
                if (QSXIGUID != null && QSXIGUID != Guid.Empty)
                {
                    sQSInstanceQry = sQSInstanceQry + " where QSI.XIGUID = @QSXIGUID ";
                }
                else if (QSIID > 0)
                {
                    sQSInstanceQry = sQSInstanceQry + " where QSI.ID = @QSIID ";
                }
                sQSInstanceQry = sQSInstanceQry + " order by QSI.ID desc";
                var oQSI = GetQSInstanceByID(iQSIID);
                if (oQSI != null)
                {
                    Guid iQSDID = oQSI.FKiQSDefinitionIDXIGUID;
                    var oQSDefinition = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, iQSDID.ToString(), "", "", 0, iOrgID);
                    var lookupQSIns = new Dictionary<int, XIIQS>();
                    using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
                    {
                        Conn.Open();
                        var lookupFieldIns = new Dictionary<long, XIIValue>();
                        Conn.Query<XIIQS, XIIValue, XIDFieldOrigin, XIIQS>(sQSInstanceQry,
                            (QS, FieldInstance, FieldOrigin) =>
                            {
                                XIIQS oQSIns = new XIIQS();
                                if (QS != null)
                                {
                                    if (!lookupQSIns.TryGetValue(QS.ID, out oQSIns))
                                    {
                                        lookupQSIns.Add(QS.ID, oQSIns = QS);
                                    }
                                    XIIValue oXIValue;
                                    if (FieldOrigin != null)
                                    {
                                        if (!lookupFieldIns.TryGetValue(FieldInstance.ID, out oXIValue))
                                        {
                                            if (FieldOrigin.bIsEncrypt && !string.IsNullOrEmpty(FieldInstance.sValue))
                                            {
                                                XIInfraEncryption oEncrypt = new XIInfraEncryption();
                                                FieldInstance.sValue = oEncrypt.DecryptData(FieldInstance.sValue, true, FieldInstance.ID.ToString());
                                                FieldInstance.sDerivedValue = FieldInstance.sValue;
                                            }
                                            lookupFieldIns.Add(FieldInstance.ID, oXIValue = FieldInstance);
                                            FieldInstance.sResolvedValue = FieldInstance.sValue;

                                            if (FieldOrigin.bIsOptionList)
                                            {
                                                try
                                                {
                                                    FieldInstance.IsOptionList = FieldOrigin.bIsOptionList;
                                                    FieldOrigin.FieldOptionList = oQSDefinition.XIDFieldOrigin.Where(x => x.Key.ToLower() == FieldOrigin.sName.ToLower()).Select(x => x.Value.FieldOptionList).FirstOrDefault();
                                                    if (FieldOrigin.FieldOptionList != null)
                                                    {
                                                        FieldInstance.sResolvedValue = FieldOrigin.FieldOptionList.Where(m => m.sOptionValue == FieldInstance.sValue).Select(m => m.sOptionName).FirstOrDefault();
                                                        FieldInstance.sResolvedValueCode = FieldOrigin.FieldOptionList.Where(m => m.sOptionValue == FieldInstance.sValue).Select(m => m.sOptionCode).FirstOrDefault();
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    oCResult.sMessage = "Critical Error while loading optionlist QSInstance: " + iQSIID;
                                                    SaveErrortoDB(oCResult);
                                                }

                                                //oFieldI.FieldOptionList = QSFieldOrigin.FieldOptionList.Select(m => new XIDOptionList
                                                //{
                                                //    ID = m.ID,
                                                //    sOptionName = m.sOptionName,
                                                //    sValues = m.sOptionValue
                                                //}).ToList();
                                                //oFieldI.iEnumBOD = QSFieldOrigin.FK1ClickID;
                                                //XIValues.sValue = FieldDefinition.Value.FieldOrigin.FieldOptionList.Where(m => m.sOptionValue == FieldInstance.Value.sValue).Select(m => m.sOptionName).FirstOrDefault();
                                            }
                                            else if (FieldOrigin.FKiBOID > 0 || (FieldOrigin.FKiBOIDXIGUID != null && FieldOrigin.FKiBOIDXIGUID != Guid.Empty))
                                            {
                                                try
                                                {
                                                    XIDBO oBOD = new XIDBO();
                                                    XIInfraCache oCahce = new XIInfraCache();
                                                    if (oBODList.ContainsKey(FieldOrigin.FKiBOID))
                                                    {
                                                        oBOD = oBODList[FieldOrigin.FKiBOID];
                                                    }
                                                    else
                                                    {
                                                        if (FieldOrigin.FKiBOIDXIGUID != null && FieldOrigin.FKiBOIDXIGUID != Guid.Empty)
                                                        {
                                                            oBOD = (XIDBO)oCahce.GetObjectFromCache(XIConstant.CacheBO, null, FieldOrigin.FKiBOIDXIGUID.ToString());
                                                            if (oBOD.iOrgObject == 1)
                                                            {
                                                                oBOD = (XIDBO)oCahce.GetObjectFromCache(XIConstant.CacheBO, null, FieldOrigin.FKiBOIDXIGUID.ToString(), null, null, 0, iOrgID);
                                                            }
                                                        }
                                                        else if (FieldOrigin.FKiBOID > 0)
                                                        {
                                                            oBOD = (XIDBO)oCahce.GetObjectFromCache(XIConstant.CacheBO, null, FieldOrigin.FKiBOID.ToString());
                                                            if (oBOD.iOrgObject == 1)
                                                            {
                                                                oBOD = (XIDBO)oCahce.GetObjectFromCache(XIConstant.CacheBO, null, FieldOrigin.FKiBOID.ToString(), null, null, 0, iOrgID);
                                                            }
                                                        }
                                                        oBODList.Add(FieldOrigin.FKiBOID, oBOD);
                                                    }
                                                    var GroupD = new XIDGroup();
                                                    if (oBOD.Groups.TryGetValue("label", out GroupD))
                                                    {
                                                        XIDBAPI Myconntection = new XIDBAPI();
                                                        if (oDataSource.ContainsKey(oBOD.iDataSource))
                                                        {
                                                            Myconntection = oDataSource[oBOD.iDataSource];
                                                        }
                                                        else
                                                        {
                                                            Myconntection = new XIDBAPI(oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID));
                                                            oDataSource.Add(oBOD.iDataSource, Myconntection);
                                                        }

                                                        if (oBOD != null && oBOD.Groups.Any(group => group.Key.ToLower() == "label") && (oBOD.sSize == "30" || oBOD.sSize == "20") && !string.IsNullOrEmpty(FieldInstance.sValue) && FieldInstance.sValue != "0")
                                                        {
                                                            var BODParams = new Dictionary<string, object>();
                                                            BODParams[oBOD.sPrimaryKey] = FieldInstance.sValue;
                                                            string FinalString = oBOD.GroupD("label").ConcatanateGroupFields(" ");//concatenate the string with join String 
                                                            if (!string.IsNullOrEmpty(FinalString))
                                                            {
                                                                var Result = Myconntection.Select<string>(oBOD.TableName, BODParams, FinalString + " As Result ").FirstOrDefault();
                                                                FieldInstance.sResolvedValue = Result;
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    oCResult.sMessage = "Critical Error while loading Label group data for QSInstance: " + iQSIID + " and BOID " + FieldOrigin.FKiBOID + " or BOIDXIGUID:" + FieldOrigin.FKiBOIDXIGUID;
                                                    SaveErrortoDB(oCResult);
                                                }
                                            }
                                            FieldInstance.sDisplayName = FieldOrigin.sDisplayName;
                                            if (!oQSIns.XIValues.ContainsKey(FieldOrigin.sName))
                                                oQSIns.XIValues[FieldOrigin.sName] = FieldInstance;
                                        }
                                    }
                                }
                                return oQSIns;
                            },
                            param
                            ).AsQueryable();
                    }
                    oQSInstance = lookupQSIns.Values.FirstOrDefault();
                    oQSInstance = oQSInstance == null ? new XIIQS() : oQSInstance;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "Critical ERROR While loading QSInstance: " + iQSIID;
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(st.FrameCount - 1);
                var line = frame.GetFileLineNumber();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - Line Number:" + line + " Message: " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            var iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            XIDefinitionBase oDef = new XIDefinitionBase();
            oDef.Cache_Performance("XIIXI/GetQSXIValuesByQSIID", iLapsedTime, Guid.Empty, 100);
            return oQSInstance;
        }
        public string XIGetENVSetting(string sScript, List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            var sResult = string.Empty;
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                oTrace.oTrace.Add(oCR.oTrace);
                string sGUID = Guid.NewGuid().ToString();
                string sSessionID = HttpContext.Current.Session.SessionID;
                oCache.SetXIParams(oParams, sGUID, sSessionID);
                XIDScript oXIScript = new XIDScript();
                oXIScript.sScript = "xi.s|{" + sScript + "}";
                oCResult = oXIScript.Execute_Script(sGUID, sSessionID);
                if (oCResult.bOK && oCResult.oResult != null)
                {
                    sResult = (string)oCResult.oResult;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Updating User and customer data" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return sResult;
        }
    }
}