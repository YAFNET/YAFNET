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

namespace YAF.Tests.TestInfrastructure;

using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;

/// <summary>
/// Changes appSettings of the configuration (what <see cref="YAF.Configuration.Config"/> reads from the web.config)
/// for a test. The settings are process wide, so fixtures using this class must be <c>[NonParallelizable]</c>.
/// <code>
/// using var settings = TestAppSettings.Set("YAF.URLRewritingMode", "Translit");
/// </code>
/// </summary>
public sealed class TestAppSettings : IDisposable
{
    private static readonly PropertyInfo IsReadOnlyProperty =
        typeof(NameObjectCollectionBase).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

    // the read only flags of the configuration element and of the element collection
    private static readonly FieldInfo[] ElementReadOnlyFields =
    [
        typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic),
        typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic)
    ];

    private readonly Dictionary<string, string> previousValues = new();

    private TestAppSettings()
    {
    }

    /// <summary>
    /// Sets an app setting (<c>null</c> removes it) until the returned instance is disposed.
    /// </summary>
    public static TestAppSettings Set(string key, string value)
    {
        return new TestAppSettings().And(key, value);
    }

    /// <summary>
    /// Sets another app setting (<c>null</c> removes it) until this instance is disposed.
    /// </summary>
    public TestAppSettings And(string key, string value)
    {
        if (!this.previousValues.ContainsKey(key))
        {
            this.previousValues[key] = ConfigurationManager.AppSettings[key];
        }

        Write(key, value);

        return this;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var entry in this.previousValues)
        {
            Write(entry.Key, entry.Value);
        }
    }

    private static void Write(string key, string value)
    {
        var settings = ConfigurationManager.AppSettings;

        // outside of a web application the collection writes into the (read only) appSettings section
        var section = settings.GetType().GetField("_root", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(settings) as AppSettingsSection;

        IsReadOnlyProperty.SetValue(settings, false);

        SetElementReadOnly(section, false);

        try
        {
            if (value is null)
            {
                settings.Remove(key);
            }
            else
            {
                settings[key] = value;
            }
        }
        finally
        {
            IsReadOnlyProperty.SetValue(settings, true);

            SetElementReadOnly(section, true);
        }
    }

    private static void SetElementReadOnly(AppSettingsSection section, bool readOnly)
    {
        if (section is null)
        {
            return;
        }

        foreach (var field in ElementReadOnlyFields.Where(f => f != null))
        {
            field.SetValue(section.Settings, readOnly);
        }
    }
}
