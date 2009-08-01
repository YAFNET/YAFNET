using System;
using System.Collections.Generic;
using System.Web;
using YAF.Classes.Base;
using YAF.Classes.Utils;

namespace YAF.Modules
{
	/// <summary>
	/// Summary description for SimpleBaseModule
	/// </summary>
	public class SimpleBaseModule : IBaseModule
	{
		virtual public void InitAfterPage()
		{
			
		}

		virtual public void InitBeforePage()
		{

		}

		#region IBaseModule Basic Members

		protected YafContext _pageContext;
		public YafContext PageContext
		{
			get
			{
				return _pageContext;
			}
			set
			{
				_pageContext = value;
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

		protected object _forumControlObj;
		public object ForumControlObj
		{
			get
			{
				return _forumControlObj;
			}
			set
			{
				_forumControlObj = value;
			}
		}

		public Forum ForumControl
		{
			get
			{
				return (Forum)ForumControlObj;
			}
		}

		protected ForumPages _forumPageType;
		public ForumPages ForumPageType
		{
			get
			{
				return _forumPageType;
			}
			set
			{
				_forumPageType = value;
			}
		}

		#endregion

		#region IBaseModule Module Information

		virtual public string ModuleAuthor
		{
			get
			{
				return "";
			}
		}

		virtual public string ModuleName
		{
			get
			{
				return "";
			}
		}

		virtual public int ModuleVersion
		{
			get
			{
				return 0;
			}
		}

		#endregion

		#region IDisposable Members

		virtual public void Dispose()
		{

		}

		#endregion
	}
}