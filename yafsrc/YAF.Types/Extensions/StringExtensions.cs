/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Types.Extensions
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    using YAF.Types;

    #endregion

    /// <summary>
    /// The string helper.
    /// </summary>
    public static class StringExtensions
    {
        #region Public Methods

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
            str = str.Replace("\"", "'");

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
            CodeContracts.VerifyNotNull(source, "source");
            CodeContracts.VerifyNotNull(pattern, "pattern");

            if (pattern.Length == 0)
            {
                return 0;
            }

            if (pattern.Length == 1)
            {
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
            CodeContracts.VerifyNotNull(input, "input");
            CodeContracts.VerifyNotNull(forEachAction, "forEachAction");

            input.ForEach(forEachAction);
        }

        /// <summary>
        /// Returns a "random" alpha-numeric string of specified length and characters.
        /// </summary>
        /// <param name="length">
        /// the length of the random string
        /// </param>
        /// <param name="pickFrom">
        /// the string of characters to pick randomly from
        /// </param>
        /// <returns>
        /// The generate random string.
        /// </returns>
        public static string GenerateRandomString(int length, [NotNull] string pickFrom)
        {
            CodeContracts.VerifyNotNull(pickFrom, "pickfrom");

            var r = new Random();
            var result = string.Empty;
            var pickFromLength = pickFrom.Length - 1;

            for (var i = 0; i < length; i++)
            {
                var index = r.Next(pickFromLength);
                result += pickFrom.Substring(index, 1);
            }

            return result;
        }

        /// <summary>
        /// Removes empty strings from the list
        /// </summary>
        /// <param name="inputList">
        /// The input list.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inputList"/> is <c>null</c>.
        /// </exception>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [NotNull]
        public static List<string> GetNewNoEmptyStrings([NotNull] this IEnumerable<string> inputList)
        {
            CodeContracts.VerifyNotNull(inputList, "inputList");

            return inputList.Where(x => x.IsSet()).ToList();
        }

        /// <summary>
        /// Removes strings that are smaller then <paramref name="minSize" />
        /// </summary>
        /// <param name="inputList">The input list.</param>
        /// <param name="minSize">The minimum size.</param>
        /// <returns></returns>
        [NotNull]
        public static List<string> GetNewNoSmallStrings([NotNull] this IEnumerable<string> inputList, int minSize)
        {
            CodeContracts.VerifyNotNull(inputList, "inputList");

            return inputList.Where(x => x.Length >= minSize).ToList();
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
        /// Processes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// The process text.
        /// </returns>
        public static string ProcessText(string text)
        {
            return ProcessText(text, true);
        }

        /// <summary>
        /// Processes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="nullify">The nullify.</param>
        /// <returns>
        /// The process text.
        /// </returns>
        public static string ProcessText(string text, bool nullify)
        {
            return ProcessText(text, nullify, true);
        }

        /// <summary>
        /// Processes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="nullify">The nullify.</param>
        /// <param name="trim">The trim.</param>
        /// <returns>
        /// The process text.
        /// </returns>
        public static string ProcessText(string text, bool nullify, bool trim)
        {
            if (trim && text.IsSet())
            {
                text = text.Trim();
            }

            if (nullify && text.IsNotSet())
            {
                text = null;
            }

            return text;
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
        /// Converts a string into it's hexadecimal representation.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns>
        /// The string to hex bytes.
        /// </returns>
        public static string StringToHexBytes(this string inputString)
        {
            var result = string.Empty;
            if (inputString.IsNotSet())
            {
                return result;
            }

            var cryptoServiceProvider = new MD5CryptoServiceProvider();

            var emailBytes = Encoding.UTF8.GetBytes(inputString);
            emailBytes = cryptoServiceProvider.ComputeHash(emailBytes);

            var s = new StringBuilder();

            emailBytes.ForEach(b => s.Append(b.ToString("x2").ToLower()));

            return s.ToString();
        }

        /// <summary>
        /// Converts a string to a list using delimiter.
        /// </summary>
        /// <param name="str">
        /// starting string
        /// </param>
        /// <param name="delimiter">
        /// value that delineates the string
        /// </param>
        /// <returns>
        /// list of strings
        /// </returns>
        public static List<string> StringToList(this string str, char delimiter)
        {
            return str.StringToList(delimiter, new List<string>());
        }

        /// <summary>
        /// Converts a string to a list using delimiter.
        /// </summary>
        /// <param name="str">
        /// starting string
        /// </param>
        /// <param name="delimiter">
        /// value that delineates the string
        /// </param>
        /// <param name="exclude">
        /// items to exclude from list
        /// </param>
        /// <returns>
        /// list of strings
        /// </returns>
        [NotNull]
        public static List<string> StringToList(
            [NotNull] this string str,
            char delimiter,
            [NotNull] List<string> exclude)
        {
            CodeContracts.VerifyNotNull(str, "str");
            CodeContracts.VerifyNotNull(exclude, "exclude");

            var list = str.Split(delimiter).ToList();

            list.RemoveAll(exclude.Contains);
            list.Remove(delimiter.ToString());

            return list;
        }

        /// <summary>
        /// Creates a delimited string an enumerable list of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objList">The object list.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>
        /// The list to string.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">objList;objList is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="objList" /> is <c>null</c>.</exception>
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
        /// Cleans a string into a proper RegEx statement. 
        ///   E.g. "[b]Whatever[/b]" will be converted to:
        ///   "\[b\]Whatever\[\/b\]"
        /// </summary>
        /// <param name="input">
        /// </param>
        /// <returns>
        /// The to reg ex string.
        /// </returns>
        [NotNull]
        public static string ToRegExString([NotNull] this string input)
        {
            CodeContracts.VerifyNotNull(input, "input");

            var sb = new StringBuilder();

            input.ForEachChar(
                c =>
                    {
                        if (!char.IsWhiteSpace(c) && !char.IsLetterOrDigit(c) && c != '_')
                        {
                            sb.Append("\\");
                        }

                        sb.Append(c);
                    });

            return sb.ToString();
        }

        /// <summary>
        /// Converts a String to a MemoryStream.
        /// </summary>
        /// <param name="str">
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        public static Stream ToStream([NotNull] this string str)
        {
            CodeContracts.VerifyNotNull(str, "str");

            var byteArray = Encoding.ASCII.GetBytes(str);
            return new MemoryStream(byteArray);
        }

        /// <summary>Truncates a string with the specified limits and adds (...) to the end if truncated</summary>
        /// <param name="input">input string</param>
        /// <param name="inputLimit"></param>
        /// <param name="cutOfString"></param>
        /// <returns>truncated string</returns>
        public static string Truncate(
            [CanBeNull] this string input,
            int inputLimit,
            [NotNull] string cutOfString = "...")
        {
            CodeContracts.VerifyNotNull(cutOfString, "cutOfString");

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
            output = output.Substring(0, limit);

            // Check if the space right after the truncate point 
            // was a space. if not, we are in the middle of a word and 
            // need to cut out the rest of it
            if (input.Substring(output.Length, 1) != " ")
            {
                var lastSpace = output.LastIndexOf(" ", StringComparison.Ordinal);

                // if we found a space then, cut back to that space
                if (lastSpace != -1)
                {
                    output = output.Substring(0, lastSpace);
                }
            }

            // Finally, add the the cut off string...
            output += cutOfString;

            return output;
        }

        /// <summary>
        /// Truncates a string with the specified limits by adding (...) to the middle
        /// </summary>
        /// <param name="input">
        /// input string
        /// </param>
        /// <param name="limit">
        /// max size of string
        /// </param>
        /// <returns>
        /// truncated string
        /// </returns>
        public static string TruncateMiddle(this string input, int limit)
        {
            if (input.IsNotSet())
            {
                return null;
            }

            var output = input;
            const string Middle = "...";

            // Check if the string is longer than the allowed amount
            // otherwise do nothing
            if (output.Length <= limit || limit <= 0)
            {
                return output;
            }

            // figure out how much to make it fit...
            var left = limit / 2 - Middle.Length / 2;
            var right = limit - left - Middle.Length / 2;

            if (left + right + Middle.Length < limit)
            {
                right++;
            }
            else if (left + right + Middle.Length > limit)
            {
                right--;
            }

            // cut the left side
            output = input.Substring(0, left);

            // add the middle
            output += Middle;

            // add the right side...
            output += input.Substring(input.Length - right, right);

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

        #endregion
    }
}