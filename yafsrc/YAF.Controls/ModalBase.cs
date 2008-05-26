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
	public class ModalBase : BaseControl
	{
		protected string _behaviorID;
		protected ModalPopupExtender _popupControlExtender = new ModalPopupExtender();
		protected Label _textMainLabel = new Label();
		protected Label _textSubLabel = new Label();
		protected Label _headerLabel = new Label();
		protected Button _okButton = new Button();
		protected Button _cancelButton = new Button();

		public ModalBase()
			: base()
		{
			this.Load += new EventHandler( ModalBase_Load );
		}

		void ModalBase_Load( object sender, EventArgs e )
		{
			// set localization here...
			if ( !String.IsNullOrEmpty( _okButton.Text ) )
			{
				_okButton.Text = PageContext.Localization.GetText( "COMMON", "OK" );
			}
			if ( !String.IsNullOrEmpty( _cancelButton.Text ) )
			{
				_cancelButton.Text = PageContext.Localization.GetText( "COMMON", "CANCEL" );
			}
		}

		protected override void OnLoad( EventArgs e )
		{			
			base.OnInit( e );
			BuildPopup();
		}	

		#region Overridable Modal Generation Functions

		virtual protected void BuildPopup()
		{
			Panel parentPanel = CreateParentPanel();
			// add and create hidden button...
			HtmlInputButton hiddenButton = CreateHiddenButton( parentPanel );

			SetupButtons();

			Control header = CreateHeader();
			Control inner = CreateInner();
			Control footer = CreateFooter();

			Control baseControl = CreateBaseControl();

			// add controls to the base, then the base to the parent panel
			baseControl.Controls.Add( header );
			baseControl.Controls.Add( inner );
			baseControl.Controls.Add( footer );
			parentPanel.Controls.Add( baseControl );

			this.Controls.Add( parentPanel );

			ConfirmExtender.TargetControlID = hiddenButton.ID;
			ConfirmExtender.PopupControlID = parentPanel.ID;
			ConfirmExtender.CancelControlID = _cancelButton.ID;
			ConfirmExtender.PopupDragHandleControlID = header.ID;
			ConfirmExtender.BehaviorID = _behaviorID;
			ConfirmExtender.BackgroundCssClass = "modalBackground";

			this.Controls.Add( ConfirmExtender );
		}
		virtual protected void SetupButtons()
		{
			// make buttons
			_okButton.ID = GetUniqueID( "btnOk" );
			_cancelButton.ID = GetUniqueID( "btnCancel" );			

			_okButton.Click += new EventHandler( okButton_Click );
			_cancelButton.Click += new EventHandler( cancelButton_Click );
		}
		virtual protected Panel CreateParentPanel()
		{
			Panel parentPanel = new Panel();
			parentPanel.ID = this.ClientID;
			parentPanel.Attributes.Add( "style", "display:none;" );
			parentPanel.CssClass = "modalPopup";

			return parentPanel;
		}
		virtual protected HtmlInputButton CreateHiddenButton( Panel parentPanel )
		{
			HtmlGenericControl span = new HtmlGenericControl( "span" );
			span.Attributes.Add( "style", "display:none" );
			HtmlInputButton hiddenButton = new HtmlInputButton();
			hiddenButton.ID = GetUniqueID( "btnHidden" );
			span.Controls.Add( hiddenButton );
			parentPanel.Controls.Add( span );

			return hiddenButton;
		}
		virtual protected Control CreateHeader()
		{
			System.Web.UI.HtmlControls.HtmlGenericControl divHeader = new HtmlGenericControl( "div" );
			divHeader.Attributes.Add( "class", "modalHeader" );
			divHeader.ID = GetUniqueID( "divHeader" );
			divHeader.Controls.Add( _headerLabel );
			return divHeader;
		}
		virtual protected Control CreateInner()
		{
			System.Web.UI.HtmlControls.HtmlGenericControl divInner = new HtmlGenericControl( "div" );
			divInner.Attributes.Add( "class", "modalInner" );

			HtmlGenericControl spanMainText = new HtmlGenericControl( "span" );
			HtmlGenericControl spanSubText = new HtmlGenericControl( "span" );
			spanMainText.Attributes.Add( "class", "modalInnerMain" );
			spanSubText.Attributes.Add( "class", "modalInnerSub" );

			spanMainText.Controls.Add( _textMainLabel );
			spanSubText.Controls.Add( _textSubLabel );

			divInner.Controls.Add( spanMainText );
			divInner.Controls.Add( spanSubText );

			return divInner;
		}
		virtual protected Control CreateFooter()
		{
			System.Web.UI.HtmlControls.HtmlGenericControl divFooter = new HtmlGenericControl( "div" );
			divFooter.Attributes.Add( "class", "modalFooter" );
			divFooter.Controls.Add( _okButton );
			divFooter.Controls.Add( _cancelButton );

			return divFooter;
		}
		virtual protected Control CreateBaseControl()
		{
			System.Web.UI.HtmlControls.HtmlGenericControl divBase = new HtmlGenericControl( "div" );
			divBase.Attributes.Add( "class", "modalBase" );

			return divBase;
		} 

		#endregion

		#region Helper Functions

		/// <summary>
		/// Shows the Modal
		/// </summary>
		public void Show()
		{
			ConfirmExtender.Show();
		}

		/// <summary>
		/// Hides the Modal
		/// </summary>
		public void Hide()
		{
			ConfirmExtender.Hide();
		}

		#endregion

		#region Events

		public event EventHandler<EventArgs> Confirm;
		public event EventHandler<EventArgs> Cancel;
		protected void okButton_Click( object sender, EventArgs e )
		{
			if ( Confirm != null ) Confirm( this, e );
			Hide();
		}
		protected void cancelButton_Click( object sender, EventArgs e )
		{
			if ( Cancel != null ) Cancel( this, e );
			Hide();
		} 

		#endregion

		#region Public Properties

		/// <summary>
		/// Base Modal Control
		/// </summary>
		public ModalPopupExtender ConfirmExtender
		{
			get { return _popupControlExtender; }
		}
		public bool CancelButtonVisible
		{
			get
			{
				if ( _cancelButton.Attributes ["style"] != null && _cancelButton.Attributes ["style"] == "display:none" )
				{
					return false;
				}
				return true;
			}
			set
			{
				if ( value && _cancelButton.Attributes ["style"] != null )
				{
					_cancelButton.Attributes.Remove( "style" );
				}
				else
				{
					_cancelButton.Attributes.Add( "style", "display:none" );
				}
			}
		}
		public bool OkButtonVisible
		{
			get { return _okButton.Visible; }
			set { _okButton.Visible = value; }
		}
		public string CancelButtonOnClientClick
		{
			get { return _cancelButton.OnClientClick; }
			set { _cancelButton.OnClientClick = value; }
		}
		public string OkButtonOnClientClick
		{
			get { return _okButton.OnClientClick; }
			set { _okButton.OnClientClick = value; }
		}
		public string MainText
		{
			get { return _textMainLabel.Text; }
			set { _textMainLabel.Text = value; }
		}
		public string SubText
		{
			get { return _textSubLabel.Text; }
			set { _textSubLabel.Text = value; }
		}
		public string HeaderText
		{
			get { return _headerLabel.Text; }
			set { _headerLabel.Text = value; }
		}
		public string BehaviorID
		{
			get { return _behaviorID; }
			set { _behaviorID = value; ConfirmExtender.BehaviorID = value;  }
		} 

		#endregion
	}
}
