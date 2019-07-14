/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
namespace YAF.Modules.BBCode
{
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using YAF.Configuration;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Web;

    /// <summary>
    /// The Album Image BB Code Module.
    /// </summary>
    public class AlbumImage : YafBBCodeControl
    {
        /// <summary>
        /// Render The Album Image as Link with Image
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            var session = this.Get<HttpSessionStateBase>();
            var settings = this.Get<YafBoardSettings>();

            if (session[name: "imagePreviewWidth"] == null)
            {
                session[name: "imagePreviewWidth"] = settings.ImageAttachmentResizeWidth;
            }

            if (session[name: "imagePreviewHeight"] == null)
            {
                session[name: "imagePreviewHeight"] = settings.ImageAttachmentResizeHeight;
            }

            if (session[name: "imagePreviewCropped"] == null)
            {
                session[name: "imagePreviewCropped"] = settings.ImageAttachmentResizeCropped;
            }

            if (session[name: "localizationFile"] == null)
            {
                session[name: "localizationFile"] = this.Get<ILocalization>().LanguageFileName;
            }

            var sb = new StringBuilder();

            sb.AppendFormat(
                format: @"<a href=""{0}resource.ashx?image={1}"" class=""attachedImage"" data-gallery>",
                arg0: YafForumInfo.ForumClientFileRoot,
                arg1: this.Parameters[key: "inner"]);

            sb.AppendFormat(
                format: @"<img src=""{0}resource.ashx?imgprv={1}"" class=""img-fluid img-thumbnail"" />",
                arg0: YafForumInfo.ForumClientFileRoot,
                arg1: this.Parameters[key: "inner"]);

            sb.Append(value: "</a>");

            writer.Write(s: sb.ToString());
        }
    }
}