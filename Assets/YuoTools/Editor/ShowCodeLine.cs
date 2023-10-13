using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OpenAi;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using YuoTools.Extend.Helper;

namespace YuoTools.Editor
{
    public class ShowCodeLine : OdinEditorWindow
    {
        [MenuItem("Tools/ShowCodeLine")]
        private static void OpenWindow()
        {
            var window = GetWindow<ShowCodeLine>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1300, 1000);
            window.Show();
            window.Init();
        }

        public void Init()
        {
            path = Data.path;
            excludes = Data.excludes;
            rankType = Data.rankType;
            ranks = Data.ranks;
            UpdatePath();
        }

        [FolderPath] [OnValueChanged("UpdatePath")] [HorizontalGroup("Top")]
        public string path;

        [FoldoutGroup("其他选项")] [FolderPath] [LabelText("忽略文件夹")] [LabelWidth(600)]
        public List<string> excludes = new();

        [LabelWidth(70)] [ReadOnly] [HorizontalGroup("统计")] [SuffixLabel("行", true)] [LabelText("有效总行数")]
        public long totalLine;

        [LabelWidth(70)] [ReadOnly] [HorizontalGroup("统计")] [SuffixLabel("行", true)] [LabelText("文件总行数")]
        public long totalFileLine;

        [LabelWidth(70)] [ReadOnly] [HorizontalGroup("统计")] [LabelText("文件总大小")]
        public string totalSize;

        [LabelWidth(70)] [ReadOnly] [HorizontalGroup("统计")] [LabelText("文件总数")]
        public long totalFileCount;

        [LabelWidth(70)] [ReadOnly] [HorizontalGroup("统计2")] [LabelText("有效字符数")]
        public long totalCharCount;

        [LabelWidth(70)] [ReadOnly] [HorizontalGroup("统计2")] [LabelText("总字符数")]
        public long totalFileCharCount;

        [LabelWidth(150)] [ReadOnly] [HorizontalGroup("统计2")] [LabelText("相当于")]
        public string amountTo;

        [EnumToggleButtons]
        [OnValueChanged("UpdatePath")]
        [LabelWidth(50)]
        [LabelText("排序")]
        [HorizontalGroup(width: 800)]
        public RankType rankType = RankType.有效行数;

        [EnumToggleButtons] [OnValueChanged("UpdatePath")] [HideLabel] [HorizontalGroup(width: 150)]
        public RankAsc ranks = RankAsc.从大到小;

        [HideLabel] [HorizontalGroup("Tip", width: 407)] [ReadOnly]
        public string Tip_Path = "路径";

        [HideLabel] [HorizontalGroup("Tip", width: 75)] [ReadOnly]
        public string Tip_LineCount = "有效行";

        [HideLabel] [HorizontalGroup("Tip", width: 75)] [ReadOnly]
        public string Tip_FileCount = "总行数";

        [HideLabel] [HorizontalGroup("Tip", width: 75)] [ReadOnly]
        public string Tip_Size = "大小";

        [HideLabel] [HorizontalGroup("Tip", width: 75)] [ReadOnly]
        public string Tip_CharCount = "字符";

        [HideLabel] [HorizontalGroup("Tip", width: 75)] [ReadOnly]
        public string Tip_FileCharCount = "总字符";

        [HideLabel] [HorizontalGroup("Tip", width: 150)] [ReadOnly]
        public string Tip_Time = "最后修改时间";

        [HideLabel]
        // [ReadOnly]
        [ListDrawerSettings(ShowFoldout = true, HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
        public List<FileInfoItem> files = new();

        [HorizontalGroup("AI", width: 200)]
        [Button("问问AI")]
        async void ToChat()
        {
            string message = JsonConvert.SerializeObject(files);
            message = "分别猜测下面这些代码文件的作用,并给出我关于行数的建议" + message;
            var result = await ChatGpt.SingleAskStream(message, x => { AI = x; });
            AI = result + "\n" + "------------------------";
        }

        [HorizontalGroup("AI")] [ReadOnly] [LabelWidth(50)] [TextArea(20, 1000)]
        public string AI = "";

        [HorizontalGroup("Top", width: 50)]
        [Button("刷新")]
        void UpdatePath()
        {
            files.Clear();
            totalLine = 0;
            totalFileLine = 0;
            totalFileCount = 0;
            totalCharCount = 0;
            totalFileCharCount = 0;
            long _totalSize = 0;
            foreach (var s in GetAllFilesOfExtension(path, ".cs"))
            {
                bool isExclude = false;
                var replace = s.Replace("\\", "/");
                foreach (var exclude in excludes)
                {
                    if (replace.Contains(exclude) && exclude != "")
                    {
                        isExclude = true;
                        break;
                    }
                }

                if (isExclude)
                {
                    continue;
                }

                var item = new FileInfoItem();
                item.FileName = replace;
                var count = GetFileLineCount(replace);
                item.LineCount = count.codeLine;
                item.FileCount = count.fileLine;
                item.CharCount = count.charCount;
                item.FileCharCount = count.fileCharCount;
                var size = GetFileSize(replace);
                item.FileSize = FileSizeToString(size);

                item.LastWriteTime = File.GetLastWriteTime(replace).ToString("yyyy-MM-dd HH:mm:ss");

                item.FileCode = FileHelper.GetFileEncodeType(replace).EncodingName;
                files.Add(item);

                _totalSize += size;
                totalLine += item.LineCount;
                totalFileLine += item.FileCount;
                totalFileCount++;
                totalCharCount += item.CharCount;
                totalFileCharCount += item.FileCharCount;
            }

            totalSize = FileSizeToString(_totalSize);

            files = rankType switch
            {
                RankType.文件名 => ranks == RankAsc.从小到大
                    ? files.OrderBy(x => x.FileName).ToList()
                    : files.OrderByDescending(x => x.FileName).ToList(),
                RankType.有效行数 => ranks == RankAsc.从小到大
                    ? files.OrderBy(x => x.LineCount).ToList()
                    : files.OrderByDescending(x => x.LineCount).ToList(),
                RankType.最后修改时间 => ranks == RankAsc.从小到大
                    ? files.OrderBy(x => x.LastWriteTime).ToList()
                    : files.OrderByDescending(x => x.LastWriteTime).ToList(),
                RankType.文件行数 => ranks == RankAsc.从小到大
                    ? files.OrderBy(x => x.FileCount).ToList()
                    : files.OrderByDescending(x => x.FileCount).ToList(),
                RankType.文件大小 => ranks == RankAsc.从小到大
                    ? files.OrderBy(x => x.FileSize).ToList()
                    : files.OrderByDescending(x => x.FileSize).ToList(),
                RankType.有效字符数 => ranks == RankAsc.从小到大
                    ? files.OrderBy(x => x.CharCount).ToList()
                    : files.OrderByDescending(x => x.CharCount).ToList(),
                RankType.总字符数 => ranks == RankAsc.从小到大
                    ? files.OrderBy(x => x.FileCharCount).ToList()
                    : files.OrderByDescending(x => x.FileCharCount).ToList(),
                _ => throw new ArgumentOutOfRangeException()
            };

            Data.excludes = excludes;
            Data.path = path;
            Data.rankType = rankType;
            Data.ranks = ranks;
            amountTo = AmountTo(totalFileCharCount);
        }

        List<(long lenth, string name)> dic = new()
        {
            (170000000, "宇宙巨校闪级生"),
            (50000000, "红轮"),
            (20223273, "从零开始"),
            (2310000, "追忆似水年华"),
            (1864465, "一千零一夜"),
            (1090000, "战争与和平"),
            (1064258, "悲惨的世界"),
            (906347, "水浒传"),
            (862514, "红楼梦"),
            (820000, "西游记"),
            (787868, "福尔摩斯探案全集"),
            (780000, "卡拉马佐夫兄弟"),
            (720000, "金瓶梅"),
            (604520, "三国演义"),
            (527031, "格林童话"),
            (496000, "白鹿原"),
            (302648, "海底两万里"),
            (259561, "喧哗与骚动"),
            (274832, "阿凡提的故事"),
            (235800, "蚀"),
            (230100, "沉默的羔羊"),
            (5360, "火车上的威风"),
        };

        string AmountTo(long size)
        {
            dic.Sort((x, y) => x.lenth.CompareTo(y.lenth));
            string result = "";
            foreach (var item in dic)
            {
                if (size > item.lenth)
                {
                    result = $"{(float)size / item.lenth:F2}本 《{item.name}》";
                }
            }

            return result;
        }

        private ShowCodeLineData data;

        public ShowCodeLineData Data
        {
            get
            {
                if (data == null)
                {
                    var assets = Resources.Load<ShowCodeLineData>("ShowCodeLineData");
                    if (assets == null)
                    {
                        data = ScriptableObject.CreateInstance<ShowCodeLineData>();
                        data.path = path;
                        data.excludes = excludes;
                        data.rankType = rankType;
                        data.ranks = ranks;
                        AssetDatabase.CreateAsset(data, "Assets/YuoTools/Editor/Resources/ShowCodeLineData.asset");
                    }

                    data = assets;
                }

                return data;
            }
        }

        public enum RankType
        {
            文件名,
            有效行数,
            文件行数,
            文件大小,
            有效字符数,
            总字符数,
            最后修改时间,
        }

        string[] GetAllFilesOfExtension(string path, string extensionName)
        {
            if (path.IsNullOrSpace()) return new string[] { };
            return Directory.GetFiles(path, $"*{extensionName}", SearchOption.AllDirectories);
        }

        (int codeLine, int fileLine, int charCount, int fileCharCount) GetFileLineCount(string s)
        {
            int count = 0;
            using StreamReader sr = new StreamReader(s);
            string line = sr.ReadLine();
            int fileLineNum = 0;

            int charCount = 0;
            int fileCharCount = 0;


            while (line != null)
            {
                fileLineNum++;
                fileCharCount += line.Replace(" ", "").Length;
                //移除空格和制表符
                var trim = line.Trim();

                //计算有效代码行
                //1.是引入命名空间的代码行
                if (line.StartsWith("using "))
                {
                    // Debug.Log($"命名空间-{line}-第{fileLineNum}行");

                    goto jump;
                }


                //2.是注释的代码行
                if (trim.StartsWith("//"))
                {
                    // Debug.Log($"注释-{trim}-第{fileLineNum}行");
                    goto jump;
                }

                //3.是空行
                if (trim == "")
                {
                    // Debug.Log($"空行-{trim}-第{fileLineNum}行");
                    goto jump;
                }

                //4.只有一个大括号的代码行
                if (trim == "{" || trim == "}")
                {
                    // Debug.Log($"大括号-{trim}-第{fileLineNum}行");
                    goto jump;
                }

                // Debug.Log($"有效代码行-{trim}-第{fileLineNum}行");
                count++;
                charCount += trim.Replace(" ", "").Length;
                jump :
                line = sr.ReadLine();
            }

            return (count, fileLineNum, charCount, fileCharCount);
        }

        long GetFileSize(string fileName)
        {
            long result = -1;
            if (File.Exists(fileName))
            {
                try
                {
                    FileInfo fi = new FileInfo(fileName);
                    result = fi.Length;
                }
                catch
                {
                    // ignored
                }
            }

            return result;
        }

        string FileSizeToString(long size)
        {
            string result = "0";
            if (size <= 0) return "0";
            if (size < 1024)
            {
                result = size + "B";
            }
            else if (size < 1024 * 1024)
            {
                result = (size / 1024.0).ToString("0.00") + "KB";
            }
            else if (size < 1024 * 1024 * 1024)
            {
                result = (size / 1024.0 / 1024.0).ToString("0.00") + "MB";
            }
            else
            {
                result = (size / 1024.0 / 1024.0 / 1024.0).ToString("0.00") + "GB";
            }

            return result;
        }

        public enum RankAsc
        {
            从小到大,
            从大到小
        }

        [Serializable]
        public class FileInfoItem
        {
            [ReadOnly] [HorizontalGroup(width: 400)] [HideLabel]
            public string FileName;

            [ReadOnly] [HorizontalGroup(width: 75)] [HideLabel] [SuffixLabel("行", true)]
            public int LineCount;

            [ReadOnly] [HorizontalGroup(width: 75)] [HideLabel] [SuffixLabel("行", true)]
            public int FileCount;

            [ReadOnly] [HorizontalGroup(width: 75)] [HideLabel]
            public string FileSize;

            [ReadOnly] [HorizontalGroup(width: 75)] [HideLabel]
            public long CharCount;

            [ReadOnly] [HorizontalGroup(width: 75)] [HideLabel]
            public long FileCharCount;

            [ReadOnly] [HorizontalGroup(width: 150)] [HideLabel]
            public string LastWriteTime;

            [ReadOnly] [HorizontalGroup(width: 150)] [HideLabel]
            public string FileCode;

            [HorizontalGroup()]
            [Button("分析")]
            async void Read()
            {
                var text = File.ReadAllText(FileName);
                string message = $"这是我的脚本,帮我分析一下功能并且找出可能存在的问题,谢谢!{text}";
                var window = GetWindow<ShowCodeLine>();
                await ChatGpt.SingleAskStream(message, (x) => { window.AI = x; });
            }
        }
    }
}