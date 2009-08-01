using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using YAF.Classes.Base;

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
			if (PageContext.LoadString.Length > 0)
			{
				if (ScriptManager.GetCurrent(ForumControl.Page) != null)
				{
					ScriptManager.RegisterStartupScript(ForumControl.Page, typeof(Forum), "modalNotification", String.Format("var fpModal = function() {1} {3}('{0}'); {2}\nSys.Application.remove_load(fpModal);\nSys.Application.add_load(fpModal);\n\n", PageContext.LoadStringJavascript, '{', '}', _errorPopup.ShowModalFunction), true);
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