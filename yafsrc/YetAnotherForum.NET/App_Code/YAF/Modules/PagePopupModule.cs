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
using YAF.Classes.Core;

namespace YAF.Modules
{
	/// <summary>
	/// Summary description for PagePopupModule
	/// </summary>
	public class PagePopupModule : SimpleBaseModule
	{
		protected YAF.Controls.ModalNotification _errorPopup = null;

		public PagePopupModule()
		{

		}

		public override void InitBeforePage()
		{
			ForumControl.PreRender += new EventHandler(CurrentPage_PreRender);

			// at this point, init has already been called...
			AddErrorPopup();
		}

		void CurrentPage_PreRender(object sender, EventArgs e)
		{
			RegisterLoadString();
		}

		/// <summary>
		/// Sets up the Modal Error Popup Dialog
		/// </summary>
		private void AddErrorPopup()
		{
			// add error control...
			_errorPopup = new YAF.Controls.ModalNotification();
			_errorPopup.ID = "ForumPageErrorPopup1";
			_errorPopup.BehaviorID = "ForumPageErrorPopup";
			ForumControl.Controls.Add(_errorPopup);
		}

		protected void RegisterLoadString()
		{
			if (PageContext.LoadMessage.LoadString.Length > 0)
			{
				if (ScriptManager.GetCurrent(ForumControl.Page) != null)
				{
					ScriptManager.RegisterStartupScript(ForumControl.Page, typeof(Forum), "modalNotification", String.Format("var fpModal = function() {1} {3}('{0}'); {2}\nSys.Application.remove_load(fpModal);\nSys.Application.add_load(fpModal);\n\n", PageContext.LoadMessage.StringJavascript, '{', '}', _errorPopup.ShowModalFunction), true);
				}
			}
			else
			{
				// make sure we don't show the popup...
				ScriptManager.RegisterStartupScript(ForumControl.Page, typeof(Forum), "modalNotificationRemove", "if (typeof(fpModal) != 'undefined') Sys.Application.remove_load(fpModal);\n", true);
			}
		}
	}
}