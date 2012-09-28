/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Core
{
  #region Using

  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The base forum module.
  /// </summary>
  public abstract class BaseForumModule : IBaseForumModule, IHaveServiceLocator, IHaveLocalization
  {
    #region Properties

    /// <summary>
    ///   Gets a value indicating whether Active.
    /// </summary>
    public virtual bool Active
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    ///   Gets Description.
    /// </summary>
    public virtual string Description
    {
      get
      {
        return this.GetType().GetAttribute<YafModule>().ModuleName;
      }
    }

    /// <summary>
    ///   Gets or sets ForumControlObj.
    /// </summary>
    public virtual object ForumControlObj { get; set; }

    /// <summary>
    ///   Gets ModuleId.
    /// </summary>
    [NotNull]
    public virtual string ModuleId
    {
      get
      {
        return this.Description.GetHashCode().ToString();
      }
    }

    /// <summary>
    /// Gets PageContext.
    /// </summary>
    public YafContext PageContext
    {
      get
      {
        return YafContext.Current;
      }
    }

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public virtual IServiceLocator ServiceLocator
    {
      get
      {
        return YafContext.Current.ServiceLocator;
      }
    }

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public virtual ILocalization Localization
    {
      get
      {
        return this.Get<ILocalization>();
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IBaseForumModule

    /// <summary>
    /// The initialization function.
    /// </summary>
    public virtual void Init()
    {
      // do nothing... 
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    public virtual void Dispose()
    {
      // no default implementation
    }

    #endregion

    #endregion
  }
}