// ***********************************************************************
// <copyright file="DefaultScripts.Math.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Runtime.CompilerServices;

namespace ServiceStack.Script
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Class DefaultScripts.
    /// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
    /// Implements the <see cref="ServiceStack.Script.IConfigureScriptContext" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptMethods" />
    /// <seealso cref="ServiceStack.Script.IConfigureScriptContext" />
    public partial class DefaultScripts
    {
        /// <summary>
        /// Adds the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Object.</returns>
        public object add(object lhs, object rhs) => DynamicNumber.Add(lhs, rhs);
        /// <summary>
        /// Subs the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Object.</returns>
        public object sub(object lhs, object rhs) => DynamicNumber.Subtract(lhs, rhs);
        /// <summary>
        /// Subtracts the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Object.</returns>
        public object subtract(object lhs, object rhs) => DynamicNumber.Subtract(lhs, rhs);
        /// <summary>
        /// Muls the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Object.</returns>
        public object mul(object lhs, object rhs) => DynamicNumber.Multiply(lhs, rhs);
        /// <summary>
        /// Multiplies the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Object.</returns>
        public object multiply(object lhs, object rhs) => DynamicNumber.Multiply(lhs, rhs);
        /// <summary>
        /// Divs the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Double.</returns>
        public double div(double lhs, double rhs) => lhs / rhs;
        /// <summary>
        /// Divides the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Double.</returns>
        public double divide(double lhs, double rhs) => lhs / rhs;

        /// <summary>
        /// Incrs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int64.</returns>
        public long incr(long value) => value + 1;
        /// <summary>
        /// Increments the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int64.</returns>
        public long increment(long value) => value + 1;
        /// <summary>
        /// Incrs the by.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="by">The by.</param>
        /// <returns>System.Int64.</returns>
        public long incrBy(long value, long by) => value + by;
        /// <summary>
        /// Increments the by.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="by">The by.</param>
        /// <returns>System.Int64.</returns>
        public long incrementBy(long value, long by) => value + by;
        /// <summary>
        /// Decrs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int64.</returns>
        public long decr(long value) => value - 1;
        /// <summary>
        /// Decrements the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int64.</returns>
        public long decrement(long value) => value - 1;
        /// <summary>
        /// Decrs the by.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="by">The by.</param>
        /// <returns>System.Int64.</returns>
        public long decrBy(long value, long by) => value - by;
        /// <summary>
        /// Decrements the by.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="by">The by.</param>
        /// <returns>System.Int64.</returns>
        public long decrementBy(long value, long by) => value - by;
        /// <summary>
        /// Mods the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>System.Int64.</returns>
        public long mod(long value, long divisor) => value % divisor;

        /// <summary>
        /// Pis this instance.
        /// </summary>
        /// <returns>System.Double.</returns>
        public double pi() => Math.PI;
        /// <summary>
        /// es this instance.
        /// </summary>
        /// <returns>System.Double.</returns>
        public double e() => Math.E;
        /// <summary>
        /// Floors the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double floor(double value) => Math.Floor(value);
        /// <summary>
        /// Ceilings the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double ceiling(double value) => Math.Ceiling(value);
        /// <summary>
        /// Abses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double abs(double value) => Math.Abs(value);
        /// <summary>
        /// Acoses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double acos(double value) => Math.Acos(value);
        /// <summary>
        /// Atans the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double atan(double value) => Math.Atan(value);
        /// <summary>
        /// Atan2s the specified y.
        /// </summary>
        /// <param name="y">The y.</param>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        public double atan2(double y, double x) => Math.Atan2(y, x);
        /// <summary>
        /// Coses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double cos(double value) => Math.Cos(value);
        /// <summary>
        /// Exps the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double exp(double value) => Math.Exp(value);
        /// <summary>
        /// Logs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double log(double value) => Math.Log(value);
        /// <summary>
        /// Logs the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="newBase">The new base.</param>
        /// <returns>System.Double.</returns>
        public double log(double a, double newBase) => Math.Log(a, newBase);
        /// <summary>
        /// Log2s the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double log2(double value) => Math.Log(value, 2);
        /// <summary>
        /// Log10s the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double log10(double value) => Math.Log10(value);
        /// <summary>
        /// Pows the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>System.Double.</returns>
        public double pow(double x, double y) => Math.Pow(x, y);
        /// <summary>
        /// Rounds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double round(double value) => Math.Round(value);
        /// <summary>
        /// Rounds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="decimals">The decimals.</param>
        /// <returns>System.Double.</returns>
        public double round(double value, int decimals) => Math.Round(value, decimals);
        /// <summary>
        /// Signs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        public int sign(double value) => Math.Sign(value);
        /// <summary>
        /// Sins the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double sin(double value) => Math.Sin(value);
        /// <summary>
        /// Sinhes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double sinh(double value) => Math.Sinh(value);
        /// <summary>
        /// SQRTs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double sqrt(double value) => Math.Sqrt(value);
        /// <summary>
        /// Tans the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double tan(double value) => Math.Tan(value);
        /// <summary>
        /// Tanhes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double tanh(double value) => Math.Tanh(value);
        /// <summary>
        /// Truncates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public double truncate(double value) => Math.Truncate(value);

        /// <summary>
        /// Ints the add.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Int32.</returns>
        public int intAdd(int lhs, int rhs) => lhs + rhs;
        /// <summary>
        /// Ints the sub.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Int32.</returns>
        public int intSub(int lhs, int rhs) => lhs - rhs;
        /// <summary>
        /// Ints the mul.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Int32.</returns>
        public int intMul(int lhs, int rhs) => lhs * rhs;
        /// <summary>
        /// Ints the div.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Int32.</returns>
        public int intDiv(int lhs, int rhs) => lhs / rhs;

        /// <summary>
        /// Longs the add.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Int64.</returns>
        public long longAdd(long lhs, long rhs) => lhs + rhs;
        /// <summary>
        /// Longs the sub.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Int64.</returns>
        public long longSub(long lhs, long rhs) => lhs - rhs;
        /// <summary>
        /// Longs the mul.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Int64.</returns>
        public long longMul(long lhs, long rhs) => lhs * rhs;
        /// <summary>
        /// Longs the div.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Int64.</returns>
        public long longDiv(long lhs, long rhs) => lhs / rhs;

        /// <summary>
        /// Doubles the add.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Double.</returns>
        public double doubleAdd(double lhs, double rhs) => lhs + rhs;
        /// <summary>
        /// Doubles the sub.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Double.</returns>
        public double doubleSub(double lhs, double rhs) => lhs - rhs;
        /// <summary>
        /// Doubles the mul.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Double.</returns>
        public double doubleMul(double lhs, double rhs) => lhs * rhs;
        /// <summary>
        /// Doubles the div.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Double.</returns>
        public double doubleDiv(double lhs, double rhs) => lhs / rhs;

        /// <summary>
        /// Decimals the add.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Decimal.</returns>
        public decimal decimalAdd(decimal lhs, decimal rhs) => lhs + rhs;
        /// <summary>
        /// Decimals the sub.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Decimal.</returns>
        public decimal decimalSub(decimal lhs, decimal rhs) => lhs - rhs;
        /// <summary>
        /// Decimals the mul.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Decimal.</returns>
        public decimal decimalMul(decimal lhs, decimal rhs) => lhs * rhs;
        /// <summary>
        /// Decimals the div.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Decimal.</returns>
        public decimal decimalDiv(decimal lhs, decimal rhs) => lhs / rhs;
    }
}