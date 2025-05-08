using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICore
{
    public class XIDRule
    {
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public string sRuleName { get; set; }
        public int BOID { get; set; }
        public Guid BOIDXIGUID { get; set; }
        public string sDescription { get; set; }
        public string sCondition { get; set; }
        public int iStatus { get; set; }
        public int iMode { get; set; }
        public string sDefaultNode { get; set; }
        public int FKiQSInstanceID { get; set; }

        private List<XIDRuleGroup> oMyRuleGroups = new List<XIDRuleGroup>();
        public List<XIDRuleGroup> RuleGroups
        {
            get
            {
                return oMyRuleGroups;
            }
            set
            {
                oMyRuleGroups = value;
            }
        }
    }

    public class XIDRuleGroup
    {
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public int FKiRuleID { get; set; }
        public Guid FKiRuleIDXIGUID { get; set; }
        public int iGroupID { get; set; }
        public int iRuleNo { get; set; }
        public string sRuleOperator { get; set; }
        public int BOAttributeID { get; set; }
        public int FKiQSInstanceID { get; set; }
        public string sOperator { get; set; }
        public string sValue { get; set; }
        public int BOID { get; set; }
        public string FKiOperator { get; set; }
        public string FKiWhereValue { get; set; }
        public int iParentRootID { get; set; }
    }
}
