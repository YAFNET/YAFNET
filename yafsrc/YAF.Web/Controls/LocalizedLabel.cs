/* Yet Another Forum.NET
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
namespace YAF.Web.Controls;

/// <summary>
/// Makes a very simple localized label
/// </summary>
public class LocalizedLabel : BaseControl, ILocalizationSupport
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedLabel"/> class.
    /// </summary>
    public LocalizedLabel()
    {
        this.LocalizedPage = string.Empty;
        this.EnableBBCode = false;
        this.LocalizedTag = string.Empty;
        this.Param2 = string.Empty;
        this.Param1 = string.Empty;
        this.Param0 = string.Empty;
    }

    /// <summary>
    /// Gets or sets a value indicating whether EnableBBCode.
    /// </summary>
    public bool EnableBBCode { get; set; }

    /// <summary>
    /// Gets or sets LocalizedPage.
    /// </summary>
    public string LocalizedPage { get; set; }

    /// <summary>
    /// Gets or sets LocalizedTag.
    /// </summary>
    public string LocalizedTag { get; set; }

    /// <summary>
    /// Gets or sets Parameter 0.
    /// </summary>
    public string Param0 { get; set; }

    /// <summary>
    /// Gets or sets Parameter 1.
    /// </summary>
    public string Param1 { get; set; }

    /// <summary>
    /// Gets or sets Parameter 2.
    /// </summary>
    public string Param2 { get; set; }

    /// <summary>
    /// Shows the localized text string (if available)
    /// </summary>
    /// <param name="writer">The output.</param>
    override protected void Render(HtmlTextWriter writer)
    {
        writer.BeginRender();

        if (!this.DesignMode)
        {
            writer.Write(this.LocalizeAndRender(this));
        }
        else
        {
            writer.Write("[{0}][{1}]", this.LocalizedPage, this.LocalizedTag);
        }

        writer.EndRender();
    }
}