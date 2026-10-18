/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Tests.UITests.Infrastructure;

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/// <summary>
/// A mail of the forum (written as .eml file into the mail folder of the test site).
/// </summary>
public class TestMail
{
    private TestMail(string headers, string body)
    {
        this.Headers = headers;
        this.Body = body;
    }

    /// <summary>
    /// Gets the headers.
    /// </summary>
    public string Headers { get; }

    /// <summary>
    /// Gets the decoded text of all parts.
    /// </summary>
    public string Body { get; }

    /// <summary>
    /// Gets the recipient.
    /// </summary>
    public string To => Regex.Match(this.Headers, @"^To:\s*(.*)$", RegexOptions.Multiline).Groups[1].Value.Trim();

    /// <summary>
    /// Gets the decoded subject (the encoded words of the folded header).
    /// </summary>
    public string Subject
    {
        get
        {
            var raw = Regex.Match(this.Headers, @"^Subject:[ \t]*(.*(?:\r\n[ \t].*)*)", RegexOptions.Multiline).Groups[1].Value;

            return Regex.Replace(
                raw.Replace("\r\n", string.Empty),
                @"=\?(?<charset>[^?]+)\?(?<encoding>[BbQq])\?(?<text>[^?]*)\?=\s*",
                word =>
                    {
                        var encoding = Encoding.GetEncoding(word.Groups["charset"].Value);
                        var text = word.Groups["text"].Value;

                        return word.Groups["encoding"].Value.ToUpperInvariant() == "B"
                                   ? encoding.GetString(Convert.FromBase64String(text))
                                   : DecodeQuotedPrintable(text.Replace('_', ' '));
                    }).Trim();
        }
    }

    /// <summary>
    /// Waits for the newest mail to the address.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="sentAfter">Only mails written after this time.</param>
    /// <returns>Returns the mail.</returns>
    public static async Task<TestMail> WaitForAsync(string address, DateTime sentAfter)
    {
        var until = DateTime.UtcNow.AddSeconds(30);

        while (DateTime.UtcNow < until)
        {
            var mail = UiTestSetup.Site.GetMails().Where(f => f.CreationTimeUtc >= sentAfter.AddSeconds(-1))
                .Select(f => Read(f.FullName))
                .FirstOrDefault(m => m.To.IndexOf(address, StringComparison.OrdinalIgnoreCase) >= 0);

            if (mail != null)
            {
                return mail;
            }

            await Task.Delay(500);
        }

        throw new TimeoutException($"No mail to {address} was sent.");
    }

    /// <summary>
    /// Gets the first link of the mail that contains the text.
    /// </summary>
    /// <param name="contains">The text of the link.</param>
    /// <returns>Returns the link.</returns>
    public string GetLink(string contains)
    {
        return Regex.Matches(this.Body, @"https?://[^\s""'<>]+").Cast<Match>().Select(m => m.Value)
                   .FirstOrDefault(l => l.IndexOf(contains, StringComparison.OrdinalIgnoreCase) >= 0)
               ?? throw new InvalidOperationException($"The mail has no link with {contains}: {this.Body}");
    }

    /// <summary>
    /// Reads a mail file (the parts encoded with base64 or quoted-printable are decoded).
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>Returns the mail.</returns>
    public static TestMail Read(string file)
    {
        var text = File.ReadAllText(file);

        var headerEnd = text.IndexOf("\r\n\r\n", StringComparison.Ordinal);
        var headers = headerEnd > 0 ? text.Substring(0, headerEnd) : text;

        var body = new StringBuilder();

        // the parts of the mail: their headers tell the encoding
        foreach (Match part in Regex.Matches(
                     text,
                     @"Content-Transfer-Encoding:\s*(?<encoding>[\w-]+)\s*\r\n(?:[^\r\n]+\r\n)*\r\n(?<content>.*?)(?=\r\n--|\z)",
                     RegexOptions.Singleline | RegexOptions.IgnoreCase))
        {
            var content = part.Groups["content"].Value;

            body.AppendLine(
                part.Groups["encoding"].Value.ToLowerInvariant() switch
                    {
                        "base64" => Encoding.UTF8.GetString(Convert.FromBase64String(Regex.Replace(content, @"\s", string.Empty))),
                        "quoted-printable" => DecodeQuotedPrintable(content),
                        _ => content
                    });
        }

        return new TestMail(headers, body.Length > 0 ? body.ToString() : text);
    }

    private static string DecodeQuotedPrintable(string content)
    {
        content = Regex.Replace(content, "=\r\n", string.Empty);

        var bytes = Regex.Replace(content, "=([0-9A-Fa-f]{2})", m => ((char)Convert.ToByte(m.Groups[1].Value, 16)).ToString())
            .Select(c => (byte)c).ToArray();

        return Encoding.UTF8.GetString(bytes);
    }
}
