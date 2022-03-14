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

namespace YAF.Core.Extensions
{
    #region Using

    using System;
    using System.Web.UI;

    using YAF.Core.Context;
    using YAF.Core.Utilities.StringUtils;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// The control extensions.
    /// </summary>
    public static class ControlExtensions
    {
        #region Public Methods

        /// <summary>
        /// Creates a Unique ID
        /// </summary>
        /// <param name="currentControl">The current Control.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>
        /// The get unique id.
        /// </returns>
        public static string GetUniqueID(this Control currentControl, string prefix)
        {
            return prefix.IsSet()
                       ? $"{prefix}{Guid.NewGuid().ToString().Substring(0, 5)}"
                       : Guid.NewGuid().ToString().Substring(0, 10);
        }

        /// <summary>
        /// XSS Encode input String.
        /// </summary>
        /// <param name="currentControl">The current Control.</param>
        /// <param name="data">The data.</param>
        /// <returns>
        /// The encoded string.
        /// </returns>
        public static string HtmlEncode(this Control currentControl, object data)
        {
            if (data is null)
            {
                return null;
            }

            return BoardContext.Current != null
                       ? new UnicodeEncoder().XSSEncode(data.ToString())
                       : BoardContext.Current.CurrentForumPage.HtmlEncode(data.ToString());
        }

        /// <summary>
        /// Gets PageBoardContext.
        /// </summary>
        /// <param name="currentControl">
        /// The current Control.
        /// </param>
        /// <returns>
        /// The <see cref="BoardContext"/>.
        /// </returns>
        public static BoardContext PageBoardContext(this Control currentControl)
        {
            return currentControl.Site is { DesignMode: true } ? null : BoardContext.Current;
        }

        /// <summary>
        /// Loads a user control with a constructor with a signature matching the supplied params
        /// Control must implement a blank default constructor as well as the custom one or we will error
        /// </summary>
        /// <param name="templateControl">Template control base object</param>
        /// <param name="controlPath">Path to the user control</param>
        /// <param name="constructorParams">Parameters for the constructor</param>
        /// <returns></returns>
        public static UserControl LoadControl(this TemplateControl templateControl, string controlPath, params object[] constructorParams)
        {
            // Load the control
            var control = templateControl.LoadControl(controlPath) as UserControl;

            // Get the types for the passed parameters
            var paramTypes = new Type[constructorParams.Length];
            for (var paramLoop = 0; paramLoop < constructorParams.Length; paramLoop++)
            {
                paramTypes[paramLoop] = constructorParams[paramLoop].GetType();
            }

            // Get the constructor that matches our signature
            var constructor = control.GetType().BaseType.GetConstructor(paramTypes);

            // Call the constructor if we found it, otherwise throw
            if (constructor is null)
            {
                throw new ArgumentException("Required constructor signature not found.");
            }

            constructor.Invoke(control, constructorParams);

            return control;
        }

        #endregion
    }
}