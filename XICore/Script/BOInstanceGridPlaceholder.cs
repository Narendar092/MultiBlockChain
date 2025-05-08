using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class BOInstanceGridPlaceholder
    {

        private Dictionary<string, XIIBO> oMyBOInstances = new Dictionary<string, XIIBO>();

        public Dictionary<string, XIIBO> oBOInstances
        {
            get
            {
                return oMyBOInstances;
            }
            set
            {
                oMyBOInstances = value;
            }
        }

        private string sMyName;

        public string sName
        {
            get
            {
                return sMyName;
            }
            set
            {
                sMyName = value;
            }
        }
    }
}