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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDatabase;
using System.Net;
using XISystem;

namespace XICore
{
    public class XIDBO : XIDefinitionBase
    {
        [Key]
        public int BOID { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        public string Name { get; set; }
        public string sNameAttribute { get; set; }
        public int OrganizationID { get; set; }
        public int TypeID { get; set; }
        public string Description { get; set; }
        public bool IsClassEnabled { get; set; }
        public int FieldCount { get; set; }
        public string LabelName { get; set; }
        public string TableName { get; set; }
        public string ClassName { get; set; }
        public string sType { get; set; }
        public string sSection { get; set; }
        public string sVersion { get; set; }
        public string sSize { get; set; }
        public bool bUID { get; set; }
        public string sPrimaryKey { get; set; }
        public string sTimeStamped { get; set; }
        public string sDeleteRule { get; set; }
        public string sSearchType { get; set; }
        public int iDataSource { get; set; }
        public Guid iDataSourceXIGUID { get; set; }
        public string sNotes { get; set; }
        public string sHelpItem { get; set; }
        public string sUpdateVersion { get; set; }
        public string sAudit { get; set; }
        public int iUpdateCount { get; set; }
        public bool bIsAutoIncrement { get; set; }
        public int iOneClickID { get; set; }
        public int iTransactionEnable { get; set; }
        public string sAuditBOName { get; set; }
        public string sAuditBOfield { get; set; }
        public bool bIsHierarchy { get; set; }
        public List<XIDropDown> DataSources { get; set; }
        public List<XIDropDown> ddlBOFieldAttributes { get; set; }
        public string sColumns { get; set; }
        public List<XIDropDown> HelpTypes { get; set; }
        public List<XIDropDown> oAttrList { get; set; }
        public List<XIDropDown> ddlApplications { get; set; }
        private XIDBOUI oMyBOUI;
        public int iUserID { get; set; }
        public int iOrgID { get; set; }
        public int FKiAppID { get; set; }
        public string AuditType { get; set; }
        public string sLastUpdate { get; set; }
        public string sTraceAttribute { get; set;/*{ return "iStatus"; }*/ }
        public Guid XIGUID { get; set; }
        public int iLevel { get; set; }
        public int iOrder { get; set; }
        XIInfraCache oCache = new XIInfraCache();
        public bool bIsEncrypt { get; set; }
        public string sBroadcastGroup { get; set; }
        public bool bOrgObject { get; set; }
        public int iOrgObject { get; set; }
        public string sTag { get; set; }
        public int XIVersion { get; set; }
        public Guid FKiTagSpaceIDXIGUID { get; set; }
        public XIDBOUI BOUID
        {
            get
            {
                return oMyBOUI;
            }
            set
            {
                oMyBOUI = value;
            }
        }

        public XIDBODefault Default {  get; set; }

        private XIDataSource oMyDataSource;
        public XIDataSource XIDataSource
        {
            get
            {
                return oMyDataSource;
            }
            set
            {
                oMyDataSource = value;
            }
        }

        private Dictionary<string, XIDAttribute> oMyAttributes = new Dictionary<string, XIDAttribute>(StringComparer.CurrentCultureIgnoreCase);
        public Dictionary<string, XIDAttribute> Attributes
        {
            get
            {
                return oMyAttributes;
            }
            set
            {
                oMyAttributes = value;
            }
        }

        private Dictionary<string, XIDBOAction> oMyActions = new Dictionary<string, XIDBOAction>(StringComparer.CurrentCultureIgnoreCase);
        public Dictionary<string, XIDBOAction> Actions
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

        public XIDAttribute AttributeD(string sAttributeName)
        {
            XIDAttribute oThisAttrD = null;
            sAttributeName = sAttributeName.ToLower();
            if (oMyAttributes.ContainsKey(sAttributeName))
            {
                oThisAttrD = oMyAttributes.Where(f => f.Key == sAttributeName).FirstOrDefault().Value;
            }
            else
            {
                oThisAttrD = new XIDAttribute();
            }

            return oThisAttrD;
        }

        private Dictionary<string, XIDGroup> oMyGroups = new Dictionary<string, XIDGroup>(StringComparer.CurrentCultureIgnoreCase);
        public Dictionary<string, XIDGroup> Groups
        {
            get
            {
                return oMyGroups;
            }
            set
            {
                oMyGroups = value;
            }
        }

        public XIDGroup GroupD(string sGroupName)
        {
            XIDGroup oThisGroupD = null/* TODO Change to default(_) if this is not a reference type */;


            sGroupName = sGroupName.ToLower();


            if (oMyGroups.ContainsKey(sGroupName))
            {
                oThisGroupD = oMyGroups[sGroupName];
            }
            else if (oMyGroups.ContainsKey(sGroupName.ToLower()))
            {
                oThisGroupD = oMyGroups[sGroupName];
            }
            else
            {
                oThisGroupD = new XIDGroup();
            }

            return oThisGroupD;
        }
        public List<XIDScript> sScripts { get; set; }
        private Dictionary<string, XIDScript> oMyScripts = new Dictionary<string, XIDScript>();
        public Dictionary<string, XIDScript> Scripts
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

        public XIDScript ScriptD(string sScriptName)
        {
            XIDScript oThisScriptD = null/* TODO Change to default(_) if this is not a reference type */;


            sScriptName = sScriptName.ToLower();


            if (oMyAttributes.ContainsKey(sScriptName))
            {
            }
            else
            {
            }

            return oThisScriptD;
        }

        private Dictionary<string, XIDStructure> oMyStructure = new Dictionary<string, XIDStructure>();
        public Dictionary<string, XIDStructure> Structures
        {
            get
            {
                return oMyStructure;
            }
            set
            {
                oMyStructure = value;
            }
        }

        private Dictionary<string, XIContentEditors> oMyTemplates = new Dictionary<string, XIContentEditors>();
        public Dictionary<string, XIContentEditors> Templates
        {
            get
            {
                return oMyTemplates;
            }
            set
            {
                oMyTemplates = value;
            }
        }


        public XIDStructure StructureD(string sStructureName)
        {
            XIDStructure oThisStructureD = null/* TODO Change to default(_) if this is not a reference type */;


            sStructureName = sStructureName.ToLower();


            if (oMyAttributes.ContainsKey(sStructureName))
            {
            }
            else
            {
            }

            return oThisStructureD;
        }

        public string Get_BOAttributeValue(string sBOName, long iBOIID)
        {
            string sBONameAttr = string.Empty;
            List<string[]> Rows = new List<string[]>();
            if (!string.IsNullOrEmpty(sNameAttribute))
            {
                XIDXI oXID = new XIDXI();
                string sBODataSource = oXID.GetBODataSource(iDataSourceXIGUID.ToString(),FKiApplicationID);
                var Con = new XIDBAPI(sBODataSource);
                Dictionary<string, object> UserParams = new Dictionary<string, object>();
                UserParams[sPrimaryKey] = iBOIID;
                sBONameAttr = Con.SelectString(sNameAttribute, TableName, UserParams).ToString();
            }
            return sBONameAttr;
        }
    }
}