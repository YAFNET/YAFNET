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

namespace YAF.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    using Newtonsoft.Json;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Objects;

    /// <summary>
    /// Based on a solution by
    /// https://forums.asp.net/t/1964610.aspx?How+to+check+the+uploaded+file+is+SECURE+
    /// The mime types.
    /// </summary>
    public static class MimeTypes
    {
        /// <summary>
        /// The mime types.
        /// </summary>
        private static List<MimeType> mimeTypes;

        /// <summary>
        /// Check if File Content Type is correct
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool FileMatchContentType(HttpPostedFile file)
        {
            return FileMatchContentType(file.FileName, file.ContentType);
        }

        /// <summary>
        /// Check if File Content Type is correct
        /// </summary>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        /// <param name="contentType">
        /// The content Type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool FileMatchContentType(string fileName, string contentType)
        {
            if (mimeTypes == null)
            {
                InitializeMimeTypeLists();
            }

            var extension = Path.GetExtension(fileName).Replace(".", string.Empty).ToLower();

            var isMatch = mimeTypes.Where(m => m.Extension == extension).Any(m => m.Type == contentType);

            if (!isMatch)
            {
                BoardContext.Current.Get<ILoggerService>().Info($"Mimetype for Extension: '{extension}' with type: '{contentType}' not found!");
            }

            return isMatch;
        }

        /// <summary>
        /// The initialize mime type lists.
        /// </summary>
        private static void InitializeMimeTypeLists()
        {
            var jsonFile = BoardContext.Current.Get<HttpContextBase>().Server.MapPath(
                $"{BoardInfo.ForumServerFileRoot}Resources/mimeTypes.json");

            mimeTypes = BoardContext.Current.Get<IDataCache>().GetOrSet(
                "MimeTypes",
                () => JsonConvert.DeserializeObject<List<MimeType>>(File.ReadAllText(jsonFile)),
                TimeSpan.FromDays(30));
        }
    }
}