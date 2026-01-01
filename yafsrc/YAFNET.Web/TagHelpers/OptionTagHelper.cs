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

namespace YAF.Web.TagHelpers;

/// <summary>
/// The body direction helper.
/// </summary>
[HtmlTargetElement("option", Attributes = "item")]
public class OptionTagHelper : TagHelper, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionTagHelper"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public OptionTagHelper(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    [HtmlAttributeName("item")]
    public SelectListItem Item { get; set; }

    /// <summary>
    /// Gets or sets the name of the icon.
    /// </summary>
    /// <value>The name of the icon.</value>
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is flag icon.
    /// </summary>
    public bool IsFlagIcon { get; set; }

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

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
        if (this.Item.Value.IsSet())
        {
            if (this.IsFlagIcon)
            {
                output.Attributes.SetAttribute(
                    "data-custom-properties",
                    $$"""{ "label": "<span class='fi fi-{{this.Item.Value.ToLower()}} me-1'></span>{{this.Item.Text}}" }""");
            }
            else
            {
                output.Attributes.SetAttribute(
                    "data-custom-properties",
                    this.IconName.IsSet()
                        ? $$"""{ "label": "<i class='fas fa-{{this.IconName}} text-secondary me-1'></i>{{this.Item.Text}}" }"""
                        : $$"""{ "label": "<img src='{{this.Item.Value}}' alt='{{this.Item.Text}}' />&nbsp;{{this.Item.Text}}" }""");
            }
        }

        if (this.Item.Selected)
        {
            output.Attributes.SetAttribute("selected", "selected");
        }
    }
}