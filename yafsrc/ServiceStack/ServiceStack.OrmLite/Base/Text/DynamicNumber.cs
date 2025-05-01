// ***********************************************************************
// <copyright file="DynamicNumber.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

using ServiceStack.OrmLite.Base.Text.Common;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Interface IDynamicNumber
/// </summary>
public interface IDynamicNumber
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    Type Type { get; }
    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    object ConvertFrom(object value);
    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool TryParse(string str, out object result);
    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    string ToString(object value);
    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <value>The default value.</value>
    object DefaultValue { get; }

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object add(object lhs, object rhs);
    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object sub(object lhs, object rhs);
    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object mul(object lhs, object rhs);
    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object div(object lhs, object rhs);
    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object mod(object lhs, object rhs);
    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object pow(object lhs, object rhs);
    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object log(object lhs, object rhs);
    /// <summary>
    /// Minimums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object min(object lhs, object rhs);
    /// <summary>
    /// Maximums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object max(object lhs, object rhs);
    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    int compareTo(object lhs, object rhs);

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object bitwiseAnd(object lhs, object rhs);
    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object bitwiseOr(object lhs, object rhs);
    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object bitwiseXOr(object lhs, object rhs);
    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object bitwiseLeftShift(object lhs, object rhs);
    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    object bitwiseRightShift(object lhs, object rhs);
}

/// <summary>
/// Class DynamicSByte.
/// Implements the <see cref="IDynamicNumber" />
/// </summary>
/// <seealso cref="IDynamicNumber" />
public class DynamicSByte : IDynamicNumber
{
    /// <summary>
    /// The instance
    /// </summary>
    public static DynamicSByte Instance { get; set; } = new();
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type => typeof(sbyte);

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.SByte.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte Convert(object value)
    {
        return System.Convert.ToSByte(this.ParseString(value) ?? value);
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object ConvertFrom(object value)
    {
        return this.ParseString(value)
               ?? System.Convert.ToSByte(value);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string str, out object result)
    {
        if (sbyte.TryParse(str, out var value))
        {
            result = value;
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public string ToString(object value)
    {
        return this.Convert(value).ToString();
    }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue => 0;

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object add(object lhs, object rhs)
    {
        return this.Convert(lhs) + this.Convert(rhs);
    }

    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object sub(object lhs, object rhs)
    {
        return this.Convert(lhs) - this.Convert(rhs);
    }

    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mul(object lhs, object rhs)
    {
        return this.Convert(lhs) * this.Convert(rhs);
    }

    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object div(object lhs, object rhs)
    {
        return this.Convert(lhs) / this.Convert(rhs);
    }

    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mod(object lhs, object rhs)
    {
        return this.Convert(lhs) % this.Convert(rhs);
    }

    /// <summary>
    /// Minimums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object min(object lhs, object rhs)
    {
        return Math.Min(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Maximums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object max(object lhs, object rhs)
    {
        return Math.Max(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object pow(object lhs, object rhs)
    {
        return Math.Pow(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object log(object lhs, object rhs)
    {
        return Math.Log(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    public int compareTo(object lhs, object rhs)
    {
        return this.Convert(lhs).CompareTo(this.Convert(rhs));
    }

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseAnd(object lhs, object rhs)
    {
        return this.Convert(lhs) & this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseOr(object lhs, object rhs)
    {
        return this.Convert(lhs) | this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseXOr(object lhs, object rhs)
    {
        return this.Convert(lhs) ^ this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseLeftShift(object lhs, object rhs)
    {
        return this.Convert(lhs) << this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseRightShift(object lhs, object rhs)
    {
        return this.Convert(lhs) >> this.Convert(rhs);
    }
}

/// <summary>
/// Class DynamicByte.
/// Implements the <see cref="IDynamicNumber" />
/// </summary>
/// <seealso cref="IDynamicNumber" />
public class DynamicByte : IDynamicNumber
{
    /// <summary>
    /// The instance
    /// </summary>
    public static DynamicByte Instance { get; } = new();
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type => typeof(byte);

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Byte.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Convert(object value)
    {
        return System.Convert.ToByte(this.ParseString(value) ?? value);
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object ConvertFrom(object value)
    {
        return this.ParseString(value)
               ?? System.Convert.ToByte(value);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string str, out object result)
    {
        if (byte.TryParse(str, out var value))
        {
            result = value;
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public string ToString(object value)
    {
        return this.Convert(value).ToString();
    }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue => 0;

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object add(object lhs, object rhs)
    {
        return this.Convert(lhs) + this.Convert(rhs);
    }

    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object sub(object lhs, object rhs)
    {
        return this.Convert(lhs) - this.Convert(rhs);
    }

    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mul(object lhs, object rhs)
    {
        return this.Convert(lhs) * this.Convert(rhs);
    }

    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object div(object lhs, object rhs)
    {
        return this.Convert(lhs) / this.Convert(rhs);
    }

    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mod(object lhs, object rhs)
    {
        return this.Convert(lhs) % this.Convert(rhs);
    }

    /// <summary>
    /// Minimums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object min(object lhs, object rhs)
    {
        return Math.Min(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Maximums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object max(object lhs, object rhs)
    {
        return Math.Max(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object pow(object lhs, object rhs)
    {
        return Math.Pow(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object log(object lhs, object rhs)
    {
        return Math.Log(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    public int compareTo(object lhs, object rhs)
    {
        return this.Convert(lhs).CompareTo(this.Convert(rhs));
    }

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseAnd(object lhs, object rhs)
    {
        return this.Convert(lhs) & this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseOr(object lhs, object rhs)
    {
        return this.Convert(lhs) | this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseXOr(object lhs, object rhs)
    {
        return this.Convert(lhs) ^ this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseLeftShift(object lhs, object rhs)
    {
        return this.Convert(lhs) << this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseRightShift(object lhs, object rhs)
    {
        return this.Convert(lhs) >> this.Convert(rhs);
    }
}

/// <summary>
/// Class DynamicShort.
/// Implements the <see cref="IDynamicNumber" />
/// </summary>
/// <seealso cref="IDynamicNumber" />
public class DynamicShort : IDynamicNumber
{
    /// <summary>
    /// The instance
    /// </summary>
    public static DynamicShort Instance { get; } = new();
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type => typeof(short);

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int16.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short Convert(object value)
    {
        return System.Convert.ToInt16(this.ParseString(value) ?? value);
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object ConvertFrom(object value)
    {
        return this.ParseString(value)
               ?? System.Convert.ToInt16(value);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string str, out object result)
    {
        if (short.TryParse(str, out var value))
        {
            result = value;
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public string ToString(object value)
    {
        return this.Convert(value).ToString();
    }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue => 0;

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object add(object lhs, object rhs)
    {
        return this.Convert(lhs) + this.Convert(rhs);
    }

    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object sub(object lhs, object rhs)
    {
        return this.Convert(lhs) - this.Convert(rhs);
    }

    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mul(object lhs, object rhs)
    {
        return this.Convert(lhs) * this.Convert(rhs);
    }

    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object div(object lhs, object rhs)
    {
        return this.Convert(lhs) / this.Convert(rhs);
    }

    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mod(object lhs, object rhs)
    {
        return this.Convert(lhs) % this.Convert(rhs);
    }

    /// <summary>
    /// Minimums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object min(object lhs, object rhs)
    {
        return Math.Min(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Maximums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object max(object lhs, object rhs)
    {
        return Math.Max(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object pow(object lhs, object rhs)
    {
        return Math.Pow(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object log(object lhs, object rhs)
    {
        return Math.Log(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    public int compareTo(object lhs, object rhs)
    {
        return this.Convert(lhs).CompareTo(this.Convert(rhs));
    }

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseAnd(object lhs, object rhs)
    {
        return this.Convert(lhs) & this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseOr(object lhs, object rhs)
    {
        return this.Convert(lhs) | this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseXOr(object lhs, object rhs)
    {
        return this.Convert(lhs) ^ this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseLeftShift(object lhs, object rhs)
    {
        return this.Convert(lhs) << this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseRightShift(object lhs, object rhs)
    {
        return this.Convert(lhs) >> this.Convert(rhs);
    }
}

/// <summary>
/// Class DynamicUShort.
/// Implements the <see cref="IDynamicNumber" />
/// </summary>
/// <seealso cref="IDynamicNumber" />
public class DynamicUShort : IDynamicNumber
{
    /// <summary>
    /// The instance
    /// </summary>
    public static DynamicUShort Instance { get; } = new();
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type => typeof(ushort);

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.UInt16.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort Convert(object value)
    {
        return System.Convert.ToUInt16(this.ParseString(value) ?? value);
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object ConvertFrom(object value)
    {
        return this.ParseString(value)
               ?? System.Convert.ToUInt16(value);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string str, out object result)
    {
        if (ushort.TryParse(str, out var value))
        {
            result = value;
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public string ToString(object value)
    {
        return this.Convert(value).ToString();
    }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue => 0;

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object add(object lhs, object rhs)
    {
        return this.Convert(lhs) + this.Convert(rhs);
    }

    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object sub(object lhs, object rhs)
    {
        return this.Convert(lhs) - this.Convert(rhs);
    }

    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mul(object lhs, object rhs)
    {
        return this.Convert(lhs) * this.Convert(rhs);
    }

    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object div(object lhs, object rhs)
    {
        return this.Convert(lhs) / this.Convert(rhs);
    }

    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mod(object lhs, object rhs)
    {
        return this.Convert(lhs) % this.Convert(rhs);
    }

    /// <summary>
    /// Minimums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object min(object lhs, object rhs)
    {
        return Math.Min(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Maximums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object max(object lhs, object rhs)
    {
        return Math.Max(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object pow(object lhs, object rhs)
    {
        return Math.Pow(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object log(object lhs, object rhs)
    {
        return Math.Log(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    public int compareTo(object lhs, object rhs)
    {
        return this.Convert(lhs).CompareTo(this.Convert(rhs));
    }

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseAnd(object lhs, object rhs)
    {
        return this.Convert(lhs) & this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseOr(object lhs, object rhs)
    {
        return this.Convert(lhs) | this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseXOr(object lhs, object rhs)
    {
        return this.Convert(lhs) ^ this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseLeftShift(object lhs, object rhs)
    {
        return this.Convert(lhs) << this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseRightShift(object lhs, object rhs)
    {
        return this.Convert(lhs) >> this.Convert(rhs);
    }
}

/// <summary>
/// Class DynamicInt.
/// Implements the <see cref="IDynamicNumber" />
/// </summary>
/// <seealso cref="IDynamicNumber" />
public class DynamicInt : IDynamicNumber
{
    /// <summary>
    /// The instance
    /// </summary>
    public static DynamicInt Instance { get; } = new();
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type => typeof(int);

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Convert(object value)
    {
        return System.Convert.ToInt32(this.ParseString(value) ?? value);
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object ConvertFrom(object value)
    {
        return this.ParseString(value)
               ?? System.Convert.ToInt32(value);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string str, out object result)
    {
        if (int.TryParse(str, out var value))
        {
            result = value;
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public string ToString(object value)
    {
        return this.Convert(value).ToString();
    }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue => 0;

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object add(object lhs, object rhs)
    {
        return this.Convert(lhs) + this.Convert(rhs);
    }

    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object sub(object lhs, object rhs)
    {
        return this.Convert(lhs) - this.Convert(rhs);
    }

    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mul(object lhs, object rhs)
    {
        return this.Convert(lhs) * this.Convert(rhs);
    }

    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object div(object lhs, object rhs)
    {
        return this.Convert(lhs) / this.Convert(rhs);
    }

    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mod(object lhs, object rhs)
    {
        return this.Convert(lhs) % this.Convert(rhs);
    }

    /// <summary>
    /// Minimums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object min(object lhs, object rhs)
    {
        return Math.Min(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Maximums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object max(object lhs, object rhs)
    {
        return Math.Max(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object pow(object lhs, object rhs)
    {
        return Math.Pow(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object log(object lhs, object rhs)
    {
        return Math.Log(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    public int compareTo(object lhs, object rhs)
    {
        return this.Convert(lhs).CompareTo(this.Convert(rhs));
    }

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseAnd(object lhs, object rhs)
    {
        return this.Convert(lhs) & this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseOr(object lhs, object rhs)
    {
        return this.Convert(lhs) | this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseXOr(object lhs, object rhs)
    {
        return this.Convert(lhs) ^ this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseLeftShift(object lhs, object rhs)
    {
        return this.Convert(lhs) << this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseRightShift(object lhs, object rhs)
    {
        return this.Convert(lhs) >> this.Convert(rhs);
    }
}

/// <summary>
/// Class DynamicUInt.
/// Implements the <see cref="IDynamicNumber" />
/// </summary>
/// <seealso cref="IDynamicNumber" />
public class DynamicUInt : IDynamicNumber
{
    /// <summary>
    /// The instance
    /// </summary>
    public static DynamicUInt Instance { get; } = new();
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type => typeof(uint);

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.UInt32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Convert(object value)
    {
        return System.Convert.ToUInt32(this.ParseString(value) ?? value);
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object ConvertFrom(object value)
    {
        return this.ParseString(value)
               ?? System.Convert.ToUInt32(value);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string str, out object result)
    {
        if (uint.TryParse(str, out var value))
        {
            result = value;
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public string ToString(object value)
    {
        return this.Convert(value).ToString();
    }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue => 0;

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object add(object lhs, object rhs)
    {
        return this.Convert(lhs) + this.Convert(rhs);
    }

    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object sub(object lhs, object rhs)
    {
        return this.Convert(lhs) - this.Convert(rhs);
    }

    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mul(object lhs, object rhs)
    {
        return this.Convert(lhs) * this.Convert(rhs);
    }

    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object div(object lhs, object rhs)
    {
        return this.Convert(lhs) / this.Convert(rhs);
    }

    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mod(object lhs, object rhs)
    {
        return this.Convert(lhs) % this.Convert(rhs);
    }

    /// <summary>
    /// Minimums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object min(object lhs, object rhs)
    {
        return Math.Min(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Maximums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object max(object lhs, object rhs)
    {
        return Math.Max(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object pow(object lhs, object rhs)
    {
        return Math.Pow(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object log(object lhs, object rhs)
    {
        return Math.Log(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    public int compareTo(object lhs, object rhs)
    {
        return this.Convert(lhs).CompareTo(this.Convert(rhs));
    }

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseAnd(object lhs, object rhs)
    {
        return this.Convert(lhs) & this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseOr(object lhs, object rhs)
    {
        return this.Convert(lhs) | this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseXOr(object lhs, object rhs)
    {
        return this.Convert(lhs) ^ this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseLeftShift(object lhs, object rhs)
    {
        return this.Convert(lhs) << (int)this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseRightShift(object lhs, object rhs)
    {
        return this.Convert(lhs) >> (int)this.Convert(rhs);
    }
}

/// <summary>
/// Class DynamicLong.
/// Implements the <see cref="IDynamicNumber" />
/// </summary>
/// <seealso cref="IDynamicNumber" />
public class DynamicLong : IDynamicNumber
{
    /// <summary>
    /// The instance
    /// </summary>
    public static DynamicLong Instance { get; } = new();
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type => typeof(long);

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int64.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Convert(object value)
    {
        return System.Convert.ToInt64(this.ParseString(value) ?? value);
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object ConvertFrom(object value)
    {
        return this.ParseString(value)
               ?? System.Convert.ToInt64(value);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string str, out object result)
    {
        if (long.TryParse(str, out var value))
        {
            result = value;
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public string ToString(object value)
    {
        return this.Convert(value).ToString();
    }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue => 0;

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object add(object lhs, object rhs)
    {
        return this.Convert(lhs) + this.Convert(rhs);
    }

    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object sub(object lhs, object rhs)
    {
        return this.Convert(lhs) - this.Convert(rhs);
    }

    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mul(object lhs, object rhs)
    {
        return this.Convert(lhs) * this.Convert(rhs);
    }

    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object div(object lhs, object rhs)
    {
        return this.Convert(lhs) / this.Convert(rhs);
    }

    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mod(object lhs, object rhs)
    {
        return this.Convert(lhs) % this.Convert(rhs);
    }

    /// <summary>
    /// Minimums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object min(object lhs, object rhs)
    {
        return Math.Min(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Maximums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object max(object lhs, object rhs)
    {
        return Math.Max(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object pow(object lhs, object rhs)
    {
        return Math.Pow(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object log(object lhs, object rhs)
    {
        return Math.Log(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    public int compareTo(object lhs, object rhs)
    {
        return this.Convert(lhs).CompareTo(this.Convert(rhs));
    }

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseAnd(object lhs, object rhs)
    {
        return this.Convert(lhs) & this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseOr(object lhs, object rhs)
    {
        return this.Convert(lhs) | this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseXOr(object lhs, object rhs)
    {
        return this.Convert(lhs) ^ this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseLeftShift(object lhs, object rhs)
    {
        return this.Convert(lhs) << (int)this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseRightShift(object lhs, object rhs)
    {
        return this.Convert(lhs) >> (int)this.Convert(rhs);
    }
}

/// <summary>
/// Class DynamicULong.
/// Implements the <see cref="IDynamicNumber" />
/// </summary>
/// <seealso cref="IDynamicNumber" />
public class DynamicULong : IDynamicNumber
{
    /// <summary>
    /// The instance
    /// </summary>
    public static DynamicULong Instance { get; } = new();
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type => typeof(ulong);

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.UInt64.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong Convert(object value)
    {
        return System.Convert.ToUInt64(this.ParseString(value) ?? value);
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object ConvertFrom(object value)
    {
        return this.ParseString(value)
               ?? System.Convert.ToUInt64(value);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string str, out object result)
    {
        if (ulong.TryParse(str, out var value))
        {
            result = value;
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public string ToString(object value)
    {
        return this.Convert(value).ToString();
    }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue => 0;

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object add(object lhs, object rhs)
    {
        return this.Convert(lhs) + this.Convert(rhs);
    }

    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object sub(object lhs, object rhs)
    {
        return this.Convert(lhs) - this.Convert(rhs);
    }

    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mul(object lhs, object rhs)
    {
        return this.Convert(lhs) * this.Convert(rhs);
    }

    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object div(object lhs, object rhs)
    {
        return this.Convert(lhs) / this.Convert(rhs);
    }

    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mod(object lhs, object rhs)
    {
        return this.Convert(lhs) % this.Convert(rhs);
    }

    /// <summary>
    /// Minimums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object min(object lhs, object rhs)
    {
        return Math.Min(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Maximums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object max(object lhs, object rhs)
    {
        return Math.Max(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object pow(object lhs, object rhs)
    {
        return Math.Pow(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object log(object lhs, object rhs)
    {
        return Math.Log(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    public int compareTo(object lhs, object rhs)
    {
        return this.Convert(lhs).CompareTo(this.Convert(rhs));
    }

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseAnd(object lhs, object rhs)
    {
        return this.Convert(lhs) & this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseOr(object lhs, object rhs)
    {
        return this.Convert(lhs) | this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseXOr(object lhs, object rhs)
    {
        return this.Convert(lhs) ^ this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseLeftShift(object lhs, object rhs)
    {
        return this.Convert(lhs) << (int)this.Convert(rhs);
    }

    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object bitwiseRightShift(object lhs, object rhs)
    {
        return this.Convert(lhs) >> (int)this.Convert(rhs);
    }
}

/// <summary>
/// Class DynamicFloat.
/// Implements the <see cref="IDynamicNumber" />
/// </summary>
/// <seealso cref="IDynamicNumber" />
public class DynamicFloat : IDynamicNumber
{
    /// <summary>
    /// The instance
    /// </summary>
    public static DynamicFloat Instance { get; } = new();
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type => typeof(float);

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Single.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Convert(object value)
    {
        return System.Convert.ToSingle(this.ParseString(value) ?? value);
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object ConvertFrom(object value)
    {
        return this.ParseString(value)
               ?? System.Convert.ToSingle(value);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string str, out object result)
    {
        if (str.AsSpan().TryParseFloat(out var value))
        {
            result = value;
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public string ToString(object value)
    {
        return this.Convert(value).ToString("r", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue => 0;

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object add(object lhs, object rhs)
    {
        return this.Convert(lhs) + this.Convert(rhs);
    }

    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object sub(object lhs, object rhs)
    {
        return this.Convert(lhs) - this.Convert(rhs);
    }

    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mul(object lhs, object rhs)
    {
        return this.Convert(lhs) * this.Convert(rhs);
    }

    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object div(object lhs, object rhs)
    {
        return this.Convert(lhs) / this.Convert(rhs);
    }

    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mod(object lhs, object rhs)
    {
        return this.Convert(lhs) % this.Convert(rhs);
    }

    /// <summary>
    /// Minimums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object min(object lhs, object rhs)
    {
        return Math.Min(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Maximums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object max(object lhs, object rhs)
    {
        return Math.Max(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object pow(object lhs, object rhs)
    {
        return Math.Pow(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object log(object lhs, object rhs)
    {
        return Math.Log(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    public int compareTo(object lhs, object rhs)
    {
        return this.Convert(lhs).CompareTo(this.Convert(rhs));
    }

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseAnd(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }

    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseOr(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }

    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseXOr(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }

    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseLeftShift(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }

    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseRightShift(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }
}

/// <summary>
/// Class DynamicDouble.
/// Implements the <see cref="IDynamicNumber" />
/// </summary>
/// <seealso cref="IDynamicNumber" />
public class DynamicDouble : IDynamicNumber
{
    /// <summary>
    /// The instance
    /// </summary>
    public static DynamicDouble Instance { get; } = new();
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type => typeof(double);

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Double.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Convert(object value)
    {
        return System.Convert.ToDouble(this.ParseString(value) ?? value);
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object ConvertFrom(object value)
    {
        return this.ParseString(value)
               ?? System.Convert.ToDouble(value);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string str, out object result)
    {
        if (str.AsSpan().TryParseDouble(out var value))
        {
            result = value;
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public string ToString(object value)
    {
        return this.Convert(value).ToString("r", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue => 0;

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object add(object lhs, object rhs)
    {
        return this.Convert(lhs) + this.Convert(rhs);
    }

    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object sub(object lhs, object rhs)
    {
        return this.Convert(lhs) - this.Convert(rhs);
    }

    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mul(object lhs, object rhs)
    {
        return this.Convert(lhs) * this.Convert(rhs);
    }

    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object div(object lhs, object rhs)
    {
        return this.Convert(lhs) / this.Convert(rhs);
    }

    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mod(object lhs, object rhs)
    {
        return this.Convert(lhs) % this.Convert(rhs);
    }

    /// <summary>
    /// Minimums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object min(object lhs, object rhs)
    {
        return Math.Min(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Maximums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object max(object lhs, object rhs)
    {
        return Math.Max(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object pow(object lhs, object rhs)
    {
        return Math.Pow(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object log(object lhs, object rhs)
    {
        return Math.Log(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    public int compareTo(object lhs, object rhs)
    {
        return this.Convert(lhs).CompareTo(this.Convert(rhs));
    }

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseAnd(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }

    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseOr(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }

    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseXOr(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }

    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseLeftShift(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }

    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseRightShift(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }
}

/// <summary>
/// Class DynamicDecimal.
/// Implements the <see cref="IDynamicNumber" />
/// </summary>
/// <seealso cref="IDynamicNumber" />
public class DynamicDecimal : IDynamicNumber
{
    /// <summary>
    /// The instance
    /// </summary>
    public static DynamicDecimal Instance { get; } = new();
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type => typeof(decimal);

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Decimal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal Convert(object value)
    {
        return System.Convert.ToDecimal(this.ParseString(value) ?? value);
    }

    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object ConvertFrom(object value)
    {
        return this.ParseString(value)
               ?? System.Convert.ToDecimal(value);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string str, out object result)
    {
        if (str.AsSpan().TryParseDecimal(out var value))
        {
            result = value;
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public string ToString(object value)
    {
        return this.Convert(value).ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue => 0;

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object add(object lhs, object rhs)
    {
        return this.Convert(lhs) + this.Convert(rhs);
    }

    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object sub(object lhs, object rhs)
    {
        return this.Convert(lhs) - this.Convert(rhs);
    }

    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mul(object lhs, object rhs)
    {
        return this.Convert(lhs) * this.Convert(rhs);
    }

    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object div(object lhs, object rhs)
    {
        return this.Convert(lhs) / this.Convert(rhs);
    }

    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object mod(object lhs, object rhs)
    {
        return this.Convert(lhs) % this.Convert(rhs);
    }

    /// <summary>
    /// Minimums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object min(object lhs, object rhs)
    {
        return Math.Min(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Maximums the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object max(object lhs, object rhs)
    {
        return Math.Max(this.Convert(lhs), this.Convert(rhs));
    }

    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object pow(object lhs, object rhs)
    {
        return Math.Pow((double)this.Convert(lhs), (double)this.Convert(rhs));
    }

    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public object log(object lhs, object rhs)
    {
        return Math.Log((double)this.Convert(lhs), (double)this.Convert(rhs));
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    public int compareTo(object lhs, object rhs)
    {
        return this.Convert(lhs).CompareTo(this.Convert(rhs));
    }

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseAnd(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }

    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseOr(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }

    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseXOr(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }

    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseLeftShift(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }

    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Bitwise operators only supported on integer types</exception>
    public object bitwiseRightShift(object lhs, object rhs)
    {
        throw new NotSupportedException("Bitwise operators only supported on integer types");
    }
}

/// <summary>
/// Class DynamicNumber.
/// </summary>
public static class DynamicNumber
{
    /// <summary>
    /// The rank numbers
    /// </summary>
    readonly static Dictionary<int, IDynamicNumber> RankNumbers = new() {
                                                                                {1, DynamicSByte.Instance},
                                                                                {2, DynamicByte.Instance},
                                                                                {3, DynamicShort.Instance},
                                                                                {4, DynamicUShort.Instance},
                                                                                {5, DynamicInt.Instance},
                                                                                {6, DynamicUInt.Instance},
                                                                                {7, DynamicLong.Instance},
                                                                                {8, DynamicULong.Instance},
                                                                                {9, DynamicFloat.Instance},
                                                                                {10, DynamicDouble.Instance},
                                                                                {11, DynamicDecimal.Instance}
    };

    /// <summary>
    /// Determines whether the specified type is number.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if the specified type is number; otherwise, <c>false</c>.</returns>
    public static bool IsNumber(Type type)
    {
        return TryGetRanking(type, out _);
    }

    /// <summary>
    /// Tries the get ranking.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="ranking">The ranking.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryGetRanking(Type type, out int ranking)
    {
        ranking = -1;
        switch (type.GetTypeCode())
        {
            case TypeCode.SByte:
                ranking = 1;
                break;
            case TypeCode.Byte:
                ranking = 2;
                break;
            case TypeCode.Int16:
                ranking = 3;
                break;
            case TypeCode.UInt16:
                ranking = 4;
                break;
            case TypeCode.Char:
            case TypeCode.Int32:
                ranking = 5;
                break;
            case TypeCode.UInt32:
                ranking = 6;
                break;
            case TypeCode.Int64:
                ranking = 7;
                break;
            case TypeCode.UInt64:
                ranking = 8;
                break;
            case TypeCode.Single:
                ranking = 9;
                break;
            case TypeCode.Double:
                ranking = 10;
                break;
            case TypeCode.Decimal:
                ranking = 11;
                break;
        }

        return ranking > 0;
    }

    /// <summary>
    /// Gets the number.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>IDynamicNumber.</returns>
    public static IDynamicNumber GetNumber(Type type)
    {
        if (!TryGetRanking(type, out var objIndex))
        {
            return null;
        }

        var maxNumber = RankNumbers[objIndex];
        return maxNumber;
    }

    /// <summary>
    /// Gets the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>IDynamicNumber.</returns>
    public static IDynamicNumber Get(object obj)
    {
        switch (obj)
        {
            case null:
            case string lhsString when !TryParse(lhsString, out obj):
                return null;
            default:
                return TryGetRanking(obj.GetType(), out var lhsRanking)
                           ? RankNumbers[lhsRanking]
                           : null;
        }
    }

    /// <summary>
    /// Gets the number.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>IDynamicNumber.</returns>
    public static IDynamicNumber GetNumber(object lhs, object rhs)
    {
        if (lhs == null || rhs == null)
        {
            return null;
        }

        if (lhs is string lhsString && !TryParse(lhsString, out lhs))
        {
            return null;
        }

        if (rhs is string rhsString && !TryParse(rhsString, out rhs))
        {
            return null;
        }

        if (!TryGetRanking(lhs.GetType(), out var lhsRanking) || !TryGetRanking(rhs.GetType(), out var rhsRanking))
        {
            return null;
        }

        var maxRanking = Math.Max(lhsRanking, rhsRanking);
        var maxNumber = RankNumbers[maxRanking];
        return maxNumber;
    }

    /// <summary>
    /// Asserts the numbers.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>IDynamicNumber.</returns>
    /// <exception cref="System.ArgumentException">Invalid numbers passed to {name}: " +
    ///                                         $"({lhs?.GetType().Name ?? "null"} '{lhs?.ToString().SubstringWithEllipsis(0, 100)}', " +
    ///                                         $"{rhs?.GetType().Name ?? "null"} '{rhs?.ToString().SubstringWithEllipsis(0, 100)}')</exception>
    public static IDynamicNumber AssertNumbers(string name, object lhs, object rhs)
    {
        var number = GetNumber(lhs, rhs);
        if (number == null)
        {
            throw new ArgumentException($"Invalid numbers passed to {name}: " +
                                        $"({lhs?.GetType().Name ?? "null"} '{lhs?.ToString().SubstringWithEllipsis(0, 100)}', " +
                                        $"{rhs?.GetType().Name ?? "null"} '{rhs?.ToString().SubstringWithEllipsis(0, 100)}')");
        }

        return number;
    }

    /// <summary>
    /// Adds the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Add(object lhs, object rhs)
    {
        return AssertNumbers(nameof(Add), lhs, rhs).add(lhs, rhs);
    }

    /// <summary>
    /// Subs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Sub(object lhs, object rhs)
    {
        return AssertNumbers(nameof(Subtract), lhs, rhs).sub(lhs, rhs);
    }

    /// <summary>
    /// Subtracts the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Subtract(object lhs, object rhs)
    {
        return AssertNumbers(nameof(Subtract), lhs, rhs).sub(lhs, rhs);
    }

    /// <summary>
    /// Muls the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Mul(object lhs, object rhs)
    {
        return AssertNumbers(nameof(Multiply), lhs, rhs).mul(lhs, rhs);
    }

    /// <summary>
    /// Multiplies the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Multiply(object lhs, object rhs)
    {
        return AssertNumbers(nameof(Multiply), lhs, rhs).mul(lhs, rhs);
    }

    /// <summary>
    /// Divs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Div(object lhs, object rhs)
    {
        return AssertNumbers(nameof(Divide), lhs, rhs).div(lhs, rhs);
    }

    /// <summary>
    /// Divides the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Divide(object lhs, object rhs)
    {
        return AssertNumbers(nameof(Divide), lhs, rhs).div(lhs, rhs);
    }

    /// <summary>
    /// Mods the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Mod(object lhs, object rhs)
    {
        return AssertNumbers(nameof(Mod), lhs, rhs).mod(lhs, rhs);
    }

    /// <summary>
    /// Determines the minimum of the parameters.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Min(object lhs, object rhs)
    {
        return AssertNumbers(nameof(Min), lhs, rhs).min(lhs, rhs);
    }

    /// <summary>
    /// Determines the maximum of the parameters.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Max(object lhs, object rhs)
    {
        return AssertNumbers(nameof(Max), lhs, rhs).max(lhs, rhs);
    }

    /// <summary>
    /// Pows the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pow(object lhs, object rhs)
    {
        return AssertNumbers(nameof(Pow), lhs, rhs).pow(lhs, rhs);
    }

    /// <summary>
    /// Logs the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Log(object lhs, object rhs)
    {
        return AssertNumbers(nameof(Log), lhs, rhs).log(lhs, rhs);
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Int32.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CompareTo(object lhs, object rhs)
    {
        return AssertNumbers(nameof(CompareTo), lhs, rhs).compareTo(lhs, rhs);
    }

    /// <summary>
    /// Bitwises the and.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object BitwiseAnd(object lhs, object rhs)
    {
        return AssertNumbers(nameof(BitwiseAnd), lhs, rhs).bitwiseAnd(lhs, rhs);
    }

    /// <summary>
    /// Bitwises the or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object BitwiseOr(object lhs, object rhs)
    {
        return AssertNumbers(nameof(BitwiseOr), lhs, rhs).bitwiseOr(lhs, rhs);
    }

    /// <summary>
    /// Bitwises the x or.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object BitwiseXOr(object lhs, object rhs)
    {
        return AssertNumbers(nameof(BitwiseXOr), lhs, rhs).bitwiseXOr(lhs, rhs);
    }

    /// <summary>
    /// Bitwises the left shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object BitwiseLeftShift(object lhs, object rhs)
    {
        return AssertNumbers(nameof(BitwiseLeftShift), lhs, rhs).bitwiseLeftShift(lhs, rhs);
    }

    /// <summary>
    /// Bitwises the right shift.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object BitwiseRightShift(object lhs, object rhs)
    {
        return AssertNumbers(nameof(BitwiseRightShift), lhs, rhs).bitwiseRightShift(lhs, rhs);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="strValue">The string value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryParse(string strValue, out object result)
    {
        if (JsConfig.TryParseIntoBestFit)
        {
            return TryParseIntoBestFit(strValue, out result);
        }

        result = null;
        if (!(strValue?.Length > 0))
        {
            return false;
        }

        if (strValue.Length == 1)
        {
            int singleDigit = strValue[0];
            if (singleDigit is >= '0' and <= '9')
            {
                result = singleDigit - 48; // 0
                return true;
            }
            return false;
        }

        var hasDecimal = strValue.IndexOf('.') >= 0;
        if (!hasDecimal)
        {
            if (int.TryParse(strValue, out var intValue))
            {
                result = intValue;
                return true;
            }
            if (long.TryParse(strValue, out var longValue))
            {
                result = longValue;
                return true;
            }
            if (ulong.TryParse(strValue, out var ulongValue))
            {
                result = ulongValue;
                return true;
            }
        }

        var spanValue = strValue.AsSpan();
        if (spanValue.TryParseDouble(out var doubleValue))
        {
            result = doubleValue;
            return true;
        }

        if (spanValue.TryParseDecimal(out var decimalValue))
        {
            result = decimalValue;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries the parse into best fit.
    /// </summary>
    /// <param name="strValue">The string value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryParseIntoBestFit(string strValue, out object result)
    {
        result = null;
        if (!(strValue?.Length > 0))
        {
            return false;
        }

        var segValue = strValue.AsSpan();
        result = segValue.ParseNumber(true);
        return result != null;
    }
}

/// <summary>
/// Class DynamicNumberExtensions.
/// </summary>
static internal class DynamicNumberExtensions
{
    /// <summary>
    /// Parses the string.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    static internal object ParseString(this IDynamicNumber number, object value)
    {
        if (value is string s)
        {
            return number.TryParse(s, out var x) ? x : null;
        }

        if (value is char c)
        {
            return number.TryParse(c.ToString(), out var x) ? x : null;
        }

        return null;
    }

}