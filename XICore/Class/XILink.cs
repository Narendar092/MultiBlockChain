using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using XISystem;
using XIDatabase;
//using XIAlgorithm;

namespace XICore
{
    [Table("XILink_T")]
    public class XILink : XIDefinitionBase
    {
        [Key]
        public int XiLinkID { get; set; }
        public Guid XIGUID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public int OneClickID { get; set; }
        public int FKiComponentID { get; set; }
        public Guid FKiComponentIDXIGUID { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        public string sType { get; set; }
        public virtual List<XiLinkNV> XiLinkNVs { get; set; }
        public virtual List<XiLinkList> XiLinkLists { get; set; }
        public List<XIDropDown> XiLinkDDLs { get; set; }
        public List<XIDropDown> XiComponentsDDLs { get; set; }
        public string sActive { get; set; }
        public string sScriptNotation { get; set; }
        //public XIIBO BOInstance { get; set; }
        public int OrganisationID { get; set; }
        public bool bMatrix { get; set; }
        public string sCode { get; set; }
        public Guid BOIDXIGUID { get; set; }
        public int BOID { get; set; }
        // XIDBO oBOD = new XIDBO();
        //public xiElement sActive
        //{
        //    get
        //    {
        //        return Execute_Script();
        //    }
        //    set
        //    {

        //    }
        //}

        //My Code
        [NotMapped]
        public List<XIDropDown> ddlXIComponents { get; set; }
        [NotMapped]
        public List<XIDropDown> ddlApplications { get; set; }
        [NotMapped]
        public int iUserID { get; set; }
        [NotMapped]
        public string sNewGUID { get; set; }
        [NotMapped]
        public string sOrgName { get; set; }
        public CResult oCResult = new CResult();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        //My Code
        public XILink Get_XILinkDetails()
        {
            XILink oXILink = new XILink();
            try
            {
                if (this.oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {

                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    if (XiLinkID != 0)
                    {
                        Params["XiLinkID"] = XiLinkID;
                        //add later
                        //Result.FKiApplicationID = fkiApplicationID;
                        oXILink = Connection.Select<XILink>("XILink_T", Params).FirstOrDefault();

                        List<XiLinkNV> oXILinkNV = new List<XiLinkNV>();
                        oXILinkNV = Connection.Select<XiLinkNV>("XILinkNV_T", Params).ToList();

                        List<XiLinkList> oXILinkLists = new List<XiLinkList>();
                        oXILinkLists = Connection.Select<XiLinkList>("XILinkList_T", Params).ToList();
                        oXILink.XiLinkNVs = oXILinkNV.Where(x => x.XiLinkListID == 0).Select(m => new XiLinkNV { XiLinkID = m.XiLinkID, Name = m.Name, Value = m.Value }).ToList();
                        oXILink.XiLinkLists = oXILinkLists.Select(m => new XiLinkList { XiLinkID = m.XiLinkID, XiLinkListID = m.XiLinkListID, ListName = m.ListName }).ToList();
                    }
                    //components
                    List<XIDropDown> XIComponents = new List<XIDropDown>();
                    Dictionary<string, object> XIComponentParams = new Dictionary<string, object>();
                    XIComponentParams["StatusTypeID"] = 10;
                    List<XIDropDown> oXICompDD = new List<XIDropDown>();
                    oXICompDD = Connection.Select<XIDComponent>("XIComponents_XC_T", XIComponentParams).ToList().Select(m => new XIDropDown { Value = m.ID, text = m.sName }).ToList();
                    oXILink.ddlXIComponents = oXICompDD;
                    //Application
                    //if (fkiApplicationID == 0)
                    //{
                    List<XIDropDown> oXIApplications = new List<XIDropDown>();
                    Dictionary<string, object> XIAppParams = new Dictionary<string, object>();
                    oXIApplications = Connection.Select<XIDApplication>("XIApplication_T", XIAppParams).ToList().Select(m => new XIDropDown { Value = m.ID, text = m.sApplicationName }).ToList();
                    oXILink.ddlApplications = oXIApplications;
                    if (XiLinkID == 0)
                    {
                        oXILink.FKiApplicationID = FKiApplicationID;
                    }
                    else
                    {
                        oXILink.FKiApplicationID = oXILink.FKiApplicationID;
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oXILink;
        }

        private xiElement Execute_Script()
        {
            CScriptController oXIScript = new CScriptController();
            CResult oCR = oXIScript.API2_Serialise_From_String(sScriptNotation);
            oCR = oXIScript.API2_ExecuteMyOM();
            xiElement oElement = new xiElement();
            oElement.sValue = (string)oCR.oResult;
            //CCompiler oCompiler = new CCompiler();
            //var Result = oCompiler.Compile_FromText(sScriptNotation);
            return oElement;
        }
    }

    [Table("XILinkNV_T")]
    public class XiLinkNV
    {
        [Key]
        public int ID { get; set; }
        public int XiLinkID { get; set; }
        public int XiLinkListID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Guid XiLinkIDXIGUID { get; set; }
        public string sType { get; set; }
        public Guid BOIDXIGUID { get; set; }
        public int BOID { get; set; }
    }

    [Table("XILinkList_T")]
    public class XiLinkList
    {
        [Key]
        public int XiLinkListID { get; set; }
        public int XiLinkID { get; set; }
        public string ListName { get; set; }
        [NotMapped]
        public virtual List<XiLinkNV> XiLinkListNVs { get; set; }
    }

    public class xiElement
    {
        public string sValue { get; set; }
    }
}