///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Security.Permissions;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace DNA.UI.JQuery
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [TypeConverter(typeof(CollectionConverter))]
    public class NavItemCollection:CollectionBase,IStateManager
    {
        private bool _Tracking = false;
        private NavView _parent;

        /// <summary>
        ///  Gets the Parent of this Collections
        /// </summary>
        public NavView Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>
        /// Initialize the NavItemCollection
        /// </summary>
        /// <param name="parent"></param>
        public NavItemCollection(NavView parent)
        {
            _parent = parent;
        }

        

        /// <summary>
        /// Add the NavItem Instance to Collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(NavItem item)
        {
            item.Index = InnerList.Count;
            InnerList.Add(item);
            if (((IStateManager)this).IsTrackingViewState)
            {
                ((IStateManager)item).TrackViewState();
                item.SetViewStateDirty();
            }
        }

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
                InnerList.AddRange(items);
            }
        }

        internal void Reindex()
        {
            int i = 0;
            foreach (NavItem item in InnerList)
                item.Index = i++;
        }

        /// <summary>
        /// Remove the NavItem Instance in Collection
        /// </summary>
        /// <param name="item"></param>
        public void Remove(NavItem item)
        {
            InnerList.Remove(item);
            Reindex();
        }

        /// <summary>
        /// Gets the NavItem Instance by Index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public NavItem this[int index]
        {
            get
            {
                return InnerList[index] as NavItem;
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
                    NavItem item = new NavItem();
                    ((IStateManager)item).LoadViewState(bag);
                    this.Add(item);
                }
            }
        }

        object IStateManager.SaveViewState()
        {
            object[] bags = new object[Count];
            int index = 0;
            foreach (NavItem item in InnerList)
                bags[index++] = ((IStateManager)item).SaveViewState();
            return bags;
        }

        void IStateManager.TrackViewState()
        {
            this._Tracking = true;
            foreach (NavItem item in InnerList)
                ((IStateManager)item).TrackViewState();
        }

        #endregion
    }
}
