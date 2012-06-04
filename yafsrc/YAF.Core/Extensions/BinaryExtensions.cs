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

using System;

namespace YAF.Core
{
	public static class BinaryExtensions
  {
    /// <summary>
    /// The binary and.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="checkAgainst">
    /// The check against.
    /// </param>
    /// <returns>
    /// The binary and.
    /// </returns>
    public static bool BinaryAnd(this int value, int checkAgainst)
    {
      return (value & checkAgainst) == checkAgainst;
    }

    /// <summary>
    /// The binary and.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="checkAgainst">
    /// The check against.
    /// </param>
    /// <returns>
    /// The binary and.
    /// </returns>
    public static bool BinaryAnd(this object value, object checkAgainst)
    {
        return BinaryAnd(Convert.ToInt32(value), Convert.ToInt32(checkAgainst));
    }    
  }
}