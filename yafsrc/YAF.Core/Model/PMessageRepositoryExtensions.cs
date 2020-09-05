namespace YAF.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    
    using ServiceStack.OrmLite;

    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The PMessage Repository Extensions
    /// </summary>
    public static class PMessageRepositoryExtensions
    {
        #region Public Methods and Operators

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
        public static void PruneAll(this IRepository<PMessage> repository, [NotNull] int daysRead, [NotNull] int daysUnread)
        {
            CodeContracts.VerifyNotNull(repository);

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
        public static DataTable UserMessageCount(this IRepository<PMessage> repository, [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.DbFunction.GetAsDataTable(cdb => cdb.user_pmcount(UserID: userId));
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
        public static void DeleteMessage(this IRepository<PMessage> repository, [NotNull] int messageId, bool deleteFromOutbox)
        {
            CodeContracts.VerifyNotNull(repository);

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
            [NotNull] int fromUserID,
            [NotNull] int toUserID,
            [NotNull] string subject,
            [NotNull] string body,
            [NotNull] int flags,
            [CanBeNull] int? replyTo)
        {
            CodeContracts.VerifyNotNull(repository);

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
        /// Get Private Messages by user
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="view">
        /// The view.
        /// </param>
        /// <param name="sortField">
        /// The sort field.
        /// </param>
        /// <param name="sortAscending">
        /// The sort ascending.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<dynamic> List(
            this IRepository<PMessage> repository,
            [NotNull] int userId,
            [NotNull] PmView view,
            [NotNull] string sortField,
            [NotNull] bool sortAscending)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<PMessage>();

                    expression.Join<UserPMessage>((a, b) => a.ID == b.PMessageID)
                        .Join<UserPMessage, User>(
                            (b, c) => b.UserID == Sql.TableAlias(c.ID, "c"),
                            db.Connection.TableAlias("c")).Join<User>(
                            (a, d) => a.FromUserID == Sql.TableAlias(d.ID, "d"),
                            db.Connection.TableAlias("d"));

                    switch (view)
                    {
                        case PmView.Inbox:
                            expression.Where<UserPMessage>(
                                b => b.UserID == userId && b.IsArchived == false && b.IsDeleted == false);
                            break;
                        case PmView.Outbox:
                            expression.Where<PMessage, UserPMessage>(
                                (a, b) => a.FromUserID == userId && b.IsArchived == false && b.IsInOutbox == true);
                            break;
                        case PmView.Archive:
                            expression.Where<UserPMessage>(b => b.UserID == userId && b.IsArchived == true);
                            break;
                    }

                    switch (sortField)
                    {
                        case "Subject":
                        {
                            if (sortAscending)
                            {
                                expression.OrderBy<PMessage>(a => a.Subject);
                            }
                            else
                            {
                                expression.OrderByDescending<PMessage>(a => a.Subject);
                            }
                        }

                            break;
                        case "Created":
                        {
                            if (sortAscending)
                            {
                                expression.OrderBy<PMessage>(a => a.Created);
                            }
                            else
                            {
                                expression.OrderByDescending<PMessage>(a => a.Created);
                            }
                        }

                            break;
                        case "ToUser":
                        {
                            if (sortAscending)
                            {
                                expression.OrderBy<UserPMessage>(a => a.UserID);
                            }
                            else
                            {
                                expression.OrderByDescending<UserPMessage>(a => a.UserID);
                            }
                        }

                            break;
                        case "FromUser":
                        {
                            if (sortAscending)
                            {
                                expression.OrderBy<PMessage>(a => a.FromUserID);
                            }
                            else
                            {
                                expression.OrderByDescending<PMessage>(a => a.FromUserID);
                            }
                        }

                            break;
                    }

                    expression.Select<PMessage, UserPMessage, User, User>(
                        (a, b, c, d) => new
                        {
                            a.ReplyTo,
                            PMessageID = a.ID,
                            UserPMessageID = b.ID,
                            a.FromUserID,
                            FromUser = Sql.TableAlias("d.Name", "d"),
                            FromUserDisplayName = Sql.TableAlias("d.DisplayName", "d"),
                            FromStyle = Sql.TableAlias("d.UserStyle", "d"),
                            FromSuspended = Sql.TableAlias("d.Suspended", "d"),
                            ToUserID = b.UserID,
                            ToUser = Sql.TableAlias("c.Name", "c"),
                            ToUserDisplayName = Sql.TableAlias("c.DisplayName", "c"),
                            ToStyle = Sql.TableAlias("c.UserStyle", "c"),
                            ToSuspended = Sql.TableAlias("c.Suspended", "c"),
                            a.Created,
                            a.Subject,
                            a.Body,
                            a.Flags,
                            UserPMFlags = b.Flags,
                            b.IsRead,
                            b.IsReply,
                            b.IsInOutbox,
                            b.IsArchived,
                            b.IsDeleted
                        });

                    return db.Connection.Select<object>(expression);
                });
        }

        /// <summary>
        /// Gets the the Private Messages By Id
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userPMessageID">
        /// The user Private Message ID.
        /// </param>
        /// <returns>
        /// The <see cref="dynamic"/>.
        /// </returns>
        public static dynamic GetMessage(this IRepository<PMessage> repository, [NotNull] int userPMessageID)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.List(userPMessageID, false).FirstOrDefault();
        }

        /// <summary>
        /// Gets the list of Private Messages By Id
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userPMessageID">
        /// The user Private Message ID.
        /// </param>
        /// <param name="includeReplies">
        /// The include Replies.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<dynamic> List(
            this IRepository<PMessage> repository,
            [NotNull] int userPMessageID,
            bool includeReplies)
        {
            CodeContracts.VerifyNotNull(repository);

            List<dynamic> messages = repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<PMessage>();

                    expression.Join<UserPMessage>((a, b) => a.ID == b.PMessageID)
                        .Join<UserPMessage, User>((b, c) => b.UserID == Sql.TableAlias(c.ID, "c"), db.Connection.TableAlias("c"))
                        .Join<User>((a, d) => a.FromUserID == Sql.TableAlias(d.ID, "d"), db.Connection.TableAlias("d"))
                        .Where<UserPMessage>(b => b.ID == userPMessageID).OrderBy<PMessage>(a => a.Created)
                        .Select<PMessage, UserPMessage, User, User>(
                            (a, b, c, d) => new
                            {
                                a.ReplyTo,
                                PMessageID = a.ID,
                                UserPMessageID = b.ID,
                                a.FromUserID,
                                FromUser = Sql.TableAlias("d.Name", "d"),
                                FromUserDisplayName = Sql.TableAlias("d.DisplayName", "d"),
                                FromStyle = Sql.TableAlias("d.UserStyle", "d"),
                                FromSuspended = Sql.TableAlias("d.Suspended", "d"),
                                ToUserID = b.UserID,
                                ToUser = Sql.TableAlias("c.Name", "c"),
                                ToUserDisplayName = Sql.TableAlias("c.DisplayName", "c"),
                                ToStyle = Sql.TableAlias("c.UserStyle", "c"),
                                ToSuspended = Sql.TableAlias("c.Suspended", "c"),
                                a.Created,
                                a.Subject,
                                a.Body,
                                a.Flags,
                                UserPMFlags = b.Flags,
                                b.IsRead,
                                b.IsReply,
                                b.IsInOutbox,
                                b.IsArchived,
                                b.IsDeleted
                            });

                    return db.Connection.Select<object>(expression);
                });

            return includeReplies
                ? repository.GetReplies(messages).OrderBy(m => m.Created).ToList()
                : messages.OrderBy(m => m.Created).ToList();
        }

        /// <summary>
        /// Get All Message Replies
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messages">
        /// The messages.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private static IEnumerable<dynamic> GetReplies(this IRepository<PMessage> repository, [CanBeNull] List<dynamic> messages)
        {
            CodeContracts.VerifyNotNull(repository);

            if (!messages.Any())
            {
                return messages;
            }

            var message = messages.FirstOrDefault();

            if (message.ReplyTo != null)
            {
                var replyToId = (int)message.ReplyTo;

                // Get original message
                messages.Add(repository.GetMessage(replyToId));

                // Get Other Replies ?!
                messages.AddRange(repository.ListReplies(replyToId));
            }
            else
            {
                // Check if this Message has replies
                var replies = repository.ListReplies((int)message.PMessageID);

                if (replies.Any())
                {
                    messages.AddRange(replies);
                }
            }

            return messages.GroupBy(m => m.PMessageID).Select(m => m.First()).ToList();
        }

        /// <summary>
        /// List all Replies
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="replyPMessageId">
        /// The reply p message id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static List<dynamic> ListReplies(this IRepository<PMessage> repository, [NotNull] int replyPMessageId)
        {
            CodeContracts.VerifyNotNull(repository);

            List<dynamic> messages = repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<PMessage>();

                    expression.Join<UserPMessage>((a, b) => a.ID == b.PMessageID)
                        .Join<UserPMessage, User>((b, c) => b.UserID == Sql.TableAlias(c.ID, "c"), db.Connection.TableAlias("c"))
                        .Join<User>((a, d) => a.FromUserID == Sql.TableAlias(d.ID, "d"), db.Connection.TableAlias("d"))
                        .Where<PMessage>(b => b.ReplyTo == replyPMessageId).OrderBy<PMessage>(a => a.Created)
                        .Select<PMessage, UserPMessage, User, User>(
                            (a, b, c, d) => new
                            {
                                a.ReplyTo,
                                PMessageID = a.ID,
                                UserPMessageID = b.ID,
                                a.FromUserID,
                                FromUser = Sql.TableAlias("d.Name", "d"),
                                FromUserDisplayName = Sql.TableAlias("d.DisplayName", "d"),
                                FromStyle = Sql.TableAlias("d.UserStyle", "d"),
                                FromSuspended = Sql.TableAlias("d.Suspended", "d"),
                                ToUserID = b.UserID,
                                ToUser = Sql.TableAlias("c.Name", "c"),
                                ToUserDisplayName = Sql.TableAlias("c.DisplayName", "c"),
                                ToStyle = Sql.TableAlias("c.UserStyle", "c"),
                                ToSuspended = Sql.TableAlias("c.Suspended", "c"),
                                a.Created,
                                a.Subject,
                                a.Body,
                                a.Flags,
                                b.IsRead,
                                b.IsReply,
                                b.IsInOutbox,
                                b.IsArchived,
                                b.IsDeleted
                            });

                    return db.Connection.Select<object>(expression);
                });

            return messages.OrderBy(m => m.Created).ToList();
        }

        #endregion
    }
}