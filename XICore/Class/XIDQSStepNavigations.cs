using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDQSStepNavigations
    {
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public int FKiStepDefinitionID { get; set; }
        public string sName { get; set; }
        public int iType { get; set; }
        public int iNextStepID { get; set; }
        public decimal iOrder { get; set; }
        public string sFunction { get; set; }
        public string sField { get; set; }
        public string sOperator { get; set; }
        public string sValue { get; set; }
    }
}