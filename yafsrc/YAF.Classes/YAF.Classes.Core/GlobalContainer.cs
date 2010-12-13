namespace YAF.Classes.Core
{
  #region Using

  using Autofac;

  #endregion

  /// <summary>
  /// Hack allowing extensions to have access to a kernel instance...
  /// </summary>
  public static class GlobalContainer
  {
    #region Constants and Fields

    /// <summary>
    /// The _sync object.
    /// </summary>
    private static readonly object _syncObject = new object();

    /// <summary>
    ///   The _container.
    /// </summary>
    private static IContainer _container;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Container.
    /// </summary>
    public static IContainer Container
    {
      get
      {
        if (_container == null)
        {
          lock (_syncObject)
          {
            if (_container == null)
            {
              _container = CreateContainer();
            }
          }
        }

        return _container;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The create container.
    /// </summary>
    /// <param name="httpApplication">
    /// The http application.
    /// </param>
    private static IContainer CreateContainer()
    {
      var builder = new ContainerBuilder();
      builder.RegisterModule(new YafBaseContainerModule());
      return builder.Build();
    }

    #endregion
  }
}