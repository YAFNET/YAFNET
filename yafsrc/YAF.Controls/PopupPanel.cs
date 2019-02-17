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

		#endregion
	}
}