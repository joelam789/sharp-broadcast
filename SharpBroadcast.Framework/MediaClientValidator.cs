using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharpBroadcast.Framework
{
    public class MediaClientValidator : IClientValidator
    {
        private string m_ValidationURL = "";
        private IServerLogger m_Logger = null;

        public MediaClientValidator(IServerLogger logger = null, string validationURL = "")
        {
            if (logger != null) m_Logger = logger;
            else m_Logger = new ConsoleLogger();

            m_ValidationURL = validationURL;
        }

        public virtual string Validate(string requestPath)
        {
            if (requestPath == null || requestPath.Length <= 0) return "";

            bool isRemoteValidationOK = true;

            if (m_ValidationURL != null && m_ValidationURL.Length > 0)
            {
                isRemoteValidationOK = false;

                try
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(m_ValidationURL);
                    httpWebRequest.ContentType = "text/plain";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(requestPath.Trim());
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        string responseResult = sr.ReadToEnd();
                        if (responseResult != null && responseResult.Trim().ToLower() == "ok")
                        {
                            isRemoteValidationOK = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.Error("Failed to send request to validate client: " + ex.Message);
                }
            }

            if (isRemoteValidationOK == false) return "";

            List<string> paramList = new List<string>();
            string[] parts = requestPath.Split('/');
            foreach (var part in parts)
            {
                if (part.Trim().Length > 0)
                {
                    paramList.Add(part.Trim());
                }
            }
            if (paramList.Count <= 0) return "";

            string sourceName = paramList.First();
            return sourceName;
        }
    }
}
