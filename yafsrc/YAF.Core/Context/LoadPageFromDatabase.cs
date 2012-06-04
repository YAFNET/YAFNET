// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadPageFromDatabase.cs" company="">
//   
// </copyright>
// <summary>
//   The load page from database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Core
{
	using System;
	using System.Data;
	using System.Linq;
	using System.Web;

	using YAF.Types;
	using YAF.Types.Attributes;
	using YAF.Types.EventProxies;
	using YAF.Types.Interfaces;
	using YAF.Types.Interfaces.Extensions;
	using YAF.Utils.Extensions;

	/// <summary>
	/// The load page from database.
	/// </summary>
	[ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
	public class LoadPageFromDatabase : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
	{
		private readonly IDbFunction _dbFunction;

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LoadPageFromDatabase"/> class.
		/// </summary>
		/// <param name="serviceLocator">
		/// The service locator.
		/// </param>
		/// <param name="legacyDb">
		/// The legacy db.
		/// </param>
		public LoadPageFromDatabase([NotNull] IServiceLocator serviceLocator, ILogger logger, IDbFunction dbFunction, [NotNull] IDataCache dataCache)
		{
			_dbFunction = dbFunction;
			this.ServiceLocator = serviceLocator;
			Logger = logger;
            this.DataCache = dataCache;
		}

		#endregion

		#region Properties

		public ILogger Logger { get; set; }

        public IDataCache DataCache { get; set; }
		/// <summary>
		///   Gets Order.
		/// </summary>
		public int Order
		{
			get
			{
				return 1000;
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
		/// <exception cref="ApplicationException">Failed to find guest user.</exception>
		public void Handle([NotNull] InitPageLoadEvent @event)
		{
			try
			{
				object userKey = null;

				if (YafContext.Current.User != null)
				{
					userKey = YafContext.Current.User.ProviderUserKey;
				}

				int tries = 0;
				DataRow pageRow;

				do
				{
					pageRow =
						((DataTable)
						 this._dbFunction.GetData.pageload(
						 	this.Get<HttpSessionStateBase>().SessionID,
						 	YafContext.Current.PageBoardID,
						 	userKey,
						 	this.Get<HttpRequestBase>().UserHostAddress,
						 	this.Get<HttpRequestBase>().FilePath,
						 	this.Get<HttpRequestBase>().QueryString.ToString(),
						 	@event.Data.Browser,
						 	@event.Data.Platform,
						 	@event.Data.CategoryID,
						 	@event.Data.ForumID,
						 	@event.Data.TopicID,
						 	@event.Data.MessageID,
						 	// don't track if this is a search engine
						 	@event.Data.IsSearchEngine,
						 	@event.Data.IsMobileDevice,
						@event.Data.DontTrack);

					// if the user doesn't exist...
					if (userKey != null && pageRow == null)
					{
						// create the user...
						if (!RoleMembershipHelper.DidCreateForumUser(YafContext.Current.User, YafContext.Current.PageBoardID))
						{
							throw new ApplicationException("Failed to create new user.");
						}
					}

					if (tries++ >= 2)
					{
						if (userKey != null && pageRow == null)
						{
							// probably no permissions, use guest user instead...
							userKey = null;
							continue;
						}

						// fail...
						break;
					}
				}
				while (pageRow == null && userKey != null);

				if (pageRow == null)
				{
					throw new ApplicationException("Unable to find the Guest User!");
				}

				// add all loaded page data into our data dictionary...
				@event.DataDictionary.AddRange(pageRow.ToDictionary());
                if (@event.DataDictionary["ActiveUpdate"].ToType<bool>())
                {
                    // purge the cache if something has changed...
                    this.DataCache.Remove(Constants.Cache.UsersOnlineStatus);
			}
			}
			catch (Exception x)
			{
#if !DEBUG

				// log the exception...
				this.Logger.Fatal(x, "Failure Initializing User/Page.");

				// log the user out...
				FormsAuthentication.SignOut();

				if (YafContext.Current.ForumPageType != ForumPages.info)
				{
					// show a failure notice since something is probably up with membership...
					YafBuildLink.RedirectInfoPage(InfoMessage.Failure);
				}
				else
				{
					// totally failing... just re-throw the exception...
					throw;
				}
#else
				// re-throw exception...
				throw;
#endif
			}
		}

		#endregion

		#endregion
	}
}