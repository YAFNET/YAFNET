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
    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// The CKEditor BBCode editor.
    /// </summary>
    public class CKEditorBBCodeEditor : CKEditor
    {
        #region Properties

        /// <summary>
        ///   Gets Description.
        /// </summary>
        [NotNull]
        public override string Description => "CKEditor (BBCode) - Full";

        /// <summary>
        ///   Gets ModuleId.
        /// </summary>
        public override string ModuleId => "4";

        /// <summary>
        ///   Gets a value indicating whether UsesBBCode.
        /// </summary>
        public override bool UsesBBCode => true;

        /// <summary>
        ///   Gets a value indicating whether UsesHTML.
        /// </summary>
        public override bool UsesHTML => false;

        /// <summary>
        /// The allows uploads.
        /// </summary>
        public override bool AllowsUploads => true;

        #endregion

        #region Methods

        /// <summary>
        /// The register CKEditor custom JS.
        /// </summary>
        protected override void RegisterCKEditorCustomJS()
        {
            var toolbar = this.Get<BoardSettings>().EditorToolbarFull;

            if (!(this.Get<BoardSettings>().EnableAlbum && this.PageContext.UsrAlbums > 0
                                                        && this.PageContext.NumAlbums > 0))
            {
                // remove albums
                toolbar = toolbar.Replace(", \"albumsbrowser\"", string.Empty);
            }

            BoardContext.Current.PageElements.RegisterJsBlock(
                "ckeditorinitbbcode",
                JavaScriptBlocks.CKEditorLoadJs(
                    this.TextAreaControl.ClientID,
                    BoardContext.Current.CultureUser.IsSet()
                        ? BoardContext.Current.CultureUser.Substring(0, 2)
                        : this.Get<BoardSettings>().Culture.Substring(0, 2),
                    this.MaxCharacters,
                    this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css"),
                    BoardInfo.GetURLToContent("forum.min.css"),
                    toolbar));
        }

        #endregion
    }
}