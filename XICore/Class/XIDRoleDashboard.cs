using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICore
{
    public class XIDRoleDashboard
    {
        public int ID { get; set; }
        public string XIGUID { get; set; }
        public int FKiRoleID { get; set; }
        public string FKiLayoutIDXIGUID { get; set; }

    }
}
