//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery.Design
{
  #region Using

  using System;

  using DNA.UI.Design;

  #endregion

  /// <summary>
  /// The nav item collection editor.
  /// </summary>
  public class NavItemCollectionEditor : ItemCollectionEditor
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NavItemCollectionEditor"/> class.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    public NavItemCollectionEditor(Type type)
      : base(type)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The create collection item type.
    /// </summary>
    /// <returns>
    /// </returns>
    protected override Type CreateCollectionItemType()
    {
      return typeof(NavItem);
    }

    /// <summary>
    /// The get display text.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <returns>
    /// The get display text.
    /// </returns>
    protected override string GetDisplayText(object value)
    {
      var item = value as NavItem;
      if (item != null)
      {
        if (!string.IsNullOrEmpty(item.Text))
        {
          return item.Text;
        }
      }

      return base.GetDisplayText(value);
    }

    #endregion
  }
}