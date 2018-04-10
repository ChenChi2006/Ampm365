using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

//using ZXing;
//using ZXing.Common;
//需要zxing.dll
//public bool CreateEANCode(string Code)
//{
//    EncodingOptions encodeOption = new EncodingOptions();
//    encodeOption.Height = 130; // 必须制定高度、宽度
//    encodeOption.Width = 240;

//    // 2.生成条形码图片并保存
//    ZXing.BarcodeWriter wr = new BarcodeWriter();
//    wr.Options = encodeOption;
//    wr.Format = BarcodeFormat.EAN_13; //  条形码规格：EAN13规格：12（无校验位）或13位数字
//    Bitmap img = wr.Write(Code); // 生成图片
//    string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\tmpBmp.bmp";
//    img.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
//    return true;
//}

namespace PosPrint
{
    public class Print
    {
        private string SYSTEM_BROWSER_NAME = "";
        private Print_Nec p1 = null;
        private Print_Other p2 = null;
        private Print_YouAn p3 = null;
        public static bool IsNewInParam()
        {
            string newInParam = "0";
            try
            {
                newInParam = Utils.GetAppConfig("newInParam");
            }
            catch
            {
            }
            return newInParam == "1";
        }

        public Print(int IsNec)
        {
            try
            {
                this.SYSTEM_BROWSER_NAME = Utils.GetAppConfig("browser_name");
            }
            catch
            {
            }
            if (this.SYSTEM_BROWSER_NAME.Trim().Length < 1)
            {
                this.SYSTEM_BROWSER_NAME = "SiriusPosPrint";
            }
            try
            {
                //Print和Print_NET、Print_Other不能使用父子关系。因为如果在宝获利和海信机子上启动OPOSPOSPrinter类库实例，将直接报错。
                if (IsNec == 1)
                {
                    p1 = new Print_Nec();
                    p2 = null;
                    p3 = null;
                }
                else if (IsNec == 0)
                {
                    p1 = null;
                    p2 = new Print_Other();
                    p3 = null;
                }
                else if (IsNec == 2)
                {
                    p1 = null;
                    p2 = null;
                    p3 = new Print_YouAn();
                }
            }
            catch
            {
                p1 = null;
                p2 = new Print_Other();
                p3 = null;
            }
        }

        public string setPrintOrder(string orderInfo)
        {
            try
            {
                LogTools.Info(orderInfo);
                if (p1 != null)
                {
                    return p1.setPrintOrder(orderInfo);
                }
                if (p2 != null)
                {
                    return p2.setPrintOrder(orderInfo);
                }
                if (p3 != null)
                {
                    return p3.setPrintOrder(orderInfo);
                }
            }
            catch(Exception ex)
            {
                LogTools.Debug(ex.ToString());
            }
            return "false";
        }
    }
}
