// ***********************************************************************
// <copyright file="SqlMapper.CacheInfo.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;
using System.Threading;

namespace ServiceStack.OrmLite.Dapper
{
    /// <summary>
    /// Class SqlMapper.
    /// </summary>
    public static partial class SqlMapper
    {
        /// <summary>
        /// Class CacheInfo.
        /// </summary>
        private class CacheInfo
        {
            /// <summary>
            /// Gets or sets the deserializer.
            /// </summary>
            /// <value>The deserializer.</value>
            public DeserializerState Deserializer { get; set; }
            /// <summary>
            /// Gets or sets the other deserializers.
            /// </summary>
            /// <value>The other deserializers.</value>
            public Func<IDataReader, object>[] OtherDeserializers { get; set; }
            /// <summary>
            /// Gets or sets the parameter reader.
            /// </summary>
            /// <value>The parameter reader.</value>
            public Action<IDbCommand, object> ParamReader { get; set; }
            /// <summary>
            /// The hit count
            /// </summary>
            private int hitCount;
            /// <summary>
            /// Gets the hit count.
            /// </summary>
            /// <returns>System.Int32.</returns>
            public int GetHitCount() { return Interlocked.CompareExchange(ref hitCount, 0, 0); }
            /// <summary>
            /// Records the hit.
            /// </summary>
            public void RecordHit() { Interlocked.Increment(ref hitCount); }
        }
    }
}
