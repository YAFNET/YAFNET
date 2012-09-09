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
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Albums Image List Selector Menu
    /// </summary>
    public class AlbumListPopMenu : BaseControl, IPostBackEventHandler
    {
        #region Constants and Fields

        /// <summary>
        ///   The _items.
        /// </summary>
        private readonly List<InternalAlbumListItem> _items = new List<InternalAlbumListItem>();

        /// <summary>
        ///   The _control.
        /// </summary>
        private string _control = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "AlbumListPopMenu" /> class.
        /// </summary>
        public AlbumListPopMenu()
        {
            Init += this.PopMenu_Init;
        }

        #endregion

        #region Events

        /// <summary>
        ///   The item click.
        /// </summary>
        public event PopEventHandler ItemClick;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Control.
        /// </summary>
        public string Control
        {
            get
            {
                return this._control;
            }

            set
            {
                this._control = value;
            }
        }

        /// <summary>
        ///   Gets ControlOnClick.
        /// </summary>
        public string ControlOnClick
        {
            get
            {
                return "yaf_popit('{0}')".FormatWith(this.ClientID);
            }
        }

        /// <summary>
        ///   Gets ControlOnMouseOver.
        /// </summary>
        public string ControlOnMouseOver
        {
            get
            {
                return "yaf_mouseover('{0}')".FormatWith(this.ClientID);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The add client script item with Icon.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="clientScript">
        /// The client script.
        /// </param>
        /// <param name="albumImage">
        /// The icon.
        /// </param>
        public void AddClientScriptItem(
            [NotNull] string description, [NotNull] string clientScript, [NotNull] string albumImage)
        {
            this._items.Add(new InternalAlbumListItem(description, null, clientScript, albumImage));
        }

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="ctl">
        /// The ctl.
        /// </param>
        public void Attach([NotNull] WebControl ctl)
        {
            ctl.Attributes["onclick"] = this.ControlOnClick;
            ctl.Attributes["onmouseover"] = this.ControlOnMouseOver;
        }

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="userLinkControl">
        /// The user link control.
        /// </param>
        public void Attach([NotNull] UserLink userLinkControl)
        {
            userLinkControl.OnClick = this.ControlOnClick;
            userLinkControl.OnMouseOver = this.ControlOnMouseOver;
        }

        /// <summary>
        /// The remove post back item.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        public void RemovePostBackItem([NotNull] string argument)
        {
            foreach (InternalAlbumListItem item in this._items.Where(item => item.PostBackArgument == argument))
            {
                this._items.Remove(item);
                break;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IPostBackEventHandler

        /// <summary>
        /// The raise post back event.
        /// </summary>
        /// <param name="eventArgument">
        /// The event argument.
        /// </param>
        public void RaisePostBackEvent([NotNull] string eventArgument)
        {
            if (this.ItemClick != null)
            {
                this.ItemClick(this, new PopEventArgs(eventArgument));
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            if (!Visible)
            {
                return;
            }

            var sb = new StringBuilder();
            sb.AppendFormat(
                @"<div class=""yafpopupmenu yafalbumlistmenu"" id=""{0}"" style=""position:absolute;z-index:100;left:0;top:0;display:none;"">",
                ClientID);

            sb.Append("<div id=\"AlbumsListBox\" class=\"content\">");
            sb.Append("<div id=\"AlbumsListPager\"></div>");
            sb.Append("<br style=\"clear:both;\" />");
            sb.Append("<div id=\"AlbumsPagerResult\">");
            sb.AppendFormat(
                "<p style=\"text-align:center\"><span>{1}</span><br /><img title=\"{1}\" src=\"{0}\" alt=\"{1}\" /></p>",
                YafForumInfo.GetURLToResource("images/loader.gif"),
                this.Get<ILocalization>().GetText("COMMON", "LOADING"));
            sb.Append("</div>");

            sb.Append("<div id=\"AlbumsPagerHidden\" style=\"display:none;\">");


            sb.Append("<div class=\"result\">");
            sb.AppendFormat("<ul class=\"AlbumImageList\">");

            int rowPanel = 0;

            for (int i = 0; i < this._items.Count; i++)
            {
                var thisItem = this._items[i];

                if (rowPanel == 3 && i < this._items.Count)
                {
                    sb.Append("</ul></div>");
                    sb.Append("<div class=\"result\">");
                    sb.Append("<ul class=\"AlbumImageList\">");

                    rowPanel = 0;
                }

                rowPanel++;

                var iconImage = string.Empty;

                var onClick = thisItem.ClientScript.IsSet()
                                  ? thisItem.ClientScript.Replace(
                                      "{postbackcode}",
                                      this.Page.ClientScript.GetPostBackClientHyperlink(this, thisItem.PostBackArgument))
                                  : this.Page.ClientScript.GetPostBackClientHyperlink(this, thisItem.PostBackArgument);

                if (thisItem.AlbumImage.IsSet())
                {
                    iconImage =
                        @"<img class=""popupitemIcon"" src=""{0}"" alt=""{1}"" title=""{1}"" />&nbsp;".FormatWith(
                            thisItem.AlbumImage, thisItem.Description);
                }

                sb.AppendFormat(
                    @"<li class=""popupitem"" onmouseover=""mouseHover(this,true)"" onmouseout=""mouseHover(this,false)"" onclick=""{2}"" style=""white-space:nowrap"" title=""{1}"">{0}</li>",
                    iconImage,
                    thisItem.Description,
                    onClick);
            }

            sb.Append("</ul>");
            sb.Append("</div>");

            sb.AppendFormat("</ul></div></div></div>");

            writer.WriteLine(sb.ToString());

            base.Render(writer);
        }

        /// <summary>
        /// The pop menu_ init.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PopMenu_Init([NotNull] object sender, [NotNull] EventArgs e)
        {
            // init the necessary js...
            PageContext.PageElements.RegisterJsResourceInclude("yafjs", "js/yaf.js");
        }

        #endregion
    }

    /// <summary>
    /// The internal pop menu item.
    /// </summary>
    public class InternalAlbumListItem
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalAlbumListItem"/> class.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="postbackArgument">
        /// The postback argument.
        /// </param>
        /// <param name="clientScript">
        /// The client script.
        /// </param>
        /// <param name="albumImage">
        /// The icon.
        /// </param>
        public InternalAlbumListItem(
            [NotNull] string description,
            [NotNull] string postbackArgument,
            [NotNull] string clientScript,
            [NotNull] string albumImage)
        {
            this.Description = description;
            this.PostBackArgument = postbackArgument;
            this.ClientScript = clientScript;
            this.AlbumImage = albumImage;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Icon.
        /// </summary>
        public string AlbumImage { get; set; }

        /// <summary>
        ///   Gets or sets ClientScript.
        /// </summary>
        public string ClientScript { get; set; }

        /// <summary>
        ///   Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///   Gets or sets PostBackArgument.
        /// </summary>
        public string PostBackArgument { get; set; }

        #endregion
    }
}