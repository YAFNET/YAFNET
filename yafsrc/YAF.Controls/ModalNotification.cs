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
	public class ModalNotification : ModalBase
	{
		public ModalNotification()
			: base()
		{
			this.Load += new EventHandler( ModalNotification_Load );
		}

		void ModalNotification_Load( object sender, EventArgs e )
		{
			// hide on OK click...
			OkButtonOnClientClick = String.Format( "$find('{0}').hide(); return false;", this.BehaviorID );
			// populate notification header text from localization...
			this.HeaderText = PageContext.Localization.GetText( "COMMON", "MODAL_NOTIFICATION_HEADER" );
		}

		protected override void OnInit( EventArgs e )
		{
			// init the popup first...
			base.OnInit( e );
			// make a few changes for this type of modal...
			CancelButtonVisible = false;
		}
	}
}
