/* YetAnotherForum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Core.Services.Startup
{
  #region Using

    using System;

    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The root service.
  /// </summary>
  public abstract class BaseStartupService : IStartupService
  {
    #region Properties

    /// <summary>
    ///   Gets a value indicating whether Initialized.
    /// </summary>
    public virtual bool Initialized
    {
      get
      {
        if (YafContext.Current[this.InitVarName] == null)
        {
          return false;
        }

        return Convert.ToBoolean(YafContext.Current[this.InitVarName]);
      }

      private set
      {
        YafContext.Current[this.InitVarName] = value;
      }
    }

    /// <summary>
    ///   Gets Priority.
    /// </summary>
    public virtual int Priority
    {
      get
      {
        return 1000;
      }
    }

    /// <summary>
    ///   Gets InitVarName.
    /// </summary>
    protected abstract string InitVarName { get; }

    #endregion

    #region Implemented Interfaces

    #region IStartupService

    /// <summary>
    /// The run.
    /// </summary>
    public void Run()
    {
      if (!this.Initialized)
      {
        this.Initialized = this.RunService();
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The run service.
    /// </summary>
    /// <returns>
    /// The run service.
    /// </returns>
    protected abstract bool RunService();

    #endregion
  }
}