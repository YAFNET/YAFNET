namespace FarsiLibrary
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Threading;

    using FarsiLibrary.Resources;
    using FarsiLibrary.Utils;
    using FarsiLibrary.Utils.Exceptions;

    using System.Collections.Generic;

    /// <summary>
    /// PersianDate class to work with dates in Jalali calendar transparently.
 	/// <example>An example on how to convert current System.DateTime to PersianDate.
    /// <code>
    ///		class MyClass 
    ///     {
    ///		   public static void Main() 
    ///        {
    ///		      Console.WriteLine("Current Persian Date Is : " + PersianDate.Now.ToString());
    ///		   }
    ///		}
    /// </code>
    /// You can alternatively create a specified date/time based on specific <c>DateTime</c> value. To do this
    /// you need to use <value>PersianDateConverter</value> class.
    /// </example>
    /// <seealso cref="PersianDateConverter"/>
    /// </summary>
    [TypeConverter("FarsiLibrary.Win.Design.PersianDateTypeConverter")]
    [Serializable]
    public sealed class PersianDate : 
        IFormattable,
        ICloneable, 
        IComparable, 
        IComparable<PersianDate>,
        IComparer, 
        IComparer<PersianDate>,
        IEquatable<PersianDate>
    {
        #region Month Names

        public class PersianMonthNames
        {
            #region Fields

            public string Farvardin = "فروردین";
            public string Ordibehesht = "ارديبهشت";
            public string Khordad = "خرداد";
            public string Tir = "تير";
            public string Mordad = "مرداد";
            public string Shahrivar = "شهریور";
            public string Mehr = "مهر";
            public string Aban = "آبان";
            public string Azar = "آذر";
            public string Day = "دی";
            public string Bahman = "بهمن";
            public string Esfand = "اسفند";

            private static PersianMonthNames def;

            #endregion

            #region Ctor

            static PersianMonthNames()
            {
                def = new PersianMonthNames();
            }

            #endregion

            #region Indexer

            public static PersianMonthNames Default
            {
                get { return def; }
            }

            public string this[int month]
            {
                get
                {
                    return GetName(month);
                }
            }

            #endregion

            #region Methods

            private string GetName(int monthNo)
            {
                switch (monthNo)
                {
                    case 1:
                        return Farvardin;

                    case 2:
                        return Ordibehesht;

                    case 3:
                        return Khordad;

                    case 4:
                        return Tir;

                    case 5:
                        return Mordad;

                    case 6:
                        return Shahrivar;

                    case 7:
                        return Mehr;

                    case 8:
                        return Aban;

                    case 9:
                        return Azar;

                    case 10:
                        return Day;

                    case 11:
                        return Bahman;

                    case 12:
                        return Esfand;

                    default:
                        throw new ArgumentOutOfRangeException("Month value " + monthNo + " is out of range");
                }
            }

            #endregion
        }

        #endregion

        #region WeekDay Names

        public class PersianWeekDayNames
        {
            #region fields

            public string Shanbeh = "شنبه";
            public string Yekshanbeh = "یکشنبه";
            public string Doshanbeh = "دوشنبه";
            public string Seshanbeh = "ﺳﻪشنبه";
            public string Chaharshanbeh = "چهارشنبه";
            public string Panjshanbeh = "پنجشنبه";
            public string Jomeh = "جمعه";
            private static PersianWeekDayNames def;

            #endregion

            #region Ctor

            static PersianWeekDayNames()
            {
                def = new PersianWeekDayNames();
            }

            #endregion

            #region Indexer

            public static PersianWeekDayNames Default
            {
                get { return def; }
            }

            public string this[int day]
            {
                get { return GetName(day); }
            }

            #endregion

            #region Methods

            private string GetName(int WeekDayNo)
            {
                switch (WeekDayNo)
                {
                    case 0:
                        return Shanbeh;

                    case 1:
                        return Yekshanbeh;

                    case 2:
                        return Doshanbeh;

                    case 3:
                        return Seshanbeh;

                    case 4:
                        return Chaharshanbeh;

                    case 5:
                        return Panjshanbeh;

                    case 6:
                        return Jomeh;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            #endregion
        }

        #endregion

        #region WeekDay Abbreviation

        public class PersianWeekDayAbbr
        {
            #region Fields

            public string Shanbeh = "ش";
            public string Yekshanbeh = "ی";
            public string Doshanbeh = "د";
            public string Seshanbeh = "س";
            public string Chaharshanbeh = "چ";
            public string Panjshanbeh = "پ";
            public string Jomeh = "ج";
            private static PersianWeekDayAbbr def;

            #endregion

            #region Ctor

            static PersianWeekDayAbbr()
            {
                def = new PersianWeekDayAbbr();
            }

            #endregion

            #region Indexer

            public static PersianWeekDayAbbr Default
            {
                get { return def; }
            }

            public string this[int day]
            {
                get { return GetName(day); }
            }

            #endregion

            #region Methods

            private string GetName(int WeekDayNo)
            {
                switch (WeekDayNo)
                {
                    case 0:
                        return Shanbeh;

                    case 1:
                        return Yekshanbeh;

                    case 2:
                        return Doshanbeh;

                    case 3:
                        return Seshanbeh;

                    case 4:
                        return Chaharshanbeh;

                    case 5:
                        return Panjshanbeh;

                    case 6:
                        return Jomeh;

                    default:
                        throw new ArgumentOutOfRangeException("WeekDay number is out of range");
                }
            }

            #endregion
        }

        #endregion

        #region Fields

        private int year;
        private int month;
        private int day;
        private int hour;
        private int minute;
        private int second;
        private int millisecond;
        private TimeSpan time;
        private static PersianDate now;
        private string amDesignator = "ق.ظ";
        private string pmDesignator = "ب.ظ";
        private PersianCalendar pc = new PersianCalendar();

        [NonSerialized]
        public static PersianDate MinValue;

        [NonSerialized]
        public static PersianDate MaxValue;

        #endregion

        #region Static Ctor
        
        /// <summary>
        /// Static constructor initializes Now property of PersianDate and Min/Max values.
        /// </summary>
        static PersianDate()
        {
            now = PersianDateConverter.ToPersianDate(DateTime.Now);
            MinValue = new PersianDate(1, 1, 1, 12, 0, 0, 0); // 12:00:00.000 AM, 22/03/0622
            MaxValue = new PersianDate(DateTime.MaxValue);
        }

        #endregion

        #region Props

        /// <summary>
        /// AMDesignator.
        /// </summary>
        public string AMDesignator
        {
            get { return amDesignator; }
        }

        /// <summary>
        /// PMDesignator.
        /// </summary>
        public string PMDesignator
        {
            get { return pmDesignator; }
        }

        /// <summary>
        /// Current date/time in PersianDate format.
        /// </summary>
        [Browsable(false)]
        [Description("Current date/time in PersianDate format")]
        public static PersianDate Now
        {
            get { return now; }
        }

        /// <summary>
        /// Year value of PersianDate.
        /// </summary>
        [Description("Year value of PersianDate")]
        [NotifyParentProperty(true)]
        public int Year
        {
            get { return year; }
            set 
            {
                CheckYear(value);
                year = value; 
            }
        }

        /// <summary>
        /// Month value of PersianDate.
        /// </summary>
        [Description("Month value of PersianDate")]
        [NotifyParentProperty(true)]
        public int Month
        {
            get { return month; }
            set 
            {
                CheckMonth(value);
                month = value; 
            }
        }

        /// <summary>
        /// Day value of PersianDate.
        /// </summary>
        [Description("Day value of PersianDate")]
        [NotifyParentProperty(true)]
        public int Day
        {
            get { return day; }
            set 
            {
                CheckDay(Year, Month, value);
                day = value; 
            }
        }

        /// <summary>
        /// Hour value of PersianDate.
        /// </summary>
        [Description("Hour value of PersianDate")]
        [NotifyParentProperty(true)]
        public int Hour
        {
            get { return hour; }
            set 
            {
                CheckHour(value);
                hour = value; 
            }
        }

        /// <summary>
        /// Minute value of PersianDate.
        /// </summary>
        [Description("Minute value of PersianDate")]
        [NotifyParentProperty(true)]
        public int Minute
        {
            get { return minute; }
            set 
            {
                CheckMinute(value);
                minute = value; 
            }
        }

        /// <summary>
        /// Second value of PersianDate.
        /// </summary>
        [Description("Second value of PersianDate")]
        [NotifyParentProperty(true)]
        public int Second
        {
            get { return second; }
            set 
            {
                CheckSecond(value);
                second = value; 
            }
        }

        /// <summary>
        /// Millisecond value of PersianDate.
        /// </summary>
        [Description("Millisecond value of PersianDate")]
        [NotifyParentProperty(true)]
        public int Millisecond
        {
            get { return millisecond; }
            set 
            {
                CheckMillisecond(value);
                millisecond = value; 
            }
        }

        /// <summary>
        /// Time value of PersianDate in TimeSpan format.
        /// </summary>
        [Browsable(false)]
        [Description("Time value of PersianDate in TimeSpan format.")]
        public TimeSpan Time
        {
            get { return time; }
        }

        /// <summary>
        /// Returns the DayOfWeek of the date instance
        /// </summary>
        public PersianDayOfWeek DayOfWeek
        {
            get
            {
                DateTime dt = this;
                switch (dt.DayOfWeek)
                {
                    case System.DayOfWeek.Saturday:
                        return PersianDayOfWeek.Saturday;

                    case System.DayOfWeek.Sunday:
                        return PersianDayOfWeek.Sunday;

                    case System.DayOfWeek.Monday:
                        return PersianDayOfWeek.Monday;

                    case System.DayOfWeek.Tuesday:
                        return PersianDayOfWeek.Tuesday;

                    case System.DayOfWeek.Wednesday:
                        return PersianDayOfWeek.Wednesday;

                    case System.DayOfWeek.Thursday:
                        return PersianDayOfWeek.Tuesday;

                    default:
                    case System.DayOfWeek.Friday:
                        return PersianDayOfWeek.Friday;
                }
            }
        }

        /// <summary>
        /// Localized name of PersianDate months.
        /// </summary>
        [Browsable(false)]
        [Description("Localized name of PersianDate months")]
        public string LocalizedMonthName
        {
            get { return PersianMonthNames.Default[month]; }
        }

        /// <summary>
        /// Weekday names of this instance in localized format.
        /// </summary>
        [Browsable(false)]
        [Description("Weekday names of this instance in localized format.")]
        public string LocalizedWeekDayName
        {
            get
            {
                return PersianDateConverter.DayOfWeek(this);
            }
        }

        /// <summary>
        /// Number of days in this month.
        /// </summary>
        [Browsable(false)]
        [Description("Number of days in this month")]
        public int MonthDays
        {
            get
            {
                return PersianDateConverter.MonthDays(month);
            }
        }

        [Browsable(false)]
        public bool IsNull
        {
            get
            {
                return Year == MinValue.Year && Month == MinValue.Month && Day == MinValue.Day;
            }
        }

        #endregion

        #region Ctor

        public PersianDate(DateTime dt)
        {
            Assign(PersianDateConverter.ToPersianDate(dt));
        }

        public PersianDate()
        {
            year = Now.year;
            month = Now.Month;
            day = Now.Day;
            hour = Now.Hour;
            minute = Now.Minute;
            second = Now.Second;
            millisecond = Now.Millisecond;
            time = Now.Time;
        }

        /// <summary>
        /// Constructs a PersianDate instance with values provided in datetime string. 
        /// use the format you want to parse the string with. Currently it can be either g, G, or d value.
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="format"></param>
        public PersianDate(string datetime, string format)
        {
            Assign(Parse(datetime, format));
        }

        /// <summary>
        /// Constructs a PersianDate instance with values provided in datetime string. You should
        /// include Date part only in <c>Date</c> and set the Time of the instance as a <c>TimeSpan</c>.
        /// </summary>
        /// <exception cref="InvalidPersianDateException"></exception>
        /// <param name="Date"></param>
        /// <param name="time"></param>
        public PersianDate(string Date, TimeSpan time)
        {
            PersianDate pd = Parse(Date);

            pd.Hour = time.Hours;
            pd.Minute = time.Minutes;
            pd.Second = time.Seconds;
            pd.Millisecond = time.Milliseconds;

            Assign(pd);
        }

        /// <summary>
        /// Constructs a PersianDate instance with values provided as a string. The provided string should be in format 'yyyy/mm/dd'.
        /// </summary>
        /// <exception cref="InvalidPersianDateException"></exception>
        /// <param name="Date"></param>
        public PersianDate(string Date)
        {
            Assign(Parse(Date));
        }

        /// <summary>
        /// Constructs a PersianDate instance with values specified as <c>Integer</c> and default second and millisecond set to zero.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        public PersianDate(int year, int month, int day, int hour, int minute) : this(year, month, day, hour, minute, 0)
        {
        }

        /// <summary>
        /// Constructs a PersianDate instance with values specified as <c>Integer</c> and default millisecond set to zero.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        public PersianDate(int year, int month, int day, int hour, int minute, int second) : this(year, month, day, hour, minute, second, 0)
        { 
        }

        /// <summary>
        /// Constructs a PersianDate instance with values specified as <c>Integer</c>.
        /// </summary>
        /// <exception cref="InvalidPersianDateException"></exception>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        public PersianDate(int year, int month, int day, int hour, int minute, int second, int millisecond)
        {
            CheckYear(year);
            CheckMonth(month);
            CheckDay(year, month, day);
            CheckHour(hour);
            CheckMinute(minute);
            CheckSecond(second);
            CheckMillisecond(millisecond);

            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.millisecond = millisecond;

            time = new TimeSpan(0, hour, minute, second, millisecond);
        }

        /// <summary>
        /// Constructs a PersianDate instance with values specified as <c>Integer</c>. Time value of this instance is set to <c>DateTime.Now</c>.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public PersianDate(int year, int month, int day) 
        {
            CheckYear(year);
            CheckMonth(month);
            CheckDay(year, month, day);

            this.year = year;
            this.month = month;
            this.day = day;
            hour = DateTime.Now.Hour;
            minute = DateTime.Now.Minute;
            second = DateTime.Now.Second;
            millisecond = DateTime.Now.Millisecond;
            
            time = new TimeSpan(0, hour, minute, second, millisecond);
        }

        #endregion

        #region Private Check Methods

        private void CheckYear(int YearNo)
        {
            if (YearNo < 1 || YearNo > 9999)
            {
                throw new InvalidPersianDateException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidYear));
            }
        }

        private void CheckMonth(int MonthNo)
        {
            if (MonthNo > 12 || MonthNo < 1)
            {
                throw new InvalidPersianDateException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidMonth));
            }
        }

        private void CheckDay(int YearNo, int MonthNo, int DayNo)
        {
            if (MonthNo < 6 && DayNo > 31)
                throw new InvalidPersianDateException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDay));

            if (MonthNo > 6 && DayNo > 30)
                throw new InvalidPersianDateException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDay));

            if (MonthNo == 12 && DayNo > 29)
            {
                if (!pc.IsLeapDay(YearNo, MonthNo, DayNo) || DayNo > 30)
                    throw new InvalidPersianDateException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDay));
            }
        }

        private void CheckHour(int HourNo)
        {
            if (HourNo > 24 || HourNo < 0)
            {
                throw new InvalidPersianDateException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidHour));
            }
        }

        private void CheckMinute(int MinuteNo)
        {
            if (MinuteNo > 60 || MinuteNo < 0)
            {
                throw new InvalidPersianDateException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidMinute));
            }
        }

        private void CheckSecond(int SecondNo)
        {
            if (SecondNo > 60 || SecondNo < 0)
            {
                throw new InvalidPersianDateException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidSecond));
            }
        }

        private void CheckMillisecond(int MillisecondNo)
        {
            if (MillisecondNo < 0 || MillisecondNo > 1000)
            {
                throw new InvalidPersianDateException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidMillisecond));
            }
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Assigns an instance of PersianDate's values to this instance.
        /// </summary>
        /// <param name="pd"></param>
        public void Assign(PersianDate pd)
        {
            Year = pd.Year;
            Month = pd.Month;
            Day = pd.Day;
            Hour = pd.Hour;
            Minute = pd.Minute;
            Second = pd.Second;
        }

        /// <summary>
        /// Returns a string representation of current PersianDate value.
        /// </summary>
        /// <returns></returns>
        public string ToWritten()
        {
            return (LocalizedWeekDayName + " " + day.ToString() + " " + LocalizedMonthName + " " + year.ToString());
        }

        #endregion

        #region Parse Methods

        /// <summary>
        /// Parse a string value into a PersianDate instance. Value could be either in 'yyyy/mm/dd hh:mm:ss' or 'yyyy/mm/dd' formats. If you want to parse <c>Time</c> value too,
        /// you should set <c>includesTime</c> to <c>true</c>.
        /// </summary>
        /// <exception cref="InvalidPersianDateException"></exception>
        /// <param name="value"></param>
        /// <param name="includesTime"></param>
        /// <returns></returns>
        public static PersianDate Parse(string value, bool includesTime)
        {
            if (value == string.Empty)
                return MinValue;

            if (includesTime)
            {
                if (value.Length > 19)
                {
                    throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDateTimeLength));
                }

                string[] dt = value.Split(" ".ToCharArray());

                if (dt.Length != 2)
                {
                    throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDateFormat));
                }

                string _date = dt[0];
                string _time = dt[1];

                string[] dateParts = _date.Split("/".ToCharArray());
                string[] timeParts = _time.Split(":".ToCharArray());

                if (dateParts.Length != 3)
                {
                    throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDateFormat));
                }

                if (timeParts.Length != 3)
                {
                    throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidTimeFormat));
                }

                int day = int.Parse(dateParts[2]);
                int month = int.Parse(dateParts[1]);
                int year = int.Parse(dateParts[0]);
                int hour = int.Parse(timeParts[0]);
                int minute = int.Parse(timeParts[1]);
                int second = int.Parse(timeParts[2]);

                return new PersianDate(year, month, day, hour, minute, second);
            }
            else
            {
                return Parse(value);
            }
        }

        public static PersianDate Parse(string value, string format)
        {
            switch(format)
            {
                case "G": //yyyy/mm/dd hh:mm:ss tt
                    return ParseFullDateTime(value);

                case "g": //yyyy/mm/dd hh:mm tt
                    return ParseDateShortTime(value);
                    
                case "d": //yyyy/mm/dd
                    return Parse(value);
                    
                default:
                    throw new ArgumentException("Currently G,g,d formats are supported.");
            }
        }

        /// <summary>
        /// Parse a string value into a PersianDate instance. Value should be in 'yyyy/mm/dd hh:mm:ss tt' formats.
        /// </summary>
        /// <exception cref="InvalidPersianDateException"></exception>
        /// <param name="value"></param>
        /// <returns></returns>
        private static PersianDate ParseFullDateTime(string value)
        {
            if (value == string.Empty)
                return MinValue;

            if (value.Length > 23)
                throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDateTimeLength));

            string[] dt = value.Split(" ".ToCharArray());

            if (dt.Length != 3)
                throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDateFormat));

            string _date = dt[0];
            string _time = dt[1];

            string[] dateParts = _date.Split("/".ToCharArray());
            string[] timeParts = _time.Split(":".ToCharArray());

            if (dateParts.Length != 3)
                throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDateFormat));

            if (timeParts.Length != 3)
                throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidTimeFormat));

            int day = int.Parse(dateParts[2]);
            int month = int.Parse(dateParts[1]);
            int year = int.Parse(dateParts[0]);
            int hour = int.Parse(timeParts[0]);
            int minute = int.Parse(timeParts[1]);
            int second = int.Parse(timeParts[2]);
            
            return new PersianDate(year, month, day, hour, minute, second, 0);
        }

        /// <summary>
        /// Parse a string value into a PersianDate instance. Value should be in 'yyyy/mm/dd hh:mm tt' formats.
        /// </summary>
        /// <exception cref="InvalidPersianDateException"></exception>
        /// <param name="value"></param>
        /// <returns></returns>
        private static PersianDate ParseDateShortTime(string value)
        {
            if (value == string.Empty)
                return MinValue;

            if (value.Length > 20)
                throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDateTimeLength));

            string[] dt = value.Split(" ".ToCharArray());

            if (dt.Length != 3)
                throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDateFormat));

            string _date = dt[0];
            string _time = dt[1];

            string[] dateParts = _date.Split("/".ToCharArray());
            string[] timeParts = _time.Split(":".ToCharArray());

            if (dateParts.Length != 3)
                throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDateFormat));

            if (timeParts.Length != 2)
                throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidTimeFormat));

            int day = int.Parse(dateParts[2]);
            int month = int.Parse(dateParts[1]);
            int year = int.Parse(dateParts[0]);
            int hour = int.Parse(timeParts[0]);
            int minute = int.Parse(timeParts[1]);

            return new PersianDate(year, month, day, hour, minute, 0, 0);
        }

        /// <summary>
        /// Parse a string value into a PersianDate instance. Value can only be in 'yyyy/mm/dd' format.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PersianDate Parse(string value)
        {
            if (value.Length == 10)
                return ParseShortDate(value);
            else if (value.Length == 20)
                return ParseDateShortTime(value);
            else if (value.Length == 23)
                return ParseFullDateTime(value);

            throw new InvalidPersianDateFormatException("Can not parse the value. Format is incorrect.");
        }

        private static PersianDate ParseShortDate(string value)
        {
            if (value == string.Empty)
                return MinValue;

            if (value.Length > 10)
            {
                throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDateTimeLength));
            }

            string[] dateParts = value.Split("/".ToCharArray());

            if (dateParts.Length != 3)
            {
                throw new InvalidPersianDateFormatException(FALocalizeManager.GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture).GetLocalizedString(StringID.PersianDate_InvalidDateFormat));
            }

            int day = int.Parse(dateParts[2]);
            int month = int.Parse(dateParts[1]);
            int year = int.Parse(dateParts[0]);

            return new PersianDate(year, month, day);
        }
        
        #endregion

        #region Overrides

        /// <summary>
        /// Returns Date in 'yyyy/mm/dd' string format.
        /// </summary>
        /// <returns>string representation of evaluated Date.</returns>
        /// <example>An example on how to get the written form of a Date.
        /// <code>
        ///		class MyClass {
        ///		   public static void Main()
        ///		   {	
        ///				Console.WriteLine(PersianDate.Now.ToString());
        ///		   }
        ///		}
        /// </code>
        /// </example>
        /// <seealso cref="ToWritten"/>
        public override string ToString()
        {
            return ToString("g", null);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Compares two instance of the PersianDate for the specified operator.
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool operator ==(PersianDate date1, PersianDate date2)
        {
            if((date1 as object) == null && (date2 as object) == null)
                return true;
            
            if((date1 as object) == null && (date2 as object) != null)
                return false;
            
            if((date2 as object) == null && (date1 as object) != null)
                return false;
                
            return date1.Year == date2.Year &&
                   date1.Month == date2.Month &&
                   date1.Day == date2.Day &&
                   date1.Hour == date2.Hour &&
                   date1.Minute == date2.Minute &&
                   date1.Second == date2.Second &&
                   date1.Millisecond == date2.Millisecond;
        }

        /// <summary>
        /// Compares two instance of the PersianDate for the specified operator.
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool operator !=(PersianDate date1, PersianDate date2)
        {
            if ((date1 as object) == null && (date2 as object) == null)
                return false;

            if ((date1 as object) == null && (date2 as object) != null)
                return true;

            if ((date2 as object) == null && (date1 as object) != null)
                return true;

            return date1.Year != date2.Year ||
                   date1.Month != date2.Month ||
                   date1.Day != date2.Day ||
                   date1.Hour != date2.Hour ||
                   date1.Minute != date2.Minute ||
                   date1.Second != date2.Second ||
                   date1.Millisecond != date2.Millisecond;
        }

        /// <summary>
        /// Compares two instance of the PersianDate for the specified operator.
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool operator >(PersianDate date1, PersianDate date2)
        {
            if ((date1 as object) == null && (date2 as object) == null)
                return false;

            if ((date1 as object) == null && (date2 as object) != null)
                throw new NullReferenceException();

            if ((date2 as object) == null && (date1 as object) != null)
                throw new NullReferenceException();

            if (date1.Year > date2.Year)
                return true;

            if (date1.Year == date2.Year && 
                date1.Month > date2.Month)
                return true;

            if (date1.Year == date2.Year &&
               date1.Month == date2.Month &&
               date1.Day > date2.Day)
                return true;

            if (date1.Year == date2.Year &&
               date1.Month == date2.Month &&
               date1.Day == date2.Day &&
               date1.Hour > date2.Hour)
                return true;

            if (date1.Year == date2.Year &&
               date1.Month == date2.Month &&
               date1.Day == date2.Day &&
               date1.Hour == date2.Hour &&
               date1.Minute > date2.Minute)
                return true;

            if (date1.Year == date2.Year &&
               date1.Month == date2.Month &&
               date1.Day == date2.Day &&
               date1.Hour == date2.Hour &&
               date1.Minute == date2.Minute &&
               date1.Second > date2.Second)
                return true;

            if (date1.Year == date2.Year &&
                date1.Month == date2.Month &&
                date1.Day == date2.Day &&
                date1.Hour == date2.Hour &&
                date1.Minute == date2.Minute &&
                date1.Second == date2.Second && 
                date1.Millisecond > date2.Millisecond)
                 return true;

            return false;
        }

        /// <summary>
        /// Compares two instance of the PersianDate for the specified operator.
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool operator <(PersianDate date1, PersianDate date2)
        {
            if ((date1 as object) == null && (date2 as object) == null)
                return false;

            if ((date1 as object) == null && (date2 as object) != null)
                throw new NullReferenceException();

            if ((date2 as object) == null && (date1 as object) != null)
                throw new NullReferenceException();

            if (date1.Year < date2.Year)
                return true;

            if (date1.Year == date2.Year &&
                date1.Month < date2.Month)
                return true;

            if (date1.Year == date2.Year &&
               date1.Month == date2.Month &&
               date1.Day < date2.Day)
                return true;

            if (date1.Year == date2.Year &&
               date1.Month == date2.Month &&
               date1.Day == date2.Day &&
               date1.Hour < date2.Hour)
                return true;

            if (date1.Year == date2.Year &&
               date1.Month == date2.Month &&
               date1.Day == date2.Day &&
               date1.Hour == date2.Hour &&
               date1.Minute < date2.Minute)
                return true;

            if (date1.Year == date2.Year &&
               date1.Month == date2.Month &&
               date1.Day == date2.Day &&
               date1.Hour == date2.Hour &&
               date1.Minute == date2.Minute &&
               date1.Second < date2.Second)
                return true;

            if (date1.Year == date2.Year &&
               date1.Month == date2.Month &&
               date1.Day == date2.Day &&
               date1.Hour == date2.Hour &&
               date1.Minute == date2.Minute &&
               date1.Second == date2.Second &&
               date1.Millisecond < date2.Millisecond)
                return true;

            return false;
        }

        /// <summary>
        /// Compares two instance of the PersianDate for the specified operator.
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool operator <=(PersianDate date1, PersianDate date2)
        {
            return (date1 < date2) || (date1 == date2);
        }

        /// <summary>
        /// Compares two instance of the PersianDate for the specified operator.
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool operator >=(PersianDate date1, PersianDate date2)
        {
            return (date1 > date2) || (date1 == date2);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. System.Object.GetHashCode() is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString("s").GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified System.Object instances are considered equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is PersianDate)
                return this == (PersianDate)obj;
            else
                return false;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new PersianDate(Year, Month, Day, Hour, Minute, Second, Millisecond);
        }

        #endregion

        #region Implicit Casting
        
        public static implicit operator DateTime(PersianDate pd)
        {
            return PersianDateConverter.ToGregorianDateTime(pd);
        }

        public static implicit operator PersianDate(DateTime dt)
        {
            if (dt.Equals(DateTime.MinValue))
                return MinValue;
            
            return PersianDateConverter.ToPersianDate(dt);
        }

        #endregion

        #region ICompareable Interface
        
        ///<summary>
        ///Compares the current instance with another object of the same type.
        ///</summary>
        ///
        ///<returns>
        ///A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than obj. Zero This instance is equal to obj. Greater than zero This instance is greater than obj. 
        ///</returns>
        ///
        ///<param name="obj">An object to compare with this instance. </param>
        ///<exception cref="T:System.ArgumentException">obj is not the same type as this instance. </exception><filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            if (!(obj is PersianDate))
                throw new ArgumentException("Comparing value is not of type PersianDate.");
            
            PersianDate pd = (PersianDate)obj;

            if (pd < this)
                return 1;
            else if(pd > this)
                return -1;

            return 0;
        }

        #endregion

        #region IComparer
        
        ///<summary>
        ///Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        ///</summary>
        ///
        ///<returns>
        ///Value Condition Less than zero x is less than y. Zero x equals y. Greater than zero x is greater than y. 
        ///</returns>
        ///
        ///<param name="y">The second object to compare. </param>
        ///<param name="x">The first object to compare. </param>
        ///<exception cref="T:System.ArgumentException">Neither x nor y implements the <see cref="T:System.IComparable"></see> interface.-or- x and y are of different types and neither one can handle comparisons with the other. </exception><filterpriority>2</filterpriority>
        ///<exception cref="T:System.ApplicationException">Either x or y is a null reference</exception>
        public int Compare(object x, object y)
        {
            if (x == null || y == null)
                throw new ApplicationException("Invalid PersianDate comparer.");

            if (!(x is PersianDate))
                throw new ArgumentException("x value is not of type PersianDate.");

            if (!(y is PersianDate))
                throw new ArgumentException("y value is not of type PersianDate.");
            
            PersianDate pd1 = (PersianDate) x;
            PersianDate pd2 = (PersianDate) y;

            if (pd1 > pd2)
                return 1;
            else if (pd1 < pd2)
                return -1;

            return 0;
        }

        #endregion

        #region IComparer<T> Implementation
        
        ///<summary>
        ///Compares the current object with another object of the same type.
        ///</summary>
        ///
        ///<returns>
        ///A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other. 
        ///</returns>
        ///
        ///<param name="other">An object to compare with this object.</param>
        public int CompareTo(PersianDate other)
        {
            if (other < this)
                return 1;
            else if (other > this)
                return -1;

            return 0;
        }

        ///<summary>
        ///Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        ///</summary>
        ///
        ///<returns>
        ///Value Condition Less than zerox is less than y.Zerox equals y.Greater than zero x is greater than y.
        ///</returns>
        ///
        ///<param name="y">The second object to compare.</param>
        ///<param name="x">The first object to compare.</param>
        public int Compare(PersianDate x, PersianDate y)
        {
            if (x > y)
                return 1;
            else if (x < y)
                return -1;

            return 0;
        }

        #endregion

        #region IEquatable<T>
        
        ///<summary>
        ///Indicates whether the current object is equal to another object of the same type.
        ///</summary>
        ///
        ///<returns>
        ///true if the current object is equal to the other parameter; otherwise, false.
        ///</returns>
        ///
        ///<param name="other">An object to compare with this object.</param>
        public bool Equals(PersianDate other)
        {
            return this == other;
        }

        #endregion
        
        #region IFormattable
        
        /// <summary>
        /// Returns string representation of this instance in default format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// Returns string representation of this instance and format it using the <see cref="IFormatProvider"/> instance.
        /// </summary>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }
        
        /// <summary>
        /// Returns string representation of this instance in desired format, or using provided <see cref="IFormatProvider"/> instance.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null) format = "G";
            int smallhour = (Hour > 12) ? Hour - 12 : Hour;
            string designator = Hour > 12 ? PMDesignator : AMDesignator;
            
            if (formatProvider != null)
            {
                ICustomFormatter formatter = formatProvider.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;
                if (formatter != null)
                    return formatter.Format(format, this, formatProvider);
            }
            switch(format)
            {
                case "D":
                    //'yyyy mm dd dddd' e.g. 'دوشنبه 20 شهریور 1384'
                    return string.Format("{0} {1} {2} {3}", new object[4] { LocalizedWeekDayName, Util.toDouble(Day), LocalizedMonthName, Year });
                    
                case "f":
                    //'hh:mm yyyy mmmm dd dddd' e.g. 'دوشنبه 20 شهریور 1384 21:30'
                    return string.Format("{0} {1} {2} {3} {4}:{5}", new object[6] { LocalizedWeekDayName, Util.toDouble(Day), LocalizedMonthName, Year, Util.toDouble(Hour), Util.toDouble(Minute) });

                case "F":
                    //'tt hh:mm:ss yyyy mmmm dd dddd' e.g. 'دوشنبه 20 شهریور 1384 02:30:22 ب.ض'
                    return string.Format("{0} {1} {2} {3} {4}:{5}:{6} {7}", new object[8] { LocalizedWeekDayName, Util.toDouble(Day), LocalizedMonthName, Year, Util.toDouble(smallhour), Util.toDouble(Minute), Util.toDouble(Second), designator });
                    
                case "g":
                    //'yyyy/mm/dd hh:mm tt'
                    return string.Format("{0}/{1}/{2} {3}:{4} {5}", new object[6] { Year, Util.toDouble(Month), Util.toDouble(Day), Util.toDouble(smallhour), Util.toDouble(Minute), designator });
                    
                case "G":
                    //'yyyy/mm/dd hh:mm:ss tt'
                    return string.Format("{0}/{1}/{2} {3}:{4}:{5} {6}", new object[7] { Year, Util.toDouble(Month), Util.toDouble(Day), Util.toDouble(smallhour), Util.toDouble(Minute), Util.toDouble(Second), designator });
                
                case "M":
                case "m":
                    //'yyyy mmmm'
                    return string.Format("{0} {1}", Year, LocalizedMonthName);
                    
                case "s":
                    //'yyyy-mm-ddThh:mm:ss'
                    return string.Format("{0}-{1}-{2}T{3}:{4}:{5}", Year, Util.toDouble(Month), Util.toDouble(Day), Util.toDouble(Hour), Util.toDouble(Minute), Util.toDouble(Second));
                    
                case "t":
                    //'hh:mm tt'
                    return string.Format("{0}:{1} {2}", Util.toDouble(smallhour), Util.toDouble(Minute), designator);
                    
                case "T":
                    //'hh:mm:ss tt'
                    return string.Format("{0}:{1}:{2} {3}", new object[4] { Util.toDouble(smallhour), Util.toDouble(Minute), Util.toDouble(Second), designator });

                    
                case "d":
                default:
                    //ShortDatePattern yyyy/mm/dd e.g. '1384/9/1'
                    return string.Format("{0}/{1}/{2}", Year, Util.toDouble(Month), Util.toDouble(Day));

            }
        }
        
	    #endregion
    }
}
