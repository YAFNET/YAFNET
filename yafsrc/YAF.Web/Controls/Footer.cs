﻿/* Yet Another Forum.NET
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

namespace YAF.Web.Controls;

#if DEBUG
using System.Reflection;
#endif

/// <summary>
/// The forum footer.
/// </summary>
public class Footer : BaseControl
{
    /// <summary>
    ///   Gets or sets a value indicating whether SimpleRender.
    /// </summary>
    public bool SimpleRender { get; set; }

    /// <summary>
    ///   Gets ThisControl.
    /// </summary>
    [NotNull]
    public Control ThisControl => this;

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
        if (!this.SimpleRender)
        {
            this.RenderRegular(ref writer);
        }

        base.Render(writer);
    }

    /// <summary>
    /// The render regular.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected void RenderRegular([NotNull] ref HtmlTextWriter writer)
    {
        // BEGIN FOOTER
        this.Get<IStopWatch>().Stop();

        writer.Write(@"<footer class=""footer""><div class=""text-end"">");

        this.RenderRulesLink(writer);

        this.RenderVersion(writer);

        this.RenderGeneratedAndDebug(writer);
    }

    /// <summary>
    /// The render generated and debug.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    private void RenderGeneratedAndDebug([NotNull] TextWriter writer)
    {
        if (this.PageBoardContext.BoardSettings.ShowPageGenerationTime)
        {
            writer.Write(@"<br /><span class=""text-muted small"">");
            writer.Write(this.GetText("COMMON", "GENERATED"), this.Get<IStopWatch>().Duration);
            writer.Write("</span>");
        }

        writer.Write(@"</div></footer>");

#if DEBUG
        if (!this.PageBoardContext.IsAdmin)
        {
            return;
        }

        writer.Write(
            @"<br /><br /><div style=""margin:auto;padding:5px;text-align:right;font-size:7pt;"">
                              <span class=""text-danger"">YAF Compiled in <strong>DEBUG MODE</strong></span>.
                  <br />Recompile in <strong>RELEASE MODE</strong> to remove this information:");

        var extensions = this.Get<IList<Assembly>>("ExtensionAssemblies").Select(a => a.FullName).ToList();

        if (extensions.Any(x => x.Contains("PublicKeyToken=f3828393ba2d803c")))
        {
            writer.Write("Official YAF.NET Release: Modules with Public Key of f3828393ba2d803c Loaded.");
        }

        writer.Write(
            @"<div style=""margin:auto;padding:5px;text-align:right;font-size:7pt;""><span style=""color: green"">{0}</span></div>",
            Config.ConnectionProviderName);

        if (extensions.Any(x => x.Contains(".Module")))
        {
            writer.Write(
                @"<br /><br />Extensions Loaded: <span style=""color: green"">{0}</span>",
                extensions.Where(x => x.Contains(".Module")).ToDelimitedString("<br />"));
        }

        writer.Write("</div>");
#endif
    }

    /// <summary>
    /// Renders the rules link.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    private void RenderRulesLink([NotNull] TextWriter writer)
    {
        if (Config.IsAnyPortal)
        {
            return;
        }

        writer.Write(
            @"<a title=""{1}"" href=""{0}"">{1}</a> | ",
            this.Get<LinkBuilder>().GetLink(ForumPages.RulesAndPrivacy),
            this.GetText("COMMON", "PRIVACY_POLICY"));
    }

    /// <summary>
    /// The render version.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    private void RenderVersion([NotNull] TextWriter writer)
    {
        // Copyright Link-back Algorithm
        // Please keep if you haven't purchased a removal or commercial license.
        var domainKey = this.PageBoardContext.BoardSettings.CopyrightRemovalDomainKey;
        var url = this.Get<HttpRequestBase>().Url;

        if (domainKey.IsSet() && url != null)
        {
            var dnsSafeHost = url.Host.ToLowerInvariant();
            if (dnsSafeHost.LastIndexOf('.') != dnsSafeHost.IndexOf('.'))
            {
                dnsSafeHost = dnsSafeHost.Remove(
                    0,
                    dnsSafeHost.IndexOf('.') + 1);
            }

            var currentDomainHash = HashHelper.Hash(
                dnsSafeHost,
                HashAlgorithmType.SHA1,
                this.GetType().GetSigningKey().ToString(),
                false);

            if (domainKey.Equals(currentDomainHash))
            {
                return;
            }
        }

        writer.Write(@"<a target=""_blank"" title=""YetAnotherForum.NET"" href=""https://www.yetanotherforum.net"">");
        writer.Write(this.GetText("COMMON", "POWERED_BY"));
        writer.Write(@" YAF.NET");

        if (this.PageBoardContext.BoardSettings.ShowYAFVersion)
        {
            writer.Write(" {0} ", BoardInfo.AppVersionName);

            if (Config.IsDotNetNuke)
            {
                writer.Write(" Under DNN ");
            }
            else if (Config.IsMojoPortal)
            {
                writer.Write(" Under MojoPortal ");
            }
        }

        writer.Write(
            @"</a> | <a target=""_blank"" title=""{0}"" href=""{1}"">YAF.NET &copy; 2003-{2} YetAnotherForum.NET</a>",
            "YetAnotherForum.NET",
            "https://www.yetanotherforum.net",
            DateTime.UtcNow.Year);
    }
}