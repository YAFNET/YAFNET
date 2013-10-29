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

namespace YAF.Providers.Utils
{
    using System;
    using System.Configuration;
    using System.Web;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    public static class ConnStringHelpers
    {
        public static void TrySetConnectionAppString(string connStrName, string appKeyName)
        {
            // is the connection string set?
            if (!connStrName.IsSet())
            {
                return;
            }

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connStrName];

            if (connectionStringSettings == null)
            {
                // doesn't exist... can't set
                return;
            }

            string connStr = connectionStringSettings.ConnectionString;

            // set the app variable...
            var application = YafContext.Current.Get<HttpApplicationStateBase>();

            CodeContracts.VerifyNotNull(application, @"Application");

            if (application[appKeyName] == null)
            {
                application.Add(appKeyName, connStr);
            }
            else
            {
                application[appKeyName] = connStr;
            }
        }
    }
}