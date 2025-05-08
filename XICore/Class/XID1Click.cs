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
using XIDatabase;
using System.Text.RegularExpressions;
using System.Data;

using System.ComponentModel.DataAnnotations.Schema;
using XISystem;
using System.Configuration;
using xiEnumSystem;
using System.Data.SqlClient;
using Dapper;
using System.Net;
using System.Web;
using XIDataBase;
using MongoDB.Bson;

namespace XICore
{
    public class XID1Click : XIDefinitionBase
    {
        //public Reports()
        //{
        //    this.Sub1Clicks = new HashSet<Reports>();
        //}.      
        public int ID { get; set; }
        public int LeadCount { get; set; }
        public bool bFlag { get; set; }
        public int OrganizationID { get; set; }
        public int? ParentID { get; set; }
        public bool IsParent { get; set; }
        public int CategoryID { get; set; }
        public int BOID { get; set; }
        public Guid BOIDXIGUID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int TypeID { get; set; }
        public string Query { get; set; }
        public string VisibleQuery { get; set; }
        public int DisplayAs { get; set; }
        public int ResultListDisplayType { get; set; }
        public string ResultIn { get; set; }
        public string PopupType { get; set; }
        public int? PopupLeft { get; set; }
        public int? PopupTop { get; set; }
        public int? PopupWidth { get; set; }
        public int? PopupHeight { get; set; }

        public string DialogMy1 { get; set; }
        public string DialogMy2 { get; set; }
        public string DialogAt1 { get; set; }
        public string DialogAt2 { get; set; }

        public int RowXiLinkID { get; set; }
        public Guid RowXiLinkIDXIGUID { get; set; }
        public string sRowXiLinkType { get; set; }
        public int ColumnXiLinkID { get; set; }
        public int CellXiLinkID { get; set; }
        public int Class { get; set; }
        public bool IsFilterSearch { get; set; }
        public bool IsNaturalSearch { get; set; }
        public bool IsExport { get; set; }
        public bool IsDynamic { get; set; }
        public bool IsStoredProcedure { get; set; }
        public bool bIsEqualOperator { get; set; }
        public string SelectFields { get; set; }
        public string FromBos { get; set; }
        public string WhereFields { get; set; }
        public string FKiWhereValue { get; set; }
        public string FKiOrderValue { get; set; }
        public string GroupFields { get; set; }
        public string OrderFields { get; set; }
        public string ActionFields { get; set; }
        public string ActionFieldValue { get; set; }
        public string ViewFields { get; set; }
        public string EditableFields { get; set; }
        public string NonEditableFields { get; set; }
        public string Description { get; set; }
        //public byte StatusTypeID { get; set; }
        public string SearchFields { get; set; }
        public int InnerReportID { get; set; }
        public bool IsRowClick { get; set; }
        public string OnRowClickType { get; set; }
        public int OnRowClickValue { get; set; }
        public bool IsColumnClick { get; set; }
        public string OnClickColumn { get; set; }
        public string OnClickParameter { get; set; }
        public string OnColumnClickType { get; set; }
        public int OnColumnClickValue { get; set; }
        public bool IsCellClick { get; set; }
        public string OnClickCell { get; set; }
        public string OnCellClickType { get; set; }
        public int OnCellClickValue { get; set; }
        public bool IsRowTotal { get; set; }
        public bool IsColumnTotal { get; set; }
        public int iPaginationCount { get; set; }
        public bool bIsExport { get; set; }
        public string sFileExtension { get; set; }
        public string sTotalColumns { get; set; }
        public string FKi1ClickScriptID { get; set; }
        public bool bIsRecordLock { get; set; }
        public string FKiListGroup { get; set; }
        public int FKiSearchGroup { get; set; }
        public Guid FKiSearchGroupXIGUID { get; set; }
        public string FKiAttributeList { get; set; }//Attribute DropDown Where Condition in Simple1Click
        public string FKiOrderList { get; set; }//For  Order List DropDown in Simple1Click
        public string FKiOperator { get; set; }//For Where Condition in Simple1Click
        public string OptionorText { get; set; }//For SearchGroup and List Group TextBox in Simple1Click
        public int DailerDrpDown { get; set; }//DailerComponent Task
        public int FKiQSInstanceID { get; set; }//For navigation in simple1click
        public int FKiStructureID { get; set; }//Rowclick for popup in simple1click
        public string FKiFilterSearch { get; set; }
        public int iOrgID { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        public int iUserID { get; set; }
        public Dictionary<string, string> XILinks { get; set; }
        public List<XIDropDown> ddlOneClicks { get; set; }
        public List<string> Parent1Clicks { get; set; }
        public Dictionary<string, string> AllBOss { get; set; }//auto complete fields
        public string parent { get; set; }
        public List<XIDropDown> Classes { get; set; }
        public List<XIDropDown> ReportTypes { get; set; }
        public List<XIDropDown> XIComponentList { get; set; }
        public string DialogType { get; set; }
        public List<XIDropDown> StatusTypes { get; set; }
        public List<XIDropDown> ddlVisualisations { get; set; }
        public bool IsMultipleBo { get; set; }
        public List<CNV> NVs { get; set; }
        public List<CNV> NDVs { get; set; }
        public List<XIDropDown> SMSTemplates { get; set; }
        public List<XID1Click> SubAttributes { get; set; }
        public int EmailTemplateID { get; set; }
        public int SMSTemplateID { get; set; }
        public List<XIDropDown> EmailTemplates { get; set; }
        public string AlertType { get; set; }
        public string Time { get; set; }
        public string Day { get; set; }
        public int Date { get; set; }
        public string TargetPeriod { get; set; }
        public int UserID { get; set; }
        public int SchQueryID { get; set; }
        public int TargetID { get; set; }
        public int TargetUsers { get; set; }
        public int TargetColumns { get; set; }
        public int Targets { get; set; }
        public string Colour { get; set; }
        public bool IsSMS { get; set; }
        public bool IsEmail { get; set; }
        public bool IsNotification { get; set; }
        public XIBOInstance StructureInstanceData { get; set; }
        public int TarQueryID { get; set; }
        public string FKiSelectedValue { get; set; }
        public bool bIsHidden { get; set; }
        public string sLastUpdate { get; set; }
        [ForeignKey("BOID")]
        public virtual ICollection<XIDAttribute> BOFields { get; set; }
        public virtual ICollection<XIDGroup> BOGroups { get; set; }
        [ForeignKey("FKiBOID")]
        public virtual ICollection<XIDScript> BOScripts { get; set; }
        public List<XIDropDown> ddlRoles { get; set; }
        public List<XIDropDown> ddlBOGroups { get; set; }
        public List<XIDropDown> InnerReports { get; set; }
        public List<XIDropDown> TargetUsersList { get; set; }
        public List<XIDropDown> PopupsList { get; set; }
        public List<XIDropDown> XiLinksList { get; set; }
        public List<XIDropDown> ddlLayouts { get; set; }
        public List<XIDropDown> ddlLayoutMappings { get; set; }
        public List<XID1ClickLink> OneClickXILinks { get; set; }
        public string BO { get; set; }
        public string BOName { get; set; }
        public string generalquery { get; set; }
        public string TableName { get; set; }
        public string generalselectedfields { get; set; }
        public string selectedgroupfields { get; set; }
        public List<groups> groupallfields { get; set; }
        public string SelectWithAlias { get; set; }
        public string selectedfields { get; set; }
        public int RepeaterType { get; set; }
        public List<XIDropDown> AllBOs { get; set; }
        public string sRowColour { get; set; }
        public int iBOAttrID { get; set; }
        public string sLabelOverrideGroup { get; set; }
        public string sActionOneClickType { get; set; }
        public bool bIsSelectAll { get; set; }
        public bool bDefaultSelect { get; set; }
        public string sStatusName { get; set; }
        public string sHours { get; set; }
        public int iActionXILinkID { get; set; }
        public Guid XIGUID { get; set; }
        public string sParentBO { get; set; }
        public bool IsFkGUID { get; set; }
        public string LayoutGuid { get; set; }

        public int iAPI { get; set; }
        public int iBroadcastUpdate { get; set; }
        public string sCssClass { get; set; }
        //IDE
        public List<XIVisualisation> oVisualisation { get; set; }
        public int iNoofFields { get; set; }
        public string sRowClickAttr { get; set; }
        public int iTriggerType { get; set; }
        public string sNoResultsMessage { get; set; }
        public bool bClosePopup { get; set; }
        public List<XI1QueryAttributeDetails> OverrideAttributeDetails { get; set; }

        private List<XID1ClickScripts> oMyScripts;
        public List<XID1ClickScripts> oScripts
        {
            get
            {
                return oMyScripts;
            }
            set
            {
                oMyScripts = value;
            }
        }
        private List<XID1Click> oMySub1Clicks;
        public List<XID1Click> Sub1Clicks
        {
            get
            {
                return oMySub1Clicks;
            }
            set
            {
                oMySub1Clicks = value;
            }
        }
        private List<XID1ClickAction> oMyActions;
        public List<XID1ClickAction> Actions
        {
            get
            {
                return oMyActions;
            }
            set
            {
                oMyActions = value;
            }
        }

        private List<XIDFormatMapping> oMyFormats;
        public List<XIDFormatMapping> Formats
        {
            get
            {
                return oMyFormats;
            }
            set
            {
                oMyFormats = value;
            }
        }

        //[ForeignKey("ParentID")]
        //public virtual ICollection<Reports> Sub1Clicks { get; set; }
        //public bool IsLeaf
        //{
        //    get
        //    {
        //        return this.Sub1Clicks.Count == 0;
        //    }
        //}
        public string Type { get; set; }
        public string HTMLCode { get; set; }
        //public string sCreateHTML { get; set; }
        public string InboxCount { get; set; }
        public string Percentage { get; set; }

        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public int CreateRoleID { get; set; }
        public int EditRoleID { get; set; }
        public int DeleteRoleID { get; set; }
        public int CreateGroupID { get; set; }
        public Guid CreateGroupIDXIGUID { get; set; }
        public int EditGroupID { get; set; }
        public int iLayoutID { get; set; }
        public int iCreateXILinkID { get; set; }
        public Guid iCreateXILinkIDXIGUID { get; set; }
        public string sAddLabel { get; set; }
        public string sCreateType { get; set; }
        public string sSystemType { get; set; }
        [NotMapped]
        public string Operator { get; set; }
        [NotMapped]
        public string sReferenceID { get; set; }
        private string sMyDisplayAs;
        public string sDisplayAs
        {
            get
            {
                return sMyDisplayAs;
            }
            set
            {
                sMyDisplayAs = value;
            }
        }

        public string ActionType { get; set; }
        public string sGUID { get; set; }
        public string SearchText { get; set; }
        public string SearchType { get; set; }
        public string sCreateGroup { get; set; }
        public Dictionary<string, string> FilterGroup { get; set; }
        public List<string> Headings { get; set; }
        public List<string> TableColumns { get; set; }
        public List<string> FKColumns { get; set; }
        public List<string> FKAttributes { get; set; }
        public List<string> CssAttributes { get; set; }
        //public List<string> OptionListCols { get; set; }
        public Dictionary<string, XIDBO> OptionListCols { get; set; }
        public List<string> TotalColumns { get; set; }
        public List<string[]> Rows { get; set; }

        public List<XIBODisplay> XIBODisplay { get; set; }
        public int FKiComponentID { get; set; }
        public Guid FKiComponentIDXIGUID { get; set; }
        public int RepeaterComponentID { get; set; }
        public XIDComponent XIComponent { get; set; }
        public string sConnectionString { get; set; }

        public Dictionary<string, XIIBO> oDataSet = new Dictionary<string, XIIBO>();
        public Dictionary<string, List<XIIBO>> oLDataSet = new Dictionary<string, List<XIIBO>>();
        public List<string[]> oDataTableResult { get; set; }
        public List<string> RepeaterResult { get; set; }
        public int iSkip { get; set; }
        public int iTake { get; set; }
        public int iTotaldisplayRecords { get; set; }
        public int iTotalRecords { get; set; }
        public bool bIsResolveFK { get; set; }
        public string NVPairs { get; set; }
        public string Fields { get; set; }
        public string Optrs { get; set; }
        public string Values { get; set; }
        public string sOrginalQuery { get; set; }
        public int iSortCol { get; set; }
        public string sBOName { get; set; }
        public bool IsRefresh { get; set; }
        public bool bIsCheckbox { get; set; }
        public bool bIsCopy { get; set; }
        public bool bIsView { get; set; }
        public bool bIsDelete { get; set; }
        public bool bIsPreview { get; set; }
        public bool bIsMultiBO { get; set; }
        public bool bIsXILoad { get; set; }
        public string sSortDir { get; set; }
        public string sIsFirstTime { get; set; }
        public bool bIsLockToUser { get; set; }
        public List<int> RoleIDs { get; set; }
        public string sJoins { get; set; }
        public int FKiVisualisationID { get; set; }
        public Guid FKiVisualisationIDXIGUID { get; set; }
        public bool IsDisableSave { get; set; }
        public bool bIsMultiSearch { get; set; }
        public bool bIsAddBottom { get; set; }
        public bool bIsAddTop { get; set; }
        public bool bIsOrderIncrement { get; set; }
        public bool bIsOrderDecrement { get; set; }
        public string sCompile { get; set; }
        public string sAssign { get; set; }
        public int iDefaultSortColumn { get; set; }
        public string sDefaultSortDirection { get; set; }
        public int DataSource { get; set; }
        public Guid DataSourceXIGUID { get; set; }
        [NotMapped]
        public List<XIDropDown> CrtdFieldDDL = new List<XIDropDown>();
        [NotMapped]
        public List<XIDropDown> UpdtdFieldDDL = new List<XIDropDown>();
        public bool bIsXICreatedBy { get; set; }
        public bool bIsXIUpdatedBy { get; set; }
        public int FKiCrtd1ClickID { get; set; }
        public int FKiUpdtd1ClickID { get; set; }
        public string sLog { get; set; }
        public bool bMatrix { get; set; }
        public string sCode { get; set; }
        [NotMapped]
        public string sLockGroup { get; set; }
        [NotMapped]
        public string sHiddenGroup { get; set; }
        [NotMapped]
        public string sParentWhere { get; set; }
        public bool bIsApplyActorWhereClause { get; set; }
        public string sSwitchDB { get; set; }
        public string sErrorMessage { get; set; }
        public bool bMergeQuotes { get; set; }
        public bool bIsGrid = false;
        public int iLevel { get; set; }
        private List<XID1ClickLink> oMyLinks = new List<XID1ClickLink>();
        public List<XID1ClickLink> MyLinks
        {
            get
            {
                return oMyLinks;
            }
            set
            {
                oMyLinks = value;
            }
        }
        [NotMapped]
        public List<SqlParameter> ListSqlParams { get; set; }
        [NotMapped]
        public DynamicParameters DapperParams = new DynamicParameters();
        //public List<SqlParameter> SqlParams
        //{
        //    get
        //    {
        //        return _sqlparams;
        //    }
        //    set
        //    {
        //        _sqlparams = value;
        //    }
        //}

        private List<XID1ClickParameter> XI1ClickParameters = new List<XID1ClickParameter>();
        public List<XID1ClickParameter> oOneClickParameters
        {
            get
            {
                return XI1ClickParameters;
            }
            set
            {
                XI1ClickParameters = value;
            }
        }

        private List<XI1ClickSummary> oMyXI1ClickSummary = new List<XI1ClickSummary>();
        public List<XI1ClickSummary> XI1ClickSummary
        {
            get
            {
                return oMyXI1ClickSummary;
            }
            set
            {
                oMyXI1ClickSummary = value;
            }
        }
        public class groups
        {
            public string singlefield { get; set; }
            public string singlefieldtype { get; set; }
            public string singlealiasname { get; set; }
            public Nullable<int> groupid { get; set; }
            public string groupname { get; set; }
            public string bofieldnames { get; set; }
            public string bosqlfieldnames { get; set; }
        }

        Dictionary<string, XIDBO> nXIDBO = new Dictionary<string, XIDBO>();
        XIInfraCache oCache = new XIInfraCache();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        public string GetBOIndentifierFromQuery()
        {
            int iJoinIndex = Query == null ? -1 : Query.IndexOf("inner join", StringComparison.OrdinalIgnoreCase);
            if (iJoinIndex <= 0)
            {
                iJoinIndex = Query == null ? -1 : Query.IndexOf("left join", StringComparison.OrdinalIgnoreCase);
            }
            if (iJoinIndex <= 0)
            {
                iJoinIndex = Query == null ? -1 : Query.IndexOf("right join", StringComparison.OrdinalIgnoreCase);
            }
            if (iJoinIndex <= 0)
            {
                iJoinIndex = Query == null ? -1 : Query.IndexOf("join", StringComparison.OrdinalIgnoreCase);
            }
            if (iJoinIndex > 0)
            {
                string sIdentfier = Query.Substring(0, iJoinIndex).Trim(' ').Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList().LastOrDefault();
                return sIdentfier;
            }
            return "";

        }
        public Dictionary<string, XIIBO> OneClick_Execute(List<CNV> SessionItems = null, XID1Click o1ClickD = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            Dictionary<string, XIIBO> nBOIns = new Dictionary<string, XIIBO>();
            XIInfraUsers oUser = new XIInfraUsers();
            CUserInfo oInfo = new CUserInfo();
            string sBOIndentifier = GetBOIndentifierFromQuery();
            if (SessionItems == null)
            {
                oInfo = oUser.Get_UserInfo();
            }
            else
            {
                oInfo = oInfo.GetUserInfo(SessionItems);
            }
            sOrginalQuery = Query;
            if (o1ClickD != null)
            {
                bIsApplyActorWhereClause = o1ClickD.bIsApplyActorWhereClause;
            }
            else
            {
                bIsApplyActorWhereClause = true;
            }
            XIDBO oBOD = new XIDBO();
            if (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)
            {
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOIDXIGUID.ToString());
            }
            else if (BOID > 0)
            {
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOID.ToString());
            }
            else if (Name != null)
            {
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, Name, null);
            }
            var WhiteListCheck = System.Configuration.ConfigurationManager.AppSettings["WhitelistCheck"];
            if (WhiteListCheck == "yes")
            {
                if (oBOD != null && oBOD.Name.ToLower() != "BO WhiteList".ToLower() && oBOD.BOID != 2459)
                {
                    XIIBO oBOI = new XIIBO();
                    oCR = oBOI.Check_Whitelist(oBOD.BOID, oInfo.iRoleID, oInfo.iOrganizationID, oInfo.iApplicationID, "query", oBOD.iLevel, oInfo.iLevel);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var bUNAuth = (bool)oCR.oResult;
                        if (bUNAuth)
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            return new Dictionary<string, XIIBO>();
                        }
                    }
                }
            }
            oCR = Check_QueryAccess(oInfo.iRoleID, oInfo.iOrganizationID, oInfo.iApplicationID);
            if (oCR.bOK && oCR.oResult != null)
            {
                var bAllowed = false;
                var bUNAuth = (string)oCR.oResult;
                bool.TryParse(bUNAuth, out bAllowed);
                if (!bAllowed)
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    return new Dictionary<string, XIIBO>();
                }
            }
            int iDataSource = oBOD.iDataSource;
            Guid iDataSourceXIGUID = oBOD.iDataSourceXIGUID;
            XIDXI oXID = new XIDXI();
            string sConntection = string.Empty;

            if (iDataSource > 0 || (iDataSourceXIGUID != null && iDataSourceXIGUID != Guid.Empty))
            {
                if (oBOD.TableName.ToLower() == "Organizations".ToLower() || oBOD.TableName.ToLower() == "XIBOWhiteList_T".ToLower() || oBOD.TableName.ToLower() == "XIAPPUsers_AU_T".ToLower() || oBOD.TableName.ToLower() == "XIAppRoles_AR_T".ToLower() || oBOD.TableName.ToLower() == "XIAppUserRoles_AUR_T".ToLower() || oBOD.TableName.ToLower() == "XIUserOrgMapping_T".ToLower())
                {
                    oInfo = oUser.Get_UserInfo();
                    if (oInfo.sCoreDataBase != null)
                    {
                        var DataSource = oXID.Get_DataSourceDefinition(oInfo.sCoreDataBase);
                        var BODS = ((XIDataSource)DataSource.oResult);
                        sConntection = oXID.GetBODataSource(BODS.XIGUID.ToString(), 0);
                    }
                    else
                    {
                        sConntection = oXID.GetBODataSource(iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                    }

                }
                else if (oBOD.iOrgObject > 0 || oBOD.TableName == "RefTraceStage_T" || oBOD.TableName == "refValidTrace_T" || oBOD.TableName == "refLeadQuality_T" || oBOD.TableName == "TraceTransactions_T")
                {
                    //oInfo = oUser.Get_UserInfo();
                    //if (!string.IsNullOrEmpty(oInfo.sDatabaseName))
                    //{
                    //    var DataSource = oXID.Get_DataSourceDefinition(oInfo.sDatabaseName);
                    //    var BODS = ((XIDataSource)DataSource.oResult);
                    //    sConntection = oXID.GetBODataSource(BODS.XIGUID.ToString(), 0);
                    //}
                    //else
                    {
                        sConntection = oXID.GetBODataSource(iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(sSwitchDB))
                    {
                        var oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, sSwitchDB);
                        sConntection = oXID.GetBODataSource(oDataSource.XIGUID.ToString(), oBOD.FKiApplicationID);
                    }
                    else
                    {
                        sConntection = oXID.GetBODataSource(iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                    }
                }

            }
            else if (!string.IsNullOrEmpty(sSwitchDB))
            {
                var oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, sSwitchDB);
                sConntection = oXID.GetBODataSource(oDataSource.XIGUID.ToString(), oBOD.FKiApplicationID);
            }
            else
                sConntection = oXID.GetBODataSource(iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);

            Connection = new XIDBAPI(sConntection);
            this.BOD = oBOD;
            if (bIsResolveFK)
            {
                ResolveFKColumns(oBOD, oInfo.sCoreDataBase);
            }
            if (Query.ToLower().Contains("case"))
            {
            }
            else
            {
                ResolveGroup(oBOD);
            }
            if (IsFkGUID)
            {
                var ActiveBoD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, sParentBO);
                List<string> Whrparams = new List<string>();
                //var sChangeWhrCondition = oCache.Get_ParamVal(sSessionID, sGUID, "", "{XIP|" + oCopy.ID + "_ChangeWhrCondition}");
                if (ActiveBoD.bUID)
                {
                    // oCache.Set_ParamVal(sSessionID, sGUID, "", "{XIP|" + o1ClickD.ID + "_ChangeWhrCondition}", "yes", "", null);
                    var sQuery = Query.Split(new string[] { " where " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    //o1ClickC.Query.Split("where");
                    if (sQuery.Count > 1 && !string.IsNullOrEmpty(sQuery[1]))
                    {
                        string GroupBY = "";
                        if (sQuery[1].IndexOf(" group by", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var pos = sQuery[1].IndexOf("group by", StringComparison.OrdinalIgnoreCase);

                            GroupBY = sQuery[1].Substring(pos);
                            GroupBY = sQuery[1].Insert(0, " ");
                            sQuery[1] = sQuery[1].Substring(0, pos - 1);
                        }
                        else if (sQuery[1].IndexOf(" order by", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var pos = sQuery[1].IndexOf("order by", StringComparison.OrdinalIgnoreCase);
                            GroupBY = sQuery[1].Substring(pos);
                            GroupBY = GroupBY.Insert(0, " ");
                            sQuery[1] = sQuery[1].Substring(0, pos - 1);
                        }
                        Query = sQuery[0];
                        Query = string.Concat(Query, GroupBY);
                        var sWhrL = sQuery[1].Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var spt in sWhrL)
                        {
                            var splits = spt.Split('=').ToList();
                            if (splits[0].ToLower() == ActiveBoD.sPrimaryKey.ToLower() && oBOD.Attributes.ContainsKey(ActiveBoD.sPrimaryKey.ToLower() + "xiguid"))
                            {
                                splits[0] = ActiveBoD.sPrimaryKey + "XIGUID";
                            }
                            Whrparams.Add(splits[0] + "=" + splits[1]);

                        }
                        if (Whrparams.Count > 0)
                        {
                            var sCondition = string.Join(" and ", Whrparams);
                            Query = string.Concat(sQuery[0], " where ", sCondition, GroupBY);
                        }
                    }

                }
            }
            //**********************************Check and build Query for result using hierarchy ********************************
            if (oBOD.bIsHierarchy)
            {
                string sHierarchyCondition = string.Empty;
                if (!string.IsNullOrEmpty(oInfo.sViewHierarchy) && oInfo.sViewHierarchy.ToLower() == XIConstant.HierarchyAll.ToLower())
                    sHierarchyCondition = null;
                else if (!string.IsNullOrEmpty(oInfo.sViewHierarchy) && oBOD.Attributes.Keys.Contains("sHierarchy"))
                {
                    int i = 1;
                    foreach (var s in oInfo.sViewHierarchy.Split('|'))
                    {
                        sHierarchyCondition += (!string.IsNullOrEmpty(sHierarchyCondition) ? " Or " : "") + " sHierarchy like '%'+@param" + i + "+'%' ";
                        AddSqlParameters("@param" + i, s);
                        i++;
                    }
                }
                else if (string.IsNullOrEmpty(oInfo.sViewHierarchy))
                {
                    sHierarchyCondition = "sHierarchy like '%'+@Hierarchy+'%' ";
                    AddSqlParameters("@Hierarchy", XIConstant.HierarchyNull);
                }
                if (!string.IsNullOrEmpty(sHierarchyCondition))
                    Query = AddSearchParameters(Query, sHierarchyCondition);
            }
            string sSearchKey = string.Empty;
            //*********************************************************************************************************************
            if (!string.IsNullOrEmpty(SearchType))
            {
                if (SearchType.ToLower() == "FilterSearch".ToLower())
                {
                    if (Fields != null && Fields.Length > 0)
                    {
                        var Condition = GetDynamicSearchStrings(Fields, Optrs, Values, oBOD);
                        if (Condition.Length > 0)
                        {
                            Query = AddSearchParameters(Query, Condition);
                            if (Query.Contains("WHERE") == true && Query.Contains("{XIP|") == true)
                            {
                                Query = Query.Replace("WHERE", "and");
                            }
                        }
                    }
                }
                else
                {
                    string sGroupName = string.Empty;
                    if (FKiSearchGroup > 0)
                    {
                        var GrpD = oBOD.Groups.Values.ToList().Where(m => m.ID == FKiSearchGroup).FirstOrDefault();
                        if (GrpD != null)
                        {
                            sGroupName = GrpD.GroupName;
                        }
                    }
                    else
                    {
                        sGroupName = SearchType.ToLower() == "quick" ? "quick search" : "natural search";
                    }
                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        sSearchKey = SearchText;
                        XIDGroup oGroupD;
                        if (oBOD.Groups.TryGetValue(sGroupName, out oGroupD))
                        {
                            SearchText = ResolveSearchString(oGroupD, SearchText, oInfo.sCoreDataBase);
                            SearchText = "(" + SearchText + ")";
                        }
                    }
                }
            }
            else
            {
                SearchType = "";
            }
            string sHideSelect = string.Empty;
            bool bManualGUID = false;
            if ((oOneClickParameters.Where(x => x.iType == 10).Count() == 0 || oOneClickParameters.Where(x => x.sName == "XIGUID").Count() == 0) && oBOD.Attributes.ContainsKey("xiguid") && (o1ClickD != null && !o1ClickD.bIsMultiBO) && !oBOD.bUID && bIsHidden)
            {
                if (oOneClickParameters.Where(x => x.iType == 10).Count() == 0)
                {
                    bManualGUID = true;
                }
                oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = "XIGUID", sValue = "{XIP|XIGUID}" });
            }
            if (oOneClickParameters.Where(x => x.sName.ToLower() == "XIGUID".ToLower()).FirstOrDefault() == null && bIsHidden)
            {
                oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = "XIGUID", sValue = "{XIP|XIGUID}" });
            }
            if (oOneClickParameters.Where(x => x.iType == 10).Count() > 0 || oBOD.bUID)
            {
                //if (DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.ResultList.ToString()))
                //{
                //if (oOneClickParameters.Where(x => x.iType == 10).Count() == 0)
                //{
                //    oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = "XIGUID", sValue = "{XIP|XIGUID}" });
                //}
                if (oOneClickParameters.Where(x => x.sName == "XIGUID").Count() == 0 && bIsHidden)
                {
                    oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = "XIGUID", sValue = "{XIP|XIGUID}" });
                }
                var Type = oOneClickParameters.Select(i => i.iType == 50).FirstOrDefault();
                if (bIsMultiBO && !Type)
                {
                    var GUIDHide = oOneClickParameters.Where(x => x.sName == "XIGUID").FirstOrDefault();
                    oOneClickParameters.Remove(GUIDHide);
                }
                //else if (oOneClickParameters.Where(x => x.sName == "XIGUID").Count() == 0)
                //{
                //    oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = "XIGUID", sValue = "{XIP|XIGUID}" });
                //}
                var ohideParams = oOneClickParameters.Where(x => x.iType == 10).Select(m => m.sName).ToList();
                if (ohideParams != null && ohideParams.Count() > 0)
                {
                    var oHideparm1 = ohideParams.Where(m => m.Contains(".")).ToList();
                    var oHideparm2 = ohideParams.Where(m => !m.Contains(".")).ToList();
                    var sConcatStr = oHideparm2.Select(f => "CONCAT('" + f + "','__',[" + oBOD.TableName + "]." + f + ")").ToList();
                    var sConcatStr2 = oHideparm1.Select(f => "CONCAT('" + f + "','__'," + f + ")").ToList();
                    sConcatStr = sConcatStr.Concat(sConcatStr2).ToList();
                    if (sConcatStr.Count() > 1)
                    {
                        sHideSelect = "CONCAT(" + string.Join(",':',", sConcatStr) + ") AS 'HiddenData'";
                    }
                    else
                    {
                        sHideSelect = sConcatStr.FirstOrDefault() + " AS 'HiddenData'";
                    }
                }
                //}
            }

            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(SearchText))
            {
                Query = AddSearchParameters(Query, SearchText, sJoins);
            }
            if (Query.IndexOf("izxdeleted", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                Query = Regex.Replace(Query, "izxdeleted", XIConstant.Key_XIDeleted, RegexOptions.IgnoreCase);
            }
            if (oBOD != null && oBOD.sDeleteRule == "10")
            {
                if (Query.IndexOf(XIConstant.Key_XIDeleted, StringComparison.CurrentCultureIgnoreCase) > 0)
                {

                }
                else
                {
                    string sCondition = "[" + oBOD.TableName + "]." + XIConstant.Key_XIDeleted + "= 0";
                    Query = AddSearchParameters(Query, sCondition, sJoins);
                }
            }

            if (!string.IsNullOrEmpty(sHideSelect))
            {
                Query = AddSelectPart(Query, sHideSelect, "append");
                if (sOrginalQuery.ToLower().Contains(" group by") && sHideSelect.ToLower().Contains("xiguid"))
                {
                    Query = string.Concat(Query, ", [" + oBOD.TableName + "].XIGUID");
                }
            }
            if (!string.IsNullOrEmpty(sParentWhere))
            {
                if (sParentWhere.Contains(" and "))
                {
                    var Split = sParentWhere.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var spt in Split)
                    {
                        var splits = spt.Split('=').ToList();
                        AddSqlParameters("@" + splits[0], splits[1]);
                        var sParentWhr = "[" + oBOD.TableName + "]." + splits[0] + "=@" + splits[0];
                        Query = AddSearchParameters(Query, sParentWhr, sJoins);
                    }
                }
                else
                {
                    var splits = sParentWhere.Split('=').ToList();
                    AddSqlParameters("@" + splits[0], splits[1]);
                    var sParentWhr = "[" + oBOD.TableName + "]." + splits[0] + "=@" + splits[0];
                    Query = AddSearchParameters(Query, sParentWhr, sJoins);
                }
            }
            string sMessage = string.Empty;
            if (!string.IsNullOrEmpty(oInfo.sWhereField) && oBOD != null && oBOD.Attributes.ContainsKey(oInfo.sWhereField.ToLower()) && bIsApplyActorWhereClause)
            {
                SearchText = oBOD.TableName + "." + oInfo.sWhereField + " = " + oInfo.iWhereFieldValue;
                Query = AddSearchParameters(Query, SearchText);
            }

            if (oBOD.iOrgObject > 0 && Query.IndexOf("FKiOrgID", StringComparison.CurrentCultureIgnoreCase) == -1) { 
                if(oBOD.iOrgObject == 1)
                {
                    if (string.IsNullOrEmpty(sBOIndentifier))
                    {
                        sBOIndentifier = oBOD.TableName;
                    }
                    SearchText = sBOIndentifier.Length > 0 ? $"{sBOIndentifier}.FKiOrgID = @FKiOrgID" : "FKiOrgID = @FKiOrgID";
                    AddSqlParameters("@FKiOrgID", oInfo.iOrganizationID);
                        Query = AddSearchParameters(Query, SearchText);
                }
                else if(oBOD.iOrgObject == 2)
                {
                    SearchText = sBOIndentifier.Length > 0 ? $"({sBOIndentifier}.FKiOrgID = @FKiOrgID or {sBOIndentifier}.FKiOrgID is null)" : "(FKiOrgID = @FKiOrgID or FKiOrgID is null)";
                    AddSqlParameters("@FKiOrgID", oInfo.iOrganizationID);
                    Query = AddSearchParameters(Query, SearchText);
                }
            }
            if (RoleIDs != null && RoleIDs.Count() > 0 && RoleIDs.Contains(oInfo.iRoleID) || true)
            {
                if (bIsLockToUser && !string.IsNullOrEmpty(oInfo.sRoleName) && oInfo.sRoleName.ToLower() == xiEnumSystem.EnumRoles.WebUsers.ToString().ToLower() && !bIsXILoad && !bIsMultiBO)
                {
                    SearchText = "FKiUserID = @FKiUserID";
                    AddSqlParameters("@FKiUserID", oInfo.iUserID);
                    Query = AddSearchParameters(Query, SearchText);
                }
                if (NVPairs != null)
                {
                    var Query = ReplaceQueryString(NVPairs);
                }
                if (Query.Contains("{") || Query.Contains("|"))
                {
                    oCResult.sMessage = "Critical Error: Unresolved Query in GetOneClickResult Method, Query Name : " + Name + ", Query : " + Query + " and ID: " + ID;
                    SaveErrortoDB(oCResult);
                }
                string Count = "";
                if (sOrginalQuery.ToLower().Contains(" distinct"))
                {
                    // Query = Query.Insert(7, "distinct ");
                }
                VisibleQuery = Query;
                if (ListSqlParams != null)
                {
                    foreach (var parm in ListSqlParams)
                    {
                        VisibleQuery = VisibleQuery.Replace(parm.ParameterName, parm.Value.ToString());
                        var valueofParam = parm.Value.ToString();

                        if (valueofParam != null && valueofParam.IndexOf(",") > 0)
                        {
                            if (!string.IsNullOrEmpty(Query))
                            {
                                Query = Query.Replace(parm.ParameterName, valueofParam);
                            }
                        }
                    }
                }
                if (Query.ToLower().Contains("distinct") || Query.ToLower().Contains("case") || Query.ToLower().Contains("pivot"))
                {
                    Count = Connection.GetRecordCount(CommandType.Text, Query, ListSqlParams);
                }
                else
                {
                    string StarQuery = ReplaceQueryStringWithStar(Query);
                    Count = Connection.GetTotalCount(CommandType.Text, StarQuery, ListSqlParams);
                }
                if (iSkip >= 0 && iTake > 0 && Query.ToLower().Contains(" order by "))
                {
                    //string strorderby = string.Empty;
                    //if (!Query.ToLower().Contains(" order by "))
                    //{
                    //    strorderby = " order by id desc ";
                    //}
                    if (TableColumns != null)
                    {
                        string sortExpression = TableColumns[iSortCol];
                        string sortDirection = sSortDir;
                        var sOrderNew = string.Empty;
                        if (!string.IsNullOrEmpty(sortDirection))
                        {
                            //sortDirection = "desc";
                            sOrderNew = " order by " + sortExpression + " " + sortDirection;
                        }

                        var sOrderI = Query.ToLower().IndexOf(" order by ");
                        var sOrderOld = Query.Substring(sOrderI, Query.Length - sOrderI);
                        //sOrderOld = sOrderOld.Replace(" order by ", "");
                        //var ords = sOrderOld.Split(' ').ToList();
                        //var OrdCol = ords[0];
                        //var OrdDir = ords[1];

                        //sOrderOld = " order by " + OrdCol + " " + OrdDir;
                        if (string.IsNullOrEmpty(OrderFields))
                            Query = Query.ToLower().Contains(" order by  order") || (Query.ToLower().Contains(" order by") && sortExpression != "Select" && !string.IsNullOrEmpty(sOrderNew)) ? Query.Replace(sOrderOld, sOrderNew) : Query.ToLower().Contains("asc") || Query.ToLower().Contains("desc") ? Query : Query + " " + sortDirection;
                        //Query = Query.Replace(sOrderOld, sOrderNew);

                        Query = Query + " OFFSET @iSkip ROWS FETCH NEXT @iTake ROWS ONLY";
                        AddSqlParameters("@iSkip", iSkip);
                        AddSqlParameters("@iTake", iTake);
                    }
                    else if(bIsMultiBO && TableColumns == null)
                    {
                        Query = Query + " OFFSET @iSkip ROWS FETCH NEXT @iTake ROWS ONLY";
                        AddSqlParameters("@iSkip", iSkip);
                        AddSqlParameters("@iTake", iTake);
                    }
                }
                else if (iSkip >= 0 && iTake > 0 && !Query.ToLower().Contains(" order by "))
                {
                    string sortExpression = string.Empty;
                    if (TableColumns != null)
                    {
                        if (TableColumns[iSortCol].ToLower() == "select")
                        {
                            sortExpression = TableColumns[iSortCol + 1];
                        }
                        else
                        {
                            sortExpression = TableColumns[iSortCol];
                        }
                        string sortDirection = sSortDir;
                        if (string.IsNullOrEmpty(sortDirection))
                        {
                            sortDirection = "desc";
                        }
                        var sOrderNew = "";
                        if (sortExpression.Contains("[" + oBOD.TableName + "]."))
                        {
                            sOrderNew = " order by " + sortExpression + " " + sortDirection;
                        }
                        else
                        {
                            sOrderNew = " order by [" + oBOD.TableName + "]." + sortExpression + " " + sortDirection;
                        }
                        Query = Query + sOrderNew;

                        Query = Query + " OFFSET @iSkip ROWS FETCH NEXT @iTake ROWS ONLY";
                        AddSqlParameters("@iSkip", iSkip);
                        AddSqlParameters("@iTake", iTake);
                    }
                }
                //var oBOIns = (DataTable)Connection.ExecuteQuery(Query);
                DataTable oBOIns = new DataTable();
                #region QUERYCACHE_CODE
                if (CategoryID == Convert.ToInt32(Enumxicache.QueryCache) && DisplayAs == Convert.ToInt32(xi1ClcikDisplayAS.List) && !string.IsNullOrEmpty(Query)) // check Query cache is Enabled or not
                {
                    string Queryhash = Utility.ConvertBase64(Utility.RemoveCharacters(Query.Trim(), "", "", " "));
                    string sQcachekey = ID.ToString() + "_" + Queryhash;
                    object oCached = oCache.GetFromCache(sQcachekey);
                    if (oCached == null) // Cache is empty then get from DB and Insert into Cache
                    {
                        //Execute Query and Insert into Cache
                        oBOIns = (DataTable)Connection.ExecuteQuery(Query, ListSqlParams);
                        oCache.InsertIntoCache(oBOIns, sQcachekey);
                    }
                    else
                    {
                        oBOIns = (DataTable)oCached;
                    }
                }
                else
                {
                    VisibleQuery = Query;
                    if (ListSqlParams != null)
                    {
                        foreach (var parm in ListSqlParams)
                        {
                            VisibleQuery = VisibleQuery.Replace(parm.ParameterName, parm.Value.ToString());
                        }
                    }
                    oBOIns = (DataTable)Connection.ExecuteQuery(Query, ListSqlParams);
                }
                #endregion

                DataView dv = oBOIns.DefaultView;
                //if (!string.IsNullOrEmpty(sortExpression.Trim()) && !string.IsNullOrEmpty(sortDirection))
                //{
                //    dv.Sort = sortExpression + " " + sortDirection;
                //}
                oBOIns = dv.ToTable();

                Dictionary<string, XIIAttribute> dictionary = new Dictionary<string, XIIAttribute>();
                Dictionary<string, XIIValue> XIValuedictionary = new Dictionary<string, XIIValue>();
                var j = 0;

                if (oBOD != null)
                {
                    List<DataRow> Rows = new List<DataRow>();
                    //iTotalRecords = oBOIns.AsEnumerable().ToList().Count();
                    iTotalRecords = Convert.ToInt32(Count);
                    if (iSkip >= 0 && iTake > 0 && !Query.ToLower().Contains(" order by "))
                    {
                        Rows = oBOIns.AsEnumerable().Skip(iSkip).Take(iTake).ToList();
                    }
                    else
                    {
                        Rows = oBOIns.AsEnumerable().ToList();
                    }
                    //iTotaldisplayRecords = Convert.ToInt32(Rows.Count());
                    //var AllCols = oBOIns.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                    TotalColumns = new List<string>();
                    //OptionListCols = OptionListCols ?? new List<string>();
                    OptionListCols = OptionListCols ?? new Dictionary<string, XIDBO>();
                    FKAttributes = FKAttributes ?? new List<string>();
                    CssAttributes = CssAttributes ?? new List<string>();
                    foreach (DataRow row in Rows)
                    {
                        XIIBO oBOII = new XIIBO();
                        dictionary = Enumerable.Range(0, oBOIns.Columns.Count)
                            .ToDictionary(i => oBOIns.Columns[i].ColumnName.ToLower(), i => new XIIAttribute
                            {
                                sName = oBOIns.Columns[i].ColumnName,
                                //sValue = OptionListCols.Contains(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), oBOD) : row.ItemArray[i].ToString(),
                                //sValue = SearchType.ToLower() == "quick" ? Regex.Replace(OptionListCols.ContainsKey(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOIns.Columns[i].ColumnName.ToLower()]) : row.ItemArray[i].ToString(), sSearchKey, "<span class=\"themecolor\">" + sSearchKey + "</span>", RegexOptions.IgnoreCase) : OptionListCols.ContainsKey(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOIns.Columns[i].ColumnName.ToLower()]) : row.ItemArray[i].ToString(),
                                sValue = (!string.IsNullOrEmpty(SearchType) && SearchType.ToLower() == "quick") ? Regex.Replace(OptionListCols.ContainsKey(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOIns.Columns[i].ColumnName.ToLower()], oBOIns.Columns, row.ItemArray) : row.ItemArray[i].ToString(), sSearchKey, sSearchKey, RegexOptions.IgnoreCase) : OptionListCols.ContainsKey(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOIns.Columns[i].ColumnName.ToLower()], oBOIns.Columns, row.ItemArray) : FKAttributes.Contains(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckFK(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), oBOD) : CssAttributes.Contains(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), oBOD, oBOIns.Columns, row.ItemArray) : row.ItemArray[i].ToString(),

                                sPreviousValue = row.ItemArray[i].ToString(),
                                sDisplayName = oBOD.Attributes.ContainsKey(oBOIns.Columns[i].ColumnName) ? oBOD.AttributeD(oBOIns.Columns[i].ColumnName).LabelName : "",
                                //iValue = TotalColumns.Contains(oBOIns.Columns[i].ColumnName.ToLower()) ? (!string.IsNullOrEmpty(row.ItemArray[i].ToString()) ? (Convert.ToInt32(row.ItemArray[i].ToString())) : 0) : 0,
                            }, StringComparer.CurrentCultureIgnoreCase);
                        oBOII.Attributes = dictionary;
                        if (oBOII.Attributes.ContainsKey("hiddendata") && !string.IsNullOrEmpty(oBOII.Attributes["hiddendata"].sValue))
                        {
                            var sHiddenAttrArray = oBOII.Attributes["hiddendata"].sValue.Split(':');
                            for (var Attr = 0; Attr < sHiddenAttrArray.Length; Attr++)
                            {
                                if (sHiddenAttrArray[Attr] != "")
                                {
                                    var oAttr = sHiddenAttrArray[Attr].Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    if (oAttr.Count > 1 && !string.IsNullOrEmpty(oAttr[0]) && oAttr[0].ToLower() == "xiguid")
                                    {
                                        oBOII.XIGUID = oAttr[1];
                                    }
                                    if (oAttr[0].ToLower() == "xiguid" && bManualGUID)
                                    {
                                        oBOII.Attributes["hiddendata"].sValue = oBOII.Attributes["hiddendata"].sValue.Replace(sHiddenAttrArray[Attr], "").TrimEnd(':');
                                    }
                                }
                            }
                        }
                        if (DisplayAs != (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.ResultList.ToString()) || bManualGUID)
                        {
                            oBOII.Attributes.Remove("hiddendata");
                        }
                        XIValuedictionary = Enumerable.Range(0, oBOIns.Columns.Count)
                             .ToDictionary(i => oBOIns.Columns[i].ColumnName, i => new XIIValue
                             {
                                 sValue = row.ItemArray[i].ToString(),
                                 sDisplayName = oBOD.Attributes.ContainsKey(oBOIns.Columns[i].ColumnName) ? oBOD.AttributeD(oBOIns.Columns[i].ColumnName).LabelName : "",
                             }, StringComparer.CurrentCultureIgnoreCase);
                        oBOII.XIIValues = XIValuedictionary;
                        oBOII.iBODID = oBOD.BOID;
                        //if (oBOD != null)
                        //{
                        //    if (oBOII.BOD == null)
                        //    {
                        //        oBOII.BOD = oBOD;
                        //    }
                        //}
                        nBOIns[j.ToString()] = oBOII;
                        j++;
                    }
                    if (DisplayAs == Convert.ToInt32(xi1ClcikDisplayAS.List) || (DisplayAs == Convert.ToInt32(xi1ClcikDisplayAS.ResultList) && !string.IsNullOrEmpty(sTotalColumns)))
                    {
                        //var WhereQuery = "";
                        var FromQuery = "";
                        //var colName = oBOD.Attributes.Values.Where(m => m.IsTotal == true).ToList();
                        var colName = new List<string>();
                        if (!string.IsNullOrEmpty(sTotalColumns))
                        {
                            colName = sTotalColumns.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                        //if (colName.Count() > 0 && Query.ToLower().Contains(" where "))
                        //{
                        //    int index = Query.ToLower().IndexOf(" where ");
                        //    if (index != -1)
                        //    {
                        //        WhereQuery = Query.Substring(index);
                        //    }
                        //    index = WhereQuery.ToLower().IndexOf(" order by ");
                        //    if (index != -1)
                        //    {
                        //        WhereQuery = WhereQuery.Substring(0, index);
                        //    }
                        //}
                        if (colName.Count() > 0)
                        {
                            //int index = Query.ToLower().IndexOf(" from ");
                            //FromQuery = Query.Substring(index);
                            int index = sOrginalQuery.ToLower().IndexOf(" from ");
                            FromQuery = sOrginalQuery.Substring(index);
                            if (FromQuery.ToLower().Contains(" group by "))
                            {
                                index = FromQuery.ToLower().IndexOf(" group by ");
                                if (index != -1)
                                {
                                    FromQuery = FromQuery.Substring(0, index);
                                }
                            }
                            if (FromQuery.ToLower().Contains(" order by "))
                            {
                                index = FromQuery.ToLower().IndexOf(" order by ");
                                if (index != -1)
                                {
                                    FromQuery = FromQuery.Substring(0, index);
                                }
                            }
                            Dictionary<string, XIIBO> nTotRows = Get_TotalQuery(oBOD, FromQuery);
                            if (nTotRows != null && nTotRows.Values.Count() > 0)
                            {
                                foreach (var rtot in nTotRows.Values)
                                {
                                    nBOIns[j.ToString()] = rtot;
                                    j++;
                                }
                            }
                        }
                    }
                }
                oDataSet = nBOIns;
            }
            else
            {
                sMessage = "Access Denied";
            }

            //oDataSet.ToList().ForEach(m =>
            //{
            //    m.Value.BOD = oBOD;
            //    m.Value.Attributes.ToList().ForEach(s => s.Value.BOD = oBOD);
            //    var dtrow = m.Value.Attributes.Values;
            //    oDataTableResult = new List<string[]>();
            //    oDataTableResult.Add(dtrow.Select(n => n.sResolvedValue).ToArray());
            //    m.Value.Attributes.ToList().ForEach(s => s.Value.BOD = null);
            //});
            watch.Stop();
            var iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            Cache_Performance(Name, iLapsedTime, XIGUID, 30);
            return nBOIns;
        }
        public CResult GetList()
        {
            CResult oCResult = new CResult();
            DataTable oDataTable = Execute_Query();
            Dictionary<string, XIIBO> nBOIns = new Dictionary<string, XIIBO>();
            Dictionary<string, XIIAttribute> dictionary = new Dictionary<string, XIIAttribute>();
            Dictionary<string, XIIValue> XIValuedictionary = new Dictionary<string, XIIValue>();
            var j = 0;
            foreach (DataRow row in oDataTable.Rows)
            {
                XIIBO oBOII = new XIIBO();
                dictionary = Enumerable.Range(0, oDataTable.Columns.Count)
                    .ToDictionary(i => oDataTable.Columns[i].ColumnName, i => new XIIAttribute { sName = oDataTable.Columns[i].ColumnName, sValue = row.ItemArray[i].ToString(), sPreviousValue = row.ItemArray[i].ToString() }, StringComparer.CurrentCultureIgnoreCase);
                oBOII.Attributes = dictionary;
                XIValuedictionary = Enumerable.Range(0, oDataTable.Columns.Count)
                 .ToDictionary(i => oDataTable.Columns[i].ColumnName, i => new XIIValue { sValue = row.ItemArray[i].ToString() }, StringComparer.CurrentCultureIgnoreCase);
                oBOII.XIIValues = XIValuedictionary;
                nBOIns[j.ToString()] = oBOII;
                j++;
            }
            oCResult.oResult = nBOIns;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            return oCResult;
        }
        public DataTable Execute_Query()
        {
            DataTable oDataTable = new DataTable();
            try
            {
                XIInfraUsers oUser = new XIInfraUsers();
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                sOrginalQuery = Query;
                string sBOIndentifier = GetBOIndentifierFromQuery();
                XIDBO oBOD = new XIDBO();
                if (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOIDXIGUID.ToString());
                }
                else if (BOID > 0)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOID.ToString());
                }
                else if (Name != null)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, Name, null);
                }
                if (oBOD != null && oBOD.iOrgObject > 0 && Query.IndexOf("FKiOrgID", StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    if (oBOD.iOrgObject == 1)
                    {
                        SearchText = sBOIndentifier.Length > 0 ? $"{sBOIndentifier}.FKiOrgID = @FKiOrgID" : "FKiOrgID = @FKiOrgID";
                        AddSqlParameters("@FKiOrgID", oInfo.iOrganizationID);
                        Query = AddSearchParameters(Query, SearchText);
                        Query = Query.Replace("@FKiOrgID", ""+oInfo.iOrganizationID);
                    }
                    else if (oBOD.iOrgObject == 2)
                    {
                        SearchText = sBOIndentifier.Length > 0 ? $"({sBOIndentifier}.FKiOrgID = @FKiOrgID or {sBOIndentifier}.FKiOrgID is null)" : "(FKiOrgID = @FKiOrgID or FKiOrgID is null)";
                        AddSqlParameters("@FKiOrgID", oInfo.iOrganizationID);
                        Query = AddSearchParameters(Query, SearchText);
                        Query = Query.Replace("@FKiOrgID", "" + oInfo.iOrganizationID);
                    }
                }

                int iDataSource = DataSource == 0 ? oBOD.iDataSource : DataSource;
                Guid iDataSourceXIGUID = DataSource == 0 ? oBOD.iDataSourceXIGUID : DataSourceXIGUID;
                XIDXI oXID = new XIDXI();
                string sConntection = oXID.GetBODataSource(iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                Connection = new XIDBAPI(sConntection);
                oDataTable = (DataTable)Connection.Execute_Query(Query);
            }
            catch (Exception ex)
            {
                CResult oCResult = new CResult();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oDataTable;
        }
        private string BulidJoin()
        {
            if (FKColumns != null && FKColumns.Count() > 0)
            {
                foreach (var fkbo in FKColumns)
                {
                    var sFKColumn = fkbo.Split('^')[0];
                    var sFKBO = fkbo.Split('^')[1];
                    var fkbod = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sFKBO, null);
                    sJoins = sJoins + " left join " + fkbod.TableName + " on " + BOD.TableName + "." + BOD.sPrimaryKey + "=" + fkbod.TableName + "." + sFKColumn;
                }
            }
            return sJoins;
        }

        private Dictionary<string, XIIBO> Get_TotalQuery(XIDBO oBOD, string FromQuery)
        {
            Dictionary<string, XIIBO> nBOIns = new Dictionary<string, XIIBO>();
            if (!nXIDBO.ContainsKey(sBOName))
            {
                nXIDBO[sBOName] = this.BOD;
            }
            var sTotalsQry = string.Empty;
            //var colName = oBOD.Attributes.Values.Where(m => m.IsTotal == true).ToList();
            var colName = sTotalColumns.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (colName != null && colName.Count() > 0)
            {
                //TotalColumns.AddRange(colName.Select(m => m.Name.ToLower()).ToList());
                foreach (var item in colName)
                {
                    if (item.Contains('.'))
                    {
                        TotalColumns.Add(item.Split('.')[1].Replace("[", "").Replace("]", ""));
                    }
                    else
                    {
                        TotalColumns.Add(item);
                    }
                }
                if (TableColumns != null && TableColumns.Count() > 0)
                {
                    foreach (var colHeader in TableColumns)
                    {
                        //var col = ""; 
                        var sCol = colHeader;
                        var oXIDBO = this.BOD;
                        if (colHeader.Contains("].[") || colHeader.Contains("]."))
                        {
                            //var value = colHeader.Split('.')[1];
                            //col = value.Substring(1, value.IndexOf(']') - 1);

                            var sFKBoName = colHeader.Split('.')[0].Trim().Replace("[", "").Replace("]", "");
                            sCol = colHeader.Split('.')[1].Replace("[", "").Replace("]", "");
                            if (sFKBoName.ToLower() != oXIDBO.Name.ToLower())
                            {
                                if (!nXIDBO.ContainsKey(sFKBoName))
                                {
                                    var oFKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sFKBoName, null);
                                    oXIDBO = oFKBOD;
                                    nXIDBO[sFKBoName] = oXIDBO;
                                }
                                else
                                {
                                    oXIDBO = nXIDBO[sFKBoName];
                                }
                            }

                        }
                        if (TotalColumns.Contains(sCol))
                        {
                            var oAttrD = oXIDBO.Attributes.Values.Where(m => m.Name.ToLower() == sCol.ToLower()).FirstOrDefault();
                            //    if (TotalColumns.Contains(col.ToLower()))
                            //{
                            //    var oAttrD = oBOD.Attributes.Values.Where(m => m.Name.ToLower() == col.ToLower()).FirstOrDefault();
                            if (oAttrD != null && !string.IsNullOrEmpty(oAttrD.Format))
                            {
                                var Char = oAttrD.Name.Select(c => char.IsUpper(c)).ToList();
                                var Position = Char.IndexOf(true);
                                if (Position == 1)
                                {
                                    char FirstLetter = oAttrD.Name[0];
                                    if ((FirstLetter == 'r' || FirstLetter == 'f') && oAttrD.TypeID == 90)
                                    {
                                        //sTotalsQry = sTotalsQry + "SUM(CONVERT(decimal(20,2),ISNULL([" + oBOD.TableName + "]." + col + ",0))) as " + col + ", ";
                                        sTotalsQry = sTotalsQry + "SUM(CONVERT(decimal(20,2),ISNULL([" + oXIDBO.TableName + "]." + sCol + ",0))) as " + sCol + ", ";
                                        //sTotalsQry = sTotalsQry + "FORMAT(SUM(CONVERT(decimal(20,2),ISNULL([" + oXIDBO.TableName + "]." + sCol + ",0))), 'C', '" + oAttrD.Format + "')" + " as '" + oAttrD.LabelName + "', ";
                                        //sTotalsQry = sTotalsQry + colHeader + " as '" + oAttrD.LabelName + "', ";
                                    }
                                    else
                                    {
                                        //sTotalsQry = sTotalsQry + "SUM(CONVERT(decimal(20,2),ISNULL([" + oBOD.TableName + "]." + col + ",0))) as " + col + ", ";
                                        sTotalsQry = sTotalsQry + "SUM(CONVERT(decimal(20,2),ISNULL([" + oXIDBO.TableName + "]." + sCol + ",0))) as " + sCol + ", ";
                                    }
                                }
                                else
                                {
                                    //sTotalsQry = sTotalsQry + "SUM(CONVERT(decimal(20,2),ISNULL([" + oBOD.TableName + "]." + col + ",0))) as " + col + ", ";
                                    sTotalsQry = sTotalsQry + "SUM(CONVERT(decimal(20,2),ISNULL([" + oXIDBO.TableName + "]." + sCol + ",0))) as " + sCol + ", ";
                                }
                            }
                            else
                            {
                                //sTotalsQry = sTotalsQry + "SUM(CONVERT(decimal(20,2),ISNULL([" + oBOD.TableName + "]." + col + ",0))) as " + col + ", ";
                                sTotalsQry = sTotalsQry + "SUM(CONVERT(decimal(20,2),ISNULL([" + oXIDBO.TableName + "]." + sCol + ",0))) as '" + sCol + "', ";
                            }

                        }
                        else if (colHeader.ToLower() != "select")
                        {
                            sTotalsQry = sTotalsQry + "'', ";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sTotalsQry))
                {
                    sTotalsQry = sTotalsQry.Substring(0, sTotalsQry.Length - 2);
                    sTotalsQry = "Select " + sTotalsQry + FromQuery;//" from " + oBOD.TableName + Where;//to do get where condition from data query
                    var Totals = (DataTable)Connection.ExecuteQuery(sTotalsQry, ListSqlParams);
                    var totRows = Totals.AsEnumerable().ToList();
                    Dictionary<string, XIIAttribute> dictionary = new Dictionary<string, XIIAttribute>();
                    int k = 0;
                    foreach (DataRow row in totRows)
                    {
                        XIIBO oBOII = new XIIBO();
                        dictionary = Enumerable.Range(0, Totals.Columns.Count)
                            .ToDictionary(i => Totals.Columns[i].ColumnName.ToLower(), i => new XIIAttribute
                            {
                                sName = Totals.Columns[i].ColumnName,
                                sValue = string.IsNullOrEmpty(row.ItemArray[i].ToString()) ? row.ItemArray[i].ToString() : (Totals.Columns[i].ColumnName.ToLower().StartsWith("r") ? Convert.ToDecimal(row.ItemArray[i]).ToString("c", CultureInfo.CreateSpecificCulture("en-GB")) : row.ItemArray[i].ToString()),
                            }, StringComparer.CurrentCultureIgnoreCase);
                        oBOII.Attributes = dictionary;
                        nBOIns[k.ToString()] = oBOII;
                        k++;
                    }
                }
            }
            return nBOIns;
        }

        public string CheckOptionList(string columnName, string sValue, XIDBO oBOD, DataColumnCollection Columns, object[] rowData)
        {
            var oAttrD = oBOD.Attributes[columnName.ToLower()];
            var oBOOption = oAttrD.OptionList.Where(m => m.sValues == sValue).FirstOrDefault();
            if (oBOOption != null)
            {
                var Cols = (from c in Columns.Cast<DataColumn>()
                            select c.ColumnName.ToLower()).ToList();
                var sOptionValue = oBOOption.sOptionName;
                string sUIStyle = oBOOption.sUIStyle;
                if (!string.IsNullOrEmpty(sOptionValue))
                {
                    if (!string.IsNullOrEmpty(sUIStyle))
                    {
                        if (sUIStyle.ToLower().StartsWith("class:"))
                        {
                            sUIStyle = sUIStyle.Replace("class:", "");
                            sValue = "<span class=" + sUIStyle + ">" + sOptionValue + "</span>";
                        }
                        else if (sUIStyle.ToLower().StartsWith("html:"))
                        {
                            sUIStyle = sUIStyle.Replace("html:", "");
                            var regx = new Regex("{.*?}");
                            var matchs = regx.Matches(sUIStyle);
                            if (matchs.Count > 0)
                            {
                                foreach (var match in matchs)
                                {
                                    var sMatch = match.ToString().Replace("{", "").Replace("}", "");
                                    if (sMatch != null && sMatch.Length > 0)
                                    {                                        
                                        if (Cols.Contains(sMatch.ToLower()))
                                        {
                                            var iIndex = Cols.IndexOf(sMatch.ToLower());
                                            var sResValue = rowData[iIndex].ToString();
                                            sUIStyle = sUIStyle.Replace(match.ToString(), sResValue);
                                        }                                            
                                    }
                                    else
                                    {
                                        sUIStyle = sUIStyle.Replace(match.ToString(), sOptionValue);
                                    }
                                }
                            }
                            sValue = sUIStyle;
                        }
                        else if (sUIStyle.ToLower().StartsWith("img:"))
                        {
                            sUIStyle = sUIStyle.Replace("img:", "");
                            if (sUIStyle.Contains("{}"))
                            {
                                sValue = sUIStyle.Replace("{}", sOptionValue);
                            }
                            else
                            {
                                sValue = sUIStyle;
                            }
                        }
                        else
                        {
                            sValue = "<span class=" + sUIStyle + ">" + sOptionValue + "</span>";
                        }
                    }
                    else
                        sValue = sOptionValue;
                }
                else if (sValue == "0")
                {
                    sValue = "";
                }
            }
           else if (!String.IsNullOrEmpty(oAttrD.sUIStyle))
            {
                var sAttrValue = sValue;
                string sUIStyle = oAttrD.sUIStyle;
                if (!string.IsNullOrEmpty(sAttrValue))
                {
                    if (!string.IsNullOrEmpty(sUIStyle))
                    {
                        if (sUIStyle.ToLower().StartsWith("class:"))
                        {
                            sUIStyle = sUIStyle.Replace("class:", "");
                            sValue = "<span class=" + sUIStyle + ">" + sAttrValue + "</span>";
                        }
                        else if (sUIStyle.ToLower().StartsWith("html:"))
                        {
                            sUIStyle = sUIStyle.Replace("html:", "");
                            sValue = sUIStyle.Replace("{}", sAttrValue);
                        }
                        else if (sUIStyle.ToLower().StartsWith("img:"))
                        {
                            sUIStyle = sUIStyle.Replace("img:", "");
                            if (sUIStyle.Contains("{}"))
                            {
                                sValue = sUIStyle.Replace("{}", sAttrValue);
                            }
                            else
                            {
                                sValue = sUIStyle;
                            }
                        }
                        else
                        {
                            sValue = "<span class=" + sUIStyle + ">" + sAttrValue + "</span>";
                        }
                    }
                    else
                        sValue = sAttrValue;
                }
                else if (sValue == "0")
                {
                    sValue = "";
                }
            }
            return sValue;
        }

        public string CheckFK(string columnName, string sValue, XIDBO oBOD)
        {
            var AttrD = oBOD.Attributes.Values.ToList().Where(m => m.LabelName.ToLower() == columnName.ToLower()).FirstOrDefault();
            if (AttrD?.iDefaultPopupIDXIGUID != null && AttrD?.iDefaultPopupIDXIGUID != Guid.Empty && !string.IsNullOrEmpty(sValue) && bIsGrid == false)
            {
                string sFKDrill = "<span class=\"popoverfk\" data-obj='" + oBOD.XIGUID + "' data-value='" + sValue + "' data-attr='" + AttrD.Name + "' data-fkbo='" + AttrD.sFKBOName + "'>" + sValue + " <a class=\"FKPopupDrill\" onclick=\"fncLoadFKPopup(event, this, '" + AttrD.iDefaultPopupIDXIGUID + "', '" + sValue + "', '" + oBOD.Name + "', '" + AttrD.sFKBOName + "', '" + AttrD.Name + "')\"><i class=\"icon-share-alt\" aria-hidden=\"true\"></i></a></span>";
                return sFKDrill;
            }
            else
            {
                return sValue;
            }
        }

        public Dictionary<string, XIIBO> OneClick_Run(bool bAccess = true, XID1Click o1ClickD = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Dictionary<string, XIIBO> nBOIns = new Dictionary<string, XIIBO>();
            CResult oCResult = new CResult();
            try
            {
                XIInfraUsers oUser = new XIInfraUsers();
                CUserInfo oInfo = new CUserInfo();
                string sBOIndentifier = GetBOIndentifierFromQuery();
                //oInfo = oInfo.GetUserInfo();
                if (bAccess)
                {
                    oInfo = oUser.Get_UserInfo();
                }
                string sMessage = string.Empty;
                if (o1ClickD != null)
                {
                    bIsApplyActorWhereClause = o1ClickD.bIsApplyActorWhereClause;
                }
                else
                {
                    bIsApplyActorWhereClause = true;
                }
                XIDBO oBOD = new XIDBO();
                if (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOIDXIGUID.ToString());
                }
                else if (BOID > 0)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOID.ToString());
                }
                else if (Name != null)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, Name, null);
                }
                var WhiteListCheck = System.Configuration.ConfigurationManager.AppSettings["WhitelistCheck"];
                if (WhiteListCheck == "yes")
                {
                    if (oBOD != null && oBOD.Name.ToLower() != "BO WhiteList".ToLower() && oBOD.BOID != 2459)
                    {
                        XIIBO oBOI = new XIIBO();
                        var oCR = oBOI.Check_Whitelist(oBOD.BOID, oInfo.iRoleID, oInfo.iOrganizationID, oInfo.iApplicationID, "query", oBOD.iLevel, oInfo.iLevel);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            var bUNAuth = (bool)oCR.oResult;
                            if (bUNAuth)
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                return new Dictionary<string, XIIBO>();
                            }
                        }
                    }
                }
                int iDataSource = oBOD.iDataSource;
                Guid DataSourceXIGUID = oBOD.iDataSourceXIGUID;
                XIDXI oXID = new XIDXI();
                if (iDataSource > 0 || (DataSourceXIGUID != null && DataSourceXIGUID != Guid.Empty))
                {
                    if (oBOD.TableName.ToLower() == "Organizations".ToLower() || oBOD.TableName.ToLower() == "XIBOWhiteList_T".ToLower() || oBOD.TableName.ToLower() == "XIAPPUsers_AU_T".ToLower() || oBOD.TableName.ToLower() == "XIAppRoles_AR_T".ToLower() || oBOD.TableName.ToLower() == "XIAppUserRoles_AUR_T".ToLower() || oBOD.TableName.ToLower() == "XIUserOrgMapping_T".ToLower())
                    {
                        if (oInfo.sCoreDataBase != null)
                        {
                            var DataSource = oXID.Get_DataSourceDefinition(oInfo.sCoreDataBase);
                            var BODS = ((XIDataSource)DataSource.oResult);
                            var sBODataSource = oXID.GetBODataSource(BODS.XIGUID.ToString(), 0);
                            Connection = new XIDBAPI(sBODataSource);
                        }
                        else
                        {
                            var sBODataSource = oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), 0);
                            Connection = new XIDBAPI(sBODataSource);
                        }
                    }
                    else if (oBOD.iOrgObject > 0 || oBOD.TableName == "RefTraceStage_T" || oBOD.TableName == "refValidTrace_T" || oBOD.TableName == "refLeadQuality_T" || oBOD.TableName == "TraceTransactions_T")
                    {
                        //oInfo = oUser.Get_UserInfo();
                        //if (oInfo.sDatabaseName != null)
                        //{
                        //    var DataSource = oXID.Get_DataSourceDefinition(oInfo.sDatabaseName);
                        //    var BODS = ((XIDataSource)DataSource.oResult);
                        //    var sBODataSource = oXID.GetBODataSource(BODS.XIGUID.ToString(), 0);
                        //    Connection = new XIDBAPI(sBODataSource);
                        //}
                        //else
                        {
                            var sBODataSource = oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), 0);
                            Connection = new XIDBAPI(sBODataSource);
                        }
                    }
                    else
                    {
                        string sConntection = string.Empty;
                        if (!string.IsNullOrEmpty(sSwitchDB))
                        {
                            var oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, sSwitchDB);
                            sConntection = oXID.GetBODataSource(oDataSource.XIGUID.ToString(), oBOD.FKiApplicationID);
                        }
                        else
                        {
                            sConntection = oXID.GetBODataSource(DataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                        }
                        Connection = new XIDBAPI(sConntection);
                    }

                }
                else if (!string.IsNullOrEmpty(sSwitchDB))
                {
                    string sConntection = string.Empty;
                    var oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, sSwitchDB);
                    sConntection = oXID.GetBODataSource(oDataSource.XIGUID.ToString(), oBOD.FKiApplicationID);
                    Connection = new XIDBAPI(sConntection);
                }
                else
                    Connection = new XIDBAPI(sConnectionString);


                if (!string.IsNullOrEmpty(oInfo.sWhereField) && oBOD != null && oBOD.Attributes.ContainsKey(oInfo.sWhereField.ToLower()) && bIsApplyActorWhereClause)
                {
                    SearchText = oInfo.sWhereField + " = " + oInfo.iWhereFieldValue;
                    Query = AddSearchParameters(Query, SearchText);
                }
                if (oBOD != null && oBOD.sDeleteRule == "10")
                {
                    if (Query.IndexOf(XIConstant.Key_XIDeleted, StringComparison.CurrentCultureIgnoreCase) > 0)
                    {

                    }
                    else
                    {
                        string sCondition = "[" + oBOD.TableName + "]." + XIConstant.Key_XIDeleted + "= 0";
                        Query = AddSearchParameters(Query, sCondition, sJoins);
                    }
                }
                if (!string.IsNullOrEmpty(Query) && oBOD != null && !IsStoredProcedure)
                {
                    ResolveGroup(oBOD);
                }
                if (oBOD.bIsHierarchy)
                {
                    string sHierarchyCondition = string.Empty;
                    if (!string.IsNullOrEmpty(oInfo.sViewHierarchy) && oInfo.sViewHierarchy.ToLower() == XIConstant.HierarchyAll.ToLower())
                        sHierarchyCondition = null;
                    else if (!string.IsNullOrEmpty(oInfo.sViewHierarchy) && oBOD.Attributes.Keys.Contains("sHierarchy"))
                    {
                        int i = 1;
                        foreach (var s in oInfo.sViewHierarchy.Split('|'))
                        {
                            sHierarchyCondition += (!string.IsNullOrEmpty(sHierarchyCondition) ? " Or " : "") + " sHierarchy like '%'+@param" + i + "+'%' ";
                            AddSqlParameters("@param" + i, s);
                            i++;
                        }
                    }
                    else if (string.IsNullOrEmpty(oInfo.sViewHierarchy))
                    {
                        sHierarchyCondition = "sHierarchy like '%'+@Hierarchy+'%' ";
                        AddSqlParameters("@Hierarchy", XIConstant.HierarchyNull);
                    }
                    if (!string.IsNullOrEmpty(sHierarchyCondition))
                        Query = AddSearchParameters(Query, sHierarchyCondition);
                }
                if (!string.IsNullOrEmpty(sParentWhere))
                {
                    if (sParentWhere.Contains(" and "))
                    {
                        var Split = sParentWhere.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var spt in Split)
                        {
                            var splits = spt.Split('=').ToList();
                            AddSqlParameters("@" + splits[0], splits[1]);
                            var sParentWhr = "[" + oBOD.TableName + "]." + splits[0] + "=@" + splits[0];
                            Query = AddSearchParameters(Query, sParentWhr, sJoins);
                        }
                    }
                    else
                    {
                        var splits = sParentWhere.Split('=').ToList();
                        AddSqlParameters("@" + splits[0], splits[1]);
                        var sParentWhr = "[" + oBOD.TableName + "]." + splits[0] + "=@" + splits[0];
                        Query = AddSearchParameters(Query, sParentWhr, sJoins);
                    }
                }

                if (oBOD.iOrgObject > 0 && Query.IndexOf("FKiOrgID", StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    if (oBOD.iOrgObject == 1)
                    {
                        SearchText = sBOIndentifier.Length > 0 ? $"{sBOIndentifier}.FKiOrgID = @FKiOrgID" : "FKiOrgID = @FKiOrgID";
                        AddSqlParameters("@FKiOrgID", oInfo.iOrganizationID);
                        Query = AddSearchParameters(Query, SearchText);
                    }
                    else if (oBOD.iOrgObject == 2)
                    {
                        SearchText = sBOIndentifier.Length > 0 ? $"({sBOIndentifier}.FKiOrgID = @FKiOrgID or {sBOIndentifier}.FKiOrgID is null)" : "(FKiOrgID = @FKiOrgID or FKiOrgID is null)";
                        AddSqlParameters("@FKiOrgID", oInfo.iOrganizationID);
                        Query = AddSearchParameters(Query, SearchText);
                    }
                }
                if (RoleIDs != null && RoleIDs.Count() > 0 && RoleIDs.Contains(oInfo.iRoleID) || true)
                {
                    if (bAccess)
                    {
                        if (bIsLockToUser && oInfo.sRoleName == "WebUsers" && !bIsXILoad && !bIsMultiBO)
                        {
                            SearchText = "FKiUserID = " + oInfo.iUserID;
                            Query = AddSearchParameters(Query, SearchText);
                        }
                    }
                    var oBOIns = new DataTable();
                    if (IsStoredProcedure)
                    {
                        ListSqlParams = new List<SqlParameter>();
                        if (NVs != null)
                        {
                        foreach (var NV in NVs)
                        {
                            ListSqlParams.Add(new SqlParameter { ParameterName = NV.sName, Value = NV.sValue });
                        }
                        }
                        oBOIns = (DataTable)Connection.ExecuteStoredProcedure(Query, ListSqlParams);
                    }
                    else
                    {
                        oBOIns = (DataTable)Connection.ExecuteQuery(Query, ListSqlParams);
                    }

                    Dictionary<string, XIIAttribute> dictionary = new Dictionary<string, XIIAttribute>();
                    Dictionary<string, XIIValue> XIValuedictionary = new Dictionary<string, XIIValue>();
                    var j = 0;
                    if (oBOD != null)
                    {
                        List<DataRow> Rows = new List<DataRow>();
                        Rows = oBOIns.AsEnumerable().ToList();
                        foreach (DataRow row in Rows)
                        {
                            XIIBO oBOII = new XIIBO();
                            dictionary = Enumerable.Range(0, oBOIns.Columns.Count)
                                .ToDictionary(i => oBOIns.Columns[i].ColumnName.ToLower(), i => new XIIAttribute
                                {
                                    sName = oBOIns.Columns[i].ColumnName,
                                    sValue = row.ItemArray[i].ToString(),
                                    sPreviousValue = row.ItemArray[i].ToString(),
                                    //sDisplayName
                                    sDisplayName = oBOD.Attributes.ContainsKey(oBOIns.Columns[i].ColumnName.ToLower()) ? oBOD.AttributeD(oBOIns.Columns[i].ColumnName.ToLower()).LabelName : "",
                                }, StringComparer.CurrentCultureIgnoreCase);
                            oBOII.Attributes = dictionary;
                            XIValuedictionary = Enumerable.Range(0, oBOIns.Columns.Count)
                                 .ToDictionary(i => oBOIns.Columns[i].ColumnName, i => new XIIValue
                                 {
                                     sValue = row.ItemArray[i].ToString(),
                                     sDisplayName = oBOD.Attributes.ContainsKey(oBOIns.Columns[i].ColumnName) ? oBOD.AttributeD(oBOIns.Columns[i].ColumnName).LabelName : "",
                                 }, StringComparer.CurrentCultureIgnoreCase);
                            oBOII.XIIValues = XIValuedictionary;

                            //if (oBOD != null)
                            //{
                            //    if (oBOII.BOD == null)
                            //    {
                            //        oBOII.BOD = oBOD;
                            //    }
                            //}
                            nBOIns[j.ToString()] = oBOII;
                            j++;
                        }
                    }
                }
                else
                {
                    sMessage = "Access Denied";
                }
                oDataSet = nBOIns;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - 1Click:" + XIGUID + " " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - 1ClickID: " + ID + " and 1Click Name: " + Name + " and Query: " + Query + "\r\n";
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            var iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            Cache_Performance(Name, iLapsedTime, XIGUID, 30);
            return nBOIns;
        }

        public DataTable OneClick_TableResult()
        {
            CResult oCResult = new CResult();
            DataTable oBOIns = new DataTable();
            string sBOIndentifier = GetBOIndentifierFromQuery();
            try
            {
                XIInfraUsers oUser = new XIInfraUsers();
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                //oInfo = oInfo.GetUserInfo();
                sOrginalQuery = Query;
                string sMessage = string.Empty;
                XIDBO oBOD = new XIDBO();
                if (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOIDXIGUID.ToString());
                }
                else if (BOID > 0)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOID.ToString());
                }
                else if (Name != null)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, Name, null);
                }
                if (oBOD.iOrgObject > 0 && Query.IndexOf("FKiOrgID", StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    if (oBOD.iOrgObject == 1)
                    {
                        if (string.IsNullOrEmpty(sBOIndentifier))
                        {
                            sBOIndentifier = oBOD.TableName;
                        }
                        SearchText = sBOIndentifier.Length > 0 ? $"{sBOIndentifier}.FKiOrgID = @FKiOrgID" : "FKiOrgID = @FKiOrgID";
                        AddSqlParameters("@FKiOrgID", oInfo.iOrganizationID);
                        Query = AddSearchParameters(Query, SearchText);
                    }
                    else if (oBOD.iOrgObject == 2)
                    {
                        SearchText = sBOIndentifier.Length > 0 ? $"({sBOIndentifier}.FKiOrgID = @FKiOrgID or {sBOIndentifier}.FKiOrgID is null)" : "(FKiOrgID = @FKiOrgID or FKiOrgID is null)";
                        AddSqlParameters("@FKiOrgID", oInfo.iOrganizationID);
                        Query = AddSearchParameters(Query, SearchText);
                    }
                }
                //int iDataSource = oBOD.iDataSource;
                XIDXI oXID = new XIDXI();
                string sConntection = oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                Connection = new XIDBAPI(sConntection);
                ResolveFKColumns(oBOD, oInfo.sCoreDataBase);
                if (!string.IsNullOrEmpty(SearchType))
                {
                    if (SearchType.ToLower() == "FilterSearch".ToLower())
                    {
                        if (Fields != null && Fields.Length > 0)
                        {
                            var Condition = GetDynamicSearchStrings(Fields, Optrs, Values, oBOD);
                            if (Condition.Length > 0)
                            {
                                Query = AddSearchParameters(Query, Condition);
                                if (Query.Contains("WHERE") == true && Query.Contains("{XIP|") == true)
                                {
                                    Query = Query.Replace("WHERE", "and");
                                }
                            }
                        }
                    }
                    else
                    {
                        string sGroupName = "natural search";
                        if (!string.IsNullOrEmpty(SearchType) && SearchType.ToLower() == "quick")
                        {
                            sGroupName = "quick search";
                        }
                        if (!string.IsNullOrEmpty(SearchText))
                        {
                            XIDGroup oGroupD;
                            if (oBOD.Groups.TryGetValue(sGroupName, out oGroupD))
                            {
                                SearchText = ResolveSearchString(oGroupD, SearchText, oInfo.sCoreDataBase);
                            }
                        }
                    }
                }
                if (RoleIDs != null && RoleIDs.Count() > 0 && RoleIDs.Contains(oInfo.iRoleID) || true)
                {
                    oBOIns = (DataTable)Connection.ExecuteQuery(Query, ListSqlParams);
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oBOIns;
        }
        public string ReplaceQueryStringWithStar(string sQuery)
        {
            string GroupBY = "", Condition = "";
            int pos, Qlength;
            int iLastIndex = CheckSubQuery(sQuery.ToLower(), " from ");
            int iFromIndex = sQuery.IndexOf(" from ", iLastIndex, StringComparison.CurrentCultureIgnoreCase);
            string sSubQuery = sQuery.Substring(0, iFromIndex);
            var SelectField = sSubQuery;
            sQuery = sQuery.Replace(sSubQuery, " count(*) ");
            string sFinalString = "select" + sQuery;
            if (sFinalString.IndexOf(" group by", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                SelectField = SelectField.Substring(SelectField.IndexOf(" "), SelectField.IndexOf(", ") - SelectField.IndexOf(" "));
                sFinalString = sFinalString.Replace("count(*)", SelectField);
                //pos = sFinalString.IndexOf("group by", StringComparison.OrdinalIgnoreCase);
                //Qlength = sFinalString.Length;
                //GroupBY = sFinalString.Substring(pos);
                //GroupBY = GroupBY.Insert(0, " ");
                //sFinalString = sFinalString.Substring(0, pos - 1);
                sFinalString = "select count(*) from (" + sFinalString + ") as Dual";
            }
            else if (sFinalString.IndexOf(" order by", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                pos = sFinalString.IndexOf("order by", StringComparison.OrdinalIgnoreCase);
                Qlength = sFinalString.Length;
                GroupBY = sFinalString.Substring(pos);
                GroupBY = GroupBY.Insert(0, " ");
                sFinalString = sFinalString.Substring(0, pos - 1);
            }
            return sFinalString;
        }
        public string ReplaceSelectWithStar(string sQuery)
        {
            int iLastIndex = CheckSubQuery(sQuery.ToLower(), " from ");
            int iFromIndex = sQuery.IndexOf(" from ", iLastIndex, StringComparison.CurrentCultureIgnoreCase);
            string sSubQuery = sQuery.Substring(0, iFromIndex);
            var SelectField = sSubQuery;
            sQuery = sQuery.Replace(sSubQuery, " * ");
            string sFinalString = "select" + sQuery;
            return sFinalString;
        }
        private string ResolveSearchString(XIDGroup oGroupD, string searchText, string sCoreDataBase)
        {
            string sSearchString = string.Empty;
            var QuickGroup = oGroupD.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (QuickGroup != null && QuickGroup.Count() > 0)
            {
                var sConcatStr = QuickGroup.Select(f => f.Contains('.') ? f + " like '%'+@SearchText+'%'" : "[" + BOD.TableName + "]." + f + " like '%'+@SearchText+'%'").ToList();
                sSearchString = sSearchString + string.Join(" or ", sConcatStr);
                //foreach (var item in QuickGroup)
                //{
                //    //sSearchString = sSearchString + item + " like '%" + SearchText + "%' or ";
                //    sSearchString = sSearchString + item + " like '%'+@SearchText+'%' or ";
                //}
            }
            if (FKColumns != null && FKColumns.Count() > 0)
            {
                foreach (var fkbo in FKColumns)
                {
                    var sFKColumn = fkbo.Split('^')[0];
                    if (QuickGroup.Contains(sFKColumn))
                    {
                        var sFKBO = fkbo.Split('^')[1];
                        var fkbod = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sFKBO, null);
                        if (fkbod.TableName == "XIAPPUsers_AU_T" || fkbod.TableName == "Organizations")
                        {
                            sJoins = sJoins + " left join " + sCoreDataBase + ".dbo." + fkbod.TableName + " on " + "[" + BOD.TableName + "]." + sFKColumn + "=  " + sCoreDataBase + ".dbo." + "[" + fkbod.TableName + "]." + fkbod.sPrimaryKey;
                        }
                        else
                        {
                            sJoins = sJoins + " left join " + fkbod.TableName + " on " + "[" + BOD.TableName + "]." + sFKColumn + "= [" + fkbod.TableName + "]." + fkbod.sPrimaryKey;
                        }

                        if (fkbod != null)
                        {
                            XIDGroup oFKGroupD;
                            if (fkbod.Groups.TryGetValue("label", out oFKGroupD))
                            {
                                var fkQuickGroup = oFKGroupD.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (fkQuickGroup != null && fkQuickGroup.Count() > 0)
                                {
                                    var sConcatStr = new List<string>();
                                    if (fkbod.TableName == "XIAPPUsers_AU_T" || fkbod.TableName == "Organizations")
                                    {
                                        sConcatStr = fkQuickGroup.Select(f => f.Contains('.') ? f + " like '%'+@SearchText+'%'" : sCoreDataBase + ".dbo.[" + fkbod.TableName + "]." + f + " like '%'+@SearchText+'%'").ToList();
                                    }
                                    else
                                    {
                                        sConcatStr = fkQuickGroup.Select(f => f.Contains('.') ? f + " like '%'+@SearchText+'%'" : "[" + fkbod.TableName + "]." + f + " like '%'+@SearchText+'%'").ToList();
                                    }
                                    sSearchString = sSearchString + " or " + string.Join(" or ", sConcatStr);
                                }
                            }
                        }
                    }
                }
            }
            //sSearchString = sSearchString.Substring(0, sSearchString.Length - 4);
            // Add Sql parameter 
            AddSqlParameters("@SearchText", SearchText);
            return sSearchString;
        }
        public void AddSqlParameters(string paramname, dynamic value)
        {
            ListSqlParams = ListSqlParams ?? new List<SqlParameter>();
            if (!string.IsNullOrEmpty(paramname) && !string.IsNullOrEmpty(value.ToString()))
            {
                if (!ListSqlParams.Any(d => d.ParameterName == paramname))
                {
                    SqlParameter osqlparam = new SqlParameter();
                    osqlparam.ParameterName = paramname;
                    osqlparam.Value = value;
                    ListSqlParams.Add(osqlparam);
                    //Add into Dapper params
                    // DapperParams.Add(paramname, value);
                }
            }
        }

        public void ResolveGroup(XIDBO oBOD)
        {
            var FromIndex = Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
            var SelectQuery = Query.Substring(0, FromIndex);
            SelectQuery = SelectQuery.TrimEnd();
            var ReplaceQry = Query.Substring(0, FromIndex);
            var regx = new Regex("{.*?}");
            var matchs = regx.Matches(ReplaceQry);
            if (matchs.Count > 0)
            {
                foreach (var match in matchs)
                {
                    List<string> oGrpHeadS = new List<string>();
                    var GroupD = new XIDGroup();
                    var sResGroup = match.ToString().Replace("{", string.Empty).Replace("}", string.Empty);
                    int id = 0;
                    if (oBOD != null && oBOD.Groups.Any(group => group.Key.ToLower() == sResGroup.ToLower()))
                    {
                        id = oBOD.GroupD(sResGroup).ID;
                    }
                    //int id = Convert.ToInt32(match.ToString().Substring(1, match.ToString().Length - 2));
                    var oGrpD = oBOD.Groups.Values.Where(m => m.ID == id).FirstOrDefault();
                    if (oGrpD != null && !oGrpD.IsMultiColumnGroup)
                    {
                        ReplaceQry = ReplaceQry.Replace("{" + id.ToString() + "}", oGrpD.BOSqlFieldNames);
                    }
                    else
                    {
                        //ReplaceQry = ReplaceQry.Replace("{" + id.ToString() + "}", oGrpD.BOFieldNames);
                        var sTabelName = oBOD.TableName;
                        var sBOFNames = string.Empty;
                        foreach (var item in oGrpD.BOFieldNames.Split(',').ToList())
                        {
                            sBOFNames = sBOFNames + "[" + sTabelName + "]." + item + ", ";
                        }
                        ReplaceQry = "SELECT " + sBOFNames.Substring(0, sBOFNames.Length - 2);
                        // ReplaceQry = ReplaceQry.Replace(match.ToString(), oGrpD.BOFieldNames);
                    }
                    Query = Query.Replace(SelectQuery, ReplaceQry);
                }
            }
        }

        public string GetDynamicSearchStrings(string Fields, string Optrs, string Values, XIDBO xibo)
        {
            string condition = string.Empty;
            var SFields = Fields.Split(',').ToList();
            var SOptrs = Optrs.Split(',').ToList();
            var SValues = new List<string>();
            if (!string.IsNullOrEmpty(Values))
            {
                SValues = Values.Split(',').ToList();
            }
            for (int i = 0; i < SFields.Count(); i++)
            {

                if (SFields[i].ToLower() == "parentboname")
                {
                    if (SValues[i] != "0")
                    {
                        XIDXI oXIAPI = new XIDXI();
                        var BODef = oXIAPI.Get_BODefinition(SValues[i], null);
                        //var ParentBOTableName = BODef.TableName;
                        var CBODef = oXIAPI.Get_BODefinition(SValues[2], null);
                        //var fk = CBODef.BOFields.Where(m => m.FKTableName == ParentBOTableName).Select(m => m.Name).FirstOrDefault();
                        //if (fk != null)
                        //{
                        //    condition = condition + fk + "=" + SValues[i + 1];
                        //    return condition;
                        //}
                    }
                    else
                    {
                        return condition;
                    }
                }
                else if ((SValues.Count() > 0 && SValues[i] != "" && SValues[i].Length > 0) || (SOptrs[i] == "is null" || SOptrs[i] == "is not null"))
                {
                    string scalarfield = string.Empty;
                    var NewSFields = SFields[i];
                    var scar = SFields[i].Split('.');
                    if (scar.Count() > 1)
                    {
                        SFields[i] = scar[1];
                        scalarfield = "@" + scar[0].Replace("[", "").Replace("]", "") + SFields[i] + "p";
                    }
                    else
                    {
                        SFields[i] = SFields[i];
                        scalarfield = "@" + SFields[i] + "p";
                    }
                    //Get Field Data Type
                    int DataTypeID = xibo.AttributeD(SFields[i]).TypeID;

                    dynamic value = null;
                    XIIXI oXI = new XIIXI();
                    bool bOverride = false;
                    DataTable oDataTable = new DataTable();
                    Guid valguid = Guid.Empty;
                    Guid.TryParse(SValues[i], out valguid);
                    if (valguid != Guid.Empty && SOptrs[i].ToLower() == "undefined")
                    {
                        var oRefBOI = oXI.BOI("Range Details", SValues[i]);
                        if (oRefBOI != null && oRefBOI.Attributes.Count > 0)
                        {
                            var Fromvalue = oRefBOI.Attributes["sFromValue"].sValue;
                            var Tovalue = oRefBOI.Attributes["sToValue"].sValue;
                            var sType = oRefBOI.Attributes["sDataType"].sValue;
                            if (!string.IsNullOrEmpty(Fromvalue) && (!string.IsNullOrEmpty(sType) && sType.ToLower() == "date"))
                            {
                                oDataTable = (DataTable)Connection.Execute_Query(Fromvalue);
                                Fromvalue = oDataTable.Rows[0].ItemArray[0].ToString();
                                SValues[i] = Fromvalue;
                            }
                            else { SValues[i] = Fromvalue; value = SValues[i]; }
                            if (!string.IsNullOrEmpty(Tovalue) && (!string.IsNullOrEmpty(sType) && sType.ToLower() == "date"))
                            {
                                oDataTable = (DataTable)Connection.Execute_Query(Tovalue);
                                Tovalue = oDataTable.Rows[0].ItemArray[0].ToString();
                                SValues[i] = Fromvalue + "_" + Tovalue;
                            }
                            else if (!string.IsNullOrEmpty(Fromvalue) && !string.IsNullOrEmpty(Tovalue))
                            { SValues[i] = Fromvalue + "_" + Tovalue; value = SValues[i]; }
                            SOptrs[i] = oRefBOI.Attributes["sOperator"].sValue;
                            bOverride = true;
                        }
                    }
                    if (DataTypeID == 150 && SValues[i].ToLower().StartsWith("t") && SFields[i].StartsWith("d") && bOverride == false)
                    {
                        if (SOptrs[i] == "between")
                        {
                            var Dates = SValues[i].Split('_').ToList();
                            var Date1 = Utility.GetDateResolvedValue(Dates[0], XIConstant.Date_Format); //"dd-MMM-yyyy"
                            var Date2 = Utility.GetDateResolvedValue(Dates[1], XIConstant.Date_Format);
                            SValues[i] = Date1 + "_" + Date2;
                        }
                        else
                        {
                            SValues[i] = Utility.GetDateResolvedValue(SValues[i], XIConstant.Date_Format);
                        }
                    }
                    if (SValues.Count() > 0 && SValues[i] != "" && SValues[i].Length > 0 /*&& bOverride==false*/ && (SValues[i].IndexOf("^") == 0 || SValues[i].IndexOf("^") == -1))
                    {
                        value = Utility.ConvertToExpected(SValues[i], DataTypeID);
                    }
                    if (SValues[i].IndexOf("^") > 0)
                    {
                        value = SValues[i].Replace('^', ',');
                    }

                    if (SOptrs[i] == "is null" || SOptrs[i] == "is not null")
                    {

                    }
                    else if (scar.Count() > 1)
                    {
                        condition = condition + NewSFields;
                    }
                    else
                    {
                        condition = condition + "[" + xibo.TableName + "]." + SFields[i];
                    }
                    if (SOptrs[i] == "starts with")
                    {
                        //condition = condition + " LIKE " + '+' + scalarfield + '+' + "'%' AND ";
                        condition = condition + " LIKE " + "cast(" + scalarfield + " as varchar(126)) + '%' AND ";
                        AddSqlParameters(scalarfield, value);
                    }
                    else if (SOptrs[i] == "not starts with")
                    {
                        //condition = condition + " NOT LIKE " + '+' + scalarfield + '+' + "'%' AND ";
                        condition = condition + " NOT LIKE " + "cast(" + scalarfield + " as varchar(126)) + '%' AND ";
                        AddSqlParameters(scalarfield, value);
                    }
                    else if (SOptrs[i] == "ends with")
                    {
                        //condition = condition + " LIKE '%'" + '+' + scalarfield + " AND ";
                        condition = condition + " LIKE '%' + " + "cast(" + scalarfield + " as varchar(126)) AND ";
                        AddSqlParameters(scalarfield, value);
                    }
                    else if (SOptrs[i] == "not ends with")
                    {
                        //condition = condition + " NOT LIKE '%'" + '+' + scalarfield + " AND ";
                        condition = condition + " NOT LIKE '%' + " + "cast(" + scalarfield + " as varchar(126)) AND ";
                        AddSqlParameters(scalarfield, value);
                    }
                    else if (SOptrs[i] == "contains")
                    {
                        //condition = condition + " LIKE '%'" + '+' + scalarfield + '+' + "'%' AND ";
                        condition = condition + " LIKE '%' + " + "cast(" + scalarfield + " as varchar(126)) + '%' AND ";
                        AddSqlParameters(scalarfield, value);
                    }
                    else if (SOptrs[i] == "between")
                    {
                        var Dates = SValues[i].Split('_').ToList();
                        dynamic dtfrom = Utility.ConvertToExpected(Dates[0], DataTypeID);
                        dynamic dtto = Utility.ConvertToExpected(Dates[1], DataTypeID);
                        var sScalarFrom = scalarfield + "From";
                        var sScalarTo = scalarfield + "To";
                        //condition = condition + " >= '" + Convert.ToDateTime(Dates[0]).ToString("yyyy/M/dd") + "' AND " + SFields[i] + " <= '" + Convert.ToDateTime(Dates[1]).ToString("yyyy/M/dd") + "' AND ";
                        if (scar.Count() > 1)
                        {
                            condition = condition + " >=" + sScalarFrom + " AND " + NewSFields + " <=" + sScalarTo + " AND ";
                        }
                        else
                        {
                            condition = condition + " >=" + sScalarFrom + " AND " + NewSFields + " <=" + sScalarTo + " AND ";
                            //condition = condition + " >= @dtfrom AND " + SFields[i] + " <= @dtto AND ";
                        }
                        //AddSqlParameters("@dtfrom", dtfrom);
                        //AddSqlParameters("@dtto", dtto);
                        AddSqlParameters(sScalarFrom, dtfrom);
                        AddSqlParameters(sScalarTo, dtto);
                    }
                    else if (SOptrs[i] == "is null")
                    {
                        condition = condition + "(" + NewSFields + " is null or " + NewSFields + " ='') AND ";
                    }
                    else if (SOptrs[i] == "is not null") { condition = condition + "(" + NewSFields + " is not null AND " + NewSFields + " <>'') AND "; }
                    else if (DataTypeID == 60)
                    {
                        if (SValues.Count() > 0 && SValues[i] != "" && SValues[i].Length > 0 && (SValues[i].IndexOf("^") == 0 || SValues[i].IndexOf("^") == -1))
                        {
                            value = Utility.ConvertToExpected(SValues[i], DataTypeID);
                        }
                        if (SValues[i].IndexOf("^") > 0 && SOptrs[i] == "in")
                        {
                            value = SValues[i].Replace('^', ',');
                            condition = condition + " " + SOptrs[i] + " (" + scalarfield + ") AND ";
                        }
                        else
                        {
                            condition = condition + " " + SOptrs[i] + " " + scalarfield + " AND ";
                        }
                        AddSqlParameters(scalarfield, value);
                    }
                    else
                    {
                        if (SFields[i].StartsWith("d"))
                        {
                            condition = condition + " " + SOptrs[i] + " " + scalarfield + " AND ";
                        }
                        else if (SValues[i].IndexOf("^") > 0 && SOptrs[i] == "in")
                        {
                            condition = condition + " " + SOptrs[i] + " (" + scalarfield + ") AND ";
                        }
                        else
                        {
                            condition = condition + " " + SOptrs[i] + " cast(" + scalarfield + " as varchar(126)) AND ";
                        }
                        //condition = condition + " " + SOptrs[i] + " " + scalarfield + " AND ";                        
                        AddSqlParameters(scalarfield, value);
                    }
                }
            }
            if (condition.Length > 0)
            {
                condition = condition.Substring(0, condition.Length - 5);
            }
            return condition;
        }
        public string AddSearchParameters(string Query, string SearchText, string sJoins = "")
        {
            //Declaration
            string GroupBY = "", Condition = "";
            int pos, Qlength;
            //Query Modification 
            int iLastIndex = CheckSubQuery(Query, " where ");
            //Query = Query.ToLower();


            if (Query.IndexOf(" group by", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                pos = Query.IndexOf("group by", StringComparison.OrdinalIgnoreCase);
                Qlength = Query.Length;
                GroupBY = Query.Substring(pos);
                GroupBY = GroupBY.Insert(0, " ");
                Query = Query.Substring(0, pos - 1);
            }
            else if (Query.IndexOf(" order by", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                pos = Query.IndexOf("order by", StringComparison.OrdinalIgnoreCase);
                Qlength = Query.Length;
                GroupBY = Query.Substring(pos);
                GroupBY = GroupBY.Insert(0, " ");
                Query = Query.Substring(0, pos - 1);
            }
            if (SearchText != null && SearchText.Length > 0)
            {
                if (!bIsMultiBO)
                {
                    Condition = Condition + SearchText;
                }
                else
                {
                    var sCon = SearchText.Split('=');
                    if (sCon.Count() == 2)
                    {
                        if (sCon[0].StartsWith("[" + FromBos + "]"))
                        {
                            SearchText = sCon[0] + "=" + sCon[1];
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(FromBos))
                            {
                                if (sCon[0].IndexOf('.')>0) {
                                    SearchText =  sCon[0] + "=" + sCon[1];
                                }
                                else
                                    SearchText = "[" + FromBos + "]." + sCon[0] + "=" + sCon[1];
                            }
                            else
                            {
                                SearchText = sCon[0] + "=" + sCon[1];
                            }
                        }

                    }
                    Condition = Condition + SearchText;
                }
            }
            //if (oInfo.sRoleName == "WebUsers" && !bIsXILoad && !bIsMultiBO)
            //{
            //CUserInfo oInfo = new CUserInfo();
            //oInfo = oInfo.GetUserInfo();
            //    if (!string.IsNullOrEmpty(Condition))
            //    {
            //        Condition += " and ";
            //    }
            //    Condition += "FKiUserID = " + oInfo.iUserID;
            //}
            if (Query.IndexOf(" where ", iLastIndex, StringComparison.OrdinalIgnoreCase) < 0 && !string.IsNullOrEmpty(Condition))
            {
                //var sSelect = string.Empty;
                //var oHeads = TableColumns.Where(f => f.ToLower() != "hiddendata").Select(f => sBOName + "." + f).ToList();
                //sSelect = "Select " + string.Join(",", oHeads);
                //Query = sSelect + " from " + BOD.TableName;
                if (!string.IsNullOrEmpty(sJoins))
                {
                    Query = Query + sJoins;
                }
                Query = Query + " where " + Condition;
                Query = string.Concat(Query, GroupBY);
            }
            else if (!string.IsNullOrEmpty(Condition)) // WHERE ==0
            {
                Query = Query + " and ( " + Condition + " )";
                Query = string.Concat(Query, GroupBY);
            }
            //string fQuery = Query;
            //if (GroupBY.Contains(" order by "))
            //{
            //    fQuery = fQuery + " " + GroupBY;
            //}
            return Query;
        }

        public string AddSelectPart(string Query, string Select, string sType = "")
        {
            //Declaration
            string WhereClause = "", GroupBY = "", Condition = "";
            int pos, Qlength;
            //Query Modification 
            int iLastIndex = CheckSubQuery(Query.ToLower(), " where ");
            int iFromIndex = Query.IndexOf(" from ", iLastIndex, StringComparison.CurrentCultureIgnoreCase);
            string sSubQuery = Query.Substring(0, iFromIndex);
            //Query = Query.ToLower();

            if (Query.IndexOf(" from ", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                pos = Query.IndexOf(" from ", StringComparison.OrdinalIgnoreCase);
                Qlength = Query.Length;
                WhereClause = Query.Substring(iFromIndex);
                WhereClause = WhereClause.Insert(0, " ");
                //Query = Query.Substring(0, pos);
            }
            else if (Query.IndexOf(" group by", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                pos = Query.IndexOf("group by", StringComparison.OrdinalIgnoreCase);
                Qlength = Query.Length;
                GroupBY = Query.Substring(pos);
                GroupBY = GroupBY.Insert(0, " ");
                //Query = Query.Substring(0, pos - 1);
            }
            else if (Query.IndexOf(" order by", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                pos = Query.IndexOf("order by", StringComparison.OrdinalIgnoreCase);
                Qlength = Query.Length;
                GroupBY = Query.Substring(pos);
                GroupBY = GroupBY.Insert(0, " ");
                //Query = Query.Substring(0, pos - 1);
            }
            if (!string.IsNullOrEmpty(Select) && string.IsNullOrEmpty(sType))
            {
                Query = "select " + Select;
            }
            else if (!string.IsNullOrEmpty(Select))
            {
                Query = sSubQuery + ", " + Select;
            }
            if (!string.IsNullOrEmpty(WhereClause))
            {
                Query = string.Concat(Query, WhereClause);
            }
            else if (!string.IsNullOrEmpty(GroupBY)) // WHERE ==0
            {
                Query = string.Concat(Query, GroupBY);
            }
            return Query;
        }

        public int CheckSubQuery(string sQuery, string findString, int iLastIndex = 0)
        {
            int inIndex = sQuery.IndexOf(" in (select", iLastIndex, StringComparison.OrdinalIgnoreCase);
            int notinIndex = sQuery.IndexOf(" not in (select", iLastIndex, StringComparison.OrdinalIgnoreCase);
            if (inIndex > 0 || notinIndex > 0)
            {
                string[] splitList = { "select" };
                var SelectList = sQuery.Split(splitList, StringSplitOptions.RemoveEmptyEntries);
                int index = 0;
                foreach (var item in SelectList)
                {
                    if (item.Contains(findString))
                    {
                        int count = Regex.Matches(item, findString).Count;
                        if (count >= 1) // when will this be > 1? (TODO)
                        {
                            index = item.LastIndexOf(findString);
                            return iLastIndex += index;
                        }
                    }
                    iLastIndex += splitList[0].Length + item.Length;
                }
            }
            else
            {
                int iSIndex = sQuery.IndexOf("(select", iLastIndex, StringComparison.OrdinalIgnoreCase);
                if (iSIndex > 0)
                {
                    int iWhereIndex = sQuery.IndexOf(findString, iLastIndex, StringComparison.OrdinalIgnoreCase);
                    if (iWhereIndex > 0)
                    {
                        iLastIndex = sQuery.IndexOf(')', iWhereIndex);
                        iLastIndex = CheckSubQuery(sQuery, findString, iLastIndex);
                    }
                }
            }

            return iLastIndex;
        }
        public string ReplaceQueryString(string NVPairs)
        {
            var sOneQuery = Regex.Replace(NVPairs, " ", "").Replace("/", "").Replace("[", "").Replace("]", "").Replace("\"", "").Split(',').ToList();
            Dictionary<string, string> QueryInfo = new Dictionary<string, string>();
            foreach (var item in sOneQuery)
            {
                var QueryData = item.Split('-').ToArray();
                QueryInfo[QueryData[0]] = QueryData[1];
            }
            string sFinalString = CheckString(Query);
            while (!string.IsNullOrEmpty(sFinalString))
            {
                foreach (var item in QueryInfo)
                {
                    var finalquery = sFinalString.Replace("{XIP|", "").Replace("}", "");
                    if (item.Key.ToLower() == finalquery.ToLower())
                    {
                        if (item.Value.ToLower() == "null" || item.Value.ToLower() == "isnull" || item.Value.ToLower() == "isnotnull")
                        {
                            var sISstring = CheckStringISNULL(Query, sFinalString);
                            var sNullValue = string.Empty;
                            if (item.Value.ToLower() == "isnull")
                            {
                                sNullValue = " IS NULL";
                            }
                            else if (item.Value.ToLower() == "isnotnull")
                            {
                                sNullValue = " IS NOT NULL";
                            }
                            Query = sISstring.Replace(sFinalString, sNullValue);
                            break;
                        }
                        else
                        {
                            if (item.Value == "")
                            {
                                Dictionary<string, object> oClickParameters = new Dictionary<string, object>();
                                oClickParameters["FKi1ClickID"] = ID;
                                XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
                                var oParamList = Connection.Select<XID1ClickParameter>("XI1ClickParameter_T", oClickParameters).ToList();
                                var sDefValue = string.Empty;
                                foreach (var OneParam in oParamList)
                                {
                                    var sParamName = OneParam.sName;
                                    if (sParamName == finalquery)
                                    {
                                        if (OneParam.sDefault == "")
                                        {
                                            sDefValue = OneParam.sValue;
                                        }
                                        else
                                        {
                                            sDefValue = OneParam.sDefault;
                                        }
                                    }
                                }
                                Query = Query.Replace(sFinalString, sDefValue);
                            }
                            else
                            {
                                Query = Query.Replace(sFinalString, "'" + item.Value + "'");
                                break;
                            }
                        }
                    }
                }
                sFinalString = CheckString(Query);
            }
            return Query;
        }
        public string CheckString(string Query)
        {
            var sFinalString = string.Empty;
            if (Query.Contains("}"))
            {
                int iCollectionIndex = Query.IndexOf("}");
                int iStartPosi = Query.LastIndexOf("{", iCollectionIndex);
                int iStringLength = "}".Length;
                sFinalString = Query.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength).Replace(" ", "");
            }
            return sFinalString;
        }
        public string CheckStringISNULL(string Query, string FindString)
        {
            var sFinalString = string.Empty;
            if (Query.Contains(FindString))
            {
                int iCollectionIndex = Query.IndexOf(FindString);
                int iStartPosi = Query.LastIndexOf("=", iCollectionIndex);
                sFinalString = Query.Remove(iStartPosi, 1);
            }
            return sFinalString;
        }

        public void ResolveFKColumns(XIDBO oXBOD, string sCoredatabase = "")
        {
            int[] CurrencyTypes = { 90, 40 };
            string[] Mathematictypes = { "+", "-", "*", "/", "%" };
            string Heading = string.Empty;
            this.BOD = oXBOD;
            Get_1ClickHeadings();
            if (!string.IsNullOrEmpty(sBOName) && !nXIDBO.ContainsKey(sBOName))
            {
                nXIDBO[sBOName] = oXBOD;
            }
            string sResGroup = string.IsNullOrEmpty(sLabelOverrideGroup) ? "fklabel" : sLabelOverrideGroup.ToLower();
            XIDXI oXID = new XIDXI();
            var FromIndex = Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
            var SelectQuery = Query.Substring(0, FromIndex);
            SelectQuery = SelectQuery.TrimEnd();
            var ReplaceQry = Query.Substring(0, FromIndex);
            if (Headings != null && Headings.Count() > 0)
            {
                int K = 0;
                var Columns = TableColumns.ToList().ConvertAll(d => d.ToLower()).ToList();
                //Headings = Headings.ToList().ConvertAll(d => d.ToLower()).ToList();
                //var oAttrs = oXBOD.Attributes.Values.Where(m => Columns.Any(n => n == m.Name.ToLower())).ToList(); //.Contains(m.Name.ToLower())).ToList();
                foreach (var items in TableColumns)
                {
                    if (items.Contains("CONCAT"))
                    {
                        var ConItem = string.Concat(items);
                        var SplitConcat = ConItem.Trim().Replace("CONCAT([", "").Replace("]", "").Replace("[", "").Replace(",'/'", "").Replace(")", "").Replace("]", "");
                        var NewItem = SplitConcat.Split(',').ToList();
                        var AS = "'" + Headings[K] + "'";
                        var sConSelect = "CONCAT(" + string.Join(",' ',", NewItem) + ") AS  " + AS + ", ";
                        Heading = Heading + sConSelect;
                    }
                    else
                    {
                        string sFKBoName = string.Empty; string sCol = items; string sBOCol = string.Empty;
                        var oXIDBO = oXBOD;
                        if (items.Contains('.'))
                        {
                            sFKBoName = items.Substring(items.IndexOf("[") + 1, items.IndexOf("]") - items.IndexOf("[") - 1);
                            var Cols = items.Split('.').ToList();
                            var Column = string.Empty;
                            if (Cols.Count() > 1)
                            {
                                sBOCol = items;
                                Column = items.Split('.')[1];
                                bool containsSymbol = Mathematictypes.Any(symbol => Column.Contains(symbol));
                                if (containsSymbol)
                                {
                                    sCol = Column.Split('*')[0];
                                }
                                else
                                {
                                    sCol = Column;
                                }
                            }
                            else
                            {
                                Column = items;
                            }

                            if (Column.IndexOf("[") >= 0 && Column.IndexOf("]") > 1)
                            {
                                sCol = Column.Substring(Column.IndexOf("[") + 1, Column.IndexOf("]") - Column.IndexOf("[") - 1);
                            }
                            //sFKBoName = items.Split('.')[0].Trim().Replace("[", "").Replace("]", "");
                            // sCol = items.Split('.')[1].Replace("[", "").Replace("]", "");

                            if (sFKBoName.ToLower() != oXIDBO.TableName.ToLower())
                            {
                                if (!nXIDBO.ContainsKey(sFKBoName))
                                {
                                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sFKBoName, null);
                                    oXIDBO = oBOD;
                                    nXIDBO[sFKBoName] = oXIDBO;
                                }
                                else
                                {
                                    oXIDBO = nXIDBO[sFKBoName];
                                }
                            }
                        }
                        if (sCol.ToLower() == "hiddendata")
                        {
                            //if (items != "HiddenData" && items != "Select" && items != "Actions" && items != "Color")
                            //{
                            //    Heading = Heading + (items.Contains('.') ? items + ", " : "[" + oXIDBO.TableName + "]." + items + ", ");
                            //}
                        }
                        else
                        {
                            var oAttrD = oXIDBO.Attributes.Values.Where(x => x.Name.ToLower() == sCol.ToLower()).FirstOrDefault();
                            //var oAttrD = oXIDBO.Attributes[sCol.ToLower()].FirstOrDefault();
                            //var oAttrD = oAttrs.Where(m => m.Name.ToLower() == sCol.ToLower()).FirstOrDefault();
                            if (oAttrD != null)
                            {
                                var oFormat = Formats.Where(m => m.FKiAttributeIDXIGUID == oAttrD.XIGUID).ToList();
                                if (oFormat != null && oFormat.Count() > 0)
                                {
                                    bool bPrefix = false; string sPrefix = string.Empty;
                                    bool bSuffix = false; string sSuffix = string.Empty;
                                    foreach (var Frmt in oFormat)
                                    {
                                        XIDFormat oFrmt = (XIDFormat)oCache.GetObjectFromCache(XIConstant.CacheXIFormat, null, Frmt.FKiFormatIDXIGUID.ToString());
                                        if (!string.IsNullOrEmpty(oFrmt.sCode))
                                        {
                                            if (oAttrD.TypeID == 150 && oFrmt.iType == 150)
                                            {
                                                if (!string.IsNullOrEmpty(oFrmt.sCode) && oFrmt.sCode.ToLower() == "date")
                                                {
                                                    Heading = Heading + "FORMAT(" + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ", '" + oFrmt.sFormat + "')" + " as '" + oAttrD.LabelName + "', ";
                                                }
                                                else if (!string.IsNullOrEmpty(oFrmt.sCode) && oFrmt.sCode.ToLower() == "time ago")
                                                {
                                                    Heading = Heading + "FORMAT(" + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ", '" + oFrmt.sFormat + "')" + " as '" + oAttrD.LabelName + "', ";
                                                }
                                            }
                                            else if (oFrmt.sCode.ToLower() == "Formatted Number".ToLower())
                                            {
                                                Heading = Heading + "FORMAT(" + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ", '#,0.00')" + " as '" + oAttrD.LabelName + "', ";
                                            }
                                            else if (oFrmt.sCode.ToLower() == "Trim character limit".ToLower())
                                            {
                                                var iNoOfChars = oFrmt.sFormat;
                                                Heading = Heading + "SUBSTRING(" + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ", 1, " + iNoOfChars + ")" + " as '" + oAttrD.LabelName + "', ";
                                            }
                                            else if (!string.IsNullOrEmpty(oFrmt.sCode))
                                            {
                                                if (oFrmt.sCode.ToLower() == "Prefix".ToLower())
                                                {
                                                    bPrefix = true;
                                                    sPrefix = oFrmt.sFormat;
                                                }
                                                else if (!string.IsNullOrEmpty(oFrmt.sCode) && oFrmt.sCode.ToLower() == "Suffix".ToLower())
                                                {
                                                    bSuffix = true;
                                                    sSuffix = oFrmt.sFormat;
                                                }
                                                else if (!string.IsNullOrEmpty(oFrmt.sCode) && oFrmt.sCode.ToLower() != "CSS Class".ToLower())
                                                {

                                                }
                                            }
                                        }
                                    }
                                    if (bPrefix && bSuffix)
                                    {
                                        Heading = Heading + "CONCAT('" + sPrefix + " ' ," + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ", ' " + sSuffix + "')" + " as '" + oAttrD.LabelName + "', ";
                                    }
                                    else if (bPrefix)
                                    {
                                        Heading = Heading + "CONCAT('" + sPrefix + " ' ," + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ")" + " as '" + oAttrD.LabelName + "', ";
                                    }
                                    else if (bSuffix)
                                    {
                                        Heading = Heading + "CONCAT(" + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ", ' " + sSuffix + "')" + " as '" + oAttrD.LabelName + "', ";
                                    }

                                }
                                else if (!string.IsNullOrEmpty(oAttrD.Format))
                                {
                                    var Char = oAttrD.Name.Select(c => char.IsUpper(c)).ToList();
                                    var Position = Char.IndexOf(true);
                                    if (Position == 1)
                                    {
                                        char FirstLetter = sCol[0];
                                        if (FirstLetter == 'd' || oAttrD.TypeID == 150 || oAttrD.TypeID == 110)
                                        {
                                            if (!string.IsNullOrEmpty(oAttrD.sDBDateFormat))
                                            {
                                                Heading = Heading + "FORMAT(" + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ", '" + oAttrD.sDBDateFormat + "')" + " as '" + oAttrD.LabelName + "', ";
                                            }
                                            else
                                            {
                                                Heading = Heading + "FORMAT(" + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ", '" + oAttrD.Format + "')" + " as '" + oAttrD.LabelName + "', ";
                                            }
                                        }
                                        //else if ((FirstLetter == 'r' || FirstLetter == 'f') && oAttrD.TypeID == 90)
                                        else if (CurrencyTypes.Contains(oAttrD.TypeID))
                                        {
                                            //FORMAT(XILinkID, 'C', 'en-gb')
                                            Heading = Heading + "FORMAT(" + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ", 'C', '" + oAttrD.Format + "')" + " as '" + oAttrD.LabelName + "', ";
                                        }
                                        else
                                        {
                                            Heading = Heading + "CONCAT(" + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ", '" + oAttrD.Format + "')" + " as '" + oAttrD.LabelName + "', ";
                                        }
                                    }
                                    else if (Position == 2)
                                    {
                                        var Prefix = items.Substring(0, 2);
                                        if (Prefix == "dt" || oAttrD.TypeID == 150)
                                        {
                                            Heading = Heading + "FORMAT(" + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ", '" + oAttrD.Format + "')" + " as '" + oAttrD.LabelName + "', ";
                                        }
                                    }
                                    else if (oAttrD.TypeID == 150)
                                    {
                                        Heading = Heading + "FORMAT(" + (items.Contains('.') ? items : "[" + oXIDBO.TableName + "]." + items) + ", '" + oAttrD.Format + "')" + " as '" + oAttrD.LabelName + "', ";
                                    }
                                }
                                else if (!string.IsNullOrEmpty(oAttrD.sFKBOName))
                                {
                                    //string sFKBOName = oAttrD.sFKBOName; //Connection.Select<string>("XIBO_T_N", Params, "Name").FirstOrDefault();
                                    if (items.StartsWith("s"))
                                    {
                                        Heading = Heading + (items.Contains('.') ? items + ", " : "[" + oXIDBO.TableName + "]." + items + ", ");
                                    }
                                    else if (!string.IsNullOrEmpty(oAttrD.sFKBOName))
                                    {
                                        var oFKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oAttrD.sFKBOName);
                                        var GroupD = new XIDGroup();
                                        sResGroup = "fklabel";
                                        if (oFKBOD.Name.ToLower() == "refConviction_T".ToLower())
                                        {
                                            sResGroup = "resolvegroup";
                                            GroupD = oFKBOD.GroupD(sResGroup);
                                        }
                                        else
                                        {
                                            GroupD = oFKBOD.GroupD(sResGroup);
                                            if(GroupD != null && GroupD.XIGUID != null && GroupD.XIGUID != Guid.Empty)
                                            {

                                            }
                                            else
                                            {
                                                sResGroup = "label";
                                                GroupD = oFKBOD.GroupD("label");
                                            }
                                        }                                        
                                        if (GroupD != null && GroupD.XIGUID != null && GroupD.XIGUID != Guid.Empty)
                                        {
                                            XIDBAPI Myconntection = new XIDBAPI(oXID.GetBODataSource(oFKBOD.iDataSourceXIGUID.ToString(), oFKBOD.FKiApplicationID));
                                            if (oFKBOD != null && oFKBOD.Groups.Any(group => group.Key.ToLower() == sResGroup)) //&& (oFKBOD.sSize == "20" || oFKBOD.sSize == "30")
                                            {
                                                oFKBOD.GroupD(sResGroup).sBOName = oFKBOD.TableName;
                                                string FinalString = oFKBOD.GroupD(sResGroup).ConcatanateGroupFields(" ");//concatenate the string with join String 
                                                if (!string.IsNullOrEmpty(FinalString))
                                                {
                                                    var sWhrClause = items;
                                                    if (!items.Contains('.'))
                                                    {
                                                        if (oFKBOD.bUID && !items.ToLower().EndsWith("xiguid"))
                                                            if (!string.IsNullOrEmpty(sReferenceID) && sReferenceID.ToLower() == "xiguid")
                                                                sWhrClause = "[" + oXBOD.TableName + "]." + items + "xiguid";
                                                            else
                                                                sWhrClause = "[" + oXBOD.TableName + "]." + items;
                                                        else
                                                            sWhrClause = "[" + oXBOD.TableName + "]." + items;
                                                    }
                                                    if (oFKBOD.iDataSource == oXIDBO.iDataSource)
                                                    {
                                                        var subQuery = "";
                                                        if (oFKBOD.bUID || (!string.IsNullOrEmpty(sReferenceID) && sReferenceID.ToLower() == "xiguid") || items.ToLower().EndsWith("xiguid"))
                                                        {
                                                            subQuery = "select " + FinalString + " from " + oFKBOD.TableName + " where [" + oFKBOD.TableName + "].XIGUID=" + sWhrClause;
                                                        }
                                                        else
                                                            subQuery = "select " + FinalString + " from " + oFKBOD.TableName + " where [" + oFKBOD.TableName + "]." + oFKBOD.sPrimaryKey + "=" + sWhrClause;

                                                        SelectQuery = SelectQuery.Replace(items, "(" + subQuery + ") as '" + oAttrD.LabelName + "'");
                                                        Heading = Heading + "(" + subQuery + ") as '" + oAttrD.LabelName + "'" + ", ";
                                                    }
                                                    else
                                                    {
                                                        //var DataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, oFKBOD.iDataSource.ToString());
                                                        var DataSource = oXID.GetBODataSource(oFKBOD.iDataSourceXIGUID.ToString(), oFKBOD.FKiApplicationID);
                                                        SqlConnection connection = new SqlConnection(DataSource);
                                                        var sFKDataBase = connection.Database;
                                                        if (oFKBOD.Name.ToLower() == "XIAPPUsers".ToLower())
                                                        {
                                                            if (!string.IsNullOrEmpty(sCoredatabase))
                                                            {
                                                                sFKDataBase = sCoredatabase;
                                                            }
                                                        }
                                                        var subQuery = string.Empty;
                                                        if (items.ToLower().EndsWith("xiguid"))
                                                        {
                                                            subQuery = "select " + FinalString + " from " + sFKDataBase + ".dbo." + oFKBOD.TableName + " where " + sFKDataBase + ".dbo.[" + oFKBOD.TableName + "].xiguid=" + sWhrClause;
                                                        }
                                                        else
                                                        {
                                                            subQuery = "select " + FinalString + " from " + sFKDataBase + ".dbo." + oFKBOD.TableName + " where " + sFKDataBase + ".dbo.[" + oFKBOD.TableName + "]." + oFKBOD.sPrimaryKey + "=" + sWhrClause;
                                                        }
                                                        SelectQuery = SelectQuery.Replace(items, "(" + subQuery + ") as '" + oAttrD.LabelName + "'");
                                                        Heading = Heading + "(" + subQuery + ") as '" + oAttrD.LabelName + "'" + ", ";
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Heading = Heading + (items.Contains('.') ? items + ", " : oXIDBO.TableName + "." + items + ", ");
                                        }
                                    }
                                }
                                else
                                {
                                    Heading = Heading + (items.Contains('.') ? items + ", " : "[" + oXIDBO.TableName + "]." + items + ", ");
                                }
                            }
                            else if (oAttrD == null || sCol.ToLower().StartsWith("zX".ToLower()))
                            {
                                sBOCol = items;
                                if (sCol.ToLower().StartsWith("zX".ToLower()))
                                {
                                    if ((sCol.ToLower() == XIConstant.Key_XICrtdWhn.ToLower() || sCol.ToLower() == XIConstant.Key_XIUpdtdWhn.ToLower()))
                                    {
                                        if (sBOCol != null)
                                        {
                                            if (sBOName.ToLower() == "lead_t" || sBOName.ToLower() == "wm file" || sBOName.ToLower() == "call_t" || sBOName.ToLower() == "audit_t")
                                            {
                                                //Heading = Heading + sBOCol + ", ";
                                                if (sBOCol.Contains('.'))
                                                {
                                                    Heading = Heading + "FORMAT(" + (sBOCol) + ", '" + XIConstant.DateTimeFull_Format + "')" + ", ";
                                                }
                                                else
                                                {
                                                    Heading = Heading + "FORMAT(" + "[" + oXIDBO.TableName + "]." + (sBOCol) + ", '" + XIConstant.DateTimeFull_Format + "')" + ", ";
                                                }

                                            }
                                            else
                                            {
                                                if (sBOCol.Contains('.'))
                                                {
                                                    Heading = Heading + "FORMAT(" + (sBOCol) + ", '" + XIConstant.Date_Format + "')" + ", ";
                                                }
                                                else
                                                {
                                                    Heading = Heading + "FORMAT(" + "[" + oXIDBO.TableName + "]." + (sBOCol) + ", '" + XIConstant.Date_Format + "')" + ", ";
                                                }

                                            }
                                        }
                                    }
                                    else if (sCol.ToLower() == XIConstant.Key_XICrtdBy.ToLower() || sCol.ToLower() == XIConstant.Key_XIUpdtdBy.ToLower())
                                    {
                                        if (sBOCol != null)
                                        {
                                            var index = TableColumns.IndexOf(sCol);
                                            //var He = Headings[index];
                                            Heading = Heading + (sBOCol.Contains('.') ? sBOCol + ", " : "[" + oXIDBO.TableName + "]." + sBOCol + ", ");

                                        }
                                    }
                                }
                                else
                                {
                                    if (items != "HiddenData" && items != "Select" && items != "Actions" && items != "Color")
                                    {
                                        Heading = Heading + (items.Contains('.') ? items + ", " : "[" + oXIDBO.TableName + "]." + items + ", ");
                                    }
                                }
                            }
                        }
                    }
                    //if (this.BOD.Groups.Any(m => m.Key == items.ToLower()))
                    //{
                    //    var BOGroupName = this.BOD.Groups.Where(m => m.Key == items.ToLower()).Select(m => m.Value.GroupName).FirstOrDefault();
                    //    var BOGroup = this.BOD.Groups.Where(m => m.Key == items.ToLower()).Select(m => m.Value.BOSqlFieldNames).FirstOrDefault();
                    //    Heading = Heading + BOGroup + " AS " + "'" + BOGroupName + "'" + ", ";
                    //}
                    K++;
                }
                if (!string.IsNullOrEmpty(Heading))
                {
                    Heading = "Select " + Heading.Substring(0, Heading.Length - 2);
                    Query = Query.Replace(ReplaceQry, Heading);
                }
            }
        }

        //private void ResolveFKColumns(XIDBO oXBOD)
        //{
        //    string sResGroup = "label";
        //    XIDXI oXID = new XIDXI();
        //    if (!string.IsNullOrEmpty(Query) && oXBOD != null)
        //    {
        //        var FromIndex = Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
        //        var SelectQuery = Query.Substring(0, FromIndex);
        //        SelectQuery = SelectQuery.TrimEnd();
        //        var ReplaceQry = Query.Substring(0, FromIndex); 
        //        var oFields = Regex.Replace(SelectQuery, "select ", "", RegexOptions.IgnoreCase).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
        //        //var oFields = Query.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
        //        var FKFields = oFields.Where(m => m.ToLower().StartsWith("fk") || m.ToLower().StartsWith("enum") || m.ToLower().StartsWith("ref")).ToList();
        //        FKFields.ForEach(m =>
        //        {
        //            if (oXBOD.Attributes.ContainsKey(m.ToLower()))
        //            {
        //                var oAttrD = oXBOD.Attributes[m.ToLower()];
        //                if (oAttrD != null)
        //                {
        //                    if (!string.IsNullOrEmpty(oAttrD.FKTableName))
        //                    {
        //                        string sBOName = oAttrD.sFKBOName; //Connection.Select<string>("XIBO_T_N", Params, "Name").FirstOrDefault();
        //                        if (!string.IsNullOrEmpty(sBOName))
        //                        {
        //                            var oFKBOD = (XIDBO)oCahce.GetObjectFromCache(XIConstant.CacheBO, sBOName);
        //                            if (oFKBOD.Name.ToLower() == "refConviction_T".ToLower())
        //                            {
        //                                sResGroup = "resolvegroup";
        //                            }
        //                            var GroupD = new XIDGroup();
        //                            if (oFKBOD.Groups.TryGetValue(sResGroup, out GroupD))
        //                            {
        //                                if (oFKBOD.iDataSource != 10)
        //                                {
        //                                    XIDBAPI Myconntection = new XIDBAPI(oXID.GetBODataSource(oFKBOD.iDataSource));
        //                                    if (oFKBOD != null && oFKBOD.Groups.Any(group => group.Key.ToLower() == sResGroup)) //&& (oFKBOD.sSize == "20" || oFKBOD.sSize == "30")
        //                                    {
        //                                        string FinalString = oFKBOD.GroupD(sResGroup).ConcatanateGroupFields(" ");//concatenate the string with join String 
        //                                        if (!string.IsNullOrEmpty(FinalString))
        //                                        {
        //                                            var subQuery = "select " + FinalString + " from " + oFKBOD.TableName + " where " + oFKBOD.sPrimaryKey + "=" + m;
        //                                            SelectQuery = SelectQuery.Replace(m, "(" + subQuery + ") as '" + oAttrD.LabelName + "'");
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        });
        //        if (!string.IsNullOrEmpty(SelectQuery))
        //        {
        //            Query = Query.Replace(ReplaceQry, SelectQuery);
        //        }
        //    }
        //}

        public void Get_1ClickHeadings()
        {
            var SelectQuery = string.Empty;
            XID1Click o1Click = this;
            if (!string.IsNullOrEmpty(sBOName) && !nXIDBO.ContainsKey(sBOName))
            {
                nXIDBO[sBOName] = this.BOD;
            }
            List<string> Heads = new List<string>();
            if (!string.IsNullOrEmpty(SelectFields))
            {
                Heads = SelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else
            {
                var FromIndex = Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                SelectQuery = Query.Substring(0, FromIndex);
                SelectQuery = SelectQuery.TrimEnd();
                Heads = Regex.Replace(SelectQuery, "select ", "", RegexOptions.IgnoreCase).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            TableColumns = new List<string>();
            Headings = new List<string>();
            //OptionListCols = new List<string>();
            OptionListCols = new Dictionary<string, XIDBO>();
            FKColumns = new List<string>();
            FKAttributes = new List<string>();
            CssAttributes = new List<string>();
            foreach (var items in Heads)
            {
                var regx = new Regex("{.*?}");
                var matchs = regx.Matches(items);
                if (items == "*")
                {
                    string sCol = "List";
                    List<string> oGrpHeadS = new List<string>();
                    var oGrpD = this.BOD.Groups.Values.Where(m => m.GroupName.ToLower() == sCol.ToLower()).FirstOrDefault();
                    if (oGrpD != null && oGrpD.IsMultiColumnGroup)
                    {
                        oGrpHeadS.AddRange(oGrpD.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList());
                        if (oGrpHeadS != null && oGrpHeadS.Count() > 0)
                        {
                            foreach (var head in oGrpHeadS)
                            {
                                var oAttrD = this.BOD.Attributes[head.ToLower()];
                                var sLabel = oAttrD.LabelName;
                                TableColumns.Add(oAttrD.Name);
                                Headings.Add(sLabel);
                                if (!string.IsNullOrEmpty(oAttrD.sFKBOName))
                                {
                                    FKColumns.Add(oAttrD.Name + "^" + oAttrD.sFKBOName);
                                    FKAttributes.Add(oAttrD.LabelName.ToLower());
                                }
                                if (oAttrD.IsOptionList)
                                {
                                    // OptionListCols.Add(oAttrD.Name.ToLower());
                                    OptionListCols.Add(oAttrD.Name.ToLower(), this.BOD);
                                }
                            }
                        }
                    }
                }
                else if (items.IndexOf(" as ", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var Head = Regex.Split(items, " as ", RegexOptions.IgnoreCase).ToList();
                    if (this.BOD.Attributes.ContainsKey(Head[0].ToLower().Trim()))
                    {
                        var oAttrD = this.BOD.Attributes[Head[0].ToLower().Trim()];
                        if (oAttrD != null)
                        {
                            TableColumns.Add(oAttrD.Name);
                            if (oAttrD.IsOptionList)
                            {
                                //OptionListCols.Add(oAttrD.Name.ToLower());
                                OptionListCols.Add(oAttrD.Name.ToLower(), this.BOD);
                            }
                            if (!string.IsNullOrEmpty(oAttrD.sFKBOName))
                            {
                                FKColumns.Add(oAttrD.Name + "^" + oAttrD.sFKBOName);
                                FKAttributes.Add(oAttrD.LabelName.ToLower());
                            }
                        }
                        else
                        {
                            TableColumns.Add(Head[0].ToString());
                        }
                    }
                    else if (Head[0].ToLower().Trim().Contains("case"))
                    {
                        var Case = Regex.Split(Head[0], "case ", RegexOptions.IgnoreCase).ToList();
                        //var CaseHead = Case[1].Split(' ')[0];
                        Head[0] = Case[1].Split(' ')[0];
                        XIDAttribute oAttrD = null;
                        if (this.BOD.Attributes.ContainsKey(Head[0].ToLower().Trim()))
                        {
                            oAttrD = this.BOD.Attributes[Head[0].ToLower().Trim()];
                        }
                        if (oAttrD != null)
                        {
                            TableColumns.Add(oAttrD.Name);
                            if (oAttrD.IsOptionList)
                            {
                                //OptionListCols.Add(oAttrD.Name.ToLower());
                                OptionListCols.Add(oAttrD.Name.ToLower(), this.BOD);
                            }
                            if (!string.IsNullOrEmpty(oAttrD.sFKBOName))
                            {
                                FKColumns.Add(oAttrD.Name + "^" + oAttrD.sFKBOName);
                                FKAttributes.Add(oAttrD.LabelName.ToLower());
                            }
                            Head[1] = oAttrD.LabelName;
                        }
                        else
                        {
                            TableColumns.Add(Head[0].ToString());
                        }
                    }
                    else
                    {
                        if (Head[0].Contains(','))
                        {
                            var Splits = Head[0].Split(',').ToList();
                            var resolveHead = Splits[0].Replace("FORMAT(", "");
                            TableColumns.Add(resolveHead);
                        }
                        else
                        {
                            TableColumns.Add(Head[0].ToString());
                        }
                    }

                    if (!string.IsNullOrEmpty(Head[1].ToString()))
                    {
                        if (Head[1].ToString().IndexOf('\'') == 0)
                        {
                            Headings.Add(Head[1].ToString().Trim('\''));
                        }
                        else
                        {
                            Headings.Add(Head[1].ToString());
                        }
                    }
                }
                else if (items.Contains("{"))
                {
                    var sCol = string.Empty;
                    if (items.Contains('.'))
                    {
                        sCol = items.Split('.')[1].Replace("{", "").Replace("}", "");
                    }
                    else
                    {
                        sCol = items.Replace("{", "").Replace("}", "");
                    }
                    List<string> oGrpHeadS = new List<string>();
                    var oGrpD = this.BOD.Groups.Values.Where(m => m.GroupName.ToLower() == sCol.ToLower()).FirstOrDefault();
                    if (oGrpD != null && oGrpD.IsMultiColumnGroup)
                    {
                        //var oGroupFields = oGrpD.BOFieldNames;
                        //var oAllGroupFields = Utility.GetBOGroupFields(oGroupFields, oGrpD.bIsCrtdBy, oGrpD.bIsCrtdWhn, oGrpD.bIsUpdtdBy, oGrpD.bIsUpdtdWhn);
                        //oGrpHeadS.AddRange(oAllGroupFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList());
                        oGrpHeadS.AddRange(oGrpD.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList());
                        if (oGrpHeadS != null && oGrpHeadS.Count() > 0)
                        {
                            foreach (var head in oGrpHeadS)
                            {
                                if (this.BOD.Attributes.ContainsKey(head.ToLower()))
                                {
                                    var oAttrD = this.BOD.Attributes[head.ToLower()];
                                    var sLabel = oAttrD.LabelName;
                                    TableColumns.Add(oAttrD.Name);
                                    Headings.Add(sLabel);
                                    if (!string.IsNullOrEmpty(oAttrD.sFKBOName))
                                    {
                                        FKColumns.Add(oAttrD.Name + "^" + oAttrD.sFKBOName);
                                        FKAttributes.Add(oAttrD.LabelName.ToLower());
                                    }
                                    if (oAttrD.IsOptionList)
                                    {
                                        // OptionListCols.Add(oAttrD.Name.ToLower());
                                        OptionListCols.Add(oAttrD.Name.ToLower(), this.BOD);
                                    }
                                }
                                else
                                {
                                    CResult oCResult = new CResult();
                                    oCResult.sMessage = "Please Check DB/Attr bo, Missing column name : " + head + " in bo is :" + BOD;
                                    oCResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                                    SaveErrortoDB(oCResult);
                                }
                            }
                        }
                    }
                    else
                    {
                        Headings.Add(oGrpD.GroupName);
                        TableColumns.Add(oGrpD.GroupName);
                    }
                }
                else if (items.Contains('.'))
                {
                    var sFKBoName = items.Split('.')[0].Trim().Replace("[", "").Replace("]", "");
                    var sCol = items.Split('.')[1].Replace("[", "").Replace("]", "");
                    var oXIDBO = this.BOD;
                    if (oXIDBO.TableName != null && sFKBoName.ToLower() != oXIDBO.TableName.ToLower())
                    {
                        if (!nXIDBO.ContainsKey(sFKBoName))
                        {
                            var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sFKBoName, null);
                            oXIDBO = oBOD;
                            nXIDBO[sFKBoName] = oXIDBO;
                        }
                        else
                        {
                            oXIDBO = nXIDBO[sFKBoName];
                        }
                    }
                    if (oXIDBO.Attributes.ContainsKey(sCol.ToLower()))
                    {
                        var oAttrD = oXIDBO.Attributes[sCol.ToLower()];
                        var sLabel = oAttrD.LabelName;
                        TableColumns.Add(items);
                        Headings.Add(sLabel);
                        if (oAttrD.IsOptionList)
                        {
                            //OptionListCols.Add(oAttrD.Name.ToLower());
                            OptionListCols.Add(oAttrD.Name.ToLower(), oXIDBO);
                        }
                        if (!string.IsNullOrEmpty(oAttrD.sFKBOName))
                        {
                            FKColumns.Add(oAttrD.Name + "^" + oAttrD.sFKBOName);
                            FKAttributes.Add(oAttrD.LabelName.ToLower());
                        }
                        if (!string.IsNullOrEmpty(oAttrD.sUIStyle))
                        {
                            CssAttributes.Add(oAttrD.Name.ToLower());
                        }
                    }
                    else
                    {
                        if (sCol.ToLower().Contains(XIConstant.Key_XICrtdWhn.ToLower()))
                        {
                            TableColumns.Add(items);
                            Headings.Add("Created On");
                        }
                        else if (sCol.ToLower().Contains(XIConstant.Key_XIUpdtdWhn.ToLower()))
                        {
                            TableColumns.Add(items);
                            Headings.Add("Updated On");
                        }
                        else if (sCol.ToLower().Contains(XIConstant.Key_XICrtdBy.ToLower()))
                        {
                            TableColumns.Add(items);
                            Headings.Add("Created By");
                        }
                        else if (sCol.ToLower().Contains(XIConstant.Key_XIUpdtdBy.ToLower()))
                        {
                            TableColumns.Add(items);
                            Headings.Add("Updated By");
                        }
                        else
                        {
                            TableColumns.Add(items);
                            Headings.Add(sCol);
                        }
                    }
                }
                else
                {
                    if (this.BOD.Attributes.ContainsKey(items.ToLower()))
                    {
                        var oAttrD = this.BOD.Attributes[items.ToLower()];
                        var sLabel = oAttrD.LabelName;
                        TableColumns.Add(oAttrD.Name);
                        Headings.Add(sLabel);
                        if (oAttrD.IsOptionList)
                        {
                            //OptionListCols.Add(oAttrD.Name.ToLower());
                            OptionListCols.Add(oAttrD.Name.ToLower(), this.BOD);
                        }
                        if (!string.IsNullOrEmpty(oAttrD.sFKBOName))
                        {
                            FKColumns.Add(oAttrD.Name + "^" + oAttrD.sFKBOName);
                            FKAttributes.Add(oAttrD.LabelName.ToLower());
                        }
                    }
                    else
                    {
                        CResult oCResult = new CResult();
                        oCResult.sMessage = "Please Check DB/Attr bo, Missing column name : " +items+" in bo is :"+sBOName;
                        oCResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                        SaveErrortoDB(oCResult);
                    }
                }
            }
            if (bIsCheckbox)
            {
                //Headings.Insert(0, "<input type='checkbox' class='chkReconcilliation' Onchange ='fncCheckboxOnchange(this," + ID + "," + CreateGroupID + "," + BOID + "," + iCreateXILinkID + ")' />");
                //TableColumns.Insert(0, "<input type='checkbox' class='chkReconcilliation' Onchange ='fncCheckboxOnchange(this," + ID + "," + CreateGroupID + "," + BOID + "," + iCreateXILinkID + ")' />");
                Headings.Insert(0, "Select");
                TableColumns.Insert(0, "Select");
            }

            if (oOneClickParameters.Where(x => x.iType == 10).Count() > 0)
            {
                if (DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.ResultList.ToString()) || DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.List.ToString()) || DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.Grid.ToString()))
                {
                    Headings.Add("HiddenData");
                    TableColumns.Add("HiddenData");
                }
            }
            else if (oOneClickParameters.Where(x => x.sName.ToLower() == "XIGUID".ToLower()).FirstOrDefault() == null)
            {
                oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = "XIGUID", sValue = "{XIP|XIGUID}" });
                oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = this.BOD.sPrimaryKey, sValue = "{XIP|iInstanceID}" });
                if (DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.ResultList.ToString()) || DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.List.ToString()) || DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.Grid.ToString()))
                {
                    Headings.Add("HiddenData");
                    TableColumns.Add("HiddenData");
                }
            }
            //else if (oOneClickParameters.Where(x => x.iType == 10).Count() == 0)
            //{
            //    Headings.Add("HiddenData");
            //    TableColumns.Add("HiddenData");
            //}
            if ((IsEdit || bIsView || bIsCopy || bIsDelete || bIsPreview || bIsAddBottom || bIsAddTop || bIsOrderIncrement || bIsOrderDecrement || sCompile != "0" || (!string.IsNullOrEmpty(sAssign) && sAssign != "0" && Code != "Assign")) || (Actions != null && Actions.Count() > 0))
            {
                Headings.Add("Actions");
                TableColumns.Add("Actions");
            }
            if (oScripts != null && oScripts.Count() > 0)
            {
                Headings.Add("Color");
                TableColumns.Add("Color");
            }
            if (MyLinks != null && MyLinks.Count() > 0)
            {
                foreach (var Link in MyLinks.Where(m => m.iType == 10))
                {
                    if (!string.IsNullOrEmpty(Link.sName))
                    {
                        Headings.Add(Link.sName);
                        TableColumns.Add(Link.sName);
                    }
                }
            }
            this.Headings = Headings;
        }

        public CResult Resolve1Click(string sBOName = "", string sUID = "")
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

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            try
            {
                foreach (var oBOI in oDataSet.Values)
                {
                    //oBOI.ResloveFKFields();
                    oBOI.iBODID = BOID;
                    oBOI.FormatAttrs();
                    //oBOI.ResolveOptionList();
                    //List<string> dtrow = new List<string>();
                    //dtrow.AddRange(oBOI.Attributes.Values.Select(m => m.sValue).ToList());
                    //oDataTableResult.Add(dtrow.ToArray());
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while Resolving FK Field" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }

        public CResult ReplaceFKExpressions(List<CNV> nParams, bool stag = false)
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

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            try
            {
                Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})", RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(Query);
                if (matches.Count > 0)
                {
                    foreach (var match in matches)
                    {
                        var Prm = "{" + match.ToString() + "}";
                        var Matches = match.ToString();
                        var sExpr = match.ToString().Replace("{", "").Replace("}", "");
                        var Value = nParams.Where(m => m.sName.ToString().ToLower() == Prm.ToLower()).Select(m => m.sValue).FirstOrDefault();
                        var Type = nParams.Where(m => m.sName.ToString().ToLower() == Prm.ToLower()).Select(m => m.sType).FirstOrDefault();
                        if (string.IsNullOrEmpty(Value))
                        {
                            Value = nParams.Where(m => m.sName.ToString().ToLower() == sExpr.ToLower()).Select(m => m.sValue).FirstOrDefault();
                        }
                        if (!string.IsNullOrEmpty(Value))
                        {
                            if (stag == true)
                            {
                                Query = Query.Replace("{" + match.ToString() + "}", "" + Value);
                            }
                            if (Value.Contains(','))
                            {
                                Query = Query.Replace("{" + match.ToString() + "}", "" + Value + "");
                            }
                            else if (Query.Contains("'" + Prm + "'"))
                            {
                                Query = Query.Replace("{" + match.ToString() + "}", Value);
                            }
                            else
                            {
                                if (bMergeQuotes)
                                    Query = Query.Replace("{" + match.ToString() + "}", "'" + Value + "'");
                                else
                                    Query = Query.Replace("{" + match.ToString() + "}", "" + Value + "");
                            }
                        }
                        if (string.IsNullOrEmpty(Value) && (!string.IsNullOrEmpty(Type) && Type.ToLower() == "mandatory"))
                        {
                            if (stag == true)
                            {
                                Query = Query.Replace("{" + match.ToString() + "}", "" + Value);
                            }
                            if (string.IsNullOrEmpty(Value))
                            {
                                Query = Query.Replace("{" + match.ToString() + "}", "null");
                            }
                            else if (Query.Contains("'" + Prm + "'"))
                            {
                                Query = Query.Replace("{" + match.ToString() + "}", Value);
                            }
                            else
                            {
                                if (bMergeQuotes)
                                    Query = Query.Replace("{" + match.ToString() + "}", "'" + Value + "'");
                                else
                                    Query = Query.Replace("{" + match.ToString() + "}", "" + Value + "");
                            }
                        }
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while Resolving FK Expression" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always            
        }

        #region QueryCreation

        public CResult Get_QueryDetails()
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

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            XIDefinitionBase oXID = new XIDefinitionBase();
            try
            {
                XID1Click o1Def = new XID1Click();

                int iType = (Int32)Enum.Parse(typeof(xi1ClcikDisplayAS), xi1ClcikDisplayAS.ResultList.ToString());

                if (ID > 0)
                {
                    List<XIDropDown> ClickParam = new List<XIDropDown>();
                    Dictionary<string, object> CParam = new Dictionary<string, object>();
                    CParam["ID"] = ID;
                    o1Def = Connection.Select<XID1Click>("XI1Click_T", CParam).FirstOrDefault();
                    Dictionary<string, object> GParam = new Dictionary<string, object>();
                    GParam["BOID"] = o1Def.BOID;
                    var GroupFields = Connection.Select<XIDGroup>("XIBOGroup_T_N", GParam).ToList();
                    Dictionary<string, object> FParam = new Dictionary<string, object>();
                    FParam["BOID"] = o1Def.BOID;
                    var BoFields = Connection.Select<XIDAttribute>("XIBOAttribute_T_N", FParam).ToList();
                    if (string.IsNullOrEmpty(o1Def.SelectFields))
                    {
                        string sSelectFields = string.Empty;
                        if (!string.IsNullOrEmpty(o1Def.Query))
                        {
                            var FromIndex = o1Def.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                            var SelectQuery = o1Def.Query.Substring(0, FromIndex);
                            SelectQuery = SelectQuery.TrimEnd();
                            sSelectFields = Regex.Replace(SelectQuery, "select ", "", RegexOptions.IgnoreCase);
                            o1Def.SelectFields = sSelectFields;
                        }
                    }
                    var res2 = o1Def.SelectFields;
                    var SelForGroup = o1Def.SelectFields;
                    var res4 = o1Def.SelectFields;
                    var splitfields = new List<string>();
                    if (res2 != null)
                    {
                        if (res2.Length != 0)
                        {
                            splitfields = res2.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                    }
                    var regex = new Regex("{.*?}");
                    //var matches = regex.Matches(model.Query); //your matches: name, name@gmail.com
                    string[] result;
                    List<groups> groupa = new List<groups>();
                    string selectwithalias = "";
                    foreach (var item in splitfields)
                    {
                        if (item.Contains('{'))
                        {
                            result = item.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                            //var id = Convert.ToInt32(result[0]);
                            int id = 0;
                            if (result.Count() > 1)
                            {
                                var sNewGroup = result[1];
                                id = o1Def.BOGroups.Where(m => m.GroupName == sNewGroup).Select(m => m.ID).FirstOrDefault();
                            }
                            XIDGroup groupname = GroupFields.Where(m => m.ID == id).FirstOrDefault();
                            if (groupname != null)
                            {
                                VisibleQuery = VisibleQuery.Replace('{' + result[0] + '}', groupname.BOSqlFieldNames);
                                res2 = res2.Replace('{' + result[0] + '}', groupname.GroupName);
                                SelForGroup = SelForGroup.Replace('{' + result[0] + '}', groupname.BOFieldNames);
                                res4 = res4.Replace('{' + result[0] + '}', groupname.BOSqlFieldNames);
                                selectwithalias = selectwithalias + "{" + id + "}, ";
                                groupa.Add(new groups
                                {
                                    groupid = groupname.ID,
                                    groupname = groupname.GroupName,
                                    bofieldnames = groupname.BOFieldNames,
                                    bosqlfieldnames = groupname.BOSqlFieldNames,
                                    singlefield = item
                                });
                            }
                        }
                        else if (item.Contains("COUNT"))
                        {
                            selectwithalias = selectwithalias + item + " AS '" + item + "', ";
                            groupa.Add(new groups
                            {
                                singlefield = item,
                                singlefieldtype = "INT",
                                singlealiasname = item,
                                groupid = null,
                                groupname = "",
                                bofieldnames = "",
                                bosqlfieldnames = ""
                            });
                        }
                        else
                        {
                            var name = item.Split(new string[] { " AS " }, StringSplitOptions.RemoveEmptyEntries)[0];
                            int TypeID = BoFields.Where(m => m.Name.ToLower() == name.ToLower()).Select(m => m.TypeID).FirstOrDefault();
                            string fieldname = BoFields.Where(m => m.Name.ToLower() == name.ToLower()).Select(m => m.LabelName).FirstOrDefault();
                            string type = ((BODatatypes)TypeID).ToString();
                            //fieldname = fieldname.Replace(" ", "_");
                            selectwithalias = selectwithalias + name + " AS '" + fieldname + "', ";
                            groupa.Add(new groups
                            {
                                singlefield = name,
                                singlefieldtype = type,
                                singlealiasname = fieldname,
                                groupid = null,
                                groupname = "",
                                bofieldnames = "",
                                bosqlfieldnames = ""
                            });
                        }
                        if (VisibleQuery != null)
                        {
                            o1Def.generalquery = VisibleQuery.Replace("\r\n", " ");
                        }
                        else
                        {
                            string BoName = "";
                            if (!string.IsNullOrEmpty(o1Def.FromBos))
                            {
                                o1Def.generalquery = " FROM " + o1Def.FromBos;
                                o1Def.FromBos = o1Def.FromBos;
                            }
                            else
                            {
                                o1Def.generalquery = " FROM " + o1Def.Name;
                                o1Def.FromBos = o1Def.Name;
                            }
                        }
                        o1Def.generalselectedfields = res2;
                        o1Def.groupallfields = groupa;
                        if (SelForGroup != null && SelForGroup.Length > 0)
                        {
                            var GrFields = SelForGroup.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                            SelForGroup = "";
                            foreach (var items in GrFields)
                            {
                                if (items.IndexOf(" AS ", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    string fld = items.Split(new string[] { " AS " }, StringSplitOptions.RemoveEmptyEntries)[0];
                                    SelForGroup = SelForGroup + fld + ", ";
                                }
                                else
                                {
                                    SelForGroup = SelForGroup + items + ", ";
                                }
                            }
                            SelForGroup = SelForGroup.Substring(0, SelForGroup.Length - 2);
                        }
                        else
                        {
                            SelForGroup = o1Def.GroupFields;
                        }
                        o1Def.selectedgroupfields = SelForGroup;
                        if (res4 != null)
                        {
                            if (res4.Length != 0)
                            {
                                if (!string.IsNullOrEmpty(selectwithalias))
                                {
                                    o1Def.SelectWithAlias = selectwithalias.Substring(0, selectwithalias.Length - 2);
                                }
                            }
                        }
                        //o1Def.parent = dbContext.Reports.Where(m => m.ID == model.ParentID).Select(m => m.Name).FirstOrDefault();
                        if (string.IsNullOrEmpty(o1Def.FromBos))
                        {
                            o1Def.FromBos = o1Def.BO;
                        }
                    }
                }

                List<XIDropDown> AllBOs = new List<XIDropDown>();
                Dictionary<string, object> BOParams = new Dictionary<string, object>();
                BOParams["TypeID"] = 10;
                var oBODef = Connection.Select<XIDBO>("XIBO_T_N", BOParams).ToList();
                AllBOs = oBODef.Select(m => new XIDropDown { Value = m.BOID, text = m.Name }).ToList();
                AllBOs.Insert(0, new XIDropDown { Value = 0, text = "--Select--" });
                o1Def.AllBOs = AllBOs;

                Dictionary<string, string> allbo = new Dictionary<string, string>();
                foreach (var item in o1Def.AllBOs)
                {
                    allbo[item.Value.ToString()] = item.text;
                }
                o1Def.AllBOss = allbo;

                List<XIDropDown> Templates = new List<XIDropDown>();
                Dictionary<string, object> TempParams = new Dictionary<string, object>();
                var oTempDef = Connection.Select<XIContentEditors>("XITemplate_T", TempParams).ToList();

                List<XIDropDown> EmailTemplates = new List<XIDropDown>();
                EmailTemplates = oTempDef.Where(m => m.OrganizationID == OrganizationID).Where(m => m.Category == 1).Select(m => new XIDropDown { Value = m.ID, text = m.Name }).ToList();
                o1Def.EmailTemplates = EmailTemplates;

                List<XIDropDown> SMSTemplates = new List<XIDropDown>();
                SMSTemplates = oTempDef.Where(m => m.OrganizationID == OrganizationID).Where(m => m.Category == 2).Select(m => new XIDropDown { Value = m.ID, text = m.Name }).ToList();
                o1Def.SMSTemplates = SMSTemplates;

                List<XIDropDown> ddlOneClicks = new List<XIDropDown>();
                Dictionary<string, object> OneParams = new Dictionary<string, object>();
                var o1ClickDef = Connection.Select<XID1Click>("XI1Click_T", OneParams).ToList();
                ddlOneClicks = o1ClickDef.Where(m => m.StatusTypeID == 10 || m.StatusTypeID == 36).Select(m => new XIDropDown { Value = m.ID, text = m.Name }).ToList();
                ddlOneClicks.Insert(0, new XIDropDown { Value = 0, text = "--Select--" });
                o1Def.ddlOneClicks = ddlOneClicks;

                List<string> Parent1Clicks = new List<string>();
                Parent1Clicks = o1ClickDef.Where(m => m.Query != null).Where(m => m.StatusTypeID == 10).Where(m => m.DisplayAs == iType).Where(m => m.IsParent == true).Select(m => m.Name).ToList();
                o1Def.Parent1Clicks = Parent1Clicks;

                List<XIDropDown> InnerReports = new List<XIDropDown>();
                InnerReports = o1ClickDef.Where(m => m.Query != null).Where(m => m.StatusTypeID == 10).Where(m => m.DisplayAs == iType).Select(m => new XIDropDown { text = m.Name, Value = m.ID }).ToList();
                o1Def.InnerReports = InnerReports;

                Dictionary<string, object> MasterParams = new Dictionary<string, object>();
                var oMasterDef = Connection.Select<XIDMasterData>("XIMasterData_T", MasterParams).ToList();

                List<XIDropDown> Classes = new List<XIDropDown>();
                if (OrganizationID == 0)
                {
                    Classes = oMasterDef.Where(m => m.Name == "Class Type").Where(m => m.Status == 10).Select(m => new XIDropDown { text = m.Expression, Value = m.ID }).ToList();
                }
                else
                {
                    //Classes = (from c in SpDb.OrganizationClasses.Where(m => m.OrganizationID == OrgID).ToList()
                    //           select new VMDropDown { text = c.Class, Value = c.ClassID }).ToList();
                }
                o1Def.Classes = Classes;

                List<XIDropDown> ReportTypes = new List<XIDropDown>();
                ReportTypes = oMasterDef.Where(m => m.Name == "Report Type").Where(m => m.Status == 10).Select(m => new XIDropDown { text = m.Expression, Value = m.TypeID }).ToList();
                o1Def.ReportTypes = ReportTypes;

                List<XIDropDown> StatusTypes = new List<XIDropDown>();
                StatusTypes = oMasterDef.Where(m => m.Name == "Status Type").Where(m => m.Status == 10).Select(m => new XIDropDown { text = m.Expression, Value = m.ID }).ToList();
                o1Def.StatusTypes = StatusTypes;

                List<XIDropDown> XiLinksList = new List<XIDropDown>();
                Dictionary<string, object> XilinkParams = new Dictionary<string, object>();
                var oXiLinkDef = Connection.Select<XILink>("XILink_T", XilinkParams).ToList();
                XiLinksList = oXiLinkDef.Where(m => m.StatusTypeID == 10).Select(m => new XIDropDown { text = m.Name, Value = m.XiLinkID }).ToList();
                o1Def.XiLinksList = XiLinksList;
                o1Def.XiLinksList.Insert(0, new XIDropDown { Value = 0, text = "--Select--" });

                List<XIDropDown> XIComponentList = new List<XIDropDown>();
                Dictionary<string, object> CompoParams = new Dictionary<string, object>();
                var oCompoDef = Connection.Select<XIDComponent>("XIComponents_XC_T", CompoParams).ToList();
                XIComponentList = oCompoDef.Where(m => m.StatusTypeID == 10).Select(m => new XIDropDown { text = m.sName, Value = m.ID }).ToList();
                o1Def.XIComponentList = XIComponentList;
                o1Def.XIComponentList.Insert(0, new XIDropDown { Value = 0, text = "--Select--" });

                List<XIDropDown> UsersList = new List<XIDropDown>();
                //if (OrganizationID > 0)
                //{
                //    UsersList = Connection.Select<>("XIAPPUsers_AU_T",UserParams).t
                //    UsersList = (from c in Users.Where(m => m.FKiOrgID == OrgID).ToList()
                //                 select new VMDropDown { text = c.sFirstName, Value = c.UserID }).ToList();
                //}
                //else
                //{
                //    UsersList = 
                //    UsersList = (from c in Users.ToList()
                //                 select new VMDropDown { text = c.sFirstName, Value = c.UserID }).ToList();
                //}
                o1Def.TargetUsersList = UsersList;

                o1Def.ddlBOGroups = new List<XIDropDown>();

                List<XIDropDown> AllXIVisualisations = new List<XIDropDown>();
                Dictionary<string, object> VisualParams = new Dictionary<string, object>();
                var oVisualisationList = Connection.Select<XIVisualisation>("XiVisualisations", VisualParams).ToList();
                var sAllXIVisualisations = oVisualisationList.Where(m => m.FKiApplicationID == FKiApplicationID).Where(m => m.OrganisationID == OrganizationID).ToList();
                AllXIVisualisations.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                foreach (var items in sAllXIVisualisations)
                {
                    AllXIVisualisations.Add(new XIDropDown
                    {
                        Value = items.XiVisualID,
                        text = items.Name.ToString()
                    });
                }
                o1Def.ddlVisualisations = AllXIVisualisations;

                Dictionary<string, string> XiLinks = new Dictionary<string, string>();
                var lXiLinks = oXiLinkDef.Where(m => m.OrganisationID == OrganizationID).Where(m => m.FKiApplicationID == FKiApplicationID).ToList();
                foreach (var items in lXiLinks)
                {
                    XiLinks[items.Name] = items.Name;
                }
                o1Def.XILinks = XiLinks;

                List<XIDropDown> Layouts = new List<XIDropDown>();
                Dictionary<string, object> LayParams = new Dictionary<string, object>();
                var oLayoutDef = Connection.Select<XIDLayout>("XILayout_T", LayParams).ToList();
                Layouts = oLayoutDef.Where(m => m.FKiApplicationID == FKiApplicationID).Where(m => m.OrganizationID == OrganizationID).Select(m => new XIDropDown { Value = m.ID, text = m.LayoutName }).ToList();
                Layouts.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                o1Def.ddlLayouts = Layouts;

                List<XIDropDown> LayoutMappings = new List<XIDropDown>();
                Dictionary<string, object> LayMapParams = new Dictionary<string, object>();
                var oLayoutMapDef = Connection.Select<XIDLayoutDetails>("XILayoutDetail_T", LayMapParams).ToList();
                LayoutMappings = oLayoutMapDef.Where(m => m.LayoutID == iLayoutID).Select(m => new XIDropDown { Value = m.PlaceHolderID, text = m.PlaceholderName }).ToList();
                LayoutMappings.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                o1Def.ddlLayoutMappings = LayoutMappings;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = o1Def;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while Getting 1Click Details" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        #endregion QueryCreation

        /*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
          :: Genereate Name Value Object of XIIBO Records for DataCompare::
          :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::*/
        public Dictionary<string, XIIBO> GetNameValue(XIDBO oBOD)
        {
            CResult oCResult = new CResult();
            Dictionary<string, XIIBO> nBOIns = new Dictionary<string, XIIBO>();
            try
            {
                Connection = new XIDBAPI(sConnectionString);
                var oBOIns = (DataTable)Connection.ExecuteQuery(Query, ListSqlParams);

                Dictionary<string, XIIAttribute> obj = new Dictionary<string, XIIAttribute>();
                int j = 0;
                foreach (DataRow row in oBOIns.Rows)
                {
                    //var result = Enumerable.Range(0, oBOIns.Columns.Count).ToDictionary(i => oBOIns.Columns[i].ColumnName, i => row.ItemArray[i].ToString());
                    //obj.Add(result);
                    XIIBO oBOII = new XIIBO();
                    obj = Enumerable.Range(0, oBOIns.Columns.Count)
                               .ToDictionary(i => oBOIns.Columns[i].ColumnName.ToLower(), i => new XIIAttribute
                               {
                                   sName = oBOIns.Columns[i].ColumnName,
                                   sValue = row.ItemArray[i].ToString(),
                                   sPreviousValue = row.ItemArray[i].ToString(),
                                   //sDisplayName
                                   sDisplayName = oBOD.Attributes.ContainsKey(oBOIns.Columns[i].ColumnName) ? oBOD.AttributeD(oBOIns.Columns[i].ColumnName).LabelName : "",
                               }, StringComparer.CurrentCultureIgnoreCase);
                    oBOII.Attributes = obj;
                    nBOIns[j.ToString()] = oBOII;
                    j++;
                }
                return nBOIns;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while Getting 1Click Details" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                throw;
            }
        }
        /*:::::::::::::::::::::::::::::::::::::::::::::::
          :: Getting Table as DataTable for DataCompare::
          :::::::::::::::::::::::::::::::::::::::::::::::*/
        public DataTable GetDataTable()
        {
            CResult oCResult = new CResult();
            try
            {
                Connection = new XIDBAPI(sConnectionString);
                var oBOIns = (DataTable)Connection.ExecuteQuery(Query, ListSqlParams);
                return oBOIns;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while Getting 1Click Details" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                throw;
            }
        }

        public Dictionary<string, XIIBO> OneClick_ExecuteV2()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            Dictionary<string, XIIBO> nBOIns = new Dictionary<string, XIIBO>();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOID.ToString());
                var iDatasource = oBOD.iDataSource;
                XIDXI oXID = new XIDXI();
                var oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, oBOD.iDataSource.ToString());
                //XIDBMongoDB oMongoDB = new XIDBMongoDB();
                //oMongoDB.sServer = oDataSource.sServer;
                //oMongoDB.sDatabase = oDataSource.sDatabase;
                //oMongoDB.sTable = oBOD.TableName;
                //oCR = oMongoDB.Get_Data();
                if (oCR.bOK && oCR.oResult != null)
                {
                    oTrace.oTrace.Add(oCR.oTrace);
                    var Data = (List<BsonDocument>)oCR.oResult;
                    int j = 1;
                    foreach (var record in Data)
                    {
                        var Attrs = record.Names.ToList();
                        var Values = record.Values.ToList();
                        XIIBO oBOI = new XIIBO();
                        Dictionary<string, XIIAttribute> dictionary = new Dictionary<string, XIIAttribute>();

                        for (var i = 0; i < Attrs.Count(); i++)
                        {
                            if (i > 0)
                            {
                                dictionary[Attrs[i]] = new XIIAttribute() { sName = Attrs[i], sValue = Values[i].RawValue.ToString() };
                            }
                        }
                        oBOI.Attributes = dictionary;
                        nBOIns[j.ToString()] = oBOI;
                        j++;

                        oDataSet = nBOIns;
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = "Success";
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return nBOIns;
        }

        public CResult Check_QueryAccess(int iRoleID, int iOrgID, int iAppID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Check 1Query access to allow execution ";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "i1ClickID", sValue = ID.ToString() });
                if (ID > 0)//check mandatory params are passed or not
                {
                    string sKey = ID + "_" + iRoleID + "_" + iOrgID + "_" + iAppID;
                    XIIBO oQueryI = new XIIBO();
                    XIInfraCache oCache = new XIInfraCache();
                    var oLinkAccess = (Dictionary<string, object>)oCache.GetObjectFromCache(XIConstant.CacheQueryAccess, "CacheQueryAccess");
                    if (oLinkAccess.ContainsKey(sKey))
                    {
                        oQueryI = (XIIBO)oLinkAccess[sKey];
                        if (oQueryI != null && oQueryI.Attributes.Count() > 0)
                        {
                            var bRead = oQueryI.AttributeI("bRead").sValue;
                            var bRun = oQueryI.AttributeI("bRun").sValue;
                            oCResult.oResult = bRun;
                        }
                        else
                        {
                            oCResult.oResult = true;
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: iLinkID:" + ID + " is missing";
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public Dictionary<string, List<XIIBO>> OneClick_Execute_Splitting(List<CNV> SessionItems = null, XID1Click o1ClickD = null)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            Dictionary<string, List<XIIBO>> nBOIns = new Dictionary<string, List<XIIBO>>();
            XIInfraUsers oUser = new XIInfraUsers();
            CUserInfo oInfo = new CUserInfo();
            if (SessionItems == null)
            {
                oInfo = oUser.Get_UserInfo();
            }
            else
            {
                oInfo = oInfo.GetUserInfo(SessionItems);
            }
            sOrginalQuery = Query;
            if (o1ClickD != null)
            {
                bIsApplyActorWhereClause = o1ClickD.bIsApplyActorWhereClause;
            }
            else
            {
                bIsApplyActorWhereClause = true;
            }
            XIDBO oBOD = new XIDBO();
            if (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)
            {
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOIDXIGUID.ToString());
            }
            else if (BOID > 0)
            {
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOID.ToString());
            }
            else if (Name != null)
            {
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, Name, null);
            }
            var WhiteListCheck = System.Configuration.ConfigurationManager.AppSettings["WhitelistCheck"];
            if (WhiteListCheck == "yes")
            {
                if (oBOD != null && oBOD.Name.ToLower() != "BO WhiteList".ToLower() && oBOD.BOID != 2459)
                {
                    XIIBO oBOI = new XIIBO();
                    oCR = oBOI.Check_Whitelist(oBOD.BOID, oInfo.iRoleID, oInfo.iOrganizationID, oInfo.iApplicationID, "query", oBOD.iLevel, oInfo.iLevel);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var bUNAuth = (bool)oCR.oResult;
                        if (bUNAuth)
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            return new Dictionary<string, List<XIIBO>>();
                        }
                    }
                }
            }
            oCR = Check_QueryAccess(oInfo.iRoleID, oInfo.iOrganizationID, oInfo.iApplicationID);
            if (oCR.bOK && oCR.oResult != null)
            {
                var bAllowed = false;
                var bUNAuth = (string)oCR.oResult;
                bool.TryParse(bUNAuth, out bAllowed);
                if (!bAllowed)
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    return new Dictionary<string, List<XIIBO>>();
                }
            }
            int iDataSource = oBOD.iDataSource;
            Guid iDataSourceXIGUID = oBOD.iDataSourceXIGUID;
            XIDXI oXID = new XIDXI();
            string sConntection = string.Empty;

            if (iDataSource > 0 || (iDataSourceXIGUID != null && iDataSourceXIGUID != Guid.Empty))
            {
                if (oBOD.TableName.ToLower() == "Organizations".ToLower() || oBOD.TableName.ToLower() == "XIBOWhiteList_T".ToLower() || oBOD.TableName.ToLower() == "XIAPPUsers_AU_T".ToLower() || oBOD.TableName.ToLower() == "XIAppRoles_AR_T".ToLower() || oBOD.TableName.ToLower() == "XIAppUserRoles_AUR_T".ToLower() || oBOD.TableName.ToLower() == "XIUserOrgMapping_T".ToLower())
                {
                    oInfo = oUser.Get_UserInfo();
                    if (oInfo.sCoreDataBase != null)
                    {
                        var DataSource = oXID.Get_DataSourceDefinition(oInfo.sCoreDataBase);
                        var BODS = ((XIDataSource)DataSource.oResult);
                        sConntection = oXID.GetBODataSource(BODS.XIGUID.ToString(), 0);
                    }
                    else
                    {
                        sConntection = oXID.GetBODataSource(iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                    }

                }
                else if (oBOD.TableName == "RefTraceStage_T" || oBOD.TableName == "refValidTrace_T" || oBOD.TableName == "refLeadQuality_T" || oBOD.TableName == "TraceTransactions_T")
                {
                    oInfo = oUser.Get_UserInfo();
                    if (oInfo.sDatabaseName != null)
                    {
                        var DataSource = oXID.Get_DataSourceDefinition(oInfo.sDatabaseName);
                        var BODS = ((XIDataSource)DataSource.oResult);
                        sConntection = oXID.GetBODataSource(BODS.XIGUID.ToString(), 0);
                    }
                    else
                    {
                        sConntection = oXID.GetBODataSource(iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(sSwitchDB))
                    {
                        var oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, sSwitchDB);
                        sConntection = oXID.GetBODataSource(oDataSource.XIGUID.ToString(), oBOD.FKiApplicationID);
                    }
                    else
                    {
                        sConntection = oXID.GetBODataSource(iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                    }
                }

            }
            else if (!string.IsNullOrEmpty(sSwitchDB))
            {
                var oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, sSwitchDB);
                sConntection = oXID.GetBODataSource(oDataSource.XIGUID.ToString(), oBOD.FKiApplicationID);
            }
            else
                sConntection = oXID.GetBODataSource(iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);

            Connection = new XIDBAPI(sConntection);
            this.BOD = oBOD;
            if (bIsResolveFK)
            {
                ResolveFKColumns(oBOD, oInfo.sCoreDataBase);
            }
            if (Query.ToLower().Contains("case"))
            {
            }
            else
            {
                ResolveGroup(oBOD);
            }
            if (IsFkGUID)
            {
                var ActiveBoD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, sParentBO);
                List<string> Whrparams = new List<string>();
                //var sChangeWhrCondition = oCache.Get_ParamVal(sSessionID, sGUID, "", "{XIP|" + oCopy.ID + "_ChangeWhrCondition}");
                if (ActiveBoD.bUID)
                {
                    // oCache.Set_ParamVal(sSessionID, sGUID, "", "{XIP|" + o1ClickD.ID + "_ChangeWhrCondition}", "yes", "", null);
                    var sQuery = Query.Split(new string[] { " where " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    //o1ClickC.Query.Split("where");
                    if (sQuery.Count > 1 && !string.IsNullOrEmpty(sQuery[1]))
                    {
                        string GroupBY = "";
                        if (sQuery[1].IndexOf(" group by", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var pos = sQuery[1].IndexOf("group by", StringComparison.OrdinalIgnoreCase);

                            GroupBY = sQuery[1].Substring(pos);
                            GroupBY = sQuery[1].Insert(0, " ");
                            sQuery[1] = sQuery[1].Substring(0, pos - 1);
                        }
                        else if (sQuery[1].IndexOf(" order by", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var pos = sQuery[1].IndexOf("order by", StringComparison.OrdinalIgnoreCase);
                            GroupBY = sQuery[1].Substring(pos);
                            GroupBY = GroupBY.Insert(0, " ");
                            sQuery[1] = sQuery[1].Substring(0, pos - 1);
                        }
                        Query = sQuery[0];
                        Query = string.Concat(Query, GroupBY);
                        var sWhrL = sQuery[1].Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var spt in sWhrL)
                        {
                            var splits = spt.Split('=').ToList();
                            if (splits[0].ToLower() == ActiveBoD.sPrimaryKey.ToLower() && oBOD.Attributes.ContainsKey(ActiveBoD.sPrimaryKey.ToLower() + "xiguid"))
                            {
                                splits[0] = ActiveBoD.sPrimaryKey + "XIGUID";
                            }
                            Whrparams.Add(splits[0] + "=" + splits[1]);

                        }
                        if (Whrparams.Count > 0)
                        {
                            var sCondition = string.Join(" and ", Whrparams);
                            Query = string.Concat(sQuery[0], " where ", sCondition, GroupBY);
                        }
                    }

                }
            }
            //**********************************Check and build Query for result using hierarchy ********************************
            if (oBOD.bIsHierarchy)
            {
                string sHierarchyCondition = string.Empty;
                if (!string.IsNullOrEmpty(oInfo.sViewHierarchy) && oInfo.sViewHierarchy.ToLower() == XIConstant.HierarchyAll.ToLower())
                    sHierarchyCondition = null;
                else if (!string.IsNullOrEmpty(oInfo.sViewHierarchy) && oBOD.Attributes.Keys.Contains("sHierarchy"))
                {
                    int i = 1;
                    foreach (var s in oInfo.sViewHierarchy.Split('|'))
                    {
                        sHierarchyCondition += (!string.IsNullOrEmpty(sHierarchyCondition) ? " Or " : "") + " sHierarchy like '%'+@param" + i + "+'%' ";
                        AddSqlParameters("@param" + i, s);
                        i++;
                    }
                }
                else if (string.IsNullOrEmpty(oInfo.sViewHierarchy))
                {
                    sHierarchyCondition = "sHierarchy like '%'+@Hierarchy+'%' ";
                    AddSqlParameters("@Hierarchy", XIConstant.HierarchyNull);
                }
                if (!string.IsNullOrEmpty(sHierarchyCondition))
                    Query = AddSearchParameters(Query, sHierarchyCondition);
            }
            string sSearchKey = string.Empty;
            //*********************************************************************************************************************
            if (!string.IsNullOrEmpty(SearchType))
            {
                if (SearchType.ToLower() == "FilterSearch".ToLower())
                {
                    if (Fields != null && Fields.Length > 0)
                    {
                        var Condition = GetDynamicSearchStrings(Fields, Optrs, Values, oBOD);
                        if (Condition.Length > 0)
                        {
                            Query = AddSearchParameters(Query, Condition);
                            if (Query.Contains("WHERE") == true && Query.Contains("{XIP|") == true)
                            {
                                Query = Query.Replace("WHERE", "and");
                            }
                        }
                    }
                }
                else
                {
                    string sGroupName = string.Empty;
                    if (FKiSearchGroup > 0)
                    {
                        var GrpD = oBOD.Groups.Values.ToList().Where(m => m.ID == FKiSearchGroup).FirstOrDefault();
                        if (GrpD != null)
                        {
                            sGroupName = GrpD.GroupName;
                        }
                    }
                    else
                    {
                        sGroupName = SearchType.ToLower() == "quick" ? "quick search" : "natural search";
                    }
                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        sSearchKey = SearchText;
                        XIDGroup oGroupD;
                        if (oBOD.Groups.TryGetValue(sGroupName, out oGroupD))
                        {
                            SearchText = ResolveSearchString(oGroupD, SearchText, oInfo.sCoreDataBase);
                        }
                    }
                }
            }
            else
            {
                SearchType = "";
            }
            string sHideSelect = string.Empty;
            bool bManualGUID = false;
            if ((oOneClickParameters.Where(x => x.iType == 10).Count() == 0 || oOneClickParameters.Where(x => x.sName == "XIGUID").Count() == 0) && oBOD.Attributes.ContainsKey("xiguid") && (o1ClickD != null && !o1ClickD.bIsMultiBO) && !oBOD.bUID && bIsHidden)
            {
                if (oOneClickParameters.Where(x => x.iType == 10).Count() == 0)
                {
                    bManualGUID = true;
                }
                oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = "XIGUID", sValue = "{XIP|XIGUID}" });
            }
            if (oOneClickParameters.Where(x => x.sName.ToLower() == "XIGUID".ToLower()).FirstOrDefault() == null && bIsHidden)
            {
                oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = "XIGUID", sValue = "{XIP|XIGUID}" });
            }
            if (oOneClickParameters.Where(x => x.iType == 10).Count() > 0 || oBOD.bUID)
            {
                //if (DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.ResultList.ToString()))
                //{
                //if (oOneClickParameters.Where(x => x.iType == 10).Count() == 0)
                //{
                //    oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = "XIGUID", sValue = "{XIP|XIGUID}" });
                //}
                if (oOneClickParameters.Where(x => x.sName == "XIGUID").Count() == 0 && bIsHidden)
                {
                    oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = "XIGUID", sValue = "{XIP|XIGUID}" });
                }
                //else if (oOneClickParameters.Where(x => x.sName == "XIGUID").Count() == 0)
                //{
                //    oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = "XIGUID", sValue = "{XIP|XIGUID}" });
                //}
                var ohideParams = oOneClickParameters.Where(x => x.iType == 10).Select(m => m.sName).ToList();
                if (ohideParams != null && ohideParams.Count() > 0)
                {
                    var oHideparm1 = ohideParams.Where(m => m.Contains(".")).ToList();
                    var oHideparm2 = ohideParams.Where(m => !m.Contains(".")).ToList();
                    var sConcatStr = oHideparm2.Select(f => "CONCAT('" + f + "','__',[" + oBOD.TableName + "]." + f + ")").ToList();
                    var sConcatStr2 = oHideparm1.Select(f => "CONCAT('" + f + "','__'," + f + ")").ToList();
                    sConcatStr = sConcatStr.Concat(sConcatStr2).ToList();
                    if (sConcatStr.Count() > 1)
                    {
                        sHideSelect = "CONCAT(" + string.Join(",':',", sConcatStr) + ") AS 'HiddenData'";
                    }
                    else
                    {
                        sHideSelect = sConcatStr.FirstOrDefault() + " AS 'HiddenData'";
                    }
                }
                //}
            }

            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(SearchText))
            {
                Query = AddSearchParameters(Query, SearchText, sJoins);
            }
            if (Query.IndexOf("izxdeleted", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                Query = Regex.Replace(Query, "izxdeleted", XIConstant.Key_XIDeleted, RegexOptions.IgnoreCase);
            }
            if (oBOD != null && oBOD.sDeleteRule == "10")
            {
                if (Query.IndexOf(XIConstant.Key_XIDeleted, StringComparison.CurrentCultureIgnoreCase) > 0)
                {

                }
                else
                {
                    string sCondition = "[" + oBOD.TableName + "]." + XIConstant.Key_XIDeleted + "= 0";
                    Query = AddSearchParameters(Query, sCondition, sJoins);
                }
            }

            if (!string.IsNullOrEmpty(sHideSelect))
            {
                Query = AddSelectPart(Query, sHideSelect, "append");
                if (sOrginalQuery.ToLower().Contains(" group by"))
                {
                    Query = string.Concat(Query, ", [" + oBOD.TableName + "].XIGUID");
                }
            }
            if (!string.IsNullOrEmpty(sParentWhere))
            {
                if (sParentWhere.Contains(" and "))
                {
                    var Split = sParentWhere.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var spt in Split)
                    {
                        var splits = spt.Split('=').ToList();
                        AddSqlParameters("@" + splits[0], splits[1]);
                        var sParentWhr = "[" + oBOD.TableName + "]." + splits[0] + "=@" + splits[0];
                        Query = AddSearchParameters(Query, sParentWhr, sJoins);
                    }
                }
                else
                {
                    var splits = sParentWhere.Split('=').ToList();
                    AddSqlParameters("@" + splits[0], splits[1]);
                    var sParentWhr = "[" + oBOD.TableName + "]." + splits[0] + "=@" + splits[0];
                    Query = AddSearchParameters(Query, sParentWhr, sJoins);
                }
            }
            string sMessage = string.Empty;
            if (!string.IsNullOrEmpty(oInfo.sWhereField) && oBOD != null && oBOD.Attributes.ContainsKey(oInfo.sWhereField.ToLower()) && bIsApplyActorWhereClause)
            {
                SearchText = oBOD.TableName + "." + oInfo.sWhereField + " = " + oInfo.iWhereFieldValue;
                Query = AddSearchParameters(Query, SearchText);
            }
            if (RoleIDs != null && RoleIDs.Count() > 0 && RoleIDs.Contains(oInfo.iRoleID) || true)
            {
                if (bIsLockToUser && !string.IsNullOrEmpty(oInfo.sRoleName) && oInfo.sRoleName.ToLower() == xiEnumSystem.EnumRoles.WebUsers.ToString().ToLower() && !bIsXILoad && !bIsMultiBO)
                {
                    SearchText = "FKiUserID = @FKiUserID";
                    AddSqlParameters("@FKiUserID", oInfo.iUserID);
                    Query = AddSearchParameters(Query, SearchText);
                }
                if (NVPairs != null)
                {
                    var Query = ReplaceQueryString(NVPairs);
                }
                if (Query.Contains("{") || Query.Contains("|"))
                {
                    oCResult.sMessage = "Critical Error: Unresolved Query in GetOneClickResult Method, Query Name : " + Name + ", Query : " + Query + " and ID: " + ID;
                    SaveErrortoDB(oCResult);
                }
                string Count = "";
                if (sOrginalQuery.ToLower().Contains(" distinct"))
                {
                    Query = Query.Insert(7, "distinct ");
                }
                if (Query.ToLower().Contains("distinct") || Query.ToLower().Contains("case") || Query.ToLower().Contains("pivot"))
                {
                    Count = Connection.GetRecordCount(CommandType.Text, Query, ListSqlParams);
                }
                else
                {
                    string StarQuery = ReplaceQueryStringWithStar(Query);
                    Count = Connection.GetTotalCount(CommandType.Text, StarQuery, ListSqlParams);
                }
                if (iSkip >= 0 && iTake > 0 && Query.ToLower().Contains(" order by "))
                {
                    if (TableColumns != null)
                    {
                        string sortExpression = TableColumns[iSortCol];
                        string sortDirection = sSortDir;
                        if (string.IsNullOrEmpty(sortDirection))
                        {
                            sortDirection = "desc";
                        }
                        var sOrderNew = " order by " + sortExpression + " " + sortDirection;

                        var sOrderI = Query.ToLower().IndexOf(" order by ");
                        var sOrderOld = Query.Substring(sOrderI, Query.Length - sOrderI);
                        sOrderOld = sOrderOld.Replace(" order by ", "");
                        var ords = sOrderOld.Split(' ').ToList();
                        var OrdCol = ords[0];
                        var OrdDir = ords[1];

                        sOrderOld = " order by " + OrdCol + " " + OrdDir;
                        if (string.IsNullOrEmpty(OrderFields))
                            Query = Query.ToLower().Contains(" order by  order") || (Query.ToLower().Contains(" order by") && sortExpression != "Select") ? Query.Replace(sOrderOld, sOrderNew) : Query.ToLower().Contains("asc") || Query.ToLower().Contains("desc") ? Query : Query + " " + sortDirection;
                        //Query = Query.Replace(sOrderOld, sOrderNew);

                        Query = Query + " OFFSET @iSkip ROWS FETCH NEXT @iTake ROWS ONLY";
                        AddSqlParameters("@iSkip", iSkip);
                        AddSqlParameters("@iTake", iTake);
                    }
                }
                else if (iSkip >= 0 && iTake > 0 && !Query.ToLower().Contains(" order by "))
                {
                    string sortExpression = string.Empty;
                    if (TableColumns != null)
                    {
                        sortExpression = TableColumns[iSortCol];
                        string sortDirection = sSortDir;
                        if (string.IsNullOrEmpty(sortDirection))
                        {
                            sortDirection = "desc";
                        }
                        var sOrderNew = " order by " + sortExpression + " " + sortDirection;
                        Query = Query + sOrderNew;

                        Query = Query + " OFFSET @iSkip ROWS FETCH NEXT @iTake ROWS ONLY";
                        AddSqlParameters("@iSkip", iSkip);
                        AddSqlParameters("@iTake", iTake);
                    }
                }
                //var oBOIns = (DataTable)Connection.ExecuteQuery(Query);
                DataTable oBOIns = new DataTable();
                #region QUERYCACHE_CODE
                if (CategoryID == Convert.ToInt32(Enumxicache.QueryCache) && DisplayAs == Convert.ToInt32(xi1ClcikDisplayAS.List) && !string.IsNullOrEmpty(Query)) // check Query cache is Enabled or not
                {
                    string Queryhash = Utility.ConvertBase64(Utility.RemoveCharacters(Query.Trim(), "", "", " "));
                    string sQcachekey = ID.ToString() + "_" + Queryhash;
                    object oCached = oCache.GetFromCache(sQcachekey);
                    if (oCached == null) // Cache is empty then get from DB and Insert into Cache
                    {
                        //Execute Query and Insert into Cache
                        oBOIns = (DataTable)Connection.ExecuteQuery(Query, ListSqlParams);
                        oCache.InsertIntoCache(oBOIns, sQcachekey);
                    }
                    else
                    {
                        oBOIns = (DataTable)oCached;
                    }
                }
                else
                {
                    oBOIns = (DataTable)Connection.ExecuteQuery(Query, ListSqlParams);
                }
                #endregion

                DataView dv = oBOIns.DefaultView;
                oBOIns = dv.ToTable();

                Dictionary<string, XIIAttribute> dictionary = new Dictionary<string, XIIAttribute>();
                Dictionary<string, XIIValue> XIValuedictionary = new Dictionary<string, XIIValue>();
                var j = 0;

                if (oBOD != null)
                {
                    List<DataRow> Rows = new List<DataRow>();
                    //iTotalRecords = oBOIns.AsEnumerable().ToList().Count();
                    iTotalRecords = Convert.ToInt32(Count);
                    if (iSkip >= 0 && iTake > 0 && !Query.ToLower().Contains(" order by "))
                    {
                        Rows = oBOIns.AsEnumerable().Skip(iSkip).Take(iTake).ToList();
                    }
                    else
                    {
                        Rows = oBOIns.AsEnumerable().ToList();
                    }
                    //iTotaldisplayRecords = Convert.ToInt32(Rows.Count());
                    //var AllCols = oBOIns.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                    TotalColumns = new List<string>();
                    //OptionListCols = OptionListCols ?? new List<string>();
                    OptionListCols = OptionListCols ?? new Dictionary<string, XIDBO>();
                    List<XIIBO> oLBOII = new List<XIIBO>();
                    foreach (DataRow row in Rows)
                    {
                        XIIBO oBOII = new XIIBO();
                        dictionary = Enumerable.Range(0, oBOIns.Columns.Count)
                            .ToDictionary(i => oBOIns.Columns[i].ColumnName.ToLower(), i => new XIIAttribute
                            {
                                sName = oBOIns.Columns[i].ColumnName,
                                //sValue = OptionListCols.Contains(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), oBOD) : row.ItemArray[i].ToString(),
                                //sValue = SearchType.ToLower() == "quick" ? Regex.Replace(OptionListCols.ContainsKey(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOIns.Columns[i].ColumnName.ToLower()]) : row.ItemArray[i].ToString(), sSearchKey, "<span class=\"themecolor\">" + sSearchKey + "</span>", RegexOptions.IgnoreCase) : OptionListCols.ContainsKey(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOIns.Columns[i].ColumnName.ToLower()]) : row.ItemArray[i].ToString(),
                                //sValue = (!string.IsNullOrEmpty(SearchType) && SearchType.ToLower() == "quick") ? Regex.Replace(OptionListCols.ContainsKey(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOIns.Columns[i].ColumnName.ToLower()]) : row.ItemArray[i].ToString(), sSearchKey, sSearchKey, RegexOptions.IgnoreCase) : OptionListCols.ContainsKey(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOIns.Columns[i].ColumnName.ToLower()]) : row.ItemArray[i].ToString(),
                                sValue = (!string.IsNullOrEmpty(SearchType) && SearchType.ToLower() == "quick") ? Regex.Replace(OptionListCols.ContainsKey(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOIns.Columns[i].ColumnName.ToLower()], oBOIns.Columns, row.ItemArray) : row.ItemArray[i].ToString(), sSearchKey, sSearchKey, RegexOptions.IgnoreCase) : OptionListCols.ContainsKey(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOIns.Columns[i].ColumnName.ToLower()], oBOIns.Columns, row.ItemArray) : row.ItemArray[i].ToString(),
                                sPreviousValue = row.ItemArray[i].ToString(),
                                sDisplayName = oBOD.Attributes.ContainsKey(oBOIns.Columns[i].ColumnName) ? oBOD.AttributeD(oBOIns.Columns[i].ColumnName).LabelName : "",
                                //iValue = TotalColumns.Contains(oBOIns.Columns[i].ColumnName.ToLower()) ? (!string.IsNullOrEmpty(row.ItemArray[i].ToString()) ? (Convert.ToInt32(row.ItemArray[i].ToString())) : 0) : 0,
                            }, StringComparer.CurrentCultureIgnoreCase);
                        oBOII.Attributes = dictionary;
                        if (oBOII.Attributes.ContainsKey("hiddendata") && !string.IsNullOrEmpty(oBOII.Attributes["hiddendata"].sValue))
                        {
                            var sHiddenAttrArray = oBOII.Attributes["hiddendata"].sValue.Split(':');
                            for (var Attr = 0; Attr < sHiddenAttrArray.Length; Attr++)
                            {
                                if (sHiddenAttrArray[Attr] != "")
                                {
                                    var oAttr = sHiddenAttrArray[Attr].Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    if (oAttr.Count > 1 && !string.IsNullOrEmpty(oAttr[0]) && oAttr[0].ToLower() == "xiguid")
                                    {
                                        oBOII.XIGUID = oAttr[1];
                                    }
                                    if (oAttr[0].ToLower() == "xiguid" && bManualGUID)
                                    {
                                        oBOII.Attributes["hiddendata"].sValue = oBOII.Attributes["hiddendata"].sValue.Replace(sHiddenAttrArray[Attr], "").TrimEnd(':');
                                    }
                                }
                            }
                        }
                        if (DisplayAs != (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.ResultList.ToString()) || bManualGUID)
                        {
                            oBOII.Attributes.Remove("hiddendata");
                        }
                        XIValuedictionary = Enumerable.Range(0, oBOIns.Columns.Count)
                             .ToDictionary(i => oBOIns.Columns[i].ColumnName, i => new XIIValue
                             {
                                 sValue = row.ItemArray[i].ToString(),
                                 sDisplayName = oBOD.Attributes.ContainsKey(oBOIns.Columns[i].ColumnName) ? oBOD.AttributeD(oBOIns.Columns[i].ColumnName).LabelName : "",
                             }, StringComparer.CurrentCultureIgnoreCase);
                        oBOII.XIIValues = XIValuedictionary;
                        oBOII.iBODID = oBOD.BOID;
                        //if (oBOD != null)
                        //{
                        //    if (oBOII.BOD == null)
                        //    {
                        //        oBOII.BOD = oBOD;
                        //    }
                        //}
                        oLBOII.Add(oBOII);

                        nBOIns[j.ToString()] = oLBOII;
                        j++;
                    }
                    if (DisplayAs == Convert.ToInt32(xi1ClcikDisplayAS.List) || (DisplayAs == Convert.ToInt32(xi1ClcikDisplayAS.ResultList) && !string.IsNullOrEmpty(sTotalColumns)))
                    {
                        //var WhereQuery = "";
                        var FromQuery = "";
                        //var colName = oBOD.Attributes.Values.Where(m => m.IsTotal == true).ToList();
                        var colName = new List<string>();
                        if (!string.IsNullOrEmpty(sTotalColumns))
                        {
                            colName = sTotalColumns.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                        if (colName.Count() > 0)
                        {
                            int index = sOrginalQuery.ToLower().IndexOf(" from ");
                            FromQuery = sOrginalQuery.Substring(index);
                            if (FromQuery.ToLower().Contains(" group by "))
                            {
                                index = FromQuery.ToLower().IndexOf(" group by ");
                                if (index != -1)
                                {
                                    FromQuery = FromQuery.Substring(0, index);
                                }
                            }
                            if (FromQuery.ToLower().Contains(" order by "))
                            {
                                index = FromQuery.ToLower().IndexOf(" order by ");
                                if (index != -1)
                                {
                                    FromQuery = FromQuery.Substring(0, index);
                                }
                            }
                            Dictionary<string, XIIBO> nTotRows = Get_TotalQuery(oBOD, FromQuery);
                            if (nTotRows != null && nTotRows.Values.Count() > 0)
                            {
                                foreach (var rtot in nTotRows.Values)
                                {
                                    //nBOIns[j.ToString()] = rtot;
                                    j++;
                                }
                            }
                        }
                    }
                }
                oLDataSet = nBOIns;
            }
            else
            {
                sMessage = "Access Denied";
            }
            return nBOIns;
        }

    }
}