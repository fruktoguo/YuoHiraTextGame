using System;
using System.Collections.Generic;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend.Helper
{
    public class ModHelper
    {
        public static void Load()
        {
            var mods = FileHelper.GetChildDirectory(AssemblyHelper.ModPath());
            foreach (var mod in mods)
            {
                $"检索到Mod-[{FileHelper.GetPathName(mod)}]".Log();
                StopwatchHelper.Start();
                foreach (var dll in FileHelper.GetAllFilesOfExtension(mod, "dll"))
                {
                    $"检索到DLL-[{FileHelper.GetPathName(dll)}]".Log();
                }

                LoadMod(mod);
                $"加载Mod [{FileHelper.GetPathName(mod)}] 完成,共耗时[{StopwatchHelper.Stop()}ms]".Log();
            }

            YuoWorld.Instance.SystemSort();
        }

        public static void LoadMod(string mod)
        {
            List<Type> types = new List<Type>();
            foreach (var dll in FileHelper.GetAllFilesOfExtension(mod, "dll"))
            {
                types.AddRange(AssemblyHelper.LoadAssembly(dll).GetTypes());
            }

            ModMain main = null;
            foreach (var type in types)
            {
                if (CheckComponent<ModMain>(type))
                {
                    main = (ModMain)YuoWorld.Main.AddChild(type);
                    break;
                }
            }

            if (main == null) return;
            main.ModPath = mod;
            YuoWorld.Instance.LoadTypes(types.ToArray());
        }

        static bool CheckComponent<TP>(Type find) where TP : class
        {
            var baseType = find.BaseType; //获取基类
            while (baseType != null) //获取所有基类
            {
                if (baseType.Name == typeof(TP).Name) return true;
                baseType = baseType.BaseType;
            }

            return false;
        }
    }

    public abstract class ModMain : YuoComponent
    {
        public abstract string ModeName { get; }
        public abstract string Version { get; }
        public string ModPath;
    }
}