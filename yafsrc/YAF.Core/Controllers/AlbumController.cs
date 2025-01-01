﻿/* Yet Another Forum.NET
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

namespace YAF.Core.Controllers;

using System.Collections.Generic;
using System.Web.Http;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The YAF Album controller.
/// </summary>
[RoutePrefix("api")]
public class AlbumController : ApiController, IHaveServiceLocator
{
    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Change the album title.
    /// </summary>
    [Route("Album/ChangeAlbumTitle")]
    [HttpPost]
    public IHttpActionResult ChangeAlbumTitle()
    {
        var imageId = this.Get<HttpRequestBase>().Form["id"].ToType<int>();
        var newCaption = HttpUtility.HtmlEncode(this.Get<HttpRequestBase>().Form["value"].Trim());

        if (newCaption.Equals(this.Get<ILocalization>().GetText("ALBUM_CHANGE_TITLE")))
        {
            return this.Ok();
        }

        this.Get<IAlbum>().ChangeAlbumTitle(imageId, newCaption);

        return this.Ok();
    }

    /// <summary>
    /// Change the album image caption.
    /// </summary>
    [Route("Album/ChangeImageCaption")]
    [HttpPost]
    public IHttpActionResult ChangeImageCaption()
    {
        var imageId = this.Get<HttpRequestBase>().Form["id"].ToType<int>();
        var newCaption = HttpUtility.HtmlEncode(this.Get<HttpRequestBase>().Form["value"].Trim());

        if (newCaption.Equals(this.Get<ILocalization>().GetText("ALBUM_IMAGE_CHANGE_CAPTION"))
            || newCaption.Equals(this.Get<ILocalization>().GetText("ALBUM_IMAGE_CHANGE_CAPTION2")))
        {
            return this.Ok();
        }

        this.Get<IAlbum>().ChangeImageCaption(imageId, newCaption);

        return this.Ok();
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
    [Route("Album/GetAlbumImages")]
    [HttpPost]
    public IHttpActionResult GetAlbumImages(PagedResults pagedResults)
    {
        var userId = BoardContext.Current.PageUserID;
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
                    var url = $"{BoardInfo.ForumClientFileRoot}resource.ashx?imgprv={image.ID}";

                    var attachment = new AttachmentItem
                                         {
                                             FileName = image.FileName,
                                             OnClick = $"setStyle('albumimg', '{image.ID}')", IconImage =
                                                 $"""<img src="{url}" alt="{(image.Caption.IsSet() ? image.Caption : image.FileName)}" title="{(image.Caption.IsSet() ? image.Caption : image.FileName)}" class="img-fluid img-thumbnail me-1 attachments-preview" />""",
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