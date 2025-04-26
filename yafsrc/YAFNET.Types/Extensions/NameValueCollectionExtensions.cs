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

namespace YAF.Types.Extensions;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

/// <summary>
/// The name value collection extensions.
/// </summary>
public static class NameValueCollectionExtensions
{
    /// <summary>
    /// Gets the first value of <paramref name="paramName"/> in the collection or default (Null).
    /// </summary>
    /// <param name="collection">
    /// The collection.
    /// </param>
    /// <param name="paramName">
    /// The parameter Name.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string GetFirstOrDefault(
        this NameValueCollection collection,
        string paramName)
    {
       return collection.GetValueList(paramName).FirstOrDefault();
    }

    /// <summary>
    /// Gets the value as an <see cref="IEnumerable"/> handling splitting the string if needed.
    /// </summary>
    /// <param name="collection">
    /// The collection.
    /// </param>
    /// <param name="paramName">
    /// The parameter Name.
    /// </param>
    /// <returns>
    /// Does not return null.
    /// </returns>
    public static IEnumerable<string> GetValueList(
        this NameValueCollection collection,
        string paramName)
    {
        return collection[paramName] is null
                   ? []
                   : collection[paramName].Split(',').AsEnumerable();
    }

    /// <summary>
    /// Check if Element Exists in the collection
    /// </summary>
    /// <param name="collection">
    /// The collection.
    /// </param>
    /// <param name="paramName">
    /// The parameter name.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public static bool Exists(this NameValueCollection collection, string paramName)
    {
        return collection[paramName] is not null;
    }
}