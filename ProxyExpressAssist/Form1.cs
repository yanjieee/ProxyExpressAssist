using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Threading;

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
            if (textBox1.Text == "")
            {
                textBox1.Focus();
                return;
            }
            if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("文件不存在或者路径名错误！");
                return;
            }

            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            button1.Enabled = false;
            button1.Text = "开始运行";

            timer1.Interval = int.Parse(textBox2.Text) * 1000;
            timer1.Start();
            timer1.Tag = 0;
            checkAndRestartPE();    //因为计时器一开始不会执行一次，所以需要先手动一次
        }

        /// <summary>
        /// 点击菜单栏中的收集代理->启动收集
        /// </summary>
        /// <param name="wnd">PE的窗体句柄</param>
        private void proxyCollect(IntPtr wnd) 
        {
            int collectProxy = GetMenu(wnd.ToInt32());
            collectProxy = GetSubMenu(collectProxy, 2);
            collectProxy = GetMenuItemID(collectProxy, 0);
            PostMessage(wnd, BM_CLICK, collectProxy, 0);
        }

        /// <summary>
        /// （1）点击网站吸取->选择全部（2）点击 开始吸取
        /// </summary>
        /// <param name="wnd">PE的窗体句柄</param>
        private void proxyDraw(IntPtr wnd)
        {
            IntPtr handle = new IntPtr(0);
            handle = FindWindowEx(wnd, handle, "TPageControl", "");
            if (handle != IntPtr.Zero)
            {
                SendMessage(handle, WM_SETFOCUS, 0, 0);
                SendMessage(handle, WM_LBUTTONDOWN, MK_LBUTTON, MAKELONG(190, 9));  //网站吸取的按钮在(190,9）这个相对坐标
                SendMessage(handle, WM_LBUTTONUP, MK_LBUTTON, MAKELONG(190, 9));

                //找到选择全部的按钮
                IntPtr TTabSheet = new IntPtr(0);
                TTabSheet = FindWindowEx(handle, TTabSheet, "TTabSheet", "网站吸取");
                IntPtr TCoolBar = new IntPtr(0);
                TCoolBar = FindWindowEx(TTabSheet, TCoolBar, "TCoolBar", "");
                IntPtr ToolBar2 = new IntPtr(0);
                ToolBar2 = FindWindowEx(TCoolBar, ToolBar2, "TToolBar", "");
                IntPtr TCheckBox = new IntPtr(0);
                TCheckBox = FindWindowEx(ToolBar2, TCheckBox, "TCheckBox", "选择全部");
                SendMessage(TCheckBox, WM_CLICK, 0, 0);
                SendMessage(ToolBar2, WM_LBUTTONDOWN, MK_LBUTTON, MAKELONG(10, 10));  //开始的按钮在ToolBar2的(10,10）这个相对坐标
                SendMessage(ToolBar2, WM_LBUTTONUP, MK_LBUTTON, MAKELONG(10, 10));

            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            checkAndRestartPE();
        } 

        /// <summary>
        /// 判断并重启PE
        /// </summary>
        private void checkAndRestartPE()
        {
            IntPtr wnd = new IntPtr(0);
            wnd = FindWindow("Tfrm_MainForm", "ProxyExpress 代理快车");

            if (wnd == IntPtr.Zero)
            {
                //已经停止
                Process proxyProcess = new Process();
                proxyProcess.StartInfo.FileName = textBox1.Text;
                proxyProcess.Start();
                button1.Text = "正在重启";
                while (wnd == IntPtr.Zero)
                {
                    Thread.Sleep(1000);
                    wnd = FindWindow("Tfrm_MainForm", "ProxyExpress 代理快车");
                }

                int count = (int)timer1.Tag;
                count++;
                timer1.Tag = count;
                button1.Text = "已重启" + count.ToString() + "次";

                if (checkBox1.Checked)
                {
                    proxyCollect(wnd);
                }
                if (checkBox2.Checked)
                {
                    proxyDraw(wnd);
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
        const int BM_CLICK = 0x111;


        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, uint wParam, uint lParam);

        //C#中引用PostMessage函数的方法：向指定hwnd的窗体或者控件发送命令消息
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        //C#中引用GetMenu函数的方法：得到指定窗口的主菜单
        [DllImport("user32.dll", EntryPoint = "GetMenu")]
        private static extern int GetMenu(int hwnd);

        //C#中引用GetSubMenu函数的方法：得到指定主菜单的子菜单
        [DllImport("user32.dll", EntryPoint = "GetSubMenu")]
        private static extern int GetSubMenu(int hMenu, int nPos);

        //C#中引用GetMenuItemID函数的方法：得到指定子菜单的ID号
        [DllImport("user32.dll", EntryPoint = "GetMenuItemID")]
        private static extern int GetMenuItemID(int hMenu, int nPos);

        private static int MAKELONG(ushort x, ushort y) 
        {
            return (x + (y<<16)); //low order WORD 是指标的x位置； high order WORD是y位置. 
        }

        
    }
}
