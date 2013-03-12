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

namespace YAF.Core
{
  #region Using

  using System;
  using System.Text;

  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// Java Script Statement Builder
  /// </summary>
  public class JavaScriptStatement : IScriptStatement
  {
    #region Constants and Fields

    /// <summary>
    ///   The String builder used to build the script.
    /// </summary>
    protected StringBuilder builder = new StringBuilder();

    #endregion

    #region Implemented Interfaces

    #region IScriptStatement

    /// <summary>
    /// Append to the script
    /// </summary>
    /// <param name="js">
    /// The js.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public void Add(string js)
    {
      CodeContracts.ArgumentNotNull(js, "js");

      this.builder.Append(js);
    }

    /// <summary>
    /// Get the script's result as String
    /// </summary>
    /// <param name="scriptScriptBuilder">
    /// The script Builder.
    /// </param>
    /// <returns>
    /// The Complete Script
    /// </returns>
    [NotNull]
    public string Build([NotNull] IScriptBuilder scriptScriptBuilder)
    {
      return this.builder.ToString().Replace("$$$", scriptScriptBuilder.ScriptSelector);
    }

    /// <summary>
    /// Clears the entire script.
    /// </summary>
    public void Clear()
    {
      this.builder = new StringBuilder();
    }

    #endregion

    #endregion
  }
}