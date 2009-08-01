using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using YAF.Classes.Base;
using YAF.Classes.Utils;
using YAF.Controls;

namespace YAF.Modules
{
	/// <summary>
	/// Summary description for PageTitleModule
	/// </summary>
	public class PageTitleModule : SimpleBaseModule
	{
		protected string _forumPageTitle = null;

		public PageTitleModule()
		{
		}

		override public void InitAfterPage()
		{
			CurrentForumPage.PreRender += new EventHandler(ForumPage_PreRender);
			CurrentForumPage.Load += new EventHandler(ForumPage_Load);
		}

		override public void InitBeforePage()
		{

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

			string pageStr = string.Empty;

			if (ForumPageType == ForumPages.posts || ForumPageType == ForumPages.topics)
			{
				// get current page...
				Pager currentPager = (Pager)CurrentForumPage.FindControl("Pager");

				if (currentPager != null && currentPager.CurrentPageIndex != 0)
				{
					pageStr = String.Format("Page {0} - ", currentPager.CurrentPageIndex + 1);
				}
			}

			if (PageContext.PageTopicID != 0)
				title.AppendFormat("{0} - ", General.BadWordReplace(PageContext.PageTopicName)); // Tack on the topic we're viewing

			if ( ForumPageType == ForumPages.posts)
			{
				title.Append( pageStr );
			}

			if (PageContext.PageForumName != string.Empty)
				title.AppendFormat("{0} - ", CurrentForumPage.HtmlEncode(PageContext.PageForumName)); // Tack on the forum we're viewing

			if (ForumPageType == ForumPages.topics)
			{
				title.Append(pageStr);
			}

			title.Append(CurrentForumPage.HtmlEncode(PageContext.BoardSettings.Name)); // and lastly, tack on the board's name
			_forumPageTitle = title.ToString();

			ForumControl.FirePageTitleSet(this, new ForumPageTitleArgs( _forumPageTitle ));
		}

		void ForumPage_PreRender(object sender, EventArgs e)
		{
			HtmlHead head = (HtmlHead)ControlHelper.FindControlRecursiveBoth(CurrentForumPage, "YafHead");

			if (head != null)
			{
				// setup the title...
				head.Title = _forumPageTitle + " - " + head.Title;
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