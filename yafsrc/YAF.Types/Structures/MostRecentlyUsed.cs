/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Types.Structures;

using System.Collections;
using System.Collections.Specialized;
using System.Text;

/// <summary>
/// The most recently used.
/// </summary>
[Serializable]
public class MostRecentlyUsed : DictionaryBase
{
    /// <summary>
    /// The link to key.
    /// </summary>
    private readonly HybridDictionary linkToKey = new();

    /// <summary>
    /// The list.
    /// </summary>
    private readonly DoubleLinkedList list = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MostRecentlyUsed"/> class.
    ///   Default constructor for the most recently used items using the default size (50)
    /// </summary>
    public MostRecentlyUsed()
    {
        this.Capacity = 50;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MostRecentlyUsed"/> class.
    /// Construct a most recently used items list with the maximum number of items
    ///   allowed in the list.
    /// </summary>
    /// <param name="maxItems">
    /// Maximum number of items allowed
    /// </param>
    public MostRecentlyUsed(uint maxItems)
    {
        this.Capacity = maxItems;
    }

    /// <summary>
    /// The purged from cache delegate.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public delegate void PurgedFromCacheDelegate(object key, object value);

    /// <summary>
    ///   Event that is fired when an item falls outside of the cache
    /// </summary>
    public event PurgedFromCacheDelegate OnPurgedFromCache;

    /// <summary>
    ///   Gets or sets the maximum capacity of the list
    /// </summary>
    public uint Capacity { get; set; }

    /// <summary>
    /// Gets Keys.
    /// </summary>
    public ICollection Keys => this.Dictionary.Keys;

    /// <summary>
    /// Gets Values.
    /// </summary>
    public ICollection Values => this.Dictionary.Values;

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    public object this[object key]
    {
        get
        {
            var item = (DoubleLinkedList.LinkItem)this.Dictionary[key];

            if (item is null)
            {
                return null;
            }

            this.list.MoveToHead(item);

            return item.Item;
        }

        set
        {
            if (this.Dictionary.Contains(key))
            {
                var link = (DoubleLinkedList.LinkItem)this.Dictionary[key];
                link.Item = value;

                this.list.MoveToHead(link);

                this.Dictionary[key] = link;

                // Keep a reverse index from the link to the key
                this.linkToKey[link] = key;
            }
            else
            {
                this.Add(key, value);
            }
        }
    }

    /// <summary>
    /// The add.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public void Add(object key, object value)
    {
        var link = this.list.Prepend(value);

        this.Dictionary.Add(key, link);

        // Keep a reverse index from the link to the key
        this.linkToKey[link] = key;
    }

    /// <summary>
    /// The contains.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public bool Contains(object key)
    {
        var hasKey = this.Dictionary.Contains(key);

        // Update the reference for this link
        if (hasKey)
        {
            this.list.MoveToHead((DoubleLinkedList.LinkItem)this.Dictionary[key]);
        }

        return hasKey;
    }

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    public void Remove(object key)
    {
        var link = (DoubleLinkedList.LinkItem)this.Dictionary[key];

        this.Dictionary.Remove(key);

        if (link is null)
        {
            return;
        }

        this.list.RemoveLink(link);

        // Keep a reverse index from the link to the key
        this.linkToKey.Remove(link);
    }

    /// <summary>
    /// The to string.
    /// </summary>
    /// <returns>
    /// The to string.
    /// </returns>
    public override string ToString()
    {
        var buff = new StringBuilder(Convert.ToInt32(this.Capacity));

        buff.Append("[");

        foreach (var item in this.list)
        {
            if (buff.Length > 1)
            {
                buff.Append(", ");
            }

            buff.Append(item);
        }

        buff.Append(']');

        return buff.ToString();
    }

    /// <summary>
    /// The on insert.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    protected override void OnInsert(object key, object value)
    {
        if (this.Dictionary.Keys.Count < this.Capacity)
        {
            return;
        }

        // Purge an item from the cache
        var tail = this.list.TailLink;

        if (tail is null)
        {
            return;
        }

        var purgeKey = this.linkToKey[tail];

        if (purgeKey is null)
        {
            return;
        }

        // Fire the event
        if (this.OnPurgedFromCache is not null && this.OnPurgedFromCache.GetInvocationList().Length > 0)
        {
            this.OnPurgedFromCache(purgeKey, tail.Item);
        }

        this.Remove(purgeKey);
    }
}