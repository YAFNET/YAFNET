/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Types.Flags;

/// <summary>
/// The message flags.
/// </summary>
[Serializable]
public class MessageFlags : FlagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageFlags"/> class.
    /// </summary>
    public MessageFlags()
        : this(Flags.IsHtml | Flags.IsBBCode | Flags.IsSmilies)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public MessageFlags(Flags flags)
        : this((int)flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public MessageFlags(object bitValue)
        : this((int)bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public MessageFlags(int bitValue)
        : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public MessageFlags(params bool[] bits)
        : base(bits)
    {
    }

    /// <summary>
    /// Use for bit comparisons
    /// </summary>
    [Flags]
    public enum Flags
    {
        /// <summary>
        /// The is html.
        /// </summary>
        IsHtml = 1,

        /// <summary>
        /// The is BBCode.
        /// </summary>
        IsBBCode = 2,

        /// <summary>
        /// The is smilies.
        /// </summary>
        IsSmilies = 4,

        /// <summary>
        /// The is deleted.
        /// </summary>
        IsDeleted = 8,

        /// <summary>
        /// The is approved.
        /// </summary>
        IsApproved = 16,

        /// <summary>
        /// The is locked.
        /// </summary>
        IsLocked = 32,

        /// <summary>
        /// The not formatted.
        /// </summary>
        NotFormatted = 64,

        /// <summary>
        /// The is reported .
        /// </summary>
        IsReported = 128,

        /// <summary>
        /// Legacy flag not in use.
        /// </summary>
        [Obsolete("Legacy MessageFlag. Not in use.")]
        IsReportedSpam = 256,

        /// <summary>
        /// Is Message Persistent
        /// </summary>
        IsPersistent = 512,

        /// <summary>
        /// Is Message Answer
        /// </summary>
        IsAnswer = 1024

        /* for future use
              xxxxxxxx = 256,
              xxxxxxxx = 2048,
              xxxxxxxx = 4096,
              xxxxxxxx = 8192,
              xxxxxxxx = 16384,
              xxxxxxxx = 32768,
              xxxxxxxx = 65536
               */
    }

    /// <summary>
    /// Gets or sets a value indicating whether this message allows HTML.
    /// </summary>
    public bool IsHtml
    {
        // int value 1
        get => this[0];

        set => this[0] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this message allows BB code.
    /// </summary>
    public bool IsBBCode
    {
        // int value 2
        get => this[1];

        set => this[1] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this message allows smilies.
    /// </summary>
    public bool IsSmilies
    {
        // int value 4
        get => this[2];

        set => this[2] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this  message is deleted.
    /// </summary>
    public virtual bool IsDeleted
    {
        // int value 8
        get => this[3];

        set => this[3] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this message is approved for publishing.
    /// </summary>
    public bool IsApproved
    {
        // int value 16
        get => this[4];

        set => this[4] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this message is locked.
    /// Locked messages cannot be modified/deleted/replied to.
    /// </summary>
    /// <remarks>
    /// Used for "ghost" posts that don't really exist,
    /// such as advertisement posts.
    /// </remarks>
    public bool IsLocked
    {
        // int value 32
        get => this[5];

        set => this[5] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this message is not formatted.
    /// </summary>
    public bool NotFormatted
    {
        // int value 64
        get => this[6];

        set => this[6] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this message is reported as abusive.
    /// </summary>
    public bool IsReported
    {
        // int value 128
        get => this[7];

        set => this[7] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this message is reported as spam
    /// </summary>
    [Obsolete("Legacy flag. Not in use.")]
    public bool IsReportedSpam
    {
        // int value 256
        get => this[8];

        set => this[8] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this message is persistent.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this message is persistent; otherwise, <c>false</c>.
    /// </value>
    public bool IsPersistent
    {
        // int value 512
        get => this[9];

        set => this[9] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this message is flagged as answer
    /// </summary>
    public bool IsAnswer
    {
        // int value 512
        get => this[10];

        set => this[10] = value;
    }
}