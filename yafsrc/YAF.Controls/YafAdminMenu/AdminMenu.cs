/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Controls
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Web.UI;
  using System.Xml.Serialization;

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
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
    /// <param name="writer">
    /// The HtmlWriter 
    /// </param>
    /// <param name="listItems">
    /// Menu Items
    /// </param>
    protected void BuildUrlList(
      [NotNull] HtmlTextWriter writer, [NotNull] IEnumerable<YafMenuYafMenuSectionYafMenuItem> listItems)
    {
      if (!listItems.Any())
      {
        return;
      }

      var itemsList = new StringBuilder();

      // add each YafMenuItem to the NavView...
      foreach (var item in listItems)
      {
        bool isVisible = true;

#if !DEBUG
        isVisible = !(item.Debug.IsSet() && Convert.ToBoolean(item.Debug));
#endif

        if (!isVisible)
        {
          continue;
        }

        string url = string.Empty;

        bool highlightPage = false;

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
          if (this.PageContext.ForumPageType.Equals(itemPage))
          {
              highlightPage = true;
          }

          if (!string.IsNullOrEmpty(item.SubForumPage_0))
          {
              if (this.PageContext.ForumPageType.Equals(item.SubForumPage_0.ToEnum<ForumPages>()))
              {
                  highlightPage = true;
              }
          }

          if (!string.IsNullOrEmpty(item.SubForumPage_1))
          {
              if (this.PageContext.ForumPageType.Equals(item.SubForumPage_1.ToEnum<ForumPages>()))
              {
                  highlightPage = true;
              }
          }

          if (!string.IsNullOrEmpty(item.SubForumPage_2))
          {
              if (this.PageContext.ForumPageType.Equals(item.SubForumPage_2.ToEnum<ForumPages>()))
              {
                  highlightPage = true;
              }
          }
        }

        var highlightStyle = string.Empty;
        var highlightCssClass = string.Empty;

        if (highlightPage)
        {
          highlightStyle = "color:red;";
          highlightCssClass = " Selected";
        }

        if (item.Image.IsSet())
        {
          itemsList.AppendFormat(
            @"<li class=""YafMenuItem {4}""><span class=""YafMenuItemIcon""></span><a style=""position:relative;"" href=""{0}"">
                          <img alt=""{1}"" src=""{2}"" /><span style=""margin-left:3px;{3}"">{1}</span></a></li>", 
            url, 
            this.GetText("ADMINMENU", !string.IsNullOrEmpty(item.ForumPage) ? item.ForumPage : "admin_install"), 
            YafForumInfo.GetURLToResource("icons/{0}.png".FormatWith(item.Image)), 
            highlightStyle,
            highlightCssClass);
        }
        else
        {
          // just add the item regular style..
          itemsList.AppendFormat(
            @"<li class=""YafMenuItem""><span class=""YafMenuItemIcon""></span><a style=""position:relative;"" href=""{0}"">
                          <span style=""margin-left:3px;{2}"">{1}</span></a></li>", 
            url, 
            this.GetText("ADMINMENU", "admin_install"), 
            highlightStyle);
        }
      }

      writer.WriteLine("<div><ul>{0}</ul></div>".FormatWith(itemsList));
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
      string currentPage = this.PageContext.ForumPageType.ToString();

      // build menu...
        int viewIndex =
            this.GetMenuSections().TakeWhile(
                value =>
                !value.YafMenuItem.Any(
                    x => x.ForumPage.Equals(currentPage) || 
                    x.SubForumPage_0 != null && x.SubForumPage_0.Equals(currentPage) ||
                    x.SubForumPage_1 != null && x.SubForumPage_1.Equals(currentPage) ||
                    x.SubForumPage_2 != null && x.SubForumPage_2.Equals(currentPage))).Count();

      // setup jQuery
      this.PageContext.PageElements.RegisterJQuery();
      YafContext.Current.PageElements.RegisterJQueryUI();

      string accordianJs;

      if (viewIndex >= 7)
      {
        accordianJs =
          @"jQuery(document).ready(function() {
					jQuery('.adminMenuAccordian').accordion({ autoHeight:false,animated:'bounceslide',event:'click' });});";
      }
      else
      {
        accordianJs =
          @"jQuery(document).ready(function() {{
					jQuery('.adminMenuAccordian').accordion({{ autoHeight:false,animated:'bounceslide',event:'click',active: {0} }});}});"
            .FormatWith(viewIndex);
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
      writer.WriteLine(@"<div id=""{0}"">".FormatWith(this.ClientID));
      writer.WriteLine(@"<table class=""adminContainer""><tr>");
      writer.WriteLine(@"<td class=""adminMenu"" valign=""top"">");

      writer.WriteLine(@"<div id=""{0}"" class=""adminMenuAccordian"">".FormatWith(this.GetExtendedID("YAF_Accordian")));

      this.RenderAccordian(writer);

      writer.WriteLine(@"</div>");

      writer.WriteLine(@"</td>");

      // contents of the admin page...
      writer.WriteLine(@"<td class=""adminContent"">");

      this.RenderChildren(writer);

      writer.WriteLine(@"</td></tr></table>");
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
            this.LoadMenuFromXML();
        }

        var menuItems = this._menuDef.Items.ToList();

        var dynamicPages =
            this.Get<IEnumerable<ILocatablePage>>().Where(p => p.IsAdminPage && p.HasInterface<INavigatablePage>());

        menuItems.AddRange(
            dynamicPages.Select(
                p =>
                new YafMenuYafMenuSection
                    {
                        HostAdminOnly = p.IsHostAdminOnly.ToString(),
                        Title = p.PageName,
                        Tag = (p as INavigatablePage).PageCategory
                    }));

        return from value in menuItems
               let addView = !(Convert.ToBoolean(value.HostAdminOnly) && !this.PageContext.IsHostAdmin)
               where addView
               select value;
    }

      /// <summary>
    /// The load menu from xml.
    /// </summary>
    private void LoadMenuFromXML()
    {
      const string DefFile = "YAF.Controls.YafAdminMenu.AdminMenuDef.xml";

      // load menu definition...
      var deserializer = new XmlSerializer(typeof(YafMenu));
      using (Stream resourceStream = Assembly.GetAssembly(this.GetType()).GetManifestResourceStream(DefFile))
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
      // build menu...
      foreach (var value in this.GetMenuSections())
      {
        writer.WriteLine(@"<h3><a href=""#"">{0}</a></h3>".FormatWith(this.GetText("ADMINMENU", value.Tag)));

        // add items...
        this.BuildUrlList(writer, value.YafMenuItem);
      }
    }

    #endregion
  }
}