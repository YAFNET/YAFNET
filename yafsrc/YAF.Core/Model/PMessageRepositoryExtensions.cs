namespace YAF.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The PMessage Repository Extensions
    /// </summary>
    public static class PMessageRepositoryExtensions
    {
        #region Public Methods and Operators

        public static DataTable GetReplies(this IRepository<PMessage> repository, PMessage message, int replyTo)
        {
            var messages = repository.ListAsDataTable(null, null, replyTo);
            var originalMessage = messages.GetFirstRow();

            if (originalMessage == null)
            {
                return messages;
            }

            var orginalPMMessage = new PMessage
                                       {
                                           ReplyTo = originalMessage["ReplyTo"].ToType<int>(),
                                           ID = originalMessage["PMessageID"].ToType<int>()
                                       };

            if (message.ID == orginalPMMessage.ID)
            {
                messages.Rows.RemoveAt(0);;
            }

            if (originalMessage["IsReply"].ToType<bool>())
            {
               var replies1 = repository.GetReplies(orginalPMMessage, orginalPMMessage.ReplyTo.Value);

                if (replies1 != null)
                {
                    messages.Merge(replies1);
                }
            }

            return messages;
        }

        public static void PruneAll(this IRepository<PMessage> repository, DateTime daysRead, DateTime daysUnread)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.pmessage_prune(
                DaysRead: daysRead,
                DaysUnread: daysUnread,
                UTCTIMESTAMP: DateTime.UtcNow);
        }

        public static DataTable UserMessageCount(this IRepository<PMessage> repository, int userId)
        {
            return repository.DbFunction.GetAsDataTable(cdb => cdb.user_pmcount(UserID: userId));
        }

        public static void ArchiveMessage(this IRepository<PMessage> repository, [NotNull] object userPMessageID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Scalar.pmessage_archive(UserPMessageID:userPMessageID);
        }


        public static void DeleteMessage(this IRepository<PMessage> repository, int messageId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Scalar.pmessage_delete(UserPMessageID: messageId, FromOutbox: true);
        }


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
        /// Returns a list of private messages based on the arguments specified.
        ///   If pMessageID != null, returns the PM of id pMessageId.
        ///   If toUserID != null, returns the list of PMs sent to the user with the given ID.
        ///   If fromUserID != null, returns the list of PMs sent by the user of the given ID.
        /// </summary>
        /// <param name="toUserID">
        /// </param>
        /// <param name="fromUserID">
        /// </param>
        /// <param name="userPMessageID">
        /// The user P Message ID.
        /// </param>
        /// <returns>
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
        /// Returns a list of private messages based on the arguments specified.
        ///   If pMessageID != null, returns the PM of id pMessageId.
        ///   If toUserID != null, returns the list of PMs sent to the user with the given ID.
        ///   If fromUserID != null, returns the list of PMs sent by the user of the given ID.
        /// </summary>
        /// <param name="userPMessageID">
        /// The user P Message ID.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable ListAsDataTable(
            this IRepository<PMessage> repository, [NotNull] object userPMessageID)
        {
            return repository.ListAsDataTable(null, null, userPMessageID);
        }

        #endregion
    }
}