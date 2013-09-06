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
namespace YAF.Types.Interfaces.Data
{
	#region Using

    using System;
    using System.Collections.Generic;
    using System.Data;

    #endregion

    /// <summary>
	/// The db specific function.
	/// </summary>
    public interface IDbSpecificFunction : IDbSortableOperation
	{
		#region Properties

		/// <summary>
		/// Gets ProviderName.
		/// </summary>
		string ProviderName { get; }

		#endregion

		#region Public Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="dbfunctionType">
        /// The dbfunction type.
        /// </param>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="transaction"></param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The execute.
        /// </returns>
        bool Execute(
	        DbFunctionType dbfunctionType,
	        string operationName,
	        IEnumerable<KeyValuePair<string, object>> parameters,
	        out object result,
            IDbTransaction transaction = null);

		#endregion
	}
}