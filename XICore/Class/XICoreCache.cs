using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using XISystem;

namespace XICore
{
    public class XICoreCache
    {
        public string sSessionID { get; set; }
        //Old Method to load objects from cache
        //Got Type A and Type B reference Error. Type A originates from Temp folder and Type B originates from D:\\TFsProjects bin folder
        public object GetObjectFromCacheN(string ObjType = "", string ObjName = "", string ObjID = "")
        {
            //Creating Instance Late Binding
            Assembly exceutable;
            Type Ltype;
            object objclass;
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\DLL";
            //var DLL = Assembly.LoadFile(sPath + "\\XIInfrastructure.dll");
            var DLL = Assembly.LoadFile("D:\\tfs projects\\XIDNAOM\\XIDNA\\XIInfrastructure\\bin\\XIInfrastructure.dll");

            //foreach (Type type in DLL.GetExportedTypes())
            //{
            //    dynamic c = Activator.CreateInstance(type);
            //    c.Output(@"Hello");
            //}

            Ltype = DLL.GetType("XIInfrastructure.XIInfraCacheAPI");
            exceutable = Assembly.GetExecutingAssembly();
            //Ltype = exceutable.GetType("XIInfrastructure.XIInfraCacheAPI");
            objclass = Activator.CreateInstance(Ltype);
            MethodInfo method = Ltype.GetMethod("GetObjectFromCache");
            object[] parametersArray = new object[] { ObjType, ObjName, ObjID };
            object Response = (object)method.Invoke(objclass, parametersArray);
            return Response;
        }

        //Old method currently not using
        public void SetXIParamsN(List<CNV> oParams, string sGUID)
        {
            //Creating Instance Late Binding
            Assembly exceutable;
            Type Ltype;
            object objclass;
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\DLL";
            //var DLL = Assembly.LoadFile(sPath + "\\XIInfrastructure.dll");
            var DLL = Assembly.LoadFile("D:\\tfs projects\\XIDNAOM\\XIDNA\\XIInfrastructure\\bin\\XIInfrastructure.dll");
            //foreach (Type type in DLL.GetExportedTypes())
            //{
            //    dynamic c = Activator.CreateInstance(type);
            //    c.Output(@"Hello");
            //}

            Ltype = DLL.GetType("XIInfrastructure.XIInfraCacheAPI");
            exceutable = Assembly.GetExecutingAssembly();
            //Ltype = exceutable.GetType("XIInfrastructure.XIInfraCacheAPI");
            objclass = Activator.CreateInstance(Ltype);
            MethodInfo method = Ltype.GetMethod("SetXIParams");
            object[] parametersArray = new object[] { oParams, sGUID };
            object Response = (object)method.Invoke(objclass, parametersArray);            
        }

        public object GetObjectFromCache(string ObjType = "", string ObjName = "", string ObjID = "")
        {            
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin\\XIInfrastructure.dll";
            //var DLL = Assembly.LoadFile(sPath + "\\XIInfrastructure.dll");
            string StrPath = sPath; //"D:\\TfsProjects\\XIDNAOM\\XIDNA\\XIInfrastructure\\bin\\XIInfrastructure.dll";
            //Creating a new appdomain
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain
            System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(LoadMyAssembly).Assembly.FullName, typeof(LoadMyAssembly).FullName);
            LoadMyAssembly loader = (LoadMyAssembly)obj.Unwrap();//As the object we are creating is from another appdomain hence we will get that object in wrapped format and hence in next step we have unwrappped it
            //Call loadassembly method so that the assembly will be loaded into the new appdomain amd the object will also remain in new appdomain only.

            loader.LoadAssembly(StrPath);
            //Call exceuteMethod and pass the name of the method from assembly and the parameters.
            Assembly _assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath));
            Type Ltype = null;
            object objclass = null;
            foreach (System.Type type in _assembly.GetTypes())
            {
                if (String.Compare(type.Name, "XIInfraCacheAPI", true) == 0)
                {
                    Ltype = type;
                    objclass = _assembly.CreateInstance(type.FullName);
                    break;
                }
            }
            MethodInfo MyMethod = Ltype.GetMethod("GetObjectFromCache");
            object[] parametersArray = new object[] { ObjType, ObjName, ObjID };
            object Response = (object)MyMethod.Invoke(objclass, parametersArray);

            //object Response = loader.ExecuteStaticMethod("GetObjectFromCache", new object[] { sSessionID, ObjType, ObjName, ObjID });
            AppDomain.Unload(newDomain); //After the method has been executed call unload method of the appdomain.
            return Response;
            //Wow you have unloaded the new appdomain and also unloaded the loaded assembly from memory.
        }

        public object SetXIParams(List<CNV> oParams, string sGUID, string sSessionID)
        {
            //Creating a new appdomain
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin\\XIInfrastructure.dll";
            //var DLL = Assembly.LoadFile(sPath + "\\XIInfrastructure.dll");
            string StrPath = sPath; //"D:\\TfsProjects\\XIDNAOM\\XIDNA\\XIInfrastructure\\bin\\XIInfrastructure.dll";
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain
            System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(LoadMyAssembly).Assembly.FullName, typeof(LoadMyAssembly).FullName);
            LoadMyAssembly loader = (LoadMyAssembly)obj.Unwrap();//As the object we are creating is from another appdomain hence we will get that object in wrapped format and hence in next step we have unwrappped it
            //Call loadassembly method so that the assembly will be loaded into the new appdomain amd the object will also remain in new appdomain only.

            loader.LoadAssembly(StrPath);
            //Call exceuteMethod and pass the name of the method from assembly and the parameters.
            Assembly _assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath));
            Type Ltype =null ;
            object objclass = null;
            foreach (System.Type type in _assembly.GetTypes())
            {
                if (String.Compare(type.Name, "XIInfraCacheAPI", true) == 0)
                {
                    Ltype = type;
                    objclass = _assembly.CreateInstance(type.FullName);
                    break;
                }
            }
            MethodInfo MyMethod = Ltype.GetMethod("SetXIParams");
            object[] parametersArray = new object[] { oParams, sGUID, sSessionID };
            object Response = (object)MyMethod.Invoke(objclass, parametersArray);

            //object Response = loader.ExecuteStaticMethod("GetObjectFromCache", new object[] { sSessionID, ObjType, ObjName, ObjID });
            AppDomain.Unload(newDomain); //After the method has been executed call unload method of the appdomain.
            return Response;
            //Wow you have unloaded the new appdomain and also unloaded the loaded assembly from memory.
        }

        public object Get_ParamVal(string sParameter, string sGUID, string sSessionID)
        {
            //string sSessionID = HttpContext.Current.Session.SessionID;
            //Creating a new appdomain
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin\\XIInfrastructure.dll";
            //var DLL = Assembly.LoadFile(sPath + "\\XIInfrastructure.dll");
            string StrPath = sPath; //"D:\\TfsProjects\\XIDNAOM\\XIDNA\\XIInfrastructure\\bin\\XIInfrastructure.dll";
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain
            System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(LoadMyAssembly).Assembly.FullName, typeof(LoadMyAssembly).FullName);
            LoadMyAssembly loader = (LoadMyAssembly)obj.Unwrap();//As the object we are creating is from another appdomain hence we will get that object in wrapped format and hence in next step we have unwrappped it
            //Call loadassembly method so that the assembly will be loaded into the new appdomain amd the object will also remain in new appdomain only.

            loader.LoadAssembly(StrPath);
            //Call exceuteMethod and pass the name of the method from assembly and the parameters.
            Assembly _assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath));
            Type Ltype = null;
            object objclass = null;
            foreach (System.Type type in _assembly.GetTypes())
            {
                if (String.Compare(type.Name, "XIInfraCacheAPI", true) == 0)
                {
                    Ltype = type;
                    objclass = _assembly.CreateInstance(type.FullName);
                    break;
                }
            }
            MethodInfo MyMethod = Ltype.GetMethod("Get_ParamVal");
            object[] parametersArray = new object[] { sSessionID, sGUID, null, sParameter};
            object Response = (object)MyMethod.Invoke(objclass, parametersArray);

            //object Response = loader.ExecuteStaticMethod("GetObjectFromCache", new object[] { sSessionID, ObjType, ObjName, ObjID });
            AppDomain.Unload(newDomain); //After the method has been executed call unload method of the appdomain.
            return Response;
            //Wow you have unloaded the new appdomain and also unloaded the loaded assembly from memory.
        }

        public object InsertIntoCache(object oCacheObj, string sCacheKey)
        {
            //Creating a new appdomain
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin\\XIInfrastructure.dll";
            //var DLL = Assembly.LoadFile(sPath + "\\XIInfrastructure.dll");
            string StrPath = sPath; //"D:\\TfsProjects\\XIDNAOM\\XIDNA\\XIInfrastructure\\bin\\XIInfrastructure.dll";
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain
            System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(LoadMyAssembly).Assembly.FullName, typeof(LoadMyAssembly).FullName);
            LoadMyAssembly loader = (LoadMyAssembly)obj.Unwrap();//As the object we are creating is from another appdomain hence we will get that object in wrapped format and hence in next step we have unwrappped it
            //Call loadassembly method so that the assembly will be loaded into the new appdomain amd the object will also remain in new appdomain only.

            loader.LoadAssembly(StrPath);
            //Call exceuteMethod and pass the name of the method from assembly and the parameters.
            Assembly _assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath));
            Type Ltype = null;
            object objclass = null;
            foreach (System.Type type in _assembly.GetTypes())
            {
                if (String.Compare(type.Name, "XIInfraCacheAPI", true) == 0)
                {
                    Ltype = type;
                    objclass = _assembly.CreateInstance(type.FullName);
                    break;
                }
            }
            MethodInfo MyMethod = Ltype.GetMethod("InsertIntoCache");
            object[] parametersArray = new object[] { oCacheObj, sCacheKey };
            object Response = (object)MyMethod.Invoke(objclass, parametersArray);

            //object Response = loader.ExecuteStaticMethod("GetObjectFromCache", new object[] { sSessionID, ObjType, ObjName, ObjID });
            AppDomain.Unload(newDomain); //After the method has been executed call unload method of the appdomain.
            return Response;
            //Wow you have unloaded the new appdomain and also unloaded the loaded assembly from memory.
        }

        public object GetFromCache(string sCacheKey)
        {
            //string sSessionID = HttpContext.Current.Session.SessionID;
            //Creating a new appdomain
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin\\XIInfrastructure.dll";
            //var DLL = Assembly.LoadFile(sPath + "\\XIInfrastructure.dll");
            string StrPath = sPath; //"D:\\TfsProjects\\XIDNAOM\\XIDNA\\XIInfrastructure\\bin\\XIInfrastructure.dll";
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain
            System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(LoadMyAssembly).Assembly.FullName, typeof(LoadMyAssembly).FullName);
            LoadMyAssembly loader = (LoadMyAssembly)obj.Unwrap();//As the object we are creating is from another appdomain hence we will get that object in wrapped format and hence in next step we have unwrappped it
            //Call loadassembly method so that the assembly will be loaded into the new appdomain amd the object will also remain in new appdomain only.

            loader.LoadAssembly(StrPath);
            //Call exceuteMethod and pass the name of the method from assembly and the parameters.
            Assembly _assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath));
            Type Ltype = null;
            object objclass = null;
            foreach (System.Type type in _assembly.GetTypes())
            {
                if (String.Compare(type.Name, "XIInfraCacheAPI", true) == 0)
                {
                    Ltype = type;
                    objclass = _assembly.CreateInstance(type.FullName);
                    break;
                }
            }
            MethodInfo MyMethod = Ltype.GetMethod("GetFromCache");
            object[] parametersArray = new object[] { sCacheKey };
            object Response = (object)MyMethod.Invoke(objclass, parametersArray);

            //object Response = loader.ExecuteStaticMethod("GetObjectFromCache", new object[] { sSessionID, ObjType, ObjName, ObjID });
            AppDomain.Unload(newDomain); //After the method has been executed call unload method of the appdomain.
            return Response;
            //Wow you have unloaded the new appdomain and also unloaded the loaded assembly from memory.
        }

        public object GetQuestionSetInstanceFromCache(string sGUID = "", string ObjName = "", int ObjID = 0)
        {
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin\\XIInfrastructure.dll";
            //var DLL = Assembly.LoadFile(sPath + "\\XIInfrastructure.dll");
            string StrPath = sPath; //"D:\\TfsProjects\\XIDNAOM\\XIDNA\\XIInfrastructure\\bin\\XIInfrastructure.dll";
            //Creating a new appdomain
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain
            System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(LoadMyAssembly).Assembly.FullName, typeof(LoadMyAssembly).FullName);
            LoadMyAssembly loader = (LoadMyAssembly)obj.Unwrap();//As the object we are creating is from another appdomain hence we will get that object in wrapped format and hence in next step we have unwrappped it
            //Call loadassembly method so that the assembly will be loaded into the new appdomain amd the object will also remain in new appdomain only.

            loader.LoadAssembly(StrPath);
            //Call exceuteMethod and pass the name of the method from assembly and the parameters.
            Assembly _assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath));
            Type Ltype = null;
            object objclass = null;
            foreach (System.Type type in _assembly.GetTypes())
            {
                if (String.Compare(type.Name, "XIInfraCache", true) == 0)
                {
                    Ltype = type;
                    objclass = _assembly.CreateInstance(type.FullName);
                    break;
                }
            }
            MethodInfo MyMethod = Ltype.GetMethod("Get_QuestionSetCache");
            object[] parametersArray = new object[] { ObjName, sGUID, ObjID };
            object Response = (object)MyMethod.Invoke(objclass, parametersArray);

            //object Response = loader.ExecuteStaticMethod("GetObjectFromCache", new object[] { sSessionID, ObjType, ObjName, ObjID });
            AppDomain.Unload(newDomain); //After the method has been executed call unload method of the appdomain.
            return Response;
            //Wow you have unloaded the new appdomain and also unloaded the loaded assembly from memory.
        }

        public object MergeXILinkParameters(XILink oXiLink, string sSessionID, string sGUID = "", List<CNV> oParams = null)
        {
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin\\XIInfrastructure.dll";
            //var DLL = Assembly.LoadFile(sPath + "\\XIInfrastructure.dll");
            string StrPath = sPath; //"D:\\TfsProjects\\XIDNAOM\\XIDNA\\XIInfrastructure\\bin\\XIInfrastructure.dll";
            //Creating a new appdomain
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain
            System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(LoadMyAssembly).Assembly.FullName, typeof(LoadMyAssembly).FullName);
            LoadMyAssembly loader = (LoadMyAssembly)obj.Unwrap();//As the object we are creating is from another appdomain hence we will get that object in wrapped format and hence in next step we have unwrappped it
            //Call loadassembly method so that the assembly will be loaded into the new appdomain amd the object will also remain in new appdomain only.

            loader.LoadAssembly(StrPath);
            //Call exceuteMethod and pass the name of the method from assembly and the parameters.
            Assembly _assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath));
            Type Ltype = null;
            object objclass = null;
            foreach (System.Type type in _assembly.GetTypes())
            {
                if (String.Compare(type.Name, "XIInfraCache", true) == 0)
                {
                    Ltype = type;
                    objclass = _assembly.CreateInstance(type.FullName);
                    break;
                }
            }
            MethodInfo MyMethod = Ltype.GetMethod("MergeXILinkParameters");
            object[] parametersArray = new object[] { oXiLink, sGUID, oParams, sSessionID };
            object Response = (object)MyMethod.Invoke(objclass, parametersArray);

            //object Response = loader.ExecuteStaticMethod("GetObjectFromCache", new object[] { sSessionID, ObjType, ObjName, ObjID });
            AppDomain.Unload(newDomain); //After the method has been executed call unload method of the appdomain.
            return Response;
            //Wow you have unloaded the new appdomain and also unloaded the loaded assembly from memory.
        }

        public object AddParamsToGUID(int iParameterID = 0, string sGUID = "")
        {
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin\\XIInfrastructure.dll";
            //var DLL = Assembly.LoadFile(sPath + "\\XIInfrastructure.dll");
            string StrPath = sPath; //"D:\\TfsProjects\\XIDNAOM\\XIDNA\\XIInfrastructure\\bin\\XIInfrastructure.dll";
            //Creating a new appdomain
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain
            System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(LoadMyAssembly).Assembly.FullName, typeof(LoadMyAssembly).FullName);
            LoadMyAssembly loader = (LoadMyAssembly)obj.Unwrap();//As the object we are creating is from another appdomain hence we will get that object in wrapped format and hence in next step we have unwrappped it
            //Call loadassembly method so that the assembly will be loaded into the new appdomain amd the object will also remain in new appdomain only.

            loader.LoadAssembly(StrPath);
            //Call exceuteMethod and pass the name of the method from assembly and the parameters.
            Assembly _assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath));
            Type Ltype = null;
            object objclass = null;
            foreach (System.Type type in _assembly.GetTypes())
            {
                if (String.Compare(type.Name, "XIInfraCache", true) == 0)
                {
                    Ltype = type;
                    objclass = _assembly.CreateInstance(type.FullName);
                    break;
                }
            }
            MethodInfo MyMethod = Ltype.GetMethod("AddParamsToGUID");
            object[] parametersArray = new object[] { iParameterID, sGUID };
            object Response = (object)MyMethod.Invoke(objclass, parametersArray);

            //object Response = loader.ExecuteStaticMethod("GetObjectFromCache", new object[] { sSessionID, ObjType, ObjName, ObjID });
            AppDomain.Unload(newDomain); //After the method has been executed call unload method of the appdomain.
            return Response;
            //Wow you have unloaded the new appdomain and also unloaded the loaded assembly from memory.
        }

        public object ResolveParameters(List<CNV> Params, string sSessionID, string sGUID)
        {
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin\\XIInfrastructure.dll";
            //var DLL = Assembly.LoadFile(sPath + "\\XIInfrastructure.dll");
            string StrPath = sPath; //"D:\\TfsProjects\\XIDNAOM\\XIDNA\\XIInfrastructure\\bin\\XIInfrastructure.dll";
            //Creating a new appdomain
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain
            System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(LoadMyAssembly).Assembly.FullName, typeof(LoadMyAssembly).FullName);
            LoadMyAssembly loader = (LoadMyAssembly)obj.Unwrap();//As the object we are creating is from another appdomain hence we will get that object in wrapped format and hence in next step we have unwrappped it
            //Call loadassembly method so that the assembly will be loaded into the new appdomain amd the object will also remain in new appdomain only.

            loader.LoadAssembly(StrPath);
            //Call exceuteMethod and pass the name of the method from assembly and the parameters.
            Assembly _assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath));
            Type Ltype = null;
            object objclass = null;
            foreach (System.Type type in _assembly.GetTypes())
            {
                if (String.Compare(type.Name, "XIInfraCache", true) == 0)
                {
                    Ltype = type;
                    objclass = _assembly.CreateInstance(type.FullName);
                    break;
                }
            }
            MethodInfo MyMethod = Ltype.GetMethod("ResolveParameters");
            object[] parametersArray = new object[] { Params, sSessionID, sGUID };
            object Response = (object)MyMethod.Invoke(objclass, parametersArray);

            //object Response = loader.ExecuteStaticMethod("GetObjectFromCache", new object[] { sSessionID, ObjType, ObjName, ObjID });
            AppDomain.Unload(newDomain); //After the method has been executed call unload method of the appdomain.
            return Response;
            //Wow you have unloaded the new appdomain and also unloaded the loaded assembly from memory.
        }
    }


    //Currently not using
    public class LoadMyAssembly : MarshalByRefObject
    {
        object retunobject;
        private Assembly _assembly;
        System.Type MyType = null;
        object inst = null;
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void LoadAssembly(string path)
        {
            _assembly = Assembly.Load(AssemblyName.GetAssemblyName(path));
        }

        public object ExecuteStaticMethod(string methodName, params object[] parameters)
        {
            foreach (System.Type type in _assembly.GetTypes())
            {
                if (String.Compare(type.Name, "XIInfraCacheAPI", true) == 0)
                {
                    MyType = type;
                    inst = _assembly.CreateInstance(type.FullName);
                    break;
                }
            }
            MethodInfo MyMethod = MyType.GetMethod(methodName);
            object[] parametersArray = parameters;
            object Response = (object)MyMethod.Invoke(inst, parametersArray);

            retunobject = Response;
            //MethodInfo MyMethod = MyType.GetMethod(methodName, new Type[] { typeof(string), typeof(string), typeof(string) });
            //object Response = MyMethod.Invoke(inst, BindingFlags.InvokeMethod, null, parameters, null);
            return retunobject;
        }
    }
}