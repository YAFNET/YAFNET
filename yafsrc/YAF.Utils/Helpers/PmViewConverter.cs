/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

 namespace YAF.Utils.Helpers
{
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;

    /// <summary>
    /// Converts <see cref="PmView"/>s to and from their URL query string representations.
    /// </summary>
    public static class PmViewConverter
    {
        #region Public Methods

        /// <summary>
        /// Returns a <see cref="PmView"/> based on its URL query string value.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <returns>Returns the Current View</returns>
        public static PmView FromQueryString([NotNull] string param)
        {
            if (param.IsNotSet())
            {
                return PmView.Inbox;
            }

            return param.ToLower() switch
                {
                    "out" => PmView.Outbox,
                    "in" => PmView.Inbox,
                    "arch" => PmView.Archive,
                    _ => PmView.Inbox
                };
        }

        /// <summary>
        /// Converts a <see cref="PmView"/> to a string representation appropriate for inclusion in a URL query string.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>
        /// The to query string param.
        /// </returns>
        [CanBeNull]
        public static string ToQueryStringParam(PmView view)
        {
            return view switch
                {
                    PmView.Outbox => "out",
                    PmView.Inbox => "in",
                    PmView.Archive => "arch",
                    _ => null
                };
        }

        #endregion
    }
}
