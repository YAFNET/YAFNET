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
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  using YAF.Types;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  /// <summary>
  /// The js script class.
  /// </summary>
  public class JavaScriptFunction : IScriptFunction
  {
    #region Constants and Fields

    /// <summary>
    /// The _inner builder.
    /// </summary>
    private readonly IScriptStatement _inner;

    /// <summary>
    /// The _function parameters.
    /// </summary>
    private IList<object> _params = new List<object>();

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="JavaScriptFunction"/> class.
    /// </summary>
    /// <param name="inner">
    /// The inner builder.
    /// </param>
    public JavaScriptFunction()
    {
      this._inner = new JavaScriptStatement();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets Params.
    /// </summary>
    public IList<object> Params
    {
      get
      {
        return this._params;
      }

      set
      {
        this._params = value;
      }
    }

    #endregion

    #region Implementation of IScriptStatement

    /// <summary>
    /// Add to the script
    /// </summary>
    /// <param name="js">
    /// The js.
    /// </param>
    /// <returns>
    /// </returns>
    public void Add(string js)
    {
      this._inner.Add(js);
    }

    /// <summary>
    /// Get the script's result as String
    /// </summary>
    /// <returns>
    /// The Completed Script
    /// </returns>
    public string Build(IScriptBuilder scriptBuilder)
    {
      var builder = new StringBuilder();

      builder.Append("function");

      if (this.Name.IsSet())
      {
        builder.AppendFormat(" {0}", this.Name);
      }

      builder.Append("(");

      if (this.Params.Any())
      {
        this.Params.ForEachFirst(
          (param, isFirst) =>
          {
            if (!isFirst)
            {
              builder.Append(", ");
            }

            builder.Append(param);
          });
      }

      builder.Append(")");
      builder.AppendFormat("{{ {0} }}", this._inner.Build(scriptBuilder));

      return builder.ToString();
    }

    /// <summary>
    /// Clears the entire statment.
    /// </summary>
    public void Clear()
    {
      this._inner.Clear();
    }

    #endregion
  }
}