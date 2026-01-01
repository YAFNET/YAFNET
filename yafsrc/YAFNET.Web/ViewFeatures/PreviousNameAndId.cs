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

namespace YAF.Web.ViewFeatures;

/// <summary>
/// Class PreviousNameAndId. This class cannot be inherited.
/// </summary>
public sealed class PreviousNameAndId
{
    // Cached ambient input for NameAndIdProvider.GetFullHtmlFieldName(). TemplateInfo.HtmlFieldPrefix may
    // change during the lifetime of a ViewContext.
    /// <summary>
    /// Gets or sets the HTML field prefix.
    /// </summary>
    /// <value>The HTML field prefix.</value>
    public string HtmlFieldPrefix { get; set; }

    // Cached input for NameAndIdProvider.GetFullHtmlFieldName().
    /// <summary>
    /// Gets or sets the expression.
    /// </summary>
    /// <value>The expression.</value>
    public string Expression { get; set; }

    // Cached return value for NameAndIdProvider.GetFullHtmlFieldName().
    /// <summary>
    /// Gets or sets the full name of the output.
    /// </summary>
    /// <value>The full name of the output.</value>
    public string OutputFullName { get; set; }

    // Cached input for NameAndIdProvider.CreateSanitizedId(). Since IHtmlHelper.GenerateIdFromName() is
    // available to all, there is no guarantee this is equal to OutputFullName when CreateSanitizedId() is
    // called.
    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    /// <value>The full name.</value>
    public string FullName { get; set; }

    // Cached return value for NameAndIdProvider.CreateSanitizedId().
    /// <summary>
    /// Gets or sets the sanitized identifier.
    /// </summary>
    /// <value>The sanitized identifier.</value>
    public string SanitizedId { get; set; }
}