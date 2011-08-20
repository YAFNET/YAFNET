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

using YAF.Classes.Pattern;

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
      public ThreadSafeDictionary<string, string> GetData([CanBeNull] string ip, [CanBeNull] string format, bool callback, string culture, string browser, string os)
    {
      CodeContracts.ArgumentNotNull(ip, "ip");
         
      ThreadSafeDictionary<string, string> res = new ThreadSafeDictionary<string, string>();
      if (YafContext.Current.BoardSettings.IPLocatorResultsMapping.IsNotSet() || YafContext.Current.BoardSettings.IPLocatorUrlPath.IsNotSet()) return res;
      
      if (YafContext.Current.BoardSettings.EnableIPInfoService)
      {
        try
        {
          string path = YafContext.Current.BoardSettings.IPLocatorUrlPath.FormatWith(Utils.Helpers.IPHelper.GetIp4Address(ip));
          var client = new WebClient();
          string[] eResult = client.DownloadString(path).Split(';');
          string[] sray = YafContext.Current.BoardSettings.IPLocatorResultsMapping.Trim().Split(',');
          if (eResult.Length > 0 && eResult.Length == sray.Length)
          {
              int i = 0;
              foreach (string str in eResult)
              {
                  res.Add(sray[i].Trim(), str);
                  i++;
              }
          }
        }
        catch
        {
        }
      }

      return res;
    }

    #endregion
 
  }

  /// <summary>
  /// The ip locator.
  /// </summary>
  [XmlRootAttribute(ElementName = "Response", IsNullable = false)]
  public class IpLocator
  {
      #region Constants and Fields

      /// <summary>
      /// The city.
      /// </summary>
      private string cityName;

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
      private string ipAddress;

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
      private string regionName;

      /// <summary>
      /// The status.
      /// </summary>
      private string statusCode;

      /// <summary>
      /// The status message.
      /// </summary>
      private string statusMessage;

      /// <summary>
      /// The timezone.
      /// </summary>
      private string timeZone;

      /// <summary>
      /// The zipCode.
      /// </summary>
      private string zipCode;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets Status.
      /// </summary>
      public string Status
      {
          get
          {
              return this.statusCode;
          }

          set
          {
              this.statusCode = value;
          }
      }

      /// <summary>
      /// Gets or sets Status.
      /// </summary>
      public string StatusMessage
      {
          get
          {
              return this.statusMessage;
          }

          set
          {
              this.statusMessage = value;
          }
      }

      /// <summary>
      /// Gets or sets IP.
      /// </summary>
      public string IP
      {
          get
          {
              return this.ipAddress;
          }

          set
          {
              this.ipAddress = value;
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
      /// Gets or sets RegionName.
      /// </summary>
      public string RegionName
      {
          get
          {
              return this.regionName;
          }

          set
          {
              this.regionName = value;
          }
      }

      /// <summary>
      /// Gets or sets City.
      /// </summary>
      public string City
      {
          get
          {
              return this.cityName;
          }

          set
          {
              this.cityName = value;
          }
      }

      /// <summary>
      /// Gets or sets Zip.
      /// </summary>
      public string Zip
      {
          get
          {
              return this.zipCode;
          }

          set
          {
              this.zipCode = value;
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
      /// Gets or sets TimezoneName.
      /// </summary>
      public string TimezoneName
      {
          get
          {
              return this.timeZone;
          }

          set
          {
              this.timeZone = value;
          }
      }

      #endregion
  }
}