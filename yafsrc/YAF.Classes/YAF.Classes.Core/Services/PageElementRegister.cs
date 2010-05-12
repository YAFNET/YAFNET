/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
  /// <summary>
  /// Helper Class providing functions to register page elements.
  /// </summary>
  public class PageElementRegister
  {
    /// <summary>
    /// The _registered elements.
    /// </summary>
    private readonly List<string> _registeredElements = new List<string>();

    /// <summary>
    /// Gets elements (using in the head or header) that are registered on the page.
    /// Used mostly by RegisterPageElementHelper.
    /// </summary>
    public List<string> RegisteredElements
    {
      get
      {
        return this._registeredElements;
      }
    }

    /// <summary>
    /// Validates if an page element exists.
    /// </summary>
    /// <param name="name">
    /// Unique name of the page element to test for
    /// </param>
    /// <returns>
    /// <see langword="true"/> if it exists
    /// </returns>
    public bool PageElementExists(string name)
    {
      return this._registeredElements.Contains(name.ToLower());
    }

    /// <summary>
    /// Adds a page element to the collection.
    /// </summary>
    /// <param name="name">
    /// Unique name of the page element to register.
    /// </param>
    public void AddPageElement(string name)
    {
      this._registeredElements.Add(name.ToLower());
    }

    /// <summary>
    /// Adds the given CSS to the page header within a <![CDATA[<style>]]> tag
    /// </summary>
    /// <param name="element">
    /// Control to add element
    /// </param>
    /// <param name="name">
    /// Name of this element so it is not added more then once (not case sensitive)
    /// </param>
    /// <param name="cssContents">
    /// The contents of the text/css style block
    /// </param>
    public void RegisterCssBlock(Control element, string name, string cssContents)
    {
      if (!PageElementExists(name))
      {
        // Add to the end of the controls collection
        element.Controls.Add(ControlHelper.MakeCssControl(cssContents));
        AddPageElement(name);
      }
    }

    /// <summary>
    /// Registers a Javascript block using the script manager. Adds script tags.
    /// </summary>
    /// <param name="name">
    /// Unique name of JS Block
    /// </param>
    /// <param name="script">
    /// Script code to register
    /// </param>
    public void RegisterJsBlock(string name, string script)
    {
      RegisterJsBlock(YafContext.Current.CurrentForumPage, name, script);
    }

    /// <summary>
    /// Registers a Javascript block using the script manager. Adds script tags.
    /// </summary>
    /// <param name="thisControl">
    /// </param>
    /// <param name="name">
    /// </param>
    /// <param name="script">
    /// </param>
    public void RegisterJsBlock(Control thisControl, string name, string script)
    {
      if (!PageElementExists(name))
      {
        ScriptManager.RegisterClientScriptBlock(thisControl, thisControl.GetType(), name, JsAndCssHelper.CompressJavaScript(script), true);
      }
    }

    /// <summary>
    /// Registers a Javascript block using the script manager. Adds script tags.
    /// </summary>
    /// <param name="name">
    /// </param>
    /// <param name="script">
    /// </param>
    public void RegisterJsBlockStartup(string name, string script)
    {
      RegisterJsBlockStartup(YafContext.Current.CurrentForumPage, name, script);
    }

    /// <summary>
    /// Registers a Javascript block using the script manager. Adds script tags.
    /// </summary>
    /// <param name="thisControl">
    /// </param>
    /// <param name="name">
    /// </param>
    /// <param name="script">
    /// </param>
    public void RegisterJsBlockStartup(Control thisControl, string name, string script)
    {
      if (!PageElementExists(name))
      {
        ScriptManager.RegisterStartupScript(thisControl, thisControl.GetType(), name, JsAndCssHelper.CompressJavaScript(script), true);
      }
    }

    /// <summary>
    /// Registers a Javascript include using the script manager.
    /// </summary>
    /// <param name="thisControl">
    /// </param>
    /// <param name="name">
    /// </param>
    /// <param name="url">
    /// </param>
    public void RegisterJsInclude(Control thisControl, string name, string url)
    {
      if (!PageElementExists(name))
      {
        ScriptManager.RegisterClientScriptInclude(thisControl, thisControl.GetType(), name, url);
        AddPageElement(name);
      }
    }

    /// <summary>
    /// Registers a Javascript include using the script manager.
    /// </summary>
    /// <param name="name">
    /// </param>
    /// <param name="url">
    /// </param>
    public void RegisterJsInclude(string name, string url)
    {
      RegisterJsInclude(YafContext.Current.CurrentForumPage, name, url);
    }

    /// <summary>
    /// Registers a Javascript resource include using the script manager.
    /// </summary>
    /// <param name="thisControl">
    /// </param>
    /// <param name="name">
    /// </param>
    /// <param name="relativeResourceUrl">
    /// </param>
    public void RegisterJsResourceInclude(Control thisControl, string name, string relativeResourceUrl)
    {
      if (!PageElementExists(name))
      {
        ScriptManager.RegisterClientScriptInclude(thisControl, thisControl.GetType(), name, YafForumInfo.GetURLToResource(relativeResourceUrl));
        AddPageElement(name);
      }
    }

    /// <summary>
    /// Registers a Javascript resource include using the script manager.
    /// </summary>
    /// <param name="name">
    /// Unique name of the JS include
    /// </param>
    /// <param name="relativeResourceUrl">
    /// URL to the JS resource
    /// </param>
    public void RegisterJsResourceInclude(string name, string relativeResourceUrl)
    {
      RegisterJsResourceInclude(YafContext.Current.CurrentForumPage, name, relativeResourceUrl);
    }

    /// <summary>
    /// Register a CSS block in the page header.
    /// </summary>
    /// <param name="name">
    /// Unique name of the CSS block
    /// </param>
    /// <param name="cssContents">
    /// CSS contents
    /// </param>
    public void RegisterCssBlock(string name, string cssContents)
    {
      RegisterCssBlock(YafContext.Current.CurrentForumPage.TopPageControl, name, JsAndCssHelper.CompressCss(cssContents));
    }

    /// <summary>
    /// Add the given CSS to the page header within a style tag
    /// </summary>
    /// <param name="cssUrl">
    /// Url of the CSS file to add
    /// </param>
    public void RegisterCssInclude(string cssUrl)
    {
      RegisterCssInclude(YafContext.Current.CurrentForumPage.TopPageControl, cssUrl);
    }

    /// <summary>
    /// Add the given CSS to the page header within a style tag
    /// </summary>
    /// <param name="cssUrlResource">
    /// Url of the CSS Resource file to add
    /// </param>
    public void RegisterCssIncludeResource(string cssUrlResource)
    {
      RegisterCssInclude(YafContext.Current.CurrentForumPage.TopPageControl, YafForumInfo.GetURLToResource(cssUrlResource));
    }

    /// <summary>
    /// Adds the given CSS to the page header within a <![CDATA[<style>]]> tag
    /// </summary>
    /// <param name="element">
    /// Control to add element
    /// </param>
    /// <param name="cssUrl">
    /// Url of the CSS file to add
    /// </param>
    public void RegisterCssInclude(Control element, string cssUrl)
    {
      if (!PageElementExists(cssUrl))
      {
        element.Controls.Add(ControlHelper.MakeCssIncludeControl(cssUrl.ToLower()));
        AddPageElement(cssUrl);
      }
    }

    /// <summary>
    /// The register j query.
    /// </summary>
    public void RegisterJQuery()
    {
      RegisterJQuery(YafContext.Current.CurrentForumPage.TopPageControl);
    }

    /// <summary>
    /// The register j query ui.
    /// </summary>
    public void RegisterJQueryUI()
    {
      RegisterJQueryUI(YafContext.Current.CurrentForumPage.TopPageControl);
    }

    /// <summary>
    /// Register jQuery
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    public void RegisterJQuery(Control element)
    {
      if (PageElementExists("jquery"))
      {
        return;
      }

      bool registerJQuery = true;

      string key = "JQuery-Javascripts";

      // check to see if DotNetAge is around and has registered jQuery for us...
      if (HttpContext.Current.Items[key] != null)
      {
        var collection = HttpContext.Current.Items[key] as StringCollection;

        if (collection != null && collection.Contains("jquery"))
        {
          registerJQuery = false;
        }
      }
      else if (Config.IsDotNetNuke)
      {
        // latest version of DNN (v5) should register jQuery for us...
        registerJQuery = false;
      }

      if (registerJQuery)
      {
        // load jQuery
        element.Controls.Add(ControlHelper.MakeJsIncludeControl("js/jquery-1.4.2.min.js"));
      }

      AddPageElement("jquery");
    }

    /// <summary>
    /// Register the JQuery UI library in the header.
    /// </summary>
    /// <param name="element">
    /// Control element to put in
    /// </param>
    public void RegisterJQueryUI(Control element)
    {
      if (PageElementExists("jqueryui"))
      {
        return;
      }

      // requires jQuery first...
      if (!PageElementExists("jquery"))
      {
        RegisterJQuery(element);
      }

      // load jQuery UI from google...
      element.Controls.Add(ControlHelper.MakeJsIncludeControl("http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.3/jquery-ui.min.js"));

      AddPageElement("jqueryui");
    }
  }
}