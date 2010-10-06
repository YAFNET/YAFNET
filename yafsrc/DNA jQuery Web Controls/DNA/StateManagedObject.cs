
///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml;
using System.Security.Permissions;

namespace DNA.UI
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class StateManagedObject : IStateManager, ICloneable
    {
        private bool _IsTrackingViewState = false;
        private StateBag _ViewState;
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public virtual object Clone()
        {
            StateManagedObject copy = (StateManagedObject)Activator.CreateInstance(this.GetType(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);

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

        #region 状态管理
      
        public virtual void SetViewStateDirty()
        {
            if (_ViewState != null)
                _ViewState.SetDirty(true);
        }

        public virtual void SetViewStateClean()
        {
            if (_ViewState != null)
                _ViewState.SetDirty(false);
        }

        /// <summary>
        /// An instance of the StateBag class that contains the view state information.
        /// </summary>
        protected StateBag ViewState
        {
            get
            {
                // To concerve resources, especially on the page,
                // only create the view state when needed.
                if (_ViewState == null)
                {
                    _ViewState = new StateBag();
                    if (((IStateManager)this).IsTrackingViewState)
                    {
                        ((IStateManager)_ViewState).TrackViewState();
                    }
                }

                return _ViewState;
            }
        }

        /// <summary>
        /// Loads the node's previously saved view state.
        /// </summary>
        /// <param name="state">An Object that contains the saved view state values for the node.</param>
        void IStateManager.LoadViewState(object state)
        {
            ((StateManagedObject)this).LoadViewState(state);
        }

        /// <summary>
        /// Loads the node's previously saved view state.
        /// </summary>
        /// <param name="state">An Object that contains the saved view state values for the node.</param>
        protected virtual void LoadViewState(object state)
        {
            if (state != null)
            {
                ((IStateManager)ViewState).LoadViewState(state);
            }
        }

        /// <summary>
        /// Saves the changes to the node's view state to an Object.
        /// </summary>
        /// <returns>The Object that contains the view state changes.</returns>
        object IStateManager.SaveViewState()
        {
            return ((StateManagedObject)this).SaveViewState();
        }

        /// <summary>
        /// Saves the changes to the node's view state to an Object.
        /// </summary>
        /// <returns>The Object that contains the view state changes.</returns>
        protected virtual object SaveViewState()
        {
            if (_ViewState != null)
            {
                return ((IStateManager)_ViewState).SaveViewState();
            }

            return null;
        }

        /// <summary>
        /// Instructs the node to track changes to its view state.
        /// </summary>
        void IStateManager.TrackViewState()
        {
            ((StateManagedObject)this).TrackViewState();
        }

        /// <summary>
        /// Instructs the node to track changes to its view state.
        /// </summary>
        protected virtual void TrackViewState()
        {
            _IsTrackingViewState = true;

            if (_ViewState != null)
            {
                ((IStateManager)_ViewState).TrackViewState();
            }
        }

        /// <summary>
        /// Gets a value indicating whether a server control is tracking its view state changes.
        /// </summary>
        bool IStateManager.IsTrackingViewState
        {
            get { return _IsTrackingViewState; }
        }
        #endregion
    }
}
