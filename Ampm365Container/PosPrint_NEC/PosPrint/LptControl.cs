using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PosPrint
{
    internal class LptControl
    {
        private struct OVERLAPPED
        {
            private int Internal;

            private int InternalHigh;

            private int Offset;

            private int OffSetHigh;

            private int hEvent;
        }

        private string LptStr = "lpt1";

        private int iHandle;

        public LptControl(string l_LPT_Str)
        {
            this.LptStr = l_LPT_Str;
        }

        [DllImport("kernel32.dll")]
        private static extern int CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, int lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);

        [DllImport("kernel32.dll")]
        private static extern bool WriteFile(int hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, ref LptControl.OVERLAPPED lpOverlapped);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(int hObject);

        [DllImport("fnthex32.dll")]
        public static extern int GETFONTHEX(string barcodeText, string fontName, int orient, int height, int width, int isBold, int isItalic, StringBuilder returnBarcodeCMD);

        public bool Open()
        {
            this.iHandle = LptControl.CreateFile(this.LptStr, 1073741824u, 0, 0, 3, 0, 0);
            return this.iHandle != -1;
        }

        public bool Write(string Mystring)
        {
            if (this.iHandle != -1)
            {
                LptControl.OVERLAPPED oVERLAPPED = default(LptControl.OVERLAPPED);
                int num = 0;
                byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(Mystring);
                return LptControl.WriteFile(this.iHandle, bytes, bytes.Length, ref num, ref oVERLAPPED);
            }
            throw new Exception("[Pos 3001] 不能连接到打印机!");
        }

        public bool Write(byte[] mybyte)
        {
            if (this.iHandle != -1)
            {
                LptControl.OVERLAPPED oVERLAPPED = default(LptControl.OVERLAPPED);
                int num = 0;
                return LptControl.WriteFile(this.iHandle, mybyte, mybyte.Length, ref num, ref oVERLAPPED);
            }
            throw new Exception("[Pos 3002] 不能连接到打印机!");
        }

        public bool Close()
        {
            return LptControl.CloseHandle(this.iHandle);
        }
    }
}
