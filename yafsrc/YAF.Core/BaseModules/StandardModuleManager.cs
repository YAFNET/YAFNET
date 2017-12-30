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
namespace YAF.Core
{
	#region Using

	using System.Collections.Generic;
	using System.Linq;

	using YAF.Types;
	using YAF.Types.Interfaces;

	#endregion

	/// <summary>
	/// The standard module manager.
	/// </summary>
	/// <typeparam name="TModule">
	/// The module type based on IBaseModule.
	/// </typeparam>
	public class StandardModuleManager<TModule> : IModuleManager<TModule>
		where TModule : IModuleDefinition
	{
		#region Constants and Fields

		/// <summary>
		/// The _modules.
		/// </summary>
		private readonly IList<TModule> _modules;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="StandardModuleManager{TModule}"/> class.
		/// </summary>
		/// <param name="modules">
		/// The modules.
		/// </param>
		public StandardModuleManager([NotNull] IEnumerable<TModule> modules)
		{
			CodeContracts.VerifyNotNull(modules, "modules");

		    this._modules = modules.ToList();
		}

		#endregion

		#region Implemented Interfaces

		#region IModuleManager<TModule>

		/// <summary>
		/// Get all instances of modules available.
		/// </summary>
		/// <param name="getInactive">
		/// The get Inactive.
		/// </param>
		/// <returns>
		/// </returns>
		public IEnumerable<TModule> GetAll(bool getInactive)
		{
		    return !getInactive ? this._modules.Where(m => m.Active) : this._modules;
		}

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
		/// Instance of TModule or null if not found.
		/// </returns>
		public TModule GetBy([NotNull] string id, bool getInactive)
		{
			CodeContracts.VerifyNotNull(id, "id");

		    return !getInactive
		               ? this._modules.SingleOrDefault(e => e.ModuleId.Equals(id) && e.Active)
		               : this._modules.SingleOrDefault(e => e.ModuleId.Equals(id));
		}

		/// <summary>
		/// Get an instance of a module (based on it's id).
		/// </summary>
		/// <param name="id">
		/// The id.
		/// </param>
		/// <returns>
		/// Instance of TModule or null if not found.
		/// </returns>
		public TModule GetBy([NotNull] string id)
		{
			CodeContracts.VerifyNotNull(id, "id");

			return this._modules.SingleOrDefault(e => e.ModuleId.Equals(id));
		}

		#endregion

		#endregion
	}
}