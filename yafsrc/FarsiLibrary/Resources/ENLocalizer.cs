/* Farsi Library - Working with Dates, Calendars, and DatePickers
 * http://www.codeproject.com/KB/selection/FarsiLibrary.aspx
 * 
 * Copyright (C) Hadi Eskandari
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 * and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
 * OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace FarsiLibrary.Resources
{
    public class ENLocalizer : BaseLocalizer
    {
        public override string GetLocalizedString(StringID id)
        {
            switch (id)
            {
                case StringID.Empty: return string.Empty;
                case StringID.Numbers_0: return "0";
                case StringID.Numbers_1: return "1";
                case StringID.Numbers_2: return "2";
                case StringID.Numbers_3: return "3";
                case StringID.Numbers_4: return "4";
                case StringID.Numbers_5: return "5";
                case StringID.Numbers_6: return "6";
                case StringID.Numbers_7: return "7";
                case StringID.Numbers_8: return "8";
                case StringID.Numbers_9: return "9";

                case StringID.FADateTextBox_Required: return "Mandatory field";
                case StringID.FAMonthView_None: return "Empty";
                case StringID.FAMonthView_Today: return "Today";

                case StringID.PersianDate_InvalidDateFormat: return "Invalid date format";
                case StringID.PersianDate_InvalidDateTime: return "Invalid date/time value";
                case StringID.PersianDate_InvalidDateTimeLength: return "Invalid date time format";
                case StringID.PersianDate_InvalidDay: return "Invalid Day value";
                case StringID.PersianDate_InvalidEra: return "Invalid Era value";
                case StringID.PersianDate_InvalidFourDigitYear: return "Invalid four digit Year value";
                case StringID.PersianDate_InvalidHour: return "Invalid Hour value";
                case StringID.PersianDate_InvalidLeapYear: return "Not a leap year. Correct the day value.";
                case StringID.PersianDate_InvalidMinute: return "Invalid Minute value";
                case StringID.PersianDate_InvalidMonth: return "Invalid Month value";
                case StringID.PersianDate_InvalidMonthDay: return "Invalid Month/Day value";
                case StringID.PersianDate_InvalidSecond: return "Invalid Second value";
                case StringID.PersianDate_InvalidTimeFormat: return "Invalid Time format";
                case StringID.PersianDate_InvalidYear: return "Invalid Year value.";

                case StringID.Validation_Cancel: return "Cancel";
                case StringID.Validation_NotValid: return "Entered value is not in valid range.";
                case StringID.Validation_Required: return "This is a mandatory field. Please fill it in.";
                case StringID.Validation_NullText: return "[Empty Value]";

                case StringID.MessageBox_Ok: return "Ok";
                case StringID.MessageBox_Abort: return "Abort";
                case StringID.MessageBox_Cancel: return "Cancel";
                case StringID.MessageBox_Ignore: return "Ignore";
                case StringID.MessageBox_No: return "No";
                case StringID.MessageBox_Retry: return "Retry";
                case StringID.MessageBox_Yes: return "Yes";
            }

            return string.Empty;
        }

        public override string GetFormatterString(FormatterStringID stringID)
        {
            switch (stringID)
            {
                case FormatterStringID.CenturyPattern: return "%n %u";
                case FormatterStringID.CenturyFuturePrefix: return "";
                case FormatterStringID.CenturyFutureSuffix: return " from now";
                case FormatterStringID.CenturyPastPrefix: return "";
                case FormatterStringID.CenturyPastSuffix: return " ago";
                case FormatterStringID.CenturyName: return "century";
                case FormatterStringID.CenturyPluralName: return "centuries";
                case FormatterStringID.DayPattern: return "%n %u";
                case FormatterStringID.DayFuturePrefix: return "";
                case FormatterStringID.DayFutureSuffix: return " from now";
                case FormatterStringID.DayPastPrefix: return "";
                case FormatterStringID.DayPastSuffix: return " ago";
                case FormatterStringID.DayName: return "day";
                case FormatterStringID.DayPluralName: return "days";
                case FormatterStringID.DecadePattern: return "%n %u";
                case FormatterStringID.DecadeFuturePrefix: return "";
                case FormatterStringID.DecadeFutureSuffix: return " from now";
                case FormatterStringID.DecadePastPrefix: return "";
                case FormatterStringID.DecadePastSuffix: return " ago";
                case FormatterStringID.DecadeName: return "decade";
                case FormatterStringID.DecadePluralName: return "decades";
                case FormatterStringID.HourPattern: return "%n %u";
                case FormatterStringID.HourFuturePrefix: return "";
                case FormatterStringID.HourFutureSuffix: return " from now";
                case FormatterStringID.HourPastPrefix: return "";
                case FormatterStringID.HourPastSuffix: return " ago";
                case FormatterStringID.HourName: return "hour";
                case FormatterStringID.HourPluralName: return "hours";
                case FormatterStringID.JustNowPattern: return "%u";
                case FormatterStringID.JustNowFuturePrefix: return "";
                case FormatterStringID.JustNowFutureSuffix: return "moments from now";
                case FormatterStringID.JustNowPastPrefix: return "moments ago";
                case FormatterStringID.JustNowPastSuffix: return "";
                case FormatterStringID.JustNowName: return "";
                case FormatterStringID.JustNowPluralName: return "";
                case FormatterStringID.MillenniumPattern: return "%n %u";
                case FormatterStringID.MillenniumFuturePrefix: return "";
                case FormatterStringID.MillenniumFutureSuffix: return " from now";
                case FormatterStringID.MillenniumPastPrefix: return "";
                case FormatterStringID.MillenniumPastSuffix: return " ago";
                case FormatterStringID.MillenniumName: return "millennium";
                case FormatterStringID.MillenniumPluralName: return "millennia";
                case FormatterStringID.MillisecondPattern: return "%n %u";
                case FormatterStringID.MillisecondFuturePrefix: return "";
                case FormatterStringID.MillisecondFutureSuffix: return " from now";
                case FormatterStringID.MillisecondPastPrefix: return "";
                case FormatterStringID.MillisecondPastSuffix: return " ago";
                case FormatterStringID.MillisecondName: return "millisecond";
                case FormatterStringID.MillisecondPluralName: return "milliseconds";
                case FormatterStringID.MinutePattern: return "%n %u";
                case FormatterStringID.MinuteFuturePrefix: return "";
                case FormatterStringID.MinuteFutureSuffix: return " from now";
                case FormatterStringID.MinutePastPrefix: return "";
                case FormatterStringID.MinutePastSuffix: return " ago";
                case FormatterStringID.MinuteName: return "minute";
                case FormatterStringID.MinutePluralName: return "minutes";
                case FormatterStringID.MonthPattern: return "%n %u";
                case FormatterStringID.MonthFuturePrefix: return "";
                case FormatterStringID.MonthFutureSuffix: return " from now";
                case FormatterStringID.MonthPastPrefix: return "";
                case FormatterStringID.MonthPastSuffix: return " ago";
                case FormatterStringID.MonthName: return "month";
                case FormatterStringID.MonthPluralName: return "months";
                case FormatterStringID.SecondPattern: return "%n %u";
                case FormatterStringID.SecondFuturePrefix: return "";
                case FormatterStringID.SecondFutureSuffix: return " from now";
                case FormatterStringID.SecondPastPrefix: return "";
                case FormatterStringID.SecondPastSuffix: return " ago";
                case FormatterStringID.SecondName: return "second";
                case FormatterStringID.SecondPluralName: return "seconds";
                case FormatterStringID.WeekPattern: return "%n %u";
                case FormatterStringID.WeekFuturePrefix: return "";
                case FormatterStringID.WeekFutureSuffix: return " from now";
                case FormatterStringID.WeekPastPrefix: return "";
                case FormatterStringID.WeekPastSuffix: return " ago";
                case FormatterStringID.WeekName: return "week";
                case FormatterStringID.WeekPluralName: return "weeks";
                case FormatterStringID.YearPattern: return "%n %u";
                case FormatterStringID.YearFuturePrefix: return "";
                case FormatterStringID.YearFutureSuffix: return " from now";
                case FormatterStringID.YearPastPrefix: return "";
                case FormatterStringID.YearPastSuffix: return " ago";
                case FormatterStringID.YearName: return "year";
                case FormatterStringID.YearPluralName: return "years";
            }

            return string.Empty;
        }
    }
}
