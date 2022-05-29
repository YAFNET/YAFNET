// ***********************************************************************
// <copyright file="ScriptPreprocessors.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using ServiceStack.Text;

namespace ServiceStack.Script;

/// <summary>
/// Class ScriptPreprocessors.
/// </summary>
public static class ScriptPreprocessors
{
    /// <summary>
    /// Transforms the code blocks.
    /// </summary>
    /// <param name="script">The script.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Unterminated '}}' multi-line block not found</exception>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Unterminated '```code' block, line with '```' not found</exception>
    public static string TransformCodeBlocks(string script)
    {
        var hadCodeBlocks = false;
        var processed = StringBuilderCache.Allocate();
        var inCodeBlock = false;
        var inMultiLineBlock = false;
        foreach (var line in script.ReadLines())
        {
            if (line == "```code")
            {
                hadCodeBlocks = true;
                inCodeBlock = true;
                continue;
            }
            if (inCodeBlock)
            {
                if (line == "```")
                {
                    inCodeBlock = false;
                    continue;
                }

                var codeOnly = line.Trim();
                if (string.IsNullOrEmpty(codeOnly))
                    continue;

                if (codeOnly.StartsWith("{{"))
                {
                    inMultiLineBlock = !codeOnly.EndsWith("}}");
                    processed.AppendLine(codeOnly);
                    continue;
                }

                if (inMultiLineBlock)
                {
                    if (codeOnly.EndsWith("}}"))
                    {
                        inMultiLineBlock = false;
                    }
                    processed.AppendLine(codeOnly);
                    continue;
                }

                if (codeOnly.StartsWith("*") && !codeOnly.EndsWith("*"))
                {
                    processed.AppendLine(codeOnly.Substring(1));
                    continue;
                }

                processed
                    .Append("{{")
                    .Append(codeOnly)
                    .AppendLine("}}");
                continue;
            }
            processed.AppendLine(line);
        }

        if (inMultiLineBlock)
            throw new SyntaxErrorException("Unterminated '}}' multi-line block not found");
        if (inCodeBlock)
            throw new SyntaxErrorException("Unterminated '```code' block, line with '```' not found");

        if (hadCodeBlocks)
            return StringBuilderCache.ReturnAndFree(processed);

        // return original script if there were no code blocks
        StringBuilderCache.Free(processed);
        return script;
    }

    /// <summary>
    /// Transforms the statement body.
    /// </summary>
    /// <param name="body">The body.</param>
    /// <returns>System.String.</returns>
    public static string TransformStatementBody(string body)
    {
        return TransformCodeBlocks("```code\n" + body + "\n```");
    }
}