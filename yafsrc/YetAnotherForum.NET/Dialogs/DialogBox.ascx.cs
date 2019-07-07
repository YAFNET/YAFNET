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

namespace YAF.Dialogs
{
    #region Using

    using System;
    using System.Text;
    using System.Web;

    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
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
                this.ViewState[key: "CancelButtonLink"] != null
                    ? (ForumLink)this.ViewState[key: "CancelButtonLink"]
                    : new ForumLink();

            set => this.ViewState[key: "CancelButtonLink"] = value;
        }

        /// <summary>
        ///   Gets or sets the Ok Button Link
        /// </summary>
        public ForumLink OkButtonLink
        {
            get =>
                this.ViewState[key: "OkButtonLink"] != null
                    ? (ForumLink)this.ViewState[key: "OkButtonLink"]
                    : new ForumLink();

            set => this.ViewState[key: "OkButtonLink"] = value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Open The Dialog
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="title">The Message title.</param>
        /// <param name="okButton">The ok button.</param>
        /// <param name="cancelButton">The cancel button.</param>
        public void Show(
            [NotNull] string message,
            [NotNull] string title,
            [NotNull] DialogButton okButton,
            [NotNull] DialogButton cancelButton)
        {
            // Message Header
            this.Header.Text = !string.IsNullOrEmpty(value: title)
                                   ? title
                                   : this.GetText(page: "COMMON", tag: "MODAL_NOTIFICATION_HEADER");

            // Message Text
            this.MessageText.Text = message;

            // OK/Yes Message Button
            if (okButton != null)
            {
                this.OkButtonLink = okButton.ForumPageLink ??
                                    new ForumLink { ForumPage = YafContext.Current.ForumPageType };

                if (okButton.Text.IsSet())
                {
                    this.OkButton.Text = okButton.Text;
                }
                else
                {
                    okButton.Text = this.GetText(page: "COMMON", tag: "OK");
                }

                this.OkButton.CssClass = okButton.CssClass.IsSet() ? okButton.CssClass : "btn btn-primary";
            }

            // Cancel/No Message Button
            if (cancelButton != null)
            {
                this.CancelButtonLink = cancelButton.ForumPageLink ??
                                        new ForumLink { ForumPage = YafContext.Current.ForumPageType };

                this.CancelButton.Visible = true;

                this.CancelButton.Text = cancelButton.Text.IsSet()
                                             ? cancelButton.Text
                                             : this.GetText(page: "COMMON", tag: "CANCEL");

                this.CancelButton.CssClass = cancelButton.CssClass.IsSet() ? cancelButton.CssClass : "btn btn-secondary";

                if (this.CancelButtonLink.ForumPage.Equals(obj: YafContext.Current.ForumPageType))
                {
                    this.CancelButton.OnClientClick =
                        $"jQuery('#{this.YafForumPageErrorPopup.ClientID}').modal('hide');return false;";
                }
                else
                {
                    this.CancelButton.OnClientClick =
                        $"jQuery('#{this.YafForumPageErrorPopup.ClientID}').modal('hide');";
                }
            }
            else
            {
                this.CancelButton.Visible = false;

                this.CancelButtonLink = new ForumLink { ForumPage = YafContext.Current.ForumPageType };
            }

            var script = new StringBuilder();

            script.Append(value: "Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(ShowNotificationPopup);");

            script.AppendFormat(
                format: "function ShowNotificationPopup() {{ jQuery(document).ready(function() {{jQuery('#{0}').modal('show');return false; }});}}",
                arg0: this.YafForumPageErrorPopup.ClientID);

            this.PageContext.PageElements.RegisterJsBlockStartup(thisControl: this.Page, name: $"PopUp{Guid.NewGuid()}", script: script.ToString());

            if (this.OkButtonLink.ForumPage.Equals(obj: YafContext.Current.ForumPageType))
            {
                this.OkButton.OnClientClick =
                    $"jQuery('#{this.YafForumPageErrorPopup.ClientID}').modal('hide');return false;";
            }
            else
            {
                this.OkButton.OnClientClick = $"jQuery('#{this.YafForumPageErrorPopup.ClientID}').modal('hide');";
            }
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
            if (this.CancelButtonLink.ForumPage.Equals(obj: YafContext.Current.ForumPageType))
            {
                // Make Sure the Current Page is correctly Returned with all querystrings
                this.Get<HttpResponseBase>().Redirect(url: this.Get<HttpRequestBase>().Url.ToString());
            }
            else
            {
                if (this.CancelButtonLink.ForumLinkFormat.IsSet() &&
                    !this.CancelButtonLink.ForumLinkArgs.IsNullOrEmptyDBField())
                {
                    YafBuildLink.Redirect(
                        page: this.CancelButtonLink.ForumPage,
                        format: this.CancelButtonLink.ForumLinkFormat,
                        args: this.CancelButtonLink.ForumLinkArgs);
                }
                else
                {
                    YafBuildLink.Redirect(page: this.CancelButtonLink.ForumPage);
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
            if (this.OkButtonLink.ForumPage.Equals(obj: YafContext.Current.ForumPageType))
            {
                // Make Sure the Current Page is correctly Returned with all querystrings
                this.Get<HttpResponseBase>().Redirect(url: this.Get<HttpRequestBase>().Url.ToString());
            }
            else
            {
                if (this.OkButtonLink.ForumLinkFormat.IsSet() && !this.OkButtonLink.ForumLinkArgs.IsNullOrEmptyDBField())
                {
                    YafBuildLink.Redirect(
                        page: this.OkButtonLink.ForumPage, format: this.OkButtonLink.ForumLinkFormat, args: this.OkButtonLink.ForumLinkArgs);
                }
                else
                {
                    YafBuildLink.Redirect(page: this.OkButtonLink.ForumPage);
                }
            }
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // this.Visible = false;
        }

        #endregion

        /// <summary>
        /// Dialog Button Class
        /// </summary>
        public class DialogButton
        {
            #region Properties

            /// <summary>
            ///   Gets or sets the Button Css Class
            /// </summary>
            public string CssClass { get; set; }

            /// <summary>
            ///   Gets or sets the Forum Link
            /// </summary>
            public ForumLink ForumPageLink { get; set; }

            /// <summary>
            ///   Gets or sets Button Text
            /// </summary>
            public string Text { get; set; }

            #endregion
        }

        /// <summary>
        /// Forum Link With Parameters
        /// </summary>
        [Serializable]
        public class ForumLink
        {
            #region Constants and Fields

            /// <summary>
            ///   Gets or sets ForumLinkArgs.
            /// </summary>
            public object[] ForumLinkArgs;

            /// <summary>
            ///   Gets or sets ForumLinkFormat.
            /// </summary>
            public string ForumLinkFormat = string.Empty;

            /// <summary>
            ///   Gets ors sets the Yaf Forum Page Link
            /// </summary>
            public ForumPages ForumPage = YafContext.Current.ForumPageType;

            #endregion
        }
    }
}