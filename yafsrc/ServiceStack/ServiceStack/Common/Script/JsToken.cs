// ***********************************************************************
// <copyright file="JsToken.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************


namespace ServiceStack.Script
{

    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using ServiceStack.Text;
    using ServiceStack.Text.Json;

#if NET48
    using ServiceStack.Text.Extensions;
#endif

    /// <summary>
    /// Class JsToken.
    /// Implements the <see cref="ServiceStack.IRawString" />
    /// </summary>
    /// <seealso cref="ServiceStack.IRawString" />
    public abstract class JsToken : IRawString
    {
        /// <summary>
        /// Converts to rawstring.
        /// </summary>
        /// <returns>System.String.</returns>
        public abstract string ToRawString();

        /// <summary>
        /// Jsons the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public string JsonValue(object value)
        {
            if (value == null || value == JsNull.Value)
                return "null";
            if (value is JsToken jt)
                return jt.ToRawString();
            if (value is string s)
                return s.EncodeJson();
            return value.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => ToRawString();

        /// <summary>
        /// Evaluates the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.Object.</returns>
        public abstract object Evaluate(ScriptScopeContext scope);

        /// <summary>
        /// Unwraps the value.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Object.</returns>
        public static object UnwrapValue(JsToken token)
        {
            if (token is JsLiteral literal)
                return literal.Value;
            return null;
        }
    }

    /// <summary>
    /// Class JsNull.
    /// </summary>
    public static class JsNull
    {
        /// <summary>
        /// The string
        /// </summary>
        public const string String = "null";
        /// <summary>
        /// The value
        /// </summary>
        public static JsLiteral Value = new JsLiteral(null);
    }

    /// <summary>
    /// Class JsTokenUtils.
    /// </summary>
    public static class JsTokenUtils
    {
        /// <summary>
        /// The valid numeric chars
        /// </summary>
        private static readonly byte[] ValidNumericChars;
        /// <summary>
        /// The valid variable name chars
        /// </summary>
        private static readonly byte[] ValidVarNameChars;
        /// <summary>
        /// The operator chars
        /// </summary>
        private static readonly byte[] OperatorChars;
        /// <summary>
        /// The expression terminator chars
        /// </summary>
        private static readonly byte[] ExpressionTerminatorChars;
        /// <summary>
        /// Creates new lineutf8.
        /// </summary>
        public static readonly byte[] NewLineUtf8;

        /// <summary>
        /// The true
        /// </summary>
        private const byte True = 1;

        /// <summary>
        /// Initializes static members of the <see cref="JsTokenUtils"/> class.
        /// </summary>
        static JsTokenUtils()
        {
            NewLineUtf8 = new byte[] { 10 }; // UTF8.GetBytes("\n");
            var n = new byte['e' + 1];
            n['0'] = n['1'] = n['2'] = n['3'] = n['4'] = n['5'] = n['6'] = n['7'] = n['8'] = n['9'] = n['.'] = True;
            ValidNumericChars = n;

            var o = new byte['~' + 1];
            o['<'] = o['>'] = o['='] = o['!'] = o['+'] = o['-'] = o['*'] = o['/'] = o['%'] = o['|'] = o['&'] = o['^'] = o['~'] = o['?'] = True;
            OperatorChars = o;

            var e = new byte['}' + 1];
            e[')'] = e['}'] = e[';'] = e[','] = e[']'] = e[':'] = True;
            ExpressionTerminatorChars = e;

            var a = new byte['z' + 1];
            for (var i = (int)'$'; i < a.Length; i++)
            {
                if (i >= 'A' && i <= 'Z' || i >= 'a' && i <= 'z' || i >= '0' && i <= '9' || i == '_' || i == '$')
                    a[i] = True;
            }
            ValidVarNameChars = a;
        }

        /// <summary>
        /// The operator precedence
        /// </summary>
        public static readonly Dictionary<string, int> OperatorPrecedence = new Dictionary<string, int> {
            {")", 0},
            {";", 0},
            {",", 0},
            {"=", 0},
            {"]", 0},
            {"??", 1},
            {"||", 1},
            {"&&", 2},
            {"|", 3},
            {"^", 4},
            {"&", 5},
            {"==", 6},
            {"!=", 6},
            {"===", 6},
            {"!==", 6},
            {"<", 7},
            {">", 7},
            {"<=", 7},
            {">=", 7},
            {"<<", 8},
            {">>", 8},
            {">>>", 8},
            {"+", 9},
            {"-", 9},
            {"*", 11},
            {"/", 11},
            {"%", 11},
        };

        /// <summary>
        /// Gets the binary precedence.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBinaryPrecedence(string token) => OperatorPrecedence.TryGetValue(token, out var precedence) ? precedence : 0;

        /// <summary>
        /// Determines whether [is numeric character] [the specified c].
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if [is numeric character] [the specified c]; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumericChar(this char c) => c < ValidNumericChars.Length && ValidNumericChars[c] == True;

        /// <summary>
        /// Determines whether [is valid variable name character] [the specified c].
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if [is valid variable name character] [the specified c]; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidVarNameChar(this char c) => c < ValidVarNameChars.Length && ValidVarNameChars[c] == True;

        /// <summary>
        /// Determines whether [is operator character] [the specified c].
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if [is operator character] [the specified c]; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOperatorChar(this char c) => c < OperatorChars.Length && OperatorChars[c] == True;

        /// <summary>
        /// Determines whether [is expression terminator character] [the specified c].
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if [is expression terminator character] [the specified c]; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsExpressionTerminatorChar(this char c) => c < ExpressionTerminatorChars.Length && ExpressionTerminatorChars[c] == True;

        /// <summary>
        /// Gets the unary operator.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>JsUnaryOperator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JsUnaryOperator GetUnaryOperator(this char c)
        {
            switch (c)
            {
                case '-':
                    return JsMinus.Operator;
                case '+':
                    return JsPlus.Operator;
                case '!':
                    return JsNot.Operator;
                case '~':
                    return JsBitwiseNot.Operator;
            }

            return null;
        }

        /// <summary>
        /// Firsts the character equals.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FirstCharEquals(this ReadOnlySpan<char> literal, char c) =>
            !literal.IsNullOrEmpty() && literal[0] == c;

        /// <summary>
        /// Firsts the character equals.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FirstCharEquals(this string literal, char c) =>
            !string.IsNullOrEmpty(literal) && literal[0] == c;

        /// <summary>
        /// Safes the get character.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Char.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char SafeGetChar(this ReadOnlySpan<char> literal, int index) =>
            index >= 0 && index < literal.Length ? literal[index] : default(char);

        /// <summary>
        /// Counts the preceding occurrences.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="index">The index.</param>
        /// <param name="c">The c.</param>
        /// <returns>System.Int32.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountPrecedingOccurrences(this ReadOnlySpan<char> literal, int index, char c)
        {
            var total = 0;
            while (index > 0)
            {
                if (!literal.SafeCharEquals(index, c))
                    break;

                total++;
                index--;
            }
            return total;
        }

        /// <summary>
        /// Safes the character equals.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="index">The index.</param>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SafeCharEquals(this ReadOnlySpan<char> literal, int index, char c) =>
            index >= 0 && index < literal.Length && literal[index] == c;


        /// <summary>
        /// Advances the past pipe operator.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Unix Pipe syntax is disallowed, use JS Pipeline Operator syntax '|>' or set ScriptConfig.AllowUnixPipeSyntax=true;</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> AdvancePastPipeOperator(this ReadOnlySpan<char> literal) =>
            literal.SafeCharEquals(1, '>')
                ? literal.Advance(2) // support new JS |> operator
                : ScriptConfig.AllowUnixPipeSyntax ? literal.Advance(1)
                    : throw new SyntaxErrorException("Unix Pipe syntax is disallowed, use JS Pipeline Operator syntax '|>' or set ScriptConfig.AllowUnixPipeSyntax=true; ");

        /// <summary>
        /// Safes the get character.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Char.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char SafeGetChar(this ReadOnlyMemory<char> literal, int index) => literal.Span.SafeGetChar(index);

        /// <summary>
        /// Determines whether the specified c is end.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if the specified c is end; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnd(this char c) => c == default(char);

        /// <summary>
        /// Chops the specified c.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="c">The c.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory<char> Chop(this ReadOnlyMemory<char> literal, char c) =>
            literal.IsEmpty || literal.Span[literal.Length - 1] != c
                ? literal
                : literal.Slice(0, literal.Length - 1);

        /// <summary>
        /// Chops the specified c.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="c">The c.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Chop(this ReadOnlySpan<char> literal, char c) =>
            literal.IsEmpty || literal[literal.Length - 1] != c
                ? literal
                : literal.Slice(0, literal.Length - 1);

        /// <summary>
        /// Chops the new line.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory<char> ChopNewLine(this ReadOnlyMemory<char> literal)
        {
            var lastChar = literal.SafeGetChar(literal.Length - 1);
            if (lastChar == '\r' || lastChar == '\n')
            {
                return literal.Span.SafeCharEquals(literal.Length - 2, '\r')
                    ? literal.Slice(0, literal.Length - 2)
                    : literal.Slice(0, literal.Length - 1);
            }
            return literal;
        }

        // Remove `Js` prefix
        /// <summary>
        /// Converts to jsasttype.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        public static string ToJsAstType(this Type type) => type.Name.Substring(2);

        /// <summary>
        /// Converts to jsast.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public static Dictionary<string, object> ToJsAst(this JsToken token) => token is JsExpression expression
            ? expression.ToJsAst()
            : throw new NotSupportedException(token.GetType().Name + " is not a JsExpression");

        /// <summary>
        /// Converts to jsaststring.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.String.</returns>
        public static string ToJsAstString(this JsToken token)
        {
            using (JsConfig.With(new Config { IncludeNullValuesInDictionaries = true }))
            {
                return token.ToJsAst().ToJson().IndentJson();
            }
        }

        /// <summary>
        /// Debugs the first character.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns>System.String.</returns>
        internal static string DebugFirstChar(this ReadOnlySpan<char> literal) => literal.IsNullOrEmpty()
            ? "<end>"
            : $"'{literal[0]}'";

        /// <summary>
        /// Debugs the first character.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns>System.String.</returns>
        internal static string DebugFirstChar(this ReadOnlyMemory<char> literal) => literal.Span.DebugFirstChar();

        /// <summary>
        /// Debugs the character.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>System.String.</returns>
        internal static string DebugChar(this char c) => c == 0 ? "'<end>'" : $"'{c}'";

        /// <summary>
        /// Debugs the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.String.</returns>
        internal static string DebugToken(this JsToken token) => $"'{token}'";

        /// <summary>
        /// Debugs the literal.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns>System.String.</returns>
        internal static string DebugLiteral(this ReadOnlySpan<char> literal) => $"'{literal.SubstringWithEllipsis(0, 50)}'";

        /// <summary>
        /// Debugs the literal.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns>System.String.</returns>
        internal static string DebugLiteral(this ReadOnlyMemory<char> literal) => $"'{literal.Span.SubstringWithEllipsis(0, 50)}'";

        /// <summary>
        /// Cooks the raw string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="quoteChar">The quote character.</param>
        /// <returns>System.String.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string CookRawString(this ReadOnlySpan<char> str, char quoteChar) =>
            JsonTypeSerializer.UnescapeJsString(str, quoteChar).Value() ?? "";

        /// <summary>
        /// Trims the first new line.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ReadOnlyMemory<char> TrimFirstNewLine(this ReadOnlyMemory<char> literal) => literal.StartsWith("\r\n")
            ? literal.Advance(2)
            : literal.StartsWith("\n") ? literal.Advance(1) : literal;

        /// <summary>
        /// Evaluates the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Object.</returns>
        public static object Evaluate(this JsToken token) => token.Evaluate(JS.CreateScope());

        /// <summary>
        /// Evaluates to bool.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="scope">The scope.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool EvaluateToBool(this JsToken token, ScriptScopeContext scope)
        {
            var ret = token.Evaluate(scope);
            if (ret is bool b)
                return b;

            return !DefaultScripts.isFalsy(ret);
        }

        /// <summary>
        /// Evaluate if result can be async, if so converts async result to Task&lt;object&gt; otherwise wraps result in a Task
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        public static async Task<bool> EvaluateToBoolAsync(this JsToken token, ScriptScopeContext scope)
        {
            var ret = await token.EvaluateAsync(scope);
            if (ret is bool b)
                return b;

            return !DefaultScripts.isFalsy(ret);
        }

        /// <summary>
        /// Evaluate if result can be async, if so converts async result to Task&lt;object&gt; otherwise wraps result in a Task
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="result">if set to <c>true</c> [result].</param>
        /// <param name="asyncResult">The asynchronous result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool EvaluateToBool(this JsToken token, ScriptScopeContext scope, out bool? result, out Task<bool> asyncResult)
        {
            if (token.Evaluate(scope, out var oResult, out var oAsyncResult))
            {
                result = oResult is bool b ? b : !DefaultScripts.isFalsy(oResult);
                asyncResult = null;
                return true;
            }

            result = null;

            var tcs = new TaskCompletionSource<bool>();
            oAsyncResult.ContinueWith(t => tcs.SetResult(!DefaultScripts.isFalsy(t.Result)), TaskContinuationOptions.OnlyOnRanToCompletion);
            oAsyncResult.ContinueWith(t => tcs.SetException(t.Exception.InnerExceptions), TaskContinuationOptions.OnlyOnFaulted);
            oAsyncResult.ContinueWith(t => tcs.SetCanceled(), TaskContinuationOptions.OnlyOnCanceled);
            asyncResult = tcs.Task;

            return false;
        }

        /// <summary>
        /// Evaluate if result can be async, if so converts async result to Task&lt;object&gt; otherwise wraps result in a Task
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public static Task<object> EvaluateAsync(this JsToken token, ScriptScopeContext scope)
        {
            var result = token.Evaluate(scope);
            if (result is Task<object> taskObj)
                return taskObj;
            if (result is Task task)
                return task.GetResult().InTask();
            return result.InTask();
        }

        /// <summary>
        /// Evaluate then set asyncResult if Result was async, otherwise set result.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="result">The result.</param>
        /// <param name="asyncResult">The asynchronous result.</param>
        /// <returns>true if result was synchronous otherwise false</returns>
        public static bool Evaluate(this JsToken token, ScriptScopeContext scope, out object result, out Task<object> asyncResult)
        {
            result = token.Evaluate(scope);
            if (result is Task<object> taskObj)
            {
                asyncResult = taskObj;
                result = null;
            }
            else if (result is Task task)
            {
                asyncResult = task.GetResult().InTask();
                result = null;
            }
            else
            {
                asyncResult = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the js token.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="token">The token.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        public static ReadOnlySpan<char> ParseJsToken(this ReadOnlySpan<char> literal, out JsToken token) => ParseJsToken(literal, out token, false);
        /// <summary>
        /// Parses the js token.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="token">The token.</param>
        /// <param name="filterExpression">if set to <c>true</c> [filter expression].</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected ')' but instead found {literal.DebugFirstChar()} near: {literal.DebugLiteral()}</exception>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected identifier but was instead '{arg.DebugToken()}', near: {literal.DebugLiteral()}</exception>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected ',' or ')' but was instead '{literal.DebugFirstChar()}', near: {literal.DebugLiteral()}</exception>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Unterminated string literal: {literal.ToString()}</exception>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException"></exception>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Unterminated object literal near: {literal.DebugLiteral()}</exception>
        public static ReadOnlySpan<char> ParseJsToken(this ReadOnlySpan<char> literal, out JsToken token, bool filterExpression)
        {
            literal = literal.AdvancePastWhitespace();

            if (literal.IsNullOrEmpty())
            {
                token = null;
                return literal;
            }

            var c = literal[0];
            if (c == '(')
            {
                literal = literal.Advance(1);
                literal = literal.ParseJsExpression(out var bracketsExpr);
                literal = literal.AdvancePastWhitespace();

                if (literal.FirstCharEquals(')'))
                {
                    literal = literal.Advance(1);
                    token = bracketsExpr;
                    return literal;
                }

                if (!literal.FirstCharEquals(',') || bracketsExpr is not JsIdentifier param1)
                    throw new SyntaxErrorException($"Expected ')' but instead found {literal.DebugFirstChar()} near: {literal.DebugLiteral()}");

                literal = literal.Advance(1);
                var args = new List<JsIdentifier> { param1, };
                while (true)
                {
                    literal = literal.AdvancePastWhitespace();
                    literal = literal.ParseIdentifier(out var arg);
                    if (arg is not JsIdentifier param)
                        throw new SyntaxErrorException($"Expected identifier but was instead '{arg.DebugToken()}', near: {literal.DebugLiteral()}");

                    args.Add(param);

                    literal = literal.AdvancePastWhitespace();

                    if (literal.FirstCharEquals(')'))
                        break;

                    if (!literal.FirstCharEquals(','))
                        throw new SyntaxErrorException($"Expected ',' or ')' but was instead '{literal.DebugFirstChar()}', near: {literal.DebugLiteral()}");

                    literal = literal.Advance(1);
                }

                literal = literal.Advance(1);
                literal = literal.ParseArrowExpressionBody(args.ToArray(), out var expr);
                token = expr;
                return literal;
            }

            token = null;
            c = (char)0;

            if (literal.IsNullOrEmpty())
                return default;

            var i = 0;
            literal = literal.AdvancePastWhitespace();

            var firstChar = literal[0];
            if (firstChar == '\'' || firstChar == '"' || firstChar == '`' || firstChar == '′')
            {
                var quoteChar = firstChar;
                i = literal.IndexOfQuotedString(quoteChar, out var hasEscapeChar);
                if (i == -1)
                    throw new SyntaxErrorException($"Unterminated string literal: {literal.ToString()}");

                var rawString = literal.Slice(1, i - 1);

                if (quoteChar == '`')
                {
                    token = ParseJsTemplateLiteral(rawString);
                }
                else if (hasEscapeChar)
                {
                    if (quoteChar == '′')
                    {
                        //All other quoted strings use unescaped strings
                        var sb = StringBuilderCache.Allocate();
                        for (var j = 0; j < rawString.Length; j++)
                        {
                            // strip the back-slash used to escape quote char in strings
                            var ch = rawString[j];
                            if (ch != '\\' || j + 1 >= rawString.Length || rawString[j + 1] != quoteChar)
                                sb.Append(ch);
                        }
                        token = new JsLiteral(StringBuilderCache.ReturnAndFree(sb));
                    }
                    else
                    {
                        var unescapedString = JsonTypeSerializer.Unescape(rawString);
                        token = new JsLiteral(unescapedString.ToString());
                    }
                }
                else
                {
                    token = new JsLiteral(rawString.ToString());
                }

                return literal.Advance(i + 1);
            }

            if (firstChar >= '0' && firstChar <= '9')
            {
                i = 1;
                var hasExponent = false;
                var firstDecimalPos = -1;

                while (i < literal.Length && IsNumericChar(c = literal[i]) ||
                       (hasExponent = c == 'e' || c == 'E'))
                {
                    i++;

                    if (c == '.' && firstDecimalPos < 0)
                        firstDecimalPos = i;

                    if (hasExponent)
                    {
                        i += 2; // [e+1]0

                        while (i < literal.Length && IsNumericChar(literal[i]))
                            i++;

                        break;
                    }
                }

                var hasMemberSuffix = literal.SafeCharEquals(i - 1, '.'); // 1.square()
                var numLiteral = literal.Slice(0, hasMemberSuffix ? i - 1 : i);

                //don't convert into ternary to avoid Type coercion
                if (firstDecimalPos > 0 && firstDecimalPos < i || hasExponent)
                    token = new JsLiteral(ScriptConfig.ParseRealNumber(numLiteral));
                else
                    token = new JsLiteral(numLiteral.ParseSignedInteger());

                if (hasMemberSuffix)
                {
                    literal = literal.Advance(numLiteral.Length).ParseJsMemberExpression(ref token, filterExpression);
                    return literal;
                }

                return literal.Advance(i);
            }
            if (firstChar == '{')
            {
                var props = new List<JsProperty>();

                literal = literal.Advance(1);
                while (!literal.IsNullOrEmpty())
                {
                    literal = literal.AdvancePastWhitespace();
                    if (literal[0] == '}')
                    {
                        literal = literal.Advance(1);
                        break;
                    }

                    JsToken mapValueToken;

                    if (literal.StartsWith("..."))
                    {
                        literal = literal.Advance(3);
                        literal = literal.ParseJsExpression(out mapValueToken);

                        props.Add(new JsProperty(null, new JsSpreadElement(mapValueToken)));
                    }
                    else
                    {
                        literal = literal.ParseJsToken(out var mapKeyToken);

                        if (mapKeyToken is not JsLiteral && mapKeyToken is not JsTemplateLiteral && mapKeyToken is not JsIdentifier && mapKeyToken is not JsMemberExpression)
                            throw new SyntaxErrorException($"{mapKeyToken.DebugToken()} is not a valid Object key, expected literal, identifier or member expression.");

                        bool shorthand = false;

                        literal = literal.AdvancePastWhitespace();
                        if (literal.Length > 0 && literal[0] == ':')
                        {
                            literal = literal.Advance(1);
                            literal = literal.ParseJsExpression(out mapValueToken);

                        }
                        else
                        {
                            shorthand = true;
                            if (literal.Length == 0 || (c = literal[0]) != ',' && c != '}')
                                throw new SyntaxErrorException($"Unterminated object literal near: {literal.DebugLiteral()}");

                            mapValueToken = mapKeyToken;
                        }

                        props.Add(new JsProperty(mapKeyToken, mapValueToken, shorthand));
                    }

                    literal = literal.AdvancePastWhitespace();
                    if (literal.IsNullOrEmpty())
                        break;

                    if (literal[0] == '}')
                    {
                        literal = literal.Advance(1);
                        break;
                    }

                    literal = literal.AdvancePastChar(',');
                    literal = literal.AdvancePastWhitespace();
                }

                token = new JsObjectExpression(props);
                return literal;
            }
            if (firstChar == '[')
            {
                literal = literal.Advance(1);
                literal = literal.ParseArguments(out var elements, termination: ']');
                literal = literal.EnsurePastChar(']');

                token = new JsArrayExpression(elements);
                return literal;
            }

            var unaryOp = firstChar.GetUnaryOperator();
            if (unaryOp != null)
            {
                literal = literal.Advance(1);
                literal = literal.ParseJsToken(out var arg);
                token = new JsUnaryExpression(unaryOp, arg);
                return literal;
            }

            // identifier
            literal = literal.ParseIdentifier(out var node);

            if (node is not JsOperator)
            {
                literal = literal.ParseJsMemberExpression(ref node, filterExpression);
            }

            token = node;
            return literal;
        }

        /// <summary>
        /// Indexes the of quoted string.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="quoteChar">The quote character.</param>
        /// <param name="hasEscapeChars">if set to <c>true</c> [has escape chars].</param>
        /// <returns>System.Int32.</returns>
        public static int IndexOfQuotedString(this ReadOnlySpan<char> literal, char quoteChar, out bool hasEscapeChars)
        {
            char c;
            int i = 1;
            hasEscapeChars = false;

            while (i < literal.Length)
            {
                c = literal[i];
                if (c == quoteChar)
                {
                    if (!literal.SafeCharEquals(i - 1, '\\') ||
                        literal.SafeCharEquals(i - 2, '\\') && !literal.SafeCharEquals(i - 3, '\\'))
                        break;
                }

                i++;
                if (!hasEscapeChars)
                    hasEscapeChars = c == '\\';
            }

            if (i >= literal.Length || literal[i] != quoteChar)
                return -1;

            return i;
        }

        /// <summary>
        /// Parses the arguments list.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected identifier but was instead '{arg.DebugToken()}', near: {literal.DebugLiteral()}</exception>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected ',' or ')' but was instead '{literal.DebugFirstChar()}', near: {literal.DebugLiteral()}</exception>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected '(' but was instead '{literal.DebugFirstChar()}', near: {literal.DebugLiteral()}</exception>
        public static ReadOnlySpan<char> ParseArgumentsList(this ReadOnlySpan<char> literal, out List<JsIdentifier> args)
        {
            args = new List<JsIdentifier>();
            var c = literal[0];
            if (c == '(')
            {
                literal = literal.Advance(1);

                while (true)
                {
                    literal = literal.AdvancePastWhitespace();
                    literal = literal.ParseIdentifier(out var arg);
                    if (arg is not JsIdentifier param)
                        throw new SyntaxErrorException(
                            $"Expected identifier but was instead '{arg.DebugToken()}', near: {literal.DebugLiteral()}");

                    args.Add(param);

                    literal = literal.AdvancePastWhitespace();

                    if (literal.FirstCharEquals(')'))
                    {
                        literal = literal.Advance(1);
                        break;
                    }

                    if (!literal.FirstCharEquals(','))
                        throw new SyntaxErrorException(
                            $"Expected ',' or ')' but was instead '{literal.DebugFirstChar()}', near: {literal.DebugLiteral()}");

                    literal = literal.Advance(1);
                }
            }
            else throw new SyntaxErrorException(
                $"Expected '(' but was instead '{literal.DebugFirstChar()}', near: {literal.DebugLiteral()}");

            return literal;
        }

        /// <summary>
        /// Parses the js template literal.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns>JsToken.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected end of template literal expression '}}' but was instead {literal.DebugFirstChar()}</exception>
        private static JsToken ParseJsTemplateLiteral(ReadOnlySpan<char> literal)
        {
            var quasis = new List<JsTemplateElement>();
            var expressions = new List<JsToken>();
            var lastPos = 0;

            for (var i = 0; i < literal.Length; i++)
            {
                var c = literal[i];
                var isExpr = c == '$' && literal.CountPrecedingOccurrences(i - 1, '\\') % 2 != 1 && literal.SafeGetChar(i + 1) == '{';
                if (!isExpr)
                    continue;

                var lastChunk = literal.Slice(lastPos, i - lastPos);
                quasis.Add(new JsTemplateElement(
                    new JsTemplateElementValue(lastChunk.ToString(), lastChunk.CookRawString('`')),
                    tail: false));

                var exprStart = literal.Slice(i + 2);
                var afterExpr = exprStart.ParseJsExpression(out var expr);
                afterExpr = afterExpr.AdvancePastWhitespace();

                if (!afterExpr.FirstCharEquals('}'))
                    throw new SyntaxErrorException($"Expected end of template literal expression '}}' but was instead {literal.DebugFirstChar()}");
                afterExpr = afterExpr.Advance(1);

                expressions.Add(expr);

                lastPos = literal.Length - afterExpr.Length;
                i = lastPos - 1;
            }

            var endChunk = literal.Slice(lastPos);

            quasis.Add(new JsTemplateElement(
                new JsTemplateElementValue(endChunk.ToString(), endChunk.CookRawString('`')),
                tail: true));

            return new JsTemplateLiteral(quasis.ToArray(), expressions.ToArray());
        }

        /// <summary>
        /// Parses the arrow expression body.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="token">The token.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected '=>' but instead found {literal.DebugFirstChar()} near: {literal.DebugLiteral()}</exception>
        internal static ReadOnlySpan<char> ParseArrowExpressionBody(this ReadOnlySpan<char> literal, JsIdentifier[] args, out JsArrowFunctionExpression token)
        {
            literal = literal.AdvancePastWhitespace();

            if (!literal.StartsWith("=>"))
                throw new SyntaxErrorException($"Expected '=>' but instead found {literal.DebugFirstChar()} near: {literal.DebugLiteral()}");

            literal = literal.Advance(2);
            literal = literal.ParseJsExpression(out var body, filterExpression: true);
            token = new JsArrowFunctionExpression(args, body);
            return literal;
        }

        /// <summary>
        /// Parses the variable declaration.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="kind">The kind.</param>
        /// <param name="token">The token.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        internal static ReadOnlySpan<char> ParseVariableDeclaration(this ReadOnlySpan<char> literal, JsVariableDeclarationKind kind, out JsVariableDeclaration token)
        {
            literal = literal.AdvancePastWhitespace();

            var declarations = new List<JsDeclaration>();
            while (true)
            {
                literal = literal.ParseIdentifier(out var id);
                literal = literal.AdvancePastWhitespace();

                if (literal.FirstCharEquals('='))
                {
                    literal = literal.Advance(1);
                    literal = literal.ParseJsExpression(out var init);
                    declarations.Add(new JsDeclaration((JsIdentifier)id, init));
                }
                else
                {
                    declarations.Add(new JsDeclaration((JsIdentifier)id, null));
                }

                literal = literal.AdvancePastWhitespace();
                if (!literal.FirstCharEquals(','))
                    break;

                literal = literal.Advance(1);
            }

            literal = literal.AdvancePastWhitespace();

            token = new JsVariableDeclaration(kind, declarations.ToArray());
            return literal;
        }

        /// <summary>
        /// Parses the assignment expression.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected '=' but was {literal.DebugFirstChar()}</exception>
        internal static ReadOnlySpan<char> ParseAssignmentExpression(this ReadOnlySpan<char> literal, JsIdentifier id, out JsAssignmentExpression token)
        {
            literal = literal.AdvancePastWhitespace();

            if (!literal.FirstCharEquals('='))
                throw new SyntaxErrorException($"Expected '=' but was {literal.DebugFirstChar()}");

            literal = literal.Advance(1);

            literal = literal.ParseJsExpression(out var init);
            token = new JsAssignmentExpression(id, JsAssignment.Operator, init);

            return literal;
        }

        /// <summary>
        /// Parses the js member expression.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="node">The node.</param>
        /// <param name="filterExpression">if set to <c>true</c> [filter expression].</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        internal static ReadOnlySpan<char> ParseJsMemberExpression(this ReadOnlySpan<char> literal, ref JsToken node, bool filterExpression)
        {
            literal = literal.AdvancePastWhitespace();

            if (literal.IsNullOrEmpty())
                return literal;

            var c = literal[0];

            while (c == '.' || c == '[' || c == '(' || filterExpression && c == ':')
            {
                literal = literal.Advance(1);

                if (c == '.')
                {
                    literal = literal.AdvancePastWhitespace();
                    literal = literal.ParseIdentifier(out var property);
                    node = new JsMemberExpression(node, property, computed: false);
                }
                else if (c == '[')
                {
                    literal = literal.AdvancePastWhitespace();
                    literal = literal.ParseJsExpression(out var property);
                    node = new JsMemberExpression(node, property, computed: true);
                    literal = EnsurePastChar(literal, ']');
                }
                else if (c == '(')
                {
                    literal = literal.ParseArguments(out var args, termination: ')');
                    node = new JsCallExpression(node, args.ToArray());
                    literal = literal.EnsurePastChar(')');
                }
                else if (filterExpression)
                {
                    if (c == ':')
                    {
                        literal = literal.ParseWhitespaceArgument(out var argument);
                        node = new JsCallExpression(node, argument);
                        return literal;
                    }

                    var peekLiteral = literal.AdvancePastWhitespace();
                    if (peekLiteral.StartsWith("=>"))
                    {
                        literal = peekLiteral.ParseArrowExpressionBody(new[] { new JsIdentifier("it") }, out var arrowExpr);
                        node = arrowExpr;
                        return literal;
                    }
                }

                literal = literal.AdvancePastWhitespace();

                if (literal.IsNullOrEmpty())
                    break;

                c = literal[0];
            }

            return literal;
        }

        /// <summary>
        /// Ensures the past character.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="c">The c.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected '{c}' but was {literal.DebugFirstChar()}</exception>
        internal static ReadOnlySpan<char> EnsurePastChar(this ReadOnlySpan<char> literal, char c)
        {
            literal = literal.AdvancePastWhitespace();
            if (!literal.FirstCharEquals(c))
                throw new SyntaxErrorException($"Expected '{c}' but was {literal.DebugFirstChar()}");

            literal = literal.Advance(1);
            return literal;
        }

        /// <summary>
        /// Parses the js binary operator.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="op">The op.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Invalid Operator found near: {literal.DebugLiteral()}</exception>
        internal static ReadOnlySpan<char> ParseJsBinaryOperator(this ReadOnlySpan<char> literal, out JsBinaryOperator op)
        {
            literal = literal.AdvancePastWhitespace();
            op = null;

            if (literal.IsNullOrEmpty())
                return literal;

            var firstChar = literal[0];
            if (firstChar.IsOperatorChar())
            {
                if (literal.StartsWith("!=="))
                {
                    op = JsStrictNotEquals.Operator;
                    return literal.Advance(3);
                }
                if (literal.StartsWith("==="))
                {
                    op = JsStrictEquals.Operator;
                    return literal.Advance(3);
                }

                if (literal.StartsWith(">="))
                {
                    op = JsGreaterThanEqual.Operator;
                    return literal.Advance(2);
                }
                if (literal.StartsWith("<="))
                {
                    op = JsLessThanEqual.Operator;
                    return literal.Advance(2);
                }
                if (literal.StartsWith("!="))
                {
                    op = JsNotEquals.Operator;
                    return literal.Advance(2);
                }
                if (literal.StartsWith("=="))
                {
                    op = JsEquals.Operator;
                    return literal.Advance(2);
                }
                if (literal.StartsWith("||"))
                {
                    op = JsOr.Operator;
                    return literal.Advance(2);
                }
                if (literal.StartsWith("??"))
                {
                    op = JsCoalescing.Operator;
                    return literal.Advance(2);
                }
                if (literal.StartsWith("&&"))
                {
                    op = JsAnd.Operator;
                    return literal.Advance(2);
                }
                if (literal.StartsWith("<<"))
                {
                    op = JsBitwiseLeftShift.Operator;
                    return literal.Advance(2);
                }
                if (literal.StartsWith(">>"))
                {
                    op = JsBitwiseRightShift.Operator;
                    return literal.Advance(2);
                }

                switch (firstChar)
                {
                    case '>':
                        op = JsGreaterThan.Operator;
                        return literal.Advance(1);
                    case '<':
                        op = JsLessThan.Operator;
                        return literal.Advance(1);
                    case '=':
                        if (ScriptConfig.AllowAssignmentExpressions)
                            op = JsAssignment.Operator;
                        else
                            op = JsEquals.Operator;
                        return literal.Advance(1);
                    case '+':
                        op = JsAddition.Operator;
                        return literal.Advance(1);
                    case '-':
                        op = JsSubtraction.Operator;
                        return literal.Advance(1);
                    case '*':
                        op = JsMultiplication.Operator;
                        return literal.Advance(1);
                    case '/':
                        op = JsDivision.Operator;
                        return literal.Advance(1);
                    case '&':
                        op = JsBitwiseAnd.Operator;
                        return literal.Advance(1);
                    case '|':
                        op = JsBitwiseOr.Operator;
                        return literal.Advance(1);
                    case '^':
                        op = JsBitwiseXOr.Operator;
                        return literal.Advance(1);
                    case '%':
                        op = JsMod.Operator;
                        return literal.Advance(1);

                    case '?': // a single '?' is not a binary operator but is an op char used in '??'
                        return literal;
                    default:
                        throw new SyntaxErrorException($"Invalid Operator found near: {literal.DebugLiteral()}");
                }
            }

            if (literal.StartsWith("and") && literal.SafeGetChar(3).IsWhiteSpace())
            {
                op = JsAnd.Operator;
                return literal.Advance(3);
            }

            if (literal.StartsWith("or") && literal.SafeGetChar(2).IsWhiteSpace())
            {
                op = JsOr.Operator;
                return literal.Advance(2);
            }

            return literal;
        }

        /// <summary>
        /// Parses the name of the variable.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="varName">Name of the variable.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected start of identifier but was {c.DebugChar()} near: {literal.DebugLiteral()}</exception>
        public static ReadOnlySpan<char> ParseVarName(this ReadOnlySpan<char> literal, out ReadOnlySpan<char> varName)
        {
            literal = literal.AdvancePastWhitespace();

            var c = literal.SafeGetChar(0);
            if (!c.IsValidVarNameChar())
                throw new SyntaxErrorException($"Expected start of identifier but was {c.DebugChar()} near: {literal.DebugLiteral()}");

            var i = 1;

            while (i < literal.Length)
            {
                c = literal[i];

                if (IsValidVarNameChar(c))
                    i++;
                else
                    break;
            }

            varName = literal.Slice(0, i).TrimEnd();
            literal = literal.Advance(i);

            return literal;
        }

        /// <summary>
        /// Parses the name of the variable.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="varName">Name of the variable.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected start of identifier but was {c.DebugChar()} near: {literal.DebugLiteral()}</exception>
        public static ReadOnlyMemory<char> ParseVarName(this ReadOnlyMemory<char> literal, out ReadOnlyMemory<char> varName)
        {
            literal = literal.AdvancePastWhitespace();

            var c = literal.SafeGetChar(0);
            if (!c.IsValidVarNameChar())
                throw new SyntaxErrorException($"Expected start of identifier but was {c.DebugChar()} near: {literal.DebugLiteral()}");

            var i = 1;

            var span = literal.Span;
            while (i < span.Length)
            {
                c = span[i];

                if (IsValidVarNameChar(c))
                    i++;
                else
                    break;
            }

            varName = literal.Slice(0, i).TrimEnd();
            literal = literal.Advance(i);

            return literal;
        }

        /// <summary>
        /// Parses the identifier.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="token">The token.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        internal static ReadOnlySpan<char> ParseIdentifier(this ReadOnlySpan<char> literal, out JsToken token)
        {
            literal = literal.ParseVarName(out var identifier);

            if (identifier.EqualsOrdinal("true"))
                token = JsLiteral.True;
            else if (identifier.EqualsOrdinal("false"))
                token = JsLiteral.False;
            else if (identifier.EqualsOrdinal("null"))
                token = JsNull.Value;
            else if (identifier.EqualsOrdinal("and"))
                token = JsAnd.Operator;
            else if (identifier.EqualsOrdinal("or"))
                token = JsOr.Operator;
            else
                token = new JsIdentifier(identifier);

            return literal;
        }

    }

    /// <summary>
    /// Class CallExpressionUtils.
    /// </summary>
    public static class CallExpressionUtils
    {
        /// <summary>
        /// The whitespace argument
        /// </summary>
        private const char WhitespaceArgument = ':';

        /// <summary>
        /// Parses the js call expression.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="filterExpression">if set to <c>true</c> [filter expression].</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Expected identifier but instead found {token.DebugToken()}</exception>
        public static ReadOnlySpan<char> ParseJsCallExpression(this ReadOnlySpan<char> literal, out JsCallExpression expression, bool filterExpression = false)
        {
            literal = literal.ParseIdentifier(out var token);

            if (token is not JsIdentifier identifier)
                throw new SyntaxErrorException($"Expected identifier but instead found {token.DebugToken()}");

            literal = literal.AdvancePastWhitespace();

            if (literal.FirstCharEquals(WhitespaceArgument))
            {
                literal = literal.Advance(1);
                literal = literal.ParseWhitespaceArgument(out var argument);
                expression = new JsCallExpression(identifier, argument);
                return literal;
            }

            if (literal.StartsWith("=>"))
            {
                literal = literal.ParseArrowExpressionBody(new[] { new JsIdentifier("it") }, out var arrowExpr);
                expression = new JsCallExpression(identifier, arrowExpr);
                return literal;
            }

            if (!literal.FirstCharEquals('('))
            {
                expression = new JsCallExpression(identifier);
                return literal;
            }

            literal = literal.Advance(1);

            literal = literal.ParseArguments(out var args, termination: ')');
            literal = literal.EnsurePastChar(')');

            expression = new JsCallExpression(identifier, args.ToArray());
            return literal;
        }

        /// <summary>
        /// Parses the whitespace argument.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="argument">The argument.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Whitespace sensitive syntax did not find a '\\n' new line to mark the end of the statement, near {literal.DebugLiteral()}</exception>
        internal static ReadOnlySpan<char> ParseWhitespaceArgument(this ReadOnlySpan<char> literal, out JsToken argument)
        {
            // replace everything after ':' up till new line and rewrite as single string to method
            var endStringPos = literal.IndexOf("\n");
            var endStatementPos = literal.IndexOf("}}");

            if (endStringPos == -1 || endStatementPos != -1 && endStatementPos < endStringPos)
                endStringPos = endStatementPos;

            if (endStringPos == -1)
                throw new SyntaxErrorException($"Whitespace sensitive syntax did not find a '\\n' new line to mark the end of the statement, near {literal.DebugLiteral()}");

            var originalArg = literal.Slice(0, endStringPos).Trim().ToString();
            var rewrittenArgs = originalArg.Replace("{", "{{").Replace("}", "}}");
            var strArg = new JsLiteral(rewrittenArgs);

            argument = strArg;
            return literal.Slice(endStringPos);
        }

        /// <summary>
        /// Parses the arguments.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="termination">The termination.</param>
        /// <returns>ReadOnlySpan&lt;System.Char&gt;.</returns>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Spread operator expected array but instead found {listValue.DebugToken()}</exception>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Unterminated arguments expression near: {literal.DebugLiteral()}</exception>
        internal static ReadOnlySpan<char> ParseArguments(this ReadOnlySpan<char> literal, out List<JsToken> arguments, char termination)
        {
            arguments = new List<JsToken>();

            while (!literal.IsNullOrEmpty())
            {
                JsToken listValue;

                literal = literal.AdvancePastWhitespace();
                if (literal[0] == termination)
                    break;

                if (literal.StartsWith("..."))
                {
                    literal = literal.Advance(3);
                    literal = literal.ParseJsExpression(out listValue);
                    if (listValue is not JsIdentifier && listValue is not JsArrayExpression && listValue is not JsCallExpression)
                        throw new SyntaxErrorException($"Spread operator expected array but instead found {listValue.DebugToken()}");

                    listValue = new JsSpreadElement(listValue);
                }
                else
                {
                    literal = literal.ParseJsExpression(out listValue);
                }

                arguments.Add(listValue);

                literal = literal.AdvancePastWhitespace();
                if (literal.IsNullOrEmpty())
                    break;

                if (literal[0] == termination)
                    break;

                literal = literal.AdvancePastWhitespace();
                var c = literal.SafeGetChar(0);
                if (c.IsEnd() || c == termination)
                    break;

                if (c != ',')
                    throw new SyntaxErrorException($"Unterminated arguments expression near: {literal.DebugLiteral()}");

                literal = literal.Advance(1);
                literal = literal.AdvancePastWhitespace();
            }

            literal = literal.AdvancePastWhitespace();

            return literal;
        }
    }
}