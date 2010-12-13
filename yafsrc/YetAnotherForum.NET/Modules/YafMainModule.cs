namespace YAF.Modules
{
  #region Using

  using Autofac;

  using YAF.Classes.Pattern;

  #endregion

  /// <summary>
  /// The yaf main module.
  /// </summary>
  public class YafMainModule : Module
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
    }

    #endregion
  }
}