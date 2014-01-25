namespace YAF.Types.Interfaces
{
  using YAF.Types;

  /// <summary>
  /// The i newsgroup.
  /// </summary>
  public interface INewsreader
  {
    #region Public Methods

    /// <summary>
    /// The read articles.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="lastUpdate">
    /// The n last update.
    /// </param>
    /// <param name="timeToRun">
    /// The n time to run.
    /// </param>
    /// <param name="createUsers">
    /// The b create users.
    /// </param>
    /// <returns>
    /// The read articles.
    /// </returns>
    int ReadArticles([NotNull] int boardID, int lastUpdate, int timeToRun, bool createUsers);

    #endregion
  }
}