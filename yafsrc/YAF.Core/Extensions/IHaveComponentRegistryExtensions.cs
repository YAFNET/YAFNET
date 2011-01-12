namespace YAF.Core
{
  #region Using

  using Autofac;
  using Autofac.Core;

  using YAF.Types;

  #endregion

  /// <summary>
  /// The i have component registry extensions.
  /// </summary>
  public static class IHaveComponentRegistryExtensions
  {
    #region Public Methods

    /// <summary>
    /// Is not registered in the component registry.
    /// </summary>
    /// <param name="haveComponentRegistry">
    /// The have component registry.
    /// </param>
    /// <typeparam name="TRegistered">
    /// </typeparam>
    /// <returns>
    /// The is registered.
    /// </returns>
    public static bool IsNotRegistered<TRegistered>([NotNull] this IHaveComponentRegistry haveComponentRegistry)
    {
      CodeContracts.ArgumentNotNull(haveComponentRegistry, "haveComponentRegistry");

      return !haveComponentRegistry.ComponentRegistry.IsRegistered(new TypedService(typeof(TRegistered)));
    }

    /// <summary>
    /// The is registered.
    /// </summary>
    /// <param name="haveComponentRegistry">
    /// The have component registry.
    /// </param>
    /// <typeparam name="TRegistered">
    /// </typeparam>
    /// <returns>
    /// The is registered.
    /// </returns>
    public static bool IsRegistered<TRegistered>([NotNull] this IHaveComponentRegistry haveComponentRegistry)
    {
      CodeContracts.ArgumentNotNull(haveComponentRegistry, "haveComponentRegistry");

      return haveComponentRegistry.ComponentRegistry.IsRegistered(new TypedService(typeof(TRegistered)));
    }

    /// <summary>
    /// Is not registered in the component registry.
    /// </summary>
    /// <param name="haveComponentRegistry">
    /// The have component registry.
    /// </param>
    /// <typeparam name="TRegistered">
    /// </typeparam>
    /// <returns>
    /// The is registered.
    /// </returns>
    public static bool IsNotRegistered<TRegistered>([NotNull] this IHaveComponentRegistry haveComponentRegistry, string named)
    {
      CodeContracts.ArgumentNotNull(haveComponentRegistry, "haveComponentRegistry");

      return !haveComponentRegistry.ComponentRegistry.IsRegistered(new KeyedService(named, typeof(TRegistered)));
    }

    /// <summary>
    /// The is registered.
    /// </summary>
    /// <param name="haveComponentRegistry">
    /// The have component registry.
    /// </param>
    /// <typeparam name="TRegistered">
    /// </typeparam>
    /// <returns>
    /// The is registered.
    /// </returns>
    public static bool IsRegistered<TRegistered>([NotNull] this IHaveComponentRegistry haveComponentRegistry, string named)
    {
      CodeContracts.ArgumentNotNull(haveComponentRegistry, "haveComponentRegistry");

      return haveComponentRegistry.ComponentRegistry.IsRegistered(new KeyedService(named, typeof(TRegistered)));
    }

    /// <summary>
    /// The update.
    /// </summary>
    /// <param name="haveComponentRegistry">
    /// The have component registry.
    /// </param>
    /// <param name="containerBuilder">
    /// The container builder.
    /// </param>
    public static void UpdateRegistry(
      [NotNull] this IHaveComponentRegistry haveComponentRegistry, [NotNull] ContainerBuilder containerBuilder)
    {
      CodeContracts.ArgumentNotNull(haveComponentRegistry, "haveComponentRegistry");
      CodeContracts.ArgumentNotNull(containerBuilder, "containerBuilder");

      containerBuilder.Update(haveComponentRegistry.ComponentRegistry);
    }

    #endregion
  }
}