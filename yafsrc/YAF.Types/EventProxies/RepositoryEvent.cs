namespace YAF.Types.EventProxies
{
    #region Using

    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The repository event type.
    /// </summary>
    public enum RepositoryEventType
    {
        /// <summary>
        /// The new.
        /// </summary>
        New, 

        /// <summary>
        /// The update.
        /// </summary>
        Update, 

        /// <summary>
        /// The delete.
        /// </summary>
        Delete
    }

    /// <summary>
    ///     The repository event.
    /// </summary>
    public class RepositoryEvent : IAmEvent
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryEvent"/> class.
        /// </summary>
        /// <param name="repositoryEventType">
        /// The repository event type.
        /// </param>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        public RepositoryEvent(RepositoryEventType repositoryEventType, int? entityId)
        {
            this.RepositoryEventType = repositoryEventType;
            this.EntityId = entityId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        public int? EntityId { get; set; }

        /// <summary>
        /// Gets or sets the repository event type.
        /// </summary>
        public RepositoryEventType RepositoryEventType { get; set; }

        #endregion
    }
}