using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Drawing;
namespace PosPrint
{
    //LPT打印
    //this.lc.Write(new byte[] { 29, 33, 16 });//中英文倍宽模式 
    //this.lc.Write(new byte[] { 29, 33, 17 });//中英文倍高倍宽模式 
    //this.lc.Write(new byte[] { 29, 33, 0 }); // 中英文正常大小 
    //this.lc.Write(new byte[] { 29, 33, 1 });//中英文倍高模式 
    public class Print_Other
    {
        private string _1w1h = new string(new char[] { (char)29, (char)33, (char)0 });
        private string _1w2h = new string(new char[] { (char)29, (char)33, (char)1 });
        private string _2w2h = new string(new char[] { (char)29, (char)33, (char)17 });
        private int _GoodNameFirstLineLenght = 18;
        private string SYSTEM_BROWSER_NAME = "";
        private string SYS_JS_CMD_PRINT1 = "";
        private string SYS_JS_CMD_PRINT2 = "";
        private LptControl lc;
        public Print_Other()
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
            string appConfig = Utils.GetAppConfig("LptStr");
            this.lc = new LptControl(appConfig);
            if (!this.lc.Open())
            {
                throw new Exception("[Pos 1001] 设备驱动失败！");
            }
            try
            {
                LogTools.Info("开始打印");
                #region 循环打印
                for (int i = 0; i < lines.Count; i++)
                {
                    PrintLineModel m = lines[i];

                    ////LPT打印，不用手动隔行，因为它不会重叠


                    switch (m.type)
                    {
                        case 1:
                        //图片和条形码都视作图片打印
                        case 2:
                            this.lc.Write(new byte[]
                            {
                        27,
                        64
                            });
                            this.lc.Write(new byte[]
                            {
                        27,
                        33,
                        48
                            });
                            this.lc.Write(new byte[]
                            {
                        27,
                        97,
                        1
                            });
                            //打印图片或条形码
                            try
                            {
                                this.Print2Bitmap(m.content, "tmpBmp.bmp", m.splitLine);
                            }
                            catch (Exception ex)
                            {
                                string sErr = string.Format("[Pos 1003] 打印图片{0}，下载地址：{2}。\r\n异常：{1}"
                                    , "tmpBmp.bmp", ex.ToString(), m.content);
                                //throw new Exception();
                                LogTools.Debug(sErr);
                            }

                            LptControl arg_164_0 = this.lc;
                            byte[] array = new byte[3];
                            array[0] = 27;
                            array[1] = 33;
                            arg_164_0.Write(array);
                            LptControl arg_186_0 = this.lc;
                            byte[] array2 = new byte[3];
                            array2[0] = 27;
                            array2[1] = 97;
                            arg_186_0.Write(array2);
                            //this.lc.Write(Environment.NewLine);
                            break;
                        case 3:
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
                            sb.Append(m.content + Environment.NewLine);
                            this.lc.Write(sb.ToString());
                            //LogTools.Info(string.Format("打印第{0}行，内容：{1}", i.ToString(), sb.ToString()));
                            for (int j = 0; j < m.splitLine - 2; i++)
                            {
                                this.lc.Write(" " + Environment.NewLine);
                            }
                            break;
                        default:
                            break;
                    }
                }
                #endregion
                #region 打印尾巴
                this.lc.Write(" " + Environment.NewLine);
                this.lc.Write(" " + Environment.NewLine);
                this.lc.Write(" " + Environment.NewLine);
                this.lc.Write(" " + Environment.NewLine);
                this.lc.Write(" " + Environment.NewLine);
                this.lc.Write(" " + Environment.NewLine);
                this.lc.Write(new byte[]
                {
                    27,
                    100,
                    3
                });
                this.lc.Write(new byte[]
                {
                    27,
                    109
                });
                LogTools.Info("打印完毕，切纸");
                #endregion
                return "";
            }
            catch (Exception ex4)
            {
                throw new Exception("[Pos 1005] 打印失败，" + ex4.Message);
            }
            finally
            {
                this.lc.Close();
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
                    this.PrintCode(bmp);
                }
                catch (Exception ex2)
                {
                    throw ex2;
                }
                finally
                {
                    bmp.Dispose();
                }
                for (int i = 0; i < lineNum - 1; i++)
                {
                    this.lc.Write(" " + Environment.NewLine);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                LogTools.Info("打印图片异常:" + ex.ToString());
                throw ex;
            }
        }
        public void PrintCode(Bitmap bmp)
        {
            if (true)
            {
                //设置字符行间距为n点行
                //byte[] data = new byte[] { 0x1B, 0x33, 0x00 };
                string send = "" + (char)(27) + (char)(51) + (char)(0);
                byte[] data = new byte[send.Length];
                for (int i = 0; i < send.Length; i++)
                {
                    data[i] = (byte)send[i];
                }
                lc.Write(data);

                data[0] = (byte)'\x00';
                data[1] = (byte)'\x00';
                data[2] = (byte)'\x00';    // Clear to Zero.

                Color pixelColor;


                //ESC * m nL nH d1…dk   选择位图模式
                // ESC * m nL nH
                byte[] escBmp = new byte[] { 0x1B, 0x2A, 0x00, 0x00, 0x00 };

                escBmp[2] = (byte)'\x21';

                //nL, nH
                escBmp[3] = (byte)(bmp.Width % 256);
                escBmp[4] = (byte)(bmp.Width / 256);

                //循环图片像素打印图片
                //循环高
                for (int i = 0; i < (bmp.Height / 24 + 1); i++)
                {
                    //设置模式为位图模式
                    lc.Write(escBmp);
                    //循环宽
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        for (int k = 0; k < 24; k++)
                        {
                            if (((i * 24) + k) < bmp.Height)  // if within the BMP size
                            {
                                pixelColor = bmp.GetPixel(j, (i * 24) + k);
                                if (pixelColor.R == 0)
                                {
                                    data[k / 8] += (byte)(128 >> (k % 8));

                                }
                            }
                        }
                        //一次写入一个data，24个像素
                        lc.Write(data);

                        data[0] = (byte)'\x00';
                        data[1] = (byte)'\x00';
                        data[2] = (byte)'\x00';    // Clear to Zero.
                    }

                    //换行，打印第二行
                    byte[] data2 = { 0xA };
                    lc.Write(data2);
                } // data
                lc.Write("\n\n");
            }
            else
            {
                byte[] array = new byte[3];
                array[0] = 27;
                array[1] = 51;
                byte[] array2 = array;
                this.lc.Write(array2);
                array2[0] = 0;
                array2[1] = 0;
                array2[2] = 0;
                byte[] array3 = new byte[]
                {
                27,
                42,
                33,
                0,
                0
                };
                array3[3] = (byte)(bmp.Width % 256);
                array3[4] = (byte)(bmp.Width / 256);
                for (int i = 0; i < bmp.Height / 24 + 1; i++)
                {
                    this.lc.Write(array3);
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        for (int k = 0; k < 24; k++)
                        {
                            if (i * 24 + k < bmp.Height && bmp.GetPixel(j, i * 24 + k).R == 0)
                            {
                                byte[] expr_B2_cp_0 = array2;
                                int expr_B2_cp_1 = k / 8;
                                expr_B2_cp_0[expr_B2_cp_1] += (byte)(128 >> k % 8);
                            }
                        }
                        this.lc.Write(array2);
                        array2[0] = 0;
                        array2[1] = 0;
                        array2[2] = 0;
                    }
                    byte[] mybyte = new byte[]
                    {
                    10
                    };
                    this.lc.Write(mybyte);
                }
            }
        }

        //#region 老方法
        //private void PrintOrder(JObject obj)
        //{
        //    string text = (string)obj["printType"];
        //    JObject jObject = (JObject)obj["printData"];
        //    string appConfig = Utils.GetAppConfig("LptStr");
        //    this.lc = new LptControl(appConfig);
        //    if (!this.lc.Open())
        //    {
        //        throw new Exception("[Pos 1001] 设备驱动失败！");
        //    }
        //    try
        //    {
        //        try
        //        {
        //            string webReqUrl = (string)jObject["onecode"];
        //            string text2 = "";
        //            try
        //            {
        //                text2 = Environment.CurrentDirectory + "\\code.bmp";
        //                LogTools.Debug("图片临时路径：" + text2);
        //                FileTransferHttp.DownloadPicture(webReqUrl, text2);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("[Pos 1002] 下载图片" + ex.Message);
        //            }
        //            this.lc.Write(new byte[]
        //            {
        //                27,
        //                64
        //            });
        //            this.lc.Write(new byte[]
        //            {
        //                27,
        //                33,
        //                48
        //            });
        //            this.lc.Write(new byte[]
        //            {
        //                27,
        //                97,
        //                1
        //            });
        //            LogTools.Info("开始打印条码");
        //            Bitmap bmp = new Bitmap(text2);
        //            this.PrintCode(bmp);
        //            bmp.Dispose();

        //            LogTools.Info("打印条码完成");
        //        }
        //        catch (Exception ex2)
        //        {
        //            //throw new Exception("[Pos 1003] 打印条码" + ex2.Message);
        //            LogTools.Info(string.Format("[Pos 1003] 打印条码失败：{0}。", ex2.ToString()));
        //            LogTools.Info("继续...");
        //        }
        //        try
        //        {
        //            LptControl arg_164_0 = this.lc;
        //            byte[] array = new byte[3];
        //            array[0] = 27;
        //            array[1] = 33;
        //            arg_164_0.Write(array);
        //            LptControl arg_186_0 = this.lc;
        //            byte[] array2 = new byte[3];
        //            array2[0] = 27;
        //            array2[1] = 97;
        //            arg_186_0.Write(array2);
        //            this.lc.Write(Environment.NewLine);
        //            LogTools.Info("开始打印详情");
        //            this.lc.Write(string.Format("欢迎光临 {0}", (string)jObject["shopname"]) + Environment.NewLine);
        //            this.lc.Write(string.Format("门店电话：{0}", (string)jObject["shoptel"]) + Environment.NewLine);
        //            this.lc.Write(string.Format("************{0}存根************", text.Equals(this.SYS_JS_CMD_PRINT2) ? "客户" : "商户") + Environment.NewLine);
        //            this.lc.Write(string.Format("订单编号：{0}", (string)jObject["order_sn"]) + Environment.NewLine);
        //            this.lc.Write(string.Format("收货地址：{0}", (string)jObject["address"]) + Environment.NewLine);
        //            this.lc.Write(string.Format("顾客姓名：{0}", (string)jObject["accept_name"]) + Environment.NewLine);
        //            this.lc.Write(string.Format("顾客电话：{0}", (string)jObject["mobile"]) + Environment.NewLine);
        //            this.lc.Write(string.Format("顾客备注：{0}", (string)jObject["buyer_note"]) + Environment.NewLine);
        //            this.lc.Write(string.Format("下单平台：{0}", (string)jObject["pingtai2"]) + Environment.NewLine);
        //            this.lc.Write(string.Format("配送方式：{0}", (string)jObject["wuliu"]) + Environment.NewLine);
        //            DateTime dateTime = DateTime.Parse((string)jObject["add_time"]);
        //            this.lc.Write(string.Format("下单时间：{0}", dateTime.ToString("yyyy-MM-dd HH:mm:ss")) + Environment.NewLine);
        //            this.lc.Write(string.Format("送达时间：{0}", (string)jObject["hope_time"]) + Environment.NewLine);
        //            this.lc.Write("********************************" + Environment.NewLine);
        //            this.lc.Write("\u3000\u3000\u3000商品\u3000\u3000\u3000\u3000\u3000单价\u3000数量" + Environment.NewLine);
        //            JArray jArray = (JArray)jObject["goods"];
        //            for (int i = 0; i < jArray.Count; i++)
        //            {
        //                JObject jObjectGood = (JObject)jArray[i];
        //                //string txtGoodName = (string)jObjectGood["goods_name"] + " ID:" + (string)jObjectGood["gb"];
        //                string txtGoodName = (string)jObjectGood["goods_name"];
        //                string txtPrice = (string)jObjectGood["real_price"];
        //                string txtNums = (string)jObjectGood["nums"];
        //                StringBuilder stringBuilder = new StringBuilder();
        //                txtPrice = txtPrice.PadLeft(6);
        //                txtNums = txtNums.PadLeft(6);
        //                #region 新需求
        //                string strGB = txtGoodName.Substring(0, 15).Replace("*", "");
        //                string strGN = txtGoodName.Substring(15);
        //                this.lc.Write(strGB + Environment.NewLine);//第一行，只有号
        //                int goodNameLenght = Encoding.Default.GetByteCount(strGN);
        //                if (goodNameLenght > _GoodNameFirstLineLenght)
        //                {
        //                    string text2 = Utils.GetSubString(strGN, _GoodNameFirstLineLenght);//第2行的商品名
        //                    text2 = text2 + new string(' ', _GoodNameFirstLineLenght - Encoding.Default.GetByteCount(text2));
        //                    this.lc.Write(string.Concat(text2, txtPrice, txtNums, Environment.NewLine));//第2行
        //                    this.lc.Write(strGN.Substring(text2.Trim().Length) + Environment.NewLine);//第3行
        //                }
        //                else
        //                {
        //                    string text2 = Utils.GetSubString(strGN, _GoodNameFirstLineLenght) + new string(' ', _GoodNameFirstLineLenght - goodNameLenght);//第2行的商品名
        //                    this.lc.Write(string.Concat(text2, txtPrice, txtNums, Environment.NewLine));//第2行
        //                }
        //                #endregion
        //            }
        //            this.lc.Write("********************************" + Environment.NewLine);
        //            //Decimal _artAmt = Convert.ToDecimal(jObject["total_price"]) - Convert.ToDecimal(jObject["Send_price"]);
        //            //if (_artAmt <= 0)
        //            //{
        //            //    _artAmt = 0;
        //            //}

        //            //this.lc.Write(string.Format("原价：{0:#.00}", Convert.ToDecimal(jObject["costprice"])) + Environment.NewLine);
        //            //this.lc.Write(string.Format("优惠：{0}", (string)jObject["cheap_price"]) + Environment.NewLine);
        //            this.lc.Write(string.Format("商品金额：{0:#.00}", Convert.ToDecimal(jObject["total_price"])) + Environment.NewLine);
        //            this.lc.Write(string.Format("优惠：{0}", (string)jObject["coupMoney"]) + Environment.NewLine);
        //            this.lc.Write(string.Format("运费：{0}", (string)jObject["send_price"]) + Environment.NewLine);
        //            this.lc.Write(string.Format("实付：{0}", (string)jObject["allprice"]) + Environment.NewLine);
        //        }
        //        catch (Exception ex3)
        //        {
        //            throw new Exception("[Pos 1004] 打印详情失败，" + ex3.Message);
        //        }
        //        this.lc.Write("********************************" + Environment.NewLine);
        //        this.lc.Write(string.Format("出单时间：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) + Environment.NewLine);
        //        this.lc.Write(" " + Environment.NewLine);
        //        this.lc.Write(" " + Environment.NewLine);
        //        this.lc.Write(" " + Environment.NewLine);
        //        this.lc.Write(" " + Environment.NewLine);
        //        this.lc.Write(" " + Environment.NewLine);
        //        this.lc.Write(" " + Environment.NewLine);
        //        this.lc.Write(new byte[]
        //        {
        //            27,
        //            100,
        //            3
        //        });
        //        this.lc.Write(new byte[]
        //        {
        //            27,
        //            109
        //        });
        //        LogTools.Info("打印完毕，切纸");
        //    }
        //    catch (Exception ex4)
        //    {
        //        throw new Exception("[Pos 1005] 打印订单失败，" + ex4.Message);
        //    }
        //    finally
        //    {
        //        this.lc.Close();
        //    }
        //}
        //#endregion

    }
}
