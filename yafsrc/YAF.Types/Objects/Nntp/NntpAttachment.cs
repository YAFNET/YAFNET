/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Types.Objects.Nntp
{
    using System.IO;

    /// <summary>
    /// The attachment.
    /// </summary>
    public class NntpAttachment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NntpAttachment"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="binaryData">
        /// The binary data.
        /// </param>
        public NntpAttachment(string id, string filename, byte[] binaryData)
        {
            this.Id = id;
            this.Filename = filename;
            this.BinaryData = binaryData;
        }

        /// <summary>
        /// Gets Id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets Filename.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Gets BinaryData.
        /// </summary>
        public byte[] BinaryData { get; }

        /// <summary>
        /// The save as.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public void SaveAs(string path)
        {
            this.SaveAs(path, false);
        }

        /// <summary>
        /// The save as.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="isOverwrite">
        /// The is overwrite.
        /// </param>
        public void SaveAs(string path, bool isOverwrite)
        {
            var fs = isOverwrite ? new FileStream(path, FileMode.Create) : new FileStream(path, FileMode.CreateNew);

            fs.Write(this.BinaryData, 0, this.BinaryData.Length);
            fs.Close();
        }
    }
}