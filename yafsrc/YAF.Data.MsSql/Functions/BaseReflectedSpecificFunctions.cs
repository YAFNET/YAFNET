/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Data.MsSql.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;

    using YAF.Types.Extensions;
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
        protected readonly IDictionary<MethodInfo, ParameterInfo[]> Methods;

        /// <summary>
        ///     The _supported operations.
        /// </summary>
        protected readonly List<string> SupportedOperations;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseReflectedSpecificFunctions"/> class.
        /// </summary>
        /// <param name="staticReflectedClass">
        /// The static reflected class. 
        /// </param>
        /// <param name="dbAccess">
        /// The db Access.
        /// </param>
        protected BaseReflectedSpecificFunctions(IReflect staticReflectedClass, IDbAccess dbAccess)
            :base(dbAccess)
        {
            this.Methods = staticReflectedClass
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .ToDictionary(k => k, k => k.GetParameters());
            this.SupportedOperations = this.Methods.Select(x => x.Key.Name).ToList();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Handle the run operation.
        /// </summary>
        /// <param name="sqlConnection">
        /// The sql Connection.
        /// </param>
        /// <param name="dbTransaction">
        /// The db Transaction.
        /// </param>
        /// <param name="dbfunctionType">
        /// The dbfunction Type.
        /// </param>
        /// <param name="operationName">
        /// The operation Name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        protected override bool RunOperation(
            SqlConnection sqlConnection,
            IDbTransaction dbTransaction,
            DbFunctionType dbfunctionType,
            string operationName,
            IEnumerable<KeyValuePair<string, object>> parameters,
            out object result)
        {
            // find operation...
            var method = this.Methods.FirstOrDefault(x => string.Equals(x.Key.Name, operationName, StringComparison.OrdinalIgnoreCase));

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

                var param1 = param;
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
            return this.SupportedOperations.Any(x => string.Equals(x, operationName, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }
}