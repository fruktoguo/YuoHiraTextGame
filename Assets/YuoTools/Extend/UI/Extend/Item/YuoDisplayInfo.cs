using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using YuoTools;

public class YuoDisplayInfo
{
    public struct DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;

        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;

        public short dmOrientation;
        public short dmPaperSize;
        public short dmPaperLength;
        public short dmPaperWidth;

        public short dmScale;
        public short dmCopies;
        public short dmDefaultSource;
        public short dmPrintQuality;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;

        public short dmLogPixels;
        public short dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;

        public int dmDisplayFlags;
        public int dmDisplayFrequency;

        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;

        public int dmPanningWidth;
        public int dmPanningHeight;
    };

    private class User_32
    {
        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int CDS_UPDATEREGISTRY = 0x01;
        public const int CDS_TEST = 0x02;
        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const int DISP_CHANGE_RESTART = 1;
        public const int DISP_CHANGE_FAILED = -1;
    }

    public static void Change(int width, int height, int frequency)
    {
        int iWidth = width;
        int iHeight = height;
        DEVMODE dm = new DEVMODE();
        dm.dmDeviceName = new string(new char[32]);
        dm.dmFormName = new string(new char[32]);
        dm.dmSize = (short) Marshal.SizeOf(dm);

        if (User_32.EnumDisplaySettings(null, User_32.ENUM_CURRENT_SETTINGS, ref dm) != 0)
        {
            dm.dmPelsWidth = iWidth;
            dm.dmPelsHeight = iHeight;
            dm.dmDisplayFrequency = frequency;
            int iRet = User_32.ChangeDisplaySettings(ref dm, User_32.CDS_TEST);

            if (iRet == User_32.DISP_CHANGE_FAILED)
            {
                Debug.Log("Unable to process your request");
                Debug.Log("Description: Unable To Process Your Request. Sorry For This Inconvenience.");
            }
            else
            {
                iRet = User_32.ChangeDisplaySettings(ref dm, User_32.CDS_UPDATEREGISTRY);

                switch (iRet)
                {
                    case User_32.DISP_CHANGE_SUCCESSFUL:
                    {
                        break;
                    }
                    case User_32.DISP_CHANGE_RESTART:
                    {
                        Debug.Log("分辨率修改成功");
                        break;
                    }
                    default:
                    {
                        Debug.Log("自动分辨率修改失败");
                        break;
                    }
                }
            }
        }
    }

    public static List<(string key, DEVMODE value)> GetScreenInfo()
    {
        DEVMODE devMode = new DEVMODE();

        //HashSet<string> datas = new HashSet<string>();
        List<(string key, DEVMODE value)> devModes = new List<(string key, DEVMODE value)>();
        int index = 0;
        while (User_32.EnumDisplaySettings(null, index, ref devMode) != 0)
        {
            var str = $"{devMode.dmPelsWidth}*{devMode.dmPelsHeight}@{devMode.dmDisplayFrequency}Hz";
            //datas.Add(str);
            devModes.Add((str, devMode));
            index++;
        }

        return devModes;
    }
}