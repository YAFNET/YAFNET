// ***********************************************************************
// <copyright file="JsvFormatter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Text;
using ServiceStack.Text.Common;

namespace ServiceStack.Text;

/// <summary>
/// Class JsvFormatter.
/// </summary>
public static class JsvFormatter
{
    /// <summary>
    /// Formats the specified serialized text.
    /// </summary>
    /// <param name="serializedText">The serialized text.</param>
    /// <returns>System.String.</returns>
    public static string Format(string serializedText)
    {
        if (string.IsNullOrEmpty(serializedText)) return null;

        var tabCount = 0;
        var sb = StringBuilderThreadStatic.Allocate();
        var firstKeySeparator = true;
        var inString = false;

        for (var i = 0; i < serializedText.Length; i++)
        {
            var current = serializedText[i];
            var previous = i - 1 >= 0 ? serializedText[i - 1] : 0;
            var next = i < serializedText.Length - 1 ? serializedText[i + 1] : 0;

            switch (current)
            {
                case JsWriter.MapStartChar:
                case JsWriter.ListStartChar:
                    {
                        if (previous == JsWriter.MapKeySeperator)
                        {
                            if (next == JsWriter.MapEndChar || next == JsWriter.ListEndChar)
                            {
                                sb.Append(current);
                                sb.Append(serializedText[++i]); //eat next
                                continue;
                            }

                            AppendTabLine(sb, tabCount);
                        }

                        sb.Append(current);
                        AppendTabLine(sb, ++tabCount);
                        firstKeySeparator = true;
                        continue;
                    }
                case JsWriter.MapEndChar:
                case JsWriter.ListEndChar:
                    AppendTabLine(sb, --tabCount);
                    sb.Append(current);
                    firstKeySeparator = true;
                    continue;
                case JsWriter.QuoteChar:
                    sb.Append(current);
                    inString = !inString;
                    continue;
                case JsWriter.ItemSeperator when !inString:
                    sb.Append(current);
                    AppendTabLine(sb, tabCount);
                    firstKeySeparator = true;
                    continue;
            }

            sb.Append(current);

            if (current == JsWriter.MapKeySeperator && firstKeySeparator)
            {
                sb.Append(" ");
                firstKeySeparator = false;
            }
        }

        return StringBuilderThreadStatic.ReturnAndFree(sb);
    }

    /// <summary>
    /// Appends the tab line.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <param name="tabCount">The tab count.</param>
    private static void AppendTabLine(StringBuilder sb, int tabCount)
    {
        sb.AppendLine();

        if (tabCount > 0)
        {
            sb.Append(new string('\t', tabCount));
        }
    }
}