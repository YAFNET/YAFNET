/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using System.IO;
  using System.Linq;
  using System.Reflection;

  using YAF.Types;
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
      return GetValidateAssemblies(GetMatchingFiles(pattern)).Select(Assembly.Load).ToList();
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
    private static IEnumerable<AssemblyName> GetValidateAssemblies([NotNull] IEnumerable<string> filenames)
    {
      var temporaryDomain = AppDomain.CreateDomain(
        "YafScannerDomain", AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);

      try
      {
        foreach (var assemblyFile in filenames)
        {
          Assembly assembly;

          if (File.Exists(assemblyFile))
          {
            try
            {
              var name = new AssemblyName { CodeBase = assemblyFile };
              assembly = temporaryDomain.Load(name);
            }
            catch (BadImageFormatException)
            {
              // fail on native images...
              continue;
            }
          }
          else
          {
            assembly = temporaryDomain.Load(assemblyFile);
          }

          yield return assembly.GetName();
        }
      }
      finally
      {
        AppDomain.Unload(temporaryDomain);
      }
    }

    #endregion
  }
}