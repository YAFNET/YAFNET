//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI
{
  #region Using

  using System;
  using System.Reflection;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI;

  #endregion

  /// <summary>
  /// The state managed object.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class StateManagedObject : IStateManager, ICloneable
  {
    #region Constants and Fields

    /// <summary>
    /// The _ is tracking view state.
    /// </summary>
    private bool _IsTrackingViewState;

    /// <summary>
    /// The _ view state.
    /// </summary>
    private StateBag _ViewState;

    #endregion

    #region Properties

    /// <summary>
    ///   An instance of the StateBag class that contains the view state information.
    /// </summary>
    protected StateBag ViewState
    {
      get
      {
        // To concerve resources, especially on the page,
        // only create the view state when needed.
        if (this._ViewState == null)
        {
          this._ViewState = new StateBag();
          if (((IStateManager)this).IsTrackingViewState)
          {
            ((IStateManager)this._ViewState).TrackViewState();
          }
        }

        return this._ViewState;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether a server control is tracking its view state changes.
    /// </summary>
    bool IStateManager.IsTrackingViewState
    {
      get
      {
        return this._IsTrackingViewState;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The set view state clean.
    /// </summary>
    public virtual void SetViewStateClean()
    {
      if (this._ViewState != null)
      {
        this._ViewState.SetDirty(false);
      }
    }

    /// <summary>
    /// The set view state dirty.
    /// </summary>
    public virtual void SetViewStateDirty()
    {
      if (this._ViewState != null)
      {
        this._ViewState.SetDirty(true);
      }
    }

    #endregion

    #region Implemented Interfaces

    #region ICloneable

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public virtual object Clone()
    {
      var copy =
        (StateManagedObject)
        Activator.CreateInstance(
          this.GetType(), 
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, 
          null, 
          null, 
          null);

      // Merge in the properties from this object into the copy
      copy._IsTrackingViewState = this._IsTrackingViewState;

      if (this._ViewState != null)
      {
        StateBag viewState = copy.ViewState;
        foreach (string key in this.ViewState.Keys)
        {
          object item = this.ViewState[key];
          if (item is ICloneable)
          {
            item = ((ICloneable)item).Clone();
          }

          viewState[key] = item;
        }
      }

      return copy;
    }

    #endregion

    #region IStateManager

    /// <summary>
    /// Loads the node's previously saved view state.
    /// </summary>
    /// <param name="state">
    /// An Object that contains the saved view state values for the node.
    /// </param>
    void IStateManager.LoadViewState(object state)
    {
      this.LoadViewState(state);
    }

    /// <summary>
    /// Saves the changes to the node's view state to an Object.
    /// </summary>
    /// <returns>
    /// The Object that contains the view state changes.
    /// </returns>
    object IStateManager.SaveViewState()
    {
      return this.SaveViewState();
    }

    /// <summary>
    /// Instructs the node to track changes to its view state.
    /// </summary>
    void IStateManager.TrackViewState()
    {
      this.TrackViewState();
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Loads the node's previously saved view state.
    /// </summary>
    /// <param name="state">
    /// An Object that contains the saved view state values for the node.
    /// </param>
    protected virtual void LoadViewState(object state)
    {
      if (state != null)
      {
        ((IStateManager)this.ViewState).LoadViewState(state);
      }
    }

    /// <summary>
    /// Saves the changes to the node's view state to an Object.
    /// </summary>
    /// <returns>
    /// The Object that contains the view state changes.
    /// </returns>
    protected virtual object SaveViewState()
    {
      if (this._ViewState != null)
      {
        return ((IStateManager)this._ViewState).SaveViewState();
      }

      return null;
    }

    /// <summary>
    /// Instructs the node to track changes to its view state.
    /// </summary>
    protected virtual void TrackViewState()
    {
      this._IsTrackingViewState = true;

      if (this._ViewState != null)
      {
        ((IStateManager)this._ViewState).TrackViewState();
      }
    }

    #endregion
  }
}