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
namespace YAF.Core.Data.Profiling
{
    using System;
    using System.Diagnostics;
    using System.Web;

    using YAF.Types.Extensions;

    public class QueryWatcher : IDisposable
    {
#if DEBUG

        /// <summary>
        ///     The _stop watch.
        /// </summary>
        private readonly Stopwatch _stopWatch = new Stopwatch();

        /// <summary>
        ///     The _cmd.
        /// </summary>
        public string _currentStepText;
#endif

        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryCounter" /> class.
        /// </summary>
        /// <param name="currentStepText">
        ///     The sql.
        /// </param>
        public QueryWatcher(string currentStepText)
        {
#if DEBUG
            this._currentStepText = currentStepText;

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
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
#if DEBUG
            this._stopWatch.Stop();

            double duration = (double)this._stopWatch.ElapsedMilliseconds / 1000.0;

            this._currentStepText = "{0}: {1:N3}".FormatWith(this._currentStepText, duration);

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
                    HttpContext.Current.Items["CmdQueries"] = this._currentStepText;
                }
                else
                {
                    HttpContext.Current.Items["CmdQueries"] += "<br />" + this._currentStepText;
                }
            }

#endif
        }
    }
}