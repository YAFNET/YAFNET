/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Web.UI;

  using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// Summary description for BaseControl.
  /// </summary>
  public class BaseControl : Control, IRaiseControlLifeCycles, IHaveServiceLocator, IHaveLocalization
  {
    #region Constants and Fields

    /// <summary>
    ///   The _localization.
    /// </summary>
    private ILocalization _localization;

    /// <summary>
    /// The _logger.
    /// </summary>
    private ILogger _logger;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "BaseControl" /> class.
    /// </summary>
    public BaseControl()
    {
      this.Get<IInjectServices>().Inject(this);
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization
    {
      get
      {
        return this._localization ?? (this._localization = this.Get<ILocalization>());
      }
    }

    /// <summary>
    ///   Gets or sets Logger.
    /// </summary>
    public ILogger Logger
    {
      get
      {
        return this._logger ?? (this._logger = this.Get<ILoggerProvider>().Create(this.GetType()));
      }
    }

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