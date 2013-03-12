/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using YAF.Core;
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
        #region Enums

        /// <summary>
        /// Dialog Icon
        /// </summary>
        public enum DialogIcon
        {
            /// <summary>
            ///   Mail Icon
            /// </summary>
            Mail = 1,

            /// <summary>
            ///   Info Icon
            /// </summary>
            Info = 2,

            /// <summary>
            ///   Error Icon
            /// </summary>
            Error = 3,

            /// <summary>
            ///   Warning Icon
            /// </summary>
            Warning = 4,

            /// <summary>
            ///   Question Icon
            /// </summary>
            Question = 5
        }
        
        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the Cancel Button Link
        /// </summary>
        public ForumLink CancelButtonLink
        {
            get
            {
                return this.ViewState["CancelButtonLink"] != null
                           ? (ForumLink)this.ViewState["CancelButtonLink"]
                           : new ForumLink();
            }

            set
            {
                this.ViewState["CancelButtonLink"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets the Ok Button Link
        /// </summary>
        public ForumLink OkButtonLink
        {
            get
            {
                return this.ViewState["OkButtonLink"] != null
                           ? (ForumLink)this.ViewState["OkButtonLink"]
                           : new ForumLink();
            }

            set
            {
                this.ViewState["OkButtonLink"] = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Open The Dialog
        /// </summary>
        /// <param name="message">
        /// The message text.
        /// </param>
        /// <param name="title">
        /// The Message title.
        /// </param>
        /// <param name="icon">
        /// The Message icon.
        /// </param>
        /// <param name="okButton">
        /// The ok button.
        /// </param>
        /// <param name="cancelButton">
        /// The cancel button.
        /// </param>
        public void Show(
            [NotNull] string message,
            [NotNull] string title,
            [CanBeNull] DialogIcon icon,
            [NotNull] DialogButton okButton,
            [NotNull] DialogButton cancelButton) 
        {
            // Message Header
            this.Header.Text = !string.IsNullOrEmpty(title)
                                   ? title
                                   : this.GetText("COMMON", "MODAL_NOTIFICATION_HEADER");

            // Message Text
            this.MessageText.Text = message;

            // Message Icon
            if (!icon.IsNullOrEmptyDBField())
            {
                switch (icon)
                {
                    case DialogIcon.Mail:
                        this.ImageIcon.ImageUrl = YafForumInfo.GetURLToResource("icons/EmailBig.png");
                        break;
                    case DialogIcon.Info:
                        this.ImageIcon.ImageUrl = YafForumInfo.GetURLToResource("icons/InfoBig.png");
                        break;
                    case DialogIcon.Warning:
                        this.ImageIcon.ImageUrl = YafForumInfo.GetURLToResource("icons/WarningBig.png");
                        break;
                    case DialogIcon.Error:
                        this.ImageIcon.ImageUrl = YafForumInfo.GetURLToResource("icons/ErrorBig.png");
                        break;
                    case DialogIcon.Question:
                        this.ImageIcon.ImageUrl = YafForumInfo.GetURLToResource("icons/QuestionBig.png");
                        break;
                }

                this.ImageIcon.Visible = true;
            }
            else
            {
                this.ImageIcon.Visible = false;
            }

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
                    okButton.Text = this.GetText("COMMON", "OK");
                }

                this.OkButton.CssClass = okButton.CssClass.IsSet() ? okButton.CssClass : "LoginButton";
            }

            // Cancel/No Message Button
            if (cancelButton != null)
            {
                this.CancelButtonLink = cancelButton.ForumPageLink ??
                                        new ForumLink { ForumPage = YafContext.Current.ForumPageType };

                this.CancelButton.Visible = true;

                this.CancelButton.Text = cancelButton.Text.IsSet()
                                             ? cancelButton.Text
                                             : this.GetText("COMMON", "CANCEL");

                this.CancelButton.CssClass = cancelButton.CssClass.IsSet() ? cancelButton.CssClass : "StandardButton CancelButton";

                if (this.CancelButtonLink.ForumPage.Equals(YafContext.Current.ForumPageType))
                {
                    this.CancelButton.OnClientClick =
                        "jQuery().YafModalDialog.Close({{ Dialog: '#{0}' }});return false;".FormatWith(
                            this.YafForumPageErrorPopup.ClientID);
                }
                else
                {
                    this.CancelButton.OnClientClick =
                        "jQuery().YafModalDialog.Close({{ Dialog: '#{0}' }});".FormatWith(
                            this.YafForumPageErrorPopup.ClientID);
                }
            }
            else
            {
                this.CancelButton.Visible = false;

                this.CancelButtonLink = new ForumLink { ForumPage = YafContext.Current.ForumPageType };
            }

            YafContext.Current.PageElements.RegisterJQuery();
            YafContext.Current.PageElements.RegisterJsResourceInclude("yafmodaldialog", "js/jquery.yafmodaldialog.js");
            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.yafmodaldialog.css");

            var sbScript = new StringBuilder();

            sbScript.Append("Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(ShowNotificationPopup);");

            sbScript.AppendFormat(
                "function ShowNotificationPopup() {{ jQuery(document).ready(function() {{jQuery().YafModalDialog.Show({{Dialog : '#{0}',ImagePath : '{1}'}});return false; }});}}",
                this.YafForumPageErrorPopup.ClientID,
                YafForumInfo.GetURLToResource("images/"));

            this.PageContext.PageElements.RegisterJsBlockStartup(this.Page, "PopUp{0}".FormatWith(Guid.NewGuid()), sbScript.ToString());

            if (this.OkButtonLink.ForumPage.Equals(YafContext.Current.ForumPageType))
            {
                this.OkButton.OnClientClick =
                    "jQuery().YafModalDialog.Close({{ Dialog: '#{0}' }});return false;".FormatWith(
                        this.YafForumPageErrorPopup.ClientID);
            }
            else
            {
                this.OkButton.OnClientClick =
                    "jQuery().YafModalDialog.Close({{ Dialog: '#{0}' }});".FormatWith(
                        this.YafForumPageErrorPopup.ClientID);
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
            if (this.CancelButtonLink.ForumPage.Equals(YafContext.Current.ForumPageType))
            {
                // Make Sure the Current Page is correctly Returned with all querystrings
                this.Get<HttpResponseBase>().Redirect(this.Get<HttpRequestBase>().Url.ToString());
            }
            else
            {
                if (this.CancelButtonLink.ForumLinkFormat.IsSet() &&
                    !this.CancelButtonLink.ForumLinkArgs.IsNullOrEmptyDBField())
                {
                    YafBuildLink.Redirect(
                        this.CancelButtonLink.ForumPage,
                        this.CancelButtonLink.ForumLinkFormat,
                        this.CancelButtonLink.ForumLinkArgs);
                }
                else
                {
                    YafBuildLink.Redirect(this.CancelButtonLink.ForumPage);
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
            if (this.OkButtonLink.ForumPage.Equals(YafContext.Current.ForumPageType))
            {
                // Make Sure the Current Page is correctly Returned with all querystrings
                this.Get<HttpResponseBase>().Redirect(this.Get<HttpRequestBase>().Url.ToString());
            }
            else
            {
                if (this.OkButtonLink.ForumLinkFormat.IsSet() && !this.OkButtonLink.ForumLinkArgs.IsNullOrEmptyDBField())
                {
                    YafBuildLink.Redirect(
                        this.OkButtonLink.ForumPage, this.OkButtonLink.ForumLinkFormat, this.OkButtonLink.ForumLinkArgs);
                }
                else
                {
                    YafBuildLink.Redirect(this.OkButtonLink.ForumPage);
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