using System;
using System.Collections.Generic;
using System.Text;

namespace YAF.Providers.Membership
{
    public static class CleanUtils
    {
        public static DateTime ToDate(object obj)
        {
            if (obj != DBNull.Value)
                return Convert.ToDateTime(obj.ToString());
            else
                return DateTime.Now;
        }

        public static string ToString(object obj)
        {
            if (obj != DBNull.Value)
                return obj.ToString();
            else
                return String.Empty;
        }

        public static bool ToBool(object obj)
        {
            if (obj != DBNull.Value)
               return Convert.ToBoolean(obj);
            else
                return false;
        }

        public static int ToInt(object obj)
        {
            if (obj != DBNull.Value)
                return Convert.ToInt32(obj);
            else
                return 0;
        }
    }
}
