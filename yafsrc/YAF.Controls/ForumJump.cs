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
				return ( int ) ViewState ["ForumID"];
			}
			set
			{
				ViewState ["ForumID"] = value;
			}
		}

		#region IPostBackDataHandler
		public virtual bool LoadPostData( string postDataKey, System.Collections.Specialized.NameValueCollection postCollection )
		{
			int nForumID;
			try
			{
				nForumID = int.Parse( postCollection [postDataKey] );
				if ( nForumID == ForumID )
					return false;
			}
			catch ( Exception )
			{
				return false;
			}

			ForumID = nForumID;
			return true;
		}

		public virtual void RaisePostDataChangedEvent()
		{
			// Ederon : 9/4/2007
			if (ForumID > 0)
				YAF.Classes.Utils.YafBuildLink.Redirect(YAF.Classes.Utils.ForumPages.topics, "f={0}", ForumID);
			else
				YAF.Classes.Utils.YafBuildLink.Redirect(YAF.Classes.Utils.ForumPages.forum, "c={0}", -ForumID);
		}
		#endregion

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			DataTable dt;
			string cachename = String.Format( "forumjump_{0}_{1}", PageContext.User != null ? PageContext.User.UserName : "Guest", PageContext.User != null );
			if ( Page.Cache [cachename] != null )
			{
				dt = ( DataTable ) Page.Cache [cachename];
			}
			else
			{
				dt = YAF.Classes.Data.DB.forum_listall_sorted( PageContext.PageBoardID, PageContext.PageUserID );
				Page.Cache [cachename] = dt;
			}

            //VS 2005 likes this more
            ClientScriptManager cs = Page.ClientScript;
            writer.WriteLine(String.Format("<select name=\"{0}\" onchange=\"{1}\" language=\"javascript\" id=\"{0}\">", this.UniqueID, cs.GetPostBackEventReference(this, this.ID)));

			int forumID = PageContext.PageForumID;
			if ( forumID <= 0 )
				writer.WriteLine( "<option/>" );

			foreach ( DataRow row in dt.Rows )
			{
				writer.WriteLine( string.Format( "<option {2}value='{0}'>{1}</option>", row ["ForumID"], row ["Title"], Convert.ToString( row ["ForumID"] ) == forumID.ToString() ? "selected=\"selected\" " : "" ) );
			}

			writer.WriteLine( "</select>" );
		}
	}
}
