using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XISystem;

namespace XICore
{
    public class XIIScript : XIInstanceBase
    {
        public object EvalueNotation(string sScript, XIBOInstance oStructureInstance, int iTemplateID = 0, string sNotaionType = "", string sBOName = null)
        {
            CResult oCResult = new CResult(); object oResult = "";
            try
            {
                string sCode = @"  using System;
                  using System.Core;
                  using System.Linq;
                  using System.Collections.Generic;
                  using System.Threading;
                  using System.Data;
                  using System.Data.SqlClient;
                  using System.Data.Entity;
                  using XISystem;
                  using XICore;
                   namespace XIScriptingNotation
                   {                
                       public class cXIScriptingNotation
                       {                
                             public static CResult GetNotationResult(XIBOInstance oStructureInstance)
                  {
              XIIQS oXIIQS = new XIIQS();
              XIIBO oBII = new XIIBO(); XIIXI oIXI = new XIIXI();
              XIFunctions oFunctions=new XIFunctions();
             CResult CResult = new CResult();
            var oLIst = " + sScript + @";
             CResult.oResult = oLIst;
            return CResult;
        }
                       }
                   }
               ";

                XIInfraCache oCache = new XIInfraCache();
                var sSessionID = "Singlenotation";
                var Key = "SingleC#Compiler_" + sNotaionType + "_" + iTemplateID + "_" + sBOName;
                var sGUID = "SingleDynamicC#Compiler";

                CompilerResults results = null;
                if (iTemplateID > 0)
                {
                    results = (CompilerResults)oCache.Get_ObjectSetCache(Key, sGUID, sSessionID);
                }
                if (results == null)
                {
                    //var oLIst = oIXI.BOI(""ACPolicy_T"", ""127"").Structure(""ClientPolicy"").XILoad().oSubStructureI(""Driver_T"");
                    // var oLIst = " + sScript + @";
                    CompilerParameters loParameters = new CompilerParameters();
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\DLL";
                    string sdllPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin";
                    //string sPath = System.Configuration.ConfigurationManager.AppSettings["DLLPath"];
                    // *** Start by adding any referenced assemblies
                    loParameters.ReferencedAssemblies.Add("System.dll");
                    loParameters.ReferencedAssemblies.Add(sPath + "\\System.Core.dll");
                    loParameters.ReferencedAssemblies.Add(sPath + "\\System.Linq.dll");
                    //loParameters.ReferencedAssemblies.Add(sPath + "\\System.Data.Linq.dll");
                    loParameters.ReferencedAssemblies.Add(sPath + "\\System.Data.dll");
                    loParameters.ReferencedAssemblies.Add(sPath + "\\EntityFramework.dll");
                    loParameters.ReferencedAssemblies.Add(sPath + "\\EntityFramework.SqlServer.dll");
                    loParameters.ReferencedAssemblies.Add(sdllPath + "\\XICore.dll");
                    loParameters.ReferencedAssemblies.Add(sdllPath + "\\XISystem.dll");
                    // loParameters.ReferencedAssemblies.Add("D:\\TfsProject\\XIDNA\\XIDNA\\XIDNA.Repository\\bin\\Debug\\XIDNA.Repository.dll");
                    loParameters.GenerateInMemory = false;
                    ICodeCompiler provider = new CSharpCodeProvider().CreateCompiler();
                    results = provider.CompileAssemblyFromSource(loParameters, sCode);
                    oCache.Set_ObjectSetCache(sSessionID, Key, sGUID, results);
                }
                if (results.Errors.HasErrors)
                {

                    StringBuilder sb = new StringBuilder();

                    foreach (CompilerError error in results.Errors)
                    {
                        //Common.SaveErrorLog(error.ErrorText, "XIDNA");
                        sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                    }
                    throw new InvalidOperationException(sb.ToString());
                }

                Type binaryFunction = results.CompiledAssembly.GetType("XIScriptingNotation.cXIScriptingNotation");
                MethodInfo methodInfo = binaryFunction.GetMethod("GetNotationResult");
                if (oStructureInstance == null)
                {
                    oResult = methodInfo.Invoke(null, new object[] { null });
                }
                else
                {
                    oResult = methodInfo.Invoke(null, new object[] { oStructureInstance });
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            return oResult;
        }
        public object EvaluateMultiNotation(string[] Stringnotations, XIBOInstance oStructureInstance, int iTemplateID = 0, string sNotaionType = "", string sBOName = null)
        {

            CResult oCResult = new CResult(); object oResult = "";
            try
            {
                string sCode = @"  using System;
                  using System.Core;
                  using System.Linq;
                  using System.Collections.Generic;
                  using System.Threading;
                  using System.Data;
                  using System.Data.SqlClient;
                  using System.Data.Entity;
                  using System.Reflection;
                  using XISystem;
                  using XICore;
                   namespace XIScriptingNotation
                   {                
                       public class cXIScriptingNotation
                       {                
                             public static CResult GetNotationResult(XIBOInstance oStructureInstance, string[] Stringnotaions)
                  {
              XIIQS oXIIQS = new XIIQS();
              XIIBO oBII = new XIIBO();
              XIIXI oIXI = new XIIXI();
              XIDefinitionBase oXID = new XIDefinitionBase();
              XIFunctions oFunctions=new XIFunctions();
             CResult oCResult = new CResult();
        try{
           Dictionary<string, string> oDict = new Dictionary<string, string>();";
                foreach (var item in Stringnotations)
                {
                    CResult oCRes = new CResult();
                    //if(item== "oSubStructureI(\"acpolicy_t\").Item(0).AttributeI(\"rCommission\").sValue")
                    //{
                    //    var sCommission = oStructureInstance.oSubStructureI("acpolicy_t").Item(0).AttributeI("rCommission").sValue;
                    //    oCRes.oTraceStack.Add(new CNV { sName = "Commission", sValue = "Markerstudy Commission amount:"+sCommission });
                    //    SaveErrortoDB(oCRes);
                    //}
                    //if (item == "oSubStructureI(\"acpolicy_t\").Item(0).AttributeI(\"rNetToSupplier\").sValue")
                    //{
                    //    var rNetToSupplier = oStructureInstance.oSubStructureI("acpolicy_t").Item(0).AttributeI("rNetToSupplier").sValue;
                    //    oCRes.oTraceStack.Add(new CNV { sName = "rNetToSupplier", sValue = "KGM rNetToSupplier:" + rNetToSupplier });
                    //    SaveErrortoDB(oCRes);
                    //}
                    //if (item == "oSubStructureI(\"acpolicy_t\").Item(0).AttributeI(\"rTax\").sValue")
                    //{
                    //    var rTax = oStructureInstance.oSubStructureI("acpolicy_t").Item(0).AttributeI("rTax").sValue;
                    //    oCRes.oTraceStack.Add(new CNV { sName = "rTax", sValue = "KGM rTax:" + rTax });
                    //    SaveErrortoDB(oCRes);
                    //}

                    string Notation = string.Empty;
                    if (item.StartsWith("oFunctions."))
                    {
                        Notation = item;
                    }
                    else
                    {
                        Notation = "oStructureInstance." + item;
                    }
                    sCode = sCode + "oDict.Add(\"" + item.ToString().Replace("\"", "\\\"") + "\", " + Notation + @");";
                }
                sCode = sCode + @"oCResult.oResult = oDict;
                }catch(Exception ex){
                oCResult.sMessage = ""Script ERROR: [""+oCResult.Get_Class() +"".""+ System.Reflection.MethodBase.GetCurrentMethod().Name+"" ] -""+ ex.Message+"" - Trace: ""+ ex.StackTrace+"" \r\n"";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
                       }
                   }
               ";
                XIInfraCache oCache = new XIInfraCache();
                var sSessionID = "Multinotation";
                var Key = "MultiC#Compiler_" + sNotaionType + "_" + iTemplateID + "_" + sBOName;
                var sGUID = "MultiDynamicC#Compiler";

                CompilerResults results = null;
                if (iTemplateID > 0 && sNotaionType != "[[]]" && !Stringnotations.Any(m=>m.Contains(":")))
                {
                    results = (CompilerResults)oCache.Get_ObjectSetCache(Key, sGUID, sSessionID);
                }
                if (results == null)
                {
                    //var oLIst = oIXI.BOI(""ACPolicy_T"", ""127"").Structure(""ClientPolicy"").XILoad().oSubStructureI(""Driver_T"");
                    // var oLIst = " + sScript + @";
                    CompilerParameters loParameters = new CompilerParameters();
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\DLL";
                    string sdllPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin";
                    //string sPath = System.Configuration.ConfigurationManager.AppSettings["DLLPath"];
                    // *** Start by adding any referenced assemblies
                    loParameters.ReferencedAssemblies.Add("System.dll");
                    loParameters.ReferencedAssemblies.Add(sPath + "\\System.Core.dll");
                    loParameters.ReferencedAssemblies.Add(sPath + "\\System.Linq.dll");
                    //loParameters.ReferencedAssemblies.Add(sPath + "\\System.Data.Linq.dll");
                    loParameters.ReferencedAssemblies.Add(sPath + "\\System.Data.dll");
                    loParameters.ReferencedAssemblies.Add(sPath + "\\EntityFramework.dll");
                    loParameters.ReferencedAssemblies.Add(sPath + "\\EntityFramework.SqlServer.dll");
                    loParameters.ReferencedAssemblies.Add(sdllPath + "\\XICore.dll");
                    loParameters.ReferencedAssemblies.Add(sdllPath + "\\XISystem.dll");
                    // loParameters.ReferencedAssemblies.Add("D:\\TfsProject\\XIDNA\\XIDNA\\XIDNA.Repository\\bin\\Debug\\XIDNA.Repository.dll");
                    loParameters.GenerateInMemory = false;
                    ICodeCompiler provider = new CSharpCodeProvider().CreateCompiler();
                    results = provider.CompileAssemblyFromSource(loParameters, sCode);
                    oCache.Set_ObjectSetCache(sSessionID, Key, sGUID, results);
                }

                if (results.Errors.HasErrors)
                {

                    StringBuilder sb = new StringBuilder();

                    foreach (CompilerError error in results.Errors)
                    {
                        //Common.SaveErrorLog(error.ErrorText, "XIDNA");
                        sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                    }
                    throw new InvalidOperationException(sb.ToString());
                }

                Type binaryFunction = results.CompiledAssembly.GetType("XIScriptingNotation.cXIScriptingNotation");
                MethodInfo methodInfo = binaryFunction.GetMethod("GetNotationResult");

                if (oStructureInstance == null)
                {
                    oResult = methodInfo.Invoke(null, new object[] { null });
                }
                else
                {
                   oResult = methodInfo.Invoke(null, new object[] { oStructureInstance, Stringnotations });
                }
                //oCResult.oResult = oResult;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            return oResult;

        }
    }
}