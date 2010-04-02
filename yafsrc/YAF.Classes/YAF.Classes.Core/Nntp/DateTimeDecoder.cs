// vzrus 01.04.10
namespace YAF.Classes.Core.Nntp
{
    using System;
    using System.Data;

    public static class NNTPDateDecoder
    {
        public static DateTime DecodeUTC(string nntpDateTime, out int tzi)
       {
           nntpDateTime = nntpDateTime.Substring(nntpDateTime.IndexOf(',') + 1);
           if (nntpDateTime.IndexOf("(") > 0)
           {
               nntpDateTime = nntpDateTime.Substring(0, nntpDateTime.IndexOf('(') - 1).Trim();
           }
           int ipos = nntpDateTime.IndexOf('+');
           int ineg = nntpDateTime.IndexOf('-');
           string tz = string.Empty;
           if (ipos > 0)
           {
               tz = nntpDateTime.Substring(ipos + 1).Trim();
               nntpDateTime = nntpDateTime.Substring(0, ipos - 1).Trim();
           }
           if (ineg > 0)
           {
               tz = nntpDateTime.Substring(ineg + 1).Trim();              
               nntpDateTime = nntpDateTime.Substring(0, ineg - 1).Trim();
           }

           DateTime dtc;
           if (DateTime.TryParse(nntpDateTime, out dtc))
           {

               if (ipos > 0)
               {
                   TimeSpan ts = TimeSpan.FromHours(Convert.ToInt32(tz.Substring(0, 2))) + TimeSpan.FromMinutes(Convert.ToInt32(tz.Substring(2, 2)));
                   tzi = ts.Minutes;
                   return dtc + ts;
               }
               else if (ineg > 0)
               {
                   TimeSpan ts = TimeSpan.FromHours(Convert.ToInt32(tz.Substring(0, 2))) + TimeSpan.FromMinutes(Convert.ToInt32(tz.Substring(2, 2)));
                   tzi = ts.Minutes;
                   return dtc - ts;
               }
               else
               {
                   tzi = 0;
                   return dtc;
               }
               // eof vzrus
           }
           else
           {
               YAF.Classes.Data.DB.eventlog_create(YafContext.Current.PageUserID, "NNTP Feature", String.Format("Unhandled NNTP DateTime nntpDateTime '{0}'", nntpDateTime));
           }
           tzi = 0;
           return DateTime.UtcNow;
       }
    }
}