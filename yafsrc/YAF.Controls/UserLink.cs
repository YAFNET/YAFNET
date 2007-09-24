/* Yet Another Forum.NET
 * Copyright (C) 2006-2007 Jaben Cargman
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
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	/// <summary>
	/// Provides a basic "profile link" for a YAF User
	/// </summary>
	public class UserLink : BaseControl
	{
		public UserLink()
		{
			this.Load += new EventHandler( UserLink_Load );
		}

		private void UserLink_Load( object sender, EventArgs e )
		{
		}

		protected override void Render( HtmlTextWriter output )
		{
			if ( _userID != -1 && !String.IsNullOrEmpty( _userName ))
			{
				// is this the guest user? If so, guest's don't have a profile.
				bool isGuest = YAF.Classes.Utils.UserMembershipHelper.IsGuestUser( _userID );

				output.BeginRender();

				if (!isGuest)
				{					
					output.WriteBeginTag( "a" );
					output.WriteAttribute( "href", YafBuildLink.GetLink(ForumPages.profile,"u={0}",_userID));
					output.WriteAttribute( "title", HtmlEncode( _userName ));
					if ( !String.IsNullOrEmpty( _onclick ) ) output.WriteAttribute( "OnClick", _onclick );
					if ( !String.IsNullOrEmpty( _onmouseover ) ) output.WriteAttribute( "OnMouseOver", _onmouseover );
					output.Write(HtmlTextWriter.TagRightChar);
				}

				output.WriteEncodedText( _userName );

				if (!isGuest)
				{
					output.WriteEndTag( "a" );
				}

				output.EndRender();
			}
		}

		/// <summary>
		/// The onclick value for the profile link
		/// </summary>
		private string _onclick = string.Empty;
		public string OnClick
		{
			get
			{
				return _onclick;
			}
			set
			{
				_onclick = value;
			}
		}
		
		/// <summary>
		/// The onmouseover value for the profile link
		/// </summary>
		private string _onmouseover = string.Empty;
		public string OnMouseOver
		{
			get
			{
				return _onmouseover;
			}
			set
			{
				_onmouseover = value;
			}
		}

		/// <summary>
		/// The name of the user for this profile link
		/// </summary>
		private string _userName = string.Empty;
		public string UserName
		{
			get
			{
				return _userName;
			}
			set
			{
				_userName = value;
			}
		}

		/// <summary>
		/// The userid of this user for the link
		/// </summary>
		private int _userID = -1;
		public int UserID
		{
			get
			{
				return _userID;
			}
			set
			{
				_userID = value;
			}
		}
	}
}
