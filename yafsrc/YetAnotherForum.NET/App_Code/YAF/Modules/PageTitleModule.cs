using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using YAF.Classes.Base;
using YAF.Classes.Utils;

namespace YAF.Modules
{
	/// <summary>
	/// Summary description for PageTitleModule
	/// </summary>
	public class PageTitleModule : HelperBaseModule
	{
		protected string _forumPageTitle = null;

		public PageTitleModule()
		{
		}

		public override void InitModule()
		{
			CurrentForumPage.PreRender += new EventHandler(ForumPage_PreRender);
			CurrentForumPage.Load += new EventHandler(ForumPage_Load);
		}

		void ForumPage_Load(object sender, EventArgs e)
		{
			GeneratePageTitle();
		}

		/// <summary>
		/// Creates this pages title and fires a PageTitleSet event if one is set
		/// </summary>
		private void GeneratePageTitle()
		{
			// compute page title..
			System.Text.StringBuilder title = new StringBuilder();

			if (PageContext.PageTopicID != 0)
				title.AppendFormat("{0} - ", General.BadWordReplace(PageContext.PageTopicName)); // Tack on the topic we're viewing
			if (PageContext.PageForumName != string.Empty)
				title.AppendFormat("{0} - ", CurrentForumPage.HtmlEncode(PageContext.PageForumName)); // Tack on the forum we're viewing
			title.Append(CurrentForumPage.HtmlEncode(PageContext.BoardSettings.Name)); // and lastly, tack on the board's name
			_forumPageTitle = title.ToString();

			//if (PageTitleSet != null) PageTitleSet(this, new ForumPageArgs(_forumPageTitle));
		}

		void ForumPage_PreRender(object sender, EventArgs e)
		{
			HtmlHead head = (HtmlHead)ControlHelper.FindControlRecursiveBoth(CurrentForumPage, "YafHead");

			if (head != null)
			{
				// setup the title...
				head.Title = _forumPageTitle;
			}
			else
			{
				// old style
				HtmlTitle title = (HtmlTitle)ControlHelper.FindControlRecursiveBoth(CurrentForumPage, "ForumTitle");
				if (title != null)
				{
					title.Text = _forumPageTitle;
				}
			}
		}
	}
}