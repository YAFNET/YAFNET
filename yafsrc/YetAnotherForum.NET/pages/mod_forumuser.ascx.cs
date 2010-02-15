/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Pages
{
  // YAF.Pages
  using System;
  using System.Data;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Control handling user invitations to forum (i.e. granting permissions by admin/moderator).
  /// </summary>
  public partial class mod_forumuser : ForumPage
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="mod_forumuser"/> class. 
    /// Default constructor.
    /// </summary>
    public mod_forumuser()
      : base("MOD_FORUMUSER")
    {
    }

    #endregion

    #region MyRegion

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    protected override void CreatePageLinks()
    {
      if (PageContext.Settings.LockedForum == 0)
      {
        // forum index
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

        // category
        this.PageLinks.AddLink(PageContext.PageCategoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", PageContext.PageCategoryID));
      }

      // forum page
      this.PageLinks.AddForumLinks(PageContext.PageForumID);

      // currect page
      this.PageLinks.AddLink(GetText("TITLE"), string.Empty);
    }

    #endregion

    #region Page Events

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
      // only moderators/admins are allowed in
      if (!PageContext.ForumModeratorAccess)
      {
        YafBuildLink.AccessDenied();
      }

      // do not repeat on postbact
      if (!IsPostBack)
      {
        // create page links
        CreatePageLinks();

        // load localized texts for buttons
        this.FindUsers.Text = GetText("FIND");
        this.Update.Text = GetText("UPDATE");
        this.Cancel.Text = GetText("CANCEL");

        // bind data
        DataBind();

        // if there is concrete user being handled
        if (Request.QueryString["u"] != null)
        {
          using (DataTable dt = DB.userforum_list(Request.QueryString["u"], PageContext.PageForumID))
          {
            foreach (DataRow row in dt.Rows)
            {
              // set username and disable its editing
              this.UserName.Text = row["Name"].ToString();
              this.UserName.Enabled = false;

              // we don't need to find users now
              this.FindUsers.Visible = false;

              // get access mask for this user
              this.AccessMaskID.Items.FindByValue(row["AccessMaskID"].ToString()).Selected = true;
            }
          }
        }
      }
    }

    #endregion

    #region Button Click Events

    /// <summary>
    /// Handles FindUsers button click event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void FindUsers_Click(object sender, EventArgs e)
    {
      // we need at least two characters to search user by
      if (this.UserName.Text.Length < 2)
      {
        return;
      }

      // get found users
      var foundUsers = PageContext.UserDisplayName.Find(this.UserName.Text.Trim());

      // have we found anyone?
      if (foundUsers.Count > 0)
      {
        // set and enable user dropdown, disable text box
        this.ToList.DataSource = foundUsers;
        this.ToList.DataValueField = "Key";
        this.ToList.DataTextField = "Value";

        // ToList.SelectedIndex = 0;
        this.ToList.Visible = true;
        this.UserName.Visible = false;
        this.FindUsers.Visible = false;
      }

      // bind data (is this necessary?)
      base.DataBind();
    }

    /// <summary>
    /// Handles click event of Update button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Update_Click(object sender, EventArgs e)
    {
      // no user was specified
      if (this.UserName.Text.Length <= 0)
      {
        PageContext.AddLoadMessage(GetText("NO_SUCH_USER"));
        return;
      }

      // if we choose user from drop down, set selected value to text box
      if (this.ToList.Visible)
      {
        this.UserName.Text = this.ToList.SelectedItem.Text;
      }

      // we need to verify user exists
      var userId = PageContext.UserDisplayName.GetId(this.UserName.Text.Trim());

      // there is no such user or reference is ambiugous
      if (!userId.HasValue)
      {
        PageContext.AddLoadMessage(GetText("NO_SUCH_USER"));
        return;
      }
      else if (UserMembershipHelper.IsGuestUser(userId))
      {
        PageContext.AddLoadMessage(GetText("NOT_GUEST"));
        return;
      }

      // save permission
      DB.userforum_save(userId.Value, PageContext.PageForumID, this.AccessMaskID.SelectedValue);

      // redirect to forum moderation page
      YafBuildLink.Redirect(ForumPages.moderate, "f={0}", PageContext.PageForumID);

      // clear moderatorss cache
      PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));
    }

    /// <summary>
    /// Handles click event of cancel button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_Click(object sender, EventArgs e)
    {
      // redirect to forum moderation page
      YafBuildLink.Redirect(ForumPages.moderate, "f={0}", PageContext.PageForumID);
    }

    #endregion

    #region Data Bidining

    /// <summary>
    /// The data bind.
    /// </summary>
    public override void DataBind()
    {
      // load data
      DataTable dt;

      // only admin can assign all access masks
      if (!PageContext.IsAdmin)
      {
        // do not include access masks with this flags set
        var flags = (int) AccessFlags.Flags.ModeratorAccess;

        // non-admins cannot assign moderation access masks
        dt = DB.accessmask_list(PageContext.PageBoardID, null, flags);
      }
      else
      {
        dt = DB.accessmask_list(PageContext.PageBoardID, null);
      }

      // setup datasource for access masks dropdown
      this.AccessMaskID.DataSource = dt;
      this.AccessMaskID.DataValueField = "AccessMaskID";
      this.AccessMaskID.DataTextField = "Name";

      base.DataBind();
    }

    #endregion

    /* Construction & Destruction */

    /* Overriden Base Methods */

    /* Event Handlers */

    /* Data Bidining & Formatting */
  }
}