using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using XISystem;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;

namespace XICore
{
    public class CAlgorithmInstance
    {
        public CAlgorithmDefinition oAlgo = new CAlgorithmDefinition();
        public CMethodStepDefinition oMethodStepD = null;
        public iSiganlR oSignalR = null;
        public string FKiPCDID { get; set; }
        //public Guid FKiPCDIDXIGUID { get; set; }
        public int iPCIID { get; set; }

        // Private oMySteps As New OrderedDictionary
        // Public Property oSteps() As OrderedDictionary
        //     Get
        //         Return oMySteps
        //     End Get
        //     Set(ByVal value As OrderedDictionary)
        //         oMySteps = value
        //     End Set
        // End Property
        private Dictionary<string, CMethodStepInstance> oMySteps = new Dictionary<string, CMethodStepInstance>();

        public Dictionary<string, CMethodStepInstance> oSteps
        {
            get
            {
                return oMySteps;
            }
            set
            {
                oMySteps = value;
            }
        }

        private CNodeItem oMyXIParams = new CNodeItem();

        public CNodeItem oXIParameters
        {
            get
            {
                return oMyXIParams;
            }
            set
            {
                oMyXIParams = value;
            }
        }

        private CScriptController oMyScriptController = new CScriptController();

        public CScriptController oScriptController
        {
            get
            {
                return oMyScriptController;
            }
            set
            {
                oMyScriptController = value;
            }
        }

        private CAlgorithmDefinition oMyAlgoDef = new CAlgorithmDefinition();

        public CAlgorithmDefinition Definition
        {
            get
            {
                return oMyAlgoDef;
            }
            set
            {
                oMyAlgoDef = value;
            }
        }

        private string sMyCurrentStepKey;

        public string sCurrentStepKey
        {
            get
            {
                return sMyCurrentStepKey;
            }
            set
            {
                sMyCurrentStepKey = value;
            }
        }

        public CResult Execute_OM(string sRunTimeParams, string sSessionID, string sGUID, List<XIDAlgorithmLine> Lines = null)//, CXIAPI oXIAPI
        {
            CResult oCResult = new CResult();
            CResult oCR = null;
            XIInfraUsers oUser = new XIInfraUsers();
            //CMethodStepDefinition oMethodStepD = null;
            CMethodStepInstance oMethodStepI = null;
            string sFirstStepKey = "";
            CMethodStepDefinition oFirstStep = null;
            //CMethodStepDefinition oThisStep = null;
            string sStepKey = "";
            string[] AsRTParams;
            //CNodeItem oXIParam = null;
            long j = 0;
            bool bAtLeastOneIssue = false;
            string sDebug = "";
            AsRTParams = sRunTimeParams.Split(',');
            var sMessage = string.Empty;
            if (Definition != null)
            {
                foreach (CNodeItem oXIParam in Definition.oXIParameters.NNodeItems.Values)
                {
                    if (AsRTParams.Length >= j)
                    {
                        oXIParameters.NNode(oXIParam.sKey).sValue = AsRTParams[j];
                    }

                    j = j + 1;
                }
                // first build a flat collection of method instances so at least each step is represented
                oSteps.Clear();
                // if running through again
                foreach (CMethodStepDefinition oMethodStepD in Definition.oMethodDefinition.NMethodSteps.Values)
                {
                    oMethodStepI = new CMethodStepInstance();
                    oMethodStepI.sKey = oMethodStepD.sKey;
                    oMethodStepI.oAlgoI = this;
                    oMethodStepI.oDefinition = oMethodStepD;
                    oSteps.Add(oMethodStepD.sKey, oMethodStepI);
                }

                // now execute the OM from the definition and assign the result to this method instance
                // 'sFirstStepKey = Definition.oMethodOM.NMethodSteps.Keys(0)
                // 'oFirstStep = Definition.oMethodOM '.NMethodSteps(sFirstStepKey)
                // 'oSteps(sFirstStepKey).Execute()
                foreach (CMethodStepDefinition oThisStep in Definition.oMethodOM.NMethodSteps.Values)
                {
                    // oSteps.Values
                    sStepKey = oThisStep.sKey;
                    oCR = oSteps[sStepKey].Execute(sSessionID, sGUID, FKiPCDID.ToString(), oSignalR);
                    if (oCR.xiStatus != xiEnumSystem.xiFuncResult.xiSuccess)
                    {
                        bAtLeastOneIssue = true;
                    }
                    var oLineD = Lines.Where(m => m.sName.ToLower() == sStepKey.ToLower()).FirstOrDefault();
                    if (oLineD != null && oLineD.bUserMessage)
                    {
                        //Save PC line log message
                        oCR = oUser.Save_UserMessage(oLineD.FKiAlgorithmID.ToString(), iPCIID.ToString(), sStepKey, oCR.xiStatus, "", sGUID);
                        if (oCR.bOK && oCR.oResult != null)
                        {

                        }
                        else
                        {

                        }
                    }

                }

                if (bAtLeastOneIssue)
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    // SET THE MESSAGE
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                // if debug:
                foreach (CMethodStepDefinition oThisStep in Definition.oMethodOM.NMethodSteps.Values)
                {
                    // oSteps.Values
                    sStepKey = oThisStep.sKey;
                    oCR = oSteps[sStepKey].ConcatDebug(ref sDebug);
                }
                var sMessageLines = Lines.Where(m => m.bISReturnUIMessage == true).Select(m => m.sName).ToList();

                foreach (var item in sMessageLines)
                {
                    if (oSteps[item.ToLower()].sMethodTrace != null)
                    {
                        var MethodTrace = oSteps[item.ToLower()].sMethodTrace.ToString();
                        sMessage += Regex.Match(MethodTrace, @"\'(.*?)\'").Groups[1].ToString();
                    }
                }
            }
            oCResult.oResult = sDebug;
            //oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            oCResult.sMessage = sMessage;
            oCResult.iCriticality = 10;
            oCResult.sCategory = "Algorithm execution information";
            XIDefinitionBase oDef = new XIDefinitionBase();
            oDef.SaveErrortoDB(oCResult);
            return oCResult;
        }

        public CResult NextStep()
        {
            CResult oCResult = new CResult();
            CMethodStepDefinition oCStep = null;
            CMethodStepDefinition oNextStep = null;
            string sKey = "";
            if (sCurrentStepKey == "")
            {
                // first step
                sCurrentStepKey = Definition.oMethodOM.NMethodSteps[0].ToString();
                // sCurrentStepKey = oCStep.sKey
                oNextStep = (CMethodStepDefinition)Definition.oMethodDefinition.NMethodSteps[sCurrentStepKey];
            }
            else
            {
                // get the next step
                oCStep = (CMethodStepDefinition)Definition.oMethodDefinition.NMethodSteps[sCurrentStepKey];
                if (oCStep.NMethodSteps.Count > 0)
                {
                    oNextStep = (CMethodStepDefinition)oCStep.NMethodSteps[oCStep.NMethodSteps[0].ToString()];
                }
                else
                {
                    // next sibling
                    if (oCStep.oParentMethod != null)
                    {
                        if (oCStep.oParentMethod.NMethodSteps.Count > (oCStep.iChildIndex + 1))
                        {
                            oNextStep = (CMethodStepDefinition)oCStep.oParentMethod.NMethodSteps[oCStep.oParentMethod.NMethodSteps[(oCStep.iChildIndex + 1)]];
                        }
                    }
                }
            }

            if (oNextStep != null)
            {
                sCurrentStepKey = oNextStep.sKey;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            else
            {
                sCurrentStepKey = "END";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiWarning;
            }

            return oCResult;
        }

        public CResult Compile_FromText(string sSourceCode)
        {
            CResult oMyResult = new CResult();
            CResult oCR = null;
            List<string> AsLines = new List<string>();
            long j;
            string[] AsThisLine;
            long k;
            string sThisLine = "";
            string sIdentifier = "";
            string sMethodName = "";
            string sReturnD = "";
            string sErrorD = "";
            string sActionD = "";
            string sExecuteD = "";
            string sAlgoName = "";
            CNodeItem oXIParam = null;
            string[] AsParams;
            string sThisItem = "";
            int p;
            string sThisParam = "";
            string[] AsMethodItem;
            string sMethodSection = "";
            string sRemainingSection = "";
            string sIndent = "";
            long iIndent;
            string sScript = "";
            int iScriptStart;
            try
            {
                oMyResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnumSystem.xiFuncResult.xiInProcess;
                List<string> dictresult = new List<string>();
                sSourceCode = sSourceCode.Replace("\r\n", "");
                AsLines = sSourceCode.Split('&').ToList();
                //Regex oRegex = new Regex("[\r\n]+");
                //if (!string.IsNullOrEmpty(sSourceCode))
                //{
                //    AsLines = oRegex.Matches(sSourceCode).Cast<Match>().Select(s => s.Groups[1].Value).Distinct().ToList();
                //    //dictresult = matches.ToDictionary(d => d);
                //}

                // build an OM of the sourcecode, don't try and interpret anything, but need to validate to ensure no invalids
                //   and that everything is on each line that should be
                foreach (var item in AsLines)
                {
                    sThisLine = item;
                    if (sThisLine != "")
                    {
                        AsThisLine = sThisLine.Split('_');
                        if (AsThisLine.Length > 0)
                        {
                            // first element always has to be identifier. If it is not, error the line
                            sIdentifier = AsThisLine[0].ToLower();
                            if (sIdentifier == "xi.ag")
                            {
                                // algorithm defintition
                                for (k = 1; k < AsThisLine.Length; k++)
                                {
                                    sThisItem = AsThisLine[k];
                                    if (k == 1)
                                    {
                                        sAlgoName = sThisItem;
                                        oAlgo.sName = sAlgoName;
                                    }
                                    else
                                    {
                                        // should be a param
                                        AsParams = sThisItem.Split(',');
                                        for (p = 0; p < AsParams.Length; p++)
                                        {
                                            sThisParam = AsParams[p];
                                            oXIParam = oAlgo.oXIParameters.NNode(sThisParam);
                                            // need to type it or anything??
                                        }

                                    }

                                    // get the params
                                }

                            }
                            else if (sIdentifier == "xi.m")
                            {
                                // method
                                oMethodStepD = null;
                                sIndent = "";
                                iIndent = 0;
                                for (k = 1; k < AsThisLine.Length; k++)
                                {
                                    sThisItem = AsThisLine[k];
                                    sMethodSection = "";
                                    sRemainingSection = "";
                                    AsMethodItem = sThisItem.Split(':');
                                    if (AsMethodItem.Length - 1 > 0)
                                    {
                                        for (p = 0; p < AsMethodItem.Length; p++)
                                        {
                                            // what this element is, is defined by the first of these. So it might read 'r:xiR' which indicates the return. Although we could force the order, we do not, but that means you have to specify what this section is
                                            if (p == 0)
                                            {
                                                sMethodSection = AsMethodItem[p];
                                                sRemainingSection = sThisItem.Substring(sMethodSection.Length + 1, sThisItem.Length - sMethodSection.Length - 1);
                                                switch (sMethodSection.Substring(0, 1))
                                                {
                                                    case "s":
                                                        sIndent = sRemainingSection;
                                                        iIndent = sIndent.Length;
                                                        // Case "-" 'another way of 's' and i prefer it
                                                        //     sIndent = sMethodSection
                                                        //     iIndent = Len(sIndent)
                                                        break;
                                                    case "i":
                                                    case "-":
                                                        if (sMethodSection.Substring(0, 1) == "-")
                                                        {
                                                            sIndent = sMethodSection;
                                                            iIndent = sIndent.Length;
                                                        }

                                                        sMethodName = sRemainingSection;
                                                        // TO DO - validate
                                                        if (oAlgo.oMethodDefinition.NMethodSteps.Contains(sMethodName.ToLower()))
                                                        {
                                                            oMyResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnumSystem.xiFuncResult.xiError;
                                                            oMyResult.sAppend = "Warning: Step: \'" + sMethodName + "\' is not unique in this algorithm";
                                                        }

                                                        oCR = oAlgo.oMethodDefinition.NMethodAdd(sMethodName);
                                                        if (oCR.xiStatus != (xiEnumSystem.xiFuncResult)xiEnumSystem.xiFuncResult.xiSuccess)
                                                        {
                                                            oMyResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnumSystem.xiFuncResult.xiError;
                                                            oMyResult.sAppend = "Error: Adding step: \'" + sMethodName + "\' :" + oCR.sMessage;
                                                            break; // TODO: Warning!!! Review that break works as 'Exit For' as it is inside another 'breakable' statement:Switch
                                                        }
                                                        else
                                                        {
                                                            oMethodStepD = (CMethodStepDefinition)oCR.oResult;
                                                        }

                                                        // oMethodStepD = oAlgo.oMethodDefinition.NMethod(sMethodName)
                                                        oMethodStepD.sStepName = sRemainingSection;
                                                        oMethodStepD.iMethodType = CMethodStepDefinition.xiMethodType.xiMethod;
                                                        oMethodStepD.sIndent = sIndent;
                                                        oMethodStepD.iIndent = iIndent;
                                                        break;
                                                    case "r":
                                                        if (oMethodStepD != null)
                                                        {
                                                            sReturnD = sRemainingSection;
                                                            // TO DO - validate
                                                            oMethodStepD.sReturnType = sReturnD;
                                                        }

                                                        break;
                                                    case "e":
                                                        if (oMethodStepD != null)
                                                        {
                                                            sErrorD = sRemainingSection;
                                                            // TO DO - validate
                                                            oMethodStepD.sErrorType = sErrorD;
                                                        }

                                                        break;
                                                    case "a":
                                                        if (oMethodStepD != null)
                                                        {
                                                            sActionD = sRemainingSection;
                                                            // TO DO - validate
                                                            oMethodStepD.sActionType = sActionD;
                                                        }

                                                        break;
                                                    case "x":
                                                        if (oMethodStepD != null)
                                                        {
                                                            sExecuteD = sRemainingSection;
                                                            // TO DO - validate
                                                            oMethodStepD.sExecute = sExecuteD;
                                                        }

                                                        break;
                                                }
                                                break;
                                            }

                                        }

                                        if (oMethodStepD == null)
                                        {
                                            break;
                                            // this line had an error, don't continue
                                        }

                                    }
                                    else if (sThisItem.Substring(0, 1) == "-")
                                    {
                                        sIndent = sThisItem;
                                        iIndent = sIndent.Length;
                                    }

                                }

                            }
                            else if (sIdentifier == "xi.s")
                            {
                                // method
                                oMethodStepD = null;
                                sIndent = "";
                                iIndent = 0;
                                for (k = 1; k < AsThisLine.Length; k++)
                                {
                                    sThisItem = AsThisLine[k];
                                    sMethodSection = "";
                                    sRemainingSection = "";
                                    AsMethodItem = sThisItem.Split(':');
                                    if (AsMethodItem.Length - 1 > 0)
                                    {
                                        for (p = 0; p < AsMethodItem.Length; p++)
                                        {
                                            // what this element is, is defined by the first of these. So it might read 'r:xiR' which indicates the return. Although we could force the order, we do not, but that means you have to specify what this section is
                                            if (p == 0)
                                            {
                                                sMethodSection = AsMethodItem[p];
                                                sRemainingSection = sThisItem.Substring(sMethodSection.Length + 1, sThisItem.Length - sMethodSection.Length - 1);
                                                switch (sMethodSection.Substring(0, 1))
                                                {
                                                    case "x":
                                                        iScriptStart = (sThisLine.IndexOf("{") + 1) - 1;
                                                        sScript = sThisLine.Substring(iScriptStart, (sThisLine.Length - iScriptStart));
                                                        oMethodStepD.sScript = sScript;
                                                        break;
                                                    case "s":
                                                        sIndent = sRemainingSection;
                                                        iIndent = sIndent.Length;
                                                        break;
                                                    case "i":
                                                    case "-":
                                                        if (sMethodSection.Substring(0, 1) == "-")
                                                        {
                                                            sIndent = sMethodSection;
                                                            iIndent = sIndent.Length;
                                                        }

                                                        sMethodName = sRemainingSection;
                                                        // TO DO - validate
                                                        // oMethodStepD = oAlgo.oMethodDefinition.NMethod(sMethodName)
                                                        oCR = oAlgo.oMethodDefinition.NMethodAdd(sMethodName);
                                                        if (oCR.xiStatus != (xiEnumSystem.xiFuncResult)xiEnumSystem.xiFuncResult.xiSuccess)
                                                        {
                                                            oMyResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnumSystem.xiFuncResult.xiError;
                                                            oMyResult.sAppend = "Error adding step: \'" + sMethodName + "\' :" + oCR.sMessage;
                                                            break; // TODO: Warning!!! Review that break works as 'Exit For' as it is inside another 'breakable' statement:Switch
                                                        }
                                                        else
                                                        {
                                                            oMethodStepD = (CMethodStepDefinition)oCR.oResult;
                                                        }

                                                        oMethodStepD.sStepName = sRemainingSection;
                                                        oMethodStepD.iMethodType = CMethodStepDefinition.xiMethodType.xiScript;
                                                        oMethodStepD.sIndent = sIndent;
                                                        oMethodStepD.iIndent = iIndent;
                                                        // Case "r"
                                                        //     If Not oMethodStepD Is Nothing Then
                                                        //         sReturnD = sRemainingSection 'TO DO - validate
                                                        //         oMethodStepD.sReturnType = sReturnD
                                                        //     End If
                                                        break;
                                                    case "e":
                                                        if (!(oMethodStepD == null))
                                                        {
                                                            sErrorD = sRemainingSection;
                                                            // TO DO - validate
                                                            oMethodStepD.sErrorType = sErrorD;
                                                        }

                                                        break;
                                                }
                                                break;
                                            }

                                        }

                                        if (oMethodStepD == null)
                                        {
                                            break;
                                            // this line had an error, don't continue
                                        }

                                    }
                                    else
                                    {
                                        // Error - we didn't find an identifier for this section. is it a return, is it error or execute etc?
                                    }

                                }

                            }
                            else
                            {
                                // TO DO - error 'unknown identifier: '
                            }

                            // sIdentifier = "xi.ag"
                        }
                        else
                        {
                            // error - not enough info on the line to do anything
                        }

                        // UBound(AsThisLine) > 0 
                    }

                    // sThisLine <> ""
                }

                oCR = oAlgo.CompileOM();
                if (oCR.xiStatus != (xiEnumSystem.xiFuncResult)xiEnumSystem.xiFuncResult.xiSuccess)
                {
                    oMyResult.xiStatus = oCR.xiStatus;
                    oMyResult.sAppend = "Error in compilation Object Model: " + oCR.sMessage;
                    return oMyResult;
                    // TODO: Exit Function: Warning!!! Need to return the value
                }
                else
                {
                    oCR = oAlgo.ValidateOM();
                }

                // now check to see if 
                oMyResult.oResult = oAlgo;
                if (oCR.xiStatus == (xiEnumSystem.xiFuncResult)xiEnumSystem.xiFuncResult.xiSuccess)
                {
                    oMyResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oMyResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnumSystem.xiFuncResult.xiError;
                    oMyResult.sAppend = oCR.sMessage;
                }

            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnumSystem.xiFuncResult.xiError;
            }

            return oMyResult;
            //         xi.ag|DiaryTemplate|p.PolicyFK,p.dGoLive,p.TemplateID
            // xi.m|s:-|i:TemplateDiaries|r:BOInstanceGrid|e:DBLog, xiFail|a: m0.1Click|x: templatediaries
            //         xi.m|s:-|i:PolicyI|r:BOInstance|e:DBLog, xiFail|a: m.BOLoad|x:Policy_T, PolicyFK
            // xi.m|s:-|i:EachDiaryT|r:xiR|e:DBLog, xiFail|a: m.Iterate|x:xim.DiaryTemplate
            //         xi.m|s:--|i:CreateDiaryInstance|r:BOInstance|e:DBLog, xiFail|a: m.CreateBO|x:bod.Diary_T
            //         xi.m|s:--|i:CopyDiary|r:xiR|e:DBLog, xiFail|a: m.BOCopy|x:EachDiaryT, xim.CreateDiaryInstance, xig.CopyLive
            // xi.m|s:--|i:SetValue1|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[dDue=xim.EachDiary.iDaysFromZero+p.dGoLive]
            // xi.m|s:--|i:SetValue2|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[iType=0]
            // xi.m|s:--|i:SetValue3|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[FKiPolicyID=p.PolicyID]
            // xi.m|s:--|i:SetValue4|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[FKiClientID=xim.PolicyI.FKiClientID]
            // xi.m|s:--|i:InsertDiary|r:xiR|e:DBLog, xiFail|a: m.BOSave|x:xim.CreateDiaryInstance
        }

        public CResult CompileFromCXI(CNodeItem oCXI)
        {
            // return a xiResult
            CResult oMyResult = new CResult();
            CResult oCR = null;
            CAlgorithmDefinition oAlgorithmDefinition = new CAlgorithmDefinition();
            foreach (CNodeItem oXIValue in oCXI.NElements.Values)
            {
                // param
                // With...
            }

            CompileFromCXI_Recurse(oCXI, oAlgorithmDefinition.oMethodDefinition);
            return oMyResult;
        }

        public CResult CompileFromCXI_Recurse(CNodeItem oCXI, CMethodStepDefinition oParentMethod)
        {
            CResult oMyResult = new CResult();
            CResult oCR;
            string sError = "";
            string sName = "";
            string sExecute = "";
            string sReturn = "";
            CMethodStepDefinition oChildMethod = null;
            foreach (CNodeItem oXIValue in oCXI.NNodeItems.Values)
            {
                // check validity of each including compiling of the xiscript in execute
                sName = oXIValue.NElements["name"].sValue;
                sReturn = oXIValue.NElements["result"].sValue;
                sError = oXIValue.NElements["error"].sValue;
                sExecute = oXIValue.NElements["execute"].sValue;
                // TO DO - check the validity of these aspects and if they are not valid return warnings and/or errors with this info
                oChildMethod = oParentMethod.NMethod(sName);
                // With...
                oChildMethod.sErrorType = sError;
                oChildMethod.sReturnType = sReturn;
                oChildMethod.sStepName = sName;
                oChildMethod.sExecute = sExecute;
                foreach (CNodeItem oSubXI in oCXI.NNodeItems.Values)
                {
                    oCR = CompileFromCXI_Recurse(oSubXI, oChildMethod);
                }

            }

            return oMyResult;
        }
    }
}