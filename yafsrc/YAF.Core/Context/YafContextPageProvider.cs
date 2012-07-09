/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
    #region Using

    using System.Web;

    using Autofac;

    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The yaf context provider.
    /// </summary>
    internal class YafContextPageProvider : IReadOnlyProvider<YafContext>
    {
        #region Constants and Fields

        /// <summary>
        /// The page yaf context name.
        /// </summary>
        private const string PageYafContextName = "YAF.YafContext";

        /// <summary>
        /// The _container.
        /// </summary>
        private readonly ILifetimeScope _lifetimeScope;

        private readonly IInjectServices _injectServices;

        /// <summary>
        /// The _global instance.
        /// </summary>
        private static YafContext _globalInstance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafContextPageProvider"/> class.
        /// </summary>
        /// <param name="lifetimeScope">
        /// The container.
        /// </param>
        public YafContextPageProvider(ILifetimeScope lifetimeScope, IInjectServices injectServices)
        {
            this._lifetimeScope = lifetimeScope;
            _injectServices = injectServices;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Instance.
        /// </summary>
        public YafContext Instance
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return _globalInstance ?? (_globalInstance = this.CreateContextInstance());
                }

                var pageInstance = HttpContext.Current.Items[PageYafContextName] as YafContext;

                if (pageInstance == null)
                {
                    pageInstance = this.CreateContextInstance();

                    // make sure it's put back in the page...
                    HttpContext.Current.Items[PageYafContextName] = pageInstance;
                }

                return pageInstance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create context instance.
        /// </summary>
        /// <returns>
        /// </returns>
        private YafContext CreateContextInstance()
        {
            var lifetimeContainer = this._lifetimeScope.BeginLifetimeScope(YafLifetimeScope.Context);

            var instance = new YafContext(lifetimeContainer);
            this._injectServices.Inject(instance);

            return instance;
        }

        #endregion
    }
}