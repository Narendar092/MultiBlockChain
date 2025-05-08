using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDMasterData
    {
        public int ID { get; set; }
        public int Code { get; set; }
        public int TypeID { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public string Expression { get; set; }
        public string Icon { get; set; }
        public int Status { get; set; }
        public string FileName { get; set; }
    }
}

