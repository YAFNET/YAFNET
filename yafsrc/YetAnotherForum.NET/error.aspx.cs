/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF;

using System.Web.UI;

/// <summary>
/// Custom Error Page
/// </summary>
public partial class Error : Page
{
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

        // show error message if one was provided...
        if (this.Session["StartupException"] != null)
        {
            this.ErrorMessage.Text = this.Server.HtmlEncode(this.Session["StartupException"].ToString());

            this.Session["StartupException"] = null;
        }
        else if (this.Application["Exception"] != null)
        {
            this.ErrorDescriptionHolder.Visible = true;

            this.ErrorMessage.Text = this.Server.HtmlEncode(this.Application["ExceptionMessage"].ToString());
            this.ErrorDescription.Text = this.Server.HtmlEncode(this.Application["Exception"].ToString());

            this.Session["Exception"] = null;
            this.Session["ExceptionMessage"] = null;
        }
        else
        {
            this.ErrorMessage.Text =
                "There has been a serious error loading the forum. No further information is available.";
        }
    }
}