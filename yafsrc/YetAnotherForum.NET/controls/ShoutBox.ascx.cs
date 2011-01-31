//*****************************************************************************************************
//  Original code by: DLESKTECH at http://www.dlesktech.com/support.aspx
//  Modifications by: KASL Technologies at www.kasltechnologies.com
//  Mod date:7/21/2009
//  Mods: working smileys, moved smilies to bottom, added clear button for admin, new stored procedure
//  Mods: fixed the time to show the viewers time not the server time
//  Mods: added small chat window popup that runs separately from forum
//  Note: flyout button opens smaller chat window
//  Note: clear button removes message more than 24hrs old from db
//*****************************************************************************************************
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Text.RegularExpressions;
  using System.Web;
  using System.Web.UI;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utilities;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The shout box.
  /// </summary>
  public partial class ShoutBox : BaseUserControl
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ShoutBox" /> class.
    /// </summary>
    public ShoutBox()
    {
      
    }

    #endregion

    #region Properties

    public IEnumerable<DataRow> ShoutBoxMessages
    {
      get
      {
        return this.Get<IDBBroker>().GetShoutBoxMessages(YafContext.Current.PageBoardID);
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The data bind.
    /// </summary>
    public override void DataBind()
    {
      this.BindData();
      base.DataBind();
    }

    #endregion

    #region Methods

    /// <summary>
    /// The format smilies on click string.
    /// </summary>
    /// <param name="code">
    /// The code.
    /// </param>
    /// <param name="path">
    /// The path.
    /// </param>
    /// <returns>
    /// The format smilies on click string.
    /// </returns>
    protected static string FormatSmiliesOnClickString([NotNull] string code, [NotNull] string path)
    {
      code = Regex.Replace(code, "['\")(\\\\]", "\\$0");
      string onClickScript = "insertsmiley('{0}','{1}');return false;".FormatWith(code, path);
      return onClickScript;
    }

    /// <summary>
    /// The collapsible image shout box_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void CollapsibleImageShoutBox_Click([NotNull] object sender, [NotNull] ImageClickEventArgs e)
    {
      this.DataBind();
    }

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      //YafContext.Current.PageElements.RegisterJsBlockStartup(
      //  this.shoutBoxUpdatePanel, "DisablePageManagerScrollJs", JavaScriptBlocks.DisablePageManagerScrollJs);

      base.OnPreRender(e);
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.PageContext.User != null)
      {
        // phShoutText.Visible = true;
        this.shoutBoxPanel.Visible = true;

        if (this.PageContext.IsAdmin)
        {
          this.btnClear.Visible = true;
        }
      }

      YafContext.Current.PageElements.RegisterJsResourceInclude("yafPageMethodjs", "js/jquery.pagemethod.js");

      if (!this.IsPostBack)
      {
        this.btnFlyOut.Text = this.GetText("SHOUTBOX", "FLYOUT");
        this.btnClear.Text = this.GetText("SHOUTBOX", "CLEAR");
        this.btnButton.Text = this.GetText("SHOUTBOX", "SUBMIT");

        this.FlyOutHolder.Visible = !YafControlSettings.Current.Popup;
        this.CollapsibleImageShoutBox.Visible = !YafControlSettings.Current.Popup;

        this.DataBind();
      }  
    }

    /// <summary>
    /// The btn button_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnButton_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      string username = this.PageContext.PageUserName;

      if (username != null && this.messageTextBox.Text != String.Empty)
      {
        LegacyDb.shoutbox_savemessage(
          this.PageContext.PageBoardID, 
          this.messageTextBox.Text, 
          username, 
          this.PageContext.PageUserID, 
          this.Get<HttpRequestBase>().UserHostAddress);

        this.Get<IDataCache>().Remove(Constants.Cache.Shoutbox);
      }

      this.DataBind();
      this.messageTextBox.Text = String.Empty;

      ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);

      if (scriptManager != null)
      {
        scriptManager.SetFocus(this.messageTextBox);
      }
    }

    /// <summary>
    /// The btn clear_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnClear_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      bool bl = LegacyDb.shoutbox_clearmessages(this.PageContext.PageBoardID);

      // cleared... re-load from cache...
      this.Get<IDataCache>().Remove(Constants.Cache.Shoutbox);
      this.DataBind();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      if (!this.shoutBoxPlaceHolder.Visible)
      {
        return;
      }

      this.shoutBoxRepeater.DataSource = this.ShoutBoxMessages;

      if (this.PageContext.BoardSettings.ShowShoutboxSmiles)
      {
        this.smiliesRepeater.DataSource = LegacyDb.smiley_listunique(this.PageContext.PageBoardID);
      }
    }

    #endregion

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
      this.DataBind();
    }
  }
}