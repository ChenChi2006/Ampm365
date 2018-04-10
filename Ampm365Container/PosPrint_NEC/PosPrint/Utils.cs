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
                            //������һ���ַ������ģ���Ϊ����ռ2λ�����Ը�λ��ȡ�ˡ�
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
        ///// �˷����������⡣
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
        /// �����ַ����ϲ���Ŀ���ַ����Ŀ�ʼ���β��
        /// ���磺���ַ���Ϊ"\"��IsBegin=0��Ŀ���ַ�������Ϊ"c:\mydir\"����"c:\mydir"�������Ϊ"c:\mydir\"
        /// </summary>
        /// <param name="Source">Ŀ���ַ���</param>
        /// <param name="sub">���ַ���</param>
        /// <param name="IsBegin">Ŀ���ַ����Ŀ�ʼ</param>
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
                    //�����if������Ϊ���������̵�ĳЩ����һ�����ʾ��׳�û��Ȩ�޵��쳣  
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
