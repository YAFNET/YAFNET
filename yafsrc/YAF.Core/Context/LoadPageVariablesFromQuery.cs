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

    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// The load page variables from query.
    /// </summary>
    [ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
    public class LoadPageVariablesFromQuery : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPageVariablesFromQuery"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public LoadPageVariablesFromQuery([NotNull] IServiceLocator serviceLocator)
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");

            this.ServiceLocator = serviceLocator;
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
                return 10;
            }
        }

        /// <summary>
        /// Gets or sets ServiceLocator.
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
            var queryString = this.Get<HttpRequestBase>().QueryString;

            @event.Data.CategoryID = queryString.GetFirstOrDefault("c").ToTypeOrDefault(0);
            @event.Data.ForumID = queryString.GetFirstOrDefault("f").ToTypeOrDefault(0);
            @event.Data.TopicID = queryString.GetFirstOrDefault("t").ToTypeOrDefault(0);
            @event.Data.MessageID = queryString.GetFirstOrDefault("m").ToTypeOrDefault(0);

            if (YafContext.Current.Settings.CategoryID != 0)
            {
                @event.Data.CategoryID = YafContext.Current.Settings.CategoryID;
            }
        }

        #endregion

        #endregion
    }
}