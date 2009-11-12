/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace YAF.Classes.Utils
{
  /// <summary>
  /// The string helper.
  /// </summary>
  public static class StringHelper
  {
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
    public static string GenerateRandomString(int length, string pickfrom)
    {
      var r = new Random();
      string result = string.Empty;
      int picklen = pickfrom.Length - 1;
      int index = 0;
      for (int i = 0; i < length; i++)
      {
        index = r.Next(picklen);
        result = result + pickfrom.Substring(index, 1);
      }

      return result;
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
    public static string Truncate(string input, int limit)
    {
      string output = input;

      if (String.IsNullOrEmpty(input))
      {
        return null;
      }

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
          int LastSpace = output.LastIndexOf(" ");

          // if we found a space then, cut back to that space
          if (LastSpace != -1)
          {
            output = output.Substring(0, LastSpace);
          }
        }

        // Finally, add the "..."
        output += "...";
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
    public static string TruncateMiddle(string input, int limit)
    {
      string output = input;
      const string middle = "...";

      // Check if the string is longer than the allowed amount
      // otherwise do nothing
      if (output.Length > limit && limit > 0)
      {
        // figure out how much to make it fit...
        int left = (limit/2) - (middle.Length/2);
        int right = limit - left - (middle.Length/2);

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

    /// <summary>
    /// When the string is trimmed, is it null or empty?
    /// </summary>
    /// <param name="str">
    /// </param>
    /// <returns>
    /// The is null or empty trimmed.
    /// </returns>
    public static bool IsNullOrEmptyTrimmed(this string str)
    {
      return str == null || String.IsNullOrEmpty(str.Trim());
    }

    /* Ederon - 9/9/2007 */

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
      if (trim)
      {
        text = text.Trim();
      }

      if (nullify && text.Trim().Length == 0)
      {
        text = null;
      }

      return text;
    }

    /// <summary>
    /// Removes multiple whitespace characters from a string.
    /// </summary>
    /// <param name="text">
    /// </param>
    /// <returns>
    /// The remove multiple whitespace.
    /// </returns>
    public static string RemoveMultipleWhitespace(string text)
    {
      var r = new Regex(@"\s+");
      return r.Replace(text, @" ");
    }

    /// <summary>
    /// Removes multiple single quote ' characters from a string.
    /// </summary>
    /// <param name="text">
    /// </param>
    /// <returns>
    /// The remove multiple single quotes.
    /// </returns>
    public static string RemoveMultipleSingleQuotes(string text)
    {
      var r = new Regex(@"\'");
      return r.Replace(text, @"'");
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
      var md5CryptoServiceProvider = new MD5CryptoServiceProvider();

      byte[] emailBytes = Encoding.UTF8.GetBytes(strValue);
      emailBytes = md5CryptoServiceProvider.ComputeHash(emailBytes);

      var s = new StringBuilder();

      foreach (byte b in emailBytes)
      {
        s.Append(b.ToString("x2").ToLower());
      }

      return s.ToString();
    }

    /// <summary>
    /// Converts a String to a MemoryStream.
    /// </summary>
    /// <param name="str">
    /// </param>
    /// <returns>
    /// </returns>
    public static MemoryStream StringToStream(string str)
    {
      byte[] byteArray = Encoding.ASCII.GetBytes(str);
      return new MemoryStream(byteArray);
    }

    /// <summary>
    /// Converts a Stream to a String.
    /// </summary>
    /// <param name="theStream">
    /// </param>
    /// <returns>
    /// The stream to string.
    /// </returns>
    public static string StreamToString(Stream theStream)
    {
      var reader = new StreamReader(theStream);
      return reader.ReadToEnd();
    }
  }
}