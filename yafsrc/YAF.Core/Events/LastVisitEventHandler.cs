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
namespace YAF.Core.Events
{
    #region Using

    using System;
    using System.Globalization;
    using System.Web;

    using YAF.Core;
    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The last visit handler.
    /// </summary>
    [ExportService(ServiceLifetimeScope.InstancePerScope)]
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
            [NotNull] ISession yafSession, HttpRequestBase requestBase, HttpResponseBase responseBase)
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
        public ISession YafSession { get; set; }

        /// <summary>
        /// Handles the specified @event.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void Handle(ForumPageUnloadEvent @event)
        {
            /*/if (BoardContext.Current.IsGuest && (!this.YafSession.LastVisit.HasValue || this.YafSession.LastVisit.Value == DateTimeHelper.SqlDbMinTime()))
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

            if (!BoardContext.Current.IsGuest && BoardContext.Current.Page[previousVisitKey] != DBNull.Value
                && !this.YafSession.LastVisit.HasValue)
            {
                this.YafSession.LastVisit = Convert.ToDateTime(BoardContext.Current.Page[previousVisitKey]);
            }
            else if (BoardContext.Current.IsGuest && !this.YafSession.LastVisit.HasValue)
            {
                if (this._requestBase.Cookies.Get(previousVisitKey) != null)
                {
                    // have previous visit cookie...
                    var previousVisitInsecure = this._requestBase.Cookies.Get(previousVisitKey).Value;

                    if (DateTime.TryParse(previousVisitInsecure, out var previousVisit))
                    {
                        this.YafSession.LastVisit = previousVisit;
                    }
                }
                else
                {
                    this.YafSession.LastVisit = DateTimeHelper.SqlDbMinTime();
                }

                // set the last visit cookie...
                var httpCookie = new HttpCookie(previousVisitKey, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture))
                    {
                       Expires = DateTime.Now.AddMonths(6) 
                    };
                this._responseBase.Cookies.Add(httpCookie);
            }
        }

        #endregion

        #endregion
    }
}