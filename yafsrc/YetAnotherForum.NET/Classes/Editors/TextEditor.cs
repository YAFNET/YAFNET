/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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
namespace YAF.Editors
{
    #region Using

    using System;
    using System.Web.UI.HtmlControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// The standard YAF text editor.
    /// </summary>
    public class TextEditor : ForumEditor
    {
        #region Constants and Fields

        /// <summary>
        ///   The Html Text Area
        /// </summary>
        protected HtmlTextArea _textCtl;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether Active.
        /// </summary>
        public override bool Active
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///   Gets the Editor Description.
        /// </summary>
        [NotNull]
        public override string Description
        {
            get
            {
                return "Plain Text Editor";
            }
        }

        /// <summary>
        ///   Gets the Module Id.
        /// </summary>
        public override string ModuleId
        {
            get
            {
                // backward compatibility...
                return "0";
            }
        }

        /// <summary>
        ///   Gets or sets Text.
        /// </summary>
        public override string Text
        {
            get
            {
                return this._textCtl.InnerText;
            }

            set
            {
                this._textCtl.InnerText = value;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether UsesBBCode.
        /// </summary>
        public override bool UsesBBCode
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether UsesHTML.
        /// </summary>
        public override bool UsesHTML
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets the Safe ID.
        /// </summary>
        [NotNull]
        protected string SafeID
        {
            get
            {
                return this._textCtl.ClientID.Replace("$", "_");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the PreRender event of the Editor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void Editor_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Ederon : 9/6/2007
            /*if (this.Visible || this.)
			{*/
            YafContext.Current.PageElements.RegisterJsInclude(
                "YafEditorJs",
#if DEBUG
                this.ResolveUrl("yafEditor/yafEditor.js"));
#else
                this.ResolveUrl("yafEditor/yafEditor.min.js"));
#endif


            YafContext.Current.PageElements.RegisterJsInclude(
                "CodeMirrorJs",
                this.ResolveUrl("ckeditor/plugins/codemirror/js/codemirror.min.js"));

            YafContext.Current.PageElements.RegisterJsInclude(
                "CodeMirrorBBCodeJs",
                this.ResolveUrl("ckeditor/plugins/codemirror/js/codemirror.mode.bbcode.min.js"));

            YafContext.Current.PageElements.RegisterJsBlock(
                "CreateYafEditorJs",
                "var {0}=new yafEditor('{0}');\nfunction setStyle(style,option) {{\n{0}.FormatText(style,option);\n}}\n"
                    .FormatWith(this.SafeID));

            YafContext.Current.PageElements.RegisterCssInclude(
                this.ResolveUrl("ckeditor/plugins/codemirror/css/codemirror.min.css"));

            this.RegisterSmilieyScript();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.PreRender += this.Editor_PreRender;

            this._textCtl = new HtmlTextArea { ID = "YafTextEditor", Rows = 15, Cols = 100 };
            this._textCtl.Attributes.Add("class", "YafTextEditor");

            this.AddEditorControl(this._textCtl);

            base.OnInit(e);
        }

        /// <summary>
        /// Registers the smiliey script.
        /// </summary>
        protected virtual void RegisterSmilieyScript()
        {
            YafContext.Current.PageElements.RegisterJsBlock(
                "InsertSmileyJs",
                "function insertsmiley(code) {{\n {0}.InsertSmiley(code);\n}}\n".FormatWith(this.SafeID));
        }

        #endregion
    }
}