namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Data;
  using System.Text;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Administration inferface for managing medals.
  /// </summary>
  public partial class medals : AdminPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "medals" /> class. 
    ///   Default constructor.
    /// </summary>
    public medals()
      : base("ADMIN_MEDALS")
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    protected override void CreatePageLinks()
    {
      // forum index
      this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

      // administration index
      this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

      // current page label (no link)
      this.PageLinks.AddLink(this.GetText("ADMIN_MEDALS", "TITLE"), string.Empty);

      this.Page.Header.Title = "{0} - {1}".FormatWith(
         this.GetText("ADMIN_ADMIN", "Administration"),
         this.GetText("ADMIN_MEDALS", "TITLE"));
    }

    /// <summary>
    /// Handles on load event for delete button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_MEDALS", "CONFIRM_DELETE"));
    }

    /// <summary>
    /// Handles item command of medal list repeater.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void MedalList_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":

          // edit medal
          YafBuildLink.Redirect(ForumPages.admin_editmedal, "m={0}", e.CommandArgument);
          break;
        case "delete":

          // delete medal
					this.Get<IDbFunction>().Query.medal_delete(e.CommandArgument);

          // re-bind data
          this.BindData();
          break;
        case "moveup":
          this.Get<IDbFunction>().Query.medal_resort(this.PageContext.PageBoardID, e.CommandArgument, -1);
          this.BindData();
          break;
        case "movedown":
					this.Get<IDbFunction>().Query.medal_resort(this.PageContext.PageBoardID, e.CommandArgument, 1);
          this.BindData();
          break;
      }
    }

    /// <summary>
    /// Handles click on new medal button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void NewMedal_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      // redirect to medal edit page
      YafBuildLink.Redirect(ForumPages.admin_editmedal);
    }

    /// <summary>
    /// Handles page load event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      // this needs to be done just once, not during postbacks
        if (this.IsPostBack)
        {
            return;
        }

        // create page links
        this.CreatePageLinks();

        this.NewMedal.Text = this.GetText("ADMIN_MEDALS", "NEW_MEDAL");

        // bind data
        this.BindData();
    }

    /// <summary>
    /// Formats HTML output to display image representation of a medal.
    /// </summary>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <returns>
    /// HTML markup with image representation of a medal.
    /// </returns>
    [NotNull]
    protected string RenderImages([NotNull] object data)
    {
      var output = new StringBuilder(250);

      var dr = (DataRowView)data;

      // image of medal
      output.AppendFormat(
        "<img src=\"{0}{5}/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" align=\"top\" />", 
        YafForumInfo.ForumClientFileRoot, 
        dr["SmallMedalURL"], 
        dr["SmallMedalWidth"], 
        dr["SmallMedalHeight"], 
        this.GetText("ADMIN_MEDALS", "DISPLAY_BOX"), 
        YafBoardFolders.Current.Medals);

      // if available, create also ribbon bar image of medal
      if (!dr["SmallRibbonURL"].IsNullOrEmptyDBField())
      {
        output.AppendFormat(
          " &nbsp; <img src=\"{0}{5}/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" align=\"top\" />", 
          YafForumInfo.ForumClientFileRoot, 
          dr["SmallRibbonURL"], 
          dr["SmallRibbonWidth"], 
          dr["SmallRibbonHeight"], 
          this.GetText("ADMIN_MEDALS", "DISPLAY_RIBBON"), 
          YafBoardFolders.Current.Medals);
      }

      return output.ToString();
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
      // list medals for this board
			this.MedalList.DataSource = this.Get<IDbFunction>().GetData.medal_list(this.PageContext.PageBoardID, null);

      // bind data to controls
      this.DataBind();
    }

    #endregion
  }
}