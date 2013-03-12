/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Types.Interfaces
{
  using System;

  /// <summary>
  /// The yaf user profile interface.
  /// </summary>
  public interface IYafUserProfile
  {
    #region Properties

    /// <summary>
    /// Gets or sets AIM.
    /// </summary>
    string AIM { get; set; }

    /// <summary>
    /// Gets or sets Birthday.
    /// </summary>
    DateTime Birthday { get; set; }

    /// <summary>
    /// Gets or sets Blog.
    /// </summary>
    string Blog { get; set; }

    /// <summary>
    /// Gets or sets BlogServicePassword.
    /// </summary>
    string BlogServicePassword { get; set; }

    /// <summary>
    /// Gets or sets BlogServiceUrl.
    /// </summary>
    string BlogServiceUrl { get; set; }

    /// <summary>
    /// Gets or sets BlogServiceUsername.
    /// </summary>
    string BlogServiceUsername { get; set; }

    /// <summary>
    /// Gets or sets Gender.
    /// </summary>
    int Gender { get; set; }

    /// <summary>
    /// Gets or sets GoogleTalk.
    /// </summary>
    string GoogleTalk { get; set; }

    /// <summary>
    /// Gets or sets Homepage.
    /// </summary>
    string Homepage { get; set; }

    /// <summary>
    /// Gets or sets ICQ.
    /// </summary>
    string ICQ { get; set; }

    /// <summary>
    /// Gets or sets Facebook.
    /// </summary>
    string Facebook { get; set; }

    /// <summary>
    /// Gets or sets Twitter.
    /// </summary>
    string Twitter { get; set; }

    /// <summary>
    /// Gets or sets Twitter Id.
    /// </summary>
    string TwitterId { get; set; }

    /// <summary>
    /// Gets or sets Interests.
    /// </summary>
    string Interests { get; set; }

    /// <summary>
    /// Gets or sets Location.
    /// </summary>
    string Location { get; set; }

    /// <summary>
    /// Gets or sets Country.
    /// </summary>
    string Country { get; set; }

    /// <summary>
    /// Gets or sets Region or State(US).
    /// </summary>
    string Region { get; set; }

    /// <summary>
    /// Gets or sets a City.
    /// </summary>
    string City { get; set; }

    /// <summary>
    /// Gets or sets MSN.
    /// </summary>
    string MSN { get; set; }

    /// <summary>
    /// Gets or sets Occupation.
    /// </summary>
    string Occupation { get; set; }

    /// <summary>
    /// Gets or sets RealName.
    /// </summary>
    string RealName { get; set; }

    /// <summary>
    /// Gets or sets Skype.
    /// </summary>
    string Skype { get; set; }

    /// <summary>
    /// Gets or sets XMPP.
    /// </summary>
    string XMPP { get; set; }

    /// <summary>
    /// Gets or sets YIM.
    /// </summary>
    string YIM { get; set; }

    #endregion
  }
}