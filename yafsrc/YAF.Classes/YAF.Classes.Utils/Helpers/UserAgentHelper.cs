/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace YAF.Classes.Utils
{
	///<summary>
	/// Helper for Figuring out the User Agent.
	///</summary>
	public static class UserAgentHelper
	{
		/// <summary>
		/// Validates if the useragent is a search engine spider
		/// </summary>
		/// <param name="userAgent"></param>
		/// <returns></returns>
		static public bool IsSearchEngineSpider( string userAgent )
		{
			string[] spiderContains = 
				{
					"Googlebot", "Slurp", "abachoBOT", "abcdatos_botlink", "AESOP_com_SpiderMan", "ah-ha.com crawler", "ia_archiver",
					"Scooter", "Mercator", "AltaVista-Intranet", "FAST-WebCrawler", "Acoon Robot", "antibot", "Atomz", "AxmoRobot",
					"Buscaplus Robi", "CanSeek", "ChristCRAWLER", "Clushbot", "Crawler", "RaBot", "DeepIndex", "DittoSpyder", "Jack",
					"EARTHCOM.info", "Speedy Spider", "ArchitextSpider", "EuripBot", "Arachnoidea", "EZResult", "FyberSearch", "geckobot",
					"GenCrawler", "GeonaBot", "getRAX", "moget", "Aranha", "Toutatis", "Hubater", "IlTrovatore-Setaccio", "IncyWincy",
					"UltraSeek", "InfoSeek Sidewinder", "Mole2", "MP3Bot", "Knowledge.com", "kuloko-bot", "LNSpiderguy", "Linknzbot",
					"lookbot", "MantraAgent", "NetResearchServer", "Lycos", "JoocerBot", "HenryTheMiragoRobot", "MojeekBot", "mozDex",
					"MSNBOT", "Navadoo Crawler", "Gulliver", "ObjectsSearch", "OnetSzukaj", "PicoSearch", "PJspider", "DIIbot",
					"nttdirectory_robot", "maxbot.com", "Openfind", "psbot", "CrawlerBoy", "QweeryBot", "AlkalineBOT", "StackRambler",
					"SeznamBot", "Search-10", "Fluffy", "Scrubby", "asterias", "speedfind ramBot xtreme", "Kototoi", "SearchByUsa",
					"Searchspider", "SightQuestBot", "Spider_Monkey", "Surfnomore", "teoma", "ESISmartSpider", "UK Searcher Spider",
					"appie", "Nazilla", "MuscatFerret", "ZyBorg", "WIRE WebRefiner", "WSCbot", "Yandex", "Yellopet-Spider", "Findexa Crawler",
					"YBSbot"
				};

			if ( !String.IsNullOrEmpty(userAgent))
			{
				return spiderContains.Select( s => s.ToLower() ).Contains( userAgent.ToLower() );
			}

			return false;
		}

		/// <summary>
		/// Tests if the user agent is a mobile device.
		/// </summary>
		/// <param name="userAgent"></param>
		/// <returns></returns>
		public static bool IsMobileDevice( string userAgent )
		{
			string[] mobileContains = 
				{
					"iphone", "ppc", "windows ce", "blackberry", "opera mini", "mobile", "palm", "portable"
				};

			if ( !String.IsNullOrEmpty( userAgent ) )
			{
				return mobileContains.Contains( userAgent.ToLower() );
			}

			return false;
		}
	}
}
