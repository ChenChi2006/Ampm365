using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Drawing;
namespace PosPrint
{
    public class Print_YouAn
    {
        public Print_YouAn()
        {
        }

        public string setPrintOrder(string orderInfo)
        {
            string result;
            if (string.IsNullOrEmpty(orderInfo))
            {
                return "false";
            }
            try
            {
                int ix = orderInfo.IndexOf('|');
                if (!(ix >= 0 && ix < orderInfo.Length - 1))
                {
                    return "false";
                }
                string[] array = new string[] { orderInfo.Substring(0, ix), orderInfo.Substring(ix + 1) };
                List<PrintLineModel> lines = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PrintLineModel>>(array[1]);
                this.PrintNewList(lines);
                result = "true";
            }
            catch (Exception ex)
            {
                LogTools.Debug(ex.Message);
                result = ex.Message;
            }
            return result;
        }
        private string PrintNewList(List<PrintLineModel> lines)
        {
            //Invoke1.PrintText0(new StringBuilder("1234567890一二三四五六七八九十abdefghijk一二三四五"), 32);
            if (lines == null || lines.Count == 0)
            {
                throw new Exception("[Pos 1000] 无打印数据！");
            }
            try
            {
                LogTools.Info("开始打印");
                for (int i = 0; i < lines.Count; i++)
                {
                    PrintLineModel m = lines[i];
                    switch (m.type)
                    {
                        case 1:
                            //图片
                            Print2Bitmap(m);
                            break;
                        case 2:
                            //有安不支持条形码打印
                            break;
                        case 3:
                            Invoke1.PrintText0(new StringBuilder(m.content + Environment.NewLine), 32);//如果，有config_uan.txt文件。传几都不会有影响。
                            break;
                        default:
                            break;
                    }
                }
                Invoke1.BeginPrint0(8);
                LogTools.Info("打印完毕，切纸");
                return "";
            }
            catch (Exception ex4)
            {
                throw new Exception("[Pos 1005] 打印失败，" + ex4.Message);
            }
            finally
            {
            }
        }
        public bool Print2Bitmap(PrintLineModel m)
        {
            string fileName = "tmpBmp.bmp";
            string text2 = Utils.Substitute(AppDomain.CurrentDomain.BaseDirectory, "\\") + fileName;
            try
            {
                FileTransferHttp.DownloadPicture(m.content, text2);
            }
            catch (Exception ex)
            {
                try
                {
                    FileTransferHttp.GetImageByHttp(m.content, text2);
                }
                catch (Exception ex2)
                {
                    LogTools.Debug(string.Format("[Pos 1003] 图片下载地址：{0}。下载异常两次：1：{1}。2：{2}。", m.content, ex.ToString(), ex2.ToString()));
                    return false;
                }
            }
            try
            {
                Invoke1.PrintMiddleBitmapFile0(new StringBuilder(text2), 0);
            }
            catch (Exception ex2)
            {
                LogTools.Debug(string.Format("[Pos 1003] 打印图片tmpBmp.bmp异常：{0}", ex2.ToString()));
                return false;
            }
            return true;
        }

    }
}
