namespace YAF.Core.Context
{
    using System;

    using YAF.Types.Models.Identity;

    /// <summary>
    /// The identity db context.
    /// </summary>
    public class IdentityDbContext : IdentityDbContext<AspNetUsers<string>, AspNetRoles<string>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContext"/> class.
        /// </summary>
        public IdentityDbContext()
            : base()
        {
        }
    }

    /// <summary>
    /// The identity db context.
    /// </summary>
    /// <typeparam name="TUser">
    /// </typeparam>
    /// <typeparam name="TRole">
    /// </typeparam>
    public class IdentityDbContext<TUser, TRole> : IdentityDbContext<TUser, TRole, string, string>
        where TUser : AspNetUsers<string>
        where TRole : AspNetRoles<string>
    {
        public IdentityDbContext() : base()
        {
        }
    }

    /// <summary>
    /// The identity db context.
    /// </summary>
    /// <typeparam name="TUser">
    /// </typeparam>
    /// <typeparam name="TRole">
    /// </typeparam>
    /// <typeparam name="TKey">
    /// </typeparam>
    /// <typeparam name="TRoleKey">
    /// </typeparam>
    public class IdentityDbContext<TUser, TRole, TKey, TRoleKey> : IDisposable
        where TUser : AspNetUsers<TKey>
        where TRole : AspNetRoles<TRoleKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContext{TUser,TRole,TKey,TRoleKey}"/> class.
        /// </summary>
        public IdentityDbContext()
        {
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
