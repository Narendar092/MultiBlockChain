using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIQSScript
    {
        public long ID { get; set; }
        public long FKiScriptID { get; set; }
        public Guid FKiScriptIDXIGUID { get; set; }
        public int FKiQSDefinitionID { get; set; }
        public Guid FKiQSDefinitionIDXIGUID { get; set; }
        public int FKiStepDefinitionID { get; set; }
        public Guid FKiStepDefinitionIDXIGUID { get; set; }
        public int FKiSectionDefinitionID { get; set; }
        public Guid FKiSectionDefinitionIDXIGUID { get; set; }
    }
}