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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Linq;

  using YAF.Types;
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
    /// The add a dot.
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
    /// The add array.
    /// </summary>
    /// <param name="scriptStatement">
    /// The script statement.
    /// </param>
    /// <param name="arrayName">
    /// The array name.
    /// </param>
    /// <param name="stringList">
    /// The string list.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddArray(
      [NotNull] this IScriptStatementContext scriptStatement, 
      [NotNull] string arrayName, 
      [NotNull] IEnumerable<string> stringList)
    {
      CodeContracts.ArgumentNotNull(scriptStatement, "scriptStatement");
      CodeContracts.ArgumentNotNull(arrayName, "arrayName");
      CodeContracts.ArgumentNotNull(stringList, "stringList");

      scriptStatement.AddLine();
      scriptStatement.AddFormat("var {0} = Array(", arrayName);
      stringList.ForEachFirst(
        (str, isFirst) =>
          {
            if (!isFirst)
            {
              scriptStatement.Add(", ");
            }

            scriptStatement.AddFormat(@"""{0}""", str.ToJsString());
          });

      scriptStatement.Add(");");

      scriptStatement.AddLine();

      return scriptStatement;
    }

    /// <summary>
    /// Adds a quoted escaped string to the statement: e.g. "jsString"
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

      if (parameters.Any())
      {
        parameters.ForEachFirst(
          (param, isFirst) =>
            {
              if (!isFirst)
              {
                scriptStatement.Add(", ");
              }

              if (param.HasInterface<IScriptStatementContext>())
              {
                scriptStatement.Add(param.ToClass<IScriptStatementContext>().ScriptStatement.Build(scriptStatement.ScriptBuilder));
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

      scriptStatement.Add(")");

      return scriptStatement;
    }

    /// <summary>
    /// Add the script to document.ready event
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
      var function = scriptStatement.ScriptBuilder.CreateFunction(false);

      buildInner(function);

      scriptStatement.AddFormat("$$$().ready({0})", function.Build());

      return scriptStatement;
    }

    /// <summary>
    /// The add format.
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
    /// The add func.
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
      var function = scriptStatement.ScriptBuilder.CreateFunction(false);

      buildFunction(function);

      return scriptStatement.Add(function.Build());
    }

    /// <summary>
    /// The add func.
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
    /// The add line.
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
    /// The add no conflict.
    /// </summary>
    /// <param name="scriptStatement">
    /// the script statement.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IScriptStatementContext AddNoConflict([NotNull] this IScriptStatementContext scriptStatement)
    {
      return scriptStatement.Add("$$$.noConflict();");
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
    /// The build.
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
    /// The add a dot.
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
    /// The add an endstatement
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

      return scriptStatement.Add(";");
    }

    #endregion
  }
}