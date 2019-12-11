/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * https://www.yetanotherforum.net/
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

#if DEBUG
    using System.Diagnostics;
    using System.Web;
#endif

    /// <summary>
    /// The query watcher.
    /// </summary>
    public class QueryWatcher : IDisposable
    {
#if DEBUG

        /// <summary>
        /// The stop watch.
        /// </summary>
        private readonly Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// The current step text.
        /// </summary>
        private string currentStepText;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryWatcher"/> class. 
        /// </summary>
        /// <param name="currentStep">
        /// The current Step.
        /// </param>
        public QueryWatcher(string currentStep)
        {
#if DEBUG
            this.currentStepText = currentStep;

            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items["NumQueries"] == null)
                {
                    HttpContext.Current.Items["NumQueries"] = 1;
                }
                else
                {
                    HttpContext.Current.Items["NumQueries"] = 1 + (int)HttpContext.Current.Items["NumQueries"];
                }
            }

            this.stopWatch.Start();
#endif
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
#if DEBUG
            this.stopWatch.Stop();

            var duration = this.stopWatch.ElapsedMilliseconds / 1000.0;

            this.currentStepText = $"{this.currentStepText}: {duration:N3}";

            if (HttpContext.Current == null)
            {
                return;
            }

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
                HttpContext.Current.Items["CmdQueries"] = this.currentStepText;
            }
            else
            {
                HttpContext.Current.Items["CmdQueries"] += $"<br />{this.currentStepText}";
            }

#endif
        }
    }
}