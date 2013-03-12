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

  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// the script builder extensions.
  /// </summary>
  public static class ScriptBuilderExtensions
  {
    #region Public Methods

    /// <summary>
    /// Creates a function statement. AddFunction is you want the function statement inserted into the Builder.
    /// </summary>
    /// <param name="scriptBuilder">
    /// The script builder.
    /// </param>
    /// <param name="addFunction">
    /// The add function.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptFunctionContext CreateFunction([NotNull] this IScriptBuilder scriptBuilder, bool addFunction)
    {
      CodeContracts.ArgumentNotNull(scriptBuilder, "scriptBuilder");

      var newFunction = new ScriptFunctionContext(scriptBuilder, new JavaScriptFunction());

      if (addFunction)
      {
        scriptBuilder.Statements.Add(newFunction.ScriptStatement);
      }

      return newFunction;
    }

    /// <summary>
    /// Creates a function statement and adds it to the builder.
    /// </summary>
    /// <param name="scriptBuilder">
    /// The script builder.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptFunctionContext CreateFunction([NotNull] this IScriptBuilder scriptBuilder)
    {
      CodeContracts.ArgumentNotNull(scriptBuilder, "scriptBuilder");

      return scriptBuilder.CreateFunction(true);
    }

    /// <summary>
    /// Creates a statement and optionally adds it to the builder.
    /// </summary>
    /// <param name="scriptBuilder">
    /// The script builder.
    /// </param>
    /// <param name="addStatement">
    /// The add statement.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext CreateStatement(
      [NotNull] this IScriptBuilder scriptBuilder, bool addStatement)
    {
      CodeContracts.ArgumentNotNull(scriptBuilder, "scriptBuilder");

      var newStatement = new ScriptStatementContext(scriptBuilder, new JavaScriptStatement());

      if (addStatement)
      {
        scriptBuilder.Statements.Add(newStatement.ScriptStatement);
      }

      return newStatement;
    }

    /// <summary>
    /// Creates a statement and adds it to the builder.
    /// </summary>
    /// <param name="scriptBuilder">
    /// The script builder.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext CreateStatement([NotNull] this IScriptBuilder scriptBuilder)
    {
      CodeContracts.ArgumentNotNull(scriptBuilder, "scriptBuilder");

      return scriptBuilder.CreateStatement(true);
    }

    #endregion
  }
}