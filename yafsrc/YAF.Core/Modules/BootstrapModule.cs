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
    using System.Reflection;

    using Autofac;
    using Autofac.Core;

    public class BootstrapModule : BaseModule
    {
        /// <summary>
        /// Bootstrap module is always called first
        /// </summary>
        public override int SortOrder
        {
            get
            {
                return 1;
            }
        }

        protected override void Load(ContainerBuilder builder)
        {
            // register all the modules in this assembly first -- excluding this module
            this.RegisterBaseModules<IModule>(
                new[] { Assembly.GetExecutingAssembly() },
                new[] { typeof(BootstrapModule) });

            // register all the modules in scanned assemblies
            this.RegisterBaseModules<IModule>(ExtensionAssemblies);
        }
    }
}