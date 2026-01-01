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
/// The medal flags.
/// </summary>
[Serializable]
public class MedalFlags : FlagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MedalFlags"/> class.
    /// </summary>
    public MedalFlags()
        : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MedalFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public MedalFlags(Flags flags)
        : this((int)flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MedalFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public MedalFlags(object bitValue)
        : this((int)bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MedalFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public MedalFlags(int bitValue)
        : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MedalFlags"/> class.
    /// </summary>forum_topics
    /// <param name="bits">
    /// The bits.
    /// </param>
    public MedalFlags(params bool[] bits)
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
        /// The show message.
        /// </summary>
        ShowMessage = 1,

        /// <summary>
        /// The allow ribbon.
        /// </summary>
        AllowRibbon = 2,

        /// <summary>
        /// The allow hiding.
        /// </summary>
        AllowHiding = 4,

        /// <summary>
        /// The allow re ordering.
        /// </summary>
        AllowReOrdering = 8

        /* for future use
              xxxxxxxx = 16,
              xxxxxxxx = 32,
              xxxxxxxx = 64,
              xxxxxxxx = 128,
              xxxxxxxx = 256,
              xxxxxxxx = 512,
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
    /// Gets or sets a value indicating whether medal message is shown.
    /// </summary>
    public virtual bool ShowMessage
    {
        // int value 1
        get => this[0];

        set => this[0] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the medal can be hidden by user.
    /// </summary>
    public virtual bool AllowHiding
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
    public static implicit operator MedalFlags(int newBitValue)
    {
        return new MedalFlags(newBitValue);
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator MedalFlags(Flags flags)
    {
        return new MedalFlags(flags);
    }
}