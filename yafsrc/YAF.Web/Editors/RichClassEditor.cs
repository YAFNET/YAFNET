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
    using System.Web.UI;

    using YAF.Core;
    using YAF.Types;

    #endregion

    /// <summary>
    /// The rich class editor.
    /// </summary>
    public abstract class RichClassEditor : ForumEditor
    {
        #region Constants and Fields

        /// <summary>
        ///   The style sheet.
        /// </summary>
        private string styleSheet;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RichClassEditor" /> class.
        /// </summary>
        protected RichClassEditor()
        {
            this.IsInitialized = false;
            this.styleSheet = string.Empty;
            this.Editor = null;
            this.TypeEditor = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichClassEditor"/> class.
        /// </summary>
        /// <param name="editorAssemblyName">
        /// The editor Assembly Name.
        /// </param>
        protected RichClassEditor([NotNull] string editorAssemblyName)
        {
            this.IsInitialized = false;
            this.styleSheet = string.Empty;
            this.Editor = null;

            this.TypeEditor = Type.GetType(editorAssemblyName, true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the editor.
        /// </summary>
        public Control Editor { get; private set; }

        /// <summary>
        /// Gets or sets the type editor.
        /// </summary>
        public Type TypeEditor { get; set; }

        /// <summary>
        ///   Gets a value indicating whether Active.
        /// </summary>
        public override bool Active => this.TypeEditor != null;

        /// <summary>
        ///   Gets a value indicating whether IsInitialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        ///   Gets or sets StyleSheet.
        /// </summary>
        public override string StyleSheet
        {
            get => this.styleSheet;

            set => this.styleSheet = value;
        }

        /// <summary>
        ///   Gets a value indicating whether UsesBBCode.
        /// </summary>
        public override bool UsesBBCode => false;

        /// <summary>
        ///   Gets a value indicating whether UsesHTML.
        /// </summary>
        public override bool UsesHTML => true;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the Editor Control
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool InitEditorObject()
        {
            try
            {
                if (!this.IsInitialized && this.TypeEditor != null)
                {
                    // create instance of main class
                    this.Editor = (Control)Activator.CreateInstance(this.TypeEditor);
                    this.IsInitialized = true;
                }
            }
            catch (Exception)
            {
                // dll is not accessible
                return false;
            }

            return true;
        }

        #endregion
    }
}