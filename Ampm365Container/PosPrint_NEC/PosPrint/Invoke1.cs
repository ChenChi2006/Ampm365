using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosPrint
{
    public class Invoke1
    {
        public static int HS_OK = 0xf0;
        public static int HS_ERROR = 0xff;

        public static string sDllPath = Utils.GetAppConfig("uanDllFullPath");
        public static DllInvoke _dllInvoke = null;
        public static DllInvoke GetDllInvoke()
        {
            if (_dllInvoke == null)
            {
                if (string.IsNullOrEmpty(sDllPath))
                {
                    sDllPath = String.Format(@"{0}pos_ad_dll.dll", Utils.Substitute(AppDomain.CurrentDomain.BaseDirectory, @"\"));
                }
                _dllInvoke = new DllInvoke(sDllPath);
            }
            return _dllInvoke;
        }

        public delegate int OpenPrinter();
        public static int OpenPrinter0()
        {
            OpenPrinter func = (OpenPrinter)GetDllInvoke().Invoke("OpenPrinter", typeof(OpenPrinter));
            return func();
        }

        public delegate int ClosePrinterEx();
        public static int ClosePrinterEx0()
        {
            ClosePrinterEx func = (ClosePrinterEx)GetDllInvoke().Invoke("ClosePrinterEx", typeof(ClosePrinterEx));
            return func();
        }

        public static void MyDispose()
        {
            _dllInvoke.MyDispose();
            _dllInvoke = null;
        }

        public delegate int BeginPrint(int printType);
        public static int BeginPrint0(int printType)
        {
            BeginPrint func = (BeginPrint)GetDllInvoke().Invoke("BeginPrint", typeof(BeginPrint));
            return func(printType);
        }
        public delegate int PrintText(StringBuilder content, int FontSize);
        public static int PrintText0(StringBuilder content, int FontSize)
        {
            PrintText func = (PrintText)GetDllInvoke().Invoke("PrintText", typeof(PrintText));
            return func(content, FontSize);
        }
        public delegate int PrintTextByPaperWidth(StringBuilder content, int FontSize, int PaperWidth);
        public static int PrintTextByPaperWidth0(StringBuilder content, int FontSize, int PaperWidth)
        {
            PrintTextByPaperWidth func = (PrintTextByPaperWidth)GetDllInvoke().Invoke("PrintTextByPaperWidth", typeof(PrintTextByPaperWidth));
            return func(content, FontSize, PaperWidth);
        }
        public delegate int PrintBitmapFile(StringBuilder fileName, int FontSize);
        public static int PrintBitmapFile0(StringBuilder fileName, int LabelAngle)
        {
            PrintBitmapFile func = (PrintBitmapFile)GetDllInvoke().Invoke("PrintBitmapFile", typeof(PrintBitmapFile));
            return func(fileName, LabelAngle);
        }
        public delegate int PrintMiddleBitmapFile(StringBuilder fileName, int FontSize);
        public static int PrintMiddleBitmapFile0(StringBuilder fileName, int LabelAngle)
        {
            PrintMiddleBitmapFile func = (PrintMiddleBitmapFile)GetDllInvoke().Invoke("PrintMiddleBitmapFile", typeof(PrintMiddleBitmapFile));
            return func(fileName, LabelAngle);
        }
    }
}
