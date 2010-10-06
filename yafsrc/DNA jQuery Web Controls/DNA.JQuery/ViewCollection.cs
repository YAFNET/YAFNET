///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System.Collections;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace DNA.UI.JQuery
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ViewCollection : CollectionBase
    {
        private readonly JQueryMultiViewControl _parent;

        public JQueryMultiViewControl Parent
        {
            get { return _parent; }
        }

        public ViewCollection(JQueryMultiViewControl parent)
        {
            _parent = parent;
        }

        public void Add(View view)
        {
            view.Index = InnerList.Count;
            view.ParentContainer = _parent;
            InnerList.Add(view);
            Parent.Controls.Add(view);
         }

        public void AddAt(int index, View view)
        {
            view.ParentContainer = _parent;
            InnerList.Insert(index, view);
            Reindex();
            Parent.Controls.AddAt(index, view);
        }

        internal void Reindex()
        {
            int i = 0;
            foreach (View view in InnerList)
                view.Index = i++;
        }

        public void Remove(View view)
        {
            InnerList.Remove(view);
            Parent.Controls.Remove(view);
            Reindex();
        }

        public new void RemoveAt(int index)
        {
            InnerList.RemoveAt(index);
            Parent.Controls.RemoveAt(index);
            Reindex();
        }

         public View this[int index]
        {
            get
            {
                if (InnerList.Count <= index)
                    return null;
                return InnerList[index] as View;
            }
        }

        public View this[string viewId]
        {
            get
            { return InnerList.Cast<View>().FirstOrDefault(view => (view.ID == viewId) || (view.ClientID == viewId)); }
        }
    }
}
