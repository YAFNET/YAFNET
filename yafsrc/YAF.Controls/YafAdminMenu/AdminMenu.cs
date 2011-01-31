/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
  /// Summary description for AdminMenu.
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
    protected void BuildUrlList([NotNull] HtmlTextWriter writer, [NotNull] YafMenuYafMenuSectionYafMenuItem[] listItems)
    {
      if (listItems.Length <= 0)
      {
        return;
      }

      var itemsList = new StringBuilder();

      // add each YafMenuItem to the NavView...
      foreach (var item in listItems)
      {
        bool isVisible = !(item.Debug.IsSet() && Convert.ToBoolean(item.Debug));

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
        }

        string highlightStyle = string.Empty;

        if (highlightPage)
        {
          highlightStyle = "color:red;";
        }

        if (item.Image.IsSet())
        {
          itemsList.AppendFormat(
            @"<li class=""YafMenuItem""><span class=""YafMenuItemIcon""></span><a style=""position:relative;"" href=""{0}"">
                          <img alt=""{1}"" src=""{2}"" /><span style=""margin-left:3px;{3}"">{1}</span></a></li>", 
            url, 
            this.GetText("ADMINMENU", !string.IsNullOrEmpty(item.ForumPage) ? item.ForumPage : "admin_install"), 
            YafForumInfo.GetURLToResource("icons/{0}.png".FormatWith(item.Image)), 
            highlightStyle);
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

      //// select the view that has the current page...
      string currentPage = this.PageContext.ForumPageType.ToString();

      // build menu...
      int viewIndex = (from value in this._menuDef.Items
                       let addView = !(Convert.ToBoolean(value.HostAdminOnly) && !this.PageContext.IsHostAdmin)
                       where addView
                       select value).TakeWhile(value => !value.YafMenuItem.Any(x => x.ForumPage == currentPage)).Count();

      // setup jQuery
      this.PageContext.PageElements.RegisterJQuery();
      YafContext.Current.PageElements.RegisterJQueryUI();

      var accordianJs = string.Empty;

      if (viewIndex >= 7)
      {
        // TODO : Shows default Tab on sub pages. Needs a better detection for subpages that are not included in Admin Menu.
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
    /// Render the Admin Menu Items
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    private void RenderAccordian([NotNull] HtmlTextWriter writer)
    {
      // build menu...
      foreach (var value in from value in this._menuDef.Items
                            let addView = !(Convert.ToBoolean(value.HostAdminOnly) && !this.PageContext.IsHostAdmin)
                            where addView
                            select value)
      {
        writer.WriteLine(@"<h3><a href=""#"">{0}</a></h3>".FormatWith(this.GetText("ADMINMENU", value.Tag)));

        // add items...
        this.BuildUrlList(writer, value.YafMenuItem);
      }
    }

    #endregion
  }
}