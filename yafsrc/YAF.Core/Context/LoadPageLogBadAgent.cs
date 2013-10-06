/* Yet Another Forum.net
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
namespace YAF.Core
{
    using System.Web;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The load page log bad agent.
    /// </summary>
    [ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
    public class LoadPageLogBadAgent : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPageLogBadAgent"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="httpRequestBase">
        /// The http request base.
        /// </param>
        public LoadPageLogBadAgent(
            [NotNull] IServiceLocator serviceLocator,
            [NotNull] ILogger logger,
            [NotNull] HttpRequestBase httpRequestBase)
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");
            CodeContracts.VerifyNotNull(logger, "logger");
            CodeContracts.VerifyNotNull(httpRequestBase, "httpRequestBase");

            this.ServiceLocator = serviceLocator;
            this.Logger = logger;
            this.HttpRequestBase = httpRequestBase;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets HttpRequestBase.
        /// </summary>
        public HttpRequestBase HttpRequestBase { get; set; }

        /// <summary>
        /// Gets or sets Logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        ///   Gets Order.
        /// </summary>
        public int Order
        {
            get
            {
                return 2000;
            }
        }

        /// <summary>
        ///   Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region Implemented Interfaces

        #region IHandleEvent<InitPageLoadEvent>

        /// <summary>
        /// Handles the specified @event.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void Handle([NotNull] InitPageLoadEvent @event)
        {
            // vzrus: to log unhandled UserAgent strings
            if (!this.Get<YafBoardSettings>().UserAgentBadLog)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(@event.Data.UserAgent))
            {
                this.Logger.Warn("UserAgent string is empty.");
            }

            if ((@event.Data.Platform.ToLower().Contains("unknown")
                 || @event.Data.Browser.ToLower().Contains("unknown"))
                && (!UserAgentHelper.IsSearchEngineSpider(@event.Data.UserAgent)))
            {
                this.Logger.Log(
                    YafContext.Current.User != null ? YafContext.Current.User.UserName : string.Empty,
                    this,
                    "Unhandled UserAgent string:'{0}'<br />Platform:'{1}'<br />Browser:'{2}'".FormatWith(
                        (string)@event.Data.UserAgent,
                        this.HttpRequestBase.Browser.Platform,
                        this.HttpRequestBase.Browser.Browser));
            }
        }

        #endregion

        #endregion
    }
}