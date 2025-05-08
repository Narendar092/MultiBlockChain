using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace XICore
{
    //internal interface XIInfraUserPreference
    //{
    //}
    [Table("XIUserPreference_T")]

    public class XIInfraUserPreference : XIDefinitionBase
    {
        [Key]
        public int IconAssist { get; set; }

        public int InlineAssist { get; set; }

        public int FKiUserID { get; set; }
        public int ID { get; set; }

        [NotMapped]
        private XIInfraUserPreference oMyUserPreference;
        [NotMapped]
        public XIInfraUserPreference UserPreference
        {
            get
            {
                return oMyUserPreference;
            }
            set
            {
                oMyUserPreference = value;
            }
        }


    }

    
}
