namespace YAF.Core.Model
{
    using System;

    using ServiceStack.OrmLite;

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    public static class TopicReadTrackingRepositoryExtensions
    {
        #region Public Methods and Operators

        public static void AddOrUpdate(this IRepository<TopicReadTracking> repository, int userID, int topicID)
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            repository.DbFunction.Query.readtopic_addorupdate(UserID: userID, TopicID: topicID, UTCTIMESTAMP: DateTime.UtcNow);
        }

        public static bool Delete(this IRepository<TopicReadTracking> repository, int userID)
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            var success = repository.DbAccess.Execute(db => db.Delete<TopicReadTracking>(x => x.UserID == userID)) == 1;

            if (success)
            {
                repository.FireDeleted();
            }

            return success;
        }

        public static DateTime? Lastread(this IRepository<TopicReadTracking> repository, int userID, int topicID)
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            return repository.DbFunction.Scalar.readtopic_lastread(UserID: userID, TopicID: topicID);
        }

        #endregion
    }
}