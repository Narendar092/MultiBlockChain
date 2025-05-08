Delete [XIBOScript_T] where xiguid='F8B8C9B7-32F0-46FA-8B79-6ACD19A413E2'
INSERT INTO [XIBOScript_T] ([ID],
 [FKiBOID],
 [FKiBOAttributeID],
 [sExecutionType],
 [sName],
 [sDescription],
 [sScript],
 [sType],
 [sLanguage],
 [StatusTypeID],
 [CreatedTime],
 [CreatedBy],
 [CreatedBySYSID],
 [UpdatedBy],
 [UpdatedBySYSID],
 [UpdatedTime],
 [FKiApplicationID],
 [sMethodName],
 [sVersion],
 [iOrder],
 [OrganisationID],
 [izXDeleted],
 [zXCrtdBy],
 [zXCrtdWhn],
 [zXUpdtdBy],
 [zXUpdtdWhn],
 [sLevel],
 [sCategory],
 [sClassification],
 [XIGUID],
 [XIDeleted],
 [XICreatedBy],
 [XIUpdatedBy],
 [XICreatedWhen],
 [XIUpdatedWhen],
 [sTag],
 [FKiTagSpaceID],
 [FKiVersionID],
 [FKiBOIDXIGUID],
 [FKiBOAttributeIDXIGUID],
 [FKiApplicationIDXIGUID],
 [FKiTagSpaceIDXIGUID],
 [FKiVersionIDXIGUID]) Values(N'20543',
 N'666',
 N'17595',
 NULL,
 N'Dynamic inbound',
 N'Dynamic inbound C# method',
 N'public static CResult DynamicDefinition(List<CNV> oParams, iSiganlR oSignalR)
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
                var iBODID = oParams.Where(m => m.sName.ToLower() == "ibodid").Select(m => m.sValue).FirstOrDefault();
                var iBOIID = oParams.Where(m => m.sName.ToLower() == "iboiid").Select(m => m.sValue).FirstOrDefault();
                oTraceInfo.Add(new CNV
                {
                    sValue = "DynamicDefinition Method called for Mandatory parameter definition uid is: " + iBODID + " and instance uid is: " + iBOIID
                });
                if (!string.IsNullOrEmpty(iBODID) && !string.IsNullOrEmpty(iBOIID))
                {
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
                    var iType = string.Empty;
                    var oFieldDBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldOrigin_T");
                    var oMapBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XILeadImportMapping");
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID);
                    var oBOI = oXI.BOI(oBOD.Name, iBOIID);
                    if (oBOI != null && oBOI.Attributes.Count() > 0)
                    {
                        sSubject = oBOI.AttributeI("sHeader").sValue;
                        sFrom = oBOI.AttributeI("sFrom").sValue;
                        FKiOrgID = oBOI.AttributeI("FKiOrgID").sValue;
                        iType = oBOI.AttributeI("iType").sValue;
                        if (sFrom.Contains(''<''))
                        {
                            var sFromIndex = sFrom.IndexOf("<");
                            sFrom = sFrom.Substring(sFromIndex + 1, sFrom.Length - sFromIndex - 1);
                            if (sFrom.EndsWith(">"))
                            {
                                sFrom = sFrom.Substring(0, sFrom.Length - 1);
                            }
                        }
                        if (sSubject.Length >= 128)
                        {
                            sSubject = sSubject.Substring(0, 127);
                        }
                        if (!string.IsNullOrEmpty(sSubject))
                        {
                            var sName = sSubject.Trim();
                            XID1Click o1Click = new XID1Click();
                            o1Click.BOIDXIGUID = new Guid("77227E41-917A-4476-A549-BEBB6D3A593E");
                            o1Click.Query = "select * from XIGenericMapping_T where FKiOrgID = " + FKiOrgID + " ";
                            var Response = o1Click.OneClick_Run();
                            if (Response != null && Response.Count() > 0)
                            {
                                foreach (var map in Response.Values.ToList())
                                {
                                    var sMapName = map.AttributeI("sName").sValue;
                                    if (sSubject.ToLower().StartsWith(sMapName.ToLower()))
                                    {
                                        var sQSDXIGUID = map.AttributeI("sValue").sValue;
                                        iSource = map.AttributeI("FKiSourceID").sValue;
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
                                oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, QSXIGUID.ToString());
                            }
                            else
                            {
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
                                if (oQSD.FKiDefaultStepDIDXIGUID == null || oQSD.FKiDefaultStepDIDXIGUID == Guid.Empty)
                                {
                                    var DefaultStep = oQSD.Steps.Values.ToList().Where(m => m.sName.ToLower() == "Default Step".ToLower()).FirstOrDefault();
                                    if (DefaultStep != null && DefaultStep.XIGUID != null && DefaultStep.XIGUID != Guid.Empty)
                                    {
                                        FKiDefaultStepDIDXIGUID = DefaultStep.XIGUID;
                                        var DefaultSecD = DefaultStep.Sections.Values.ToList().FirstOrDefault();
                                        if (DefaultSecD != null && DefaultSecD.XIGUID != null && DefaultSecD.XIGUID != Guid.Empty)
                                        {
                                            QSDSecXIGUID = DefaultSecD.XIGUID;
                                            iQSSecDID = DefaultSecD.ID;
                                        }
                                    }
                                }
                                else
                                    FKiDefaultStepDIDXIGUID = oQSD.FKiDefaultStepDIDXIGUID;
                            }
                            else
                            {
                                oQSD.sName = sName;
                                oQSD.sDescription = sName;
                                oCR = oConfig.Save_QuestionSet(oQSD);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    var oQSDBOI = (XIIBO)oCR.oResult;
                                    var QSDUID = oQSDBOI.AttributeI("xiguid").sValue;
                                    Guid.TryParse(QSDUID, out QSDXIGUID);
                                    if (QSDXIGUID == null || QSDXIGUID == Guid.Empty)
                                    {
                                        var QSDID = oQSDBOI.AttributeI("id").sValue;
                                        int.TryParse(QSDID, out iQSDID);
                                    }
                                    oTraceInfo.Add(new CNV
                                    {
                                        sValue = "QS definition saved: " + QSDXIGUID
                                    });
                                    XIDQSStep oStepD = new XIDQSStep();
                                    oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                    oStepD.FKiQSDefintionID = iQSDID;
                                    oStepD.sName = "Step1";
                                    oStepD.sDisplayName = "Step";
                                    oStepD.iDisplayAs = 20;
                                    oStepD.StatusTypeID = 10;
                                    oStepD.bIsMerge = true;
                                    oStepD.sIsHidden = "off";
                                    oStepD.bIsCopy = true;
                                    oStepD.bIsHistory = true;
                                    oStepD.bIsSaveNext = true;
                                    oStepD.bIsBack = true;
                                    oStepD.bIsReload = true;
                                    oStepD.bIsAutoSaving = true;
                                    oCR = oConfig.Save_QuestionSetStep(oStepD);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        var oQSDStepBOI = (XIIBO)oCR.oResult;
                                        var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                        Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                        if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                        {
                                            var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                            int.TryParse(QSStepDID, out iQSStepDID);
                                        }
                                        oTraceInfo.Add(new CNV
                                        {
                                            sValue = "QS step definition saved: " + QSDStepXIGUID
                                        });
                                        XIDQSSection oSecD = new XIDQSSection();
                                        oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                        oSecD.FKiStepDefinitionID = iQSStepDID;
                                        oSecD.iDisplayAs = 30;
                                        oSecD.sIsHidden = "off";
                                        oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            var oQSDSecBOI = (XIIBO)oCR.oResult;
                                            var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                            Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                            if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                            {
                                                var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                int.TryParse(QSSecDID, out iQSSecDID);
                                            }
                                            oTraceInfo.Add(new CNV
                                            {
                                                sValue = "QS step section definition saved: " + QSDSecXIGUID
                                            });
                                        }
                                    }
                                }
                            }
                            if (bPreDefinedQSD)
                            {
                                o1Click = new XID1Click();
                                o1Click.BOIDXIGUID = new Guid("E45A3D8E-9F20-49AF-9C4D-494067D8AC2D");
                                o1Click.Query = "select * from XILeadImportMapping_T where FKiQSDIDXIGUID = ''" + QSXIGUID + "''";
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
                        }
                    }
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
                        if (iType == "10")
                        {
                            if (!string.IsNullOrEmpty(sSubject) && sSubject.Contains("PC NCB Dream Cars") || sSubject.Contains("Motor Trade (0 NCB) (SVO)") || sSubject.Contains("Motor Trade (PC NCB) (Sales)") || sSubject.Contains("Motor Trade (PC NCB) (Skilled)") || sSubject.Contains("Motor Trade (PC NCB) (BRCD)") || sSubject.Contains("Motor Trade (MT NCB) (Skilled)") || sSubject.Contains("Motor Trade (MT NCB) (BRCD)") || sSubject.Contains("Motor Trade (MT NCB) (SVO)") || sSubject.Contains("Motor Trade (0 NCB) (Sales)") || sSubject.Contains("Motor Trade (0 NCB) (Skilled)") || sSubject.Contains("Motor Trade (0 NCB) (BRCD)") || sSubject.Contains("(MT NCB) (Sales)") || sSubject.Contains("New submission") || sSubject.Contains("Motor Trade (PC NCB) (SVO)"))
                            {
                                sBody = oBOI.AttributeI("sContent").sValue;
                                string sName = string.Empty;
                                string sValue = string.Empty;
                                var td3 = "";
                                var td4 = "";
                                if (!string.IsNullOrEmpty(sBody))
                                {
                                    var TRContent = Regex.Matches(sBody.Trim(), @"(?<1><TR[^>]*>\s*<td.*?</tr>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                    if (!string.IsNullOrEmpty(sSubject) && sSubject.Contains("New submission"))
                                    {
                                        int j = 0;
                                        foreach (var tr in TRContent)
                                        {
                                            j++;
                                            var Content = tr.ToString();
                                            if (Content.Contains("<tr bgcolor=\"#EAF2FA\">"))
                                            {
                                                if (Content.Contains("<table width=\"100%"))
                                                {
                                                    td3 = Content.Replace("<tr>", "").Replace("<td>", "").Replace("<table width=\"100%\" border=\"0\" cellpadding=\"5\" cellspacing=\"0\" bgcolor=\"#FFFFFF\">", "").Replace("<tr bgcolor=\"#EAF2FA\">", "").Replace("<td colspan=\"2\">", "").Replace("<font style=\"font-family: sans-serif; font-size:12px;\">", "").Replace("<strong>", "").Replace("</strong>", "").Replace("</font>", "").Replace("</td>", "").Replace("</tr>", "").Trim();
                                                }
                                                else
                                                {
                                                    td3 = Content.Replace("<tr bgcolor=\"#EAF2FA\">", "").Replace("<td colspan=\"2\">", "").Replace("<font style=\"font-family: sans-serif; font-size:12px;\">", "").Replace("<strong>", "").Replace("</strong>", "").Replace("</font>", "").Replace("</td>", "").Replace("</tr>", "").Trim();
                                                }
                                            }
                                            if (Content.Contains("<tr bgcolor=\"#FFFFFF\">"))
                                            {
                                                if (Content.Contains("<a href="))
                                                {
                                                    td4 = Content.Replace("<tr bgcolor=\"#FFFFFF\">", "").Replace("<td width=\"20\">&nbsp;", "").Replace("</td>", "").Replace("<td>", "").Replace("<font style=\"font-family: sans-serif; font-size:12px;\">", "").Replace("</font>", "").Replace("</td>", "").Replace("</tr>", "").Trim();
                                                    if (td4.IndexOf("<a ") != -1)
                                                    {
                                                        td4 = td4.Substring(td4.IndexOf(">") + 1, td4.IndexOf("</a>") - (td4.IndexOf(">") + 1));
                                                    }
                                                }
                                                else
                                                {
                                                    td4 = Content.Replace("<tr bgcolor=\"#FFFFFF\">", "").Replace("<td width=\"20\">&nbsp;", "").Replace("<td>", "").Replace("<font style=\"font-family: sans-serif; font-size:12px;\">", "").Replace("</font>", "").Replace("</td>", "").Replace("</td>", "").Replace("</tr>", "").Trim();
                                                }
                                            }
                                            if (j == 1)
                                                sName = td3;
                                            if (j == 2)
                                                sValue = td4;
                                            if (!string.IsNullOrEmpty(sName) && !string.IsNullOrEmpty(sValue))
                                            {
                                                if (sName.ToLower() == "full name" || sName.ToLower() == "first name" || sName.ToLower() == "name" || sName.ToLower() == "name of policyholder" || sName.ToLower() == "policyholder name")
                                                {
                                                    if (sName.ToLower() == "name of policyholder" || sName.ToLower() == "policyholder name")
                                                    {
                                                        if (sValue.Contains('' ''))
                                                        {
                                                            var Valuespliting = sValue.Split('' '');
                                                            sFname = Valuespliting[0].Trim();
                                                            sLname = Valuespliting[1].Trim();
                                                            sValue = sFname;
                                                            if (!string.IsNullOrEmpty(sLname))
                                                            {
                                                                var Surnamefieldguid = "307527F8-ABC4-40F2-ADDB-914528253BE8";
                                                                oRisk.Add(new CNV
                                                                {
                                                                    sName = sName,
                                                                    sValue = sLname,
                                                                    sType = Surnamefieldguid.ToString()
                                                                });
                                                            }
                                                        }
                                                        else
                                                        {
                                                            sFname = sValue;
                                                        }
                                                    }
                                                }
                                                if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone" || sName.ToLower() == "preferred telephone number" || sName.ToLower() == "contact no." || sName.ToLower() == "phone")
                                                {
                                                    sMob = sValue;
                                                }
                                                if (sName.ToLower() == "last name")
                                                {
                                                    sLname = sValue;
                                                }
                                                if (sName.ToLower() == "postcode")
                                                {
                                                    sPostcode = sValue;
                                                }
                                                if (sName.ToLower() == "date of birth of policyholder")
                                                {
                                                    dDob = sValue;
                                                }
                                                if (sName.ToLower() == "email" || sName.ToLower() == "your email")
                                                {
                                                    sEmail = sValue;
                                                }
                                                var FKiFieldOriginIDXIGUID = Guid.Empty;
                                                var oFieldD = new XIDFieldOrigin();
                                                var MappedNV = MappedNVs.Where(m => m.sLabel.ToLower() == sName.ToLower()).FirstOrDefault();
                                                if (MappedNV == null)
                                                {
                                                    oCResult.sMessage = "Mapping NV not found for:" + sName;
                                                    oDefBase.SaveErrortoDB(oCResult);
                                                    oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                    if (oFieldD != null && oFieldD.ID > 0)
                                                    {
                                                        FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                        XIIBO oMapNV = new XIIBO();
                                                        oMapNV.BOD = oMapBOD;
                                                        oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                        oMapNV.SetAttribute("sName", sName);
                                                        oMapNV.SetAttribute("sCode", sName);
                                                        oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                        oCR = oMapNV.Save(oMapNV);
                                                        if (oCR.bOK && oCR.oResult != null) { } else { }
                                                    }
                                                    else
                                                    {
                                                        XIIBO oFO = new XIIBO();
                                                        oFO.BOD = oFieldDBOD;
                                                        oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                                        oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                                        oFO.SetAttribute("sDisplayName", sName);
                                                        oCR = oFO.Save(oFO);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            oFO = (XIIBO)oCR.oResult;
                                                            oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                            if (oFieldD != null && oFieldD.ID > 0)
                                                            {
                                                                FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                            }
                                                            else
                                                            {
                                                                XIIBO oFDI = new XIIBO();
                                                                List<CNV> oWhr = new List<CNV>();
                                                                oWhr.Add(new CNV
                                                                {
                                                                    sName = "sName",
                                                                    sValue = sName.Replace(" ", "")
                                                                });
                                                                oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                                var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                                Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                            }
                                                        }
                                                        XIIBO oMapNV = new XIIBO();
                                                        oMapNV.BOD = oMapBOD;
                                                        oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                        oMapNV.SetAttribute("sName", sName);
                                                        oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                        oCR = oMapNV.Save(oMapNV);
                                                        if (oCR.bOK && oCR.oResult != null) { } else { }
                                                    }
                                                    if (FKiDefaultStepDIDXIGUID != null && FKiDefaultStepDIDXIGUID != Guid.Empty) { }
                                                    else
                                                    {
                                                        XIDQSStep oStepD = new XIDQSStep();
                                                        oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                                        oStepD.FKiQSDefintionID = iQSDID;
                                                        oStepD.sName = "Default Step";
                                                        oStepD.sDisplayName = "Default Step";
                                                        oStepD.iDisplayAs = 20;
                                                        oStepD.StatusTypeID = 10;
                                                        oStepD.bIsMerge = true;
                                                        oStepD.sIsHidden = "off";
                                                        oStepD.iOrder = 100;
                                                        oStepD.bIsSaveNext = true;
                                                        oStepD.bIsBack = true;
                                                        oStepD.bIsCopy = true;
                                                        oStepD.bIsHistory = true;
                                                        oStepD.bIsReload = true;
                                                        oStepD.bIsAutoSaving = true;
                                                        oStepD.iStage = 200;
                                                        oCR = oConfig.Save_QuestionSetStep(oStepD);
                                                        oCache.Clear_DefInCache("IO_Definition_questionset_" + QSDXIGUID.ToString());
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            var oQSDStepBOI = (XIIBO)oCR.oResult;
                                                            var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                                            Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                                            if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                                            {
                                                                var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                                                int.TryParse(QSStepDID, out iQSStepDID);
                                                                FKiDefaultStepDID = iQSStepDID;
                                                            }
                                                            FKiDefaultStepDIDXIGUID = QSDStepXIGUID;
                                                            XIDQSSection oSecD = new XIDQSSection();
                                                            oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                                            oSecD.FKiStepDefinitionID = iQSStepDID;
                                                            oSecD.iDisplayAs = 30;
                                                            oSecD.sIsHidden = "off";
                                                            oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                                            if (oCR.bOK && oCR.oResult != null)
                                                            {
                                                                var oQSDSecBOI = (XIIBO)oCR.oResult;
                                                                var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                                                Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                                                if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                                                {
                                                                    var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                                    int.TryParse(QSSecDID, out iQSSecDID);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                                    oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                                    oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                                    oFieldDef.FKiXIStepDefinitionIDXIGUID = FKiDefaultStepDIDXIGUID;
                                                    oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                                    oFieldDef.FKiXIStepDefinitionID = FKiDefaultStepDID;
                                                    oFieldDef.FKiStepSectionID = iQSSecDID;
                                                    oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        oTraceInfo.Add(new CNV
                                                        {
                                                            sValue = "Field definition saved for Field: " + sName
                                                        });
                                                    }
                                                    else
                                                    {
                                                        oTraceInfo.Add(new CNV
                                                        {
                                                            sValue = "Field definition not saved for Field: " + sName
                                                        });
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                    }
                                                    oRisk.Add(new CNV
                                                    {
                                                        sName = sName,
                                                        sValue = sValue,
                                                        sType = FKiFieldOriginIDXIGUID.ToString()
                                                    });
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
                                                j = 0;
                                                sName = "";
                                                sValue = "";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var tr in TRContent)
                                        {
                                            var trContent = tr.ToString();
                                            var tdRegex = @"<td\b[^>]*?>(?<V>[\s\S]*?)</\s*td>";
                                            var tdContent = Regex.Matches(trContent, tdRegex, RegexOptions.IgnoreCase);
                                            if (tdContent.Count > 1)
                                            {
                                                var tdContent1 = tdContent[0].ToString();
                                                var tdContent2 = tdContent[1].ToString();
                                                if (!string.IsNullOrEmpty(tdContent1) && !string.IsNullOrEmpty(tdContent2))
                                                {
                                                    if (tdContent1.Contains("rgb(237, 244, 255)") || tdContent1.Contains("rgb(247, 250, 255)"))
                                                    {
                                                        if (tdContent2.Contains("rgb(200, 223, 179)") || tdContent2.Contains("rgb(237, 244, 255)") || tdContent2.Contains("rgb(247, 250, 255)"))
                                                        {
                                                            var SpanRegex = @"<span\b[^>]*?>(?<V>[\s\S]*?)</\s*span>";
                                                            var td1SpanContent = Regex.Matches(tdContent1, SpanRegex, RegexOptions.IgnoreCase);
                                                            if (td1SpanContent.Count > 0)
                                                            {
                                                                var regex = new System.Text.RegularExpressions.Regex("<span(.*?)>(.*?)</span>");
                                                                var sNameMatch = regex.Match(td1SpanContent[0].ToString());
                                                                if (sNameMatch.Groups.Count > 2)
                                                                {
                                                                    sName = sNameMatch.Groups[2].Value;
                                                                }

                                                            }
                                                            var td2SpanContent = Regex.Matches(tdContent2, SpanRegex, RegexOptions.IgnoreCase);
                                                            if (td2SpanContent.Count > 0)
                                                            {
                                                                var regex = new System.Text.RegularExpressions.Regex("<span(.*?)>(.*?)</span>");
                                                                var sValueMatch = regex.Match(td2SpanContent[0].ToString());
                                                                if (sValueMatch.Groups.Count > 2)
                                                                {
                                                                    sValue = sValueMatch.Groups[2].Value;
                                                                }
                                                            }
                                                            if (!string.IsNullOrEmpty(sName) && !string.IsNullOrEmpty(sValue))
                                                            {
                                                                if (sName.ToLower() == "full name" || sName.ToLower() == "first name")
                                                                {
                                                                    sFname = sValue;
                                                                }
                                                                if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone")
                                                                {
                                                                    sMob = sValue;
                                                                }
                                                                if (sName.ToLower() == "last name")
                                                                {
                                                                    sLname = sValue;
                                                                }
                                                                if (sName.ToLower() == "postcode")
                                                                {
                                                                    sPostcode = sValue;
                                                                }
                                                                if (sName.ToLower() == "email")
                                                                {
                                                                    sEmail = sValue;
                                                                }
                                                                var FKiFieldOriginIDXIGUID = Guid.Empty;
                                                                var oFieldD = new XIDFieldOrigin();
                                                                var MappedNV = MappedNVs.Where(m => m.sLabel.ToLower() == sName.ToLower()).FirstOrDefault();
                                                                if (MappedNV == null)
                                                                {
                                                                    oCResult.sMessage = "Mapping NV not found for:" + sName;
                                                                    oDefBase.SaveErrortoDB(oCResult);
                                                                    oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                                    if (oFieldD != null && oFieldD.ID > 0)
                                                                    {
                                                                        FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                        XIIBO oMapNV = new XIIBO();
                                                                        oMapNV.BOD = oMapBOD;
                                                                        oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                                        oMapNV.SetAttribute("sName", sName);
                                                                        oMapNV.SetAttribute("sCode", sName);
                                                                        oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                                        oCR = oMapNV.Save(oMapNV);
                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                        {

                                                                        }
                                                                        else
                                                                        {

                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        XIIBO oFO = new XIIBO();
                                                                        oFO.BOD = oFieldDBOD;
                                                                        oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                                                        oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                                                        oFO.SetAttribute("sDisplayName", sName);
                                                                        oCR = oFO.Save(oFO);
                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                        {
                                                                            oFO = (XIIBO)oCR.oResult;
                                                                            oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                                            if (oFieldD != null && oFieldD.ID > 0)
                                                                            {
                                                                                FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                            }
                                                                            else
                                                                            {
                                                                                XIIBO oFDI = new XIIBO();
                                                                                List<CNV> oWhr = new List<CNV>();
                                                                                oWhr.Add(new CNV
                                                                                {
                                                                                    sName = "sName",
                                                                                    sValue = sName.Replace(" ", "")
                                                                                });
                                                                                oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                                                var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                                                Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                                            }
                                                                        }
                                                                        XIIBO oMapNV = new XIIBO();
                                                                        oMapNV.BOD = oMapBOD;
                                                                        oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                                        oMapNV.SetAttribute("sName", sName);
                                                                        oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                                        oCR = oMapNV.Save(oMapNV);
                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                        {

                                                                        }
                                                                        else
                                                                        {

                                                                        }
                                                                    }
                                                                    if (FKiDefaultStepDIDXIGUID != null && FKiDefaultStepDIDXIGUID != Guid.Empty)
                                                                    {

                                                                    }
                                                                    else
                                                                    {
                                                                        XIDQSStep oStepD = new XIDQSStep();
                                                                        oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                                                        oStepD.FKiQSDefintionID = iQSDID;
                                                                        oStepD.sName = "Default Step";
                                                                        oStepD.sDisplayName = "Default Step";
                                                                        oStepD.iDisplayAs = 20;
                                                                        oStepD.StatusTypeID = 10;
                                                                        oStepD.bIsMerge = true;
                                                                        oStepD.sIsHidden = "off";
                                                                        oStepD.iOrder = 100;
                                                                        oStepD.bIsSaveNext = true;
                                                                        oStepD.bIsBack = true;
                                                                        oStepD.bIsCopy = true;
                                                                        oStepD.bIsHistory = true;
                                                                        oStepD.bIsReload = true;
                                                                        oStepD.bIsAutoSaving = true;
                                                                        oStepD.iStage = 200;
                                                                        oCR = oConfig.Save_QuestionSetStep(oStepD);
                                                                        oCache.Clear_DefInCache("IO_Definition_questionset_" + QSDXIGUID.ToString());
                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                        {
                                                                            var oQSDStepBOI = (XIIBO)oCR.oResult;
                                                                            var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                                                            Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                                                            if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                                                            {
                                                                                var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                                                                int.TryParse(QSStepDID, out iQSStepDID);
                                                                                FKiDefaultStepDID = iQSStepDID;
                                                                            }
                                                                            FKiDefaultStepDIDXIGUID = QSDStepXIGUID;
                                                                            XIDQSSection oSecD = new XIDQSSection();
                                                                            oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                                                            oSecD.FKiStepDefinitionID = iQSStepDID;
                                                                            oSecD.iDisplayAs = 30;
                                                                            oSecD.sIsHidden = "off";
                                                                            oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                                                            if (oCR.bOK && oCR.oResult != null)
                                                                            {
                                                                                var oQSDSecBOI = (XIIBO)oCR.oResult;
                                                                                var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                                                                Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                                                                if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                                                                {
                                                                                    var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                                                    int.TryParse(QSSecDID, out iQSSecDID);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                                                    oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                                                    oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                                                    oFieldDef.FKiXIStepDefinitionIDXIGUID = FKiDefaultStepDIDXIGUID;
                                                                    oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                                                    oFieldDef.FKiXIStepDefinitionID = FKiDefaultStepDID;
                                                                    oFieldDef.FKiStepSectionID = iQSSecDID;
                                                                    oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                                                    if (oCR.bOK && oCR.oResult != null)
                                                                    {
                                                                        oTraceInfo.Add(new CNV
                                                                        {
                                                                            sValue = "Field definition saved for Field: " + sName
                                                                        });
                                                                    }
                                                                    else
                                                                    {
                                                                        oTraceInfo.Add(new CNV
                                                                        {
                                                                            sValue = "Field definition not saved for Field: " + sName
                                                                        });
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                    }
                                                                    oRisk.Add(new CNV
                                                                    {
                                                                        sName = sName,
                                                                        sValue = sValue,
                                                                        sType = FKiFieldOriginIDXIGUID.ToString()
                                                                    });
                                                                }
                                                                else
                                                                {
                                                                    FKiFieldOriginIDXIGUID = new Guid(MappedNV.sValue);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (!string.IsNullOrEmpty(tdContent1) && !string.IsNullOrEmpty(tdContent2))
                                                    {
                                                        if (tdContent1.Contains("td style=\"text-indent: 0px; line-height: normal; width: 300px;\"") || tdContent1.Contains("td style=\"line-height: normal; width: 300px; box-sizing: border-box;\""))
                                                        {
                                                            if (tdContent2.Contains("td style=\"text-indent: 0px; line-height: normal; width: 300px;\"") || tdContent2.Contains("td style=\"line-height: normal; width: 300px; box-sizing: border-box;\""))
                                                            {
                                                                var SpanRegex = @"<span\b[^>]*?>(?<V>[\s\S]*?)</\s*span>";
                                                                var td1SpanContent = Regex.Matches(tdContent1, SpanRegex, RegexOptions.IgnoreCase);
                                                                if (td1SpanContent.Count > 0)
                                                                {
                                                                    var regex = new System.Text.RegularExpressions.Regex("<span(.*?)>(.*?)</span>");
                                                                    var sNameMatch = regex.Match(td1SpanContent[0].ToString());
                                                                    if (sNameMatch.Groups.Count > 2)
                                                                    {
                                                                        var sName1 = sNameMatch.Groups[2].Value.ToString();
                                                                        sName = sName1.Replace("<b>", "").Replace("</b>", "");
                                                                    }
                                                                }
                                                                var td2SpanContent = Regex.Matches(tdContent2, SpanRegex, RegexOptions.IgnoreCase);
                                                                if (td2SpanContent.Count > 0)
                                                                {
                                                                    var regex = new System.Text.RegularExpressions.Regex("<span(.*?)>(.*?)</span>");
                                                                    var sValueMatch = regex.Match(td2SpanContent[0].ToString());
                                                                    if (sValueMatch.Groups.Count > 2)
                                                                    {
                                                                        var sValue1 = sValueMatch.Groups[2].Value;
                                                                        sValue = sValue1.Replace("<b>", "").Replace("</b>", "");
                                                                    }
                                                                }
                                                                if (!string.IsNullOrEmpty(sName) && !string.IsNullOrEmpty(sValue))
                                                                {
                                                                    if (sName.ToLower() == "full name" || sName.ToLower() == "first name" || sName.ToLower() == "name")
                                                                    {
                                                                        var sTname = "";
                                                                        if (sName.ToLower() == "name" && sValue.Contains('' ''))
                                                                        {
                                                                            var Valuespliting = sValue.Split('' '').ToList();
                                                                            if (Valuespliting.Count() >= 3)
                                                                            {
                                                                                sTname = Valuespliting[0].Trim();
                                                                                sFname = Valuespliting[1].Trim();
                                                                                sValue = sFname;
                                                                                sLname = Valuespliting[2].Trim() + (Valuespliting.Count() > 3 ? (" " + Valuespliting[3].Trim()) : "");
                                                                            }
                                                                            else
                                                                            {
                                                                                sFname = Valuespliting[0].Trim();
                                                                                sValue = sFname;
                                                                                sLname = Valuespliting[1].Trim();
                                                                            }
                                                                            if (!string.IsNullOrEmpty(sLname))
                                                                            {
                                                                                var Surnamefieldguid = "307527F8-ABC4-40F2-ADDB-914528253BE8";
                                                                                oRisk.Add(new CNV
                                                                                {
                                                                                    sName = sName,
                                                                                    sValue = sLname,
                                                                                    sType = Surnamefieldguid.ToString()
                                                                                });
                                                                            }
                                                                            if (!string.IsNullOrEmpty(sTname))
                                                                            {
                                                                                var Titlefieldguid = "F1AB572A-DE7D-4817-928A-718C6FED4F56";
                                                                                oRisk.Add(new CNV
                                                                                {
                                                                                    sName = sName,
                                                                                    sValue = sTname,
                                                                                    sType = Titlefieldguid.ToString()
                                                                                });
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            sFname = sValue;
                                                                        }
                                                                    }
                                                                    if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone" || sName.ToLower() == "preferred telephone number")
                                                                    {
                                                                        sMob = sValue;
                                                                    }
                                                                    if (sName.ToLower() == "last name")
                                                                    {
                                                                        sLname = sValue;
                                                                    }
                                                                    if (sName.ToLower() == "postcode")
                                                                    {
                                                                        sPostcode = sValue;
                                                                    }
                                                                    if (sName.ToLower() == "email" || sName.ToLower() == "email address")
                                                                    {
                                                                        sEmail = sValue;
                                                                    }
                                                                    if (sName.ToLower() == "date of birth")
                                                                    {
                                                                        dDob = sValue;
                                                                    }
                                                                    var FKiFieldOriginIDXIGUID = Guid.Empty;
                                                                    var oFieldD = new XIDFieldOrigin();
                                                                    var MappedNV = MappedNVs.Where(m => m.sLabel.ToLower() == sName.ToLower()).FirstOrDefault();
                                                                    if (MappedNV == null)
                                                                    {
                                                                        oCResult.sMessage = "Mapping NV not found for:" + sName;
                                                                        oDefBase.SaveErrortoDB(oCResult);
                                                                        oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                                        if (oFieldD != null && oFieldD.ID > 0)
                                                                        {
                                                                            FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                            XIIBO oMapNV = new XIIBO();
                                                                            oMapNV.BOD = oMapBOD;
                                                                            oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                                            oMapNV.SetAttribute("sName", sName);
                                                                            oMapNV.SetAttribute("sCode", sName);
                                                                            oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                                            oCR = oMapNV.Save(oMapNV);
                                                                            if (oCR.bOK && oCR.oResult != null) { } else { }
                                                                        }
                                                                        else
                                                                        {
                                                                            XIIBO oFO = new XIIBO();
                                                                            oFO.BOD = oFieldDBOD;
                                                                            oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                                                            oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                                                            oFO.SetAttribute("sDisplayName", sName);
                                                                            oCR = oFO.Save(oFO);
                                                                            if (oCR.bOK && oCR.oResult != null)
                                                                            {
                                                                                oFO = (XIIBO)oCR.oResult;
                                                                                oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                                                if (oFieldD != null && oFieldD.ID > 0)
                                                                                {
                                                                                    FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                                }
                                                                                else
                                                                                {
                                                                                    XIIBO oFDI = new XIIBO();
                                                                                    List<CNV> oWhr = new List<CNV>();
                                                                                    oWhr.Add(new CNV
                                                                                    {
                                                                                        sName = "sName",
                                                                                        sValue = sName.Replace(" ", "")
                                                                                    });
                                                                                    oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                                                    var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                                                    Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                                                }
                                                                            }
                                                                            XIIBO oMapNV = new XIIBO();
                                                                            oMapNV.BOD = oMapBOD;
                                                                            oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                                            oMapNV.SetAttribute("sName", sName);
                                                                            oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                                            oCR = oMapNV.Save(oMapNV);
                                                                            if (oCR.bOK && oCR.oResult != null) { } else { }
                                                                        }
                                                                        if (FKiDefaultStepDIDXIGUID != null && FKiDefaultStepDIDXIGUID != Guid.Empty) { }
                                                                        else
                                                                        {
                                                                            XIDQSStep oStepD = new XIDQSStep();
                                                                            oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                                                            oStepD.FKiQSDefintionID = iQSDID;
                                                                            oStepD.sName = "Default Step";
                                                                            oStepD.sDisplayName = "Default Step";
                                                                            oStepD.iDisplayAs = 20;
                                                                            oStepD.StatusTypeID = 10;
                                                                            oStepD.bIsMerge = true;
                                                                            oStepD.sIsHidden = "off";
                                                                            oStepD.iOrder = 100;
                                                                            oStepD.bIsSaveNext = true;
                                                                            oStepD.bIsBack = true;
                                                                            oStepD.bIsCopy = true;
                                                                            oStepD.bIsHistory = true;
                                                                            oStepD.bIsReload = true;
                                                                            oStepD.bIsAutoSaving = true;
                                                                            oStepD.iStage = 200;
                                                                            oCR = oConfig.Save_QuestionSetStep(oStepD);
                                                                            oCache.Clear_DefInCache("IO_Definition_questionset_" + QSDXIGUID.ToString());
                                                                            if (oCR.bOK && oCR.oResult != null)
                                                                            {
                                                                                var oQSDStepBOI = (XIIBO)oCR.oResult;
                                                                                var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                                                                Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                                                                if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                                                                {
                                                                                    var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                                                                    int.TryParse(QSStepDID, out iQSStepDID);
                                                                                    FKiDefaultStepDID = iQSStepDID;
                                                                                }
                                                                                FKiDefaultStepDIDXIGUID = QSDStepXIGUID;
                                                                                XIDQSSection oSecD = new XIDQSSection();
                                                                                oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                                                                oSecD.FKiStepDefinitionID = iQSStepDID;
                                                                                oSecD.iDisplayAs = 30;
                                                                                oSecD.sIsHidden = "off";
                                                                                oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                                                                if (oCR.bOK && oCR.oResult != null)
                                                                                {
                                                                                    var oQSDSecBOI = (XIIBO)oCR.oResult;
                                                                                    var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                                                                    Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                                                                    if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                                                                    {
                                                                                        var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                                                        int.TryParse(QSSecDID, out iQSSecDID);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                                                        oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                                                        oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                                                        oFieldDef.FKiXIStepDefinitionIDXIGUID = FKiDefaultStepDIDXIGUID;
                                                                        oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                                                        oFieldDef.FKiXIStepDefinitionID = FKiDefaultStepDID;
                                                                        oFieldDef.FKiStepSectionID = iQSSecDID;
                                                                        oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                        {
                                                                            oTraceInfo.Add(new CNV
                                                                            {
                                                                                sValue = "Field definition saved for Field: " + sName
                                                                            });
                                                                        }
                                                                        else
                                                                        {
                                                                            oTraceInfo.Add(new CNV
                                                                            {
                                                                                sValue = "Field definition not saved for Field: " + sName
                                                                            });
                                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                        }
                                                                        if (sName.Contains("ARE YOU A FULL-TIME TRADER?"))
                                                                        {
                                                                            FKiFieldOriginIDXIGUID = new Guid("D6CBC718-2358-4EED-83B5-FE99A8EA8E29");
                                                                            if (sValue.Contains("Yes") || sValue.Contains("full-time"))
                                                                            {
                                                                                sValue = "Full-Time";
                                                                            }
                                                                            else
                                                                            {
                                                                                sValue = "Part-Time";
                                                                            }
                                                                        }
                                                                        oRisk.Add(new CNV
                                                                        {
                                                                            sName = sName,
                                                                            sValue = sValue,
                                                                            sType = FKiFieldOriginIDXIGUID.ToString()
                                                                        });
                                                                    }
                                                                    else
                                                                    {
                                                                        FKiFieldOriginIDXIGUID = new Guid(MappedNV.sValue);
                                                                    }
                                                                    if (sName.Contains("ARE YOU A FULL-TIME TRADER?"))
                                                                    {
                                                                        FKiFieldOriginIDXIGUID = new Guid("D6CBC718-2358-4EED-83B5-FE99A8EA8E29");
                                                                        if (sValue.Contains("Yes") || sValue.Contains("full-time"))
                                                                        {
                                                                            sValue = "Full-Time";
                                                                        }
                                                                        else
                                                                        {
                                                                            sValue = "Part-Time";
                                                                        }
                                                                    }
                                                                    oRisk.Add(new CNV
                                                                    {
                                                                        sName = sName,
                                                                        sValue = sValue,
                                                                        sType = FKiFieldOriginIDXIGUID.ToString()
                                                                    });
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (tdContent1.Contains("td width=\"300\""))
                                                            {
                                                                var td1 = tdContent1.Replace("<td width=\"300\">", "").Replace("</td>", "");
                                                                sName = td1;
                                                                if (tdContent2.Contains("td width=\"300\""))
                                                                {
                                                                    var td2 = tdContent2.Replace("<td width=\"300\">", "").Replace("</td>", "");
                                                                    sValue = td2;
                                                                    if (!string.IsNullOrEmpty(sName) && !string.IsNullOrEmpty(sValue))
                                                                    {
                                                                        if (sName.ToLower() == "full name" || sName.ToLower() == "first name" || sName.ToLower() == "name")
                                                                        {
                                                                            var sTname = "";
                                                                            if (sName.ToLower() == "name" && sValue.Contains('' ''))
                                                                            {
                                                                                var Valuespliting = sValue.Split('' '').ToList();
                                                                                if (Valuespliting.Count() >= 3)
                                                                                {
                                                                                    sTname = Valuespliting[0].Trim();
                                                                                    sFname = Valuespliting[1].Trim();
                                                                                    sValue = sFname;
                                                                                    sLname = Valuespliting[2].Trim() + (Valuespliting.Count() > 3 ? (" " + Valuespliting[3].Trim()) : "");
                                                                                }
                                                                                else
                                                                                {
                                                                                    sFname = Valuespliting[0].Trim();
                                                                                    sValue = sFname;
                                                                                    sLname = Valuespliting[1].Trim();
                                                                                }
                                                                                if (!string.IsNullOrEmpty(sLname))
                                                                                {
                                                                                    var Surnamefieldguid = "307527F8-ABC4-40F2-ADDB-914528253BE8";
                                                                                    oRisk.Add(new CNV
                                                                                    {
                                                                                        sName = sName,
                                                                                        sValue = sLname,
                                                                                        sType = Surnamefieldguid.ToString()
                                                                                    });
                                                                                }
                                                                                if (!string.IsNullOrEmpty(sTname))
                                                                                {
                                                                                    var Titlefieldguid = "F1AB572A-DE7D-4817-928A-718C6FED4F56";
                                                                                    oRisk.Add(new CNV
                                                                                    {
                                                                                        sName = sName,
                                                                                        sValue = sTname,
                                                                                        sType = Titlefieldguid.ToString()
                                                                                    });
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                sFname = sValue;
                                                                            }
                                                                        }
                                                                        if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone" || sName.ToLower() == "preferred telephone number")
                                                                        {
                                                                            sMob = sValue;
                                                                        }
                                                                        if (sName.ToLower() == "last name")
                                                                        {
                                                                            sLname = sValue;
                                                                        }
                                                                        if (sName.ToLower() == "postcode")
                                                                        {
                                                                            sPostcode = sValue;
                                                                        }
                                                                        if (sName.ToLower() == "email" || sName.ToLower() == "email address")
                                                                        {
                                                                            sEmail = sValue;
                                                                        }
                                                                        if (sName.ToLower() == "date of birth")
                                                                        {
                                                                            dDob = sValue;
                                                                        }
                                                                        var FKiFieldOriginIDXIGUID = Guid.Empty;
                                                                        var oFieldD = new XIDFieldOrigin();
                                                                        var MappedNV = MappedNVs.Where(m => m.sLabel.ToLower() == sName.ToLower()).FirstOrDefault();
                                                                        if (MappedNV == null)
                                                                        {
                                                                            oCResult.sMessage = "Mapping NV not found for:" + sName;
                                                                            oDefBase.SaveErrortoDB(oCResult);
                                                                            oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                                            if (oFieldD != null && oFieldD.ID > 0)
                                                                            {
                                                                                FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                                XIIBO oMapNV = new XIIBO();
                                                                                oMapNV.BOD = oMapBOD;
                                                                                oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                                                oMapNV.SetAttribute("sName", sName);
                                                                                oMapNV.SetAttribute("sCode", sName);
                                                                                oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                                                oCR = oMapNV.Save(oMapNV);
                                                                                if (oCR.bOK && oCR.oResult != null) { } else { }
                                                                            }
                                                                            else
                                                                            {
                                                                                XIIBO oFO = new XIIBO();
                                                                                oFO.BOD = oFieldDBOD;
                                                                                oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                                                                oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                                                                oFO.SetAttribute("sDisplayName", sName);
                                                                                oCR = oFO.Save(oFO);
                                                                                if (oCR.bOK && oCR.oResult != null)
                                                                                {
                                                                                    oFO = (XIIBO)oCR.oResult;
                                                                                    oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                                                    if (oFieldD != null && oFieldD.ID > 0)
                                                                                    {
                                                                                        FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        XIIBO oFDI = new XIIBO();
                                                                                        List<CNV> oWhr = new List<CNV>();
                                                                                        oWhr.Add(new CNV
                                                                                        {
                                                                                            sName = "sName",
                                                                                            sValue = sName.Replace(" ", "")
                                                                                        });
                                                                                        oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                                                        var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                                                        Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                                                    }
                                                                                }
                                                                                XIIBO oMapNV = new XIIBO();
                                                                                oMapNV.BOD = oMapBOD;
                                                                                oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                                                oMapNV.SetAttribute("sName", sName);
                                                                                oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                                                oCR = oMapNV.Save(oMapNV);
                                                                                if (oCR.bOK && oCR.oResult != null) { } else { }
                                                                            }
                                                                            if (FKiDefaultStepDIDXIGUID != null && FKiDefaultStepDIDXIGUID != Guid.Empty) { }
                                                                            else
                                                                            {
                                                                                XIDQSStep oStepD = new XIDQSStep();
                                                                                oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                                                                oStepD.FKiQSDefintionID = iQSDID;
                                                                                oStepD.sName = "Default Step";
                                                                                oStepD.sDisplayName = "Default Step";
                                                                                oStepD.iDisplayAs = 20;
                                                                                oStepD.StatusTypeID = 10;
                                                                                oStepD.bIsMerge = true;
                                                                                oStepD.sIsHidden = "off";
                                                                                oStepD.iOrder = 100;
                                                                                oStepD.bIsSaveNext = true;
                                                                                oStepD.bIsBack = true;
                                                                                oStepD.bIsCopy = true;
                                                                                oStepD.bIsHistory = true;
                                                                                oStepD.bIsReload = true;
                                                                                oStepD.bIsAutoSaving = true;
                                                                                oStepD.iStage = 200;
                                                                                oCR = oConfig.Save_QuestionSetStep(oStepD);
                                                                                oCache.Clear_DefInCache("IO_Definition_questionset_" + QSDXIGUID.ToString());
                                                                                if (oCR.bOK && oCR.oResult != null)
                                                                                {
                                                                                    var oQSDStepBOI = (XIIBO)oCR.oResult;
                                                                                    var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                                                                    Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                                                                    if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                                                                    {
                                                                                        var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                                                                        int.TryParse(QSStepDID, out iQSStepDID);
                                                                                        FKiDefaultStepDID = iQSStepDID;
                                                                                    }
                                                                                    FKiDefaultStepDIDXIGUID = QSDStepXIGUID;
                                                                                    XIDQSSection oSecD = new XIDQSSection();
                                                                                    oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                                                                    oSecD.FKiStepDefinitionID = iQSStepDID;
                                                                                    oSecD.iDisplayAs = 30;
                                                                                    oSecD.sIsHidden = "off";
                                                                                    oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                                                                    if (oCR.bOK && oCR.oResult != null)
                                                                                    {
                                                                                        var oQSDSecBOI = (XIIBO)oCR.oResult;
                                                                                        var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                                                                        Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                                                                        if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                                                                        {
                                                                                            var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                                                            int.TryParse(QSSecDID, out iQSSecDID);
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                                                            oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                                                            oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                                                            oFieldDef.FKiXIStepDefinitionIDXIGUID = FKiDefaultStepDIDXIGUID;
                                                                            oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                                                            oFieldDef.FKiXIStepDefinitionID = FKiDefaultStepDID;
                                                                            oFieldDef.FKiStepSectionID = iQSSecDID;
                                                                            oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                                                            if (oCR.bOK && oCR.oResult != null)
                                                                            {
                                                                                oTraceInfo.Add(new CNV
                                                                                {
                                                                                    sValue = "Field definition saved for Field: " + sName
                                                                                });
                                                                            }
                                                                            else
                                                                            {
                                                                                oTraceInfo.Add(new CNV
                                                                                {
                                                                                    sValue = "Field definition not saved for Field: " + sName
                                                                                });
                                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                            }
                                                                            if (sName.Contains("ARE YOU A FULL-TIME TRADER?"))
                                                                            {
                                                                                FKiFieldOriginIDXIGUID = new Guid("D6CBC718-2358-4EED-83B5-FE99A8EA8E29");
                                                                                if (sValue.Contains("Yes") || sValue.Contains("full-time"))
                                                                                {
                                                                                    sValue = "Full-Time";
                                                                                }
                                                                                else
                                                                                {
                                                                                    sValue = "Part-Time";
                                                                                }
                                                                            }
                                                                            oRisk.Add(new CNV
                                                                            {
                                                                                sName = sName,
                                                                                sValue = sValue,
                                                                                sType = FKiFieldOriginIDXIGUID.ToString()
                                                                            });
                                                                        }
                                                                        else
                                                                        {
                                                                            FKiFieldOriginIDXIGUID = new Guid(MappedNV.sValue);
                                                                        }
                                                                        if (sName.Contains("ARE YOU A FULL-TIME TRADER?"))
                                                                        {
                                                                            FKiFieldOriginIDXIGUID = new Guid("D6CBC718-2358-4EED-83B5-FE99A8EA8E29");
                                                                            if (sValue.Contains("Yes") || sValue.Contains("full-time"))
                                                                            {
                                                                                sValue = "Full-Time";
                                                                            }
                                                                            else
                                                                            {
                                                                                sValue = "Part-Time";
                                                                            }
                                                                        }
                                                                        oRisk.Add(new CNV
                                                                        {
                                                                            sName = sName,
                                                                            sValue = sValue,
                                                                            sType = FKiFieldOriginIDXIGUID.ToString()
                                                                        });
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (sName == "MOTOR TRADE NO CLAIMS BONUS" || sName == "PRIVATE CAR NO CLAIMS BONUS")
                                        {
                                            var value = "";
                                            List<CNV> oWhereParams = new List<CNV>();
                                            oWhereParams.Add(new CNV { sName = sName, sValue = sValue });
                                            foreach (var param in oWhereParams) { value = param.sValue; }
                                            if (value != "No NCB")
                                            {
                                                oRisk.Add(new CNV { sName = sName, sValue = sName, sType = "112526d2-1a79-480b-996c-f6a6f5ec783b" });
                                                oRisk.Add(new CNV { sName = sName, sValue = sValue, sType = "11d07762-e624-4216-b58a-c0509fc30958" });
                                            }
                                        }
                                    }
                                }
                             else
                             {
                                oTraceInfo.Add(new CNV { sValue = "Body is null for instance: " + iBOIID });
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                             }
                            }
                        else
                        {
                            sBody = oBOI.AttributeI("sContent").sValue;
                            if (!string.IsNullOrEmpty(sBody))
                            {
                                if (sBody.Contains("<html>"))
                                {
                                    XIDValue oVal = new XIDValue();
                                    sBody = oVal.getHtmlBody(sBody);
                                    sBody = Regex.Replace(sBody, @"\r\n", "");
                                    NVs = Regex.Split(sBody.Trim(), "<br>").ToList();
                                }
                                else
                                {
                                    sBody = Regex.Replace(sBody, @"\n", "");
                                    NVs = Regex.Split(sBody.Trim(), "<br/>").ToList();
                                }
                                if (NVs != null && NVs.Count() > 0)
                                {
                                    foreach (var NV in NVs)
                                    {
                                        if (NV.Contains('':''))
                                        {
                                            var Splits = NV.Split('':'').ToList();
                                            var sName = Splits[0].Trim();
                                            var sValue = Splits[1].Trim();
                                            if (sName.ToLower() == "full name" || sName.ToLower() == "first name" || sName.ToLower() == "name")
                                            {
                                                if (sName.ToLower() == "full name" && sValue.Contains('' ''))
                                                {
                                                    var Valuespliting = sValue.Split('' '').ToList();
                                                    if (Valuespliting.Count() == 3)
                                                    {
                                                        sFname = Valuespliting[0].Trim();
                                                        sLname = Valuespliting[1].Trim();
                                                        sLname = Valuespliting[2].Trim();
                                                        sValue = sFname;
                                                    }
                                                    else
                                                    {
                                                        sFname = Valuespliting[0].Trim();
                                                        sLname = Valuespliting[1].Trim();
                                                        sValue = sFname;
                                                    }
                                                    if (!string.IsNullOrEmpty(sLname))
                                                    {
                                                        var Surnamefieldguid = "307527F8-ABC4-40F2-ADDB-914528253BE8";
                                                        oRisk.Add(new CNV
                                                        {
                                                            sName = sName,
                                                            sValue = sLname,
                                                            sType = Surnamefieldguid.ToString()
                                                        });
                                                    }
                                                }
                                                else
                                                {
                                                    sFname = sValue;
                                                }
                                            }
                                            if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone" || sName.ToLower() == "telephone number" || sName.ToLower() == "main telephone number")
                                            {
                                                if (sValue.Contains("</span"))
                                                {
                                                    var value = sValue.Replace("<p style=\"text-align", "").Replace("</span></p>", "");
                                                    sMob = value;
                                                }
                                                else
                                                {
                                                    sMob = sValue;
                                                }
                                            }
                                            if (sName.ToLower() == "last name" || sName.ToLower() == "sur name" || sName.ToLower() == "surname")
                                            {
                                                sLname = sValue;
                                            }
                                            if (sName.ToLower() == "postcode")
                                            {
                                                sPostcode = sValue;
                                            }
                                            if (sName.ToLower() == "email" || sName.ToLower() == "email address")
                                            {
                                                if (sValue.Contains("@"))
                                                {
                                                    sEmail = sValue;
                                                }
                                            }
                                            if (sName.ToLower() == "date of birth" || sName.ToLower() == "dob")
                                            {
                                                dDob = sValue;
                                            }
                                            var FKiFieldOriginIDXIGUID = Guid.Empty;
                                            var oFieldD = new XIDFieldOrigin();
                                            var MappedNV = MappedNVs.Where(m => m.sLabel.ToLower() == sName.ToLower()).FirstOrDefault();
                                            if (MappedNV == null)
                                            {
                                                oCResult.sMessage = "Mapping NV not found for:" + sName;
                                                oDefBase.SaveErrortoDB(oCResult);
                                                oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                if (oFieldD != null && oFieldD.ID > 0)
                                                {
                                                    FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                    XIIBO oMapNV = new XIIBO();
                                                    oMapNV.BOD = oMapBOD;
                                                    oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                    oMapNV.SetAttribute("sName", sName);
                                                    oMapNV.SetAttribute("sCode", sName);
                                                    oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                    oCR = oMapNV.Save(oMapNV);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {

                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                                    else
                                                    {
                                                        XIIBO oFO = new XIIBO();
                                                        oFO.BOD = oFieldDBOD;
                                                        oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                                        oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                                        oFO.SetAttribute("sDisplayName", sName);
                                                        oCR = oFO.Save(oFO);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            oFO = (XIIBO)oCR.oResult;
                                                            oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                            if (oFieldD != null && oFieldD.ID > 0)
                                                            {
                                                                FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                            }
                                                            else
                                                            {
                                                                XIIBO oFDI = new XIIBO();
                                                                List<CNV> oWhr = new List<CNV>();
                                                                oWhr.Add(new CNV
                                                                {
                                                                    sName = "sName",
                                                                    sValue = sName.Replace(" ", "")
                                                                });
                                                                oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                                var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                                Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                            }
                                                        }
                                                        XIIBO oMapNV = new XIIBO();
                                                        oMapNV.BOD = oMapBOD;
                                                        oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                        oMapNV.SetAttribute("sName", sName);
                                                        oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                        oCR = oMapNV.Save(oMapNV);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {

                                                        }
                                                        else
                                                        {

                                                        }
                                                    }
                                                    if (FKiDefaultStepDIDXIGUID != null && FKiDefaultStepDIDXIGUID != Guid.Empty)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        XIDQSStep oStepD = new XIDQSStep();
                                                        oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                                        oStepD.FKiQSDefintionID = iQSDID;
                                                        oStepD.sName = "Default Step";
                                                        oStepD.sDisplayName = "Default Step";
                                                        oStepD.iDisplayAs = 20;
                                                        oStepD.StatusTypeID = 10;
                                                        oStepD.bIsMerge = true;
                                                        oStepD.sIsHidden = "off";
                                                        oStepD.iOrder = 100;
                                                        oStepD.bIsSaveNext = true;
                                                        oStepD.bIsBack = true;
                                                        oStepD.bIsHistory = true;
                                                        oStepD.bIsCopy = true;
                                                        oStepD.bIsReload = true;
                                                        oStepD.bIsAutoSaving = true;
                                                        oStepD.iStage = 200;
                                                        oCR = oConfig.Save_QuestionSetStep(oStepD);
                                                        oCache.Clear_DefInCache("IO_Definition_questionset_" + QSDXIGUID.ToString());
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            var oQSDStepBOI = (XIIBO)oCR.oResult;
                                                            var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                                            Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                                            if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                                            {
                                                                var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                                                int.TryParse(QSStepDID, out iQSStepDID);
                                                                FKiDefaultStepDID = iQSStepDID;
                                                            }
                                                            FKiDefaultStepDIDXIGUID = QSDStepXIGUID;
                                                            XIDQSSection oSecD = new XIDQSSection();
                                                            oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                                            oSecD.FKiStepDefinitionID = iQSStepDID;
                                                            oSecD.iDisplayAs = 30;
                                                            oSecD.sIsHidden = "off";
                                                            oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                                            if (oCR.bOK && oCR.oResult != null)
                                                            {
                                                                var oQSDSecBOI = (XIIBO)oCR.oResult;
                                                                var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                                                Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                                                if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                                                {
                                                                    var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                                    int.TryParse(QSSecDID, out iQSSecDID);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                                    oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                                    oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                                    oFieldDef.FKiXIStepDefinitionIDXIGUID = FKiDefaultStepDIDXIGUID;
                                                    oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                                    oFieldDef.FKiXIStepDefinitionID = FKiDefaultStepDID;
                                                    oFieldDef.FKiStepSectionID = iQSSecDID;
                                                    oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        oTraceInfo.Add(new CNV
                                                        {
                                                            sValue = "Field definition saved for Field: " + sName
                                                        });
                                                    }
                                                    else
                                                    {
                                                        oTraceInfo.Add(new CNV
                                                        {
                                                            sValue = "Field definition not saved for Field: " + sName
                                                        });
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                    }
                                                }
                                            else if (sName == "Phone" && sValue == "Yes")
                                            {
                                                oRisk.Add(new CNV { sName = sName, sValue = sValue, sType = "30BDCADF-0126-48DB-BBF9-6EB6F3167392" });
                                            }
                                            else
                                            {
                                                FKiFieldOriginIDXIGUID = new Guid(MappedNV.sValue);
                                            }
                                            if (sName == "Full Time Trader")
                                            {
                                                FKiFieldOriginIDXIGUID = new Guid("D6CBC718-2358-4EED-83B5-FE99A8EA8E29");
                                                if (sValue == "Yes")
                                                {
                                                    sValue = "Full-Time";
                                                }
                                                else
                                                {
                                                    sValue = "Part-Time";
                                                }
                                            }
                                            oTraceInfo.Add(new CNV { sValue = "NV: " + NV + " and Risk value for field " + sName + ": is " + sValue });
                                            oRisk.Add(new CNV { sName = sName, sValue = sValue, sType = FKiFieldOriginIDXIGUID.ToString() });
                                            if (sName == "TradeNCB" || sName == "CarNCB" || sName == "Motor Trade No Claims Bonus" || sName == "Private Car No Claims Bonus")
                                            {
                                                var value = "";
                                                List<CNV> oWhereParams = new List<CNV>();
                                                oWhereParams.Add(new CNV { sName = sName, sValue = sValue });
                                                foreach (var param in oWhereParams) { value = param.sValue; }
                                                if (value != "None")
                                                {
                                                    oRisk.Add(new CNV { sName = sName, sValue = sName, sType = "112526d2-1a79-480b-996c-f6a6f5ec783b" });
                                                    oRisk.Add(new CNV { sName = sName, sValue = sValue, sType = "11d07762-e624-4216-b58a-c0509fc30958" });
                                                }
                                            }
                                            if (sName == "Any Drivers With Claims And Convictions")
                                            {
                                                if (sValue == "No")
                                                {
                                                    sValue = "No";
                                                }
                                                else
                                                {
                                                    sValue = "Yes";
                                                }
                                                oRisk.Add(new CNV { sName = sName, sValue = sValue, sType = "aec00dd3-4ab6-470b-a2be-1756592bb00d" });
                                                oRisk.Add(new CNV { sName = sName, sValue = sValue, sType = "053c5ea7-d625-4179-8a40-846e241c56e2" });
                                            }
                                            if (sName == "Full UK licence held for")
                                            {
                                                if (sName == "Full UK licence held for")
                                                {
                                                    sValue = "Full UK";
                                                }
                                                else
                                                {
                                                    sValue = "Full EU";
                                                }
                                                oRisk.Add(new CNV { sName = sName, sValue = sValue, sType = "c775f75f-df1f-4bb2-9c38-1e7d55819ff2" });
                                            }
                                        }
                                    }
                                }
                            }
                                else
                                {
                                    oTraceInfo.Add(new CNV
                                    {
                                        sValue = "Body is null for instance: " + iBOIID
                                    });
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                        }
                        }
                        else if (iType == "20")
                        {
                            sBody = oBOI.AttributeI("sContent").sValue;
                            string sName = string.Empty;
                            string sValue = string.Empty;
                            string sAge = string.Empty;
                            if (!string.IsNullOrEmpty(sBody))
                            {
                                XmlDocument xDoc = new XmlDocument();
                                xDoc.LoadXml(sBody.ToString());
                                XmlNodeList nodes = xDoc.SelectNodes("//lead");
                                bool bIsDob = false;
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
                                    if (sName.ToLower() == "dob")
                                    {
                                        dDob = node.InnerText;
                                        int iYear = Convert.ToDateTime(dDob).Year;
                                        if (iYear > 0)
                                        {
                                            int iAge = DateTime.Now.Year - iYear;
                                            sAge = iAge.ToString();
                                        }
                                        bIsDob = true;
                                    }
                                    if (bIsDob == false)
                                    {
                                        if (sName.ToLower() == "dobday")
                                            dDob = node.InnerText;

                                        if (sName.ToLower() == "dobmonth")
                                            dDob = dDob + "-" + node.InnerText;

                                        if (sName.ToLower() == "dobyear")
                                        {
                                            dDob = dDob + "-" + node.InnerText;
                                            sValue = dDob;
                                            int iYear = Convert.ToDateTime(dDob).Year;
                                            if (iYear > 0)
                                            {
                                                int iAge = DateTime.Now.Year - iYear;
                                                sAge = iAge.ToString();
                                            }
                                        }
                                    }
                                    if (sName.ToLower() == "postcode")
                                        sPostcode = node.InnerText;
                                    if (sName.ToLower() == "uklicence")
                                    {
                                        int numericValue;
                                        if (int.TryParse(System.Text.RegularExpressions.Regex.Match(sValue, @"\d+").Value, out numericValue))
                                        {
                                            if (numericValue >= 10)
                                            {
                                                sValue = "10 Years+";
                                            }
                                        }
                                    }
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
                                            if (sName.ToLower() == "dobyear" || sName.ToLower() == "dob")
                                            {
                                                oRisk.Add(new CNV
                                                {
                                                    sName = "age",
                                                    sValue = sAge,
                                                    sType = FKiFieldOriginIDXIGUID.ToString()
                                                });
                                            }
                                            else
                                            {
                                                oRisk.Add(new CNV
                                                {
                                                    sName = sName,
                                                    sValue = sValue,
                                                    sType = FKiFieldOriginIDXIGUID.ToString()
                                                });
                                            }
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
                                    sValue = "Body is null for instance: " + iBOIID
                                });
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        var Fieldidguid = "860702CD-1D09-4E09-946D-714F595D0931";
                        if (!string.IsNullOrEmpty(iSource))
                        {
                            oRisk.Add(new CNV
                            {
                                sName = "iMTSource",
                                sValue = iSource,
                                sType = Fieldidguid.ToString()
                            });
                        }
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
                        XIDQS oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, QSDXIGUID.ToString(), "", sGUID, 0, 0);
                        oTraceInfo.Add(new CNV
                        {
                            sValue = "QS Definition loaded from Cache"
                        });
                        XIDQS oQSDC = (XIDQS)oQSD.Clone(oQSD);
                        int iOrgID = 0;
                        int.TryParse(FKiOrgID, out iOrgID);
                        oCR = oXI.CreateQSI(null, QSDXIGUID.ToString(), null, null, 0, 0, null, 0, null, 0, sGUID, null, iOrgID);
                        oTraceInfo.Add(new CNV { sValue = "QS Instance is created" });
                        var oQSInstance = (XIIQS)oCR.oResult;
                        oQSInstance.QSDefinition = oQSDC;
                        if (oQSInstance.iCurrentStepIDXIGUID == Guid.Empty || oQSInstance.iCurrentStepIDXIGUID == null)
                        {
                            var iActiveStepID = oQSD.Steps.Values.ToList().OrderBy(m => m.iOrder).FirstOrDefault().XIGUID;
                            oQSInstance.iCurrentStepIDXIGUID = iActiveStepID;
                            oQSInstance = oQSInstance.Save(oQSInstance, "");
                        }
                        oTraceInfo.Add(new CNV
                        {
                            sValue = "Step instance loaded"
                        });
                        var RiskBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T");
                        if (oRisk != null && oRisk.Count() > 0)
                        {
                            XIDValue oVal = new XIDValue();
                            oVal.MTLeadGenMapping(oRisk);
                            foreach (var risk in oRisk)
                            {
                                if (risk.sName.ToLower() == "vehiclevalue")
                                {
                                    XIDValue oVall = new XIDValue();
                                    oVall.Vechicalculation(oRisk);
                                }
								if (risk.sName.ToLower() == "source".ToLower())
                                {
                                    XIDValue oVals = new XIDValue();
                                    oVals.APISourceMapping(oRisk, FKiOrgID);
                                }
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
								if (risk.sName.ToLower() == "source".ToLower()) 
                                 {
                                     iSource = risk.sValue;
                                 }
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
						if (QSDXIGUID == new Guid("4DD02A52-CA10-40D4-B8D5-0AF5847D0E1F")) 
						{
                          oLeadI.SetAttribute("iStatus", "331");
                        } 
		               else 
					   {
                         oLeadI.SetAttribute("iStatus", "0");
                      }
                        oLeadI.SetAttribute("sName", sFname);
                        oLeadI.SetAttribute("dDOB", dDob);
                        oLeadI.SetAttribute("FKiOrgID", FKiOrgID.ToString());
                        if (!string.IsNullOrEmpty(iSource))
                        {
                            oLeadI.SetAttribute("FKiSourceID", iSource);
                        }
                        oLeadI.oSignalR = oSignalR;
                        oCR = oLeadI.Save(oLeadI, false);
						XID1Click PV1Click = new XID1Click();
                        var Query = "select top (1)  (ID)  from Lead_T where IsBlocked = " + 1 + " and " + "sPostCode=''" + sPostcode + "''" +" and " + "sLastName=''" + sLname + "''" + " and " + "sMob=''" + sMob + "''" + " and " + "sFirstName=''" + sFname + "''" + " and " + "FKiOrgID=''" + FKiOrgID + "''";
                        PV1Click.Query = Query;
                        PV1Click.Name = "Lead_T";
                        var Result = PV1Click.OneClick_Execute();
                        if (Result.Count() != 0)
                        {
                            XIIXI oIXI = new XIIXI();
                            var LeadID = oLeadI.AttributeI("id").sValue;
                            var oBOII = oIXI.BOI("Lead", LeadID);
                            oBOII.SetAttribute("IsBlocked", "1");
                            oBOII.SetAttribute("BlockedReason", "20");
                            oCResult = oBOII.Save(oBOII);
                            XIIBO BlockedBOI = new XIIBO();
                            XIDBO ObjectBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIBlockedLeads_T", null);
                            BlockedBOI.BOD = ObjectBOD;
                            BlockedBOI.SetAttribute("FKiLeadID", LeadID);
                            BlockedBOI.SetAttribute("FKiBlockedReason", "20");
                            BlockedBOI.Save(BlockedBOI);
                        }
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
                                oCommMatch.SetAttribute("FKiCommunicationID", iBOIID);
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
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTraceInfo.Add(new CNV
                    {
                        sValue = "Mandatory Params: iBODID " + iBODID + " or iBOIID:" + iBOIID + " are missing"
                    });
                }
                oCResult.oTraceStack = oTraceInfo;
                oDefBase.SaveErrortoDB(oCResult);
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = QSDXIGUID;
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
            return oCResult;
        }',
 N'Prepersist',
 N'C# Code',
 N'0',
 N'2019-07-09 15:12:01.000',
 N'173',
 N'192.168.3.51',
 N'180',
 N'169.254.204.77',
 N'2019-07-09 15:12:01.000',
 N'15',
 N'DynamicDefinition',
 N'0',
 NULL,
 N'5',
 N'0',
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 N'f8b8c9b7-32f0-46fa-8b79-6acd19a413e2',
 N'0',
 N'0',
 N'0',
 N'2021-08-18 14:03:31.000',
 N'2021-08-18 14:03:31.000',
 NULL,
 NULL,
 NULL,
 N'd9ae2558-9acc-44f9-918d-aea8ad0ff962',
 N'789b6fd1-e5d8-4be9-8d9a-d76eae8589bc',
 NULL,
 NULL,
 NULL)
