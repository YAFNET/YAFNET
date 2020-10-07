namespace YAF.Core.Model
{
    using System;

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
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        public static void AddOrUpdate(
            this IRepository<TopicReadTracking> repository,
            [NotNull] int userId,
            [NotNull] int topicId)
        {
            CodeContracts.VerifyNotNull(repository);

            var item = repository.GetSingle(x => x.TopicID == topicId && x.UserID == userId);

            if (item != null)
            {
                repository.UpdateOnly(
                    () => new TopicReadTracking { LastAccessDate = DateTime.UtcNow },
                    x => x.LastAccessDate == item.LastAccessDate && x.TopicID == topicId && x.UserID == userId);
            }
            else
            {
                repository.Insert(
                    new TopicReadTracking { UserID = userId, TopicID = topicId, LastAccessDate = DateTime.UtcNow });
            }
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Delete(this IRepository<TopicReadTracking> repository, [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            var success = repository.Delete(x => x.UserID == userId) == 1;

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
        public static DateTime? LastRead(
            this IRepository<TopicReadTracking> repository,
            [NotNull] int userId,
            [NotNull] int topicId)
        {
            CodeContracts.VerifyNotNull(repository);

            var topic = repository.GetSingle(t => t.UserID == userId && t.TopicID == topicId);

            return topic?.LastAccessDate;
        }

        #endregion
    }
}