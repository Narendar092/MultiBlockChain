using System;
using System.Web.Mvc;
using XIDNA.Models;
using System.Linq;
using XIDNA.ViewModels;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Text;
using XICore;
using System.Data;
using System.Data.SqlClient;
using XISystem;
using System.Configuration;
using System.Reflection;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using XIDatabase;
using System.Diagnostics;
using XIDNA.Repository;
using System.IO;
using System.Web;
using System.Web.UI;
using System.EnterpriseServices;
using ZeeInsurance.IDUServiceLive;

namespace XIDNA.Controllers
{
    public class DataBaseController : Controller
    {
        XIDatabaseSchema oSchema = new XIDatabaseSchema();
        ModelDbContext dbContext = new ModelDbContext();
        XIDefinitionBase oXID = new XIDefinitionBase();
        #region Source_and_Target_Compare
        //public ActionResult Index()
        //{
        //    XIDatabaseInfo res = new XIDatabaseInfo();
        //    //var Databases = oSchema.GetDatabaseConnections();
        //    //ViewBag.DataBases = Databases;
        //    res.Application = dbContext.XIApplications.ToList();
        //    res.Environment = dbContext.XIEnvironment.ToList();
        //    //res.Application = fds;
        //    return View(res);
        //}
        [HttpPost]
        public ActionResult GetDatabases(XIDatabaseInfo DBInfo)
        {
            var DataBases = DBInfo.GetDataBases();

            return Json(DataBases, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CheckConnection(XIDatabaseInfo DBInfo)
        {
            try
            {
                bool check = DBInfo.CheckConnection();
                return Json(new { bIsConnected = check, DBInfo.ConnectionString }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult SchemaCompare(string sSource, string sTarget, string[] BONames = null)
        {
            try
            {
                //sSource = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SourceDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                //sTarget = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TargetDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                ViewBag.IsFromBO = false;
                CResult result = new CResult();
                if (BONames != null)
                {
                    ViewBag.IsFromBO = true;
                    result = oSchema.NewBOStructureTablesSchemaCompare(sSource, BONames, true);
                }
                else
                    result = oSchema.NewDatabasesCompare(sSource, sTarget, BONames);
                return PartialView("_CompareDatabases", JsonSerialize(result.oResult));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult GenerateScript(string SelectedTables, string sSource, string sTarget)
        {
            try
            {
                //sSource = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SourceDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                //sTarget = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TargetDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                string sScript = string.Empty;
                if (!string.IsNullOrEmpty(SelectedTables) && !string.IsNullOrEmpty(sSource) && !string.IsNullOrEmpty(sTarget))
                {
                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    var Schema = serializer.Deserialize<List<XIDatabaseSchema>>(SelectedTables);
                    if (Schema.Count > 0)
                    {
                        sScript = "/************************** " + sTarget + " ************************/\n\n";
                        oSchema.ConnectionString = sSource;
                        sScript += oSchema.CreateAndDropTableScript(Schema.Where(m => m.iActionType == 10).Select(m => m).ToList(), 10);
                        //var ddsScript = oSchema.AlterTablesFromDatabase(Schema.Where(m => m.iActionType == 20).Select(m => m.Columns).ToList();

                        var oAlter = Schema.Where(m => m.iActionType == 20).Select((m) => new { sAlterScript = m.GenerateAlterTablesScript() }).Select(m => m.sAlterScript).ToList();
                        sScript += string.Join("", oAlter);
                        oSchema.ConnectionString = sTarget;
                        sScript += oSchema.CreateAndDropTableScript(Schema.Where(m => m.iActionType == 30).Select(m => m).ToList(), 30);
                    }

                }
                return PartialView("_SchemaPreview", sScript);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Source_and_Target_Compare

        #region BO's_and_Selft_Database_compare
        public ActionResult SelfDatabaseCompare(int iApplicationID = 0)
        {
            try
            {
                ModelDbContext DbContext = new ModelDbContext();
                if (iApplicationID > 0)
                {
                    var DDDataBases = DbContext.XIDataSources.Where(m => m.FKiApplicationID == iApplicationID).ToList().Select(s => new VMDropDown { ID = s.ID, text = s.sName }).ToList();
                    return Json(DDDataBases, JsonRequestBehavior.AllowGet);
                }
                var DDApplications = DbContext.XIApplications.Select(s => new VMDropDown { ID = s.ID, text = s.sApplicationName }).ToList();
                return View(DDApplications);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult BOSchemaCompare(string iDataSource)
        {
            try
            {
                ViewBag.IsFromBO = true;
                var result = oSchema.BOSchemaCompare(Convert.ToInt32(iDataSource));
                return PartialView("_CompareDatabases", JsonSerialize(result));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult GenerateBOScript(string SelectedTables, int InstanceID, string sBoName, string sDBType, bool bSavefile = false, string FilePath = null)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                var Schema = serializer.Deserialize<List<XIDatabaseSchema>>(SelectedTables);
                var dicTables = Schema.GroupBy(m => m.iActionType).ToDictionary(m => m.Key, m => m.ToList());
                string sScript = oSchema.GenerateBOScript(dicTables, bSavefile, FilePath);
                if (!string.IsNullOrEmpty(sScript))
                {
                    XIIXI oIXI = new XIIXI();
                    var oBOI = oIXI.BOI(sBoName, InstanceID.ToString());
                    if (oBOI != null && oBOI.Attributes.ContainsKey("sScript"))
                    {
                        //oBOI.Attributes["sScript"].sValue = sScript;
                        //oBOI.Attributes["sScript"].bDirty = true;
                        //oBOI.Save(oBOI);
                        XIEnvironmentTemplate TemplateSaving = new XIEnvironmentTemplate();
                        TemplateSaving.FKiTemplateID = InstanceID;
                        TemplateSaving.sTemplateType = sDBType;
                        TemplateSaving.sScript = sScript;

                        XIConfigs Saving = new XIConfigs();
                        Saving.sBOName = "Master Environment";
                        //Saving.sBOName = sBoName;
                        Saving.Save_EnvironmentTemplate(TemplateSaving);

                    }
                    return PartialView("_SchemaPreview", sScript);
                }
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion BO's_and_Selft_Database_compare

        #region Validate_and_Execute_Script

        [HttpPost]
        public ActionResult ValidateScript(string sScript, string sTarget = null, string iDataSource = "")
        {
            try
            {
                return Json(oSchema.ValidateScript(sScript, sTarget, Convert.ToInt32(iDataSource)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult Execute(string sScript, string sTarget = null, string iDataSource = "")
        {
            try
            {
                return Json(oSchema.Execute(sScript, sTarget, Convert.ToInt32(iDataSource)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Validate_and_Execute_Script

        #region DataBase_Data_Compare
        [HttpGet]
        public ActionResult GetBOStructureDetails(string sSource, string ID)
        {
            try
            {
                return Json(oSchema.GetBOStructureDetails(sSource, ID), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult GetBOStructures(string sSource, string ID)
        {
            try
            {
                return Json(oSchema.GetBOStructures(sSource, ID), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult GetBOTreeStructure(string sSource, string ID)
        {
            try
            {
                return PartialView("_GetBOTreeStructure", oSchema.GetTreeStructure(sSource, ID));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //[HttpPost]
        //public ActionResult DataCompareWithBONames(string[] BONames)
        //{
        //    try
        //    {
        //        return PartialView("_DataCompare", oSchema.DataCompareWithBONames(BONames));
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        [HttpPost]
        public ActionResult DataCompare(jQueryDataTableParamModels Params, string sSource, string sTarget, string CompareWith, string ParentIDs = null, string[] BONames = null, string StructureID = null, string iInstanceID = null, string sStructureName = null, string iBOID = null, string QueryType = null, string sExcludeLargeTables = null, string sTagText = null)
        {
            try
            {
                //sSource = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SourceCheckData;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                //sTarget = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TargetCheckData;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                //if (BONames != null)
                //{
                //    sSource = "Data Source=192.168.7.222;initial catalog=XIDNAQA;User Id=cruser; Password=cruser; MultipleActiveResultSets=True;";
                //    sTarget = "Data Source=192.168.7.222;initial catalog=XIDNAQAV2;User Id=cruser; Password=cruser; MultipleActiveResultSets=True;";
                //}
                var RecordCounts = oSchema.NewDataCompare(Params, sSource, sTarget, CompareWith, BONames, iInstanceID, sStructureName, iBOID, ParentIDs, QueryType, sExcludeLargeTables, sTagText);
                return Json(RecordCounts);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult TableSelection(string sSource, bool AllBos = false)
        {
            try
            {
                List<string> TableList = new List<string>();
                var RecordCounts = oSchema.NewTableSelection(sSource, AllBos);
                var jsonResult = Json(oSchema.NewTableSelection(sSource, AllBos), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult GetTableRecords(string sTableName, string sActionType, string sDBType, string iInstanceID)
        {
            try
            {
                var Result = oSchema.NewGetTableRecords(sTableName, sActionType, sDBType);
                if (!string.IsNullOrEmpty(iInstanceID))
                {
                    XIEnvironmentTemplate TemplateSaving = new XIEnvironmentTemplate();
                    TemplateSaving.FKiTemplateID = Convert.ToInt32(iInstanceID);
                    TemplateSaving.sTemplateType = sDBType;
                    TemplateSaving.sScript = Result.oResult.ToString();

                    XIConfigs Saving = new XIConfigs();
                    Saving.sBOName = "Master Environment";
                    //Saving.sBOName = sBoName;
                    Saving.Save_EnvironmentTemplate(TemplateSaving);
                }
                var JsonResult = Json(Result.oResult, JsonRequestBehavior.AllowGet);
                JsonResult.MaxJsonLength = int.MaxValue;
                return JsonResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion DataBase_Data_Compare

        #region Database_Data_Insert_Update
        public ActionResult InsertUpdateTarget(string iInstanceID, bool bSavingType = false, string FilePath = null, string QueryType = "")
        {
            try
            {
                var Result = oSchema.NewInsertUpdateTarget(bSavingType, FilePath, QueryType);
                //if (!string.IsNullOrEmpty(iInstanceID))
                //{
                //    XIEnvironmentTemplate TemplateSaving = new XIEnvironmentTemplate();
                //    TemplateSaving.FKiTemplateID = Convert.ToInt32(iInstanceID);
                //    TemplateSaving.sTemplateType = "Data";
                //    TemplateSaving.sScript = Result.oResult.ToString();

                //    XIConfigs Saving = new XIConfigs();
                //    Saving.sBOName = "Master Environment";
                //    //Saving.sBOName = sBoName;
                //    Saving.Save_EnvironmentTemplate(TemplateSaving);
                //}
                return PartialView("_SchemaPreview", Result.oResult.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult NonZeroTargetInsertUpdate(string iInstanceID, string ExceptTables, bool bSavingType = false, string FilePath = null)
        {
            try
            {
                var Result = oSchema.NonZeroInsertUpdateTarget(bSavingType, FilePath, ExceptTables);
                //if (!string.IsNullOrEmpty(iInstanceID))
                //{
                //    XIEnvironmentTemplate TemplateSaving = new XIEnvironmentTemplate();
                //    TemplateSaving.FKiTemplateID = Convert.ToInt32(iInstanceID);
                //    TemplateSaving.sTemplateType = "Data";
                //    TemplateSaving.sScript = Result.oResult.ToString();

                //    XIConfigs Saving = new XIConfigs();
                //    Saving.sBOName = "Master Environment";
                //    //Saving.sBOName = sBoName;
                //    Saving.Save_EnvironmentTemplate(TemplateSaving);
                //}
                return PartialView("_SchemaPreview", Result.oResult.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UpdateSourceTarget(string sTableName, List<string> GUIDs, string sActionType, string sDBType, string iInstanceID, bool bSavingType = false, string FilePath = null, string QueryType = null)
        {
            try
            {
                var ScriptResult = oSchema.NewUpdateSourceTarget(sTableName, GUIDs, sActionType, sDBType, bSavingType, FilePath, QueryType);
                //if (!string.IsNullOrEmpty(iInstanceID))
                //{
                //    XIEnvironmentTemplate TemplateSaving = new XIEnvironmentTemplate();
                //    TemplateSaving.FKiTemplateID = Convert.ToInt32(iInstanceID);
                //    TemplateSaving.sTemplateType = sDBType;
                //    TemplateSaving.sScript = ScriptResult.oResult.ToString();

                //    XIConfigs Saving = new XIConfigs();
                //    Saving.sBOName = "Master Environment";
                //    //Saving.sBOName = sBoName;
                //    Saving.Save_EnvironmentTemplate(TemplateSaving);
                //}
                //}
                return PartialView("_SchemaPreview", ScriptResult.oResult.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Database_Data_Insert_Update

        public string JsonSerialize(object obj)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                return serializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public CResult DBReset(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            XIInfraCache oCache = new XIInfraCache();
            XIDefinitionBase oXID = new XIDefinitionBase();
            try
            {
                string sBOName = oParams.Where(x => x.sName.ToLower() == "sBOName".ToLower()).Select(t => t.sValue).First();
                string sGroupName = oParams.Where(x => x.sName.ToLower() == "sGroupName".ToLower()).Select(t => t.sValue).First();
                XIIBO oEnvironmentI = new XIIBO();
                XIDBO oEnvironmentBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
                oEnvironmentI.BOD = oEnvironmentBOD;
                oEnvironmentI.LoadBOI(sGroupName);
                XIIBO oEnvironmentDetails = new XIIBO();
                oEnvironmentDetails.BOD = oEnvironmentBOD;
                foreach (var item in oEnvironmentI.Attributes)
                {
                    oEnvironmentDetails.SetAttribute(item.Key, oParams.Where(x => x.sName.ToLower() == item.Key.ToLower()).Select(t => t.sValue).First());
                }
                oCResult = oEnvironmentDetails.Save(oEnvironmentDetails);
                string sTargetDB = oParams.Where(x => x.sName.ToLower() == "sDestinationLocation".ToLower()).Select(t => t.sValue).FirstOrDefault();
                string sSourceDB = oParams.Where(x => x.sName.ToLower() == "sSourceLocation".ToLower()).Select(t => t.sValue).FirstOrDefault();
                XIConfigs oXIConfigs = new XIConfigs();
                oXIConfigs.sAppName = sTargetDB;
                oXIConfigs.iAppID = Convert.ToInt32(oParams.Where(x => x.sName.ToLower() == "FKiApplicationID".ToLower()).Select(t => t.sValue).FirstOrDefault());
                oXIConfigs.Save_DataSource(oParams);
                string FileName = null; string[] Tables = null;
                DataSet ds = new DataSet();
                string sDataBaseServer = ConfigurationManager.AppSettings["DataBaseServer"];
                string sDataBaseUser = ConfigurationManager.AppSettings["DataBaseUser"];
                string sDataBasePassword = ConfigurationManager.AppSettings["DataBasePassword"];
                string connectionString = "Data Source=" + sDataBaseServer + ";Initial Catalog=" + sTargetDB + ";User Id=" + sDataBaseUser + "; Password=" + sDataBasePassword + "";  //;Integrated Security=True;Persist Security Info=True
                SqlConnection con = new SqlConnection(connectionString);
                //render table name from database
                //string sqlTable = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
                //con.Open();
                //SqlDataAdapter da = new SqlDataAdapter();
                //SqlCommand cmd = new SqlCommand(sqlTable, con);
                //cmd.CommandType = CommandType.Text;
                //da.SelectCommand = cmd;
                //da.Fill(ds);
                //con.Close();
                StringBuilder sb = new StringBuilder();
                StringBuilder sbCreate = new StringBuilder();

                Server srv = new Server(new Microsoft.SqlServer.Management.Common.ServerConnection(sDataBaseServer, sDataBaseUser, sDataBasePassword));
                Database dbs = srv.Databases[sSourceDB];
                ScriptingOptions options = new ScriptingOptions();
                options.ScriptData = true;
                options.ScriptDrops = false;
                //options.FileName = FileName;
                //options.EnforceScriptingOptions = true;
                options.ScriptSchema = true;
                options.IncludeHeaders = false;
                options.AppendToFile = false;
                options.Indexes = true;
                options.WithDependencies = true;
                con.Open();

                foreach (Table myTable in dbs.Tables)
                {
                    /* Generating IF EXISTS and DROP command for tables */
                    //var tableScripts = myTable.EnumScript(options);
                    //foreach (string script in tableScripts)
                    //{
                    //    sb.Append(script);
                    //}

                    /* Generating CREATE TABLE command */
                    var CreatetableScripts = myTable.EnumScript(options);
                    sbCreate = new StringBuilder();
                    foreach (string script in CreatetableScripts)
                    {
                        sbCreate.Append(script + "\\");
                    }
                    //StringCollection stringCollection = new StringCollection();
                    //stringCollection.AddRange(sb.ToString().Split(' ').ToArray());
                    StringCollection CreatestringCollection = new StringCollection();
                    CreatestringCollection.AddRange(sbCreate.ToString().Split('\\').ToArray());
                    Server server = new Server(new ServerConnection(con));
                    //server.ConnectionContext.ExecuteNonQuery(stringCollection);
                    server.ConnectionContext.ExecuteNonQuery(CreatestringCollection);
                }
                con.Close();
                oCResult.oResult = "Success";
                oCResult.sMessage = "Copied successfully.";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.LogToFile();
                //oXID.SaveErrortoDB(oCResult, 0);
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Copying Data base process" });
                oCResult.sMessage = "ERROR:  " + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oXID.SaveErrortoDB(oCResult, 0);
            }
            return oCResult;
        }
        public ActionResult GetApplications()
        {
            var model = dbContext.XIApplications.Select(x => new { x.ID, x.sApplicationName }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        //***************** Schema Coompare Button ******************************
        [HttpPost]
        public ActionResult NewDataSchemaComapare(List<XIIAttribute> oNVParams, string sBoName, string BONames, string sTagText)
        {
            var result = new object();
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Schema Comparison for two Data bases";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "sBoName", sValue = sBoName });
                if (oNVParams != null && !string.IsNullOrEmpty(sBoName))//check mandatory params are passed or not
                {
                    string InstanceID = oNVParams.Where(x => x.sName.ToLower() == "id".ToLower()).Select(t => t.sValue).First();
                    string sApplicationID = oNVParams.Where(x => x.sName.ToLower() == "FKiApplicationID".ToLower()).Select(t => t.sValue).First();
                    string sSourceID = oNVParams.Where(x => x.sName.ToLower() == "FKiSourceEnvID".ToLower()).Select(t => t.sValue).First();
                    string sTargetID = oNVParams.Where(x => x.sName.ToLower() == "FKiTargetEnvID".ToLower()).Select(t => t.sValue).First();
                    string sTargetDB = oNVParams.Where(x => x.sName.ToLower() == "FKiTargetDB".ToLower()).Select(t => t.sValue).First();
                    string sSourceDB = oNVParams.Where(x => x.sName.ToLower() == "FKiSourceDB".ToLower()).Select(t => t.sValue).First();
                    XIInfraCache oCache = new XIInfraCache();
                    XIIXI oIXI = new XIIXI();
                    //************Getting Sorce and Target ConnectionString from "XIEnvironment DataBases" BO***********
                    var sSourceDetails = oIXI.BOI("XIEnvironment DataBases", sSourceDB);
                    var sTargetDetails = oIXI.BOI("XIEnvironment DataBases", sTargetDB);
                    var sSource = sSourceDetails.Attributes.Values.Where(n => n.sName.ToLower() == "sConnectionString".ToLower()).Select(i => i.sValue).FirstOrDefault(); //"Data Source=" + SourceDataSourceID + ";initial catalog=" + sSourceDBCon + ";User Id=" + SourceUserID + "; Password=" + SourcePassword + "; MultipleActiveResultSets=True;";
                    var sTarget = sTargetDetails.Attributes.Values.Where(n => n.sName.ToLower() == "sConnectionString".ToLower()).Select(i => i.sValue).FirstOrDefault();//"Data Source=" + TargetDataSourceID + ";initial catalog=" + sTargetDataBase + ";User Id=" + TargetUserID + "; Password=" + TargetPassword + "; MultipleActiveResultSets=True;";

                    ViewBag.IsFromBo = true;
                    ViewBag.iInstanceID = InstanceID;
                    ViewBag.sBoName = sBoName;
                    oCR = oSchema.NewDatabasesCompare(sSource, sTarget, null, false);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oTrace.oTrace.Add(oCR.oTrace);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult = oCR;
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: oNVParams = " + oNVParams + " or sBoName = " + sBoName + " is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return PartialView("_CompareDatabases", JsonSerialize(oCResult.oResult));

        }


        //*********************** Data Compare button ********************************************
        [HttpPost]
        public ActionResult NewDBDataCompare(jQueryDataTableParamModels Params, string sSourceDB, string sTargetDB, string sSourceEnvID, string sTargetEnvID, string sApplicationID, string ParentID, string iInstanceID, string StructureName, string Boid, string BONames, string QueryType, string sExcludeLargeTables, string sTagText)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Two Data Base data compare";//expalin about this method logic
                                                        // VMDTResponses RecordCounts = new VMDTResponses();
            try
            {
                oTrace.oParams.Add(new CNV { sName = "sSourceDB", sValue = sSourceDB });
                oTrace.oParams.Add(new CNV { sName = "sTargetDB", sValue = sTargetDB });
                oTrace.oParams.Add(new CNV { sName = "sSourceEnvID", sValue = sSourceEnvID });
                oTrace.oParams.Add(new CNV { sName = "sTargetEnvID", sValue = sTargetEnvID });
                oTrace.oParams.Add(new CNV { sName = "sApplicationID", sValue = sApplicationID });
                oTrace.oParams.Add(new CNV { sName = "ParentID", sValue = ParentID });
                oTrace.oParams.Add(new CNV { sName = "iInstanceID", sValue = iInstanceID });
                oTrace.oParams.Add(new CNV { sName = "StructureName", sValue = StructureName });
                oTrace.oParams.Add(new CNV { sName = "Boid", sValue = Boid });
                if (!string.IsNullOrEmpty(sSourceDB) && !string.IsNullOrEmpty(sTargetDB) && !string.IsNullOrEmpty(sSourceEnvID) && !string.IsNullOrEmpty(sTargetEnvID))//check mandatory params are passed or not
                {
                    string CompareWith = "XIGUID";
                    XIInfraCache oCache = new XIInfraCache();
                    XIIXI oIXI = new XIIXI();
                    var sSourceDetails = oIXI.BOI("XIEnvironment DataBases", sSourceDB);

                    var sTargetDetails = oIXI.BOI("XIEnvironment DataBases", sTargetDB);


                    var sSource = sSourceDetails.Attributes.Values.Where(n => n.sName.ToLower() == "sConnectionString".ToLower()).Select(i => i.sValue).FirstOrDefault(); //"Data Source=" + SourceDataSourceID + ";initial catalog=" + sSourceDBCon + ";User Id=" + SourceUserID + "; Password=" + SourcePassword + "; MultipleActiveResultSets=True;";
                    var sTarget = sTargetDetails.Attributes.Values.Where(n => n.sName.ToLower() == "sConnectionString".ToLower()).Select(i => i.sValue).FirstOrDefault();//"Data Source=" + TargetDataSourceID + ";initial catalog=" + sTargetDataBase + ";User Id=" + TargetUserID + "; Password=" + TargetPassword + "; MultipleActiveResultSets=True;";

                    if (Params.iDisplayLength == 0)
                        Params.iDisplayLength = 10;
                    string[] sBoNames = null;
                    if (!string.IsNullOrEmpty(BONames))
                        sBoNames = BONames.Split(',');
                    if (StructureName == "select" && !string.IsNullOrEmpty(StructureName))
                    {
                        iInstanceID = "0";
                    }
                    oCResult = oSchema.NewDataCompare(Params, sSource, sTarget, CompareWith, sBoNames, iInstanceID, StructureName, Boid, ParentID, QueryType, sExcludeLargeTables, sTagText);
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sSourceDB = " + sSourceDB + " or sTargetDB = " + sTargetDB + " or sSourceEnvID = " + sSourceEnvID + " or sTargetEnvID = " + sTargetEnvID + " or sApplicationID = " + sApplicationID + " or ParentID = " + ParentID + " or iInstanceID = " + iInstanceID + " or StructureName = " + StructureName + " or Boid = " + Boid + " is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return Json(oCResult.oResult);
        }

        [HttpPost]
        public ActionResult DBNonZeroDataCompare(jQueryDataTableParamModels Params, string sSourceDB, string sTargetDB, string sSourceEnvID, string sTargetEnvID, string sApplicationID, string ParentID, string iInstanceID, string StructureName, string Boid, string BONames)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Two Data Base data compare";//expalin about this method logic
                                                        // VMDTResponses RecordCounts = new VMDTResponses();
            try
            {
                oTrace.oParams.Add(new CNV { sName = "sSourceDB", sValue = sSourceDB });
                oTrace.oParams.Add(new CNV { sName = "sTargetDB", sValue = sTargetDB });
                oTrace.oParams.Add(new CNV { sName = "sSourceEnvID", sValue = sSourceEnvID });
                oTrace.oParams.Add(new CNV { sName = "sTargetEnvID", sValue = sTargetEnvID });
                oTrace.oParams.Add(new CNV { sName = "sApplicationID", sValue = sApplicationID });
                oTrace.oParams.Add(new CNV { sName = "ParentID", sValue = ParentID });
                oTrace.oParams.Add(new CNV { sName = "iInstanceID", sValue = iInstanceID });
                oTrace.oParams.Add(new CNV { sName = "StructureName", sValue = StructureName });
                oTrace.oParams.Add(new CNV { sName = "Boid", sValue = Boid });
                if (!string.IsNullOrEmpty(sSourceDB) && !string.IsNullOrEmpty(sTargetDB) && !string.IsNullOrEmpty(sSourceEnvID) && !string.IsNullOrEmpty(sTargetEnvID))//check mandatory params are passed or not
                {
                    XIInfraCache oCache = new XIInfraCache();
                    XIIXI oIXI = new XIIXI();
                    var sSourceDetails = oIXI.BOI("XIEnvironment DataBases", sSourceDB);

                    var sTargetDetails = oIXI.BOI("XIEnvironment DataBases", sTargetDB);


                    var sSource = sSourceDetails.Attributes.Values.Where(n => n.sName.ToLower() == "sConnectionString".ToLower()).Select(i => i.sValue).FirstOrDefault(); //"Data Source=" + SourceDataSourceID + ";initial catalog=" + sSourceDBCon + ";User Id=" + SourceUserID + "; Password=" + SourcePassword + "; MultipleActiveResultSets=True;";
                    var sTarget = sTargetDetails.Attributes.Values.Where(n => n.sName.ToLower() == "sConnectionString".ToLower()).Select(i => i.sValue).FirstOrDefault();//"Data Source=" + TargetDataSourceID + ";initial catalog=" + sTargetDataBase + ";User Id=" + TargetUserID + "; Password=" + TargetPassword + "; MultipleActiveResultSets=True;";

                    if (Params.iDisplayLength == 0)
                        Params.iDisplayLength = 10;
                    string[] sBoNames = null;
                    if (!string.IsNullOrEmpty(BONames))
                        sBoNames = BONames.Split(',');
                    if (StructureName == "select" && !string.IsNullOrEmpty(StructureName))
                    {
                        iInstanceID = "0";
                    }
                    oCResult = oSchema.DBNonZeroDataCompare(Params, sSource, sTarget, sBoNames, Convert.ToInt32(iInstanceID), StructureName, Convert.ToInt32(Boid), ParentID);
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sSourceDB = " + sSourceDB + " or sTargetDB = " + sTargetDB + " or sSourceEnvID = " + sSourceEnvID + " or sTargetEnvID = " + sTargetEnvID + " or sApplicationID = " + sApplicationID + " or ParentID = " + ParentID + " or iInstanceID = " + iInstanceID + " or StructureName = " + StructureName + " or Boid = " + Boid + " is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return Json(oCResult.oResult);
        }

        //******** This method is used for loadded BOs and BO's againest Structure**********
        [HttpPost]
        public ActionResult NewDBTableSelection(List<XIIAttribute> oNVParams, bool AllBos = false)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Structure Data Comparison for selected Tables";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "AllBos", sValue = AllBos.ToString() });
                if (oNVParams != null)//check mandatory params are passed or not
                {

                    XIIXI oIXI = new XIIXI();
                    string sApplicationID = oNVParams.Where(x => x.sName.ToLower() == "FKiApplicationID".ToLower()).Select(t => t.sValue).First();
                    string sSourceID = oNVParams.Where(x => x.sName.ToLower() == "FKiSourceEnvID".ToLower()).Select(t => t.sValue).First();
                    string sSourceDB = oNVParams.Where(x => x.sName.ToLower() == "FKiSourceDB".ToLower()).Select(t => t.sValue).First();

                    var sSourceDetails = oIXI.BOI("XIEnvironment DataBases", sSourceDB);
                    var sSource = sSourceDetails.Attributes.Values.Where(n => n.sName.ToLower() == "sConnectionString".ToLower()).Select(i => i.sValue).FirstOrDefault(); //"Data Source=" + SourceDataSourceID + ";initial catalog=" + sSourceDBCon + ";User Id=" + SourceUserID + "; Password=" + SourcePassword + "; MultipleActiveResultSets=True;";
                    oCR = oSchema.NewTableSelection(sSource, AllBos);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oTrace.oTrace.Add(oCR.oTrace);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult = oCR;
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: AllBos = " + AllBos + " is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            var jsonResult = Json(oCResult.oResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;


        }


        [HttpPost]
        //******** loading Tree Structure *********
        public ActionResult DBGetBOTreeStructure(string sSource, string ID)
        {
            try
            {
                XIIXI oIXI = new XIIXI();
                var sSourceDetails = oIXI.BOI("XIEnvironment DataBases", sSource);
                var sSourceCon = sSourceDetails.Attributes.Values.Where(n => n.sName.ToLower() == "sConnectionString".ToLower()).Select(i => i.sValue).FirstOrDefault(); //"Data Source=" + SourceDataSourceID + ";initial catalog=" + sSourceDBCon + ";User Id=" + SourceUserID + "; Password=" + SourcePassword + "; MultipleActiveResultSets=True;";
                var StructureResult = oSchema.NewGetTreeStructure(sSourceCon, ID);
                return PartialView("_GetBOTreeStructure", (List<XIDStructure>)StructureResult.oResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public string DBCreation(List<XIIAttribute> oNVParam)
        {
            string Result = string.Empty;
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "DB Creating";//expalin about this method logic
            string sServerName = oNVParam.Where(x => x.sName.ToLower() == "03ba1548-c4c9-46d8-ba6a-0cb51e04d190".ToLower()).Select(t => t.sValue).First();
            string sUsername = oNVParam.Where(x => x.sName.ToLower() == "03ba1548-c4c9-46d8-ba6a-0cb51e04d898".ToLower()).Select(t => t.sValue).First();
            string sPassword = oNVParam.Where(x => x.sName.ToLower() == "03ba1548-c4c9-46d8-ba6a-0cb51e04d890".ToLower()).Select(t => t.sValue).First();
            string NewDBName = oNVParam.Where(x => x.sName.ToLower() == "03ba1548-c4c9-46d8-ba6a-0cb51e04d801".ToLower()).Select(t => t.sValue).First();
            String str = "CREATE DATABASE " + NewDBName;
            if (!string.IsNullOrEmpty(sServerName) && !string.IsNullOrEmpty(sUsername) && !string.IsNullOrEmpty(sPassword) && !string.IsNullOrEmpty(NewDBName))//check mandatory params are passed or not
            {
                var sConnectionString = "Data Source = " + sServerName + "; initial catalog = master; User Id = " + sUsername + "; Password = " + sPassword + "; MultipleActiveResultSets = True";

                SqlConnection myConn = new SqlConnection(sConnectionString);
                SqlCommand myCommand = new SqlCommand(str, myConn);
                try
                {
                    myConn.Open();
                    var sqlCreateDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", NewDBName);
                    SqlCommand myCommand1 = new SqlCommand(sqlCreateDBQuery, myConn);
                    var resultObj = myCommand1.ExecuteScalar();
                    int databaseID = 0;

                    if (resultObj != null)
                    {
                        int.TryParse(resultObj.ToString(), out databaseID);
                    }
                    if (databaseID == 0)
                    {
                        myCommand.ExecuteNonQuery();
                        var oCResult1 = DBChecking(oNVParam, false, NewDBName, sConnectionString);
                        Result = NewDBName + " DB Created Sucessfully" + oCResult1.oResult;

                    }
                    else
                    {
                        var oCResult1 = DBChecking(oNVParam, false, NewDBName, sConnectionString);
                        if (!string.IsNullOrEmpty((string)oCResult1.oResult))
                            Result = NewDBName + " Existing DB But " + oCResult1.oResult;
                        else
                            Result = NewDBName + " Existing DB";
                    }
                }
                catch (Exception ex)
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                    oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.sCategory = ex.GetType().ToString();
                    oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                    oXID.SaveErrortoDB(oCResult);
                }
                finally
                {
                    if (myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                }
            }
            return Result;
        }

        public CResult DBChecking(List<XIIAttribute> oNVParam, bool result, string NewDBName, string sConnectionString)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "DB Creating";//expalin about this method logic
            var fileName = string.Empty;
            try
            {
                sConnectionString = sConnectionString.Replace("master", NewDBName);
                string sSchemaPath = oNVParam.Where(x => x.sName.ToLower() == "03ba1548-c4c9-46d8-ba6a-0cb51e04d806".ToLower()).Select(t => t.sValue).First();
                string sDataPath = oNVParam.Where(x => x.sName.ToLower() == "03ba1548-c4c9-46d8-ba6a-0cb51e04d896".ToLower()).Select(t => t.sValue).First();
                if (result == false)
                {
                    if (!string.IsNullOrEmpty(sSchemaPath))
                    {
                        //string dir = Directory.GetDirectories(@sSchemaPath).FirstOrDefault();
                        string[] filePaths = Directory.GetFiles(sSchemaPath, "*.sql", SearchOption.AllDirectories);

                        if (filePaths.Count() > 0)
                        {
                            SqlConnection sConnetion = new SqlConnection(sConnectionString);
                            sConnetion.Open();
                            // Copy the files and overwrite destination files if they already exist.
                            foreach (string s in filePaths)
                            {
                                fileName = System.IO.Path.GetFileName(s);
                                string text = System.IO.File.ReadAllText(s);
                                //SqlConnection Connetionstring = new SqlConnection("Data Source=INATIVE-PC137\\MSSQLEXPRESS;initial catalog=" + NewDBName + ";User Id=XIDNADMIN; Password=XIDNADMIN;"); ///*INATIVE-PC137"+@"\MSSQLEXPRESS*/
                                string createtableCommandString = text;
                                SqlCommand command = new SqlCommand(createtableCommandString, sConnetion);
                                command.ExecuteNonQuery();
                            }
                            sConnetion.Close();
                        }
                    }
                    if (!string.IsNullOrEmpty(sDataPath))
                    {
                        string[] subdirectoryEntries = Directory.GetDirectories(sDataPath);
                        string[] filePaths;
                        foreach (var subdirectory in subdirectoryEntries)
                        {

                            filePaths = Directory.GetFiles(subdirectory, "*.sql", SearchOption.AllDirectories);
                            string sTableName = subdirectory.Split('\\')[subdirectory.Split('\\').Length - 1];
                            if (filePaths.Count() > 0)
                            {
                                // Copy the files and overwrite destination files if they already exist.
                                StringBuilder text = new StringBuilder();
                                //SqlConnection Connetionstring = new SqlConnection("Data Source=INATIVE-PC137\\MSSQLEXPRESS;initial catalog="+NewDBName+";User Id=XIDNADMIN; Password=XIDNADMIN;"); 
                                SqlConnection oConnection = new SqlConnection(sConnectionString);
                                SqlCommand command = new SqlCommand();
                                oConnection.Open();
                                //command = new SqlCommand("TRUNCATE TABLE " + sTableName + "\n SET IDENTITY_INSERT[dbo].[" + sTableName + "] ON ", oConnection); try   //To do
                                //{
                                //    command.ExecuteNonQuery();
                                //}
                                //catch (Exception ex) { }

                                // Copy the files and overwrite destination files if they already exist.
                                int maxRows = 5000;
                                int Count = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(filePaths.Count()) / maxRows));
                                int startIndex = 0; int RecordCount = 0; int FileCount = maxRows;
                                for (startIndex = 1; startIndex <= Count; startIndex++)
                                {
                                    if (startIndex == Count)
                                    {
                                        FileCount = filePaths.Count() - RecordCount;
                                    }
                                    for (int i = 0; i < FileCount; i++)
                                    {
                                        fileName = System.IO.Path.GetFileName(filePaths[RecordCount + i]);
                                        text.Append(System.IO.File.ReadAllText(filePaths[RecordCount + i]) + "\n");
                                    }
                                    command = new SqlCommand(text.ToString(), oConnection);
                                    command.ExecuteNonQuery();
                                    RecordCount = startIndex * maxRows;
                                    text = new StringBuilder();
                                }
                                //command = new SqlCommand("SET IDENTITY_INSERT[dbo].[" + sTableName + "] OFF ", oConnection);
                                //try
                                //{          //To do
                                //    command.ExecuteNonQuery();
                                //}
                                //catch (Exception ex) { }
                                oConnection.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.oResult = "Alredy Existing " + fileName;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public string TestConnection(List<XIIAttribute> oNVParam)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "DB Connection Checking";//expalin about this method logic

            string sStatus = "Success";

            try
            {
                string sServerName = oNVParam.Where(x => x.sName.ToLower() == "sServerName".ToLower()).Select(t => t.sValue).First();
                string sUsername = oNVParam.Where(x => x.sName.ToLower() == "sUserName".ToLower()).Select(t => t.sValue).First();
                string sPassword = oNVParam.Where(x => x.sName.ToLower() == "sPassword".ToLower()).Select(t => t.sValue).First();
                string DBName = oNVParam.Where(x => x.sName.ToLower() == "sDbName".ToLower()).Select(t => t.sValue).First();

                if (!string.IsNullOrEmpty(sServerName) && !string.IsNullOrEmpty(sUsername) && !string.IsNullOrEmpty(sPassword) && !string.IsNullOrEmpty(DBName))//check mandatory params are passed or not
                {
                    var sConnectionString = "Data Source = " + sServerName + "; initial catalog = master; User Id = " + sUsername + "; Password = " + sPassword + "; MultipleActiveResultSets = True";
                    using (SqlConnection conn = new SqlConnection(sConnectionString))
                    {
                        conn.Open(); // throws if invalid
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                sStatus = "Failure";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }

            return sStatus;
        }
        public string DataGeneration(string sSourceDB, string sTargetDB, string sSourceEnvID, string sTargetEnvID, string sBOName, bool bSavingType, string SelectedTables, string FilePath)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            var Schema = serializer.Deserialize<List<XIDatabaseSchema>>(SelectedTables);
            var dicTables = Schema.GroupBy(m => m.iActionType).ToDictionary(m => m.Key, m => m.ToList());
            oSchema.GenerateBOScript(dicTables, bSavingType, FilePath, "full");

            XIIXI oIXI = new XIIXI();
            var sSourceDetails = oIXI.BOI("XIEnvironment DataBases", sSourceDB);
            var sTargetDetails = oIXI.BOI("XIEnvironment DataBases", sTargetDB);

            var sSource = sSourceDetails.Attributes.Values.Where(n => n.sName.ToLower() == "sConnectionString".ToLower()).Select(i => i.sValue).FirstOrDefault(); //"Data Source=" + SourceDataSourceID + ";initial catalog=" + sSourceDBCon + ";User Id=" + SourceUserID + "; Password=" + SourcePassword + "; MultipleActiveResultSets=True;";
            var sTarget = sTargetDetails.Attributes.Values.Where(n => n.sName.ToLower() == "sConnectionString".ToLower()).Select(i => i.sValue).FirstOrDefault();//"Data Source=" + TargetDataSourceID + ";initial catalog=" + sTargetDataBase + ";User Id=" + TargetUserID + "; Password=" + TargetPassword + "; MultipleActiveResultSets=True;";

            var TableList = Schema.Select(s => s.sTableName).ToList();
            oSchema.FullSchemawithData(sSource, sTarget, TableList);
            oSchema.NonZeroInsertUpdateTarget(bSavingType, FilePath, string.Join(",", TableList.ToList().Select(k => k)));
            var Alert = "Success";
            return Alert;
        }

        public ActionResult DBHelper()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Execute_Query(string sUser, string sTable, string sQuery)
        {
            XIDefinitionBase oDef = new XIDefinitionBase();
            CResult oCR = new CResult();
            Dictionary<string, object> QueryResult = new Dictionary<string, object>();
            try
            {
                if (!string.IsNullOrEmpty(sQuery) && (sQuery.Contains("delete") || sQuery.Contains("drop")))
                {
                    oCR.sMessage = "Critical Error: User:" + sUser + " executed the query:" + sQuery + " from query executor which contains delete or drop command in query";
                    oCR.sCategory = "Manual DB query operation";
                    oDef.SaveErrortoDB(oCR);
                    QueryResult["Message"] = "Failure: Query contains delete or drop statements";
                    return Json(QueryResult, JsonRequestBehavior.AllowGet);
                }
                if (!string.IsNullOrEmpty(sUser) && !string.IsNullOrEmpty(sQuery) && !string.IsNullOrEmpty(sTable))
                {
                    oCR.sMessage = "User:" + sUser + " executed the query:" + sQuery + " from query executor";
                    oCR.sCategory = "Manual DB query operation";
                    oDef.SaveErrortoDB(oCR);
                    XIInfraCache oCache = new XIInfraCache();
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sTable);
                    if (oBOD != null && !string.IsNullOrEmpty(oBOD.Name))
                    {
                        XIIBO oBOI = new XIIBO();
                        XIDXI oXID = new XIDXI();
                        string sBODataSource = oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), 0);
                        XIDBAPI Connection = new XIDBAPI(sBODataSource);
                        sQuery = sQuery.Trim();
                        if (sQuery.ToLower().StartsWith("select "))
                        {
                            if (sQuery.Contains(" top "))
                            {
                                XID1Click oXI1Click = new XID1Click();
                                oXI1Click.Query = sQuery;
                                oXI1Click.Name = sTable;
                                //oXI1Click.DataSource = 10;
                                var Result = oXI1Click.GetList();
                                List<XIIBO> oDocumentsList = new List<XIIBO>();
                                if (Result.bOK && Result.oResult != null)
                                {
                                    oDocumentsList = ((Dictionary<string, XIIBO>)Result.oResult).Values.ToList();
                                    QueryResult["Data"] = oDocumentsList;
                                    return Json(QueryResult, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                QueryResult["Message"] = "Failure: Select query does not contain TOP calause";
                                return Json(QueryResult, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (sQuery.ToLower().StartsWith("alter "))
                        {
                            Connection.ExecuteQuery(sQuery);
                            QueryResult["Message"] = "Success";
                            return Json(QueryResult, JsonRequestBehavior.AllowGet);
                        }
                        else if (sQuery.ToLower().StartsWith("update "))
                        {
                            Connection.ExecuteQuery(sQuery);
                            QueryResult["Message"] = "Success";
                            return Json(QueryResult, JsonRequestBehavior.AllowGet);
                        }
                        else if (sQuery.ToLower().StartsWith("drop "))
                        {

                        }
                        else if (sQuery.ToLower().StartsWith("delete "))
                        {

                        }
                    }
                    else
                    {
                        QueryResult["Message"] = "Failure: BO not found for table:" + sTable;
                        return Json(QueryResult, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "Failure: User:" + sUser + " executed the query:" + sQuery + " from query executor. Failure Message:" + ex.ToString();
                oCR.sCategory = "Manual DB query operation";
                oDef.SaveErrortoDB(oCR);
                QueryResult["Message"] = "Failure:" + ex.Message;
                return Json(QueryResult, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        #region Tagging

        [HttpPost]
        public ActionResult Run_Tagging(string sBO, string sUID, string sStrCode, string sTag, string sType, string s1Click)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Tag the instances";//expalin about this method logic
            XIDefinitionBase oDef = new XIDefinitionBase();
            try
            {
                //sBO = "XIQSDefinition";
                //sUID = "15615";
                //sStrCode = "Tag";
                //sTag = "#Core";
                oTrace.oParams.Add(new CNV { sName = "sUID", sValue = sUID });
                oTrace.oParams.Add(new CNV { sName = "sStrCode", sValue = sStrCode });
                oTrace.oParams.Add(new CNV { sName = "sTag", sValue = sTag });
                oTrace.oParams.Add(new CNV { sName = "sType", sValue = sType });
                XIInfraCache oCache = new XIInfraCache();
                if (!string.IsNullOrEmpty(sBO) && !string.IsNullOrEmpty(sUID))//check mandatory params are passed or not
                {
                    XIIXI oXII = new XIIXI();
                    if (string.IsNullOrEmpty(sTag))
                    {
                        var oBOI = oXII.BOI(sBO, sUID);
                        sTag = oBOI.AttributeI("sTag").sValue;
                    }
                    sStrCode = sStrCode == null ? "Tag" : sStrCode;
                    sType = sType == null ? "Tag" : sType;
                    if (!string.IsNullOrEmpty(sTag))
                    {
                        XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBO);
                        List<XIDStructure> oStrD = (List<XIDStructure>)oCache.GetObjectFromCache(XIConstant.CacheStructure, sStrCode, oBOD.XIGUID.ToString());

                        var oStructobj = oXII.BOI(sBO, sUID).Structure(sStrCode).XILoad();
                        oCR = new CResult();
                        oCR = Recurrsive_Tagging(oStructobj.BOI, sTag, sType);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            if (oStrD != null && oStrD.Count() > 0)
                            {
                                //var ChildStrD = oStrD.Where(m => !string.IsNullOrEmpty(m.sChildStructure)).ToList();
                                //if (ChildStrD != null && ChildStrD.Count() > 0)
                                //{
                                //    foreach (var items in ChildStrD)
                                //    {
                                //        if (!string.IsNullOrEmpty(items.sChildBOAttr) && !string.IsNullOrEmpty(items.sChildBO))
                                //        {
                                //            var oChildIns = oStructobj.oSubStructureI(items.sBO).oBOIList.FirstOrDefault();
                                //            var ChildIns = oChildIns.AttributeI(items.sChildBOAttr).sValue;
                                //            oCR = Tag_ChildStructures(items.sChildBO, ChildIns, items.sChildStructure, sTag, sType);
                                //        }
                                //    }
                                //}
                            }
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = "Success";
                        }
                        else
                        {
                            var sTagErrors = string.Empty;
                            if (TagErrors != null && TagErrors.Count() > 0)
                            {
                                sTagErrors = string.Join("->", TagErrors);
                            }
                            oCResult.sMessage = "ERROR: Tagging failue:" + TagErrors;
                            oCResult.sCategory = "Tagging";
                            oDef.SaveErrortoDB(oCResult);
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.oResult = sTagErrors;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(s1Click))
                {
                    XID1Click o1ClickD = new XID1Click();
                    XID1Click o1ClickC = new XID1Click();
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, s1Click);
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOIDXIGUID.ToString());
                    o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    var oResult = o1ClickC.OneClick_Run();
                    if (oResult != null && oResult.Count() > 0)
                    {
                        XIIXI oXII = new XIIXI();
                        foreach (var items in oResult.Values.ToList())
                        {
                            var Guid = items.AttributeI("xiguid").sValue;
                            var oStructobj = oXII.BOI(oBOD.Name, Guid).Structure(sStrCode).XILoad();
                            oCR = Recurrsive_Tagging(oStructobj.BOI, sTag, sType);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(sTag))
                    {
                        var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, "3068C005-A38C-4FB4-A7F7-A628EE6D3893");
                        XID1Click o1Click = new XID1Click();
                        o1Click.BOIDXIGUID = new Guid("3068C005-A38C-4FB4-A7F7-A628EE6D3893");
                        //o1Click.Query = "select * from XIBO_T_N where sTag like '%.Onecorestudio%' and xiguid not in('96ED7B97-0756-4773-A75D-12B8C0F76F9A','97F666D0-357E-4B48-87A5-C4A560048CC5') order by name";
                        o1Click.Query = "select * from XIBOStructure_T where sCode = 'IDEStructure'";
                        var oResult = o1Click.OneClick_Run();
                        if (oResult != null && oResult.Count() > 0)
                        {
                            XIIXI oXII = new XIIXI();
                            foreach (var items in oResult.Values.ToList())
                            {
                                var Guid = items.AttributeI("xiguid").sValue;
                                var oStructobj = oXII.BOI(oBOD.Name, Guid).Structure(sStrCode).XILoad();
                                oCR = Recurrsive_Tagging(oStructobj.BOI, sTag, sType);
                            }
                        }
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sBO or sUID or sStrCode or sTag missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = "Tagging";
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            TagErrors = new List<string>();
            return Json(oCResult.oResult, JsonRequestBehavior.AllowGet);
        }
        List<string> TagErrors = new List<string>();
        XIConfigs oConfig = new XIConfigs();
        public CResult Recurrsive_Tagging(XIIBO oBOI, string sTag, string sType)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "tag all the child instances with sTag recurrsively";//expalin about this method logic
            XIDefinitionBase oDef = new XIDefinitionBase();
            string sBO = string.Empty;
            string sUID = string.Empty;
            try
            {
                oTrace.oParams.Add(new CNV { sName = "sTag", sValue = sTag });
                oTrace.oParams.Add(new CNV { sName = "sType", sValue = sType });
                if (oBOI != null && oBOI.Attributes.Count() > 0 && !string.IsNullOrEmpty(sTag) && !string.IsNullOrEmpty(sType))//check mandatory params are passed or not
                {
                    bool bTagFound = false;
                    if (oBOI != null && oBOI.Attributes.Count() > 0)
                    {
                        if (oBOI.BOD.Name == "XI Link")
                        {

                        }
                        if (sType.ToLower() == "tag")
                        {
                            var sExistingTag = oBOI.AttributeI("sTag").sValue;
                            var FKiTagSpaceIDXIGUID = oBOI.AttributeI("FKiTagSpaceIDXIGUID").sValue;
                            var FKiVersionIDXIGUID = oBOI.AttributeI("FKiVersionIDXIGUID").sValue;
                            if (!string.IsNullOrEmpty(sExistingTag))
                            {
                                var AllTags = sExistingTag.Split(' ').ToList();
                                if (AllTags.Contains(sTag))
                                {
                                    //instance already contains the tag
                                    bTagFound = true;
                                }
                            }
                            if (!bTagFound)
                            {
                                var oTagAttrD = oBOI.BOD.AttributeD("sTag");
                                if (oTagAttrD != null)
                                {
                                    sBO = oBOI.BOD.Name;
                                    sUID = oBOI.AttributeI("xiguid").sValue;
                                    if (!string.IsNullOrEmpty(sExistingTag))
                                    {
                                        oBOI.SetAttribute("sTag", sExistingTag + " " + sTag);
                                    }
                                    else
                                        oBOI.SetAttribute("sTag", sTag);
                                    oBOI.SetAttribute("FKiTagSpaceIDXIGUID", FKiTagSpaceIDXIGUID);
                                    oBOI.SetAttribute("FKiVersionIDXIGUID", FKiVersionIDXIGUID);
                                    oCR = oBOI.Save(oBOI, false);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {

                                    }
                                    else
                                    {
                                        TagErrors.Add("Tag: Tagging failed for BO:" + sBO + " and instance:" + sUID);
                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                                else
                                {
                                    //XIDAttribute oAttrD = new XIDAttribute();
                                    //oAttrD.Name = "sTag";
                                    //oAttrD.LabelName = "Tag";
                                    //oAttrD.Description = "Tag";
                                    //oAttrD.TypeID = 60;
                                    //oAttrD.IsVisible = true;
                                    //oAttrD.IsWhere = true;
                                    //oAttrD.StatusTypeID = 10;
                                    //oCR = oConfig.Save_BOAttribute(oAttrD);
                                    TagErrors.Add("Tag: Tagging failed for BO:" + sBO + " as sTag attribute is not available");
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                        }
                        else if (sType.ToLower() == "tagspace")
                        {
                            var oTagAttrD = oBOI.BOD.AttributeD("FKiTagSpaceID");
                            if (oTagAttrD != null)
                            {
                                sBO = oBOI.BOD.Name;
                                sUID = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue;
                                oBOI.SetAttribute("FKiTagSpaceID", sTag);
                                oCR = oBOI.Save(oBOI, false);
                                if (oCR.bOK && oCR.oResult != null)
                                {

                                }
                                else
                                {
                                    TagErrors.Add("Tagspace: Tagging failed for BO:" + sBO + " and instance:" + sUID);
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                        }
                        //set tag to child instances
                        if (oBOI.SubChildI != null && oBOI.SubChildI.Values.Count() > 0)
                        {
                            foreach (var subBOI in oBOI.SubChildI.Values.ToList())
                            {
                                foreach (var oboi in subBOI)
                                {
                                    oCR = Recurrsive_Tagging(oboi, sTag, sType);
                                }
                            }
                        }
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = "Success";
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sTag is missing";
                }
                if (TagErrors != null && TagErrors.Count > 0)
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                TagErrors.Add("Tagging failed for BO:" + sBO + " and instance:" + sUID);
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: Tagging: failure while tagging for BO:" + sBO + " and instance:" + sUID + " in [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = "Tagging";
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        [HttpPost]
        public ActionResult Get_TagStructureCodes(string sBO, string sUID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            List<string> TagStrCodes = new List<string>();
            XIDefinitionBase oDef = new XIDefinitionBase();
            try
            {
                oTrace.oParams.Add(new CNV { sName = "sBO", sValue = sBO });
                oTrace.oParams.Add(new CNV { sName = "sUID", sValue = sUID });
                if (!string.IsNullOrEmpty(sBO) && !string.IsNullOrEmpty(sUID))//check mandatory params are passed or not
                {
                    XIInfraCache oCache = new XIInfraCache();
                    var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBO);
                    if (BOD != null && BOD.BOID > 0)
                    {
                        if (BOD.Structures != null && BOD.Structures.Count > 0)
                        {
                            TagStrCodes = BOD.Structures.Values.ToList().Where(m => m.sCode.ToLower().StartsWith("tag") && m.FKiParentID == "#").Select(m => m.sCode).ToList();
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sBO or sUID missing";
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
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return Json(TagStrCodes, JsonRequestBehavior.AllowGet);
        }

        public CResult Tag_ChildStructures(string sBO, string sUID, string sStrCode, string sTag, string sType)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            List<string> TagStrCodes = new List<string>();
            XIDefinitionBase oDef = new XIDefinitionBase();
            try
            {
                oTrace.oParams.Add(new CNV { sName = "sBO", sValue = sBO });
                oTrace.oParams.Add(new CNV { sName = "sUID", sValue = sUID });
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oXII = new XIIXI();
                var oStructobj = oXII.BOI(sBO, sUID).Structure(sStrCode).XILoad();
                oCR = Recurrsive_Tagging(oStructobj.BOI, sTag, sType);
                List<XIDStructure> oStrD = (List<XIDStructure>)oCache.GetObjectFromCache(XIConstant.CacheStructure, sStrCode);
                if (oStrD != null && oStrD.Count() > 0)
                {
                    //var ChildStrD = oStrD.Where(m => !string.IsNullOrEmpty(m.sChildStructure)).ToList();
                    //if (ChildStrD != null && ChildStrD.Count() > 0)
                    //{
                    //    foreach (var items in ChildStrD)
                    //    {
                    //        if (!string.IsNullOrEmpty(items.sChildBOAttr) && !string.IsNullOrEmpty(items.sChildBO))
                    //        {
                    //            var oChildIns = oStructobj.oSubStructureI(items.sChildBO).oBOIList.FirstOrDefault();
                    //            var ChildIns = oChildIns.AttributeI(items.sChildBOAttr).sValue;
                    //            if (!string.IsNullOrEmpty(items.sChildStructure) && !string.IsNullOrEmpty(items.sChildBOAttr) && !string.IsNullOrEmpty(ChildIns))
                    //            {
                    //                Tag_ChildStructures(items.sChildBO, ChildIns, items.sChildStructure, sTag, sType);
                    //            }
                    //        }
                    //    }
                    //}
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sBO or sUID missing";
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
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        #endregion Tagging
    }
}