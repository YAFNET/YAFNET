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

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    #region "Telerik RadEditor"

    /// <summary>
    /// The rad editor.
    /// </summary>
    public class RadEditor : RichClassEditor
    {
        // base("Namespace,AssemblyName")

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RadEditor" /> class.
        /// </summary>
        public RadEditor()
            : base("Telerik.Web.UI.RadEditor,Telerik.Web.UI")
        {
            this.InitEditorObject();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Description.
        /// </summary>
        [NotNull]
        public override string Description => "Telerik RAD Editor (HTML)";

        /// <summary>
        ///   Gets ModuleId.
        /// </summary>
        public override string ModuleId => "8";

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

                PropertyInfo pInfo = this._typEditor.GetProperty("Html");
                return Convert.ToString(pInfo.GetValue(this._editor, null));
            }

            set
            {
                if (!this._init)
                {
                    return;
                }

                PropertyInfo pInfo = this._typEditor.GetProperty("Html");
                pInfo.SetValue(this._editor, value, null);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The editor_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void Editor_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this._init || !this._editor.Visible)
            {
                return;
            }

            PropertyInfo pInfo = this._typEditor.GetProperty("ID");
            pInfo.SetValue(this._editor, "edit", null);
            pInfo = this._typEditor.GetProperty("Skin");

            pInfo.SetValue(this._editor, Config.RadEditorSkin, null);
            pInfo = this._typEditor.GetProperty("Height");

            pInfo.SetValue(this._editor, Unit.Pixel(400), null);
            pInfo = this._typEditor.GetProperty("Width");

            pInfo.SetValue(this._editor, Unit.Percentage(100), null);

            if (Config.UseRadEditorToolsFile)
            {
                pInfo = this._typEditor.GetProperty("ToolsFile");
                pInfo.SetValue(this._editor, Config.RadEditorToolsFile, null);
            }

            // Add Editor
            this.AddEditorControl(this._editor);
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            if (!this._init)
            {
                return;
            }

            this.Load += this.Editor_Load;
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

    #endregion
}