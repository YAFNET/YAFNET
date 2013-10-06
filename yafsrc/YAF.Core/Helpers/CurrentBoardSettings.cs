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
    #region Using

    using System.Web;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    ///     The current board settings.
    /// </summary>
    public class CurrentBoardSettings : IReadWriteProvider<YafBoardSettings>
    {
        #region Fields

        /// <summary>
        ///     The _application state base.
        /// </summary>
        private readonly HttpApplicationStateBase _applicationStateBase;

        /// <summary>
        ///     The _have board id.
        /// </summary>
        private readonly IHaveBoardID _haveBoardId;

        /// <summary>
        ///     The _inject services.
        /// </summary>
        private readonly IInjectServices _injectServices;

        /// <summary>
        ///     The _treat cache key.
        /// </summary>
        private readonly ITreatCacheKey _treatCacheKey;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentBoardSettings"/> class.
        /// </summary>
        /// <param name="applicationStateBase">
        /// The application state base.
        /// </param>
        /// <param name="injectServices">
        /// The inject services.
        /// </param>
        /// <param name="haveBoardId">
        /// The have board id.
        /// </param>
        /// <param name="treatCacheKey">
        /// </param>
        /// <param name="boardRepository">
        /// The board Repository.
        /// </param>
        public CurrentBoardSettings(
            [NotNull] HttpApplicationStateBase applicationStateBase, 
            [NotNull] IInjectServices injectServices, 
            [NotNull] IHaveBoardID haveBoardId, 
            [NotNull] ITreatCacheKey treatCacheKey)
        {
            CodeContracts.VerifyNotNull(applicationStateBase, "applicationStateBase");
            CodeContracts.VerifyNotNull(injectServices, "injectServices");
            CodeContracts.VerifyNotNull(haveBoardId, "haveBoardId");
            CodeContracts.VerifyNotNull(treatCacheKey, "treatCacheKey");

            this._applicationStateBase = applicationStateBase;
            this._injectServices = injectServices;
            this._haveBoardId = haveBoardId;
            this._treatCacheKey = treatCacheKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets Object.
        /// </summary>
        public YafBoardSettings Instance
        {
            get
            {
                return this._applicationStateBase.GetOrSet(
                    this._treatCacheKey.Treat(Constants.Cache.BoardSettings), 
                    () =>
                        {
                            var boardSettings = new YafLoadBoardSettings(this._haveBoardId.BoardID);

                            // inject
                            this._injectServices.Inject(boardSettings);

                            return boardSettings;
                        });
            }

            set
            {
                this._applicationStateBase.Set(this._treatCacheKey.Treat(Constants.Cache.BoardSettings), value);
            }
        }

        #endregion
    }
}