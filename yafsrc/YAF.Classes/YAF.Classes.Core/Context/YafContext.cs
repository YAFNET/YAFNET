/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Classes.Core
{
  using System;
  using System.Web;
  using System.Web.Security;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;
  using YAF.Editors;
  using YAF.Modules;

  /// <summary>
  /// Context class that accessible with the same instance from all locations
  /// </summary>
  public partial class YafContext : UserPageBase, IDisposable
  {
    /// <summary>
    /// The _application.
    /// </summary>
    protected static HttpApplicationState _application = null;

    /// <summary>
    /// The _current forum page.
    /// </summary>
    protected ForumPage _currentForumPage = null;

    /// <summary>
    /// The _repository.
    /// </summary>
    protected ContextVariableRepository _repository;

    /// <summary>
    /// The _single instance factory.
    /// </summary>
    protected SingleClassInstanceFactory _singleInstanceFactory = new SingleClassInstanceFactory();

    /// <summary>
    /// The _user.
    /// </summary>
    private MembershipUser _user = null;

    /// <summary>
    /// The _variables.
    /// </summary>
    protected TypeDictionary _variables = new TypeDictionary();

    /// <summary>
    /// Initializes a new instance of the <see cref="YafContext"/> class. 
    /// YafContext Constructor
    /// </summary>
    public YafContext()
    {
      // init the respository
      this._repository = new ContextVariableRepository(this._variables);

      // init context...
      if (Init != null)
      {
        Init(this, new EventArgs());
      }
    }

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

    /// <summary>
    /// Get the instance of the Http Context if available
    /// </summary>
    public static HttpContext HttpContext
    {
      get
      {
        return HttpContext.Current ?? null;
      }
    }

    /// <summary>
    /// Get/set the current state of the Http Application.
    /// Defaults to HttpContext.Current.Application. If not available
    /// pulls from application variable.
    /// </summary>
    public static HttpApplicationState Application
    {
      get
      {
        if (HttpContext.Current != null)
        {
          return HttpContext.Current.Application;
        }

        return _application;
      }

      set
      {
        _application = value;
      }
    }

    /// <summary>
    /// Provides access to the YafContext Per-Instance Class Factory
    /// </summary>
    public SingleClassInstanceFactory InstanceFactory
    {
      get
      {
        return this._singleInstanceFactory;
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
        return this._currentForumPage;
      }

      set
      {
        this._currentForumPage = value;
        value.Load += new EventHandler(ForumPageLoad);
        value.Unload += new EventHandler(ForumPageUnload);
      }
    }

    /// <summary>
    /// Access to the Context Global Variable Repository Class
    /// which is a helper class that accesses YafContext.Vars with strongly
    /// typed properties for primary variables. 
    /// </summary>
    public ContextVariableRepository Globals
    {
      get
      {
        return this._repository;
      }
    }

    /// <summary>
    /// Current Page Localization
    /// </summary>
    public YafLocalization Localization
    {
      get
      {
        return this._singleInstanceFactory.GetInstance<LocalizationHandler>().Localization;
      }
    }

    /// <summary>
    /// Current Page Theme
    /// </summary>
    public YafTheme Theme
    {
      get
      {
        return this._singleInstanceFactory.GetInstance<ThemeHandler>().Theme;
      }
    }

    /// <summary>
    /// Current System-Wide Cache
    /// </summary>
    public YafCache Cache
    {
      get
      {
        return this._singleInstanceFactory.GetInstance<YafCache>();
      }
    }

    /// <summary>
    /// YafContext Global Instance Variables
    /// Use for plugins or other situations where a value is needed per instance.
    /// </summary>
    public TypeDictionary Vars
    {
      get
      {
        return this._variables;
      }
    }

    /// <summary>
    /// Returns a value from the YafContext Global Instance Variables (Vars) collection.
    /// </summary>
    /// <param name="varName"></param>
    /// <returns>Value if it's found, null if it doesn't exist.</returns>
    public object this[string varName]
    {
      get
      {
        if (this._variables.ContainsKey(varName))
        {
          return this._variables[varName];
        }

        return null;
      }

      set
      {
        this._variables[varName] = value;
      }
    }

    /// <summary>
    /// Current Page Elements
    /// </summary>
    public PageElementRegister PageElements
    {
      get
      {
        return this._singleInstanceFactory.GetInstance<PageElementRegister>();
      }
    }

    /// <summary>
    /// Current Page Load Message
    /// </summary>
    public LoadMessage LoadMessage
    {
      get
      {
        return this._singleInstanceFactory.GetInstance<LoadMessage>();
      }
    }

    /// <summary>
    /// Current Page Query ID Helper
    /// </summary>
    public QueryStringIDHelper QueryIDs
    {
      get
      {
        return this._singleInstanceFactory.GetInstance<QueryStringIDHelper>();
      }

      set
      {
        this._singleInstanceFactory.SetInstance(value);
      }
    }

    /// <summary>
    /// Current Page Instance of the Module Manager
    /// </summary>
    public YafBaseModuleManager BaseModuleManager
    {
      get
      {
        return this._singleInstanceFactory.GetInstance<YafBaseModuleManager>();
      }
    }

    /// <summary>
    /// Current Page Instance of the Module Manager
    /// </summary>
    public YafEditorModuleManager EditorModuleManager
    {
      get
      {
        return this._singleInstanceFactory.GetInstance<YafEditorModuleManager>();
      }
    }

    /// <summary>
    /// Current Page User Profile
    /// </summary>
    public YafUserProfile Profile
    {
      get
      {
        return (YafUserProfile) HttpContext.Current.Profile;
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

    /// <summary>
    /// Current Membership Provider used by YAF
    /// </summary>
    public MembershipProvider CurrentMembership
    {
      get
      {
        if (!String.IsNullOrEmpty(Config.MembershipProvider) && Membership.Providers[Config.MembershipProvider] != null)
        {
          return Membership.Providers[Config.MembershipProvider];
        }

        // return default membership provider
        return Membership.Provider;
      }
    }

    /// <summary>
    /// Current Membership Roles Provider used by YAF
    /// </summary>
    public RoleProvider CurrentRoles
    {
      get
      {
        if (!String.IsNullOrEmpty(Config.RoleProvider) && Roles.Providers[Config.RoleProvider] != null)
        {
          return Roles.Providers[Config.RoleProvider];
        }

        // return default role provider
        return Roles.Provider;
      }
    }

    /// <summary>
    /// Current Membership User
    /// </summary>
    public MembershipUser User
    {
      get
      {
        if (this._user == null)
        {
          this._user = UserMembershipHelper.GetUser();
        }

        return this._user;
      }

      set
      {
        this._user = value;
      }
    }

    /// <summary>
    /// Instance of the Combined UserData for the current user.
    /// </summary>
    public CombinedUserDataHelper CurrentUserData
    {
      get
      {
        return this._singleInstanceFactory.GetInstance<CombinedUserDataHelper>();
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
        {
          return ForumPages.forum;
        }

        try
        {
          return (ForumPages) Enum.Parse(typeof (ForumPages), HttpContext.Current.Request.QueryString["g"], true);
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
    public virtual YafBoardSettings BoardSettings
    {
      get
      {
        string key = YafCache.GetBoardCacheKey(Constants.Cache.BoardSettings);

        if (Application[key] == null)
        {
          Application[key] = new YafLoadBoardSettings(PageBoardID);
        }

        return (YafBoardSettings) Application[key];
      }

      set
      {
        string key = YafCache.GetBoardCacheKey(Constants.Cache.BoardSettings);

        if (value == null)
        {
          Application.Remove(key);
        }
        else
        {
          // set the updated board settings...	
          Application[key] = value;
        }
      }
    }

    #region IDisposable Members

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose()
    {
      if (Unload != null)
      {
        Unload(this, new EventArgs());
      }
    }

    #endregion

    /// <summary>
    /// On YafContext Constructor Call
    /// </summary>
    public event EventHandler<EventArgs> Init;

    /// <summary>
    /// On ForumPage Init Call
    /// </summary>
    public event EventHandler<EventArgs> PageInit;

    /// <summary>
    /// On ForumPage Load Call
    /// </summary>
    public event EventHandler<EventArgs> PageLoad;

    /// <summary>
    /// On ForumPage PreLoad (Before Load) Call
    /// </summary>
    public event EventHandler<EventArgs> PagePreLoad;

    /// <summary>
    /// On ForumPage Unload Call
    /// </summary>
    public event EventHandler<EventArgs> PageUnload;

    /// <summary>
    /// On YafContext Unload Call
    /// </summary>
    public event EventHandler<EventArgs> Unload;

    /// <summary>
    /// Fired from ForumPage
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    internal void ForumPageInit(object sender, EventArgs e)
    {
      if (PageInit != null)
      {
        PageInit(this, new EventArgs());
      }
    }

    /// <summary>
    /// Fired from ForumPage
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    internal void ForumPagePreLoad(object sender, EventArgs e)
    {
      if (PagePreLoad != null)
      {
        PagePreLoad(this, new EventArgs());
      }
    }

    /// <summary>
    /// The forum page load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ForumPageLoad(object sender, EventArgs e)
    {
      if (PageLoad != null)
      {
        PageLoad(this, new EventArgs());
      }
    }

    /// <summary>
    /// The forum page unload.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ForumPageUnload(object sender, EventArgs e)
    {
      if (PageUnload != null)
      {
        PageUnload(this, new EventArgs());
      }
    }

    /// <summary>
    /// Helper Function that adds a "load message" to the load message class.
    /// </summary>
    /// <param name="loadMessage">
    /// </param>
    public void AddLoadMessage(string loadMessage)
    {
      LoadMessage.Add(loadMessage);
    }
  }
}