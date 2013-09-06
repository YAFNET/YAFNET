/* Yet Another Forum.NET
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

    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     The search data cleanup module.
    /// </summary>
    public class SearchDataCleanupForumModule : SimpleBaseForumModule
    {
        #region Fields

        /// <summary>
        ///     The _page pre load.
        /// </summary>
        private readonly IFireEvent<ForumPagePreLoadEvent> _pagePreLoad;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchDataCleanupForumModule" /> class.
        /// </summary>
        /// <param name="pagePreLoad">
        ///     The page pre load.
        /// </param>
        public SearchDataCleanupForumModule([NotNull] IFireEvent<ForumPagePreLoadEvent> pagePreLoad)
        {
            this._pagePreLoad = pagePreLoad;

            this._pagePreLoad.HandleEvent += this._pagePreLoad_HandleEvent;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The _page pre load_ handle event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void _pagePreLoad_HandleEvent([NotNull] object sender, [NotNull] EventConverterArgs<ForumPagePreLoadEvent> e)
        {
            // no security features for login/logout pages
            if (this.ForumPageType == ForumPages.search)
            {
                return;
            }

            // clear out any search data in the session.... just in case...
            this.Get<IYafSession>().SearchData = null;
        }

        #endregion
    }
}