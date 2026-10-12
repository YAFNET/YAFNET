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

namespace YAF.Tests.CoreTests.Services;

using System.Reflection;

using YAF.Configuration;
using YAF.Types.InputModels;

/// <summary>
/// The host settings page copies its fields to the board settings by name and type
/// (bool to bool, string to string, int or string to int): a field of another type is never saved.
/// </summary>
[TestFixture]
public class HostSettingsInputModelTests
{
    [Test]
    [Description("Every field of the host settings form has a type that is saved to the board setting of the same name")]
    public void Field_Types_Match_Board_Settings_Test()
    {
        var settings = typeof(BoardSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name);

        var mismatches = typeof(HostSettingsInputModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => settings.ContainsKey(p.Name))
            .Where(
                p =>
                    {
                        var settingType = settings[p.Name].PropertyType;

                        // enums are converted by the page itself
                        return !(p.PropertyType == settingType
                                 || settingType == typeof(int) && p.PropertyType == typeof(string)
                                 || settingType.IsEnum && p.PropertyType == typeof(int));
                    })
            .Select(p => $"{p.Name}: {p.PropertyType.Name} (board setting: {settings[p.Name].PropertyType.Name})")
            .ToList();

        string.Join(", ", mismatches).Should().BeEmpty("these settings could not be changed on the host settings page");
    }
}
