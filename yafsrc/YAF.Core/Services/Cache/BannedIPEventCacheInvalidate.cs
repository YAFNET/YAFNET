namespace YAF.Core.Services.Cache
{
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    /// <summary>
    /// The banned ip event cache invalidate.
    /// </summary>
    [ExportService(ServiceLifetimeScope.OwnedByContainer)]
    public class BannedIPEventCacheInvalidate : IHandleEvent<RepositoryEvent<BannedIP>>
    {
        #region Fields

        /// <summary>
        /// The _data cache.
        /// </summary>
        private readonly IDataCache _dataCache;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BannedIPEventCacheInvalidate"/> class.
        /// </summary>
        /// <param name="dataCache">
        /// The data cache.
        /// </param>
        public BannedIPEventCacheInvalidate(IDataCache dataCache)
        {
            this._dataCache = dataCache;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the order.
        /// </summary>
        public int Order
        {
            get
            {
                return 1000;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        public void Handle(RepositoryEvent<BannedIP> @event)
        {
            this._dataCache.Remove(Constants.Cache.BannedIP);
        }

        #endregion
    }
}