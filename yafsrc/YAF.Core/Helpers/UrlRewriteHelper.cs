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

namespace YAF.Core.Helpers
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils.Helpers.StringUtils;

    /// <summary>
    /// URL Rewriter Helper Class
    /// </summary>
    public class UrlRewriteHelper
    {
        #region Constants and Fields
        

        /// <summary>
        /// The cache size.
        /// </summary>
        private static int cacheSize = 500;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets CacheSize.
        /// </summary>
        protected static int CacheSize
        {
            get => cacheSize;

            set
            {
                if (cacheSize > 0)
                {
                    cacheSize = value;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the name of the category.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The get category name.
        /// </returns>
        public static string GetCategoryName(int id)
        {
            const string Type = "Category";
            const string PrimaryKey = "CategoryID";
            const string NameField = "Name";

            var row = GetDataRowFromCache(Type, id);

            if (row != null)
            {
                return CleanStringForURL(row[NameField].ToString());
            }

            // get the section desired...
            var list = BoardContext.Current.GetRepository<Category>().SimpleListAsDataTable(LowRange(id), CacheSize);

            // set it up in the cache
            row = SetupDataToCache(ref list, Type, id, PrimaryKey);

            return row == null ? string.Empty : CleanStringForURL(row[NameField].ToString());
        }


        /// <summary>
        /// Gets the name of the forum.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The get forum name.
        /// </returns>
        public static string GetForumName(int id)
        {
            const string Type = "Forum";
            const string PrimaryKey = "ForumID";
            const string NameField = "Name";

            var row = GetDataRowFromCache(Type, id);

            if (row != null)
            {
                return CleanStringForURL(row[NameField].ToString());
            }

            // get the section desired...
            var list = BoardContext.Current.GetRepository<Forum>().SimpleListAsDataTable(LowRange(id), CacheSize);

            // set it up in the cache
            row = SetupDataToCache(ref list, Type, id, PrimaryKey);

            return row == null ? string.Empty : CleanStringForURL(row[NameField].ToString());
        }

        /// <summary>
        /// Gets the name of the profile.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The get profile name.
        /// </returns>
        [Obsolete("To reduce db calls use url parameters only")]
        public static string GetProfileName(int id)
        {
            const string Type = "Profile";
            const string PrimaryKey = "UserID";

            string nameField;

            try
            {
                nameField = BoardContext.Current.Get<BoardSettings>().EnableDisplayName ? "DisplayName" : "Name";
            }
            catch (Exception)
            {
                nameField = "Name";
            }

            var row = GetDataRowFromCache(Type, id);

            if (row != null)
            {
                return CleanStringForURL(row[nameField].ToString());
            }

            // get the section desired...
            var list = BoardContext.Current.GetRepository<User>().SimpleListAsDataTable(LowRange(id), CacheSize);

            // set it up in the cache
            row = SetupDataToCache(ref list, Type, id, PrimaryKey);

            return row == null ? string.Empty : CleanStringForURL(row[nameField].ToString());
        }

        /// <summary>
        /// Gets the name of the topic.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The get topic name.
        /// </returns>
        public static string GetTopicName(int id)
        {
            const string Topic = "Topic";
            const string PrimaryKey = "TopicID";
            const string NameField = "Topic";

            var row = GetDataRowFromCache(Topic, id);

            if (row != null)
            {
                return CleanStringForURL(row[NameField].ToString());
            }

            // get the section desired...
            var list = BoardContext.Current.GetRepository<Topic>().SimpleListAsDataTable(LowRange(id), CacheSize);

            // set it up in the cache
            row = SetupDataToCache(ref list, Topic, id, PrimaryKey);

            return row == null ? string.Empty : CleanStringForURL(row[NameField].ToString());
        }

        /// <summary>
        /// Gets the topic name from message.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The get topic name from message.
        /// </returns>
        public static string GetTopicNameFromMessage(int id)
        {
            const string Type = "Message";
            const string PrimaryKey = "MessageID";

            var row = GetDataRowFromCache(Type, id);

            if (row != null)
            {
                return GetTopicName(row["TopicID"].ToType<int>());
            }

            // get the section desired...
            var list = BoardContext.Current.GetRepository<Message>().SimpleListAsDataTable(LowRange(id), CacheSize);

            // set it up in the cache
            row = SetupDataToCache(ref list, Type, id, PrimaryKey);

            return row == null ? string.Empty : GetTopicName(row["TopicID"].ToType<int>());
        }

        /// <summary>
        /// Cleans the string for URL.
        /// </summary>
        /// <param name="inputString">The input String.</param>
        /// <returns>
        /// The clean string for url.
        /// </returns>
        public static string CleanStringForURL(string inputString)
        {
            var sb = new StringBuilder();

            // fix ampersand...
            inputString = inputString.Replace(" & ", "and").Replace("ـ", string.Empty);

            // trim...
            inputString = Config.UrlRewritingMode == "Unicode"
                      ? HttpUtility.UrlDecode(inputString.Trim())
                      : HttpContext.Current.Server.HtmlDecode(inputString.Trim());

            inputString = Regex.Replace(inputString, @"\p{Cs}", string.Empty);

            // normalize the Unicode
            inputString = inputString.Normalize(NormalizationForm.FormD);

            switch (Config.UrlRewritingMode)
            {
                case "Unicode":
                    {
                        inputString.ForEach(
                            currentChar =>
                                {
                                    if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                                    {
                                        sb.Append('-');
                                    }
                                    else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark
                                             && !char.IsSymbol(currentChar))
                                    {
                                        sb.Append(currentChar);
                                    }
                                });
                        
                        var strNew = sb.ToString();

                        while (strNew.EndsWith("-"))
                        {
                            strNew = strNew.Remove(strNew.Length - 1, 1);
                        }

                        return strNew.Length.Equals(0) ? "Default" : HttpUtility.UrlEncode(strNew);
                    }

                case "Translit":
                    {
                        string strUnidecode;

                        try
                        {
                            strUnidecode = inputString.Unidecode().Replace(" ", "-");
                        }
                        catch (Exception)
                        {
                            strUnidecode = inputString;
                        }

                        strUnidecode.ForEach(
                            currentChar =>
                                {
                                    if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                                    {
                                        sb.Append('-');
                                    }
                                    else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark
                                             && !char.IsSymbol(currentChar))
                                    {
                                        sb.Append(currentChar);
                                    }
                                });

                        var strNew = sb.ToString();

                        while (strNew.EndsWith("-"))
                        {
                            strNew = strNew.Remove(strNew.Length - 1, 1);
                        }

                        return strNew.Length.Equals(0) ? "Default" : strNew;
                    }

                default:
                    {
                        inputString.ForEach(
                            currentChar =>
                                {
                                    if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                                    {
                                        sb.Append('-');
                                    }
                                    else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark
                                             && !char.IsSymbol(currentChar) && currentChar < 128)
                                    {
                                        sb.Append(currentChar);
                                    }
                                });

                        var strNew = sb.ToString();

                        while (strNew.EndsWith("-"))
                        {
                            strNew = strNew.Remove(strNew.Length - 1, 1);
                        }

                        return strNew.Length.Equals(0) ? "Default" : strNew;
                    }
            }
        }

        /// <summary>
        /// High range.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The high range.
        /// </returns>
        protected static int HighRange(int id)
        {
            return (Math.Ceiling((id / cacheSize).ToType<double>()) * cacheSize).ToType<int>();
        }

        /// <summary>
        /// Low range.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The low range.
        /// </returns>
        protected static int LowRange(int id)
        {
            return (Math.Floor((id / cacheSize).ToType<double>()) * cacheSize).ToType<int>();
        }

        /// <summary>
        /// Gets the name of the cache.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The get cache name.
        /// </returns>
        protected static string GetCacheName(string type, int id)
        {
            return $@"urlRewritingDT-{type}-Range-{HighRange(id)}-to-{LowRange(id)}";
        }

        /// <summary>
        /// The get data row from cache.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        ///  Cached data Row
        /// </returns>
        protected static DataRow GetDataRowFromCache(string type, int id)
        {
            // get the data table and find the value
            if (!(HttpContext.Current.Cache[GetCacheName(type, id)] is DataTable list))
            {
                return null;
            }

            var row = list.Rows.Find(id);

            // valid, return...
            if (row != null)
            {
                return row;
            }

            // invalidate this cache section
            HttpContext.Current.Cache.Remove(GetCacheName(type, id));

            return null;
        }

        /// <summary>
        /// The setup data to cache.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="primaryKey">
        /// The primary key.
        /// </param>
        /// <returns>
        /// The Data row
        /// </returns>
        protected static DataRow SetupDataToCache(ref DataTable list, string type, int id, string primaryKey)
        {
            if (list == null)
            {
                return null;
            }

            list.Columns[primaryKey].Unique = true;
            list.PrimaryKey = new[] { list.Columns[primaryKey] };

            // store it for the future
            var randomValue = new Random();

            HttpContext.Current.Cache.Insert(
                GetCacheName(type, id),
                list,
                null,
                DateTime.UtcNow.AddMinutes(randomValue.Next(5, 15)),
                Cache.NoSlidingExpiration,
                CacheItemPriority.Low,
                null);

            // find and return profile..
            var row = list.Rows.Find(id);

            if (row == null)
            {
                // invalidate this cache section
                HttpContext.Current.Cache.Remove(GetCacheName(type, id));
            }

            return row;
        }

        #endregion 
    }
}