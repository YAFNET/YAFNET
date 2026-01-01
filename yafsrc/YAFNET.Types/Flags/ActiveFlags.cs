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
/// The access flags.
/// </summary>
public class ActiveFlags : FlagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveFlags"/> class.
    /// </summary>
    public ActiveFlags()
        : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public ActiveFlags(Flags flags)
        : this((int)flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public ActiveFlags(object bitValue)
        : this((int)bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public ActiveFlags(int bitValue)
        : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public ActiveFlags(params bool[] bits)
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
        /// The Is Active Now.
        /// </summary>
        IsActiveNow = 1,

        /// <summary>
        /// The Is Guest.
        /// </summary>
        IsGuest = 2,

        /// <summary>
        /// The Is Registered.
        /// </summary>
        IsRegistered = 4,

        /// <summary>
        /// The Is Crawler.
        /// </summary>
        IsCrawler = 8
    }

    /// <summary>
    /// Gets or sets a value indicating whether the user is active right now.
    /// </summary>
    public bool IsActiveNow
    {
        // int value 1
        get => this[0];

        set => this[0] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether that user is a guest.
    /// </summary>
    public bool IsGuest
    {
        // int value 2
        get => this[1];

        set => this[1] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the user is a registered one.
    /// </summary>
    public bool IsRegistered
    {
        // int value 4
        get => this[2];

        set => this[2] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether that user is a crawler.
    /// </summary>
    public bool IsCrawler
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
    public static implicit operator ActiveFlags(int newBitValue)
    {
        var flags = new ActiveFlags(newBitValue);
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
    public static implicit operator ActiveFlags(Flags flags)
    {
        return new ActiveFlags(flags);
    }
}