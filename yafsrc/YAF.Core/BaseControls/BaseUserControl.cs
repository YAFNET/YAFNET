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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Web.UI;

  using YAF.Types.Attributes;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The base user control.
  /// </summary>
  public class BaseUserControl : UserControl, IRaiseControlLifeCycles, IHaveServiceLocator
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseUserControl"/> class. 
    ///   Initializes a new instance of the <see cref="BaseControl"/> class.
    /// </summary>
    public BaseUserControl()
    {
      this.Get<IInjectServices>().Inject(this);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Logger.
    /// </summary>
    [Inject]
    public ILogger Logger { get; set; }

    /// <summary>
    ///   Gets PageContext.
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
    public IServiceLocator ServiceLocator
    {
      get
      {
        return this.PageContext.ServiceLocator;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IRaiseControlLifeCycles

    /// <summary>
    /// The raise init.
    /// </summary>
    void IRaiseControlLifeCycles.RaiseInit()
    {
      this.OnInit(new EventArgs());
    }

    /// <summary>
    /// The raise load.
    /// </summary>
    void IRaiseControlLifeCycles.RaiseLoad()
    {
      this.OnLoad(new EventArgs());
    }

    /// <summary>
    /// The raise pre render.
    /// </summary>
    void IRaiseControlLifeCycles.RaisePreRender()
    {
      this.OnPreRender(new EventArgs());
    }

    #endregion

    #endregion
  }
}