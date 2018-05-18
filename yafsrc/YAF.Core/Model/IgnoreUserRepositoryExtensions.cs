namespace YAF.Core.Model
{
    using ServiceStack.OrmLite;

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The IgnoreUser Repository Extensions
    /// </summary>
    public static class IgnoreUsergRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Deletes the specified user identifier.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ignoreUserId">The ignore user identifier.</param>
        /// <returns></returns>
        public static bool Delete(this IRepository<IgnoreUser> repository, int userId, int ignoreUserId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var success = repository.DbAccess.Execute(
                              db => db.Connection.Delete<IgnoreUser>(x => x.UserID == userId && x.IgnoredUserID == ignoreUserId))
                          == 1;

            if (success)
            {
                repository.FireDeleted();
            }

            return success;
        }

        #endregion
    }
}