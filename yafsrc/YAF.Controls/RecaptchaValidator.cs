namespace YAF.Controls
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Web;

    public class RecaptchaValidator
    {
        private string challenge;
        private string privateKey;
        private string remoteIp;
        private string response;
        private const string VerifyUrl = "http://api-verify.recaptcha.net/verify";

        private void CheckNotNull(object obj, string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public RecaptchaResponse Validate()
        {
            string[] strArray;
            this.CheckNotNull(this.PrivateKey, "PrivateKey");
            this.CheckNotNull(this.RemoteIP, "RemoteIp");
            this.CheckNotNull(this.Challenge, "Challenge");
            this.CheckNotNull(this.Response, "Response");
            if ((this.challenge == string.Empty) || (this.response == string.Empty))
            {
                return RecaptchaResponse.InvalidSolution;
            }
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://api-verify.recaptcha.net/verify");
            request.ProtocolVersion = HttpVersion.Version10;
            request.Timeout = 0x7530;
            request.Method = "POST";
            request.UserAgent = "reCAPTCHA/ASP.NET";
            request.ContentType = "application/x-www-form-urlencoded";
            string s = string.Format("privatekey={0}&remoteip={1}&challenge={2}&response={3}", new object[] { HttpUtility.UrlEncode(this.PrivateKey), HttpUtility.UrlEncode(this.RemoteIP), HttpUtility.UrlEncode(this.Challenge), HttpUtility.UrlEncode(this.Response) });
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (TextReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        strArray = reader.ReadToEnd().Split(new char[0]);
                    }
                }
            }
            catch (WebException exception)
            {
                EventLog.WriteEntry("Application", exception.Message, EventLogEntryType.Error);
                return RecaptchaResponse.RecaptchaNotReachable;
            }
            switch (strArray[0])
            {
                case "true":
                    return RecaptchaResponse.Valid;

                case "false":
                    return new RecaptchaResponse(false, strArray[1]);
            }
            throw new InvalidProgramException("Unknown status response.");
        }

        public string Challenge
        {
            get
            {
                return this.challenge;
            }
            set
            {
                this.challenge = value;
            }
        }

        public string PrivateKey
        {
            get
            {
                return this.privateKey;
            }
            set
            {
                this.privateKey = value;
            }
        }

        public string RemoteIP
        {
            get
            {
                return this.remoteIp;
            }
            set
            {
                IPAddress address = IPAddress.Parse(value);
                if ((address == null) || ((address.AddressFamily != AddressFamily.InterNetwork) && (address.AddressFamily != AddressFamily.InterNetworkV6)))
                {
                    throw new ArgumentException("Expecting an IP address, got " + address);
                }
                this.remoteIp = address.ToString();
            }
        }

        public string Response
        {
            get
            {
                return this.response;
            }
            set
            {
                this.response = value;
            }
        }
    }
}

