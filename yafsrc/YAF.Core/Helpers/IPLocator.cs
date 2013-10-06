/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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

    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Xml.Serialization;
    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

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
        /// <param name="ip">
        /// The ip.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <param name="browser">
        /// The browser.
        /// </param>
        /// <param name="os">
        /// The os.
        /// </param>
        /// <returns>
        /// IPLocator Class
        /// </returns>
        public IDictionary<string, string> GetData([CanBeNull] string ip, [CanBeNull] string format, bool callback, string culture, string browser, string os)
        {
            CodeContracts.VerifyNotNull(ip, "ip");

            IDictionary<string, string> res = new ConcurrentDictionary<string, string>();

            if (YafContext.Current.Get<YafBoardSettings>().IPLocatorResultsMapping.IsNotSet() ||
                YafContext.Current.Get<YafBoardSettings>().IPLocatorUrlPath.IsNotSet())
            {
                return res;
            }

            if (!YafContext.Current.Get<YafBoardSettings>().EnableIPInfoService)
            {
                return res;
            }

            try
            {
                string path = YafContext.Current.Get<YafBoardSettings>().IPLocatorUrlPath.FormatWith(Utils.Helpers.IPHelper.GetIp4Address(ip));
                var client = new WebClient();
                string[] result = client.DownloadString(path).Split(';');
                string[] sray = YafContext.Current.Get<YafBoardSettings>().IPLocatorResultsMapping.Trim().Split(',');
                if (result.Length > 0 && result.Length == sray.Length)
                {
                    int i = 0;
                    foreach (string str in result)
                    {
                        res.Add(sray[i].Trim(), str);
                        i++;
                    }
                }
            }
            catch
            {
                return res;
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
      /// The gmtoffset.
      /// </summary>
      private string gmtoffset;

      /// <summary>
      /// The isdst.
      /// </summary>
      private string isdst;

      /// <summary>
      /// The regioncode.
      /// </summary>
      private string regioncode;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets Status.
      /// </summary>
      public string Status { get; set; }

      /// <summary>
      /// Gets or sets Status.
      /// </summary>
      public string StatusMessage { get; set; }

      /// <summary>
      /// Gets or sets IP.
      /// </summary>
      public string IP { get; set; }

      /// <summary>
      /// Gets or sets CountryCode.
      /// </summary>
      public string CountryCode { get; set; }

      /// <summary>
      /// Gets or sets CountryName.
      /// </summary>
      public string CountryName { get; set; }

      /// <summary>
      /// Gets or sets RegionName.
      /// </summary>
      public string RegionName { get; set; }

      /// <summary>
      /// Gets or sets City.
      /// </summary>
      public string City { get; set; }

      /// <summary>
      /// Gets or sets Zip.
      /// </summary>
      public string Zip { get; set; }

      /// <summary>
      /// Gets or sets Latitude.
      /// </summary>
      public string Latitude { get; set; }

      /// <summary>
      /// Gets or sets Longitude.
      /// </summary>
      public string Longitude { get; set; }

      /// <summary>
      /// Gets or sets Time zone Name.
      /// </summary>
      public string TimezoneName { get; set; }

      #endregion
  }
}