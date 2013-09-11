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
namespace YAF.Data.MsSql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;

    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     The base reflected specific functions.
    /// </summary>
    public abstract class BaseReflectedSpecificFunctions : BaseMsSqlFunction
    {
        #region Fields

        /// <summary>
        ///     The _methods.
        /// </summary>
        protected readonly IDictionary<MethodInfo, ParameterInfo[]> _methods = null;

        /// <summary>
        ///     The _supported operations.
        /// </summary>
        protected readonly List<string> _supportedOperations;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseReflectedSpecificFunctions"/> class.
        /// </summary>
        /// <param name="staticReflectedClass">
        /// The static reflected class. 
        /// </param>
        protected BaseReflectedSpecificFunctions(Type staticReflectedClass, IDbAccess dbAccess)
            :base(dbAccess)
        {
            this._methods = staticReflectedClass
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .ToDictionary(k => k, k => k.GetParameters());
            this._supportedOperations = this._methods.Select(x => x.Key.Name).ToList();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Handle the run operation.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="dbfunctionType"></param>
        /// <param name="operationName"></param>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected override bool RunOperation(
            SqlConnection sqlConnection,
            IDbTransaction dbTransaction,
            DbFunctionType dbfunctionType,
            string operationName,
            IEnumerable<KeyValuePair<string, object>> parameters,
            out object result)
        {
            // find operation...
            var method = this._methods.FirstOrDefault(x => string.Equals(x.Key.Name, operationName, StringComparison.OrdinalIgnoreCase));

            if (method.IsDefault())
            {
                result = null;

                return false;
            }

            var mappedParameters = new List<object>();

            var globalParams = new Dictionary<Type, object>()
                               {
                                   { typeof(IDbTransaction), dbTransaction },
                                   { typeof(SqlConnection), sqlConnection },
                                   { typeof(DbFunctionType), dbfunctionType },
                                   { typeof(IDbAccess), this.DbAccess }
                               };

            var incomingParameters = parameters.ToList();
            var incomingIndex = 0;

            // match up parameters...
            foreach (var param in method.Value)
            {
                var global = globalParams.FirstOrDefault(p => p.Key == param.ParameterType);
                if (global.IsNotDefault())
                {
                    mappedParameters.Add(global.Value);
                    continue;
                }

                ParameterInfo param1 = param;
                var matchedNameParam = incomingParameters.FirstOrDefault(x => x.Key == param1.Name);

                if (matchedNameParam.IsNotDefault())
                {
                    // use this named parameter...
                    mappedParameters.Add(matchedNameParam.Value);
                }
                else if (incomingIndex < incomingParameters.Count)
                {
                    // just use the indexed value of...
                    mappedParameters.Add(incomingParameters[incomingIndex++].Value);
                }
            }

            // execute the method...
            result = method.Key.Invoke(null, mappedParameters.ToArray());

            return true;
        }

        /// <summary>
        /// The is supported operation.
        /// </summary>
        /// <param name="operationName">
        /// The operation name. 
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> . 
        /// </returns>
        public override bool IsSupportedOperation(string operationName)
        {
            return this._supportedOperations.Any(x => string.Equals(x, operationName, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }
}