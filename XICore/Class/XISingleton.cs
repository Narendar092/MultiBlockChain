using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;

namespace XICore
{
    public sealed class Singleton
    {
        private string sMyGUID;
        public string sGUID
        {
            get
            {
                return sMyGUID;
            }
            set
            {
                sMyGUID = value;
            }
        }

        private string sMyUserID;
        public string sUserID
        {
            get
            {
                return sMyUserID;
            }
            set
            {
                sMyUserID = value;
            }
        }

        private string sMyOrgID;
        public string sOrgID
        {
            get
            {
                return sMyOrgID;
            }
            set
            {
                sMyOrgID = value;
            }
        }

        private string sMyCoreDatabase;
        public string sCoreDatabase
        {
            get
            {
                return sMyCoreDatabase;
            }
            set
            {
                sMyCoreDatabase = value;
            }
        }

        private string sMyAppName;
        public string sAppName
        {
            get
            {
                return sMyAppName;
            }
            set
            {
                sMyAppName = value;
            }
        }

        private Dictionary<string, string> oMyParentGUID = new Dictionary<string, string>();
        public Dictionary<string, string> oParentGUID
        {
            get
            {
                return oMyParentGUID;
            }
            set
            {
                oMyParentGUID = value;
            }
        }        

        private Dictionary<string, string> oMyActiveGUID = new Dictionary<string, string>();
        public Dictionary<string, string> sActiveGUID
        {
            get
            {
                return oMyActiveGUID;
            }
            set
            {
                oMyActiveGUID = value;
            }
        }

        private static readonly Lazy<Singleton> lazy =
        new Lazy<Singleton>(() => new Singleton());

        public static Singleton Instance { get { return lazy.Value; } }

        private Singleton()
        {
        }

        public void GetGuid()
        {
            sGUID = Guid.NewGuid().ToString();
        }
        private string sMyActiveMenu;
        public string ActiveMenu
        {
            get
            {
                return sMyActiveMenu;
            }
            set
            {
                sMyActiveMenu = value;
            }
        }

    }
}