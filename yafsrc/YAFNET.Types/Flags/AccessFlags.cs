/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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
/// The access flags.
/// </summary>
public class AccessFlags : FlagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccessFlags"/> class.
    /// </summary>
    public AccessFlags()
        : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public AccessFlags(Flags flags)
        : this((int)flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public AccessFlags(object bitValue)
        : this((int)bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public AccessFlags(int bitValue)
        : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public AccessFlags(params bool[] bits)
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
        /// The none.
        /// </summary>
        None = 0,

        /// <summary>
        /// The read access.
        /// </summary>
        ReadAccess = 1,

        /// <summary>
        /// The post access.
        /// </summary>
        PostAccess = 2,

        /// <summary>
        /// The reply access.
        /// </summary>
        ReplyAccess = 4,

        /// <summary>
        /// The priority access.
        /// </summary>
        PriorityAccess = 8,

        /// <summary>
        /// The poll access.
        /// </summary>
        PollAccess = 16,

        /// <summary>
        /// The vote access.
        /// </summary>
        VoteAccess = 32,

        /// <summary>
        /// The moderator access.
        /// </summary>
        ModeratorAccess = 64,

        /// <summary>
        /// The edit access.
        /// </summary>
        EditAccess = 128,

        /// <summary>
        /// The delete access.
        /// </summary>
        DeleteAccess = 256

        /* for future use
          xxxxxxxx =  512,
          xxxxxxxx = 1024,
          xxxxxxxx = 2048,
          xxxxxxxx = 4096,
          xxxxxxxx = 8192,
          xxxxxxxx = 16384,
          xxxxxxxx = 32768,
          xxxxxxxx = 65536
               */
    }

    /// <summary>
    /// Gets or sets a value indicating whether the User has Forum read access.
    /// </summary>
    public bool ReadAccess
    {
        // int value 1
        get => this[0];

        set => this[0] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the User has post access.
    /// </summary>
    public bool PostAccess
    {
        // int value 2
        get => this[1];

        set => this[1] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the User has reply access.
    /// </summary>
    public bool ReplyAccess
    {
        // int value 4
        get => this[2];

        set => this[2] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the User has priority access.
    /// </summary>
    public bool PriorityAccess
    {
        // int value 8
        get => this[3];

        set => this[3] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the User has poll access.
    /// </summary>
    public bool PollAccess
    {
        // int value 16
        get => this[4];

        set => this[4] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the User has vote access.
    /// </summary>
    public bool VoteAccess
    {
        // int value 32
        get => this[5];

        set => this[5] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the User has moderate access.
    /// </summary>
    public bool ModeratorAccess
    {
        // int value 64
        get => this[6];

        set => this[6] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the User has edit access.
    /// </summary>
    public bool EditAccess
    {
        // int value 128
        get => this[7];

        set => this[7] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the User has delete access.
    /// </summary>
    public bool DeleteAccess
    {
        // int value 256
        get => this[8];

        set => this[8] = value;
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="newBitValue">
    /// The new bit value.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator AccessFlags(int newBitValue)
    {
        var flags = new AccessFlags(newBitValue);
        return flags;
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator AccessFlags(Flags flags)
    {
        return new AccessFlags(flags);
    }
}