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
namespace YAF.Core.Data
{
	#region Using

	using System;
	using System.Collections.Generic;
	using System.Data.Common;
	using System.Dynamic;
	using System.Linq;

	using YAF.Types;
	using YAF.Types.Interfaces;
	using YAF.Types.Interfaces.Data;
	using YAF.Utils;

	#endregion

	/// <summary>
	/// The dynamic db function.
	/// </summary>
	public class DynamicDbFunction : IDbFunction
	{
		#region Constants and Fields

		/// <summary>
		/// The _db specific functions.
		/// </summary>
		private readonly IEnumerable<IDbSpecificFunction> _dbSpecificFunctions;

		/// <summary>
		///   The _get data proxy.
		/// </summary>
		private readonly TryInvokeMemberProxy _getDataProxy;

		/// <summary>
		///   The _get data set proxy.
		/// </summary>
		private readonly TryInvokeMemberProxy _getDataSetProxy;

		/// <summary>
		///   The _query proxy.
		/// </summary>
		private readonly TryInvokeMemberProxy _queryProxy;

		/// <summary>
		///   The _scalar proxy.
		/// </summary>
		private readonly TryInvokeMemberProxy _scalarProxy;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicDbFunction"/> class.
		/// </summary>
		/// <param name="dbAccess">
		/// The db access.
		/// </param>
		/// <param name="dbSpecificFunctions">
		/// The db Specific Functions.
		/// </param>
		public DynamicDbFunction([NotNull] IDbAccess dbAccess, IEnumerable<IDbSpecificFunction> dbSpecificFunctions)
		{
			this._dbSpecificFunctions = dbSpecificFunctions;
			this.DbAccess = dbAccess;
			this._getDataProxy = new TryInvokeMemberProxy(this.InvokeGetData);
			this._getDataSetProxy = new TryInvokeMemberProxy(this.InvokeGetDataSet);
			this._queryProxy = new TryInvokeMemberProxy(this.InvokeQuery);
			this._scalarProxy = new TryInvokeMemberProxy(this.InvokeScalar);
		}

		#endregion

		#region Properties

		/// <summary>
		///   Gets or sets DbAccess.
		/// </summary>
		public IDbAccess DbAccess { get; set; }

		/// <summary>
		///   Gets GetData.
		/// </summary>
		public dynamic GetData
		{
			get
			{
				return this._getDataProxy.ToDynamic();
			}
		}

		/// <summary>
		///   Gets GetDataSet.
		/// </summary>
		public dynamic GetDataSet
		{
			get
			{
				return this._getDataSetProxy.ToDynamic();
			}
		}

		/// <summary>
		///   Gets Query.
		/// </summary>
		public dynamic Query
		{
			get
			{
				return this._queryProxy.ToDynamic();
			}
		}

		/// <summary>
		///   Gets Scalar.
		/// </summary>
		public dynamic Scalar
		{
			get
			{
				return this._scalarProxy.ToDynamic();
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// The db function execute.
		/// </summary>
		/// <param name="functionType">
		/// The function Type.
		/// </param>
		/// <param name="binder">
		/// The binder.
		/// </param>
		/// <param name="parameters">
		/// The parameters.
		/// </param>
		/// <param name="executeDb">
		/// The execute db.
		/// </param>
		/// <param name="result">
		/// The result.
		/// </param>
		/// <returns>
		/// The db function execute.
		/// </returns>
		protected bool DbFunctionExecute(
			DbFunctionType functionType, 
			[NotNull] InvokeMemberBinder binder, 
			[NotNull] IList<KeyValuePair<string, object>> parameters, 
			[NotNull] Func<DbCommand, object> executeDb, 
			[CanBeNull] out object result)
		{
			CodeContracts.ArgumentNotNull(binder, "binder");
			CodeContracts.ArgumentNotNull(parameters, "parameters");
			CodeContracts.ArgumentNotNull(executeDb, "executeDb");

			var operationName = binder.Name;

			// see if there's a specific function override...
			var specificFunction =
				this._dbSpecificFunctions.OrderBy(f => f.SortOrder).Where(
					f => f.OperationNames.Any(s => string.Equals(s, operationName, StringComparison.OrdinalIgnoreCase))).FirstOrDefault
					();

			if (specificFunction != null && specificFunction.Execute(functionType, operationName, parameters, out result))
			{
				return true;
			}

			using (var cmd = this.DbAccess.GetCommand(operationName.ToLower(), true, parameters))
			{
				result = executeDb(cmd);
			}

			return true;
		}

		/// <summary>
		/// The invoke get data.
		/// </summary>
		/// <param name="binder">
		/// The binder.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		/// <param name="result">
		/// The result.
		/// </param>
		/// <returns>
		/// The invoke get data.
		/// </returns>
		protected bool InvokeGetData(
			[NotNull] InvokeMemberBinder binder, [NotNull] object[] args, [NotNull] out object result)
		{
			return this.DbFunctionExecute(
				DbFunctionType.DataTable, 
				binder, 
				this.MapParameters(binder.CallInfo, args), 
				(cmd) => this.DbAccess.GetData(cmd), 
				out result);
		}

		/// <summary>
		/// The invoke get data set.
		/// </summary>
		/// <param name="binder">
		/// The binder.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		/// <param name="result">
		/// The result.
		/// </param>
		/// <returns>
		/// The invoke get data set.
		/// </returns>
		protected bool InvokeGetDataSet(
			[NotNull] InvokeMemberBinder binder, [NotNull] object[] args, [NotNull] out object result)
		{
			return this.DbFunctionExecute(
				DbFunctionType.DataSet, 
				binder, 
				this.MapParameters(binder.CallInfo, args), 
				(cmd) => this.DbAccess.GetDataset(cmd), 
				out result);
		}

		/// <summary>
		/// The invoke query.
		/// </summary>
		/// <param name="binder">
		/// The binder.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		/// <param name="result">
		/// The result.
		/// </param>
		/// <returns>
		/// The invoke query.
		/// </returns>
		protected bool InvokeQuery([NotNull] InvokeMemberBinder binder, [NotNull] object[] args, [NotNull] out object result)
		{
			return this.DbFunctionExecute(
				DbFunctionType.Query, 
				binder, 
				this.MapParameters(binder.CallInfo, args), 
				(cmd) =>
					{
						this.DbAccess.ExecuteNonQuery(cmd);
						return null;
					}, 
				out result);
		}

		/// <summary>
		/// The invoke scalar.
		/// </summary>
		/// <param name="binder">
		/// The binder.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		/// <param name="result">
		/// The result.
		/// </param>
		/// <returns>
		/// The invoke scalar.
		/// </returns>
		protected bool InvokeScalar([NotNull] InvokeMemberBinder binder, [NotNull] object[] args, [NotNull] out object result)
		{
			return this.DbFunctionExecute(
				DbFunctionType.Scalar, 
				binder, 
				this.MapParameters(binder.CallInfo, args), 
				(cmd) => this.DbAccess.ExecuteScalar(cmd), 
				out result);
		}

		/// <summary>
		/// The map parameters.
		/// </summary>
		/// <param name="callInfo">
		/// The call info.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		/// <returns>
		/// </returns>
		[NotNull]
		protected IList<KeyValuePair<string, object>> MapParameters([NotNull] CallInfo callInfo, [NotNull] object[] args)
		{
			CodeContracts.ArgumentNotNull(callInfo, "callInfo");
			CodeContracts.ArgumentNotNull(args, "args");

			return
				args.Reverse().Zip(
					callInfo.ArgumentNames.Reverse().Infinite(), (a, name) => new KeyValuePair<string, object>(name, a)).Reverse().
					ToList();
		}

		#endregion
	}
}