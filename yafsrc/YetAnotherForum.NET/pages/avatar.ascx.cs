/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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
  #region Using

  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for avatar.
  ///   Summary description for avatar.
  /// </summary>
  public partial class avatar : ForumPage
  {
    #region Constants and Fields

    /// <summary>
    ///   The pagenum.
    /// </summary>
    public int Pagenum;

    /// <summary>
    ///   The pagesize.
    /// </summary>
    public int Pagesize = 20;

    /// <summary>
    ///   The title.
    /// </summary>
    protected Label title;

    /// <summary>
    ///   The return user id.
    /// </summary>
    private int returnUserID;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "avatar" /> class.
    /// </summary>
    public avatar()
      : base("AVATAR")
    {
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets CurrentDirectory.
    /// </summary>
    protected string CurrentDirectory
    {
      get
      {
        return this.ViewState["CurrentDir"] != null ? (string)this.ViewState["CurrentDir"] : string.Empty;
      }

      set
      {
        this.ViewState["CurrentDir"] = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The directories_ bind.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public void Directories_Bind([NotNull] object sender, [NotNull] DataListItemEventArgs e)
    {
      var directory = String.Concat(YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Avatars, "/");

      if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
      {
        return;
      }

      var dirName = e.Item.FindControl("dirName") as LinkButton;
      dirName.CommandArgument = directory + Convert.ToString(DataBinder.Eval(e.Item.DataItem, "name"));
      dirName.Text =
        @"<p style=""text-align:center""><img src=""{0}images/folder.gif"" alt=""{1}"" title=""{1}"" /><br />{1}</p>".FormatWith(
          YafForumInfo.ForumClientFileRoot, Convert.ToString(DataBinder.Eval(e.Item.DataItem, "name")));
    }

    /// <summary>
    /// The files_ bind.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public void Files_Bind([NotNull] object sender, [NotNull] DataListItemEventArgs e)
    {
      string strDirectory = Path.Combine(YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Avatars);

      var fname = (Literal)e.Item.FindControl("fname");

      if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
      {
        var finfo = new FileInfo(this.Server.MapPath(Convert.ToString(DataBinder.Eval(e.Item.DataItem, "name"))));

        if (!string.IsNullOrEmpty(this.CurrentDirectory))
        {
          strDirectory = this.CurrentDirectory;
        }

        string tmpExt = finfo.Extension.ToLower();

        if (tmpExt == ".gif" || tmpExt == ".jpg" || tmpExt == ".jpeg" || tmpExt == ".png" || tmpExt == ".bmp")
        {
          string link;

          if (this.returnUserID > 0)
          {
            link = YafBuildLink.GetLink(
              ForumPages.admin_edituser, 
              "u={0}&av={1}", 
              this.returnUserID, 
              this.Server.UrlEncode("{0}/{1}".FormatWith(strDirectory, finfo.Name)));
          }
          else
          {
            link = YafBuildLink.GetLink(
              ForumPages.cp_editavatar, "av={0}", this.Server.UrlEncode("{0}/{1}".FormatWith(strDirectory, finfo.Name)));
          }

          fname.Text =
            @"<div style=""text-align:center""><a href=""{0}""><img src=""{1}"" alt=""{2}"" title=""{2}"" class=""borderless"" /></a><br /><small>{2}</small></div>{3}"
              .FormatWith(link, "{0}/{1}".FormatWith(strDirectory, finfo.Name), finfo.Name, Environment.NewLine);
        }
      }

      if (e.Item.ItemType != ListItemType.Header)
      {
        return;
      }

      // get the previous directory...
      string previousDirectory = Path.Combine(YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Avatars);

      var up = e.Item.FindControl("up") as LinkButton;
      up.CommandArgument = previousDirectory;
      up.Text =
        @"<p style=""text-align:center""><img src=""{0}images/folder.gif"" alt=""Up"" /><br />UP</p>".FormatWith(
          YafForumInfo.ForumClientFileRoot);
      up.ToolTip = this.GetText("UP_TITLE");

      // Hide if Top Folder
      if (this.CurrentDirectory.Equals(previousDirectory))
      {
          up.Visible = false;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The directory list clean.
    /// </summary>
    /// <param name="baseDir">
    /// The base dir.
    /// </param>
    /// <returns>
    /// The Directory List
    /// </returns>
    [NotNull]
    protected List<DirectoryInfo> DirectoryListClean([NotNull] DirectoryInfo baseDir)
    {
      DirectoryInfo[] avatarDirectories = baseDir.GetDirectories();

      return
        avatarDirectories.Where(
          dir =>
          (dir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden &&
          (dir.Attributes & FileAttributes.System) != FileAttributes.System).ToList();
    }

    /// <summary>
    /// The files list clean.
    /// </summary>
    /// <param name="baseDir">
    /// The base dir.
    /// </param>
    /// <returns>
    /// File List
    /// </returns>
    [NotNull]
    protected List<FileInfo> FilesListClean([NotNull] DirectoryInfo baseDir)
    {
      FileInfo[] avatarfiles = baseDir.GetFiles("*.*");

      return
        avatarfiles.Where(
          file =>
          (file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden &&
          (file.Attributes & FileAttributes.System) != FileAttributes.System &&
          this.IsValidAvatarExtension(file.Extension.ToLower())).ToList();
    }

    /// <summary>
    /// The is valid avatar extension.
    /// </summary>
    /// <param name="extension">
    /// The extension.
    /// </param>
    /// <returns>
    /// The is valid avatar extension.
    /// </returns>
    protected bool IsValidAvatarExtension([NotNull] string extension)
    {
      return extension == ".gif" || extension == ".jpg" || extension == ".jpeg" || extension == ".png" ||
             extension == ".bmp";
    }

    /// <summary>
    /// The item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ItemCommand([NotNull] object source, [NotNull] DataListCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "directory":
          this.CurrentDirectory = e.CommandArgument.ToString();
          this.BindData(e.CommandArgument.ToString());
          break;
      }
    }

    /// <summary>
    /// The btn cancel query_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void BtnCancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        // Redirect to the edit avatar page
        YafBuildLink.Redirect(ForumPages.cp_editavatar);
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
      if (this.Request.QueryString.GetFirstOrDefault("u") != null)
      {
        this.returnUserID = this.Request.QueryString.GetFirstOrDefault("u").ToType<int>();
      }

      if (this.IsPostBack)
      {
        return;
      }

      this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));

      if (this.returnUserID > 0)
      {
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), string.Empty);
        this.PageLinks.AddLink("Users", YafBuildLink.GetLink(ForumPages.admin_users));
      }
      else
      {
        this.PageLinks.AddLink(
          this.HtmlEncode(this.PageContext.PageUserName), YafBuildLink.GetLink(ForumPages.cp_profile));
        this.PageLinks.AddLink(this.GetText("CP_EDITAVATAR", "TITLE"), YafBuildLink.GetLink(ForumPages.cp_editavatar));
      }

      this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

      this.CurrentDirectory = Path.Combine(YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Avatars);

      this.BindData(this.CurrentDirectory);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    /// <param name="currentFolder">
    /// The current Folder.
    /// </param>
    private void BindData([NotNull] string currentFolder)
    {
      var strDirectory = Path.Combine(YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Avatars);

      if (!string.IsNullOrEmpty(currentFolder))
      {
        strDirectory = currentFolder;
      }

      var baseDirectory = new DirectoryInfo(this.Server.MapPath(strDirectory));

      var avatarSubDirs = this.DirectoryListClean(baseDirectory);

      if (avatarSubDirs.Count > 0)
      {
        this.directories.Visible = true;
        this.directories.DataSource = avatarSubDirs;
        this.directories.DataBind();
      }
      else
      {
        this.directories.Visible = false;
      }

      this.files.DataSource = this.FilesListClean(baseDirectory);
      this.files.Visible = true;
      this.files.DataBind();
    }

    #endregion
  }
}