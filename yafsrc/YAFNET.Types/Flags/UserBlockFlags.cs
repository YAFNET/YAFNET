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
/// The User block flags
/// </summary>
[Serializable]
public class UserBlockFlags : FlagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserBlockFlags"/> class.
    /// </summary>
    public UserBlockFlags()
        : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserBlockFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public UserBlockFlags(Flags flags)
        : this((int)flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserBlockFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public UserBlockFlags(object bitValue)
        : this((int)bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserBlockFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public UserBlockFlags(int bitValue)
        : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserBlockFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public UserBlockFlags(params bool[] bits)
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
        /// None Flag
        /// </summary>
        None = 0,

        /// <summary>
        /// The block PMs.
        /// </summary>
        BlockPMs = 1,

        /// <summary>
        /// The block friend requests.
        /// </summary>
        BlockFriendRequests = 2,

        /// <summary>
        /// The block emails.
        /// </summary>
        BlockEmails = 4
    }

    /// <summary>
    /// Gets or sets a value indicating whether block PMs.
    /// </summary>
    public bool BlockPMs
    {
        // int value 1
        get => this[0];

        set => this[0] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether block friend requests.
    /// </summary>
    public bool BlockFriendRequests
    {
        // int value 2
        get => this[1];

        set => this[1] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether block emails.
    /// </summary>
    public bool BlockEmails
    {
        // int value 4
        get => this[2];

        set => this[2] = value;
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="newBitValue">
    /// The new bit value.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator UserBlockFlags(int newBitValue)
    {
        return new UserBlockFlags(newBitValue);
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator UserBlockFlags(Flags flags)
    {
        return new UserBlockFlags(flags);
    }
}