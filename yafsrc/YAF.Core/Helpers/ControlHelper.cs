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

namespace YAF.Core.Helpers;

using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

/// <summary>
/// Provides helper functions for using and accessing controls.
/// </summary>
public static class ControlHelper
{
    /// <summary>
    /// Renders a control to a string.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <returns>Returns the Rendered Control as string</returns>
    public static string RenderToString(this Control control)
    {
        (control as IRaiseControlLifeCycles)?.RaiseLoad();

        if (!control.Visible)
        {
            return string.Empty;
        }

        using var stringWriter = new StringWriter();
        using var writer = new HtmlTextWriter(stringWriter);
        (control as IRaiseControlLifeCycles)?.RaisePreRender();

        control.RenderControl(writer);
        return stringWriter.ToString();
    }

    /// <summary>
    /// Finds the control recursive reverse.
    /// </summary>
    /// <param name="sourceControl">The source control.</param>
    /// <param name="id">The id.</param>
    /// <returns>
    /// The find control recursive reverse.
    /// </returns>
    public static Control FindControlRecursiveReverse(this Control sourceControl, string id)
    {
        var foundControl = sourceControl.FindControl(id);

        if (foundControl != null)
        {
            return foundControl;
        }

        return sourceControl.Parent != null ? FindControlRecursiveReverse(sourceControl.Parent, id) : null;
    }

    /// <summary>
    /// Finds the control recursive both.
    /// </summary>
    /// <param name="sourceControl">The source control.</param>
    /// <param name="id">The id.</param>
    /// <returns>
    /// The find control recursive both.
    /// </returns>
    public static Control FindControlRecursiveBoth(this Control sourceControl, string id)
    {
        var found = FindControlRecursiveReverse(sourceControl, id);
        if (found != null)
        {
            return found;
        }

        found = FindControlRecursive(sourceControl, id);
        return found;
    }

    /// <summary>
    /// Makes Find Control strongly typed.
    /// </summary>
    /// <param name="sourceControl">
    /// The source control.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <typeparam name="T">
    /// the type parameter
    /// </typeparam>
    /// <returns>
    /// Returns the Control as strongly typed
    /// </returns>
    public static T FindControlAs<T>(this Control sourceControl, string id) where T : class
    {
        var foundControl = sourceControl.FindControl(id);

        return foundControl is T ? foundControl.ToClass<T>() : null;
    }

    /// <summary>
    /// Finds the control recursive as.
    /// </summary>
    /// <typeparam name="T">the type parameter</typeparam>
    /// <param name="sourceControl">The source control.</param>
    /// <param name="id">The id.</param>
    /// <returns>
    /// The find control recursive as.
    /// </returns>
    public static T FindControlRecursiveAs<T>(this Control sourceControl, string id)
        where T : class
    {
        var foundControl = FindControlRecursive(sourceControl, id);

        return foundControl is T ? foundControl.ToClass<T>() : null;
    }

    /// <summary>
    /// Finds the control recursive both as.
    /// </summary>
    /// <typeparam name="T">the type parameter</typeparam>
    /// <param name="sourceControl">The source control.</param>
    /// <param name="id">The id.</param>
    /// <returns>
    /// The find control recursive both as.
    /// </returns>
    public static T FindControlRecursiveBothAs<T>(this Control sourceControl, string id)
        where T : class
    {
        var foundControl = FindControlRecursiveBoth(sourceControl, id);

        return foundControl is T ? foundControl.ToClass<T>() : null;
    }

    /// <summary>
    /// Find Wizard Control - Find a control in a wizard
    /// </summary>
    /// <param name="wizardControl">
    /// Wizard control
    /// </param>
    /// <param name="id">
    /// ID of target control
    /// </param>
    /// <returns>
    /// A control reference, if found, null, if not
    /// </returns>
    public static Control FindWizardControlRecursive(this Wizard wizardControl, string id)
    {
        Control foundControl = null;

        for (var i = 0; i < wizardControl.WizardSteps.Count; i++)
        {
            for (var j = 0; j < wizardControl.WizardSteps[i].Controls.Count; j++)
            {
                foundControl = FindControlRecursive(wizardControl.WizardSteps[i].Controls[j], id);
                if (foundControl != null)
                {
                    break;
                }
            }

            if (foundControl != null)
            {
                break;
            }
        }

        return foundControl;
    }

    /// <summary>
    /// Find Wizard Control - Find a control in a wizard, is recursive
    /// </summary>
    /// <param name="sourceControl">
    /// Source/Root Control
    /// </param>
    /// <param name="id">
    /// ID of target control
    /// </param>
    /// <returns>
    /// A Control, if found; null, if not
    /// </returns>
    public static Control FindControlRecursive(this Control sourceControl, string id)
    {
        var foundControl = sourceControl.FindControl(id);

        if (foundControl != null)
        {
            return foundControl;
        }

        if (!sourceControl.HasControls())
        {
            return null;
        }

        foreach (Control tmpCtr in sourceControl.Controls)
        {
            // Check all child controls of sourceControl
            foundControl = FindControlRecursive(tmpCtr, id);
            if (foundControl != null)
            {
                break;
            }
        }

        return foundControl;
    }

    /// <summary>
    /// Finds all controls in <paramref name="sourceControl"/> of type T.
    /// </summary>
    /// <param name="sourceControl">
    /// Control to search within.
    /// </param>
    /// <typeparam name="T">Type to Find and Return
    /// </typeparam>
    /// <returns>
    /// List of type T with controls.
    /// </returns>
    public static IEnumerable<T> FindControlType<T>(this Control sourceControl)
    {
        return sourceControl.Controls.OfType<T>();
    }

    /// <summary>
    /// Makes the CSS include control.
    /// </summary>
    /// <param name="href">The href.</param>
    /// <returns>
    /// The make CSS include control.
    /// </returns>
    public static HtmlLink MakeCssIncludeControl(string href)
    {
        var stylesheet = new HtmlLink { Href = href };
        stylesheet.Attributes.Add("rel", "stylesheet");
        stylesheet.Attributes.Add("type", "text/css");

        return stylesheet;
    }

    /// <summary>
    /// The make CSS control.
    /// </summary>
    /// <param name="css">
    /// The style information to add to the control.
    /// </param>
    /// <returns>
    /// Returns the CSS control
    /// </returns>
    public static HtmlGenericControl MakeCssControl(string css)
    {
        var style = new HtmlGenericControl { TagName = "style" };
        style.Attributes.Add("type", "text/css");
        style.InnerHtml = css;

        return style;
    }

    /// <summary>
    /// Creates a <see cref="HtmlMeta"/> control robots no-index.
    /// </summary>
    /// <returns><see cref="HtmlMeta"/> control</returns>
    public static HtmlMeta MakeMetaNoIndexControl()
    {
        var meta = new HtmlMeta { Name = "robots", Content = "noindex,follow" };

        return meta;
    }
}