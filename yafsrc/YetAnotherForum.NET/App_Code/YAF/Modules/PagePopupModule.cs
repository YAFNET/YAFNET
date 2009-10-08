/* YetAnotherForum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DNA.UI.JQuery;
using YAF.Classes.Core;

namespace YAF.Modules
{
	/// <summary>
	/// Summary description for PagePopupModule
	/// </summary>
	[YafModule( "Page Popup Module", "Tiny Gecko", 1 )]
	public class PagePopupModule : SimpleBaseModule
	{
		protected PopupDialogNotification _errorPopup = null;

		public PagePopupModule()
		{

		}

		public override void InitForum()
		{
			ForumControl.Init += new EventHandler( ForumControl_Init );
		}

		public override void InitAfterPage()
		{
			_errorPopup.Title = PageContext.Localization.GetText( "COMMON", "MODAL_NOTIFICATION_HEADER" );
			CurrentForumPage.PreRender += new EventHandler( CurrentForumPage_PreRender );
		}

		void CurrentForumPage_PreRender( object sender, EventArgs e )
		{
			RegisterLoadString();
		}

		void ForumControl_Init( object sender, EventArgs e )
		{
			// at this point, init has already been called...
			AddErrorPopup();
		}

		/// <summary>
		/// Sets up the Modal Error Popup Dialog
		/// </summary>
		private void AddErrorPopup()
		{
			// add error control...
			_errorPopup = new PopupDialogNotification();
			_errorPopup.ID = "YafForumPageErrorPopup1";

			ForumControl.Controls.Add( _errorPopup );
		}

		protected void RegisterLoadString()
		{
			PageContext.PageElements.RegisterJQuery();

			if ( PageContext.LoadMessage.LoadString.Length > 0 )
			{
				if ( ScriptManager.GetCurrent( ForumControl.Page ) != null )
				{
					//ScriptManager.RegisterStartupScript( ForumControl.Page, typeof( Forum ), "modalNotification", String.Format( "var fpModal = function() {0} {2}; {1}\nSys.Application.remove_load(fpModal);\nSys.Application.add_load(fpModal);\n\n", '{', '}', dialogOpen ), true );

					ScriptManager.RegisterStartupScript( ForumControl.Page, typeof( Forum ), "modalNotification", String.Format( "var fpModal = function() {1} {3}('{0}'); Sys.Application.remove_load(fpModal); {2}\nSys.Application.add_load(fpModal);\n\n", PageContext.LoadMessage.StringJavascript, '{', '}', _errorPopup.ShowModalFunction ), true );
				}
			}
			else
			{
				// make sure we don't show the popup...
				//ScriptManager.RegisterStartupScript( ForumControl.Page, typeof( Forum ), "modalNotificationRemove", "if (typeof(fpModal) != 'undefined') Sys.Application.remove_load(fpModal);\n", true );
			}
		}
	}

	public class PopupDialogNotification : DNA.UI.JQuery.Dialog
	{
		public class ErrorPopupCustomTemplate : ITemplate
		{
			public HtmlGenericControl SpanOuterMessage = new HtmlGenericControl( "span" );
			public HtmlGenericControl SpanInnerMessage = new HtmlGenericControl( "span" );

			public void InstantiateIn( Control container )
			{
				SpanOuterMessage.ID = "YafPopupErrorMessageOuter";
				SpanOuterMessage.Attributes.Add( "class", "modalOuter" );

				SpanInnerMessage.ID = "YafPopupErrorMessageInner";
				SpanInnerMessage.Attributes.Add( "class", "modalInner" );

				SpanInnerMessage.InnerText = "ERROR";

				SpanOuterMessage.Controls.Add( SpanInnerMessage );

				container.Controls.Add( SpanOuterMessage );
			}
		}

		protected DialogButton _okayButton = new DialogButton();
		protected ErrorPopupCustomTemplate _template = new ErrorPopupCustomTemplate();

		public PopupDialogNotification()
			: base()
		{
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			_okayButton.Text = YafContext.Current.Localization.GetText( "COMMON", "OK" );
			Title = YafContext.Current.Localization.GetText( "COMMON", "MODAL_NOTIFICATION_HEADER" );
		}

		protected override void OnPreRender( EventArgs e )
		{
			base.OnPreRender( e );

			// add js for client-side error settings...
			string jsFunction = String.Format( "\n{4} = function( newErrorStr ) {2}\n if (newErrorStr != null && newErrorStr != \"\" && jQuery('#{1}') != null) {2}\njQuery('#{1}').text(newErrorStr);\njQuery('#{0}').dialog('open');\n{3}\n{3}\n", this.ClientID, MainTextClientID, '{', '}', ShowModalFunction );
			ScriptManager.RegisterClientScriptBlock( this, typeof( PopupDialogNotification ), ShowModalFunction, jsFunction, true );
		}

		protected override void OnInit( EventArgs e )
		{
			// init the popup first...
			base.OnInit( e );
			// make a few changes for this type of modal...
			ShowModal = true;
			IsDraggable = true;
			IsResizable = false;
			DialogButtons = DialogButtons.OK;
			Width = Unit.Pixel( 400 );

			BodyTemplate = _template;

			_okayButton.Text = "OK";
			_okayButton.OnClientClick = "jQuery(this).dialog('close');"; ;
			Buttons.Add( _okayButton );
		}

		public string ShowModalFunction
		{
			get
			{
				return string.Format( "ShowPopupDialogNotification{0}", ClientID );
			}
		}

		public string MainTextClientID
		{
			get
			{
				return _template.SpanInnerMessage.ClientID;
			}
		}
	}
}