using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.ComponentModel;

namespace YAF.Controls.QuickView
{
    public sealed class QuickViewColumnCollection : ICollection, IStateManager
    {
        public QuickViewColumnCollection(QuickView owner, ArrayList columns)
        {
            this.owner = owner;
            this.columns = columns;
        }


        public void Add(QuickViewColumn column)
        {
            columns.Add(column);
            column.Set_Owner(owner);
            if (track)
                ((IStateManager)column).TrackViewState();
        }

        public void AddAt(int index, QuickViewColumn column)
        {
            columns.Insert(index, column);
            column.Set_Owner(owner);
            if (track)
                ((IStateManager)column).TrackViewState();
        }

        public void Clear()
        {
            columns.Clear();
        }

        public void CopyTo(Array array, int index)
        {
            columns.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return columns.GetEnumerator();
        }

        public int IndexOf(QuickViewColumn column)
        {
            return columns.IndexOf(column);
        }

        [Obsolete("figure out what you need with me")]
        internal void OnColumnsChanged()
        {
            // Do something
        }

        public void Remove(QuickViewColumn column)
        {
            columns.Remove(column);
        }

        public void RemoveAt(int index)
        {
            columns.RemoveAt(index);
        }

        void System.Web.UI.IStateManager.LoadViewState(object savedState)
        {
            object[] o = (object[])savedState;
            if (o == null)
                return;

            int i = 0;
            foreach (IStateManager ism in this)
                ism.LoadViewState(o[i++]);
        }

        object System.Web.UI.IStateManager.SaveViewState()
        {
            object[] o = new object[Count];

            int i = 0;
            foreach (IStateManager ism in this)
                o[i++] = ism.SaveViewState();

            foreach (object a in o)
                if (a != null)
                    return o;
            return null;
        }

        void System.Web.UI.IStateManager.TrackViewState()
        {
            track = true;
            foreach (IStateManager ism in this)
                ism.TrackViewState();
        }

        [Browsable(false)]
        public int Count
        {
            get { return columns.Count; }
        }

        bool IStateManager.IsTrackingViewState
        {
            get { return track; }
        }

        [Browsable(false)]
        public bool IsReadOnly
        {
            get { return columns.IsReadOnly; }
        }

        [Browsable(false)]
        public bool IsSynchronized
        {
            get { return columns.IsSynchronized; }
        }

        [Browsable(false)]
        public QuickViewColumn this[int index]
        {
            get { return (QuickViewColumn)columns[index]; }
        }

        [Browsable(false)]
        public object SyncRoot
        {
            get { return columns.SyncRoot; }
        }

        QuickView owner;
        ArrayList columns;
        bool track;
    }
}
