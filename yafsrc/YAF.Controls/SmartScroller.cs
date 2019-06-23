/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
namespace YAF.Controls
{
  #region Using

    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

  /// <summary>
  /// Smart Scroller Control.
  /// </summary>
  public class SmartScroller : BaseControl
  {
    /* Ederon : 6/16/2007 - conventions */
    #region Constants and Fields

    /// <summary>
    ///   The _hid scroll left.
    /// </summary>
    private readonly HtmlInputHidden hidScrollLeft = new HtmlInputHidden();

    /// <summary>
    ///   The _hid scroll top.
    /// </summary>
    private readonly HtmlInputHidden hidScrollTop = new HtmlInputHidden();

    #endregion

    #region Public Methods

    /// <summary>
    /// The register startup reset.
    /// </summary>
    public void RegisterStartupReset()
    {
      this.Reset();
      const string Script = @"Sys.WebForms.PageRequestManager.getInstance().add_endRequest(yaf_SmartScroller_Reset);";
      YafContext.Current.PageElements.RegisterJsBlockStartup("SmartScrollerResetJs", Script);
    }

    /// <summary>
    /// The reset.
    /// </summary>
    public void Reset()
    {
      this.hidScrollLeft.Value = "0";
      this.hidScrollTop.Value = "0";
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
      this.hidScrollLeft.ID = "scrollLeft";
      this.hidScrollTop.ID = "scrollTop";

      this.Controls.Add(this.hidScrollLeft);
      this.Controls.Add(this.hidScrollTop);
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        var scriptString =
            string
                .Format(@"
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
", this.hidScrollLeft.ClientID, this.hidScrollTop.ClientID);

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