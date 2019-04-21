/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web.UI.WebControls;

    using YAF.Core;

    #endregion

    /// <summary>
    /// The localized custom validator.
    /// </summary>
    public class LocalizedCustomValidator : CustomValidator, ILocalizationSupport
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether EnableBBCode.
        /// </summary>
        public bool EnableBBCode { get; set; }

        /// <summary>
        /// Gets or sets LocalizedPage.
        /// </summary>
        public string LocalizedPage { get; set; }

        /// <summary>
        /// Gets or sets LocalizedTag.
        /// </summary>
        public string LocalizedTag { get; set; }

        /// <summary>
        /// Gets PageContext.
        /// </summary>
        public YafContext PageContext
        {
            get
            {
                if (this.Site != null && this.Site.DesignMode)
                {
                    // design-time, return null...
                    return null;
                }

                return YafContext.Current;
            }
        }

        /// <summary>
        /// Gets or sets Parameter 0.
        /// </summary>
        public string Param0 { get; set; }

        /// <summary>
        /// Gets or sets Parameter 1.
        /// </summary>
        public string Param1 { get; set; }

        /// <summary>
        /// Gets or sets Parameter 2.
        /// </summary>
        public string Param2 { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // localize ErrorMessage, ToolTip
            this.ErrorMessage = this.Localize(this);
            this.ToolTip = this.Localize(this);
        }

        #endregion
    }
}