﻿/* Yet Another Forum.NET
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

  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The script statement context.
  /// </summary>
  public class ScriptStatementContext : IScriptStatementContext
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptStatementContext"/> class.
    /// </summary>
    /// <param name="scriptBuilder">
    /// The script builder.
    /// </param>
    /// <param name="scriptStatement">
    /// The script statement.
    /// </param>
    public ScriptStatementContext([NotNull] IScriptBuilder scriptBuilder, [NotNull] IScriptStatement scriptStatement)
    {
      CodeContracts.ArgumentNotNull(scriptBuilder, "scriptBuilder");
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");

      this.ScriptBuilder = scriptBuilder;
      this.ScriptStatement = scriptStatement;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets ScriptBuilder.
    /// </summary>
    public IScriptBuilder ScriptBuilder { get; set; }

    /// <summary>
    /// Gets or sets ScriptStatement.
    /// </summary>
    public IScriptStatement ScriptStatement { get; set; }

    #endregion
  }
}