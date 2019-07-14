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
namespace YAF.Modules
{
    using System.Text;
    using System.Web.UI;

    using YAF.Core;
    using YAF.Web;

    /// <summary>
    /// The spoiler bb code module.
    /// </summary>
    public class SpoilerBBCodeModule : YafBBCodeControl
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SpoilerBBCodeModule"/> class.
    /// </summary>
    public SpoilerBBCodeModule()
      : base()
    {
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      var sb = new StringBuilder();

      var spoilerTitle = this.HtmlEncode(data: this.LocalizedString(tag: "SPOILERMOD_TOOLTIP", defaultStr: "Click here to show or hide the hidden text (also known as a spoiler)"));

      sb.AppendLine(value: "<!-- BEGIN spoiler -->");
      sb.AppendLine(value: @"<div class=""mb-3"">");
      sb.AppendFormat(
        format: @"<input type=""button"" value=""{2}"" class=""btn btn-secondary btn-sm"" name=""{0}"" onclick='toggleSpoiler(this,""{1}"");' title=""{3}"" /></div><div class=""card card-body"" id=""{1}"" style=""display:none"">", 
        this.GetUniqueID(prefix: "spoilerBtn"), 
        this.GetUniqueID(prefix: "spoil_"), 
        this.HtmlEncode(data: this.LocalizedString(tag: "SPOILERMOD_SHOW", defaultStr: "Show Spoiler")), 
        spoilerTitle);
      sb.AppendLine(value: this.Parameters[key: "inner"]);
      sb.AppendLine(value: "</div>");
      sb.AppendLine(value: "<!-- END spoiler -->");

      writer.Write(s: sb.ToString());
    }
  }
}