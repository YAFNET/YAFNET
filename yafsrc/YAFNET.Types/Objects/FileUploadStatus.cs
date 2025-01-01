/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Types.Objects;

/// <summary>
/// FileUploadStatus Class
/// </summary>
public class FilesUploadStatus
{
    /// <summary>
    /// The handler path
    /// </summary>
    public const string HandlerPath = "/";

    /// <summary>
    /// Initializes a new instance of the <see cref="FilesUploadStatus"/> class.
    /// </summary>
    public FilesUploadStatus()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilesUploadStatus"/> class.
    /// </summary>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    /// <param name="fileLength">
    /// Length of the file.
    /// </param>
    /// <param name="fileID">
    /// The file ID.
    /// </param>
    public FilesUploadStatus(string fileName, int fileLength, int fileID)
    {
        this.SetValues(fileName, fileLength, fileID);
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string name { get; set; }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public int size { get; set; }

    /// <summary>
    /// Gets or sets the progress.
    /// </summary>
    /// <value>
    /// The progress.
    /// </value>
    public string progress { get; set; }

    /// <summary>
    /// Gets or sets the file identifier.
    /// </summary>
    /// <value>
    /// The file identifier.
    /// </value>
    public int fileID { get; set; }

    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    /// <value>
    /// The error.
    /// </value>
    public string error { get; set; }

    /// <summary>
    /// Sets the values.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="fileLength">Length of the file.</param>
    /// <param name="fileId">The file identifier.</param>
    private void SetValues(string fileName, int fileLength, int fileId)
    {
        this.name = fileName;
        this.size = fileLength;
        this.progress = "1.0";
        this.fileID = fileId;
    }
}