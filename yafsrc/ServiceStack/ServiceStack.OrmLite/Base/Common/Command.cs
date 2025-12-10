// ***********************************************************************
// <copyright file="Command.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack;

using System;
using System.Collections.Generic;

/// <summary>
/// Class Command.
/// </summary>
public class Command
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the original.
    /// </summary>
    /// <value>The original.</value>
    public ReadOnlyMemory<char> Original { get; set; }

    /// <summary>
    /// Gets the arguments.
    /// </summary>
    /// <value>The arguments.</value>
    public List<ReadOnlyMemory<char>> Args { get; internal set; } = [];

    /// <summary>
    /// Gets or sets the suffix.
    /// </summary>
    /// <value>The suffix.</value>
    public ReadOnlyMemory<char> Suffix { get; set; }

    /// <summary>
    /// Indexes the of method end.
    /// </summary>
    /// <param name="commandsString">The commands string.</param>
    /// <param name="pos">The position.</param>
    /// <returns>System.Int32.</returns>
    public int IndexOfMethodEnd(ReadOnlyMemory<char> commandsString, int pos)
    {
        //finding end of suffix, e.g: 'SUM(*) Total' or 'SUM(*) as Total'
        var endPos = pos;
        var cmdSpan = commandsString.Span;
        while (cmdSpan.Length > endPos && char.IsWhiteSpace(cmdSpan[endPos]))
        {
            endPos++;
        }

        if (cmdSpan.Length > endPos && cmdSpan.IndexOf("as ", endPos) == endPos)
        {
            endPos += "as ".Length;
        }

        while (cmdSpan.Length > endPos && char.IsWhiteSpace(cmdSpan[endPos]))
        {
            endPos++;
        }

        while (cmdSpan.Length > endPos &&
               char.IsLetterOrDigit(cmdSpan[endPos]))
        {
            endPos++;
        }

        this.Suffix = commandsString.Slice(pos, endPos - pos).TrimEnd();

        return endPos;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        var sb = StringBuilderCacheAlt.Allocate();
        foreach (var arg in this.Args)
        {
            if (sb.Length > 0)
            {
                sb.Append(',');
            }

            sb.Append(arg);
        }

        return $"{this.Name}({StringBuilderCacheAlt.ReturnAndFree(sb)}){this.Suffix}";
    }
}