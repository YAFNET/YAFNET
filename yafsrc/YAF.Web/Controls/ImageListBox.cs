/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
/// Custom DropDown List Controls with Images
/// </summary>
public class ImageListBox : DropDownList
{
    /// <summary>
    /// Gets or sets the place holder.
    /// </summary>
    [DefaultValue("")]
    public string PlaceHolder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether allow clear.
    /// </summary>
    [DefaultValue(false)]
    public bool AllowClear { get; set; }

    /// <summary>
    /// Add Flag Images to Items
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
        if (this.PlaceHolder.IsSet())
        {
            this.Attributes.Add("placeholder", this.PlaceHolder);
        }

        this.Attributes.Add("data-allow-clear", this.AllowClear.ToString());

        this.Items.Cast<ListItem>().Where(item => item.Value.IsSet()).ForEach(
            item => item.Attributes.Add(
                "data-content",
                $"<span class=\"select2-image-select-icon\"><img src=\"{item.Value.ToLower()}\" />{item.Text}</span>"));

        base.Render(writer);
    }
}