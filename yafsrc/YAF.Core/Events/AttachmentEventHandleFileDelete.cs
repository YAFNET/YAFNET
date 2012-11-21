namespace YAF.Core
{
    using System;
    using System.Linq;

    using YAF.Classes;
    using YAF.Core.Model;
    using YAF.Types.Attributes;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The attachment event handle file delete.
    /// </summary>
    [ExportService(ServiceLifetimeScope.OwnedByContainer)]
    public class AttachmentEventHandleFileDelete : IHandleEvent<RepositoryEvent<Attachment>>
    {
        #region Fields

        /// <summary>
        /// The _attachment repository.
        /// </summary>
        private readonly IRepository<Attachment> _attachmentRepository;

        /// <summary>
        /// The _board settings.
        /// </summary>
        private readonly YafBoardSettings _boardSettings;

        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILogger _logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentEventHandleFileDelete"/> class.
        /// </summary>
        /// <param name="boardSettings">
        /// The board settings.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="attachmentRepository">
        /// The attachment repository.
        /// </param>
        public AttachmentEventHandleFileDelete(YafBoardSettings boardSettings, ILogger logger, IRepository<Attachment> attachmentRepository)
        {
            this._boardSettings = boardSettings;
            this._logger = logger;
            this._attachmentRepository = attachmentRepository;
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
        public void Handle(RepositoryEvent<Attachment> @event)
        {
            if (@event.RepositoryEventType != RepositoryEventType.Delete || this._boardSettings.UseFileTable)
            {
                return;
            }

            if (@event.Entity == null)
            {
                @event.Entity = this._attachmentRepository.ListTyped(attachmentID: @event.EntityId).FirstOrDefault();
            }

            if (@event.Entity == null)
            {
                return;
            }

            try
            {
                @event.Entity.DeleteFile();
            }
            catch (Exception e)
            {
                // error deleting that file... 
                this._logger.Warn(e, "Error Deleting Attachment");
            }
        }

        #endregion
    }
}