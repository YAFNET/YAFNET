namespace YAF.Core.Services.Cache
{
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    /// <summary>
    /// The category event handle cache invalidate.
    /// </summary>
    [ExportService(ServiceLifetimeScope.OwnedByContainer)]
    public class CategoryEventHandleCacheInvalidate : IHandleEvent<RepositoryEvent<Category>>
    {
        #region Fields

        /// <summary>
        /// The _data cache.
        /// </summary>
        private readonly IDataCache _dataCache;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryEventHandleCacheInvalidate"/> class.
        /// </summary>
        /// <param name="dataCache">
        /// The data cache.
        /// </param>
        public CategoryEventHandleCacheInvalidate(IDataCache dataCache)
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
                return 10000;
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
        public void Handle(RepositoryEvent<Category> @event)
        {
            if (!@event.RepositoryEventType.IsIn(RepositoryEventType.Delete, RepositoryEventType.Update))
            {
                return;
            }

            // clear moderatorss cache
            this._dataCache.Remove(Constants.Cache.ForumModerators);

            // clear category cache...
            this._dataCache.Remove(Constants.Cache.ForumCategory);

            // clear active discussions cache..
            this._dataCache.Remove(Constants.Cache.ForumActiveDiscussions);
        }

        #endregion
    }
}