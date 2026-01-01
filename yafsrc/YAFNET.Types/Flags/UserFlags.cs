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
/// User flags manipulation class from the DB.
/// </summary>
[Serializable]
public class UserFlags : FlagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserFlags"/> class.
    /// </summary>
    public UserFlags()
        : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserFlags"/> class.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    public UserFlags(Flags flags)
        : this((int)flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public UserFlags(object bitValue)
        : this((int)bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public UserFlags(int bitValue)
        : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserFlags"/> class.
    /// </summary>
    /// <param name="bits">
    /// The bits.
    /// </param>
    public UserFlags(params bool[] bits)
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
        /// The is host admin.
        /// </summary>
        IsHostAdmin = 1,

        /// <summary>
        /// The is approved.
        /// </summary>
        IsApproved = 2,

        /// <summary>
        /// The is guest.
        /// </summary>
        IsGuest = 4,

        /// <summary>
        /// The is captcha excluded.
        /// </summary>
        IsCaptchaExcluded = 8,

        /// <summary>
        /// The is active excluded.
        /// </summary>
        IsActiveExcluded = 16,

        /// <summary>
        /// The User is deleted flag.
        /// </summary>
        IsDeleted = 32,

        /// <summary>
        /// Is Dirty data flag.
        /// </summary>
        IsDirty = 64,

        /// <summary>
        /// The moderated flag.
        /// </summary>
        Moderated = 128

        /*  for future use
         *   xxxxx = 256,
             xxxxx = 512
              */
    }

    /// <summary>
    /// Gets or sets a value indicating whether the user is host administrator.
    /// </summary>
    public bool IsHostAdmin
    {
        // int value 1
        get => this[0];

        set => this[0] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether is approved for posting.
    /// </summary>
    public bool IsApproved
    {
        // int value 2
        get => this[1];

        set => this[1] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether user is guest, i.e. not registered and logged in.
    /// </summary>
    public bool IsGuest
    {
        // int value 4
        get => this[2];

        set => this[2] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether user is excluded from the "Active Users" list on the forum pages.
    /// </summary>
    public bool IsActiveExcluded
    {
        // int value 16
        get => this[4];

        set => this[4] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether User is (soft) deleted.
    /// </summary>
    public bool IsDeleted
    {
        // int value 32
        get => this[5];

        set => this[5] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether a user profile/personal data was changed.
    /// The flag is set every time when a user profile changes.
    /// Used for portal integration.
    /// </summary>
    public bool IsDirty
    {
        // int value 64
        get => this[6];

        set => this[6] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether moderated.
    /// </summary>
    public bool Moderated
    {
        // int value 128
        get => this[7];

        set => this[7] = value;
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="newBitValue">
    /// The new bit value.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator UserFlags(int newBitValue)
    {
        return new UserFlags(newBitValue);
    }

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator UserFlags(Flags flags)
    {
        return new UserFlags(flags);
    }
}