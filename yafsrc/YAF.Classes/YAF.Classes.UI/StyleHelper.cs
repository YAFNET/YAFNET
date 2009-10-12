using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace YAF.Classes.UI
{
    public static class StyleHelper
    {
        public static DataTable ClearStyle(DataTable dt)
        {
         foreach (DataRow dr in dt.Rows)
           {
               string[] skins = dr["Style"].ToString().Trim().Split('/');
               for (int i = 0; i < skins.GetLength(0); i++)
               {
                string[] pair  = skins[i].Split('!');
                if (pair[0].ToLowerInvariant().Trim() == "default")
                    dr["Style"] = pair[1];
               
                for (int j = 0; j < pair.Length; j++)
                {

                    if ((pair[0] + ".xml").ToLower().Trim() == YAF.Classes.Core.YafContext.Current.BoardSettings.Theme.ToLower().Trim())
                    dr["Style"] = pair[1];
                }
               }
           } 
            return dt;
    }
}
}
