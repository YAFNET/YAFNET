/* Yet Another Forum.NET
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
using System.Web.UI;

namespace YAF.Controls
{
	public class ModalNotification : ModalBase
	{
		public ModalNotification()
			: base()
		{
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			// hide on OK click...
			OkButtonOnClientClick = String.Format( "$find('{0}').hide(); return false;", this.BehaviorID );
			// populate notification header text from localization...
			this.HeaderText = PageContext.Localization.GetText( "COMMON", "MODAL_NOTIFICATION_HEADER" );
			// add js for client-side error settings...
			string jsFunction = String.Format( "\n{4} = function( newErrorStr ) {2}\n if (newErrorStr != null && newErrorStr != \"\" && $find('{0}') != null && document.getElementById('{1}') != null) {2}\ndocument.getElementById('{1}').innerHTML = newErrorStr;\n$find('{0}').show();\n{3}\n{3}\n", this.BehaviorID, this.MainTextClientID, '{', '}', ShowModalFunction );
			ScriptManager.RegisterClientScriptBlock( this, typeof( ModalNotification ), ShowModalFunction, jsFunction, true );
		}

		protected override void OnInit( EventArgs e )
		{
			// init the popup first...
			base.OnInit( e );
			// make a few changes for this type of modal...
			CancelButtonVisible = false;
		}

		public string ShowModalFunction
		{
			get
			{
				return string.Format( "ShowModalNotification{0}", BehaviorID );
			}
		}

		public string MainTextClientID
		{
			get
			{
				return _textMainLabel.ClientID;
			}
		}
	}
}
