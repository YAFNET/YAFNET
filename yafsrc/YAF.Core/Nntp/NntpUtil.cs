/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
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
namespace YAF.Core.Nntp
{
  #region Using

  using System;
  using System.Collections;
  using System.IO;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Web;

  using YAF.Core;
  using YAF.Core.Extensions;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Classes.Data;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The nntp util.
  /// </summary>
  public class NntpUtil
  {
    #region Constants and Fields

    /// <summary>
    ///   The base 64 pem code.
    /// </summary>
    private static readonly char[] base64PemCode = {
                                                     'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
                                                     'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 
                                                     'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
                                                     'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
                                                     '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
                                                   };

    /// <summary>
    ///   The base 64 pem convert code.
    /// </summary>
    private static readonly byte[] base64PemConvertCode;

    /// <summary>
    ///   The hex value.
    /// </summary>
    private static readonly int[] hexValue;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes static members of the <see cref = "NntpUtil" /> class.
    /// </summary>
    static NntpUtil()
    {
      hexValue = new int[128];
      for (int i = 0; i <= 9; i++)
      {
        hexValue[i + '0'] = i;
      }

      for (int i = 0; i < 6; i++)
      {
        hexValue[i + 'A'] = i + 10;
      }

      base64PemConvertCode = new byte[256];
      for (int i = 0; i < 255; i++)
      {
        base64PemConvertCode[i] = 255;
      }

      for (int i = 0; i < base64PemCode.Length; i++)
      {
        base64PemConvertCode[base64PemCode[i]] = (byte)i;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The base 64 decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <param name="outputStream">
    /// The output stream.
    /// </param>
    /// <returns>
    /// The base 64 decode.
    /// </returns>
    [NotNull]
    public static string Base64Decode([NotNull] string encodedData, [CanBeNull] Encoding encoding = null)
    {
      CodeContracts.VerifyNotNull(encodedData, "encodedData");

      byte[] decodedDataAsBytes = Convert.FromBase64String(encodedData);

      return (encoding ?? Encoding.Unicode).GetString(decodedDataAsBytes);
    }

    public static int Base64Decode([NotNull] string encodedData, Stream output)
    {
      CodeContracts.VerifyNotNull(encodedData, "encodedData");

      byte[] decodedDataAsBytes = Convert.FromBase64String(encodedData);

      foreach (var decodedByte in decodedDataAsBytes)
      {
        output.WriteByte(decodedByte);
      }

      return decodedDataAsBytes.Length;
    }

    /// <summary>
    /// The base 64 header decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <returns>
    /// The base 64 header decode.
    /// </returns>
    public static string Base64HeaderDecode(string line)
    {
      Match m = Regex.Match(line, @"=\?([^?]+)\?[^?]+\?([^?]+)\?=");

      try
      {
        while (m.Success)
        {
          string matched = m.Groups[0].ToString();
          string encodingCode = m.Groups[1].ToString();

          line = line.Replace(matched, Base64Decode(m.Groups[2].ToString(), Encoding.GetEncoding(encodingCode)));

          m = m.NextMatch();
        }
      }
      catch (Exception ex)
      {
        // format problem...
      }

      return line;
    }

    /// <summary>
    /// The convert list to tree.
    /// </summary>
    /// <param name="list">
    /// The list.
    /// </param>
    /// <returns>
    /// </returns>
    public static ArrayList ConvertListToTree(ArrayList list)
    {
      var hash = new Hashtable(list.Count);
      var treeList = new ArrayList();
      int len;
      bool isTop;
      foreach (Article article in list)
      {
        isTop = true;
        hash[article.MessageId] = article;
        article.LastReply = article.Header.Date;
        article.Children = new ArrayList();
        len = article.Header.ReferenceIds.Length;
        for (int i = 0; i < len; i++)
        {
          if (hash.ContainsKey(article.Header.ReferenceIds[i]))
          {
            ((Article)hash[article.Header.ReferenceIds[i]]).LastReply = article.LastReply;
            break;
          }
        }

        for (int i = len - 1; i >= 0; i--)
        {
          if (hash.ContainsKey(article.Header.ReferenceIds[i]))
          {
            isTop = false;
            ((Article)hash[article.Header.ReferenceIds[i]]).Children.Add(article);
            break;
          }
        }

        if (isTop)
        {
          treeList.Add(article);
        }
      }

      return treeList;
    }

    /// <summary>
    /// Date from an Article Header converted to UTC
    /// </summary>
    /// <param name="nntpDateTime">
    /// </param>
    /// <param name="tzi">
    /// </param>
    /// <returns>
    /// </returns>
    public static DateTime DecodeUTC(string nntpDateTime, out int tzi)
    {
      try
      {
        nntpDateTime = nntpDateTime.Substring(nntpDateTime.IndexOf(',') + 1);
        if (nntpDateTime.IndexOf("(") > 0)
        {
          nntpDateTime = nntpDateTime.Substring(0, nntpDateTime.IndexOf('(') - 1).Trim();
        }

        int ipos = nntpDateTime.IndexOf('+');
        int ineg = nntpDateTime.IndexOf('-');
        string tz = string.Empty;
        if (ipos > 0)
        {
          tz = nntpDateTime.Substring(ipos + 1).Trim();
          nntpDateTime = nntpDateTime.Substring(0, ipos - 1).Trim();
        }
        else if (ineg > 0)
        {
          tz = nntpDateTime.Substring(ineg + 1).Trim();
          nntpDateTime = nntpDateTime.Substring(0, ineg - 1).Trim();
        }

        int indGMT = nntpDateTime.IndexOf("GMT");

        if (indGMT > 0 && ineg < 0 && ipos < 0)
        {
          nntpDateTime = nntpDateTime.Substring(0, indGMT - 1).Trim();
        }

        DateTime dtc;

        if (DateTime.TryParse(nntpDateTime, out dtc))
        {
          if (ipos > 0)
          {
            TimeSpan ts = TimeSpan.FromHours(Convert.ToInt32(tz.Substring(0, 2))) +
                          TimeSpan.FromMinutes(Convert.ToInt32(tz.Substring(2, 2)));
            tzi = ts.Minutes;
            return dtc + ts;
          }
          else if (ineg > 0)
          {
            TimeSpan ts = TimeSpan.FromHours(Convert.ToInt32(tz.Substring(0, 2))) +
                          TimeSpan.FromMinutes(Convert.ToInt32(tz.Substring(2, 2)));
            tzi = ts.Minutes;
            return dtc - ts;
          }
          else
          {
            tzi = 0;
            return dtc;
          }

          // eof vzrus
        }




      }
      catch (Exception ex)
      {
          YafContext.Current.Get<ILogger>()
                    .Log(YafContext.Current.PageUserID, "NntpUtil", "Unhandled NNTP DateTime nntpDateTime '{0}': {1}".FormatWith(nntpDateTime, ex));
      }

      tzi = 0;

      return DateTime.UtcNow;
    }

    /// <summary>
    /// The dispatch mime content.
    /// </summary>
    /// <param name="sr">
    /// The sr.
    /// </param>
    /// <param name="part">
    /// The part.
    /// </param>
    /// <param name="seperator">
    /// The seperator.
    /// </param>
    /// <returns>
    /// </returns>
    public static MIMEPart DispatchMIMEContent(StreamReader sr, MIMEPart part, string seperator)
    {
      string line = null;
      Match m = null;
      MemoryStream ms;
      byte[] bytes;
      switch (part.ContentType.Substring(0, part.ContentType.IndexOf('/')).ToUpper())
      {
        case "MULTIPART":
          MIMEPart newPart = null;
          while ((line = sr.ReadLine()) != null && line != seperator && line != seperator + "--")
          {
            m = Regex.Match(line, @"CONTENT-TYPE: ""?([^""\s;]+)", RegexOptions.IgnoreCase);
            if (!m.Success)
            {
              continue;
            }

            newPart = new MIMEPart();
            newPart.ContentType = m.Groups[1].ToString();
            newPart.Charset = "US-ASCII";
            newPart.ContentTransferEncoding = "7BIT";
            while (line != string.Empty)
            {
              m = Regex.Match(line, @"BOUNDARY=""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                newPart.Boundary = m.Groups[1].ToString();
                newPart.EmbeddedPartList = new ArrayList();
              }

              m = Regex.Match(line, @"CHARSET=""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                newPart.Charset = m.Groups[1].ToString();
              }

              m = Regex.Match(line, @"CONTENT-TRANSFER-ENCODING: ""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                newPart.ContentTransferEncoding = m.Groups[1].ToString();
              }

              m = Regex.Match(line, @"NAME=""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                newPart.Filename = Base64HeaderDecode(m.Groups[1].ToString());
                newPart.Filename = newPart.Filename.Substring(newPart.Filename.LastIndexOfAny(new[] { '\\', '/' }) + 1);
              }

              line = sr.ReadLine();
            }

            part.EmbeddedPartList.Add(DispatchMIMEContent(sr, newPart, "--" + part.Boundary));
          }

          break;
        case "TEXT":
          ms = new MemoryStream();
          bytes = null;
          long pos;
          var msr = new StreamReader(ms, Encoding.GetEncoding(part.Charset));
          var sb = new StringBuilder();
          while ((line = sr.ReadLine()) != null && line != seperator && line != seperator + "--")
          {
            pos = ms.Position;
            if (line != string.Empty)
            {
              switch (part.ContentTransferEncoding.ToUpper())
              {
                case "QUOTED-PRINTABLE":
                  QuotedPrintableDecode(line, ms);
                  break;
                case "BASE64":
                  if (line != null && line != string.Empty)
                  {
                    Base64Decode(line, ms);
                  }

                  break;
                case "UU":
                  if (line != null && line != string.Empty)
                  {
                    UUDecode(line, ms);
                  }

                  break;
                case "7BIT":
                  bytes = Encoding.ASCII.GetBytes(line);
                  ms.Write(bytes, 0, bytes.Length);
                  ms.WriteByte((byte)'\n');
                  break;
                default:
                  bytes = Encoding.ASCII.GetBytes(line);
                  ms.Write(bytes, 0, bytes.Length);
                  ms.WriteByte((byte)'\n');
                  break;
              }
            }

            ms.Position = pos;
            if (part.ContentType.ToUpper() == "TEXT/HTML")
            {
              sb.Append(msr.ReadToEnd());
            }
            else
            {
              sb.Append(HttpUtility.HtmlEncode(msr.ReadToEnd()).Replace("\n", "<br>\n"));
            }
          }

          part.Text = sb.ToString();
          break;
        default:
          ms = new MemoryStream();
          bytes = null;
          while ((line = sr.ReadLine()) != null && line != seperator && line != seperator + "--")
          {
            if (line != string.Empty)
            {
              switch (part.ContentTransferEncoding.ToUpper())
              {
                case "QUOTED-PRINTABLE":
                  QuotedPrintableDecode(line, ms);
                  break;
                case "BASE64":
                  if (line != null && line != string.Empty)
                  {
                    Base64Decode(line, ms);
                  }

                  break;
                case "UU":
                  if (line != null && line != string.Empty)
                  {
                    UUDecode(line, ms);
                  }

                  break;
                default:
                  bytes = Encoding.ASCII.GetBytes(line);
                  ms.Write(bytes, 0, bytes.Length);
                  break;
              }
            }
          }

          ms.Seek(0, SeekOrigin.Begin);
          part.BinaryData = new byte[ms.Length];
          ms.Read(part.BinaryData, 0, (int)ms.Length);
          break;
      }

      return part;
    }

    /// <summary>
    /// The quoted printable decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <param name="outputStream">
    /// The output stream.
    /// </param>
    /// <returns>
    /// The quoted printable decode.
    /// </returns>
    public static int QuotedPrintableDecode(string line, Stream outputStream)
    {
      return QuotedPrintableDecode(line.ToCharArray(), outputStream);
    }

    /// <summary>
    /// The quoted printable decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <param name="outputStream">
    /// The output stream.
    /// </param>
    /// <returns>
    /// The quoted printable decode.
    /// </returns>
    public static int QuotedPrintableDecode(char[] line, Stream outputStream)
    {
      int length = line.Length;
      int i = 0, j = 0;
      while (i < length)
      {
        if (line[i] == '=')
        {
          if (i + 2 < length)
          {
            outputStream.WriteByte((byte)(hexValue[line[i + 1]] << 4 | hexValue[line[i + 2]]));
            i += 3;
          }
          else
          {
            i++;
          }
        }
        else
        {
          outputStream.WriteByte((byte)line[i]);
          i++;
        }

        j++;
      }

      if (line[length - 1] != '=')
      {
        outputStream.WriteByte((byte)'\n');
      }

      return j;
    }

    /// <summary>
    /// The uu decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <param name="outputStream">
    /// The output stream.
    /// </param>
    /// <returns>
    /// The uu decode.
    /// </returns>
    public static int UUDecode(string line, Stream outputStream)
    {
      return UUDecode(line.ToCharArray(), outputStream);
    }

    /// <summary>
    /// The uu decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <param name="outputStream">
    /// The output stream.
    /// </param>
    /// <returns>
    /// The uu decode.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    public static int UUDecode(char[] line, Stream outputStream)
    {
      if (line.Length < 1)
      {
        throw new InvalidOperationException("Invalid line: " + new string(line) + ".");
      }

      if (line[0] == '`')
      {
        return 0;
      }

      var line2 = new uint[line.Length];
      for (int ii = 0; ii < line.Length; ii++)
      {
        line2[ii] = (uint)line[ii] - 32 & 0x3f;
      }

      var length = (int)line2[0];
      if ((int)(length / 3.0 + 0.999999999) * 4 > line.Length - 1)
      {
        throw new InvalidOperationException("Invalid length(" + length + ") with line: " + new string(line) + ".");
      }

      int i = 1;
      int j = 0;
      while (length > j + 3)
      {
        outputStream.WriteByte((byte)((line2[i] << 2 & 0xfc | line2[i + 1] >> 4 & 0x3) & 0xff));
        outputStream.WriteByte((byte)((line2[i + 1] << 4 & 0xf0 | line2[i + 2] >> 2 & 0xf) & 0xff));
        outputStream.WriteByte((byte)((line2[i + 2] << 6 & 0xc0 | line2[i + 3] & 0x3f) & 0xff));
        i += 4;
        j += 3;
      }

      if (length > j)
      {
        outputStream.WriteByte((byte)((line2[i] << 2 & 0xfc | line2[i + 1] >> 4 & 0x3) & 0xff));
      }

      if (length > j + 1)
      {
        outputStream.WriteByte((byte)((line2[i + 1] << 4 & 0xf0 | line2[i + 2] >> 2 & 0xf) & 0xff));
      }

      if (length > j + 2)
      {
        outputStream.WriteByte((byte)((line2[i + 2] << 6 & 0xc0 | line2[i + 3] & 0x3f) & 0xff));
      }

      return length;
    }

    #endregion
  }
}