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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;

  using Autofac;
  using Autofac.Core;

  using YAF.Classes;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  using Module = Autofac.Module;

  #endregion

  /// <summary>
  /// The module for all singleton scoped items...
  /// </summary>
  public class YafBaseContainerModule : Module
  {
    private List<IModule> _externalModules = new List<IModule>();

    #region Methods

    /// <summary>
    /// The load.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    protected override void Load([NotNull] ContainerBuilder builder)
    {
      CodeContracts.ArgumentNotNull(builder, "builder");

      this.RegisterBasicBindings(builder);

      this.RegisterEventBindings(builder);

      this.RegisterServices(builder);

      this.RegisterModules(builder);

      this.RegisterExternalModules(builder);
    }

    /// <summary>
    /// The get assembly sort order.
    /// </summary>
    /// <param name="assembly">
    /// The assembly.
    /// </param>
    /// <returns>
    /// The get assembly sort order.
    /// </returns>
    private int GetAssemblySortOrder([NotNull] Assembly assembly)
    {
      CodeContracts.ArgumentNotNull(assembly, "assembly");

      var attribute = assembly.GetCustomAttributes(typeof(AssemblyModuleSortOrder), true).OfType<AssemblyModuleSortOrder>();

      return attribute.Any() ? attribute.First().SortOrder : 9999;
    }

    /// <summary>
    /// The register basic bindings.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private void RegisterBasicBindings([NotNull] ContainerBuilder builder)
    {
      CodeContracts.ArgumentNotNull(builder, "builder");

      builder.RegisterType<AutoFacServiceLocatorProvider>().AsSelf().As<IServiceLocator>().As<IInjectServices>().
        OwnedByLifetimeScope();

      // the scopes will probably change
      builder.RegisterType<YafSendMail>().As<ISendMail>().OwnedByLifetimeScope();
      builder.RegisterType<YafSession>().As<IYafSession>().OwnedByLifetimeScope();
      builder.RegisterType<YafDBBroker>().As<IDBBroker>().OwnedByLifetimeScope();
      builder.RegisterType<YafBadWordReplace>().As<IBadWordReplace>().OwnedByLifetimeScope();
      builder.RegisterType<YafPermissions>().As<IPermissions>().OwnedByLifetimeScope();
      builder.RegisterType<YafDateTime>().As<IDateTime>().OwnedByLifetimeScope();
      builder.RegisterType<YafAvatars>().As<IAvatars>().OwnedByLifetimeScope();
      builder.RegisterType<YafFavoriteTopic>().As<IFavoriteTopic>().OwnedByLifetimeScope();
      builder.RegisterType<YafUserIgnored>().As<IUserIgnored>().OwnedByLifetimeScope();
      builder.RegisterType<YafSendNotification>().As<ISendNotification>().OwnedByLifetimeScope();
      builder.RegisterType<YafDigest>().As<IDigest>().OwnedByLifetimeScope();

      builder.RegisterType<DefaultUserDisplayName>().As<IUserDisplayName>().OwnedByLifetimeScope();

      builder.RegisterType<DefaultUrlBuilder>().As<IUrlBuilder>().OwnedByLifetimeScope();

      builder.RegisterType<JavaScriptBuilder>().As<IScriptBuilder>().OwnedByLifetimeScope();

      builder.RegisterType<RewriteUrlBuilder>().Named<IUrlBuilder>("rewriter").OwnedByLifetimeScope();

      builder.RegisterType<YafStopWatch>().As<IStopWatch>().InstancePerLifetimeScope();

      // localization registration...
      builder.RegisterType<LocalizationProvider>().InstancePerLifetimeScope();
      builder.Register(k => k.Resolve<LocalizationProvider>().Localization);

      // theme registration...
      builder.RegisterType<ThemeProvider>().InstancePerLifetimeScope();
      builder.Register(k => k.Resolve<ThemeProvider>().Theme);

      // module resolution bindings...
      builder.RegisterGeneric(typeof(StandardModuleManager<>)).As(typeof(IModuleManager<>)).InstancePerLifetimeScope();
    }

    /// <summary>
    /// Register event bindings
    /// </summary>
    /// <param name="builder">
    /// </param>
    private void RegisterEventBindings([NotNull] ContainerBuilder builder)
    {
      builder.RegisterType<ServiceLocatorEventRaiser>().As<IRaiseEvent>().InstancePerLifetimeScope();
      builder.RegisterGeneric(typeof(FireEvent<>)).As(typeof(IFireEvent<>)).InstancePerLifetimeScope();

      // scan assemblies for events to wire up...
      var assemblies =
        AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.IsSet() && a.FullName.ToLower().StartsWith("yaf"))
          .ToArray();

      builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IHandleEvent<>)).AsImplementedInterfaces().
        InstancePerLifetimeScope();
    }

    /// <summary>
    /// The register external modules.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private void RegisterExternalModules([NotNull] ContainerBuilder builder)
    {
      CodeContracts.ArgumentNotNull(builder, "builder");

      var moduleList =
        AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.IsSet() && a.FullName.ToLower().StartsWith("yaf"))
          .ToList();

      // make sure we don't include this assembly -- otherwise we'll have a recusive situation.
      moduleList.Remove(Assembly.GetExecutingAssembly());

      // little bit of filtering...
      moduleList.OrderByDescending(this.GetAssemblySortOrder);

      // TODO: create real abstracted plugin model. This is a stop-gap.
      var modules = moduleList.FindModules<IModule>();

      // create module instances...
      modules.ForEach(mi => this._externalModules.Add(Activator.CreateInstance(mi) as IModule));

      this._externalModules.ForEach(m => builder.RegisterModule(m));
    }

    /// <summary>
    /// The register modules.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private void RegisterModules([NotNull] ContainerBuilder builder)
    {
      CodeContracts.ArgumentNotNull(builder, "builder");

      var assemblies =
        AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.IsSet() && a.FullName.ToLower().StartsWith("yaf"))
          .ToArray();

      // forum modules...
      builder.RegisterAssemblyTypes(assemblies).AssignableTo<IBaseForumModule>().As<IBaseForumModule>().
        InstancePerLifetimeScope();

      // editor modules...
      builder.RegisterAssemblyTypes(assemblies).AssignableTo<ForumEditor>().As<ForumEditor>().InstancePerLifetimeScope();
    }

    /// <summary>
    /// The register services.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private void RegisterServices([NotNull] ContainerBuilder builder)
    {
      CodeContracts.ArgumentNotNull(builder, "builder");

      builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AssignableTo<IStartupService>().As<IStartupService>
        ().InstancePerLifetimeScope();

      builder.Register(
        x =>
        x.Resolve<IEnumerable<IStartupService>>().Where(t => t is StartupInitializeDb).FirstOrDefault() as
        StartupInitializeDb).InstancePerLifetimeScope();
    }

    #endregion
  }
}