/* Yet Another Forum.NET
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
  /// <summary>
  /// The Script builder Interface.
  /// </summary>
  public interface IScriptStatement
  {
    #region Public Methods

    /// <summary>
    /// Add to the script
    /// </summary>
    /// <param name="js">
    /// The js.
    /// </param>
    /// <returns>
    /// </returns>
    void Add([NotNull] string js);

    /// <summary>
    /// Get the script's result as String
    /// </summary>
    /// <returns>
    /// The Completed Script
    /// </returns>
    string Build(IScriptBuilder scriptBuilder);

    /// <summary>
    /// Clears the entire statment.
    /// </summary>
    void Clear();

    #endregion
  }
}