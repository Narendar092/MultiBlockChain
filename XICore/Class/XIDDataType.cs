using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDDataType
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please Enter Name")]
        [StringLength(32, ErrorMessage = "Name can be no larger than 32 characters")]
        public string sName { get; set; }
        [Required(ErrorMessage = "Please Select BaseDataType")]
        public string sBaseDataType { get; set; }
        public int FKiOrganisationID { get; set; }
        public string sStartRange { get; set; }
        public string sEndRange { get; set; }
        public string sRegex { get; set; }
        public string sScript { get; set; }
        public string sValidationMessage { get; set; }
        public int FKiApplicationID { get; set; }
        public int StatusTypeID { get; set; }
        public List<XIDropDown> ddlApplications { get; set; }
    }
}