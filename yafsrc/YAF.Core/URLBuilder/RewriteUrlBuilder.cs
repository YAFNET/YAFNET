/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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

using YAF.Core.Syndication;
using YAF.Types.Constants;

namespace YAF.Core
{
  #region Using

  using System;
  using System.Data;
  using System.Globalization;
  using System.Text;
  using System.Web;
  using System.Web.Caching;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Utils;
  using YAF.Utils.Helpers.StringUtils;

  #endregion

  /// <summary>
  /// The rewrite url builder.
  /// </summary>
  public class RewriteUrlBuilder : BaseUrlBuilder
  {
    #region Constants and Fields

    /// <summary>
    /// The cache size.
    /// </summary>
    private int _cacheSize = 500;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets CacheSize.
    /// </summary>
    protected int CacheSize
    {
      get
      {
        return this._cacheSize;
      }

      set
      {
        if (this._cacheSize > 0)
        {
          this._cacheSize = value;
        }
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The build url.
    /// </summary>
    /// <param name="url">
    /// The url.
    /// </param>
    /// <returns>
    /// The build url.
    /// </returns>
    public override string BuildUrl(string url)
    {
      string newUrl = "{0}{1}?{2}".FormatWith(AppPath, Config.ForceScriptName ?? ScriptName, url);

      // create scriptName
      string scriptName = "{0}{1}".FormatWith(AppPath, Config.ForceScriptName ?? ScriptName);

      // get the base script file from the config -- defaults to, well, default.aspx :)
      string scriptFile = Config.BaseScriptFile;

      if (scriptName.EndsWith(scriptFile))
      {
        string before = scriptName.Remove(scriptName.LastIndexOf(scriptFile));

        var parser = new SimpleURLParameterParser(url);

        // create "rewritten" url...
        newUrl = before + Config.UrlRewritingPrefix;

        string useKey = string.Empty;
        string useKey2 = string.Empty;
        string description = string.Empty;
        string pageName = parser["g"];
        bool isFeed = false;
       // const bool showKey = false;
        bool handlePage = false;

        switch (parser["g"])
        {
          case "topics":
            useKey = "f";
            description = this.GetForumName(parser[useKey].ToType<int>());
            handlePage = true;
            break;
          case "posts":
            if (parser["t"].IsSet())
            {
              useKey = "t";
              pageName += "t";
              description = this.GetTopicName(parser[useKey].ToType<int>());
            }
            else if (parser["m"].IsSet())
            {
              useKey = "m";
              pageName += "m";

              try
              {
                  description = this.GetTopicNameFromMessage(parser[useKey].ToType<int>());
              }
              catch (Exception)
              {
                  description = "posts";
              }
            }
                handlePage = true;
            break;
          case "profile":
            useKey = "u";

            // description = GetProfileName( Convert.ToInt32( parser [useKey] ) );
            break;
          case "forum":
            if (parser["c"].IsSet())
            {
              useKey = "c";
              description = this.GetCategoryName(parser[useKey].ToType<int>());
            }
            break;
          case "rsstopic":
            if (parser["pg"].IsSet())
            {
                useKey = "pg";
               // pageName += "pg";
                if (parser[useKey].ToType<int>() == YafRssFeeds.Active.ToInt())
                {
                    description = "active";
                }
                if (parser[useKey].ToType<int>() == YafRssFeeds.Favorite.ToInt())
                {
                    description = "favorite";
                }
                if (parser[useKey].ToType<int>() == YafRssFeeds.Forum.ToInt())
                {
                    description = "forum";
                }
                if (parser[useKey].ToType<int>() == YafRssFeeds.LatestAnnouncements.ToInt())
                {
                    description = "latestannouncements";
                }
                if (parser[useKey].ToType<int>() == YafRssFeeds.LatestPosts.ToInt())
                {
                    description = "latestposts";
                }
                if (parser[useKey].ToType<int>() == YafRssFeeds.Posts.ToInt())
                {
                    description = "posts";
                }
                if (parser[useKey].ToType<int>() == YafRssFeeds.Topics.ToInt())
                {
                    description = "topics";
                } 
            }
            if (parser["f"].IsSet())
            {
                useKey2 = "f";
                description += this.GetForumName(parser[useKey2].ToType<int>());
            }
            if (parser["t"].IsSet())
            {
                useKey2 = "t";
                description += this.GetTopicName(parser[useKey2].ToType<int>());
            }
            if (parser["ft"].IsSet())
            {
                useKey2 = "ft";
                if ((parser[useKey2].ToType<int>()) == YafSyndicationFormats.Atom.ToInt())
                {
                    description += "-atom";
                }
                else
                {
                    description += "-rss";
                }
            }
            handlePage = true;
                isFeed = true;
            break; 
        } 

        newUrl += pageName;

        if (useKey.Length > 0)
        {
            newUrl += parser[useKey];
        }

        if (handlePage && parser["p"] != null && !isFeed)
        {
          int page = parser["p"].ToType<int>();
          if (page != 1)
          {
            newUrl += "p{0}".FormatWith(page);
          }

          parser.Parameters.Remove("p");
        }
        if (isFeed)
        {
            if (parser["ft"] != null)
            {
                int page = parser["ft"].ToType<int>();
                newUrl += "ft{0}".FormatWith(page);
                parser.Parameters.Remove("ft");
            }
            if (parser["f"] != null)
            {
                int page = parser["f"].ToType<int>();
                newUrl += "f{0}".FormatWith(page);
                parser.Parameters.Remove("f");
            }
            if (parser["t"] != null)
            {
                int page = parser["t"].ToType<int>();
                newUrl += "t{0}".FormatWith(page);
                parser.Parameters.Remove("t");
            }
        }

       if (parser["find"] != null)
        {
            newUrl += "find{0}".FormatWith(parser["find"].Trim());
            parser.Parameters.Remove("find");
        } 

          if (description.Length > 0)
        {
            if (description.EndsWith("-"))
            {
                description = description.Remove(description.Length - 1, 1);
            }

          newUrl += "_{0}".FormatWith(description);
        }
        newUrl += ".aspx";
       /*  if (!isFeed)
        {
            newUrl += ".aspx";
        }
        else
        {
            newUrl += ".xml"; 
        } */

          string restURL = parser.CreateQueryString(new[] { "g", useKey });

        // append to the url if there are additional (unsupported) parameters
        if (restURL.Length > 0)
        {
          newUrl += "?{0}".FormatWith(restURL);
        }

        // see if we can just use the default (/)
        if (newUrl.EndsWith("{0}forum.aspx".FormatWith(Config.UrlRewritingPrefix)))
        {
          // remove in favor of just slash...
            newUrl = newUrl.Remove(newUrl.LastIndexOf("{0}forum.aspx".FormatWith(Config.UrlRewritingPrefix), System.StringComparison.Ordinal), "{0}forum.aspx".FormatWith(Config.UrlRewritingPrefix).Length);
        }

        // add anchor
        if (parser.HasAnchor)
        {
          newUrl += "#{0}".FormatWith(parser.Anchor);
        }
      }

      // just make sure & is &amp; ...
      newUrl = newUrl.Replace("&amp;", "&").Replace("&", "&amp;");

      return newUrl;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The high range.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// The high range.
    /// </returns>
    protected int HighRange(int id)
    {
      return (int)(Math.Ceiling((double)(id / this._cacheSize)) * this._cacheSize);
    }

    /// <summary>
    /// The low range.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// The low range.
    /// </returns>
    protected int LowRange(int id)
    {
      return (int)(Math.Floor((double)(id / this._cacheSize)) * this._cacheSize);
    }

    /// <summary>
    /// The clean string for url.
    /// </summary>
    /// <param name="str">
    /// The str.
    /// </param>
    /// <returns>
    /// The clean string for url.
    /// </returns>
    protected static string CleanStringForURL(string str)
    {
        var sb = new StringBuilder();

        // trim...
        str = Config.UrlRewritingMode == "Unicode"
                  ? HttpUtility.UrlDecode(str.Trim())
                  : HttpContext.Current.Server.HtmlDecode(str.Trim());

        // fix ampersand...
        str = str.Replace("&", "and");

        // normalize the Unicode
        str = str.Normalize(NormalizationForm.FormD);

        switch (Config.UrlRewritingMode)
        {
            case "Unicode":
                {
                    foreach (char currentChar in str)
                    {
                        if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                        {
                            sb.Append('-');
                        }
                        else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark &&
                                 !char.IsSymbol(currentChar))
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
                    string strUnidecode = str.Unidecode().Replace(" ", "-");

                    foreach (char currentChar in strUnidecode)
                    {
                        if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                        {
                            sb.Append('-');
                        }
                        else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark &&
                                 !char.IsSymbol(currentChar))
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
                    foreach (char currentChar in str)
                    {
                        if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                        {
                            sb.Append('-');
                        }
                        else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark &&
                                 !char.IsSymbol(currentChar) && currentChar < 128)
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
    /// The get cache name.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// The get cache name.
    /// </returns>
    protected string GetCacheName(string type, int id)
    {
      return @"urlRewritingDT-{0}-Range-{1}-to-{2}".FormatWith(type, this.HighRange(id), this.LowRange(id));
    }

    /// <summary>
    /// The get category name.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// The get category name.
    /// </returns>
    protected string GetCategoryName(int id)
    {
      const string type = "Category";
      const string primaryKey = "CategoryID";
      const string nameField = "Name";

      DataRow row = this.GetDataRowFromCache(type, id);

      if (row == null)
      {
        // get the section desired...
        DataTable list = LegacyDb.category_simplelist(this.LowRange(id), this.CacheSize);

        // set it up in the cache
        row = this.SetupDataToCache(ref list, type, id, primaryKey);

        if (row == null)
        {
          return string.Empty;
        }
      }

      return CleanStringForURL(row[nameField].ToString());
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
    protected DataRow GetDataRowFromCache(string type, int id)
    {
      // get the datatable and find the value
      var list = HttpContext.Current.Cache[this.GetCacheName(type, id)] as DataTable;

      if (list != null)
      {
        DataRow row = list.Rows.Find(id);

        // valid, return...
        if (row != null)
        {
          return row;
        }

          // invalidate this cache section
          HttpContext.Current.Cache.Remove(this.GetCacheName(type, id));
      }

      return null;
    }

    /// <summary>
    /// The get forum name.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// The get forum name.
    /// </returns>
    protected string GetForumName(int id)
    {
      const string type = "Forum";
      const string primaryKey = "ForumID";
      const string nameField = "Name";

      DataRow row = this.GetDataRowFromCache(type, id);

      if (row == null)
      {
        // get the section desired...
        DataTable list = LegacyDb.forum_simplelist(this.LowRange(id), this.CacheSize);

        // set it up in the cache
        row = this.SetupDataToCache(ref list, type, id, primaryKey);

        if (row == null)
        {
          return string.Empty;
        }
      }

      return CleanStringForURL(row[nameField].ToString());
    }

    /// <summary>
    /// The get profile name.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// The get profile name.
    /// </returns>
    protected string GetProfileName(int id)
    {
      const string type = "Profile";
      const string primaryKey = "UserID";
      const string nameField = "Name";

      DataRow row = this.GetDataRowFromCache(type, id);

      if (row == null)
      {
        // get the section desired...
        DataTable list = LegacyDb.user_simplelist(this.LowRange(id), this.CacheSize);

        // set it up in the cache
        row = this.SetupDataToCache(ref list, type, id, primaryKey);

        if (row == null)
        {
          return string.Empty;
        }
      }

      return CleanStringForURL(row[nameField].ToString());
    }

    /// <summary>
    /// The get topic name.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// The get topic name.
    /// </returns>
    protected string GetTopicName(int id)
    {
      const string type = "Topic";
      const string primaryKey = "TopicID";
      const string nameField = "Topic";

      DataRow row = this.GetDataRowFromCache(type, id);

      if (row == null)
      {
        // get the section desired...
        DataTable list = LegacyDb.topic_simplelist(this.LowRange(id), this.CacheSize);

        // set it up in the cache
        row = this.SetupDataToCache(ref list, type, id, primaryKey);

        if (row == null)
        {
          return string.Empty;
        }
      }

      return CleanStringForURL(row[nameField].ToString());
    }

    /// <summary>
    /// The get topic name from message.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// The get topic name from message.
    /// </returns>
    protected string GetTopicNameFromMessage(int id)
    {
      const string type = "Message";
      const string primaryKey = "MessageID";

      DataRow row = this.GetDataRowFromCache(type, id);

      if (row == null)
      {
        // get the section desired...
        DataTable list = LegacyDb.message_simplelist(this.LowRange(id), this.CacheSize);

        // set it up in the cache
        row = this.SetupDataToCache(ref list, type, id, primaryKey);

        if (row == null)
        {
          return string.Empty;
        }
      }

      return this.GetTopicName(row["TopicID"].ToType<int>());
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
    protected DataRow SetupDataToCache(ref DataTable list, string type, int id, string primaryKey)
    {
      DataRow row = null;

      if (list != null)
      {
        list.Columns[primaryKey].Unique = true;
        list.PrimaryKey = new[] { list.Columns[primaryKey] };

        // store it for the future
        var randomValue = new Random();
        HttpContext.Current.Cache.Insert(
          this.GetCacheName(type, id), 
          list, 
          null, 
          DateTime.UtcNow.AddMinutes(randomValue.Next(5, 15)), 
          Cache.NoSlidingExpiration, 
          CacheItemPriority.Low, 
          null);

        // find and return profile..
        row = list.Rows.Find(id);

        if (row == null)
        {
          // invalidate this cache section
          HttpContext.Current.Cache.Remove(this.GetCacheName(type, id));
        }
      }

      return row;
    }

    #endregion
  }
}