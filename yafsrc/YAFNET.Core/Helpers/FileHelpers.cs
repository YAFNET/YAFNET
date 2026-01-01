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

using System;

namespace YAF.Core.Helpers;

using System.IO;

using Microsoft.AspNetCore.Hosting;

using YAF.Types.Models;

/// <summary>
///     The file helpers.
/// </summary>
public static class FileHelpers
{
    /// <summary>
    /// Deletes an attachment from the file system. No exceptions are handled in this function.
    /// </summary>
    /// <param name="attachment">
    /// The attachment.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public static bool DeleteFile(this Attachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);

        var webRootPath = BoardContext.Current.Get<IWebHostEnvironment>().WebRootPath;

        var uploadFolder = Path.Combine(webRootPath, BoardContext.Current.Get<BoardFolders>().Uploads);

        var fileNameOld =
            $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}")}.{attachment.FileName}.yafupload";

        if (File.Exists(fileNameOld))
        {
            File.Delete(fileNameOld);
            return true;
        }

        var fileName =
            $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}-{attachment.ID}")}.{attachment.FileName}.yafupload";

        if (!File.Exists(fileName))
        {
            return false;
        }

        File.Delete(fileName);

        return true;
    }
}