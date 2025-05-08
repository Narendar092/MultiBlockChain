using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using XISystem;
using System.Data.SqlTypes;
using System.Web.Caching;

namespace XICore
{
    public class ProtectedObject
    {
        public String BoName { get; set; }
        List<String> Methods = new List<String>();

        static List<String> ProtectedObjectedBOs;
        static List<String> ErrorReferences = new List<string>();
        static ProtectedObject()
        {
            if (ProtectedObjectedBOs == null)
            {
                ProtectedObjectedBOs = new List<String>();
                //ProtectedObjectedBOs.Add("ProtectMe");
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
                foreach (Assembly assembly in assemblies)
                {
                    //var assembly = assemblyObj;
                    try
                    {
                        var types = assembly.GetTypes();


                        // Filter classes that implement the IProtectedObjects interface
                        var protectedObjectsImplementingTypes = types
                            .Where(t => typeof(IProtectedObjects).IsAssignableFrom(t) && t.IsClass)
                            .ToList();

                        foreach (var type in protectedObjectsImplementingTypes)
                        {
                            var instance = Activator.CreateInstance(type) as IProtectedObjects;
                            ProtectedObjectedBOs.AddRange(ProtectedObject.GetBONames(instance.GetModuleProtectedObjects()));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is System.Reflection.ReflectionTypeLoadException)
                        {
                            var typeLoadException = ex as ReflectionTypeLoadException;
                            var loaderExceptions = typeLoadException.LoaderExceptions;
                            foreach (var item in loaderExceptions)
                            {
                                ErrorReferences.Add(item.Message);
                            }
                        }
                    }
                }
            }
        }
        public ProtectedObject()
        {

        }
        public ProtectedObject(String boName)
        {
            this.BoName = boName;
        }
        public ProtectedObject(string boName, List<String> methods) : this(boName)
        {
            Methods = methods;
        }
        public ProtectedObject(string boName, String[] methods) : this(boName, methods.ToList())
        {
        }
        public static List<String> GetBONames(Dictionary<String, List<ProtectedObject>> ObjList)
        {
            //Dictionary<String, List<ProtectedObject>> ObjList = GetModuleProtectedObjects();
            List<String> boNames = new List<String>();
            foreach (var obj in ObjList)
            {
                foreach (ProtectedObject po in obj.Value)
                {
                    boNames.Add(po.BoName);
                }
            }
            return boNames;
        }

        public static List<String> GetMethodNames(String project, String boName, Dictionary<String, List<ProtectedObject>> ObjList)
        {
            //Dictionary<String, List<ProtectedObject>> ObjList = GetModuleProtectedObjects();
            List<String> methodNames = new List<String>();
            List<ProtectedObject> ProtectedObjs;
            ObjList.TryGetValue(project, out ProtectedObjs);

            if (ProtectedObjs != null)
            {
                foreach (var obj in ProtectedObjs)
                {
                    if (obj.BoName == boName)
                    {
                        methodNames.AddRange(obj.Methods);
                        break;
                    }
                }
            }
            return methodNames;
        }

        public static bool isProtectedObject(string boName)
        {
            return ProtectedObjectedBOs.Contains(boName);
        }
        static string GetCallingMethod(String ClassNameStartsWith = null)
        {
            List<String> callingMethods = new List<String>();
            String callingMethod = "";
            foreach (StackFrame sf in new StackTrace().GetFrames())
            {
                //"" + sf.GetMethod().ReflectedType.Assembly + sf.GetMethod().Name;
                try
                {
                    callingMethod = //sf.GetMethod().ReflectedType.Assembly.GetName().Name + "." +
                    sf.GetMethod().DeclaringType.Name + "." + sf.GetMethod().Name;
                    callingMethods.Add(callingMethod);
                    if (!String.IsNullOrEmpty(ClassNameStartsWith))
                    {
                        if (sf.GetMethod().DeclaringType.Name.IndexOf(ClassNameStartsWith) == 0)
                        {
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    break;
                }

            }

            return callingMethods.Last();
        }

        public static bool isInProtectedList(Assembly assemblyObj, String boName)
        {
            bool isFound = false;
            //String callingMethod = GetCallingMethod();

            var assembly = assemblyObj;
            var types = assembly.GetTypes();

            // Filter classes that implement the IProtectedObjects interface
            var protectedObjectsImplementingTypes = types
                .Where(t => typeof(IProtectedObjects).IsAssignableFrom(t) && t.IsClass)
                .ToList();

            List<String> classList;
            foreach (var type in protectedObjectsImplementingTypes)
            {
                var instance = Activator.CreateInstance(type) as IProtectedObjects;
                classList = ProtectedObject.GetMethodNames(assembly.GetName().Name, boName, instance.GetModuleProtectedObjects());
                if (classList != null && classList.Contains(GetCallingMethod(type.Name)))
                {
                    isFound = true;
                }
            }
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            if (!isFound)
            {
                CResult oCR = new CResult();
                oCR.sCategory = "Object protection";
                oCR.sMessage = "Object:" + boName + " is in protection ";
                //oDefBase.SaveErrortoDB(oCR);
            }
            return isFound;
        }

    }
    public interface IProtectedObjects
    {
        Dictionary<String, List<ProtectedObject>> GetModuleProtectedObjects();
    }

    public class BOObject
    {
        public string BoName { get; set; }
        public void Save()
        {
            if (!String.IsNullOrEmpty(BoName) & ProtectedObject.isProtectedObject(BoName))
            {
                //check if calling method is in list or not.
                if (!ProtectedObject.isInProtectedList(Assembly.GetCallingAssembly(), BoName))
                {
                    //write to error log as it is protected object and method is not configured
                    Console.WriteLine("not saving");
                    return;
                }
            }

            Console.WriteLine("continue saving");
        }


    }
}
