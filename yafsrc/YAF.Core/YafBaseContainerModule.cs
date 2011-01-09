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
  using YAF.Types.Interfaces;
  using YAF.Utils;

  using Module = Autofac.Module;

  #endregion

  /// <summary>
  /// The module for all singleton scoped items...
  /// </summary>
  public class YafBaseContainerModule : Module
  {
    public IList<Assembly> ExtensionAssemblies { get; protected set; }

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

      this.ExtensionAssemblies =
        new YafModuleScanner().GetModules("YAF*.dll").OrderByDescending(x => x.GetAssemblySortOrder()).ToList();

      this.RegisterBasicBindings(builder);

      this.RegisterEventBindings(builder);

      this.RegisterServices(builder);

      this.RegisterModules(builder);

      this.RegisterExternalModules(builder);
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

      // needs to be "instance per dependancy" so that each new request gets a new ScripBuilder.
      builder.RegisterType<JavaScriptBuilder>().As<IScriptBuilder>().InstancePerDependency();

      builder.RegisterType<RewriteUrlBuilder>().Named<IUrlBuilder>("rewriter").OwnedByLifetimeScope();

      builder.RegisterType<YafStopWatch>().As<IStopWatch>().InstancePerMatchingLifetimeScope(YafLifetimeScope.Context);

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
      builder.RegisterAssemblyTypes(ExtensionAssemblies.ToArray()).AsClosedTypesOf(typeof(IHandleEvent<>)).
        AsImplementedInterfaces().InstancePerLifetimeScope();
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

      // forum modules...
      builder.RegisterAssemblyTypes(ExtensionAssemblies.ToArray()).AssignableTo<IBaseForumModule>().As<IBaseForumModule>().
        InstancePerLifetimeScope();

      // editor modules...
      builder.RegisterAssemblyTypes(ExtensionAssemblies.ToArray()).AssignableTo<ForumEditor>().As<ForumEditor>().InstancePerLifetimeScope();
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

    /// <summary>
    /// The register external modules.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private void RegisterExternalModules([NotNull] ContainerBuilder builder)
    {
      CodeContracts.ArgumentNotNull(builder, "builder");

      var modules =
        this.ExtensionAssemblies.Where(a => a != Assembly.GetExecutingAssembly()).FindModules<IModule>().Select(
          m => Activator.CreateInstance(m) as IModule);

      modules.ForEach(m => builder.RegisterModule(m));
    }

    #endregion
  }
}