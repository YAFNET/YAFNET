///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;

namespace DNA.UI
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class StateManagedObjectCollection<T>:CollectionBase,IStateManager
        where T:StateManagedObject
    {
        private bool _Tracking = false;

        public virtual void Add(T item)
        {
            InnerList.Add(item);
            if (((IStateManager)this).IsTrackingViewState)
            {
                ((IStateManager)item).TrackViewState();
                item.SetViewStateDirty();
            }
        }

        public void Remove(T item)
        {
            InnerList.Remove(item);
        }

        public T this[int index]
        {
            get
            {
                return InnerList[index] as T;
            }
        }

        #region IStateManager Members

        bool IStateManager.IsTrackingViewState
        {
            get { return this._Tracking; }
        }

        void IStateManager.LoadViewState(object state)
        {
            object[] bags = state as object[];
            foreach (object bag in bags)
            {
                if (bag != null)
                {
                    T item = Activator.CreateInstance<T>();
                    ((IStateManager)item).LoadViewState(bag);
                    this.Add(item);
                }
            }
        }

        object IStateManager.SaveViewState()
        {
            object[] bags = new object[Count];
            int index = 0;
            foreach (T item in InnerList)
                bags[index++] = ((IStateManager)item).SaveViewState();
            return bags;
        }

        void IStateManager.TrackViewState()
        {
            this._Tracking = true;
            foreach (T item in InnerList)
                ((IStateManager)item).TrackViewState();
        }

        #endregion
    }
}
