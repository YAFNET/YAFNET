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
/// The poll flags.
/// </summary>
[Serializable]
public class PollFlags : FlagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PollFlags"/> class.
    /// </summary>
    public PollFlags()
        : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PollFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public PollFlags(Flags flags)
        : this((int)flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PollFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public PollFlags(object bitValue)
        : this((int)bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PollFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public PollFlags(int bitValue)
        : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PollFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public PollFlags(params bool[] bits)
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
        /// The is start.
        /// </summary>
        IsClosedBound = 4,

        /// <summary>
        /// The allow multiple choices.
        /// </summary>
        AllowMultipleChoices = 8,

        /// <summary>
        /// The show voters.
        /// </summary>
        ShowVoters = 16,

        /// <summary>
        /// The allow skip vote.
        /// </summary>
        AllowSkipVote = 32
    }

    /// <summary>
    /// Gets or sets a value indicating whether is closed bound.
    /// </summary>
    public bool IsClosedBound
    {
        // int value 4
        get => this[2];

        set => this[2] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether allow multiple choice.
    /// </summary>
    public bool AllowMultipleChoice
    {
        // int value 8
        get => this[3];

        set => this[3] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether show voters.
    /// </summary>
    public bool ShowVoters
    {
        // int value 8
        get => this[4];

        set => this[4] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether allow skip vote.
    /// </summary>
    public bool AllowSkipVote
    {
        // int value 8
        get => this[5];

        set => this[5] = value;
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="newBitValue">
    /// The new bit value.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator PollFlags(int newBitValue)
    {
        return new PollFlags(newBitValue);
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator PollFlags(Flags flags)
    {
        return new PollFlags(flags);
    }
}