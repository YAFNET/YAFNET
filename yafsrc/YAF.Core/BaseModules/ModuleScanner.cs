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
namespace YAF.Core.BaseModules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using YAF.Types;
    using YAF.Types.Extensions;

    /// <summary>
  /// The module scanner
  /// </summary>
  public class ModuleScanner
  {
    #region Public Methods

    /// <summary>
    /// The get modules.
    /// </summary>
    /// <param name="pattern">
    /// The pattern.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public IEnumerable<Assembly> GetModules([NotNull] string pattern)
    {
      var files = GetMatchingFiles(pattern).ToList();

      return GetValidateAssemblies(files).ToList();
    }

    #endregion

    #region Methods

    /// <summary>
    /// The clean path.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    /// <returns>
    /// The clean path.
    /// </returns>
    [NotNull]
    private static string CleanPath([NotNull] string path)
    {
      if (!Path.IsPathRooted(path))
      {
        path = Path.Combine(GetAppBaseDirectory(), path);
      }

      return Path.GetFullPath(path);
    }

    /// <summary>
    /// The get app base directory.
    /// </summary>
    /// <returns>
    /// The get app base directory.
    /// </returns>
    [NotNull]
    private static string GetAppBaseDirectory()
    {
      var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
      var searchPath = AppDomain.CurrentDomain.RelativeSearchPath;

      return searchPath.IsNotSet() ? baseDirectory : Path.Combine(baseDirectory, searchPath);
    }

    /// <summary>
    /// The get matching files.
    /// </summary>
    /// <param name="pattern">
    /// The pattern.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    private static IEnumerable<string> GetMatchingFiles([NotNull] string pattern)
    {
      var path = CleanPath(Path.GetDirectoryName(pattern));
      var glob = Path.GetFileName(pattern);

      return Directory.GetFiles(path, glob);
    }

    /// <summary>
    /// The load and validate assemblies.
    /// </summary>
    /// <param name="fileNames">
    /// The file Names.
    /// </param>
    /// <returns>
    /// </returns>
    private static IEnumerable<Assembly> GetValidateAssemblies([NotNull] IEnumerable<string> fileNames)
    {
      CodeContracts.VerifyNotNull(fileNames, "filenames");

      foreach (var assemblyFile in fileNames.Where(File.Exists))
      {
        Assembly assembly;

        try
        {
          assembly = Assembly.LoadFrom(assemblyFile);
        }
        catch (BadImageFormatException)
        {
          // fail on native images...
          continue;
        }

        yield return assembly;
      }
    }

    #endregion
  }
}