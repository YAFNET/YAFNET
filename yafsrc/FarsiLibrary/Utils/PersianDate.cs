namespace FarsiLibrary.Utils;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using FarsiLibrary.Localization;
using FarsiLibrary.Utils.Exceptions;
using FarsiLibrary.Utils.Internals;

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
public sealed class PersianDate : IFormattable,
                                  ICloneable,
                                  IComparable,
                                  IComparable<PersianDate>,
                                  IComparer,
                                  IComparer<PersianDate>,
                                  IEquatable<PersianDate>
{
    private int year;

    private int month;

    private int day;

    private int hour;

    private int minute;

    private int second;

    private int millisecond;

    private readonly TimeSpan time;

    private static readonly PersianCalendar pc;

    [NonSerialized]
    public static DateTime MinValue;

    [NonSerialized]
    public static DateTime MaxValue;

    /// <summary>
    /// Static constructor initializes Now property of PersianDate and Min/Max values.
    /// </summary>
    static PersianDate()
    {
        MinValue = new DateTime(196037280000000000L); // 12:00:00.000 AM, 22/03/0622
        MaxValue = DateTime.MaxValue;
        pc = new PersianCalendar();
    }

    /// <summary>
    /// Current date/time in PersianDate format.
    /// </summary>
    [Description("Current date/time in PersianDate format")]
    public static PersianDate Now => PersianDateConverter.ToPersianDate(DateTime.Now);

    /// <summary>
    /// Current date in PersianDate format.
    /// </summary>
    [Description("Todays date in PersianDate format")]
    public static PersianDate Today => PersianDateConverter.ToPersianDate(DateTime.Today);

    /// <summary>
    /// Year value of PersianDate.
    /// </summary>
    [Description("Year value of PersianDate")]
    [NotifyParentProperty(true)]
    public int Year
    {
        get => this.year;
        set
        {
            CheckYear(value);
            this.year = value;
        }
    }

    /// <summary>
    /// Month value of PersianDate.
    /// </summary>
    [Description("Month value of PersianDate")]
    [NotifyParentProperty(true)]
    public int Month
    {
        get => this.month;
        set
        {
            CheckMonth(value);
            this.month = value;
        }
    }

    /// <summary>
    /// Day value of PersianDate.
    /// </summary>
    [Description("Day value of PersianDate")]
    [NotifyParentProperty(true)]
    public int Day
    {
        get => this.day;
        set
        {
            this.CheckDay(this.Year, this.Month, value);
            this.day = value;
        }
    }

    /// <summary>
    /// Hour value of PersianDate.
    /// </summary>
    [Description("Hour value of PersianDate")]
    [NotifyParentProperty(true)]
    public int Hour
    {
        get => this.hour;
        set
        {
            CheckHour(value);
            this.hour = value;
        }
    }

    /// <summary>
    /// Minute value of PersianDate.
    /// </summary>
    [Description("Minute value of PersianDate")]
    [NotifyParentProperty(true)]
    public int Minute
    {
        get => this.minute;
        set
        {
            CheckMinute(value);
            this.minute = value;
        }
    }

    /// <summary>
    /// Second value of PersianDate.
    /// </summary>
    [Description("Second value of PersianDate")]
    [NotifyParentProperty(true)]
    public int Second
    {
        get => this.second;
        set
        {
            CheckSecond(value);
            this.second = value;
        }
    }

    /// <summary>
    /// Millisecond value of PersianDate.
    /// </summary>
    [Description("Millisecond value of PersianDate")]
    [NotifyParentProperty(true)]
    public int Millisecond
    {
        get => this.millisecond;
        set
        {
            CheckMillisecond(value);
            this.millisecond = value;
        }
    }

    /// <summary>
    /// Time value of PersianDate in TimeSpan format.
    /// </summary>
    [Browsable(false)]
    [Description("Time value of PersianDate in TimeSpan format.")]
    public TimeSpan Time => this.time;

    /// <summary>
    /// Returns the DayOfWeek of the date instance
    /// </summary>
    public DayOfWeek DayOfWeek
    {
        get
        {
            DateTime dt = this;
            return dt.DayOfWeek;
        }
    }

    /// <summary>
    /// Localized name of PersianDate months.
    /// </summary>
    [Browsable(false)]
    [Description("Localized name of PersianDate months")]
    public string LocalizedMonthName => PersianMonthNames.Default[this.month - 1];

    /// <summary>
    /// Weekday names of this instance in localized format.
    /// </summary>
    [Browsable(false)]
    [Description("Weekday names of this instance in localized format.")]
    public string LocalizedWeekDayName => PersianDateConverter.DayOfWeek(this);

    /// <summary>
    /// Number of days in this month.
    /// </summary>
    [Browsable(false)]
    [Description("Number of days in this month")]
    public int MonthDays => PersianDateConverter.MonthDays(this.month);

    [Browsable(false)]
    public bool IsNull => this.Year <= MinValue.Year && this.Month <= MinValue.Month && this.Day <= MinValue.Day;

    public PersianDate(DateTime dt)
    {
        this.Assign(PersianDateConverter.ToPersianDate(dt));
    }

    public PersianDate()
    {
        var now = Now;
        this.year = now.year;
        this.month = now.Month;
        this.day = now.Day;
        this.hour = now.Hour;
        this.minute = now.Minute;
        this.second = now.Second;
        this.millisecond = now.Millisecond;
        this.time = now.Time;
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
        var pd = Parse(Date);

        pd.Hour = time.Hours;
        pd.Minute = time.Minutes;
        pd.Second = time.Seconds;
        pd.Millisecond = time.Milliseconds;

        this.Assign(pd);
    }

    /// <summary>
    /// Constructs a PersianDate instance with values provided as a string. The provided string should be in format 'yyyy/mm/dd'.
    /// </summary>
    /// <exception cref="InvalidPersianDateException"></exception>
    /// <param name="Date"></param>
    public PersianDate(string Date)
    {
        this.Assign(Parse(Date));
    }

    /// <summary>
    /// Constructs a PersianDate instance with values specified as <c>Integer</c> and default second and millisecond set to zero.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    public PersianDate(int year, int month, int day, int hour, int minute)
        : this(year, month, day, hour, minute, 0)
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
    public PersianDate(int year, int month, int day, int hour, int minute, int second)
        : this(year, month, day, hour, minute, second, 0)
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
        this.CheckDay(year, month, day);
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

        this.time = new TimeSpan(0, hour, minute, second, millisecond);
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
        this.CheckDay(year, month, day);

        this.year = year;
        this.month = month;
        this.day = day;
        this.hour = 0;
        this.minute = 0;
        this.second = 0;
        this.millisecond = 0;
        this.time = new TimeSpan(0, this.hour, this.minute, this.second, this.millisecond);
    }

    private static void CheckYear(int YearNo)
    {
        if (YearNo is < 1 or > 9999)
        {
            throw new InvalidPersianDateException(
                FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidYear),
                YearNo);
        }
    }

    private static void CheckMonth(int MonthNo)
    {
        if (MonthNo is > 12 or < 1)
        {
            throw new InvalidPersianDateException(
                FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidMonth),
                MonthNo);
        }
    }

    private void CheckDay(int YearNo, int MonthNo, int DayNo)
    {
        if (MonthNo < 6 && DayNo > 31)
            throw new InvalidPersianDateException(
                FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidDay),
                DayNo);

        if (MonthNo > 6 && DayNo > 30)
            throw new InvalidPersianDateException(
                FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidDay),
                DayNo);

        if (MonthNo == 12 && DayNo > 29)
        {
            if (!pc.IsLeapDay(YearNo, MonthNo, DayNo) || DayNo > 30)
                throw new InvalidPersianDateException(
                    FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidDay),
                    DayNo);
        }
    }

    private static void CheckHour(int HourNo)
    {
        if (HourNo is > 24 or < 0)
        {
            throw new InvalidPersianDateException(
                FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidHour),
                HourNo);
        }
    }

    private static void CheckMinute(int MinuteNo)
    {
        if (MinuteNo is > 60 or < 0)
        {
            throw new InvalidPersianDateException(
                FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidMinute),
                MinuteNo);
        }
    }

    private static void CheckSecond(int SecondNo)
    {
        if (SecondNo is > 60 or < 0)
        {
            throw new InvalidPersianDateException(
                FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidSecond));
        }
    }

    private static void CheckMillisecond(int MillisecondNo)
    {
        if (MillisecondNo is < 0 or > 1000)
        {
            throw new InvalidPersianDateException(
                FALocalizeManager.Instance.GetLocalizer()
                    .GetLocalizedString(StringID.PersianDate_InvalidMillisecond));
        }
    }

    /// <summary>
    /// Assigns an instance of PersianDate's values to this instance.
    /// </summary>
    /// <param name="pd"></param>
    internal void Assign(PersianDate pd)
    {
        this.Year = pd.Year;
        this.Month = pd.Month;
        this.Day = pd.Day;
        this.Hour = pd.Hour;
        this.Minute = pd.Minute;
        this.Second = pd.Second;
    }

    /// <summary>
    /// Returns a string representation of current PersianDate value.
    /// </summary>
    /// <returns></returns>
    public string ToWritten()
    {
        return $"{this.LocalizedWeekDayName} {this.day} {this.LocalizedMonthName} {this.year}";
    }

    /// <summary>
    /// Returns a pretty representation of this date instance
    /// </summary>
    /// <returns></returns>
    public string ToPrettyDate()
    {
        return null;
    }

    /// <summary>
    /// Tries to parse a string value into a PersianDate instance. 
    /// Value can only be in 'yyyy/mm/dd' format.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="persianDate"></param>
    /// <returns></returns>
    public static bool TryParse(string value, out PersianDate persianDate)
    {
        persianDate = null;

        try
        {
            persianDate = Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Parse a string value into a PersianDate instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static PersianDate Parse(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                var dt = DateTime.Parse(value, CultureHelper.PersianCulture, DateTimeStyles.None);

                var year = pc.GetYear(dt);
                var month = pc.GetMonth(dt);
                var day = pc.GetDayOfMonth(dt);

                // Fixed: If year is 2 digit, it is probably 13xx.
                if (year < 100)
                    year = 1300 + year;

                return new PersianDate(year, month, day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
            }
            catch (Exception)
            {
                throw new InvalidPersianDateFormatException("Can not parse the value. Format is incorrect.");
            }
        }

        throw new InvalidPersianDateFormatException("Can not parse the value. Format is incorrect.");
    }

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
        return this.ToString("G", null);
    }

    /// <summary>
    /// Compares two instance of the PersianDate for the specified operator.
    /// </summary>
    /// <param name="date1"></param>
    /// <param name="date2"></param>
    /// <returns></returns>
    public static bool operator ==(PersianDate date1, PersianDate date2)
    {
        if (date1 as object == null && date2 as object == null)
            return true;

        if ((object) date1 == null)
            return false;

        if (date2 as object == null)
            return false;

        return date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day &&
               date1.Hour == date2.Hour && date1.Minute == date2.Minute && date1.Second == date2.Second &&
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
        if (date1 as object == null && date2 as object == null)
            return false;

        if (date1 as object == null)
            return true;

        if (date2 as object == null)
            return true;

        return date1.Year != date2.Year || date1.Month != date2.Month || date1.Day != date2.Day ||
               date1.Hour != date2.Hour || date1.Minute != date2.Minute || date1.Second != date2.Second ||
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
        if (date1 as object == null && date2 as object == null)
            return false;

        if (date1 as object == null && date2 as object != null)
            throw new InvalidOperationException("date can not be null.");

        if (date2 as object == null && date1 as object != null)
            throw new InvalidOperationException("date can not be null.");

        if (date1.Year > date2.Year)
            return true;

        if (date1.Year == date2.Year && date1.Month > date2.Month)
            return true;

        if (date1.Year == date2.Year && date1.Month == date2.Month && date1.Day > date2.Day)
            return true;

        if (date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day &&
            date1.Hour > date2.Hour)
            return true;

        if (date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day &&
            date1.Hour == date2.Hour && date1.Minute > date2.Minute)
            return true;

        if (date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day &&
            date1.Hour == date2.Hour && date1.Minute == date2.Minute && date1.Second > date2.Second)
            return true;

        return date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day &&
               date1.Hour == date2.Hour && date1.Minute == date2.Minute && date1.Second == date2.Second &&
               date1.Millisecond > date2.Millisecond;
    }

    /// <summary>
    /// Compares two instance of the PersianDate for the specified operator.
    /// </summary>
    /// <param name="date1"></param>
    /// <param name="date2"></param>
    /// <returns></returns>
    public static bool operator <(PersianDate date1, PersianDate date2)
    {
        if (date1 as object == null && date2 as object == null)
            return false;

        if (date1 as object == null && date2 as object != null)
            throw new InvalidOperationException("date can not be null.");

        if (date2 as object == null && date1 as object != null)
            throw new InvalidOperationException("date can not be null.");

        if (date1.Year < date2.Year)
            return true;

        if (date1.Year == date2.Year && date1.Month < date2.Month)
            return true;

        if (date1.Year == date2.Year && date1.Month == date2.Month && date1.Day < date2.Day)
            return true;

        if (date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day &&
            date1.Hour < date2.Hour)
            return true;

        if (date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day &&
            date1.Hour == date2.Hour && date1.Minute < date2.Minute)
            return true;

        if (date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day &&
            date1.Hour == date2.Hour && date1.Minute == date2.Minute && date1.Second < date2.Second)
            return true;

        return date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day &&
               date1.Hour == date2.Hour && date1.Minute == date2.Minute && date1.Second == date2.Second &&
               date1.Millisecond < date2.Millisecond;
    }

    /// <summary>
    /// Compares two instance of the PersianDate for the specified operator.
    /// </summary>
    /// <param name="date1"></param>
    /// <param name="date2"></param>
    /// <returns></returns>
    public static bool operator <=(PersianDate date1, PersianDate date2)
    {
        return date1 < date2 || date1 == date2;
    }

    /// <summary>
    /// Compares two instance of the PersianDate for the specified operator.
    /// </summary>
    /// <param name="date1"></param>
    /// <param name="date2"></param>
    /// <returns></returns>
    public static bool operator >=(PersianDate date1, PersianDate date2)
    {
        return date1 > date2 || date1 == date2;
    }

    /// <summary>
    /// Serves as a hash function for a particular type. System.Object.GetHashCode() is suitable for use in hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return this.ToString("s").GetHashCode();
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

        return false;
    }

    object ICloneable.Clone()
    {
        return new PersianDate(
            this.Year,
            this.Month,
            this.Day,
            this.Hour,
            this.Minute,
            this.Second,
            this.Millisecond);
    }

    public static implicit operator DateTime(PersianDate pd)
    {
        return PersianDateConverter.ToGregorianDateTime(pd);
    }

    public static implicit operator PersianDate(DateTime dt)
    {
        if (dt > pc.MaxSupportedDateTime || dt < pc.MinSupportedDateTime)
            return null;

        return PersianDateConverter.ToPersianDate(dt);
    }

    /// <summary>
    /// Converts a nullable DateTime to PersianDate.
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static implicit operator PersianDate(DateTime? dt)
    {
        return dt.HasValue ? PersianDateConverter.ToPersianDate(dt.Value) : null;
    }

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
    int IComparable.CompareTo(object obj)
    {
        if (!(obj is PersianDate))
            throw new InvalidOperationException("Comparing value is not of type PersianDate.");

        var pd = (PersianDate)obj;

        if (pd < this)
            return 1;

        if (pd > this)
            return -1;

        return 0;
    }

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
    int IComparer.Compare(object x, object y)
    {
        if (x == null || y == null)
            throw new InvalidOperationException("Invalid PersianDate comparer.");

        if (!(x is PersianDate))
            throw new InvalidOperationException("x value is not of type PersianDate.");

        if (!(y is PersianDate))
            throw new InvalidOperationException("y value is not of type PersianDate.");

        var pd1 = (PersianDate)x;
        var pd2 = (PersianDate)y;

        if (pd1 > pd2)
            return 1;

        if (pd1 < pd2)
            return -1;

        return 0;
    }

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

        if (other > this)
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

        if (x < y)
            return -1;

        return 0;
    }

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

    /// <summary>
    /// Returns string representation of this instance in default format.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string ToString(string format)
    {
        return this.ToString(format, null);
    }

    /// <summary>
    /// Returns string representation of this instance and format it using the <see cref="IFormatProvider"/> instance.
    /// </summary>
    /// <param name="formatProvider"></param>
    /// <returns></returns>
    public string ToString(IFormatProvider formatProvider)
    {
        return this.ToString(null, formatProvider);
    }


    /// <summary>
    /// Returns string representation of this instance in desired format, or using provided <see cref="IFormatProvider"/> instance.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="formatProvider"></param>
    /// <returns></returns>
    public string ToString(string format, IFormatProvider formatProvider)
    {
        format ??= "G";
        var smallhour = this.Hour > 12 ? this.Hour - 12 : this.Hour;
        var designator = this.Hour >= 12
                             ? PersianDateTimeFormatInfo.PMDesignator
                             : PersianDateTimeFormatInfo.AMDesignator;

        if (formatProvider?.GetFormat(typeof(ICustomFormatter)) is ICustomFormatter formatter)
            return formatter.Format(format, this, formatProvider);

        switch (format)
        {
            case "D":
            case "dddd, MMMM dd, yyyy":
            case "yyyy mm dd dddd":
                // 'yyyy mm dd dddd' e.g. 'دوشنبه 20 شهریور 1384'
                return
                    $"{this.LocalizedWeekDayName} {Util.toDouble(this.Day)} {this.LocalizedMonthName} {this.Year}";


            case "f":
                // 'hh:mm yyyy mmmm dd dddd' e.g. 'دوشنبه 20 شهریور 1384 21:30'
                return
                    $"{this.LocalizedWeekDayName} {Util.toDouble(this.Day)} {this.LocalizedMonthName} {this.Year} {Util.toDouble(this.Hour)}:{Util.toDouble(this.Minute)}";

            case "F":
            case "tt hh:mm:ss yyyy mmmm dd dddd":
            case "dddd, MMMM dd, yyyy hh:mm:ss tt":
                // 'tt hh:mm:ss yyyy mmmm dd dddd' e.g. 'دوشنبه 20 شهریور 1384 02:30:22 ب.ض'
                return
                    $"{this.LocalizedWeekDayName} {Util.toDouble(this.Day)} {this.LocalizedMonthName} {this.Year} {Util.toDouble(smallhour)}:{Util.toDouble(this.Minute)}:{Util.toDouble(this.Second)} {designator}";

            case "g":
                // 'yyyy/mm/dd hh:mm tt'
                return
                    $"{this.Year}/{Util.toDouble(this.Month)}/{Util.toDouble(this.Day)} {Util.toDouble(smallhour)}:{Util.toDouble(this.Minute)} {designator}";

            case "G":
                // 'yyyy/mm/dd hh:mm:ss tt'
                return
                    $"{this.Year}/{Util.toDouble(this.Month)}/{Util.toDouble(this.Day)} {Util.toDouble(smallhour)}:{Util.toDouble(this.Minute)}:{Util.toDouble(this.Second)} {designator}";

            case "MMMM dd":
            case "dd MMMM":
                // MMMM dd e.g. 'تیر 10'
                return $"{this.LocalizedMonthName} {Util.toDouble(this.Day)}";

            case "MMMM, yyyy":
            case "M":
            case "m":
                // 'yyyy mmmm'
                return $"{this.Year} {this.LocalizedMonthName}";

            case "s":
                // 'yyyy-mm-ddThh:mm:ss'
                return
                    $"{this.Year}-{Util.toDouble(this.Month)}-{Util.toDouble(this.Day)}T{Util.toDouble(this.Hour)}:{Util.toDouble(this.Minute)}:{Util.toDouble(this.Second)}";

            case "hh:mm tt":
            case "t":
                // 'hh:mm tt' e.g. 12:22 ب.ض
                return $"{Util.toDouble(smallhour)}:{Util.toDouble(this.Minute)} {designator}";

            case "T":
            case "hh:mm:ss tt":
                // 'hh:mm:ss tt' e.g. 12:22:30 ب.ض
                return
                    $"{Util.toDouble(smallhour)}:{Util.toDouble(this.Minute)}:{Util.toDouble(this.Second)} {designator}";

            case "w":
            case "W":
                return this.ToWritten();
            default:
                // ShortDatePattern yyyy/mm/dd e.g. '1384/09/01'
                return $"{this.Year}/{Util.toDouble(this.Month)}/{Util.toDouble(this.Day)}";
        }
    }
}