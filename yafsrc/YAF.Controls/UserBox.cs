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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.UI;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public class UserBox : BaseControl
	{
		private MessageFlags _messageFlags;
		private DataRowView _row = null;
		public DataRowView DataRow
		{
			get
			{
				return _row;
			}
			set
			{
				_row = (DataRowView)value;
				_messageFlags = ( _row != null ) ? new YAF.Classes.Data.MessageFlags( _row["Flags"] ) : new MessageFlags( 0 );
			}
		}

		protected int UserId
		{
			get
			{
				if ( DataRow != null )
					return Convert.ToInt32( DataRow["UserID"] );

				return 0;
			}
		}

		private Hashtable _pageCache = null;
		public Hashtable PageCache
		{
			get
			{
				return _pageCache;
			}
			set
			{
				_pageCache = value;
			}
		}

		private bool PostDeleted
		{
			get
			{
				return _messageFlags.IsDeleted;
			}
		}

		protected string CachedUserBox
		{
			get
			{
				if ( PageCache != null && DataRow != null )
				{
					// get cache for user boxes
					object cache = PageCache[Constants.Cache.UserBoxes];

					// is it hashtable?
					if ( cache != null && cache is Hashtable )
					{
						// get only record for user who made message being
						cache = ( (Hashtable)cache )[UserId];

						// return from cache if there is something there
						if ( cache != null && cache.ToString() != "" ) return cache.ToString();
					}
				}

				return null;
			}
			set
			{
				if ( PageCache != null && DataRow != null )
				{
					// get cache for user boxes
					object cache = PageCache[Constants.Cache.UserBoxes];

					// is it hashtable?
					if ( cache != null && cache is Hashtable )
					{
						// save userbox for user of this id to cache
						( (Hashtable)cache )[UserId] = value;
					}
					else
					{
						// create new hashtable for userbox caching
						cache = new Hashtable();

						// save userbox of this user
						( (Hashtable)cache )[UserId] = value;

						// save cache
						PageCache[Constants.Cache.UserBoxes] = cache;
					}
				}
			}
		}

		private YafUserProfile _userProfile = null;
		protected YafUserProfile UserProfile
		{
			get
			{
				if ( _userProfile == null )
				{
					// setup instance of the user profile...
					_userProfile = YafUserProfile.GetProfile( UserMembershipHelper.GetUserNameFromID( UserId ) );
				}

				return _userProfile;
			}
		}

		public UserBox()
		{

		}

		protected override void Render( HtmlTextWriter output )
		{
			output.WriteLine( String.Format( @"<div class=""yafUserBox"" id=""{0}"">", this.ClientID ) );

			string userBox = CachedUserBox;

			if ( string.IsNullOrEmpty( userBox ) )
			{
				userBox = CreateUserBox();
				// cache...
				CachedUserBox = userBox;
			}

			// output the user box info...
			output.WriteLine( userBox );

			output.WriteLine( "</div>" );
		}

		protected string CreateUserBox()
		{
			string userBox = PageContext.BoardSettings.UserBox;
			// Avatar
			userBox = MatchUserBoxAvatar( userBox );

			// User Medals
			userBox = MatchUserBoxMedals( userBox );

			// Rank Image
			userBox = MatchUserBoxRankImages( userBox );

			// Rank
			userBox = MatchUserBoxRank( userBox );

			// Groups
			userBox = MatchUserBoxGroups( userBox );

            //ThanksFrom
            userBox = MatchUserBoxThanksFrom(userBox);

            //ThanksTo
            userBox = MatchUserBoxThanksTo(userBox);

			if ( !PostDeleted )
			{
				// Ederon : 02/24/2007
				// Joined Date
				userBox = MatchUserBoxJoinedDate( userBox );

				// Posts
				userBox = MatchUserBoxPostCount( userBox );

				// Points
				userBox = MatchUserBoxPoints( userBox );

				// Location
				userBox = MatchUserBoxLocation( userBox );
			}
			else
			{
				userBox = MatchUserBoxClearAll( userBox );
			}

			return userBox;
		}

		private string MatchUserBoxClearAll( string userBox )
		{
			const string filler = "";

			Regex rx = new Regex( Constants.UserBox.JoinDate );
			userBox = rx.Replace( userBox, filler );
			rx = new Regex( Constants.UserBox.Posts );
			userBox = rx.Replace( userBox, filler );
			rx = new Regex( Constants.UserBox.Points );
			userBox = rx.Replace( userBox, filler );
			rx = new Regex( Constants.UserBox.Location );
			userBox = rx.Replace( userBox, filler );
            rx = new Regex(Constants.UserBox.ThanksFrom);
            userBox = rx.Replace(userBox, filler);
            rx = new Regex(Constants.UserBox.ThanksTo);
            userBox = rx.Replace(userBox, filler);
            return userBox;
		}

		private string MatchUserBoxLocation( string userBox )
		{
			string filler = "";
			Regex rx = new Regex( Constants.UserBox.Location );

			if ( UserProfile.Location != string.Empty )
			{
				filler = String.Format(
						PageContext.BoardSettings.UserBoxLocation,
						PageContext.Localization.GetText( "location" ),
						FormatMsg.RepairHtml( UserProfile.Location, false )
						);
			}

			// replaces template placeholder with actual location
			userBox = rx.Replace( userBox, filler );
			return userBox;
		}

		private string MatchUserBoxPoints( string userBox )
		{
			string filler = "";
			Regex rx = new Regex( Constants.UserBox.Points );

			if ( PageContext.BoardSettings.DisplayPoints )
			{
				filler = String.Format(
						PageContext.BoardSettings.UserBoxPoints,
						PageContext.Localization.GetText( "points" ),
						DataRow["Points"]
						);
			}

			// replaces template placeholder with actual points
			userBox = rx.Replace( userBox, filler );
			return userBox;
		}

		private string MatchUserBoxPostCount( string userBox )
		{
			Regex rx = new Regex( Constants.UserBox.Posts );

			string filler = String.Format(
					PageContext.BoardSettings.UserBoxPosts,
					PageContext.Localization.GetText( "posts" ),
					DataRow["Posts"]
					);

			// replaces template placeholder with actual post count
			userBox = rx.Replace( userBox, filler );
			return userBox;
		}

		private string MatchUserBoxJoinedDate( string userBox )
		{
			string filler = "";
			Regex rx = new Regex( Constants.UserBox.JoinDate );

			if ( PageContext.BoardSettings.DisplayJoinDate )
			{
				filler = String.Format(
						PageContext.BoardSettings.UserBoxJoinDate,
						PageContext.Localization.GetText( "joined" ),
						YafServices.DateTime.FormatDateShort( (DateTime)DataRow["Joined"] )
						);
			}

			// replaces template placeholder with actual join date
			userBox = rx.Replace( userBox, filler );
			return userBox;
		}

		private string MatchUserBoxGroups( string userBox )
		{
			string filler = "";
			Regex rx;
			rx = new Regex( Constants.UserBox.Groups );

			if ( PageContext.BoardSettings.ShowGroups )
			{
				System.Text.StringBuilder groupsText = new System.Text.StringBuilder( 500 );

				bool bFirst = true;
                string roleStyle = null;
				foreach ( string role in RoleMembershipHelper.GetRolesForUser( DataRow["UserName"].ToString() ) )
				{
                    // Get styles 	
	            
			    DataTable roleStyleTable =  YafContext.Current.Cache.GetItem<DataTable>( YafCache.GetBoardCacheKey( Constants.Cache.GroupRankStyles ),
                YafContext.Current.BoardSettings.ForumStatisticsCacheTimeout,
                () => YAF.Classes.Data.DB.group_rank_style(YafContext.Current.PageBoardID ) );
			       foreach( DataRow drow in roleStyleTable.Rows )
                   {
                      if ( Convert.ToInt32( drow["LegendID"] ) == 1 && drow["Style"] != null && drow["Name"].ToString() == role ) 
                      {
                        roleStyle = YAF.Classes.UI.StyleHelper.DecodeStyleByString(drow["Style"].ToString(),true);
                        break;
                      }
                   }
                    
 
					if ( bFirst )
					{
                         if ( YafContext.Current.BoardSettings.UseStyledNicks )                         
                        groupsText.AppendLine(string.Format(@"<span id=""{0}1"" runat=""server"" style =""{1}"">{0}</span>",role,roleStyle));
                         else
                        groupsText.AppendLine( role );
						bFirst = false;
					}
					else
					{
                         if ( YafContext.Current.BoardSettings.UseStyledNicks )
                        groupsText.AppendLine(string.Format(@", <span id=""{0}1"" runat=""server"" style =""{1}"">{0}</span>",role,roleStyle));
                         else
                        groupsText.AppendFormat( ", {0}", role );
					}
                   
				}

                roleStyle = null;

				filler = String.Format(
						PageContext.BoardSettings.UserBoxGroups,
						PageContext.Localization.GetText( "groups" ),
						groupsText.ToString()
						);

				// mddubs : 02/21/2009
				// Remove the space before the first comma when multiple groups exist.
				filler = filler.Replace( "\r\n,", "," );
			}

			// replaces template placeholder with actual groups
			userBox = rx.Replace( userBox, filler );
			return userBox;
		}

		private string MatchUserBoxRank( string userBox )
		{
            // Get styles 	
            string rankStyle = null;
            DataTable roleStyleTable = YafContext.Current.Cache.GetItem<DataTable>(YafCache.GetBoardCacheKey(Constants.Cache.GroupRankStyles),
            YafContext.Current.BoardSettings.ForumStatisticsCacheTimeout,
            () => YAF.Classes.Data.DB.group_rank_style(YafContext.Current.PageBoardID));
            foreach (DataRow drow in roleStyleTable.Rows)
            {
                if (Convert.ToInt32(drow["LegendID"]) == 2 && drow["Style"] != null && drow["Name"].ToString() == DataRow["RankName"].ToString())
                {
                    rankStyle = YAF.Classes.UI.StyleHelper.DecodeStyleByString(drow["Style"].ToString(), true);
                    break;
                }
            }
             string filler =null;  
           
            Regex rx = new Regex( Constants.UserBox.Rank );
            if ( YafContext.Current.BoardSettings.UseStyledNicks )
            filler = String.Format(
					PageContext.BoardSettings.UserBoxRank,
					PageContext.Localization.GetText( "rank" ),
					 string.Format(@"<span id=""{0}2"" runat=""server"" style =""{1}"">{0}</span>", DataRow["RankName"], rankStyle)
					);              
            else
			 filler = String.Format(
					PageContext.BoardSettings.UserBoxRank,
					PageContext.Localization.GetText( "rank" ),
					DataRow["RankName"]
					);

			// replaces template placeholder with actual rank
			userBox = rx.Replace( userBox, filler );
			return userBox;
		}

		private string MatchUserBoxRankImages( string userBox )
		{
			string filler = "";
			Regex rx = new Regex( Constants.UserBox.RankImage );

			if ( DataRow["RankImage"].ToString().Length > 0 )
			{
				filler = String.Format( PageContext.BoardSettings.UserBoxRankImage,
																String.Format( @"<img class=""rankimage"" src=""{0}{1}/{2}"" alt="""" />",
                                                                                             YafForumInfo.ForumRoot, YafBoardFolders.Current.Ranks, DataRow["RankImage"]));
			}

			// replaces template placeholder with actual rank image
			userBox = rx.Replace( userBox, filler );
			return userBox;
		}

		private string MatchUserBoxMedals( string userBox )
		{
			string filler = "";
			Regex rx = new Regex( Constants.UserBox.Medals );

			if ( PageContext.BoardSettings.ShowMedals )
			{
				DataTable dt = YafServices.DBBroker.UserMedals( UserId );

				System.Text.StringBuilder ribbonBar = new System.Text.StringBuilder( 500 );
				System.Text.StringBuilder medals = new System.Text.StringBuilder( 500 );

				DataRow r;
				MedalFlags f;

				int i = 0;
				int inRow = 0;

				// do ribbon bar first
				while ( dt.Rows.Count > i )
				{
					r = dt.Rows[i];
					f = new MedalFlags( r["Flags"] );

					// do only ribbon bar items first
					if ( !SqlDataLayerConverter.VerifyBool( r["OnlyRibbon"] ) ) break;

					// skip hidden medals
					if ( !f.AllowHiding || !SqlDataLayerConverter.VerifyBool( r["Hide"] ) )
					{
						if ( inRow == 3 )
						{
							// add break - only three ribbons in a row
							ribbonBar.Append( "<br />" );
							inRow = 0;
						}

						ribbonBar.AppendFormat(
								"<img src=\"{0}{6}/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}{5}\" />",
								YafForumInfo.ForumRoot,
								r["SmallRibbonURL"],
								r["SmallRibbonWidth"],
								r["SmallRibbonHeight"],
								r["Name"],
								f.ShowMessage ? String.Format( ": {0}", r["Message"] ) : "",
                                YafBoardFolders.Current.Medals
								);

						inRow++;
					}

					// move to next row
					i++;
				}

				// follow with the rest
				while ( dt.Rows.Count > i )
				{
					r = dt.Rows[i];
					f = new MedalFlags( r["Flags"] );

					// skip hidden medals
					if ( !f.AllowHiding || !SqlDataLayerConverter.VerifyBool( r["Hide"] ) )
					{
						medals.AppendFormat(
								"<img src=\"{0}{6}/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}{5}\" title=\"{4}{5}\" />",
								YafForumInfo.ForumRoot,
								r["SmallMedalURL"],
								r["SmallMedalWidth"],
								r["SmallMedalHeight"],
								r["Name"],
								f.ShowMessage ? String.Format( ": {0}", r["Message"] ) : "",
                                YafBoardFolders.Current.Medals
								);
					}

					// move to next row
					i++;
				}

				filler = String.Format(
						PageContext.BoardSettings.UserBoxMedals,
                        PageContext.Localization.GetText("MEDALS"),
						ribbonBar.ToString(),
						medals.ToString()
						);

			}

			// replaces template placeholder with actual medals
			userBox = rx.Replace( userBox, filler );

			return userBox;
		}

		private string MatchUserBoxAvatar( string userBox )
		{
			string filler = "";
			Regex rx = new Regex( Constants.UserBox.Avatar );

			if ( !PostDeleted )
			{
				string avatarUrl = YafServices.Avatar.GetAvatarUrlForUser( UserId );

				if ( !String.IsNullOrEmpty( avatarUrl ) )
				{
					filler = String.Format( PageContext.BoardSettings.UserBoxAvatar,
																	String.Format( @"<img class=""avatarimage"" src=""{0}"" alt="""" />", avatarUrl ) );
				}
			}

			// replaces template placeholder with actual avatar
			userBox = rx.Replace( userBox, filler );
			return userBox;
		}
        private string MatchUserBoxThanksFrom(string userBox)
        {
            string filler = "";
            Regex rx = new Regex(Constants.UserBox.ThanksFrom);
            filler = String.Format(
                        PageContext.BoardSettings.UserBoxThanksFrom,
                        String.Format(PageContext.Localization.GetText("thanksfrom"),
                        DB.user_getthanks_from(DataRow["UserID"]))
                        );

            // replaces template placeholder with actual thanks from
            userBox = rx.Replace(userBox, filler);
            return userBox;
        }

        private string MatchUserBoxThanksTo(string userBox)
        {
            string filler = "";
            Regex rx = new Regex(Constants.UserBox.ThanksTo);
            int[] ThanksToArray = DB.user_getthanks_to(DataRow["UserID"]);
            filler = String.Format(
                        PageContext.BoardSettings.UserBoxThanksTo,
                        String.Format(PageContext.Localization.GetText("thanksto"), ThanksToArray[0], ThanksToArray[1])
                        );

            // replaces template placeholder with actual thanks from
            userBox = rx.Replace(userBox, filler);
            return userBox;
        }
	}
}
