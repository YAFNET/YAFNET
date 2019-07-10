/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
namespace YAF.Modules
{
    #region Using

    using System;
    using System.Web;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The last visit handler.
    /// </summary>
    [ExportService(serviceLifetimeScope: ServiceLifetimeScope.InstancePerScope)]
    public class LastVisitEventHandler : IHandleEvent<ForumPagePreLoadEvent>, IHandleEvent<ForumPageUnloadEvent>
    {
        /// <summary>
        /// The _request base
        /// </summary>
        private readonly HttpRequestBase _requestBase;

        /// <summary>
        /// The _response base
        /// </summary>
        private readonly HttpResponseBase _responseBase;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LastVisitEventHandler"/> class.
        /// </summary>
        /// <param name="yafSession">The YAF session.</param>
        /// <param name="requestBase">The request base.</param>
        /// <param name="responseBase">The response base.</param>
        public LastVisitEventHandler(
            [NotNull] IYafSession yafSession, HttpRequestBase requestBase, HttpResponseBase responseBase)
        {
            this._requestBase = requestBase;
            this._responseBase = responseBase;
            this.YafSession = yafSession;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Order.
        /// </summary>
        public int Order => 1000;

        /// <summary>
        /// Gets or sets YafSession.
        /// </summary>
        public IYafSession YafSession { get; set; }

        /// <summary>
        /// Handles the specified @event.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void Handle(ForumPageUnloadEvent @event)
        {
            /*/if (YafContext.Current.IsGuest && (!this.YafSession.LastVisit.HasValue || this.YafSession.LastVisit.Value == DateTimeHelper.SqlDbMinTime()))
            //{
            //  // update last visit going forward...
            //  this.YafSession.LastVisit = DateTime.UtcNow;
            //}*/
        }

        #endregion

        #region Implemented Interfaces

        #region IHandleEvent<ForumPagePreLoadEvent>

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        public void Handle([NotNull] ForumPagePreLoadEvent @event)
        {
            var previousVisitKey = "PreviousVisit";

            if (!YafContext.Current.IsGuest && YafContext.Current.Page[key: previousVisitKey] != DBNull.Value
                && !this.YafSession.LastVisit.HasValue)
            {
                this.YafSession.LastVisit = Convert.ToDateTime(value: YafContext.Current.Page[key: previousVisitKey]);
            }
            else if (YafContext.Current.IsGuest && !this.YafSession.LastVisit.HasValue)
            {
                if (this._requestBase.Cookies.Get(name: previousVisitKey) != null)
                {
                    // have previous visit cookie...
                    var previousVisitInsecure = this._requestBase.Cookies.Get(name: previousVisitKey).Value;
                    DateTime previousVisit;

                    if (DateTime.TryParse(s: previousVisitInsecure, result: out previousVisit))
                    {
                        this.YafSession.LastVisit = previousVisit;
                    }
                }
                else
                {
                    this.YafSession.LastVisit = DateTimeHelper.SqlDbMinTime();
                }

                // set the last visit cookie...
                var httpCookie = new HttpCookie(name: previousVisitKey, value: DateTime.UtcNow.ToString())
                    {
                       Expires = DateTime.Now.AddMonths(months: 6) 
                    };
                this._responseBase.Cookies.Add(cookie: httpCookie);
            }
        }

        #endregion

        #endregion
    }
}