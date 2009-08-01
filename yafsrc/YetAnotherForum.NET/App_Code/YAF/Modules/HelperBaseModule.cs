using System;
using System.Collections.Generic;
using System.Web;
using YAF.Classes.Base;
using YAF.Classes.Utils;

namespace YAF.Modules
{
	/// <summary>
	/// Summary description for HelperBaseModule
	/// </summary>
	public class HelperBaseModule : IBaseModule
	{
		private ForumPages _forumPageType;

		public ForumPages ForumPageType
		{
			get
			{
				return _forumPageType;
			}
		}

		private YafContext _pageContext;

		public YafContext PageContext
		{
			get
			{
				return _pageContext;
			}
		}

		private ForumPage _currentPage;
		public ForumPage CurrentPage
		{
			get
			{
				return _currentPage;
			}
		}

		public HelperBaseModule()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public virtual void InitModule()
		{
			
		}

		#region IBaseModule Members

		public void Initalize(YAF.Classes.Utils.YafContext currentContext, YAF.Classes.Base.ForumPage currentPage, YAF.Classes.Utils.ForumPages pageType)
		{
			// save the page type...
			_forumPageType = pageType;
			_pageContext = currentContext;
			_currentPage = currentPage;

			InitModule();
		}

		#endregion
	}
}