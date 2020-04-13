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

namespace YAF.Dialogs
{
    #region Using

    using System;
    using System.Text;
    using System.Web;

    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Modal Notification Dialog Box (Pop Up)
    /// </summary>
    public partial class DialogBox : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the Cancel Button Link
        /// </summary>
        public ForumLink CancelButtonLink
        {
            get =>
                this.ViewState["CancelButtonLink"] != null
                    ? (ForumLink)this.ViewState["CancelButtonLink"]
                    : new ForumLink(BoardContext.Current.ForumPageType);

            set => this.ViewState["CancelButtonLink"] = value;
        }

        /// <summary>
        ///   Gets or sets the Ok Button Link
        /// </summary>
        public ForumLink OkButtonLink
        {
            get =>
                this.ViewState["OkButtonLink"] != null
                    ? (ForumLink)this.ViewState["OkButtonLink"]
                    : new ForumLink(BoardContext.Current.ForumPageType);

            set => this.ViewState["OkButtonLink"] = value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Open The Dialog
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="title">The Message title.</param>
        /// <param name="okay">The ok button.</param>
        /// <param name="cancel">The cancel button.</param>
        public void Show(
            [NotNull] string message,
            [NotNull] string title,
            [NotNull] DialogButton okay,
            [NotNull] DialogButton cancel)
        {
            // Message Header
            this.Header.Text = title.IsSet() ? title : this.GetText("COMMON", "MODAL_NOTIFICATION_HEADER");

            // Message Text
            this.MessageText.Text = message;

            // OK/Yes Message Button
            if (okay != null)
            {
                this.OkButtonLink =
                    okay.ForumPageLink ?? new ForumLink(BoardContext.Current.ForumPageType);

                if (okay.Text.IsSet())
                {
                    this.OkButton.Text = okay.Text;
                }
                else
                {
                    okay.Text = this.GetText("COMMON", "OK");
                }

                this.OkButton.CssClass = okay.CssClass.IsSet() ? okay.CssClass : "btn btn-primary";
            }

            // Cancel/No Message Button
            if (cancel != null)
            {
                this.CancelButtonLink =
                    cancel.ForumPageLink ?? new ForumLink(BoardContext.Current.ForumPageType);

                this.CancelButton.Visible = true;

                this.CancelButton.Text = cancel.Text.IsSet()
                                             ? cancel.Text
                                             : this.GetText("COMMON", "CANCEL");

                this.CancelButton.CssClass = cancel.CssClass.IsSet() ? cancel.CssClass : "btn btn-secondary";
            }
            else
            {
                this.CancelButton.Visible = false;

                this.CancelButtonLink = new ForumLink(BoardContext.Current.ForumPageType);
            }

            var script = new StringBuilder();

            script.Append(
                "Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(ShowNotificationPopup);");

            script.AppendFormat(
                "function ShowNotificationPopup() {{ jQuery(document).ready(function() {{jQuery('#{0}').modal('show');return false; }});}}",
                this.YafForumPageErrorPopup.ClientID);

            this.PageContext.PageElements.RegisterJsBlockStartup(
                this.Page,
                $"PopUp{Guid.NewGuid()}",
                script.ToString());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when Cancel Button is Clicked
        /// </summary>
        /// <param name="sender">
        /// standard event object sender
        /// </param>
        /// <param name="e">
        /// event args
        /// </param>
        protected void CancelButton_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.CancelButtonLink.ForumPage.Equals(BoardContext.Current.ForumPageType))
            {
                // Make Sure the Current Page is correctly Returned with all query strings
                this.Get<HttpResponseBase>().Redirect(this.Get<HttpRequestBase>().Url.ToString());
            }
            else
            {
                if (this.CancelButtonLink.ForumLinkFormat.IsSet()
                    && !this.CancelButtonLink.ForumLinkArgs.IsNullOrEmptyDBField())
                {
                    BuildLink.Redirect(
                        this.CancelButtonLink.ForumPage,
                        this.CancelButtonLink.ForumLinkFormat,
                        this.CancelButtonLink.ForumLinkArgs);
                }
                else
                {
                    BuildLink.Redirect(this.CancelButtonLink.ForumPage);
                }
            }
        }

        /// <summary>
        /// Called when Ok Button is Clicked
        /// </summary>
        /// <param name="sender">
        /// standard event object sender
        /// </param>
        /// <param name="e">
        /// event args
        /// </param>
        protected void OkButton_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.OkButtonLink.ForumPage.Equals(BoardContext.Current.ForumPageType))
            {
                // Make Sure the Current Page is correctly Returned with all query strings
                this.Get<HttpResponseBase>().Redirect(this.Get<HttpRequestBase>().Url.ToString());
            }
            else
            {
                if (this.OkButtonLink.ForumLinkFormat.IsSet()
                    && !this.OkButtonLink.ForumLinkArgs.IsNullOrEmptyDBField())
                {
                    BuildLink.Redirect(
                        this.OkButtonLink.ForumPage,
                        this.OkButtonLink.ForumLinkFormat,
                        this.OkButtonLink.ForumLinkArgs);
                }
                else
                {
                    BuildLink.Redirect(this.OkButtonLink.ForumPage);
                }
            }
        }

        #endregion
    }
}