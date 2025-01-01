/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Types.Interfaces.Services;

/// <summary>
/// The Album interface.
/// </summary>
public interface IAlbum
{
    /// <summary>
    /// Deletes the specified album.
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
    void AlbumDelete(string uploadFolder, int albumId, int userId);

    /// <summary>
    /// Deletes the specified image.
    /// </summary>
    /// <param name="uploadFolder">
    /// The Upload folder.
    /// </param>
    /// <param name="imageId">
    /// The image id.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    void AlbumImageDelete(string uploadFolder, int imageId, int userId);

    /// <summary>
    /// Changes the album title.
    /// </summary>
    /// <param name="imageId">
    /// The Image id.
    /// </param>
    /// <param name="newTitle">
    /// The New title.
    /// </param>
    void ChangeAlbumTitle(int imageId, string newTitle);

    /// <summary>
    /// Changes the album image caption.
    /// </summary>
    /// <param name="imageId">
    /// The Image id.
    /// </param>
    /// <param name="newCaption">
    /// The New caption.
    /// </param>
    void ChangeImageCaption(int imageId, string newCaption);
}