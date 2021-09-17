// ***********************************************************************
// <copyright file="SqlMapper.DapperRow.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ServiceStack.OrmLite.Dapper
{
    /// <summary>
    /// Class SqlMapper.
    /// </summary>
    public static partial class SqlMapper
    {
        /// <summary>
        /// Class DapperRow. This class cannot be inherited.
        /// Implements the <see cref="object" />
        /// Implements the <see cref="object" />
        /// Implements the <see cref="System.Dynamic.IDynamicMetaObjectProvider" />
        /// </summary>
        /// <seealso cref="object" />
        /// <seealso cref="object" />
        /// <seealso cref="System.Dynamic.IDynamicMetaObjectProvider" />
        private sealed partial class DapperRow
            : IDictionary<string, object>
            , IReadOnlyDictionary<string, object>
        {
            /// <summary>
            /// The table
            /// </summary>
            private readonly DapperTable table;
            /// <summary>
            /// The values
            /// </summary>
            private object[] values;

            /// <summary>
            /// Initializes a new instance of the <see cref="DapperRow"/> class.
            /// </summary>
            /// <param name="table">The table.</param>
            /// <param name="values">The values.</param>
            /// <exception cref="System.ArgumentNullException">table</exception>
            /// <exception cref="System.ArgumentNullException">values</exception>
            public DapperRow(DapperTable table, object[] values)
            {
                this.table = table ?? throw new ArgumentNullException(nameof(table));
                this.values = values ?? throw new ArgumentNullException(nameof(values));
            }

            /// <summary>
            /// Class DeadValue. This class cannot be inherited.
            /// </summary>
            private sealed class DeadValue
            {
                /// <summary>
                /// The default
                /// </summary>
                public static readonly DeadValue Default = new DeadValue();
                /// <summary>
                /// Prevents a default instance of the <see cref="DeadValue"/> class from being created.
                /// </summary>
                private DeadValue() { /* hiding constructor */ }
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <value>The count.</value>
            int ICollection<KeyValuePair<string, object>>.Count
            {
                get
                {
                    int count = 0;
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (!(values[i] is DeadValue)) count++;
                    }
                    return count;
                }
            }

            /// <summary>
            /// Gets the value associated with the specified key.
            /// </summary>
            /// <param name="key">The key whose value to get.</param>
            /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
            /// <returns><see langword="true" /> if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, <see langword="false" />.</returns>
            public bool TryGetValue(string key, out object value)
                => TryGetValue(table.IndexOfName(key), out value);

            /// <summary>
            /// Tries the get value.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="value">The value.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            internal bool TryGetValue(int index, out object value)
            {
                if (index < 0)
                { // doesn't exist
                    value = null;
                    return false;
                }
                // exists, **even if** we don't have a value; consider table rows heterogeneous
                value = index < values.Length ? values[index] : null;
                if (value is DeadValue)
                { // pretend it isn't here
                    value = null;
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                var sb = GetStringBuilder().Append("{DapperRow");
                foreach (var kv in this)
                {
                    var value = kv.Value;
                    sb.Append(", ").Append(kv.Key);
                    if (value != null)
                    {
                        sb.Append(" = '").Append(kv.Value).Append('\'');
                    }
                    else
                    {
                        sb.Append(" = NULL");
                    }
                }

                return sb.Append('}').__ToStringRecycle();
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                var names = table.FieldNames;
                for (var i = 0; i < names.Length; i++)
                {
                    object value = i < values.Length ? values[i] : null;
                    if (!(value is DeadValue))
                    {
                        yield return new KeyValuePair<string, object>(names[i], value);
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region Implementation of ICollection<KeyValuePair<string,object>>

            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
            void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
            {
                IDictionary<string, object> dic = this;
                dic.Add(item.Key, item.Value);
            }

            /// <summary>
            /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            void ICollection<KeyValuePair<string, object>>.Clear()
            { // removes values for **this row**, but doesn't change the fundamental table
                for (int i = 0; i < values.Length; i++)
                    values[i] = DeadValue.Default;
            }

            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
            /// <returns><see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.</returns>
            bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
            {
                return TryGetValue(item.Key, out object value) && Equals(value, item.Value);
            }

            /// <summary>
            /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
            /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
            void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                foreach (var kv in this)
                {
                    array[arrayIndex++] = kv; // if they didn't leave enough space; not our fault
                }
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
            /// <returns><see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
            bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
            {
                IDictionary<string, object> dic = this;
                return dic.Remove(item.Key);
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;
            #endregion

            #region Implementation of IDictionary<string,object>

            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
            /// </summary>
            /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
            /// <returns><see langword="true" /> if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, <see langword="false" />.</returns>
            bool IDictionary<string, object>.ContainsKey(string key)
            {
                int index = table.IndexOfName(key);
                if (index < 0 || index >= values.Length || values[index] is DeadValue) return false;
                return true;
            }

            /// <summary>
            /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
            /// </summary>
            /// <param name="key">The object to use as the key of the element to add.</param>
            /// <param name="value">The object to use as the value of the element to add.</param>
            void IDictionary<string, object>.Add(string key, object value)
            {
                SetValue(key, value, true);
            }

            /// <summary>
            /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
            /// </summary>
            /// <param name="key">The key of the element to remove.</param>
            /// <returns><see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
            bool IDictionary<string, object>.Remove(string key)
                => Remove(table.IndexOfName(key));

            /// <summary>
            /// Removes the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            internal bool Remove(int index)
            {
                if (index < 0 || index >= values.Length || values[index] is DeadValue) return false;
                values[index] = DeadValue.Default;
                return true;
            }

            /// <summary>
            /// Gets or sets the <see cref="System.Object"/> with the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns>System.Object.</returns>
            object IDictionary<string, object>.this[string key]
            {
                get { TryGetValue(key, out object val); return val; }
                set { SetValue(key, value, false); }
            }

            /// <summary>
            /// Sets the value.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            /// <returns>System.Object.</returns>
            public object SetValue(string key, object value)
            {
                return SetValue(key, value, false);
            }

            /// <summary>
            /// Sets the value.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            /// <param name="isAdd">if set to <c>true</c> [is add].</param>
            /// <returns>System.Object.</returns>
            /// <exception cref="System.ArgumentNullException">key</exception>
            /// <exception cref="System.ArgumentException">An item with the same key has already been added - key</exception>
            private object SetValue(string key, object value, bool isAdd)
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                int index = table.IndexOfName(key);
                if (index < 0)
                {
                    index = table.AddField(key);
                }
                else if (isAdd && index < values.Length && !(values[index] is DeadValue))
                {
                    // then semantically, this value already exists
                    throw new ArgumentException("An item with the same key has already been added", nameof(key));
                }
                return SetValue(index, value);
            }
            /// <summary>
            /// Sets the value.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="value">The value.</param>
            /// <returns>System.Object.</returns>
            internal object SetValue(int index, object value)
            {
                int oldLength = values.Length;
                if (oldLength <= index)
                {
                    // we'll assume they're doing lots of things, and
                    // grow it to the full width of the table
                    Array.Resize(ref values, table.FieldCount);
                    for (int i = oldLength; i < values.Length; i++)
                    {
                        values[i] = DeadValue.Default;
                    }
                }
                return values[index] = value;
            }

            /// <summary>
            /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
            /// </summary>
            /// <value>The keys.</value>
            ICollection<string> IDictionary<string, object>.Keys
            {
                get { return this.Select(kv => kv.Key).ToArray(); }
            }

            /// <summary>
            /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
            /// </summary>
            /// <value>The values.</value>
            ICollection<object> IDictionary<string, object>.Values
            {
                get { return this.Select(kv => kv.Value).ToArray(); }
            }

            #endregion


            #region Implementation of IReadOnlyDictionary<string,object>


            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <value>The count.</value>
            int IReadOnlyCollection<KeyValuePair<string, object>>.Count
            {
                get
                {
                    return values.Count(t => !(t is DeadValue));
                }
            }

            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
            /// </summary>
            /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
            /// <returns><see langword="true" /> if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, <see langword="false" />.</returns>
            bool IReadOnlyDictionary<string, object>.ContainsKey(string key)
            {
                int index = table.IndexOfName(key);
                return index >= 0 && index < values.Length && !(values[index] is DeadValue);
            }

            /// <summary>
            /// Gets the <see cref="System.Object"/> with the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns>System.Object.</returns>
            object IReadOnlyDictionary<string, object>.this[string key]
            {
                get { TryGetValue(key, out object val); return val; }
            }

            /// <summary>
            /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
            /// </summary>
            /// <value>The keys.</value>
            IEnumerable<string> IReadOnlyDictionary<string, object>.Keys
            {
                get { return this.Select(kv => kv.Key); }
            }

            /// <summary>
            /// Gets the values.
            /// </summary>
            /// <value>The values.</value>
            IEnumerable<object> IReadOnlyDictionary<string, object>.Values
            {
                get { return this.Select(kv => kv.Value); }
            }

            #endregion
        }
    }
}
