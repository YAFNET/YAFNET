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
/// Abstract class as a foundation for various flags implementations
/// </summary>
[Serializable]
public abstract class FlagsBase
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "FlagsBase" /> class.
    ///   Creates new instance with all bits set to false (integer 0).
    /// </summary>
    protected FlagsBase()
        : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlagsBase"/> class.
    ///   Creates new instance and initialize it with value of bitValue parameter.
    /// </summary>
    /// <param name="bitValue">
    ///     Initialize integer value.
    ///     Initialize integer value.
    /// </param>
    /// <summary>
    /// Initializes a new instance of the <see cref="FlagsBase"/> class.
    ///   Creates new instance and initialize it with value of bitValue parameter.
    /// </summary>
    protected FlagsBase(int bitValue)
    {
        this.BitValue = bitValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlagsBase"/> class.
    ///   Creates new instance with bits set according to parameter array.
    /// </summary>
    /// <param name="bits">
    /// Boolean values to initialize class with. If their number is lower than 32, remaining bits are set to false. If more than 32 values is specified, excess values are ignored.
    /// </param>
    protected FlagsBase(params bool[] bits)
        : this(0)
    {
        // process up to 32 parameters
        for (var i = 0; i < Math.Min(bits.Length, 31); i++)
        {
            // set this bit
            this[i] = bits[i];
        }
    }

    /// <summary>
    ///   Gets or sets integer value of flags.
    /// </summary>
    public int BitValue { get; set; }

    /// <summary>
    ///   Gets or sets bit at position specified by index.
    /// </summary>
    /// <param name = "index">Zero-based index of bit to get or set.</param>
    /// <returns>Boolean value indicating whether bit at position specified by index is set or not.</returns>
    public bool this[int index]
    {
        get => GetBitAsBool(this.BitValue, index);

        set => this.BitValue = SetBitFromBool(this.BitValue, index, value);
    }

    /// <summary>
    /// Gets boolean indicating whether bit on bitShift position in bitValue integer is set or not.
    /// </summary>
    /// <param name="bitValue">
    /// Integer value.
    /// </param>
    /// <param name="bitShift">
    /// Zero-based position of bit to get.
    /// </param>
    /// <returns>
    /// Returns boolean indicating whether bit at bitShift position is set or not.
    /// </returns>
    public static bool GetBitAsBool(int bitValue, int bitShift)
    {
        if (bitShift > 63)
        {
            bitShift %= 63;
        }

        return ((bitValue >> bitShift) & 0x00000001) == 1;
    }

    /// <summary>
    /// Sets or un-sets bit of bitValue integer at position specified by bitShift, depending on value parameter.
    /// </summary>
    /// <param name="bitValue">
    /// Integer value.
    /// </param>
    /// <param name="bitShift">
    /// Zero-based position of bit to set.
    /// </param>
    /// <param name="value">
    /// New boolean value of bit.
    /// </param>
    /// <returns>
    /// Returns new integer value with bit at position specified by bitShift parameter set to value.
    /// </returns>
    public static int SetBitFromBool(int bitValue, int bitShift, bool value)
    {
        if (bitShift > 63)
        {
            bitShift %= 63;
        }

        if (GetBitAsBool(bitValue, bitShift) == value)
        {
            return bitValue;
        }

        // toggle that value using XOR
        var tV = 0x00000001 << bitShift;
        bitValue ^= tV;

        return bitValue;
    }

    /// <summary>
    /// Converts a Flag Enum to the associated index value.
    /// </summary>
    /// <param name="theEnum">
    /// The Enumerator
    /// </param>
    /// <returns>
    /// The enum to index.
    /// </returns>
    public int EnumToIndex(Enum theEnum)
    {
        return Convert.ToInt32(Math.Sqrt(Convert.ToInt32(theEnum))) - 1;
    }
}