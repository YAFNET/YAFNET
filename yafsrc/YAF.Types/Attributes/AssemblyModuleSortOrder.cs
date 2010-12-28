namespace YAF.Types.Attributes
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// The assembly sort order -- sorts the assembly load order in the modules.
  /// </summary>
  [AttributeUsage(AttributeTargets.Assembly)]
  public class AssemblyModuleSortOrder : Attribute
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyLoadOrder"/> class.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    public AssemblyModuleSortOrder(int value)
    {
      this.SortOrder = value;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets SortOrder.
    /// </summary>
    public int SortOrder { get; protected set; }

    #endregion
  }
}