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
namespace YAF.Editors
{
    #region Using

    using System;
    using System.Reflection;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// The free text box editor.
    /// </summary>
    public class FreeTextBoxEditor : RichClassEditor
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "FreeTextBoxEditor" /> class.
        /// </summary>
        public FreeTextBoxEditor()
            : base("FreeTextBoxControls.FreeTextBox,FreeTextBox")
        {
            this.InitEditorObject();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Description.
        /// </summary>
        [NotNull]
        public override string Description
        {
            get
            {
                return "Free Text Box v2 (HTML)";
            }
        }

        /// <summary>
        ///   Gets ModuleId.
        /// </summary>
        public override string ModuleId
        {
            get
            {
                // backward compatibility...
                return "3";
            }
        }

        /// <summary>
        ///   Gets or sets Text.
        /// </summary>
        [NotNull]
        public override string Text
        {
            get
            {
                if (!this._init)
                {
                    return string.Empty;
                }

                PropertyInfo pInfo = this._typEditor.GetProperty("Text");
                return Convert.ToString(pInfo.GetValue(this._editor, null));
            }

            set
            {
                if (!this._init)
                {
                    return;
                }
                PropertyInfo pInfo = this._typEditor.GetProperty("Text");
                pInfo.SetValue(this._editor, value, null);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Editor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void Editor_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this._init || !this._editor.Visible)
            {
                return;
            }
            PropertyInfo pInfo = this._typEditor.GetProperty("SupportFolder");
            pInfo.SetValue(this._editor, this.ResolveUrl("FreeTextBox/"), null);
            pInfo = this._typEditor.GetProperty("Width");
            pInfo.SetValue(this._editor, Unit.Percentage(100), null);
            pInfo = this._typEditor.GetProperty("DesignModeCss");
            pInfo.SetValue(this._editor, this.StyleSheet, null);

            // pInfo = typEditor.GetProperty("EnableHtmlMode");
            // pInfo.SetValue(objEditor,false,null);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            if (this._init)
            {
                this.Load += this.Editor_Load;
                PropertyInfo pInfo = this._typEditor.GetProperty("ID");
                pInfo.SetValue(this._editor, "edit", null);
                pInfo = this._typEditor.GetProperty("AutoGenerateToolbarsFromString");
                pInfo.SetValue(this._editor, true, null);
                pInfo = this._typEditor.GetProperty("ToolbarLayout");
                pInfo.SetValue(
                    this._editor,
                    "FontFacesMenu,FontSizesMenu,FontForeColorsMenu;Bold,Italic,Underline|Cut,Copy,Paste,Delete,Undo,Redo|CreateLink,Unlink|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent",
                    null);

                this.AddEditorControl(this._editor);
            }

            base.OnInit(e);
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            base.OnPreRender(e);
        }

        #endregion
    }
}