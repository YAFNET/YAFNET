/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2011 Jaben Cargman
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
using System;
using System.Diagnostics;
using System.Web;

namespace YAF.Classes.Data
{
  using YAF.Utils;
  using YAF.Utils.Helpers.StringUtils;

  /// <summary>
  /// The query counter.
  /// </summary>
  public sealed class QueryCounter : IDisposable
  {
    /* Ederon : 6/16/2007 - conventions */
#if DEBUG

    /// <summary>
    /// The _stop watch.
    /// </summary>
    private Stopwatch _stopWatch = new Stopwatch();

    /// <summary>
    /// The _cmd.
    /// </summary>
    private string _cmd;
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryCounter"/> class.
    /// </summary>
    /// <param name="sql">
    /// The sql.
    /// </param>
    public QueryCounter(string sql)
    {
#if DEBUG
      this._cmd = sql;

      if (HttpContext.Current != null)
      {
        if (HttpContext.Current.Items["NumQueries"] == null)
        {
          HttpContext.Current.Items["NumQueries"] = (int)1;
        }
        else
        {
          HttpContext.Current.Items["NumQueries"] = 1 + (int)HttpContext.Current.Items["NumQueries"];
        }
      }

      this._stopWatch.Start();
#endif
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose()
    {
#if DEBUG
      this._stopWatch.Stop();

      double duration = (double) this._stopWatch.ElapsedMilliseconds/1000.0;

      this._cmd = "{0}: {1:N3}".FormatWith(this._cmd, duration);

      if (HttpContext.Current != null)
      {
        if (HttpContext.Current.Items["TimeQueries"] == null)
        {
          HttpContext.Current.Items["TimeQueries"] = duration;
        }
        else
        {
          HttpContext.Current.Items["TimeQueries"] = duration + (double)HttpContext.Current.Items["TimeQueries"];
        }

        if (HttpContext.Current.Items["CmdQueries"] == null)
        {
          HttpContext.Current.Items["CmdQueries"] = this._cmd;
        }
        else
        {
          HttpContext.Current.Items["CmdQueries"] += "<br />" + this._cmd;
        }
      }

#endif
    }

#if DEBUG

    /// <summary>
    /// The reset.
    /// </summary>
    public static void Reset()
    {
      if (HttpContext.Current != null)
      {
        HttpContext.Current.Items["NumQueries"] = 0;
        HttpContext.Current.Items["TimeQueries"] = (double) 0;
        HttpContext.Current.Items["CmdQueries"] = string.Empty;
      }
    }

    /// <summary>
    /// Gets Count.
    /// </summary>
    public static int Count
    {
      get
      {
        return (int) ((HttpContext.Current == null) ? 0 : HttpContext.Current.Items["NumQueries"]);
      }
    }

    /// <summary>
    /// Gets Duration.
    /// </summary>
    public static double Duration
    {
      get
      {
        return (double) ((HttpContext.Current == null) ? 0.0 : HttpContext.Current.Items["TimeQueries"]);
      }
    }

    /// <summary>
    /// Gets Commands.
    /// </summary>
    public static string Commands
    {
      get
      {
        return (string) ((HttpContext.Current == null) ? string.Empty : HttpContext.Current.Items["CmdQueries"]);
      }
    }
#endif
  }
}