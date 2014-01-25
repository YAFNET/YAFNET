/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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

namespace YAF.Types.Objects
{
    #region

    using System;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// The Twitter User Profile
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "user", Namespace = "", IsNullable = false)]
    public class TwitterUser
    {
        #region Constants and Fields

        /// <summary>
        /// The description.
        /// </summary>
        private string description;

        /// <summary>
        /// The followers_count.
        /// </summary>
        private string followers_count;

        /// <summary>
        /// The id.
        /// </summary>
        private string id;

        /// <summary>
        /// The location.
        /// </summary>
        private string location;

        /// <summary>
        /// The name.
        /// </summary>
        private string name;

        /// <summary>
        /// The profile_image_url.
        /// </summary>
        private string profile_image_url;

        /// <summary>
        /// The profile_image_url_https.
        /// </summary>
        private string profile_image_url_https;

        /// <summary>
        /// The screen_name.
        /// </summary>
        private string screen_name;

        /// <summary>
        /// The url.
        /// </summary>
        private string url;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [XmlElement(ElementName = "description", Form = XmlSchemaForm.Unqualified)]
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// Gets or sets FollowersCount.
        /// </summary>
        [XmlElement(ElementName = "followers_count", Form = XmlSchemaForm.Unqualified)]
        public int FollowersCount
        {
            get
            {
                return this.followers_count.ToType<int>();
            }

            set
            {
                this.followers_count = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets Location.
        /// </summary>
        [XmlElement(ElementName = "location", Form = XmlSchemaForm.Unqualified)]
        public string Location
        {
            get
            {
                return this.location;
            }

            set
            {
                this.location = value;
            }
        }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [XmlElement(ElementName = "name", Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets ProfileImageUrl.
        /// </summary>
        [XmlElement(ElementName = "profile_image_url", Form = XmlSchemaForm.Unqualified)]
        public string ProfileImageUrl
        {
            get
            {
                return this.profile_image_url;
            }

            set
            {
                this.profile_image_url = value;
            }
        }

        /// <summary>
        /// Gets or sets ProfileImageUrlSecure.
        /// </summary>
        [XmlElement(ElementName = "profile_image_url_https", Form = XmlSchemaForm.Unqualified)]
        public string ProfileImageUrlSecure
        {
            get
            {
                return this.profile_image_url_https;
            }

            set
            {
                this.profile_image_url_https = value;
            }
        }

        /// <summary>
        /// Gets or sets Url.
        /// </summary>
        [XmlElement(ElementName = "url", Form = XmlSchemaForm.Unqualified)]
        public string Url
        {
            get
            {
                return this.url;
            }

            set
            {
                this.url = value;
            }
        }

        /// <summary>
        /// Gets or sets UserId.
        /// </summary>
        [XmlElement(ElementName = "id", Form = XmlSchemaForm.Unqualified)]
        public int UserId
        {
            get
            {
                return this.id.ToType<int>();
            }

            set
            {
                this.id = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets UserName.
        /// </summary>
        [XmlElement(ElementName = "screen_name", Form = XmlSchemaForm.Unqualified)]
        public string UserName
        {
            get
            {
                return this.screen_name;
            }

            set
            {
                this.screen_name = value;
            }
        }

        #endregion
    }
}