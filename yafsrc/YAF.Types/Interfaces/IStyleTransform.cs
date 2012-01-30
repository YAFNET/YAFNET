/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Types.Interfaces
{
  using System.Data;

  /// <summary>
  /// The i style transform.
  /// </summary>
  public interface IStyleTransform
  {
    /// <summary>
    /// The decode style by table.
    /// </summary>
    /// <param name="dt">
    /// The dt.
    /// </param>
    void DecodeStyleByTable(ref DataTable dt);

    /// <summary>
    /// The decode style by table.
    /// </summary>
    /// <param name="dt">
    /// The dt.
    /// </param>
    /// <param name="colorOnly">
    /// The color only.
    /// </param>
    void DecodeStyleByTable(ref DataTable dt, bool colorOnly);

    /// <summary>
    /// The decode style by table.
    /// </summary>
    /// <param name="dt">
    /// The dt.
    /// </param>
    /// <param name="colorOnly">
    /// The color only.
    /// </param>
    /// <param name="styleColumns">
    /// The style Columns.
    /// </param>
    /// <param name="colorOnly">
    /// The styleColumns can contain param array to handle several style columns.
    /// </param>
    void DecodeStyleByTable(ref DataTable dt, bool colorOnly, params string[] styleColumns);

    /// <summary>
    /// The decode style by row.
    /// </summary>
    /// <param name="dr">
    /// The dr.
    /// </param>
    void DecodeStyleByRow(ref DataRow dr);

    /// <summary>
    /// The decode style by row.
    /// </summary>
    /// <param name="dr">
    /// The dr.
    /// </param>
    /// <param name="colorOnly">
    /// The color only.
    /// </param>
    void DecodeStyleByRow(ref DataRow dr, bool colorOnly);

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
    string DecodeStyleByString(string styleStr, bool colorOnly);
  }
}