using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OposCashDrawer_1_11_Lib;

namespace PosPrint
{
    public class OpenCashBox_QS
    {
        private OPOSCashDrawerClass cashDrawer1;
        private string cashBox_name;
        public int IsNec = 0;
        public string PosNumInit = string.Empty;
        public OpenCashBox_QS()
        {
            string strIsNec = string.Empty;
            try
            {
                strIsNec = Utils.GetAppConfig("isNec");
            }
            catch
            {
            }
            if (strIsNec == "2")
            {
                IsNec = 2;
                //有安称
                LogTools.Info("最终判断此pos机品牌为有安称，如判断错误会影响后续打印功能。");
            }
            else
            {
                try
                {
                    string posType = string.Empty;
                    int rst = OpenCashBox.OpenCashBox.posType(out posType);
                    LogTools.Info(string.Format("调OpenCashBox.posType的结果：返回值:{0}，posType:{1}。", rst.ToString(), posType));
                    if (posType.ToUpper() == "NEC")
                    {
                        cashDrawer1 = new OPOSCashDrawerClass();
                        IsNec = 1;
                    }
                    else
                    {
                        if (strIsNec == "1")
                        {
                            cashDrawer1 = new OPOSCashDrawerClass();
                            IsNec = 1;
                        }
                    }
                    if (IsNec == 1)
                    {
                        LogTools.Info("最终判断此pos机品牌为NEC，如判断错误会影响后续打印功能。");
                    }
                    else
                    {
                        LogTools.Info("最终判断此pos机品牌不是NEC。如判断错误，会影响后续打印功能，请检查配置文件是否正确");
                    }
                    OpenCashBox.OpenCashBox.posNo(out PosNumInit);
                    LogTools.Info(string.Format("初始化调OpenCashBox.posNo。出参:{0}。", PosNumInit));
                }
                catch (Exception ex)
                {
                    LogTools.Info("最终判断此pos机品牌不是NEC。如判断错误，会影响后续打印功能，请检查配置文件是否正确。弹钱箱初始化异常:" + ex.ToString());
                }
            }
            try
            {
                cashBox_name = Utils.GetAppConfig("cashBox_name");
                if (string.IsNullOrEmpty(cashBox_name))
                {
                    cashBox_name = "NEC.CDW.1";
                }
            }
            catch
            {
                cashBox_name = "NEC.CDW.1";
            }
        }
        public string openCashBox()
        {
            string posNum = string.Empty;
            string posType = string.Empty;
            try
            {
                //nec的机子，此方法不会真正调钱箱方法，只是会用返回的posNum。真正真正调钱箱方法在IsNec的判断里面
                OpenCashBox.OpenCashBox.openCashBox(out posNum, out posType);
                LogTools.Info(string.Format("调OpenCashBox.openCashBox的结果：posNum:{0}，posType:{1}。", posNum, posType));
            }
            catch(Exception ex)
            {
                LogTools.Info("调弹钱箱异常:" + ex.ToString());
            }
            if (IsNec == 1)
            {
                try
                {
                    int num = this.cashDrawer1.Open(cashBox_name);
                    if (num != 0)
                    {
                        cashDrawer1.Close();
                        num = cashDrawer1.Open(cashBox_name);
                        if (num != 0)
                        {
                            throw new Exception("[Pos 2001] 钱箱驱动Open失败！返回码：" + num.ToString());
                        }
                    }
                    int claimResult = cashDrawer1.ClaimDevice(1000);
                    if (claimResult != 0)
                    {
                        throw new Exception("[Pos 2002] 钱箱声明失败！返回码：" + claimResult.ToString());
                    }
                    cashDrawer1.DeviceEnabled = true;
                    cashDrawer1.OpenDrawer();
                    cashDrawer1.DeviceEnabled = false;
                }
                catch (Exception ex)
                {
                    LogTools.Debug("开钱箱异常：" + ex.ToString());
                }
                finally
                {
                    cashDrawer1.ReleaseDevice();
                    cashDrawer1.Close();
                }
            }
            return posNum;
        }
        public int isNec()
        {
            return IsNec;
        }
        public string posNum()
        {
            LogTools.Info(string.Format("调OpenCashBox_QS.posNum。返回:{0}。", PosNumInit));
            return PosNumInit;
        }
    }
}
