/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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

using System;

namespace YAF.Core
{
  using YAF.Classes.Data;

  public static class BinaryExtensions
  {
    /// <summary>
    /// The binary and.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="checkAgainst">
    /// The check against.
    /// </param>
    /// <returns>
    /// The binary and.
    /// </returns>
    public static bool BinaryAnd(this int value, int checkAgainst)
    {
      return (value & checkAgainst) == checkAgainst;
    }

    /// <summary>
    /// The binary and.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="checkAgainst">
    /// The check against.
    /// </param>
    /// <returns>
    /// The binary and.
    /// </returns>
    public static bool BinaryAnd(this object value, object checkAgainst)
    {
        return BinaryAnd(Convert.ToInt32(value), Convert.ToInt32(checkAgainst));
    }    
  }
}