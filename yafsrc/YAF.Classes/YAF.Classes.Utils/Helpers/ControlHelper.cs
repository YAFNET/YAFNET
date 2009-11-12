/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace YAF.Classes.Utils
{
  /// <summary>
  /// Provides helper functions for using and accessing controls.
  /// </summary>
  public static class ControlHelper
  {
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
    public static List<Control> ControlListRecursive(Control sourceControl, Func<Control, bool> isControl)
    {
      var list = new List<Control>();

      var withParents = (from c in sourceControl.Controls.Cast<Control>().AsQueryable()
                         where c.HasControls()
                         select c).ToList();

      // recusively call this function looking for controls...
      withParents.ForEach(x => list.AddRange(ControlListRecursive(x, isControl)));

      // add controls from this level...
      list.AddRange(
        (from c in sourceControl.Controls.Cast<Control>().AsQueryable()
         where !c.HasControls()
         select c).ToList().Where(isControl));


      // return the lot...
      return list;
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
    public static Control FindControlRecursiveReverse(Control sourceControl, string id)
    {
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
    public static Control FindControlRecursiveBoth(Control sourceControl, string id)
    {
      Control found = FindControlRecursiveReverse(sourceControl, id);
      if (found != null)
      {
        return found;
      }

      found = FindControlRecursive(sourceControl, id);
      return found;
    }

    /// <summary>
    /// The find control as.
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
    public static T FindControlAs<T>(Control sourceControl, string id) where T : class
    {
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
    public static T FindControlRecursiveAs<T>(Control sourceControl, string id) where T : class
    {
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
    public static T FindControlRecursiveReverseAs<T>(Control sourceControl, string id) where T : class
    {
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
    public static T FindControlRecursiveBothAs<T>(Control sourceControl, string id) where T : class
    {
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
    public static Control FindWizardControlRecursive(Wizard wizardControl, string id)
    {
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
    public static Control FindControlRecursive(Control sourceControl, string id)
    {
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
    /// The css.
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
    /// The make js include control.
    /// </summary>
    /// <param name="href">
    /// The href.
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
    public static void AddStyleAttributeSize(WebControl control, string width, string height)
    {
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
    public static void AddStyleAttributeWidth(WebControl control, string width)
    {
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
    public static void AddStyleAttributeHeight(WebControl control, string height)
    {
      control.Attributes.Add("style", String.Format("height: {0};", height));
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
      control.Attributes["onclick"] = String.Format("return confirm('{0}');", message);
    }
  }
}