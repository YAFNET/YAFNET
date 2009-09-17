/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;
using YAF.Classes.Core;

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

		protected override void OnInit(EventArgs e)
		{				
			_hidScrollLeft.ID = "scrollLeft";
			_hidScrollTop.ID = "scrollTop";
	
			this.Controls.Add(_hidScrollLeft);
			this.Controls.Add(_hidScrollTop);						
	
			string scriptString = @"
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
	  jQuery('#" + _hidScrollLeft.ClientID + @"').val( scrollX );
		jQuery('#" + _hidScrollTop.ClientID + @"').val( scrollY );
  }

  function yaf_SmartScroller_Scroll()
  {
		var x = jQuery('#" + _hidScrollLeft.ClientID + @"').val();
		var y = jQuery('#" + _hidScrollTop.ClientID + @"').val();
		if (x || y) window.scrollTo(x,y);
  }

	function yaf_SmartScroller_Reset()
	{
	  jQuery('#" + _hidScrollLeft.ClientID + @"').val( 0 );
		jQuery('#" + _hidScrollTop.ClientID + @"').val( 0 );	
		// force change...
		window.scrollTo(0,0);
	}

	jQuery(window).bind('scroll', yaf_SmartScroller_GetCoords);
	jQuery(window).bind('click', yaf_SmartScroller_GetCoords);
	jQuery(window).bind('keypress', yaf_SmartScroller_GetCoords);
	jQuery(document).ready(yaf_SmartScroller_Scroll);
";

			YafContext.Current.PageElements.RegisterJQuery();
			YafContext.Current.PageElements.RegisterJsBlock( "SmartScrollerJs", scriptString );
		}

		protected override void Render(HtmlTextWriter writer)
		{
			Page.VerifyRenderingInServerForm(this);
			base.Render(writer);
		}

		public void RegisterStartupReset()
		{
			Reset();
			const string script = @"Sys.WebForms.PageRequestManager.getInstance().add_endRequest(yaf_SmartScroller_Reset);";
			YafContext.Current.PageElements.RegisterJsBlockStartup( "SmartScrollerResetJs", script );
		}

		public void Reset()
		{
			_hidScrollLeft.Value = "0";
			_hidScrollTop.Value = "0";
		}
	}
}