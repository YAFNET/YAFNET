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
using System;
using System.Linq;
using System.Web;

namespace YAF.Classes.Utils
{
  /// <summary>
  /// Helper for Figuring out the User Agent.
  /// </summary>
  public static class UserAgentHelper
  {
    /// <summary>
    /// Validates if the useragent is a search engine spider
    /// </summary>
    /// <param name="userAgent">
    /// </param>
    /// <returns>
    /// The is search engine spider.
    /// </returns>
    public static bool IsSearchEngineSpider(string userAgent)
    {
      string[] spiderContains = {
                                  "abachoBOT", "abcdatos_botlink", "ah-ha.com crawler", "antibot", "appie", "AltaVista-Intranet", "Acoon Robot", "Atomz", 
                                  "Arachnoidea","AESOP_com_SpiderMan","AxmoRobot","ArchitextSpider","AlkalineBOT","Aranha", "asterias", "Buscaplus Robi",
                                  "CanSeek", "ChristCRAWLER", "Clushbot", "Crawler", "CrawlerBoy", "DeepIndex","DefaultCrawler", "DittoSpyder", "DIIbot", 
                                  "EZResult", "EARTHCOM.info", "EuripBot","ESISmartSpider", "FAST-WebCrawler", "FyberSearch", "Findexa Crawler", 
                                  "Fluffy", "Googlebot", "geckobot", "GenCrawler","GeonaBot", "getRAX", "Gulliver", "Hubater", "ia_archiver",
                                  "Slurp", "Scooter", "Mercator", "RaBot",  "Jack", "Speedy Spider", "moget", "Toutatis", "IlTrovatore-Setaccio", 
                                  "IncyWincy", "UltraSeek", "InfoSeek Sidewinder", "Mole2", "MP3Bot", "Knowledge.com", "kuloko-bot", "LNSpiderguy", 
                                  "Linknzbot", "lookbot", "MantraAgent", "NetResearchServer", "Lycos", "JoocerBot", "HenryTheMiragoRobot", "MojeekBot", 
                                  "mozDex", "MSNBOT", "Navadoo Crawler", "ObjectsSearch", "OnetSzukaj", "PicoSearch", "PJspider", 
                                  "nttdirectory_robot", "maxbot.com", "Openfind", "psbot", "QweeryBot", "StackRambler", "SeznamBot", "Search-10",  "Scrubby",  
                                  "speedfind ramBot xtreme", "Kototoi", "SearchByUsa", "Searchspider", "SightQuestBot", "Spider_Monkey", "Surfnomore", "teoma", 
                                  "UK Searcher Spider", "Nazilla", "MuscatFerret", "ZyBorg", "WIRE WebRefiner", "WSCbot", "Yandex", 
                                  "Yellopet-Spider", "YBSbot", "OceanSpiders", "MozSpider" 
                                };

      if (!String.IsNullOrEmpty(userAgent))
      {
        return spiderContains.Select(s => s.ToLower()).Contains(userAgent.ToLower());
      }

      return false;
    }

    /// <summary>
    /// Validates if the useragent is a known ignored UA string
    /// </summary>
    /// <param name="userAgent">
    /// </param>
    /// <returns>
    /// The true if the UA string patterrn should not be displayed in active users.
    /// </returns>
    public static bool IsIgnoredForDisplay(string userAgent)
    {
        string[] stringContains = {
                                  "Apple-PubSub"
                                  };

        if (!String.IsNullOrEmpty(userAgent))
        {
            return stringContains.Select(s => s.ToLower()).Contains(userAgent.ToLower());
        }

        return false;
    }

    /// <summary>
    /// Is this user agent IE v6?
    /// </summary>
    /// <returns>
    /// The is browser i e 6.
    /// </returns>
    public static bool IsBrowserIE6()
    {
      if (HttpContext.Current.Request.Browser.Browser.Contains("IE") && HttpContext.Current.Request.Browser.Version.StartsWith("6."))
      {
        // IE version 6
        return true;
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
    public static bool IsMobileDevice(string userAgent)
    {
      string[] mobileContains = {
                                  "iphone", "ppc", "windows ce", "blackberry", "opera mini", "mobile", "palm", "portable"
                                };

      if (!String.IsNullOrEmpty(userAgent))
      {
        return mobileContains.Contains(userAgent.ToLower());
      }

      return false;
    }
  }
}