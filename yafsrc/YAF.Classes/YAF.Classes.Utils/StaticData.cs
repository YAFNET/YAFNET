/* Yet Another Forum.net
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Web;

namespace YAF.Classes.Utils
{
	/// <summary>
	/// Summary description for StaticData.
	/// </summary>
	public class YafStaticData
	{
		public static DataTable TimeZones()
		{
			using ( DataTable dt = new DataTable( "TimeZone" ) )
			{
				dt.Columns.Add( "Value", Type.GetType( "System.Int32" ) );
				dt.Columns.Add( "Name", Type.GetType( "System.String" ) );

				dt.Rows.Add( new object [] { -720, "(GMT - 12:00) Enitwetok, Kwajalien" } );
				dt.Rows.Add( new object [] { -660, "(GMT - 11:00) Midway Island, Samoa" } );
				dt.Rows.Add( new object [] { -600, "(GMT - 10:00) Hawaii" } );
				dt.Rows.Add( new object [] { -540, "(GMT - 9:00) Alaska" } );
				dt.Rows.Add( new object [] { -480, "(GMT - 8:00) Pacific Time (US & Canada)" } );
				dt.Rows.Add( new object [] { -420, "(GMT - 7:00) Mountain Time (US & Canada)" } );
				dt.Rows.Add( new object [] { -360, "(GMT - 6:00) Central Time (US & Canada), Mexico City" } );
				dt.Rows.Add( new object [] { -300, "(GMT - 5:00) Eastern Time (US & Canada), Bogota, Lima, Quito" } );
				dt.Rows.Add( new object [] { -240, "(GMT - 4:00) Atlantic Time (Canada), Caracas, La Paz" } );
				dt.Rows.Add( new object [] { -210, "(GMT - 3:30) Newfoundland" } );
				dt.Rows.Add( new object [] { -180, "(GMT - 3:00) Brazil, Buenos Aires, Georgetown, Falkland Is." } );
				dt.Rows.Add( new object [] { -120, "(GMT - 2:00) Mid-Atlantic, Ascention Is., St Helena" } );
				dt.Rows.Add( new object [] { -60, "(GMT - 1:00) Azores, Cape Verde Islands" } );
				dt.Rows.Add( new object [] { 0, "(GMT) Casablanca, Dublin, Edinburgh, London, Lisbon, Monrovia" } );
				dt.Rows.Add( new object [] { 60, "(GMT + 1:00) Berlin, Brussels, Kristiansund, Madrid, Paris, Rome, Warsaw" } );
				dt.Rows.Add( new object [] { 120, "(GMT + 2:00) Kaliningrad, South Africa" } );
				dt.Rows.Add( new object [] { 180, "(GMT + 3:00) Baghdad, Riyadh, Moscow, Nairobi" } );
				dt.Rows.Add( new object [] { 210, "(GMT + 3:30) Tehran" } );
				dt.Rows.Add( new object [] { 240, "(GMT + 4:00) Adu Dhabi, Baku, Muscat, Tbilisi" } );
				dt.Rows.Add( new object [] { 270, "(GMT + 4:30) Kabul" } );
				dt.Rows.Add( new object [] { 300, "(GMT + 5:00) Ekaterinburg, Islamabad, Karachi, Tashkent" } );
				dt.Rows.Add( new object [] { 330, "(GMT + 5:30) Bombay, Calcutta, Madras, New Delhi" } );
				dt.Rows.Add( new object [] { 360, "(GMT + 6:00) Almaty, Colomba, Dhakra" } );
				dt.Rows.Add( new object [] { 420, "(GMT + 7:00) Bangkok, Hanoi, Jakarta" } );
				dt.Rows.Add( new object [] { 480, "(GMT + 8:00) Beijing, Hong Kong, Perth, Singapore, Taipei" } );
				dt.Rows.Add( new object [] { 540, "(GMT + 9:00) Osaka, Sapporo, Seoul, Tokyo, Yakutsk" } );
				dt.Rows.Add( new object [] { 570, "(GMT + 9:30) Adelaide, Darwin" } );
				dt.Rows.Add( new object [] { 600, "(GMT + 10:00) Melbourne, Papua New Guinea, Sydney, Vladivostok" } );
				dt.Rows.Add( new object [] { 660, "(GMT + 11:00) Magadan, New Caledonia, Solomon Islands" } );
				dt.Rows.Add( new object [] { 720, "(GMT + 12:00) Auckland, Wellington, Fiji, Marshall Island" } );
				return dt;
			}
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
