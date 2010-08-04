/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Classes.Utils
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
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
    /// <param name="control"></param>
    /// <returns></returns>
    public static string RenderToString(this Control control)
    {
      if (control.Visible)
      {
        using (var stringWriter = new StringWriter())
        {
          using (var writer = new HtmlTextWriter(stringWriter))
          {
            control.RenderControl(writer);
            return stringWriter.ToString();
          }
        }
      }

      return String.Empty;
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
    public static List<Control> ControlListRecursive(this Control sourceControl, Func<Control, bool> isControl)
    {
      if (sourceControl == null)
      {
        throw new ArgumentNullException("sourceControl", "sourceControl is null.");
      }

      if (isControl == null)
      {
        throw new ArgumentNullException("isControl", "isControl is null.");
      }

      var list = new List<Control>();

      var withParents = (from c in sourceControl.Controls.Cast<Control>().AsQueryable()
                         where c.HasControls()
                         select c).ToList();

      // recusively call this function looking for controls...
      withParents.ForEach(x => list.AddRange(ControlListRecursive(x, isControl)));

      // add controls from this level...
      list.AddRange(ControlListNoParents(sourceControl, isControl));

      // return the lot...
      return list;
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
    private static List<Control> ControlListNoParents(this Control sourceControl, Func<Control, bool> isControl)
    {
      if (sourceControl == null)
      {
        throw new ArgumentNullException("sourceControl", "sourceControl is null.");
      }

      if (isControl == null)
      {
        throw new ArgumentNullException("isControl", "isControl is null.");
      }

      return (from c in sourceControl.Controls.Cast<Control>().AsQueryable()
              where !c.HasControls()
              select c).Where(isControl).ToList();
    }

    /// <summary>
    /// The find control recursive reverse.
    /// </summary>
    /// <param name="sourceControl">
    /// The source control.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// </returns>
    public static Control FindControlRecursiveReverse(this Control sourceControl, string id)
    {
      if (sourceControl == null)
      {
        throw new ArgumentNullException("sourceControl", "sourceControl is null.");
      }

      if (String.IsNullOrEmpty(id))
      {
        throw new ArgumentException("id is null or empty.", "id");
      }

      Control foundControl = sourceControl.FindControl(id);

      if (foundControl != null)
      {
        return foundControl;
      }
      else if (sourceControl.Parent != null)
      {
        return FindControlRecursiveReverse(sourceControl.Parent, id);
      }

      return null;
    }

    /// <summary>
    /// The find control recursive both.
    /// </summary>
    /// <param name="sourceControl">
    /// The source control.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// </returns>
    public static Control FindControlRecursiveBoth(this Control sourceControl, string id)
    {
      if (sourceControl == null)
      {
        throw new ArgumentNullException("sourceControl", "sourceControl is null.");
      }

      if (String.IsNullOrEmpty(id))
      {
        throw new ArgumentException("id is null or empty.", "id");
      }

      Control found = FindControlRecursiveReverse(sourceControl, id);
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
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T FindControlAs<T>(this Control sourceControl, string id) where T : class
    {
      if (sourceControl == null)
      {
        throw new ArgumentNullException("sourceControl", "sourceControl is null.");
      }

      if (String.IsNullOrEmpty(id))
      {
        throw new ArgumentException("id is null or empty.", "id");
      }

      Control foundControl = sourceControl.FindControl(id);
      if (foundControl != null && foundControl is T)
      {
        return foundControl.ToClass<T>();
      }

      return null;
    }

    /// <summary>
    /// The find control recursive as.
    /// </summary>
    /// <param name="sourceControl">
    /// The source control.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T FindControlRecursiveAs<T>(this Control sourceControl, string id) where T : class
    {
      if (sourceControl == null)
      {
        throw new ArgumentNullException("sourceControl", "sourceControl is null.");
      }

      if (String.IsNullOrEmpty(id))
      {
        throw new ArgumentException("id is null or empty.", "id");
      }

      Control foundControl = FindControlRecursive(sourceControl, id);
      if (foundControl != null && foundControl is T)
      {
        return foundControl.ToClass<T>();
      }

      return null;
    }

    /// <summary>
    /// The find control recursive reverse as.
    /// </summary>
    /// <param name="sourceControl">
    /// The source control.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T FindControlRecursiveReverseAs<T>(this Control sourceControl, string id) where T : class
    {
      if (sourceControl == null)
      {
        throw new ArgumentNullException("sourceControl", "sourceControl is null.");
      }

      if (String.IsNullOrEmpty(id))
      {
        throw new ArgumentException("id is null or empty.", "id");
      }

      Control foundControl = FindControlRecursiveReverse(sourceControl, id);
      if (foundControl != null && foundControl is T)
      {
        return foundControl.ToClass<T>();
      }

      return null;
    }

    /// <summary>
    /// The find control recursive both as.
    /// </summary>
    /// <param name="sourceControl">
    /// The source control.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T FindControlRecursiveBothAs<T>(this Control sourceControl, string id) where T : class
    {
      if (sourceControl == null)
      {
        throw new ArgumentNullException("sourceControl", "sourceControl is null.");
      }

      if (String.IsNullOrEmpty(id))
      {
        throw new ArgumentException("id is null or empty.", "id");
      }

      Control foundControl = FindControlRecursiveBoth(sourceControl, id);

      if (foundControl != null && foundControl is T)
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
    public static Control FindWizardControlRecursive(this Wizard wizardControl, string id)
    {
      if (wizardControl == null)
      {
        throw new ArgumentNullException("wizardControl", "wizardControl is null.");
      }

      if (String.IsNullOrEmpty(id))
      {
        throw new ArgumentException("id is null or empty.", "id");
      }

      Control foundControl = null;

      for (int i = 0; i < wizardControl.WizardSteps.Count; i++)
      {
        for (int j = 0; j < wizardControl.WizardSteps[i].Controls.Count; j++)
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
      if (sourceControl == null)
      {
        throw new ArgumentNullException("sourceControl", "sourceControl is null.");
      }

      if (String.IsNullOrEmpty(id))
      {
        throw new ArgumentException("id is null or empty.", "id");
      }

      Control foundControl = sourceControl.FindControl(id);

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
    /// <exception cref="ArgumentNullException">
    /// </exception>
    public static List<T> FindControlType<T>(this Control sourceControl)
    {
      if (sourceControl == null)
      {
        throw new ArgumentNullException("sourceControl", "sourceControl is null.");
      }

      if (sourceControl.HasControls())
      {
        // get all controls of type T as a list...
        return sourceControl.Controls.Cast<Control>().Where(x => x.GetType() == typeof(T)).Cast<T>().ToList();
      }

      // return nothing found...
      return new List<T>();
    }

    /// <summary>
    /// The make css include control.
    /// </summary>
    /// <param name="href">
    /// The href.
    /// </param>
    /// <returns>
    /// </returns>
    public static HtmlLink MakeCssIncludeControl(string href)
    {
      var stylesheet = new HtmlLink();
      stylesheet.Href = href;
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
    /// </returns>
    public static HtmlGenericControl MakeCssControl(string css)
    {
      var style = new HtmlGenericControl();
      style.TagName = "style";
      style.Attributes.Add("type", "text/css");
      style.InnerText = css;

      return style;
    }

    /// <summary>
    /// The make a javascript include control.
    /// </summary>
    /// <param name="href">
    /// The href to the javascript script file.
    /// </param>
    /// <returns>
    /// </returns>
    public static HtmlGenericControl MakeJsIncludeControl(string href)
    {
      var js = new HtmlGenericControl();
      js.TagName = "script";
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
      HtmlMeta meta = new HtmlMeta
        {
          Name = "keywords",
          Content = keywords
        };

      return meta;
    }

    /// <summary>
    /// Creates a <see cref="HtmlMeta"/> control for description.
    /// </summary>
    /// <param name="description">description that go inside the meta</param>
    /// <returns><see cref="HtmlMeta"/> control</returns>
    public static HtmlMeta MakeMetaDiscriptionControl(string description)
    {
      HtmlMeta meta = new HtmlMeta
      {
        Name = "description",
        Content = description
      };

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
    public static void AddStyleAttributeSize(this WebControl control, string width, string height)
    {
      if (control == null)
      {
        throw new ArgumentNullException("control", "control is null.");
      }

      if (String.IsNullOrEmpty(width))
      {
        throw new ArgumentException("width is null or empty.", "width");
      }

      if (String.IsNullOrEmpty(height))
      {
        throw new ArgumentException("height is null or empty.", "height");
      }

      control.Attributes.Add("style", String.Format("width: {0}; height: {1};", width, height));
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
    public static void AddStyleAttributeWidth(this WebControl control, string width)
    {
      if (control == null)
      {
        throw new ArgumentNullException("control", "control is null.");
      }

      if (String.IsNullOrEmpty(width))
      {
        throw new ArgumentException("width is null or empty.", "width");
      }

      control.Attributes.Add("style", String.Format("width: {0};", width));
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
    public static void AddStyleAttributeHeight(this WebControl control, string height)
    {
      if (control == null)
      {
        throw new ArgumentNullException("control", "control is null.");
      }

      if (String.IsNullOrEmpty(height))
      {
        throw new ArgumentException("height is null or empty.", "height");
      }

      control.Attributes.Add("style", String.Format("height: {0};", height));
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
    public static void AddAttributeMaxWidth(this WebControl control, string maxLength)
    {
        if (control == null)
        {
            throw new ArgumentNullException("control", "control is null.");
        }

        if (String.IsNullOrEmpty(maxLength))
        {
            throw new ArgumentException("MaxLength is null or empty.", "height");
        }

        if (control is TextBox)
        {
            control.Attributes.Add("MaxLength", maxLength);
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
    public static void AddOnClickConfirmDialog(object control, string message)
    {
      if (control == null)
      {
        throw new ArgumentNullException("control", "control is null.");
      }

      if (String.IsNullOrEmpty(message))
      {
        throw new ArgumentException("message is null or empty.", "message");
      }

      AddOnClickConfirmDialog((WebControl) control, message);
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
    public static void AddOnClickConfirmDialog(WebControl control, string message)
    {
      if (control == null)
      {
        throw new ArgumentNullException("control", "control is null.");
      }

      if (String.IsNullOrEmpty(message))
      {
        throw new ArgumentException("message is null or empty.", "message");
      }

      control.Attributes["onclick"] = String.Format("return confirm('{0}');", message);
    }
  }
}