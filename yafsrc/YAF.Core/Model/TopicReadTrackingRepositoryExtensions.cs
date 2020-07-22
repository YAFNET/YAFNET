namespace YAF.Core.Model
{
    using System;
    using System.Linq;

    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The TopicRead Repository Extensions
    /// </summary>
    public static class TopicReadTrackingRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add or update.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        public static void AddOrUpdate(this IRepository<TopicReadTracking> repository, int userID, int topicID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.readtopic_addorupdate(UserID: userID, TopicID: topicID, UTCTIMESTAMP: DateTime.UtcNow);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Delete(this IRepository<TopicReadTracking> repository, int userID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var success = repository.DbAccess.Execute(db => db.Connection.Delete<TopicReadTracking>(x => x.UserID == userID)) == 1;

            if (success)
            {
                repository.FireDeleted();
            }

            return success;
        }

        /// <summary>
        /// The last read.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime?"/>.
        /// </returns>
        public static DateTime? LastRead(this IRepository<TopicReadTracking> repository, int userId, int topicId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var topic = repository.Get(t => t.UserID == userId && t.TopicID == topicId);

            return topic.FirstOrDefault()?.LastAccessDate;
        }

        #endregion
    }
}