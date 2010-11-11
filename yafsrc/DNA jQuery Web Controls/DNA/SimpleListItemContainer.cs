//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI
{
  #region Using

  using System.ComponentModel;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  #endregion

  /// <summary>
  /// The simple list item container.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxItem(false)]
  public class SimpleListItemContainer : WebControl, INamingContainer, IDataItemContainer
  {
    #region Constants and Fields

    /// <summary>
    /// The data item.
    /// </summary>
    private readonly object dataItem;

    /// <summary>
    /// The data item index.
    /// </summary>
    private readonly int dataItemIndex;

    /// <summary>
    /// The display index.
    /// </summary>
    private readonly int displayIndex;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleListItemContainer"/> class.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <param name="dataIndex">
    /// The data index.
    /// </param>
    public SimpleListItemContainer(SimpleListItem item, int dataIndex)
    {
      this.dataItem = item;
      this.dataItemIndex = dataIndex;
      this.displayIndex = dataIndex;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets DataItem.
    /// </summary>
    public object DataItem
    {
      get
      {
        return this.dataItem;
      }
    }

    /// <summary>
    /// Gets DataItemIndex.
    /// </summary>
    public int DataItemIndex
    {
      get
      {
        return this.dataItemIndex;
      }
    }

    /// <summary>
    /// Gets DisplayIndex.
    /// </summary>
    public int DisplayIndex
    {
      get
      {
        return this.displayIndex;
      }
    }

    /// <summary>
    /// Gets TagKey.
    /// </summary>
    protected override HtmlTextWriterTag TagKey
    {
      get
      {
        return HtmlTextWriterTag.Li;
      }
    }

    #endregion
  }
}