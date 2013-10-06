/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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

namespace YAF.Core.Services
{
  #region Using

    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Services.Startup;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

  #endregion

    /// <summary>
  /// Album Service for the current user.
  /// </summary>
  public class YafAlbum
  {
    #region Public Methods

    /// <summary>
    /// Deletes the specified album/image.
    /// </summary>
    /// <param name="upDir">
    /// The Upload dir.
    /// </param>
    /// <param name="albumID">
    /// The album id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="imageID">
    /// The image id.
    /// </param>
    public static void Album_Image_Delete([NotNull] object upDir, [CanBeNull] object albumID, int userID, [NotNull] object imageID)
    {
      if (albumID != null)
      {
        var dt = LegacyDb.album_image_list(albumID, null);

        foreach (var fullName in from DataRow dr in dt.Rows
                                 select "{0}/{1}.{2}.{3}.yafalbum".FormatWith(upDir, userID, albumID, dr["FileName"]) into fullName 
                                 let file = new FileInfo(fullName) 
                                 where file.Exists 
                                 select fullName)
        {
            File.Delete(fullName);
        }

        LegacyDb.album_delete(albumID);
      }
      else
      {
        using (DataTable dt = LegacyDb.album_image_list(null, imageID))
        {
          var dr = dt.Rows[0];
          var fileName = dr["FileName"].ToString();
          var imgAlbumID = dr["albumID"].ToString();
          var fullName = "{0}/{1}.{2}.{3}.yafalbum".FormatWith(upDir, userID, imgAlbumID, fileName);
          var file = new FileInfo(fullName);
          if (file.Exists)
          {
            File.Delete(fullName);
          }
        }

        LegacyDb.album_image_delete(imageID);
      }
    }

    /// <summary>
    /// The change album title.
    /// </summary>
    /// <param name="albumID">
    /// The album id.
    /// </param>
    /// <param name="newTitle">
    /// The New title.
    /// </param>
    /// <returns>
    /// the return object.
    /// </returns>
    public static ReturnClass ChangeAlbumTitle(int albumID, [NotNull] string newTitle)
    {
      // load the DB so YafContext can work...
      CodeContracts.VerifyNotNull(newTitle, "newTitle");

      YafContext.Current.Get<StartupInitializeDb>().Run();

      // newTitle = System.Web.HttpUtility.HtmlEncode(newTitle);
      LegacyDb.album_save(albumID, null, newTitle, null);

      var returnObject = new ReturnClass { NewTitle = newTitle };

        returnObject.NewTitle = (newTitle == string.Empty)
                                ? YafContext.Current.Get<ILocalization>().GetText("ALBUM", "ALBUM_CHANGE_TITLE")
                                : newTitle;
      returnObject.Id = "0{0}".FormatWith(albumID.ToString());
      return returnObject;
    }

    /// <summary>
    /// The change image caption.
    /// </summary>
    /// <param name="imageID">
    /// The Image id.
    /// </param>
    /// <param name="newCaption">
    /// The New caption.
    /// </param>
    /// <returns>
    /// the return object.
    /// </returns>
    public static ReturnClass ChangeImageCaption(int imageID, [NotNull] string newCaption)
    {
      // load the DB so YafContext can work...
      CodeContracts.VerifyNotNull(newCaption, "newCaption");

      YafContext.Current.Get<StartupInitializeDb>().Run();

      // newCaption = System.Web.HttpUtility.HtmlEncode(newCaption);
      LegacyDb.album_image_save(imageID, null, newCaption, null, null, null);
      var returnObject = new ReturnClass { NewTitle = newCaption };

        returnObject.NewTitle = (newCaption == string.Empty)
                                ? YafContext.Current.Get<ILocalization>().GetText("ALBUM", "ALBUM_IMAGE_CHANGE_CAPTION")
                                : newCaption;
      returnObject.Id = imageID.ToString();
      return returnObject;
    }

    #endregion

    /// <summary>
    /// the HTML elements class.
    /// </summary>
    [Serializable]
    public class ReturnClass
    {
      #region Properties

      /// <summary>
      ///  Gets or sets the Album/Image's Id
      /// </summary>
      public string Id { get; set; }

      /// <summary>
      ///   Gets or sets the album/image's new Title/Caption
      /// </summary>
      public string NewTitle { get; set; }

      #endregion
    }
  }
}