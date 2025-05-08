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
namespace XICore
{
    public class XIIStructure : XIInstanceBase
    {
        private Dictionary<string, XIIStructure> oMyStructNodes = new Dictionary<string, XIIStructure>();

        public Dictionary<string, XIIStructure> StructNodes
        {
            get
            {
                return oMyStructNodes;
            }
            set
            {
                oMyStructNodes = value;
            }
        }
        public XIIStructure StructureI(string sValueName)
        {
            XIIStructure oThisValueI = null;



            sValueName = sValueName.ToLower();

            if (sValueName == "")
            {
            }


            if (oMyStructNodes.ContainsKey(sValueName))
            {
            }
            else
            {
            }

            return oThisValueI;
        }
    }
}