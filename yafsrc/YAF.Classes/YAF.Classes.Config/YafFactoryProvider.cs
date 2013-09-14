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
namespace YAF.Classes
{
    #region Using

    using YAF.Classes.Pattern;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The YAF Factory provider.
    /// </summary>
    public static class YafFactoryProvider
    {
        #region Constants and Fields

        /// <summary>
        /// The _builder factory.
        /// </summary>
        private static ITypeFactoryInstance<IUrlBuilder> _builderFactory;

        /// <summary>
        /// The _user display name factory.
        /// </summary>
        private static ITypeFactoryInstance<IUserDisplayName> _userDisplayNameFactory;

        #endregion

        #region Properties

        /// <summary>
        /// Gets UrlBuilder.
        /// </summary>
        public static IUrlBuilder UrlBuilder
        {
            get
            {
                if (_builderFactory == null)
                {
                    _builderFactory = new TypeFactoryInstanceApplicationBoardScope<IUrlBuilder>(UrlBuilderType);
                }

                return _builderFactory.Get();
            }
        }

        /// <summary>
        /// Gets current <see cref="IUserDisplayName"/>.
        /// </summary>
        public static IUserDisplayName UserDisplayName
        {
            get
            {
                if (_userDisplayNameFactory == null)
                {
                    _userDisplayNameFactory =
                        new TypeFactoryInstanceApplicationBoardScope<IUserDisplayName>(UserDisplayNameType);
                }

                return _userDisplayNameFactory.Get();
            }
        }

        /// <summary>
        /// Gets UrlBuilderType.
        /// </summary>
        private static string UrlBuilderType
        {
            get
            {
                var urlAssembly = Config.GetProvider("UrlBuilder");

                if (urlAssembly.IsSet())
                {
                    return urlAssembly;
                }

                if (Config.IsDotNetNuke)
                {
                    urlAssembly = "YAF.DotNetNuke.DotNetNukeUrlBuilder,YAF.DotNetNuke.Module";
                }
                else if (Config.IsMojoPortal)
                {
                    urlAssembly = "YAF.Mojo.MojoPortalUrlBuilder,YAF.Mojo";
                }
                else if (Config.IsRainbow)
                {
                    urlAssembly = "yaf_rainbow.RainbowUrlBuilder,yaf_rainbow";
                }
                else if (Config.IsPortal)
                {
                    urlAssembly = "Portal.UrlBuilder,Portal";
                }
                else if (Config.IsPortalomatic)
                {
                    urlAssembly = "Portalomatic.NET.Utils.URLBuilder,Portalomatic.NET.Utils";
                }
                else if (Config.EnableURLRewriting)
                {
                    switch (Config.UrlRewritingFormat)
                    {
                        case "advanced":
                            urlAssembly = "YAF.Core.URLBuilder.AdvancedUrlRewriter,YAF.Core";
                            break;
                        case "standard":
                            urlAssembly = "YAF.Core.URLBuilder.StandardUrlRewriter,YAF.Core";
                            break;
                        default:
                            urlAssembly = "YAF.Core.URLBuilder.StandardUrlRewriter,YAF.Core";
                            break;
                    }
                }
                else
                {
                    urlAssembly = "YAF.Classes.DefaultUrlBuilder";
                }

                return urlAssembly;
            }
        }

        /// <summary>
        /// Gets UserDisplayNameType.
        /// </summary>
        private static string UserDisplayNameType
        {
            get
            {
                return Config.GetProvider("UserDisplayName").IsSet()
                           ? Config.GetProvider("UserDisplayName")
                           : "YAF.Classes.Core.DefaultUserDisplayName,YAF.Classes.Core";
            }
        }

        #endregion
    }
}