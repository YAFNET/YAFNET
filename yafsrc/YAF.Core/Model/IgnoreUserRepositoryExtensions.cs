namespace YAF.Core.Model
{
    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The IgnoreUser Repository Extensions
    /// </summary>
    public static class IgnoreUserRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Deletes the specified user identifier.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ignoreUserId">The ignore user identifier.</param>
        /// <returns>Returns if deleting was successfully or not</returns>
        public static bool Delete(this IRepository<IgnoreUser> repository, int userId, int ignoreUserId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var success = repository.DbAccess.Execute(
                              db => db.Connection.Delete<IgnoreUser>(
                                  x => x.UserID == userId && x.IgnoredUserID == ignoreUserId)) == 1;

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
            CodeContracts.VerifyNotNull(repository, "repository");

            var ignoreUser = repository.GetSingle(i => i.UserID == userId && i.IgnoredUserID == ignoredUserId);

            if (ignoreUser == null)
            {
                repository.Insert(
                    new IgnoreUser
                        {
                            UserID = userId,
                            IgnoredUserID = ignoredUserId
                        });
            }
        }

        #endregion
    }
} 