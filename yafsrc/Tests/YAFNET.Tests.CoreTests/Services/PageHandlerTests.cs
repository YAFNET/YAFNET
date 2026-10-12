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

using System.IO;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Every button or form of a page that calls a handler of its own page (asp-page-handler) has a matching
/// public OnGet... / OnPost... method in the page model (a misspelled or non public handler is never called).
/// </summary>
[TestFixture]
public partial class PageHandlerTests
{
    [Test]
    [Description("All asp-page-handler names of the pages exist as public handlers of the page models")]
    public void All_Page_Handlers_Exist_Test()
    {
        var root = FindSourceRoot();

        if (root is null)
        {
            Assert.Ignore("The source code is not available next to the tests.");
        }

        var missing = new List<string>();
        var checkedHandlers = 0;

        foreach (var view in Directory.EnumerateFiles(Path.Combine(root, "YetAnotherForum.NET", "Pages"), "*.cshtml", SearchOption.AllDirectories))
        {
            var model = $"{view}.cs";

            // partial views are used by other pages
            if (!File.Exists(model))
            {
                continue;
            }

            var handlers = Handler().Matches(Read(model)).Select(m => m.Groups["name"].Value.ToLowerInvariant()).ToHashSet();

            foreach (Match tag in Tag().Matches(Read(view)))
            {
                var handler = PageHandler().Match(tag.Value);

                // a handler of another page or a controller action
                if (!handler.Success || tag.Value.Contains("asp-page=") || tag.Value.Contains("asp-controller=")
                    || tag.Value.Contains("asp-action="))
                {
                    continue;
                }

                checkedHandlers++;

                if (!handlers.Contains(handler.Groups["name"].Value.ToLowerInvariant()))
                {
                    missing.Add($"{Path.GetRelativePath(root, view)}: {handler.Groups["name"].Value}");
                }
            }
        }

        checkedHandlers.Should().BeGreaterThan(100, "the pages are found (guards the test against checking nothing)");
        string.Join(", ", missing).Should().BeEmpty("these buttons call a handler that does not exist");
    }

    private static string Read(string path)
    {
        var bytes = File.ReadAllBytes(path);

        try
        {
            return new UTF8Encoding(false, true).GetString(bytes);
        }
        catch (DecoderFallbackException)
        {
            // some files are not saved as utf-8
            return Encoding.Latin1.GetString(bytes);
        }
    }

    private static string FindSourceRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

        while (directory != null && !Directory.Exists(Path.Combine(directory.FullName, "YetAnotherForum.NET", "Pages")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName;
    }

    [GeneratedRegex(@"public\s+[^\n(]*\bOn(?:Get|Post)(?<name>\w*?)(?:Async)?\s*\(")]
    private static partial Regex Handler();

    [GeneratedRegex(@"<(?:button|form|a|input)\b[^>]*>", RegexOptions.Singleline)]
    private static partial Regex Tag();

    [GeneratedRegex(@"asp-page-handler=""(?<name>[A-Za-z0-9_]+)""")]
    private static partial Regex PageHandler();
}
