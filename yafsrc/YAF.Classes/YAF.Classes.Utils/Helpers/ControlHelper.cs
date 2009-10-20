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
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace YAF.Classes.Utils
{
	static public class ControlHelper
	{
		static public Control FindControlRecursiveReverse(Control sourceControl, string id)
		{
			Control foundControl = sourceControl.FindControl(id);

			if (foundControl != null)
			{
				return foundControl;
			}
			else if (sourceControl.Parent != null)
			{
				return FindControlRecursiveReverse( sourceControl.Parent, id );
			}
			return null;
		}

		static public Control FindControlRecursiveBoth( Control sourceControl, string id)
		{
			Control found = FindControlRecursiveReverse( sourceControl, id );
			if ( found != null ) return found;
			found = FindControlRecursive( sourceControl, id );
			return found;
		}

		static public T FindControlAs<T>(Control sourceControl, string id) where T : class
		{
			Control foundControl = sourceControl.FindControl( id );
			if ( foundControl != null && foundControl is T )
			{
				return foundControl.ToClass<T>();
			}

			return null;
		}

		static public T FindControlRecursiveAs<T>(Control sourceControl, string id) where T : class
		{
			Control foundControl = FindControlRecursive( sourceControl, id );
			if ( foundControl != null && foundControl is T )
			{
				return foundControl.ToClass<T>();
			}

			return null;
		}

		static public T FindControlRecursiveReverseAs<T>(Control sourceControl, string id) where T : class
		{
			Control foundControl = FindControlRecursiveReverse(sourceControl, id);
			if ( foundControl != null && foundControl is T )
			{
				return foundControl.ToClass<T>();
			}

			return null;
		}

		static public T FindControlRecursiveBothAs<T>(Control sourceControl, string id) where T : class
		{
			Control foundControl = FindControlRecursiveBoth(sourceControl, id);

			if ( foundControl != null && foundControl is T )
			{
				return foundControl.ToClass<T>();
			}

			return null;
		}

		/// <summary>
		/// Find Wizard Control - Find a control in a wizard
		/// </summary>
		/// <param name="wizardControl">Wizard control</param>
		/// <param name="id">ID of target control</param>
		/// <returns>A control reference, if found, null, if not</returns>
		static public Control FindWizardControlRecursive(Wizard wizardControl, string id)
		{
			Control foundControl = null;

			for (int i = 0; i < wizardControl.WizardSteps.Count; i++)
			{
				for (int j = 0; j < wizardControl.WizardSteps[i].Controls.Count; j++)
				{
					foundControl = FindControlRecursive( (Control)wizardControl.WizardSteps[i].Controls[j], id );
					if (foundControl != null) break;
				}
				if (foundControl != null) break;
			}

			return foundControl;

		}

		/// <summary>
		/// Find Wizard Control - Find a control in a wizard, is recursive
		/// </summary>
		/// <param name="sourceControl">Source/Root Control</param>
		/// <param name="id">ID of target control</param>
		/// <returns>A Control, if found; null, if not</returns>
		static public Control FindControlRecursive(Control sourceControl, string id)
		{
			Control foundControl = sourceControl.FindControl( id );

			if (foundControl == null)
			{
				if (sourceControl.HasControls())
				{
					foreach ( Control tmpCtr in sourceControl.Controls )
					{
						// Check all child controls of sourceControl
						foundControl = FindControlRecursive( tmpCtr, id );
						if (foundControl != null) break;
					} 
				} 
			} 
			return foundControl;
		} 

		static public HtmlLink MakeCssIncludeControl(string href)
		{
			HtmlLink stylesheet = new HtmlLink();
			stylesheet.Href = href;
			stylesheet.Attributes.Add("rel", "stylesheet");
			stylesheet.Attributes.Add("type", "text/css");

			return stylesheet;
		}

		static public HtmlGenericControl MakeCssControl(string css)
		{
			HtmlGenericControl style = new HtmlGenericControl();
			style.TagName = "style";
			style.Attributes.Add("type", "text/css");
			style.InnerText = css;

			return style;
		}

		static public HtmlGenericControl MakeJsIncludeControl(string href)
		{
			HtmlGenericControl js = new HtmlGenericControl();
			js.TagName = "script";
			js.Attributes.Add( "type", "text/javascript" );
			js.Attributes.Add( "src", href );

			return js;
		}

		/* Ederon - 7/1/2007 start */
		static public void AddStyleAttributeSize(System.Web.UI.WebControls.WebControl control, string width, string height)
		{
			control.Attributes.Add("style", String.Format("width: {0}; height: {1};", width, height));
		}

		static public void AddStyleAttributeWidth(System.Web.UI.WebControls.WebControl control, string width)
		{
			control.Attributes.Add("style", String.Format("width: {0};", width));
		}

		static public void AddStyleAttributeHeight(System.Web.UI.WebControls.WebControl control, string height)
		{
			control.Attributes.Add("style", String.Format("height: {0};", height));
		}

		/* Ederon - 7/1/2007 end */
		static public void AddOnClickConfirmDialog(object control, string message)
		{
			AddOnClickConfirmDialog((System.Web.UI.WebControls.WebControl)control, message);
		}
		static public void AddOnClickConfirmDialog(System.Web.UI.WebControls.WebControl control, string message)
		{
			control.Attributes["onclick"] = String.Format("return confirm('{0}');", message);
		}

	}
}
