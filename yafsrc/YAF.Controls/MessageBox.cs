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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The MessagBox
    /// </summary>
    public class MessageBox : BaseControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The buttons.
        /// </summary>
        private List<HyperLink> buttons;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the Body Template
        /// </summary>
        public ITemplate BodyTemplate { get; set; }

        /// <summary>
        ///   Gets Buttons.
        /// </summary>
        [NotNull]
        public List<HyperLink> Buttons
        {
            get
            {
                return this.buttons ?? (this.buttons = new List<HyperLink>());
            }
        }

        /// <summary>
        ///   Gets or sets CssClass.
        /// </summary>
        [NotNull]
        public string CssClass
        {
            get
            {
                return this.ViewState["CssClass"] != null ? this.ViewState["CssClass"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["CssClass"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets Title.
        /// </summary>
        public string Icon
        {
            get
            {
                return this.ViewState["Icon"] != null
                           ? this.ViewState["Icon"].ToString()
                           : YafForumInfo.GetURLToResource("icons/InfoBig.png");
            }

            set
            {
                this.ViewState["Icon"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets Title.
        /// </summary>
        [NotNull]
        public string MessageText
        {
            get
            {
                return this.ViewState["MessageText"] != null ? this.ViewState["MessageText"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["MessageText"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets Title.
        /// </summary>
        [NotNull]
        public string Title
        {
            get
            {
                return this.ViewState["Title"] != null ? this.ViewState["Title"].ToString() : "Info";
            }

            set
            {
                this.ViewState["Title"] = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create default box.
        /// </summary>
        /// <param name="template">
        /// The template.
        /// </param>
        protected virtual void CreateBox([NotNull] Control template)
        {
            /*HtmlGenericControl spanInnerMessage = new HtmlGenericControl("span");

                  HtmlGenericControl divOuterMessage = new HtmlGenericControl("div") { ID = "YafPopupErrorMessageOuter" };

                  divOuterMessage.Attributes.Add("class", "modalOuter");

                  spanInnerMessage.ID = "YafPopupErrorMessageInner";
                  spanInnerMessage.Attributes.Add("class", "modalInner");

                  spanInnerMessage.InnerText = "Error";

                  divOuterMessage.Controls.Add(spanInnerMessage);

                  template.Controls.Add(divOuterMessage);*/
        }

        /// <summary>
        /// Overrides the <see cref="Control.CreateChildControls"/> method.
        /// </summary>
        protected override void CreateChildControls()
        {
            var titleControl = new HtmlGenericControl("div");

            titleControl.Attributes.Add("class", "modalHeader");

            titleControl.InnerHtml = "<h3>{0}</h3>".FormatWith(this.Title);

            this.Controls.Add(titleControl);

            this.Controls.Add(new LiteralControl("<div style=\"float:left\">"));

            var imageIcon = new HtmlImage { Src = this.Icon, Alt = "Icon", };

            imageIcon.Attributes.Add("class", "DialogIcon");
            imageIcon.Attributes.Add("style", "padding:5px");

            this.Controls.Add(imageIcon);

            this.Controls.Add(new LiteralControl("</div>"));

            if (this.MessageText.IsSet())
            {
                var spanInnerMessage = new HtmlGenericControl("span");

                var divOuterMessage = new HtmlGenericControl("div") { ID = "YafPopupErrorMessageOuter" };

                divOuterMessage.Attributes.Add("class", "modalOuter");

                spanInnerMessage.ID = "YafPopupErrorMessageInner";
                spanInnerMessage.Attributes.Add("class", "modalInner");

                spanInnerMessage.InnerText = this.MessageText;

                divOuterMessage.Controls.Add(spanInnerMessage);

                this.Controls.Add(divOuterMessage);
            }

            if (!this.DesignMode)
            {
                if (this.BodyTemplate == null)
                {
                    this.BodyTemplate = new CompiledTemplateBuilder(this.CreateBox);
                }

                if (!this.DesignMode)
                {
                    this.BodyTemplate.InstantiateIn(this);
                }
            }

            this.Controls.Add(new LiteralControl("<hr />"));

            if (this.Buttons != null && this.Buttons.Count > 0)
            {
                var divFooter = new HtmlGenericControl("div") { ID = "YafModalFooter" };
                divFooter.Attributes.Add("class", "modalFooter");

                this.Controls.Add(divFooter);

                foreach (HyperLink btnLink in
                    this.Buttons.Select(
                        btn =>
                        new HyperLink
                            {
                                CssClass = btn.CssClass, ID = Guid.NewGuid().ToString(), Text = btn.Text
                            }))
                {
                    // btnLink.NavigateUrl = this.Page.ClientScript.GetPostBackClientHyperlink(btnLink, string.Empty);
                    btnLink.NavigateUrl = "#";
                    btnLink.Attributes.Add(
                        "onclick", "jQuery(this).YafModalDialog.Close({{ Dialog: '#{0}' }});".FormatWith(this.ClientID));

                    divFooter.Controls.Add(btnLink);
                }
            }

            this.Controls.Add(new LiteralControl("</div>"));
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            writer.BeginRender();

            writer.WriteBeginTag("div");
            writer.WriteAttribute("id", this.ClientID);

            writer.WriteAttribute("class", "MessageBox");

            writer.Write(">");
            writer.WriteLine();

            writer.EndRender();

            base.Render(writer);
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // Setup JS
            YafContext.Current.PageElements.RegisterJQuery();
            YafContext.Current.PageElements.RegisterJsResourceInclude("yafmodaldialog", "js/jquery.yafmodaldialog.js");
            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.yafmodaldialog.css");

            base.OnPreRender(e);
        }

        #endregion
    }
}