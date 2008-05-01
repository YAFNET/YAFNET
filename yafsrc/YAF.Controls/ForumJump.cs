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
using System.Web.UI;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for ForumJump.
	/// </summary>
	public class ForumJump : BaseControl, System.Web.UI.IPostBackDataHandler
	{
		private void Page_Load( object sender, System.EventArgs e )
		{
			if ( !Page.IsPostBack )
				ForumID = PageContext.PageForumID;
		}

		override protected void OnInit( EventArgs e )
		{
			this.Load += new System.EventHandler( this.Page_Load );
			base.OnInit( e );
		}

		private int ForumID
		{
			get
			{
				return ( int )ViewState ["ForumID"];
			}
			set
			{
				ViewState ["ForumID"] = value;
			}
		}

		#region IPostBackDataHandler
		public virtual bool LoadPostData( string postDataKey, System.Collections.Specialized.NameValueCollection postCollection )
		{
			int forumID;
			if (int.TryParse(postCollection[postDataKey], out forumID) &&
				forumID != ForumID)
			{
				this.ForumID = forumID;
				return true;
			}

			return false;
		}

		public virtual void RaisePostDataChangedEvent()
		{
			// Ederon : 9/4/2007
			if ( ForumID > 0 )
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.topics, "f={0}", ForumID );
			else
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.forum, "c={0}", -ForumID );
		}
		#endregion

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			DataTable dt;

			string cachename = String.Format( "forumjump_{0}_{1}", PageContext.User != null ? PageContext.User.UserName : "Guest", PageContext.User != null );

			if ( Page.Cache [cachename] != null )
			{
				dt = ( DataTable )Page.Cache [cachename];
			}
			else
			{
				dt = YAF.Classes.Data.DB.forum_listall_sorted( PageContext.PageBoardID, PageContext.PageUserID );
				Page.Cache [cachename] = dt;
			}

			//VS 2005 likes this more
			ClientScriptManager cs = Page.ClientScript;
			writer.WriteLine( String.Format( @"<select name=""{0}"" onchange=""{1}"" id=""{2}"">", this.UniqueID, cs.GetPostBackEventReference( this, this.ID ), this.ClientID ) );

			int forumID = PageContext.PageForumID;
			if ( forumID <= 0 )
				writer.WriteLine( "<option/>" );

			foreach ( DataRow row in dt.Rows )
			{
				writer.WriteLine( string.Format( @"<option {2}value=""{0}"">{1}</option>", row ["ForumID"], HtmlEncode(row ["Title"]), Convert.ToString( row ["ForumID"] ) == forumID.ToString() ? @"selected=""selected"" " : "" ) );
			}

			writer.WriteLine( "</select>" );
		}
	}
}
