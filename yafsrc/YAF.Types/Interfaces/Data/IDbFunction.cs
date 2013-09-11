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
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    #endregion

    /// <summary>
    ///     The db function type.
    /// </summary>
    public enum DbFunctionType
    {
        /// <summary>
        ///     The query.
        /// </summary>
        Query, 

        /// <summary>
        ///     The data table.
        /// </summary>
        DataTable, 

        /// <summary>
        ///     The data set.
        /// </summary>
        DataSet, 

        /// <summary>
        ///     The scalar.
        /// </summary>
        Scalar, 

        /// <summary>
        ///     The reader.
        /// </summary>
        Reader
    }

    /// <summary>
    ///     The db function cancelled exception.
    /// </summary>
    public class DbFunctionCancelledException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbFunctionCancelledException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        public DbFunctionCancelledException([NotNull] string message)
            : base(message)
        {
        }

        #endregion
    }

    /// <summary>
    ///     The i db function.
    /// </summary>
    public interface IDbFunction
    {
        #region Public Properties

        /// <summary>
        ///     Gets GetData.
        /// </summary>
        dynamic GetData { get; }

        /// <summary>
        ///     Gets GetDataSet.
        /// </summary>
        dynamic GetDataSet { get; }

        /// <summary>
        ///     Gets Query.
        /// </summary>
        dynamic Query { get; }

        /// <summary>
        ///     Gets Scalar.
        /// </summary>
        dynamic Scalar { get; }

        /// <summary>
        /// Gets all the Db Specific Functions
        /// </summary>
        IEnumerable<IDbSpecificFunction> DbSpecificFunctions { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create session.
        /// </summary>
        /// <param name="isolationLevel"> </param>
        /// <returns>
        /// The <see cref="IDbFunctionSession"/>.
        /// </returns>
        IDbFunctionSession CreateSession(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);

        #endregion
    }
}