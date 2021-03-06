/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Types.Interfaces
{
    using System.Web;

    using YAF.Types.Objects;

    /// <summary>
    /// The Album interface.
    /// </summary>
    public interface IAlbum
    {
        /// <summary>
        /// Deletes the specified album/image.
        /// </summary>
        /// <param name="uploadFolder">
        /// The Upload folder.
        /// </param>
        /// <param name="albumId">
        /// The album id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="imageId">
        /// The image id.
        /// </param>
        void AlbumImageDelete([NotNull] string uploadFolder, [CanBeNull] int? albumId, int userId, [CanBeNull] int? imageId);

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
        ReturnClass ChangeAlbumTitle(int albumId, [NotNull] string newTitle);

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
        ReturnClass ChangeImageCaption(int imageId, [NotNull] string newCaption);

        /// <summary>
        /// The get album image preview.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="localizationFile">The localization file.</param>
        /// <param name="previewCropped">if set to <c>true</c> [preview cropped].</param>
        void GetAlbumImagePreview([NotNull] HttpContext context, string localizationFile, bool previewCropped);

        /// <summary>
        /// The get album cover.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="localizationFile">The localization file.</param>
        /// <param name="previewCropped">if set to <c>true</c> [preview cropped].</param>
        void GetAlbumCover([NotNull] HttpContext context, string localizationFile, bool previewCropped);

        /// <summary>
        /// The get album image.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        void GetAlbumImage([NotNull] HttpContext context);

        /// <summary>
        /// Gets the Preview Image as Response
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="localizationFile">The localization file.</param>
        /// <param name="previewCropped">if set to <c>true</c> [preview cropped].</param>
        void GetResponseImagePreview([NotNull] HttpContext context, string localizationFile, bool previewCropped);
    }
}