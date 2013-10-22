/* Yet Another Foru.NET
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

namespace YAF.Core.Services.CheckForSpam
{
    #region

    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Extensions;

    #endregion

    /// <summary>
    /// Spam Checking Class for the StopForumSpam.com API
    /// </summary>
    public class StopForumSpam : ICheckForBot
    {
        /// <summary>
        /// Checks if user is a Bot.
        /// </summary>
        /// <param name="ipAddress">The IP Address.</param>
        /// <param name="emailAddress">The email Address.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>
        /// Returns if user is a possible Bot or not
        /// </returns>
        public bool IsBot([CanBeNull] string ipAddress, [CanBeNull] string emailAddress, [CanBeNull] string userName)
        {
            string responseText;
            return this.IsBot(ipAddress, emailAddress, userName, out responseText);
        }

        /// <summary>
        /// Checks if user is a Bot.
        /// </summary>
        /// <param name="ipAddress">The IP Address.</param>
        /// <param name="emailAddress">The email Address.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="responseText">The response text.</param>
        /// <returns>
        /// Returns if user is a possible Bot or not
        /// </returns>
        public bool IsBot(
            [CanBeNull] string ipAddress,
            [CanBeNull] string emailAddress,
            [CanBeNull] string userName,
            out string responseText)
        {
            responseText = string.Empty;
            try
            {
                var url =
                    "http://www.stopforumspam.com/api?{0}{1}{2}&f=json".FormatWith(
                        ipAddress.IsSet() ? "ip={0}".FormatWith(ipAddress) : string.Empty,
                        emailAddress.IsSet() ? "&email={0}".FormatWith(emailAddress) : string.Empty,
                        userName.IsSet() ? "&username={0}".FormatWith(userName) : string.Empty);

                var webRequest = (HttpWebRequest)WebRequest.Create(url);

                var response = (HttpWebResponse)webRequest.GetResponse();

                var streamReader = new StreamReader(response.GetResponseStream());

                responseText = streamReader.ReadToEnd();

                var stopForumResponse = responseText.FromJson<StopForumSpamResponse>();

                if (!stopForumResponse.Success)
                {
                    return false;
                }

                // Match name + email address
                if (stopForumResponse.UserName.Appears && stopForumResponse.Email.Appears)
                {
                    return true;
                }

                // Match name + IP address
                if (stopForumResponse.UserName.Appears && stopForumResponse.IPAddress.Appears)
                {
                    return true;
                }

                // Match IP + email address
                return stopForumResponse.IPAddress.Appears && stopForumResponse.Email.Appears;
            }
            catch (Exception ex)
            {
                YafContext.Current.Get<ILogger>().Error(ex, "Error while Checking for Bot");

                return false;
            }
        }
    }

    /// <summary>
    /// StopForumSpam.com JSON Response Class
    /// </summary>
    [DataContract]
    public class StopForumSpamResponse
    {
        /// <summary>
        /// Gets or sets the success string.
        /// </summary>
        /// <value>
        /// The success string.
        /// </value>
        [DataMember(Name = "success")]
        public string SuccessString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [success].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [success]; otherwise, <c>false</c>.
        /// </value>
        public bool Success
        {
            get
            {
                return this.SuccessString == "1";
            }

            set
            {
                this.SuccessString = value ? "1" : "0";
            }
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [DataMember(Name = "username")]
        public UserName UserName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [DataMember(Name = "email")]
        public Email Email { get; set; }

        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        /// <value>
        /// The IP address.
        /// </value>
        [DataMember(Name = "ip")]
        public IP IPAddress { get; set; }
    }

    /// <summary>
    /// User Name Namespace
    /// </summary>
    [DataContract(Namespace = "username")]
    public class UserName
    {
        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>
        /// The frequency.
        /// </value>
        [DataMember(Name = "frequency")]
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the appears string.
        /// </summary>
        /// <value>
        /// The appears string.
        /// </value>
        [DataMember(Name = "appears")]
        public string AppearsString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [appears].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [appears]; otherwise, <c>false</c>.
        /// </value>
        public bool Appears
        {
            get
            {
                return this.AppearsString == "1";
            }

            set
            {
                this.AppearsString = value ? "1" : "0";
            }
        }
    }

    /// <summary>
    /// Email Namespace
    /// </summary>
    [DataContract(Namespace = "email")]
    public class Email
    {
        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>
        /// The frequency.
        /// </value>
        [DataMember(Name = "frequency")]
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the appears string.
        /// </summary>
        /// <value>
        /// The appears string.
        /// </value>
        [DataMember(Name = "appears")]
        public string AppearsString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [appears].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [appears]; otherwise, <c>false</c>.
        /// </value>
        public bool Appears
        {
            get
            {
                return this.AppearsString == "1";
            }

            set
            {
                this.AppearsString = value ? "1" : "0";
            }
        }
    }

    /// <summary>
    /// IP Address Namespace
    /// </summary>
    [DataContract(Namespace = "ip")]
    public class IP
    {
        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>
        /// The frequency.
        /// </value>
        [DataMember(Name = "frequency")]
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the appears string.
        /// </summary>
        /// <value>
        /// The appears string.
        /// </value>
        [DataMember(Name = "appears")]
        public string AppearsString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [appears].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [appears]; otherwise, <c>false</c>.
        /// </value>
        public bool Appears
        {
            get
            {
                return this.AppearsString == "1";
            }

            set
            {
                this.AppearsString = value ? "1" : "0";
            }
        }
    }
}