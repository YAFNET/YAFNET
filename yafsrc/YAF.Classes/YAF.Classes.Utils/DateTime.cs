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
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace YAF.Classes.Utils
{
	public static class YafDateTime
	{
		/// <summary>
		/// Returns the user timezone offset from GMT
		/// </summary>
		public static TimeSpan TimeZoneOffsetUser
		{
			get
			{
				if ( YafContext.Current.Page != null )
				{
					int min = YafContext.Current.TimeZoneUser;
					return new TimeSpan( min / 60, min % 60, 0 );
				}
				else
					return new TimeSpan( 0 );
			}
		}
		/// <summary>
		/// Returns the time zone offset for the current user compared to the forum time zone.
		/// </summary>
		public static TimeSpan TimeOffset
		{
			get
			{
				return TimeZoneOffsetUser - YafContext.Current.BoardSettings.TimeZone;
			}
		}
		/// <summary>
		/// Formats a datetime value into 07.03.2003 22:32:34
		/// </summary>
		/// <param name="o">The date to be formatted</param>
		/// <returns>Formatted string of the formatted DateTime Object.</returns>
		public static string FormatDateTime( object o )
		{
			DateTime dt = ( DateTime ) o + TimeOffset;
			string strDateFormat;

			strDateFormat = String.Format( "{0:F}", dt );

			try
			{
				if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
					strDateFormat = YafContext.Current.Localization.FormatDateTime(YafContext.Current.Localization.GetText("FORMAT_DATE_TIME_LONG"), dt);
			}
			catch ( Exception )
			{

			}

			return strDateFormat;
		}

		/// <summary>
		/// Formats a datatime value into 07.03.2003 00:00:00 except if 
		/// the date is yesterday or today -- in which case it says that.
		/// </summary>
		/// <param name="o">The datetime to be formatted</param>
		/// <returns>Formatted string of DateTime object</returns>
		public static string FormatDateTimeTopic( object o )
		{
			string strDateFormat;
			DateTime dt;
			DateTime nt;


			try
			{
				dt = Convert.ToDateTime( o ) + TimeOffset;
			}
			catch
			{
				// failed convert...
				return "[error]";
			}
			
			nt = DateTime.Now + TimeOffset;

			try
			{
				if (dt.Date == nt.Date)
				{
					// today
					strDateFormat = String.Format(YafContext.Current.Localization.GetText("TodayAt"), dt);

					if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
						strDateFormat = YafContext.Current.Localization.FormatString(YafContext.Current.Localization.GetText("TodayAt"), dt);
				}
				else if (dt.Date == nt.AddDays(-1).Date)
				{
					// yesterday
					strDateFormat = String.Format(YafContext.Current.Localization.GetText("YesterdayAt"), dt);

					if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
						strDateFormat = YafContext.Current.Localization.FormatString(YafContext.Current.Localization.GetText("YesterdayAt"), dt);
				}
				else if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
				{
					strDateFormat = YafContext.Current.Localization.FormatDateTime(YafContext.Current.Localization.GetText("FORMAT_DATE_TIME_SHORT"), dt);
				}
				else
				{
					strDateFormat = String.Format("{0:f}", dt);
				}
				return strDateFormat;
			}
			catch ( Exception )
			{
				return dt.ToString( "f" );
			}
		}
		/// <summary>
		/// This formats a DateTime into a short string
		/// </summary>
		/// <param name="o">The DateTime like object you wish to make a formatted string.</param>
		/// <returns>The formatted string created from the DateTime object.</returns>
		public static string FormatDateTimeShort( object o )
		{
			DateTime dt = ( DateTime ) o + TimeOffset;
			try
			{
				if ( YafContext.Current.BoardSettings.DateFormatFromLanguage )
					return YafContext.Current.Localization.FormatDateTime(YafContext.Current.Localization.GetText("FORMAT_DATE_TIME_SHORT"), dt);
				else
					return String.Format( "{0:f}", dt );
			}
			catch ( Exception )
			{
				return dt.ToString( "f" );
			}
		}
		/// <summary>
		/// Formats a datetime value into 7. february 2003
		/// </summary>
		/// <param name="dt">The date to be formatted</param>
		/// <returns></returns>
		public static string FormatDateLong( DateTime dt )
		{
			dt += TimeOffset;
			try
			{
				if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
					return YafContext.Current.Localization.FormatDateTime(YafContext.Current.Localization.GetText("FORMAT_DATE_LONG"), dt);
				else
					return String.Format("{0:D}", dt);
			}
			catch ( Exception )
			{
				return dt.ToString( "D" );
			}
		}
		/// <summary>
		/// Formats a datetime value into 07.03.2003
		/// </summary>
		/// <param name="o">This formats the date.</param>
		/// <returns>Short formatted date.</returns>
		public static string FormatDateShort( object o )
		{
			DateTime dt = ( DateTime ) o + TimeOffset;
			try
			{
				if ( YafContext.Current.BoardSettings.DateFormatFromLanguage )
					return YafContext.Current.Localization.FormatDateTime(YafContext.Current.Localization.GetText("FORMAT_DATE_SHORT"), dt);
				else
					return String.Format( "{0:d}", dt );
			}
			catch ( Exception )
			{
				return dt.ToString( "d" );
			}
		}
		/// <summary>
		/// Formats a datetime value into 22:32:34
		/// </summary>
		/// <param name="dt">The date to be formatted</param>
		/// <returns></returns>
		public static string FormatTime( DateTime dt )
		{
			dt += TimeOffset;
			try
			{
				if ( YafContext.Current.BoardSettings.DateFormatFromLanguage )
					return YafContext.Current.Localization.FormatDateTime(YafContext.Current.Localization.GetText("FORMAT_TIME"), dt);
				else
					return String.Format( "{0:T}", dt );
			}
			catch ( Exception )
			{
				return dt.ToString( "T" );
			}
		}
	}
}
