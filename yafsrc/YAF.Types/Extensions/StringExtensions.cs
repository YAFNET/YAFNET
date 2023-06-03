/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Types.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using YAF.Types;

/// <summary>
/// The string helper.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts a string to an escaped JavaString string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>
    /// The JS string.
    /// </returns>
    public static string ToJsString([CanBeNull] this string str)
    {
        if (!str.IsSet())
        {
            return str;
        }

        str = str.Replace("\\", @"\\");
        str = str.Replace("'", @"\'");
        str = str.Replace("\r", @"\r");
        str = str.Replace("\n", @"\n");
        str = str.Replace("\"", "\'");

        return str;
    }

    /// <summary>
    /// Fast index of.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="pattern">The pattern.</param>
    /// <returns>
    /// The fast index of.
    /// </returns>
    public static int FastIndexOf([NotNull] this string source, [NotNull] string pattern)
    {
        CodeContracts.VerifyNotNull(source);
        CodeContracts.VerifyNotNull(pattern);

        switch (pattern.Length)
        {
            case 0:
                return 0;
            case 1:
                return source.IndexOf(pattern[0]);
        }

        var limit = source.Length - pattern.Length + 1;
        if (limit < 1)
        {
            return -1;
        }

        // Store the first 2 characters of "pattern"
        var c0 = pattern[0];
        var c1 = pattern[1];

        // Find the first occurrence of the first character
        var first = source.IndexOf(c0, 0, limit);
        while (first != -1)
        {
            // Check if the following character is the same like
            // the 2nd character of "pattern"
            if (source[first + 1] != c1)
            {
                first = source.IndexOf(c0, ++first, limit - first);
                continue;
            }

            // Check the rest of "pattern" (starting with the 3rd character)
            var found = true;
            for (var j = 2; j < pattern.Length; j++)
            {
                if (source[first + j] == pattern[j])
                {
                    continue;
                }

                found = false;
                break;
            }

            // If the whole word was found, return its index, otherwise try again
            if (found)
            {
                return first;
            }

            first = source.IndexOf(c0, ++first, limit - first);
        }

        return -1;
    }

    /// <summary>
    /// Does an action for each character in the input string. Kind of useless, but in a
    /// useful way. ;)
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="forEachAction">For each action.</param>
    public static void ForEachChar([NotNull] this string input, [NotNull] Action<char> forEachAction)
    {
        CodeContracts.VerifyNotNull(input);
        CodeContracts.VerifyNotNull(forEachAction);

        input.ForEach(forEachAction);
    }

    /// <summary>
    /// When the string is trimmed, is it <see langword="null" /> or empty?
    /// </summary>
    /// <param name="inputString">The input string.</param>
    /// <returns>
    /// The is <see langword="null" /> or empty trimmed.
    /// </returns>
    [ContractAnnotation("str:null => true")]
    public static bool IsNotSet([CanBeNull] this string inputString)
    {
        return string.IsNullOrWhiteSpace(inputString);
    }

    /// <summary>
    /// When the string is trimmed, is it <see langword="null" /> or empty?
    /// </summary>
    /// <param name="inputString">The input string.</param>
    /// <returns>
    /// The is <see langword="null" /> or empty trimmed.
    /// </returns>
    [ContractAnnotation("str:null => false")]
    public static bool IsSet([CanBeNull] this string inputString)
    {
        return !string.IsNullOrWhiteSpace(inputString);
    }

    /// <summary>
    /// Removes multiple whitespace characters from a string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>
    /// The remove multiple whitespace.
    /// </returns>
    public static string RemoveMultipleWhitespace(this string text)
    {
        var result = string.Empty;
        if (text.IsNotSet())
        {
            return result;
        }

        var r = new Regex(@"\s+");
        return r.Replace(text, @" ");
    }

    /// <summary>
    /// Creates a delimited string an enumerable list of T.
    /// </summary>
    /// <typeparam name="T">The typed Parameter</typeparam>
    /// <param name="objList">The object list.</param>
    /// <param name="delimiter">The delimiter.</param>
    /// <returns>
    /// The list to string.
    /// </returns>
    public static string ToDelimitedString<T>(this IEnumerable<T> objList, string delimiter) where T : IConvertible
    {
        if (objList == null)
        {
            throw new ArgumentNullException(nameof(objList), "objList is null.");
        }

        var sb = new StringBuilder();

        objList.ForEachFirst(
            (x, isFirst) =>
                {
                    if (!isFirst)
                    {
                        // append delimiter if this isn't the first string
                        sb.Append(delimiter);
                    }

                    // append string...
                    sb.Append(x);
                });

        return sb.ToString();
    }

    /// <summary>
    /// Cleans a string into a proper Regular Expression statement.
    ///   E.g. "[b]Whatever[/b]" will be converted to:
    ///   "\[b\]Whatever\[\/b\]"
    /// </summary>
    /// <param name="input">
    /// The input.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    [NotNull]
    public static string ToRegExString([NotNull] this string input)
    {
        CodeContracts.VerifyNotNull(input);

        var sb = new StringBuilder();

        input.ForEachChar(
            c =>
                {
                    if (!char.IsWhiteSpace(c) && !char.IsLetterOrDigit(c) && c != '_')
                    {
                        sb.Append('\\');
                    }

                    sb.Append(c);
                });

        return sb.ToString();
    }

    /// <summary>
    /// Truncates a string with the specified limits and adds (...) to the end if truncated
    /// </summary>
    /// <param name="input">
    /// input string
    /// </param>
    /// <param name="inputLimit">
    /// The input Limit.
    /// </param>
    /// <param name="cutOfString">
    /// The cut Of String.
    /// </param>
    /// <returns>
    /// truncated string
    /// </returns>
    public static string Truncate(
        [CanBeNull] this string input,
        int inputLimit,
        [NotNull] string cutOfString = "...")
    {
        CodeContracts.VerifyNotNull(cutOfString);

        var output = input;

        if (input.IsNotSet())
        {
            return null;
        }

        var limit = inputLimit - cutOfString.Length;

        // Check if the string is longer than the allowed amount
        // otherwise do nothing
        if (output.Length <= limit || limit <= 0)
        {
            return output;
        }

        // cut the string down to the maximum number of characters
        output = output[..limit];

        // Check if the space right after the truncate point
        // was a space. if not, we are in the middle of a word and
        // need to cut out the rest of it
        if (input.Substring(output.Length, 1) != " ")
        {
            var lastSpace = output.LastIndexOf(" ", StringComparison.Ordinal);

            // if we found a space then, cut back to that space
            if (lastSpace != -1)
            {
                output = output[..lastSpace];
            }
        }

        // Finally, add the the cut off string...
        output += cutOfString;

        return output;
    }

    /// <summary>
    /// Determines whether [is image name] [the specified input string].
    /// </summary>
    /// <param name="inputString">The input string.</param>
    /// <returns>Returns if the String is a Image Name</returns>
    public static bool IsImageName(this string inputString)
    {
        return inputString.EndsWith("png", StringComparison.InvariantCultureIgnoreCase)
               || inputString.EndsWith("gif", StringComparison.InvariantCultureIgnoreCase)
               || inputString.EndsWith("jpeg", StringComparison.InvariantCultureIgnoreCase)
               || inputString.EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase)
               || inputString.EndsWith("bmp", StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Converts persian Numbers to english.
    /// </summary>
    /// <param name="persianString">
    /// The persian string.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string PersianNumberToEnglish(this string persianString)
    {
        var lettersDictionary = new Dictionary<string, string>
                                    {
                                        ["۰"] = "0",
                                        ["۱"] = "1",
                                        ["۲"] = "2",
                                        ["۳"] = "3",
                                        ["۴"] = "4",
                                        ["۵"] = "5",
                                        ["۶"] = "6",
                                        ["۷"] = "7",
                                        ["۸"] = "8",
                                        ["۹"] = "9"
                                    };

        return lettersDictionary.Aggregate(persianString, (current, item) => current.Replace(item.Key, item.Value));
    }

    /// <summary>
    /// Converts a String Number to GUID.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <returns>
    /// The <see cref="Guid"/>.
    /// </returns>
    public static Guid ToGuid([NotNull] this string value)
    {
        CodeContracts.VerifyNotNull(value);

        var bytes = new byte[16];
        BitConverter.GetBytes(value.ToType<int>()).CopyTo(bytes, 0);
        return new Guid(bytes);
    }
}