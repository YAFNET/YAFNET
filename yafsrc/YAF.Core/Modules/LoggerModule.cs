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

namespace YAF.Core.Modules
{
    #region Using

    using System.Linq;

    using Autofac;
    using Autofac.Core;

    using YAF.Core.Services.Logger;
    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     The logging module.
    /// </summary>
    public class LoggingModule : BaseModule
    {
        #region Methods

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="builder">
        ///     The builder.
        /// </param>
        protected override void Load([NotNull] ContainerBuilder builder)
        {
            CodeContracts.VerifyNotNull(builder, "builder");

            this.ComponentRegistry.Registered += (sender, e) => e.ComponentRegistration.Preparing += OnComponentPreparing;

            if (this.IsRegistered<ILoggerProvider>())
            {
                return;
            }

            builder.RegisterType<YafDbLoggerProvider>().As<ILoggerProvider>().SingleInstance();
            builder.Register(c => c.Resolve<ILoggerProvider>().Create(null)).SingleInstance();
        }

        /// <summary>
        ///     The on component preparing.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private static void OnComponentPreparing([NotNull] object sender, [NotNull] PreparingEventArgs e)
        {
            var t = e.Component.Activator.LimitType;
            e.Parameters =
                e.Parameters.Union(
                    new[]
                    {
                        new ResolvedParameter(
                            (p, i) => p.ParameterType == typeof(ILogger),
                            (p, i) => i.Resolve<ILoggerProvider>().Create(t))
                    });
        }

        #endregion
    }
}