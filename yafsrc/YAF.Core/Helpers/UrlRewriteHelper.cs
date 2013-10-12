/* Yet Another Forum.net
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

namespace YAF.Core.Helpers
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using System.Web.Caching;

    using YAF.Classes;
    using YAF.Classes.Data;
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
        private static int _cacheSize = 500;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets CacheSize.
        /// </summary>
        protected static int CacheSize
        {
            get
            {
                return _cacheSize;
            }

            set
            {
                if (_cacheSize > 0)
                {
                    _cacheSize = value;
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
            var list = YafContext.Current.GetRepository<Category>().Simplelist(LowRange(id), CacheSize);

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
            var list = LegacyDb.forum_simplelist(LowRange(id), CacheSize);

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
        public static string GetProfileName(int id)
        {
            const string Type = "Profile";
            const string PrimaryKey = "UserID";
            const string NameField = "Name";

            var row = GetDataRowFromCache(Type, id);

            if (row != null)
            {
                return CleanStringForURL(row[NameField].ToString());
            }

            // get the section desired...
            var list = LegacyDb.user_simplelist(LowRange(id), CacheSize);

            // set it up in the cache
            row = SetupDataToCache(ref list, Type, id, PrimaryKey);

            return row == null ? string.Empty : CleanStringForURL(row[NameField].ToString());
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
            const string TSype = "Topic";
            const string PrimaryKey = "TopicID";
            const string NameField = "Topic";

            var row = GetDataRowFromCache(TSype, id);

            if (row != null)
            {
                return CleanStringForURL(row[NameField].ToString());
            }

            // get the section desired...
            var list = LegacyDb.topic_simplelist(LowRange(id), CacheSize);

            // set it up in the cache
            row = SetupDataToCache(ref list, TSype, id, PrimaryKey);

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
            var list = LegacyDb.message_simplelist(LowRange(id), CacheSize);

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

            // trim...
            inputString = Config.UrlRewritingMode == "Unicode"
                      ? HttpUtility.UrlDecode(inputString.Trim())
                      : HttpContext.Current.Server.HtmlDecode(inputString.Trim());

            // fix ampersand...
            inputString = inputString.Replace("&", "and");

            // normalize the Unicode
            inputString = inputString.Normalize(NormalizationForm.FormD);

            switch (Config.UrlRewritingMode)
            {
                case "Unicode":
                    {
                        foreach (char currentChar in inputString)
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
                        }

                        string strNew = sb.ToString();

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

                        foreach (char currentChar in strUnidecode)
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
                        }

                        string strNew = sb.ToString();

                        while (strNew.EndsWith("-"))
                        {
                            strNew = strNew.Remove(strNew.Length - 1, 1);
                        }

                        return strNew.Length.Equals(0) ? "Default" : strNew;
                    }

                default:
                    {
                        foreach (char currentChar in inputString)
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
                        }

                        string strNew = sb.ToString();

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
            return (Math.Ceiling((id / _cacheSize).ToType<double>()) * _cacheSize).ToType<int>();
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
            return (Math.Floor((id / _cacheSize).ToType<double>()) * _cacheSize).ToType<int>();
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
            return @"urlRewritingDT-{0}-Range-{1}-to-{2}".FormatWith(type, HighRange(id), LowRange(id));
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
            // get the datatable and find the value
            var list = HttpContext.Current.Cache[GetCacheName(type, id)] as DataTable;

            if (list == null)
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