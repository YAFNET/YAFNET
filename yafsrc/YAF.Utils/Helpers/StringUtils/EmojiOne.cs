//  The MIT License (MIT)
//  Copyright (c) 2016 Linus Birgerstam
//    
//  Permission is hereby granted, free of charge, to any person obtaining a copy of
//  this software and associated documentation files (the "Software"), to deal in
//  the Software without restriction, including without limitation the rights to
//  use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//  of the Software, and to permit persons to whom the Software is furnished to do
//  so, subject to the following conditions:
//    
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//    
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.

namespace YAF.Utils.Helpers.StringUtils
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper class for converting emoji to different formats.
    /// </summary>
    public static partial class EmojiOne
    {
        /// <summary>
        /// Converts shortname emojis to unicode, useful for sending emojis back to mobile devices.
        /// </summary>
        /// <param name="str">The input string</param>
        /// <param name="ascii"><c>true</c> to also convert ascii emoji in the inpur string to unicode.</param>
        /// <returns>A string with unicode replacements</returns>
        public static string ShortnameToUnicode(string str, bool ascii = false)
        {
            if (str != null)
            {
                str = Regex.Replace(
                    str,
                    IGNORE_PATTERN + "|" + SHORTNAME_PATTERN,
                    ShortnameToUnicodeCallback,
                    RegexOptions.IgnoreCase);
            }

            if (ascii)
            {
                str = AsciiToUnicode(str);
            }

            return str;
        }

        /// <summary>
        /// Converts ascii emoji to unicode, e.g. :) -&gt; 😄
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        /// Returns the converted string
        /// </returns>
        public static string AsciiToUnicode(string str)
        {
            if (str != null)
            {
                str = Regex.Replace(
                    str,
                    IGNORE_PATTERN + "|" + ASCII_PATTERN,
                    AsciiToUnicodeCallback,
                    RegexOptions.IgnoreCase);
            }

            return str;
        }

        /// <summary>
        /// ASCIIs to unicode callback.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>Returns the converted string</returns>
        private static string AsciiToUnicodeCallback(Match match)
        {
            // check if the emoji exists in our dictionaries
            var ascii = match.Value;

            return ASCII_TO_CODEPOINT.ContainsKey(ascii) ? ToUnicode(ASCII_TO_CODEPOINT[ascii]) : match.Value;

            // we didn't find a replacement so just return the entire match
        }

        /// <summary>
        /// Shortnames to unicode callback.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>Returns the converted string</returns>
        private static string ShortnameToUnicodeCallback(Match match)
        {
            // check if the emoji exists in our dictionaries
            var shortname = match.Value;

            return SHORTNAME_TO_CODEPOINT.ContainsKey(shortname)
                       ? ToUnicode(SHORTNAME_TO_CODEPOINT[shortname])
                       : match.Value;

            // we didn't find a replacement so just return the entire match
        }

        /// <summary>
        /// Convert a unicode character to its code point/code pair
        /// </summary>
        /// <param name="unicode">The unicode.</param>
        /// <returns>
        /// Returns the converted string
        /// </returns>
        internal static string ToCodePoint(string unicode)
        {
            var codepoint = string.Empty;
            for (var i = 0; i < unicode.Length; i += char.IsSurrogatePair(unicode, i) ? 2 : 1)
            {
                if (i > 0)
                {
                    codepoint += "-";
                }

                codepoint += string.Format("{0:X4}", char.ConvertToUtf32(unicode, i));
            }

            return codepoint.ToLower();
        }

        /// <summary>
        /// Converts a unicode code point/code pair to a unicode character
        /// </summary>
        /// <param name="codepoint">The codepoint.</param>
        /// <returns>Returns the converted string</returns>
        internal static string ToUnicode(string codepoint)
        {
            if (codepoint.Contains('-'))
            {
                var pair = codepoint.Split('-');
                var hilos = new string[pair.Length];
                var chars = new char[pair.Length];
                for (var i = 0; i < pair.Length; i++)
                {
                    var part = Convert.ToInt32(pair[i], 16);
                    if (part >= 0x10000 && part <= 0x10FFFF)
                    {
                        var hi = Math.Floor((decimal)(part - 0x10000) / 0x400) + 0xD800;
                        var lo = (part - 0x10000) % 0x400 + 0xDC00;
                        hilos[i] = new String(new char[] { (char)hi, (char)lo });
                    }
                    else
                    {
                        chars[i] = (char)part;
                    }
                }

                return hilos.Any(x => x != null) ? string.Concat(hilos) : new String(chars);
            }
            else
            {
                var i = Convert.ToInt32(codepoint, 16);
                return char.ConvertFromUtf32(i);
            }
        }
    }
}