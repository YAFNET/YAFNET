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
using System;
using System.Collections.Generic;

namespace YAF.Classes.Pattern
{
  /// <summary>
  /// The single class instance factory.
  /// </summary>
  public class SingleClassInstanceFactory
  {
    /// <summary>
    /// The _context classes.
    /// </summary>
    private readonly Dictionary<int, object> _contextClasses = new Dictionary<int, object>();

    /// <summary>
    /// The get instance.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public T GetInstance<T>() where T : class
    {
      int objNameHash = typeof (T).ToString().GetHashCode();

      if (!this._contextClasses.ContainsKey(objNameHash))
      {
        this._contextClasses[objNameHash] = (T) Activator.CreateInstance(typeof (T), true);
      }

      return (T) this._contextClasses[objNameHash];
    }

    /// <summary>
    /// The set instance.
    /// </summary>
    /// <param name="instance">
    /// The instance.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public void SetInstance<T>(T instance) where T : class
    {
      int objNameHash = typeof (T).ToString().GetHashCode();
      this._contextClasses[objNameHash] = instance;
    }
  }
}