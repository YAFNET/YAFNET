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
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Controls;

namespace YAF.Modules
{
	/// <summary>
	/// Summary description for PageTitleModule
	/// </summary>
	[YafModule("Page Title Module", "Tiny Gecko",1)]
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
				title.AppendFormat("{0} - ", YafServices.BadWordReplace.Replace(PageContext.PageTopicName)); // Tack on the topic we're viewing

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
			HtmlHead head = ForumControl.Page.Header ??
			                ControlHelper.FindControlRecursiveBothAs<HtmlHead>( CurrentForumPage, "YafHead" );

			if (head != null)
			{
				// setup the title...
				string addition = string.Empty;

				if ( head.Title.Trim().Length > 0 )
					addition = " - " + head.Title;

				head.Title = _forumPageTitle + addition;
			}
			else
			{
				// old style
				object objTitle = ControlHelper.FindControlRecursiveBoth( CurrentForumPage, "ForumTitle" );

				if ( objTitle != null && objTitle is HtmlTitle )
				{
					HtmlTitle title = objTitle as HtmlTitle;
					title.Text = _forumPageTitle;
				}
			}
		}
	}
}