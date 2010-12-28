// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchDataCleanupModule.cs" company="">
//   
// </copyright>
// <summary>
//   The search data cleanup module.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Modules
{
  #region Using

  using System;

  using YAF.Classes;
  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Core.Services;
  using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The search data cleanup module.
  /// </summary>
  public class SearchDataCleanupForumModule : SimpleBaseForumModule
  {
    #region Public Methods

    /// <summary>
    /// The init before page.
    /// </summary>
    public override void InitBeforePage()
    {
      this.PageContext.PagePreLoad += this.PageContext_PagePreLoad;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The page context_ page pre load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void PageContext_PagePreLoad([NotNull] object sender, [NotNull] EventArgs e)
    {
      // no security features for login/logout pages
      if (this.ForumPageType == ForumPages.search)
      {
        return;
      }

      // clear out any search data in the session.... just in case...
      if (PageContext.Get<IYafSession>().SearchData != null)
      {
        // clear it...
        PageContext.Get<IYafSession>().SearchData.Dispose();
        PageContext.Get<IYafSession>().SearchData = null;
      }
    }

    #endregion
  }
}