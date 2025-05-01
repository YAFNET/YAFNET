// ***********************************************************************
// <copyright file="OrmLiteContext.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
#if !NET9_0_OR_GREATER
using System.Runtime.Remoting.Messaging;
#endif
using System.Threading;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteContext.
/// </summary>
public class OrmLiteContext
{
    /// <summary>
    /// The instance
    /// </summary>
    public readonly static OrmLiteContext Instance = new();

    /// <summary>
    /// Tell ServiceStack to use ThreadStatic Items Collection for Context Scoped items.
    /// Warning: ThreadStatic Items aren't pinned to the same request in async services which callback on different threads.
    /// </summary>
    public static bool UseThreadStatic = false;

    /// <summary>
    /// The context items
    /// </summary>
    [ThreadStatic]
    public static IDictionary ContextItems;

#if NET9_0_OR_GREATER
    readonly AsyncLocal<IDictionary> localContextItems = new();
#endif

    /// <summary>
    /// Gets a list of items for this context.
    /// </summary>
    /// <value>The items.</value>
    public virtual IDictionary Items
    {
        get => this.GetItems() ?? this.CreateItems();
        set => this.CreateItems(value);
    }

    /// <summary>
    /// The key
    /// </summary>
    private const string _key = "__OrmLite.Items";

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <returns>IDictionary.</returns>
    private IDictionary GetItems()
    {
#if NET9_0_OR_GREATER
            return UseThreadStatic ? ContextItems : this.localContextItems.Value;

#else
        try
        {
            if (UseThreadStatic)
                return ContextItems;

            return CallContext.LogicalGetData(_key) as IDictionary;
        }
        catch (NotImplementedException)
        {
            // Fixed in Mono master: https://github.com/mono/mono/pull/817
            return CallContext.GetData(_key) as IDictionary;
        }
#endif
    }

    /// <summary>
    /// Creates the items.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns>IDictionary.</returns>
    private IDictionary CreateItems(IDictionary items = null)
    {
#if NET9_0_OR_GREATER
        if (UseThreadStatic)
            {
                ContextItems = items ??= new Dictionary<object, object>();
            }
            else
            {
                this.localContextItems.Value = items ??= new ConcurrentDictionary<object, object>();
            }

#else
        try
        {
            if (UseThreadStatic)
            {
                ContextItems = items ??= new Dictionary<object, object>();
            }
            else
            {
                CallContext.LogicalSetData(_key, items ??= new ConcurrentDictionary<object, object>());
            }
        }
        catch (NotImplementedException)
        {
            // Fixed in Mono master: https://github.com/mono/mono/pull/817
            CallContext.SetData(_key, items ??= new ConcurrentDictionary<object, object>());
        }
#endif
        return items;
    }

    /// <summary>
    /// Clears the items.
    /// </summary>
    public void ClearItems()
    {
        if (UseThreadStatic)
        {
            ContextItems = new Dictionary<object, object>();
        }
        else
        {
#if NET9_0_OR_GREATER
            this.localContextItems.Value = new ConcurrentDictionary<object, object>();
#else
            CallContext.FreeNamedDataSlot(_key);
#endif
        }
    }

    /// <summary>
    /// Gets the or create.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="createFn">The create function.</param>
    /// <returns>T.</returns>
    public T GetOrCreate<T>(Func<T> createFn)
    {
        if (this.Items.Contains(typeof(T).Name))
        {
            return (T)this.Items[typeof(T).Name];
        }

        return (T)(this.Items[typeof(T).Name] = createFn());
    }

    /// <summary>
    /// Sets the item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    static internal void SetItem<T>(string key, T value)
    {
        if (Equals(value, default(T)))
        {
            Instance.Items.Remove(key);
        }
        else
        {
            Instance.Items[key] = value;
        }
    }

    /// <summary>
    /// Creates the new state.
    /// </summary>
    /// <returns>OrmLiteState.</returns>
    public static OrmLiteState CreateNewState()
    {
        var state = new OrmLiteState();
        OrmLiteState = state;
        return state;
    }

    /// <summary>
    /// Gets the state of the or create.
    /// </summary>
    /// <returns>OrmLiteState.</returns>
    public static OrmLiteState GetOrCreateState()
    {
        return OrmLiteState
               ?? CreateNewState();
    }

    /// <summary>
    /// Gets or sets the state of the orm lite.
    /// </summary>
    /// <value>The state of the orm lite.</value>
    public static OrmLiteState OrmLiteState
    {
        get
        {
            if (Instance.Items.Contains("OrmLiteState"))
            {
                return Instance.Items["OrmLiteState"] as OrmLiteState;
            }

            return null;
        }

        set => SetItem("OrmLiteState", value);
    }

    // Only used when using OrmLite API's against a native IDbConnection (i.e. not from DbFactory)
    /// <summary>
    /// Gets or sets the ts transaction.
    /// </summary>
    /// <value>The ts transaction.</value>
    static internal IDbTransaction TSTransaction
    {
        get
        {
            var state = OrmLiteState;
            return state?.TSTransaction;
        }

        set => GetOrCreateState().TSTransaction = value;
    }
}

/// <summary>
/// Class OrmLiteState.
/// </summary>
public class OrmLiteState
{
    /// <summary>
    /// The counter
    /// </summary>
    private static long Counter;

    /// <summary>
    /// The identifier
    /// </summary>
    public long Id;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteState" /> class.
    /// </summary>
    public OrmLiteState()
    {
        this.Id = Interlocked.Increment(ref Counter);
    }

    /// <summary>
    /// The ts transaction
    /// </summary>
    public IDbTransaction TSTransaction;

    /// <summary>
    /// The results filter
    /// </summary>
    public IOrmLiteResultsFilter ResultsFilter;

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        return $"State Id: {this.Id}";
    }
}