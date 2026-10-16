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

namespace YAF.Core.Helpers;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// The download links of attachments: they contain a signature of the attachment id (HMAC-SHA256 with a random key
/// of the forum), only links shown by the forum can be used (the ids are counted up, any attachment could be
/// downloaded with its id).
/// </summary>
public static class AttachmentLinkHelper
{
    /// <summary>
    /// The name of the key in the registry (for all boards).
    /// </summary>
    private const string KeyName = "attachmentlinkkey";

    /// <summary>
    /// The length of the signature in bytes.
    /// </summary>
    private const int SignatureLength = 16;

    /// <summary>
    /// Only one request creates the key.
    /// </summary>
    private readonly static Lock KeyLock = new();

    /// <summary>
    /// Gets the download link of an attachment.
    /// </summary>
    /// <param name="attachmentId">The attachment identifier.</param>
    /// <param name="editor">If the download is counted (the link of the file, not the preview of an image).</param>
    /// <returns>Returns the link.</returns>
    public static string GetUrl(int attachmentId, bool editor)
    {
        return BoardContext.Current.Get<IUrlHelper>().Action(
            "GetAttachment",
            "Attachments",
            new { attachmentId, editor, sig = Sign(attachmentId) });
    }

    /// <summary>
    /// Gets the signature of an attachment id.
    /// </summary>
    /// <param name="attachmentId">The attachment identifier.</param>
    /// <returns>Returns the signature (base64url).</returns>
    public static string Sign(int attachmentId)
    {
        return WebEncoders.Base64UrlEncode(ComputeSignature(attachmentId));
    }

    /// <summary>
    /// Checks the signature of a download link.
    /// </summary>
    /// <param name="attachmentId">The attachment identifier.</param>
    /// <param name="signature">The signature of the link.</param>
    /// <returns>Returns if the signature belongs to the attachment.</returns>
    public static bool IsValid(int attachmentId, string signature)
    {
        if (signature.IsNotSet() || signature.Length > 64)
        {
            return false;
        }

        byte[] given;

        try
        {
            given = WebEncoders.Base64UrlDecode(signature);
        }
        catch (FormatException)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(given, ComputeSignature(attachmentId));
    }

    /// <summary>
    /// Computes the signature of an attachment id.
    /// </summary>
    /// <param name="attachmentId">The attachment identifier.</param>
    /// <returns>Returns the signature.</returns>
    private static byte[] ComputeSignature(int attachmentId)
    {
        var hash = HMACSHA256.HashData(GetKey(), Encoding.UTF8.GetBytes(attachmentId.ToString()));

        return hash[..SignatureLength];
    }

    /// <summary>
    /// Gets the key of the forum, it is created once (a random key, stored in the registry).
    /// </summary>
    /// <returns>Returns the key.</returns>
    private static byte[] GetKey()
    {
        var key = BoardContext.Current.Get<IDataCache>().GetOrSet(
            KeyName,
            () =>
            {
                lock (KeyLock)
                {
                    var registry = BoardContext.Current.GetRepository<Registry>();

                    var stored = registry.GetSingle(r => r.Name == KeyName && r.BoardID == null)?.Value;

                    if (stored.IsNotSet())
                    {
                        registry.Save(KeyName, Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)));

                        // the stored key (another server could have saved its key at the same time)
                        stored = registry.GetSingle(r => r.Name == KeyName && r.BoardID == null).Value;
                    }

                    return stored;
                }
            });

        return Convert.FromBase64String(key);
    }
}
