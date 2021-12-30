/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
    /// The Activity flags.
    /// </summary>
    [Serializable]
    public class ActivityFlags : FlagsBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityFlags"/> class.
        /// </summary>
        public ActivityFlags()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityFlags"/> class.
        /// </summary>
        /// <param name="flags">
        /// The flags.
        /// </param>
        public ActivityFlags(Flags flags)
            : this((int)flags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityFlags"/> class.
        /// </summary>
        /// <param name="bitValue">
        /// The bit value.
        /// </param>
        public ActivityFlags(object bitValue)
            : base((int)bitValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityFlags"/> class.
        /// </summary>
        /// <param name="bitValue">
        /// The bit value.
        /// </param>
        public ActivityFlags(int bitValue)
            : base(bitValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityFlags"/> class.
        /// </summary>
        /// <param name="bits">
        /// The bits.
        /// </param>
        public ActivityFlags(params bool[] bits)
            : base(bits)
        {
        }

        #endregion

        #region Flags Enumeration

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
            /// The created topic.
            /// </summary>
            CreatedTopic = 1,

            /// <summary>
            /// The created reply.
            /// </summary>
            CreatedReply = 8,

            /// <summary>
            /// The was mentioned.
            /// </summary>
            WasMentioned = 512,

            /// <summary>
            /// The received thanks.
            /// </summary>
            ReceivedThanks = 1024,

            /// <summary>
            /// The given thanks.
            /// </summary>
            GivenThanks = 2048,

            /// <summary>
            /// The was quoted.
            /// </summary>
            WasQuoted = 4096
        }

        #endregion

        #region Single Flags (can be 32 of them)

        /// <summary>
        /// Gets or sets a value indicating whether created topic.
        /// </summary>
        public virtual bool CreatedTopic
        {
            // int value 1
            get => this[0];

            set => this[0] = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether created reply.
        /// </summary>
        public virtual bool CreatedReply
        {
            // int value 8
            get => this[3];

            set => this[3] = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether was mentioned.
        /// </summary>
        public virtual bool WasMentioned
        {
            // int value 512
            get => this[9];

            set => this[9] = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether received thanks.
        /// </summary>
        public virtual bool ReceivedThanks
        {
            // int value 1024
            get => this[10];

            set => this[10] = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether given thanks.
        /// </summary>
        public virtual bool GivenThanks
        {
            // 2048
            get => this[11];

            set => this[11] = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether received thanks.
        /// </summary>
        public virtual bool WasQuoted
        {
            // int value 4096
            get => this[12];

            set => this[12] = value;
        }

        #endregion
    }
}