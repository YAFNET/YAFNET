//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.JQuery
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// The nav item event args.
  /// </summary>
  public class NavItemEventArgs : EventArgs
  {
    #region Constants and Fields

    /// <summary>
    /// The _item.
    /// </summary>
    private readonly NavItem _item;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NavItemEventArgs"/> class.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    public NavItemEventArgs(NavItem item)
    {
      this._item = item;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets Item.
    /// </summary>
    public NavItem Item
    {
      get
      {
        return this._item;
      }
    }

    #endregion
  }

  /// <summary>
  /// The nav item event handler.
  /// </summary>
  /// <param name="sender">
  /// The sender.
  /// </param>
  /// <param name="e">
  /// The e.
  /// </param>
  public delegate void NavItemEventHandler(object sender, NavItemEventArgs e);
}