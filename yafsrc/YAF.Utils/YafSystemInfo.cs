/* Yet Another Forum.NET
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
namespace YAF.Utils
{
  #region Using

  using System;

  using YAF.Types;

  #endregion

  /// <summary>
  /// The class gets common system info. Used in data layers other than MSSQL. Created by vzrus 2010
  /// </summary>
  public static class Platform
  {
    #region Constants and Fields

    /// <summary>
    /// The inited.
    /// </summary>
    private static bool inited;

    /// <summary>
    /// The is mono.
    /// </summary>
    private static bool isMono;

    #endregion

    #region Properties

    /// <summary>
    /// Gets AllocatedMemory.
    /// </summary>
    public static long AllocatedMemory
    {
      get
      {
        return Environment.WorkingSet;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsMono.
    /// </summary>
    public static bool IsMono
    {
      get
      {
        if (!inited)
        {
          Init();
        }

        return isMono;
      }
    }

    /// <summary>
    /// Gets Processors.
    /// </summary>
    [NotNull]
    public static string Processors
    {
      get
      {
        return Environment.ProcessorCount.ToString();
      }
    }

      /// <summary>
      /// Gets RuntimeName.
      /// </summary>
      [NotNull]
      public static string RuntimeName
      {
          get
          {
              if (!inited)
              {
                  Init();
              }

              return isMono ? "Mono" : ".NET";
          }
      }

      /// <summary>
    /// Gets RuntimeString.
    /// </summary>
    [NotNull]
    public static string RuntimeString
    {
      get
      {
        Version os = Environment.Version;
        return os.ToString();
      }
    }

    /// <summary>
    /// Gets VersionString.
    /// </summary>
    public static string VersionString
    {
      get
      {
        OperatingSystem os = Environment.OSVersion;
        return os.VersionString;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The init.
    /// </summary>
    private static void Init()
    {
      Type t = Type.GetType("Mono.Runtime");
      isMono = t != null;
    }

    #endregion
  }
}