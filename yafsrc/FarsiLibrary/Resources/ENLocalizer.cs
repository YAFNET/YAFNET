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

            return "";
        }
    }
}
