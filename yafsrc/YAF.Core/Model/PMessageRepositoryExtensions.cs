namespace YAF.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;

using ServiceStack.OrmLite;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Types.Constants;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

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
    public static void PruneAll(
        this IRepository<PMessage> repository,
        [NotNull] int daysRead,
        [NotNull] int daysUnread)
    {
        CodeContracts.VerifyNotNull(repository);

        // Delete Read Messages
        repository.DbAccess.Execute(
            db =>
                {
                    var q = db.Connection.From<UserPMessage>().Join<PMessage>((a, b) => a.PMessageID == b.ID);

                    q.Where(
                        $"{OrmLiteConfig.DialectProvider.DateDiffFunction("dd", q.Column<PMessage>(b1 => b1.Created, true), OrmLiteConfig.DialectProvider.GetUtcDateFunction())} > {daysRead}");

                    q.And(a => (a.Flags & 1) == 1);

                    return db.Connection.Delete(q);
                });

        // Delete Unread Messages
        repository.DbAccess.Execute(
            db =>
                {
                    var q = db.Connection.From<UserPMessage>().Join<PMessage>((a, b) => a.PMessageID == b.ID);

                    q.Where(
                        $"{OrmLiteConfig.DialectProvider.DateDiffFunction("dd", q.Column<PMessage>(b1 => b1.Created, true), OrmLiteConfig.DialectProvider.GetUtcDateFunction())} > {daysUnread}");

                    q.And(a => (a.Flags & 1) != 1);

                    return db.Connection.Delete(q);
                });

        // Delete old Messages
        repository.DbAccess.Execute(
            db =>
                {
                    var q = db.Connection.From<PMessage>();

                    q.UnsafeWhere(
                        $@"not exists (select 1 from {q.Table<UserPMessage>()} where {q.Column<UserPMessage>(x => x.PMessageID, true)} 
                                                                                       = {q.Column<PMessage>(p => p.ID, true)})");

                    return db.Connection.Delete(q);
                });
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
    public static PMessageCount UserMessageCount(this IRepository<PMessage> repository, [NotNull] int userId)
    {
        CodeContracts.VerifyNotNull(repository);

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

        expression.Join<UserGroup>((a, b) => b.UserID == a.ID).Join<UserGroup, Group>((b, c) => c.ID == b.GroupID)
            .Join<Rank>((a, d) => d.ID == a.RankID).Where<User>(a => a.ID == userId)
            .OrderByDescending<Group>(c => c.PMLimit).ThenByDescending<Rank>(c => c.PMLimit).Limit(1)
            .Select<Group, Rank>((c, d) => new { GroupLimit = c.PMLimit, RankLimit = d.PMLimit });

        var limit = repository.DbAccess
            .Execute(db => db.Connection.Single<(int GroupLimit, int? RankLimit)>(expression));

        var numberAllowed = limit.RankLimit ?? 0;

        if (limit.GroupLimit > limit.RankLimit)
        {
            numberAllowed = limit.GroupLimit;
        }

        // -- get count of pm's in user's sent items
        var countOutBoxExpression = OrmLiteConfig.DialectProvider.SqlExpression<UserPMessage>();

        countOutBoxExpression.Join<PMessage>((a, b) => a.PMessageID == b.ID).Where<UserPMessage, PMessage>(
            (a, b) => (a.Flags & 2) == 2 && (a.Flags & 8) != 8 && (a.Flags & 4) != 4 &&
                      b.FromUserID == userId);

        var numberOut = repository.DbAccess.Execute(db => db.Connection.Count(countOutBoxExpression)).ToType<int>();

        // -- get count of pm's in user's received items
        var countInBoxExpression = OrmLiteConfig.DialectProvider.SqlExpression<PMessage>();

        countInBoxExpression.Join<UserPMessage>((a, b) => b.PMessageID == a.ID).Where<PMessage, UserPMessage>(
            (a, b) => (b.Flags & 8) != 8 && (b.Flags & 4) != 4 && b.UserID == userId);

        var numberIn = repository.DbAccess.Execute(db => db.Connection.Count(countInBoxExpression)).ToType<int>();

        var countArchivedExpression = OrmLiteConfig.DialectProvider.SqlExpression<PMessage>();

        countArchivedExpression.Join<UserPMessage>((a, b) => b.PMessageID == a.ID).Where<PMessage, UserPMessage>(
            (a, b) => (b.Flags & 8) != 8 && (b.Flags & 4) == 4 && b.UserID == userId);

        var numberArchived = repository.DbAccess.Execute(db => db.Connection.Count(countArchivedExpression)).ToType<int>();

        var count = new PMessageCount
                        {
                            InboxCount = numberIn,
                            OutBoxCount = numberOut,
                            NumberTotal = numberIn + numberOut + numberArchived,
                            ArchivedCount = numberArchived,
                            Allowed = numberAllowed
                        };

        return count;
    }

    /// <summary>
    /// The send message.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="fromUserId">
    /// The from user id.
    /// </param>
    /// <param name="toUserId">
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
        [NotNull] int fromUserId,
        [NotNull] int toUserId,
        [NotNull] string subject,
        [NotNull] string body,
        [NotNull] int flags,
        [CanBeNull] int? replyTo)
    {
        CodeContracts.VerifyNotNull(repository);

        var newMessageId = repository.Insert(
            new PMessage
                {
                    FromUserID = fromUserId,
                    Created = DateTime.UtcNow,
                    Subject = subject,
                    Body = body,
                    Flags = flags,
                    ReplyTo = replyTo
                });

        if (replyTo.HasValue)
        {
            BoardContext.Current.GetRepository<UserPMessage>().UpdateOnly(
                () => new UserPMessage { IsReply = true },
                x => x.ID == replyTo.Value);
        }

        var userPMFlags = new PMessageFlags {
                                                    IsInOutbox = true
                                                };

        if (toUserId == 0)
        {
            // Get all board users
            var users = BoardContext.Current.GetRepository<User>().Get(
                u => u.BoardID == repository.BoardID && (u.Flags & 2) == 2 && (u.Flags & 4) != 4);

            users.ForEach(
                u => BoardContext.Current.GetRepository<UserPMessage>().Insert(
                    new UserPMessage { UserID = u.ID, PMessageID = newMessageId, Flags = userPMFlags.BitValue }));
        }
        else
        {
            BoardContext.Current.GetRepository<UserPMessage>().Insert(
                new UserPMessage { UserID = toUserId, PMessageID = newMessageId, Flags = userPMFlags.BitValue, IsReply = replyTo.HasValue});
        }

        repository.FireNew(newMessageId);
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
    public static List<PagedPm> List(
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
                                b => b.UserID == userId && (b.Flags & 4) != 4 && (b.Flags & 8) != 8);
                            break;
                        case PmView.Outbox:
                            expression.Where<PMessage, UserPMessage>(
                                (a, b) => a.FromUserID == userId && (b.Flags & 4) != 4 && (b.Flags & 2) == 2);
                            break;
                        case PmView.Archive:
                            expression.Where<UserPMessage>(b => b.UserID == userId && (b.Flags & 4) == 4);
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
                                                FromUser = Sql.TableAlias(d.Name, "d"),
                                                FromUserDisplayName = Sql.TableAlias(d.DisplayName, "d"),
                                                FromStyle = Sql.TableAlias(d.UserStyle, "d"),
                                                FromSuspended = Sql.TableAlias(d.Suspended, "d"),
                                                ToUserID = b.UserID,
                                                ToUser = Sql.TableAlias(c.Name, "c"),
                                                ToUserDisplayName = Sql.TableAlias(c.DisplayName, "c"),
                                                ToStyle = Sql.TableAlias(c.UserStyle, "c"),
                                                ToSuspended = Sql.TableAlias(c.Suspended, "c"),
                                                a.Created,
                                                a.Subject,
                                                a.Body,
                                                a.Flags,
                                                UserPMFlags = b.Flags,
                                                IsRead = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<UserPMessage>(x => x.Flags, true)}&1")})"),
                                                b.IsReply,
                                                IsInOutbox = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<UserPMessage>(x => x.Flags, true)}&2")})"),
                                                IsArchived = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<UserPMessage>(x => x.Flags, true)}&4")})"),
                                                IsDeleted = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<UserPMessage>(x => x.Flags, true)}&8")})")
                                            });

                    return db.Connection.Select<PagedPm>(expression);
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
    public static PagedPm GetMessage(this IRepository<PMessage> repository, [NotNull] int userPMessageID)
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
    public static List<PagedPm> List(
        this IRepository<PMessage> repository,
        [NotNull] int userPMessageID,
        [NotNull] bool includeReplies)
    {
        CodeContracts.VerifyNotNull(repository);

        var messages = repository.DbAccess.Execute(
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
                                                    FromUser = Sql.TableAlias(d.Name, "d"),
                                                    FromUserDisplayName = Sql.TableAlias(d.DisplayName, "d"),
                                                    FromStyle = Sql.TableAlias(d.UserStyle, "d"),
                                                    FromSuspended = Sql.TableAlias(d.Suspended, "d"),
                                                    FromAvatar = Sql.TableAlias(d.Avatar, "d"),
                                                    FromHasAvatarImage = Sql.TableAlias(d.AvatarImage, "d") != null ? true : false,
                                                    ToUserID = b.UserID,
                                                    ToUser = Sql.TableAlias(c.Name, "c"),
                                                    ToUserDisplayName = Sql.TableAlias(c.DisplayName, "c"),
                                                    ToStyle = Sql.TableAlias(c.UserStyle, "c"),
                                                    ToSuspended = Sql.TableAlias(c.Suspended, "c"),
                                                    a.Created,
                                                    a.Subject,
                                                    a.Body,
                                                    a.Flags,
                                                    UserPMFlags = b.Flags,
                                                    IsRead = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<UserPMessage>(x => x.Flags, true)}&1")})"),
                                                    b.IsReply,
                                                    IsInOutbox = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<UserPMessage>(x => x.Flags, true)}&2")})"),
                                                    IsArchived = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<UserPMessage>(x => x.Flags, true)}&4")})"),
                                                    IsDeleted = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<UserPMessage>(x => x.Flags, true)}&8")})")
                                                });

                    return db.Connection.Select<PagedPm>(expression);
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
    private static IEnumerable<PagedPm> GetReplies(
        this IRepository<PMessage> repository,
        [CanBeNull] List<PagedPm> messages)
    {
        CodeContracts.VerifyNotNull(repository);

        if (!messages.Any())
        {
            return messages;
        }

        var message = messages.FirstOrDefault();

        var replyTo = message.ReplyTo;

        if (replyTo is > 0)
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
            var replies = repository.ListReplies(message.PMessageID);

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
    private static List<PagedPm> ListReplies(this IRepository<PMessage> repository, [NotNull] int replyPMessageId)
    {
        CodeContracts.VerifyNotNull(repository);

        var messages = repository.DbAccess.Execute(
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
                                                    FromUser = Sql.TableAlias(d.Name, "d"),
                                                    FromUserDisplayName = Sql.TableAlias(d.DisplayName, "d"),
                                                    FromStyle = Sql.TableAlias(d.UserStyle, "d"),
                                                    FromSuspended = Sql.TableAlias(d.Suspended, "d"),
                                                    FromAvatar = Sql.TableAlias(d.Avatar, "d"),
                                                    FromHasAvatarImage = Sql.TableAlias(d.AvatarImage, "d") != null ? true : false,
                                                    ToUserID = b.UserID,
                                                    ToUser = Sql.TableAlias(c.Name, "c"),
                                                    ToUserDisplayName = Sql.TableAlias(c.DisplayName, "c"),
                                                    ToStyle = Sql.TableAlias(c.UserStyle, "c"),
                                                    ToSuspended = Sql.TableAlias(c.Suspended, "c"),
                                                    a.Created,
                                                    a.Subject,
                                                    a.Body,
                                                    a.Flags,
                                                    b.IsReply,
                                                    IsInOutbox = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<UserPMessage>(x => x.Flags, true)}&2")})"),
                                                    IsArchived = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<UserPMessage>(x => x.Flags, true)}&4")})"),
                                                    IsDeleted = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<UserPMessage>(x => x.Flags, true)}&8")})")
                                                });

                    return db.Connection.Select<PagedPm>(expression);
                });

        return messages.OrderBy(m => m.Created).ToList();
    }

    #endregion
}