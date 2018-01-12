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
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web.UI;
    using System.Xml.Serialization;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Admin Menu Class.
    /// </summary>
    public class AdminMenu : BasePanel
    {
        #region Constants and Fields

        /// <summary>
        ///   The _menu def.
        /// </summary>
        private YafMenu _menuDef;

        #endregion

        #region Methods

        /// <summary>
        /// Builds a Url List
        /// </summary>
        /// <param name="activeMenu">if set to <c>true</c> [active menu].</param>
        /// <param name="listItems">Menu Items</param>
        /// <param name="page">The page.</param>
        /// <returns>
        /// Returns the Url List
        /// </returns>
        protected string BuildUrlList(out bool activeMenu, [NotNull] IEnumerable<YafMenuYafMenuSectionYafMenuItem> listItems, string page)
        {
            var itemsList = new StringBuilder();
            var isActive = false;
            activeMenu = false;


            foreach (var item in listItems.ToList())
            {
#if !DEBUG
                var isVisible = !(item.Debug.IsSet() && Convert.ToBoolean(item.Debug));

                if (!isVisible)
                {
                    continue;
                }

#endif

                var url = string.Empty;
                var highlightPage = false;

                if (item.Link.IsSet())
                {
                    url = item.Link.Replace("~", YafForumInfo.ForumClientFileRoot);
                }
                else if (item.ForumPage.IsSet())
                {
                    var itemPage = item.ForumPage.ToEnum<ForumPages>();

                    // internal "page" link...
                    url = YafBuildLink.GetLink(itemPage);

                    // Highlight the Current Page
                    highlightPage = this.PageContext.ForumPageType.Equals(itemPage)
                                    || (!string.IsNullOrEmpty(item.SubForumPage_0)
                                        && this.PageContext.ForumPageType.Equals(
                                            item.SubForumPage_0.ToEnum<ForumPages>()))
                                    || (!string.IsNullOrEmpty(item.SubForumPage_1)
                                        && this.PageContext.ForumPageType.Equals(
                                            item.SubForumPage_1.ToEnum<ForumPages>()))
                                    || (!string.IsNullOrEmpty(item.SubForumPage_2)
                                        && this.PageContext.ForumPageType.Equals(
                                            item.SubForumPage_2.ToEnum<ForumPages>()));
                }

                var activeClass = string.Empty;

                if (highlightPage)
                {
                    activeClass = " class=\"active\"";
                    isActive = true;
                    activeMenu = true;
                }

                if (item.Image.IsSet())
                {
                    itemsList.AppendFormat(
                        @"<li{4}><a href=""{0}"" aria-expanded=""{3}""{4}>
                          <i class=""fa fa-{2} fa-fw""></i>&nbsp;{1}</a></li>",
                        url,
                        this.GetText("ADMINMENU", item.ForumPage.IsSet() ? item.ForumPage : "admin_install"),
                        item.Image,
                        isActive.ToString().ToLower(),
                        isActive ? activeClass : string.Empty);
                }
                else
                {
                    // just add the item regular style..
                    itemsList.AppendFormat(
                        @"<li{3}><a href=""{0}"" aria-expanded=""{2}""{3}> {1}</a></li>",
                        url,
                        this.GetText("ADMINMENU", "admin_install"),
                        isActive.ToString().ToLower(),
                        isActive ? activeClass : string.Empty);
                }
            }

            return "<ul id=\"#{2}\" class=\"nav-second-level collapse {3}\" aria-expanded=\"{1}\">{0}</ul></li>".FormatWith(
                itemsList,
                isActive.ToString().ToLower(),
                page,
                isActive ? "show" : "in");
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            //// select the view that has the current page...
            var currentPage = this.PageContext.ForumPageType.ToString();

            // build menu...
            var viewIndex =
                this.GetMenuSections()
                    .TakeWhile(
                        value =>
                        !value.YafMenuItem.Any(
                            x =>
                            x.ForumPage.Equals(currentPage)
                            || x.SubForumPage_0 != null && x.SubForumPage_0.Equals(currentPage)
                            || x.SubForumPage_1 != null && x.SubForumPage_1.Equals(currentPage)
                            || x.SubForumPage_2 != null && x.SubForumPage_2.Equals(currentPage)))
                    .Count();

            // setup jQuery
            string accordianJs;

            // A magic digit why it's so?
            // 7 is the Number of sections inside the accordion and viewIndex marks the current opened section
            if (viewIndex >= 7)
            {
                accordianJs = @"jQuery(document).ready(function() {
					jQuery('.side-menu').metisMenu();
                    });";
            }
            else
            {
                accordianJs =
                    @"jQuery(document).ready(function() {
					jQuery('.side-menu').metisMenu();
                     });";
            }

            YafContext.Current.PageElements.RegisterJsBlockStartup("accordianJs", accordianJs);

            base.OnPreRender(e);
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            writer.BeginRender();

            // render the contents of the admin menu....
            writer.WriteLine(@"<div class=""navbar-light bg-light sidebar"" role=""navigation"">");

            writer.WriteLine(@"<div class=""collapse show sidebar-nav navbar-toggleable-xs"">");

            writer.WriteLine(@"<ul id=""nav"" class=""side-menu metismenu"">");

            this.RenderAccordian(writer);

            writer.WriteLine(@"</ul></div></div>");

            writer.WriteLine("</nav>");

            // contents of the admin page...
            writer.WriteLine(@"<div id=""page-wrapper"">");

            this.RenderChildren(writer);

            writer.WriteLine("</div>");

            writer.EndRender();
        }

        /// <summary>
        /// The get menu sections.
        /// </summary>
        /// <returns>
        /// Returns the Menu Sections
        /// </returns>
        [NotNull]
        private IEnumerable<YafMenuYafMenuSection> GetMenuSections()
        {
            if (this._menuDef == null)
            {
                this.LoadMenuFromXml();
            }

            var menuItems = this._menuDef.Items.ToList();

            var dynamicPages =
                this.Get<IEnumerable<ILocatablePage>>().Where(p => p.IsAdminPage && p.HasInterface<INavigatablePage>());

            menuItems.AddRange(
                dynamicPages.Select(
                    p =>
                        {
                            var navigatablePage = p as INavigatablePage;
                            return navigatablePage != null
                                       ? new YafMenuYafMenuSection
                                             {
                                                 HostAdminOnly =
                                                     p.IsHostAdminOnly.ToString(
                                                         CultureInfo.InvariantCulture),
                                                 Title = p.PageName,
                                                 Tag = navigatablePage.PageCategory
                                             }
                                       : null;
                        }));

            return from value in menuItems
                   let addView = !(Convert.ToBoolean(value.HostAdminOnly) && !this.PageContext.IsHostAdmin)
                   where addView
                   select value;
        }

        /// <summary>
        /// The load menu from xml.
        /// </summary>
        private void LoadMenuFromXml()
        {
            const string defFile = "YAF.Controls.YafAdminMenu.AdminMenuDef.xml";

            // load menu definition...
            var deserializer = new XmlSerializer(typeof(YafMenu));
            using (var resourceStream = Assembly.GetAssembly(this.GetType()).GetManifestResourceStream(defFile))
            {
                if (resourceStream != null)
                {
                    this._menuDef = (YafMenu)deserializer.Deserialize(resourceStream);
                }
            }
        }

        /// <summary>
        /// Render the Admin Menu Items
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private void RenderAccordian([NotNull] HtmlTextWriter writer)
        {
            var show = false;

            IEnumerable<DataRow> dt = !this.PageContext.IsHostAdmin
                                          ? LegacyDb.adminpageaccess_list(this.PageContext.PageUserID, null)
                                                .AsEnumerable()
                                                .ToList()
                                          : null;

            // build menu...
            foreach (var value in this.GetMenuSections())
            {
                // add items.. No items in menu - continue
                if (!value.YafMenuItem.Any())
                {
                    show = false;
                    continue;
                }

                // add items.. No items in menu - continue
                if ((dt == null || !dt.Any()) && !this.PageContext.IsHostAdmin)
                {
                    show = false;
                    continue;
                }


                // Check access rights to the page. Double check will be next to hide categories.
                if (!this.PageContext.IsHostAdmin)
                {
                    if (
                        value.YafMenuItem.Any(
                            va =>
                            dt.Any() && va.ForumPage.IsSet()
                            && dt.Any(row => va.ForumPage == row["PageName"].ToString())))
                    {
                        show = true;
                    }
                }

                // If a candidate entry was found ar this is a host admin
                if (show || this.PageContext.IsHostAdmin)
                {
                    IEnumerable<YafMenuYafMenuSectionYafMenuItem> g;

                    // no need to check access rights for host admin
                    if (this.PageContext.IsHostAdmin)
                    {
                        g = value.YafMenuItem;
                    }
                    else
                    {
                        g =
                            value.YafMenuItem.Where(
                                va =>
                                dt.Any(
                                    row =>
                                    dt.Any() && va.ForumPage.IsSet() && va.ForumPage == row["PageName"].ToString()));
                    }

                    // Highlight the Current Page
                    bool highlightPage;

                    var ret = this.BuildUrlList(out highlightPage, g, value.Tag);

                    if (!ret.IsSet())
                    {
                        continue;
                    }

                    var activeClass = string.Empty;

                    if (highlightPage)
                    {
                        activeClass = " class=\"active\"";
                    }

                    writer.WriteLine(
                        @"<li{3}><a aria-expanded=""{4}"" href=""#{2}""{3}><i class=""fa fa-{1} fa-fw""></i>&nbsp;{0}<span class=""fa arrow""></span></a>"
                            .FormatWith(
                                this.GetText("ADMINMENU", value.Tag),
                                value.Icon,
                                value.Tag,
                                activeClass,
                                highlightPage.ToString().ToLower()));

                    writer.WriteLine(ret);
                }

                show = false;
            }
        }

        #endregion
    }
}