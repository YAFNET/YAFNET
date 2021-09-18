// ***********************************************************************
// <copyright file="ObjectDictionary.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Text
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// UX friendly alternative alias of Dictionary&lt;string, object&gt;
    /// </summary>
    public class ObjectDictionary : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDictionary" /> class.
        /// </summary>
        public ObjectDictionary() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDictionary" /> class.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.Dictionary`2" /> can contain.</param>
        public ObjectDictionary(int capacity) : base(capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDictionary" /> class.
        /// </summary>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> for the type of the key.</param>
        public ObjectDictionary(IEqualityComparer<string> comparer) : base(comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDictionary" /> class.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.Dictionary`2" /> can contain.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> for the type of the key.</param>
        public ObjectDictionary(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDictionary" /> class.
        /// </summary>
        /// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the new <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        public ObjectDictionary(IDictionary<string, object> dictionary) : base(dictionary) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDictionary" /> class.
        /// </summary>
        /// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the new <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> for the type of the key.</param>
        public ObjectDictionary(IDictionary<string, object> dictionary, IEqualityComparer<string> comparer) : base(dictionary, comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDictionary" /> class.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object containing the information required to serialize the <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure containing the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        protected ObjectDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// UX friendly alternative alias of Dictionary&lt;string, string&gt;
    /// </summary>
    public class StringDictionary : Dictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringDictionary" /> class.
        /// </summary>
        public StringDictionary() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StringDictionary" /> class.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.Dictionary`2" /> can contain.</param>
        public StringDictionary(int capacity) : base(capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StringDictionary" /> class.
        /// </summary>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> for the type of the key.</param>
        public StringDictionary(IEqualityComparer<string> comparer) : base(comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StringDictionary" /> class.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.Dictionary`2" /> can contain.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> for the type of the key.</param>
        public StringDictionary(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StringDictionary" /> class.
        /// </summary>
        /// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the new <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        public StringDictionary(IDictionary<string, string> dictionary) : base(dictionary) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StringDictionary" /> class.
        /// </summary>
        /// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the new <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> for the type of the key.</param>
        public StringDictionary(IDictionary<string, string> dictionary, IEqualityComparer<string> comparer) : base(dictionary, comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StringDictionary" /> class.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object containing the information required to serialize the <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure containing the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        protected StringDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// UX friendly alternative alias of List&lt;KeyValuePair&lt;string, object&gt;gt;
    /// </summary>
    public class KeyValuePairs : List<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairs" /> class.
        /// </summary>
        public KeyValuePairs() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairs" /> class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public KeyValuePairs(int capacity) : base(capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairs" /> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public KeyValuePairs(IEnumerable<KeyValuePair<string, object>> collection) : base(collection) { }

        /// <summary>
        /// Creates the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>KeyValuePair&lt;System.String, System.Object&gt;.</returns>
        public static KeyValuePair<string, object> Create(string key, object value) =>
            new(key, value);
    }

    /// <summary>
    /// UX friendly alternative alias of List&lt;KeyValuePair&lt;string, string&gt;gt;
    /// </summary>
    public class KeyValueStrings : List<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueStrings" /> class.
        /// </summary>
        public KeyValueStrings() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueStrings" /> class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public KeyValueStrings(int capacity) : base(capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueStrings" /> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public KeyValueStrings(IEnumerable<KeyValuePair<string, string>> collection) : base(collection) { }

        /// <summary>
        /// Creates the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>KeyValuePair&lt;System.String, System.String&gt;.</returns>
        public static KeyValuePair<string, string> Create(string key, string value) =>
            new(key, value);
    }
}