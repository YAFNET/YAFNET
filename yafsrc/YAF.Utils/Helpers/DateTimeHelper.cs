using System;

namespace YAF.Utils.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime SqlDbMinTime()
        {
            return DateTime.MinValue.AddYears(1902);
        }
    }

}
