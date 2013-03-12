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
  using System.Collections.Generic;
  using System.Linq;

  using YAF.Types;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// the script statement extensions.
  /// </summary>
  public static class ScriptStatementContextExtensions
  {
    #region Public Methods

    /// <summary>
    /// Adds a script string without a closeing;
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <param name="js">
    /// The js.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext Add(
      [NotNull] this IScriptStatementContext scriptStatement, [NotNull] string js)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(js, "js");

      scriptStatement.ScriptStatement.Add(js);
      return scriptStatement;
    }

    /// <summary>
    /// The add a script statement without a closing;
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <param name="addStatement">
    /// The add builder.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext Add(
      [NotNull] this IScriptStatementContext scriptStatement, [NotNull] IScriptStatement addStatement)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(addStatement, "addStatement");

      return scriptStatement.Add(addStatement.Build(scriptStatement.ScriptBuilder));
    }

    /// <summary>
    /// Adds an array to the output: var arrayName = Array(parameters);
    /// </summary>
    /// <param name="scriptStatement">
    /// The script statement.
    /// </param>
    /// <param name="arrayName">
    /// The array name.
    /// </param>
    /// <param name="parameters">
    /// The parameters.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddArray(
      [NotNull] this IScriptStatementContext scriptStatement, 
      [NotNull] string arrayName, 
      [NotNull] params object[] parameters)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(arrayName, "arrayName");

      scriptStatement.AddLine();
      scriptStatement.AddFormat("var {0} = Array(", arrayName);

      AddParameters(scriptStatement, false, parameters);

      scriptStatement.Add(");");

      scriptStatement.AddLine();

      return scriptStatement;
    }

    /// <summary>
    /// Adds a call. E.g. functionName(parameters) to the statement.
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <param name="functionName">
    /// The function Name.
    /// </param>
    /// <param name="parameters">
    /// The parameters.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddCall(
      [NotNull] this IScriptStatementContext scriptStatement, 
      [NotNull] string functionName, 
      [NotNull] params object[] parameters)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(functionName, "functionName");
      CodeContracts.ArgumentNotNull(parameters, "parameters");

      scriptStatement.AddFormat("{0}(", functionName);

      AddParameters(scriptStatement, true, parameters);

      scriptStatement.Add(")");

      return scriptStatement;
    }

    /// <summary>
    /// Add a document.ready event to the statement.
    /// </summary>
    /// <param name="scriptStatement">
    /// The script Builder.
    /// </param>
    /// <param name="buildInner">
    /// The build Inner.
    /// </param>
    /// <returns>
    /// The jquery document ready script.
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddDocumentReady(
      [NotNull] this IScriptStatementContext scriptStatement, [NotNull] Action<IScriptFunctionContext> buildInner)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(buildInner, "buildInner");

      var function = scriptStatement.ScriptBuilder.CreateFunction(false);

      buildInner(function);

      scriptStatement.AddFormat("$$$().ready({0})", function.Build());

      return scriptStatement;
    }

    /// <summary>
    /// Adds a formatted string to the statement.
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <param name="js">
    /// The js.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddFormat(
      [NotNull] this IScriptStatementContext scriptStatement, [NotNull] string js, [NotNull] params object[] args)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(js, "js");
      CodeContracts.ArgumentNotNull(args, "args");

      scriptStatement.Add(js.FormatWith(args));

      return scriptStatement;
    }

    /// <summary>
    /// Adds a function (using the IScriptFunctionContext) to the statement.
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <param name="buildFunction">
    /// The build function.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddFunc(
      [NotNull] this IScriptStatementContext scriptStatement, [NotNull] Action<IScriptFunctionContext> buildFunction)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(buildFunction, "buildFunction");

      var function = scriptStatement.ScriptBuilder.CreateFunction(false);

      buildFunction(function);

      return scriptStatement.Add(function.Build());
    }

    /// <summary>
    /// Adds a simple annonymous function to the statement.
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <param name="jsInner">
    /// The js inner.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddFunc(
      [NotNull] this IScriptStatementContext scriptStatement, [NotNull] string jsInner)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(jsInner, "jsInner");

      return scriptStatement.AddFormat("function() {{ {0} }}", jsInner);
    }

    /// <summary>
    /// The add a script statement if.
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <param name="condition">
    /// The condition.
    /// </param>
    /// <param name="doInner">
    /// The do Inner.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddIf(
      [NotNull] this IScriptStatementContext scriptStatement, 
      [NotNull] string condition, 
      [NotNull] Action<IScriptStatementContext> doInner)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(condition, "condition");
      CodeContracts.ArgumentNotNull(doInner, "doInner");

      var newStatementContext = scriptStatement.ScriptBuilder.CreateStatement(false);

      doInner(newStatementContext);

      return scriptStatement.AddFormat(
        "if ({0}) {{\r\n{1}\r\n}}", condition, newStatementContext.ScriptStatement.Build(scriptStatement.ScriptBuilder));
    }

    /// <summary>
    /// Adds a line.
    /// </summary>
    /// <param name="scriptStatement">
    /// The script statement.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddLine([NotNull] this IScriptStatementContext scriptStatement)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");

      return scriptStatement.Add("\r\n");
    }

    /// <summary>
    /// Adds a jQuery selector to the builder statement. E.g.: jQuery('selector')
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddSelector(
      [NotNull] this IScriptStatementContext scriptStatement, [NotNull] string selector)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(selector, "selector");

      return scriptStatement.AddFormat(@"$$$({0})", selector);
    }

    /// <summary>
    /// Adds a jQuery selector to the builder statement. E.g.: jQuery('selector') with formatting
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddSelectorFormat(
      [NotNull] this IScriptStatementContext scriptStatement, [NotNull] string selector, [NotNull] params object[] args)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(selector, "selector");
      CodeContracts.ArgumentNotNull(args, "args");

      return scriptStatement.AddFormat(@"$$$({0})", selector.FormatWith(args));
    }

    /// <summary>
    /// Adds a quoted escaped string to the statement: e.g. "jsString"
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <param name="jsString">
    /// The js String.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddString(
      [NotNull] this IScriptStatementContext scriptStatement, [NotNull] string jsString)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");

      return scriptStatement.AddFormat(@"""{0}""", jsString.ToJsString());
    }

    /// <summary>
    /// The add a script statement without a closing ;
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <param name="js">
    /// The Javascript to Add
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddWithEnd(
      [NotNull] this IScriptStatementContext scriptStatement, [NotNull] string js)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(js, "js");

      return scriptStatement.AddFormat("{0};\r\n", js);
    }

    /// <summary>
    /// Builds the Statement from the context and returns the built string.
    /// </summary>
    /// <param name="scriptStatementContext">
    /// The script statement context.
    /// </param>
    /// <returns>
    /// The build.
    /// </returns>
    public static string Build([NotNull] this IScriptStatementContext scriptStatementContext)
    {
      CodeContracts.ArgumentNotNull(scriptStatementContext, "scriptStatementContext");

      return scriptStatementContext.ScriptStatement.Build(scriptStatementContext.ScriptBuilder);
    }

    /// <summary>
    /// Adds a dot.
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext Dot([NotNull] this IScriptStatementContext scriptStatement)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");

      return scriptStatement.Add(".");
    }

    /// <summary>
    /// The add an end to the statement. E.g. ;
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext End([NotNull] this IScriptStatementContext scriptStatement)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");

      return scriptStatement.Add(";\r\n");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Add parameters to the script statement.
    /// </summary>
    /// <param name="scriptStatement">
    /// The script statement.
    /// </param>
    /// <param name="allowStatement">
    /// The allow statement.
    /// </param>
    /// <param name="parameters">
    /// The parameters.
    /// </param>
    private static void AddParameters([NotNull] this IScriptStatementContext scriptStatement, bool allowStatement, [NotNull] IEnumerable<object> parameters)
    {
      if (parameters.Any())
      {
        parameters.ForEachFirst(
          (param, isFirst) =>
            {
              if (!isFirst)
              {
                scriptStatement.Add(",");
              }

              if (allowStatement && param.HasInterface<IScriptStatementContext>())
              {
                scriptStatement.Add(
                  param.ToClass<IScriptStatementContext>().ScriptStatement.Build(scriptStatement.ScriptBuilder));
              }
              else if (param is string)
              {
                scriptStatement.AddString(param.ToString());
              }
              else
              {
                scriptStatement.Add(param.ToString());
              }
            });
      }
    }

    #endregion
  }
}