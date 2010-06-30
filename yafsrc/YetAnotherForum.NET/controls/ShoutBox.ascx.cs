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
  using System;
  using System.Data;
  using System.Web.UI;
  using Classes.Utils;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Utilities;

  /// <summary>
  /// The shout box.
  /// </summary>
  public partial class ShoutBox : BaseUserControl
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ShoutBox"/> class.
    /// </summary>
    public ShoutBox()
    {
      PreRender += ShoutBox_PreRender;
    }

    /// <summary>
    /// Gets CacheKey.
    /// </summary>
    public string CacheKey
    {
      get
      {
        return YafCache.GetBoardCacheKey(Constants.Cache.Shoutbox);
      }
    }

    /// <summary>
    /// The shout box_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    void ShoutBox_PreRender(object sender, EventArgs e)
    {
      // set timer status based on if the place holder is visible...
      this.shoutBoxRefreshTimer.Enabled = this.shoutBoxPlaceHolder.Visible;
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
    protected void Page_Load(object sender, EventArgs e)
    {
      YafContext.Current.PageElements.RegisterJsBlockStartup(
        this.shoutBoxUpdatePanel, "DisablePageManagerScrollJs", JavaScriptBlocks.DisablePageManagerScrollJs);

      if (PageContext.User != null)
      {
        // phShoutText.Visible = true;
        this.shoutBoxPanel.Visible = true;

        if (PageContext.IsAdmin)
        {
          this.btnClear.Visible = true;
        }
      }

      if (!IsPostBack)
      {
        this.btnFlyOut.Text = PageContext.Localization.GetText("SHOUTBOX", "FLYOUT");
        this.btnClear.Text = PageContext.Localization.GetText("SHOUTBOX", "CLEAR");
        this.btnButton.Text = PageContext.Localization.GetText("SHOUTBOX", "SUBMIT");

        this.FlyOutHolder.Visible = !YafControlSettings.Current.Popup;
        this.CollapsibleImageShoutBox.Visible = !YafControlSettings.Current.Popup;

        DataBind();
      }
    }

    /// <summary>
    /// The shout box refresh timer_ tick.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ShoutBoxRefreshTimer_Tick(object sender, EventArgs e)
    {
      DataBind();
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
    protected void btnButton_Click(object sender, EventArgs e)
    {
      string username = PageContext.PageUserName;

      if (username != null && this.messageTextBox.Text != String.Empty)
      {
        DB.shoutbox_savemessage(this.messageTextBox.Text, username, PageContext.PageUserID, Request.UserHostAddress);

        // clear cache...
        PageContext.Cache.Remove(CacheKey);
      }

      DataBind();
      this.messageTextBox.Text = String.Empty;

      ScriptManager scriptManager = ScriptManager.GetCurrent(Page);

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
    protected void btnClear_Click(object sender, EventArgs e)
    {
      bool bl = DB.shoutbox_clearmessages();

      // cleared... re-load from cache...
      PageContext.Cache.Remove(CacheKey);
      DataBind();
    }

    /// <summary>
    /// The data bind.
    /// </summary>
    public override void DataBind()
    {
      BindData();
      base.DataBind();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    void BindData()
    {
      if (!this.shoutBoxPlaceHolder.Visible)
      {
        return;
      }

      var shoutBoxMessages = (DataTable) PageContext.Cache[CacheKey];

      if (shoutBoxMessages == null)
      {
        shoutBoxMessages = DB.shoutbox_getmessages(PageContext.BoardSettings.ShoutboxShowMessageCount, PageContext.BoardSettings.UseStyledNicks);

        // Set colorOnly parameter to false, as we get all color info from data base
        if (PageContext.BoardSettings.UseStyledNicks)
        {
          new StyleTransform(PageContext.Theme).DecodeStyleByTable(ref shoutBoxMessages, false);
        }

        var flags = new MessageFlags
          {
            IsBBCode = true,
            IsHtml = false
          };

        for (int i = 0; i < shoutBoxMessages.Rows.Count; i++)
        {
          string formattedMessage = FormatMsg.FormatMessage(shoutBoxMessages.Rows[i]["Message"].ToString(), flags);
          formattedMessage = FormatHyperLink(formattedMessage);
          shoutBoxMessages.Rows[i]["Message"] = formattedMessage;
        }

        // cache for 30 seconds -- could cause problems on web farm configurations.
        PageContext.Cache.Add(CacheKey, shoutBoxMessages, DateTime.UtcNow.AddSeconds(30));
      }

      this.shoutBoxRepeater.DataSource = shoutBoxMessages;
      if (PageContext.BoardSettings.ShowShoutboxSmiles)
      {
          this.smiliesRepeater.DataSource = DB.smiley_listunique(PageContext.PageBoardID);
      }
    }

    /// <summary>
    /// The format hyper link.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <returns>
    /// The format hyper link.
    /// </returns>
    static string FormatHyperLink(string message)
    {
      if (message.Contains("<a"))
      {
        for (int i = 0; i < message.Length; i++)
        {
          if (i <= message.Length - 2)
          {
            if (message.Substring(i, 2) == "<a")
            {
              message = message.Substring(i, 2) + " target=\"_blank\"" + message.Substring(i + 2, message.Length - (i + 2));
            }
          }
        }
      }

      return message;
    }

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
    protected static string FormatSmiliesOnClickString(string code, string path)
    {
      code = code.Replace("'", "\'");
      code = code.Replace("\"", "\"\"");
      code = code.Replace("\\", "\\\\");
      string onClickScript = String.Format("insertsmiley('{0}','{1}');return false;", code, path);
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
    protected void CollapsibleImageShoutBox_Click(object sender, ImageClickEventArgs e)
    {
      DataBind();
    }
  }
}