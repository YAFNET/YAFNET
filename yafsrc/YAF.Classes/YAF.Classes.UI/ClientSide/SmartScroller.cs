/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace YAF.Classes.UI
{
	/// <summary>
	/// Summary description for SmartScroller.
	/// </summary>
	public class SmartScroller : System.Web.UI.Control
	{
		/* Ederon : 6/16/2007 - conventions */

		private HtmlForm _theForm = null;

		private HtmlInputHidden _hidScrollLeft = new HtmlInputHidden();
		private HtmlInputHidden _hidScrollTop = new HtmlInputHidden();
		
		public SmartScroller() {}
		
		private HtmlForm GetServerForm(ControlCollection parent)
		{
			HtmlForm tmpHtmlForm = null;
            
			foreach (Control child in parent)
			{                                
				Type t = child.GetType();
				if (t == typeof(System.Web.UI.HtmlControls.HtmlForm))
					return (HtmlForm)child;
                
				if (child.HasControls())    
				{
					tmpHtmlForm = GetServerForm(child.Controls);
					if (tmpHtmlForm != null && tmpHtmlForm.ClientID != null)
						return tmpHtmlForm;
				}
			}
        
			return null;
		}

		protected override void OnInit(EventArgs e)
		{			
			string formID = "Form";

			if (Page.Parent != null)
				_theForm = GetServerForm(Page.Parent.Controls);
			else
				_theForm = GetServerForm(Page.Controls);

			if (_theForm != null && _theForm.ClientID != null)
			{
				formID = _theForm.ClientID;
			}
										
			_hidScrollLeft.ID = "scrollLeft";
			_hidScrollTop.ID = "scrollTop";
	
			this.Controls.Add(_hidScrollLeft);
			this.Controls.Add(_hidScrollTop);						
	
			string scriptString = @"

  function yaf_GetForm()
  {
    var theform;
    if (window.navigator.appName.toLowerCase().indexOf(""microsoft"") > -1)
    {
	  theform = document." + formID + @";
	}
	else {
	  theform = document.forms[""" + formID + @"""];
    }
    return theform;
  }

  function yaf_SmartScroller_GetCoords()
  {
    var scrollX, scrollY;
    if (document.all)
    {
      if (!document.documentElement.scrollLeft)
        scrollX = document.body.scrollLeft;
      else
        scrollX = document.documentElement.scrollLeft;

      if (!document.documentElement.scrollTop)
        scrollY = document.body.scrollTop;
      else
        scrollY = document.documentElement.scrollTop;
    }
    else
    {
      scrollX = window.pageXOffset;
      scrollY = window.pageYOffset;
    }
	var cForm = yaf_GetForm();
    cForm." + _hidScrollLeft.ClientID + @".value = scrollX;
    cForm." + _hidScrollTop.ClientID + @".value = scrollY;
  }

  function yaf_SmartScroller_Scroll()
  {
    var cForm = yaf_GetForm();
    var x = cForm." + _hidScrollLeft.ClientID + @".value;
    var y = cForm." + _hidScrollTop.ClientID + @".value;
		if (x || y) window.scrollTo(x, y);
		if (oldOnLoad != null) oldOnLoad();
  }

	function yaf_SmartScroller_Reset()
	{
    var cForm = yaf_GetForm();
    var x = cForm." + _hidScrollLeft.ClientID + @".value;
    var y = cForm." + _hidScrollTop.ClientID + @".value;
		if ( x ) x = 0;
		if ( y ) y = 0;		
		// force change...
		window.scrollTo(0,0);		
	}
	
	var oldOnLoad = window.onload;
  
  window.onload = yaf_SmartScroller_Scroll;
  window.onscroll = yaf_SmartScroller_GetCoords;
  window.onclick = yaf_SmartScroller_GetCoords;
  window.onkeypress = yaf_SmartScroller_GetCoords;

";


			ScriptManager.RegisterStartupScript( Page, Page.GetType(), "SmartScroller", scriptString, true );
		}

		protected override void Render(HtmlTextWriter writer)
		{
			Page.VerifyRenderingInServerForm(this);
			base.Render(writer);
		}

		public void RegisterStartupReset()
		{
			Reset();
			string script = @"

				Sys.WebForms.PageRequestManager.getInstance().add_endRequest(yaf_SmartScroller_Reset);

			";
			ScriptManager.RegisterStartupScript( Page, Page.GetType(), "SmartScrollerReset", script, true );
		}

		public void Reset()
		{
			_hidScrollLeft.Value = "0";
			_hidScrollTop.Value = "0";
		}
	}
}