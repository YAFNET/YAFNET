﻿/* Yet Another Forum.NET
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

namespace YAF.Web.TagHelpers;

/// <summary>
/// Helper to inject the inline scripts in to the body tag
/// </summary>
[HtmlTargetElement("body")]
public class ScriptsTagHelper : TagHelper, IHaveServiceLocator
{
    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="output">
    /// The output.
    /// </param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var items = BoardContext.Current.InlineElements.Items;

        var count = items.Count(x => x.Type == InlineType.Script);

        if (count > 0)
        {
            output
                .PostContent
                .AppendHtml("<script>");
        }

        // Inject Inline Scripts
        items.ForEach(
            script =>
                {
                    if (!script.IsInjected && script.Type == InlineType.Script)
                    {
                        output
                            .PostContent
                            .AppendHtml(script.Code);
                    }

                    script.IsInjected = true;
                });

        if (count > 0)
        {
            output
                .PostContent
                .AppendHtml("</script>");
        }
    }
}