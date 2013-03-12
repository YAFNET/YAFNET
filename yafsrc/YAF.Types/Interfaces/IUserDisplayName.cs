/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
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
  using System.Collections.Generic;

  /// <summary>
  /// User Display Name interface.
  /// </summary>
  public interface IUserDisplayName
  {
    /// <summary>
    /// Get the Display Name from a <paramref name="userId"/>
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    string GetName(int userId);

    /// <summary>
    /// Get the <paramref name="userId"/> from the user name.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// </returns>
    int? GetId(string name);

    /// <summary>
    /// Find a displayName value.
    /// </summary>
    /// <param name="contains">
    /// The contains.
    /// </param>
    /// <returns>
    /// </returns>
    IDictionary<int, string> Find(string contains);

    /// <summary>
    /// Clears a user value (if there is one) for <paramref name="userId"/> from the cache
    /// </summary>
    /// <param name="userId"></param>
    void Clear(int userId);

    /// <summary>
    /// Clears the display name cache (if there is one)
    /// </summary>
    void Clear();
  }
}