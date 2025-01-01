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
/// The private message flags.
/// </summary>
[Serializable]
public class PrivateMessageFlags : FlagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrivateMessageFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public PrivateMessageFlags(int bitValue)
        : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrivateMessageFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public PrivateMessageFlags(params bool[] bits)
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
        /// Is read by to user (receiver)
        /// </summary>
        IsRead = 1,

        /// <summary>
        /// Is deleted by from user (sender)
        /// </summary>
        IsDeletedByFromUser = 2,

        /// <summary>
        /// Is deleted by to user (receiver)
        /// </summary>
        IsDeletedByToUser = 4
    }

    /// <summary>
    /// Gets or sets a value indicating whether
    /// Is read by to user (receiver)
    /// </summary>
    public bool IsRead
    {
        // int value 1
        get => this[0];

        set => this[0] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether
    /// Is deleted by from user (sender)
    /// </summary>
    public bool IsDeletedByFromUser
    {
        // int value 2
        get => this[1];

        set => this[1] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether
    /// Is deleted by to user (receiver)
    /// </summary>
    public bool IsDeletedByToUser
    {
        // int value 4
        get => this[2];

        set => this[2] = value;
    }
}