/* Yet Another Forum.NET
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
using YAF.Classes.Pattern;

namespace YAF.Classes.Core
{
  /// <summary>
  /// Place to put helper properties for context variables inside.
  /// </summary>
  public class ContextVariableRepository
  {
    /// <summary>
    /// The _dic.
    /// </summary>
    private TypeDictionary _dic = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextVariableRepository"/> class.
    /// </summary>
    /// <param name="dictionary">
    /// The dictionary.
    /// </param>
    public ContextVariableRepository(TypeDictionary dictionary)
    {
      this._dic = dictionary;
    }

    /// <summary>
    /// Gets Vars.
    /// </summary>
    protected TypeDictionary Vars
    {
      get
      {
        return this._dic;
      }
    }

    /// <summary>
    /// Flag set if the system should check if the user is suspended and redirect appropriately. Defaults to true.
    /// Setting to false effectively disables suspend checking.
    /// </summary>
    public bool IsSuspendCheckEnabled
    {
      get
      {
        return Vars.AsBoolean("IsSuspendCheckEnabled") ?? true;
      }

      set
      {
        this.Vars["IsSuspendCheckEnabled"] = value;
      }
    }
  }
}