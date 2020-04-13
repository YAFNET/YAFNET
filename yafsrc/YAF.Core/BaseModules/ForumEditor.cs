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

namespace YAF.Core.BaseModules
{
    #region Using

    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The Base ForumEditor Class
    /// </summary>
    public abstract class ForumEditor : BaseControl, IEditorModule
    {
        #region Properties
        
        /// <summary>
        ///   Gets a value indicating whether Active.
        /// </summary>
        public abstract bool Active { get; }

        /// <summary>
        ///   Gets Description.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        ///   Gets ModuleId.
        /// </summary>
        public virtual string ModuleId => this.Description.GetHashCode().ToString();

        /// <summary>
        ///   Gets or sets Text.
        /// </summary>
        public abstract string Text { get; set; }

        /// <summary>
        ///   Gets a value indicating whether UsesBBCode.
        /// </summary>
        public virtual bool UsesBBCode => false;

        /// <summary>
        ///   Gets a value indicating whether UsesHTML.
        /// </summary>
        public virtual bool UsesHTML => false;

        /// <summary>
        /// Gets a value indicating whether [allows uploads].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allows uploads]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AllowsUploads => false;

        /// <summary>
        /// Gets or sets the max characters.
        /// </summary>
        public virtual int MaxCharacters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [user can upload].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow uploads]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool UserCanUpload { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The add editor control.
        /// </summary>
        /// <param name="editor">
        /// The editor.
        /// </param>
        protected virtual void AddEditorControl([NotNull] Control editor)
        {
            var newDiv = new HtmlGenericControl("div") { ID = "EditorDiv" };
            newDiv.Attributes.Add("class", "EditorDiv");
            newDiv.Controls.Add(editor);
            this.Controls.Add(newDiv);
        }

        #endregion
    }
}