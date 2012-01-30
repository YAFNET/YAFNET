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
namespace YAF.Core
{
  #region Using

  using System.Collections.Generic;
  using System.Text;

  using YAF.Classes;
  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The java script builder.
  /// </summary>
  public class JavaScriptBuilder : IScriptBuilder
  {
    #region Constants and Fields

    /// <summary>
    /// The _statements.
    /// </summary>
    public List<IScriptStatement> _statements = new List<IScriptStatement>();

    #endregion

    #region Properties

    /// <summary>
    ///   Gets the ScriptSelector.
    /// </summary>
    public string ScriptSelector
    {
      get
      {
        return Config.JQueryAlias;
      }
    }

    /// <summary>
    /// Gets Statements.
    /// </summary>
    public IList<IScriptStatement> Statements
    {
      get
      {
        return this._statements;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IScriptBuilder

    /// <summary>
    /// The build.
    /// </summary>
    /// <returns>
    /// The build.
    /// </returns>
    [NotNull]
    public string Build()
    {
      var built = new StringBuilder();

      foreach (var scriptStatement in this.Statements)
      {
        built.Append(scriptStatement.Build(this));
      }

      return built.ToString();
    }

    #endregion

    #endregion
  }
}