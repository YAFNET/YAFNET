namespace YAF.Editors
{
  /// <summary>
  /// IBaseEditorModule Interface for Editor classes.
  /// </summary>
  internal interface IBaseEditorModule
  {
    /// <summary>
    /// Gets a value indicating whether Active.
    /// </summary>
    bool Active
    {
      get;
    }

    /// <summary>
    /// Gets Description.
    /// </summary>
    string Description
    {
      get;
    }

    /// <summary>
    /// Gets ModuleId.
    /// </summary>
    int ModuleId
    {
      get;
    }
  }
}