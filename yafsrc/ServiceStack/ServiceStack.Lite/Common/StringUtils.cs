// ***********************************************************************
// <copyright file="StringUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ServiceStack.Text;

/// <summary>
/// Class TextNode.
/// </summary>
public class TextNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextNode" /> class.
    /// </summary>
    public TextNode()
    {
        this.Children = [];
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the children.
    /// </summary>
    /// <value>The children.</value>
    public List<TextNode> Children { get; set; }
}

/// <summary>
/// Class StringUtils.
/// </summary>
public static class StringUtils
{
    /// <summary>
    /// Parses the commands.
    /// </summary>
    /// <param name="commandsString">The commands string.</param>
    /// <returns>List&lt;Command&gt;.</returns>
    public static List<Command> ParseCommands(this string commandsString)
    {
        return commandsString.AsMemory().ParseCommands(',');
    }

    /// <summary>
    /// Parses the commands.
    /// </summary>
    /// <param name="commandsString">The commands string.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>List&lt;Command&gt;.</returns>
    /// <exception cref="ServiceStack.DiagnosticEvent.Exception">Illegal syntax near '{commandsString.SafeSlice(pos - 10, 50)}...'</exception>
    /// <exception cref="System.Exception">Illegal syntax near '{commandsString.SafeSlice(pos - 10, 50)}...'</exception>
    public static List<Command> ParseCommands(this ReadOnlyMemory<char> commandsString,
                                              char separator = ',')
    {
        var to = new List<Command>();
        var pos = 0;

        if (commandsString.IsNullOrEmpty())
        {
            return to;
        }

        var inDoubleQuotes = false;
        var inSingleQuotes = false;
        var inBackTickQuotes = false;
        var inPrimeQuotes = false;
        var inBrackets = false;

        var endBlockPos = commandsString.Length;
        var cmd = new Command();
        var segmentStartPos = 0;

        try
        {
            for (var i = 0; i < commandsString.Length; i++)
            {
                var c = commandsString.Span[i];
                if (c.IsWhiteSpace())
                {
                    continue;
                }

                if (inDoubleQuotes)
                {
                    if (c == '"')
                    {
                        inDoubleQuotes = false;
                    }

                    continue;
                }
                if (inSingleQuotes)
                {
                    if (c == '\'')
                    {
                        inSingleQuotes = false;
                    }

                    continue;
                }
                if (inBackTickQuotes)
                {
                    if (c == '`')
                    {
                        inBackTickQuotes = false;
                    }

                    continue;
                }
                if (inPrimeQuotes)
                {
                    if (c == '′')
                    {
                        inPrimeQuotes = false;
                    }

                    continue;
                }
                switch (c)
                {
                    case '"':
                        inDoubleQuotes = true;
                        continue;
                    case '\'':
                        inSingleQuotes = true;
                        continue;
                    case '`':
                        inBackTickQuotes = true;
                        continue;
                    case '′':
                        inPrimeQuotes = true;
                        continue;
                }

                if (c == '(')
                {
                    inBrackets = true;
                    cmd.Name = commandsString.Slice(pos, i - pos).Trim().ToString();
                    pos = i + 1;

                    var literal = commandsString.Slice(pos);
                    var literalRemaining = ParseArguments(literal, out var args);
                    cmd.Args = args;
                    var endPos = literal.Length - literalRemaining.Length;

                    i += endPos;
                    pos = i + 1;
                    continue;
                }
                if (c == ')')
                {
                    inBrackets = false;
                    pos = i + 1;

                    pos = cmd.IndexOfMethodEnd(commandsString, pos);
                    continue;
                }

                if (inBrackets && c == ',')
                {
                    var arg = commandsString.Slice(pos, i - pos).Trim();
                    cmd.Args.Add(arg);
                    pos = i + 1;
                }
                else if (c == separator)
                {
                    if (string.IsNullOrEmpty(cmd.Name))
                    {
                        cmd.Name = commandsString.Slice(pos, i - pos).Trim().ToString();
                    }
                    else
                    {
                        cmd.Suffix = commandsString.Slice(pos - cmd.Suffix.Length, i - pos + cmd.Suffix.Length);
                    }

                    cmd.Original = commandsString.Slice(segmentStartPos, i - segmentStartPos).Trim();

                    to.Add(cmd);
                    cmd = new Command();
                    segmentStartPos = pos = i + 1;
                }
            }

            var remaining = commandsString.Slice(pos, endBlockPos - pos);
            if (!remaining.Trim().IsNullOrEmpty())
            {
                pos += remaining.Length;
                cmd.Name = remaining.Trim().ToString();
            }

            if (!cmd.Name.IsNullOrEmpty())
            {
                cmd.Original = commandsString.Slice(segmentStartPos, commandsString.Length - segmentStartPos).Trim();
                to.Add(cmd);
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Illegal syntax near '{commandsString.SafeSlice(pos - 10, 50)}...'", e);
        }

        return to;
    }

    // ( {args} , {args} )
    //   ^
    /// <summary>
    /// Parses the arguments.
    /// </summary>
    /// <param name="argsString">The arguments string.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
    public static ReadOnlyMemory<char> ParseArguments(ReadOnlyMemory<char> argsString, out List<ReadOnlyMemory<char>> args)
    {
        var to = new List<ReadOnlyMemory<char>>();

        var inDoubleQuotes = false;
        var inSingleQuotes = false;
        var inBackTickQuotes = false;
        var inPrimeQuotes = false;
        var inBrackets = 0;
        var inParens = 0;
        var inBraces = 0;
        var lastPos = 0;

        for (var i = 0; i < argsString.Length; i++)
        {
            var c = argsString.Span[i];
            if (inDoubleQuotes)
            {
                if (c == '"')
                {
                    inDoubleQuotes = false;
                }

                continue;
            }
            if (inSingleQuotes)
            {
                if (c == '\'')
                {
                    inSingleQuotes = false;
                }

                continue;
            }
            if (inBackTickQuotes)
            {
                if (c == '`')
                {
                    inBackTickQuotes = false;
                }

                continue;
            }
            if (inPrimeQuotes)
            {
                if (c == '′')
                {
                    inPrimeQuotes = false;
                }

                continue;
            }
            if (inBrackets > 0)
            {
                if (c == '[')
                {
                    ++inBrackets;
                }
                else if (c == ']')
                {
                    --inBrackets;
                }

                continue;
            }
            if (inBraces > 0)
            {
                if (c == '{')
                {
                    ++inBraces;
                }
                else if (c == '}')
                {
                    --inBraces;
                }

                continue;
            }
            if (inParens > 0)
            {
                if (c == '(')
                {
                    ++inParens;
                }
                else if (c == ')')
                {
                    --inParens;
                }

                continue;
            }

            switch (c)
            {
                case '"':
                    inDoubleQuotes = true;
                    continue;
                case '\'':
                    inSingleQuotes = true;
                    continue;
                case '`':
                    inBackTickQuotes = true;
                    continue;
                case '′':
                    inPrimeQuotes = true;
                    continue;
                case '[':
                    inBrackets++;
                    continue;
                case '{':
                    inBraces++;
                    continue;
                case '(':
                    inParens++;
                    continue;
                case ',':
                    {
                        var arg = argsString.Slice(lastPos, i - lastPos).Trim();
                        to.Add(arg);
                        lastPos = i + 1;
                        continue;
                    }
                case ')':
                    {
                        var arg = argsString.Slice(lastPos, i - lastPos).Trim();
                        if (!arg.IsNullOrEmpty())
                        {
                            to.Add(arg);
                        }

                        args = to;
                        return argsString.Advance(i);
                    }
            }
        }

        args = to;
        return TypeConstants.EmptyStringMemory;
    }


    /// <summary>
    /// HTMLs the encode.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <returns>System.String.</returns>
    public static string HtmlEncode(this string html)
    {
        return System.Net.WebUtility.HtmlEncode(html).Replace("′", "&prime;");
    }

    /// <summary>
    /// Splits the generic arguments.
    /// </summary>
    /// <param name="argList">The argument list.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public static List<string> SplitGenericArgs(string argList)
    {
        var to = new List<string>();
        if (string.IsNullOrEmpty(argList))
        {
            return to;
        }

        var lastPos = 0;
        var blockCount = 0;
        for (var i = 0; i < argList.Length; i++)
        {
            var argChar = argList[i];
            switch (argChar)
            {
                case ',':
                    if (blockCount == 0)
                    {
                        var arg = argList[lastPos..i];
                        to.Add(arg);
                        lastPos = i + 1;
                    }
                    break;
                case '<':
                    blockCount++;
                    break;
                case '>':
                    blockCount--;
                    break;
            }
        }

        if (lastPos > 0)
        {
            var arg = argList[lastPos..];
            to.Add(arg);
        }
        else
        {
            to.Add(argList);
        }

        return to;
    }

    /// <summary>
    /// The block chars
    /// </summary>
    private readonly static char[] blockChars = ['<', '>'];
    /// <summary>
    /// Parses the type into nodes.
    /// </summary>
    /// <param name="typeDef">The type definition.</param>
    /// <returns>TextNode.</returns>
    public static TextNode ParseTypeIntoNodes(this string typeDef)
    {
        if (string.IsNullOrEmpty(typeDef))
        {
            return null;
        }

        var node = new TextNode();
        var lastBlockPos = typeDef.IndexOf('<');

        if (lastBlockPos >= 0)
        {
            node.Text = typeDef[..lastBlockPos].Trim();

            var blockStartingPos = new Stack<int>();
            blockStartingPos.Push(lastBlockPos);

            while (lastBlockPos != -1 || blockStartingPos.Count == 0)
            {
                var nextPos = typeDef.IndexOfAny(blockChars, lastBlockPos + 1);
                if (nextPos == -1)
                {
                    break;
                }

                var blockChar = typeDef.Substring(nextPos, 1);

                if (blockChar == "<")
                {
                    blockStartingPos.Push(nextPos);
                }
                else
                {
                    var startPos = blockStartingPos.Pop();
                    if (blockStartingPos.Count == 0)
                    {
                        var endPos = nextPos;
                        var childBlock = typeDef.Substring(startPos + 1, endPos - startPos - 1);

                        var args = SplitGenericArgs(childBlock);
                        foreach (var arg in args)
                        {
                            if (arg.IndexOfAny(blockChars) >= 0)
                            {
                                var childNode = ParseTypeIntoNodes(arg);
                                if (childNode != null)
                                {
                                    node.Children.Add(childNode);
                                }
                            }
                            else
                            {
                                node.Children.Add(new TextNode { Text = arg.Trim() });
                            }
                        }

                    }
                }

                lastBlockPos = nextPos;
            }
        }
        else
        {
            node.Text = typeDef.Trim();
        }

        return node;
    }

    /// <summary>
    /// Determines whether [is white space] [the specified c].
    /// </summary>
    /// <param name="c">The c.</param>
    /// <returns>bool.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpace(this char c)
    {
        return c == ' ' || c is >= '\x0009' and <= '\x000d' || c == '\x00a0' || c == '\x0085';
    }

    /// <summary>
    /// Firsts the character equals.
    /// </summary>
    /// <param name="literal">The literal.</param>
    /// <param name="c">The c.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FirstCharEquals(this string literal, char c)
    {
        return !string.IsNullOrEmpty(literal) && literal[0] == c;
    }
}