using System;
using System.Collections.Generic;
using System.Text;

namespace YAF.Classes.Utils
{
	public static class yaf_DateTime
	{
		/// <summary>
		/// Returns the user timezone offset from GMT
		/// </summary>
		public static TimeSpan TimeZoneOffsetUser
		{
			get
			{
				if ( yaf_Context.Current.Page != null )
				{
					int min = yaf_Context.Current.Page.TimeZoneUser;
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
				return TimeZoneOffsetUser - yaf_Context.Current.BoardSettings.TimeZone;
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
				if ( yaf_Context.Current.BoardSettings.DateFormatFromLanguage )
					strDateFormat = dt.ToString( yaf_Context.Current.Localization.GetText( "FORMAT_DATE_TIME_LONG" ) );
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
			DateTime dt = Convert.ToDateTime( o ) + TimeOffset;
			DateTime nt = DateTime.Now + TimeOffset;

			try
			{
				if ( dt.Date == nt.Date )
				{
					// today
					strDateFormat = String.Format( yaf_Context.Current.Localization.GetText( "TodayAt" ), dt );
				}
				else if ( dt.Date == nt.AddDays( -1 ).Date )
				{
					// yesterday
					strDateFormat = String.Format( yaf_Context.Current.Localization.GetText( "YesterdayAt" ), dt );
				}
				else if ( yaf_Context.Current.BoardSettings.DateFormatFromLanguage )
				{
					strDateFormat = dt.ToString( yaf_Context.Current.Localization.GetText( "FORMAT_DATE_TIME_SHORT" ) );
				}
				else
				{
					strDateFormat = String.Format( "{0:f}", dt );
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
				if ( yaf_Context.Current.BoardSettings.DateFormatFromLanguage )
					return dt.ToString( yaf_Context.Current.Localization.GetText( "FORMAT_DATE_TIME_SHORT" ) );
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
				if ( yaf_Context.Current.BoardSettings.DateFormatFromLanguage )
					return dt.ToString( yaf_Context.Current.Localization.GetText( "FORMAT_DATE_LONG" ) );
				else
					return String.Format( "{0:D}", dt );
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
				if ( yaf_Context.Current.BoardSettings.DateFormatFromLanguage )
					return dt.ToString( yaf_Context.Current.Localization.GetText( "FORMAT_DATE_SHORT" ) );
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
				if ( yaf_Context.Current.BoardSettings.DateFormatFromLanguage )
					return dt.ToString( yaf_Context.Current.Localization.GetText( "FORMAT_TIME" ) );
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
