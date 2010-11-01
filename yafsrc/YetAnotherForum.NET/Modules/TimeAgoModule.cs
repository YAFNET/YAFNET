namespace YAF.Modules
{
  #region Using

  using System;

  using YAF.Classes.Core;
  using YAF.Classes.Pattern;
  using YAF.Utilities;

  #endregion

  /// <summary>
  /// The time ago module.
  /// </summary>
  [YafModule("Time Ago Javascript Loading Module", "Tiny Gecko", 1)]
  public class TimeAgoModule : SimpleBaseModule
  {
    #region Public Methods

    /// <summary>
    /// The init before page.
    /// </summary>
    public override void InitBeforePage()
    {
      base.InitBeforePage();

      this.PageContext.PageLoad += this.PageContext_PageLoad;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The page context_ page load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void PageContext_PageLoad([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (PageContext.BoardSettings.ShowRelativeTime && !this.PageContext.Vars.ContainsKey("RegisteredTimeago"))
      {
        YafContext.Current.PageElements.RegisterJsResourceInclude("timeagojs", "js/jquery.timeago.js");
        YafContext.Current.PageElements.RegisterJsBlockStartup("timeagoloadjs", JavaScriptBlocks.TimeagoLoadJs);
        this.PageContext.Vars["RegisteredTimeago"] = true;
      }
    }

    #endregion
  }
}