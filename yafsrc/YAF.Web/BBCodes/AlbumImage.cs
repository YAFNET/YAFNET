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
namespace YAF.Web.BBCodes;

/// <summary>
/// The Album Image BB Code Module.
/// </summary>
public class AlbumImage : BBCodeControl
{
    /// <summary>
    /// Render The Album Image as Link with Image
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
        var imageId = HtmlTagHelper.StripHtml(this.Parameters["inner"]);

        if (!ValidationHelper.IsNumeric(imageId))
        {
            return;
        }

        var albumImage = this.GetRepository<UserAlbumImage>().GetById(imageId.ToType<int>());

        if (albumImage is null || !this.PageBoardContext.BoardSettings.EnableAlbum)
        {
            return;
        }

        writer.Write(
            """<div class="card text-bg-dark" style="max-width:{0}px">""",
            this.PageBoardContext.BoardSettings.ImageThumbnailMaxWidth);

        writer.Write(
            """<a href="{0}resource.ashx?image={1}" data-toggle="lightbox" data-gallery="#gallery-{2}" title="{1}">""",
            BoardInfo.ForumClientFileRoot,
            imageId,
            this.MessageID.Value);

        writer.Write(
            """<img src="{0}resource.ashx?imgprv={1}" class="img-user-posted card-img-top" style="max-height:{2}px;object-fit:contain" alt="{1}">""",
            BoardInfo.ForumClientFileRoot,
            imageId,
            this.PageBoardContext.BoardSettings.ImageThumbnailMaxHeight);

        writer.Write("</a>");

        writer.Write("""<div class="card-body py-1">""");

        writer.Write("""<p class="card-text text-center small">{0}</p>""", this.GetText("IMAGE_RESIZE_ENLARGE"));

        writer.Write(@"</div></div>");
    }
}