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
  using System.Linq;
  using System.Text;

  using YAF.Types;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The script function extensions.
  /// </summary>
  public static class ScriptFunctionContextExtensions
  {
    #region Public Methods

    /// <summary>
    /// Defines the function inner statement.
    /// </summary>
    /// <param name="scriptFunction">
    /// The script function.
    /// </param>
    /// <param name="innerFuncStatment">
    /// The inner Func Statment.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptFunctionContext Func(
      [NotNull] this IScriptFunctionContext scriptFunction, [NotNull] Action<IScriptStatementContext> innerFuncStatment)
    {
      innerFuncStatment(scriptFunction);
      return scriptFunction;
    }

    /// <summary>
    /// The function Name (optional).
    /// </summary>
    /// <param name="scriptFunction">
    /// The script function.
    /// </param>
    /// <param name="Name">
    /// The name.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptFunctionContext Name(
      [NotNull] this IScriptFunctionContext scriptFunction, [NotNull] string Name)
    {
      scriptFunction.ScriptFunction.Name = Name;
      return scriptFunction;
    }

    /// <summary>
    /// The function parameters.
    /// </summary>
    /// <param name="scriptFunction">
    /// The script function.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptFunctionContext WithParams(
      [NotNull] this IScriptFunctionContext scriptFunction, [NotNull] params string[] args)
    {
      args.ForEach(a => scriptFunction.ScriptFunction.Params.Add(a));
      return scriptFunction;
    }

    #endregion
  }
}