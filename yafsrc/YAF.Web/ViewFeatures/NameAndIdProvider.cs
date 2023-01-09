/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Web.ViewFeatures;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

// NOTE: This is copied from .net core 2.0 code as this is removed from .net core 3.0
internal static class NameAndIdProvider
{
    private static readonly object PreviousNameAndIdKey = typeof(PreviousNameAndId);

    /// <summary>
    /// Returns a valid HTML 4.01 "id" attribute value for an element with the given <paramref name="fullName"/>.
    /// </summary>
    /// <param name="viewContext">A <see cref="ViewContext"/> instance for the current scope.</param>
    /// <param name="fullName">
    /// The fully-qualified expression name, ignoring the current model. Also the original HTML element name.
    /// </param>
    /// <param name="invalidCharReplacement">
    /// The <see cref="string"/> (normally a single <see cref="char"/>) to substitute for invalid characters in
    /// <paramref name="fullName"/>.
    /// </param>
    /// <returns>
    /// Valid HTML 4.01 "id" attribute value for an element with the given <paramref name="fullName"/>.
    /// </returns>
    /// <remarks>
    /// Similar to <see cref="TagBuilder.CreateSanitizedId"/> but caches value for repeated invocations.
    /// </remarks>
    public static string CreateSanitizedId(ViewContext viewContext, string fullName, string invalidCharReplacement)
    {
        if (viewContext == null)
        {
            throw new ArgumentNullException(nameof(viewContext));
        }

        if (invalidCharReplacement == null)
        {
            throw new ArgumentNullException(nameof(invalidCharReplacement));
        }

        if (string.IsNullOrEmpty(fullName))
        {
            return string.Empty;
        }

        // Check cache to avoid whatever TagBuilder.CreateSanitizedId() may do.
        var items = viewContext.HttpContext.Items;
        PreviousNameAndId previousNameAndId = null;
        if (items.TryGetValue(PreviousNameAndIdKey, out var previousNameAndIdObject) &&
            (previousNameAndId = (PreviousNameAndId)previousNameAndIdObject) != null &&
            string.Equals(previousNameAndId.FullName, fullName, StringComparison.Ordinal))
        {
            return previousNameAndId.SanitizedId;
        }

        var sanitizedId = TagBuilder.CreateSanitizedId(fullName, invalidCharReplacement);

        if (previousNameAndId == null)
        {
            // Do not create a PreviousNameAndId when TagBuilder.CreateSanitizedId() only examined fullName.
            if (string.Equals(fullName, sanitizedId, StringComparison.Ordinal))
            {
                return sanitizedId;
            }

            previousNameAndId = new PreviousNameAndId();
            items[PreviousNameAndIdKey] = previousNameAndId;
        }

        previousNameAndId.FullName = fullName;
        previousNameAndId.SanitizedId = sanitizedId;

        return previousNameAndId.SanitizedId;
    }

    /// <summary>
    /// Adds a valid HTML 4.01 "id" attribute for an element with the given <paramref name="fullName"/>. Does
    /// nothing if <see cref="TagBuilder.Attributes"/> already contains an "id" attribute or the
    /// <paramref name="fullName"/> is <c>null</c> or empty.
    /// </summary>
    /// <param name="viewContext">A <see cref="ViewContext"/> instance for the current scope.</param>
    /// <param name="tagBuilder">A <see cref="TagBuilder"/> instance that will contain the "id" attribute.</param>
    /// <param name="fullName">
    /// The fully-qualified expression name, ignoring the current model. Also the original HTML element name.
    /// </param>
    /// <param name="invalidCharReplacement">
    /// The <see cref="string"/> (normally a single <see cref="char"/>) to substitute for invalid characters in
    /// <paramref name="fullName"/>.
    /// </param>
    /// <remarks>
    /// Similar to <see cref="TagBuilder.GenerateId"/> but caches value for repeated invocations.
    /// </remarks>
    /// <seealso cref="CreateSanitizedId"/>
    public static void GenerateId(
        ViewContext viewContext,
        TagBuilder tagBuilder,
        string fullName,
        string invalidCharReplacement)
    {
        if (viewContext == null)
        {
            throw new ArgumentNullException(nameof(viewContext));
        }

        if (tagBuilder == null)
        {
            throw new ArgumentNullException(nameof(tagBuilder));
        }

        if (invalidCharReplacement == null)
        {
            throw new ArgumentNullException(nameof(invalidCharReplacement));
        }

        if (string.IsNullOrEmpty(fullName))
        {
            return;
        }

        if (!tagBuilder.Attributes.ContainsKey("id"))
        {
            var sanitizedId = CreateSanitizedId(viewContext, fullName, invalidCharReplacement);

            // Duplicate check for null or empty to cover the corner case where fullName contains only invalid
            // characters and invalidCharReplacement is empty.
            if (!string.IsNullOrEmpty(sanitizedId))
            {
                tagBuilder.Attributes["id"] = sanitizedId;
            }
        }
    }

    /// <summary>
    /// Returns the full HTML element name for the specified <paramref name="expression"/>.
    /// </summary>
    /// <param name="viewContext">A <see cref="ViewContext"/> instance for the current scope.</param>
    /// <param name="expression">Expression name, relative to the current model.</param>
    /// <returns>Fully-qualified expression name for <paramref name="expression"/>.</returns>
    /// <remarks>
    /// Similar to <see cref="TemplateInfo.GetFullHtmlFieldName"/> but caches value for repeated invocations.
    /// </remarks>
    public static string GetFullHtmlFieldName(ViewContext viewContext, string expression)
    {
        var htmlFieldPrefix = viewContext.ViewData.TemplateInfo.HtmlFieldPrefix;
        if (string.IsNullOrEmpty(expression))
        {
            return htmlFieldPrefix;
        }

        if (string.IsNullOrEmpty(htmlFieldPrefix))
        {
            return expression;
        }

        // Need to concatenate. See if we've already done that.
        var items = viewContext.HttpContext.Items;
        PreviousNameAndId previousNameAndId = null;
        if (items.TryGetValue(PreviousNameAndIdKey, out var previousNameAndIdObject) &&
            (previousNameAndId = (PreviousNameAndId)previousNameAndIdObject) != null &&
            string.Equals(previousNameAndId.HtmlFieldPrefix, htmlFieldPrefix, StringComparison.Ordinal) &&
            string.Equals(previousNameAndId.Expression, expression, StringComparison.Ordinal))
        {
            return previousNameAndId.OutputFullName;
        }

        if (previousNameAndId == null)
        {
            previousNameAndId = new PreviousNameAndId();
            items[PreviousNameAndIdKey] = previousNameAndId;
        }

        previousNameAndId.HtmlFieldPrefix = htmlFieldPrefix;
        previousNameAndId.Expression = expression;
        if (expression.StartsWith("[", StringComparison.Ordinal))
        {
            // The expression might represent an indexer access, in which case  with a 'dot' would be invalid.
            previousNameAndId.OutputFullName = htmlFieldPrefix + expression;
        }
        else
        {
            previousNameAndId.OutputFullName = htmlFieldPrefix + "." + expression;
        }

        return previousNameAndId.OutputFullName;
    }

    private class PreviousNameAndId
    {
        // Cached ambient input for NameAndIdProvider.GetFullHtmlFieldName(). TemplateInfo.HtmlFieldPrefix may
        // change during the lifetime of a ViewContext.
        public string HtmlFieldPrefix { get; set; }

        // Cached input for NameAndIdProvider.GetFullHtmlFieldName().
        public string Expression { get; set; }

        // Cached return value for NameAndIdProvider.GetFullHtmlFieldName().
        public string OutputFullName { get; set; }

        // Cached input for NameAndIdProvider.CreateSanitizedId(). Since IHtmlHelper.GenerateIdFromName() is
        // available to all, there is no guarantee this is equal to OutputFullName when CreateSanitizedId() is
        // called.
        public string FullName { get; set; }

        // Cached return value for NameAndIdProvider.CreateSanitizedId().
        public string SanitizedId { get; set; }
    }
}