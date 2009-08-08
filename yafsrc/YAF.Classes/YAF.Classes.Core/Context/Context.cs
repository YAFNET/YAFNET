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
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using YAF.Classes.Pattern;
using YAF.Classes.Utils;
using YAF.Editors;
using YAF.Modules;

namespace YAF.Classes.Core
{
	/// <summary>
	/// Context class that accessable with the same instance from all locations
	/// </summary>
	public sealed class YafContext : UserPageBase, IDisposable
	{
		private readonly SingleClassInstanceFactory _singleInstanceFactory = new SingleClassInstanceFactory();
		private readonly Dictionary<string, object> _variables = new Dictionary<string, object>();

		public event EventHandler<EventArgs> Init;
		public event EventHandler<EventArgs> PageLoad;
		public event EventHandler<EventArgs> PageUnload;
		public event EventHandler<EventArgs> Unload;
		private ForumPage _currentForumPage = null;

		private YafContext()
		{
			// init context...
			if (Init != null) Init(this, new EventArgs());
		}

		void ForumPageLoad(object sender, EventArgs e)
		{
			if ( PageLoad != null ) PageLoad(this,new EventArgs());
		}

		void ForumPageUnload(object sender, EventArgs e)
		{
			if (PageUnload != null) PageUnload(this, new EventArgs());
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (Unload != null) Unload(this, new EventArgs());
		}

		#endregion

		/// <summary>
		/// Get the instance of the Forum Context
		/// </summary>
		public static YafContext Current
		{
			get
			{
				return PageSingleton<YafContext>.Instance;
			}
		}

		public SingleClassInstanceFactory InstanceFactory
		{
			get
			{
				return _singleInstanceFactory;
			}
		}

		/// <summary>
		/// Forum page instance of the current forum page.
		/// May not be valid until everything is initialized.
		/// </summary>
		public ForumPage CurrentForumPage
		{
			get
			{
				return _currentForumPage;
			}
			set
			{
				_currentForumPage = value;
				value.Load += new EventHandler(ForumPageLoad);
				value.Unload += new EventHandler(ForumPageUnload);				
			}
		}

		/// <summary>
		/// Current Page Localization
		/// </summary>
		public YafLocalization Localization
		{
			get
			{
				return _singleInstanceFactory.GetInstance<LocalizationHandler>().Localization;
			}
		}

		/// <summary>
		/// Current Page Theme
		/// </summary>
		public YafTheme Theme
		{
			get
			{
				return _singleInstanceFactory.GetInstance<ThemeHandler>().Theme;
			}
		}

		/// <summary>
		/// Current System-Wide Cache
		/// </summary>
		public YafCache Cache
		{
			get
			{
				return _singleInstanceFactory.GetInstance<YafCache>();
			}
		}

		public Dictionary<string,object> Vars
		{
			get
			{
				return _variables;
			}
		}

		public object this[string varName]
		{
			get
			{
				if (_variables.ContainsKey(varName))
				{
					return _variables[varName];
				}

				return null;
			}
			set
			{
				_variables[varName] = value;
			}
		}

		/// <summary>
		/// Current Page Elements
		/// </summary>
		public PageElementRegister PageElements
		{
			get
			{
				return _singleInstanceFactory.GetInstance<PageElementRegister>();
			}
		}

		/// <summary>
		/// Current Page Load Message
		/// </summary>
		public LoadMessage LoadMessage
		{
			get
			{
				return _singleInstanceFactory.GetInstance<LoadMessage>();
			}
		}

		/// <summary>
		/// Current Page Query ID Helper
		/// </summary>
		public QueryStringIDHelper QueryIDs
		{
			get
			{
				return _singleInstanceFactory.GetInstance<QueryStringIDHelper>();
			}
			set
			{
				_singleInstanceFactory.SetInstance( value );
			}
		}

		/// <summary>
		/// Current Page Instance of the Module Manager
		/// </summary>
		public YafBaseModuleManager BaseModuleManager
		{
			get
			{
				return _singleInstanceFactory.GetInstance<YafBaseModuleManager>();
			}
		}

		/// <summary>
		/// Current Page Instance of the Module Manager
		/// </summary>
		public YafEditorModuleManager EditorModuleManager
		{
			get
			{
				return _singleInstanceFactory.GetInstance<YafEditorModuleManager>();
			}
		}

		/// <summary>
		/// Current Page User Profile
		/// </summary>
		public YafUserProfile Profile
		{
			get
			{
				return (YafUserProfile)HttpContext.Current.Profile;
			}
		}

		/// <summary>
		/// Current Page Control Settings from Forum Control
		/// </summary>
		public YafControlSettings Settings
		{
			get
			{
				return YafControlSettings.Current;
			}
		}

		private MembershipUser _user = null;

		/// <summary>
		/// Current Membership User
		/// </summary>
		public MembershipUser User
		{
			get
			{
				if (_user == null)
					_user = Membership.GetUser();
				return _user;
			}
			set
			{
				_user = value;
			}
		}

		/// <summary>
		/// Get the current page as the forumPage Enum (for comparison)
		/// </summary>
		public ForumPages ForumPageType
		{
			get
			{
				if (HttpContext.Current.Request.QueryString["g"] == null)
					return ForumPages.forum;

				try
				{
					return (ForumPages)Enum.Parse(typeof(ForumPages), HttpContext.Current.Request.QueryString["g"], true);
				}
				catch (Exception)
				{
					return ForumPages.forum;
				}
			}
		}

		/// <summary>
		/// Current Board Settings
		/// </summary>
		public YafBoardSettings BoardSettings
		{
			get
			{
				string key = YafCache.GetBoardCacheKey(Constants.Cache.BoardSettings);

				if (HttpContext.Current.Application[key] == null)
					HttpContext.Current.Application[key] = new YafLoadBoardSettings(PageBoardID);

				return (YafBoardSettings)HttpContext.Current.Application[key];
			}
			set
			{
				string key = YafCache.GetBoardCacheKey(Constants.Cache.BoardSettings);

				if (value == null)
					HttpContext.Current.Application.Remove(key);
				else
				{
					// set the updated board settings...	
					HttpContext.Current.Application[key] = value;
				}
			}
		}

		public void AddLoadMessage(string loadMessage)
		{
			this.LoadMessage.Add( loadMessage );
		}
	}
}