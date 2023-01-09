using System.Collections.Generic;

namespace FarsiLibrary.Utils;

internal class PersianWeekDayNames
{
    public string Shanbeh = "شنبه";
    public string Yekshanbeh = "یکشنبه";
    public string Doshanbeh = "دوشنبه";
    public string Seshanbeh = "ﺳﻪشنبه";
    public string Chaharshanbeh = "چهارشنبه";
    public string Panjshanbeh = "پنجشنبه";
    public string Jomeh = "جمعه";

    public string Sh = "ش";
    public string Ye = "ی";
    public string Do = "د";
    public string Se = "س";
    public string Ch = "چ";
    public string Pa = "پ";
    public string Jo = "ج";

    private static PersianWeekDayNames instance;

    private PersianWeekDayNames()
    {
        this.Days = new List<string>
                        {
                            this.Yekshanbeh,
                            this.Doshanbeh,
                            this.Seshanbeh,
                            this.Chaharshanbeh,
                            this.Panjshanbeh,
                            this.Jomeh,
                            this.Shanbeh,
                        };

        this.DaysAbbr = new List<string>
                            {
                                this.Ye,
                                this.Do,
                                this.Se,
                                this.Ch,
                                this.Pa,
                                this.Jo,
                                this.Sh,
                            };
    }

    public static PersianWeekDayNames Default => instance ??= new PersianWeekDayNames();

    internal List<string> Days { get; }

    internal List<string> DaysAbbr { get; }
}