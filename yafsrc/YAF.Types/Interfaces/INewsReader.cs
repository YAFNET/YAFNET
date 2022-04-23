namespace YAF.Types.Interfaces;

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
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="lastUpdate">
    /// The last update.
    /// </param>
    /// <param name="timeToRun">
    /// The time to run.
    /// </param>
    /// <param name="createUsers">
    /// The create users.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    int ReadArticles([NotNull] int boardId, int lastUpdate, int timeToRun, bool createUsers);

    #endregion
}