/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Types.Objects
{
    using System.Xml.Serialization;

    using ServiceStack.DataAnnotations;

    /// <summary>
    /// The IP locator.
    /// </summary>
    [XmlRoot(ElementName = "Response", IsNullable = false)]
    public class IpLocator
    {
        #region Properties

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        [Alias("statusCode")]
        public string StatusCode { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        [Alias("statusMessage")]
        public string StatusMessage { get; set; }

        /// <summary>
        /// Gets or sets IP.
        /// </summary>
        [Alias("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets CountryCode.
        /// </summary>
        [Alias("countryCode")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets CountryName.
        /// </summary>
        [Alias("countryName")]
        public string CountryName { get; set; }

        /// <summary>
        /// Gets or sets RegionName.
        /// </summary>
        [Alias("RegionName")]
        public string regionName { get; set; }

        /// <summary>
        /// Gets or sets City.
        /// </summary>
        [Alias("cityName")]
        public string CityName { get; set; }

        /// <summary>
        /// Gets or sets Zip.
        /// </summary>
        [Alias("zipCode")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets Latitude.
        /// </summary>
        [Alias("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// Gets or sets Longitude.
        /// </summary>
        [Alias("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// Gets or sets Time zone Name.
        /// </summary>
        [Alias("timeZone")]
        public string TimeZone { get; set; }

        #endregion
    }
}
