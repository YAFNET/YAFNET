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

namespace YAF.Types.Objects
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Serialization;

    /// <summary>
    /// The language resources page resource.
    /// </summary>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class LanguageResourcesPageResource
    {
        #region Constants and Fields

        /// <summary>
        /// The edit type field.
        /// </summary>
        private string editTypeField;

        /// <summary>
        /// The tag field.
        /// </summary>
        private string tagField;

        /// <summary>
        /// The value field.
        /// </summary>
        private string valueField;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [XmlText]
        public string Value
        {
            get => this.valueField;

            set => this.valueField = value;
        }

        /// <summary>
        /// Gets or sets the edit type.
        /// </summary>
        [XmlAttribute]
        public string editType
        {
            get => this.editTypeField;

            set => this.editTypeField = value;
        }

        /// <summary>
        /// Gets or sets tag.
        /// </summary>
        [XmlAttribute]
        public string tag
        {
            get => this.tagField;

            set => this.tagField = value;
        }

        #endregion
    }
}