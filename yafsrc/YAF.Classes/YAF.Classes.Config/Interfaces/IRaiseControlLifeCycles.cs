namespace YAF.Classes.Interfaces
{
  /// <summary>
  /// The i raise control life cycles.
  /// </summary>
  public interface IRaiseControlLifeCycles
  {
    #region Public Methods

    /// <summary>
    /// The raise init.
    /// </summary>
    void RaiseInit();

    /// <summary>
    /// The raise load.
    /// </summary>
    void RaiseLoad();

    /// <summary>
    /// The raise pre render.
    /// </summary>
    void RaisePreRender();

    #endregion
  }
}