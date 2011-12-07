// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicDbFunction.cs" company="">
//   
// </copyright>
// <summary>
//   The dynamic db function.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Core.Data
{
	using System;
	using System.Collections.Generic;
	using System.Data.Common;
	using System.Dynamic;
	using System.Linq;

	using YAF.Types;
	using YAF.Types.EventProxies;
	using YAF.Types.Interfaces;
	using YAF.Utils;

	/// <summary>
	/// The dynamic db function.
	/// </summary>
	public class DynamicDbFunction : IDbFunction
	{
		#region Constants and Fields

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
		public DynamicDbFunction([NotNull] IDbAccess dbAccess)
		{
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

	/// <summary>
	/// The dynamic db.
	/// </summary>
	public class TryInvokeMemberProxy : DynamicObject
	{
		#region Constants and Fields

		/// <summary>
		///   The _try invoke func.
		/// </summary>
		private readonly TryInvokeFunc _tryInvokeFunc;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TryInvokeMemberProxy"/> class.
		/// </summary>
		/// <param name="tryInvokeFunc">
		/// The try invoke func.
		/// </param>
		public TryInvokeMemberProxy([NotNull] TryInvokeFunc tryInvokeFunc)
		{
			CodeContracts.ArgumentNotNull(tryInvokeFunc, "tryInvokeFunc");

			this._tryInvokeFunc = tryInvokeFunc;
		}

		#endregion

		#region Delegates

		/// <summary>
		/// The try invoke func.
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
		public delegate bool TryInvokeFunc(
			[NotNull] InvokeMemberBinder binder, [NotNull] object[] args, [NotNull] out object result);

		#endregion

		#region Public Methods

		/// <summary>
		/// The try invoke member.
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
		/// The try invoke member.
		/// </returns>
		public override bool TryInvokeMember(
			[NotNull] InvokeMemberBinder binder, [NotNull] object[] args, [NotNull] out object result)
		{
			return this._tryInvokeFunc(binder, args, out result);
		}

		#endregion
	}
}