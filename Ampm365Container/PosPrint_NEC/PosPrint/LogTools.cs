using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Xml;

namespace PosPrint
{
	public class LogTools
	{
		public static ILog logger = LogManager.GetLogger("logger");

		private static string logXmlFilePath = Environment.CurrentDirectory + "\\Files\\log4Net.xml";

		public static void SetConfig(string configPath, string logName)
		{
			LogTools.SaveLogFilePath(configPath + logName);
			FileInfo configFile = new FileInfo(LogTools.logXmlFilePath);
			XmlConfigurator.Configure(configFile);
        }

		public static void Debug(string msg, Exception e)
		{
			if (LogTools.logger != null && LogTools.logger.IsDebugEnabled)
			{
				LogTools.logger.Debug(msg, e);
			}
		}

		public static void Debug(string msg)
		{
			LogTools.Debug(msg, null);
		}

		public static void Error(string msg, Exception err)
		{
			if (LogTools.logger != null && LogTools.logger.IsErrorEnabled)
			{
				LogTools.logger.Error(msg, err);
			}
		}

		public static void Warn(string msg)
		{
			if (LogTools.logger != null && LogTools.logger.IsWarnEnabled)
			{
				LogTools.logger.Warn(msg);
			}
		}

		public static void Info(string msg)
		{
			if (LogTools.logger != null && LogTools.logger.IsInfoEnabled)
			{
				LogTools.logger.Info(msg);
			}
		}

		public static void SaveLogFilePath(string path)
		{
            if (!File.Exists(LogTools.logXmlFilePath))
            {
                LogTools.logXmlFilePath = Environment.CurrentDirectory + "\\Ampm365\\Files\\log4Net.xml"; ;
            }
            else if (!File.Exists(LogTools.logXmlFilePath))
			{
				LogTools.logXmlFilePath = "C:\\log4net.xml";
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(LogTools.logXmlFilePath);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("log4net");
			XmlNode xmlNode2 = xmlNode.SelectSingleNode("appender");
			XmlNode xmlNode3 = xmlNode2.SelectSingleNode("file");
			xmlNode3.Attributes["value"].Value = path;
			xmlDocument.Save(LogTools.logXmlFilePath);
		}
	}
}
