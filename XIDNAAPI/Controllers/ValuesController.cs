using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using XICore;
//using XIDNAAPI.Handlers;
using XISystem;
namespace XIDNAAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
        XIInfraCache oCache = new XIInfraCache();
        CResult oCResult = new CResult();
        XIDBO oBOD = new XIDBO();
        XIDefinitionBase oXID = new XIDefinitionBase();
        string sReceiver = "slgm";

        public XIContentEditors LoadTemplate(string sTemplateID)
        {
            XIContentEditors oContent = new XIContentEditors();
            List<XIContentEditors> oContentDef = new List<XIContentEditors>();
            if (!string.IsNullOrEmpty(sTemplateID))
            {
                oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, sTemplateID, null);
                if (oContentDef != null && oContentDef.Count() > 0)
                {
                    oContent = oContentDef.FirstOrDefault();
                }
            }
            return oContent;
        }
        public string GetMails(string sKey)
        {
            string sFunction = "xi.s|{xi.a|'XIConfig_T','" + sKey + "','sValue','','sName'}";
            CResult oCRSLMG = new CResult();
            XIDScript oXIScript = new XIDScript();
            oXIScript.sScript = sFunction.ToString();
            oCRSLMG = oXIScript.Execute_Script("", "");
            return oCRSLMG.oResult.ToString();
        }
        public CResult SendLeadMail(XIContentEditors oContent, XIBOInstance oInstance, XIIBO oBOI, string sEmail, string o1ClickID)
        {
            XIInfraEmail oEmail = new XIInfraEmail();
            XIContentEditors oContentEditor = new XIContentEditors();
            CResult Result = oContentEditor.MergeContentTemplate(oContent, oInstance);
            string sContext = XIConstant.Lead_Transfer;
            oEmail.EmailID = sEmail;
            oEmail.From = oContent.sFrom;
            oEmail.Bcc = oContent.sBCC;
            oEmail.cc = oContent.sCC;
            oEmail.sSubject = oContent.sSubject;// + oInstance.AttributeI("id").sValue;

            oCResult.oTraceStack.Add(new CNV { sName = "Mail Sending started", sValue = "Sending mail started, email:" + oEmail.EmailID + "" });
            //var oMailResult = oEmail.Sendmail(oContent.OrganizationID, Result.oResult.ToString(), null, 0, sContext, oBOI.AttributeI("id").iValue, "", 0, oContent.bIsBCCOnly);//send mail with attachments
            var oMailResult = oEmail.Sendmail(oContent.OrganizationID, Result.oResult.ToString(), null, 0, sContext, 44541, null, 0, oContent.bIsBCCOnly);

            oCResult.oTraceStack.Add(new CNV { sName = "Lead Transfer", sValue = "Lead Transferred" });
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            oBOI.BOD = oBOD;
            if (oMailResult.bOK && oMailResult.oResult != null)
            {
                //insert  Lead life cycle event
                XIIBO oBOILifeCycle = new XIIBO();
                XIDBO oBODLifeCycle = new XIDBO();
                oBODLifeCycle = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "LifeCycle", null);
                oBOILifeCycle.BOD = oBODLifeCycle;
                oBOILifeCycle.SetAttribute("FKiLeadID", oBOI.AttributeI("id").sValue);
                oBOILifeCycle.SetAttribute("sFrom", oBOI.AttributeI("iStatus").sResolvedValue);
                oBOILifeCycle.SetAttribute("sTo", "Junk");
                var LifeCycle = oBOILifeCycle.Save(oBOILifeCycle);
                // update lead
                oBOI.SetAttribute("iTransferStatus", "20"); //Transfer Completed
                oBOI.SetAttribute("iTransferRestrict", "10"); //Disabled
                oBOI.SetAttribute("iJunkResaonID", "90");
                oBOI.SetAttribute("iStatus", "50");  //junk status
                var res = oBOI.Save(oBOI);
                if (res.bOK && res.oResult != null)
                {
                    XIIBO oBOITransferTransaction = new XIIBO();
                    XIDBO oBODTransferTransaction = new XIDBO();
                    oBODTransferTransaction = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "TransferTransaction", null);
                    oBOITransferTransaction.BOD = oBODTransferTransaction;
                    oBOITransferTransaction.SetAttribute("FKiLeadID", oBOI.AttributeI("id").sValue);
                    oBOITransferTransaction.SetAttribute("FKi1ClickID", o1ClickID);
                    oBOITransferTransaction.SetAttribute("sContent", Result.oResult.ToString());
                    oBOITransferTransaction.SetAttribute("sReceiver", sReceiver);
                    oBOITransferTransaction.SetAttribute("sEmail", sEmail);
                    var Transaction = oBOITransferTransaction.Save(oBOITransferTransaction);
                }
                oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully to email:" + oEmail.EmailID + "" });
                oXID.SaveErrortoDB(oCResult);
            }
            else
            {
                oBOI.SetAttribute("iTransferStatus", "30"); //Transfer Error
                oBOI.SetAttribute("iTransferRestrict", "0"); //None
                oBOI.Save(oBOI);
                oCResult.oTraceStack.Add(new CNV { sName = "Mail not sended ", sValue = "Mail not sended to email:" + oEmail.EmailID + "" });
                oXID.SaveErrortoDB(oCResult);
            }
            return oMailResult;
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("api/values/LeadTransfer/")]
        public string LeadTransfer()
        {
            oCResult = new CResult();
            XIIXI oIXI = new XIIXI();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                string sNewGUID = Guid.NewGuid().ToString();

                //Load Templates
                string sTemplateID = "Quote searcher";
                XIContentEditors oQuoteSearcher = LoadTemplate(sTemplateID);
                sTemplateID = "Lead Transfer";
                XIContentEditors oKompare = LoadTemplate(sTemplateID);
                XIContentEditors oSLGM = LoadTemplate(sTemplateID);

                //Get Mails
                string SLGM = GetMails("LeadTransferMail");
                string QuoteSearcher = GetMails("LeadTransferMail");
                string Kompare = GetMails("LeadTransferMail");

                XID1Click o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Transfer transaction", null);
                XID1Click o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                //Get BO Definition
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                o1ClickC.BOD = oBOD;
                //Get Headings of 1-Click
                //o1ClickC.Get_1ClickHeadings();
                Dictionary<string, XIIBO> oRes = o1ClickC.OneClick_Execute();
                sReceiver = oRes.Values.Select(x => x.AttributeI("sreceiver").sValue).FirstOrDefault();
                string sOneClickName = "Lead Transfer";
                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, sOneClickName, null);
                o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                //Get BO Definition
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                o1ClickC.BOD = oBOD;
                //Get Headings of 1-Click
                //o1ClickC.Get_1ClickHeadings();
                oRes = o1ClickC.OneClick_Execute();
                XIIBO oBOI = new XIIBO();
                for (int i = 1; i <= oRes.Values.Count(); i++)
                {
                    oBOI = oRes.ElementAt(i).Value;
                    XIBOInstance oBOIns = new XIBOInstance();
                    List<XIIBO> nBOI = new List<XIIBO>();
                    if (oBOI.iBODID == 0)
                    {
                        oBOI.iBODID = o1ClickC.BOID;
                    }
                    nBOI.Add(oBOI);
                    oBOIns.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                    oBOIns.oStructureInstance[oBOD.Name.ToLower()] = nBOI;
                    CResult oMailResult = new CResult();
                    if (sReceiver == "slgm")
                    {
                        var oLIst = oIXI.BOI("QS Instance", oBOI.AttributeI("FKiQSInstanceID").sValue);  //Quote searcher
                        oBOIns = oLIst.Structure("NotationStructure").XILoad();
                        oMailResult = SendLeadMail(oQuoteSearcher, oBOIns, oBOI, QuoteSearcher, o1ClickC.ID.ToString());
                        sReceiver = "quote searcher";
                    }
                    else
                    {
                        oMailResult = SendLeadMail(oSLGM, oBOIns, oBOI, SLGM, o1ClickC.ID.ToString()); //SLGM
                        sReceiver = "slgm";
                    }
                    oMailResult = SendLeadMail(oKompare, oBOIns, oBOI, Kompare, o1ClickC.ID.ToString());  //Kompare
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.oTraceStack.Add(new CNV { sName = "Lead Transfer", sValue = "Error: In Lead Transfer Method" + oCResult.sMessage });
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oXID.SaveErrortoDB(oCResult);
            }
            return "";
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("DynamicDefinition")]
        public async Task<IHttpActionResult> DynamicDefinition(HttpRequestMessage Request)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";
            List<CNV> oTraceInfo = new List<CNV>();
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            try
            {
                string rawMessage = await Request.Content.ReadAsStringAsync();
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                bool bDefDone = false;
                int iQSDID = 0;
                Guid QSDXIGUID = Guid.Empty;
                Guid QSXIGUID = Guid.Empty;
                bool bPreDefinedQSD = false;
                int FKiDefaultStepDID = 0;
                Guid FKiDefaultStepDIDXIGUID = Guid.Empty;
                List<CNV> MappedNVs = new List<CNV>();
                //var iBODID = oParams.Where(m => m.sName.ToLower() == "ibodid").Select(m => m.sValue).FirstOrDefault();
                //var iBOIID = oParams.Where(m => m.sName.ToLower() == "iboiid").Select(m => m.sValue).FirstOrDefault();
                //var iBODID = "4595";
                //var iBOIID = "1";
                //oTraceInfo.Add(new CNV
                //{
                //    sValue = "DynamicDefinition Method called for Mandatory parameter definition uid is: " + iBODID + " and instance uid is: " + iBOIID
                //});
                //if (!string.IsNullOrEmpty(iBODID) && !string.IsNullOrEmpty(iBOIID))
                //{
                XIConfigs oConfig = new XIConfigs();
                Guid QSDStepXIGUID = Guid.Empty;
                int iQSStepDID = 0;
                Guid QSDSecXIGUID = Guid.Empty;
                int iQSSecDID = 0;
                var sSubject = string.Empty;
                var sBody = string.Empty;
                var sFrom = string.Empty;
                var iSource = string.Empty;
                string FKiOrgID = string.Empty;
                var oFieldDBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldOrigin_T");
                var oMapBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XILeadImportMapping");
                //var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID);
                //var oBOI = oXI.BOI(oBOD.Name, iBOIID);
                //if (oBOI != null && oBOI.Attributes.Count() > 0)
                //{
                //sSubject = oBOI.AttributeI("sHeader").sValue;
                //sFrom = oBOI.AttributeI("sFrom").sValue;
                FKiOrgID = "19";// oBOI.AttributeI("FKiOrgID").sValue;
                var sName = string.Empty;
                XID1Click o1Click = new XID1Click();
                o1Click.BOIDXIGUID = new Guid("77227E41-917A-4476-A549-BEBB6D3A593E");
                o1Click.Query = "select * from XIGenericMapping_T where FKiOrgID = " + FKiOrgID + " ";
                var Response = o1Click.OneClick_Run();
                if (Response != null && Response.Count() > 0)
                {
                    foreach (var map in Response.Values.ToList())
                    {
                        var sMapName = map.AttributeI("sName").sValue;
                        if (!string.IsNullOrEmpty(sMapName))
                        {
                            oTraceInfo.Add(new CNV
                            {
                                sValue = "Inside For loop : " + sMapName
                            });
                            var sQSDXIGUID = map.AttributeI("sValue").sValue;
                            oTraceInfo.Add(new CNV
                            {
                                sValue = "QS GUID : " + sQSDXIGUID
                            });
                            iSource = map.AttributeI("FKiSourceID").sValue;
                            oTraceInfo.Add(new CNV
                            {
                                sValue = "SourceID : " + iSource
                            });
                            if (Guid.TryParse(sQSDXIGUID, out QSXIGUID))
                            {
                                bPreDefinedQSD = true;
                            }
                            else if (!string.IsNullOrEmpty(sQSDXIGUID))
                            {
                                bPreDefinedQSD = true;
                                sName = sQSDXIGUID;
                            }
                        }
                    }
                }
                oTraceInfo.Add(new CNV
                {
                    sValue = "QS definition code is: " + sName
                });
                XIDQS oQSD = new XIDQS();
                if (QSXIGUID != null && QSXIGUID != Guid.Empty)
                {
                    oTraceInfo.Add(new CNV
                    {
                        sValue = "Identifying GUID: " + QSXIGUID
                    });
                    oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, QSXIGUID.ToString());
                }
                else
                {
                    oTraceInfo.Add(new CNV
                    {
                        sValue = "in Else part Identifying Name: " + sName
                    });
                    oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, sName);
                }
                if (oQSD != null && oQSD.XIGUID != null && oQSD.XIGUID != Guid.Empty)
                {
                    QSDXIGUID = oQSD.XIGUID;
                    QSXIGUID = oQSD.XIGUID;
                    if (oQSD.Steps != null && oQSD.Steps.Values.FirstOrDefault() != null)
                    {
                        QSDStepXIGUID = oQSD.Steps.Values.FirstOrDefault().XIGUID;
                        if (oQSD.Steps.Values.FirstOrDefault().Sections != null && oQSD.Steps.Values.FirstOrDefault().Sections.Values.FirstOrDefault() != null)
                        {
                            QSDSecXIGUID = oQSD.Steps.Values.FirstOrDefault().Sections.Values.FirstOrDefault().XIGUID;
                        }
                    }
                }
                if (bPreDefinedQSD)
                {
                    o1Click = new XID1Click();
                    o1Click.BOIDXIGUID = new Guid("E45A3D8E-9F20-49AF-9C4D-494067D8AC2D");
                    o1Click.Query = "select * from XILeadImportMapping_T where FKiQSDIDXIGUID = '" + QSXIGUID + "'";
                    Response = o1Click.OneClick_Run();
                    if (Response != null && Response.Count() > 0)
                    {
                        foreach (var items in Response.Values.ToList())
                        {
                            MappedNVs.Add(new CNV
                            {
                                sName = items.AttributeI("sName").sValue,
                                sValue = items.AttributeI("FKiFieldOriginIDXIGUID").sValue,
                                sLabel = items.AttributeI("sCode").sValue
                            });
                        }
                    }
                }
                //}
                //}
                string sFname = string.Empty;
                string sLname = string.Empty;
                string sMob = string.Empty;
                string sEmail = string.Empty;
                string sPostcode = string.Empty;
                string dDob = string.Empty;
                List<CNV> oRisk = new List<CNV>();
                List<string> NVs = new List<string>();
                if ((iQSDID > 0 || (QSDXIGUID != null && QSDXIGUID != Guid.Empty)) && (iQSStepDID > 0 || (QSDStepXIGUID != null && QSDStepXIGUID != Guid.Empty)) && (iQSSecDID > 0 || (QSDSecXIGUID != null && QSDSecXIGUID != Guid.Empty)))
                {
                    //if (!string.IsNullOrEmpty(sSubject) && sSubject.Contains("Motor Trade (0 NCB) (SVO)"))
                    //{
                    //}
                    //else
                    //{
                    sBody = rawMessage;
                    sName = string.Empty;
                    string sValue = string.Empty;
                    if (!string.IsNullOrEmpty(sBody))
                    {
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(sBody.ToString());
                        XmlNodeList nodes = xDoc.SelectNodes("//lead");
                        foreach (XmlNode node in nodes[0].ChildNodes)
                        {
                            sName = node.Name;
                            sValue = node.InnerText;
                            if (sName.ToLower() == "firstname")
                                sFname = node.InnerText;
                            if (sName.ToLower() == "lastname")
                                sLname = node.InnerText;
                            if (sName.ToLower() == "phone1")
                                sMob = node.InnerText;
                            if (sName.ToLower() == "email")
                                sEmail = node.InnerText;
                            if (sName.ToLower() == "dobday")
                                dDob = node.InnerText;

                            if (sName.ToLower() == "dobmonth")
                                dDob = dDob + "-" + node.InnerText;

                            if (sName.ToLower() == "dobyear")
                            {
                                dDob = dDob + "-" + node.InnerText;
                                sValue = dDob;
                            }
                            if (sName.ToLower() == "postcode")
                                sPostcode = node.InnerText;

                            var FKiFieldOriginIDXIGUID = Guid.Empty;
                            var oFieldD = new XIDFieldOrigin();
                            var MappedNV = MappedNVs.Where(m => m.sName.ToLower() == sName.ToLower()).FirstOrDefault();
                            if (MappedNV == null)
                            {
                                oCResult.sMessage = "Mapping NV not found for:" + sName;
                                oDefBase.SaveErrortoDB(oCResult);
                                oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                if (oFieldD != null && oFieldD.ID > 0)
                                {
                                    oRisk.Add(new CNV
                                    {
                                        sName = sName,
                                        sValue = sValue,
                                        sType = FKiFieldOriginIDXIGUID.ToString()
                                    });
                                }
                            }
                            else
                            {
                                FKiFieldOriginIDXIGUID = new Guid(MappedNV.sValue);
                            }
                            oRisk.Add(new CNV
                            {
                                sName = sName,
                                sValue = sValue,
                                sType = FKiFieldOriginIDXIGUID.ToString()
                            });
                        }
                    }
                    else
                    {
                        oTraceInfo.Add(new CNV
                        {
                            sValue = "Body is null for instance: "// + iBOIID
                        });
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                    //}
                    //var Fieldidguid = "860702CD-1D09-4E09-946D-714F595D0931";
                    //if (!string.IsNullOrEmpty(iSource))
                    //{
                    //    oRisk.Add(new CNV
                    //    {
                    //        sName = "iMTSource",
                    //        sValue = iSource,
                    //        sType = Fieldidguid.ToString()
                    //    });
                    //}
                }
                else
                {
                    oTraceInfo.Add(new CNV
                    {
                        sValue = "Data parsing falied for:" + QSDXIGUID + "_" + QSDStepXIGUID + "_" + QSDSecXIGUID
                    });
                }
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    bDefDone = true;
                }
                string sLeadInfo = sFname + "-" + sLname + "-" + sMob + "-" + sPostcode + "-" + sEmail;
                if (bDefDone && (iQSDID > 0 || (QSDXIGUID != null && QSDXIGUID != Guid.Empty)))
                {
                    string sGUID = Guid.NewGuid().ToString();
                    oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, QSDXIGUID.ToString(), "", sGUID, 0, 0);
                    oTraceInfo.Add(new CNV
                    {
                        sValue = "QS Definition loaded from Cache"
                    });
                    XIDQS oQSDC = (XIDQS)oQSD.Clone(oQSD);
                    int iOrgID = 0;
                    int.TryParse(FKiOrgID, out iOrgID);
                    oCR = oXI.CreateQSI(null, QSDXIGUID.ToString(), null, null, 0, 0, null, 0, null, 0, sGUID, null);
                    oTraceInfo.Add(new CNV
                    {
                        sValue = "QS Instance is created"
                    });
                    var oQSInstance = (XIIQS)oCR.oResult;
                    oQSInstance.QSDefinition = oQSDC;
                    oTraceInfo.Add(new CNV
                    {
                        sValue = "Step instance loaded"
                    });
                    var RiskBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T");
                    if (oRisk != null && oRisk.Count() > 0)
                    {
                        foreach (var risk in oRisk)
                        {
                            XIIBO oRiskI = new XIIBO();
                            oRiskI.BOD = RiskBOD;
                            oRiskI.SetAttribute("sDerivedValue", risk.sValue);
                            var oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, null, risk.sType);
                            if (oFieldD != null && oFieldD.XIGUID != null && oFieldD.XIGUID != Guid.Empty)
                            {
                                if (oFieldD.bIsOptionList)
                                {
                                    var optionValue = oFieldD.FieldOptionList.Where(m => m.sOptionName.ToLower() == risk.sValue.ToLower()).FirstOrDefault();
                                    if (optionValue != null)
                                    {
                                        var iOptValue = optionValue.sOptionValue;
                                        oRiskI.SetAttribute("sValue", iOptValue);
                                    }
                                }
                                else
                                {
                                    oRiskI.SetAttribute("sValue", risk.sValue);
                                }
                            }
                            oRiskI.SetAttribute("FKiFieldOriginIDXIGUID", risk.sType);
                            oRiskI.SetAttribute("FKiQSInstanceIDXIGUID", oQSInstance.XIGUID.ToString());
                            oRiskI.Save(oRiskI);
                        }
                    }
                    oTraceInfo.Add(new CNV
                    {
                        sValue = "Lead creation statred for:" + sLeadInfo + "-" + oQSInstance.XIGUID
                    });
                    var oLeadBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Lead");
                    XIIBO oLeadI = new XIIBO();
                    oLeadI.BOD = oLeadBOD;
                    oLeadI.SetAttribute("sFirstName", sFname);
                    oLeadI.SetAttribute("sLastName", sLname);
                    oLeadI.SetAttribute("sMob", sMob);
                    oLeadI.SetAttribute("sPostCode", sPostcode);
                    oLeadI.SetAttribute("sEmail", sEmail);
                    oLeadI.SetAttribute("FKiQSInstanceIDXIGUID", oQSInstance.XIGUID.ToString());
                    oLeadI.SetAttribute("iStatus", "0");
                    oLeadI.SetAttribute("sName", sFname);
                    oLeadI.SetAttribute("dDOB", dDob);
                    oLeadI.SetAttribute("FKiOrgID", FKiOrgID.ToString());
                    if (!string.IsNullOrEmpty(iSource))
                    {
                        oLeadI.SetAttribute("FKiSourceID", iSource);
                    }
                    //oLeadI.oSignalR = oSignalR;
                    oCR = oLeadI.Save(oLeadI, false);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        int iLeadID = 0;
                        oLeadI = (XIIBO)oCR.oResult;
                        var LeadID = oLeadI.AttributeI("id").sValue;
                        int.TryParse(LeadID, out iLeadID);
                        if (iLeadID > 0)
                        {
                            oTraceInfo.Add(new CNV
                            {
                                sValue = "Lead creation success for:" + sFname + "-" + sLname + "-" + sMob + "-" + sPostcode + "-" + sEmail + "-" + oQSInstance.XIGUID
                            });
                            var CommMatchBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XCommunicationMatch");
                            XIIBO oCommMatch = new XIIBO();
                            oCommMatch.BOD = CommMatchBOD;
                            oCommMatch.SetAttribute("FKiBODID", "717");
                            oCommMatch.SetAttribute("FKiBOIID", iLeadID.ToString());
                            oCommMatch.SetAttribute("FKiCommunicationID", "0");
                            oCR = oCommMatch.Save(oCommMatch);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oTraceInfo.Add(new CNV
                                {
                                    sValue = "Communication linked successfully to Lead"
                                });
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV
                                {
                                    sValue = "Communication linking to Lead is falied"
                                });
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            oTraceInfo.Add(new CNV
                            {
                                sValue = "Lead creation failed for:" + sFname + "-" + sLname + "-" + sMob + "-" + sPostcode + "-" + sEmail + "-" + oQSInstance.XIGUID
                            });
                        }
                    }
                    else
                    {
                        oTraceInfo.Add(new CNV
                        {
                            sValue = "Lead creation failed for:" + sFname + "-" + sLname + "-" + sMob + "-" + sPostcode + "-" + sEmail + "-" + oQSInstance.XIGUID
                        });
                    }
                }
                else
                {
                    oTraceInfo.Add(new CNV
                    {
                        sValue = "QS instance creation falied for:" + sLeadInfo
                    });
                }
                //}
                //else
                //{
                //    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                //    oTraceInfo.Add(new CNV
                //    {
                //        sValue = "Mandatory Params: iBODID " + iBODID + " or iBOIID:" + iBOIID + " are missing"
                //    });
                //}
                oCResult.oTraceStack = oTraceInfo;
                oDefBase.SaveErrortoDB(oCResult);
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = QSDXIGUID;
                }
                return Ok();
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
                oTraceInfo.Add(new CNV
                {
                    sValue = oCResult.sMessage
                });
                oCResult.oTraceStack = oTraceInfo;
                oDefBase.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return Ok();
        }
        private bool MergeTemplate()
        {
            return true;
        }

       
        //[XIDNAAuthorizationFilter]
        [AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("GenerateLeadAsyncFleet")]
        public async Task<IHttpActionResult> GenerateLeadAsyncFleet(HttpRequestMessage Request)
        {
            try
            {
                string rawMessage = await Request.Content.ReadAsStringAsync();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(rawMessage.ToString());

                var sEmail = xDoc.GetElementsByTagName("email").Item(0).InnerText;

                var sKey = xDoc.GetElementsByTagName("key").Item(0).InnerText;
                var oRes = KeyVerification(sKey);
                if (!string.IsNullOrEmpty(oRes))
                {
                    //TODO Store Lead in your db
                    XIIBO oBOI = new XIIBO();
                    XIInfraCache oCache = new XIInfraCache();
                    XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XICommEMailIn");
                    oBOI.BOD = oBOD;
                    oBOI.SetAttribute("sContent", rawMessage);
                    oBOI.SetAttribute("iStatus", "10");
                    oBOI.SetAttribute("iType", "20");
                    oBOI.SetAttribute("sTo", sEmail);
                    oBOI.SetAttribute("sHeader", "Lead Gen");
                    oBOI.SetAttribute("FKiOrgID", oRes); //19
                    var res = oBOI.Save(oBOI);

                    //XIInfraCommunicationComponent ocomm = new XIInfraCommunicationComponent();
                    //List<CNV> oParams = new List<CNV>();
                    //oParams.Add(new CNV { sName = "ibodid", sValue = "4546" });
                    //oParams.Add(new CNV { sName = "iboiid", sValue = "99" });
                    //DynamicDefinition(oParams);
                }
                else
                {

                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok();
            }
        }
        [AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("GenerateLeadAsyncMH")]
        public async Task<IHttpActionResult> GenerateLeadAsyncMH(HttpRequestMessage Request)
        {
            try
            {
                string rawMessage = await Request.Content.ReadAsStringAsync();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(rawMessage.ToString());

                var sEmail = xDoc.GetElementsByTagName("email").Item(0).InnerText;

                var sKey = xDoc.GetElementsByTagName("key").Item(0).InnerText;
                var oRes = KeyVerification(sKey);
                if (!string.IsNullOrEmpty(oRes))
                {
                    //TODO Store Lead in your db
                    XIIBO oBOI = new XIIBO();
                    XIInfraCache oCache = new XIInfraCache();
                    XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XICommEMailIn");
                    oBOI.BOD = oBOD;
                    oBOI.SetAttribute("sContent", rawMessage);
                    oBOI.SetAttribute("iStatus", "10");
                    oBOI.SetAttribute("iType", "20");
                    oBOI.SetAttribute("sTo", sEmail);
                    oBOI.SetAttribute("sHeader", "Motor home");
                    oBOI.SetAttribute("FKiOrgID", oRes); //19
                    var res = oBOI.Save(oBOI);

                    //XIInfraCommunicationComponent ocomm = new XIInfraCommunicationComponent();
                    //List<CNV> oParams = new List<CNV>();
                    //oParams.Add(new CNV { sName = "ibodid", sValue = "4546" });
                    //oParams.Add(new CNV { sName = "iboiid", sValue = "99" });
                    //DynamicDefinition(oParams);
                }
                else
                {

                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok();
            }
        }
        [AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("GenerateLeadAsyncMT")]
        public async Task<IHttpActionResult> GenerateLeadAsyncMT(HttpRequestMessage Request)
        {
            try
            {
                string rawMessage = await Request.Content.ReadAsStringAsync();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(rawMessage.ToString());

                var sEmail = xDoc.GetElementsByTagName("email").Item(0).InnerText;

                var sKey = xDoc.GetElementsByTagName("key").Item(0).InnerText;
                var oRes = KeyVerification(sKey);
                if (!string.IsNullOrEmpty(oRes))
                {
                    //TODO Store Lead in your db
                    XIIBO oBOI = new XIIBO();
                    XIInfraCache oCache = new XIInfraCache();
                    XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XICommEMailIn");
                    oBOI.BOD = oBOD;
                    oBOI.SetAttribute("sContent", rawMessage);
                    oBOI.SetAttribute("iStatus", "10");
                    oBOI.SetAttribute("iType", "20");
                    oBOI.SetAttribute("sTo", sEmail);
                    oBOI.SetAttribute("sHeader", "Motor trade");
                    oBOI.SetAttribute("FKiOrgID", oRes); //19
                    var res = oBOI.Save(oBOI);

                    //XIInfraCommunicationComponent ocomm = new XIInfraCommunicationComponent();
                    //List<CNV> oParams = new List<CNV>();
                    //oParams.Add(new CNV { sName = "ibodid", sValue = "4546" });
                    //oParams.Add(new CNV { sName = "iboiid", sValue = "99" });
                    //DynamicDefinition(oParams);
                }
                else
                {

                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok();
            }
        }       
        private static string KeyVerification(string skey)
        {
            try
            {
                XIIBO oBOI = new XIIBO();
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oIXI = new XIIXI();
                List<CNV> oWhrParams = new List<CNV>();
                oWhrParams.Add(new CNV { sName = "sPublicKey", sValue = skey });
                var oList = oIXI.BOI("AppClients", null, "FKiOrgID", oWhrParams);
                if (oList != null && oList.Attributes.Count() > 0)
                {
                    return oList.AttributeI("FKiOrgID").sValue;
                }
                return "";
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}