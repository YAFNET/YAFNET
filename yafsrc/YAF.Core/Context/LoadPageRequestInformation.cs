// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadPageRequestInformation.cs" company="">
//   
// </copyright>
// <summary>
//   The load page request information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Core
{
	using System.Web;

	using YAF.Classes;
	using YAF.Types;
	using YAF.Types.Attributes;
	using YAF.Types.EventProxies;
	using YAF.Types.Interfaces;
	using YAF.Utils;
	using YAF.Utils.Helpers;

	/// <summary>
	/// The load page request information.
	/// </summary>
	[ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
	public class LoadPageRequestInformation : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LoadPageRequestInformation"/> class.
		/// </summary>
		/// <param name="serviceLocator">
		/// The service locator.
		/// </param>
		/// <param name="httpRequestBase">
		/// The http request base.
		/// </param>
		public LoadPageRequestInformation([NotNull] IServiceLocator serviceLocator, [NotNull] HttpRequestBase httpRequestBase)
		{
			this.ServiceLocator = serviceLocator;
			this.HttpRequestBase = httpRequestBase;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets HttpRequestBase.
		/// </summary>
		public HttpRequestBase HttpRequestBase { get; set; }

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
			string browser = "{0} {1}".FormatWith(this.HttpRequestBase.Browser.Browser, this.HttpRequestBase.Browser.Version);
			string platform = this.HttpRequestBase.Browser.Platform;

			bool isSearchEngine;
			bool dontTrack;

			string userAgent = this.HttpRequestBase.UserAgent;

			bool isMobileDevice = UserAgentHelper.IsMobileDevice(userAgent) || this.HttpRequestBase.Browser.IsMobileDevice;

			// try and get more verbose platform name by ref and other parameters             
			UserAgentHelper.Platform(
				userAgent, this.HttpRequestBase.Browser.Crawler, ref platform, ref browser, out isSearchEngine, out dontTrack);

			dontTrack = !this.Get<YafBoardSettings>().ShowCrawlersInActiveList && isSearchEngine;

			// don't track if this is a feed reader. May be to make it switchable in host settings.
			// we don't have page 'g' token for the feed page.
			if (browser.Contains("Unknown") && !dontTrack)
			{
				dontTrack = UserAgentHelper.IsFeedReader(userAgent);
			}

			@event.Data.DontTrack = dontTrack;
			@event.Data.UserAgent = userAgent;
			@event.Data.IsSearchEngine = isSearchEngine;
			@event.Data.IsMobileDevice = isMobileDevice;
			@event.Data.Browser = browser;
			@event.Data.Platform = platform;

			YafContext.Current.Vars["DontTrack"] = dontTrack;
		}

		#endregion

		#endregion
	}
}