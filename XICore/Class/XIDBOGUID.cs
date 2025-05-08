using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDBOGUID
    {
        public string sTableName { get; set; }
        public bool bIsChangeFK { get; set; }

        public List<XIDropDown> ddlBOs { get; set; }
    }
}