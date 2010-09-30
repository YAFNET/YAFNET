namespace YAF.Classes.Data
{
  /// <summary>
  /// The search condition.
  /// </summary>
  public class SearchCondition
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "SearchCondition" /> class.
    /// </summary>
    public SearchCondition()
    {
      this.ConditionType = SearchConditionType.AND;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets Condition.
    /// </summary>
    public string Condition { get; set; }

    /// <summary>
    ///   Gets or sets ConditionType.
    /// </summary>
    public SearchConditionType ConditionType { get; set; }

    #endregion
  }
}