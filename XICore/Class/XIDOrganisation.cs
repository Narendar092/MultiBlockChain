using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XICore
{
    [Table("Organizations")]
    public class XIDOrganisation
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public int TypeID { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public int NoOfUsers { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PostCode { get; set; }
        public string Website { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public int StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public String CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ModifiedByID { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string DatabaseType { get; set; }
        public int ThemeID { get; set; }
        public int FKiApplicationID { get; set; }
        public bool bNannoApp { get; set; }
        public string sShortName { get; set; }
    }
}