using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using XIDatabase;

namespace XICore
{
    [Table("XITeams_T")]
    public class XIInfraTeams : XIDefinitionBase
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public string sCode { get; set; }
        public string sDescription { get; set; }
        public int iStatus { get; set; }
        public int iType { get; set; }
        public string sHierarchy { get; set; }
        public List<XIDropDown> GetTeamsList()
        {
            List<XIDropDown> TeamsList = new List<XIDropDown>();
            cConnectionString oConString = new cConnectionString();
            string sCoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreDataBase"];
            string sConString = oConString.ConnectionString(sCoreDatabase);
            XIDBAPI Connection = new XIDBAPI(sConString);
            var data = Connection.Select<XIInfraTeams>("XITeams_T", new Dictionary<string, object>(), "id,scode").ToList();
            TeamsList = data.Where(m => 1 == 1).ToList().Select(m => new XIDropDown { Value = m.ID, text = m.sCode }).ToList();
            return TeamsList;
        }
    }
}