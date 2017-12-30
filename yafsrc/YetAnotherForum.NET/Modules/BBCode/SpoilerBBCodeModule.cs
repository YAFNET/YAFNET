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
namespace YAF.Modules
{
    using System.Text;
    using System.Web.UI;

    using YAF.Core;
    using YAF.Controls;

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

      string spoilerTitle = this.HtmlEncode(LocalizedString("SPOILERMOD_TOOLTIP", "Click here to show or hide the hidden text (also known as a spoiler)"));

      sb.AppendLine("<!-- BEGIN spoiler -->");
      sb.AppendLine(@"<div class=""spoilertitle"">");
      sb.AppendFormat(
        @"<input type=""button"" value=""{2}"" class=""spoilerbutton"" name=""{0}"" onclick='toggleSpoiler(this,""{1}"");' title=""{3}"" /></div><div class=""spoilerbox"" id=""{1}"" style=""display:none"">", 
        this.GetUniqueID("spoilerBtn"), 
        this.GetUniqueID("spoil_"), 
        this.HtmlEncode(LocalizedString("SPOILERMOD_SHOW", "Show Spoiler")), 
        spoilerTitle);
      sb.AppendLine(Parameters["inner"]);
      sb.AppendLine("</div>");
      sb.AppendLine("<!-- END spoiler -->");

      writer.Write(sb.ToString());
    }
  }
}