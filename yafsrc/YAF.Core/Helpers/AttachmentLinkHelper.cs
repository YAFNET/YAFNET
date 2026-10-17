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

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// The download links of attachments: the attachments of the posts are not linked to a post (they are added with
/// [attach]id[/attach]), the links shown in a post contain a signature of the attachment id (HMAC-SHA256 with a
/// random key of the forum). Only links shown by the forum can be used, the ids can't be counted up.
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
    private static readonly object KeyLock = new();

    /// <summary>
    /// Gets the signature of an attachment id.
    /// </summary>
    /// <param name="attachmentId">The attachment identifier.</param>
    /// <returns>Returns the signature (base64url).</returns>
    public static string Sign(int attachmentId)
    {
        return Convert.ToBase64String(ComputeSignature(attachmentId)).TrimEnd('=').Replace('+', '-')
            .Replace('/', '_');
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

        var expected = Sign(attachmentId);

        // compared in constant time
        var difference = expected.Length ^ signature.Length;

        for (var i = 0; i < expected.Length && i < signature.Length; i++)
        {
            difference |= expected[i] ^ signature[i];
        }

        return difference == 0;
    }

    /// <summary>
    /// Checks if the current user can download the attachment: the download permission of the groups is needed,
    /// an attachment of a post (older versions) only of a post the user can see, an attachment added with
    /// [attach] only with a signed link of the forum (or by the uploader and the admins).
    /// </summary>
    /// <param name="attachment">The attachment.</param>
    /// <param name="signature">The signature of the link.</param>
    /// <returns>Returns if the attachment can be downloaded.</returns>
    public static bool CanDownload(Attachment attachment, string signature)
    {
        if (attachment == null || !BoardContext.Current.DownloadAccess)
        {
            return false;
        }

        var userId = BoardContext.Current.PageUserID;

        if (attachment.MessageID <= 0)
        {
            return attachment.UserID == userId || BoardContext.Current.IsAdmin || IsValid(attachment.ID, signature);
        }

        var message = BoardContext.Current.GetRepository<Message>().GetMessageWithAccess(attachment.MessageID, userId);

        if (message == null)
        {
            return false;
        }

        // the attachments of deleted and not approved posts (of others) only for the moderators of the forum
        var forumId = message.Item4.ID;

        var isModerator = BoardContext.Current.GetRepository<VAccess>()
            .Exists(v => v.UserID == userId && v.ForumID == forumId && v.ModeratorAccess > 0);

        return MessageAccessHelper.CanViewMessage(message.Item2, userId, isModerator);
    }

    /// <summary>
    /// Computes the signature of an attachment id.
    /// </summary>
    /// <param name="attachmentId">The attachment identifier.</param>
    /// <returns>Returns the signature.</returns>
    private static byte[] ComputeSignature(int attachmentId)
    {
        using var hmac = new HMACSHA256(GetKey());

        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(attachmentId.ToString()));

        var signature = new byte[SignatureLength];
        Array.Copy(hash, signature, SignatureLength);

        return signature;
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
                            var bytes = new byte[32];

                            using (var random = RandomNumberGenerator.Create())
                            {
                                random.GetBytes(bytes);
                            }

                            registry.Save(KeyName, Convert.ToBase64String(bytes));

                            // the stored key (another server could have saved its key at the same time)
                            stored = registry.GetSingle(r => r.Name == KeyName && r.BoardID == null).Value;
                        }

                        return stored;
                    }
                });

        return Convert.FromBase64String(key);
    }
}
