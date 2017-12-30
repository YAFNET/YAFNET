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
    using System.Web;

    /// <summary>
    ///     The query counter.
    /// </summary>
    public static class QueryCounter
    {
#if DEBUG

        /// <summary>
        ///     The reset.
        /// </summary>
        public static void Reset()
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            HttpContext.Current.Items["NumQueries"] = 0;
            HttpContext.Current.Items["TimeQueries"] = (double)0;
            HttpContext.Current.Items["CmdQueries"] = string.Empty;
        }

        /// <summary>
        ///     Gets Count.
        /// </summary>
        public static int Count
        {
            get
            {
                return (int)((HttpContext.Current == null) ? 0 : HttpContext.Current.Items["NumQueries"]);
            }
        }

        /// <summary>
        ///     Gets Duration.
        /// </summary>
        public static double Duration
        {
            get
            {
                return (double)((HttpContext.Current == null) ? 0.0 : HttpContext.Current.Items["TimeQueries"]);
            }
        }

        /// <summary>
        ///     Gets Commands.
        /// </summary>
        public static string Commands
        {
            get
            {
                return (string)((HttpContext.Current == null) ? string.Empty : HttpContext.Current.Items["CmdQueries"]);
            }
        }
#endif
    }
}