/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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

  using Autofac;

  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The utilities module.
  /// </summary>
  public class UtilitiesModule : Module
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
      builder.RegisterType<HttpRequestIsSecure>().As<IRequestSecure>().InstancePerLifetimeScope();
      builder.RegisterType<StyleTransform>().As<IStyleTransform>().InstancePerLifetimeScope();
      builder.RegisterType<SimpleBaseContext>().As<IBaseContext>().InstancePerLifetimeScope();
    }

    #endregion
  }
}