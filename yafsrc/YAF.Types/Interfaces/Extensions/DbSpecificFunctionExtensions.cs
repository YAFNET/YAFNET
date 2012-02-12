/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
namespace YAF.Types.Interfaces.Extensions
{
	#region Using

	using System;
	using System.Collections.Generic;
	using System.Linq;

	using YAF.Types.Interfaces.Data;

	#endregion

	/// <summary>
	/// The db specific function extensions.
	/// </summary>
	public static class DbSpecificFunctionExtensions
	{
		#region Public Methods

		/// <summary>
		/// The is operation supported.
		/// </summary>
		/// <param name="functions">
		/// The functions.
		/// </param>
		/// <param name="providerName">
		/// The provider name.
		/// </param>
		/// <returns>
		/// The is operation supported.
		/// </returns>
		[NotNull]
		public static IEnumerable<IDbSpecificFunction> GetForProvider(
			[NotNull] this IEnumerable<IDbSpecificFunction> functions, [NotNull] string providerName)
		{
			CodeContracts.ArgumentNotNull(functions, "functions");
			CodeContracts.ArgumentNotNull(providerName, "providerName");

			return
				functions.Where(p => string.Equals(p.ProviderName, providerName, StringComparison.OrdinalIgnoreCase)).OrderBy(
					f => f.SortOrder);
		}

		/// <summary>
		/// The get for provider and operation.
		/// </summary>
		/// <param name="functions">
		/// The functions.
		/// </param>
		/// <param name="providerName">
		/// The provider name.
		/// </param>
		/// <param name="operationName">
		/// The operation name.
		/// </param>
		/// <returns>
		/// </returns>
		[NotNull]
		public static IEnumerable<IDbSpecificFunction> GetForProviderAndOperation(
			[NotNull] this IEnumerable<IDbSpecificFunction> functions, 
			[NotNull] string providerName, 
			[NotNull] string operationName)
		{
			CodeContracts.ArgumentNotNull(functions, "functions");
			CodeContracts.ArgumentNotNull(providerName, "providerName");
			CodeContracts.ArgumentNotNull(operationName, "operationName");

			return functions.GetForProvider(providerName).Where(s => s.IsSupportedOperation(operationName));
		}

		/// <summary>
		/// The is operation supported.
		/// </summary>
		/// <param name="functions">
		/// The functions.
		/// </param>
		/// <param name="providerName">
		/// The provider name.
		/// </param>
		/// <param name="operationName">
		/// The operation name.
		/// </param>
		/// <returns>
		/// The is operation supported.
		/// </returns>
		public static bool IsOperationSupported(
			[NotNull] this IEnumerable<IDbSpecificFunction> functions, 
			[NotNull] string providerName, 
			[NotNull] string operationName)
		{
			CodeContracts.ArgumentNotNull(functions, "functions");
			CodeContracts.ArgumentNotNull(providerName, "providerName");
			CodeContracts.ArgumentNotNull(operationName, "operationName");

			return functions.GetForProvider(providerName).Any(x => x.IsSupportedOperation(operationName));
		}

		#endregion
	}
}