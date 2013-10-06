/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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
		/// <param name="str">
		/// </param>
		/// <returns>
		/// The to js string.
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
			str = str.Replace("\"", @"\""");

			return str;
		}

		/// <summary>
		/// Function to check a max word length, used i.e. in topic names.
		/// </summary>
		/// <param name="text">
		/// The raw string to format
		/// </param>
		/// <param name="maxWordLength">
		/// The max Word Length.
		/// </param>
		/// <returns>
		/// The formatted string
		/// </returns>
		public static bool AreAnyWordsOverMaxLength([NotNull] this string text, int maxWordLength)
		{
			CodeContracts.VerifyNotNull(text, "text");

			if (maxWordLength > 0 && text.Length > 0)
			{
				var overMax = text.Split(' ').Where(w => w.IsSet() && w.Length > maxWordLength);

				return overMax.Any();
			}

			return false;
		}

        /// <summary>
        /// Function to remove words in a string based on a max string length, used i.e. in search.
        /// </summary>
        /// <param name="text">
        /// The raw string to format
        /// </param>
        /// <param name="maxStringLength">
        /// The max string length.
        /// </param>
        /// <returns>
        /// The formatted string
        /// </returns>
        public static string TrimWordsOverMaxLengthWordsPreserved([NotNull] this string text, int maxStringLength)
        {
            CodeContracts.VerifyNotNull(text, "text");
            string newText = string.Empty;
            if (maxStringLength > 0 && text.Length > 0)
            {
                string[] texArr = text.Trim().Split(' ');
               
                int length = 0;
                int count = 0;
                foreach (string s in texArr)
                {
                    length += s.Length;
                    if (length > maxStringLength)
                    {
                        if (count == 0)
                        {
                            newText = string.Empty;
                            
                        }
                        break;
                        
                    }
                    count++;
                    newText = newText + " " + s;
                }
            }

            return newText.Trim();
        }

		/// <summary>
		/// The fast index of.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="pattern">
		/// The pattern.
		/// </param>
		/// <returns>
		/// The fast index of.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// </exception>
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

			int limit = source.Length - pattern.Length + 1;
			if (limit < 1)
			{
				return -1;
			}

			// Store the first 2 characters of "pattern"
			char c0 = pattern[0];
			char c1 = pattern[1];

			// Find the first occurrence of the first character
			int first = source.IndexOf(c0, 0, limit);
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
				bool found = true;
				for (int j = 2; j < pattern.Length; j++)
				{
					if (source[first + j] != pattern[j])
					{
						found = false;
						break;
					}
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
		///   useful way. ;)
		/// </summary>
		/// <param name="input">
		/// </param>
		/// <param name="forEachAction">
		/// </param>
		public static void ForEachChar([NotNull] this string input, [NotNull] Action<char> forEachAction)
		{
			CodeContracts.VerifyNotNull(input, "input");
			CodeContracts.VerifyNotNull(forEachAction, "forEachAction");

			foreach (char c in input)
			{
				forEachAction(c);
			}
		}

		/// <summary>
		/// Formats a string with the provided parameters
		/// </summary>
		/// <param name="s">
		/// The s.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		/// <returns>
		/// The formatted string
		/// </returns>
		[StringFormatMethod("s")]
		public static string FormatWith(this string s, params object[] args)
		{
			return string.IsNullOrEmpty(s) ? null : String.Format(s, args);
		}

		/// <summary>
		/// Returns a "random" alpha-numeric string of specified length and characters.
		/// </summary>
		/// <param name="length">
		/// the length of the random string
		/// </param>
		/// <param name="pickfrom">
		/// the string of characters to pick randomly from
		/// </param>
		/// <returns>
		/// The generate random string.
		/// </returns>
		public static string GenerateRandomString(int length, [NotNull] string pickfrom)
		{
			CodeContracts.VerifyNotNull(pickfrom, "pickfrom");

			var r = new Random();
			string result = string.Empty;
			int picklen = pickfrom.Length - 1;

			for (int i = 0; i < length; i++)
			{
				int index = r.Next(picklen);
				result = result + pickfrom.Substring(index, 1);
			}

			return result;
		}

		/// <summary>
		/// Removes empty strings from the list
		/// </summary>
		/// <param name="inputList">
		/// </param>
		/// <returns>
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="inputList"/> is <c>null</c>.
		/// </exception>
		[NotNull]
		public static List<string> GetNewNoEmptyStrings([NotNull] this IEnumerable<string> inputList)
		{
			CodeContracts.VerifyNotNull(inputList, "inputList");

			return inputList.Where(x => x.IsSet()).ToList();
		}

		/// <summary>
		/// Removes strings that are smaller then <paramref name="minSize"/>
		/// </summary>
		/// <param name="inputList">
		/// </param>
		/// <param name="minSize">
		/// </param>
		/// <returns>
		/// </returns>
		[NotNull]
		public static List<string> GetNewNoSmallStrings([NotNull] this IEnumerable<string> inputList, int minSize)
		{
			CodeContracts.VerifyNotNull(inputList, "inputList");

			return inputList.Where(x => x.Length >= minSize).ToList();
		}

		/// <summary>
		/// When the string is trimmed, is it <see langword="null"/> or empty?
		/// </summary>
		/// <param name="str">
		/// </param>
		/// <returns>
		/// The is <see langword="null"/> or empty trimmed.
		/// </returns>
        [ContractAnnotation("str:null => true")]
		public static bool IsNotSet([CanBeNull] this string str)
		{
			return string.IsNullOrWhiteSpace(str);
		}

		/// <summary>
		/// When the string is trimmed, is it <see langword="null"/> or empty?
		/// </summary>
		/// <param name="str">
		/// </param>
		/// <returns>
		/// The is <see langword="null"/> or empty trimmed.
		/// </returns>
        [ContractAnnotation("str:null => false")]
		public static bool IsSet([CanBeNull] this string str)
		{
			return !string.IsNullOrWhiteSpace(str);
		}

		/// <summary>
		/// The process text.
		/// </summary>
		/// <param name="text">
		/// The text.
		/// </param>
		/// <returns>
		/// The process text.
		/// </returns>
		public static string ProcessText(string text)
		{
			return ProcessText(text, true);
		}

		/// <summary>
		/// The process text.
		/// </summary>
		/// <param name="text">
		/// The text.
		/// </param>
		/// <param name="nullify">
		/// The nullify.
		/// </param>
		/// <returns>
		/// The process text.
		/// </returns>
		public static string ProcessText(string text, bool nullify)
		{
			return ProcessText(text, nullify, true);
		}

		/// <summary>
		/// The process text.
		/// </summary>
		/// <param name="text">
		/// The text.
		/// </param>
		/// <param name="nullify">
		/// The nullify.
		/// </param>
		/// <param name="trim">
		/// The trim.
		/// </param>
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
		/// Removes multiple single quote ' characters from a string.
		/// </summary>
		/// <param name="text">
		/// </param>
		/// <returns>
		/// The remove multiple single quotes.
		/// </returns>
		public static string RemoveMultipleSingleQuotes(this string text)
		{
			string result = String.Empty;
			if (text.IsNotSet())
			{
				return result;
			}

			var r = new Regex(@"\'");
			return r.Replace(text, @"'");
		}

		/// <summary>
		/// Removes multiple whitespace characters from a string.
		/// </summary>
		/// <param name="text">
		/// </param>
		/// <returns>
		/// The remove multiple whitespace.
		/// </returns>
		public static string RemoveMultipleWhitespace(this string text)
		{
			string result = String.Empty;
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
		/// <param name="strValue">
		/// </param>
		/// <returns>
		/// The string to hex bytes.
		/// </returns>
		public static string StringToHexBytes(string strValue)
		{
			string result = String.Empty;
			if (strValue.IsNotSet())
			{
				return result;
			}

			var cryptoServiceProvider = new MD5CryptoServiceProvider();

			byte[] emailBytes = Encoding.UTF8.GetBytes(strValue);
			emailBytes = cryptoServiceProvider.ComputeHash(emailBytes);

			var s = new StringBuilder();

			foreach (byte b in emailBytes)
			{
				s.Append(b.ToString("x2").ToLower());
			}

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
		public static List<string> StringToList([NotNull] this string str, char delimiter, [NotNull] List<string> exclude)
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
		/// <typeparam name="T">
		/// </typeparam>
		/// <param name="objList">
		/// </param>
		/// <param name="delimiter">
		/// </param>
		/// <returns>
		/// The list to string.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="objList"/> is <c>null</c>.
		/// </exception>
		public static string ToDelimitedString<T>(this IEnumerable<T> objList, string delimiter) where T : IConvertible
		{
			if (objList == null)
			{
				throw new ArgumentNullException("objList", "objList is null.");
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
						sb.Append((object)x);
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
						if (!Char.IsWhiteSpace(c) && !Char.IsLetterOrDigit(c) && c != '_')
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

			byte[] byteArray = Encoding.ASCII.GetBytes(str);
			return new MemoryStream(byteArray);
		}

		/// <summary>
		/// Truncates a string with the specified limits and adds (...) to the end if truncated
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
		public static string Truncate([CanBeNull] this string input, int inputLimit, [NotNull] string cutOfString = "...")
		{
			CodeContracts.VerifyNotNull(cutOfString, "cutOfString");

			string output = input;

			if (input.IsNotSet())
			{
				return null;
			}

			int limit = inputLimit - cutOfString.Length;

			// Check if the string is longer than the allowed amount
			// otherwise do nothing
			if (output.Length > limit && limit > 0)
			{
				// cut the string down to the maximum number of characters
				output = output.Substring(0, limit);

				// Check if the space right after the truncate point 
				// was a space. if not, we are in the middle of a word and 
				// need to cut out the rest of it
				if (input.Substring(output.Length, 1) != " ")
				{
					int lastSpace = output.LastIndexOf(" ");

					// if we found a space then, cut back to that space
					if (lastSpace != -1)
					{
						output = output.Substring(0, lastSpace);
					}
				}

				// Finally, add the the cut off string...
				output += cutOfString;
			}

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

			string output = input;
			const string middle = "...";

			// Check if the string is longer than the allowed amount
			// otherwise do nothing
			if (output.Length > limit && limit > 0)
			{
				// figure out how much to make it fit...
				int left = (limit / 2) - (middle.Length / 2);
				int right = limit - left - (middle.Length / 2);

				if ((left + right + middle.Length) < limit)
				{
					right++;
				}
				else if ((left + right + middle.Length) > limit)
				{
					right--;
				}

				// cut the left side
				output = input.Substring(0, left);

				// add the middle
				output += middle;

				// add the right side...
				output += input.Substring(input.Length - right, right);
			}

			return output;
		}

		#endregion
	}
}
