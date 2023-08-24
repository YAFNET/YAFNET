/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Core.Controllers;

using System.Collections.Generic;

using YAF.Core.BasePages;
using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The YAF Album controller.
/// </summary>
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class AlbumController : ForumBaseController
{
    /// <summary>
    /// The change image caption.
    /// </summary>
    /// <param name="jsonData">
    /// The JSON Data.
    /// </param>
    /// <returns>
    /// the return object.
    /// </returns>
    [ValidateAntiForgeryToken]
    [HttpPost("ChangeImageCaption")]
    public IActionResult ChangeImageCaption([FromBody] UserAlbumImage jsonData)
    {
        var albumImage = this.GetRepository<UserAlbumImage>().GetImage(jsonData.ID);

        if (albumImage.Item2.UserID != this.PageBoardContext.PageUserID)
        {
            return this.NotFound();
        }

        return this.Ok(this.Get<IAlbum>().ChangeImageCaption(jsonData.ID, jsonData.Caption));
    }

    /// <summary>
    /// Gets the paged album images
    /// </summary>
    /// <param name="pagedResults">
    /// The paged Results.
    /// </param>
    /// <returns>
    /// Returns the Attachment List as Grid Data Set
    /// </returns>
    [ValidateAntiForgeryToken]
    [HttpPost("GetAlbumImages")]
    public IActionResult GetAlbumImages([FromBody] PagedResults pagedResults)
    {
        var userId = this.PageBoardContext.PageUserID;
        var pageSize = pagedResults.PageSize;
        var pageNumber = pagedResults.PageNumber;

        var albumImages = this.GetRepository<UserAlbumImage>().GetUserAlbumImagesPaged(
            userId,
            pageNumber,
            pageSize);

        var images = new List<AttachmentItem>();

        albumImages.ForEach(
            image =>
            {
                var url = this.Get<IUrlHelper>()
                    .Action("GetImagePreview", "Albums", new {imageId = image.ID});

                    var attachment = new AttachmentItem
                                         {
                                             FileName = image.FileName,
                                             OnClick = $"setStyle('AlbumImgId', '{image.ID}')",
                                             IconImage =
                                                 $@"<img src=""{url}"" alt=""{(image.Caption.IsSet() ? image.Caption : image.FileName)}"" title=""{(image.Caption.IsSet() ? image.Caption : image.FileName)}"" class=""img-fluid img-thumbnail me-1 attachments-preview"" />",
                                             DataURL = url
                                         };

                    images.Add(attachment);
                });

        return this.Ok(
            new GridDataSet
                {
                    PageNumber = pageNumber,
                    TotalRecords =
                        albumImages.Any()
                            ? this.GetRepository<UserAlbumImage>().GetUserAlbumImageCount(userId)
                            : 0,
                    PageSize = pageSize,
                    AttachmentList = images
                });
    }
}