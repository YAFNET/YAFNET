using System.Collections;
using System.Security.Permissions;
using System;

namespace YAF.Controls.QuickView
{
    public class QuickViewItemCollection : ICollection, IEnumerable
    {
        #region Fields
        private ArrayList array;
        #endregion	// Fields

        #region Public Constructors
        public QuickViewItemCollection(ArrayList items)
        {
            array = items;
        }
        #endregion	// Public Constructors


        #region Public Instance Properties
        public int Count
        {
            get
            {
                return array.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return array.IsReadOnly;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return array.IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return array.SyncRoot;
            }
        }

        public QuickViewItem this[int index]
        {
            get
            {
                return (QuickViewItem)array[index];
            }
        }
        #endregion	// Public Instance Properties

        #region Public Instance Methods
        public void Add(QuickViewItem dvItem)
        {
            array.Add(dvItem);
        }

        public void CopyTo(System.Array array, int index)
        {
            if (!(array is QuickViewItem[]))
            {
                throw new InvalidCastException("Target array must be QuickViewItem[]");
            }

            if ((index + this.array.Count) > array.Length)
            {
                throw new IndexOutOfRangeException("Target array not large enough to hold copied array.");
            }
            this.array.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return array.GetEnumerator();
        }
        #endregion	// Public Instance Methods
    }
}
