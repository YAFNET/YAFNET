/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;
using YAF.Classes.UI;

namespace YAF.Pages // YAF.Pages
{
  /// <summary>
  /// Summary description for profile.
  /// </summary>
  public partial class profile : YAF.Classes.Base.ForumPage
  {
    protected Repeater ForumAccess;

    public profile()
      : base( "PROFILE" )
    {
    }

    protected void Page_Load( object sender, System.EventArgs e )
    {
      // 20050909 CHP : BEGIN
      if ( PageContext.IsPrivate && User == null )
      {
        YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.login, "ReturnUrl={0}", Request.RawUrl );
      }
      // 20050909 CHP : END

      if ( Request.QueryString ["u"] == null )
        yaf_BuildLink.AccessDenied();

      if ( !IsPostBack )
      {
        userGroupsRow.Visible = PageContext.BoardSettings.ShowGroupsProfile || PageContext.IsAdmin;

        UpdateLast10Panel();

        BindData();
      }
    }

    private void BindData()
    {
      using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, Request.QueryString ["u"], true ) )
      {
        if ( dt.Rows.Count < 1 )
          yaf_BuildLink.AccessDenied(/*No such user exists*/);
        DataRow user = dt.Rows [0];

        // populate user information controls...
        UserName.Text = Server.HtmlEncode( user ["Name"].ToString() );
        Name.Text = Server.HtmlEncode( user ["Name"].ToString() );
        Joined.Text = String.Format( "{0}", yaf_DateTime.FormatDateLong( ( DateTime ) user ["Joined"] ) );
        LastVisit.Text = yaf_DateTime.FormatDateTime( ( DateTime ) user ["LastVisit"] );
        Rank.Text = user ["RankName"].ToString();
        Location.Text = Server.HtmlEncode( General.BadWordReplace( user ["Location"].ToString() ) );
        RealName.InnerHtml = Server.HtmlEncode( General.BadWordReplace( user ["RealName"].ToString() ) );
        Interests.InnerHtml = Server.HtmlEncode( General.BadWordReplace( user ["Interests"].ToString() ) );
        Occupation.InnerHtml = Server.HtmlEncode( General.BadWordReplace( user ["Occupation"].ToString() ) );
        Gender.InnerText = GetText( "GENDER" + user ["Gender"].ToString() );

        PageLinks.Clear();
        PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
        PageLinks.AddLink( GetText( "MEMBERS" ), YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.members ) );
        PageLinks.AddLink( Server.HtmlEncode( user ["Name"].ToString() ), "" );

        double dAllPosts = 0.0;
        if ( ( int ) user ["NumPostsForum"] > 0 )
          dAllPosts = 100.0 * ( int ) user ["NumPosts"] / ( int ) user ["NumPostsForum"];

        Stats.InnerHtml = String.Format( "{0:N0}<br/>[{1} / {2}]",
          user ["NumPosts"],
          String.Format( GetText( "NUMALL" ), dAllPosts ),
          String.Format( GetText( "NUMDAY" ), ( double ) ( int ) user ["NumPosts"] / ( int ) user ["NumDays"] )
          );

        // private messages
        Pm.Visible = User != null && PageContext.BoardSettings.AllowPrivateMessages;
        Pm.Text = GetThemeContents( "BUTTONS", "PM" );
        Pm.NavigateUrl = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.pmessage, "u={0}", user ["UserID"] );
        // email link
        Email.Visible = User != null && PageContext.BoardSettings.AllowEmailSending;
        Email.Text = GetThemeContents( "BUTTONS", "EMAIL" );
        Email.NavigateUrl = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_email, "u={0}", user ["UserID"] );
        if ( PageContext.IsAdmin ) Email.ToolTip = user ["Email"].ToString();
        Home.Visible = user ["HomePage"] != DBNull.Value;
        Home.NavigateUrl = user ["HomePage"].ToString();
        Home.Text = GetThemeContents( "BUTTONS", "WWW" );
        Blog.Visible = user ["Weblog"] != DBNull.Value;
        Blog.NavigateUrl = user ["Weblog"].ToString();
        Blog.Text = GetThemeContents( "BUTTONS", "WEBLOG" );
        Msn.Visible = User != null && user ["MSN"] != DBNull.Value;
        Msn.Text = GetThemeContents( "BUTTONS", "MSN" );
        Msn.NavigateUrl = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_email, "u={0}", user ["UserID"] );
        Yim.Visible = User != null && user ["YIM"] != DBNull.Value;
        Yim.NavigateUrl = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_yim, "u={0}", user ["UserID"] );
        Yim.Text = GetThemeContents( "BUTTONS", "YAHOO" );
        Aim.Visible = User != null && user ["AIM"] != DBNull.Value;
        Aim.Text = GetThemeContents( "BUTTONS", "AIM" );
        Aim.NavigateUrl = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_aim, "u={0}", user ["UserID"] );
        Icq.Visible = User != null && user ["ICQ"] != DBNull.Value;
        Icq.Text = GetThemeContents( "BUTTONS", "ICQ" );
        Icq.NavigateUrl = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_icq, "u={0}", user ["UserID"] );

        if ( PageContext.BoardSettings.AvatarUpload && user ["HasAvatarImage"] != null && long.Parse( user ["HasAvatarImage"].ToString() ) > 0 )
        {
          Avatar.ImageUrl = yaf_ForumInfo.ForumRoot + "resource.ashx?u=" + ( Request.QueryString ["u"] );
        }
        else if ( user ["Avatar"].ToString().Length > 0 ) // Took out PageContext.BoardSettings.AvatarRemote
        {
          Avatar.ImageUrl = String.Format( "{3}resource.ashx?url={0}&width={1}&height={2}",
            Server.UrlEncode( user ["Avatar"].ToString() ),
            PageContext.BoardSettings.AvatarWidth,
            PageContext.BoardSettings.AvatarHeight,
            yaf_ForumInfo.ForumRoot );
        }
        else
        {
          Avatar.Visible = false;
        }

        Groups.DataSource = YAF.Classes.Data.DB.usergroup_list( Request.QueryString ["u"] );

        //EmailRow.Visible = PageContext.IsAdmin;
        ModeratorInfo.Visible = PageContext.IsAdmin || PageContext.IsForumModerator;
        AdminUser.Visible = PageContext.IsAdmin;

        if ( PageContext.IsAdmin )
        {
          SignatureEditControl.InAdminPages = true;
        }

        if ( PageContext.IsAdmin || PageContext.IsForumModerator )
        {
          using ( DataTable dt2 = YAF.Classes.Data.DB.user_accessmasks( PageContext.PageBoardID, Request.QueryString ["u"] ) )
          {
            System.Text.StringBuilder html = new System.Text.StringBuilder();
            int nLastForumID = 0;
            foreach ( DataRow row in dt2.Rows )
            {
              if ( nLastForumID != Convert.ToInt32( row ["ForumID"] ) )
              {
                if ( nLastForumID != 0 )
                  html.AppendFormat( "</td></tr>" );
                html.AppendFormat( "<tr><td width='50%' class='postheader'>{0}</td><td width='50%' class='post'>", row ["ForumName"] );
                nLastForumID = Convert.ToInt32( row ["ForumID"] );
              }
              else
              {
                html.AppendFormat( ", " );
              }
              html.AppendFormat( "{0}", row ["AccessMaskName"] );
            }
            if ( nLastForumID != 0 )
              html.AppendFormat( "</td></tr>" );
            AccessMaskRow.Text = html.ToString();
          }
        }
      }

      if ( LastPosts.Visible )
      {
        LastPosts.DataSource = YAF.Classes.Data.DB.post_last10user( PageContext.PageBoardID, Request.QueryString ["u"], PageContext.PageUserID );
      }

      DataBind();
    }

    protected string FormatBody( object o )
    {
      DataRowView row = ( DataRowView ) o;
      string html = FormatMsg.FormatMessage( row ["Message"].ToString(), new MessageFlags( Convert.ToInt32( row ["Flags"] ) ) );

      if ( row ["Signature"].ToString().Length > 0 )
      {
        string sig = row ["Signature"].ToString();

        // don't allow any HTML on signatures
        MessageFlags tFlags = new MessageFlags();
        tFlags.IsHTML = false;

        sig = FormatMsg.FormatMessage( sig, tFlags );
        html += "<br/><hr noshade/>" + sig;
      }

      return html;
    }

    private void UpdateLast10Panel()
    {
      expandLast10.ImageUrl = GetCollapsiblePanelImageURL( "ProfileLast10Posts", PanelSessionState.CollapsiblePanelState.Collapsed );
      LastPosts.Visible = ( Mession.PanelState ["ProfileLast10Posts"] == PanelSessionState.CollapsiblePanelState.Expanded );
    }

    protected void expandLast10_Click( object sender, ImageClickEventArgs e )
    {
      // toggle the panel visability state...
      Mession.PanelState.TogglePanelState( "ProfileLast10Posts", PanelSessionState.CollapsiblePanelState.Collapsed );

      UpdateLast10Panel();

      BindData();
    }

    #region Web Form Designer generated code
    override protected void OnInit( EventArgs e )
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInit( e );
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {

    }
    #endregion

  }
}
