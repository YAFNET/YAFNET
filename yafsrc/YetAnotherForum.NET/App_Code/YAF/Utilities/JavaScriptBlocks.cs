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
					YafContext.Current.Localization.GetText( "IMAGE_DOWNLOAD" ) + @"'
					}
				});
			});";				
			}
		}

		/// <summary>
		/// Requires {0} formatted elementId.
		/// </summary>
		public static string BlockUIExecuteJs(string elementId)
		{
			return
				string.Format(
					@"jQuery(document).ready(function() {{ 
            jQuery.blockUI({{ message: jQuery('#{0}') }}); 
        }});",
					elementId );
		}

        /// <summary>
        /// script for the addThanks button
        /// </summary>
        /// <param name="RemoveThankBoxHTML">HTML code for the "Remove Thank Note" button</param>
        /// <returns></returns>
        public static string addThanksJs(string RemoveThankBoxHTML)
        {
            return
                string.Format("function addThanks(messageID){{YAF.Controls.ThankYou.AddThanks(messageID, addThanksSuccess, CallFailed);}}" +
            "function addThanksSuccess(res){{if (res.value != null) {{" +
            "var dvThanks=document.getElementById('dvThanks' + res.value.messageID); dvThanks.innerHTML=res.value.Thanks;" +
            "dvThanksInfo=document.getElementById('dvThanksInfo' + res.value.messageID); dvThanksInfo.innerHTML=res.value.ThanksInfo;" +
            "dvThankbox=document.getElementById('dvThankBox' + res.value.messageID); dvThankbox.innerHTML={0};}}}}", RemoveThankBoxHTML);
        }

        /// <summary>
        /// script for the removeThanks button
        /// </summary>
        /// <param name="RemoveThankBoxHTML">HTML code for the "Thank" button</param>
        /// <returns></returns>
        public static string removeThanksJs(string AddThankBoxHTML)
        {
            return
                string.Format("function removeThanks(messageID){{YAF.Controls.ThankYou.RemoveThanks(messageID, removeThanksSuccess, CallFailed);}}" +
            "function removeThanksSuccess(res){{if (res.value != null) {{" +
            "var dvThanks=document.getElementById('dvThanks' + res.value.messageID); dvThanks.innerHTML=res.value.Thanks;" +
            "dvThanksInfo=document.getElementById('dvThanksInfo' + res.value.messageID); dvThanksInfo.innerHTML=res.value.ThanksInfo;" +
            "dvThankbox=document.getElementById('dvThankBox' + res.value.messageID); dvThankbox.innerHTML={0};}}}}", AddThankBoxHTML);
        }

        /// <summary>
        /// If asynchronous callback encounters any problem, this javascript function will be called.
        /// </summary>
        /// <returns></returns>
        public static string asynchCallFailedJs
        {
            get
            {
                return "function CallFailed(res){{alert('Error Occured');}}";
            }
        }

	}
}