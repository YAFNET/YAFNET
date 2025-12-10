// ***********************************************************************
// <copyright file="RecyclableMemoryStream.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ServiceStack.OrmLite.Base.Text;
//Internalize to avoid conflicts



/// <summary>
/// Class MemoryStreamFactory.
/// </summary>
public static class MemoryStreamFactory
{
    /// <summary>
    /// Gets or sets the use recyclable memory stream.
    /// </summary>
    /// <value>The use recyclable memory stream.</value>
    public static bool UseRecyclableMemoryStream { get; set; }

    /// <summary>
    /// The recyclable instance
    /// </summary>
    public static RecyclableMemoryStreamManager RecyclableInstance { get; } = new();

    /// <summary>
    /// Gets the stream.
    /// </summary>
    /// <returns>System.IO.MemoryStream.</returns>
    public static MemoryStream GetStream()
    {
        return UseRecyclableMemoryStream
                   ? RecyclableInstance.GetStream()
                   : new MemoryStream();
    }

    /// <summary>
    /// Gets the stream.
    /// </summary>
    /// <param name="capacity">The capacity.</param>
    /// <returns>System.IO.MemoryStream.</returns>
    public static MemoryStream GetStream(int capacity)
    {
        return UseRecyclableMemoryStream
                   ? RecyclableInstance.GetStream(nameof(MemoryStreamFactory), capacity)
                   : new MemoryStream(capacity);
    }

    /// <summary>
    /// Gets the stream.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns>System.IO.MemoryStream.</returns>
    public static MemoryStream GetStream(byte[] bytes)
    {
        return UseRecyclableMemoryStream
                   ? RecyclableInstance.GetStream(nameof(MemoryStreamFactory), bytes, 0, bytes.Length)
                   : new MemoryStream(bytes, 0, bytes.Length, true, true);
    }

    /// <summary>
    /// Gets the stream.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <param name="index">The index.</param>
    /// <param name="count">The count.</param>
    /// <returns>System.IO.MemoryStream.</returns>
    public static MemoryStream GetStream(byte[] bytes, int index, int count)
    {
        return UseRecyclableMemoryStream
                   ? RecyclableInstance.GetStream(nameof(MemoryStreamFactory), bytes, index, count)
                   : new MemoryStream(bytes, index, count, true, true);
    }
}

#if !NET10_0_OR_GREATER
/// <summary>
/// Enum EventLevel
/// </summary>
public enum EventLevel
{
    /// <summary>
    /// The log always
    /// </summary>
    LogAlways = 0,
    /// <summary>
    /// The critical
    /// </summary>
    Critical,
    /// <summary>
    /// The error
    /// </summary>
    Error,
    /// <summary>
    /// The warning
    /// </summary>
    Warning,
    /// <summary>
    /// The informational
    /// </summary>
    Informational,
    /// <summary>
    /// The verbose
    /// </summary>
    Verbose,
}

/// <summary>
/// Enum EventKeywords
/// </summary>
public enum EventKeywords : long
{
    /// <summary>
    /// The none
    /// </summary>
    None = 0x0,
}

/// <summary>
/// Class EventSourceAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Class)]
public sealed class EventSourceAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    /// <value>The unique identifier.</value>
    public string Guid { get; set; }
}

/// <summary>
/// Class EventAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Method)]
public sealed class EventAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventAttribute"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public EventAttribute(int id) { }

    /// <summary>
    /// Gets or sets the level.
    /// </summary>
    /// <value>The level.</value>
    public EventLevel Level { get; set; }
}

/// <summary>
/// Class EventSource.
/// </summary>
public class EventSource
{
    /// <summary>
    /// Writes the event.
    /// </summary>
    /// <param name="unused">The unused.</param>
    public void WriteEvent(params object[] unused)
    {
        return;
    }

    /// <summary>
    /// Determines whether this instance is enabled.
    /// </summary>
    /// <returns>bool.</returns>
    public bool IsEnabled()
    {
        return false;
    }

    /// <summary>
    /// Determines whether the specified level is enabled.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="keywords">The keywords.</param>
    /// <returns>bool.</returns>
    public bool IsEnabled(EventLevel level, EventKeywords keywords)
    {
        return false;
    }
}
#endif

/// <summary>
/// Class RecyclableMemoryStreamManager. This class cannot be inherited.
/// </summary>
public sealed partial class RecyclableMemoryStreamManager
{
    /// <summary>
    /// ETW events for RecyclableMemoryStream
    /// </summary>
    [EventSource(Name = "Microsoft-IO-RecyclableMemoryStream", Guid = "{B80CD4E4-890E-468D-9CBA-90EB7C82DFC7}")]
    public sealed class Events : EventSource
    {
        /// <summary>
        /// Static log object, through which all events are written.
        /// </summary>
        public static Events Writer = new();

        /// <summary>
        /// Type of buffer
        /// </summary>
        public enum MemoryStreamBufferType
        {
            /// <summary>
            /// Small block buffer
            /// </summary>
            Small,
            /// <summary>
            /// Large pool buffer
            /// </summary>
            Large
        }

        /// <summary>
        /// The possible reasons for discarding a buffer
        /// </summary>
        public enum MemoryStreamDiscardReason
        {
            /// <summary>
            /// Buffer was too large to be re-pooled
            /// </summary>
            TooLarge,
            /// <summary>
            /// There are enough free bytes in the pool
            /// </summary>
            EnoughFree
        }

        /// <summary>
        /// Logged when a stream object is created.
        /// </summary>
        /// <param name="guid">A unique ID for this stream.</param>
        /// <param name="tag">A temporary ID for this stream, usually indicates current usage.</param>
        /// <param name="requestedSize">Requested size of the stream</param>
        [Event(1, Level = EventLevel.Verbose)]
        public void MemoryStreamCreated(Guid guid, string tag, int requestedSize)
        {
            if (this.IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                this.WriteEvent(1, guid, tag ?? string.Empty, requestedSize);
            }
        }

        /// <summary>
        /// Logged when the stream is disposed
        /// </summary>
        /// <param name="guid">A unique ID for this stream.</param>
        /// <param name="tag">A temporary ID for this stream, usually indicates current usage.</param>
        [Event(2, Level = EventLevel.Verbose)]
        public void MemoryStreamDisposed(Guid guid, string tag)
        {
            if (this.IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                this.WriteEvent(2, guid, tag ?? string.Empty);
            }
        }

        /// <summary>
        /// Logged when the stream is disposed for the second time.
        /// </summary>
        /// <param name="guid">A unique ID for this stream.</param>
        /// <param name="tag">A temporary ID for this stream, usually indicates current usage.</param>
        /// <param name="allocationStack">Call stack of initial allocation.</param>
        /// <param name="disposeStack1">Call stack of the first dispose.</param>
        /// <param name="disposeStack2">Call stack of the second dispose.</param>
        /// <remarks>Note: Stacks will only be populated if RecyclableMemoryStreamManager.GenerateCallStacks is true.</remarks>
        [Event(3, Level = EventLevel.Critical)]
        public void MemoryStreamDoubleDispose(Guid guid, string tag, string allocationStack, string disposeStack1,
                                              string disposeStack2)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(3, guid, tag ?? string.Empty, allocationStack ?? string.Empty,
                    disposeStack1 ?? string.Empty, disposeStack2 ?? string.Empty);
            }
        }

        /// <summary>
        /// Logged when a stream is finalized.
        /// </summary>
        /// <param name="guid">A unique ID for this stream.</param>
        /// <param name="tag">A temporary ID for this stream, usually indicates current usage.</param>
        /// <param name="allocationStack">Call stack of initial allocation.</param>
        /// <remarks>Note: Stacks will only be populated if RecyclableMemoryStreamManager.GenerateCallStacks is true.</remarks>
        [Event(4, Level = EventLevel.Error)]
        public void MemoryStreamFinalized(Guid guid, string tag, string allocationStack)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(4, guid, tag ?? string.Empty, allocationStack ?? string.Empty);
            }
        }

        /// <summary>
        /// Logged when the RecyclableMemoryStreamManager is initialized.
        /// </summary>
        /// <param name="blockSize">Size of blocks, in bytes.</param>
        /// <param name="largeBufferMultiple">Size of the large buffer multiple, in bytes.</param>
        /// <param name="maximumBufferSize">Maximum buffer size, in bytes.</param>
        [Event(6, Level = EventLevel.Informational)]
        public void MemoryStreamManagerInitialized(int blockSize, int largeBufferMultiple, int maximumBufferSize)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(6, blockSize, largeBufferMultiple, maximumBufferSize);
            }
        }

        /// <summary>
        /// Logged when a new block is created.
        /// </summary>
        /// <param name="smallPoolInUseBytes">Number of bytes in the small pool currently in use.</param>
        [Event(7, Level = EventLevel.Verbose)]
        public void MemoryStreamNewBlockCreated(long smallPoolInUseBytes)
        {
            if (this.IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                this.WriteEvent(7, smallPoolInUseBytes);
            }
        }

        /// <summary>
        /// Logged when a new large buffer is created.
        /// </summary>
        /// <param name="requiredSize">Requested size</param>
        /// <param name="largePoolInUseBytes">Number of bytes in the large pool in use.</param>
        [Event(8, Level = EventLevel.Verbose)]
        public void MemoryStreamNewLargeBufferCreated(int requiredSize, long largePoolInUseBytes)
        {
            if (this.IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                this.WriteEvent(8, requiredSize, largePoolInUseBytes);
            }
        }

        /// <summary>
        /// Logged when a buffer is created that is too large to pool.
        /// </summary>
        /// <param name="requiredSize">Size requested by the caller</param>
        /// <param name="tag">A temporary ID for this stream, usually indicates current usage.</param>
        /// <param name="allocationStack">Call stack of the requested stream.</param>
        /// <remarks>Note: Stacks will only be populated if RecyclableMemoryStreamManager.GenerateCallStacks is true.</remarks>
        [Event(9, Level = EventLevel.Verbose)]
        public void MemoryStreamNonPooledLargeBufferCreated(int requiredSize, string tag, string allocationStack)
        {
            if (this.IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                this.WriteEvent(9, requiredSize, tag ?? string.Empty, allocationStack ?? string.Empty);
            }
        }

        /// <summary>
        /// Logged when a buffer is discarded (not put back in the pool, but given to GC to clean up).
        /// </summary>
        /// <param name="bufferType">Type of the buffer being discarded.</param>
        /// <param name="tag">A temporary ID for this stream, usually indicates current usage.</param>
        /// <param name="reason">Reason for the discard.</param>
        [Event(10, Level = EventLevel.Warning)]
        public void MemoryStreamDiscardBuffer(MemoryStreamBufferType bufferType, string tag,
                                              MemoryStreamDiscardReason reason)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(10, bufferType, tag ?? string.Empty, reason);
            }
        }

        /// <summary>
        /// Logged when a stream grows beyond the maximum capacity.
        /// </summary>
        /// <param name="requestedCapacity">The requested capacity.</param>
        /// <param name="maxCapacity">Maximum capacity, as configured by RecyclableMemoryStreamManager.</param>
        /// <param name="tag">A temporary ID for this stream, usually indicates current usage.</param>
        /// <param name="allocationStack">Call stack for the capacity request.</param>
        /// <remarks>Note: Stacks will only be populated if RecyclableMemoryStreamManager.GenerateCallStacks is true.</remarks>
        [Event(11, Level = EventLevel.Error)]
        public void MemoryStreamOverCapacity(int requestedCapacity, long maxCapacity, string tag,
                                             string allocationStack)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(11, requestedCapacity, maxCapacity, tag ?? string.Empty, allocationStack ?? string.Empty);
            }
        }
    }
}

/// <summary>
/// Manages pools of RecyclableMemoryStream objects.
/// </summary>
/// <remarks>There are two pools managed in here. The small pool contains same-sized buffers that are handed to streams
/// as they write more data.
/// For scenarios that need to call GetBuffer(), the large pool contains buffers of various sizes, all
/// multiples/exponentials of LargeBufferMultiple (1 MB by default). They are split by size to avoid overly-wasteful buffer
/// usage. There should be far fewer 8 MB buffers than 1 MB buffers, for example.</remarks>
public partial class RecyclableMemoryStreamManager
{
    /// <summary>
    /// Generic delegate for handling events without any arguments.
    /// </summary>
    public delegate void EventHandler();

    /// <summary>
    /// Delegate for handling large buffer discard reports.
    /// </summary>
    /// <param name="reason">Reason the buffer was discarded.</param>
    public delegate void LargeBufferDiscardedEventHandler(Events.MemoryStreamDiscardReason reason);

    /// <summary>
    /// Delegate for handling reports of stream size when streams are allocated
    /// </summary>
    /// <param name="bytes">Bytes allocated.</param>
    public delegate void StreamLengthReportHandler(long bytes);

    /// <summary>
    /// Delegate for handling periodic reporting of memory use statistics.
    /// </summary>
    /// <param name="smallPoolInUseBytes">Bytes currently in use in the small pool.</param>
    /// <param name="smallPoolFreeBytes">Bytes currently free in the small pool.</param>
    /// <param name="largePoolInUseBytes">Bytes currently in use in the large pool.</param>
    /// <param name="largePoolFreeBytes">Bytes currently free in the large pool.</param>
    public delegate void UsageReportEventHandler(
        long smallPoolInUseBytes, long smallPoolFreeBytes, long largePoolInUseBytes, long largePoolFreeBytes);

    /// <summary>
    /// Default block size, in bytes
    /// </summary>
    public const int DefaultBlockSize = 128 * 1024;
    /// <summary>
    /// Default large buffer multiple, in bytes
    /// </summary>
    public const int DefaultLargeBufferMultiple = 1024 * 1024;
    /// <summary>
    /// Default maximum buffer size, in bytes
    /// </summary>
    public const int DefaultMaximumBufferSize = 128 * 1024 * 1024;

    /// <summary>
    /// The large buffer free size
    /// </summary>
    private readonly long[] largeBufferFreeSize;
    /// <summary>
    /// The large buffer in use size
    /// </summary>
    private readonly long[] largeBufferInUseSize;


    /// <summary>
    /// The large pools
    /// </summary>
    private readonly ConcurrentStack<byte[]>[] largePools;

    /// <summary>
    /// The small pool
    /// </summary>
    private readonly ConcurrentStack<byte[]> smallPool;

    /// <summary>
    /// The small pool free size
    /// </summary>
    private long smallPoolFreeSize;
    /// <summary>
    /// The small pool in use size
    /// </summary>
    private long smallPoolInUseSize;

    /// <summary>
    /// Initializes the memory manager with the default block/buffer specifications.
    /// </summary>
    public RecyclableMemoryStreamManager()
        : this(DefaultBlockSize, DefaultLargeBufferMultiple, DefaultMaximumBufferSize, false) { }

    /// <summary>
    /// Initializes the memory manager with the given block requiredSize.
    /// </summary>
    /// <param name="blockSize">Size of each block that is pooled. Must be &gt; 0.</param>
    /// <param name="largeBufferMultiple">Each large buffer will be a multiple/exponential of this value.</param>
    /// <param name="maximumBufferSize">Buffers larger than this are not pooled</param>
    /// <param name="useExponentialLargeBuffer">Switch to exponential large buffer allocation strategy</param>
    /// <exception cref="ArgumentOutOfRangeException">nameof(blockSize), blockSize, blockSize must be a positive number</exception>
    /// <exception cref="ArgumentOutOfRangeException">nameof(blockSize), blockSize, blockSize must be a positive number</exception>
    /// <exception cref="ArgumentOutOfRangeException">nameof(blockSize), blockSize, blockSize must be a positive number</exception>
    /// <exception cref="ArgumentOutOfRangeException">nameof(blockSize), blockSize, blockSize must be a positive number</exception>
    /// <exception cref="ArgumentException">blockSize is not a positive number, or largeBufferMultiple is not a positive number, or maximumBufferSize is less than blockSize.</exception>
    public RecyclableMemoryStreamManager(int blockSize, int largeBufferMultiple, int maximumBufferSize, bool useExponentialLargeBuffer)
    {
        if (blockSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockSize), blockSize, "blockSize must be a positive number");
        }

        if (largeBufferMultiple <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(largeBufferMultiple),
                "largeBufferMultiple must be a positive number");
        }

        if (maximumBufferSize < blockSize)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumBufferSize),
                "maximumBufferSize must be at least blockSize");
        }

        this.BlockSize = blockSize;
        this.LargeBufferMultiple = largeBufferMultiple;
        this.MaximumBufferSize = maximumBufferSize;
        this.UseExponentialLargeBuffer = useExponentialLargeBuffer;

        if (!this.IsLargeBufferSize(maximumBufferSize))
        {
            throw new ArgumentException(
                $"maximumBufferSize is not {(this.UseExponentialLargeBuffer ? "an exponential" : "a multiple")} of largeBufferMultiple",
                nameof(maximumBufferSize));
        }

        this.smallPool = new ConcurrentStack<byte[]>();
        var numLargePools = useExponentialLargeBuffer
                                ? (int)Math.Log((double)maximumBufferSize / largeBufferMultiple, 2) + 1
                                : maximumBufferSize / largeBufferMultiple;

        // +1 to store size of bytes in use that are too large to be pooled
        this.largeBufferInUseSize = new long[numLargePools + 1];
        this.largeBufferFreeSize = new long[numLargePools];

        this.largePools = new ConcurrentStack<byte[]>[numLargePools];

        for (var i = 0; i < this.largePools.Length; ++i)
        {
            this.largePools[i] = new ConcurrentStack<byte[]>();
        }

        Events.Writer.MemoryStreamManagerInitialized(blockSize, largeBufferMultiple, maximumBufferSize);
    }

    /// <summary>
    /// The size of each block. It must be set at creation and cannot be changed.
    /// </summary>
    /// <value>The size of the block.</value>
    public int BlockSize { get; }

    /// <summary>
    /// All buffers are multiples/exponentials of this number. It must be set at creation and cannot be changed.
    /// </summary>
    /// <value>The large buffer multiple.</value>
    public int LargeBufferMultiple { get; }

    /// <summary>
    /// Use exponential large buffer allocation strategy. It must be set at creation and cannot be changed.
    /// </summary>
    /// <value>The use exponential large buffer.</value>
    public bool UseExponentialLargeBuffer { get; }

    /// <summary>
    /// Gets the maximum buffer size.
    /// </summary>
    /// <value>The maximum size of the buffer.</value>
    /// <remarks>Any buffer that is returned to the pool that is larger than this will be
    /// discarded and garbage collected.</remarks>
    public int MaximumBufferSize { get; }

    /// <summary>
    /// Number of bytes in small pool not currently in use
    /// </summary>
    /// <value>The size of the small pool free.</value>
    public long SmallPoolFreeSize => this.smallPoolFreeSize;

    /// <summary>
    /// Number of bytes in large pool not currently in use
    /// </summary>
    /// <value>The size of the large pool free.</value>
    public long LargePoolFreeSize => this.largeBufferFreeSize.Sum();

    /// <summary>
    /// Number of bytes currently in use by streams from the large pool
    /// </summary>
    /// <value>The size of the large pool in use.</value>
    public long LargePoolInUseSize => this.largeBufferInUseSize.Sum();

    /// <summary>
    /// How many bytes of small free blocks to allow before we start dropping
    /// those returned to us.
    /// </summary>
    /// <value>The maximum free small pool bytes.</value>
    public long MaximumFreeSmallPoolBytes { get; set; }

    /// <summary>
    /// How many bytes of large free buffers to allow before we start dropping
    /// those returned to us.
    /// </summary>
    /// <value>The maximum free large pool bytes.</value>
    public long MaximumFreeLargePoolBytes { get; set; }

    /// <summary>
    /// Maximum stream capacity in bytes. Attempts to set a larger capacity will
    /// result in an exception.
    /// </summary>
    /// <value>The maximum stream capacity.</value>
    /// <remarks>A value of 0 indicates no limit.</remarks>
    public long MaximumStreamCapacity { get; set; }

    /// <summary>
    /// Whether to save callstacks for stream allocations. This can help in debugging.
    /// It should NEVER be turned on generally in production.
    /// </summary>
    /// <value>The generate call stacks.</value>
    public bool GenerateCallStacks { get; set; }

    /// <summary>
    /// Whether dirty buffers can be immediately returned to the buffer pool. E.g. when GetBuffer() is called on
    /// a stream and creates a single large buffer, if this setting is enabled, the other blocks will be returned
    /// to the buffer pool immediately.
    /// Note when enabling this setting that the user is responsible for ensuring that any buffer previously
    /// retrieved from a stream which is subsequently modified is not used after modification (as it may no longer
    /// be valid).
    /// </summary>
    /// <value>The aggressive buffer return.</value>
    public bool AggressiveBufferReturn { get; set; }

    /// <summary>
    /// Removes and returns a single block from the pool.
    /// </summary>
    /// <returns>A byte[] array</returns>
    internal byte[] GetBlock()
    {
        if (!this.smallPool.TryPop(out var block))
        {
            // We'll add this back to the pool when the stream is disposed
            // (unless our free pool is too large)
            block = new byte[this.BlockSize];
            Events.Writer.MemoryStreamNewBlockCreated(this.smallPoolInUseSize);
            this.ReportBlockCreated();
        }
        else
        {
            Interlocked.Add(ref this.smallPoolFreeSize, -this.BlockSize);
        }

        Interlocked.Add(ref this.smallPoolInUseSize, this.BlockSize);
        return block;
    }

    /// <summary>
    /// Returns a buffer of arbitrary size from the large buffer pool. This buffer
    /// will be at least the requiredSize and always be a multiple/exponential of largeBufferMultiple.
    /// </summary>
    /// <param name="requiredSize">The minimum length of the buffer</param>
    /// <param name="tag">The tag of the stream returning this buffer, for logging if necessary.</param>
    /// <returns>A buffer of at least the required size.</returns>
    internal byte[] GetLargeBuffer(int requiredSize, string tag)
    {
        requiredSize = this.RoundToLargeBufferSize(requiredSize);

        var poolIndex = this.GetPoolIndex(requiredSize);

        byte[] buffer;
        if (poolIndex < this.largePools.Length)
        {
            if (!this.largePools[poolIndex].TryPop(out buffer))
            {
                buffer = new byte[requiredSize];

                Events.Writer.MemoryStreamNewLargeBufferCreated(requiredSize, this.LargePoolInUseSize);
                this.ReportLargeBufferCreated();
            }
            else
            {
                Interlocked.Add(ref this.largeBufferFreeSize[poolIndex], -buffer.Length);
            }
        }
        else
        {
            // Buffer is too large to pool. They get a new buffer.

            // We still want to track the size, though, and we've reserved a slot
            // in the end of the inuse array for nonpooled bytes in use.
            poolIndex = this.largeBufferInUseSize.Length - 1;

            // We still want to round up to reduce heap fragmentation.
            buffer = new byte[requiredSize];
            string callStack = null;
            if (this.GenerateCallStacks)
            {
                // Grab the stack -- we want to know who requires such large buffers
                callStack = Environment.StackTrace;
            }
            Events.Writer.MemoryStreamNonPooledLargeBufferCreated(requiredSize, tag, callStack);
            this.ReportLargeBufferCreated();
        }

        Interlocked.Add(ref this.largeBufferInUseSize[poolIndex], buffer.Length);

        return buffer;
    }

    /// <summary>
    /// Rounds the size of to large buffer.
    /// </summary>
    /// <param name="requiredSize">Size of the required.</param>
    /// <returns>int.</returns>
    private int RoundToLargeBufferSize(int requiredSize)
    {
        if (this.UseExponentialLargeBuffer)
        {
            var pow = 1;
            while (this.LargeBufferMultiple * pow < requiredSize)
            {
                pow <<= 1;
            }
            return this.LargeBufferMultiple * pow;
        }
        else
        {
            return (requiredSize + this.LargeBufferMultiple - 1) / this.LargeBufferMultiple * this.LargeBufferMultiple;
        }
    }

    /// <summary>
    /// Determines whether [is large buffer size] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>bool.</returns>
    private bool IsLargeBufferSize(int value)
    {
        return value != 0 && (this.UseExponentialLargeBuffer
                                  ? value == this.RoundToLargeBufferSize(value)
                                  : value % this.LargeBufferMultiple == 0);
    }

    /// <summary>
    /// Gets the index of the pool.
    /// </summary>
    /// <param name="length">The length.</param>
    /// <returns>int.</returns>
    private int GetPoolIndex(int length)
    {
        if (this.UseExponentialLargeBuffer)
        {
            var index = 0;
            while (this.LargeBufferMultiple << index < length)
            {
                ++index;
            }
            return index;
        }
        else
        {
            return length / this.LargeBufferMultiple - 1;
        }
    }

    /// <summary>
    /// Returns the buffer to the large pool
    /// </summary>
    /// <param name="buffer">The buffer to return.</param>
    /// <param name="tag">The tag of the stream returning this buffer, for logging if necessary.</param>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentException">buffer is null</exception>
    internal void ReturnLargeBuffer(byte[] buffer, string tag)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (!this.IsLargeBufferSize(buffer.Length))
        {
            throw new ArgumentException(
                $"buffer did not originate from this memory manager. The size is not {(this.UseExponentialLargeBuffer ? "an exponential" : "a multiple")} of " +
                this.LargeBufferMultiple);
        }

        var poolIndex = this.GetPoolIndex(buffer.Length);

        if (poolIndex < this.largePools.Length)
        {
            if ((this.largePools[poolIndex].Count + 1) * buffer.Length <= this.MaximumFreeLargePoolBytes ||
                this.MaximumFreeLargePoolBytes == 0)
            {
                this.largePools[poolIndex].Push(buffer);
                Interlocked.Add(ref this.largeBufferFreeSize[poolIndex], buffer.Length);
            }
            else
            {
                Events.Writer.MemoryStreamDiscardBuffer(Events.MemoryStreamBufferType.Large, tag,
                    Events.MemoryStreamDiscardReason.EnoughFree);
                this.ReportLargeBufferDiscarded(Events.MemoryStreamDiscardReason.EnoughFree);
            }
        }
        else
        {
            // This is a non-poolable buffer, but we still want to track its size for inuse
            // analysis. We have space in the inuse array for this.
            poolIndex = this.largeBufferInUseSize.Length - 1;

            Events.Writer.MemoryStreamDiscardBuffer(Events.MemoryStreamBufferType.Large, tag,
                Events.MemoryStreamDiscardReason.TooLarge);
            this.ReportLargeBufferDiscarded(Events.MemoryStreamDiscardReason.TooLarge);
        }

        Interlocked.Add(ref this.largeBufferInUseSize[poolIndex], -buffer.Length);

        this.ReportUsageReport(this.smallPoolInUseSize, this.smallPoolFreeSize, this.LargePoolInUseSize,
            this.LargePoolFreeSize);
    }

    /// <summary>
    /// Returns the blocks to the pool
    /// </summary>
    /// <param name="blocks">Collection of blocks to return to the pool</param>
    /// <param name="tag">The tag of the stream returning these blocks, for logging if necessary.</param>
    /// <exception cref="ArgumentNullException">nameof(blocks)</exception>
    /// <exception cref="ArgumentNullException">nameof(blocks)</exception>
    /// <exception cref="ArgumentException">blocks is null</exception>
    internal void ReturnBlocks(ICollection<byte[]> blocks, string tag)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        var bytesToReturn = blocks.Count * this.BlockSize;
        Interlocked.Add(ref this.smallPoolInUseSize, -bytesToReturn);

        if (blocks.Any(block => block == null || block.Length != this.BlockSize))
        {
            throw new ArgumentException("blocks contains buffers that are not BlockSize in length");
        }

        foreach (var block in blocks)
        {
            if (this.MaximumFreeSmallPoolBytes == 0 || this.SmallPoolFreeSize < this.MaximumFreeSmallPoolBytes)
            {
                Interlocked.Add(ref this.smallPoolFreeSize, this.BlockSize);
                this.smallPool.Push(block);
            }
            else
            {
                Events.Writer.MemoryStreamDiscardBuffer(Events.MemoryStreamBufferType.Small, tag,
                    Events.MemoryStreamDiscardReason.EnoughFree);
                this.ReportBlockDiscarded();
                break;
            }
        }

        this.ReportUsageReport(this.smallPoolInUseSize, this.smallPoolFreeSize, this.LargePoolInUseSize,
            this.LargePoolFreeSize);
    }

    /// <summary>
    /// Reports the block created.
    /// </summary>
    internal void ReportBlockCreated()
    {
        this.BlockCreated?.Invoke();
    }

    /// <summary>
    /// Reports the block discarded.
    /// </summary>
    internal void ReportBlockDiscarded()
    {
        this.BlockDiscarded?.Invoke();
    }

    /// <summary>
    /// Reports the large buffer created.
    /// </summary>
    internal void ReportLargeBufferCreated()
    {
        this.LargeBufferCreated?.Invoke();
    }

    /// <summary>
    /// Reports the large buffer discarded.
    /// </summary>
    /// <param name="reason">The reason.</param>
    internal void ReportLargeBufferDiscarded(Events.MemoryStreamDiscardReason reason)
    {
        this.LargeBufferDiscarded?.Invoke(reason);
    }

    /// <summary>
    /// Reports the stream created.
    /// </summary>
    internal void ReportStreamCreated()
    {
        this.StreamCreated?.Invoke();
    }

    /// <summary>
    /// Reports the stream disposed.
    /// </summary>
    internal void ReportStreamDisposed()
    {
        this.StreamDisposed?.Invoke();
    }

    /// <summary>
    /// Reports the stream finalized.
    /// </summary>
    internal void ReportStreamFinalized()
    {
        this.StreamFinalized?.Invoke();
    }

    /// <summary>
    /// Reports the length of the stream.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    internal void ReportStreamLength(long bytes)
    {
        this.StreamLength?.Invoke(bytes);
    }

    /// <summary>
    /// Reports the usage report.
    /// </summary>
    /// <param name="smallPoolInUseBytes">The small pool in use bytes.</param>
    /// <param name="smallPoolFreeBytes">The small pool free bytes.</param>
    /// <param name="largePoolInUseBytes">The large pool in use bytes.</param>
    /// <param name="largePoolFreeBytes">The large pool free bytes.</param>
    internal void ReportUsageReport(
        long smallPoolInUseBytes, long smallPoolFreeBytes, long largePoolInUseBytes, long largePoolFreeBytes)
    {
        this.UsageReport?.Invoke(smallPoolInUseBytes, smallPoolFreeBytes, largePoolInUseBytes, largePoolFreeBytes);
    }

    /// <summary>
    /// Retrieve a new MemoryStream object with no tag and a default initial capacity.
    /// </summary>
    /// <returns>A MemoryStream.</returns>
    public MemoryStream GetStream()
    {
        return new RecyclableMemoryStream(this);
    }

    /// <summary>
    /// Retrieve a new MemoryStream object with no tag and a default initial capacity.
    /// </summary>
    /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
    /// <returns>A MemoryStream.</returns>
    public MemoryStream GetStream(Guid id)
    {
        return new RecyclableMemoryStream(this, id);
    }

    /// <summary>
    /// Retrieve a new MemoryStream object with the given tag and a default initial capacity.
    /// </summary>
    /// <param name="tag">A tag which can be used to track the source of the stream.</param>
    /// <returns>A MemoryStream.</returns>
    public MemoryStream GetStream(string tag)
    {
        return new RecyclableMemoryStream(this, tag);
    }

    /// <summary>
    /// Retrieve a new MemoryStream object with the given tag and a default initial capacity.
    /// </summary>
    /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
    /// <param name="tag">A tag which can be used to track the source of the stream.</param>
    /// <returns>A MemoryStream.</returns>
    public MemoryStream GetStream(Guid id, string tag)
    {
        return new RecyclableMemoryStream(this, id, tag);
    }

    /// <summary>
    /// Retrieve a new MemoryStream object with the given tag and at least the given capacity.
    /// </summary>
    /// <param name="tag">A tag which can be used to track the source of the stream.</param>
    /// <param name="requiredSize">The minimum desired capacity for the stream.</param>
    /// <returns>A MemoryStream.</returns>
    public MemoryStream GetStream(string tag, int requiredSize)
    {
        return new RecyclableMemoryStream(this, tag, requiredSize);
    }

    /// <summary>
    /// Retrieve a new MemoryStream object with the given tag and at least the given capacity.
    /// </summary>
    /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
    /// <param name="tag">A tag which can be used to track the source of the stream.</param>
    /// <param name="requiredSize">The minimum desired capacity for the stream.</param>
    /// <returns>A MemoryStream.</returns>
    public MemoryStream GetStream(Guid id, string tag, int requiredSize)
    {
        return new RecyclableMemoryStream(this, id, tag, requiredSize);
    }

    /// <summary>
    /// Retrieve a new MemoryStream object with the given tag and at least the given capacity, possibly using
    /// a single contiguous underlying buffer.
    /// </summary>
    /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
    /// <param name="tag">A tag which can be used to track the source of the stream.</param>
    /// <param name="requiredSize">The minimum desired capacity for the stream.</param>
    /// <param name="asContiguousBuffer">Whether to attempt to use a single contiguous buffer.</param>
    /// <returns>A MemoryStream.</returns>
    /// <remarks>Retrieving a MemoryStream which provides a single contiguous buffer can be useful in situations
    /// where the initial size is known and it is desirable to avoid copying data between the smaller underlying
    /// buffers to a single large one. This is most helpful when you know that you will always call GetBuffer
    /// on the underlying stream.</remarks>
    public MemoryStream GetStream(Guid id, string tag, int requiredSize, bool asContiguousBuffer)
    {
        if (!asContiguousBuffer || requiredSize <= this.BlockSize)
        {
            return this.GetStream(id, tag, requiredSize);
        }

        return new RecyclableMemoryStream(this, id, tag, requiredSize, this.GetLargeBuffer(requiredSize, tag));
    }

    /// <summary>
    /// Retrieve a new MemoryStream object with the given tag and at least the given capacity, possibly using
    /// a single contiguous underlying buffer.
    /// </summary>
    /// <param name="tag">A tag which can be used to track the source of the stream.</param>
    /// <param name="requiredSize">The minimum desired capacity for the stream.</param>
    /// <param name="asContiguousBuffer">Whether to attempt to use a single contiguous buffer.</param>
    /// <returns>A MemoryStream.</returns>
    /// <remarks>Retrieving a MemoryStream which provides a single contiguous buffer can be useful in situations
    /// where the initial size is known and it is desirable to avoid copying data between the smaller underlying
    /// buffers to a single large one. This is most helpful when you know that you will always call GetBuffer
    /// on the underlying stream.</remarks>
    public MemoryStream GetStream(string tag, int requiredSize, bool asContiguousBuffer)
    {
        return this.GetStream(Guid.NewGuid(), tag, requiredSize, asContiguousBuffer);
    }

    /// <summary>
    /// Retrieve a new MemoryStream object with the given tag and with contents copied from the provided
    /// buffer. The provided buffer is not wrapped or used after construction.
    /// </summary>
    /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
    /// <param name="tag">A tag which can be used to track the source of the stream.</param>
    /// <param name="buffer">The byte buffer to copy data from.</param>
    /// <param name="offset">The offset from the start of the buffer to copy from.</param>
    /// <param name="count">The number of bytes to copy from the buffer.</param>
    /// <returns>A MemoryStream.</returns>
    /// <exception cref="RecyclableMemoryStream">this, id, tag, count</exception>
    /// <remarks>The new stream's position is set to the beginning of the stream when returned.</remarks>
    public MemoryStream GetStream(Guid id, string tag, byte[] buffer, int offset, int count)
    {
        RecyclableMemoryStream stream = null;
        try
        {
            stream = new RecyclableMemoryStream(this, id, tag, count);
            stream.Write(buffer, offset, count);
            stream.Position = 0;
            return stream;
        }
        catch
        {
            stream?.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Retrieve a new MemoryStream object with the contents copied from the provided
    /// buffer. The provided buffer is not wrapped or used after construction.
    /// </summary>
    /// <param name="buffer">The byte buffer to copy data from.</param>
    /// <returns>A MemoryStream.</returns>
    /// <remarks>The new stream's position is set to the beginning of the stream when returned.</remarks>
    public MemoryStream GetStream(byte[] buffer)
    {
        return this.GetStream(null, buffer, 0, buffer.Length);
    }


    /// <summary>
    /// Retrieve a new MemoryStream object with the given tag and with contents copied from the provided
    /// buffer. The provided buffer is not wrapped or used after construction.
    /// </summary>
    /// <param name="tag">A tag which can be used to track the source of the stream.</param>
    /// <param name="buffer">The byte buffer to copy data from.</param>
    /// <param name="offset">The offset from the start of the buffer to copy from.</param>
    /// <param name="count">The number of bytes to copy from the buffer.</param>
    /// <returns>A MemoryStream.</returns>
    /// <remarks>The new stream's position is set to the beginning of the stream when returned.</remarks>
    public MemoryStream GetStream(string tag, byte[] buffer, int offset, int count)
    {
        return this.GetStream(Guid.NewGuid(), tag, buffer, offset, count);
    }

#if NET10_0_OR_GREATER && !NETSTANDARD2_0
        /// <summary>
        /// Retrieve a new MemoryStream object with the given tag and with contents copied from the provided
        /// buffer. The provided buffer is not wrapped or used after construction.
        /// </summary>
        /// <remarks>The new stream's position is set to the beginning of the stream when returned.</remarks>
        /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="buffer">The byte buffer to copy data from.</param>
        /// <returns>A MemoryStream.</returns>
        public MemoryStream GetStream(Guid id, string tag, Memory<byte> buffer)
        {
            RecyclableMemoryStream stream = null;
            try
            {
                stream = new RecyclableMemoryStream(this, id, tag, buffer.Length);
                stream.Write(buffer.Span);
                stream.Position = 0;
                return stream;
            }
            catch
            {
                stream?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Retrieve a new MemoryStream object with the contents copied from the provided
        /// buffer. The provided buffer is not wrapped or used after construction.
        /// </summary>
        /// <remarks>The new stream's position is set to the beginning of the stream when returned.</remarks>
        /// <param name="buffer">The byte buffer to copy data from.</param>
        /// <returns>A MemoryStream.</returns>
        public MemoryStream GetStream(Memory<byte> buffer)
        {
            return this.GetStream(null, buffer);
        }

        /// <summary>
        /// Retrieve a new MemoryStream object with the given tag and with contents copied from the provided
        /// buffer. The provided buffer is not wrapped or used after construction.
        /// </summary>
        /// <remarks>The new stream's position is set to the beginning of the stream when returned.</remarks>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="buffer">The byte buffer to copy data from.</param>
        /// <returns>A MemoryStream.</returns>
        public MemoryStream GetStream(string tag, Memory<byte> buffer)
        {
            return this.GetStream(Guid.NewGuid(), tag, buffer);
        }
#endif
    /// <summary>
    /// Triggered when a new block is created.
    /// </summary>
    public event EventHandler BlockCreated;

    /// <summary>
    /// Triggered when a new block is created.
    /// </summary>
    public event EventHandler BlockDiscarded;

    /// <summary>
    /// Triggered when a new large buffer is created.
    /// </summary>
    public event EventHandler LargeBufferCreated;

    /// <summary>
    /// Triggered when a new stream is created.
    /// </summary>
    public event EventHandler StreamCreated;

    /// <summary>
    /// Triggered when a stream is disposed.
    /// </summary>
    public event EventHandler StreamDisposed;

    /// <summary>
    /// Triggered when a stream is finalized.
    /// </summary>
    public event EventHandler StreamFinalized;

    /// <summary>
    /// Triggered when a stream is finalized.
    /// </summary>
    public event StreamLengthReportHandler StreamLength;

    /// <summary>
    /// Triggered when a large buffer is discarded, along with the reason for the discard.
    /// </summary>
    public event LargeBufferDiscardedEventHandler LargeBufferDiscarded;

    /// <summary>
    /// Periodically triggered to report usage statistics.
    /// </summary>
    public event UsageReportEventHandler UsageReport;
}


/// <summary>
/// MemoryStream implementation that deals with pooling and managing memory streams which use potentially large
/// buffers.
/// </summary>
/// <remarks>This class works in tandem with the RecyclableMemoryStreamManager to supply MemoryStream
/// objects to callers, while avoiding these specific problems:
/// 1. LOH allocations - since all large buffers are pooled, they will never incur a Gen2 GC
/// 2. Memory waste - A standard memory stream doubles its size when it runs out of room. This
/// leads to continual memory growth as each stream approaches the maximum allowed size.
/// 3. Memory copying - Each time a MemoryStream grows, all the bytes are copied into new buffers.
/// This implementation only copies the bytes when GetBuffer is called.
/// 4. Memory fragmentation - By using homogeneous buffer sizes, it ensures that blocks of memory
/// can be easily reused.
/// The stream is implemented on top of a series of uniformly-sized blocks. As the stream's length grows,
/// additional blocks are retrieved from the memory manager. It is these blocks that are pooled, not the stream
/// object itself.
/// The biggest wrinkle in this implementation is when GetBuffer() is called. This requires a single
/// contiguous buffer. If only a single block is in use, then that block is returned. If multiple blocks
/// are in use, we retrieve a larger buffer from the memory manager. These large buffers are also pooled,
/// split by size--they are multiples/exponentials of a chunk size (1 MB by default).
/// Once a large buffer is assigned to the stream the small blocks are NEVER again used for this stream. All operations take place on the
/// large buffer. The large buffer can be replaced by a larger buffer from the pool as needed. All blocks and large buffers
/// are maintained in the stream until the stream is disposed (unless AggressiveBufferReturn is enabled in the stream manager).
/// </remarks>
public sealed class RecyclableMemoryStream : MemoryStream
{
    /// <summary>
    /// The maximum stream length
    /// </summary>
    private const long MaxStreamLength = int.MaxValue;

    /// <summary>
    /// The empty array
    /// </summary>
    private readonly static byte[] emptyArray = [];

    /// <summary>
    /// All of these blocks must be the same size
    /// </summary>
    private readonly List<byte[]> blocks = new(1);

    /// <summary>
    /// The identifier
    /// </summary>
    private readonly Guid id;

    /// <summary>
    /// The memory manager
    /// </summary>
    private readonly RecyclableMemoryStreamManager memoryManager;

    /// <summary>
    /// The tag
    /// </summary>
    private readonly string tag;

    /// <summary>
    /// This list is used to store buffers once they're replaced by something larger.
    /// This is for the cases where you have users of this class that may hold onto the buffers longer
    /// than they should and you want to prevent race conditions which could corrupt the data.
    /// </summary>
    private List<byte[]> dirtyBuffers;

    // long to allow Interlocked.Read (for .NET Standard 1.4 compat)
    /// <summary>
    /// The disposed state
    /// </summary>
    private long disposedState;

    /// <summary>
    /// This is only set by GetBuffer() if the necessary buffer is larger than a single block size, or on
    /// construction if the caller immediately requests a single large buffer.
    /// </summary>
    private byte[] largeBuffer;

    /// <summary>
    /// Unique identifier for this stream across its entire lifetime
    /// </summary>
    /// <value>The identifier.</value>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    internal Guid Id
    {
        get
        {
            this.CheckDisposed();
            return this.id;
        }
    }

    /// <summary>
    /// A temporary identifier for the current usage of this stream.
    /// </summary>
    /// <value>The tag.</value>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    internal string Tag
    {
        get
        {
            this.CheckDisposed();
            return this.tag;
        }
    }

    /// <summary>
    /// Callstack of the constructor. It is only set if MemoryManager.GenerateCallStacks is true,
    /// which should only be in debugging situations.
    /// </summary>
    /// <value>The allocation stack.</value>
    internal string AllocationStack { get; }

    /// <summary>
    /// Callstack of the Dispose call. It is only set if MemoryManager.GenerateCallStacks is true,
    /// which should only be in debugging situations.
    /// </summary>
    /// <value>The dispose stack.</value>
    internal string DisposeStack { get; private set; }

    /// <summary>
    /// Allocate a new RecyclableMemoryStream object.
    /// </summary>
    /// <param name="memoryManager">The memory manager</param>
    public RecyclableMemoryStream(RecyclableMemoryStreamManager memoryManager)
        : this(memoryManager, Guid.NewGuid(), null, 0, null) { }

    /// <summary>
    /// Allocate a new RecyclableMemoryStream object.
    /// </summary>
    /// <param name="memoryManager">The memory manager</param>
    /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
    public RecyclableMemoryStream(RecyclableMemoryStreamManager memoryManager, Guid id)
        : this(memoryManager, id, null, 0, null) { }

    /// <summary>
    /// Allocate a new RecyclableMemoryStream object
    /// </summary>
    /// <param name="memoryManager">The memory manager</param>
    /// <param name="tag">A string identifying this stream for logging and debugging purposes</param>
    public RecyclableMemoryStream(RecyclableMemoryStreamManager memoryManager, string tag)
        : this(memoryManager, Guid.NewGuid(), tag, 0, null) { }

    /// <summary>
    /// Allocate a new RecyclableMemoryStream object
    /// </summary>
    /// <param name="memoryManager">The memory manager</param>
    /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
    /// <param name="tag">A string identifying this stream for logging and debugging purposes</param>
    public RecyclableMemoryStream(RecyclableMemoryStreamManager memoryManager, Guid id, string tag)
        : this(memoryManager, id, tag, 0, null) { }

    /// <summary>
    /// Allocate a new RecyclableMemoryStream object
    /// </summary>
    /// <param name="memoryManager">The memory manager</param>
    /// <param name="tag">A string identifying this stream for logging and debugging purposes</param>
    /// <param name="requestedSize">The initial requested size to prevent future allocations</param>
    public RecyclableMemoryStream(RecyclableMemoryStreamManager memoryManager, string tag, int requestedSize)
        : this(memoryManager, Guid.NewGuid(), tag, requestedSize, null) { }

    /// <summary>
    /// Allocate a new RecyclableMemoryStream object
    /// </summary>
    /// <param name="memoryManager">The memory manager</param>
    /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
    /// <param name="tag">A string identifying this stream for logging and debugging purposes</param>
    /// <param name="requestedSize">The initial requested size to prevent future allocations</param>
    public RecyclableMemoryStream(RecyclableMemoryStreamManager memoryManager, Guid id, string tag, int requestedSize)
        : this(memoryManager, id, tag, requestedSize, null) { }

    /// <summary>
    /// Allocate a new RecyclableMemoryStream object
    /// </summary>
    /// <param name="memoryManager">The memory manager</param>
    /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
    /// <param name="tag">A string identifying this stream for logging and debugging purposes</param>
    /// <param name="requestedSize">The initial requested size to prevent future allocations</param>
    /// <param name="initialLargeBuffer">An initial buffer to use. This buffer will be owned by the stream and returned to the memory manager upon Dispose.</param>
    internal RecyclableMemoryStream(RecyclableMemoryStreamManager memoryManager, Guid id, string tag, int requestedSize, byte[] initialLargeBuffer)
        : base(emptyArray)
    {
        this.memoryManager = memoryManager;
        this.id = id;
        this.tag = tag;

        if (requestedSize < memoryManager.BlockSize)
        {
            requestedSize = memoryManager.BlockSize;
        }

        if (initialLargeBuffer == null)
        {
            this.EnsureCapacity(requestedSize);
        }
        else
        {
            this.largeBuffer = initialLargeBuffer;
        }

        if (this.memoryManager.GenerateCallStacks)
        {
            this.AllocationStack = Environment.StackTrace;
        }

        RecyclableMemoryStreamManager.Events.Writer.MemoryStreamCreated(this.id, this.tag, requestedSize);
        this.memoryManager.ReportStreamCreated();
    }

    /// <summary>
    /// The finalizer will be called when a stream is not disposed properly.
    /// </summary>
    /// <remarks>Failing to dispose indicates a bug in the code using streams. Care should be taken to properly account for stream lifetime.</remarks>
    ~RecyclableMemoryStream()
    {
        this.Dispose(false);
    }

    /// <summary>
    /// Returns the memory used by this stream back to the pool.
    /// </summary>
    /// <param name="disposing">Whether we're disposing (true), or being called by the finalizer (false)</param>
    override protected void Dispose(bool disposing)
    {
        if (Interlocked.CompareExchange(ref this.disposedState, 1, 0) != 0)
        {
            string doubleDisposeStack = null;
            if (this.memoryManager.GenerateCallStacks)
            {
                doubleDisposeStack = Environment.StackTrace;
            }

            RecyclableMemoryStreamManager.Events.Writer.MemoryStreamDoubleDispose(this.id, this.tag,
                this.AllocationStack, this.DisposeStack, doubleDisposeStack);
            return;
        }

        RecyclableMemoryStreamManager.Events.Writer.MemoryStreamDisposed(this.id, this.tag);

        if (this.memoryManager.GenerateCallStacks)
        {
            this.DisposeStack = Environment.StackTrace;
        }

        if (disposing)
        {
            this.memoryManager.ReportStreamDisposed();

            GC.SuppressFinalize(this);
        }
        else
        {
            // We're being finalized.

            RecyclableMemoryStreamManager.Events.Writer.MemoryStreamFinalized(this.id, this.tag, this.AllocationStack);

            if (AppDomain.CurrentDomain.IsFinalizingForUnload())
            {
                // If we're being finalized because of a shutdown, don't go any further.
                // We have no idea what's already been cleaned up. Triggering events may cause
                // a crash.
                base.Dispose(disposing);
                return;
            }

            this.memoryManager.ReportStreamFinalized();
        }

        this.memoryManager.ReportStreamLength(this.length);

        if (this.largeBuffer != null)
        {
            this.memoryManager.ReturnLargeBuffer(this.largeBuffer, this.tag);
        }

        if (this.dirtyBuffers != null)
        {
            foreach (var buffer in this.dirtyBuffers)
            {
                this.memoryManager.ReturnLargeBuffer(buffer, this.tag);
            }
        }

        this.memoryManager.ReturnBlocks(this.blocks, this.tag);
        this.blocks.Clear();

        base.Dispose(disposing);
    }

    /// <summary>
    /// Equivalent to Dispose
    /// </summary>
    public override void Close()
    {
        this.Dispose(true);
    }

    /// <summary>
    /// Gets or sets the capacity
    /// </summary>
    /// <value>The capacity.</value>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    /// <remarks>Capacity is always in multiples of the memory manager's block size, unless
    /// the large buffer is in use.  Capacity never decreases during a stream's lifetime.
    /// Explicitly setting the capacity to a lower value than the current value will have no effect.
    /// This is because the buffers are all pooled by chunks and there's little reason to
    /// allow stream truncation.
    /// Writing past the current capacity will cause Capacity to automatically increase, until MaximumStreamCapacity is reached.</remarks>
    public override int Capacity
    {
        get
        {
            this.CheckDisposed();
            if (this.largeBuffer != null)
            {
                return this.largeBuffer.Length;
            }

            var size = (long)this.blocks.Count * this.memoryManager.BlockSize;
            return (int)Math.Min(int.MaxValue, size);
        }
        set
        {
            this.CheckDisposed();
            this.EnsureCapacity(value);
        }
    }

    /// <summary>
    /// The length
    /// </summary>
    private int length;

    /// <summary>
    /// Gets the number of bytes written to this stream.
    /// </summary>
    /// <value>The length.</value>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    public override long Length
    {
        get
        {
            this.CheckDisposed();
            return this.length;
        }
    }

    /// <summary>
    /// The position
    /// </summary>
    private int position;

    /// <summary>
    /// Gets the current position in the stream
    /// </summary>
    /// <value>The position.</value>
    /// <exception cref="ArgumentOutOfRangeException">value, value must be non-negative</exception>
    /// <exception cref="ArgumentOutOfRangeException">value, value must be non-negative</exception>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    public override long Position
    {
        get
        {
            this.CheckDisposed();
            return this.position;
        }
        set
        {
            this.CheckDisposed();
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
            }

            if (value > MaxStreamLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "value cannot be more than " + MaxStreamLength);
            }

            this.position = (int)value;
        }
    }

    /// <summary>
    /// Whether the stream can currently read
    /// </summary>
    /// <value>The can read.</value>
    public override bool CanRead => !this.Disposed;

    /// <summary>
    /// Whether the stream can currently seek
    /// </summary>
    /// <value>The can seek.</value>
    public override bool CanSeek => !this.Disposed;

    /// <summary>
    /// Always false
    /// </summary>
    /// <value>The can timeout.</value>
    public override bool CanTimeout => false;

    /// <summary>
    /// Whether the stream can currently write
    /// </summary>
    /// <value>The can write.</value>
    public override bool CanWrite => !this.Disposed;

    /// <summary>
    /// Returns a single buffer containing the contents of the stream.
    /// The buffer may be longer than the stream length.
    /// </summary>
    /// <returns>A byte[] buffer</returns>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    /// <remarks>IMPORTANT: Doing a Write() after calling GetBuffer() invalidates the buffer. The old buffer is held onto
    /// until Dispose is called, but the next time GetBuffer() is called, a new buffer from the pool will be required.</remarks>
    public override byte[] GetBuffer()
    {
        this.CheckDisposed();

        if (this.largeBuffer != null)
        {
            return this.largeBuffer;
        }

        if (this.blocks.Count == 1)
        {
            return this.blocks[0];
        }

        // Buffer needs to reflect the capacity, not the length, because
        // it's possible that people will manipulate the buffer directly
        // and set the length afterward. Capacity sets the expectation
        // for the size of the buffer.
        var newBuffer = this.memoryManager.GetLargeBuffer(this.Capacity, this.tag);

        // InternalRead will check for existence of largeBuffer, so make sure we
        // don't set it until after we've copied the data.
        this.InternalRead(newBuffer, 0, this.length, 0);
        this.largeBuffer = newBuffer;

        if (this.blocks.Count > 0 && this.memoryManager.AggressiveBufferReturn)
        {
            this.memoryManager.ReturnBlocks(this.blocks, this.tag);
            this.blocks.Clear();
        }

        return this.largeBuffer;
    }

    /// <summary>
    /// Reads from the current position into the provided buffer
    /// </summary>
    /// <param name="buffer">Destination buffer</param>
    /// <param name="offset">Offset into buffer at which to start placing the read bytes.</param>
    /// <param name="count">Number of bytes to read.</param>
    /// <returns>The number of bytes read</returns>
    /// <exception cref="ArgumentNullException">buffer is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">offset or count is less than 0</exception>
    /// <exception cref="ArgumentException">offset subtracted from the buffer length is less than count</exception>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    public override int Read(byte[] buffer, int offset, int count)
    {
        return this.SafeRead(buffer, offset, count, ref this.position);
    }

    /// <summary>
    /// Reads from the specified position into the provided buffer
    /// </summary>
    /// <param name="buffer">Destination buffer</param>
    /// <param name="offset">Offset into buffer at which to start placing the read bytes.</param>
    /// <param name="count">Number of bytes to read.</param>
    /// <param name="streamPosition">Position in the stream to start reading from</param>
    /// <returns>The number of bytes read</returns>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentOutOfRangeException">buffer is null</exception>
    /// <exception cref="ArgumentException">offset or count is less than 0</exception>
    /// <exception cref="ObjectDisposedException">offset subtracted from the buffer length is less than count</exception>
    public int SafeRead(byte[] buffer, int offset, int count, ref int streamPosition)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(buffer);

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "offset cannot be negative");
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "count cannot be negative");
        }

        if (offset + count > buffer.Length)
        {
            throw new ArgumentException("buffer length must be at least offset + count");
        }

        var amountRead = this.InternalRead(buffer, offset, count, streamPosition);
        streamPosition += amountRead;
        return amountRead;
    }

#if !NETSTANDARD2_0 && NET10_0_OR_GREATER
    /// <summary>
    /// Reads from the current position into the provided buffer
    /// </summary>
    /// <param name="buffer">Destination buffer</param>
    /// <returns>The number of bytes read</returns>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    public override int Read(Span<byte> buffer)
        {
            return this.SafeRead(buffer, ref this.position);
        }

        /// <summary>
        /// Reads from the specified position into the provided buffer
        /// </summary>
        /// <param name="buffer">Destination buffer</param>
        /// <param name="streamPosition">Position in the stream to start reading from</param>
        /// <returns>The number of bytes read</returns>
        /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
        public int SafeRead(Span<byte> buffer, ref int streamPosition)
        {
            this.CheckDisposed();

            var amountRead = this.InternalRead(buffer, streamPosition);
            streamPosition += amountRead;
            return amountRead;
        }
#endif

    /// <summary>
    /// Writes the buffer to the stream
    /// </summary>
    /// <param name="buffer">Source buffer</param>
    /// <param name="offset">Start position</param>
    /// <param name="count">Number of bytes to write</param>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentNullException">nameof(buffer)</exception>
    /// <exception cref="ArgumentOutOfRangeException">buffer is null</exception>
    /// <exception cref="ArgumentException">offset or count is negative</exception>
    /// <exception cref="ObjectDisposedException">buffer.Length - offset is not less than count</exception>
    public override void Write(byte[] buffer, int offset, int count)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(buffer);

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset,
                "Offset must be in the range of 0 - buffer.Length-1");
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "count must be non-negative");
        }

        if (count + offset > buffer.Length)
        {
            throw new ArgumentException("count must be greater than buffer.Length - offset");
        }

        var blockSize = this.memoryManager.BlockSize;
        var end = (long)this.position + count;
        // Check for overflow
        if (end > MaxStreamLength)
        {
            throw new IOException("Maximum capacity exceeded");
        }

        this.EnsureCapacity((int)end);

        if (this.largeBuffer == null)
        {
            var bytesRemaining = count;
            var bytesWritten = 0;
            var blockAndOffset = this.GetBlockAndRelativeOffset(this.position);

            while (bytesRemaining > 0)
            {
                var currentBlock = this.blocks[blockAndOffset.Block];
                var remainingInBlock = blockSize - blockAndOffset.Offset;
                var amountToWriteInBlock = Math.Min(remainingInBlock, bytesRemaining);

                Buffer.BlockCopy(buffer, offset + bytesWritten, currentBlock, blockAndOffset.Offset,
                    amountToWriteInBlock);

                bytesRemaining -= amountToWriteInBlock;
                bytesWritten += amountToWriteInBlock;

                ++blockAndOffset.Block;
                blockAndOffset.Offset = 0;
            }
        }
        else
        {
            Buffer.BlockCopy(buffer, offset, this.largeBuffer, this.position, count);
        }
        this.position = (int)end;
        this.length = Math.Max(this.position, this.length);
    }

#if !NETSTANDARD2_0 && NET10_0_OR_GREATER
    /// <summary>
    /// Writes the buffer to the stream
    /// </summary>
    /// <param name="source">Source buffer</param>
    /// <exception cref="ArgumentNullException">buffer is null</exception>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    public override void Write(ReadOnlySpan<byte> source)
        {
            this.CheckDisposed();

            var blockSize = this.memoryManager.BlockSize;
            var end = (long)this.position + source.Length;
            // Check for overflow
            if (end > MaxStreamLength)
            {
                throw new IOException("Maximum capacity exceeded");
            }

            this.EnsureCapacity((int)end);

            if (this.largeBuffer == null)
            {
                var blockAndOffset = this.GetBlockAndRelativeOffset(this.position);

                while (source.Length > 0)
                {
                    var currentBlock = this.blocks[blockAndOffset.Block];
                    var remainingInBlock = blockSize - blockAndOffset.Offset;
                    var amountToWriteInBlock = Math.Min(remainingInBlock, source.Length);

                    source.Slice(0, amountToWriteInBlock)
                        .CopyTo(currentBlock.AsSpan(blockAndOffset.Offset));

                    source = source.Slice(amountToWriteInBlock);

                    ++blockAndOffset.Block;
                    blockAndOffset.Offset = 0;
                }
            }
            else
            {
                source.CopyTo(this.largeBuffer.AsSpan(this.position));
            }
            this.position = (int)end;
            this.length = Math.Max(this.position, this.length);
        }
#endif

    /// <summary>
    /// Returns a useful string for debugging. This should not normally be called in actual production code.
    /// </summary>
    /// <returns>string.</returns>
    public override string ToString()
    {
        return $"Id = {this.Id}, Tag = {this.Tag}, Length = {this.Length:N0} bytes";
    }

    /// <summary>
    /// Writes a single byte to the current position in the stream.
    /// </summary>
    /// <param name="value">byte value to write</param>
    /// <exception cref="IOException">Maximum capacity exceeded</exception>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    public override void WriteByte(byte value)
    {
        this.CheckDisposed();

        var end = (long)this.position + 1;

        // Check for overflow
        if (end > MaxStreamLength)
        {
            throw new IOException("Maximum capacity exceeded");
        }

        if (this.largeBuffer == null)
        {
            var blockSize = this.memoryManager.BlockSize;

            var block = this.position / blockSize;

            if (block >= this.blocks.Count)
            {
                this.EnsureCapacity((int)end);
            }

            this.blocks[block][this.position % blockSize] = value;
        }
        else
        {
            if (this.position >= this.largeBuffer.Length)
            {
                this.EnsureCapacity((int)end);
            }

            this.largeBuffer[this.position] = value;
        }

        this.position = (int)end;

        if (this.position > this.length)
        {
            this.length = this.position;
        }
    }

    /// <summary>
    /// Reads a single byte from the current position in the stream.
    /// </summary>
    /// <returns>The byte at the current position, or -1 if the position is at the end of the stream.</returns>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    public override int ReadByte()
    {
        return this.SafeReadByte(ref this.position);
    }

    /// <summary>
    /// Reads a single byte from the specified position in the stream.
    /// </summary>
    /// <param name="streamPosition">The position in the stream to read from</param>
    /// <returns>The byte at the current position, or -1 if the position is at the end of the stream.</returns>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    public int SafeReadByte(ref int streamPosition)
    {
        this.CheckDisposed();
        if (streamPosition == this.length)
        {
            return -1;
        }
        byte value;
        if (this.largeBuffer == null)
        {
            var blockAndOffset = this.GetBlockAndRelativeOffset(streamPosition);
            value = this.blocks[blockAndOffset.Block][blockAndOffset.Offset];
        }
        else
        {
            value = this.largeBuffer[streamPosition];
        }
        streamPosition++;
        return value;
    }

    /// <summary>
    /// Sets the length of the stream
    /// </summary>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentOutOfRangeException">nameof(value), value must be non-negative and at most " + MaxStreamLength</exception>
    /// <exception cref="ObjectDisposedException">value is negative or larger than MaxStreamLength</exception>
    public override void SetLength(long value)
    {
        this.CheckDisposed();
        if (value < 0 || value > MaxStreamLength)
        {
            throw new ArgumentOutOfRangeException(nameof(value),
                "value must be non-negative and at most " + MaxStreamLength);
        }

        this.EnsureCapacity((int)value);

        this.length = (int)value;
        if (this.position > value)
        {
            this.position = (int)value;
        }
    }

    /// <summary>
    /// Sets the position to the offset from the seek location
    /// </summary>
    /// <param name="offset">How many bytes to move</param>
    /// <param name="loc">From where</param>
    /// <returns>The new position</returns>
    /// <exception cref="ArgumentOutOfRangeException">nameof(offset), offset cannot be larger than " + MaxStreamLength</exception>
    /// <exception cref="ArgumentOutOfRangeException">nameof(offset), offset cannot be larger than " + MaxStreamLength</exception>
    /// <exception cref="ArgumentOutOfRangeException">nameof(offset), offset cannot be larger than " + MaxStreamLength</exception>
    /// <exception cref="ObjectDisposedException">Object has been disposed</exception>
    /// <exception cref="ArgumentException">offset is larger than MaxStreamLength</exception>
    /// <exception cref="IOException">Invalid seek origin</exception>
    public override long Seek(long offset, SeekOrigin loc)
    {
        this.CheckDisposed();
        if (offset > MaxStreamLength)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "offset cannot be larger than " + MaxStreamLength);
        }

        var newPosition = loc switch {
                SeekOrigin.Begin => (int)offset,
                SeekOrigin.Current => (int)offset + this.position,
                SeekOrigin.End => (int)offset + this.length,
                _ => throw new ArgumentException("Invalid seek origin", nameof(loc))
            };
        if (newPosition < 0)
        {
            throw new IOException("Seek before beginning");
        }
        this.position = newPosition;
        return this.position;
    }

    /// <summary>
    /// Synchronously writes this stream's bytes to the argument stream.
    /// </summary>
    /// <param name="stream">Destination stream</param>
    /// <exception cref="ArgumentNullException">stream is null</exception>
    /// <remarks>Important: This does a synchronous write, which may not be desired in some situations</remarks>
    public override void WriteTo(Stream stream)
    {
        this.WriteTo(stream, 0, this.length);
    }

    /// <summary>
    /// Synchronously writes this stream's bytes, starting at offset, for count bytes, to the argument stream.
    /// </summary>
    /// <param name="stream">Destination stream</param>
    /// <param name="offset">Offset in source</param>
    /// <param name="count">Number of bytes to write</param>
    /// <exception cref="ArgumentNullException">nameof(stream)</exception>
    /// <exception cref="ArgumentNullException">nameof(stream)</exception>
    /// <exception cref="ArgumentOutOfRangeException">stream is null</exception>
    public void WriteTo(Stream stream, int offset, int count)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(stream);

        if (offset < 0 || offset + count > this.length)
        {
            throw new ArgumentOutOfRangeException("offset must not be negative and offset + count must not exceed the length of the stream", innerException: null);
        }

        if (this.largeBuffer == null)
        {
            var blockAndOffset = this.GetBlockAndRelativeOffset(offset);
            var bytesRemaining = count;
            var currentBlock = blockAndOffset.Block;
            var currentOffset = blockAndOffset.Offset;

            while (bytesRemaining > 0)
            {
                var amountToCopy = Math.Min(this.blocks[currentBlock].Length - currentOffset, bytesRemaining);
                stream.Write(this.blocks[currentBlock], currentOffset, amountToCopy);

                bytesRemaining -= amountToCopy;

                ++currentBlock;
                currentOffset = 0;
            }
        }
        else
        {
            stream.Write(this.largeBuffer, offset, count);
        }

    }

    /// <summary>
    /// Sets the disposed.
    /// </summary>
    /// <value>The disposed.</value>
    private bool Disposed => Interlocked.Read(ref this.disposedState) != 0;

    /// <summary>
    /// Checks the disposed.
    /// </summary>
    [MethodImpl((MethodImplOptions)256)]
    private void CheckDisposed()
    {
        if (this.Disposed)
        {
            this.ThrowDisposedException();
        }
    }

    /// <summary>
    /// Throws the disposed exception.
    /// </summary>
    /// <exception cref="ObjectDisposedException">$"The stream with Id {this.id} and Tag {this.tag} is disposed.</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowDisposedException()
    {
        throw new ObjectDisposedException($"The stream with Id {this.id} and Tag {this.tag} is disposed.");
    }

    /// <summary>
    /// Internals the read.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="count">The count.</param>
    /// <param name="fromPosition">From position.</param>
    /// <returns>int.</returns>
    private int InternalRead(byte[] buffer, int offset, int count, int fromPosition)
    {
        if (this.length - fromPosition <= 0)
        {
            return 0;
        }

        int amountToCopy;

        if (this.largeBuffer == null)
        {
            var blockAndOffset = this.GetBlockAndRelativeOffset(fromPosition);
            var bytesWritten = 0;
            var bytesRemaining = Math.Min(count, this.length - fromPosition);

            while (bytesRemaining > 0)
            {
                amountToCopy = Math.Min(this.blocks[blockAndOffset.Block].Length - blockAndOffset.Offset,
                    bytesRemaining);
                Buffer.BlockCopy(this.blocks[blockAndOffset.Block], blockAndOffset.Offset, buffer,
                    bytesWritten + offset, amountToCopy);

                bytesWritten += amountToCopy;
                bytesRemaining -= amountToCopy;

                ++blockAndOffset.Block;
                blockAndOffset.Offset = 0;
            }
            return bytesWritten;
        }
        amountToCopy = Math.Min(count, this.length - fromPosition);
        Buffer.BlockCopy(this.largeBuffer, fromPosition, buffer, offset, amountToCopy);
        return amountToCopy;
    }

#if NET10_0_OR_GREATER
        private int InternalRead(Span<byte> buffer, int fromPosition)
        {
            if (this.length - fromPosition <= 0)
            {
                return 0;
            }

            int amountToCopy;

            if (this.largeBuffer == null)
            {
                var blockAndOffset = this.GetBlockAndRelativeOffset(fromPosition);
                var bytesWritten = 0;
                var bytesRemaining = Math.Min(buffer.Length, this.length - fromPosition);

                while (bytesRemaining > 0)
                {
                    amountToCopy = Math.Min(this.blocks[blockAndOffset.Block].Length - blockAndOffset.Offset,
                                            bytesRemaining);
                    this.blocks[blockAndOffset.Block].AsSpan(blockAndOffset.Offset, amountToCopy)
                        .CopyTo(buffer.Slice(bytesWritten));

                    bytesWritten += amountToCopy;
                    bytesRemaining -= amountToCopy;

                    ++blockAndOffset.Block;
                    blockAndOffset.Offset = 0;
                }
                return bytesWritten;
            }
            amountToCopy = Math.Min(buffer.Length, this.length - fromPosition);
            this.largeBuffer.AsSpan(fromPosition, amountToCopy).CopyTo(buffer);
            return amountToCopy;
        }
#endif

    /// <summary>
    /// Struct BlockAndOffset
    /// </summary>
    private struct BlockAndOffset
    {
        /// <summary>
        /// The block
        /// </summary>
        public int Block;
        /// <summary>
        /// The offset
        /// </summary>
        public int Offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockAndOffset"/> struct.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="offset">The offset.</param>
        public BlockAndOffset(int block, int offset)
        {
            this.Block = block;
            this.Offset = offset;
        }
    }

    /// <summary>
    /// Gets the block and relative offset.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <returns>ServiceStack.Text.RecyclableMemoryStream.BlockAndOffset.</returns>
    [MethodImpl((MethodImplOptions)256)]
    private BlockAndOffset GetBlockAndRelativeOffset(int offset)
    {
        var blockSize = this.memoryManager.BlockSize;
        return new BlockAndOffset(offset / blockSize, offset % blockSize);
    }

    /// <summary>
    /// Ensures the capacity.
    /// </summary>
    /// <param name="newCapacity">The new capacity.</param>
    /// <exception cref="InvalidOperationException">Requested capacity is too large: " + newCapacity + ". Limit is " +
    ///                                                     this.memoryManager.MaximumStreamCapacity</exception>
    private void EnsureCapacity(int newCapacity)
    {
        if (newCapacity > this.memoryManager.MaximumStreamCapacity && this.memoryManager.MaximumStreamCapacity > 0)
        {
            RecyclableMemoryStreamManager.Events.Writer.MemoryStreamOverCapacity(newCapacity,
                this.memoryManager.MaximumStreamCapacity, this.tag, this.AllocationStack);
            throw new InvalidOperationException("Requested capacity is too large: " + newCapacity + ". Limit is " +
                                                this.memoryManager.MaximumStreamCapacity);
        }

        if (this.largeBuffer != null)
        {
            if (newCapacity > this.largeBuffer.Length)
            {
                var newBuffer = this.memoryManager.GetLargeBuffer(newCapacity, this.tag);
                this.InternalRead(newBuffer, 0, this.length, 0);
                this.ReleaseLargeBuffer();
                this.largeBuffer = newBuffer;
            }
        }
        else
        {
            while (this.Capacity < newCapacity)
            {
                this.blocks.Add(this.memoryManager.GetBlock());
            }
        }
    }

    /// <summary>
    /// Release the large buffer (either stores it for eventual release or returns it immediately).
    /// </summary>
    private void ReleaseLargeBuffer()
    {
        if (this.memoryManager.AggressiveBufferReturn)
        {
            this.memoryManager.ReturnLargeBuffer(this.largeBuffer, this.tag);
        }
        else
        {
            this.dirtyBuffers ??= new List<byte[]>(1);
            this.dirtyBuffers.Add(this.largeBuffer);
        }

        this.largeBuffer = null;
    }
}