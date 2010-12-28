/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Utils.Helpers
{
  #region Using

  using System;
  using System.Linq;
  using System.Web;

  using YAF.Utils;
  using YAF.Utils.Helpers.StringUtils;
  using YAF.Types;

  #endregion

  /// <summary>
  /// Helper for Figuring out the User Agent.
  /// </summary>
  public static class UserAgentHelper
  {
    #region Public Methods

    /// <summary>
    /// Is this user agent IE v6?
    /// </summary>
    /// <returns>
    /// The is browser i e 6.
    /// </returns>
    public static bool IsBrowserIE6()
    {
      return HttpContext.Current.Request.Browser.Browser.Contains("IE") &&
             HttpContext.Current.Request.Browser.Version.StartsWith("6.");
    }

    /// <summary>
    /// Validates if the useragent owner is a feed reader
    /// </summary>
    /// <param name="userAgent">
    /// </param>
    /// <returns>
    /// The is feed reader.
    /// </returns>
    public static bool IsFeedReader([CanBeNull] string userAgent)
    {
      string[] agentContains = { "Windows-RSS-Platform", "FeedDemon", "Feedreader", "Apple-PubSub" };

      return userAgent.IsSet()
               ? agentContains.Any(
                 agentContain => userAgent.ToLowerInvariant().Contains(agentContain.ToLowerInvariant()))
               : false;
    }

    /// <summary>
    /// Validates if the useragent is a known ignored UA string
    /// </summary>
    /// <param name="userAgent">
    /// </param>
    /// <returns>
    /// The true if the UA string patterrn should not be displayed in active users.
    /// </returns>
    public static bool IsIgnoredForDisplay([CanBeNull] string userAgent)
    {
      if (userAgent.IsSet())
      {
        // Apple-PubSub - Safary RSS reader
        string[] stringContains = { "PlaceHolder" };

        return stringContains.Any(x => userAgent.ToLowerInvariant().Contains(x.ToLowerInvariant()));
      }

      return false;
    }

    /// <summary>
    /// Tests if the user agent is a mobile device.
    /// </summary>
    /// <param name="userAgent">
    /// </param>
    /// <returns>
    /// The is mobile device.
    /// </returns>
    public static bool IsMobileDevice([CanBeNull] string userAgent)
    {
      string[] mobileContains = {
                                  "iphone", "ipad", "midp", "windows ce", "android", "blackberry", "opera mini", "mobile", "palm", 
                                  "portable", "webos",  "htc", "armv7l", "lg/u"
                                };

      return userAgent.IsSet() && mobileContains.Any(s => userAgent.ToLowerInvariant().Contains(s));
    }

    /// <summary>
    /// Validates if the useragent is a search engine spider
    /// </summary>
    /// <param name="userAgent">
    /// </param>
    /// <returns>
    /// The is search engine spider.
    /// </returns>
    public static bool IsSearchEngineSpider([CanBeNull] string userAgent)
    {
      if (userAgent.IsSet())
      {
        string[] spiderContains = {
                                    "abachoBOT", "abcdatos_botlink", "ah-ha.com crawler", "antibot", "appie", 
                                    "AltaVista-Intranet", "Acoon Robot", "Atomz", "Arachnoidea", "AESOP_com_SpiderMan", 
                                    "AxmoRobot", "ArchitextSpider", "AlkalineBOT", "Aranha", "asterias", 
                                    "Buscaplus Robi", "CanSeek", "ChristCRAWLER", "Clushbot", "Crawler", "CrawlerBoy", 
                                    "DeepIndex", "DefaultCrawler", "DittoSpyder", "DIIbot", "EZResult", "EARTHCOM.info", 
                                    "EuripBot", "ESISmartSpider", "FAST-WebCrawler", "FyberSearch", "Findexa Crawler", 
                                    "Fluffy", "Googlebot", "geckobot", "GenCrawler", "GeonaBot", "getRAX", "Gulliver", 
                                    "Hubater", "ia_archiver", "Slurp", "Scooter", "Mercator", "RaBot", "Jack", 
                                    "Speedy Spider", "moget", "Toutatis", "IlTrovatore-Setaccio", "IncyWincy", 
                                    "UltraSeek", "InfoSeek Sidewinder", "Mole2", "MP3Bot", "Knowledge.com", "kuloko-bot"
                                    , "LNSpiderguy", "Linknzbot", "lookbot", "MantraAgent", "NetResearchServer", "Lycos"
                                    , "JoocerBot", "HenryTheMiragoRobot", "MojeekBot", "mozDex", "MSNBOT", 
                                    "Navadoo Crawler", "ObjectsSearch", "OnetSzukaj", "PicoSearch", "PJspider", 
                                    "nttdirectory_robot", "maxbot.com", "Openfind", "psbot", "QweeryBot", "StackRambler"
                                    , "SeznamBot", "Search-10", "Scrubby", "speedfind ramBot xtreme", "Kototoi", 
                                    "SearchByUsa", "Searchspider", "SightQuestBot", "Spider_Monkey", "Surfnomore", 
                                    "teoma", "UK Searcher Spider", "Nazilla", "MuscatFerret", "ZyBorg", 
                                    "WIRE WebRefiner", "WSCbot", "Yandex", "Yellopet-Spider", "YBSbot", "OceanSpiders", 
                                    "MozSpider"
                                  };
        return spiderContains.Any(x => userAgent.ToLowerInvariant().Contains(x.ToLowerInvariant()));
      }

      return false;
    }

    /// <summary>
    /// Returns a platform user friendly name.
    /// </summary>
    /// <param name="userAgent">
    /// </param>
    /// <param name="isCrawler">
    /// </param>
    /// <param name="platform">
    /// </param>
    /// <param name="isSearchEngine">
    /// </param>
    /// <param name="isIgnoredForDisplay">
    /// </param>
    public static void Platform([CanBeNull] string userAgent, bool isCrawler, [NotNull] ref string platform, out bool isSearchEngine, out bool isIgnoredForDisplay)
    {
      CodeContracts.ArgumentNotNull(platform, "platform");

      isSearchEngine = false;
      isIgnoredForDisplay = false;

      if (userAgent.IsNotSet())
      {
        platform = "[Empty]";
        isIgnoredForDisplay = true;

        return;
      }

      if (userAgent.IndexOf("Windows NT 6.1") >= 0)
      {
        platform = "Win7";
      }
      else if (userAgent.IndexOf("Windows NT 6.0") >= 0)
      {
        platform = "Vista";
      }
      else if (userAgent.IndexOf("Windows NT 5.1") >= 0)
      {
        platform = "WinXP";
      }
      else if (userAgent.IndexOf("Linux") >= 0)
      {
        platform = "Linux";
      }
      else if (userAgent.IndexOf("Windows NT 5.2") >= 0)
      {
        platform = "Win2003";
      }
      else if (userAgent.IndexOf("FreeBSD") >= 0)
      {
        platform = "FreeBSD";
      }
      else if (userAgent.IndexOf("iPad") >= 0)
      {
        platform = "iPad(iOS)";
      }
      else if (userAgent.IndexOf("iPhone") >= 0)
      {
        platform = "iPhone(iOS)";
      }
      else if (userAgent.IndexOf("iPod") >= 0)
      {
        platform = "iPod(iOS)";
      }
      else if (userAgent.IndexOf("WindowsMobile") >= 0)
      {
        platform = "WindowsMobile";
      }
      else if (userAgent.IndexOf("webOS") >= 0)
      {
        platform = "WebOS";
      }
      else if (userAgent.IndexOf("Android") >= 0)
      {
        platform = "Android";
      }
      else
      {
        // check if it's a search engine spider or an ignored UI string...
        isSearchEngine = (!isCrawler) ? IsSearchEngineSpider(userAgent) : true;
        isIgnoredForDisplay = IsIgnoredForDisplay(userAgent) | isSearchEngine;
      }
    }

    #endregion
  }
}