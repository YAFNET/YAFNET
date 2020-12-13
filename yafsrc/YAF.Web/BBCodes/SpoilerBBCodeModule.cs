/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Web.BBCodes
{
    using System.Web.UI;

    using YAF.Core.BBCode;
    using YAF.Core.Extensions;

    /// <summary>
    /// The spoiler bb code module.
    /// </summary>
    public class SpoilerBBCodeModule : BBCodeControl
    {
        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            var spoilerTitle = this.HtmlEncode(
                this.LocalizedString(
                    "SPOILERMOD_TOOLTIP",
                    "Click here to show or hide the hidden text (also known as a spoiler)"));

            writer.Write("<!-- BEGIN spoiler -->");
            writer.Write(
                @"<p>
                      <a class=""btn btn-secondary btn-sm"" data-bs-toggle=""collapse"" href=""#{0}"" role=""button"" aria-expanded=""false"" title=""{2}"">{1}</a>
                  </p>
                  <div class=""collapse"" id=""{0}""><div class=""card card-body"">",
                this.GetUniqueID("spoil_"),
                this.HtmlEncode(this.LocalizedString("SPOILERMOD_SHOW", "Show Spoiler")),
                spoilerTitle);
            writer.Write(this.Parameters["inner"]);
            writer.Write("</div></div>");
            writer.Write("<!-- END spoiler -->");
        }
    }
}