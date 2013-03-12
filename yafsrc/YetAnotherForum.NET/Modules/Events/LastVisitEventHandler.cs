/* YetAnotherForum.NET
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

    #endregion

    /// <summary>
    /// The last visit handler.
    /// </summary>
    [ExportService(ServiceLifetimeScope.InstancePerScope)]
    public class LastVisitEventHandler : IHandleEvent<ForumPagePreLoadEvent>, IHandleEvent<ForumPageUnloadEvent>
    {
        private readonly HttpRequestBase _requestBase;

        private readonly HttpResponseBase _responseBase;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LastVisitEventHandler"/> class.
        /// </summary>
        /// <param name="yafSession">The yaf session.</param>
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
        public int Order
        {
            get
            {
                return 1000;
            }
        }

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
            /*/if (YafContext.Current.IsGuest && (!this.YafSession.LastVisit.HasValue || this.YafSession.LastVisit.Value == DateTime.MinValue))
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
            string previousVisitKey = "PreviousVisit";

            if (!YafContext.Current.IsGuest && YafContext.Current.Page[previousVisitKey] != DBNull.Value
                && !this.YafSession.LastVisit.HasValue)
            {
                this.YafSession.LastVisit = Convert.ToDateTime(YafContext.Current.Page[previousVisitKey]);
            }
            else if (YafContext.Current.IsGuest && !this.YafSession.LastVisit.HasValue)
            {
                if (this._requestBase.Cookies.Get(previousVisitKey) != null)
                {
                    // have previous visit cookie...
                    string previousVisitInsecure = this._requestBase.Cookies.Get(previousVisitKey).Value;
                    DateTime previousVisit;

                    if (DateTime.TryParse(previousVisitInsecure, out previousVisit))
                    {
                        this.YafSession.LastVisit = previousVisit;
                    }
                }
                else
                {
                    this.YafSession.LastVisit = DateTime.MinValue;
                }

                // set the last visit cookie...
                HttpCookie httpCookie = new HttpCookie(previousVisitKey, DateTime.UtcNow.ToString())
                    { Expires = DateTime.Now.AddMonths(6) };
                this._responseBase.Cookies.Add(httpCookie);
            }
        }

        #endregion

        #endregion
    }
}