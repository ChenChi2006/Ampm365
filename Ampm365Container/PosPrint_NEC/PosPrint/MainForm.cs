using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

namespace PosPrint
{
    [ComVisible(true)]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class MainForm : Form
    {
        ChromiumWebBrowser webCom = null;
        public MainForm()
        {
            InitializeComponent();
            //this.FormBorderStyle = FormBorderStyle.FixedToolWindow; //FixedToolWindow;// 或SizableToolWindow
            try
            {
                string pageUrl = Utils.GetAppConfig("url");
                if (pageUrl.Trim().Length < 1)
                {
                    pageUrl = "http://order.ampm365.cn/";
                }
                if (typeof(ChromiumWebBrowser).Name == "ChromiumWebBrowser")
                {
                    webCom = new ChromiumWebBrowser(pageUrl);
                    webCom.Dock = DockStyle.Fill;
                    this.Controls.Add(webCom);
                    //加载页面
                    webCom.Load(pageUrl);
                }
                else
                {
                    //Settings settings = new Settings();
                    //CEF.Initialize(settings);
                    //BrowserSettings settings2 = new BrowserSettings();
                    ////初始化webview
                    //webCom = new WebView(pageUrl, settings2);
                    //webCom.Dock = DockStyle.Fill;
                    ////加入到父控件
                    //webCom.Parent = this;
                }

                try
                {
                    LogTools.SetConfig(Environment.CurrentDirectory + "\\", "PosPrint.log");
                    LogTools.Info("日志初始化成功...");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("日志初始化失败：" + ex.Message);
                }
                //注册打印服务
                regSvr();
                OpenCashBox_QS ocb = new OpenCashBox_QS();
                if (ocb.IsNec == 2)
                {
                    bool isExists = Utils.IsExistsProcess(Utils.GetAppConfig("uanPosExeFullPath"));
                    if (!isExists)
                    {
                        MessageBox.Show("请先启动Pos主程序，然后再启动接单平台！如果已启动Pos主程序，那么请查看config文件，是否配置正确");
                        this.Dispose(true);
                    }
                }
                Print print = new Print(ocb.IsNec);
                webCom.RegisterJsObject("Print", print);
                webCom.RegisterJsObject("OpenCashBox_QS", ocb);

                //webCom.RegisterJsObject("CallNotifyIcon", new CallNotifyIcon(this));

                //try
                //{
                //    print.CreateEANCode("1234567890123");
                //}
                //catch (Exception ex)
                //{
                //    LogTools.Info("测试生成条形码异常：" + ex.ToString());
                //}

                //#region test
                //string sssss = Utils.GetAppConfig("bmpUrl");
                ////string sssss = "http://bto.oh.quanshishequ.com/order/print/receiptPrint?orderId=54232356057";
                //print.print2Bitmap(sssss);
                //#endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void regSvr()
        {
            string s1 = System.Environment.CurrentDirectory;
            string fullFileName = s1 + "\\Files\\Reg.bat";
            bool IsExists = false;
            if (!System.IO.File.Exists(fullFileName))
            {
                fullFileName = s1 + "\\Ampm365\\Files\\Reg.bat";
                if (!System.IO.File.Exists(fullFileName))
                    IsExists = false;
                else
                    IsExists = true;
            }
            else
            {
                IsExists = true;
            }
            if (IsExists)
            {
                try
                {
                    System.Diagnostics.ProcessStartInfo myStartInfo = new System.Diagnostics.ProcessStartInfo();
                    myStartInfo.FileName = fullFileName;
                    myStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
                    myProcess.StartInfo = myStartInfo;
                    myProcess.Start();
                    myProcess.WaitForExit(3000); //等待程序退出
                    myProcess.Close();
                    myStartInfo = null;
                    myProcess = null;
                }
                catch (Exception ex)
                {
                    LogTools.Debug("注册打印OCX异常：" + ex.Message);
                }
                finally { System.GC.Collect(); }
            }
            else
            {
                LogTools.Debug("注册打印OCX失败：找不到bat文件");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;

            }
        }

        ////关闭按钮不可用
        //private const int CP_NOCLOSE_BUTTON = 0x200;
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams myCp = base.CreateParams;
        //        myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
        //        return myCp;
        //    }
        //}

    }
}
