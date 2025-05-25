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

namespace YAF.Core.Services;

using System.Collections.Generic;

using YAF.Types.Objects;

/// <summary>
/// Helper Class providing functions to Insert Inline CSS or Script Blocks.
/// </summary>
public class InlineElements
{
    /// <summary>
    ///   Gets the Inline CSS or Script blocks
    /// </summary>
    public List<InlineItem> Items { get; } = [];

    /// <summary>
    /// Insert Internal Style sheet in the header
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="css">
    /// The CSS Code.
    /// </param>
    public void InsertInternalCss(string name, string css)
    {
        this.AddItem(
            new InlineItem
                {
                    Type = InlineType.Css,
                    Name = name,
                    Code = JsAndCssHelper.CompressCss(css),
                    IsInjected = false
                });
    }

    /// <summary>
    /// Insert Inline Script Block add the end of the Body Html Tag
    /// </summary>
    /// <param name="name">
    /// Unique name of JS Block
    /// </param>
    /// <param name="script">
    /// Script code to register
    /// </param>
    public void InsertJsBlock(string name, string script)
    {
        this.AddItem(
            new InlineItem
                {
                    Type = InlineType.Script,
                    Name = name,
                    Code = JsAndCssHelper.CompressJavaScript(script),
                    IsInjected = false
                });
    }

    /// <summary>
    /// The add item.
    /// </summary>
    /// <param name="script">
    /// The script.
    /// </param>
    private void AddItem(InlineItem script)
    {
        if (this.Items.All(item => item.Name != script.Name))
        {
            this.Items.Add(script);
        }
    }
}