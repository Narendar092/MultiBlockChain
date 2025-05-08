using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XICore
{
    public class XIInfraTabComponent : XIDefinitionBase
    {
        public string sStructureCode { get; set; }
        public int iBODID { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sOutputContent { get; set; }
        public string sMode { get; set; }

        public CResult XILoad(List<CNV> oParams)
        {
            XIInfraCache oCache = new XIInfraCache();
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
            List<XIDStructure> oTree = new List<XIDStructure>();
            try
            {
                sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                var BODID = oParams.Where(m => m.sName == XIConstant.Param_BODID).Select(m => m.sValue).FirstOrDefault();
                int iBODID = 0;
                Guid BOIDXIGUID = Guid.Empty;
                int.TryParse(BODID, out iBODID);
                Guid.TryParse(BODID, out BOIDXIGUID);
                sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                //if (!string.IsNullOrEmpty(sGUID))
                //{
                //    oCache.sSessionID = sSessionID;
                //    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                //    sGUID = ParentGUID;
                //}
                sStructureCode = oParams.Where(m => m.sName == XIConstant.Param_StructureCode).Select(m => m.sValue).FirstOrDefault();
                sMode = oParams.Where(m => m.sName == "Mode").Select(m => m.sValue).FirstOrDefault();
                sOutputContent = oParams.Where(m => m.sName == XIConstant.Param_OutputContent).Select(m => m.sValue).FirstOrDefault();
                var sCallHierarchy = oParams.Where(m => m.sName == XIConstant.Param_CallHierarchy).Select(m => m.sValue).FirstOrDefault();
                var ActiveBO = oCache.Get_ParamVal(sSessionID, sGUID, null, XIConstant.XIP_BOName);
                var iBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, XIConstant.XIP_InstanceID);
                if ((iBODID > 0 || (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)) && !string.IsNullOrEmpty(sStructureCode))
                {
                    XIDStructure oStrct = new XIDStructure();
                    if (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)
                    {
                        oTree = oStrct.GetXIStructureTreeDetails(BOIDXIGUID.ToString(), sStructureCode);
                    }
                    else if (iBODID > 0)
                    {
                        oTree = oStrct.GetXIStructureTreeDetails(iBODID.ToString(), sStructureCode);
                    }
                    XIIXI oIXI = new XIIXI();

                    //it might be getting used in Policy popup risk view tree structure
                    //keeping in comment to test performance 
                    //if (!string.IsNullOrEmpty(ActiveBO))
                    //{
                    //    var oStructobj = oIXI.BOI(ActiveBO, iBOIID).Structure(sStructureCode).XILoad("partial");
                    //    foreach (var items in oTree)
                    //    {
                    //        if (items.sMode != null && items.sMode.ToLower() == "single")
                    //        {
                    //            if (!string.IsNullOrEmpty(items.sBO))
                    //            {
                    //                var NodeData = oStructobj.oChildBOI(items.sBO);
                    //                if (NodeData != null)
                    //                {
                    //                    if (NodeData.FirstOrDefault().BOD != null)
                    //                    {
                    //                        var sPrimaryKey = NodeData.FirstOrDefault().BOD.sPrimaryKey.ToLower();
                    //                        var PKValue = NodeData.FirstOrDefault().Attributes[sPrimaryKey].sValue;
                    //                        items.iInstanceID = Convert.ToInt32(PKValue);
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        else if (items.sMode != null && items.sMode.ToLower() == "Multiple".ToLower())
                    //        {
                    //            //var NodeData = oStructobj.oChildBOI(items.sBO);
                    //            //if (NodeData != null)
                    //            //{
                    //            //    if (NodeData.FirstOrDefault().BOD != null)
                    //            //    {
                    //            //        int iOneClickID = NodeData.FirstOrDefault().BOD.iOneClickID;
                    //            //        items.i1ClickID = iOneClickID;
                    //            //    }
                    //            //}
                    //        }
                    //    }
                    //}
                    if (oTree.Select(m => m.StatusTypeID == 10).ToList().Count() == 0)
                    {
                        oCResult.sMessage = "Config Error: XIInfraTabComponent_XILoad() : There are no nodes for Structure Code " + sStructureCode + " with status active - Call Hierarchy: " + sCallHierarchy;
                        oCResult.sCode = "Config Error";
                        SaveErrortoDB(oCResult);
                    }
                    oCResult.oResult = oTree;
                    if (oTree != null && oTree.Count() > 0)
                    {
                        oTree.ToList().ForEach(m => m.sOutputContent = sOutputContent);
                    }
                }
                else if (!string.IsNullOrEmpty(sStructureCode))
                {
                    if (!string.IsNullOrEmpty(sMode) && sMode.ToLower() == "nonbo")
                    {
                        XIDXI oXID = new XIDXI();
                        var oRes = (List<XIDStructure>)(oXID.Get_StructureNodes(sStructureCode).oResult);
                        oRes.FirstOrDefault().sMode = sMode;
                        oCResult.oResult = oRes;
                    }
                }
                else
                {
                    oTree = oTree == null ? oTree = new List<XIDStructure>() : oTree;
                    if (oTree.Count > 0)
                    {
                        oTree.FirstOrDefault().sErrorMessage = "Mandatory parameters not passed to load the Tab component";
                    }
                    oCResult.oResult = oTree;
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Params loading error for Tab Component" });
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oTree = oTree == null ? oTree = new List<XIDStructure>() : oTree;
                oTree.FirstOrDefault().sErrorMessage = "Something went wrong while loading the Tab component";
                oCResult.oResult = oTree;
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Tab Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }

            return oCResult;
        }
    }
}