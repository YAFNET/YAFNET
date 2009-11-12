/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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

namespace YAF.Pages.Admin
{
  using System;
  using System.Data;
  using System.IO;
  using System.Web.Security;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for WebForm1.
  /// </summary>
  public partial class editboard : AdminPage
  {
    /// <summary>
    /// Gets BoardID.
    /// </summary>
    protected int? BoardID
    {
      get
      {
        if (!String.IsNullOrEmpty(Request.QueryString["b"]))
        {
          int boardId;
          if (int.TryParse(Request.QueryString["b"], out boardId))
          {
            return boardId;
          }
        }

        return null;
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
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Boards", string.Empty);

        BindData();

        if (BoardID != null)
        {
          this.CreateNewAdminHolder.Visible = false;

          using (DataTable dt = DB.board_list(BoardID))
          {
            DataRow row = dt.Rows[0];
            this.Name.Text = (string) row["Name"];
            this.AllowThreaded.Checked = SqlDataLayerConverter.VerifyBool(row["AllowThreaded"]);
            this.BoardMembershipAppName.Text = row["MembershipAppName"].ToString();
          }
        }
        else
        {
          this.UserName.Text = User.UserName;
          this.UserEmail.Text = User.Email;
        }
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      DataBind();
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
    protected void Save_Click(object sender, EventArgs e)
    {
      if (this.Name.Text.Trim().Length == 0)
      {
        PageContext.AddLoadMessage("You must enter a name for the board.");
        return;
      }

      if (BoardID == null && this.CreateAdminUser.Checked)
      {
        if (this.UserName.Text.Trim().Length == 0)
        {
          PageContext.AddLoadMessage("You must enter the name of a administrator user.");
          return;
        }

        if (this.UserEmail.Text.Trim().Length == 0)
        {
          PageContext.AddLoadMessage("You must enter the email address of the administrator user.");
          return;
        }

        if (this.UserPass1.Text.Trim().Length == 0)
        {
          PageContext.AddLoadMessage("You must enter a password for the administrator user.");
          return;
        }

        if (this.UserPass1.Text != this.UserPass2.Text)
        {
          PageContext.AddLoadMessage("The passwords don't match.");
          return;
        }
      }

      if (BoardID != null)
      {
        // Save current board settings
        DB.board_save(BoardID, this.Name.Text.Trim(), this.AllowThreaded.Checked);
      }
      else
      {
        // Create board
        // MEK says : Purposefully set MembershipAppName without including RolesAppName yet, as the current providers don't support different Appnames.
        if (this.CreateAdminUser.Checked)
        {
          CreateBoard(
            this.UserName.Text.Trim(), 
            this.UserPass1.Text, 
            this.UserEmail.Text.Trim(), 
            this.UserPasswordQuestion.Text.Trim(), 
            this.UserPasswordAnswer.Text.Trim(), 
            this.Name.Text.Trim(), 
            this.BoardMembershipAppName.Text.Trim(), 
            this.BoardMembershipAppName.Text.Trim(), 
            true);
        }
        else
        {
          // create admin user from logged in user...
          CreateBoard(
            null, null, null, null, null, this.Name.Text.Trim(), this.BoardMembershipAppName.Text.Trim(), this.BoardMembershipAppName.Text.Trim(), false);
        }
      }

      // Done
      PageContext.BoardSettings = null;
      YafBuildLink.Redirect(ForumPages.admin_boards);
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
    protected void Cancel_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_boards);
    }

    /// <summary>
    /// The bind data_ access mask id.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void BindData_AccessMaskID(object sender, EventArgs e)
    {
      ((DropDownList) sender).DataSource = DB.accessmask_list(PageContext.PageBoardID, null);
      ((DropDownList) sender).DataValueField = "AccessMaskID";
      ((DropDownList) sender).DataTextField = "Name";
    }

    /// <summary>
    /// The set drop down index.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SetDropDownIndex(object sender, EventArgs e)
    {
      try
      {
        var list = (DropDownList) sender;
        list.Items.FindByValue(list.Attributes["value"]).Selected = true;
      }
      catch (Exception)
      {
      }
    }

    /// <summary>
    /// The create board.
    /// </summary>
    /// <param name="adminName">
    /// The admin name.
    /// </param>
    /// <param name="adminPassword">
    /// The admin password.
    /// </param>
    /// <param name="adminEmail">
    /// The admin email.
    /// </param>
    /// <param name="adminPasswordQuestion">
    /// The admin password question.
    /// </param>
    /// <param name="adminPasswordAnswer">
    /// The admin password answer.
    /// </param>
    /// <param name="boardName">
    /// The board name.
    /// </param>
    /// <param name="boardMembershipAppName">
    /// The board membership app name.
    /// </param>
    /// <param name="boardRolesAppName">
    /// The board roles app name.
    /// </param>
    /// <param name="createUserAndRoles">
    /// The create user and roles.
    /// </param>
    /// <exception cref="ApplicationException">
    /// </exception>
    protected void CreateBoard(
      string adminName, 
      string adminPassword, 
      string adminEmail, 
      string adminPasswordQuestion, 
      string adminPasswordAnswer, 
      string boardName, 
      string boardMembershipAppName, 
      string boardRolesAppName, 
      bool createUserAndRoles)
    {
      // Store current App Names
      string currentMembershipAppName = PageContext.CurrentMembership.ApplicationName;
      string currentRolesAppName = PageContext.CurrentRoles.ApplicationName;

      if (!String.IsNullOrEmpty(boardMembershipAppName) && !String.IsNullOrEmpty(boardRolesAppName))
      {
        // Change App Names for new board
        PageContext.CurrentMembership.ApplicationName = boardMembershipAppName;
        PageContext.CurrentMembership.ApplicationName = boardRolesAppName;
      }

      int newBoardID = 0;

      if (createUserAndRoles)
      {
        // Create new admin users
        MembershipCreateStatus createStatus;
        MembershipUser newAdmin = PageContext.CurrentMembership.CreateUser(
          adminName, adminPassword, adminEmail, adminPasswordQuestion, adminPasswordAnswer, true, null, out createStatus);
        if (createStatus != MembershipCreateStatus.Success)
        {
          PageContext.AddLoadMessage(string.Format("Create User Failed: {0}", GetMembershipErrorMessage(createStatus)));
          throw new ApplicationException(string.Format("Create User Failed: {0}", GetMembershipErrorMessage(createStatus)));
        }

        // Create groups required for the new board
        RoleMembershipHelper.CreateRole("Administrators");
        RoleMembershipHelper.CreateRole("Registered");

        // Add new admin users to group
        RoleMembershipHelper.AddUserToRole(newAdmin.UserName, "Administrators");

        // Create Board
        newBoardID = DB.board_create(newAdmin.UserName, newAdmin.ProviderUserKey, boardName, boardMembershipAppName, boardRolesAppName);
      }
      else
      {
        // new admin
        MembershipUser newAdmin = UserMembershipHelper.GetUser();

        // Create Board
        newBoardID = DB.board_create(newAdmin.UserName, newAdmin.ProviderUserKey, boardName, boardMembershipAppName, boardRolesAppName);
      }


      if (newBoardID > 0 && Config.MultiBoardFolders)
      {
        // Successfully created the new board
        string boardFolder = Server.MapPath(Path.Combine(Config.BoardRoot, newBoardID.ToString() + "/"));

        // Create New Folders.
        if (!Directory.Exists(Path.Combine(boardFolder, "Images")))
        {
          // Create the Images Folders
          Directory.CreateDirectory(Path.Combine(boardFolder, "Images"));

          // Create Sub Folders
          Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Avatars"));
          Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Categories"));
          Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Emoticons"));
          Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Medals"));
          Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Ranks"));
        }

        if (!Directory.Exists(Path.Combine(boardFolder, "Themes")))
        {
          Directory.CreateDirectory(Path.Combine(boardFolder, "Themes"));

          // Need to copy default theme to the Themes Folder
        }

        if (!Directory.Exists(Path.Combine(boardFolder, "Uploads")))
        {
          Directory.CreateDirectory(Path.Combine(boardFolder, "Uploads"));
        }
      }


      // Return application name to as they were before.
      YafContext.Current.CurrentMembership.ApplicationName = currentMembershipAppName;
      YafContext.Current.CurrentRoles.ApplicationName = currentRolesAppName;
    }

    /// <summary>
    /// The create admin user_ checked changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void CreateAdminUser_CheckedChanged(object sender, EventArgs e)
    {
      this.AdminInfo.Visible = this.CreateAdminUser.Checked;
    }

    /// <summary>
    /// The get membership error message.
    /// </summary>
    /// <param name="status">
    /// The status.
    /// </param>
    /// <returns>
    /// The get membership error message.
    /// </returns>
    protected string GetMembershipErrorMessage(MembershipCreateStatus status)
    {
      switch (status)
      {
        case MembershipCreateStatus.DuplicateUserName:
          return "Username already exists. Please enter a different user name.";

        case MembershipCreateStatus.DuplicateEmail:
          return "A username for that e-mail address already exists. Please enter a different e-mail address.";

        case MembershipCreateStatus.InvalidPassword:
          return "The password provided is invalid. Please enter a valid password value.";

        case MembershipCreateStatus.InvalidEmail:
          return "The e-mail address provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
          return "The password retrieval answer provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
          return "The password retrieval question provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
          return "The user name provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.ProviderError:
          return
            "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        case MembershipCreateStatus.UserRejected:
          return
            "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        default:
          return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }
  }
}