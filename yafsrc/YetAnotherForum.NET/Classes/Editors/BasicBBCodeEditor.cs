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

    using YAF.Classes.Editors;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The same as the TextEditor except it adds YAF BBCode support. Used for QuickReply
    ///   functionality.
    /// </summary>
    public class BasicBBCodeEditor : TextEditor
    {
        #region Properties

        /// <summary>
        ///   Gets Description.
        /// </summary>
        [NotNull]
        public override string Description => "Basic BBCode Editor";

        /// <summary>
        ///   Gets ModuleId.
        /// </summary>
        public override string ModuleId => "5";

        /// <summary>
        ///   Gets a value indicating whether UsesBBCode.
        /// </summary>
        public override bool UsesBBCode => true;

        #endregion

        #region Methods

        /// <summary>
        /// Handles the PreRender event of the Editor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void Editor_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            base.Editor_PreRender(sender, e);

            YafContext.Current.PageElements.RegisterJsInclude(
                "YafEditorJs",
#if DEBUG
                this.ResolveUrl("yafEditor/yafEditor.js"));
#else
                this.ResolveUrl("yafEditor/yafEditor.min.js"));
#endif

            YafContext.Current.PageElements.RegisterJsBlock(
                "CreateYafEditorJs",
                "var {0}=new yafEditor('{0}');\nfunction setStyle(style,option) {{\n{0}.FormatText(style,option);\n}}\nfunction insertAttachment(id,url) {{\n{0}.FormatText('attach', id);\n}}\n"
                    .FormatWith(this.SafeID));

            // register custom YafBBCode javascript (if there is any)
            // this call is supposed to be after editor load since it may use
            // JS variables created in editor_load...
            this.Get<IBBCode>().RegisterCustomBBCodePageElements(this.Page, this.GetType(), this.SafeID);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            base.OnInit(e);
            this._textCtl.Attributes.Add("class", "basicBBCodeEditor form-control");
        }

        #endregion
    }
}