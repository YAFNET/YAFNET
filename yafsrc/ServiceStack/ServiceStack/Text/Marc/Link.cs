// ***********************************************************************
// <copyright file="Link.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Threading;

//Not using it here, but @marcgravell's stuff is too good not to include
namespace ServiceStack.Text.Marc;

/// <summary>
/// Pretty Thread-Safe cache class from:
/// http://code.google.com/p/dapper-dot-net/source/browse/Dapper/SqlMapper.cs
/// This is a micro-cache; suitable when the number of terms is controllable (a few hundred, for example),
/// and strictly append-only; you cannot change existing values. All key matches are on **REFERENCE**
/// equality. The type is fully thread-safe.
/// </summary>
/// <typeparam name="TKey">The type of the t key.</typeparam>
/// <typeparam name="TValue">The type of the t value.</typeparam>
class Link<TKey, TValue> where TKey : class
{
    /// <summary>
    /// Tries the get.
    /// </summary>
    /// <param name="link">The link.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryGet(Link<TKey, TValue> link, TKey key, out TValue value)
    {
        while (link != null)
        {
            if ((object)key == (object)link.Key)
            {
                value = link.Value;
                return true;
            }
            link = link.Tail;
        }
        value = default(TValue);
        return false;
    }

    /// <summary>
    /// Tries the add.
    /// </summary>
    /// <param name="head">The head.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryAdd(ref Link<TKey, TValue> head, TKey key, ref TValue value)
    {
        bool tryAgain;
        do
        {
            var snapshot = Interlocked.CompareExchange(ref head, null, null);
            TValue found;
            if (TryGet(snapshot, key, out found))
            { // existing match; report the existing value instead
                value = found;
                return false;
            }
            var newNode = new Link<TKey, TValue>(key, value, snapshot);
            // did somebody move our cheese?
            tryAgain = Interlocked.CompareExchange(ref head, newNode, snapshot) != snapshot;
        } while (tryAgain);
        return true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Link{TKey, TValue}" /> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <param name="tail">The tail.</param>
    private Link(TKey key, TValue value, Link<TKey, TValue> tail)
    {
        Key = key;
        Value = value;
        Tail = tail;
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The key.</value>
    public TKey Key { get; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value.</value>
    public TValue Value { get; }

    /// <summary>
    /// Gets the tail.
    /// </summary>
    /// <value>The tail.</value>
    public Link<TKey, TValue> Tail { get; }
}