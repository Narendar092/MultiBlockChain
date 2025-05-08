using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XICore
{
    public class XIParameter : XIDefinitionBase
    {
        [Key]
        public int XiParameterID { get; set; }

        public Guid XIGUID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public int OneClickID { get; set; }
        public int FKiApplicationID { get; set; }
        [ForeignKey("XiParameterID")]
        public virtual List<XiParameterNVs> XiParameterNVs { get; set; }
        public virtual List<XiParameterLists> XiParameterLists { get; set; }
        [NotMapped]
        public List<XIDropDown> XiParametersDDLs { get; set; }
        public List<XIDropDown> ddlApplications { get; set; }
    }

    public class XiParameterNVs
    {
        [Key]
        public int ID { get; set; }
        public int XiParameterID { get; set; }
        public int XiParameterListID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string sContext { get; set; }
        public string sTag { get; set; }
    }

    public class XiParameterLists
    {
        [Key]
        public int XiParameterListID { get; set; }
        public int XiParameterID { get; set; }
        public string ListName { get; set; }
        [ForeignKey("XiParameterListID")]
        public virtual List<XiParameterNVs> XiParameterListNVs { get; set; }
    }
}