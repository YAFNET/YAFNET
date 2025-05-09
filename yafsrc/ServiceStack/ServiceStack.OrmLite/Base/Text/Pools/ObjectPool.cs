﻿// ***********************************************************************
// <copyright file="ObjectPool.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Diagnostics;
using System.Threading;

namespace ServiceStack.OrmLite.Base.Text.Pools;

// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// define TRACE_LEAKS to get additional diagnostics that can lead to the leak sources. note: it will
// make everything about 2-3x slower
// 
// #define TRACE_LEAKS

// define DETECT_LEAKS to detect possible leaks
// #if DEBUG
// #define DETECT_LEAKS  //for now always enable DETECT_LEAKS in debug.
// #endif

#if DETECT_LEAKS
using System.Runtime.CompilerServices;
#endif
/// <summary>
/// Generic implementation of object pooling pattern with predefined pool size limit. The main
/// purpose is that limited number of frequently used objects can be kept in the pool for
/// further recycling.
/// Notes:
/// 1) it is not the goal to keep all returned objects. Pool is not meant for storage. If there
/// is no space in the pool, extra returned objects will be dropped.
/// 2) it is implied that if object was obtained from a pool, the caller will return it back in
/// a relatively short time. Keeping checked out objects for long durations is ok, but
/// reduces usefulness of pooling. Just new up your own.
/// Not returning objects to the pool in not detrimental to the pool's work, but is a bad practice.
/// Rationale:
/// If there is no intent for reusing the object, do not use pool - just use "new".
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectPool<T> where T : class
{
#if !PCL
    /// <summary>
    /// Struct Element
    /// </summary>
    [DebuggerDisplay("{Value,nq}")]
#endif
    private struct Element
    {
        /// <summary>
        /// The value
        /// </summary>
        internal T Value;
    }

    /// <summary>
    /// Delegate Factory
    /// </summary>
    /// <returns>T.</returns>
    /// <remarks>Not using System.Func{T} because this file is linked into the (debugger) Formatter,
    /// which does not have that type (since it compiles against .NET 2.0).</remarks>
    public delegate T Factory();

    // Storage for the pool objects. The first item is stored in a dedicated field because we
    // expect to be able to satisfy most requests from it.
    /// <summary>
    /// The first item
    /// </summary>
    private T _firstItem;
    /// <summary>
    /// The items
    /// </summary>
    private readonly Element[] _items;

    // factory is stored for the lifetime of the pool. We will call this only when pool needs to
    // expand. compared to "new T()", Func gives more flexibility to implementers and faster
    // than "new T()".
    /// <summary>
    /// The factory
    /// </summary>
    private readonly Factory _factory;

#if DETECT_LEAKS
        private static readonly ConditionalWeakTable<T, LeakTracker> leakTrackers = new ConditionalWeakTable<T, LeakTracker>();

        private class LeakTracker : IDisposable
        {
            private volatile bool disposed;

#if TRACE_LEAKS
            internal volatile object Trace = null;
#endif

            public void Dispose()
            {
                disposed = true;
                GC.SuppressFinalize(this);
            }

            private string GetTrace()
            {
#if TRACE_LEAKS
                return Trace == null ? "" : Trace.ToString();
#else
                return "Leak tracing information is disabled. Define TRACE_LEAKS on ObjectPool`1.cs to get more info \n";
#endif
            }

            ~LeakTracker()
            {
                if (!this.disposed && !Environment.HasShutdownStarted)
                {
                    var trace = GetTrace();

                    // If you are seeing this message it means that object has been allocated from the pool
                    // and has not been returned back. This is not critical, but turns pool into rather
                    // inefficient kind of "new".
                    Debug.WriteLine($"TRACEOBJECTPOOLLEAKS_BEGIN\nPool detected potential leaking of {typeof(T)}. \n Location of the leak: \n {GetTrace()} TRACEOBJECTPOOLLEAKS_END");
                }
            }
        }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectPool{T}" /> class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="size">The size.</param>
    public ObjectPool(Factory factory, int size)
    {
#if !PCL
        Debug.Assert(size >= 1);
#endif
        this._factory = factory;
        this._items = new Element[size - 1];
    }

    /// <summary>
    /// Creates the instance.
    /// </summary>
    /// <returns>T.</returns>
    private T CreateInstance()
    {
        var inst = this._factory();
        return inst;
    }

    /// <summary>
    /// Produces an instance.
    /// </summary>
    /// <returns>T.</returns>
    /// <remarks>Search strategy is a simple linear probing which is chosen for it cache-friendliness.
    /// Note that Free will try to store recycled objects close to the start thus statistically
    /// reducing how far we will typically search.</remarks>
    public T Allocate()
    {
        // PERF: Examine the first element. If that fails, AllocateSlow will look at the remaining elements.
        // Note that the initial read is optimistically not synchronized. That is intentional.
        // We will interlock only when we have a candidate. in a worst case we may miss some
        // recently returned objects. Not a big deal.
        var inst = this._firstItem;
        if (inst == null || inst != Interlocked.CompareExchange(ref this._firstItem, null, inst))
        {
            inst = this.AllocateSlow();
        }

#if DETECT_LEAKS
            var tracker = new LeakTracker();
            leakTrackers.Add(inst, tracker);

#if TRACE_LEAKS
            var frame = CaptureStackTrace();
            tracker.Trace = frame;
#endif
#endif
        return inst;
    }

    /// <summary>
    /// Allocates the slow.
    /// </summary>
    /// <returns>T.</returns>
    private T AllocateSlow()
    {
        var items = this._items;

        for (var i = 0; i < items.Length; i++)
        {
            // Note that the initial read is optimistically not synchronized. That is intentional.
            // We will interlock only when we have a candidate. in a worst case we may miss some
            // recently returned objects. Not a big deal.
            var inst = items[i].Value;
            if (inst != null)
            {
                if (inst == Interlocked.CompareExchange(ref items[i].Value, null, inst))
                {
                    return inst;
                }
            }
        }

        return this.CreateInstance();
    }

    /// <summary>
    /// Returns objects to the pool.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <remarks>Search strategy is a simple linear probing which is chosen for it cache-friendliness.
    /// Note that Free will try to store recycled objects close to the start thus statistically
    /// reducing how far we will typically search in Allocate.</remarks>
    public void Free(T obj)
    {
        this.Validate(obj);
        this.ForgetTrackedObject(obj);

        if (this._firstItem == null)
        {
            // Intentionally not using interlocked here.
            // In a worst case scenario two objects may be stored into same slot.
            // It is very unlikely to happen and will only mean that one of the objects will get collected.
            this._firstItem = obj;
        }
        else
        {
            this.FreeSlow(obj);
        }
    }

    /// <summary>
    /// Frees the slow.
    /// </summary>
    /// <param name="obj">The object.</param>
    private void FreeSlow(T obj)
    {
        var items = this._items;
        for (var i = 0; i < items.Length; i++)
        {
            if (items[i].Value == null)
            {
                // Intentionally not using interlocked here.
                // In a worst case scenario two objects may be stored into same slot.
                // It is very unlikely to happen and will only mean that one of the objects will get collected.
                items[i].Value = obj;
                break;
            }
        }
    }

    /// <summary>
    /// Removes an object from leak tracking.
    /// This is called when an object is returned to the pool.  It may also be explicitly
    /// called if an object allocated from the pool is intentionally not being returned
    /// to the pool.  This can be of use with pooled arrays if the consumer wants to
    /// return a larger array to the pool than was originally allocated.
    /// </summary>
    /// <param name="old">The old.</param>
    /// <param name="replacement">The replacement.</param>
    [Conditional("DEBUG")]
    public void ForgetTrackedObject(T old, T replacement = null)
    {
#if DETECT_LEAKS
            LeakTracker tracker;
            if (leakTrackers.TryGetValue(old, out tracker))
            {
                tracker.Dispose();
                leakTrackers.Remove(old);
            }
            else
            {
                var trace = CaptureStackTrace();
                Debug.WriteLine($"TRACEOBJECTPOOLLEAKS_BEGIN\nObject of type {typeof(T)} was freed, but was not from pool. \n Callstack: \n {trace} TRACEOBJECTPOOLLEAKS_END");
            }

            if (replacement != null)
            {
                tracker = new LeakTracker();
                leakTrackers.Add(replacement, tracker);
            }
#endif
    }

#if DETECT_LEAKS
        private static Lazy<Type> _stackTraceType = new Lazy<Type>(() => Type.GetType("System.Diagnostics.StackTrace"));

        private static object CaptureStackTrace()
        {
            return Activator.CreateInstance(_stackTraceType.Value);
        }
#endif

#if !PCL
    /// <summary>
    /// Validates the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    [Conditional("DEBUG")]
#endif
    private void Validate(object obj)
    {
#if !PCL
        Debug.Assert(obj != null, "freeing null?");

        Debug.Assert(this._firstItem != obj, "freeing twice?");
#endif

        var items = this._items;
        for (var i = 0; i < items.Length; i++)
        {
            var value = items[i].Value;
            if (value == null)
            {
                return;
            }

#if !PCL
            Debug.Assert(value != obj, "freeing twice?");
#endif
        }
    }
}