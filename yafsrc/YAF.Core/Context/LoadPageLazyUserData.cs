// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadPageLazyUserData.cs" company="">
//   
// </copyright>
// <summary>
//   The load page lazy user data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Core
{
	using System.Data;

	using YAF.Types;
	using YAF.Types.Attributes;
	using YAF.Types.Constants;
	using YAF.Types.EventProxies;
	using YAF.Types.Interfaces;
	using YAF.Types.Interfaces.Extensions;
	using YAF.Utils;
	using YAF.Utils.Extensions;

	/// <summary>
	/// The load page lazy user data.
	/// </summary>
	[ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
	public class LoadPageLazyUserData : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
	{
		#region Constants and Fields

		/// <summary>
		///   The _db broker.
		/// </summary>
		private readonly IDBBroker _dbBroker;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LoadPageLazyUserData"/> class.
		/// </summary>
		/// <param name="serviceLocator">
		/// The service locator.
		/// </param>
		/// <param name="dbBroker">
		/// The db broker.
		/// </param>
		/// <param name="dataCache">
		/// The data Cache.
		/// </param>
		public LoadPageLazyUserData(
			[NotNull] IServiceLocator serviceLocator, [NotNull] IDBBroker dbBroker)
		{
			this._dbBroker = dbBroker;
			this.ServiceLocator = serviceLocator;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets DataCache.
		/// </summary>
		// public IDataCache DataCache { get; set; }

		/// <summary>
		///   Gets Order.
		/// </summary>
		public int Order
		{
			get
			{
				return 3000;
			}
		}

		/// <summary>
		///   Gets or sets ServiceLocator.
		/// </summary>
		public IServiceLocator ServiceLocator { get; set; }

		#endregion

		#region Implemented Interfaces

		#region IHandleEvent<InitPageLoadEvent>

		/// <summary>
		/// The handle.
		/// </summary>
		/// <param name="event">
		/// The event.
		/// </param>
		public void Handle([NotNull] InitPageLoadEvent @event)
		{
			DataRow activeUserLazyData = this._dbBroker.ActiveUserLazyData(@event.Data.UserID);

			if (activeUserLazyData != null)
			{
				// add the lazy user data to this page data...
				@event.DataDictionary.AddRange(activeUserLazyData.ToDictionary());
			}
		}

		#endregion

		#endregion
	}
}