using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDDistribute
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public int iStatus { get; set; }
        public string sStatusAttribute { get; set; }
        public string iSuccessStatus { get; set; }
        public string iNonAssignedStatus { get; set; }
        public int FKiBODID { get; set; }
        public int iType { get; set; }
        public Guid FKiBODIDXIGUID { get; set; }
        public int FKiOrgID { get; set; }
        private List<XIDDistributeLine> oMyDistributeLines = new List<XIDDistributeLine>();

        public List<XIDDistributeLine> DistributeLines
        {
            get
            {
                return oMyDistributeLines;
            }
            set
            {
                oMyDistributeLines = value;
            }
        }
    }

    public class XIDDistributeLine
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public int iStatus { get; set; }
        public int FKiDistributeID { get; set; }
        public Guid FKiDistributeIDXIGUID { get; set; }
        public int FKi1ClickID { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
        public int FKiProcessControllerID { get; set; }
        public Guid FKiProcessControllerIDXIGUID { get; set; }
        public float XIfOrder { get; set; }
        public int iOverrideStauts { get; set; }
        public int iUserID { get; set; }
        public int iTeamID { get; set; }
        public int FKiRuleID { get; set; }
        public Guid FKiRuleIDXIGUID { get; set; }
        public int FKiParameterID { get; set; }
        public Guid FKiParameterIDXIGUID { get; set; }
    }
}