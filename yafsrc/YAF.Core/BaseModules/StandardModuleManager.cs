/* YetAnotherForum.NET
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