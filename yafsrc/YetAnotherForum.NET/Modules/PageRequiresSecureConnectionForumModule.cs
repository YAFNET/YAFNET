namespace YAF.Modules
{
  #region Using

  using System;
  using System.Web;

  using YAF.Classes;
  using YAF.Core;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;
  

  #endregion

  /// <summary>
  /// The page requires secure connection module.
  /// </summary>
  [YafModule("Page Requires Secure Connection Module", "Tiny Gecko", 1)]
  public class PageRequiresSecureConnectionForumModule : SimpleBaseForumModule
  {
    #region Public Methods

    /// <summary>
    /// The init forum.
    /// </summary>
    public override void InitForum()
    {
      this.ForumControl.Load += this.ForumControl_Load;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The forum control_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumControl_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      bool accessDenied = false;

      switch (this.ForumPageType)
      {
        case ForumPages.login:
          if (!HttpContext.Current.Request.IsSecureConnection & this.PageContext.BoardSettings.UseSSLToLogIn)
          {
            accessDenied = true;
          }

          break;

        case ForumPages.register:
          if (!HttpContext.Current.Request.IsSecureConnection & this.PageContext.BoardSettings.UseSSLToRegister)
          {
            accessDenied = true;
          }

          break;
      }

      if (accessDenied)
      {
        YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
      }
    }

    #endregion
  }
}