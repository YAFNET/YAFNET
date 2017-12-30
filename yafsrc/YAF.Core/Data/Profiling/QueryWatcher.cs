/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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