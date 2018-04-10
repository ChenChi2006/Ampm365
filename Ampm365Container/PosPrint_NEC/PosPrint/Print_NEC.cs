using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using OposPOSPrinter_1_11_Lib;
using System.Text;
using System.Drawing;

namespace PosPrint
{
    //NEC驱动打印，直接拼接在字符串内
    //string _1BbC = "\x1B|bC";
    //string _1B1C = "\x1B|1C";
    //string _1B3C = "\x1B|3C";
    //string _1B4C = "\x1B|4C";
    //string _1BN = "\x1B|N";
    //string _1BuC = "\x1B|#uC";
    public class Print_Nec
    {
        private string _1w1h = "\x1B|N";
        private string _1w2h = "\x1B|3C";
        private string _2w2h = "\x1B|4C";
        private int _GoodNameFirstLineLenght = 18;

        private string SYSTEM_BROWSER_NAME = "";

        private string SYS_JS_CMD_PRINT1 = "";

        private string SYS_JS_CMD_PRINT2 = "";

        private OPOSPOSPrinter posPrinter = new OPOSPOSPrinterClass();

        public Print_Nec()
        {
            try
            {
                this.SYSTEM_BROWSER_NAME = Utils.GetAppConfig("browser_name");
                this.SYS_JS_CMD_PRINT1 = Utils.GetAppConfig("QRCode");
                this.SYS_JS_CMD_PRINT2 = Utils.GetAppConfig("barCode");
            }
            catch
            {
            }
            if (this.SYSTEM_BROWSER_NAME.Trim().Length < 1)
            {
                this.SYSTEM_BROWSER_NAME = "SiriusPosPrint";
            }
            if (this.SYS_JS_CMD_PRINT1.Trim().Length < 1)
            {
                this.SYS_JS_CMD_PRINT1 = "print1";
            }
            if (this.SYS_JS_CMD_PRINT2.Trim().Length < 1)
            {
                this.SYS_JS_CMD_PRINT2 = "print2";
            }
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
                if (array[0].Equals(this.SYSTEM_BROWSER_NAME))
                {
                    if (Print.IsNewInParam())
                    {
                        List<PrintLineModel> lines = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PrintLineModel>>(array[1]);
                        this.PrintNewList(lines);
                    }
                    else
                    {
                        ////{\"printType\":\"Print2\",\"printData\":{\"order_id\":\"167523\",\"change_price\":\"0.30\",\"order_sn\":\"722450976000041\",\"shop_sn\":\"B00062\",\"org_price\":\"50.10\",\"allprice\":\"40.12(需补现金0.30元)\",\"total_price\":\"40.12\",\"coupMoney\":\"20.00\",\"pay_name\":\"\",\"allnums\":\"\",\"send_type\":\"6\",\"send_price\":\"8.50\",\"cheap_price\":\"9.98\",\"accept_name\":\"蔡云鹤\",\"address\":\"北京市朝阳区西坝河东里社区3号楼3单元303室\",\"mobile\":\"13718070605\",\"add_time\":\"2017-09-17 20:22:55\",\"hope_time\":\"立即送达\",\"platform\":\"京东到家\",\"buyer_note\":\"#所购商品如遇缺货，您需要：其他商品继续配送（缺货商品退款）\",\"wm_order_id_view\":\"\",\"pingtai\":\"\",\"pingtai2\":\"京东到家\",\"costprice\":\"50.10\",\"wuliu\":\"京东到家\",\"shopname\":\"全时-光熙家园\",\"shoptel\":\"010-56912050\",\"onecode\":\"http://service.ampm365.cn/org/generate/barcode?orderSn=722450976000041&dpi=80&fontSize=7&fileType=image/png&width=2.0\",\"status\":\"2\",\"goods\":[{\"goods_name\":\"03010090       多谷物芝士软欧面包\",\"gb\":\"6932005201820\",\"real_price\":5.4,\"nums\":\"1\"},{\"goods_name\":\"07010227       纯享原味发酵乳230g\",\"gb\":\"6922577726494\",\"real_price\":4.03,\"nums\":\"4\"},{\"goods_name\":\"51010104       统一小浣熊干脆面（烤肉味）\",\"gb\":\"6902447168708\",\"real_price\":1.2,\"nums\":\"1\"},{\"goods_name\":\"07010189       乳此新鲜香蕉牛奶236ml\",\"gb\":\"6950418707579\",\"real_price\":5.2,\"nums\":\"1\"},{\"goods_name\":\"51020301       双汇鸡肉肠65g根\",\"gb\":\"6902890235835\",\"real_price\":1.2,\"nums\":\"1\"},{\"goods_name\":\"27010006       怡达果丹皮200g\",\"gb\":\"6920242100280\",\"real_price\":5.5,\"nums\":\"1\"},{\"goods_name\":\"03010292       炼乳吐司面包\",\"gb\":\"6932005204173\",\"real_price\":5.5,\"nums\":\"1\"}]}}
                        //JObject obj = JObject.Parse(array[1]);
                        //this.PrintOrder(obj);
                    }
                }
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
            if (lines == null || lines.Count == 0)
            {
                throw new Exception("[Pos 1000] 无打印数据！");
            }
            StringBuilder stringBuilder = new StringBuilder("");
            string device_name = Utils.GetAppConfig("device_name");
            int num = this.posPrinter.Open(device_name);
            if (num != 0)
            {
                throw new Exception("[Pos 1001] 设备驱动失败！返回码：" + num.ToString());
            }
            try
            {
                LogTools.Info("开始打印");
                #region 初始化
                try
                {
                    this.posPrinter.ClaimDevice(1000);
                    this.posPrinter.DeviceEnabled = true;
                    this.posPrinter.RecLineChars = 32;
                    this.posPrinter.RecLetterQuality = true;
                    this.posPrinter.TransactionPrint(2, 11);
                }
                catch (Exception ex2)
                {
                    throw new Exception(string.Format("[Pos 1002] 打印初始化失败：{0}。", ex2.ToString()));
                }
                #endregion
                #region 循环打印
                for (int i = 0; i < lines.Count; i++)
                {
                    PrintLineModel m = lines[i];
#if DEBUG
                    if (m.type == 1)
                    {
                        m.type = 2;
                        m.content = "1234567890123";
                    }
#endif
                    #region 手动换行（而且要先打换行），如果不这么做，打印图像或高倍字体会重叠
                    if (i == 0)
                    {
                        //第一行就不用前置打换行了。顶头即可。
                        m.splitLine = 1;
                    }
                    for (int j = 0; j < m.splitLine - 1; j++)
                    {
                        this.posPrinter.PrintNormal(2, " " + Environment.NewLine);
                    }
                    #endregion
                    switch (m.type)
                    {
                        case 1:
                            #region 打印图片
                            LogTools.Info("打印图片开始");
                            try
                            {
                                this.Print2Bitmap(m.content, "tmpBmp.bmp", m.splitLine);
                            }
                            catch (Exception ex)
                            {
                                string sErr = string.Format("[Pos 1003] 打印图片{0}，下载地址：{2}。\r\n异常：{1}"
                                       , "tmpBmp.bmp", ex.ToString(), m.content);
                                LogTools.Debug(sErr);
                            }
                            LogTools.Info("打印图片结束");
                            #endregion
                            break;
                        case 2:
                            #region 打印条码
                            LogTools.Info("打印条码开始");
                            string data = m.content;
                            int codeWidth = 2;
                            if (m.content.Length> 15)
                            {
                                codeWidth = 1;
                            }
                            num = this.posPrinter.PrintBarCode(2, data, 110, 60, codeWidth, -2, -13);
                            if (num != 0)
                            {
                                LogTools.Info("[Pos 1003] 打印条码失败！ 返回码：" + num.ToString());
                                throw new Exception("[Pos 1003] 打印条码失败！ 返回码：" + num.ToString());
                            }
                            LogTools.Info("打印条码完成");
                            #endregion
                            break;
                        case 3:
                            #region 打文字
                            if (m.fontSize < 2)
                            {
                                m.splitLine = 1;
                            }
                            else
                            {
                                m.splitLine = 2;
                            }
                            StringBuilder sb = new StringBuilder();
                            switch (m.fontSize)
                            {
                                case 1:
                                    sb.Append(_1w1h);
                                    break;
                                case 2:
                                    sb.Append(_1w2h);
                                    break;
                                case 3:
                                    sb.Append(_2w2h);
                                    break;
                                default:
                                    break;
                            }
                            sb.Append(m.content);
                            sb.Append(Environment.NewLine);
                            //LogTools.Info(string.Format("打印第{0}行，内容：{1}", i.ToString(), sb.ToString()));
                            this.posPrinter.PrintNormal(2, sb.ToString());
                            #endregion
                            break;
                        default:
                            break;
                    }
                }
                #endregion
                #region 打印尾巴
                this.posPrinter.PrintNormal(2, " " + Environment.NewLine + " "+ Environment.NewLine + " " + Environment.NewLine
                    + " " + Environment.NewLine + " " + Environment.NewLine + " " + Environment.NewLine + " " + Environment.NewLine
                    + " " + Environment.NewLine + " " + Environment.NewLine + " " + Environment.NewLine);
                this.posPrinter.CutPaper(100);
                LogTools.Info("打印完毕，切纸");
                #endregion
                return "";

            }
            catch (Exception ex4)
            {
                throw new Exception("[Pos 1005] 打印失败，" + ex4.ToString());
            }
            finally
            {
                this.posPrinter.TransactionPrint(2, 12);
                this.posPrinter.RecLetterQuality = false;
                this.posPrinter.DeviceEnabled = false;
                this.posPrinter.ReleaseDevice();
                this.posPrinter.Close();
            }
        }
        public string Print2Bitmap(string url, string fileName, int lineNum)
        {
            try
            {
                string text2 = Utils.Substitute(AppDomain.CurrentDomain.BaseDirectory, "\\") + fileName;
                FileTransferHttp.DownloadPicture(url, text2);
                LogTools.Info("临时图片路径:" + text2);
                Bitmap bmp = new Bitmap(text2);
                try
                {
                    //前置回车
                    for (int i = 0; i < lineNum - 1; i++)
                    {
                        this.posPrinter.PrintNormal(2, " " + Environment.NewLine);
                    }
                    int rst = 0;
                    #region 图片
                    try
                    {
                        rst = this.posPrinter.SetBitmap(1, 2, text2, -11, -2);
                    }
                    catch (Exception ex)
                    {
                        LogTools.Debug(ex.Message);
                    }
                    rst = this.posPrinter.PrintBitmap(2, text2, -11, -2);
                    #endregion
                }
                catch (Exception ex)
                {
                    LogTools.Info("打印图片异常:" + ex.ToString());
                    throw ex;
                }
                finally
                {
                    bmp.Dispose();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return string.Empty;
        }

        //#region 老方法
        //private void PrintOrder(JObject obj)
        //{
        //    string text = (string)obj["printType"];
        //    JObject jObject = (JObject)obj["printData"];
        //    StringBuilder stringBuilder = new StringBuilder("");
        //    string device_name = Utils.GetAppConfig("device_name");
        //    int num = this.posPrinter.Open(device_name);
        //    if (num != 0)
        //    {
        //        throw new Exception("[Pos 1001] 设备驱动失败！返回码：" + num.ToString());
        //    }
        //    this.posPrinter.ClaimDevice(1000);
        //    this.posPrinter.DeviceEnabled = true;
        //    this.posPrinter.TransactionPrint(2, 12);
        //    this.posPrinter.RecLineChars = 32;
        //    try
        //    {
        //        #region 打印条码
        //        LogTools.Info("开始打印条码");
        //        string data = (string)jObject["order_sn"];
        //        num = this.posPrinter.PrintBarCode(2, data, 110, 60, 2, -2, -13);
        //        if (num != 0)
        //        {
        //            //throw new Exception("[Pos 1003] 打印条码失败！ 返回码：" + num.ToString());
        //            LogTools.Info("[Pos 1003] 打印条码失败！ 返回码：" + num.ToString());
        //        }
        //        LogTools.Info("打印条码完成");
        //        #endregion
        //        try
        //        {
        //            stringBuilder.Append(" " + Environment.NewLine);
        //            LogTools.Info("开始打印详情");
        //            stringBuilder.Append(string.Format("欢迎光临 {0}", (string)jObject["shopname"]) + Environment.NewLine);
        //            stringBuilder.Append(string.Format("门店电话：{0}", (string)jObject["shoptel"]) + Environment.NewLine);
        //            stringBuilder.Append(string.Format("************{0}存根************", text.Equals(this.SYS_JS_CMD_PRINT2) ? "客户" : "商户") + Environment.NewLine);
        //            stringBuilder.Append(string.Format("订单编号：{0}", (string)jObject["order_sn"]) + Environment.NewLine);
        //            string text2 = string.Format("收货地址：{0}", (string)jObject["address"]);
        //            int i = Encoding.Default.GetByteCount(text2);
        //            if (i > 32)
        //            {
        //                string text3 = "";
        //                string text4 = text3;
        //                while (i > 32)
        //                {
        //                    text3 = text2.Substring(text4.Trim().Length);
        //                    text3 = Utils.GetSubString(text3, 32);
        //                    stringBuilder.Append(text3 + Environment.NewLine);
        //                    i -= 32;
        //                    text4 += text3;
        //                }
        //                if (i > 0)
        //                {
        //                    text3 = text2.Substring(text4.Trim().Length);
        //                    stringBuilder.Append(text3 + Environment.NewLine);
        //                }
        //            }
        //            else
        //            {
        //                stringBuilder.Append(text2 + Environment.NewLine);
        //            }
        //            stringBuilder.Append(string.Format("顾客姓名：{0}", (string)jObject["accept_name"]) + Environment.NewLine);
        //            stringBuilder.Append(string.Format("顾客电话：{0}", (string)jObject["mobile"]) + Environment.NewLine);
        //            text2 = string.Format("顾客备注：{0}", (string)jObject["buyer_note"]);
        //            i = Encoding.Default.GetByteCount(text2);
        //            if (i > 32)
        //            {
        //                string text5 = "";
        //                string text6 = text5;
        //                while (i > 32)
        //                {
        //                    text5 = text2.Substring(text6.Trim().Length);
        //                    text5 = Utils.GetSubString(text5, 32);
        //                    stringBuilder.Append(text5 + Environment.NewLine);
        //                    i -= 32;
        //                    text6 += text5;
        //                }
        //                if (i > 0)
        //                {
        //                    text5 = text2.Substring(text6.Trim().Length);
        //                    stringBuilder.Append(text5 + Environment.NewLine);
        //                }
        //            }
        //            else
        //            {
        //                stringBuilder.Append(text2 + Environment.NewLine);
        //            }
        //            stringBuilder.Append(string.Format("下单平台：{0}", (string)jObject["pingtai2"]) + Environment.NewLine);
        //            stringBuilder.Append(string.Format("配送方式：{0}", (string)jObject["wuliu"]) + Environment.NewLine);
        //            stringBuilder.Append(string.Format("下单时间：{0}", DateTime.Parse((string)jObject["add_time"]).ToString("yyyy-MM-dd HH:mm:ss")) + Environment.NewLine);
        //            stringBuilder.Append(string.Format("送达时间：{0}", (string)jObject["hope_time"]) + Environment.NewLine);
        //            stringBuilder.Append("********************************" + Environment.NewLine);
        //            stringBuilder.Append("\u3000\u3000\u3000商品\u3000\u3000\u3000\u3000\u3000单价\u3000数量" + Environment.NewLine);
        //            JArray jArray = (JArray)jObject["goods"];
        //            for (int j = 0; j < jArray.Count; j++)
        //            {
        //                JObject jObjectGood = (JObject)jArray[j];
        //                //string txtGoodName = (string)jObjectGood["goods_name"] + " ID:" + (string)jObjectGood["gb"];
        //                string txtGoodName = (string)jObjectGood["goods_name"];//格式或许：123524366236**中文商品名；或许235125616 中文商品名。
        //                string txtPrice = (string)jObjectGood["real_price"];
        //                string txtNums = (string)jObjectGood["nums"];
        //                txtPrice = txtPrice.PadLeft(6);
        //                txtNums = txtNums.PadLeft(6);
        //                #region 新需求
        //                string strGB = txtGoodName.Substring(0, 15).Replace("*", "");
        //                string strGN = txtGoodName.Substring(15);
        //                stringBuilder.Append(strGB + Environment.NewLine);//第一行，只有号
        //                i = Encoding.Default.GetByteCount(strGN);
        //                if (i > _GoodNameFirstLineLenght)
        //                {
        //                    text2 = Utils.GetSubString(strGN, _GoodNameFirstLineLenght);//第2行的商品名
        //                    text2 = text2 + new string(' ', _GoodNameFirstLineLenght - Encoding.Default.GetByteCount(text2));
        //                    stringBuilder.Append(text2 + txtPrice + txtNums + Environment.NewLine);//第2行
        //                    stringBuilder.Append(strGN.Substring(text2.Trim().Length) + Environment.NewLine);
        //                }
        //                else
        //                {
        //                    text2 = Utils.GetSubString(strGN, _GoodNameFirstLineLenght) + new string(' ', _GoodNameFirstLineLenght - i);//第2行的商品名
        //                    stringBuilder.Append(text2 + txtPrice + txtNums + Environment.NewLine);//第2行
        //                }
        //                #endregion
        //            }
        //            stringBuilder.Append("********************************" + Environment.NewLine);
        //            //stringBuilder.Append(string.Format("原价：{0:#.00}", Convert.ToDecimal(jObject["costprice"])) + Environment.NewLine);
        //            //Decimal _artAmt = Convert.ToDecimal(jObject["total_price"]) - Convert.ToDecimal(jObject["Send_price"]);
        //            //   if (_artAmt <= 0)
        //            //{
        //            //    _artAmt = 0;
        //            //}
        //            stringBuilder.Append(string.Format("商品金额：{0:#.00}", Convert.ToDecimal(jObject["total_price"])) + Environment.NewLine);
        //            //stringBuilder.Append(string.Format("优惠：{0}", (string)jObject["cheap_price"]) + Environment.NewLine);
        //            stringBuilder.Append(string.Format("优惠：{0}", (string)jObject["coupMoney"]) + Environment.NewLine);
        //            stringBuilder.Append(string.Format("运费：{0}", (string)jObject["send_price"]) + Environment.NewLine);
        //            stringBuilder.Append(string.Format("实付：{0}", (string)jObject["allprice"]) + Environment.NewLine);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("[Pos 1004] 打印详情失败，" + ex.Message);
        //        }
        //        stringBuilder.Append("********************************" + Environment.NewLine);
        //        stringBuilder.Append(string.Format("出单时间：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) + Environment.NewLine);
        //        stringBuilder.Append(" " + Environment.NewLine);
        //        stringBuilder.Append(" " + Environment.NewLine);
        //        stringBuilder.Append(" " + Environment.NewLine);
        //        stringBuilder.Append(" " + Environment.NewLine);
        //        stringBuilder.Append(" " + Environment.NewLine);
        //        stringBuilder.Append(" " + Environment.NewLine);
        //        stringBuilder.Append(" " + Environment.NewLine);
        //        stringBuilder.Append(" " + Environment.NewLine);
        //        stringBuilder.Append(" " + Environment.NewLine);
        //        stringBuilder.Append(" " + Environment.NewLine);
        //        this.posPrinter.PrintNormal(2, stringBuilder.ToString());

        //        this.posPrinter.CutPaper(100);
        //        LogTools.Info("打印完毕，切纸");
        //    }
        //    catch (Exception ex2)
        //    {
        //        throw new Exception("[Pos 1005] 打印订单失败，" + ex2.Message);
        //    }
        //    finally
        //    {
        //        this.posPrinter.DeviceEnabled = false;
        //        this.posPrinter.ReleaseDevice();
        //        this.posPrinter.Close();
        //    }
        //}
        //#endregion
    }
}
