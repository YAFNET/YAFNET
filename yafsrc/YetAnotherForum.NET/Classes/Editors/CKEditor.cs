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
    using System.Web.UI;

    using YAF.Types;

    #endregion

    /// <summary>
    /// The tiny mce editor.
    /// </summary>
    public abstract class CKEditor : TextEditor
    {
        #region Properties

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
        ///   Gets SafeID.
        /// </summary>
        [NotNull]
        protected new string SafeID
        {
            get
            {
                return this._textCtl.ClientID.Replace("$", "_");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The editor_ PreRender.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void Editor_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            ScriptManager.RegisterClientScriptInclude(
              this.Page, this.Page.GetType(), "ckeditor", this.ResolveUrl("ckeditor/ckeditor.js"));
            this.RegisterCKEditorCustomJS();
            this.RegisterSmilieyScript();
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            base.OnInit(e);

            this._textCtl.Attributes.CssStyle.Add("width", "100%");
            this._textCtl.Attributes.CssStyle.Add("height", "350px");
        }

        /// <summary>
        /// The register ckeditor custom js.
        /// </summary>
        protected abstract void RegisterCKEditorCustomJS();

        #endregion
    }
}