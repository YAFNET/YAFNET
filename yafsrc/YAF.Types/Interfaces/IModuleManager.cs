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
namespace YAF.Types.Interfaces
{
    #region Using

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// The i module manager.
    /// </summary>
    /// <typeparam name="TModule">
    /// The module type of this module manager.
    /// </typeparam>
    public interface IModuleManager<out TModule>
      where TModule : IModuleDefinition
    {
        #region Public Methods

        /// <summary>
        /// Get an instance of a module (based on it's id).
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="getInactive">
        /// The get Inactive.
        /// </param>
        /// <returns>
        /// </returns>
        TModule GetBy(string id, bool getInactive);

        /// <summary>
        /// Get an instance of a module (based on it's id).
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// </returns>
        TModule GetBy(string id);

        /// <summary>
        /// Gets all the instances of the modules.
        /// </summary>
        /// <param name="getInactive">
        /// The get Inactive.
        /// </param>
        /// <returns>
        /// </returns>
        IEnumerable<TModule> GetAll(bool getInactive);

        #endregion
    }
}