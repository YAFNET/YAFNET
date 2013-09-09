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
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using YAF.Core;
	using YAF.Types;
	using YAF.Types.Extensions;
	using YAF.Utils;

	/// <summary>
	/// The popup panel.
	/// </summary>
	public class PopupPanel : Panel
	{
		#region Constants and Fields


		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PopupPanel"/> class.
		/// </summary>
		public PopupPanel()
		{
			this.Init += new EventHandler(this.PopupPanel_Init);
		}

		#endregion

		#region Properties

		/// <summary>
		///   Gets or sets Control.
		/// </summary>
		public string AttachToControl { get; set; }

		public bool AutoAttach
		{
			get
			{
				if (ViewState["AutoAttach"] == null)
				{
					return true;
				}

				return (bool)ViewState["AutoAttach"];
			}

			set
			{
				ViewState["AutoAttach"] = value;
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

		/// <summary>
		/// Gets InternalCssClass.
		/// </summary>
		protected virtual string InternalCssClass
		{
			get
			{
				return "yafpopuppanel";
			}
		}

		#endregion

		#region Methods

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
		/// The render.
		/// </summary>
		/// <param name="writer">
		/// The writer.
		/// </param>
		protected override void Render([NotNull] HtmlTextWriter writer)
		{
			if (!this.Visible)
			{
				return;
			}

			var begin =
				string.Format(
					@"<div class=""{1}"" id=""{0}"" style=""position:absolute;z-index:100;left:0;top:0;display:none;"">", 
					this.ClientID, 
					this.InternalCssClass);

			writer.WriteLine(begin);

			this.RenderChildren(writer);

			writer.WriteLine("</div>");
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (this.AutoAttach && this.AttachToControl.IsSet())
			{
				var attachedControl = this.Parent.FindControl(this.AttachToControl) as Control;
                var webControl = attachedControl as WebControl;
                var userLink = attachedControl as UserLink;

                if (webControl != null)
				{
                    this.Attach(webControl);	
				}
                else if (userLink != null)
				{
                    this.Attach(userLink);
				}
			}

		}

		/// <summary>
		/// The pop menu_ init.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void PopupPanel_Init([NotNull] object sender, [NotNull] EventArgs e)
		{
			// init the necessary js...
			YafContext.Current.PageElements.RegisterJsResourceInclude("yafjs", "js/yaf.js");
		}

		#endregion
	}
}