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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// The resources page.
    /// </summary>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class LanguageResourcesPage
    {
        #region Constants and Fields

        /// <summary>
        /// The name field.
        /// </summary>
        private string nameField;

        /// <summary>
        /// The resource field.
        /// </summary>
        private List<LanguageResourcesPageResource> resourceField;

        #endregion

        #region Properties

        /// <summary>
        /// The resource.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement("Resource", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public List<LanguageResourcesPageResource> Resource
        {
            get => this.resourceField;

            set => this.resourceField = value;
        }

        /// <summary>
        /// The name.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlAttribute]
        public string name
        {
            get => this.nameField;

            set => this.nameField = value;
        }

        #endregion
    }
}