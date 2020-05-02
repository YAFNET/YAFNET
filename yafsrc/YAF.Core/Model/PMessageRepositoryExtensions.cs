namespace YAF.Core.Model
{
    using System;
    using System.Data;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The PMessage Repository Extensions
    /// </summary>
    public static class PMessageRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get replies.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="replyTo">
        /// The reply to.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable GetReplies(this IRepository<PMessage> repository, PMessage message, int replyTo)
        {
            var messages = repository.ListAsDataTable(null, null, replyTo);
            var originalMessage = messages.GetFirstRow();

            if (originalMessage == null)
            {
                return messages;
            }

            var originalPMMessage = new PMessage
                                       {
                                           ReplyTo = originalMessage["ReplyTo"].ToType<int>(),
                                           ID = originalMessage["PMessageID"].ToType<int>()
                                       };

            if (message.ID == originalPMMessage.ID)
            {
                messages.Rows.RemoveAt(0);
            }

            if (!originalMessage["IsReply"].ToType<bool>())
            {
                return messages;
            }

            var replies1 = repository.GetReplies(originalPMMessage, originalPMMessage.ReplyTo.Value);

            if (replies1 != null)
            {
                messages.Merge(replies1);
            }

            return messages;
        }

        /// <summary>
        /// Prune All Private Messages
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="daysRead">
        /// The days read.
        /// </param>
        /// <param name="daysUnread">
        /// The days unread.
        /// </param>
        public static void PruneAll(this IRepository<PMessage> repository, DateTime daysRead, DateTime daysUnread)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.pmessage_prune(
                DaysRead: daysRead,
                DaysUnread: daysUnread,
                UTCTIMESTAMP: DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the User Message Count
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable UserMessageCount(this IRepository<PMessage> repository, int userId)
        {
            return repository.DbFunction.GetAsDataTable(cdb => cdb.user_pmcount(UserID: userId));
        }

        /// <summary>
        /// archive message.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userPMessageID">
        /// The user p message id.
        /// </param>
        public static void ArchiveMessage(this IRepository<PMessage> repository, [NotNull] object userPMessageID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Scalar.pmessage_archive(UserPMessageID: userPMessageID);
        }

        /// <summary>
        /// Deletes the Private Message
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="deleteFromOutbox">
        /// The delete From Outbox.
        /// </param>
        public static void DeleteMessage(this IRepository<PMessage> repository, int messageId, bool deleteFromOutbox)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Scalar.pmessage_delete(UserPMessageID: messageId, FromOutbox: deleteFromOutbox);
        }

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="fromUserID">
        /// The from user id.
        /// </param>
        /// <param name="toUserID">
        /// The to user id.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <param name="replyTo">
        /// The reply to.
        /// </param>
        public static void SendMessage(
            this IRepository<PMessage> repository,
            int fromUserID,
            int toUserID,
            [NotNull] string subject,
            [NotNull] string body,
            [NotNull] int flags,
            [CanBeNull] int? replyTo)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Scalar.pmessage_save(
                FromUserID: fromUserID,
                ToUserID: toUserID,
                Subject: subject,
                Body: body,
                Flags: flags,
                ReplyTo: replyTo,
                UTCTIMESTAMP: DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the list of Private Messages
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="toUserID">
        /// The to User ID.
        /// </param>
        /// <param name="fromUserID">
        /// The from User ID.
        /// </param>
        /// <param name="userPMessageID">
        /// The user P Message ID.
        /// </param>
        /// <returns>
        /// Returns a list of private messages based on the arguments specified.
        ///   If pMessageID != null, returns the PM of id pMessageId.
        ///   If toUserID != null, returns the list of PMs sent to the user with the given ID.
        ///   If fromUserID != null, returns the list of PMs sent by the user of the given ID.
        /// </returns>
        public static DataTable ListAsDataTable(
            this IRepository<PMessage> repository, [NotNull] object toUserID, [NotNull] object fromUserID, [NotNull] object userPMessageID)
        {
            return repository.DbFunction.GetData.pmessage_list(
                ToUserID: toUserID,
                FromUserID: fromUserID,
                UserPMessageID: userPMessageID);
        }

        /// <summary>
        /// Gets the list of Private Messages
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userPMessageID">
        /// The user Private Message ID.
        /// </param>
        /// <returns>
        /// Returns a list of private messages based on the arguments specified.
        ///   If pMessageID != null, returns the PM of id pMessageId.
        ///   If toUserID != null, returns the list of PMs sent to the user with the given ID.
        ///   If fromUserID != null, returns the list of PMs sent by the user of the given ID.
        /// </returns>
        public static DataTable ListAsDataTable(
            this IRepository<PMessage> repository, [NotNull] object userPMessageID)
        {
            return repository.ListAsDataTable(null, null, userPMessageID);
        }

        #endregion
    }
}