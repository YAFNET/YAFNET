namespace YAF.Classes.Core
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Web;

  using Autofac;
  using Autofac.Core;

  using Moq;

  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  using Module = Autofac.Module;

  #endregion

  /// <summary>
  /// The yaf lifetime scope.
  /// </summary>
  public enum YafLifetimeScope
  {
    /// <summary>
    ///   The root.
    /// </summary>
    Root, 

    /// <summary>
    ///   The context.
    /// </summary>
    Context
  }

  /// <summary>
  /// The module for all singleton scoped items...
  /// </summary>
  public class YafBaseContainerModule : Module
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
      this.RegisterBasicBindings(builder);

      this.RegisterWebAbstractions(builder);

      this.RegisterServices(builder);

      this.RegisterExternalModules(builder);
    }

    /// <summary>
    /// The get assembly sort order.
    /// </summary>
    /// <param name="a">
    /// The a.
    /// </param>
    /// <returns>
    /// The get assembly sort order.
    /// </returns>
    private int GetAssemblySortOrder([NotNull] Assembly a)
    {
      var attribute = a.GetCustomAttributes(typeof(AssemblyModuleSortOrder), true).OfType<AssemblyModuleSortOrder>();

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

      builder.RegisterType<YafStopWatch>().As<IStopWatch>().InstancePerMatchingLifetimeScope(YafLifetimeScope.Context);
      builder.RegisterType<LocalizationHandler>().As<ILocalizationHandler>().InstancePerMatchingLifetimeScope(
        YafLifetimeScope.Context);
      builder.RegisterType<ThemeHandler>().As<IThemeHandler>().InstancePerMatchingLifetimeScope(
YafLifetimeScope.Context);
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

      // make sure we don't include this assembly -- for obvious reasons ;)
      moduleList.Remove(Assembly.GetExecutingAssembly());

      // little bit of filtering...
      moduleList.OrderByDescending(this.GetAssemblySortOrder);

      // TODO: create real abstracted plugin model. This is a stop-gap.
      var modules = moduleList.FindModules<IModule>();

      foreach (var moduleInstance in modules)
      {
        builder.RegisterModule(Activator.CreateInstance(moduleInstance) as IModule);
      }
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

      builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(
        t => t.GetInterfaces().Contains(typeof(IStartupService))).As<IStartupService>().InstancePerMatchingLifetimeScope
        (YafLifetimeScope.Context);

      builder.Register(
        x =>
        x.Resolve<IEnumerable<IStartupService>>().Where(t => t is StartupInitializeDb).FirstOrDefault() as
        StartupInitializeDb).InstancePerMatchingLifetimeScope(YafLifetimeScope.Context);
    }

    /// <summary>
    /// The register web abstractions.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private void RegisterWebAbstractions([NotNull] ContainerBuilder builder)
    {
      CodeContracts.ArgumentNotNull(builder, "builder");

      builder.Register(
        k =>
        HttpContext.Current != null ? new HttpContextWrapper(HttpContext.Current) : new Mock<HttpContextBase>().Object).
        InstancePerMatchingLifetimeScope(YafLifetimeScope.Context);

      builder.Register(
        k =>
        HttpContext.Current != null
          ? new HttpSessionStateWrapper(HttpContext.Current.Session)
          : new Mock<HttpSessionStateBase>().Object).InstancePerMatchingLifetimeScope(YafLifetimeScope.Context);

      builder.Register(
        k =>
        HttpContext.Current != null
          ? new HttpRequestWrapper(HttpContext.Current.Request)
          : new Mock<HttpRequestBase>().Object).InstancePerMatchingLifetimeScope(YafLifetimeScope.Context);

      builder.Register(
        k =>
        HttpContext.Current != null
          ? new HttpResponseWrapper(HttpContext.Current.Response)
          : new Mock<HttpResponseBase>().Object).InstancePerMatchingLifetimeScope(YafLifetimeScope.Context);

      builder.Register(
        k =>
        HttpContext.Current != null
          ? new HttpServerUtilityWrapper(HttpContext.Current.Server)
          : new Mock<HttpServerUtilityBase>().Object).InstancePerMatchingLifetimeScope(YafLifetimeScope.Context);

      builder.Register(
        k =>
        HttpContext.Current != null
          ? new HttpApplicationStateWrapper(HttpContext.Current.Application)
          : new Mock<HttpApplicationStateBase>().Object).InstancePerMatchingLifetimeScope(YafLifetimeScope.Context);
    }

    #endregion
  }
}