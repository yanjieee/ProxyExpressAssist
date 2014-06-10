using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ProxyExpressAssist
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IntPtr wnd = new IntPtr(0);
            wnd = FindWindow("Tfrm_MainForm", "ProxyExpress 代理快车");
            if (wnd != IntPtr.Zero)
            {
                IntPtr pageControl = new IntPtr(0);
                pageControl = FindWindowEx(wnd, pageControl, "TPageControl", "");
                if (pageControl != IntPtr.Zero)
                {
                    int a = SendMessage(pageControl, WM_SETFOCUS, 0, 0);
                    a = SendMessage(pageControl, WM_LBUTTONDOWN, MK_LBUTTON, MAKELONG(190, 9));
                    a = SendMessage(pageControl, WM_LBUTTONUP, MK_LBUTTON, MAKELONG(190, 9));
//                     NMHDR nmhdr = new NMHDR();
//                     nmhdr.hwndFrom = (UInt32)pageControl;
//                     int a = SendMessage(pageControl, WM_NOTIFY, new IntPtr(TCN_SELCHANGE), nmhdr);

                }
            }
        }

        const int WM_GETTEXT = 0x000D;
        const int WM_SETTEXT = 0x000C;
        const int WM_CLICK = 0x00F5;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        const int TCN_SELCHANGE = -551;
        const int WM_NOTIFY = 0x004E;
        const int MK_LBUTTON = 0x0001;
        const int WM_SETFOCUS = 0x0007;

        struct NMHDR
        {
            public UInt32 hwndFrom;
            public UInt32 idFrom;
            public UInt32 code;         // NM_ code
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private static int MAKELONG(ushort x, ushort y) 
        {
            return (x + (y<<16)); //low order WORD 是指标的x位置； high order WORD是y位置. 
        } 

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
    }
}
