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
using XISystem;
using System.Web;
using XIDatabase;
using System.Configuration;
using System.Data.SqlClient;

namespace XICore
{
    public class XIDPartialQS : XIDefinitionBase
    {
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public int FKiMainQSID { get; set; }
        public int FKiPartialQSID { get; set; }
        public int FkiStepID { get; set; }
        public decimal iOrderGap { get; set; }

        private Dictionary<string, XIDQS> oMyQSD = new Dictionary<string, XIDQS>();
        public Dictionary<string, XIDQS> oQSD
        {
            get
            {
                return oMyQSD;
            }
            set
            {
                oMyQSD = value;
            }
        }
    }
}