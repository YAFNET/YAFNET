//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System.Collections;
  using System.ComponentModel;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI;

  #endregion

  /// <summary>
  /// The nav item collection.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [TypeConverter(typeof(CollectionConverter))]
  public class NavItemCollection : CollectionBase, IStateManager
  {
    #region Constants and Fields

    /// <summary>
    /// The _parent.
    /// </summary>
    private readonly NavView _parent;

    /// <summary>
    /// The _ tracking.
    /// </summary>
    private bool _Tracking;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NavItemCollection"/> class. 
    /// Initialize the NavItemCollection
    /// </summary>
    /// <param name="parent">
    /// </param>
    public NavItemCollection(NavView parent)
    {
      this._parent = parent;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets the Parent of this Collections
    /// </summary>
    public NavView Parent
    {
      get
      {
        return this._parent;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsTrackingViewState.
    /// </summary>
    bool IStateManager.IsTrackingViewState
    {
      get
      {
        return this._Tracking;
      }
    }

    #endregion

    #region Indexers

    /// <summary>
    ///   Gets the NavItem Instance by Index
    /// </summary>
    /// <param name = "index"></param>
    /// <returns></returns>
    public NavItem this[int index]
    {
      get
      {
        return this.InnerList[index] as NavItem;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Add the NavItem Instance to Collection
    /// </summary>
    /// <param name="item">
    /// </param>
    public void Add(NavItem item)
    {
      item.Index = this.InnerList.Count;
      this.InnerList.Add(item);
      if (((IStateManager)this).IsTrackingViewState)
      {
        ((IStateManager)item).TrackViewState();
        item.SetViewStateDirty();
      }
    }

    /// <summary>
    /// The add range.
    /// </summary>
    /// <param name="items">
    /// The items.
    /// </param>
    public void AddRange(ICollection items)
    {
      if (items != null)
      {
        IEnumerator en = items.GetEnumerator();
        while (en.MoveNext())
        {
          if (((IStateManager)en.Current).IsTrackingViewState)
          {
            ((IStateManager)en.Current).TrackViewState();
            ((StateManagedObject)en.Current).SetViewStateDirty();
          }
        }

        this.InnerList.AddRange(items);
      }
    }

    /// <summary>
    /// Remove the NavItem Instance in Collection
    /// </summary>
    /// <param name="item">
    /// </param>
    public void Remove(NavItem item)
    {
      this.InnerList.Remove(item);
      this.Reindex();
    }

    #endregion

    #region Implemented Interfaces

    #region IStateManager

    /// <summary>
    /// The load view state.
    /// </summary>
    /// <param name="state">
    /// The state.
    /// </param>
    void IStateManager.LoadViewState(object state)
    {
      var bags = state as object[];
      foreach (object bag in bags)
      {
        if (bag != null)
        {
          var item = new NavItem();
          ((IStateManager)item).LoadViewState(bag);
          this.Add(item);
        }
      }
    }

    /// <summary>
    /// The save view state.
    /// </summary>
    /// <returns>
    /// The save view state.
    /// </returns>
    object IStateManager.SaveViewState()
    {
      var bags = new object[this.Count];
      int index = 0;
      foreach (NavItem item in this.InnerList)
      {
        bags[index++] = ((IStateManager)item).SaveViewState();
      }

      return bags;
    }

    /// <summary>
    /// The track view state.
    /// </summary>
    void IStateManager.TrackViewState()
    {
      this._Tracking = true;
      foreach (NavItem item in this.InnerList)
      {
        ((IStateManager)item).TrackViewState();
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The reindex.
    /// </summary>
    internal void Reindex()
    {
      int i = 0;
      foreach (NavItem item in this.InnerList)
      {
        item.Index = i++;
      }
    }

    #endregion
  }
}