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

namespace YAF
{
    #region Using

    using System;
    using System.Web.UI;

    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// Custom Error Page
    /// </summary>
    public partial class Error : Page
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            var errorMessage = @"There has been a serious error loading the forum. No further information is available.";

            // show error message if one was provided...
            if (this.Session["StartupException"] != null)
            {
                errorMessage =
                    "<strong>Error:</strong> {0}".FormatWith(
                        this.Server.HtmlEncode(this.Session["StartupException"].ToString()));

                this.Session["StartupException"] = null;
            }

            this.ErrorMessage.Text = errorMessage;
        }

        #endregion
    }
}