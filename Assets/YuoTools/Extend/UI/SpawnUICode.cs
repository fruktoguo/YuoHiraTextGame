using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YuoTools.UI;

#if UNITY_EDITOR
using UnityEditor;
using YuoTools.Extend.Helper;
#endif

namespace YuoTools.Extend.UI
{
    public static class SpawnUICode
    {
        public static void SpawnCode(GameObject gameObject)
        {
            if (null == gameObject) return;

            string UIName = gameObject.name;
            string strDlgName = $"View_{UIName}Component";

            string strFilePath = Application.dataPath + "/YuoUI/View";

            if (!Directory.Exists(strFilePath))
            {
                Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBuilder = new StringBuilder();

            Dictionary<string, string> allTypes = new();

            strBuilder.AppendLine(CodeSpawn_AddAllComponent(SpawnUICodeConfig.UIComponentTag, allTypes,
                gameObject.transform));

            strBuilder.AppendLine(CodeSpawn_FindAll(allTypes));

            strBuilder.AppendLine("\n\t\t}");

            strBuilder.AppendLine("\t}\r}");

            //引入命名空间
            StringBuilder final = new StringBuilder();

            final.AppendLine(CodeSpawn_AddNameSpace(strDlgName, allTypes));


            final.AppendLine("\tpublic static partial class ViewType");
            final.AppendLine("\t{");
            final.AppendLine($"\t\tpublic const string {UIName} = \"{UIName}\";");
            final.AppendLine("\t}");

            final.AppendLine($"\tpublic partial class {strDlgName} : UIComponent \n\t{{");

            final.AppendLine("");
            final.AppendLine($"\t\tpublic static {strDlgName} GetView() => UIManagerComponent.Get.GetUIView<{strDlgName}>();");
            final.AppendLine("");
            
            final.Append(strBuilder);

            sw.Write(final.ToString());
            
            sw.Close();

            SpawnSystemCode(UIName);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public static void SpawnGeneralUICode(GameObject gameObject)
        {
            if (null == gameObject) return;

            string UIName = gameObject.name.Replace(SpawnUICodeConfig.GeneralUITag, "");

            string strDlgName = $"View_{UIName}Component";

            string strFilePath = Application.dataPath + "/YuoUI/View/General";

            if (!Directory.Exists(strFilePath))
            {
                Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBuilder = new StringBuilder();
            Dictionary<string, string> allTypes = new();

            strBuilder.AppendLine(CodeSpawn_AddAllComponent(SpawnUICodeConfig.UIComponentTag, allTypes,
                gameObject.transform));

            strBuilder.AppendLine(CodeSpawn_FindAll(allTypes));

            strBuilder.AppendLine("\n\t\t}");

            strBuilder.AppendLine("\t}\r}");

            //引入命名空间
            StringBuilder final = new StringBuilder();

            final.AppendLine(CodeSpawn_AddNameSpace(strDlgName, allTypes));
            final.AppendLine($"\tpublic partial class {strDlgName} : UIComponent \n\t{{");

            final.Append(strBuilder);
            sw.Close();
            SpawnSystemCode(UIName, UIType.General);
        }

        private static void SpawnChildUICode(GameObject gameObject)
        {
            if (null == gameObject) return;

            string uiName = gameObject.name.Replace(SpawnUICodeConfig.VariantChildUITag, SpawnUICodeConfig.ChildUITag);
            uiName = uiName.Replace(SpawnUICodeConfig.ChildUITag, "");
            if (uiName.Contains("_"))
            {
                uiName = uiName.Split('_')[0];
            }

            string strDlgName = $"View_{uiName}Component";

            string strFilePath = Application.dataPath + "/YuoUI/View/Child";

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBuilder = new StringBuilder();

            Dictionary<string, string> allTypes = new();

            strBuilder.AppendLine(CodeSpawn_AddAllComponent(SpawnUICodeConfig.UIComponentTag, allTypes,
                gameObject.transform, findThis: true));

            strBuilder.AppendLine(CodeSpawn_FindAll(allTypes));

            strBuilder.AppendLine("\n\t\t}");

            strBuilder.AppendLine("\t}\r}");

            //引入命名空间
            StringBuilder final = new StringBuilder();

            final.AppendLine(CodeSpawn_AddNameSpace(strDlgName, allTypes));
            final.AppendLine($"\tpublic partial class {strDlgName} : UIComponent \n\t{{");

            final.Append(strBuilder);

            sw.Write(final.ToString());

            sw.Close();

            SpawnSystemCode(uiName, UIType.Child);
        }

        private static void SpawnVariantChildUICode(GameObject gameObject)
        {
            if (null == gameObject) return;

            string uiName = gameObject.name.Replace(SpawnUICodeConfig.VariantChildUITag, "");

            string strDlgName = $"View_{uiName}VariantComponent";

            string strFilePath = Application.dataPath + "/YuoUI/View/Child";

            if (!Directory.Exists(strFilePath))
            {
                Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBuilder = new StringBuilder();

            Dictionary<string, string> allTypes = new();

            strBuilder.AppendLine(CodeSpawn_AddAllComponent(SpawnUICodeConfig.VariantChildComponentTag, allTypes,
                gameObject.transform));

            strBuilder.AppendLine(CodeSpawn_FindAll(allTypes));

            strBuilder.AppendLine("\n\t\t}");

            strBuilder.AppendLine("\t}\r}");

            //引入命名空间
            StringBuilder final = new StringBuilder();

            final.AppendLine(CodeSpawn_AddNameSpace(strDlgName, allTypes));
            final.AppendLine($"\tpublic partial class {strDlgName} : UIComponent \n\t{{");

            final.Append(strBuilder);

            sw.Write(final.ToString());

            sw.Close();

            SpawnSystemCode(uiName + "Variant", UIType.Child);
        }

        public static string[] GetValue(string type)
        {
            Dictionary<string, string[]> dic = new Dictionary<string, string[]>();
            dic.Add("Text", new string[] { ".text" });
            dic.Add("Image", new string[] { ".sprite", ".color" });
            dic.Add("Slider", new string[] { ".value" });
            dic.Add("Toggle", new string[] { ".isOn" });
            dic.Add("InputField", new string[] { ".text" });
            return dic.ContainsKey(type) ? dic[type] : null;
        }

        public static string CodeSpawn_AddNameSpace(string strDlgName, Dictionary<string, string> allTypes)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (var space in SpawnConfig.ComponentNameSpace)
            {
                strBuilder.AppendLine(space);
            }

            foreach (var type in allTypes.Keys)
            {
                foreach (var item in SpawnConfig.ComponentAddNameSpace)
                {
                    if (item.Key.Name == type)
                        strBuilder.AppendLine(item.Value);
                }
            }

            strBuilder.AppendLine();
            //设置命名空间
            strBuilder.AppendLine("namespace YuoTools.UI\n{");

            return strBuilder.ToString();
        }

        public static string CodeSpawn_AddAllComponent(string tag, Dictionary<string, string> allTypes,
            Transform transform, bool findThis = false)
        {
            StringBuilder strBuilder = new StringBuilder();

            //获取所有组件
            // List<Transform> trans = FindAll(transform);
            List<Transform> trans = new();
            TransformInfo tranInfo = new TransformInfo(transform);

            FindAll(tranInfo);

            //剔除自己
            trans.Remove(transform);

            #region 初始化额外组件

            //获取通用组件
            List<Transform> generalTemps = new List<Transform>();
            List<Transform> generals = new List<Transform>();

            //获取子组件
            List<Transform> childTemps = new List<Transform>();
            List<Transform> children = new List<Transform>();

            //获取变体子组件
            List<Transform> variantChildren = new List<Transform>();
            List<Transform> variantChildTemps = new List<Transform>();

            void GetAll(TransformInfo info)
            {
                foreach (var tran in info.children)
                {
                    if (tran.transform.name.StartsWith(SpawnUICodeConfig.GeneralUITag))
                    {
                        generals.Add(tran.transform);
                    }
                    else if (tran.transform.name.StartsWith(SpawnUICodeConfig.ChildUITag))
                    {
                        children.Add(tran.transform);
                    }
                    else if (tran.transform.name.StartsWith(SpawnUICodeConfig.VariantChildUITag))
                    {
                        variantChildren.Add(tran.transform);
                    }
                    else
                    {
                        trans.Add(tran.transform);
                        GetAll(tran);
                    }
                }
            }

            GetAll(tranInfo);

            #endregion

            if (findThis)
            {
                var types = GetTypes(transform);
                foreach (var type in types)
                {
                    if (!allTypes.ContainsKey(type)) allTypes.Add(type, "");
                    allTypes[type] += $"\n\t\t\tall_{type}.Add(Main{type});";
                    string get = "{\n\t\t\tget\n\t\t\t{\n\t\t\t\tif (" + $"main{type}" +
                                 $" == null)\n\t\t\t\t\tmain{type} = rectTransform.GetComponent<{type}>();\n\t\t\t\treturn " +
                                 $"main{type}" +
                                 ";\n\t\t\t}\n\t\t}";

                    strBuilder.AppendLine($"\n\t\tprivate {type} main{type};");

                    strBuilder.AppendLine($"\n\t\tpublic {type} Main{type}\n\t\t{get}");
                }
            }

            foreach (var item in trans)
            {
                if (item.name.StartsWith(tag))
                {
                    string name = item.name.Replace(tag, "");
                    var types = GetTypes(item);

                    if (types.Count == 0)
                    {
                        types.Add("RectTransform");
                    }

                    foreach (var type in types)
                    {
                        CodeSpawn_AddFindAll(allTypes, type, name);
                        strBuilder.AppendLine(CodeSpawn_AddComponent(type, name,
                            GetRelativePath(item, transform)));
                    }
                }
            }

            #region 生成额外组件

            foreach (var general in generals)
            {
                Debug.Log(general);
                tag = SpawnUICodeConfig.ChildUITag;
                SpawnGeneralUICode(general.gameObject);
                string name = general.name.Replace(tag, "");
                string type = $"View_{name}Component";
                string get = "{\n\t\t\tget" +
                             "\n\t\t\t{" +
                             "\n\t\t\t\tif (" + $"mGeneral_{name} == null)" +
                             "\n\t\t\t\t{" +
                             $"\n\t\t\t\t\tmGeneral_{name} = Entity.AddChild<{type}>();" +
                             $"\n\t\t\t\t\tmGeneral_{name}.rectTransform = rectTransform.Find(" +
                             $"\"{GetRelativePath(general, transform)}\"" + ") as RectTransform;" +
                             "\n\t\t\t\t}" +
                             $"\n\t\t\t\treturn " + $"mGeneral_{name}" + ";\n\t\t\t}\n\t\t}";

                strBuilder.AppendLine($"\n\t\tprivate {type} mGeneral_{name};");

                strBuilder.AppendLine($"\n\t\tpublic {type} General_{name}\n\t\t{get}");
            }

            foreach (var child in children)
            {
                tag = SpawnUICodeConfig.ChildUITag;
                SpawnChildUICode(child.gameObject);
                string name = child.name.Replace(tag, "");
                string typeName = child.name.Replace(tag, "");
                if (typeName.Contains("_"))
                {
                    typeName = typeName.Split('_')[0];
                }

                string type = $"View_{typeName}Component";
                Debug.Log($"检索到子物体 [ {child} ]  _ 类型为 [ {type} ]");
                if (!allTypes.ContainsKey(type))
                {
                    allTypes.Add(type, "");
                }

                allTypes[type] += $"\n\t\t\tall_{type}.Add(Child_{name});";

                string get = "{\n\t\t\tget" +
                             "\n\t\t\t{" +
                             "\n\t\t\t\tif (" + $"mChild_{name} == null)" +
                             "\n\t\t\t\t{" +
                             $"\n\t\t\t\t\tmChild_{name} = Entity.AddChild<{type}>();" +
                             "\n\t\t\t\t\t" + $@"mChild_{name}.Entity.EntityName = ""{name}"";" +
                             $"\n\t\t\t\t\tmChild_{name}.rectTransform = rectTransform.Find(" +
                             $"\"{GetRelativePath(child, transform)}\"" + ") as RectTransform;" +
                             $"\n\t\t\t\t\tmChild_{name}.RunSystem<IUICreate>();" +
                             "\n\t\t\t\t}" +
                             $"\n\t\t\t\treturn " + $"mChild_{name}" + ";\n\t\t\t}\n\t\t}";

                strBuilder.AppendLine($"\n\t\tprivate {type} mChild_{name};");

                strBuilder.AppendLine($"\n\t\tpublic {type} Child_{name}\n\t\t{get}");
            }

            foreach (var variantChild in variantChildren)
            {
                tag = SpawnUICodeConfig.VariantChildUITag;
                SpawnChildUICode(variantChild.gameObject);
                SpawnVariantChildUICode(variantChild.gameObject);
                string name = variantChild.name.Replace(tag, "");
                string typeName = variantChild.name.Replace(tag, "");
                string typeVariantName = variantChild.name.Replace(tag, "");
                if (typeName.Contains("_"))
                {
                    typeName = typeName.Split('_')[0];
                }

                string type = $"View_{typeName}Component";
                string typeVariant = $"View_{typeVariantName}VariantComponent";
                Debug.Log($"检索到子物体变体 [ {variantChild} ]  _ 类型为 [ {type} ]");
                allTypes.TryAdd(type, "");

                allTypes[type] += $"\n\t\t\tall_{type}.Add(Child_{name});";

                string get = "{\n\t\t\tget" +
                             "\n\t\t\t{" +
                             "\n\t\t\t\tif (" + $"mChild_{name} == null)" +
                             "\n\t\t\t\t{" +
                             $"\n\t\t\t\t\tmChild_{name} = Entity.AddChild<{type}>();" +
                             "\n\t\t\t\t\t" + $@"mChild_{name}.Entity.EntityName = ""{name}"";" +
                             $"\n\t\t\t\t\tmChild_{name}Variant = mChild_{name}.AddComponent<{typeVariant}>();" +
                             $"\n\t\t\t\t\tmChild_{name}.rectTransform = rectTransform.Find(" +
                             $"\"{GetRelativePath(variantChild, transform)}\"" + ") as RectTransform;" +
                             $"\n\t\t\t\t\tmChild_{name}Variant.rectTransform = mChild_{name}.rectTransform;" +
                             $"\n\t\t\t\t\tmChild_{name}.Entity.RunSystem<IUICreate>();;" +
                             "\n\t\t\t\t}" +
                             $"\n\t\t\t\treturn " + $"mChild_{name}" + ";\n\t\t\t}\n\t\t}";

                strBuilder.AppendLine($"\n\t\tprivate {type} mChild_{name};");

                strBuilder.AppendLine($"\n\t\tpublic {type} Child_{name}\n\t\t{get}");

                string getVariant = "{\n\t\t\tget" +
                                    "\n\t\t\t{" +
                                    $"\n\t\t\t\treturn " + $"mChild_{name}Variant" + ";\n\t\t\t}\n\t\t}";

                strBuilder.AppendLine($"\n\t\tprivate {typeVariant} mChild_{name}Variant;");

                strBuilder.AppendLine($"\n\t\tpublic {typeVariant} Child_{name}Variant\n\t\t{getVariant}");
            }

            #endregion

            return strBuilder.ToString();
        }

        public static string CodeSpawn_AddComponent(string type, string name, string relativePath)
        {
            StringBuilder strBuilder = new StringBuilder();

            string get = "{\n\t\t\tget\n\t\t\t{\n\t\t\t\tif (" + $"m{type}_{name}" +
                         $" == null)\n\t\t\t\t\tm{type}_{name} = rectTransform.Find(\"" +
                         relativePath +
                         $"\").GetComponent<{type}>();\n\t\t\t\treturn " + $"m{type}_{name}" +
                         ";\n\t\t\t}\n\t\t}";

            strBuilder.AppendLine($"\n\t\tprivate {type} m{type}_{name};");

            strBuilder.AppendLine($"\n\t\tpublic {type} {type}_{name}\n\t\t{get}");
            return strBuilder.ToString();
        }

        public static void CodeSpawn_AddFindAll(Dictionary<string, string> allTypes, string type, string name)
        {
            if (!allTypes.ContainsKey(type)) allTypes.Add(type, "");
            allTypes[type] += $"\n\t\t\tall_{type}.Add({type}_{name});";
        }

        public static string CodeSpawn_FindAll(Dictionary<string, string> allTypes)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (var item in allTypes.Keys)
            {
                strBuilder.AppendLine("\n\t\t[FoldoutGroup(\"ALL\")]");
                strBuilder.AppendLine($"\n\t\tpublic List<{item}> all_{item} = new();");
            }

            strBuilder.AppendLine("\n\t\tpublic void FindAll()\n\t\t{");
            foreach (var item in allTypes)
            {
                strBuilder.AppendLine($"\t\t\t\t{item.Value};");
            }

            return strBuilder.ToString();
        }

        public static void FindChild()
        {
        }

        public enum UIType
        {
            Window,
            Child,
            General
        }

        public static void SpawnSystemCode(string name, UIType uiType = UIType.Window)
        {
            string strFilePath = Application.dataPath + "/YuoUI/System";
            if (uiType != UIType.Window) strFilePath += "/Child";
            if (!Directory.Exists(strFilePath))
            {
                Directory.CreateDirectory(strFilePath);
            }

            string strDlgName = $"View_{name}System";
            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            if (File.Exists(strFilePath))
            {
                Debug.LogWarning($"{strDlgName}已存在");
                return;
            }

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBuilder = new StringBuilder();
            foreach (var space in SpawnConfig.SystemNameSpace)
            {
                strBuilder.AppendLine(space);
            }

            //设置命名空间
            strBuilder.AppendLine("namespace YuoTools.UI");
            strBuilder.AppendLine("{");
            switch (uiType)
            {
                case UIType.Window:
                    strBuilder.AppendLine(
                        $"\tpublic class View_{name}CreateSystem :YuoSystem<View_{name}Component>, IUICreate");
                    strBuilder.AppendLine("\t{");
                    strBuilder.Append("\t\t");
                    strBuilder.Append(@$"public override string Group =>""UI/{name}"";");
                    strBuilder.AppendLine("");
                    strBuilder.AppendLine("");
                    strBuilder.AppendLine($"\t\tprotected override void Run(View_{name}Component view)");
                    strBuilder.AppendLine($"\t\t{{");
                    strBuilder.AppendLine("\t\t\tview.FindAll();");
                    strBuilder.AppendLine("\t\t\t//关闭窗口的事件注册,名字不同请自行更");
                    strBuilder.AppendLine("\t\t\tview.Button_Close.SetUIClose(view.ViewName);");
                    strBuilder.AppendLine(" \t\t\tview.Button_Mask.SetUIClose(view.ViewName);");
                    strBuilder.AppendLine("\t\t}");
                    strBuilder.AppendLine("\t}");

                    strBuilder.AppendLine(
                        $"\tpublic class View_{name}OpenSystem :YuoSystem<View_{name}Component,UIAnimaComponent>, IUIOpen");
                    strBuilder.AppendLine("\t{");
                    strBuilder.Append("\t\t");
                    strBuilder.Append(@$"public override string Group =>""UI/{name}"";");
                    strBuilder.AppendLine("");
                    strBuilder.AppendLine("");
                    strBuilder.AppendLine(
                        $"\t\tprotected override void Run(View_{name}Component view, UIAnimaComponent anima)");
                    strBuilder.AppendLine($"\t\t{{");
                    strBuilder.AppendLine("\t\t\tview.Button_Mask.image.SetColorA(0);\n");
                    strBuilder.AppendLine("\t\t\tview.Button_Mask.image.DOFade(0.6f, 0.2f);");
                    strBuilder.AppendLine("\t\t}");
                    strBuilder.AppendLine("\t}");

                    strBuilder.AppendLine(
                        $"\tpublic class View_{name}CloseSystem :YuoSystem<View_{name}Component,UIAnimaComponent>, IUIClose");
                    strBuilder.AppendLine("\t{");
                    strBuilder.Append("\t\t");
                    strBuilder.Append(@$"public override string Group =>""UI/{name}"";");
                    strBuilder.AppendLine("");
                    strBuilder.AppendLine("");
                    strBuilder.AppendLine(
                        $"\t\tprotected override void Run(View_{name}Component view, UIAnimaComponent anima)");
                    strBuilder.AppendLine("\t\t{");
                    strBuilder.AppendLine("\t\t\tview.Button_Mask.image.DOFade(0f, 0.2f);");
                    strBuilder.AppendLine("\t\t}");
                    strBuilder.AppendLine("\t}");
                    break;
                case UIType.General:
                    strBuilder.AppendLine(
                        $"\tpublic class View_{name}ActiveSystem :YuoSystem<View_{name}Component>, IUIActive");
                    strBuilder.AppendLine("\t{");
                    strBuilder.Append("\t\t");
                    strBuilder.Append(@$"public override string Group =>""UI/{name}"";");
                    strBuilder.AppendLine("");
                    strBuilder.AppendLine("");
                    strBuilder.AppendLine($"\t\tprotected override void Run(View_{name}Component view)");
                    strBuilder.AppendLine($"\t\t{{");
                    strBuilder.AppendLine("\t\t\tview.FindAll();");
                    strBuilder.AppendLine("\t\t}");
                    strBuilder.AppendLine("\t}");
                    break;
                case UIType.Child:
                    strBuilder.AppendLine(
                        $"\tpublic class View_{name}CreateSystem :YuoSystem<View_{name}Component>, IUICreate");
                    strBuilder.AppendLine("\t{");
                    strBuilder.Append("\t\t");
                    strBuilder.Append(@$"public override string Group =>""UI/{name}"";");
                    strBuilder.AppendLine("");
                    strBuilder.AppendLine("");
                    strBuilder.AppendLine($"\t\tprotected override void Run(View_{name}Component view)");
                    strBuilder.AppendLine($"\t\t{{");
                    strBuilder.AppendLine("\t\t\tview.FindAll();");
                    strBuilder.AppendLine("\t\t}");
                    strBuilder.AppendLine("\t}");
                    break;
                default:
                    break;
            }

            strBuilder.AppendLine("}");

            sw.Write(strBuilder.ToString());
            sw.Close();
        }

        public static SpawnUICodeConfig SpawnConfig
        {
            get
            {
#if UNITY_EDITOR
                if (_spawnConfig == null)
                {
                    _spawnConfig = new();
                    ReflexHelper.InvokeMethodByPrefix(_spawnConfig, "Init");
                }
#endif
                return _spawnConfig;
            }
        }

        private static SpawnUICodeConfig _spawnConfig;

        private static List<string> GetTypes(Transform transform)
        {
            List<string> ts = new List<string>();

            foreach (var item in SpawnConfig.SpawnType)
            {
                Get(item);
            }

            foreach (var item in SpawnConfig.RemoveType)
            {
                if (ts.Contains(item.Key.Name)) ts.Remove(item.Value.Name);
            }

            if (ts.Count == 0)
            {
                ts.Add(nameof(RectTransform));
            }

            return ts;

            void Get(Type type)
            {
                var t = transform.GetComponent(type);
                if (t != null)
                {
                    ts.Add(t.GetType().Name);
                }
            }
        }

        private static string GetRelativePath(Transform child, Transform parent)
        {
            if (child == parent)
            {
                return parent.name;
            }

            var path = child.name;
            Transform nowParent = child.parent;
            while (nowParent != parent && nowParent != null)
            {
                path = nowParent.name + "/" + path;
                nowParent = nowParent.parent;
            }

            return path;
        }

        public static List<T> GetAllSelectComponent<T>() where T : Component
        {
            List<T> list = new List<T>();

#if UNITY_EDITOR
            foreach (var item in Selection.transforms)
            {
                foreach (var item_1 in FindAll(item))
                {
                    T t = item_1.GetComponent<T>();
                    if (t != null)
                    {
                        list.Add(t);
                    }
                }
            }
#endif
            return list;
        }

        private static List<Transform> FindAll(Transform transform)
        {
            List<Transform> list = new List<Transform>();
            list.Add(transform);
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).childCount > 0)
                {
                    list.AddRange(FindAll(transform.GetChild(i)));
                }
                else
                {
                    list.Add(transform.GetChild(i));
                }
            }

            return list;
        }

        class TransformInfo
        {
            public readonly Transform transform;
            public readonly List<TransformInfo> children = new List<TransformInfo>();

            public TransformInfo(Transform transform)
            {
                this.transform = transform;
            }
        }

        private static void FindAll(TransformInfo transform)
        {
            for (int i = 0; i < transform.transform.childCount; i++)
            {
                if (transform.transform.GetChild(i).childCount > 0)
                {
                    var child = new TransformInfo(transform.transform.GetChild(i));
                    transform.children.Add(child);
                    FindAll(child);
                }
                else
                {
                    var child = new TransformInfo(transform.transform.GetChild(i));
                    transform.children.Add(child);
                }
            }
        }
    }

    public partial class SpawnUICodeConfig
    {
        public const string UIComponentTag = "C_";
        public const string GeneralUITag = "G_";
        public const string ChildUITag = "D_";
        public const string VariantChildUITag = "DV_";
        public const string VariantChildComponentTag = "CV_";

        public static List<string> AllTag = new List<string>()
        {
            UIComponentTag,
            GeneralUITag,
            ChildUITag,
            VariantChildUITag,
            VariantChildComponentTag,
        };

        [LabelText("会被检索到的组件类型")] public List<Type> SpawnType = new()
        {
            typeof(Button),
            typeof(Image),
            typeof(RawImage),
            typeof(Text),
            typeof(TextMeshProUGUI),
            typeof(TMP_InputField),
            typeof(TMP_Dropdown),
            typeof(Toggle),
            typeof(ToggleGroup),
            typeof(Dropdown),
            typeof(InputField),
            typeof(Slider),
            typeof(ScrollRect),
            typeof(EventTrigger),
            typeof(Scrollbar),
            typeof(LayoutGroup),
            typeof(ContentSizeFitter),
            typeof(ParticleSystem),
        };

        [LabelText("相斥组件")] public Dictionary<Type, Type> RemoveType = new()
        {
            { typeof(Button), typeof(Image) },
            { typeof(Dropdown), typeof(Image) },
            { typeof(InputField), typeof(Image) },
            { typeof(Toggle), typeof(Image) },
            { typeof(ToggleGroup), typeof(Image) },
            { typeof(Slider), typeof(Image) },
            { typeof(ScrollRect), typeof(Image) },
            { typeof(Scrollbar), typeof(Image) },
            { typeof(LayoutGroup), typeof(Image) },
            { typeof(ContentSizeFitter), typeof(Image) },
            { typeof(ParticleSystem), typeof(Image) },
            { typeof(TMP_InputField), typeof(Image) },
            { typeof(TMP_Dropdown), typeof(Image) }
        };

        [LabelText("组件的默认命名空间")] public List<string> ComponentNameSpace = new()
        {
            "using UnityEngine;",
            "using System.Collections;",
            "using System.Collections.Generic;",
            "using UnityEngine.UI;",
            "using YuoTools.Main.Ecs;",
            "using Sirenix.OdinInspector;",
        };

        [LabelText("需要额外添加命名空间的组件")] public Dictionary<Type, string> ComponentAddNameSpace = new()
        {
            { typeof(TextMeshProUGUI), "using TMPro;" },
            { typeof(TMP_InputField), "using TMPro;" },
            { typeof(TMP_Dropdown), "using TMPro;" },
            { typeof(EventTrigger), "using UnityEngine.EventSystems;" },
        };

        [LabelText("System的默认命名空间")] public List<string> SystemNameSpace = new()
        {
            "using DG.Tweening;",
            "using YuoTools.Extend.Helper;",
            "using YuoTools.Main.Ecs;",
        };
    }
}