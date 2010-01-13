/* Yet Another Forum.net
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
  using System;

  using Classes.Core;

  /// <summary>
  /// The user clean up task module.
  /// </summary>
  [YafModule("Clean Up User Task Starting Module", "Tiny Gecko", 1)]
  public class UserCleanUpTaskModule : IBaseModule
  {
    /// <summary>
    /// The _forum control obj.
    /// </summary>
    private object _forumControlObj;

    #region IBaseModule Members

    /// <summary>
    /// Gets or sets ForumControlObj.
    /// </summary>
    public object ForumControlObj
    {
      get
      {
        return this._forumControlObj;
      }

      set
      {
        this._forumControlObj = value;
      }
    }

    /// <summary>
    /// The init.
    /// </summary>
    public void Init()
    {
      // hook the page init for mail sending...
      YafContext.Current.AfterInit += new EventHandler<EventArgs>(this.Current_AfterInit);
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose()
    {
    }

    #endregion

    /// <summary>
    /// The current_ after init.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Current_AfterInit(object sender, EventArgs e)
    {
      // add the mailing task if it's not already added...
      if (YafTaskModule.Current != null && !YafTaskModule.Current.TaskExists(UserCleanUpTask.TaskName))
      {
        // start it...
        YafTaskModule.Current.StartTask(UserCleanUpTask.TaskName, new UserCleanUpTask());
      }
    }
  }
}