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
namespace YAF.Web.Editors
{
    #region Using

    using System;
    using System.Web.UI.WebControls;

    using YAF.Types;

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
        public override string Description => "Free Text Box v2 (HTML)";

        /// <summary>
        ///   Gets ModuleId.
        /// </summary>
        public override string ModuleId => "3";

        /// <summary>
        ///   Gets or sets Text.
        /// </summary>
        [NotNull]
        public override string Text
        {
            get
            {
                if (!this.IsInitialized)
                {
                    return string.Empty;
                }

                var propertyInfo = this.TypeEditor.GetProperty("Text");
                return Convert.ToString(propertyInfo.GetValue(this.Editor, null));
            }

            set
            {
                if (!this.IsInitialized)
                {
                    return;
                }

                var propertyInfo = this.TypeEditor.GetProperty("Text");
                propertyInfo.SetValue(this.Editor, value, null);
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
            if (!this.IsInitialized || !this.Editor.Visible)
            {
                return;
            }

            var propertyInfo = this.TypeEditor.GetProperty("SupportFolder");
            propertyInfo.SetValue(this.Editor, this.ResolveUrl("FreeTextBox/"), null);
            propertyInfo = this.TypeEditor.GetProperty("Width");
            propertyInfo.SetValue(this.Editor, Unit.Percentage(100), null);
            propertyInfo = this.TypeEditor.GetProperty("DesignModeCss");
            propertyInfo.SetValue(this.Editor, this.StyleSheet, null);

            // pInfo = typEditor.GetProperty("EnableHtmlMode");
            // pInfo.SetValue(objEditor,false,null);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            if (this.IsInitialized)
            {
                this.Load += this.Editor_Load;
                var propertyInfo = this.TypeEditor.GetProperty("ID");
                propertyInfo.SetValue(this.Editor, "edit", null);
                propertyInfo = this.TypeEditor.GetProperty("AutoGenerateToolbarsFromString");
                propertyInfo.SetValue(this.Editor, true, null);
                propertyInfo = this.TypeEditor.GetProperty("ToolbarLayout");
                propertyInfo.SetValue(
                    this.Editor,
                    "FontFacesMenu,FontSizesMenu,FontForeColorsMenu;Bold,Italic,Underline|Cut,Copy,Paste,Delete,Undo,Redo|CreateLink,Unlink|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent",
                    null);

                this.AddEditorControl(this.Editor);
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