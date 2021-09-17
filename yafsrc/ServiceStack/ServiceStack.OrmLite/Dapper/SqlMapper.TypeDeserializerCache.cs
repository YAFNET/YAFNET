// ***********************************************************************
// <copyright file="SqlMapper.TypeDeserializerCache.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ServiceStack.OrmLite.Dapper
{
    /// <summary>
    /// Class SqlMapper.
    /// </summary>
    public static partial class SqlMapper
    {
        /// <summary>
        /// Class TypeDeserializerCache.
        /// </summary>
        private class TypeDeserializerCache
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TypeDeserializerCache"/> class.
            /// </summary>
            /// <param name="type">The type.</param>
            private TypeDeserializerCache(Type type)
            {
                this.type = type;
            }

            /// <summary>
            /// The by type
            /// </summary>
            private static readonly Hashtable byType = new();

            /// <summary>
            /// The type
            /// </summary>
            private readonly Type type;

            /// <summary>
            /// Purges the specified type.
            /// </summary>
            /// <param name="type">The type.</param>
            internal static void Purge(Type type)
            {
                lock (byType)
                {
                    byType.Remove(type);
                }
            }

            /// <summary>
            /// Purges this instance.
            /// </summary>
            internal static void Purge()
            {
                lock (byType)
                {
                    byType.Clear();
                }
            }

            /// <summary>
            /// Gets the reader.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <param name="reader">The reader.</param>
            /// <param name="startBound">The start bound.</param>
            /// <param name="length">The length.</param>
            /// <param name="returnNullIfFirstMissing">if set to <c>true</c> [return null if first missing].</param>
            /// <returns>Func&lt;IDataReader, System.Object&gt;.</returns>
            internal static Func<IDataReader, object> GetReader(Type type, IDataReader reader, int startBound, int length, bool returnNullIfFirstMissing)
            {
                var found = (TypeDeserializerCache)byType[type];
                if (found == null)
                {
                    lock (byType)
                    {
                        found = (TypeDeserializerCache)byType[type];
                        if (found == null)
                        {
                            byType[type] = found = new TypeDeserializerCache(type);
                        }
                    }
                }
                return found.GetReader(reader, startBound, length, returnNullIfFirstMissing);
            }

            /// <summary>
            /// The readers
            /// </summary>
            private readonly Dictionary<DeserializerKey, Func<IDataReader, object>> readers = new();

            /// <summary>
            /// Struct DeserializerKey
            /// </summary>
            private struct DeserializerKey : IEquatable<DeserializerKey>
            {
                /// <summary>
                /// The start bound
                /// </summary>
                private readonly int startBound, length;
                /// <summary>
                /// The return null if first missing
                /// </summary>
                private readonly bool returnNullIfFirstMissing;
                /// <summary>
                /// The reader
                /// </summary>
                private readonly IDataReader reader;
                /// <summary>
                /// The names
                /// </summary>
                private readonly string[] names;
                /// <summary>
                /// The types
                /// </summary>
                private readonly Type[] types;
                /// <summary>
                /// The hash code
                /// </summary>
                private readonly int hashCode;

                /// <summary>
                /// Initializes a new instance of the <see cref="DeserializerKey"/> struct.
                /// </summary>
                /// <param name="hashCode">The hash code.</param>
                /// <param name="startBound">The start bound.</param>
                /// <param name="length">The length.</param>
                /// <param name="returnNullIfFirstMissing">if set to <c>true</c> [return null if first missing].</param>
                /// <param name="reader">The reader.</param>
                /// <param name="copyDown">if set to <c>true</c> [copy down].</param>
                public DeserializerKey(int hashCode, int startBound, int length, bool returnNullIfFirstMissing, IDataReader reader, bool copyDown)
                {
                    this.hashCode = hashCode;
                    this.startBound = startBound;
                    this.length = length;
                    this.returnNullIfFirstMissing = returnNullIfFirstMissing;

                    if (copyDown)
                    {
                        this.reader = null;
                        names = new string[length];
                        types = new Type[length];
                        int index = startBound;
                        for (int i = 0; i < length; i++)
                        {
                            names[i] = reader.GetName(index);
                            types[i] = reader.GetFieldType(index++);
                        }
                    }
                    else
                    {
                        this.reader = reader;
                        names = null;
                        types = null;
                    }
                }

                /// <summary>
                /// Returns a hash code for this instance.
                /// </summary>
                /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
                public override int GetHashCode() => hashCode;

                /// <summary>
                /// Returns a <see cref="string" /> that represents this instance.
                /// </summary>
                /// <returns>A <see cref="string" /> that represents this instance.</returns>
                public override string ToString()
                { // only used in the debugger
                    if (names != null)
                    {
                        return string.Join(", ", names);
                    }
                    if (reader != null)
                    {
                        var sb = new StringBuilder();
                        int index = startBound;
                        for (int i = 0; i < length; i++)
                        {
                            if (i != 0) sb.Append(", ");
                            sb.Append(reader.GetName(index++));
                        }
                        return sb.ToString();
                    }
                    return base.ToString();
                }

                /// <summary>
                /// Determines whether the specified <see cref="object" /> is equal to this instance.
                /// </summary>
                /// <param name="obj">The object to compare with the current instance.</param>
                /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
                public override bool Equals(object obj) => obj is DeserializerKey key && Equals(key);

                /// <summary>
                /// Indicates whether the current object is equal to another object of the same type.
                /// </summary>
                /// <param name="other">An object to compare with this object.</param>
                /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
                public bool Equals(DeserializerKey other)
                {
                    if (hashCode != other.hashCode
                        || startBound != other.startBound
                        || length != other.length
                        || returnNullIfFirstMissing != other.returnNullIfFirstMissing)
                    {
                        return false; // clearly different
                    }
                    for (int i = 0; i < length; i++)
                    {
                        if ((names?[i] ?? reader?.GetName(startBound + i)) != (other.names?[i] ?? other.reader?.GetName(startBound + i))
                            ||
                            (types?[i] ?? reader?.GetFieldType(startBound + i)) != (other.types?[i] ?? other.reader?.GetFieldType(startBound + i))
                            )
                        {
                            return false; // different column name or type
                        }
                    }
                    return true;
                }
            }

            /// <summary>
            /// Gets the reader.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="startBound">The start bound.</param>
            /// <param name="length">The length.</param>
            /// <param name="returnNullIfFirstMissing">if set to <c>true</c> [return null if first missing].</param>
            /// <returns>Func&lt;IDataReader, System.Object&gt;.</returns>
            private Func<IDataReader, object> GetReader(IDataReader reader, int startBound, int length, bool returnNullIfFirstMissing)
            {
                if (length < 0) length = reader.FieldCount - startBound;
                int hash = GetColumnHash(reader, startBound, length);
                if (returnNullIfFirstMissing) hash *= -27;
                // get a cheap key first: false means don't copy the values down
                var key = new DeserializerKey(hash, startBound, length, returnNullIfFirstMissing, reader, false);
                Func<IDataReader, object> deser;
                lock (readers)
                {
                    if (readers.TryGetValue(key, out deser)) return deser;
                }
                deser = GetTypeDeserializerImpl(type, reader, startBound, length, returnNullIfFirstMissing);
                // get a more expensive key: true means copy the values down so it can be used as a key later
                key = new DeserializerKey(hash, startBound, length, returnNullIfFirstMissing, reader, true);
                lock (readers)
                {
                    return readers[key] = deser;
                }
            }
        }
    }
}
