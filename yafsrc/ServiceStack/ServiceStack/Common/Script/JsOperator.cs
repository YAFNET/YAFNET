// ***********************************************************************
// <copyright file="JsOperator.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Script;

using ServiceStack.Text;

/// <summary>
/// Class JsOperator.
/// Implements the <see cref="ServiceStack.Script.JsToken" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsToken" />
public abstract class JsOperator : JsToken
{
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public abstract string Token { get; }
    /// <summary>
    /// Converts to rawstring.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string ToRawString() => Token;
    /// <summary>
    /// Evaluates the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(ScriptScopeContext scope) => this;
}

/// <summary>
/// Class JsBinaryOperator.
/// Implements the <see cref="ServiceStack.Script.JsOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsOperator" />
public abstract class JsBinaryOperator : JsOperator
{
    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public abstract object Evaluate(object lhs, object rhs);
}

/// <summary>
/// Class JsUnaryOperator.
/// Implements the <see cref="ServiceStack.Script.JsOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsOperator" />
public abstract class JsUnaryOperator : JsOperator
{
    /// <summary>
    /// Evaluates the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public abstract object Evaluate(object target);
}

/// <summary>
/// Class JsLogicOperator.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public abstract class JsLogicOperator : JsBinaryOperator
{
    /// <summary>
    /// Tests the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public abstract bool Test(object lhs, object rhs);
    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object lhs, object rhs) => Test(lhs, rhs);
}

/// <summary>
/// Class JsGreaterThan.
/// Implements the <see cref="ServiceStack.Script.JsLogicOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsLogicOperator" />
public class JsGreaterThan : JsLogicOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsGreaterThan Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsGreaterThan" /> class from being created.
    /// </summary>
    private JsGreaterThan() { }
    /// <summary>
    /// Tests the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool Test(object lhs, object rhs) => DefaultScripts.Instance.greaterThan(lhs, rhs);
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => ">";
}

/// <summary>
/// Class JsGreaterThanEqual.
/// Implements the <see cref="ServiceStack.Script.JsLogicOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsLogicOperator" />
public class JsGreaterThanEqual : JsLogicOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsGreaterThanEqual Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsGreaterThanEqual" /> class from being created.
    /// </summary>
    private JsGreaterThanEqual() { }
    /// <summary>
    /// Tests the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool Test(object lhs, object rhs) => DefaultScripts.Instance.greaterThanEqual(lhs, rhs);
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => ">=";
}

/// <summary>
/// Class JsLessThanEqual.
/// Implements the <see cref="ServiceStack.Script.JsLogicOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsLogicOperator" />
public class JsLessThanEqual : JsLogicOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsLessThanEqual Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsLessThanEqual" /> class from being created.
    /// </summary>
    private JsLessThanEqual() { }
    /// <summary>
    /// Tests the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool Test(object lhs, object rhs) => DefaultScripts.Instance.lessThanEqual(lhs, rhs);
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "<=";
}

/// <summary>
/// Class JsLessThan.
/// Implements the <see cref="ServiceStack.Script.JsLogicOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsLogicOperator" />
public class JsLessThan : JsLogicOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsLessThan Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsLessThan" /> class from being created.
    /// </summary>
    private JsLessThan() { }
    /// <summary>
    /// Tests the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool Test(object lhs, object rhs) => DefaultScripts.Instance.lessThan(lhs, rhs);
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "<";
}

/// <summary>
/// Class JsEquals.
/// Implements the <see cref="ServiceStack.Script.JsLogicOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsLogicOperator" />
public class JsEquals : JsLogicOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsEquals Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsEquals" /> class from being created.
    /// </summary>
    private JsEquals() { }
    /// <summary>
    /// Tests the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool Test(object lhs, object rhs) => DefaultScripts.Instance.equals(lhs, rhs);
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "==";
}

/// <summary>
/// Class JsNotEquals.
/// Implements the <see cref="ServiceStack.Script.JsLogicOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsLogicOperator" />
public class JsNotEquals : JsLogicOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsNotEquals Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsNotEquals" /> class from being created.
    /// </summary>
    private JsNotEquals() { }
    /// <summary>
    /// Tests the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool Test(object lhs, object rhs) => DefaultScripts.Instance.notEquals(lhs, rhs);
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "!=";
}

/// <summary>
/// Class JsStrictEquals.
/// Implements the <see cref="ServiceStack.Script.JsLogicOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsLogicOperator" />
public class JsStrictEquals : JsLogicOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsStrictEquals Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsStrictEquals" /> class from being created.
    /// </summary>
    private JsStrictEquals() { }
    /// <summary>
    /// Tests the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool Test(object lhs, object rhs) => DefaultScripts.Instance.equals(lhs, rhs);
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "===";
}

/// <summary>
/// Class JsStrictNotEquals.
/// Implements the <see cref="ServiceStack.Script.JsLogicOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsLogicOperator" />
public class JsStrictNotEquals : JsLogicOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsStrictNotEquals Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsStrictNotEquals" /> class from being created.
    /// </summary>
    private JsStrictNotEquals() { }
    /// <summary>
    /// Tests the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool Test(object lhs, object rhs) => DefaultScripts.Instance.notEquals(lhs, rhs);
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "!==";
}

/// <summary>
/// Class JsCoalescing.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public class JsCoalescing : JsBinaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsCoalescing Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsCoalescing" /> class from being created.
    /// </summary>
    private JsCoalescing() { }

    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object lhs, object rhs) =>
        DefaultScripts.isFalsy(lhs) ? rhs : lhs;

    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "??";
}

/// <summary>
/// Class JsOr.
/// Implements the <see cref="ServiceStack.Script.JsLogicOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsLogicOperator" />
public class JsOr : JsLogicOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsOr Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsOr" /> class from being created.
    /// </summary>
    private JsOr() { }

    /// <summary>
    /// Tests the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool Test(object lhs, object rhs) =>
        !DefaultScripts.isFalsy(lhs) || !DefaultScripts.isFalsy(rhs);

    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "||";
}

/// <summary>
/// Class JsAnd.
/// Implements the <see cref="ServiceStack.Script.JsLogicOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsLogicOperator" />
public class JsAnd : JsLogicOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsAnd Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsAnd" /> class from being created.
    /// </summary>
    private JsAnd() { }

    /// <summary>
    /// Tests the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool Test(object lhs, object rhs) =>
        !DefaultScripts.isFalsy(lhs) && !DefaultScripts.isFalsy(rhs);

    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "&&";
}

/// <summary>
/// Class JsNot.
/// Implements the <see cref="ServiceStack.Script.JsUnaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsUnaryOperator" />
public class JsNot : JsUnaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsNot Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsNot" /> class from being created.
    /// </summary>
    private JsNot() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "!";
    /// <summary>
    /// Evaluates the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object target) => DefaultScripts.isFalsy(target);
}

/// <summary>
/// Class JsBitwiseAnd.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public class JsBitwiseAnd : JsBinaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsBitwiseAnd Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsBitwiseAnd" /> class from being created.
    /// </summary>
    private JsBitwiseAnd() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "&";

    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object lhs, object rhs) =>
        DynamicNumber.BitwiseAnd(lhs, rhs);
}

/// <summary>
/// Class JsBitwiseOr.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public class JsBitwiseOr : JsBinaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsBitwiseOr Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsBitwiseOr" /> class from being created.
    /// </summary>
    private JsBitwiseOr() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "|";

    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object lhs, object rhs) =>
        DynamicNumber.BitwiseOr(lhs, rhs);
}

/// <summary>
/// Class JsBitwiseXOr.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public class JsBitwiseXOr : JsBinaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsBitwiseXOr Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsBitwiseXOr" /> class from being created.
    /// </summary>
    private JsBitwiseXOr() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "^";

    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object lhs, object rhs) =>
        DynamicNumber.BitwiseXOr(lhs, rhs);
}

/// <summary>
/// Class JsBitwiseNot.
/// Implements the <see cref="ServiceStack.Script.JsUnaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsUnaryOperator" />
public class JsBitwiseNot : JsUnaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsBitwiseNot Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsBitwiseNot" /> class from being created.
    /// </summary>
    private JsBitwiseNot() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "~";
    /// <summary>
    /// Evaluates the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object target) => DynamicNumber.BitwiseNot(target);
}

/// <summary>
/// Class JsBitwiseLeftShift.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public class JsBitwiseLeftShift : JsBinaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsBitwiseLeftShift Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsBitwiseLeftShift" /> class from being created.
    /// </summary>
    private JsBitwiseLeftShift() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "<<";

    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object lhs, object rhs) =>
        DynamicNumber.BitwiseLeftShift(lhs, rhs);
}

/// <summary>
/// Class JsBitwiseRightShift.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public class JsBitwiseRightShift : JsBinaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsBitwiseRightShift Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsBitwiseRightShift" /> class from being created.
    /// </summary>
    private JsBitwiseRightShift() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => ">>";

    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object lhs, object rhs) =>
        DynamicNumber.BitwiseRightShift(lhs, rhs);
}

/// <summary>
/// Class JsAddition.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public class JsAddition : JsBinaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsAddition Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsAddition" /> class from being created.
    /// </summary>
    private JsAddition() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "+";

    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object lhs, object rhs)
    {
        if (lhs is string || rhs is string)
        {
            var lhsString = lhs.ConvertTo<string>();
            var rhsString = rhs.ConvertTo<string>();
            return string.Concat(lhsString, rhsString);
        }

        return DynamicNumber.Add(lhs, rhs);
    }
}

/// <summary>
/// Class JsSubtraction.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public class JsSubtraction : JsBinaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsSubtraction Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsSubtraction" /> class from being created.
    /// </summary>
    private JsSubtraction() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "-";

    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object lhs, object rhs) =>
        DynamicNumber.Subtract(lhs, rhs);
}

/// <summary>
/// Class JsMultiplication.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public class JsMultiplication : JsBinaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsMultiplication Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsMultiplication" /> class from being created.
    /// </summary>
    private JsMultiplication() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "*";

    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object lhs, object rhs) =>
        DynamicNumber.Multiply(lhs, rhs);
}

/// <summary>
/// Class JsDivision.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public class JsDivision : JsBinaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsDivision Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsDivision" /> class from being created.
    /// </summary>
    private JsDivision() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "/";

    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object lhs, object rhs) =>
        DynamicNumber.Divide(lhs, rhs.ConvertTo<double>());
}

/// <summary>
/// Class JsMod.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public class JsMod : JsBinaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsMod Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsMod" /> class from being created.
    /// </summary>
    private JsMod() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "%";

    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object lhs, object rhs) =>
        DynamicNumber.Mod(lhs, rhs);
}

/// <summary>
/// Class JsAssignment.
/// Implements the <see cref="ServiceStack.Script.JsBinaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsBinaryOperator" />
public class JsAssignment : JsBinaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsAssignment Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsAssignment" /> class from being created.
    /// </summary>
    private JsAssignment() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "=";

    /// <summary>
    /// Evaluates the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override object Evaluate(object lhs, object rhs)
    {
        throw new System.NotImplementedException();
    }
}

/// <summary>
/// Class JsMinus.
/// Implements the <see cref="ServiceStack.Script.JsUnaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsUnaryOperator" />
public class JsMinus : JsUnaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsMinus Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsMinus" /> class from being created.
    /// </summary>
    private JsMinus() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "-";

    /// <summary>
    /// Evaluates the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object target) => target == null
                                                          ? 0
                                                          : DynamicNumber.Multiply(target, -1).ConvertTo(target.GetType());
}

/// <summary>
/// Class JsPlus.
/// Implements the <see cref="ServiceStack.Script.JsUnaryOperator" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsUnaryOperator" />
public class JsPlus : JsUnaryOperator
{
    /// <summary>
    /// The operator
    /// </summary>
    public static JsPlus Operator = new();
    /// <summary>
    /// Prevents a default instance of the <see cref="JsPlus" /> class from being created.
    /// </summary>
    private JsPlus() { }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public override string Token => "+";
    /// <summary>
    /// Evaluates the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(object target) => target ?? 0;
}