using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Mahex
{
    public class Mahex_CustomWebRequest : IDisposable
    {
        protected HttpWebResponse _myHttpWebResponse = null;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="URL"></param>
        public Mahex_CustomWebRequest(string URL)
        {
            _URL = URL;
        }

        /// <summary>
        /// Class destructor
        /// </summary>
        ~Mahex_CustomWebRequest()
        {
            this.Dispose();
        }

        //Begin Properties
        private string _URL = string.Empty;
        /// <summary>
        /// Get site URL
        /// </summary>
        public string URL
        {
            get { return _URL; }
        }

        private ArrayList _ParamsCollection = new ArrayList();
        /// <summary>
        /// Parameters collection
        /// </summary>
        public ArrayList ParamsCollection
        {
            get { return _ParamsCollection; }
            set { _ParamsCollection = value; }
        }

        private System.IO.Stream _ResponseStream = null;
        /// <summary>
        /// Response of request data
        /// </summary>
        public System.IO.Stream ResponseStream
        {
            get { return _ResponseStream; }
        }

        //private string _Method = "POST";
        /// <summary>
        /// Request method GET/POST
        /// </summary>
        //public string Method
        //{
        //    get { return _Method; }
        //    set { _Method = value; }

        //}

        private string _ContentType = "application/json;charset=UTF-8";
        /// <summary>
        /// Content type of request
        /// </summary>
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }

        private string _Boundary = "AaB03x";
        /// <summary>
        /// Separator of multipart data
        /// </summary>
        public string Boundary
        {
            get { return _Boundary; }
            set { _Boundary = value; }
        }

        private string _Accept = "iso-8859-1";
        /// <summary>
        /// 
        /// </summary>
        public string Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }

        /// <summary>
        /// Response of request data in string format
        /// </summary>
        public string ResponseString
        {
            get
            {
                string resultData = "";

                StreamReader streamRead = new StreamReader(_ResponseStream);
                Char[] readBuffer = new Char[256];
                // Read from buffer
                int count = streamRead.Read(readBuffer, 0, 256);
                while (count > 0)
                {
                    // get string
                    resultData += new String(readBuffer, 0, count);
                    // Write the data 
                    Console.WriteLine(resultData);
                    // Read from buffer
                    count = streamRead.Read(readBuffer, 0, 256);
                }
                // Release the response object resources.
                streamRead.Close();

                return resultData;
            }
        }
        //End Properties
        //Begin Methods
        /// <summary>
        /// This method send the request to URL
        /// </summary>
        public string PostData(string Auth,string postData,string method = "POST")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Credentials = GetCredential(_URL, Auth,"");
            request.PreAuthenticate = true;
            request.AllowWriteStreamBuffering = true;
            request.ContentType = _ContentType;
            request.Method = method;
            request.Headers.Add("Authorization", $"Basic {Auth}");
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = noCachePolicy;

            if (!string.IsNullOrEmpty(postData))
            {
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] buffer = encoding.GetBytes(postData);
                request.ContentLength = buffer.Length;
                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(buffer, 0, buffer.Length);
                    newStream.Close();
                }
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }
        private CredentialCache GetCredential(string url,string username,string password)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(new System.Uri(url), "Basic", new NetworkCredential(username,password));
            return credentialCache;
        }

        /// <summary>
        /// This method get the data in multipart format.
        /// </summary>
        /// <returns></returns>
        private string GetPostData()
        {
            string boundary = "--" + this._Boundary;

            int arrReqs = this._ParamsCollection.Count * 5;
            string[] auxReqBody = new string[arrReqs];
            int count = 0;

            foreach (ParamsStruct par in this._ParamsCollection)
            {
                auxReqBody[count] = boundary;
                count++;
                switch (par.Type)
                {
                    case ParamsStruct.ParamType.File:
                        {
                            auxReqBody[count] = "Content-Disposition: file; name=\"" + par.Name + "\"; filename=\"" + par.GetOnlyFileName() + "\"";
                            count++;
                            auxReqBody[count] = "Content-Type: text/plain";
                            count++;
                            auxReqBody[count] = "";
                            count++;
                            auxReqBody[count] = par.StringValue;
                            count++;
                            break;
                        }
                    case ParamsStruct.ParamType.Parameter:
                    default:
                        {
                            auxReqBody[count] = "Content-Disposition: form-data; name=\"" + par.Name + "\"";
                            count++;
                            auxReqBody[count] = "";
                            count++;
                            auxReqBody[count] = par.StringValue;
                            count++;
                            break;
                        }
                }

            }

            auxReqBody[count] = boundary;
            count++;

            string requestBody = String.Join("\r\n", auxReqBody);
            return requestBody;
        }
        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            if (_myHttpWebResponse != null)
            {
                _myHttpWebResponse.Close();
                _myHttpWebResponse = null;
            }
        }
        //End Methods

    }

    public class ParamsStruct
    {
        public ParamsStruct(string name, object value)
        {
            _Name = name;
            _Value = value;
        }

        public ParamsStruct(string name, object value, ParamType type, string fileName)
        {
            _Name = name;
            _Value = value;
            _Type = type;
            _FileName = fileName;
        }

        public enum ParamType { File, Parameter };

        private string _Name = string.Empty;
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        private object _Value = null;
        public object Value
        {
            get
            {
                return _Value;
            }
        }

        public string StringValue
        {
            get
            {
                string retVal = string.Empty;

                switch (_Type)
                {
                    case ParamType.File:
                        {
                            Stream auxSt = _Value as Stream;
                            if (auxSt != null)
                            {
                                StreamReader sr = new StreamReader(auxSt);
                                retVal = sr.ReadToEnd();
                            }
                            else
                                retVal = (string)_Value;
                            break;
                        }
                    case ParamType.Parameter:
                        {
                            retVal = (string)_Value;
                            break;
                        }
                    default:
                        {
                            retVal = _Value.ToString();
                            break;
                        }
                }

                return retVal;
            }
        }

        private ParamType _Type = ParamType.Parameter;
        public ParamType Type
        {
            get
            {
                return _Type;
            }
        }

        private string _FileName = string.Empty;
        public string FileName
        {
            get
            {
                return _FileName;
            }
        }

        public string GetOnlyFileName()
        {
            return System.IO.Path.GetFileName(this._FileName);
        }
    }
}
