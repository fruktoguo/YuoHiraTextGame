using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace YuoTools.Extend.Helper
{
#if UNITY_EDITOR_WIN
    public class WindowsHelper
    {
        public static OpenFileName OpenSaveFileWindow(string FormatName, int fileSize = 256)
        {
            OpenFileName openFileName = new OpenFileName();
            openFileName.structSize = Marshal.SizeOf(openFileName);
            openFileName.filter = $"{FormatName}文件(*.{FormatName})\0*.{FormatName}";
            openFileName.file = new string(new char[fileSize]);
            openFileName.maxFile = openFileName.file.Length;
            openFileName.fileTitle = new string(new char[64]);
            openFileName.maxFileTitle = openFileName.fileTitle.Length;
            openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\'); //默认路径
            openFileName.title = "选择文件";
            openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

            if (LocalDialog.GetSaveFileName(openFileName))
            {
                UnityEngine.Debug.Log(openFileName.file);
                UnityEngine.Debug.Log(openFileName.fileTitle);
                return openFileName;
            }

            return null;
        }

        /// <summary>
        /// 打开“选择目录”对话框，返回用户选择的路径
        /// </summary>
        /// <returns>成功则返回路径、失败则返回""</returns>
        public static string GetSavePath(string tille = "choose", string defPath = "")
        {
            BrowseInfo browseInfo = new BrowseInfo();
            browseInfo.displayName = new string(new char[256]);
            browseInfo.title = tille;
            browseInfo.flags = (int)BrowseFlag.BIF_RETURNONLYFSDIRS + (int)BrowseFlag.BIF_STATUSTEXT +
                               (int)BrowseFlag.BIF_VALIDATE;
            //browseInfo.flags = 0x00000010 | 0x00000002 | 0x00000001 | 0x00000040 | 0x00000080;    //需查阅MSDN，根据需求添加
            IntPtr a = LocalDialog.SHBrowseForFolder(browseInfo); //打开对话框，让用户选择

            if (a != IntPtr.Zero)
            {
                byte[] path = new byte[256];
                LocalDialog.SHGetPathFromIDList(a, path); //调用系统函数，解析结果并输出至path中

                //将路径的中文乱码恢复正常（稍后再说）
                Encoding utf8 = Encoding.UTF8;
                byte[] utf8Bytes = Encoding.Convert(Encoding.Default, utf8, path);
                string utf8String = utf8.GetString(utf8Bytes);

                utf8String = utf8String.Replace("\0", "");
                return utf8String;
            }

            return "";
        }

        /// <summary>
        /// 屏幕截图,记得隐藏不需要截取的部分
        /// </summary>
        /// <param name="name"></param>
        private static void CaptureScreen(string name)
        {
            ScreenCapture.CaptureScreenshot($"{name}.png", 0);
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isFile"></param>
        public static void OpenDirectory(string path, bool isFile = false)
        {
            if (string.IsNullOrEmpty(path)) return;
            path = path.Replace("/", "\\");
            if (isFile)
            {
                if (!File.Exists(path))
                {
                    UnityEngine.Debug.LogError("No File: " + path);
                    return;
                }

                path = string.Format("/Select, {0}", path);
            }
            else
            {
                if (!Directory.Exists(path))
                {
                    UnityEngine.Debug.LogError("No Directory: " + path);
                    return;
                }
            }

            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        public static void CopyToClipboard(string str)
        {
            UnityEngine.GUIUtility.systemCopyBuffer = str;
        }

        public static MemoryInfo LogMemory()
        {
            MemoryInfo MemInfo = new MemoryInfo();
            GlobalMemoryStatus(ref MemInfo);

            double totalMb = MemInfo.TotalPhysical / 1024 / 1024;
            double avaliableMb = MemInfo.AvailablePhysical / 1024 / 1024;

            Debug.Log($"物理内存共有:{totalMb}MB");
            Debug.Log($"可使用的物理内存:{avaliableMb}MB");
            Debug.Log($"剩余内存百分比：{Math.Round((avaliableMb / totalMb) * 100, 2)}");
            return MemInfo;
        }

        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref MemoryInfo meminfo);

        /// <summary>
        /// 打开CMD并执行命令
        /// </summary>
        /// <param name="cmd"></param>
        public static void Command(string cmd)
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Arguments = "/k" + cmd,
                    CreateNoWindow = false,
                }
            };
            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
            }
            finally
            {
                process.Close();
            }
        }
    }

    #region 类型

    public struct MemoryInfo
    {
        public uint Length;
        public uint MemoryLoad;
        public ulong TotalPhysical; //总内存
        public ulong AvailablePhysical; //可用物理内存
        public ulong TotalPageFile;
        public ulong AvailablePageFile;
        public ulong TotalVirtual;
        public ulong AvailableVirtual;
    }

    public class OpenFileName
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }

    public enum BrowseFlag
    {
        BIF_RETURNONLYFSDIRS = 0x0001, // For finding a folder to start document searching
        BIF_DONTGOBELOWDOMAIN = 0x0002, // For starting the Find Computer
        BIF_STATUSTEXT = 0x0004,
        BIF_RETURNFSANCESTORS = 0x0008,
        BIF_EDITBOX = 0x0010,
        BIF_VALIDATE = 0x0020, // insist on valid result (or CANCEL)

        BIF_BROWSEFORCOMPUTER = 0x1000, // Browsing for Computers.
        BIF_BROWSEFORPRINTER = 0x2000, // Browsing for Printers
        BIF_BROWSEINCLUDEFILES = 0x4000 // Browsing for Everything
    }

    //目录对话框所需数据类型
    public struct BrowseInfo
    {
        public IntPtr hwndOwner;
        public IntPtr pidlRoot;

        [MarshalAs(UnmanagedType.LPTStr)] public string displayName;

        [MarshalAs(UnmanagedType.LPTStr)] public string title;

        public int flags;
        public IntPtr callback;
        public IntPtr lparam;
    }

    public class LocalDialog
    {
        //链接指定系统函数       打开文件对话框
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

        public static bool GetOFN([In, Out] OpenFileName ofn)
        {
            return GetOpenFileName(ofn);
        }

        //链接指定系统函数        另存为对话框
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

        public static bool GetSFN([In, Out] OpenFileName ofn)
        {
            return GetSaveFileName(ofn);
        }

        //弹出“选择目录”对话框
        [DllImport("Shell32.dll", SetLastError = true)]
        public static extern IntPtr SHBrowseForFolder([In, Out] BrowseInfo browse);

        //结果集解析函数
        [DllImport("Shell32.dll", SetLastError = true)]
        public static extern bool SHGetPathFromIDList([In] IntPtr dlist, [In] byte[] PathName);
    }

    #endregion

    /// <summary>
    /// 常用CMD命令
    /// </summary>
    public static class CmdCommand
    {
        /// <summary>
        /// 打开文件管理器
        /// </summary>
        public const string OpenFolder = "explorer.exe";

        /// <summary>
        /// 立即关机
        /// </summary>
        public const string AutoShutdown = "shutdown -s -t 0";

        /// <summary>
        /// 立即重启
        /// </summary>
        public const string AutoRestart = "shutdown -r -t 0";

        /// <summary>
        /// 取消关机
        /// </summary>
        public const string CancelShutdown = "shutdown -a";

        /// <summary>
        /// 刷新DNS
        /// </summary>
        public const string RefreshDns = "ipconfig /flushdns";

        /// <summary>
        /// 清空DNS缓存
        /// </summary>
        public const string ClearDnsCache = "ipconfig /displaydns";
        
        public static Dictionary<string,string> commandDic = new Dictionary<string, string>()
        {
            {"OpenFolder",OpenFolder},
            {"AutoShutdown",AutoShutdown},
            {"AutoRestart",AutoRestart},
            {"CancelShutdown",CancelShutdown},
            {"RefreshDns",RefreshDns},
            {"ClearDnsCache",ClearDnsCache},
        };
    }
#endif
}