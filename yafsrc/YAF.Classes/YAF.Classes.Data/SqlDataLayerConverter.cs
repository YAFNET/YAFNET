
/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Classes.Data
{
  using System;

  /// <summary>
  /// The sql data layer converter.
  /// </summary>
  public static class SqlDataLayerConverter
  {
    /// <summary>
    /// The verify int 32.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The verify int 32.
    /// </returns>
    public static int VerifyInt32(object o)
    {
      return Convert.ToInt32(o);
    }

    /// <summary>
    /// The verify bool.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The verify bool.
    /// </returns>
    public static bool VerifyBool(object o)
    {
      return Convert.ToBoolean(o);
    }
  }
}