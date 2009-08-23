/* Yet Another Forum.net
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
using System.Web;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
	public class YafAvatars
	{
		public string GetAvatarUrlForUser( int userId )
		{
			CombinedUserDataHelper userData = new CombinedUserDataHelper( userId );
			return GetAvatarUrlForUser( userData );
		}

		public string GetAvatarUrlForUser( CombinedUserDataHelper userData )
		{
			string avatarUrl = string.Empty;

			if ( YafContext.Current.BoardSettings.AvatarUpload && userData.HasAvatarImage )
			{
				avatarUrl = String.Format( "{0}resource.ashx?u={1}", YafForumInfo.ForumRoot, userData.UserID );
			}
			else if ( !String.IsNullOrEmpty( userData.Avatar ) ) // Took out PageContext.BoardSettings.AvatarRemote
			{
				avatarUrl = String.Format( "{3}resource.ashx?url={0}&width={1}&height={2}",
					HttpContext.Current.Server.UrlEncode( userData.Avatar ),
					YafContext.Current.BoardSettings.AvatarWidth,
					YafContext.Current.BoardSettings.AvatarHeight,
					YafForumInfo.ForumRoot );
			}
			//JoeOuts added 8/17/09 for Gravatar use
			else if ( YafContext.Current.BoardSettings.AvatarGravatar )
			{
				// if no avatar -- use "NoAvatar" image.
				string gravatarUrl = String.Format( @"http://www.gravatar.com/avatar/{0}.jpg?r={1}&d={2}",
				                                    StringHelper.StringToHexBytes( userData.Email ),
				                                    YafContext.Current.BoardSettings.GravatarRating,
				                                    HttpContext.Current.Server.UrlEncode( string.Format( "{0}/images/avatars/{1}",
				                                                                                         YafForumInfo.ForumBaseUrl,
				                                                                                         "NoAvatar.gif" ) ) );
																						

				avatarUrl = String.Format( "{3}resource.ashx?url={0}&width={1}&height={2}",
																	 HttpContext.Current.Server.UrlEncode( gravatarUrl ),
																	 YafContext.Current.BoardSettings.AvatarWidth,
																	 YafContext.Current.BoardSettings.AvatarHeight, YafForumInfo.ForumRoot );
			}

			return avatarUrl;			
		}

		public string GetAvatarUrlForCurrentUser()
		{
			return GetAvatarUrlForUser( YafContext.Current.CurrentUserData );
		}
	}
}
