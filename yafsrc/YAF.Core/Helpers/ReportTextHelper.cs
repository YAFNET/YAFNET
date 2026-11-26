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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Helpers;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// The report text of a user and a post: the reports are stored as "date??text", joined with "|" (the date in the
/// invariant culture).
/// </summary>
public static class ReportTextHelper
{
    /// <summary>
    /// The separator of two reports: a "|" in front of the date of the next report (a "|" in the text of a report is
    /// part of the text, the text was split there).
    /// </summary>
    private static readonly Regex Separator = new(
        @"\|(?=\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}\?\?)",
        RegexOptions.None,
        RegexHelper.Timeout);

    /// <summary>
    /// Gets the reports of a report text.
    /// </summary>
    /// <param name="reportText">The report text.</param>
    /// <returns>The reports with the date (UTC, null if it is missing) and the text.</returns>
    public static List<(DateTime? Date, string Text)> Parse(string reportText)
    {
        var reports = new List<(DateTime? Date, string Text)>();

        if (string.IsNullOrWhiteSpace(reportText))
        {
            return reports;
        }

        foreach (var report in Separator.Split(reportText.Trim()))
        {
            // "date??text": only the first separator (a question mark in the text is kept)
            var separator = report.IndexOf("??", StringComparison.Ordinal);

            if (separator >= 0 && DateTime.TryParse(
                    report.Substring(0, separator),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out var date))
            {
                reports.Add((date, report.Substring(separator + 2)));
            }
            else
            {
                reports.Add((null, report));
            }
        }

        return reports;
    }
}
