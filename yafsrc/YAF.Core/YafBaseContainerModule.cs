/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

  #endregion

  /// <summary>
  /// The module for all singleton scoped items...
  /// </summary>
  public class YafBaseContainerModule : IModule, IHaveComponentRegistry
  {
    #region Properties

    /// <summary>
    /// Gets or sets ComponentRegistry.
    /// </summary>
    public IComponentRegistry ComponentRegistry { get; set; }

    /// <summary>
    /// Gets or sets ExtensionAssemblies.
    /// </summary>
    public IList<Assembly> ExtensionAssemblies { get; protected set; }

    #endregion

    #region Implemented Interfaces

    #region IModule

    /// <summary>
    /// Apply the module to the component registry.
    /// </summary>
    /// <param name="componentRegistry">
    /// Component registry to apply configuration to.
    /// </param>
    public void Configure([NotNull] IComponentRegistry componentRegistry)
    {
      CodeContracts.ArgumentNotNull(componentRegistry, "componentRegistry");

      this.ComponentRegistry = componentRegistry;

      this.ExtensionAssemblies =
        new YafModuleScanner().GetModules("YAF*.dll").OrderByDescending(x => x.GetAssemblySortOrder()).ToList();

      // handle registration...
      this.RegisterExternalModules();
      this.RegisterBasicBindings();
      this.RegisterEventBindings();
      this.RegisterServices();
      this.RegisterModules();
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The register basic bindings.
    /// </summary>
    private void RegisterBasicBindings()
    {
      var builder = new ContainerBuilder();

      builder.RegisterType<AutoFacServiceLocatorProvider>().AsSelf().As<IServiceLocator>().As<IInjectServices>().
        OwnedByLifetimeScope();

      // optional defaults.
      if (this.IsNotRegistered<ISendMail>())
      {
        builder.RegisterType<YafSendMail>().As<ISendMail>().SingleInstance();
      }

      if (this.IsNotRegistered<ISendNotification>())
      {
        builder.RegisterType<YafSendNotification>().As<ISendNotification>().OwnedByLifetimeScope();
      }

      if (this.IsNotRegistered<IDigest>())
      {
        builder.RegisterType<YafDigest>().As<IDigest>().OwnedByLifetimeScope();
      }

      if (this.IsNotRegistered<IUserDisplayName>())
      {
        builder.RegisterType<DefaultUserDisplayName>().As<IUserDisplayName>().OwnedByLifetimeScope();
      }

      if (this.IsNotRegistered<IUrlBuilder>())
      {
        builder.RegisterType<DefaultUrlBuilder>().As<IUrlBuilder>().OwnedByLifetimeScope();
      }

      // staionary bindings...
      builder.RegisterType<YafSession>().As<IYafSession>().OwnedByLifetimeScope();
      builder.RegisterType<YafDBBroker>().As<IDBBroker>().OwnedByLifetimeScope();
      builder.RegisterType<YafBadWordReplace>().As<IBadWordReplace>().OwnedByLifetimeScope();
      builder.RegisterType<YafPermissions>().As<IPermissions>().OwnedByLifetimeScope();
      builder.RegisterType<YafDateTime>().As<IDateTime>().OwnedByLifetimeScope();
      builder.RegisterType<YafAvatars>().As<IAvatars>().OwnedByLifetimeScope();
      builder.RegisterType<YafFavoriteTopic>().As<IFavoriteTopic>().OwnedByLifetimeScope();
      builder.RegisterType<YafUserIgnored>().As<IUserIgnored>().OwnedByLifetimeScope();

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

      // background task.
      builder.RegisterType<YafSendMailThreaded>().AsSelf().SingleInstance();

      this.UpdateRegistry(builder);
    }

    /// <summary>
    /// Register event bindings
    /// </summary>
    private void RegisterEventBindings()
    {
      var builder = new ContainerBuilder();

      builder.RegisterType<ServiceLocatorEventRaiser>().As<IRaiseEvent>().InstancePerLifetimeScope();
      builder.RegisterGeneric(typeof(FireEvent<>)).As(typeof(IFireEvent<>)).InstancePerLifetimeScope();

      // scan assemblies for events to wire up...
      builder.RegisterAssemblyTypes(this.ExtensionAssemblies.ToArray()).AsClosedTypesOf(typeof(IHandleEvent<>)).
        AsImplementedInterfaces().InstancePerLifetimeScope();

      this.UpdateRegistry(builder);
    }

    /// <summary>
    /// The register external modules.
    /// </summary>
    private void RegisterExternalModules()
    {
      var builder = new ContainerBuilder();

      var modules =
        this.ExtensionAssemblies.Where(a => a != Assembly.GetExecutingAssembly()).FindModules<IModule>().Select(
          m => Activator.CreateInstance(m) as IModule);

      modules.ForEach(m => builder.RegisterModule(m));

      this.UpdateRegistry(builder);
    }

    /// <summary>
    /// The register modules.
    /// </summary>
    private void RegisterModules()
    {
      var builder = new ContainerBuilder();

      // forum modules...
      builder.RegisterAssemblyTypes(this.ExtensionAssemblies.ToArray()).AssignableTo<IBaseForumModule>().As
        <IBaseForumModule>().InstancePerLifetimeScope();

      // editor modules...
      builder.RegisterAssemblyTypes(this.ExtensionAssemblies.ToArray()).AssignableTo<ForumEditor>().As<ForumEditor>().
        InstancePerLifetimeScope();

      this.UpdateRegistry(builder);
    }

    /// <summary>
    /// The register services.
    /// </summary>
    private void RegisterServices()
    {
      var builder = new ContainerBuilder();

      builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AssignableTo<IStartupService>().As<IStartupService>
        ().InstancePerLifetimeScope();

      builder.Register(
        x =>
        x.Resolve<IEnumerable<IStartupService>>().Where(t => t is StartupInitializeDb).FirstOrDefault() as
        StartupInitializeDb).InstancePerLifetimeScope();

      this.UpdateRegistry(builder);
    }

    #endregion
  }
}