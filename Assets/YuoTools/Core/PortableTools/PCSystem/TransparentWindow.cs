#if UNITY_STANDALONE_WIN
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using YuoTools;

public class TransparentWindow : SingletonMono<TransparentWindow>
{
    public bool WindowTop;
    public bool HideFrame;

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    #region 定义外部函数

    // 获取显示在最上面的窗口
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    // 注意要编写与 32 位和 64 位版本的 Windows 兼容的代码 请使用SetWindowLongPtr
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy,
        uint uFlags);

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

    #endregion 定义外部函数

    // 窗口句柄
    private IntPtr Handle;

    public void Init()
    {
#if !UNITY_EDITOR
        // 获取窗口句柄
        Handle = GetForegroundWindow();

        // 设置窗口的属性
       if(HideFrame) SetWindowLong(Handle, -16, 0x80000000);

        var margins = new MARGINS() { cxLeftWidth = -1 };

        // 将窗口框架扩展到工作区
        DwmExtendFrameIntoClientArea(Handle, ref margins);

        // 设置窗口位置
        SetWindowPos(Handle, -1, 0, 0, 0, 0, 2 | 1 | 64);
#endif
    }

    private async void Start()
    {
        await YuoWait.WaitTimeAsync(1);
        Init();
    }

    private void Update()
    {
#if !UNITY_EDITOR
        // 保持窗口始终在最前面
        //if (WindowTop && Handle != GetForegroundWindow()) SetForegroundWindow(Handle);
        //Ctrl+ 左键拖动
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            ReleaseCapture();
            SendMessage(Handle, 0xA1, 0x02, 0);
            SendMessage(Handle, 0x0202, 0, 0);
        }
#endif
    }
}
#endif