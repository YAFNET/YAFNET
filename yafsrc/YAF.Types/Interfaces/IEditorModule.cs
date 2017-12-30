/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
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
namespace YAF.Types.Interfaces
{
  /// <summary>
  /// IEditorModule Interface for Editor classes.
  /// </summary>
  public interface IEditorModule : IModuleDefinition
  {
    #region Properties

    /// <summary>
    /// Gets or sets StyleSheet.
    /// </summary>
    string StyleSheet { get; set; }

    /// <summary>
    /// Gets or sets Text.
    /// </summary>
    string Text { get; set; }

    /// <summary>
    /// Gets a value indicating whether UsesBBCode.
    /// </summary>
    bool UsesBBCode { get; }

    /// <summary>
    /// Gets a value indicating whether UsesHTML.
    /// </summary>
    bool UsesHTML { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Resolves the Url -- bad idea, needs to be replaced by an interface.
    /// </summary>
    /// <param name="relativeUrl">
    /// </param>
    /// <returns>
    /// The resolve url.
    /// </returns>
    string ResolveUrl([NotNull] string relativeUrl);

    #endregion
  }
}