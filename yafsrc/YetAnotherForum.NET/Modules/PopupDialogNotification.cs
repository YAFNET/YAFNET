/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Modules
{
    #region Using

    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The popup dialog notification.
    /// </summary>
    public class PopupDialogNotification : MessageBox
    {
        #region Constants and Fields

        /// <summary>
        ///   The _okay button.
        /// </summary>
        private readonly HyperLink _okayButton = new HyperLink();

        /// <summary>
        ///   The _template.
        /// </summary>
        private readonly ErrorPopupCustomTemplate _template = new ErrorPopupCustomTemplate();

        #endregion

        #region Properties

        /// <summary>
        ///   Gets MainTextClientID.
        /// </summary>
        [NotNull]
        public string MainTextClientID
        {
            get
            {
                return !string.IsNullOrEmpty(this._template.SpanInnerMessage.ClientID)
                           ? this._template.SpanInnerMessage.ClientID
                           : this.ClientID.Replace("YafForumPageErrorPopup1", "YafPopupErrorMessageInner");
            }
        }

        /// <summary>
        ///   Gets ShowModalFunction.
        /// </summary>
        public string ShowModalFunction
        {
            get
            {
                return "ShowPopupDialogNotification{0}".FormatWith(this.ClientID);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            // init the popup first...
            base.OnInit(e);

            this.BodyTemplate = this._template;

            this._okayButton.Text = "OK";
            this._okayButton.CssClass = "StandardButton";

            this.Buttons.Add(this._okayButton);
        }

        /// <summary>
        /// The on load.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnLoad([NotNull] EventArgs e)
        {
            base.OnLoad(e);
        }

        /// <summary>
        /// The on pre render.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this._okayButton.Text = this.GetText("COMMON", "OK");

            // add js for client-side error settings...
            string jsFunction;

            if (this.Get<YafBoardSettings>().MessageNotificationSystem.Equals(0))
            {
                // Show as Modal Dialog
                jsFunction =
                    @"function {2}(newErrorStr, newErrorType) {{  if (newErrorStr != null && newErrorStr != """" && {4}('#{1}') != null) {{
                     {4}('#{1}').html(newErrorStr);
                     {4}().YafModalDialog.Show({{Dialog : '#{0}',ImagePath : '{3}', Type : newErrorType}});
                }} }}"
                        .FormatWith(
                            this.ClientID,
                            this.MainTextClientID,
                            this.ShowModalFunction,
                            YafForumInfo.GetURLToResource("images/"),
                            Config.JQueryAlias);

                YafContext.Current.PageElements.RegisterJsResourceInclude(
                    "yafmodaldialog", "js/jquery.yafmodaldialog.js");
                YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.yafmodaldialog.css");
            }
            else
            {
                // Show as Notification Bar
                jsFunction =
                    @"function {2}(newErrorStr, newErrorType) {{ if (newErrorStr != null && newErrorStr != """") {{
                      showNotification({{
                            type : newErrorType,
                            message: newErrorStr,
                            autoClose: true,
                            duration: {4},
                            imagepath : '{3}'
                        }});}} }}"
                        .FormatWith(
                            this.ClientID,
                            this.MainTextClientID,
                            this.ShowModalFunction,
                            YafForumInfo.GetURLToResource("icons/"),
                            this.Get<YafBoardSettings>().MessageNotifcationDuration,
                            Config.JQueryAlias);

                YafContext.Current.PageElements.RegisterJsResourceInclude(
                    "yafnotification", "js/jquery.notification.js");
                YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.notification.css");
            }

            // Override Notification Setting if Mobile Device is used
            if (this.Get<YafBoardSettings>().NotifcationNativeOnMobile && this.Get<HttpRequestBase>().Browser.IsMobileDevice)
            {
                // Show as Modal Dialog
                jsFunction =
                    @"function {2}(newErrorStr) {{  if (newErrorStr != null && newErrorStr != """") {{ 
                                                    alert(newErrorStr);
                      }} }}"
                        .FormatWith(this.ClientID, this.MainTextClientID, this.ShowModalFunction);
            }

            YafContext.Current.PageElements.RegisterJsBlock(this, this.ShowModalFunction, jsFunction);

            base.OnPreRender(e);
        }

        #endregion

        /// <summary>
        /// The error popup custom template.
        /// </summary>
        public class ErrorPopupCustomTemplate : ITemplate
        {
            #region Constants and Fields

            /// <summary>
            ///   The DIV outer message.
            /// </summary>
            public HtmlGenericControl DivOuterMessage = new HtmlGenericControl("div");

            /// <summary>
            ///   The SPAN inner message.
            /// </summary>
            public HtmlGenericControl SpanInnerMessage = new HtmlGenericControl("span");

            #endregion

            #region Implemented Interfaces

            #region ITemplate

            /// <summary>
            /// The instantiate in.
            /// </summary>
            /// <param name="container">
            /// The container.
            /// </param>
            public void InstantiateIn([NotNull] Control container)
            {
                this.DivOuterMessage.ID = "YafPopupErrorMessageOuter";
                this.DivOuterMessage.Attributes.Add("class", "modalOuter");

                this.SpanInnerMessage.ID = "YafPopupErrorMessageInner";
                this.SpanInnerMessage.Attributes.Add("class", "modalInner");

                this.SpanInnerMessage.InnerText = "Error";

                this.DivOuterMessage.Controls.Add(this.SpanInnerMessage);

                container.Controls.Add(this.DivOuterMessage);
            }

            #endregion

            #endregion
        }
    }
}