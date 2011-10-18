namespace YAF.Core
{
	using System.Web;

	using YAF.Types;
	using YAF.Types.Attributes;
	using YAF.Types.EventProxies;
	using YAF.Types.Interfaces;
	using YAF.Utils;

	/// <summary>
	/// The load page variables from query.
	/// </summary>
	[ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
	public class LoadPageVariablesFromQuery : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LoadPageVariablesFromQuery"/> class.
		/// </summary>
		/// <param name="serviceLocator">
		/// The service locator.
		/// </param>
		public LoadPageVariablesFromQuery([NotNull] IServiceLocator serviceLocator)
		{
			CodeContracts.ArgumentNotNull(serviceLocator, "serviceLocator");

			this.ServiceLocator = serviceLocator;
		}

		#endregion

		#region Properties

		/// <summary>
		///   Gets Order.
		/// </summary>
		public int Order
		{
			get
			{
				return 10;
			}
		}

		/// <summary>
		/// Gets or sets ServiceLocator.
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
			var queryString = this.Get<HttpRequestBase>().QueryString;

			@event.Data.CategoryID = queryString.GetFirstOrDefault("c").ToTypeOrDefault<int>(0);
			@event.Data.ForumID = queryString.GetFirstOrDefault("f").ToTypeOrDefault<int>(0);
			@event.Data.TopicID = queryString.GetFirstOrDefault("t").ToTypeOrDefault<int>(0);
			@event.Data.MessageID = queryString.GetFirstOrDefault("m").ToTypeOrDefault<int>(0);

			if (YafContext.Current.Settings.CategoryID != 0)
			{
				@event.Data.CategoryID = YafContext.Current.Settings.CategoryID;
			}
		}

		#endregion

		#endregion
	}
}