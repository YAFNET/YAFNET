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
namespace YAF.Core.Services
{
    #region Using

    using System.Collections.Generic;
    using System.Web;
    using System.Web.UI;

    using YAF.Core.Context;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Helper Class providing functions to register page elements.
    /// </summary>
    public class PageElementRegister
    {
        #region Properties

        /// <summary>
        ///   Gets elements (using in the head or header) that are registered on the page.
        ///   Used mostly by RegisterPageElementHelper.
        /// </summary>
        public List<string> RegisteredElements { get; } = new List<string>();

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
            this.RegisteredElements.Add(name.ToLower());
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
            return this.RegisteredElements.Contains(name.ToLower());
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
        /// The contents of the text/CSS style block
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
                BoardContext.Current.CurrentForumPage.TopPageControl,
                name,
                JsAndCssHelper.CompressCss(cssContents));
        }

        /// <summary>
        /// Add the given CSS to the page header within a style tag
        /// </summary>
        /// <param name="cssUrl">
        /// Url of the CSS file to add
        /// </param>
        public void RegisterCssInclude(string cssUrl)
        {
            this.RegisterCssInclude(BoardContext.Current.CurrentForumPage.TopPageControl, cssUrl);
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
        /// <param name="cssUrlContent">Content of the CSS URL.</param>
        public void RegisterCssIncludeContent(string cssUrlContent)
        {
            this.RegisterCssInclude(
                BoardContext.Current.CurrentForumPage.TopPageControl,
                BoardInfo.GetURLToContent(cssUrlContent));
        }

        /// <summary>
        /// Registers a Java Script block using the script manager. Adds script tags.
        /// </summary>
        /// <param name="name">
        /// Unique name of JS Block
        /// </param>
        /// <param name="script">
        /// Script code to register
        /// </param>
        public void RegisterJsBlock(string name, string script)
        {
            this.RegisterJsBlock(GetCurrentPage(), name, script);
        }

        /// <summary>
        /// Registers a Java Script block using the script manager. Adds script tags.
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
                ScriptManager.RegisterStartupScript(
                    thisControl,
                    thisControl.GetType(),
                    name,
                    JsAndCssHelper.CompressJavaScript(script),
                    true);
            }
        }

        /// <summary>
        /// Registers a Java Script block using the script manager. Adds script tags.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="script">
        /// The script.
        /// </param>
        public void RegisterJsBlockStartup(string name, string script)
        {
            this.RegisterJsBlockStartup(GetCurrentPage(), name, script);
        }

        /// <summary>
        /// Registers a Java Script block using the script manager. Adds script tags.
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
                    thisControl,
                    thisControl.GetType(),
                    name,
                    JsAndCssHelper.CompressJavaScript(script),
                    true);
            }
        }

        /// <summary>
        /// The add script reference.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void AddScriptReference(string name)
        {
            this.AddScriptReference(new ScriptReference { Name = name });
        }

        /// <summary>
        /// The add script reference.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        public void AddScriptReference(string name, string path)
        {
            ScriptManager.ScriptResourceMapping.AddDefinition(
                name,
                new ScriptResourceDefinition
                    {
                        Path = BoardInfo.GetURLToScripts(path)
                    });

            this.AddScriptReference(new ScriptReference { Name = name });
        }

        /// <summary>
        /// The add script reference.
        /// </summary>
        /// <param name="scriptReference">
        /// The script reference.
        /// </param>
        public void AddScriptReference(ScriptReference scriptReference)
        {
            var scriptManager = ScriptManager.GetCurrent(GetCurrentPage().Page);

            if (scriptReference.Name == "jquery")
            {
                scriptManager.Scripts.Insert(0, scriptReference);
            }
            else
            {
                scriptManager.Scripts.Add(scriptReference);
            }
        }

        #endregion

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <returns>Returns the current page</returns>
        private static Control GetCurrentPage()
        {
            return BoardContext.Current.CurrentForumPage ?? (Control)(HttpContext.Current.Handler as Page);
        }
    }
}