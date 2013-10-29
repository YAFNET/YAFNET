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

namespace YAF.Core.Services.Startup
{
    #region Using

    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Tasks;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    ///     The yaf initialize db.
    /// </summary>
    public class StartupInitializeDb : BaseStartupService, ICriticalStartupService
    {
        #region Properties

        /// <summary>
        ///     Gets InitVarName.
        /// </summary>
        [NotNull]
        protected override string InitVarName
        {
            get
            {
                return "YafInitializeDb_Init";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The run service.
        /// </summary>
        /// <returns>
        ///     The run service.
        /// </returns>
        protected override bool RunService()
        {
            // init the db...
            string errorStr = string.Empty;
            bool debugging = false;

#if DEBUG
            debugging = true;
#endif

            if (HttpContext.Current != null)
            {
                var response = YafContext.Current.Get<HttpResponseBase>();

                if (Config.ConnectionString == null)
                {
                    // attempt to create a connection string...
                    response.Redirect(YafForumInfo.ForumClientFileRoot + "install/default.aspx");
                    return false;
                }

                // attempt to init the db...
                if (!LegacyDb.forumpage_initdb(out errorStr, debugging))
                {
                    // unable to connect to the DB...
                    YafContext.Current.Get<HttpSessionStateBase>()["StartupException"] = errorStr;
                    response.Redirect(YafForumInfo.ForumClientFileRoot + "error.aspx");
                    return false;
                }

                // step 2: validate the database version...
                string redirectStr = LegacyDb.forumpage_validateversion(YafForumInfo.AppVersion);
                if (redirectStr.IsSet())
                {
                    response.Redirect(YafForumInfo.ForumClientFileRoot + redirectStr);
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}