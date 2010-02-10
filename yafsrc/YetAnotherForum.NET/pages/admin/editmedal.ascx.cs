namespace YAF.Pages.Admin
{
  using System;
  using System.Data;
  using System.Drawing;
  using System.IO;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;
  using Image = System.Drawing.Image;

  /// <summary>
  /// The editmedal.
  /// </summary>
  public partial class editmedal : AdminPage
  {
    #region Constructors & Overriden Methods

    /// <summary>
    /// Initializes a new instance of the <see cref="editmedal"/> class. 
    /// Default constructor.
    /// </summary>
    public editmedal()
      : base("ADMIN_EDITMEDAL")
    {
    }


    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    protected override void CreatePageLinks()
    {
      // forum index
      this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

      // administration index
      this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));

      // currect page
      this.PageLinks.AddLink("Edit Medal", string.Empty);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles page load event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      // this needs to be done just once, not during postbacks
      if (!IsPostBack)
      {
        // create page links
        CreatePageLinks();

        // bind data
        BindData();
      }

      // set previews
      SetPreview(this.MedalImage, this.MedalPreview);
      SetPreview(this.RibbonImage, this.RibbonPreview);
      SetPreview(this.SmallMedalImage, this.SmallMedalPreview);
      SetPreview(this.SmallRibbonImage, this.SmallRibbonPreview);
    }


    /// <summary>
    /// Handles click on cancel button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_Click(object sender, EventArgs e)
    {
      // go back to medals administration
      YafBuildLink.Redirect(ForumPages.admin_medals);
    }


    /// <summary>
    /// Handles save button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Save_Click(object sender, EventArgs e)
    {
      if (this.MedalImage.SelectedIndex <= 0)
      {
        PageContext.AddLoadMessage("Medal image must be specified!");
        return;
      }
      else if (this.SmallMedalImage.SelectedIndex <= 0)
      {
        PageContext.AddLoadMessage("Small medal image must be specified!");
        return;
      }

      // data
      object medalID = null;
      object imageURL, smallImageURL, ribbonURL = null, smallRibbonURL = null;
      object ribbonWidth = null, ribbonHeight = null;
      Size imageSize;
      var flags = new MedalFlags(0);

      // retrieve medal ID, use null if we are creating new one
      if (Request.QueryString["m"] != null)
      {
        medalID = Request.QueryString["m"];
      }

      // flags
      flags.ShowMessage = this.ShowMessage.Checked;
      flags.AllowRibbon = this.AllowRibbon.Checked;
      flags.AllowReOrdering = this.AllowReOrdering.Checked;
      flags.AllowHiding = this.AllowHiding.Checked;

      // get medal images
      imageURL = this.MedalImage.SelectedValue;
      smallImageURL = this.SmallMedalImage.SelectedValue;
      if (this.RibbonImage.SelectedIndex > 0)
      {
        ribbonURL = this.RibbonImage.SelectedValue;
      }

      if (this.SmallRibbonImage.SelectedIndex > 0)
      {
        smallRibbonURL = this.SmallRibbonImage.SelectedValue;

        imageSize = GetImageSize(smallRibbonURL.ToString());
        ribbonWidth = imageSize.Width;
        ribbonHeight = imageSize.Height;
      }

      // get size of small image
      imageSize = GetImageSize(smallImageURL.ToString());

      // save medal
      DB.medal_save(
        PageContext.PageBoardID, 
        medalID, 
        this.Name.Text, 
        this.Description.Text, 
        this.Message.Text, 
        this.Category.Text, 
        imageURL, 
        ribbonURL, 
        smallImageURL, 
        smallRibbonURL, 
        imageSize.Width, 
        imageSize.Height, 
        ribbonWidth, 
        ribbonHeight, 
        this.SortOrder.Text, 
        flags.BitValue);

      // go back to medals administration
      YafBuildLink.Redirect(ForumPages.admin_medals);
    }


    /// <summary>
    /// Adds javascript popup to remove group link button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void GroupRemove_Load(object sender, EventArgs e)
    {
      ControlHelper.AddOnClickConfirmDialog(sender, "Remove medal from this group?");
    }


    /// <summary>
    /// Adds javascript popup to remove user link button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UserRemove_Load(object sender, EventArgs e)
    {
      ControlHelper.AddOnClickConfirmDialog(sender, "Remove medal from this user?");
    }


    /// <summary>
    /// Handles clear button click event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Clear_Click(object sender, EventArgs e)
    {
      // clear drop down
      this.UserNameList.Items.Clear();

      // hide it and show empty UserName text box
      this.UserNameList.Visible = false;
      this.UserName.Text = null;
      this.UserName.Visible = true;
      this.UserID.Text = null;

      // show find users and all users (if user is admin)
      this.FindUsers.Visible = true;

      // clear button is not necessary now
      this.Clear.Visible = false;
    }


    /// <summary>
    /// Handles find users button click event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void FindUsers_Click(object sender, EventArgs e)
    {
      // try to find users by user name
      using (DataTable dt = DB.user_find(PageContext.PageBoardID, true, this.UserName.Text, null, null))
      {
        if (dt.Rows.Count > 0)
        {
          // we found a user(s)
          this.UserNameList.DataSource = dt;
          this.UserNameList.DataValueField = "UserID";
          this.UserNameList.DataTextField = "Name";
          this.UserNameList.DataBind();

          // hide To text box and show To drop down
          this.UserNameList.Visible = true;
          this.UserName.Visible = false;

          // find is no more needed
          this.FindUsers.Visible = false;

          // we need clear button displayed now
          this.Clear.Visible = true;
        }
      }
    }


    /// <summary>
    /// Handles click on GroupList repeaters item command link buttton.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void GroupList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":

          // load group-medal to the controls
          using (DataTable dt = DB.group_medal_list(e.CommandArgument, Request.QueryString["m"]))
          {
            // prepare editing interface
            AddGroup_Click(null, e);

            // tweak it for editing
            this.GroupMedalEditTitle.Text = "Edit Medal of a Group";
            this.AvailableGroupList.Enabled = false;

            // we are intereseted inly in first row
            DataRow r = dt.Rows[0];

            // load data to controls
            this.AvailableGroupList.SelectedIndex = -1;
            this.AvailableGroupList.Items.FindByValue(r["GroupID"].ToString()).Selected = true;
            this.GroupMessage.Text = r["MessageEx"].ToString();
            this.GroupSortOrder.Text = r["SortOrder"].ToString();
            this.GroupOnlyRibbon.Checked = (bool) r["OnlyRibbon"];
            this.GroupHide.Checked = (bool) r["Hide"];

            // remove all user medals...
            RemoveMedalsFromCache();
          }

          break;
        case "remove":
          DB.group_medal_delete(e.CommandArgument, Request.QueryString["m"]);

          // remove all user medals...
          RemoveMedalsFromCache();

          BindData();
          break;
      }
    }

    /// <summary>
    /// Removals all medals from the cache...
    /// </summary>
    protected void RemoveMedalsFromCache()
    {
      // remove all user medals...
      PageContext.Cache.RemoveAllStartsWith(YafCache.GetBoardCacheKey(String.Format(Constants.Cache.UserMedals, string.Empty)));
    }


    /// <summary>
    /// Handles click on UserList repeaters item command link buttton.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UserList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":

          // load user-medal to the controls
          using (DataTable dt = DB.user_medal_list(e.CommandArgument, Request.QueryString["m"]))
          {
            // prepare editing interface
            AddUser_Click(null, e);

            // tweak it for editing
            this.UserMedalEditTitle.Text = "Edit Medal of a User";
            this.UserName.Enabled = false;
            this.FindUsers.Visible = false;

            // we are intereseted inly in first row
            DataRow r = dt.Rows[0];

            // load data to controls
            this.UserID.Text = r["UserID"].ToString();
            this.UserName.Text = r["UserName"].ToString();
            this.UserMessage.Text = r["MessageEx"].ToString();
            this.UserSortOrder.Text = r["SortOrder"].ToString();
            this.UserOnlyRibbon.Checked = (bool) r["OnlyRibbon"];
            this.UserHide.Checked = (bool) r["Hide"];
          }

          break;
        case "remove":

          // delete user-medal
          DB.user_medal_delete(e.CommandArgument, Request.QueryString["m"]);

          // clear cache...
          RemoveUserFromCache(Convert.ToInt32(Request.QueryString["m"]));
          BindData();
          break;
      }
    }


    /// <summary>
    /// Handles click on add user button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <remarks>
    /// Shows user-medal adding/editing controls.
    /// </remarks>
    protected void AddUser_Click(object sender, EventArgs e)
    {
      // set title
      this.UserMedalEditTitle.Text = "Add Medal to a User";

      // clear controls
      this.UserID.Text = null;
      this.UserName.Text = null;
      this.UserNameList.Items.Clear();
      this.UserMessage.Text = null;
      this.UserOnlyRibbon.Checked = false;
      this.UserHide.Checked = false;
      this.UserSortOrder.Text = this.SortOrder.Text;

      // set controls visibility and availability
      this.UserName.Enabled = true;
      this.UserName.Visible = true;
      this.UserNameList.Visible = false;
      this.FindUsers.Visible = true;
      this.Clear.Visible = false;

      // show editing controls and hide row with add user button
      this.AddUserRow.Visible = false;
      this.AddUserPanel.Visible = true;

      // focus on save button
      this.AddUserSave.Focus();

      // disable global save button to prevent confusion
      this.Save.Enabled = false;
    }


    /// <summary>
    /// Handles click on save user button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void AddUserSave_Click(object sender, EventArgs e)
    {
      // test if there is specified unsername/user id
      if (String.IsNullOrEmpty(this.UserID.Text) && String.IsNullOrEmpty(this.UserNameList.SelectedValue) && String.IsNullOrEmpty(this.UserName.Text))
      {
        // no username, nor userID specified
        PageContext.AddLoadMessage("Please specify valid user!");
        return;
      }
      else if (String.IsNullOrEmpty(this.UserNameList.SelectedValue) && String.IsNullOrEmpty(this.UserID.Text))
      {
        // only username is specified, we must find id for it
        using (DataTable dt = DB.user_find(PageContext.PageBoardID, true, this.UserName.Text, null, null))
        {
          if (dt.Rows.Count > 1)
          {
            // more than one user is avalilable for this username
            PageContext.AddLoadMessage("Ambiguous user name specified!");
            return;
          }
          else if (dt.Rows.Count < 1)
          {
            // no user found
            PageContext.AddLoadMessage("Please specify valid user!");
            return;
          }
          else
          {
            // save id to the control
            this.UserID.Text = dt.Rows[0]["UserID"].ToString();
          }
        }
      }
      else if (String.IsNullOrEmpty(this.UserID.Text))
      {
        // user is selected in dropdown, we must get id to UserID control
        this.UserID.Text = this.UserNameList.SelectedValue;
      }

      // save user, if there is no message specified, pass null
      DB.user_medal_save(
        this.UserID.Text, 
        Request.QueryString["m"], 
        String.IsNullOrEmpty(this.UserMessage.Text) ? null : this.UserMessage.Text, 
        this.UserHide.Checked, 
        this.UserOnlyRibbon.Checked, 
        this.UserSortOrder.Text, 
        null);

      // disable/hide edit controls
      AddUserCancel_Click(sender, e);

      // clear cache...
      RemoveUserFromCache(Convert.ToInt32(this.UserID.Text));

      // re-bind data
      BindData();
    }

    /// <summary>
    /// Removes an individual user from the cache...
    /// </summary>
    /// <param name="userId">
    /// </param>
    protected void RemoveUserFromCache(int userId)
    {
      // remove user from cache...
      PageContext.Cache.Remove(YafCache.GetBoardCacheKey(String.Format(Constants.Cache.UserMedals, userId)));
    }

    /// <summary>
    /// Hides user add/edit controls.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void AddUserCancel_Click(object sender, EventArgs e)
    {
      // set visibility
      this.AddUserRow.Visible = true;
      this.AddUserPanel.Visible = false;

      // re-enable global save button
      this.Save.Enabled = true;
    }


    /// <summary>
    /// Handles click on add group button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <remarks>
    /// Shows user-medal adding/editing controls.
    /// </remarks>
    protected void AddGroup_Click(object sender, EventArgs e)
    {
      // set title
      this.GroupMedalEditTitle.Text = "Add Medal to a Group";

      // clear controls
      this.AvailableGroupList.SelectedIndex = -1;
      this.GroupMessage.Text = null;
      this.GroupOnlyRibbon.Checked = false;
      this.GroupHide.Checked = false;
      this.GroupSortOrder.Text = this.SortOrder.Text;

      // set controls visibility and availability
      this.AvailableGroupList.Enabled = true;

      // show editing controls and hide row with add user button
      this.AddGroupRow.Visible = false;
      this.AddGroupPanel.Visible = true;

      // focus on save button
      this.AddGroupSave.Focus();

      // disable global save button to prevent confusion
      this.Save.Enabled = false;
    }


    /// <summary>
    /// Handles click on save group button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void AddGroupSave_Click(object sender, EventArgs e)
    {
      // test if there is specified unsername/user id
      if (this.AvailableGroupList.SelectedIndex < 0)
      {
        // no group selected
        PageContext.AddLoadMessage("Please select user group!");
        return;
      }

      // save group, if there is no message specified, pass null
      DB.group_medal_save(
        this.AvailableGroupList.SelectedValue, 
        Request.QueryString["m"], 
        String.IsNullOrEmpty(this.GroupMessage.Text) ? null : this.GroupMessage.Text, 
        this.GroupHide.Checked, 
        this.GroupOnlyRibbon.Checked, 
        this.GroupSortOrder.Text);

      // disable/hide edit controls
      AddGroupCancel_Click(sender, e);

      // re-bind data
      BindData();
    }


    /// <summary>
    /// Hides group add/edit controls.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void AddGroupCancel_Click(object sender, EventArgs e)
    {
      // set visibility
      this.AddGroupRow.Visible = true;
      this.AddGroupPanel.Visible = false;

      // re-enable global save button
      this.Save.Enabled = true;
    }

    #endregion

    #region Data Bidining & Formatting

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
      // load available images from images/medals folder
      using (var dt = new DataTable("Files"))
      {
        // create structure
        dt.Columns.Add("FileID", typeof(long));
        dt.Columns.Add("FileName", typeof(string));
        dt.Columns.Add("Description", typeof(string));

        // add blank row
        DataRow dr = dt.NewRow();
        dr["FileID"] = 0;
        dr["FileName"] = "../spacer.gif"; // use blank.gif for Description Entry
        dr["Description"] = "Select Medal Image";
        dt.Rows.Add(dr);

        // add files from medals folder
        var dir = new DirectoryInfo(Request.MapPath(String.Format("{0}{1}", YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Medals)));
        FileInfo[] files = dir.GetFiles("*.*");

        long nFileID = 1;

        foreach (FileInfo file in files)
        {
          string sExt = file.Extension.ToLower();

          // do just images
          if (sExt != ".png" && sExt != ".gif" && sExt != ".jpg")
          {
            continue;
          }

          dr = dt.NewRow();
          dr["FileID"] = nFileID++;
          dr["FileName"] = file.Name;
          dr["Description"] = file.Name;
          dt.Rows.Add(dr);
        }

        // medal image
        this.MedalImage.DataSource = dt;
        this.MedalImage.DataValueField = "FileName";
        this.MedalImage.DataTextField = "Description";

        // ribbon bar image
        this.RibbonImage.DataSource = dt;
        this.RibbonImage.DataValueField = "FileName";
        this.RibbonImage.DataTextField = "Description";

        // small medal image
        this.SmallMedalImage.DataSource = dt;
        this.SmallMedalImage.DataValueField = "FileName";
        this.SmallMedalImage.DataTextField = "Description";

        // small ribbon bar image
        this.SmallRibbonImage.DataSource = dt;
        this.SmallRibbonImage.DataValueField = "FileName";
        this.SmallRibbonImage.DataTextField = "Description";
      }

      // bind data to controls
      DataBind();

      // load existing medal if we are editing one
      if (Request.QueryString["m"] != null)
      {
        // load users and groups who has been assigned this medal
        this.UserList.DataSource = DB.user_medal_list(null, Request.QueryString["m"]);
        this.UserList.DataBind();
        this.GroupList.DataSource = DB.group_medal_list(null, Request.QueryString["m"]);
        this.GroupList.DataBind();

        // enable adding users/groups
        this.AddUserRow.Visible = true;
        this.AddGroupRow.Visible = true;

        using (DataTable dt = DB.medal_list(Request.QueryString["m"]))
        {
          // get data row
          DataRow row = dt.Rows[0];

          // load flags
          var flags = new MedalFlags(row["Flags"]);

          // set controls
          this.Name.Text = row["Name"].ToString();
          this.Description.Text = row["Description"].ToString();
          this.Message.Text = row["Message"].ToString();
          this.Category.Text = row["Category"].ToString();
          this.SortOrder.Text = row["SortOrder"].ToString();
          this.ShowMessage.Checked = flags.ShowMessage;
          this.AllowRibbon.Checked = flags.AllowRibbon;
          this.AllowHiding.Checked = flags.AllowHiding;
          this.AllowReOrdering.Checked = flags.AllowReOrdering;

          // select images
          SelectImage(this.MedalImage, this.MedalPreview, row["MedalURL"]);
          SelectImage(this.RibbonImage, this.RibbonPreview, row["RibbonURL"]);
          SelectImage(this.SmallMedalImage, this.SmallMedalPreview, row["SmallMedalURL"]);
          SelectImage(this.SmallRibbonImage, this.SmallRibbonPreview, row["SmallRibbonURL"]);
        }

        using (DataTable dt = DB.group_list(PageContext.PageBoardID, null))
        {
          // get data row
          DataRow row = dt.Rows[0];

          this.AvailableGroupList.DataSource = dt;
          this.AvailableGroupList.DataTextField = "Name";
          this.AvailableGroupList.DataValueField = "GroupID";
          this.AvailableGroupList.DataBind();
        }
      }
      else
      {
        // set all previews on blank image
        this.MedalPreview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumClientFileRoot);
        this.RibbonPreview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumClientFileRoot);
        this.SmallMedalPreview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumClientFileRoot);
        this.SmallRibbonPreview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumClientFileRoot);
      }
    }


    /// <summary>
    /// Creates link to group editing admin interface.
    /// </summary>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <returns>
    /// The format group link.
    /// </returns>
    protected string FormatGroupLink(object data)
    {
      var dr = (DataRowView) data;

      return String.Format("<a href=\"{1}\">{0}</a>", dr["GroupName"], YafBuildLink.GetLink(ForumPages.admin_editgroup, "i={0}", dr["GroupID"]));
    }


    /// <summary>
    /// Creates link to user editing admin interface.
    /// </summary>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <returns>
    /// The format user link.
    /// </returns>
    protected string FormatUserLink(object data)
    {
      var dr = (DataRowView) data;

      return String.Format("<a href=\"{1}\">{0}</a>", dr["UserName"], YafBuildLink.GetLink(ForumPages.admin_edituser, "u={0}", dr["UserID"]));
    }


    /// <summary>
    /// Select image in dropdown list and sets appropriate preview.
    /// </summary>
    /// <param name="list">
    /// DropDownList where to search.
    /// </param>
    /// <param name="preview">
    /// Preview image.
    /// </param>
    /// <param name="imageURL">
    /// URL to seach for.
    /// </param>
    private void SelectImage(DropDownList list, HtmlImage preview, object imageURL)
    {
      SelectImage(list, preview, imageURL.ToString());
    }

    /// <summary>
    /// Select image in dropdown list and sets appropriate preview.
    /// </summary>
    /// <param name="list">
    /// DropDownList where to search.
    /// </param>
    /// <param name="preview">
    /// Preview image.
    /// </param>
    /// <param name="imageURL">
    /// URL to seach for.
    /// </param>
    private void SelectImage(DropDownList list, HtmlImage preview, string imageURL)
    {
      // try to find item in a list
      ListItem item = list.Items.FindByText(imageURL);

      if (item != null)
      {
        // select found item
        item.Selected = true;

        // set preview image
        preview.Src = String.Format("{0}{1}/{2}", YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Medals, imageURL);
      }
      else
      {
        // if we found nothing, set blank image as preview
        preview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumClientFileRoot);
      }
    }


    /// <summary>
    /// Set onchange event for image selector DropDown to set preview image.
    /// </summary>
    /// <param name="imageSelector">
    /// DropDownList with image file listed.
    /// </param>
    /// <param name="imagePreview">
    /// Image for showing preview.
    /// </param>
    private void SetPreview(WebControl imageSelector, HtmlControl imagePreview)
    {
      // create javascript
      imageSelector.Attributes["onchange"] = String.Format(
        "getElementById('{2}').src='{0}{1}/' + this.value", YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Medals, imagePreview.ClientID);
    }


    /// <summary>
    /// Gets size of image located in medals directory.
    /// </summary>
    /// <param name="filename">
    /// Name of file.
    /// </param>
    /// <returns>
    /// Size of image.
    /// </returns>
    private Size GetImageSize(string filename)
    {
      using (Image img = Image.FromFile(Server.MapPath(String.Format("{0}{1}/{2}", YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Medals, filename))))
      {
        return img.Size;
      }
    }

    #endregion
  }
}