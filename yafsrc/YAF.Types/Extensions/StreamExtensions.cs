/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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
    using System.IO;

    public static class StreamExtensions
  {
    /// <summary>
    /// Converts a Stream to a String.
    /// </summary>
    /// <param name="theStream">
    /// </param>
    /// <returns>
    /// The stream to string.
    /// </returns>
    public static string AsString(this Stream theStream)
    {
      var reader = new StreamReader(theStream);
      return reader.ReadToEnd();
    }

    /// <summary>
    /// The copy stream.
    /// </summary>
    /// <param name="input">
    /// The input.
    /// </param>
    /// <param name="output">
    /// The output.
    /// </param>
    public static void CopyTo(this Stream input, Stream output)
    {
      var buffer = new byte[1024];
      int count = buffer.Length;

      while (count > 0)
      {
        count = input.Read(buffer, 0, count);
        if (count > 0)
        {
          output.Write(buffer, 0, count);
        }
      }
    }
  }
}