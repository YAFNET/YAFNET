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
/// The forum flags.
/// </summary>
[Serializable]
public class ForumFlags : FlagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForumFlags"/> class.
    /// </summary>
    public ForumFlags()
        : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public ForumFlags(Flags flags)
        : this((int)flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public ForumFlags(object bitValue)
        : this((int)bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public ForumFlags(int bitValue)
        : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public ForumFlags(params bool[] bits)
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
        /// The is locked.
        /// </summary>
        IsLocked = 1,

        /// <summary>
        /// The is hidden.
        /// </summary>
        IsHidden = 2,

        /// <summary>
        /// The is test.
        /// </summary>
        IsTest = 4,

        /// <summary>
        /// The is moderated.
        /// </summary>
        IsModerated = 8

        /* for future use
              xxxxx = 16,
              xxxxx = 32,
              xxxxx = 64,
              xxxxx = 128,
              xxxxx = 256,
              xxxxx = 512
               */
    }

    /// <summary>
    /// Gets or sets a value indicating whether forum allows locked. No posting/activity can be made in locked forums.
    /// </summary>
    public bool IsLocked
    {
        // int value 1
        get => this[0];

        set => this[0] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether forum is hidden to users without read access.
    /// </summary>
    public bool IsHidden
    {
        // int value 2
        get => this[1];

        set => this[1] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether forum does not count to users' post count.
    /// </summary>
    public bool IsTest
    {
        // int value 4
        get => this[2];

        set => this[2] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the forum is moderated.
    /// Posts in moderated posts has to be approved by moderator before they are published.
    /// </summary>
    public bool IsModerated
    {
        // int value 8
        get => this[3];

        set => this[3] = value;
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="newBitValue">
    /// The new bit value.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator ForumFlags(int newBitValue)
    {
        return new ForumFlags(newBitValue);
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator ForumFlags(Flags flags)
    {
        return new ForumFlags(flags);
    }
}