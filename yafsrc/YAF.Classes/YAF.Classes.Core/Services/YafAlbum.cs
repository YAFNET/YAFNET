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

namespace YAF.Classes.Core
{
    using System;
    using System.Data;
    using System.IO;
    using AjaxPro;
    using YAF.Classes.Data;
    using YAF.Classes.Utils;

  /// <summary>
    /// Album Service for the current user.
    /// </summary>
    public static class YafAlbum
    {
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
        [AjaxMethod]
        public static ReturnClass ChangeAlbumTitle(int albumID, string newTitle)
        {
            // load the DB so YafContext can work...
            YafContext.Current.Get<YafInitializeDb>().Run();

           // newTitle = System.Web.HttpUtility.HtmlEncode(newTitle);
            DB.album_save(albumID, null, newTitle, null);
            var returnObject = new ReturnClass();
            returnObject.NewTitle = newTitle;
            returnObject.NewTitle = (newTitle == string.Empty)
                                        ? YafContext.Current.Localization.GetText("ALBUM", "ALBUM_CHANGE_TITLE")
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
        [AjaxMethod]
        public static ReturnClass ChangeImageCaption(int imageID, string newCaption)
        {
            // load the DB so YafContext can work...
            YafContext.Current.Get<YafInitializeDb>().Run();
           // newCaption = System.Web.HttpUtility.HtmlEncode(newCaption);
            DB.album_image_save(imageID, null, newCaption, null, null, null);
            var returnObject = new ReturnClass();
            returnObject.NewTitle = newCaption;
            returnObject.NewTitle = (newCaption == string.Empty)
                                        ? YafContext.Current.Localization.GetText("ALBUM", "ALBUM_IMAGE_CHANGE_CAPTION")
                                        : newCaption;
            returnObject.Id = imageID.ToString();
            return returnObject;
        }

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
        public static void Album_Image_Delete(object upDir, object albumID, int userID, object imageID)
        {
            if (albumID != null)
            {
                var dt = DB.album_image_list(albumID, null);
                foreach (DataRow dr in dt.Rows)
                {
                    var fullName = "{0}/{1}.{2}.{3}.yafalbum".FormatWith(upDir, userID, albumID, dr["FileName"]);
                    var file = new FileInfo(fullName);
                    if (file.Exists)
                    {
                        File.Delete(fullName);
                    }
                }
                DB.album_delete(albumID);
            }
            else
            {
                using (DataTable dt = DB.album_image_list(null, imageID))
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

                DB.album_image_delete(imageID);
            }
        }
        
        /// <summary>
        /// the HTML elements class.
        /// </summary>
        public class ReturnClass
        {
            /// <summary>
            /// the album/image's new Title/Caption
            /// </summary>
            public string NewTitle
            {
                get; 
                set;
            }

            /// <summary>
            /// the Album/Image's Id
            /// </summary>
            public string Id
            {
                get;
                set;
            }
        }    
    }
}