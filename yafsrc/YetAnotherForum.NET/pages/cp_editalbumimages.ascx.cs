// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="cp_editalbumimages.ascx.cs">
//   
// </copyright>
// <summary>
//   The cp_editalbumimages.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Pages
{
  #region Using

  using System;
  using System.Data;
  using System.IO;
  using System.Web;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The cp_editalbumimages.
  /// </summary>
  public partial class cp_editalbumimages : ForumPageRegistered
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the cp_editalbumimages class.
    /// </summary>
    public cp_editalbumimages()
      : base("CP_EDITALBUMIMAGES")
    {
    }

    #endregion

    #region Methods


    /// <summary>
    /// The back button click event handler.
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void Back_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.List.Items.Count > 0)
      {
        YafBuildLink.Redirect(
          ForumPages.album, 
          "u={0}&a={1}", 
          this.PageContext.PageUserID.ToString(), 
          this.Request.QueryString.GetFirstOrDefault("a"));
      }
      else
      {
        YafBuildLink.Redirect(ForumPages.albums, "u={0}", this.PageContext.PageUserID);
      }
    }

    /// <summary>
    /// Deletes the album and all the images in it.
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void DeleteAlbum_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      string sUpDir = this.Request.MapPath(
        String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));
      YafAlbum.Album_Image_Delete(
        sUpDir, this.Request.QueryString.GetFirstOrDefault("a"), this.PageContext.PageUserID, null);
      YafBuildLink.Redirect(ForumPages.albums, "u={0}", this.PageContext.PageUserID);
    }

    /// <summary>
    /// The btn delete_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteAlbum_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((Button)sender).Attributes["onclick"] = "return confirm(\'{0}\')".FormatWith(this.GetText("ASK_DELETEALBUM"));
    }

    /// <summary>
    /// The Upload file delete confirmation dialog.
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void ImageDelete_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((LinkButton)sender).Attributes["onclick"] = "return confirm('{0}')".FormatWith(this.GetText("ASK_DELETEIMAGE"));
    }

    /// <summary>
    /// The repater Item command event responsible for handling deletion of uploaded files.
    /// </summary>
    /// <param name="source">
    /// the source.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void List_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "delete":
                string sUpDir =
                    this.Request.MapPath(String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));
                YafAlbum.Album_Image_Delete(sUpDir, null, this.PageContext.PageUserID,
                                            Convert.ToInt32(e.CommandArgument));

                this.BindData();

                DataTable sigData = DB.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);


                int[] albumSize = DB.album_getstats(this.PageContext.PageUserID, null);

                var usrAlbumImagesAllowed =
                    sigData.GetFirstRowColumnAsValue<int?>(
                        "UsrAlbumImages", null);
                // Has the user uploaded maximum number of images?   
                // vzrus: changed for DB check The default number of album images is 0. In the case albums are disabled.
                if (usrAlbumImagesAllowed.HasValue && usrAlbumImagesAllowed > 0)
                {
                    if (List.Items.Count >= usrAlbumImagesAllowed)
                    {
                        this.uploadtitletr.Visible = false;
                        this.selectfiletr.Visible = false;
                    }
                    else
                    {
                        this.uploadtitletr.Visible = true;
                        this.selectfiletr.Visible = true;
                    }

                   this.imagesInfo.Text = this.PageContext.Localization.GetTextFormatted("IMAGES_INFO",
                                                                                     List.Items.Count, usrAlbumImagesAllowed);

                }
                else
                {
                    this.uploadtitletr.Visible = false;
                    this.selectfiletr.Visible = false;
                }

                break;
        }
    }

      /// <summary>
    /// the page load event.
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (!this.PageContext.BoardSettings.EnableAlbum)
      {
        YafBuildLink.AccessDenied();
      }

      if (!this.IsPostBack)
      {
       DataTable sigData = DB.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);


          var usrAlbumsAllowed =
                 sigData.GetFirstRowColumnAsValue<int?>(
                   "UsrAlbums", null);

        int[] albumSize = DB.album_getstats(this.PageContext.PageUserID, null);
        int userID;
        switch (this.Request.QueryString.GetFirstOrDefault("a"))
        {
            // A new album is being created. check the permissions.
          case "new":

            // Is album feature enabled?
            if (!this.PageContext.BoardSettings.EnableAlbum)
            {
              YafBuildLink.AccessDenied();
            }

            // Has the user created maximum number of albums?
            if (usrAlbumsAllowed.HasValue && usrAlbumsAllowed > 0)
            {
              // Albums count. If we reached limit then we go to info page.
                if (usrAlbumsAllowed > 0 &&
                  (albumSize[0] >= usrAlbumsAllowed))
              {
                YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
              }
            }

            /* if (this.PageContext.BoardSettings.AlbumsMax > 0 &&
                                    albumSize[0] > this.PageContext.BoardSettings.AlbumsMax - 1)
                          {
                              YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
                          }*/
            userID = this.PageContext.PageUserID;
            break;
          default:
            userID =
              Convert.ToInt32(
                DB.album_list(null, Security.StringToLongOrRedirect(this.Request.QueryString.GetFirstOrDefault("a"))).
                  Rows[0]["UserID"]);
            if (userID != this.PageContext.PageUserID)
            {
              YafBuildLink.AccessDenied();
            }

            break;
        }

        string displayName = UserMembershipHelper.GetDisplayNameFromID(userID);

        // Add the page links.
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(
          !string.IsNullOrEmpty(displayName) ? displayName : UserMembershipHelper.GetUserNameFromID(userID), 
          YafBuildLink.GetLink(ForumPages.profile, "u={0}", userID.ToString()));
        this.PageLinks.AddLink(
          this.GetText("ALBUMS"), YafBuildLink.GetLink(ForumPages.albums, "u={0}", userID.ToString()));
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        this.Back.Text = this.GetText("BACK");
        this.Upload.Text = this.GetText("UPLOAD");

        this.BindData();

        var usrAlbumImagesAllowed =
             sigData.GetFirstRowColumnAsValue<int?>(
               "UsrAlbumImages", null);
        // Has the user uploaded maximum number of images?   
        // vzrus: changed for DB check The default number of album images is 0. In the case albums are disabled.
        if (usrAlbumImagesAllowed.HasValue && usrAlbumImagesAllowed > 0)
        {
            if (List.Items.Count >= usrAlbumImagesAllowed)
            {
                this.uploadtitletr.Visible = false;
                this.selectfiletr.Visible = false;
            }
            else
            {
                this.uploadtitletr.Visible = true;
                this.selectfiletr.Visible = true;
            }

            this.imagesInfo.Text = this.PageContext.Localization.GetTextFormatted("IMAGES_INFO",
                                                                                        List.Items.Count, usrAlbumImagesAllowed);

        }
        else
        {
            this.uploadtitletr.Visible = false;
            this.selectfiletr.Visible = false;
        }
      }
    }

    /// <summary>
    /// Update the album title.
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UpdateTitle_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      string albumID = this.Request.QueryString.GetFirstOrDefault("a");
      this.txtTitle.Text = HttpUtility.HtmlEncode(this.txtTitle.Text);
      if (this.Request.QueryString.GetFirstOrDefault("a") == "new")
      {
        albumID = DB.album_save(null, this.PageContext.PageUserID, this.txtTitle.Text, null).ToString();
      }
      else
      {
        DB.album_save(this.Request.QueryString.GetFirstOrDefault("a"), null, this.txtTitle.Text, null);
      }

      YafBuildLink.Redirect(ForumPages.cp_editalbumimages, "a={0}", albumID);
    }

    /// <summary>
    /// The Upload button click event handler.
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void Upload_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      try
      {
        if (this.CheckValidFile(this.File))
        {
          this.SaveAttachment(this.File);
        }

        this.BindData();

        DataTable sigData = DB.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);


        int[] albumSize = DB.album_getstats(this.PageContext.PageUserID, null);

        var usrAlbumImagesAllowed =
            sigData.GetFirstRowColumnAsValue<int?>(
                "UsrAlbumImages", null);
        // Has the user uploaded maximum number of images?   
        // vzrus: changed for DB check The default number of album images is 0. In the case albums are disabled.
        if (usrAlbumImagesAllowed.HasValue && usrAlbumImagesAllowed > 0)
        {
            if (List.Items.Count >= usrAlbumImagesAllowed)
            {
                this.uploadtitletr.Visible = false;
                this.selectfiletr.Visible = false;
            }
            else
            {
                this.uploadtitletr.Visible = true;
                this.selectfiletr.Visible = true;
            }

            this.imagesInfo.Text = this.PageContext.Localization.GetTextFormatted("IMAGES_INFO",
                                                                            List.Items.Count, usrAlbumImagesAllowed);
        }
        else
        {
            this.uploadtitletr.Visible = false;
            this.selectfiletr.Visible = false;
        }
      }
      catch (Exception x)
      {
        DB.eventlog_create(this.PageContext.PageUserID, this, x);
        this.PageContext.AddLoadMessage(x.Message);
        return;
      }
    }

    /// <summary>
    /// Initializes the repeater control and the visibilities of form elements.
    /// </summary>
    private void BindData()
    {
      // If the user is trying to edit an existing album, initialize the repeater.
      if (this.Request.QueryString.GetFirstOrDefault("a") != "new")
      {
        this.txtTitle.Text = DB.album_gettitle(this.Request.QueryString.GetFirstOrDefault("a"));
        DataTable dt = DB.album_image_list(this.Request.QueryString.GetFirstOrDefault("a"), null);
        this.List.DataSource = dt;
        this.List.Visible = (dt.Rows.Count > 0) ? true : false;
        this.Delete.Visible = true;
        this.DataBind();
      }
      else
      {
        this.Delete.Visible = false;
      }
    }

    /// <summary>
    /// Check to see if the user is trying to upload a valid file.
    /// </summary>
    /// <param name="uploadedFile">
    /// the uploaded file.
    /// </param>
    /// <returns>
    /// true if file is valid for uploading. otherwise false.
    /// </returns>
    private bool CheckValidFile([NotNull] HtmlInputFile uploadedFile)
    {
      string filePath = uploadedFile.PostedFile.FileName.Trim();

      if (filePath.IsNotSet() || uploadedFile.PostedFile.ContentLength == 0)
      {
        return false;
      }

      string extension = Path.GetExtension(filePath).ToLower();

      // remove the "period"
      extension = extension.Replace(".", string.Empty);
      string[] aImageExtensions = { "jpg", "gif", "png", "bmp" };

      // If we don't get a match from the db, then the extension is not allowed
      DataTable dt = DB.extension_list(this.PageContext.PageBoardID, extension);

      // also, check to see an image is being uploaded.
      if (Array.IndexOf(aImageExtensions, extension) == -1 || dt.Rows.Count == 0)
      {
        this.PageContext.AddLoadMessage(this.GetTextFormatted("FILEERROR", extension));
        return false;
      }

      return true;
    }

    /// <summary>
    /// Save the attached file both physically and in the db.
    /// </summary>
    /// <param name="file">
    /// the file.
    /// </param>
    private void SaveAttachment([NotNull] HtmlInputFile file)
    {
      if (file.PostedFile == null || file.PostedFile.FileName.Trim().Length == 0 || file.PostedFile.ContentLength == 0)
      {
        return;
      }

      string sUpDir = this.Request.MapPath(
        String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));
      string filename = file.PostedFile.FileName;

      int pos = filename.LastIndexOfAny(new[] { '/', '\\' });
      if (pos >= 0)
      {
        filename = filename.Substring(pos + 1);
      }

      // filename can be only 255 characters long (due to table column)
      if (filename.Length > 255)
      {
        filename = filename.Substring(filename.Length - 255);
      }

      // verify the size of the attachment
      if (this.PageContext.BoardSettings.AlbumImagesSizeMax > 0 &&
          file.PostedFile.ContentLength > this.PageContext.BoardSettings.AlbumImagesSizeMax)
      {
        throw new Exception(this.GetText("ERROR_TOOBIG"));
      }

      // vzrus: the checks here are useless but in a case...
      DataTable sigData = DB.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);

         var usrAlbumsAllowed =
                 sigData.GetFirstRowColumnAsValue<int?>(
                   "UsrAlbums", null);
        var usrAlbumImagesAllowed =
                  sigData.GetFirstRowColumnAsValue<int?>(
                    "UsrAlbumImages", null);

        //if (!usrAlbums.HasValue || usrAlbums <= 0) return;

        if (this.Request.QueryString.GetFirstOrDefault("a") == "new")
        {
            int[] alstats = DB.album_getstats(this.PageContext.PageUserID, null);


            // Albums count. If we reached limit then we exit.
            if (alstats[0] >= usrAlbumsAllowed)
            {
                this.PageContext.AddLoadMessage(this.GetTextFormatted("ALBUMS_COUNT_LIMIT", usrAlbumImagesAllowed));
                return;
            }

            int albumID = DB.album_save(null, this.PageContext.PageUserID, this.txtTitle.Text, null);
            file.PostedFile.SaveAs(
                "{0}/{1}.{2}.{3}.yafalbum".FormatWith(sUpDir, this.PageContext.PageUserID, albumID.ToString(), filename));
            DB.album_image_save(null, albumID, null, filename, file.PostedFile.ContentLength, file.PostedFile.ContentType);
            YafBuildLink.Redirect(ForumPages.cp_editalbumimages, "a={0}", albumID);
        }
        else
        {
            // vzrus: the checks here are useless but in a case...
            int[] alstats = DB.album_getstats(
                this.PageContext.PageUserID, this.Request.QueryString.GetFirstOrDefault("a"));
            /*
            // Albums count. If we reached limit then we exit. 
            // Check it first as user could be in other group or prev YAF version was used;
            if (DB.album_getstats(this.PageContext.PageUserID, null)[0] >= usrAlbums)
            {
                this.PageContext.AddLoadMessage(this.GetTextFormatted("ALBUMS_COUNT_LIMIT", usrAlbums));
               return;
            }*/

            // Images count. If we reached limit then we exit.
            if (alstats[1] >= usrAlbumImagesAllowed)
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("IMAGES_COUNT_LIMIT", usrAlbumImagesAllowed));
                return;
            }

            file.PostedFile.SaveAs(
                "{0}/{1}.{2}.{3}.yafalbum".FormatWith(
                    sUpDir, this.PageContext.PageUserID, this.Request.QueryString.GetFirstOrDefault("a"), filename));
            DB.album_image_save(
                null, 
                this.Request.QueryString.GetFirstOrDefault("a"), 
                null, 
                filename, 
                file.PostedFile.ContentLength, 
                file.PostedFile.ContentType);
        }
    }

    #endregion
  }
}