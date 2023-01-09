namespace FarsiLibrary.Utils;

using System.Collections.Generic;

internal class PersianMonthNames
{
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

    private static PersianMonthNames instance;

    private PersianMonthNames()
    {
        this.Months = new List<string>
                          {
                              this.Farvardin,
                              this.Ordibehesht,
                              this.Khordad,
                              this.Tir,
                              this.Mordad,
                              this.Shahrivar,
                              this.Mehr,
                              this.Aban,
                              this.Azar,
                              this.Day,
                              this.Bahman,
                              this.Esfand,
                              string.Empty
                          };
    }

    public static PersianMonthNames Default => instance ??= new PersianMonthNames();

    internal List<string> Months { get; }

    public string this[int month] => this.Months[month];
}