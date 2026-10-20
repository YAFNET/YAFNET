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

namespace YAF.Tests.Extensions;

using System.Reflection;

using YAF.Configuration;
using YAF.Configuration.Pattern;

/// <summary>
/// Every board setting is stored in the registry under its own key: a changed value can be read back,
/// also by a new <see cref="BoardSettings"/> over the same registry (that is how the settings are loaded).
/// </summary>
[TestFixture]
public class BoardSettingsTests
{
    private readonly static string[] NotInRegistry = ["BoardId", "Name", "Description", "Registry", "RegistryBoard"];

    private static IEnumerable<PropertyInfo> Settings() =>
        typeof(BoardSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0 && !NotInRegistry.Contains(p.Name))
            .OrderBy(p => p.Name);

    [Test]
    public void Settings_Are_Found_Test()
    {
        Settings().Should().HaveCountGreaterThan(150);
    }

    [TestCaseSource(nameof(Settings))]
    [Description("A changed setting is read back, also by a new settings instance over the same registry")]
    public void Setting_Round_Trip_Test(PropertyInfo setting)
    {
        var settings = new BoardSettings();
        var value = OtherValue(setting.PropertyType, setting.GetValue(settings));

        setting.SetValue(settings, value);

        setting.GetValue(settings).Should().Be(value, $"{setting.Name} keeps the value");

        var reloaded = new BoardSettings(1, "Board", "Description", settings.Registry, settings.RegistryBoard);
        setting.GetValue(reloaded).Should().Be(value, $"{setting.Name} is stored in the registry");
    }

    [Test]
    [Description("Every setting has a default, read only settings can be read from an empty registry")]
    public void Defaults_Test()
    {
        var settings = new BoardSettings();
        var failing = new List<string>();

        foreach (var property in typeof(BoardSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(p => p.CanRead && p.GetIndexParameters().Length == 0))
        {
            try
            {
                property.GetValue(settings);
            }
            catch (TargetInvocationException ex)
            {
                failing.Add($"{property.Name}: {ex.InnerException?.GetType().Name}");
            }
        }

        // the application id is created by the installer, without one there is no valid guid
        failing.Should().BeEquivalentTo(["ApplicationId: FormatException"]);
    }

    [Test]
    [Description("Board values override the host values of the same key")]
    public void Board_Overrides_Host_Test()
    {
        var host = new RegistryDictionaryOverride { ["LogError"] = "false" };
        var board = new RegistryDictionary();
        var settings = new BoardSettings(1, "Board", "Description", host, board);

        settings.LogError.Should().BeFalse("the host value is used");

        board["LogError"] = "true";
        settings.LogError.Should().BeTrue("the board value wins");

        board.Remove("LogError");
        settings.LogError.Should().BeFalse();
    }

    private static object OtherValue(Type type, object current)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;

        if (underlying == typeof(bool))
        {
            return !(bool)(current ?? false);
        }

        if (underlying == typeof(int))
        {
            return (int)(current ?? 0) + 7;
        }

        if (underlying == typeof(long))
        {
            return (long)(current ?? 0L) + 7;
        }

        if (underlying == typeof(double))
        {
            return (double)(current ?? 0d) + 0.5;
        }

        if (underlying == typeof(string))
        {
            return $"value-{current}-x";
        }

        if (underlying == typeof(DateTime))
        {
            return new DateTime(2026, 10, 9, 12, 0, 0);
        }

        if (underlying.IsEnum)
        {
            var values = Enum.GetValues(underlying).Cast<object>().ToList();
            return values.FirstOrDefault(v => !v.Equals(current)) ?? values[0];
        }

        throw new NotSupportedException($"No test value for {type}");
    }

    [Test]
    [Description("The host settings page reads and writes the settings by type: every setting is in one list")]
    public void Setting_Collection_Test()
    {
        var collection = new BoardSettingCollection(new BoardSettings());

        collection.SettingsString.Should().ContainKey(nameof(BoardSettings.Name));
        collection.SettingsBool.Should().ContainKey(nameof(BoardSettings.AllowDigestEmail));
        collection.SettingsInt.Should().ContainKey(nameof(BoardSettings.AvatarWidth));
        collection.SettingsOther.Should().ContainKeys(nameof(BoardSettings.MaxUsersWhen), nameof(BoardSettings.EditorEnterMode));

        var all = collection.SettingsString.Keys.Concat(collection.SettingsBool.Keys).Concat(collection.SettingsInt.Keys)
            .Concat(collection.SettingsOther.Keys).ToList();

        all.Should().OnlyHaveUniqueItems().And.BeEquivalentTo(typeof(BoardSettings).GetProperties().Select(p => p.Name));
    }
}

