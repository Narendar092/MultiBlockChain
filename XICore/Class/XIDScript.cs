using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using XIAlgorithm;
using XIDatabase;
using XISystem;
using static XIDatabase.XIDBAPI;

namespace XICore
{
    public class XIDScript
    {
        public long ID { get; set; }
        public Guid XIGUID { get; set; }
        public int FKiBOID { get; set; }
        public int FKiBOAttributeID { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        [AllowHtml]
        public string sScript { get; set; }
        public string sType { get; set; }
        public string sLevel { get; set; }
        public string sCategory { get; set; }
        public string sClassification { get; set; }
        public string sLanguage { get; set; }
        public string sExecutionType { get; set; }
        [DapperIgnore]
        public bool IsSuccess { get; set; }
        [DapperIgnore]
        public string sFieldName { get; set; }
        public int iOrder { get; set; }
        //Mycode
        public string sVersion { get; set; }
        [NotMapped]
        public List<XIDropDown> ddlStatusTypes { get; set; }
        [NotMapped]
        public List<XIDropDown> ddlScriptTypes { get; set; }
        [NotMapped]
        public List<XIDropDown> ddlScriptClassification { get; set; }
        [NotMapped]
        public List<XIDropDown> ddlScriptLevels { get; set; }
        [NotMapped]
        public List<XIDropDown> ddlScriptCategory { get; set; }
        [NotMapped]
        public List<XIDropDown> ddlLanguages { get; set; }
        public List<XIDropDown> ddlAllBOs { get; set; }
        public string sMethodName { get; set; }
        public int StatusTypeID { get; set; }
        public int UpdatedBy { get; set; }
        public int CreatedBy { get; set; }
        [DapperIgnore]
        public string sBOName { get; set; }
        [DapperIgnore]
        public string sAttrName { get; set; }

        public CResult oCResult = new CResult();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

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

        public CResult Execute_Script(string sGUID, string sSessionID)
        {
            CScriptController oXIScript = new CScriptController();
            XIInfraCache oCache = new XIInfraCache();
            CResult oCR = new CResult();
            var oCacheObj = oCache.GetFromCache(sScript);
            if (oCacheObj == null)
            {
                oCR = oXIScript.API2_Serialise_From_String(sScript);
                oCache.InsertIntoCache(oXIScript, sScript);
            }
            else
            {
                oXIScript = (CScriptController)oCacheObj;
            }

            //CResult oCR = oXIScript.API2_Serialise_From_String(sScript);
            oCR = oXIScript.API2_ExecuteMyOM("", null, null, sGUID, sSessionID);
            //xiElement oElement = new xiElement();
            //oElement.sValue = (string)oCR.oResult;
            //CCompiler oCompiler = new CCompiler();
            //var Result = oCompiler.Compile_FromText(sScriptNotation);
            return oCR;
        }

        //My Code
        public CResult Get_XIBOScriptDetails(string ScriptID = "")
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
                XIDScript oXIDScript = new XIDScript();
                if (!string.IsNullOrEmpty(ScriptID))
                {
                    if (ScriptID != "0")
                    {
                        Dictionary<string, object> Params = new Dictionary<string, object>();
                        Params["ID"] = ScriptID;
                        oXIDScript = Connection.Select<XIDScript>("XIBOScript_T", Params).FirstOrDefault();
                    }
                    List<XIDropDown> Languages = new List<XIDropDown>();
                    Dictionary<string, object> TypeParams = new Dictionary<string, object>();
                    var Types = Connection.Select<XIDMasterData>("XIMasterData_T", TypeParams).ToList();
                    Languages = Types.Where(m => m.Name == "ScriptLanguages").ToList().Select(m => new XIDropDown { Value = m.ID, text = m.Expression }).ToList();
                    oXIDScript.ddlLanguages = Languages;

                    List<XIDropDown> ScriptTypes = new List<XIDropDown>();
                    ScriptTypes = Types.Where(m => m.Name == "ScriptTypes").ToList().Select(m => new XIDropDown { Value = m.ID, text = m.Expression }).ToList();
                    oXIDScript.ddlScriptTypes = ScriptTypes;

                    List<XIDropDown> StatusTypes = new List<XIDropDown>();
                    StatusTypes = Types.Where(m => m.Name == "Status Type").ToList().Select(m => new XIDropDown { Value = m.ID, text = m.Expression }).ToList();
                    oXIDScript.ddlStatusTypes = StatusTypes;

                    List<XIDScriptResult> ScriptResults = new List<XIDScriptResult>();
                    Dictionary<string, object> ScriptResultParams = new Dictionary<string, object>();
                    ScriptResultParams["FKiScriptID"] = ScriptID;
                    ScriptResults = Connection.Select<XIDScriptResult>("XIBOScriptResult_T", ScriptResultParams).ToList();
                    oXIDScript.ScriptResults = ScriptResults;
                }
                oCResult.oResult = oXIDScript;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Menu definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                XIDXI oXIDXI = new XIDXI();
                oXIDXI.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }


        public XIDScript Get_ScriptDetails(int iBOScriptID = 0)
        {
            XIDScript oXIDScript = new XIDScript();
            try
            {
                string sScriptID = iBOScriptID.ToString();
                if (!string.IsNullOrEmpty(sScriptID))
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["ID"] = sScriptID;
                    oXIDScript = Connection.Select<XIDScript>("XIBOScript_T", Params).FirstOrDefault();
                    List<XIDropDown> Languages = new List<XIDropDown>();
                    Dictionary<string, object> TypeParams = new Dictionary<string, object>();
                    var Types = Connection.Select<XIDMasterData>("XIMasterData_T", TypeParams).ToList();
                    Languages = Types.Where(m => m.Name == "ScriptLanguages").ToList().Select(m => new XIDropDown { Value = m.ID, text = m.Expression }).ToList();
                    oXIDScript.ddlLanguages = Languages;

                    List<XIDropDown> ScriptTypes = new List<XIDropDown>();
                    ScriptTypes = Types.Where(m => m.Name == "ScriptTypes").ToList().Select(m => new XIDropDown { Value = m.ID, text = m.Expression }).ToList();
                    oXIDScript.ddlScriptTypes = ScriptTypes;

                    List<XIDropDown> StatusTypes = new List<XIDropDown>();
                    StatusTypes = Types.Where(m => m.Name == "Status Type").ToList().Select(m => new XIDropDown { Value = m.ID, text = m.Expression }).ToList();
                    oXIDScript.ddlStatusTypes = StatusTypes;

                    List<XIDScriptResult> ScriptResults = new List<XIDScriptResult>();
                    Dictionary<string, object> ScriptResultParams = new Dictionary<string, object>();
                    ScriptResultParams["FKiScriptID"] = ID;
                    ScriptResults = Connection.Select<XIDScriptResult>("XIBOScriptResult_T", ScriptResultParams).ToList();
                    oXIDScript.ScriptResults = ScriptResults;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Menu definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                XIDXI oXIDXI = new XIDXI();
                oXIDXI.SaveErrortoDB(oCResult);
            }
            return oXIDScript;
        }
    }
}