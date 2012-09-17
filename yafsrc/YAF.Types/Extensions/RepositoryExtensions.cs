namespace YAF.Core.Extensions
{
    #region Using

    using YAF.Types;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    /// The repository extensions.
    /// </summary>
    public static class RepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The fire deleted.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public static void FireDeleted<T>(this IRepository<T> repository, int? id)
            where T : IEntity
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            repository.DbEvent.Raise(new RepositoryEvent(RepositoryEventType.Delete, id));
        }

        /// <summary>
        /// The fire new.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public static void FireNew<T>([NotNull] this IRepository<T> repository, int? id)
            where T : IEntity
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            repository.DbEvent.Raise(new RepositoryEvent(RepositoryEventType.New, id));
        }

        /// <summary>
        /// The fire updated.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public static void FireUpdated<T>(this IRepository<T> repository, int? id)
            where T : IEntity
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            repository.DbEvent.Raise(new RepositoryEvent(RepositoryEventType.Update, id));
        }

        #endregion
    }
}