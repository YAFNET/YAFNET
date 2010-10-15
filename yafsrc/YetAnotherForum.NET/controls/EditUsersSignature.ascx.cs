/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;
  using YAF.Editors;

  #endregion

  /// <summary>
  /// The edit users signature.
  /// </summary>
  public partial class EditUsersSignature : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _sig.
    /// </summary>
    private BaseForumEditor _sig;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets a value indicating whether InAdminPages.
    /// </summary>
    public bool InAdminPages { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether InModeratorMode.
    /// </summary>
    public bool InModeratorMode { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether ShowHeader.
    /// </summary>
    public bool ShowHeader
    {
      get
      {
        if (this.ViewState["ShowHeader"] != null)
        {
          return Convert.ToBoolean(this.ViewState["ShowHeader"]);
        }

        return true;
      }

      set
      {
        this.ViewState["ShowHeader"] = value;
      }
    }

    /// <summary>
    ///   Gets CurrentUserID.
    /// </summary>
    private int CurrentUserID
    {
      get
      {
        if (this.InAdminPages && this.PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey("u"))
        {
          return (int)this.PageContext.QueryIDs["u"];
        }
        else if (this.InModeratorMode && (this.PageContext.IsAdmin || this.PageContext.IsModerator) &&
                 this.PageContext.QueryIDs.ContainsKey("u"))
        {
          return (int)this.PageContext.QueryIDs["u"];
        }
        else
        {
          return this.PageContext.PageUserID;
        }
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The bind data.
    /// </summary>
    protected void BindData()
    {
      this._sig.Text = DB.user_getsignature(this.CurrentUserID);
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      // since signatures are so small only allow YafBBCode in them...
      this._sig = new BBCodeEditor();
      this.EditorLine.Controls.Add(this._sig);

      this.save.Click += this.Save_Click;
      this.cancel.Click += this.cancel_Click;

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      this.InitializeComponent();
      base.OnInit(e);
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
      this.PageContext.QueryIDs = new QueryStringIDHelper("u");

      this._sig.BaseDir = YafForumInfo.ForumClientFileRoot + "editors";
      this._sig.StyleSheet = YafContext.Current.Theme.BuildThemePath("theme.css");

      if (!this.IsPostBack)
      {
        this.save.Text = this.PageContext.Localization.GetText("COMMON", "Save");
        this.cancel.Text = this.PageContext.Localization.GetText("COMMON", "Cancel");

        this.BindData();
      }
    }

    /// <summary>
    /// The page_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
      this.trHeader.Visible = this.ShowHeader;
    }

    /// <summary>
    /// The do redirect.
    /// </summary>
    private void DoRedirect()
    {
      if (this.InModeratorMode)
      {
        YafBuildLink.Redirect(ForumPages.profile, "u={0}", this.CurrentUserID);
      }
      else
      {
        YafBuildLink.Redirect(ForumPages.cp_profile);
      }
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
    }

    /// <summary>
    /// The save_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Save_Click(object sender, EventArgs e)
    {
      string body = this._sig.Text;
      DataTable sigData = DB.user_getsignaturedata(this.CurrentUserID, YafContext.Current.PageBoardID);
      if (sigData.Rows.Count > 0)
      {
        // find forbidden BBcodes in signature
        string detectedBbCode = YafFormatMessage.BBCodeForbiddenDetector(
          body, sigData.Rows[0]["UsrSigBBCodes"].ToString().Trim().Trim(',').Trim(), ',');
        if (!string.IsNullOrEmpty(detectedBbCode) && detectedBbCode != "ALL")
        {
          this.PageContext.AddLoadMessage(
            this.PageContext.Localization.GetTextFormatted("SIGNATURE_BBCODE_WRONG", detectedBbCode));
          return;
        }
        else if (detectedBbCode == "ALL")
        {
          this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("BBCODE_FORBIDDEN"));
          return;
        }

        // find forbidden HTMLTags in signature
        if (!PageContext.IsAdmin)
        {
            string detectedHtmlTag = YafFormatMessage.CheckHtmlTags(
                body, sigData.Rows[0]["UsrSigHTMLTags"].ToString().Trim().Trim(',').Trim(), ',');
            if (detectedHtmlTag.IsSet())
            {
                this.PageContext.AddLoadMessage(detectedHtmlTag);
                return;
            }
        }
      }

      // body = YafFormatMessage.RepairHtml(this,body,false);
      if (this._sig.Text.Length > 0)
      {
        if (this._sig.Text.Length <= Convert.ToInt32(sigData.Rows[0]["UsrSigChars"]))
        {
          DB.user_savesignature(this.CurrentUserID, this.Get<YafBadWordReplace>().Replace(body));
        }
        else
        {
          this.PageContext.AddLoadMessage(
            this.PageContext.Localization.GetTextFormatted("SIGNATURE_MAX", sigData.Rows[0]["UsrSigChars"]));
          return;
        }
      }
      else
      {
        DB.user_savesignature(this.CurrentUserID, DBNull.Value);
      }

      // clear the cache for this user...
      UserMembershipHelper.ClearCacheForUserId(this.CurrentUserID);

      if (this.InAdminPages)
      {
        this.BindData();
      }
      else
      {
        this.DoRedirect();
      }
    }

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void cancel_Click(object sender, EventArgs e)
    {
      this.DoRedirect();
    }

    #endregion
  }
}