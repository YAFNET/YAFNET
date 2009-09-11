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
		public static decimal GetHourOffsetFromNode( XmlNode node )
		{
			// calculate hours -- can use prefix of either UTC or GMT...
			decimal hours = 0;

			try
			{
				hours = Convert.ToDecimal( node.Attributes["tag"].Value.Replace( "UTC", "" ).Replace( "GMT", "" ) );
			}
			catch ( FormatException ex )
			{
				hours = Convert.ToDecimal( node.Attributes["tag"].Value.Replace( ".", "," ).Replace( "UTC", "" ).Replace( "GMT", "" ) );
			}

			return hours;
		}

		static public string TraceResources()
		{
			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();

			// get a list of resource names from the manifest
			string[] resNames = a.GetManifestResourceNames();

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
			return BinaryAnd( SqlDataLayerConverter.VerifyInt32( value ), SqlDataLayerConverter.VerifyInt32( checkAgainst ) );
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
			return ( (String)originalMessage != (String)newMessage );
		}
	}
}
