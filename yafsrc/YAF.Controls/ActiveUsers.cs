/* Yet Another Forum.NET
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
using System.Data;
using YAF.Classes.UI;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class ActiveUsers : BaseControl
	{
		private DataTable _activeUserTable = null;
		public DataTable ActiveUserTable
		{
			get
			{
				if ( _activeUserTable == null )
				{
					if ( ViewState ["ActiveUserTable"] != null )
					{
						_activeUserTable = ViewState ["ActiveUserTable"] as DataTable;
					}
				}

				return _activeUserTable;
			}
			set
			{
				ViewState ["ActiveUserTable"] = value;
				_activeUserTable = value;
			}
		}

		public bool TreatGuestAsHidden
		{
			get
			{
				if ( ViewState ["TreatGuestAsHidden"] != null )
				{
					return Convert.ToBoolean( ViewState ["TreatGuestAsHidden"] );
				}

				return false;
			}
			set
			{
				ViewState ["TreatGuestAsHidden"] = value;
			}
		}

		public ActiveUsers()
		{
			this.PreRender += new EventHandler( ActiveUsers_PreRender );
		}

		void ActiveUsers_PreRender( object sender, EventArgs e )
		{
			if ( ActiveUserTable != null )
			{
				foreach ( DataRow row in ActiveUserTable.Rows )
				{
					bool addControl = true;

					UserLink userLink = new UserLink();
					userLink.UserID = Convert.ToInt32( row ["UserID"] );
					userLink.UserName = row ["UserName"].ToString();
					userLink.ID = "UserLink" + userLink.UserID.ToString();

					int userCount = Convert.ToInt32( row ["UserCount"] );

					if ( userCount > 1 )
					{
						userLink.PostfixText = String.Format( " ({0})", userCount );
					}

					if ( Convert.ToBoolean( row ["IsHidden"] ) == true || ( TreatGuestAsHidden == true && YAF.Classes.Utils.UserMembershipHelper.IsGuestUser( row ["UserID"] ) ) )
					{
						if ( PageContext.IsAdmin || userLink.UserID == PageContext.PageUserID )
						{
							// show regardless...
							addControl = true;
							userLink.CssClass = "active_hidden";
							userLink.PostfixText = String.Format( " ({0})", PageContext.Localization.GetText( "HIDDEN" ) );
						}
						else
						{
							// user is hidden from this user...
							addControl = false;
						}
					}

					if ( addControl )
					{
						this.Controls.Add( userLink );
					}
				}
			}
		}

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			writer.WriteLine( String.Format( @"<div class=""yafactiveusers"" id=""{0}"">", this.ClientID ) );

			bool bFirst = true;
			foreach ( System.Web.UI.Control control in this.Controls )
			{
				if ( control is UserLink && control.Visible )
				{
					if ( !bFirst ) writer.WriteLine( ", " );
					else bFirst = false;

					control.RenderControl( writer );
				}
			}

			writer.WriteLine( "</div>" );
		}
	}
}
