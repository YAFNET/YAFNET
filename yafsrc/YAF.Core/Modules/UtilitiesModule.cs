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
namespace YAF.Utils
{
    #region Using

    using System.Web;

    using Autofac;

    using YAF.Core;
    using YAF.Core.Data.Profiling;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     The utilities module.
    /// </summary>
    public class UtilitiesModule : BaseModule
    {
        #region Methods

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        protected override void Load([NotNull] ContainerBuilder builder)
        {
            builder.RegisterType<QueryProfile>().As<IProfileQuery>().SingleInstance().PreserveExistingDefaults();

            this.RegisterWebAbstractions(builder);
        }

        /// <summary>
        /// The register web abstractions.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        private void RegisterWebAbstractions([NotNull] ContainerBuilder builder)
        {
            CodeContracts.VerifyNotNull(builder, "builder");

            builder.Register(c => new HttpContextWrapper(HttpContext.Current)).As<HttpContextBase>().InstancePerYafContext();
            builder.Register(c => c.Resolve<HttpContextBase>().Request).As<HttpRequestBase>().InstancePerYafContext();
            builder.Register(c => c.Resolve<HttpContextBase>().Response).As<HttpResponseBase>().InstancePerYafContext();
            builder.Register(c => c.Resolve<HttpContextBase>().Server).As<HttpServerUtilityBase>().InstancePerYafContext();
            builder.Register(c => c.Resolve<HttpContextBase>().Session).As<HttpSessionStateBase>().InstancePerYafContext();
        }

        #endregion
    }
}