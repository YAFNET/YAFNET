/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Utils.Structures
{
  using System;
  using System.Collections;
  using System.Threading;

  using YAF.Types;

  /// <summary>
  /// Class that represents a doubly linked list (I can't believe that .NET didn't 
  ///   have one of these).  The primary usage for this class is with the Most Recently Used class,
  ///   but can be used in a variety of scenarios.
  /// </summary>
  [Serializable]
  public class DoubleLinkedList : IList
  {
    #region Constants and Fields

    /// <summary>
    /// The head link.
    /// </summary>
    public LinkItem HeadLink;

    /// <summary>
    /// The tail link.
    /// </summary>
    public LinkItem TailLink;

    /// <summary>
    /// The m_count.
    /// </summary>
    private int m_count;

    #endregion

    #region Properties

    /// <summary>
    /// Gets Count.
    /// </summary>
    public int Count => this.m_count;

    /// <summary>
    /// Gets a value indicating whether IsFixedSize.
    /// </summary>
    public bool IsFixedSize => false;

    /// <summary>
    /// Gets a value indicating whether IsReadOnly.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets a value indicating whether IsSynchronized.
    /// </summary>
    public bool IsSynchronized => false;

    /// <summary>
    /// Gets SyncRoot.
    /// </summary>
    public object SyncRoot { get; } = new object();

    #endregion

    #region Indexers

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    public object this[int index]
    {
      get
      {
        int i;
        LinkItem current;
        object item = null;

        // Skip to the index
        for (i = 0, current = this.HeadLink; current != null && i < index; i++, current = current.Next)
        {
        }

        if (i == index && current != null)
        {
          item = current.Item;
        }

        return item;
      }

      set
      {
        int i;
        LinkItem current;

        // Skip past existing items
        for (i = 0, current = this.HeadLink; current != null && i < index; i++, current = current.Next)
        {
        }

        if (i == index && current != null)
        {
          current.Item = value;
        }
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Append the specified value to the tail of the list (not a LinkItem)
    /// </summary>
    /// <param name="value">
    /// The value to append to the list
    /// </param>
    /// <returns>
    /// The new LinkItem which points to the value
    /// </returns>
    public LinkItem Append(object value)
    {
      // Append this item to the tail
      if (this.TailLink != null)
      {
        this.TailLink = new LinkItem(null, this.TailLink, value);
        this.TailLink.Previous.Next = this.TailLink;
      }

      this.HeadLink ??= this.TailLink;

      // Increment the count
      Interlocked.Increment(ref this.m_count);

      return this.TailLink;
    }

    /// <summary>
    /// Move the specified LinkItem to the head of the list
    /// </summary>
    /// <param name="item">
    /// The existing LinkItem in the list
    /// </param>
    public void MoveToHead(LinkItem item)
    {
      if (item == null)
      {
        return;
      }

      if (item == this.HeadLink)
      {
          return;
      }

      var prev = item.Previous;
      var next = item.Next;

      if (prev != null)
      {
          prev.Next = next;
      }

      if (next != null)
      {
          next.Previous = prev;
      }

      if (this.TailLink == item)
      {
          this.TailLink = prev;
      }

      if (this.HeadLink != null)
      {
          this.HeadLink.Previous = item;
      }

      item.Next = this.HeadLink;
      item.Previous = null;
      this.HeadLink = item;
    }

    /// <summary>
    /// Prepend the specified value to the head of the list
    /// </summary>
    /// <param name="value">
    /// Value to prepend to the list (not a LinkItem)
    /// </param>
    /// <returns>
    /// The LinkItem which points to this new value
    /// </returns>
    public LinkItem Prepend(object value)
    {
      var newItem = new LinkItem(this.HeadLink, null, value);

      if (this.HeadLink != null)
      {
        this.HeadLink.Previous = newItem;
      }

      this.TailLink ??= newItem;

      this.HeadLink = newItem;

      // Increment the count
      Interlocked.Increment(ref this.m_count);

      return newItem;
    }

    /// <summary>
    /// Remove the specified link from the list
    /// </summary>
    /// <param name="item">
    /// Item to remove 
    /// </param>
    public void RemoveLink(LinkItem item)
    {
      // Check the arguments
      if (item == null)
      {
        return;
      }

      var next = item.Next;
      var prev = item.Previous;

      if (this.HeadLink == item)
      {
        this.HeadLink = next;
      }

      if (this.TailLink == item)
      {
        this.TailLink = prev;
      }

      if (prev != null)
      {
        prev.Next = next;
      }

      if (next != null)
      {
        next.Previous = prev;
      }

      // Decrement the count
      Interlocked.Decrement(ref this.m_count);
    }

    #endregion

    #region Implemented Interfaces

    #region ICollection

    /// <summary>
    /// The copy to.
    /// </summary>
    /// <param name="array">
    /// The array.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    public void CopyTo([NotNull] Array array, int index)
    {
      int i;
      LinkItem current;

      for (i = 0, current = this.HeadLink; current != null && i + index < array.Length; i++, current = current.Next)
      {
        array.SetValue(current.Item, index + i);
      }
    }

    #endregion

    #region IEnumerable

    /// <summary>
    /// The get enumerator.
    /// </summary>
    /// <returns>
    /// </returns>
    public IEnumerator GetEnumerator()
    {
      return new EnumLinkList(this);
    }

    #endregion

    #region IList

    /// <summary>
    /// The add.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <returns>
    /// The add.
    /// </returns>
    public int Add(object value)
    {
      // Append this item to the tail
      this.TailLink = new LinkItem(null, this.TailLink, value);

      if (this.TailLink.Previous != null)
      {
        this.TailLink.Previous.Next = this.TailLink;
      }

      this.HeadLink ??= this.TailLink;

      // Adjust the count
      Interlocked.Increment(ref this.m_count);

      return this.m_count - 1;
    }

    /// <summary>
    /// The clear.
    /// </summary>
    public void Clear()
    {
      this.HeadLink = null;
      this.TailLink = null;
    }

    /// <summary>
    /// The contains.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <returns>
    /// The contains.
    /// </returns>
    public bool Contains(object value)
    {
      LinkItem current;
      var hasItem = false;

      // Skip past existing items
      for (current = this.HeadLink; current != null && current.Item != value; current = current.Next)
      {
      }

      if (current != null)
      {
        hasItem = true;
      }

      return hasItem;
    }

    /// <summary>
    /// The index of.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <returns>
    /// The index of.
    /// </returns>
    public int IndexOf(object value)
    {
      LinkItem current;
      int index;

      // Skip past existing items
      for (index = 0, current = this.HeadLink;
           current != null && current.Item != value;
           index++, current = current.Next)
      {
      }

      if (current != null)
      {
        return index;
      }

      return -1;
    }

    /// <summary>
    /// The insert.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public void Insert(int index, object value)
    {
      int i;
      LinkItem current;

      // Skip past existing items
      for (i = 0, current = this.HeadLink; current != null && i < index; i++, current = current.Next)
      {
      }

      if (i == index && current != null)
      {
        // Create the next link item
        var newItem = new LinkItem(current, current.Previous, value);

        current.Previous.Next = newItem;
        current.Previous = newItem;

        if (this.HeadLink == current)
        {
          this.HeadLink = newItem;
        }

        // Adjust the count
        Interlocked.Increment(ref this.m_count);
      }
      else if (current == null)
      {
        this.Add(value);
      }
    }

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    public void Remove(object value)
    {
      LinkItem current;

      // Skip past existing items
      for (current = this.HeadLink; current != null && current.Item != value; current = current.Next)
      {
      }

      if (current == null)
      {
          return;
      }

      var prev = current.Previous;
      var next = current.Next;

      if (current == this.HeadLink)
      {
          this.HeadLink = next;
      }
      else if (prev != null)
      {
          prev.Next = next;
      }

      if (current == this.TailLink)
      {
          this.TailLink = prev;
      }
      else if (next != null)
      {
          next.Previous = prev;
      }

      // Adjust the count
      Interlocked.Decrement(ref this.m_count);
    }

    /// <summary>
    /// The remove at.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    public void RemoveAt(int index)
    {
      int i;
      LinkItem current;

      // Skip past existing items
      for (i = 0, current = this.HeadLink; current != null && i < index; i++, current = current.Next)
      {
      }

      if (i != index || current == null)
      {
          return;
      }

      var prev = current.Previous;
      var next = current.Next;

      if (current == this.HeadLink)
      {
          this.HeadLink = next;
      }
      else if (prev != null)
      {
          prev.Next = next;
      }

      if (this.TailLink == current)
      {
          this.TailLink = prev;
      }
      else if (next != null)
      {
          next.Previous = prev;
      }

      // Decrement the count
      Interlocked.Decrement(ref this.m_count);
    }

    #endregion

    #endregion

    /// <summary>
    /// Public class to enumerate the items in the list
    /// </summary>
    [Serializable]
    public class EnumLinkList : IEnumerator
    {
      #region Constants and Fields

      /// <summary>
      /// The m_list.
      /// </summary>
      private readonly DoubleLinkedList m_list;

      /// <summary>
      /// The m_current.
      /// </summary>
      private LinkItem m_current;

      #endregion

      #region Constructors and Destructors

      /// <summary>
      /// Initializes a new instance of the <see cref="EnumLinkList"/> class.
      /// </summary>
      /// <param name="list">
      /// The list.
      /// </param>
      public EnumLinkList(DoubleLinkedList list)
      {
        this.m_list = list;
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets Current.
      /// </summary>
      public object Current => this.m_current;

      #endregion

      #region Implemented Interfaces

      #region IEnumerator

      /// <summary>
      /// The move next.
      /// </summary>
      /// <returns>
      /// The move next.
      /// </returns>
      public bool MoveNext()
      {
        bool result;

        if (this.m_current == null)
        {
          // There are no items in the list
          if (this.m_list.HeadLink == null)
          {
            return false;
          }

          this.m_current = this.m_list.HeadLink;
          result = true;
        }
        else if (this.m_current.Next == null)
        {
          this.m_current = null;
          result = false;
        }
        else
        {
          this.m_current = this.m_current.Next;
          result = true;
        }

        return result;
      }

      /// <summary>
      /// The reset.
      /// </summary>
      public void Reset()
      {
        this.m_current = null;
      }

      #endregion

      #endregion
    }

    /// <summary>
    /// Class that represents an element in the doubly linked list
    /// </summary>
    [Serializable]
    public class LinkItem
    {
      #region Constants and Fields

      /// <summary>
      ///   Current item that this node points to (ie the value)
      /// </summary>
      public object Item;

      /// <summary>
      ///   Next item in the list
      /// </summary>
      public LinkItem Next;

      /// <summary>
      ///   Previous item in the list
      /// </summary>
      public LinkItem Previous;

      #endregion

      #region Constructors and Destructors

      /// <summary>
      /// Initializes a new instance of the <see cref="LinkItem"/> class. 
      /// Build a new LinkItem pointing to the next, previous and current value
      /// </summary>
      /// <param name="next">
      /// The next LinkItem in the list
      /// </param>
      /// <param name="previous">
      /// The previous LinkItem in the list
      /// </param>
      /// <param name="item">
      /// The current value that this item points to
      /// </param>
      public LinkItem(LinkItem next, LinkItem previous, object item)
      {
        this.Next = next;
        this.Previous = previous;
        this.Item = item;
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// The to string.
      /// </summary>
      /// <returns>
      /// The to string.
      /// </returns>
      public override string ToString()
      {
          return this.Item != null ? this.Item.ToString() : "null";
      }

      #endregion
    }
  }
}