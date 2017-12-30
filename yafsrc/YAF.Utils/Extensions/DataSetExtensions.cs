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
namespace YAF.Utils.Extensions
{
	using System.Data;

	using YAF.Types;

	/// <summary>
	/// The data set extensions.
	/// </summary>
	public static class DataSetExtensions
	{
		#region Public Methods

		/// <summary>
		/// The get table.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		/// <param name="basicTableName">
		/// The basic table name.
		/// </param>
		/// <returns>
		/// </returns>
		public static DataTable GetTable([NotNull] this DataSet dataSet, [NotNull] string basicTableName)
		{
			CodeContracts.VerifyNotNull(dataSet, "dataSet");
			CodeContracts.VerifyNotNull(basicTableName, "basicTableName");

			return dataSet.Tables[DataExtensions.GetObjectName(basicTableName)];
		}

		#endregion
	}
}