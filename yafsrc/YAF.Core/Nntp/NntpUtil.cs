/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
namespace YAF.Core.Nntp;

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using YAF.Types.Objects.Nntp;

/// <summary>
/// The NNTP utility Class.
/// </summary>
public static class NntpUtil
{
    /// <summary>
    ///   The base 64 pem code.
    /// </summary>
    private static readonly char[] Base64PemCode =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
            'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+',
            '/'
        };

    /// <summary>
    ///   The hex value.
    /// </summary>
    private static readonly int[] HexValue;

    /// <summary>
    ///   Initializes static members of the <see cref = "NntpUtil" /> class.
    /// </summary>
    static NntpUtil()
    {
        HexValue = new int[128];
        for (var i = 0; i <= 9; i++)
        {
            HexValue[i + '0'] = i;
        }

        for (var i = 0; i < 6; i++)
        {
            HexValue[i + 'A'] = i + 10;
        }

        var base64PemConvertCode = new byte[256];
        for (var i = 0; i < 255; i++)
        {
            base64PemConvertCode[i] = 255;
        }

        for (var i = 0; i < Base64PemCode.Length; i++)
        {
            base64PemConvertCode[Base64PemCode[i]] = (byte)i;
        }
    }

    /// <summary>
    /// The base 64 decode.
    /// </summary>
    /// <param name="encodedData">
    /// The encoded Data.
    /// </param>
    /// <param name="encoding">
    /// The encoding.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    [NotNull]
    public static string Base64Decode([NotNull] string encodedData, [CanBeNull] Encoding encoding = null)
    {
        CodeContracts.VerifyNotNull(encodedData);

        var decodedDataAsBytes = Convert.FromBase64String(encodedData);

        return (encoding ?? Encoding.Unicode).GetString(decodedDataAsBytes);
    }

    /// <summary>
    /// The base 64 decode.
    /// </summary>
    /// <param name="encodedData">
    /// The encoded data.
    /// </param>
    /// <param name="output">
    /// The output.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public static int Base64Decode([NotNull] string encodedData, Stream output)
    {
        CodeContracts.VerifyNotNull(encodedData);

        var decodedDataAsBytes = Convert.FromBase64String(encodedData);

        decodedDataAsBytes.AsEnumerable().ForEach(output.WriteByte);

        return decodedDataAsBytes.Length;
    }

    /// <summary>
    /// The base 64 header decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string Base64HeaderDecode(string line)
    {
        var m = Regex.Match(line, @"=\?([^?]+)\?[^?]+\?([^?]+)\?=");

        try
        {
            while (m.Success)
            {
                var matched = m.Groups[0].ToString();
                var encodingCode = m.Groups[1].ToString();

                line = line.Replace(
                    matched,
                    Base64Decode(m.Groups[2].ToString(), Encoding.GetEncoding(encodingCode)));

                m = m.NextMatch();
            }
        }
        catch (Exception)
        {
            // format problem...
        }

        return line;
    }

    /// <summary>
    /// Date from an Article Header converted to UTC
    /// </summary>
    /// <param name="nntpDateTime">
    /// The nntp Date Time.
    /// </param>
    /// <param name="tzi">
    /// The tzi.
    /// </param>
    /// <returns>
    /// The <see cref="DateTime"/>.
    /// </returns>
    public static DateTime DecodeUTC(string nntpDateTime, out int tzi)
    {
        try
        {
            nntpDateTime = nntpDateTime.Substring(nntpDateTime.IndexOf(',') + 1);
            if (nntpDateTime.IndexOf("(", StringComparison.Ordinal) > 0)
            {
                nntpDateTime = nntpDateTime.Substring(0, nntpDateTime.IndexOf('(') - 1).Trim();
            }

            var ipos = nntpDateTime.IndexOf('+');
            var ineg = nntpDateTime.IndexOf('-');
            var tz = string.Empty;
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

            var indGMT = nntpDateTime.IndexOf("GMT", StringComparison.Ordinal);

            if (indGMT > 0 && ineg < 0 && ipos < 0)
            {
                nntpDateTime = nntpDateTime.Substring(0, indGMT - 1).Trim();
            }

            if (DateTime.TryParse(nntpDateTime, out var dtc))
            {
                if (ipos > 0)
                {
                    var ts = TimeSpan.FromHours(tz.Substring(0, 2).ToType<int>()) +
                             TimeSpan.FromMinutes(tz.Substring(2, 2).ToType<int>());
                    tzi = ts.Minutes;
                    return dtc + ts;
                }

                if (ineg > 0)
                {
                    var ts = TimeSpan.FromHours(tz.Substring(0, 2).ToType<int>()) +
                             TimeSpan.FromMinutes(tz.Substring(2, 2).ToType<int>());
                    tzi = ts.Minutes;
                    return dtc - ts;
                }

                tzi = 0;
                return dtc;
            }
        }
        catch (Exception ex)
        {
            BoardContext.Current.Get<ILoggerService>().Log(
                BoardContext.Current.PageUserID,
                "NntpUtil",
                $"Unhandled NNTP DateTime nntpDateTime '{nntpDateTime}': {ex}");
        }

        tzi = 0;

        return DateTime.UtcNow;
    }

    /// <summary>
    /// The dispatch mime content.
    /// </summary>
    /// <param name="sr">
    /// The Stream Reader
    /// </param>
    /// <param name="part">
    /// The part.
    /// </param>
    /// <param name="separator">
    /// The separator.
    /// </param>
    /// <returns>
    /// The <see cref="MIMEPart"/>.
    /// </returns>
    public static MIMEPart DispatchMIMEContent(StreamReader sr, MIMEPart part, string separator)
    {
        string line;
        MemoryStream ms;
        byte[] bytes;
        switch (part.ContentType.Substring(0, part.ContentType.IndexOf('/')).ToUpper())
        {
            case "MULTIPART":
                while ((line = sr.ReadLine()) != null && line != separator && line != $"{separator}--")
                {
                    var m = Regex.Match(line, @"CONTENT-TYPE: ""?([^""\s;]+)", RegexOptions.IgnoreCase);
                    if (!m.Success)
                    {
                        continue;
                    }

                    var newPart = new MIMEPart
                                      {
                                          ContentType = m.Groups[1].ToString(),
                                          Charset = "US-ASCII",
                                          ContentTransferEncoding = "7BIT"
                                      };
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

                        m = Regex.Match(
                            line,
                            @"CONTENT-TRANSFER-ENCODING: ""?([^""\s;]+)",
                            RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            newPart.ContentTransferEncoding = m.Groups[1].ToString();
                        }

                        m = Regex.Match(line, @"NAME=""?([^""\s;]+)", RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            newPart.Filename = Base64HeaderDecode(m.Groups[1].ToString());
                            newPart.Filename = newPart.Filename.Substring(
                                newPart.Filename.LastIndexOfAny(new[] { '\\', '/' }) + 1);
                        }

                        line = sr.ReadLine();
                    }

                    part.EmbeddedPartList.Add(DispatchMIMEContent(sr, newPart, $"--{part.Boundary}"));
                }

                break;
            case "TEXT":
                ms = new MemoryStream();
                var msr = new StreamReader(ms, Encoding.GetEncoding(part.Charset));
                var sb = new StringBuilder();
                while ((line = sr.ReadLine()) != null && line != separator && line != $"{separator}--")
                {
                    var pos = ms.Position;
                    if (line != string.Empty)
                    {
                        switch (part.ContentTransferEncoding.ToUpper())
                        {
                            case "QUOTED-PRINTABLE":
                                QuotedPrintableDecode(line, ms);
                                break;
                            case "BASE64":
                                if (line != string.Empty)
                                {
                                    Base64Decode(line, ms);
                                }

                                break;
                            case "UU":
                                if (line != string.Empty)
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
                    sb.Append(
                        part.ContentType.ToUpper() == "TEXT/HTML"
                            ? msr.ReadToEnd()
                            : HttpUtility.HtmlEncode(msr.ReadToEnd()).Replace("\n", "<br>\n"));
                }

                part.Text = sb.ToString();
                break;
            default:
                ms = new MemoryStream();
                while ((line = sr.ReadLine()) != null && line != separator && line != $"{separator}--")
                {
                    if (line != string.Empty)
                    {
                        switch (part.ContentTransferEncoding.ToUpper())
                        {
                            case "QUOTED-PRINTABLE":
                                QuotedPrintableDecode(line, ms);
                                break;
                            case "BASE64":
                                if (line != string.Empty)
                                {
                                    Base64Decode(line, ms);
                                }

                                break;
                            case "UU":
                                if (line != string.Empty)
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
    /// The <see cref="int"/>.
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
    /// The <see cref="int"/>.
    /// </returns>
    public static int QuotedPrintableDecode(char[] line, Stream outputStream)
    {
        var length = line.Length;
        int i = 0, j = 0;
        while (i < length)
        {
            if (line[i] == '=')
            {
                if (i + 2 < length)
                {
                    outputStream.WriteByte((byte)(HexValue[line[i + 1]] << 4 | HexValue[line[i + 2]]));
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
    /// The <see cref="int"/>.
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
    /// The <see cref="int"/>.
    /// </returns>
    public static int UUDecode(char[] line, Stream outputStream)
    {
        if (line.Length < 1)
        {
            throw new InvalidOperationException($"Invalid line: {new string(line)}.");
        }

        if (line[0] == '`')
        {
            return 0;
        }

        var line2 = new uint[line.Length];
        for (var ii = 0; ii < line.Length; ii++)
        {
            line2[ii] = (uint)line[ii] - 32 & 0x3f;
        }

        var length = (int)line2[0];

        if ((int)(length / 3.0 + 0.999999999) * 4 > line.Length - 1)
        {
            throw new InvalidOperationException($"Invalid length({length}) with line: {new string(line)}.");
        }

        var i = 1;
        var j = 0;
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
}