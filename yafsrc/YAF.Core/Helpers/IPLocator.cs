/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core
{
  #region Using

  using System.IO;
  using System.Net;
  using System.Text;
  using System.Xml;
  using System.Xml.Serialization;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Utils.Helpers.StringUtils;
  using YAF.Types;

  #endregion

  /// <summary>
  /// Summary description for IPLocater
  /// </summary>
  public class IPDetails
  {
    #region Public Methods

    /// <summary>
    /// IP Details From IP Address
    /// </summary>
    /// <param name="ipAddress">
    /// string IPAddess 
    /// </param>
    /// <param name="tzInfo">
    /// The tz Info.
    /// </param>
    /// <returns>
    /// IPLocator Class
    /// </returns>
    public IPLocator GetData([NotNull] string ipAddress, bool tzInfo)
    {
      CodeContracts.ArgumentNotNull(ipAddress, "ipAddress");

      var ipLoc = new IPLocator();
      if (YafContext.Current.BoardSettings.EnableIPInfoService)
      {
        try
        {
          string path = YafContext.Current.BoardSettings.IPLocatorPath.FormatWith(ipAddress, tzInfo ? "true" : "false");
          var client = new WebClient();
          string[] eResult = client.DownloadString(path).Split(',');
          if (eResult.Length > 0)
          {
            // replace here 
            object o = this.Deserialize(eResult[0]);
            ipLoc = (IPLocator)this.Deserialize(eResult[0]);
          }
        }
        catch
        {
        }
      }

      return ipLoc;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deserialize XML String
    /// </summary>
    /// <param name="pXmlizedString">
    /// </param>
    /// <returns>
    /// The deserialize.
    /// </returns>
    private object Deserialize([NotNull] string pXmlizedString)
    {
      CodeContracts.ArgumentNotNull(pXmlizedString, "pXmlizedString");

      var xs = new XmlSerializer(typeof(IPLocator));
      var memoryStream = new MemoryStream(this.StringToUTF8ByteArray(pXmlizedString));
      var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
      return xs.Deserialize(memoryStream);
    }

    /// <summary>
    /// The string to ut f 8 byte array.
    /// </summary>
    /// <param name="pXmlString">
    /// The p xml string.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    private byte[] StringToUTF8ByteArray([NotNull] string pXmlString)
    {
      CodeContracts.ArgumentNotNull(pXmlString, "pXmlString");

      var encoding = new UTF8Encoding();
      byte[] byteArray = encoding.GetBytes(pXmlString);
      return byteArray;
    }

    #endregion
  }

  /// <summary>
  /// The ip locator.
  /// </summary>
  [XmlRootAttribute(ElementName = "Response", IsNullable = false)]
  public class IPLocator
  {
    #region Constants and Fields

    /// <summary>
    /// The city.
    /// </summary>
    private string city;

    /// <summary>
    /// The countrycode.
    /// </summary>
    private string countrycode;

    /// <summary>
    /// The countryname.
    /// </summary>
    private string countryname;

    /// <summary>
    /// The gmtoffset.
    /// </summary>
    private string gmtoffset;

    /// <summary>
    /// The ip.
    /// </summary>
    private string ip;

    /// <summary>
    /// The isdst.
    /// </summary>
    private string isdst;

    /// <summary>
    /// The latitude.
    /// </summary>
    private string latitude;

    /// <summary>
    /// The longitude.
    /// </summary>
    private string longitude;

    /// <summary>
    /// The regioncode.
    /// </summary>
    private string regioncode;

    /// <summary>
    /// The regionname.
    /// </summary>
    private string regionname;

    /// <summary>
    /// The status.
    /// </summary>
    private string status;

    /// <summary>
    /// The timezone.
    /// </summary>
    private string timezone;

    /// <summary>
    /// The zip.
    /// </summary>
    private string zip;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets City.
    /// </summary>
    public string City
    {
      get
      {
        return this.city;
      }

      set
      {
        this.city = value;
      }
    }

    /// <summary>
    /// Gets or sets CountryCode.
    /// </summary>
    public string CountryCode
    {
      get
      {
        return this.countrycode;
      }

      set
      {
        this.countrycode = value;
      }
    }

    /// <summary>
    /// Gets or sets CountryName.
    /// </summary>
    public string CountryName
    {
      get
      {
        return this.countryname;
      }

      set
      {
        this.countryname = value;
      }
    }

    /// <summary>
    /// Gets or sets Gmtoffset.
    /// </summary>
    public string Gmtoffset
    {
      get
      {
        return this.gmtoffset;
      }

      set
      {
        this.gmtoffset = value;
      }
    }

    /// <summary>
    /// Gets or sets IP.
    /// </summary>
    public string IP
    {
      get
      {
        return this.ip;
      }

      set
      {
        this.ip = value;
      }
    }

    /// <summary>
    /// Gets or sets Isdst.
    /// </summary>
    public string Isdst
    {
      get
      {
        return this.isdst;
      }

      set
      {
        this.isdst = value;
      }
    }

    /// <summary>
    /// Gets or sets Latitude.
    /// </summary>
    public string Latitude
    {
      get
      {
        return this.latitude;
      }

      set
      {
        this.latitude = value;
      }
    }

    /// <summary>
    /// Gets or sets Longitude.
    /// </summary>
    public string Longitude
    {
      get
      {
        return this.longitude;
      }

      set
      {
        this.longitude = value;
      }
    }

    /// <summary>
    /// Gets or sets RegionCode.
    /// </summary>
    public string RegionCode
    {
      get
      {
        return this.regioncode;
      }

      set
      {
        this.regioncode = value;
      }
    }

    /// <summary>
    /// Gets or sets RegionName.
    /// </summary>
    public string RegionName
    {
      get
      {
        return this.regionname;
      }

      set
      {
        this.regionname = value;
      }
    }

    /// <summary>
    /// Gets or sets Status.
    /// </summary>
    public string Status
    {
      get
      {
        return this.status;
      }

      set
      {
        this.status = value;
      }
    }

    /// <summary>
    /// Gets or sets TimezoneName.
    /// </summary>
    public string TimezoneName
    {
      get
      {
        return this.timezone;
      }

      set
      {
        this.timezone = value;
      }
    }

    /// <summary>
    /// Gets or sets Zip.
    /// </summary>
    public string Zip
    {
      get
      {
        return this.zip;
      }

      set
      {
        this.zip = value;
      }
    }

    #endregion
  }
}