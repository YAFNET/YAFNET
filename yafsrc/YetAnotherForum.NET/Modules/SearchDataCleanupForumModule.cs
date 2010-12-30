// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchDataCleanupModule.cs" company="">
// </copyright>
// <summary>
//   The search data cleanup module.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Modules
{
  #region Using

  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The search data cleanup module.
  /// </summary>
  public class SearchDataCleanupForumModule : SimpleBaseForumModule
  {
    #region Constants and Fields

    /// <summary>
    /// The _page pre load.
    /// </summary>
    private readonly IFireEvent<ForumPagePreLoadEvent> _pagePreLoad;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchDataCleanupForumModule"/> class.
    /// </summary>
    /// <param name="pagePreLoad">
    /// The page pre load.
    /// </param>
    public SearchDataCleanupForumModule([NotNull] IFireEvent<ForumPagePreLoadEvent> pagePreLoad)
    {
      this._pagePreLoad = pagePreLoad;

      this._pagePreLoad.HandleEvent += this._pagePreLoad_HandleEvent;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The _page pre load_ handle event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void _pagePreLoad_HandleEvent([NotNull] object sender, [NotNull] EventConverterArgs<ForumPagePreLoadEvent> e)
    {
      // no security features for login/logout pages
      if (this.ForumPageType == ForumPages.search)
      {
        return;
      }

      // clear out any search data in the session.... just in case...
      if (this.Get<IYafSession>().SearchData != null)
      {
        // clear it...
        this.Get<IYafSession>().SearchData.Dispose();
        this.Get<IYafSession>().SearchData = null;
      }
    }

    #endregion
  }
}