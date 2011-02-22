/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
namespace YAF.Pages.help
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Web;

  using YAF.Classes;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for main.
  /// </summary>
  public partial class index : ForumPage
  {
    #region Constants and Fields

    ///<summary>
    ///  List with the Help Content
    ///</summary>
    public List<YafHelpContent> helpContents = new List<YafHelpContent>();

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "index" /> class.
    /// </summary>
    public index()
      : base(null)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      this.DoSearch.Click += this.DoSearch_Click;
      base.OnInit(e);
      if (! this.Get<IPermissions>().Check(YafContext.Current.BoardSettings.ShowHelpTo))
      {
          YafBuildLink.AccessDenied();
      }
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
      this.LoadHelpContent();

      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(
          this.GetText("subtitle"), YafBuildLink.GetLink(ForumPages.help_index));

        this.DoSearch.Text = this.GetText("SEARCH", "BTNSEARCH");

        if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("faq").IsSet())
        {
          this.BindData();
        }
        else
        {
          // Load Index and Search
          this.SearchHolder.Visible = true;

          this.SubTitle.Text = this.GetText("subtitle");
          this.HelpContent.Text = this.GetText("welcome");
        }
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      string FaqPage = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("faq");

      switch (FaqPage)
      {
        case "index":
          {
            // Load Index and Search
            this.SearchHolder.Visible = true;
            this.HelpList.Visible = false;

            this.SubTitle.Text = this.GetText("subtitle");
            this.HelpContent.Text = this.GetText("welcome");
          }

          break;
        default:
          {
            var HelpContentList = this.helpContents.FindAll(check => check.HelpPage.Equals(FaqPage));

            if (HelpContentList.Count > 0)
            {
              this.SearchHolder.Visible = false;
              this.HelpList.Visible = true;
              this.HelpList.DataSource = HelpContentList;
            }
            else
            {
              // Load Index and Search
              this.SearchHolder.Visible = true;
              this.HelpList.Visible = false;

              this.SubTitle.Text = this.GetText("subtitle");
              this.HelpContent.Text = this.GetText("welcome");
            }
          }

          break;
      }

      this.HelpList.DataBind();

      this.DataBind();
    }

    /// <summary>
    /// The do search_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <exception cref="ApplicationException">
    /// </exception>
    private void DoSearch_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (string.IsNullOrEmpty(this.search.Text))
      {
        return;
      }

      if (this.search.Text.Length <= 3)
      {
        this.PageContext.AddLoadMessage(this.GetText("SEARCHLONGER"));

        return;
      }

      IList<string> highlightWords = new List<string> { this.search.Text };

      List<YafHelpContent> searchlist =
        this.helpContents.FindAll(
          check =>
          check.HelpContent.ToLower().Contains(this.search.Text.ToLower()) ||
          check.HelpTitle.ToLower().Contains(this.search.Text.ToLower()));

      foreach (YafHelpContent item in searchlist)
      {
        item.HelpContent = this.Get<IFormatMessage>().SurroundWordList(
          item.HelpContent, highlightWords, @"<span class=""highlight"">", @"</span>");
        item.HelpTitle = this.Get<IFormatMessage>().SurroundWordList(
          item.HelpTitle, highlightWords, @"<span class=""highlight"">", @"</span>");
      }

      if (searchlist.Count.Equals(0))
      {
        this.PageContext.AddLoadMessage(this.GetText("NORESULTS"));

        return;
      }

      this.HelpList.DataSource = searchlist;
      this.HelpList.DataBind();

      this.SearchHolder.Visible = false;
      this.HelpList.Visible = true;
    }

    /// <summary>
    /// Load the Complete Help Pages From the language File.
    /// </summary>
    private void LoadHelpContent()
    {
      if (!this.helpContents.Count.Equals(0))
      {
        return;
      }

      this.helpContents.Add(
        new YafHelpContent
          {
            HelpPage = "recover", 
            HelpTitle = this.GetText("RECOVERTITLE"), 
            HelpContent = this.GetText("RECOVERCONTENT").FormatWith(YafBuildLink.GetLink(ForumPages.recoverpassword))
          });

      if (!this.PageContext.BoardSettings.DisableRegistrations && !Config.IsAnyPortal)
      {
        this.helpContents.Add(
          new YafHelpContent
            {
              HelpPage = "registration", 
              HelpTitle = this.GetText("REGISTRATIONTITLE"), 
              HelpContent =
                this.GetText("REGISTRATIONCONTENT").FormatWith(YafBuildLink.GetLink(ForumPages.recoverpassword))
            });
      }

      // vzrus tip: some features can be disabled and users shouldn't normally see them in help.
      // The list can include some limitations based on host settings when features are enabled.  
      IList<string> HelpPageNames = new List<string> {
          "anounce", 
          "forums", 
          "searching", 
          "newposts", 
          "display", 
          "threadopt", 
          "memberslist", 
          "popups", 
          "pm", 
          "rss", 
          "thanks", 
          "messenger", 
          "publicprofile", 
          "editprofile", 
          "mysettings", 
          "mypics", 
          "buddies", 
          "myalbums", 
          "subscriptions", 
          "mailsettings", 
          "posting", 
          "replying", 
          "editdelete", 
          "polls", 
          "attachments", 
          "smilies", 
          "modsadmins"
        };

      foreach (string HelpPageName in HelpPageNames)
      {
        this.helpContents.Add(
          new YafHelpContent
            {
              HelpPage = HelpPageName, 
              HelpTitle = this.GetText("{0}TITLE".FormatWith(HelpPageName.ToUpper())), 
              HelpContent = this.GetText("{0}CONTENT".FormatWith(HelpPageName.ToUpper()))
            });
      }
    }

    #endregion

    /// <summary>
    /// Class that Can store the Help Content
    /// </summary>
    public class YafHelpContent
    {
      #region Properties

      /// <summary>
      ///   The Content of the Help page
      /// </summary>
      public string HelpContent { get; set; }

      /// <summary>
      ///   The Help page Name
      /// </summary>
      public string HelpPage { get; set; }

      /// <summary>
      ///   The Title of the Help page
      /// </summary>
      public string HelpTitle { get; set; }

      #endregion
    }
  }
}