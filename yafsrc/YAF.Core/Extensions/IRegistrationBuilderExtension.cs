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
namespace YAF.Core.Extensions
{
    #region Using

    using Autofac.Builder;

    using YAF.Types;

    #endregion

    /// <summary>
    ///     The i registration builder extension.
    /// </summary>
    public static class IRegistrationBuilderExtension
    {
        #region Public Methods and Operators

        /// <summary>
        /// The owned by yaf context.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <typeparam name="TLimit">
        /// </typeparam>
        /// <typeparam name="TActivatorData">
        /// </typeparam>
        /// <typeparam name="TRegistrationStyle">
        /// </typeparam>
        /// <returns>
        /// The instance per yaf context.
        /// </returns>
        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> InstancePerYafContext
            <TLimit, TActivatorData, TRegistrationStyle>([NotNull] this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder)
        {
            CodeContracts.VerifyNotNull(builder, "builder");

            return builder.InstancePerMatchingLifetimeScope(YafLifetimeScope.Context);
        }

        #endregion
    }
}