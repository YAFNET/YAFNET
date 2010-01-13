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
namespace YAF.Modules
{
  using System.Collections.Generic;
  using System.Reflection;
  using System.Web.Compilation;

  /// <summary>
  /// Handles IBaseModule types.
  /// </summary>
  public class YafBaseModuleManager : YafModuleManager<IBaseModule>
  {
    /// <summary>
    /// The _init called.
    /// </summary>
    protected bool _initCalled = false;

    /// <summary>
    /// Prevents a default instance of the <see cref="YafBaseModuleManager"/> class from being created.
    /// </summary>
    private YafBaseModuleManager()
      : base("YAF.Modules", "YAF.Modules.IBaseModule")
    {
      if (this.ModuleClassTypes == null)
      {
        // get the .Core module to add...
        this.AddModules(
          new List<Assembly>()
            {
              Assembly.GetExecutingAssembly()
            });

        // re-add these modules...
        this.AddModules(BuildManager.CodeAssemblies);
      }
    }

    /// <summary>
    /// The call init modules.
    /// </summary>
    /// <param name="forumControl">
    /// The forum control.
    /// </param>
    public void CallInitModules(object forumControl)
    {
      if (!this._initCalled)
      {
        foreach (IBaseModule currentModule in this.Modules)
        {
          currentModule.ForumControlObj = forumControl;
          currentModule.Init();
        }

        this._initCalled = true;
      }
    }
  }
}