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
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Specialized;
using System.Xml;
using YAF.Classes.Data;

namespace YAF.Classes.Utils
{
	/// <summary>
	/// Summary description for General Utils.
	/// </summary>
	public static class General
	{
		static public string GetSafeRawUrl()
		{
			return GetSafeRawUrl( System.Web.HttpContext.Current.Request.RawUrl );
		}

		/// <summary>
		/// Cleans up a URL so that it doesn't contain any problem characters.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		static public string GetSafeRawUrl( string url )
		{
			string tProcessedRaw = url;
			tProcessedRaw = tProcessedRaw.Replace( "\"", string.Empty );
			tProcessedRaw = tProcessedRaw.Replace( "<", "%3C" );
			tProcessedRaw = tProcessedRaw.Replace( ">", "%3E" );
			tProcessedRaw = tProcessedRaw.Replace( "&", "%26" );
			return tProcessedRaw.Replace( "'", string.Empty );
		}

		/// <summary>
		/// Helper function for the Language TimeZone XML.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static decimal GetHourOffsetFromNode(XmlNode node)
		{
			// calculate hours -- can use prefix of either UTC or GMT...
			decimal hours = 0;

			try
			{
				hours = Convert.ToDecimal(node.Attributes["tag"].Value.Replace("UTC", "").Replace("GMT", ""));
			}
			catch (FormatException ex)
			{
				hours = Convert.ToDecimal(node.Attributes["tag"].Value.Replace(".", ",").Replace("UTC", "").Replace("GMT", ""));
			}

			return hours;
		}

		/// <summary>
		/// Validates if the useragent is a search engine spider or not
		/// </summary>
		/// <param name="UserAgent"></param>
		/// <returns></returns>
		static public bool IsSearchEngineSpider( string userAgent )
		{
			string [] spiderstrings = 
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

			// see if the current useragent is one of these spiders...
			string userAgentLow = userAgent.ToLower();

			foreach ( string spider in spiderstrings )
			{
				if ( userAgentLow.Contains( spider.Trim().ToLower() ) )
				{
					// it's a spider...
					return true;
				}
			}

			return false;
		}

		static public bool IsValidEmail( string email )
		{
			return Regex.IsMatch( email, @"^([0-9a-z]+[-._+&])*[0-9a-z]+@([-0-9a-z]+[.])+[a-z]{2,6}$", RegexOptions.IgnoreCase );
		}

		static public bool IsValidURL( string url )
		{
			return Regex.IsMatch( url, @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&%\$#\=~])*[^\.\,\)\(\s]$" );
		}

		static public bool IsValidInt( string intstr )
		{
			int value;
			return int.TryParse( intstr, out value );
		}

		static public string TraceResources()
		{
			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();

			// get a list of resource names from the manifest
			string [] resNames = a.GetManifestResourceNames();

			// populate the textbox with information about our resources
			// also look for images and put them in our arraylist
			string txtInfo = "";

			txtInfo += String.Format( "Found {0} resources\r\n", resNames.Length );
			txtInfo += "----------\r\n";
			foreach ( string s in resNames )
			{
				txtInfo += s + "\r\n";
			}
			txtInfo += "----------\r\n";

			return txtInfo;
		}

		/* Ederon : 9/12/2007 */
		static public bool BinaryAnd( object value, object checkAgainst )
		{
            return BinaryAnd(SqlDataLayerConverter.VerifyInt32(value), SqlDataLayerConverter.VerifyInt32(checkAgainst));
		}
		static public bool BinaryAnd( int value, int checkAgainst )
		{
			return ( value & checkAgainst ) == checkAgainst;
		}

		static public string EncodeMessage( string message )
		{
			if ( message.IndexOf( '<' ) >= 0 )
				return HttpUtility.HtmlEncode( message );

			return message;
		}

		/// <summary>
		/// Compares two messages.
		/// </summary>
		/// <param name="originalMessage">Original message text.</param>
		/// <param name="newMessage">New message text.</param>
		/// <returns>True if messages differ, false if they are identical.</returns>
		static public bool CompareMessage( Object originalMessage, Object newMessage )
		{
			return ( ( String )originalMessage != ( String )newMessage );
		}
	}
}
