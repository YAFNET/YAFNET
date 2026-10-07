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

namespace YAF.Tests.CoreTests.TestInfrastructure;

using System.Globalization;

using YAF.Types.Interfaces;
using YAF.Types.Objects.Language;

/// <summary>
/// A deterministic <see cref="ILocalization"/> for unit tests.
/// Texts added with <see cref="Add"/> are returned as is (falling back to the same tag
/// on another page, like the real Localizer), everything else comes back as
/// <c>[PAGE.TAG]</c> so tests can assert which resource was used.
/// </summary>
public class FakeLocalization : ILocalization
{
    private readonly Dictionary<string, string> resources = new(StringComparer.OrdinalIgnoreCase);

    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

    public string LanguageCode => this.Culture.TwoLetterISOLanguageName;

    public string LanguageFileName => "english.json";

    public string TransPage { get; set; } = "DEFAULT";

    public bool TranslationLoaded => true;

    /// <summary>
    /// Adds (or replaces) a resource text.
    /// </summary>
    public FakeLocalization Add(string page, string tag, string text)
    {
        this.resources[Key(page, tag)] = text;
        return this;
    }

    public string FormatDateTime(string format, DateTime date)
    {
        return date.ToString(format, this.Culture);
    }

    public string FormatString(string format, params object[] args)
    {
        return string.Format(this.Culture, format, args);
    }

    public IEnumerable<Resource> GetNodesUsingQuery(string page, Func<Resource, bool> predicate)
    {
        return [];
    }

    public string GetText(string text)
    {
        return this.GetText(this.TransPage, text);
    }

    public string GetText(string page, string tag)
    {
        if (this.resources.TryGetValue(Key(page, tag), out var text))
        {
            return text;
        }

        // like the real Localizer: fall back to the tag on any other page
        var suffix = $".{tag}".ToUpperInvariant();
        var fallback = this.resources.FirstOrDefault(r => r.Key.EndsWith(suffix, StringComparison.Ordinal));

        return fallback.Key != null ? fallback.Value : $"[{Key(page, tag)}]";
    }

    public string GetText(string page, string tag, string languageFile)
    {
        return this.GetText(page, tag);
    }

    public string GetAttributeText(string text)
    {
        return this.GetText(text);
    }

    public string GetAttributeText(string page, string tag)
    {
        return this.GetText(page, tag);
    }

    public bool GetTextExists(string page, string tag)
    {
        return this.resources.ContainsKey(Key(page, tag));
    }

    public string GetTextFormatted(string text, params object[] args)
    {
        return this.FormatString(this.GetText(text), args);
    }

    public CultureInfo LoadTranslation(string fileName)
    {
        return this.Culture;
    }

    public CultureInfo LoadTranslation()
    {
        return this.Culture;
    }

    public LanguageResource LoadLanguageFile(string fileName)
    {
        return new LanguageResource();
    }

    private static string Key(string page, string tag)
    {
        return $"{page}.{tag}".ToUpperInvariant();
    }
}
