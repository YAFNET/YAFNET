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

    using System.Web.UI;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The UserLabel
    /// </summary>
    public class UserLabel : BaseControl
    {
        #region Properties

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
        ///   Gets or sets the onclick value for the profile link
        /// </summary>
        [NotNull]
        public string OnClick
        {
            get
            {
                return this.ViewState["OnClick"] != null ? this.ViewState["OnClick"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["OnClick"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets The onmouseover value for the profile link
        /// </summary>
        [NotNull]
        public string OnMouseOver
        {
            get
            {
                return this.ViewState["OnMouseOver"] != null ? this.ViewState["OnMouseOver"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["OnMouseOver"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets The name of the user for this profile link
        /// </summary>
        [NotNull]
        public string PostfixText
        {
            get
            {
                return this.ViewState["PostfixText"] != null ? this.ViewState["PostfixText"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["PostfixText"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets The replace Crawler name of this user for the link. Attention! Use it ONLY for crawlers. 
        /// </summary>
        [NotNull]
        public string CrawlerName
        {
            get
            {
                return this.ViewState["CrawlerName"] != null ? this.ViewState["CrawlerName"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["CrawlerName"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets Style.
        /// </summary>
        [NotNull]
        public string Style
        {
            get
            {
                return this.ViewState["Style"] != null ? this.ViewState["Style"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["Style"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets Style.
        /// </summary>
        [NotNull]
        public string ReplaceName
        {
            get
            {
                return this.ViewState["ReplaceName"] != null ? this.ViewState["ReplaceName"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["ReplaceName"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets The userid of this user for the link
        /// </summary>
        public int UserID
        {
            get
            {
                if (this.ViewState["UserID"] != null)
                {
                    return this.ViewState["UserID"].ToType<int>();
                }

                return -1;
            }

            set
            {
                this.ViewState["UserID"] = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter output)
        {
            string displayName;
            if (this.ReplaceName.IsNotSet())
            {
                displayName = this.Get<IUserDisplayName>().GetName(this.UserID);
            }
            else
            {
                displayName = this.ReplaceName;
            }

            if (this.UserID == -1 || displayName.IsNotSet())
            {
                return;
            }

            output.BeginRender();

            output.WriteBeginTag("span");

            this.RenderMainTagAttributes(output);

            output.Write(HtmlTextWriter.TagRightChar);

            displayName = this.CrawlerName.IsNotSet() ? displayName : this.CrawlerName;

            output.WriteEncodedText(this.CrawlerName.IsNotSet() ? displayName : this.CrawlerName);

            output.WriteEndTag("span");

            if (this.PostfixText.IsSet())
            {
                output.Write(this.PostfixText);
            }

            output.EndRender();
        }

        /// <summary>
        /// Renders "id", "style", "onclick", "onmouseover" and "class"
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected void RenderMainTagAttributes([NotNull] HtmlTextWriter output)
        {
            if (this.ClientID.IsSet())
            {
                output.WriteAttribute("id", this.ClientID);
            }

            if (this.Style.IsSet())
            {
                output.WriteAttribute("style", this.HtmlEncode(this.Style));
            }

            if (this.OnClick.IsSet())
            {
                output.WriteAttribute("onclick", this.OnClick);
            }

            if (this.OnMouseOver.IsSet())
            {
                output.WriteAttribute("onmouseover", this.OnMouseOver);
            }

            if (this.CssClass.IsSet())
            {
                output.WriteAttribute("class", this.CssClass);
            }
        }

        #endregion
    }
}