namespace YAF.Core.Model
{
    using System.Data;

    using YAF.Types;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The UserPMessage Repository Extensions
    /// </summary>
    public static class UserPMessageRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The mark as read.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userPMessageId">
        /// The user p message id.
        /// </param>
        public static void MarkAsRead(this IRepository<UserPMessage> repository, [NotNull] int userPMessageId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Scalar.pmessage_markread(UserPMessageID: userPMessageId);
        }

        #endregion
    }
}