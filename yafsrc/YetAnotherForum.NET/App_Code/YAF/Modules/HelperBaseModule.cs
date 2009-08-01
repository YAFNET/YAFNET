using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using YAF.Classes.Base;
using YAF.Classes.Utils;

namespace YAF.Modules
{
	/// <summary>
	/// Summary description for HelperBaseModule
	/// </summary>
	public class HelperBaseModule : IBaseModule
	{
		protected ForumPages _forumPageType;
		public ForumPages ForumPageType
		{
			get
			{
				return _forumPageType;
			}
		}

		protected YafContext _pageContext;
		public YafContext PageContext
		{
			get
			{
				return _pageContext;
			}
		}

		protected ForumPage _currentForumPage;
		public ForumPage CurrentForumPage
		{
			get
			{
				return _currentForumPage;
			}
			set
			{
				_currentForumPage = value;
			}
		}

		protected Forum _forumControl;
		public Forum ForumControl
		{
			get
			{
				return _forumControl;
			}
			set
			{
				_forumControl = value;
			}
		}

		protected bool _initBefore = false;
		public bool InitBefore
		{
			get
			{
				return _initBefore;
			}
			set
			{
				_initBefore = value;
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

		public void Initalize(YAF.Classes.Utils.YafContext pageContext, object forumControl, ForumPage forumPage, YAF.Classes.Utils.ForumPages pageType)
		{
			// save the page type...
			_forumPageType = pageType;
			_pageContext = pageContext;
			_currentForumPage = forumPage;
			_forumControl = (Forum)forumControl;

			InitModule();
		}

		public bool InitBeforeForumPage
		{
			get
			{
				return InitBefore;
			}
		}

		#endregion
	}
}