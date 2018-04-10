using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace PosPrint
{
    internal class Utils
    {
        public static string GetSubString(string str, int length)
        {
            if (str == string.Empty)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            int num = 0;
            int num2 = 0;
            while (num2 < length)
            {
                if (num < str.Length)
                {
                    char c = str[num];
                    if (c > '\u007f')
                    {
                        if (num2 == length - 1)
                        {
                            //如果最后一个字符是中文，因为中文占2位，所以该位不取了。
                            break;
                        }
                        num2 += 2;
                        stringBuilder.Append(c);
                    }
                    else
                    {
                        num2++;
                        stringBuilder.Append(c);
                    }
                }
                else
                {
                    break;
                    //stringBuilder.Append(" ");
                }
                num++;
            }
            return stringBuilder.ToString();
        }

        ///// <summary>
        ///// 此方法还有问题。
        ///// </summary>
        ///// <param name="str"></param>
        ///// <param name="wight"></param>
        ///// <returns></returns>
        //public static string[] GroupBySameWight(string str, int wight)
        //{
        //    if ((!string.IsNullOrEmpty(str)) && wight > 0)
        //    {
        //        List<string> lst = new List<string>();
        //        string tmp = string.Empty;
        //        for (; !string.IsNullOrEmpty(str);)
        //        {
        //            tmp = GetSubString(str, wight);
        //            lst.Add(tmp);
        //            if (str.Length > tmp.Length)
        //            {
        //                str = str.Substring(tmp.Length);
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //        return lst.ToArray();
        //    }
        //    return null;
        //}

        public static string GetAppConfig(string strKey)
        {
            try
            {
                return ConfigurationManager.AppSettings[strKey];
            }
            catch 
            {
                return string.Empty;
            }
        }
        
        /// <summary>
        /// 把子字符串合并到目标字符串的开始或结尾。
        /// 例如：子字符串为"\"，IsBegin=0，目标字符串不论为"c:\mydir\"还是"c:\mydir"，结果都为"c:\mydir\"
        /// </summary>
        /// <param name="Source">目标字符串</param>
        /// <param name="sub">子字符串</param>
        /// <param name="IsBegin">目标字符串的开始</param>
        /// <returns></returns>
        public static string Substitute(string Source, string Sub, int IsBegin = 0)
        {
            if (string.IsNullOrEmpty(Source))
            {
                return string.Empty;
            }
            if (IsBegin == 1)
            {
                if (Source.StartsWith(Sub))
                {
                    return Source;
                }
                else
                {
                    return Source + Sub;
                }
            }
            else
            {
                if (Source.EndsWith(Sub))
                {
                    return Source;
                }
                else
                {
                    return Source + Sub;
                }
            }
        }

        public static bool IsExistsProcess(string ProcessName)
        {
            bool rst = false;
            Process[] processes = Process.GetProcesses();
            //Process process = null;
            foreach (Process p in processes)
            {
                try
                {
                    //这里加if就是因为这两个进程的某些属性一旦访问就抛出没有权限的异常  
                    if (p.ProcessName != "System" && p.ProcessName != "Idle")
                    {
                        if (p.MainModule.FileName == ProcessName)
                        {
                            rst = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return rst;
        }

    }
}
