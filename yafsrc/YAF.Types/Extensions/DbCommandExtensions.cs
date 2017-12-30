/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Types.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Dynamic;
    using System.Linq;

    using YAF.Types;

    /// <summary>
	/// The db command extensions.
	/// </summary>
	public static class DbCommandExtensions
	{
		#region Public Methods

		/// <summary>
		/// Extension method for adding in a bunch of parameters
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="parameters">
		/// </param>
		/// <param name="excludeNames">
		/// List of parameter exclusions.
		/// </param>
		public static void AddDynamicParams([NotNull] this IDbCommand cmd, [NotNull] dynamic parameters, [CanBeNull] params string[] excludeNames)
		{
			CodeContracts.VerifyNotNull(cmd, "cmd");
			CodeContracts.VerifyNotNull(parameters, "parameters");

			IDictionary<string, object> dictionary = ((object)parameters).AnyToDictionary();

			var excludeList = new List<string>();

			if (excludeNames != null)
			{
				excludeList.AddRange(excludeNames.Select(x => x.ToLower()));
			}

			foreach (var item in dictionary.Where(x => !excludeList.Contains(x.Key.ToLower())))
			{
				AddParam(cmd, item);
			}
		}

		/// <summary>
		/// Extension method for adding a parameter
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="item">
		/// The item.
		/// </param>
		public static void AddParam([NotNull] this IDbCommand cmd, [CanBeNull] object item)
		{
			CodeContracts.VerifyNotNull(cmd, "cmd");

			if (item is KeyValuePair<string, object>)
			{
				AddParam(cmd, (KeyValuePair<string, object>)item);
			}
			else
			{
				AddParam(cmd, new KeyValuePair<string, object>(null, item));
			}
		}

		/// <summary>
		/// Extension method for adding a parameter
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="name">
		/// The name.
		/// </param>
		/// <param name="item">
		/// The item.
		/// </param>
		public static void AddParam([NotNull] this IDbCommand cmd, [NotNull] string name, [CanBeNull] object item)
		{
			CodeContracts.VerifyNotNull(cmd, "cmd");
			CodeContracts.VerifyNotNull(name, "name");

			AddParam(cmd, new KeyValuePair<string, object>(name, item));
		}

		/// <summary>
		/// Extension for adding single parameter named or automatically named by number (0, 1, 2, 3, 4, etc.)
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="param">
		/// The param.
		/// </param>
		public static void AddParam([NotNull] this IDbCommand cmd, KeyValuePair<string, object> param)
		{
			CodeContracts.VerifyNotNull(cmd, "cmd");

			var item = param.Value;

			var p = cmd.CreateParameter();

			p.ParameterName = "{0}".FormatWith(param.Key.IsSet() ? param.Key : cmd.Parameters.Count.ToString());

			if (item == null)
			{
				p.Value = DBNull.Value;
			}
			else
			{
				if (item is string)
                {
                    var asString = item as string;

                    if (asString.Length < 4000)
                    {
                        p.Size = 4000;
                    }
                    else
                    {
                        p.Size = -1;
                    }

                    p.Value = item;
                }
                if (item is Guid)
                {
                    p.Value = item.ToString();
                    p.DbType = DbType.String;
                    p.Size = 4000;
                }
                else if (item is ExpandoObject)
                {
                    var d = (IDictionary<string, object>)item;
                    p.Value = d.Values.FirstOrDefault();
                }
				else
				{
					p.Value = item;
				}
			}

			cmd.Parameters.Add(p);
		}

		/// <summary>
		/// Extension method for adding in a bunch of parameters
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		public static void AddParams([NotNull] this DbCommand cmd, [NotNull] params object[] args)
		{
			CodeContracts.VerifyNotNull(cmd, "cmd");
			CodeContracts.VerifyNotNull(args, "args");

			foreach (var item in args)
			{
				AddParam(cmd, item);
			}
		}

		/// <summary>
		/// The create output parameter.
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="parameterName">
		/// The parameter name.
		/// </param>
		/// <param name="dbType">
		/// The db type.
		/// </param>
		/// <param name="size">
		/// The size.
		/// </param>
		/// <param name="direction">
		/// The direction.
		/// </param>
        public static void CreateOutputParameter([NotNull] this IDbCommand cmd, [NotNull] string parameterName,
                                                 DbType dbType,
                                                 int size = 0,
                                                 ParameterDirection direction = ParameterDirection.Output)
		{
		    CodeContracts.VerifyNotNull(cmd, "cmd");
		    CodeContracts.VerifyNotNull(parameterName, "parameterName");

		    var p = cmd.CreateParameter();

		    p.ParameterName = parameterName;
		    p.DbType = dbType;
		    p.Size = size;
		    p.Direction = direction;

		    cmd.Parameters.Add(p);
		}

        #endregion
	}
}