//using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Caching;
using XISystem;
using System.Net.Mail;
using System.Threading;

namespace XICore
{
    public class CMethodStepInstance
    {

        private object oMyResult;

        private string sMyKey;

        public string sKey
        {
            get
            {
                return sMyKey;
            }
            set
            {
                sMyKey = value;
            }
        }

        private string sMyMethodTrace;

        public string sMethodTrace
        {
            get
            {
                return sMyMethodTrace;
            }
            set
            {
                sMyMethodTrace = value;
            }
        }

        private CMethodStepDefinition oMyDefinition;

        public CMethodStepDefinition oDefinition
        {
            get
            {
                return oMyDefinition;
            }
            set
            {
                oMyDefinition = value;
                iExecuteStatus = xiAlgoExecuteStatus.xiWaiting;
            }
        }

        private CAlgorithmInstance oMyAlgoI;

        public CAlgorithmInstance oAlgoI
        {
            get
            {
                return oMyAlgoI;
            }
            set
            {
                oMyAlgoI = value;
            }
        }

        public object oMethodResult
        {
            get
            {
                return oMyResult;
            }
            set
            {
                oMyResult = value;
            }
        }

        private xiAlgoExecuteStatus iMyExecuteStatus;

        public xiAlgoExecuteStatus iExecuteStatus
        {
            get
            {
                return iMyExecuteStatus;
            }
            set
            {
                iMyExecuteStatus = value;
            }
        }

        public string sGUID { get; set; }
        public string sSessionID { get; set; }
        public Dictionary<string, string> AlgoDict = new Dictionary<string, string>();
        public CAlgorithmInstance oAlgoIns = new CAlgorithmInstance();
        XIInfraUsers oUser = new XIInfraUsers();
        public enum xiAlgoExecuteStatus
        {

            xiWaiting = 10,

            xiProcessing = 20,

            xiComplete = 30,

            xiError = 40,
        }

        public CResult ConcatDebug(ref string sDebug)
        {
            CResult oCResult = new CResult();
            //CMethodStepDefinition oSubMethod = null;
            CMethodStepInstance oSubMethodI = null;
            sDebug = sDebug + sMyMethodTrace;
            foreach (CMethodStepDefinition oSubMethod in oDefinition.NMethodSteps.Values)
            {
                if (oAlgoI.oSteps.ContainsKey(oSubMethod.sKey))
                {
                    oSubMethodI = oAlgoI.oSteps[oSubMethod.sKey];
                    oSubMethodI.ConcatDebug(ref sDebug);
                }
            }
            return oCResult;
        }

        public CResult Execute(string sSessionID, string sGUID, string FKiPCDID, iSiganlR oSignalR)
        {
            //Pass oCXIAPI, remove sessionid and GUID
            var watch = System.Diagnostics.Stopwatch.StartNew();
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            CResult oCResult = new CResult();
            CResult oCR = null;
            CMethodStepDefinition.xiAlgoActionType iAction = CMethodStepDefinition.xiAlgoActionType.xiNothing;
            string sExecute = "";
            CXICorePlaceholder oPlaceholder = new CXICorePlaceholder();
            XIIBO oBOI1 = null;
            XIIBO oBOI2 = null;
            CAlgorithmDefinition oAlgoD = null;
            //CMethodStepDefinition oSubMethod = null;
            BOInstanceGridPlaceholder oDictionary = new BOInstanceGridPlaceholder();
            CMethodStepInstance oSubMethodI = null;
            string oSValue1 = "";
            string oSValue2 = "";
            bool bYes = false;
            // so this class knows its parent definition
            //   and it can execute child lines first. The only reason for having child lines is if this is an iterative method (??)
            //   then, depending on its type it can do certain things
            // ALERT TO DO - the compiler should check that the return of a method is compatible with the action
            // ALERT TO DO - in the compiler, are there parameters matching the expected input params for the action
            // ALERT TO DO - if you reference algo inbound params, are they defined in the algo param set?
            // ALERT TO DO - the compiler has to check that an iterate method uses an input that can be iterated (such as a BOInstanceGrid). This might be relatively wide ranging and could also be numbers - eg 1 to 5 or something
            // ALERT TO DO - compiler to make sure each line is named uniquely. Or it can have no name and use a default
            // TO DO - replace CXI with simple NV with the NNodes method - use NMethodDefinitionas example template
            // need to get the parameters
            // oAlgoI.
            // xi.ag|DiaryTemplate|p.PolicyFK,p.dGoLive,p.TemplateID
            oAlgoD = oAlgoI.Definition;
            Debug.WriteLine(oDefinition.sStepName);
            sMyMethodTrace = sMyMethodTrace + oDefinition.sStepName + " [" + oDefinition.iActionType.ToString() + "] " + ": ";
            iAction = oDefinition.iActionType;
            CXIAPI oXIAPI = new CXIAPI();
            List<CNV> oParams = new List<CNV>();
            string WhrParams = string.Empty;
            XIInfraCache oCache1 = new XIInfraCache();
            List<CNV> oMergeParms = new List<CNV>();
            oAlgoIns.oSignalR = oSignalR;
            if (oDefinition.iMethodType == CMethodStepDefinition.xiMethodType.xiMethod)
            {
                switch (iAction)
                {
                    case CMethodStepDefinition.xiAlgoActionType.xiQSLoad:
                        oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                        oSValue1 = oCR.oResult.ToString();
                        oCR = oPlaceholder.QSLoad(oSValue1, sSessionID, sGUID);
                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                        {
                            // assign result to the instance in the OM
                            sMyMethodTrace = sMyMethodTrace + " QSI loaded succesfully with QSIID \'" + oSValue1 + "\'" + "\r\n";
                            oMethodResult = oCR.oResult;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            sMyMethodTrace = sMyMethodTrace + " QSI loading is falied with QSIID \'" + oSValue1 + "\'" + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }

                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiBOLoad:
                        // xi.m|s:-|i:PolicyI|r:BOInstance|e:DBLog,xiFail|a:m.BOLoad|x:Policy_T,PolicyFK
                        // expecting 2 params. These have to be here, the compiler is checking, we can assume
                        // ALERT TO DO - Interpret these inputs - so they might be parameters or some other dynamic value
                        List<CNV> sWhrCond = new List<CNV>();
                        var Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        for (int i = 0; i < Params.Count(); i++)
                        {
                            if (i == 0)
                            {
                                oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                {
                                    oSValue1 = oCR.oResult.ToString();
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                string sParam = string.Empty;
                                var sWhrParam = ((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue;
                                sParam = sWhrParam;
                                bool bIsnotPK = false;
                                if (sWhrParam.Contains('='))
                                {
                                    sWhrParam = sWhrParam.Replace("[", "").Replace("]", "");
                                    var NVs = sWhrParam.Split('=').ToList();
                                    var NVPair = NVs[1].Split('.').ToList();
                                    if (NVPair.Count() == 2)
                                    {
                                        sParam = NVs[1];
                                        oCR = Get_ParameterObject(sParam);
                                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                        {
                                            oSValue2 = oCR.oResult.ToString();
                                            //oSValue2 = sWhrParam.Replace(sParam, oSValue2);
                                            sWhrCond.Add(new CNV { sName = NVs[0], sValue = oSValue2 });
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                        }
                                        else
                                        {
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        }
                                    }
                                    else if (NVPair.Count() == 3)
                                    {
                                        sParam = NVPair[0] + "." + NVPair[1];
                                        oCR = Get_ParameterObject(sParam);
                                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                        {
                                            var BOI = (XIIBO)oCR.oResult;
                                            if (BOI.Attributes.ContainsKey(NVPair[2]))
                                            {
                                                oSValue2 = BOI.Attributes[NVPair[2]].sValue;
                                                //oSValue2 = sWhrParam.Replace(sParam+"."+NVPair[2], oSValue2);
                                                sWhrCond.Add(new CNV { sName = NVs[0], sValue = oSValue2 });
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                            }
                                        }
                                        else
                                        {
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        }
                                    }
                                    //sWhrCond.Add(oSValue2);
                                    //bIsnotPK = true;
                                }

                                //oCR = Get_ParameterObject(sParam);
                                //oSValue2 = oCR.oResult.ToString();
                                //if (bIsnotPK)
                                //{
                                //    oSValue2 = sWhrParam.Replace(sParam, oSValue2);
                                //    sWhrCond.Add(oSValue2);
                                //}
                            }
                        }
                        if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                        {
                            oCR = oPlaceholder.BOLoad(oSValue1, sWhrCond);
                            // oAlgoD.oXIParameters.Get_NodeByNumber(1).sValue, oAlgoD.oMethodOM.oParamSet.Get_NodeByNumber(2).sValue)
                            List<string> Whr = new List<string>();
                            foreach (var items in sWhrCond)
                            {
                                Whr.Add(items.sName + "=" + items.sValue);
                            }
                            oSValue2 = string.Join(" and ", Whr);
                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                            {
                                // assign result to the instance in the OM
                                sMyMethodTrace = sMyMethodTrace + " \'" + oSValue1 + "\' BO instance loaded with Where Clause \'" + oSValue2 + "\' Successfully" + "\r\n";
                                oMethodResult = oCR.oResult;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " \'" + oSValue1 + "\' BO instance loaded with Where Clause \'" + oSValue2 + "\' Failed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            sMyMethodTrace = sMyMethodTrace + " BO instance loading failed" + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiBOCopy:
                        // xi.m|s:--|i:CopyDiary|r:xiR|e:DBLog, xiFail|a: m.BOCopy|x:EachDiaryT, xim.CreateDiaryInstance, xig.CopyLive
                        // Need to get hold of 2 BOs. in the above example, one is from the iteration and one is from a method which created
                        oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                        oBOI1 = (XIIBO)oCR.oResult;
                        oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue);
                        oBOI2 = (XIIBO)oCR.oResult;
                        oCR = oPlaceholder.BOCopy(oBOI1, oBOI2);
                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                        {
                            sMyMethodTrace = sMyMethodTrace + "With \'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' and \'"
                                        + ((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue + "\' Copied Successfully" + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            sMyMethodTrace = sMyMethodTrace + "With \'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' and \'"
                                        + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' FAILED" + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }

                        oMethodResult = oCR.xiStatus;
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiBOSave:
                        // xi.m|s:--|i:InsertDiary|r:xiR|e:DBLog, xiFail|a: m.BOSave|x:xim.CreateDiaryInstance
                        bool bValidate = true;
                        bool bAction = true;
                        Params = new List<CNodeItem>();
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                        oBOI1 = (XIIBO)oCR.oResult;
                        if (Params.Count() > 1)
                        {

                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue);
                            var sValidate = (string)oCR.oResult;
                            if (sValidate == "false")
                            {
                                bValidate = false;
                            }
                        }
                        if (Params.Count() > 2)
                        {
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[2]).sValue);
                            var sAction = (string)oCR.oResult;
                            if (sAction == "false")
                            {
                                bAction = false;
                            }
                        }
                        var ContextValue = oCache1.Get_ParamVal(sSessionID, sGUID, null, "{-sContextCode}");
                        oCR = oPlaceholder.BOSave(oBOI1, "", bValidate, oSignalR, bAction, ContextValue);
                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                        {
                            sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' BO Instance Saved Successfully" + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' BO Instance Save FAILED" + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }

                        oMethodResult = oCR.xiStatus;
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiCreateBO:
                        // xi.m|s:--|i:CreateDiaryInstance|r:BOInstance|e:DBLog, xiFail|a: m.CreateBO|x:bod.Diary_T
                        oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                        oSValue1 = oCR.oResult.ToString();
                        oCR = oPlaceholder.BOCreate(oSValue1);
                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                        {
                            // assign result to the instance in the OM
                            oMethodResult = oCR.oResult;
                            sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' Created Successfully" + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' Create FAILED" + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }

                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiCondition:
                    case CMethodStepDefinition.xiAlgoActionType.xiConditionNot:
                        // xi.m|s:--|i:IfClient7|a:m.Condition|sc:{if|{eq|{xi.p|'-clientid'},'7'},'y','n'}
                        var sScript = oDefinition.sExecute;
                        CScriptController oXIScript = new CScriptController();
                        oCR = oXIScript.API2_Serialise_From_String(sScript);
                        oCR = oXIScript.API2_ExecuteMyOM("", null, oAlgoI, sGUID, sSessionID);
                        //var oResult = oCR.oResult;

                        //oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                        {
                            var oResult = oCR.oResult.ToString();
                            // TO DO - Execute the condition (or get it??)
                            if (iAction == CMethodStepDefinition.xiAlgoActionType.xiConditionNot)
                            {
                                bYes = !bYes;
                            }
                            else
                            {
                                if (oResult == "y" || oResult == "true")
                                {
                                    bYes = true;
                                }
                            }
                            if (bYes)
                            {
                                sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' Executing" + "\r\n";
                                foreach (CMethodStepDefinition oSubMethod in oDefinition.NMethodSteps.Values)
                                {
                                    if (oAlgoI.oSteps.ContainsKey(oSubMethod.sKey))
                                    {
                                        sMyMethodTrace = sMyMethodTrace + "\'" + oSubMethod.sKey + "\'" + "\r\n";
                                        oSubMethodI = oAlgoI.oSteps[oSubMethod.sKey];
                                        oCR = oSubMethodI.Execute(sSessionID, sGUID, FKiPCDID, oSignalR);
                                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                        {
                                            // do anything?
                                            oCR = oUser.Save_UserMessage(FKiPCDID.ToString(), "", oSubMethod.sKey, oCR.xiStatus);
                                        }
                                        else
                                        {
                                            oCR = oUser.Save_UserMessage(FKiPCDID.ToString(), "", oSubMethod.sKey, oCR.xiStatus);
                                            oCResult.xiStatus = oCR.xiStatus;
                                            // keep going?
                                        }

                                    }
                                }

                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' Not Executed" + "\r\n";
                            }
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            // bYes
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }

                        // oMethodResult = oCR.xiStatus
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiRecursive:
                        oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                        {
                            var sType = oCR.oResult.GetType().Name;
                            var sCondVal = string.Empty;
                            if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "BOInstanceGridPlaceholder".ToLower())
                            {
                                oDictionary = (BOInstanceGridPlaceholder)oCR.oResult;
                                //var vars = (BOInstanceGridPlaceholder)oCR.oResult;
                                if (oDictionary != null && oDictionary.oBOInstances.Count() > 0)
                                {
                                    sCondVal = oDictionary.oBOInstances.Count().ToString();
                                    var iCondVal = 0;
                                    int.TryParse(sCondVal, out iCondVal);
                                    if (iCondVal > 0)
                                    {
                                        foreach (var oIterate in oDictionary.oBOInstances.Values.ToList())//.oBOInstances.values)
                                        {
                                            oMethodResult = oIterate;
                                            foreach (CMethodStepDefinition oSubMethod in oDefinition.NMethodSteps.Values)
                                            {
                                                if (oAlgoI.oSteps.ContainsKey(oSubMethod.sKey))
                                                {
                                                    sMyMethodTrace = sMyMethodTrace + "\'" + oSubMethod.sKey + "\'" + "\r\n";
                                                    oSubMethodI = oAlgoI.oSteps[oSubMethod.sKey];
                                                    oCR = oSubMethodI.Execute(sSessionID, sGUID, FKiPCDID, oSignalR);
                                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                                    {
                                                        // do anything?
                                                    }
                                                    else
                                                    {
                                                        oCResult.xiStatus = oCR.xiStatus;
                                                        // keep going?
                                                    }
                                                }
                                            }
                                            Execute(sSessionID, sGUID, FKiPCDID, oSignalR);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                sCondVal = (string)oCR.oResult;
                                var iCondVal = 0;
                                int.TryParse(sCondVal, out iCondVal);
                                if (iCondVal > 0)
                                {
                                    foreach (CMethodStepDefinition oSubMethod in oDefinition.NMethodSteps.Values)
                                    {
                                        if (oAlgoI.oSteps.ContainsKey(oSubMethod.sKey))
                                        {
                                            sMyMethodTrace = sMyMethodTrace + "\'" + oSubMethod.sKey + "\'" + "\r\n";
                                            oSubMethodI = oAlgoI.oSteps[oSubMethod.sKey];
                                            oCR = oSubMethodI.Execute(sSessionID, sGUID, FKiPCDID, oSignalR);
                                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                            {
                                                // do anything?
                                            }
                                            else
                                            {
                                                oCResult.xiStatus = oCR.xiStatus;
                                                // keep going?
                                            }
                                        }
                                    }
                                    Execute(sSessionID, sGUID, FKiPCDID, oSignalR);
                                }
                            }
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiIterate:
                        // xi.m|s:-|i:EachDiaryT|r:xiR|e:DBLog, xiFail|a: m.Iterate|x:xim.DiaryTemplate
                        // has to be a dictionary?
                        // 
                        oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                        {
                            //oDictionary = oCR.oResult;

                            oDictionary = (BOInstanceGridPlaceholder)oCR.oResult;
                            foreach (var oIterate in oDictionary.oBOInstances.Values.ToList())//.oBOInstances.values)
                            {
                                oMethodResult = oIterate;
                                // this cycle, changes each iteration
                                // FOR EACH [GET SUB ENTITY FROM COLLECTION] 'Set the current instance 
                                //  THEN WITHIN EACH ITEM DO THESE STEPS:
                                foreach (CMethodStepDefinition oSubMethod in oDefinition.NMethodSteps.Values)
                                {
                                    if (oAlgoI.oSteps.ContainsKey(oSubMethod.sKey))
                                    {
                                        sMyMethodTrace = sMyMethodTrace + "\'" + oSubMethod.sKey + "\'" + "\r\n";
                                        oSubMethodI = oAlgoI.oSteps[oSubMethod.sKey];
                                        oCR = oSubMethodI.Execute(sSessionID, sGUID, FKiPCDID, oSignalR);
                                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                        {
                                            // do anything?
                                            oCR = oUser.Save_UserMessage(FKiPCDID.ToString(), "", oSubMethod.sKey, oCR.xiStatus);
                                        }
                                        else
                                        {
                                            oCR = oUser.Save_UserMessage(FKiPCDID.ToString(), "", oSubMethod.sKey, oCR.xiStatus);
                                            oCResult.xiStatus = oCR.xiStatus;
                                            // keep going?
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                        // oMethodResult = oCR.xiStatus
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiSetLineValue:
                        Params = new List<CNodeItem>();
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        var Param = ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue;
                        if (Param.Contains('='))
                        {
                            var Splits = Param.Split('=').ToList();
                            var NVs = Splits[1].Split('.').ToList();
                            if (NVs.Count() == 3)
                            {
                                oCR = Get_ParameterObject(NVs[0] + "." + NVs[1]);
                                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                {
                                    var BOI = (XIIBO)oCR.oResult;
                                    if (BOI.Attributes.ContainsKey(NVs[2]))
                                    {
                                        oSValue1 = BOI.Attributes[NVs[2]].sValue;
                                        oAlgoI.oSteps[Splits[0].ToLower()].oMyResult = oSValue1;
                                        sMyMethodTrace = sMyMethodTrace + " Algorithm variable " + Splits[0].ToLower() + " Value is set to \'" + oSValue1 + "\' Successfully" + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + "Algorithm variable" + Splits[0].ToLower() + "value setting is failed" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else if (NVs.Count() == 2)
                            {
                                oCR = Get_ParameterObject(NVs[0] + "." + NVs[1]);
                                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                {
                                    var sType = oCR.oResult.GetType().Name;
                                    if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "BOInstanceGridPlaceholder".ToLower())
                                    {
                                        oAlgoI.oSteps[Splits[0].ToLower()].oMyResult = oCR.oResult;
                                        sMyMethodTrace = sMyMethodTrace + " Algorithm variable " + Splits[0].ToLower() + " Value is set to " + oCR.oResult + " Successfully" + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        oAlgoI.oSteps[Splits[0].ToLower()].oMyResult = oCR.oResult;
                                        sMyMethodTrace = sMyMethodTrace + " Algorithm variable " + Splits[0].ToLower() + " Value is set to " + oCR.oResult + " Successfully" + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + "Algorithm variable" + Splits[0].ToLower() + "value setting is failed" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiSetValue:
                        // xi.m|s:--|i:SetValue1|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[dDue=xim.EachDiary.iDaysFromZero+p.dGoLive]
                        Params = new List<CNodeItem>();
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count() == 1)
                        {
                            var NV = ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue;
                            var NVs = NV.Split('.').ToList();
                            if (NVs.Count() == 3)
                            {
                                oCR = Get_ParameterObject(NVs[0] + "." + NVs[1]);
                                var sType = oCR.oResult.GetType().Name;
                                if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                {
                                    var oQSI = (XIIQS)oCR.oResult;
                                    if (oQSI.XIValues.ContainsKey(NVs[2]))
                                    {
                                        oSValue1 = oQSI.XIIValues(NVs[2]);
                                    }
                                }
                                else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                {
                                    var BOI = (XIIBO)oCR.oResult;
                                    if (BOI.Attributes.ContainsKey(NVs[2]))
                                    {
                                        oSValue1 = BOI.Attributes[NVs[2]].sValue;
                                    }
                                    else
                                    {
                                        oSValue1 = "";
                                    }
                                }

                            }
                            else
                            {
                                oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                                oSValue1 = (string)oCR.oResult;
                            }
                            oMethodResult = oSValue1;
                            if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                            {
                                sMyMethodTrace = sMyMethodTrace + "To \'" + oDefinition.sStepName + "\' Value \'"
                                            + oSValue1 + "\' Set Successfully" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + "To \'" + oDefinition.sStepName + "\' Value \'"
                                            + oSValue1 + "\' Set FAILED" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else if (Params.Count() == 2)
                        {
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                            oBOI1 = (XIIBO)oCR.oResult;
                            var NV = ((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue;
                            var NVSplits = NV.Split('=').ToList();
                            var IndirAttr = NVSplits[0];
                            var DeriverAttr = string.Empty;
                            if (IndirAttr.Contains('.'))
                            {
                                if (IndirAttr.StartsWith("["))
                                {
                                    IndirAttr = IndirAttr.Replace("[", "");
                                }
                                oCR = Get_ParameterObject(IndirAttr, sSessionID, sGUID);
                                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess && oCR.oResult != null))
                                {
                                    DeriverAttr = (string)oCR.oResult;
                                    if (!string.IsNullOrEmpty(DeriverAttr))
                                    {
                                        NV = NV.Replace(IndirAttr, DeriverAttr);
                                    }
                                }
                            }
                            var sParamName = ((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue;
                            oCR = Get_ParameterName(sParamName);
                            if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                            {
                                sParamName = (string)oCR.oResult;
                            }
                            var sParms = sParamName.Split('.').ToList();
                            string sAttr = string.Empty;
                            if (sParms.Count() == 3)
                            {
                                sParamName = sParms[0] + "." + sParms[1];
                                sAttr = sParms[2];
                            }
                            oCR = Get_ParameterObject(sParamName, sSessionID, sGUID);
                            if (!string.IsNullOrEmpty(sAttr) && oCR.oResult != null)
                            {
                                var sType = oCR.oResult.GetType().Name;
                                if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                {
                                    var oQSI = (XIIQS)oCR.oResult;
                                    oSValue1 = oQSI.XIIValues(sAttr);
                                    sParamName = sParamName + "." + sAttr;
                                }
                                else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                {
                                    var oBOI = (XIIBO)oCR.oResult;
                                    if (oBOI != null && oBOI.Attributes.Count() > 0)
                                    {
                                        if (oBOI.Attributes.ContainsKey(sAttr))
                                        {
                                            oSValue1 = oBOI.Attributes[sAttr].sValue;
                                            sParamName = sParamName + "." + sAttr;
                                        }
                                        else
                                        {
                                            oSValue1 = "";
                                            sParamName = sParamName + "." + sAttr;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (oCR.oResult != null)
                                {
                                    oSValue1 = oCR.oResult.ToString();
                                }
                                else
                                {
                                    oSValue1 = "0";
                                }

                            }
                            NV = NV.Replace(sParamName, oSValue1);
                            oCR = oPlaceholder.BOSetValue(oBOI1, NV);
                            if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                            {
                                sMyMethodTrace = sMyMethodTrace + "To \'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' BO Instance \'"
                                            + NV + "\' Value Set Successfully" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + "To \'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' BO Instance \'"
                                            + NV + "\' Value Set Failed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                            oMethodResult = oCR.xiStatus;
                        }
                        else if (Params.Count() > 2)
                        {
                            Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                            int i = 0;
                            foreach (var val in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                                    oBOI1 = (XIIBO)oCR.oResult;
                                    i++;
                                }
                                else
                                {
                                    var NV = ((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue;
                                    var NVSplits = NV.Split('=').ToList();
                                    var IndirAttr = NVSplits[0];
                                    var DeriverAttr = string.Empty;
                                    if (IndirAttr.Contains('.'))
                                    {
                                        if (IndirAttr.StartsWith("["))
                                        {
                                            IndirAttr = IndirAttr.Replace("[", "");
                                        }
                                        oCR = Get_ParameterObject(IndirAttr, sSessionID, sGUID);
                                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess && oCR.oResult != null))
                                        {
                                            DeriverAttr = (string)oCR.oResult;
                                            if (!string.IsNullOrEmpty(DeriverAttr))
                                            {
                                                NV = NV.Replace(IndirAttr, DeriverAttr);
                                            }
                                        }
                                    }
                                    var sParamName = ((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue;
                                    oCR = Get_ParameterName(sParamName);
                                    if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                    {
                                        sParamName = (string)oCR.oResult;
                                    }
                                    var sParms = sParamName.Split('.').ToList();
                                    string sAttr = string.Empty;
                                    if (sParms.Count() == 3)
                                    {
                                        sParamName = sParms[0] + "." + sParms[1];
                                        sAttr = sParms[2];
                                    }
                                    oCR = Get_ParameterObject(sParamName, sSessionID, sGUID);
                                    if (!string.IsNullOrEmpty(sAttr))
                                    {
                                        var sType = oCR.oResult.GetType().Name;
                                        if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                        {
                                            var oQSI = (XIIQS)oCR.oResult;
                                            oSValue1 = oQSI.XIIValues(sAttr);
                                            sParamName = sParamName + "." + sAttr;
                                        }
                                        else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                        {
                                            var oBOI = (XIIBO)oCR.oResult;
                                            if (oBOI != null && oBOI.Attributes.Count() > 0)
                                            {
                                                if (oBOI.Attributes.ContainsKey(sAttr))
                                                {
                                                    oSValue1 = oBOI.Attributes[sAttr].sValue;
                                                    sParamName = sParamName + "." + sAttr;
                                                }
                                                else
                                                {
                                                    oSValue1 = "";
                                                    sParamName = sParamName + "." + sAttr;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (oCR.oResult != null || (!string.IsNullOrEmpty(sParamName) && sParamName.ToLower() == "xif.null"))
                                        {
                                            if (oCR.oResult != null)
                                                oSValue1 = oCR.oResult.ToString();
                                            else
                                                oSValue1 = null;
                                        }
                                        else
                                        {
                                            oSValue1 = "0";
                                        }

                                    }
                                    NV = NV.Replace(sParamName, oSValue1);
                                    oCR = oPlaceholder.BOSetValue(oBOI1, NV);
                                    i++;
                                    if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                    {
                                        sMyMethodTrace = sMyMethodTrace + "To \'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' BO Instance \'"
                                                    + NV + "\' Value Set Successfully" + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        sMyMethodTrace = sMyMethodTrace + "To \'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' BO Instance \'"
                                                    + NV + "\' Value Set Failed" + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                                oMethodResult = oCR.xiStatus;
                            }
                        }

                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiSetCache:
                        // xi.m|s:--|i:SetValue1|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[dDue=xim.EachDiary.iDaysFromZero+p.dGoLive]
                        Params = new List<CNodeItem>();
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count() == 2)
                        {
                            string sValue = string.Empty;
                            var sName = Params[0].sValue;
                            var NV = ((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue;
                            var NVs = NV.Split('.').ToList();
                            if (NVs.Count() == 3)
                            {
                                oCR = Get_ParameterObject(NVs[0] + "." + NVs[1]);
                                var sType = oCR.oResult.GetType().Name;
                                if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                {
                                    var oQSI = (XIIQS)oCR.oResult;
                                    if (oQSI.XIValues.ContainsKey(NVs[2]))
                                    {
                                        sValue = oQSI.XIIValues(NVs[2]);
                                    }
                                }
                                else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                {
                                    var BOI = (XIIBO)oCR.oResult;
                                    if (BOI.Attributes.ContainsKey(NVs[2]))
                                    {
                                        sValue = BOI.Attributes[NVs[2]].sValue;
                                    }
                                }
                                else
                                {
                                    sValue = (string)oCR.oResult;
                                }
                            }
                            else
                            {
                                oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    sValue = (string)oCR.oResult;
                                }
                            }
                            if (!string.IsNullOrEmpty(sName) && !string.IsNullOrEmpty(sValue))
                            {
                                oParams = new List<CNV>();
                                oParams.Add(new CNV() { sName = "sGUID", sValue = sGUID });
                                oParams.Add(new CNV() { sName = "sParamName", sValue = sName });
                                oParams.Add(new CNV() { sName = "sParamValue", sValue = sValue });
                                oParams.Add(new CNV() { sName = "sSessionID", sValue = sSessionID });

                                oCR = oXIAPI.Set_Parameter(sName, sValue, sGUID, sSessionID);
                            }


                            if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                            {
                                sMyMethodTrace = sMyMethodTrace + "To \'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' Value \'"
                                            + sValue + "\' Set to Cache successfully" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + "To \'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' Value \'"
                                            + sValue + "\' Set to Cache FAILED" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                            oMethodResult = oCR.xiStatus;
                        }


                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiGetCache:
                        // xi.m|s:--|i:SetValue1|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[dDue=xim.EachDiary.iDaysFromZero+p.dGoLive]
                        Params = new List<CNodeItem>();
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count() == 1)
                        {
                            string sValue = string.Empty;
                            var sName = Params[0].sValue;
                            if (!string.IsNullOrEmpty(sName))
                            {
                                oParams = new List<CNV>();
                                oParams.Add(new CNV() { sName = "sGUID", sValue = sGUID });
                                oParams.Add(new CNV() { sName = "sParamName", sValue = sName });
                                oParams.Add(new CNV() { sName = "sSessionID", sValue = sSessionID });

                                oCR = oXIAPI.Parameter(sName, sGUID, sSessionID);
                                oSValue1 = (string)oCR.oResult;
                            }


                            if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                            {
                                sMyMethodTrace = sMyMethodTrace + "To \'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' Value \'"
                                            + sValue + "\' Get from Cache successful" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + "To \'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' Value \'"
                                            + sValue + "\' Get from Cache FAILED" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                            oMethodResult = oSValue1;
                        }


                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiSetArray:
                        Params = new List<CNodeItem>();
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params != null && Params.Count() > 0)
                        {
                            if (Params.Count() == 2)
                            {
                                oSValue1 = string.Empty;
                                oSValue2 = string.Empty;
                                var NV = ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue;
                                var NVs = NV.Split('.').ToList();
                                if (NVs.Count() == 3)
                                {
                                    oCR = Get_ParameterObject(NVs[0] + "." + NVs[1]);
                                    var sType = oCR.oResult.GetType().Name;
                                    if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                    {
                                        var oQSI = (XIIQS)oCR.oResult;
                                        if (oQSI.XIValues.ContainsKey(NVs[2]))
                                        {
                                            oSValue1 = oQSI.XIIValues(NVs[2]);
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                    {
                                        var BOI = (XIIBO)oCR.oResult;
                                        if (BOI.Attributes.ContainsKey(NVs[2]))
                                        {
                                            oSValue1 = BOI.Attributes[NVs[2]].sValue;
                                        }
                                    }

                                }

                                var NV1 = ((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue;
                                var NVs1 = NV1.Split('.').ToList();
                                if (NVs1.Count() == 3)
                                {
                                    oCR = Get_ParameterObject(NVs1[0] + "." + NVs1[1]);
                                    var sType = oCR.oResult.GetType().Name;
                                    if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                    {
                                        var oQSI = (XIIQS)oCR.oResult;
                                        if (oQSI.XIValues.ContainsKey(NVs1[2]))
                                        {
                                            oSValue2 = oQSI.XIIValues(NVs1[2]);
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                    {
                                        var BOI = (XIIBO)oCR.oResult;
                                        if (BOI.Attributes.ContainsKey(NVs1[2]))
                                        {
                                            oSValue2 = BOI.Attributes[NVs1[2]].sValue;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(oSValue1) && !string.IsNullOrEmpty(oSValue2))
                                {
                                    AlgoDict[oSValue1] = oSValue2;
                                    oMethodResult = AlgoDict;
                                }
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiGetArray:
                        var oAlgoDict = new Dictionary<string, string>();
                        Params = new List<CNodeItem>();
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params != null && Params.Count() > 0)
                        {
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                            oAlgoDict = (Dictionary<string, string>)oCR.oResult;
                            var NV = ((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue;
                            var NVs = NV.Split('.').ToList();
                            if (NVs.Count() == 3)
                            {
                                oCR = Get_ParameterObject(NVs[0] + "." + NVs[1]);
                                var sType = oCR.oResult.GetType().Name;
                                if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                {
                                    var oQSI = (XIIQS)oCR.oResult;
                                    if (oQSI.XIValues.ContainsKey(NVs[2]))
                                    {
                                        oSValue1 = oQSI.XIIValues(NVs[2]);
                                    }
                                }
                                else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                {
                                    var BOI = (XIIBO)oCR.oResult;
                                    if (BOI.Attributes.ContainsKey(NVs[2]))
                                    {
                                        oSValue1 = BOI.Attributes[NVs[2]].sValue;
                                    }
                                }
                            }
                            if (oAlgoDict != null && oAlgoDict.ContainsKey(oSValue1))
                            {
                                oMethodResult = oAlgoDict[oSValue1];
                            }
                            else
                            {
                                oMethodResult = "0";
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiFunction:
                        Params = new List<CNodeItem>();
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        var Pram1 = Params[0].sValue;
                        var Pram2 = Params[1].sValue;
                        var Prms = Pram2.Split('.').ToList();
                        if ((Params.Count() == 3 || Params.Count() == 2) && Params[0].sValue.ToLower() != "count")
                        {
                            var PName = Prms[0] + '.' + Prms[1];
                            oCR = Get_ParameterObject(PName);
                            var sType = oCR.oResult.GetType().Name;
                            if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                            {
                                var oQSI = (XIIQS)oCR.oResult;
                                oSValue1 = oQSI.XIIValues(Prms[2]);
                            }
                            else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                            {
                                var oBOI = (XIIBO)oCR.oResult;
                                if (oBOI != null && oBOI.Attributes.Count() > 0)
                                {
                                    if (oBOI.Attributes.ContainsKey(Prms[2]))
                                    {
                                        oSValue1 = oBOI.Attributes[Prms[2]].sValue;
                                    }
                                }
                            }
                            else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "string")
                            {
                                oSValue1 = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue).oResult.ToString();
                            }
                            var Pram3 = Params.Count() > 2 ? Params[2].sValue : "";
                            if (Params.Count() > 2)
                                Prms = Params[2].sValue.Split('.').ToList();
                            if (Prms.Count() == 3 || Prms.Count() == 2)
                            {
                                PName = Prms[0] + '.' + Prms[1];
                                oCR = Get_ParameterObject(PName);
                                sType = oCR.oResult.GetType().Name;
                                if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                {
                                    var oQSI = (XIIQS)oCR.oResult;
                                    Pram3 = oQSI.XIIValues(Prms[2]);
                                }
                                else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                {
                                    var oBOI = (XIIBO)oCR.oResult;
                                    if (oBOI != null && oBOI.Attributes.Count() > 0)
                                    {
                                        if (oBOI.Attributes.ContainsKey(Prms[2]))
                                        {
                                            Pram3 = oBOI.Attributes[Prms[2]].sValue;
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "string")
                                {
                                    Pram3 = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue).oResult.ToString();
                                }
                            }
                            oParams = new List<CNV>();
                            if (Pram1.ToLower() == "age")
                            {
                                oParams.Add(new CNV() { sName = "xifunction", sValue = Pram1 });
                                oParams.Add(new CNV() { sName = "fromdate", sValue = oSValue1 });
                                oParams.Add(new CNV() { sName = "todate", sValue = Pram3 });
                            }
                            else
                            {
                                oParams.Add(new CNV() { sName = "xifunction", sValue = Pram1 });
                                oParams.Add(new CNV() { sName = "inputdate", sValue = oSValue1 });
                                oParams.Add(new CNV() { sName = "format", sValue = Pram3 });
                                oParams.Add(new CNV() { sName = "days", sValue = Pram3 });
                            }
                            oCR = oXIAPI.XIfunction(oParams);
                            if (oCR.oResult == null && (Prms!=null && Prms.Count>0))
                            {
                                oParams = new List<CNV>();
                                oParams.Add(new CNV() { sName = "xifunction", sValue = Pram1 });
                                oParams.Add(new CNV() { sName = "inputdate", sValue = oSValue1 });
                                oParams.Add(new CNV() { sName = "format", sValue = Pram3 });
                                oParams.Add(new CNV() { sName = "days", sValue = Prms[1] });
                                oCR = oXIAPI.XIfunction(oParams);
                            }
                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                            {
                                // assign result to the instance in the OM
                                sMyMethodTrace = sMyMethodTrace + " \'" + oSValue1 + "\' converted to \'" + Pram3 + "\' format Successfully" + "\r\n";
                                oMethodResult = oCR.oResult;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " \'" + oSValue1 + "\' format convertion failed \'" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue);
                            var sType = oCR.oResult.GetType().Name;
                            if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "BOInstanceGridPlaceholder".ToLower())
                            {
                                var Result = (BOInstanceGridPlaceholder)oCR.oResult;
                                if (Pram1.ToLower() == "count")
                                {
                                    if (Result.oBOInstances != null && Result.oBOInstances.Count() > 0)
                                    {
                                        oMethodResult = Result.oBOInstances.Count();
                                        sMyMethodTrace = sMyMethodTrace + " Count of \'" + Result.oBOInstances.Count() + "\' returned Successfully" + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        oMethodResult = 0;
                                        sMyMethodTrace = sMyMethodTrace + " Count of 0 returned Successfully" + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                }
                            }
                        }


                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xi1Click:
                    case CMethodStepDefinition.xiAlgoActionType.xi1ClickCount:
                        // xi.m|s:-|i:TemplateDiaries|r:BOInstanceGrid|e:DBLog,xiFail|a:m.1Click|x:templatediaries
                        oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                        oSValue1 = oCR.oResult.ToString();
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        WhrParams = string.Empty;
                        oCache1 = new XIInfraCache();
                        oMergeParms = new List<CNV>();
                        if (Params.Count() > 1)
                        {
                            for (int i = 0; i < Params.Count(); i++)
                            {
                                if (i > 0)
                                {
                                    var NV = Params[i].sValue;
                                    var sAttr = NV.Replace("[", "").Replace("]", "").Split('=').ToList()[0];
                                    var sParam = NV.Replace("[", "").Replace("]", "").Split('=').ToList()[1];
                                    var NVs = sParam.Split('.');
                                    sParam = NVs[0] + '.' + NVs[1];
                                    oCR = Get_ParameterObject(sParam, sSessionID, sGUID);
                                    var sType = oCR.oResult.GetType().Name;
                                    if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                    {
                                        var oQSI = (XIIQS)oCR.oResult;
                                        oSValue2 = oQSI.XIIValues(NVs[2]);
                                        WhrParams += sAttr + "='" + oSValue2 + "' and ";
                                    }
                                    else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                    {
                                        var oBOI = (XIIBO)oCR.oResult;
                                        oSValue2 = oBOI.AttributeI(NVs[2]).sValue;
                                        WhrParams += sAttr + "='" + oSValue2 + "' and ";
                                    }
                                    else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "string")
                                    {
                                        oSValue2 = (string)oCR.oResult;
                                        WhrParams += sAttr + "='" + oSValue2 + "' and ";
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(WhrParams))
                            {
                                WhrParams = WhrParams.Substring(0, WhrParams.Length - 4);
                            }
                        }
                        oCR = oPlaceholder.BO1Click(oSValue1, WhrParams, sSessionID, sGUID);
                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                        {
                            // assign result to the instance in the OM
                            if (iAction == CMethodStepDefinition.xiAlgoActionType.xi1Click)
                            {
                                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' 1-Click Run Successfully" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' 1-Click Run FAILED" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else if (iAction == CMethodStepDefinition.xiAlgoActionType.xi1ClickCount)
                            {
                                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                {
                                    var Result = (BOInstanceGridPlaceholder)oCR.oResult;
                                    if (Result.oBOInstances != null && Result.oBOInstances.Count() > 0)
                                    {
                                        oMethodResult = Result.oBOInstances.Count();
                                    }
                                    else
                                    {
                                        oMethodResult = "0";
                                    }
                                    sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' 1-Click Count of " + Result.oBOInstances.Count() + " Returned Successfully" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' 1-Click Count FAILED" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xi1ClickRun:
                        // xi.m|s:-|i:TemplateDiaries|r:BOInstanceGrid|e:DBLog,xiFail|a:m.1Click|x:templatediaries
                        oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                        oSValue1 = oCR.oResult.ToString();
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        WhrParams = string.Empty;
                        oCache1 = new XIInfraCache();
                        oMergeParms = new List<CNV>();
                        if (Params.Count() > 0)
                        {
                            for (int i = 0; i < Params.Count(); i++)
                            {
                                if (i > 0)
                                {
                                    var NV = Params[i].sValue;
                                    if (NV.Contains("="))
                                    {
                                        var sAttr = NV.Replace("[", "").Replace("]", "").Split('=').ToList()[0];
                                        var sParam = NV.Replace("[", "").Replace("]", "").Split('=').ToList()[1];
                                        var NVs = sParam.Split('.');
                                        sParam = NVs[0] + '.' + NVs[1];
                                        oCR = Get_ParameterObject(sParam, sSessionID, sGUID);
                                        var sType = oCR.oResult.GetType().Name;
                                        if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                        {
                                            var oQSI = (XIIQS)oCR.oResult;
                                            oSValue2 = oQSI.XIIValues(NVs[2]);
                                            WhrParams += sAttr + "='" + oSValue2 + "' and ";
                                        }
                                        else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                        {
                                            var oBOI = (XIIBO)oCR.oResult;
                                            oSValue2 = oBOI.AttributeI(NVs[2]).sValue;
                                            WhrParams += sAttr + "='" + oSValue2 + "' and ";
                                        }
                                        else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "string")
                                        {
                                            oSValue2 = (string)oCR.oResult;
                                            WhrParams += sAttr + "='" + oSValue2 + "' and ";
                                        }
                                    }
                                    else
                                    {
                                        if (NV.StartsWith("{XIP"))
                                        {
                                            var sParam = NV.Replace("[", "").Replace("]", "");
                                            var oValue = oCache1.Get_ParamVal(sSessionID, sGUID, null, sParam);
                                            oMergeParms.Add(new CNV { sName = sParam, sValue = oValue });
                                        }
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(WhrParams))
                            {
                                WhrParams = WhrParams.Substring(0, WhrParams.Length - 4);
                            }
                        }
                        oCR = oPlaceholder.BO1ClickRun(oSValue1, WhrParams, sSessionID, sGUID);
                        if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                        {
                            // assign result to the instance in the OM
                            if (iAction == CMethodStepDefinition.xiAlgoActionType.xi1Click || iAction == CMethodStepDefinition.xiAlgoActionType.xi1ClickRun)
                            {
                                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' 1-Click Run Successfully" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' 1-Click Run FAILED" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else if (iAction == CMethodStepDefinition.xiAlgoActionType.xi1ClickCount)
                            {
                                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                {
                                    var Result = (BOInstanceGridPlaceholder)oCR.oResult;
                                    if (Result.oBOInstances != null && Result.oBOInstances.Count() > 0)
                                    {
                                        oMethodResult = Result.oBOInstances.Count();
                                    }
                                    else
                                    {
                                        oMethodResult = "0";
                                    }
                                    sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' 1-Click Count of " + Result.oBOInstances.Count() + " Returned Successfully" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + "\'" + ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue + "\' 1-Click Count FAILED" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiRunScript:
                        int iScriptID = 0;
                        Guid gScriptIDXIGUID = Guid.Empty;
                        int iQSIID = 0;
                        var ScriptID = string.Empty;
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count() == 2)
                        {
                            var NV = ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue;
                            var NVs = NV.Split('.').ToList();
                            if (NVs.Count() == 3)
                            {
                                oCR = Get_ParameterObject(NVs[0] + "." + NVs[1]);
                                var sType = oCR.oResult.GetType().Name;
                                if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                {
                                    var oQSI = (XIIQS)oCR.oResult;
                                    if (oQSI.XIValues.ContainsKey(NVs[2]))
                                    {
                                        oSValue1 = oQSI.XIIValues(NVs[2]);
                                    }
                                }
                                else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                {
                                    var BOI = (XIIBO)oCR.oResult;
                                    if (BOI.Attributes.ContainsKey(NVs[2]))
                                    {
                                        oSValue1 = BOI.Attributes[NVs[2]].sValue;
                                    }
                                }
                                ScriptID = oSValue1;
                                int.TryParse(ScriptID, out iScriptID);
                                Guid.TryParse(ScriptID, out gScriptIDXIGUID);
                            }
                            var NV1 = ((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue;
                            var NVs1 = NV1.Split('.').ToList();
                            if (NVs1.Count() == 2)
                            {
                                oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    var QSIID = (string)oCR.oResult;
                                    int.TryParse(QSIID, out iQSIID);
                                }
                            }
                        }
                        if ((iScriptID > 0 || (gScriptIDXIGUID != Guid.Empty))  && iQSIID > 0)
                        {
                            XIInfraScript oScript = new XIInfraScript();
                            if (gScriptIDXIGUID != Guid.Empty)
                            {
                                oCR = oScript.XIScripting(gScriptIDXIGUID.ToString(), null, iQSIID.ToString(), 0, null, 0, null, null, null, null, null, 0, null);
                            }
                            else if (iScriptID > 0)
                            {
                                oCR = oScript.XIScripting(iScriptID.ToString(), null, iQSIID.ToString(), 0, null, 0, null, null, null, null, null, 0, null);
                            }
                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                            {
                                sMyMethodTrace = sMyMethodTrace + "Script " + ScriptID + " executed Successfully" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " Error occured while executing Script, ID is " + ScriptID + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            sMyMethodTrace = sMyMethodTrace + "\' ScriptID is " + ScriptID + " and QSIID is " + iQSIID + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;

                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiWriteLog:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count() >= 2)
                        {
                            string sMessage = string.Empty;
                            string iCriticality = "0";
                            iQSIID = 0;
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                sMessage = (string)oCR.oResult;
                            }
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                iCriticality = (string)oCR.oResult;
                            }
                            if (Params.Count() == 3)
                            {
                                oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[2]).sValue);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    var QSIID = (string)oCR.oResult;
                                    int.TryParse(QSIID, out iQSIID);
                                }
                            }
                            if (!string.IsNullOrEmpty(sMessage) && !string.IsNullOrEmpty(iCriticality) && iCriticality != "0")
                            {
                                XIInfraCache oCache = new XIInfraCache();
                                var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Errorlog");
                                XIIBO oBOI = new XIIBO();
                                oBOI.BOD = BOD;
                                oBOI.SetAttribute("Name", "sanction issue");
                                oBOI.SetAttribute("Description", sMessage);
                                oBOI.SetAttribute("sCategory", "regulation exception");
                                oBOI.SetAttribute("iCriticality", iCriticality);
                                oBOI.SetAttribute("FKiQSInstanceID", iQSIID.ToString());
                                oCR = oBOI.SaveV2(oBOI);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    sMyMethodTrace = sMyMethodTrace + " Log saved successfully" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " Log saving failed" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory params are not passed for Log saving " + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            sMyMethodTrace = sMyMethodTrace + " mandatory params are not passed for Log saving " + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiTemplateMerge:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count() == 4)
                        {
                            XIInfraCache oCache = new XIInfraCache();
                            XIContentEditors oTempD = new XIContentEditors();
                            string sObjectName = string.Empty;
                            string ObjUID = string.Empty;
                            string sStrCode = string.Empty;
                            for (int i = 0; i < Params.Count(); i++)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        int iTemplateID = 0;
                                        oSValue1 = oCR.oResult.ToString();
                                        if (oSValue1 != null)
                                        {
                                            var Template = new List<XIContentEditors>();
                                            int.TryParse(oSValue1, out iTemplateID);
                                            if (iTemplateID > 0)
                                            {
                                                Template = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, iTemplateID.ToString());
                                            }
                                            else
                                            {
                                                Template = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, oSValue1, "0");
                                            }
                                            if (Template != null && Template.Count() > 0)
                                            {
                                                oTempD = Template.FirstOrDefault();
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                            }
                                            else
                                            {
                                                sMyMethodTrace = sMyMethodTrace + " Template: \'" + oSValue1 + "\' loading failed \'" + "\r\n";
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        sMyMethodTrace = sMyMethodTrace + " Template: \'" + oSValue1 + "\' loaded but data loading is failed \'" + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                                else if (i == 1)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sStrCode = (string)oCR.oResult;
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        sMyMethodTrace = sMyMethodTrace + " Template: \'" + oSValue1 + "\' loaded but object name not loaded \'" + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                                else if (i == 2)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sObjectName = (string)oCR.oResult;
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        sMyMethodTrace = sMyMethodTrace + " Template: \'" + oSValue1 + "\' loaded but object name not loaded \'" + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                                else if (i == 3)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        ObjUID = (string)oCR.oResult;
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        sMyMethodTrace = sMyMethodTrace + " Template: \'" + oSValue1 + "\' loaded but object name not loaded \'" + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                            }
                            if (Params.Count() == 4)
                            {
                                if (!string.IsNullOrEmpty(sObjectName) && !string.IsNullOrEmpty(ObjUID) && !string.IsNullOrEmpty(sStrCode))
                                {
                                    XIIXI oXI = new XIIXI();
                                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sObjectName);
                                    var oStructobj = oXI.BOI(oBOD.Name, ObjUID).Structure(sStrCode).XILoad(null, true,"",null, sSessionID, sGUID);
                                    //var BOI = (XIIBO)oCR.oResult;
                                    //XIBOInstance oBOInstance = new XIBOInstance();
                                    //List<XIIBO> ListXIIBO = new List<XIIBO>();
                                    //Dictionary<string, List<XIIBO>> oStructure = new Dictionary<string, List<XIIBO>>();
                                    //ListXIIBO.Add(BOI);
                                    //oStructure[sObjectName] = ListXIIBO;
                                    //oBOInstance.oStructureInstance = oStructure;
                                    XIContentEditors oTemp = new XIContentEditors();
                                    oCR = oTemp.MergeContentTemplate(oTempD, oStructobj);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        // assign result to the instance in the OM
                                        sMyMethodTrace = sMyMethodTrace + " Template: \'" + oSValue1 + "\' loaded and merged successfully \'" + "\r\n";
                                        oMethodResult = oCR.oResult;
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        sMyMethodTrace = sMyMethodTrace + " Template: \'" + oSValue1 + "\' loaded but data not merged successfully \'" + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " Template: \'" + oSValue1 + "\' loaded but data loading is failed \'" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else if (Params.Count() == 3)
                            {
                                //oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);

                            }
                        }
                        else
                        {
                            sMyMethodTrace = sMyMethodTrace + " Required parameters are not passed for Template Merge action \'" + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiQSInsert:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count() == 1)
                        {
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                            {
                                oSValue1 = oCR.oResult.ToString();
                                List<CNV> oNVParams = new List<CNV>();
                                oNVParams.Add(new CNV { sName = XIConstant.Param_QSName, sValue = oSValue1 });
                                oCR = oPlaceholder.QSInsert(oNVParams);
                                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                {
                                    sMyMethodTrace = sMyMethodTrace + " QS: \'" + oSValue1 + "\' instance saved successfully \'" + "\r\n";
                                    oMethodResult = oCR.oResult;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " QS: \'" + oSValue1 + "\' instance saving failed \'" + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " Parameter QS Name: not resolved \'" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }


                        }
                        else
                        {
                            sMyMethodTrace = sMyMethodTrace + " Input params not passed correctly \'" + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiDistributeBO:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count() == 2)
                        {
                            string sInstanceID = string.Empty;
                            string DistributionDefID = string.Empty;
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                            //oCR= Get_ParameterObject(sParam);
                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                            {
                                sInstanceID = oCR.oResult.ToString();
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " InstanceID for Ditribution is not resolved \'" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue);
                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                            {
                                DistributionDefID = oCR.oResult.ToString();
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " Distribution Definition ID is not resolved \'" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                            if (!string.IsNullOrEmpty(sInstanceID) && !string.IsNullOrEmpty(DistributionDefID))
                            {
                                XIInfraCache oCache = new XIInfraCache();
                                var DistributionDef = (XIDDistribute)oCache.GetObjectFromCache(XIConstant.CacheDistribution, "", DistributionDefID);
                                XIInfraDistributionComponent oDistributionComponent = new XIInfraDistributionComponent();
                                List<CNV> oCNVParams = new List<CNV>();
                                oCNVParams.Add(new CNV { sName = "iInstanceID", sValue = sInstanceID });
                                oCNVParams.Add(new CNV { sName = "iDistributionDefID", sValue = DistributionDefID });
                                oCR = oDistributionComponent.XILoad(oCNVParams);
                                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + " distribution executed successfully for instance:" + sInstanceID + " with distribution:" + DistributionDefID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " distribution execution failed for instance:" + sInstanceID + " with distribution:" + DistributionDefID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory params not passed for distribution execution - instance:" + sInstanceID + " and distribution:" + DistributionDefID + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                        }
                        else
                        {
                            sMyMethodTrace = sMyMethodTrace + " Input params not passed correctly \'" + "\r\n";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiCommunicate:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            string sInstanceID = string.Empty;
                            var NV = ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue;
                            var NVs = NV.Split('.').ToList();
                            if (NVs.Count() == 3)
                            {
                                var sValue = string.Empty;
                                oCR = Get_ParameterObject(NVs[0] + "." + NVs[1]);
                                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                {
                                    var sType = oCR.oResult.GetType().Name;
                                    if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                    {
                                        var oQSI = (XIIQS)oCR.oResult;
                                        if (oQSI.XIValues.ContainsKey(NVs[2]))
                                        {
                                            sInstanceID = oQSI.XIIValues(NVs[2]);
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                    {
                                        var BOI = (XIIBO)oCR.oResult;
                                        if (BOI.Attributes.ContainsKey(NVs[2]))
                                        {
                                            sInstanceID = BOI.Attributes[NVs[2]].sValue;
                                        }
                                    }
                                    else
                                    {
                                        sInstanceID = (string)oCR.oResult;
                                    }
                                    XIInfraCommunicationComponent oComm = new XIInfraCommunicationComponent();
                                    var oComParams = new List<CNV>();
                                    oComParams.Add(new CNV() { sName = XIConstant.Param_InstanceID, sValue = sInstanceID });
                                    oComParams.Add(new CNV() { sName = XIConstant.Param_SessionID, sValue = sSessionID });
                                    oComParams.Add(new CNV() { sName = XIConstant.Param_GUID, sValue = sGUID });
                                    oCR = oComm.Load(oComParams);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oMethodResult = oCR.oResult;
                                        sMyMethodTrace = sMyMethodTrace + " communication executed successfully for instance:" + sInstanceID + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        sMyMethodTrace = sMyMethodTrace + " communication execution failed for instance:" + sInstanceID + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                            }
                            else if (NVs.Count() == 2)
                            {
                                oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    sInstanceID = (string)oCR.oResult;
                                    XIInfraCommunicationComponent oComm = new XIInfraCommunicationComponent();
                                    var oComParams = new List<CNV>();
                                    oComParams.Add(new CNV() { sName = XIConstant.Param_InstanceID, sValue = sInstanceID });
                                    oComParams.Add(new CNV() { sName = XIConstant.Param_SessionID, sValue = sSessionID });
                                    oComParams.Add(new CNV() { sName = XIConstant.Param_GUID, sValue = sGUID });
                                    oCR = oComm.Load(oComParams);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oMethodResult = oCR.oResult;
                                        sMyMethodTrace = sMyMethodTrace + " communication executed successfully for instance:" + sInstanceID + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        sMyMethodTrace = sMyMethodTrace + " communication execution failed for instance:" + sInstanceID + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " InstanceID for Ditribution is not resolved \'" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiCommunicationout:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            string sInstanceID = string.Empty;
                            var NV = ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue;
                            var NVs = NV.Split('.').ToList();
                            if (NVs.Count() == 3 || NVs.Count() == 2)
                            {
                                var sValue = string.Empty;
                                oCR = Get_ParameterObject(NVs[0] + "." + NVs[1]);
                                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                {
                                    var sType = oCR.oResult.GetType().Name;
                                    if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                    {
                                        var oQSI = (XIIQS)oCR.oResult;
                                        if (oQSI.XIValues.ContainsKey(NVs[2]))
                                        {
                                            sInstanceID = oQSI.XIIValues(NVs[2]);
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                    {
                                        var BOI = (XIIBO)oCR.oResult;
                                        if (BOI.Attributes.ContainsKey(NVs[2]))
                                        {
                                            sInstanceID = BOI.Attributes[NVs[2]].sValue;
                                        }
                                    }
                                    else
                                    {
                                        sInstanceID = (string)oCR.oResult;
                                    }
                                    XIInfraCommunicationComponent oComm = new XIInfraCommunicationComponent();
                                    var oComParams = new List<CNV>();
                                    oComParams.Add(new CNV() { sName = XIConstant.Param_InstanceID, sValue = sInstanceID });
                                    oComParams.Add(new CNV() { sName = XIConstant.Param_SessionID, sValue = sSessionID });
                                    oComParams.Add(new CNV() { sName = XIConstant.Param_GUID, sValue = sGUID });
                                    oCR = oComm.CommunicationOut(oComParams);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oMethodResult = oCR.oResult;
                                        sMyMethodTrace = sMyMethodTrace + " communication executed successfully for instance:" + sInstanceID + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        sMyMethodTrace = sMyMethodTrace + " communication execution failed for instance:" + sInstanceID + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " InstanceID for Ditribution is not resolved \'" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiImportFile:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            string sInstanceID = string.Empty;
                            var NV = ((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue;
                            var NVs = NV.Split('.').ToList();
                            if (NVs.Count() == 2)
                            {
                                var sValue = string.Empty;
                                oCR = Get_ParameterObject(NVs[0] + "." + NVs[1]);
                                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                {
                                    var sType = oCR.oResult.GetType().Name;
                                    if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                    {
                                        var oQSI = (XIIQS)oCR.oResult;
                                        if (oQSI.XIValues.ContainsKey(NVs[2]))
                                        {
                                            sInstanceID = oQSI.XIIValues(NVs[2]);
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                    {
                                        var BOI = (XIIBO)oCR.oResult;
                                        if (BOI.Attributes.ContainsKey(NVs[2]))
                                        {
                                            sInstanceID = BOI.Attributes[NVs[2]].sValue;
                                        }
                                    }
                                    else
                                    {
                                        sInstanceID = (string)oCR.oResult;
                                    }
                                    XIInfraImportComponent oImport = new XIInfraImportComponent();
                                    var oImpParams = new List<CNV>();
                                    oImpParams.Add(new CNV() { sName = XIConstant.Param_InstanceID, sValue = sInstanceID });
                                    oImpParams.Add(new CNV() { sName = XIConstant.Param_SessionID, sValue = sSessionID });
                                    oImpParams.Add(new CNV() { sName = XIConstant.Param_GUID, sValue = sGUID });
                                    oCR = oImport.Import_File(oImpParams);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oMethodResult = oCR.oResult;
                                        sMyMethodTrace = sMyMethodTrace + " importing completed successfully for instance:" + sInstanceID + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        sMyMethodTrace = sMyMethodTrace + " importing failed for instance:" + sInstanceID + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " InstanceID for Ditribution is not resolved \'" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiCopyStructure:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            string sObject = string.Empty;
                            string sInstanceID = string.Empty;
                            string sURL = string.Empty;
                            string sStrCode = string.Empty;
                            string sParameter = string.Empty;
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                            {
                                sObject = (string)oCR.oResult;
                            }
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue);
                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                            {
                                sInstanceID = (string)oCR.oResult;
                            }
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[2]).sValue);
                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                            {
                                sStrCode = (string)oCR.oResult;
                            }
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[3]).sValue);
                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                            {
                                sURL = (string)oCR.oResult;
                            }
                            oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[4]).sValue);
                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                            {
                                sParameter = (string)oCR.oResult;
                            }
                            if (!string.IsNullOrEmpty(sObject) && !string.IsNullOrEmpty(sInstanceID) && !string.IsNullOrEmpty(sStrCode) && !string.IsNullOrEmpty(sURL))
                            {
                                var oCopyParams = new List<CNV>();
                                oCopyParams.Add(new CNV() { sName = "sObject", sValue = sObject });
                                oCopyParams.Add(new CNV() { sName = "sMethod", sValue = sURL });
                                oCopyParams.Add(new CNV() { sName = "sStructureName", sValue = sStrCode });
                                oCopyParams.Add(new CNV() { sName = XIConstant.Param_InstanceID, sValue = sInstanceID });
                                oCopyParams.Add(new CNV() { sName = "sParameter", sValue = sParameter });
                                oCopyParams.Add(new CNV() { sName = XIConstant.Param_SessionID, sValue = sSessionID });
                                oCopyParams.Add(new CNV() { sName = XIConstant.Param_GUID, sValue = sGUID });
                                oCR = oPlaceholder.Copy_Structure(oCopyParams);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + " copying completed successfully for instance:" + sInstanceID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " copying failed for instance:" + sInstanceID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory params not passed for copying structure:sObject :" + sObject + " sURL :" + sURL + " sStructureName :" + sStrCode + " Instance :" + sInstanceID + " failed \r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiCallOM:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            List<CNV> ReqParms = new List<CNV>();
                            string sURL = string.Empty;
                            int i = 0;
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[0]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sURL = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "sMethod", sValue = sURL });
                                    }
                                }
                                else
                                {
                                    var ParmName = ((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue;
                                    ParmName = ParmName.Replace("xim.", "").Replace("xif.", "");
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        var sResponse = (string)oCR.oResult;
                                        ReqParms.Add(new CNV { sName = ParmName, sValue = sResponse });
                                    }
                                }
                                i++;
                            }

                            if (!string.IsNullOrEmpty(sURL))
                            {
                                oCR = oPlaceholder.Call_OM(ReqParms);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + " Call_OM executed successfully for URL:" + sURL + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " Call_OM execution failed for URL:" + sURL + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory param URL is not passed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiFileImport:
                        //sIdentifier sDelimiter  sInboundFolder sFKInstanceGUID sFKDefinitionGUID sFKTargetGUID   sFKTargetGUID sMoveToFolder
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count == 3 || Params.Count == 4)
                        {
                            var NV = ((CNodeItem)oDefinition.oParamSet.NNodeItems[1]).sValue;
                            var NVs = NV.Split('.').ToList();
                            oCR = Get_ParameterObject(NVs[0] + "." + NVs[1]);
                            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                            {
                                var sInstanceID = string.Empty;
                                if (NVs.Count() == 3)
                                {
                                    var BOI = (XIIBO)oCR.oResult;
                                    if (BOI.Attributes.ContainsKey(NVs[2]))
                                    {
                                        sInstanceID = BOI.Attributes[NVs[2]].sValue;
                                    }
                                }
                                var sExtUID = string.Empty;
                                string sFrom = string.Empty;
                                oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[2]).sValue);
                                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                {
                                    sExtUID = (string)oCR.oResult;
                                }
                                if (Params.Count == 4)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[3]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sFrom = (string)oCR.oResult;
                                    }
                                }
                                XIInfraCache oCache = new XIInfraCache();
                                var sSourcePath = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "Attachment source path");
                                var sTargetPath = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "Attachment target path");
                                var sErrorPath = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "Attachment error path");
                                XIDValue oVal = new XIDValue();
                                List<CNV> oParms = new List<CNV>();
                                oParms.Add(new CNV { sName = "sSourcePath", sValue = sSourcePath });
                                oParms.Add(new CNV { sName = "sTargetPath", sValue = sTargetPath });
                                oParms.Add(new CNV { sName = "sCategory", sValue = "Inbound Documents" });
                                oParms.Add(new CNV { sName = "sDelim", sValue = "__" });
                                oParms.Add(new CNV { sName = "sSpecificReference", sValue = sExtUID });
                                oParms.Add(new CNV { sName = "sXMappingDefinition", sValue = "XAttachment" });
                                oParms.Add(new CNV { sName = "sParentFK", sValue = sInstanceID });
                                oParms.Add(new CNV { sName = "sParentFKAttr", sValue = "FKiCommunicationID" }); ;
                                oParms.Add(new CNV { sName = "sFrom", sValue = sFrom });
                                oParms.Add(new CNV { sName = "sErrorPath", sValue = sErrorPath });
                                oCR = oVal.ImportFiles(oParms);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + " Files attached successfully for instance:" + sInstanceID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " Files attaching failed for instance:" + sInstanceID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                        }
                        // oMethodResult = oCR.xiStatus
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiEmailAttach:
                        //sIdentifier sDelimiter  sInboundFolder sFKInstanceGUID sFKDefinitionGUID sFKTargetGUID   sFKTargetGUID sMoveToFolder

                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            List<CNV> ReqParms = new List<CNV>();
                            string sAttachementPath = System.Configuration.ConfigurationManager.AppSettings["AttachementPath"];
                            string sCategory = "Attachment";
                            string sDelim = "";
                            string sXMappingDefinition = "XAttachment";
                            string sParentFK = "";
                            string sParentKAttr = "";
                            int i = 0;
                            ReqParms.Add(new CNV() { sName = "sFolderPath", sValue = sAttachementPath });
                            ReqParms.Add(new CNV() { sName = "sCategory", sValue = sCategory });
                            ReqParms.Add(new CNV() { sName = "sXMappingDefinition", sValue = sXMappingDefinition });
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sDelim = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "sDelim", sValue = sDelim });
                                    }
                                }
                                else if (i == 1)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sParentFK = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "sParentFK", sValue = sParentFK });
                                    }
                                }
                                else if (i == 2)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sParentKAttr = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "sParentKAttr", sValue = sParentKAttr });
                                    }
                                }
                                i++;
                            }
                            ReqParms.Add(new CNV() { sName = "sMethod", sValue = "XIDNA/APIInvoke/SetAppToSGMessage" });
                            oCR = oPlaceholder.Call_OM(ReqParms);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oMethodResult = oCR.oResult;
                                sMyMethodTrace = sMyMethodTrace + " SetAppToSGMessage executed successfully" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " SetAppToSGMessage execution failed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        // oMethodResult = oCR.xiStatus
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiWebhookOutbound:
                        //Call third party URL
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            List<CNV> ReqParms = new List<CNV>();
                            string sIdentifier = string.Empty;
                            int i = 0;
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sIdentifier = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "sIdentifier", sValue = sIdentifier });
                                    }
                                }
                                else
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        oSValue1 = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "sParam" + i, sValue = oSValue1 });
                                    }
                                }
                                i++;
                            }
                            oCR = oPlaceholder.Call_OM(ReqParms);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oMethodResult = oCR.oResult;
                                sMyMethodTrace = sMyMethodTrace + " Call_OM executed successfully" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " Call_OM execution failed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        // oMethodResult = oCR.xiStatus
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiGetFKToObject:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            string sBO = string.Empty;
                            string sFKBO = string.Empty;
                            List<CNV> ReqParms = new List<CNV>();
                            string sIdentifier = string.Empty;
                            int i = 0;
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sBO = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "sBO", sValue = sBO });
                                    }
                                }
                                else
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sFKBO = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "sFKBO", sValue = sFKBO });
                                    }
                                }
                                i++;
                            }
                            if (!string.IsNullOrEmpty(sBO) && !string.IsNullOrEmpty(sFKBO))
                            {
                                oCR = oPlaceholder.Get_FKToObject(ReqParms);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + " FK attribute got successfully for BO:" + sBO + " and FKBO:" + sFKBO + " is " + oCR.oResult + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " FK attribute getting failed for BO:" + sBO + " and FKBO:" + sFKBO + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory param BO or FKBO are not passed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiRunDynamicScript:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            string iCommIID = string.Empty;
                            List<CNV> ReqParms = new List<CNV>();
                            int i = 0;
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        var iBODID = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "iBODID", sValue = iBODID });
                                    }
                                }
                                else if (i == 1)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        var iBOIID = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "iBOIID", sValue = iBOIID });
                                    }
                                }
                                else if (i == 2)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        var DynamicScriptID = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "scriptid", sValue = DynamicScriptID });
                                        iCommIID = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "commiid", sValue = iCommIID });
                                    }
                                }
                                i++;
                            }
                            XIDValue oVal = new XIDValue();
                            //oVal.DynamicDefinition(ReqParms, oSignalR);
                            XIInfraScript oScriptD = new XIInfraScript();
                            var Response = oScriptD.RunScript(ReqParms, oSignalR);
                            //TestScript.DynamicDefinition(ReqParms);
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiProcessCommunication:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            string iCommIID = string.Empty;
                            List<CNV> ReqParms = new List<CNV>();
                            int i = 0;
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        iCommIID = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "commiid", sValue = iCommIID });
                                    }
                                }
                                i++;
                            }
                            if (!string.IsNullOrEmpty(iCommIID))
                            {
                                XIInfraCommunicationComponent oComm = new XIInfraCommunicationComponent();
                                oCR = oComm.CommunicationStreamInbound(ReqParms, oSignalR);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + " Communication processed successfully for Communication ID:" + iCommIID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " Communication processing failed for Communication ID:" + iCommIID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory param communication instance is not passed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiMatchCommunication:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            string iCommIID = string.Empty;
                            List<CNV> ReqParms = new List<CNV>();
                            int i = 0;
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        iCommIID = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "iCommID", sValue = iCommIID });
                                    }
                                }
                                i++;
                            }
                            if (!string.IsNullOrEmpty(iCommIID))
                            {
                                XIInfraCommunicationComponent oComm = new XIInfraCommunicationComponent();
                                oCR = oComm.Match_Communication(ReqParms);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + " Communication processed successfully for Communication ID:" + iCommIID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " Communication processing failed for Communication ID:" + iCommIID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory param communication instance is not passed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiChildPC:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            string PCXIGUID = string.Empty;
                            List<CNV> ReqParms = new List<CNV>();
                            string sIdentifier = string.Empty;
                            int i = 0;
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        PCXIGUID = (string)oCR.oResult;
                                    }
                                }
                                else
                                {
                                    var ParmName = ((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue;
                                    ParmName = ParmName.Replace("xim.", "").Replace("xif.", "");
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        if (ParmName.Contains("."))
                                        {
                                            ParmName = ParmName.Split('.').ToList()[1];
                                        }
                                        oSValue1 = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = ParmName, sValue = oSValue1 });
                                    }
                                }
                                i++;
                            }
                            if (!string.IsNullOrEmpty(PCXIGUID))
                            {
                                XIInfraCache oCache = new XIInfraCache();
                                XIDAlgorithm oSubAlgoD = new XIDAlgorithm();
                                XIDAlgorithm oSubAlgoC = new XIDAlgorithm();
                                oSubAlgoD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, PCXIGUID.ToString());
                                oSubAlgoC = (XIDAlgorithm)oSubAlgoD.Clone(oSubAlgoD);
                                //oCR = oSubAlgoC.Execute_XIAlgorithm(sSessionID, sGUID, ReqParms);
                                // Create a new thread for child PC
                                Thread workerThread = new Thread(() =>
                                {
                                    oCR = oSubAlgoC.Execute_XIAlgorithm(sSessionID, sGUID, ReqParms);
                                });

                                // Start the worker thread
                                workerThread.Start();

                                // Wait for the worker thread to complete
                                workerThread.Join();
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + " Child pc executed successfully for PC:" + PCXIGUID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " Child pc execution failed for PC:" + PCXIGUID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory param pc guid is not passed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiExcelImport:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            string TargetImportID = string.Empty;
                            string iDocumentID = string.Empty;
                            List<CNV> ReqParms = new List<CNV>();
                            int i = 0;
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        TargetImportID = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "instanceid", sValue = TargetImportID });
                                    }
                                }
                                else if (i == 1)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        iDocumentID = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "iDocumentID", sValue = iDocumentID });
                                    }
                                }
                                i++;
                            }
                            if (!string.IsNullOrEmpty(TargetImportID))
                            {
                                XIDValue oVal = new XIDValue();
                                oCR = oVal.Import_TargetFromExcelV3(ReqParms);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + " Target import processed successfully for ID:" + TargetImportID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " Target import processing failed for ID:" + TargetImportID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory param Target Import instance is not passed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiImportLead:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            string LeadImportID = string.Empty;
                            string SelectedID = string.Empty;
                            List<CNV> ReqParms = new List<CNV>();
                            int i = 0;
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        LeadImportID = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "instanceid", sValue = LeadImportID });
                                    }
                                }
                                if (i == 1)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        SelectedID = (string)oCR.oResult;
                                        ReqParms.Add(new CNV() { sName = "SelectedID", sValue = SelectedID });
                                    }
                                }
                                i++;
                            }
                            if (!string.IsNullOrEmpty(LeadImportID))
                            {
                                XIDValue oVal = new XIDValue();
                                oCR = oVal.Import_TargetToObject(ReqParms);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + " Lead import processed successfully for ID:" + LeadImportID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " Lead import processing failed for ID:" + LeadImportID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory param communication instance is not passed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiVersionise:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            string BODXIGuid = string.Empty;
                            string BOIUID = string.Empty;
                            int i = 0;
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        BODXIGuid = (string)oCR.oResult;
                                    }
                                }
                                if (i == 1)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        BOIUID = (string)oCR.oResult;
                                    }
                                }
                                i++;
                            }
                            if (!string.IsNullOrEmpty(BODXIGuid) && !string.IsNullOrEmpty(BOIUID))
                            {
                                XIIBO oBOI = new XIIBO();
                                oCR = oBOI.Versionise_Instance(BODXIGuid, BOIUID);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oMethodResult = oCR.oResult;
                                    sMyMethodTrace = sMyMethodTrace + " Versionising successfully completed for BODXIGuid:" + BODXIGuid + ", BOIUID" + BOIUID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    sMyMethodTrace = sMyMethodTrace + " Versionising failed for BODXIGuid:" + BODXIGuid + ", BOIUID" + BOIUID + "\r\n";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory param communication instance is not passed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;

                    case CMethodStepDefinition.xiAlgoActionType.xiBOToXIValues:
                        // oMethodResult = oCR.xiStatus
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xixiValuesToBO:
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            XIIQS oQSI = new XIIQS();
                            string RelatedObject = string.Empty;
                            string s1Click = string.Empty;
                            string sCurrentStep = string.Empty;
                            string iBOIID = string.Empty;
                            int i = 0;
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        oQSI = (XIIQS)oCR.oResult;
                                    }
                                }
                                else if (i == 1)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        RelatedObject = (string)oCR.oResult;
                                    }
                                }
                                else if (i == 2)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        s1Click = (string)oCR.oResult;
                                    }
                                }
                                else if (i == 3)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sCurrentStep = (string)oCR.oResult;
                                    }
                                }
                                else if (i == 4)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        iBOIID = (string)oCR.oResult;
                                    }
                                }
                                i++;
                            }
                            if (!string.IsNullOrEmpty(sCurrentStep) && !string.IsNullOrEmpty(s1Click) && !string.IsNullOrEmpty(RelatedObject))
                            {
                                WhrParams = "FKiStepDefIDXIGUID='" + sCurrentStep + "'";
                                oCR = oPlaceholder.BO1Click(s1Click, WhrParams, sSessionID, sGUID);
                                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                {
                                    XIDBO oBOD = new XIDBO();
                                    oBOD = (XIDBO)oCache1.GetObjectFromCache(XIConstant.CacheBO, null, RelatedObject);
                                    XIIBO oBOI = new XIIBO();
                                    oBOI.BOD = oBOD;
                                    oBOI.SetAttribute(oBOD.sPrimaryKey, iBOIID);
                                    oDictionary = (BOInstanceGridPlaceholder)oCR.oResult;
                                    foreach (var oIterate in oDictionary.oBOInstances.Values.ToList())//.oBOInstances.values)
                                    {
                                        var sAttribute = oIterate.AttributeI("FKiAttributeIDXIGUID").sValue;
                                        var sFieldOrigin = oIterate.AttributeI("FKiFieldOriginIDXIGUID").sValue;
                                        var oAttrD = oBOD.Attributes.Values.ToList().Where(m => m.XIGUID.ToString().ToLower() == sAttribute.ToLower()).FirstOrDefault();
                                        var FieldOrigin = (XIDFieldOrigin)oCache1.GetObjectFromCache(XIConstant.CacheFieldOrigin, null, sFieldOrigin);
                                        if (oQSI.XIValues.ContainsKey(FieldOrigin.sName) && oAttrD != null)
                                        {
                                            var sXIValue = oQSI.XIValues[FieldOrigin.sName].sValue;
                                            oBOI.SetAttribute(oAttrD.Name, sXIValue);
                                        }
                                    }
                                    oCR = oBOI.Save(oBOI);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oMethodResult = oCR.oResult;
                                        sMyMethodTrace = sMyMethodTrace + " XIValues mapping success for Object:" + oBOD.Name + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        sMyMethodTrace = sMyMethodTrace + " XIValues mapping failed for Object:" + oBOD.Name + "\r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory param communication instance is not passed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        // oMethodResult = oCR.xiStatus
                        break;

                    case CMethodStepDefinition.xiAlgoActionType.xiSaveFile:
                        string sContent = string.Empty;
                        string sDocType = string.Empty;
                        string sDocName = "SampleDoc";
                        Params = oDefinition.oParamSet.NNodeItems.Values.Cast<CNodeItem>().ToList();
                        if (Params.Count > 0)
                        {
                            List<CNV> ReqParms = new List<CNV>();
                            string sURL = string.Empty;
                            int i = 0;
                            foreach (var param in Params)
                            {
                                if (i == 0)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sContent = (string)oCR.oResult;
                                    }
                                }
                                else if (i == 1)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sDocType = (string)oCR.oResult;
                                    }
                                }
                                else if (i == 2)
                                {
                                    oCR = Get_ParameterObject(((CNodeItem)oDefinition.oParamSet.NNodeItems[i]).sValue);
                                    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                                    {
                                        sDocName = (string)oCR.oResult;
                                    }
                                }
                                i++;
                            }

                            if (!string.IsNullOrEmpty(sContent) && !string.IsNullOrEmpty(sDocType))
                            {
                                XIInfraDocs oXIDocs = new XIInfraDocs();
                                if (sDocType.ToLower() == "txt")
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        StreamWriter writer = new StreamWriter(ms);
                                        writer.WriteLine(sContent);
                                        writer.Flush();
                                        //You have to rewind the MemoryStream before copying
                                        ms.Seek(0, SeekOrigin.Begin);
                                        var oResponse = oXIDocs.SaveDocuments(ms, sDocName);
                                        if (oResponse.bOK && oResponse.oResult != null)
                                        {
                                            sMyMethodTrace = sMyMethodTrace + " Document generated successfully \r\n";
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                            sWhrCond = new List<CNV>();
                                            sWhrCond.Add(new CNV { sName = "id", sValue = oResponse.oResult.ToString() });
                                            oCR = oPlaceholder.BOLoad("Documents_T", sWhrCond);
                                            oMethodResult = oCR.oResult;
                                        }
                                        else
                                        {
                                            sMyMethodTrace = sMyMethodTrace + " Document generation failed \r\n";
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        }
                                    }
                                }
                                else if (sDocType.ToLower() == "pdf")
                                {
                                    XIInfraEmail oEmail = new XIInfraEmail();
                                    //oCR = oEmail.PDFGenerate(sContent, false, "", "");
                                    oCR = oEmail.IronPdf(sContent, "", false, "", "");
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oEmail.sDocumentName = sDocName;
                                        var oAttachment = oEmail.GeneratePDFFile((MemoryStream)oCR.oResult);
                                        if (oAttachment.bOK && oAttachment.oResult != null)
                                        {
                                            Attachment data = (Attachment)oAttachment.oResult;
                                            MemoryStream file = (MemoryStream)oCR.oResult;
                                            var oResponse = oXIDocs.SaveDocuments(file, sDocName);//save documents to folder
                                            if (oResponse.bOK && oResponse.oResult != null)
                                            {
                                                oMethodResult = oResponse.xiStatus;
                                                sMyMethodTrace = sMyMethodTrace + " Document generated successfully \r\n";
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                            }
                                            else
                                            {
                                                sMyMethodTrace = sMyMethodTrace + " Document generation failed \r\n";
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            }
                                        }
                                        else
                                        {
                                            sMyMethodTrace = sMyMethodTrace + " Document generation failed \r\n";
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        }
                                    }
                                    else
                                    {
                                        sMyMethodTrace = sMyMethodTrace + " Document generation failed \r\n";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                            }
                            else
                            {
                                sMyMethodTrace = sMyMethodTrace + " mandatory param sContent is not passed" + "\r\n";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        break;
                }
            }
            else if (oDefinition.iMethodType == CMethodStepDefinition.xiMethodType.xiScript)
            {
                // TO DO - REPLACE THIS WITH A MORE GLOBAL API AND ALSO THE PLACEHOLDER - MERGE INTO THIS API
                CXIAPI oCAPI = new CXIAPI();

                oCR = Script_Execute(sKey, oCAPI, oAlgoI, sSessionID, sGUID);
                if (oCR.xiStatus != xiEnumSystem.xiFuncResult.xiSuccess)
                {
                    // TO DO - handle the error - to log? Check error method and do what it says (abandon or whatever)
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.sMessage = oCR.sMessage;
                }

                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
                {
                    sMyMethodTrace = sMyMethodTrace + " Script returned \'" + oCR.oResult + "\'" + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    sMyMethodTrace = sMyMethodTrace + "Script FAILED: \'" + oCR.sMessage + "\'" + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
            }
            watch.Stop();
            var iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            Guid DefXIGUID = Guid.Empty;
            Guid.TryParse(FKiPCDID, out DefXIGUID);
            oDefBase.Cache_Performance(sKey, iLapsedTime, DefXIGUID, 50);
            return oCResult;
        }

        public CResult Get_ParameterName(string sParameter)
        {
            CResult oCResult = new CResult();
            CResult oCR = null;
            string sParam = sParameter;
            if (sParam.Contains('['))
            {
                sParam = sParam.Replace("[", "");
            }
            if (sParam.Contains(']'))
            {
                sParam = sParam.Replace("]", "");
            }
            if (sParam.Contains('='))
            {
                sParam = sParam.Split('=')[1];
            }
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            oCResult.oResult = sParam;
            return oCResult;
        }

        public CResult Get_ParameterObject(string sParameterValue, string sSessionID = "", string sGUID = "")
        {
            CResult oCResult = new CResult();
            CResult oCR = null;
            string[] AsParam;
            string sValue = "";
            object oValue = null;
            string sPrefix = "";
            string sParamIdentifier = "";
            CMethodStepInstance oStepResult = null;
            CAlgorithmDefinition oAlgoD = null;
            if (!sParameterValue.Contains('/'))
                sParameterValue = sParameterValue.ToLower();
            AsParam = sParameterValue.Split('.');
            if (AsParam.Length == 0)
            {
                // just the value
                sValue = sParameterValue;
                oCResult.oResult = sValue;
            }
            else if (AsParam.Length > 0)
            {
                sPrefix = AsParam[0];
                if (sPrefix.StartsWith("x:"))
                {
                    sPrefix = "x:";
                }
                else
                {
                    sParamIdentifier = sParameterValue.Substring((sPrefix.Length + 1), (sParameterValue.Length - sPrefix.Length - 1));
                }

                switch (sPrefix.ToLower())
                {
                    case "xif":
                        sValue = sParamIdentifier;
                        if (!string.IsNullOrEmpty(sValue) && sValue.ToLower() == "null")
                        {
                            sValue = null;
                        }
                        else if (!string.IsNullOrEmpty(sValue) && sValue.ToLower() == "TodayDate".ToLower())
                        {
                            oCResult.oResult = DateTime.Now.ToString();
                        }
                        else
                        {
                            oCResult.oResult = sValue;
                        }
                        break;
                    case "xiv":
                        sValue = sParamIdentifier;
                        oCResult.oResult = sValue;
                        break;
                    case "bod":
                        sValue = sParamIdentifier;
                        if (!string.IsNullOrEmpty(sValue) && sValue.ToLower().StartsWith("xim."))
                        {
                            var Value = sValue.Replace("xim.", "");
                            if (!string.IsNullOrEmpty(Value))
                            {
                                oStepResult = oAlgoI.oSteps[Value];
                                oCResult.oResult = oStepResult.oMethodResult;
                            }
                        }
                        else
                        {
                            oCResult.oResult = sValue;
                        }
                        break;
                    case "p":
                        List<CNodeItem> oRes = new List<CNodeItem>();
                        foreach (var item in oAlgoI.oXIParameters.NNodeItems.Values)
                        {
                            oRes.Add((CNodeItem)item);
                        }
                        oCResult.oResult = oRes.Where(x => x.sKey.Contains(sParameterValue)).Select(t => t.sValue).FirstOrDefault();
                        break;
                    case "xim":
                    case "xis":
                        if (sParamIdentifier.Contains('.'))
                        {
                            var sParamName = sParamIdentifier.Split('.')[1];
                            sParamIdentifier = sParamIdentifier.Split('.')[0];
                            if (oAlgoI.oSteps.ContainsKey(sParamIdentifier))
                            {
                                oStepResult = oAlgoI.oSteps[sParamIdentifier];
                                if (oStepResult.oMethodResult == null)
                                {
                                    if (sKey == sParamIdentifier)
                                    {
                                        oCResult.oResult = "0";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                                    }

                                    // EXIT the whole thing? Maybe there are circumstances when we should not
                                    // NO! in my case i am looking up the value in an iteration of the current line, which to start with is null, but i want to count that as zero
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(sParamName))
                                    {

                                    }
                                    var oSValue1 = string.Empty;
                                    var sType = oStepResult.oMethodResult.GetType().Name;
                                    if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiiqs")
                                    {
                                        var oQSI = (XIIQS)oStepResult.oMethodResult;
                                        if (oQSI.XIValues.ContainsKey(sParamName))
                                        {
                                            oSValue1 = oQSI.XIIValues(sParamName);
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(sType) && sType.ToLower() == "xiibo")
                                    {
                                        var BOI = (XIIBO)oStepResult.oMethodResult;
                                        if (BOI.Attributes.ContainsKey(sParamName))
                                        {
                                            oSValue1 = BOI.Attributes[sParamName].sValue;
                                        }
                                    }
                                    oCResult.oResult = oSValue1;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                            }
                        }
                        else
                        {
                            if (oAlgoI.oSteps.ContainsKey(sParamIdentifier))
                            {
                                oStepResult = oAlgoI.oSteps[sParamIdentifier];
                                if (oStepResult.oMethodResult == null)
                                {
                                    if (sKey == sParamIdentifier)
                                    {
                                        oCResult.oResult = "0";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                                    }

                                    // EXIT the whole thing? Maybe there are circumstances when we should not
                                    // NO! in my case i am looking up the value in an iteration of the current line, which to start with is null, but i want to count that as zero
                                }
                                else
                                {
                                    oCResult.oResult = oStepResult.oMethodResult;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                            }
                        }


                        break;
                    case "xic":
                        if (oAlgoI.oSteps.ContainsKey(sParamIdentifier))
                        {
                            oStepResult = oAlgoI.oSteps[sParamIdentifier];
                            if (oStepResult.oMethodResult == null)
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                                // EXIT the whole thing? Maybe there are circumstances when we should not
                            }
                            else
                            {
                                oCResult.oResult = oStepResult.oMethodResult;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                        }

                        break;
                    case "x:":
                        sParameterValue = sParameterValue.Replace("x:", "");
                        oCR = Script_Exe(sParameterValue, sSessionID, sGUID);
                        oCResult.oResult = oCR.oResult;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        break;
                        if (sParameterValue.Substring(0, 1) == "[")
                        {
                            // this is some kind of method/script etc, so just return it
                            sValue = sParameterValue;
                            oCResult.oResult = sValue;
                        }
                        break;
                    case "xid":
                        if (!string.IsNullOrEmpty(sParamIdentifier))
                        {
                            if (sParamIdentifier.ToLower() == "today")
                            {
                                oCResult.oResult = DateTime.Now.ToString();
                            }
                        }
                        break;
                }
            }
            return oCResult;
        }

        public CResult Script_Execute(string sKeyEx, CXIAPI oCXIAPI, CAlgorithmInstance oAlgoI, string sSessionID, string sGUID)
        {
            CResult oCResult = new CResult();
            CResult oCR = null;
            string sScript = "";
            CScriptController oXIScript = null;
            if (oDefinition.iMethodType == CMethodStepDefinition.xiMethodType.xiScript)
            {
                oXIScript = oAlgoI.oScriptController;
                sScript = oDefinition.sScript;
                oCR = oXIScript.API2_Serialise_From_String(sScript);
                oCR = oXIScript.API2_ExecuteMyOM(sKeyEx, oCXIAPI, oAlgoI, sGUID, sSessionID);
                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.xiStatus = oCR.xiStatus;
                }
                else
                {
                    oCResult.xiStatus = oCR.xiStatus;
                    oCResult.oResult = oCR.oResult;
                    oMethodResult = oCR.oResult;
                }
            }
            else
            {
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                oCResult.sMessage = "\'" + oDefinition.sStepName + "\' is not a script";
            }

            // oCR = oXIScript.API2_Serialise_From_String(txtScript.Text)
            // txtCodeFormatted.Text = oXIScript.API_FormattedFunction.oResult
            // txtParsedFunction.Text = oXIScript.API_ParsedFunction.oResult
            // oCR = oXIScript.API2_ExecuteMyOM
            // If oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError Then
            //     txtOutput.Text = oCR.sMessage
            //     txtScript.BackColor = Color.Red
            // Else
            //     txtOutput.Text = oCR.oResult
            //     txtScript.BackColor = Color.Green
            // End If
            return oCResult;
        }

        public CResult Script_Exe(string sScript, string sSessionID, string sGUID)
        {
            CResult oCResult = new CResult();
            CResult oCR = null;
            CScriptController oXIScript = new CScriptController();
            oCR = oXIScript.API2_Serialise_From_String(sScript);
            oCR = oXIScript.API2_ExecuteMyOM("", null, null, sGUID, sSessionID);
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
            }
            else
            {
                oCResult.xiStatus = oCR.xiStatus;
                oCResult.oResult = oCR.oResult;
                oMethodResult = oCR.oResult;
            }

            return oCResult;
        }
    }
}