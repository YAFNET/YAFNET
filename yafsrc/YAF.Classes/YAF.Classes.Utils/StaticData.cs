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
using System.Data;
using System.Web;
using System.Xml;

namespace YAF.Classes.Utils
{

	public class YafLanguageNodeComparer : IComparer<XmlNode>
	{
		#region IComparer<XmlNode> Members

		public int Compare(XmlNode x, XmlNode y)
		{
			return General.GetHourOffsetFromNode( x ).CompareTo( General.GetHourOffsetFromNode( y ) );
		}
		#endregion
	}
	/// <summary>
	/// Summary description for StaticData.
	/// </summary>
	public class YafStaticData
	{
		public static DataTable TimeZones( YafLocalization localization )
		{
			using ( DataTable dt = new DataTable( "TimeZone" ) )
			{
				dt.Columns.Add( "Value", Type.GetType( "System.Int32" ) );
				dt.Columns.Add( "Name", Type.GetType( "System.String" ) );

				List<XmlNode> timezones = localization.GetNodesUsingQuery( "TIMEZONES", "@tag[starts-with(.,'UTC')]" );

				timezones.Sort( new YafLanguageNodeComparer() );

				foreach ( XmlNode node in timezones )
				{
					dt.Rows.Add(new object[] { General.GetHourOffsetFromNode(node) * 60, node.InnerText });
				}

				return dt;
			}
		}

		public static DataTable TimeZones()
		{
			return TimeZones( YafContext.Current.Localization );
		}

		public static DataTable TimeZones( string forceLanguage )
		{
			YafLocalization localization = new YafLocalization();
			localization.LoadTranslation( forceLanguage );

			return TimeZones( localization );
		}

		public static DataTable Themes()
		{
			using ( DataTable dt = new DataTable( "Themes" ) )
			{
				dt.Columns.Add( "Theme", typeof( string ) );
				dt.Columns.Add( "FileName", typeof( string ) );

				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo( System.Web.HttpContext.Current.Request.MapPath( String.Format( "{0}themes", YafForumInfo.ForumFileRoot ) ) );
				System.IO.FileInfo [] files = dir.GetFiles( "*.xml" );
				foreach ( System.IO.FileInfo file in files )
				{
					try
					{
						System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
						doc.Load( file.FullName );

						DataRow dr = dt.NewRow();
						dr ["Theme"] = doc.DocumentElement.Attributes ["theme"].Value;
						dr ["FileName"] = file.Name;
						dt.Rows.Add( dr );
					}
					catch ( Exception )
					{
					}
				}
				return dt;
			}
		}

		public static DataTable Languages()
		{
			using ( DataTable dt = new DataTable( "Languages" ) )
			{
				dt.Columns.Add( "Language", typeof( string ) );
				dt.Columns.Add( "FileName", typeof( string ) );

				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo( System.Web.HttpContext.Current.Request.MapPath( String.Format( "{0}languages", YafForumInfo.ForumFileRoot ) ) );
				System.IO.FileInfo [] files = dir.GetFiles( "*.xml" );
				foreach ( System.IO.FileInfo file in files )
				{
					try
					{
						System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
						doc.Load( file.FullName );
						DataRow dr = dt.NewRow();
						dr ["Language"] = doc.DocumentElement.Attributes ["language"].Value;
						dr ["FileName"] = file.Name;
						dt.Rows.Add( dr );
					}
					catch ( Exception )
					{
					}
				}
				return dt;
			}
		}

		static public DataTable AllowDisallow()
		{
			using ( DataTable dt = new DataTable( "AllowDisallow" ) )
			{
				dt.Columns.Add( "Text", typeof( string ) );
				dt.Columns.Add( "Value", typeof( int ) );

				string [] tTextArray = { "Allowed", "Disallowed" };

				for ( int i = 0; i < tTextArray.Length; i++ )
				{
					DataRow dr = dt.NewRow();
					dr ["Text"] = tTextArray [i];
					dr ["Value"] = i;
					dt.Rows.Add( dr );
				}
				return dt;
			}
		}

		static public DataTable TopicTimes()
		{
			using ( DataTable dt = new DataTable( "TopicTimes" ) )
			{
				dt.Columns.Add( "TopicText", typeof( string ) );
				dt.Columns.Add( "TopicValue", typeof( int ) );

				string [] tTextArray = { "all", "last_day", "last_two_days", "last_week", "last_two_weeks", "last_month", "last_two_months", "last_six_months", "last_year" };
				string [] tTextArrayProp = { "All", "Last Day", "Last Two Days", "Last Week", "Last Two Weeks", "Last Month", "Last Two Months", "Last Six Months", "Last Year" };

				for ( int i = 0; i < 8; i++ )
				{
					DataRow dr = dt.NewRow();
					dr ["TopicText"] = ( YafContext.Current.Localization.TransPage == null ) ? tTextArrayProp [i] : YafContext.Current.Localization.GetText( tTextArray [i] );
					dr ["TopicValue"] = i;
					dt.Rows.Add( dr );
				}
				return dt;
			}
		}
	}
}
