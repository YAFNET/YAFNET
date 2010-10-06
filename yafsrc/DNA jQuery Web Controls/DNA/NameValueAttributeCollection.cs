
///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace DNA.UI
{
    public class NameValueAttributeCollection<T>:CollectionBase,IStateManager
        where T:NameValueAttribute
    {
        private bool _Tracking = false;

        public void Add(T attr)
        {
            if (!this.Contains(attr))
                InnerList.Add(attr);
            else
                throw new Exception("The attribute is exists!");

            if (((IStateManager)this).IsTrackingViewState)
            {
                ((IStateManager)attr).TrackViewState();
                attr.SetViewStateDirty();
            }
        }

        public void Add(string name, string value)
        {
            T instance = Activator.CreateInstance<T>();
            instance.Name = name;
            instance.Value = value;
            Add(instance);
        }

        public bool Contains(string name)
        {
            return this[name] != null;
        }

        public bool Contains(T attribute)
        {
           return InnerList.Contains(attribute);
        }

        public string ToJSONString()
        {
            StringBuilder jsonStr = new StringBuilder();
            foreach (T attr in InnerList)
            {
                if (jsonStr.Length > 0)
                    jsonStr.Append(",");
                jsonStr.Append(attr.Name + ":\"" + attr.Value + "\"");
            }

            if (jsonStr.Length > 0)
                return "{" + jsonStr.ToString() + "}";
            else
                return "{}";
        }
        public T this[string name]
        {
            get 
            {
                foreach (T attr in InnerList)
                {

                    if (attr.Name.ToLower() == name.ToLower())
                        return attr;
                }
                return null;
            }
        }

        public T this[int index]
        {
            get
            {
                return InnerList[index] as T;
            }
        }

        public void Remove(T attr)
        {
            InnerList.Remove(attr);
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
                    T attr = Activator.CreateInstance<T>();
                    ((IStateManager)attr).LoadViewState(bag);
                    this.Add(attr);
                }
            }
        }

        object IStateManager.SaveViewState()
        {
            object[] bags = new object[Count];
            int index = 0;
            foreach (NameValueAttribute item in InnerList)
                bags[index++] = ((IStateManager)item).SaveViewState();
            return bags;
        }

        void IStateManager.TrackViewState()
        {
            this._Tracking = true;
            foreach (NameValueAttribute item in InnerList)
                ((IStateManager)item).TrackViewState();
        }

        #endregion
    }
}
