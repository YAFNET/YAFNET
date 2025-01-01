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
/// The topic flags.
/// </summary>
[Serializable]
public class TopicFlags : FlagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TopicFlags"/> class.
    /// </summary>
    public TopicFlags()
        : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TopicFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public TopicFlags(Flags flags)
        : this((int)flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TopicFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public TopicFlags(object bitValue)
        : base((int)bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TopicFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public TopicFlags(int bitValue)
        : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TopicFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public TopicFlags(params bool[] bits)
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
        /// The is locked.
        /// </summary>
        IsLocked = 1,

        /// <summary>
        /// The is deleted.
        /// </summary>
        IsDeleted = 8,

        /// <summary>
        /// The is persistent.
        /// </summary>
        IsPersistent = 512

        /* for future use
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
    /// Gets or sets a value indicating whether topic is locked. Locked topics cannot be modified/deleted/replied to.
    /// </summary>
    public virtual bool IsLocked
    {
        // int value 1
        get => this[0];

        set => this[0] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether topic is deleted.
    /// </summary>
    public virtual bool IsDeleted
    {
        // int value 8
        get => this[3];

        set => this[3] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether topic is persistent. Persistent topics cannot be purged.
    /// </summary>
    public virtual bool IsPersistent
    {
        // int value 512
        get => this[9];

        set => this[9] = value;
    }
}