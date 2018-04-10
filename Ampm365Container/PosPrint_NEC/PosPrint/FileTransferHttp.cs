using System;
using System.IO;
using System.Net;

namespace PosPrint
{
	internal class FileTransferHttp
	{
		public static string GetImageByHttp(string WebReqUrl, string FileName)
		{
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(WebReqUrl);
				httpWebRequest.SendChunked = true;
				httpWebRequest.Method = "get";
				httpWebRequest.Proxy = null;
				httpWebRequest.ContentType = "text/xml; charset=utf-8";
				httpWebRequest.Accept = "*/*";
				httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
				httpWebRequest.Headers.Add("Accept-Language", "zh-cn");
				httpWebRequest.Headers.Add("Accept-CharSet", "utf-8");
				httpWebRequest.UserAgent = "Mozilla/3.0 (compatible; Indy Library)";
				httpWebRequest.Timeout = 20000;
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				if (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.OK)
				{
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						if (File.Exists(FileName))
						{
							File.Delete(FileName);
                        }
						FileStream fileStream = new FileStream(FileName, FileMode.CreateNew, FileAccess.ReadWrite);
						try
						{
							while (true)
							{
								int num = responseStream.ReadByte();
								if (num == -1)
								{
									break;
								}
								fileStream.WriteByte((byte)num);
                            }
                            LogTools.Info(string.Format("下载图片完毕：{0}。", FileName));
                        }
						finally
						{
							fileStream.Close();
						}
					}
				}
				httpWebResponse.Close();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return FileName;
		}

        public static bool DownloadPicture(string picUrl, string savePath)
        {
            string website = Utils.GetAppConfig("url");
            if (website.IndexOf("ohtest.quanshishequ.com") >= 0)
            {
                picUrl = picUrl.Replace("oh.quanshishequ.com", "ohtest.quanshishequ.com");
            }
            bool value = false;
            WebResponse response = null;
            Stream stream = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(picUrl);
                request.Timeout = 20000;
                response = request.GetResponse();
                stream = response.GetResponseStream();
                //LogTools.Info(string.Format("请求：{0}的response.ContentType为：{1}", picUrl, response.ContentType));
                if (!response.ContentType.ToLower().StartsWith("text/"))
                    value = SaveBinaryFile(response, savePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null) stream.Close();
                if (response != null) response.Close();
            }
            return value;
        }
        private static bool SaveBinaryFile(WebResponse response, string savePath)
        {
            bool value = false;
            byte[] buffer = new byte[1024];
            Stream outStream = null;
            Stream inStream = null;
            try
            {
                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                }
                outStream = System.IO.File.Create(savePath);
                inStream = response.GetResponseStream();
                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0) outStream.Write(buffer, 0, l);
                } while (l > 0);
                value = true;
                LogTools.Info(string.Format("下载图片完毕：{0}。", savePath));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (outStream != null) outStream.Close();
                if (inStream != null) inStream.Close();
            }
            return value;
        }
    }
}
