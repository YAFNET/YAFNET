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
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The search data cleanup module.
  /// </summary>
  public class SearchDataCleanupModule : SimpleBaseModule
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
      if (Mession.SearchData != null)
      {
        // clear it...
        Mession.SearchData.Dispose();
        Mession.SearchData = null;
      }
    }

    #endregion
  }
}