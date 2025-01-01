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

namespace YAF.Types.InputModels;

/// <summary>
/// The input model.
/// </summary>
public class EditBBCodeInputModel
{
    /// <summary>
    /// Gets or sets the bb code identifier.
    /// </summary>
    /// <value>The bb code identifier.</value>
    public int BBCodeId { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the execute order.
    /// </summary>
    /// <value>The execute order.</value>
    public int ExecOrder { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the on click js.
    /// </summary>
    /// <value>The on click js.</value>
    public string OnClickJS { get; set; }

    /// <summary>
    /// Gets or sets the display js.
    /// </summary>
    /// <value>The display js.</value>
    public string DisplayJS { get; set; }

    /// <summary>
    /// Gets or sets the edit js.
    /// </summary>
    /// <value>The edit js.</value>
    public string EditJS { get; set; }

    /// <summary>
    /// Gets or sets the display CSS.
    /// </summary>
    /// <value>The display CSS.</value>
    public string DisplayCSS { get; set; }

    /// <summary>
    /// Gets or sets the search reg ex.
    /// </summary>
    /// <value>The search reg ex.</value>
    public string SearchRegEx { get; set; }

    /// <summary>
    /// Gets or sets the replace reg ex.
    /// </summary>
    /// <value>The replace reg ex.</value>
    public string ReplaceRegEx { get; set; }

    /// <summary>
    /// Gets or sets the variables.
    /// </summary>
    /// <value>The variables.</value>
    public string Variables { get; set; }

    /// <summary>
    /// Gets or sets the module class.
    /// </summary>
    /// <value>The module class.</value>
    public string ModuleClass { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use module].
    /// </summary>
    /// <value><c>true</c> if [use module]; otherwise, <c>false</c>.</value>
    public bool UseModule { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use toolbar].
    /// </summary>
    /// <value><c>true</c> if [use toolbar]; otherwise, <c>false</c>.</value>
    public bool UseToolbar { get; set; }
}