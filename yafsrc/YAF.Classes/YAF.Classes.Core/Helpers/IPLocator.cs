using System;
using System.Collections;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Net;


namespace YAF.Classes.Core
{
    
    /// <summary>
    /// Summary description for IPLocater
    /// </summary>
    public class IPDetails
    {
        public IPDetails()
        {
        }
        /// <summary>
        /// IP Details From IP Address
        /// </summary>
        /// <param name="ipAddress">string IPAddess </param>
        /// <returns>IPLocator Class</returns>
        public IPLocator GetData(string ipAddress, bool tzInfo)
        {
           IPLocator ipLoc = new IPLocator();
           if (YafContext.Current.BoardSettings.EnableIPInfoService)
            {
                try
                {
                    string path = String.Format(YafContext.Current.BoardSettings.IPLocatorPath, ipAddress,
                                                tzInfo ? "true" : "false");
                    var client = new WebClient();
                    string[] eResult = client.DownloadString(path).ToString().Split(',');
                    if (eResult.Length > 0)
                    {
                        // replace here 
                        object o = Deserialize(eResult[0].ToString());
                        ipLoc = (IPLocator) Deserialize(eResult[0].ToString());
                    }
                }
                catch
                {
                }
            }
            return ipLoc;
        }
        /// <summary>
        /// Deserialize XML String
        /// </summary>
        /// <param name="pXmlizedString"></param>
        /// <returns></returns>
        private Object Deserialize(String pXmlizedString)
        {
            XmlSerializer xs = new XmlSerializer(typeof(IPLocator));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            return xs.Deserialize(memoryStream);
        }
        private Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }
       
    }
    [XmlRootAttribute(ElementName = "Response", IsNullable = false)]
    public class IPLocator
    {
        public IPLocator()
        { }

        private string isdst;
        public string Isdst
        {
            get { return isdst; }
            set { isdst = value; }
        }
        private string gmtoffset;

        public string Gmtoffset
        {
            get { return gmtoffset; }
            set { gmtoffset = value; }
        }


        private string timezone;

        public string TimezoneName
        {
            get { return timezone; }
            set { timezone = value; }
        }


        private string longitude;

        public string Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }


        private string latitude;

        public string Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        private string zip;

        public string Zip
        {
            get { return zip; }
            set { zip = value; }
        }



        private string city;

        public string City
        {
            get { return city; }
            set { city = value; }
        }


        private string regionname;

        public string RegionName
        {
            get { return regionname; }
            set { regionname = value; }
        }

        private string regioncode;

        public string RegionCode
        {
            get { return regioncode; }
            set { regioncode = value; }
        }

        private string countryname;

        public string CountryName
        {
            get { return countryname; }
            set { countryname = value; }
        }


        private string countrycode;

        public string CountryCode
        {
            get { return countrycode; }
            set { countrycode = value; }
        }


        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        private string ip;

        public string IP
        {
            get { return ip; }
            set { ip = value; }
        }

    }
}
