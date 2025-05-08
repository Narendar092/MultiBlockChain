using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using XICore;
using XISystem;

namespace XICore
{
    public class XIInfraHtmlComponent : XIDefinitionBase
    {
        public string sTemplate { get; set; }
        public string sLayout { get; set; }
        public int iBOID { get; set; }
        public string sContent { get; set; }
        public int iType { get; set; }
        public int i1ClickID;
        public bool bIsEdit { get; set; }
        public bool bIsDelete { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sStructureName { get; set; }
        public int iTemplateID { get; set; }

        private XID1Click myoneClickDef;
        public XID1Click oneClickDef
        {
            get
            {
                return myoneClickDef;
            }
            set
            {
                myoneClickDef = value;
            }
        }
        private XIDLayout myXiDLayout;
        public XIDLayout XiDLayout
        {
            get
            {
                return myXiDLayout;
            }
            set
            {
                myXiDLayout = value;
            }
        }
        private List<CNV> myresolveExpressions;
        public List<CNV> resolveExpressions
        {
            get
            {
                return myresolveExpressions;
            }
            set
            {
                myresolveExpressions = value;
            }
        }
        private List<XIContentEditors> myXiDTemplate;
        public List<XIContentEditors> XiDTemplate
        {
            get
            {
                return myXiDTemplate;
            }
            set
            {
                myXiDTemplate = value;
            }
        }
        XIInfraCache oCache = new XIInfraCache();

        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
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
            try
            {
                List<string> Info = new List<string>();
                XID1Click o1ClickD = new XID1Click();
                XID1Click o1ClickC = new XID1Click();
                sSessionID = oParams.Where(m => m.sName.ToLower() == "sSessionID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var s1ClickID = oParams.Where(m => m.sName.ToLower() == "i1ClickID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                sLayout = oParams.Where(m => m.sName.ToLower() == "sLayout".ToLower()).Select(m => m.sValue).FirstOrDefault();
                sTemplate = oParams.Where(m => m.sName.ToLower() == "sTemplate".ToLower()).Select(m => m.sValue).FirstOrDefault();
                sStructureName = oParams.Where(m => m.sName.ToLower() == "sStructureName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sCallHierarchy = oParams.Where(m => m.sName == XIConstant.Param_CallHierarchy).Select(m => m.sValue).FirstOrDefault();
                var Mode = oParams.Where(m => m.sName.ToLower() == "sMode".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sContent = oParams.Where(m => m.sName.ToLower() == "Content".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //string iLayoutID = string.Empty;
                //var iTemplateID = string.Empty;
                //iLayoutID = oParams.Where(m => m.sName.ToLower() == "slayout").Select(m => m.sValue).FirstOrDefault();
                var TemplateID = oParams.Where(m => m.sName.ToLower() == "sTemplate".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iTemplateID = 0;
                int.TryParse(TemplateID, out iTemplateID);
                Guid TemplateXIGUID = Guid.Empty;
                Guid.TryParse(TemplateID, out TemplateXIGUID);
                bool bSuccess = Int32.TryParse(s1ClickID, out i1ClickID);
                Guid ClickGUID = Guid.Empty;
                Guid.TryParse(s1ClickID, out ClickGUID);
                XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                List<CNV> nParms = new List<CNV>();
                nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                int iPageCount = 0;
                var sPageCount = oParams.Where(m => m.sName == "iPageCount").Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sPageCount, out iPageCount);
                bool bIsLockStep = false;
                var bIsDisabled = string.Empty;
                if (bIsLockStep)
                {
                    bIsDisabled = "Disabled=" + "Disabled";
                }
                var disable = "unchecked";
                if (i1ClickID > 0 || (ClickGUID != null && ClickGUID != Guid.Empty) && string.IsNullOrEmpty(Mode))
                {
                    //Get 1-Click Defintion             
                    if (ClickGUID != null && ClickGUID != Guid.Empty)
                    {
                        o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, ClickGUID.ToString());
                    }
                    else if (i1ClickID > 0)
                    {
                        o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID.ToString());
                    }
                    o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    //Get BO Definition
                    XIDBO oBOD = new XIDBO();
                    if (o1ClickD.BOIDXIGUID != null && o1ClickD.BOIDXIGUID != Guid.Empty)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOIDXIGUID.ToString());
                    }
                    else if (o1ClickD.BOID > 0)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                    }
                    o1ClickC.BOD = oBOD;

                    //Get Headings of 1-Click
                    o1ClickC.Get_1ClickHeadings();
                    if (o1ClickC.DisplayAs == 120)
                    {
                        XIContentEditors oContent = new XIContentEditors();
                        o1ClickC.iSkip = iPageCount * o1ClickC.iPaginationCount;
                        o1ClickC.iTake = o1ClickC.iPaginationCount;
                        o1ClickC.iTotaldisplayRecords = iPageCount;
                        o1ClickC.sGUID = sGUID;
                        o1ClickC.ReplaceFKExpressions(nParms);
                        //Dictionary<string, XIIBO> oRes = o1ClickC.OneClick_Execute();
                        Dictionary<string, XIIBO> oRes = o1ClickC.OneClick_Execute();
                        if (o1ClickC.XIComponent != null && o1ClickC.XIComponent.sName.ToLower() == XIConstant.HTMLComponent.ToLower())
                        {

                            //if (o1ClickC.XIComponent.Params != null && o1ClickC.XIComponent.Params.Count() > 0)
                            //{
                            //    iLayoutID = o1ClickC.XIComponent.Params.Where(m => m.sName.ToLower() == "slayout").Select(m => m.sValue).FirstOrDefault();
                            //    iTemplateID = o1ClickC.XIComponent.Params.Where(m => m.sName.ToLower() == "stemplate").Select(m => m.sValue).FirstOrDefault();
                            //}
                            List<XIContentEditors> oContentDef = new List<XIContentEditors>();
                            if (!string.IsNullOrEmpty(sLayout))
                            {
                                var oLayout = (XIDLayout)oCache.GetObjectFromCache(XIConstant.CacheLayout, sLayout);
                                oContent = new XIContentEditors();
                                oContent.Content = oLayout.LayoutCode;
                            }
                            else if (TemplateXIGUID != null && TemplateXIGUID != Guid.Empty)
                            {
                                oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, TemplateXIGUID.ToString());
                                if (oContentDef != null && oContentDef.Count() > 0)
                                {
                                    oContent = oContentDef.FirstOrDefault();
                                }
                            }
                            else if (!string.IsNullOrEmpty(Convert.ToString(iTemplateID)))
                            {
                                oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, Convert.ToString(iTemplateID));
                                if (oContentDef != null && oContentDef.Count() > 0)
                                {
                                    oContent = oContentDef.FirstOrDefault();
                                }
                            }
                            o1ClickC.RepeaterResult = new List<string>();
                            foreach (var oBOI in oRes.Values)
                            {
                                string sNewGUID = Guid.NewGuid().ToString();
                                List<XIIBO> nBOI = new List<XIIBO>();
                                if (oBOI.iBODID == 0)
                                {
                                    oBOI.iBODID = o1ClickC.BOID;
                                }
                                nBOI.Add(oBOI);
                                var sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iParentQSInstanceID}");
                                if (string.IsNullOrEmpty(sInstanceID) || sInstanceID=="0")
                                {
                                    sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|" + oBOD.Name + ".id}");
                                }
                                if (string.IsNullOrEmpty(sInstanceID) || sInstanceID == "0")
                                {
                                   sInstanceID = oBOI.AttributeI(oBOD.sPrimaryKey).sValue;
                                }
                                oCache.Set_ParamVal(sSessionID, sNewGUID, null, "{-iInstanceID}", sInstanceID, null, null);
                                oCache.Set_ParamVal(sSessionID, sNewGUID, null, "{XIP|" + oBOD.Name + ".id}", sInstanceID, null, null);
                                XIBOInstance oBOIns = new XIBOInstance();
                                oBOIns.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                                oBOIns.oStructureInstance[oBOD.Name.ToLower()] = nBOI;
                                XIContentEditors oConent = new XIContentEditors();
                                var Result = new CResult();
                                XIIXI oIXI = new XIIXI();
                                oConent.sSessionID = sSessionID;
                                if (!string.IsNullOrEmpty(sInstanceID) && !string.IsNullOrEmpty(sStructureName))
                                {
                                    var oStructobj = oIXI.BOI(oBOD.Name, sInstanceID).Structure(sStructureName).XILoad(null, true);
                                    //Result = oConent.MergeTemplateContent(oContent, oStructobj);
                                    Result = oConent.MergeContentTemplate(oContent, oStructobj);
                                }
                                else
                                {
                                    //Result = oConent.MergeTemplateContent(oContent, oBOIns);
                                    Result = oConent.MergeContentTemplate(oContent, oBOIns);
                                }
                                if (Result.bOK && Result.oResult != null)
                                {
                                    var oBOIHTML = Result.oResult.ToString();
                                    var regx = new Regex("{.*?}");
                                    var Matches = regx.Matches(oBOIHTML);
                                    if (Matches.Count > 0)
                                    {
                                        foreach (var match in Matches)
                                        {
                                            if (match.ToString().ToLower() == "{editbtn}")
                                            {
                                                var EditBtn = "<input type=\"button\" value=\"Edit\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                oBOIHTML = oBOIHTML.Replace("{EditBtn}", EditBtn);
                                            }
                                            else if (match.ToString().ToLower() == "{deletebtn}")
                                            {
                                                var DeleteBtn = "<input type=\"button\" value=\"Delete\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                oBOIHTML = oBOIHTML.Replace("{DeleteBtn}", DeleteBtn);
                                            }
                                            else if (match.ToString().ToLower() == "{more details}" || match.ToString().ToLower() == "{view quote}" || match.ToString().ToLower() == "{view}")
                                            {
                                                //List<CNV> fncParams = new List<CNV>();
                                                //fncParams.Add(new CNV { sName = "{-iInstanceID}", sValue = sInstanceID });
                                                //fncParams.Add(new CNV { sName = "{XIP|" + oBOD.Name + ".id}", sValue = sInstanceID });
                                                //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                                                //string sJSON = oSerializer.Serialize(fncParams);
                                                //string sInputValue = Regex.Replace(match.ToString().Replace("{", "").Replace("}", ""), @"\s+", "");
                                                var MoreDetailsBtn = "<input type=\"button\" onclick=XILinkLoadJson(" + o1ClickC.RowXiLinkID + ",'" + sNewGUID + "') value=\"" + match.ToString().Replace("{", "").Replace("}", "") + "\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                //var MoreDetailsBtn = "<input type=\"button\" onclick=\"XIRun('" + o1ClickC.RowXiLinkID + "','" + sInstanceID + "','" + sGUID + "','" + o1ClickC.sBOName + "',false," + o1ClickC.BOID + ",0)\" value=\"" + match.ToString().Replace("{", "").Replace("}", "") + "\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                //var MoreDetailsBtn = "<input type=\"button\" value=\""+ match.ToString().Replace("{", "").Replace("}", "") + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"ViewMore('"+ sInputValue + "_" + sInstanceID + "')\" />";
                                                oBOIHTML = oBOIHTML.Replace(match.ToString(), MoreDetailsBtn);
                                            }
                                            else if (match.ToString().ToLower() == "{remove}" || match.ToString().ToLower() == "{add}")
                                            {
                                                var RemoveBtn = "<input type=\"checkbox\" class=\"switch-input\" Onchange =\"fnc1clickremove('" + o1ClickC.ID + "','" + o1ClickC.CreateGroupID + "','" + o1ClickC.BOID + "','" + sInstanceID + "' ,this " + ")\" " + disable + " " + bIsDisabled + " />";
                                                oBOIHTML = oBOIHTML.Replace("{Remove}", RemoveBtn);
                                            }
                                            else if (match.ToString().ToLower() == "{snewguid}")
                                            {
                                                oBOIHTML = oBOIHTML.Replace(match.ToString(), string.Format("'{0}'", sNewGUID));
                                            }
                                            else if (match.ToString().ToLower() == "{buynow}")
                                            {
                                                var BuyBtn = "<input type=\"button\" value=\"BuyNow\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"BuyQuoteBtn('" + oBOI.AttributeI("rfinalquote").sValue + "','" + sInstanceID + "',this " + ")\" " + bIsDisabled + " />";
                                                oBOIHTML = oBOIHTML.Replace(match.ToString(), BuyBtn);
                                            }
                                            else if (match.ToString().ToLower() == "{remove quote}")
                                            {
                                                var DeleteBtn = "<input type=\"button\" value=\"Remove\" class=\"btn btn-theme\" id=\"Delete_" + oBOD.Name + "_" + sInstanceID + "\" data-id=" + sInstanceID + " data-Remove=\"Quote_" + sInstanceID + "\" onclick=\"DeleteBO('" + oBOI.Attributes[oBOD.sPrimaryKey].sValue + "','" + sGUID + "','" + oBOD.Name + "','" + o1ClickC.XIComponent.sName.ToLower() + "' ,this " + " )\" " + bIsDisabled + " />";
                                                oBOIHTML = oBOIHTML.Replace("{Remove Quote}", DeleteBtn);
                                            }
                                            else
                                            {
                                                string Code = match.ToString().TrimStart('{').TrimEnd('}');
                                                var oLink = o1ClickC.MyLinks.Where(m => m.sCode == Code).FirstOrDefault();
                                                //if (Code.StartsWith("Close"))
                                                //{
                                                //    if (oLink != null)
                                                //    {
                                                //        var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"CloseDetails('PolicyDetails_" + sInstanceID + "')\" />";
                                                //        oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                //    }
                                                //}
                                                if (Code.StartsWith("Make a Change"))
                                                {
                                                    if (oLink != null)
                                                    {
                                                        var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"MakeaChange('" + sInstanceID + "',this)\" />";
                                                        oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                    }
                                                }
                                                else if (Code.StartsWith("Make a Claim"))
                                                {
                                                    if (oLink != null)
                                                    {
                                                        var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"MakeaClaim('" + sInstanceID + "',this)\" />";
                                                        oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                    }
                                                }
                                                else
                                                {
                                                    if (oLink != null)
                                                    {
                                                        List<CNV> fncParams = new List<CNV>();//{XIP|ACPolicy_T.id}
                                                        fncParams.Add(new CNV { sName = oBOD.Name, sValue = sInstanceID });
                                                        fncParams.Add(new CNV { sName = "{XIP|" + oBOD.Name + ".id}", sValue = sInstanceID });
                                                        System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                                                        string sJSON = oSerializer.Serialize(fncParams);
                                                        var Btn = string.Empty;
                                                        if (oLink.iType == 10)
                                                        {
                                                            Btn = "<input type=\"button\" onclick=XIRun(" + oLink.FKiXILinkID + ",0,'" + sGUID + "',null,false,0,null) value=\"" + oLink.sName + "\" class=\"btn btn-theme BusinessCol\" data-id=" + sInstanceID + " />";
                                                        }
                                                        else
                                                        {
                                                            Btn = "<input type=\"button\" onclick=XILinkLoadJson(" + oLink.FKiXILinkID + ",'" + sGUID + "',JSON.parse('" + sJSON + "')) value=\"" + oLink.sName + "\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                        }
                                                        //var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"fnc1clickcreate('" + o1ClickC.ID + "','" + o1ClickC.CreateGroupID + "','" + o1ClickC.BOID + "','" + oLink.FKiXILinkID + "','" + sInstanceID + "')\" />";
                                                        oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    o1ClickC.RepeaterResult.Add(oBOIHTML);
                                }
                            }
                        }
                        else if (iTemplateID > 0 || (TemplateXIGUID != null && TemplateXIGUID != Guid.Empty))
                        {
                            List<XIContentEditors> oContentDef = new List<XIContentEditors>();
                            if (TemplateXIGUID != null && TemplateXIGUID != Guid.Empty)
                            {
                                oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, TemplateXIGUID.ToString());
                                if (oContentDef != null && oContentDef.Count() > 0)
                                {
                                    oContent = oContentDef.FirstOrDefault();
                                }
                            }
                            else if (iTemplateID > 0)
                            {
                                oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, Convert.ToString(iTemplateID));
                                if (oContentDef != null && oContentDef.Count() > 0)
                                {
                                    oContent = oContentDef.FirstOrDefault();
                                }
                            }
                            o1ClickC.RepeaterResult = new List<string>();
                            var Result = new CResult();
                            XIContentEditors oConent = new XIContentEditors();
                            foreach (var oBOI in oRes.Values)
                            {
                                List<XIIBO> nBOI = new List<XIIBO>();
                                if (oBOI.iBODID == 0)
                                {
                                    oBOI.iBODID = o1ClickC.BOID;
                                }
                                nBOI.Add(oBOI);
                                XIBOInstance oBOIns = new XIBOInstance();
                                oBOIns.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                                oBOIns.oStructureInstance[oBOD.Name.ToLower()] = nBOI;
                                //oContent.Content = @"<|<xi.s|{translate|{{oSubStructureI(\""xidocumenttree\"").Item(0).AttributeI(\""iApprovalStatus\"").sValue}},'10','green','20','red','30','amber'}>|>";
                                Result = oConent.MergeContentTemplate(oContent, oBOIns);
                                var oBOIHTML = Result.oResult.ToString();
                                o1ClickC.RepeaterResult.Add(oBOIHTML);
                            }
                        }

                        //o1ClickC.RepeaterResult = new List<string>();
                        //string sHTMLLayout = string.Empty;
                        //XIContentEditors oContent;
                        //if (!string.IsNullOrEmpty(sTemplate))
                        //{
                        //    oContent = (XIContentEditors)oCache.GetObjectFromCache(XIConstant.CacheTemplate, sTemplate, "0");
                        //}
                        //else
                        //{
                        //    var oLayout = (XIDLayout)oCache.GetObjectFromCache(XIConstant.CacheLayout, sLayout, "0");
                        //    oContent = new XIContentEditors();
                        //    oContent.Content = oLayout.LayoutCode;
                        //}                        
                        //foreach (var oBOI in oRes.Values)
                        //{
                        //    List<XIIBO> nBOI = new List<XIIBO>();
                        //    nBOI.Add(oBOI);
                        //    XIBOInstance oBOIns = new XIBOInstance();
                        //    oBOIns.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                        //    oBOIns.oStructureInstance[oBOD.Name.ToLower()] = nBOI;
                        //    XIContentEditors oConent = new XIContentEditors();
                        //    var Result = oConent.MergeTemplateContent(oContent, oBOIns);
                        //    if (Result.bOK && Result.oResult != null)
                        //    {
                        //        o1ClickC.RepeaterResult.Add(Result.oResult.ToString());
                        //    }
                        //}
                        o1ClickC.sBOName = oBOD.Name;
                        o1ClickC.BOID = oBOD.BOID;
                        o1ClickC.BOD = null;
                        o1ClickC.HTMLCode = oContent.sContainerHTML;
                        //o1ClickC.sCreateHTML = oContent.sCreateHTML;
                        oCResult.oResult = o1ClickC;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                }
                else if (i1ClickID == 0 && sTemplate != "0" && string.IsNullOrEmpty(Mode))
                {

                    //XIDBO oBOD = new XIDBO();
                    //oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, "717");
                    //o1ClickC.BOD = oBOD;
                    //o1ClickC.BOID = oBOD.BOID;
                    //o1ClickC.BOName = oBOD.Name;
                    List<XIContentEditors> oContentDef = new List<XIContentEditors>();
                    XIContentEditors oContent = new XIContentEditors();
                    //Dictionary<string, XIIBO> oRes = o1ClickC.OneClick_Run();
                    if (TemplateXIGUID != null && TemplateXIGUID != Guid.Empty)
                    {
                        oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, TemplateXIGUID.ToString());
                        if (oContentDef != null && oContentDef.Count() > 0)
                        {
                            oContent = oContentDef.FirstOrDefault();
                        }
                    }
                    else if (iTemplateID > 0)
                    {
                        oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, Convert.ToString(iTemplateID));
                        if (oContentDef != null && oContentDef.Count() > 0)
                        {
                            oContent = oContentDef.FirstOrDefault();
                        }
                    }
                    o1ClickC.RepeaterResult = new List<string>();
                    //foreach (var oBOI in oRes.Values)
                    //{
                    string sNewGUID = Guid.NewGuid().ToString();
                    XIContentEditors oConent = new XIContentEditors();
                    var Result = new CResult();
                    XIIXI oIXI = new XIIXI();
                    oConent.sSessionID = sSessionID;
                    //Result = oConent.MergeTemplateContent(oContent, oBOIns);
                    Result = oConent.MergeContentTemplate(oContent, null);
                    if (Result.bOK && Result.oResult != null)
                    {
                        var oBOIHTML = Result.oResult.ToString();
                        o1ClickC.RepeaterResult.Add(oBOIHTML);
                        o1ClickC.bFlag = true;
                    }
                    //o1ClickC.sBOName = oBOD.Name;
                    //o1ClickC.BOID = oBOD.BOID;
                    o1ClickC.BOD = null;
                    oCResult.oResult = o1ClickC;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else if (!string.IsNullOrEmpty(Mode))
                {
                    o1ClickC.RepeaterResult = new List<string>();
                    XIIQS oQSInstance = new XIIQS();
                    XIIXI oIXI = new XIIXI();
                    var QSInstanceID = oParams.Where(m => m.sName.ToLower() == "FKiQSInstanceIDXIGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    var StructureName = string.Empty;
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|FKiQSInstanceIDXIGUID}", QSInstanceID.ToString(), null, null);

                    XIIQS oQSI = new XIIQS();
                    oQSInstance = oIXI.GetQSXIValuesByQSIID(QSInstanceID.ToString());
                    if (oQSInstance.XIValues.Count > 0)
                    {
                        var iInstanceID = oQSInstance.XIValues["Instance ID"].sValue;
                        var StructureID = oQSInstance.XIValues["Structure"].sValue;
                        var Template = oQSInstance.XIValues["TemplateID"].sValue;
                        var oLIst = oIXI.BOI("XI BOStructure", StructureID);
                        if (oLIst != null)
                        {
                            var BOName = oLIst.Attributes["sBO"].sValue;
                            oLIst = oIXI.BOI(BOName, iInstanceID);
                            if (oLIst != null)
                            {
                                StructureName = oQSInstance.XIValues["Structure"].sDerivedValue;
                                var oStructobj = oLIst.Structure(StructureName).XILoad(null, true);
                                if (oStructobj.oParent == null)
                                {
                                    o1ClickC.sErrorMessage = "Selected structure has some configuration issues, please check error log";
                                }
                                XIContentEditors oConent = new XIContentEditors();
                                List<XIContentEditors> oContentDef = new List<XIContentEditors>();
                                if (Template != "0")
                                {
                                    oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, Template);
                                }
                                if (oContentDef != null && oContentDef.Count() > 0)
                                {
                                    oConent = oContentDef.FirstOrDefault();
                                    if (!string.IsNullOrEmpty(sContent))
                                    {
                                        oConent.Content = sContent;
                                    }
                                    if (oConent.Content != null)
                                    {
                                        o1ClickC.RepeaterResult.Add(oConent.Content.ToString());
                                    }
                                    else
                                    {
                                        o1ClickC.RepeaterResult.Add("");
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(sContent))
                                    {
                                        oConent.Content = sContent;
                                    }
                                    else
                                    {
                                        o1ClickC.RepeaterResult.Add("");
                                    }
                                }
                                if (!string.IsNullOrEmpty(sContent))
                                {
                                    oConent.ID = 0;
                                    var Result = oConent.MergeContentTemplate(oConent, oStructobj);
                                    Result.sCode = Mode;
                                    o1ClickC.RepeaterResult = new List<string>();
                                    o1ClickC.RepeaterResult.Add(Result.oResult.ToString());
                                    oCResult.oResult = Result.oResult;
                                }
                                else
                                {
                                    //o1ClickC.RepeaterResult.Add("");
                                    oCResult.oResult = o1ClickC;
                                }
                            }
                            else
                            {
                                o1ClickC.RepeaterResult.Add("");
                                o1ClickC.sErrorMessage = "Instance ID not available for the selected structure: " + iInstanceID;
                                oCR.sMessage = "Config Error: XIInfraHtmlComponent_XILoad() : Instance ID not available for the selected structure - Call Hierarchy: " + sCallHierarchy;
                                oCR.sCode = "Config Error";
                                SaveErrortoDB(oCR);
                            }
                            o1ClickC.sCode = Mode;
                            o1ClickC.ID = Convert.ToInt32(iInstanceID);
                            o1ClickC.FKiStructureID = Convert.ToInt32(StructureID);
                            o1ClickC.EmailTemplateID = Convert.ToInt32(Template);
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = o1ClickC;
                            //return oCResult;
                        }
                        else
                        {
                            o1ClickC.RepeaterResult.Add("");
                            o1ClickC.sErrorMessage = "Please check your Selected structure: " + StructureName;
                            oCR.sMessage = "Config Error: XIInfraHtmlComponent_XILoad() : Structure " + StructureName + " is not defined - Call Hierarchy: " + sCallHierarchy;
                            oCR.sCode = "Config Error";
                            oCResult.oResult = o1ClickC;
                            SaveErrortoDB(oCR);
                        }
                    }

                }
                else
                {
                    o1ClickC.RepeaterResult.Add("");
                    oCR.sMessage = "Config Error: XIInfraHtmlComponent_XILoad() : 1-Click ID is not passed as parameter - Call Hierarchy: " + sCallHierarchy;
                    oCR.sCode = "Config Error";
                    oCResult.oResult = o1ClickC;
                    SaveErrortoDB(oCR);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oCResult;
        }
        public CResult XILoadRefactored(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;
            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
            }
            try
            {
                //XID1Click o1ClickD = new XID1Click();
                XID1Click o1ClickC = new XID1Click();
                sSessionID = oParams.Where(m => m.sName.ToLower() == "sSessionID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var s1ClickID = oParams.Where(m => m.sName.ToLower() == "i1ClickID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                sLayout = oParams.Where(m => m.sName.ToLower() == "sLayout".ToLower()).Select(m => m.sValue).FirstOrDefault();
                sTemplate = oParams.Where(m => m.sName.ToLower() == "sTemplate".ToLower()).Select(m => m.sValue).FirstOrDefault();
                sStructureName = oParams.Where(m => m.sName.ToLower() == "sStructureName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                iTemplateID = Convert.ToInt32(oParams.Where(m => m.sName.ToLower() == "sTemplate".ToLower()).Select(m => m.sValue).FirstOrDefault());
                bool bSuccess = Int32.TryParse(s1ClickID, out i1ClickID);
                if (i1ClickID > 0)
                {
                    //Get 1-Click Defintion             
                    // o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID.ToString());
                    // o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    //o1ClickC = oneClickDef.GetCopy();
                    o1ClickC = (XID1Click)oneClickDef.Clone(oneClickDef);
                    o1ClickC.BOD = oneClickDef.BOD;
                    //Get BO Definition
                    //XIDBO oBOD = new XIDBO();
                    //oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                    //o1ClickC.BOD = oBOD;
                    //Get Headings of 1-Click
                    o1ClickC.Get_1ClickHeadings();
                    if (o1ClickC.DisplayAs == 120)
                    {
                        //XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                        //List<CNV> nParms = new List<CNV>();
                        //nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                        o1ClickC.ReplaceFKExpressions(resolveExpressions);
                        Dictionary<string, XIIBO> oRes = o1ClickC.OneClick_Run(false);
                        if (o1ClickC.XIComponent.sName.ToLower() == XIConstant.HTMLComponent.ToLower())
                        {
                            XIContentEditors oContent = new XIContentEditors();
                            //List<XIContentEditors> oContentDef = new List<XIContentEditors>();
                            if (!string.IsNullOrEmpty(sLayout))
                            {
                                var oLayout = XiDLayout;// (XIDLayout)oCache.GetObjectFromCache(XIConstant.CacheLayout, sLayout);
                                oContent = new XIContentEditors();
                                oContent.Content = oLayout.LayoutCode;
                            }
                            else if (!string.IsNullOrEmpty(Convert.ToString(iTemplateID)))
                            {
                                var oContentDef = XiDTemplate;//(List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, Convert.ToString(iTemplateID));
                                if (oContentDef != null && oContentDef.Count() > 0)
                                {
                                    oContent = oContentDef.FirstOrDefault();
                                }
                            }
                            o1ClickC.RepeaterResult = new List<string>();
                            foreach (var oBOI in oRes.Values)
                            {
                                List<XIIBO> nBOI = new List<XIIBO>();
                                if (oBOI.iBODID == 0)
                                {
                                    oBOI.iBODID = o1ClickC.BOID;
                                }
                                nBOI.Add(oBOI);

                                var sInstanceID = oBOI.AttributeI(o1ClickC.BOD.sPrimaryKey).sValue;
                                XIBOInstance oBOIns = new XIBOInstance();
                                oBOIns.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                                oBOIns.oStructureInstance[o1ClickC.BOD.Name.ToLower()] = nBOI;
                                XIContentEditors oConent = new XIContentEditors();
                                var Result = new CResult();
                                XIIXI oIXI = new XIIXI();
                                oConent.sSessionID = sSessionID;
                                if (!string.IsNullOrEmpty(sStructureName))
                                {
                                    var oStructobj = oIXI.BOI(o1ClickC.BOD.Name, sInstanceID).Structure(sStructureName).XILoad(null, true);
                                    Result = oConent.MergeContentTemplate(oContent, oStructobj);
                                }
                                else
                                {
                                    Result = oConent.MergeContentTemplate(oContent, oBOIns);
                                }
                                if (Result.bOK && Result.oResult != null)
                                {
                                    var oBOIHTML = Result.oResult.ToString();
                                    var regx = new Regex("{.*?}");
                                    var Matches = regx.Matches(oBOIHTML);
                                    if (Matches.Count > 0)
                                    {
                                        foreach (var match in Matches)
                                        {
                                            if (match.ToString().ToLower() == "{editbtn}")
                                            {
                                                var EditBtn = "<input type=\"button\" value=\"Edit\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                oBOIHTML = oBOIHTML.Replace("{EditBtn}", EditBtn);
                                            }
                                            else if (match.ToString().ToLower() == "{deletebtn}")
                                            {
                                                var DeleteBtn = "<input type=\"button\" value=\"Delete\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                oBOIHTML = oBOIHTML.Replace("{DeleteBtn}", DeleteBtn);
                                            }
                                            else if (match.ToString().ToLower() == "{more details}" || match.ToString().ToLower() == "{view quote}")
                                            {
                                                List<CNV> fncParams = new List<CNV>();
                                                fncParams.Add(new CNV { sName = "{-iInstanceID}", sValue = sInstanceID });
                                                fncParams.Add(new CNV { sName = "{XIP|" + o1ClickC.BOD.Name + ".id}", sValue = sInstanceID });
                                                System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                                                string sJSON = oSerializer.Serialize(fncParams);
                                                var MoreDetailsBtn = "<input type=\"button\" onclick=XILinkLoadJson(" + o1ClickC.RowXiLinkID + ",'" + sGUID + "',JSON.parse('" + sJSON + "')) value=\"" + match.ToString().Replace("{", "").Replace("}", "") + "\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                oBOIHTML = oBOIHTML.Replace(match.ToString(), MoreDetailsBtn);
                                            }
                                            else
                                            {
                                                string Code = match.ToString().TrimStart('{').TrimEnd('}');
                                                var oLink = o1ClickC.MyLinks.Where(m => m.sCode == Code).FirstOrDefault();

                                                if (Code.StartsWith("Make a Change"))
                                                {
                                                    if (oLink != null)
                                                    {
                                                        var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"MakeaChange('" + sInstanceID + "',this)\" />";
                                                        oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                    }
                                                }
                                                else if (Code.StartsWith("Make a Claim"))
                                                {
                                                    if (oLink != null)
                                                    {
                                                        var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"MakeaClaim('" + sInstanceID + "',this)\" />";
                                                        oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                    }
                                                }
                                                else
                                                {
                                                    if (oLink != null)
                                                    {
                                                        List<CNV> fncParams = new List<CNV>();//{XIP|ACPolicy_T.id}
                                                        fncParams.Add(new CNV { sName = o1ClickC.BOD.Name, sValue = sInstanceID });
                                                        fncParams.Add(new CNV { sName = "{XIP|" + o1ClickC.BOD.Name + ".id}", sValue = sInstanceID });
                                                        System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                                                        string sJSON = oSerializer.Serialize(fncParams);
                                                        var Btn = "<input type=\"button\" onclick=XILinkLoadJson(" + oLink.FKiXILinkID + ",'" + sGUID + "',JSON.parse('" + sJSON + "')) value=\"" + oLink.sName + "\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                        //var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"fnc1clickcreate('" + o1ClickC.ID + "','" + o1ClickC.CreateGroupID + "','" + o1ClickC.BOID + "','" + oLink.FKiXILinkID + "','" + sInstanceID + "')\" />";
                                                        oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    o1ClickC.RepeaterResult.Add(oBOIHTML);
                                }
                            }
                        }
                        o1ClickC.sBOName = o1ClickC.BOD.Name;
                        o1ClickC.BOID = o1ClickC.BOD.BOID;
                        o1ClickC.BOD = null;
                        oCResult.oResult = o1ClickC;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                }
                else // For Structure Loading
                {
                    XIIXI oIXI = new XIIXI();
                    XIContentEditors oXIContent = new XIContentEditors();
                    oXIContent.iDriverEDICount = Convert.ToInt32(oParams.Where(m => m.sName == "iDriverEDICount").Select(m => m.sValue).FirstOrDefault());
                    oXIContent.iClaimEDICount = Convert.ToInt32(oParams.Where(m => m.sName == "iClaimEDICount").Select(m => m.sValue).FirstOrDefault());
                    oXIContent.iCovictionEDICount = Convert.ToInt32(oParams.Where(m => m.sName == "iCovictionEDICount").Select(m => m.sValue).FirstOrDefault());
                    oXIContent.iEndorsementEDICount = Convert.ToInt32(oParams.Where(m => m.sName == "iEndorsementEDICount").Select(m => m.sValue).FirstOrDefault());
                    oXIContent.iDriverEDICommaCount = Convert.ToInt32(oParams.Where(m => m.sName == "iDriverEDICommaCount").Select(m => m.sValue).FirstOrDefault());
                    oXIContent.iClaimEDICommaCount = Convert.ToInt32(oParams.Where(m => m.sName == "iClaimEDICommaCount").Select(m => m.sValue).FirstOrDefault());
                    oXIContent.iConvictionEDICommaCount = Convert.ToInt32(oParams.Where(m => m.sName == "iConvictionEDICommaCount").Select(m => m.sValue).FirstOrDefault());
                    oXIContent.iEndorsementEDICommaCount = Convert.ToInt32(oParams.Where(m => m.sName == "iEndorsementEDICommaCount").Select(m => m.sValue).FirstOrDefault());
                    oXIContent.Category = Convert.ToInt32(oParams.Where(m => m.sName == "Category").Select(m => m.sValue).FirstOrDefault());

                    oXIContent.Content = oParams.Where(d => d.sName.ToLower() == "RawContent".ToLower()).Select(d => d.sValue).FirstOrDefault();
                    string sBOName = oParams.Where(d => d.sName.ToLower() == "sBO".ToLower()).Select(d => d.sValue).FirstOrDefault();
                    int iInstanceID = Convert.ToInt32(oParams.Where(m => m.sName == "iInstanceID").Select(m => m.sValue).FirstOrDefault());
                    var oLIst = oIXI.BOI(sBOName, iInstanceID.ToString());
                    if (oLIst != null)
                    {
                        XIBOInstance oInstance = new XIBOInstance();
                        oInstance = oLIst.Structure(sStructureName).XILoad();
                        oXIContent.sSessionID = sSessionID;
                        oCResult = oXIContent.MergeContentTemplate(oXIContent, oInstance);//Get template content with dynamic data
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }
    }
}