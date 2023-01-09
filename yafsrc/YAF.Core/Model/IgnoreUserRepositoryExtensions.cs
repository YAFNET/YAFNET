namespace YAF.Core.Model;

using System.Collections.Generic;

using YAF.Types.Attributes;
using YAF.Types.Models;

/// <summary>
/// The IgnoreUser Repository Extensions
/// </summary>
public static class IgnoreUserRepositoryExtensions
{
    /// <summary>
    /// Deletes the specified user identifier.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="ignoreUserId">The ignore user identifier.</param>
    /// <returns>Returns if deleting was successfully or not</returns>
    public static bool Delete(
        this IRepository<IgnoreUser> repository,
        [NotNull] int userId,
        [NotNull] int ignoreUserId)
    {
        CodeContracts.VerifyNotNull(repository);

        var success = repository.DbAccess.Execute(
                          db => db.Connection.Delete<IgnoreUser>(
                              x => x.UserID == userId && x.IgnoredUserID == ignoreUserId)) ==
                      1;

        if (success)
        {
            repository.FireDeleted();
        }

        return success;
    }

    /// <summary>
    /// Add Ignored User
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    public static void AddIgnoredUser(
        this IRepository<IgnoreUser> repository,
        [NotNull] int userId,
        [NotNull] int ignoredUserId)
    {
        CodeContracts.VerifyNotNull(repository);

        var ignoreUser = repository.GetSingle(i => i.UserID == userId && i.IgnoredUserID == ignoredUserId);

        if (ignoreUser == null)
        {
            repository.Insert(new IgnoreUser { UserID = userId, IgnoredUserID = ignoredUserId });
        }
    }

    /// <summary>
    /// Gets the List of Ignored Users
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <returns>
    /// The <see cref="List"/>.
    /// </returns>
    [NotNull]
    public static List<User> IgnoredUsers(this IRepository<IgnoreUser> repository, [NotNull] int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<IgnoreUser>();

        expression.Join<User>((i, u) => u.ID == i.IgnoredUserID).Where<IgnoreUser>(u => u.UserID == userId);

        return repository.DbAccess.Execute(db => db.Connection.Select<User>(expression));
    }
}