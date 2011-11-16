/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2011 Jaben Cargman
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Types;
  using YAF.Utils;

    #endregion

  /// <summary>
  /// Summary description for SmartScroller.
  /// </summary>
  public class SmartScroller : BaseControl
  {
    /* Ederon : 6/16/2007 - conventions */
    #region Constants and Fields

    /// <summary>
    ///   The _hid scroll left.
    /// </summary>
    private readonly HtmlInputHidden _hidScrollLeft = new HtmlInputHidden();

    /// <summary>
    ///   The _hid scroll top.
    /// </summary>
    private readonly HtmlInputHidden _hidScrollTop = new HtmlInputHidden();

    #endregion

    #region Public Methods

    /// <summary>
    /// The register startup reset.
    /// </summary>
    public void RegisterStartupReset()
    {
      this.Reset();
      const string script = @"Sys.WebForms.PageRequestManager.getInstance().add_endRequest(yaf_SmartScroller_Reset);";
      YafContext.Current.PageElements.RegisterJsBlockStartup("SmartScrollerResetJs", script);
    }

    /// <summary>
    /// The reset.
    /// </summary>
    public void Reset()
    {
      this._hidScrollLeft.Value = "0";
      this._hidScrollTop.Value = "0";
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      this._hidScrollLeft.ID = "scrollLeft";
      this._hidScrollTop.ID = "scrollTop";

      this.Controls.Add(this._hidScrollLeft);
      this.Controls.Add(this._hidScrollTop);
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        string scriptString =
            @"
  function yaf_SmartScroller_GetCoords()
  {{
    var scrollX, scrollY;
    if (document.all)
    {{
      if (!document.documentElement.scrollLeft)
        scrollX = document.body.scrollLeft;
      else
        scrollX = document.documentElement.scrollLeft;

      if (!document.documentElement.scrollTop)
        scrollY = document.body.scrollTop;
      else
        scrollY = document.documentElement.scrollTop;
    }}
    else
    {{
      scrollX = window.pageXOffset;
      scrollY = window.pageYOffset;
    }}
      jQuery('#{0}').val( scrollX );
      jQuery('#{1}').val( scrollY );
    
  }}

  function yaf_SmartScroller_Scroll()
  {{
        var x = jQuery('#{0}').val();
		var y = jQuery('#{1}').val();
		if (x ||y) window.scrollTo(x,y);
  }}

	function yaf_SmartScroller_Reset()
	{{
	    jQuery('#{0}').val( 0 );
		jQuery('#{1}').val( 0 );	
		// force change...
		window.scrollTo(0,0);
	}}

	jQuery(window).bind('scroll', yaf_SmartScroller_GetCoords);
	jQuery(window).bind('click', yaf_SmartScroller_GetCoords);
	jQuery(window).bind('keypress', yaf_SmartScroller_GetCoords);
	jQuery(document).ready(yaf_SmartScroller_Scroll);
"
                .FormatWith(this._hidScrollLeft.ClientID, this._hidScrollTop.ClientID);

      YafContext.Current.PageElements.RegisterJsBlock("SmartScrollerJs", scriptString);

      base.OnPreRender(e);
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
      this.Page.VerifyRenderingInServerForm(this);
      base.Render(writer);
    }

    #endregion
  }
}