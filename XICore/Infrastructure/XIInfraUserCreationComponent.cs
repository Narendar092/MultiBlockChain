using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIInfraUserCreationComponent
    {
        XIInfraCache oCache = new XIInfraCache();

        public static List<T> GetMappedBOList<T>(Dictionary<string, XIIBO> o1ClickRes) where T: class, new()
        {
            List<T> oList = o1ClickRes.Values.Select(v => v.Attributes).ToList().Select(x => x.Values).ToList()
                .Select(x =>
                {
                    var z = x.Select(y => { var sName = y.sName; var sValue = y.sValue; Dictionary<string, string> oDict = new Dictionary<string, string>(); oDict[sName] = sValue; return oDict; }).ToList();
                    Dictionary<string, string> oDictObj = new Dictionary<string, string>();
                    z = z.Select(p => { oDictObj[p.Keys.FirstOrDefault()] = p[p.Keys.FirstOrDefault()]; return p; }).ToList();
                    return ToObject<T>(oDictObj);
                }).ToList();
            return oList;
        }

        public static T ToObject<T>(IDictionary<string, string> source) where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                Type type = someObjectType.GetProperty(item.Key)?.PropertyType;
                if (type != null)
                {
                    someObjectType
                             .GetProperty(item.Key)
                             .SetValue(someObject, GetParsedValue(item.Value, type), null);
                }
            }

            return someObject;
        }

        public static dynamic GetParsedValue(string inputValue, Type type)
        {
            if(type == typeof(int))
            {
                return inputValue == String.Empty ? -1 : int.Parse(inputValue);
            }
            else if(type == typeof(int?))
            {
                return inputValue == String.Empty ? (int?)null : int.Parse(inputValue);
            }
            else if(type == typeof(bool))
            {
                return inputValue == String.Empty ? false : bool.Parse(inputValue);
            }
            else if (type == typeof(bool?))
            {
                return inputValue == String.Empty ? (bool?)null : bool.Parse(inputValue);
            }
            else if(type == typeof(Guid))
            {
                return inputValue == String.Empty ? Guid.Empty : Guid.Parse(inputValue);
            }
            else if (type == typeof(Guid?))
            {
                return inputValue == String.Empty ? (Guid?)null : Guid.Parse(inputValue);
            }
            else
            {
                return inputValue;
            }
        }

        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                var sCoreDB = oParams.Where(m => m.sName.ToLower() == "sCoreDatabase".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sUserID = oParams.Where(m => m.sName.ToLower() == "iUserID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iUserID = 0;
                int.TryParse(sUserID, out iUserID);
                var sOrgID = oParams.Where(m => m.sName.ToLower() == "iorgid").Select(m => m.sValue).FirstOrDefault();
                int iOrgID = 0;
                int.TryParse(sOrgID, out iOrgID);               
                oTrace.oParams.Add(new CNV { sName = "iUserID", sValue = iUserID.ToString() });
                Dictionary<string, object> Data = new Dictionary<string, object>();
                XIInfraRoles oRole = new XIInfraRoles();
                XID1Click o1Click = new XID1Click();
                o1Click = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, "DC28355D-337C-4CA6-9EA7-2C76AD074396");
                Dictionary<string, XIIBO> oRes = new Dictionary<string, XIIBO>();
                oRes = o1Click.OneClick_Execute(null, o1Click); //Use OneClick_TableResult() function instead to prevent multiple conversions (TODO).
                List<XIInfraRoles> oRoles =  GetMappedBOList<XIInfraRoles>(oRes);
                var Roles = oRole.CreateHierarchy(oRoles.Where(r => r.iParentIDXIGUID.ToString() == Guid.Empty.ToString()).ToList(), oRoles);
                Data["Roles"] = Roles;
                if (iUserID > 0)//check mandatory params are passed or not
                {
                    XIInfraUsers xifuser = new XIInfraUsers();
                    xifuser.UserID = iUserID;
                    xifuser = (XIInfraUsers)xifuser.Get_UserDetails(sCoreDB).oResult;
                    Data["User"] = xifuser;
                }
                else
                {
                    
                }
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = Data;
                oCResult.xiStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                //else
                //{
                //    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                //    oTrace.sMessage = "Mandatory Param:  is missing";
                //}
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}