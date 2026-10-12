/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
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

namespace YAF.Core.Helpers;

using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
/// Validates the values of the custom profile fields.
/// </summary>
public static class CustomProfileHelper
{
    /// <summary>
    /// Finds the first custom profile field with a missing or invalid value.
    /// The required flags and the data types are taken from the field definitions of the board (from the database),
    /// not from the posted values (they are sent by the client).
    /// </summary>
    /// <param name="definitions">The field definitions of the board.</param>
    /// <param name="values">The posted values (the ids and values), can be null.</param>
    /// <param name="checkRequired">if set to <c>true</c> the required fields must have a value.</param>
    /// <param name="isNumberError">Returns true if the value of the field is not a number.</param>
    /// <returns>Returns the definition of the invalid field, or null if all values are valid.</returns>
    public static ProfileDefinition FindInvalidField(
        IEnumerable<ProfileDefinition> definitions,
        IEnumerable<ProfileDefinition> values,
        bool checkRequired,
        out bool isNumberError)
    {
        isNumberError = false;

        var posted = values?.ToList() ?? [];

        foreach (var definition in definitions)
        {
            var type = definition.DataType.ToEnum<DataType>();

            if (type is not (DataType.Text or DataType.Number))
            {
                continue;
            }

            var value = posted.FirstOrDefault(v => v.ID == definition.ID)?.Value;

            if (value.IsNotSet() || value.Trim().Length == 0)
            {
                if (checkRequired && definition.Required)
                {
                    return definition;
                }

                continue;
            }

            if (type is DataType.Number && !decimal.TryParse(
                    value.Trim(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out _))
            {
                isNumberError = true;
                return definition;
            }
        }

        return null;
    }
}
