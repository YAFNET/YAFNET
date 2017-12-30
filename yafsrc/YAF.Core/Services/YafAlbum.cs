/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Data;
    using System.IO;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Services.Startup;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

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
        public static void Album_Image_Delete(
            [NotNull] object upDir,
            [CanBeNull] object albumID,
            int userID,
            [NotNull] object imageID)
        {
            if (albumID != null)
            {
                var dt = LegacyDb.album_image_list(albumID, null);

                foreach (DataRow dr in dt.Rows)
                {
                    var fullName = "{0}/{1}.{2}.{3}.yafalbum".FormatWith(upDir, userID, albumID, dr["FileName"]);
                    var file = new FileInfo(fullName);

                    try
                    {
                        if (file.Exists)
                        {
                            File.SetAttributes(fullName, FileAttributes.Normal);
                            File.Delete(fullName);
                        }
                    }
                    finally
                    {
                        LegacyDb.album_image_delete(dr["ImageID"]);
                    }
                }

                LegacyDb.album_delete(albumID);
            }
            else
            {
                using (var dt = LegacyDb.album_image_list(null, imageID))
                {
                    var dr = dt.Rows[0];
                    var fileName = dr["FileName"].ToString();
                    var imgAlbumId = dr["albumID"].ToString();
                    var fullName = "{0}/{1}.{2}.{3}.yafalbum".FormatWith(upDir, userID, imgAlbumId, fileName);
                    var file = new FileInfo(fullName);

                    try
                    {
                        if (file.Exists)
                        {
                            File.SetAttributes(fullName, FileAttributes.Normal);
                            File.Delete(fullName);
                        }
                    }
                    finally 
                    {
                        LegacyDb.album_image_delete(imageID);
                    }
                }
            }
        }

        /// <summary>
        /// The change album title.
        /// </summary>
        /// <param name="albumId">
        /// The album id.
        /// </param>
        /// <param name="newTitle">
        /// The New title.
        /// </param>
        /// <returns>
        /// the return object.
        /// </returns>
        public static ReturnClass ChangeAlbumTitle(int albumId, [NotNull] string newTitle)
        {
            // load the DB so YafContext can work...
            CodeContracts.VerifyNotNull(newTitle, "newTitle");

            YafContext.Current.Get<StartupInitializeDb>().Run();

            // newTitle = System.Web.HttpUtility.HtmlEncode(newTitle);
            LegacyDb.album_save(albumId, null, newTitle, null);

            var returnObject = new ReturnClass { NewTitle = newTitle };

            returnObject.NewTitle = (newTitle == string.Empty)
                                        ? YafContext.Current.Get<ILocalization>().GetText("ALBUM", "ALBUM_CHANGE_TITLE")
                                        : newTitle;
            returnObject.Id = "0{0}".FormatWith(albumId.ToString());
            return returnObject;
        }

        /// <summary>
        /// The change image caption.
        /// </summary>
        /// <param name="imageId">
        /// The Image id.
        /// </param>
        /// <param name="newCaption">
        /// The New caption.
        /// </param>
        /// <returns>
        /// the return object.
        /// </returns>
        public static ReturnClass ChangeImageCaption(int imageId, [NotNull] string newCaption)
        {
            // load the DB so YafContext can work...
            CodeContracts.VerifyNotNull(newCaption, "newCaption");

            YafContext.Current.Get<StartupInitializeDb>().Run();

            // newCaption = System.Web.HttpUtility.HtmlEncode(newCaption);
            LegacyDb.album_image_save(imageId, null, newCaption, null, null, null);
            var returnObject = new ReturnClass { NewTitle = newCaption };

            returnObject.NewTitle = (newCaption == string.Empty)
                                        ? YafContext.Current.Get<ILocalization>()
                                              .GetText("ALBUM", "ALBUM_IMAGE_CHANGE_CAPTION")
                                        : newCaption;
            returnObject.Id = imageId.ToString();
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