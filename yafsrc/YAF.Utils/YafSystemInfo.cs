/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2017 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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
    /// Gets the number of memory bytes currently thought to be allocated.
    /// </summary>
    public static long AllocatedMemory
    {
      get
      {
          return GC.GetTotalMemory(false);
      }
    }

    /// <summary>
    /// Gets the amount of physical memory mapped to the process context.
    /// </summary>
    public static long MappedMemory
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