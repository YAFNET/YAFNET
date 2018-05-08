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
namespace YAF.Utils.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

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
        [NotNull]
        public static string RenderToString([NotNull] this Control control)
        {
            CodeContracts.VerifyNotNull(control, "control");

            if (control is IRaiseControlLifeCycles)
            {
                (control as IRaiseControlLifeCycles).RaiseLoad();
            }

            if (!control.Visible)
            {
                return string.Empty;
            }

            using (var stringWriter = new StringWriter())
            {
                using (var writer = new HtmlTextWriter(stringWriter))
                {
                    if (control is IRaiseControlLifeCycles)
                    {
                        (control as IRaiseControlLifeCycles).RaisePreRender();
                    }

                    control.RenderControl(writer);
                    return stringWriter.ToString();
                }
            }
        }

        /// <summary>
        /// New user control.
        /// </summary>
        /// <typeparam name="T">the typeparameter</typeparam>
        /// <param name="control">The control.</param>
        /// <param name="controlPath">The control path.</param>
        /// <returns>Returns the new Control</returns>
        [CanBeNull]
        public static T NewUserControl<T>([NotNull] this UserControl control, [NotNull] string controlPath)
            where T : UserControl
        {
            CodeContracts.VerifyNotNull(control, "control");
            CodeContracts.VerifyNotNull(controlPath, "controlPath");

            var loaded = control.LoadControl(controlPath).ToClass<T>();

            if (loaded is IRaiseControlLifeCycles)
            {
                (loaded as IRaiseControlLifeCycles).RaiseInit();
            }

            return loaded;
        }

        /// <summary>
        /// Finds a control recursively (forward only) using <paramref name="isControl"/> function.
        /// </summary>
        /// <param name="sourceControl">
        /// Control to start search from.
        /// </param>
        /// <param name="isControl">
        /// Function to test if we found the control.
        /// </param>
        /// <returns>
        /// List of controls found
        /// </returns>
        [NotNull]
        public static List<Control> ControlListRecursive(
            [NotNull] this Control sourceControl,
            [NotNull] Func<Control, bool> isControl)
        {
            CodeContracts.VerifyNotNull(sourceControl, "sourceControl");
            CodeContracts.VerifyNotNull(isControl, "isControl");

            var list = new List<Control>();

            var withParents =
                (from c in sourceControl.Controls.Cast<Control>().AsQueryable() where c.HasControls() select c).ToList();

            // recusively call this function looking for controls...
            withParents.ForEach(x => list.AddRange(ControlListRecursive(x, isControl)));

            // add controls from this level...
            list.AddRange(ControlListNoParents(sourceControl, isControl));

            // return the lot...
            return list;
        }

        /// <summary>
        /// Finds the control recursive reverse.
        /// </summary>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The find control recursive reverse.
        /// </returns>
        public static Control FindControlRecursiveReverse([NotNull] this Control sourceControl, [NotNull] string id)
        {
            CodeContracts.VerifyNotNull(sourceControl, "sourceControl");
            CodeContracts.VerifyNotNull(id, "id");

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
        public static Control FindControlRecursiveBoth([NotNull] this Control sourceControl, [NotNull] string id)
        {
            CodeContracts.VerifyNotNull(sourceControl, "sourceControl");
            CodeContracts.VerifyNotNull(id, "id");

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
        /// the typeparameter
        /// </typeparam>
        /// <returns>
        /// Returns the Control as strongly typed
        /// </returns>
        public static T FindControlAs<T>([NotNull] this Control sourceControl, [NotNull] string id) where T : class
        {
            CodeContracts.VerifyNotNull(sourceControl, "sourceControl");
            CodeContracts.VerifyNotNull(id, "id");

            var foundControl = sourceControl.FindControl(id);

            if (foundControl is T)
            {
                return foundControl.ToClass<T>();
            }

            return null;
        }

        /// <summary>
        /// Finds the control recursive as.
        /// </summary>
        /// <typeparam name="T">the typeparameter</typeparam>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The find control recursive as.
        /// </returns>
        public static T FindControlRecursiveAs<T>([NotNull] this Control sourceControl, [NotNull] string id)
            where T : class
        {
            CodeContracts.VerifyNotNull(sourceControl, "sourceControl");
            CodeContracts.VerifyNotNull(id, "id");

            var foundControl = FindControlRecursive(sourceControl, id);
            if (foundControl is T)
            {
                return foundControl.ToClass<T>();
            }

            return null;
        }

        /// <summary>
        /// Finds the control recursive reverse as.
        /// </summary>
        /// <typeparam name="T">the typeparameter</typeparam>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The find control recursive reverse as.
        /// </returns>
        public static T FindControlRecursiveReverseAs<T>([NotNull] this Control sourceControl, [NotNull] string id)
            where T : class
        {
            CodeContracts.VerifyNotNull(sourceControl, "sourceControl");
            CodeContracts.VerifyNotNull(id, "id");

            var foundControl = FindControlRecursiveReverse(sourceControl, id);
            if (foundControl is T)
            {
                return foundControl.ToClass<T>();
            }

            return null;
        }

        /// <summary>
        /// Finds the control recursive both as.
        /// </summary>
        /// <typeparam name="T">the typeparameter</typeparam>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The find control recursive both as.
        /// </returns>
        public static T FindControlRecursiveBothAs<T>([NotNull] this Control sourceControl, [NotNull] string id)
            where T : class
        {
            CodeContracts.VerifyNotNull(sourceControl, "sourceControl");
            CodeContracts.VerifyNotNull(id, "id");

            var foundControl = FindControlRecursiveBoth(sourceControl, id);

            if (foundControl is T)
            {
                return foundControl.ToClass<T>();
            }

            return null;
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
        [CanBeNull]
        public static Control FindWizardControlRecursive([NotNull] this Wizard wizardControl, [NotNull] string id)
        {
            CodeContracts.VerifyNotNull(wizardControl, "wizardControl");
            CodeContracts.VerifyNotNull(id, "id");

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
        [CanBeNull]
        public static Control FindControlRecursive([NotNull] this Control sourceControl, [NotNull] string id)
        {
            CodeContracts.VerifyNotNull(sourceControl, "sourceControl");
            CodeContracts.VerifyNotNull(id, "id");

            var foundControl = sourceControl.FindControl(id);

            if (foundControl == null)
            {
                if (sourceControl.HasControls())
                {
                    foreach (Control tmpCtr in sourceControl.Controls)
                    {
                        // Check all child controls of sourceControl
                        foundControl = FindControlRecursive(tmpCtr, id);
                        if (foundControl != null)
                        {
                            break;
                        }
                    }
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
        [NotNull]
        public static IEnumerable<T> FindControlType<T>([NotNull] this Control sourceControl)
        {
            CodeContracts.VerifyNotNull(sourceControl, "sourceControl");

            return sourceControl.Controls.OfType<T>();
        }

        /// <summary>
        /// Makes the CSS include control.
        /// </summary>
        /// <param name="href">The href.</param>
        /// <returns>
        /// The make css include control.
        /// </returns>
        [NotNull]
        public static HtmlLink MakeCssIncludeControl([NotNull] string href)
        {
            CodeContracts.VerifyNotNull(href, "href");

            var stylesheet = new HtmlLink { Href = href };
            stylesheet.Attributes.Add("rel", "stylesheet");
            stylesheet.Attributes.Add("type", "text/css");

            return stylesheet;
        }

        /// <summary>
        /// The make css control.
        /// </summary>
        /// <param name="css">
        /// The style information to add to the control.
        /// </param>
        /// <returns>
        /// Returns the css control
        /// </returns>
        [NotNull]
        public static HtmlGenericControl MakeCssControl([NotNull] string css)
        {
            CodeContracts.VerifyNotNull(css, "css");

            var style = new HtmlGenericControl { TagName = "style" };
            style.Attributes.Add("type", "text/css");
            style.InnerText = css;

            return style;
        }

        /// <summary>
        /// The make a javascript include control.
        /// </summary>
        /// <param name="href">
        /// The href to the javascript script file.
        /// </param>s
        /// <returns>
        /// Returns the JS Include Control
        /// </returns>
        [NotNull]
        public static HtmlGenericControl MakeJsIncludeControl([NotNull] string href)
        {
            CodeContracts.VerifyNotNull(href, "href");

            var js = new HtmlGenericControl { TagName = "script" };
            js.Attributes.Add("type", "text/javascript");
            js.Attributes.Add("src", href);

            return js;
        }

        /// <summary>
        /// Creates a <see cref="HtmlMeta"/> control for keywords.
        /// </summary>
        /// <param name="keywords">keywords that go inside the meta</param>
        /// <returns><see cref="HtmlMeta"/> control</returns>
        public static HtmlMeta MakeMetaKeywordsControl(string keywords)
        {
            var meta = new HtmlMeta { Name = "keywords", Content = keywords };

            return meta;
        }

        /// <summary>
        /// Creates a <see cref="HtmlMeta"/> control for description.
        /// </summary>
        /// <param name="description">description that go inside the meta</param>
        /// <returns><see cref="HtmlMeta"/> control</returns>
        public static HtmlMeta MakeMetaDiscriptionControl(string description)
        {
            var meta = new HtmlMeta { Name = "description", Content = description };

            return meta;
        }

        /* Ederon - 7/1/2007 start */

        /// <summary>
        /// The add style attribute size.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public static void AddStyleAttributeSize(
            [NotNull] this WebControl control,
            [NotNull] string width,
            [NotNull] string height)
        {
            CodeContracts.VerifyNotNull(control, "control");
            CodeContracts.VerifyNotNull(width, "width");
            CodeContracts.VerifyNotNull(height, "height");

            control.Attributes.Add("style", "width: {0}; height: {1};".FormatWith(width, height));
        }

        /// <summary>
        /// The add style attribute width.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        public static void AddStyleAttributeWidth([NotNull] this WebControl control, [NotNull] string width)
        {
            CodeContracts.VerifyNotNull(control, "control");
            CodeContracts.VerifyNotNull(width, "width");

            control.Attributes.Add("style", "width: {0};".FormatWith(width));
        }

        /// <summary>
        /// The add style attribute height.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public static void AddStyleAttributeHeight([NotNull] this WebControl control, [NotNull] string height)
        {
            CodeContracts.VerifyNotNull(control, "control");
            CodeContracts.VerifyNotNull(height, "height");

            control.Attributes.Add("style", "height: {0};".FormatWith(height));
        }

        /// <summary>
        /// The add MaxLength attribute to TextBox.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="maxLength">
        /// The MaxLength.
        /// </param>
        public static void AddAttributeMaxWidth([NotNull] this WebControl control, [NotNull] string maxLength)
        {
            CodeContracts.VerifyNotNull(control, "control");
            CodeContracts.VerifyNotNull(maxLength, "maxLength");

            if (control is TextBox)
            {
                control.Attributes.Add("MaxLength", maxLength);
            }
        }

        /// <summary>
        /// Adds a class to the attribute "class". If one exists, it appends the class.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="cssClass">The CSS class.</param>
        public static void AddClass([NotNull] this WebControl control, [NotNull] string cssClass)
        {
            CodeContracts.VerifyNotNull(control, "control");
            CodeContracts.VerifyNotNull(cssClass, "cssClass");

            var currentClass = control.Attributes["class"];

            if (currentClass.IsSet())
            {
                control.Attributes["class"] = "{0} {1}".FormatWith(currentClass, cssClass);
            }
            else
            {
                control.Attributes.Add("class", cssClass);
            }
        }

        /* Ederon - 7/1/2007 end */

        /// <summary>
        /// The add on click confirm dialog.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void AddOnClickConfirmDialog([NotNull] object control, [NotNull] string message)
        {
            CodeContracts.VerifyNotNull(control, "control");
            CodeContracts.VerifyNotNull(message, "message");

            AddOnClickConfirmDialog((WebControl)control, message);
        }

        /// <summary>
        /// The add on click confirm dialog.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void AddOnClickConfirmDialog([NotNull] WebControl control, [NotNull] string message)
        {
            CodeContracts.VerifyNotNull(control, "control");
            CodeContracts.VerifyNotNull(message, "message");

            control.Attributes["onclick"] = "return confirm('{0}');".FormatWith(message);
        }

        /// <summary>
        /// Finds a control recursively (forward only) using <paramref name="isControl"/> function.
        /// </summary>
        /// <param name="sourceControl">
        /// Control to start search from.
        /// </param>
        /// <param name="isControl">
        /// Function to test if we found the control.
        /// </param>
        /// <returns>
        /// List of controls found
        /// </returns>
        [NotNull]
        private static IEnumerable<Control> ControlListNoParents(
            [NotNull] this Control sourceControl,
            [NotNull] Func<Control, bool> isControl)
        {
            CodeContracts.VerifyNotNull(sourceControl, "sourceControl");
            CodeContracts.VerifyNotNull(isControl, "isControl");

            return
                (from c in sourceControl.Controls.Cast<Control>().AsQueryable() where !c.HasControls() select c).Where(
                    isControl).ToList();
        }
    }
}