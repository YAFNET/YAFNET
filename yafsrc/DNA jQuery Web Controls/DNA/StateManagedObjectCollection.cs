//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI
{
  #region Using

  using System;
  using System.Collections;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI;

  #endregion

  /// <summary>
  /// The state managed object collection.
  /// </summary>
  /// <typeparam name="T">
  /// </typeparam>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class StateManagedObjectCollection<T> : CollectionBase, IStateManager
    where T : StateManagedObject
  {
    #region Constants and Fields

    /// <summary>
    /// The _ tracking.
    /// </summary>
    private bool _Tracking;

    #endregion

    #region Properties

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
    /// The this.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    public T this[int index]
    {
      get
      {
        return this.InnerList[index] as T;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The add.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    public virtual void Add(T item)
    {
      this.InnerList.Add(item);
      if (((IStateManager)this).IsTrackingViewState)
      {
        ((IStateManager)item).TrackViewState();
        item.SetViewStateDirty();
      }
    }

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    public void Remove(T item)
    {
      this.InnerList.Remove(item);
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
          var item = Activator.CreateInstance<T>();
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
      foreach (T item in this.InnerList)
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
      foreach (T item in this.InnerList)
      {
        ((IStateManager)item).TrackViewState();
      }
    }

    #endregion

    #endregion
  }
}