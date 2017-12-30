/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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