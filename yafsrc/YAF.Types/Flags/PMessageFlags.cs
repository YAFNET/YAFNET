/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Types.Flags
{
    using System;

    /// <summary>
    /// The private message flags.
    /// </summary>
    [Serializable]
    public class PMessageFlags : FlagsBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PMessageFlags"/> class.
        /// </summary>
        /// <param name="flags">
        /// The flags.
        /// </param>
        public PMessageFlags(Flags flags)
            : this((int)flags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PMessageFlags"/> class.
        /// </summary>
        /// <param name="bitValue">
        /// The bit value.
        /// </param>
        public PMessageFlags(object bitValue)
            : this((int)bitValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PMessageFlags"/> class.
        /// </summary>
        /// <param name="bitValue">
        /// The bit value.
        /// </param>
        public PMessageFlags(int bitValue)
            : base(bitValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PMessageFlags"/> class.
        /// </summary>
        /// <param name="bits">
        /// The bits.
        /// </param>
        public PMessageFlags(params bool[] bits)
            : base(bits)
        {
        }

        #endregion

        #region Flags Enumeration

        /// <summary>
        /// Use for bit comparisons
        /// </summary>
        [Flags]
        public enum Flags : int
        {
            /// <summary>
            /// The is read.
            /// </summary>
            IsRead = 1,

            /// <summary>
            /// The is in outbox.
            /// </summary>
            IsInOutbox = 2,

            /// <summary>
            /// The is archived.
            /// </summary>
            IsArchived = 4,

            /// <summary>
            /// The is deleted.
            /// </summary>
            IsDeleted = 8
        }

        #endregion

        #region Single Flags (can be 32 of them)

        /// <summary>
        /// Gets or sets a value indicating whether is read.
        /// </summary>
        public bool IsRead
        {
            // int value 1
            get => this[0];

            set => this[0] = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is in outbox.
        /// </summary>
        public bool IsInOutbox
        {
            // int value 2
            get => this[1];

            set => this[1] = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is archived.
        /// </summary>
        public bool IsArchived
        {
            // int value 4
            get => this[2];

            set => this[2] = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is deleted.
        /// </summary>
        public virtual bool IsDeleted
        {
            // int value 8
            get => this[3];

            set => this[3] = value;
        }

        #endregion
    }
}