/* Yet Another Forum.net
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

  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The init services module.
  /// </summary>
  [YafModule("Start Stop Watch Module", "Tiny Gecko", 1)]
  public class StartStopWatch : BaseForumModule
  {
    #region Constants and Fields

    /// <summary>
    /// The _stop watch.
    /// </summary>
    private readonly IStopWatch _stopWatch;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StartStopWatch"/> class.
    /// </summary>
    /// <param name="stopWatch">
    /// The stop watch.
    /// </param>
    public StartStopWatch([NotNull] IStopWatch stopWatch)
    {
      this._stopWatch = stopWatch;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The init.
    /// </summary>
    public override void Init()
    {
      // initialize the base services before anyone notices...
      this._stopWatch.Start();

      // hook unload...
      this.PageContext.PageUnload += this.Current_PageUnload;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The current_ page unload.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Current_PageUnload([NotNull] object sender, [NotNull] EventArgs e)
    {
      // stop the stop watch in case the footer did not...
      this._stopWatch.Stop();
    }

    #endregion
  }
}