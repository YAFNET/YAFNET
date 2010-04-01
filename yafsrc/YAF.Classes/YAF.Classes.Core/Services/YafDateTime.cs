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

namespace YAF.Classes.Core
{
  using System;

  /// <summary>
  /// The yaf date time.
  /// </summary>
  public class YafDateTime
  {
    /// <summary>
    /// Time zone suffix for Guests
    /// </summary>
    private string timeZoneName = "(UTC)"; 

    /// <summary>
    /// Gets the time zone offset 
    /// for the current user.
    /// </summary>
    public TimeSpan TimeOffset
    {
        get
        {
            if (YafContext.Current.Page != null)
            {
                int min = YafContext.Current.TimeZoneUser;
                return new TimeSpan((min / 60) + Convert.ToInt32(YafContext.Current.DSTUser), (min % 60) + YafContext.Current.BoardSettings.ServerTimeCorrection, 0);
            }
            else
            {
                return new TimeSpan(0);
            }
        }
    }

    /// <summary>
    /// Formats a datetime value into 07.03.2003 22:32:34
    /// </summary>
    /// <param name="objectDateTIme"> 
    /// The date to be formatted
    /// </param>    
    /// <returns>
    /// Formatted  <see cref="string"/> of the formatted <see cref="DateTime"/> Object.
    /// </returns>
    public string FormatDateTime(object objectDateTime)
    {
      if (objectDateTime == null)
      { 
        throw new ArgumentNullException("objectDateTime", "objectDateTime is null.");
      }
        
      DateTime dt = (DateTime)objectDateTime + this.TimeOffset;

      string strDateFormat = String.Format("{0:F}", dt);

      try
      {
        if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
        {
          strDateFormat = YafContext.Current.Localization.FormatDateTime(YafContext.Current.Localization.GetText("FORMAT_DATE_TIME_LONG"), dt);
        }
      }
      catch (Exception)
      {
      }

      return YafContext.Current.IsGuest ? String.Format("{0}{1}", strDateFormat, this.timeZoneName) : strDateFormat;
    }

    /// <summary>
    /// Formats a datatime value into 07.03.2003 00:00:00 except if 
    /// the date is yesterday or today -- in which case it says that.
    /// </summary>
    /// <param name="objectDateTime">
    /// The datetime to be formatted
    /// </param>
    /// <returns>
    /// Formatted string of DateTime object
    /// </returns>
    public string FormatDateTimeTopic(object objectDateTime)
    {
      if (objectDateTime == null)
      {
        throw new ArgumentNullException("objectDateTime", "objectDateTime is null.");
      }

      string strDateFormat;
      DateTime dt;
      DateTime nt;

      try
      {
        dt = Convert.ToDateTime(objectDateTime) + this.TimeOffset;
      }
      catch
      {
        // failed convert...
        return "[error]";
      }

      nt = DateTime.UtcNow + this.TimeOffset;

      try
      {
        if (dt.Date == nt.Date)
        {
          // today
          strDateFormat = String.Format(YafContext.Current.Localization.GetText("TodayAt"), dt);

          if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
          {
            strDateFormat = YafContext.Current.Localization.FormatString(YafContext.Current.Localization.GetText("TodayAt"), dt);
          }
        }
        else if (dt.Date == nt.AddDays(-1).Date)
        {
          // yesterday
          strDateFormat = String.Format(YafContext.Current.Localization.GetText("YesterdayAt"), dt);

          if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
          {
            strDateFormat = YafContext.Current.Localization.FormatString(YafContext.Current.Localization.GetText("YesterdayAt"), dt);
          }
        }
        else if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
        {
          strDateFormat = YafContext.Current.Localization.FormatDateTime(YafContext.Current.Localization.GetText("FORMAT_DATE_TIME_SHORT"), dt);
        }
        else
        {
          strDateFormat = String.Format("{0:f}", dt);
        }

        return YafContext.Current.IsGuest ? String.Format("{0}{1}", strDateFormat, this.timeZoneName) : strDateFormat;
      }
      catch (Exception)
      {
        return dt.ToString("f");
      }
    }

    /// <summary>
    /// This formats a DateTime into a short string
    /// </summary>
    /// <param name="objectDateTime">
    /// The DateTime like object you wish to make a formatted string.
    /// </param>
    /// <returns>
    /// The formatted string created from the DateTime object.
    /// </returns>
    public string FormatDateTimeShort(object objectDateTime)
    {
        string strDateFormat;
 
      if (objectDateTime == null)
      {
        throw new ArgumentNullException("objectDateTime", "objectDateTime is null.");
      }

      DateTime dt = (DateTime) objectDateTime + this.TimeOffset;

      try
      {
        if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
        {
         strDateFormat = YafContext.Current.Localization.FormatDateTime(YafContext.Current.Localization.GetText("FORMAT_DATE_TIME_SHORT"), dt);
        }
        else
        {
         strDateFormat = String.Format("{0:f}", dt);
        }
      }
      catch (Exception)
      {
          strDateFormat = dt.ToString("f");
      }

      return YafContext.Current.IsGuest ? String.Format("{0}{1}", strDateFormat, this.timeZoneName) : strDateFormat;

    }

    /// <summary>
    /// Formats a datetime value into 7. february 2003
    /// </summary>
    /// <param name="dt">
    /// The date to be formatted
    /// </param>
    /// <returns>
    /// The format date long.
    /// </returns>
    public string FormatDateLong(DateTime dt)
    {
        string strDateFormat;

        dt += this.TimeOffset;     

      try
      {
        if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
        {
          strDateFormat = YafContext.Current.Localization.FormatDateTime(YafContext.Current.Localization.GetText("FORMAT_DATE_LONG"), dt);
        }
        else
        {
          strDateFormat = String.Format("{0:D}", dt);
        }
      }
      catch (Exception)
      {
          strDateFormat = dt.ToString("D");
      }

      return YafContext.Current.IsGuest ? String.Format("{0}{1}", strDateFormat, this.timeZoneName) : strDateFormat;
    }

    /// <summary>
    /// Formats a datetime value into 07.03.2003
    /// </summary>
    /// <param name="objectDateTime">
    /// This formats the date.
    /// </param>
    /// <returns>
    /// Short formatted date.
    /// </returns>
    public string FormatDateShort(object objectDateTime)
    {
      string strDateFormat;

      DateTime dt = (DateTime)objectDateTime + this.TimeOffset;

      try
      {
        if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
        {
          strDateFormat = YafContext.Current.Localization.FormatDateTime(YafContext.Current.Localization.GetText("FORMAT_DATE_SHORT"), dt);
        }
        else
        {
          strDateFormat = String.Format("{0:d}", dt);
        }
      }
      catch (Exception)
      {
          strDateFormat = dt.ToString("d");
      }

      return YafContext.Current.IsGuest ? String.Format("{0}{1}", strDateFormat, this.timeZoneName) : strDateFormat;
    }

    /// <summary>
    /// Formats a datetime value into 22:32:34
    /// </summary>
    /// <param name="dt">
    /// The date to be formatted
    /// </param>
    /// <returns>
    /// The format time.
    /// </returns>
    public string FormatTime(DateTime dt)
    {
      string strDateFormat;

      dt += this.TimeOffset;

      try
      {
        if (YafContext.Current.BoardSettings.DateFormatFromLanguage)
        {
          strDateFormat = YafContext.Current.Localization.FormatDateTime(YafContext.Current.Localization.GetText("FORMAT_TIME"), dt);
        }
        else
        {
          strDateFormat = String.Format("{0:T}", dt);
        }
      }
      catch (Exception)
      {
        strDateFormat = dt.ToString("T");
      }

      return YafContext.Current.IsGuest ? String.Format("{0}{1}", strDateFormat, this.timeZoneName) : strDateFormat;
    }
  }
}