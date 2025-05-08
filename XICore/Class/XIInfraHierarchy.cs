using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using XICore;
using XISystem;
using XIDatabase;

namespace XICore
{
    [Table("XIHierarchy_T")]
    public class XIInfraHierarchy : XIDefinitionBase
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public string sCode { get; set; }
        public string sDescription { get; set; }
        public int iStatus { get; set; }
        public int iType { get; set; }
        public string sHierarchy { get; set; }
        public List<XIDropDown> GetHierarchiesList()
        {
            List<XIDropDown> HierarchiesList = new List<XIDropDown>();
            cConnectionString oConString = new cConnectionString();
            string sCoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreDataBase"];
            string sConString = oConString.ConnectionString(sCoreDatabase);
            XIDBAPI Connection = new XIDBAPI(sConString);
            Dictionary<string, object> ListParams = new Dictionary<string, object>();
            ListParams["izXDeleted"] = "0";
            var data = Connection.Select<XIInfraHierarchy>("XIHierarchy_T", ListParams, "id,scode").ToList();
            HierarchiesList = data.Where(m => 1 == 1).ToList().Select(m => new XIDropDown { Value = m.ID, text = m.sCode }).ToList();
            return HierarchiesList;
        }
    }
}