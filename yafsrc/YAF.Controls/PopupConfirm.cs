/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;

namespace YAF.Controls
{
	public class PopupConfirm : System.Web.UI.Control
	{
		protected string _confirmText;
		protected string _behaviorID;
		protected ModalPopupExtender _popupControlExtender = new ModalPopupExtender();

		public PopupConfirm()
			: base()
		{
		}

		public ModalPopupExtender ConfirmExtender
		{
			get { return _popupControlExtender; }
		}

		public void Show()
		{
			ConfirmExtender.Show();
		}

		public void Hide()
		{
			ConfirmExtender.Hide();
		}

		protected override void OnInit( EventArgs e )
		{
			this.BuildPopup();
			base.OnInit( e );
		}

		protected void BuildPopup()
		{
			Panel popupPanel = new Panel();
			popupPanel.ID = "PopUpPanel";

			System.Web.UI.WebControls.Button okButton = new Button();
			okButton.ID = "PopUpOKBtn";
			okButton.Text = "Ok";

			System.Web.UI.WebControls.Button cancelButton = new Button();
			cancelButton.ID = "PopUpCancelBtn";
			cancelButton.Text = "Cancel";

			okButton.Click += new EventHandler( okButton_Click );
			cancelButton.Click += new EventHandler( cancelButton_Click );

			HtmlGenericControl span = new HtmlGenericControl( "span" );
			span.Attributes.Add( "style", "display:none" );
			HtmlInputButton hiddenButton = new HtmlInputButton();
			hiddenButton.ID = "coolio1";
			span.Controls.Add( hiddenButton );
			popupPanel.Controls.Add( span );

			System.Web.UI.HtmlControls.HtmlGenericControl div = new HtmlGenericControl( "div" );
			div.Attributes.Add( "class", "inner" );
			System.Web.UI.HtmlControls.HtmlGenericControl header = new HtmlGenericControl( "h2" );
			header.InnerText = this.Text;
			System.Web.UI.HtmlControls.HtmlGenericControl basediv = new HtmlGenericControl( "div" );
			div.Attributes.Add( "class", "base" );
			div.Controls.Add( header );
			popupPanel.Controls.Add( div );
			popupPanel.Controls.Add( basediv );

			basediv.Controls.Add( okButton );
			basediv.Controls.Add( cancelButton );

			popupPanel.Attributes.Add( "style", "display:none;" );
			popupPanel.CssClass = "confirm-dialog";

			this.Controls.Add( popupPanel );

			ConfirmExtender.TargetControlID = hiddenButton.ID;
			ConfirmExtender.PopupControlID = popupPanel.ID;
			ConfirmExtender.CancelControlID = cancelButton.ID;
			ConfirmExtender.BehaviorID = _behaviorID;
			ConfirmExtender.BackgroundCssClass = "modalBackground";

			this.Controls.Add( ConfirmExtender );
		}

		void okButton_Click( object sender, EventArgs e )
		{
			if ( Confirm != null ) Confirm( this, e );
			Hide();
		}

		void cancelButton_Click( object sender, EventArgs e )
		{
			if ( Cancel != null ) Cancel( this, e );
			Hide();
		}

		public string Text
		{
			get { return _confirmText; }
			set { _confirmText = value; }
		}

		public string BehaviorID
		{
			get { return _behaviorID; }
			set { _behaviorID = value; }
		}

		public event EventHandler<EventArgs> Confirm;
		public event EventHandler<EventArgs> Cancel;
	}
}
