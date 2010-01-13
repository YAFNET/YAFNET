/* YetAnotherForum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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

namespace YAF.Classes.Core
{
  /// <summary>
  /// The root service.
  /// </summary>
  public abstract class BaseYafService : IYafService
  {
    /// <summary>
    /// Gets InitVarName.
    /// </summary>
    protected abstract string InitVarName
    {
      get;
    }

    /// <summary>
    /// Gets a value indicating whether Initialized.
    /// </summary>
    public bool Initialized
    {
      get
      {
        if (YafContext.Current[InitVarName] == null)
        {
          return false;
        }

        return Convert.ToBoolean(YafContext.Current[InitVarName]);
      }

      private set
      {
        YafContext.Current[InitVarName] = value;
      }
    }

    /// <summary>
    /// The run.
    /// </summary>
    public void Run()
    {
      if (!Initialized)
      {
        Initialized = RunService();
      }
    }

    /// <summary>
    /// The run service.
    /// </summary>
    /// <returns>
    /// The run service.
    /// </returns>
    protected abstract bool RunService();
  }
}