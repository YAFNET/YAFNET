/* Cteated by vzrus(c) for Yet Another Forum and can be use with any Yet Another Forum licence and modified in any way.*/
using System.Data;
using YAF.Classes.Core;

namespace YAF.Classes.UI
{
  /// <summary>
  /// The style helper.
  /// </summary>
  public static class StyleHelper
  {
    /// <summary>
    /// The decode style by table.
    /// </summary>
    /// <param name="dt">
    /// The dt.
    /// </param>
    public static void DecodeStyleByTable(ref DataTable dt)
    {
      DecodeStyleByTable(ref dt, false);
    }
    
    /// <summary>
    /// The decode style by table.
    /// </summary>
    /// <param name="dt">
    /// The dt.
    /// </param>
    /// <param name="colorOnly">
    /// The color only.
    /// </param>  
    public static void DecodeStyleByTable(ref DataTable dt, bool colorOnly)
    {
        DecodeStyleByTable(ref  dt, colorOnly, "Style");
    }

    /// <summary>
    /// The decode style by table.
    /// </summary>
    /// <param name="dt">
    /// The dt.
    /// </param>
    /// <param name="colorOnly">
    /// The color only.
    /// </param>
     /// <param name="colorOnly">
    /// The styleColumns can contain param array to handle several style columns.
    /// </param> 
    public static void DecodeStyleByTable(ref DataTable dt, bool colorOnly, params string [] styleColumns )
    {
        foreach (DataRow dr in dt.Rows)
        {
            for (int k = 0; k < styleColumns.Length; k++)
            {
                string[] styleRow = dr[styleColumns[k]].ToString().Trim().Split('/');
                for (int i = 0; i < styleRow.GetLength(0); i++)
                {
                    string[] pair = styleRow[i].Split('!');
                    if (pair[0].ToLowerInvariant().Trim() == "default")
                    {
                        if (colorOnly)
                        {
                            dr[styleColumns[k]] = GetColorOnly(pair[1]);
                        }
                        else
                        {
                            dr[styleColumns[k]] = pair[1];
                        }
                    }

                    for (int j = 0; j < pair.Length; j++)
                    {
                        if ((pair[0] + ".xml").ToLower().Trim() == YafContext.Current.Theme.ThemeFile.ToLower().Trim())
                        {
                            if (colorOnly)
                            {
                                dr[styleColumns[k]] = GetColorOnly(pair[1]);
                            }
                            else
                            {
                                dr[styleColumns[k]] = pair[1];
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// The decode style by row.
    /// </summary>
    /// <param name="dr">
    /// The dr.
    /// </param>
    public static void DecodeStyleByRow(ref DataRow dr)
    {
      DecodeStyleByRow(ref dr, false);
    }

    /// <summary>
    /// The decode style by row.
    /// </summary>
    /// <param name="dr">
    /// The dr.
    /// </param>
    /// <param name="colorOnly">
    /// The color only.
    /// </param>
    public static void DecodeStyleByRow(ref DataRow dr, bool colorOnly)
    {
      string[] styleRow = dr["Style"].ToString().Trim().Split('/');
      for (int i = 0; i < styleRow.GetLength(0); i++)
      {
        string[] pair = styleRow[i].Split('!');
        if (pair[0].ToLowerInvariant().Trim() == "default")
        {
          if (colorOnly)
          {
            dr["Style"] = GetColorOnly(pair[1]);
          }
          else
          {
            dr["Style"] = pair[1];
          }
        }

        for (int j = 0; j < pair.Length; j++)
        {
          if ((pair[0] + ".xml").ToLower().Trim() == YafContext.Current.Theme.ThemeFile.ToLower().Trim())
          {
            if (colorOnly)
            {
              dr["Style"] = GetColorOnly(pair[1]);
            }
            else
            {
              dr["Style"] = pair[1];
            }
          }
        }
      }
    }

    /// <summary>
    /// The decode style by string.
    /// </summary>
    /// <param name="styleStr">
    /// The style str.
    /// </param>
    /// <param name="colorOnly">
    /// The color only.
    /// </param>
    /// <returns>
    /// The decode style by string.
    /// </returns>
    public static string DecodeStyleByString(string styleStr, bool colorOnly)
    {
      string[] styleRow = styleStr.Trim().Split('/');
      for (int i = 0; i < styleRow.GetLength(0); i++)
      {
        string[] pair = styleRow[i].Split('!');
        if (pair[0].ToLowerInvariant().Trim() == "default")
        {
          if (colorOnly)
          {
            styleStr = GetColorOnly(pair[1]);
          }
          else
          {
            styleStr = pair[1];
          }
        }

        for (int j = 0; j < pair.Length; j++)
        {
          if ((pair[0] + ".xml").ToLower().Trim() == YafContext.Current.Theme.ThemeFile.ToLower().Trim())
          {
            if (colorOnly)
            {
              styleStr = GetColorOnly(pair[1]);
            }
            else
            {
              styleStr = pair[1];
            }
          }
        }
      }

      return styleStr;
    }

    /// <summary>
    /// The get color only.
    /// </summary>
    /// <param name="styleString">
    /// The style string.
    /// </param>
    /// <returns>
    /// The get color only.
    /// </returns>
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