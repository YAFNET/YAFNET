// ***********************************************************************
// <copyright file="BufferPool.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.Text.Pools
{
    /// <summary>
    /// Courtesy of @marcgravell
    /// https://github.com/mgravell/protobuf-net/blob/master/src/protobuf-net/BufferPool.cs
    /// </summary>
    public sealed class BufferPool
    {
        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public static void Flush()
        {
            lock (Pool)
            {
                for (var i = 0; i < Pool.Length; i++)
                    Pool[i] = null;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="BufferPool" /> class from being created.
        /// </summary>
        private BufferPool() { }
        /// <summary>
        /// The pool size
        /// </summary>
        private const int POOL_SIZE = 20;
        /// <summary>
        /// The buffer length
        /// </summary>
        public const int BUFFER_LENGTH = 1450; //<= MTU - DJB
        /// <summary>
        /// The pool
        /// </summary>
        private static readonly CachedBuffer[] Pool = new CachedBuffer[POOL_SIZE];

        /// <summary>
        /// Gets the buffer.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBuffer()
        {
            return GetBuffer(BUFFER_LENGTH);
        }

        /// <summary>
        /// Gets the buffer.
        /// </summary>
        /// <param name="minSize">The minimum size.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBuffer(int minSize)
        {
            byte[] cachedBuff = GetCachedBuffer(minSize);
            return cachedBuff ?? new byte[minSize];
        }

        /// <summary>
        /// Gets the cached buffer.
        /// </summary>
        /// <param name="minSize">The minimum size.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetCachedBuffer(int minSize)
        {
            lock (Pool)
            {
                var bestIndex = -1;
                byte[] bestMatch = null;
                for (var i = 0; i < Pool.Length; i++)
                {
                    var buffer = Pool[i];
                    if (buffer == null || buffer.Size < minSize)
                    {
                        continue;
                    }
                    if (bestMatch != null && bestMatch.Length < buffer.Size)
                    {
                        continue;
                    }

                    var tmp = buffer.Buffer;
                    if (tmp == null)
                    {
                        Pool[i] = null;
                    }
                    else
                    {
                        bestMatch = tmp;
                        bestIndex = i;
                    }
                }

                if (bestIndex >= 0)
                {
                    Pool[bestIndex] = null;
                }

                return bestMatch;
            }
        }

        /// <summary>
        /// The maximum byte array size
        /// </summary>
        private const int MaxByteArraySize = int.MaxValue - 56;

        /// <summary>
        /// Resizes the and flush left.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="toFitAtLeastBytes">To fit at least bytes.</param>
        /// <param name="copyFromIndex">Index of the copy from.</param>
        /// <param name="copyBytes">The copy bytes.</param>
        public static void ResizeAndFlushLeft(ref byte[] buffer, int toFitAtLeastBytes, int copyFromIndex, int copyBytes)
        {
            Helpers.DebugAssert(buffer != null);
            Helpers.DebugAssert(toFitAtLeastBytes > buffer.Length);
            Helpers.DebugAssert(copyFromIndex >= 0);
            Helpers.DebugAssert(copyBytes >= 0);

            int newLength = buffer.Length * 2;
            if (newLength < 0)
            {
                newLength = MaxByteArraySize;
            }

            if (newLength < toFitAtLeastBytes) newLength = toFitAtLeastBytes;

            if (copyBytes == 0)
            {
                ReleaseBufferToPool(ref buffer);
            }

            var newBuffer = GetCachedBuffer(toFitAtLeastBytes) ?? new byte[newLength];

            if (copyBytes > 0)
            {
                Buffer.BlockCopy(buffer, copyFromIndex, newBuffer, 0, copyBytes);
                ReleaseBufferToPool(ref buffer);
            }

            buffer = newBuffer;
        }

        /// <summary>
        /// Releases the buffer to pool.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public static void ReleaseBufferToPool(ref byte[] buffer)
        {
            if (buffer == null) return;

            lock (Pool)
            {
                var minIndex = 0;
                var minSize = int.MaxValue;
                for (var i = 0; i < Pool.Length; i++)
                {
                    var tmp = Pool[i];
                    if (tmp == null || !tmp.IsAlive)
                    {
                        minIndex = 0;
                        break;
                    }
                    if (tmp.Size < minSize)
                    {
                        minIndex = i;
                        minSize = tmp.Size;
                    }
                }

                Pool[minIndex] = new CachedBuffer(buffer);
            }

            buffer = null;
        }

        /// <summary>
        /// Class CachedBuffer.
        /// </summary>
        private class CachedBuffer
        {
            /// <summary>
            /// The reference
            /// </summary>
            private readonly WeakReference _reference;

            /// <summary>
            /// Gets the size.
            /// </summary>
            /// <value>The size.</value>
            public int Size { get; }

            /// <summary>
            /// Gets a value indicating whether this instance is alive.
            /// </summary>
            /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
            public bool IsAlive => _reference.IsAlive;
            /// <summary>
            /// Gets the buffer.
            /// </summary>
            /// <value>The buffer.</value>
            public byte[] Buffer => (byte[])_reference.Target;

            /// <summary>
            /// Initializes a new instance of the <see cref="CachedBuffer" /> class.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            public CachedBuffer(byte[] buffer)
            {
                Size = buffer.Length;
                _reference = new WeakReference(buffer);
            }
        }
    }

    /// <summary>
    /// Class Helpers. This class cannot be inherited.
    /// </summary>
    internal sealed class Helpers
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="Helpers" /> class from being created.
        /// </summary>
        private Helpers() { }

        /// <summary>
        /// Debugs the assert.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        [System.Diagnostics.Conditional("DEBUG")]
        internal static void DebugAssert(bool condition)
        {
#if DEBUG   
            if (!condition && System.Diagnostics.Debugger.IsAttached) 
                System.Diagnostics.Debugger.Break();
            System.Diagnostics.Debug.Assert(condition);
#endif
        }

    }
}