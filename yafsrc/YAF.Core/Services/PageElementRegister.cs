/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Core.Services
{
    #region Using

    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Helper Class providing functions to register page elements.
    /// </summary>
    public class PageElementRegister
    {
        #region Constants and Fields

        /// <summary>
        ///   The _registered elements.
        /// </summary>
        private readonly List<string> _registeredElements = new List<string>();

        #endregion

        #region Properties

        /// <summary>
        ///   Gets elements (using in the head or header) that are registered on the page.
        ///   Used mostly by RegisterPageElementHelper.
        /// </summary>
        public List<string> RegisteredElements
        {
            get
            {
                return this._registeredElements;
            }
        }

        #endregion

        #region Public Methods

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
            if (this.PageElementExists(name))
            {
                return;
            }

            // Add to the end of the controls collection
            element.Controls.Add(ControlHelper.MakeCssControl(cssContents));
            this.AddPageElement(name);
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
            this.RegisterCssBlock(
              YafContext.Current.CurrentForumPage.TopPageControl, name, JsAndCssHelper.CompressCss(cssContents));
        }

        /// <summary>
        /// Add the given CSS to the page header within a style tag
        /// </summary>
        /// <param name="cssUrl">
        /// Url of the CSS file to add
        /// </param>
        public void RegisterCssInclude(string cssUrl)
        {
            this.RegisterCssInclude(YafContext.Current.CurrentForumPage.TopPageControl, cssUrl);
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
            if (this.PageElementExists(cssUrl))
            {
                return;
            }

            element.Controls.Add(ControlHelper.MakeCssIncludeControl(cssUrl.ToLower()));
            this.AddPageElement(cssUrl);
        }

        /// <summary>
        /// Add the given CSS to the page header within a style tag
        /// </summary>
        /// <param name="cssUrlResource">
        /// Url of the CSS Resource file to add
        /// </param>
        public void RegisterCssIncludeResource(string cssUrlResource)
        {
            this.RegisterCssInclude(
              YafContext.Current.CurrentForumPage.TopPageControl, YafForumInfo.GetURLToResource(cssUrlResource));
        }

        /// <summary>
        /// The register j query.
        /// </summary>
        public void RegisterJQuery()
        {
            this.RegisterJQuery(YafContext.Current.CurrentForumPage.TopPageControl);
        }

        /// <summary>
        /// Register jQuery
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        public void RegisterJQuery(Control element)
        {
            if (this.PageElementExists("jquery") || Config.DisableJQuery)
            {
                return;
            }

            bool registerJQuery = true;

            const string Key = "JQuery-Javascripts";

            // check to see if DotNetAge is around and has registered jQuery for us...
            if (HttpContext.Current.Items[Key] != null)
            {
                var collection = HttpContext.Current.Items[Key] as StringCollection;

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
                var url = Config.JQueryFile;

                if (!url.StartsWith("http") && !url.StartsWith("//"))
                {
                    url = YafForumInfo.GetURLToResource(Config.JQueryFile);
                }

                // load jQuery
                element.Controls.Add(ControlHelper.MakeJsIncludeControl(url));
            }

            this.AddPageElement("jquery");
        }

        /// <summary>
        /// Registers the jQuery-UI Library and Loads the jQuery-UI Theme CSS File.
        /// </summary>
        public void RegisterJQueryUI()
        {
            this.RegisterJQueryUI(YafContext.Current.CurrentForumPage.TopPageControl);

            // Register the jQueryUI Theme CSS
            if (YafContext.Current.Get<YafBoardSettings>().JqueryUIThemeCDNHosted)
            {
                this.RegisterCssInclude(
                    "//ajax.googleapis.com/ajax/libs/jqueryui/1/themes/{0}/jquery-ui.min.css".FormatWith(
                        YafContext.Current.Get<YafBoardSettings>().JqueryUITheme));
            }
            else
            {
                this.RegisterCssIncludeResource(
                     "css/jquery-ui-themes/{0}/jquery-ui.min.css".FormatWith(
                         YafContext.Current.Get<YafBoardSettings>().JqueryUITheme));
            }
        }

        /// <summary>
        /// Register the JQuery UI library in the header.
        /// </summary>
        /// <param name="element">
        /// Control element to put in
        /// </param>
        public void RegisterJQueryUI(Control element)
        {
            // If registered or told not to register, don't bother
            if (this.PageElementExists("jqueryui") || Config.DisableJQuery || Config.IsDotNetNuke)
            {
                return;
            }

            // requires jQuery first...
            this.RegisterJQuery(element);

            var url = Config.JQueryUIFile;

            if (!url.StartsWith("http") && !url.StartsWith("//"))
            {
                url = YafForumInfo.GetURLToResource(Config.JQueryUIFile);
            }

            // load jQuery UI from google...
            element.Controls.Add(
              ControlHelper.MakeJsIncludeControl(url));

            this.AddPageElement("jqueryui");
        }

        /// <summary>
        /// Registers the J query UI.
        /// </summary>
        public void RegisterJQueryUILanguageFile()
        {
            this.RegisterJQueryUILanguageFile(YafContext.Current.CurrentForumPage.TopPageControl);
        }

        /// <summary>
        /// Register the JQuery UI library in the header.
        /// </summary>
        /// <param name="element">
        /// Control element to put in
        /// </param>
        public void RegisterJQueryUILanguageFile(Control element)
        {
            // If registered or told not to register, don't bother
            if (this.PageElementExists("datepickerlang"))
            {
                return;
            }

            var url = !Config.JQueryUILangFile.StartsWith("http") && !Config.JQueryUILangFile.StartsWith("//")
                      ? YafForumInfo.GetURLToResource(Config.JQueryUILangFile)
                      : Config.JQueryUILangFile;

            // load jQuery UI from google...
            element.Controls.Add(
              ControlHelper.MakeJsIncludeControl(url));

            this.AddPageElement("datepickerlang");
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
            this.RegisterJsBlock(YafContext.Current.CurrentForumPage, name, script);
        }

        /// <summary>
        /// Registers a Javascript block using the script manager. Adds script tags.
        /// </summary>
        /// <param name="thisControl">
        /// The this Control.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="script">
        /// The script.
        /// </param>
        public void RegisterJsBlock(Control thisControl, string name, string script)
        {
            if (!this.PageElementExists(name))
            {
                ScriptManager.RegisterClientScriptBlock(
                  thisControl, thisControl.GetType(), name, JsAndCssHelper.CompressJavaScript(script), true);
            }
        }

        /// <summary>
        /// Registers a Javascript block using the script manager. Adds script tags.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="script">
        /// The script.
        /// </param>
        public void RegisterJsBlockStartup(string name, string script)
        {
            this.RegisterJsBlockStartup(YafContext.Current.CurrentForumPage, name, script);
        }

        /// <summary>
        /// Registers a Javascript block using the script manager. Adds script tags.
        /// </summary>
        /// <param name="thisControl">
        /// The this Control.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="script">
        /// The script.
        /// </param>
        public void RegisterJsBlockStartup(Control thisControl, string name, string script)
        {
            if (!this.PageElementExists(name))
            {
                ScriptManager.RegisterStartupScript(
                  thisControl, thisControl.GetType(), name, JsAndCssHelper.CompressJavaScript(script), true);
            }
        }

        /// <summary>
        /// Registers a Javascript include using the script manager.
        /// </summary>
        /// <param name="thisControl">
        /// The this Control.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        public void RegisterJsInclude(Control thisControl, string name, string url)
        {
            if (this.PageElementExists(name))
            {
                return;
            }

            ScriptManager.RegisterClientScriptInclude(thisControl, thisControl.GetType(), name, url);
            this.AddPageElement(name);
        }

        /// <summary>
        /// Registers a Javascript include using the script manager.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        public void RegisterJsInclude(string name, string url)
        {
            this.RegisterJsInclude(YafContext.Current.CurrentForumPage, name, url);
        }

        /// <summary>
        /// Registers a Javascript resource include using the script manager.
        /// </summary>
        /// <param name="thisControl">
        /// The this Control.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="relativeResourceUrl">
        /// The relative Resource Url.
        /// </param>
        public void RegisterJsResourceInclude(Control thisControl, string name, string relativeResourceUrl)
        {
            if (this.PageElementExists(name))
            {
                return;
            }

            ScriptManager.RegisterClientScriptInclude(
                thisControl, thisControl.GetType(), name, YafForumInfo.GetURLToResource(relativeResourceUrl));
            this.AddPageElement(name);
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
            this.RegisterJsResourceInclude(YafContext.Current.CurrentForumPage, name, relativeResourceUrl);
        }

        #endregion
    }
}