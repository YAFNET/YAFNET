/* Cteated by vzrus(c) for Yet Another Forum and can be use with any Yet Another Forum licence and modified in any way.*/
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
        public static void DecodeStyleByTable(ref DataTable dt)
        {
            DecodeStyleByTable( ref dt, false );
        }
        public static void DecodeStyleByTable( ref DataTable dt, bool colorOnly )
		{
			foreach ( DataRow dr in dt.Rows )
			{
				string[] styleRow = dr["Style"].ToString().Trim().Split( '/' );
				for ( int i = 0; i < styleRow.GetLength( 0 ); i++ )
				{
					string[] pair = styleRow[i].Split( '!' );
                    if (pair[0].ToLowerInvariant().Trim() == "default")
                    {
                        if (colorOnly) dr["Style"] = GetColorOnly(pair[1]);
                        else dr["Style"] = pair[1];
                    }

					for ( int j = 0; j < pair.Length; j++ )
					{
                        if ((pair[0] + ".xml").ToLower().Trim() == YAF.Classes.Core.YafContext.Current.Theme.ThemeFile.ToLower().Trim())
                        {
                            if (colorOnly) dr["Style"] = GetColorOnly(pair[1]);
                            else  dr["Style"] = pair[1];
                        }
					}
				}
			}
		}
        public static void DecodeStyleByRow(ref DataRow dr)
        {
            DecodeStyleByRow(ref dr, false);
        }
        public static void DecodeStyleByRow(ref DataRow dr, bool colorOnly)
		{
			string[] styleRow = dr["Style"].ToString().Trim().Split( '/' );
			for ( int i = 0; i < styleRow.GetLength( 0 ); i++ )
			{
				string[] pair = styleRow[i].Split( '!' );
                if (pair[0].ToLowerInvariant().Trim() == "default")
                {
                    if (colorOnly) dr["Style"] = GetColorOnly(pair[1]);
                    else dr["Style"] = pair[1];
                }

				for ( int j = 0; j < pair.Length; j++ )
				{
                    if ((pair[0] + ".xml").ToLower().Trim() == YAF.Classes.Core.YafContext.Current.Theme.ThemeFile.ToLower().Trim())
                    {
                        if ( colorOnly ) dr["Style"] = GetColorOnly(pair[1]);
                        else dr["Style"] = pair[1];
                    }

				}
			}
		}
        public static string DecodeStyleByString(string styleStr, bool colorOnly)
        {
            string[] styleRow = styleStr.Trim().Split('/');
            for (int i = 0; i < styleRow.GetLength(0); i++)
            {
                string[] pair = styleRow[i].Split('!');
                if (pair[0].ToLowerInvariant().Trim() == "default")
                {
                    if (colorOnly) styleStr = GetColorOnly(pair[1]);
                    else styleStr = pair[1];
                }

                for (int j = 0; j < pair.Length; j++)
                {
                    if ((pair[0] + ".xml").ToLower().Trim() == YAF.Classes.Core.YafContext.Current.Theme.ThemeFile.ToLower().Trim())
                    {
                        if (colorOnly) styleStr = GetColorOnly(pair[1]);
                        else styleStr = pair[1];
                    }

                }
            }
            return styleStr;
        }
        public static string GetColorOnly(string styleString)
        {
            string[] styleArray = styleString.Split(';');
            for (int i = 0; i < styleArray.Length; i++)
            {
                if (styleArray[i].ToLower().Contains("color"))
                {
                    return styleArray[i];

                }
            }
            return null;
        }
	}
}
