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

namespace YAF.Core
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     The i module manager extensions.
    /// </summary>
    public static class IModuleManagerExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The get editors table.
        /// </summary>
        /// <typeparam name="TModule">
        /// </typeparam>
        /// <param name="moduleManager">
        ///     The module Manager.
        /// </param>
        /// <param name="tableName">
        ///     The table Name.
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        public static DataTable ActiveAsDataTable<TModule>(
            [NotNull] this IModuleManager<TModule> moduleManager,
            [NotNull] string tableName) where TModule : IModuleDefinition
        {
            CodeContracts.VerifyNotNull(moduleManager, "moduleManager");
            CodeContracts.VerifyNotNull(tableName, "tableName");

            using (var dataTable = new DataTable(tableName))
            {
                dataTable.Columns.Add("Value", Type.GetType("System.Int32"));
                dataTable.Columns.Add("Name", Type.GetType("System.String"));

                foreach (var module in moduleManager.GetAll())
                {
                    dataTable.Rows.Add(new object[] { module.ModuleId, module.Description });
                }

                return dataTable;
            }
        }

        /// <summary>
        ///     Get a dictionary list discribing the active modules.
        /// </summary>
        /// <typeparam name="TModule">
        /// </typeparam>
        /// <param name="moduleManager">
        ///     The module Manager.
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        public static IDictionary<string, string> ActiveAsDictionary<TModule>(
            [NotNull] this IModuleManager<TModule> moduleManager) where TModule : IModuleDefinition
        {
            CodeContracts.VerifyNotNull(moduleManager, "moduleManager");

            return moduleManager.GetAll().ToDictionary((mk) => mk.ModuleId, (mv) => mv.Description);
        }

        /// <summary>
        ///     Get all active modules.
        /// </summary>
        /// <typeparam name="TModule">
        /// </typeparam>
        /// <param name="moduleManager">
        ///     The module Manager.
        /// </param>
        /// <returns>
        /// </returns>
        public static IEnumerable<TModule> GetAll<TModule>([NotNull] this IModuleManager<TModule> moduleManager)
            where TModule : IModuleDefinition
        {
            CodeContracts.VerifyNotNull(moduleManager, "moduleManager");

            return moduleManager.GetAll(false);
        }

        #endregion
    }
}