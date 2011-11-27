namespace FarsiLibrary.Resources
{
    /// <summary>
    /// Localizer class used to get string values of Arabic language.
    /// </summary>
    public class ARLocalizer : BaseLocalizer
    {
        /// <summary>
        /// Gets a localized string for Arabic culture, for specified <see cref="StringID"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override string GetLocalizedString(StringID id)
        {
            switch (id)
            {
                case StringID.Empty: return string.Empty;
                case StringID.Numbers_0: return "۰";
                case StringID.Numbers_1: return "۱";
                case StringID.Numbers_2: return "۲";
                case StringID.Numbers_3: return "۳";
                case StringID.Numbers_4: return "۴";
                case StringID.Numbers_5: return "۵";
                case StringID.Numbers_6: return "۶";
                case StringID.Numbers_7: return "۷";
                case StringID.Numbers_8: return "۸";
                case StringID.Numbers_9: return "۹";
                case StringID.FADateTextBox_Required: return "فيلد اجباری میباشد";
                case StringID.FAMonthView_None: return "امح";
                case StringID.FAMonthView_Today: return "اليوم";
                case StringID.PersianDate_InvalidDateFormat: return "ساختار تاریخ مجاز نمیباشد.";
                case StringID.PersianDate_InvalidDateTime: return "مقدار زمان/ساعت صحیح نمیباشد.";
                case StringID.PersianDate_InvalidDateTimeLength: return "متن وارد شده برای زمان/ساعت صحیح نمیباشد.";
                case StringID.PersianDate_InvalidDay: return "مقدار روز صحیح نمیباشد.";
                case StringID.PersianDate_InvalidEra: return "محدوده وارد شده صحیح نمیباشد.";
                case StringID.PersianDate_InvalidFourDigitYear: return "مقدار وارد شده را نمیتوان به سال تبدیل کرد.";
                case StringID.PersianDate_InvalidHour: return "مقدار ساعت صحیح نمیباشد.";
                case StringID.PersianDate_InvalidLeapYear: return "این سال ، سال کبیسه نیست. مقدار روز صحیح نمیباشد.";
                case StringID.PersianDate_InvalidMinute: return "مقدار دقیقه صحیح نمیباشد.";
                case StringID.PersianDate_InvalidMonth: return "مقدار ماه صحیح نمیباشد.";
                case StringID.PersianDate_InvalidMonthDay: return "مقدار ماه/روز صحیح نمیباشد.";
                case StringID.PersianDate_InvalidSecond: return "مقدار ثانیه صحیح نمیباشد.";
                case StringID.PersianDate_InvalidTimeFormat: return "ساختار زمان صحیح نمیباشد.";
                case StringID.PersianDate_InvalidYear: return "مقدار سال صحیح نمیباشد.";
                case StringID.Validation_Cancel: return "مقدار انتخاب شده مجاز نمیباشد.";
                case StringID.Validation_NotValid: return "داخل النص ليس صحيحا.";
                case StringID.Validation_Required: return "انتخاب اجباری. لطفا مقداری برای این فیلد وارد کنید.";
            }

            return "";
        }
    }
}
