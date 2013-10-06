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
namespace YAF.Core.Data
{
    using System.Dynamic;

    using YAF.Types;

    /// <summary>
    ///     The dynamic db.
    /// </summary>
    public class TryInvokeMemberProxy : DynamicObject
    {
        #region Fields

        /// <summary>
        ///     The _try invoke func.
        /// </summary>
        private readonly TryInvokeFunc _tryInvokeFunc;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TryInvokeMemberProxy"/> class.
        /// </summary>
        /// <param name="tryInvokeFunc">
        /// The try invoke func. 
        /// </param>
        public TryInvokeMemberProxy([NotNull] TryInvokeFunc tryInvokeFunc)
        {
            CodeContracts.VerifyNotNull(tryInvokeFunc, "tryInvokeFunc");

            this._tryInvokeFunc = tryInvokeFunc;
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     The try invoke func.
        /// </summary>
        /// <param name="binder"> The binder. </param>
        /// <param name="args"> The args. </param>
        /// <param name="result"> The result. </param>
        public delegate bool TryInvokeFunc(
            [NotNull] InvokeMemberBinder binder, [NotNull] object[] args, [NotNull] out object result);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The try invoke member.
        /// </summary>
        /// <param name="binder">
        /// The binder. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <param name="result">
        /// The result. 
        /// </param>
        /// <returns>
        /// The try invoke member. 
        /// </returns>
        public override bool TryInvokeMember(
            [NotNull] InvokeMemberBinder binder, [NotNull] object[] args, [NotNull] out object result)
        {
            return this._tryInvokeFunc(binder, args, out result);
        }

        #endregion
    }
}