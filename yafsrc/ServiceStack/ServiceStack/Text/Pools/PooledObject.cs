// ***********************************************************************
// <copyright file="PooledObject.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Text.Pools
{
    // Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// this is RAII object to automatically release pooled object when its owning pool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct PooledObject<T> : IDisposable where T : class
    {
        /// <summary>
        /// The releaser
        /// </summary>
        private readonly Action<ObjectPool<T>, T> _releaser;
        /// <summary>
        /// The pool
        /// </summary>
        private readonly ObjectPool<T> _pool;
        /// <summary>
        /// The pooled object
        /// </summary>
        private T _pooledObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledObject{T}" /> struct.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="allocator">The allocator.</param>
        /// <param name="releaser">The releaser.</param>
        public PooledObject(ObjectPool<T> pool, Func<ObjectPool<T>, T> allocator, Action<ObjectPool<T>, T> releaser) : this()
        {
            _pool = pool;
            _pooledObject = allocator(pool);
            _releaser = releaser;
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <value>The object.</value>
        public T Object => _pooledObject;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_pooledObject != null)
            {
                _releaser(_pool, _pooledObject);
                _pooledObject = null;
            }
        }

        #region factory
        /// <summary>
        /// Creates the specified pool.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <returns>PooledObject&lt;StringBuilder&gt;.</returns>
        public static PooledObject<StringBuilder> Create(ObjectPool<StringBuilder> pool)
        {
            return new(pool, Allocator, Releaser);
        }

        /// <summary>
        /// Creates the specified pool.
        /// </summary>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <returns>PooledObject&lt;Stack&lt;TItem&gt;&gt;.</returns>
        public static PooledObject<Stack<TItem>> Create<TItem>(ObjectPool<Stack<TItem>> pool)
        {
            return new(pool, Allocator, Releaser);
        }

        /// <summary>
        /// Creates the specified pool.
        /// </summary>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <returns>PooledObject&lt;Queue&lt;TItem&gt;&gt;.</returns>
        public static PooledObject<Queue<TItem>> Create<TItem>(ObjectPool<Queue<TItem>> pool)
        {
            return new(pool, Allocator, Releaser);
        }

        /// <summary>
        /// Creates the specified pool.
        /// </summary>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <returns>PooledObject&lt;HashSet&lt;TItem&gt;&gt;.</returns>
        public static PooledObject<HashSet<TItem>> Create<TItem>(ObjectPool<HashSet<TItem>> pool)
        {
            return new(pool, Allocator, Releaser);
        }

        /// <summary>
        /// Creates the specified pool.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <returns>PooledObject&lt;Dictionary&lt;TKey, TValue&gt;&gt;.</returns>
        public static PooledObject<Dictionary<TKey, TValue>> Create<TKey, TValue>(ObjectPool<Dictionary<TKey, TValue>> pool)
        {
            return new(pool, Allocator, Releaser);
        }

        /// <summary>
        /// Creates the specified pool.
        /// </summary>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <returns>PooledObject&lt;List&lt;TItem&gt;&gt;.</returns>
        public static PooledObject<List<TItem>> Create<TItem>(ObjectPool<List<TItem>> pool)
        {
            return new(pool, Allocator, Releaser);
        }
        #endregion

        #region allocators and releasers
        /// <summary>
        /// Allocators the specified pool.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <returns>StringBuilder.</returns>
        private static StringBuilder Allocator(ObjectPool<StringBuilder> pool)
        {
            return pool.AllocateAndClear();
        }

        /// <summary>
        /// Releasers the specified pool.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="sb">The sb.</param>
        private static void Releaser(ObjectPool<StringBuilder> pool, StringBuilder sb)
        {
            pool.ClearAndFree(sb);
        }

        /// <summary>
        /// Allocators the specified pool.
        /// </summary>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <returns>Stack&lt;TItem&gt;.</returns>
        private static Stack<TItem> Allocator<TItem>(ObjectPool<Stack<TItem>> pool)
        {
            return pool.AllocateAndClear();
        }

        /// <summary>
        /// Releasers the specified pool.
        /// </summary>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <param name="obj">The object.</param>
        private static void Releaser<TItem>(ObjectPool<Stack<TItem>> pool, Stack<TItem> obj)
        {
            pool.ClearAndFree(obj);
        }

        /// <summary>
        /// Allocators the specified pool.
        /// </summary>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <returns>Queue&lt;TItem&gt;.</returns>
        private static Queue<TItem> Allocator<TItem>(ObjectPool<Queue<TItem>> pool)
        {
            return pool.AllocateAndClear();
        }

        /// <summary>
        /// Releasers the specified pool.
        /// </summary>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <param name="obj">The object.</param>
        private static void Releaser<TItem>(ObjectPool<Queue<TItem>> pool, Queue<TItem> obj)
        {
            pool.ClearAndFree(obj);
        }

        /// <summary>
        /// Allocators the specified pool.
        /// </summary>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <returns>HashSet&lt;TItem&gt;.</returns>
        private static HashSet<TItem> Allocator<TItem>(ObjectPool<HashSet<TItem>> pool)
        {
            return pool.AllocateAndClear();
        }

        /// <summary>
        /// Releasers the specified pool.
        /// </summary>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <param name="obj">The object.</param>
        private static void Releaser<TItem>(ObjectPool<HashSet<TItem>> pool, HashSet<TItem> obj)
        {
            pool.ClearAndFree(obj);
        }

        /// <summary>
        /// Allocators the specified pool.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <returns>Dictionary&lt;TKey, TValue&gt;.</returns>
        private static Dictionary<TKey, TValue> Allocator<TKey, TValue>(ObjectPool<Dictionary<TKey, TValue>> pool)
        {
            return pool.AllocateAndClear();
        }

        /// <summary>
        /// Releasers the specified pool.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <param name="obj">The object.</param>
        private static void Releaser<TKey, TValue>(ObjectPool<Dictionary<TKey, TValue>> pool, Dictionary<TKey, TValue> obj)
        {
            pool.ClearAndFree(obj);
        }

        /// <summary>
        /// Allocators the specified pool.
        /// </summary>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <returns>List&lt;TItem&gt;.</returns>
        private static List<TItem> Allocator<TItem>(ObjectPool<List<TItem>> pool)
        {
            return pool.AllocateAndClear();
        }

        /// <summary>
        /// Releasers the specified pool.
        /// </summary>
        /// <typeparam name="TItem">The type of the t item.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <param name="obj">The object.</param>
        private static void Releaser<TItem>(ObjectPool<List<TItem>> pool, List<TItem> obj)
        {
            pool.ClearAndFree(obj);
        }
        #endregion
    }
}