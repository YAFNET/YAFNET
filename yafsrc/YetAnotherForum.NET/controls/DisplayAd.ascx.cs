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
using System.Data;
using System.Drawing;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using YAF.Classes.UI;

namespace YAF.Controls
{
	/// <summary>
	///		Summary description for DisplayAd.
	/// </summary>
	public partial class DisplayAd : YAF.Classes.Base.BaseUserControl
	{
		private bool _isAlt = false;


		protected void Page_Load(object sender, System.EventArgs e)
		{
			AdMessage.Message = PageContext.BoardSettings.AdPost;
			AdMessage.Signature = PageContext.Localization.GetText( "AD_SIGNATURE" );

			AdMessage.MessageFlags.IsLocked = true;
			AdMessage.MessageFlags.NotFormatted = true;
		}

		protected string GetPostClass()
		{
			if ( this.IsAlt )
				return "post_alt";
			else
				return "post";
		}

		public bool IsAlt
		{
			get { return this._isAlt; }
			set { this._isAlt = value; }
		}
	}
}
