/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The check email repository extensions.
    /// </summary>
    public static class CheckEmailRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable List(this IRepository<CheckEmail> repository, string email = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.checkemail_list(Email: email);
        }

        /// <summary>
        /// The list typed.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<CheckEmail> ListTyped(this IRepository<CheckEmail> repository, string email = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var session = repository.DbFunction.CreateSession())
            {
                return session.GetTyped<CheckEmail>(r => r.repository.DbFunction.GetData.checkemail_list(Email: email));
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        public static void Save(this IRepository<CheckEmail> repository, int? userID, [NotNull] string hash, [NotNull] string email)
        {
            CodeContracts.VerifyNotNull(hash, "hash");
            CodeContracts.VerifyNotNull(email, "email");
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.checkemail_save(UserID: userID, Hash: hash, Email: email.ToLower(), UTCTIMESTAMP: DateTime.UtcNow);
            repository.FireNew();
        }

        /// <summary>
        /// Very confusingly named function that finds a user record with associated check email and returns a user if it's found.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable Update(this IRepository<CheckEmail> repository, [NotNull] string hash)
        {
            CodeContracts.VerifyNotNull(hash, "hash");
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.checkemail_update(Hash: hash);
        }

        #endregion
    }
}