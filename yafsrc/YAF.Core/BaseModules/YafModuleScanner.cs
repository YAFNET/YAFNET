/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Security;
  using System.Security.Permissions;
  using System.Security.Policy;

  using YAF.Types;
  using YAF.Types.Extensions;
  using YAF.Utils;

  /// <summary>
  /// The module scanner
  /// </summary>
  public class YafModuleScanner
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
      string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
      string searchPath = AppDomain.CurrentDomain.RelativeSearchPath;

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
      string path = CleanPath(Path.GetDirectoryName(pattern));
      string glob = Path.GetFileName(pattern);

      return Directory.GetFiles(path, glob);
    }

    /// <summary>
    /// The load and validate assemblies.
    /// </summary>
    /// <param name="filenames">
    /// The filenames.
    /// </param>
    /// <returns>
    /// </returns>
    private static IEnumerable<Assembly> GetValidateAssemblies([NotNull] IEnumerable<string> filenames)
    {
      CodeContracts.VerifyNotNull(filenames, "filenames");

      foreach (var assemblyFile in filenames.Where(File.Exists))
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