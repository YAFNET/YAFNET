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

namespace YAF.Core.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc.Rendering;

/// <summary>
/// Provides helper functions for File handling
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// FileName Validator Expression
    /// </summary>
    private readonly static string FileNameValidatorExpression =
        $"^[^{string.Join(string.Empty, Array.ConvertAll(Path.GetInvalidFileNameChars(), x => Regex.Escape(x.ToString())))}]+$";

    /// <summary>
    /// FileName Validator Regex
    /// </summary>
    private readonly static Regex FileNameValidator = new(FileNameValidatorExpression, RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// FileName Cleaner Expression
    /// </summary>
    private readonly static string FileNameCleanerExpression =
        $"[{string.Join(string.Empty, Array.ConvertAll(Path.GetInvalidFileNameChars(), x => Regex.Escape(x.ToString())))}]";

    /// <summary>
    /// FileName Cleaner Regex
    /// </summary>
    private readonly static Regex FileNameCleaner = new(FileNameCleanerExpression, RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// Validates the name of the file.
    /// </summary>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    /// <returns>
    /// The validate file name.
    /// </returns>
    public static bool ValidateFileName(string fileName)
    {
        return FileNameValidator.IsMatch(fileName);
    }

    /// <summary>
    /// Cleans the name of the file.
    /// </summary>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    /// <returns>
    /// The clean file name.
    /// </returns>
    public static string CleanFileName(string fileName)
    {
        return FileNameCleaner.Replace(fileName, string.Empty);
    }

    /// <summary>
    /// The add image files.
    /// </summary>
    /// <param name="list">
    /// The list.
    /// </param>
    /// <param name="files">
    /// The files.
    /// </param>
    /// <param name="folder">
    /// The folder.
    /// </param>
    public static void AddImageFiles(
        this List<SelectListItem> list,
        List<FileInfo> files,
        string folder)
    {
        ArgumentNullException.ThrowIfNull(files);
        ArgumentNullException.ThrowIfNull(folder);

        files.Where(
            e => e.Extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase)
                 || e.Extension.Equals(".gif", StringComparison.InvariantCultureIgnoreCase)
                 || e.Extension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) || e.Extension.Equals(
                     ".svg",
                     StringComparison.InvariantCultureIgnoreCase)).ForEach(
            f =>
                {
                    var item = new SelectListItem(
                        f.Name,
                        BoardContext.Current.Get<IUrlHelper>().Content($"~/{folder}/{f.Name}"));

                    list.Add(item);
                });
    }
}