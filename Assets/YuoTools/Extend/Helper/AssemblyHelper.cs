using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace YuoTools.Extend.Helper
{
    public static class AssemblyHelper
    {
        public static Dictionary<string, Type> GetAssemblyTypes(params Assembly[] args)
        {
            var types = new Dictionary<string, Type>();
            
            foreach (var ass in args)
            foreach (var type in ass.GetTypes())
                types[type.Name] = type;

            return types;
        }

        public static Type[] LoadAssemblyTypes(string path)
        {
            var assembly = Assembly.Load(System.IO.File.ReadAllBytes(path));
            return assembly.GetTypes();
        }        
        public static Assembly LoadAssembly(string path)
        {
            return Assembly.Load(System.IO.File.ReadAllBytes(path));
        }

        public static string ModPath() => $"{Application.dataPath}/../Mod";
    }
}