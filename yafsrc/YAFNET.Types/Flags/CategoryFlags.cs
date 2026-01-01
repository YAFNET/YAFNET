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
/// The category flags.
/// </summary>
public class CategoryFlags : FlagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryFlags"/> class.
    /// </summary>
    /// <param name="bitValue">
    /// The bit value.
    /// </param>
    public CategoryFlags(int bitValue)
        : base(bitValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryFlags"/> class.
    /// </summary>
    public CategoryFlags()
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
        /// The is active.
        /// </summary>
        IsActive = 1

        /* for future use
          XXXXXXXX = 2,
          XXXXXXXX = 4,
          XXXXXXXX = 8,
          XXXXXXXX = 16,
          XXXXXXXX = 32,
          XXXXXXXX = 64,
          XXXXXXXX = 128,
          XXXXXXXX = 256,
          xxxxxxxx =  512,
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
    /// Gets or sets a value indicating whether the Category is Active.
    /// </summary>
    public bool IsActive
    {
        // int value 1
        get => this[0];

        set => this[0] = value;
    }
}