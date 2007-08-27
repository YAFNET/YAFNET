/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Security;
using System.Web.Security;
using YAF.Classes.Data;

namespace YAF.Classes.Utils
{
  public class MembershipHelper
  {
    #region method SyncUsers
    public static void SyncUsers( int PageBoardID )
    {
      string ForumURL = "forumurl";
      string ForumEmail = "forumemail";
      string ForumName = "forumname";

      using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageBoardID, DBNull.Value, true ) )
      {
        foreach ( DataRow row in dt.Rows )
        {
          if ( ( int ) row ["IsGuest"] > 0 )
            continue;

          string name = ( string ) row ["Name"];

          MembershipUser user = Membership.GetUser( name );
          if ( user == null )
          {
            string password;
            MembershipCreateStatus status;
            int retry = 0;
            do
            {
              password = Membership.GeneratePassword( 7 + retry, 1 + retry );
              user = Membership.CreateUser( name, password, ( string ) row ["Email"], "-", ( string ) row ["Password"], true, out status );
            } while ( status == MembershipCreateStatus.InvalidPassword && ++retry < 10 );

            if ( status != MembershipCreateStatus.Success )
            {
              throw new ApplicationException( string.Format( "Failed to create user {0}: {1}", name, status ) );
            }
            else
            {
              user.Comment = "Copied from Yet Another Forum.net";
              Membership.UpdateUser( user );

              /// Email generated password to user
              System.Text.StringBuilder msg = new System.Text.StringBuilder();
              msg.AppendFormat( "Hello {0}.\r\n\r\n", name );
              msg.AppendFormat( "Here is your new password: {0}\r\n\r\n", password );
              msg.AppendFormat( "Visit {0} at {1}", ForumName, ForumURL );

              YAF.Classes.Data.DB.mail_create( ForumEmail, user.Email, "Forum Upgrade", msg.ToString() );
            }
          }
          YAF.Classes.Data.DB.user_migrate( row ["UserID"], user.ProviderUserKey );


          using ( DataTable dtGroups = YAF.Classes.Data.DB.usergroup_list( row ["UserID"] ) )
          {
            foreach ( DataRow rowGroup in dtGroups.Rows )
            {
              Roles.AddUserToRole( user.UserName, rowGroup ["Name"].ToString() );
            }
          }
        }
      }
    }
    #endregion

    #region method SetupUserRoles
    static private bool BitSet( object _o, int bitmask )
    {
      int i = ( int ) _o;
      return ( i & bitmask ) != 0;
    }

    static public void SetupUserRoles( int PageBoardID, string userName )
    {
      using ( DataTable dt = YAF.Classes.Data.DB.group_list( PageBoardID, DBNull.Value ) )
      {
        foreach ( DataRow row in dt.Rows )
        {
          // see if the "Is Start" flag is set for this group
          if ( BitSet( row ["Flags"], 4 ) )
          {
            // add the user to this role...
            string roleName = row ["Name"].ToString();
            Roles.AddUserToRole( userName, roleName );
          }
        }
      }
    }
    #endregion

    #region method SyncRoles
    static public void SyncRoles( int PageBoardID )
    {
      using ( DataTable dt = YAF.Classes.Data.DB.group_list( PageBoardID, DBNull.Value ) )
      {
        foreach ( DataRow row in dt.Rows )
        {
          string name = ( string ) row ["Name"];

          if ( !Roles.RoleExists( name ) )
          {
            Roles.CreateRole( name );
          }
        }

        foreach ( string role in Roles.GetAllRoles() )
        {
          int nGroupID = 0;
          string filter = string.Format( "Name='{0}'", role );
          DataRow [] rows = dt.Select( filter );
          if ( rows.Length == 0 )
            nGroupID = ( int ) YAF.Classes.Data.DB.group_save( DBNull.Value, PageBoardID, role, false, false, false, 1 ); // TODO - select default access mask id
          else
            nGroupID = ( int ) rows [0] ["GroupID"];
        }
      }
    }
    #endregion

    #region method CreateForumUser
    public static bool CreateForumUser( MembershipUser user, int PageBoardID )
    {
      try
      {
        //SyncRoles( yaf_Context.Current.Settings.BoardID );

        int nUserID = YAF.Classes.Data.DB.user_aspnet( PageBoardID, user.UserName, user.Email, user.ProviderUserKey );
        foreach ( string role in Roles.GetRolesForUser( user.UserName ) )
          YAF.Classes.Data.DB.user_setrole( PageBoardID, user.ProviderUserKey, role );

        YAF.Classes.Data.DB.eventlog_create( DBNull.Value, user, string.Format( "Created forum user {0}", user.UserName ) );
        return true;
      }
      catch ( Exception x )
      {
        YAF.Classes.Data.DB.eventlog_create( DBNull.Value, "CreateForumUser", x );
        return false;
      }
    }
    #endregion

    #region method UpdateForumUser

    public static void UpdateForumUser( int nBoardID, MembershipUser user )
    {
      //YAF.Classes.Data.DB.user_setinfo(nBoardID, user);
      int nUserID = YAF.Classes.Data.DB.user_aspnet( nBoardID, user.UserName, user.Email, user.ProviderUserKey );
      YAF.Classes.Data.DB.user_setrole( nBoardID, user.ProviderUserKey, DBNull.Value );
      foreach ( string role in Roles.GetRolesForUser( user.UserName ) )
        YAF.Classes.Data.DB.user_setrole( nBoardID, user.ProviderUserKey, role );
    }
    #endregion
  }
}
