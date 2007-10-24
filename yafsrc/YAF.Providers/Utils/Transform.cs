using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace YAF.Providers.Utils
{
    public static class Transform
    {
        public static DateTime ToDate(object obj)
        {
            if ((obj != DBNull.Value) && (obj != null))
                return Convert.ToDateTime(obj.ToString());
            else
                return DateTime.Now; // Yeah I admit it, what the hell should this return?
        }

        public static string ToString(object obj)
        {
            if ((obj != DBNull.Value) && (obj != null))
                return obj.ToString();
            else
                return String.Empty;
        }

        public static string ToString(object obj, string defValue)
        {
            if ((obj != DBNull.Value) && (obj != null))
                return obj.ToString();
            else
                return defValue;
        }

        public static string[] ToStringArray(StringCollection coll)
        {
            String[] strReturn = new String[coll.Count];
            coll.CopyTo(strReturn, 0);
            return strReturn;
        }

        public static bool ToBool(object obj)
        {
            if ((obj != DBNull.Value) && (obj != null))
                return Convert.ToBoolean(obj);
            else
                return false;
        }

        public static int ToInt(object obj)
        {
            if ((obj != DBNull.Value) && (obj != null))
                return Convert.ToInt32(obj);
            else
                return 0;
        }
    }
}
