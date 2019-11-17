namespace YAF.Core.Model
{
    using System.Data;

    using YAF.Types;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The UserPMessage Repository Extensions
    /// </summary>
    public static class UserPMessageRepositoryExtensions
    {
        #region Public Methods and Operators

        public static void MarkAsRead(this IRepository<UserPMessage> repository, [NotNull] int userPMessageId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Scalar.pmessage_markread(UserPMessageID: userPMessageId);
        }


        public static DataTable InfoAsDataTable(this IRepository<UserPMessage> repository)
        {
            return repository.DbFunction.GetAsDataTable(cdb => cdb.pmessage_info());
        }

        #endregion
    }
}