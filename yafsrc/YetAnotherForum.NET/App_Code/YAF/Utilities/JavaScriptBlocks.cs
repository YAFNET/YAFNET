/* Yet Another Forum.NET
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
using System.Web;
using YAF.Classes.Core;

namespace YAF.Utilities
{
	/// <summary>
	/// Summary description for JavaScriptBlocks
	/// </summary>
	public static class JavaScriptBlocks
	{
		public static string ToogleMessageJs
		{
			get
			{
				return
					@"
function toggleMessage(divId)
{
    if(divId != null)
    {
        var o = $get(divId);

        if(o != null)
        {
            o.style.display = (o.style.display == ""none"" ? ""block"" : ""none"");
        }
    }
}
";	
			}
		}

		public static string DisablePageManagerScrollJs
		{
			get
			{
				return
					@"
	var prm = Sys.WebForms.PageRequestManager.getInstance();

	prm.add_beginRequest(beginRequest);

	function beginRequest() {
		prm._scrollPosition = null;
	}
";
			}
		}

		public static string LightBoxLoadJs
		{
			get
			{
				return
					@"jQuery(document).ready(function() { 
					jQuery.Lightbox.construct({
						show_linkback:	false,
						show_helper_text: false,
				text: {
					image:		'" +
					YafContext.Current.Localization.GetText( "IMAGE_TEXT" ) + @"',
					close:    '" +
					YafContext.Current.Localization.GetText( "CLOSE_TEXT" ) + @"',
					download:    '" +
					YafContext.Current.Localization.GetText( "IMAGE_DOWNLOAD" ) + @"',
					}
				});
			});";				
			}
		}


	}
}