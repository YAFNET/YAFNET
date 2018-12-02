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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.Data.Profiling;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The forum footer.
    /// </summary>
    public class Footer : BaseControl
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets a value indicating whether SimpleRender.
        /// </summary>
        public bool SimpleRender { get; set; }

        /// <summary>
        ///   Gets ThisControl.
        /// </summary>
        [NotNull]
        public Control ThisControl => this;

        #endregion

        #region Methods

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
            var footer = new StringBuilder();

            this.Get<IStopWatch>().Stop();

            footer.Append(@"<div class=""clearfix""></div><footer class=""footer""><div class=""container text-right"">");

            this.RenderMobileLink(footer);

            this.RenderRulesLink(footer);

            this.RenderVersion(footer);

            this.RenderGeneratedAndDebug(footer);

            // write CSS, Refresh, then header...
            writer.Write(footer);
        }

        /// <summary>
        /// The render generated and debug.
        /// </summary>
        /// <param name="footer">
        /// The footer.
        /// </param>
        private void RenderGeneratedAndDebug([NotNull] StringBuilder footer)
        {
            if (this.Get<YafBoardSettings>().ShowPageGenerationTime)
            {
                footer.Append(@"<br /><span class=""text-muted"">");
                footer.AppendFormat(this.GetText("COMMON", "GENERATED"), this.Get<IStopWatch>().Duration);
                footer.Append("</span>");
            }

            footer.Append(@"</div></footer>");

#if DEBUG
			if (!this.PageContext.IsAdmin)
			{
				return;
			}

			footer.AppendFormat(
				@"<br /><br /><div style=""margin:auto;padding:5px;text-align:right;font-size:7pt;""><span style=""color:#990000"">YAF Compiled in <strong>DEBUG MODE</strong></span>.<br />Recompile in <strong>RELEASE MODE</strong> to remove this information:");
			footer.Append(@"<br /><br /><a href=""http://validator.w3.org/check?uri=referer"" >XHTML</a> | ");
			footer.Append(@"<a href=""http://jigsaw.w3.org/css-validator/check/referer"" >CSS</a><br /><br />");

			var extensions = this.Get<IList<Assembly>>("ExtensionAssemblies").Select(a => a.FullName).ToList();

			if (extensions.Any(x => x.Contains("PublicKeyToken=f3828393ba2d803c")))
			{
				footer.Append("Offical YAF.NET Release: Modules with Public Key of f3828393ba2d803c Loaded.");
			}

			if (extensions.Any(x => x.Contains(".Module")))
			{
				footer.AppendFormat(
					@"<br /><br />Extensions Loaded: <span style=""color: green"">{0}</span>",
					extensions.Where(x => x.Contains(".Module")).ToDelimitedString("<br />"));
			}

			footer.AppendFormat(
				@"<br /><br /><b>{0}</b> SQL Queries: <b>{1:N3}</b> Seconds (<b>{2:N2}%</b> of Total Page Load Time).<br />{3}",
				QueryCounter.Count,
				QueryCounter.Duration,
				(100 * QueryCounter.Duration) / this.Get<IStopWatch>().Duration,
				QueryCounter.Commands);
			footer.Append("</div>");
#endif
        }

        /// <summary>
        /// Renders the rules link.
        /// </summary>
        /// <param name="footer">The footer.</param>
        private void RenderRulesLink([NotNull] StringBuilder footer)
        {
            footer.Append(
                @"<a target=""_top"" title=""{1}"" href=""{0}"">{1}</a> | ".FormatWith(
                    YafBuildLink.GetLink(ForumPages.rules),
                    this.GetText("COMMON", "PRIVACY_POLICY")));
        }

        /// <summary>
        /// The render mobile link.
        /// </summary>
        /// <param name="footer">
        /// The footer.
        /// </param>
        private void RenderMobileLink([NotNull] StringBuilder footer)
        {
            if (this.Get<IYafSession>().UseMobileTheme ?? false)
            {
                footer.Append(
                    @"<a target=""_top"" title=""{1}"" href=""{0}"">{1}</a> | ".FormatWith(
                        YafBuildLink.GetLink(ForumPages.forum, "fullsite=true"),
                        this.GetText("COMMON", "MOBILE_FULLSITE")));
            }
            else if (this.PageContext.Vars.ContainsKey("IsMobile") && this.PageContext.Vars["IsMobile"] != null
                                                                   && this.PageContext.Vars["IsMobile"].ToType<bool>())
            {
                footer.Append(
                    @"<a target=""_top"" title=""{1}"" href=""{0}"">{1}</a> | ".FormatWith(
                        YafBuildLink.GetLink(ForumPages.forum, "mobilesite=true"),
                        this.GetText("COMMON", "MOBILE_VIEWSITE")));
            }
        }

        /// <summary>
        /// The render version.
        /// </summary>
        /// <param name="footer">
        /// The footer.
        /// </param>
        private void RenderVersion([NotNull] StringBuilder footer)
        {
            CodeContracts.VerifyNotNull(footer, "footer");

            // Copyright Linkback Algorithm
            // Please keep if you haven't purchased a removal or commercial license.
            var domainKey = this.Get<YafBoardSettings>().CopyrightRemovalDomainKey;
            var url = this.Get<HttpRequestBase>().Url;

            if (domainKey.IsSet() && url != null)
            {
                var dnsSafeHost = url.DnsSafeHost.ToLower();

                // handle www and non www domains correctly.
                if (dnsSafeHost.StartsWith("www."))
                {
                    dnsSafeHost = dnsSafeHost.Replace("www.", string.Empty);
                }

                if (domainKey.StartsWith("www."))
                {
                    domainKey = domainKey.Replace("www.", string.Empty);
                }

                var currentDomainHash = HashHelper.Hash(
                    dnsSafeHost,
                    HashHelper.HashAlgorithmType.SHA1,
                    this.GetType().GetSigningKey().ToString(),
                    false);

                if (domainKey.Equals(currentDomainHash))
                {
                    return;
                }
            }

            // get the theme credit info from the theme file
            // it's not really an error if it doesn't exist
            var themeCredit = this.Get<ITheme>().GetItem("THEME", "CREDIT", null);

            // append theme Credit if it exists...
            if (themeCredit.IsSet())
            {
                footer.AppendFormat(@"<span id=""themecredit"" style=""color:#999999"">{0}</span>", themeCredit);
                footer.Append("<br />");
            }

            footer.Append(@"<a target=""_top"" title=""YetAnotherForum.NET"" href=""http://www.yetanotherforum.net"">");
            footer.Append(this.GetText("COMMON", "POWERED_BY"));
            footer.Append(@" YAF.NET");

            if (this.Get<YafBoardSettings>().ShowYAFVersion)
            {
                footer.AppendFormat(" {0} ", YafForumInfo.AppVersionName);
                if (Config.IsDotNetNuke)
                {
                    footer.Append(" Under DNN ");
                }
                else if (Config.IsRainbow)
                {
                    footer.Append(" Under Rainbow ");
                }
                else if (Config.IsMojoPortal)
                {
                    footer.Append(" Under MojoPortal ");
                }
                else if (Config.IsPortalomatic)
                {
                    footer.Append(" Under Portalomatic ");
                }
            }

            footer.AppendFormat(
                @"</a> | <a target=""_top"" title=""{0}"" href=""{1}"">YAF.NET &copy; 2003-{2}, Yet Another Forum.NET</a>",
                "YetAnotherForum.NET",
                "http://www.yetanotherforum.net",
                DateTime.UtcNow.Year);
        }

        #endregion
    }
}